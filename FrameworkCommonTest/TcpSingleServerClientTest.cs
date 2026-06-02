using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common.Comm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Comm
{
    [TestClass]
    public class TcpSingleServerClientTest
    {
        private const int TimeoutMilliseconds = 3000;

        [TestMethod]
        public async Task TcpSingleServer_Receives_Data_From_TcpClient()
        {
            var port = GetAvailablePort();
            var server = new TestTcpSingleServer(port);
            var client = new TestTcpClient();
            var serverTask = server.OpenAsync();

            try
            {
                Assert.IsTrue(await client.OpenAsync(IPAddress.Loopback.ToString(), port));
                Assert.IsTrue(SpinWait.SpinUntil(() => server.HasClient, TimeoutMilliseconds));

                client.Send("client-to-server");

                Assert.IsTrue(server.Received.TryTake(out var received, TimeoutMilliseconds));
                Assert.AreEqual("client-to-server", Encoding.UTF8.GetString(received));
            }
            finally
            {
                await client.CloseAsync();
                await server.CloseAsync();
                await serverTask;
            }
        }

        [TestMethod]
        public async Task TcpClient_Receives_Data_From_TcpSingleServer()
        {
            var port = GetAvailablePort();
            var server = new TestTcpSingleServer(port);
            var client = new TestTcpClient();
            var serverTask = server.OpenAsync();

            try
            {
                Assert.IsTrue(await client.OpenAsync(IPAddress.Loopback.ToString(), port));
                Assert.IsTrue(SpinWait.SpinUntil(() => server.HasClient, TimeoutMilliseconds));
                Assert.IsTrue(await server.SendAsync(Encoding.UTF8.GetBytes("server-to-client")));

                Assert.IsTrue(client.Received.TryTake(out var received, TimeoutMilliseconds));
                Assert.AreEqual("server-to-client", Encoding.UTF8.GetString(received));
            }
            finally
            {
                await client.CloseAsync();
                await server.CloseAsync();
                await serverTask;
            }
        }

        private static int GetAvailablePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            try
            {
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        private sealed class TestTcpSingleServer : TcpListenerServer
        {
            public TestTcpSingleServer(int port) : base("TcpSingleServerTest", port)
            {
            }

            public BlockingCollection<byte[]> Received { get; } = new BlockingCollection<byte[]>();

            protected override void ParsingReceivedData(byte[] rcv)
            {
                this.Received.Add(rcv);
            }
        }

        private sealed class TestTcpClient : TCPClient
        {
            protected override int TcpPacketSize => 4096;

            public BlockingCollection<byte[]> Received { get; } = new BlockingCollection<byte[]>();

            protected override void ParseReceivedData()
            {
                while (this.ReceivedPacketQueue.Count > 0)
                {
                    this.Received.Add(this.ReceivedPacketQueue.Dequeue());
                }
            }

            protected override List<byte[]> SplitRcvDataByPacket(byte[] buffer, int size)
            {
                return new List<byte[]> { buffer.Take(size).ToArray() };
            }
        }
    }
}

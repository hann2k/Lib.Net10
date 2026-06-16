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
    /// <summary>
    /// 보안 점검 #1 (평문·무인증 TCP 서버 - 0.0.0.0 바인딩) 수정에 대한 테스트.
    /// - 기본 바인딩 주소가 127.0.0.1(루프백)인지 검증한다.
    /// - SetBindAddress / 생성자 인자로 다른 IP를 지정할 수 있는지 검증한다.
    /// - 잘못된 IP, 시점 위반 시 예외가 발생하는지 검증한다.
    /// </summary>
    [TestClass]
    public class TcpServerBindAddressTest
    {
        private const int TimeoutMilliseconds = 3000;

        // ---------------------------------------------------------------------
        // TcpListenerServer
        // ---------------------------------------------------------------------

        [TestMethod]
        public void TcpListenerServer_Default_BindAddress_Is_Loopback()
        {
            var server = new TestListenerServer(GetAvailablePort());

            Assert.AreEqual("127.0.0.1", server.BindAddressText);
        }

        [TestMethod]
        public void TcpListenerServer_SetBindAddress_Updates_BindAddress()
        {
            var server = new TestListenerServer(GetAvailablePort());

            server.SetBindAddress("0.0.0.0");

            Assert.AreEqual("0.0.0.0", server.BindAddressText);
        }

        [TestMethod]
        public void TcpListenerServer_SetBindAddress_Invalid_Throws()
        {
            var server = new TestListenerServer(GetAvailablePort());

            Assert.ThrowsException<ArgumentException>(() => server.SetBindAddress("not-an-ip"));
            Assert.ThrowsException<ArgumentException>(() => server.SetBindAddress("999.0.0.1"));
        }

        [TestMethod]
        public async Task TcpListenerServer_SetBindAddress_After_Open_Throws()
        {
            var port = GetAvailablePort();
            var server = new TestListenerServer(port);
            server.Open();

            try
            {
                Assert.ThrowsException<InvalidOperationException>(() => server.SetBindAddress("127.0.0.1"));
            }
            finally
            {
                await server.CloseAsync();
            }
        }

        [TestMethod]
        public async Task TcpListenerServer_Default_Server_Accepts_Loopback_Connection()
        {
            var port = GetAvailablePort();
            var server = new TestListenerServer(port);
            var client = new TestTcpClient();
            var serverTask = server.OpenAsync();

            try
            {
                Assert.AreEqual("127.0.0.1", server.BindAddressText);
                Assert.IsTrue(await client.OpenAsync(IPAddress.Loopback.ToString(), port));
                Assert.IsTrue(SpinWait.SpinUntil(() => server.HasClient, TimeoutMilliseconds));

                client.Send("hello-loopback");

                Assert.IsTrue(server.Received.TryTake(out var received, TimeoutMilliseconds));
                Assert.AreEqual("hello-loopback", Encoding.UTF8.GetString(received));
            }
            finally
            {
                await client.CloseAsync();
                await server.CloseAsync();
                await serverTask;
            }
        }

        [TestMethod]
        public async Task TcpListenerServer_SetBindAddress_Loopback_Then_Serves()
        {
            var port = GetAvailablePort();
            var server = new TestListenerServer(port);
            server.SetBindAddress("127.0.0.1");

            var client = new TestTcpClient();
            var serverTask = server.OpenAsync();

            try
            {
                Assert.IsTrue(await client.OpenAsync(IPAddress.Loopback.ToString(), port));
                Assert.IsTrue(SpinWait.SpinUntil(() => server.HasClient, TimeoutMilliseconds));

                client.Send("hello-explicit");

                Assert.IsTrue(server.Received.TryTake(out var received, TimeoutMilliseconds));
                Assert.AreEqual("hello-explicit", Encoding.UTF8.GetString(received));
            }
            finally
            {
                await client.CloseAsync();
                await server.CloseAsync();
                await serverTask;
            }
        }

        // ---------------------------------------------------------------------
        // TcpListenerServer2
        // ---------------------------------------------------------------------

        [TestMethod]
        public void TcpListenerServer2_Default_BindAddress_Is_Loopback()
        {
            var server = new TcpListenerServer2("Server2Test", GetAvailablePort());

            Assert.AreEqual("127.0.0.1", server.BindAddressText);
        }

        [TestMethod]
        public void TcpListenerServer2_SetBindAddress_Updates_BindAddress()
        {
            var server = new TcpListenerServer2("Server2Test", GetAvailablePort());

            server.SetBindAddress("192.168.0.10");

            Assert.AreEqual("192.168.0.10", server.BindAddressText);
        }

        [TestMethod]
        public void TcpListenerServer2_SetBindAddress_Invalid_Throws()
        {
            var server = new TcpListenerServer2("Server2Test", GetAvailablePort());

            Assert.ThrowsException<ArgumentException>(() => server.SetBindAddress("bad-ip"));
        }

        // ---------------------------------------------------------------------
        // TcpMultiServer (바인딩 IP는 생성자 인자로 지정)
        // ---------------------------------------------------------------------

        [TestMethod]
        public void TcpMultiServer_Default_BindAddress_Is_Loopback()
        {
            var server = new TestMultiServer("MultiTest", GetAvailablePort(), null);

            try
            {
                Assert.AreEqual("127.0.0.1", server.BindAddressText);
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void TcpMultiServer_InvalidBindIp_Throws()
        {
            // 잘못된 IP는 바인딩(소켓 생성) 전에 거부되어야 한다.
            Assert.ThrowsException<ArgumentException>(
                () => new TestMultiServer("MultiTest", GetAvailablePort(), null, "not-an-ip"));
        }

        // ---------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------

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

        private sealed class TestListenerServer : TcpListenerServer
        {
            public TestListenerServer(int port) : base("ListenerServerTest", port)
            {
            }

            public BlockingCollection<byte[]> Received { get; } = new BlockingCollection<byte[]>();

            protected override void ParsingReceivedData(byte[] rcv) => this.Received.Add(rcv);
        }

        private sealed class TestMultiServer : TcpMultiServer
        {
            public TestMultiServer(string id, int port, TcpTaskClient ttc, string bindIp = null)
                : base(id, port, ttc, bindIp)
            {
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

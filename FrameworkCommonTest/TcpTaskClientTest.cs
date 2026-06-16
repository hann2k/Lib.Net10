using System;
using System.Collections.Concurrent;
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
    /// 보안 점검 #2-1 (TcpTaskClient 미사용 수신버퍼 누적 제거) 및 최소 기능 정리 검증.
    /// - 내부 누적 버퍼 제거 후에도 ReceivedData 이벤트로 수신이 정상 동작하는지(회귀).
    /// - Close() 호출 / 원격 종료 시 ClientDisconnected 이벤트가 발생하는지.
    /// </summary>
    [TestClass]
    public class TcpTaskClientTest
    {
        private const int TimeoutMilliseconds = 3000;

        [TestMethod]
        public async Task ReceivedData_Event_Fires_With_Received_Bytes()
        {
            var (serverSide, remote) = await CreateConnectedPairAsync();
            var handler = new TestTaskClient(serverSide);
            var received = new BlockingCollection<byte[]>();
            handler.ReceivedData += (s, e) => received.Add(e.RcvBuff);

            try
            {
                var payload = Encoding.UTF8.GetBytes("hello-server");
                await remote.GetStream().WriteAsync(payload);

                Assert.IsTrue(received.TryTake(out var got, TimeoutMilliseconds));
                CollectionAssert.AreEqual(payload, got);
            }
            finally
            {
                handler.Close();
                remote.Close();
            }
        }

        [TestMethod]
        public async Task Close_Raises_ClientDisconnected()
        {
            var (serverSide, remote) = await CreateConnectedPairAsync();
            var handler = new TestTaskClient(serverSide);
            var disconnected = new ManualResetEventSlim(false);
            handler.ClientDisconnected += (s, e) => disconnected.Set();

            try
            {
                handler.Close();

                Assert.IsTrue(disconnected.Wait(TimeoutMilliseconds));
            }
            finally
            {
                remote.Close();
            }
        }

        [TestMethod]
        public async Task RemoteDisconnect_Raises_ClientDisconnected()
        {
            var (serverSide, remote) = await CreateConnectedPairAsync();
            var handler = new TestTaskClient(serverSide);
            var disconnected = new ManualResetEventSlim(false);
            handler.ClientDisconnected += (s, e) => disconnected.Set();

            try
            {
                remote.Close();   // 원격이 연결 종료(FIN)

                Assert.IsTrue(disconnected.Wait(TimeoutMilliseconds));
            }
            finally
            {
                handler.Close();
            }
        }

        private static async Task<(TcpClient serverSide, TcpClient remote)> CreateConnectedPairAsync()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var remote = new TcpClient();
            var acceptTask = listener.AcceptTcpClientAsync();
            await remote.ConnectAsync(IPAddress.Loopback, port);
            var serverSide = await acceptTask;

            listener.Stop();
            return (serverSide, remote);
        }

        private sealed class TestTaskClient : TcpTaskClient
        {
            public TestTaskClient(TcpClient c) : base(c, 8192)
            {
            }
        }
    }
}

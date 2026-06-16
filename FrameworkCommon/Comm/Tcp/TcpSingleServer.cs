using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Diagnostics;

using Framework.Common.Logger;
using Framework.Common.Event;
using Framework.Common.Converter;
using System.Net;
using Framework.Common.ThreadManager;

namespace Framework.Common.Comm
{
    public class TcpSocketServer
    {
        /// <summary>
        /// 서버 데이터 수신 이벤트
        /// </summary>
        public event EventHandler<ReceivedDataArgs> ReceivedData;

        private Socket Server;
        private Socket Client;

        private readonly string ServerID;
        private readonly int ServerPort;

        public TcpSocketServer(string id, int port)
        {
            this.ServerID = id;
            this.ServerPort = port;
        }

        public void Open()
        {
			var host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
			var endPoint = new IPEndPoint(ipAddress, this.ServerPort);
            this.Server = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Log.Ins.Debug($"{this.ServerID} is Opened");

            this.Server.Bind(endPoint);
            this.Server.Listen(1);

			// 클라이언트의 승인 요청을 받는 작업을 할 비동기소켓작업 객체를 생성하고,
			// 작업이 끝나면 OnAcceptCompleted()를 호출하도록 이벤트핸들러에 등록합니다.
			var eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnAcceptCompleted);

            this.RegisterAccept(eventArgs);

            
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

			// 서버는 승인요청을 받는 작업을 할 SocketAsyncEventArgs 객체와 함께
			// 클라이언트의 요청이 들어오면 승인 작업을 하도록 처리합니다.
			var pending = this.Server.AcceptAsync(args);

            Console.WriteLine($"4, {pending}");

            if (pending == false)
            {
                Console.WriteLine(5);
				this.OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                this.Client = args.AcceptSocket;

				// 데이터 수신용 SocketAsyncEventArgs 객체
				var recvArgs = new SocketAsyncEventArgs();
                recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnRecvCompleted);
                recvArgs.SetBuffer(new byte[1024], 0, 1024);
                recvArgs.UserToken = args.AcceptSocket;

				// 데이터 수신을 기다립니다.
				// 데이터 수신용 SocketAsyncEventArgs 객체가 인수로 넘어갑니다.

				this.RegisterRecv(recvArgs);
            }
            else
            {
            }

			// 새로운 승인을 기다립니다.
			// 승인작업을 가지고 온 SocketAsyncEventArgs 객체가 작업을 마치고
			// 다시 승인작업을 하러 넘어갑니다.
			this.RegisterAccept(args);
        }

        void RegisterRecv(SocketAsyncEventArgs args)
        {
			var client = args.UserToken as Socket;
			var pending = client.ReceiveAsync(args);
            if (pending == false)
            {
				this.OnRecvCompleted(null, args);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
				var recvData = System.Text.Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);

				var a = new ReceivedDataArgs {
					RcvBuff = System.Text.Encoding.UTF8.GetBytes(recvData)
                };

                this.ReceivedData?.Invoke(this, a);
            }

            // 새로운 데이터 수신을 위해 준비합니다.
            this.RegisterRecv(args);
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {

            }
        }

        public void Send(byte[] buffer, int size)
        {
			var sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnSendCompleted);
            sendArgs.SetBuffer(buffer, 0, buffer.Length);

			var pending = this.Client.SendAsync(sendArgs);

            if (pending == false)
            {
				this.OnSendCompleted(null, sendArgs);
            }
        }


		public void Send(byte[] buffer) => this.Send(buffer, buffer.Length);

		public void Send(string msg)
        {
			var buffer = ByteConverter.ToBytes(msg);

            this.Send(buffer, buffer.Length);
        }



        public void Close()
        {
            if (this.Client != null)
            {
                this.Client.Shutdown(SocketShutdown.Both);
                Log.Ins.Debug( $"{this.ServerID} is Closed");
            }
        }
    }

#if false
    public class TcpListenerServer : NoneParameterThread
    {
        /// <summary>
        /// 서버 데이터 수신 이벤트
        /// </summary>
        public event EventHandler<ReceivedDataArgs> ReceivedData;

        /// <summary>
        /// 서버 클라이언트 접속 이벤트
        /// </summary>
        public event EventHandler<ConnectionStateArgs> ClientConnected;

        private readonly string ServerID;
        private readonly int ServerPort;

        private TcpListener TcpListener;
        private NetworkStream NetworkStream;

        private TcpClient TcpClient;

        private bool Listening = true;

		public bool HasClient => this.TcpClient == null ? false : this.TcpClient.Connected;

		public TcpListenerServer(string id, int port)
        {
            this.ServerID = id;
            this.ServerPort = port;
        }

        public async void Open()
        {
            try
            {
                while (this.Listening)
                {

                    Log.Ins.Info($"TcpServer.Listening.Start({this.ServerID}) : {this.ServerPort}");

                    this.TcpListener = new TcpListener(IPAddress.Any, this.ServerPort);
                    this.TcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    this.TcpListener.Start();

                    // 비동기 
                    Log.Ins.Info($"TcpServer.Listening.Accept({this.ServerID})");
                    this.TcpClient = await this.TcpListener.AcceptTcpClientAsync();

                    Log.Ins.Info($"TcpServer.Client.Connected({this.ServerID})");
                    this.NetworkStream = this.TcpClient.GetStream();

                    this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = true });
                    // 새 쓰레드에서 처리
                    await Task.Factory.StartNew(this.AsyncTcpReceiving);

                    while (this.TcpClient.Connected)
                    {
                        Thread.Sleep(100);

						// 소켓 연결되어 있는지 검사기능 추가
						if (!this.IsClientConnected(this.TcpClient))
						{
							break;
						}
                    }

                    this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = false });
                    Log.Ins.Info($"TcpServer.Client.Disconnected({this.ServerID})");

                    this.NetworkStream.Close();
                    this.TcpClient.Close();

                    this.TcpListener.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Log.Ins.Info("TcpServer.Listening.Close()");
        }

		private bool IsClientConnected(TcpClient tcp)
		{
			Socket socket = tcp.Client;

			try
			{
				return !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
			}
			catch (SocketException ex)
			{
				Log.Ins.Exception(ex);
				return false; // 소켓 예외 발생 시 연결이 끊어진 것으로 간주
			}
		}

		protected virtual void OnClientConnected(bool c) => this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = c });


		protected virtual void ParsingReceivedData(byte[] rcv) => throw new NotImplementedException($"{this.ToString()}.ParsingReceivedData() 구현해서 사용하세요.");

		private async void AsyncTcpReceiving()
        {
			var MAX_SIZE = 4096;  // 가정

			// 비동기 수신            
			var buff = new byte[MAX_SIZE];

            while (this.TcpClient.Connected)
            {
                try
                {
                    if (this.NetworkStream.DataAvailable == true)
                    {
                        // Console.WriteLine("Rcv Wating()");
						var nbytes = await this.NetworkStream.ReadAsync(buff, 0, buff.Length).ConfigureAwait(false);
                        if (nbytes > 0)
                        {
							var e = new ReceivedDataArgs {
								RcvBuff = new byte[nbytes]
                            };
                            Array.Copy(buff, e.RcvBuff, nbytes);

							this.ParsingReceivedData( e.RcvBuff);

                            this.ReceivedData?.Invoke(this, e);
                        }
                    }
                    else
                    {
                        // Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    Log.Ins.Exception(ex);
                }
            }

            // Console.WriteLine("Rcv Finished()");

            this.NetworkStream?.Close();
            this.TcpClient?.Close();
        }

        public bool Send(byte[] buff)
        {
			var r = false;

            try
            {
                if (this.TcpClient != null && this.TcpClient.Connected)
                {
                    Console.WriteLine($"Send({ByteConverter.ToHexString(buff)}, Connected : {this.TcpClient.Connected})");

                    this.NetworkStream?.Write(buff, 0, buff.Length);

					r = true;
                }
    
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
            }

            return r;
        }

		public bool Send(string str)
		{
			return this.Send(ByteConverter.ToBytes(str));
		}


		public void Close() => this.Listening = false;

		protected override void CustomThreadRunner() => throw new NotImplementedException();
	}

#endif

    public class TcpListenerServer : NoneParameterThread
    {
        private readonly string ServerID;
        private readonly int ServerPort;
        private readonly SemaphoreSlim SendLock = new SemaphoreSlim(1, 1);

        // 보안 기본값: 외부 인터페이스(0.0.0.0)가 아닌 루프백(127.0.0.1)에만 바인딩한다.
        private IPAddress BindAddress = IPAddress.Loopback;

        private TcpListener TcpListener;
        private TcpClient TcpClient;
        private NetworkStream NetworkStream;
        private CancellationTokenSource ListeningCancellation;
        private Task ListeningTask = Task.CompletedTask;

        public event EventHandler<ReceivedDataArgs> ReceivedData;
        public event EventHandler<ConnectionStateArgs> ClientConnected;

        public bool HasClient => this.TcpClient?.Connected == true;

        /// <summary>
        /// 현재 설정된 바인딩 IP 주소. 기본값은 127.0.0.1(루프백).
        /// </summary>
        public string BindAddressText => this.BindAddress.ToString();

        public TcpListenerServer(string id, int port)
        {
            this.ServerID = id;
            this.ServerPort = port;
        }

        /// <summary>
        /// 서버가 수신 대기할 바인딩 IP 주소를 지정한다.
        /// 반드시 Open()/OpenAsync() 호출 전에 설정해야 한다.
        /// </summary>
        /// <param name="ip">바인딩할 IPv4/IPv6 주소 문자열 (예: "127.0.0.1", "0.0.0.0", "192.168.0.10")</param>
        /// <exception cref="ArgumentException">유효하지 않은 IP 주소일 때</exception>
        /// <exception cref="InvalidOperationException">이미 수신 대기 중일 때</exception>
        public void SetBindAddress(string ip)
        {
            if (!this.ListeningTask.IsCompleted)
            {
                throw new InvalidOperationException("수신 대기 중에는 바인딩 주소를 변경할 수 없습니다. Open() 호출 전에 설정하세요.");
            }

            if (!IPAddress.TryParse(ip, out var address))
            {
                throw new ArgumentException($"유효하지 않은 IP 주소입니다: '{ip}'", nameof(ip));
            }

            this.BindAddress = address;
        }

        public void Open()
        {
            if (this.ListeningTask.IsCompleted)
            {
                this.ListeningCancellation = new CancellationTokenSource();
                this.ListeningTask = this.ListenAsync(this.ListeningCancellation.Token);
            }
        }

        public async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            if (this.ListeningTask.IsCompleted)
            {
                this.ListeningCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                this.ListeningTask = this.ListenAsync(this.ListeningCancellation.Token);
            }

            await this.ListeningTask.ConfigureAwait(false);
        }

        private async Task ListenAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.TcpListener = new TcpListener(this.BindAddress, this.ServerPort);
                this.TcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this.TcpListener.Start();

                Log.Ins.Info($"TcpServer.Listening.Start({this.ServerID}) : {this.ServerPort} @ {this.BindAddress}");

                while (!cancellationToken.IsCancellationRequested)
                {
                    Log.Ins.Info($"TcpServer.Listening.Accept({this.ServerID})");
                    this.TcpClient = await this.TcpListener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                    this.NetworkStream = this.TcpClient.GetStream();

                    Log.Ins.Info($"TcpServer.Client.Connected({this.ServerID})");
                    this.OnClientConnected(true);

                    await this.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                    this.OnClientConnected(false);
                    Log.Ins.Info($"TcpServer.Client.Disconnected({this.ServerID})");
                    this.CloseClient();
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
            }
            finally
            {
                this.CloseClient();
                this.TcpListener?.Stop();
                Log.Ins.Info("TcpServer.Listening.Close()");
            }
        }

        public bool Send(byte[] buffer) => this.SendAsync(buffer).GetAwaiter().GetResult();

        public bool Send(string message) => this.Send(ByteConverter.ToBytes(message));

        public async Task<bool> SendAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            if (!this.HasClient || this.NetworkStream == null)
            {
                return false;
            }

            await this.SendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await this.NetworkStream.WriteAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                await this.NetworkStream.FlushAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
                return false;
            }
            finally
            {
                this.SendLock.Release();
            }
        }

        public void Close() => this.CloseAsync().GetAwaiter().GetResult();

        public async Task CloseAsync()
        {
            this.ListeningCancellation?.Cancel();
            this.CloseClient();
            this.TcpListener?.Stop();

            try
            {
                await this.ListeningTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }

            this.ListeningCancellation?.Dispose();
            this.ListeningCancellation = null;
        }

        protected virtual void OnClientConnected(bool connected) =>
            this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = connected });

        protected virtual void ParsingReceivedData(byte[] rcv) =>
            throw new NotImplementedException($"{this}.ParsingReceivedData() must be implemented.");

        protected override void CustomThreadRunner() =>
            throw new NotSupportedException("Use OpenAsync() instead.");

        private async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            while (!cancellationToken.IsCancellationRequested)
            {
                var count = await this.NetworkStream.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                if (count == 0)
                {
                    return;
                }

                var received = new byte[count];
                Array.Copy(buffer, received, count);

                this.ParsingReceivedData(received);
                this.ReceivedData?.Invoke(this, new ReceivedDataArgs { RcvBuff = received });
            }
        }

        private void CloseClient()
        {
            this.NetworkStream?.Close();
            this.NetworkStream = null;

            this.TcpClient?.Close();
            this.TcpClient = null;
        }
    }

	/// <summary>
	/// 1개의 클라이언트만 접속할수 있도록 한 TCP 소켓 서버
	/// </summary>
	public abstract class TcpSingleSocketServer
	{
		/// <summary>
		/// 서버 클라이언트 접속 이벤트
		/// </summary>
		public event EventHandler<ConnectionStateArgs> ClientConnected;

		/// <summary>
		/// 서버 데이터 수신 이벤트
		/// </summary>
		public event EventHandler<ReceivedDataArgs> ReceivedData;

		private readonly Socket Listiner = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
		private readonly int Port = 8888;

		protected Socket Handler = null;

		public TcpSingleSocketServer(int port) => this.Port = port;

		~TcpSingleSocketServer()
		{
			this.Listiner.Close();

			Console.WriteLine("TCP Single 서버 중단됨.");
		}

		/// <summary>
		/// 수신한 데이터를 파싱한다.
		/// </summary>
		/// <param name="rcv"></param>
		protected abstract void ReceivedDataParsing(List<byte> rcv);

		public void Listening()
		{
			var buffer = new byte[65535];

			do
			{
				try
				{
					var localEndPoint = new IPEndPoint( IPAddress.Parse( "127.0.0.1" ), this.Port );
					this.Listiner.Bind( localEndPoint );
					this.Listiner.Listen( 10 );

					Console.WriteLine("1. TCP Single 서버 시작됨.");

					var runner = true;
					this.Handler = this.Listiner.Accept();

					// 클라이언트 접속 이벤트
					this.ClientConnected?.Invoke( this, new ConnectionStateArgs { Connected = true } );

					while (runner)
					{
						Console.WriteLine("2.1 Checking RcvBuffer");
						var bytes = this.Handler.Receive( buffer );
						
						if (bytes > 0)
						{
							List<byte> rcv = this.GetReceivedDatas( buffer, bytes );
							Console.WriteLine("2.2 RCV : " + ByteConverter.ToHexString(rcv));

							this.ReceivedDataParsing( rcv );
							

							// 데이터 수신 이벤트
							this.ReceivedData?.Invoke( this, new ReceivedDataArgs { RcvBuff = rcv.ToArray() } );
						}
						else
						{
							Console.WriteLine("2.3 Rcv nothing.");
							Thread.Sleep( 10 );
						}
					}

					Console.WriteLine("3. TCP Single 서버 종료됨.");
					this.Handler.Close();
				}
				catch (Exception ex)
				{
					Log.Ins.Exception( ex );
				}
				finally
				{
					// 클라이언트 중단 이벤트
					this.ClientConnected?.Invoke( this, new ConnectionStateArgs { Connected = false } );
				}

				

			} while (true);

			// Console.WriteLine("TCP Single 서버 끝남.");
		}

		/// <summary>
		/// 수신한 데이터를 byte List로 추출한다.
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		protected List<byte> GetReceivedDatas(byte[] buf, int length)
		{
			if (length < 1)
			{
				throw new ArgumentOutOfRangeException("수신데이터의 크기는 0이하가 될 수 없습니다.");
			}

			var cut = new byte[ length ];
			Array.Copy( buf, cut, length );

			return cut.ToList();
		}
	}
}

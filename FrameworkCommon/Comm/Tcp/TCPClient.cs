using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Diagnostics;

using Framework.Common.Event;
using Framework.Common.Logger;
using Framework.Common.Converter;
using Framework.Common.ThreadManager;

namespace Framework.Common.Comm
{
    /// <summary>
    /// TCP Client 클래스. (사용안함. 코드로만 남겨둘 예정임)
    /// </summary>
    public abstract class _TCPClient
    {
        public event EventHandler<EventPingArgs> EventPing;

        /// <summary>
        /// 서버 클라이언트 접속 이벤트
        /// </summary>
        public event EventHandler<ConnectionStateArgs> ClientConnected;

        /// <summary>
        /// 서버 클라이언트 접속 종료 이벤트
        /// </summary>
        // public event EventHandler<ConnectionStateArgs> ClientDisconnected;

		/// <summary>
		/// TCP 포트의 최대 크기
		/// </summary>
		public const int MAX_PORT = 65535;

		/// <summary>
		/// TCP 포트의 최소 크기
		/// </summary>
		public const int MIN_PORT = 1;

		/// <summary>
		/// Ping 응답 있음
		/// </summary>
		private const int NETWORK_READY = 4010;

        /// <summary>
        /// Ping 응답 없음
        /// </summary>
        private const int NETWORK_NOK = 4011;

        /// <summary>
        /// 네트워크 연결 안됨
        /// </summary>
        private const int NETWORK_DISCONNECT = 4012;

        /// <summary>
        /// 클라이언트 식별자.
        /// </summary>
        private string ClientID = string.Empty;

        private readonly object TcpClientLock = new object();
        private readonly Ping Tcping = new Ping();

        private TcpClient TcpClient = new TcpClient();
        private NetworkStream Ns = null;

        private string ServerIP = string.Empty;
        private int ServerPort = 10000;

        private bool RunPing = true;

        /// <summary>
        /// 수신 기록 허용 플래그
        /// </summary>
        protected bool RcvLogging = true;

		protected abstract int TcpPacketSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPClient"/> class.
        /// </summary>
        public _TCPClient() : base()
        {
			// 핑 타이머 설정
			this.PingThread = new Thread(new ThreadStart(this.PingTimer_Elapsed)) {
				IsBackground = true
			};

			// 스레드 방식으로 동작
			// base.RunMode = false;

            // base.Wait();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPClient"/> class.
        /// </summary>
        public _TCPClient( string sender ) : this()
        {
            // this.ClientID = sender;
            this.SetClientID(sender);
        }

        public void StartPing()
        {
            // 핑타이머 시작
            // this.PingThread.Start();
        }

		#region 접속전 설정정보

		/// <summary>
		/// TCP 클라이언트 식별자를 설정한다.
		/// </summary>
		/// <param name="id"></param>
		public void SetClientID(string id) => this.ClientID = id;

		public void SetConnectionTimeout(int second) => this.ConnectionTimeout = second;

		public void SetIPAddress(string ip, int port)
        {
            this.ServerIP = ip;
            this.ServerPort = port;

            Log.Ins.Debug($"{this.ClientID}.IP:Port = {this.ServerIP}:{this.ServerPort}");
        }

		public void SetIPAddress(Uri uri) => this.SetIPAddress(uri.Host, uri.Port);

		#endregion

		#region 접속, 연결해제

		/// <summary>
		/// 접속중 플래그
		/// </summary>
		public bool IsConnected { get; private set; } = false;

        public int ConnectionTimeout { get; private set; } = 3;

		/// <summary>
		/// 접속가능 플래그<br/>
		/// IsConnected : true 일때 Don't care.<br/>
		/// IsConnected : false 일때 <br/>
		/// true : 접속가능<br/>
		/// false : 접속불가능<br/>
		/// </summary>
		public bool Connectable { get; private set; } = false;

        /// <summary>
        /// 기 저장된 서버 IP로 클라이언트 연결
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (this.ServerIP == null)
            {
                throw new ArgumentNullException("Server IP의 값이 null 입니다.");
            }

            return this.Connect(this.ServerIP, this.ServerPort);
        }

        /// <summary>
        /// TCP 클라이언트 연결
        /// </summary>
        /// <param name="ConnectionInfo">ip:port 형태의 문자열</param>
        /// <returns>true : 접속성공, false : 접속실패</returns>
        public bool Connect(string ConnectionInfo)
        {
			var sp = ConnectionInfo.Split(':');
			var address = sp[0];
			var port = StringToNumbers.ToInt32(sp[1], 0, MIN_PORT, MAX_PORT);
			return (port > 0) ? this.Connect(address, port) : false;
		}

        /// <summary>
        /// TCP 클라이언트 연결
        /// </summary>
        /// <param name="ServerIP">서버 주소.</param>
        /// <param name="ServerPort">서버 포트.</param>
        /// <returns>true : 접속성공, false : 접속실패</returns>
        public bool Connect(string ServerIP, int ServerPort)
        {
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;

            try
            {
				var isSuccess = false;

                Console.WriteLine( Log.Ins.Info($"{this.ClientID}.ServerIP : {this.ServerIP}, Port : {this.ServerPort}") );
                this.TcpClient = new TcpClient();
                IAsyncResult asyncResult = this.TcpClient.BeginConnect(this.ServerIP, this.ServerPort, null, null);
                this.TcpClient.ReceiveTimeout = 1000;

                for(var i = -1; i < this.ConnectionTimeout * 10; i++)
                {
                    if (this.TcpClient.Connected)
                    {
                        isSuccess = true;
                        break;
                    }

                    Thread.Sleep(100);
                }

                if (isSuccess == false)
                {
                    this.TcpClient.EndConnect(asyncResult);
                    return false;
                }

                this.TcpClient.EndConnect(asyncResult);

                if (isSuccess == true)
                {
                    Console.WriteLine(Log.Ins.Info($"{this.ClientID}.Open({this.ServerIP}, {this.ServerPort}) : Success") );

                    this.IsConnected = this.IsConnected;

                    this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = this.TcpClient.Connected });

                    this.Ns = this.TcpClient.GetStream();

                    // base.Resume();
                }

            }
            catch(ArgumentException e)
            {
                Log.Ins.Exception(e);
            }
            catch(SocketException e)
            {
                Log.Ins.Exception(e);
            }
            catch (Exception e)
            {
                Log.Ins.Exception(e);
            }

            return this.IsConnected;
        }

        /// <summary>
        /// 접속 종료.
        /// </summary>
        public void Disconnect()
        {
            this.StopPing();

            if (this.IsConnected == true)
            {
                Log.Ins.Debug("Client Disconnect()");
                this.IsConnected = false;
                
                this.Ns.Close();
                this.TcpClient.Close();

                // base.Suspend();
            }
        }

        #endregion

        #region 송수신


        /// <summary>
        /// byte 배열을 전송한다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <param name="logging"></param>
        /// <returns>1: 전송성공, 2: 전송실패</returns>
        public bool Send(byte[] buffer, int size)
        {
            lock (this.TcpClientLock)
            {
                if (this.IsConnected == true)
                {
                    try
                    {
                        Log.Ins.Info($"{this.ClientID}.Send : {ByteConverter.ToHexString(buffer)}");

                        this.Ns.Write(buffer, 0, size);
                        this.Ns.Flush();

                        Thread.Sleep(10);

                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.Ins.Exception(e);
                    }
                }

                return false;
            }
        }

		public bool Send(byte[] buffer) => this.Send(buffer, buffer.Length);

		public bool Send(string msg)
        {
			var buffer = ByteConverter.ToBytes(msg);

            return this.Send(buffer, buffer.Length);
        }


        /// <summary>
        /// 데이터 수신 부 처리.
        /// </summary>
        /// <param name="buffer">읽은 패킷 데이터.</param>
        /// <param name="size">패킷 데이터 크기.</param>
        protected abstract void ReceiveAnalyze(byte[] buffer, int size);

        #endregion

        #region 접속대상서버 접속가능여부(Ping)

        /// <summary>
        /// Ping 실패 갯수
        /// </summary>
        private int PingFailCount = 60;

        /// <summary>
        /// Ping을 제어할 타이머
        /// </summary>
        private readonly Thread PingThread;

        private void PingTimer_Elapsed()
        {
            while (this.RunPing)
            {
                try
                {
                    Thread.Sleep(450);
                    // 서버에 연결되어 있지 않으면
                    if (this.TcpClient == null || this.TcpClient.Connected == false)
                    {
                        // 핑 테스트 실행
                        this.PingPing();
                    }
                }
                catch (Exception ex)
                {
                    Log.Ins.Exception(ex);
                }
            }
        }

        private void PingPing()
        {
			var arg = new EventPingArgs();

            try
            {
                if (this.ServerIP != null && this.ServerIP != string.Empty)
                {

                    PingReply TPreply = this.Tcping.Send(this.ServerIP);

                    if (TPreply.Status == IPStatus.Success)
                    {
                        arg.PingState = true;
                        arg.PingMessage = $"연결가능";
                        this.Connectable = true;
                    }
                    else
                    {
                        arg.PingState = false;
                        arg.PingMessage = $"{this.ClientID} Ping : {this.PingFailCount}";

                        if (--this.PingFailCount < 1)
                        {
                            this.PingFailCount = 60;
                        }

                        this.Connectable = false;
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Log.Ins.Exception(e);
            }
            catch (Exception e)
            {
                Log.Ins.Exception(this.ClientID);
                Log.Ins.Exception(e);
            }
            finally
            {
                this.OnEventPing(arg);
                Thread.Sleep(1000);
            }
        }

		protected virtual void OnEventPing(EventPingArgs e) => this.EventPing?.Invoke(this, e);

		public void StopPing() => this.RunPing = false;

		#endregion


		public override string ToString() => $"{this.ClientID}.{this.ServerIP}:{this.ServerPort}";

		//public void SetRcvLogging(bool onoff)
		//{
		//    this.RcvLogging = onoff;
		//}

		public virtual void Close() => this.RunPing = false;
	}

	/// <summary>
	/// TCP Client 클래스.
	/// </summary>
	public abstract class TCPClient
	{
		public bool SimMode { get; private set; } = false;

		public void DebugModeOn() => this.SimMode = true;

		protected readonly string ClientID = string.Empty;
		private readonly object TcpClientLock = new object();

		private TcpClient Client = null;
		private NetworkStream Nstream = null;

		private bool WaitConnect = true;

		/// <summary>
		/// 즉시 보내야 할 메시지가 있을 때 Set된다.
		/// </summary>
		private bool Urgency = false;

		/// <summary>
		/// 수신 기록 허용 플래그
		/// </summary>
		protected bool RemoteLogging = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="TCPClient"/> class.
		/// </summary>
		public TCPClient()
		{

		}

		public TCPClient(string id) : base()
		{
			this.Client = new TcpClient() {
				ReceiveTimeout = 1000,
			};

			this.ClientID = id;
		}

		/// <summary>
		/// Gets 서버의 포트번호.
		/// </summary>
		private int ServerPort = 10000;

		/// <summary>
		/// Gets 서버의 IP주소.
		/// </summary>
		private string ServerIP = string.Empty;

		public void SetLocalLogging()
		{
			this.RemoteLogging = false;
		}

		#region 연결 / 해제

		

		/// <summary>
		/// 서버 클라이언트 접속 이벤트
		/// </summary>
		public event EventHandler<ConnectionStateArgs> ClientConnected;

		/// <summary>
		/// 서버 클라이언트 접속 종료 이벤트
		/// </summary>
		public event EventHandler<ConnectionStateArgs> ClientDisconnected;

		/// <summary>
		/// Gets a value indicating whether 접속 여부.
		/// </summary>
		public bool IsConnected { get; private set; }

		/// <summary>
		/// 연결.
		/// </summary>
		/// <param name="serverIP">서버 주소.</param>
		/// <param name="serverPort">서버 포트.</param>
		/// <returns>접속 여부.</returns>
		public bool Open(string serverIP, int serverPort) => this.Open(serverIP, serverPort, true);

		public bool Open(string serverIP, int serverPort, bool connect = true)
		{
			this.ServerIP = serverIP;
			this.ServerPort = serverPort;

			if(this.SimMode)
			{
				this.IsConnected = true;
			}
			else
			{
				try
				{
					this.WaitConnect = true;
					Log.Ins.Debug($" -------------------- {this.ClientID}.Connect()", this.RemoteLogging);

					this.Client = new TcpClient() {
						ReceiveTimeout = 1000,
					};

					this.Client.Connect(this.ServerIP, this.ServerPort);

					if (this.Client.Connected)
					{
						Log.Ins.Info($"{this.ClientID}.Connect Success", this.RemoteLogging);
						this.Nstream = this.Client.GetStream();

						this.IsConnected = true;
						this.WaitConnect = !this.IsConnected;
						this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = this.Client.Connected });
					}
					else
					{
						Log.Ins.Info($"{this.ClientID}.Connect Failed", this.RemoteLogging);
						this.Client = new TcpClient() {
							ReceiveTimeout = 1000,
						};
					}
				}
				catch (Exception ex)
				{
					Log.Ins.Exception(ex);
					this.WaitConnect = false;
					this.IsConnected = false;
				}

				if (this.IsConnected && this.Nstream != null)
				{
					Log.Ins.Info($"{this.ClientID}.Connected()", this.RemoteLogging);

					this.Sending = Task.Run(() => this.SendThreading());
					this.Recieveing = Task.Run(() => this.ReceiveThreading());
				}
			}

			return this.IsConnected;
		}

		/// <summary>
		/// 접속 종료.
		/// </summary>
		public void Close()
		{
			if (this.IsConnected == true)
			{
				while (this.FirstSendQueue.Count > 0)
				{
					Log.Ins.Debug($"즉시 전송대기 있음", this.RemoteLogging);
					Thread.Sleep(10);
				}

				if (this.Nstream != null && !this.SimMode)
				{
					this.Nstream.Close();
				}

				if (this.Client != null && !this.SimMode)
				{
					this.Client.Close();
				}

				this.IsConnected = false;
				Thread.Sleep(200);
				// this.Client.Close();
				Log.Ins.Info($"{this.ClientID}.Disconnected()");
			}
		}

		#endregion

		#region 데이터 전송

		private Task Sending;

		/// <summary>
		/// 송수신에 필요한 버퍼 사이즈. 상속받은 클래스에서 크기를 정한다.
		/// </summary>
		protected abstract int TcpPacketSize { get; }

		private readonly Queue<byte[]> SendQueue = new Queue<byte[]>();
		private readonly Queue<byte[]> FirstSendQueue = new Queue<byte[]>();

		/// <summary>
		/// 데이터 송신.
		/// </summary>
		/// <param name="buffer">송신할 데이터.</param>
		/// <param name="size">데이터 크기.</param>
		private void Send(byte[] buffer, int size, bool mode = false)
		{
			lock (this.TcpClientLock)
			{
				if (this.IsConnected == true)
				{
					try
					{
						if (this.SimMode)
						{
							var log = $"{this.ClientID}.Send(simul,{this.SendCount}) : " + ByteConverter.ToHexString(buffer, size);
							if (mode)
							{
								log += " [즉시]";
							}
							Log.Ins.Sim(log, this.RemoteLogging);
						}
						else
						{
							this.Nstream.Write(buffer, 0, size);
							this.Nstream.Flush();
							var log = $"{this.ClientID}.Send({this.SendCount}) : " + ByteConverter.ToHexString(buffer, size);
							if (mode)
							{
								log += " [즉시]";
							}
							Log.Ins.Info(log, this.RemoteLogging);

							this.SendCount++;
							if (this.SendCount > int.MaxValue - 1)
							{
								this.SendCount = 0;
							}
						}
					}
					catch (Exception ex)
					{
						Log.Ins.Exception(ex);
					}
				}
			}
		}

		/// <summary>
		/// 긴급 메시지를 처리한다.
		/// </summary>
		/// <param name="b"></param>
		public void SendFirst(byte[] b)
		{
			this.Urgency = true;

			Log.Ins.Info("급속전송 : " + ByteConverter.ToHexString(b), this.RemoteLogging);
			this.Send(b, b.Length, true);

			this.Urgency = false;
		}

		/// <summary>
		/// 우선전송 메시지를 등록한다.
		/// </summary>
		/// <param name="b"></param>
		public void Send(byte[] b)
		{
			try
			{
				lock (this.FirstSendQueue)
				{
					this.FirstSendQueue.Enqueue(b);
				}
			}
			catch (Exception ex)
			{
				Log.Ins.Exception(ex);
			}

		}

		public void Send(string str) => this.Send(ByteConverter.ToBytes(str));

		/// <summary>
		/// 일반전송 메시지를 등록한다.
		/// </summary>
		/// <param name="sendArray"></param>
		public void SendAdd(byte[] sendArray)
		{
			try
			{
				lock (this.SendQueue)
				{
					this.SendQueue.Enqueue(sendArray);
				}
			}
			catch (Exception ex)
			{
				Log.Ins.Exception(ex);
			}
		}

		protected int SendCount = 0;


		private void SendThreading()
		{
			// Log.Ins.Debug($"{this.ClientID}.SendThreading() 접속대기 시작");

			while (this.WaitConnect)
			{
				Thread.Sleep(100);
			}

			// Log.Ins.Debug($"{this.ClientID}.SendThreading() 전송모듈 시작");

			while (this.IsConnected)
			{
				try
				{
					// 긴급 메시지 처리
					if (this.Urgency)
					{
						Thread.Sleep(10);
						continue;
					}

					lock (this.FirstSendQueue)
					{
						// Log.Ins.Debug($"우선전송 메시지 처리 {this.FirstSendQueue.Count}");
						// 우선전송 메시지 처리
						if (this.FirstSendQueue.Count > 0)
						{
							var sendb = this.FirstSendQueue.Dequeue();
							this.Send(sendb, sendb.Length, true);
							continue;
						}

						// Log.Ins.Debug($"우선전송 메시지 처리 완료 {this.FirstSendQueue.Count}");
					}

					lock (this.SendQueue)
					{
						// 일상 메시지 처리
						if (this.SendQueue.Count > 0)
						{
							// Log.Ins.Debug($"일상 메시지 처리 {this.SendQueue.Count}");
							var sendb = this.SendQueue.Dequeue();
							this.Send(sendb, sendb.Length);

						}
						else
						{
							Thread.Sleep(1);
						}
					}
				}
				catch (Exception ex)
				{
					Log.Ins.Exception(ex);
				}
			}

			// Log.Ins.Debug($"{this.ClientID} 전송모듈 종료");
		}

		#endregion

		#region 데이터 수신

		private Task Recieveing;
		

		protected readonly Queue<byte[]> ReceivedPacketQueue = new Queue<byte[]>();

		protected int RcvCount = 0;

		/// <summary>
		/// 수신한 데이터를 분석한다.
		/// </summary>
		/// <param name="buffer">읽은 패킷 데이터.</param>
		/// <param name="size">패킷 데이터 크기.</param>
		protected abstract void ParseReceivedData();

		/// <summary>
		/// 수신한 데이터 덩어리를 패킷으로 분리한다.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		protected abstract List<byte[]> SplitRcvDataByPacket(byte[] buffer, int size);

		/// <summary>
		/// 수신 스레드
		/// </summary>
		private void ReceiveThreading()
		{
			var buffer = new byte[this.TcpPacketSize];

			while (this.WaitConnect)
			{
				Thread.Sleep(1);
			}

			while (this.IsConnected)
			{
				try
				{
					if (this.Nstream != null && this.Nstream.DataAvailable == true)
					{
						var readSize = this.Nstream.Read(buffer, 0, buffer.Length);

						var s = ByteConverter.ToHexString(buffer, readSize);

						Log.Ins.Info($"{this.ClientID}.Receive({this.RcvCount}, {readSize}) : {s}", this.RemoteLogging);

						this.RcvCount++;

						if (this.RcvCount > int.MaxValue - 1)
						{
							this.RcvCount = 0;
						}

						// 수신한 데이터 덩어리를 비콘 패킷으로 분리한다.
						List<byte[]> vr = this.SplitRcvDataByPacket(buffer, readSize);

						// Log.Ins.Debug($"{this.ClientID}.ReceivedPacketSaparated({readSize}) : {vr.Count}");

						// 분리한 비콘 패킷을 수신 큐에 넣는다.
						foreach (var v in vr)
						{
							this.ReceivedPacketQueue.Enqueue(v);
						}

						this.ParseReceivedData();
					}
					else
					{
						Thread.Sleep(1);
					}

				}
				catch (System.ObjectDisposedException)
				{
					Log.Ins.Warning($"{this.ClientID} 삭제된 개체어 접근을 시도했습니다.");
				}
				catch (Exception ex)
				{
					Log.Ins.Exception($"{this.ClientID} 전송모듈 Exception");
					Log.Ins.Exception(ex);

					this.ClientDisconnected?.Invoke(this, new ConnectionStateArgs { Connected = false });
				}
			}


		}

		#endregion

		#region 핑

		/// <summary>
		/// 핑 이벤트
		/// </summary>
		public event EventHandler<EventPingArgs> EventPing;

		protected virtual void OnEventPing(EventPingArgs e) => this.EventPing?.Invoke(this, e);

		#endregion

	}
}

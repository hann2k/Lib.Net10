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
    

    public class TcpListenerServer2 : NoneParameterThread
    {
		/// <summary>
		/// 수신 데이터 버퍼
		/// </summary>
		private readonly List<byte[]> RcvBuff = new List<byte[]>();

		/// <summary>
		/// 수신후 처리되지 않은 데이터 개수
		/// </summary>
		public int RcvCount => this.RcvBuff.Count;

		private readonly object RcvBufferLock = new object();

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

		public TcpListenerServer2(string id, int port)
        {
            this.ServerID = id;
            this.ServerPort = port;
        }

        public void Listen()
        {
            try
            {
                while (this.Listening)
                {

                    Log.Ins.Info($"TcpServer.Listening.Start({this.ServerID}) : {this.ServerPort}");

                    this.TcpListener = new TcpListener(IPAddress.Any, this.ServerPort);
                    this.TcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    this.TcpListener.Start();

                    Log.Ins.Info($"TcpServer.Listening.Accept({this.ServerID})");
                    this.TcpClient = this.TcpListener.AcceptTcpClient();

                    Log.Ins.Info($"TcpServer.Client.Connected({this.ServerID})");
                    this.NetworkStream = this.TcpClient.GetStream();

                    this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = true });

					this.TcpReceiving();

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

		private void StoreReceivedData(byte[] rcv)
		{
			lock (this.RcvBufferLock)
			{
				this.RcvBuff.Add(rcv);
			}
		}

		public List<byte[]> DequeueRcvBuff()
		{
			if (this.RcvBuff.Count > 0)
			{
				var rcv = new List<byte[]>();

				lock (this.RcvBufferLock)
				{
					rcv.AddRange(this.RcvBuff);
					this.RcvBuff.Clear();
				}

				return rcv;
			}
			else
			{
				return null;
			}
		}

		private void TcpReceiving()
        {
			var MAX_SIZE = (int)Math.Pow(2, 16);  // 가정

			// 비동기 수신            
			var buff = new byte[MAX_SIZE];

            while (this.TcpClient.Connected)
            {
                try
                {
                    if (this.NetworkStream.DataAvailable == true)
                    {
                        // Console.WriteLine("Rcv Wating()");
						var nbytes = this.NetworkStream.Read(buff, 0, buff.Length);
                        if (nbytes > 0)
                        {
							var e = new ReceivedDataArgs {
								RcvBuff = new byte[nbytes]
                            };
                            Array.Copy(buff, e.RcvBuff, nbytes);

							this.StoreReceivedData(e.RcvBuff);
							this.ParsingReceivedData(e.RcvBuff);

                            this.ReceivedData?.Invoke(this, e);
                        }
                    }
                    else
                    {
                        // Thread.Sleep(10);
                    }

					// 소켓 연결되어 있는지 검사기능 추가
					if (!this.IsClientConnected(this.TcpClient))
					{
						break;
					}
				}
                catch (Exception ex)
                {
                    Log.Ins.Exception(ex);
                }
            }
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
                    this.NetworkStream?.Write(buff, 0, buff.Length);
					Console.WriteLine($"Send({ByteConverter.ToHexString(buff)}, Connected : {this.TcpClient.Connected})");
					r = true;
                }
    
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
            }

            return r;
        }

		public bool Send(string str) => this.Send(ByteConverter.GetBytes(str));


		public void Close() => this.Listening = false;

		protected override void CustomThreadRunner() => throw new NotImplementedException();
	}

	
}

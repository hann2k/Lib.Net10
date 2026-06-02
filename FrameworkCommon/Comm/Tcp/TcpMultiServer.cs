using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common.Converter;
using Framework.Common.Event;

namespace Framework.Common.Comm
{
	public abstract class TcpMultiServer
	{
		/// <summary>
		/// 서버 클라이언트 접속 이벤트
		/// </summary>
		public event EventHandler<ServerConnectionArgs> ClientConnected;

		private readonly string ServerID;
		private readonly int ServerPort;
		private bool Listening = true;

		private readonly TcpListener TcpListener;

		// 클라이언트 집합.
		private readonly Dictionary<int, TcpTaskClient> Clients = new Dictionary<int, TcpTaskClient>();
		private readonly Task MainTask;
		private readonly CancellationTokenSource MainTaskCon = new CancellationTokenSource();

		public TcpMultiServer(string id, int port, TcpTaskClient ttc)
		{
			this.ServerID = id;
			this.ServerPort = port;

			this.TcpListener = new TcpListener(IPAddress.Any, this.ServerPort);
			this.TcpListener.Start();

			this.MainTaskCon = new CancellationTokenSource();

			this.MainTask = Task.Run(() => {

				var seq = 0;

				while (this.Listening)
				{
					try
					{
						Console.WriteLine("새 클라이언트 대기");
						TcpClient c = this.TcpListener.AcceptTcpClient();
						ttc.SetClient(c);
						// this.Clients.Add(seq++, ttc);

						Console.WriteLine("새 클라이언트 접속");
						this.ClientConnected?.Invoke(this, new ServerConnectionArgs { ConnectedClientCount = seq });

						this.Clients.Add(seq++, ttc);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}
			}, this.MainTaskCon.Token);
		}

		public void Stop() => this.MainTaskCon.Cancel();

		private void ClientControl(int cid, Action ac)
		{
			if (this.Clients.ContainsKey(cid))
			{
				ac();
			}
		}

		/// <summary>
		/// 클라이언트 1개의 접속을 종료한다.
		/// </summary>
		/// <param name="cid"></param>
		public void Disconnect(int cid)
		{
			this.ClientControl(cid, () => {
				this.Clients[cid].Send("Close");
				this.Clients[cid].Close();
				this.Clients.Remove(cid);
			});
		}

		public void Disconnect()
		{
			foreach (TcpTaskClient c in Clients.Values)
			{
				c.Send("Close");
				c.Close();
			}
			this.Clients.Clear();
		}

		#region 전송

		/// <summary>
		/// 하나의 클라이언트에 바이트 배열을 전송한다.
		/// </summary>
		/// <param name="cid"></param>
		/// <param name="buf"></param>
		public void Send(int cid, byte[] buf)
		{
			this.ClientControl(cid, () => {
				this.Clients[cid].Send(buf);
			});
		}

		public void Send(int cid, string str) => this.Send(cid, ByteConverter.ToBytes(str));

		/// <summary>
		/// 접속해 있는 모든 클라이언트에 바이트 배열을 전송한다.
		/// </summary>
		/// <param name="buf"></param>
		public void BroadCast(byte[] buf)
		{
			foreach (TcpTaskClient c in Clients.Values)
			{
				c.Send(buf);
			}
		}

		public void BroadCast(string str) => this.BroadCast(ByteConverter.ToBytes(str));

		#endregion
	}
}

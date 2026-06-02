using Framework.Common.Converter;
using Framework.Common.Event;
using Framework.Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Common.Comm
{
	public abstract class TcpTaskClient
	{
		/// <summary>
		/// 클라이언트 접속 종료 이벤트
		/// </summary>
		public event EventHandler<ConnectionStateArgs> ClientDisconnected;

		/// <summary>
		/// 데이터 수신 이벤트
		/// </summary>
		public event EventHandler<ReceivedDataArgs> ReceivedData;

		private readonly List<byte> RcvBuffer = new List<byte>();

		private TcpClient TcpClient = new TcpClient();
		private NetworkStream Ns;
		private Task TaskIns;

		public TcpTaskClient(TcpClient c, int rcvBufSize) => this.SetClient(c, rcvBufSize);

		public void SetClient(TcpClient c, int rcvBufSize = 8192)
		{
			this.TcpClient = c;
			// (3) TcpClient 객체에서 NetworkStream을 얻어옴
			this.Ns = c.GetStream();

			this.TaskIns = Task.Run(async () => {

				var buff = new byte[rcvBufSize];

				// (4) 클라이언트가 연결을 끊을 때까지 데이타 수신
				int nbytes;
				while ((nbytes = await this.Ns.ReadAsync(buff, 0, buff.Length)) > 0)
				{
					Console.WriteLine("수신 : " + ByteConverter.ToHexString(buff, nbytes));

					var rcvs = new byte[nbytes];
					Array.Copy(buff, rcvs, nbytes);

					this.RcvBuffer.AddRange(rcvs);

					var rcvAgrs = new ReceivedDataArgs {
						RcvBuff = (byte[])rcvs.Clone()
					};

					this.ReceivedData?.Invoke(this, rcvAgrs);
				}

				// (6) 스트림과 TcpClient 객체 닫기
				this.Ns?.Close();
				c?.Close();

				this.ClientDisconnected?.Invoke(this, new ConnectionStateArgs { Connected = false });

				Console.WriteLine("새 클라이언트 종료");
			});
		}

		private void ParseReceivedDatas()
		{
			do
			{
				if (this.RcvBuffer.Count > 0)
				{
					try
					{

					}
					catch (Exception ex)
					{
						Log.Ins.Exception(ex);
					}
				}
				else
				{
					Thread.Sleep(1);
				}
			} while (true);
		}

		public void Close()
		{

		}

		/// <summary>
		/// 바이트 배열을 전송한다.
		/// </summary>
		/// <param name="buf">전송할 데이터 바이트 배열</param>
		public void Send(byte[] buf)
		{
			try
			{
				if (this.Ns.CanWrite)
				{
					this.Ns.Write(buf, 0, buf.Length);
				}
			}
			catch(Exception ex)
			{
				Log.Ins.Exception(ex);
			}
		}

		/// <summary>
		/// 문자열을 바이트 배열로 변경하여 전송한다.
		/// </summary>
		/// <param name="str"></param>
		public void Send(string str) => this.Send(ByteConverter.ToBytes(str));
	}
}

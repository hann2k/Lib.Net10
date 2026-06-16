using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using Framework.Common.Converter;
using Framework.Common.Event;
using Framework.Common.Logger;

namespace Framework.Common.Comm
{
	/// <summary>
	/// 서버가 수락(Accept)한 TcpClient를 감싸 수신 데이터를 이벤트로 전달하는 서버측 핸들러.
	/// 수신 데이터는 ReceivedData 이벤트로 전달한다. (내부 누적 버퍼는 두지 않는다.)
	///
	/// 주의: 단일 연결용 최소 구현이다. 취소토큰·송신큐 등 고급 수명관리가 필요하면
	/// 신규 비동기 핸들러로 대체할 것. (BACKLOG 참조)
	/// </summary>
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

		private TcpClient TcpClient;
		private NetworkStream Ns;
		private Task TaskIns;

		public TcpTaskClient(TcpClient c, int rcvBufSize) => this.SetClient(c, rcvBufSize);

		/// <summary>
		/// 수락된 TcpClient를 주입하고 수신 루프를 시작한다.
		/// </summary>
		/// <param name="c">연결된(Accept된) TcpClient</param>
		/// <param name="rcvBufSize">수신 버퍼 크기</param>
		public void SetClient(TcpClient c, int rcvBufSize = 8192)
		{
			this.TcpClient = c;
			this.Ns = c.GetStream();

			this.TaskIns = Task.Run(async () =>
			{
				var buff = new byte[rcvBufSize];

				try
				{
					// 클라이언트가 연결을 끊을 때까지 데이터 수신
					int nbytes;
					while ((nbytes = await this.Ns.ReadAsync(buff, 0, buff.Length)) > 0)
					{
						var rcvs = new byte[nbytes];
						Array.Copy(buff, rcvs, nbytes);

						// 수신 데이터는 이벤트로만 전달한다. (내부 누적 없음)
						this.ReceivedData?.Invoke(this, new ReceivedDataArgs { RcvBuff = rcvs });
					}
				}
				catch (Exception ex)
				{
					// 정상 종료(FIN)는 while 탈출, 비정상 종료/소켓 닫힘은 여기서 처리
					Log.Ins.Exception(ex);
				}
				finally
				{
					this.Ns?.Close();
					this.TcpClient?.Close();

					this.ClientDisconnected?.Invoke(this, new ConnectionStateArgs { Connected = false });
				}
			});
		}

		/// <summary>
		/// 연결을 종료한다. 수신 루프가 종료되며 ClientDisconnected 이벤트가 발생한다.
		/// </summary>
		public void Close()
		{
			this.Ns?.Close();
			this.TcpClient?.Close();
		}

		/// <summary>
		/// 바이트 배열을 전송한다.
		/// </summary>
		/// <param name="buf">전송할 데이터 바이트 배열</param>
		public void Send(byte[] buf)
		{
			try
			{
				if (this.Ns != null && this.Ns.CanWrite)
				{
					this.Ns.Write(buf, 0, buf.Length);
				}
			}
			catch (Exception ex)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Comm
{
    public class PowerTCPClient : TCPClient
    {
		protected override int TcpPacketSize => 2048;

		//protected override void ReceiveAnalyze(byte[] buffer, int size)
		//      {
		//          // 아무것도 하지 않는다.
		//      }

		protected override List<byte[]> SplitRcvDataByPacket(byte[] buffer, int size) => new List<byte[]>();

		protected override void ParseReceivedData()
		{
			// 아무것도 하지 않는다.
		}

		/// <summary>
		/// 전원서버에 On 명령을 보낸다.
		/// </summary>
		public void PowerOn() => base.Send("#010001\r");

		/// <summary>
		/// 전원서버에 Off 명령을 보낸다.
		/// </summary>
		public void PowerOff() => base.Send("#010002\r");
	}
}

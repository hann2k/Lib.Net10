using Framework.Common.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Logger
{
	public class BasicLogClient : TCPClient
	{
		public BasicLogClient(string id) : base(id)
		{
		}

		protected override int TcpPacketSize => 2048;

		protected override void ParseReceivedData()
		{
			
		}

		protected override List<byte[]> SplitRcvDataByPacket(byte[] buffer, int size) => new List<byte[]>();
	}
}

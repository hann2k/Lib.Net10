using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Event
{
    public class ReceivedDataArgs : EventArgs
    {
        public byte[] RcvBuff;
        public byte Cmd = 0x00;
    }

    public class ConnectionStateArgs : EventArgs
    {
        public bool Connected;
    }

	public class ServerConnectionArgs : EventArgs
	{
		public int ConnectedClientCount = 0;
	}
}

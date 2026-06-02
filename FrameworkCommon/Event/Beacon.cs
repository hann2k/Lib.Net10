using Framework.Common.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Event
{
    public class BeaconPacketEventArgs : EventArgs
    {
        public Framework_Beacon_Packet BeaconPacket;
    }
}

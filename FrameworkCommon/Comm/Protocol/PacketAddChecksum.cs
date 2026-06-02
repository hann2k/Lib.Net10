using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Comm
{
    public abstract class PacketAddChecksum : Packet
    {
        private readonly List<byte> Checksum = new List<byte>();

        protected abstract void SetChecksum();
    }
}

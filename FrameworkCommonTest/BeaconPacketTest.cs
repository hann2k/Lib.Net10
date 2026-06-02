using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Common.Comm;
using Framework.Common.Unit;
using Framework.Common.Unit.SI;

namespace Framework.Test.Common.Packet
{
    [TestClass]
    public class Framework_Unit_BeaconPacket_Test
    {
        [TestMethod]
        public void BeaconPacket_CheckMagicCode_01()
        {
            byte[] magiccode = { 0x4c, 0x55, 0x65, 0xAA };

            Assert.IsTrue(Beacon_Packet.CheckMagicCode(magiccode));
        }

        [TestMethod]
        public void BeaconPacket_CheckMagicCode_02()
        {
            byte[] magiccode = { 0x4c, 0x55, 0x65 };

            Assert.IsFalse(Beacon_Packet.CheckMagicCode(magiccode));
        }

        [TestMethod]
        public void BeaconPacket_CheckMagicCode_03()
        {
            byte[] magiccode = { 0x4c, 0x55, 0x65, 0xAA, 0, 0 };

            Assert.IsTrue(Beacon_Packet.CheckMagicCode(magiccode));
        }

    }
}

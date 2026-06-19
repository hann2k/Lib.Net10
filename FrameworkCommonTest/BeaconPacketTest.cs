using System;
using System.Collections.Generic;
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

        // ---- 보안 점검 #3: 패킷 크기 검증 / 최소 길이 가드 ----

        // 추상 Beacon_Packet의 수신 검증을 테스트하기 위한 구체 클래스.
        private sealed class TestBeaconPacket : Beacon_Packet
        {
            public bool ParseCalled { get; private set; }

            protected override byte SetCommand() => 0x01;
            protected override void Parse(byte[] rcv) => this.ParseCalled = true;
            protected override string[] PrintParts() => Array.Empty<string>();
        }

        // 매직코드(4) + 커맨드(1) + 사이즈(2, LE) + 바디. 사이즈필드 기본값 = 7 + 바디길이(총길이).
        private static byte[] BuildPacket(byte command, byte[] body, ushort? sizeOverride = null)
        {
            var size = sizeOverride ?? (ushort)(7 + body.Length);
            var p = new List<byte> { 0x4C, 0x55, 0x65, 0xAA, command };
            p.AddRange(BitConverter.GetBytes(size)); // little-endian 2바이트
            p.AddRange(body);
            return p.ToArray();
        }

        [TestMethod]
        public void SetReceivedBytes_ValidPacket_Parses_NoError()
        {
            var packet = BuildPacket(0x01, new byte[] { 0x10, 0x20, 0x30 });

            var p = new TestBeaconPacket();
            p.SetReceivedBytes(packet);

            Assert.IsFalse(p.Error.Error);
            Assert.IsTrue(p.ParseCalled);
        }

        [TestMethod]
        public void SetReceivedBytes_SizeMismatch_IsRejected_ParseNotCalled()
        {
            // 사이즈필드를 실제 총길이(10)와 다르게(99) 선언 → 검증 복원으로 거부되어야 한다.
            var packet = BuildPacket(0x01, new byte[] { 0x10, 0x20, 0x30 }, sizeOverride: 99);

            var p = new TestBeaconPacket();
            p.SetReceivedBytes(packet);

            Assert.IsTrue(p.Error.Error);
            Assert.IsFalse(p.ParseCalled);
        }

        [TestMethod]
        public void SetReceivedBytes_ShortPacket_IsRejected_NoException()
        {
            // 7바이트 미만 → 최소 길이 가드로 거부(OOB 예외 없이).
            var packet = new byte[] { 0x4C, 0x55, 0x65, 0xAA };

            var p = new TestBeaconPacket();
            p.SetReceivedBytes(packet);

            Assert.IsTrue(p.Error.Error);
            Assert.IsFalse(p.ParseCalled);
        }

        [TestMethod]
        public void SetReceivedBytes_HeaderOnly_EmptyBody_IsValid()
        {
            // 정확히 7바이트(헤더만, 바디 없음) → 정상.
            var packet = BuildPacket(0x01, Array.Empty<byte>());

            var p = new TestBeaconPacket();
            p.SetReceivedBytes(packet);

            Assert.IsFalse(p.Error.Error);
            Assert.IsTrue(p.ParseCalled);
        }

        [TestMethod]
        public void SetReceivedBytes_BadMagicCode_IsRejected_ParseNotCalled()
        {
            var packet = BuildPacket(0x01, new byte[] { 0x10, 0x20, 0x30 });
            packet[0] = 0x00; // 매직코드 훼손

            var p = new TestBeaconPacket();
            p.SetReceivedBytes(packet);

            Assert.IsTrue(p.Error.Error);
            Assert.IsFalse(p.ParseCalled);
        }
    }
}

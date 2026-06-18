using System;
using Framework.Common.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Converter
{
    /// <summary>
    /// 보안 점검 #10 (Encoding.Default → Encoding.UTF8 명시) 검증.
    /// 저장소 기본 인코딩(UTF-8 = utf8mb4 동등)으로 바이트↔문자열 왕복이 정확한지 확인한다.
    /// </summary>
    [TestClass]
    public class ByteConverterTest
    {
        [TestMethod]
        public void ToBytes_Produces_Utf8_For_Korean()
        {
            // '가' = U+AC00 → UTF-8 0xEA 0xB0 0x80
            var bytes = ByteConverter.ToBytes("가");

            CollectionAssert.AreEqual(new byte[] { 0xEA, 0xB0, 0x80 }, bytes);
        }

        [TestMethod]
        public void RoundTrip_Preserves_Korean_Ascii_Mixed()
        {
            var original = "한글 ABC 123 !@#";

            var result = ByteConverter.ToString(ByteConverter.ToBytes(original));

            Assert.AreEqual(original, result);
        }

        [TestMethod]
        public void RoundTrip_Preserves_4Byte_Characters_Emoji()
        {
            // 이모지(🚀 U+1F680)는 4바이트 문자 — MySQL utf8mb3에서는 깨지는 영역.
            var original = "deploy 🚀 완료 😀";

            var result = ByteConverter.ToString(ByteConverter.ToBytes(original));

            Assert.AreEqual(original, result);
        }

        // ---- 보안 점검 #9: ByteConverter 정수 오버플로 가드 ----

        [TestMethod]
        public void ToHexString_NormalRange_ReturnsExpectedSlice()
        {
            var buffer = new byte[] { 0x01, 0xAB, 0xFF, 0x10 };

            // offset 1부터 2바이트 → "AB FF"
            var result = ByteConverter.ToHexString(buffer, 1, 2);

            Assert.AreEqual("AB FF", result);
        }

        [TestMethod]
        public void ToHexString_OffsetPlusLengthOverflow_ThrowsArgumentOutOfRange_NotOutOfMemory()
        {
            var buffer = new byte[] { 0x01, 0x02, 0x03 };

            // length + offset 이 int 오버플로로 음수가 되어 경계검사를 우회하던 케이스.
            // 거대 배열(new byte[length]) 할당 전에 의도한 예외가 나와야 한다.
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => ByteConverter.ToHexString(buffer, 2, int.MaxValue));
        }

        [TestMethod]
        public void ToHexString_NegativeOffset_ThrowsArgumentOutOfRange()
        {
            var buffer = new byte[] { 0x01, 0x02, 0x03 };

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => ByteConverter.ToHexString(buffer, -1, 1));
        }

        [TestMethod]
        public void ToHexString_NegativeLength_ThrowsArgumentOutOfRange()
        {
            var buffer = new byte[] { 0x01, 0x02, 0x03 };

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => ByteConverter.ToHexString(buffer, 0, -1));
        }

        [TestMethod]
        public void ToHexString_LengthExceedsBuffer_ThrowsArgumentOutOfRange()
        {
            var buffer = new byte[] { 0x01, 0x02, 0x03 };

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => ByteConverter.ToHexString(buffer, 1, 5));
        }

        [TestMethod]
        public void ToHexString_LengthOverload_NegativeLength_ThrowsArgumentOutOfRange()
        {
            var buffer = new byte[] { 0x01, 0x02, 0x03 };

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => ByteConverter.ToHexString(buffer, -1));
        }

        [TestMethod]
        public void ToHexString_LengthOverload_FullLength_ReturnsWholeBuffer()
        {
            var buffer = new byte[] { 0x01, 0xAB, 0xFF };

            // buffer.Length == length 경계: 전체를 반환해야 한다(기존 동작 유지).
            var result = ByteConverter.ToHexString(buffer, 3);

            Assert.AreEqual("01 AB FF", result);
        }
    }
}

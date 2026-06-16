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
    }
}

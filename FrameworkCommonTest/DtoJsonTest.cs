using System;
using System.Text.Json;
using Framework.Common.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.DTO
{
    /// <summary>
    /// 보안 점검 #11 (Newtonsoft 제거 → System.Text.Json 이관) 검증.
    /// 핵심: 기존 Newtonsoft 출력과의 호환(이스케이프/런타임 타입/들여쓰기)을 확인한다.
    /// </summary>
    [TestClass]
    public class DtoJsonTest
    {
        [TestMethod]
        public void ToJson_Serializes_RuntimeType_Properties()
        {
            // 추상 기반 타입(Dto)이 아니라 런타임 실제 타입(SampleDto)의 속성이 나와야 한다.
            Dto dto = new SampleDto { Name = "abc", Value = 7 };

            var json = dto.ToJson();

            StringAssert.Contains(json, "\"Name\"");
            StringAssert.Contains(json, "\"Value\"");
            StringAssert.Contains(json, "abc");
            StringAssert.Contains(json, "7");
        }

        [TestMethod]
        public void ToJson_Keeps_Korean_Literal_Not_Unicode_Escaped()
        {
            // Newtonsoft처럼 한글이 \uXXXX로 이스케이프되지 않고 그대로 나와야 한다.
            var dto = new SampleDto { Name = "홍길동" };

            var json = dto.ToJson();

            StringAssert.Contains(json, "홍길동");
            Assert.IsFalse(json.Contains("\\u"), $"유니코드 이스케이프가 발생했습니다: {json}");
        }

        [TestMethod]
        public void ToJson_Keeps_SingleQuote_And_Html_Chars_Literal()
        {
            // 작은따옴표('), <, >, & 는 그대로 출력되어야 한다(UnsafeRelaxedJsonEscaping).
            var dto = new SampleDto { Name = "a'b<c>&d" };

            var json = dto.ToJson();

            StringAssert.Contains(json, "a'b<c>&d");
        }

        [TestMethod]
        public void ToJson_Escapes_DoubleQuote()
        {
            // 큰따옴표는 JSON 규약상 반드시 \" 로 이스케이프된다(Newtonsoft와 동일).
            var dto = new SampleDto { Name = "he said \"hi\"" };

            var json = dto.ToJson();

            StringAssert.Contains(json, "\\\"hi\\\"");
        }

        [TestMethod]
        public void ToJson_Is_Indented()
        {
            var dto = new SampleDto { Name = "x", Value = 1 };

            var json = dto.ToJson();

            StringAssert.Contains(json, "\n");   // 들여쓰기(Formatting.Indented 대응)
            StringAssert.Contains(json, "  ");   // 2칸 들여쓰기
        }

        [TestMethod]
        public void ToJson_Produces_Valid_Json()
        {
            var dto = new SampleDto { Name = "홍길동", Value = 42 };

            // 유효한 JSON으로 다시 파싱되는지 확인(왕복 안전성).
            using var parsed = JsonDocument.Parse(dto.ToJson());
            Assert.AreEqual("홍길동", parsed.RootElement.GetProperty("Name").GetString());
            Assert.AreEqual(42, parsed.RootElement.GetProperty("Value").GetInt32());
        }

        private sealed class SampleDto : Dto
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}

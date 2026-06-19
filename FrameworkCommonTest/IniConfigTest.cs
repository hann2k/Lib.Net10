using System;
using System.IO;
using Framework.Common.Config;
using Framework.Common.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Config
{
    /// <summary>
    /// 보안 점검 #6 (레거시 P/Invoke INI + 고정 255 버퍼) 검증.
    /// 레거시 INI_Manager(P/Invoke, StringBuilder(255))를 제거하고 INI_UTF8 경로로 일원화한 뒤,
    /// 255자를 넘는 값도 잘림 없이 정확히 읽히는지 확인한다.
    /// </summary>
    [TestClass]
    public class IniConfigTest
    {
        // IniConfig는 abstract + ReadFile() 추상 메서드를 가지므로 테스트용 구체 클래스를 둔다.
        private sealed class TestIniConfig : IniConfig
        {
            public override Dto ReadFile() => null;
        }

        private static string WriteTempIni(string content)
        {
            var path = Path.Combine(Path.GetTempPath(), "FwIniTest_" + Guid.NewGuid().ToString("N") + ".ini");
            File.WriteAllText(path, content, new System.Text.UTF8Encoding(false));
            return path;
        }

        [TestMethod]
        public void GetString_Value_Longer_Than_255_Is_Not_Truncated()
        {
            // 레거시 버퍼(255) 한계를 넘는 길이 — 일원화된 경로에서는 잘리면 안 된다.
            var longValue = new string('A', 1000);
            var path = WriteTempIni($"[S]\nbig={longValue}\n");

            try
            {
                var cfg = new TestIniConfig();
                cfg.SetAutoFile(path);

                var read = cfg.GetString("S", "big");

                Assert.AreEqual(1000, read.Length, "255자 초과 값이 잘렸다.");
                Assert.AreEqual(longValue, read);
            }
            finally
            {
                try { File.Delete(path); } catch { /* 정리 단계 예외 무시 */ }
            }
        }

        [TestMethod]
        public void GetString_Reads_Korean_Utf8_Value()
        {
            // UTF-8 한글 값(레거시 ANSI 마샬링에서 깨지던 영역)도 정확히 읽혀야 한다.
            var path = WriteTempIni("[S]\nname=한글 값 테스트 🚀\n");

            try
            {
                var cfg = new TestIniConfig();
                cfg.SetAutoFile(path);

                Assert.AreEqual("한글 값 테스트 🚀", cfg.GetString("S", "name"));
            }
            finally
            {
                try { File.Delete(path); } catch { /* 정리 단계 예외 무시 */ }
            }
        }

        // ---- 보안 점검 #4: 악성/오타 설정에 대한 파싱 견고성(크래시 방지) ----

        [TestMethod]
        public void Parse_Line_Without_Separator_Is_Skipped_And_Does_Not_Crash()
        {
            // '=' 없는 줄(garbage)이 있어도 로드가 중단되지 않고 정상 키는 읽혀야 한다.
            var path = WriteTempIni("[S]\nthis_is_garbage_line\nok=value\n");

            try
            {
                var cfg = new TestIniConfig();
                cfg.SetAutoFile(path); // 예외 없이 통과해야 함

                Assert.AreEqual("value", cfg.GetString("S", "ok"));
            }
            finally
            {
                try { File.Delete(path); } catch { /* 정리 단계 예외 무시 */ }
            }
        }

        [TestMethod]
        public void Parse_Value_Containing_Equals_Is_Preserved()
        {
            // 값 안의 '='가 잘리지 않고 보존되어야 한다(Split('=', 2)).
            var path = WriteTempIni("[S]\nconn=a=b=c\n");

            try
            {
                var cfg = new TestIniConfig();
                cfg.SetAutoFile(path);

                Assert.AreEqual("a=b=c", cfg.GetString("S", "conn"));
            }
            finally
            {
                try { File.Delete(path); } catch { /* 정리 단계 예외 무시 */ }
            }
        }

        [TestMethod]
        public void Parse_KeyValue_Before_Any_Section_Is_Skipped_Not_Crash()
        {
            // 섹션 없이 등장한 key=value는 KeyNotFound 크래시 대신 건너뛰고,
            // 이후 정상 섹션의 항목은 정상 로드되어야 한다.
            var path = WriteTempIni("orphan=1\n[S]\nok=2\n");

            try
            {
                var cfg = new TestIniConfig();
                cfg.SetAutoFile(path); // 예외 없이 통과해야 함

                Assert.AreEqual("2", cfg.GetString("S", "ok"));
            }
            finally
            {
                try { File.Delete(path); } catch { /* 정리 단계 예외 무시 */ }
            }
        }

        [TestMethod]
        public void Parse_Duplicate_Key_Keeps_First_Value()
        {
            // 중복 키는 첫 항목을 유지한다(기존 동작).
            var path = WriteTempIni("[S]\nk=first\nk=second\n");

            try
            {
                var cfg = new TestIniConfig();
                cfg.SetAutoFile(path);

                Assert.AreEqual("first", cfg.GetString("S", "k"));
            }
            finally
            {
                try { File.Delete(path); } catch { /* 정리 단계 예외 무시 */ }
            }
        }

        [TestMethod]
        public void Parse_Whitespace_And_Comment_Lines_Are_Ignored()
        {
            // 공백 줄과 '#' 주석 줄은 무시되고 정상 키만 읽혀야 한다.
            var path = WriteTempIni("[S]\n   \n# comment=ignored\nreal=ok\n");

            try
            {
                var cfg = new TestIniConfig();
                cfg.SetAutoFile(path);

                Assert.AreEqual("ok", cfg.GetString("S", "real"));
            }
            finally
            {
                try { File.Delete(path); } catch { /* 정리 단계 예외 무시 */ }
            }
        }
    }
}

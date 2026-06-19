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
    }
}

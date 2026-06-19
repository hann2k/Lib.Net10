using System;
using System.IO;
using Framework.Common.Files;
using Framework.Common.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Files
{
    /// <summary>
    /// 보안 점검 #5-1 / #5-2 검증.
    /// 정책: 절대경로는 허용하고, 상대경로의 '..' 디렉터리 탈출만 차단한다.
    /// </summary>
    [TestClass]
    public class PathSecurityTest
    {
        // ---- PathGuard 단위 테스트 ----

        [TestMethod]
        public void EnsureSafe_AbsolutePath_Is_Allowed_And_Normalized()
        {
            var abs = Path.Combine(Path.GetTempPath(), "fw_pg", "sub");

            var result = PathGuard.EnsureSafe(abs);

            Assert.AreEqual(Path.GetFullPath(abs), result);
        }

        [TestMethod]
        public void EnsureSafe_RelativeUnderBase_Is_Allowed()
        {
            // '..' 없는 상대경로는 기준 디렉터리 하위로 해석되어 허용된다.
            var result = PathGuard.EnsureSafe(@"logs\today");

            var expected = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"logs\today"));
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EnsureSafe_RelativeTraversal_Throws()
        {
            Assert.ThrowsException<ArgumentException>(
                () => PathGuard.EnsureSafe(@"..\..\..\..\..\Windows\System32\drivers\etc\hosts"));
        }

        [TestMethod]
        public void EnsureSafe_Empty_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => PathGuard.EnsureSafe(""));
            Assert.ThrowsException<ArgumentException>(() => PathGuard.EnsureSafe("   "));
        }

        // ---- Log.SetLogDir 통합 (#5-2 쓰기 경로) ----

        [TestMethod]
        public void SetLogDir_RelativeTraversal_IsRejected()
        {
            Assert.ThrowsException<ArgumentException>(
                () => Log.Ins.SetLogDir(@"..\..\..\..\..\EvilLogDir"));
        }

        [TestMethod]
        public void SetLogDir_AbsolutePath_IsAllowed_And_Created()
        {
            var dir = Path.Combine(Path.GetTempPath(), "FwLogDir_" + Guid.NewGuid().ToString("N"));

            try
            {
                Log.Ins.SetLogDir(dir); // 절대경로 허용, 디렉터리 생성

                Assert.IsTrue(Directory.Exists(dir));
            }
            finally
            {
                try { Directory.Delete(dir, true); } catch { /* 정리 단계 예외 무시 */ }
            }
        }
    }
}

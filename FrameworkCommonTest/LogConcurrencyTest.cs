using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common.Enum;
using Framework.Common.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Logger
{
    /// <summary>
    /// 보안 점검 #7 (Log 클래스 락 불일치 / 경합 조건) 검증.
    /// 큐 보호 락을 LogBufferLock 하나로 통일한 뒤, 여러 스레드가 동시에 기록해도
    /// 항목이 유실·손상되지 않고 모두 파일에 기록되는지 확인한다.
    /// </summary>
    [TestClass]
    public class LogConcurrencyTest
    {
        private static string ReadAllLogLines(string dir, out int fileCount)
        {
            var files = Directory.GetFiles(dir, "SystemLog_*.log");
            fileCount = files.Length;
            var sb = new System.Text.StringBuilder();
            foreach (var f in files)
            {
                // 백그라운드 스레드가 동시에 append 중일 수 있으므로 공유 읽기로 연다.
                using var fs = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                sb.Append(reader.ReadToEnd());
            }
            return sb.ToString();
        }

        [TestMethod]
        public void Concurrent_Writes_Are_All_Persisted_Without_Loss()
        {
            var dir = Path.Combine(Path.GetTempPath(), "FwLogTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);

            // 다른 테스트/이전 실행과 섞이지 않도록 고유 마커를 쓴다.
            var marker = "MARK_" + Guid.NewGuid().ToString("N");

            const int threads = 8;
            const int perThread = 250;
            var expected = threads * perThread;

            try
            {
                Log.Ins.SetLogDir(dir);
                Log.Ins.SetLogLevel(LogType.Debug);

                var tasks = new List<Task>();
                for (var t = 0; t < threads; t++)
                {
                    var id = t;
                    tasks.Add(Task.Run(() =>
                    {
                        for (var i = 0; i < perThread; i++)
                        {
                            Log.Ins.Info($"{marker} {id}-{i}");
                        }
                    }));
                }
                Task.WaitAll(tasks.ToArray());

                // 백그라운드 스레드가 파일로 flush할 때까지 폴링(최대 ~10초).
                var count = 0;
                for (var attempt = 0; attempt < 100; attempt++)
                {
                    var text = ReadAllLogLines(dir, out _);
                    count = CountOccurrences(text, marker);
                    if (count >= expected)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }

                Assert.AreEqual(expected, count,
                    "동시 기록된 로그 항목 일부가 유실/중복되었다 — 큐 락 경합 가능성.");
            }
            finally
            {
                try { Log.Ins.Stop(); } catch { /* 정리 단계 예외는 무시 */ }
                try { Directory.Delete(dir, true); } catch { /* 정리 단계 예외는 무시 */ }
            }
        }

        private static int CountOccurrences(string haystack, string needle)
        {
            var count = 0;
            var idx = 0;
            while ((idx = haystack.IndexOf(needle, idx, StringComparison.Ordinal)) >= 0)
            {
                count++;
                idx += needle.Length;
            }
            return count;
        }
    }
}

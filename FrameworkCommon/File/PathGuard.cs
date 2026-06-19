using System;
using System.IO;

namespace Framework.Common.Files
{
    /// <summary>
    /// 경로 검증 도우미 (보안 점검 #5-1 / #5-2).
    /// <para>
    /// 정책: <b>절대경로(완전 정규화된 경로)는 호출자 책임으로 허용</b>하고,
    /// <b>상대경로가 기준 디렉터리를 <c>..</c>로 벗어나는 경로 탐색(path traversal)만 차단</b>한다.
    /// </para>
    /// </summary>
    public static class PathGuard
    {
        /// <summary>
        /// 경로를 정규화(<see cref="Path.GetFullPath(string)"/>)한 뒤,
        /// 상대경로의 <c>..</c> 기반 디렉터리 탈출만 차단한다.
        /// 절대경로(드라이브/UNC 등 완전 정규화 경로)는 그대로 허용한다.
        /// </summary>
        /// <param name="path">검증할 경로</param>
        /// <returns>정규화된 전체 경로</returns>
        /// <exception cref="ArgumentException">
        /// 경로가 비었거나, 상대경로가 기준 디렉터리 밖으로 탈출하는 경우.
        /// </exception>
        public static string EnsureSafe(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("경로가 비어 있습니다.", nameof(path));
            }

            // 절대경로(완전 정규화 경로)는 호출자 책임으로 허용한다. 정규화만 수행.
            if (Path.IsPathFullyQualified(path))
            {
                return Path.GetFullPath(path);
            }

            // 상대경로: 기준 디렉터리에 결합해 정규화한 뒤, 기준 밖으로 벗어나면(.. 탈출) 거부한다.
            var baseDir = Path.GetFullPath(Environment.CurrentDirectory);
            var full = Path.GetFullPath(Path.Combine(baseDir, path));

            if (!IsUnder(baseDir, full))
            {
                throw new ArgumentException(
                    $"상대경로가 기준 디렉터리를 벗어납니다(경로 탐색 차단): '{path}'", nameof(path));
            }

            return full;
        }

        /// <summary>
        /// <paramref name="candidate"/>가 <paramref name="baseDir"/> 하위(또는 동일)인지 검사한다.
        /// </summary>
        private static bool IsUnder(string baseDir, string candidate)
        {
            // 끝에 구분자를 붙여 접두 일치 오판(예: C:\app vs C:\application)을 막는다.
            var normalizedBase = AppendSeparator(baseDir);
            var normalizedCandidate = AppendSeparator(candidate);

            return normalizedCandidate.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase);
        }

        private static string AppendSeparator(string p)
            => p.EndsWith(Path.DirectorySeparatorChar) ? p : p + Path.DirectorySeparatorChar;
    }
}

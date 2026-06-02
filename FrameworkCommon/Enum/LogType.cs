using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Enum
{
    public enum LogType
    {
		/// <summary>
		/// 로그자체에러
		/// </summary>
		LogError,
		/// <summary>
		/// 치명적인 오류
		/// </summary>
        Fatal,
		/// <summary>
		/// 프로그램 실행중 오류 발생
		/// </summary>
        Exception,
		/// <summary>
		/// 단순 오류
		/// </summary>
        Error,
		/// <summary>
		/// 경고
		/// </summary>
        Warning,
		/// <summary>
		/// 통신 로그
		/// </summary>
		Comm,
		/// <summary>
		/// 작동 로그
		/// </summary>
        Info,
		/// <summary>
		/// 시뮬레이션 로그
		/// </summary>
		Simul,
		/// <summary>
		/// 디버그 로그
		/// </summary>
        Debug
    }
}

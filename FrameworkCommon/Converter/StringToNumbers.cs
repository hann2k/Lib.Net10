using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Converter
{
	public class StringToNumbers
	{
		/// <summary>
		/// 문자열을 True | False 로 변환한다.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultValue">기본값 false</param>
		/// <returns></returns>
		public static bool ToBool(string str, bool defaultValue = false)
		{
			var result = bool.TryParse(str, out var bul);
			return result ? bul : defaultValue;
		}

		/// <summary>
		/// 숫자를 C언어식의 True False 로 변경한다.<br/>
		/// 0이면 False. 그외의 숫자는 모두 True
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		//public static bool ToBool(int num) => num == 0 ? false : true;
		//public static bool ToBool(uint num) => ToBool((int)num);
		//public static bool ToBool(long num) => ToBool((int)num);
		//public static bool ToBool(ulong num) => ToBool((int)num);
		//public static bool ToBool(sbyte num) => ToBool((int)num);
		//public static bool ToBool(byte num) => ToBool((int)num);
		//public static bool ToBool(short num) => ToBool((int)num);
		//public static bool ToBool(ushort num) => ToBool((int)num);
		//public static bool ToBool(float num) => ToBool((int)num);
		//public static bool ToBool(double num) => ToBool((int)num);
		//public static bool ToBool(decimal num) => ToBool((int)num);

		public static sbyte ToSByte(string str, sbyte defaultValue) => !sbyte.TryParse(str, out var num) ? defaultValue : num;

		public static sbyte ToSByte(string str, sbyte defaultValue, sbyte min, sbyte max)
		{
			var b = ToSByte(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		/// <summary>
		/// 숫자로 된 문자열을 Byte 값으로 변경한다.<br/>
		/// 변환에 실패하면 defaultValue(기본값)이 된다.
		/// </summary>
		/// <param name="str">숫자 문자열</param>
		/// <param name="defaultValue">변환 실패시 출력할 기본값</param>
		/// <returns></returns>
		public static byte ToByte(string str, byte defaultValue) => !byte.TryParse(str, out var num) ? defaultValue : num;

		/// <summary>
		/// 숫자로 된 문자열을 최대 최소값 사이의 Byte 값으로 변경한다.<br/>
		/// 범위를 벗어나면 defaultValue(기본값)이 된다.
		/// </summary>
		/// <param name="str">숫자 문자열</param>
		/// <param name="defaultValue">변환 실패시 출력할 기본값</param>
		/// <param name="min">최소값</param>
		/// <param name="max">최대값</param>
		/// <returns></returns>
		public static byte ToByte(string str, byte defaultValue, byte min, byte max)
		{
			var b = ToByte(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static short ToInt16(string str, short defaultValue) => !short.TryParse(str, out var num) ? defaultValue : num;

		public static short ToInt16(string str, short defaultValue, short min, short max)
		{
			var b = ToInt16(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static ushort ToUInt16(string str, ushort defaultValue) => !ushort.TryParse(str, out var num) ? defaultValue : num;

		public static ushort ToUInt16(string str, ushort defaultValue, ushort min, ushort max)
		{
			var b = ToUInt16(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static int ToInt32(string str, int defaultValue) => !int.TryParse(str, out var num) ? defaultValue : num;

		public static int ToInt32(string str, int defaultValue, int min, int max)
		{
			var b = ToInt32(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static uint ToUInt32(string str, uint defaultValue) => !uint.TryParse(str, out var num) ? defaultValue : num;

		public static uint ToUInt32(string str, uint defaultValue, uint min, uint max)
		{
			var b = ToUInt32(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static long ToInt64(string str, long defaultValue) => !long.TryParse(str, out var num) ? defaultValue : num;

		public static long ToInt64(string str, long defaultValue, long min, long max)
		{
			var b = ToInt64(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static float ToFloat(string str, float defaultValue) => !float.TryParse(str, out var num) ? defaultValue : num;

		public static float ToFloat(string str, float defaultValue, double min, double max)
		{
			var b = ToFloat(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static double ToDouble(string str, double defaultValue) => !double.TryParse(str, out var num) ? defaultValue : num;

		public static double ToDouble(string str, double defaultValue, double min, double max)
		{
			var b = ToDouble(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}

		public static decimal ToDecimal(string str, decimal defaultValue) => !decimal.TryParse(str, out var num) ? defaultValue : num;

		public static decimal ToDecimal(string str, decimal defaultValue, decimal min, decimal max)
		{
			var b = ToDecimal(str, defaultValue);
			return b < min || b > max ? defaultValue : b;
		}
	}
}

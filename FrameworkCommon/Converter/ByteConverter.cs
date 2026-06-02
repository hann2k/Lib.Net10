using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Converter
{
    /// <summary>
    /// Byte Array 관련 변환기.
    /// </summary>
    public class ByteConverter
    {

        /// <summary>
        /// Byte 배열을 offset 위치부터 지정된 길이(length)만큼 Hex String으로 변경한다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] buffer, int offset, int length)
        {
            if (buffer.Length < length + offset)
            {
                throw new ArgumentOutOfRangeException("원본 배열 크기가 출력할 크기보다 적습니다.");
            }
            else if (buffer.Length >= length + offset)
            {
				var bytes = new byte[length];
                Array.Copy(buffer, offset, bytes, 0, length);
                return ToHexString(bytes);
            }
            else
            {
                return ToHexString(buffer);
            }
        }

        /// <summary>
        /// Byte 배열을 처음부터 지정된 길이만큼 Hex String으로 변경한다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToHexString( byte[] buffer, int length )
        {
            if ( buffer.Length < length )
            {
                throw new ArgumentOutOfRangeException("원본 배열 크기가 출력할 크기보다 적습니다.");
            }
            else if (buffer.Length > length)
            {
				var bytes = new byte[length];
                Array.Copy(buffer, 0, bytes, 0, length);
                return ToHexString(bytes);
            }
            else
            {
                return ToHexString(buffer);
            }
        }

        /// <summary>
        /// Byte 배열을 Hex String 으로 변환한다.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString( byte[] bytes)
        {
			var hex = BitConverter.ToString(bytes);
            return hex.Replace("-", " ");
        }

		/// <summary>
		/// Byte 리스트를 Hex String 으로 변환한다.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToHexString(List<byte> byteList) => ToHexString(byteList.ToArray());

		/// <summary>
		/// String을 byte Array 로 변환한다.
		/// </summary>
		/// <param name="msg">바이트 배열로 변환할 문자열</param>
		/// <returns></returns>
		public static byte[] ToBytes(string msg)
        {
			var StrByte = Encoding.UTF8.GetBytes(msg);
            return StrByte;
        }

		/// <summary>
		/// String을 byte Array 로 변환한다.<br/>
		/// MS DLL과 호환성을 위해 추가
		/// </summary>
		/// <param name="msg">바이트 배열로 변환할 문자열</param>
		/// <returns></returns>
		public static byte[] GetBytes(string msg) => ToBytes(msg);

		/// <summary>
		/// byte Array 를 string 으로 변환한다.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToString( byte[] bytes )
        {
			var str = Encoding.Default.GetString(bytes);
            return str;
        }

        /// <summary>
        /// Byte 1개를 이진수 문자열로 변환환다. MSB -> LSB 순서로 표현된다.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToBinaryString( byte b )
        {
            var str = Convert.ToString(b, 2).PadLeft(8, '0');
            return str;
        }
    }
}

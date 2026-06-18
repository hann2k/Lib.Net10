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
            ArgumentNullException.ThrowIfNull(buffer);
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "offset은 음수일 수 없습니다.");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "length는 음수일 수 없습니다.");
            }
            // 보안 점검 #9: (offset + length)는 큰 값에서 int 오버플로로 음수가 되어 경계검사를
            // 우회할 수 있다. offset/length/buffer.Length가 모두 음이 아니므로 뺄셈으로 비교하면
            // 오버플로 없이 안전하게 검사할 수 있다. (거대 배열 할당 전에 의도한 예외를 던진다.)
            if (offset > buffer.Length - length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "원본 배열 크기가 출력할 크기보다 적습니다.");
            }

            var bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, length);
            return ToHexString(bytes);
        }

        /// <summary>
        /// Byte 배열을 처음부터 지정된 길이만큼 Hex String으로 변경한다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToHexString( byte[] buffer, int length )
        {
            ArgumentNullException.ThrowIfNull(buffer);
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "length는 음수일 수 없습니다.");
            }
            if (buffer.Length < length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "원본 배열 크기가 출력할 크기보다 적습니다.");
            }

            var bytes = new byte[length];
            Array.Copy(buffer, 0, bytes, 0, length);
            return ToHexString(bytes);
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
			// Encoding.Default는 런타임/플랫폼 의존으로 오해 소지가 있어 UTF-8을 명시한다.
			// (저장소 기본 인코딩 정책: UTF-8 = MySQL utf8mb4 동등)
			var str = Encoding.UTF8.GetString(bytes);
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

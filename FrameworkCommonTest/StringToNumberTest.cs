using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Common.Converter;
// using Framework.Common.Unit.SI;

namespace Framework.Test.Convert
{
    [TestClass]
    public class Framework_Unit_StringToNumbers_Test
    {
        [TestMethod]
		[DataRow("True", true)]
		[DataRow("False", false)]
		public void StringToNumbers_ToBool_01(string a, bool b)
        {
			var 기준 = b;
			var 실제 = StringToNumbers.ToBool(a);

            Assert.AreEqual(기준, 실제);
        }

		[TestMethod]
		[DataRow("0", 0)]
		[DataRow("128", 128)]
		[DataRow("255", 255)]
		[DataRow("256", 0)]     // byte 범위초과시 실패
		[DataRow("257", 0)]     // byte 범위초과시 실패
		[DataRow("10해석불가능숫자", 0)]
		public void StringToNumbers_ToByte_01(string a, int b)
		{
			var 기준 = (byte)b;
			var 실제 = StringToNumbers.ToByte(a, 0);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("0", 0, (byte)0, (byte)200)]
		[DataRow("128", 128, (byte)0, (byte)200)]
		[DataRow("255", 0, (byte)0, (byte)200)]		// 최대최소 범위초과시 실패
		[DataRow("256", 0, (byte)0, (byte)200)]     // byte 범위초과시 실패
		[DataRow("257", 0, (byte)0, (byte)200)]     // byte 범위초과시 실패
		[DataRow("10해석불가능숫자", 0, (byte)0, (byte)255)]
		public void StringToNumbers_ToByte_02(string a, int b, byte min, byte max)
		{
			var 기준 = (byte)b;
			var 실제 = StringToNumbers.ToByte(a, 0, min, max);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("-129", 0)]     // sbyte 범위초과시 실패
		[DataRow("-128", 128)]
		[DataRow("0", 0)]
		[DataRow("127", 127)]
		[DataRow("128", 0)]		// sbyte 범위초과시 실패
		[DataRow("10해석불가능숫자", 0)]
		[DataRow("-10해석불가능숫자", 0)]
		public void StringToNumbers_ToSByte_01(string a, int b)
		{
			var 기준 = (sbyte)b;
			var 실제 = StringToNumbers.ToSByte(a, 0);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("0", 0, (sbyte)-128, (sbyte)127)]
		[DataRow("1", 1, (sbyte)-128, (sbyte)127)]
		[DataRow("-1", -1, (sbyte)-128, (sbyte)127)]
		[DataRow("-128", -128, (sbyte)-128, (sbyte)127)]
		[DataRow("127", 127, (sbyte)-128, (sbyte)127)]
		[DataRow("-127", -127, (sbyte)-128, (sbyte)127)]
		[DataRow("128", 0, (sbyte)-128, (sbyte)127)]
		[DataRow("101", 0, (sbyte)-100, (sbyte)100)]
		[DataRow("102", 0, (sbyte)-100, (sbyte)100)]     // 최대최소 범위초과시 실패
		[DataRow("-101", 0, (sbyte)-100, (sbyte)100)]     // byte 범위초과시 실패
		[DataRow("-102", 0, (sbyte)-100, (sbyte)100)]     // byte 범위초과시 실패
		[DataRow("10해석불가능숫자", 0, (sbyte)0, (sbyte)127)]
		public void StringToNumbers_ToSByte_02(string a, int b, sbyte min, sbyte max)
		{
			var 기준 = (sbyte)b;
			var 실제 = StringToNumbers.ToSByte(a, 0, min, max);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("0", 0)]
		[DataRow("128", 128)]
		[DataRow("255", 255)]
		[DataRow("65536", 65536)]     // byte 범위초과시 실패
		[DataRow("65537", 0)]     // byte 범위초과시 실패
		[DataRow("-1", 0)]     // byte 범위초과시 실패
		[DataRow("10해석불가능숫자", 0)]
		public void StringToNumbers_ToUInt16_01(string a, int b)
		{
			var 기준 = (ushort)b;
			var 실제 = StringToNumbers.ToUInt16(a, 0);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("0", 0, ushort.MinValue, ushort.MaxValue)]
		[DataRow("65535", 65535, ushort.MinValue, ushort.MaxValue)]
		[DataRow("-1", 0, ushort.MinValue, ushort.MaxValue)]
		[DataRow("32768", 32768, ushort.MinValue, ushort.MaxValue)]
		[DataRow("30000", 30000, (ushort)10000, (ushort)50000)]
		[DataRow("9999", 0, (ushort)10000, (ushort)50000)]
		[DataRow("50001", 0, (ushort)10000, (ushort)50000)]
		[DataRow("10해석불가능숫자", 0, ushort.MinValue, ushort.MaxValue)]
		public void StringToNumbers_ToUInt16_02(string a, int b, ushort min, ushort max)
		{
			var 기준 = (ushort)b;
			var 실제 = StringToNumbers.ToUInt16(a, 0, min, max);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("0", 0)]
		[DataRow("128", 128)]
		[DataRow("255", 255)]
		[DataRow("32767", 32767)]     // byte 범위초과시 실패
		[DataRow("32768", 0)]     // byte 범위초과시 실패
		[DataRow("-1", -1)]     // byte 범위초과시 실패
		[DataRow("-32768", -32768)]     // byte 범위초과시 실패
		[DataRow("-32769", 0)]     // byte 범위초과시 실패
		[DataRow("10해석불가능숫자", 0)]
		public void StringToNumbers_ToInt16_01(string a, int b)
		{
			var 기준 = (short)b;
			var 실제 = StringToNumbers.ToInt16(a, 0);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("-32769", 0, short.MinValue, short.MaxValue)]
		[DataRow("-32768", -32768, short.MinValue, short.MaxValue)]
		[DataRow("0", 0, short.MinValue, short.MaxValue)]
		[DataRow("32767", 32767, short.MinValue, short.MaxValue)]
		[DataRow("32768", 0, short.MinValue, short.MaxValue)]

		[DataRow("-10001", 0, (short)-10000, (short)10000)]
		[DataRow("-10000", -10000, (short)-10000, (short)10000)]
		[DataRow("-5000", -5000, (short)-10000, (short)10000)]
		[DataRow("5000", 5000, (short)-10000, (short)10000)]
		[DataRow("10000", 10000, (short)-10000, (short)10000)]
		[DataRow("10001", 0, (short)-10000, (short)10000)]
		public void StringToNumbers_ToInt16_02(string a, int b, short min, short max)
		{
			var 기준 = (short)b;
			var 실제 = StringToNumbers.ToInt16(a, 0, min, max);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("-2147483649", 0)]     // byte 범위초과시 실패
		[DataRow("-2147483648", int.MinValue)]     // byte 범위초과시 실패
		[DataRow("-32768", -32768)]     // byte 범위초과시 실패
		[DataRow("-1", -1)]     // byte 범위초과시 실패
		[DataRow("0", 0)]
		[DataRow("128", 128)]
		[DataRow("255", 255)]
		[DataRow("32767", 32767)]     // byte 범위초과시 실패
		[DataRow("2147483647", int.MaxValue)]     // byte 범위초과시 실패
		[DataRow("2147483648", 0)]     // byte 범위초과시 실패
		[DataRow("10해석불가능숫자", 0)]
		public void StringToNumbers_ToInt32_01(string a, int b)
		{
			var 기준 = b;
			var 실제 = StringToNumbers.ToInt32(a, 0);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}

		[TestMethod]
		[DataRow("-2147483649", 0, -2147483648, 2147483647)]     // byte 범위초과시 실패
		[DataRow("-2147483648", int.MinValue, -2147483648, 2147483647)]     // byte 범위초과시 실패
		[DataRow("0", 0, -2147483648, 2147483647)]
		[DataRow("2147483647", int.MaxValue, -2147483648, 2147483647)]     // byte 범위초과시 실패
		[DataRow("2147483648", 0, -2147483648, 2147483647)]     // byte 범위초과시 실패
		[DataRow("-2000001", 0, -2000000, 2000000)]
		[DataRow("-2000000", -2000000, -2000000, 2000000)]
		[DataRow("-100", -100, -2000000, 2000000)]
		[DataRow("100", 100, -2000000, 2000000)]
		[DataRow("2000000", 2000000, -2000000, 2000000)]
		[DataRow("2000001", 0, -2000000, 2000000)]
		public void StringToNumbers_ToInt32_02(string a, int b, int min, int max)
		{
			var 기준 = b;
			var 실제 = StringToNumbers.ToInt32(a, 0, min, max);

			Console.WriteLine($"{기준}, {실제}");
			Assert.AreEqual(기준, 실제);
		}
	}
}

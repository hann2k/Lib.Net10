using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Common.Converter;
using Framework.Common.DTO;
using Framework.Common.Unit;
using Framework.Common.Unit.SI;

namespace Framework.Test.Unit
{
    [TestClass]
    public class Framework_Unit_BitManager_Test
    {
        [TestMethod]
        [DataRow(0,  0x00000001)]
        [DataRow(1,  0x00000002)]
        [DataRow(2,  0x00000004)]
        [DataRow(3,  0x00000008)]
        [DataRow(4,  0x00000010)]
        [DataRow(5,  0x00000020)]
        [DataRow(6,  0x00000040)]
        [DataRow(7,  0x00000080)]
        //[DataRow(8,  0x00000100)]
        //[DataRow(9,  0x00000200)]
        //[DataRow(10, 0x00000400)]
        //[DataRow(11, 0x00000800)]
        //[DataRow(12, 0x00001000)]
        //[DataRow(13, 0x00002000)]
        //[DataRow(14, 0x00004000)]
        //[DataRow(15, 0x00008000)]
        public void BitManager_생성시험_8Bit(int p, int r)
        {
            BitManager m = new BitManager();

            m.PutBool(true, p);

            byte b = m.GetUInt8;

            Assert.AreEqual(r, b);
        }

        [TestMethod]
        [DataRow(0,  0x00000001)]
        [DataRow(1,  0x00000002)]
        [DataRow(2,  0x00000004)]
        [DataRow(3,  0x00000008)]
        [DataRow(4,  0x00000010)]
        [DataRow(5,  0x00000020)]
        [DataRow(6,  0x00000040)]
        [DataRow(7,  0x00000080)]
        [DataRow(8,  0x00000100)]
        [DataRow(9,  0x00000200)]
        [DataRow(10, 0x00000400)]
        [DataRow(11, 0x00000800)]
        [DataRow(12, 0x00001000)]
        [DataRow(13, 0x00002000)]
        [DataRow(14, 0x00004000)]
        [DataRow(15, 0x00008000)]
        public void BitManager_생성시험_16Bit(int p, int r)
        {
            BitManager m = new BitManager();

            m.PutBool(true, p);

            ushort b = m.GetUInt16;

            Assert.AreEqual(r, b);
        }

        [TestMethod]
        public void BitManager_생성시험_32Bit()
        {
            uint[] masked = {
                0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080,
                0x00000100, 0x00000200, 0x00000400, 0x00000800, 0x00001000, 0x00002000, 0x00004000, 0x00008000,
                0x00010000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000,
                0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000
            };

            for (int i = 0; i < masked.Length; i++)
            {

                BitManager m = new BitManager();

                m.PutBool(true, i);

                uint b = m.GetUInt32;

                Assert.AreEqual(masked[i], b);
            }
        }

        [TestMethod]
        public void BitManager_생성시험_64Bit()
        {
            ulong[] masked = {
                0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080,
                0x00000100, 0x00000200, 0x00000400, 0x00000800, 0x00001000, 0x00002000, 0x00004000, 0x00008000,
                0x00010000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000,
                0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000,

                0x0000000100000000, 0x0000000200000000, 0x0000000400000000, 0x0000000800000000,
                0x0000001000000000, 0x0000002000000000, 0x0000004000000000, 0x0000008000000000,
                0x0000010000000000, 0x0000020000000000, 0x0000040000000000, 0x0000080000000000,
                0x0000100000000000, 0x0000200000000000, 0x0000400000000000, 0x0000800000000000,
                0x0001000000000000, 0x0002000000000000, 0x0004000000000000, 0x0008000000000000,
                0x0010000000000000, 0x0020000000000000, 0x0040000000000000, 0x0080000000000000,
                0x0100000000000000, 0x0200000000000000, 0x0400000000000000, 0x0800000000000000,
                0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x8000000000000000,
            };

            for (int i = 0; i < masked.Length; i++)
            {

                BitManager m = new BitManager();

                m.PutBool(true, i);

                ulong b = m.GetUInt64;

                Assert.AreEqual(masked[i], b);
            }
        }


        [TestMethod]
        public void BitManager_생성시험_02()
        {
            BitManager m = new BitManager();

            Limit_Bool s = new Limit_Bool();
            s.Set(true);

            m.PutBool(false, 0);
            m.PutBool(s.Checked, 1);

            byte b = m.GetUInt8;

            Assert.AreEqual(2, b);
        }

        [TestMethod]
        public void BitManager_생성시험_03()
        {
            BitManager m = new BitManager();

            Limit_Bool s = new Limit_Bool();
            s.Set(true);

            Limit_Byte a2 = new Limit_Byte(4);

            m.PutBool(true, 0);
            m.PutUInt8(a2.Value, 3 ,3);

            byte b = m.GetUInt8;
            short s1 = m.GetInt16;
            int i1 = m.GetInt32;
            long l1 = m.GetInt64;

            int expect = 9;
            Assert.AreEqual(expect, b);
            Assert.AreEqual(expect, s1);
            Assert.AreEqual(expect, i1);
            Assert.AreEqual(expect, l1);

            m.Clear();

            Assert.AreEqual((ulong)0, m.GetUInt64);
            Assert.AreEqual((uint)0, m.GetUInt32);
            Assert.AreEqual((ushort)0, m.GetUInt16);
            Assert.AreEqual((byte)0, m.GetUInt8);
        }

        [TestMethod]
        public void BitManager_생성시험_04()
        {
            BitManager m = new BitManager();

            Limit_Bool s = new Limit_Bool();
            s.Set(true);

            Limit_Byte a2 = new Limit_Byte(4);

            m.PutBool(false, 0);
            m.PutUInt8(a2.Value, 3, 3);

            byte b = m.GetUInt8;
            short s1 = m.GetInt16;
            int i1 = m.GetInt32;
            long l1 = m.GetInt64;

            // int expect = 9;

            byte[] br = m.GetBytes(2);
            byte[] brexpect = new byte[] { 8, 0 };

            Console.WriteLine(ByteConverter.ToHexString(br));

            CollectionAssert.AreEqual(brexpect, br);
        }

        [TestMethod]
        public void BitManager_추출시험_01()
        {
            BitManager m = new BitManager();

            m.PutUInt64(0xFF00110033004400, 63, 64);

            Assert.AreEqual(0xFF, m.ExtractUInt8(63, 8));
            Assert.AreEqual(0x00, m.ExtractUInt8(55, 8));
            Assert.AreEqual(0x11, m.ExtractUInt8(47, 8));
            Assert.AreEqual(0x00, m.ExtractUInt8(39, 8));
            Assert.AreEqual(0x33, m.ExtractUInt8(31, 8));
            Assert.AreEqual(0x00, m.ExtractUInt8(23, 8));
            Assert.AreEqual(0x44, m.ExtractUInt8(15, 8));
            Assert.AreEqual(0x00, m.ExtractUInt8(7, 8));

            Assert.AreEqual(0xFF00, m.ExtractUInt16(63, 16));
            Assert.AreEqual(0x1100, m.ExtractUInt16(47, 16));
            Assert.AreEqual(0x3300, m.ExtractUInt16(31, 16));
            Assert.AreEqual(0x4400, m.ExtractUInt16(15, 16));

            Assert.AreEqual((uint)0xFF001100, m.ExtractUInt32(63, 32));
            Assert.AreEqual((uint)0x33004400, m.ExtractUInt32(31, 32));

            Assert.AreEqual((ulong)0xFF00110033004400, m.ExtractUInt64(63, 64));

            Assert.AreEqual(0x03, m.ExtractUInt8(63, 2));
            Assert.AreEqual(0x03, m.ExtractUInt16(63, 2));
            Assert.AreEqual((uint)0x03, m.ExtractUInt32(63, 2));
            Assert.AreEqual((ulong)0x03, m.ExtractUInt64(63, 2));

            Assert.AreEqual(0x02, m.ExtractUInt8(40, 2));
            Assert.AreEqual(0x02, m.ExtractUInt16(40, 2));
            Assert.AreEqual((uint)0x02, m.ExtractUInt32(40, 2));
            Assert.AreEqual((ulong)0x02, m.ExtractUInt64(40, 2));
        }
    }
}

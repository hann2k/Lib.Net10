using Framework.Common.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Framework.Test.DTO.Limit_Datas
{
    [TestClass]
    public class Limit_XX_UnitTest
    {
        /// <summary>
        /// 이 테스트 안에서만 사용되는 테스트 파라미터 클래스
        /// Expect 와 Origin 은 서로 Scaled 와 normal 사이에서 교차 검증되어야 한다.
        /// originvalue -> expectScale
        /// scalevalue -> expectorigin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class CTestParameter<T>
        {
            public string StrScaledValue;
            public string FullString;
            public string Hex;

            public string String1;
            public string String2;
            public string String3;

            public decimal Min;
            public decimal Max;
            public decimal Scale;

            public T OriginValue;
            public decimal ScaledValue;
        }

        [TestMethod]
        public void Limit_Double_Set_01()
        {
            var d = new Limit_Double();

            d.Set("123.12345,0.0001");

            Assert.AreEqual(123.1234M, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_02()
        {
            var d = new Limit_Double();

            d.Set("1,23.12345", true);

            Assert.AreEqual(123, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_03()
        {
			var d = new Limit_Double();

            d.Set("1,0.1", false);

            // Assert.ThrowsException
            Assert.AreEqual(1, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_04()
        {
			var d = new Limit_Double();

            d.Set("1,0, 10,0.5", false);

            Assert.AreEqual(1, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_05()
        {
			var d = new Limit_Double();

            d.Set("100,0, 200, 0.25", false);

            // Assert.ThrowsException
            // Assert.AreEqual(400, d.Value);
            Assert.AreEqual(100, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_Limit_01()
        {
			var d = new Limit_Double();

            d.Set("100,0, 50, 0.25", false);

            // Assert.ThrowsException
            // Assert.AreEqual(200, d.Value);
            Assert.AreEqual(100, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_Limit_02()
        {
			var d = new Limit_Double();

            d.Set("-100,-50, 50, 0.25", false);

            // Assert.ThrowsException
            // Assert.AreEqual(-200, d.Value);
            Assert.AreEqual(-50, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_Limit_03()
        {
			var d = new Limit_Double();

            d.SetLimit(decimal.MinValue, decimal.MaxValue);
            d.SetScale(0.01M);
            d.SetUV(1000);

            Assert.AreEqual(10, d.ScaledValue());
        }

        [TestMethod]
        public void Limit_Double_Set_Limit_04()
        {
			var d = new Limit_Int32();

            d.SetLimit(-100, 100);
            d.SetScale(0.1M);
            d.SetUV(101);

            Assert.AreEqual(10.1M, d.ScaledValue());
        }

        [TestMethod]
        [DataRow(100, "0.1", "10")]
        [DataRow(100, "0.01", "1")]
        [DataRow(100, "0.001", "0.1")]
        [DataRow(100, "0.0001", "0.01")]
        [DataRow(-100, "0.1", "-10")]
        [DataRow(-100, "0.01", "-1")]
        [DataRow(-100, "0.001", "-0.1")]
        [DataRow(-100, "0.0001", "-0.01")]
        public void Limit_Int32_Set_05(int x, string y, string z)
        {
			var d = new Limit_Int32();

            d.SetLimit(-1000, 1000);
            d.SetScale(System.Convert.ToDecimal(y));
            d.SetUV(x);

            Assert.AreEqual(System.Convert.ToDecimal(z), d.ScaledValue());
        }

        [TestMethod]
        [DataRow(100, "0.1", "10")]
        [DataRow(100, "0.01", "1")]
        [DataRow(100, "0.001", "0.1")]
        [DataRow(100, "0.0001", "0.01")]
        [DataRow(-100, "0.1", "-10")]
        [DataRow(-100, "0.01", "-1")]
        [DataRow(-100, "0.001", "-0.1")]
        [DataRow(-100, "0.0001", "-0.01")]
        public void Limit_Int16_Set_01(int x, string y, string z)
        {
			var d = new Limit_Int16();

            d.SetLimit(-1000, 1000);
            d.SetScale(System.Convert.ToDecimal(y));
            d.SetUV((short)x);

            Console.WriteLine(d.FullString);

            Assert.AreEqual(System.Convert.ToDecimal(z), d.ScaledValue());
        }

        [TestMethod]
        [DataRow(100, "0.1", "10")]
        [DataRow(100, "0.01", "1")]
        [DataRow(100, "0.001", "0.1")]
        [DataRow(100, "0.0001", "0.01")]
        [DataRow(-100, "0.1", "-10")]
        [DataRow(-100, "0.01", "-1")]
        [DataRow(-100, "0.001", "-0.1")]
        [DataRow(-100, "0.0001", "-0.01")]
        public void Limit_Int8_Set_01(double x, string y, string z)
        {
			var d = new Limit_SByte();

            var scale = System.Convert.ToDecimal(y);

            d.SetLimit(sbyte.MinValue, sbyte.MaxValue);
            d.SetScale(scale);
            d.SetUV((sbyte)x);

			// "0.0100,-128,127,0.0001";

			var s = d.ToScaleString(4);
            Console.WriteLine($"{s}     {scale}");

            Assert.AreEqual(System.Convert.ToDecimal(z), d.ScaledValue());
            Assert.AreEqual(scale, d.Scale);
        }




        private List<CTestParameter<bool>> TestParameterBool()
        {
			var param = new List<CTestParameter<bool>> {
                new CTestParameter<bool> {
                    FullString = "True",
                    OriginValue = true,
                    ScaledValue = 1,
                    Min = 0,
                    Max = 1,
                    Scale = 1
                },
                new CTestParameter<bool> {
                    FullString = "False",
                    OriginValue = false,
                    ScaledValue = 0,
                    Min = 0,
                    Max = 1,
                    Scale = 1
                },
            };

            return param;
        }

        [TestMethod]
        public void Limit_Bool_Test_01()
        {
            List<CTestParameter<bool>> param = this.TestParameterBool();

			var v = new Limit_Bool();

            // 기본값 검사
            Assert.AreEqual(false, v.Value);

            foreach (CTestParameter<bool> item in param)
            {
                v.Set(item.OriginValue);

                Assert.AreEqual(item.OriginValue, v.Value);
                Assert.AreEqual(item.FullString, v.FullString);
            }

            foreach (CTestParameter<bool> item in param)
            {
                v.Checked = item.OriginValue;

                Assert.AreEqual(item.OriginValue, v.Value);
                Assert.AreEqual(item.OriginValue, v.Checked);
                Assert.AreEqual(item.FullString, v.FullString);
            }
        }

        private List<CTestParameter<byte>> TestParameterByte()
        {
			var param = new List<CTestParameter<byte>> {
                // 정수
                new CTestParameter<byte> {
                    FullString = "10.0,0,20,0.1",
                    String1="10.0",
                    String2="10.0,0.1",
                    String3="10.0,0,20",
                    StrScaledValue="10",
                    Hex="64",

                    OriginValue = 100,
                    ScaledValue = 10,
                    Min = 0,
                    Max = 20,
                    Scale = 0.1m
                },
                // 실수
                new CTestParameter<byte> {
                    FullString = "10.1,0,20,0.1",
                    String1="10.1",
                    String2="10.1,0.1",
                    String3="10.1,0,20",
                    StrScaledValue="10.1",
                    Hex="65",

                    OriginValue = 101,
                    ScaledValue = 10.1m,
                    Min = 0,
                    Max = 20,
                    Scale = 0.1m
                },
            };

            return param;
        }

        [TestMethod]
        public void Limit_Byte_Test_01()
        {
            List<CTestParameter<byte>> param = this.TestParameterByte();

			var v = new Limit_Byte();

            // 기본형 검사
            Assert.AreEqual(1, v.Scale);
            Assert.AreEqual(byte.MaxValue, v.Max);
            Assert.AreEqual(byte.MinValue, v.Min);
            Assert.AreEqual(0, v.ScaledValue());
            Assert.AreEqual(0, v.Value);

            // 입력제한 검사
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => v.SetScale(-100));
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => v.SetScale(-1));
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => v.SetScale(-0.1m));
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => v.SetScale(0));
            // Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => v.SetLimit(byte.MinValue - 1, byte.MaxValue + 1));

            foreach (CTestParameter<byte> item in param)
            {
                v.SetLimit(item.Min, item.Max);
                v.SetScale(item.Scale);
                v.SetUV(item.OriginValue);

                Assert.AreEqual(item.Scale, v.Scale);
                Assert.AreEqual(item.Max, v.Max);
                Assert.AreEqual(item.Min, v.Min);

                Assert.AreEqual(item.OriginValue, v.Value);
                Assert.AreEqual(item.ScaledValue, v.ScaledValue());
                Assert.AreEqual(item.StrScaledValue, v.StrScaledValue);
                Assert.AreEqual(item.FullString, v.FullString);
                Assert.AreEqual(item.Hex, v.Hex);

                Assert.AreEqual(item.String1, v.ToScaleString(1));
                Assert.AreEqual(item.String2, v.ToScaleString(2));
                Assert.AreEqual(item.String3, v.ToScaleString(3));
                Assert.AreEqual(item.FullString, v.ToScaleString(4));

                // Assert.AreEqual(item.StrScaledValue, v.To1000CommaString());
            }

            foreach (CTestParameter<byte> item in param)
            {
                v.SetLimit(item.Min, item.Max);
                v.SetScale(item.Scale);
                v.Set(item.ScaledValue);

                Assert.AreEqual(item.Scale, v.Scale);
                Assert.AreEqual(item.Max, v.Max);
                Assert.AreEqual(item.Min, v.Min);

                Assert.AreEqual(item.OriginValue, v.Value);
                Assert.AreEqual(item.ScaledValue, v.ScaledValue());
                Assert.AreEqual(item.StrScaledValue, v.StrScaledValue);
                Assert.AreEqual(item.FullString, v.FullString);
                Assert.AreEqual(item.Hex, v.Hex);

                Assert.AreEqual(item.String1, v.ToScaleString(1));
                Assert.AreEqual(item.String2, v.ToScaleString(2));
                Assert.AreEqual(item.String3, v.ToScaleString(3));
                Assert.AreEqual(item.FullString, v.ToScaleString(4));

                // Assert.AreEqual(item.StrScaledValue, v.To1000CommaString());
            }

            foreach (CTestParameter<byte> item in param)
            {
                v.Clear();
                v.Set(item.FullString);

                Assert.AreEqual(item.Scale, v.Scale);
                Assert.AreEqual(item.Max, v.Max);
                Assert.AreEqual(item.Min, v.Min);

                Assert.AreEqual(item.OriginValue, v.Value);
                Assert.AreEqual(item.ScaledValue, v.ScaledValue());
                Assert.AreEqual(item.StrScaledValue, v.StrScaledValue);
                Assert.AreEqual(item.FullString, v.FullString);
                Assert.AreEqual(item.Hex, v.Hex);

                Assert.AreEqual(item.String1, v.ToScaleString(1));
                Assert.AreEqual(item.String2, v.ToScaleString(2));
                Assert.AreEqual(item.String3, v.ToScaleString(3));
                Assert.AreEqual(item.FullString, v.ToScaleString(4));

                // Assert.AreEqual(item.StrScaledValue, v.To1000CommaString());
            }

            foreach (CTestParameter<byte> item in param)
            {
                v.Clear();
                v.Set(item.String2);

                Assert.AreEqual(item.Scale, v.Scale);
                Assert.AreEqual(byte.MaxValue, v.Max);
                Assert.AreEqual(byte.MinValue, v.Min);

                Assert.AreEqual(item.OriginValue, v.Value);
                Assert.AreEqual(item.ScaledValue, v.ScaledValue());
                Assert.AreEqual(item.StrScaledValue, v.StrScaledValue);
                // Assert.AreEqual(item.FullString, v.FullString);
                Assert.AreEqual(item.Hex, v.Hex);

                Assert.AreEqual(item.String1, v.ToScaleString(1));
                Assert.AreEqual(item.String2, v.ToScaleString(2));
                // Assert.AreEqual(item.String3, v.ToScaleString(3));
                // Assert.AreEqual(item.FullString, v.ToScaleString(4));

                // Assert.AreEqual(item.StrScaledValue, v.To1000CommaString());
            }
        }


    }
}

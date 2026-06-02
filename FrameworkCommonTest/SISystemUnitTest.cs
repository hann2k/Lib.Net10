using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Framework.Common.Unit.SI;

namespace Framework.Test.Unit.Si
{
    [TestClass]
    public class Framework_Unit_SIUnit_Test
    {
        [TestMethod]
        public void SiUnit_생성시험_01()
        {
            SIUnit unit = new SIUnit(SIUnitBase.Meter);

            string 기준 = SIUnitBase.Meter.Symbol;
            string 실제 = unit.Symbol;

            Assert.AreEqual(기준, 실제);
        }

        [TestMethod]
        public void SiUnit_생성시험_02()
        {
            SIUnit unit = new SIUnit(SIUnitBase.Ampere);

            string 기준 = SIUnitBase.Ampere.Symbol;
            string 실제 = unit.Symbol;

            Assert.AreEqual(기준, 실제);
        }

        [TestMethod]
        public void SiUnit_비교시험_같음1()
        {
            SIUnit unit1 = new SIUnit(SIUnitBase.Ampere);

            SIUnit unit2 = new SIUnit(SIUnitBase.Ampere);

            // bool result = ;

            Assert.AreEqual(true, unit1 == unit2);
        }

        [TestMethod]
        public void SiUnit_비교시험_같음2()
        {
            SIUnit unit1 = new SIUnit(SIUnitBase.Ampere);

            SIUnit unit2 = new SIUnit(SIUnitBase.Acceleration);

            bool result = unit1 == unit2;

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void SiUnit_비교시험_다름1()
        {
            SIUnit unit1 = new SIUnit(SIUnitBase.Ampere);

            SIUnit unit2 = new SIUnit(SIUnitBase.Ampere);

            bool result = unit1 != unit2;

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void SiUnit_비교시험_다름2()
        {
            SIUnit unit1 = new SIUnit(SIUnitBase.Ampere);

            SIUnit unit2 = new SIUnit(SIUnitBase.Acceleration);

            bool result = unit1 != unit2;

            Assert.AreEqual(true, result);
        }


    }

    [TestClass]
    public class Framework_Unit_SIPrefix_Test
    {
        [TestMethod]
        public void SiPrefix_생성시험_01()
        {
			var p = new MetricPrefix(MPS.Q);

            // SI 접두어 초기값 검사
            Assert.AreEqual(MPS.Q, p.Symbol);
            Assert.AreEqual(30, p.Base10);
        }

        [TestMethod]
        public void SiPrefix_생성시험_02()
        {
			var p = new MetricPrefix(30);

            // SI 접두어 초기값 검사
            Assert.AreEqual(MPS.Q, p.Symbol);
            Assert.AreEqual((int)MPS.Q, p.Base10);
        }

        [TestMethod]
        public void SiPrefix_생성시험_03()
        {
			var p = new MetricPrefix();

            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => p.SetBase10(31));
        }

        [TestMethod]
        public void SiPrefix_생성시험_04()
        {
            var p = new MetricPrefix();

            p.SetBase10(29);

            Assert.AreEqual(MPS.R, p.Symbol);
            Assert.AreEqual(2, p.OverFlow);

            Assert.AreEqual(0, p.OverFlow);

            // Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => p.SetBase10(29));
        }

        [TestMethod]
        public void SiPrefix_변경시험_01()
        {
			var p = new MetricPrefix(29);

            Assert.AreEqual(MPS.R, p.Symbol);
            Assert.AreEqual(2, p.OverFlow);

            p.SetSymbol(MPS.Z);

            Assert.AreEqual(MPS.Z, p.Symbol);
            Assert.AreEqual(0, p.OverFlow);

            // Assert.AreEqual(0, p.OverFlow);

            // Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => p.SetBase10(29));
        }

        [TestMethod]
        public void SiPrefix_변경시험_02()
        {
			var p = new MetricPrefix(29);

            Assert.AreEqual(MPS.R, p.Symbol);
            Assert.AreEqual(2, p.OverFlow);

            p.SetSymbol("Z");

            Assert.AreEqual(MPS.Z, p.Symbol);
            Assert.AreEqual(0, p.OverFlow);
        }

        [TestMethod]
        public void SiPrefix_변경시험_03()
        {
			var p = new MetricPrefix(29);

            Assert.AreEqual(MPS.R, p.Symbol);
            Assert.AreEqual(2, p.OverFlow);

            p.SetSymbol("X");

            Assert.AreEqual(MPS.Zero, p.Symbol);
            Assert.AreEqual(0, p.OverFlow);
        }

        // 접두어 음양 양음 변경
        [TestMethod]
        public void SiPrefix_변경시험_04()
        {
			var p = new MetricPrefix(MPS.R);

            Assert.AreEqual(MPS.R, p.Symbol);
            // Assert.AreEqual(2, p.OverFlow);

            p = -p;

            Assert.AreEqual(MPS.r, p.Symbol);

            p = -p;

            Assert.AreEqual(MPS.R, p.Symbol);

        }

        // 접두어 음양 양음 변경
        [TestMethod]
        public void SiPrefix_변경시험_05()
        {
			var p = new MetricPrefix(27);

            Assert.AreEqual(MPS.R, p.Symbol);
            // Assert.AreEqual(2, p.OverFlow);

            p = -p;

            Assert.AreEqual(MPS.r, p.Symbol);

            p = -p;

            Assert.AreEqual(MPS.R, p.Symbol);

        }

        // 접두어 음양 양음 변경
        [TestMethod]
        public void SiPrefix_변경시험_06()
        {
			var p = new MetricPrefix(29);

            Assert.AreEqual(MPS.R, p.Symbol);
            // Assert.AreEqual(2, p.OverFlow);

            p = -p;

            Assert.AreEqual(MPS.r, p.Symbol);

            p = -p;

            Assert.AreEqual(MPS.R, p.Symbol);

        }

        [TestMethod]
        public void SiPrefix_연산시험_01()
        {
			var p1 = new MetricPrefix(MPS.k);
			var p2 = new MetricPrefix(MPS.k);
			var p3 = new MetricPrefix(MPS.k);

			MetricPrefix p4 = p1 + p2 + p3;

            Assert.AreEqual(MPS.G, p4.Symbol);
        }

        [TestMethod]
        public void SiPrefix_연산시험_02()
        {
			var p1 = new MetricPrefix(MPS.k);
			var p2 = new MetricPrefix(MPS.k);
			var p3 = new MetricPrefix(MPS.k);

			MetricPrefix p4 = p1 - p2 - p3;

            Assert.AreEqual(MPS.m, p4.Symbol);
        }
    }

//    [TestClass]
//    public class Framework_Unit_SIValue_Test
//    {

//        [TestMethod]
//        public void SiValue_생성시험_01()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter);

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 초기값 설정 검사
//            Assert.AreEqual(0, m.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.Zero, m.Prefix.Symbol);
//            Assert.AreEqual(0, m.Prefix.Base10);
//        }

//        [TestMethod]
//        public void SiValue_생성시험_02()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 0.1m);

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 초기값 설정 검사
//            Assert.AreEqual(0.1m, m.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.Zero, m.Prefix.Symbol);
//            Assert.AreEqual(0, m.Prefix.Base10);
//        }

//        [TestMethod]
//        [DataRow(3, -10)]
//        [DataRow(3, 1.1)]
//        [DataRow(3, 10)]
//        public void SiValue_생성시험_03(int base10, double baseValue)
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, (decimal)baseValue, new MetricPrefix(base10));

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 초기값 설정 검사
//            Assert.AreEqual((decimal)baseValue, m.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual(base10, m.Prefix.Base10);
//        }

//        [TestMethod]
//        public void SiValue_생성시험_04()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 10, new MetricPrefix(MPS.k));

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 초기값 설정 검사
//            Assert.AreEqual(10, m.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"10 km", m.ToString());
//        }

//        [TestMethod]
//        public void SiValue_생성시험_05()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 10, MPS.k);

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 초기값 설정 검사
//            Assert.AreEqual(10, m.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"10 km", m.ToString());
//        }

//        [TestMethod]
//        public void SiValue_생성시험_06()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 10, 5);

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 초기값 설정 검사
//            Assert.AreEqual(1000, m.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"1000 km", m.ToString());
//        }

//        [TestMethod]
//        public void SiValue_생성시험_07()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 10, 10);

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 초기값 설정 검사
//            Assert.AreEqual(100, m.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.G, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.G, m.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"100 Gm", m.ToString());
//        }

//        [TestMethod]
//        public void SiValue_Prefix변경시험_01()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m = new SIValue(SIUnitBase.Meter, 10, p);

//            // SI 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 기본값 설정 검사
//            Assert.AreEqual(10m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);

//            m.SetPrefix(MPS.M);

//            // 기본값 변경 검사
//            Assert.AreEqual(10m / 1000m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.M, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.M, m.Prefix.Base10);
//        }

//        [TestMethod]
//        public void SiValue_Prefix변경시험_02()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.M);

//            SIValue m = new SIValue(SIUnitBase.Meter, 10, p);

//            // SI 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 기본값 설정 검사
//            Assert.AreEqual(10m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.M, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.M, m.Prefix.Base10);

//            m.SetPrefix(MPS.k);

//            // 기본값 변경 검사
//            Assert.AreEqual(10m * 1000m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);
//        }

//        /// <summary>
//        /// 같은 prefix로 변경할 때
//        /// </summary>
//        [TestMethod]
//        public void SiValue_Prefix변경시험_03()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 10, MPS.k);

//            // SI 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 기본값 설정 검사
//            Assert.AreEqual(10m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);

//            m.SetPrefix(MPS.k);

//            // 기본값 변경 검사
//            Assert.AreEqual(10m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);
//        }

//        /// <summary>
//        /// 같은 prefix로 변경할 때
//        /// </summary>
//        [TestMethod]
//        public void SiValue_Prefix변경시험_04()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 10, MPS.k);

//            // SI 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 기본값 설정 검사
//            Assert.AreEqual(10m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);

//            m.SetPrefix(MPS.G);

//            // 기본값 변경 검사
//            Assert.AreEqual(10m / (1000m * 1000m), m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.G, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.G, m.Prefix.Base10);
//        }

//        /// <summary>
//        /// 같은 prefix로 변경할 때
//        /// </summary>
//        [TestMethod]
//        public void SiValue_Prefix변경시험_05()
//        {
//            SIValue m = new SIValue(SIUnitBase.Meter, 10, MPS.k);

//            // SI 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m.Unit.Symbol);

//            // 기본값 설정 검사
//            Assert.AreEqual(10m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.k, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m.Prefix.Base10);

//            m.SetPrefix(30);

//            // 기본값 변경 검사
//            Assert.AreEqual(0.00000000000000000000000001m, m.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.Q, m.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.Q, m.Prefix.Base10);
//        }

        

        

        

        
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_더하기_Test
//    {
//        [TestMethod]
//        public void SiValue_연산시험_더하기_01()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, p);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, p);

//            SIValue m3 = m1 + m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m3.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(30, m3.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m3.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m3.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"30 km", m3.ToString());
//        }

//        [TestMethod]
//        public void SiValue_연산시험_더하기_02()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 0.01m, MPS.M);

//            SIValue m3 = m1 + m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m3.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(20m, m3.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m3.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m3.Prefix.Base10);

//            // 출력 검사
//            // Assert.AreEqual($"20 km", m3.ToString());
//        }

//        [TestMethod]
//        public void SiValue_연산시험_더하기_03()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 100, MPS.k);

//            SIValue m3 = m1 + m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m3.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(10.1m, m3.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.M, m3.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.M, m3.Prefix.Base10);

//            // 출력 검사
//            // Assert.AreEqual($"20 km", m3.ToString());
//        }

//        [TestMethod]
//        public void SiValue_연산시험_더하기3항_01()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, p);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, p);
//            SIValue m3 = new SIValue(SIUnitBase.Meter, 31.15m, p);

//            SIValue m4 = m1 + m2 + m3;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m4.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(61.15m, m4.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m4.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m4.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"61.15 km", m4.ToString());
//        }
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_빼기_Test
//    {
//        [TestMethod]
//        public void SiValue_연산시험_빼기_01()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, p);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, p);

//            SIValue m3 = m1 - m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m3.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(-10, m3.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m3.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m3.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"-10 km", m3.ToString());
//        }

//        [TestMethod]
//        public void SiValue_연산시험_빼기_02()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, 9);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, 10);

//            SIValue m3 = m1 - m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m3.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(-190, m3.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.G, m3.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.G, m3.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"-190 Gm", m3.ToString());
//        }

//        [TestMethod]
//        public void SiValue_연산시험_빼기_03()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 0.1m, 12);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, 9);

//            SIValue m3 = m1 - m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m3.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(0.08m, m3.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.T, m3.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.T, m3.Prefix.Base10);

//            m3.SetPrefix(MPS.k);

//            // 출력 검사
//            Assert.AreEqual($"80000000.000 km", m3.ToString());
//        }

//        [TestMethod]
//        public void SiValue_연산시험_빼기3항_01()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, p);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, p);
//            SIValue m3 = new SIValue(SIUnitBase.Meter, 31.15m, p);

//            SIValue m4 = m1 - m2 - m3;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m4.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(-41.15m, m4.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m4.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.k, m4.Prefix.Base10);

//            // 출력 검사
//            Assert.AreEqual($"-41.15 km", m4.ToString());
//        }
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_곱하기_Test
//    {
//        [TestMethod]
//        public void SiValue_연산시험_곱하기_01()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);
//            // SIValue m3 = new SIValue(SIUnitBase.Meter, 31.15m, p);

//            SIValue m4 = m1 * m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Area, m4.Unit.BaseUnit);

//            // 연산값 설정 검사
//            Assert.AreEqual(200m, m4.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.M, m4.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.M, m4.Prefix.Base10);
//        }

//        [TestMethod]
//        public void SiValue_연산시험_곱하기_02()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.M);
//            // SIValue m3 = new SIValue(SIUnitBase.Meter, 31.15m, p);

//            SIValue m4 = m1 * m2;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Area, m4.Unit.BaseUnit);

//            // 연산값 설정 검사
//            Assert.AreEqual(200m, m4.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.G, m4.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.G, m4.Prefix.Base10);

//        }

//        [TestMethod]
//        public void SiValue_연산시험_곱하기_03()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, -10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.M);
//            SIValue m3 = new SIValue(SIUnitBase.Meter, -30, MPS.n);
//            // SIValue m3 = new SIValue(SIUnitBase.Meter, 31.15m, p);

//            SIValue m4 = m1 * m2 * m3;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Volume, m4.Unit.BaseUnit);

//            // 연산값 설정 검사
//            Assert.AreEqual(6000m, m4.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.Zero, m4.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.Zero, m4.Prefix.Base10);
//        }
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_나누기_Test
//    {
//        [TestMethod]
//        public void SiValue_연산시험_나누기_01()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            SIValue m4 = m1 / 20;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m4.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(0.5m, m4.BaseValue);

//            // SI 접두어 검사
//            Assert.AreEqual(MPS.k, m4.Prefix.Symbol);

//            // 출력 검사
//            Assert.AreEqual($"0.5 km", m4.ToString());
//        }

//        [TestMethod]
////        [ExpectedException(typeof(SIUnitCannotConvertException))]
//        public void SiValue_연산시험_나누기_02()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            Assert.ThrowsException<SIUnitCannotConvertException>(()=> {
//                SIValue m4 = m1 / m2;

//                // 단위 일치 검사
//                Assert.AreEqual(SIUnitBase.Meter.Symbol, m4.Unit.Symbol);

//                // 연산값 설정 검사
//                Assert.AreEqual(0.5m, m4.BaseValue);

//                // SI 접두어 초기값 검사
//                Assert.AreEqual(MPS.k, m4.Prefix.Symbol);

//                // 출력 검사
//                Assert.AreEqual($"0.5 km", m4.ToString());
//            });
            
//        }

//        [TestMethod]
//        public void SiValue_연산시험_나누기_03()
//        {
//            MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.M);

//            SIValue m4 = 20 / m1;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Meter.Symbol, m4.Unit.Symbol);

//            // 연산값 설정 검사
//            Assert.AreEqual(2m, m4.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.k, m4.Prefix.Symbol);
//            // Assert.AreEqual((int)MPS.m, m4.Prefix.Base10);

//        }
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_복합연산_Test
//    {
//        [TestMethod]
//        public void SiValue_복합연산_곱하기_더하기_01()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            SIValue m3 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            SIValue m4 = m1 * (m2 + m3);

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Area, m4.Unit.BaseUnit);

//            // 연산값 설정 검사
//            Assert.AreEqual(400m, m4.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.M, m4.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.M, m4.Prefix.Base10);
//        }

//        [TestMethod]
//        public void SiValue_복합연산_곱하기_더하기_02()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            SIValue m3 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            // SIValue m4 = m1 * m2 + m3;

//            Assert.ThrowsException<SIUnitMismatchException>(() => m1 * m2 + m3);
//        }

//        [TestMethod]
//        public void SiValue_복합연산_곱하기_더하기_빼기01()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            SIValue m3 = new SIValue(SIUnitBase.Meter, 20, MPS.k);
//            SIValue m4 = new SIValue(SIUnitBase.Meter, 20, MPS.M);

//            // SIValue mr = m4 - m1 * m2 + m3;

//            Assert.ThrowsException<SIUnitMismatchException>(() => m4 - m1 * m2 + m3);

//            //// 단위 일치 검사
//            //Assert.AreEqual(SIUnitBase.Meter.Symbol, mr.Unit.Symbol);

//            //// 연산값 설정 검사
//            //Assert.AreEqual(-179.98m, mr.BaseValue);

//            //// SI 접두어 초기값 검사
//            //Assert.AreEqual(MPS.M, mr.Prefix.Symbol);
//            //Assert.AreEqual((int)MPS.M, mr.Prefix.Base10);
//        }

//        [TestMethod]
//        public void SiValue_복합연산_곱하기_더하기_빼기02()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            SIValue m3 = new SIValue(SIUnitBase.Meter, 20, MPS.k);
//            SIValue m4 = new SIValue(SIUnitBase.Meter, 20, MPS.M);

//            SIValue mr = (m4 - m1) * (m2 + m3);

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Area, mr.Unit.BaseUnit);

//            // 연산값 설정 검사
//            Assert.AreEqual(799.6m, mr.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.G, mr.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.G, mr.Prefix.Base10);
//        }

//        [TestMethod]
//        public void SiValue_복합연산_곱하기_더하기_빼기_나누기_01()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Meter, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 20, MPS.k);

//            SIValue m3 = new SIValue(SIUnitBase.Meter, 20, MPS.k);
//            SIValue m4 = new SIValue(SIUnitBase.Meter, 20, MPS.M);

//            // SIValue m5 = new SIValue(SIUnitBase.Meter, 20, MPS.M);

//            SIValue mr = (m4 - m1) * (m2 + m3) / 20;

//            // 단위 일치 검사
//            Assert.AreEqual(SIUnitBase.Area, mr.Unit.BaseUnit);

//            // 연산값 설정 검사
//            Assert.AreEqual(39.98m, mr.BaseValue);

//            // SI 접두어 초기값 검사
//            Assert.AreEqual(MPS.G, mr.Prefix.Symbol);
//            Assert.AreEqual((int)MPS.G, mr.Prefix.Base10);
//        }
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_Exception_Test
//    {
//        [TestMethod]
//        public void SiValue_예외시험_단위다른것끼리_더하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 0.01m, MPS.M);

//            Assert.ThrowsException<SIUnitMismatchException>(() => m1 + m2);
//        }

//        [TestMethod]
//        public void SiValue_예외시험_단위다른것끼리_빼기_01()
//        {
//            // MetricPrefix p = new MetricPrefix(MPS.k);

//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 0.01m, MPS.M);

//            Assert.ThrowsException<SIUnitMismatchException>(() => m1 - m2);
//        }

//        [TestMethod]
//        public void SiValue_예외시험_같음_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 0.01m, MPS.M);

//            Assert.ThrowsException<SIUnitMismatchException>(() => m1 == m2);
//        }

//        [TestMethod]
//        public void SiValue_예외시험_다름_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.k);
//            SIValue m2 = new SIValue(SIUnitBase.Meter, 0.01m, MPS.M);

//            Assert.ThrowsException<SIUnitMismatchException>(() => m1 != m2);
//        }
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_같음다름_비교하기
//    {
//        [TestMethod]
//        public void SiValue_같음_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(true, m1 == m2);
//        }

//        [TestMethod]
//        public void SiValue_같음_비교하기_02()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);

//            Assert.AreEqual(false, m1 == m2);
//        }

//        [TestMethod]
//        public void SiValue_다름_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(false, m1 != m2);
//        }

//        [TestMethod]
//        public void SiValue_다름_비교하기_02()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);

//            Assert.AreEqual(true, m1 != m2);
//        }
//    }

//    [TestClass]
//    public class Framework_Unit_SIValue_크다작다_비교하기
//    {
//        [TestMethod]
//        public void SiValue_크다_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(true, m1 > m2);
//        }

//        [TestMethod]
//        public void SiValue_크다_비교하기_02()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);

//            Assert.AreEqual(false, m1 > m2);
//        }

//        [TestMethod]
//        public void SiValue_크거나같다_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(true, m1 >= m2);
//        }

//        [TestMethod]
//        public void SiValue_크거나같다_비교하기_02()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(true, m1 >= m2);
//        }

//        [TestMethod]
//        public void SiValue_크거나같다_비교하기_03()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);

//            Assert.AreEqual(false, m1 >= m2);
//        }

//        [TestMethod]
//        public void SiValue_작다_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);

//            Assert.AreEqual(true, m1 < m2);
//        }

//        [TestMethod]
//        public void SiValue_작다_비교하기_02()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(false, m1 < m2);
//        }

//        [TestMethod]
//        public void SiValue_작거나같다_비교하기_01()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);

//            Assert.AreEqual(true, m1 <= m2);
//        }

//        [TestMethod]
//        public void SiValue_작거나같다_비교하기_02()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(true, m1 <= m2);
//        }

//        [TestMethod]
//        public void SiValue_작거나같다_비교하기_03()
//        {
//            SIValue m1 = new SIValue(SIUnitBase.Ampere, 11, MPS.M);
//            SIValue m2 = new SIValue(SIUnitBase.Ampere, 10, MPS.M);

//            Assert.AreEqual(false, m1 <= m2);
//        }
//    }
}

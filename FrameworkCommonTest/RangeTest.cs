using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Framework.Common.Unit;
using Framework.Common.Unit.SI;
using Range = Framework.Common.Unit.Range;

namespace Framework.Test.Unit
{
    [TestClass]
    public class Framework_Unit_Range_Test
    {
        [TestMethod]
        public void Range_생성시험_01()
        {
            Range a = new Range(-100, 100);

            Assert.AreEqual(-100, a.Min);
            Assert.AreEqual(100, a.Max);
            Assert.AreEqual(0, a.Offset_1);
            Assert.AreEqual(0, a.Offset_2);
        }

        [TestMethod]
        public void Range_생성시험_02()
        {
            Range a = new Range(100, -100);

            Assert.AreEqual(-100, a.Min);
            Assert.AreEqual(100, a.Max);
            Assert.AreEqual(0, a.Offset_1);
            Assert.AreEqual(0, a.Offset_2);
        }

        [TestMethod]
        public void Range_생성시험_03()
        {
            Range a = new Range("시험", "-100, 100");
            Assert.AreEqual(-100, a.Min);
            Assert.AreEqual(100, a.Max);
            Assert.AreEqual(0, a.Offset_1);
            Assert.AreEqual(0, a.Offset_2);
        }

        [TestMethod]
        public void Range_생성시험_04()
        {
            Range a = new Range("시험", "100, -100");
            Assert.AreEqual(-100, a.Min);
            Assert.AreEqual(100, a.Max);
            Assert.AreEqual(0, a.Offset_1);
            Assert.AreEqual(0, a.Offset_2);
        }

        [TestMethod]
        public void Range_생성시험_05()
        {
            Range a = new Range("시험", "100, -100, 1, 2");
            Assert.AreEqual(-100, a.Min);
            Assert.AreEqual(100, a.Max);
            Assert.AreEqual(1, a.Offset_1);
            Assert.AreEqual(2, a.Offset_2);
        }

        [TestMethod]
        public void Range_생성시험_06()
        {
            Range a = new Range("시험", "100, -100, -1, -2");
            Assert.AreEqual(-100, a.Min);
            Assert.AreEqual(100, a.Max);
            Assert.AreEqual(-1, a.Offset_1);
            Assert.AreEqual(-2, a.Offset_2);
        }

        [TestMethod]
        public void Range_이상이하경계시험_01()
        {
            Range a = new Range(-100, 100);

            Assert.IsTrue(a.In(100, true));
            Assert.IsTrue(a.In(-100, true));
        }

        [TestMethod]
        public void Range_이상이하경계시험_02()
        {
            Range a = new Range(-100, 100);

            Assert.IsFalse(a.In(100.000001, true));
            Assert.IsFalse(a.In(-100.0000001, true));
        }

        [TestMethod]
        public void Range_초과미만경계시험_01()
        {
            Range a = new Range(-100, 100);

            Assert.IsTrue(a.In(99.9999999, false));
            Assert.IsTrue(a.In(-99.9999999, false));
        }

        [TestMethod]
        public void Range_초과미만경계시험_02()
        {
            Range a = new Range(-100, 100);

            Assert.IsFalse(a.In(100, false));
            Assert.IsFalse(a.In(-100, false));
        }

        [TestMethod]
        public void Range_경계바깥시험_01()
        {
            Range a = new Range(-100, 100);

            Assert.IsFalse(a.In(100.1, true));
            Assert.IsFalse(a.In(-100.1, true));
        }

        [TestMethod]
        public void Range_경계바깥시험_02()
        {
            Range a = new Range(-100, 100);

            Assert.IsFalse(a.In(100.1, false));
            Assert.IsFalse(a.In(-100.1, false));
        }
    }
}

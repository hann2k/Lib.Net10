using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Common.Converter;
using Framework.Common.DTO;
using Framework.Common.Unit;
using Framework.Common.Unit.SI;

namespace Framework.Test.Unit
{
    [TestClass]
    public class System_Math_Round_Test
    {

        /// <summary>
        /// 마지막 자리에서 반올림
        /// </summary>
        [TestMethod]
        [DataRow(0.5, 0, 1)]
        [DataRow(0.55, 1, 0.6)]
        [DataRow(0.555, 2, 0.56)]
        [DataRow(0.5555, 3, 0.556)]
        [DataRow(0.55555, 4, 0.5556)]
        [DataRow(0.555555, 5, 0.55556)]
        public void MathRound(double v, int digit, double r)
        {
            // Assert.AreEqual(r, Math.Round(v, digit));
            Assert.AreEqual(r, Math.Round(v, digit, MidpointRounding.AwayFromZero));
        }
    }
}

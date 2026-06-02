using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Unit
{
    public class SIKilogram : SIValue
    {
        public SIKilogram()
        {
            base.Unit = new SIUnit("g", "gram", "mass", "M", new[] { "m"}, "The kilogram is defined by setting the Planck constant h exactly to 6.62607015×10^−34 J⋅s (J = kg⋅m^2⋅s^−2), given the definitions of the metre and the second.");
        }

        public override int CompareTo(object obj)
        {
            SIKilogram c = (SIKilogram)obj;

            return Decimal.Compare(base.Value, c.Value);
        }
    }
}

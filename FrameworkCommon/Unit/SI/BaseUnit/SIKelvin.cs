using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Unit
{
    public class SIKelvin : SIValue
    {
        public SIKelvin()
        {
            base.Unit = new SIUnit("K", "kelvin", "themodynamic temperature", "Θ", new[] { "T" }, "The kelvin is defined by setting the fixed numerical value of the Boltzmann constant k to 1.380649×10^−23 J⋅K^−1, (J = kg⋅m^2⋅s^−2), given the definition of the kilogram, the metre, and the second.");
        }

        public override int CompareTo(object obj)
        {
            SIKelvin c = (SIKelvin)obj;

            return Decimal.Compare(base.Value, c.Value);
        }
    }
}

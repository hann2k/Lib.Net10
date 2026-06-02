using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Unit
{
    public class SIMole : SIValue
    {
        public SIMole()
        {
            base.Unit = new SIUnit("mol", "mole", "amount of substance", "N", new[] {"n"}, "The amount of substance of exactly 6.02214076×10^23 elementary entities. This number is the fixed numerical value of the Avogadro constant, N_A, when expressed in the unit mol^−1.");
        }

        public override int CompareTo(object obj)
        {
            SIMole c = (SIMole)obj;

            return Decimal.Compare(base.Value, c.Value);
        }
    }
}

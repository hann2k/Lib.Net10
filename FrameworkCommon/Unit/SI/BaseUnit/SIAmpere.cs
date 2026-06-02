using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Unit
{
    public class SIAmpere : SIValue
    {
        public SIAmpere()
        {
            base.Unit = new SIUnit("A", "ampere", "electric current","I", new[] {"I","i"}, "The flow of exactly ( 1 / (1.602176634×10^−19) ) times the elementary charge e per second. Equalling approximately 6.2415090744×10^18 elementary charges per second.");
        }

        public override int CompareTo(object obj)
        {
            SIAmpere c = (SIAmpere)obj;

            return Decimal.Compare(base.Value, c.Value);
        }
    }
}

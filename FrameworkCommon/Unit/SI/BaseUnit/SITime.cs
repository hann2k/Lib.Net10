using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Unit
{
    public class SISecond : SIValue
    {
        public SISecond()
        {
            base.Unit = new SIUnit("s", "second", "time", "T", new[] {"t"}, "The duration of 9,192,631,770 periods of the radiation corresponding to the transition between the two hyperfine levels of the ground state of the caesium-133 atom.");
        }

        public override int CompareTo(object obj)
        {
            SISecond c = (SISecond)obj;

            return Decimal.Compare(base.Value, c.Value);
        }
    }
}

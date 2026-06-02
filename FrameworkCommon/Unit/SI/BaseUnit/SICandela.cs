using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Unit
{
    public class SICandelra : SIValue
    {
        public SICandelra()
        {
            base.Unit = new SIUnit("cd", "candela", "luminous intensity", "j", new []{ "I_v" }, @"The luminous intensity, in a given direction, of a source that emits monochromatic radiation of frequency 5.4×10^14 hertz and that has a radiant intensity in that direction of 1 / 683  watt per steradian.");
        }

        public override int CompareTo(object obj)
        {
            SICandelra c = (SICandelra)obj;

            return Decimal.Compare(base.Value, c.Value);
        }
    }
}

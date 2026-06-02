using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Unit
{
    public class SIMeter : SIValue
    {
        public SIMeter()
        {
            base.Unit = new SIUnit("m", "metre", "length", "L", new[] {"l","h","a","b","x","y","r","etc"}, "The distance travelled by light in vacuum in 1 / 299792458 seconds.");
        }

        public override int CompareTo(object obj)
        {
            SIMeter c = (SIMeter)obj;

            return Decimal.Compare(base.Value, c.Value);
        }
    }
}

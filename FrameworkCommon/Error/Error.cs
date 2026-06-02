using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Error
{
    public abstract class Error
    {
        public string Reason { get; private set; } = string.Empty;

        public void SetReason(string reason)
        {
            this.Reason = reason;
        }

        public virtual void Clear()
        {
            this.Reason = string.Empty;
        }
    }
}

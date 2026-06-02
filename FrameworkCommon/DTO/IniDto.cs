using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.DTO
{
    public abstract class IniDto : Dto
    {
        public Uri ControllerIp = null;
        public Uri PowerIp = null;
    }
}

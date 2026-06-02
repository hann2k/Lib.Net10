using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.DTO
{
    public class StringDto : Dto
    {
        private string StringVALUE = string.Empty;
        public string StringValue {
            get => this.StringVALUE;
            set {
                this.StringVALUE = value;
                base.OnDtoUpdated();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Common.DTO;

namespace Framework.Common.Event
{
    public class DtoEventArgs : EventArgs
    {
        public Dto Dto;
    }

    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Value;
        public decimal ScaledValue;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Unit.SI
{
    /// <summary>
    /// SI 단위계에서 발생하는 모든 예외의 기본
    /// </summary>
    public class SIUnitException : System.Exception
    {
        public SIUnitException() : base() { }
        public SIUnitException(string message) : base(message) { }
        public SIUnitException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// SI Value 비교 or + - 연산시 서로 단위가 맞지 않을 때 발생하는 예외
    /// </summary>
    public class SIUnitMismatchException : SIUnitException
    {
        public SIUnitMismatchException() : base() { }
        public SIUnitMismatchException(string message) : base(message) { }
        public SIUnitMismatchException(string message, Exception inner) : base(message, inner) { }

        public SIUnitMismatchException(SIUnit a, SIUnit b) : this()
        {

        }
    }

    /// <summary>
    /// 곱 / 나눗셈 연산에서 단위변환이 불가능할 때 발생하는 예외
    /// </summary>
    public class SIUnitCannotConvertException : SIUnitException
    {
        public SIUnitCannotConvertException() : base() { }
        public SIUnitCannotConvertException(string message) : base(message) { }
        public SIUnitCannotConvertException(string message, Exception inner) : base(message, inner) { }

        public SIUnitCannotConvertException(SIUnit a, SIUnit b) : this()
        {

        }
    }
}

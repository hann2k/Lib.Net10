using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Unit.SI
{
    /// <summary>
    /// 국제도량형 단위계를 정의한 클래스
    /// 10진법에서만 사용되어야 한다.
    /// </summary>
    public class SIValue: IComparable
    {
        /// <summary>
        /// 연산으로 인해 단위가 변경될 때 발생되는 이벤트
        /// 아직 사용하지 않음
        /// </summary>
        public static event EventHandler<EventArgs> UnitChanged;

        /// <summary>
        /// SI 접두어(Prefix) 10의 배수
        /// </summary>
        public MetricPrefix Prefix = new MetricPrefix();

        /// <summary>
        /// 기본 수치
        /// </summary>
        public decimal BaseValue = 0;

        /// <summary>
        /// SI 단위
        /// </summary>
        public SIUnit Unit = SIUnit.Empty;

        #region 생성자

        public SIValue()
        {
            this.Unit = SIUnit.Empty;
		}

		/// <summary>
		/// 특정 단위의 변수를 생성한다.
		/// </summary>
		/// <param name="baseUnit"></param>
		public SIValue(SIUnitDefine baseUnit)
        {
            this.Unit = new SIUnit(baseUnit);
        }

        /// <summary>
        /// 특정 단위와 초기화된 값을 갖는 변수를 생성한다.
        /// </summary>
        /// <param name="baseUnit"></param>
        /// <param name="value"></param>
        public SIValue(SIUnitDefine baseUnit, decimal value) : this(baseUnit)
        {
            this.BaseValue = value; // * (decimal)Math.Pow(10, this.Prefix.OverFlow );
        }

        /// <summary>
        /// 특정 단위와 초기화된 값 및 SI 접두어 값을 갖는 변수를 생성한다.
        /// </summary>
        /// <param name="baseUnit"></param>
        /// <param name="value"></param>
        /// <param name="p"></param>
        public SIValue(SIUnitDefine baseUnit, decimal value, MetricPrefix p) : this(baseUnit, value)
        {
            this.Prefix = p;
        }

        /// <summary>
        /// 특정 단위와 초기화된 값 및 SI 접두어 값을 갖는 변수를 생성한다.
        /// </summary>
        /// <param name="baseUnit"></param>
        /// <param name="value"></param>
        /// <param name="p"></param>
        public SIValue(SIUnitDefine baseUnit, decimal value, MPS p) : this(baseUnit, value)
        {
            this.Prefix = new MetricPrefix(p);
        }

        /// <summary>
        /// 특정 단위와 초기화된 값 및 SI 접두어 값을 갖는 변수를 생성한다.
        /// </summary>
        /// <param name="baseUnit"></param>
        /// <param name="value"></param>
        /// <param name="p"></param>
        public SIValue(SIUnitDefine baseUnit, decimal value, int p) : this(baseUnit)
        {
            this.Prefix = new MetricPrefix(p);
            this.BaseValue = value * (decimal)Math.Pow(10, this.Prefix.OverFlow);
        }

        /// <summary>
        /// 특정 단위와 초기화된 값 및 SI 접두어 값을 갖는 변수를 생성한다.
        /// </summary>
        /// <param name="a"></param>
        public SIValue( SIValue a )
        {
            this.Unit = a.Unit;
            this.Prefix = a.Prefix;
            this.BaseValue = a.BaseValue;
        }

        #endregion

        #region 접두어

        /// <summary>
        /// 현재 변수의 접두어를 새로 설정하고 기준값을 변경한다.
        /// </summary>
        /// <param name="p"></param>
        public void SetPrefix(MetricPrefix p)
        {
            int gap = this.Prefix.Base10 - p.Base10;
            this.BaseValue = this.BaseValue * (decimal)Math.Pow(10, gap);
            this.Prefix.SetBase10(p.Base10);
        }

        /// <summary>
        /// 현재 변수의 접두어를 새로 설정하고 기준값을 변경한다.
        /// </summary>
        /// <param name="m"></param>
        public void SetPrefix(MPS m)
        {
            if (this.Prefix.Symbol != m)
            {
                MetricPrefix p = new MetricPrefix(m);
                this.SetPrefix(p);
            }
        }

        /// <summary>
        /// 현재 변수의 접두어를 새로 설정하고 기준값을 변경한다.
        /// </summary>
        /// <param name="m"></param>
        public void SetPrefix(int m)
        {
            MetricPrefix p = new MetricPrefix(m);
            this.SetPrefix(p);
        }

        #endregion

        /// <summary>
        /// 문자열로 출력한다.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (this.Prefix.Symbol == MPS.Zero )?
                $"{this.BaseValue} {this.Unit.Symbol}":
                $"{this.BaseValue} {this.Prefix.Symbol}{this.Unit.Symbol}";
        }

        /// <summary>
        /// 문자열을 SIValue로 변환한다.
        /// </summary>
        /// <param name="value"></param>
        public void SetSIValue(string value)
        {
            var numbers = new List<char>();

            // 문자열이 '-'로 시작하거나 숫자로 시작할 경우, 숫자가 아닌 문자가 나올 때까지 numbers 배열에 넣는다.
            var i = 0;

            if ( value[0] == '-')
            {
                numbers.Add('-');
                i = 1;
            }

            var dotCount = 0;

            for (; i < value.Length; i++)
            {
                // 숫자일 경우
                if (value[i] >= '0' && value[i] >= '9')
                {
                    numbers.Add(value[i]);
                }
                else if (value[i] == '.')
                {
                    dotCount++;
                    if (dotCount == 1)
                    {
                        numbers.Add(value[i]);
                    }
                    else
                    {
                        // '.' 이 2개 이상이라 숫자가 아니다. 예외를 리턴해야 한다.
                        throw new ArgumentException($"제공된 {value}는 SI 표준으로 변환할 수 없는 문자열입니다.");
                    }
                }
                else
                {
                    break;
                }
            }

            // 숫자를 추출한다.
            decimal number = Convert.ToDecimal(numbers.ToString());

			// 단위를 추출한다.
			(string Unit, bool Result) unit = SIUnit.ExtractUnit(value);

			// SI접두어 배수를 추출한다.
			(string Symbol, bool Result) prefix = SIUnit.ExtractOthers(value, unit.Unit);
            
            // 적용한다.
            // this.Prefix.SetSymbol(prefix.Symbol);
        }

        int IComparable.CompareTo(object obj)
        {
            SIValue c = (SIValue)obj;

            if ( this.Unit != c.Unit )
            {
                throw new SIUnitMismatchException();
            }

            c.Prefix = this.Prefix;

            return Decimal.Compare(this.BaseValue, c.BaseValue);
        }

        protected static void OnUnitChanged()
        {
            UnitChanged?.Invoke(null, EventArgs.Empty);
        }

        #region 사칙연산

        public static SIValue operator +(SIValue a) => a;

        public static SIValue operator -(SIValue a)
        {
            a.BaseValue = -a.BaseValue;
            return a;
        }

        /// <summary>
        /// 더하기 연산에서 Prefix 는 첫번째 항의 Prefix가 된다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIValue operator +(SIValue a, SIValue b)
        {
            if (a.Unit != b.Unit)
            {
                throw new SIUnitMismatchException();
            }

            SIValue c = new SIValue(a);

            c.Prefix = a.Prefix;

            a.SetPrefix(c.Prefix);
            b.SetPrefix(c.Prefix);

            c.BaseValue = (a.BaseValue + b.BaseValue) * (decimal)Math.Pow(10, c.Prefix.OverFlow);
            // c.Unit = a.Unit;

            return c;
        }

        /// <summary>
        /// 단위 변수에 일반상수를 더하면, 동종의 단위변수를 더한 것으로 판정한다.
        /// 이 케이스는 사용하지 않을 수 있다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //public static SIValue operator +(SIValue a, decimal b)
        //{
        //    a.BaseValue += b;

        //    return a;
        //}

        /// <summary>
        /// 단위 변수에 일반상수를 더하면, 동종의 단위변수를 더한 것으로 판정한다.
        /// 이 케이스는 사용하지 않을 수 있다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //public static SIValue operator +(decimal a, SIValue b)
        //{
        //    return b + a;
        //}

        public static SIValue operator -(SIValue a, SIValue b)
        {
            if (a.Unit != b.Unit)
            {
                throw new SIUnitMismatchException(a.Unit, b.Unit);
            }

            SIValue c = new SIValue(a);

            c.Prefix = a.Prefix;

            a.SetPrefix(c.Prefix);
            b.SetPrefix(c.Prefix);

            c.BaseValue = (a.BaseValue - b.BaseValue) * (decimal)Math.Pow(10, c.Prefix.OverFlow);
            c.Unit = a.Unit;

            return c;
        }

        /// <summary>
        /// 샘플을 처리해 가면서 만들자.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIValue operator *(SIValue a, SIValue b)
        {
            SIValue c = new SIValue(a);

            c.Prefix = a.Prefix + b.Prefix;

            try
            {
                c.Unit = SIUnit.ConvertTimes(a.Unit, b.Unit);
                OnUnitChanged();
            }
            catch(SIUnitCannotConvertException)
            {
                throw new SIUnitCannotConvertException();
            }

            c.BaseValue = a.BaseValue * b.BaseValue * (decimal)Math.Pow(10, c.Prefix.OverFlow);

            return c;
        }

        /// <summary>
        /// 단위변수에 일반상수 곱하기
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIValue operator *(SIValue a, decimal b)
        {
            a.BaseValue *= b;

            return a;
        }

        /// <summary>
        /// 단위변수에 일반상수 곱하기
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIValue operator *(decimal a, SIValue b)
        {
            return b * a;
        }

        /// <summary>
        /// 샘플을 처리해 가면서 만들자.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIValue operator /(SIValue a, SIValue b)
        {
            if (b.BaseValue == 0)
            {
                throw new DivideByZeroException();
            }

            if (a.Unit == b.Unit)
            {
                throw new SIUnitCannotConvertException();
            }

            SIValue c = new SIValue(a);

            try
            { 
                c.Unit = SIUnit.ConvertDevide(a.Unit, b.Unit);
                OnUnitChanged();
            }
            catch (SIUnitCannotConvertException)
            {
                throw new SIUnitCannotConvertException();
            }

            c.Prefix = a.Prefix - b.Prefix;
            c.BaseValue = a.BaseValue / b.BaseValue * (decimal)Math.Pow(10, c.Prefix.OverFlow);

            return c;
        }

        /// <summary>
        /// 단위변수에 일반상수 나누기
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIValue operator /(SIValue a, decimal b)
        {
            a.BaseValue /= b;

            return a;
        }

        /// <summary>
        /// 단위변수에 일반상수 나누기
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIValue operator /(decimal a, SIValue b)
        {
            // 시간을 Hertz로 변환한다.
            if (b.Unit.BaseUnit == SIUnitBase.Second)
            {
                b.Unit.SecondToHertz();
                b.Prefix = -b.Prefix;
            }

            // Hertz를 시간으로 변환한다.
            else if (b.Unit.BaseUnit == SIUnitBase.Hertz)
            {
                b.Unit.HertzToSecond();
                b.Prefix = -b.Prefix;
            }

            b.BaseValue = a / b.BaseValue;

            return b;
        }

        #endregion

        #region 비교연산

        public static bool operator ==(SIValue a, SIValue b)
        {
            // 접두어를 같게 만든다.
            b.Prefix = PreCompare(a, b);

            // 값이 같은지 검사한다.
            return (a.BaseValue == b.BaseValue) ? true : false;
        }

        public static bool operator !=(SIValue a, SIValue b)
        {
            return a == b ? false : true;
        }

        public override bool Equals(object obj)
        {
            var o = (SIValue) obj;

            return this == o;
        }

        public override int GetHashCode() => (this.BaseValue, this.Prefix, this.Unit).GetHashCode();

        public static bool operator >(SIValue a, SIValue b)
        {
            // 접두어를 같게 만든다.
            b.Prefix = PreCompare(a, b);

            // 값이 같은지 검사한다.
            return (a.BaseValue > b.BaseValue) ? true : false;
        }

        public static bool operator <(SIValue a, SIValue b)
        {
            // 접두어를 같게 만든다.
            b.Prefix = PreCompare(a, b);

            // 값이 같은지 검사한다.
            return (a.BaseValue < b.BaseValue) ? true : false;
        }

        public static bool operator >=(SIValue a, SIValue b)
        {
            // 접두어를 같게 만든다.
            b.Prefix = PreCompare(a, b);

            // 값이 같은지 검사한다.
            return (a.BaseValue >= b.BaseValue) ? true : false;
        }

        public static bool operator <=(SIValue a, SIValue b)
        {
            // 접두어를 같게 만든다.
            b.Prefix = PreCompare(a, b);

            // 값이 같은지 검사한다.
            return (a.BaseValue <= b.BaseValue) ? true : false;
        }

        private static MetricPrefix PreCompare(SIValue a, SIValue b)
        {
            if (a.Unit != b.Unit)
            {
                throw new SIUnitMismatchException();
            }

            // 접두어를 같게 만든다.
            return a.Prefix;
        }

        #endregion



        //private static bool CompareUnit(SIUnit a, SIUnit b)
        //{
        //    return (a.Symbol != b.Symbol) ? false : true;
        //}

    }
}

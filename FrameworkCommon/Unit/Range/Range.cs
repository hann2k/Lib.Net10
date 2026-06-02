using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Unit
{
    /// <summary>
    /// 최대값과 최소값을 지정하는 클래스
    /// </summary>
    [Browsable(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Range
    {
        /// <summary>
        /// 최소값
        /// </summary>
        public double Min { get; set; } = 0.0;

        /// <summary>
        /// 최대값
        /// </summary>
        public double Max { get; set; } = 0.0;

        /// <summary>
        /// 추가값
        /// </summary>
        public double Offset_1 { get; set; } = 0.0;
        public double Offset_2 { get; set; } = 0.0;

        /// <summary>
        /// 최대값과 최소값을 설정한다.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Range( double min, double max )
        {
            this.Min = min;
            this.Max = max;

            this.Exchange();
        }

        /// <summary>
        /// "min,max" 형태의 문자열로 된 최대값 최소값 설정.
        /// 범위 숫자의 구분자는 ',' 임
        /// </summary>
        /// <param name="str"></param>
        public Range(string name, string str)
        {
            string[] strs = str.Split(',');
            
            try
            {
                if (strs.Length >= 2)
                {
                    this.Min = Convert.ToDouble(strs[0]);
                    this.Max = Convert.ToDouble(strs[1]);
                }

                if (strs.Length >= 3)
                {
                    this.Offset_1 = Convert.ToDouble(strs[2]);
                }

                if (strs.Length >= 4)
                {
                    this.Offset_2 = Convert.ToDouble(strs[3]);
                }


                this.Exchange();
            }
            catch
            {
                Logger.Log.Ins.Error($"{name} 범위 값 '{str}' 을 읽는 중에 포맷 오류가 발생하였습니다.");
                // throw new Exception();
            }
        }

        private void Exchange()
        {
            if (this.Min > this.Max)
            {
                (this.Min, this.Max) = (this.Max, this.Min);
            }
        }

        public override string ToString()
        {
            return $"{this.Min} ~ {this.Max}";
        }

        public bool In(double value, bool borderInclude)
        {
            
            if ( borderInclude )
            {
                // Console.WriteLine($"Range In : {this.Min} <= {value} <= {this.Max}");
                return this.Min <= value && this.Max >= value;
            }
            else
            {
                // Console.WriteLine($"Range In : {this.Min} < {value} < {this.Max}");
                return this.Min < value && this.Max > value;
            }
        }
    }

    public class RangeError : Error.Error
    {
        public bool ErrorFound { get => (this.MinError || this.MaxError); }

        public bool MinError { get; private set; } = false;
        public bool MaxError { get; private set; } = false;

        public void SetMinError()
        {
            this.MinError = true;
        }

        public void SetMaxError()
        {
            this.MaxError = true;
        }

        public override void Clear()
        {
            base.Clear();

            this.MinError = false;
            this.MaxError = false;
        }
    }
}

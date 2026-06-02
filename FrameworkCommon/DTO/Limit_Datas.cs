using Framework.Common.Logger;
using Framework.Common.Unit;
using Framework.Common.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Framework.Common.Converter;

namespace Framework.Common.DTO
{
    public interface IGetBytes
    {
        /// <summary>
        /// 지정된 데이터를 바이트 배열로 변경한다.
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();
    }

    public interface IToHexString
    {
        string ToHex();
    }

    public interface ITo1000CommaString
    {
        string To1000CommaString();
    }

    public abstract class Limit_Datas : Dto
    {
        public RangeError Error { get; protected set; } = new RangeError();

        protected readonly char[] Separators = { ',' };
    }

    public abstract class Limit<T> : Limit_Datas
    {


        #region 3. 기본 값 관리 / 출력

        /// <summary>
        /// 통신에 사용되는 순수한 값
        /// </summary>
        protected T ORIGIN_VALUE;

        /// <summary>
        /// 원래 값을 출력한다.
        /// </summary>
        public T Value => this.ORIGIN_VALUE;

        /// <summary>
        /// 스케일링 된 값을 출력한다.
        /// </summary>
        public virtual string FullString => $"{this.ORIGIN_VALUE}";

        public abstract string DebugString();

        #endregion

        #region 4. 값 넣기(스케일링된 값)

        /// <summary>
        /// 문자열을 기반으로 값을 설정한다.
        /// 구분자로 분리한 문자열 개수에 따라 적용 방식이 다르다. 구분자는 ','를 사용한다.
        /// 문자열 4: value,min,max,scale
        /// 문자열 3: value,min,max (scale:1)
        /// 문자열 2: value,scale   (min:default, max:default)
        /// 문자열 1: value         (min:default, max:default, scale:1)
        /// </summary>
        /// <param name="str">설정할 숫자의 문자열</param>
        /// <param name="single">true : 문자열에서 구분자','를 제거하고 하나의 숫자로 설정한다. false : 기본형</param>
        public abstract void Set(string str, bool single = false);

        #endregion

        #region 5. 값 넣기(원래 값)



        #endregion

        #region 6. 이벤트

        /// <summary>
        /// 이벤트
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

        protected virtual void OnValueChanged()
        {
			var args = new ValueChangedEventArgs<T> {
				Value = this.ORIGIN_VALUE
			};

			this.SetValueChange(args);
        }

		protected void SetValueChange(ValueChangedEventArgs<T> args) => this.ValueChanged?.Invoke(this, args);

		#endregion


	}

    public abstract class Limit_Number<T> : Limit<T>
    {
        #region 1. 배율. (Scale)

        /// <summary>
        /// 배율
        /// </summary>
        protected decimal SCALE = 1.0M;

        /// <summary>
        /// 소수점 자리수 추출기
        /// </summary>
        /// <param name="real"></param>
        /// <returns></returns>
        public int DecimalPlace {
            get {
                // 소수점 부분만 추출한다.
                // 정밀도 때문에 decimal로만 추출한다.
                var decimalPart = this.SCALE - (int)this.SCALE;

                var count = 0;

                // 10을 곱해서 정수부분을 제거하는 행위의 회수를 센다. 최종값이 0보다 작아지는 회수가 소수점의 자리수이다.
                while (decimalPart > 0)
                {
                    // 10을 곱한다.
                    decimalPart = decimalPart * 10;

                    // 곱한 회수를 센다.
                    count++;

                    // 소수점 부분만 추출한다.
                    decimalPart = decimalPart - (int)decimalPart;
                }

                return count;
            }
        }

		/// <summary>
		/// 설정된 스케일을 구한다.
		/// </summary>
		public decimal Scale => this.SCALE;

		/// <summary>
		/// 저장된 값에 배율을 적용한다. 배율은 0이하로 입력될 수 없다.
		/// </summary>
		/// <param name="scale"></param>
		public void SetScale(decimal scale)
        {
            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException("Scale은 0이하로 설정될 수 없습니다.");
            }

            this.SCALE = scale;
        }

        #endregion

        #region 2. 최대, 최소값

        /// <summary>
        /// 배율 적용된 제한 최소값. 
        /// </summary>
        protected decimal MIN;

        /// <summary>
        /// 배율 적용된 제한 최대값.
        /// </summary>
        protected decimal MAX;

		/// <summary>
		/// 설정된 최소값을 구한다.
		/// </summary>
		public decimal Min => this.MIN;

		/// <summary>
		/// 설정된 최대값을 구한다.
		/// </summary>
		public decimal Max => this.MAX;

		protected abstract decimal LimitCheckMin(decimal min);
        protected abstract decimal LimitCheckMax(decimal max);

        /// <summary>
        /// 값의 범위를 제한한다.
        /// 초기값은 해당 변수 타입의 최소 ~ 최대치이다.
        /// max, min의 순서에 상관 없이, 더 작은 값이 min, 큰 값이 max로 설정된다.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetLimit(decimal min, decimal max)
        {
            this.MIN = this.LimitCheckMin(min);
            this.MAX = this.LimitCheckMax(max);
            //if (this.LimitCheck(min))
            //{
            //    this.MIN = min;
            //}
            //else
            //{
            //    // throw new ArgumentOutOfRangeException("최소값의 지정 범위는 각 타입의 최소~최대 사이의 값으로 지정해야 합니다.");
            //}

            //if (this.LimitCheck(max))
            //{
            //    this.MAX = max;
            //}
            //else
            //{
            //    throw new ArgumentOutOfRangeException("최대값의 지정 범위는 각 타입의 최소~최대 사이의 값으로 지정해야 합니다.");
            //}

            if (max == min)
            {
                throw new ArgumentException("최소, 최대값이 같아서는 안됩니다.");
            }

            if (max < min)
            {
                (this.MIN, this.MAX) = (this.MAX, this.MIN);
                //var t = this.MIN;
                //this.MIN = this.MAX;
                //this.MAX = t;
            }
        }

		protected bool CheckLimit(decimal scaledvalue) => scaledvalue < this.Min || scaledvalue > this.Max ? false : false;

		protected (decimal, RangeError) FilteringMaxScaledValue(decimal scaledvalue, bool hex = false)
        {
            var rv = scaledvalue;
            var error = new RangeError();

            if (scaledvalue > this.MAX)
            {
				var reason = hex ? string.Format("입력된 값(0x{0:X})이 최대값(0x{1:X})보다 큽니다.", (ulong)rv, (ulong)this.Max) : $"입력된 값({rv})이 최대값({this.MAX})보다 큽니다.";
				error.SetReason(reason);

				//if (hex)
    //            {
    //                error.SetReason(string.Format("입력된 값(0x{0:X})이 최대값(0x{1:X})보다 큽니다.", (ulong)rv, (ulong)this.Max));
    //            }
    //            else
    //            {
    //                error.SetReason($"입력된 값({rv})이 최대값({this.MAX})보다 큽니다.");
    //            }

                rv = this.MAX;
                error.SetMaxError();
            }

            return (rv, error);
        }

        protected (decimal, RangeError) FilteringMinScaledValue(decimal scaledvalue, bool hex = false)
        {
            var rv = scaledvalue;
            var error = new RangeError();

            if (scaledvalue < this.MIN)
            {
				var reason = hex
					? string.Format("입력된 값(0x{0:X})이 최대값(0x{1:X})보다 작습니다.", (ulong)rv, (ulong)this.Min)
					: $"입력된 값({rv})이 최소값({this.MIN})보다 작습니다.";
				error.SetReason(reason);

				//if (hex)
    //            {
    //                error.SetReason(string.Format("입력된 값(0x{0:X})이 최대값(0x{1:X})보다 작습니다.", (ulong)rv, (ulong)this.Min));
    //            }
    //            else
    //            {

    //                error.SetReason($"입력된 값({rv})이 최소값({this.MIN})보다 작습니다.");
    //            }

                rv = this.MIN;
                error.SetMinError();

            }

            return (rv, error);
        }

        #endregion

        #region 3. 값 출력

        public override string DebugString()
        {
			return this.SCALE == 1
				? $"{this.Value,8} [{this.ByteString}]"
				: $"{this.Value,8} ({this.ScaledValue(),8}) [{this.ByteString}]";
		}

        /// <summary>
        /// 스케일링 된 값을 출력한다.
        /// 소수점이 있을 수 있어, Decimal로 통일한다.
        /// 결과는 MIN < scaledValue < MAX 사이에 있어야 한다.
        /// </summary>
        public abstract decimal ScaledValue();

        /// <summary>
        /// 스케일을 적용하여 값을 추출한다. 지정한 포인트에서 소수점 반올림이 일어난다.
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public decimal ScaledValue(int point) => (decimal)Math.Round(this.ScaledValue(), point, MidpointRounding.AwayFromZero);

        /// <summary>
        /// 스케일링 된 값을 문자열로 출력한다.. 0으로 끝나는 소수점은 삭제한다.
        /// </summary>
        public string StrScaledValue => this.ScaledValue().ToString("G29");

        /// <summary>
        /// 스케일링 된 값을 출력한다.
        /// </summary>
        public override string FullString => $"{this.ScaledValue()},{this.MIN},{this.MAX},{this.SCALE}";

        /// <summary>
        /// 스케일링 된 값을 4가지 방법으로 출력한다.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public virtual string ToScaleString(int num)
        {
            switch (num)
            {
                case 1:
                    return $"{this.ScaledValue()}";
                case 2:
                    return $"{this.ScaledValue()},{this.SCALE}";
                case 3:
                    return $"{this.ScaledValue()},{this.MIN},{this.MAX}";
                case 4:
                default:
                    return this.FullString;
            }
        }

        public abstract string Hex { get; }

        #endregion

        #region 4. 값 넣기(스케일링된 값)



        /// <summary>
        /// 최대값 검사 및 값 입력
        /// </summary>
        /// <param name="v"></param>
        public virtual void SetMax(decimal v, bool hex = false)
        {
            // Console.WriteLine($"SetMax(v:{v});");
            (v, base.Error) = this.FilteringMaxScaledValue(v, hex);

            this.ORIGIN_VALUE = this.ToUnscaledValue(v);

            this.OnValueChanged();
        }

        /// <summary>
        /// 최소값 검사 및 값 입력
        /// </summary>
        /// <param name="v"></param>
        /// <param name="hex"></param>
        public virtual void SetMin(decimal v, bool hex = false)
        {
            // Console.WriteLine($"SetMin(v:{v});");
            (v, base.Error) = this.FilteringMinScaledValue(v, hex);

            this.ORIGIN_VALUE = this.ToUnscaledValue(v);

            this.OnValueChanged();
        }

        public void Set(decimal v)
        {
            // 최대값 필터 검사
            this.SetMax(v);

            // 입력한 값이 최대값보다 낮으면
            if (!base.Error.ErrorFound)
            {
                // 최소값 필터 검사
                this.SetMin(v);
            }
        }

		/// <summary>
		/// 문자열을 기반으로 값을 설정한다.<br/>
		/// 구분자로 분리한 문자열 개수에 따라 적용 방식이 다르다. 구분자는 ','를 사용한다.<br/>
		/// 문자열 4: value,min,max,scale<br/>
		/// 문자열 3: value,min,max (scale:1)<br/>
		/// 문자열 2: value,scale   (min:default, max:default)<br/>
		/// 문자열 1: value         (min:default, max:default, scale:1)<br/>
		/// </summary>
		/// <param name="str">설정할 숫자의 문자열</param>
		/// <param name="single">true : 문자열에서 구분자','를 제거하고 하나의 숫자로 설정한다. false : 기본형</param>
		public override void Set(string str, bool single = false)
        {
            var strs = single ? new string[] { str } : str.Split(this.Separators);

            switch (strs.Length)
            {
                case 4:
                {
                    var min = this.ToDecimal(strs[1], decimal.MinValue);
                    var max = this.ToDecimal(strs[2], decimal.MaxValue);
                    this.SetScale(this.ToDecimal(strs[3], 1));
                    this.SetLimit(min, max);
                    this.SetMax(this.ToDecimal(strs[0], 0));
                    this.SetMin(this.ToDecimal(strs[0], 0));
                }
                break;

                case 3:
                {
                    var min = this.ToDecimal(strs[1], decimal.MinValue);
                    var max = this.ToDecimal(strs[2], decimal.MaxValue);
                    this.SetScale(1);
                    this.SetLimit(min, max);
                    this.SetMax(this.ToDecimal(strs[0], 0));
                    this.SetMin(this.ToDecimal(strs[0], 0));
                }
                break;

                case 2:
                    this.SetScale(this.ToDecimal(strs[1], 1));
                    this.SetMax(this.ToDecimal(strs[0], 0));
                    this.SetMin(this.ToDecimal(strs[0], 0));
                    break;

                case 1:
                    this.SetScale(1);
                    this.Set(this.ToDecimal(strs[0], 0));
                    // this.SetMax(this.ToDecimal(strs[0], 0));
                    // this.SetMin(this.ToDecimal(strs[0], 0));
                    break;

                default:
                    throw new ArgumentException($"입력값 '{str}'을 Limit_Double 타입으로 변환할 수 없습니다.");
            }
        }

        #endregion

        #region 5. 값 넣기(원래 값)

        public abstract void SetUV(T v);

        public abstract void SetUV(byte[] array, int index);

        /// <summary>
        /// 입력된 값에 지정된 스케일로 나누어 원래 크기의 값을 구한다.
        /// </summary>
        /// <param name="v"></param>
        protected abstract T ToUnscaledValue(decimal v);

        /// <summary>
        /// 스케일링된 숫자가 포함된 문자열에서 원래 크기의 값을 구한다.
        /// </summary>
        /// <param name="v"></param>
        protected abstract T ToUnscaledValue(string v);

        public abstract void Clear();

        #endregion

        protected abstract string ByteString { get; }

		/// <summary>
		/// 저장된 값을 출력한다.
		/// 배율적용값, 배율, 원래값(전송용), 원래값(Byte String)
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{this.ScaledValue()}, {this.Scale}, {this.MIN}, {this.MAX}, {this.Value}, {this.ByteString}";

		protected override void OnValueChanged()
        {
			var args = new ValueChangedEventArgs<T> {
				Value = this.ORIGIN_VALUE,
				ScaledValue = this.ScaledValue()
			};

			base.SetValueChange(args);
        }



        private string GetDecimalPlaceString()
        {
            var f = string.Empty;
            switch (this.DecimalPlace)
            {
                case 6:
                    f = ".000000";
                    break;
                case 5:
                    f = ".00000";
                    break;
                case 4:
                    f = ".0000";
                    break;
                case 3:
                    f = ".000";
                    break;
                case 2:
                    f = ".00";
                    break;
                case 1:
                    f = ".0";
                    break;

                default:
                    break;
            }

            return "#,#" + f;
        }

        public virtual string To1000CommaString()
        {
            // Console.WriteLine("Origin : " + this.ORIGIN_VALUE);

            var sv = this.ScaledValue();

            var decimalFilter = this.GetDecimalPlaceString();

            return sv == 0 ? "0" : sv.ToString(decimalFilter);
        }

		/// <summary>
		/// 문자열을 decimal로 변경한다.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultvalue">파싱실패시 적용되는 기본값</param>
		/// <returns></returns>
		protected decimal ToDecimal(string str, decimal defaultvalue) => decimal.TryParse(str, out var conv) ? conv : defaultvalue;

		public abstract void Set(Limit_Number<T> v);
    }

    /// <summary>
    /// 0 과 1 값만을 갖는 바이트 크기의 데이터
    /// </summary>
    public class Limit_Bool : Limit<bool>
    {
        /// <summary>
        /// Limit_one 타입에 한하여, 1:true, 0:false 형태로 제공한다.
        /// </summary>
        public bool Checked {
            get => this.ORIGIN_VALUE;
            set {
                this.ORIGIN_VALUE = value;
                base.OnValueChanged();
            }
        }

        public byte Byte => (byte)(this.Checked ? 1 : 0);

        public override string FullString => $"{this.Checked}";

        public Limit_Bool()
        {
        }

		public Limit_Bool(bool v) : this() => this.Checked = v;

		public void Set(bool b) => this.ORIGIN_VALUE = b;

		/// <summary>
		/// 입력시 숫자로 입력받을 일이 있어 사용함.
		/// </summary>
		/// <param name="b"></param>
		public void Set(byte b) => this.ORIGIN_VALUE = b == 1 ? true : false;

		public override void Set(string str, bool single = false)
        {
            try
            {
				var b = Convert.ToBoolean(str);
            }
            catch (Exception ex)
            {
                Log.Ins.Exception($"{str} 은 Limit_Bool에 입력할 수 없습니다.");
                Log.Ins.Exception(ex);
            }
        }

		public override string DebugString() => this.Checked ? "1(T)" : "0(F)";
	}

    /// <summary>
    /// -128 ~ 127 사이의 값을 갖는 바이트 크기의 데이터
    /// </summary>
    public class Limit_SByte : Limit_Number<sbyte>
    {
        public override string Hex => string.Format("{0,2:X2}", this.ORIGIN_VALUE);

        public sbyte Byte => this.ORIGIN_VALUE;

        protected override string ByteString => this.Hex;

		public Limit_SByte() => this.Clear();

		public Limit_SByte(sbyte value) : this()
        {
            this.SetMax(value);
            this.SetMin(value);
        }

        public override decimal ScaledValue() => this.ORIGIN_VALUE * base.SCALE;

		/// <summary>
		/// Unscale 된 Base값을 넣는다.
		/// </summary>
		/// <param name="v"></param>
		public override void SetUV(sbyte v) => this.ORIGIN_VALUE = v;

		public override void SetUV(byte[] array, int index) => this.SetUV((sbyte)array[index]);

		protected override sbyte ToUnscaledValue(decimal v) => (sbyte)Math.Truncate(v / this.SCALE);
		protected override sbyte ToUnscaledValue(string v)
        {
			var c = Convert.ToSByte(v);
            return this.ToUnscaledValue(c);
        }

        public override void Set(Limit_Number<sbyte> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		protected override decimal LimitCheckMin(decimal value) => value < sbyte.MinValue ? sbyte.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > sbyte.MaxValue ? sbyte.MaxValue : value;

		public override void Clear()
        {
            base.MIN = sbyte.MinValue;
            base.MAX = sbyte.MaxValue;
            base.SCALE = 1;
        }
    }

    /// <summary>
    /// 최대 255의 값을 갖는 바이트 크기의 데이터
    /// </summary>
    public class Limit_Byte : Limit_Number<byte>
    {
        public override string Hex => string.Format("{0,2:X2}", this.ORIGIN_VALUE);

        public byte Byte => this.ORIGIN_VALUE;

        protected override string ByteString => this.Hex;

		public Limit_Byte() => this.Clear();

		public Limit_Byte(byte value) : this()
        {
            this.SetMax(value);
            this.SetMin(value);
        }

        public override decimal ScaledValue() => this.ORIGIN_VALUE * base.SCALE;

		/// <summary>
		/// Unscale 된 Base값을 넣는다.
		/// </summary>
		/// <param name="v"></param>
		public override void SetUV(byte v) => this.ORIGIN_VALUE = v;

		public override void SetUV(byte[] array, int index) => this.SetUV(array[index]);

		public void Set(string str, int _base)
        {
            if (_base == 16)
            {
                var b = Convert.ToByte(str, 16);
                this.SetMax(b);
                this.SetMin(b);
            }
            else
            {
                this.Set(str);
            }
        }

		protected override byte ToUnscaledValue(decimal v) => (byte)(v / this.SCALE);

		protected override byte ToUnscaledValue(string v)
        {
			var c = Convert.ToByte(v);
            return this.ToUnscaledValue(c);
        }

        public override void Set(Limit_Number<byte> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		protected override decimal LimitCheckMin(decimal value) => value < byte.MinValue ? byte.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > byte.MaxValue ? byte.MaxValue : value;

		public override void Clear()
        {
            base.MIN = byte.MinValue;
            base.MAX = byte.MaxValue;
            base.SCALE = 1;
        }
    }

    /// <summary>
    /// 최대 signed 2바이트의 값을 갖는 데이터
    /// </summary>
    public class Limit_Int16 : Limit_Number<short>, IGetBytes
    {
        public override string Hex => string.Format("{0,4:X4}", this.ORIGIN_VALUE);

        protected override string ByteString => $"{ByteConverter.ToHexString(this.GetBytes())}";

		public Limit_Int16() => this.Clear();

		public Limit_Int16(short value) : this()
        {
            this.SetMax(value);
            this.SetMin(value);
            // Log.Ins.Debug($"Limit_Int16({value})");
        }

        public override decimal ScaledValue() => this.ORIGIN_VALUE * base.SCALE;

        public byte[] GetBytes()
        {
            var b = BitConverter.GetBytes(this.ORIGIN_VALUE);

            return b;
        }

        /// <summary>
        /// Unscale 된 Base값을 넣는다.
        /// </summary>
        /// <param name="v"></param>
        public override void SetUV(short v)
        {
            // Log.Ins.Debug($"Limit_Int16.SetUV({v})");
            this.ORIGIN_VALUE = v;
        }

        public override void SetUV(byte[] array, int index)
        {
			var bs = ByteConverter.ToHexString(array, index, 2);

			var b = BitConverter.ToInt16(array, index);

            this.SetUV(b);
        }

		protected override short ToUnscaledValue(decimal v) => (short)Math.Truncate(v / this.SCALE);

		protected override short ToUnscaledValue(string s)
        {
			var c = Convert.ToInt16(s);

            return this.ToUnscaledValue(c);
        }

        //public override void SetLimit(decimal min, decimal max)
        //{
        //    this.MIN = min < short.MinValue ? short.MinValue : min;
        //    this.MAX = max > short.MaxValue ? short.MaxValue : max;
        //}

        public override void Set(Limit_Number<short> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		protected override decimal LimitCheckMin(decimal value) => value < short.MinValue ? short.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > short.MaxValue ? short.MaxValue : value;

		public override void Clear()
        {
            base.MIN = short.MinValue;
            base.MAX = short.MaxValue;
            base.SCALE = 1;
        }
    }

    /// <summary>
    /// 0 ~ 65535 값을 갖는 데이터
    /// </summary>
    public class Limit_UInt16 : Limit_Number<ushort>, IGetBytes
    {
        public override string Hex => string.Format("{0,4:X4}", this.ORIGIN_VALUE);

        protected override string ByteString => $"{ByteConverter.ToHexString(this.GetBytes())}";

		public Limit_UInt16() => this.Clear();

		public override decimal ScaledValue() => this.ORIGIN_VALUE * base.SCALE;

        public Limit_UInt16(ushort value) : this()
        {
            this.SetMax(value);
            this.SetMin(value);
        }

		public byte[] GetBytes() => BitConverter.GetBytes(this.ORIGIN_VALUE);

		/// <summary>
		/// Unscale 된 Base값을 넣는다.
		/// </summary>
		/// <param name="v"></param>
		public override void SetUV(ushort v) => this.ORIGIN_VALUE = v;

		public override void SetUV(byte[] array, int index) => this.SetUV(BitConverter.ToUInt16(array, index));

		protected override ushort ToUnscaledValue(decimal v) => (ushort)Math.Truncate(v / this.SCALE);

		protected override ushort ToUnscaledValue(string v)
        {
            var c = Convert.ToUInt16(v);
            return this.ToUnscaledValue(c);
        }

        public override void Set(Limit_Number<ushort> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		protected override decimal LimitCheckMin(decimal value) => value < ushort.MinValue ? ushort.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > ushort.MaxValue ? ushort.MaxValue : value;

		public override void Clear()
        {
            base.MIN = ushort.MinValue;
            base.MAX = ushort.MaxValue;
            base.SCALE = 1;
        }
    }

    public class Limit_Int32 : Limit_Number<int>, IGetBytes
    {
        public override string Hex => string.Format("{0,8:X8}", this.ORIGIN_VALUE);

        protected override string ByteString => $"{ByteConverter.ToHexString(this.GetBytes())}";

		public Limit_Int32() => this.Clear();

		public Limit_Int32(int value) : this()
        {
            this.SetMax(value);
            this.SetMin(value);
        }

        public override decimal ScaledValue() => this.ORIGIN_VALUE * base.SCALE;

		public byte[] GetBytes() => BitConverter.GetBytes(this.ORIGIN_VALUE);

		/// <summary>
		/// Unscale 된 Base값을 넣는다.
		/// </summary>
		/// <param name="v"></param>
		public override void SetUV(int v) => this.ORIGIN_VALUE = v;

		public override void SetUV(byte[] array, int index) => this.SetUV(BitConverter.ToInt32(array, index));

		protected override int ToUnscaledValue(decimal v) => (int)Math.Truncate(v / this.SCALE);

		protected override int ToUnscaledValue(string v)
        {
			var c = Convert.ToInt32(v);
            return this.ToUnscaledValue(c);
        }

        public override void Set(Limit_Number<int> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		protected override decimal LimitCheckMin(decimal value) => value < int.MinValue ? int.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > int.MaxValue ? int.MaxValue : value;

		public override void Clear()
        {
            base.MIN = int.MinValue;
            base.MAX = int.MaxValue;
            base.SCALE = 1;
        }
    }

    public class Limit_UInt32 : Limit_Number<uint>, IGetBytes
    {
        public override string Hex => string.Format("{0,8:X8}", this.ORIGIN_VALUE);

        protected override string ByteString => $"{ByteConverter.ToHexString(this.GetBytes())}";

		public Limit_UInt32() => this.Clear();

		public Limit_UInt32(uint value) : this()
        {
            this.SetMax(value);
            this.SetMin(value);
        }

        public override decimal ScaledValue() => this.ORIGIN_VALUE * base.SCALE;

		public byte[] GetBytes() => BitConverter.GetBytes(this.ORIGIN_VALUE);

		/// <summary>
		/// Unscale 된 Base값을 넣는다.
		/// </summary>
		/// <param name="v"></param>
		public override void SetUV(uint v) => this.ORIGIN_VALUE = v;

		public override void SetUV(byte[] array, int index) => this.SetUV(BitConverter.ToUInt32(array, index));

		protected override uint ToUnscaledValue(decimal v) => (uint)Math.Truncate(v / this.SCALE);

		protected override uint ToUnscaledValue(string v)
        {
			var c = Convert.ToUInt32(v);
            return this.ToUnscaledValue(c);
        }

        public override void Set(Limit_Number<uint> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		protected override decimal LimitCheckMin(decimal value) => value < uint.MinValue ? uint.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > uint.MaxValue ? uint.MaxValue : value;

		public override void Clear()
        {
            base.MIN = uint.MinValue;
            base.MAX = uint.MaxValue;
            base.SCALE = 1;
        }
    }

    public class Limit_Double : Limit_Number<double>, IGetBytes
    {
        public override string Hex => throw new NotImplementedException();

        protected override string ByteString => $"{ByteConverter.ToHexString(this.GetBytes())}";

		public Limit_Double() => this.Clear();

		public Limit_Double(double value) : this()
        {
            this.SetMax((decimal)value);
            this.SetMin((decimal)value);
        }

		public Limit_Double(string str) => this.Set(str);

		public override decimal ScaledValue() => (decimal)this.ORIGIN_VALUE * base.SCALE;

		public byte[] GetBytes() => BitConverter.GetBytes(this.ORIGIN_VALUE);

		/// <summary>
		/// Unscale 된 Base값을 넣는다.
		/// </summary>
		/// <param name="v"></param>
		public override void SetUV(double v) => this.ORIGIN_VALUE = v;

		public override void SetUV(byte[] array, int index) => this.SetUV(BitConverter.ToDouble(array, index));

		protected override double ToUnscaledValue(decimal v) => (double)Math.Truncate(v / this.SCALE);

		protected override double ToUnscaledValue(string v) => this.ToUnscaledValue(this.ToDecimal(v, 0));

		public override void Set(Limit_Number<double> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		protected override decimal LimitCheckMin(decimal value) => value < decimal.MinValue ? decimal.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > decimal.MaxValue ? decimal.MaxValue : value;

		public override void Clear()
        {
            base.MIN = decimal.MinValue;
            base.MAX = decimal.MaxValue;
            base.SCALE = 1;
        }
    }

    public class Limit_Decimal : Limit_Number<decimal>, IGetBytes
    {
        public override string Hex => ((ulong)this.ScaledValue()).ToString("X");

        protected override string ByteString => $"{ByteConverter.ToHexString(this.GetBytes())}";

		public Limit_Decimal() => this.Clear();

		public override decimal ScaledValue() => this.ORIGIN_VALUE * base.SCALE;

        public byte[] GetBytes()
        {
			var temp = decimal.GetBits(this.ORIGIN_VALUE);
            var tempList = new List<byte>();
            foreach (var i in temp)
            {
                tempList.AddRange(BitConverter.GetBytes(i));
            }

            return tempList.ToArray();
        }

        public override void Set(Limit_Number<decimal> v)
        {
            this.SetLimit(v.Min, v.Max);
            this.SetScale(v.Scale);
            this.ORIGIN_VALUE = v.Value;
        }

		public override void SetUV(decimal v) => this.ORIGIN_VALUE = v;

		public override void SetUV(byte[] array, int index)
        {
			var temp = new int[4];

            for (var i = 0; i < temp.Length; i++)
            {
                temp[i] = BitConverter.ToInt32(array, index + (i * 4));
            }

            this.SetUV(new decimal(temp));
        }

		protected override decimal ToUnscaledValue(decimal v) => Math.Truncate(v / this.SCALE);

		protected override decimal ToUnscaledValue(string v) => this.ToUnscaledValue(this.ToDecimal(v, 0));

		protected override decimal LimitCheckMin(decimal value) => value < decimal.MinValue ? decimal.MinValue : value;

		protected override decimal LimitCheckMax(decimal value) => value > decimal.MaxValue ? decimal.MaxValue : value;

		public override void Clear()
        {
            base.MIN = decimal.MinValue;
            base.MAX = decimal.MaxValue;
            base.SCALE = 1;
        }
    }
}

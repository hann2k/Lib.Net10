using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Unit.SI
{
    /// <summary>
    /// Metric Prefix Symbol
    /// </summary>
    public enum MPS
    {
        /// <summary>
        /// Quetta 10^30
        /// </summary>
        Q = 30,
        /// <summary>
        /// Ronna 10^27
        /// </summary>
        R = 27,
        /// <summary>
        /// Yotta 10^24
        /// </summary>
        Y = 24,
        /// <summary>
        /// Zetta 10^21
        /// </summary>
        Z = 21,
        /// <summary>
        /// Exa 10^18
        /// </summary>
        E = 18,
        /// <summary>
        /// Peta 10^15
        /// </summary>
        P = 15,
        /// <summary>
        /// Tera 10^12
        /// </summary>
        T = 12,
        /// <summary>
        /// Giga 10^9
        /// </summary>
        G = 9,
        /// <summary>
        /// Mega 10^6
        /// </summary>
        M = 6,
        /// <summary>
        /// kilo 10^3
        /// </summary>
        k = 3,
        /// <summary>
        /// Hecto 10^2
        /// </summary>
        h = 2,
        /// <summary>
        /// Deca 10^1
        /// </summary>
        da = 1,
        /// <summary>
        /// Zero 10^0
        /// </summary>
        Zero = 0,
        /// <summary>
        /// deci 10^-1
        /// </summary>
        d = -1,
        /// <summary>
        /// centi 10^-2
        /// </summary>
        c = -2,
        /// <summary>
        /// milli 10^-3
        /// </summary>
        m = -3,
        /// <summary>
        /// micro 10^-6
        /// </summary>
        u = -6,
        /// <summary>
        /// nano 10^-9
        /// </summary>
        n = -9,
        /// <summary>
        /// pico 10^-12
        /// </summary>
        p = -12,
        /// <summary>
        /// femto 10^-15
        /// </summary>
        f = -15,
        /// <summary>
        /// atto 10^-18
        /// </summary>
        a = -18,
        /// <summary>
        /// zepto 10^-21
        /// </summary>
        z = -21,
        /// <summary>
        /// yocto 10^-24
        /// </summary>
        y = -24,
        /// <summary>
        /// ronto 10^-27
        /// </summary>
        r = -27,
        /// <summary>
        /// quecto 10^-30
        /// </summary>
        q = -30
    }

    /// <summary>
    /// SI 접두어(Metric Prefix) 정의 클래스<br/>
    /// <br/>
    /// SI 접두어(SI 接頭語)는 SI(국제단위계)의 단위와 결합하여 SI의 배량과 분량을 나타내는 데 사용된다. <br/>
    /// 1960년부터 1991년까지 국제 도량형국(BIPM)에 의해 국제단위계(SI) 사용이 표준화되었다.
    /// </summary>
    public class MetricPrefix
    {
        private const string Quetta = "quetta";
        private const string Ronna = "ronna";
        private const string Yotta = "yotta";
        private const string Zetta = "zetta";
        private const string Exa = "exa";
        private const string Peta = "peta";
        private const string Tera = "tera";
        private const string Giga = "giga";
        private const string Mega = "mega";
        private const string kilo = "kilo";
        private const string Hecto = "hecto";
        private const string Deca = "deca";

        private const string Zero = "";

        private const string deci = "deci";
        private const string centi = "centi";
        private const string milli = "milli";
        private const string micro = "micro";
        private const string nano = "nano";
        private const string pico = "pico";
        private const string femto = "femto";
        private const string atto = "atto";
        private const string zepto = "zepto";
        private const string yocto = "yocto";
        private const string ronto = "ronto";
        private const string quecto = "quecto";

        public string Name = MetricPrefix.Zero;
        public MPS Symbol = MPS.Zero;
        public int Base10 = 0;
        private int Overflow = 0;

        /// <summary>
        /// 기본 접두어
        /// </summary>
        public MetricPrefix()
        {

        }

		public MetricPrefix(int base10) => this.SetBase10(base10);

		public MetricPrefix(MPS m) => this.SetSymbol(m.ToString());

		/// <summary>
		/// Prefix 값을 초과하는 값을 설정한다.<br/>
		/// 29 -> 27, overflow 2<br/>
		/// 30 -> 30, overflow 0<br/>
		/// 값을 가져가면 0으로 설정된다.
		/// </summary>
		public int OverFlow {
            get 
            {
                var over = this.Overflow;
                this.Overflow = 0;
                return over;
            }
        }

        /// <summary>
        /// Base10 으로 심볼과 이름을 설정한다.
        /// </summary>
        /// <param name="_base"></param>
        public void SetBase10(int _base)
        {
            if (Math.Abs(_base) > 30)
			{
				throw new ArgumentOutOfRangeException();
			}

			this.Base10 = _base;

            switch (_base)
            {
                case 2:
                    this.Symbol = MPS.h;
                    break;
                case 1:
                    this.Symbol = MPS.da;
                    break;
                case 0:
                    break;
                case -1:
                    this.Symbol = MPS.d;
                    break;
                case -2:
                    this.Symbol = MPS.c;
                    break;
                default:
                    this.SetBase10_Over3(_base);
                    break;
            }

            this.SetMps2Name();
        }

        /// <summary>
        /// Base10 으로 심볼과 이름을 설정한다.
        /// </summary>
        /// <param name="_base"></param>
        private void SetBase10_Over3(int _base)
        {
            this.Overflow = _base % 3;
            _base -= this.Overflow;

            this.Base10 = _base;

            switch (_base)
            {
                case 30:
                    this.Symbol = MPS.Q;
                    break;

                case 27:
                    this.Symbol = MPS.R;
                    break;

                case 24:
                    this.Symbol = MPS.Y;
                    break;

                case 21:
                    this.Symbol = MPS.Z;
                    break;

                case 18:
                    this.Symbol = MPS.E;
                    break;

                case 15:
                    this.Symbol = MPS.P;
                    break;

                case 12:
                    this.Symbol = MPS.T;
                    break;

                case 9:
                    this.Symbol = MPS.G;
                    break;

                case 6:
                    this.Symbol = MPS.M;
                    break;

                case 3:
                    this.Symbol = MPS.k;
                    break;

                case -30:
                    this.Symbol = MPS.q;
                    break;

                case -27:
                    this.Symbol = MPS.r;
                    break;

                case -24:
                    this.Symbol = MPS.y;
                    break;

                case -21:
                    this.Symbol = MPS.z;
                    break;

                case -18:
                    this.Symbol = MPS.a;
                    break;

                case -15:
                    this.Symbol = MPS.f;
                    break;

                case -12:
                    this.Symbol = MPS.p;
                    break;

                case -9:
                    this.Symbol = MPS.n;
                    break;

                case -6:
                    this.Symbol = MPS.u;
                    break;

                case -3:
                    this.Symbol = MPS.m;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 설정된 심볼에 이름을 넣는다.
        /// </summary>
        private void SetMps2Name()
        {
            switch (this.Symbol)
            {
                case MPS.Q:
                    this.Name = MetricPrefix.Quetta;
                    break;

                case MPS.R:
                    this.Name = MetricPrefix.Ronna;
                    break;

                case MPS.Y:
                    this.Name = MetricPrefix.Yotta;
                    break;

                case MPS.Z:
                    this.Name = MetricPrefix.Zetta;
                    break;

                case MPS.E:
                    this.Name = MetricPrefix.Exa;
                    break;

                case MPS.P:
                    this.Name = MetricPrefix.Peta;
                    break;

                case MPS.T:
                    this.Name = MetricPrefix.Tera;
                    break;

                case MPS.G:
                    this.Name = MetricPrefix.Giga;
                    break;

                case MPS.M:
                    this.Name = MetricPrefix.Mega;
                    break;

                case MPS.k:
                    this.Name = MetricPrefix.kilo;
                    break;

                case MPS.h:
                    this.Name = MetricPrefix.Hecto;
                    break;

                case MPS.da:
                    this.Name = MetricPrefix.Deca;
                    break;

                case MPS.Zero:
                    this.Name = MetricPrefix.Zero;
                    break;

                case MPS.q:
                    this.Name = MetricPrefix.quecto;
                    break;

                case MPS.r:
                    this.Name = MetricPrefix.ronto;
                    break;

                case MPS.y:
                    this.Name = MetricPrefix.yocto;
                    break;

                case MPS.z:
                    this.Name = MetricPrefix.zepto;
                    break;

                case MPS.a:
                    this.Name = MetricPrefix.atto;
                    break;

                case MPS.f:
                    this.Name = MetricPrefix.femto;
                    break;

                case MPS.p:
                    this.Name = MetricPrefix.pico;
                    break;

                case MPS.n:
                    this.Name = MetricPrefix.nano;
                    break;

                case MPS.u:
                    this.Name = MetricPrefix.micro;
                    break;

                case MPS.m:
                    this.Name = MetricPrefix.milli;
                    break;

                case MPS.c:
                    this.Name = MetricPrefix.centi;
                    break;

                case MPS.d:
                    this.Name = MetricPrefix.deci;
                    break;

                default:
                    // 아무것도 하지 않는다.
                    break;
            }
        }

        public void SetSymbol(MPS m)
        {
            this.Symbol = m;
            this.Base10 = (int)this.Symbol;
            this.Overflow = 0;
            this.SetMps2Name();
        }

        /// <summary>
        /// 문자열을 심볼로 변환하고 Base10을 설정한다.
        /// </summary>
        /// <param name="prefix">symbol string</param>
        public void SetSymbol(string prefix)
        {
            if (prefix.Length > 0)
            {
                try
                {
                    this.SetSymbol( (MPS)System.Enum.Parse(typeof(MPS), prefix) );
                    // this.Base10 = (int)this.Symbol;
                    // this.SetMps2Name();
                }
                catch
                {
                    // 이 경우 prefix에 대한 로그를 남겨야 한다.
                    this.Symbol = MPS.Zero;
                    this.Name = MetricPrefix.Zero;
                    this.Base10 = 0;
                }
            }
            else
            {
                this.Symbol = MPS.Zero;
                this.Name = MetricPrefix.Zero;
                this.Base10 = 0;
            }
        }

        public static MetricPrefix operator +(MetricPrefix a) => a;

        public static MetricPrefix operator -(MetricPrefix a)
        {
            a.SetBase10(-a.Base10);
            return a;
        }

        /// <summary>
        /// SIValue 의 곱연산에서만 사용되어야 한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static MetricPrefix operator +(MetricPrefix a, MetricPrefix b)
        {
			var c = new MetricPrefix();

            var baseNum = a.Base10 + b.Base10;
            
            c.Overflow = baseNum % 3;
            c.SetBase10(baseNum - c.OverFlow);

            return c;
        }

        /// <summary>
        /// SIValue의 나누기 연산에서만 사용되어야 한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static MetricPrefix operator -(MetricPrefix a, MetricPrefix b) => a + (-b);

    }
}

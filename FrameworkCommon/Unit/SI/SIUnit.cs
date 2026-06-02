using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Unit.SI
{
    /// <summary>
    /// SI 표준 단위를 정의한 클래스
    /// </summary>
    public class SIUnit : SIUnitBase, IEquatable<SIUnit>
    {
        /// <summary>
        /// 빈 단위를 표시한다.
        /// </summary>
        public static readonly SIUnit Empty;

		/// <summary>
		/// 기호
		/// </summary>
		public string Symbol => this.BaseUnit.Symbol;

		public SIUnitDefine BaseUnit { get; private set; }

		/// <summary>
		/// 이름
		/// </summary>
		public string Name => this.BaseUnit.Name;
		/// <summary>
		/// 물리량
		/// </summary>
		public string Qualtity => this.BaseUnit.Qualtity;

		public SIUnit(SIUnitDefine unit) => this.BaseUnit = unit;

		public SIUnit(string symbol) => throw new NotImplementedException();

		/// <summary>
		/// 입력된 문자열에서 단위를 추출하고 남은 문자열을 뱉는다.
		/// 
		/// .NET 4.7 이하의 경우, Nuget -> ValueTuple 설치 후 사용한다.
		/// </summary>
		/// <param name="candidate"></param>
		/// <returns></returns>
		public static (string Symbol, bool Result) ExtractOthers(string candidate, string unit)
        {
            var symbol = string.Empty;
            var result = false;

            return (symbol, result);
        }

        /// <summary>
        /// 입력된 문자열에서 단위를 추출한다.
        /// 
        /// .NET 4.7 이하의 경우, Nuget -> ValueTuple 설치 후 사용한다.
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static (string Unit, bool Result) ExtractUnit(string candidate)
        {
            var unit = string.Empty;
            var result = false;

			SIUnitDefine funit = SIUnitBase.FindUnit(candidate);
            if ( funit != SIUnitDefine.Empty )
            {
                unit = funit.Symbol;
                result = true;
            }

            return (unit, result);
        }

        public static SIUnit operator *(SIUnit a, SIUnit b)
        {

			var c = new SIUnit(a.Symbol)
			{
				BaseUnit = a.BaseUnit
			};
			c.BaseUnit.SetSymbol(a.Symbol + "⋅" + b.Symbol);

            return c;
        }

        public static bool operator ==(SIUnit a, SIUnit b)
        {
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
		}

        public static bool operator !=(SIUnit a, SIUnit b) => !(a == b);

        public override bool Equals(object obj) => this.Equals(obj as SIUnit);

        /// <summary>
        /// 단위를 비교한다.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Equals(SIUnit u)
        {
            bool result;

            if (Object.ReferenceEquals(this, u))
            {
                result = true;
            }
            else
            { 
                result = !(u is null) &&
                    this.GetType() == u.GetType() &&
                    this.Name == u.Name &&
                    this.Symbol == u.Symbol &&
                    this.Qualtity == u.Qualtity;
            }

			// If run-time types are not exactly the same, return false.
			// Return true if the fields match.
			// Note that the base class is not invoked because it is
			// System.Object, which defines Equals as reference equality.

			return result;
		}

        public override int GetHashCode() => (this.Name, this.Symbol, this.Qualtity).GetHashCode();

        /// <summary>
        /// 변환식 a * b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIUnit ConvertTimes(SIUnit a, SIUnit b)
        {
            // 면적 = 길이 * 길이
            if (a.BaseUnit == SIUnitBase.Meter && b.BaseUnit == SIUnitBase.Meter)
            {
                return new SIUnit(SIUnitBase.Area);
            }
            // 부피 = 면적 * 길이
            else if ((a.BaseUnit == SIUnitBase.Area && b.BaseUnit == SIUnitBase.Meter) || (b.BaseUnit == SIUnitBase.Area && a.BaseUnit == SIUnitBase.Meter))
            {
                return new SIUnit(SIUnitBase.Volume);
            }
            else
            {
                throw new SIUnitCannotConvertException();
            }
        }

        /// <summary>
        /// 변환식 a / b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SIUnit ConvertDevide(SIUnit a, SIUnit b)
        {
            // 속력(속도) = 길이 / 시간(초)
            if (a.BaseUnit == SIUnitBase.Meter && b.BaseUnit == SIUnitBase.Second)
            {
                return new SIUnit(SIUnitBase.Speed);
            }
            // 가속도 = 속력(속도) / 시간(초)
            else if (a.BaseUnit == SIUnitBase.Speed && b.BaseUnit == SIUnitBase.Second)
            {
                return new SIUnit(SIUnitBase.Acceleration);
            }
            // 밀도 = 질량 / 부피
            else if (a.BaseUnit == SIUnitBase.Gram && b.BaseUnit == SIUnitBase.Volume)
            {
                return new SIUnit(SIUnitBase.Density);
            }
            // 농도 = 몰 / 부피
            else if (a.BaseUnit == SIUnitBase.Mole && b.BaseUnit == SIUnitBase.Volume)
            {
                return new SIUnit(SIUnitBase.Concentration);
            }
            // 광휘도 = 칸델라 / 면적
            else if (a.BaseUnit == SIUnitBase.Candela && b.BaseUnit == SIUnitBase.Area)
            {
                return new SIUnit(SIUnitBase.Luminance);
            }
            // 같은 단위일때
            //else if ( a.CurrentUnit == b.CurrentUnit)
            //{
            //    return new SIUnit(a.CurrentUnit);
            //}
            // 연산할 수 없는 단위일때
            else
            {
                throw new SIUnitCannotConvertException();
            }
        }

        public void SecondToHertz()
        {
            this.BaseUnit = SIUnitBase.Hertz;
        }

        public void HertzToSecond()
        {
            this.BaseUnit = SIUnitBase.Second;
        }

    }

    public abstract class SIUnitBase
    {
        // 이 클래스에서 다루는 단위들 집합.
        // SI Base Units
        /// <summary>
        /// 전류 : 암페어
        /// </summary>
        public static readonly SIUnitDefine Ampere = new SIUnitDefine("A", "ampere", "electric current", "I", new[] { "I", "i" }, "The flow of exactly ( 1 / (1.602176634×10^−19) ) times the elementary charge e per second. Equalling approximately 6.2415090744×10^18 elementary charges per second.");
        /// <summary>
        /// 광도 : 칸델라
        /// </summary>
        public static readonly SIUnitDefine Candela = new SIUnitDefine("cd", "candela", "luminous intensity", "j", new[] { "I_v" }, @"The luminous intensity, in a given direction, of a source that emits monochromatic radiation of frequency 5.4×10^14 hertz and that has a radiant intensity in that direction of 1 / 683  watt per steradian.");
        /// <summary>
        /// (절대)온도 : 켈빈
        /// </summary>
        public static readonly SIUnitDefine Kelvin = new SIUnitDefine("K", "kelvin", "themodynamic temperature", "Θ", new[] { "T" }, "The kelvin is defined by setting the fixed numerical value of the Boltzmann constant k to 1.380649×10^−23 J⋅K^−1, (J = kg⋅m^2⋅s^−2), given the definition of the kilogram, the metre, and the second.");
        /// <summary>
        /// 질량 : 그램
        /// </summary>
        public static readonly SIUnitDefine Gram = new SIUnitDefine("g", "gram", "mass", "M", new[] { "m" }, "The kilogram is defined by setting the Planck constant h exactly to 6.62607015×10^−34 J⋅s (J = kg⋅m^2⋅s^−2), given the definitions of the metre and the second.");
        /// <summary>
        /// 길이 : 미터
        /// </summary>
        public static readonly SIUnitDefine Meter = new SIUnitDefine("m", "metre", "length", "L", new[] { "l", "h", "a", "b", "x", "y", "r", "etc" }, "The distance travelled by light in vacuum in 1 / 299792458 seconds.");
        /// <summary>
        /// 물질량 : 몰
        /// </summary>
        public static readonly SIUnitDefine Mole = new SIUnitDefine("mol", "mole", "amount of substance", "N", new[] { "n" }, "The amount of substance of exactly 6.02214076×10^23 elementary entities. This number is the fixed numerical value of the Avogadro constant, N_A, when expressed in the unit mol^−1.");
        /// <summary>
        /// 시간 : 초
        /// </summary>
        public static readonly SIUnitDefine Second = new SIUnitDefine("s", "second", "time", "T", new[] { "t" }, "The duration of 9,192,631,770 periods of the radiation corresponding to the transition between the two hyperfine levels of the ground state of the caesium-133 atom.");

        // SI Derived Units
        /// <summary>
        /// 라디안 : 평면각
        /// </summary>
        public static readonly SIUnitDefine Radian = new SIUnitDefine("rad", "radian", "plane angle");
        /// <summary>
        /// 스테라디안 : 입체각
        /// </summary>
        public static readonly SIUnitDefine SteRadian = new SIUnitDefine("sr", "steradian", "solid angle");

        /// <summary>
        /// 주파수
        /// </summary>
        public static readonly SIUnitDefine Hertz = new SIUnitDefine("Hz", "Hertz", "frequency");
        /// <summary>
        /// 힘, 뉴턴
        /// </summary>
        public static readonly SIUnitDefine Newton = new SIUnitDefine("N", "Newton", "plane angle");
        /// <summary>
        /// 압력, 변형력 파스칼
        /// </summary>
        public static readonly SIUnitDefine Pascal = new SIUnitDefine("Pa", "Pascal", "plane angle");
        /// <summary>
        /// 에너지, 일, 열량 : 줄
        /// </summary>
        public static readonly SIUnitDefine Joule = new SIUnitDefine("J", "Joule", "plane angle");
        /// <summary>
        /// 일률, 전력, 동력 : 와트
        /// </summary>
        public static readonly SIUnitDefine Watt = new SIUnitDefine("W", "Watt", "plane angle");
        /// <summary>
        /// 전하량, 전기량 : 쿨롱
        /// </summary>
        public static readonly SIUnitDefine Coulomb = new SIUnitDefine("C", "Coulomb", "plane angle");
        /// <summary>
        /// 전위차, 기전력, 전압 : 볼트
        /// </summary>
        public static readonly SIUnitDefine Volt = new SIUnitDefine("V", "Volt", "plane angle");
        /// <summary>
        /// 전기 용량 : 패럿
        /// </summary>
        public static readonly SIUnitDefine Farad = new SIUnitDefine("F", "Farad", "plane angle");
        /// <summary>
        /// 전기 저항 : 옴
        /// </summary>
        public static readonly SIUnitDefine Ohm = new SIUnitDefine("Ω", "Ohm", "plane angle");
        /// <summary>
        /// 전도율 : 지멘스
        /// </summary>
        public static readonly SIUnitDefine Siemens = new SIUnitDefine("S", "Siemens", "plane angle");
        /// <summary>
        /// 자기 선속 : 웨버
        /// </summary>
        public static readonly SIUnitDefine Weber = new SIUnitDefine("Wb", "Weber", "plane angle");
        /// <summary>
        /// 자기선속밀도 : 테슬라
        /// </summary>
        public static readonly SIUnitDefine Tesla = new SIUnitDefine("T", "Tesla", "plane angle");
        /// <summary>
        /// 인덕턴스 : 헨리
        /// </summary>
        public static readonly SIUnitDefine Henry = new SIUnitDefine("H", "Henry", "plane angle");
        /// <summary>
        /// 섭씨 온도 : 셀시우스
        /// </summary>
        public static readonly SIUnitDefine DegreeCelsius = new SIUnitDefine("℃", "DegreeCelsius", "plane angle");
        /// <summary>
        /// 광선속 : 루멘
        /// </summary>
        public static readonly SIUnitDefine Lumen = new SIUnitDefine("lm", "Lumen", "plane angle");
        /// <summary>
        /// 조도 : 럭스
        /// </summary>
        public static readonly SIUnitDefine Lux = new SIUnitDefine("lx", "Lux", "plane angle");
        /// <summary>
        /// 방사능 : 베크렐
        /// </summary>
        public static readonly SIUnitDefine Becquerel = new SIUnitDefine("Bq", "Becquerel", "plane angle");
        /// <summary>
        /// 흡수선량 : 그레이
        /// </summary>
        public static readonly SIUnitDefine Gray = new SIUnitDefine("Gy", "Gray", "plane angle");
        /// <summary>
        /// 선량당량 : 시버트
        /// </summary>
        public static readonly SIUnitDefine Sievert = new SIUnitDefine("Sv", "Sievert", "plane angle");
        /// <summary>
        /// 촉매 활성도 : 카탈
        /// </summary>
        public static readonly SIUnitDefine Katal = new SIUnitDefine("kat", "Katal", "plane angle");

        // SI coherent derivedUunits
        /// <summary>
        /// 넓이
        /// </summary>
        public static readonly SIUnitDefine Area = new SIUnitDefine("㎡", "square metre", "area");
        /// <summary>
        /// 부피
        /// </summary>
        public static readonly SIUnitDefine Volume = new SIUnitDefine("㎥", "cubic metre", "volume");
        /// <summary>
        /// 속력
        /// </summary>
        public static readonly SIUnitDefine Speed = new SIUnitDefine("m/s", "metre per second", "speed, velocity");
        /// <summary>
        /// 속도
        /// </summary>
        public static readonly SIUnitDefine Velocity = Speed;
        /// <summary>
        /// 가속도
        /// </summary>
        public static readonly SIUnitDefine Acceleration = new SIUnitDefine("㎨", "metre per second squared", "acceleration");
        // public static readonly SIUnitDefine wavenumber = new SIUnitDefine("㎡", "square metre", "wavenumber");
        /// <summary>
        /// 밀도
        /// </summary>
        public static readonly SIUnitDefine Density = new SIUnitDefine("g/㎥", "gram per cubic metre", "density");
        /// <summary>
        /// 표면밀도
        /// </summary>
        public static readonly SIUnitDefine SurfaceDensity = new SIUnitDefine("g/㎡", "gram per square metre", "Surface Density");
        /// <summary>
        /// 농도
        /// </summary>
        public static readonly SIUnitDefine Concentration = new SIUnitDefine("mol/㎥", "square metre", "concentration");
        /// <summary>
        /// 광휘도
        /// </summary>
        public static readonly SIUnitDefine Luminance = new SIUnitDefine("cd/㎡", "square metre", "concentration");

        // 국제단위계와 함께 쓰이는 단위

        /// <summary>
        /// 시간 : 분
        /// </summary>
        public static readonly SIUnitDefine Minute = new SIUnitDefine("min", "Minute", "1 min = 60 s");
        /// <summary>
        /// 시간 : 시간(Hour)
        /// </summary>
        public static readonly SIUnitDefine Hour = new SIUnitDefine("h", "Hour", "1 h = 60 min = 3600 s");
        /// <summary>
        /// 시간 : 일
        /// </summary>
        public static readonly SIUnitDefine Day = new SIUnitDefine("d", "Day", "1 d = 24 h = 1440 min = 86400 s");

        /// <summary>
        /// 각도 : 도
        /// </summary>
        public static readonly SIUnitDefine Degree = new SIUnitDefine("°", "Degree", "1° = (π/180) rad");
        /// <summary>
        /// 각도 : 분
        /// </summary>
        public static readonly SIUnitDefine ArcMinute = new SIUnitDefine("′", "ArcMinute", "1′ = (1/60)° = (π/10800) rad");
        /// <summary>
        /// 각도 : 초
        /// </summary>
        public static readonly SIUnitDefine ArcSecond = new SIUnitDefine("″", "ArcSecond", "1″ = (1/60)′ = (1/3600)° = (π /648000) rad");
        /// <summary>
        /// 부피 : 리터
        /// </summary>
        public static readonly SIUnitDefine Liter = new SIUnitDefine("L", "Liter", "0.001 ㎥");
        /// <summary>
        /// 질량 : 톤
        /// </summary>
        public static readonly SIUnitDefine Tonne = new SIUnitDefine("t", "Tonne", "1 t = 10³kg");

        /// <summary>
        /// 길이 : 해리(NauticalMile)
        /// </summary>
        public static readonly SIUnitDefine NauticalMile = new SIUnitDefine("NM", "NauticalMile", "1 NM(해리) = 1852 m");
        /// <summary>
        /// 속력 : 노트
        /// </summary>
        public static readonly SIUnitDefine Knot = new SIUnitDefine("kn", "Knot", "1 kn = 시간당 1 NM(해리) = (1852/3600) m/s");
        /// <summary>
        /// 넓이 : 아르
        /// </summary>
        public static readonly SIUnitDefine Are = new SIUnitDefine("a", "Are", "1a = 1da㎡ = 100 ㎡");
        /// <summary>
        /// 넓이 : 헥타르
        /// </summary>
        public static readonly SIUnitDefine HectAre = new SIUnitDefine("ha", "HectAre", "1ha = 100a = 10000 ㎡");
        /// <summary>
        /// 압력 : 바
        /// </summary>
        public static readonly SIUnitDefine Bar = new SIUnitDefine("t", "bar", "1 bar = 10⁵ Pa");
        /// <summary>
        /// 길이 : 옹스트롬
        /// </summary>
        public static readonly SIUnitDefine Angstrom = new SIUnitDefine("Å", "Angstrom", "1 Å = 0.1 nm = 10^-10 m");
        /// <summary>
        /// 면적 : 바안
        /// </summary>
        public static readonly SIUnitDefine Barn = new SIUnitDefine("b", "Barn", "1b = 10^−28 ㎡");

        public static readonly SIUnitDefine ElectroVolt = new SIUnitDefine("eV", "ElectroVolt", "1eV = 1.60217733 (49) × 10^−19 J");
        public static readonly SIUnitDefine AtomicMassUnit = new SIUnitDefine("u", "Atomic Mass Unit", "1u = 1.6605402 (10) × 10^−27 kg");
        public static readonly SIUnitDefine AstronomicalUnit = new SIUnitDefine("au", "Astronomical Unit", "1au = 1.49597870691 (30) × 10^11 m");

        private static readonly List<SIUnitDefine> UnitList = new List<SIUnitDefine>() {
            // Base Unit List
            Ampere, Candela, Kelvin, Gram, Meter, Mole, Second,
            Radian, SteRadian, Hertz, Newton, Pascal, Joule, Watt, Coulomb, Volt, Farad, Ohm, Siemens, Weber, Tesla, Henry, DegreeCelsius, Lumen, Lux, Becquerel, Gray, Sievert, Katal,
            Area, Volume, Speed, Velocity, Acceleration, Density, SurfaceDensity, Concentration, Luminance,
            Minute, Hour, Day, Degree, ArcMinute, ArcSecond, Liter, Tonne,
            NauticalMile, Knot, Are, HectAre, Bar, Angstrom, Barn,
            ElectroVolt, AtomicMassUnit, AstronomicalUnit
        };

        public static SIUnitDefine FindUnit(string symbol)
        {
            int idx = SIUnitBase.UnitList.FindIndex(n => n.Symbol == symbol);

            return idx < 0 ? SIUnitDefine.Empty : UnitList[idx];
        }
    }
}

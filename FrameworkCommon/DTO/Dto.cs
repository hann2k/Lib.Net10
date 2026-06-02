using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Framework.Common.Converter;
using Framework.Common.Enum;

namespace Framework.Common.DTO
{
    /// <summary>
    /// 데이터 교환용 클래스의 원형<br/>
    /// 확장시 클래스명은 Dto로 끝내야 한다.
    /// </summary>
    public abstract class Dto
    {
        public event EventHandler DtoUpdated;

        /// <summary>
        /// 빈 Dto 표시
        /// </summary>
        public static readonly Dto Empty;

        /// <summary>
        /// DTO 내부에서 Bit관리를 할 수 있도록 지원하는 클래스
        /// </summary>
        protected static readonly BitManager Bm = new BitManager();

        private DtoType MyDtoType;

        public Dto()
        {

        }

		public Dto(DtoType type) => this.SetDtoType(type);

		/// <summary>
		/// DTO 타입을 지정한다.
		/// </summary>
		/// <param name="type"></param>
		protected void SetDtoType(DtoType type) => this.MyDtoType = type;

		// public abstract void Set(Dto dto);

		public override string ToString() => throw new NotImplementedException("DTO를 출력하려면 base 참조 없이 ToString()을 새로 정의하세요.");

		public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

		protected void OnDtoUpdated()
        {
            // Console.WriteLine("DtoUpdate");
            this.DtoUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface ILimitData
    {
        Limit_Datas Dto { get; set; }
    }

    public interface IDto
    {
        Dto Dto { get; set; }
    }

    public interface IGetDTO
    {
        Dto GetDto();
    }

    public interface ISetDTO
    {
        void SetDto(Dto DTO);
    }
}

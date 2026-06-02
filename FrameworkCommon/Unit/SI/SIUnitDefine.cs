using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Unit.SI
{
    public class SIUnitDefine
    {
        /// <summary>
        /// 빈 단위를 표시한다.
        /// </summary>
        public static readonly SIUnitDefine Empty;

        /// <summary>
        /// 기호
        /// </summary>
        public readonly string Symbol;
        /// <summary>
        /// 이름
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 물리량
        /// </summary>
        public readonly string Qualtity;

        public readonly string DimensionSymbol;
        public readonly string[] TypicalSymbols;
        public readonly string Definition;

        /// <summary>
        /// 기본 생성자
        /// </summary>
        /// <param name="s">Symbol</param>
        /// <param name="n">Name</param>
        /// <param name="q">Quantity Name</param>
        public SIUnitDefine(string s, string n, string q)
        {
            this.Symbol = s;
            this.Name = n;
            this.Qualtity = q;
        }

        /// <summary>
        /// 상세 내용 생성자
        /// </summary>
        /// <param name="s">Symbol</param>
        /// <param name="n">Name</param>
        /// <param name="q">Quantity Name</param>
        /// <param name="ds">Dimension Symbol</param>
        /// <param name="ts">Typical Symbols</param>
        /// <param name="d">Definition</param>
        public SIUnitDefine(string s, string n, string q, string ds, string[] ts, string d) : this(s, n, q)
        {
            this.DimensionSymbol = ds;
            this.TypicalSymbols = ts;
            this.Definition = d;
        }

        public void SetSymbol(string sym)
        {
            // this.Symbol = sym;
        }
    }
}

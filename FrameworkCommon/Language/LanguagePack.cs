using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common
{
    public enum Language
    {
        Korean,
        Engligh
    }

    internal class LangKey
    {
        public Language Lang = Language.Korean;
        public string Name = string.Empty;
    }

    public class LanguagePack
    {
        protected Language CurrentLanguage { get; set; } = Language.Korean;

        /// <summary>
        /// 현재 사용하는 언어를 설정한다.
        /// </summary>
        /// <param name="lan"></param>
        private void SetLanguage(Language lan)
        {
            this.CurrentLanguage = lan;
        }

        public void SetLanguage(string lan)
        {
            Language l = (Language)System.Enum.Parse(typeof(Language), lan);
            this.SetLanguage( l );
        }
    }

    public class LanguageStorage : LanguagePack
    {
        readonly Dictionary<LangKey, string> Pack = new Dictionary<LangKey, string>();

        public void SetLangFile(string lan, string filename)
        {
            throw new NotImplementedException();
        }
    }
}

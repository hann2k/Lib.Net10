using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.DTO
{
    public interface IJson
    {
        /// <summary>
        /// Json 문자열을 Dto타입으로 파싱한다.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        void SetJson(string json);
    }
}

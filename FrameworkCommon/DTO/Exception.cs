using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.DTO
{
    /// <summary>
    /// DTO 관련하여 발생하는 모든 예외의 기본
    /// </summary>
    [Serializable]
    public class DTOException : System.Exception
    {
        public DTOException() : base() { }
        public DTOException(string message) : base(message) { }
        public DTOException(string message, Exception inner) : base(message, inner) { }
    }
}

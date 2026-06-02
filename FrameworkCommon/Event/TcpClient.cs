using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Event
{
    /// <summary>
    /// Ping 실행시 발생하는 이벤트
    /// </summary>
    public class EventPingArgs : EventArgs
    {
        /// <summary>
        /// 네트워크 Ping 상태
        /// </summary>
        public bool PingState = false;
        /// <summary>
        /// 네트워크 Ping 상태 메시지
        /// </summary>
        public string PingMessage { get; set; } = "Network Disconnect";
    }
}

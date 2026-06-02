using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Common.Converter;

namespace Framework.Common.Comm
{
    /// <summary>
    /// 양방향 통신 패킷의 기본형
    /// </summary>
    public abstract class PacketTwoWay : Packet
    {
        private readonly List<byte> RcvBuffer = new List<byte>();

		/// <summary>
		/// 수신 패킷 전체의 크기
		/// </summary>
		public int RcvLength => this.RcvBuffer.Count;

		/// <summary>
		/// 수신 패킷 중 헤더의 크기
		/// </summary>
		public abstract int RcvHeaderLength { get; protected set; }

        /// <summary>
        /// 수신 패킷 중 Body의 크기
        /// </summary>
        public abstract int RcvBodyLength { get; protected set; }

		public void ClearRcv() => this.RcvBuffer.Clear();

		public void SetRcvData(byte[] rcvData) => this.RcvBuffer.AddRange(rcvData);

		public void SetRcvData(List<byte> rcvData) => this.RcvBuffer.AddRange(rcvData);

		public void SetRcvData( string rcvData )
        {
            var bs = ByteConverter.ToBytes(rcvData);
            this.RcvBuffer.AddRange(bs);
        }

        /// <summary>
        /// 송신 패킷과 수신 패킷을 비교한다.
        /// </summary>
        /// <returns>
        /// True : 송수신 패킷이 맞음.<br/>
        /// False : 서로 짝이 맞지 않는 패킷임.
        /// </returns>
        public abstract bool MatchData();
    }
}

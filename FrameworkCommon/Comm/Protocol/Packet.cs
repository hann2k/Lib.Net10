using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Framework.Common.Converter;
using Framework.Common.Buffer;
using Framework.Common.Logger;

namespace Framework.Common.Comm
{
    /// <summary>
    /// 데이터 통신용 패킷의 기본형
    /// </summary>
    public abstract class Packet
    {
        /// <summary>
        /// 패킷 헤더 저장소
        /// </summary>
        protected readonly PacketBuffer Header = new PacketBuffer();
        /// <summary>
        /// 패킷 본문 저장소
        /// </summary>
        protected readonly PacketBuffer Body = new PacketBuffer();

		/// <summary>
		/// 패킷의 전체 크기
		/// </summary>
		public virtual int Size => this.Header.Length + this.Body.Length;

		/// <summary>
		/// 패킷 파싱 상태. 수신한 패킷에 오류가 발생하면 표시한다.
		/// </summary>
		public PacketError Error { get; protected set; } = new PacketError();

		/// <summary>
		/// 패킷 내부에서 Bit관리를 할 수 있도록 지원하는 클래스
		/// </summary>
		protected readonly BitManager Bm = new BitManager();

		/// <summary>
		/// 저장된 데이터를 Header, Body Byte Array 로 변경한다.
		/// </summary>
		protected abstract void MakePacket();

        /// <summary>
        /// 수신한 데이터를 Header, Body로 구분하고 Body의 데이터를 파싱한다.
        /// </summary>
        /// <param name="rcv"></param>
        protected abstract void Parse(byte[] rcv);

        /// <summary>
        /// 수신한 데이터를 파싱한다.
        /// </summary>
        /// <param name="array"></param>
        public abstract void SetReceivedBytes(byte[] array);

        /// <summary>
        /// 패킷에 저장된 데이터를 바이트 어레이로 변경한다.
        /// 이름은 나중에 변경될 수 있다.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            try
            {
                this.MakePacket();

                var t = new List<byte>();
                t.AddRange(this.Header.GetBytes());
                t.AddRange(this.Body.GetBytes());

                return t.ToArray();
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
            }

            return Array.Empty<byte>();

        }

        /// <summary>
        /// 명세서의 인덱스를 데이터의 절대인덱스로 변경한다.
        /// </summary>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        protected abstract int ToAP(int dataIndex);

        /// <summary>
        /// 패킷 내용을 출력한다.
        /// </summary>
        /// <returns></returns>
        public string[] Print()
        {
            try
            {
                return this.PrintParts();
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
            }

            return Array.Empty<string>();
        }

        protected abstract string[] PrintParts();
    }

    /// <summary>
    /// 패킷 에러 관리 클래스
    /// </summary>
    public class PacketError : Error.Error
    {
        /// <summary>
        /// 패킷 에러 상태
        /// </summary>
        public bool Error { get; private set; } = false;

        /// <summary>
        /// 패킷 에러 원인
        /// </summary>
        public PacketErrorReason EReason { get; private set; } = PacketErrorReason.Clean;

        /// <summary>
        /// 패킷 에러 원인 설정용 메서드
        /// </summary>
        /// <param name="reason"></param>
        public void SetReason(PacketErrorReason reason)
        {
            this.EReason = reason;

            if (this.EReason != PacketErrorReason.Clean)
            {
                this.Error = true;
            }
        }
    }

    /// <summary>
    /// 패킷 오류가 발생된 원인
    /// </summary>
    public enum PacketErrorReason
    {
        /// <summary>
        /// 없음
        /// </summary>
        Clean,
        /// <summary>
        /// 패킷 크기 불일치
        /// </summary>
        MismatchLength,
        /// <summary>
        /// 패킷 헤더 오류
        /// </summary>
        HeaderError,
        /// <summary>
        /// 패킷 데이터 오류
        /// </summary>
        BodyError,
        /// <summary>
        /// 패킷 체크섬 오류
        /// </summary>
        ChecksumError,
        /// <summary>
        /// 알 수 없는 원인의 오류
        /// </summary>
        UnknownError,
    }
}

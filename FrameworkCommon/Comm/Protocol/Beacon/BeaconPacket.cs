using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Framework.Common.Enum;
using Framework.Common.Converter;
using Framework.Common.Logger;
// using Framework.Common.Comm;

namespace Framework.Common.Comm
{
    /// <summary>
    /// Beacon 통신 패킷의 원형
    /// </summary>
    public abstract class Framework_Beacon_Packet : Packet
    {
        /// <summary>
        /// 커맨드
        /// </summary>
        public byte Command { get; protected set; }

        public Framework_Beacon_Packet()
        {
        }

        /// <summary>
        /// Data Index Change to Absolute Position in received packet byte arrays
        /// </summary>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        protected override int ToAP(int dataIndex)
        {
            if ( dataIndex < 1)
            {
                throw new ArgumentOutOfRangeException("ICD의 데이터 인덱스는 1 미만이 될 수 없습니다.");
            }

            return dataIndex - 1;
        }
    }

    public abstract class Beacon_Packet : Framework_Beacon_Packet
    {
        /// <summary>
        /// 매직코드
        /// </summary>
        protected readonly static byte[] MagicCode = { 0x4c, 0x55, 0x65, 0xAA };

        public static bool CheckMagicCode(byte[] source)
        {
            var MagicCodeLength = MagicCode.Length;

            if (source.Length < MagicCodeLength)
            {
                return false;
            }

            var s = new byte[MagicCodeLength];
            Array.Copy(source, s, MagicCodeLength);

            return Enumerable.SequenceEqual(s, Beacon_Packet.MagicCode);
        }

		public static bool CheckMagicCode(List<byte> source) => CheckMagicCode(source.ToArray());

		#region Send 관련기능

		protected virtual void MakeHeader()
		{
			try
			{
				this.Header.Clear();

				this.Header.AddRange(Beacon_Packet.MagicCode);
				this.Header.Add(this.SetCommand());

				var sizeArray = BitConverter.GetBytes((ushort)(this.Body.Length + 7));

				this.Header.AddRange(sizeArray);

				Log.Ins.Debug(this.Print());
			}
			catch (Exception ex)
			{
				Log.Ins.Exception(ex);
			}
		}

		protected override void MakePacket() => this.MakeHeader();

		/// <summary>
		/// Beacon 패킷에서 사용할 명령어 코드를 설정한다.
		/// </summary>
		/// <returns></returns>
		protected abstract byte SetCommand();

        #endregion

        #region Receive 관련기능

        /// <summary>
        /// 수신 패킷의 무결성 여부 확인
        /// </summary>
        /// <param name="array">수신 패킷 전체</param>
        /// <returns>정상 : true, 이상 : false</returns>
        protected bool ValidateHeader(byte[] header, int bodyLength)
        {
            try
            {
                // 매직코드 불일치
                if (header[0] != 0x4C
                    || header[1] != 0x55
                    || header[2] != 0x65
                    || header[3] != 0xAA)
                {
                    base.Error.SetReason(PacketErrorReason.HeaderError);
                    base.Error.SetReason("매직코드가 맞지 않습니다.");
                    return false;
                }

                // 커맨드 불일치
                if (!this.CheckCommand(header[4]))
                {
                    base.Error.SetReason(PacketErrorReason.HeaderError);
                    base.Error.SetReason("커맨드 값이 맞지 않습니다.");
                    return false;
                }

                // 메시지 사이즈 불일치
                var size = header[5] + (header[6] * 256) - header.Length;
                if (bodyLength != size)
                {
                    base.Error.SetReason(PacketErrorReason.BodyError);
                    base.Error.SetReason("데이터 사이즈가 실제 길이와 값이 맞지 않습니다.");
                    // 보안 점검 #3: 크기 불일치 검증 복원. 헤더가 선언한 길이와 실제 바디 길이가
                    // 다르면 무효 처리하여 하위 Parse가 헤더 길이를 신뢰해 OOB 읽기를 하지 못하게 한다.
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
                base.Error.SetReason(PacketErrorReason.UnknownError);
                base.Error.SetReason("알 수 없는 원인의 오류가 발생했습니다. 로그를 참조하세요.");
            }

            return false;
        }

        /// <summary>
        /// 수신 패킷의 커맨드가 정상인지 여부 확인
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool CheckCommand(byte b)
        {
            if (this.Command == 0x00)
            {
                throw new ArgumentOutOfRangeException("Dto의 기준 Command가 설정되지 않았습니다.");
            }

            return (b == this.Command);
        }

        /// <summary>
        /// 수신한 데이터를 파싱한다.
        /// </summary>
        /// <param name="rcv"></param>
        public override void SetReceivedBytes(byte[] array)
        {
            try
            {
                // 보안 점검 #3: 최소 길이(헤더 7바이트) 가드.
                // 미만이면 GetRange가 OOB 예외를 던지므로, 사전에 명시적으로 거부한다.
                const int HeaderLength = 7;
                if (array == null || array.Length < HeaderLength)
                {
                    base.Error.SetReason(PacketErrorReason.HeaderError);
                    base.Error.SetReason($"패킷 길이가 최소 헤더 길이({HeaderLength}바이트)보다 짧습니다.");
                    Log.Ins.Error(this.Error.Reason);
                    return;
                }

                // 헤더를 분리한다
                this.Header.AddRange(array.ToList().GetRange(0, HeaderLength));

                // 데이터 영역을 분리한다.
                this.Body.AddRange(array.ToList().GetRange(HeaderLength, array.Length - HeaderLength));

                // 명령어를 추출한다.
                this.Command = this.Header[4]; //.Get(4);

                // 헤더부분의 유효성을 검사한다. (보안 점검 #3: 반환값을 실제 게이트로 사용)
                if (this.ValidateHeader(this.Header.GetBytes(), this.Body.Length))
                {
                    // 수신한 데이터를 파싱한다.
                    this.Parse(this.Body.GetBytes());
                }
                else
                {
                    // 에러를 기록한다.
                    Logger.Log.Ins.Error(this.Error.Reason);
                }
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
            }
        }



        #endregion
    }

}

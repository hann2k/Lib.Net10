using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Motion
{
    /// <summary>
    /// 어떤 물체가 가질 수 있는 운동의 성분
    /// </summary>
    public class Motion
    {
        /// <summary>
        /// 이동거리(m) = 운동한 거리 (방향 없이 총 운동한 거리)
        /// </summary>
        public decimal Distince { get; private set; }

        /// <summary>
        /// 변위(m) = 위치의 변한 정도 (이전 측정 위치와 다음 측정 위치의 거리)
        /// </summary>
        public decimal Displacement { get; private set; }

        /// <summary>
        /// 시간(Sec)
        /// </summary>
        public decimal Time { get; private set; }

        /// <summary>
        /// 속도(m/s) = 변위 / 시간의 변화량
        /// </summary>
        public decimal Velocity { get; private set; }

        /// <summary>
        /// 속력(m/s) = 이동거리 / 시간의 변화량 
        /// </summary>
        public decimal Speed { get; private set; }

        /// <summary>
        /// 이동거리, 시간으로 속력 환산하기
        /// </summary>
        /// <param name="distanse"></param>
        /// <param name="timesec"></param>
        /// <returns></returns>
        public decimal SetSpeed( int distanse, int timesec )
        {
            this.Distince = distanse;
            this.Time = timesec;

            this.Speed = this.Distince / this.Time;

            return this.Speed;
        }

        /// <summary>
        /// 변위, 시간으로 속도 환산하기
        /// </summary>
        /// <param name="displacement"></param>
        /// <param name="timesec"></param>
        /// <returns></returns>
        public decimal SetVelocity(int displacement, int timesec)
        {
            this.Displacement = displacement;
            this.Time = timesec;

            this.Velocity = this.Displacement / this.Time;

            return this.Velocity;
        }
    }
}

using Framework.Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.CoordinateSystem
{
    /// <summary>
    /// 미사일 이동 구간 아이템.
    /// 미사일은 해당 이동구간을 등가속/등속 운동한다.
    /// </summary>
    public class MissileMovingSection : Coordinates
    {
        #region 구간 초기화 값

        /// <summary>
        /// 방위각
        /// </summary>
        public decimal Azimuth { get; private set; } = 0;
        /// <summary>
        /// 고각
        /// </summary>
        public decimal Elevation { get; private set; } = 0;

        /// <summary>
        /// 구간 시작거리
        /// </summary>
        public decimal StartDistance { get; private set; } = 0;
        /// <summary>
        /// 구간 종료거리
        /// </summary>
        public decimal FinishDistance { get; private set; } = 0;
        /// <summary>
        /// 구간 시작속도
        /// </summary>
        public decimal StartVelocity { get; private set; } = 0;
        /// <summary>
        /// 구간 종료속도
        /// </summary>
        public decimal FinishVelocity { get; private set; } = 0;

        #endregion

        #region 유도되는 값

        /// <summary>
        /// 구간 총거리
        /// </summary>
        public decimal SectionDistance { get; private set; } = 0;
        /// <summary>
        /// 구간 총 시간
        /// </summary>
        public decimal SectionSecond { get; private set; } = 0;
        /// <summary>
        /// 구간 평균 가속도
        /// </summary>
        public decimal Accelation { get; private set; } = 0;

        /// <summary>
        /// 진입 : true
        /// 퇴각 : false
        /// </summary>
        public bool Direction { get; private set; } = false;

        #endregion

        /// <summary>
        /// 누적 거리
        /// </summary>
        public decimal AccumulatedDistance { get; private set; } = 0;
        /// <summary>
        /// 누적 시간
        /// </summary>
        public decimal AccumulatedSecond { get; private set; } = 0;
        /// <summary>
        /// 구간 시작시간
        /// </summary>
        private decimal StartSecond { get; set; } = 0;



        public MissileMovingSection()
        {

        }

        public MissileMovingSection(decimal start, decimal finish, decimal startv, decimal finishv)
        {
            this.Initialize(start, finish, startv, finishv);
        }

        public MissileMovingSection(string start, string finish, string startv, string finishv)
        {
            decimal sd = Convert.ToDecimal(start);
            decimal fd = Convert.ToDecimal(finish);
            decimal sv = Convert.ToDecimal(startv);
            decimal fv = Convert.ToDecimal(finishv);

            this.Initialize(sd, fd, sv, fv);
        }

        public MissileMovingSection(decimal start, decimal finish, decimal startv, decimal finishv, decimal azimuth, decimal elevation) : this(start, finish, startv, finishv)
        {
            this.SetElevation(elevation);
            this.SetAzimuth(azimuth);
        }

        private void Initialize(decimal start, decimal finish, decimal startv, decimal finishv)
        {
            try
            {
                this.StartDistance = start;
                this.FinishDistance = finish;

                this.SectionDistance = Math.Abs(this.FinishDistance - this.StartDistance);
                this.Direction = this.StartDistance > this.FinishDistance;

                this.StartVelocity = startv * (this.Direction ? -1 : 1);
                this.FinishVelocity = finishv * (this.Direction ? -1 : 1);

                // 주어진 속도로 시간 계산
                decimal sumSpeed = this.FinishVelocity + this.StartVelocity;

                // Log.Ins.Debug("평균속도 : " + sumSpeed);

                if (sumSpeed.Equals( 0.0m ))
                {
                    // Log.Ins.Debug("평균속도 : *0 설정");
                    this.SectionSecond = 0;
                    this.Accelation = 0;
                }
                else
                {
                    // Log.Ins.Debug($"평균속도 : {sumSpeed} 설정");
                    this.SectionSecond = Math.Abs(2 * this.SectionDistance / sumSpeed);
                    // Log.Ins.Debug($"구간진행시간 : {this.SectionSecond} 설정");

                    if (this.SectionSecond > 0)
                    {
                        this.Accelation = (this.FinishVelocity - this.StartVelocity) / this.SectionSecond;
                    }
                    else
                    {
                        this.Accelation = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                // Log.Ins.Exception("0으로 나눴다는데 어디야?");
                Log.Ins.Exception(ex);
            }
        }

        public void SetAzimuth(decimal a)
        {
            this.Azimuth = a;

            // Console.WriteLine($" - Azimuth : {this.Azimuth}");
        }

        public void SetElevation( decimal e )
        {
            this.Elevation = e;

            // Console.WriteLine($" - Elevation : {this.Elevation}");
        }

        /// <summary>
        /// 특정시간에서의 거리
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public decimal RunningDistance(decimal sec)
        {
            if (this.StartSecond < sec)
            {
                sec = (sec - this.StartSecond);
            }

            return this.StartDistance + (this.StartVelocity * sec) + (0.5M * this.Accelation * sec * sec );
        }

        public int RunningDistance(double sec)
        {
            var d = this.RunningDistance((decimal)sec);
            return (int)Math.Round(d, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 특정시간에서의 속도
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public decimal RunningVelocity(decimal sec)
        {
            if (this.StartSecond < sec)
            {
                sec = sec - this.StartSecond;
            }

            // double v = (double)(this.StartVelocity + (this.Accelation * (decimal)sec));
            return this.StartVelocity + (this.Accelation * sec);
        }

        public int RunningVelocity(double sec)
        {
            var d = this.RunningVelocity((decimal)sec);
            return (int)Math.Round(d, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 특정거리에서의 3차원좌표
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public OCS_3D CurrentLocation3D(decimal distance)
        {
            decimal el = this.ToRadian(this.Elevation);
            decimal az = this.ToRadian(this.Azimuth);

            decimal x = distance * (decimal)Math.Cos((double)el) * (decimal)Math.Cos((double)az);
            decimal y = distance * (decimal)Math.Sin((double)el);
            decimal z = distance * (decimal)Math.Cos((double)el) * (decimal)Math.Sin((double)az);

            return new OCS_3D(x, y, z);
        }

        /// <summary>
        /// 특정속도에서의 3차원좌표속도
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public OCS_3D_Velocity CurrentVelocity3D(decimal velocity)
        {
            var v = CurrentLocation3D(velocity);

            return new OCS_3D_Velocity(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// 특정시간에서의 3차원좌표
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public OCS_3D CurrentLocation3DmSecond(decimal milisecond)
        {
            decimal d = this.RunningDistance(milisecond / 1000.0m);
            return this.CurrentLocation3D(d);
        }

        /// <summary>
        /// 특정시간에서의 3차원좌표속도
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public OCS_3D_Velocity CurrentVelocity3DmSecond(decimal milisecond)
        {
            decimal v = this.RunningVelocity(milisecond / 1000.0m);

            return this.CurrentVelocity3D(v);
        }

        public void Add(decimal d, decimal s)
        {
            this.AccumulatedDistance = this.SectionDistance + d;
            this.AccumulatedSecond = this.SectionSecond + s;
            this.StartSecond = s;
        }
    }

    public class MissileMovingSectionItem : MissileMovingSection
    {


        public MissileMovingSectionItem(decimal start, decimal finish, decimal startv, decimal finishv) : base(start, finish, startv, finishv)
        {
        }

        public MissileMovingSectionItem(string start, string finish, string startv, string finishv) : base(start, finish, startv, finishv)
        {
        }

        public MissileMovingSectionItem(decimal start, decimal finish, decimal startv, decimal finishv, decimal azimuth, decimal elevation) : base(start, finish, startv, finishv, azimuth, elevation)
        {
        }

        
    }
}

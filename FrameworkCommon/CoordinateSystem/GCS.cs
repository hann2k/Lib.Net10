using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.CoordinateSystem
{
    /// <summary>
    /// Geographic Coordinate System / 지리좌표계 : 지구 중심을 원점으로 하는 구면좌표계
    /// 
    /// 위도, 경도
    /// 고도
    /// </summary>
    public class GCS : Coordinates
    {
        /// <summary>
        /// 위도(Degree). 원점 : 적도기준
        /// </summary>
        public double Latitude {
            get;
            private set;
        } = 0;

        /// <summary>
        /// 경도(Degree). 원점 : 영국 그리니치 천문대 자오선 -> 현재는 IERS기준자오선, 그리지치 자오선보다 동쪽으로 102.5m 떨어져 있음.
        /// </summary>
        public double Longitude
        {
            get;
            private set;
        } = 0;

        /// <summary>
        /// 고도(meter)
        /// </summary>
        public double Altitude
        {
            get;
            private set;
        } = 0;

        public GCS()
        {
        }

        public GCS(double lati, double longi, double alti)
        {
            this.SetLatitude(lati);
            this.SetLongitude(longi);
            this.SetAltitude(alti);
        }

        public void SetLatitude(double lati)
        {
            this.Latitude = lati;
        }

        public void SetLongitude( double longi)
        {
            this.Longitude = longi;
        }

        public void SetAltitude( double alti )
        {
            this.Altitude = alti;
        }

        public OCS_3D ToOCS_3D()
        {
            double ecc1sq = Earth.Flatterning * (2.0 - Earth.Flatterning);
            double rp = Earth.MajorRadius / Math.Sqrt(1.0 - (ecc1sq * Math.Sin(base.ToRadian(this.Latitude)) * Math.Sin(base.ToRadian(this.Latitude))));

            OCS_3D ocs = new OCS_3D(
                (rp + this.Altitude) * Math.Cos(base.ToRadian(this.Latitude)) * Math.Cos(base.ToRadian(this.Longitude)),
                (rp + this.Altitude) * Math.Cos(base.ToRadian(this.Latitude)) * Math.Sin(base.ToRadian(this.Longitude)),
                (((1.0 - ecc1sq) * rp) + this.Altitude) * Math.Sin(base.ToRadian(this.Latitude))
            );

            return ocs;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.CoordinateSystem
{
    /// <summary>
    /// 좌표계
    /// </summary>
    /// <remarks>여긴뭐지</remarks>
    public abstract class Coordinates
    {
        /// <summary>
        /// 360각도를 라디안으로 변경
        /// </summary>
        /// <param name="degreeAngle"></param>
        /// <returns></returns>
        protected double ToRadian(double degreeAngle)
        {
            return Math.PI * degreeAngle / 180;
        }

        protected decimal ToRadian(decimal degreeAngle)
        {
            return (decimal)Math.PI * degreeAngle / 180m;
        }

        /// <summary>
        /// 라디안을 360각도로 변경
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        protected double ToDegree(double radian)
        {
            return radian * 180 / Math.PI;
        }

        protected decimal ToDegree(decimal radian)
        {
            return radian * 180m / (decimal)Math.PI;
        }
    }

    /// <summary>
    /// 평면 직각 좌표계(데카르트 좌표계)
    /// </summary>
    public class CartesianCoordinates : Coordinates
    {
        public double X { get; private set; } = 0;
        public double Y { get; private set; } = 0;

        /// <summary>
        /// 사분면
        /// </summary>
        public virtual int Quadrant {
            get {
                return this.X == 0 && this.Y == 0 ? 0 : this.X > 0 ? this.Y > 0 ? 1 : 4 : this.Y > 0 ? 2 : 3;
            }
        }

        public CartesianCoordinates( double x, double y )
        {
            this.X = x;
            this.Y = y;
        }

        public CartesianCoordinates(PolarCoordinates p)
        {
            var c = p.ToCartesian();
            this.X = c.X;
            this.Y = c.Y;
        }

        /// <summary>
        /// 직교좌표를 극좌표로 변환
        /// </summary>
        /// <returns></returns>
        public PolarCoordinates ToPolarCoordinates()
        {
            double r = Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
            double φ_ph = Math.Tanh(base.ToRadian(this.Y / this.X));
            return new PolarCoordinates(r, φ_ph);
        }
    }

    /// <summary>
    /// 평면 극좌표계
    /// </summary>
    public class PolarCoordinates : Coordinates
    {
        public double R { get; private set; } = 0;
        public double φ_ph { get; private set; } = 0;

        public PolarCoordinates( double r, double φ)
        {
            this.R = r;
            this.φ_ph = φ;
        }

        public PolarCoordinates(CartesianCoordinates c)
        {
            var p = c.ToPolarCoordinates();
            this.R = p.R;
            this.φ_ph = p.φ_ph;
        }

        /// <summary>
        /// 극좌표를 직교좌표로 변환
        /// </summary>
        /// <returns></returns>
        public CartesianCoordinates ToCartesian()
        {
            double rad = this.ToRadian(this.φ_ph);
            double x = this.R * Math.Cos(rad);
            double y = this.R * Math.Sin(rad);

            return new CartesianCoordinates(x, y);
        }
    }

    /// <summary>
    /// 입체 직각 좌표계(3D)
    /// </summary>
    public class Cartesian3DCoordinates: CartesianCoordinates
    {
        public double Z { get; private set; } = 0;

        public Cartesian3DCoordinates( double x, double y, double z) : base (x, y)
        {
            this.Z = z;
        }

        /// <summary>
        /// 직교좌표를 원통좌표로 변환
        /// </summary>
        /// <returns></returns>
        public CylindricalCoordinate ToCylindrical()
        {
            PolarCoordinates p = base.ToPolarCoordinates();
            return new CylindricalCoordinate(p.R, p.φ_ph, this.Z);
        }

        /// <summary>
        /// 직교좌표를 구면좌표로 변환
        /// </summary>
        /// <returns></returns>
        public SphericalCoordinate ToSpherical()
        {
            double r = Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2));
            double p = Math.Tanh(base.ToRadian(this.Y / this.X));
            double th = Math.Cosh(base.ToRadian(this.Z / this.X));

            return new SphericalCoordinate(r, p, th);
        }
    }

    /// <summary>
    /// 구면 좌표계(3D)
    /// </summary>
    public class SphericalCoordinate: PolarCoordinates
    {
        public double θ { get; private set; } = 0;

        public SphericalCoordinate(double r, double p, double th) : base(r, p)
        {
            this.θ = th;
        }

        /// <summary>
        /// 구면좌표를 원통좌표로 변환
        /// </summary>
        /// <returns></returns>
        public CylindricalCoordinate ToCylinderical()
        {
            double thR = base.ToRadian(this.θ);

            double r = this.R * Math.Sin(thR);
            double z = this.R * Math.Cos(thR);

            return new CylindricalCoordinate(r, this.φ_ph, z);
        }

        /// <summary>
        /// 구면좌표를 직교좌표로 변환
        /// </summary>
        /// <returns></returns>
        public Cartesian3DCoordinates TooCartesian3D()
        {
            double thR = base.ToRadian(this.θ);
            double pR = base.ToRadian(this.φ_ph);

            double x = this.R * Math.Sin(thR) * Math.Cos(pR);
            double y = this.R * Math.Sin(thR) * Math.Sin(pR);
            double z = this.R * Math.Cos(thR);

            return new Cartesian3DCoordinates(x, y, z);
        }
    }

    /// <summary>
    /// 원통 좌표계(3D)
    /// </summary>
    public class CylindricalCoordinate : PolarCoordinates
    {
        public double Z { get; private set; } = 0;

        public CylindricalCoordinate(double r, double p, double z ) : base (r, p)
        {
            this.Z = z;
        }

        /// <summary>
        /// 원통좌표를 구면좌표로 변환
        /// </summary>
        /// <returns></returns>
        public SphericalCoordinate ToSpherical()
        {
            double r = Math.Sqrt(Math.Pow(this.R, 2) + Math.Pow(this.Z, 2));
            double th = Math.Tanh(base.ToRadian(this.R / this.Z));

            return new SphericalCoordinate(r, this.φ_ph, th);
        }

        /// <summary>
        /// 원통좌표를 직교좌표로 변환
        /// </summary>
        /// <returns></returns>
        public Cartesian3DCoordinates TooCartesian3D()
        {
            var c = base.ToCartesian();
            return new Cartesian3DCoordinates(c.X, c.Y, this.Z);
        }
    }
}

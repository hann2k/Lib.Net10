using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.CoordinateSystem
{
    /// <summary>
    /// Orthogonal Coordinate System / 직교좌표계
    /// </summary>
    public class OCS_3D
    {
        /// <summary>
        /// 길이
        /// </summary>
        public decimal X { get; private set; }

        /// <summary>
        /// 길이
        /// </summary>
        public decimal Y { get; private set; }

        /// <summary>
        /// 높이
        /// </summary>
        public decimal Z { get; private set; }

        public int IntX => (int)Math.Round(this.X, MidpointRounding.AwayFromZero);
        public int IntY => (int)Math.Round(this.Y, MidpointRounding.AwayFromZero);
        public int IntZ => (int)Math.Round(this.Z, MidpointRounding.AwayFromZero);

        public OCS_3D(short x, short y, short z) : this((decimal)x, y, z)
        {

        }
        public OCS_3D(int x, int y, int z) : this((decimal)x, y, z)
        {

        }

        public OCS_3D(double x, double y, double z) : this((decimal)x, (decimal)y, (decimal)z)
        {

        }
        
        public OCS_3D(decimal x, decimal y, decimal z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public OCS_3D Clone()
        {
            return new OCS_3D(this.X, this.Y, this.Z);
        }
    }

    public class OCS_3D_Velocity
    {
        /// <summary>
        /// 길이
        /// </summary>
        public decimal Vx { get; private set; }

        /// <summary>
        /// 길이
        /// </summary>
        public decimal Vy { get; private set; }

        /// <summary>
        /// 높이
        /// </summary>
        public decimal Vz { get; private set; }

        public int IntVx => (int)Math.Round(this.Vx, MidpointRounding.AwayFromZero);
        public int IntVy => (int)Math.Round(this.Vy, MidpointRounding.AwayFromZero);
        public int IntVz => (int)Math.Round(this.Vz, MidpointRounding.AwayFromZero);

        public OCS_3D_Velocity(short x, short y, short z) : this((decimal)x, (decimal)y, (decimal)z)
        {

        }

        public OCS_3D_Velocity(int x, int y, int z) : this((decimal)x, (decimal)y, (decimal)z)
        {

        }

        public OCS_3D_Velocity(double x, double y, double z) : this((decimal)x, (decimal)y, (decimal)z)
        {

        }

        public OCS_3D_Velocity(decimal vx, decimal vy, decimal vz)
        {
            this.Vx = vx;
            this.Vy = vy;
            this.Vz = vz;
        }

        public OCS_3D_Velocity Clone()
        {
            return new OCS_3D_Velocity(this.Vx, this.Vy, this.Vz);
        }
    }
}

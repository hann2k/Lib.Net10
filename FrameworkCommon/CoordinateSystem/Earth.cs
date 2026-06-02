using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.CoordinateSystem
{
    /// <summary>
    /// 지구 관련 상수 모음
    /// </summary>
    public class Earth
    {
        /// <summary>
        /// 편평율.
        /// </summary>
        public const double Flatterning = 1.0 / 298.257223563;

        /// <summary>
        /// 지구 반지름. (m)
        /// </summary>
        public const double MajorRadius = 6378137.0;
    }
}

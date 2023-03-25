using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Static helper methods for hex classes.
    /// </summary>
    public static class HexHelper
    {
        /// <summary>
        /// Determine the <see cref="HexEdgeType"/> based on two elevations.
        /// </summary>
        /// <param name="elevation1">First elevation.</param>
        /// <param name="elevation2">Second elevation.</param>
        /// <returns>Matching <see cref="HexEdgeType"/>.</returns>
        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
                return HexEdgeType.Flat;
            int delta = elevation2 - elevation1;
            if (delta == 1 || delta == -1)
                return HexEdgeType.Slope;
            return HexEdgeType.Cliff;
        }
    }
}

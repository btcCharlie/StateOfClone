using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Constant metrics and utility methods for the hex map.
    /// </summary>
    public static class HexMetrics
    {
        #region Hex Geometric Constants
        /// <summary>
        /// Ratio of outer to inner radius of a hex cell.
        /// </summary>
        public const float outerToInner = 0.866025404f;

        /// <summary>
        /// Ratio of inner to outer radius of a hex cell.
        /// </summary>
        public const float innerToOuter = 1f / outerToInner;

        /// <summary>
        /// Outer radius of a hex cell (center to vertex). 
        /// Radius of a smallest circle that fully contains the hex.
        /// </summary>
        public const float outerRadius = 10f;

        /// <summary>
        /// Inner radius of a hex cell (center to edge).
        /// Radius of a largest circle that can be inscribed in the hex.
        /// </summary>
        public const float innerRadius = outerRadius * outerToInner;

        /// <summary>
        /// Inner diameter of a hex cell.
        /// </summary>
        public const float innerDiameter = innerRadius * 2f;

        private static readonly Vector3[] corners = {
            new Vector3(0f, 0f, outerRadius),
            new Vector3(innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(0f, 0f, -outerRadius),
            new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(0f, 0f, outerRadius)
        };
        #endregion

        #region Cell Body
        /// <summary>
        /// Factor of the solid uniform region inside a hex cell.
        /// </summary>
        public const float solidFactor = 0.8f;

        /// <summary>
        /// Factor of the blending region inside a hex cell.
        /// </summary>
        public const float blendFactor = 1f - solidFactor;

        /// <summary>
        /// Factor of the solid uniform water region inside a hex cell.
        /// </summary>
        public const float waterFactor = 0.6f;

        /// <summary>
        /// Offset for water elevation.
        /// </summary>
        public const float waterElevationOffset = -0.5f;

        /// <summary>
        /// Factor of the water blending region inside a hex cell.
        /// </summary>
        public const float waterBlendFactor = 1f - waterFactor;

        /// <summary>
        /// Height of a single elevation step.
        /// </summary>
        public const float elevationStep = 3f;
        #endregion

        #region Noise
        /// <summary>
        /// Size of the hash grid.
        /// </summary>
        public const int hashGridSize = 256;

        /// <summary>
        /// World scale of the hash grid.
        /// </summary>
        public const float hashGridScale = 0.25f;

        /// <summary>
        /// World scale of the noise.
        /// </summary>
        public const float noiseScale = 0.003f;

        private static HexHash[] hashGrid;

        /// <summary>
        /// Texture used for sampling noise.
        /// </summary>
        public static Texture2D noiseSource;

        /// <summary>
        /// Sample the noise texture.
        /// </summary>
        /// <param name="position">Sample position.</param>
        /// <returns>Four-component noise sample.</returns>
        public static Vector4 SampleNoise(Vector3 position)
        {
            Vector4 sample = noiseSource.GetPixelBilinear(
                position.x * noiseScale,
                position.z * noiseScale
            );

            if (Wrapping && position.x < innerDiameter * 1.5f)
            {
                Vector4 sample2 = noiseSource.GetPixelBilinear(
                    (position.x + wrapSize * innerDiameter) * noiseScale,
                    position.z * noiseScale
                );
                sample = Vector4.Lerp(
                    sample2, sample, position.x * (1f / innerDiameter) - 0.5f
                );
            }

            return sample;
        }

        /// <summary>
        /// Initialize the hash grid.
        /// </summary>
        /// <param name="seed">Seed to use for initialization.</param>
        public static void InitializeHashGrid(int seed)
        {
            hashGrid = new HexHash[hashGridSize * hashGridSize];
            Random.State currentState = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < hashGrid.Length; i++)
            {
                hashGrid[i] = HexHash.Create();
            }
            Random.state = currentState;
        }

        /// <summary>
        /// Sample the hash grid.
        /// </summary>
        /// <param name="position">Sample position</param>
        /// <returns>Sampled <see cref="HexHash"/>.</returns>
        public static HexHash SampleHashGrid(Vector3 position)
        {
            int x = (int)(position.x * hashGridScale) % hashGridSize;
            if (x < 0)
            {
                x += hashGridSize;
            }
            int z = (int)(position.z * hashGridScale) % hashGridSize;
            if (z < 0)
            {
                z += hashGridSize;
            }
            return hashGrid[x + z * hashGridSize];
        }
        #endregion

        #region Perturbation
        /// <summary>
        /// Strength of cell position terturbation.
        /// </summary>
        public const float cellPerturbStrength = 4f;

        /// <summary>
        /// Strength of vertical elevation perturbation.
        /// </summary>
        public const float elevationPerturbStrength = 1.5f;

        /// <summary>
        /// Perturn a position.
        /// </summary>
        /// <param name="position">A position.</param>
        /// <returns>The positions with noise applied to its XZ components.</returns>
        public static Vector3 Perturb(Vector3 position)
        {
            Vector4 sample = SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
            position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
            return position;
        }
        #endregion

        #region Chunking
        /// <summary>
        /// Hex grid chunk size in the X dimension.
        /// </summary>
        public const int chunkSizeX = 5;

        /// <summary>
        /// Hex grid chunk size in the Z dimension.
        /// </summary>
        public const int chunkSizeZ = 5;

        /// <summary>
        /// Wrap size of the map, matching its X size if east-west wrapping is enabled.
        /// </summary>
        public static int wrapSize;

        /// <summary>
        /// Whether east-west map wrapping is enabled.
        /// </summary>
        public static bool Wrapping => wrapSize > 0;
        #endregion

        #region Geometric Calculations
        /// <summary>
        /// Get the first outer cell corner for a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the counter-clockwise side.</returns>
        public static Vector3 GetFirstCorner(HexDirection direction) =>
            corners[(int)direction];

        /// <summary>
        /// Get the second outer cell corner for a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the clockwise side.</returns>
        public static Vector3 GetSecondCorner(HexDirection direction) =>
            corners[(int)direction + 1];

        /// <summary>
        /// Get the first inner solid cell corner for a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the counter-clockwise side.</returns>
        public static Vector3 GetFirstSolidCorner(HexDirection direction) =>
            corners[(int)direction] * solidFactor;

        /// <summary>
        /// Get the second inner solid cell corner for a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the clockwise side.</returns>
        public static Vector3 GetSecondSolidCorner(HexDirection direction) =>
            corners[(int)direction + 1] * solidFactor;

        /// <summary>
        /// Get the middle of the inner solid cell edge for a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The position in between the two inner solid cell corners.</returns>
        public static Vector3 GetSolidEdgeMiddle(HexDirection direction) =>
            (corners[(int)direction] + corners[(int)direction + 1]) *
            (0.5f * solidFactor);

        /// <summary>
        /// Get the first inner water cell corner for a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the counter-clockwise side.</returns>
        public static Vector3 GetFirstWaterCorner(HexDirection direction) =>
            corners[(int)direction] * waterFactor;

        /// <summary>
        /// Get the second inner water cell corner for a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the clockwise side.</returns>
        public static Vector3 GetSecondWaterCorner(HexDirection direction) =>
            corners[(int)direction + 1] * waterFactor;

        /// <summary>
        /// Get the vector needed to bridge to the next cell for a given direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The bridge vector.</returns>
        public static Vector3 GetBridge(HexDirection direction) =>
            (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;

        /// <summary>
        /// Get the vector needed to bridge to the next water cell for a given direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The bridge vector.</returns>
        public static Vector3 GetWaterBridge(HexDirection direction) =>
            (corners[(int)direction] + corners[(int)direction + 1]) * waterBlendFactor;
        #endregion

        /// <summary>
        /// Determine the <see cref="HexEdgeType"/> based on two elevations.
        /// </summary>
        /// <param name="elevation1">First elevation.</param>
        /// <param name="elevation2">Second elevation.</param>
        /// <returns>Matching <see cref="HexEdgeType"/>.</returns>
        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {
                return HexEdgeType.Flat;
            }
            int delta = elevation2 - elevation1;
            if (delta == 1 || delta == -1)
            {
                return HexEdgeType.Slope;
            }
            return HexEdgeType.Cliff;
        }
    }
}


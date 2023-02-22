using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Container component for hex cell data.
    /// </summary>
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// Hexagonal coordinates unique to the cell.
        /// </summary>
        public HexCoordinates Coordinates { get; set; }

        /// <summary>
        /// Transform component for the cell's UI visiualization. 
        /// </summary>
        public RectTransform UIRect { get; set; }

        /// <summary>
        /// Grid chunk that contains the cell.
        /// </summary>
        public HexGridChunk Chunk { get; set; }

        /// <summary>
        /// Unique global index of the cell.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Surface elevation level.
        /// </summary>
        public int Elevation
        {
            get => elevation;
            set
            {
                if (elevation == value)
                {
                    return;
                }
                int originalViewElevation = ViewElevation;
                elevation = value;
                if (ViewElevation != originalViewElevation)
                {
                    ShaderData.ViewElevationChanged();
                }
                RefreshPosition();
                Refresh();
            }
        }

        /// <summary>
        /// Water elevation level.
        /// </summary>
        public int WaterLevel
        {
            get => waterLevel;
            set
            {
                if (waterLevel == value)
                    return;
                int originalViewElevation = ViewElevation;
                waterLevel = value;
                if (ViewElevation != originalViewElevation)
                    ShaderData.ViewElevationChanged();
                Refresh();
            }
        }

        /// <summary>
        /// Elevation at which the cell is visible. Highest of surface and water level.
        /// </summary>
        public int ViewElevation => elevation >= waterLevel ? elevation : waterLevel;

        /// <summary>
        /// Whether the cell counts as underwater, which is when water is higher than surface.
        /// </summary>
        public bool IsUnderwater => waterLevel > elevation;

        /// <summary>
        /// Local position of this cell's game object.
        /// </summary>
        public Vector3 Position => transform.localPosition;

        /// <summary>
        /// Vertical position of the water surface, if applicable.
        /// </summary>
        public float WaterSurfaceY =>
            (waterLevel + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep;

        /// <summary>
        /// Terrain type index.
        /// </summary>
        public int TerrainTypeIndex
        {
            get => terrainTypeIndex;
            set
            {
                if (terrainTypeIndex != value)
                {
                    terrainTypeIndex = value;
                    ShaderData.RefreshTerrain(this);
                }
            }
        }

        /// <summary>
        /// Whether the cell counts as visible.
        /// </summary>
        public bool IsVisible => visibility > 0 && Explorable;

        /// <summary>
        /// Whether the cell counts as explored.
        /// </summary>
        public bool IsExplored
        {
            get => explored && Explorable;
            private set => explored = value;
        }

        /// <summary>
        /// Whether the cell is explorable. If not it never counts as explored or visible.
        /// </summary>
        public bool Explorable { get; set; }

        /// <summary>
        /// Distance data used by pathfiding algorithm.
        /// </summary>
        public int Distance
        {
            get => distance;
            set => distance = value;
        }

        /// <summary>
        /// Reference to <see cref="HexCellShaderData"/> that contains the cell.
        /// </summary>
        public HexCellShaderData ShaderData { get; set; }

        private int terrainTypeIndex;

        private int elevation = int.MinValue;
        private int waterLevel;

        private int distance;

        private int visibility;

        private bool explored;

        [SerializeField]
        private HexCell[] neighbors;

        /// <summary>
        /// Increment visibility level.
        /// </summary>
        public void IncreaseVisibility()
        {
            visibility += 1;
            if (visibility == 1)
            {
                IsExplored = true;
                ShaderData.RefreshVisibility(this);
            }
        }

        /// <summary>
        /// Decrement visiblility level.
        /// </summary>
        public void DecreaseVisibility()
        {
            visibility -= 1;
            if (visibility == 0)
                ShaderData.RefreshVisibility(this);
        }

        /// <summary>
        /// Reset visibility level to zero.
        /// </summary>
        public void ResetVisibility()
        {
            if (visibility > 0)
            {
                visibility = 0;
                ShaderData.RefreshVisibility(this);
            }
        }

        /// <summary>
        /// Get one of the neighbor cells.
        /// </summary>
        /// <param name="direction">Neighbor direction relative to the cell.</param>
        /// <returns>Neighbor cell, if it exists.</returns>
        public HexCell GetNeighbor(HexDirection direction) => neighbors[(int)direction];

        /// <summary>
        /// Set a specific neighbor.
        /// </summary>
        /// <param name="direction">Neighbor direction relative to the cell.</param>
        /// <param name="cell">Neighbor.</param>
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        /// <summary>
        /// Get the <see cref="HexEdgeType"/> of a cell edge.
        /// </summary>
        /// <param name="direction">Edge direction relative to the cell.</param>
        /// <returns><see cref="HexEdgeType"/> based on the neighboring cells.</returns>
        public HexEdgeType GetEdgeType(HexDirection direction) => HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );

        /// <summary>
        /// Get the <see cref="HexEdgeType"/> based on this and another cell.
        /// </summary>
        /// <param name="otherCell">Other cell to consider as neighbor.</param>
        /// <returns><see cref="HexEdgeType"/> based on this and the other cell.</returns>
        public HexEdgeType GetEdgeType(HexCell otherCell) => HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );

        /// <summary>
        /// Get the elevation difference with a neighbor. The indicated neighbor must exist.
        /// </summary>
        /// <param name="direction">Direction to the neighbor, relative to the cell.</param>
        /// <returns>Absolute elevation difference.</returns>
        public int GetElevationDifference(HexDirection direction)
        {
            int difference = elevation - GetNeighbor(direction).elevation;
            return difference >= 0 ? difference : -difference;
        }

        void RefreshPosition()
        {
            Vector3 position = transform.localPosition;
            position.y = elevation * HexMetrics.elevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = UIRect.localPosition;
            uiPosition.z = -position.y;
            UIRect.localPosition = uiPosition;
        }

        void Refresh()
        {
            if (Chunk)
            {
                Chunk.Refresh();
                for (int i = 0; i < neighbors.Length; i++)
                {
                    HexCell neighbor = neighbors[i];
                    if (neighbor != null && neighbor.Chunk != Chunk)
                        neighbor.Chunk.Refresh();
                }
            }
        }

        void RefreshSelfOnly()
        {
            Chunk.Refresh();
        }

        /// <summary>
        /// Save the cell data.
        /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> to use.</param>
        public void Save(BinaryWriter writer)
        {
            writer.Write((byte)terrainTypeIndex);
            writer.Write((byte)(elevation + 127));
            writer.Write((byte)waterLevel);

            writer.Write(IsExplored);
        }

        /// <summary>
        /// Load the cell data.
        /// </summary>
        /// <param name="reader"><see cref="BinaryReader"/> to use.</param>
        /// <param name="header">Header version.</param>
        public void Load(BinaryReader reader, int header)
        {
            terrainTypeIndex = reader.ReadByte();
            ShaderData.RefreshTerrain(this);
            elevation = reader.ReadByte();
            if (header >= 4)
                elevation -= 127;
            RefreshPosition();
            waterLevel = reader.ReadByte();

            IsExplored = header >= 3 ? reader.ReadBoolean() : false;
            ShaderData.RefreshVisibility(this);
        }

        /// <summary>
        /// Set the cell's UI label.
        /// </summary>
        /// <param name="text">Label text.</param>
        public void SetLabel(string text)
        {
            UnityEngine.UI.Text label = UIRect.GetComponent<Text>();
            label.text = text;
        }

        /// <summary>
        /// Disable the cell's highlight.
        /// </summary>
        public void DisableHighlight()
        {
            Image highlight = UIRect.GetChild(0).GetComponent<Image>();
            highlight.enabled = false;
        }

        /// <summary>
        /// Enable the cell's highlight. 
        /// </summary>
        /// <param name="color">Highlight color.</param>
        public void EnableHighlight(Color color)
        {
            Image highlight = UIRect.GetChild(0).GetComponent<Image>();
            highlight.color = color;
            highlight.enabled = true;
        }

        /// <summary>
        /// Set the map data for this cell's <see cref="ShaderData"/>.
        /// </summary>
        /// <param name="data">Data value.</param>
        public void SetMapData(float data) => ShaderData.SetMapData(this, data);
    }
}

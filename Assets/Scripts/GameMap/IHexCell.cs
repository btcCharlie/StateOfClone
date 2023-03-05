using UnityEngine;
using System.IO;

namespace StateOfClone.GameMap
{
    public interface IHexCell
    {
        HexCoordinates Coordinates { get; set; }
        RectTransform UIRect { get; set; }
        HexGridChunk Chunk { get; set; }
        int Index { get; set; }
        int ColumnIndex { get; set; }
        int Elevation { get; set; }
        int WaterLevel { get; set; }
        int ViewElevation { get; }
        bool IsUnderwater { get; }
        Vector3 Position { get; }
        float WaterSurfaceY { get; }
        int TerrainTypeIndex { get; set; }
        bool IsVisible { get; }
        bool IsExplored { get; }
        bool Explorable { get; set; }
        int Distance { get; set; }
        HexCell PathFrom { get; set; }
        int SearchHeuristic { get; set; }
        int SearchPriority { get; }
        int SearchPhase { get; set; }
        HexCell NextWithSamePriority { get; set; }
        HexCellShaderData ShaderData { get; set; }

        void DecreaseVisibility();
        void DisableHighlight();
        void EnableHighlight(Color color);
        HexEdgeType GetEdgeType(HexDirection direction);
        HexEdgeType GetEdgeType(HexCell otherCell);
        int GetElevationDifference(HexDirection direction);
        HexCell GetNeighbor(HexDirection direction);
        void IncreaseVisibility();
        void Load(BinaryReader reader, int header);
        void ResetVisibility();
        void Save(BinaryWriter writer);
        void SetLabel(string text);
        void SetMapData(float data);
        void SetNeighbor(HexDirection direction, HexCell cell);
    }
}

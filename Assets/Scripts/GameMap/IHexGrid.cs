using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace StateOfClone.GameMap
{
    public interface IHexGrid
    {
        int CellCountX { get; }
        int CellCountZ { get; }
        bool HasPath { get; }
        bool Wrapping { get; }

        void CenterMap(float xPosition);
        void ClearPath();
        bool CreateMap(int x, int z, bool wrapping);
        void DecreaseVisibility(HexCell fromCell, int range);
        void FindPath(HexCell fromCell, HexCell toCell, IHexUnit unit);
        HexCell GetCell(Ray ray);
        HexCell GetCell(Vector3 position);
        HexCell GetCell(HexCoordinates coordinates);
        HexCell GetCell(int xOffset, int zOffset);
        HexCell GetCell(int cellIndex);
        List<HexCell> GetPath();
        void IncreaseVisibility(HexCell fromCell, int range);
        void Load(BinaryReader reader, int header);
        void MakeChildOfColumn(Transform child, int columnIndex);
        void ResetVisibility();
        void Save(BinaryWriter writer);
        void ShowUI(bool visible);
    }
}

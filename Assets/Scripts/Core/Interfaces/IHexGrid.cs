using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace StateOfClone.Core
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
        void DecreaseVisibility(IHexCell fromCell, int range);
        void FindPath(IHexCell fromCell, IHexCell toCell, IHexUnit unit);
        IHexCell GetCell(Ray ray);
        IHexCell GetCell(Vector3 position);
        IHexCell GetCell(HexCoordinates coordinates);
        IHexCell GetCell(int xOffset, int zOffset);
        IHexCell GetCell(int cellIndex);
        List<IHexCell> GetPath();
        void IncreaseVisibility(IHexCell fromCell, int range);
        void Load(BinaryReader reader, int header);
        void MakeChildOfColumn(Transform child, int columnIndex);
        void ResetVisibility();
        void Save(BinaryWriter writer);
        void ShowUI(bool visible);
    }
}

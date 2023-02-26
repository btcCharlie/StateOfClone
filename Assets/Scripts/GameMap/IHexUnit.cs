using System.Collections.Generic;
using System.IO;

namespace StateOfClone.GameMap
{
    public interface IHexUnit
    {
        HexGrid Grid { get; set; }
        HexCell Location { get; set; }
        float Orientation { get; set; }
        int Speed { get; }
        int VisionRange { get; }

        void Die();
        int GetMoveCost(HexCell fromCell, HexCell toCell, HexDirection direction);
        bool IsValidDestination(HexCell cell);
        void Save(BinaryWriter writer);
        void Travel(List<HexCell> path);
        void ValidateLocation();
    }
}

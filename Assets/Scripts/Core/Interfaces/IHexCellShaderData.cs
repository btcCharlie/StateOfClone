namespace StateOfClone.Core
{
    public interface IHexCellShaderData
    {
        IHexGrid Grid { get; set; }
        bool ImmediateMode { get; set; }

        void Initialize(int x, int z);
        void RefreshTerrain(IHexCell cell);
        void RefreshVisibility(IHexCell cell);
        void SetMapData(IHexCell cell, float data);
        void ViewElevationChanged();
    }
}

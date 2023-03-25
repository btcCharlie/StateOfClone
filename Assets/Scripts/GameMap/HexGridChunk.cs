using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Component that manages a single chunk of <see cref="HexGrid"/>.
    /// </summary>
    public class HexGridChunk : MonoBehaviour, IHexGridChunk
    {
        [SerializeField]
        private HexMesh terrain, rivers, roads, water, waterShore, estuaries;

        private HexTriangulator _triangulator;

        private IHexCell[] cells;

        private Canvas gridCanvas;

        private void Awake()
        {
            cells = new IHexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
            gridCanvas = GetComponentInChildren<Canvas>();

            _triangulator = new HexTriangulator(
                terrain, rivers, roads, water, waterShore, estuaries
            );
        }

        private void LateUpdate()
        {
            _triangulator.Triangulate(cells);
            enabled = false;
        }

        /// <summary>
        /// Add a cell to the chunk.
        /// </summary>
        /// <param name="index">Index of the cell for the chunk.</param>
        /// <param name="cell">Cell to add.</param>
        public void AddCell(int index, IHexCell cell)
        {
            cells[index] = cell;
            cell.Chunk = this;
            cell.transform.SetParent(transform, false);
            cell.UIRect.SetParent(gridCanvas.transform, false);
        }

        /// <summary>
        /// Refresh the chunk.
        /// </summary>
        public void Refresh() => enabled = true;

        /// <summary>
        /// Control whether the map UI is visibile or hidden for the chunk.
        /// </summary>
        /// <param name="visible">Whether the UI should be visible.</param>
        public void ShowUI(bool visible) => gridCanvas.gameObject.SetActive(visible);
    }
}
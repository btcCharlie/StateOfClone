using UnityEngine;
using UnityEngine.UI;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Component that manages a single chunk of <see cref="HexGrid"/>.
    /// </summary>
    public class HexGridChunk : MonoBehaviour
    {
        [SerializeField]
        private HexMesh terrain, water, waterShore;

        private HexCell[] cells;

        private Canvas gridCanvas;

        private static Color weights1 = new(1f, 0f, 0f);
        private static Color weights2 = new(0f, 1f, 0f);
        private static Color weights3 = new(0f, 0f, 1f);

        private void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();

            cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        }

        /// <summary>
        /// Add a cell to the chunk.
        /// </summary>
        /// <param name="index">Index of the cell for the chunk.</param>
        /// <param name="cell">Cell to add.</param>
        public void AddCell(int index, HexCell cell)
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

        private void LateUpdate()
        {
            Triangulate();
            enabled = false;
        }

        /// <summary>
        /// Triangulate everything in the chunk.
        /// </summary>
        public void Triangulate()
        {
            terrain.Clear();
            water.Clear();
            waterShore.Clear();
            for (int i = 0; i < cells.Length; i++)
                Triangulate(cells[i]);
            terrain.Apply();
            water.Apply();
            waterShore.Apply();
        }

        void Triangulate(HexCell cell)
        {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                Triangulate(d, cell);
        }

        void Triangulate(HexDirection direction, HexCell cell)
        {
            Vector3 center = cell.Position;
            EdgeVertices e = new EdgeVertices(
                center + HexMetrics.GetFirstSolidCorner(direction),
                center + HexMetrics.GetSecondSolidCorner(direction)
            );

            TriangulateWithoutRiver(direction, cell, center, e);

            if (direction <= HexDirection.SE)
                TriangulateConnection(direction, cell, e);

            if (cell.IsUnderwater)
                TriangulateWater(direction, cell, center);
        }

        void TriangulateWater(
            HexDirection direction, HexCell cell, Vector3 center
        )
        {
            center.y = cell.WaterSurfaceY;

            HexCell neighbor = cell.GetNeighbor(direction);
            if (neighbor != null && !neighbor.IsUnderwater)
                TriangulateWaterShore(direction, cell, neighbor, center);
            else
                TriangulateOpenWater(direction, cell, neighbor, center);
        }

        void TriangulateOpenWater(
            HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
        )
        {
            Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
            Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);

            water.AddTriangle(center, c1, c2);
            Vector3 indices;
            indices.x = indices.y = indices.z = cell.Index;
            water.AddTriangleCellData(indices, weights1);

            if (direction <= HexDirection.SE && neighbor != null)
            {
                Vector3 bridge = HexMetrics.GetWaterBridge(direction);
                Vector3 e1 = c1 + bridge;
                Vector3 e2 = c2 + bridge;

                water.AddQuad(c1, c2, e1, e2);
                indices.y = neighbor.Index;
                water.AddQuadCellData(indices, weights1, weights2);

                if (direction <= HexDirection.E)
                {
                    HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
                    if (nextNeighbor == null || !nextNeighbor.IsUnderwater)
                        return;

                    water.AddTriangle(
                        c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next())
                    );
                    indices.z = nextNeighbor.Index;
                    water.AddTriangleCellData(
                        indices, weights1, weights2, weights3
                    );
                }
            }
        }

        void TriangulateWaterShore(
            HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
        )
        {
            EdgeVertices e1 = new(
                center + HexMetrics.GetFirstWaterCorner(direction),
                center + HexMetrics.GetSecondWaterCorner(direction)
            );
            water.AddTriangle(center, e1.v1, e1.v2);
            water.AddTriangle(center, e1.v2, e1.v3);
            water.AddTriangle(center, e1.v3, e1.v4);
            water.AddTriangle(center, e1.v4, e1.v5);
            Vector3 indices;
            indices.x = indices.z = cell.Index;
            indices.y = neighbor.Index;
            water.AddTriangleCellData(indices, weights1);
            water.AddTriangleCellData(indices, weights1);
            water.AddTriangleCellData(indices, weights1);
            water.AddTriangleCellData(indices, weights1);

            Vector3 center2 = neighbor.Position;
            center2.y = center.y;
            EdgeVertices e2 = new EdgeVertices(
                center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
                center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
            );

            waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
            waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
            waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
            waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
            waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            waterShore.AddQuadCellData(indices, weights1, weights2);
            waterShore.AddQuadCellData(indices, weights1, weights2);
            waterShore.AddQuadCellData(indices, weights1, weights2);
            waterShore.AddQuadCellData(indices, weights1, weights2);

            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (nextNeighbor != null)
            {
                Vector3 center3 = nextNeighbor.Position;
                Vector3 v3 = center3 + (nextNeighbor.IsUnderwater ?
                    HexMetrics.GetFirstWaterCorner(direction.Previous()) :
                    HexMetrics.GetFirstSolidCorner(direction.Previous()));
                v3.y = center.y;
                waterShore.AddTriangle(e1.v5, e2.v5, v3);
                waterShore.AddTriangleUV(
                    new Vector2(0f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)
                );
                indices.z = nextNeighbor.Index;
                waterShore.AddTriangleCellData(
                    indices, weights1, weights2, weights3
                );
            }
        }

        void TriangulateWithoutRiver(
            HexDirection roadDirection, HexCell cell, Vector3 center, EdgeVertices e
        )
        {
            TriangulateEdgeFan(center, e, cell.Index);
        }

        void TriangulateConnection(
            HexDirection direction, HexCell cell, EdgeVertices e1
        )
        {
            HexCell neighbor = cell.GetNeighbor(direction);
            if (neighbor == null)
            {
                return;
            }

            Vector3 bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;
            EdgeVertices e2 = new EdgeVertices(
                e1.v1 + bridge,
                e1.v5 + bridge
            );

            TriangulateEdgeStrip(
                e1, weights1, cell.Index,
                e2, weights2, neighbor.Index
            );

            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
                v5.y = nextNeighbor.Position.y;

                if (cell.Elevation <= neighbor.Elevation)
                {
                    if (cell.Elevation <= nextNeighbor.Elevation)
                    {
                        TriangulateCorner(
                            e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor
                        );
                    }
                    else
                    {
                        TriangulateCorner(
                            v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
                        );
                    }
                }
                else if (neighbor.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(
                        e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell
                    );
                }
                else
                {
                    TriangulateCorner(
                        v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
                    );
                }
            }
        }

        void TriangulateCorner(
            Vector3 bottom, HexCell bottomCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            // HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
            // HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

            terrain.AddTriangle(bottom, left, right);
            Vector3 indices;
            indices.x = bottomCell.Index;
            indices.y = leftCell.Index;
            indices.z = rightCell.Index;
            terrain.AddTriangleCellData(indices, weights1, weights2, weights3);
        }

        void TriangulateBoundaryTriangle(
            Vector3 begin, Color beginWeights,
            Vector3 left, Color leftWeights,
            Vector3 boundary, Color boundaryWeights, Vector3 indices
        )
        {
            // Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
            // Color w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, 1);

            // terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
            // terrain.AddTriangleCellData(indices, beginWeights, w2, boundaryWeights);

            // for (int i = 2; i < HexMetrics.terraceSteps; i++)
            // {
            //     Vector3 v1 = v2;
            //     Color w1 = w2;
            //     v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            //     w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, i);
            //     terrain.AddTriangleUnperturbed(v1, v2, boundary);
            //     terrain.AddTriangleCellData(indices, w1, w2, boundaryWeights);
            // }

            terrain.AddTriangleUnperturbed(
                HexMetrics.Perturb(begin), HexMetrics.Perturb(left), boundary
                );
            terrain.AddTriangleCellData(
                indices, beginWeights, leftWeights, boundaryWeights
                );
        }

        void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index)
        {
            terrain.AddTriangle(center, edge.v1, edge.v2);
            terrain.AddTriangle(center, edge.v2, edge.v3);
            terrain.AddTriangle(center, edge.v3, edge.v4);
            terrain.AddTriangle(center, edge.v4, edge.v5);

            Vector3 indices;
            indices.x = indices.y = indices.z = index;
            terrain.AddTriangleCellData(indices, weights1);
            terrain.AddTriangleCellData(indices, weights1);
            terrain.AddTriangleCellData(indices, weights1);
            terrain.AddTriangleCellData(indices, weights1);
        }

        void TriangulateEdgeStrip(
            EdgeVertices e1, Color w1, float index1,
            EdgeVertices e2, Color w2, float index2
        )
        {
            terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
            terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
            terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
            terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);

            Vector3 indices;
            indices.x = indices.z = index1;
            indices.y = index2;
            terrain.AddQuadCellData(indices, w1, w2);
            terrain.AddQuadCellData(indices, w1, w2);
            terrain.AddQuadCellData(indices, w1, w2);
            terrain.AddQuadCellData(indices, w1, w2);
        }
    }
}

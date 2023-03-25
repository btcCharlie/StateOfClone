using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Contains triangulation logic for meshes in <see cref="HexGridChunk"/>.
    /// </summary>
    public class HexTriangulator
    {
        private readonly HexMesh _terrain, _rivers, _roads, _water, _waterShore, _estuaries;

        private static Color weights1 = new(1f, 0f, 0f);
        private static Color weights2 = new(0f, 1f, 0f);
        private static Color weights3 = new(0f, 0f, 1f);

        public HexTriangulator(
            HexMesh terrain, HexMesh rivers, HexMesh roads,
            HexMesh water, HexMesh waterShore, HexMesh estuaries
            )
        {
            _terrain = terrain;
            _rivers = rivers;
            _roads = roads;
            _water = water;
            _waterShore = waterShore;
            _estuaries = estuaries;
        }

        /// <summary>
        /// Triangulate everything in the chunk.
        /// </summary>
        public void Triangulate(IHexCell[] cells)
        {
            _terrain.Clear();
            _rivers.Clear();
            _roads.Clear();
            _water.Clear();
            _waterShore.Clear();
            _estuaries.Clear();
            for (int i = 0; i < cells.Length; i++)
                Triangulate((HexCell)cells[i]);
            _terrain.Apply();
            _rivers.Apply();
            _roads.Apply();
            _water.Apply();
            _waterShore.Apply();
            _estuaries.Apply();
        }

        private void Triangulate(HexCell cell)
        {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                Triangulate(d, cell);
        }

        private void Triangulate(HexDirection direction, HexCell cell)
        {
            Vector3 center = cell.Position;
            EdgeVertices e = new(
                center + HexMetrics.GetFirstSolidCorner(direction),
                center + HexMetrics.GetSecondSolidCorner(direction)
            );

            TriangulateWithoutRiver(direction, cell, center, e);

            if (direction <= HexDirection.SE)
                TriangulateConnection(direction, cell, e);

            if (cell.IsUnderwater)
                TriangulateWater(direction, cell, center);
        }

        private void TriangulateWater(
            HexDirection direction, HexCell cell, Vector3 center
        )
        {
            center.y = cell.WaterSurfaceY;

            HexCell neighbor = (HexCell)cell.GetNeighbor(direction);
            if (neighbor != null && !neighbor.IsUnderwater)
                TriangulateWaterShore(direction, cell, neighbor, center);
            else
                TriangulateOpenWater(direction, cell, neighbor, center);
        }

        private void TriangulateOpenWater(
            HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
        )
        {
            Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
            Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);

            _water.AddTriangle(center, c1, c2);
            Vector3 indices;
            indices.x = indices.y = indices.z = cell.Index;
            _water.AddTriangleCellData(indices, weights1);

            if (direction <= HexDirection.SE && neighbor != null)
            {
                Vector3 bridge = HexMetrics.GetWaterBridge(direction);
                Vector3 e1 = c1 + bridge;
                Vector3 e2 = c2 + bridge;

                _water.AddQuad(c1, c2, e1, e2);
                indices.y = neighbor.Index;
                _water.AddQuadCellData(indices, weights1, weights2);

                if (direction <= HexDirection.E)
                {
                    HexCell nextNeighbor = (HexCell)cell.GetNeighbor(direction.Next());
                    if (nextNeighbor == null || !nextNeighbor.IsUnderwater)
                        return;
                    _water.AddTriangle(
                        c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next())
                    );
                    indices.z = nextNeighbor.Index;
                    _water.AddTriangleCellData(
                        indices, weights1, weights2, weights3
                    );
                }
            }
        }

        private void TriangulateWaterShore(
            HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
        )
        {
            EdgeVertices e1 = new EdgeVertices(
                center + HexMetrics.GetFirstWaterCorner(direction),
                center + HexMetrics.GetSecondWaterCorner(direction)
            );
            _water.AddTriangle(center, e1.v1, e1.v2);
            _water.AddTriangle(center, e1.v2, e1.v3);
            _water.AddTriangle(center, e1.v3, e1.v4);
            _water.AddTriangle(center, e1.v4, e1.v5);
            Vector3 indices;
            indices.x = indices.z = cell.Index;
            indices.y = neighbor.Index;
            _water.AddTriangleCellData(indices, weights1);
            _water.AddTriangleCellData(indices, weights1);
            _water.AddTriangleCellData(indices, weights1);
            _water.AddTriangleCellData(indices, weights1);

            Vector3 center2 = neighbor.Position;
            if (neighbor.ColumnIndex < cell.ColumnIndex - 1)
                center2.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
            else if (neighbor.ColumnIndex > cell.ColumnIndex + 1)
                center2.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
            center2.y = center.y;
            EdgeVertices e2 = new EdgeVertices(
                center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
                center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
            );

            _waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
            _waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
            _waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
            _waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
            _waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            _waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            _waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            _waterShore.AddQuadUV(0f, 0f, 0f, 1f);
            _waterShore.AddQuadCellData(indices, weights1, weights2);
            _waterShore.AddQuadCellData(indices, weights1, weights2);
            _waterShore.AddQuadCellData(indices, weights1, weights2);
            _waterShore.AddQuadCellData(indices, weights1, weights2);

            HexCell nextNeighbor = (HexCell)cell.GetNeighbor(direction.Next());
            if (nextNeighbor != null)
            {
                Vector3 center3 = nextNeighbor.Position;
                if (nextNeighbor.ColumnIndex < cell.ColumnIndex - 1)
                    center3.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
                else if (nextNeighbor.ColumnIndex > cell.ColumnIndex + 1)
                    center3.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
                Vector3 v3 = center3 + (nextNeighbor.IsUnderwater ?
                    HexMetrics.GetFirstWaterCorner(direction.Previous()) :
                    HexMetrics.GetFirstSolidCorner(direction.Previous()));
                v3.y = center.y;
                _waterShore.AddTriangle(e1.v5, e2.v5, v3);
                _waterShore.AddTriangleUV(
                    new Vector2(0f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)
                );
                indices.z = nextNeighbor.Index;
                _waterShore.AddTriangleCellData(
                    indices, weights1, weights2, weights3
                );
            }
        }

        private void TriangulateEstuary(
            EdgeVertices e1, EdgeVertices e2, bool incomingRiver, Vector3 indices
        )
        {
            _waterShore.AddTriangle(e2.v1, e1.v2, e1.v1);
            _waterShore.AddTriangle(e2.v5, e1.v5, e1.v4);
            _waterShore.AddTriangleUV(
                new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
            );
            _waterShore.AddTriangleUV(
                new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
            );
            _waterShore.AddTriangleCellData(indices, weights2, weights1, weights1);
            _waterShore.AddTriangleCellData(indices, weights2, weights1, weights1);

            _estuaries.AddQuad(e2.v1, e1.v2, e2.v2, e1.v3);
            _estuaries.AddTriangle(e1.v3, e2.v2, e2.v4);
            _estuaries.AddQuad(e1.v3, e1.v4, e2.v4, e2.v5);

            _estuaries.AddQuadUV(
                new Vector2(0f, 1f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2(0f, 0f)
            );
            _estuaries.AddTriangleUV(
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)
            );
            _estuaries.AddQuadUV(
                new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2(0f, 1f)
            );
            _estuaries.AddQuadCellData(
                indices, weights2, weights1, weights2, weights1
            );
            _estuaries.AddTriangleCellData(indices, weights1, weights2, weights2);
            _estuaries.AddQuadCellData(indices, weights1, weights2);

            if (incomingRiver)
            {
                _estuaries.AddQuadUV2(
                    new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f),
                    new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
                );
                _estuaries.AddTriangleUV2(
                    new Vector2(0.5f, 1.1f),
                    new Vector2(1f, 0.8f),
                    new Vector2(0f, 0.8f)
                );
                _estuaries.AddQuadUV2(
                    new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
                    new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)
                );
            }
            else
            {
                _estuaries.AddQuadUV2(
                    new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f),
                    new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
                );
                _estuaries.AddTriangleUV2(
                    new Vector2(0.5f, -0.3f),
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0f)
                );
                _estuaries.AddQuadUV2(
                    new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f),
                    new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)
                );
            }
        }

        private void TriangulateWithoutRiver(
            HexDirection roadDirection, HexCell cell, Vector3 center, EdgeVertices e
        )
        {
            TriangulateEdgeFan(center, e, cell.Index);
        }

        private void TriangulateConnection(
            HexDirection direction, HexCell cell, EdgeVertices e1
        )
        {
            HexCell neighbor = (HexCell)cell.GetNeighbor(direction);
            if (neighbor == null)
                return;
            Vector3 bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;
            EdgeVertices e2 = new(
                e1.v1 + bridge,
                e1.v5 + bridge
            );

            TriangulateEdgeStrip(
                e1, weights1, cell.Index,
                e2, weights2, neighbor.Index
            );

            HexCell nextNeighbor = (HexCell)cell.GetNeighbor(direction.Next());
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

        private void TriangulateWaterfallInWater(
            Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
            float y1, float y2, float waterY, Vector3 indices
        )
        {
            v1.y = v2.y = y1;
            v3.y = v4.y = y2;
            v1 = HexMetrics.Perturb(v1);
            v2 = HexMetrics.Perturb(v2);
            v3 = HexMetrics.Perturb(v3);
            v4 = HexMetrics.Perturb(v4);
            float t = (waterY - y2) / (y1 - y2);
            v3 = Vector3.Lerp(v3, v1, t);
            v4 = Vector3.Lerp(v4, v2, t);
            _rivers.AddQuadUnperturbed(v1, v2, v3, v4);
            _rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
            _rivers.AddQuadCellData(indices, weights1, weights2);
        }

        private void TriangulateCorner(
            Vector3 bottom, HexCell bottomCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            _terrain.AddTriangle(bottom, left, right);
            Vector3 indices;
            indices.x = bottomCell.Index;
            indices.y = leftCell.Index;
            indices.z = rightCell.Index;
            _terrain.AddTriangleCellData(indices, weights1, weights2, weights3);
        }

        private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index)
        {
            _terrain.AddTriangle(center, edge.v1, edge.v2);
            _terrain.AddTriangle(center, edge.v2, edge.v3);
            _terrain.AddTriangle(center, edge.v3, edge.v4);
            _terrain.AddTriangle(center, edge.v4, edge.v5);

            Vector3 indices;
            indices.x = indices.y = indices.z = index;
            _terrain.AddTriangleCellData(indices, weights1);
            _terrain.AddTriangleCellData(indices, weights1);
            _terrain.AddTriangleCellData(indices, weights1);
            _terrain.AddTriangleCellData(indices, weights1);
        }

        private void TriangulateEdgeStrip(
            EdgeVertices e1, Color w1, float index1,
            EdgeVertices e2, Color w2, float index2
        )
        {
            _terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
            _terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
            _terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
            _terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);

            Vector3 indices;
            indices.x = indices.z = index1;
            indices.y = index2;
            _terrain.AddQuadCellData(indices, w1, w2);
            _terrain.AddQuadCellData(indices, w1, w2);
            _terrain.AddQuadCellData(indices, w1, w2);
            _terrain.AddQuadCellData(indices, w1, w2);
        }
    }
}
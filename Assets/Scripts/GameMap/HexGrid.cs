using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Component that represents an entire hexagon map.
    /// </summary>
    public class HexGrid : MonoBehaviour, IHexGrid
    {
        /// <summary>
        /// Amount of cells in the X dimension.
        /// </summary>
        [field: SerializeField] public int CellCountX { get; private set; }

        /// <summary>
        /// Amount of cells in the Z dimension.
        /// </summary>
        [field: SerializeField] public int CellCountZ { get; private set; }

        [SerializeField]
        HexCell cellPrefab;

        [SerializeField]
        Text cellLabelPrefab;

        [SerializeField]
        HexGridChunk chunkPrefab;

        [SerializeField]
        Texture2D noiseSource;

        [SerializeField]
        int seed;

        /// <summary>
        /// Whether there currently exists a path that should be displayed.
        /// </summary>
        public bool HasPath => currentPathExists;

        /// <summary>
        /// Whether east-west wrapping is enabled.
        /// </summary>
        public bool Wrapping { get; private set; }

        Transform[] columns;
        HexGridChunk[] chunks;
        HexCell[] cells;

        int chunkCountX, chunkCountZ;

        HexCellPriorityQueue searchFrontier;

        int searchFrontierPhase;

        HexCell currentPathFrom, currentPathTo;
        bool currentPathExists;

        int currentCenterColumnIndex = -1;

        HexCellShaderData cellShaderData;

        void Awake()
        {
            Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
            // CellCountX = 20;
            // CellCountZ = 15;
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            cellShaderData = gameObject.AddComponent<HexCellShaderData>();
            cellShaderData.Grid = this;
            CreateMap(CellCountX, CellCountZ, false);
        }

        /// <summary>
        /// Make a game object a child of a map column.
        /// </summary>
        /// <param name="child"><see cref="Transform"/> of the child game object.</param>
        /// <param name="columnIndex">Index of the parent column.</param>
        public void MakeChildOfColumn(Transform child, int columnIndex) =>
            child.SetParent(columns[columnIndex], false);

        /// <summary>
        /// Create a new map.
        /// </summary>
        /// <param name="x">X size of the map.</param>
        /// <param name="z">Z size of the map.</param>
        /// <param name="wrapping">Whether the map wraps east-west.</param>
        /// <returns>Whether the map was successfully created. It fails if the X or Z size
        /// is not a multiple of the respective chunk size.</returns>
        public bool CreateMap(int x, int z, bool wrapping)
        {
            if (
                x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
                z <= 0 || z % HexMetrics.chunkSizeZ != 0
            )
            {
                Debug.LogError("Unsupported map size.");
                return false;
            }

            ClearPath();
            if (columns != null)
            {
                for (int i = 0; i < columns.Length; i++)
                    Destroy(columns[i].gameObject);
            }

            CellCountX = x;
            CellCountZ = z;
            this.Wrapping = wrapping;
            currentCenterColumnIndex = -1;
            HexMetrics.wrapSize = wrapping ? CellCountX : 0;
            chunkCountX = CellCountX / HexMetrics.chunkSizeX;
            chunkCountZ = CellCountZ / HexMetrics.chunkSizeZ;
            cellShaderData.Initialize(CellCountX, CellCountZ);
            CreateChunks();
            CreateCells();
            return true;
        }

        void CreateChunks()
        {
            columns = new Transform[chunkCountX];
            for (int x = 0; x < chunkCountX; x++)
            {
                columns[x] = new GameObject("Column").transform;
                columns[x].SetParent(transform, false);
            }

            chunks = new HexGridChunk[chunkCountX * chunkCountZ];
            for (int z = 0, i = 0; z < chunkCountZ; z++)
            {
                for (int x = 0; x < chunkCountX; x++)
                {
                    HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                    chunk.transform.SetParent(columns[x], false);
                }
            }
        }

        void CreateCells()
        {
            cells = new HexCell[CellCountZ * CellCountX];

            for (int z = 0, i = 0; z < CellCountZ; z++)
            {
                for (int x = 0; x < CellCountX; x++)
                    CreateCell(x, z, i++);
            }
        }

        void OnEnable()
        {
            if (!HexMetrics.noiseSource)
            {
                HexMetrics.noiseSource = noiseSource;
                HexMetrics.InitializeHashGrid(seed);
                HexMetrics.wrapSize = Wrapping ? CellCountX : 0;
                ResetVisibility();
            }
        }

        /// <summary>
        /// Get a cell given a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray"><see cref="Ray"/> used to perform a raycast.</param>
        /// <returns>The hit cell, if any.</returns>
        public HexCell GetCell(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
                return GetCell(hit.point);
            return null;
        }

        /// <summary>
        /// Get the cell that contains a position.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>The cell containing the position, if it exists.</returns>
        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            return GetCell(coordinates);
        }

        /// <summary>
        /// Get the cell with specific <see cref="HexCoordinates"/>.
        /// </summary>
        /// <param name="coordinates"><see cref="HexCoordinates"/> of the cell.</param>
        /// <returns>The cell with the given coordinates, if it exists.</returns>
        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= CellCountZ)
                return null;
            int x = coordinates.X + z / 2;
            if (x < 0 || x >= CellCountX)
                return null;
            return cells[x + z * CellCountX];
        }

        /// <summary>
        /// Get the cell with specific offset coordinates.
        /// </summary>
        /// <param name="xOffset">X array offset coordinate.</param>
        /// <param name="zOffset">Z array offset coordinate.</param>
        /// <returns></returns>
        public HexCell GetCell(int xOffset, int zOffset) =>
            cells[xOffset + zOffset * CellCountX];

        /// <summary>
        /// Get the cell with a specific index.
        /// </summary>
        /// <param name="cellIndex">Cell index, which should be valid.</param>
        /// <returns>The indicated cell.</returns>
        public HexCell GetCell(int cellIndex) => cells[cellIndex];

        /// <summary>
        /// Control whether the map UI should be visible or hidden.
        /// </summary>
        /// <param name="visible">Whether the UI should be visibile.</param>
        public void ShowUI(bool visible)
        {
            for (int i = 0; i < chunks.Length; i++)
                chunks[i].ShowUI(visible);
        }

        void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerDiameter;
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);

            HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.localPosition = position;
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Index = i;
            cell.ColumnIndex = x / HexMetrics.chunkSizeX;
            cell.ShaderData = cellShaderData;

            if (Wrapping)
                cell.Explorable = z > 0 && z < CellCountZ - 1;
            else
                cell.Explorable =
                    x > 0 && z > 0 && x < CellCountX - 1 && z < CellCountZ - 1;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
                if (Wrapping && x == CellCountX - 1)
                    cell.SetNeighbor(HexDirection.E, cells[i - x]);
            }
            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - CellCountX]);
                    if (x > 0)
                        cell.SetNeighbor(HexDirection.SW, cells[i - CellCountX - 1]);
                    else if (Wrapping)
                        cell.SetNeighbor(HexDirection.SW, cells[i - 1]);
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - CellCountX]);
                    if (x < CellCountX - 1)
                        cell.SetNeighbor(HexDirection.SE, cells[i - CellCountX + 1]);
                    else if (Wrapping)
                    {
                        cell.SetNeighbor(
                            HexDirection.SE, cells[i - CellCountX * 2 + 1]
                        );
                    }
                }
            }

            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            cell.UIRect = label.rectTransform;

            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x / HexMetrics.chunkSizeX;
            int chunkZ = z / HexMetrics.chunkSizeZ;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

            int localX = x - chunkX * HexMetrics.chunkSizeX;
            int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }

        /// <summary>
        /// Save the map.
        /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> to use.</param>
        public void Save(BinaryWriter writer)
        {
            writer.Write(CellCountX);
            writer.Write(CellCountZ);
            writer.Write(Wrapping);

            for (int i = 0; i < cells.Length; i++)
                cells[i].Save(writer);
        }

        /// <summary>
        /// Load the map.
        /// </summary>
        /// <param name="reader"><see cref="BinaryReader"/> to use.</param>
        /// <param name="header">Header version.</param>
        public void Load(BinaryReader reader, int header)
        {
            ClearPath();
            int x = 20, z = 15;
            if (header >= 1)
            {
                x = reader.ReadInt32();
                z = reader.ReadInt32();
            }
            bool wrapping = header >= 5 ? reader.ReadBoolean() : false;
            if (x != CellCountX || z != CellCountZ || this.Wrapping != wrapping)
            {
                if (!CreateMap(x, z, wrapping))
                    return;
            }

            bool originalImmediateMode = cellShaderData.ImmediateMode;
            cellShaderData.ImmediateMode = true;

            for (int i = 0; i < cells.Length; i++)
                cells[i].Load(reader, header);
            for (int i = 0; i < chunks.Length; i++)
                chunks[i].Refresh();

            cellShaderData.ImmediateMode = originalImmediateMode;
        }

        /// <summary>
        /// Get a list of cells representing the currently visible path.
        /// </summary>
        /// <returns>The current path list, if a visible path exists.</returns>
        public List<HexCell> GetPath()
        {
            if (!currentPathExists)
                return null;
            List<HexCell> path = ListPool<HexCell>.Get();
            for (HexCell cell = currentPathTo; cell != currentPathFrom; cell = cell.PathFrom)
                path.Add(cell);
            path.Add(currentPathFrom);
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Clear the current path.
        /// </summary>
        public void ClearPath()
        {
            if (currentPathExists)
            {
                HexCell current = currentPathTo;
                while (current != currentPathFrom)
                {
                    current.SetLabel(null);
                    current.DisableHighlight();
                    current = current.PathFrom;
                }
                current.DisableHighlight();
                currentPathExists = false;
            }
            else if (currentPathFrom)
            {
                currentPathFrom.DisableHighlight();
                currentPathTo.DisableHighlight();
            }
            currentPathFrom = currentPathTo = null;
        }

        void ShowPath(int speed)
        {
            if (currentPathExists)
            {
                HexCell current = currentPathTo;
                while (current != currentPathFrom)
                {
                    int turn = (current.Distance - 1) / speed;
                    current.SetLabel(turn.ToString());
                    current.EnableHighlight(Color.white);
                    current = current.PathFrom;
                }
            }
            currentPathFrom.EnableHighlight(Color.blue);
            currentPathTo.EnableHighlight(Color.red);
        }

        /// <summary>
        /// Try to find a path.
        /// </summary>
        /// <param name="fromCell">Cell to start the search from.</param>
        /// <param name="toCell">Cell to find a path towards.</param>
        /// <param name="unit">Unit for which the path is.</param>
        public void FindPath(HexCell fromCell, HexCell toCell, IHexUnit unit)
        {
            ClearPath();
            currentPathFrom = fromCell;
            currentPathTo = toCell;
            currentPathExists = Search(fromCell, toCell, unit);
            ShowPath(unit.Speed);
        }

        bool Search(HexCell fromCell, HexCell toCell, IHexUnit unit)
        {
            int speed = unit.Speed;
            searchFrontierPhase += 2;
            if (searchFrontier == null)
                searchFrontier = new HexCellPriorityQueue();
            else
                searchFrontier.Clear();
            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            while (searchFrontier.Count > 0)
            {
                HexCell current = searchFrontier.Dequeue();
                current.SearchPhase += 1;

                if (current == toCell)
                    return true;
                int currentTurn = (current.Distance - 1) / speed;

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > searchFrontierPhase
                    )
                        continue;
                    if (!unit.IsValidDestination(neighbor))
                        continue;
                    int moveCost = unit.GetMoveCost(current, neighbor, d);
                    if (moveCost < 0)
                        continue;
                    int distance = current.Distance + moveCost;
                    int turn = (distance - 1) / speed;
                    if (turn > currentTurn)
                        distance = turn * speed + moveCost;
                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic =
                            neighbor.Coordinates.DistanceTo(toCell.Coordinates);
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Increase the visibility of all cells relative to a view cell.
        /// </summary>
        /// <param name="fromCell">Cell from which to start viewing.</param>
        /// <param name="range">Visibility range.</param>
        public void IncreaseVisibility(HexCell fromCell, int range)
        {
            List<HexCell> cells = GetVisibleCells(fromCell, range);
            for (int i = 0; i < cells.Count; i++)
                cells[i].IncreaseVisibility();
            ListPool<HexCell>.Add(cells);
        }

        /// <summary>
        /// Decrease the visibility of all cells relative to a view cell.
        /// </summary>
        /// <param name="fromCell">Cell from which to stop viewing.</param>
        /// <param name="range">Visibility range.</param>
        public void DecreaseVisibility(HexCell fromCell, int range)
        {
            List<HexCell> cells = GetVisibleCells(fromCell, range);
            for (int i = 0; i < cells.Count; i++)
                cells[i].DecreaseVisibility();
            ListPool<HexCell>.Add(cells);
        }

        /// <summary>
        /// Reset visibility of the entire map, viewing from all units.
        /// </summary>
        public void ResetVisibility()
        {
            for (int i = 0; i < cells.Length; i++)
                cells[i].ResetVisibility();
        }

        List<HexCell> GetVisibleCells(HexCell fromCell, int range)
        {
            List<HexCell> visibleCells = ListPool<HexCell>.Get();

            searchFrontierPhase += 2;
            if (searchFrontier == null)
                searchFrontier = new HexCellPriorityQueue();
            else
                searchFrontier.Clear();
            range += fromCell.ViewElevation;
            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            HexCoordinates fromCoordinates = fromCell.Coordinates;
            while (searchFrontier.Count > 0)
            {
                HexCell current = searchFrontier.Dequeue();
                current.SearchPhase += 1;
                visibleCells.Add(current);

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > searchFrontierPhase ||
                        !neighbor.Explorable
                    )
                        continue;

                    int distance = current.Distance + 1;
                    if (distance + neighbor.ViewElevation > range ||
                        distance > fromCoordinates.DistanceTo(neighbor.Coordinates)
                    )
                        continue;

                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.SearchHeuristic = 0;
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }
            return visibleCells;
        }

        /// <summary>
        /// Center the map given an X position, to facilitate east-west wrapping.
        /// </summary>
        /// <param name="xPosition">X position.</param>
        public void CenterMap(float xPosition)
        {
            int centerColumnIndex = (int)
                (xPosition / (HexMetrics.innerDiameter * HexMetrics.chunkSizeX));

            if (centerColumnIndex == currentCenterColumnIndex)
                return;

            currentCenterColumnIndex = centerColumnIndex;

            int minColumnIndex = centerColumnIndex - chunkCountX / 2;
            int maxColumnIndex = centerColumnIndex + chunkCountX / 2;

            Vector3 position;
            position.y = position.z = 0f;
            for (int i = 0; i < columns.Length; i++)
            {
                if (i < minColumnIndex)
                    position.x = chunkCountX *
                        (HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
                else if (i > maxColumnIndex)
                    position.x = chunkCountX *
                        -(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
                else
                    position.x = 0f;
                columns[i].localPosition = position;
            }
        }
    }
}

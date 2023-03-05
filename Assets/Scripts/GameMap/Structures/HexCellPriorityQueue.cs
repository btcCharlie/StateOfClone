using System.Collections.Generic;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Priority queue to store hex cells for the pathfinding algorithm.
    /// </summary>
    public class HexCellPriorityQueue
    {

        List<IHexCell> list = new();

        int count = 0;
        int minimum = int.MaxValue;

        /// <summary>
        /// How many cells are in the queue.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Add a cell to the queue.
        /// </summary>
        /// <param name="cell">Cell to add.</param>
        public void Enqueue(IHexCell cell)
        {
            count += 1;
            int priority = cell.SearchPriority;
            if (priority < minimum)
                minimum = priority;
            while (priority >= list.Count)
                list.Add(null);
            cell.NextWithSamePriority = list[priority];
            list[priority] = cell;
        }

        /// <summary>
        /// Remove a cell from the queue.
        /// </summary>
        /// <returns>The cell with the highest priority.</returns>
        public IHexCell Dequeue()
        {
            count -= 1;
            for (; minimum < list.Count; minimum++)
            {
                IHexCell cell = list[minimum];
                if (cell != null)
                {
                    list[minimum] = cell.NextWithSamePriority;
                    return cell;
                }
            }
            return null;
        }

        /// <summary>
        /// Apply the currently priority of a cell that was previously enqueued.
        /// </summary>
        /// <param name="cell">Cell to update</param>
        /// <param name="oldPriority">Priority of the cell before it was changed.</param>
        public void Change(IHexCell cell, int oldPriority)
        {
            IHexCell current = list[oldPriority];
            IHexCell next = current.NextWithSamePriority;
            if (current == cell)
                list[oldPriority] = next;
            else
            {
                while (next != cell)
                {
                    current = next;
                    next = current.NextWithSamePriority;
                }
                current.NextWithSamePriority = cell.NextWithSamePriority;
            }
            Enqueue(cell);
            count -= 1;
        }

        /// <summary>
        /// Clear the queue.
        /// </summary>
        public void Clear()
        {
            list.Clear();
            count = 0;
            minimum = int.MaxValue;
        }
    }
}

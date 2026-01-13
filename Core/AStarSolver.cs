using System;
using System.Collections.Generic;
using Core.Policies;

namespace Core
{
    /// <summary>
    /// Implements the A* pathfinding algorithm using per-cell scores and precomputed neighbours.
    /// </summary>
    public class AStarSolver
    {
        private Cell end = null!;
        private readonly IHeuristic heuristic = new ChebyshevHeuristic();

        private readonly List<Cell> openSet = new();
        private readonly List<Cell> closedSet = new();
        private readonly List<Cell> currentPath = new();

        public Cell? Current { get; private set; }
        public List<Cell> OpenSet => openSet;
        public List<Cell> ClosedSet => closedSet;
        public List<Cell> CurrentPath => currentPath;

        /// <summary>
        /// Initialises the solver with the provided grid and start/end cells.
        /// </summary>
        /// <param name="grid">The grid containing cells and neighbours.</param>
        /// <param name="start">The start cell.</param>
        /// <param name="end">The goal cell.</param>
        public void Initialize(Grid grid, Cell start, Cell end)
        {
            this.end = end;

            openSet.Clear();
            closedSet.Clear();
            currentPath.Clear();
            Current = null;

            // Reset scores on all cells
            for (int x = 0; x < grid.Columns; x++)
            {
                for (int y = 0; y < grid.Rows; y++)
                {
                    var c = grid.GetCell(x, y);
                    c.f = c.g = c.h = 0;
                    c.parent = null;
                }
            }

            start.g = 0;
            start.h = heuristic.Estimate(start, end);
            start.f = start.g + start.h;
            openSet.Add(start);
        }
        
        /// <summary>
        /// Advances the A* search by one iteration.
        /// </summary>
        /// <returns>True if the search has finished (path found or no solution); otherwise false.</returns>
        public bool Step()
        {
            if (openSet.Count <= 0)
            {
                Current = null;
                Console.WriteLine("No solution found.");
                return true;
            }

            // Choose node with lowest f
            int lowestIndex = 0;
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].f < openSet[lowestIndex].f)
                {
                    lowestIndex = i;
                }
            }

            var current = openSet[lowestIndex];
            Current = current;

            if (current == end)
            {
                ReconstructPath(current);
                Console.WriteLine("Solution found!");
                return true;
            }

            openSet.RemoveAt(lowestIndex);
            closedSet.Add(current);

            var neighbours = current.neighbours;
            for (int i = 0; i < neighbours.Count; i++)
            {
                var neighbour = neighbours[i];
                if (closedSet.Contains(neighbour) || neighbour.isWall)
                {
                    continue;
                }

                int tentativeG = current.g + 1;
                bool newPath = false;

                if (openSet.Contains(neighbour))
                {
                    if (tentativeG < neighbour.g)
                    {
                        neighbour.g = tentativeG;
                        newPath = true;
                    }
                }
                else
                {
                    neighbour.g = tentativeG;
                    openSet.Add(neighbour);
                    newPath = true;
                }

                if (newPath)
                {
                    neighbour.h = heuristic.Estimate(neighbour, end);
                    neighbour.f = neighbour.g + neighbour.h;
                    neighbour.parent = current;
                }
            }

            // Update current path preview
            ReconstructPath(current);
            return false;
        }

        private void ReconstructPath(Cell node)
        {
            currentPath.Clear();
            var t = node;
            currentPath.Add(t);
            while (t.parent != null)
            {
                currentPath.Add(t.parent);
                t = t.parent;
            }
        }
    }
}

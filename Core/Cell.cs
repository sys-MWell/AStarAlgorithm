
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Represents a single grid cell used by the A* algorithm.
    /// </summary>
    public class Cell(int px, int py)
    {
        // Grid position
        public int x = px;
        public int y = py;

        // A* scores stored on the cell (original style)
        public int f = 0;
        public int g = 0;
        public int h = 0;
        public Cell? parent = null;

        // Precomputed neighbours
        public List<Cell> neighbours = new List<Cell>();

        // Random wall flag (kept to match original)
        private static readonly Random rng = new Random();
        public bool isWall = rng.NextDouble() < 0.3;
    }
}
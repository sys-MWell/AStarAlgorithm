using System.Drawing;
using Core;

namespace AStarAlgorithm.UI
{
    /// <summary>
    /// Renders the grid, open/closed sets, and current path to a Graphics surface.
    /// </summary>
    public class GridRenderer
    {
        /// <summary>
        /// Draws the current state of the A* visualization.
        /// </summary>
        /// <param name="g">Graphics context to draw on.</param>
        /// <param name="clientBounds">Client bounds used to compute cell sizes.</param>
        /// <param name="grid">The grid to visualize.</param>
        /// <param name="closedSet">Closed set of explored cells.</param>
        /// <param name="openSet">Open set of frontier cells.</param>
        /// <param name="path">Current reconstructed path from the solver.</param>
        public void Render(Graphics g, Rectangle clientBounds, Grid grid,
            List<Cell> closedSet,
            List<Cell> openSet,
            List<Cell> path)
        {
            int padding = 20;
            int drawW = clientBounds.Width - padding * 2;
            int drawH = clientBounds.Height - padding * 2;
            if (drawW <= 0 || drawH <= 0)
            {
                return;
            }

            float cellW = (float)drawW / grid.Columns;
            float cellH = (float)drawH / grid.Rows;

            // Draw grid
            for (int x = 0; x < grid.Columns; x++)
            {
                for (int y = 0; y < grid.Rows; y++)
                {
                    var cell = grid.GetCell(x, y);
                    var fillColor = cell.isWall ? Color.Black : Color.White;
                    float rx = padding + x * cellW;
                    float ry = padding + y * cellH;
                    using (var brush = new SolidBrush(fillColor))
                    using (var pen = new Pen(Color.Black, 1))
                    {
                        g.FillRectangle(brush, rx, ry, cellW, cellH);
                        g.DrawRectangle(pen, rx, ry, cellW, cellH);
                    }
                }
            }

            // Closed set in red
            foreach (var c in closedSet)
            {
                FillCell(g, c, grid, padding, cellW, cellH, Color.Red);
            }
            // Open set in green
            foreach (var c in openSet)
            {
                FillCell(g, c, grid, padding, cellW, cellH, Color.Green);
            }
            // Path in blue
            foreach (var c in path)
            {
                FillCell(g, c, grid, padding, cellW, cellH, Color.Blue);
            }

            // Draw path lines
            for (int i = 0; i + 1 < path.Count; i++)
            {
                using var pen = new Pen(Color.White, 2);
                float x1 = padding + path[i].x * cellW + cellW / 2;
                float y1 = padding + path[i].y * cellH + cellH / 2;
                float x2 = padding + path[i + 1].x * cellW + cellW / 2;
                float y2 = padding + path[i + 1].y * cellH + cellH / 2;
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private static void FillCell(Graphics g, Cell c, Grid grid, int padding, float cellW, float cellH, Color color)
        {
            float rx = padding + c.x * cellW;
            float ry = padding + c.y * cellH;
            using var brush = new SolidBrush(color);
            using var pen = new Pen(Color.Black, 1);
            g.FillRectangle(brush, rx, ry, cellW, cellH);
            g.DrawRectangle(pen, rx, ry, cellW, cellH);
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using Core;
using AStarAlgorithm.UI;

namespace AStarAlgorithm
{
    /// <summary>
    /// Coordinates the grid, solver, controller, and renderer for the A* visualization.
    /// </summary>
    public class AlgorithmLogic
    {
        private readonly int columns = 80;
        private readonly int rows = 80;
        private readonly int width = 1000;
        private readonly int height = 800;

        private readonly Form parent;
        private readonly Grid grid;
        private readonly AStarSolver solver;
        private readonly GridRenderer renderer;
        private readonly AlgorithmController controller;

        /// <summary>
        /// Creates and initializes the A* visualization bound to the given parent form.
        /// </summary>
        /// <param name="parent">The host form used for rendering and invalidation.</param>
        public AlgorithmLogic(Form parent)
        {
            this.parent = parent;

            grid = new Grid(columns, rows);

            // Ensure start/end are not walls
            var start = grid.GetCell(0, 0);
            var end = grid.GetCell(columns - 1, rows - 1);
            start.isWall = false;
            end.isWall = false;

            solver = new AStarSolver();
            renderer = new GridRenderer();

            grid.addNeighbours();
            solver.Initialize(grid, start, end);

            controller = new AlgorithmController(solver, intervalMs: 100, stepsPerTick: 5);
            controller.Updated += (_, __) => parent.Invalidate();

            parent.BackColor = Color.White;
            parent.ClientSize = new Size(width, height);
            parent.Paint += Parent_Paint;
            parent.Resize += (_, __) => parent.Invalidate();

            controller.Start();
            parent.Invalidate();
        }

        private void Parent_Paint(object? sender, PaintEventArgs e)
        {
            var bounds = new Rectangle(Point.Empty, parent.ClientSize);
            renderer.Render(e.Graphics, bounds, grid, solver.ClosedSet, solver.OpenSet, solver.CurrentPath);
        }
    }
}
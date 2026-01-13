using System.Collections.Generic;

namespace Core
{
	/// <summary>
	/// Represents a 2D grid of cells and provides neighbour generation and utility checks.
	/// </summary>
	public class Grid
	{
		private readonly Cell[,] cells;
		public int Columns { get; }
		public int Rows { get; }

		/// <summary>
		/// Creates a grid with the specified dimensions and allocates its cells.
		/// </summary>
		/// <param name="columns">Number of columns.</param>
		/// <param name="rows">Number of rows.</param>
		public Grid(int columns, int rows)
		{
			Columns = columns;
			Rows = rows;
			cells = new Cell[columns, rows];
			for (int x = 0; x < columns; x++)
			{
				for (int y = 0; y < rows; y++)
				{
					cells[x, y] = new Cell(x, y);
				}
			}
		}

		/// <summary>
		/// Gets the cell at the specified coordinates.
		/// </summary>
		/// <param name="x">Column index.</param>
		/// <param name="y">Row index.</param>
		/// <returns>The cell instance at the given coordinates.</returns>
		public Cell GetCell(int x, int y) => cells[x, y];

		/// <summary>
		/// Determines whether the specified coordinates are within the grid bounds.
		/// </summary>
		/// <param name="x">Column index.</param>
		/// <param name="y">Row index.</param>
		/// <returns>True if the coordinates are inside the grid; otherwise false.</returns>
		public bool IsInBounds(int x, int y) => x >= 0 && x < Columns && y >= 0 && y < Rows;

		/// <summary>
		/// Indicates whether the specified cell is in bounds and not a wall.
		/// </summary>
		/// <param name="x">Column index.</param>
		/// <param name="y">Row index.</param>
		/// <returns>True if the cell exists and is not a wall; otherwise false.</returns>
		public bool IsFree(int x, int y)
		{
			if (!IsInBounds(x, y))
			{
				return false;
			}
			return !cells[x, y].isWall;
		}

		private bool isFree(int x, int y) => IsFree(x, y);

		/// <summary>
		/// Populates each cell's neighbour list with orthogonal and diagonal neighbours.
		/// Diagonals are added only when they do not cut corners (both adjacent orthogonals free).
		/// </summary>
		public void addNeighbours()
		{
			for (int x = 0; x < Columns; x++)
			{
				for (int y = 0; y < Rows; y++)
				{
					var current = cells[x, y];
					current.neighbours.Clear();

					// Orthogonals
					if (x < Columns - 1)
					{
						current.neighbours.Add(cells[x + 1, y]);
					}
					if (x > 0)
					{
						current.neighbours.Add(cells[x - 1, y]);
					}
					if (y < Rows - 1)
					{
						current.neighbours.Add(cells[x, y + 1]);
					}
					if (y > 0)
					{
						current.neighbours.Add(cells[x, y - 1]);
					}

					// Diagonals (no corner cutting): only add if both side-adjacent cells are free
					if (x > 0 && y > 0 && isFree(x - 1, y) && isFree(x, y - 1))
					{
						current.neighbours.Add(cells[x - 1, y - 1]);
					}
					if (x < Columns - 1 && y > 0 && isFree(x + 1, y) && isFree(x, y - 1))
					{
						current.neighbours.Add(cells[x + 1, y - 1]);
					}
					if (x > 0 && y < Rows - 1 && isFree(x - 1, y) && isFree(x, y + 1))
					{
						current.neighbours.Add(cells[x - 1, y + 1]);
					}
					if (x < Columns - 1 && y < Rows - 1 && isFree(x + 1, y) && isFree(x, y + 1))
					{
						current.neighbours.Add(cells[x + 1, y + 1]);
					}
				}
			}
		}
	}
}

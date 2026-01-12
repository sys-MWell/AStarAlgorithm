namespace AStarAlgorithm
{
    public class AlgorithmLogic
    {
        private int columns = 80;
        private int rows = 80;
        private int width = 1000;
        private int height = 800;
        private Spot[][] grid;

        private Spot start = null!;
        private Spot end = null!;

        private List<Spot> openSet = new List<Spot>();
        private List<Spot> closedSet = new List<Spot>();

        // Reference to the owning WinForms Form that hosts the algorithm’s UI.
        private Form parent;
        private System.Windows.Forms.Timer? timer;
        private Spot? current;

        private List<Spot> path = new List<Spot>();

        private const int stepsPerTick = 5; // Run multiple steps each tick for speed

        /// <summary>
        /// Initialises the AlgorithmLogic, allocates the grid, and configures the host Form for rendering:
        /// sets the Form's background to white, applies the algorithm's client size, subscribes to Paint and Resize
        /// events, and issues an initial Invalidate for a first draw.
        /// </summary>
        /// <param name="parent">
        /// The host Form used as the drawing surface and event source; it will have its BackColor, ClientSize,
        /// Paint, and Resize behavior configured by this constructor.
        /// </param>
        public AlgorithmLogic(Form parent)
        {
            this.parent = parent;
            grid = new Spot[columns][];
            setup();
            parent.BackColor = Color.White;
            parent.ClientSize = new Size(width, height);
            parent.Paint += Parent_Paint;
            parent.Resize += (_, __) => parent.Invalidate();
            // Drive algorithm via timer ticks
            // 16 ms ≈ 1000ms / 60 ≈ 60 frames per second (60 FPS)
            timer = new System.Windows.Forms.Timer { Interval = 100 };
            timer.Tick += Timer_Tick;
            timer.Start();
            parent.Invalidate();
        }

        private class Spot(int px, int py)
        {
            // Grid position
            // X coordinate
            public int x = px;
            // Y coordinate
            public int y = py;
            // Dikstra's and A* variables - f(n) = g(n) + h(n)
            // f: total score to decide next move. It’s the node’s “priority” A* uses. Lower is better. Calculated as f(n)=g(n)+h(n).
            public int f = 0;
            // g: cost so far from start. Think “how much effort we’ve already spent to reach this square.”
            public int g = 0;
            // h: guessed cost to the goal. A quick estimate of “how far we still have to go.” - Using manhattan distance
            public int h = 0;
            // Parent spot in path
            public Spot? parent = null;
            // Neighbouring spots
            public List<Spot> neighbours = new List<Spot>();
            // Current spot in consideration
            public Spot current = null!;

            // Randomly decide if this spot is a wall (30% chance)
            private static readonly Random rng = new Random();
            public bool isWall = rng.NextDouble() < 0.3;



            /// <summary>
            /// Renders this Spot onto the provided Graphics context, using the specified cell dimensions, padding
            /// and colour. If the Spot is marked as a wall, it will be drawn in black regardless of the provided colour.
            /// </summary>
            public void DrawRectangle(Graphics g, float cellW, float cellH, int padding, Color colour)
            {
                var fillColor = colour.IsEmpty ? Color.White : colour;

                // Override fill color if this spot is a wall
                if (this.isWall)
                {
                    fillColor = Color.Black;
                }

                // Draw rectangle
                float x1 = padding + x * cellW; 
                float y1 = padding + y * cellH;
                using (var pen = new Pen(Color.Black, 1))
                {
                    // Fill rectangle
                    using (var brush = new SolidBrush(fillColor))
                    {
                        g.FillRectangle(brush, x1, y1, cellW, cellH);
                    }
                    g.DrawRectangle(pen, x1, y1, cellW, cellH);
                }
            }
        }

        private void addNeighbours()
        {
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    // Orthogonals
                    // Right
                    if (x < columns - 1)
                    {
                        grid[x][y].neighbours.Add(grid[x + 1][y]);
                    }
                    // Left
                    if (x > 0)
                    {
                        grid[x][y].neighbours.Add(grid[x - 1][y]);
                    }
                    // Down
                    if (y < rows - 1)
                    {
                        grid[x][y].neighbours.Add(grid[x][y + 1]);
                    }
                    // Up
                    if (y > 0)
                    {
                        grid[x][y].neighbours.Add(grid[x][y - 1]);
                    }

                    // Diagonals (no corner cutting): only add if both side-adjacent cells are free
                    // Top-Left: requires free(x-1, y) and free(x, y-1)
                    if (x > 0 && y > 0 && isFree(x - 1, y) && isFree(x, y - 1))
                    {
                        grid[x][y].neighbours.Add(grid[x - 1][y - 1]);
                    }
                    // Top-Right: requires free(x+1, y) and free(x, y-1)
                    if (x < columns - 1 && y > 0 && isFree(x + 1, y) && isFree(x, y - 1))
                    {
                        grid[x][y].neighbours.Add(grid[x + 1][y - 1]);
                    }
                    // Bottom-Left: requires free(x-1, y) and free(x, y+1)
                    if (x > 0 && y < rows - 1 && isFree(x - 1, y) && isFree(x, y + 1))
                    {
                        grid[x][y].neighbours.Add(grid[x - 1][y + 1]);
                    }
                    // Bottom-Right: requires free(x+1, y) and free(x, y+1)
                    if (x < columns - 1 && y < rows - 1 && isFree(x + 1, y) && isFree(x, y + 1))
                    {
                        grid[x][y].neighbours.Add(grid[x + 1][y + 1]);
                    }
                }
            }
        }

        private int heuristic(Spot a, Spot b)
        {
            // Using Chebyshev distance - suitable for a grid with orthogonal and diagonal movement
            return Math.Max(Math.Abs(a.x-b.x), Math.Abs(a.y-b.y));
        }

        private void setup()
        {
            Console.WriteLine ("A*");

            // Making a 2D array
            // Allocate spots in grid
            for (var x = 0; x < columns; x++)
            {
            grid[x] = new Spot[rows];
            }

            // Initialise spots
            // Populate grid with Spot instances
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    grid[x][y] = new Spot(x, y);
                }
            } 

            // Populate neighbour lists for all spots
            addNeighbours();

            // Define start and end
            start = grid[0][0];
            end = grid[columns - 1][rows - 1];
            // Ensure start and end are not walls
            start.isWall = false;
            end.isWall = false;

            // Initialise start node costs
            start.g = 0;
            start.h = heuristic(start, end);
            start.f = start.g + start.h;

            openSet.Add(start);
        }

        private void Parent_Paint(object? sender, PaintEventArgs e)
        {
            // Draw the current state of the algorithm
            Draw(e.Graphics);
        }

        private void Draw(Graphics g)
        {
            int padding = 20;
            int drawW = parent.ClientSize.Width - padding * 2;
            int drawH = parent.ClientSize.Height - padding * 2;
            if (drawW <= 0 || drawH <= 0) return;

            float cellW = (float)drawW / columns;
            float cellH = (float)drawH / rows;

            // Draw-only: render current state
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    grid[x][y].DrawRectangle(g, cellW, cellH, padding, Color.White);
                }
            }

            // Highlight closed set in red
            for (var i = 0; i < closedSet.Count; i++)
            {
                closedSet[i].DrawRectangle(g, cellW, cellH, padding, Color.Red);
            }
            // Highlight open set in green
            for (var i = 0; i < openSet.Count; i++)
            {
                openSet[i].DrawRectangle(g, cellW, cellH, padding, Color.Green);
            }
            // Highlight path in blue
            for (var i = 0; i < path.Count; i++)
            {
                path[i].DrawRectangle(g, cellW, cellH, padding, Color.Blue);
            }

        
            for (var i = 0; i < path.Count; i++)
            {
                Pen pen = new Pen(Color.White, 2);
                float x1 = padding + path[i].x * cellW + cellW / 2;
                float y1 = padding + path[i].y * cellH + cellH / 2;
                if (i + 1 < path.Count)
                {
                    float x2 = padding + path[i + 1].x * cellW + cellW / 2;
                    float y2 = padding + path[i + 1].y * cellH + cellH / 2;
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }

            // For debugging: inspect grid in debugger
            var debug = grid;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Perform multiple steps per tick for speed
            // This allows the algorithm to progress faster while still providing visual updates.
            // Required for WinForms, stops finish refresh and screen going blank.
            for (int i = 0; i < stepsPerTick; i++)
            {
                Step();
                if (timer == null || !timer.Enabled) break; // stop if finished
            }
            parent.Invalidate();
        }

        private void Step()
        {
            // Check if there are no more nodes to evaluate
            if (openSet.Count <= 0)
            {
                timer?.Stop();
                Console.WriteLine("No solution");
                return;
            }

            // Choose node with lowest f
            var lowestIndex = 0;
            for (var i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].f < openSet[lowestIndex].f)
                {
                    lowestIndex = i;
                }
            }

            // Current node being evaluated
            current = openSet[lowestIndex];
            // Check if end reached
            if (current == end)
            {
                timer?.Stop();
                Console.WriteLine("Solution found");
                // Reconstruct final path
                path = new List<Spot>();
                var temp = current;
                path.Add(temp);
                while (temp.parent != null)
                {
                    path.Add(temp.parent);
                    temp = temp.parent;
                }
                return;
            }

            // Move current from open to closed set
            openSet.Remove(current);
            closedSet.Add(current);

            // Evaluate neighbours of current node
            var neighbours = current.neighbours;
            for (int i = 0; i < neighbours.Count; i++)
            {
                var neighbour = neighbours[i];
                // Valid next spot - not in closed set, not a wall, and diagonal move does not cut corners
                if (!closedSet.Contains(neighbour) && !neighbour.isWall && isDiagonalAllowed(current, neighbour))
                {
                    // Tentative g score
                    var tempG = current.g + 1;

                    var newPath = false;
                    // If neighbour already in open set, check if this path is better
                    if (openSet.Contains(neighbour))
                    {
                        if (tempG < neighbour.g)
                        {
                            neighbour.g = tempG;
                            newPath = true;
                        }
                    }
                    // Not in open set - add it
                    else
                    {
                        neighbour.g = tempG;
                        openSet.Add(neighbour);
                        newPath = true;
                    }

                    // Can be altered to use different heuristics
                    if (newPath)
                    {
                        neighbour.h = heuristic(neighbour, end);
                        neighbour.f = neighbour.g + neighbour.h;
                        neighbour.parent = current;   
                    }
                }
            }

            // Update path preview from current back to start
            path = new List<Spot>();
            var t = current;
            path.Add(t);
            while (t.parent != null)
            {
                path.Add(t.parent);
                t = t.parent;
            }
        }

        // Returns true if the grid cell at (x, y) exists and is not a wall
        private bool isFree(int x, int y)
        {
            if (x < 0 || x >= columns || y < 0 || y >= rows) return false;
            return !grid[x][y].isWall;
        }

        // Allow diagonal move only if it does not cut corners
        private bool isDiagonalAllowed(Spot from, Spot to)
        {
            int dx = to.x - from.x;
            int dy = to.y - from.y;

            // Orthogonal moves are always fine (walls are checked elsewhere)
            if (dx == 0 || dy == 0) return true;

            // For diagonals, ensure both adjacent orthogonal cells are free
            int sx = Math.Sign(dx);
            int sy = Math.Sign(dy);
            return isFree(from.x + sx, from.y) && isFree(from.x, from.y + sy);
        }
    }
}
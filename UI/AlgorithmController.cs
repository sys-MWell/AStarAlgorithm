using System;
using System.Windows.Forms;
using Core;

namespace AStarAlgorithm.UI
{
    /// <summary>
    /// Drives the A* solver using a WinForms timer and publishes update events.
    /// </summary>
    public class AlgorithmController : IDisposable
    {
        private readonly AStarSolver solver;
        private readonly System.Windows.Forms.Timer timer;
        private readonly int stepsPerTick;

        /// <summary>
        /// Occurs after each timer tick when the solver state updates.
        /// </summary>
        public event EventHandler? Updated;

        /// <summary>
        /// Creates a controller for the given solver with a timer interval and step batch size.
        /// </summary>
        /// <param name="solver">The A* solver to drive.</param>
        /// <param name="intervalMs">Timer interval in milliseconds.</param>
        /// <param name="stepsPerTick">Number of steps to execute each tick.</param>
        public AlgorithmController(AStarSolver solver, int intervalMs = 100, int stepsPerTick = 5)
        {
            this.solver = solver;
            this.stepsPerTick = stepsPerTick;
            timer = new System.Windows.Forms.Timer { Interval = intervalMs };
            timer.Tick += OnTick;
        }

        private void OnTick(object? sender, EventArgs e)
        {
            for (int i = 0; i < stepsPerTick; i++)
            {
                var finished = solver.Step();
                if (finished) { Stop(); break; }
            }
            Updated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Starts the timer-driven updates.
        /// </summary>
        public void Start() { timer.Start(); }
        /// <summary>
        /// Stops the timer-driven updates.
        /// </summary>
        public void Stop() { timer.Stop(); }

        /// <summary>
        /// Disposes internal resources.
        /// </summary>
        public void Dispose()
        {
            timer.Dispose();
        }
    }
}

using System;

namespace Core.Policies
{
    /// <summary>
    /// Strategy interface for estimating the distance (remaining cost) between two cells.
    /// </summary>
    public interface IHeuristic
    {
        /// <summary>
        /// Estimates the remaining cost from <paramref name="a"/> to <paramref name="b"/>.
        /// </summary>
        /// <param name="a">Start cell.</param>
        /// <param name="b">Goal cell.</param>
        /// <returns>An integer estimate of the remaining cost.</returns>
        int Estimate(Cell a, Cell b);
    }

    /// <summary>
    /// Chebyshev distance heuristic suitable for 8-directional movement on a grid.
    /// </summary>
    public sealed class ChebyshevHeuristic : IHeuristic
    {
        /// <summary>
        /// Returns the Chebyshev distance between two cells.
        /// </summary>
        /// <param name="a">Start cell.</param>
        /// <param name="b">Goal cell.</param>
        /// <returns>Max of absolute x/y differences.</returns>
        public int Estimate(Cell a, Cell b)
        {
            return Math.Max(Math.Abs(a.x - b.x), Math.Abs(a.y - b.y));
        }
    }
}

using AoC.Helpers.Models;

namespace AoC.Helpers.Helpers;

public static class PathFindingHelper
{
    /// <summary>
    /// A generic Breadth-First Search (BFS) to find the shortest path distance.
    /// </summary>
    /// <typeparam name="T">The type of the node (e.g., (int r, int c) or a custom Class).</typeparam>
    /// <param name="start">The starting node.</param>
    /// <param name="getNeighbors">Function that returns valid neighbors for a given node.</param>
    /// <param name="isGoal">Function that returns true if the node is the target.</param>
    /// <returns>The number of steps to reach the goal, or -1 if unreachable.</returns>
    public static int BfsDistance<T>(
        T start,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> isGoal) where T : notnull
    {
        var queue = new Queue<(T Node, int Distance)>();
        var visited = new HashSet<T>();

        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            if (isGoal(current))
            {
                return dist;
            }

            foreach (var neighbor in getNeighbors(current))
            {
                if (visited.Add(neighbor))
                {
                    queue.Enqueue((neighbor, dist + 1));
                }
            }
        }

        // Path not found
        return -1;
    }

    public static long ReverseHalvingMinCost(
        IReadOnlyList<int> startCounters,
        IReadOnlyList<int[]> buttons,
        long impossibleCost = long.MaxValue / 4)
    {
        if (startCounters.Count == 0)
        {
            return 0;
        }

        var n = startCounters.Count;
        var buttonCount = buttons.Count;

        // Can't solve anyway
        if (buttonCount == 0)
        {
            return startCounters.All(x => x == 0) ? 0 : impossibleCost;
        }

        // Button higher than mask.
        if (buttonCount > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(buttons), "Too many buttons for subset enumeration (2^N).");
        }

        // Button indexes didn't align with indexes of counter.
        for (var b = 0; b < buttonCount; b++)
        {
            foreach (var idx in buttons[b])
            {
                if ((uint)idx >= (uint)n)
                {
                    throw new ArgumentOutOfRangeException(nameof(buttons),
                        $"Button {b} references index {idx} but counters length is {n}.");
                }
            }
        }
        
        // Which numbers are odd
        bool[] TargetParity(int[] state) => state.Select(x => (x & 1) == 1).ToArray();

        // It must do odd amount of times, so after the button presses all should be even.
        bool SameParity(bool[] a, bool[] b)
        {
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        Func<StateKey, long> solve = null!;
        solve = Memoizer.Memoize<StateKey, long>(key =>
        {
            // Current state
            var state = key.Values;
            
            // Cost from here is 0 (no more presses needed).
            if (state.All(x => x == 0))
            {
                return 0;
            }

            // Any negative numbers can't be possible in this case
            if (state.Any(x => x < 0))
            {
                return impossibleCost;
            }

            // Which counters are odd
            var targetParity = TargetParity(state);

            // Tracks the best (lowest) total cost found so far for this. state
            var best = impossibleCost;
            var subsetCount = 1 << buttonCount;

            // Try all ways to press the buttons
            // subset is n^2 buttons
            for (var mask = 0; mask < subsetCount; mask++)
            {
                // Amoount of presses this iteration
                var presses = 0;
                var parity = new bool[n];
                // How much should the counter decrease
                var decreaseCounter = new int[n];

                for (var b = 0; b < buttonCount; b++)
                {
                    // If the button is not set by the mask, it is not pressed.
                    if (((mask >> b) & 1) == 0)
                    {
                        continue;
                    }

                    presses++;

                    // press the specific buttons
                    foreach (var idx in buttons[b])
                    {
                        decreaseCounter[idx]++;
                        parity[idx] = !parity[idx];
                    }
                }

                // Can't devide by 2 skip
                if (!SameParity(parity, targetParity))
                {
                    continue;
                }

                // Decrease counters.
                var next = new int[n];
                var ok = true;

                for (var i = 0; i < n; i++)
                {
                    var v = state[i] - decreaseCounter[i];
                    // Button gets smaller than 0, break invalid.
                    if (v < 0)
                    {
                        ok = false;
                        break;
                    }

                    // Devide all buttons by 2
                    next[i] = v / 2;
                }

                // Target below 0, break.
                if (!ok)
                {
                    continue;
                }

                // Call again.
                var recursiveSolve = solve(new StateKey(next));
                // Target higher than impossible cost, no need.
                if (recursiveSolve >= impossibleCost)
                {
                    continue;
                }

                // Ammount pressed during iteration is pressed + 2 * recursive value (since we devided it by 2).
                var total = presses + 2L * recursiveSolve;
                // Total lower than best, Total is new best. (target lowest button presses.
                if (total < best)
                {
                    best = total;
                }
            }

            return best;
        });

        return solve(new StateKey(startCounters.ToArray()));
    }
}
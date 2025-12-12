namespace AoC.Helpers.Helpers;

/// <summary>
/// Extension methods for working with 2D arrays (T[,]).
/// Complements arrays created by TextInputHelper.
/// </summary>
public static class DimensionalArrayHelper
{
    /// <summary>
    /// Checks if the given coordinates are valid within the grid boundaries.
    /// </summary>
    private static bool IsInBounds<T>(this T[,] grid, int row, int col)
    {
        return row >= 0 && row < grid.GetLength(0) &&
               col >= 0 && col < grid.GetLength(1);
    }

    /// <summary>
    /// Gets the valid neighboring cells for a specific coordinate.
    /// </summary>
    /// <param name="grid">The source 2D array.</param>
    /// <param name="row">The row index.</param>
    /// <param name="col">The column index.</param>
    /// <param name="includeDiagonals">If true, includes the 4 diagonal neighbors (Moore neighborhood). Defaults to false (Von Neumann neighborhood).</param>
    /// <returns>An enumerable of tuples containing the coordinate (Row, Col) and the Value.</returns>
    public static IEnumerable<(int Row, int Col, T Value)> GetNeighbors<T>(this T[,] grid, int row, int col,
        bool includeDiagonals = false)
    {
        var directions = new List<(int dr, int dc)>
        {
            (-1, 0), // Up
            (1, 0), // Down
            (0, -1), // Left
            (0, 1) // Right
        };

        if (includeDiagonals)
        {
            directions.AddRange(new[]
            {
                (-1, -1), // Top-Left
                (-1, 1), // Top-Right
                (1, -1), // Bottom-Left
                (1, 1) // Bottom-Right
            });
        }

        foreach (var (dr, dc) in directions)
        {
            var newRow = row + dr;
            var newCol = col + dc;

            if (grid.IsInBounds(newRow, newCol))
            {
                yield return (newRow, newCol, grid[newRow, newCol]);
            }
        }
    }

    /// <summary>
    /// Safely retrieves a value from the grid at (row, col). 
    /// Returns the provided defaultValue if the coordinates are out of bounds.
    /// </summary>
    public static T GetValueSafe<T>(this T[,] grid, int row, int col, T defaultValue = default)
    {
        return grid.IsInBounds(row, col) ? grid[row, col] : defaultValue;
    }

    /// <summary>
    /// Finds all coordinates in the grid where the value matches the given predicate.
    /// </summary>
    public static IEnumerable<(int Row, int Col)> FindAll<T>(this T[,] grid, Func<T, bool> predicate)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (predicate(grid[r, c]))
                {
                    yield return (r, c);
                }
            }
        }
    }

    /// <summary>
    /// Finds the first coordinate in the grid where the value matches the given predicate.
    /// </summary>
    public static (int Row, int Col)? FindFirst<T>(this T[,] grid, Func<T, bool> predicate)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (predicate(grid[r, c]))
                {
                    return (r, c);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Performs a flood fill algorithm on the grid starting from the specified coordinates.
    /// Replaces all connected cells matching the target condition with the new value.
    /// </summary>
    /// <param name="grid">The source 2D array.</param>
    /// <param name="startRow">The starting row index.</param>
    /// <param name="startCol">The starting column index.</param>
    /// <param name="match">A predicate to determine if a cell should be filled.</param>
    /// <param name="newValue">The value to fill the cells with.</param>
    /// <param name="includeDiagonals">If true, includes diagonal neighbors in the fill.</param>
    public static void FloodFill<T>(this T[,] grid, int startRow, int startCol, Func<T, bool> match, T newValue,
        bool includeDiagonals = false)
    {
        if (!grid.IsInBounds(startRow, startCol))
        {
            return;
        }

        // Use a HashSet to track visited coordinates to prevent infinite loops
        var visited = new HashSet<(int Row, int Col)>();
        var queue = new Queue<(int Row, int Col)>();

        // Start condition check: only proceed if the start node matches
        if (match(grid[startRow, startCol]))
        {
            queue.Enqueue((startRow, startCol));
            visited.Add((startRow, startCol));
        }

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();

            // Update the grid value
            grid[r, c] = newValue;

            // Get valid neighbors using the existing helper method
            foreach (var neighbor in grid.GetNeighbors(r, c, includeDiagonals))
            {
                // Check if not visited and matches criteria
                if (visited.Contains((neighbor.Row, neighbor.Col)) || !match(neighbor.Value))
                {
                    continue;
                }

                visited.Add((neighbor.Row, neighbor.Col));
                queue.Enqueue((neighbor.Row, neighbor.Col));
            }
        }
    }
    
    /// <summary>
    /// Performs a read-only BFS traversal (similar to FloodFill) to find reachable targets.
    /// Useful for pathfinding puzzles where you need to count reachable destinations.
    /// </summary>
    public static int CountReachableTargets<T>(
        this T[,] grid, 
        int startRow, 
        int startCol, 
        Func<T, T, bool> canStep, 
        Func<T, bool> isTarget)
    {
        if (!grid.IsInBounds(startRow, startCol))
        {
            return 0;
        }

        var visited = new HashSet<(int Row, int Col)>();
        var reachedTargets = new HashSet<(int Row, int Col)>();
        var queue = new Queue<(int Row, int Col)>();

        queue.Enqueue((startRow, startCol));
        visited.Add((startRow, startCol));

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();
            var currentVal = grid[r, c];

            if (isTarget(currentVal))
            {
                reachedTargets.Add((r, c));
            }

            foreach (var neighbor in grid.GetNeighbors(r, c))
            {
                if (!visited.Contains((neighbor.Row, neighbor.Col)) && 
                    canStep(currentVal, neighbor.Value))
                {
                    visited.Add((neighbor.Row, neighbor.Col));
                    queue.Enqueue((neighbor.Row, neighbor.Col));
                }
            }
        }

        return reachedTargets.Count;
    }
    
    /// <summary>
    /// Counts the number of distinct paths from a starting point to any target matching the criteria.
    /// Uses DFS with memoization to efficiently count paths (e.g., for calculating Trailhead Ratings).
    /// </summary>
    public static int CountDistinctPaths<T>(
        this T[,] grid,
        int startRow,
        int startCol,
        Func<T, T, bool> canStep,
        Func<T, bool> isTarget)
    {
        // Cache to store the number of paths from a specific coordinate to the end
        var cache = new Dictionary<(int Row, int Col), int>();
        return CountPathsRecursive(grid, startRow, startCol, canStep, isTarget, cache);
    }

    private static int CountPathsRecursive<T>(
        T[,] grid,
        int r,
        int c,
        Func<T, T, bool> canStep,
        Func<T, bool> isTarget,
        Dictionary<(int Row, int Col), int> cache)
    {
        if (!grid.IsInBounds(r, c))
        {
            return 0;
        }

        // Check cache first to see if we already calculated paths from this spot
        if (cache.TryGetValue((r, c), out var count))
        {
            return count;
        }

        var currentVal = grid[r, c];

        // Base case: if we reached the target (9), we found 1 valid path ending here
        if (isTarget(currentVal))
        {
            return 1;
        }

        var totalPaths = 0;
        foreach (var neighbor in grid.GetNeighbors(r, c))
        {
            // If we can step to the neighbor (e.g., next == current + 1)
            if (canStep(currentVal, neighbor.Value))
            {
                totalPaths += CountPathsRecursive(grid, neighbor.Row, neighbor.Col, canStep, isTarget, cache);
            }
        }

        // Store result in cache and return
        cache[(r, c)] = totalPaths;
        return totalPaths;
    }
}
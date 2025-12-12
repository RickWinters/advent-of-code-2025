using AoC.Helpers.Helpers;
using AoC.Helpers.Models;

namespace AoC.Year2025;

public class Day8(string dayPath) : DayBase(8, dayPath)
{
    //Kruskal's Algorithm: Building Minimum Spanning Trees by adding edges and checking if endpoints are already connected.
    public override object RunDay(int part)
    {
        return part switch
        {
            1 => Part1(),
            2 => Part2(),
            _ => throw new ArgumentOutOfRangeException(nameof(part))
        };
    }

    private object Part1()
    {
        var input = TextInputHelper.ReadLinesAsList(DayPath, s =>
        {
            var parts = s.Split(',');
            return new Point3D(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        });
        var pairs = GetPairs(input);

        // Take top 1000 closest pairs since we have a list of 1000
        var sortedPairs = pairs.OrderBy(p => p.distSq).Take(input.Count);

        // Create a distinct group IDRange
        var groupIds = Enumerable.Range(0, input.Count).ToArray();

        foreach (var (u, v, _) in sortedPairs)
        {
            var groupU = groupIds[u];
            var groupV = groupIds[v];

            if (groupU == groupV)
            {
                continue;
            }

            // Merge groups: update all nodes having groupV to groupU
            for (var k = 0; k < groupIds.Length; k++)
            {
                if (groupIds[k] == groupV)
                {
                    groupIds[k] = groupU;
                }
            }
        }

        // Count sizes of each group
        var sizes = groupIds
            .GroupBy(id => id)
            .Select(g => g.Count())
            .OrderByDescending(count => count)
            .Take(3)
            .ToList();

        long result = 1;
        foreach (var s in sizes)
        {
            result *= s;
        }

        return result;
    }

    private object Part2()
    {
        var input = TextInputHelper.ReadLinesAsList(DayPath, s =>
        {
            var parts = s.Split(',');
            return new Point3D(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        });
        var pairs = GetPairs(input);

        // Sort ALL pairs by distance take all.
        var sortedPairs = pairs.OrderBy(p => p.distSq);

        // Create a distinct group IDRange
        var groupIds = Enumerable.Range(0, input.Count).ToArray();
        
        // amount of groups.
        var distinctGroups = input.Count;

        foreach (var (u, v, _) in sortedPairs)
        {
            var groupU = groupIds[u];
            var groupV = groupIds[v];

            ConsoleWrite($"{groupU}, {groupV}, {distinctGroups}");

            if (groupU == groupV)
            {
                continue;
            }

            // Merge groups: replace all instances of groupV with groupU
            for (var k = 0; k < groupIds.Length; k++)
            {
                if (groupIds[k] == groupV)
                {
                    groupIds[k] = groupU;
                }
            }

            distinctGroups--;

            // If only 1 group remains, everyone is connected return the last connection distance
            if (distinctGroups != 1)
            {
                continue;
            }

            var p1 = input[u];
            var p2 = input[v];
            return (long)p1.X * p2.X;
        }

        return 0;
    }

    private List<(int p1Idx, int p2Idx, long distSq)> GetPairs(List<Point3D> input)
    {
        var pairs = new List<(int p1Idx, int p2Idx, long distSq)>();
        // Iterate over the complete input connecting every box and calculate the distance.
        for (var i = 0; i < input.Count; i++)
        {
            // skip yourself (i + 1)
            for (var j = i + 1; j < input.Count; j++)
            {
                var p1 = input[i];
                var p2 = input[j];
                long dx = p1.X - p2.X;
                long dy = p1.Y - p2.Y;
                long dz = p1.Z - p2.Z;
                var distSq = dx * dx + dy * dy + dz * dz;
                // add pair to pairs with distance (this will create 1-2 with a distance 1-3, then 2-3 etc.)
                // save the index of the points
                pairs.Add((i, j, distSq));
            }
        }

        return pairs;
    }
}
using AoC.Helpers;
using AoC.Helpers.Converters;
using AoC.Helpers.GraphClasses;
using AoC.Helpers.Helpers;

namespace AoC.Year2025;

// Hippy hopety graph is my property
public class Day11(string dayPath) : DayBase(12, dayPath)
{
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
        var tangledCablesLinked = GraphConverter.CreateFromAdjacencyList(DayPath);
        var you = tangledCablesLinked.Nodes.FirstOrDefault(n => n.Value == "you");
        var @out = tangledCablesLinked.Nodes.FirstOrDefault(n => n.Value == "out");

        if (you == null || @out == null)
        {
            return "Start or End node not found";
        }

        var allCablesFromYouToOut = new List<List<string>>();
        
        // Find all paths using DFS with backtracking
        FindAllPaths(you, @out, [], [], allCablesFromYouToOut);

        // Returning the count of paths, or you could return the paths themselves
        return allCablesFromYouToOut.Count;
    }

    private object Part2()
    {
        var tangledCablesLinked = GraphConverter.CreateFromAdjacencyList(DayPath);
        
        var server = tangledCablesLinked.Nodes.FirstOrDefault(n => n.Value == "svr");
        var @out = tangledCablesLinked.Nodes.FirstOrDefault(n => n.Value == "out");

        if (server == null || @out == null)
        {
            return "Start or End node not found";
        }

        // Define the recursive function signature
        Func<(GraphNode<string> Node, bool HasFft, bool HasDac), long> allPathsFromServerToOutCounter = null!;

        // Implement the logic
        allPathsFromServerToOutCounter = Memoizer.Memoize<(GraphNode<string> Node, bool HasFft, bool HasDac), long>(solveNextNode =>
        {
            var (currentNodeFromCable, foundFft, foundDac) = solveNextNode;

            // Update state of current cable
            if (currentNodeFromCable.Value == "fft")
            {
                foundFft = true;
            }

            if (currentNodeFromCable.Value == "dac")
            {
                foundDac = true;
            }

            // Base case: We found an end
            if (currentNodeFromCable == @out)
            {
                return foundFft && foundDac ? 1 : 0;
            }

            long count = 0;
            foreach (var edge in currentNodeFromCable.Edges)
            {
                // Recursive call
                count += allPathsFromServerToOutCounter((edge.To, foundFft, foundDac));
            }

            return count;
        });

        // Start the calculation
        return allPathsFromServerToOutCounter((server, false, false));
    }
    
    private void FindAllPaths(
        GraphNode<string> current, 
        GraphNode<string> end, 
        List<string> currentPath, 
        HashSet<string> visited, 
        List<List<string>> results)
    {
        // Mark current node as visited for this specific path
        visited.Add(current.Value);
        currentPath.Add(current.Value);

        if (current == end)
        {
            // Found a path! Save a copy of it.
            results.Add([..currentPath]);
        }
        else
        {
            foreach (var edge in current.Edges)
            {
                if (!visited.Contains(edge.To.Value))
                {
                    FindAllPaths(edge.To, end, currentPath, visited, results);
                }
            }
        }

        // Backtrack: Unmark current node so it can be used in other valid paths
        visited.Remove(current.Value);
        currentPath.RemoveAt(currentPath.Count - 1);
    }
}
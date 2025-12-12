using AoC.Helpers.Helpers;

namespace AoC.Year2025;

public class Day7(string dayPath) : DayBase(7, dayPath)
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
        var beamTree = TextInputHelper.ReadLinesAs2DCharArray(DayPath);

        var canSplitCounter = 0;
        var rows = beamTree.GetLength(0);
        var cols = beamTree.GetLength(1);

        for (var r = 1; r < rows; r++)
        {
            var updates = new List<(int c, char val)>();
            for (var c = 0; c < cols; c++)
            {
                var above = beamTree[r - 1, c];
                var current = beamTree[r, c];

                // character above should be a | or S
                if (above != 'S' && above != '|')
                {
                    continue;
                }

                // replace "." with a "|" (beam)
                if (current != '.')
                {
                    // update left and right side with "|" if splitter "^" exists and char is "."
                    if (current != '^')
                    {
                        continue;
                    }

                    // add canSplitCounter++ because it was able to split.
                    canSplitCounter++;
                    if (c > 0 && beamTree[r, c - 1] == '.')
                    {
                        updates.Add((c - 1, '|'));
                    }

                    if (c < cols - 1 && beamTree[r, c + 1] == '.')
                    {
                        updates.Add((c + 1, '|'));
                    }
                }
                else
                {
                    updates.Add((c, '|'));
                }
            }

            // update beamTree with new values
            foreach (var (c, val) in updates)
            {
                beamTree[r, c] = val;
            }
        }

        return canSplitCounter;
        //return string.Join(Environment.NewLine, beamTree.Select(x => new string(x)));
    }

    private object Part2()
    {
        var beamTree = TextInputHelper.ReadLinesAs2DCharArray(DayPath);
        var rows = beamTree.GetLength(0);
        var cols = beamTree.GetLength(1);
        
        // Create copy of beamTree as Pascals Triangle with long values.
        var beamTreePascalsTriangle = new long[rows, cols];

        // Initialize set 1 in the Pascals Triangle where S is the starting position of beamTree
        for (var col = 0; col < cols; col++)
        {
            if (beamTree[0, col] != 'S')
            {
                continue;
            }

            beamTreePascalsTriangle[0, col] = 1;
            break;
        }

        for (var row = 1; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                long incoming = 0;

                //                                          53
                // always check straight down from above so .^
                // end result will become 8 with logic below
                if (beamTree[row, col] != '^')
                {
                    incoming += beamTreePascalsTriangle[row - 1, col];
                }

                //                                                                       3
                // Check if splitter exists on the left side of current column index so ^.
                // if so add that value to our beamTreePascalsTriangle tree
                if (col > 0 && beamTree[row, col - 1] == '^')
                {
                    incoming += beamTreePascalsTriangle[row - 1, col - 1];
                }

                //                                                                        3
                // Check if splitter exists on the right side of current column index so .^
                // if so add that value to our beamTreePascalsTriangle tree
                if (col < cols - 1 && beamTree[row, col + 1] == '^')
                {
                    incoming += beamTreePascalsTriangle[row - 1, col + 1];
                }

                beamTreePascalsTriangle[row, col] = incoming;
            }
        }

        long totalPossibleBeamPaths = 0;
        // Sum all values in last row, which should be the total possible beam paths.
        for (var c = 0; c < cols; c++)
        {
            totalPossibleBeamPaths += beamTreePascalsTriangle[rows - 1, c];
        }

        return totalPossibleBeamPaths;
    }
}
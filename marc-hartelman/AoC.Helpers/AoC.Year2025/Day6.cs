using AoC.Helpers.Helpers;

namespace AoC.Year2025;

public class Day6(string dayPath) : DayBase(6, dayPath)
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
        // Read the input as a string grid, handling irregular spaces
        var grid = TextInputHelper.ReadSpaceSeparated2DArray(DayPath, x => x);
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        
        long grandTotal = 0;

        for (var c = 0; c < cols; c++)
        {
            // The operator is in the last row of the column
            var op = grid[rows - 1, c];

            // * = 1 since 1 * a number results in a number we can work with instead of 0
            long colResult = op == "*" ? 1 : 0;

            // Process all rows except the last one (which contains the operator)
            for (var r = 0; r < rows - 1; r++)
            {
                if (!long.TryParse(grid[r, c], out var number))
                {
                    continue;
                }

                if (op == "*")
                {
                    colResult *= number;
                }
                else
                {
                    colResult += number;
                }
            }
            ConsoleWrite($"{colResult}");
            grandTotal += colResult;
            ConsoleWrite($"{grandTotal}");
        }

        return grandTotal;
    }

    private object Part2()
    {
        // Use the fixed-width reader to preserve padding
        var gridColumns = TextInputHelper.ReadFixedColumnGrid(DayPath);
        long grandTotal = 0;

        // Iterate columns Right to Left
        for (var c = 0; c < gridColumns.Count; c++)
        {
            var colRows = gridColumns[c];
            // take last row, which contains the operator and trim it (since it's fixed width)
            var op = colRows[^1].Trim(); 
            var numberRows = colRows.Take(colRows.Count - 1).ToList();

            // * = 1 since 1 * a number results in a number we can work with instead of 0
            long colResult = op == "*" ? 1 : 0;
            var width = numberRows[0].Length;

            // Scan character in column from Right to Left
            for (var i = width - 1; i >= 0; i--)
            {
                var verticalNumStr = "";
                foreach (var row in numberRows)
                {
                    var charAtPos = row[i];
                    // ignore spaces
                    // for example " 12"
                    //             "123"
                    //                ^ take this so 23 -> then 12 -> then 1
                    
                    // other example "12 "
                    //               "123"
                    //                  ^ take this so 3 then 22 then 11
                    if (charAtPos != ' ')
                    {
                        verticalNumStr += charAtPos;
                    }
                }

                if (string.IsNullOrEmpty(verticalNumStr))
                {
                    continue;
                }

                ConsoleWrite($"{verticalNumStr}");
                var val = long.Parse(verticalNumStr);
                if (op == "*")
                {
                    colResult *= val;
                }
                else
                {
                    colResult += val;
                }
            }

            ConsoleWrite($"{colResult} done with operator {op}");
            // Add to grand total
            grandTotal += colResult;
            ConsoleWrite($"{grandTotal}");
        }

        return grandTotal;
    }
}
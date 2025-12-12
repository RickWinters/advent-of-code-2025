using AoC.Helpers.Helpers;

namespace AoC.Year2025;

public class Day4(string dayPath) : DayBase(4, dayPath)
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
        var canPickUpRolls = 0;
        
        var inputGrid = TextInputHelper.ReadLinesAs2DArray(DayPath, x => new GridSpecification(x == '@'));
        for (var i = 0; i < inputGrid.GetLength(0); i++)
        {
            for (var j = 0; j < inputGrid.GetLength(1); j++)
            {
                if (!inputGrid[i, j].IsPaper)
                {
                    continue;
                }

                var neighbors = inputGrid.GetNeighbors(i, j, true).ToList();
                var canPickUp = neighbors.Count(x => x.Value.IsPaper) < 4;
                if (!canPickUp)
                {
                    continue;
                }

                ConsoleWrite($"location: {i},{j} -- can pick up");
                canPickUpRolls++;
            }
        }

        return canPickUpRolls;
    }

    private object Part2()
    {
        var canPickUpRolls = 0;
        
        var inputGrid = TextInputHelper.ReadLinesAs2DArray(DayPath, x => new GridSpecification(x == '@'));
        bool stilCanPickUp;
        do
        {
            stilCanPickUp = false;
            var toRemove = new List<(int r, int c)>();

            for (int i = 0; i < inputGrid.GetLength(0); i++)
            {
                for (int j = 0; j < inputGrid.GetLength(1); j++)
                {
                    if (inputGrid[i, j].IsPaper)
                    {
                        var neighbors = inputGrid.GetNeighbors(i, j, true).ToList();
                        var canPickUp = neighbors.Count(x => x.Value.IsPaper) < 4;
                        if (canPickUp)
                        {
                            ConsoleWrite($"location: {i},{j} -- can pick up");
                            toRemove.Add((i, j));
                        }
                    }
                }
            }

            if (toRemove.Count <= 0)
            {
                continue;
            }

            ConsoleWrite($"amount to remove: {toRemove.Count} -- removed");
            canPickUpRolls += toRemove.Count;
            ConsoleWrite($"total picked up: {canPickUpRolls} -- pickd up");
            foreach (var (r, c) in toRemove)
            {
                // Update the grid: change character to '.' and isPaper to false
                inputGrid[r, c] = new GridSpecification(false);
            }
                
            stilCanPickUp = true;
        } while (stilCanPickUp);

        return canPickUpRolls;
    }

    private record GridSpecification(bool IsPaper);
}
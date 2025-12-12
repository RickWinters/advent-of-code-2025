using AoC.Helpers.Helpers;
using AoC.Helpers.Models;

namespace AoC.Year2025;

public class Day5(string dayPath) : DayBase(5, dayPath)
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
        var ingredientRanges = TextInputHelper.ReadLinesAsStringList(DayPath);
        var allRanges = new List<LongRange>();
        var freshIdList = new List<long>();
        var parsingRanges = true;

        var amountFresh = 0;

        foreach (var ingredientRange in ingredientRanges)
        {
            if (string.IsNullOrWhiteSpace(ingredientRange))
            {
                parsingRanges = false;
                continue;
            }

            if (parsingRanges)
            {
                var parts = ingredientRange.Split('-');
                if (parts.Length != 2 || !long.TryParse(parts[0], out var start) ||
                    !long.TryParse(parts[1], out var end))
                {
                    continue;
                }

                var newRange = new LongRange(start, end);
                ConsoleWrite($"new range {newRange}");
                allRanges.Add(newRange);
            }
            else
            {
                ConsoleWrite($"fresh id: {ingredientRange}");
                freshIdList.Add(long.Parse(ingredientRange));
            }
        }

        foreach (var freshId in freshIdList)
        {
            foreach (var range in allRanges)
            {
                if (!range.Contains(freshId))
                {
                    continue;
                }

                amountFresh++;
                break;
            }
        }

        return amountFresh;
    }

    private object Part2()
    {
        var ingredientRanges = TextInputHelper.ReadLinesAsStringList(DayPath);
        var allRanges = new List<LongRange>();
        var amountFresh = 0L;

        foreach (var line in ingredientRanges)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            // Split by space to handle multiple ranges on a line (if any)
            var ranges = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var range in ranges)
            {
                var parts = range.Split('-');
                if (parts.Length != 2 || !long.TryParse(parts[0], out var start) ||
                    !long.TryParse(parts[1], out var end))
                {
                    continue;
                }

                var newRange = new LongRange(start, end);
                ConsoleWrite($"{newRange}");
                allRanges.Add(newRange);
            }
        }

        var mergedRanges = LongRange.Merge(allRanges);

        foreach (var mergedRange in mergedRanges)
        {
            ConsoleWrite($"merged range: {mergedRange}");
            amountFresh += mergedRange.Length;
        }

        return amountFresh;
    }
}
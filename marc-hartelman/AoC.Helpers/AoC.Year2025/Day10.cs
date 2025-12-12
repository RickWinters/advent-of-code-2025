using AoC.Helpers.Helpers;

namespace AoC.Year2025;

public class Day10(string dayPath) : DayBase(10, dayPath)
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
        var machines = TextInputHelper.ReadLinesAsList(DayPath, ParseLine);
        var totalPresses = 0L;

        foreach (var machine in machines)
        {
            ConsoleWrite(
                $"{machine.Lights} -> {machine.TargetLightsAsBinary} ({machine.ButtonWiringsAsBinary.Count} buttons) ");
            var leastButtonPresses = PathFindingHelper.BfsDistance(
                start: 0L,
                getNeighbors: current => machine.ButtonWiringsAsBinary.Select(b => current ^ b),
                isGoal: current => current == machine.TargetLightsAsBinary
            );

            if (leastButtonPresses != -1)
            {
                totalPresses += leastButtonPresses;
            }

            ConsoleWrite(leastButtonPresses.ToString());
        }

        return totalPresses;
    }

    private object Part2()
    {
        var machines = TextInputHelper.ReadLinesAsList(DayPath, ParseLine);
        var totalPresses = 0L;

        foreach (var machine in machines)
        {
            long presses = PathFindingHelper.ReverseHalvingMinCost(machine.Joltages.ToArray(), machine.ButtonWiringList);
            totalPresses += presses;
            ConsoleWrite($"Total presses for this machine: {presses.ToString()}");
        }

        return totalPresses;
    }

    private Machine ParseLine(string line)
    {
        // 1. Parse Pattern: [.##.] -> Binary
        var patternStart = line.IndexOf('[');
        var patternEnd = line.IndexOf(']');
        var patternStr = line.Substring(patternStart + 1, patternEnd - patternStart - 1);

        long targetPatternAsBinary = 0;
        for (var i = 0; i < patternStr.Length; i++)
        {
            if (patternStr[i] == '#')
            {
                targetPatternAsBinary |= 1L << i; // 'Reverse' binary: Index 0 is LSB
            }
        }

        var targetPattern = line.Substring(patternStart + 1, patternEnd - patternStart - 1);

        // 2. Parse Set: {3,5,4,7} -> List of indices
        var setStart = line.IndexOf('{');
        var setEnd = line.IndexOf('}');
        var setString = line.Substring(setStart + 1, setEnd - setStart - 1);

        var joltages = setString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        // 3. Parse Button Wirings: (3) (1,3) ... -> List of Binary values
        var sequencePart = line.Substring(patternEnd + 1, setStart - patternEnd - 1);

        var buttonWiringsAsBinary = sequencePart
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s =>
            {
                // Remove ( and )
                var trimmed = s.Trim('(', ')');
                var bits = trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse);

                long wiringValue = 0;
                foreach (var bit in bits)
                {
                    wiringValue |= 1L << bit;
                }

                return wiringValue;
            })
            .ToList();

        // 1. Parse Pattern: [.##.]

        var buttonWiringList = sequencePart.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim('(', ')') // Remove outer parenthesis
                .Split(',') // Split numbers inside
                .Select(int.Parse)
                .ToArray())
            .ToList();

        return new Machine(targetPattern, targetPatternAsBinary, buttonWiringList, buttonWiringsAsBinary, joltages);
    }

    private record Machine(
        string Lights,
        long TargetLightsAsBinary,
        List<int[]> ButtonWiringList,
        List<long> ButtonWiringsAsBinary,
        List<int> Joltages);
}
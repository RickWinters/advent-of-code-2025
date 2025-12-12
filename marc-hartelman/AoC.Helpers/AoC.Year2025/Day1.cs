using AoC.Helpers.Helpers;

namespace AoC.Year2025;

public class Day1(string dayPath) : DayBase(1, dayPath)
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

    private int Part1()
    {
        var initialValue = 50;
        var password = 0;
        
        var lines = TextInputHelper.ReadLinesAsList(DayPath, x => new Combination(x[0] == 'L' ? DirectionCombination.L : DirectionCombination.R, int.Parse(x[1..])));

        foreach (var line in lines)
        {
            if (line.Direction == DirectionCombination.L)
            {
                initialValue -= line.Distance;
            }else if (line.Direction == DirectionCombination.R)
            {
                initialValue += line.Distance;
            }

            if (initialValue % 100 == 0)
            {
                password++;
            }

            ConsoleWrite($"current value: {initialValue}, password: {password}");
        }

        return password;
    }

    private int Part2()
    {
        var initialValue = 50;
        var password = 0;
        
        var lines = TextInputHelper.ReadLinesAsList(DayPath, x => new Combination(x[0] == 'L' ? DirectionCombination.L : DirectionCombination.R, int.Parse(x[1..])));

        foreach (var line in lines)
        {
            var currentValue = initialValue;
            
            if (line.Direction == DirectionCombination.L)
            {
                initialValue -= line.Distance;
            }else if (line.Direction == DirectionCombination.R)
            {
                initialValue += line.Distance;
            }

            for (var i = currentValue; i != initialValue; i += line.Direction == DirectionCombination.L ? -1 : 1)
            {
                if (i % 100 == 0) password++;
            }

            ConsoleWrite($"current value: {initialValue}, password: {password}");
        }

        return password;
    }
}

public record Combination(DirectionCombination Direction, int Distance);


public enum DirectionCombination
{
    L = 0,
    R = 1
}
using AoC.Helpers.Helpers;

namespace AoC.Year2025;

public class Day3(string dayPath) : DayBase(3, dayPath)
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
        var output = 0;
        
        var batteries =
            TextInputHelper.ReadLinesAsList(DayPath, x => x.ToString().Select(c => int.Parse(c.ToString())).ToList());

        foreach (var battery in batteries)
        {
            var max = 0;
            var nextMax = 0;
            for (var index = 0; index < battery.Count; index++)
            {
                var i = battery[index];
                if (i <= nextMax)
                {
                    continue;
                }

                nextMax = i;
                if (nextMax <= max || index + 1 == battery.Count)
                {
                    continue;
                }

                max = i;
                nextMax = 0;
            }
        
            var currentOutput = max.ToString() + nextMax.ToString();
            output += int.Parse(currentOutput);
            ConsoleWrite(currentOutput);
        }
        // Could've started writing the function.
        // foreach (var battery in batteries)
        // {
        //     var ouputSequence = GetMaxJoltage(battery, 2);
        //     Console.WriteLine(ouputSequence);
        //     output += int.Parse(ouputSequence);
        // }
        // Console.WriteLine(output);

        return output;
    }

    private long Part2()
    {
        long output = 0;
        var batteries = TextInputHelper.ReadLinesAsList(DayPath, x => x.ToString().Select(c => int.Parse(c.ToString())).ToList());
            
        foreach (var battery in batteries)
        {
            // removable digits from stack, when counter reaches 0 no possible removal left and everything should be stored to the stack.
            var toRemove = battery.Count - 12;
            if (toRemove <= 0)
            {
                continue;
            }

            var stack = new Stack<int>();

            foreach (var num in battery)
            {
                // If current number is bigger than the top of stack, 
                // and we still need to remove digits, pop the smaller one.
                // Do this while this is still possible.
                while (toRemove > 0 && stack.Count > 0 && stack.Peek() < num)
                {
                    stack.Pop();
                    toRemove--;
                }
                stack.Push(num);
            }
            
            
            var ouputSequence = string.Join("", stack.Reverse().Take(12));
            ConsoleWrite(ouputSequence);
            output += long.Parse(ouputSequence);
        }
        
        // foreach (var battery in batteries)
        // {
        //     var ouputSequence = GetMaxJoltage(battery, 12);
        //     Console.WriteLine(ouputSequence);
        //     output += long.Parse(ouputSequence);
        // }
        return output;
    }

    /// <summary>
    /// Returns the largest sequence of batteryPower in the list that sum up to the target length.
    /// Function can be used for both Part 1 and Part 2.
    /// </summary>
    /// <param name="batteryPower"></param>
    /// <param name="targetBatteries"></param>
    /// <returns></returns>
    private static string GetMaxJoltage(List<int> batteryPower, int targetBatteries)
    {
        // removable digits from stack, when counter reaches 0 no possible removal left and everything should be stored to the stack.
        var toRemove = batteryPower.Count - targetBatteries;
        if (toRemove <= 0)
        {
            return string.Join("", batteryPower);
        }

        var stack = new Stack<int>();

        foreach (var num in batteryPower)
        {
            // If current number is bigger than the top of stack, 
            // and we still need to remove digits, pop the smaller one.
            // Do this while this is still possible.
            while (toRemove > 0 && stack.Count > 0 && stack.Peek() < num)
            {
                stack.Pop();
                toRemove--;
            }
            stack.Push(num);
        }

        return string.Join("", stack.Reverse().Take(targetBatteries));
    }
}
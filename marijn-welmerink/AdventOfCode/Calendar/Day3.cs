namespace AdventOfCode.Calendar;

/// <summary>
/// Day 3: Lobby
/// </summary>
public class Day3 : ICalendarDayChallenge
{
    private readonly IReadOnlyList<string> _inputLines = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Day3"/> class.
    /// </summary>
    public Day3()
    {
        var inputLines = FileReader.ReadAllLines(InputFile.Day3Input, false);

        if (inputLines == null)
        {
            Console.WriteLine("ERR: Could not read input for Day 3.");

            return;
        }

        if (inputLines.Count == 0)
        {
            Console.WriteLine("ERR: Input lines for Day 3 are empty.");
        }

        _inputLines = inputLines;
    }

    /// <inheritdoc/>
    public void PrintResult()
    {
        var joltSum = GetJoltSum();
        var joltOverrideSum = GetJoltOverrideSum();

        Console.WriteLine($"The total joltage output is: {joltSum}");
        Console.WriteLine($"The maximum joltage output with override is: {joltOverrideSum}");
    }

    /// <summary>
    /// Calculate the total joltage by finding the largest two valid banks in each input line.
    /// </summary>
    /// <returns>The total joltage output.</returns>
    private int GetJoltSum()
    {
        var joltSum = 0;

        foreach (var inputLine in _inputLines)
        {
            var banks = inputLine.Select(x => int.Parse(x.ToString())).ToList();
            var validFirstBanks = banks.Take(banks.Count - 1).ToList();

            var biggestFirstBank = validFirstBanks.Max();
            var biggestFirstBankIndex = validFirstBanks.IndexOf(biggestFirstBank);

            var validSecondBanks = banks.Skip(biggestFirstBankIndex + 1).ToList();
            var biggestSecondBank = validSecondBanks.Max();

            var jolt = int.Parse($"{biggestFirstBank}{biggestSecondBank}");

            joltSum += jolt;
        }

        return joltSum;
    }

    /// <summary>
    /// Build the largest possible joltage output based on the maximum length constraint.
    /// </summary>
    /// <returns>The total joltage output.</returns>
    /// <remarks>It ain't (worth) much, but it's honest work.</remarks>
    private long GetJoltOverrideSum()
    {
        const int maxJoltOverrideLength = 12;
        long joltSum = 0;

        foreach (var inputLine in _inputLines)
        {
            var banks = inputLine.Select(x => int.Parse(x.ToString())).ToList();

            var bank = string.Empty;
            var limit = maxJoltOverrideLength;
            var target = 9;

            while (bank.Length < maxJoltOverrideLength)
            {
                var highestValueInBank = banks.Find(x => x == target);

                // No bank found for the current target, try the next lower target.
                if (highestValueInBank != target)
                {
                    target--;

                    continue;
                }

                var highestValueInBankIndex = banks.IndexOf(highestValueInBank);

                // Last digit, no additional processing needed.
                if (bank.Length == maxJoltOverrideLength - 1)
                {
                    bank += highestValueInBank.ToString();

                    break;
                }

                // Ensure there are enough digits left to complete the bank.
                if (banks.Count - highestValueInBankIndex < limit)
                {
                    target--;

                    continue;
                }

                // Remove everything before the max index, including the max index itself.
                banks = banks.Skip(highestValueInBankIndex + 1).ToList();

                bank += highestValueInBank.ToString();

                limit--;
                target = 9;
            }

            joltSum += long.Parse(bank);
        }

        return joltSum;
    }
}

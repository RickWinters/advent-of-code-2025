namespace AdventOfCode.Calendar;

/// <summary>
/// Day 2: Gift Shop
/// </summary>
public class Day2 : ICalendarDayChallenge
{
    private readonly IReadOnlyList<string> _inputLines = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Day2"/> class.
    /// </summary>
    public Day2()
    {
        var inputLines = FileReader.ReadAllLines(InputFile.Day2Input, true);

        if (inputLines == null)
        {
            Console.WriteLine("ERR: Could not read input for Day 2.");

            return;
        }

        if (inputLines.Count == 0)
        {
            Console.WriteLine("ERR: Input lines for Day 2 are empty.");
        }

        _inputLines = inputLines;
    }

    /// <inheritdoc/>
    public void PrintResult()
    {
        var initialInvalidIdSum = GetInitialInvalidIdSum();
        var finalInvalidIdSum = GetFinalInvalidIdSum();

        Console.WriteLine($"Adding up all initial invalid IDs produces: {initialInvalidIdSum}");
        Console.WriteLine($"Adding up all final invalid IDs produces: {finalInvalidIdSum}");
    }

    /// <summary>
    /// Calculates the final sum of all IDs within the input ranges that are considered invalid IDs.
    /// </summary>
    /// <returns>The final sum of invalid IDs.</returns>
    /// <remarks>This method is very inefficient, but fuck it.</remarks>
    private long GetInitialInvalidIdSum()
    {
        long invalidIdSum = 0;

        foreach (var line in _inputLines)
        {
            var idRange = line.Split('-');

            var start = long.Parse(idRange[0]);
            var end = long.Parse(idRange[1]);

            for (var i = start; i <= end; i++)
            {
                var id = i.ToString();

                if (!IsEven(id.Length))
                {
                    continue;
                }

                if (IsInitialInvalidId(id))
                {
                    invalidIdSum += i;
                }
            }
        }

        return invalidIdSum;
    }

    /// <summary>
    /// Calculates the final sum of all IDs within the input ranges that are considered invalid IDs.
    /// </summary>
    /// <returns>The final sum of invalid IDs.</returns>
    /// <remarks>This method is even more inefficient, but again, fuck it.</remarks>
    private long GetFinalInvalidIdSum()
    {
        long invalidIdSum = 0;

        foreach (var line in _inputLines)
        {
            var idRange = line.Split('-');

            var start = long.Parse(idRange[0]);
            var end = long.Parse(idRange[1]);

            for (var i = start; i <= end; i++)
            {
                var id = i.ToString();

                if (IsFinalInvalidId(id))
                {
                    invalidIdSum += i;
                }
            }
        }

        return invalidIdSum;
    }

    /// <summary>
    /// Check if the length is even.
    /// </summary>
    /// <param name="length">The length to check.</param>
    /// <returns>True if the length is even; otherwise, false.</returns>
    private static bool IsEven(int length)
    {
        return length % 2 == 0;
    }

    /// <summary>
    /// Divides the ID in half and checks if both halves are identical.
    /// </summary>
    /// <param name="id">The ID to check.</param>
    /// <returns>True if the ID is identical in both halves; otherwise, false.</returns>
    private static bool IsInitialInvalidId(string id)
    {
        var halfLength = id.Length / 2;
        var leftPart = id[..halfLength];
        var rightPart = id[halfLength..];

        return leftPart == rightPart;
    }

    /// <summary>
    /// Checks if the ID is made up of a repeating pattern.
    /// </summary>
    /// <param name="id">The ID to check.</param>
    /// <returns>True if the ID is made up of a repeating pattern; otherwise, false.</returns>
    private static bool IsFinalInvalidId(string id)
    {
        var idLength = id.Length;

        for (var i = 1; i < idLength; i++)
        {
            // Only consider lengths that divide evenly into the ID length.
            if (idLength % i != 0)
            {
                continue;
            }

            var pattern = id[..i];
            var repeatCount = idLength / i;
            var repeated = string.Concat(Enumerable.Repeat(pattern, repeatCount));

            if (repeated == id)
            {
                return true;
            }
        }

        return false;
    }
}

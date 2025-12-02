namespace AdventOfCode.Calendar;

/// <summary>
/// Day 2: Gift Shop
/// </summary>
public class Day2 : ICalendarDayChallenge
{
    private readonly IReadOnlyList<string> _inputLines = [];

    private readonly IReadOnlyList<string> _temporaryTestInputLines = new List<string>
    {
        "11-22",
        "95-115",
        "998-1012",
        "1188511880-1188511890",
        "222220-222224",
        "1698522-1698528",
        "446443-446449",
        "38593856-38593862",
        "565653-565659",
        "824824821-824824827",
        "2121212118-2121212124"
    };

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
    }
}

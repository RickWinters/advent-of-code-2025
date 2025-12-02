namespace AdventOfCode.Calendar;

/// <summary>
/// Day 1: Secret Entrance
/// </summary>
public class Day1 : ICalendarDayChallenge
{
    private const int MinimumDialPosition = 0;
    private const int MaximumDialPosition = 99;
    private const int DialStartingPosition = 50;

    private readonly IReadOnlyList<string> _inputLines = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Day1"/> class.
    /// </summary>
    public Day1()
    {
        var inputLines = FileReader.ReadAllLines(InputFile.Day1Input, false);

        if (inputLines == null)
        {
            Console.WriteLine("ERR: Could not read input for Day 1.");

            return;
        }

        if (inputLines.Count == 0)
        {
            Console.WriteLine("ERR: Input lines for Day 1 are empty.");
        }

        _inputLines = inputLines;
    }

    /// <summary>
    /// Prints the result of the Day 1 challenge.
    /// </summary>
    public void PrintResult()
    {
        var currentPosition = DialStartingPosition;
        var initialPassword = 0;
        var finalPassword = 0;

        foreach (var inputLine in _inputLines)
        {
            var (direction, steps) = ParseInputLine(inputLine);
            var newPosition = GetNextPosition(currentPosition, direction, steps);
            var positionsAtZero = GetPositionsAtZeroForInputLine(currentPosition, direction, steps);

            currentPosition = newPosition;

            if (currentPosition == 0)
            {
                initialPassword++;
            }

            finalPassword += positionsAtZero.Count();
        }

        Console.WriteLine($"The password to open the door is: {initialPassword}");
        Console.WriteLine($"The password by using password method 0x434C49434B is: {finalPassword}");
    }

    /// <summary>
    /// Parses an input line to extract the direction and number of steps.
    /// </summary>
    /// <param name="inputLine">The input line to parse.</param>
    /// <returns>The direction and number of steps as a tuple.</returns>
    private static (Direction, int) ParseInputLine(string inputLine)
    {
        var direction = inputLine.StartsWith('L') ? Direction.Left : Direction.Right;

        var stepsString = inputLine[1..];
        var steps = int.Parse(stepsString);

        return (direction, steps);
    }

    /// <summary>
    /// Calculates the next position on the dial given the old position, direction, and number of steps. Starts by
    /// checking the total number of positions on the dial and direction of movement. Then, it calculates the new
    /// position, ensuring it wraps around correctly using modular arithmetic.
    /// </summary>
    /// <param name="oldPosition">The current/old position on the dial.</param>
    /// <param name="newDirection">The direction to move (left or right).</param>
    /// <param name="newSteps">The number of steps to move in the specified direction.</param>
    /// <returns>The new position on the dial after moving.</returns>
    private static int GetNextPosition(int oldPosition, Direction newDirection, int newSteps)
    {
        const int dialRange = MaximumDialPosition - MinimumDialPosition + 1;

        var positionChange = newDirection == Direction.Left ? -newSteps : newSteps;
        var movedAndNormalizedPosition = oldPosition + positionChange - MinimumDialPosition + dialRange;
        var newPosition = movedAndNormalizedPosition % dialRange + MinimumDialPosition;

        return newPosition;
    }

    /// <summary>
    /// Determines all intermediate steps during a dial movement that result in the dial being at position zero.
    /// </summary>
    /// <param name="oldPosition">The current/old position on the dial.</param>
    /// <param name="newDirection">The direction to move (left or right).</param>
    /// <param name="newSteps">The number of steps to move in the specified direction.</param>
    /// <returns>A collection of step counts where the dial reaches position zero.</returns>
    private static IEnumerable<int> GetPositionsAtZeroForInputLine(int oldPosition, Direction newDirection, int newSteps)
    {
        return from step in Enumerable.Range(1, newSteps)
            let intermediatePosition = GetNextPosition(oldPosition, newDirection, step)
            where intermediatePosition == 0
            select step;
    }

    /// <summary>
    /// Represents the direction of movement on the dial.
    /// </summary>
    private enum Direction
    {
        Left,
        Right
    }
}

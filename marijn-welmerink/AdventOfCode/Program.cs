using AdventOfCode.Calendar;

namespace AdventOfCode;

public static class Program
{
    public static void Main()
    {
        var day1 = new Day1();
        var day2 = new Day2();

        Console.WriteLine("=== Advent of Code 2025 ===");
        Console.WriteLine("The following are the results for each day's challenges.");
        Console.WriteLine();

        Console.WriteLine("--- Day 1: Secret Entrance ---");
        day1.PrintResult();
        Console.WriteLine();

        Console.WriteLine("--- Day 2: Gift Shop ---");
        day2.PrintResult();
        Console.WriteLine();
    }
}

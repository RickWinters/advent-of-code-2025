using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AoC.Year2025;

public class AoCDayRunner
{
    private const string QualifiedSolutionTypeNameFormat = "{0}.{1}";
    private const string SolutionTypeName = "Solution";

    public Task Run(int day, string path, bool writerEnabled = false)
    {
        if (!TryCreateSolutionInstance(day, path, out var solution))
        {
            Console.WriteLine($"day: {day} not found.");
            return Task.CompletedTask;
        }


        RunInternal(solution, writerEnabled, day);
        return Task.CompletedTask;
    }

    private static void RunInternal(DayBase solution, bool writerEnabled, int day)
    {
        solution.WriterEnabled = writerEnabled;
        
        for (var i = 0; i < 2; i++)
        {
            RunPartInternal(solution, part: i + 1, day);
        }
    }

    private static void RunPartInternal(DayBase solutionInstance, int part, int day)
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            var result = solutionInstance.RunDay(part);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Elapsed: {stopwatch.Elapsed}] Day number: {day} Part: {part} solution => {result}");
            Console.ResetColor();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error running solution:\n{e}");
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private static bool TryCreateSolutionInstance(int day, string dayPath, [NotNullWhen(true)] out DayBase? instance)
    {
        try
        {
            var assembly = typeof(DayBase).Assembly;
            var type = assembly.GetType(GetQualifiedSolutionTypeName(day))!;

            instance = (DayBase)Activator.CreateInstance(type, args: [dayPath])!;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to create {SolutionTypeName} instance:\n{e}");
        }

        instance = null;
        return false;
    }

    private static string GetQualifiedSolutionTypeName(int day)
    {
        var owningAssemblyName = typeof(DayBase).Assembly.GetName().Name;
        var formattedDayString = $"Day{day}";

        return string.Format(QualifiedSolutionTypeNameFormat,
            owningAssemblyName,
            formattedDayString);
    }
}
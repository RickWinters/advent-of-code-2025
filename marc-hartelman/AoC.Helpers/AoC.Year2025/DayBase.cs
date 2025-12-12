namespace AoC.Year2025;

public abstract class DayBase
{
    protected string DayPath { get; }
    public bool WriterEnabled { get; set; }
    public DayBase(int day, string dayPath)
    {
        DayPath = dayPath;
    }

    protected void ConsoleWrite(string text)
    {
        if (WriterEnabled)
        {
            Console.WriteLine(text);
        }
    }

    public abstract object RunDay(int part);
}
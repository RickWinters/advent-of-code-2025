using AoC.Helpers.Helpers;

namespace AoC.Year2025;

// Evil Question easy input :D
public class Day12(string dayPath) : DayBase(12, dayPath)
{
    public override object RunDay(int part)
    {
        return part switch
        {
            1 => Part1(),
            2 => -1,
            _ => throw new ArgumentOutOfRangeException(nameof(part))
        };
    }

    private object Part1()
    {
        var input = Parse(DayPath);

        // We now parse presents as (Index, Area), so no grid work needed here.
        var presentAreas = input.Presents
            .OrderBy(p => p.Index)
            .Select(p => p.Area)
            .ToArray();

        var packableTrees = 0;

        // Hacked solution
        foreach (var tree in input.ChristmasTrees)
        {
            long needed = 0;
            for (var i = 0; i < tree.Values.Length; i++)
            {
                needed += (long)tree.Values[i] * presentAreas[i];
            }

            var capacity = (long)tree.Width * tree.Height;
            if (needed <= capacity)
            {
                packableTrees++;
            }
        }

        return packableTrees;
    }

    private object Part2()
    {
        throw new NotImplementedException();
    }

    private static Input Parse(string filePath)
    {
        // Important: do NOT ignore spaces. Only normalize Windows newlines.
        var lines = TextInputHelper.ReadLinesAsStringList(filePath, ignoredChars: ['\r']);

        var packages = new List<Present>();
        var commands = new List<ChristmasTree>();

        var i = 0;

        // Parse packages until we hit the command section (or EOF)
        while (i < lines.Count)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                i++;
                continue;
            }

            if (IsPresentHeader(lines[i]))
            {
                packages.Add(ParsePresentBlock(lines, ref i));
                continue;
            }

            // not a package header -> next section
            break;
        }

        // Parse ChristmasTree boundry
        while (i < lines.Count)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                i++;
                continue;
            }

            commands.Add(ParseChristmasTreeLine(lines[i]));
            i++;
        }

        return new Input(packages, commands);
    }

    private static bool IsPresentHeader(string line)
        => line.EndsWith(':') && int.TryParse(line[..^1], out _);

    private static Present ParsePresentBlock(List<string> lines, ref int i)
    {
        // Header: "0:"
        var header = lines[i].Trim();
        var indexText = header[..^1];
        var index = int.Parse(indexText);

        i++; // move past header

        // Grid lines: until blank line OR next header OR command line
        var area = 0;

        while (i < lines.Count)
        {
            var line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                break;

            if (IsPresentHeader(line))
                break;

            if (LooksLikeChristmasTree(line))
                break;

            // Count filled cells directly, no bool[,] needed
            for (var c = 0; c < line.Length; c++)
            {
                if (line[c] != '#') continue;
                area++;
            }

            i++;
        }

        return new Present(index, area);
    }

    private static bool LooksLikeChristmasTree(string line)
    {
        // "4x4: 0 0 0" -> has ':' and "WxH" before it
        var colon = line.IndexOf(':');
        if (colon < 0) return false;

        var dims = line[..colon].Trim();
        var x = dims.IndexOf('x');
        if (x <= 0 || x >= dims.Length - 1) return false;

        return int.TryParse(dims[..x], out _) && int.TryParse(dims[(x + 1)..], out _);
    }

    private static ChristmasTree ParseChristmasTreeLine(string line)
    {
        var colon = line.IndexOf(':');
        if (colon < 0)
        {
            return new ChristmasTree(0, 0, Array.Empty<int>());
        }

        var dims = line[..colon].Trim(); // "4x4"
        var payload = line[(colon + 1)..].Trim(); // "0 0 0 0 2 0"

        var x = dims.IndexOf('x');
        if (x < 0)
        {
            return new ChristmasTree(0, 0, Array.Empty<int>());
        }

        var width = int.Parse(dims[..x]);
        var height = int.Parse(dims[(x + 1)..]);

        var values = payload
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();

        return new ChristmasTree(width, height, values);

    }

    private record Present(int Index, int Area);

    private record ChristmasTree(int Width, int Height, int[] Values);

    private record Input(List<Present> Presents, List<ChristmasTree> ChristmasTrees);
}
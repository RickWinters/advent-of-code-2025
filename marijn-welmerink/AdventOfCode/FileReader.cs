namespace AdventOfCode;

/// <summary>
/// Provides functionality to read files.
/// </summary>
public static class FileReader
{
    /// <summary>
    /// Reads all lines from the specified file.
    /// </summary>
    /// <param name="targetInputFile">The target file to read from.</param>
    /// <param name="isCommaSeparated">Whether the file content is comma-separated.</param>
    /// <returns>
    /// A read-only list of strings representing the lines or values from the file, or null if an error occurs.
    /// </returns>
    public static IReadOnlyList<string>? ReadAllLines(InputFile targetInputFile, bool isCommaSeparated)
    {
        var filePathString = FilePathToString(targetInputFile);

        try
        {
            if (!isCommaSeparated)
            {
                return File.ReadAllLines(filePathString);
            }

            var content = File.ReadAllText(filePathString);

            return content.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
        catch (Exception)
        {
            Console.WriteLine($"ERR: Could not read file at path '{filePathString}'.");

            return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="InputFile"/> enum value to its corresponding file path string.
    /// </summary>
    /// <param name="inputFile">The file enum value.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the file enum value is not recognized.</exception>
    private static string FilePathToString(InputFile inputFile)
    {
        var assetFolder = Path.Combine(AppContext.BaseDirectory, "Assets");

        return inputFile switch
        {
            InputFile.Day1Input => Path.Combine(assetFolder, "day-1-input.txt"),
            InputFile.Day2Input => Path.Combine(assetFolder, "day-2-input.txt"),
            _ => throw new ArgumentOutOfRangeException(nameof(inputFile), inputFile, null)
        };
    }
}

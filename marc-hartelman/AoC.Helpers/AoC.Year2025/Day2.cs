using AoC.Helpers.Helpers;

namespace AoC.Year2025;

public class Day2(string dayPath) : DayBase(2, dayPath)
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

    private long Part1()
    {
        long inValidIds = 0;
        
        var delimited = TextInputHelper.ReadAsStringList(DayPath, ',');
        var ranges = delimited.Select(x => new Range(long.Parse(x[..x.IndexOf('-')]), long.Parse(x[(x.IndexOf('-') + 1)..]))).ToList();

        foreach (var range in ranges)
        {
            for (var i = range.Begin; i <= range.End; i++)
            {
                if (i.ToString().Length % 2 != 0 || i <= 9)
                {
                    continue;
                }

                var text = i.ToString();
                var mid = text.Length / 2;

                var firstHalf = text[..mid];
                var secondHalf = text[mid..];

                if (firstHalf != secondHalf)
                {
                    continue;
                }

                inValidIds += i;
                ConsoleWrite($"valid id found: {i}");
            }
        }

        return inValidIds;
    }

    private long Part2()
    {
        long inValidIds = 0;
        
        var delimited = TextInputHelper.ReadAsStringList(DayPath, ',');
        var ranges = delimited.Select(x => new Range(long.Parse(x[..x.IndexOf('-')]), long.Parse(x[(x.IndexOf('-') + 1)..]))).ToList();
        
        foreach (var range in ranges)
        {
            for (var i = range.Begin; i <= range.End; i++)
            {
                // regex pattern couldve been used bool isPattern = Regex.IsMatch(s, @"^(.+)\1+$");
                
                // integers below 10 should be skippped
                if (i <= 9)
                {
                    continue;
                }

                var s = i.ToString();
                
                // position should only check half of the string
                for (var positionToCheck = 1; positionToCheck <= s.Length / 2; positionToCheck++)
                {
                    if (s.Length % positionToCheck != 0)
                    {
                        continue;
                    }

                    var isRepeating = true;

                    // check if the characters at the positionToCheck are the same loop through full string
                    for (var character = positionToCheck; character < s.Length; character++)
                    {
                        // mod check on the character position to ensure we don't go out of bounds'
                        // if position is 2 and character in 5 translates to 5 % 2 = 1 so 5th character is checked against the 1st one (2nd on in the array)
                        if (s[character] != s[character % positionToCheck])
                        {
                            isRepeating = false;
                            break;
                        }
                    }
                    
                    // if we found a repeating character break out of the loop
                    if (!isRepeating)
                    {
                        continue;
                    }

                    ConsoleWrite($"invalid id found: {i}");
                    inValidIds += i;
                    break;
                }
            }
        }
        
        return inValidIds;
    }

    private record Range(long Begin, long End);
}
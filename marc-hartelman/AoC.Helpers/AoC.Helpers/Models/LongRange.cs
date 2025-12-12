namespace AoC.Helpers.Models;

/// <summary>
/// Represents an inclusive range of long integers [Start, End].
/// </summary>
public readonly record struct LongRange(long Start, long End)
{
    public long Length => End - Start + 1;

    /// <summary>
    /// Checks if a value is inside the range (inclusive).
    /// </summary>
    public bool Contains(long value) => value >= Start && value <= End;

    /// <summary>
    /// Checks if this range overlaps with another range.
    /// </summary>
    public bool Intersects(LongRange other)
    {
        return Start <= other.End && End >= other.Start;
    }

    /// <summary>
    /// Returns the overlapping section of two ranges, or null if they don't overlap.
    /// </summary>
    public LongRange? GetIntersection(LongRange other)
    {
        return !Intersects(other) 
            ? null 
            : new LongRange(Math.Max(Start, other.Start), Math.Min(End, other.End));
    }
    
    public LongRange? GetUnion(LongRange other)
    {
        return new LongRange(Math.Min(Start, other.Start), Math.Max(End, other.End));
    }

    /// <summary>
    /// Offsets the range by a value (e.g., "Shift all seeds by +50").
    /// </summary>
    public LongRange Shift(long offset) => new(Start + offset, End + offset);
    
    /// <summary>
    /// Merges overlapping ranges into single range.
    /// </summary>
    public static List<LongRange> Merge(List<LongRange> ranges)
    {
        var sorted = ranges.OrderBy(r => r.Start).ToList();
        if (sorted.Count == 0)
        {
            return [];
        }

        var result = new List<LongRange>();
        
        // Pointer to current range
        var current = sorted[0];

        // Iterate over remaining ranges
        for (var i = 1; i < sorted.Count; i++)
        {
            var next = sorted[i];
            
            // If the next range overlaps with the current one, merge them
            // Don't Point current to next, as it may be intersected with the next range
            if (current.Intersects(next))
            {
                current = new LongRange(current.Start, Math.Max(current.End, next.End));
            }
            // Otherwise, add the current range to the result and point current to the next range
            else
            {
                result.Add(current);
                current = next;
            }
        }

        result.Add(current);
        return result;
    }
}
namespace AoC.Helpers.Models;

/// <summary>
/// A generic record to hold state for BFS/Dijkstra algorithms.
/// </summary>
/// <typeparam name="T">The type identifying the location (e.g., Point2D).</typeparam>
public record SearchState<T>(T Node, long Cost) : IComparable<SearchState<T>>
{
    public int CompareTo(SearchState<T>? other)
    {
        if (other is null)
        {
            return 1;
        }

        return Cost.CompareTo(other.Cost);
    }
}

public class StateKey(int[] values) : IEquatable<StateKey>
{
    public int[] Values { get; } = values;

    public bool Equals(StateKey? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || Values.SequenceEqual(other.Values);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as StateKey);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var v in Values) hash.Add(v);
        return hash.ToHashCode();
    }
}
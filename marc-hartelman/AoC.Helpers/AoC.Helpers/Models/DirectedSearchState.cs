namespace AoC.Helpers.Models;

/// <summary>
/// A specific state for "Crucible" style problems where direction matters.
/// </summary>
public record DirectedSearchState(Point2D Position, Direction Facing, long Cost) 
    : IComparable<DirectedSearchState>
{
    public int CompareTo(DirectedSearchState? other)
    {
        return other != null ? Cost.CompareTo(other.Cost) : 1;
    }
}
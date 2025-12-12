using AoC.Helpers.Models;

namespace AoC.Helpers.Helpers;

public static class PolygonHelper
{
    /// <summary>
    /// Calculates the Area of a polygon using the Shoelace Formula.
    /// Vertices must be ordered (clockwise or counter-clockwise).
    /// </summary>
    public static long CalculateArea(List<Point2D> vertices)
    {
        long area = 0;
        for (var i = 0; i < vertices.Count; i++)
        {
            var j = (i + 1) % vertices.Count;
            area += (long)vertices[i].X * vertices[j].Y;
            area -= (long)vertices[j].X * vertices[i].Y;
        }
        return Math.Abs(area) / 2;
    }

    /// <summary>
    /// Calculates the number of integer points *strictly inside* the polygon using Pick's Theorem.
    /// Derived from: Area = Interior + (Boundary / 2) - 1
    /// </summary>
    /// <param name="area">Area from Shoelace formula.</param>
    /// <param name="boundaryPoints">The number of points on the perimeter (usually the sum of steps taken).</param>
    private static long CalculateInteriorPoints(long area, long boundaryPoints)
    {
        return area - (boundaryPoints / 2) + 1;
    }
    
    /// <summary>
    /// Calculates total area covered by the loop (Interior + Boundary).
    /// Useful for "Lava lagoon" style problems where the trench itself counts as volume.
    /// </summary>
    public static long CalculateTotalArea(long area, long boundaryPoints)
    {
        return CalculateInteriorPoints(area, boundaryPoints) + boundaryPoints;
    }
}
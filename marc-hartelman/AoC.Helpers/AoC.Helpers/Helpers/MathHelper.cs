namespace AoC.Helpers.Helpers;

public static class MathHelper
{
    /// <summary>
    /// Calculates the Greatest Common Divisor (GCD) of two numbers.
    /// </summary>
    private static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// <summary>
    /// Calculates the Least Common Multiple (LCM) of two numbers.
    /// </summary>
    private static long Lcm(long a, long b)
    {
        return (a / Gcd(a, b)) * b;
    }

    /// <summary>
    /// Calculates the LCM of a collection of numbers.
    /// Essential for "Cycle detection" problems where multiple independent cycles align.
    /// </summary>
    public static long Lcm(IEnumerable<long> numbers)
    {
        return numbers.Aggregate(Lcm);
    }
    
    /// <summary>
    /// Calculates the Manhattan Distance between two points (x,y).
    /// |x1 - x2| + |y1 - y2|
    /// </summary>
    public static int ManhattanDistance((int x, int y) p1, (int x, int y) p2)
    {
        return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
    }
    
    /// <summary>
    /// Calculates the Manhattan Distance for 3D points (x,y,z).
    /// </summary>
    public static int ManhattanDistance((int x, int y, int z) p1, (int x, int y, int z) p2)
    {
        return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y) + Math.Abs(p1.z - p2.z);
    }
}
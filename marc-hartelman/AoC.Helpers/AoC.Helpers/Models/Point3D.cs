namespace AoC.Helpers.Models;

public readonly record struct Point3D(int X, int Y, int Z)
{
    public static Point3D Zero => new(0, 0, 0);

    public static Point3D operator +(Point3D a, Point3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Point3D operator -(Point3D a, Point3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    
    public int ManhattanDistance(Point3D other) => 
        Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

    public long ShortLineDistance(Point3D other)
    {
        long dx = X - other.X;
        long dy = Y - other.Y;
        long dz = Z - other.Z;
        var distSq = dx * dx + dy * dy + dz * dz;
        return distSq;
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
}
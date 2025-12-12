using AoC.Helpers.Helpers;
using AoC.Helpers.Models;

namespace AoC.Year2025;

public class Day9(string dayPath) : DayBase(9, dayPath)
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

    private object Part1()
    {
        // no need for explanation 
        var corners = TextInputHelper.ReadLinesAsList(DayPath,
            x => new Point2D(int.Parse(x.Split(",")[0]), int.Parse(x.Split(",")[1])));

        var rectangles = new List<Rect>();

        for (var i = 0; i < corners.Count; i++)
        {
            for (var j = 0; j < corners.Count; j++)
            {
                var p1 = corners[i];
                var p2 = corners[j];
                rectangles.Add(Rect.FromPoints([p1, p2]));
            }
        }

        var max = rectangles.Max(r => r.Area);
        return max;
    }

    // Raycasting
    private object Part2()
    {
        var corners = TextInputHelper.ReadLinesAsList(DayPath,
            x => new Point2D(int.Parse(x.Split(",")[0]), int.Parse(x.Split(",")[1])));

        List<Rect> rectangles = new List<Rect>();

        for (var i = 0; i < corners.Count; i++)
        {
            for (var j = 0; j < corners.Count; j++)
            {
                var p1 = corners[i];
                var p2 = corners[j];
                rectangles.Add(Rect.FromPoints([p1, p2]));
            }
        }

        var cornerCount = corners.Count;

        // Round trip, get all vertical lines X is the row, topY and bottomY is the top and bottom corner
        // i is current corner i + 1 is next corner, i + 1 % count used to get the last value on the first value in the loop
        var verticalLines = Enumerable.Range(0, cornerCount).Where(i => corners[i].X == corners[(i + 1) % cornerCount].X)
            .Select(i => (row: corners[i].X, topY: Math.Min(corners[i].Y, corners[(i + 1) % cornerCount].Y),
                bottomY: Math.Max(corners[i].Y, corners[(i + 1) % cornerCount].Y))).ToArray();
        
        // Round trip, get all horizontal lines Y is the col, leftX and rightX is the left and right corner
        // i is current corner i + 1 is next corner, i + 1 % count used to get the last value on the first value in the loop
        var horizontalLines = Enumerable.Range(0, cornerCount).Where(i => corners[i].Y == corners[(i + 1) % cornerCount].Y)
            .Select(i => (col: corners[i].Y, leftX: Math.Min(corners[i].X, corners[(i + 1) % cornerCount].X),
                rightX: Math.Max(corners[i].X, corners[(i + 1) % cornerCount].X))).ToArray();

        
        // Point can be on one of the lines, if so return true.
        bool OnLine((long row, long col) point)
        {
            return horizontalLines.Any(line => line.col == point.col && point.row >= line.leftX && point.row <= line.rightX) ||
                   verticalLines.Any(line => line.row == point.row && point.col >= line.topY && point.col <= line.bottomY);
        }

        // When point is on line we return true or
        // if count % 2 == 1 we know we are inside, since we saw the wall on top and wall on bottom. we need to see the wall on the right once
        // this is why the modulo checks on uneven.
        bool InsidePolygon((long row, long col) point)
        {
            int count = 0;
            foreach (var line in verticalLines)
            {
                // loop over every vertical line
                // check if point is inside the wall, col > topY, check if bottomY is bigger than point, then you are between the lines.
                // check if the point sees a right wall.
                // all conditions check out, we are inside the wall
                if (line.topY > point.col != line.bottomY > point.col && point.row < line.row)
                {
                    count++;
                }
            }
            
            return OnLine(point) || count % 2 == 1;
        }

        // check if point crosses Y value can happen after raycasting, when there is a U or C wall
        bool VerticalCornerCrossesY(long leftX, long rightX, long col)
        {
            return verticalLines.Any(e => e.row > leftX && e.row < rightX && col > e.topY && col < e.bottomY);
        }

        // check if point crosses X value can happen after raycasting, when there is a U or C wall
        bool VerticalCornerCrossesX(long upperY, long lowerY, long row)
        {
            return horizontalLines.Any(e => e.col > upperY && e.col < lowerY && row > e.leftX && row < e.rightX);
        }

        // fully check if our rectangle fits in the provided polygon (the carpet)
        bool RectangleInPolygon(Rect rectangle)
        {
            return InsidePolygon((rectangle.Left, rectangle.Top)) 
                   && InsidePolygon((rectangle.Left, rectangle.Bottom)) 
                   && InsidePolygon((rectangle.Right, rectangle.Top)) 
                   && InsidePolygon((rectangle.Right, rectangle.Bottom))
                   && !VerticalCornerCrossesY(rectangle.Left, rectangle.Right, rectangle.Top) 
                   && !VerticalCornerCrossesY(rectangle.Left, rectangle.Right, rectangle.Bottom)
                   && !VerticalCornerCrossesX(rectangle.Top, rectangle.Bottom, rectangle.Left) 
                   && !VerticalCornerCrossesX(rectangle.Top, rectangle.Bottom, rectangle.Right);
        }

        var maxArea = 0L;
        
        foreach (var rectangle in rectangles)
        {
            if (RectangleInPolygon(rectangle))
            {
                if (rectangle.Area > maxArea)
                {
                    maxArea = rectangle.Area;
                }
            }
        }
            
        return maxArea;
    }
}
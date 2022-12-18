namespace AdventOfCode2022.Day14;

public class Day14Processor: ProcessorBase
{
    // ReSharper disable InconsistentNaming
    private const int SAND_START_X = 500;

    #region CavePoint
    public enum PointType
    {
        None,
        Rock,
        Sand
    }

    public struct CavePoint : IEquatable<CavePoint>
    {
        public CavePoint(int x, int y, PointType pointType)
        {
            X = x;
            Y = y;
            PointType = pointType;
        }
        
        public int X { get; }

        public int Y { get; }
        
        public PointType PointType { get; }
        
        //the PointType does not from part of the comparison for equality so that we don't have more than one type on the same point
        public static bool operator ==(CavePoint left, CavePoint right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(CavePoint left, CavePoint right) => !(left == right);
        
        public bool Equals(CavePoint other)
        {
            return this == other;
        }

        public override bool Equals(object? obj)
        {
            return obj is CavePoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
    #endregion
    
    protected override void Silver(string[] lines)
    {
        var (cave, min, max) = GetCaveScan(lines);
        var sandDrops = CountSandDrops(cave, SAND_START_X, min, max, false);
        //RenderCave(cave, SAND_START_X);
        
        Console.WriteLine($"Number of units of sand that come to rest: {sandDrops}");
    }

    protected override void Gold(string[] lines)
    {
        var (cave, min, max) = GetCaveScan(lines);
        var sandDrops = CountSandDrops(cave, SAND_START_X, min, max, true);
        
        Console.WriteLine($"Number of units of sand that come to rest on floor: {sandDrops}");
    }

    private static int CountSandDrops(HashSet<CavePoint> cave, int sandStartX, CavePoint min, CavePoint max, bool hasFloor)
    {
        var sandCount = 0;
        var canDrop = true;
        while (canDrop)
        {
            canDrop = DropSingleSand(cave, sandStartX, 0, min, max, hasFloor);
            if (canDrop)
                sandCount++;
        }

        return sandCount;
    }

    private static bool DropSingleSand(HashSet<CavePoint> cave, int x, int startY, CavePoint min, CavePoint max, bool hasFloor)
    {
        var y = startY;
        var floorY = max.Y + 2;
        while (true)
        {
            if (!IsTileBlocked(cave, x, y) && (!hasFloor || y < floorY))
            {
                y++;
                if (y > max.Y && !hasFloor)
                    return false; //fell off the bottom or hit the floor
                continue;
            }
            
            //we hit something
            if (hasFloor && y == 0)
            {
                //we reached the top
                return false;
            }

            if (!hasFloor || y < floorY)
            {
                if (x == min.X && !hasFloor)
                {
                    //falls off, can't go left - infinity and beyond!
                    return false;
                }

                //check if we can go left
                if (!IsTileBlocked(cave, x - 1, y))
                {
                    //we can go left
                    x--;
                    continue;
                }

                if (x == max.X && !hasFloor)
                {
                    //currently in the right most column, can't go right
                    return false;
                }

                //check if we can go right
                if (!IsTileBlocked(cave, x + 1, y))
                {
                    //we can go right
                    x++;
                    continue;
                }
            }

            //we need to come to rest
            cave.Add(new CavePoint(x, y - 1, PointType.Sand));
            return true;
        }
    }

    private static bool IsTileBlocked(HashSet<CavePoint> cave, int x, int y)
    {
        var checkPoint = new CavePoint(x, y, PointType.None);
        return cave.TryGetValue(checkPoint, out _);
    }
    
    private static (HashSet<CavePoint> cave, CavePoint min, CavePoint max) GetCaveScan(string[] lines)
    {
        var data = GetPathSegments(lines);

        var cave = new HashSet<CavePoint>();

        //populate rock
        foreach (var segment in data.segments)
        {
            if (segment.from.Y == segment.to.Y)
            {
                //horizontal segment
                
                var fromX = segment.from.X;
                var toX = segment.to.X;
                var xFactor = fromX <= toX ? 1 : -1;
                
                for (var x = fromX; fromX <= toX && x <= toX || fromX > toX && x >= toX; x += xFactor)
                {
                    cave.Add(new CavePoint(x, segment.from.Y, PointType.Rock));
                }
            }
            else
            {
                //vertical segment
                var fromY = segment.from.Y;
                var toY = segment.to.Y;
                var yFactor = fromY <= toY ? 1 : -1;
                var x = segment.from.X;
                
                for (var y = fromY; fromY <= toY && y <= toY || fromY > toY && y >= toY; y += yFactor)
                {
                    cave.Add(new CavePoint(x, y, PointType.Rock));
                }
            }
        }

        return (cave, data.min, data.max);
    }

    private static (List<(CavePoint from, CavePoint to)> segments, CavePoint min, CavePoint max) GetPathSegments(string[] lines)
    {
        var result = new List<(CavePoint, CavePoint)>();
        var minX = 999999;
        var maxX = 0;
        var minY = 999999;
        var maxY = 0;

        foreach (var line in lines)
        {
            var pos = 0;
            var idx = line.IndexOf(" ", StringComparison.InvariantCulture);
            CavePoint? lastPoint = null;
            while (idx > 0)
            {
                var coords = line.Substring(pos, idx - pos)
                    .Trim()
                    .Split(",")
                    .Select(int.Parse)
                    .ToList();
                var point = new CavePoint(coords[0], coords[1], PointType.Rock);
                if (lastPoint.HasValue)
                {
                    result.Add((lastPoint.Value, point));
                }
                lastPoint = point;
                if (point.X > maxX)
                    maxX = point.X;
                if (point.X < minX)
                    minX = point.X;
                if (point.Y > maxY)
                    maxY = point.Y;
                if (point.Y < minY)
                    minY = point.Y;
                
                //after arrow
                idx = line.IndexOf(" ", idx + 1, StringComparison.InvariantCulture);
                pos = idx + 1;
                
                //get end of next point
                idx = line.IndexOf(" ", pos, StringComparison.InvariantCulture);
            }

            //last one
            var lastCoords = line.Substring(pos)
                .Trim()
                .Split(",")
                .Select(int.Parse)
                .ToList();
            var p2 = new CavePoint(lastCoords[0], lastCoords[1], PointType.Rock);
            if (lastPoint.HasValue)
            {
                result.Add((lastPoint.Value, p2));
            }
            if (p2.X > maxX)
                maxX = p2.X;
            if (p2.X < minX)
                minX = p2.X;
            if (p2.Y > maxY)
                maxY = p2.Y;
            if (p2.Y < minY)
                minY = p2.Y;
        }

        return (result, new CavePoint(minX, minY, PointType.None), new CavePoint(maxX, maxY, PointType.None));
    }
    
    private static void RenderCave(HashSet<CavePoint> cave, int sandStartX)
    {
        var maxY = cave.Select(a => a.Y).Max();
        var allX = cave.Select(a => a.X).ToList();
        var minX = allX.Min();
        var maxX = allX.Max();
        for (var y = 0; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var symbol = ".";
                if (x == sandStartX && y == 0)
                    symbol = "+";
                else
                {
                    var comp = new CavePoint(x, y, PointType.None);
                    if (cave.TryGetValue(comp, out var cavePoint))
                    {
                        switch (cavePoint.PointType)
                        {
                            case PointType.Rock:
                                symbol = "#";
                                break;
                            case PointType.Sand:
                                symbol = "o";
                                break;
                        }
                    }
                }
              
                Console.Write(symbol);
            }
            Console.WriteLine();
        }
    }
}
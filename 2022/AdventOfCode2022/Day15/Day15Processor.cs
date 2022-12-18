using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace AdventOfCode2022.Day15;

public class Day15Processor: ProcessorBase
{
    // ReSharper disable once InconsistentNaming
    private const int ROW_TO_INVESTIGATE = 2000000;
    private const int LOWER_BOUND = 0;
    private const int UPPER_BOUND = 4000000;
    
    protected override void Silver(string[] lines)
    {
        var sensorsWithBeacons = GetSensorsWithBeacons(lines);
        var locationsWithoutBeacon = KnownLocationsWithoutBeacon(sensorsWithBeacons, ROW_TO_INVESTIGATE);

        var known = locationsWithoutBeacon.Count(a => a.Y == ROW_TO_INVESTIGATE);
        Console.WriteLine($"The number of positions that cannot contain a beacon at y={ROW_TO_INVESTIGATE}: {known}");
    }

    protected override void Gold(string[] lines)
    {
        var sensorsWithBeacons = GetSensorsWithBeacons(lines);

        Point? beacon = null;
        var range = UPPER_BOUND - LOWER_BOUND + 1;
        for (var y = LOWER_BOUND; y <= UPPER_BOUND; y++)
        {
            var beaconX = FindBeaconX(sensorsWithBeacons, y, LOWER_BOUND, UPPER_BOUND);
            if (beaconX.HasValue)
            {
                //beacon is on this row
                beacon = new Point(beaconX.Value, y);
                break;
            }
        }

        BigInteger tuningFrequency = beacon.Value.X;
        tuningFrequency *= 4000000;
        tuningFrequency += beacon.Value.Y;
        
        Console.WriteLine($"The tuning frequency of the emergency beacon (at {beacon.Value.X},{beacon.Value.Y}) is: {tuningFrequency}");
    }

    private HashSet<Point> KnownLocationsWithoutBeacon(List<(Point sensor, Point beacon)> sensorsWithBeacons, int targetY)
    {
        var points = sensorsWithBeacons.Select(a => a.sensor)
            .Union(sensorsWithBeacons.Select(a => a.beacon)).ToHashSet();
        var result = new HashSet<Point>();
        foreach (var values in sensorsWithBeacons)
        {
            var distance = Math.Abs(values.sensor.X - values.beacon.X) + Math.Abs(values.sensor.Y - values.beacon.Y);

            var lowerX = values.sensor.X - distance;
            var upperX = values.sensor.X + distance;
            var lowerY = values.sensor.Y - distance;
            var upperY = values.sensor.Y + distance;

            if (lowerY > targetY || targetY > upperY)
                continue;
            
            var startX = lowerX;
            var endX = upperX;

            var y = 0;
            while (startX <= endX)
            {
                if (values.sensor.Y + y == targetY || values.sensor.Y - y == targetY)
                {
                    for (var x = startX; x <= endX; x++)
                    {
                        if (values.sensor.Y + y == targetY)
                        {
                            var point = new Point(x, values.sensor.Y + y);
                            if (!points.Contains(point))
                            {
                                result.Add(point);
                            }
                        }

                        if (y > 0 && values.sensor.Y - y == targetY)
                        {
                            var point = new Point(x, values.sensor.Y - y);
                            if (!points.Contains(point))
                            {
                                result.Add(point);    
                            }
                        }
                    }
                }

                startX++;
                endX--;
                y++;
            }
        }

        return result;
    }
    
    private int? FindBeaconX(List<(Point sensor, Point beacon)> sensorsWithBeacons, int targetY, int min, int max)
    {
        var rowRanges = new List<(int fromX, int toX)>();
        var sw = Stopwatch.StartNew();
        foreach (var values in sensorsWithBeacons)
        {
            var distance = Math.Abs(values.sensor.X - values.beacon.X) + Math.Abs(values.sensor.Y - values.beacon.Y);

            var lowerY = values.sensor.Y - distance;
            var upperY = values.sensor.Y + distance;

            if (lowerY > targetY || targetY > upperY)
                continue;
            
            var lowerX = values.sensor.X - distance;
            var upperX = values.sensor.X + distance;
            
            if ((lowerX < min && upperX < min) ||
                (lowerX > max && upperX > max))
                continue;
            
            var y = Math.Abs(values.sensor.Y - targetY);
            var startX = lowerX + y;
            var endX = upperX - y;
            
            if (startX < min)
                startX = min;
            if (endX > max)
                endX = max;
            
            rowRanges.Add((startX, endX));
        }

        var x = sw.ElapsedMilliseconds;
        if (rowRanges.Count == 0)
            return null;

        MergeOverlaps(rowRanges);

        if (rowRanges.Count > 1)
        {
            rowRanges.Sort((a, b) => a.fromX.CompareTo(b.fromX));
            for(var r = 0; r <= rowRanges.Count - 2; r++)
            {
                var a = rowRanges[r];
                var b = rowRanges[r + 1];

                if (r == 0 && a.fromX > min)
                {
                    return a.fromX - 1;
                }

                if (r == rowRanges.Count - 2 && b.toX < max)
                {
                    return b.toX + 1;
                }

                if (b.fromX - a.toX > 1)
                {
                    return a.toX + 1;
                }
            }
        }
        else
        {
            var range = rowRanges.First();
            if (range.fromX > min)
            {
                return range.fromX - 1;
            }  
            if (range.toX < max)
            {
                return range.toX + 1;
            }
        }

        return null;
    }

    private static void MergeOverlaps(List<(int fromX, int toX)> rowRanges)
    {
        var hasOverlaps = rowRanges
            .Where(src => rowRanges.Count(a => a != src && ((a.fromX <= src.fromX && src.fromX <= a.toX) ||
                                               (a.fromX <= src.toX && src.toX <= a.toX) ||
                                               (src.fromX <= a.fromX && a.toX <= src.toX))) > 0)
            .ToList();
        var def = (0, 0);
        while (hasOverlaps.Count > 0)
        {
            var item = hasOverlaps.First();
            var startX = item.fromX;
            var endX = item.toX;
            var overlap = rowRanges.FirstOrDefault(a => a != item && ((a.fromX <= startX && startX <= a.toX) ||
                                                         (a.fromX <= endX && endX <= a.toX) ||
                                                         (startX <= a.fromX && a.toX <= endX)));
            if (overlap != def)
            {
                if (startX <= overlap.fromX || overlap.toX <= endX)
                {
                    //merge ranges
                    rowRanges.Remove(overlap);
                    rowRanges.Remove(item);
                    if (startX <= overlap.fromX && endX >= overlap.toX)
                    {
                        //the new ranges replaces the old
                        rowRanges.Add((startX, endX));
                    }
                    else if (startX <= overlap.fromX)
                    {
                        //extend the existing range
                        rowRanges.Add((startX, overlap.toX));
                    } 
                    else if (endX >= overlap.toX)
                    {
                        //extend the existing range
                        rowRanges.Add((overlap.fromX, endX));
                    }
                }
                else
                {
                    //outer one contains this one
                    rowRanges.Remove(item);
                }
            }

            hasOverlaps = rowRanges
                .Where(src => rowRanges.Count(a => a != src && ((a.fromX <= src.fromX && src.fromX <= a.toX) ||
                                                                (a.fromX <= src.toX && src.toX <= a.toX) ||
                                                                (src.fromX <= a.fromX && a.toX <= src.toX))) > 0)
                .ToList();
        }
    }

    private List<(Point sensor, Point beacon)> GetSensorsWithBeacons(string[] lines)
    {
        var result = new List<(Point, Point)>();
        foreach (var line in lines)
        {
            var idx = line.IndexOf(":", StringComparison.InvariantCulture);
            var coords = line.Substring(0, idx)
                .Replace("Sensor at x=", "")
                .Replace(" y=", "")
                .Split(",")
                .Select(int.Parse)
                .ToArray();

            var sensor = new Point(coords[0], coords[1]);

            idx = line.IndexOf("closest", StringComparison.InvariantCulture);
            coords = line.Substring(idx)
                .Replace("closest beacon is at x=", "")
                .Replace(" y=", "")
                .Split(",")
                .Select(int.Parse)
                .ToArray();

            var beacon = new Point(coords[0], coords[1]);
            result.Add((sensor, beacon));
        }

        return result;
    }
}
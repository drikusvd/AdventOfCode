namespace AdventOfCode2022.Day4;

public class Day4Processor: ProcessorBase
{
    protected override void Silver(string[] lines)
    {
        var fullyContainsCount = 0;
        foreach (var line in lines)
        {
            var ranges = GetRanges(line);
            if (RangeContainsOther(ranges[0], ranges[1]))
                fullyContainsCount++;
        }

        Console.WriteLine($"Ranges fully containing another: {fullyContainsCount}");
    }

    protected override void Gold(string[] lines)
    {
        var overlapCount = 0;
        foreach (var line in lines)
        {
            var ranges = GetRanges(line);
            if (RangesOverlap(ranges[0], ranges[1]))
                overlapCount++;
        }

        Console.WriteLine($"Ranges overlapping: {overlapCount}");
    }

    private static List<int[]> GetRanges(string line)
    {
        var split = line.Split(",");
        var range1 = split[0];
        var range2 = split[1];

        return new List<int[]>() { GetRange(range1), GetRange(range2) };
    }

    private static int[] GetRange(string rangePart)
    {
        var split = rangePart.Split("-");
        return new[] { int.Parse(split[0]), int.Parse(split[1]) };
    }

    private static bool RangeContainsOther(int[] range1, int[] range2)
    {
        var range1Min = range1[0];
        var range1Max = range1[1];
        var range2Min = range2[0];
        var range2Max = range2[1];

        if (range2Min <= range1Max && range2Min >= range1Min &&
            range2Max <= range1Max && range2Max >= range1Min)
        {
            return true;
        } else if (range1Min <= range2Max && range1Min >= range2Min &&
                   range1Max <= range2Max && range1Max >= range2Min)
        {
            return true;
        }

        return false;
    }

    private static bool RangesOverlap(int[] range1, int[] range2)
    {
        var range1Min = range1[0];
        var range1Max = range1[1];
        var range2Min = range2[0];
        var range2Max = range2[1];
        
        if ((range2Min <= range1Max && range2Min >= range1Min) ||
            (range2Max <= range1Max && range2Max >= range1Min))
        {
            return true;
        } else if ((range1Min <= range2Max && range1Min >= range2Min) ||
                   (range1Max <= range2Max && range1Max >= range2Min))
        {
            return true;
        }

        return false;
    }
}
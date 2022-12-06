namespace AdventOfCode2022.Day3;

public class Day3Processor: ProcessorBase
{
    protected override void Silver(string[] lines)
    {
        var total = 0;
        foreach (var line in lines)
        {
            var lineVal = GetLineValue(line);
            total += lineVal;
        }

        Console.WriteLine($"The sum of the common item types: {total}");
    }

    protected override void Gold(string[] lines)
    {
        var total = 0;
        for (var r = 0; r < lines.Length; r+=3)
        {
            var elf1 = lines[r];
            var elf2 = lines[r + 1];
            var elf3 = lines[r + 2];
            var common1 = GetCommonChars(elf1, elf2);
            var common2 = GetCommonChars(elf1, elf3);
            var totalCommon = GetCommonChars(string.Concat(common1), string.Concat(common2));

            var badge = 0;
            foreach (var c in totalCommon)
            {
                var cVal = GetCharVal(c);
                if (cVal > badge)
                    badge = cVal;
            }

            total += badge;
        }
        
        Console.WriteLine($"The sum of the badges: {total}");
    }

    private static int GetLineValue(string line)
    {
        var common = GetCommonChars(line);
        var total = 0;
        foreach (var c in common)
        {
            var charVal = GetCharVal(c);
            total += charVal;
        }

        return total;
    }

    private static int GetCharVal(char c)
    {
        var val = (int)c;
        var charVal = 0;
        if (val >= 97)
            charVal = val - 96;
        else if (val >= 65) //uppercase
            charVal = (val - 64) + 26;
        return charVal;
    }

    private static List<char> GetCommonChars(string line)
    {
        var half = line.Length / 2;
        var part1 = line.Substring(0, half);
        var part2 = line.Substring(half);

        return GetCommonChars(part1, part2);
    }

    private static List<char> GetCommonChars(string part1, string part2)
    {
        var common = new List<char>();
        foreach (var c in part1)
        {
            if (part2.IndexOf(c) >= 0 && !common.Contains(c))
                common.Add(c);
        }

        return common;
    }
}
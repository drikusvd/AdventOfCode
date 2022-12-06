namespace AdventOfCode2022.Day6;

public class Day6Processor: ProcessorBase
{
    protected override void Silver(string[] lines)
    {
        var line = lines[0];
        var result = GetCharacterCountUntilMarker(line, 4);

        Console.WriteLine($"The number of characters until the start-of-packet marker is: {result}");
    }

    protected override void Gold(string[] lines)
    {
        var line = lines[0];
        var result = GetCharacterCountUntilMarker(line, 14);

        Console.WriteLine($"The number of characters until the start-of-message marker is: {result}");
    }

    private static int GetCharacterCountUntilMarker(string line, int distinctCharacterCountForMarker)
    {
        var str = "";
        var result = 0;
        for (var c = 0; c < line.Length; c++)
        {
            str += line[c];
            while (str.Length > distinctCharacterCountForMarker)
            {
                str = str.Substring(1);
            }

            var charCount = str.ToCharArray().Distinct().Count();
            if (charCount == distinctCharacterCountForMarker)
            {
                result = c + 1;
                break;
            }
        }

        return result;
    }
}
namespace AdventOfCode2022.Day1;

public class Day1Processor: ProcessorBase
{

    protected override void Silver(string[] lines)
    {
        var elfLoads = GetElfLoads(lines);
        
        var elfWithMost = 0;
        var mostCalories = 0;
        for(var i = 0; i < elfLoads.Count; i++)
        {
            var total = elfLoads[i].Sum();
            if (total > mostCalories)
            {
                mostCalories = total;
                elfWithMost = i + 1;
            }
        }
        
        Console.WriteLine($"The elf with the most calories of food is: {elfWithMost} with total calories of {mostCalories}");
    }

    protected override void Gold(string[] lines)
    {
        var elfLoads = GetElfLoads(lines);
        
        var totalsByElf = new List<int>();
        for(var i = 0; i < elfLoads.Count; i++)
        {
            var total = elfLoads[i].Sum();
            totalsByElf.Add(total);
        }

        var top3 = totalsByElf.OrderByDescending(a => a).Take(3).ToList();
        Console.WriteLine($"The top 3 totals are: {string.Join(',', top3.Select(a => a.ToString()))}. Total of the top 3 elves is: {top3.Sum()}");
    }

    private static List<List<int>> GetElfLoads(string[] lines)
    {
        var elfLoads = new List<List<int>>();
        var startNewElf = true;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                startNewElf = true;
            }
            else
            {
                var value = int.Parse(line);
                if (startNewElf)
                {
                    elfLoads.Add(new List<int>());
                    startNewElf = false;
                }

                var curElf = elfLoads.Last();
                curElf.Add(value);
            }
        }

        return elfLoads;
    }
}
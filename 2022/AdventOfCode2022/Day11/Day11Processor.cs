using System.Text;

namespace AdventOfCode2022.Day11;

public class Day11Processor: ProcessorBase
{
    private enum Operation
    {
        Add,
        Multiply
    }
    
    private class Monkey
    {
        public Queue<long> Items { get; set; } = new();
        public Operation WorryOperation { get; set; }
        public int OperationValue { get; set; }
        public int TestDivisibleBy { get; set; }
        public int TestTrueTarget { get; set; }
        public int TestFalseTarget { get; set; }
        public long TotalInspectionCount { get; set; } = 0;
    }
    
    protected override void Silver(string[] lines)
    {
        Evaluate(lines, 20, true);
    }

    protected override void Gold(string[] lines)
    {
        Evaluate(lines, 10000, false);
    }

    private void Evaluate(string[] lines, int rounds, bool withRelief)
    {
        var monkeys = InitMonkeys(lines);
        
        //do x rounds
        for (var r = 1; r <= rounds; r++)
        {
            DoRound(monkeys, withRelief);
        }

        Console.WriteLine($"After {rounds} rounds{(withRelief ? "" : " without relief")}:");
        for (var i = 0; i < monkeys.Count; i++)
        {
            Console.WriteLine($"Monkey {i} inspection count: {monkeys[i].TotalInspectionCount}");
        }

        var top2 = monkeys.ToList().OrderByDescending(a => a.TotalInspectionCount).Take(2).ToList();
        var monkeyBusiness = top2[0].TotalInspectionCount * top2[1].TotalInspectionCount;
        
        Console.WriteLine($"Top 2 inspection counts: {top2[0].TotalInspectionCount}, {top2[1].TotalInspectionCount}");
        Console.WriteLine($"The level of monkey business: {monkeyBusiness}");
        Console.WriteLine();
    }

    private static List<Monkey> InitMonkeys(string[] lines)
    {
        var result = new List<Monkey>();
        Monkey? curMonkey = null;
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("Monkey"))
            {
                curMonkey = new Monkey();
                result.Add(curMonkey);
            } 
            else if (trimmedLine.StartsWith("Starting items"))
            {
                var items = trimmedLine
                    .Replace("Starting items:", "")
                    .Trim()
                    .Split(", ")
                    .Select(long.Parse);
                foreach (var item in items)
                {
                    curMonkey.Items.Enqueue(item);
                }
            }
            else if (trimmedLine.StartsWith("Operation"))
            {
                var action = trimmedLine.Replace("Operation: new = old", "").Trim();
                curMonkey.WorryOperation = action.StartsWith("*") ? Operation.Multiply : Operation.Add;
                var actionValue = action.Substring(2);
                curMonkey.OperationValue = actionValue == "old" ? int.MaxValue : int.Parse(actionValue);
            }
            else if (trimmedLine.StartsWith("Test"))
            {
                curMonkey.TestDivisibleBy = int.Parse(trimmedLine.Replace("Test: divisible by", "").Trim());
            }
            else if (trimmedLine.StartsWith("If true"))
            {
                curMonkey.TestTrueTarget = int.Parse(trimmedLine.Replace("If true: throw to monkey", "").Trim());
            }
            else if (trimmedLine.StartsWith("If false"))
            {
                curMonkey.TestFalseTarget = int.Parse(trimmedLine.Replace("If false: throw to monkey", "").Trim());
            }
            else if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                curMonkey = null;
            }
        }

        return result;
    }

    private static void DoRound(List<Monkey> monkeys, bool reliefReduction)
    {
        long reductionMod = 1;
        foreach (var m in monkeys)
        {
            reductionMod *= m.TestDivisibleBy;
        }
        foreach (var m in monkeys)
        {
            while (m.Items.Count > 0)
            {
                var item = m.Items.Dequeue();
                
                m.TotalInspectionCount++;
                var value = GetNewWorryValue(m, item, reliefReduction, reductionMod);

                var throwTarget = GetTestTarget(m, value);
                monkeys[throwTarget].Items.Enqueue(value);  
            }
        }
    }

    private static long GetNewWorryValue(Monkey monkey, long value, bool reliefReduction, long reductionMod)
    {
        //apply operation
        var operationValue = monkey.OperationValue == int.MaxValue ? value : monkey.OperationValue;
        switch (monkey.WorryOperation)
        {
            case Operation.Add:
                value += operationValue;
                break;
            case Operation.Multiply:
                value *= operationValue;
                break;
        }
        
        //apply relief
        if (reliefReduction)
        {
            value /= 3;    
        }
        
        //ensure we don't have large values
        value %= reductionMod;

        return value;
    }

    private static int GetTestTarget(Monkey monkey, long value)
    {
        return value % monkey.TestDivisibleBy == 0 ? monkey.TestTrueTarget : monkey.TestFalseTarget;
    }
}
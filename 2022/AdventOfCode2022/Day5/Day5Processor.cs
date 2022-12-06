namespace AdventOfCode2022.Day5;

public class Day5Processor: ProcessorBase
{
    private class Instruction
    {
        public int Quantity { get; set; }
        public int FromStack { get; set; }
        public int ToStack { get; set; }

        public override string ToString()
        {
            return $"move {Quantity} from {FromStack} to {ToStack}";
        }
    }
    
    protected override void Silver(string[] lines)
    {
        var (stacks, instructions) = InitStacks(lines);

        foreach (var instruction in instructions)
        {
            ExecutionInstruction(stacks, instruction);
        }

        var result = "";
        for (var i = 0; i < stacks.Count; i++)
        {
            var top = stacks[i].Peek();
            result += top;
        }
        
        Console.WriteLine($"The top crates are: {result}");
    }

    protected override void Gold(string[] lines)
    {
        var (stacks, instructions) = InitStacks(lines);

        foreach (var instruction in instructions)
        {
            ExecutionInstructionRetainOrder(stacks, instruction);
        }

        var result = "";
        for (var i = 0; i < stacks.Count; i++)
        {
            var top = stacks[i].Peek();
            result += top;
        }
        
        Console.WriteLine($"The top crates are for CrateMover 9001: {result}");
    }

    private (List<Stack<string>> stacks, List<Instruction> instructions) InitStacks(string[] lines)
    {
        //find stacks start
        var numbersIndex = Array.IndexOf(lines, lines.FirstOrDefault(a => a.StartsWith(" 1")));
        var instructionsStart = numbersIndex + 2;

        var stacks = new List<Stack<string>?>();
        
        for (var l = numbersIndex - 1; l >= 0; l--)
        {
            var stacksLine = lines[l];

            var charPos = 0;
            Stack<string>? curStack = null;
            var curStackIndex = -1;
            while (charPos < stacksLine.Length)
            {
                var isStackStart = charPos % 4;
                if (isStackStart == 0)
                {
                    curStackIndex++;
                    
                    //start of stack
                    if (stacks.Count < curStackIndex + 1)
                    {
                        stacks.Add(new Stack<string>());
                    }
                    
                    curStack = stacks[curStackIndex];
                }

                if (stacksLine[charPos] == '[')
                {
                    var stackId = stacksLine[charPos + 1].ToString();
                    curStack.Push(stackId);
                } 
                else if (stacksLine[charPos] == ' ' || stacksLine[charPos] == ']')
                {
                    //done with stack
                    curStack = null;
                }

                charPos++;
            }
        }

        var instructions = GetInstructions(lines, instructionsStart);

        return (stacks, instructions);
    }

    private List<Instruction> GetInstructions(string[] lines, int instructionsStart)
    {
        var result = new List<Instruction>();
        for (var i = instructionsStart; i < lines.Length; i++)
        {
            var line = lines[i];
            line = line.Replace("move ", "");
            var idx = line.IndexOf(" ", StringComparison.InvariantCulture);
            var qty = int.Parse(line.Substring(0, idx));
            idx = line.IndexOf("from", StringComparison.InvariantCulture) + 5;
            var idx2 = line.IndexOf(" ", idx, StringComparison.InvariantCulture);
            var fromStack = int.Parse(line.Substring(idx, idx2 - idx));
            idx = line.IndexOf("to", StringComparison.InvariantCulture) + 3;
            var toStack = int.Parse(line.Substring(idx));

            var instr = new Instruction
            {
                Quantity = qty,
                FromStack = fromStack,
                ToStack = toStack
            };
            result.Add(instr);
        }

        return result;
    }

    private void ExecutionInstruction(List<Stack<string>> stacks, Instruction instruction)
    {
        var sourceStack = stacks[instruction.FromStack - 1];
        var targetStack = stacks[instruction.ToStack - 1];

        for (var i = 1; i <= instruction.Quantity; i++)
        {
            var item = sourceStack.Pop();
            targetStack.Push(item);
        }
    }

    private void ExecutionInstructionRetainOrder(List<Stack<string>> stacks, Instruction instruction)
    {
        var sourceStack = stacks[instruction.FromStack - 1];
        var targetStack = stacks[instruction.ToStack - 1];

        var items = new List<string>();
        for (var i = 1; i <= instruction.Quantity; i++)
        {
            var item = sourceStack.Pop();
            items.Add(item);
        }

        items.Reverse();
        foreach (var item in items)
        {
            targetStack.Push(item);    
        }
    }
}
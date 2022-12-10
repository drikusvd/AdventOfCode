namespace AdventOfCode2022.Day10;

public class Day10Processor: ProcessorBase
{
    protected override void Silver(string[] lines)
    {
        var x = 1;
        var currentCycle = 1;
        var totalSignalStrengths = 0;
        foreach (var instruction in lines)
        {
            if (instruction == "noop")
            {
                currentCycle++;
            }
            else
            {
                var addValue = int.Parse(instruction.Substring(5));
                if (IsSignalStrengthPoint(currentCycle + 1))
                {
                    totalSignalStrengths += x * (currentCycle + 1);
                    //Console.WriteLine($"Signal strength for cycle {currentCycle+1}: {x * (currentCycle + 1)} with X:{x} (Mid instruction {instruction})");
                }

                x += addValue;
                currentCycle += 2;
            }

            if (IsSignalStrengthPoint(currentCycle))
            {
                totalSignalStrengths += x * currentCycle;
                //Console.WriteLine($"Signal strength for cycle {currentCycle}: {x * currentCycle} with X:{x} (After instruction {instruction})");
            }

            if (currentCycle >= 220)
                break;
        }
        
        Console.WriteLine($"The total sum of the signal strengths is: {totalSignalStrengths}");
    }

    protected override void Gold(string[] lines)
    {
        Console.WriteLine("Display rendering:");
        var x = 1;
        var pixelPoint = 0;
        foreach (var instruction in lines)
        {
            pixelPoint = RenderPixelPoint(x, pixelPoint);
            
            if (instruction != "noop")
            {
                var addValue = int.Parse(instruction.Substring(5));

                pixelPoint = RenderPixelPoint(x, pixelPoint);

                x += addValue;
            }
        }
    }

    private static int RenderPixelPoint(int x, int pixelPoint)
    {
        if (pixelPoint >= x - 1 && pixelPoint <= x + 1)
        {
            //render 
            Console.Write("#");
        }
        else
        {
            Console.Write(".");
        }
        pixelPoint++;
        if (pixelPoint % 40 == 0)
        {
            pixelPoint = 0;
            Console.WriteLine();
        }

        return pixelPoint;
    }

    private static bool IsSignalStrengthPoint(int cycle)
    {
        return cycle == 20 || (cycle - 20) % 40 == 0;
    }
}
using System.Diagnostics;

namespace AdventOfCode2022.Day2;

public class Day2Processor: ProcessorBase
{
    private enum Shape
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    protected override void Silver(string[] lines)
    {
        var totalScore = 0;
        foreach (var line in lines)
        {
            var lineScore = GetLineScore(line);
            totalScore += lineScore;
        }
        Console.WriteLine($"Total score: {totalScore}");
    }

    protected override void Gold(string[] lines)
    {
        var totalScore = 0;
        foreach (var line in lines)
        {
            var lineScore = GetLineScoreForGold(line);
            totalScore += lineScore;
        }
        Console.WriteLine($"Total score for gold: {totalScore}");
    }

    private static int GetLineScore(string line)
    {
        var values = line.Split(" ");
        var opponentShape = GetShape(values[0]);
        var myShape = GetShape(values[1]);
        return MyScore(opponentShape, myShape);
    }
    
    private static int GetLineScoreForGold(string line)
    {
        var values = line.Split(" ");
        var opponentShape = GetShape(values[0]);
        var myShape = GetShapeForGold(values[1], opponentShape);
        return MyScore(opponentShape, myShape);
    }

    private static Shape GetShape(string text)
    {
        if (text == "A" || text == "X")
        {
            return Shape.Rock;
        }

        if (text == "B" || text == "Y")
        {
            return Shape.Paper;
        }

        return Shape.Scissors;
    }
    
    private static Shape GetShapeForGold(string text, Shape opponentShape)
    {
        if (text == "X")
        {
            return GetResultShape(opponentShape, true);
        }

        if (text == "Y")
        {
            return opponentShape;
        }

        return GetResultShape(opponentShape, false);
    }

    private static int MyScore(Shape opponentShape, Shape myShape)
    {
        var result = DidWin(opponentShape, myShape);
        if (!result.HasValue)
        {
            return 3 + (int)myShape;
        }

        if (result.Value)
        {
            //won
            return 6 + (int)myShape;
        }

        //lost
        return (int)myShape;
    }

    private static bool? DidWin(Shape opponentShape, Shape myShape)
    {
        if (opponentShape == myShape) return null;

        return myShape == GetResultShape(opponentShape, false);
    }

    private static Shape GetResultShape(Shape opponentShape, bool toLose)
    {
        var val = (int) opponentShape;
        if (toLose)
        {
            val--;    
        }
        else
        {
            val++;
        }
        
        if (val > 3)
            val = 1;
        if (val == 0)
            val = 3;
        return (Shape)val;
    }
}
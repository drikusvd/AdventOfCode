using System.Drawing;

namespace AdventOfCode2022.Day9;

public class Day9Processor: ProcessorBase
{
    private Point[] _rope;
    private HashSet<Point> _tailLocations =  new();
    
    protected override void Silver(string[] lines)
    {
        InitRope(2);

        foreach (var line in lines)
        {
            Move(line);
        }
        
        Console.WriteLine($"The number of tail locations visited: {_tailLocations.Count}");
    }

    protected override void Gold(string[] lines)
    {
        InitRope(10);

        foreach (var line in lines)
        {
            Move(line);
        }
        
        Console.WriteLine($"The number of tail locations visited for rope with 10 knots: {_tailLocations.Count}");
    }

    private void InitRope(int knots)
    {
        _rope = new Point[knots];
        for (var i = 0; i < knots; i++)
        {
            _rope[i] = new Point(0, 0);
        }

        _tailLocations = new HashSet<Point>();
        _tailLocations.Add(_rope[knots - 1]);
    }

    private void Move(string line)
    {
        var value = int.Parse(line.Substring(2));
        if (line.StartsWith("U") || line.StartsWith("D"))
        {
            //vertical
            var up = line.StartsWith("U");
            for (var i = 1; i <= value; i++)
            {
                MoveOneVertical(up);
            }
        }
        else
        {
            //horizontal
            var left = line.StartsWith("L");
            for (var i = 1; i <= value; i++)
            {
                MoveOneHorizontal(left);
            }
        }
    }

    private void MoveOneVertical(bool up)
    {
        if (up)
        {
            _rope[0] = _rope[0] with { Y = _rope[0].Y - 1 };
        }
        else
        {
            _rope[0] = _rope[0] with { Y = _rope[0].Y + 1 };
        }

        FollowHead();
    }

    private void MoveOneHorizontal(bool left)
    {
        if (left)
        {
            _rope[0] = _rope[0] with { X = _rope[0].X - 1 };
        }
        else
        {
            _rope[0] = _rope[0] with { X = _rope[0].X + 1 };
        }

        FollowHead();
    }

    private void FollowHead()
    {
        for (var i = 1; i < _rope.Length; i++)
        {
            _rope[i] = MoveTailIfNeeded(_rope[i-1], _rope[i]);    
        }

        _tailLocations.Add(_rope[^1]);
    }

    private static Point MoveTailIfNeeded(Point head, Point tail)
    {
        var xDiff = tail.X - head.X;
        var yDiff = tail.Y - head.Y;

        if (Math.Abs(xDiff) <= 1 && Math.Abs(yDiff) <= 1) return tail; //nothing to do

        if (xDiff == 0)
        {
            //same column
            tail = tail with { Y = yDiff > 0 ? tail.Y - 1 : tail.Y + 1 };
        } 
        else if (yDiff == 0)
        {
            //same row     
            tail = tail with { X = xDiff > 0 ? tail.X - 1 : tail.X + 1 };
        }
        else
        {
            //diagonal
            tail = new Point(xDiff > 0 ? tail.X - 1 : tail.X + 1,
                yDiff > 0 ? tail.Y - 1 : tail.Y + 1);
        }

        return tail;
    }
}
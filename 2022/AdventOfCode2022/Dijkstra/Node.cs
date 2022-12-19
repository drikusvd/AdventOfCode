using System.Drawing;

namespace AdventOfCode2022.Dijkstra;

//Adapted from https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp
public class Node
{
    public string Id => GetIdentifier(Point.X, Point.Y);
    public Point Point { get; set; }
    public string Name { get; set; }
    public List<Edge> Edges { get; set; } = new();

    public override string ToString()
    {
        return Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static string GetIdentifier(int x, int y)
    {
        return $"({x},{y})";
    }
}
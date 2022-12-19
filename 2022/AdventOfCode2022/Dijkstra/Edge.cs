namespace AdventOfCode2022.Dijkstra;

//Adapted from https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp
public class Edge
{
    public double Cost { get; set; }
    public Node ConnectedNode { get; set; }

    public override string ToString()
    {
        return "-> " + ConnectedNode.ToString();
    }
}
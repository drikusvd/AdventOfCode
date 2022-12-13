using System.Drawing;
using AdventOfCode2022.Day12.Dijkstra;

namespace AdventOfCode2022.Day12;

public class Day12Processor: ProcessorBase
{
    protected override void Silver(string[] lines)
    {
        var (startNodes, endNode, _) = BuildNodes(lines);
        var shortestRoute = DijkstraShortestPath.GetShortestPath(startNodes.First(), endNode);
        
        Console.WriteLine($"The shortest route from S to E is: {shortestRoute.Count-1} steps");
    }

    protected override void Gold(string[] lines)
    {
        var (startNodes, endNode, allNodes) = BuildNodes(lines, "Sa");
        var shortest = 10000;
        foreach (var startNode in startNodes)
        {
            foreach (var node in allNodes)
            {
                node.Reinit();
            }
            
            var shortestRoute = DijkstraShortestPath.GetShortestPath(startNode, endNode);
            var steps = shortestRoute.Count - 1;
            //Console.WriteLine($"{(steps == 0 ? "Infinite" : steps.ToString())} steps from {startNode.Id} to E");
            if (steps > 0 && steps < shortest)
                shortest = steps;
        }
        
        Console.WriteLine($"The shortest route from any S or a to E is: {shortest} steps");
    }

    private static (List<Node> startNodes, Node endNode, HashSet<Node> allNodes) BuildNodes(string[] lines, string allowedStartingElevations = "S")
    {
        var startNodes = new List<Node>();
        Node endNode = null;
        var allNodes = new HashSet<Node>();
        var y = -1;
        foreach (var line in lines)
        {
            y++;

            for (var x = 0; x < line.Length; x++)
            {
                var elevation = line[x];
                var node = new Node()
                {
                    Point = new Point(x, y),
                    Elevation = elevation
                };
                allNodes.Add(node);
                if (allowedStartingElevations.Contains(elevation))
                {
                    startNodes.Add(node);
                    node.Elevation = 'a';
                } 
                else if (elevation == 'E')
                {
                    endNode = node;
                    node.Elevation = 'z';
                }

                //check for neighbours
                PopulateNeighbours(x, y, allNodes, node);
            }
        }

        return (startNodes, endNode!, allNodes);
    }

    private static void PopulateNeighbours(int x, int y, HashSet<Node> allNodes, Node currentNode)
    {
        var neighbourIdentifiers = new List<string>();
        if (y > 0)
        {
            neighbourIdentifiers.Add(Node.GetIdentifier(x, y - 1));
            //We work from the top down in the lines, so we don't need to check the bottom
        }

        if (x > 0)
        {
            neighbourIdentifiers.Add(Node.GetIdentifier(x - 1, y));
            //We work from the left to right on the line, so we don't need to check the right
        }

        if (neighbourIdentifiers.Any())
        {
            var neighbours = allNodes.Where(a => neighbourIdentifiers.Contains(a.Id)).ToList();

            foreach (var neighbour in neighbours)
            {
                if (neighbour.Elevation > currentNode.Elevation)
                {
                    var dist = neighbour.Elevation - currentNode.Elevation;
                    if (dist == 1)
                    {
                        //not too high, can go to the neighbour
                        var toNeighbourEdge = new Edge
                        {
                            Cost = 1,
                            ConnectedNode = neighbour
                        };
                        currentNode.Edges.Add(toNeighbourEdge);
                    }

                    var fromNeighbourEdge = new Edge
                    {
                        Cost = 1,
                        ConnectedNode = currentNode
                    };
                    neighbour.Edges.Add(fromNeighbourEdge);
                }
                else
                {
                    var toNeighbourEdge = new Edge
                    {
                        Cost = 1,
                        ConnectedNode = neighbour
                    };
                    currentNode.Edges.Add(toNeighbourEdge);

                    var dist = currentNode.Elevation - neighbour.Elevation;
                    if (dist <= 1)
                    {
                        //not too high, can come from the neighbour
                        var fromNeighbourEdge = new Edge
                        {
                            Cost = 1,
                            ConnectedNode = currentNode
                        };
                        neighbour.Edges.Add(fromNeighbourEdge);
                    }
                }
            }
        }
    }
}
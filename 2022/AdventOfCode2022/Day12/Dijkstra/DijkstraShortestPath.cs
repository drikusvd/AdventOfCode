namespace AdventOfCode2022.Day12.Dijkstra;

//Adapted from https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp
public class DijkstraShortestPath
{
    public static List<Node> GetShortestPath(Node startNode, Node endNode)
    {
        DijkstraSearch(startNode, endNode);
        var shortestPath = new List<Node>();
        shortestPath.Add(endNode);
        BuildShortestPath(shortestPath, endNode);
        shortestPath.Reverse();
        return shortestPath;
    }

    private static void BuildShortestPath(List<Node> list, Node node)
    {
        if (node.NearestToStart == null)
            return;
        list.Add(node.NearestToStart);
        BuildShortestPath(list, node.NearestToStart);
    }

    private static void DijkstraSearch(Node startNode, Node endNode)
    {
        startNode.MinimumCostToStart = 0;
        var list = new List<Node>();
        list.Add(startNode);
        do
        {
            list = list.OrderBy(x => x.MinimumCostToStart.Value).ToList();
            var node = list.First();
            list.Remove(node);
            foreach (var edge in node.Edges.OrderBy(x => x.Cost))
            {
                var childNode = edge.ConnectedNode;
                if (childNode.Visited)
                    continue;
                if (childNode.MinimumCostToStart == null ||
                    node.MinimumCostToStart + edge.Cost < childNode.MinimumCostToStart)
                {
                    childNode.MinimumCostToStart = node.MinimumCostToStart + edge.Cost;
                    childNode.NearestToStart = node;
                    if (!list.Contains(childNode))
                        list.Add(childNode);
                }
            }

            node.Visited = true;
            if (node == endNode)
                return;
        } while (list.Any());
    }
}
namespace AdventOfCode2022.Dijkstra;

//Adapted from https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp
public class DijkstraShortestPath
{
    public static List<Node> GetShortestPath(Node startNode, Node endNode)
    {
        var nearestToStart = DijkstraSearch(startNode, endNode);
        var shortestPath = new List<Node>();
        shortestPath.Add(endNode);
        BuildShortestPath(shortestPath, endNode, nearestToStart);
        shortestPath.Reverse();
        return shortestPath;
    }

    private static void BuildShortestPath(List<Node> list, Node node, Dictionary<Node, Node> nearestToStart)
    {
        if (!nearestToStart.ContainsKey(node))
            return;
        list.Add(nearestToStart[node]);
        BuildShortestPath(list, nearestToStart[node], nearestToStart);
    }

    private static Dictionary<Node, Node> DijkstraSearch(Node startNode, Node endNode)
    {
        var minCostToStart = new Dictionary<Node, double>();
        var visited = new Dictionary<Node, bool>();
        var nearestToStart = new Dictionary<Node, Node>();

        minCostToStart[startNode] = 0;
        var list = new List<Node>();
        list.Add(startNode);
        do
        {
            list = list.OrderBy(x => minCostToStart[x]).ToList();
            var node = list.First();
            list.Remove(node);
            foreach (var edge in node.Edges.OrderBy(x => x.Cost))
            {
                var childNode = edge.ConnectedNode;
                if (visited.ContainsKey(childNode))
                    continue;
                if (!minCostToStart.ContainsKey(childNode) ||
                    minCostToStart[node] + edge.Cost < minCostToStart[childNode])
                {
                    minCostToStart[childNode] = minCostToStart[node] + edge.Cost;
                    nearestToStart[childNode] = node;
                    if (!list.Contains(childNode))
                        list.Add(childNode);
                }
            }

            visited[node] = true;
            if (node == endNode)
                return nearestToStart;
        } while (list.Any());

        return nearestToStart;
    }
}
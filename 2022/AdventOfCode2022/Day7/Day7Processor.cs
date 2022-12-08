namespace AdventOfCode2022.Day7;

public class Day7Processor: ProcessorBase
{
    private enum NodeType
    {
        Dir,
        File
    }
    
    private class Node
    {
        public NodeType Type { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }

        public Node Parent { get; set; }
        public List<Node> Children { get; set; } = new();
    }

    private Node rootNode;
    private List<Node> dirList;

    private void Init(string[] lines)
    {
        if (rootNode == null)
        {
            rootNode = BuildTree(lines);
            dirList = GetFlatListDirectories(rootNode);
        }
    }
    
    protected override void Silver(string[] lines)
    {
        Init(lines);
        var sumOfDirsLessThan100k = dirList.Where(a => a.Size <= 100000).Sum(a => a.Size);
        
        Console.WriteLine($"The sum of all dirs with size at most 100 000: {sumOfDirsLessThan100k}");
    }

    protected override void Gold(string[] lines)
    {
        Init(lines);
        var totalAvailable = 70000000;
        var minimumFree = 30000000;
        var currentFree = totalAvailable - rootNode.Size;
        var required = minimumFree - currentFree;

        var smallestToDelete = dirList.Where(a => a.Size >= required).OrderBy(a => a.Size).First();
        Console.WriteLine($"The size of the directory to delete: {smallestToDelete.Size}");
    }

    private Node BuildTree(string[] lines)
    {
        var root = new Node
        {
            Name = "/",
            Type = NodeType.Dir
        };

        var curNode = root;
        for (var l = 0; l < lines.Length; l++)
        {
            var line = lines[l];
            if (line.StartsWith("$"))
            {
                //command
                if (line == "$ cd ..")
                {
                    //go up
                    curNode = curNode.Parent;
                } 
                else if (line == "$ cd /")
                {
                    curNode = root;
                }
                else if (line.StartsWith("$ cd "))
                {
                    var targetDirName = line.Substring(5);
                    var targetNode = curNode.Children.First(a => a.Type == NodeType.Dir && a.Name == targetDirName);
                    curNode = targetNode;
                }
            }
            else if (line.StartsWith("dir "))
            {
                //directory
                var name = line.Substring(4);
                var dirNode = curNode.Children.FirstOrDefault(a => a.Type == NodeType.Dir && a.Name == name);
                if (dirNode == null)
                {
                    dirNode = new Node
                    {
                        Name = name,
                        Type = NodeType.Dir,
                        Parent = curNode,
                    };
                    curNode.Children.Add(dirNode);
                }
            }
            else if (char.IsNumber(line[0]))
            {
                //file
                var idx = line.IndexOf(" ", StringComparison.InvariantCulture);
                var name = line.Substring(idx + 1);
                var fileNode = curNode.Children.FirstOrDefault(a => a.Type == NodeType.File && a.Name == name);
                if (fileNode == null)
                {
                    var size = int.Parse(line.Substring(0, idx));
                    fileNode = new Node
                    {
                        Name = name,
                        Type = NodeType.File,
                        Parent = curNode,
                        Size = size
                    };
                    curNode.Children.Add(fileNode);
                }
            }
        }

        CalculateSizes(root);
        return root;
    }

    private void CalculateSizes(Node node)
    {
        var total = 0;
        foreach (var child in node.Children)
        {
            if (child.Type == NodeType.File)
            {
                total += child.Size;
            }
            else if (child.Type == NodeType.Dir)
            {
                CalculateSizes(child);
                total += child.Size;
            }
        }

        node.Size = total;
    }

    private List<Node> GetFlatListDirectories(Node node, List<Node> dirList = null)
    {
        if (dirList == null)
            dirList = new List<Node>();
        foreach (var child in node.Children)
        {
            if (child.Type == NodeType.Dir)
            {
                dirList.Add(child);
                GetFlatListDirectories(child, dirList);
            }
        }

        return dirList;
    }
}
namespace AdventOfCode2022.Day13;

public class Day13Processor: ProcessorBase
{
    private class Packet: IComparable<Packet>
    {
        public List<Packet>? List { get; set; }
        public int? Value { get; set; }
        
        public string RootPacketText { get; set; }

        public static bool operator >(Packet a, Packet b) => a.CompareTo(b) == 1;
        public static bool operator <(Packet a, Packet b) => a.CompareTo(b) == -1;
        
        public static bool operator >=(Packet a, Packet b) => a.CompareTo(b) >= 0;
        public static bool operator <=(Packet a, Packet b)=> a.CompareTo(b) <= 0;
        
        public int CompareTo(Packet? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            
            if (List == null && !Value.HasValue)
            {
                if (other.List != null || Value.HasValue)
                    return -1;
                return 0;
            }

            if (other.List == null && !other.Value.HasValue)
            {
                return 1;
            }

            if (List != null && other.Value.HasValue)
            {
                //convert other to list
                var tempPacket = new Packet()
                {
                    List = new List<Packet> { other }
                };
                return CompareTo(tempPacket);
            }

            if (Value.HasValue && other.List != null)
            {
                //convert this to a list
                var tempPacket = new Packet()
                {
                    List = new List<Packet> { this }
                };
                return tempPacket.CompareTo(other);
            }

            if (Value.HasValue && other.Value.HasValue)
            {
                return Value.Value.CompareTo(other.Value.Value);
            }
            
            //list comparison
            for (var i = 0; i < List!.Count; i++)
            {
                if (i > other.List!.Count - 1)
                {
                    //this list is longer than the other list
                    return 1;
                }

                var a = List[i];
                var b = other.List[i];
                var comp = a.CompareTo(b);
                if (comp == 0)
                    continue;
                return comp;
            }

            if (List.Count < other.List!.Count)
                return -1;
            
            return 0;
        }
    }
    
    protected override void Silver(string[] lines)
    {
        var pairs = GetPacketPairs(lines);
        var rightIndices = new List<int>();
        for (var i = 0; i < pairs.Count; i++)
        {
            var pair = pairs[i];
            if (pair.a <= pair.b)
                rightIndices.Add(i+1);
        }
        
        Console.WriteLine($"The sum of the packet indices in the right order: {rightIndices.Sum()}");
    }

    protected override void Gold(string[] lines)
    {
        var result = new List<Packet>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var packet = CreatePacket(line);
            result.Add(packet);
        }
        
        result.Add(CreatePacket("[[2]]"));
        result.Add(CreatePacket("[[6]]"));
        
        result.Sort();

        var idx1 = result.FindIndex(a => a.RootPacketText == "[[2]]")+1;
        var idx2 = result.FindIndex(a => a.RootPacketText == "[[6]]")+1;
        
        Console.WriteLine($"The decoder key for the distress signal: {idx1 * idx2}");
    }

    private static List<(Packet a, Packet b)> GetPacketPairs(string[] lines)
    {
        var result = new List<(Packet, Packet)>();
        Packet? a = null;
        for (var l = 0; l < lines.Length; l++)
        {
            var line = lines[l];
            if (string.IsNullOrWhiteSpace(line))
            {
                a = null;
                continue;
            }

            if (a == null)
            {
                a = CreatePacket(line);
                CreatePacket(line, 1, a);
            }
            else
            {
                var b = CreatePacket(line);
                result.Add((a, b));
                a = null;
            }
        }

        return result;
    }

    private static Packet CreatePacket(string line)
    {
        var packet = new Packet
        {
            List = new List<Packet>(),
            RootPacketText = line
        };
        CreatePacket(line, 1, packet);
        return packet;
    }

    private static int CreatePacket(string line, int index, Packet parent)
    {
        var ch = line[index];
        while (ch != ']')
        {
            if (ch == '[')
            {
                //the child is a list
                var listChild = new Packet()
                {
                    List = new List<Packet>()
                };
                parent.List!.Add(listChild);
                index = CreatePacket(line, index + 1, listChild);
            }
            else if (char.IsNumber(ch))
            {
                var end = index;
                while (char.IsNumber(line[end]))
                {
                    end++;
                }
                var valStr = line.Substring(index, end - index);
                var numChild = new Packet
                {
                    Value = int.Parse(valStr)
                };
                parent.List!.Add(numChild);
                index = end;
            } 
            else if (ch == ',')
            {
                //move to next child
                index++;
            }
            
            ch = line[index];
        }

        return index + 1;
    }
}
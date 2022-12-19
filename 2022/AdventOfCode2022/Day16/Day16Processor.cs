using System.Diagnostics;
using AdventOfCode2022.Dijkstra;

namespace AdventOfCode2022.Day16;

public class Day16Processor: ProcessorBase
{
    private const int MAX_MINUTES = 30;
    
    private enum ValveAction
    {
        MoveDownTunnel,
        OpenValve
    }
    
    private class Valve : Node
    {
        public int FlowRate { get; set; }
    }

    private class ValveActionTaken
    {
        public Valve Valve { get; set; }
        public ValveAction Action { get; set; }
        public Valve? MoveToValve { get; set; }

        public ValveActionTaken(Valve valve, ValveAction action, Valve? moveToValve = null)
        {
            Valve = valve;
            Action = action;
            MoveToValve = moveToValve;
        }
    }

    private class MinuteStats
    {
        public List<Valve> OpenValves { get; set; } = new();
        public int Minute { get; set; }
        public int ReleasedPressure { get; set; }

        public MinuteStats Clone()
        {
            return new MinuteStats
            {
                OpenValves = OpenValves.ToList(),
                ReleasedPressure = ReleasedPressure,
                Minute = Minute
            };
        }
    }
    
    protected override void Silver(string[] lines)
    {
        var valves = GetValves(lines);

        var sw = Stopwatch.StartNew();
        var aa = valves.First(a => a.Name == "AA");

        var positives = valves.Where(a => a.FlowRate > 0).ToList();

        var (bestActions, bestStats) = BestByShortestPath(aa, positives, new List<ValveActionTaken>(), new List<MinuteStats>());
        Console.WriteLine($"Total duration: {sw.ElapsedMilliseconds} ms");
        
        for (var i = 0; i < MAX_MINUTES; i++)
        {
            var act = bestActions.Count > i ? bestActions[i] : null;
            var stats = bestStats![i];
            Console.WriteLine($"Minute {i+1}");
            Console.WriteLine($"Open valves: {string.Join(",", stats.OpenValves.Select(a => a.Name))}. Released pressure: {stats.ReleasedPressure}");
            if (act != null)
            {
                Console.WriteLine($"{act.Action}{(act.Action == ValveAction.MoveDownTunnel ? $" to {act.MoveToValve!.Name}" : act.Valve.Name)}");    
            }
            Console.WriteLine();
        }
        
        var totalPressure = bestStats!.Sum(a => a.ReleasedPressure);
        Console.WriteLine($"Total released pressure: {totalPressure}");
    }

    protected override void Gold(string[] lines)
    {
        
    }
    
    private static (List<ValveActionTaken> actionsTaken, List<MinuteStats> minuteStats) BestByShortestPath(Valve currentValve,
        List<Valve> allPositiveValves, List<ValveActionTaken> actionsTaken, List<MinuteStats> minuteStats)
    {
        if (minuteStats.Count >= MAX_MINUTES)
            return (actionsTaken, minuteStats);
        
        var allOptionsActions = new List<List<ValveActionTaken>>();
        var allOptionsMinuteStats = new List<List<MinuteStats>>();

        if (currentValve.FlowRate > 0 && 
            !actionsTaken.Any(a => a.Action == ValveAction.OpenValve && a.Valve == currentValve))
        {
            TurnValve(currentValve, actionsTaken, minuteStats);
            
            if (minuteStats.Count >= MAX_MINUTES)
                return (actionsTaken, minuteStats);
            allOptionsActions.Add(actionsTaken);
            allOptionsMinuteStats.Add(minuteStats);
        }
        
        foreach (var valve in allPositiveValves)
        {
            if (actionsTaken.Any(a => a.Action == ValveAction.OpenValve && a.Valve == valve))
                continue; //already done in this hierarchy
            
            var path = DijkstraShortestPath.GetShortestPath(currentValve, valve);

            var newActions = actionsTaken.ToList();
            var newStats = minuteStats.ToList();

            var fromNode = currentValve;
            foreach (var node in path)
            {
                if (node == currentValve) continue;
                
                MoveDownTunnel(fromNode, (Valve)node, newActions, newStats);

                fromNode = (Valve)node;
                
                if (newStats.Count >= MAX_MINUTES)
                    break;
            }

            if (newStats.Count < MAX_MINUTES)
            {
                (newActions, newStats) = BestByShortestPath(valve, allPositiveValves, newActions, newStats);
                allOptionsActions.Add(newActions);
                allOptionsMinuteStats.Add(newStats);
            }
        }
        
        return ReturnBestResult(allOptionsActions, allOptionsMinuteStats);
    }

    private static (List<ValveActionTaken> actionsTaken, List<MinuteStats> minuteStats) ReturnBestResult(List<List<ValveActionTaken>> allOptionsActions,
        List<List<MinuteStats>> allOptionsMinuteStats)
    {
        var maxPressure = -1;
        List<ValveActionTaken>? bestActionsTaken = null;
        List<MinuteStats>? bestMinuteStats = null;

        for (var i = 0; i < allOptionsActions.Count; i++)
        {
            var optActions = allOptionsActions[i];
            var optStats = allOptionsMinuteStats[i];

            while (optStats.Count < MAX_MINUTES)
            {
                var newStats = optStats.Last().Clone();
                newStats.Minute++;
                optStats.Add(newStats);
            }
            
            var totalPressure = optStats.Sum(a => a.ReleasedPressure);
            if (totalPressure> maxPressure)
            {
                maxPressure = totalPressure;
                bestActionsTaken = optActions;
                bestMinuteStats = optStats;
            }
        }

        return (bestActionsTaken!, bestMinuteStats!);
    }

    private static void TurnValve(Valve currentValve, List<ValveActionTaken> actionsTaken,
        List<MinuteStats> minuteStats)
    {
        var action = new ValveActionTaken(currentValve, ValveAction.OpenValve);
        actionsTaken.Add(action);

        AddMinuteStats(minuteStats, currentValve);
    }

    private static void MoveDownTunnel(Valve currentValve, Valve toValve, List<ValveActionTaken> actionsTaken,
        List<MinuteStats> minuteStats)
    {
        var action = new ValveActionTaken(currentValve, ValveAction.MoveDownTunnel, toValve);
        actionsTaken.Add(action);

        AddMinuteStats(minuteStats);
    }

    private static void AddMinuteStats(List<MinuteStats> minuteStats, Valve? addValve = null)
    {
        MinuteStats stats;
        if (minuteStats.Count > 0)
        {
            stats = minuteStats.Last().Clone();
            stats.Minute++;
        }
        else
        {
            stats = new MinuteStats()
            {
                Minute = 1
            };
        }

        var totalPressure = 0;
        foreach (var valve in stats.OpenValves)
        {
            totalPressure += valve.FlowRate;
        }

        stats.ReleasedPressure = totalPressure;

        //add valve after pressure calc so that it is not included in the first minute
        if (addValve != null && !stats.OpenValves.Contains(addValve))
        {
            stats.OpenValves.Add(addValve);
        }

        minuteStats.Add(stats);
    }

    private static List<Valve> GetValves(string[] lines)
    {
        var result = new List<Valve>();
        var valveTunnels = new Dictionary<string, List<string>>();
        foreach (var line in lines)
        {
            var id = line.Substring(6, 2);
            var idx = line.IndexOf(";", StringComparison.InvariantCulture);
            var rate = int.Parse(line.Substring(23, idx - 23));
            valveTunnels[id] = line.Substring(idx + 1)
                .Replace("tunnel ", "")
                .Replace("tunnels ", "")
                .Replace("lead to valves ", "")
                .Replace("leads to valve ", "")
                .Trim()
                .Split(",")
                .Select(a => a.Trim())
                .ToList();

            var valve = new Valve()
            {
                Name = id,
                FlowRate = rate,
            };
            result.Add(valve);
        }
        
        //fix up tunnels
        foreach (var keyPair in valveTunnels)
        {
            var v = result.First(a => a.Name == keyPair.Key);
            foreach (var tv in keyPair.Value)
            {
                var tunnelValve = result.First(a => a.Name == tv);
                var edge = new Edge()
                {
                    Cost = 1,
                    ConnectedNode = tunnelValve
                };
                v.Edges.Add(edge);
            }
        }

        return result;
    }
}
using Structure;
using static Util.Util;

namespace Program;

public class Program
{
    public static readonly Dictionary<int, string> id_to_name = new Dictionary<int, string>();
    public static readonly bool bidirect = false;

    public static void Main(string[] args) {
        var lines = File.ReadAllLines(args[0]);

        var node_count = int.Parse(lines[0]);
        var nodes = new List<Node>();
        for(int i = 1; i <= node_count; i++) {
            id_to_name.Add(i-1, lines[i]);
            nodes.Add(new Node());
        }

        var graph = new int[node_count, node_count];
        var edge_count = int.Parse(lines[node_count+1]);
        var edge_data = new List<(int, int, int)>();
        for(int i = 1; i <= edge_count; i++) {
            var data = lines[node_count+1+i].Split(" ").Select(e => int.Parse(e)).ToArray();
            edge_data.Add((data[0], data[1], data[2]));
            if (bidirect) edge_data.Add((data[1], data[0], data[2]));
        }

        var flow_graph = new FlowGraph(nodes.ToArray(), edge_data);

        var cut = flow_graph.MinCut(0, node_count-1);
        var sum = 0;
        foreach(var (from, to, flow) in cut) {
            sum += flow;
            System.Console.WriteLine($"{from} - {to}: {flow}");
        }
        System.Console.WriteLine(sum);
    }
}

public class FlowGraph
{
    private Node[] graph;

    public FlowGraph(Node[] nodes, List<(int, int, int)> edges) {

        foreach(var (from, to, cap) in edges) {
            var edge = new Edge(cap);

            nodes[from].AddEdge(to, false, edge);
            nodes[to].AddEdge(from, true, edge);
        }

        graph = nodes;
    }

    public (int, int, int)[] MinCut(int from, int to) {
        while(true) {
            var (success, path) = Path(from, to, graph);
            if (!success) break;
            AugmentPath(path, PathMinRemainingCapacity(path, graph), graph);
        }

        var mincut = new List<(int from, int to, int flow)>();
        var reachable = Reachable(from, graph);
        foreach(var node in reachable) {
            var edges = graph[node].Edges();
            foreach(var connection in edges) {
                if (reachable.Contains(connection.to)) continue;
                if (connection.edge.flow == 0) continue;
                mincut.Add((node, connection.to, connection.edge.flow));
            }
        }

        return mincut.ToArray();
    }
}
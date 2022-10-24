
public class Program
{
    public static readonly Dictionary<int, string> id_to_name = new Dictionary<int, string>();

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
        for(int i = 1; i < edge_count; i++) {
            var data = lines[node_count+1+i].Split(" ").Select(e => int.Parse(e)).ToArray();
            nodes[data[0]].Connect(data[1], data[2]);
            nodes[data[1]].Connect(data[0], data[2]);
        }

        var flow_graph = new FlowGraph(nodes);

        var (success, path) = flow_graph.Path(0,54);
        System.Console.WriteLine(success);
        foreach(var p in path) System.Console.WriteLine(id_to_name[p]);

        System.Console.WriteLine(flow_graph.MaxFlow(0, 54));
    }
}

public class Edge
{
    public readonly int to;
    public int capacity;
    public int flow;

    public Edge(int t, int c) {
        to = t;
        capacity = c;
        flow = 0;
    }

    public bool Augment(int by) {
        if (flow + by > capacity || flow + by < 0) return false; 
        flow += by;
        return true;
    }
}

public class Node
{
    private Dictionary<int, Edge> connections = new Dictionary<int, Edge>();

    public Node() {}

    public void Connect(int to, int cap) {
        connections.Add(to, new Edge(to, cap));
    }

    public Dictionary<int, Edge> Connections() {
        return connections;
    }
}

public class FlowGraph
{
    private List<Node> graph;

    public FlowGraph(List<Node> ns) {
        graph = ns;
    }

    public (bool, int[]) Path(int from, int to) {
        return PathFromTo(from, to, new HashSet<int>());
    }

    private (bool, int[]) PathFromTo(int from, int to, HashSet<int> prev) {

        var connections = graph[from].Connections();

        foreach(var connection in connections) {
            var remainingCap = connection.Value.capacity - connection.Value.flow;
            if (remainingCap == 0) continue;
            if (prev.Contains(connection.Key)) continue;
            if (connection.Key == to) return (true, new int[] {to});

            prev.Add(from);
            var (success, path) = PathFromTo(connection.Value.to, to, prev);
            if (success) return (true, path.ToList().Prepend(connection.Key).ToArray());
            prev.Remove(from);
        }

        return (false, new int[0]);
    }

    public int[] Connections(int node) {
        return graph[node].Connections().Select(e => e.Key).ToArray();
    }

    public int PathMinRemainingCapacity(int[] path) {
        var minRemainingCap = int.MaxValue;
        for(int i = 1; i < path.Length; i++) {
            var edge = graph[path[i-1]].Connections()[path[i]];
            if (edge.capacity != -1 && edge.capacity - edge.flow < minRemainingCap) 
                minRemainingCap = edge.capacity - edge.flow;
        }
        return minRemainingCap;
    }

    public void AugmentPath(int[] path, int by) {
        for(int i = 1; i < path.Length; i++) {
            var forward = graph[path[i-1]].Connections()[path[i]];
            var backward = graph[forward.to].Connections()[path[i-1]];
            forward.flow += by;
            if (backward.capacity != -1) backward.capacity += by;
        }
    }

    public int MaxFlow(int from, int to) {
        while(true) {
            var (success, path) = Path(from, to);
            if (!success) break;
            AugmentPath(path, PathMinRemainingCapacity(path));
        }

        var sum = 0;
        foreach(var e in graph[from].Connections()) sum += e.Value.flow;
        return sum;
    }
}
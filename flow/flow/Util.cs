using Structure;

namespace Util;

public static class Util
{
    public static HashSet<int> Reachable(int from, Node[] graph) {
        return _Reachable(from, graph, new HashSet<int>());
    }

    private static HashSet<int> _Reachable(int from, Node[] graph, HashSet<int> set) {
        set.Add(from);
        foreach(var c in graph[from].Edges()) {
            if (c.Value.rev) continue;
            if (c.Value.edge.capacity - c.Value.edge.flow == 0) continue;
            if (set.Contains(c.Key)) continue;
            _Reachable(c.Key, graph, set);
        }
        return set;
    }

    public static int PathMinRemainingCapacity(int[] path, Node[] graph) {
        var minRemainingCap = int.MaxValue;
        for(int i = 1; i < path.Length; i++) {
            var connection = graph[path[i-1]].Edges()[path[i]];
            if (connection.rev) {
                if (connection.edge.flow < minRemainingCap)
                    minRemainingCap = connection.edge.flow;
            }
            else {
                if (connection.edge.capacity - connection.edge.flow < minRemainingCap) 
                    minRemainingCap = connection.edge.capacity - connection.edge.flow;
            }
        }
        return minRemainingCap;
    }

    public static (bool, int[]) Path(int from, int to, Node[] graph) {
        return _Path(from, to, graph, new HashSet<int>());
    }

    private static (bool, int[]) _Path(int from, int to, Node[] graph, HashSet<int> prev) {

        var connections = graph[from].Edges();

        foreach(var c in connections) {
            if (c.Value.rev) {
                if (c.Value.edge.flow == 0) continue;
            }
            else {
                if (c.Value.edge.capacity - c.Value.edge.flow == 0) continue;
            }
            if (prev.Contains(c.Key)) continue;
            if (c.Key == to) return (true, new int[] {to});

            prev.Add(from);
            var (success, path) = _Path(c.Key, to, graph, prev);
            if (success) return (true, path.ToList().Prepend(c.Key).ToArray());
            prev.Remove(from);
        }

        return (false, new int[0]);
    }

    public static void AugmentPath(int[] path, int by, Node[] graph) {
        for(int i = 1; i < path.Length; i++) {
            var connection = graph[path[i-1]].Edges()[path[i]];
            
            if (connection.rev) {
                connection.edge.flow -= by;
            }
            else {
                connection.edge.flow += by;
            }
        }
    }
}
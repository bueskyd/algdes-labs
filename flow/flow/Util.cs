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

        var path = new int[graph.Length];
        var visited = new bool[graph.Length];
        var stack = new Stack<int>();
        stack.Push(from);

        while(stack.Count > 0) {
            var node = stack.Pop();
            visited[node] = true;
            foreach(var c in graph[node].Edges()) {
                if (visited[c.Key]) continue;
                if (c.Value.rev && c.Value.edge.flow == 0) continue;
                if (!c.Value.rev && c.Value.edge.capacity - c.Value.edge.flow == 0) continue;

                stack.Push(c.Key);
                path[c.Key] = node;

                if(c.Key == to) {
                    var s = new Stack<int>();
                    var p = to;
                    while(true) {
                        s.Push(p);
                        if (p == from) return (true, s.ToArray());
                        p = path[p];
                    }
                }
            }
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
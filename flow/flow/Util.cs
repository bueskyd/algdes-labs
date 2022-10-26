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
            if (c.rev) continue;
            if (c.edge.capacity - c.edge.flow == 0) continue;
            if (set.Contains(c.to)) continue;
            _Reachable(c.to, graph, set);
        }
        return set;
    }

    public static (int to, bool rev, Edge edge) FindConnection(bool rev, int to, List<(int to, bool rev, Edge edge)> edges) {
        foreach(var e in edges) {
            if (e.rev == rev && e.to == to) return e;
        }
        return (-1, false, null);
    }

    public static int PathMinRemainingCapacity((int to, bool rev, Edge edge)[] path, Node[] graph) {
        var minRemainingCap = int.MaxValue;
        for(int i = 0; i < path.Length; i++) {
            var connection = path[i];// FindConnection(path[i-1].rev, path[i].to, graph[path[i-1].to].Edges());
            if (connection.rev) {
                if (connection.edge.capacity < 0) continue;
                if (connection.edge.flow < minRemainingCap)
                    minRemainingCap = connection.edge.flow;
            }
            else {
                if (connection.edge.capacity < 0) continue;
                if (connection.edge.capacity - connection.edge.flow < minRemainingCap) 
                    minRemainingCap = connection.edge.capacity - connection.edge.flow;
            }
        }
        return minRemainingCap;
    }

    public static (bool success, (int to, bool rev, Edge edge)[]) Path(int from, int to, Node[] graph) {

        var path = new (bool rev, int from)[graph.Length];
        var visited = new bool[graph.Length];
        var stack = new Stack<int>();
        stack.Push(from);

        while(stack.Count > 0) {
            var node = stack.Pop();
            visited[node] = true;
            foreach(var c in graph[node].Edges()) {
                if (visited[c.to]) continue;
                if (c.rev && c.edge.flow == 0) continue;
                if (!c.rev && c.edge.capacity - c.edge.flow == 0) continue;

                stack.Push(c.to);
                path[c.to] = (c.rev, node);

                if(c.to == to) {
                    var s = new Stack<(int, bool, Edge)>();
                    var current = to;
                    var p = path[to];
                    while(true) {
                        var connection = FindConnection(p.rev, current, graph[p.from].Edges());
                        s.Push(connection);
                        if (p.from == from) return (true, s.ToArray());
                        current = p.from;
                        p = path[p.from];
                    }
                }
            }
        } 

        return (false, new (int,bool,Edge)[0]);
    }

    public static void AugmentPath((int to, bool rev, Edge edge)[] path, int by, Node[] graph) {
        for(int i = 0; i < path.Length; i++) {
            var connection = path[i];// FindConnection(path[i-1].rev, path[i].to, graph[path[i-1].to].Edges());
            
            if (connection.rev) {
                connection.edge.flow -= by;
            }
            else {
                connection.edge.flow += by;
            }
        }
    }
}
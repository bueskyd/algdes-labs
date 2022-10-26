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

    public static int PathMinRemainingCapacity((bool rev, int to)[] path, Node[] graph) {
        var minRemainingCap = int.MaxValue;
        for(int i = 1; i < path.Length; i++) {
            var connection = FindConnection(path[i-1].rev, path[i].to, graph[path[i-1].to].Edges());
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

    public static (bool, (bool, int)[]) Path(int from, int to, Node[] graph) {

        var path = new (bool rev, int to)[graph.Length];
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
                    var s = new Stack<(bool, int)>();
                    var p = path[to];
                    while(true) {
                        s.Push(p);
                        if (p.to == from) return (true, s.ToArray());
                        p = path[p.to];
                    }
                }
            }
        } 

        return (false, new (bool,int)[0]);
    }

    public static void AugmentPath((bool rev, int to)[] path, int by, Node[] graph) {
        for(int i = 1; i < path.Length; i++) {
            var connection = FindConnection(path[i-1].rev, path[i].to, graph[path[i-1].to].Edges());
            
            if (connection.rev) {
                connection.edge.flow -= by;
            }
            else {
                connection.edge.flow += by;
            }
        }
    }
}
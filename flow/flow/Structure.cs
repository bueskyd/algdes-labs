
namespace Structure;

public class Edge
{
    public readonly int capacity;
    public int flow;

    public Edge(int c) {
        capacity = c;
        flow = 0;
    }
}

public class Node
{
    private List<(int, bool, Edge)> edges = new List<(int, bool, Edge)>();

    public Node() {}

    public void AddEdge(int to, bool rev, Edge e) {
        edges.Add((to, rev, e));
    }

    public List<(int to, bool rev, Edge edge)> Edges() {
        return edges;
    }
}
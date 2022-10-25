
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
    private Dictionary<int, (bool, Edge)> edges = new Dictionary<int, (bool, Edge)>();

    public Node() {}

    public void AddEdge(int to, bool rev, Edge e) {
        edges.Add(to, (rev, e));
    }

    public Dictionary<int, (bool rev, Edge edge)> Edges() {
        return edges;
    }
}
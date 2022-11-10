using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using RedScare.entities;

namespace RedScare;

public class NoneGraph : GenericGraph
{
    public NoneGraph() : base(0, 0)
    {
        
    }

    public override void Solve()
    {
        var result = _graph.Dijkstra(_nameToIndex[_s], _nameToIndex[_t]);
        
        var path = result.GetPath();
        var pathLength = path == null ? -1 : path.Count() - 1;
        Console.WriteLine(pathLength);
    }

    protected override bool ShouldAddEdge(Node toNode)
    {
        return toNode.Color == Color.Black;
    }
}
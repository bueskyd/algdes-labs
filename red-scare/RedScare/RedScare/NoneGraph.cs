using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using RedScare.entities;

namespace RedScare;

public class NoneGraph : Graph
{

    public NoneGraph() : base()
    {
        var sourceIndex = _nameToIndex[_s];
        var sinkIndex = _nameToIndex[_t];

        var result = Dijkstra();
        
        var sinkEdge = result[sinkIndex];

        Console.WriteLine(sinkEdge.Weight);
    }

    public void Solve()
    {
        var sourceIndex = _nameToIndex[_s];
        var sinkIndex = _nameToIndex[_t];

        var result = Dijkstra();
        
        var sinkEdge = result[sinkIndex];

        Console.WriteLine(sinkEdge.Weight);
    }

    public override bool ShouldAddEdge(Node fromNode, Node toNode)
    {
        return toNode.Color != Color.Red;
    }

    public override int EdgeCost(Node fromNode, Node toNode)
    {
        return 1;
    }
}
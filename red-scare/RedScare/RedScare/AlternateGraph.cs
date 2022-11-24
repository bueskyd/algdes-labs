using RedScare.entities;

namespace RedScare;

public class AlternateGraph : Graph
{

    public AlternateGraph() : base()
    {
        
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
        return fromNode.Color != toNode.Color;
    }

    public override int EdgeCost(Node fromNode, Node toNode)
    {
        return 1;
    }
}
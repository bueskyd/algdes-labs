namespace RedScare.entities;

public class Edge
{
    public uint ToNodeIndex;
    public int Weight;

    public Edge(uint toNodeIndex, int weight)
    {
        ToNodeIndex = toNodeIndex;
        Weight = weight;
    }
    
    public override string ToString()
    {
        return $"{nameof(ToNodeIndex)}: {ToNodeIndex}, {nameof(Weight)}: {Weight}";
    }
}
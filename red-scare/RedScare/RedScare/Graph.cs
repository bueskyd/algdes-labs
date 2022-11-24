using System.Runtime.CompilerServices;
using RedScare.entities;

namespace RedScare;

public abstract class Graph
{
    private const string RedNode = "*";
    private const string UndirectedSplitter = "--";
    private const string DirectedSplitter = "->";
    
    protected readonly Dictionary<string, uint> _nameToIndex;
    protected readonly Dictionary<uint, Node> _indexToNode;
    protected readonly Dictionary<uint, List<Edge>> _edges;

    protected readonly long _n;
    protected readonly long _m;
    protected readonly long _r;
    protected readonly string _s;
    protected readonly string _t;

    public Graph()
    {
        var firstLine = Console.ReadLine().Split();
        _n = long.Parse(firstLine[0]);
        _m = long.Parse(firstLine[1]);
        _r = long.Parse(firstLine[2]);
            
        var secondLine = Console.ReadLine().Split();
        _s = secondLine[0];
        _t = secondLine[1];
        
        _nameToIndex = new Dictionary<string, uint>();
        _indexToNode = new Dictionary<uint, Node>();
        _edges = new Dictionary<uint, List<Edge>>();
        
            
        ParseInput();
    }
    
    private void ParseInput()
    {
        // vertices
        for (var i = 0; i < _n; i++)
        {
            var vertexLine = Console.ReadLine().Split();
            var name = vertexLine[0];

            if (vertexLine.Length > 1 && vertexLine[1] == RedNode)
            { 
                AddNode(new Node(name, Color.Red));
            }
            else
            {
                AddNode(new Node(name, Color.Black));
            }
        }

        // edges
        for (var i = 0; i < _m; i++)
        {
            var edgeInput = Console.ReadLine();

            // undirected edge
            if (edgeInput.Contains(UndirectedSplitter))
            {
                AddEdge(edgeInput, true);
            } 
                
            // directed edge
            else if (edgeInput.Contains(DirectedSplitter))
            {
                AddEdge(edgeInput, false);
            }
        } 
    }
    
    private void AddNode(Node node)
    {
        var nodeIndex = (uint) _nameToIndex.Count;
        node.Index = nodeIndex;
        
        _nameToIndex.Add(node.Name, nodeIndex);
        _indexToNode.Add(nodeIndex, node);
    }
    
    private void AddEdge(string? edgeInput, bool undirected)
    {
        var splitter = undirected ? UndirectedSplitter : DirectedSplitter;
            
        var edgeLine = edgeInput.Split(splitter, StringSplitOptions.TrimEntries);

        var from = edgeLine[0];
        var to = edgeLine[1];

        var fromNode = _indexToNode[_nameToIndex[from]];
        var toNode = _indexToNode[_nameToIndex[to]];

        AddEdge(fromNode, toNode);

        if (!undirected)
        {
            AddEdge(toNode, fromNode);
        }
    }

    private void AddEdge(Node fromNode, Node toNode)
    {
        if (ShouldAddEdge(fromNode, toNode))
        {
            var fromNodeIndex = fromNode.Index;
            var toNodeIndex = toNode.Index;
            
            if (!_edges.ContainsKey(fromNodeIndex))
            {
                _edges.Add(fromNodeIndex, new List<Edge>());
            }
            _edges[fromNodeIndex].Add(new Edge(toNodeIndex, EdgeCost(fromNode, toNode)));
        }
    }

    public abstract bool ShouldAddEdge(Node fromNode, Node toNode);
    public abstract int EdgeCost(Node fromNode, Node toNode);

    public Edge[] Dijkstra()
    {
        var source = _nameToIndex[_s];
        var sink = _nameToIndex[_t];

        return Dijkstra(source, sink);
    }

    private Edge[] Dijkstra(uint s, uint t)
    {
        var dist = new int[_n];
        var prevEdge = new Edge[_n];
        var queue = new PriorityQueue<uint, int>();

        foreach (var nodeIndex in _nameToIndex.Values)
        {
            dist[nodeIndex] = int.MaxValue;
            prevEdge[nodeIndex] = null;
        }
        dist[s] = 0;
        
        queue.Enqueue(s, dist[s]);
        var fromNodeIndex = s;
        
        while (queue.Count > 0 && (fromNodeIndex = queue.Dequeue()) != t)
        {
            if (_edges.ContainsKey(fromNodeIndex))
            {
                foreach (var edge in _edges[fromNodeIndex])
                {
                    Relax(queue, dist, prevEdge, fromNodeIndex, edge);
                }
            }
        }

        return prevEdge;
    }

    private void Relax(PriorityQueue<uint, int> queue, int[] dist, Edge[] prev, uint fromNodeIndex, Edge e)
    {
        var toNodeIndex = e.ToNodeIndex;
        if (dist[toNodeIndex] <= dist[fromNodeIndex] + e.Weight) return;

        var cost = dist[fromNodeIndex] + e.Weight;
        dist[toNodeIndex] = cost;
        prev[toNodeIndex] = new Edge(fromNodeIndex, cost);

        queue.Enqueue(toNodeIndex, dist[toNodeIndex]);
    }
}
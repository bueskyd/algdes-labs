using Dijkstra.NET.Graph;
using RedScare.entities;

namespace RedScare
{
    public abstract class GenericGraph
    {
        private const string RedNode = "*";
        private const string UndirectedSplitter = "--";
        private const string DirectedSplitter = "->";

        private readonly int _redCost;
        private readonly int _blackCost;
        
        protected readonly Graph<Node, bool> _graph;
        protected readonly Dictionary<string, uint> _nameToIndex;

        private readonly long _n;
        private readonly long _m;
        private readonly long _r;
        protected readonly string _s;
        protected readonly string _t;
        
        public GenericGraph(int redCost, int blackCost)
        {
            _redCost = redCost;
            _blackCost = blackCost;
            
            _graph = new Graph<Node, bool>();
            _nameToIndex = new Dictionary<string, uint>();

            var firstLine = Console.ReadLine().Split();
            _n = long.Parse(firstLine[0]);
            _m = long.Parse(firstLine[1]);
            _r = long.Parse(firstLine[2]);
            
            var secondLine = Console.ReadLine().Split();
            _s = secondLine[0];
            _t = secondLine[1];
            
            ParseInput();
        }

        public abstract void Solve();

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
            var nodeId = _graph.AddNode(node);
            _nameToIndex.Add(node.Name, nodeId);
        }

        private void AddEdge(string? edgeInput, bool undirected)
        {
            var splitter = undirected ? UndirectedSplitter : DirectedSplitter;
            
            var edgeLine = edgeInput.Split(splitter, StringSplitOptions.TrimEntries);

            var from = edgeLine[0];
            var to = edgeLine[1];

            var fromNode = _graph[_nameToIndex[from]];
            var toNode = _graph[_nameToIndex[to]];

            AddNode(fromNode, toNode);

            if (!undirected)
            {
                AddNode(toNode, fromNode);
            }

        }

        protected void AddNode(INode<Node, bool>? fromNode, INode<Node, bool>? toNode)
        {
            if (ShouldAddEdge(toNode.Item))
            {
                _graph.Connect(fromNode.Key, toNode.Key, NodeToCost(toNode.Item), true);
            }
        }

        protected virtual bool ShouldAddEdge(Node toNode)
        {
            return true;
        }
        
        private int NodeToCost(Node node)
        {
            return node.Color switch
            {
                Color.Black => _blackCost,
                Color.Red => _redCost,
                _ => throw new ArgumentOutOfRangeException(nameof(node.Color), node.Color, "Invalid color")
            };
        }
    }
}
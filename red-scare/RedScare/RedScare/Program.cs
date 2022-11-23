namespace RedScare
{
    public class Helpers {
        public static bool IsCyclic(Graph g) {
            // Inspired from: https://www.geeksforgeeks.org/detect-cycle-in-a-directed-graph-using-bfs/?ref=rp
            if (g.directed) {
                var visited = 0;
                var queue = new Queue<int>();
                var in_degree = new int[g.adjacent.Count];
                for(int v = 0; v < g.adjacent.Count; v++) 
                    for(int e = 0; e < g.adjacent[v].edges.Count; e++) 
                        in_degree[g.adjacent[v].edges[e].to]++;
                for(int v = 0; v < in_degree.Length; v++) if (in_degree[v] == 0) queue.Enqueue(v);   

                while(queue.Count > 0) {
                    var current_id = queue.Dequeue();
                    visited++;
                    for(int e = 0; e < g.adjacent[current_id].edges.Count; e++) {
                        if (g.adjacent[current_id].edges[e].from == current_id) in_degree[g.adjacent[current_id].edges[e].to]--;
                        if (in_degree[g.adjacent[current_id].edges[e].to] == 0) queue.Enqueue(g.adjacent[current_id].edges[e].to);
                    }
                }
                if (visited != g.adjacent.Count) return true;
                else return false;
            }
            // Custom
            else {
                var visited = new HashSet<int>();
                var queue = new Queue<(int to, int from)>();
                queue.Enqueue((0, -1));

                while(queue.Count > 0) {
                    var (current_id, from_id) = queue.Dequeue();
                    var current_node = g.adjacent[current_id];
                    visited.Add(current_id);
                    foreach(var edge in current_node.edges) {
                        if (edge.from == current_id) {
                            if (edge.to == from_id) continue;
                            if (visited.Contains(edge.to)) return true;
                            else queue.Enqueue((edge.to, current_id));
                        }
                    }
                }
                return false;
            }
        }
    }

    public class FlowEdge
    {
        public FlowEdge other;
        public int from;
        public int to;
        public int id;
        public bool isResidualEdge;
    }

    public class FlowGraph
    {
        public List<List<FlowEdge>> adjacent = new();
        public int superSink;
        public List<int> flows = new();
        public List<int> capacities = new();
        public int edgeToSuperSink0;
        public int edgeToSuperSink1;
    }

    public class Edge
    {
        public int from, to;
    }

    public class Node
    {
        public List<Edge> edges = new();
        public string name;
        public bool red;
    }

    public class Graph
    {
        public bool directed = false;
        public List<Node> adjacent = new();
        public int redNodes;
    }

    public class RedScare
    {
        private Graph graph;
        private Dictionary<string, int> nameToId;
        private string s, t;
        private int sId, tId;

        public void ReadInput(TextReader reader)
        {
            string[] line = reader.ReadLine().Split(' ');
            int n = int.Parse(line[0]);
            int m = int.Parse(line[1]);
            int r = int.Parse(line[2]);
            line = reader.ReadLine().Split(' ');
            nameToId = new();
            s = line[0];
            t = line[1];
            graph = new();
            graph.redNodes = r;
            
            for (int i = 0; i < n; i++)
            {
                line = reader.ReadLine().Split(
                    ' ',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                string name = line[0];
                nameToId.Add(name, i);
                bool red = line.Length > 1;
                graph.adjacent.Add(new Node { name = name, red = red });
            }
            for (int i = 0; i < m; i++)
            {
                line = reader.ReadLine().Split(
                    ' ',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                nameToId.TryGetValue(line[0], out int u);
                nameToId.TryGetValue(line[2], out int v);
                graph.adjacent[u].edges.Add(new Edge
                {
                    from = u,
                    to = v
                });
                if (line[1] == "--")
                {
                    graph.adjacent[v].edges.Add(new Edge
                    {
                        from = v,
                        to = u
                    });
                }
                else graph.directed = true;
            }
            nameToId.TryGetValue(s, out sId);
            nameToId.TryGetValue(t, out tId);
        }

        public int None()
        {
            PriorityQueue<int, int> nodes = new();
            nodes.Enqueue(sId, 0);
            int[] costs = new int[graph.adjacent.Count];
            for (int i = 0; i < costs.Length; i++)
                costs[i] = -1;
            costs[sId] = 0;
            while (nodes.Count > 0)
            {
                int nodeId = nodes.Dequeue();
                int cost = costs[nodeId];
                var node = graph.adjacent[nodeId];
                for (int i = 0; i < node.edges.Count; i++)
                {
                    var edge = node.edges[i];
                    if (graph.adjacent[edge.to].red && edge.to != tId)
                        continue;
                    int c = cost + 1;
                    if (costs[edge.to] == -1 || c < costs[edge.to])
                    {
                        costs[edge.to] = c;
                        nodes.Enqueue(edge.to, c);
                    }
                }    
            }
            return costs[tId];
        }

        public int Few()
        {
            PriorityQueue<int, int> nodes = new();
            nodes.Enqueue(sId, 0);
            int[] costs = new int[graph.adjacent.Count];
            for (int i = 0; i < costs.Length; i++)
                costs[i] = -1;
            costs[sId] = graph.adjacent[sId].red ? 1 : 0;
            while (nodes.Count > 0)
            {
                int nodeId = nodes.Dequeue();
                int cost = costs[nodeId];
                var node = graph.adjacent[nodeId];
                for (int i = 0; i < node.edges.Count; i++)
                {
                    var edge = node.edges[i];
                    int edgeWeight = graph.adjacent[edge.to].red ? 1 : 0;
                    int c = cost + edgeWeight;
                    if (costs[edge.to] == -1 || c < costs[edge.to])
                    {
                        costs[edge.to] = c;
                        nodes.Enqueue(edge.to, c);
                    }
                }
            }
            return costs[tId];
        }

        private int Many(int nodeId, int[] most)
        {
            if (most[nodeId] != -1)
                return most[nodeId];
            Node node = graph.adjacent[nodeId];
            most[nodeId] = node.red ? 1 : 0;
            int max = 0;
            for (int i = 0; i < node.edges.Count; i++)
                max = Math.Max(max, Many(node.edges[i].to, most));
            return most[nodeId] += max;
        }

        public int Many()
        {
            int[] most = new int[graph.adjacent.Count];
            for (int i = 0; i < most.Length; i++)
                most[i] = -1;
            return Many(sId, most);
        }

        private (List<FlowEdge>, int) BuildPath(FlowGraph graph, List<(FlowEdge, int)> from, int nodeId)
        {
            List<FlowEdge> path = new()
            {
                from[graph.superSink].Item1
            };
            int prev = from[nodeId].Item2;
            int bottleneck = int.MaxValue;
            if (graph.capacities[path[0].id] != -1)
                bottleneck = Math.Min(bottleneck, Remaining(graph, path[0].id));

            while (prev != nodeId)
            {
                FlowEdge edge = from[prev].Item1;
                if (graph.capacities[edge.id] != -1)
                    bottleneck = Math.Min(bottleneck, Remaining(graph, edge.id));
                path.Add(edge);
                prev = from[prev].Item2;
            }
            return (path, bottleneck);
        }

        private (List<FlowEdge>, int) FindPath(FlowGraph graph, int nodeId)
        {
            List<(FlowEdge, int)> from = new();
            for (int i = 0; i < graph.adjacent.Count; i++)
                from.Add((null, -1));
            from[nodeId] = (null, nodeId);
            Stack<int> stack = new();
            stack.Push(nodeId);
            while (stack.Count > 0)
            {
                int node = stack.Pop();
                var adjacent = graph.adjacent[node];
                foreach (var edge in adjacent)
                {
                    if (from[edge.to].Item2 != -1)
                        continue;
                    if (graph.capacities[edge.id] == Math.Abs(graph.flows[edge.id]))
                        continue;
                    stack.Push(edge.to);
                    from[edge.to] = (edge, node);
                    if (edge.to == graph.superSink)
                        return BuildPath(graph, from, nodeId);
                }
            }
            return (null, -1);
        }

        private int Remaining(FlowGraph graph, int edgeId)
        {
            return graph.capacities[edgeId] - Math.Abs(graph.flows[edgeId]);
        }

        private void Augment(FlowGraph graph, List<FlowEdge> path, int bottleneck)
        {
            foreach (FlowEdge edge in path)
            {
                if (graph.capacities[edge.id] != -1)
                    if (edge.isResidualEdge)
                        graph.flows[edge.id] -= bottleneck;
                    else
                        graph.flows[edge.id] += bottleneck;
            }
        }

        private FlowGraph CreateFlowGraph(Graph graph)
        {
            //Assumes that the graph is undirected
            var flowGraph = new FlowGraph();
            var edgesCreated = new HashSet<(int, int)>();
            int edgePairId = 0;
            for (int i = 0; i < graph.adjacent.Count; i++)
                flowGraph.adjacent.Add(new List<FlowEdge>());
            for (int i = 0; i < graph.adjacent.Count; i++)
            {
                var edges = graph.adjacent[i].edges;
                for (int j = 0; j < edges.Count; j++)
                {
                    if (edgesCreated.Contains((i, edges[j].to)))
                        continue;
                    flowGraph.flows.Add(0);
                    flowGraph.capacities.Add(1);
                    FlowEdge edge = new()
                    {
                        from = i,
                        to = edges[j].to,
                        id = edgePairId,
                        isResidualEdge = false
                    };
                    FlowEdge residual = new()
                    {
                        from = edges[j].to,
                        to = i,
                        id = edgePairId,
                        isResidualEdge = true
                    };
                    edgePairId++;
                    edge.other = residual;
                    residual.other = edge;
                    flowGraph.adjacent[i].Add(edge);
                    flowGraph.adjacent[edges[j].to].Add(residual);
                    edgesCreated.Add((i, edges[j].to));
                }
            }
            flowGraph.flows.Add(0);
            flowGraph.capacities.Add(1);
            flowGraph.superSink = flowGraph.adjacent.Count;
            flowGraph.adjacent.Add(new List<FlowEdge>());
            FlowEdge sIdToSink = new()
            {
                from = sId,
                to = flowGraph.superSink,
                id = edgePairId,
                isResidualEdge = false
            };
            FlowEdge sinkToSId = new()
            {
                from = flowGraph.superSink,
                to = sId,
                id = edgePairId,
                isResidualEdge = true
            };
            sIdToSink.other = sinkToSId;
            sinkToSId.other = sIdToSink;
            flowGraph.adjacent[sId].Add(sIdToSink);
            flowGraph.adjacent[flowGraph.superSink].Add(sinkToSId);

            flowGraph.flows.Add(0);
            flowGraph.capacities.Add(1);
            FlowEdge tIdToSink = new()
            {
                from = tId,
                to = flowGraph.superSink,
                id = edgePairId + 1,
                isResidualEdge = false
            };
            FlowEdge sinkToTId = new()
            {
                from = flowGraph.superSink,
                to = tId,
                id = edgePairId + 1,
                isResidualEdge = true
            };
            tIdToSink.other = sinkToTId;
            sinkToTId.other = tIdToSink;
            flowGraph.adjacent[tId].Add(tIdToSink);
            flowGraph.adjacent[flowGraph.superSink].Add(sinkToTId);
            flowGraph.edgeToSuperSink0 = edgePairId;
            flowGraph.edgeToSuperSink1 = edgePairId + 1;
            return flowGraph;
        }

        private bool UndirectedAcyclicSome(int nodeId, FlowGraph flowGraph)
        {
            while (true)
            {
                var (path, bottleneck) = FindPath(flowGraph, nodeId);
                if (path is null)
                    break;
                Augment(flowGraph, path, bottleneck);
            }
            int flow =
                flowGraph.flows[flowGraph.edgeToSuperSink0] +
                flowGraph.flows[flowGraph.edgeToSuperSink1];
            return flow == 2;
        }

        public bool UndirectedCyclicSome()
        {
            //Transform the graph to enable running Ford-Fulkerson.
            //Could have been prevented by doing this when the graph is read from file.
            var flowGraph = CreateFlowGraph(graph);

            for (int i = 0; i < graph.adjacent.Count; i++)
                if (graph.adjacent[i].red)
                {
                    if (UndirectedAcyclicSome(i, flowGraph))
                        return true;
                    for (int j = 0; j < flowGraph.flows.Count; j++)
                        flowGraph.flows[j] = 0;
                }
            return false;
        }

        public bool AcyclicSome() => Many() > 0;

        public bool Alternate()
        {
            PriorityQueue<int, int> nodes = new();
            nodes.Enqueue(sId, 0);
            int[] costs = new int[graph.adjacent.Count];
            for (int i = 0; i < costs.Length; i++)
                costs[i] = -1;
            costs[sId] = 0;
            while (nodes.Count > 0)
            {
                int nodeId = nodes.Dequeue();
                int cost = costs[nodeId];
                var node = graph.adjacent[nodeId];
                for (int i = 0; i < node.edges.Count; i++)
                {
                    var edge = node.edges[i];
                    if (node.red == graph.adjacent[edge.to].red)
                        continue;
                    int c = cost + 1;
                    if (costs[edge.to] == -1 || c < costs[edge.to])
                    {
                        costs[edge.to] = c;
                        nodes.Enqueue(edge.to, c);
                    }
                }
            }
            return costs[tId] != -1;
        }

        public static void Main(string[] args)
        {
            var redScare = new RedScare();
            string dir = "..\\..\\..\\..\\..\\data";
            if (args.Length != 0)
                dir = args[0];
            foreach (var file in Directory.EnumerateFiles(dir))
            {
                if (file.EndsWith(".md"))
                    continue;
                using var reader = new StreamReader(file);
                redScare.ReadInput(reader);
                Console.WriteLine($"{Path.GetFileName(file)}:\t");
                Console.WriteLine($"\tNone = {redScare.None()}");
                bool isCyclic = Helpers.IsCyclic(redScare.graph);
                if (isCyclic)
                {
                    if (redScare.graph.directed)
                        Console.WriteLine("\tGraph is cyclic and directed. Skipping some as it would run in non-polynomial time.");
                    else
                        Console.WriteLine($"\tSome = {redScare.UndirectedCyclicSome()}");
                }
                else
                    Console.WriteLine($"\tSome = {redScare.AcyclicSome()}");
                Console.WriteLine($"\tFew = {redScare.Few()}");
                if (!isCyclic)
                    Console.WriteLine($"\tMany = {redScare.Many()}");
                else
                    Console.WriteLine("\tGraph is cyclic. Skipping Many as it would run in non-polynomial time.");
                Console.WriteLine($"\tAlternate = {redScare.Alternate()}");
                Console.WriteLine();
            }
            Console.WriteLine();

            /*var redScare = new RedScare();
            using var reader = Console.In;
            redScare.ReadInput(reader);
            Console.WriteLine($"\tNone = {redScare.None()}");
            Console.WriteLine($"\tSome = {redScare.Some()}");
            Console.WriteLine($"\tFew = {redScare.Few()}");
            Console.WriteLine($"\tMany = {redScare.Many()}");
            Console.WriteLine($"\tAlternate = {redScare.Alternate()}");
            Console.WriteLine();
            Console.WriteLine();*/
        }
    }
}

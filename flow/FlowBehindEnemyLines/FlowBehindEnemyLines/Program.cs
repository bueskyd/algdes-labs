namespace Program
{
    public class Edge
    {
        public Edge other;
        public int from;
        public int to;
        public int id;
        public bool isResidualEdge;
    }

    public class Graph
    {
        public List<List<Edge>> adjacent = new();
        public int source;
        public int sink;
    }

    public class FlowBehindEnemyLines
    {
        private static Graph graph = new();
        private static List<int> flows = new();
        private static List<int> capacities = new();

        private static void ReadInput()
        {
            using var reader = new StreamReader("..\\..\\data\\rail.txt");
            int n = int.Parse(reader.ReadLine());
            for (int i = 0; i < n; i++)
            {
                reader.ReadLine();
                graph.adjacent.Add(new List<Edge>());
            }
            int m = int.Parse(reader.ReadLine());
            for (int i = 0; i < m; i++)
            {
                flows.Add(0);
                string[] words = reader.ReadLine().Split(' ');
                int a = int.Parse(words[0]);
                int b = int.Parse(words[1]);
                int c = int.Parse(words[2]);
                capacities.Add(c);
                Edge edge = new()
                {
                    from = a,
                    to = b,
                    id = i,
                    isResidualEdge = false
                };
                Edge residual = new()
                {
                    from = b,
                    to = a,
                    id = i,
                    isResidualEdge = true
                };
                edge.other = residual;
                residual.other = edge;
                graph.adjacent[a].Add(edge);
                graph.adjacent[b].Add(residual);
            }
            graph.source = 0;
            graph.sink = n - 1;
        }

        private static int Remaining(int edgeId)
        {
            return capacities[edgeId] - Math.Abs(flows[edgeId]);
        }

        private static (List<Edge>, int) FindPath()
        {
            List<(Edge, int)> from = new();
            for (int i = 0; i < graph.adjacent.Count; i++)
                from.Add((null, -1));
            from[graph.source] = (null, graph.source);
            Stack<int> stack = new();
            stack.Push(graph.source);
            while (stack.Count > 0)
            {
                int node = stack.Pop();
                var adjacent = graph.adjacent[node];
                foreach (var edge in adjacent)
                {
                    if (from[edge.to].Item2 != -1)
                        continue;
                    if (capacities[edge.id] == Math.Abs(flows[edge.id]))
                        continue;
                    stack.Push(edge.to);
                    from[edge.to] = (edge, node);
                    if (edge.to == graph.sink)
                        return BuildPath(from);
                }
            }
            return (null, -1);
        }

        private static (List<Edge>, int) BuildPath(List<(Edge, int)> from)
        {
            List<Edge> path = new()
            {
                from[graph.sink].Item1
            };
            int prev = from[graph.sink].Item2;
            int bottleneck = int.MaxValue;
            if (capacities[path[0].id] != -1)
                bottleneck = Math.Min(bottleneck, Remaining(path[0].id));

            while (prev != graph.source)
            {
                Edge edge = from[prev].Item1;
                if (capacities[edge.id] != -1)
                    bottleneck = Math.Min(bottleneck, Remaining(edge.id));
                path.Add(edge);
                prev = from[prev].Item2;
            }
            return (path, bottleneck);
        }

        private static void Augment(List<Edge> path, int bottleneck)
        {
            foreach (Edge edge in path)
            {
                if (capacities[edge.id] != -1)
                    if (edge.isResidualEdge)
                        flows[edge.id] -= bottleneck;
                    else
                        flows[edge.id] += bottleneck;
            }
        }

        private static List<Edge> FindMinCut()
        {
            List<bool> visited = new();
            for (int i = 0; i < flows.Count; i++)
                visited.Add(false);
            Queue<int> queue = new();
            queue.Enqueue(graph.source);
            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                if (visited[node])
                    continue;
                visited[node] = true;
                var adjacent = graph.adjacent[node];
                foreach (Edge edge in adjacent)
                {
                    if (!edge.isResidualEdge && Math.Abs(flows[edge.id]) == capacities[edge.id])
                        continue;
                    queue.Enqueue(edge.to);
                }
            }
            List<Edge> minimumCut = new();
            for (int i = 0; i < visited.Count; i++)
                if (visited[i])
                    foreach (Edge edge in graph.adjacent[i])
                        if (!visited[edge.to])
                            minimumCut.Add(edge);
            return minimumCut;
        }

        private static int FindMaxFlow(List<Edge> minCut)
        {
            int maxFlow = 0;
            foreach (Edge edge in minCut)
                maxFlow += Math.Abs(flows[edge.id]);
            return maxFlow;
        }

        private static void Solve()
        {
            while (true)
            {
                var (path, bottleneck) = FindPath();
                if (path is null)
                    break;
                Augment(path, bottleneck);
            }
            var minimumCut = FindMinCut();
            int maxFlow = FindMaxFlow(minimumCut);
            Console.WriteLine(maxFlow);
            foreach (Edge edge in minimumCut)
                Console.WriteLine($"{edge.from} {edge.to} {Math.Abs(flows[edge.id])}");
        }

        public static void Main()
        {
            ReadInput();
            Solve();
        }
    }
}

namespace Program
{
    public class Edge
    {
        public int other;
        public int capacity;
        public int from;
        public int to;
        public bool isRecidualEdge;

        public override string ToString()
        {
            return $"({from}, {to}, {capacity}, {isRecidualEdge})";
        }
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

        private static void ReadInput()
        {
            int n = int.Parse(Console.ReadLine());
            for (int i = 0; i < n; i++)
            {
                Console.ReadLine();
                graph.adjacent.Add(new List<Edge>());
            }
            int m = int.Parse(Console.ReadLine());
            for (int i = 0; i < m; i++)
            {
                string[] words = Console.ReadLine().Split(' ');
                int a = int.Parse(words[0]);
                int b = int.Parse(words[1]);
                int c = int.Parse(words[2]);
                Edge edge = new()
                {
                    capacity = c == -1 ? -1 : 0,
                    from = a,
                    to = b,
                    other = graph.adjacent[b].Count,
                    isRecidualEdge = false
                };
                Edge residual = new()
                {
                    capacity = c,
                    from = b,
                    to = a,
                    other = graph.adjacent[a].Count,
                    isRecidualEdge = true
                };
                graph.adjacent[a].Add(edge);
                graph.adjacent[b].Add(residual);
            }
            graph.source = 0;
            graph.sink = n - 1;
        }

        private static Edge GetOther(Edge edge)
        {
            return graph.adjacent[edge.to][edge.other];
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
                    if (edge.isRecidualEdge && edge.capacity == 0)
                        continue;
                    Edge other = GetOther(edge);
                    if (!edge.isRecidualEdge && other.capacity == 0)
                        continue;
                    if (from[edge.to].Item2 != -1)
                        continue;
                    stack.Push(edge.to);
                    from[edge.to] = (edge, node);
                    if (edge.to == graph.sink)
                    {
                        stack.Clear();
                        break;
                    }
                }
            }
            if (from[graph.sink].Item2 == -1)
                return (null, -1);
            List<Edge> path = new()
            {
                from[graph.sink].Item1
            };
            int prev = from[graph.sink].Item2;
            int bottleneck = int.MaxValue;
            if (path[0].capacity != -1)
                if (path[0].isRecidualEdge)
                    bottleneck = path[0].capacity;
                else
                    bottleneck = GetOther(path[0]).capacity;

            while (prev != graph.source)
            {
                Edge edge = from[prev].Item1;
                if (edge.capacity != -1)
                {
                    if (edge.isRecidualEdge)
                        bottleneck = Math.Min(bottleneck, edge.capacity);
                    else
                    {
                        Edge recidual = GetOther(edge);
                        bottleneck = Math.Min(bottleneck, recidual.capacity);
                    }
                }
                path.Add(edge);
                prev = from[prev].Item2;
            }
            return (path, bottleneck);
        }

        private static void Augment(List<Edge> path, int bottleneck)
        {
            foreach (Edge edge in path)
            {
                Edge other = GetOther(edge);
                if (edge.isRecidualEdge)
                {
                    if (edge.capacity > 0)
                    {
                        edge.capacity -= bottleneck;
                        graph.adjacent[edge.to][edge.other].capacity += bottleneck;
                    }
                }
                else if (other.capacity > 0)
                {
                    edge.capacity += bottleneck;
                    graph.adjacent[edge.to][edge.other].capacity -= bottleneck;
                }
            }
        }

        private static List<Edge> FindMinCut()
        {
            List<Edge> minimumCut = new();
            List<bool> visited = new();
            for (int i = 0; i < graph.adjacent.Count; i++)
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
                    if (edge.isRecidualEdge)
                        continue;
                    Edge recidual = GetOther(edge);
                    if (recidual.capacity == 0)
                        minimumCut.Add(edge);
                    else
                        queue.Enqueue(edge.to);
                }
            }
            return minimumCut;
        }

        private static int FindMaxFlow(List<Edge> minCut)
        {
            int maxFlow = 0;
            foreach (Edge edge in minCut)
                maxFlow += edge.capacity;
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
                Console.WriteLine($"{edge.from} {edge.to} {edge.capacity}");
        }

        public static void Main()
        {
            ReadInput();
            Console.WriteLine();
            Solve();
            Console.WriteLine();
        }
    }
}

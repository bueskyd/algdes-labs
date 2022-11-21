namespace RedScare
{
    public class Helpers {
        public static bool IsCyclic(Graph g) {

            if (g.directed) {
                throw new Exception("IsCyclic is not supported for directed graphs... yet");
            }
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
    }

    public class RedScare
    {
        private Graph graph;
        private Dictionary<string, int> nameToId;
        private string s, t;
        private int sId, tId;

        public void ReadInput(StreamReader reader)
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

        public bool Some() => Many() > 0;

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

        private int Many(int nodeId, bool[] visited)
        {
            if (visited[nodeId])
                return 0;
            Node node = graph.adjacent[nodeId];
            if (nodeId == tId)
                return node.red ? 1 : 0;
            int max = 0;
            visited[nodeId] = true;
            for (int i = 0; i < node.edges.Count; i++)
                max = Math.Max(max, Many(node.edges[i].to, visited));
            visited[nodeId] = false;
            return max + (node.red ? 1 : 0);
        }

        public int Many() => Many(sId, new bool[graph.adjacent.Count]);

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
            foreach (var file in Directory.EnumerateFiles(args[0]))
            {
                if (file.EndsWith(".md"))
                    continue;
                using var reader = new StreamReader(file);
                redScare.ReadInput(reader);
                Console.WriteLine($"{Path.GetFileName(file)}:\t");
                try {
                    System.Console.WriteLine($" IsCyclic: {Helpers.IsCyclic(redScare.graph)}");
                }
                catch (Exception) {}
                // Console.WriteLine(
                //     $"\tNone = {redScare.None()}\n" +
                //     $"\tSome = {redScare.Some()}\n" +
                //     $"\tFew = {redScare.Few()}\n" +
                //     $"\tMany = {redScare.Many()}\n" +
                //     $"\tAlternate = {redScare.Alternate()}\n");
            }
            Console.WriteLine();
        }
    }
}

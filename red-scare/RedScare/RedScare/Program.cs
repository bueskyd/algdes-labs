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

        private int Many(int nodeId, int[] most)
        {
            if (most[nodeId] != -1)
                return most[nodeId];
            Node node = graph.adjacent[nodeId];
            int max = 0;
            for (int i = 0; i < node.edges.Count; i++)
                max = Math.Max(max, Many(node.edges[i].to, most));
            return most[nodeId] = max + (node.red ? 1 : 0);
        }

        public int Many()
        {
            int[] most = new int[graph.adjacent.Count];
            for (int i = 0; i < most.Length; i++)
                most[i] = -1;
            return Many(sId, most);
        }

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
                try
                {
                    Console.WriteLine($" IsCyclic: {Helpers.IsCyclic(redScare.graph)}");
                }
                catch (Exception) { }
                Console.WriteLine(
                    $"\tNone = {redScare.None()}\n" +
                    $"\tSome = {redScare.Some()}\n" +
                    $"\tFew = {redScare.Few()}\n" +
                    $"\tMany = {redScare.Many()}\n" +
                    $"\tAlternate = {redScare.Alternate()}\n");
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

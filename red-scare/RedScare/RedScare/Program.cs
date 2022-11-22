namespace RedScare
{
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
            foreach (var file in Directory.EnumerateFiles("..\\..\\..\\..\\..\\data\\"))
            {
                if (file == "..\\..\\..\\..\\..\\data\\README.md")
                    continue;
                if (file == "..\\..\\..\\..\\..\\data\\bht.txt")
                    continue;
                using var reader = new StreamReader(file);
                redScare.ReadInput(reader);
                Console.WriteLine($"{Path.GetFileName(file)}:\t");
                Console.WriteLine($"\tNone = {redScare.None()}");
                Console.WriteLine($"\tSome = {redScare.Some()}");
                Console.WriteLine($"\tFew = {redScare.Few()}");
                Console.WriteLine($"\tMany = {redScare.Many()}");
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

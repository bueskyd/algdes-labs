using System.Collections.Generic;
using System;
using System.Linq;

namespace ElementaryMath
{
    class Graph
    {
        private const int Ops = 3;
        private readonly Dictionary<long, int> ValueToIndex;
        private readonly Dictionary<int, long> IndexToValue;
        private readonly Pair[] _pairs;
        private readonly int[,] _graph;

        private Graph()
        {
            ValueToIndex = new Dictionary<long, int>();
            IndexToValue = new Dictionary<int, long>();
            
            // number of nodes
            var n = int.Parse(Console.ReadLine());
            var size = n * Ops + n + 4;

            _graph = new int[size, size];
            _pairs = new Pair[n];

            for (var i = 1; i <= n; i++)
            {
                var line = Console.ReadLine()?.Split(" ");
                var a = long.Parse(line[0]);
                var b = long.Parse(line[1]);

                _pairs[i - 1] = new Pair(a, b);
                _graph[0, i] = 1;

                var mulIndex = AddValue(a * b, n+2);
                _graph[i, mulIndex] = 1;
                _graph[mulIndex, size - 1] = 1;

                var addIndex = AddValue(a + b, n+2);
                _graph[i, addIndex] = 1;
                _graph[addIndex, size - 1] = 1;

                var subIndex = AddValue(a - b, n+2);
                _graph[i, subIndex] = 1;
                _graph[subIndex, size - 1] = 1;
            }
        }

        public static void Main()
        {
            var graph = new Graph();
            graph.FordFulkerson();
        }

        public void FordFulkerson()
        {
            var n = _graph.GetLength(1);
            Augment(_graph);

            var visited = 0;
            var results = new long[_pairs.Length];
            
            for (var i = 0; i < n; i++) {
                
                if (_graph[n - 1, i] > 0)
                {
                    for (var j = 0; j < _pairs.Length + 1; j++)
                    {
                        if (_graph[i, j] > 0)
                        {
                            results[j - 1] = IndexToValue[i];
                        }
                    }

                    visited++;
                }
            }
            if (visited < _pairs.Length)
            {
                Console.WriteLine("impossible");
                return;
            }

            for (var i = 0; i < _pairs.Length; i++)
            {
                var pair = _pairs[i];
                
                var a = pair.A;
                var b = pair.B;
                var result = results[i];

                if (a * b == result)
                {
                    Console.WriteLine($"{a} * {b} = {result}");
                }
                else if (a + b == result)
                {
                    Console.WriteLine($"{a} + {b} = {result}");
                }
                else
                {
                    Console.WriteLine($"{a} - {b} = {result}");
                }
            }
        }

        private void Augment(int[,] residualGraph)
        {
            var sink = residualGraph.GetLength(1) - 1;
            
            // while there is a path in the residual graph
            int[] path;
            while ((path = BFS(residualGraph)).Length > 0 && path[path.Length - 1] != 0)
            {

                for (var i = sink; i != 0; i = path[i])
                {
                    var j = path[i];
                    residualGraph[i, j] += 1;
                    residualGraph[j, i] -= 1;
                }
            }
        }
        
        private int[] BFS(int[,] residualGraph)
        {
            var n = residualGraph.GetLength(1);
            var visited = new bool[n];
            var path = new int[n];
            
            var queue = new Stack<int>();
            queue.Push(0);
            visited[0] = true;
            
            while (queue.Count != 0)
            {
                var i = queue.Pop();
                for (var j = 0; j < residualGraph.GetLength(0); j++)
                {
                    if (residualGraph[i, j] > 0 && !visited[j])
                    {
                        queue.Push(j);
                        visited[j] = true;
                        path[j] = i;

                        if (j == n - 1)
                        {
                            return path;
                        }
                    }
                }
            }

            return path;
        }

        private int AddValue(long value, int size)
        {
            if (!ValueToIndex.ContainsKey(value))
            {
                ValueToIndex.Add(value, ValueToIndex.Count + size);
                IndexToValue.Add(ValueToIndex[value], value);
            }

            return ValueToIndex[value];
        }

        class Pair
        {
            public long A;
            public long B;

            public Pair(long a, long b)
            {
                A = a;
                B = b;
            }

            public override string ToString()
            {
                return $"{nameof(A)}: {A}, {nameof(B)}: {B}";
            }
        }
    }
}
namespace Flow;


// C# program for finding min-cut in the given graph
using System;
using System.Collections.Generic;
 
class Graph
{
    private readonly int[,] _graph;
    
    public Graph()
    {
        // number of nodes
        var n = int.Parse(Console.ReadLine());
        _graph = new int[n, n];
            
        for (var i = 0; i < n; i++)
        {
            // fuck the names
            Console.ReadLine();
        }
            
        // number of edges
        var m = int.Parse(Console.ReadLine());
        for (var i = 0; i < m; i++)
        {
            var line = Console.ReadLine().Split(" ");
            var from = int.Parse(line[0]);
            var to = int.Parse(line[1]);
            var capacity = int.Parse(line[2]);
            
            _graph[from, to] = capacity == -1 ? int.MaxValue : capacity;
            _graph[to, from] = capacity == -1 ? int.MaxValue : capacity;
        }
    }
    public static void Main()
    {
        var graph = new Graph();
        graph.FordFulkerson();
    }

    public void FordFulkerson()
    {
        // create the residual graph
        var residualGraph = CreateResidualGraph(_graph);
        
        Augment(residualGraph);

        // find all reachable nodes from the source
        var isVisited = DFS(residualGraph);

        // or the flow
        var minCutCapacity = 0;
        for (var i = 0; i < _graph.GetLength(0); i++)
        {
            for (var j = 0; j < _graph.GetLength(1); j++)
            {
                // if the edge is included in the minimum cut
                if (_graph[i, j] > 0 && isVisited[i] && !isVisited[j])
                {
                    Console.WriteLine(i + " - " + j);
                    minCutCapacity += _graph[i, j];
                }
            }
        }
        Console.WriteLine(minCutCapacity);
    }
    
    private int[,] CreateResidualGraph(int[,] graph)
    {
        var residualGraph = new int[graph.GetLength(0), graph.GetLength(1)];
        
        for (var i = 0; i < graph.GetLength(0); i++)
        {
            for (var j = 0; j < graph.GetLength(1); j++)
            {
                residualGraph[i, j] = graph[i, j];
            }
        }

        return residualGraph;
    }

    private void Augment(int[,] residualGraph)
    {
        var sink = residualGraph.GetLength(0) - 1;
        
        // while there is a path in the residual graph
        int[] path;
        while ((path = BFS(residualGraph)).Length > 0 && path[^1] != 0)
        {
            var pathFlow = FindBottleNeck(residualGraph, path);

            for (var i = sink; i != 0; i = path[i])
            {
                var j = path[i];
                residualGraph[i, j] += pathFlow;
                residualGraph[j, i] -= pathFlow;
            }
        }
    }
    
    private int[] BFS(int[,] residualGraph)
    {
        var n = residualGraph.GetLength(0);
        var visited = new bool[n];
        var path = new int[n];
        
        var queue = new Queue<int>();
        queue.Enqueue(0);
        visited[0] = true;
        
        while (queue.Count != 0)
        {
            var i = queue.Dequeue();
            for (var j = 0; j < residualGraph.GetLength(0); j++)
            {
                if (residualGraph[i,j] > 0 && !visited[j])
                {
                    queue.Enqueue(j);
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

    private int FindBottleNeck(int[,] graph, int[] path)
    {
        var sink = path[^1];
        var bottleNeck = int.MaxValue;
        
        for (var i = sink; i != 0; i = path[i])
        {
            var j = path[i];
            bottleNeck = Math.Min(bottleNeck, graph[j, i]);
        }

        return bottleNeck;
    }
    
    private bool[] DFS(int[,] residualGraph)
    {
        var n = residualGraph.GetLength(0);
        var visited = new bool[n];

        var stack = new Stack<int>();
        stack.Push(0);
        visited[0] = true;
        
        while (stack.Count != 0)
        {
            var i = stack.Pop();
            for (var j = 0; j < residualGraph.GetLength(0); j++)
            {
                if (residualGraph[i,j] > 0 && !visited[j])
                {
                    stack.Push(j);
                    visited[j] = true;
                }
            }
        }

        return visited;
    }
}
using System;
using System.Collections.Generic;

namespace Baas
{
    public static class SchedulingAlgorithms
    {

        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            var nodes = new Node[n];
            
            var nodeWeights = Console.ReadLine().Split(" ");
            for (var i = 0; i < n; i++)
            {
                var node = new Node()
                {
                    Weight = int.Parse(nodeWeights[i])
                };

                nodes[i] = node;
            }

            for (var i = 0; i < n; i++)
            {
                var line = Console.ReadLine().Split(" ");
                var amountOfOutgoingEdges = int.Parse(line[0]);
                var outgoingEdges = new int[amountOfOutgoingEdges];

                for (var j = 0; j < amountOfOutgoingEdges; j++)
                {
                    outgoingEdges[j] = int.Parse(line[j + 1]);
                }

                nodes[i].OutgoingEdges = outgoingEdges;
            }
            
            var shortestDuration = ShortestPathFromFinish(nodes);
            Console.WriteLine(shortestDuration);
        }
        
        private static long ShortestPathFromFinish(Node[] nodes)
        {
            var n = nodes.Length;
            var minPathCost = int.MaxValue;
            
            // we need to try setting every nodes weight to 0
            for (var i = 0; i < n; i++)
            {
                var memoizedResults = new int[n];
                Array.Fill(memoizedResults, -1);
            
                // save the old weight, and set it to 0 for this iteration
                var node = nodes[i];
                var weight = node.Weight;
                node.Weight = 0;
                
                // update minPathCost accordingly
                minPathCost = Math.Min(minPathCost, LongestPath(nodes, memoizedResults, n - 1));
                
                // set the nodes weight back to what it used to be
                node.Weight = weight;
            }

            return minPathCost;
        }

        private static int LongestPath(Node[] nodes, int[] memoizedResults, int i)
        {
            if (i < 0) return 0;
            if (memoizedResults[i] != -1)
            {
                return memoizedResults[i];
            }
            
            var node = nodes[i];
            var totalWeight = 0;

            foreach (var t in node.OutgoingEdges)
            {
                var searchedPath = LongestPath(nodes, memoizedResults, t - 1);
                totalWeight = Math.Max(totalWeight, searchedPath);
            }

            totalWeight += node.Weight;
            memoizedResults[i] = totalWeight;
            return totalWeight;
        }

        class Node
        {
            public int Weight { get; set; }
            public int[] OutgoingEdges { get; set; }
        }
    }
}
using System.Collections;
using static System.Array;

namespace StableMatching;

public class StableMatchAlgorithm
{
    /// <summary>
    /// Stable matching
    /// </summary>
    /// <param name="proposers"><b>Sorted</b> array of proposers</param>
    /// <param name="rejecters">Array of rejecters</param>
    /// <returns>Returns a stable matching</returns>
    public static int[] StableMatch(int n, Person[] proposers, Person[] rejecters)
    {
        var freeProposers = new Stack<int>(Enumerable.Range(0, n).ToArray());
        var next = Enumerable.Repeat(0, n).ToArray();
        var matches = Enumerable.Repeat(-1, n).ToArray();
        
        var rank = new int[n][];
        for (var i = 0; i < n; i++)
        {
            rank[i] = new int[n];
            var rejecter = rejecters[i];
            
            for (var j = 0; j < n; j++)
            {
                var proposerId = rejecter.priorities[j];
                // even id's
                rank[i][proposerId / 2] = j;
            }
        }

        return Matches(proposers, freeProposers, next, matches, rank);
    }

    private static int[] Matches(Person[] proposers, Stack<int> freeProposers, int[] next, int[] matches, int[][] rank)
    {
        while (freeProposers.Count > 0)
        {
            var proposerIndex = freeProposers.Pop();
            var proposer = proposers[proposerIndex];
            var rejecterIndexForProposer = next[proposerIndex];
            // uneven id's
            var rejecterIndex = proposer.priorities[rejecterIndexForProposer] / 2 - 1;

            var match = matches[rejecterIndex];

            if (match == -1)
            {
                matches[rejecterIndex] = proposerIndex;
            }
            else if (rank[rejecterIndex][proposerIndex] < rank[rejecterIndex][match])
            {
                matches[rejecterIndex] = proposerIndex;
                freeProposers.Push(match);
            }
            else
            {
                freeProposers.Push(proposerIndex);
            }

            next[proposerIndex] += 1;
        }

        return matches;
    }
}
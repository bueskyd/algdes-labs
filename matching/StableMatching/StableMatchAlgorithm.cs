namespace StableMatching;

public static class StableMatchAlgorithm
{
    /// <summary>
    /// Stable matching
    /// </summary>
    /// <param name="proposers"><b>Sorted</b> array of proposers</param>
    /// <param name="rejecters">Array of rejecters</param>
    /// <returns>Returns an array of a stable matching. Indexes in the array represent
    /// index of the rejecter in rejecters array, and values represent index
    /// of the proposer in the proposer array. </returns>
    public static IEnumerable<int> StableMatch(Person[] proposers, Person[] rejecters)
    {
        if (proposers.Length != rejecters.Length)
        {
            throw new Exception("Proposer and rejecter list must be equal length");
        }

        var n = proposers.Length;
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
                rank[i][proposerId] = j;
            }
        }

        return Matches(proposers, freeProposers, next, matches, rank);
    }

    private static IEnumerable<int> Matches(IReadOnlyList<Person> proposers, Stack<int> freeProposers, IList<int> next, IList<int> matches, IReadOnlyList<int[]> rank)
    {
        while (freeProposers.Count > 0)
        {
            var proposerIndex = freeProposers.Pop();
            var proposer = proposers[proposerIndex];
            var rejecterIndexForProposer = next[proposerIndex];
            var rejecterIndex = proposer.priorities[rejecterIndexForProposer];

            var match = matches[rejecterIndex];

            if (match == -1)
            {
                matches[rejecterIndex] = proposerIndex;
            }
            // first priority = index 0, last priority = index n
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
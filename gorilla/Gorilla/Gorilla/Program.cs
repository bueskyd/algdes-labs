namespace Gorilla
{
    public static class SchedulingAlgorithms
    {

        public static void Main(string[] args)
        {
            var tableParser = new TableParser();
            
            var sequences = ParseSequences().ToList();

            for (var i = 0; i < sequences.Count(); i++)
            {
                var sequenceI = sequences[i];
                for (var j = i + 1; j < sequences.Count(); j++)
                {
                    var sequenceJ = sequences[j];
                    var (optimal, s1, s2) = Alignment(sequenceI, sequenceJ, tableParser);
                    
                    Console.WriteLine($"{sequenceI.Name}--{sequenceJ.Name}: {optimal}");
                    Console.WriteLine(s1);
                    Console.WriteLine(s2);
                }
            }
        }

        private static IEnumerable<Sequence> ParseSequences()
        {
            var nextLine = Console.ReadLine();
            while (nextLine.Length > 0 && nextLine.StartsWith('>'))
            {
                var name = nextLine.Trim().Split()[0];
                var sequenceString = "";
                while ((nextLine = Console.ReadLine()).Length > 0 && !nextLine.StartsWith('>'))
                {
                    sequenceString += nextLine;
                }
                var sequence = sequenceString.Trim().ToCharArray();

                yield return new Sequence
                {
                    Name = name.Substring(1),
                    SequenceString = sequence.ToList()
                };
            }
        }

        private static (int optimal, string s1, string s2) Alignment(Sequence a, Sequence b, TableParser tableParser)
        {
            var aN = a.SequenceString.Count();
            var bN = b.SequenceString.Count();
            
            var arr = new SeqRes[aN, bN];

            for (var i = 0; i < aN; i++)
            {
                arr[i, 0] = new SeqRes
                {
                    Cost = i * tableParser.GetCost(a.SequenceString[i], '*') +
                           tableParser.GetCost(a.SequenceString[i], b.SequenceString[0]),
                    I = a.SequenceString[i],
                    J = b.SequenceString[0],
                    PrevI = i == 0 ? null : i - 1,
                    PrevJ = null
                };
            }
            for (var j = 0; j < bN; j++)
            {

                arr[0, j] = new SeqRes
                {
                    Cost = j * tableParser.GetCost('*', b.SequenceString[j]) +
                           tableParser.GetCost(a.SequenceString[0], b.SequenceString[j]),
                    I = a.SequenceString[0],
                    J = b.SequenceString[j],
                    PrevI = null,
                    PrevJ = j == 0 ? null : j - 1
                };
            }
            
            // save the two used characters in the array, and the index of the previously used result.
            // reverse the string when done.
            for (var i = 1; i < aN; i++)
            {
                for (var j = 1; j < bN; j++)
                {
                    var res = Opt(i, a.SequenceString, j, b.SequenceString, arr, tableParser);
                    arr[i, j] = res;
                }
            }
            
            var optimal = arr[aN - 1, bN - 1];
            var (stringA, stringB) = BuildString(arr, optimal);

            return (optimal.Cost, stringA, stringB);
        }

        
        private static SeqRes Opt(int i, List<char> sequenceI, int j, List<char> sequenceJ, SeqRes[,] arr, TableParser tableParser)
        {
            if (i == 0)
            {
                return new SeqRes 
                {
                    Cost = j * tableParser.GetCost('*', sequenceJ[j]),
                    I = '*', 
                    J = sequenceJ[j], 
                    PrevI = null, 
                    PrevJ = j-1
                };
            }
            
            if (j == 0)
            {
                return new SeqRes
                {
                    Cost = i * tableParser.GetCost(sequenceI[i], '*'),
                    I = sequenceI[i], 
                    J = '*', 
                    PrevI = i-1, 
                    PrevJ = null
                };
            }
            
            var match = tableParser.GetCost(sequenceI[i], sequenceJ[j]) + arr[i - 1, j - 1].Cost;

            var caseI = tableParser.GetCost(sequenceI[i], '*') + arr[i - 1 , j].Cost;

            var caseII = tableParser.GetCost('*', sequenceJ[j]) + arr[i, j - 1].Cost;

            if (match >= caseI && match >= caseII)
            {
                return new SeqRes
                {
                    Cost = match,
                    I = sequenceI[i],
                    J = sequenceJ[j],
                    PrevI = i - 1,
                    PrevJ = j - 1
                };
            } else if (caseI >= match && caseI >= caseII)
            {
                return new SeqRes
                {
                    Cost = caseI,
                    I = sequenceI[i],
                    J = '*',
                    PrevI = i - 1,
                    PrevJ = j
                };
            } else
            {
                return new SeqRes
                {
                    Cost = caseII,
                    I = '*',
                    J = sequenceJ[j],
                    PrevI = i,
                    PrevJ = j - 1
                };
            }
        }

        private static (string s1, string s2) BuildString(SeqRes[,] arr, SeqRes seqRes)
        {
            var chars1 = new List<char> { seqRes.I };
            var chars2 = new List<char> { seqRes.J };

            (chars1, chars2) = BuildString(arr, seqRes.PrevI, seqRes.PrevJ, chars1, chars2);
            chars1.Reverse();
            chars2.Reverse();

            return (new string(chars1.ToArray()), new string(chars2.ToArray()));
        }

        private static (List<char> chars1, List<char> chars2) BuildString(SeqRes[,] arr, int? i, int? j, List<char> chars1, List<char> chars2)
        {
            char? cI = null;
            char? cJ = null;
            if (i == null && j == null)
            {
                return (chars1, chars2);
            }

            if (i == null)
            {
                cI = '*';
                i = 0;
            }

            if (j == null)
            {
                cJ = '*';
                j = 0;
            }

            var seqRes = arr[i.Value, j.Value];

            if (cI == null)
            {
                cI = seqRes.I;
            }
            
            if (cJ == null)
            {
                cJ = seqRes.J;
            }
            
            chars1.Add(cI.Value);
            chars2.Add(cJ.Value);

            return BuildString(arr, seqRes.PrevI, seqRes.PrevJ, chars1, chars2);
        }

        private class SeqRes
        {
            public int Cost { get; set; }
            public char I { get; set; }
            public char J { get; set; }
            public int? PrevI { get; set; }
            public int? PrevJ { get; set; }

            public override string ToString()
            {
                return $"{nameof(Cost)}: {Cost}";
            }
        }

        private class Sequence
        {
            public string Name { get; set; }
            public List<char> SequenceString { get; set; }
        }
    }

    public class TableParser
    {
        private static readonly StreamReader reader = new StreamReader("../../../../../data/BLOSUM62.txt");
        private static readonly Dictionary<char, Dictionary<char, int>> table = new Dictionary<char, Dictionary<char, int>>();

        public TableParser()
        {
            LoadTable();
        }

        private void LoadTable()
        {
            SkipHeaders();

            // first line are main entries in dict
            var chars = reader.ReadLine().Trim().Split("  ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in chars.Select(char.Parse))
            {
                table.Add(c, new Dictionary<char, int>());
            }

            var line = "";
            while ((line = reader.ReadLine()) != null)
            {
                var mappings = line.Trim().Split(new[] { " ", "  " }, StringSplitOptions.RemoveEmptyEntries);
                var second = char.Parse(mappings[0]);

                for (var i = 1; i < mappings.Length; i++)
                {
                    var cost = int.Parse(mappings[i]);
                    table.ElementAt(i - 1).Value.Add(second, cost);
                }
            }
        }

        private void SkipHeaders()
        {
            while ((char) reader.Peek() == '#')
            {
                reader.ReadLine();
            }
        }

        public int GetCost(char a, char b)
        {
            return table[char.ToUpper(a)][char.ToUpper(b)];
        }
    }
}
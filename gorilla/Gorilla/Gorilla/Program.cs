namespace Gorilla
{
    public static class SchedulingAlgorithms
    {

        private static readonly char BlossumMismatch = '*';
        private static readonly char OutputMismatch = '-';
        
        public static void Main(string[] args)
        {
            var tableParser = new TableParser();
            var sequences = ParseSequences().ToList();

            for (var i = 0; i < sequences.Count; i++)
            {
                var sequenceI = sequences[i];
                for (var j = i + 1; j < sequences.Count; j++)
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

                yield return new Sequence
                {
                    Name = name.Substring(1),
                    SequenceString = sequenceString.Trim()
                };
            }
        }

        private static (int optimal, string sI, string sJ) Alignment(Sequence sequenceI, Sequence sequenceJ, TableParser tableParser)
        {
            var iN = sequenceI.SequenceString.Length;
            var jN = sequenceJ.SequenceString.Length;

            var arr = new SeqRes[iN + 1, jN + 1];
            
            // the first link..
            arr[0, 0] = new SeqRes
            {
                Cost = 0,
                PrevI = null,
                PrevJ = null
            };

            for (var i = 1; i < iN; i++)
            {
                arr[i, 0] = new SeqRes
                {
                    Cost = 0,
                    I = sequenceI.SequenceString[i],
                    J = sequenceJ.SequenceString[0],
                    PrevI = i - 1,
                    PrevJ = null
                };
            }
            for (var j = 1; j < jN; j++)
            {
                arr[0, j] = new SeqRes
                {
                    Cost = 0,
                    I = sequenceI.SequenceString[0],
                    J = sequenceJ.SequenceString[j],
                    PrevI = null,
                    PrevJ = j - 1
                };
            }

            for (var j = 0; j < jN + 1; j++)
            {
                for (var i = j == 0 ? 1 : 0; i < iN + 1; i++)
                {
                    var res = Opt(i, sequenceI.SequenceString, j, sequenceJ.SequenceString, arr, tableParser);
                    arr[i, j] = res;
                }
            }
            
            var optimal = arr[iN, jN];
            var (stringA, stringB) = BuildStrings(arr, optimal);

            return (optimal.Cost, stringA, stringB);
        }

        
        private static SeqRes Opt(int i, string sequenceI, int j, string sequenceJ, SeqRes[,] arr, TableParser tableParser)
        {
            var match = int.MinValue;
            var caseI = int.MinValue;
            var caseII = int.MinValue;
            
            if(i > 0 && j > 0)
                match = tableParser.GetCost(sequenceI[i - 1], sequenceJ[j - 1]) + arr[i - 1, j - 1].Cost;
            
            if(i > 0)
                caseI = tableParser.GetMismatchCost(sequenceI[i - 1]) + arr[i - 1, j].Cost;
            
            if(j > 0)
                caseII = tableParser.GetMismatchCost(sequenceJ[j - 1]) + arr[i, j - 1].Cost;

            if (caseI > match && caseI >= caseII)
            {
                return new SeqRes
                {
                    Cost = caseI,
                    I = sequenceI[i - 1],
                    J = OutputMismatch,
                    PrevI = i - 1,
                    PrevJ = j
                };
            }
            
            if (caseII > match && caseII > caseI)
            {
                return new SeqRes
                {
                    Cost = caseII,
                    I = OutputMismatch,
                    J = sequenceJ[j - 1],
                    PrevI = i,
                    PrevJ = j - 1
                };
            }
            return new SeqRes
                {
                    Cost = match,
                    I = sequenceI[i - 1],
                    J = sequenceJ[j - 1],
                    PrevI = i - 1,
                    PrevJ = j - 1
                };
        }

        private static (string s1, string s2) BuildStrings(SeqRes[,] arr, SeqRes seqRes)
        {
            var chars1 = new List<char> { seqRes.I };
            var chars2 = new List<char> { seqRes.J };

            (chars1, chars2) = BuildStrings(arr, seqRes.PrevI, seqRes.PrevJ, chars1, chars2);

            return (new string(chars1.ToArray()), new string(chars2.ToArray()));
        }

        private static (List<char> chars1, List<char> chars2) BuildStrings(SeqRes[,] arr, int? i, int? j, List<char> chars1, List<char> chars2)
        {
            char? cI = null;
            char? cJ = null;
            if (i == null && j == null)
            {
                return (chars1, chars2);
            }

            if (i == null)
            {
                cI = OutputMismatch;
                i = 0;
            }

            if (j == null)
            {
                cJ = OutputMismatch;
                j = 0;
            }

            var seqRes = arr[i.Value, j.Value];

            cI ??= seqRes.I;
            
            cJ ??= seqRes.J;
            
            chars1.Insert(0, cI.Value);
            chars2.Insert(0, cJ.Value);

            return BuildStrings(arr, seqRes.PrevI, seqRes.PrevJ, chars1, chars2);
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
            public string SequenceString { get; set; }
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
            
            public int GetMismatchCost(char a)
            {
                return table[char.ToUpper(a)][BlossumMismatch];
            }
        }
    }
}
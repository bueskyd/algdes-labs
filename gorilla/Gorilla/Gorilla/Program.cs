using System.Collections.Generic;
using System.Text;

namespace Gorilla
{
    public class Gorilla
    {
        private static int[,] costs;

        private static void ReadCosts()
        {
            using var reader = new StreamReader("..\\..\\..\\..\\..\\data\\BLOSUM62.txt");
            string line;
            while ((line = reader.ReadLine())[0] == '#') ;
            string[] chars = line.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

            int minChar = int.MaxValue;
            int maxChar = 0;
            char[] indexToChar = new char[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                indexToChar[i] = chars[i][0];
                minChar = Math.Min(minChar, chars[i][0]);
                maxChar = Math.Max(maxChar, chars[i][0]);
            }

            costs = new int[minChar + maxChar, minChar + maxChar];

            for (int i = 0; i < chars.Length; i++)
            {
                line = reader.ReadLine();

                string[] row = line.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

                for (int j = 1; j < row.Length; j++)
                    costs[chars[j - 1][0], row[0][0]] = int.Parse(row[j]);
            }
        }

        class Animal
        {
            public string name;
            public string genome;
        }

        private static List<Animal> animals = new List<Animal>();

        private static void ReadInput(TextReader reader)
        {
            string line = reader.ReadLine();
            while (line is not null)
            {
                string[] words = line.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);
                var animal = new Animal();
                animal.name = words[0].Substring(1);
                while ((line = reader.ReadLine()) is not null && line[0] != '>')
                    animal.genome += line;
                animals.Add(animal);
            }
        }

        struct TableEntry
        {
            public int m;
            public int deltaI;
            public int deltaJ;

            public TableEntry(int m, int deltaI, int deltaJ)
            {
                this.m = m;
                this.deltaI = deltaI;
                this.deltaJ = deltaJ;
            }
        }

        private static (string, string) ConstructSolution(
            TableEntry[,] m, string str0, string str1)
        {
            int x = str0.Length - 1;
            int y = str1.Length - 1;
            StringBuilder sb1 = new();
            StringBuilder sb0 = new();
            while (x != 0 || y != 0)
            {
                TableEntry te = m[x, y];
                if (te.deltaI == te.deltaJ)
                {
                    sb0.Append(str0[x]);
                    sb1.Append(str1[y]);
                }
                else if (te.deltaI == 1)
                {
                    sb0.Append(str0[x]);
                    sb1.Append('-');
                }
                else if (te.deltaJ == 1)
                {
                    sb0.Append('-');
                    sb1.Append(str1[y]);
                }
                x -= te.deltaI;
                y -= te.deltaJ;
            }
            sb0.Append(str0[0]);
            sb1.Append(str1[0]);
            string result0 = new(sb0.ToString().Reverse().ToArray());
            string result1 = new(sb1.ToString().Reverse().ToArray());
            return (result0, result1);
        }

        private static void Print(TableEntry[,] m, Func<TableEntry, int> f)
        {
            for (int y = 0; y < m.GetLength(1); y++)
            {
                for (int x = 0; x < m.GetLength(0); x++)
                    Console.Write(f(m[x, y]) + "\t");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static (int, string, string) Solve(string str0, string str1)
        {
            int gapCost = -4;
            TableEntry[,] m = new TableEntry[str0.Length + 1, str1.Length + 1];
            for (int i = 1; i < str0.Length + 1; i++)
                m[i, 0] = new TableEntry(i * gapCost, 1, 0);
            for (int i = 1; i < str1.Length + 1; i++)
                m[0, i] = new TableEntry(i * gapCost, 0, 1);

            for (int i = 1; i < str0.Length + 1; i++)
            {
                for (int j = 1; j < str1.Length + 1; j++)
                {
                    int match =
                        costs[str0[i - 1], str1[j - 1]] +
                        m[i - 1, j - 1].m;
                    int gap0 = gapCost + m[i - 1, j].m;
                    int gap1 = gapCost + m[i, j - 1].m;
                    int deltaI = 0, deltaJ = 0;
                    int cost;
                    if (match >= gap0 && match >= gap1)
                    {
                        cost = match;
                        deltaI = 1;
                        deltaJ = 1;
                    }
                    else if (gap0 >= match && gap0 >= gap1)
                    {
                        cost = gap0;
                        deltaI = 1;
                    }
                    else if (gap1 >= match && gap1 >= gap0)
                    {
                        cost = gap1;
                        deltaJ = 1;
                    }
                    else
                    {
                        throw new Exception("This should never happen");
                    }
                    m[i, j] = new TableEntry(cost, deltaI, deltaJ);
                }
            }
            int minCost = m[str0.Length, str1.Length].m;
            var (s0, s1)= ConstructSolution(m, str0, str1);
            return (minCost, s0, s1);
        }

        private class Result
        {
            public int cost;
            public string name0;
            public string name1;
            public string alignment0;
            public string alignment1;

            public Result(
                int cost,
                string name0, string name1,
                string alignment0, string alignment1)
            {
                this.cost = cost;
                this.name0 = name0;
                this.name1 = name1;
                this.alignment0 = alignment0;
                this.alignment1 = alignment1;
            }
        }

        private static List<Result> Solve()
        {
            var results = new List<Result>();
            for (int i = 0; i < animals.Count; i++)
                for (int j = i + 1; j < animals.Count; j++)
                {
                    var (cost, s0, s1) =
                        Solve(animals[i].genome, animals[j].genome);
                    var result = new Result(
                        cost,
                        animals[i].name,
                        animals[j].name,
                        s0, s1);
                    results.Add(result);
                }
            return results;
        }

        private static Dictionary<(string, string), Result> ReadExpected(
            string file)
        {
            var expected = new Dictionary<(string, string), Result>();
            using var reader = new StreamReader(file);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] words = line.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);
                int expectedAlignment = int.Parse(words[1]);
                string[] names = words[0].Split(
                    "--",
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);
                string name0 = names[0];
                string name1 = names[1].Substring(0, names[1].Length - 1);
                string s0 = reader.ReadLine();
                string s1 = reader.ReadLine();
                var e = new Result(expectedAlignment, name0, name1, s0, s1);
                expected.Add((name0, name1), e);
                expected.Add((name1, name0), e);
            }
            return expected;
        }

        private static void PrintResults(List<Result> results)
        {
            foreach (var result in results)
            {
                Console.WriteLine(
                        $"{result.name0}--{result.name1}: {result.cost}");
                Console.WriteLine(result.alignment0);
                Console.WriteLine(result.alignment1);
            }
        }

        private static void CompareResults(
            Dictionary<(string, string), Result> expected,
            List<Result> actual)
        {
            foreach (var result in actual)
            {
                expected.TryGetValue((result.name0, result.name1), out var e);
                if (result.cost != e.cost)
                {
                    Console.WriteLine(
                        $"Incorrect cost on test case {e.name0}--{e.name1}." +
                        $" Was {result.cost}, expected {e.cost} " +
                        $"(difference {Math.Abs(e.cost - result.cost)}).");
                    Console.WriteLine($"Expected alignments\n{e.alignment0}\n{e.alignment1}");
                    Console.WriteLine($"Actual alignments\n{result.alignment0}\n{result.alignment1}");
                    if (e.alignment0 == result.alignment0)
                        Console.WriteLine($"First alignments are equal");
                    else
                    {
                        string s0 = "";
                        string s1 = "";
                        for (int i = 0; i < e.alignment0.Length; i++)
                        {
                            if (e.alignment0[i] != result.alignment0[i])
                            {
                                s0 += e.alignment0[i];
                                s1 += result.alignment0[i];
                            }
                            else
                            {
                                s0 += '-';
                                s1 += '-';
                            }
                        }
                        Console.WriteLine(s0);
                        Console.WriteLine(s1);
                    }
                    if (e.alignment1 == result.alignment1)
                        Console.WriteLine($"Second alignments are equal");
                    else
                    {
                        string s0 = "";
                        string s1 = "";
                        for (int i = 0; i < e.alignment1.Length; i++)
                        {
                            if (e.alignment1[i] != result.alignment1[i])
                            {
                                s0 += e.alignment1[i];
                                s1 += result.alignment1[i];
                            }
                            else
                            {
                                s0 += '-';
                                s1 += '-';
                            }
                        }
                        Console.WriteLine(s0);
                        Console.WriteLine(s1);
                    }
                    Console.WriteLine();
                }
            }
        }

        public static void Main(string[] args)
        {
            ReadCosts();
            ReadInput(new StreamReader("..\\..\\..\\..\\..\\data\\HbB_FASTAs-in.txt"));
            var actual = Solve();
            var expected = ReadExpected("..\\..\\..\\..\\..\\data\\HbB_FASTAs-out.txt");
            CompareResults(expected, actual);
            PrintResults(actual);
        }
    }
}

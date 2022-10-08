using System.Text;

namespace Gorilla
{
    public class Gorilla
    {
        private static int[,] costs;
        private static int gapCost;

        private static void ReadCosts()
        {
            using var reader = new StreamReader("..\\..\\data\\BLOSUM62.txt");
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
            gapCost = costs['A', '*'];
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

        private static (string, string) ConstructSolution(
            int[,] m, string str0, string str1)
        {
            int x = str0.Length;
            int y = str1.Length;
            StringBuilder sb1 = new();
            StringBuilder sb0 = new();
            while (x != 0 || y != 0)
            {
                int cost = m[x, y];

                if (x == 0)
                {
                    sb0.Append('-');
                    sb1.Append(str1[y--]);
                }
                else if (y == 0)
                {
                    sb0.Append(str0[x--]);
                    sb1.Append('-');
                }
                else if (cost ==
                        m[x, y - 1] + gapCost)
                {
                    sb0.Append('-');
                    sb1.Append(str1[--y]);
                }
                else if (
                    cost ==
                    m[x - 1, y] + gapCost)
                {
                    sb0.Append(str0[--x]);
                    sb1.Append('-');
                }
                else if (
                    cost == m[x - 1, y - 1] +
                    costs[str0[x - 1], str1[y - 1]])
                {
                    sb0.Append(str0[--x]);
                    sb1.Append(str1[--y]);
                }
            }
            string result0 = new(sb0.ToString().Reverse().ToArray());
            string result1 = new(sb1.ToString().Reverse().ToArray());
            return (result0, result1);
        }

        private static (int, string, string) Solve(string str0, string str1)
        {
            int[,] m = new int[str0.Length + 1, str1.Length + 1];
            for (int i = 1; i < str0.Length + 1; i++)
                m[i, 0] = i * gapCost;
            for (int i = 1; i < str1.Length + 1; i++)
                m[0, i] = i * gapCost;

            for (int i = 1; i < str0.Length + 1; i++)
            {
                for (int j = 1; j < str1.Length + 1; j++)
                {
                    int match =
                        costs[str0[i - 1], str1[j - 1]] +
                        m[i - 1, j - 1];
                    int gap0 = gapCost + m[i - 1, j];
                    int gap1 = gapCost + m[i, j - 1];
                    int cost = Math.Max(match, Math.Max(gap0, gap1));
                    m[i, j] = cost;
                }
            }
            int minCost = m[str0.Length, str1.Length];
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

        public static void Main(string[] args)
        {
            ReadCosts();
            ReadInput(new StreamReader("..\\..\\data\\HbB_FASTAs-in.txt"));
            var actual = Solve();
            PrintResults(actual);
        }
    }
}

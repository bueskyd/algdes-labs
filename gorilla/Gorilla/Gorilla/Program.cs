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

        private static string ConstructSolution(int[,] m, string str0, string str1)
        {
            return "";
        }

        private static void Print(int[,] m)
        {
            for (int y = 0; y < m.GetLength(1); y++)
            {
                for (int x = 0; x < m.GetLength(0); x++)
                {
                    Console.Write(m[x, y] + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static (int, string) Solve(string str0, string str1)
        {
            int gapCost = -4;
            int firstCharCost = costs[str0[0], str1[0]];
            int[,] m = new int[str0.Length, str1.Length];
            for (int i = 0; i < str0.Length; i++)
                m[i, 0] = firstCharCost + i * gapCost;
            for (int i = 0; i < str1.Length; i++)
                m[0, i] = firstCharCost + i * gapCost;

            for (int i = 1; i < str0.Length; i++)
            {
                for (int j = 1; j < str1.Length; j++)
                {
                    int match =
                        costs[str0[i], str1[j]] +
                        m[i - 1, j - 1];
                    int gap0 = gapCost + m[i - 1, j];
                    int gap1 = gapCost + m[i, j - 1];
                    int cost = Math.Max(match, Math.Max(gap0, gap1));
                    m[i, j] = cost;
                }
            }
            int minCost = m[str0.Length - 1, str1.Length - 1];
            Print(m);
            string solution = ConstructSolution(m, str0, str1);
            return (minCost, solution);
        }

        private static void Solve()
        {
            for (int i = 0; i < animals.Count; i++)
                for (int j = i + 1; j < animals.Count; j++)
                {
                    var (cost, solution) =
                        Solve(animals[i].genome, animals[j].genome);
                    Console.WriteLine(cost);
                }
        }

        public static void Main(string[] args)
        {
            ReadCosts();
            ReadInput(new StreamReader("..\\..\\..\\..\\..\\data\\TOY_FASTAs-in.txt"));
            Solve();
        }
    }
}

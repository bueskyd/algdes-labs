using System.Text.RegularExpressions;

public class Gorilla
{
    public static void Main(string[] args)
    {
        var matrix = GetValueMatrix();
        var data = ReadData("../data/HbB_FASTAs-in.txt");

    }

    public static List<(string, string)> ReadData(string path)
    {
        var lines = File.ReadAllLines(path);
        List<(string, string)> entries = new();

        int i = 0;

        while (i < lines.Count() && lines[i] != "")
        {
            var species = lines[i].Split(" ")[0].Substring(1);
            List<string> parts = new();
            i++;
            while (i < lines.Count() && lines[i][0] != '>')
            {
                parts.Add(lines[i]);
                i++;
            }
            entries.Add((species, string.Join("", parts)));
        }

        return entries;
    }

    public static void ReadEntry(int i, string[] lines)
    {
        List<string> parts = new();
        var species = Regex.Split(lines[i], @"\s+").First().Substring(1);
        i++;
        while (lines[i][0] != '>')
        {

        }
    }

    public static Dictionary<string, int> GetValueMatrix()
    {
        var dict = new Dictionary<string, int>();
        var lines = File.ReadAllLines("../data/BLOSUM62.txt").Skip(6).ToArray();

        var characters = Regex.Split(lines[0], @"\s+").Skip(1).ToArray();

        foreach (var c in characters) Console.Write(c);

        for (int i = 1; i < lines.Count(); i++)
        {
            var values = Regex.Split(lines[i], @"\s+");
            var currLetter = values[0];
            Console.WriteLine("curr " + currLetter);
            var numbers = values.Skip(1).TakeWhile(x => x != "").ToArray();

            for (int j = 0; j < numbers.Count(); j++)
            {
                if (characters[j] == "*")
                {
                    Console.Write("");
                }
                dict.Add($"{currLetter} {characters[j]}", int.Parse(numbers[j]));
                Console.WriteLine($"({currLetter} {characters[j]} {numbers[j]}");
            }
        }
        return dict;
    }
}



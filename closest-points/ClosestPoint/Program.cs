using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        // Console.WriteLine(Double.Parse("2.88", NumberStyles.Float, CultureInfo.InvariantCulture));
        foreach (var coord in ReadData())
        {
            Console.WriteLine(coord);
        }
    }

    public static List<Coordinate> ReadData()
    {
        var currLine = Console.ReadLine();

        List<Coordinate> coords = new();

        var pattern = new Regex(@"\s*(?<id>\S+)\s*(?<x>\S+)\s*(?<y>\S+)");

        while (currLine != null && !currLine.Contains("EOF") && currLine.Trim() != "")
        {
            try
            {
                var match = pattern.Match(currLine);
                var id = match.Groups[1].Value;
                var x = match.Groups[2].Value;
                var y = match.Groups[3].Value;

                var xP = double.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture);
                var yP = double.Parse(y, NumberStyles.Float, CultureInfo.InvariantCulture);
                coords.Add(new Coordinate(id, xP, yP));
            }
            catch { }

            currLine = Console.ReadLine();
        }

        return coords;
    }
}

public class Coordinate
{
    public string id;
    public double x;
    public double y;
    public Coordinate(string id, double x, double y)
    {
        this.id = id;
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"{id}: {x}, {y}";
    }
}

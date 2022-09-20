using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        var coords = ReadData();
        var sortedX = coords.OrderBy(a => a.x).ToList();
        var sortedY = coords.OrderBy(a => a.y).ToList();
        var result = ClosestPair(sortedX, sortedY);
        Console.WriteLine(result.distance);
    }

    public static Pair ClosestPair(List<Coordinate> sortedX, List<Coordinate> sortedY)
    {
        // Stop the recursion by returning the remaining pair
        if (sortedX.Count() <= 3)
        {
            if (sortedX.Count() == 2) return new Pair(sortedX[0], sortedX[1]);
            var pairs = new List<Pair>() {
                new Pair(sortedX[0], sortedX[1]),
                new Pair(sortedX[1], sortedX[2]),
                new Pair(sortedX[2], sortedX[0])
            }.OrderBy(a => a.distance);
            return pairs.First();
        }

        // Split the points into right and left part. Alternates between splitting on x and y. Maybe no alternating needed?
        int splitIndex = sortedX.Count() / 2;
        var leftPart = sortedX.Take(splitIndex).ToList();
        var rightPart = sortedX.Skip(splitIndex).ToList();

        var maxX = leftPart.Last().x;

        // Points in left and right parts sorted by y, with linear time
        var (leftPartY, rightPartY) = SortY(sortedY, maxX);

        // Find best solutions in each half
        var bestLeft = ClosestPair(leftPart, leftPartY);
        var bestRight = ClosestPair(rightPart, rightPartY);


        var shortestDist = bestLeft.distance < bestRight.distance ? bestLeft : bestRight;

        // All points within maxX of the splitting line on the y axis
        var Sy = sortedY.Where(p => Math.Abs(p.x - maxX) <= shortestDist.distance).ToList();

        // Get best pair from these, checking next 15 options for each
        Pair bestMid = Pair.WorstPair;
        for (int i = 0; i < Sy.Count(); i++)
        {
            var curr = Sy[i];

            for (int j = 1; j <= 15; j++)
            {
                if (i + j >= Sy.Count()) break;
                var next = Sy[i + j];
                if ((bestMid.distance > curr.DistanceTo(next))) bestMid = new Pair(curr, next);
            }
        }

        return new List<Pair>() { bestLeft, bestMid, bestRight }.OrderBy(p => p.distance).First();
    }

    public static (List<Coordinate>, List<Coordinate>) SortY(List<Coordinate> sortedY, double splittingLine)
    {
        List<Coordinate> left = new();
        List<Coordinate> right = new();
        foreach (var coord in sortedY)
        {
            if (coord.x <= splittingLine) left.Add(coord);
            else right.Add(coord);
        }

        return (left, right);
    }

    public static List<Coordinate> ReadData()
    {
        var currLine = Console.ReadLine();

        List<Coordinate> coords = new();

        var number = @"[-+]?\d*\.?\d+(?:[eE][-+]?\d+)?";
        var pattern = new Regex(@$"(\w+)\s+({number})\s+({number})");
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

    public double DistanceTo(Coordinate that)
    {
        return Math.Sqrt(Math.Pow(this.x - that.x, 2) + Math.Pow(this.y - that.y, 2));
    }
}

public class Pair
{
    public Coordinate fst;
    public Coordinate snd;
    public double distance;
    public Pair(Coordinate fst, Coordinate snd)
    {
        this.fst = fst;
        this.snd = snd;
        distance = fst.DistanceTo(snd);
    }

    public override string ToString()
    {
        return "(" + fst.id + ", " + snd.id + "): " + this.distance;
    }

    public static Pair WorstPair = new Pair(new Coordinate("", double.PositiveInfinity, double.PositiveInfinity), new Coordinate("", double.NegativeInfinity, double.NegativeInfinity));
}

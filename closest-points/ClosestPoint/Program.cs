using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        var coords = ReadData();
        var sortedX = coords.OrderBy(a => a.x).Select(x => x).ToList();
        var sortedY = coords.OrderBy(a => a.y).Select(x => x).ToList();

        var result = ClosestPair(sortedX, sortedY);
        Console.WriteLine(result);
    }

    public static Pair ClosestPair(List<Coordinate> sortedX, List<Coordinate> sortedY)
    {
        // Stop the recursion by returning the remaining pair
        if (sortedX.Count() <= 3)
        {
            if (sortedX.Count() == 2) return new Pair(sortedX[0], sortedY[1]);
            var pairs = new List<Pair>() {
                new Pair(sortedX[0], sortedX[1]),
                new Pair(sortedX[1], sortedX[2]),
                new Pair(sortedX[2], sortedX[0])
            }.OrderBy(a => a.distance);
            return pairs.First();
        }

        // Split the points into right and left part. Alternates between splitting on x and y. Maybe no alternating needed?
        int splitIndex = sortedX.Count() / 2;
        var leftPart = sortedX.Take(splitIndex);
        var rightPart = sortedX.Skip(splitIndex);

        // Points in left part sorted by x and y
        var leftPartX = leftPart.OrderBy(a => a.x).Select(x => x).ToList();
        var leftPartY = leftPart.OrderBy(a => a.y).Select(x => x).ToList();

        var rightPartX = rightPart.OrderBy(a => a.x).Select(x => x).ToList();
        var rightPartY = rightPart.OrderBy(a => a.y).Select(x => x).ToList();

        // Find best solutions in each half
        var bestLeft = ClosestPair(leftPartX, leftPartY);
        var bestRight = ClosestPair(rightPartX, rightPartY);


        var shortestDist = bestLeft.distance < bestRight.distance ? bestLeft : bestRight;
        var maxX = leftPartX.Last().x; // TODO

        // All points within maxX of the splitting line on the y axis
        var Sy = sortedY.Where(p => Math.Abs(p.x) <= maxX).ToList();

        // Get best pair from these, checking next 15 options for each
        Pair bestPairSy = Pair.WorstPair;
        for (int i = 0; i < Sy.Count(); i++)
        {
            var curr = Sy[i];
            int next15 = Math.Min(i + 15, Sy.Count()) - 1;
            Console.WriteLine("Count: " + Sy.Count());
            for (int j = 1; j < next15; j++)
            {
                Console.WriteLine(i + j);
                if (i + j < Sy.Count() && !curr.Equals(Sy[i + j]) && (bestPairSy is null || bestPairSy.distance > curr.DistanceTo(Sy[i + j]))) bestPairSy = new Pair(curr, Sy[i + j]);
            }
        }

        Console.WriteLine("Left: " + bestLeft);
        Console.WriteLine("Mid: " + bestPairSy);
        Console.WriteLine("Right: " + bestRight);

        // Return pair with shortest distance
        if (bestPairSy.distance < bestLeft.distance && bestPairSy.distance < bestRight.distance) return bestPairSy;
        if (bestLeft.distance < bestPairSy.distance && bestLeft.distance < bestRight.distance) return bestLeft;
        return bestRight;
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

    public bool Equals(Coordinate that)
    {
        return this.x == that.x && this.y == that.y;
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
        return "(" + fst.id + ", " + snd.id + ")";
    }

    public static Pair WorstPair = new Pair(new Coordinate("", double.PositiveInfinity, double.PositiveInfinity), new Coordinate("", double.NegativeInfinity, double.NegativeInfinity));
}

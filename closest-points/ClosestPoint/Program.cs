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



        ClosestPair(sortedX, sortedY, 0);
    }

    public static Pair ClosestPair(List<Coordinate> sortedX, List<Coordinate> sortedY, int layer)
    {
        // Stop the recursion by returning the remaining pair
        if (sortedX.Count() <= 3)
        {
            if (sortedX.Count() == 2) return new Pair(sortedX[0], sortedY[1]);
            var pairs = new List<Pair>() {
                new Pair(sortedX[0], sortedX[1]),
                new Pair(sortedX[1], sortedX[2]),
                new Pair(sortedX[2], sortedX[0])
            }.OrderBy(a => a.Distance());
            return pairs.First();
        }

        // Split the points into right and left part. Alternates between splitting on x and y. Maybe no alternating needed?
        int splitIndex = sortedX.Count() / 2;
        // var leftPart = layer % 2 == 0 ? sortedX.Take(splitIndex) : sortedY.Take(splitIndex);
        // var rightPart = layer % 2 == 0 ? sortedX.Skip(splitIndex) : sortedY.Skip(splitIndex);
        var leftPart = sortedX.Take(splitIndex);
        var rightPart = sortedX.Skip(splitIndex);

        // Points in left part sorted by x and y
        var leftPartX = leftPart.OrderBy(a => a.x).Select(x => x).ToList();
        var leftPartY = leftPart.OrderBy(a => a.y).Select(x => x).ToList();

        // Maps from point-id to their index in the left lists
        var leftPartXIndexMap = GetIndexMap(leftPartX);
        var leftPartYIndexMap = GetIndexMap(leftPartY);

        var rightPartX = rightPart.OrderBy(a => a.x).Select(x => x).ToList();
        var rightPartY = rightPart.OrderBy(a => a.y).Select(x => x).ToList();

        var rightPartXIndexMap = GetIndexMap(rightPartX);
        var rightPartYIndexMap = GetIndexMap(rightPartY);

        // Find best solutions in each half
        var bestLeft = ClosestPair(leftPartX, leftPartY, layer + 1);
        var bestRight = ClosestPair(rightPartX, rightPartY, layer + 1);


        var shortestDist = bestLeft.Distance() < bestRight.Distance() ? bestLeft : bestRight;
        var maxX = leftPartX.Last().x; // TODO

        // All points within maxX of the splitting line on the y axis
        var Sy = sortedY.Where(p => Math.Abs(p.x - maxX) <= shortestDist.Distance()).Select(p => p).ToList();

        // Get best pair from these, checking next 15 options for each
        Pair bestPairSy = Pair.WorstPair;
        for (int i = 0; i < Sy.Count(); i++)
        {
            var curr = Sy[i];
            int next15 = Math.Min(i + 15, Sy.Count());
            for (int j = 0; j < next15; j++)
            {
                if (bestPairSy is null || bestPairSy.Distance() > curr.DistanceTo(Sy[i + j])) bestPairSy = new Pair(curr, Sy[i + j]);
            }
        }

        // Return pair with shortest distance
        if (bestPairSy.Distance() < bestLeft.Distance() && bestPairSy.Distance() < bestRight.Distance()) return bestPairSy;
        else if (bestLeft.Distance() < bestPairSy.Distance() && bestLeft.Distance() < bestRight.Distance()) return bestLeft;
        else return bestRight;

        // return bestLeft.Distance() < bestRight.Distance() ? bestLeft : bestRight;
    }

    public static Dictionary<string, int> GetIndexMap(List<Coordinate> coords)
    {
        Dictionary<string, int> idToIndex = new();
        for (int i = 0; i < coords.Count(); i++)
        {
            idToIndex.Add(coords[i].id, i);
        }
        return idToIndex;
    }

    public static List<Coordinate> ReadData()
    {
        var currLine = Console.ReadLine();

        List<Coordinate> coords = new();

        var number = @"[-+]?\d*\.?\d+(?:[eE][-+]?\d+)?";
        var pattern = new Regex(@$"(\d+)\s+({number})\s+({number})");

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
    public Pair(Coordinate fst, Coordinate snd)
    {
        this.fst = fst;
        this.snd = snd;
    }

    public static Pair WorstPair = new Pair(new Coordinate("", double.PositiveInfinity, double.PositiveInfinity), new Coordinate("", double.NegativeInfinity, double.NegativeInfinity));

    public double Distance()
    {
        return fst.DistanceTo(snd);
    }
}

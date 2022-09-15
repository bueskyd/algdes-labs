using System.Text.RegularExpressions;

namespace Solution;

public delegate double parser(string input);

public class Point
{
    public double x { get; init; }
    public double y { get; init; }
    public string id { get; init; }

    public static Point New(string id, double x, double y) {
        return new Point() { id = id, x = x, y = y };
    }
}

// TSP files start with "NAME"

public class Program
{
    public static List<Point> points;
    public static void Main(string[] args) 
    {
        var index = 0;
        string[] lines;
        if (args.Length == 0) { Run(() => Console.ReadLine()); return; }
        switch (args[0])
        {
            case "f":
                index = 0;
                lines = File.ReadAllLines(args[1]);
                Run(() => { if (index >= lines.Length) return null; else return lines[index++]; });
                break;
            case "d":
                foreach(var file in Directory.GetFiles(args[1])) { 
                    if (!file.EndsWith("tsp.txt")) continue;
                    index = 0;
                    lines = File.ReadAllLines(file);
                    Run(() => { if (index >= lines.Length) return null; else return lines[index++]; });
                }
                break;
            default:
                System.Console.WriteLine("Unknown command!");
                break;
        }
    }

    public static void Run(Func<string> reader) 
    {
        parser parse = (s) => double.Parse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

        while(true) {
            if (reader() == "NODE_COORD_SECTION") break;
        }
        
        points = new List<Point>();
        var id = 0;
        while(true) {
            var line = reader().Trim();
            if (line.Contains("EOF") || line is null || line == "") break;
            var data = Regex.Split(line, " +");
            points.Add(Point.New(data[0], parse(data[1]), parse(data[2])));
            id++;
        }
        points.Sort((a,b) => a.x == b.x ? 0 : a.x - b.x <= 0 ? -1 : 1 ); 

        var closest = Closest(0, points.Count-1);
        System.Console.WriteLine($"{closest.a.id} - {closest.b.id} : {Distance(closest.a, closest.b)}");
    }

    public static (Point a, Point b) Closest(int left, int right) 
    {
        // System.Console.WriteLine($"{left} - {right}");

        // 2 points left
        if (right - left == 1) return (points[left], points[right]);
        // 3 points left
        if (right - left == 2) {    
            var guess = Distance(points[left], points[right]);
            if (Distance(points[left], points[left+1]) < guess) return (points[left], points[left+1]);
            if (Distance(points[left+1], points[right]) < guess) return (points[left+1], points[right]);
            return (points[left], points[right]);
        }

        var L = left + ((right - left)/2);

        var leftBest = Closest(left, L);
        var rightBest = Closest(L+1, right);
        var currentBest = (Distance(leftBest.a, leftBest.b) < Distance(rightBest.a, rightBest.b)) ? leftBest : rightBest;
        var currentBestDistance = Distance(currentBest.a, currentBest.b);

        var searchZoneLower = L-currentBestDistance;
        var searchZoneUpper = L+currentBestDistance;

        var searchZoneLower_idx = L;
        while (points[searchZoneLower_idx].x >= searchZoneLower) {if (searchZoneLower_idx == 0) break; searchZoneLower_idx--;}

        var searchZoneUpper_idx = L;
        while (points[searchZoneUpper_idx].x <= searchZoneUpper) {if (searchZoneUpper_idx == points.Count-1) break; searchZoneUpper_idx++;}

        var searchZoneLeft = points.GetRange(searchZoneLower_idx, L - searchZoneLower_idx);
        searchZoneLeft.Sort((a,b) => a.y == b.y ? 0 : a.y - b.y <= 0 ? -1 : 1 ); 

        var searchZoneRight = points.GetRange(L, searchZoneUpper_idx - L);
        searchZoneRight.Sort((a,b) => a.y == b.y ? 0 : a.y - b.y <= 0 ? -1 : 1 ); 

        var CrossBestLeft = 0;
        var CrossBestRight = 0;
        var CrossBestDistance = double.PositiveInfinity;

        for(int lzi = 0; lzi < searchZoneLeft.Count; lzi++) {
            for(int rzi = 0; rzi < searchZoneRight.Count; rzi++) {
                var distance = Distance(searchZoneLeft[lzi], searchZoneRight[rzi]);
                if (distance < CrossBestDistance) {
                    CrossBestLeft = lzi;
                    CrossBestRight = rzi;
                    CrossBestDistance = distance;
                }
            }
        }

        if (CrossBestDistance == double.PositiveInfinity) return currentBest;
        else if (CrossBestDistance < currentBestDistance) return (searchZoneLeft[CrossBestLeft], searchZoneRight[CrossBestRight]);
        else return currentBest;
    } 

    public static double Distance(Point a, Point b) 
    {
        return Math.Sqrt(Math.Pow(Math.Abs(a.x - b.x), 2) + Math.Pow(Math.Abs(a.y - b.y), 2));
    }
}
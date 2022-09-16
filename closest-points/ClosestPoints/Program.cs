namespace ClosestPoints
{
    public static class ClosestPoints
    {
        public static void Main(string[] args)
        {
            SkipThoreHeaders();

            var points = new List<Point>();
            
            string? pointInfo;
            while ((pointInfo = Console.ReadLine()) != null)
            {
                pointInfo = pointInfo.Trim();
                if(pointInfo == "EOF" || pointInfo.Length == 0) break;
                points.Add(new Point(pointInfo.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries)));
            }

            var (p1, p2) = MinLength(points);
            Console.WriteLine($"{points.Count} {p1.Distance(p2)}");
        }

        private static void SkipThoreHeaders()
        {
            var firstChar = (char) Console.In.Peek();
            if (firstChar == 'N')
            {
                while (Console.ReadLine().Trim() != "NODE_COORD_SECTION")
                {
                }
            }
        }

        private static (Point p1, Point p2) MinLength(List<Point> p)
        {
            var pX = p.OrderBy(p => p.X).ToList();
            var pY = pX.OrderBy(p => p.Y).ToList();
            
            return MinLengthRec(pX, pY);
        }
        
        /// <param name="pX">Points <b>SORTED</b> by x coordinate</param>
        /// <param name="pY">Points <b>SORTED</b> by y coordinate</param>
        private static (Point p1, Point p2) MinLengthRec(List<Point> pX, List<Point> pY)
        {
            var n = pX.Count;
            if (n < 2) throw new Exception("Need two or more points!");

            if (n < 4)
            {
                var (p1, p2, _) = MeasurePairwise(pX);
                return (p1, p2);
            }

            var mid = n / 2;
            var midPoint = pX[mid-1];
            
            var qX = pX.GetRange(0, mid);
            var rX = pX.GetRange(mid, pX.Count - mid);
            var qY = new List<Point>();
            var rY = new List<Point>();
            
            foreach (var pointY in pY)
            {
                if(pointY.X > midPoint.X) rY.Add(pointY);
                else qY.Add(pointY);
            }

            var (q0, q1) = MinLengthRec(qX, qY);
            var (r0, r1) = MinLengthRec(rX, rY);

            var shortestDistance = Math.Min(q0.Distance(q1), r0.Distance(r1));

            pY = pY
                .Where(p => p.DistanceToLineThroughX(midPoint.X) < shortestDistance)
                .ToList();
            
            var (s0,s1, sShortestDistance) = MeasurePairwise(pY);

            if (shortestDistance > sShortestDistance)
            {
                return (s0, s1);
            }
            else if (r0.Distance(r1) > q0.Distance(q1))
            {
                return (q0, q1);
            }
            else
            {
                return (r0, r1);
            }
        }

        private static (Point p1, Point p2, decimal shortestDistance) MeasurePairwise(List<Point> points)
        {
            (Point p1, Point p2, var shortestDistance) = (null!, null!, decimal.MaxValue);
            for (var i = 0; i < points.Count - 1; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    var sc1 = points[i];
                    var sc2 = points[j];
                    var scDistance = sc1.Distance(sc2);
                    if (shortestDistance > scDistance)
                    {
                        p1 = sc1;
                        p2 = sc2;
                        shortestDistance = scDistance;
                    }
                }
            }

            return (p1, p2, shortestDistance);
        } 

        public class Point
        {
            public Point(string[] pointInfo)
            {
                Name = pointInfo[0];
                X = (decimal) double.Parse(pointInfo[1]);
                Y = (decimal) double.Parse(pointInfo[2]);
            }

            private string Name;
            public decimal X { get; }
            public decimal Y { get; }

            public decimal Distance(Point other)
            {
                return (decimal) Math.Sqrt(Math.Pow((double) (other.X - X), 2) + Math.Pow((double) (other.Y - Y), 2));
            }
            
            public decimal DistanceToLineThroughX(decimal x)
            {
                return (decimal) Math.Sqrt(Math.Pow((double) (x - X), 2));
            }

            public override string ToString()
            {
                return $"{nameof(Name)}: {Name}, {nameof(X)}: {X}, {nameof(Y)}: {Y}";
            }
        }
    }
    
}
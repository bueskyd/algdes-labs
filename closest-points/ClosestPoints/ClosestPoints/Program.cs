﻿namespace ClosestPoints
{
    public class ClosestPoints
    {
        struct Point
        {
            public double x;
            public double y;
            public int id;

            public Point(double x, double y, int id)
            {
                this.x = x;
                this.y = y;
                this.id = id;
            }

            public override string ToString()
            {
                return $"{id}: ({x}, {y})";
            }
        }

        private static double Parse(string s)
        {
            return (double)decimal.Parse(
                s,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture);
        }

        private static List<Point> ReadInput()
        {
            while ((Console.ReadLine().Trim()) != "NODE_COORD_SECTION") ;
            List<Point> points = new List<Point>();
            string? line = Console.ReadLine()?.Trim();
            while (line is not null && line != "" && line != "EOF")
            {
                line = line.Trim();
                string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int pointId = int.Parse(words[0]);
                double x = Parse(words[1]);
                double y = Parse(words[2]);
                points.Add(new Point(x, y, pointId - 1));
                line = Console.ReadLine()?.Trim();
            }
            return points;
        }

        private static double Distance(Point a, Point b)
        {
            double xx = (a.x - b.x);
            xx *= xx;
            double yy = (a.y - b.y);
            yy *= yy;
            return Math.Sqrt(xx + yy);
        }

        private static List<Point> Strip(List<Point> qy, List<Point> ry, double l, double minDistance)
        {
            List<Point> strip = new();
            int i = 0, j = 0;
            while (true)
            {
                while (i < qy.Count && l - qy[i].x > minDistance) ++i;
                if (i == qy.Count) break;
                while (j < ry.Count && ry[j].x - l > minDistance) ++j;
                if (j == ry.Count) break;
                if (qy[i].y < ry[j].y) strip.Add(qy[i++]);
                else strip.Add(ry[j++]);
            }

            for (; i < qy.Count; ++i) if (l - qy[i].x <= minDistance) strip.Add(qy[i]);
            for (; j < ry.Count; ++j) if (ry[j].x - l <= minDistance) strip.Add(ry[j]);
            return strip;
        }

        private static (double, int, int) ClosestPair(
            List<bool> inLeft,
            List<Point> xSorted,
            List<Point> ySorted)
        {
            if (xSorted.Count == 2)
            {
                double d = Distance(xSorted[0], xSorted[1]);
                return (d, xSorted[0].id, xSorted[1].id);
            }
            if (xSorted.Count == 3)
            {
                double d01 = Distance(xSorted[0], xSorted[1]);
                double d02 = Distance(xSorted[0], xSorted[2]);
                double d12 = Distance(xSorted[1], xSorted[2]);
                if (d01 <= d02 && d01 <= d12)
                    return (d01, xSorted[0].id, xSorted[1].id);
                if (d02 <= d01 && d02 <= d12)
                    return (d02, xSorted[0].id, xSorted[2].id);
                if (d12 <= d01 && d12 <= d02)
                    return (d12, xSorted[1].id, xSorted[2].id);
                throw new InvalidOperationException("This should never be reached");
            }

            List<Point> qx = new();
            List<Point> qy = new();
            List<Point> rx = new();
            List<Point> ry = new();

            for (int i = 0; i < xSorted.Count / 2; i++)
            {
                qx.Add(xSorted[i]);
                inLeft[xSorted[i].id] = true;
            }
            for (int i = xSorted.Count / 2; i < xSorted.Count; i++)
            {
                rx.Add(xSorted[i]);
                inLeft[xSorted[i].id] = false;
            }

            foreach (Point point in ySorted)
                if (inLeft[point.id])
                    qy.Add(point);
                else
                    ry.Add(point);

            var (d0, id0, id1) = ClosestPair(inLeft, qx, qy);
            var (d1, id2, id3) = ClosestPair(inLeft, rx, ry);

            double minDistance = d0;
            if (d1 < minDistance)
            {
                minDistance = d1;
                id0 = id2;
                id1 = id3;
            }

            double l = qx[^1].x;
            List<Point> strip = Strip(qy, ry, l, minDistance);

            for (int i = 0; i < strip.Count; i++)
                for (int j = 1; j < 15; j++)
                {
                    if (i + j >= strip.Count)
                        break;
                    double distance = Distance(strip[i], strip[i + j]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        id0 = strip[i].id;
                        id1 = strip[i + j].id;
                    }
                }
            return (minDistance, id0, id1);
        }

        public static void Main(string[] args)
        {
            var points = ReadInput();
            List<Point> xSorted = new();
            List<Point> ySorted = new();
            List<bool> inLeft = new();
            foreach (Point point in points)
            {
                xSorted.Add(point);
                ySorted.Add(point);
                inLeft.Add(false);
            }

            xSorted.Sort((a, b) => a.x == b.x ? 0 : a.x < b.x ? -1 : 1);
            ySorted.Sort((a, b) => a.y == b.y ? 0 : a.y < b.y ? -1 : 1);

            var (distance, id0, id1) = ClosestPair(inLeft, xSorted, ySorted);
            Console.WriteLine(distance);
        }
    }
}
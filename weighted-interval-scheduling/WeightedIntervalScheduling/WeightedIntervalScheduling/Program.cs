using System;

namespace WeightedIntervalScheduling
{
    public static class SchedulingAlgorithms
    {

        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());

            var activities = new Activity[n];

            for (var i = 0; i < n; i++)
            {
                var line = Console.ReadLine().Split(" ");

                var activity = new Activity()
                {
                    StartTime = int.Parse(line[0]),
                    EndTime = int.Parse(line[1]),
                    Weight = int.Parse(line[2])
                };
                activities[i] = activity;
            }

            var maxWeight = ComputeOpt(activities);
            Console.WriteLine(maxWeight);
        }

        private static long ComputeOpt(Activity[] activities)
        {
            var n = activities.Length;
            Array.Sort(activities, (a, b) => a.EndTime.CompareTo(b.EndTime));
            
            var memoizedResults = new int[n];
            var finishTimes = new int[n];
            
            Array.Fill(memoizedResults, -1);
            for (var i = 0; i < n; i++)
            {
                finishTimes[i] = activities[i].EndTime;
            }

            return ComputeOpt(activities, finishTimes, memoizedResults, n - 1);
        }

        private static int ComputeOpt(Activity[] activities, int[] finishTimes, int[] memoizedResults, int j)
        {
            if (j < 0) return 0;
            if (memoizedResults[j] != -1)
            {
                return memoizedResults[j];
            }
            
            var activity = activities[j];
            var inclusive = activity.Weight + ComputeOpt(activities, finishTimes, memoizedResults, BinarySearchUpperBound(finishTimes, activity.StartTime));
            var exclusive = ComputeOpt(activities, finishTimes, memoizedResults, j - 1);
            
            var result = Math.Max(inclusive, exclusive);
            memoizedResults[j] = result;
            return result;
        }

        /// <summary>
        /// Finds index of the largest element less than k in list,
        /// returns -1 if it does not exist.
        /// The input list must be sorted in ascending order
        /// </summary>
        private static int BinarySearchUpperBound(int[] list, int k)
        {
            var n = list.Length;
            if (list[0] > k) {
                return -1;
            }
            var l = 0;
            var r = n - 1;
            
            while (l <= r) {
                var m = l + (r - l) / 2;

                // If mid element is greater than
                // k update right index
                if (list[m] > k) r = m - 1;

                // If mid element is less than
                // or equal to k update left index
                else l = m + 1;
            }

            return r;
        }

        private class Activity
        {
            public int StartTime { get; set; }
            public int EndTime { get; set; }
            public int Weight { get; set; }
        }
    }
}
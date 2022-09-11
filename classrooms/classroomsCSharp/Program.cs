using System;
using System.Collections.Generic;

namespace classrooms
{
    public class SchedulingAlgorithms
    {

        public static void Main(string[] args)
        {
            var initialLines = Console.ReadLine().Split(" ");
            var amountOfActivities  = int.Parse(initialLines[0]);
            var amountOfClassrooms = int.Parse(initialLines[1]);

            var activities = new Activity[amountOfActivities];

            for (var i = 0; i < amountOfActivities; i++)
            {
                var line = Console.ReadLine().Split(" ");

                var activity = new Activity()
                {
                    StartTime = int.Parse(line[0]),
                    EndTime = int.Parse(line[1])
                };
                activities[i] = activity;
            }

            var allocatedClassrooms = EarliestStartTimeFirst(amountOfClassrooms, activities);
            Console.WriteLine(allocatedClassrooms);
        }

        private static long EarliestStartTimeFirst(long amountOfClassrooms, Activity[] activities)
        {
            Array.Sort(activities, (a, b) => a.EndTime.CompareTo(b.EndTime));
            
            var list = new List<int>();
            for(var i = 0; i < amountOfClassrooms; i++) list.Add(0);
            
            var activityCount = 0;
            
            foreach (var activity in activities)
            {
                var index = BinarySearchUpperBound(list, -activity.StartTime);
                
                if(index == -1) {
                    continue;
                }

                list.RemoveAt(index);
                list.Add(-activity.EndTime);
                activityCount++;
            }

            return activityCount;
        }
        
        /// <summary>
        /// Finds number smallest element greater than in list, -1 if it does not exist 
        /// </summary>
        private static int BinarySearchUpperBound(List<int> list, int k)
        {
            var n = list.Count;
            if (list[0] <= k) {
                return -1;
            }
            var l = 0;
            var r = n - 1;
            
            while (l <= r) {
                var m = l + (r - l) / 2;

                // If mid element is greater than
                // k update left index
                if (list[m] > k) l = m + 1;

                // If mid element is less than
                // k update right index
                else r = m - 1;
            }

            return r;
        }

        private class Activity
        {
            public int StartTime { get; set; }
            public int EndTime { get; set; }
        }
    }
}
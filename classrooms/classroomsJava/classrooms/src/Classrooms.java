import java.util.*;

public class Classrooms {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        String[] initialLines = scanner.nextLine().split(" ");
        int amountOfActivities  = Integer.parseInt(initialLines[0]);
        int amountOfClassrooms = Integer.parseInt(initialLines[1]);

        var activities = new Activity[amountOfActivities];

        for (var i = 0; i < amountOfActivities; i++) {
            String[] line = scanner.nextLine().split(" ");
            activities[i] = new Activity(Integer.parseInt(line[0]), Integer.parseInt(line[1]));
        }

        int allocatedClassrooms = earliestEndTimeFirst(amountOfClassrooms, activities);
        System.out.println(allocatedClassrooms);
    }

    private static int earliestEndTimeFirst(int amountOfClassrooms, Activity[] activities) {
        Arrays.sort(activities, Comparator.comparing(o -> o.endTime));
        List<Integer> list = new ArrayList<>();
        for(int i = 0; i < amountOfClassrooms; i++) list.add(0);

        int activityCount = 0;

        for (Activity activity : activities) {

            int index = binarySearchUpperBound(list, -activity.startTime);

            if(index == -1) {
                continue;
            }

            list.remove(index);
            list.add(-activity.endTime);
            activityCount++;
        }

        return activityCount;
    }

    /**
     * Finds number smallest element greater than in list, -1 if it does not exist
     */
    public static int binarySearchUpperBound(List<Integer> list, Integer k)
    {
        int n = list.size();
        if (list.get(0) <= k) {
            return -1;
        }
        int l = 0;
        int r = n - 1;

        while (l <= r) {
            int m = l + (r - l) / 2;

            // If mid element is greater than
            // k update left index
            if (list.get(m) > k) l = m + 1;

                // If mid element is less than
                // k update right index
            else r = m - 1;
        }

        return r;
    }

    static class Activity
    {
        public int startTime;
        public int endTime;

        public Activity(int startTime, int endTime) {
            this.startTime = startTime;
            this.endTime = endTime;
        }
    }
}

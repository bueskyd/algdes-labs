import java.util.Arrays;
import java.util.Comparator;
import java.util.List;
import java.util.Scanner;
import java.util.function.Function;
import java.util.stream.Collectors;

public class IntervalScheduling {

    public static void main(String[] args) {
        int amountOfIntervals = Parser.parseAs(Integer::parseInt);

        Interval[] intervals = new Interval[amountOfIntervals];
        for(int i = 0; i < amountOfIntervals; i++) {
            Long[] intervalEntry = Parser.parseAllAs(Long::parseLong).toArray(Long[]::new);
            intervals[i] = new Interval(intervalEntry[0], intervalEntry[1]);
        }

        int nonOverlappingIntervals = latestFinishTimeFirst(intervals);
        System.out.println(nonOverlappingIntervals);
    }

    private static int latestFinishTimeFirst(Interval[] intervals) {
        Arrays.sort(intervals, Comparator.comparing(a -> a.finish));

        int nonOverlappingIntervals = 0;
        long previousFinishTime = 0;

        for (Interval interval : intervals) {
            if (interval.start >= previousFinishTime) {
                nonOverlappingIntervals++;
                previousFinishTime = interval.finish;
            }
        }
        return nonOverlappingIntervals;
    }


    static class Parser {
        private static final Scanner scanner = new Scanner(System.in);

        public static <T> List<T> parseAllAs(Function<String, T> converter) {

            if(!scanner.hasNext()) {
                throw new RuntimeException("No more input");
            }

            String line = scanner.nextLine();
            return Arrays.stream(line.split(" ")).map(converter).collect(Collectors.toList());
        }

        public static <T> T parseAs(Function<String, T> converter) {
            return parseAllAs(converter).get(0);
        }

    }

    static class Interval {
        private Long start;
        private Long finish;

        public Interval(Long start, Long finish) {
            this.start = start;
            this.finish = finish;
        }
    }
}

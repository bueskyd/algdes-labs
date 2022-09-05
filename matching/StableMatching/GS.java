import java.util.Scanner;
import java.util.Stack;

/**
 * Gale-Shapley stable matching algorithm
 */
public class GS {
  public String[] man;
  public String[] woman;
  public int[][] manPref;
  public int[][] womanPref;
  public Stack<Integer> free = new Stack<>();
  public int[] next;
  public Integer[] current;
  public int[][] ranking;
  public boolean[] done;

  public GS (int N) {
    this.man = new String[N];
    this.woman = new String[N];
    this.manPref = new int[N][N];
    this.womanPref = new int[N][N];
    this.next = new int[N];
    this.current = new Integer[N];
    this.ranking = new int[N][N];
    this.done = new boolean[N];
  }

  public static void main(String[] args) {
    Scanner sc = new Scanner(System.in);

    String line;
    while (true) {
      line = sc.nextLine();
      if (!line.startsWith("#")) break;
    }

    int N = Integer.parseInt(line.split("n=")[1]);

    GS gs = parse(N, sc);

    sc.close();

    while (!gs.free.isEmpty()) {
      int man = gs.free.pop();
      
      if (gs.done[man]) continue;

      int woman = gs.manPref[man][gs.next[man]];
      gs.next[man] += 1;
      if (gs.next[man] >= N) gs.done[man] = true;
      
      boolean wIsFree = gs.current[woman] == null;

      if (wIsFree) {
        gs.current[woman] = man;
      } else {
        int man2 = gs.current[woman];
        int manRank = gs.ranking[woman][man];
        int man2Rank = gs.ranking[woman][man2];

        if (manRank > man2Rank) {
          gs.free.push(man);
        } else {
          gs.current[woman] = man;
          gs.free.push(man2);
        }
      }
    }
    printPairings(N, gs);
  }

  private static GS parse(int N, Scanner sc) {

    GS gs = new GS(N);

    for (int i = 0; i < N*2; i++) {
      String[] tokens = sc.nextLine().split(" ");
      int id = (Integer.parseInt(tokens[0])-1)/2;
      String name = tokens[1];
      
      boolean isMan = i%2==0;
      if (isMan) gs.man[id] = name;
      else       gs.woman[id] = name;
    }

    sc.nextLine();

    for (int i = 0; i < N*2; i++) {
      String[] parts = sc.nextLine().split(": ");
      int rawId = Integer.parseInt(parts[0]);
      int id = (rawId-1)/2;
      String[] prefsTokens = parts[1].split(" ");

      int[] prefs = new int[N];
      
      for (int j = 0; j < N; j++) {
        prefs[j] = (Integer.parseInt(prefsTokens[j])-1)/2;
      }

      boolean isMan = (rawId-1)%2==0;
      if (isMan) gs.manPref[id] = prefs;
      else       gs.womanPref[id] = prefs;

      if (!isMan) {
        for (int j = 0; j < N; j++) {
          gs.ranking[id][prefs[j]] = j;
        }
      }
    }

    for (int i = 0; i < N; i++) {
      gs.free.push(i);
    }

    return gs;
  }

  private static void printPairings(int N, GS gs) {
    for (int woman = 0; woman < N; woman++) {
      Integer man = gs.current[woman];

      System.out.println(((man==null) ? "null" : gs.man[man]) + " -- " + gs.woman[woman]);
    }
    System.out.println();
  }

  private static void printStuff(int N, GS gs) {
    for (int i = 0; i < N; i++)
      System.out.println(i + ": " + gs.man[i]);
    for (int i = 0; i < N; i++)
      System.out.println(i + ": " + gs.woman[i]);
    for (int i = 0; i < N; i++) {
      int[] pref = gs.manPref[i];
      System.out.print(gs.man[i] + ": ");
      for (int j = 0; j < N; j++) {
        System.out.print(gs.woman[pref[j]] + ", ");
      }
      System.out.println();
    }
    for (int i = 0; i < N; i++) {
      int[] pref = gs.womanPref[i];
      System.out.print(gs.woman[i] + ": ");
      for (int j = 0; j < N; j++) {
        System.out.print(gs.man[pref[j]] + ", ");
      }
      System.out.println();
    }
  }
}


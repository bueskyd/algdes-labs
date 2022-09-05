
namespace Matching;

public class Person 
{
    public int id { get; internal set; }
    public string Name { get; internal set; }
}

public class Proposer : Person
{
    public FixedStack<int> Prefs { get; internal set; } 
}

public class Rejector : Person
{
    public Proposer engagement { get; internal set; }
    public int[] Prefs { get; internal set; }
}

public class FixedStack<T>
{
    private T[] Data;
    private int Index;

    public FixedStack(int size) {
        Data = new T[size];
        Index = -1;
    }

    public T this[int idx] {
        get {
            return Data[idx];
        }
    }

    public int Count {
        get { return Index+1; }
    }

    public void Push(T element) {
        Data[++Index] = element;
    }

    public T Pop() {
        return Data[Index--];
    }
}

public class Program
{
    // ***
    // HELPERS
    public static void PrintMatch(Rejector r)
    {
        Console.WriteLine($"{r.engagement.Name} -- {r.Name}");
    }

    public static string[] ReadConsole()
    {
        List<string> buffer = new List<string>();
        while(true) {
            var line = Console.ReadLine();
            if (line is null) return buffer.ToArray();
            buffer.Add(line);
        }
    }

    // ***
    // DRIVER
    public static void Main(string[] args)
    {
        int index = 0;
        string[] lines = (args.Length == 0) ? ReadConsole() : File.ReadAllLines(args[0]);
        while(true) if (lines[index].StartsWith("#")) index++; else break;
        foreach(var s in lines) s.Trim();

        int n = int.Parse(lines[index++].Substring(2));

        FixedStack<Proposer> proposers = new FixedStack<Proposer>(n);
        Rejector[] rejectors = new Rejector[n];

        for(int i = 0; i < n*2; i++) {
            var info = lines[index++].Split(" ");
            if (i % 2 == 0) {
                var p = new Proposer() { id = (int.Parse(info[0])-1)/2, Name = info[1], Prefs = new FixedStack<int>(n) };
                proposers.Push(p);
            }
            else {
                var r = new Rejector() { id = (int.Parse(info[0])-1)/2, Name = info[1], Prefs = new int[n] };
                rejectors[r.id] = r;
            }
        }

        index++;

        for(int i = 0; i < n*2; i++) {
            var info = lines[index++].Replace(":", "").Split(" ");

            Person person;
            var id = (int.Parse(info[0])-1);
            if ( id % 2 == 0) {
                person = proposers[id/2];
            }
            else {
                person = rejectors[id/2];
            }

            for(int o = 0; o < n; o++) {
                if (person is Proposer prop) {
                    prop.Prefs.Push((int.Parse(info[(n-(1+o))+1])-1)/2);
                }
                else if (person is Rejector reje) {
                    reje.Prefs[int.Parse(info[1+o])/2] = o;
                }
            }
        }

        // Destructive to 'proposeres'
        Match(proposers, rejectors);

        foreach(var p in rejectors) PrintMatch(p);
    }

    // ***
    // ALGORITHM
    public static void Match(FixedStack<Proposer> proposers, Rejector[] rejectors)
    {
        while(proposers.Count > 0) {
            var proposer = proposers.Pop();
            var rejector = rejectors[proposer.Prefs.Pop()];

            if (rejector.engagement is null) {
                rejector.engagement = proposer;
                continue;
            }
            
            var current = rejector.Prefs[rejector.engagement.id];
            var given = rejector.Prefs[proposer.id];

            // Lower is better
            if (given < current) {
                proposers.Push(rejector.engagement);
                rejector.engagement = proposer;
            }
            else proposers.Push(proposer);
        }
    }
}
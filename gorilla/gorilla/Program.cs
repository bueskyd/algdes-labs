using System.Text.RegularExpressions;
using System.Text;

using static SeqElement;

public class CacheEntry
{
    public int Value { get; set; }
    public char? Symbols { get; set; }
    public byte BackTrack { get; set; }
}
// Could combine Symbols and BackTrack in a char, bbxxxxxxxyyyyyyy, where b is BackTrack, x is the first symbol and y is the second
// This would get us to 6B pr. entry, excluding object overhead

public class Program
{
    private readonly int[,] ValueMatrix;
    private int[,] Cache;
    private readonly Dictionary<string, SeqElement[]> Sequences;

    public static void Main(string[] args)
    {
        var matrix = LoadCostMatrix(args[0]);
        var seqs = LoadSequences(args[1]);

        var program = new Program(matrix, seqs);

        System.Console.WriteLine("Choose pair:");
        program.Match(Console.ReadLine(), Console.ReadLine());
    }

    public static CacheEntry Cache_Entry(int v, char? s, byte bt) {
        return new CacheEntry() {
            Value = v,
            Symbols = s,
            BackTrack = bt,
        };
    }

    public Program(int[,] matrix, Dictionary<string, SeqElement[]> seqs) {
        ValueMatrix = matrix;
        Sequences = seqs;
    }

    public void Match(string name1, string name2)
    {
        var seq1 = Sequences[name1];
        var seq2 = Sequences[name2];

        var Cache = new CacheEntry[seq1.Length+1, seq2.Length+1];

        var deltaValue = Value(_,A);

        var i = 0;
        var j = 0;
        while(i < seq1.Length+1) {Cache[i,0] = Cache_Entry(i * deltaValue, null, 0b00); i++;}
        while(j < seq2.Length+1) {Cache[0,j] = Cache_Entry(j * deltaValue, null, 0b00); j++;}

        i = 1;
        while(i <= seq1.Length) {
            j = 1;
            while(j <= seq2.Length) {

                var match = Value(seq1[i-1], seq2[j-1]) + Cache[i-1, j-1].Value;
                var move1 = deltaValue + Cache[i-1,j].Value;
                var move2 = deltaValue + Cache[i,j-1].Value;

                if (move1 >= match && move1 >= move2) 
                    Cache[i,j] = Cache_Entry(move1, Helpers.combine(seq1[i-1],_), 0b10);
                else if (move2 >= match && move2 >= move1) 
                    Cache[i,j] = Cache_Entry(move2, Helpers.combine(_,seq2[j-1]), 0b01);
                else 
                    Cache[i,j] = Cache_Entry(match, Helpers.combine(seq1[i-1],seq2[j-1]), 0b11);

                j++;
            }
            i++;
        }

        System.Console.WriteLine($"{name1} : {name2} -> {Cache[seq1.Length, seq2.Length].Value}");
        var sbi = new StringBuilder();
        var sbj = new StringBuilder();
        i = seq1.Length;
        j = seq2.Length;
        while(true) {
            var entry = Cache[i,j];
            if (entry.BackTrack == 0b00) break;
            var (seq1e, seq2e) = Helpers.split(entry.Symbols.Value);
            sbi.Append(seq1e);
            sbj.Append(seq2e);
            if ((entry.BackTrack & 0b10) != 0) i--;
            if ((entry.BackTrack & 0b01) != 0) j--;
        }
        
        Console.WriteLine(new string(sbi.ToString().Reverse().ToArray()));
        Console.WriteLine(new string(sbj.ToString().Reverse().ToArray()));
        Console.WriteLine();

        Cache = null;
    }

    public static Dictionary<string, SeqElement[]> LoadSequences(string path) 
    {
        var contents = File.ReadAllLines(path);
        var results = new Dictionary<string, SeqElement[]>();

        string temp_name;
        string temp_seq = "";

        int lp = 0;
        while(lp < contents.Length-1) {
            temp_name = contents[lp].Split(" ")[0].Substring(1);
            int lc = 1;
            while(true) {
                if (lp+lc >= contents.Length || contents[lp+lc].StartsWith('>')) {
                    results.Add(temp_name, LoadSequence(temp_seq));
                    temp_seq = "";
                    lp += lc;
                    break;
                }
                temp_seq = String.Concat(temp_seq, contents[lp+lc]);
                lc++;
            }
        }

        return results;
    }

    public static SeqElement[] LoadSequence(string seq) {
        var result = new SeqElement[seq.Length];
        var chars = seq.ToCharArray();

        for(int i = 0; i < chars.Length; i++) result[i] = Helpers.toSeqElem(chars[i]);

        return result;
    }

    public static int[,] LoadCostMatrix(string path)
    {
        var contents = File.ReadAllLines(path);

        var lp = 0;
        while(contents[lp].StartsWith('#')) lp++;
        lp++;

        var elementCount = contents.Length - lp;

        var result = new int[elementCount,elementCount];

        for(int o = 0; o < elementCount; o++) {
            var line = Regex.Split(contents[o+lp], " +");
            for(int i = 1; i < elementCount+1; i++) {
                result[o,i-1] = int.Parse(line[i]);
            }
        }

        return result;
    }

    private int Value(SeqElement a, SeqElement b) {
        return ValueMatrix[(int)a,(int)b];
    }
}
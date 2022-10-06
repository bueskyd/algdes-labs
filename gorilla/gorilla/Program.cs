using System.Text.RegularExpressions;
using System.Text;

using static SeqElement;

public class Program
{
    private readonly int[,] ValueMatrix;
    private readonly Dictionary<string, SeqElement[]> Sequences;

    public static void Main(string[] args)
    {
        var matrix = LoadCostMatrix(args[1]);
        var seqs = LoadSequences(args[2]);

        var program = new Program(matrix, seqs);

        switch(args[0]) {
            case "single":
                System.Console.WriteLine("Choose pair:");
                program.Match(Console.ReadLine(), Console.ReadLine());
                break;
            case "all":
                var keys = seqs.Keys.ToArray();
                var c = 0;
                var i = 0;
                while(i < keys.Length-1) {
                    var j = i+1;
                    while(j < keys.Length) {
                        program.Match(keys[i], keys[j]);
                        c++;
                        j++;
                    }
                    i++;
                }
                System.Console.WriteLine($"Ran {c} matches");
                break;
            default:
                System.Console.WriteLine("Unknown command!");
                break;
        }
    }

    public Program(int[,] matrix, Dictionary<string, SeqElement[]> seqs) {
        ValueMatrix = matrix;
        Sequences = seqs;
    }

    public void Match(string name1, string name2)
    {
        var seq1 = Sequences[name1];
        var seq2 = Sequences[name2];

        var Cache = new (int value, byte trace)[seq1.Length+1, seq2.Length+1];

        var deltaValue = Value(_,A);

        var i = 0;
        var j = 0;
        while(i < seq1.Length+1) {Cache[i,0] = (i * deltaValue, 0b00); i++;}
        while(j < seq2.Length+1) {Cache[0,j] = (j * deltaValue, 0b00); j++;}

        i = 1;
        while(i <= seq1.Length) {
            j = 1;
            while(j <= seq2.Length) {
                
                var match = Value(seq1[i-1], seq2[j-1]) + Cache[i-1, j-1].value;
                var move1 = deltaValue + Cache[i-1,j].value;
                var move2 = deltaValue + Cache[i,j-1].value;

                if (move1 >= match && move1 >= move2) 
                    Cache[i,j] = (move1, 0b10);
                else if (move2 >= match && move2 >= move1) 
                    Cache[i,j] = (move2, 0b01);
                else 
                    Cache[i,j] = (match, 0b11);

                j++;
            }
            i++;
        }

        System.Console.WriteLine($"{name1} : {name2} -> {Cache[seq1.Length, seq2.Length].value}");
        var sbi = new StringBuilder();
        var sbj = new StringBuilder();
        i = seq1.Length;
        j = seq2.Length;
        while(true) {
            var entry = Cache[i,j];

            if (entry.trace == 0b00) {
                while(i > 0) {
                    sbi.Append(Helpers.toChar(seq1[i-1]));
                    sbj.Append('-');
                    i--;
                };
                while(j > 0) {
                    sbi.Append('-');
                    sbj.Append(Helpers.toChar(seq2[j-1]));
                    j--;
                }
                break;
            }

            if ((entry.trace & 0b10) != 0) {
                sbi.Append(Helpers.toChar(seq1[i-1]));
                i--;
            } 
            else sbi.Append("-");
            if ((entry.trace & 0b01) != 0) {
                sbj.Append(Helpers.toChar(seq2[j-1]));
                j--;
            }
            else sbj.Append("-");
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
public enum SeqElement
{
    A,
    R,
    N,
    D,
    C,
    Q,
    E,
    G,
    H,
    I,
    L,
    K,
    M,
    F,
    P,
    S,
    T,
    W,
    Y,
    V,
    B,
    Z,
    X,
    _,
}

public class Helpers
{
    public static SeqElement toSeqElem(char c) {
        switch (c) {
                case 'A': return  SeqElement.A;
                case 'R': return  SeqElement.R;
                case 'N': return  SeqElement.N;
                case 'D': return  SeqElement.D;
                case 'C': return  SeqElement.C;
                case 'Q': return  SeqElement.Q;
                case 'E': return  SeqElement.E;
                case 'G': return  SeqElement.G;
                case 'H': return  SeqElement.H;
                case 'I': return  SeqElement.I;
                case 'L': return  SeqElement.L;
                case 'K': return  SeqElement.K;
                case 'M': return  SeqElement.M;
                case 'F': return  SeqElement.F;
                case 'P': return  SeqElement.P;
                case 'S': return  SeqElement.S;
                case 'T': return  SeqElement.T;
                case 'W': return  SeqElement.W;
                case 'Y': return  SeqElement.Y;
                case 'V': return  SeqElement.V;
                case 'B': return  SeqElement.B;
                case 'Z': return  SeqElement.Z;
                case 'X': return  SeqElement.X;
                case '-': return  SeqElement._;
                default:
                    throw new Exception($"Unknown sequence element: {c}");
        }
    }

    public static char toChar(SeqElement s) {
        switch (s) {
                case SeqElement.A : return 'A';
                case SeqElement.R : return 'R';
                case SeqElement.N : return 'N';
                case SeqElement.D : return 'D';
                case SeqElement.C : return 'C';
                case SeqElement.Q : return 'Q';
                case SeqElement.E : return 'E';
                case SeqElement.G : return 'G';
                case SeqElement.H : return 'H';
                case SeqElement.I : return 'I';
                case SeqElement.L : return 'L';
                case SeqElement.K : return 'K';
                case SeqElement.M : return 'M';
                case SeqElement.F : return 'F';
                case SeqElement.P : return 'P';
                case SeqElement.S : return 'S';
                case SeqElement.T : return 'T';
                case SeqElement.W : return 'W';
                case SeqElement.Y : return 'Y';
                case SeqElement.V : return 'V';
                case SeqElement.B : return 'B';
                case SeqElement.Z : return 'Z';
                case SeqElement.X : return 'X';
                case SeqElement._ : return '-';
                default:
                    throw new Exception($"Unknown sequence element: {s}");
        }
    }

    public static char combine(SeqElement a, SeqElement b) {
        var ac = (char) a;
        var bc = (char) b;
        return (char)((ac << 8) | bc);
    }

    public static (char, char) split(char c) {
        var msbs = c >> 8;
        var lsbs = 0b11111111 & c;
        return (toChar((SeqElement)msbs), toChar((SeqElement)lsbs));
    }
}
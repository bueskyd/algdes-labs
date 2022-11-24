namespace RedScare.entities
{
    public class Node
    {
        public string Name;
        public Color Color;
        public uint Index;

        public Node(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Color)}: {Color}, {nameof(Index)}: {Index}";
        }
    }
}
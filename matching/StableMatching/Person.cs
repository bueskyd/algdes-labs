namespace StableMatching;

public class Person
{
    public Person(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public Person(int id, string name, int[] priorities)
    {
        this.id = id;
        this.name = name;
        this.priorities = priorities;
    }

    public int id;
    public string name;
    public int[] priorities;

    public override string ToString()
    {
        return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(priorities)}: {priorities}";
    }
}

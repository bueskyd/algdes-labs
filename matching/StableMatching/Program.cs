// See https://aka.ms/new-console-template for more information

using StableMatching;

var n = ThoreParser.ParseInt();
var persons = new Person[n * 2];

// fill people 
for (var i = 0; i < n * 2; i++)
{
    var line = ThoreParser.GetNextLineNonComment();
    var arr = line.Split(" ");

    var id = int.Parse(arr[0]);
    var name = arr[1];
    var person = new Person(id, name);
    
    // assuming the list of persons is always specified in the order of their id's
    persons[id - 1] = person;
}

// fill priorities 
for (var i = 0; i < n * 2; i++)
{
    var line = ThoreParser.GetNextLineNonComment();
    var arr = line.Split(": ");
    
    var id = int.Parse(arr[0]);
    var prioritiesString = arr[1];
    var prioritiesWithId = prioritiesString.Split(" ").Select(int.Parse).ToArray();
    
    // persons are indexed by their id as specified in the docs
    var person = persons[id - 1];
    person.priorities = prioritiesWithId;
    persons[id - 1] = person;
}

var men = persons.Where(p => IsMan(p.id)).Select(p =>
{
    // convert women id's to their index in the women array
    var priorities = p.priorities.Select(id => id / 2 - 1).ToArray();
    return new Person(p.id, p.name, priorities);
}).ToArray();

var women = persons.Where(p => !IsMan(p.id)).Select(p =>
{
    // convert men id's to their index in the men array
    var priorities = p.priorities.Select(id => id / 2).ToArray();
    return new Person(p.id, p.name, priorities);
}).ToArray();

bool IsMan(int id)
{
    return id % 2 == 1;
}

var matches = StableMatchAlgorithm.StableMatch(men, women)
    .Select((n, i) => (n, i))
    .OrderBy(entry => entry.n);

foreach (var entry in matches)
{
    Console.WriteLine($"{men[entry.n].name} -- {women[entry.i].name}");
}

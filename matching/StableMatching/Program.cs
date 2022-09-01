// See https://aka.ms/new-console-template for more information

using StableMatching;

var thoreParser = new ThoreParser("../../../sm-kt-p-4-in.txt");
var n = thoreParser.ParseInt();

var persons = new Person[n * 2];

// fill people 
for (var i = 0; i < n * 2; i++)
{
    var line = thoreParser.GetNextLineNonComment();
    var arr = line.Split(" ");

    var id = int.Parse(arr[0]);
    var name = arr[1];
    var person = new Person(id, name);
    
    persons[id - 1] = person;
}

// fill priorities 
for (var i = 0; i < n * 2; i++)
{
    var line = thoreParser.GetNextLineNonComment();
    var arr = line.Split(": ");
    
    var id = int.Parse(arr[0]);
    var prioritiesString = arr[1];
    var prioritiesStringArray = prioritiesString.Split(" ");
    var priorities = prioritiesStringArray.Select(int.Parse).ToArray();

    var person = persons[id - 1];
    person.priorities = priorities;
    persons[id - 1] = person;
}

persons = persons.OrderBy(p => p.id).ToArray(); 
var men = persons.Where(p => p.id % 2 == 1).ToArray();
var women = persons.Where(p => p.id % 2 == 0).ToArray();

var matches = StableMatchAlgorithm.StableMatch(n, men, women)
    .Select((n, i) => (n, i))
    .OrderBy(entry => entry.n);

foreach (var entry in matches)
{
    Console.WriteLine($"{men[entry.n].name} -- {women[entry.i].name}");
}

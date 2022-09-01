using System.Text.RegularExpressions;

namespace StableMatching;

public class ThoreParser
{
    private readonly StreamReader reader;
    
    public ThoreParser(string fileName)
    {
        reader = File.OpenText(fileName);
    }

    public int ParseInt()
    {
        var intLine = GetNextLineNonComment();

        var stringVal = Regex.Match(intLine, @"\d+").Value;
        return int.Parse(stringVal);
        
    }

    public string GetNextLineNonComment()
    {
        var nextLine = reader.ReadLine();
        if (nextLine == null)
        {
            throw new Exception("No new line");
        }
        
        while (nextLine.StartsWith("#") || nextLine.Length == 0)
        {
            nextLine = reader.ReadLine();
        }

        return nextLine;
    }
}
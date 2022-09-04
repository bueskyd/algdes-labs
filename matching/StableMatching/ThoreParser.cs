using System.Text.RegularExpressions;

namespace StableMatching;

public class ThoreParser
{
    public static int ParseInt()
    {
        var intLine = GetNextLineNonComment();

        var stringVal = Regex.Match(intLine, @"\d+").Value;
        return int.Parse(stringVal);
        
    }

    public static string GetNextLineNonComment()
    {
        var nextLine = Console.ReadLine();
        if (nextLine == null)
        {
            throw new Exception("No new line");
        }
        
        while (nextLine.StartsWith("#") || nextLine.Length == 0)
        {
            nextLine = Console.ReadLine();
        }

        return nextLine.Trim();
    }
}
using System.Text.RegularExpressions;

var lines = File.ReadLines("testdata.txt").ToList().Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
var packets = new List<Packet>();

for (int i = 0; i < lines.Count(); i+=2)
{
    packets.Add(new Packet
    {
        Part1 = GetMembers(lines[i]),
        Part2 = GetMembers(lines[i+1])
    });
}

Console.ReadLine();

List<string> GetMembers(string line)
{
    var members = new List<string>();

    while (true)
    {
        var indexesOfOpenings = line.IndexOfAll("[").Reverse();

        if(indexesOfOpenings == null || indexesOfOpenings.Count() == 0)
        {
            break;
        }

        var opening = indexesOfOpenings.First();
        var closingSearchArea = line.Substring(opening);
        var closing = closingSearchArea.IndexOf(closingSearchArea.First(x => x == closingSearchArea.First(y => y == ']')));
        var distanceBetweenOpeningAndClosing = closing - opening;
        var content = line.Substring(opening, opening + distanceBetweenOpeningAndClosing + 1);
        line = line.Replace(content, "@");
        members.Add(content);
    }
    members.Reverse();
    return members;
}

class Packet
{
    public List<string> Part1 { get; set; }

    public List<string> Part2 { get; set; }
}

public static class Extensions
{
    public static IEnumerable<int> IndexOfAll(this string sourceString, string subString)
    {
        return Regex.Matches(sourceString, Regex.Escape(subString)).Cast<Match>().Select(m => m.Index);
    }
}
var elves = new List<int>();
string[] lines = File.ReadAllLines(@"day1.txt");

var elfCalorieCounter = 0;

foreach (string line in lines)
{
    if(line == string.Empty)
    {
        elves.Add(elfCalorieCounter);
        elfCalorieCounter = 0;
    }
    else
    {
        elfCalorieCounter += int.Parse(line);
    }
}

var caloriesOfMostPreparedElf = elves.Max();
var indexOfMostPreparedElf = elves.IndexOf(caloriesOfMostPreparedElf);

Console.WriteLine($"Most Prepared Elf");
Console.WriteLine($"Calories: {caloriesOfMostPreparedElf}, Position:{indexOfMostPreparedElf}");

var sortedElves = elves.OrderByDescending(elve => elve).ToList();

var topThreeElves = sortedElves.Take(3).ToList();

Console.WriteLine($"Top three prepared elves");
foreach (var elve in topThreeElves)
{
    Console.WriteLine($"Calories: {elve}");
}

Console.WriteLine($"Total calories: {topThreeElves.Sum(elf => elf)}");

Console.ReadLine();

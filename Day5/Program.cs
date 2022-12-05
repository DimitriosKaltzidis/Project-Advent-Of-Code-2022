using System.Text.RegularExpressions;

var listOfStacks = new List<Stack<string>>();
var loadingProgram = File.ReadAllLines(@"day5.txt").ToList();
var indexOfCrateAndInstrunctionSeperator = loadingProgram.IndexOf(string.Empty);
var calculatePartTwo = false;

for (int i = indexOfCrateAndInstrunctionSeperator - 2; i >= 0 ; i--)
{
    var itemsPerStackLayer = loadingProgram[i].Replace("[", " ").Replace("]", " ").Split("   ");
    
    // Shitty fix becuase of incrorrent splitting above so.. just to get an answer skip the first position
    if(itemsPerStackLayer.Length > 9) itemsPerStackLayer = itemsPerStackLayer.Skip(1).ToArray();
    if (listOfStacks.Count == 0) foreach (var item in itemsPerStackLayer) { listOfStacks.Add(new Stack<string>()); }

    for (int j = 0; j < itemsPerStackLayer.Length; j++)
    {
        var item = itemsPerStackLayer[j].Replace(" ", string.Empty);
        if (!string.IsNullOrEmpty(item)) listOfStacks[j].Push(item);
    }
}

for (var i = indexOfCrateAndInstrunctionSeperator + 1; i < loadingProgram.Count; i++)
{
    var instructionFirstPart = Regex.Replace(loadingProgram[i].Split("from")[0], @"[^\d]", "");
    var instructionSecondPart = Regex.Replace(loadingProgram[i].Split("from")[1], @"[^\d]", "");
    var crateToRemoveFrom = listOfStacks[int.Parse(instructionSecondPart[0].ToString()) - 1];
    var crateToAddTo = listOfStacks[int.Parse(instructionSecondPart[1].ToString()) - 1];
    var removedCrateItems = new List<string>();

    for (var j = 0; j < int.Parse(instructionFirstPart.ToString()); j++)
    {
        removedCrateItems.Add(crateToRemoveFrom.Pop());
    }
    
    if(calculatePartTwo) removedCrateItems.Reverse();
    
    foreach (var it in removedCrateItems)
    {
        crateToAddTo.Push(it);
    }
}

foreach(var item in listOfStacks)
{
    Console.Write(item.First());
}
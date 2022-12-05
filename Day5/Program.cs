using System.Text.RegularExpressions;
var loadingProgram = File.ReadAllLines(@"day5.txt").ToList();
Console.WriteLine($"Part One: {CalculateTopStackItems(loadingProgram)} - Part Two: {CalculateTopStackItems(loadingProgram, true)}");

string CalculateTopStackItems(List<string> loadingProgram, bool isCrateMover9001 = false)
{
    var crateStacks = new List<Stack<string>>();
    var indexOfCrateStackAndLoadingInstrunctionSeperator = loadingProgram.IndexOf(string.Empty);

    for (int i = indexOfCrateStackAndLoadingInstrunctionSeperator - 2; i >= 0; i--) // We read crate stacks from bottom to top
    {
        var suppliesPerCrateStackLayer = loadingProgram[i].Replace("] ", "]").Replace("   ", "-").Replace("[", string.Empty).Replace("]", string.Empty).Replace(" ", string.Empty);

        if (crateStacks.Count == 0) foreach (var item in suppliesPerCrateStackLayer) { crateStacks.Add(new Stack<string>()); }

        for (int j = 0; j < suppliesPerCrateStackLayer.Length; j++)
        {
            var item = suppliesPerCrateStackLayer[j].ToString();
            if (item != "-") crateStacks[j].Push(item);
        }
    }

    for (var i = indexOfCrateStackAndLoadingInstrunctionSeperator + 1; i < loadingProgram.Count; i++)
    {
        var loadingInstructionFirstPart = Regex.Replace(loadingProgram[i].Split("from")[0], @"[^\d]", "");
        var loadingInstructionSecondPart = Regex.Replace(loadingProgram[i].Split("from")[1], @"[^\d]", "");
        var crateStackToRemoveFrom = crateStacks[int.Parse(loadingInstructionSecondPart[0].ToString()) - 1];
        var crateStackToAddTo = crateStacks[int.Parse(loadingInstructionSecondPart[1].ToString()) - 1];
        var suppliesToBeMoved = new List<string>();

        for (var j = 0; j < int.Parse(loadingInstructionFirstPart.ToString()); j++) suppliesToBeMoved.Add(crateStackToRemoveFrom.Pop());
        if (isCrateMover9001) suppliesToBeMoved.Reverse();
        foreach (var supply in suppliesToBeMoved) crateStackToAddTo.Push(supply);
    }

    return string.Join(string.Empty, crateStacks.Select(x => x.First()));
}
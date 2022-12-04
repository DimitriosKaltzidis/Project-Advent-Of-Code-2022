var elves = File.ReadAllLines(@"daythree.txt");
Console.WriteLine($"Part One {CalculateTotalElfCommonItemPriority(elves)} - Part Two {CalculateTotalElfGroupPriority(elves)}");

int CalculateTotalElfCommonItemPriority(string[] elves)
{
    var priority = 0;
    foreach (var elf in elves)
    {
        var firstCompartmentOfRucksack = elf.Substring(0, (int)(elf.Length / 2));
        var secondCompartmentOfRucksack = elf.Substring((int)(elf.Length / 2), (int)(elf.Length / 2));
        var rucksackCommonItem = firstCompartmentOfRucksack.Intersect(secondCompartmentOfRucksack).FirstOrDefault();
        priority += CalculateCharacterPriority(rucksackCommonItem);
    }

    return priority;
}

int CalculateTotalElfGroupPriority(string[] elves)
{
    var priority = 0;
    for (int i = 0; i < elves.Length; i = i + 3)
    {
        var groupOfThreeElfRucksacks = elves.Skip(i).Take(3);
        groupOfThreeElfRucksacks.Skip(1)
        .Aggregate(new HashSet<char>(groupOfThreeElfRucksacks.First()), (commonGroupItems, nextElfRucksackInGroup) =>
            {
                commonGroupItems.IntersectWith(nextElfRucksackInGroup);
                if (commonGroupItems.Count == 1) priority += CalculateCharacterPriority(commonGroupItems.First());
                return commonGroupItems;
            });
    }

    return priority;
}

int CalculateCharacterPriority(char commonItem)
{
    var priority = (int)Char.ToLower(commonItem) % 32;
    if (Char.IsUpper(commonItem)) priority += 26;
    return priority;
}
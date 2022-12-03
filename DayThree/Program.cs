var elves = File.ReadAllLines(@"daythree.txt");
Console.WriteLine($"Part One {CalculateTotalElfCommonItemPriority(elves)} - Part Two {CalculateTotalElfGroupPriority(elves)}");

int CalculateTotalElfCommonItemPriority(string[] elves)
{
    var priority = 0;
    foreach (var elf in elves)
    {
        var firstCompartmentOfRucksack = elf.Substring(0, (int)(elf.Length / 2));
        var secondCompartmentOfRucksack = elf.Substring((int)(elf.Length / 2), (int)(elf.Length / 2));
        var commonItem = firstCompartmentOfRucksack.Intersect(secondCompartmentOfRucksack).FirstOrDefault();
        priority += CalculateCharacterPriority(commonItem);
    }

    return priority;
}

int CalculateTotalElfGroupPriority(string[] elves)
{
    var priority = 0;
    for (int i = 0; i < elves.Length; i = i + 3)
    {
        var group = elves.Skip(i).Take(3);
        group.Skip(1)
        .Aggregate(
            new HashSet<char>(group.First()),
            (h, e) =>
            {
                h.IntersectWith(e);
                if (h.Count == 1) priority += CalculateCharacterPriority(h.First());
                return h;
            }
        );
    }

    return priority;
}

int CalculateCharacterPriority(char commonItem)
{
    var priority = (int)Char.ToLower(commonItem) % 32;
    if (Char.IsUpper(commonItem)) priority += 26;
    return priority;
}
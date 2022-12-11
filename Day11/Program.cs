using System.Text.RegularExpressions;
var monkeyProperties = File.ReadAllLines(@"testdata.txt").ToList();
var troopOfMonkeys = InitializeMonkeyTroopFromFile(monkeyProperties);
var maxRounds = 20;

for (int i = 0; i < maxRounds; i++)
{
    //Console.WriteLine($"--- Start Of Round {i + 1} ---");

    foreach (var monkey in troopOfMonkeys)
    {
        var itemTranfersToOtherMonkeys = monkey.InspectAndGiveItemsToOtherMokeys();

        foreach (var transfer in itemTranfersToOtherMonkeys)
        {
            var targetMonkey = troopOfMonkeys.First(x => x.Id == transfer.MonkeyIdToGiveItemTo);
            targetMonkey.CatchThrownItem(transfer.Item);
            //Console.WriteLine($"\nMonkey {monkey.Id} threw item {transfer.Item} to Monkey {targetMonkey.Id}");
        }

        //Console.WriteLine($"\n\n{monkey.StolenRucksackItems.Count} stolen items in Monkey {monkey.Id}\n");
    }

    Console.WriteLine($"--- End Of Round {i + 1} ---");

    foreach (var monkey in troopOfMonkeys)
    {
        monkey.PrintTotalItemsInspected();
    }
}



Console.WriteLine($"Part One: {troopOfMonkeys.Select(x => x.TotalItemsInspected).OrderByDescending(x => x).Take(2).Aggregate((a, b) => a * b)}");
Console.ReadLine();

List<Monkey> InitializeMonkeyTroopFromFile(List<string> fileLines)
{
    var monkeys = new List<Monkey>();
    var monkeyIds = fileLines.Where(fileLines => fileLines.Contains("Monkey")).ToList();

    foreach (var monkeyId in monkeyIds)
    {
        var indexOfMonkeyId = fileLines.IndexOf(monkeyId);
        var id = GetIntegerOnly(monkeyId.Split(' ')[1]);
        var startingItems = fileLines[indexOfMonkeyId + 1].Split(":")[1].Replace(" ", string.Empty).Trim().Split(",").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (ulong)GetIntegerOnly(x)).ToList();
        var operation = GetOperation(fileLines[indexOfMonkeyId + 2]);
        var testDivisionNumber = ulong.Parse(Regex.Replace(fileLines[indexOfMonkeyId + 3], @"[^\d]", string.Empty));
        var successfullTestMonkeyId = GetIntegerOnly(fileLines[indexOfMonkeyId + 4]);
        var notSuccessfullTestMonkeyId = GetIntegerOnly(fileLines[indexOfMonkeyId + 5]);

        monkeys.Add(new Monkey(id: id, testDivisionNumber: testDivisionNumber, successfullTestMonkeyId: successfullTestMonkeyId, notSuccessfullTestMonkeyId: notSuccessfullTestMonkeyId, operation, startingItems: startingItems));
    }

    return monkeys;
}

Func<ulong, ulong> GetOperation(string line)
{
    var operationPart = line.Replace(" ", string.Empty).Trim().Split('=')[1].Trim();

    if (operationPart.Contains("+"))
    {
        var elementsOfOperation = operationPart.Split('+');
        var secondOperant = elementsOfOperation[1];
        return secondOperant == "old" ? x => x + x : x => x + (ulong)GetIntegerOnly(secondOperant);
    }
    else
    {
        var elementsOfOperation = operationPart.Split('*');
        var secondOperant = elementsOfOperation[1];
        return secondOperant == "old" ? x => x * x : x => x * (ulong)GetIntegerOnly(secondOperant);
    }
}

int GetIntegerOnly(string line)
{
    return int.Parse(Regex.Replace(line, @"[^\d]", ""));
}

class Monkey
{
    public Monkey(int id, ulong testDivisionNumber, int successfullTestMonkeyId, int notSuccessfullTestMonkeyId, Func<ulong, ulong> worryLevelOperation, List<ulong> startingItems)
    {
        this.Id = id;
        this.TestDivisionNumber = testDivisionNumber;
        this.MonkeyIdForAnItemToBeThrownIfTestIsSuccessfull = successfullTestMonkeyId;
        this.MonkeyIdForAnItemToBeThrownIfTestIsNotSuccessfull = notSuccessfullTestMonkeyId;
        this.WorryLevelOperation = worryLevelOperation;
        this.StolenRucksackItems = startingItems;
    }

    public int Id { get; set; }

    public ulong TestDivisionNumber { get; set; }

    public int MonkeyIdForAnItemToBeThrownIfTestIsSuccessfull { get; set; }

    public int MonkeyIdForAnItemToBeThrownIfTestIsNotSuccessfull { get; set; }

    public List<ulong> StolenRucksackItems { get; set; } = new List<ulong>();

    public ulong TotalItemsInspected { get; set; } = 0;

    private Func<ulong, ulong> WorryLevelOperation;

    //private Func<ulong, ulong, int, int, int> IsDivisable = (item, divisionNumber, successFullTestMonkeyId, notSuccessFullTestMonkeyId) => { return item % divisionNumber == 0 ? successFullTestMonkeyId : notSuccessFullTestMonkeyId; };

    public List<ItemTransfer> InspectAndGiveItemsToOtherMokeys()
    {
        var itemsToGive = new List<ItemTransfer>();

        if(Id == 3)
        {

        }

        for (int i = 0; i < StolenRucksackItems.Count; i++)
        {
            var hasMoreItemsToThrow = CalculateWorryLevelAndDestinationMonkeyOfItemHeld(i, out var transfer);

            if (hasMoreItemsToThrow) itemsToGive.Add(transfer);
            else Console.WriteLine("ELSE");
        }

        StolenRucksackItems.Clear();
        return itemsToGive;
    }

    private bool CalculateWorryLevelAndDestinationMonkeyOfItemHeld(int itemPosition, out ItemTransfer itemTransfer)
    {
        itemTransfer = new ItemTransfer();

        if (this.StolenRucksackItems != null && this.StolenRucksackItems.Count > 0)
        {
            var itemToInspect = this.StolenRucksackItems[itemPosition];
            //Console.WriteLine($"Monkey {this.Id}:\n\tMonkey inspects an item with worry level of {itemToInspect}.");
            itemTransfer.Item = WorryLevelOperation(itemToInspect);
            TotalItemsInspected++;
            //Console.WriteLine($"\tWorry level becomes {itemTransfer.Item}");
            //var nonRoundedResult = itemTransfer.Item / 3d;
            //itemTransfer.Item = (ulong)Math.Floor((decimal)nonRoundedResult);
            //Console.WriteLine($"\tMonkey gets bored with item. Worry level is divided by 3 to {itemTransfer.Item}");
            //-itemTransfer.MonkeyIdToGiveItemTo = IsDivisable(itemTransfer.Item, this.TestDivisionNumber, this.MonkeyIdForAnItemToBeThrownIfTestIsSuccessfull, MonkeyIdForAnItemToBeThrownIfTestIsNotSuccessfull);
            itemTransfer.MonkeyIdToGiveItemTo = itemTransfer.Item % TestDivisionNumber == 0 ? MonkeyIdForAnItemToBeThrownIfTestIsSuccessfull : MonkeyIdForAnItemToBeThrownIfTestIsNotSuccessfull;
            //Console.WriteLine($"\tItem is thrown to monkey {itemTransfer.MonkeyIdToGiveItemTo}");
            return true;
        }

        itemTransfer = new ItemTransfer()
        {
            MonkeyIdToGiveItemTo = 0,
            Item = 0
        };

        return false;
    }

    public void CatchThrownItem(ulong item)
    {
        StolenRucksackItems.Add(item);
    }

    public void PrintTotalItemsInspected()
    {
        Console.WriteLine($"Monkey {this.Id} inspected items {this.TotalItemsInspected} times.");
        //Console.WriteLine($"Monkey {this.Id}: {string.Join(", ", StolenRucksackItems)}");
    }
}

class ItemTransfer
{
    public int MonkeyIdToGiveItemTo { get; set; }
    public ulong Item { get; set; }
}
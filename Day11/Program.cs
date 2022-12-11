using System.Text.RegularExpressions;
var monkeyProperties = File.ReadAllLines(@"day11.txt").ToList();
var partOne = CalculateMonkeyBusiness(monkeyProperties, 20, true);
var partTwo = CalculateMonkeyBusiness(monkeyProperties, 10000);

Console.WriteLine($"Part One: {partOne} - Part Two: {partTwo}");
Console.ReadLine();

long CalculateMonkeyBusiness(List<string> monkeyCharacteristics, int maxRounds, bool calculateWithRelief = false)
{
    var troopOfMonkeys = InitializeMonkeyTroopFromFile(monkeyProperties);
    var factor = troopOfMonkeys.Aggregate((long)1, (c, monkey) => c * monkey.TestDivisionNumber);

    for (int i = 0; i < maxRounds; i++)
    {
        foreach (var monkey in troopOfMonkeys)
        {
            var itemTranfersToOtherMonkeys = calculateWithRelief ? monkey.InspectAndGiveItemsToOtherMokeys(null) : monkey.InspectAndGiveItemsToOtherMokeys(factor);

            foreach (var transfer in itemTranfersToOtherMonkeys)
            {
                var targetMonkey = troopOfMonkeys.First(x => x.Id == transfer.MonkeyIdToGiveItemTo);
                targetMonkey.CatchThrownItem(transfer.Item);
            }
        }
    }

    return troopOfMonkeys.Select(x => x.TotalItemsInspected).OrderByDescending(x => x).Take(2).Aggregate((a, b) => a * b);
}

List<Monkey> InitializeMonkeyTroopFromFile(List<string> fileLines)
{
    var monkeys = new List<Monkey>();
    var monkeyIds = fileLines.Where(fileLines => fileLines.Contains("Monkey")).ToList();

    foreach (var monkeyId in monkeyIds)
    {
        var indexOfMonkeyId = fileLines.IndexOf(monkeyId);
        var id = int.Parse(GetOnlyNumberPart(monkeyId.Split(' ')[1]));
        var startingItems = fileLines[indexOfMonkeyId + 1].Split(":")[1].Replace(" ", string.Empty).Trim().Split(",").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => long.Parse(GetOnlyNumberPart(x))).ToList();
        var operation = GetOperation(fileLines[indexOfMonkeyId + 2]);
        var testDivisionNumber = long.Parse(Regex.Replace(fileLines[indexOfMonkeyId + 3], @"[^\d]", string.Empty));
        var successfullTestMonkeyId = int.Parse(GetOnlyNumberPart(fileLines[indexOfMonkeyId + 4]));
        var notSuccessfullTestMonkeyId = int.Parse(GetOnlyNumberPart(fileLines[indexOfMonkeyId + 5]));

        monkeys.Add(new Monkey(id: id, testDivisionNumber: testDivisionNumber, successfullTestMonkeyId: successfullTestMonkeyId, notSuccessfullTestMonkeyId: notSuccessfullTestMonkeyId, operation, startingItems: startingItems));
    }

    return monkeys;
}

Func<long, long> GetOperation(string line)
{
    var operationPart = line.Replace(" ", string.Empty).Trim().Split('=')[1].Trim();

    if (operationPart.Contains("+"))
    {
        var elementsOfOperation = operationPart.Split('+');
        var secondOperant = elementsOfOperation[1];
        return secondOperant == "old" ? (x) => (x + x) : (x) => (x + long.Parse(GetOnlyNumberPart(secondOperant)));
    }
    else
    {
        var elementsOfOperation = operationPart.Split('*');
        var secondOperant = elementsOfOperation[1];
        return secondOperant == "old" ? (x) => (x * x) : (x) => (x * long.Parse(GetOnlyNumberPart(secondOperant)));
    }
}

string GetOnlyNumberPart(string line)
{
    return Regex.Replace(line, @"[^\d]", "");
}

class Monkey
{
    public Monkey(int id, long testDivisionNumber, int successfullTestMonkeyId, int notSuccessfullTestMonkeyId, Func<long, long> worryLevelOperation, List<long> startingItems)
    {
        this.Id = id;
        this.TestDivisionNumber = testDivisionNumber;
        this.MonkeyIdForAnItemToBeThrownIfTestIsSuccessfull = successfullTestMonkeyId;
        this.MonkeyIdForAnItemToBeThrownIfTestIsNotSuccessfull = notSuccessfullTestMonkeyId;
        this.WorryLevelOperation = worryLevelOperation;
        this.StolenRucksackItems = startingItems;
    }

    public int Id { get; set; }

    public long TestDivisionNumber { get; set; }

    public int MonkeyIdForAnItemToBeThrownIfTestIsSuccessfull { get; set; }

    public int MonkeyIdForAnItemToBeThrownIfTestIsNotSuccessfull { get; set; }

    public List<long> StolenRucksackItems { get; set; } = new List<long>();

    public long TotalItemsInspected { get; set; } = 0;

    private Func<long, long> WorryLevelOperation;

    public List<ItemTransfer> InspectAndGiveItemsToOtherMokeys(long? factor = null)
    {
        var itemsToGive = new List<ItemTransfer>();

        for (int i = 0; i < StolenRucksackItems.Count; i++)
        {
            var hasMoreItemsToThrow = CalculateWorryLevelAndDestinationMonkeyOfItemHeld(i, out var transfer, factor);

            if (hasMoreItemsToThrow) itemsToGive.Add(transfer);
        }

        StolenRucksackItems.Clear();
        return itemsToGive;
    }

    private bool CalculateWorryLevelAndDestinationMonkeyOfItemHeld(int itemPosition, out ItemTransfer itemTransfer, long? factor = null)
    {
        itemTransfer = new ItemTransfer();

        if (this.StolenRucksackItems != null && this.StolenRucksackItems.Count > 0)
        {
            var itemToInspect = this.StolenRucksackItems[itemPosition];
            itemTransfer.Item = WorryLevelOperation(itemToInspect);
            TotalItemsInspected++;

            if (factor != null)
                itemTransfer.Item %= factor.Value;
            else
                itemTransfer.Item = (long)Math.Floor((decimal)(itemTransfer.Item / 3d));

            itemTransfer.MonkeyIdToGiveItemTo = itemTransfer.Item % TestDivisionNumber == 0 ? MonkeyIdForAnItemToBeThrownIfTestIsSuccessfull : MonkeyIdForAnItemToBeThrownIfTestIsNotSuccessfull;
            
            return true;
        }

        itemTransfer = new ItemTransfer()
        {
            MonkeyIdToGiveItemTo = 0,
            Item = 0
        };

        return false;
    }

    public void CatchThrownItem(long item)
    {
        StolenRucksackItems.Add(item);
    }
}

class ItemTransfer
{
    public int MonkeyIdToGiveItemTo { get; set; }
    public long Item { get; set; }
}
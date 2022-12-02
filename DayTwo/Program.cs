var moveDictionary = new Dictionary<string, Shape>()
{
    { "A", new Shape("Rock", "A", "X", "Z", "Y", 1) },
    { "X", new Shape("Rock", "X", "A", "C", "B", 1) },
    { "B", new Shape("Paper", "B", "Y", "X", "Z", 2) },
    { "Y", new Shape("Paper", "Y", "B", "A", "C", 2) },
    { "C", new Shape("Scissors", "C", "Z", "Y", "X", 3) },
    { "Z", new Shape("Scissors",  "Z", "C","B", "A",  3) },
};

var moveGuideDictionary = new Dictionary<string, RoundOutcome>()
{
    { "X", RoundOutcome.PlayerLose },
    { "Y", RoundOutcome.Draw },
    { "Z", RoundOutcome.PlayerWins },
};

string[] lines = File.ReadAllLines(@"dayTwo.txt");

int totalPlayerScorePartOne = 0;
int totalPlayerScorePartTwo = 0;

foreach (var line in lines)
{
    var moves = line.Split(' ');

    var opponentMoveKey = moves[0];
    var playerMoveKey = moves[1];

    var opponentMoveIsValid = moveDictionary.TryGetValue(opponentMoveKey, out var opponentMove);
    var playerMoveIsValid = moveDictionary.TryGetValue(playerMoveKey, out var playerMove);

    // Part One
    var roundOutcome = GetRoundOutcomeBetweenTwoShapes(opponentMove, playerMove);

    var roundScore = (int)roundOutcome + playerMove.WinScore;
    totalPlayerScorePartOne += roundScore;

    // Part Two
    var desiredOutcomeIsValid = moveGuideDictionary.TryGetValue(playerMoveKey, out var desiredOutcome);

    var playerCalculatedMove = ChooseShapeForOutcome(opponentMove, desiredOutcome);
    var roundCalculatedOutcome = GetRoundOutcomeBetweenTwoShapes(opponentMove, playerCalculatedMove);
    var roundCalculatedScore = (int)roundCalculatedOutcome + playerCalculatedMove.WinScore;
    totalPlayerScorePartTwo += roundCalculatedScore;
}

Console.WriteLine($"Total Player Score Part One: {totalPlayerScorePartOne}");
Console.WriteLine($"Total Player Score Part Two: {totalPlayerScorePartTwo}");
Console.ReadLine();

Shape ChooseShapeForOutcome(Shape opponentShape, RoundOutcome roundOutcome)
{
    switch (roundOutcome)
    {
        case RoundOutcome.PlayerLose:
            return moveDictionary[opponentShape.WinsOver];
        case RoundOutcome.Draw:
            return moveDictionary[opponentShape.OpponentEquivalentSymbol];
        case RoundOutcome.PlayerWins:
            return moveDictionary[opponentShape.LosesFrom];
        default:
            throw new InvalidDataException();
    }
}

RoundOutcome GetRoundOutcomeBetweenTwoShapes(Shape opponent, Shape player)
{
    if (opponent.WinsOver == player.Symbol)
    {
        return RoundOutcome.PlayerLose;
    }
    else if (opponent.LosesFrom == player.Symbol)
    {
        return RoundOutcome.PlayerWins;
    }
    else
    {
        return RoundOutcome.Draw;
    }
}

class Shape
{
    public Shape(string name, string symbol, string opponentEquivalentSymbol, string winsOver, string losesFrom, int winScore)
    {
        Name = name;
        Symbol = symbol;
        OpponentEquivalentSymbol = opponentEquivalentSymbol;
        WinsOver = winsOver;
        LosesFrom = losesFrom;
        WinScore = winScore;
    }

    public string Name { get; }

    public string Symbol { get; }

    public string OpponentEquivalentSymbol { get; }

    public string WinsOver { get; }

    public string LosesFrom { get; }

    public int WinScore { get; }
}

enum RoundOutcome
{
    PlayerLose = 0,
    Draw = 3,
    PlayerWins = 6
}
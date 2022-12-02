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
    { "X", RoundOutcome.PlayerDefeat },
    { "Y", RoundOutcome.Draw },
    { "Z", RoundOutcome.PlayerWin },
};

string[] lines = File.ReadAllLines(@"dayTwo.txt");

int totalPlayerScorePartOne = 0;
int totalPlayerScorePartTwo = 0;

foreach (var line in lines)
{
    var moves = line.Split(' ');

    totalPlayerScorePartOne += CalculateScore(moves[0], moves[1]);
    totalPlayerScorePartTwo += CalculatePredeterminedMoveScore(moves[0], moves[1]);
}

Console.WriteLine($"Score Part One: {totalPlayerScorePartOne} - Score Part Two: {totalPlayerScorePartTwo}");
Console.ReadLine();

int CalculateScore(string opponentMoveKey, string playerMoveKey)
{
    moveDictionary.TryGetValue(opponentMoveKey, out var opponentMove);
    moveDictionary.TryGetValue(playerMoveKey, out var playerMove);
    var roundOutcome = GetRoundOutcomeBetweenTwoShapes(opponentMove, playerMove);

    return (int)roundOutcome + playerMove.WinScore;
}

int CalculatePredeterminedMoveScore(string opponentMoveKey, string playerMoveKey)
{
    moveGuideDictionary.TryGetValue(playerMoveKey, out var desiredOutcome);
    moveDictionary.TryGetValue(opponentMoveKey, out var opponentMove);
    var playerCalculatedMove = ChooseShapeForOutcome(opponentMove, desiredOutcome);
    var roundCalculatedOutcome = GetRoundOutcomeBetweenTwoShapes(opponentMove, playerCalculatedMove);

    return (int)roundCalculatedOutcome + playerCalculatedMove.WinScore;
}

Shape ChooseShapeForOutcome(Shape opponentShape, RoundOutcome roundOutcome)
{
    switch (roundOutcome)
    {
        case RoundOutcome.PlayerDefeat:
            return moveDictionary[opponentShape.WinsOver];
        case RoundOutcome.Draw:
            return moveDictionary[opponentShape.OpponentEquivalentSymbol];
        case RoundOutcome.PlayerWin:
            return moveDictionary[opponentShape.LosesFrom];
        default:
            throw new InvalidDataException();
    }
}

RoundOutcome GetRoundOutcomeBetweenTwoShapes(Shape opponent, Shape player)
{
    if (opponent.WinsOver == player.Symbol)
        return RoundOutcome.PlayerDefeat;
    else if (opponent.LosesFrom == player.Symbol)
        return RoundOutcome.PlayerWin;
    else
        return RoundOutcome.Draw;
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
    PlayerDefeat = 0,
    Draw = 3,
    PlayerWin = 6
}
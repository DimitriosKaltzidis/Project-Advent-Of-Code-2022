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

// Part One Solution
PartOne(lines);

// Part Two solution
PartOne(PartTwo(lines));

Console.ReadLine();

string[] PartTwo(string[] initialLines)
{
    var lines = new List<string>();
    foreach (var line in initialLines)
    {
        var moves = line.Split(' ');

        if (moves.Length == 2)
        {
            var opponentMoveKey = moves[0];
            var opponentMoveIsValid = moveDictionary.TryGetValue(opponentMoveKey, out var opponentMove);

            if (opponentMoveIsValid)
            {
                var playerMoveKey = moves[1];
                var desiredOutcomeIsValid = moveGuideDictionary.TryGetValue(playerMoveKey, out var desiredOutcome);

                if (desiredOutcomeIsValid && opponentMove != null)
                {
                    var playerMove = ChooseShapeForOutcome(opponentMove, desiredOutcome);
                    lines.Add($"{opponentMoveKey} {playerMove.Symbol}");
                }
            }
        }
    }

    return lines.ToArray();
}

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

void PartOne(string[] lines)
{
    int totalPlayerScore = 0;

    foreach (var line in lines)
    {
        var moves = line.Split(' ');

        if (moves.Length == 2)
        {
            var opponentMoveKey = moves[0];
            var playerMoveKey = moves[1];

            var opponentMoveIsValid = moveDictionary.TryGetValue(opponentMoveKey, out var opponentMove);
            var playerMoveIsValid = moveDictionary.TryGetValue(playerMoveKey, out var playerMove);

            if (opponentMoveIsValid && playerMoveIsValid && opponentMove != null && playerMove != null)
            {
                var roundOutcome = GetRoundOutcomeBetweenTwoShapes(opponentMove, playerMove);

                var roundScore = (int)roundOutcome + playerMove.WinScore;
                totalPlayerScore += roundScore;

                Console.WriteLine($"(Opponent) {opponentMove.Name} - {playerMove.Name} (Player) -> {Enum.GetName(typeof(RoundOutcome), roundOutcome)} / + {roundScore} / Current Score: {totalPlayerScore}");
            }
            else
            {
                Console.WriteLine($"Invalid inputs. Opponent: {opponentMoveKey} - Player: {playerMoveKey} ->  ");
            }
        }
    }

    Console.WriteLine($"Total Player Score: {totalPlayerScore}");
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
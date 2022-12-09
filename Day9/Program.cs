var ropeMoves = File.ReadAllLines(@"day9.txt");
var bridge = CalculateBridge(ropeMoves);
PrintBridge(bridge.BridgeMap);
EmulateRopeMovement(ropeMoves, bridge);
Console.ReadLine();


Bridge CalculateBridge(string[] ropeMoves)
{
    var width = 0;
    var maxDistanceToTheRightOfStart = 0;
    var maxDistanceToTheLeftOfStart = 0;
    var height = 0;
    var maxDistanceToTheTopOfStart = 0;
    var maxDistanceToTheBottomOfStart = 0;
    foreach (var move in ropeMoves)
    {
        var moveDirection = move.Split(' ')[0];
        var moveSteps = int.Parse(move.Split(' ')[1]);
        switch (moveDirection)
        {
            case "U":
                height += moveSteps;
                if (height > maxDistanceToTheTopOfStart) maxDistanceToTheTopOfStart = height;
                break;
            case "R":
                width += moveSteps;
                if (width > maxDistanceToTheRightOfStart) maxDistanceToTheRightOfStart = width;
                break;
            case "L":
                width += -moveSteps;
                if (width < maxDistanceToTheLeftOfStart) maxDistanceToTheLeftOfStart = width;
                break;
            case "D":
                height += -moveSteps;
                if (height < maxDistanceToTheBottomOfStart) maxDistanceToTheBottomOfStart = height;
                break;
            default:
                throw new InvalidDataException();
        }
    }

    var rows = maxDistanceToTheTopOfStart + Math.Abs(maxDistanceToTheBottomOfStart) + 1; // 1 = the startposition
    var columns = maxDistanceToTheRightOfStart + Math.Abs(maxDistanceToTheLeftOfStart) + 1; // 1 = the startposition
    var bridge = new Bridge(new char[rows, columns], maxDistanceToTheTopOfStart, Math.Abs(maxDistanceToTheLeftOfStart));
    FillBridge(bridge, '.');
    return bridge;
}

void EmulateRopeMovement(string[] ropeMoves, Bridge bridge)
{
    var headHorizontalPosition = bridge.StartPositionColumn;
    var headVerticalPosition = bridge.StartPositionRow;
    var tailHorizontalPosition = bridge.StartPositionColumn;
    var tailVerticalPosition = bridge.StartPositionRow;

    foreach (var move in ropeMoves)
    {
        var moveDirection = move.Split(' ')[0];
        var moveSteps = int.Parse(move.Split(' ')[1]);

        for (int i = 0; i < moveSteps; i++)
        {
            var headPreviousHorizontalPosition = headHorizontalPosition;
            var headPreviousVerticalPosition = headVerticalPosition;

            MoveHead(ref headHorizontalPosition, ref headVerticalPosition, 1, moveDirection);


            MoveTail(headHorizontalPosition, headVerticalPosition, ref tailHorizontalPosition, ref tailVerticalPosition, headPreviousHorizontalPosition, headPreviousVerticalPosition);
            bridge.BridgeMap[tailVerticalPosition, tailHorizontalPosition] = '#';
        }
    }

    PrintBridge(bridge.BridgeMap);
}

void MoveTail(int headHorizontalPosition, int headVerticalPosition, ref int tailHorizontalPosition, ref int tailVerticalPosition, int headPreviousHorizontalPosition, int headPreviousVerticalPosition)
{
    var distanceBetweenTailAndHead = Math.Abs(tailHorizontalPosition - headHorizontalPosition) + Math.Abs(tailVerticalPosition - headVerticalPosition);

    if (headHorizontalPosition != tailHorizontalPosition && headVerticalPosition != tailVerticalPosition)
    {
        // We are diagonal positioned
        if(distanceBetweenTailAndHead > 2)
        {
            tailHorizontalPosition = headPreviousHorizontalPosition;
            tailVerticalPosition = headPreviousVerticalPosition;
        }
    }
    else
    {
        // We are on the same row or column
        if (distanceBetweenTailAndHead > 1)
        {
            tailHorizontalPosition = headPreviousHorizontalPosition;
            tailVerticalPosition = headPreviousVerticalPosition;
        }
    }
}

void MoveHead(ref int horizontalPosition, ref int verticalPosition, int steps, string direction)
{
    switch (direction)
    {
        case "R":
        case "L":
            horizontalPosition = MoveHorizontally(horizontalPosition, steps, direction);
            break;
        case "U":
        case "D":
            verticalPosition = MoveVertically(verticalPosition, steps, direction);
            break;
        default:
            throw new NotImplementedException();
    }
}

int MoveHorizontally(int start, int steps, string direction)
{
    if (direction == "R") return start + steps;
    else return start - steps;
}

int MoveVertically(int start, int steps, string direction)
{
    if (direction == "U") return start - steps;
    else return start + steps;
}

int PrintBridge(char[,] matrix)
{
    int counterOfTailPositionsVisitedAtLeastOnce = 0;
    Console.Clear();
    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if(matrix[i, j] == '#')
            {
                counterOfTailPositionsVisitedAtLeastOnce++;
            }

            Console.Write(matrix[i, j]);
        }
        Console.WriteLine();
    }
    Console.WriteLine(counterOfTailPositionsVisitedAtLeastOnce);

    return counterOfTailPositionsVisitedAtLeastOnce;
}

void FillBridge(Bridge bridge, char characterToFillBridgeWith)
{
    for (int i = 0; i < bridge.BridgeMap.GetLength(0); i++)
    {
        for (int j = 0; j < bridge.BridgeMap.GetLength(1); j++)
        {
            bridge.BridgeMap[i, j] = characterToFillBridgeWith;
            if (i == (bridge.StartPositionRow) && j == (bridge.StartPositionColumn))
            {
                bridge.BridgeMap[i, j] = 's';
            }
        }

    }
}

record Bridge(char[,] BridgeMap, int StartPositionRow, int StartPositionColumn);
var ropeMoves = File.ReadAllLines(@"day9.txt");
var bridge = CalculateBridge(ropeMoves);
Console.WriteLine($"Part One: {EmulateRopeMovement(ropeMoves, bridge)}");
Console.WriteLine($"Part Two: {EmulateRopeMovement(ropeMoves, bridge, 9)}");

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

int EmulateRopeMovement(string[] ropeMoves, Bridge bridge, int numberOfRopeKnotsAfterHead = 1)
{
    var headHorizontalPosition = bridge.StartPositionColumn;
    var headVerticalPosition = bridge.StartPositionRow;

    var ropeKnots = new List<Position>();

    for (int i = 0; i < numberOfRopeKnotsAfterHead; i++)
    {
        ropeKnots.Add(new Position(bridge.StartPositionColumn, bridge.StartPositionRow));
    }

    foreach (var move in ropeMoves)
    {
        var moveDirection = move.Split(' ')[0];
        var moveSteps = int.Parse(move.Split(' ')[1]);

        for (int i = 0; i < moveSteps; i++)
        {
            MoveHead(ref headHorizontalPosition, ref headVerticalPosition, 1, moveDirection);
            var previousKnotCurrentHorizontalPosition = headHorizontalPosition;
            var previousKnotCurrentVerticalPosition = headVerticalPosition;

            foreach (var ropeKnot in ropeKnots)
            {
                var currentKnotHorizontalPosition = ropeKnot.HorizontalPosition;
                var currentKnotVerticalPosition = ropeKnot.VerticalPosition;
                MoveKnot(previousKnotCurrentHorizontalPosition, previousKnotCurrentVerticalPosition, ref currentKnotHorizontalPosition, ref currentKnotVerticalPosition, bridge);

                ropeKnot.HorizontalPosition = currentKnotHorizontalPosition;
                ropeKnot.VerticalPosition = currentKnotVerticalPosition;
                previousKnotCurrentHorizontalPosition = currentKnotHorizontalPosition;
                previousKnotCurrentVerticalPosition = currentKnotVerticalPosition;
                
                if (ropeKnots.IndexOf(ropeKnot) == ropeKnots.Count -1) 
                {
                    bridge.BridgeMap[ropeKnot.VerticalPosition, ropeKnot.HorizontalPosition] = '#';
                }
            }
        }
    }

    return PrintBridge(bridge.BridgeMap);
}

void MoveKnot(int previousKnotCurrentHorizontalPosition, int previousKnotCurrentVerticalPosition, ref int currentKnotHorizontalPosition, ref int currentKnotVerticalPosition, Bridge bridge)
{
    var distanceBetweenCurrentKnotAndPreviousKnot = Math.Abs(currentKnotHorizontalPosition - previousKnotCurrentHorizontalPosition) + Math.Abs(currentKnotVerticalPosition - previousKnotCurrentVerticalPosition);

    if (previousKnotCurrentHorizontalPosition != currentKnotHorizontalPosition && previousKnotCurrentVerticalPosition != currentKnotVerticalPosition) // We are diagonaly positioned
    {
        if (distanceBetweenCurrentKnotAndPreviousKnot > 2)
        {
            MoveKnotDiagonally(new Position(previousKnotCurrentHorizontalPosition, previousKnotCurrentVerticalPosition), ref currentKnotHorizontalPosition, ref currentKnotVerticalPosition, bridge);
        }
    }
    else // We are on the same row or column
    {   
        if (distanceBetweenCurrentKnotAndPreviousKnot > 1)
        {
            MoveKnotHorizontallyOrVertically(new Position(previousKnotCurrentHorizontalPosition, previousKnotCurrentVerticalPosition), ref currentKnotHorizontalPosition, ref currentKnotVerticalPosition, bridge);
        }
    }
}

void MoveKnotHorizontallyOrVertically(Position previousKnot, ref int currentKnotHorizontalPosition, ref int currentKnotVerticalPosition, Bridge bridge)
{
    var possiblehorizontalAndVerticalPositions = CalculatePossibleHorizontalAndVerticalPositions(currentKnotHorizontalPosition, currentKnotVerticalPosition, bridge.BridgeMap.GetLength(0), bridge.BridgeMap.GetLength(1));
    var distancesPerPosition = new List<Tuple<int, Position>>();

    foreach (var possiblePosition in possiblehorizontalAndVerticalPositions)
    {
        distancesPerPosition.Add(new Tuple<int, Position>(CalculateDistanceBetweenTwoPositions(possiblePosition, previousKnot), possiblePosition));
    }

    var closerToPreviousKnotPosition = distancesPerPosition.MinBy(x => x.Item1).Item2;

    currentKnotHorizontalPosition = closerToPreviousKnotPosition.HorizontalPosition;
    currentKnotVerticalPosition = closerToPreviousKnotPosition.VerticalPosition;
}

void MoveKnotDiagonally(Position previousKnot, ref int currentKnotHorizontalPosition, ref int currentKnotVerticalPosition, Bridge bridge)
{
    var possibleDiagonalPositions = CalculatePossibleDiagonalPositions(currentKnotHorizontalPosition, currentKnotVerticalPosition, bridge.BridgeMap.GetLength(0), bridge.BridgeMap.GetLength(1));
    var distancesPerPosition = new List<Tuple<int, Position>>();

    foreach(var possiblePosition in possibleDiagonalPositions)
    {
        distancesPerPosition.Add(new Tuple<int, Position>(CalculateDistanceBetweenTwoPositions(possiblePosition, previousKnot), possiblePosition));
    }

    var closerToPreviousKnotPosition = distancesPerPosition.MinBy(x=>x.Item1).Item2;

    currentKnotHorizontalPosition = closerToPreviousKnotPosition.HorizontalPosition;
    currentKnotVerticalPosition = closerToPreviousKnotPosition.VerticalPosition;
}

List<Position> CalculatePossibleDiagonalPositions(int currentKnotHorizontalPosition, int currentKnotVerticalPosition, int maxBridgeMapHorizontalPosition, int maxBridgeVerticalPosition)
{
    var possibleDiagonalPositions = new List<Position>();
    var upLeftHorizontalPosition = currentKnotHorizontalPosition - 1;
    var upLeftVerticalPosition = currentKnotVerticalPosition - 1;
    possibleDiagonalPositions.Add(new Position(upLeftHorizontalPosition, upLeftVerticalPosition));

    var upRightHorizontalPosition = currentKnotHorizontalPosition - 1;
    var upRightVerticalPosition = currentKnotVerticalPosition + 1;
    possibleDiagonalPositions.Add(new Position(upRightHorizontalPosition, upRightVerticalPosition));

    var bottomLeftHorizontalPosition = currentKnotHorizontalPosition + 1;
    var bottomLeftVerticalPosition = currentKnotVerticalPosition - 1;
    possibleDiagonalPositions.Add(new Position(bottomLeftHorizontalPosition, bottomLeftVerticalPosition));

    var bottomRightHorizontalPosition = currentKnotHorizontalPosition + 1;
    var bottomRightVerticalPosition = currentKnotVerticalPosition + 1;
    possibleDiagonalPositions.Add(new Position(bottomRightHorizontalPosition, bottomRightVerticalPosition));

    return possibleDiagonalPositions.Where(x => !IsPositionOutsideOfBridge(x, maxBridgeMapHorizontalPosition, maxBridgeVerticalPosition)).ToList();
}

List<Position> CalculatePossibleHorizontalAndVerticalPositions(int currentKnotHorizontalPosition, int currentKnotVerticalPosition, int maxBridgeMapHorizontalPosition, int maxBridgeVerticalPosition)
{
    var possibleVerticalAndHorizontalPositions = new List<Position>();
    var leftHorizontalPosition = currentKnotHorizontalPosition;
    var leftVerticalPosition = currentKnotVerticalPosition - 1;
    possibleVerticalAndHorizontalPositions.Add(new Position(leftHorizontalPosition, leftVerticalPosition));

    var upHorizontalPosition = currentKnotHorizontalPosition - 1;
    var upVerticalPosition = currentKnotVerticalPosition;
    possibleVerticalAndHorizontalPositions.Add(new Position(upHorizontalPosition, upVerticalPosition));

    var bottomHorizontalPosition = currentKnotHorizontalPosition + 1;
    var bottomVerticalPosition = currentKnotVerticalPosition;
    possibleVerticalAndHorizontalPositions.Add(new Position(bottomHorizontalPosition, bottomVerticalPosition));

    var rightHorizontalPosition = currentKnotHorizontalPosition;
    var rightVerticalPosition = currentKnotVerticalPosition + 1;
    possibleVerticalAndHorizontalPositions.Add(new Position(rightHorizontalPosition, rightVerticalPosition));

    return possibleVerticalAndHorizontalPositions.Where(x => !IsPositionOutsideOfBridge(x, maxBridgeMapHorizontalPosition, maxBridgeVerticalPosition)).ToList();
}

bool IsPositionOutsideOfBridge(Position position, int maxBridgeMapHorizontalPosition, int maxBridgeVerticalPosition)
{
    if(position.VerticalPosition < 0 || position.VerticalPosition > maxBridgeMapHorizontalPosition)
    {
        return true;
    }

    if(position.HorizontalPosition < 0 || position.HorizontalPosition > maxBridgeVerticalPosition)
    {
        return true;
    }

    return false;
}

int CalculateDistanceBetweenTwoPositions(Position positionA, Position positionB)
{
    return Math.Abs(positionA.HorizontalPosition - positionB.HorizontalPosition) + Math.Abs(positionA.VerticalPosition - positionB.VerticalPosition);
}

void MoveHead(ref int horizontalPosition, ref int verticalPosition, int steps, string direction)
{
    switch (direction)
    {
        case "R":
        case "L":
            horizontalPosition = MoveHeadHorizontally(horizontalPosition, steps, direction);
            break;
        case "U":
        case "D":
            verticalPosition = MoveHeadVertically(verticalPosition, steps, direction);
            break;
        default:
            throw new NotImplementedException();
    }
}

int MoveHeadHorizontally(int start, int steps, string direction)
{
    if (direction == "R") return start + steps;
    else return start - steps;
}

int MoveHeadVertically(int start, int steps, string direction)
{
    if (direction == "U") return start - steps;
    else return start + steps;
}

int PrintBridge(char[,] matrix)
{
    int counterOfTailPositionsVisitedAtLeastOnce = 0;

    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if (matrix[i, j] == '#')
            {
                counterOfTailPositionsVisitedAtLeastOnce++;
            }

            Console.Write(matrix[i, j]);
        }
        Console.WriteLine();
    }

    FillBridge(bridge, '.');

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

class Position
{

    public Position(int horizontalPosition, int verticalPosition)
    {
        HorizontalPosition = horizontalPosition;
        VerticalPosition = verticalPosition;
    }

    public int HorizontalPosition { get; set; }

    public int VerticalPosition { get; set; }
}
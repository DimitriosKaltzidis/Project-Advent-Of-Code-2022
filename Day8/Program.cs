var forestRawData = File.ReadAllLines(@"day8.txt");
var forestMap = CreateForestMap(forestRawData);
var forestProperties = CalculateForestProperties(forestMap);
Console.WriteLine($"Part One: {forestProperties.Item1} - Part Two: {forestProperties.Item2}");

int[,] CreateForestMap(string[] rawData)
{
    var forestMap = new int[rawData.Length, rawData[0].Length];
    for (int i = 0; i < forestMap.GetLength(0); ++i)
    {
        var line = rawData[i];
        for (int j = 0; j < line.Length; ++j)
        {
            forestMap[i, j] = int.Parse(line[j].ToString());
        }
    }

    return forestMap;
}

Tuple<int, int> CalculateForestProperties(int[,] treeMap)
{
    var visibleTreesNumber = 0;
    var maxScenicScore = 0;
    for (int i = 0; i < treeMap.GetLength(0); i++)
    {
        for (int j = 0; j < treeMap.GetLength(1); j++)
        {
            var verticalLineOfTrees = GetColumn(treeMap, j);
            var horizontalLineOfTrees = GetRow(treeMap, i);
            var scenicScore = GetTreeScenicScore(treeMap[i, j], j, i, verticalLineOfTrees.ToList(), horizontalLineOfTrees.ToList());
            if (scenicScore > maxScenicScore) maxScenicScore = scenicScore;

            if (i == 0 || j == 0 || i == (treeMap.GetLength(0) - 1) || j == (treeMap.GetLength(1) - 1))
            {
                visibleTreesNumber++;
                continue;
            }

            if (IsTreeVisibleFromAnyDirection(treeMap[i, j], j, i, verticalLineOfTrees.ToList(), horizontalLineOfTrees.ToList())) visibleTreesNumber++;
        }

    }

    return new Tuple<int, int>(visibleTreesNumber, maxScenicScore);
}

bool IsTreeVisibleFromAnyDirection(int treeHeight, int verticalIndex, int horizontalIndex, List<int> verticalLineOfTrees, List<int> horizontalLineOfTrees)
{
    var topVisible = IsVisible(treeHeight, verticalLineOfTrees.Take(horizontalIndex).ToList());
    var leftVisible = IsVisible(treeHeight, horizontalLineOfTrees.Take(verticalIndex).ToList());
    var bottomVisible = IsVisible(treeHeight, verticalLineOfTrees.Skip(horizontalIndex + 1).Take(verticalLineOfTrees.Count - (horizontalIndex + 1)).ToList());
    var rightVisible = IsVisible(treeHeight, horizontalLineOfTrees.Skip(verticalIndex + 1).Take(horizontalLineOfTrees.Count - (verticalIndex + 1)).ToList());
    
    return leftVisible || rightVisible || topVisible || bottomVisible;
}

int GetTreeScenicScore(int treeHeight, int verticalIndex, int horizontalIndex, List<int> verticalLineOfTrees, List<int> horizontalLineOfTrees)
{
    var topScore = CalculateDistanceFromForestEdgeOrTallerTree(treeHeight, verticalLineOfTrees.Take(horizontalIndex).Reverse().ToList());
    var leftScore = CalculateDistanceFromForestEdgeOrTallerTree(treeHeight, horizontalLineOfTrees.Take(verticalIndex).Reverse().ToList());
    var bottomScore = CalculateDistanceFromForestEdgeOrTallerTree(treeHeight, verticalLineOfTrees.Skip(horizontalIndex + 1).Take(verticalLineOfTrees.Count - (horizontalIndex + 1)).ToList());
    var rightScore = CalculateDistanceFromForestEdgeOrTallerTree(treeHeight, horizontalLineOfTrees.Skip(verticalIndex + 1).Take(horizontalLineOfTrees.Count - (verticalIndex + 1)).ToList());

    return leftScore * rightScore * topScore * bottomScore;
}

bool IsVisible(int tree, List<int> treesInADirection)
{
    return !treesInADirection.Any(x => x >= tree);
}

int CalculateDistanceFromForestEdgeOrTallerTree(int tree, List<int> treesInADirection)
{
    var viewDistance = 0;
    foreach (var distantTree in treesInADirection)
    {
        viewDistance++;
        if (distantTree >= tree) break;
    }

    return viewDistance;
}

int[] GetColumn(int[,] matrix, int columnNumber)
{
    return Enumerable.Range(0, matrix.GetLength(0))
            .Select(x => matrix[x, columnNumber])
            .ToArray();
}

int[] GetRow(int[,] matrix, int rowNumber)
{
    return Enumerable.Range(0, matrix.GetLength(1))
            .Select(x => matrix[rowNumber, x])
            .ToArray();
}
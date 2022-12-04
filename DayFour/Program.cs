﻿string[] pairsOfElves = File.ReadAllLines(@"dayfour.txt");
var fullyOverRangeCounter = 0;
var overlapRangeCounter = 0;

foreach (var pairOfElves in pairsOfElves)
{
    var elvesInPair = pairOfElves.Split(',');
    var elfOneRangeLimits = elvesInPair[0].Split('-');
    var elfTwoRangeLimits = elvesInPair[1].Split('-');
    var elfOneRange = new Range(int.Parse(elfOneRangeLimits[0]), int.Parse(elfOneRangeLimits[1]));
    var elfTwoRange = new Range(int.Parse(elfTwoRangeLimits[0]), int.Parse(elfTwoRangeLimits[1]));
    if (DoRangesOverlap(elfOneRange, elfTwoRange)) overlapRangeCounter++;
    if (DoRangesFullyOverlap(elfOneRange, elfTwoRange)) fullyOverRangeCounter++;
}

Console.WriteLine($"Part One: {fullyOverRangeCounter} - Part Two: {overlapRangeCounter}");
Console.ReadLine();

bool DoRangesOverlap(Range firstRange, Range secondRange)
{
    return firstRange.Start <= secondRange.End && secondRange.Start <= firstRange.End;
}

bool DoRangesFullyOverlap(Range firstRange, Range secondRange)
{
     return secondRange.Start >= firstRange.Start && secondRange.End <= firstRange.End || firstRange.Start >= secondRange.Start && firstRange.End <= secondRange.End;
}

record Range(int Start, int End);
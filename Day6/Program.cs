var datastreamBuffer = File.ReadAllText(@"day6.txt");
Console.WriteLine($"Part One: {DetectSignalEventPosition(datastreamBuffer, 4)} - Part Two: {DetectSignalEventPosition(datastreamBuffer, 14)}");

int DetectSignalEventPosition(string signalDatastreamBuffer, int windowSize)
{
    for (int i = 0; i < signalDatastreamBuffer.Length; i++)
    {
        if (i + (windowSize - 1) > signalDatastreamBuffer.Length) throw new InvalidDataException();
        var window = new HashSet<char>();
        for (int j = i; j < (i + windowSize); j++) window.Add(signalDatastreamBuffer[j]);
        if (window.Count == windowSize) return (i + windowSize);
    }

    throw new InvalidDataException();
}
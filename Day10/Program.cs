var tasks = File.ReadAllLines(@"day10.txt").Select(x => new Task() { Command = x, Status = TaskStatus.New }).ToList();
var clock = new PeriodicTimer(TimeSpan.FromMilliseconds(1));
var registerXValue = 1;
var cycles = 1;
var sumOfSignalStrengths = 0;
var cyclesToCaclulateSignalStrength = new List<int>() { 20, 60, 100, 140, 180, 220 };
var monitorOutput = new List<char>();

Console.WriteLine("Calculating...\n");

while (await clock.WaitForNextTickAsync())
{
    if (cyclesToCaclulateSignalStrength.Contains(cycles))
    {
        var currentSignalStrength = cycles * registerXValue;
        sumOfSignalStrengths += currentSignalStrength;
    }

    var taskToExecute = tasks.FirstOrDefault(x => x.Status == TaskStatus.Executing);
    if (taskToExecute == null) taskToExecute = tasks.FirstOrDefault(x => x.Status == TaskStatus.New);

    if (taskToExecute == null)
    {
        Console.WriteLine($"PartOne: {sumOfSignalStrengths}\n\nPart Two:");

        clock.Dispose();
        PrintMonitor(monitorOutput, 40);
        
        Console.ReadLine();
        break;
    }

    var spriteRow = CalculateSprite(registerXValue);
    monitorOutput.Add(spriteRow[(cycles - 1) % 40]);

    taskToExecute.Status = ExecuteTask(taskToExecute);
    cycles++;
}

void PrintMonitor(List<char> monitorOutput, int lineLength)
{
    var lines = Split<char>(monitorOutput, lineLength);

    foreach(var line in lines)
    {
        Console.WriteLine(String.Join(string.Empty, line));
    }
}

char[] CalculateSprite(int registerXValue, int size = 40)
{
    var spriteLine = Enumerable.Repeat('.', size).ToArray();
    var spitePositions = new List<int> { registerXValue - 1, registerXValue, registerXValue + 1 };
    var positionsToRender = spitePositions.Where(x => !IsPositionOutOfBounds(x, spriteLine.Length));
    
    foreach (var spitePosition in positionsToRender)
    {
        spriteLine[spitePosition] = '#';
    }

    return spriteLine;
}

bool IsPositionOutOfBounds(int position, int arrayMaxPosition)
{
    return position < 0 || position >= arrayMaxPosition;
}

TaskStatus ExecuteTask(Task taskToExecute)
{
    var instruction = taskToExecute.Command.Split(' ')[0];

    switch (instruction)
    {
        case "noop":
            return TaskStatus.Completed;
        default:
            {
                if (taskToExecute.Status == TaskStatus.Executing)
                {
                    registerXValue += int.Parse(taskToExecute.Command.Split(' ')[1]);
                    return TaskStatus.Completed;
                }
                else
                {
                    return TaskStatus.Executing;
                }
            }
    }
}

 IEnumerable<IEnumerable<T>> Split<T>(List<T> arr, int size)
{
    return arr.Select((s, i) => arr.Skip(i * size).Take(size)).Where(a => a.Any());
}

class Task
{
    public string Command { get; set; }

    public TaskStatus Status { get; set; }
}

enum TaskStatus
{
    New = 0,
    Executing,
    Completed
}
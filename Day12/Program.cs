var lines = File.ReadLines("day12.txt").ToList();
Location[][] ToTerrain(IEnumerable<string> list) => list.Select(l => l.Select(x => new Location(x)).ToArray()).ToArray();
var path = Navigate(ToTerrain(lines), 'S', 'E', (f, t) => f - t >= -1);
var path2 = Navigate(ToTerrain(lines), 'E', 'a', (f, t) => f - t <= 1);
Console.WriteLine($"Part One: {path.Seen().Count()} - Part Two: {path2.Seen().Count()}");
Console.ReadLine();

bool Traversable(Location[][] terrain, WayPoint from, WayPoint to, Func<char, char, bool> rule)
{
    char NoMarker(char c) => c switch { 'S' => 'a', 'E' => 'z', _ => c };
    if (to.X < 0 || to.X >= terrain[0].Length || to.Y < 0 || to.Y >= terrain.Length || terrain[from.Y][from.X].Visited) return false;
    return rule(
        NoMarker(terrain[from.Y][from.X].Height),
        NoMarker(terrain[to.Y][to.X].Height)
    );
}
WayPoint Navigate(Location[][] terrain, char beginAt, char End, Func<char, char, bool> rule)
{
    var start = terrain.SelectMany((l, y) => l.Select((h, x) => new WayPoint(h.Height == beginAt ? x : -1, y))).First(p => p.X > -1);
    Queue<WayPoint> queue = new(new[] { start });
    int i = 0;
    while (queue.Any())
    {
        var p = queue.Dequeue();
        if (terrain[p.Y][p.X].Height == End) return p;
        new[] { p.Up, p.Down, p.Left, p.Right }.Where(n => Traversable(terrain, p, n, rule)).ToList().ForEach(n =>
        {
            n.Last = p;
            queue.Enqueue(n);
        });
        terrain[p.Y][p.X].Visited = true;
        i++;
    }
    return null;
}

record Location(char Height)
{
    public bool Visited { get; set; } = false;
}

record WayPoint(int X, int Y)
{
    public WayPoint? Last { get; set; } = null;
    public WayPoint Up => new(X, Y - 1);
    public WayPoint Down => new(X, Y + 1);
    public WayPoint Left => new(X - 1, Y);
    public WayPoint Right => new(X + 1, Y);
    public IEnumerable<WayPoint> Seen()
    {
        var p = Last;
        while (p != null)
        {
            yield return p;
            p = p.Last;
        }
    }
}
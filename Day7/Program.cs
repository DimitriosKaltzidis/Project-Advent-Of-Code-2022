var listOfCandidateDirectoriesForDeletion = new List<int>();
int maxWantedDirecotrySpace = 100000;

var terminalOutput = File.ReadAllLines(@"day7.txt");
var root = new TreeNode()
{
    Name = "/",
    Type = TreeBranchType.Directory,
    Depth = 0,
};

var currentNode = root;

int currentTreeDepth = 1;

for (var i = 1; i < terminalOutput.Length; i++)
{
    var terminalLine = terminalOutput[i];

    if (terminalLine.StartsWith("$"))
    {
        HandleCommand(terminalLine);
    }
    else
    {
        HandleNode(terminalLine);
    }
}
root.Traverse();
Console.WriteLine($"Part One: {CalculateGoodCandidatesForDeletion()}");
Console.WriteLine($"Part Two: {CalculateDirecotryToDeleteForUpdate()}");


Console.ReadLine();

void HandleNode(string line)
{
    var lineProperties = line.Split(" ");

    switch (lineProperties[0])
    {
        case "dir":
            currentNode.AddChild(lineProperties[1], TreeBranchType.Directory, currentTreeDepth);
            break;
        default:
            currentNode.AddChild(lineProperties[1], TreeBranchType.File, currentTreeDepth, int.Parse(lineProperties[0]));
            break;
    }
}

void HandleCommand(string command)
{
    var commandAndParameters = command.Replace("$ ", string.Empty).Split(" ");

    if (commandAndParameters[0] == "cd")
        if (commandAndParameters[1] == "..")
        {
            currentTreeDepth--;
            currentNode = currentNode.Parent;
        }
        else
        {
            currentTreeDepth++;
            currentNode = currentNode.Children.First(x => x.Name == commandAndParameters[1]);
        }

}

int CalculateDirecotryToDeleteForUpdate()
{
    listOfCandidateDirectoriesForDeletion.Clear();
    var totalDeviceSpace = 70000000;
    var spaceNeededForTheUpdate = 30000000;

    var availableSpace = totalDeviceSpace - root.Size;
    var neededSpace = spaceNeededForTheUpdate - availableSpace;
    maxWantedDirecotrySpace = neededSpace;
    FindOfCandidateDirectories(root, true);

    return listOfCandidateDirectoriesForDeletion.Min();
}


int CalculateGoodCandidatesForDeletion()
{
    listOfCandidateDirectoriesForDeletion.Clear();
    maxWantedDirecotrySpace = 100000;
    FindOfCandidateDirectories(root);

    return listOfCandidateDirectoriesForDeletion.Sum(d => d);
}

void FindOfCandidateDirectories(TreeNode treeNode, bool calculateForUpdate = false)
{
    if (treeNode.Size <= maxWantedDirecotrySpace && !calculateForUpdate  && treeNode.Type == TreeBranchType.Directory)
    {
        listOfCandidateDirectoriesForDeletion.Add(treeNode.Size);
    }
    else if (treeNode.Size >= maxWantedDirecotrySpace && calculateForUpdate && treeNode.Type == TreeBranchType.Directory)
    {
        listOfCandidateDirectoriesForDeletion.Add(treeNode.Size);
    }

    foreach (var child in treeNode.Children)
        FindOfCandidateDirectories(child, calculateForUpdate);
}

class TreeNode
{
    public string Name { get; set; }
    public TreeBranchType Type { get; set; }
    public int Depth { get; set; }
    public int Size { get; set; }
    public TreeNode Parent { get; set; }
    public List<TreeNode> Children { get; set; } = new List<TreeNode>();

    public TreeNode AddChild(string name, TreeBranchType type, int depth, int size = 0)
    {
        var node = new TreeNode() { Parent = this, Name = name, Type = type, Size = size, Depth = depth };
        Children.Add(node);
        UpdateParentSize(this, size);
        return node;
    }

    private void UpdateParentSize(TreeNode dad, int size)
    {
        if (dad != null)
        {
            dad.Size += size;

            if (dad.Parent != null)
            {
                UpdateParentSize(dad.Parent, size);
            }
        }
    }

    public void Traverse()
    {
        Console.WriteLine($"{new String('\t', Depth)} - {Name} ({Enum.GetName(typeof(TreeBranchType), Type)}) (Size={Size})");
        foreach (var child in Children)
            child.Traverse();
    }
}

enum TreeBranchType
{
    Directory,
    File
}
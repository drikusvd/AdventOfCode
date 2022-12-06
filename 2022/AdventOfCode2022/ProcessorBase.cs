namespace AdventOfCode2022;

public abstract class ProcessorBase
{
    public async Task Process()
    {
        var type = GetType();
        var folder = type.Namespace.Substring(type.Namespace.LastIndexOf(".") + 1);
        var lines = await File.ReadAllLinesAsync($"{folder}\\Input.txt");
        Console.WriteLine(folder);
        Console.WriteLine("".PadLeft(folder.Length, '-'));
        Silver(lines);
        Gold(lines);
        Console.WriteLine();
    }

    protected abstract void Silver(string[] lines);

    protected abstract void Gold(string[] lines);
}
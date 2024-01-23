namespace CliTool.Names
{
    public interface IConcatedNamesProcessor
    {
        string Separator { get; }
        IReadOnlyCollection<string> Process(string fileNames);
    }
}
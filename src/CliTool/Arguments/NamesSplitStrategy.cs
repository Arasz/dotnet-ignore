namespace CliTool.Arguments;

public static class NamesSplitStrategy
{
    public const string SplitSeparator = ",";

    public static string[] Split(string names) => names.Split(SplitSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
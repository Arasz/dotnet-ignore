using CliTool.Github.Services;

namespace CliTool.Commands;

public sealed class ListGitIgnoreFilesCommand(IGitignoreService gitignoreService)
{
    public async Task ListGitIgnoreFiles(bool shorten)
    {
        var gitignoreFilesNames = await gitignoreService.GetAllIgnoreFilesNames();

        if (shorten)
        {
            gitignoreFilesNames = gitignoreFilesNames
                .Select(fileName => fileName.Replace(".gitignore", ""))
                .ToList();
        }

        foreach (var gitignoreFileName in gitignoreFilesNames)
        {
            Console.WriteLine($"- {gitignoreFileName}");
        }
    }
}
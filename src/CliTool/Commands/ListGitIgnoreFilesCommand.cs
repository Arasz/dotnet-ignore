using System.Collections.Immutable;
using CliTool.Github.Services;

namespace CliTool.Commands;

public sealed class ListGitIgnoreFilesCommand(IGitignoreService gitignoreService)
{
    public async Task ListGitIgnoreFiles(bool fullNames)
    {
        var gitignoreFilesNames = await gitignoreService.GetAllIgnoreFilesNames();

        if (!fullNames)
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
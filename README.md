# dotnet-ignore
[![build](https://github.com/Arasz/dotnet-ignore/actions/workflows/build.yml/badge.svg)](https://github.com/Arasz/dotnet-ignore/actions/workflows/build.yml)

Global .NET tool that downloads `.gitignore` files from the
[github/gitignore](https://github.com/github/gitignore) repository.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Installation

```
dotnet tool install -g dotnet-ignore
```

## Usage

Download and merge `.gitignore` files by name (case insensitive, with or without
the `.gitignore` suffix; multiple names are merged into one file):

```
dotnet ignore get VisualStudio,Node
```

Remove comments, duplicate and empty lines to minimize the file size:

```
dotnet ignore get VisualStudio,Node -m
```

Save the file into a specific directory:

```
dotnet ignore get VisualStudio -d ./path/to/dir
```

List all available files:

```
dotnet ignore list
```

List only file names without the `.gitignore` suffix:

```
dotnet ignore list -s
```

Run `dotnet ignore -h` for the complete command description.

## Releasing

Releases are driven by a `latest` label on a pull request and a manual approval:

1. Prepare the release PR: bump `<Version>` in `src/CliTool/CliTool.csproj` and add a
   [CHANGELOG.md](CHANGELOG.md) entry.
2. Comment `/latest` on the PR (or add the `latest` label from the PR page).
3. Merge the PR to `master`. The `publish` workflow starts and **waits for approval**.
4. Approve the run: Actions -> the `publish` run -> Review -> Approve. The package is
   pushed to NuGet via [Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
   (no API keys).

The `latest` label is removed and a release comment is posted on the PR after a
successful push. The workflow can also be started manually (Actions -> publish ->
Run workflow), which is subject to the same approval.

> The approval gate is a GitHub environment (`publish`) with a required reviewer.
> Configure it once: Settings -> Environments -> `publish` -> Required reviewers ->
> add your account. Until then the workflow runs without waiting.

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License

[Apache-2.0](LICENSE)

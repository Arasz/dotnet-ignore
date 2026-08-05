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

Every merge to `master` triggers the `publish` workflow, which waits for approval
before pushing to NuGet:

1. Prepare the release PR: bump `<Version>` in `src/CliTool/CliTool.csproj` and add a
   [CHANGELOG.md](CHANGELOG.md) entry.
2. Merge the PR to `master`. The `publish` workflow starts and **waits for approval**.
3. Approve the run: Actions -> the `publish` run -> Review deployments -> Approve and
   deploy. The package is pushed to NuGet via
   [Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
   (no API keys).

The workflow can also be started manually (Actions -> publish -> Run workflow),
subject to the same approval. Every push to `master` creates a pending run; the
approval step is the release control.

> The approval gate is the `production` environment, which already lists you as a
> required reviewer and allows `master` as the only deployment source — every
> publish run pauses for your approval (Review deployments -> Approve and deploy).
> The NuGet Trusted Publishing policy is bound to the same environment name, so
> the OIDC token exchange only succeeds when the job runs in it. Manage the gate
> at Settings -> Environments -> `production`.

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License

[Apache-2.0](LICENSE)

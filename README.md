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

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License

[Apache-2.0](LICENSE)

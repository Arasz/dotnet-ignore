# dotnet-ignore
.NET CLI tool that can download .gitignore file from gitignore repository.

## How to get it?
From [nuget](https://www.nuget.org/packages/dotnet-ignore/1.0.0)

## Installation

### Global

Execute 
```
dotnet install tool -g <PACKAGE_ID>
```

### Local

Add to your .csproj file 

```
<ItemGroup>
	<DotNetCliToolReference Include="DotNetIgnore" Version="*" />
</ItemGroup>
```

## How to run it? 

Just run

```
dotnet ignore
```

to see complete description of usage.

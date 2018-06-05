# dotnet-ignore
.NET CLI tool that can download .gitignore file from gitignore repository.

## How to get it?
From [nuget](https://www.nuget.org/packages/dotnet-ignore/1.0.0)

## Installation

### Global

Execute 
```
dotnet tool install -g dotnet-ignore
```

### Local

Add to your .csproj file 

```
<ItemGroup>
	<DotNetCliToolReference Include="dotnet-ignore" Version="*" />
</ItemGroup>
```

## How to run it? 

Just run

```
dotnet ignore -h
```

to see complete description of usage.

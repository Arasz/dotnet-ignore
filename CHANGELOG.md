# Changelog

All notable changes to this project are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [1.1.0] - 2026-08-05

### Added
- GitHub Actions CI/CD replacing Azure DevOps: `build.yml` (build, test, coverage) and
  `publish.yml` (NuGet push via Trusted Publishing, no API keys)

### Changed
- Migrated to .NET 10 (`net10.0`), updated all NuGet packages:
  Octokit 9.1.0 -> 14.0.0, CommunityToolkit.Diagnostics -> 8.4.2, Moq -> 4.20.72,
  FluentAssertions 6.12.0 -> 8.10.0, xunit 2.6.6 -> xunit v3 3.2.2,
  Microsoft.NET.Test.Sdk 17.8.0 -> 18.8.1
- Merge header uses a deterministic UTC timestamp (`yyyy-MM-ddTHH:mm:ssZ`)
- Errors print a single clean message to stderr and exit with code 1 (was a full
  stack trace)
- GitHub repository listing is fetched once per run instead of once per requested
  name (avoids the unauthenticated API rate limit)
- `GitignoreFileWriter` uses `File.WriteAllTextAsync`

### Fixed
- `list -s` printed names *with* the `.gitignore` suffix while plain `list` printed
  them *without*; the flag now follows its documented contract
- File-write failures are no longer mislabeled as `ArgumentException` about an
  invalid destination path
- `NamesSplitStrategy.Split` throws `ArgumentNullException` instead of
  `NullReferenceException`
- Typo in the minimize-mode header: "where removed" -> "were removed"

### Removed
- Live-GitHub-API integration tests (replaced with mocked unit tests, coverage
  30% -> 78% line / 97% branch)
- Obsolete `DotnetCliTool` package type and `dotnet-xunit` tool reference
- Dead test harness (`UnitTestBase`, AutoFixture)

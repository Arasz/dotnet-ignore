# Code Review — dotnet-ignore (pre-refactor baseline)

Date: 2026-08-05
Branch: feature/dotnet10-upgrade (net10.0 migration already merged into this branch)
Reviewers: Hermes agent (glider MCP semantic analysis + manual reading + live smoke tests), QA subagent (code coverage)

## Scope

Full review of `src/CliTool` and `test/DotnetIgnoreCliToolTests` after the .NET 10 /
package-upgrade migration. Focus: structure and optimization. Every finding lists
severity / location / evidence / fix.

## Coverage baseline (QA subagent, coverlet 10.0.1)

- Overall line coverage: 30.23% (52/172), branch coverage: 19.44% (7/36)
- Program.cs (top-level statements): 0% line coverage
- SimpleMergeStrategy: 37.14% line; minimize-mode branch (lines 45-47) 0% — **uncovered**
- GithubGitignoreService: no unit tests at all; only 4 live-API integration tests exist
- ListGitIgnoreFilesCommand: untested
- GetGitIgnoreFileCommand default-destination branch: untested

## Findings

### Bugs (High)

1. **`list -s` flag is inverted** — Program.cs:34-36 passes the `shorten` option
   directly as `ListGitIgnoreFiles(bool fullNames)`; the command strips the
   `.gitignore` suffix when `fullNames == false`. Result: `-s` prints names *with*
   the suffix (contradicting its help text "Prints files names without .gitignore
   part") and the default prints *without*. Evidence: live smoke test
   (`list -s` -> `AL.gitignore`, `list` -> `AL`). Fix: rename the parameter to
   `shorten` and invert the branch; add unit tests.

2. **No global error handling — raw stack traces** — `get NonExistingFile` prints
   `Unhandled Exception: System.ArgumentException: ...` plus full stack. Any network
   failure does the same. Cocona does not intercept exceptions escaping the command
   handler. Fix: try/catch around `consoleApp.Run()` in Program.cs: friendly
   `Error: <message>` on stderr, exit code 1.

3. **Write errors mislabeled and mis-wrapped** — GetGitIgnoreFileCommand.cs:19-32
   wraps *only* the file write in a catch-all that converts every failure
   (IOException, UnauthorizedAccessException, ...) into
   `ArgumentException("Destination path {destination} is invalid or there were
   problem with file access")`. Wrong exception type, misleading message, real cause
   only in `InnerException`. Network errors (outside the try) escape raw. Fix:
   remove the wrapper; let I/O exceptions propagate to the global handler.

4. **API inefficiency / rate-limit risk** — GithubGitignoreService.GetIgnoreFile
   (GithubGitignoreService.cs:39-75) calls `GetAllContents` (one API call) *and* a
   content download per name; the command fires N of these in parallel. GitHub's
   unauthenticated API limit is 60 requests/hour, so `get` with ~30 names exhausts
   the budget and the tool breaks. Fix: cache the repository listing per service
   instance (1 call per run), resolve all names against it, download via
   `DownloadUrl` (raw.githubusercontent.com).

### Medium

5. **Zero unit tests for GithubGitignoreService** — the name-matching logic
   (case-insensitive, with/without `.gitignore` suffix) is untested. The 4 tests in
   GithubClientTests.cs hit the live GitHub API: network-dependent, flaky in CI,
   test Octokit rather than this tool, and trigger xUnit1051 (no cancellation
   token). Fix: unit tests with mocked `IGitHubClient` + mocked
   `HttpMessageHandler`; delete the live-API tests.

6. **HttpClient created per instance, never disposed** — GithubGitignoreService
   constructs `new HttpClient()` in both ctors. Not disposed, not from
   `IHttpClientFactory`. Fix: inject HttpClient (DI-registered), single ctor.

7. **Ambiguous DI shape** — two public ctors (IGitHubClient + parameterless);
   `IGitHubClient` is not registered in Program.cs, so Microsoft DI silently picks
   the parameterless ctor and the other is dead code in the DI path. Fix: one ctor,
   register `IGitHubClient` as singleton.

8. **Non-deterministic merge header** — SimpleMergeStrategy.cs:20 uses
   `DateTime.Now` with culture-sensitive format `g` (e.g. `05/08/2026 11:17`), so
   output depends on locale/timezone and is unreproducible. Fix:
   `DateTimeOffset.UtcNow.ToString("u", CultureInfo.InvariantCulture)`.

9. **`GitignoreFile.Empty` sentinel conflates "not found" with "empty content"** —
   a genuinely empty file would be reported as a bad name. Fix: make
   `GetIgnoreFile` return `GitignoreFile?` (null = not found); update command and
   tests; remove the sentinel.

10. **Minimize mode has no tests** — `MergeWithFileSizeMinimalization`
    (lines 41-61) is 0% covered on its core branches. Add characterization tests:
    comments/blank lines removed, duplicates deduped, headers preserved.

11. **Coverage gaps** (see baseline): Program.cs 0%, ListGitIgnoreFilesCommand
    untested, default-destination branch untested. Target after refactor: >= 70%
    line coverage on src/CliTool.

12. **Warnings** — CS8019 (unused `using System.Collections.Immutable`,
    ListGitIgnoreFilesCommand.cs:1), CA1866 (`StartsWith(char)`,
    SimpleMergeStrategyTests.cs:88), CA1816 (`GC.SuppressFinalize`,
    UnitTestBase.cs:10), xUnit1051 x2 (GithubClientTests.cs:61-64).

### Low (hygiene)

13. **Dead code** — UnitTestBase (AutoFixture `Fixture` never used; no-op
    `IDisposable`) and the AutoFixture package only exist for it. Remove both;
    tests become plain classes.

14. **Accidental null behavior** — NamesSplitStrategy.Split(null) throws
    `NullReferenceException`; the test *asserts* the NRE (ConcatedNamesProcessor
    Tests.cs:44-56). Fix: `ArgumentNullException.ThrowIfNull`, update the test.

15. **Typos** — folder `FIles` (should be `Files`), method
    `EnsureAllGitIgnoreFilesWhereDownloaded` ("Were"), minimize header "where
    removed" ("were"), write-error message "there were problem" (grammar).

16. **GeneratePackageOnBuild=True** packs a nupkg on every Debug build (test
    iterations pay for packing). Fix: condition on `'$(Configuration)'=='Release'`.

17. **MergeStrategyStub.Aggregate** (test infra) crashes on empty input (no seed).
    Use an overload with seed.

## What was checked and found clean

- No hardcoded secrets; no tracked credentials.
- No `#pragma warning disable` suppressions anywhere.
- Project reference graph is minimal and correct (glider: 0 unused references).
- No unused private symbols in production code (glider: only expected entry points).
- Package versions are consistent across projects (Octokit 14.0.0 both).
- Exceptions are not swallowed anywhere in production code (the catch-all in
  finding 3 is the one mislabel, not a swallow).
- Clean layering for a single-project CLI: Github/ is the adapters, Merge/ and
  Commands/ are orchestration, no persistence layer to police.

## Unreviewed / out of scope

- Azure DevOps pipeline (external, not in repo) — GeneratePackageOnBuild change
  (finding 16) keeps Release packing so the "Build and push" pipeline is unaffected.
- `Github` namespace casing (GitHub) — cosmetic, deferred; folder typo `FIles` is
  fixed instead.

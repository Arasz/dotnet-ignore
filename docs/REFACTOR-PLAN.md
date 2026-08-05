# Refactor Plan — dotnet-ignore (findings -> TDD)

Source: docs/CODE-REVIEW.md. Every step is RED (failing test) -> GREEN (minimal
production change) -> run gate. Steps are ordered so behavior fixes land before
renames (cheaper diffs) and docs last.

## Phase A — Test-harness hygiene (no production behavior change)

- A1. Delete UnitTestBase.cs and the AutoFixture package reference; make test
  classes plain. (Kills CA1816 and the dead Fixture.)
- A2. Fix CS8019 (drop unused `using System.Collections.Immutable` in
  ListGitIgnoreFilesCommand.cs) and CA1866 (`StartsWith(char)` in tests).
- Gate: `dotnet build` 0 warnings/errors, `dotnet test` 18/18 green.

## Phase B — Behavior fixes (TDD: failing test first, then minimal change)

- B1. `list -s` inversion (review #1)
  - RED: new ListGitIgnoreFilesCommandTests — `ListGitIgnoreFiles(true)` strips the
    `.gitignore` suffix, `ListGitIgnoreFiles(false)` keeps it (stubbed service).
  - GREEN: rename parameter to `shorten`, invert the branch.
- B2. Null guard (review #14)
  - RED: update ConcatedNamesProcessorTests to expect `ArgumentNullException`.
  - GREEN: `ArgumentNullException.ThrowIfNull(names)` in NamesSplitStrategy.Split.
- B3. Write-error handling (review #3)
  - RED: GitignoreGetCommandHandlerTests — writer throwing `IOException` /
    `UnauthorizedAccessException` must propagate unchanged (no ArgumentException).
  - GREEN: remove the try/catch wrapper in GetGitIgnoreFile.
- B4. Service nullability + single listing (reviews #4, #5, #9)
  - RED: new GithubGitignoreServiceTests with mocked `IGitHubClient` and mocked
    `HttpMessageHandler`:
    - exact-name match downloads the file content
    - name without `.gitignore` suffix matches
    - case-insensitive match
    - unknown name returns null
    - `GetAllContents` is called exactly once across two `GetIgnoreFile` calls
      (the listing cache)
  - GREEN: single ctor taking `IGitHubClient` + `HttpClient`; lazy cached listing;
    return `GitignoreFile?`; delete `GitignoreFile.Empty`.
  - Update GetGitIgnoreFileCommand to null-check (also fixes "Where" typo in the
    method name and the error wording) and its tests (mock returns null instead of
    `GitignoreFile.Empty`); update GitignoreServiceStub.
  - Delete GithubClientTests.cs (live-API tests, review #5) — the service is now
    unit-tested; xUnit1051 warnings disappear.
- B5. Deterministic merge header (review #8)
  - RED: SimpleMergeStrategyTests — header line matches
    `^# Git ignore file created by dotnet-ignore on \d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$`.
  - GREEN: `DateTimeOffset.UtcNow.ToString("u", CultureInfo.InvariantCulture)`.
- B6. Minimize-mode characterization tests (review #10)
  - Tests (pass against existing behavior): comments removed, blank lines removed,
    duplicates deduped first-wins, headers preserved, "were removed" wording.
  - GREEN: fix "where removed" -> "were removed" in the minimize header.
- B7. Global error handler (review #2)
  - Program.cs: try/catch around `consoleApp.Run()` -> `Error: {message}` on
    stderr, exit code 1. Also reword the invalid-name message.
  - Gate: manual smoke test `get NonExistingFile` -> clean one-line error, exit 1.
- Gate after B: `dotnet build` clean; `dotnet test` all green; smoke-test `get
  VisualStudio,Node`, `get VisualStudio,Node -m`, `list`, `list -s` (now correct).

## Phase C — Structure cleanup

- C1. Rename folder/namespace `FIles` -> `Files` (review #15).
- C2. DI cleanup (review #7): register `IGitHubClient` singleton in Program.cs;
  delete the parameterless ctor. Register HttpClient via `AddHttpClient` with a
  ProductHeaderValue default. (Review #6 satisfied by injection.)
- C3. `GeneratePackageOnBuild` conditioned to Release (review #16); delete
  MergeStrategyStub's seedless Aggregate usage (review #17) if touched by B-tests.
- Gate: build clean, tests green, smoke tests pass.

## Phase D — Coverage gate

- Run coverage again (coverlet.collector already referenced): target >= 70% line
  coverage on src/CliTool (Program.cs top-level statements excepted — wiring is
  verified by smoke tests; B1-B7 raise the rest). Record in docs/CODE-REVIEW.md
  post-refactor section.

## Phase E — Packaging, changelog, docs

- E1. Confirm `<Version>1.1.0</Version>`; update PackageReleaseNotes to the 1.1.0
  changelog bullet list.
- E2. Add CHANGELOG.md (1.1.0 entry: .NET 10 migration, package updates, bug fixes
  B1-B7, coverage uplift).
- E3. README: add requirements (.NET 10 SDK), usage examples (`get`, `get -m`,
  `list`, `list -s`), link CHANGELOG.
- E4. Pack verification: `dotnet pack -c Release`, install the nupkg into a temp
  tool path, run `ignore -h`, `ignore get VisualStudio -d <tmp>`.

## Phase F — PR

- F1. Commit per phase (small commits), push feature/dotnet10-upgrade, open PR to
  master with the review summary and coverage before/after.

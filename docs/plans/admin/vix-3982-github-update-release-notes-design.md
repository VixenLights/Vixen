# Move update metadata and release notes to GitHub

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain it according to `.agents/PLANS.md` from the repository root.

This plan implements Jira issue `VIX-3982`. Its description must be kept aligned with the requirements, acceptance criteria, and test plan below; add final validation results as an issue comment before the work is considered complete.

## Purpose / Big Picture

After this work, Vixen checks GitHub directly for update metadata and shows the release notes that GitHub published with the available update. The automatic status-strip check obtains only whether an update exists, while the manual Check for Updates dialog makes one request, accurately reports availability even when notes are empty, and displays the matching GitHub release body. A user can verify this by opening Help > Check for Updates on a development or release installation and by running focused fake-HTTP tests without contacting GitHub.

## Progress

- [x] (2026-08-14 00:00Z) Read the approved design, `.agents/PLANS.md`, current callers, project dependencies, and existing uncommitted GitHub endpoint changes.
- [x] (2026-08-14 00:00Z) Created this ExecPlan and chose a shared application-lifetime `HttpClient` to avoid introducing a second dependency-injection container.
- [ ] Update VIX-3982 with this plan's final requirements, acceptance criteria, and test plan before the production migration (blocked: the Atlassian connector is not available in this session).
- [x] (2026-08-14 00:00Z) Added the update contracts, tag comparison, typed GitHub wire model, friend-assembly test seam, and fake-handler tests.
- [x] (2026-08-14 00:00Z) Added and composed `GitHubUpdateService`; fake-handler tests verify bounded requests, error states, and in-memory caching.
- [x] (2026-08-14 00:00Z) Migrated automatic startup status checks and the manual dialog; removed Jira preflight and Jira release-note I/O.
- [x] (2026-08-14 00:00Z) Built the application and tests; focused tests passed 11/11 and the full already-built x64 test assembly passed 705/705.
- [x] (2026-08-14 00:00Z) Replaced the legacy packaged release-notes text with an exact-tag GitHub release lookup for the running release or development build.
- [x] (2026-08-14 00:00Z) Built `Vixen.Application`; focused `GitHubUpdateServiceTests` passed 12/12 after the current-release notes addition.
- [ ] Manually validate release, development, test-build, no-update, empty-body, offline, and skipped-release paths; update VIX-3982 with final validation results.

## Surprises & Discoveries

- Observation: The working tree already contained a partial GitHub endpoint migration in `VersionInfo.cs`.
  Evidence: It calls GitHub but still calls `CheckForConnectionToWebsite()`, which probes `http://bugs.vixenlights.com`; it selects prereleases by `published_at` and returns `0` on failure.
- Observation: `Vixen.Tests` does not reference `Vixen.Application`.
  Evidence: `src/Vixen.Tests/Vixen.Tests.csproj` has no application project reference, so focused service tests require an application reference and friend-assembly access.
- Observation: This repository declares friend assemblies in project files rather than standalone `AssemblyInfo.cs` files.
  Evidence: Existing projects use the MSBuild `AssemblyAttribute` item for `InternalsVisibleTo`.

## Decision Log

- Decision: Use an injected, application-lifetime `HttpClient` backed by `SocketsHttpHandler` with a ten-minute pooled connection lifetime.
  Rationale: This fulfils the design’s lower-risk lifetime option without adding a Microsoft DI composition root to an application that currently constructs `VixenApplication` directly and uses Catel registration separately.
  Date/Author: 2026-08-14 / Codex
- Decision: Keep update contracts internal and test them using `InternalsVisibleTo`.
  Rationale: No other assembly currently needs the contracts; making them public merely for tests would expand the application API. Declare the friend assembly with an `AssemblyAttribute` item in `Vixen.Application.csproj`, following repository convention; do not add `AssemblyInfo.cs`.
  Date/Author: 2026-08-14 / Codex
- Decision: Treat a `null` service result as an unavailable update check, never as an installed-current result.
  Rationale: A failed optional network operation must not make a false update claim.
  Date/Author: 2026-08-14 / Codex

## Outcomes & Retrospective

The update path now owns GitHub transport, parsed release metadata, comparison, availability, caching, and optional release-note exposure in one internal service. `VersionInfo` now contains installed-version facts only, the manual update dialog has no Jira requests, and Help > Release Notes retrieves the exact GitHub release for the running tag. `Vixen.Application` built successfully; focused fake-HTTP tests passed 12/12 and the full already-built x64 test assembly previously passed 705/705. Manual UI validation and the mandated VIX-3982 description/comment updates remain because this session has no Atlassian connector.

## Context and Orientation

`src/Vixen.Application/VersionInfo.cs` currently owns local installed-version facts and remote update requests. `src/Vixen.Application/VixenApplication.cs` uses those requests at startup to populate the status strip. `src/Vixen.Application/CheckForUpdates.cs` repeats the lookup then performs Jira REST calls to construct notes in a WinForms text box. `src/Vixen.Application/Program.cs` constructs the application form and is the appropriate place to create and dispose application-lifetime networking resources.

GitHub exposes a full release from `GET releases/latest` and a bounded release list from `GET releases?per_page=5`. A release body is the text published with that GitHub release. Development tags have the exact form `DevBuild-N`, where `N` is a positive numeric build number. Stable Vixen tags use `major.minor` or `major.minoruN`; `uN` is an update number and must compare numerically. A draft GitHub release is unpublished and must be ignored.

## Plan of Work

Milestone 1 updates VIX-3982's description, preserving its original content and adding the purpose, all acceptance criteria from this plan, and the fake-HTTP plus manual test plan. Do not transition the issue unless its workflow requires it. At the final milestone, update any changed acceptance or test text, then add a comment with exact build and test results.

Milestone 2 adds internal one-type-per-file contracts in `src/Vixen.Application/Updates`: `UpdateChannel`, `UpdateCheckRequest`, `UpdateCheckResult`, `IUpdateService`, a typed GitHub release DTO, and a stable-tag comparison helper. Add an `AssemblyAttribute` item to `src/Vixen.Application/Vixen.Application.csproj` granting `Vixen.Tests` internal access, reference `Vixen.Application` from `src/Vixen.Tests/Vixen.Tests.csproj`, and add fake-`HttpMessageHandler` tests under `src/Vixen.Tests/Application/Updates`. Do not add `AssemblyInfo.cs`. The tests must prove that only valid non-draft development prereleases are considered, the largest build wins despite timestamps, stable tags compare `3.13`, `3.13u1`, and later releases correctly, empty bodies do not affect availability, notes are omitted unless requested, and failure is distinguishable.

Milestone 2 implements `GitHubUpdateService` with the shared injected client. It configures the client once in `Program`: GitHub base URI, five-second timeout, User-Agent, Accept, and current GitHub API-version header. The service uses typed JSON deserialization, accepts a cancellation token, performs exactly `releases/latest` for stable checks or `releases?per_page=5` for development checks, filters drafts, validates tags, compares installed and latest values, and retains a short successful in-memory cache including the body. It logs transport, HTTP, rate-limit, and malformed-data failures with structured NLog calls and returns `null` on failure.

Milestone 3 passes `IUpdateService` through constructors into `VixenApplication` and `CheckForUpdates`. Startup asks for no notes and uses `IsUpdateAvailable`. The dialog asks once for notes, sets availability from the returned boolean, normalizes release-body line endings for the WinForms text box, and reports an explicit unavailable state on `null`. Delete `PopulateChangeLog` and its Jira parsing, delete remote/update methods and the Jira connectivity probe from `VersionInfo`, and remove the menu preflight. Keep installer download work out of scope.

Milestone 4 runs the focused tests, full repository test workflow, and application build. It then manually tests release, development, test-build, no-update, empty-body, offline, and skipped-release paths. A skipped stable installation must show only the latest stable release body.

Milestone 5 replaces the Help > Release Notes file read with `IUpdateService.GetReleaseNotesAsync`. The menu converts the running version to its exact GitHub tag (`major.minor`, `major.minoruN`, or `DevBuild-N`), while test builds do not make a network call. The service retrieves `releases/tags/{tag}`, validates the returned tag, accepts an empty body, and caches the parsed release. The dialog presents an explicit unavailable message if GitHub cannot provide the release.

## Concrete Steps

Run commands from `C:\Dev\Vixen` in PowerShell. Before each milestone, inspect `git status --short` and preserve the pre-existing design document and partial update edits until the replacement is ready. Use `git diff --check` after each edit.

For Milestone 1, use the Atlassian connector to update VIX-3982. Keep the ticket in progress unless its available transitions or the user direct otherwise. Use the following issue-description sections: `Implementation scope` (move remote update discovery and notes to `GitHubUpdateService`; preserve `VersionInfo` for installed facts), `Acceptance Criteria` (copy the complete Validation and Acceptance section), and `Test Plan` (focused fake-handler tests, full x64 test workflow, and the manual release/development/test/no-update/empty/offline/skipped-release cases). Record the returned update timestamp in Progress.

For Milestone 2, add the contracts and tests, then run:

    msbuild src/Vixen.Tests/Vixen.Tests.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

For Milestone 2, run the same focused test command after adding service tests. Confirm fake handler request paths are exactly `/repos/VixenLights/Vixen/releases/latest` or `/repos/VixenLights/Vixen/releases?per_page=5`, and no test needs a network connection.

For Milestone 3, build the application:

    msbuild src/Vixen.Application/Vixen.Application.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -v:m

For Milestone 4, use the repository-required full test sequence:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

Expect exit code zero. Record the actual test count in this plan. If full tests cannot run because the machine lacks the C++ toolset, record that separately and retain focused test output.

## Validation and Acceptance

Acceptance requires focused tests proving development selection, bounded one-request behavior, stable comparison, empty-body availability, notes inclusion, latest-stable-only notes, and transport/HTTP/rate-limit/malformed-JSON failure handling. No production update path may contain a Jira REST URL. The application build and full x64 test workflow must pass. Manually, the updates menu opens without a preflight request; offline GitHub reports that updates cannot be checked; a new update with an empty body is still offered; and a skipped stable installation displays only the newest stable release body. VIX-3982 must contain the initial requirements update and final validation comment.

## Idempotence and Recovery

Fake-handler tests are deterministic and make no network calls. Re-running build and test commands is safe. The in-memory cache lives only for the application process. If constructor wiring breaks the WinForms designer, preserve a parameterless designer-only form constructor that does not build a second production service graph; runtime construction must use the injected service. If a failure occurs mid-migration, keep `VersionInfo` local-version members intact, restore only the changed update callers, and retain this plan’s discoveries.

## Artifacts and Notes

The intended runtime flow is:

    Program creates shared GitHub HttpClient and GitHubUpdateService
        VixenApplication startup -> CheckAsync(channel, installed, includeNotes: false)
        Check for Updates dialog -> CheckAsync(channel, installed, includeNotes: true)
        GitHubUpdateService -> one GitHub response -> availability and optional body

The update result must contain at least `LatestVersion`, optional `LatestBuildNumber`, `IsUpdateAvailable`, optional `ReleaseNotes`, `ReleasePageUri`, and optional `PublishedAt`. `IncludeReleaseNotes` only controls exposing the already-received body; it must not trigger another GitHub request.

## Interfaces and Dependencies

Create these internal contracts in `VixenApplication.Updates`, one type per file:

    internal enum UpdateChannel { Release, Development }

    internal sealed record UpdateCheckRequest(
        UpdateChannel Channel,
        string InstalledVersion,
        bool IncludeReleaseNotes);

    internal sealed record UpdateCheckResult(
        string LatestVersion,
        int? LatestBuildNumber,
        bool IsUpdateAvailable,
        string? ReleaseNotes,
        Uri ReleasePageUri,
        DateTimeOffset? PublishedAt);

    internal interface IUpdateService
    {
        Task<UpdateCheckResult?> CheckAsync(
            UpdateCheckRequest request,
            CancellationToken cancellationToken = default);
    }

`GitHubUpdateService` accepts `HttpClient` by constructor and does not create or dispose it. `Program` disposes its handler/client after `Application.Run`. Use existing `Newtonsoft.Json` for typed deserialization; do not use `dynamic`, Jira, or a new package. `VersionInfo` remains the documented public owner of installed assembly version facts only.

Revision note (2026-08-14): Initial ExecPlan created from `docs/reviews/github-update-release-notes-design.md`, direct source inspection, and the pre-existing partial GitHub endpoint migration. It selects the design’s shared-client option to keep the composition change scoped.

Revision note (2026-08-14): Associated the work with VIX-3982. Added initial and final Jira-update milestones plus the issue key required by repository commit-message conventions.

Revision note (2026-08-14): Completed the code migration and automated validation. The GitHub service uses the shared-client option, caches successful parsed responses for five minutes, and the dialog consumes one result rather than querying Jira. Jira and manual validation are recorded as remaining external work because the configured tools cannot access Atlassian or operate an installed Vixen build interactively.

Revision note (2026-08-14): Moved the plan to the `admin/vix-3982-...` convention and corrected the friend-assembly declaration. `InternalsVisibleTo` now uses the established MSBuild `AssemblyAttribute` pattern in `Vixen.Application.csproj`; the standalone `AssemblyInfo.cs` file was removed.

Revision note (2026-08-14): Extended VIX-3982 to replace Help > Release Notes' `Release Notes.txt` read. The running build now requests its exact GitHub release tag through the existing update service, rather than displaying the packaged text file.

Revision note (2026-08-14): Hardened the Release Notes load event. It now delegates to a task-returning method and catches/logs unexpected failures at the required `async void` event boundary before showing the unavailable state.

Revision note (2026-08-14): Added a display-only fallback to the packaged `Release Notes.txt` when the exact GitHub release is unavailable or its body is empty. The file remains unchanged; its LF-only content is normalized to WinForms line endings in memory before display.

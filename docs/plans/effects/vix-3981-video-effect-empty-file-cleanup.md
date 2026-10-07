# VIX-3981: Prevent an unconfigured Video effect from crashing sequencer shutdown

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

Users can add a Video effect as a placeholder and close the sequencer before choosing a video file. Today that normal workflow can terminate the application with `System.IO.DirectoryNotFoundException` while the effect disposes. After this change, an unconfigured Video effect closes cleanly whether the “Clear Effect Cache on Exit” option is enabled or disabled. Configured Video effects retain their existing pairing-file and stale-cache cleanup behavior, and an unconfigured effect can never delete the shared Video cache root.

The behavior is observable by adding a Video effect, leaving Filename empty, and closing the sequence. The application must remain open and usable. A focused automated test must prove the same behavior using a unique temporary cache root, without inspecting or deleting the developer’s real `%TEMP%\\Vixen\\VideoEffect` folder.

## Progress

- [x] (2026-08-10 10:00 -05:00) Read VIX-3981, the Video effect lifecycle, the cache-improvement history, project build instructions, and `.agents/PLANS.md`.
- [x] (2026-08-10 10:00 -05:00) Identified the missing-directory exception and the separate empty-hash shared-root deletion risk.
- [x] (2026-08-10 10:00 -05:00) Wrote this ExecPlan; no production or test source has been changed by this planning work.
- [x] (2026-08-10 08:56 -05:00) Updated VIX-3981 with the confirmed diagnosis, scope, acceptance criteria, and validation plan. The issue remains In Progress; JIRA returned update timestamp `2026-08-10T08:56:49.471-0500`.
- [x] (2026-08-10 09:02 -05:00) Added `VideoCacheCleanup`, internal test visibility, and seven isolated cache-cleanup regression tests. The Video module build and `Vixen_Tests` build succeeded; `dotnet test --no-build` reported 683 passed, 0 failed, and 0 skipped.
- [x] (2026-08-10 09:05 -05:00) Routed `Video.Removing()` and `Video.Dispose(bool)` through the guarded helper. The Video module build and `Vixen_Tests` build succeeded; `dotnet test --no-build` reported 683 passed, 0 failed, and 0 skipped. Manual sequencer validation remains Milestone 4.
- [x] (2026-08-10 09:17 -05:00) Added the final VIX-3981 validation comment (ID 40349). It records the successful build/test results, manual no-crash reproduction, configured-cache cleanup, and intentional retention of the shared cache root. No scope adjustment or status transition was needed.

## Surprises & Discoveries

- Observation: the reported stack trace is caused by filesystem enumeration rather than ffmpeg or rendering.
  Evidence: `Video.Removing()` at `src/Vixen.Modules/Effect/Video/Video.cs:1173` calls `Directory.EnumerateFiles(TempPath, ...)`; the ticket’s stack trace identifies this exact line and reports that `%TEMP%\\Vixen\\VideoEffect` does not exist.

- Observation: an empty Filename bypasses all cache creation.
  Evidence: `Video.SetupRender()` returns at line 562 when `_data.FileName == ""`; `ProcessMovie()` is therefore never reached, and that method is the path that creates the hash directory and pairing file.

- Observation: an unconfigured effect has an empty settings hash, so its initial `_tempFilePath` is the shared cache root rather than a per-settings subdirectory.
  Evidence: the constructor calls `PopulateTempPath()` while `_settingsHash` is `String.Empty`; `PopulateTempPath()` uses `Path.Combine(TempPath, _settingsHash)`. The current clear-on-exit code then calls `Directory.Delete(_tempFilePath, true)` at line 1186.

- Observation: the cache behavior was introduced by VIX-3625 to share hash-based thumbnail folders among effects.
  Evidence: commit `2b046e986` changed the folder model from per-instance folders to settings-hash folders plus `InstanceId.settingsHash` pairing files. The correction must retain that sharing model.

- Observation: `Vixen.Tests` does not currently reference the Video effect project.
  Evidence: `src/Vixen.Tests/Vixen.Tests.csproj` has no `Video.csproj` project reference, and the Video project does not currently grant `Vixen.Tests` access to internal members.

- Observation: a non-copy-local Video project reference compiles the test assembly but prevents `dotnet test` from loading `Module.Effect.Video`.
  Evidence: the initial test run built successfully but all seven `VideoCacheCleanupTests` failed with `FileNotFoundException` for `Module.Effect.Video`. The test project’s established project references rely on the default copy-local behavior, so the Video reference must follow that local convention.

- Observation: Video cache cleanup retains the top-level `%TEMP%\\Vixen\\VideoEffect` directory after a configured effect is closed.
  Evidence: manual validation confirmed that cache artifacts are cleaned while the shared root remains. This is expected because the root is the reusable container for settings-hash subdirectories and pairing files.

## Decision Log

- Decision: Fix the cleanup boundary instead of forcing an empty Video effect to create a cache directory.
  Rationale: an empty Filename is a valid editor state and does not require ffmpeg output. Creating a directory merely to make shutdown safe would leave unnecessary filesystem state and conceal the ownership error.
  Date/Author: 2026-08-10 / Codex

- Decision: Treat the cache root as optional for every enumeration operation and treat a blank settings hash as “no per-effect cache directory.”
  Rationale: cleanup is best-effort maintenance, not a condition for closing the editor. A settings hash is calculated only for a configured rendering attempt, so a blank hash must never identify a deletable cache directory.
  Date/Author: 2026-08-10 / Codex

- Decision: Extract only deterministic cache-root cleanup into an internal helper with a supplied root path.
  Rationale: directly exercising `Video.Dispose()` would use the real static `%TEMP%\\Vixen\\VideoEffect` path and would make tests interfere with a user’s cache. A small helper can be tested against a GUID-named temporary root while keeping the Video effect responsible for lifecycle policy, settings-hash state, and logging.
  Date/Author: 2026-08-10 / Codex

- Decision: Update the JIRA issue before code changes and add final evidence after validation; do not transition the issue unless the current workflow requires it.
  Rationale: `.agents/PLANS.md` requires an initial requirements/acceptance update and a final results comment. The existing issue is already In Progress, so an unsolicited status transition is out of scope.
  Date/Author: 2026-08-10 / Codex

- Decision: Use the default copy-local setting for the Video project reference from `Vixen.Tests`.
  Rationale: the test runner loads assemblies from `src/Vixen.Tests/bin/Release`; a non-copy-local Video module cannot be found there. This follows the existing test-project reference pattern and permits the isolated regression tests to execute.
  Date/Author: 2026-08-10 / Codex

- Decision: Keep the helper responsible for deleting unpaired cache directories and have `Video.Dispose(bool)` log any cleanup failure.
  Rationale: the helper already has an isolated, deterministic filesystem contract. Wrapping that operation in the existing NLog-based disposal error handling prevents cleanup maintenance from terminating disposal without adding another production abstraction.
  Date/Author: 2026-08-10 / Codex

- Decision: Retain the top-level Video cache root after cleanup.
  Rationale: it is a shared, reusable container rather than an effect-owned cache directory. Deleting only settings-hash children and pairing files preserves the VIX-3625 cache model and avoids treating the root as a deletion target.
  Date/Author: 2026-08-10 / Codex

## Outcomes & Retrospective

All milestones are complete. VIX-3981 now has a guarded cache-cleanup boundary, seven isolated xUnit tests, and lifecycle integration in `Video.Removing()` / `Video.Dispose(bool)`. The Video module build and full `Vixen_Tests` build succeeded; `dotnet test --no-build` reported 683 passed, 0 failed, and 0 skipped. Manual testing confirmed that an empty-Filename Video effect no longer crashes the sequencer on close, and that a configured Video effect cleans cache artifacts while retaining the intended shared root. Final evidence was added to VIX-3981 comment 40349; the issue remains In Progress for normal workflow handling.

## Context and Orientation

Vixen is a modular Windows lighting-sequencer application. The Video effect module is in `src/Vixen.Modules/Effect/Video`. A Video effect converts a selected source video into still thumbnail frames for rendering. It stores those frames in a shared cache root at `%TEMP%\\Vixen\\VideoEffect`; in source this is the static `TempPath` field in `Video.cs`.

The shared cache root contains two kinds of entries. A hash directory contains the generated frames for one complete set of rendering settings. A pairing file named `<effect-instance-guid>.<settings-hash>` records that a particular effect instance uses that hash directory. `InstanceId` is the unique identifier inherited by every module instance. The pairing files allow cleanup to remove hash directories that no existing effect uses.

`src/Vixen.Modules/Effect/Video/Video.cs` derives `Video` from `PixelEffectBase`. `SetupRender()` prepares a configured effect. It deliberately returns when `VideoData.FileName` is empty, because there is no video to render. `Removing()` is invoked when the effect is deleted and removes the instance’s pairing files. `Dispose(bool)` runs when the sequence editor closes; it optionally clears the current effect cache and always scans for unpaired hash directories. `VixenSystem.ClearEffectCacheOnExit` is the application option that controls the optional per-effect delete.

VIX-3981 reports this reproduction: add a Video effect, do not set Filename, then close the sequencer. The existing `Removing()` and the later cache-root scan enumerate `TempPath` without first verifying it exists. `Directory.EnumerateFiles` and `Directory.EnumerateDirectories` throw `DirectoryNotFoundException` for an absent root, which propagates through module disposal and crashes the application.

The desired implementation adds `src/Vixen.Modules/Effect/Video/VideoCacheCleanup.cs`. This internal helper owns only safe directory enumeration and pairing-file deletion for a root path supplied by its caller. It must not own `Video` lifecycle decisions, create directories, invoke ffmpeg, or alter persisted effect data. `Video.cs` remains the caller that decides whether `ClearEffectCacheOnExit` applies and catches/logs failures when deleting an existing per-hash directory.

The test project is `src/Vixen.Tests/Vixen.Tests.csproj` and uses xUnit v3. It currently needs a project reference to `Video.csproj`. The Video module assembly name is inherited from `src/Vixen.Modules/Effect/Directory.Build.props` as `Module.Effect.Video`; grant the test assembly `Vixen.Tests` access with the MSBuild-generated `InternalsVisibleToAttribute` pattern already used by other module projects. This is solely to test the helper without widening Vixen’s public module API.

## Plan of Work

### Milestone 1: Record the diagnosis and acceptance contract in JIRA

Before editing the repository, retrieve VIX-3981 with the Atlassian connector and add the following information to its description, preserving the original reproduction steps and stack trace. State that the root cause is unconditional enumeration of the optional `%TEMP%\\Vixen\\VideoEffect` root in `Video.Removing()` and `Video.Dispose(bool)`. State that `SetupRender()` exits before cache creation when Filename is blank. Record the secondary safety defect: a blank `_settingsHash` makes `_tempFilePath` equal the shared root, so clear-on-exit must not recursively delete it.

Add these acceptance criteria to the issue description: an empty-Filename Video effect closes the sequencer without an exception when the cache root does not exist; it also closes without deleting a pre-existing shared cache root; configured effects still remove their own pairing files and can remove unpaired hash folders; enabling or disabling Clear Effect Cache on Exit does not alter those safety guarantees; and automated tests plus the manual reproduction pass. Add the test plan described in the Validation and Acceptance section. Keep the issue In Progress unless the issue’s available transitions or the user direct otherwise.

Use the JIRA API only for the VIX-3981 description update. Do not add unrelated issue comments, change priority, or upload files. Record the returned update timestamp in this plan’s Progress section.

### Milestone 2: Create deterministic cache-cleanup regression tests

Create `src/Vixen.Modules/Effect/Video/VideoCacheCleanup.cs` with one internal static class named `VideoCacheCleanup`. Give it focused methods whose root path is a parameter rather than the production static path:

    internal static void RemovePairingFiles(string cacheRoot, Guid instanceId)
    internal static void RemoveUnpairedCacheDirectories(string cacheRoot)

`RemovePairingFiles` must return immediately if `Directory.Exists(cacheRoot)` is false. Otherwise it removes only top-level files matching `<instanceId>.*`. It must not create `cacheRoot` and must not delete hash directories.

`RemoveUnpairedCacheDirectories` must also return immediately when the root is absent. When it exists, it must reproduce the current VIX-3625 ownership rule: start with the top-level directories, inspect top-level pairing files, derive the referenced hash from each file extension without its leading dot, retain only directories that exist at `Path.Combine(cacheRoot, hash)`, and delete the remaining directories. It must ignore files that do not point to an existing child directory. Directory deletion failures must be allowed to reach `Video.Dispose(bool)`, where the existing per-directory logging policy is retained or moved with equivalent structured error context. Do not catch broadly and silently.

Add an internal helper for deleting a resolved hash directory only if needed to avoid duplicating path safeguards. Its contract must reject null, empty, or whitespace cache keys and must never use the root itself as a deletion target. It may use `Path.Combine(cacheRoot, cacheKey)` only after validating the key. Keep this method internal and test it directly; do not make cache-management APIs public.

Update `src/Vixen.Modules/Effect/Video/Video.csproj` to grant `Vixen.Tests` internal visibility using an `AssemblyAttribute` item. Update `src/Vixen.Tests/Vixen.Tests.csproj` to reference `..\\Vixen.Modules\\Effect\\Video\\Video.csproj`, following the project’s existing project-reference style and keeping the reference limited to the test project.

Create `src/Vixen.Tests/Effect/Video/VideoCacheCleanupTests.cs`. Use one fresh root per test, for example `Path.Combine(Path.GetTempPath(), $"VIX-3981-{Guid.NewGuid():N}")`, and implement `IDisposable` to remove only that exact root in cleanup. Use Arrange, Act, Assert structure. At minimum, add these tests:

1. `RemovePairingFiles_WhenCacheRootDoesNotExist_DoesNotThrow`: call the method on the unique, absent root and assert no exception and no directory creation.
2. `RemoveUnpairedCacheDirectories_WhenCacheRootDoesNotExist_DoesNotThrow`: call the method on the same kind of absent root and assert no exception and no directory creation.
3. `RemovePairingFiles_RemovesOnlyMatchingInstanceFiles`: create two pairing files for different GUIDs and one hash directory, remove one instance, then assert only that instance’s pairing is gone and the other pairing/directory remain.
4. `RemoveUnpairedCacheDirectories_DeletesOnlyDirectoriesWithoutPairings`: create two hash directories, create a pairing file referencing one hash, run cleanup, then assert the referenced directory remains and the unreferenced directory is deleted.
5. `DeleteResolvedCacheDirectory_WhenCacheKeyIsEmpty_DoesNotDeleteRoot`: create the unique root with a sentinel file, request deletion with an empty cache key, then assert the root and sentinel remain.

These tests prove the filesystem boundary that caused VIX-3981 without depending on WinForms, ffmpeg, the sequence editor, or an actual user cache.

### Milestone 3: Route Video lifecycle cleanup through the guarded helper

Edit `src/Vixen.Modules/Effect/Video/Video.cs` only after the helper tests exist. In `Removing()`, replace direct `Directory.EnumerateFiles(TempPath, ...)` and `File.Delete` calls with `VideoCacheCleanup.RemovePairingFiles(TempPath, InstanceId)`. This makes deletion of an unconfigured effect harmless when no cache has ever been created.

In `Dispose(bool)`, preserve the existing high-level order: optional clear-on-exit work first, then unpaired-directory cleanup, then `base.Dispose(disposing)`. When `VixenSystem.ClearEffectCacheOnExit` is true, call `Removing()` and attempt to delete only a resolved, non-empty settings-hash directory. Do not call `Directory.Delete(_tempFilePath, true)` when `_settingsHash` is blank. Do not log an error for the normal case in which that hash directory was never created; use `Directory.Exists` before attempting deletion. Continue to catch and log real deletion failures with the existing NLog `Logging.Error(exception, ...)` convention.

Use `VideoCacheCleanup.RemoveUnpairedCacheDirectories(TempPath)` for the final stale-cache scan. Preserve the existing behavior of deleting hash directories that have no pairing file. If the helper reports candidate directories and `Video` performs deletion instead, preserve individual `try/catch` logging around each directory delete; do not let cleanup errors terminate disposal. Choose one ownership model and make it explicit in XML comments only if a public/protected API changes. This work adds only internal types and changes no public/protected API, so no XML documentation edit is expected.

Do not modify `VideoData.cs`, ffmpeg code, `VixenSystem`, sequence serialization, or UI behavior. Do not create the cache root for blank filenames. Do not alter `_VideoCacheKeyedSemaphore`; it protects cache generation and is unrelated to shutdown enumeration.

### Milestone 4: Build, validate, and publish final tracker evidence

First build the affected module in the repository root. Then build the repository’s full test target with full MSBuild, because `Vixen.Tests` has C++/CLI transitive dependencies. Finally run the already-built test assembly with `dotnet test --no-build` using x64 settings. Record the exact test count and any pre-existing warnings separately from new failures.

Perform the manual regression in a disposable test sequence. Ensure `%TEMP%\\Vixen\\VideoEffect` is absent before the first case, add Video without Filename, and close the sequencer. Repeat with Clear Effect Cache on Exit enabled and disabled. For the shared-root safety case, create a valid Video effect that has generated a cache, then add a second Video effect without Filename and close the sequencer; verify the configured effect’s cache root and its generated frames were not removed by the unconfigured effect. Do not remove a real user cache to prepare this test; use an isolated test account, a temporary `%TEMP%` environment for a launched development build, or a disposable virtual machine.

After successful validation, append a VIX-3981 comment containing: the final root cause, the files changed, the automated test names and results, manual results for both option values, the shared-root preservation result, and any known limitations. If implementation changes the acceptance criteria, update the description before the comment. Do not resolve or transition VIX-3981 without explicit direction or a required workflow transition.

## Concrete Steps

Run all commands from `C:\\Dev\\Vixen` in PowerShell.

Before implementation, confirm the relevant call sites and that no earlier work changed the files:

    git status --short
    rg -n "SetupRender|PopulateTempPath|Removing\(|Dispose\(bool|EnumerateFiles\(TempPath|EnumerateDirectories\(TempPath" src/Vixen.Modules/Effect/Video/Video.cs
    rg -n "AssemblyAttribute Include=\"System.Runtime.CompilerServices.InternalsVisibleToAttribute\"" src/Vixen.Modules -g "*.csproj"

Expected result: `git status --short` is empty or contains only changes the implementer has explicitly preserved; `Video.cs` identifies `SetupRender` near line 559 and cleanup near lines 1170–1218.

Use the Atlassian connector to retrieve and update VIX-3981. Send Markdown preserving its current reproduction and stack trace, then append a “Findings”, “Acceptance Criteria”, and “Test Plan” section using the text in Milestone 1. Confirm the returned issue still has key `VIX-3981` and status In Progress.

Implement Milestones 2 and 3. Before building, inspect the diff:

    git diff --check
    git diff -- src/Vixen.Modules/Effect/Video/Video.cs src/Vixen.Modules/Effect/Video/VideoCacheCleanup.cs src/Vixen.Modules/Effect/Video/Video.csproj src/Vixen.Tests/Vixen.Tests.csproj src/Vixen.Tests/Effect/Video/VideoCacheCleanupTests.cs

Expected result: no whitespace errors; the only production behavior change is guarded cache cleanup.

Build the module:

    msbuild src/Vixen.Modules/Effect/Video/Video.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -v:m

Expected result: exit code 0 and no errors from the Video project.

Build and run the full test suite as required by this repository:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

Expected result: both commands exit 0. The second command reports all tests passed, including `VideoCacheCleanupTests`; record the actual passed/failed/skipped counts in this document and the final JIRA comment.

Run the manual cases from Milestone 4. If a manual case fails, keep VIX-3981 In Progress, save the observed exception/log text, add it to Surprises & Discoveries, and revise the Decision Log before broadening the code change.

## Validation and Acceptance

The work is accepted only when all of the following are true:

- The seven `VideoCacheCleanupTests` described in Milestone 2 pass. They cover absent roots, instance pairing isolation, paired/unpaired cache directories, an empty key, a root-resolving key, and a resolved child directory.
- The Video module build completes successfully.
- `Vixen_Tests` builds with full MSBuild and the already-built test assembly passes through `dotnet test --no-build` on x64.
- With the cache root absent, adding a Video effect with no Filename and closing the sequencer does not crash or show a `DirectoryNotFoundException` when Clear Effect Cache on Exit is enabled.
- The same empty-Filename workflow closes cleanly when Clear Effect Cache on Exit is disabled.
- When a configured effect has created a shared cache root, closing a second unconfigured Video effect does not delete the cache root, configured hash directory, pairing file, or rendered frame files.
- A configured effect still removes its own pairing file when deleted, and a later cleanup removes an unreferenced hash directory while preserving a paired one.
- VIX-3981 contains the initial findings/acceptance update and a final validation comment with actual results.

## Idempotence and Recovery

All tests use GUID-named roots under the system temporary directory and remove only their own root in `Dispose`; rerunning them is safe. The helper must never create a directory when asked to clean an absent root. The JIRA description update must preserve the ticket’s original reproduction and stack trace, so it can be retried by replacing or editing only the added findings sections.

If a build fails due to the Video test project reference or internal visibility, inspect `src/Vixen.Modules/Effect/Directory.Build.props` for the assembly name and compare the `AssemblyAttribute` syntax with `src/Vixen.Modules/Effect/State/State.csproj`. Correct the friend assembly name rather than making the helper public. If a test fails during cleanup, inspect the test’s GUID root before deleting it; do not run recursive cleanup against `%TEMP%` or the existing `Vixen` temp directory. If implementation must be backed out, revert only the files named in the diff review command and retain this plan plus the JIRA findings.

## Artifacts and Notes

The current failing shutdown path is:

    new Video()
        _settingsHash == ""
        _tempFilePath == TempPath

    SetupRender()
        FileName == ""
        return; // cache root is never created

    Dispose(true), ClearEffectCacheOnExit == true
        Removing()
        Directory.EnumerateFiles(TempPath, ...)
        throws DirectoryNotFoundException

The safe target behavior is:

    cleanup helper receives absent root
        Directory.Exists(root) == false
        return without creating or enumerating anything

    dispose receives blank settings hash
        remove pairing files if root exists
        do not request deletion of _tempFilePath / shared root
        scan stale cache only if root exists
        call base.Dispose(disposing)

The relevant production boundary is deliberately small:

    src/Vixen.Modules/Effect/Video/Video.cs
    src/Vixen.Modules/Effect/Video/VideoCacheCleanup.cs
    src/Vixen.Modules/Effect/Video/Video.csproj
    src/Vixen.Tests/Vixen.Tests.csproj
    src/Vixen.Tests/Effect/Video/VideoCacheCleanupTests.cs

## Interfaces and Dependencies

The completed implementation adds this internal-only interface in `VixenModules.Effect.Video`:

    internal static class VideoCacheCleanup
    {
        internal static void RemovePairingFiles(string cacheRoot, Guid instanceId);
        internal static void RemoveUnpairedCacheDirectories(string cacheRoot);
        internal static void DeleteResolvedCacheDirectory(string cacheRoot, string cacheKey);
    }

The exact third method name may differ only if its contract remains explicit: it accepts a root and a resolved hash key, returns without deletion for null/empty/whitespace keys or nonexistent paths, and cannot delete the supplied root. Its directory-deletion exception behavior must be documented in its implementation comments and handled with NLog context by `Video.Dispose(bool)` if it is not handled inside the helper.

No NuGet package is added. The test project gains a project reference to the existing Video module. The Video project grants `Vixen.Tests` access to the helper with `System.Runtime.CompilerServices.InternalsVisibleToAttribute`. Existing dependencies remain `System.IO`, xUnit v3, and the Vixen module framework. No public or protected `Video` API changes, data-contract changes, migration, or UI changes are permitted by this plan.

Revision note (2026-08-10): Initial ExecPlan created from VIX-3981 and direct source/history analysis. It records both the ticket’s missing-directory exception and the otherwise latent empty-settings-hash shared-root deletion risk, and it includes required initial and final JIRA updates.

Revision note (2026-08-10): Completed Milestone 1 by updating VIX-3981’s description. The issue now contains Findings, Acceptance Criteria, and Test Plan sections; no status transition, production change, or test change was made.

Revision note (2026-08-10): During Milestone 2 validation, replaced the initial non-copy-local Video test-project reference with the project’s established reference style because `dotnet test` could not load `Module.Effect.Video` otherwise.

Revision note (2026-08-10): Completed Milestone 2. The helper is intentionally not called by `Video.cs` until Milestone 3; the new tests validate its filesystem boundary independently. The module build and complete test suite passed with 683 tests.

Revision note (2026-08-10): Completed Milestone 3. `Removing()` now handles an absent cache root safely, and `Dispose(bool)` deletes only a validated settings-hash child directory before invoking guarded stale-cache cleanup. Module and full-test validation passed; manual validation remains required.

Revision note (2026-08-10): Completed Milestone 4. Manual validation confirmed the crash is fixed and configured cache cleanup works; the retained top-level cache root is intentional. Added final validation evidence to VIX-3981 comment 40349 without changing issue status.

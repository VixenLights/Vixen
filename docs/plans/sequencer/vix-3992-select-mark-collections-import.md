# Select Mark Collections Before Importing Them

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Keep it self-contained when revising it so a contributor can implement the feature using only this file and the working tree.

## Purpose / Big Picture

VIX-3992 makes every Marks Docker import safe to review before it changes the active timed sequence. A user will choose an existing import format and file as they do today; after the file (and any source-specific choices) has been fully parsed, Vixen will show one WPF dialog listing every imported Mark Collection. All rows start selected, duplicate names against the active sequence are visibly identified, and the user can keep or exclude individual collections. Nothing is appended, renamed, relinked, or made default until the user selects at least one row and presses OK.

The observable outcome is that Cancel at any file picker, source-specific dialog, or the new collection-selection dialog leaves the active sequence unchanged. Accepting a subset appends only that subset in source order, gives each collection a collision-free final name, and preserves valid phrase/word/phoneme relationships without silently re-selecting excluded parent collections.

## Progress

- [x] (2026-08-28 00:00Z) Read `.agents/PLANS.md`, the VIX-3992 handoff, the Marks Docker import entry point, current import service, Pangolin factory/parser tests, and the existing VIX-3947 naming plan.
- [x] (2026-08-28 17:22Z) Updated Jira issue VIX-3992 with the finalized user-facing requirements, acceptance criteria, and automated/manual test plan. The issue remains In Progress.
- [x] (2026-08-28 18:05Z) Created detached import result contracts and refactored Vixen 3, Bar Labels, Beat Labels, xTiming, Papagayo, Singing Faces, and Pangolin Beyond materializers to return candidates without mutating the target sequence. Adapted Pangolin tests and added xTiming local-link coverage.
- [x] (2026-08-28 18:30Z) Added the UI-free `MarkCollectionImportCommitter` and focused unit coverage for ordered unique names, default selection, valid/cleared links, empty commits, and invalid candidates without target mutation. Full MSBuild test build succeeded; focused materializer/committer tests passed 26/26.
- [x] (2026-08-28 19:00Z) Added the detached Catel collection-selection dialog, including duplicate-name indication, keyboard row toggling, command enablement, accepted-subset capture, and no-mutation selection tests. Full MSBuild test build succeeded; focused selection/materializer/committer tests passed 32/32.
- [ ] Replace the import command with the awaited materialize-select-commit workflow and apply XML documentation where public/protected APIs changed.
- [ ] Run automated validation, manually exercise all import formats and keyboard/mouse dialog behavior, then update Jira with results.

## Surprises & Discoveries

- Observation: The existing import service appends to the live collection set inside every import method, so a later cancellation cannot undo earlier additions.
  Evidence: `MarkImportExportService.ImportVixen3Beats`, `LoadBarLabels`, `LoadBeatLabels`, `LoadXTiming`, `ImportPapagayoTracks`, and `ImportPangolinBeyondMarks` all reach `AddUniqueCollection(collections, ...)`; `ImportSingingFacesTracks` ultimately calls `LoadXTimingTracks(xmlDoc, markCollection)`.

- Observation: xTiming creates word and phoneme links from `collections.Last().Id`, which only works because it appends each preceding collection immediately.
  Evidence: `ProcessTiming` in `MarkImportExportService.cs` assigns `LinkedMarkCollectionId = collections.Last().Id` for its second and third layers.

- Observation: Singing Faces is currently `async void`, so `MarkDockerViewModel.ImportCollection()` cannot await its downloaded timing result or place the selection dialog afterward.
  Evidence: `ImportSingingFacesTracks(ICollection<IMarkCollection>)` is declared `public static async void`; its caller invokes it from the synchronous `ImportCollection()` command.

- Observation: VIX-3947 already provides `MarkCollectionNameService.IsUniqueName` and `GetUniqueName`, including trimmed, case-insensitive collision behavior and suffixes such as ` - 2`.
  Evidence: `src/Vixen.Core/Marks/MarkCollectionNameService.cs` and `src/Vixen.Tests/Sequencer/MarkCollectionNameServiceTests.cs`.

- Observation: `dotnet test` cannot build the test project directly because its C++/CLI dependencies require the full Visual Studio C++ MSBuild targets, but the documented full-MSBuild-then-no-build workflow works.
  Evidence: the direct focused command failed in `QMLibrary.vcxproj` and `LiquidLiquidFunWrapper.vcxproj` with `MSB4278`; after `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Debug -p:Platform=x64 -p:PlatformTarget=x64 -v:m`, the no-build Pangolin filter passed 18/18.

## Decision Log

- Decision: Use a three-phase workflow: materialize detached candidates, let the user select candidates, then commit the selected candidates atomically.
  Rationale: Selection and cancellation must occur before any sequence mutation, and all seven formats need the same review behavior.
  Date/Author: 2026-08-28 / Codex

- Decision: Duplicate indicators compare candidates only to live, pre-import collections; they do not rename candidates or predict collisions among other candidates.
  Rationale: The dialog should explain collisions the current sequence already has without changing the user's source data. Final names must be calculated only at the commit boundary, where source order and the final selected subset are known.
  Date/Author: 2026-08-28 / Codex

- Decision: The committer is internal and UI-free.
  Rationale: It owns deterministic model rules that must be unit-tested without WPF dialogs, file dialogs, or network calls. The view model owns selection state; the service owns parsing/materialization.
  Date/Author: 2026-08-28 / Codex

- Decision: Excluding an imported parent collection clears the imported child's link rather than force-selecting the parent.
  Rationale: A user selection must be honored. A link to an excluded transient candidate would be invalid, while a cleared link leaves the selected child usable and does not alter existing sequence data.
  Date/Author: 2026-08-28 / Codex

- Decision: Keep the existing WinForms import-type chooser and existing Pangolin parser, import-mode prompt, color picker, and collection factory.
  Rationale: VIX-3992 changes the final collection-selection/commit stage, not source file formats or format-specific choices.
  Date/Author: 2026-08-28 / Codex

- Decision: Convert the import command to `TaskCommand` as part of the materialization refactor, but defer selection and commit wiring to Milestone 5.
  Rationale: Singing Faces now returns `Task<MarkCollectionImportResult>` and must be awaited; retaining a synchronous command would require prohibited blocking or an unobserved task. This is a necessary transitional seam, not completion of the selection workflow.
  Date/Author: 2026-08-28 / Codex

## Outcomes & Retrospective

Not implemented yet. At completion, record the final user-visible behavior, the test totals and build output, manual validation results, remaining gaps, and any deviations from the decisions above.

## Context and Orientation

A Mark Collection is a named collection of timestamped marks on a timed sequence. The active sequence exposes it as `ObservableCollection<IMarkCollection>`, named `MarkCollections` in the Marks Docker. A collection may be a phrase, word, or phoneme collection; `LinkedMarkCollectionId` stores the parent collection's stable identifier. `IsDefault` identifies the one active/default collection, and `IsVisible` determines the preferred default when Vixen must select one.

The WPF Marks Docker is under `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/`. `ViewModels/MarkDockerViewModel.cs` is created with the active sequence collection and owns the toolbar `ImportCollectionCommand`. It first opens the legacy WinForms `MarkCollectionImportDialog`, then dispatches to static methods in `Services/MarkImportExportService.cs`. The seven supported import types are Vixen 3, Bar Labels, Beat Labels, xTiming, Papagayo, Singing Faces, and Pangolin Beyond.

`MarkImportExportService` currently combines file/source UI, parsing, candidate construction, naming, default selection, and append operations. `MarkCollectionNameService` is the existing shared naming helper. Its `IsUniqueName(collections, name)` answers whether a user-facing name is unique among a set of live collections, and `GetUniqueName(desiredName, collections)` returns the desired name or a sequential ` - N` suffix. This plan must reuse those rules, not reimplement string comparison.

`Views/MarkExportWindow.xaml` and `ViewModels/MarkExportWindowViewModel.cs` are nearby Catel WPF dialog examples. The editor project uses SDK default file inclusion, so the new XAML and C# files normally require no `.csproj` edits; confirm this after creating them. The project exposes internals to `Vixen.Tests`, allowing tests for internal service, committer, and view-model types.

## Plan of Work

### Milestone 1: Record the delivery contract in Jira

Update VIX-3992 before code changes. Copy the purpose, seven formats, two-phase materialize/select/commit rule, cancellation invariant, duplicate/default/link rules, async constraints, out-of-scope list, and the automated/manual acceptance scenarios from this plan into the issue description. The issue must state that this selection dialog occurs after every source has been parsed/materialized and before the active sequence is changed. This milestone is independently verifiable by opening VIX-3992 and seeing an implementation-ready description and test plan.

When this milestone changes the Jira issue, record the link or comment identifier and timestamp in `Progress`. No repository file changes occur in this milestone.

### Milestone 2: Establish detached import contracts and refactor source materialization

Under `Forms/WPF/MarksDocker/Services`, add the following internal types. Keep the exact names because tests and the orchestrator will use them:

- `MarkCollectionImportType`, an enum with `Vixen3`, `BarLabels`, `BeatLabels`, `XTiming`, `Papagayo`, `SingingFaces`, and `PangolinBeyond`.
- `MarkCollectionImportStatus`, an enum with distinct `Succeeded`, `Cancelled`, and `Failed` values. `Cancelled` means the user dismissed a file/source dialog; `Failed` means parsing/loading failed after the service logged and displayed its existing format-specific error; neither may supply committable candidates.
- `MarkCollectionImportResult`, an immutable internal result containing the import type, status, and an ordered read-only list of detached `IMarkCollection` candidates. Provide factory construction or constructors that make success, cancellation, and failure unambiguous and ensure non-success results expose an empty candidate list.

Refactor `MarkImportExportService` so each loader returns `MarkCollectionImportResult` and never receives or mutates the active `ObservableCollection<IMarkCollection>`. It may continue showing its existing file picker, error UI, Pangolin mode prompt, and color picker. A successful return has fully-created, non-appended `MarkCollection` instances; a cancelled or failed return has no candidates. Do not call `AddUniqueCollection`, `SetDefaultCollection`, `GetUniqueName`, or alter `IsDefault` during materialization. Remove those now-commit-only helpers from loader flow.

Preserve format behavior while changing the destination as follows:

- Vixen 3: deserialize current and legacy files; migrate legacy collections into a local ordered list. Retain imported default flags for the committer to normalize later.
- Bar Labels and Beat Labels: create the same yellow/scheduled collections and marks locally, preserving source order and existing error behavior.
- xTiming: replace `LoadXTimingTracks(XmlDocument, ICollection<IMarkCollection>)` and `ProcessTiming(..., ICollection<IMarkCollection>)` with candidate-producing helpers. Build each timing node's local list first. Assign phrase/word/phoneme links from the local preceding candidate ids, never `collections.Last()`. If an empty layer is discarded, link only to candidates actually emitted; a child whose intended local parent is not emitted starts with no link rather than borrowing an unrelated collection.
- Papagayo: build each voice's phrase, word, and phoneme collections as a local chain before adding them to the returned list. Set `phrase -> no parent`, `word.LinkedMarkCollectionId = phrase.Id`, and `phoneme.LinkedMarkCollectionId = word.Id`; do not append while traversing voices. Keep the existing summary message only after successful source materialization, and do not claim the items were copied to a clipboard.
- Singing Faces: change the method to return `Task<MarkCollectionImportResult>`, await `GetSelectedSongTiming()`, parse it into xTiming candidates, and propagate cancel/failure as a non-success result. Do not use `async void`, `.Wait()`, `.Result`, or `Task.Run` around WPF, dialogs, or model construction.
- Pangolin Beyond: retain `PangolinBeyondMarkParser`, `PangolinBeyondImportMode`, and `PangolinBeyondMarkCollectionFactory`. Replace `TryAddPangolinBeyondMarks` with a candidate-returning helper (or rename it to reflect materialization) that returns factory-created collections without appending or naming them. Update existing tests away from append-specific behavior.

Use `using`/`await using` for disposable dialogs/streams where appropriate. Catch format errors at the same user-facing boundary as today, log the exception, show the existing error message, and return `Failed`; never leave a partially built candidate set visible to the caller. This milestone is complete when all seven source paths can be invoked in isolation and the target collection instance/count/defaults are unchanged after success, cancellation, or failure.

### Milestone 3: Add deterministic selection-independent commit logic

Add `Forms/WPF/MarksDocker/Services/MarkCollectionImportCommitter.cs` as an internal, UI-free class. Give it one operation with this effective contract:

    void Commit(ICollection<IMarkCollection> target, IEnumerable<IMarkCollection> candidates)

The caller supplies candidates already filtered by selection and in their original source order. The method must validate arguments before changing `target`. It then applies this exact algorithm, in order:

1. Materialize the candidate enumerable once into an ordered list. If it is empty, return without touching `target`.
2. Capture the existing target ids and selected candidate ids. For every selected candidate, retain `LinkedMarkCollectionId` only when it is either an existing target id or a selected candidate id. Otherwise clear it. This handles an excluded imported parent without force-selecting it.
3. Normalize defaults. If any target collection is already default, set every selected candidate's `IsDefault` to `false`. Otherwise retain `IsDefault = true` only for the first selected candidate that was imported as default. If no selected candidate was imported as default, set the first visible selected candidate as default; if none are visible, set the first selected candidate as default. Set all other selected candidates to false.
4. For each selected candidate in order, call `MarkCollectionNameService.GetUniqueName` against the progressively growing target, assign that final name, then append the candidate. This makes two selected candidates with the same source name deterministic (`Name`, then `Name - 2`) and preserves the source order.

Do not modify existing target names, existing default flags, existing links, serialized ids, or unselected candidates. The atomicity requirement means no exception-prone parsing or dialog work belongs here; all of that was completed before calling it. If argument validation fails, throw before mutation.

Add focused tests under `src/Vixen.Tests/Sequencer/` for: source-order append and sequential duplicate suffixes; target name collisions; preservation of an existing default; first selected imported default wins; visible then first-selected fallback defaults; imported links to target and selected ids preserved; links to excluded candidates cleared; no candidates/no commit leaves target identical; and no mutation on invalid input. Use the real `MarkCollection` type and assert object identity where proving target preservation matters.

### Milestone 4: Build the Catel collection-selection dialog

Add `ViewModels/MarkCollectionImportOptionViewModel.cs` and `ViewModels/MarkCollectionImportSelectionViewModel.cs`, both internal. Each option wraps exactly one detached candidate and exposes its display name, `IsIncluded` (initially true), and `HasDuplicateName`. Compute the duplicate indicator at construction using `MarkCollectionNameService.IsUniqueName(liveExistingCollections, candidate.Name)`. It must compare only against live existing collections and must not alter candidate names. Present an accessible text such as `Name already exists in this sequence` and an icon/warning glyph in addition to color so the duplicate signal is not color-only.

The selection view model receives the live target collection set and the successful materialized candidates. It exposes an ordered observable option list, `SelectedOption` for ListBox focus/selection, `HasIncludedOptions`, and Catel `OkCommand`, `CancelCommand`, and `ToggleSelectedOptionCommand`. `OkCommand` must be disabled whenever no option is included and must call `SaveAndCloseViewModelAsync()` only when at least one remains. Cancel calls `CancelAndCloseViewModelAsync()` and never changes candidates or target. The space-toggle command toggles the current `SelectedOption`, raises/invalidates the OK command's can-execute state, and does nothing when no row is selected.

Add `Views/MarkCollectionImportSelectionWindow.xaml` as a Catel `Window` and `MarkCollectionImportSelectionWindow.xaml.cs` containing only the partial class, constructor, and `InitializeComponent()`. Follow the export dialog's theme resource and tool-window conventions. The layout must have a textual instruction, a `ListBox` bound to the options, checkbox rows bound two-way to `IsIncluded`, candidate name, duplicate text/icon, and `SelectedItem` bound to `SelectedOption`. Ensure normal ListBox arrow, Home, End, and Page Up/Page Down navigation is not intercepted. Bind Space to `ToggleSelectedOptionCommand` when the list has focus (including the selected row, not only a focused CheckBox). Make OK the default button, Cancel an `IsCancel` button, support Escape through Catel's escape-close behavior, and bind the OK button's enabled state to command can-execute. Do not add logic to code-behind.

Add view-model tests for all-selected initialization, collision indication against a live collection (including the name service's case/trim behavior), selected-row space toggle, OK disabled/enabled transitions, save result with an included subset, and cancellation. Tests must show that selection/cancellation does not mutate the live collection set or candidate names.

### Milestone 5: Orchestrate materialize, select, then commit

In `ViewModels/MarkDockerViewModel.cs`, replace the misspelled synchronous `Command` backing field/property implementation with a `TaskCommand` backed by `async Task ImportCollection()`. Preserve the existing legacy `MarkCollectionImportDialog` as the first format chooser and preserve its individual selection predicates. If the chooser is cancelled, return before any loader runs.

Map the selected chooser value to the corresponding `MarkCollectionImportType`, await the appropriate `MarkImportExportService` materialization method (including Singing Faces), and return immediately unless its status is `Succeeded` and it has candidates. Resolve Catel's `UIVisualizerService` through the existing dependency resolver, construct `MarkCollectionImportSelectionViewModel` with the current `MarkCollections` and returned candidates, and await `ShowDialogAsync`. If the dialog result is not true, return. Read selected candidate instances from the view model in source order and pass them to `MarkCollectionImportCommitter.Commit(MarkCollections, selectedCandidates)`.

There must be no direct `MarkCollections.Add` call in `MarkDockerViewModel.ImportCollection`, no loader append call, and no model mutation before selection-dialog OK. Do not wrap UI/model work in `Task.Run`; do not block with `.Wait()` or `.Result`; and do not add `async void`. Let exceptions not already converted to a `Failed` import result be logged/shown consistently at the service boundary before the dialog is reached.

Because this changes public/protected C# command/import members if their accessibility remains public, read and apply the project version of `.agents/skills/csharp-docs/SKILL.md` during implementation. Update XML summaries, parameter documentation, return documentation, remarks, and exception documentation for every affected public or protected API. If the new contracts can safely be internal, keep them internal to minimize public API surface, but still correct stale documentation on changed existing public members.

### Milestone 6: Validate end to end and close the Jira loop

First run focused test filters for the new import selection, committer, existing Pangolin parser/factory, and existing name service tests. Then use the repository-required full-MSBuild test build and no-build test execution from the repository root:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)/"

Also run:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    git diff --check

Expect zero build errors, zero failing tests, and no whitespace errors. If a build is blocked by a known missing native toolchain or an application process holding output files, record exact output in `Surprises & Discoveries`, resolve the local condition safely, and rerun; do not misrepresent a blocked build as success.

Manually run a Debug build and check all seven imports. For each format, cancel its file/source dialog and confirm no sequence collections/defaults/links changed; complete it and verify the universal selection window appears before changes. Use a source that creates several candidates to confirm mouse checkbox toggling, arrow/Home/End/Page navigation, selecting a row and pressing Space, Enter/OK, Escape/Cancel, the disabled OK state after clearing all rows, and duplicate wording/icon. Import a phrase/word/phoneme source, exclude its phrase parent, accept, and confirm the word/phoneme collection remains selected but has no invalid parent link. Import colliding names and confirm names are unchanged in the dialog then become sequentially unique only after OK. Verify defaults for a sequence that already has one, an empty sequence with an imported default, and an empty sequence with no imported default but both visible and hidden candidates.

Finally update VIX-3992's description if implementation discoveries changed requirements, and add a Jira comment with test commands/results and the manual scenarios completed. Update the plan's living sections and add a dated change note at the bottom.

## Concrete Steps

All commands below run from `C:\Dev\Vixen` in PowerShell. Before editing, inspect the current worktree so unrelated user changes are preserved:

    git status --short
    rg -n "ImportCollection|ImportSingingFacesTracks|LoadXTimingTracks|ProcessTiming|TryAddPangolinBeyondMarks|AddUniqueCollection" src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker

Create the types and tests described in Milestones 2 through 5 with tabs and LF line endings as required by `src/.editorconfig`. Run focused tests as each independently-testable layer is complete. Use the actual fully qualified test class names created by the implementation, for example:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-restore --filter "FullyQualifiedName~MarkCollectionImportCommitter|FullyQualifiedName~MarkCollectionImportSelection|FullyQualifiedName~PangolinBeyondMarkImport|FullyQualifiedName~MarkCollectionNameService"

Expected focused output includes a nonzero total and `Failed: 0`. If the filter matches zero tests, correct the test name/filter and retry rather than accepting an empty run.

After all changes, use the Milestone 6 full commands. Record concise successful transcripts in `Artifacts and Notes`, including passed/failed totals and any environmental warning that does not affect success.

## Validation and Acceptance

The feature is accepted only when all of the following are true:

- Every one of Vixen 3, Bar Labels, Beat Labels, xTiming, Papagayo, Singing Faces, and Pangolin Beyond fully produces detached candidates before the shared selection dialog is shown.
- Cancelling the format picker, any source-specific dialog, the selection dialog, or an import that fails leaves the target collection count, object identities, names, default flags, and links unchanged.
- The dialog initially checks all rows, visibly and textually identifies a candidate name collision with existing live collections, does not pre-rename candidates, has a selected row, supports mouse and the stated keyboard navigation, makes Space toggle the selected row, makes Enter accept enabled selections, and makes Escape cancel.
- OK is unavailable with no checked rows; accepting a subset appends exactly that subset in original source order.
- The committer generates names sequentially against the target as it grows, preserves links only to existing or selected ids, clears links to excluded imported parents, and applies the stated single-default rules.
- Automated tests cover selection state, collision indication, keyboard command, cancel/no mutation, commit ordering, names, defaults, links, and candidate generation for every format. Existing Pangolin tests are adapted to materialization rather than immediate append.

## Idempotence and Recovery

The code changes are additive/refactoring-only and create no data migration. It is safe to repeat builds and tests. A cancelled selection deliberately leaves detached candidates to be garbage-collected and must leave the sequence untouched. If a source importer fails after opening a file, return `Failed` with no candidates and retain current error reporting; retry by rerunning the import from the toolbar after correcting the source file.

Do not delete or rewrite user sequence data to test this feature. Use a disposable test sequence or close without saving after manual scenarios. If a test run changes build output, use normal build cleanup only; never reset unrelated worktree changes.

## Artifacts and Notes

At implementation time, add compact evidence such as:

    Focused importer/selection/committer tests: Passed: <N>, Failed: 0.
    Full Vixen.Tests run: Passed: <N>, Failed: 0.
    Debug rebuild: 0 Error(s).
    git diff --check: no output.

The essential flow to preserve is:

    legacy format chooser
        -> source-specific picker/parser/materializer (detached candidates)
        -> MarkCollectionImportSelectionWindow (user subset)
        -> MarkCollectionImportCommitter (links, default, sequential final names, append)

No arrow in this flow before the committer may change `MarkCollections`.

## Interfaces and Dependencies

Use Catel `TaskCommand`, `ViewModelBase`, `Command`, `UIVisualizerService`, and its `ShowDialogAsync` result for the WPF/MVVM workflow. Use the existing Marks Docker theme resource and `MarkExportWindow` as the window convention. No new NuGet package, project reference, file format, serialized identifier, or replacement import-type chooser is required.

The final internal API surface must include `MarkCollectionImportType`, `MarkCollectionImportStatus`, `MarkCollectionImportResult`, `MarkCollectionImportOptionViewModel`, `MarkCollectionImportSelectionViewModel`, and `MarkCollectionImportCommitter` at the paths described above. `MarkCollectionImportCommitter` must accept existing target collections plus selected detached candidates and must not depend on WPF, file dialogs, `UIVisualizerService`, or the import service. `MarkImportExportService` materializers must return `Task<MarkCollectionImportResult>` where awaiting is necessary (Singing Faces) and may return a synchronous `MarkCollectionImportResult` for wholly synchronous format paths; the view-model workflow awaits via `Task.FromResult`/a uniform async wrapper without blocking.

Out of scope: changing import file formats, changing serialized collection ids, replacing `MarkCollectionImportDialog`, merging duplicate collections, or redesigning Mark Collection relationships.

2026-08-28 / Codex: Created this ExecPlan from the VIX-3992 implementation handoff after inspecting the current Marks Docker import dispatch, all loader mutation seams, xTiming/Papagayo relationship construction, the Pangolin test suite, and the VIX-3947 shared naming behavior. No implementation or Jira mutation was performed while creating the plan.

2026-08-28 / Codex: Completed Milestone 1 by replacing VIX-3992's issue description with the finalized user-facing summary, scope, acceptance criteria, and test plan. This records the implementation contract in Jira without exposing repository-internal design details.

2026-08-28 / Codex: Completed Milestone 2 by introducing result/type/status contracts and refactoring all seven import paths into detached candidate materializers. xTiming and Papagayo now build links locally, Pangolin no longer assigns names/defaults during materialization, and Singing Faces is task-returning and awaited. The final selection/commit orchestration remains intentionally pending Milestone 5. The documented full-MSBuild test build completed, then the focused no-build Pangolin/xTiming filter passed 18/18; direct `dotnet test` remains unsuitable here because it cannot build the C++/CLI dependencies.

2026-08-28 / Codex: Completed Milestone 3 by adding `MarkCollectionImportCommitter`, a UI-free internal commit boundary. It validates the input before mutation; preserves only valid existing/selected links; applies the specified single-default rule; then resolves names against the progressively growing target and appends in source order. Focused committer and materializer tests passed 26/26 after the full MSBuild test build.

2026-08-28 / Codex: Completed Milestone 4 by adding the Catel collection-selection dialog and its two internal view models. Every detached candidate is initially selected; live-sequence name collisions display text and a warning glyph without renaming candidates; ListBox Space toggles only the selected row while normal navigation remains native. Selection captures an ordered accepted subset and never mutates either candidates or live collections. The full MSBuild test build succeeded, and focused selection/committer/materializer tests passed 32/32.

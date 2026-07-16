# VIX-3947 Prevent Duplicate Mark Collection Names

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Keep this document self-contained when revising it so a future contributor can implement the work with only this file and the current working tree.

## Purpose / Big Picture

VIX-3947 fixes a user-visible ambiguity in the Timed Sequence Editor: the Marks Docker currently allows more than one Mark Collection to have the same display name. Effects and other editor workflows present mark collections by name, so duplicate names make it impossible for the user to reliably choose the intended collection; the first matching collection can be selected or used instead of the one the user expected.

After this change, the Marks Docker will assign unique default names when adding collections, inline rename will refuse duplicate names before committing them, imports and editor-created collections will avoid creating new duplicate names, and existing sequences with duplicates will be repaired when they are loaded. The user will see a summary after the sequence finishes loading, for example `Renamed duplicate Mark Collections: Beat Marks -> Beat Marks - 2; Beat Marks -> Beat Marks - 3`.

## Progress

- [x] (2026-07-16 00:00Z) Read `.agents/PLANS.md` and confirmed this work should be captured as an ExecPlan in `docs/plans`.
- [x] (2026-07-16 00:00Z) Inspected the Marks Docker WPF view and view models in `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker`.
- [x] (2026-07-16 00:00Z) Inspected Timed Sequence Editor sequence load, mark collection hooks, mark creation, and duplicate-name warning paths in `TimedSequenceEditorForm.cs` and `Form_AddMultipleEffects.cs`.
- [x] (2026-07-16 00:00Z) Identified the effect property converter `src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs` as one reason duplicate names are ambiguous: it exposes only names, not collection ids.
- [x] (2026-07-16 14:32Z) Implemented shared mark collection naming helper and focused unit tests.
- [x] (2026-07-16 14:42Z) Prevented duplicate creation and import paths by applying `MarkCollectionNameService.GetUniqueName` in the Marks Docker add command, editor-created collection helper, phoneme child collection creation, and Mark import add paths.
- [ ] Add inline rename validation and duplicate-name visual feedback.
- [ ] Repair duplicate mark collection names on sequence load and show a post-load summary.
- [ ] Update Jira issue VIX-3947 with the final requirements, acceptance criteria, and automated and manual test plan.
- [ ] Run targeted unit tests and a build or practical editor validation. Completed: `MarkCollectionNameService` targeted tests pass; broader `Sequencer` test slice passes. Remaining: practical editor validation after UI/editor integration. Timed Sequence Editor project build is currently blocked in this shell by missing x86 apphost/native dependency setup.

## Surprises & Discoveries

- Observation: `MarkDockerViewModel.AddCollection()` creates `new MarkCollection()` without assigning a name, so every toolbar-created collection starts with the `MarkCollection` constructor default name, `Mark Collection`.
  Evidence: `src/Vixen.Modules/App/Marks/MarkCollection.cs` sets `Name = @"Mark Collection";`; `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkDockerViewModel.cs` adds the new instance directly.

- Observation: Inline rename currently writes to the model `Name` while editing, so validation cannot be a final commit-only check unless the edit buffer is separated from the model.
  Evidence: `MarkCollectionView.xaml` binds the edit `TextBox` directly to `{Binding Name}` and sets `utils:TextBoxExtender.CommitOnTyping="true"`; `MarkCollectionViewModel.Name` is a `[ViewModelToModel("MarkCollection")]` property.

- Observation: There is already a downstream warning for duplicate mark names in the Add Multiple Effects workflow, which confirms the bug affects effect placement behavior today.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs` shows a warning when checked Beat Mark collections contain duplicate names.

- Observation: `TimedSequenceEditorForm.GetOrAddNewMarkCollection()` already uses name lookup and returns the first matching collection, so existing duplicate names can route new marks or generated phoneme collections to the wrong collection.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` uses `_sequence.LabeledMarkCollections.FirstOrDefault(mCollection => mCollection.Name == name)`.

- Observation: `Vixen.Core` already has a generic `NamingUtilities.Uniquify` helper and tests, but it only works on a `HashSet<string>` and does not return rename records or support excluding the current collection during inline rename validation.
  Evidence: `src/Vixen.Core/Utility/NamingUtilities.cs` and `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs`.

- Observation: The duplicate repair helper also needs to handle blank persisted names without creating a duplicate of an existing `Mark Collection` name.
  Evidence: `MarkCollectionNameServiceTests.RenameDuplicates_BlankNameUsesAvailableDefaultName` covers a blank name plus an existing `Mark Collection` and expects the blank name to become `Mark Collection - 2`.

- Observation: Building the Timed Sequence Editor project in this environment currently fails before compiling the editor project because native dependency projects require the x86 .NET apphost pack.
  Evidence: `msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj /t:Build /p:Configuration=Debug /p:UseAppHost=false /m` fails in `QMLibrary.vcxproj` and `LiquidLiquidFunWrapper.vcxproj` with `NETSDK1145` for missing `Microsoft.NETCore.App.Host.win-x86`.

## Decision Log

- Decision: Put the core naming and duplicate-repair rules in a shared helper under `src/Vixen.Core/Marks`, not in Marks Docker view code.
  Rationale: The same rules are needed by the Marks Docker, editor mark creation, imports, and sequence load repair. `Vixen.Core` already owns `IMarkCollection`, so a helper operating on that interface avoids depending on WPF or the Timed Sequence Editor UI assembly.
  Date/Author: 2026-07-16 / Codex

- Decision: Treat names as duplicates after trimming leading and trailing whitespace and comparing case-insensitively.
  Rationale: These names are user-facing labels in dropdowns and rows. `Beat Marks`, `beat marks`, and ` Beat Marks ` are not meaningfully distinguishable to users selecting an effect parameter by name.
  Date/Author: 2026-07-16 / Codex

- Decision: Use the suffix format ` - 2`, ` - 3`, and so on, preserving the first existing instance of a name and renaming later duplicates.
  Rationale: The issue explicitly requests `- 2` naming. Preserving the first instance minimizes disruption to existing sequences while making all later names selectable.
  Date/Author: 2026-07-16 / Codex

- Decision: Show the duplicate-repair summary from the same post-load completion path that currently shows deprecated-element warnings.
  Rationale: `LoadSequence` renders effects asynchronously and already uses `Task.Factory.ContinueWhenAll` to run `CheckDeprecatedEffects()` after the sequence is visible and rendering suppression is lifted. This is the right user-visible point for a load summary.
  Date/Author: 2026-07-16 / Codex

## Outcomes & Retrospective

Milestone 1 added `src/Vixen.Core/Marks/MarkCollectionNameService.cs` and `src/Vixen.Core/Marks/MarkCollectionRenameRecord.cs`, plus focused tests in `src/Vixen.Tests/Sequencer/MarkCollectionNameServiceTests.cs`. The helper validates unique names, generates ` - 2` and later suffixes, excludes a current collection id for rename validation, and repairs duplicate names while returning rename records. The test project now references `src/Vixen.Modules/App/Marks/Marks.csproj` so tests use the real `MarkCollection` implementation.

The targeted command `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~MarkCollectionNameService --no-restore` passed with 12 tests. Existing package and compiler warnings were present, including the known `LiteDB` vulnerability warning and pre-existing nullable/obsolete warnings outside this change.

Milestone 2 updated future creation paths so they no longer add duplicate Mark Collection names. The Marks Docker add command now names repeated new collections as `Mark Collection`, `Mark Collection - 2`, and later suffixes. The Timed Sequence Editor's mark-creation helper now uses case-insensitive trimmed lookup when reusing an existing named collection and a separate unique-create helper when a linked phoneme/word collection must be newly created. Mark import paths now add imported Vixen, migrated legacy, Audacity, xTiming, Papagayo, and Singing Faces collections through a shared `AddUniqueCollection` helper that assigns a unique name before adding the collection.

Validation for milestone 2: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~MarkCollectionNameService --no-restore` passed with 12 tests, and `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~Sequencer --no-restore` passed with 67 tests. `dotnet build src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj --no-restore`, `dotnet build ... -p:BuildProjectReferences=false`, and MSBuild project builds were attempted but could not complete in this shell because required native/x86 apphost dependencies are missing before the Timed Sequence Editor project can be compiled.

## Context and Orientation

A Mark Collection is a named set of marks stored on a timed sequence. A mark is a timestamp or labeled time range used by the Sequencer and by effects. The active sequence exposes these collections through `TimedSequence.LabeledMarkCollections`, an `ObservableCollection<IMarkCollection>`.

The user-facing Marks Docker is a WPF panel hosted inside a WinForms dock window. The host is `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Marks.cs`; it creates a `MarkDockerViewModel` with `sequence.LabeledMarkCollections` and displays `MarkDockerView`. The toolbar add button in `MarkDockerView.xaml` calls `MarkDockerViewModel.AddCollectionCommand`. Each row is displayed through `MarkCollectionView.xaml` and edited through `MarkCollectionViewModel`.

The current row edit flow is split between `BeginEdit`, `DoneEditing`, and `CancelEditing` in `MarkCollectionViewModel`. Double-clicking the read-only name sets `IsEditing = true`; the `TextBox` appears; pressing Enter or losing keyboard focus calls `DoneEditing`; pressing Escape calls `CancelEditing`. The problem is that the edit `TextBox` is currently bound directly to the model-backed `Name`, so duplicate names can be written before `DoneEditing` runs.

The Timed Sequence Editor form is `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`. Its `Sequence` property accepts a `TimedSequence`, calls `SequenceAdded()`, and later `TimedSequenceEditorForm_Shown()` calls `LoadSequence(_sequence)`. `LoadSequence` queues timeline element loading, then its `Task.Factory.ContinueWhenAll` callback clears loading state, renders rows, and calls `CheckDeprecatedEffects()`. This post-load callback is where the duplicate-rename summary should be shown.

Effects select Mark Collections by name through `src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs`, which returns the collection names as standard values. Some effect implementations store a selected name, so fixing future duplicate creation and repairing old duplicate names is the lowest-risk mitigation for VIX-3947. This plan does not redesign effects to store Mark Collection ids.

## Plan of Work

First, add a shared naming helper in `src/Vixen.Core/Marks/MarkCollectionNameService.cs`. Because this adds a public or protected C# API if the helper must be consumed outside `Vixen.Core`, use the project `csharp-docs` skill during implementation and include XML documentation on public members. The helper should provide these behaviors:

- Normalize a name for comparison by trimming whitespace and comparing with `StringComparer.CurrentCultureIgnoreCase` or `StringComparer.OrdinalIgnoreCase`; choose one and use it consistently. Prefer `OrdinalIgnoreCase` unless an existing repository convention points elsewhere.
- Determine whether a proposed name is unique among a set of `IMarkCollection` instances, excluding an optional current collection id for rename validation.
- Generate a unique name from a desired base name using `Base`, `Base - 2`, `Base - 3`, etc. The first available candidate wins. If `Base - 2` already exists, skip to `Base - 3`.
- Repair duplicates in collection order. The first `Beat Marks` remains `Beat Marks`; the second becomes `Beat Marks - 2`; the third becomes `Beat Marks - 3`. If any generated name is already occupied, use the next available suffix. Return a list of rename records with collection id, old name, and new name for the load summary.

Next, update creation paths. In `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkDockerViewModel.cs`, change `AddCollection()` so it assigns a unique name before adding:

    var name = MarkCollectionNameService.GetUniqueName("Mark Collection", MarkCollections);
    var mc = new MarkCollection { Name = name };

Keep the existing default-selection behavior for the first collection. This should make repeated toolbar clicks produce `Mark Collection`, `Mark Collection - 2`, `Mark Collection - 3`.

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` in `GetOrAddNewMarkCollection(Color color, string name = "New Collection")`. This method has two different semantics today: it returns an existing collection with the requested name if one exists, otherwise creates one. Preserve that behavior when the requested name is already unique, but do not allow creation of a duplicate. When creating a new collection, set its name with the shared unique-name helper. Also update the phoneme helper path that builds names like `parent.Name + " Words"` or `parent.Name + " Phonemes"` so generated child collections receive unique names if an unrelated collection already uses the computed name.

Update import paths in `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services/MarkImportExportService.cs`. Any method that appends collections from Vixen 3 marks, Audacity, xTiming, Papagayo, or Singing Faces should call the shared unique-name helper before `collections.Add(...)`. Preserve linked relationships by setting unique names before adding the linked phrase, word, and phoneme collections, but do not change ids; linked relationships use `LinkedMarkCollectionId`, not names.

Then rework inline rename. In `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs`, keep the model-backed `Name` property for display, but introduce a separate edit buffer such as `EditableName`, plus `HasNameValidationError` and `NameValidationMessage`. `BeginEdit()` should copy `Name` to `_textHold` and `EditableName`, clear validation state, and set `IsEditing = true`. `CancelEditing()` should restore `EditableName` from `_textHold`, clear validation state, and leave `Name` unchanged.

Change `DoneEditing()` so it validates `EditableName.Trim()` before writing to `Name`. If the trimmed name is empty, keep the existing behavior of reverting to `_textHold` and ending edit mode. If the trimmed name duplicates another collection name after excluding the current `MarkCollection.Id`, set `HasNameValidationError = true`, set `NameValidationMessage` to a concise message like `A Mark Collection named "Beat Marks" already exists.`, keep `IsEditing = true`, and do not update `Name`. If the name is valid and different, assign `Name = trimmedName`, clear validation state, set `IsEditing = false`, and mark the view model dirty.

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Views/MarkCollectionView.xaml` so the edit `TextBox` binds to `EditableName` with `UpdateSourceTrigger=PropertyChanged`, not directly to `Name`. Add inline visual feedback without code-behind: give the `TextBox` a red or warning border when `HasNameValidationError` is true and show `NameValidationMessage` as a tooltip or a small adjacent `TextBlock` visible only during editing. Keep Enter mapped to `DoneEditingCommand` and Escape mapped to `CancelEditingCommand`. Avoid modal message boxes for this rename validation; the user asked for inline duplicate feedback.

Finally, add load-time duplicate repair. In `TimedSequenceEditorForm.cs`, add a private field such as `_markCollectionRenameRecords` to store records returned by the helper. In the `Sequence` property setter, after `_sequence = (TimedSequence)value` and before `SequenceAdded()`, call the duplicate-repair helper on `_sequence.LabeledMarkCollections`. Store the returned records. This timing repairs data before the Marks Docker and timeline subscribe to collection changes, avoiding a burst of per-property change notifications.

Because `LoadSequence()` calls `SequenceNotModified()` for existing files, add explicit dirty-state handling after the existing modified/not-modified decision. If `_markCollectionRenameRecords.Any()`, call `SequenceModified()` so the user can save the repaired names. In the `Task.Factory.ContinueWhenAll` callback, after `TimelineControl.grid.RenderAllRows()` and near `CheckDeprecatedEffects()`, call a new method `ShowMarkCollectionRenameSummary()`. Model it after `CheckDeprecatedEffects()` by using `InvokeRequired` to marshal onto the UI thread. The message should be information-level, concise, and include every rename:

    The following duplicate Mark Collections were renamed so effects can select them correctly:

    Beat Marks -> Beat Marks - 2
    Beat Marks -> Beat Marks - 3

If both deprecated-effect warnings and mark-rename summaries are present, show the rename summary first because it explains a data change that just occurred.

Before closing the implementation, update Jira issue VIX-3947 so the tracker matches the implemented behavior. The Jira update should include the user-facing requirements, acceptance criteria, and test plan from this ExecPlan. Requirements should cover unique names for new Marks Docker collections, duplicate rejection during inline rename, unique naming for imported or editor-created collections, load-time repair of existing duplicate names using ` - 2`, ` - 3`, and later suffixes, and a post-load summary when repairs occur. Acceptance criteria should describe the observable Marks Docker, rename, import, load-repair, effect dropdown, and modified-sequence behaviors. The test plan should list the automated unit tests for the shared naming helper and the manual Timed Sequence Editor checks described in this plan.

## Concrete Steps

Work from `C:\Dev\Vixen`.

1. Read the Catel and C# docs skills before editing because this change touches WPF view models and adds or modifies public C# APIs:

    Get-Content .agents/skills/catel-mvvm/SKILL.md
    Get-Content .agents/skills/csharp-docs/SKILL.md

2. Add `src/Vixen.Core/Marks/MarkCollectionNameService.cs` with the shared helper and XML documentation. Keep it free of UI dependencies. It should depend only on `System`, `System.Collections.Generic`, `System.Linq`, and `Vixen.Marks`.

3. Add tests in `src/Vixen.Tests/Sequencer/MarkCollectionNameServiceTests.cs`. Use xUnit v3. Build collections with `VixenModules.App.Marks.MarkCollection` and assert the helper behavior:

    - `GetUniqueName` returns `Mark Collection` when no collection exists.
    - `GetUniqueName` returns `Mark Collection - 2` when `Mark Collection` exists.
    - `GetUniqueName` skips occupied suffixes.
    - `IsUniqueName` rejects case-only and whitespace-only duplicates.
    - `RenameDuplicates` preserves the first name and renames later duplicates in order.
    - `RenameDuplicates` returns records containing old and new names.

4. Update `MarkDockerViewModel.AddCollection()` to use `MarkCollectionNameService.GetUniqueName("Mark Collection", MarkCollections)`.

5. Update `MarkCollectionViewModel` and `MarkCollectionView.xaml` to use `EditableName` for editing and inline duplicate feedback. Follow the existing Catel command style and avoid adding code-behind handlers.

6. Update `TimedSequenceEditorForm.GetOrAddNewMarkCollection()` and phoneme child collection creation to use the helper when creating names. Do not change existing selection or mark-adding behavior except to avoid duplicate names.

7. Update `MarkImportExportService` so every collection added by an import path is renamed to a unique name before it enters the target sequence collection.

8. Update `TimedSequenceEditorForm.Sequence` and `LoadSequence()` to repair duplicates, mark the sequence modified if repair occurred, and show the summary after load completion.

9. Update Jira issue VIX-3947 with the requirements, acceptance criteria, and test plan. Use the project `jira` skill before making the Jira change. The issue body or an implementation comment should include:

    Requirements:
    - New Marks Docker collections are assigned unique names using `Mark Collection`, `Mark Collection - 2`, `Mark Collection - 3`, and so on.
    - Inline rename cannot commit a name that duplicates another Mark Collection after trimming whitespace and comparing case-insensitively.
    - Imported and editor-created Mark Collections are renamed to unique names before they are added to the sequence.
    - Existing duplicate Mark Collections are repaired on sequence load; the first instance keeps its name, the second receives ` - 2`, the third receives ` - 3`, and occupied suffixes are skipped.
    - The user receives a summary after the sequence finishes loading when duplicate collections were renamed.

    Acceptance criteria:
    - Repeated Marks Docker add actions produce unique collection names.
    - Attempting to inline rename a collection to another collection's name leaves edit mode active, shows inline validation feedback, and does not change the model name.
    - Loading a sequence with duplicate Mark Collection names renames only later duplicates, marks the sequence modified, and displays a summary after the timeline is visible.
    - Effects that expose Mark Collection dropdowns show unique names after duplicate repair.

    Test plan:
    - Automated: run `MarkCollectionNameServiceTests` for unique-name generation, duplicate detection, suffix skipping, case-insensitive comparison, and ordered duplicate repair records.
    - Automated: run the broader sequencer test slice to catch regressions around timeline/editor behavior.
    - Manual: add three Marks Docker collections, attempt duplicate inline rename, load a sequence with duplicate collections, verify the post-load summary and modified state, and verify an effect Mark Collection dropdown shows unique names.

10. Run targeted tests:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~MarkCollectionNameService --no-restore

11. Run a broader sequencer-related test slice:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~Sequencer --no-restore

12. If time allows, run the full test project:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore

## Validation and Acceptance

Automated acceptance starts with the new `MarkCollectionNameServiceTests`. The tests must fail before the helper exists and pass after implementation. The targeted command should report all `MarkCollectionNameService` tests passed:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~MarkCollectionNameService --no-restore

Manual acceptance in the Timed Sequence Editor:

Open a sequence, show the Marks Docker, and click the new collection toolbar button three times. The collection names should be `Mark Collection`, `Mark Collection - 2`, and `Mark Collection - 3`. There should be no duplicate `Mark Collection` rows.

Double-click a collection name to inline edit it. Type the exact name of another collection and press Enter. The edit should remain active, the model name should not change, and inline feedback should state that a Mark Collection with that name already exists. Press Escape and the original name should be restored. Type a unique name and press Enter; the edit should commit.

Create or load a sequence file that contains three Mark Collections with the same name, for example `Beat Marks`, `Beat Marks`, and `Beat Marks`. Open it in the Timed Sequence Editor. After the sequence finishes loading and the timeline is visible, the user should see a summary that the second and third collections were renamed to `Beat Marks - 2` and `Beat Marks - 3`. The sequence should be marked modified so saving persists the repaired names.

Open an effect that supports Mark Collections. Its Mark Collection dropdown should show unique names after load repair, so the user can distinguish and select each collection by name.

Jira acceptance requires VIX-3947 to contain or be updated with the implementation requirements, acceptance criteria, and test plan before the work is considered ready to close. The Jira content should be specific enough that another developer or tester can verify the behavior without reading this ExecPlan.

## Idempotence and Recovery

The duplicate-repair helper must be idempotent. Running it on already repaired collections should return no records and make no changes. Running it on a partially repaired set should only rename names that still collide.

The implementation should not delete or merge any Mark Collections or Marks. It only renames collection labels. If a user dislikes the generated names, they can use the now-validated inline rename workflow to choose unique names manually.

If load-time repair is implemented and then needs to be backed out, remove only the call from the `Sequence` setter and the post-load summary method. The standalone helper and Marks Docker prevention remain useful and safe because they only affect future creation and rename attempts.

## Artifacts and Notes

Key code locations from research:

    src/Vixen.Modules/App/Marks/MarkCollection.cs
        Constructor default: Name = @"Mark Collection"

    src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkDockerViewModel.cs
        AddCollection() currently creates new MarkCollection() and adds it directly.

    src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs
        Name is [ViewModelToModel("MarkCollection")].
        DoneEditing() currently only rejects empty names.

    src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Views/MarkCollectionView.xaml
        Editable TextBox currently binds Text directly to Name.

    src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs
        LoadSequence() uses Task.Factory.ContinueWhenAll and calls CheckDeprecatedEffects() after rendering.
        GetOrAddNewMarkCollection() uses FirstOrDefault(mCollection => mCollection.Name == name).

    src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs
        Effect property dropdowns expose only markCollection.Name strings.

    src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs
        Existing duplicate-name warning says duplicate Beat Mark collections may produce unexpected results.

## Interfaces and Dependencies

Add a public helper in `Vixen.Core` so both the editor module and tests can consume the same rules. Exact names may be adjusted during implementation, but the final API should provide equivalent behavior:

    namespace Vixen.Marks
    {
        public sealed class MarkCollectionRenameRecord
        {
            public Guid CollectionId { get; }
            public string OldName { get; }
            public string NewName { get; }
        }

        public static class MarkCollectionNameService
        {
            public static bool IsUniqueName(IEnumerable<IMarkCollection> collections, string proposedName, Guid? excludedCollectionId = null);

            public static string GetUniqueName(string desiredName, IEnumerable<IMarkCollection> collections, Guid? excludedCollectionId = null);

            public static IReadOnlyList<MarkCollectionRenameRecord> RenameDuplicates(IList<IMarkCollection> collections);
        }
    }

`RenameDuplicates` accepts an ordered collection because collection order determines which duplicate keeps the original name. If the implementation uses `IEnumerable<IMarkCollection>`, it must still enumerate once into a list before mutating names so order and conflict checks remain stable.

No new NuGet packages are required. No solution file changes are expected unless tests need an additional project reference; prefer placing the helper in `Vixen.Core` to avoid adding a new test dependency on the WinForms/WPF Timed Sequence Editor module.

## Revision Note

Initial plan created for VIX-3947 after inspecting the Marks Docker, Timed Sequence Editor load path, mark collection model, and effect Mark Collection name converter.

2026-07-16: Added an explicit implementation step to update Jira issue VIX-3947 with requirements, acceptance criteria, and automated/manual test plan so the tracker stays aligned with the planned behavior.

2026-07-16: Completed the first implementation slice by adding the shared Mark Collection naming service, rename record DTO, unit tests, and the test project reference to the Marks module.

2026-07-16: Completed the second implementation slice by wiring unique Mark Collection naming into Marks Docker creation, editor-created collections, linked phoneme collection creation, and import add paths. Recorded passing targeted tests and the environment-specific Timed Sequence Editor build blocker.

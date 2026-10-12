# VIX-4019: Copy a State Item in both State editors


This ExecPlan is a living document maintained in accordance with `.agents/PLANS.md`. Keep `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` current. This document contains the design and the active execution plan; authoring it does not implement the feature or authorize immediate Jira updates.

## Purpose / Big Picture


First align both State Item grids on the original State Property Setup behavior: clicking a sortable column header applies an ascending sort once, persists that order, and releases the active sort so Move Up and Move Down can change the displayed order afterwards. Every new Add appends at the bottom, including after sorting. Complete and review this prerequisite before any naming or Copy work.

Users will then be able to select one State Item and click Copy in either Display Setup's State Property Setup window or the Custom Prop Editor's State tab. The copy will have a new name, the same color and assignments, and independent editable data. It will use the same insertion, selection, validation, and save behavior as Add and append at the bottom even after a column sort. This saves users from rebuilding nearly identical assignments by hand.

The source issue is [VIX-4019 — Ability to duplicate a State Item in a State Definition](https://vixenlights.atlassian.net/browse/VIX-4019). Its description was retrieved on 2026-10-12; it was Accepted and had no attachments, comments, or linked issues. The user resolved both planning questions: Copy is disabled unless exactly one row is selected, and sorting must behave as existing Add does in the same situation.

## Progress


- [x] (2026-10-12 15:41Z) Read the requested project skill and `.agents/PLANS.md`; retrieve VIX-4019 and inspect existing State requirements.
- [x] (2026-10-12 15:41Z) Resolve selection and sorting questions; inspect production models, commands, views, and existing tests.
- [x] (2026-10-12 15:41Z) Author the design and small, sequential implementation milestones. Feature implementation has not started.
- [x] (2026-10-12) Milestone 1: Record clarified requirements in Jira and capture the Add baseline.
  - Jira description updated and read back on 2026-10-12; title and Accepted status preserved.
  - User-observed baseline (2026-10-12): State Property Setup sorting only works ascending; Add always appends at the end. Custom Prop Editor sorting works ascending and descending; Add inserts at the active sort position. Resolve the Display Setup sorting behavior so both editors handle active sorting consistently.
  - The prescribed Release solution rebuild was attempted but failed in TimedSequenceMapper because `ElementMapperView.g.cs` and `SourceTreeView.g.cs` were missing under `obj`.
- [x] (2026-10-12) Milestone 2: Fix Display Setup sorting and align Add insertion with Custom Prop Editor.
  - Removed `ClearSortAfterCommand="True"` from the State Property Setup grid. This preserves the active DataGrid sort after persisting the sorted order, matching Custom Prop Editor; Add now follows the active sorted view when the observable collection changes.
  - Release solution rebuild succeeded (73 warnings, 0 errors). `StateMapperDefinitionTests`: 22 passed, 0 failed, 0 skipped. `git diff --check` passed.
  - Rider file diagnostics and interactive UI confirmation were unavailable in this session; verify both sort directions and Add placement in the manual end-to-end milestone.
- [x] (2026-10-12 16:23Z) Review the attempted sorting amendment and record the user's correction. Completed Milestones 1 and 2 below remain historical records, not approval of retained live sorting. The existing Display Setup XAML change is uncommitted; this revision changes only the plan.
- [x] (2026-10-12 16:35Z) User reverted the earlier Display Setup sorting change to baseline; git diff for StateMapperView.xaml is empty and git status lists only the untracked plan. No Display Setup sorting edit remains for the coding agent.
- [ ] Milestone 2A: Enable one-time sorting in Custom Prop Editor only (one XAML setting).
- [ ] Milestone 2B: Verify ascending sort, then visible manual moves and bottom insertion in both editors; reconcile Jira requirements. This review gate must pass before Milestone 3.
- [ ] Milestone 3: Add source-based State Item naming and focused tests.
- [ ] Milestone 4: Implement the Display Setup Copy command and basic command tests.
- [ ] Milestone 5: Add the Display Setup Copy button.
- [ ] Milestone 6: Implement the Custom Prop Editor Copy command and basic command tests.
- [ ] Milestone 7: Add the Custom Prop Editor Copy button.
- [ ] Milestone 8: Prove Display Setup hierarchy independence and draft/save behavior.
- [ ] Milestone 9: Prove Custom Prop Editor independence and editor integration.
- [ ] Milestone 10: Complete end-to-end validation and update Jira with actual results.

## Surprises & Discoveries


Observation: Both item models already support copying with a new ID and a separate assignment collection. Evidence: `StateItemData.CloneAsNew()` copies `ElementNodeIds` into a new list; `StateItemModel.CloneAsNew()` copies `ElementModelIds` into a new observable collection. Neither needs new persisted fields or a serialization migration.

Observation: Existing naming starts at 1 and compares names case-sensitively. Evidence: `StateNamingRules.GetNextName` produces `baseName + " - " + index`, starts index at 1, and increments while `existingNames.Contains(candidate)`. Ordinary duplicate item names are allowed, but Copy should choose a currently unused name.

Observation: The Custom Prop Editor State tab is in `src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml`, not a separate State editor view. The actual tab header is State. Older requirements describe a State Definition tab; use the current source's location and label for this improvement.

Observation: The attempted amendment retained live sorting in Display Setup by removing its explicit ClearSortAfterCommand setting. The user's subsequent report identifies that retained sorting conflicts with visible Move Up/Down reordering. Original Display Setup behavior is the required reference: one-time ascending sorting followed by freely editable item order and bottom insertion.

Observation: The shared DataGridSortedItemsCommandBehavior already implements the required sequence. It queues ExecuteSortedItemsCommand at DispatcherPriority.Background, reads the displayed order, calls the grid's persistence command, then, when ClearSortAfterCommand is true, clears Items.SortDescriptions, resets every column's SortDirection to null, and refreshes Items. Clearing column direction means the next ordinary header click starts from the ascending state again. Source inspected during this revision:

    src/Vixen.Common/WPFCommon/Behaviors/DataGridSortedItemsCommandBehavior.cs

Its default is false. The user has restored State Property Setup's explicit true setting to baseline; the remaining coding change is to add that setting to the Custom Prop Editor State Item grid only. Keep State Property Setup as the unchanged comparison reference. Do not change the shared default or its implementation. Verify the actual grids after the queued callback; existing view-model sorting tests alone cannot reproduce a retained WPF sort.

Observation: The final VIX-3591 requirements contain older plain `dotnet test` commands. Current `AGENTS.md` requires full MSBuild first because the test project depends on C++/CLI projects. Use the commands in this plan.

Observation: Rider's `findTests` hook failed for the naming helper and both Add methods with `System.InvalidOperationException: Launch failed`. Gortex's type-level test query also returned no mapped coverage, although source inspection found existing tests in `StateMapperDefinitionTests` and `CustomPropStateEditorViewModelTests`. These results are tooling limitations, not evidence that the code has no tests.

## Decision Log


These entries preserve decision history. The 2026-10-12 16:23Z correction supersedes retained live sorting; the 16:35Z update narrows the remaining production change to Custom Prop Editor because the user restored Display Setup to baseline. Single selection and independent copying remain required. Completed milestone descriptions are retained for audit; they are not the current sorting acceptance criteria.

- Decision: Enable Copy only when the selected-items collection contains exactly one row that belongs to the current State Definition. Recheck the same condition inside execution.
  Rationale: User clarification on 2026-10-12. Checking membership also prevents a stale row from another definition being copied.
  Date/Author: 2026-10-12 / User and Codex.
- Decision: Copy follows existing Add under sorting. It appends to the underlying collection and selects the new row through the same path as Add; its visible position follows that editor's existing sort behavior.
  Rationale: User clarification supersedes interpreting the original bottom-of-list requirement as permission to change sorting. No new sorting policy is required.
  Date/Author: 2026-10-12 / User and Codex.
- Decision: Reuse both existing `CloneAsNew()` methods and add one overload to the shared `StateNamingRules.GetNextStateItemName` helper.
  Rationale: Existing copy methods already preserve colors and assignment IDs while giving the item a new identity and independent collection. Shared naming keeps the two editors consistent without combining their different models.
  Date/Author: 2026-10-12 / Codex.
- Decision: Treat the complete source name as the naming base, including any existing numeric suffix. Trim surrounding whitespace; use `DefaultItemName` for an empty base. Start the added suffix at 1.
  Rationale: This directly reuses the current name-generation algorithm, rather than introducing suffix parsing or changing Add naming. For example, copying `Arm - 1` produces `Arm - 1 - 1` if that name is free.
  Date/Author: 2026-10-12 / Codex.
- Decision: Extract only the existing append-and-select tail of each Add method into a private helper shared by Add and Copy.
  Rationale: Preserving the existing notification, validation, preview, and selection sequence makes Copy behave consistently with Add. This is a local change, not a new service or model abstraction.
  Date/Author: 2026-10-12 / Codex.
- Decision: Keep every implementation milestone bounded to one editor or one shared helper, with its own proof and review stop.
  Rationale: The user requested milestones suitable for a lower reasoning model. The invoked skill explicitly requires manual review stops; those specific stops take precedence over the generic automatic-advance wording in `.agents/PLANS.md`. No commit may be created without explicit user authorization.
  Date/Author: 2026-10-12 / Codex.
- Decision: Resolve Display Setup's sort-direction defect and make Add insert according to the active sort before implementing Copy; use Custom Prop Editor behavior as the alignment target.
  Rationale: User-observed behavior on 2026-10-12: Display Setup only sorts ascending and appends Add at the end, while Custom Prop Editor supports ascending/descending and inserts at the active sort position. Copy should then follow the corrected Add path in both editors.
  Date/Author: 2026-10-12 / User and Codex.

- Decision: Supersede retained sorting with one-time ascending sorting in both State Item grids; after persisting the sorted order, clear the active sort so manual moves remain visible and new Add/Copy rows append at the bottom.
  Rationale: The user reported that retained Custom Prop Editor sorting makes Move Up/Down ineffective in the displayed grid and explicitly chose original State Property Setup behavior as the reference. The previous amendment aligned in the opposite direction. The existing shared behavior supplies the requested operation without new C# logic.
  Date/Author: 2026-10-12 16:23Z / User and Codex.
- Decision: Preserve completed Milestones 1 and 2 verbatim and insert corrective Milestones 2A and 2B before pending naming/Copy work. Restore the Display Setup flag and add the Custom Prop Editor flag; require real-grid sort/move/add verification before proceeding.
  Rationale: The revise-active-plan skill protects execution history. Git confirms the earlier Display Setup change remains uncommitted, so the coding agent should make a guarded correction, not rewrite history or reset files. Earlier passing view-model tests did not prove that the WPF grid released its sort.
  Date/Author: 2026-10-12 16:23Z / Codex.

- Decision: Narrow Milestone 2A to the Custom Prop Editor State Item grid only; treat State Property Setup as the already-restored, unchanged baseline and retain both-screen parity validation in Milestone 2B.
  Rationale: The user reverted the prior Display Setup edit. Git confirms StateMapperView.xaml has no diff and only the untracked plan remains in status. The coding agent therefore has one production setting to change, with no remaining restoration work. This supersedes the earlier two-view edit scope without rewriting completed history.
  Date/Author: 2026-10-12 16:35Z / User and Codex.

## Outcomes & Retrospective


Current outcome (2026-10-12 16:35Z): State Property Setup is back at baseline following the user's revert. Pending Milestone 2A changes only Custom Prop Editor's State Item sorting behavior; Milestone 2B compares it with the unchanged State Property Setup reference before naming/Copy work. This update changes only the plan. Completed milestones and prior review evidence remain historical records.

Previous review outcome (2026-10-12 16:23Z): The plan review found that the attempted amendment chose retained live sorting, opposite to the newly confirmed requirement. This revision defines the existing clear-after-persist behavior as the reference for both grids and adds two small prerequisites. Next implement Milestone 2A, then complete and review Milestone 2B; do not start pending Milestone 3 or Copy work before that gate. No production code or Jira was changed during this review. The previously reported build and 22 passing StateMapperDefinitionTests are historical evidence, not acceptance of the new sorting requirement.

Historical execution report before this correction (retained): Planning is complete. Milestone 1's Jira update/readback and user-reported Add baseline are recorded above; Milestone 2 implementation and automated validation are complete. Jira VIX-4019's title remains “Ability to duplicate a State Item in a State Definition” and status remains Accepted. `git status --short` showed only the plan file untracked before the update. `Get-Command msbuild` found MSBuild 18.4.0. The first prescribed Release solution rebuild (`msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64`) failed with two CS2001 errors in TimedSequenceMapper because `ElementMapperView.g.cs` and `SourceTreeView.g.cs` were missing under `obj`; a subsequent full rebuild succeeded with 73 warnings and 0 errors. The focused `StateMapperDefinitionTests` run passed 22/22. `git diff --check` passed. Interactive verification of ascending/descending toggling and Add placement remains outstanding because computer-use tooling is unavailable in this session. Milestone 2 is at its review boundary; do not begin Copy implementation until review. No production C# or tests changed; issue status and child issues remain unchanged.

## Context and Orientation


A State Definition contains State Items, each with a name, color, stable ID, and assigned elements. A stable ID is a persisted GUID used to distinguish a logical item even after its name changes. Copy creates a different logical item, so its ID must differ. Assigned element IDs still refer to the same existing elements; independence means separate editable item data and assignment collections, not duplication of the element hierarchy itself.

Display Setup uses `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs`. It clones persisted `StateData` into an editable draft. `Items` belongs to `SelectedStateDefinition`; `SelectedItems` is the grid's selection collection. `AddItemAsync` creates a default item, appends a fresh `StateItemViewModel`, and selects it. `SaveAsync` copies all draft definitions back to the source; dismissing the window without saving leaves the source unchanged. `StateItemViewModel` constructs fresh assignment-tree nodes from `_elementTree` and the item's stored IDs. Its assignment changes update the item's ID list. A copied wrapper must be newly constructed, never shared with the source row.

Custom Prop Editor uses `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/StateDefinitionEditorViewModel.cs`. `SelectedStateItems` is the grid selection. Definitions wrap the current prop's editable models rather than a separate Display Setup draft. `AddStateItem` calls `CustomPropStateDefinitionViewModel.AddItem`, which appends both the `StateItemModel` and its fresh row wrapper. The editor then selects one row and calls its existing dirty/validation/preview update sequence. Existing prop save and unsaved-change prompts persist or discard these changes.

Primary references inspected before design are `docs/plans/state/vix-3591-state-property-final-requirements.md`, the Copy/Clone section of `docs/plans/state/vix-3591-state-property-prerequisites.md`, existing State Item editing requirements in `docs/plans/state/vix-3591-state-property-phase-4-requirements.md`, and the State Definition tab requirements in `docs/plans/state/vix-3929-custom-prop-editor-state-requirements.md`. Their relevant contracts are embedded here: logical draft clones preserve IDs; user copies create new IDs; assignments preserve group/leaf semantics; existing validation, save, cancel, and preview behavior still applies.

## Architecture Design: VIX-4019


### Detected IDE Environment


Rider is available with file analysis and test hooks. Apply implementation edits in small file-scoped or method-scoped blocks suitable for Rider's Apply snippet from chat workflow. Native Gortex MCP remains mandatory for indexed source inspection and guarded mutations; do not substitute Rider file reads or patch tools for those operations.

### Core Strategy


Use the existing Catel command-binding pattern, the current item copy methods, and shared State naming rules. Project `dotnet-best-practices`, `catel-mvvm`, and `csharp-docs` guidance informed this design: view models own behavior, XAML buttons bind commands, command predicates drive enabled state, and added public APIs have XML documentation. No new dependency, registration, or lifecycle service is needed.

### Sorting and Manual Order Contract


Both State Item grids must opt into the existing ClearSortAfterCommand behavior with a value of true. An ordinary header click performs one ascending sort. The existing persistence command makes that displayed order the definition's stored row order before the queued behavior clears sort descriptors and column directions. Once that callback finishes, Move Up/Down visibly changes collection order, and Add appends a new row at the bottom. Another ordinary click on the same header again starts ascending rather than toggling to descending. Copy will later append in exactly the same way as Add. A subsequent explicit header click may sort the newly added/copied row with the rest; there is no continuing automatic sort between clicks.

The user has already restored Display Setup to baseline. The prerequisite adds the setting only to the State Item DataGrid in Custom Prop Editor; there is no pending State Property Setup sorting edit. Existing persistence commands, selection logic, dirty notifications, preview callbacks, and shared behavior implementation remain unchanged. Do not change other grids, force a shared default, introduce custom comparers, or rewrite Move/Add commands. The sole production file modified in Milestone 2A is:

    src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml

State Property Setup remains the read-only comparison reference for sorting validation:

    src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml

### Data Model & Property Contracts


Use `StateItemData.CloneAsNew()` in Display Setup and `StateItemModel.CloneAsNew()` in Custom Prop Editor. Preserve source color exactly, including alpha; do not replace it with Add's default/discrete-color choice. Preserve assignment IDs and their current grouping semantics. Generate a new non-empty item ID while preserving the parent definition's ID and every original item's ID. Copy does not clone or modify a State Definition.

The only new public API outside commands is the overload in `src/Vixen.Modules/Property/State/StateNamingRules.cs`:

    public static string GetNextStateItemName(string baseName, IReadOnlyCollection<string> existingNames)

The existing one-argument overload and private `GetNextName` remain available with their existing behavior. The new overload rejects null arguments using `ArgumentNullException.ThrowIfNull`, trims `baseName`, substitutes `DefaultItemName` when the result is empty, and delegates to `GetNextName`. Add summary, parameter, return, and exception XML comments for the new overload.

### Mathematical / Boundary Logic


For Copy, the selected-items collection is authoritative. Exactly one selected row must belong to the selected definition. Zero, multiple, or foreign rows disable Copy; executing the private action in such a state must return without mutation. Do not fall back to a cached primary row when the selected-items collection is empty or has multiple rows.

Given source `Arm` and existing names `Arm`, `Arm - 1`, and `Arm - 3`, the copy name is `Arm - 2`. With only `Arm` present it is `Arm - 1`. Case-only differences do not reserve a candidate: `arm - 1` does not prevent `Arm - 1`. If `Arm - 1` itself is copied, use the complete name and produce `Arm - 1 - 1` when free. Numeric suffixes are only appended; there is no parsing, stripping, or wrap-around operation to add.

After copying, the new row is the sole selected row, as with Add. Copy remains available for that row. Repeated clicks therefore copy the currently selected row and apply the same naming algorithm to its complete current name. No rename dialog or confirmation dialog is introduced.

### Subsystem Component Matrix


| Area | Exact repository-relative file | Change or reuse |
| --- | --- | --- |
| Shared naming | `src/Vixen.Modules/Property/State/StateNamingRules.cs` | Add the source-based overload; reuse suffix generation. |
| Display data | `src/Vixen.Modules/Property/State/StateItemData.cs` | Reuse `CloneAsNew`; update its internal summary to cover copying an item within a definition. |
| Display command | `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs` | Add Copy command/predicate/action and shared append helper; extend command-state notifications. |
| Display row/tree | `src/Vixen.Modules/Property/State/Setup/ViewModels/StateItemViewModel.cs` | Reuse constructor and assignment logic; no planned edits. |
| Display view | `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml` | Sorting baseline already restored by the user; no prerequisite edit. Later insert the Copy button beside Add and Remove. |
| Prop data | `src/Vixen.Modules/App/CustomPropEditor/Model/StateItemModel.cs` | Reuse `CloneAsNew`; update its internal summary. |
| Prop command | `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/StateDefinitionEditorViewModel.cs` | Add Copy command/predicate/action and shared append helper; extend command-state notifications. |
| Prop item insertion | `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/CustomPropStateDefinitionViewModel.cs` | Reuse `AddItem`; no planned edits. |
| Prop view | `src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml` | First enable ClearSortAfterCommand for the State Item grid; later insert its Copy button. |
| Naming tests | `src/Vixen.Tests/Property/State/StateItemCopyNamingTests.cs` | New focused xUnit test file. |
| Display tests | `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs` | Extend existing fixtures and private-method invocation helpers. |
| Prop tests | `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStateEditorViewModelTests.cs` | Extend existing command, model, and drawing-preview fixtures. |

## Plan of Work


Milestones 1 and 2 have already been recorded as completed; their original descriptions below are protected execution history and must not be re-executed as current instructions. The next active milestone is 2A, followed by 2B. Both corrective prerequisites and explicit review must finish before pending Milestone 3 or any Copy implementation. The current sorting contract is one-time ascending sort, visible manual reordering afterwards, and all new Add/Copy items at the bottom.

Execute remaining milestones in that order. Each implementation milestone must leave the affected projects buildable and its focused checks passing. Apply only the stated changes; update this plan's Progress and evidence at each review stop. If verification exposes a necessary change beyond the named scope, record the evidence and revise the plan before expanding that scope.

### Milestone 1: Record requirements and capture the Add baseline


Context: This milestone changes VIX-4019's description and this plan's execution evidence only, after the user instructs an agent to execute the plan. Preserve the issue title and unrelated fields. Use the project Jira skill and the user-facing description in Artifacts and Notes; it includes the clarified sorting and selection requirements. Read back the description to verify it. Do not change the issue status or create child issues.

Concrete steps: In `C:\Dev\Vixen`, run `git status --short` and check MSBuild availability with `Get-Command msbuild`. Inspect any existing working-tree changes before editing. Using a disposable profile/prop, observe Add in each editor with no sorting, after sorting by Name, and after sorting by Count. Record where the new row appears, what remains selected, whether sort indicators clear, and whether saved order persists. Use the existing built application or Rider run configuration; if no usable build exists, run the solution build from Concrete Steps.

Validation and acceptance: Jira reflects both user decisions. The plan records an actual Add baseline for each editor; source already shows their different sorting configuration. This evidence defines Copy's expected sorted behavior without changing Add.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below. This milestone is incomplete until the baseline and Jira readback are recorded.

### Milestone 2: Fix Display Setup sorting and align Add insertion with Custom Prop Editor


Context: Implement this user-added requirement before Copy so both editors share the same Add semantics. First diagnose the State Property Setup DataGrid sort behavior and compare it with the Custom Prop Editor State Item grid. The user observed that Property Setup only sorts ascending and Add always appends, while Custom Prop Editor sorts in both directions and Add inserts at the active sorted position. Limit production/test changes to the verified Display Setup sorting and insertion path; preserve the Custom Prop Editor behavior as the reference.

Plan of work: Identify why descending sorting is unavailable in the Property Setup grid and why Add bypasses the active sort position. Correct the smallest responsible view/behavior/collection path so both sort directions work and Add appears at the active sort position. Add focused regression tests for sort persistence/insertion where the existing test architecture supports them; add no broad cleanup. Record any manual-only part precisely.

Concrete steps: Read the relevant DataGrid and sorting behavior code, assess impact before editing, and inspect existing State mapper tests. Run the narrowest relevant tests and Rider file diagnostics, then `git diff --check`.

Validation and acceptance: Property Setup sorts ascending and descending. Adding an item with either sort direction active places it where Custom Prop Editor would place it. With no active sort, Add remains at the end. The sort indicator and persisted item order remain consistent with the existing behavior contract.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 2A: Enable one-time sorting in Custom Prop Editor only


Context: This is the next active coding milestone. The user has reverted Display Setup's prior sorting edit to baseline; no restoration is required. Modify only the following production file and this plan's progress/evidence:

    src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml

Plan of work: Locate the State tab's State Item grid and set ClearSortAfterCommand to true on its existing DataGridSortedItemsCommandBehavior. Retain its SortedItemsCommand binding. The intended declaration is:

    <behaviors1:DataGridSortedItemsCommandBehavior SortedItemsCommand="{Binding PersistStateItemSortCommand}"
    ClearSortAfterCommand="True"/>

Use the surrounding file's existing tabs and line layout when applying the snippet. Leave State Property Setup's restored baseline intact; do not edit that view for this prerequisite. Do not modify the common behavior, its default value, either view model, item models, selection behaviors, or unrelated controls. There is no Copy work in this milestone. The existing persist-then-clear callback supplies one-time ascending sorting and then releases manual row order.

Concrete steps: In the repository working directory below, inspect git status and the scoped diff, use Gortex reads and impact assessment for ElementTree, apply the one attribute change, run Rider file diagnostics on that XAML file, and build the Release solution. Confirm the State Property Setup diff remains empty before and after the change. The shared Concrete Steps section contains the test workflow for the following milestone.

    C:\Dev\Vixen
    git status --short
    git diff -- src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml
    git diff -- src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64
    git diff --check

Validation and acceptance: Custom Prop Editor explicitly sets the flag to true and retains its command binding. State Property Setup's restored sorting configuration is unchanged. Release builds with zero errors; changed-line diagnostics introduce no errors. Configuration and compilation alone do not accept this prerequisite for Copy: Milestone 2B must prove visible reordering and insertion against the unchanged reference.

STOP HERE for manual review and commit execution before proceeding. STOP BOUNDARY: halt execution, inspect scoped git diff and git status, invoke the project commit-msg skill with VIX-4019 as the subject prefix, provide its complete paste-ready commit message, and wait for explicit human review. Do not create a commit without authorization. Follow the Review Stop Protocol below.

### Milestone 2B: Verify sort, manual movement, and bottom insertion before Copy


Context: Validate the Custom Prop Editor State Item grid changed in Milestone 2A against the unchanged, user-restored State Property Setup grid. Keep the scenarios below for both screens to demonstrate consistent behavior; no State Property Setup edit is part of this prerequisite. No new production code or test harness is planned. Existing view-model sort/order tests are useful regressions, but they cannot prove that an actual WPF DataGrid released its sort. Keep this milestone pending until interactive evidence is available; user-performed checks may provide that evidence if the coding agent cannot interact with the app. Pending naming/Copy work must not begin before this gate passes and is reviewed.

Concrete steps: From the same repository directory, build the test target with full MSBuild and run the two existing focused suites using the no-build commands in Concrete Steps. Run changed-view Rider file diagnostics and git diff --check. Record exact results and any tooling limitations. Exercise the built Release application through its available run configuration or installed output. Use a disposable prop/profile in each editor, and record the following scenarios independently for both screens.

For Name sorting, begin with three named rows in creation order Zulu, Alpha, Bravo. Click Name once and wait for the UI sort callback to finish: the visible order is Alpha, Bravo, Zulu and no active sort remains. Select Bravo, move it up, and confirm Bravo, Alpha, Zulu with Bravo still selected; move it down and confirm Alpha, Bravo, Zulu. Click Name again after the first callback completed; it must again produce ascending order, not descend. Move Zulu above Bravo, giving Alpha, Zulu, Bravo, then Add a new default-named row. The new row must appear at the bottom and the manually chosen order above it must remain. An explicit later Name click may sort all rows once again.

For Count sorting, give rows distinct effective assignment counts 3, 1, and 2. Click Count and confirm 1, 2, 3. Move the count-2 row up and confirm 2, 1, 3. Add an unassigned count-0 row and confirm it is last rather than automatically moving to the first position. Verify identities, names, colors, assignments, counts, and selected row are preserved by sorting/manual moves. Enable preview for a move and verify the existing selected-row preview remains correct; verify no preview output while preview is off.

Before saving, satisfy existing validation, including assigning at least one valid element to the newly added row in Custom Prop Editor. The count-0 check above observes initial Add placement; it does not bypass the editor's existing assignment requirement. After assigning that new row, confirm it remains at the bottom rather than being automatically re-sorted by its changed Count. Save the manually reordered list with its appended row, reopen it, and confirm the displayed order and item data remain. Display Setup Cancel/window close must discard unsaved ordering/add changes; Custom Prop Editor must retain its existing save/dirty/discard workflow. Switch definitions and confirm there is no retained sorting that changes the next definition's order. Record these results in Progress and Outcomes & Retrospective. Do not mark existing tests or the earlier 22-test run as evidence that these real-grid cases passed.

After the sort gate passes, reconcile VIX-4019's description using the revised user-facing description in Artifacts and Notes and the project Jira skill. Preserve the title, unrelated requirements, status, and fields, and read back the update. Milestone 1's previous update remains recorded history; this follow-up makes its sorting requirements consistent with the current correction. Do not publish a Copy-completion comment or transition the issue.

Validation and acceptance: In both screens, ordinary header clicks sort ascending once, both arrow directions visibly move the selected row afterwards, and every new Add remains at the bottom even after sorting by Name or Count. The stored order survives save/reopen. Both focused suites discover tests and report zero failures. Jira reflects one-time sorting and bottom insertion. If any grid retains sorting or moves do not visibly work, keep this gate incomplete and diagnose that failure before expanding scope; no change to the shared behavior is pre-authorized by this plan.

STOP HERE for manual review and commit execution before proceeding. STOP BOUNDARY: halt execution, inspect scoped git diff and git status, invoke the project commit-msg skill for this milestone's plan/evidence changes with the VIX-4019 prefix, provide its complete paste-ready commit message, and wait for explicit human review. Do not create a commit without authorization. Follow the Review Stop Protocol below. Only after this gate is accepted may Milestone 3 begin.

### Milestone 3: Add shared source-based naming


Context: Modify only `src/Vixen.Modules/Property/State/StateNamingRules.cs` and create `src/Vixen.Tests/Property/State/StateItemCopyNamingTests.cs`, using namespace `Vixen.Tests.Property.State`. Read the existing helper before editing.

Plan of work: Add the two-argument overload specified in Data Model & Property Contracts; delegate to the existing private `GetNextName`. Do not expose that private helper or alter existing default name generation. Document the new overload completely. Add xUnit tests for first suffix, occupied suffixes/gaps, case-sensitive collision matching, a source already containing a suffix, trimmed/blank bases, and null arguments. Include a regression assertion that the existing one-argument overload still generates `State Item - 1` or skips existing default names as before.

Concrete steps: Build `Vixen_Tests` with full MSBuild and run the focused test command from Concrete Steps with filter `FullyQualifiedName~Property.State.StateItemCopyNamingTests`. Run Rider `get_file_problems` on both changed files and `git diff --check`.

Validation and acceptance: `Arm` becomes `Arm - 1` when free; occupied names are skipped exactly as specified; existing Add naming is unchanged. The new tests pass and no new diagnostics are introduced in changed lines.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 4: Implement Display Setup's Copy command


Context: Modify only `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs`, `src/Vixen.Modules/Property/State/StateItemData.cs`, and `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs`. Do not add the button yet.

Plan of work: Add `_copyItemCommand` and the documented public command property alongside `AddItemCommand` and `RemoveItemCommand`:

    public TaskCommand CopyItemCommand
    private bool CanCopyItem()
    private Task CopyItemAsync()
    private Task AppendAndSelectItemAsync(StateItemData itemData)

Use the existing lazy Catel `TaskCommand` construction with action and predicate. The predicate requires a selected definition, `SelectedItems.Count == 1`, and membership of `SelectedItems[0]` in `Items`. The action first checks the predicate, captures `SelectedItems[0]`, calls `source.Item.CloneAsNew()`, and sets the copied name with the new naming overload and `GetSelectedStateDefinitionStateItemNames()`. Then append using the shared helper.

Move only the existing append/select tail of `AddItemAsync` into `AppendAndSelectItemAsync`: preserve construction of a fresh `StateItemViewModel(itemData, _elementTree)`, `_suppressPreviewRefresh` handling, `Items.Add`, `SelectedItem`, the final preview refresh, command-state refresh, validation, and completed-task return. Keep Add's null-definition guard, naming, and default-color creation unchanged, and call the helper with `CreateDefaultStateItem(name)`. Copy must never call `CreateDefaultStateItem`, since that would replace the source color and assignments. Add `_copyItemCommand?.RaiseCanExecuteChanged()` to `RaiseItemCommandCanExecuteChanged`, already reached by selection, definition, and collection updates. Update `CloneAsNew`'s summary from a distinct-definition copy to a new-item-identity copy; do not change its implementation.

Concrete steps: Extend the existing test class using `CreateViewModel`, `CreateSource`, and `InvokeAsync`. Test no selection, one valid selection, two selections, a foreign row, and availability after switching definitions. Test that direct invocation of `CopyItemAsync` cannot mutate on an invalid selection. With one source row selected, assert one appended copy, source-based name, identical color, new item ID, same parent definition ID, separate assignment list, and the copy as the sole selection. Instantiate the command before changing selection so command availability notifications are exercised. Run the test build and filter `FullyQualifiedName~Property.State.StateMapperDefinitionTests`; check all changed files with Rider and `git diff --check`.

Validation and acceptance: The command can be exercised from tests without a view; existing Add/default-color and reorder tests still pass. No default-color calculation is used for Copy, and multi-selection cannot produce a copy.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 5: Add Display Setup's Copy button


Context: Modify only `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml`, in the State Item `WrapPanel` containing Add and Remove, not the State Definition menu.

Plan of work: Insert `Content="Copy"`, `Command="{Binding CopyItemCommand}"`, and `Style="{StaticResource RowButtonStyle}"` immediately after Add. Let the command determine enabled state; do not add a separate `IsEnabled` binding. Preserve State Property Setup's existing, user-restored one-time sorting setting and selection behavior; Milestone 2A changes only Custom Prop Editor. The small button insertion is the only view edit.

Concrete steps: Use Rider file analysis for this XAML file, build the affected solution configuration as described in Concrete Steps, and run `git diff --check`. Open State Property Setup in the disposable profile.

Validation and acceptance: The row reads Add, Copy, Remove followed by reorder buttons. Copy matches adjacent button styling and is disabled for zero or multiple selected rows. Copying one row selects the copy and preserves assignments and color. Its insertion after sorting appends at the bottom, matching the approved Milestone 2B Add baseline. Check the minimum window size for clipping or wrapping and verify no new binding errors in Rider's application output.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 6: Implement Custom Prop Editor's Copy command


Context: Modify only `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/StateDefinitionEditorViewModel.cs`, `src/Vixen.Modules/App/CustomPropEditor/Model/StateItemModel.cs`, and `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStateEditorViewModelTests.cs`. Do not add the button yet.

Plan of work: Add `_copyStateItemCommand` and a documented lazy Catel `Command`, matching synchronous `AddStateItemCommand`:

    public Command CopyStateItemCommand
    private bool CanCopyStateItem()
    private void CopyStateItem()
    private void AppendAndSelectStateItem(StateItemModel item)

The predicate requires a selected definition, exactly one `SelectedStateItems` entry, and membership of that entry in `SelectedStateDefinition.Items`. The action rechecks the predicate, captures that sole row, calls `source.StateItem.CloneAsNew()`, assigns the source-based name using the shared overload and `GetSelectedDefinitionStateItemNames()`, then calls the append helper.

Move only Add's append/select/update tail into the helper: call `SelectedStateDefinition.AddItem(item)`, `SelectSingleStateItem(SelectedStateDefinition.SelectedItem)`, `RaisePropertyChanged(nameof(SelectedStateItem))`, and `OnStateDataChanged()` in the existing order. Leave Add's guard and new-default-item construction intact. Reuse `CustomPropStateDefinitionViewModel.AddItem` so both the model and view-model collections receive the copy and existing callbacks mark the editor dirty. Add the new command to `RaiseCommandCanExecuteChanged`. Do not optimize or replace the existing event sequence. Update only the internal `StateItemModel.CloneAsNew` summary to cover a new item in the same definition.

Concrete steps: Add tests to the existing class with `CreatePropWithModel`, the fake dialog service, and command helpers. Test zero, one, multiple, and foreign selections, and changed availability after a definition switch. Invoke the private action directly for an invalid selection to prove a safe no-op. Assert that Copy appends exactly one model and row, gives a new item ID, leaves the definition ID unchanged, preserves name-base/color/assignments, makes an independent assignment collection, selects only the copy, and marks the editor dirty. Run the test build and filter `FullyQualifiedName~App.CustomPropEditor.State.CustomPropStateEditorViewModelTests`; run Rider file checks and `git diff --check`.

Validation and acceptance: Model and row collections stay aligned, existing Add/Remove/reorder tests pass, and no copy is possible with multiple selected rows. The command updates selection and dirty state using the same path as Add.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 7: Add Custom Prop Editor's Copy button


Context: Modify only `src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml` inside the State tab's State Item button row, currently near line 467. There is no new view to create.

Plan of work: Insert Copy immediately after the item Add button, binding `CopyStateItemCommand`. Use `RowButtonAutoStyle` and the same `MinWidth="{Binding ElementName=RemoveBtn, Path=ActualWidth}"` as Add. Preserve the definition Copy menu and grid selection, and keep ClearSortAfterCommand true as established in Milestone 2A. No event handler is required.

Concrete steps: Run Rider file analysis, build the affected solution configuration, and run `git diff --check`. Open the Custom Prop Editor State tab and exercise zero/single/multiple selection and Copy.

Validation and acceptance: The Copy button matches the State Item Add and Remove buttons, is usable at the editor's supported sizes, and its insertion after sorting appends at the bottom, matching the approved Milestone 2B Add baseline. The copied assignments appear on the drawing surface when previewing or editing the copied item. No new binding errors appear.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 8: Prove Display Setup independence and draft persistence


Context: Extend only `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs`, using the existing root-node factories and `InvokeSaveAsync`. Production fixes, if any, require a recorded failing test and a revised scope before editing.

Plan of work: Add a hierarchy fixture with a parent group covering two leaves plus a separate directly assigned leaf. Copy a row that assigns the parent group and the separate leaf. Assert the same effective count and checked/disabled hierarchy, but distinct row wrappers, tree roots, selection controllers, and assignment collections. Edit the source's color and assignments, then the copy's; each edit must affect only its own row. Remove the source through the existing Remove path and confirm the copy survives with its ID, color, and assignments. Include a case with no source assignments; Copy must retain that empty map and the editor's existing validation behavior.

Test that copying does not mutate persisted `StateData` before save; construct a fresh view model from the unchanged source to demonstrate discarded draft behavior. Save another copied draft through `InvokeSaveAsync`, reopen from that source, and assert the new ID, name, color, map, and item order remain. Check another State Definition is unaffected. Extend existing order tests to show insertion at the end of the current underlying order and retained copied values if the existing sort-order command is applied. Do not claim these view-model tests prove WPF sorting; that proof remains the manual Add/Copy comparison.

Concrete steps: Build tests and run filter `FullyQualifiedName~Property.State.StateMapperDefinitionTests`. Run Rider file analysis and `git diff --check`.

Validation and acceptance: Original and copied assignments can change independently in both directions; deleting the original cannot remove the copy; saving preserves the copy and discarding a draft leaves the persisted source unchanged. Existing assignment and order semantics remain intact.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 9: Prove Custom Prop Editor independence and integration


Context: Extend only `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStateEditorViewModelTests.cs`. Use its existing prop, light, drawing-panel, and `PropEditorViewModel` fixtures.

Plan of work: Copy a colored item assigned to multiple elements and verify the copy targets those same existing elements without duplicating them. Change source/copy colors and assignments in both directions through existing row methods; assert independent model collections and counts. Delete the source and verify the copy remains usable. Cover repeated copies and suffix collisions in the command path. Recreate `StateDefinitionEditorViewModel` over the edited prop to prove both rows are retained in the prop model, while explicitly recording that this is not a disk round-trip.

Use the existing `CreatePropEditorWithState` fixture with the State tab selected to execute Copy through the nested editor command. Assert the copied row is the sole selection, assignment editing targets it, and existing preview callbacks render its assigned lights. Changing the copy's assignments must leave the original assignments intact. Verify existing validation still reports an empty map and is refreshed when assignments change; do not add Copy-specific save rules.

Concrete steps: Build tests and run filter `FullyQualifiedName~App.CustomPropEditor.State.CustomPropStateEditorViewModelTests`. Run Rider file analysis and `git diff --check`.

Validation and acceptance: Copy participates in the real editor's dirty and preview update path; later edits and source deletion are independent. Existing prop-save infrastructure remains responsible for disk persistence, verified in Milestone 10.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below.

### Milestone 10: Validate both workflows and close the documentation loop


Context: Update this plan with actual evidence, then reconcile VIX-4019's description with delivered behavior and add one concise validation comment. This milestone creates no unrelated code changes or issue transitions.

Concrete steps: Run the Release test build, the combined focused filter, the full already-built test suite, the Debug solution build, changed-file Rider checks, and `git diff --check` from Concrete Steps. Record exact command results and passed/failed/skipped counts; do not infer success from missing output. Run the manual scenarios in Validation and Acceptance, including Custom Prop Editor disk save/reopen and its existing discard prompt. If any check fails, fix only the relevant earlier milestone's defect and rerun the affected check; revise this living plan to record the work. Once accepted, read the current Jira description, preserve unrelated additions, apply only necessary final adjustments, and post actual validation results using the closing-comment guidance in Artifacts and Notes.

Validation and acceptance: Both workflows satisfy every Copy criterion and the approved Milestone 2B prerequisite remains intact: ordinary header clicks sort ascending once, both manual arrow directions work visibly afterwards, and every new Add/Copy appends at the bottom. Persistence/discard behavior works, and all relevant checks pass or have explicitly documented limitations accepted by the user. The final Jira comment reports actual results and does not claim unrun checks passed. Record remaining gaps in Outcomes & Retrospective.

STOP HERE for manual review and commit execution before proceeding. Follow the Review Stop Protocol below. There is no next implementation milestone; report completed behavior and any remaining limitations.

### Review Stop Protocol


Every milestone's explicit stop uses this same protocol. Halt implementation at that boundary. Update Progress and any changed decisions or discoveries. Run `git status --short` and scoped `git diff` for that milestone's files; inspect newly created files through Gortex because untracked files are not included in ordinary diffs. For a milestone changing repository files, invoke `.agents/skills/commit-msg/SKILL.md` and finish the handoff with a clean Commit message block containing a VIX-4019-prefixed imperative subject, blank line, and wrapped body. Do not stage or create a commit unless explicitly requested. Pause for explicit user confirmation before advancing. If a milestone changes only Jira and no repository files, report the issue readback instead of inventing a repository commit.

## Concrete Steps


All commands below run from `C:\Dev\Vixen` in PowerShell with full Visual Studio MSBuild and the C++ toolset available. If `msbuild` is not on PATH, use a Visual Studio Developer PowerShell or the installed full MSBuild executable; do not replace it with `dotnet build` for the C++/CLI test dependencies.

Rebuild tests after each code/test milestone. Then run only the specified focused filter with the no-build command. UI-only milestones require a solution build; they do not require new tests mirroring XAML button declarations.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State.StateItemCopyNamingTests"

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State.StateMapperDefinitionTests"

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~App.CustomPropEditor.State.CustomPropStateEditorViewModelTests"

For final acceptance, run the combined State suites and then the full test suite once against the completed Release test build. The broader Property.State filter also covers existing assignment and persistence regressions.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State|FullyQualifiedName~App.CustomPropEditor.State"

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug -p:Platform=x64 -p:PlatformTarget=x64

    git diff --check

Expected outcomes are MSBuild reporting Build succeeded, each filter discovering tests and reporting zero failures, the full suite reporting zero failures, and `git diff --check` producing no output. A filter that discovers zero tests is not a passing validation. Record actual test counts instead of inventing an expected count for this evolving repository. Start the built Vixen application through Rider's existing run configuration; Debug outputs are in `Debug/Output/`.

For every edit, follow the repository Gortex workflow: inspect source through Gortex; assess `change(impact)` before mutation; use `change(verify)` before an API/signature addition or change, with its exact operation schema and proposed signature; mutate through Gortex `edit`/`refactor`; call `change(detect)` and use its returned symbol IDs for `change(tests)`, `change(guards)`, and `change(contract)`. Do not interpret missing graph test edges as proof that no tests exist. Run Rider `get_file_problems` on changed C# and XAML files, resolving only diagnostics introduced in changed lines. Read project skills before applying them; use the project C# documentation skill for new public command properties and the naming overload. Use semantic refactoring tooling and its applicable skill if extracting the shared append helpers through a refactoring operation.

## Validation and Acceptance


Use a disposable test profile/prop with at least two State Definitions and a nested element hierarchy. In each editor, select one colored State Item with both a group assignment and a separate leaf assignment where supported by that workflow. Copy must produce a new row in the same definition with the correct generated name, source color, identical effective assignments/count, a distinct non-empty item ID, and an independent mapping. Existing hierarchy/group semantics must determine the effective assignments; do not flatten Display Setup's checked groups to leaves when copying. Custom Prop Editor must keep its existing model assignment representation and drawing surface workflow.

Copy must be disabled with no selected row or multiple selected rows and become enabled immediately when selection returns to exactly one valid row. Switching definitions, removing rows, or opening an empty editor must not leave Copy enabled for a stale row. Confirm the original is unchanged immediately after Copy; editing either row's color or assignment must leave the other unchanged. Delete the original and continue editing the copy. Try an unassigned source and retain existing validation behavior.

Before Copy work, Milestone 2B must prove ordinary header clicks apply ascending order once and then release sorting in both grids. Move Up and Move Down must visibly change row order afterwards, retaining the moved row's selection. Repeat a normal click on the same header and confirm it again applies ascending order, not a retained-sort descending toggle. Then manually reorder rows and Add: the new row must remain at the bottom and existing rows must retain their manual order. Verify this for Name and Count independently, including a count-0 Add that would otherwise be reinserted first by an active ascending count sort. This accepted baseline replaces Milestone 1's historical mismatch as the reference.

After Copy is implemented, repeat the accepted sort/move/Add scenarios with Copy and confirm every copied row also appends at the bottom. Copy must not reinstall sort descriptions or change the clear-after-persist setting. A later explicit header click may sort the newly added/copied row with the whole list once. Confirm copied values and current order survive save/reopen. Display Setup Cancel and window close must discard copied draft items. In Custom Prop Editor, save the prop through the existing Save action, close/reopen it from disk, and verify the copy's values and assignments. In a separate run copy again, close using the existing discard-unsaved-changes choice, reopen, and confirm the unsaved copy is absent. These manual disk checks are required even when view-model reconstruction tests pass.

With preview off, Copy must follow Add without publishing new preview output. With preview on, the selected copied row must be previewed through existing callbacks and support subsequent assignment/color edits. Verify preview cleanup when closing through the existing workflow. No new preview mode, preview persistence, renderer, or State Effect behavior is part of this improvement.

## Idempotence and Recovery


The initial planning working tree was clean, and the plan path did not exist. Before each implementation milestone, inspect current status and preserve unrelated changes. Jira updates should replace only the relevant description content and be read back; do not append repeated copies of the requirements or duplicate closing comments on retries.

Source edits are repeatable when guarded against their observed content. Before retrying after a partial edit, inspect the actual diff and Progress rather than reapplying the whole milestone. Test and build commands can be rerun safely. Feature Copy intentionally creates another item on each valid activation; it is not an idempotent user operation.

Use only disposable props/profiles for destructive manual scenarios such as removing the original. Do not reset, clean, or discard the user's worktree. If a milestone must be backed out, make a scoped reviewed inverse edit; do not restore entire files containing other people's changes.

## Artifacts and Notes


The following is the revised user-facing description for Milestone 2B's Jira reconciliation. Milestone 1's earlier Jira update remains recorded history. This plan revision does not change Jira; apply the correction and read it back when executing Milestone 2B. Preserve unrelated additions to the issue rather than blindly replacing them.

    ## Summary

    First align State Item sorting in State Property Setup and the Custom Prop Editor so users can sort ascending once, then manually reorder items with the arrow buttons. New items must be added at the bottom.

    Add a Copy button for a selected State Item in both screens. Users can reuse an item's color and assigned elements without rebuilding the item.

    ## Scope

    - Copy is available only when exactly one State Item is selected.
    - The new item stays in the selected State Definition and receives an unused name based on the original name with the existing dash-number naming style.
    - Copy preserves the original color and assigned elements, including existing hierarchy behavior, while allowing each item to be edited or removed independently.
    - Clicking a sortable State Item column header applies an ascending sort once. Afterwards Move Up and Move Down remain usable and visibly change item order.
    - Every new Add and Copy appends at the bottom, including after sorting; another explicit header click may sort the list again.
    - Copy follows Add for selection, validation, saving, and discarding unsaved changes in each screen.
    - The Copy button matches the adjacent Add and Remove buttons in each screen.

    ## Acceptance Criteria

    - Given no selected item or multiple selected items, Copy is disabled; selecting exactly one item enables it.
    - Given one selected item, clicking Copy creates and selects an independent item in the same State Definition with the same color and element assignments.
    - Given an occupied generated name, copying chooses the next unused dash-number name.
    - Given a copied item, editing or deleting either the original or the copy does not change the other.
    - Given a column header click, items are sorted ascending once; afterwards either arrow button visibly moves the selected item in the chosen direction.
    - Given an earlier column sort or manual reorder, Add and Copy append a new row at the bottom without rearranging existing rows.
    - Given another click on the same column header, the list is sorted ascending again, including any new rows.
    - Given saved changes, reopening shows the copy's name, color, assignments, and order; discarding unsaved changes leaves saved data unchanged.
    - These outcomes work in both Display Setup State Property Setup and Custom Prop Editor, with existing validation and preview behavior.

The detailed test plan is local to this ExecPlan. The final Jira comment should briefly state that both Copy workflows were delivered, summarize actual automated test/build results, and state whether one-time ascending sorting followed by arrow movement and bottom insertion was verified in both screens, together with selection, independence, save/reopen, discard, and preview checks. Report skips or failures honestly. Do not post implementation paths or unverified completion claims.

Planning evidence: source inspection confirmed both existing deep-copy methods and the two Add paths. `git status --short` was empty before planning; the target plan path did not exist. Rider test discovery failed as recorded above. Build/test commands in this document are implementation instructions, not executed planning results.

Authoring validation (2026-10-12): `git status --short` showed only this new plan. `git diff --check` exited 0 with no output. `git diff --no-index --check -- /dev/null docs/plans/state/vix-4019-copy-state-item.md` emitted no whitespace diagnostics and exited 1 for the new-file comparison. Gortex verified the file was written to disk; its post-edit detection reported no tracked changes and explicitly noted that untracked files are excluded. No changed C# symbols exist for test/guard/contract analysis. Rider `get_file_problems` could not analyze the Markdown file because it is not included in a solution project. Builds and feature tests were skipped because this deliverable is a design document; no production code was changed.

## Interfaces and Dependencies


Use existing Catel `TaskCommand` for Display Setup and Catel `Command` for Custom Prop Editor. Display Setup exposes `CopyItemCommand`; Custom Prop Editor exposes `CopyStateItemCommand`. The new shared naming overload takes the complete source name and the selected definition's current item names. The two private append helpers accept the respective existing data/model type and create or select rows through existing Add behavior.

Keep `StateItemData` and `StateItemModel` separate because one stores Display Setup element-node IDs and the other stores custom prop element-model IDs. Both copies retain assignment IDs and receive a fresh item ID. Existing `CloneAsNew` implementations, `StateItemViewModel`, `CustomPropStateDefinitionViewModel.AddItem`, parent editor callbacks, and save workflows supply the needed contracts; no new persisted field, package, project, interface, service, or migration is required.

Revision note (2026-10-12): Created after the user clarified single selection and Add-consistent sorting. Split naming, commands, each view, and each workflow's integration proof into separate milestones so a lower reasoning model can execute one bounded change at a time. Analysis complete and plan integrated with plans.md.

Revision note (2026-10-12 16:23Z): Reviewed the coding agent's amendment after the user reported retained-sort contention with Move Up/Down. Preserved completed Milestones 1 and 2 and all earlier decision entries. Inserted corrective Milestones 2A and 2B before pending naming/Copy work, prescribing only the two clear-after-persist view settings and an interactive sort/move/Add acceptance gate. Updated the active purpose, findings, architecture, component scope, remaining Copy acceptance, proposed Jira requirements, and final validation to require ascending sorting once and bottom insertion. Corrected the pending prop-persistence cross-reference to Milestone 10. Production edits and Jira updates are left to execution of the revised prerequisites.

Revision validation (2026-10-12): git status still shows the pre-existing Display Setup XAML change and the untracked plan only. The scoped Display Setup diff is identical to the pre-review diff; no production file was changed in this revision. Git diff --check exited 0, and the untracked-plan no-index whitespace check emitted no diagnostics (exit 1 for the file comparison). Rider get_file_problems returned an empty errors list for this Markdown document. Gortex change detection identified only the pre-existing tracked XAML change with no changed symbol IDs; the untracked plan is outside that detection scope. No C# symbols were changed for tests/guards/contracts analysis. Builds, automated feature tests, and interactive sorting checks were not run because this task revises the plan; their implementation acceptance remains pending in Milestones 2A and 2B.

Revision note (2026-10-12 16:35Z): The user reverted the previous Display Setup sorting edit to baseline. Confirmed its scoped git diff is empty and git status lists only the untracked plan. Narrowed pending Milestone 2A to one setting in Custom Prop Editor ElementTree.xaml, removed active State Property restoration instructions, and updated Progress, current outcomes, architecture, component scope, and remaining milestone references. Preserved completed execution history and earlier dated review evidence. Milestone 2B still compares both real screens before naming/Copy work. This update changes only the plan; no build or feature test is required for the documentation edit.

Plan revision complete and recorded in the Decision Log. Ready for coding model to implement the next milestone.

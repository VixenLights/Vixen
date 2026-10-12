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
- [x] (2026-10-12 16:45Z) Milestone 2A: Enable one-time sorting in Custom Prop Editor only (one XAML setting).
  - Added `ClearSortAfterCommand="True"` to the State Item grid's existing behavior in `ElementTree.xaml`, retaining `PersistStateItemSortCommand`. The scoped State Property Setup diff remains empty.
  - Release solution rebuild passed: 73 warnings, 0 errors, exit 0. Rider file analysis completed with 38 warnings/weak warnings, no errors, and no diagnostics on either changed line. `git diff --check` passed.
  - Gortex detected only the tracked XAML file and no changed symbol IDs; symbol-based tests/guards/contracts have no applicable target. No automated tests or interactive grid checks were run in this UI-only milestone; both are explicitly required by pending Milestone 2B.
  - Milestone 2A is at its prescribed review stop. No commit was created; Milestone 2B and naming/Copy work remain pending.
- [x] (2026-10-12 17:01Z) Milestone 2B: User accepted sorting/manual-order parity and bottom insertion; automated checks passed and Jira requirements reconciled.
  - User report: “The sort in the Custom Prop editor is now a one time event and the up down arrows function afterwards. It matches the behavior of the State Property setup now. This should confirm 2B.” This confirms the real-grid sort release and both arrow directions, with parity to the unchanged State Property Setup reference.
  - Release `Vixen_Tests` build exited 0. `StateMapperDefinitionTests`: 22 passed, 0 failed, 0 skipped (786 ms). `CustomPropStateEditorViewModelTests`: 22 passed, 0 failed, 0 skipped (866 ms). Both focused no-build runs exited 0.
  - Rider rechecked `ElementTree.xaml`: 38 warnings/weak warnings, no errors, no changed-line diagnostics. `git diff --check` exited 0.
  - The initial report needed clarification about insertion and existing workflows. User follow-up: “Yes, the new one was added at the bottom on both. Persistence, preview and discard all worked.” Together with the preceding acceptance of 2B, this supplies user-performed manual acceptance for the prerequisite. Reports are summary evidence, not individual Name/Count or definition-switch transcripts; the detailed final validation matrix remains required in Milestone 10. No desktop-control tools are available for agent-performed grid checks.
  - Re-read Jira before updating; no unrelated description additions were present. Reconciled its description with one-time ascending sorting, visible manual moves, and bottom Add/Copy insertion. Readback confirmed the full requirements and preserved title and Accepted status. No completion comment or transition was posted.
  - Only plan evidence changed in 2B; the earlier 2A XAML edit is preserved and State Property Setup remains unchanged. No staging or commit occurred. Milestone 3 remains unstarted at the 2B review boundary.
- [x] (2026-10-12 17:13Z) Milestone 3: Add source-based State Item naming and focused tests.
  - Added the documented two-argument `GetNextStateItemName` overload with null guards, trimming, blank-base fallback, and delegation to the unchanged private naming algorithm. Existing one-argument Add naming is unchanged.
  - Created `StateItemCopyNamingTests`: 17 cases cover free/occupied suffixes, first gaps, case-sensitive matching, complete suffixed bases, trimming, blank fallback, both null arguments, collection independence, and original-overload regressions.
  - Full Release `Vixen_Tests` MSBuild succeeded (exit 0), including a repeat after the final test-source adjustment. Focused naming tests: 17 passed, 0 failed, 0 skipped (47 ms); combined State suites: 140 passed, 0 failed, 0 skipped (899 ms). Both test commands exited 0. `git diff --check` passed.
  - Rider checked both C# files with no errors or ordinary warnings. The production file has three existing allocation weak warnings outside the added overload; the new test file has 12 allocation weak warnings from necessary test collections/assertion delegates. These are recorded, not suppressed or falsely reported as zero diagnostics.
  - Gortex signature verification models a replacement rather than an additive overload and flagged existing callers; no caller was changed. Post-edit tests/guards/contracts were run. Its test-edge query reports no mapped coverage despite discovering the new test file in impact, and its contract warning for broad shared-helper impact persists after passing regressions. No parse-error reason was reported. Review the scoped additive diff at this boundary; no commit or staging occurred and Milestone 4 remains unstarted.
- [x] (2026-10-12 17:23Z) Milestone 4: Implement the Display Setup Copy command and basic command tests.
  - Added a documented lazy Catel `CopyItemCommand`, the exact-one-current-row predicate, and execution-time recheck. Copy uses `CloneAsNew()` and source-based naming, never Add's default-color calculation.
  - Extracted only Add's append/select tail into `AppendAndSelectItemAsync`, preserving preview suppression/refresh, fresh wrapper construction, selection, command refresh, validation, and completed-task return. Added Copy availability refresh to the existing item-command notification path.
  - Updated only `StateItemData.CloneAsNew`'s summary; its implementation is unchanged. Added 10 test cases for selection/definition/removal notifications, five invalid execution states, copied values/IDs/assignment-list independence/parent identity/order/selection, and actual public command execution.
  - Full Release `Vixen_Tests` MSBuild exited 0. An initial 32-test run passed, but Rider identified a new nullable assertion warning; corrected it, rebuilt with full MSBuild (exit 0), then reran: `StateMapperDefinitionTests` 32 passed, 0 failed, 0 skipped (855 ms). Expanded State regression filter: 253 passed, 0 failed, 0 skipped (737 ms). Both final runs exited 0.
  - Final Rider checks completed on all three changed C# files with no errors and no ordinary warnings on changed lines. Changed lines have four command/wrapper allocation weak warnings and 25 test allocation weak warnings; the documentation-only edit has none. Existing unrelated warnings remain intact. `git diff --check` passed.
  - Gortex impact/signature/detection/test/guard/contract checks performed. New symbols cannot be resolved before creation; later verification misreads the helper's existing parameter count as zero. Builds and direct tests prove actual call compatibility. No mapped test edges or configured guards; broad-impact/historical-co-change contract warnings remain review evidence, not permission to edit other files.
  - Only the three named C# files and this plan changed. No button, prop-editor code, Jira update, staging, or commit. Milestone 5 remains unstarted at this review boundary.
- [x] (2026-10-12 17:33Z) Milestone 5 implementation and automated checks: Add the Display Setup Copy button.
  - Inserted one button immediately after State Item Add, binding `CopyItemCommand` with `RowButtonStyle` and no separate `IsEnabled`. State Definition controls, sorting, selection, and all C# files remain unchanged.
  - Release solution rebuild passed: 73 warnings, 0 errors, exit 0 (40.11 s). Focused State mapper regressions: 32 passed, 0 failed, 0 skipped, exit 0 (861 ms). `git diff --check` passed.
  - Rider analysis completed without errors or ordinary warnings on the new line. It reports an unknown-DataContext weak warning for `CopyItemCommand`, matching neighboring command bindings; runtime binding correctness still requires the prescribed live check. Unrelated diagnostics were preserved.
  - Gortex impact warns about missing XAML edges; detection finds the XAML change but no changed symbol IDs. Symbol-based test/guard/contract checks have no applicable production target, not a clean coverage claim.
- [x] (2026-10-12 17:58Z) Milestone 5/5A user-performed live checks: The narrow style keeps all buttons on one row. Copy appends at the bottom with the proper name and correct color/element mapping. Removing the copy does not affect the original. Recorded the user's report as actual live evidence, not agent-driven testing.
- [x] (2026-10-12 18:06Z) Milestone 5 manual acceptance and review complete: User confirms correct enablement for zero, one and multiple selections; the newly copied row is selected at the bottom with or without sort; text buttons have equal widths and stay on one row; no binding errors were observed; wrapping at minimum size remains usable. Together with the earlier naming/color/mapping/removal checks, this accepts the Display Setup button milestone. Reports are user-performed summary evidence, not fabricated per-column transcripts. No Milestone 6 work, Jira update, staging, or commit.
- [x] (2026-10-12 17:52Z) Milestone 5A solution selection and user implementation: Adopt the user's shared `RowButtonNarrowStyle` (75-unit minimum) in `src/Vixen.Common/WPFCommon/Theme/Theme.xaml`, already applied by the user to all five Display Setup item buttons. This supersedes the proposed local 72/32 styles; no local-style edit remains after interruption. Reuse the shared solution in pending Milestone 7.
- [x] (2026-10-12 18:06Z) Milestone 5A validation and review complete: User confirms one-row equal-text-width layout, usable wrapping down to minimum size, and a successful Release build; explicitly requested no repeated build. Both arrows share the same narrow style and 16-by-16 content dimensions, confirmed from source, without claiming a separate manual measurement. Rider checks completed for theme and view: no errors, no narrow-style diagnostics, no ordinary warnings on item-button lines; known static DataContext hints remain. Focused no-build State mapper tests: 32 passed, 0 failed, 0 skipped (798 ms), exit 0. Shared-style solution and reuse in Milestone 7 remain accepted.
- [x] (2026-10-12 19:24Z) Milestone 6: Implement the Custom Prop Editor Copy command and basic command tests.
  - Added a lazy documented `CopyStateItemCommand` with exact-one-current-definition-row eligibility, command availability refresh, execution-time recheck, `CloneAsNew()`, and source-based unused naming. Add and Copy now share only the existing append/select/property-notification/dirty-update tail; Add keeps its original naming and null guard. Updated only `StateItemModel.CloneAsNew()` XML summary.
  - Added tests for selection and definition-switch availability, five invalid direct-action states, and complete append/name/identity/color/assignment independence/model-row synchronization/sole-selection/dirty behavior.
  - Full `Vixen_Tests` MSBuild command passed (exit 0). The persistence/state filter passed 43 tests, 0 failed, 0 skipped; the expanded persistence/state/VixenPreview regression filter passed 90 tests, 0 failed, 0 skipped (both exit 0). Earlier attempts caught and fixed a transient output-DLL lock, a missing sync reflection test helper, and fixture assumptions about `CreateDefault` starter rows; the final repeated build and regression run were clean.
  - Rider `get_file_problems` reported zero errors across all three C# files. No ordinary warnings were reported on changed lines; allocation-only weak warnings remain (4 in the view model, 21 in added test code; the documentation-only model change has no changed-line diagnostic). `git diff --check` passed.
  - Gortex detection identified the three intended C# files. Its test-edge query has no mapped coverage for the new symbols; no guard rules are configured. Contract review retains a high-blast-radius warning and historical-co-change suggestions; its recommended State and VixenPreview areas were both included in the 90-test passing regression run. No out-of-scope files were changed.
  - Stop at this review boundary. Do not implement the Milestone 7 button until explicitly requested; no Jira update, staging, or commit occurred.
- [x] (2026-10-12) Milestone 7: Add the Custom Prop Editor Copy button; user-performed acceptance and Rider/build validation complete.
  - User report: move-up/down use `RowButtonAutoStyle`; `StateMapperView.xaml` was aligned; the shared `RowButtonNarrowStyle` minimum was reduced to 70; Add/Copy/Remove have equal widths and the arrows are auto-sized. Copy is wired to the intended command, appends at the bottom regardless of sort, uses the proper name, remains independent from its source, persists/reloads, and later sorts/moves correctly. Follow-up confirms Copy is enabled only for exactly one selected item and the copied assignments appear on the canvas identical to the source.
  - Rider `get_file_problems` completed for `ElementTree.xaml` and `StateMapperView.xaml`: no errors. The StateMapper button-row bindings (Add, Copy, Remove, Move Up/Down) receive WEAK WARNING unknown-DataContext diagnostics; Rider cannot statically resolve these view-model bindings. Other XAML diagnostics are also reported. The user confirms no runtime binding errors; do not suppress the static hints or alter unrelated lines.
  - `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` exited 0 and built `Vixen.Tests.dll`; existing unrelated compiler warnings were emitted. User also confirms `git diff --check` passes.
  - M7 is accepted from the user's live validation and the successful build. Stop at the prescribed review boundary: scoped diff/status inspection and paste-ready commit message only; do not commit or begin M8 without authorization.
- [ ] Milestone 8: Prove Display Setup hierarchy independence and draft/save behavior.
- [ ] Milestone 9: Prove Custom Prop Editor independence and editor integration.
- [ ] Milestone 10: Complete end-to-end validation and update Jira with actual results.

## Surprises & Discoveries


Observation (2026-10-12 19:24Z, Milestone 6): `StateDefinitionModel.CreateDefault()` supplies a starter State Item. Tests that need deterministic explicit sources must clear that row first or compare collection snapshots instead of assuming a one-row definition. The existing Custom Prop `StateItemModel.CloneAsNew()` gives the copy a fresh ID and copies `ElementModelIds` into a separate observable collection. The model has no parent-ID property; definition ownership is expressed by the containing `Items` collection.

Observation: Both item models already support copying with a new ID and a separate assignment collection. Evidence: `StateItemData.CloneAsNew()` copies `ElementNodeIds` into a new list; `StateItemModel.CloneAsNew()` copies `ElementModelIds` into a new observable collection. Neither needs new persisted fields or a serialization migration.

Observation: Existing naming starts at 1 and compares names case-sensitively. Evidence: `StateNamingRules.GetNextName` produces `baseName + " - " + index`, starts index at 1, and increments while `existingNames.Contains(candidate)`. Ordinary duplicate item names are allowed, but Copy should choose a currently unused name.

Observation: The Custom Prop Editor State tab is in `src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml`, not a separate State editor view. The actual tab header is State. Older requirements describe a State Definition tab; use the current source's location and label for this improvement.

Observation: The attempted amendment retained live sorting in Display Setup by removing its explicit ClearSortAfterCommand setting. The user's subsequent report identifies that retained sorting conflicts with visible Move Up/Down reordering. Original Display Setup behavior is the required reference: one-time ascending sorting followed by freely editable item order and bottom insertion.

Observation: The shared DataGridSortedItemsCommandBehavior already implements the required sequence. It queues ExecuteSortedItemsCommand at DispatcherPriority.Background, reads the displayed order, calls the grid's persistence command, then, when ClearSortAfterCommand is true, clears Items.SortDescriptions, resets every column's SortDirection to null, and refreshes Items. Clearing column direction means the next ordinary header click starts from the ascending state again. Source inspected during this revision:

    src/Vixen.Common/WPFCommon/Behaviors/DataGridSortedItemsCommandBehavior.cs

Its default is false. The user has restored State Property Setup's explicit true setting to baseline; the remaining coding change is to add that setting to the Custom Prop Editor State Item grid only. Keep State Property Setup as the unchanged comparison reference. Do not change the shared default or its implementation. Verify the actual grids after the queued callback; existing view-model sorting tests alone cannot reproduce a retained WPF sort.

Observation: The final VIX-3591 requirements contain older plain `dotnet test` commands. Current `AGENTS.md` requires full MSBuild first because the test project depends on C++/CLI projects. Use the commands in this plan.

Observation: Rider's `findTests` hook failed for the naming helper and both Add methods with `System.InvalidOperationException: Launch failed`. Gortex's type-level test query also returned no mapped coverage, although source inspection found existing tests in `StateMapperDefinitionTests` and `CustomPropStateEditorViewModelTests`. These results are tooling limitations, not evidence that the code has no tests.

Observation (2026-10-12 17:13Z, Milestone 3): Gortex's signature checker evaluates the new two-argument overload as a replacement for the existing method, reporting four caller mismatches and an extracted old parameter count of zero. The actual edit keeps the old overload and private helper intact; successful full MSBuild and two original-overload tests demonstrate preserved caller compatibility. Git-based change detection excludes the new untracked test file from changed-symbol IDs, although impact now finds it. The symbol test query still reports no mapped coverage. Contract analysis warns about the shared type's broad blast radius; re-running after 140 passing State regressions retains that warning. These limitations are review evidence, not grounds for changing existing callers or expanding implementation scope.

Observation (2026-10-12 17:13Z, Milestone 3): Rider allocation inspections report 12 weak warnings in the new tests, reflecting fixture collections and exception assertion delegates. No error or ordinary warning is present, and no diagnostic is present in the added production overload. Keep the normal readable xUnit fixtures rather than suppressing inspections or reworking unrelated code to manufacture an empty result.

Observation (2026-10-12 17:23Z, Milestone 4): Selection and definition changes already reach `RaiseItemCommandCanExecuteChanged`; adding Copy there supplies availability updates without a new event path. New tests subscribe before selection changes and assert actual `CanExecuteChanged` events for one/multiple/empty selection, definition switching, and item removal. Invalid private action invocation is tested separately for no selection, multiple rows, a foreign row, no selected definition, and a removed row reintroduced into selection.

Observation (2026-10-12 17:23Z, Milestone 4): Pre-edit signature verification reports new command/helper symbols as not found; it cannot validate declarations that do not yet exist. After indexing, re-verification still incorrectly reports the helper's old parameter count as zero and flags its two real one-argument callers. Both full builds and passing Add/Copy tests demonstrate the actual calls are compatible. Contract analysis warns about broad impact and historical co-change of State data, dialogs, and XAML; none requires modification for this command-only milestone. No parse-error reason is reported. The broader regression filter covers all three graph-recommended State test areas.

Observation (2026-10-12 17:33Z, Milestone 5): Rider cannot resolve the view's runtime DataContext, including existing Add/Remove commands and the new Copy command. The new line has one weak warning, not a demonstrated runtime binding failure. The existing constructor evidence sets the view model as DataContext, and the binding uses its existing public command. Do not add design-time metadata or rewrite bindings to suppress the inspection: the plan permits only the button insertion. Live application output remains required evidence. Gortex likewise reports a possible XAML extraction gap and no changed symbol IDs; a passing compilation or view-model test cannot prove WPF layout, binding, or released-sort insertion.

Observation (2026-10-12 17:52Z, Milestone 5A): The user supplied RowButtonNarrowStyle in the shared Theme.xaml with MinWidth 75 and the existing six-unit left margin, and applied it to all five StateMapperView item buttons. The scoped diff contains no local resource styles or proposed 72/32 setters from the interrupted edit. Five buttons now have a combined minimum width of 405 including margins, versus 530 before; minimum width is not a fixed actual width or a live layout result. The user's arrow requirement permits, but does not require, narrower arrows. Preserve their shared solution and verify actual equal widths/no-wrap rather than replacing it with the discarded local approach.

Observation (2026-10-12 17:58Z, user validation): The user reports that all buttons now stay on one row and Copy produces a bottom-appended row with the proper name, correct color and element mapping. Removing that copy leaves the original unaffected. This is direct user-performed UI evidence for the shared style and basic Copy workflow; it is not evidence for separate per-column sort, selection, minimum-size, runtime-log, save/discard/preview, or reverse-deletion scenarios.

Observation (2026-10-12 18:06Z, final M5 acceptance): The user's follow-up confirms selection-driven enablement, selection of the new bottom-appended copy, insertion with/without sort, equal text widths, no observed binding errors and usable wrapping at minimum size. The user reports a successful Release build and asks not to repeat it. Agent Rider checks find no errors; shared narrow-style lines have no diagnostic, and the view retains its known static DataContext hints. Source confirms both arrow buttons use the same style with 16-by-16 image content. These are complementary evidence sources, not an invented agent-run build or per-column live test transcript.

## Decision Log


These entries preserve decision history. The 2026-10-12 16:23Z correction supersedes retained live sorting; the 16:35Z update narrows the remaining production change to Custom Prop Editor because the user restored Display Setup to baseline. Single selection and independent copying remain required. Completed milestone descriptions are retained for audit; they are not the current sorting acceptance criteria.

- Decision (2026-10-12, Milestone 6): Share only Add's append/select/property-notification/dirty-update tail with Copy; preserve Add's guard and default naming path, and use `CloneAsNew()` plus source-based naming for Copy.
  Rationale: This keeps the two commands' existing insertion and update behavior aligned without changing Add's semantics or coupling the two item models.

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

- Decision: Accept the user's two manual reports as the Milestone 2B prerequisite review, together with the passing build, focused suites, and diagnostics; reconcile Jira without reopening production scope.
  Rationale: The user explicitly proposed confirming 2B after proving one-time sorting and both arrow directions, then confirmed bottom insertion in both editors and persistence, preview, and discard. Record exactly those outcomes as user-performed evidence; do not fabricate detailed scenario transcripts. Milestone 10 retains the full end-to-end matrix with Copy.
  Date/Author: 2026-10-12 17:01Z / User and Codex.

- Decision: Keep Milestone 3 additive and run the combined State regression filter in addition to the required naming filter before the review stop.
  Rationale: Signature verification treats the addition as replacement and shared-type impact has a broad warning. Preserving the original overload/private helper and validating both existing editor paths is the appropriate compatibility proof; changing callers or hiding allocation inspections would exceed the milestone or weaken its evidence.
  Date/Author: 2026-10-12 17:13Z / Codex.

- Decision: Preserve the existing append/selection/preview/validation sequence and keep State model, dialog, and view implementations outside Milestone 4.
  Rationale: Add and Copy differ only in creating their item data. The existing item clone supplies independent IDs and assignments without a schema change; historical co-change warnings do not require unrelated edits. The Copy button is explicitly Milestone 5. Broaden validation, not production scope, to cover the graph-identified consumers.
  Date/Author: 2026-10-12 17:23Z / Codex.

- Decision: Keep Milestone 5 partially complete until the prescribed real-window checks are supplied; preserve the exact one-line view scope despite static DataContext and graph limitations.
  Rationale: Desktop-control tools are unavailable. Compilation and command tests provide useful evidence but cannot substitute for button enablement, minimum-size layout, actual grid insertion, or runtime binding output. Request user-performed evidence instead of claiming these steps passed or advancing to Milestone 6.
  Date/Author: 2026-10-12 17:33Z / Codex.

- Decision: Add a bounded Milestone 5A layout correction before Custom Prop Editor work; keep completed Milestone 5 implementation/history unchanged.
  Rationale: User reports the five item buttons wrap and requests one line at the default dialog size, three equally sized text buttons, and two equally sized narrower arrows. Shared RowButtonStyle enforces MinWidth 100 and six-unit left margins, so five buttons need at least 530 layout units. Override width/minimum width locally to 72 for text and 32 for arrows, retaining the theme appearance and reducing total required width to 310. No shared-theme or dialog-size change is needed.
  Date/Author: 2026-10-12 17:43Z / User and Codex.

- Decision: Mark the user-created shared RowButtonNarrowStyle as the layout solution, superseding the proposed local 72/32 sizing in Milestone 5A, and reuse it for the pending Custom Prop Editor item row.
  Rationale: The user explicitly supplied a reusable theme style and requested it be recorded as the solution. Source confirms its 75-unit minimum and existing margins, with all five Display Setup item buttons already referencing it. This task changes only plan documentation; preserve the user's theme/view edits and do not implement Custom Prop Editor early. Previous layout proposal and completed milestone evidence remain historical records.
  Date/Author: 2026-10-12 17:52Z / User and Codex.

- Decision: Accept the user's reported no-wrap and basic Copy/remove outcomes as passed manual scenarios without extrapolating to unreported checks.
  Rationale: The user's report directly proves one-row layout, bottom insertion, proper naming, copied color/mapping and removal of the copy without changing the original. Keep remaining detailed validation explicit and preserve the shared-style solution and sequential review boundary. This evidence update does not authorize implementing Milestone 6.
  Date/Author: 2026-10-12 17:58Z / User and Codex.

- Decision: Mark M5 and M5A complete using the user's accumulated manual acceptance and successful Release build, current file diagnostics, and the passing focused test rerun; do not repeat the build.
  Rationale: The user's latest reports resolve the remaining button-state, selection, sorted/unsorted bottom insertion, layout and runtime-binding concerns. Static inspection confirms paired-arrow sizing. Preserve exact evidence provenance and honor the explicit no-rebuild request. Save/discard, preview and independence are accepted as user summary evidence for Display Setup, but they do not complete later detailed automated hierarchy/integration milestones or prove Custom Prop Editor behavior.
  Date/Author: 2026-10-12 18:06Z / User and Codex.

## Outcomes & Retrospective


Current outcome (2026-10-12 19:24Z): Milestones 5/5A remain user-accepted, and Milestone 6 is implemented and validated. The Custom Prop Editor view model now exposes a command-driven Copy operation for exactly one item in the current definition. It clones the selected model with a new item identity and independent assignments, generates a source-based unused name, appends through the same selection/dirty/update tail as Add, and leaves Add naming unchanged. Availability refreshes with selection/definition changes, and direct invalid invocation is guarded. The full Release `Vixen_Tests` target build and expanded Persistence/State/VixenPreview regressions pass (90 tests); Rider reports no errors and `git diff --check` is clean. Gortex has no mapped test edges or configured guards and retains a broad-impact/co-change review warning; those results are not represented as clean coverage. The working tree change is limited to the two production model/view-model files, their test file, and this plan. Stop at the Milestone 6 review boundary; Milestone 7's button is not implemented. No Jira update, staging, or commit occurred.

Previous partial live outcome (2026-10-12 17:58Z): User-performed live checks confirm that RowButtonNarrowStyle keeps all buttons on one row, Copy appends at the bottom with the proper name and correct source color/element mapping, and removing the copy leaves the original unaffected. This accepts those specific Milestone 5/5A scenarios. The unreported enablement, selected-copy, per-column sorted insertion, equal-width/minimum-size and runtime-output checks remain pending, as do automated validation of the new shared-style edits and later hierarchy/save/preview integration. Only plan evidence changed; no production code, Jira field, staging, commit or Milestone 6 work was performed.

Previous style-selection outcome (2026-10-12 17:52Z): RowButtonNarrowStyle is the selected reusable layout solution. The user added it to the shared theme and applied it to all five Display Setup item buttons. The proposed local styles were not left in the worktree; no agent production edit was needed for this request. Milestone 5A implementation/solution choice is recorded, while Release/diagnostic/live validation of these new user edits and the remaining Milestone 5 checks are not yet recorded. Milestone 7 now prescribes reuse in Custom Prop Editor, but no prop command or button was implemented. This turn updates only the plan, preserves all user edits, and stops for review without staging, committing, or updating Jira.

Previous button outcome (2026-10-12 17:33Z): The Display Setup State Item row now contains Add, Copy, Remove, then the existing reorder buttons, with the same button style and command-driven availability. The only production change is one XAML line. Release solution rebuilding, all 32 focused State mapper tests, and whitespace validation pass. Rider has no errors or ordinary warnings on the added line, but its unknown-DataContext weak warning is retained and cannot establish live binding success. Required real-window checks have been requested from the user because desktop control is unavailable; they are pending, not skipped or marked passed. Milestone 5 is implemented but not fully accepted. Stop here; Milestone 6, Jira updates, staging, and commits are not authorized by this execution.

Previous command outcome (2026-10-12 17:23Z): Milestone 4 is implemented and stops for review. Display Setup now exposes a tested Copy command for exactly one current-definition row, with an execution guard, source-based unused name, preserved color/assignments, fresh item identity and wrapper, bottom append, and sole selection. The same helper preserves Add's existing workflow. The focused suite passes 32 tests (10 new cases); expanded State regression suites pass 253 tests with no failures or skips. Release test compilation and whitespace validation pass. The new nullable test warning was fixed and all changed lines are free of ordinary warnings/errors; normal allocation weak warnings are explicitly retained. Gortex signature extraction/test mapping and broad-impact/co-change warnings are documented limitations. No Copy button or prop-editor command was added. Hierarchy edit-independence, Copy draft/save/discard/preview integration, and real UI validation remain in their specified later milestones. No Jira update, staging, or commit occurred; Milestone 5 is unstarted pending review.

Previous naming outcome (2026-10-12 17:13Z): Milestone 3 is implemented and stops for review. The new naming overload supplies unused names based on the complete source name without changing Add naming or the shared suffix algorithm. Full Release test compilation passed; all 17 focused naming tests and 140 combined State regression tests passed without failures or skips. Rider reported no errors/ordinary warnings, but the new test fixtures have 12 allocation weak warnings; production weak warnings are outside the added overload. Whitespace validation passed. Gortex's replacement-model signature results, missing test-edge mapping, and persistent broad-impact warning are explicitly recorded above. No command, UI, model, persistence, Jira, staging, or commit change is part of this milestone. Milestone 4 remains unstarted pending review.

Previous prerequisite outcome (2026-10-12 17:01Z): Milestone 2B is complete and user-accepted. Both editors share the user-verified one-time sorting, working manual arrows, bottom Add insertion, persistence, preview, and discard behavior. The Release test build and both focused suites passed (44 tests total, no failures or skips); changed-view Rider checks and whitespace validation passed. Jira's description was reconciled and read back with its title and Accepted status preserved. Manual evidence comes from the user's reports, not agent-driven desktop testing or invented per-column transcripts. The detailed final matrix remains in Milestone 10. No production changes were needed in 2B, no commit was made, and naming/Copy work remains unstarted at this review boundary.

Previous partial verification outcome (2026-10-12 16:56Z): The user confirmed one-time sorting and functional Up/Down arrows in Custom Prop Editor, matching State Property Setup. Milestone 2B's Release test build and both focused suites passed (44 tests total, no failures or skips), as did the changed-view diagnostics and whitespace check. Detailed Add/persistence/preview/definition-switch scenarios are not individually reported; clarification is pending before recording their completion or agreed deferral. Jira was read but not changed. Milestone 2B remains partially complete, and naming/Copy implementation has not begun.

Previous implementation outcome (2026-10-12 16:45Z): Milestone 2A is implemented: Custom Prop Editor's State Item grid explicitly enables the existing persist-then-clear behavior and retains its sorting command binding. State Property Setup remains at the user's restored baseline with an empty scoped diff. Release compilation and changed-line Rider checks passed, and whitespace validation passed. This proves configuration and compilation, not actual grid movement or insertion; Milestone 2B remains pending for both-screen interactive validation, focused regression suites, and Jira reconciliation. Execution stops at the Milestone 2A review boundary; no commit, Jira write, or naming/Copy work was performed.

Previous prerequisite state (2026-10-12 16:35Z): State Property Setup is back at baseline following the user's revert. Pending Milestone 2A changes only Custom Prop Editor's State Item sorting behavior; Milestone 2B compares it with the unchanged State Property Setup reference before naming/Copy work. This update changes only the plan. Completed milestones and prior review evidence remain historical records.

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


Milestones 1 and 2 have already been recorded as completed; their original descriptions below are protected execution history and must not be re-executed as current instructions. Corrective Milestones 2A and 2B are complete, with prerequisite user acceptance recorded above. Milestones 3 through 6 were implemented and reviewed through subsequent user milestone instructions. Milestones 5 and 5A are complete: Display Setup's Copy button and the user's reusable RowButtonNarrowStyle have passing manual acceptance, user-reported Release compilation, current diagnostics and focused regressions. Milestone 7 is now complete: Custom Prop Editor's Copy button is wired and user-validated for selection, bottom insertion regardless of sort, naming, independence, persistence/reload, and canvas assignments; its companion StateMapper layout uses the user's updated shared styles. The Release test-target build succeeded and Rider analysis found no errors, with static unknown-DataContext hints documented in Progress. Stop for scoped review and commit-message handoff; Milestone 8 remains next and requires explicit authorization. The sorting contract remains one-time ascending sort, visible manual reordering afterwards, and all new Add/Copy items at the bottom.

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

### Milestone 5A: Fit the Display Setup item buttons on one line


Context: The user reported wrapping after the Copy button insertion and requested narrower buttons that fit on one line at the default dialog size. The user subsequently created RowButtonNarrowStyle in src/Vixen.Common/WPFCommon/Theme/Theme.xaml and applied it to all five item buttons in src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml. Mark this shared style as the solution; preserve these user-owned edits and completed Milestone 5 history. Existing live checks remain pending unless actual evidence is supplied.

Plan of work: Reuse the user's RowButtonNarrowStyle, which sets MinWidth 75 and Margin 6,6,0,0 and retains the base Button theme. Display Setup's Add, Copy, Remove and both arrow buttons already use it. Do not add the superseded local 72/32 styles, force fixed widths, alter the original RowButtonStyle, change dialog dimensions, or touch commands/sorting/selection. The three text buttons must remain equally sized; both arrows must remain equally sized and may be narrower than the text buttons. The user's shared solution currently gives all five the same minimum, which is permitted. Actual width still depends on content and requires visual verification. Custom Prop Editor reuse belongs to pending Milestone 7, not this milestone.

Concrete steps: Inspect the scoped theme/view diff and preserve the user's changes. For the remaining validation, run Rider get_file_problems on both XAML files, rebuild Release with native MSBuild using the shared command, run existing focused State mapper regressions, and run git diff --check. Open the disposable State Property Setup window and verify all five buttons fit at default 920-by-560 size, the three text buttons have equal widths, and both arrows have equal widths. Retain Milestone 5's minimum-size usability and runtime-binding checks. Without desktop control, request user-performed evidence and leave live acceptance pending. The preceding build/test results predate this new style and must not be presented as validation of it.

Validation and acceptance: Source confirms the shared style exists and all five item buttons reference it. Their combined minimum width including margins falls from 530 to 405 layout units; this arithmetic does not prove actual rendering. Release compilation, focused tests, and changed-line diagnostics must be recorded for the new user edits. Actual default-size layout meets the no-wrap and equal-width requirements. Minimum-size usability and remaining Milestone 5 checks still need real-window evidence.

STOP BOUNDARY: Stop execution, inspect scoped git diff and git status, invoke the project commit-msg skill with VIX-4019 prefix, supply a complete paste-ready message, and wait for explicit human review. Do not create a commit or begin Milestone 6 without authorization and required acceptance.

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

Plan of work: Insert Copy immediately after the item Add button, binding CopyStateItemCommand. Reuse the user-created shared RowButtonNarrowStyle for the State Item button row, as selected in Milestone 5A, rather than introducing a separate local sizing solution or keeping the earlier RowButtonAutoStyle prescription. Retain Add's existing MinWidth binding to RemoveBtn.ActualWidth and give Copy the same binding so the text buttons remain aligned with Remove. Preserve equal sizing of the two arrow buttons; they may be narrower than the text buttons. Read the actual row and its width overrides before editing, and ensure no retained 100-unit minimum defeats the narrow style. Preserve the definition Copy menu and grid selection, and keep ClearSortAfterCommand true as established in Milestone 2A. No event handler or shared-theme rewrite is required.

Concrete steps: Run Rider file analysis, build the affected solution configuration, and run `git diff --check`. Open the Custom Prop Editor State tab and exercise zero/single/multiple selection and Copy.

Validation and acceptance: The Copy button matches the State Item Add and Remove buttons. At the editor's default size the item buttons fit on one line, the three text buttons have equal widths, and the two arrows have equal widths; controls remain usable at supported smaller sizes. Insertion after sorting appends at the bottom, matching the approved Milestone 2B Add baseline. The copied assignments appear on the drawing surface when previewing or editing the copied item. No new binding errors appear.

STOP BOUNDARY: Stop execution, inspect scoped git diff and git status, invoke the project commit-msg skill with VIX-4019 prefix, supply a complete paste-ready message, and wait for explicit human review. Do not create a commit or begin Milestone 8 without authorization. Follow the Review Stop Protocol below.

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

Execution evidence (2026-10-12 16:45Z, Milestone 2A): Initial status showed only the untracked plan, and both scoped view diffs were empty. The sole production edit adds `ClearSortAfterCommand="True"` to the State Item sorting behavior in `src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml`. Gortex impact reported low risk but warned of missing XAML graph edges; post-edit detection reported that XAML file with no changed symbol IDs. Symbol-level tests/guards/contracts are therefore not applicable; a file-target test query also confirmed that symbol IDs are required. Rider `get_file_problems` (`errorsOnly=false`) completed with 38 warnings/weak warnings, zero errors, and none on changed lines 431–432. Unchanged-line diagnostics were left intact.

The prescribed Release rebuild was run. Its large initial output was truncated, so the same rebuild was repeated with summary logging to capture reliable completion evidence:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -verbosity:minimal -consoleLoggerParameters:Summary
    73 Warning(s)
    0 Error(s)
    Time Elapsed 00:00:44.26
    Exit code: 0

`git diff --check` exited 0 with no output. The final scoped State Property Setup diff is empty; final status contains only the modified Custom Prop Editor XAML and the modified plan (the plan became tracked during execution). Plan post-edit detection returned three documentation IDs; their test query found no mapped tests, guard analysis reported no configured rules or violations, and contract analysis allowed the change. Automated tests and interactive sort/move/Add verification were not run because this UI-only milestone assigns them to Milestone 2B; none are claimed as passed. The project `catel-mvvm` skill kept the change on the existing attached behavior, and `commit-msg` was invoked for the review handoff. No staging or commit occurred. Progress and Outcomes & Retrospective now record implementation and its validation limits; the sorting decisions are unchanged.

Verification evidence (2026-10-12 16:56Z, Milestone 2B): Recorded the user's actual-grid one-time sort and arrow confirmation without extending it to unreported scenarios. Ran the following from `C:\Dev\Vixen`:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    Exit code: 0; Vixen.Tests.dll built successfully (warnings present).

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State.StateMapperDefinitionTests"
    Passed: 22, Failed: 0, Skipped: 0; exit code: 0; duration: 786 ms.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~App.CustomPropEditor.State.CustomPropStateEditorViewModelTests"
    Passed: 22, Failed: 0, Skipped: 0; exit code: 0; duration: 866 ms.

    git diff --check
    Exit code: 0; no output.

Rider `get_file_problems` completed for ElementTree.xaml with no errors and no problems on changed lines 431–432 (38 warnings/weak warnings elsewhere). Initial status and scoped diffs showed only the earlier milestone's Custom Prop Editor XAML setting and plan evidence; StateMapperView.xaml remains unchanged. Read VIX-4019's current description and confirmed its title and Accepted status; made no Jira update or comment. This follow-up changes only this plan's evidence. The user confirmation establishes sorting/manual-arrow parity; further manual evidence or explicit agreement to defer the remaining detailed cases is pending. Progress and Outcomes & Retrospective reflected that partial state at the time.

Acceptance and reconciliation evidence (2026-10-12 17:01Z, Milestone 2B): The user's follow-up confirmed Add at the bottom in both editors and working persistence, preview, and discard, completing the prerequisite review alongside the earlier sort/arrow report. Accepted this summary evidence without inventing separate per-column or definition-switch results. The full manual matrix will be repeated with Copy in Milestone 10. Re-read VIX-4019, verified no unrelated additions, updated only its description to the revised user-facing requirements in Artifacts and Notes, then read it back. The title remains “Ability to duplicate a State Item in a State Definition” and status remains Accepted. Description readback contains the one-time ascending sort, visible arrow movement, bottom Add/Copy insertion, and preserved Copy criteria. No Jira comment or status transition occurred.

Progress, Decision Log, and Outcomes & Retrospective now record completed 2B and its evidence source. The project Jira skill guided the concise user-facing reconciliation; the project commit-msg skill is used for the plan-only milestone handoff. The preceding successful build, 22+22 tests, and changed-view checks remain current because no production code changed; they were not rerun. Final git status/scoped diff and git diff --check are required for this evidence update. No staging, commit, or Milestone 3 work was performed.

Execution evidence (2026-10-12 17:13Z, Milestone 3): Initial `git status --short` was empty. Read the active naming contract, `.agents/PLANS.md`, the complete naming helper, and adjacent xUnit conventions before editing. The project dotnet-best-practices and csharp-docs skills guided null guards, collection expressions, focused AAA tests, sealed test class, and complete new API/test documentation. The sole production edit is the additive overload; the original overload and `GetNextName` are byte-for-byte unchanged. The new test file remains untracked and must be inspected through Gortex at the review stop.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    Exit code: 0; Vixen.Tests.dll built successfully (existing warnings present).
    Repeated the same build after final collection-expression/documentation adjustment: exit code 0.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State.StateItemCopyNamingTests"
    Passed: 17, Failed: 0, Skipped: 0; exit code: 0; duration: 47 ms.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State|FullyQualifiedName~App.CustomPropEditor.State"
    Passed: 140, Failed: 0, Skipped: 0; exit code: 0; duration: 899 ms.

    git diff --check
    Exit code: 0; no output.

Rider `get_file_problems` (`errorsOnly=false`) completed for both changed C# files: three existing production allocation weak warnings outside the added overload, and 12 test allocation weak warnings; no errors or ordinary warnings. No unrelated diagnostics were changed or suppressed. Gortex pre-edit impact and proposed-signature verification were performed, followed by change detection, tests, guards, and contract checks. No guard rules are configured. Its initial detect retry waited for the pending graph receipt, without restarting or reindexing the daemon. Its persistent impact warning and unmapped test coverage are documented under Surprises & Discoveries rather than claimed as clean checks. The 140-test run covers the graph-requested Property.State suite and Custom Prop State regressions. Debug/full-suite/interactive Copy validation remains reserved for later milestones, since this milestone introduces no visible Copy action. No Jira update, staging, or commit occurred.

Revision note (2026-10-12 17:13Z): Updated Progress, discoveries, decisions, current outcome, active milestone wording, and execution evidence for the implemented naming milestone. Preserved prior prerequisite history. Stop at Milestone 3 for scoped-diff review and the project commit-msg handoff before authorizing Milestone 4.

Execution evidence (2026-10-12 17:23Z, Milestone 4): Initial `git status --short` was empty. Read the milestone, relevant view-model implementation, existing test fixtures, and clone XML/implementation through Gortex before editing. Applied project catel-mvvm, dotnet-best-practices, csharp-docs, and csharp-async guidance: a lazy command/predicate in the view model, documented new public API/tests, ordinary awaited test actions, and direct Task forwarding without unnecessary async wrappers. The helper extraction was a guarded source edit, not a semantic rename/refactoring operation. The scoped diff confirms only the planned three C# files changed, with an unchanged clone body and no XAML edit.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    Initial build: exit code 0; Vixen.Tests.dll built, including one new CS8602 assertion warning.
    Initial focused run: 32 passed, 0 failed, 0 skipped; exit code 0; duration 850 ms.
    Fixed that changed-line warning and repeated the full MSBuild command: exit code 0.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State.StateMapperDefinitionTests"
    Final run: 32 passed, 0 failed, 0 skipped; exit code 0; duration 855 ms.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State|FullyQualifiedName~App.CustomPropEditor.State|FullyQualifiedName~Effect.State"
    Final expanded regressions: 253 passed, 0 failed, 0 skipped; exit code 0; duration 737 ms.

    git diff --check
    Exit code 0; no output.

Final Rider analysis (`errorsOnly=false`) completed for all three changed C# files. StateMapperViewModel has 108 allocation weak warnings and three existing ordinary warnings (lines 152, 166, 188); its changed lines have only four necessary lazy-command/fresh-wrapper allocation weak warnings. StateItemData has six allocation weak warnings and two existing nullable-contract warnings (lines 87–88); the changed documentation line has no diagnostic. StateMapperDefinitionTests has 146 allocation weak warnings and one existing redundant-cast warning at line 908; its added test block has 25 fixture/delegate/collection allocation weak warnings and no ordinary warning/error. No inspection suppression or unrelated cleanup was applied.

Pre-edit impact identified item-command notification consumers and existing cloning tests. Proposed new signatures were checked, but unresolved new symbols are not a successful API proof. Re-verification after indexing miscounts the helper's old parameters, as documented above. Post-edit detection and symbol test/guard/contract checks were performed; test-edge mapping is absent and no guards are configured. Contract review identifies broad State consumers and historical co-change suggestions; the expanded 253-test run exercises the recommended Property.State, CustomPropEditor.State, and Effect.State areas without expanding code scope. Persistent warnings are retained for human review. Full-suite/Debug/real Copy UI, hierarchy edit independence, draft persistence, preview, and discard checks remain assigned to later milestones; none is claimed as completed here.

Revision note (2026-10-12 17:23Z): Updated the living sections, active milestone wording, and actual validation evidence for Display Setup command completion, preserving all prior history. The project commit-msg skill supplies this milestone's paste-ready handoff. Stop at Milestone 4 with scoped status/diff review; do not begin the button milestone or create a commit without the required user instruction.

Execution evidence (2026-10-12 17:33Z, Milestone 5): Initial `git status --short` was empty. Read this plan, the complete `.agents/PLANS.md`, project catel-mvvm and commit-msg skills, and the complete StateMapperView.xaml through Gortex. Recalled earlier command work before continuing. The sole production edit adds the Copy button after State Item Add; no definition control, selection behavior, clear-after-sort setting, layout property, C# source, or Custom Prop Editor file changed. The Catel skill keeps availability in the existing command with no code-behind event or separate enabled binding.

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -verbosity:minimal -consoleLoggerParameters:Summary
    73 Warning(s), 0 Error(s); exit code 0; elapsed 00:00:40.11.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State.StateMapperDefinitionTests"
    Passed 32, failed 0, skipped 0; exit code 0; duration 861 ms.

    git diff --check
    Exit code 0; no output.

The full solution rebuild used native MSBuild and built the test assembly before the no-build regression run. No additional tests were added for this XAML-only milestone; existing command tests are regression evidence, not live UI acceptance. Rider `get_file_problems` (`errorsOnly=false`) completed: no errors, with an unknown-DataContext weak warning on the new line 184 and existing warnings elsewhere. The new binding references the existing public command, and neighboring bindings have the same static limitation. No suppression, design-time annotation, or unrelated diagnostic cleanup was applied.

Gortex pre-edit impact reported LOW risk with a possible extraction-gap caveat for StateItemsGrid. Post-edit detection found one modified XAML file and no changed symbols, so production symbol-based test/guard/contract checks have no applicable IDs. Plan evidence updates receive their own impact/detection/checks. Scoped diff review confirms the one-line insertion and unchanged sorting/selection. No desktop-control tools are callable; requested user checks in a disposable profile for styling/order, zero/single/multiple selection, copied values/selection, bottom insertion after Name and Count sorting, minimum 720-by-420 window usability, and no new binding errors in Rider application output. Those checks remain pending. No application-output inspection or actual grid/layout result is claimed. No Jira update, staging, commit, or Milestone 6 implementation occurred.

Revision note (2026-10-12 17:33Z): Updated all living sections and active execution wording to distinguish implemented/automatically validated Milestone 5 from its pending live acceptance. Preserved earlier history and the prescribed milestone scope. The project commit-msg skill supplies a paste-ready message for the scoped button/evidence changes; halt at this boundary for manual evidence and review before further implementation.

Revision note (2026-10-12 17:43Z): Applied the project revise-active-plan skill for the user's new no-wrap/equal-width requirement. Added bounded Milestone 5A with explicit local sizing, validation and review stop, and a dated Decision Log entry. Preserved committed Milestone 5 steps and evidence; no prior pending manual scenario is falsely accepted. The user's request authorizes implementation of this correction in the current turn, not advancement to Milestone 6.

Revision evidence (2026-10-12 17:52Z): The user interrupted the proposed local-style edit and supplied RowButtonNarrowStyle instead, requesting it be marked as the solution and reused in Custom Prop Editor. Gortex read confirms MinWidth 75 and Margin 6,6,0,0. Scoped git diff confirms the new theme style and its references on all five Display Setup item buttons, with no local resources or 72/32 overrides from the interrupted call. Preserve both user-owned production diffs unchanged. This request changes only the plan: update the solution/validation split, discoveries, decision, current outcome, active work, pending 5A and 7 instructions, and this evidence. Builds, tests, Rider application output, and actual layout checks were not run for these new user edits; they remain explicit pending validation, not inherited passes from the earlier button build. Git whitespace validation and Gortex documentation checks are recorded at the review stop. No Jira update, staging, commit, or Custom Prop Editor implementation is authorized.

Revision note (2026-10-12 17:52Z): Project revise-active-plan records the user's reusable narrow-button solution and supersedes uncompleted local sizing instructions without changing completed milestones. Original equal-width/default-size requirements remain intact; narrower arrows are optional, not required. Project catel-mvvm informed the read-only binding/style inspection, and project commit-msg supplies the plan-only review handoff. Plan revision complete and recorded in the Decision Log. Ready for coding model to implement the next milestone.

Manual acceptance evidence (2026-10-12 17:58Z): User report: “The new style works leaving all the buttons on the same row. Copy creates a new version at the bottom of the list with the proper name. The color and element mapping is also correct. It can be removed without affecting the original.” Record these exact outcomes as user-performed checks in State Property Setup. The report does not state exact window dimensions, equal-width measurement, enablement states, copied selection, preceding Name/Count sorts, application-output inspection, or save/preview/discard results. Removing the copy is not proof that deleting or editing the original leaves the copy independent; those later scenarios remain required.

Revision note (2026-10-12 17:58Z): Initial git status was clean. Updated Progress to split accepted live scenarios from remaining checks; added the evidence, decision, current outcome and active-work clarification without changing production or completed milestone steps. Applied project revise-active-plan and commit-msg guidance for this plan-only review update. Builds, tests and Rider code diagnostics were not rerun because no source changed; earlier runs are not retroactively credited to the new shared style. Scoped git diff/whitespace and Gortex documentation checks are the relevant validation for this update. No Jira update, staging, commit or next-milestone implementation occurred.

Final M5/M5A acceptance evidence (2026-10-12 18:06Z): The user first confirmed that zero, one and multiple selected rows properly enable/disable Copy. Follow-up: “The new state item copy is the selected row after adding to the bottom. It adds to bottom with or without sort. Text buttons have equal width and remain on the same line. No binding errors observed. The buttons wrap and remain usable when the form resizes down to the minimum. I ran the release build and it succeeded, so no need to do that again. Save / Discard preview and independence also pass.” Accept this as user-performed Display Setup evidence completing the remaining M5/5A acceptance; keep its summary granularity rather than inventing exact build commands/warning counts or individual Name/Count transcripts. The build was not repeated. The earlier report already confirms correct naming/color/mapping and removal of the copy without affecting the original.

Agent Rider get_file_problems with errorsOnly=false completed on both current XAML files. Theme.xaml has nine ordinary warnings elsewhere, zero errors and no diagnostic in the new narrow style. StateMapperView.xaml has 25 weak and 12 ordinary warnings, zero errors; item-button changes have only the known unknown-DataContext binding hints, not ordinary warnings or unresolved narrow-style errors. These static hints do not contradict the user's observed runtime success. Source read confirms both arrows share RowButtonNarrowStyle with 16-by-16 image dimensions. No diagnostic cleanup or production edit was made. Applied project catel-mvvm for this scoped inspection and commit-msg for the plan-only completion handoff.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Property.State.StateMapperDefinitionTests"
    Passed 32, failed 0, skipped 0; exit code 0; duration 798 ms.

This no-build rerun exercises the existing command regressions without repeating compilation; full native MSBuild has already built this test assembly, and no C# source changed in the styling/acceptance follow-ups. Initial git status contained only this plan's preceding evidence update, which was preserved. The user-reported save/discard, preview and independence results are retained as manual summary evidence for later milestones. Milestone 8 still requires its detailed new automated hierarchy and draft/save tests, and Milestone 10 still requires final two-editor end-to-end validation; neither is marked complete by this report.

Revision note (2026-10-12 18:06Z): Marked M5 and M5A complete in Progress, added the acceptance decision and evidence, updated discoveries/current outcome/active work, and preserved earlier history and production files. Scoped status/diff/whitespace and Gortex documentation checks complete this review handoff. No build repetition, Jira update, staging, commit or Milestone 6 implementation was performed.

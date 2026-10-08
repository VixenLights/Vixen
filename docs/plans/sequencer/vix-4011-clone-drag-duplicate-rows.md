# Keep cloned effects under the pointer across duplicate timeline rows (VIX-4011)


This ExecPlan is a living document maintained in accordance with `.agents/PLANS.md`. Keep Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective current. This document was prepared using `.agents/skills/analyze-and-plan-issue/SKILL.md`, the project .NET guidance, and the project C# documentation guidance. The current authorization covers Milestone 1 and, after the user's manual-reproduction confirmation, Milestone 2 implementation and validation. Milestone 3 and commits remain future work.

## Purpose / Big Picture


When one lighting element belongs to several expanded groups, the sequence editor displays its effects in several timeline rows. Users must be able to Ctrl+Drag or Ctrl+Shift+Drag a copy through those rows without the copy jumping away from the pointer. The other visible representations of that same effect may appear simultaneously, as they do during an ordinary move. After this fix, the representation under the pointer should continue to follow the pointer through consecutive upward and downward row transitions.

The issue is https://vixenlights.atlassian.net/browse/VIX-4011, titled “Ctrl Drag to clone an effect has issues when the same element is exposed multiple times in the sequencer tree”. Its status at analysis was Accepted. It contains no attachments, comments, or linked issues. The expected behavior is explicitly to match ordinary moving while placing the cloned effect; no product clarification is required.

## Progress


- [x] (2026-10-07) Read VIX-4011, the analysis skill, `.agents/PLANS.md`, and relevant VIX-3940 modifier/clone documentation.
- [x] (2026-10-07) Inspect the vertical movement method and identify a likely stale mouse-row anchor caused by comparing a moving clone with the original mouse-down effect.
- [x] (2026-10-07) Assess clone-path impact and check test discovery. Gortex maps no tests to the vertical movement method; Rider could not resolve its coverage symbol.
- [x] (2026-10-07) Save this analysis and executable plan. No production code or commit changed.
- [x] (2026-10-07) Align VIX-4011's Jira description with the user-visible scope, acceptance criteria, and validation scenario while preserving the original report.
- [x] (2026-10-07) Confirm from current source that clone selection switches to newly created `Element` objects while `m_mouseDownElements` remains the original mouse-down context.
- [x] (2026-10-07) Confirm that the editor row-change handler updates the target and synchronizes the same moved element across rows representing the old and new lighting elements.
- [x] (2026-10-07) Confirm the sequencer test framework is xUnit v3 with `Xunit.StaFact`; timeline-control tests use a non-parallel collection.
- [x] (2026-10-07) User confirmed manual reproduction of the defect; accepted as sufficient to proceed without recording unprovided coordinates/debugger observations.
- [x] Milestone 1: Align Jira requirements, confirm the clone identity handoff, and establish the failing interaction to the user's accepted manual-testing standard.
- [x] Milestone 2: Repair tracked-row advancement and verify regression, clipping, hidden-row, deprecated-target, ordinary-move, and no-op behavior.
- [x] (2026-10-08) User-reported M3 validation: full build succeeded and all 1,069 unit tests passed; timeline move, Ctrl-clone, and Ctrl+Shift-clone worked across single/multiple effects and repeated elements at several tree levels.
- [x] Milestone 3: Complete full-build/test and editor validation; report user-visible results in Jira without transitioning the issue.

## Surprises & Discoveries


- Observation: Vertical movement already distinguishes the visible row under the mouse from other representations of the same effect. Replacing the duplicate-handling algorithm is unnecessary.
  Evidence: `Grid.MoveElementsVerticallyToLocation`, `src/Vixen.Common/Controls/TimeLineControl/Grid.cs:2167`, passes `m_mouseDownElementRow` into both vertical-limit calculations and uses that row to prefer the matching representation during the move loop.
- Observation: The tracked row advances only when the moved effect matches the original mouse-down effect identity.
  Evidence: The same method contains the following comparison immediately after removing, adding, and notifying about a moved effect:

    if (m_mouseDownElements != null && element == m_mouseDownElements.FirstOrDefault())
        newMouseDownRow = visibleRows[i + visibleRowsToMove];

- Observation: The current clone handoff selects newly created elements but leaves the original mouse-down context intact.
  Evidence: `Grid.CloneSelectedElementsForMove` invokes `SelectedElementsCloneDelegate`, clears selection, then selects each returned element. `TimedSequenceEditorForm.CloneElements` clones effect data into a new effect instance, adds the new effect node to the source row, and returns the new timeline elements. `Grid_Mouse.OnMouseDown` is the only assignment site for `m_mouseDownElements`; clone initiation does not replace it. This confirms the identity mismatch described in the diagnosis.
- Observation: Row changes synchronize one logical effect across displayed rows that represent the same lighting element.
  Evidence: `TimedSequenceEditorForm.ElementChangedRowsHandler` changes the moved element's target, removes it from other rows representing the old target, and adds it to other rows representing the new target. This is why the duplicate row display does not imply extra logical clones.
- Observation: Sequencer tests run with xUnit v3 and WinForms STA support; the current timeline-control fixture is serialized.
  Evidence: `src/Vixen.Tests/Vixen.Tests.csproj` references `xunit.v3` and `Xunit.StaFact`. `GridMarkSnapPointTests` uses `Xunit.WinFormsFactAttribute`, `[Collection(TimelineControlTestCollection.Name)]`, and a helper that constructs the actual `Grid`; `TimelineControlTestCollection` disables parallelization. Gortex still maps no tests to `MoveElementsVerticallyToLocation`, and Rider `findTests` previously found no mapped test for that method. This is a coverage-discovery limitation, not evidence that no related tests exist anywhere in the repository.
- Observation: The user manually reproduced the defect and explicitly accepted that as sufficient to proceed; exact coordinates and debugger values were not captured in this plan.
  Evidence: User confirmation in the task conversation on 2026-10-07. No independent ordinary-move comparison was recorded, and no additional runtime details are claimed.

## Decision Log


- Decision: Repair the movement anchor using the source row of the actual moving selection, rather than rewriting the original mouse-down effect list to contain clones.
  Rationale: The source row is the visible representation the user is manipulating. It remains meaningful for both existing effects and clones. The mouse-down list may be used by selection, modifier, or completion behavior; avoid changing its semantics to fix a row-tracking defect.
  Date/Author: 2026-10-07 / Codex.
- Decision: Keep the existing clone delegate, duplicate-row preference, vertical boundaries, modifier locks, snapping, target validity checks, and undo routes.
  Rationale: The issue asks for clone dragging to match ordinary movement. VIX-3940 documents the established Ctrl+Shift vertical-only contract and clone-add/move undo behavior.
  Date/Author: 2026-10-07 / Codex.
- Decision: Treat the user's manual reproduction as sufficient to proceed, while keeping its unrecorded details and the ordinary-move comparison explicitly unclaimed.
  Rationale: The user confirmed the defect was manually reproduced and authorized moving forward. Automated characterization then demonstrated stale anchor state before the repair.
  Date/Author: 2026-10-07 / Codex.
- Decision: Keep milestone boundaries: finish M2 automated regression/build work, then stop for review before M3 editor validation.
  Rationale: The user authorized moving on after accepting manual reproduction, not execution of M3's full editor workflow or Jira completion reporting.
  Date/Author: 2026-10-07 / Codex.
- Decision: Update the Jira description during Milestone 1, and reserve issue comments or workflow transitions for later milestones if authorized.
  Rationale: The user explicitly authorized Milestone 1. The issue description now states user-visible behavior and acceptance criteria; no issue status transition or completion comment was made.
  Date/Author: 2026-10-07 / Codex.

## Outcomes & Retrospective


Milestone 1 requirements were aligned in Jira and source inspection confirmed clone identity handoff, shared-row synchronization, and test conventions. The user confirmed manual reproduction and accepted it as sufficient to proceed; exact interaction coordinates and an independent ordinary-move comparison were not recorded. In Milestone 2, the repeated-row clone regressions failed on the unfixed implementation with stale-anchor assertions and passed after the row-identity-based advancement change. Seven focused tests now cover consecutive moves, upward/downward transitions, repeated-row starts, ordinary movement, multi-effect selection, no-op movement, the last row, clipped displacement, hidden repeated occurrences, and deprecated-target refusal without changing either tracker. For Milestone 3, the user reported that the full build completed and all 1,069 unit tests passed. In the timeline, they tested ordinary drag movement, Ctrl-drag cloning, and Ctrl+Shift cloning with one or many effects across multiple duplicate-element arrangements at different tree levels; all behaved as expected. This plan records the user-reported results and does not claim an independent rerun. A concise user-facing Jira comment records these validation results; issue status remains Accepted.

## Context and Orientation


Vixen is a Windows desktop light-show editor. The timeline grid is a legacy WinForms control embedded in the application. A timeline `Element` represents an effect; a `Row` represents one displayed occurrence of a lighting element or group. Several displayed rows can contain the same timeline effect object. Those repeated representations are expected and must not become independently cloned effects.

`src/Vixen.Common/Controls/TimeLineControl/Grid.cs` owns rows, selected effects, cloning through a delegate, and vertical movement. Its `CloneSelectedElementsForMove` begins at analysis line 828 and `MoveElementsVerticallyToLocation` begins at line 2167. `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` owns the interaction state machine. `HandleMouseMove` begins at analysis line 374; `MouseMove_DragMoving` begins at line 858 and calls vertical movement. The auto-scroll timer also calls `HandleMouseMove`, so auto-scroll needs regression coverage.

The important private state is `m_mouseDownElements`, the original effects beneath the pressed pointer; `m_mouseDownElementRow`, the displayed row used as the drag anchor; and `CurrentRowIndexUnderMouse`, the row index used to calculate movement. An anchor is the displayed occurrence from which subsequent movement is measured. The method calculates the difference between destination and current visible-row indexes, limits that difference at the grid boundaries, prefers effects in the anchor row, moves each effect once, and then updates the tracked row state.

The existing final identity comparison can leave the row anchor behind when selection has changed to clones. On the next move, duplicate selection preference and boundary calculations refer to the wrong row even though `CurrentRowIndexUnderMouse` has advanced. Once the copied effect appears in several rows, choosing another occurrence can produce the reported jump.

The editor clone route documented by VIX-3940 is `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`, where `SelectedElementsCloneDelegate` is assigned to `CloneElements`. Existing clone-add undo is in `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedUndoAction.cs`; move undo is in the neighboring `ElementsTimeChangedUndoAction.cs`. These are inspection and validation targets, not planned modification targets.

Before implementation, use Gortex recall for prior VIX-4011 work, then a Gortex task query and at most one bounded follow-up to inspect the current clone route, row sharing and notifications, and relevant test fixtures. Do not reopen indexed source through shell searches. Inspect git status and preserve unrelated work. Use Gortex impact before any edit, mutate through Gortex edit/refactor, and run detect followed by tests, guards, and contract for the returned changed symbols. Any new signature also requires verify. Repository line numbers are navigation hints and can drift.

## Architecture Design: VIX-4011


Detected IDE Environment: Rider, with test-discovery and file-problem automation available. Use Rider's Apply snippet from chat for small reviewed C# updates when implementing, subject to the mandatory Gortex mutation workflow.

Core Strategy: Maintain the visible drag anchor as row state during the existing movement operation. The project .NET guidance allows maintenance of this WinForms control; this bug does not require a new UI, MVVM layer, interface, or lifecycle. The C# documentation skill applies to the public movement method's changed behavior.

Data Model & Property Contracts: Add no serialized fields, preferences, dependencies, or public signatures. Keep `m_mouseDownElements` as the original pointer-down context. Keep `m_mouseDownElementRow` synchronized with the successfully moved selection from that row. Keep blocked or zero-displacement moves from advancing either row tracker.

Mathematical / Boundary Logic: Retain the existing visible-row displacement, displacement clipping, and duplicate-count calculations. During a successful move from visible row i to visible row i plus the clipped displacement, update the next anchor to that destination if the source is the current anchor row and the effect belongs to the previously captured moving selection in that row. Compare row references; do not match display names, element tags, or the original effect identity. Use the destination after clipping, not the raw pointer destination. A row index in the full row list must not be substituted for a visible-row index.

Subsystem Component Matrix:

| Component | Planned change or verification |
| --- | --- |
| Grid.cs / MoveElementsVerticallyToLocation | Advance the mouse-row anchor from the actual source/destination of moving effects; document the contract. |
| Grid_Mouse.cs / HandleMouseMove and MouseMove_DragMoving | Verify the clone handoff and repeated movement; no planned modifier or state-machine rewrite. |
| Row.cs / element membership and notifications | Inspect sharing behavior so the test models duplicate rows accurately; no planned production edit. |
| TimedSequenceEditorForm.cs / CloneElements and row-change handling | Verify clone selection, shared target updates, and undo; no planned production edit. |
| src/Vixen.Tests/Sequencer/GridCloneDragDuplicateRowsTests.cs | Add focused regression coverage following the actual neighboring test infrastructure. |

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Milestone 1: Record requirements and establish the failing interaction


Context: Read the current clone route and existing timeline test setup using Gortex, including the production files identified above and relevant fixtures under `src/Vixen.Tests`. Reconcile source with `docs/plans/sequencer/vix-3940-ctrl-shift-clone-vertical-drag.md`; retain its Ctrl+Shift time lock and modifier semantics. Confirm that the effects selected after cloning are new objects and that the original mouse-down list remains original context. Confirm how row-change events propagate an effect to the other rows for the same lighting element.

Plan of Work: Milestone 1 was authorized and the VIX-4011 description was updated using the project Jira skill. The original report was preserved, and the description now states the user-facing requirements and acceptance criteria: both clone gestures follow the pointer through repeated visible occurrences, in either direction; simultaneous display on the same lighting element remains expected; originals remain in place; Ctrl+Shift preserves times; ordinary moves, boundaries, blocked targets, and undo remain consistent. A short user-facing validation description was included. Formulas and class names remain in this plan.

Concrete Steps: In Rider, reproduce on a disposable sequence with an element exposed in two or three expanded groups, separated by other rows. Start from a different element and clone-drag into the first repeated occurrence, continue to an intervening row, then cross the other occurrence. Repeat from the last occurrence upward and begin directly on each occurrence as well. Record the pointer row, current anchor row, selected clone identity, and original mouse-down identity at consecutive moves. Use read-only debugger inspection if needed; read the debugging skill before using debugger tooling. Do not save accidental sequence changes.

Validation and Acceptance: Record at least one reproducible failure before repair and verify that ordinary dragging of an existing effect through the same rows succeeds. If current source already remaps the mouse-down list or advances the row by another route, revise the diagnosis before changing code; do not apply the proposed patch without evidence. Record actual test framework, fixture thread requirements, row-sharing setup, and reproducible coordinates in this plan.

STOP HERE for manual review and commit execution before proceeding. Halt code execution. Run `git status --short` and `git diff -- docs/plans/sequencer/vix-4011-clone-drag-duplicate-rows.md` from `C:\Dev\Vixen`. For repository changes completed in this milestone, read and invoke the project `commit-msg` skill with VIX-4011 as the subject prefix and output its paste-ready Commit message block. Do not create a commit unless requested. Pause for explicit confirmation before the next milestone, as required by the analysis skill.

### Milestone 2: Repair anchor advancement and prove consecutive moves


Context: The production edit is limited to `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`, inside `MoveElementsVerticallyToLocation`. The regression fixture is `src/Vixen.Tests/Sequencer/GridCloneDragDuplicateRowsTests.cs`, matching existing sequencer tests. No project or package dependencies were added.

Plan of Work: First add a failing regression that exercises the real vertical movement method on consecutive transitions, with a cloned effect selected and the original effect retained as mouse-down context. Use the existing fixture conventions and thread setup; instantiate the actual Grid, Row, and Element types using their current constructors. Model duplicate membership through the actual row-sharing behavior or the equivalent existing host notification path. A test that has two unrelated effects with matching labels does not reproduce this bug. Use an existing test seam where available. Otherwise initialize the small amount of private drag state in a fixture helper using reflection, keeping reflection confined to tests and adding no public production API. Specifically initialize `m_mouseDownElements`, `m_mouseDownElementRow`, and `CurrentRowIndexUnderMouse` consistently with clone initiation.

Exercise at least two moves: the first places the clone on a repeated lighting element, and the next leaves that occurrence. Assert that the final target follows the pointer occurrence rather than the first or last other representation; also assert the anchor after each accepted transition. Cover upward and downward movement and starting on a repeated occurrence. Assert one logical clone, untouched originals, and correct target membership. Include the non-clone comparison and a multi-effect selection with several effects in the anchor row so all resolve to the same destination.

Replace the condition that advances `newMouseDownRow` by original effect identity. Capture source and destination rows, and whether the moved element belongs to the current anchor row's captured moving selection, before removing/adding elements or invoking row-change notifications. After a successful transfer, assign the destination to `newMouseDownRow` when that captured predicate is true. Preserve the existing final assignments of `CurrentRowIndexUnderMouse` and `m_mouseDownElementRow`. Leave duplicate preference, event order, clipping, and target validity checks intact.

The intended predicate uses the existing selection snapshot:

    var sourceRow = visibleRows[i];
    var destinationRow = visibleRows[i + visibleRowsToMove];
    var advancesMouseRow = ReferenceEquals(sourceRow, m_mouseDownElementRow)
        && ElementsToMoveInMouseRow.Contains(element);

Use these locals in the existing successful transfer branch and replace only the identity-based anchor update:

    if (advancesMouseRow)
        newMouseDownRow = destinationRow;

This is the implementation applied in `Grid.MoveElementsVerticallyToLocation`; the regression suite verifies it against the real movement method and the fixture's host-equivalent row-sharing event. Multiple effects moving from the anchor row have the same destination; they must not advance the anchor multiple row distances. Other selected effects must not replace that anchor.

Add XML documentation to the public method in accordance with the project `csharp-docs` skill: describe moving effects relative to visible rows, document both parameters, explain duplicate occurrences and tracked-row advancement, and document the existing exception if an effect would move off-grid with no alternative instance. Do not document a return value for this void method or modify unrelated APIs.

Concrete Steps: From `C:\Dev\Vixen`, build the test target with full Visual Studio MSBuild because two dependencies require the C++/CLI toolset, then run the built tests. If MSBuild is not on PATH, use its installed full Visual Studio path rather than dotnet's bundled MSBuild. Start long-running commands with a 10-second yield and poll completion with at least a 30-second yield.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~GridCloneDragDuplicateRowsTests"

Expected results are Build succeeded and all tests in the regression fixture passed after the fix. Capture the actual counts. First run the regression on the unfixed implementation and record a failure caused by incorrect row placement or stale row state, not a constructor, thread, or fixture setup error. Rebuild after the production repair and rerun once. Run Rider `get_file_problems` for each changed C# file, inspect all results, and resolve only issues introduced within changed lines. Complete Gortex detect, tests, guards, and contract checks and record any uncovered behavior requiring editor validation.

Validation and Acceptance: The seven fixture tests establish the clone regression failed before the repair and passes afterward. Coverage includes clone moves in both directions, starting on a repeated occurrence, ordinary non-clone movement, multi-effect selection, unchanged destination, reaching the last visible row, displacement clipped by a multi-row selection, a hidden repeated occurrence, and a deprecated destination. Refused deprecated moves assert both the anchor row and pointer row remain unchanged. The tests exercise the production Grid movement method.

STOP HERE for manual review and commit execution before proceeding. Halt code execution. Run `git status --short` and a scoped `git diff` for Grid.cs, the regression fixture, and this plan. Invoke the project `commit-msg` skill with VIX-4011 as the subject prefix and output the required paste-ready Commit message block. Do not commit unless requested. Pause for explicit confirmation before advancing.

### Milestone 3: Validate the editor and close the issue reporting loop


Context: The common grid is consumed by the sequence editor, and the host supplies shared-row synchronization, clone creation, and undo. Unit tests alone cannot prove the full pointer/modifier workflow.

Plan of Work: Build the affected solution configuration and exercise the repeated-element sequence in the editor. Run the test suite once after the focused regression passes. Update this plan with actual commands, counts, editor results, and any deviations. Following authorization to execute the Jira milestones, reconcile the Jira description with any agreed requirement changes and add a concise completion comment describing user-visible results and validation. Do not transition or close the issue unless that action is authorized.

Concrete Steps: The user reports the full build completed and all 1,069 unit tests passed. They tested timeline drag movement, Ctrl-drag cloning, and Ctrl+Shift cloning with one or multiple effects across several duplicate-element variations at different levels in the tree. All tested variants behaved the same and worked as expected. These are user-reported results; the agent did not independently rerun the full build, suite, or UI scenarios. Jira received a completion comment with these user-visible results; the issue status was not changed.

Validation and Acceptance: The dragged copy follows the pointer row throughout successive transitions, including while auto-scrolling. Expected repeated representations remain visible on the shared lighting element. Ctrl+Shift retains start time and duration; ordinary Ctrl+Drag still permits horizontal movement. Originals remain in place. No extra logical copies are created as repeated representations appear. Normal movement and the documented modifier combinations retain their behavior. Blocked targets and grid edges behave as before. Final placement, undo, and redo restore the expected targets and times.

STOP HERE for manual review and commit execution before proceeding. Halt code execution. Run `git status --short` and the scoped final diff. For milestone repository changes, invoke the project `commit-msg` skill with VIX-4011 as the subject prefix and output its paste-ready Commit message block. Do not create a commit unless requested. Report the actual validation results and pause for explicit confirmation before any further work.

## Validation and Acceptance


The decisive automated test is a real repeated-row movement sequence using a new selected clone while mouse-down context still refers to the source effect. A single initial move is insufficient because the row state is used again on the next transition. Final acceptance combines that failing-before/passing-after regression with the actual editor workflow in Milestone 3. Do not claim verification based solely on compilation or test discovery.

## Idempotence and Recovery


Analysis and build commands can be repeated safely. Inspect existing files and user changes before implementing; do not overwrite an existing fixture or unrelated plan edits. Reuse a disposable sequence for editor validation and discard unintended sequence changes. If the failing test demonstrates a different cause, update the hypothesis and decision log before expanding the production change. Never restart or re-track Gortex to resolve a discovery limitation. Preserve unrelated work when reverting only this task's changes through the normal reviewed edit workflow.

## Artifacts and Notes


The working tree was clean before this plan was created:

    git status --short
    [no output]

Gortex mapped the clone entry point to `Grid_Mouse.HandleMouseMove` and its normal/auto-scroll callers. Its edit-plan operation also listed other controls with similarly named mouse methods; those controls are outside the issue and must not be modified merely because they appeared in that list.

Rider test discovery did not identify an existing fixture for the movement method. The clone selection, editor clone delegate, and shared-row handler were confirmed from source. The user manually reproduced the defect; the exact sequence, coordinates, and debugger observations were not captured here. M2 automated validation recorded the full-MSBuild `Vixen_Tests` target build and the filtered regression run (7 passed). Rider file-problem analysis of the new fixture returned no errors; remaining suggestions are test-only style/allocation hints. Rider continues to report existing Grid.cs errors on unrelated lines. M3 editor validation was reported by the user: the full build succeeded, all 1,069 unit tests passed, and timeline movement, Ctrl-drag cloning, and Ctrl+Shift cloning behaved as expected across one or multiple effects and duplicate-element arrangements at different tree levels. Results were recorded in Jira; issue status remains Accepted.

## Interfaces and Dependencies


Retain `public void MoveElementsVerticallyToLocation(IEnumerable<Element> elements, Point gridLocation)` and `CloneSelectedElementsForMove` without signature changes. Retain existing events and delegate contracts. Add no dependency, serialization change, settings migration, or project. Use the installed repository test framework and existing Windows/thread initialization, determined from the test project before creating the fixture. Use the existing editor clone and undo routes.

## Plan Revision Note


2026-10-07: Initial plan records the stale-anchor cause, minimal row-based repair, evidence limits, and Jira milestones. Milestone 1 aligned Jira and verified clone identity handoff, row synchronization, and test conventions. The user confirmed manual reproduction. M2 added the source/destination-row anchor update, XML documentation, and seven passing regression/edge-case tests after observing the targeted clone tests fail before repair. Coverage includes both movement directions, repeated-occurrence starts, ordinary movement, multi-effect selection, no-op movement, last-row movement, clipped displacement, a hidden repeated occurrence, and a deprecated destination. The fixture must mark `VisibleRows` dirty after directly adding test rows; otherwise `rowAt` sees an empty cached list and the test only exercises setup failure. M3 validation was reported by the user on 2026-10-08: full build succeeded, all 1,069 unit tests passed, and timeline movement, Ctrl-drag cloning, and Ctrl+Shift cloning were exercised with one or multiple effects across duplicate-element arrangements at different tree levels. The user reported all behaved as expected. These results were added to Jira comment 40575; the issue remains Accepted. This validation was not independently rerun by the assistant.

Analysis complete and plan integrated with plans.md.

# VIX-3985: Incrementally update Mark grid snap points during drag

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document according to `docs/plans/PLANS.md` (the repository guidance is `.agents/PLANS.md`). It is intentionally a separate plan from Remediation A: A is validated, and the profile evidence now identifies the Mark grid as the next independent bottleneck. Remediation C is not authorized by this plan.

## Purpose / Big Picture

Users editing a sequence with many Marks should see the Mark-bar lines, ruler, waveform guides, and grid alignment lines follow a drag or resize together. After this change, moving one or a few Marks updates only their corresponding grid snap points instead of rebuilding snap points for every Mark on every mouse move. The interaction must keep its current movement, resize, Alt glued-resize, multi-select, snapping, auto-scroll, undo, and collection-style behavior.

The user can observe the improvement by continuously dragging a Mark in a dense sequence: the grid lines should keep up with the pointer and the waveform/ruler, while an equivalent 10-second dotTrace capture no longer contains `Grid.CreateSnapPointsFromMarks` under the live `MarksMoving` event path.

## Progress

- [x] (2026-08-15 20:30Z) Completed and manually validated Remediation A; the user reported a successful full build, all 728 unit tests passing, and no noticeable waveform regression.
- [x] (2026-08-15 20:30Z) Analyzed `C:\Dev\Snapshots\MarkBar\vixen-marksbar-mark drag-A.dtp` and identified full Mark-grid snap-point rebuilding as the remaining drag bottleneck.
- [x] (2026-08-15 20:30Z) Created this implementation plan for the evidence-supported Remediation B.
- [x] (2026-08-15 20:43Z) Updated VIX-3985 with the user-facing Remediation B scope, acceptance criteria, and validation plan.
- [x] (2026-08-15 20:59Z) Added the Mark snap-point registration foundation and deterministic full-rebuild/scale-change tests; the focused sequencer suite passed 22 tests with zero failures.
- [ ] Make `Grid` own live, incremental snap-point maintenance and remove the editor form's duplicate live full rebuild.
- [ ] Run focused and complete automated validation, perform the manual sequencer scenarios, and capture a comparable 10-second replacement profile.
- [ ] Adjust VIX-3985 if needed and add a concise user-facing validation comment; decide from the profile whether Remediation C needs a separate plan.

## Surprises & Discoveries

- Observation: Remediation A removed the waveform as the material source of drag delay, but did not make the grid lines visibly keep up with the grid's alignment lines.
  Evidence: The user reported that the Mark bar, waveform alignment, and ruler lag the alignment lines in the grid after validating A.

- Observation: The controlled replacement capture attributes nearly the entire live mouse-move branch to a full Grid snap-point rebuild.
  Evidence: In `vixen-marksbar-mark drag-A.dtp`, `MarksBar.MouseMove_DragMoving` has 1,108.8 ms total sampled time. `Grid.CreateSnapPointsFromMarks` has 1,079.5 ms total / 882.1 ms own time, whereas `Waveform.WaveFormSelectedTimeLineGlobalMove` has 6.0 ms.

- Observation: A snap-point time can have more than one `SnapDetails` entry.
  Evidence: `Grid.StaticSnapPoints` is `SortedDictionary<TimeSpan, List<SnapDetails>>`, so removing an entire time key for one moving Mark would incorrectly remove another Mark's grid line when the Marks share that time.

- Observation: The live event already orders affected parent collections before its subscribers run.
  Evidence: `TimeLineGlobalEventManager.OnMarksMoving` calls `EnsureOrder()` for each distinct `e.Marks` parent before raising `MarksMoving`; `Grid.CreateSnapPointsFromMarks` redundantly orders every visible-grid-line collection.

- Observation: Replacing every `SnapDetails` object when the pixel-to-time scale changes would leave a Mark registration pointing to stale objects.
  Evidence: The former `RecalculateAllStaticSnapPoints()` created a new dictionary and new details, while registrations must later remove the exact objects that remain in the live dictionary.

## Decision Log

- Decision: Implement only Remediation B in this plan; keep presentation coalescing (Remediation C) out of scope.
  Rationale: The replacement profile identifies one concrete full-rebuild bottleneck. A timer would add latency and lifecycle complexity without evidence that incremental updates themselves are still material.
  Date/Author: 2026-08-15 / Codex.

- Decision: Put the live update in `Grid` rather than in `TimedSequenceEditorForm`.
  Rationale: `Grid` owns `StaticSnapPoints`, its rendering invalidation, and Mark-collection configuration. The editor form should not rebuild Grid-internal state for every pointer event.
  Date/Author: 2026-08-15 / Codex.

- Decision: Track exact `SnapDetails` instances per contributing Mark and remove entries by reference.
  Rationale: Time is not a unique owner key; two Marks at the same start or end time must retain each other's snap entries when only one moves.
  Date/Author: 2026-08-15 / Codex.

- Decision: Retain `TimedSequenceEditorForm.TimeLineGlobalMoved`'s completed-operation call to `UpdateGridSnapTimes()` in the first implementation.
  Rationale: It is an inexpensive mouse-up correctness backstop while the new live registration index proves itself. The editor's `MarksMoving` subscription is removed so it cannot duplicate live work.
  Date/Author: 2026-08-15 / Codex.

- Decision: Describe B in VIX-3985 as an outcome for users and reviewers, while retaining implementation mechanics only in this ExecPlan.
  Rationale: The issue now states the expected responsiveness, preserved editing behavior, and validation result without coupling review to Grid internals or prematurely committing to C.
  Date/Author: 2026-08-15 / Codex.

- Decision: Preserve `SnapDetails` identity when the time-per-pixel scale changes by updating the snap windows in place.
  Rationale: A later live update can reliably remove the objects registered for a Mark, while the computed snap windows still reflect the current scale and snap strength.
  Date/Author: 2026-08-15 / Codex.

## Outcomes & Retrospective

Milestones 1 and 2 are complete. The Grid full-rebuild path now records the exact start/end details contributed by each eligible Mark, batches its own rebuild invalidation, and retains those exact objects when scale changes. `GridMarkSnapPointTests`, waveform tests, and auto-scroll tests passed 22 focused tests with zero failures. Live `MarksMoving` ownership and editor-handler removal remain for Milestone 3. Remediation A reduced the sampled `MouseMove_DragMoving` subtree from 4,886.3 ms to 1,108.8 ms (about 77.3%), but the remaining full grid rebuild still causes visible lag. Record changed files, full test results, manual findings, and the comparable replacement profile after subsequent milestones. State explicitly whether the evidence warrants a separately approved Remediation C plan.

## Context and Orientation

The Timed Sequence Editor is a WinForms timeline embedded in Vixen's desktop application. A Mark is a timed sequence annotation. A Mark Collection groups Marks and supplies its grid-line style and whether start and end (tail) lines should participate in snapping. A snap point is a grid-owned `SnapDetails` object that records a time, snapping window, and visual line style.

`src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs` raises two relevant events. `MarksMoving` is raised repeatedly while the pointer moves; before doing so, it orders each affected Mark's parent collection. `MarksMoved` is raised once when the interaction completes and is used for undo and final bookkeeping. Those event contracts must not change.

`src/Vixen.Common/Controls/TimeLineControl/Grid.cs` owns `StaticSnapPoints`, a sorted dictionary from a time to a list of `SnapDetails`. It currently constructs all entries in `CreateSnapPointsFromMarks()` by clearing the dictionary and iterating every eligible Mark Collection and Mark. `AddSnapPoint`, `RemoveSnapPoint`, and `ClearSnapPoints` each invalidate the Grid, so a full rebuild currently requests many invalidations. Its `RecalculateAllStaticSnapPoints()` replaces each `SnapDetails` object when the pixel-to-time scale changes.

`src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` currently subscribes to `MarksMoving` and calls `UpdateGridSnapTimes()`, which calls `Grid.CreateSnapPointsFromMarks()` for every mouse move. It also performs the same full rebuild after `MarksMoved`, which this plan intentionally preserves initially.

`src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` publishes movement events and is not to be throttled or otherwise changed by this work. The auto-scroll path replays the latest mouse move, so the live Grid subscriber must be safe for repeated events and must remain UI-thread-only.

The existing Remediation A plan is `docs/plans/sequencer/vix-3985-marks-bar-drag-performance-remediation-a.md`. It contains the original baseline and confirmed A validation. This document incorporates the relevant facts so it can be implemented independently.

## Plan of Work

### Milestone 1: Record the user-visible scope in VIX-3985

Before code changes, update VIX-3985's description in user-facing language. Explain that the remaining work makes Mark grid lines follow edits promptly in dense sequences, preserves all existing editing and undo behavior, and will be verified with focused tests, the complete suite, manual sequencer checks, and a comparable profile. State that Remediation C is not part of this phase. Do not put class names, implementation choices, or profiling internals in Jira. Record the update in this plan's Progress and Decision Log.

Acceptance: VIX-3985 makes the next user outcome and validation approach understandable without reading repository code.

### Milestone 2: Build a safe, testable incremental snap-point index

Create `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarkSnapPointRegistration.cs`. Define one internal, non-serialized type that holds the exact start `SnapDetails` and optional end `SnapDetails` created for one `IMark`. Keep its name and visibility minimal; it is Grid implementation state, not a new public model contract.

In `Grid.cs`, add a private dictionary keyed by `IMark` that maps to this registration. Use the default reference identity of the existing Mark instances; do not key by time. Refactor the existing insertion path into a private non-invalidating helper that creates a `SnapDetails`, appends it under its time key, and returns that exact object. Keep public `AddSnapPoint` behavior unchanged by letting it call the helper and invalidate once.

Add private batch helpers that clear the dictionary and registration map together, remove one registered detail by object reference from the list at its stored time and delete the dictionary key only if the list becomes empty, and invalidate once after a changed batch unless `SuppressInvalidate` is true. Refactor `CreateSnapPointsFromMarks()` to use these batch helpers: full rebuild remains the implementation for initial configuration, collection membership changes, collection/decorator property changes, and snap-strength changes. While rebuilding, record a registration for every eligible Mark. Remove its redundant `mc.EnsureOrder()` call because the sorted dictionary provides time ordering and live event dispatch already orders moved parents.

Preserve registration object identity across `RecalculateAllStaticSnapPoints()`. Prefer recalculating each existing `SnapDetails.SnapStart` and `SnapDetails.SnapEnd` in place rather than replacing `StaticSnapPoints` with freshly allocated details. If the existing method cannot be safely changed that way, explicitly call the full rebuild routine that recreates both dictionary entries and registrations after a time-per-pixel change. The final behavior must make a later incremental removal find the exact registered object.

Add `src/Vixen.Tests/Sequencer/GridMarkSnapPointTests.cs`, using `TimelineControlTestCollection` and existing sequencer test patterns. Tests must create a Grid/Timestamp control with eligible Mark Collections and prove: initial full registration; one moving Mark removes its old start/end entries and creates new ones; a non-moving Mark's `SnapDetails` object remains the same reference; duplicate times retain the non-moving Mark's detail; disabled start lines and disabled tail lines create no inappropriate registrations; distinct multi-selection Marks across multiple parents update once; a time-scale change does not break later removal; and disposal removes the live event subscription. Use `InternalsVisibleTo` internal diagnostics only if a deterministic test cannot otherwise inspect behavior; do not promote production state to public API merely for tests.

Acceptance: the new tests fail against the former full-rebuild-only behavior where meaningful, then pass with a registration that correctly represents all grid-visible Mark boundaries.

### Milestone 3: Subscribe Grid to live moves and remove duplicate editor work

In `Grid`'s construction path, subscribe to `_timelineGlobalEventManager.MarksMoving`; in `Dispose(bool)`, unsubscribe it along with the existing alignment subscription. Add a private handler that treats `e.Marks` as the complete set of Mark instances affected by this pointer update. Deduplicate those instances by reference before processing, which protects multi-selection and glued-resize event data from accidental duplicate work.

For each distinct moving Mark, first remove its prior registration if one exists. Remove the registered start and optional end detail by reference, then remove the Mark's dictionary entry. If the Mark's parent does not show grid lines, stop there. Otherwise create and register a current start detail from the parent collection's level and decorator; create/register the end detail only when tail grid lines are enabled. Do not call public single-entry invalidating APIs inside this loop. After processing all Marks, invalidate Grid at most once if the batch changed state and invalidation is not suppressed.

In `TimedSequenceEditorForm.cs`, remove the `MarksMoving += TimeLineGlobalMoving` subscription and delete the now-unused `TimeLineGlobalMoving` handler. Do not remove `TimeLineGlobalMoved` or its `UpdateGridSnapTimes()` call: it remains the completed-operation consistency rebuild and supports undo semantics without occurring at pointer frequency.

Exercise the focused test class after this milestone. Add any missing cases discovered in test setup before proceeding; do not suppress failures or make event delivery asynchronous.

Acceptance: a live `MarksMoving` event mutates snap entries only for its Marks, `CreateSnapPointsFromMarks()` no longer appears in the live editor event handler, all lines retain correct styles, and no disposed Grid receives later events.

### Milestone 4: Verify behavior and performance; close the B phase in VIX-3985

Run the focused tests, then build and run the full test project using the repository's full-MSBuild-before-`dotnet test` sequence. Launch Vixen and open a dense sequence with audio, many visible Marks, grid lines, and a viewport narrower than the timeline. Continuously drag a Mark, resize both edges, Alt glued-resize, move multiple selected Marks from one and multiple collections, and auto-scroll in both directions. Release and undo each operation. Confirm grid lines, Mark-bar lines, ruler, waveform guides, and snapping reflect the current Mark positions with no stale old start/end lines.

Capture a comparable 10-second dotTrace profile using the same sequence, zoom, visible range, and approximate pointer movement as `C:\Dev\Snapshots\MarkBar\vixen-marksbar-mark drag-A.dtp`. Record the `MarksBar.MouseMove_DragMoving` sampled time and the Grid child paths in this plan. The live path must not contain `Grid.CreateSnapPointsFromMarks`; its Grid work should scale with the number of moved Marks. Use the replacement evidence, not subjective responsiveness alone, to decide whether C is necessary.

Make final VIX-3985 description adjustments only if the implemented and validated user-facing scope differs from Milestone 1. Add a concise comment reporting the tested interactions, automated results, and whether B eliminated the measured full-rebuild bottleneck. If incremental Grid work is no longer a material child path and the interaction meets the performance goal, record that C is intentionally not planned. If it remains material, stop after documenting the evidence and create a separate, approved C ExecPlan; do not implement C in this plan.

Acceptance: the build and test suite pass, manual interactions remain correct, the profile proves the live full rebuild is gone, and VIX-3985 contains a clear outcome update.

## Concrete Steps

Work from `C:\Dev\Vixen`. At every stopping point, update the living sections of this plan with UTC timestamps and observed evidence.

1. Read the current source and tests before each edit:

       Get-Content src/Vixen.Common/Controls/TimeLineControl/Grid.cs
       Get-Content src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs
       Get-Content src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs
       Get-Content src/Vixen.Tests/Sequencer/MarksBarAutoScrollTests.cs

2. Implement and execute the focused test command after Milestones 2 and 3. First ensure the test graph builds with full MSBuild because C++/CLI dependencies are not built by `dotnet test` alone:

       msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
       dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/" --filter "FullyQualifiedName~GridMarkSnapPointTests|FullyQualifiedName~WaveformAlignmentRenderingTests|FullyQualifiedName~MarksBarAutoScrollTests"

   Expected result: zero failures. Record the exact passed count, and distinguish any unrelated environment failure from a test failure.

3. At Milestone 4, repeat the full test build and then run:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/"

   Expected result: zero failures. The current known validation baseline is 728 passing unit tests; if test inventory legitimately changes, record the new count and why.

4. Run `git diff --check` before handing off. Do not create a commit unless the user explicitly asks. For each milestone that changes repository files, invoke `.agents/skills/commit-msg/SKILL.md` and include its generated paste-ready commit message in the completion response.

## Validation and Acceptance

Automated acceptance requires focused Grid, waveform, and auto-scroll tests with zero failures, followed by the complete test project with zero failures. Tests must be deterministic; avoid elapsed-time assertions or a running message-pump dependency.

Manual acceptance requires a dense audio-backed sequence to retain correct grid lines and snap behavior during normal moves, left/right resizing, Alt glued resize, multi-select moves, and left/right edge auto-scroll. On every release and Undo, the Mark positions and grid lines must return to the correct prior state. Moving an effect afterward must snap to a moved Mark's new start and, where tail lines are enabled, end position rather than the old position.

Performance acceptance requires a controlled replacement capture. The target is to remove full all-Mark rebuilding from the live branch: `Grid.CreateSnapPointsFromMarks` must not appear under `MarksBar.MouseMove_DragMoving` through `MarksMoving`. The remaining Grid update must be attributable to only the moved Mark set. Compare the normalized drag subtree with A's 1,108.8 ms result and the original 4,886.3 ms baseline, and record the numbers rather than assuming a percentage threshold in advance.

## Idempotence and Recovery

The full rebuild remains a safe source of truth. Re-running it must clear and recreate both the snap-point dictionary and registration index together. If an incremental test exposes inconsistent state, retain the completed-operation full rebuild, correct the registration/removal logic, and rerun focused tests; do not conceal the issue with exception handling or by disabling incremental updates.

The implementation must not mutate sequence serialization, Mark interfaces, Mark Collection interfaces, event argument contracts, or undo data. It does not introduce background work, locks, timers, or a new dependency. Re-running builds and tests is safe. If a profile is not comparable because the sequence, zoom, or duration differs, document the mismatch and recapture instead of drawing a performance conclusion.

## Artifacts and Notes

The evidence motivating this plan is the controlled replacement snapshot:

    C:\Dev\Snapshots\MarkBar\vixen-marksbar-mark drag-A.dtp

Its relevant sampled call path is:

    MarksBar.MouseMove_DragMoving                         1,108.8 ms total
      TimedSequenceEditorForm.TimeLineGlobalMoving
        Grid.CreateSnapPointsFromMarks                    1,079.5 ms total / 882.1 ms own
      Waveform.WaveFormSelectedTimeLineGlobalMove             6.0 ms total

The original capture measured `MouseMove_DragMoving` at 4,886.3 ms and `Grid.CreateSnapPointsFromMarks` at 1,116.6 ms total / 904.2 ms own. This confirms A removed the waveform bottleneck and B targets a separate, largely unchanged full-rebuild cost.

## Interfaces and Dependencies

No external package or serialized data change is needed.

In `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarkSnapPointRegistration.cs`, introduce an internal registration type with an exact start detail and nullable end detail. It must never infer ownership from `TimeSpan`; it stores references returned by Grid's private insertion helper.

In `Grid.cs`, maintain a private `Dictionary<IMark, MarkSnapPointRegistration>` and private operations equivalent to the following responsibilities: add one detail without invalidation and return it; remove one registered detail by reference; rebuild all Mark-derived points and registrations in one batch; and apply a live `MarksMovingEventArgs` batch with one invalidation at most. Public `AddSnapPoint`, `RemoveSnapPoint`, and `ClearSnapPoints` retain their existing public behavior. New production members must remain private or internal unless an existing consumer requires broader visibility.

In `TimedSequenceEditorForm.cs`, `TimeLineGlobalMoved` continues calling `UpdateGridSnapTimes()` after it records undo information. The live `TimeLineGlobalMoving` handler and subscription are removed. `TimeLineGlobalEventManager` remains unchanged.

Plan revision note (2026-08-15 21:02Z): Corrected the nullable flow in the new Grid insertion helper and enabled nullable annotations in the new registration file. The full `Vixen_Tests` build target and the focused sequencer test run both passed; no nullable warnings remain in the touched files. Milestone 3 remains next.

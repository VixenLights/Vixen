# VIX-3985: Coalesce Mark-derived Grid repaint requests during drag

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document according to `docs/plans/PLANS.md` (the repository guidance is `.agents/PLANS.md`). This is a separate, approved-before-implementation phase of VIX-3985. It incorporates the measured outcome of the completed Remediation A and B implementation work so that a new contributor can carry out C without needing their plans or profile files.

## Purpose / Big Picture

Dragging or resizing a Mark in a dense sequence currently makes the Grid repaint its complete timeline for every pointer update. The Grid alignment line can therefore look current while the Mark bar, ruler, and waveform wait for time on the same user-interface thread and visibly lag behind it.

After this change, the Mark edit, snap information, auto-scroll, and final completed edit remain immediate. Only repeated requests to redraw the already-updated Grid snap lines are combined into at most one pending repaint per short display interval. A user should be able to drag, resize, Alt glued-resize, and multi-select Marks while the Mark bar, ruler, waveform guides, and Grid lines remain visually coordinated rather than the Grid consuming the paint queue.

## Progress

- [x] (2026-08-15 21:20Z) Captured and analyzed Remediation B performance and timeline profiles after the user confirmed retained behavior but continued visible lag.
- [x] (2026-08-15 21:20Z) Created this separate Remediation C ExecPlan from the measured repaint bottleneck; no C code has been implemented.
- [x] (2026-08-16 15:12Z) Updated VIX-3985 with the user-facing Remediation C scope, acceptance criteria, and validation approach.
- [x] (2026-08-16 15:15Z) Added the deterministic immediate-state and invalidation-observation test foundation; the focused build passed and the new coalescing contract is intentionally red until Milestone 3 adds the scheduler.
- [x] (2026-08-16 15:19Z) Implemented Grid-owned repaint coalescing with a 16 ms UI-thread timer, immediate snap-state maintenance, completion/rebuild/scale/disposal cleanup, and deterministic tests; the focused suite passed 14 tests.
- [x] (2026-08-16 15:46Z) Completed full validation: the user reports a successful full build, all 742 unit tests passing, confirmed functional behavior, and much improved performance.
- [x] (2026-08-16 15:46Z) Confirmed that VIX-3985's approved user-facing scope remains accurate and added the final concise validation comment.
- [x] (2026-08-16 15:50Z) Corrected the post-validation hidden-Mark-lines regression: alignment activity now uses the existing coalesced Grid repaint request, with focused deterministic coverage.
- [ ] (2026-08-16 15:50Z) Validate the regression test after the running Vixen application releases the shared Release output files; the build is currently blocked by `Vixen.Application` PID 39776 locks.

## Surprises & Discoveries

- Observation: Remediation A removed the waveform as the material source of drag delay, and Remediation B removed the previous all-Mark snap-point rebuild from the live drag path.
  Evidence: The user reports retained functionality in B. The B timeline capture shows `MarksBar.MouseMove_DragMoving` at 316.3 ms, compared with 1,108.8 ms in the A replacement capture; the B live Grid update is 39.9 ms.

- Observation: The remaining delay is complete Grid painting, not Mark-model or snap-point maintenance.
  Evidence: `C:\Dev\Snapshots\MarkBar\vixen-marksbar-mark drag-B-timeline.dtp` spans 11,065.609 ms. `Grid.OnPaint` totals 2,326.7 ms, and `_drawSnapPoints` totals 1,988.4 ms, including 1,836.4 ms in `GdipDrawLine`.

- Observation: The Grid is visibly smooth because it is invalidated directly during every live update, while other controls wait for their own paint messages on the same UI thread.
  Evidence: The same B timeline capture attributes native invalidation time to the MarksBar and Ruler live handlers, and only a narrow waveform invalidation, while complete Grid paint is the dominant running-thread work.

- Observation: Grid snap-point data is already incremental and must not be delayed along with painting.
  Evidence: `Grid.TimeLineGlobalEventManager_MarksMoving` removes and registers only the distinct changed Marks; `StaticSnapPoints` remains the source used by snapping and by `_drawSnapPoints`.

- Observation: A completed Mark operation still performs the existing full snap-point rebuild as a correctness backstop.
  Evidence: `TimedSequenceEditorForm.TimeLineGlobalMoved` calls `UpdateGridSnapTimes()`, which calls `Grid.CreateSnapPointsFromMarks()` after `MarksMoved`; that is not on the pointer-frequency path.

- Observation: The existing WinForms test harness can deterministically observe direct Grid invalidation without waiting for a paint or timer.
  Evidence: After `grid.CreateControl()`, `Control.Invalidated` recorded three events for three `MarksMoving` updates. The new focused run built successfully and reported 9 passing tests plus the one expected red coalescing contract.

- Observation: A time-per-pixel change performs established immediate Grid redraw work independently of the pending repaint request.
  Evidence: The focused test observed three invalidation notifications during scale change. Clearing the pending request before that path and then forcing its deterministic tick produced no additional notification.

- Observation: The C replacement profile confirms that the UI thread is no longer occupied by Grid redraw work for most of the drag capture.
  Evidence: `vixen-marksbar-mark drag-C-timeline.dtp` lasts 8,121.618 ms with the main thread running for 2,913 ms and 6.988 ms of GC. B's corresponding timeline lasts 11,065.609 ms with the main thread running for 6,833 ms. C has sustained CPU only in the active drag regions rather than throughout the capture.

- Observation: Alignment activity changes the yellow Grid alignment-line positions independently of Mark-derived snap points.
  Evidence: `TimeLineAlignmentHandler` updated `MarkAlignmentPoints` without invalidating the Grid. With Show Mark Lines disabled, `MarksMoving` does not alter snap points, so it no longer supplied the incidental repaint that previously displayed the latest alignment lines.

## Decision Log

- Decision: Limit C to Grid presentation invalidation caused by live Mark movement.
  Rationale: The profile identifies repeated full Grid painting as the remaining material cost. Coalescing Mark movement, snapping, auto-scroll, waveform state, Mark bar state, ruler state, or final completion would add latency to the edit rather than address the measured cause.
  Date/Author: 2026-08-15 / Codex.

- Decision: Update the Grid snap-point registry immediately for every `MarksMoving` event, then request a coalesced repaint.
  Rationale: The Grid must retain the current snapping information and correct final state even when painting is intentionally deferred. This separates correctness-critical state from derived presentation.
  Date/Author: 2026-08-15 / Codex.

- Decision: Use a Grid-owned `System.Windows.Forms.Timer` with a 16 ms interval and one pending flag.
  Rationale: A WinForms timer raises its callback on the Grid's existing UI thread, needs no cross-thread locking, and provides a simple approximately-60 Hz upper bound for repeated invalidations. It also fits the existing WinForms Grid and can be stopped and disposed with it.
  Date/Author: 2026-08-15 / Codex.

- Decision: Keep ordinary non-drag Grid invalidations immediate.
  Rationale: Configuration changes, explicit public snap-point operations, initial/full rebuilds, scale changes, and completed-operation recovery should retain their established prompt redraw behavior. Only the high-frequency live Mark-update handler will request deferred presentation.
  Date/Author: 2026-08-15 / Codex.

- Decision: Flush a pending coalesced repaint synchronously when a Mark operation completes, before a full rebuild path, and during disposal.
  Rationale: No stale timer request may survive the final edit or target a disposed control. The completed operation must leave the visual state eligible for immediate painting without relying on a later timer tick.
  Date/Author: 2026-08-15 / Codex.

- Decision: Describe the current C phase in VIX-3985 as limiting repeated display work while keeping each edit and snap decision immediate.
  Rationale: Reviewers and users need a clear statement of the visual outcome and preserved editing behavior, without coupling the issue to the implementation mechanism.
  Date/Author: 2026-08-16 / Codex.

- Decision: Use the real `Control.Invalidated` event in the Grid tests rather than introduce a test-only production abstraction before the scheduler exists.
  Rationale: Creating the control makes direct invalidation observable deterministically. The red test now accurately captures the behavior Milestone 3 must replace, while the existing registration diagnostics continue to prove immediate snap-state updates.
  Date/Author: 2026-08-16 / Codex.

- Decision: Retain immediate invalidation for the established time-per-pixel path while canceling any live-drag pending request first.
  Rationale: Zooming already refreshes Grid through the base timeline control. C must prevent a later duplicate timer repaint without changing the existing scale-change paint behavior.
  Date/Author: 2026-08-16 / Codex.

- Decision: Close Remediation C without a further redraw remediation.
  Rationale: The full build and all 742 unit tests pass, functional checks are confirmed, the user reports much improved drag responsiveness, and C's replacement profile shows substantially reduced UI-thread occupancy.
  Date/Author: 2026-08-16 / Codex.

- Decision: Route alignment-activity changes through C's existing coalesced Grid repaint request.
  Rationale: Alignment lines are derived presentation, like live Mark snap lines. Scheduling the repaint independently of whether a Mark collection draws persistent lines preserves the 16 ms paint bound while ensuring the newest alignment position is rendered and cleared during every Mark drag.
  Date/Author: 2026-08-16 / Codex.

## Outcomes & Retrospective

Milestone 3 outcome: Grid now updates Mark-derived snap state synchronously but records only one pending repaint request during a live drag. A 16 ms WinForms timer consumes that request on the UI thread and invalidates the Grid once. Completed edits, Mark snap rebuilds, scale changes, suppression, and disposal clear the pending request safely. The focused `GridMarkSnapPointTests` suite passed 14 tests after a full `Vixen_Tests` build. Full-suite, manual, and replacement-profile validation remain pending. Remediation B's focused tests passed, but its full regression validation is included in C's final validation so VIX-3985 closes only with the complete behavior covered.

Replacement-profile update: the user reports much improved observed dragging. C's supplied timeline profile reduces main-thread running time from B's 6.833 seconds to 2.913 seconds, and no material GC pressure is present. This supports the presentation-coalescing design. The subsequent full validation completed successfully.

Final outcome: the user reports the full Vixen build succeeds and all 742 unit tests pass. Functional checks are confirmed and the C drag behavior is much improved. A later hidden-Mark-lines regression was corrected by scheduling an alignment-activity repaint through the existing coalescer; focused test validation remains pending until the running application releases its output locks. With that validation complete, no additional redraw remediation is planned.

## Context and Orientation

The Timed Sequence Editor is a WinForms timeline in Vixen. A Mark is a timed annotation. Its start and, optionally, end position produce Grid snap points: objects that both support snapping and draw the colored vertical alignment lines in the timeline. The Mark bar, ruler, waveform, and Grid are separate controls on one Windows UI thread. An invalidation asks Windows to paint a control later; it does not itself draw, but repeated invalidations can cause expensive full paints that prevent other controls from painting promptly.

`src/Vixen.Common/Controls/TimeLineControl/Grid.cs` owns `StaticSnapPoints`, the Mark-to-snap-point registration map, Grid rendering, and Grid invalidation. Its live `TimeLineGlobalEventManager_MarksMoving` handler removes and registers only the affected distinct Marks, then requests a coalesced repaint. Its `TimeLineAlignmentHandler` stores the transient yellow alignment positions used while a Mark drag is active and must request that same repaint even when the moving collection has persistent Mark lines disabled. `Grid.OnPaint` calls `_drawSnapPoints`, which draws every visible snap line as part of a complete Grid paint.

`src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs` raises `MarksMoving` repeatedly as the pointer changes and `MarksMoved` once when the edit completes. It keeps both events synchronous. `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` retains `TimeLineGlobalMoved`, which calls `UpdateGridSnapTimes()` after `MarksMoved` to rebuild snap points as a final consistency backstop. `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`, the ruler, and `Waveform.cs` independently respond to movement; they are explicitly outside this remediation.

The existing tests live in `src/Vixen.Tests/Sequencer/`. `GridMarkSnapPointTests.cs` verifies the B incremental registration behavior, and `MarksBarAutoScrollTests.cs` covers replayed movement during edge auto-scroll. New C tests must use the same test collection and WinForms test setup so timer lifecycle and event delivery stay deterministic.

## Plan of Work

### Milestone 1: Record the user-visible C scope in VIX-3985

Before changing code, update VIX-3985's description in user-facing language. State that this phase makes the timeline indicators repaint together during dense Mark drags by reducing repeated redraw work, while retaining Mark movement, resizing, Alt glued resize, multi-selection, snapping, auto-scroll, undo, and the final completed edit. State that the work is verified with focused tests, the complete test suite, manual sequencer interaction, and a comparable profile. Do not mention classes, timers, internal events, source paths, or profiling method names in Jira.

Update this plan's Progress and Decision Log with the UTC timestamp and any wording changes. The description must make clear that the edit itself remains immediate; only redundant display work is limited.

Acceptance: a VIX-3985 reader can understand the expected responsiveness improvement, preserved behavior, and validation approach without repository knowledge.

### Milestone 2: Add deterministic coverage for deferred presentation without deferred state

Read `Grid.cs`, `TimeLineGlobalEventManager.cs`, the existing Grid Mark tests, and auto-scroll tests before editing. In `src/Vixen.Tests/Sequencer/GridMarkSnapPointTests.cs`, add focused tests that raise `MarksMoving` repeatedly without waiting for elapsed wall-clock time. Each event must immediately replace the moving Mark's registered start/end `SnapDetails` and preserve any stationary Mark at a shared time. The test must also prove that repeated live updates produce one pending presentation request rather than one immediate Grid invalidation per event.

Make the production coalescing state observable to tests only through a minimal `internal` diagnostic or a narrowly scoped internal test hook. It may expose whether one deferred repaint is pending and may execute the timer-tick logic deterministically, but it must not expose a new public UI API or allow tests to mutate production state. Use the project’s existing `InternalsVisibleTo` relationship. Do not write timing-sensitive tests that sleep for 16 ms.

Add tests for these boundaries:

- A deterministic tick consumes the one pending request, stops the timer, and permits a later movement batch to queue one new request.
- `MarksMoved` flushes any pending request synchronously before the editor's existing final full rebuild can leave the timeline idle.
- Calling `CreateSnapPointsFromMarks()` or a time-per-pixel update with a pending request cancels or consumes that request without a later duplicate timer paint.
- Disposing Grid stops and disposes the timer, removes both Mark event subscriptions, and makes a later event or test tick harmless.
- `SuppressInvalidate` continues to suppress both immediate and scheduled Grid invalidation; it must not leave a running timer or a permanently pending flag.

If WinForms `Invalidate` cannot be observed safely in the current test harness, factor only the request/consume state machine into a private implementation with a narrowly scoped internal observation seam. Do not replace the actual WinForms timer with a production abstraction merely to test it.

Acceptance: tests demonstrate that snap data changes immediately, repeated pointer events collapse to one presentation request, normal completion flushes it, and no callback survives disposal or suppression.

### Milestone 3: Implement Grid-owned repaint coalescing

In `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`, add a private `System.Windows.Forms.Timer` owned by each Grid. Configure its interval to 16 milliseconds, subscribe one private tick handler in the constructor, and do not enable it until the first live update requests a paint. Add private fields representing a pending snap-point repaint and disposal-safe timer ownership. Use only the UI thread that already delivers Grid and Mark events; do not use tasks, thread-pool timers, `BeginInvoke`, locks, or cross-thread state.

Keep `TimeLineGlobalEventManager_MarksMoving`'s current incremental remove/register loop unchanged through its state mutation. Replace only its final direct `InvalidateSnapPoints()` call with a private request method. That method must return without scheduling when `SuppressInvalidate` is true. Otherwise, if no repaint is pending, set the pending flag and start the timer. If one is already pending, do nothing: the future paint will render the newest `StaticSnapPoints` state.

The timer tick must first stop its timer and clear the pending flag, then call the existing immediate `InvalidateSnapPoints()` once unless Grid is disposing/disposed or invalidation is suppressed. Clearing the flag before invalidating allows a new live update arriving during later message processing to schedule the next bounded repaint. Keep `InvalidateSnapPoints()` as the immediate primitive used by public snap operations and existing configuration/full-rebuild paths; it must not schedule a timer itself.

Subscribe Grid to `MarksMoved` in the constructor and unsubscribe in `Dispose(bool)`. Its private completed-operation handler must synchronously stop the timer, clear a pending request, and invoke the immediate invalidation once when a request was pending and invalidation is allowed. The handler must not rebuild snap points and must not replace the editor form's final rebuild; it only ensures C-owned presentation has a deterministic endpoint. If event subscription order means the editor's final rebuild can execute first, ensure the full-rebuild path itself also clears/stops a pending request before it performs its established immediate invalidation. The resulting behavior must be correct regardless of subscriber order.

Before `CreateSnapPointsFromMarks()` clears/repopulates data, and before any other current path that immediately calls `InvalidateSnapPoints()` after reconfiguring Mark-derived snap data, cancel the pending timer request. The path then performs exactly its existing one immediate invalidation. Do the same in `OnTimePerPixelChanged` when recalculation requires an immediate redraw. This avoids a stale later timer tick causing an unnecessary second full Grid paint.

In `Dispose(bool)`, stop the timer, unsubscribe its tick handler, dispose it, clear pending state, and unsubscribe both `MarksMoving` and `MarksMoved` before `base.Dispose(disposing)`. Follow existing disposal style and ensure partial construction/disposal cannot dereference a null timer. Do not change Mark bar, ruler, waveform, event-manager ordering, auto-scroll replay, `TimedSequenceEditorForm`, or the B registration data structure in this milestone.

Acceptance: a high-frequency drag leaves current snap data available immediately but permits at most one outstanding Grid snap-line repaint. The final event, a full rebuild, and disposal leave no deferred repaint outstanding.

### Milestone 4: Verify visual behavior, profile outcome, and update VIX-3985

Run the focused sequencer tests after Milestones 2 and 3, then build and run the full Vixen test suite. From `C:\Dev\Vixen`, use full MSBuild first because the test graph includes C++/CLI dependencies, then run already-built tests:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/"

Launch Vixen with a dense sequence containing audio, visible Mark grid lines, tail lines, and a viewport narrower than the full timeline. Continuously drag a Mark at ordinary speed and quickly, resize both edges, Alt glued-resize, move a multi-selection in one and several collections, and auto-scroll in both directions. Release each operation and undo it. Confirm that snapping remains immediate, no old lines remain, final positions remain correct, and the Grid, Mark bar, ruler, and waveform guides no longer build up a visible delay relative to one another. Also change zoom/scale and close the editor during or immediately after a drag; no exception, delayed repaint, or stale line may appear.

Capture a comparable approximately 10-second dotTrace performance snapshot and timeline snapshot using the same sequence, zoom, visible range, and pointer movement used for `C:\Dev\Snapshots\MarkBar\vixen-marksbar-mark drag-B.dtp` and `C:\Dev\Snapshots\MarkBar\vixen-marksbar-mark drag-B-timeline.dtp`. Analyze them with the repository’s `dottrace-analyze` skill. Record total capture duration, `MarksBar.MouseMove_DragMoving`, `Grid.OnPaint`, `_drawSnapPoints`, and `GdipDrawLine` totals in this plan. The expected result is fewer Grid paint invocations and less Grid paint occupancy during the drag, giving the Mark bar, ruler, and waveform opportunities to repaint. Do not claim success solely because the live Grid handler is fast; confirm the observed visual coordination and the replacement profile.

Make any final VIX-3985 description change only if the delivered user-facing scope differs from Milestone 1. Then add a concise user-facing Jira comment naming the automated result, interactions checked, and observed responsiveness result. If the full test result is not available, state that plainly and leave the phase open. Update all living-plan sections, including outcomes and measured before/after values.

Acceptance: focused tests and the complete suite pass; manual editing behavior is retained; replacement profiling shows the complete Grid paint path no longer monopolizes the drag; and VIX-3985 communicates the validated result in user terms.

## Concrete Steps

Work from `C:\Dev\Vixen`. Update this document after each milestone with UTC timestamps, exact focused-test counts, full-suite counts, manual observations, and profile values.

1. Read the current implementation before each edit:

    Get-Content src/Vixen.Common/Controls/TimeLineControl/Grid.cs
    Get-Content src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs
    Get-Content src/Vixen.Tests/Sequencer/GridMarkSnapPointTests.cs
    Get-Content src/Vixen.Tests/Sequencer/MarksBarAutoScrollTests.cs
    Get-Content src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs

2. Update VIX-3985 during Milestone 1 using the user-facing wording described above. Record the change in the plan; do not add technical timer details to Jira.

3. After Milestones 2 and 3, run the focused test class through the already-built test project, or use the project’s test filter syntax appropriate to the current test runner. Expected result: all `GridMarkSnapPointTests` and related auto-scroll tests pass with zero failures. Record the exact count rather than assuming the current count.

4. At Milestone 4, run the full commands shown above. Expected result: MSBuild reports zero errors and `dotnet test` reports every discovered test passed. If the known suite size changes from 728, record the actual discovered/pass count.

5. Before handing off each code-changing milestone, run `git diff --check` and use the repository `commit-msg` skill to provide a formatted proposed commit message. Do not create a commit unless the user explicitly asks.

## Validation and Acceptance

The change is accepted only when all of the following are true:

- Every `MarksMoving` event immediately updates the snap entries for the moved Marks; a repeated paint request never delays snapping or the final model state.
- A dense continuous drag produces no more than one pending Grid repaint at a time, and an actual timer tick consumes that request once.
- Mouse-up, a full Mark snap rebuild, scale change, suppression, and disposal do not leave a timer callback or redundant delayed repaint outstanding.
- Existing behavior remains correct for moves, both resize edges, Alt glued resize, multi-selection, shared-time lines, hidden/tail-line settings, auto-scroll replay, release, and undo.
- Focused tests, the complete build/test sequence, and manual checks pass.
- A comparable replacement profile demonstrates materially reduced Grid paint pressure and the user can observe the Mark bar, ruler, waveform guides, and Grid lines remaining coordinated during drag.

## Idempotence and Recovery

The Jira description update can be safely repeated by replacing the description with the current approved user-facing scope. Test and build commands are read-only except for normal build outputs and can be rerun after a failure. Timer state is per Grid instance: if a test or manual run fails, close the editor or application so `Dispose(bool)` stops it, then rerun from a new Grid instance. Do not delete snapshots or build artifacts to retry validation. If profiling shows that a 16 ms interval still leaves the UI paint-bound, record the evidence and stop for a new approved remediation plan rather than widening C to change drawing algorithms or throttle other controls.

## Artifacts and Notes

The evidence that authorizes C is:

    B timeline duration: 11,065.609 ms
    Grid.OnPaint: 2,326.7 ms
    Grid._drawSnapPoints: 1,988.4 ms
    GdipDrawLine below _drawSnapPoints: 1,836.4 ms
    Live Grid Mark update: 39.9 ms

The C replacement evidence is:

    C timeline duration: 8,121.618 ms
    C main-thread running time: 2,913 ms
    C garbage-collection time: 6.988 ms
    User observation: Much improved dragging

The intended live sequence after implementation is:

    pointer move
      -> MarksMoving
      -> Grid updates affected snap entries immediately
      -> Grid records one pending repaint (or leaves the existing request pending)
      -> timer tick invalidates Grid once using newest entries
      -> Windows paints Grid when its message queue permits

The intended completion sequence is:

    pointer release
      -> MarksMoved
      -> Grid flushes any C-owned pending repaint
      -> editor retains its existing final snap-point rebuild and immediate redraw

## Interfaces and Dependencies

Use the existing `System.Windows.Forms.Timer` supplied by the WinForms project and create no new package dependency. Keep all C implementation members private except a narrowly scoped `internal` diagnostic/test hook required for deterministic tests. No public or protected API is added or changed, so no XML documentation update is expected.

In `Grid.cs`, the implementation must provide private operations with responsibilities equivalent to these names; exact private names may vary only if the resulting code is clearer:

    private void RequestSnapPointInvalidate();
    private void FlushPendingSnapPointInvalidate();
    private void SnapPointInvalidateTimer_Tick(object? sender, EventArgs e);
    private void TimeLineGlobalEventManager_MarksMoved(object sender, MarksMovedEventArgs e);

`RequestSnapPointInvalidate` is called after live Mark snap data changes and after a transient alignment-line position changes. `FlushPendingSnapPointInvalidate` stops the timer, clears state, and performs one immediate invalidation when warranted. The tick handler uses the same flush semantics. The completed-operation handler uses the flush operation and performs no rebuild. `Dispose(bool)` must detach both global event handlers and the timer callback before disposing the timer.

Revision note (2026-08-15): Created after the B replacement profiles demonstrated that incremental Mark snap maintenance succeeded but complete Grid painting remained the UI-thread bottleneck. The plan confines C to presentation coalescing and explicitly preserves immediate edit state.

Revision note (2026-08-16): Completed Milestone 1 by replacing VIX-3985's prior B-only description with user-facing C scope, preserved behaviors, acceptance criteria, and validation approach. No code changed.

Revision note (2026-08-16): Completed Milestone 2's test foundation using the real WinForms invalidation event. The focused build completed; the coalescing contract intentionally fails with the current three direct invalidations and will become green in Milestone 3.

Revision note (2026-08-16): Completed Milestone 3. Grid now coalesces only live Mark-derived repaint requests and the red contract is green. Added deterministic checks for a timer tick, completed edit, rebuild, scale change, suppression, and disposal; the focused suite passed 14 tests.

Revision note (2026-08-16): Recorded the user-supplied C replacement profiles. They confirm materially lower UI-thread occupancy and the reported visual improvement; final validation remains open for the complete suite and interaction checklist.

Revision note (2026-08-16): Completed final validation with the user-reported successful full build, 742 passing unit tests, confirmed functionality, and much improved performance. VIX-3985 scope was already accurate, so only the final validation comment was added.

Revision note (2026-08-16): A post-validation test found that alignment activity had no repaint trigger when Show Mark Lines was disabled, because no snap point changed. The alignment handler now uses C's existing coalesced repaint request and a deterministic hidden-lines regression test covers the behavior. The focused build is deferred until the user closes the running Vixen application that holds shared Release output locks.

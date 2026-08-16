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
- [ ] Add deterministic tests for immediate snap-state updates, coalesced repaint requests, final flushing, and disposal safety.
- [ ] Implement Grid-owned repaint coalescing without changing Mark movement semantics.
- [ ] Run focused tests, the complete Vixen test build and suite, manual sequencer checks, and a comparable replacement profile.
- [ ] Make final VIX-3985 adjustments if needed and add a concise user-facing validation comment.

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

## Outcomes & Retrospective

Planning outcome: Remediation C is justified by replacement profile evidence and is intentionally narrower than a general scheduling rewrite. The planned change does not make Mark edits asynchronous and does not alter their event ordering. Implementation and validation remain pending. Remediation B's focused tests passed, but its full regression validation remains pending and is included in C's final validation so VIX-3985 closes only with the complete behavior covered.

## Context and Orientation

The Timed Sequence Editor is a WinForms timeline in Vixen. A Mark is a timed annotation. Its start and, optionally, end position produce Grid snap points: objects that both support snapping and draw the colored vertical alignment lines in the timeline. The Mark bar, ruler, waveform, and Grid are separate controls on one Windows UI thread. An invalidation asks Windows to paint a control later; it does not itself draw, but repeated invalidations can cause expensive full paints that prevent other controls from painting promptly.

`src/Vixen.Common/Controls/TimeLineControl/Grid.cs` owns `StaticSnapPoints`, the Mark-to-snap-point registration map, Grid rendering, and Grid invalidation. Its live `TimeLineGlobalEventManager_MarksMoving` handler already removes and registers only the affected distinct Marks. It currently calls `InvalidateSnapPoints()`, which calls `Invalidate()` on the entire Grid. `Grid.OnPaint` calls `_drawSnapPoints`, which draws every visible snap line as part of a complete Grid paint.

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

`RequestSnapPointInvalidate` is called only after live Mark snap data changes. `FlushPendingSnapPointInvalidate` stops the timer, clears state, and performs one immediate invalidation when warranted. The tick handler uses the same flush semantics. The completed-operation handler uses the flush operation and performs no rebuild. `Dispose(bool)` must detach both global event handlers and the timer callback before disposing the timer.

Revision note (2026-08-15): Created after the B replacement profiles demonstrated that incremental Mark snap maintenance succeeded but complete Grid painting remained the UI-thread bottleneck. The plan confines C to presentation coalescing and explicitly preserves immediate edit state.

Revision note (2026-08-16): Completed Milestone 1 by replacing VIX-3985's prior B-only description with user-facing C scope, preserved behaviors, acceptance criteria, and validation approach. No code changed.

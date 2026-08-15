# Technical Specification: Marks Bar Drag Performance Remediation

**Target File Path:** `docs/reviews/marks-bar-drag-performance-remediation-plan.md`  
**Status:** Ready for Review  
**Implementation Target:** Terra model

---

## 1. Refined Requirements

### Functional Overview

Dragging or resizing a Mark in the Timed Sequence Editor must remain visually responsive even when the sequence contains many waveform samples and many Marks. The existing interaction semantics must remain unchanged: selected Marks move together, resize constraints remain enforced, Alt glued-resize continues to work, horizontal auto-scroll continues replaying the latest mouse position, grid lines and waveform alignment guides follow the active operation, and `MarksMovedEventArgs` still produces one undoable completed operation.

The implementation should remediate the three issues identified in `C:\Dev\Snapshots\MarkBar\vixen-marksbar-mark drag.dtp`:

- `MarksBar.MouseMove_DragMoving` accumulated 4,886.3 ms of sampled work.
- The synchronous `AlignmentActivity` path consumed 3,752.2 ms, or 76.8% of the drag branch. `Waveform.OnPaint` alone consumed 3,363.7 ms.
- The synchronous `MarksMoving` path consumed 1,134.1 ms, or 23.2% of the drag branch. `Grid.CreateSnapPointsFromMarks` consumed 1,116.6 ms, including 904.2 ms of own time.
- The two event paths account for effectively all sampled work below `MouseMove_DragMoving`; the Mark position arithmetic itself is not the bottleneck.

### Detailed Requirements List

#### Remediation A: stop repainting the complete waveform synchronously

- Keep `TimeLineGlobalEventManager.AlignmentActivity` and its current callers intact.
- In `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`, replace the synchronous `Refresh()` in `WaveFormSelectedTimeLineGlobalMove` with invalidation-based painting.
- When alignment times change, invalidate narrow client-coordinate rectangles covering both the previous and new alignment guide positions. Invalidating the previous positions is required to erase old guide lines.
- Make `Waveform.OnPaint` honor `PaintEventArgs.ClipRectangle` when iterating waveform samples. A narrow invalidation must redraw only the affected sample columns, not loop across the complete visible waveform.
- Preserve full-control repaint behavior for zoom, horizontal scroll, audio changes, sample regeneration, resizing, theme-related invalidation, and ordinary WinForms paint events.
- Treat an inactive alignment event or a null `AlignmentEventArgs.Times` value as an empty collection. It must clear the previous alignment guides without throwing.
- Do not introduce a background painting thread. `Graphics` and WinForms control state remain UI-thread-owned.

#### Remediation B: update only snap points affected by moving Marks

- Keep `TimeLineGlobalEventManager.MarksMoving` as the live-operation event and `MarksMoved` as the completed-operation event.
- Move ownership of live snap-point maintenance into `Grid`, which already owns `StaticSnapPoints` and subscribes to other timeline-global events.
- Subscribe `Grid` to `MarksMoving` during construction and unsubscribe during `Dispose(bool)`.
- For a `MarksMovingEventArgs`, remove and recreate snap-point entries only for the Marks in `e.Marks`. Preserve live grid-line movement during drag and resize.
- Maintain a private registration from each contributing `IMark` to the exact `SnapDetails` objects created for its start and optional end time. Removal must use object identity so two Marks at the same time do not remove one another's snap details.
- Continue using a full rebuild when Mark Collections are initially configured or structurally changed, or when collection/decorator settings change. These are infrequent correctness paths rather than pointer-frequency paths.
- Remove the unconditional `mc.EnsureOrder()` from `CreateSnapPointsFromMarks`. `StaticSnapPoints` is a `SortedDictionary`, and `TimeLineGlobalEventManager.OnMarksMoving` already orders each affected parent collection before raising `MarksMoving`.
- Batch invalidation: clearing, removing, or adding a set of snap points must cause at most one `Grid.Invalidate()` after the batch. Public single-point methods may retain their existing one-call/one-invalidation behavior.
- Preserve the final full `UpdateGridSnapTimes()` call from `TimedSequenceEditorForm.TimeLineGlobalMoved` during the first implementation pass as a mouse-up correctness backstop. It may be removed later only if tests prove that every add, delete, move, resize, collection-style change, and time-scale change keeps the incremental index consistent.
- Remove the Timed Sequence Editor's `MarksMoving` subscription and `TimeLineGlobalMoving` handler after `Grid` owns the live incremental update. Leaving both active would perform duplicate work.
- `RecalculateAllStaticSnapPoints`, which runs when `TimePerPixel` changes, must preserve the `SnapDetails` object identities stored in the Mark registration map. Recalculate `SnapStart` and `SnapEnd` in place, or perform an explicit full rebuild that also recreates every registration.

#### Remediation C: coalesce presentation work only if re-profiling still requires it

- Do not throttle Mark model updates, boundary checks, auto-scroll replay, or the final `MarksMoved` event. These define the user's actual drag result and undo behavior.
- First implement Remediations A and B and repeat the controlled performance scenario.
- If `MouseMove_DragMoving` still exceeds the performance boundary in Section 3, coalesce only derived presentation work. For Grid snap points, collect changed `IMark` references in a `HashSet<IMark>` and flush the latest state no more than once every 16 ms on a `System.Windows.Forms.Timer`.
- A pending presentation update must be flushed synchronously before processing `MarksMoved`, disposal, or a full snap-point rebuild.
- Own and dispose any timer in the consumer that uses it. Do not add a process-wide timer or use a thread-pool timer for UI state.
- Record the post-Remediation-A/B profile evidence in this document before deciding whether the timer is necessary. If the target is already met, mark Remediation C as unnecessary and do not add timing complexity.

### Data Model and State Changes

No serialized sequence, Mark, Mark Collection, setting, or undo data changes are required.

In `Waveform`, initialize `_activeTimes` to an empty collection and add private or internal calculation helpers for alignment invalidation rectangles and clip-bounded sample indexes. Suggested responsibilities are:

    private void InvalidateAlignmentRegions(IEnumerable<TimeSpan> previousTimes, IEnumerable<TimeSpan> currentTimes)
    internal Rectangle GetAlignmentInvalidationRectangle(TimeSpan alignmentTime)
    internal (int Start, int EndExclusive) GetVisibleSampleRange(Rectangle clipRectangle)

Names may follow local conventions, but the calculations should be separated enough to unit test without creating a real paint message.

In `Grid`, add a private registration dictionary and a small internal sealed support type in its own file, for example `src/Vixen.Common/Controls/TimeLineControl/MarkSnapPointRegistration.cs`:

    private readonly Dictionary<IMark, MarkSnapPointRegistration> _markSnapPointRegistrations = [];

    internal sealed class MarkSnapPointRegistration
    {
        public required SnapDetails Start { get; init; }
        public SnapDetails? End { get; init; }
    }

The final code must use tabs, C# collection expressions where consistent with the project, and one type per file. No new public API is expected. If implementation requires modifying a public or protected API, read and apply `.agents/skills/csharp-docs/SKILL.md` and update its XML documentation in the same change.

## 2. Technical Architecture and Impact

### Current Execution Flow

`MarksBar.MouseMove_DragMoving` updates selected Mark start times and then synchronously raises two events:

    MouseMove_DragMoving
      -> OnMarksMoving
         -> TimedSequenceEditorForm.TimeLineGlobalMoving
            -> Grid.CreateSnapPointsFromMarks
      -> OnAlignmentActivity
         -> Waveform.WaveFormSelectedTimeLineGlobalMove
            -> Refresh
               -> Waveform.OnPaint

Because both branches complete before the mouse event returns, expensive derived rendering blocks subsequent input. Marks Bar auto-scroll also calls `HandleMouseMove` from its UI timer, so any work added to these branches is amplified during edge scrolling.

### Implementation Strategy

#### Waveform painting

In `WaveFormSelectedTimeLineGlobalMove`, materialize the previous and next alignment times once, update `_showMarkAlignment` and `_activeTimes`, then invalidate the union of narrow regions covering old and new guide positions. Do not call `Refresh`, `Update`, `Application.DoEvents`, or manually invoke `OnPaint`.

An alignment time maps to a client X coordinate as:

    x = floor(timeToPixels(alignmentTime - VisibleTimeStart))

Use a small safety margin around the one-pixel line, such as two pixels on each side, intersected with `ClientRectangle`:

    invalidRectangle = Rectangle(x - 2, 0, 5, ClientSize.Height) intersect ClientRectangle

In `OnPaint`, the existing graphics transform makes waveform sample coordinates absolute timeline pixels. Convert the client clip bounds back to timeline-pixel indexes:

    visibleStartPixel = floor(timeToPixels(VisibleTimeStart))
    start = max(0, visibleStartPixel + clip.Left - 1)
    endExclusive = min(samples.Count, visibleStartPixel + clip.Right + 1)

Also cap `endExclusive` at the media-duration pixel. The one-pixel margin prevents clipped edge artifacts. A full-control invalidation naturally yields the existing full visible range.

#### Incremental snap-point index

Refactor snap insertion into a private core method that returns the created `SnapDetails` and does not invalidate. `AddSnapPoint` can call this core method and then preserve its existing invalidation contract. The full rebuild and incremental batch methods call the core method repeatedly and invalidate once.

For each moving Mark:

1. Look up its prior `MarkSnapPointRegistration`.
2. Remove the registered start and optional end `SnapDetails` by reference from the list stored under each detail's previous `SnapTime`.
3. Remove a `StaticSnapPoints` dictionary key only when its detail list becomes empty.
4. Remove the old registration.
5. If `mark.Parent.ShowGridLines` is false, stop for that Mark.
6. Create and register the current start detail.
7. If `mark.Parent.ShowTailGridLines` is true, create and register the current end detail.
8. After all distinct moving Marks are processed, invalidate the Grid once.

Deduplicate `e.Marks` by reference before updating so glued resize or multi-selection cannot process one Mark twice. All operations remain on the WinForms UI thread; no lock or concurrent collection is required.

During a full rebuild, clear both `StaticSnapPoints` and `_markSnapPointRegistrations`, recreate the entries from all eligible collections, and invalidate once. Do not call the public invalidating `ClearSnapPoints` or `AddSnapPoint` inside the batch.

### Component Impact Matrix

| Component | Required change | Runtime effect |
|---|---|---|
| `MarksBar.cs` | Preserve existing drag calculations and event publication. No initial throttling. | Mark movement, resize, auto-scroll, and undo semantics remain stable. |
| `Waveform.cs` | Replace `Refresh` with narrow invalidation and honor the paint clip rectangle. | Alignment movement no longer forces a complete waveform repaint inside each mouse event. |
| `Grid.cs` | Own incremental Mark snap-point registration and subscribe to `MarksMoving`. | Work scales with the number of moved Marks instead of all Marks and invalidates once per batch. |
| `MarkSnapPointRegistration.cs` | Store the exact start/end `SnapDetails` associated with one Mark. | Duplicate snap times can be updated safely by object identity. |
| `TimeLineGlobalEventManager.cs` | No behavioral change expected. | Affected parent collections remain ordered before live subscribers run. |
| `TimedSequenceEditorForm.cs` | Remove the live full-rebuild handler; retain completed-operation bookkeeping and initial correctness backstop. | Prevents duplicate/full snap rebuilding during pointer movement. |
| `Vixen.Tests` | Add waveform range/invalidation and Grid incremental-index tests. | Protects visual correctness and cache consistency without timing-sensitive UI automation. |

### Constraints and Non-Goals

- Do not change Mark serialization, `IMark`, `IMarkCollection`, `MarksMovingEventArgs`, `MarksMovedEventArgs`, or undo action contracts.
- Do not remove or weaken Marks Bar auto-scroll implemented by VIX-3944.
- Do not cache the waveform as a new bitmap in the first pass. Clip-aware drawing plus deferred invalidation directly addresses the measured synchronous path with less resource-management risk. A bitmap cache is a later option only if re-profiling shows `OnPaint` remains material.
- Do not convert the legacy WinForms controls to WPF or introduce a new package.
- Do not optimize unrelated render workers, output threads, or application-idle samples visible in the snapshot.

## 3. Acceptance Criteria

### Happy Path

- **Given** a sequence with audio and visible waveform samples, **when** a user continuously drags one Mark, **then** the waveform alignment guides follow the Mark without `Waveform.Refresh()` executing in the mouse-move call path.
- **Given** a Mark Collection with grid lines enabled, **when** one Mark moves, **then** only that Mark's start and optional end snap details are removed and recreated, while unrelated `SnapDetails` objects remain registered.
- **Given** two Marks with the same start time, **when** one moves, **then** the original time retains the other Mark's snap detail and the moved Mark appears at its new time.
- **Given** a completed move or resize, **when** the mouse button is released, **then** undo restores the original Mark times and the final Grid snap points match all Mark Collections.

### Boundary and Edge Cases

- **Given** an alignment time outside the visible range, **when** alignment activity changes, **then** invalidation is clipped to `ClientRectangle` and no invalid rectangle or exception is produced.
- **Given** an inactive alignment event whose times are null, **when** it is handled, **then** old guide regions are invalidated and `_activeTimes` becomes empty.
- **Given** the waveform is partially invalidated at its left or right edge, **when** `OnPaint` executes, **then** calculated sample indexes remain within `[0, samples.Count]` and within the media-duration pixel.
- **Given** `ShowGridLines` is false, **when** a Mark moves, **then** it has no registered static snap details.
- **Given** `ShowTailGridLines` changes, **when** the existing collection/decorator property-change path triggers a full rebuild, **then** end-time registrations are added or removed correctly.
- **Given** multiple selected Marks across multiple parent collections, **when** they move or resize, **then** every distinct affected Mark is updated exactly once.
- **Given** time-per-pixel changes, **when** snap ranges are recalculated, **then** later incremental removal still finds the registered details and uses the new snap-strength window.
- **Given** Marks Bar auto-scroll replays a mouse move, **when** the viewport advances, **then** Mark movement, waveform guides, and Grid lines continue updating without recursive paint or stale snap entries.

### Fault Tolerance and Lifecycle

- **Given** a `Waveform` or `Grid` is disposed, **when** timeline-global events are raised later, **then** the disposed control receives no callback because all added subscriptions were removed.
- **Given** an incremental registration is missing because a collection was rebuilt, **when** that Mark next moves, **then** the update safely creates its current registration rather than throwing.
- **Given** Remediation C is implemented, **when** a move completes or the Grid is disposed with pending Marks, **then** pending updates are flushed or discarded deterministically and the timer is stopped and disposed.

### Performance and Regression Boundaries

Use a controlled 10-second drag capture with the same sequence, zoom, visible range, and approximate pointer motion as the baseline snapshot.

- `Waveform.WaveFormSelectedTimeLineGlobalMove` must no longer synchronously contain `Waveform.OnPaint` beneath `MarksBar.MouseMove_DragMoving`.
- `Grid.CreateSnapPointsFromMarks` must not appear beneath the live `MarksMoving` branch during continuous dragging; only the incremental update method should appear.
- Cumulative sampled time beneath `MarksBar.MouseMove_DragMoving`, normalized to a 10-second drag, must decrease by at least 70% from the 4,886.3 ms baseline.
- The waveform and Grid must show no visible stale alignment line for more than one normal UI frame after pointer motion stops.
- Focused and full tests must pass, and manual validation must show no regression in move, resize, glued resize, multi-select, auto-scroll, snapping, or undo.

## 4. Test Plan

### Automated Testing Strategy

Add `src/Vixen.Tests/Sequencer/WaveformAlignmentRenderingTests.cs` using the existing `TimelineControlTestCollection` and nearby reflection/helper style where needed.

- Verify conversion of an alignment time to a clipped invalidation rectangle at the left edge, center, right edge, and outside the viewport.
- Verify clip rectangles produce bounded start/end sample indexes.
- Verify a full-client clip covers the current visible sample range.
- Verify null/inactive alignment state is normalized to an empty set.
- If practical without message-pump timing, raise `AlignmentActivity` through the instance's `TimeLineGlobalEventManager` and assert that invalidation occurs without a synchronous paint. Keep timing assertions out of unit tests.

Add `src/Vixen.Tests/Sequencer/GridMarkSnapPointTests.cs`.

- Build a Grid with two eligible Mark Collections and assert the full initial index.
- Move one Mark and raise `MarksMoving`; assert the old start/end keys are removed when empty and new keys are present.
- Assert unrelated Mark registrations retain the same `SnapDetails` object references.
- Verify duplicate start/end times retain the non-moving Mark's detail.
- Verify `ShowGridLines == false` and `ShowTailGridLines == false` behavior.
- Verify a multi-Mark, multi-parent update changes each registration once.
- Change `TimePerPixel`, then move a Mark and verify registration removal and recreated snap windows remain correct.
- Dispose the Grid and verify subsequent events do not mutate it or throw.

Keep private production state private. Prefer small internal read-only diagnostics or deterministic calculation helpers exposed through the existing `InternalsVisibleTo` relationship over making new public APIs. If reflection is used, follow the established `MarksBarAutoScrollTests` and `WaveformLockHeightTests` pattern.

### Manual and Profiling Verification

1. Build and launch Vixen, open the Timed Sequence Editor, load a sequence with audio, many Marks, visible mark grid lines, and a timeline wider than the viewport.
2. Drag a single Mark continuously for 10 seconds while remaining inside the viewport. Confirm the waveform guides, Marks Bar, ruler, and effect-grid mark lines follow smoothly.
3. Repeat while auto-scrolling left and right.
4. Resize the left edge and right edge, then repeat an Alt glued resize.
5. Move multiple selected Marks from one and multiple collections.
6. Release and undo each operation. Confirm original times and grid lines are restored.
7. Move an effect afterward and verify it snaps to the moved Mark's new start/end positions, not the old positions.
8. Capture a new dotTrace performance snapshot of the controlled 10-second drag and evaluate the criteria in Section 3.
9. If the 70% reduction target is not met, identify the remaining child path before implementing Remediation C. Do not add a timer solely because it was listed as an option.

### Build and Test Commands

Run from `C:\Dev\Vixen`.

Build the test target with full MSBuild because the test graph includes C++/CLI projects:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Run focused tests against the built output:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/" --filter "FullyQualifiedName~WaveformAlignmentRenderingTests|FullyQualifiedName~GridMarkSnapPointTests|FullyQualifiedName~MarksBarAutoScrollTests"

Run the complete test project:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/"

Expected result: the focused tests and complete test project report zero failures. Record unrelated pre-existing failures separately and retain a passing focused run as evidence.

## 5. Implementation Milestones

### Milestone 1: Waveform invalidation and clip-aware rendering

Change `Waveform.cs`, add deterministic calculation tests, and manually confirm alignment guides erase and redraw correctly. At the end of this milestone, `AlignmentActivity` must return without forcing a synchronous `OnPaint`, and a narrow invalidation must iterate only the corresponding waveform sample columns.

### Milestone 2: Incremental Grid snap-point maintenance

Add the registration type, refactor Grid's batch/core snap-point methods, subscribe Grid to `MarksMoving`, and remove the Timed Sequence Editor's live full-rebuild handler. Add cache-consistency tests covering duplicate times, tail lines, multiple parents, time-scale changes, and disposal. At the end of this milestone, live drag work must be proportional to moved Marks rather than total Marks.

### Milestone 3: Functional and performance validation

Run focused and full tests, exercise the manual scenarios, and capture a controlled replacement profile. Record the before/after `MouseMove_DragMoving`, waveform, and snap-point values in this document's Outcomes section.

### Milestone 4: Conditional presentation coalescing

Implement the 16 ms Grid presentation timer only if Milestone 3 misses the performance boundary and the replacement call tree still attributes material time to repeated incremental snap updates. Repeat all validation after adding it. If the target is met, document that coalescing was intentionally omitted.

## 6. Implementation Tracking

### Progress

- [x] (2026-08-15 19:53Z) Analyzed the supplied dotTrace performance snapshot and correlated the drag call tree with repository source.
- [x] (2026-08-15 19:53Z) Produced this implementation-ready remediation specification.
- [ ] Implement and validate Milestone 1.
- [ ] Implement and validate Milestone 2.
- [ ] Complete the controlled re-profile and Milestone 3 acceptance checks.
- [ ] Decide and document whether Milestone 4 is required.

### Surprises and Discoveries

- Observation: The Marks Bar's own position calculations did not register as a meaningful hotspot; both expensive branches are synchronous event consumers.
  Evidence: `MouseMove_DragMoving` had 4,886.3 ms total, split exactly between 3,752.2 ms in alignment handling and 1,134.1 ms in moving-mark handling.
- Observation: Existing VIX-3944 auto-scroll intentionally replays `HandleMouseMove`, so optimizing subscribers is safer than suppressing or delaying the drag calculation itself.
  Evidence: `docs/plans/sequencer/vix-3944-marks-bar-autoscroll.md` requires the existing move, alignment, and completion event roles to remain intact.
- Observation: `CreateSnapPointsFromMarks` sorts collections even though the event manager already sorts moved parents and the destination dictionary sorts by time.
  Evidence: `TimeLineGlobalEventManager.OnMarksMoving` calls `EnsureOrder` before subscribers, while `Grid.CreateSnapPointsFromMarks` calls it again for every grid-line collection.

### Decision Log

- Decision: Preserve the existing event contracts and optimize their consumers.
  Rationale: This minimizes behavioral risk to selection, auto-scroll, alignment, snapping, and undo.
  Date/Author: 2026-08-15 / Codex.
- Decision: Use partial invalidation and clip-aware drawing before introducing a waveform bitmap cache.
  Rationale: It directly removes synchronous full repainting without adding bitmap lifecycle and scaling complexity.
  Date/Author: 2026-08-15 / Codex.
- Decision: Give Grid ownership of live incremental snap-point maintenance.
  Rationale: Grid owns the data structure and already participates in timeline-global alignment events; the editor form should not rebuild Grid internals for every pointer event.
  Date/Author: 2026-08-15 / Codex.
- Decision: Make explicit 16 ms throttling evidence-gated.
  Rationale: WinForms already coalesces `Invalidate` calls, and incremental snap updates may be cheap enough. Avoid adding latency and timer lifecycle risk without profile evidence.
  Date/Author: 2026-08-15 / Codex.

### Outcomes and Retrospective

Planning is complete; implementation and validation have not started. Update this section after every milestone with changed files, focused/full test results, manual observations, and before/after profiling values. At completion, state whether Remediation C was required and compare the normalized drag subtree against the 4,886.3 ms baseline.

## 7. Idempotence, Recovery, and Handoff Notes

Implement the milestones in order and keep the application buildable after each. Re-running full rebuilds and tests is safe. If incremental snap-point tests expose an inconsistent registration, temporarily retain the final `MarksMoved` full rebuild and fix the live registration logic; do not mask errors with broad exception handling.

If partial waveform invalidation leaves visual trails, first expand the invalidation margin and verify client-versus-timeline coordinate conversion. Revert only the narrow-region optimization while keeping `Invalidate()` instead of `Refresh()` so synchronous painting does not return.

The implementing agent must update `Progress`, `Surprises and Discoveries`, `Decision Log`, and `Outcomes and Retrospective` as work proceeds. Any milestone that changes repository files must conclude with a commit message generated through `.agents/skills/commit-msg/SKILL.md`; do not create a Git commit unless the user explicitly requests it.

Is this specification approved? Once approved, implementation can proceed using the code execution plan.

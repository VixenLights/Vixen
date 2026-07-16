# VIX-3944 Marks Bar Drag and Resize Auto-Scroll

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. It is self-contained so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users can move and resize Marks in the Timed Sequence Editor's Marks Bar, but today that operation stalls at the edge of the visible timeline. If the target time is off-screen, the user must release the mouse, manually scroll, and start again. After this change, dragging or resizing a Mark past the left or right edge of the visible timeline scrolls the timeline automatically, matching the workflow users already have when moving or resizing effects in the main timeline grid.

The visible result is in the Timed Sequence Editor. With one or more visible Mark Collections shown in the Marks Bar, a user can drag a Mark toward the right edge and keep holding the mouse button while the timeline scrolls right and the Mark continues moving. The same behavior works to the left, and while resizing either the start edge or end edge of a Mark. The operation still respects the sequence start, sequence end, minimum Mark width, multi-selected Marks, and the existing Alt glued-resize behavior.

## Progress

- [x] (2026-07-16 00:00 -05:00) Researched the current Marks Bar drag and resize code, the existing effect-grid auto-scroll implementation, and the timeline shared time/scroll APIs.
- [x] (2026-07-16 00:00 -05:00) Created this initial ExecPlan for VIX-3944 with implementation milestones, Jira update text, acceptance criteria, and validation steps.
- [x] (2026-07-16 16:09 -05:00) Milestone 1: Updated Jira issue VIX-3944 with requirements, high-level design, acceptance criteria, and test plan in planning comment `40204`.
- [ ] Milestone 2: Add horizontal auto-scroll support to `MarksBar` during Mark move and resize operations.
- [ ] Milestone 3: Add focused automated coverage for scroll calculation and behavior that can be tested without brittle mouse simulation.
- [ ] Milestone 4: Run focused and full validation, manually verify the Timed Sequence Editor workflow, and update Jira with final evidence.

## Surprises & Discoveries

- Observation: Mark move and resize operations already live in a reusable timeline control, not in the Timed Sequence Editor form itself.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` owns `OnMouseDown`, `OnMouseMove`, `MouseMove_DragMoving`, and `MouseMove_HResizing`.

- Observation: Effect move and resize auto-scroll is already implemented in the main timeline grid with a timer and last-mouse-move replay.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` has the `[Mouse Drag] Auto-Scroll` region with `m_autoScrollTimer`, `m_mouseOutside`, `m_lastMouseMove`, and `m_autoScrollTimer_Tick`.

- Observation: `MarksBar` does not use `AutoScrollPosition`; it translates mouse and paint coordinates through shared timeline time using `VisibleTimeStart`.
  Evidence: `MarksBar.TranslateLocation` offsets the mouse X coordinate by `timeToPixels(VisibleTimeStart)`, and `OnPaint` translates graphics by `-timeToPixels(VisibleTimeStart)`.

- Observation: The Atlassian tool surface available in this session exposed issue read and comment creation tools, but no direct issue-description edit tool.
  Evidence: VIX-3944 was read with `getJiraIssue`, and planning content was added with `addCommentToJiraIssue` as comment `40204`.

## Decision Log

- Decision: Scope VIX-3944 to horizontal time auto-scroll only.
  Rationale: The reported problem is dragging or resizing beyond the start or end of the visible timeline. Marks Bar rows are shallow horizontal timing lanes, and the existing effect comparison is horizontal auto-scroll while extending a timeline operation.
  Date/Author: 2026-07-16 / Codex.

- Decision: Reuse the existing effect-grid feel where practical: edge proximity starts scrolling, a timer advances the viewport repeatedly, and the last mouse move is replayed after each scroll tick.
  Rationale: Users explicitly asked for behavior like dragging or resizing effects. Reusing the interaction model reduces surprise and keeps scroll speed and trigger behavior familiar.
  Date/Author: 2026-07-16 / Codex.

- Decision: Implement scrolling in `MarksBar` by changing `VisibleTimeStart`, not by adding `AutoScrollPosition` to the Marks Bar.
  Rationale: `MarksBar` already renders and hit-tests from `VisibleTimeStart`, while `Grid` owns `AutoScrollPosition`. Introducing a second scroll model into `MarksBar` would make synchronization harder than directly using the shared timeline time state.
  Date/Author: 2026-07-16 / Codex.

- Decision: Prefer automated helper tests plus manual WinForms validation unless the existing test harness can cleanly exercise mouse/timer behavior.
  Rationale: Timer-driven WinForms mouse simulation is easy to make brittle. The important deterministic logic is scroll eligibility, delta calculation, clamping, and replay behavior; the final user workflow still needs manual verification in the actual editor.
  Date/Author: 2026-07-16 / Codex.

## Outcomes & Retrospective

This plan has been created, but implementation has not started. The current expected outcome is that VIX-3944 will update Jira before coding, add horizontal auto-scroll to Mark move and resize operations, preserve all existing Mark timing constraints, and validate with both automated tests and manual Timed Sequence Editor scenarios.

Milestone 1 is complete. VIX-3944 now has planning comment `40204` containing the requirements, high-level design, acceptance criteria, and test plan from this ExecPlan. Code implementation has not started.

## Context and Orientation

Vixen is a Windows desktop sequence editor for animated light shows. A sequence is displayed on a timeline. The visible part of that timeline has a start time, `VisibleTimeStart`, and a width expressed as a duration, `VisibleTimeSpan`. The right edge of the visible range is `VisibleTimeEnd`.

A Mark is a labeled time span stored through the `Vixen.Marks.IMark` interface. Marks are grouped into Mark Collections through `Vixen.Marks.IMarkCollection`. Users can show a Mark Collection as a row in the Marks Bar above the effect grid, then drag a Mark to move it or drag one of its edges to resize it.

The reusable timeline controls live under `src/Vixen.Common/Controls/TimeLineControl`. The Timed Sequence Editor in `src/Vixen.Modules/Editor/TimedSequenceEditor` hosts these controls and connects them to sequence data and undo history.

The key files for this work are:

- `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`: owns Mark Bar rendering, Mark hit testing, selection, drag move, drag resize, paste, delete, and Mark move events.
- `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs`: owns effect-grid mouse operations and the existing effect auto-scroll implementation to use as the behavioral model.
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`: owns effect-grid scroll synchronization and shows how `VisibleTimeStart` is kept in sync with the grid's horizontal scrollbar.
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControlBase.cs`: provides shared time properties and helpers such as `PixelsToTime`, `timeToPixels`, `VisibleTimeStart`, `VisibleTimeSpan`, `VisibleTimeEnd`, and `TotalTime`.
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMoveResizeInfo.cs`: stores original Mark start and duration values during a move or resize operation.
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs`: confirms that completed Mark moves and resizes already participate in undo through `MarksMovedEventArgs`.
- `src/Vixen.Tests/Sequencer`: contains existing timeline tests that can be used as examples if a focused non-UI test is added.

In `MarksBar.cs`, `OnMouseDown` finds the Mark under the cursor using `MarkAt`, selects it through `MarksSelectionManager`, and starts either a waiting move state or a horizontal resize state. `OnMouseMove` calls `HandleMouseMove`, which dispatches by `_dragState`. During a move, `MouseMove_DragMoving` computes a time delta from the current mouse X position and `_moveResizeStartLocation`, clamps the result so the earliest selected Mark does not move before zero and the latest selected Mark does not move past `TimeInfo.TotalTime`, then updates each selected Mark's `StartTime`. During resize, `MouseMove_HResizing` computes a time delta, clamps to sequence boundaries, enforces `MinMarkWidthPx`, updates `StartTime` or `Duration`, and handles the existing Alt glued-resize mode for adjacent Marks. On mouse up, `FinishedResizeMoveMarks` raises `MarksMovedEventArgs`, which is what the editor uses for undo and final bookkeeping.

Effect-grid auto-scroll in `Grid_Mouse.cs` starts when the mouse is near or outside the edge during `Moving`, `Selecting`, `Drawing`, or `HResizing`. The grid stores how far the pointer is outside the viewport in `m_mouseOutside`, starts `m_autoScrollTimer`, and on each tick updates `AutoScrollPosition`, calls `HandleHorizontalScroll`, then calls `HandleMouseMove(m_lastMouseMove)` so the current drag operation continues using the newly scrolled coordinate space. `MarksBar` needs the same idea, but its horizontal viewport should be changed by assigning `VisibleTimeStart` directly because its mouse translation and painting already depend on that value.

## Plan of Work

Milestone 1 updates Jira before code changes. Use the project `jira` skill by reading `.agents/skills/jira/SKILL.md` first. Read the current VIX-3944 issue, then update its description with the requirements, acceptance criteria, and test plan from this ExecPlan. Do not transition the issue unless the team's workflow explicitly requires it. Record the Jira update result in `Progress` and `Artifacts and Notes`.

Milestone 2 adds auto-scroll behavior to `MarksBar`. In `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`, add private fields similar in purpose to the grid's auto-scroll fields: a `Timer`, the last `MouseEventArgs`, the current horizontal mouse distance outside the Mark Bar viewport, an edge margin, and a scale factor. Use names consistent with the file's existing underscore-prefixed fields, for example `_autoScrollTimer`, `_lastMouseMove`, `_mouseOutsideX`, `AutoScrollMarginPx`, and `AutoScrollPxScaleFactor`.

Initialize the timer from the `MarksBar(TimeInfo timeinfo, Guid instanceId)` constructor. The timer interval should match the effect grid unless implementation evidence suggests a different value; start with 50 milliseconds because `Grid_Mouse.cs` uses `m_autoScrollTimer.Interval = 50`. Dispose the timer in `Dispose(bool disposing)` and unsubscribe its tick handler, mirroring the grid cleanup pattern.

In `OnMouseMove`, before `HandleMouseMove(e)` or immediately after it, update auto-scroll state only when `_dragState` is `DragState.Moving` or `DragState.HResizing`. Store `_lastMouseMove = e`. Compute the pointer's horizontal distance into the left or right edge zone using the raw control-relative `e.X`, not `TranslateLocation(e.Location)`, because edge detection is about the visible control bounds. If `e.X <= AutoScrollMarginPx`, set `_mouseOutsideX` to a negative value whose magnitude grows as the pointer moves left. If `e.X >= ClientSize.Width - AutoScrollMarginPx`, set `_mouseOutsideX` to a positive value whose magnitude grows as the pointer moves right. Otherwise set `_mouseOutsideX` to zero. Start the timer when `_mouseOutsideX` is nonzero and stop it when `_mouseOutsideX` is zero.

In the timer tick, convert `_mouseOutsideX / AutoScrollPxScaleFactor` into a time delta using `PixelsToTime`. Add that delta to `VisibleTimeStart`, clamping the result to the valid horizontal range. The minimum is `TimeSpan.Zero`. The maximum is normally `TotalTime - VisibleTimeSpan`, but if `VisibleTimeSpan >= TotalTime`, the maximum must be `TimeSpan.Zero`. Assign the clamped result back to `VisibleTimeStart`. If the clamped value did not change, leave the timer running only if the mouse may become movable again by changing direction; otherwise it is acceptable to keep the timer active until mouse-up because the replayed mouse move will continue to clamp Mark movement at the sequence boundary.

After changing `VisibleTimeStart`, call `HandleMouseMove(_lastMouseMove)` so the active move or resize recalculates against the new visible origin. This is critical because `TranslateLocation` includes `VisibleTimeStart`; without replay, the viewport would scroll but the Mark would not continue moving until another mouse event arrives. Guard against null `_lastMouseMove` and against reentrant timer calls if needed.

Stop the timer in `EndAllDrag`, in `OnMouseUp` after finishing the operation, and in any cancel/cleanup path found during implementation. `EndAllDrag` already resets `_dragState`, `Cursor`, and `_mouseDownMark`; extend it to stop auto-scroll and clear `_mouseOutsideX` and `_lastMouseMove`. This ensures auto-scroll cannot continue after the user releases the mouse.

Do not change `MouseMove_DragMoving` or `MouseMove_HResizing` boundary behavior except where strictly needed to support replay after scrolling. Those methods are already responsible for Mark time constraints. The new code should change the viewport, then let those existing methods compute and clamp the Mark's actual time. Preserve calls to `_timeLineGlobalEventManager.OnMarksMoving`, `_timeLineGlobalEventManager.OnAlignmentActivity`, and `FinishedResizeMoveMarks`, because they drive live refresh, alignment visuals, and undo.

If the implementation extracts a helper such as `GetAutoScrollVisibleTimeStart(TimeSpan currentStart, int mouseOutsideX)` or `ClampVisibleTimeStart(TimeSpan candidate)`, keep it private or internal unless tests require broader visibility. If tests need access, prefer an internal helper in the same assembly with a clear name and no dependency on WinForms timer state rather than making UI internals public.

Milestone 3 adds focused automated coverage where it is stable. Start by inspecting `src/Vixen.Tests/Sequencer/TimelineActiveRowNavigationTests.cs`, `TimelineCursorSelectionTests.cs`, and `TimelineControlTestCollection.cs` for patterns around instantiating timeline controls. If a helper was extracted for clamping or scroll delta calculation, add tests that prove:

- scrolling left clamps `VisibleTimeStart` at `TimeSpan.Zero`;
- scrolling right clamps `VisibleTimeStart` at `TotalTime - VisibleTimeSpan`;
- no scrolling is possible when `VisibleTimeSpan >= TotalTime`;
- a positive right-edge mouse offset advances time and a negative left-edge mouse offset moves time backward.

If the existing test project can cleanly instantiate `MarksBar` and call non-public methods through normal public behavior, add a focused test for the helper without relying on real timer ticks or screen coordinates. If doing so would require brittle reflection-heavy or timing-dependent tests, record that in `Surprises & Discoveries` and rely on helper tests plus manual UI validation.

Milestone 4 validates and closes out. Run focused tests first, then the full Vixen test project. Manually validate in the Timed Sequence Editor using a sequence that has at least one visible Mark Collection with several Marks across a timeline longer than the visible viewport. Update Jira VIX-3944 with implementation notes, automated test results, manual verification results, and any known limitations. Update `Progress`, `Surprises & Discoveries`, `Decision Log`, `Outcomes & Retrospective`, and `Artifacts and Notes` with the final evidence.

## Concrete Steps

Work from repository root `C:\Dev\Vixen`.

First inspect the current files:

    rg -n "MouseMove_DragMoving|MouseMove_HResizing|EndAllDrag|TranslateLocation|VisibleTimeStart|Dispose|AutoScroll|m_autoScrollTimer|HandleHorizontalScroll" src\Vixen.Common\Controls\TimeLineControl

Update Jira VIX-3944 using the Milestone 1 text in `Artifacts and Notes`. If Jira tools are not available, record that in `Surprises & Discoveries` and leave the paste-ready text in this plan so a human can update it manually.

Implement the `MarksBar` auto-scroll fields and timer setup in `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`. Keep the code local to `MarksBar`; do not modify `Grid_Mouse.cs` unless implementation discovers a small reusable helper that genuinely reduces duplication without changing grid behavior.

Add methods with names close to these, adjusted to match final code style:

    private void InitAutoScrollTimer()
    private void UpdateAutoScrollState(MouseEventArgs e)
    private bool IsAutoScrollDragState()
    private void AutoScrollTimer_Tick(object sender, EventArgs e)
    private TimeSpan ClampVisibleTimeStart(TimeSpan visibleTimeStart)
    private void StopAutoScroll()

Use `System.Windows.Forms.Timer`, not a background-thread timer. The drag state and Mark updates are WinForms UI state and must stay on the UI thread.

In `OnMouseMove`, keep the existing right-button early return. For left-button move and resize states, record the latest event, update auto-scroll state, then call `HandleMouseMove(e)`. If the exact ordering affects responsiveness during manual testing, prefer the order that matches the grid's behavior: the grid records auto-scroll state before calling `HandleMouseMove(e)`.

In `AutoScrollTimer_Tick`, update `VisibleTimeStart` using `PixelsToTime(_mouseOutsideX / AutoScrollPxScaleFactor)`, then call `HandleMouseMove(_lastMouseMove)`. Because `HandleMouseMove` calls `TranslateLocation`, the same raw mouse point maps to a later or earlier timeline coordinate after `VisibleTimeStart` changes.

Run formatting only if necessary. Do not reformat unrelated C# code.

Run focused tests. If a new test class is named `MarksBarAutoScrollTests`, use:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~MarksBarAutoScroll --no-restore

If tests are added to an existing sequencer test class, adjust the filter to that class name.

Run the full test project:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Manual validation in the running application requires a Debug or Release build. From repository root, build with one of:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

or:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release

Launch the built Vixen application from the corresponding output folder, open the Timed Sequence Editor, show one or more Mark Collections in the Marks Bar, zoom in so the full sequence is wider than the visible viewport, and perform the manual scenarios in `Validation and Acceptance`.

## Validation and Acceptance

Automated validation should include at least one focused command and the full test command:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~MarksBarAutoScroll --no-restore
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If the focused test class uses a different name, update this plan with the actual command. A successful focused run should report all new Mark auto-scroll tests passed. A successful full run should report all `Vixen.Tests` tests passed. If unrelated pre-existing failures appear, capture their names and evidence, then also capture a passing focused run for this feature.

Manual acceptance is required because this is a pointer-and-timer workflow in a WinForms control. Verify these scenarios in the Timed Sequence Editor:

1. With the timeline zoomed so only part of the sequence is visible, drag a single Mark to the right edge of the Marks Bar and keep holding the mouse button. The visible timeline scrolls right and the Mark continues moving until the user releases the mouse or the Mark reaches sequence end.
2. Drag a single Mark to the left edge. The visible timeline scrolls left and the Mark continues moving until the user releases the mouse or the Mark reaches time zero.
3. Resize a Mark by its right edge past the right visible edge. The visible timeline scrolls right and the Mark duration continues increasing until release, minimum/maximum constraints, or sequence end.
4. Resize a Mark by its left edge past the left visible edge. The visible timeline scrolls left and the Mark start continues moving earlier while duration changes, stopping at time zero or minimum width.
5. Select multiple Marks and drag them as a group past both left and right visible edges. All selected Marks maintain their relative offsets and remain constrained by the earliest start and latest end.
6. With a single selected Mark, hold Alt and resize against an adjacent glued Mark past the visible edge. The viewport scrolls and the adjacent Mark adjustment remains correct.
7. Release the mouse while auto-scroll is active. Scrolling stops immediately and no Mark continues moving or resizing.
8. Existing effect-grid drag and resize auto-scroll still works after the change.
9. Undo after a completed Mark move or resize restores the previous Mark times, confirming `MarksMovedEventArgs` and existing undo integration were preserved.

Acceptance criteria for VIX-3944 are:

- Moving selected Marks in the Marks Bar auto-scrolls horizontally when the cursor is dragged beyond the left or right visible timeline edge.
- Resizing Marks in the Marks Bar auto-scrolls horizontally when the resize handle is dragged beyond the left or right visible timeline edge.
- Auto-scroll behavior is similar to effect drag and resize auto-scroll in trigger distance, timer feel, and continuous update behavior.
- Mark movement and resizing remain constrained by sequence start, sequence end, and minimum Mark width.
- Multi-selected Marks and Alt glued resize continue to work.
- Auto-scroll stops when the drag or resize operation ends or is cancelled.
- Existing effect drag and resize auto-scroll behavior is not regressed.
- Jira issue VIX-3944 is updated before implementation with requirements, acceptance criteria, and test plan, and updated after validation with final evidence.

## Idempotence and Recovery

The implementation is additive and can be done incrementally. Re-running tests is safe. Updating Jira should be done once before implementation and once after validation; if a Jira update fails, do not retry blindly with duplicate comments. First read the issue or comments to see whether the previous attempt succeeded.

If the auto-scroll timer causes runaway scrolling during development, disable the timer start call and validate the helper/clamping logic before re-enabling it. The timer must always be stopped in `EndAllDrag` and disposed in `Dispose(bool disposing)`. If manual testing reveals that the timer can continue after losing focus or mouse capture, add a cleanup hook such as `OnMouseCaptureChanged`, `OnLeave`, or another existing WinForms cancellation point, and record the discovery in this plan.

Do not change Mark model persistence, Mark Collection serialization, undo action types, or the effect grid's existing auto-scroll behavior unless new evidence shows a required shared fix. If a broader refactor appears tempting, record it as out of scope and keep VIX-3944 focused on Marks Bar auto-scroll.

## Artifacts and Notes

Milestone 1 Jira update text:

    ### VIX-3944 Marks Bar Drag and Resize Auto-Scroll

    #### Overview

    Users can drag-move and resize Marks in the Timed Sequence Editor's Marks Bar. Today, when the cursor reaches the left or right edge of the visible timeline, the timeline does not auto-scroll, so users cannot continue the operation into off-screen time without releasing the mouse and manually scrolling. This improvement adds horizontal auto-scroll to Mark move and resize operations, matching the workflow already available when moving or resizing effects in the main timeline grid.

    #### Requirements

    - When moving one or more selected Marks, dragging into the left or right edge zone of the Marks Bar scrolls the visible timeline horizontally while the move continues.
    - When resizing a Mark from either edge, dragging into the left or right edge zone scrolls the visible timeline horizontally while the resize continues.
    - Scrolling is horizontal only and affects timeline time, not Mark Collection visibility or row layout.
    - The implementation should feel similar to effect drag/resize auto-scroll: edge proximity starts scrolling, scrolling repeats while the cursor remains at the edge, and the active drag/resize updates continuously.
    - Existing Mark constraints remain in force: Marks cannot move before time zero or beyond sequence end, resizing respects minimum Mark width, multi-selected Marks keep relative offsets, and Alt glued resize continues to adjust the adjacent Mark.
    - Auto-scroll stops immediately when the drag or resize operation ends or is cancelled.
    - Existing effect drag/resize auto-scroll must not regress.

    #### High-Level Design

    Implement the behavior in `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`, where Mark selection, drag move, and drag resize already live. Use the existing effect-grid auto-scroll in `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` as the behavioral model: track the last mouse move, detect when the cursor is in the left or right edge zone, use a UI timer to advance the viewport repeatedly, and replay the last mouse move after each scroll tick.

    `MarksBar` should update `VisibleTimeStart` directly, clamped between zero and the latest valid visible start. `MarksBar.TranslateLocation` and painting already use `VisibleTimeStart`, so replaying the last mouse move after the visible start changes lets the existing move/resize code recalculate the Mark time using the new viewport. The existing `MouseMove_DragMoving`, `MouseMove_HResizing`, `MarksMoving`, and `MarksMoved` behavior should remain responsible for Mark constraints, live refresh, and undo integration.

    #### Acceptance Criteria

    - Moving selected Marks in the Marks Bar auto-scrolls horizontally when the cursor is dragged beyond the left or right visible timeline edge.
    - Resizing Marks in the Marks Bar auto-scrolls horizontally when the resize handle is dragged beyond the left or right visible timeline edge.
    - Auto-scroll behavior is similar to effect drag and resize auto-scroll.
    - Mark movement and resizing remain constrained by sequence start, sequence end, and minimum Mark width.
    - Multi-selected Marks and Alt glued resize continue to work.
    - Auto-scroll stops when the drag or resize operation ends or is cancelled.
    - Existing effect drag and resize auto-scroll behavior is not regressed.

    #### Test Plan

    Automated tests should cover any extracted scroll delta and clamping helper, including left clamp at zero, right clamp at sequence end, no-scroll behavior when the whole sequence is visible, and positive/negative scroll deltas. Manual Timed Sequence Editor validation must cover single Mark move left/right, single Mark resize from both edges, multi-selected Mark move, Alt glued resize, mouse release while scrolling, undo after a completed Mark time change, and regression of effect drag/resize auto-scroll.

Milestone 1 Jira update:

    2026-07-16 16:09 -05:00 / Codex: Read VIX-3944 using cloud ID cc8261dc-f522-4dfa-96f0-3effdc1f0a1f. The issue summary is "Scroll timeline when moving marks and mark labels", issue type is Improvement, status is In Progress, priority is Normal, assignee is Jeff Uchitjil, and the existing description was the short original reporter text: "When dragging to move or resize marks and mark labels scroll the timeline with the movement to keep the direction you’re moving to visible." Added the Milestone 1 planning content as Jira comment 40204. The issue description was not edited because this session's exposed Atlassian tools did not include a direct issue-description update tool.

Record validation transcripts here as implementation proceeds. Keep them concise, for example:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~MarksBarAutoScroll --no-restore
    Passed!  - Failed: 0, Passed: N, Skipped: 0, Total: N

## Interfaces and Dependencies

This work should use existing .NET WinForms APIs and existing Vixen timeline types only. Use `System.Windows.Forms.Timer` for UI-thread timer ticks. Do not introduce a new package.

The intended implementation surface is private to `MarksBar` unless tests require a small internal helper. At the end of implementation, `MarksBar` should still expose no new public API unless a deliberate design decision is recorded. The important behavior is:

    private void InitAutoScrollTimer()
    private void UpdateAutoScrollState(MouseEventArgs e)
    private void AutoScrollTimer_Tick(object sender, EventArgs e)
    private TimeSpan ClampVisibleTimeStart(TimeSpan candidate)
    private void StopAutoScroll()

Exact names may differ to match local style, but the responsibilities must remain clear. `MouseMove_DragMoving` and `MouseMove_HResizing` should continue to own Mark time calculation and constraints. `FinishedResizeMoveMarks` should continue to raise `MarksMovedEventArgs` once per completed operation so undo remains unchanged.

## Change Notes

- 2026-07-16 / Codex: Initial ExecPlan created from VIX-3944 requirements and current source research in `MarksBar.cs`, `Grid_Mouse.cs`, `Grid.cs`, and `TimelineControlBase.cs`. The plan resolves the open scope questions as horizontal-only auto-scroll with effect-grid-like timing and focused helper tests plus manual UI validation.
- 2026-07-16 / Codex: Completed Milestone 1 by adding Jira planning comment 40204 to VIX-3944, then updated this ExecPlan's living sections with the Jira evidence and the reason the update was a comment rather than an issue-description edit.

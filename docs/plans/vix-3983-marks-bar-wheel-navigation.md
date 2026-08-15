# VIX-3983: Make Marks Bar pan and zoom match the timeline grid

This ExecPlan is a living document. Maintain its `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections as work proceeds. Follow `.agents/plans.md` from the repository root when revising this document.

## Purpose / Big Picture

Vixen's sequencer has a timeline grid where users navigate horizontally with wheel gestures. Its Marks Bar is the narrow control above the grid used to display and edit marks. After this work, users can place the pointer over either the grid or the Marks Bar and receive the same horizontal pan and zoom result for the same navigation gesture. This is observable by opening a sequence whose duration exceeds the visible time span, comparing viewport movement over both surfaces, and confirming mark editing remains unchanged.

## Progress

- [x] (2026-08-15 00:00Z) Researched the timeline event paths and wrote the approved technical specification in `docs/sequencer/vix-3983-marks-bar-wheel-navigation.md`.
- [x] (2026-08-15 14:29Z) Updated VIX-3983 with a user-facing summary, scope, and acceptance criteria; detailed implementation notes remain in the local specification and this ExecPlan.
- [x] (2026-08-15 14:36Z) Added Marks Bar wheel-event subscriptions, narrow pan/zoom dispatch, coordinate translation for pointer-relative zoom, and a shared native-horizontal pan helper used by Grid and Marks Bar.
- [ ] Add focused sequencer tests for Marks Bar navigation parity.
- [ ] Build and run focused and complete test suites; perform manual navigation and mark-editing regression checks.
- [ ] Update VIX-3983 with final requirements alignment and validation results.

## Surprises & Discoveries

- Observation: The source path for horizontal pan is not solely `TimelineControl.OnMouseWheel`.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/TimelineControlBase.cs` handles native `WM_MOUSEHWHEEL` and raises `MouseHWheel`; `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` overrides `OnMouseHWheel` and adjusts `VisibleTimeStart` by ten percent of the visible span.

- Observation: `Grid.OnMouseWheel` deliberately suppresses the base WinForms behavior.
  Evidence: `Grid_Mouse.cs` contains an empty `OnMouseWheel` override with a comment explaining that the base behavior may scroll unexpectedly when no vertical scrollbar is available.

## Decision Log

- Decision: Treat user-visible parity as the governing requirement rather than assuming one particular WinForms event path.
  Rationale: The user verified Shift/Ctrl+Shift pan and Ctrl zoom over the grid, while source inspection found both vertical-wheel and native-horizontal-wheel paths. Supporting the Marks Bar's inherited `MouseWheel` and `MouseHWheel` events is necessary to reproduce what users experience.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Route only pan and zoom from Marks Bar events; do not forward a complete event to `TimelineControl.OnMouseWheel`.
  Rationale: Whole-event forwarding could activate vertical scrolling and row-height branches that are not navigation parity requirements and could use an invalid child-control coordinate.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Keep `MarksBar.cs` unchanged.
  Rationale: Marks Bar already inherits the needed events from `TimelineControlBase`; navigation policy belongs to its composite owner, `TimelineControl`.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Keep the Jira description concise and user-facing.
  Rationale: The tracker should describe the user benefit and reviewable acceptance criteria. The repository-local specification and ExecPlan are the source of truth for event routing, code locations, and test design.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Centralize native horizontal-wheel math in an internal TimelineControl helper.
  Rationale: Grid and Marks Bar are separate child controls, but both must retain the grid's established positive/negative native-wheel movement. A shared helper prevents their calculations from drifting while preserving each control's existing shared `TimeInfo` state.
  Date/Author: 2026-08-15 / Codex

## Outcomes & Retrospective

No implementation has started. Update this section after validation with the behavior achieved, validation commands and outcomes, remaining gaps, and any follow-up discovered during testing.

## Context and Orientation

`src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` defines the composite WinForms sequencer control. It creates a `Grid`, a `MarksBar`, a ruler, and a waveform in the right pane of a split container. These controls receive one shared `TimeInfo` instance from the TimelineControl constructor. `TimeInfo` contains timeline viewport state, so setting `TimelineControl.VisibleTimeStart` redraws all relevant surfaces.

`src/Vixen.Common/Controls/TimeLineControl/TimelineControlBase.cs` is the common base control. It clamps `VisibleTimeStart` to zero and the latest allowed start. It also processes Windows message `WM_MOUSEHWHEEL`, the native horizontal-wheel message, and raises its `MouseHWheel` event.

`src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` contains the grid's input overrides. Its `OnMouseHWheel` currently moves the shared viewport. `TimelineControl.OnMouseWheel` contains vertical-wheel modifier behavior, including Ctrl zoom. Inspect both paths before editing: the effective gesture route depends on the input message and which child has focus.

`src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` derives from `TimelineControlBase` and calls `Focus()` in `OnMouseDown`; do not edit it. Its inherited `MouseWheel` and `MouseHWheel` events are the integration points. `src/Vixen.Tests/Sequencer/TimelineControlTestCollection.cs` disables parallelism for WinForms timeline tests. Existing `MarksBarAutoScrollTests.cs` is the closest Marks Bar test style reference. The Controls project exposes internals to `Vixen.Tests` through `src/Vixen.Common/Controls/Controls.csproj`.

## Plan of Work

### Milestone 1: Record the approved issue contract in Jira

Use the repository Jira workflow to update VIX-3983 before code changes. Include the purpose, the behavior matrix in this plan, the explicit rule that Shift combinations pan and Ctrl without Shift zooms, the no-plain-wheel/no-row-height Marks Bar boundary, automated tests, and manual regression checks. This milestone is complete when the tracker description is enough for a reviewer to understand scope without reading source code.

Run the project Jira skill or connected Jira tool from the repository root. If the tracker is unavailable, document the exact access failure in `Surprises & Discoveries`, continue with local implementation, and leave the final Jira update pending rather than inventing tracker state.

### Milestone 2: Establish one TimelineControl-owned navigation policy

Read all of `TimelineControl.cs`, `TimelineControlBase.cs`, `Grid_Mouse.cs`, and `MarksBar.cs` before editing. In `TimelineControl.InitializePanel2()`, subscribe to `MarksBar.MouseWheel` and `MarksBar.MouseHWheel` after creating Marks Bar. In `TimelineControl.Dispose(bool)`, unsubscribe both handlers before `MarksBar.Dispose()`.

Extract the existing standard vertical Shift-pan calculation into a private TimelineControl helper. Also extract or centralize the grid's native-horizontal-wheel calculation so the grid and Marks Bar call the same helper rather than holding similar calculations in two places. Preserve `TimelineControl.VisibleTimeStart` as the only clamping route.

Add dedicated Marks Bar event handlers that perform a narrow dispatch. For a vertical-wheel event, Shift has priority: Shift, Ctrl+Shift, and Shift+Alt call the standard horizontal-pan helper; Ctrl without Shift invokes the same existing Ctrl zoom logic as TimelineControl. Translate the Marks Bar pointer point to TimelineControl coordinates before calling pointer-relative zoom so `ZoomToMousePosition` remains equivalent to grid behavior. Do not call row-height methods or adjust `VerticalOffset` from these handlers. For native horizontal-wheel input, call the shared native-horizontal pan helper.

Keep the main grid's behavior unchanged unless focused manual testing proves a source-level change is necessary for parity. If a change is necessary, update the Decision Log before making it, explain the observed input route, and add tests covering both controls.

At the end of this milestone, build the affected project or solution target and inspect the diff to confirm only the intended control files changed. The visible result is that Marks Bar events update the shared timeline viewport or zoom state with no direct Marks Bar viewport math.

### Milestone 3: Add direct, deterministic navigation-parity tests

Create `src/Vixen.Tests/Sequencer/MarksBarMouseWheelTests.cs` and apply `[Collection(TimelineControlTestCollection.Name)]`. Use a `TimelineControl` with a controlled `TotalTime`, `TimePerPixel`, size, and initial visible start. Add only internal test seams necessary to invoke the narrow Marks Bar dispatch or shared navigation helpers; the existing `InternalsVisibleTo("Vixen.Tests")` configuration permits this without reflection.

Test standard vertical pan with Shift and deltas `-120` and `+120`, verify Shift+Alt and Ctrl+Shift pan, verify a partial delta is proportional, and verify left and right bounds. Test Ctrl without Shift changes the zoom state by the same scale as the grid path, including pointer-relative behavior if `ZoomToMousePosition` is enabled. Test both positive and negative native horizontal-wheel deltas against the grid helper result. Verify no-modifier input and Ctrl+Alt do not move the Marks Bar viewport or change row height. Dispose every TimelineControl with `using`.

The milestone is complete when these tests fail against the prior code path and pass with the new routing, and existing `MarksBarAutoScrollTests` remain green.

### Milestone 4: Validate in the application and close the tracker loop

From `C:\Dev\Vixen`, first build the test target with full Visual Studio MSBuild because the test graph includes C++/CLI projects. Then run the focused class and the full test project without rebuilding:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\" --filter FullyQualifiedName~MarksBarMouseWheelTests
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\"

Expect MSBuild to finish without errors and each `dotnet test` command to report zero failed tests. If infrastructure prevents a test run, capture the command and error verbatim in this plan and run all viable lower-scope checks.

Manually open a sequence longer than the viewport. Hover the grid and Marks Bar in turn, then compare Shift-wheel, Ctrl+Shift-wheel, Shift+Alt-wheel, Ctrl-wheel, and native horizontal-wheel input if hardware is available. Confirm matching pan/zoom movement, no plain-wheel/row-height behavior over Marks Bar, mark drag and resize, and the VIX-3944 auto-scroll scenario.

Finally, update VIX-3983's description if implementation discoveries refined its wording and add a Jira comment containing build, focused-test, full-suite, and manual-check results. Mark the Progress items and Outcomes & Retrospective with the actual evidence.

## Concrete Steps

All commands run from `C:\Dev\Vixen`.

1. Inspect the current navigation implementation before editing:

       rg -n -C 6 "OnMouseWheel|OnMouseHWheel|MouseHWheel|VisibleTimeStart" src/Vixen.Common/Controls/TimeLineControl -g "*.cs"

2. Make the Milestone 2 and Milestone 3 edits with tabs and LF, then review only the intended change:

       git diff --check
       git diff -- src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs src/Vixen.Tests/Sequencer/MarksBarMouseWheelTests.cs

3. Run the Milestone 4 commands. A successful focused run contains a summary like:

       Passed!  - Failed:     0, Passed:     <count>, Skipped:     0

4. Record actual test counts, the exact manual scenario, and results in this plan and Jira.

## Validation and Acceptance

The implementation is accepted only when a user cannot distinguish grid from Marks Bar for the supported navigation gestures. In a sequence wider than the viewport, Shift-wheel and Ctrl+Shift-wheel must pan over both controls with the same direction, amount, proportional partial-delta response, and boundaries. Ctrl-wheel without Shift must zoom by the same scale and use the same cursor anchoring behavior. A native horizontal wheel must pan over both controls equally. No modifier and row-height gestures must not gain new behavior over Marks Bar.

Automated acceptance requires the new `MarksBarMouseWheelTests`, existing sequencer tests, and the full `Vixen.Tests` suite to pass with no failures after a full x64 MSBuild test build. Manual acceptance additionally requires successful Mark drag/resize and VIX-3944 auto-scroll regression checks.

## Idempotence and Recovery

Event subscription changes must be paired with matching unsubscriptions so repeated creation and disposal of TimelineControl does not retain handlers. The test file is additive and can be rerun without setup beyond the Visual Studio build prerequisites. If a build fails due to missing C++/CLI tooling, do not delete build outputs or change project references; record the failure and retry on a machine with the required Visual Studio C++ toolset. Revert only the files introduced for this issue if abandoning the work, after confirming their exact paths with `git status --short`.

## Artifacts and Notes

The approved product specification is `docs/sequencer/vix-3983-marks-bar-wheel-navigation.md`. Source discovery established these relevant facts:

    TimelineControlBase.WndProc: WM_MOUSEHWHEEL -> MouseHWheel
    Grid.OnMouseHWheel: updates VisibleTimeStart by +/- 10% of VisibleTimeSpan
    Grid.OnMouseWheel: intentionally empty
    TimelineControl.OnMouseWheel: owns Ctrl zoom and an inline Shift pan branch

Milestone 2 build evidence:

    dotnet build src/Vixen.Common/Controls/Controls.csproj -c Release --no-restore
    Build succeeded. 4 existing Vixen.Core warnings; 0 errors.

The plan intentionally avoids assuming a single input-message route because WinForms dispatch depends on the focused child and input device. Its acceptance condition is visible parity rather than reproducing incidental message delivery.

## Interfaces and Dependencies

Use only existing .NET WinForms types and Vixen timeline types. `System.Windows.Forms.MouseEventArgs` supplies delta and pointer coordinates; `System.Windows.Forms.Keys` represents modifier state. `TimelineControl` is the single policy owner and may expose narrow `internal` methods solely for `Vixen.Tests`, which is already a friend assembly of the Controls project.

The final implementation must keep the following conceptual interfaces local to `TimelineControl`:

    private void MarksBarMouseWheelHandler(object sender, MouseEventArgs e)
    private void MarksBarMouseHWheelHandler(object sender, MouseEventArgs e)
    private void PanTimelineHorizontally(int delta)

The exact internal test seam names and signatures may be finalized during Milestone 3, but they must accept only the values needed for deterministic gesture dispatch and must not become public API.

Plan revision note (2026-08-15): Initial approved plan created after source inspection found that the grid uses both ordinary wheel and native horizontal-wheel paths. The plan therefore requires behavior parity across both event routes instead of relying on the original MouseWheel-only handoff.

Plan revision note (2026-08-15): Completed Milestone 1 by updating VIX-3983 with a general user-facing description and acceptance criteria. Detailed technical design remains in the local documentation at the user's direction.

Plan revision note (2026-08-15): Completed Milestone 2. Marks Bar now subscribes to ordinary and native horizontal-wheel events and dispatches only approved pan and zoom behavior through TimelineControl helpers. Grid now uses the shared native-horizontal pan helper.

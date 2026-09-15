# VIX-4001: Apply Timeline Zoom Limits to Ctrl-Wheel Zoom

This ExecPlan is a living document. Maintain its `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections as work proceeds. Follow `.agents/PLANS.md` from the repository root when revising this document.

## Purpose / Big Picture

The sequencer currently honors its horizontal zoom limits when users choose a toolbar zoom command, but Ctrl+mouse-wheel zoom anchored to the cursor can bypass them. After this change, both ways of zooming stop at the same minimum time resolution, show the full sequence when zooming out as far as possible, and leave the viewport unchanged when no valid zoom can be applied. Users can observe this by Ctrl-wheeling at either boundary and by using the toolbar zoom command with the same starting view.

## Progress

- [x] (2026-09-15 00:00Z) Inspected the current toolbar zoom, pointer-relative zoom, Marks Bar Ctrl-wheel route, and existing WinForms tests.
- [x] (2026-09-15 00:18Z) Updated VIX-4001 with user-facing scope and acceptance criteria.
- [x] (2026-09-15 00:22Z) Extracted the shared zoom-limit decision and used it from `Zoom` and `ZoomTime` while preserving their distinct viewport positioning.
- [x] (2026-09-15 00:25Z) Added focused Ctrl-wheel minimum-boundary, maximum-boundary toolbar-parity, and valid pointer-focused zoom tests.
- [x] (2026-09-15 00:31Z) Built the x64 test target and passed all 15 focused Marks Bar mouse-wheel tests.
- [x] (2026-09-15 00:33Z) Added Jira comment 40485 with the x64 build and focused-test results.
- [x] (2026-09-15 00:40Z) Moved timeline zoom-boundary tests into the dedicated `TimelineZoomTests` class; Marks Bar tests now cover event routing only.
- [x] (2026-09-15 00:44Z) Rebuilt the x64 test target and passed all 15 combined timeline-zoom and Marks Bar wheel tests after the reorganization.
- [x] (2026-09-15 00:35Z) Recorded user-provided full-suite and manual-validation evidence in Jira comment 40486.

## Surprises & Discoveries

- Observation: The existing Marks Bar test seam already dispatches Ctrl-wheel to `ZoomTime` when `ZoomToMousePosition` is enabled.
  Evidence: `TimelineControl.HandleMarksBarMouseWheel(int, Keys, Point)` calls `ZoomTimelineHorizontally`, which selects `ZoomTime` for that setting.

- Observation: The existing toolbar full-sequence calculation can display a span a few ticks shorter than `TotalTime` because `TotalTime.Ticks / grid.Width` truncates fractional ticks.
  Evidence: The first focused maximum-boundary assertion observed `00:09:59.9999148` for a ten-minute sequence. Both toolbar and Ctrl-wheel produce this identical existing result.

## Decision Log

- Decision: Keep viewport positioning outside the shared helper.
  Rationale: Toolbar zoom preserves the right edge after an ordinary valid zoom, while pointer-relative zoom preserves cursor focus. The shared concern is only whether a scaled time-per-pixel value is valid and whether maximum zoom-out requires a full-sequence reset.
  Date/Author: 2026-09-15 / Codex

- Decision: Test maximum zoom-out against toolbar parity and a zero start rather than requiring exact `VisibleTimeSpan == TotalTime`.
  Rationale: The handoff requires Ctrl-wheel to match the toolbar's established calculation. Exact-span equality would incorrectly reject that calculation's known tick truncation while providing no additional user-visible guardrail coverage.
  Date/Author: 2026-09-15 / Codex

## Outcomes & Retrospective

The implementation now shares a single private zoom-validity calculation between toolbar and pointer-focused zoom. Ctrl-wheel no longer applies rejected scales, maximum zoom-out resets to zero like the toolbar, and valid pointer-focused zoom retains its established offset behavior. The behavior tests are owned by `TimelineZoomTests`, while the Marks Bar tests are limited to event-routing coverage. VIX-4001 has the finalized user-facing description and validation comments 40485 and 40486. The x64 test build and all 939 unit tests passed. Manual testing confirmed the Time + and Time - buttons and Ctrl+mouse-wheel zoom share the same limits.

## Context and Orientation

`src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` defines the composite WinForms timeline used by the sequencer. `TimePerPixel` is the duration represented by one horizontal grid pixel; multiplying it by the grid width gives `VisibleTimeSpan`, the time currently displayed. `TotalTime` is the sequence duration and `VisibleTimeStart` is the left edge of that displayed range. `TimelineControlBase` clamps the start so users cannot pan outside the sequence.

The public `TimelineControl.Zoom(double)` method is the toolbar-driven route. It rejects nonpositive scales. When the scaled visible range would exceed `TotalTime`, it computes the full-sequence `TimePerPixel` from `TotalTime.Ticks / grid.Width`, applies it only if it is greater than 2,000 ticks, and resets the start to zero. Otherwise it applies a scaled value only when it remains greater than 2,000 ticks and, if necessary, moves the start to retain the right edge.

The public `TimelineControl.ZoomTime(double, Point)` method is the cursor-focused route used when Ctrl-wheel is routed through `ZoomTimelineHorizontally` with `ZoomToMousePosition` enabled. It currently assigns the scaled value directly and then moves `VisibleTimeStart` to retain the pointer's time location. It must receive the same bounds decision as `Zoom`, but it must not run focus-offset math after a rejected scale or after the full-sequence reset. Its XML documentation must state the bounded behavior.

`src/Vixen.Tests/Sequencer/TimelineZoomTests.cs` owns the xUnit WinForms tests for `TimelineControl.Zoom` and `TimelineControl.ZoomTime`. `src/Vixen.Tests/Sequencer/MarksBarMouseWheelTests.cs` uses the internal `HandleMarksBarMouseWheel` seam only to verify Marks Bar event dispatch. The test collection serializes access to WinForms timeline controls. Tests construct deterministic controls and dispose controls with `using`.

## Plan of Work

### Milestone 1: Record the user-facing issue contract in Jira

Update VIX-4001 before source edits. State that Ctrl-wheel zoom follows the same safe limits as toolbar zoom, retains pointer-focused zoom while a change is valid, shows the complete sequence from the beginning at the maximum zoom-out boundary, and does nothing at the minimum resolution boundary. Include the automated verification at a user-outcome level. Keep class names, file names, formulas, and test seams in this ExecPlan rather than the Jira description. If Jira access is unavailable, record the exact failure below and continue with the local implementation.

### Milestone 2: Centralize only the shared zoom decision

In `TimelineControl.cs`, add a private helper near `Zoom` and `ZoomTime` that accepts a scale and reports whether it can apply a new `TimePerPixel`, the resulting value, and whether that value represents maximum zoom-out. Reject `scale <= 0`. For a requested visible span larger than `TotalTime`, calculate the existing full-sequence value with `TotalTime.Ticks / grid.Width`; reject the request if that value is not greater than 2,000 ticks. For all other requests, calculate `TimePerPixel.Scale(scale)` and reject it when it is not greater than 2,000 ticks.

Make `Zoom` call the helper before beginning drawing. If rejected, leave all state untouched. If it receives a maximum zoom-out result, set `TimePerPixel`, set `VisibleTimeStart` to `TimeSpan.Zero`, and return after ending drawing. For an ordinary result, set `TimePerPixel` and retain the established right-edge correction.

Make `ZoomTime` call the same helper before beginning drawing. If rejected, leave all state untouched. If it receives a maximum zoom-out result, set `TimePerPixel`, set `VisibleTimeStart` to zero, and do not calculate a mouse offset. For an ordinary result, capture the original visible span, apply the resulting `TimePerPixel`, then perform the existing pointer-focus calculation and start adjustment. Add complete XML documentation for this changed public method, including both parameters and the no-op/full-sequence boundary behavior.

### Milestone 3: Prove Ctrl-wheel boundary behavior

Create `TimelineZoomTests.cs` with three xUnit WinForms tests for `TimelineControl.Zoom` and `TimelineControl.ZoomTime`. First set a time-per-pixel value whose next pointer-focused zoom-in falls at or below 2,000 ticks and verify both time-per-pixel and visible start remain unchanged. Next create equivalent controls that exceed the viewport on zoom-out; invoke toolbar `Zoom` on one and `ZoomTime` on the other, then verify the pointer-focused result matches the toolbar result and starts at zero. Finally use a valid scale and a nonzero pointer location to verify time-per-pixel changes and pointer-focused movement still changes the visible start instead of being treated as a boundary no-op. Keep `MarksBarMouseWheelTests.cs` focused on asserting that Ctrl-wheel dispatches to timeline zoom.

### Milestone 4: Validate and close the tracker loop

From `C:\Dev\Vixen`, build the test target with full Visual Studio MSBuild because the test project depends transitively on C++/CLI projects, then run the focused tests without rebuilding:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\" --filter "FullyQualifiedName~TimelineZoomTests|FullyQualifiedName~MarksBarMouseWheelTests"

Expect both commands to end with zero errors or failures. Run `git diff --check` and inspect the diff limited to the timeline control, test file, and ExecPlan. Update the Jira description if requirements changed, add a concise Jira comment with the actual build/test result, and update every living section of this plan.

## Concrete Steps

All commands run from `C:\Dev\Vixen`.

1. Inspect the relevant behavior and test seam:

       rg -n -C 8 "ZoomTime|void Zoom|HandleMarksBarMouseWheel" src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs src/Vixen.Tests/Sequencer/MarksBarMouseWheelTests.cs

2. Make the Milestone 2 and Milestone 3 changes using tabs and LF line endings, then run:

       git diff --check
       git diff -- src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs src/Vixen.Tests/Sequencer/TimelineZoomTests.cs src/Vixen.Tests/Sequencer/MarksBarMouseWheelTests.cs docs/plans/sequencer/vix-4001-ctrl-wheel-zoom-limits.md

3. Run the Milestone 4 commands. A successful focused run includes:

       Passed! - Failed: 0, Passed: <count>, Skipped: 0, Total: <count>

## Validation and Acceptance

The work is accepted when Ctrl-wheel behavior cannot pass either toolbar zoom limit. A Ctrl-wheel zoom-in that would reach 2,000 ticks or fewer makes no state change. A Ctrl-wheel zoom-out beyond the sequence duration gives the same result as toolbar zoom: the entire sequence is visible and its left edge is zero. A valid Ctrl-wheel zoom with pointer focus enabled still changes the zoom and repositions the view around the pointer. Existing Marks Bar wheel behavior remains green.

Automated acceptance requires the focused `TimelineZoomTests` and `MarksBarMouseWheelTests` classes to pass after the x64 MSBuild test build. `TimelineZoomTests` covers the minimum boundary, maximum boundary parity/zero start, and ordinary pointer-focused zoom; Marks Bar tests cover the Ctrl-wheel route to the timeline owner.

## Idempotence and Recovery

The helper and tests are source-only changes and can be rebuilt or rerun repeatedly. If the full MSBuild prerequisite fails because the C++ toolchain is absent, retain existing build outputs, capture the exact error in this plan, and run any already-built focused test command that remains viable. To abandon the work, revert only the three paths named in the diff command after confirming them with `git status --short`; do not reset unrelated changes.

## Artifacts and Notes

Initial source evidence:

    Zoom: rejects scale <= 0; checks VisibleTimeSpan.Scale(scale) against TotalTime;
          rejects candidates with ticks <= 2000; resets start to zero at full sequence.
    ZoomTime: rejects scale <= 0 but directly assigns TimePerPixel.Scale(scale)
              before calculating the pointer-focus offset.
    HandleMarksBarMouseWheel: Ctrl without Alt or Shift reaches ZoomTimelineHorizontally.

Validation evidence:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:q
    Exit code: 0. Existing nullable-reference warnings in unrelated test sources were emitted; no errors occurred.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\" --filter "FullyQualifiedName~TimelineZoomTests|FullyQualifiedName~MarksBarMouseWheelTests"
    Passed! - Failed: 0, Passed: 15, Skipped: 0, Total: 15.

Final validation evidence provided by the user:

    Full Vixen.Tests suite: 939 passed.
    Manual testing: Time + and Time - buttons and Ctrl+mouse-wheel zoom
    enforce identical limits.

## Interfaces and Dependencies

Use only existing .NET WinForms and Vixen timeline types. The new shared routine remains a private `TimelineControl` helper; it must not introduce a public API, setting, or configuration value. `Zoom(double)` and `ZoomTime(double, Point)` keep their existing signatures. The tests use existing xUnit, `Xunit.WinFormsFactAttribute`, `System.Drawing.Point`, and `System.Windows.Forms.Keys` dependencies.

Plan revision note (2026-09-15): Created from the VIX-4001 handoff after inspecting the current toolbar, mouse zoom, and Marks Bar test paths. It deliberately separates shared validity calculation from the two callers' distinct viewport-positioning rules.

Plan revision note (2026-09-15): Completed the implementation and focused validation. The maximum-boundary test follows toolbar parity because the established full-sequence calculation truncates fractional ticks; this preserves the required behavior exactly.

Plan revision note (2026-09-15): Completed the Jira closeout loop. Comment 40485 records the successful x64 test build and focused test result.

Plan revision note (2026-09-15): Reorganized test ownership at the user's direction. Timeline zoom behavior is now tested directly through `TimelineZoomTests`; `MarksBarMouseWheelTests` retains only its narrow event-routing responsibility.

Plan revision note (2026-09-15): Rebuilt the x64 test target after the test reorganization and verified all 15 combined timeline zoom and Marks Bar routing tests pass. The build emitted only existing nullable-reference warnings in unrelated tests.

Plan revision note (2026-09-15): Recorded final user-provided validation evidence. Jira comment 40486 reports all 939 unit tests passing and manual confirmation that button and Ctrl-wheel zoom limits match.

# VIX-3985: Eliminate synchronous waveform repainting while dragging Marks

This ExecPlan is a living document. Maintain its `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections as work proceeds. Follow `.agents/PLANS.md` from the repository root when revising this document.

## Purpose / Big Picture

Dragging or resizing a Mark in the Timed Sequence Editor currently makes the waveform paint synchronously before the mouse-move handler can return. This is especially noticeable in long sequences with dense audio waveforms and during auto-scroll, which deliberately replays the latest mouse position. After this change, the yellow waveform alignment guides will still follow the active Mark and erase their old locations, but Windows will schedule the repaint normally and redraw only the small affected waveform columns.

The immediate scope is Remediation A from `docs/reviews/marks-bar-drag-performance-remediation-plan.md`: deferred, clip-aware waveform painting. VIX-3985 will advance to Remediation B only if the replacement profile shows that live Grid snap-point rebuilding remains the material blocker. It will advance to Remediation C only after B if repeated incremental Grid presentation is still materially responsible for a missed performance target. Neither B nor C is authorized by this plan's implementation milestones.

## Progress

- [x] (2026-08-15 00:00Z) Evaluated `docs/reviews/marks-bar-drag-performance-remediation-plan.md`, `.agents/PLANS.md`, the waveform implementation, timeline coordinate conversion, alignment-event publishers, and nearby WinForms test conventions.
- [x] (2026-08-15 20:08Z) Updated VIX-3985 with the user-facing Remediation A scope, acceptance criteria, validation approach, and evidence-based follow-up gates.
- [x] (2026-08-15 20:11Z) Replaced synchronous alignment repainting with narrow old/new guide invalidation and clip-aware waveform sample drawing; the affected Controls project builds successfully.
- [x] (2026-08-15 20:22Z) Added deterministic waveform alignment rendering tests and verified 25 focused waveform, height-lock, and Marks Bar auto-scroll tests pass.
- [ ] Build, run focused and complete tests, perform the manual drag regressions, and capture a controlled replacement dotTrace profile.
- [ ] Update VIX-3985 with final requirements changes, validation evidence, and an understandable user-facing result; decide whether Remediation B is necessary.

## Surprises & Discoveries

- Observation: The original profile attributes 3,752.2 ms (76.8%) of the 4,886.3 ms sampled `MarksBar.MouseMove_DragMoving` time to synchronous alignment handling, including 3,363.7 ms in `Waveform.OnPaint`.
  Evidence: `docs/reviews/marks-bar-drag-performance-remediation-plan.md` records the dotTrace call-tree measurements.

- Observation: `Waveform.WaveFormSelectedTimeLineGlobalMove` assigns the incoming `IEnumerable<TimeSpan>` directly and calls `Refresh()`. `Refresh()` invalidates and immediately updates a WinForms control, so painting remains in the input call stack.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` lines 67-72.

- Observation: The waveform currently loops over every visible sample regardless of `PaintEventArgs.ClipRectangle`; a narrow invalidation alone would not eliminate the drawing loop.
  Evidence: `Waveform.OnPaint` calculates `start` and `end` only from `VisibleTimeStart` and `VisibleTimeEnd` before its sample loop.

- Observation: Several normal lifecycle paths already request a full repaint through `Invalidate()`, including viewport changes in `TimelineControlBase`, completed sample generation, audio changes, and cursor movement. Those paths must retain complete visible-waveform redraw behavior.
  Evidence: `TimelineControlBase.OnVisibleTimeStartChanged`, `Waveform.FinishedSamples`, `Waveform.SetAudio`, and `Waveform.CursorMoved` each call `Invalidate()`.

- Observation: The Controls project builds successfully after the waveform change, with four existing Vixen.Core warnings and no errors.
  Evidence: `dotnet build src/Vixen.Common/Controls/Controls.csproj -c Release --no-restore` completed with `0 Error(s)`; the warnings are CS8632, CS0618, and CS0067 in Vixen.Core files outside this milestone.

- Observation: The focused test set passes with the new deterministic rendering-bound tests included.
  Evidence: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/" --filter "FullyQualifiedName~WaveformAlignmentRenderingTests|FullyQualifiedName~WaveformLockHeightTests|FullyQualifiedName~MarksBarAutoScrollTests"` reported `Failed: 0, Passed: 25`.

## Decision Log

- Decision: Implement Remediation A independently before changing Grid snap-point ownership or scheduling.
  Rationale: The profile shows the waveform branch is the primary cost. Isolating this change preserves drag, resize, snapping, undo, and VIX-3944 auto-scroll semantics while producing a clear measurement for the next decision.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Preserve `TimeLineGlobalEventManager.AlignmentActivity`, its publishers, and the alignment-guide appearance.
  Rationale: Marks Bar, Ruler, and Grid all use the existing event contract. The issue is synchronous rendering in one subscriber, not event publication.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Use UI-thread invalidation with clip-aware drawing; do not add a painting thread or a bitmap cache in this pass.
  Rationale: WinForms controls and `Graphics` are UI-thread-owned. Narrow invalidation directly removes the synchronous work with less lifecycle and scaling risk than a cache.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Treat the 70% normalized drag-time reduction as an overall program target, not a Remediation A-only promise.
  Rationale: The baseline retains a separate 1,134.1 ms live Grid branch. After A, profile first; promote B only when that residual branch is demonstrably the next limiter.
  Date/Author: 2026-08-15 / Codex and user

- Decision: Expose narrow internal waveform calculation helpers for the existing `Vixen.Tests` friend assembly.
  Rationale: Guide invalidation and clip-to-sample calculations need deterministic boundary tests without publishing a new API or requiring paint-message timing.
  Date/Author: 2026-08-15 / Codex

## Outcomes & Retrospective

VIX-3985 now describes the user-visible Remediation A contract, and the production waveform change plus its deterministic test coverage are complete. `Waveform.cs` materializes alignment times, invalidates the union of the old and new narrow guide areas without forcing a paint, and restricts waveform drawing to the paint clip's bounded sample range. The Controls project builds successfully and the 25-test focused suite passes; end-to-end validation and replacement profiling remain.

## Context and Orientation

`src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` performs the Mark move or resize arithmetic. During live movement it publishes `MarksMoving` and then publishes `AlignmentActivity` with one or two times for the active Mark. Its auto-scroll timer intentionally replays the current mouse position, making any synchronous subscriber work repeat at pointer frequency.

`src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs` owns those per-timeline events. Its `AlignmentActivity` event supplies an `AlignmentEventArgs`, defined in `src/Vixen.Common/Controls/TimeLineControl/AlignmentEventArgs.cs`, containing `Active` and `Times`. Existing callers use `new AlignmentEventArgs(false, null)` to clear guides, so the waveform handler must normalize an inactive or null-times event to an empty set.

`src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` is the WinForms waveform control and the only code changed for runtime behavior in Remediation A. It subscribes to `AlignmentActivity` in its constructor. Today `WaveFormSelectedTimeLineGlobalMove` saves the state and calls `Refresh()`. In WinForms, `Refresh()` synchronously forces painting; `Invalidate()` instead marks an area dirty and returns, allowing Windows to coalesce paint requests. The handler must invalidate guide regions for both old and new alignment times so a moved or cleared guide does not leave a yellow trail.

`TimelineControlBase`, in `src/Vixen.Common/Controls/TimeLineControl/TimelineControlBase.cs`, stores shared timeline state in `TimeInfo`. `timeToPixels(time)` converts a time span to a timeline-pixel float. To draw a guide in the waveform client area, subtract `VisibleTimeStart` first. `Waveform.OnPaint` translates its graphics context left by the timeline-pixel value of `VisibleTimeStart`, so waveform samples themselves use absolute timeline-pixel indexes. The paint clip rectangle is client-relative; the sample loop must translate that range back by `floor(timeToPixels(VisibleTimeStart))`.

`src/Vixen.Tests/Sequencer/TimelineControlTestCollection.cs` serializes WinForms timeline tests. `WaveformLockHeightTests.cs` demonstrates safe waveform construction and the existing reflection style. `src/Vixen.Common/Controls/Controls.csproj` already grants `Vixen.Tests` access to internal members, so deterministic calculation helpers may be `internal` without making a new public API. Because public or protected APIs are not expected to change, the XML-documentation skill is not required unless implementation discovers such a change is necessary.

## Plan of Work

### Milestone 1: Publish the user-facing VIX-3985 contract

Before editing code, use `.agents/skills/jira/SKILL.md` and the configured Jira connection to update VIX-3985. Describe the outcome in non-technical language: Mark drags and resizes should remain responsive with audio waveforms while the yellow alignment guides continue to follow and clear correctly. State that the first delivery changes only waveform repainting, preserves all existing Mark interaction semantics, and will be evaluated before any broader snap-point work begins.

Include these user-reviewable acceptance criteria in the issue: a continuous drag, resize, Alt glued resize, multi-select movement, and auto-scroll continue to work; guide lines do not visibly trail after a move stops; undo remains one completed operation; and a replacement 10-second profile no longer shows `Waveform.OnPaint` synchronously under `MarksBar.MouseMove_DragMoving`. Include the automated, manual, and profiling validation described below. Add a concise progress comment after each completed implementation or validation milestone, phrased as what changed and what users can verify, rather than raw implementation detail.

If Jira cannot be reached, record the access failure in `Surprises & Discoveries`, continue with the local work, and leave both tracker updates pending. Do not invent Jira comments or state changes.

### Milestone 2: Make alignment-guide repainting deferred and bounded

Read all of `Waveform.cs`, `TimelineControlBase.cs`, and `AlignmentEventArgs.cs` immediately before editing. In `Waveform`, replace the nullable, lazily enumerable alignment state with a materialized empty-safe collection. The implementation may use an array or list consistent with existing local style, but it must not enumerate a caller-owned sequence later during paint.

Refactor `WaveFormSelectedTimeLineGlobalMove` so it first materializes the old guide times, then derives new guide times as an empty collection unless both `e.Active` is true and `e.Times` is non-null. Update `_showMarkAlignment` and the stored active collection before invalidating. Do not call `Refresh`, `Update`, `Application.DoEvents`, or `OnPaint` directly.

Add small deterministic helpers in `Waveform` for the coordinate math. One helper must map a single alignment time to a narrow client rectangle: calculate `floor(timeToPixels(alignmentTime - VisibleTimeStart))`, expand the one-pixel guide by two pixels on each horizontal side, use the full `ClientSize.Height`, and intersect with `ClientRectangle`. A second helper must combine the old and new guide rectangles into one invalidation rectangle, ignoring empty intersections. Invalidate that union once when it is non-empty. This invalidates old guide pixels for erasure as well as the current guide pixels for drawing; guides outside the viewport produce no invalidation and no exception.

In `Waveform.OnPaint`, preserve the existing alignment-guide drawing, waveform style, cursor, loading text, and full redraw behavior. Before iterating `samples`, calculate the sample bounds from `e.ClipRectangle`: add the floored absolute timeline pixel of `VisibleTimeStart` to the clip's left and right edges, include a one-pixel margin on either side, clamp the start to zero, clamp the exclusive end to both `samples.Count` and the audio media-duration pixel, and iterate only that half-open range. The normal full client clip must still cover the existing visible range. Retain `base.OnPaint(e)` and do not move graphics or control state to a worker thread.

At the end of this milestone, inspect the diff. The only production runtime file expected to change is `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`; do not modify `MarksBar`, Grid, event contracts, undo actions, or auto-scroll behavior.

### Milestone 3: Add deterministic rendering-bound tests

Create `src/Vixen.Tests/Sequencer/WaveformAlignmentRenderingTests.cs` and apply `[Collection(TimelineControlTestCollection.Name)]`. Build a waveform with a controlled `TimeInfo`, size, visible start, and time-per-pixel scale, as in `WaveformLockHeightTests`. Prefer direct calls to narrow internal calculation helpers. If implementation instead keeps helpers private, use the nearby established reflection pattern; do not make production state public merely to test it.

Test an alignment guide rectangle at the left edge, centre, right edge, and outside the viewport. Assert that each returned rectangle is within `ClientRectangle`; outside locations must be empty. Test a move from one visible time to another by combining prior and current times, and assert that the union covers both guide locations. Test inactive events and active events with null `Times` normalize to an empty guide collection and invalidate the old guide location without throwing.

Test the sample-range helper using a partial clip at the left edge, a partial clip at the right edge, an out-of-viewport clip, and a full client clip. Assert `0 <= start <= endExclusive <= samples.Count`, the end is capped by the media-duration pixel, and the full clip matches the former visible sample range. Keep tests deterministic: they must not rely on paint-message timing, elapsed milliseconds, or a real audio playback session.

The milestone is complete when the new tests fail against the old synchronous/full-loop behavior where directly testable and pass after the implementation, while `WaveformLockHeightTests` and `MarksBarAutoScrollTests` remain green.

### Milestone 4: Validate behavior, profile, and choose the next remediation

From `C:\Dev\Vixen`, build the test target with full Visual Studio MSBuild because the test graph includes C++/CLI projects. Then run the focused test set and the complete test project without rebuilding:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/" --filter "FullyQualifiedName~WaveformAlignmentRenderingTests|FullyQualifiedName~WaveformLockHeightTests|FullyQualifiedName~MarksBarAutoScrollTests"
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/"

Expect MSBuild and both test commands to report zero failures. If an unrelated existing failure or missing C++ toolchain prevents a command, preserve the command and its complete error in this plan, run the remaining viable checks, and do not change dependencies simply to make the command pass.

Manually open a sequence containing audio, many Marks, visible Mark grid lines, and a timeline wider than the viewport. Drag one Mark continuously for 10 seconds inside the viewport, then repeat while auto-scrolling left and right. Resize each Mark edge, perform an Alt glued resize, move multiple selected Marks, release, and undo each operation. Confirm the waveform guides, Marks Bar, ruler, and effect-grid guides keep up; no old yellow guide remains after normal repaint; Mark constraints and snapping retain their prior behavior; and undo restores the original times.

Capture a dotTrace snapshot using the same sequence, zoom, visible range, approximate pointer motion, and 10-second duration as the 4,886.3 ms baseline. The A-specific acceptance condition is that `Waveform.WaveFormSelectedTimeLineGlobalMove` no longer has `Waveform.OnPaint` synchronously beneath `MarksBar.MouseMove_DragMoving`. Record the normalized `MouseMove_DragMoving`, alignment branch, waveform paint, and live Grid branch times.

Choose the follow-up explicitly from the evidence:

- Close the Remediation A delivery if input is visibly responsive, guide repainting is correct, all validation passes, and the replacement profile shows no material synchronous waveform paint beneath the mouse-move stack. Document any remaining cost as measured follow-up context.
- Create or activate a Remediation B plan only if the controlled profile still misses the agreed responsiveness/performance boundary and identifies live `Grid.CreateSnapPointsFromMarks` or equivalent full snap-point rebuilding under `MarksMoving` as a material remaining cost. B must use a separate, approved ExecPlan that owns incremental Grid registrations; do not add it opportunistically here.
- Consider Remediation C only after B has been implemented and profiled. It requires evidence that repeated incremental Grid updates—not waveform painting or another child path—still materially limit drag responsiveness. Its plan must define timer lifecycle, deterministic flush/discard rules, and a separate validation pass.

Finally, make any needed VIX-3985 description adjustment so it matches the delivered scope, then add a concise comment with the build result, focused and full test results, manual checks, profile comparison, and the B/C decision. Update this plan's Progress, Surprises & Discoveries, Decision Log, Outcomes & Retrospective, and revision note with the same evidence. When a milestone changes repository files, generate a commit message using `.agents/skills/commit-msg/SKILL.md`, but do not create a commit unless the user explicitly asks.

## Concrete Steps

All commands run from `C:\Dev\Vixen`.

1. Before editing, inspect the current event and paint paths:

       rg -n -C 6 "WaveFormSelectedTimeLineGlobalMove|OnPaint|AlignmentActivity|timeToPixels|VisibleTimeStart" src/Vixen.Common/Controls/TimeLineControl -g "*.cs"

2. Perform the Milestone 2 and 3 edits using tabs and LF line endings, then review only the intentional files:

       git diff --check
       git diff -- src/Vixen.Common/Controls/TimeLineControl/Waveform.cs src/Vixen.Tests/Sequencer/WaveformAlignmentRenderingTests.cs docs/plans/sequencer/vix-3985-marks-bar-drag-performance-remediation-a.md

3. Run the Milestone 4 commands. A passing test run ends with a result similar to:

       Passed!  - Failed:     0, Passed:     <count>, Skipped:     0

4. Record actual test counts, profile values, the manual sequence used, the decision about B/C, and the VIX-3985 update in this plan.

## Validation and Acceptance

The Remediation A delivery is accepted when dragging, resizing, glued resizing, multi-select movement, auto-scroll, snapping, and undo continue to behave as before, while waveform alignment lines follow the operation without visual trails. A null or inactive alignment event clears existing guides without an exception. A partial invalidation redraws only bounded waveform sample columns, and ordinary full-control invalidations retain full visible waveform rendering.

Automated acceptance requires the new waveform rendering tests, existing waveform height tests, Marks Bar auto-scroll tests, and the complete `Vixen.Tests` suite to pass after the x64 MSBuild test build. Profiling acceptance requires a controlled replacement profile to prove that `Waveform.OnPaint` is no longer synchronous beneath the Marks Bar mouse-move path. This plan does not declare the overall 70% drag-subtree reduction achieved until the replacement profile establishes whether B is necessary.

## Idempotence and Recovery

The invalidation calculations are pure with respect to the current control state and may be exercised repeatedly. WinForms can safely coalesce repeated `Invalidate(Rectangle)` calls; do not force a repaint to make tests or manual checks appear immediate. If guides leave trails, first inspect the old/new rectangle union and client-versus-timeline coordinate conversion, then expand the narrow safety margin if evidence shows a one-pixel rounding artifact. Do not restore `Refresh()` as a workaround.

If a partial paint shows missing waveform columns, verify clip conversion, the one-pixel sample margin, and media-duration/sample-count clamping. Full-control invalidation remains a safe correctness fallback while those calculations are fixed. Do not delete generated build outputs or alter package/project references when build tooling is unavailable; record the failure and retry in an environment with the required Visual Studio C++ toolset.

## Artifacts and Notes

Baseline profile evidence from the approved remediation review:

    MarksBar.MouseMove_DragMoving: 4,886.3 ms sampled work
      AlignmentActivity path: 3,752.2 ms (76.8%)
        Waveform.OnPaint: 3,363.7 ms
      MarksMoving path: 1,134.1 ms (23.2%)
        Grid.CreateSnapPointsFromMarks: 1,116.6 ms

The required guide-coordinate calculation is:

    clientX = floor(timeToPixels(alignmentTime - VisibleTimeStart))
    guideRectangle = ClientRectangle intersect Rectangle(clientX - 2, 0, 5, ClientSize.Height)

The required clip-to-sample calculation is:

    visibleStartPixel = floor(timeToPixels(VisibleTimeStart))
    start = max(0, visibleStartPixel + clip.Left - 1)
    endExclusive = min(samples.Count, mediaDurationPixel, visibleStartPixel + clip.Right + 1)

`endExclusive` is exclusive: the loop draws `x` while `x < endExclusive`. The margin prevents an edge column from disappearing when a guide's narrow rectangle is rounded or clipped.

## Interfaces and Dependencies

Use existing .NET WinForms painting types only: `PaintEventArgs`, `Rectangle`, `Graphics`, and `Control.Invalidate(Rectangle)`. Retain existing `TimeLineGlobalEventManager`, `AlignmentEventArgs`, `TimeInfo`, `Sample`, `Audio`, and `TimelineControlBase` contracts. Do not add packages, public APIs, background painting, or changes to the Marks model.

The final local implementation should have testable calculation seams equivalent to these internal members; exact names may follow repository conventions:

    internal Rectangle GetAlignmentInvalidationRectangle(TimeSpan alignmentTime)
    internal Rectangle GetAlignmentInvalidationRectangle(IEnumerable<TimeSpan> previousTimes, IEnumerable<TimeSpan> currentTimes)
    internal (int Start, int EndExclusive) GetVisibleSampleRange(Rectangle clipRectangle)

The waveform's stored alignment times must always be a non-null materialized collection. `WaveFormSelectedTimeLineGlobalMove` must retain the existing private event-handler shape:

    private void WaveFormSelectedTimeLineGlobalMove(object sender, AlignmentEventArgs e)

Plan revision note (2026-08-15): Initial VIX-3985 ExecPlan created after evaluating the remediation review against `.agents/PLANS.md` and the current waveform code. It intentionally limits the first implementation to Remediation A, adds the required initial/final Jira milestones and user-facing updates, and makes B/C promotion dependent on a replacement profile.

Plan revision note (2026-08-15): Completed Milestone 1 by updating VIX-3985 with a concise user-facing Summary, Scope, Acceptance Criteria, validation approach, and the profile-driven follow-up rule. No code or test changes were made.

Plan revision note (2026-08-15): Completed Milestone 2. `Waveform.cs` no longer calls `Refresh()` for alignment activity; it invalidates one bounded union of previous and current guide regions and honors `PaintEventArgs.ClipRectangle` when drawing samples. Added documented internal calculation seams for Milestone 3 tests. The Controls Release build succeeded with four pre-existing Vixen.Core warnings and no errors. Added a user-facing VIX-3985 progress comment describing the completed first pass and next validation step.

Plan revision note (2026-08-15): Completed Milestone 3. Added `WaveformAlignmentRenderingTests` for guide clipping, prior/current guide coverage, inactive null-time clearing, and left/right/outside/full clip sample ranges. A full x64 test-target build succeeded, and the focused waveform, height-lock, and Marks Bar auto-scroll run passed all 25 tests without timing-sensitive assertions. Added a user-facing VIX-3985 validation comment describing the focused automated coverage and remaining end-to-end checks.

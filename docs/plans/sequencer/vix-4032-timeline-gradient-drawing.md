# Keep the Grinch timeline drawing near the Showstopper mini-tree effects

This ExecPlan follows `.agents/PLANS.md`. Keep `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` current as work proceeds.

## Purpose / Big Picture

Users should be able to expand the EFL Showstopper mini-tree rows in the Grinch sequence and drag across the 52-second region without a grid drawing error. The next paint should still show the other effect thumbnails, and normal thumbnails should retain their existing colors and gradients. Reproduce the report with playback stopped first, then verify the fix during both drag and playback. [VIX-4032](https://vixenlights.atlassian.net/browse/VIX-4032)

## Progress

- [x] (2026-11-19 00:50Z) Reviewed the Jira description, comments, screenshot, logs, and Grinch profile. Traced the failing timeline drawing path and reproduced the same HRESULT in a standalone Windows brush probe.
- [x] (2026-11-19 00:50Z) Drafted a Rider execution plan with a baseline debugger capture and a bitmap regression.
- [x] (2026-11-19 01:23Z) Recorded the acceptance criteria and reproduction test plan in Jira, preserving the original report.
- [x] Implemented the gradient brush repair and bounded dense-effect thumbnail sampling. The Release x64 test target built and the full suite passed 1,220 tests, including the dense thumbnail bitmap test.
- [x] Added captured-gradient bitmap regression coverage: the original rectangle brush reproduces the HRESULT, the repaired brush accepts the captured bounds and ARGB colors, and ordinary gradients match the original pixels. The focused four tests and full suite of 1,224 tests passed on Windows Release x64.
- [x] (2026-11-19 01:07Z) Captured a thin Butterfly segment in the live Grinch profile and stepped over its brush constructor successfully.
- [x] (2026-11-19 01:10Z) Captured the actual Butterfly throw in Rider and reproduced it with the exact bounds and colors in a standalone Windows process.
- [x] (2026-11-19 01:11Z) Verified that a two-point horizontal brush accepts the captured inputs and compared ordinary rendered pixels with the original constructor.
- [x] User reports manual testing confirms the original error no longer occurs, scrolling performance is restored with Butterfly visible, and rendered images remain representative. Playback, zoom, row expansion, and log-check variants were not specified.
- [x] Reconciled the Jira acceptance criteria with the performance fix and posted the observed manual and automated results, including the remaining coverage gap.

## Surprises & Discoveries

- The reporter can trigger the dialog by dragging the timeline with playback stopped. The report describes a catch near 52 seconds with the trees collapsed and the dialog with the rows expanded. The supplied stack and the attached DevBuild logs both fail at `LinearGradientBrush` construction from the `RGBValue` static-array branch. The screenshot shows the generic grid drawing dialog. The profile also contains separate `NullReferenceException` entries, whose stacks should be distinguished from this brush failure.
- The attached `Vixen 3111826.zip` contains `Vixen 3/Sequence/Grinch.tim` and `Vixen 3/SystemData/SystemConfig.xml`. The sequence lasts 113.084 seconds and saves 10.5441 milliseconds per pixel. The serialized data shows Butterfly effects starting near 52.71 seconds on all six Showstopper mini trees. This narrows the inspection area. The attachment logs do not identify the runtime effect or rectangle at the throw.
- Current code can submit a positive fractional rectangle. `EffectRasterizer.Rasterize` clips an intent in time and computes a proportional pixel width. A clipped head or tail can be very short. The static-array path splits the result again at roughly 50 ms per color segment. `IntentRasterizer.Rasterize` checks exact zero at its entry point, before that split. The per-segment brush expands by `rectangle.X * 0.004F` on each side, so a segment at bitmap X = 0 receives no padding.
- A single `LinearGradientBrush` allocation on Windows reproduced `ExternalException (0x8000FFFF)` for `(0, 0, 1e-8f, 2)` and `(0, 0, 4, 1e-8f)`. A zero-width rectangle threw `ArgumentException`. The same probe succeeded with width `0.05f` and with one tick mapped at the sequence's saved zoom. Filling each tiny rectangle with a `SolidBrush` completed successfully. This establishes a native failure mode and leaves the actual Grinch rectangle to be measured.
- Rider confirms that `Vixen.Tests` references `TimedSequenceEditor` and xUnit v3. Rider's `findTests` coverage launch failed. The graph identified `src/Vixen.Tests/Sequencer/TimelineGridDrawingTests.cs` as neighboring grid coverage and found no test linked directly to `DrawGradient`.

- Rider paused before the constructor on the second chunk of a Butterfly `StaticArrayIntent<RGBValue>` for a Showstopper mini-tree pixel. The 19-pixel preview height was divided by `14945 / 35` sampled target rows using integer division, then by 22 parallel intent lanes. This produced a height of `0.00202256767f` and a padded rectangle of `(4.69899321f, 0f, 4.7556076f, 0.00202256767f)`. Rider stepped to line 62 with a live `LinearGradientBrush`, and the standalone probe accepted the same inputs. A later break on throw captured the failing segment at a different pixel target of the Butterfly effect.
- A standalone Windows probe also tested `LinearGradientBrush(PointF, PointF, Color, Color)` with horizontal endpoints derived from the existing padded left and right edges. The overload accepted a height of `1e-8f` at a normal width, and it produced identical pixels for the sampled valid rectangles. Both overloads failed at width `1e-8f`.

- At the actual first-chance throw, Rider showed `Status.OutOfMemory` from the rectangle brush constructor and `ExternalException` with HRESULT `0x8000FFFF`. Butterfly rendered 400 intent entries for a target list containing 43,396 elements. `EffectRasterizer` divided a 19-pixel bitmap height over `43396 / 40` sampled rows using integer division, then divided that row by 215 overlapping intent lanes. Lane index 62 for Showstopper Mini Tree-1 1 Px 104 - 5 passed a strip of height `8.1524071e-5f`. The brush failed on chunk 64 of 67. The original fill rectangle was `(297.225464f, 7.57701015f, 4.71786451f, 8.1524071e-5f)`. The padded brush rectangle was `(296.03656f, 7.57701015f, 7.09566832f, 8.1524071e-5f)`. The endpoint colors were `0x5B0505FE` and `0x580505FF` in ARGB order.
- One standalone allocation with those exact values reproduced the HRESULT. Doubling only the brush height succeeded. Two horizontal `PointF` endpoints at Y = 0 with the original padded X bounds also succeeded and painted within the original fill bounds. For three valid nonempty comparison images with varied positions and aspect ratios, the two constructors produced identical pixels. The failed and successful live debugger stops both had 256 live GDI objects and 262 peak. The exact-input standalone failure establishes a geometry trigger for this incident.

## Decision Log

- **Decision** — Construct the horizontal gradient using two `PointF` endpoints at Y = 0 and preserve the padded X bounds. Use a solid fallback only when the horizontal gradient interval is unusable. **Rationale** — The actual throw reproduced with a valid 7.0957-pixel gradient width and an extremely thin height. The two-point constructor accepts the same X interval and preserves color interpolation on the tested visible rectangles. The previous minimum height proposal is superseded. The upstream grouping remains useful context for the thin strips and can be examined independently if needed. **Date and author** — 2026-11-19 01:11Z, Codex.
- **Decision** — Confirm the failing inputs before selecting the production change. **Rationale** — A thin brush reproduces the same exception, but the original capture contains no geometry and the minimum positive overlap at the saved zoom succeeds in the standalone probe. **Date and author** — 2026-11-19, Codex.
- **Decision** — For confirmed brush degeneracy, guard in `IntentRasterizer.DrawGradient`, reuse its solid brush for a drawable rectangle with a tiny or invalid gradient domain, and preserve the existing fill bounds. **Rationale** — This is the common boundary for ordinary and sampled lighting intent painting. It can handle one bad segment and continue drawing later segments. It also preserves the padding documented in the existing code for usable gradients. **Date and author** — 2026-11-19, Codex.
- **Decision** — Use `1f / 256f` as the minimum horizontal brush span in the effect bitmap's coordinates. **Rationale** — Both native constructors rejected a positive `1e-8f` horizontal interval. The fallback keeps that degeneracy out of GDI+ and the cutoff is much smaller than a destination pixel. A valid positive height participates only in the original fill bounds. **Date and author** — 2026-11-19 01:11Z, Codex.

## Outcomes & Retrospective

The live Grinch throw and standalone replay establish that the legacy rectangle brush fails on the narrow-height Butterfly lane. A standalone comparison verified the two-point design and preserved visible gradient pixels. The production repair also bounds dense-effect thumbnail sampling, and the user reports that the original error and scrolling delay are resolved while the images remain representative. The exact playback, zoom, row expansion, and log-check variants were not specified.

The Release x64 test target built successfully and the full suite passed 1,224 tests. The effect rasterizer bitmap test covers sparse targets and many overlapping intents. The intent rasterizer bitmap tests reproduce the original rectangle-brush failure with the captured geometry and ARGB colors, verify the repaired brush accepts those inputs, compare ordinary gradient pixels, and check subsequent drawing after tiny and invalid bounds. Because RGBValue(Color) does not retain alpha, the exact captured colors are passed directly to the private gradient boundary after initializing the rasterizer through its normal entry point; that entry point also receives the captured geometry with opaque distinct colors. Jira's acceptance criteria and earlier verification comment reflect the performance fix and manual results; the comment predates this regression test.

## Context and Orientation

`src/Vixen.Common/Controls/TimeLineControl/Grid.cs`, in `DrawElement` and `OnPaint`, draws visible sequence elements and reports paint failures. `src/Vixen.Common/Controls/TimeLineControl/Element.cs`, in `DrawV2` and `DrawImage`, requests a bitmap for the visible portion of each effect. `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs`, in `DrawCanvasContent`, delegates that bitmap's rendering to `EffectRasterizer`.

`src/Vixen.Modules/Editor/TimedSequenceEditor/EffectRasterizer.cs`, in `Rasterize`, converts the visible time interval into bitmap coordinates for each intent. An intent is an effect's output for one target element over a time interval. For the ordinary clipped branch, an intent that starts before the visible interval uses bitmap X = 0. The width is `overallWidth * intentDurationTicks / effectDurationTicks * visibleIntentDuration / intentDuration`. The base-layer branch can also pass X = 0 with the clip width. Keep both origins in mind in the debugger.

`src/Vixen.Modules/Editor/TimedSequenceEditor/IntentRasterizer.cs`, in `Handle(IIntent<RGBValue>)`, forwards sampled RGB intents to `DrawStaticArrayIntent`. That method divides the rectangle into equal-width chunks and calls `DrawGradient` for each pair of sampled colors. `DrawGradient` uses a solid fill for equal colors or a gamma-corrected horizontal gradient for different colors. The gradient brush rectangle describes the color transition. The original rectangle controls the pixels filled. These two rectangles have different roles.

## Architecture Design for VIX-4032

Rider is the detected IDE. Use its debugger, file problem inspection, and test runner where available. Apply the implementation as a single block in `IntentRasterizer.DrawGradient` with Rider's Apply snippet from chat flow when working interactively, or use the repository's Gortex edit workflow when an agent performs the edit.

Validate the original fill rectangle before drawing. Require finite `X`, `Y`, `Width`, `Height`, `Right`, and `Bottom`, positive dimensions, and distinct left/right and top/bottom edges at `float` precision. Return for an unusable fill rectangle so the caller can move to the next segment. For drawable input, retain the equal-color solid path.

For different colors, compute the existing `padding = rectangle.X * 0.004f`, `gradientLeft = rectangle.X - padding`, and `gradientRight = gradientLeft + (rectangle.Width + 2f * padding)` in single precision. Require finite endpoints and a finite `gradientRight - gradientLeft` of at least `1f / 256f`. Use the existing solid brush with the unmodified `startColor` for an unusable horizontal interval. Otherwise construct `LinearGradientBrush(new PointF(gradientLeft, 0f), new PointF(gradientRight, 0f), startColor, endColor)`, set `GammaCorrection = true`, and dispose it via `using`. Both branches pass the original rectangle to `FillRectangle`. The fill may have a small positive height because the horizontal brush constructor now depends only on the padded X interval.

The editor owns this rendering policy. The grid supplies the visible bitmap, the effect rasterizer supplies the clip rectangle and intent times, and the intent rasterizer selects the brush. The proposed production patch fits in the intent rasterizer. There are no new serialized fields, settings, public interfaces, or dependencies. Existing color sampling, timing, target expansion, playback logic, and error reporting remain as currently defined.

## Plan of Work

### Milestone 1 — Capture the original failure and record the requirements

In Jira, preserve the report and add the finalized user-visible acceptance criteria and reproduction test plan. Record that Grinch near 52 seconds can be scrubbed with the Showstopper tree rows expanded and playback stopped, that the same section can play without the grid dialog, and that thumbnails remain visible. Keep the test instructions focused on the reporter's actions and expected results.

In Rider, load an isolated copy of the attached Vixen profile. Keep hardware outputs disabled and leave the shared/default profile alone. Open `Grinch.tim`, use the saved zoom as the starting point, expand the six mini-tree rows, and drag or scroll horizontally through 50–58 seconds. Record the viewport start, zoom, row, operation, and exact first thrown exception. In Rider, enable a .NET exception breakpoint for `System.Runtime.InteropServices.ExternalException` and set it to break on throw. Confirm the HRESULT is `0x8000FFFF` and that the stack contains `IntentRasterizer.DrawGradient`. Select the `DrawGradient` caller frame before unwinding. A stop at its constructor line is useful for geometry sampling. Step over the call to verify whether it throws before treating a sampled rectangle as the fault. Capture the original and padded rectangle with round-trip float precision, both colors, chunk index and count, `_startOffset`, `_endTime`, effect name and ID, intent start and duration, and `overallWidth` from the caller. Record whether the failure reproduces on the first pass after launch, and copy the exact rectangle into the standalone probe. Compare the behavior with rows collapsed and after another redraw.

The baseline debugger capture and standalone reproduction in `Surprises & Discoveries` satisfy the geometry check. Carry those measured inputs into Milestone 2. Use the same expanded rows and horizontal drag as the patched application acceptance check. Keep pre-existing null-reference stacks separate when evaluating logs.

Run `git status --short` and `git diff` for any milestone files. For a repository change, use the `commit-msg` skill to draft a `VIX-4032` message and present it in its `Commit message` block. STOP HERE for manual review and commit execution before proceeding.

### Milestone 2 — Fix the confirmed brush failure and prove the regression

Update the `DrawGradient` block in `src/Vixen.Modules/Editor/TimedSequenceEditor/IntentRasterizer.cs` to construct the two-point brush and apply the geometry checks above. If the fill check needs a helper, keep it private in the class. Place the invalid fill guard before the equal-color fast path. Compute padding only for different colors, then select the existing solid fill or the scoped two-point brush fill. Keep the callers and exception handling in `Grid.OnPaint` as they currently are. Run Gortex impact analysis before editing and detect, tests, guards, and contract after editing. Run Rider `get_file_problems` on the changed C# files and address findings on the edited lines.

Add `src/Vixen.Tests/Sequencer/IntentRasterizerDrawingTests.cs` using xUnit and a Windows `Bitmap` with `Graphics.FromImage`. Drive the existing `Rasterize` entry point with a `StaticArrayIntent<RGBValue>` that supplies distinct endpoint colors, so the tests cover the production sampling and drawing path. Read the intent constructor and existing tests for their data setup. Use the existing editor test visibility convention. If the editor DLL does not already grant access to `Vixen.Tests`, add a test-only `InternalsVisibleTo` declaration using the project's existing assembly metadata pattern. A project reference to `TimedSequenceEditor` already exists. Resource-owning test locals should use `using`.

Use the captured original fill rectangle and ARGB colors in a 320 × 24 bitmap. Drive a single static-array chunk with a visible interval shorter than 50 ms and put the two distinct samples within the intent's duration, so both endpoint lookups are valid. A single chunk recreates the captured brush geometry after padding. Run this case on the baseline and verify `ExternalException (0x8000FFFF)`. On the patch, verify that the call completes and that a subsequent ordinary gradient paints in the same bitmap. Include a tiny-width case at X = 0 and an invalid fill case. Compare a valid fractional-width case and a normal gradient with measured visible pixels against the existing constructor's output with identical bounds, padding, alpha, and gamma correction. Include an equal-color fill. Run the focused and full tests and repeat the original Rider profile steps on the fixed build.

Run `git status --short` and the targeted `git diff`. Use the `commit-msg` skill to draft a `VIX-4032` message and present it in its `Commit message` block. STOP HERE for manual review and commit execution before proceeding.

### Milestone 3 — Close the verification loop in Jira

Reconcile the Jira description with the final acceptance and test cases. Add a comment with the verified profile, actions, observed result, and automated test result. State any remaining reproduction gaps. Record the same evidence in `Outcomes & Retrospective` and mark the corresponding progress entries complete.

Run `git status --short` and `git diff` for any milestone files. For a repository change, use the `commit-msg` skill to draft a `VIX-4032` message and present it in its `Commit message` block. STOP HERE for manual review and commit execution before proceeding.

## Concrete Steps

Run from `C:\Dev\Vixen` in PowerShell with Visual Studio full MSBuild and the x64 C++/CLI toolset installed. The first command builds the test assembly and its native dependencies. Build the full release solution for the patched Rider app acceptance run if the test target's outputs are insufficient to launch the complete application.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 "-p:SolutionDir=$($PWD.Path)/" --filter "FullyQualifiedName~IntentRasterizerDrawingTests"
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 "-p:SolutionDir=$($PWD.Path)/"
    msbuild Vixen.sln -m -restore -t:Build -p:Configuration=Release -p:Platform=x64 -v:m

Expect the focused suite to report zero failing tests after the fix, including the new case that failed on the baseline. The full suite should pass. Record actual counts and any baseline failures. To run through Rider, select the `IntentRasterizerDrawingTests` class in Unit Tests for the focused run and use Rider's file problem inspection for the modified source and test files. Use the built `Release\Output\Vixen.Application.exe` with the isolated profile for the manual check or the equivalent Rider launch configuration that targets the built app.

## Validation and Acceptance

With the mini-tree rows expanded, drag or scroll through 50–58 seconds in Grinch with playback stopped. The effects around 52 seconds and the other visible thumbnails should continue painting and the grid dialog should never appear. Repeat during playback, after collapsing and re-expanding, and at a coarser zoom that clips visible effects. Inspect new log entries for `ExternalException (0x8000FFFF)` from `IntentRasterizer.DrawGradient`.

The focused bitmap regression must fail on the original drawing path with the captured Butterfly bounds and colors and pass after the fix. When invalid inputs draw nothing, later valid draws in the same bitmap must still succeed. Normal gradients must show both endpoint color directions and equal-color fills must look uniform. Compare the area near 52.71 seconds to visible effects outside the failing region to check that the thumbnail art is intact.

## Idempotence and Recovery

Repeat the debugger capture and test commands against an isolated copy of the archived profile. Inspect the zip before extracting and use a directory outside the repository for profile data and logs. Keep the archive, source profile, and standard Rider profile intact. When a baseline build or test launch fails, verify that x64 full MSBuild completed before retrying the no-build test command. If new evidence changes the diagnosis, update this plan and keep the baseline evidence before selecting a new change.

## Artifacts and Notes

The attached screenshot is `vixen error.JPG` and the log archive is `vixen error logs.zip`. The Grinch sequence is in `Vixen 3111826.zip`. The ZIP and bitmap probes were inspected locally during planning. The following observed results came from a Windows PowerShell 7.7 `System.Drawing.Common` probe using different endpoint colors.

    (0, 0, 1e-8, 2)    LinearGradientBrush -> ExternalException (0x8000FFFF)
    (0, 0, 4, 1e-8)    LinearGradientBrush -> ExternalException (0x8000FFFF)
    (0, 0, 0.05, 2)    LinearGradientBrush -> success
    Same tiny bounds   SolidBrush fill -> success
    Captured Butterfly bounds and colors  RectangleF brush -> ExternalException (0x8000FFFF)
    Same brush bounds with doubled height RectangleF brush -> success
    Same padded X bounds with both Y = 0  PointF brush and original fill -> success
    Three valid comparison bitmaps        RectangleF vs PointF brush -> 0 differing pixels

## Interfaces and Dependencies

`IntentRasterizer.DrawGradient(Color startColor, Color endColor, RectangleF rectangle)` remains private. `IntentRasterizer.Rasterize(IIntent intent, RectangleF rect, Graphics g, TimeSpan startOffset, TimeSpan endTime)` remains the rendering entry point used by `EffectRasterizer` and the proposed bitmap test. Use the existing Windows `System.Drawing` types, the existing test project dependency on the editor, and the repository's xUnit v3 test runner. [Microsoft's LinearGradientBrush documentation](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.drawing2d.lineargradientbrush?view=windowsdesktop-10.0)

Analysis complete and plan integrated with plans.md.

Revision on 2026-11-19 records the actual Rider throw, the exact standalone reproduction, and why the two-point horizontal brush supersedes the minimum-height proposal. Later verification updates record the user-reported manual results, dense-effect sampling fix, and captured-gradient regression coverage.

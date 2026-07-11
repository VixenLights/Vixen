# Harden Timeline Grid GDI Drawing Failures (VIX-3705)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. This document is self-contained so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users have reported a rare Timed Sequence Editor paint failure where the timeline grid logs `Unable to draw element image` from `Grid.DrawElement`, then logs `Exception in TimelineGrid.OnPaint()` from `_drawCursors`. The stack traces point into native GDI+ drawing and include `System.Runtime.InteropServices.SEHException` and `System.InvalidOperationException: Object is currently in use elsewhere`. After this work, drawing the timeline should be more robust against rare image-cache races and GDI handle pressure, and any remaining failures should be easier to diagnose without trapping the user in repeated modal paint errors.

The visible result is in the Timed Sequence Editor. Opening a sequence, scrolling the grid, selecting effects, resizing effects, and letting background rendering complete should still draw effects, cursors, snap lines, selection boxes, and resize indicators as before. The internal result is that cached effect bitmaps have clear ownership and synchronization, temporary placeholder bitmaps are disposed, and paint failures do not cascade into misleading later drawing calls.

## Progress

- [x] (2026-07-11) Investigated the current `Grid.DrawElement`, `_drawCursors`, `OnPaint`, background rendering, and `Element` image-cache code paths after a reported VIX-3705 stack trace.
- [x] (2026-07-11) Created this initial ExecPlan for VIX-3705 with incremental milestones and narrow validation points.
- [x] (2026-07-11) Milestone 1: Established focused test coverage in `src/Vixen.Tests/Sequencer/TimelineGridDrawingTests.cs` using a test-only `Element` subclass and a mocked `IEffectModuleInstance`; no production test seam or public API change was needed.
- [x] (2026-07-11) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore`; 4 tests passed. The final run emitted pre-existing LiteDB/package warnings only. An earlier passing run also emitted transient copy retry warnings for `SharpFont.dll`/`QuickFont.dll` locked by .NET Host and Microsoft Defender, but completed successfully.
- [x] (2026-07-11) Milestone 2: Added ownership-aware drawing through `Element.DrawV2(...)`, marked the old `Element.Draw(...)` API obsolete, and updated `Grid.DrawElement(...)` to dispose only images marked `DisposeAfterDraw`, which covers temporary placeholders while leaving cached rendered images owned by `Element`.
- [x] (2026-07-11) Added `TimelineGridDrawing` coverage for the new ownership flags and obsolete-wrapper delegation, then reran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore`; 7 tests passed. The run emitted pre-existing package/compiler warnings only.
- [x] (2026-07-11) Milestone 3: Added a lazily initialized nonserialized cached-image lock to `Element`, synchronized cached image creation/replacement/disposal on that lock, and updated `Grid.DrawElement(...)` to hold the same lock before calling `DrawV2(...)` and while drawing the returned image.
- [x] (2026-07-11) Added `TimelineGridDrawing` coverage proving selection-driven cache invalidation waits while the draw lock is held, then reran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore`; 8 tests passed. The run emitted pre-existing package/compiler warnings only.
- [x] (2026-07-11) Milestone 4: Changed `Grid.DrawElement(...)` and `_drawElements(...)` to report draw success, abort the current paint pass after an element image draw failure, add element geometry/ownership context to the failure log, and throttle the generic `OnPaint` modal dialog until a later paint pass succeeds.
- [x] (2026-07-11) Reran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore`; 8 tests passed. The run emitted pre-existing LiteDB/package/compiler warnings only.
- [x] (2026-07-11) Manual Timed Sequence Editor testing after Milestone 4 showed no visible drawing issues.
- [ ] Milestone 5: Run focused and full validation, then manually exercise the Timed Sequence Editor.
- [ ] Milestone 6: Update Jira issue `VIX-3705` with the final implementation notes, validation results, and any residual risk.

## Surprises & Discoveries

- Observation: The reported `_drawCursors` stack frame is probably not the root failure in current code.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` `_drawCursors(Graphics g)` only creates a local cursor pen, draws a vertical cursor line, and optionally draws playback time with `Pens.Green`. There is no image access in that method. The preceding log entry from `DrawElement` failed inside `GdipDrawImageRectI`, which is a stronger pointer to image or graphics state corruption earlier in the same `OnPaint`.

- Observation: `Element._cachedImage` can be disposed without taking the same lock used by `Grid.DrawElement`.
  Evidence: `Grid.DrawElement` locks `elementImage` only around `g.DrawImage(elementImage, destRect)`. `Element.Selected` and `Element.RenderElement()` dispose `_cachedImage` directly, and rendering can be initiated from background worker and task paths in `Grid`.

- Observation: Placeholder images are temporary but are not disposed by `Grid.DrawElement`.
  Evidence: `Element.Draw(Size, Graphics, TimeSpan, TimeSpan, int, bool)` returns `DrawPlaceholder(...)` when `IsRendered` is false. `DrawPlaceholder` creates a new `Bitmap` every call. `Grid.DrawElement` draws the returned bitmap but does not dispose it, because the same return path also handles cached images that must not be disposed by the grid.

- Observation: Background rendering mutates element render state and image cache state off the UI thread.
  Evidence: `Grid.renderWorker_DoWork` calls `element.RenderElement()` inside `Parallel.ForEach`, and `Grid.RenderElement(Element element)` can call `Task.Factory.StartNew(() => { element.RenderElement(); Invalidate(); })`.

- Observation: Direct unit coverage for `Element.Draw(...)` is feasible without launching the Timed Sequence Editor.
  Evidence: `TimelineGridDrawingTests` creates a test-only subclass of `Common.Controls.Timeline.Element`, assigns an `EffectNode` built from a mocked `IEffectModuleInstance`, and controls the rendered versus placeholder path by changing `IEffectModuleInstance.IsDirty`.

- Observation: Element rendered content is covered by a border overlay, so tests must avoid sampling edge pixels when asserting canvas content.
  Evidence: The initial `Draw_DisposedPlaceholder_DoesNotAffectLaterRenderedImage` assertion sampled pixel `(0, 0)` and saw black from `AddSelectionOverlayToCanvas`; sampling interior pixel `(3, 2)` correctly observes the red content drawn by the test subclass.

- Observation: `Element.Draw(...)` had no production callers other than `Grid.DrawElement(...)`.
  Evidence: `rg` found only `Grid.cs` calling the visible-slice overload of `Draw(...)`, plus the tests added for VIX-3705. This allowed Milestone 2 to route grid drawing through an internal ownership-aware method while preserving the old public method for compatibility.

- Observation: Returning a draw lock from `DrawV2(...)` is not enough by itself to close the race for grid drawing.
  Evidence: If `DrawV2(...)` releases the cache lock before `Grid.DrawElement(...)` takes the returned lock, `Selected` or `RenderElement()` can still acquire the lock and dispose `_cachedImage` in that gap. `Grid.DrawElement(...)` now takes `Element.DrawLock` before calling `DrawV2(...)`, then holds it through `Graphics.DrawImage(...)`.

## Decision Log

- Decision: Treat the element-image path as the primary root-cause area, not `_drawCursors`.
  Rationale: The first logged exception is from `Grid.DrawElement` and native `GdipDrawImageRectI`. `_drawCursors` is the next drawing method after `_drawDrawBox` in `OnPaint`, so a damaged graphics state or earlier native error can surface there even though cursor drawing is simple.
  Date/Author: 2026-07-11 / Codex.

- Decision: Break VIX-3705 into small fixes that can be validated independently.
  Rationale: The bug is rare and timing-sensitive. A broad rendering refactor would be hard to prove and risky. Placeholder disposal, cached image synchronization, and paint error isolation are separable changes with narrower tests and easier rollback.
  Date/Author: 2026-07-11 / Codex.

- Decision: Prefer internal test seams over public API changes for this issue.
  Rationale: The problem is internal drawing resource ownership. If tests need visibility into whether an image is cached or temporary, use internal helpers or narrowly scoped test-only patterns where possible. If a public or protected C# API must change, use the project `csharp-docs` skill and update XML documentation in the same change.
  Date/Author: 2026-07-11 / Codex.

- Decision: Use tuple-returning `Element.DrawV2(...)` and mark the existing `Element.Draw(...)` method obsolete.
  Rationale: The ownership-aware result is the replacement for the old public drawing contract. Keeping `Draw(...)` as an obsolete wrapper gives existing callers a migration path while allowing `Grid.DrawElement(...)` and future code to use the ownership-aware API immediately.
  Date/Author: 2026-07-11 / Codex.

- Decision: Keep `Element.DrawV2(...)` and `Element.DrawLock` internal.
  Rationale: Correct use of `DrawV2(...)` requires taking `DrawLock` before calling it and holding that lock until the returned rendered image has been consumed. Publishing `DrawV2(...)` without the lock exposes an API that external callers cannot use safely, while publishing the lock would expose an implementation synchronization object. The only production consumer is `Grid.DrawElement(...)`, so the whole ownership-aware draw contract belongs inside the assembly.
  Date/Author: 2026-07-11 / Codex.

- Decision: Use a lazily initialized nonserialized object as the element image-cache lock.
  Rationale: `Element` is marked `[Serializable]`, so a nonserialized lock field can be `null` after deserialization. A lazy `CachedImageLock` property preserves normal construction behavior and safely creates a lock for deserialized elements before cache use.
  Date/Author: 2026-07-11 / Codex.

- Decision: Have `Grid.DrawElement(...)` take `Element.DrawLock` before calling `DrawV2(...)`.
  Rationale: Cache disposal and replacement must not occur between retrieving the cached bitmap and drawing it. Taking the element lock before `DrawV2(...)` and holding it through `Graphics.DrawImage(...)` makes `Selected`, `RenderElement()`, and cache regeneration wait for the draw to finish.
  Date/Author: 2026-07-11 / Codex.

- Decision: Expose `Vixen.Common.Controls` internals to `Vixen.Tests`.
  Rationale: The draw lock should remain internal production surface, but the focused concurrency test should be compile-time checked instead of using reflection. `InternalsVisibleTo` follows the existing test access pattern used by other Vixen projects.
  Date/Author: 2026-07-11 / Codex.

- Decision: Stop the current grid paint pass after an element image draw failure.
  Rationale: The reported failure sequence logs a native image draw failure, then continues into later paint layers where `_drawCursors(...)` reports a misleading second exception. Returning a Boolean failure from `DrawElement(...)` and `_drawElements(...)` lets `OnPaint(...)` stop before cursor and resize drawing without throwing a generic paint exception or showing a modal dialog for a failure already logged with element context.
  Date/Author: 2026-07-11 / Codex.

- Decision: Throttle the generic grid paint exception dialog after the first display.
  Rationale: If a non-element paint failure repeats on every invalidation, showing the same modal dialog repeatedly can trap the user. The error is still logged every time; the dialog remains suppressed until a full paint pass succeeds, then a later independent failure can show the dialog again.
  Date/Author: 2026-07-11 / Codex.

## Outcomes & Retrospective

Milestones 1, 2, 3, and 4 are complete. Focused tests now document the current `Element.DrawV2(...)` behavior: rendered effects reuse a cached bitmap for the same visible slice, selection invalidates that rendered cache, unrendered effects return independent placeholder bitmaps, and disposing a placeholder does not poison a later rendered image. Milestone 2 adds an internal ownership-aware drawing contract so `Grid.DrawElement(...)` disposes temporary placeholder images after drawing without disposing cached rendered images, while the obsolete public `Element.Draw(...)` wrapper is marked as a hard do-not-use compatibility API until it is retired. Milestone 3 synchronizes cached bitmap creation, replacement, disposal, and grid drawing on the same per-element lock. Milestone 4 prevents a logged element image draw failure from cascading into later cursor/resize drawing in the same paint pass and adds more context to the element draw failure log.

The main expected implementation outcome remains fewer native GDI+ paint failures caused by image lifetime races or handle pressure. Because the original failure is rare, success should be demonstrated through focused tests for the deterministic ownership bugs, full test-suite validation, and manual Timed Sequence Editor exercise rather than relying on reproducing the exact user crash.

## Context and Orientation

The Timed Sequence Editor displays effects on a scrolling timeline grid. The grid control is `Common.Controls.Timeline.Grid` in `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`. It draws grid lines, rows, snap points, effect images, tooltips, selection boxes, cursors, and resize indicators in `OnPaint(PaintEventArgs e)`.

An effect displayed in the grid is represented by `Common.Controls.Timeline.Element` in `src/Vixen.Common/Controls/TimeLineControl/Element.cs`. The `Element` object stores timing, selection state, display rectangles, and a private bitmap cache named `_cachedImage`. A bitmap is an unmanaged GDI+ image resource. If a bitmap is disposed while another native GDI+ call is drawing it, or if too many temporary bitmaps are leaked, GDI+ can throw misleading exceptions such as `External component has thrown an exception` or `Object is currently in use elsewhere`.

The relevant paint flow is:

`Grid.OnPaint` translates the graphics origin by the current scroll position, then calls `_drawGridlines`, `_drawRows`, `_drawSnapPoints`, `_drawElements`, `_drawInfo`, `_drawSelection`, `_drawDrawBox`, `_drawCursors`, and `_drawResizeIndicator`. The `_drawElements` method iterates visible rows and visible elements, then calls `DrawElement(Graphics g, Row row, Element currentElement, int top)`. `DrawElement` computes a destination rectangle, calls `currentElement.Draw(...)`, and then calls `g.DrawImage(elementImage, destRect)`.

The relevant image flow is:

`Element.Draw(...)` returns `DrawImage(...)` when the effect is rendered, otherwise it returns `DrawPlaceholder(...)`. `DrawImage(...)` creates and stores `_cachedImage` when the cache is missing or the visible slice size changed, then returns `_cachedImage`. `DrawPlaceholder(...)` creates a new gray placeholder bitmap each time and returns it. The caller cannot currently tell whether the returned bitmap is cached and must be retained, or temporary and should be disposed after drawing.

The relevant background rendering flow is:

`Grid.StartBackgroundRendering()` creates a `BackgroundWorker`. `renderWorker_DoWork` processes `_blockingElementQueue` with `Parallel.ForEach`, calls `element.RenderElement()`, and invalidates the grid when changed elements are visible. `Grid.RenderElement(Element element)` has another path that starts a task to call `element.RenderElement()` and `Invalidate()`. `Element.RenderElement()` can call `EffectNode.Effect.PreRender()` and dispose `_cachedImage`. This means image cache mutation can happen while the UI thread is painting.

The reported log sequence for VIX-3705 is:

    Unable to draw element image. System.Runtime.InteropServices.SEHException (0x80004005): External component has thrown an exception.
       at System.Drawing.SafeNativeMethods.Gdip.GdipDrawImageRectI(...)
       at Common.Controls.Timeline.Grid.DrawElement(...)

    Exception in TimelineGrid.OnPaint() System.InvalidOperationException: Object is currently in use elsewhere.
       at Common.Controls.Timeline.Grid._drawCursors(Graphics g)
       at Common.Controls.Timeline.Grid.OnPaint(PaintEventArgs e)

This plan assumes the old line numbers in the report are not exact for current source. Always inspect current code before editing.

## Plan of Work

Milestone 1 establishes a narrow validation strategy before changing behavior. Start by reading `Grid.cs`, `Element.cs`, and existing sequencer tests under `src/Vixen.Tests/Sequencer`. Determine whether a direct unit test can instantiate an `Element` subclass and exercise `Draw(...)`, `RenderElement()`, and `Selected` without a full WinForms paint surface. If current visibility makes direct assertions difficult, add the smallest internal helper needed to distinguish a cached image from a temporary placeholder, or add tests around behavior that is already observable. Do not add a broad UI automation test for the rare crash as the first validation step.

The preferred tests for Milestone 1 are:

1. A test-only `Element` subclass whose `DrawCanvasContent` fills the bitmap and whose render state can be controlled.
2. A test that proves drawing an unrendered element creates a temporary placeholder that can be disposed by the caller without affecting later rendered cache behavior.
3. A test that proves selecting an element invalidates the cached rendered bitmap without racing a draw lock, after Milestone 3 adds synchronization.

If these tests require changing public or protected API, use `.agents/skills/csharp-docs/SKILL.md` and document the API. Prefer internal methods and `InternalsVisibleTo` only if the project already uses that pattern; do not introduce it casually for this ticket.

Milestone 2 fixes the deterministic placeholder leak. Update the image-return contract so `Grid.DrawElement` knows whether it owns the returned bitmap. A minimal shape is to introduce an internal lightweight result type in `Element.cs`, for example an internal readonly struct with a `Bitmap Image` and `bool DisposeAfterDraw`, or an internal method that returns those two values. Keep the existing public `Draw(...)` method if other code calls it, and route `Grid.DrawElement` through the new ownership-aware path. Cached rendered images must have `DisposeAfterDraw = false`; placeholder images must have `DisposeAfterDraw = true`.

After drawing in `Grid.DrawElement`, dispose only the temporary placeholder image in a `finally` block. Do not dispose cached rendered images owned by `Element`. Ensure the code still catches and logs draw failures with enough context to identify the effect, size, and destination rectangle, but do not swallow ownership cleanup.

Milestone 2 acceptance is a focused test that fails before the change or can be manually reasoned from deterministic behavior: unrendered placeholder bitmaps are disposed after `DrawElement` or after the new helper result is consumed, while cached rendered images remain available for the next draw. If direct `DrawElement` testing is impractical because it is private and requires `Row` setup, test the lower-level ownership-return helper and manually inspect `DrawElement`.

Milestone 3 synchronizes cached bitmap ownership. Add a private lock object to `Element`, for example:

    [NonSerialized]
    private readonly object _cachedImageLock = new object();

Because `Element` is `[Serializable]`, consider whether a `readonly` nonserialized lock survives deserialization in this codebase. If deserialization bypasses constructors for this type, use a lazily initialized lock property instead of a readonly field. Record the choice in the `Decision Log`.

Use the same lock for every `_cachedImage` read, replace, and dispose operation. This includes `Selected`, `RenderElement()`, and the code in `DrawImage(...)` that checks cache dimensions and assigns `_cachedImage`. Avoid locking on the `Bitmap` object itself because the bitmap can be replaced and external code can also lock it. The lock should belong to the `Element` instance and remain stable across cache replacement.

The safest pattern is that `Element` does not expose a cached bitmap without protecting it during drawing. If returning a raw cached bitmap is retained for compatibility, provide a method such as `DrawCachedImageTo(Graphics target, Rectangle destRect, ...)` or an ownership result with a callback that draws while holding the element cache lock. Keep the implementation narrow: the goal is to prevent disposing or replacing `_cachedImage` while `Graphics.DrawImage` is using it. Do not hold the lock while calling `EffectNode.Effect.PreRender()` because effect rendering can be expensive and may reenter other code; only lock around cache disposal/replacement and drawing.

Milestone 3 acceptance is a focused concurrency-style test where one thread repeatedly requests/draws the cached image while another thread repeatedly toggles selection or invalidates the cache. The test should not rely on reproducing the native crash; it should assert no managed exceptions and no disposed-image access under a bounded loop. If this is too brittle in CI, keep it as a local stress harness under `src/Vixen.Tests/Sequencer` with conservative iteration counts and record why it is safe.

Milestone 4 reduces cascading paint failures and improves diagnostics. Review the catch in `Grid.DrawElement` and the catch in `OnPaint`. The current `DrawElement` catch logs `Unable to draw element image` and then lets `OnPaint` continue drawing later layers. If native GDI+ has left the `Graphics` object in a bad state, continuing to `_drawCursors` can produce a second misleading exception and show the user a modal error. Consider one of these narrow approaches:

1. Let `DrawElement` rethrow a small internal exception after logging context, so `OnPaint` stops the current paint pass.
2. Have `DrawElement` return a Boolean success value and have `_drawElements`/`OnPaint` stop later drawing for that paint pass when a GDI draw fails.
3. Track a local paint-failed flag in `OnPaint` and skip cursor/resize drawing after an element image failure.

Choose the smallest approach that fits existing code style. Avoid showing repeated modal `MessageBoxForm` dialogs for repeated paint failures. If the modal dialog remains, add throttling or state so one bad paint does not trap the user in repeated dialogs. Record the decision and rationale.

Leave `Pens.Green` in `_drawCursors` unless there is concrete evidence it contributes to the failure. The shared static pen is intended for this kind of draw call, and replacing it does not materially address the reported image draw failure.

Milestone 5 runs validation. Use focused tests first, then the full test project. Manual validation should open a timed sequence in the Timed Sequence Editor, scroll horizontally and vertically, select effects, resize an effect, trigger background rendering by changing an effect that becomes dirty, and verify that effect images, placeholders, cursor lines, selection rectangles, and resize indicators still draw correctly.

Milestone 6 updates Jira issue `VIX-3705`. Include the root-cause hypothesis, code areas changed, automated test results, manual validation notes, and residual risk. Be explicit that the original bug is rare and that validation focused on deterministic resource ownership and stress coverage rather than reproducing the exact customer crash.

## Concrete Steps

Work from repository root `C:\Dev\Vixen`.

Inspect the relevant code before editing:

    rg -n "_cachedImage|DrawPlaceholder|DrawImage|public Bitmap Draw|RenderElement|Selected\\s*=|private void DrawElement|_drawCursors|protected override void OnPaint|Task\\.Factory\\.StartNew|Parallel\\.ForEach" src\Vixen.Common\Controls\TimeLineControl src\Vixen.Modules\Editor\TimedSequenceEditor src\Vixen.Tests\Sequencer -g "*.cs"

Read the core files:

    Get-Content -Path src\Vixen.Common\Controls\TimeLineControl\Element.cs
    Get-Content -Path src\Vixen.Common\Controls\TimeLineControl\Grid.cs
    Get-ChildItem -Path src\Vixen.Tests\Sequencer -File

For any public or protected C# API addition or behavior-changing documentation update, first read:

    Get-Content -Path .agents\skills\csharp-docs\SKILL.md

After Milestone 1 and each implementation milestone, run the most focused relevant tests. If new tests are added under `TimelineGridDrawing`, use:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore

If tests are added under another name, adapt only the filter:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~<NewTestClassName> --no-restore

Before marking the plan complete, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If full tests fail because a running Vixen instance has output DLLs locked, do not kill processes without user approval. Record the error in `Surprises & Discoveries`, close the app manually or ask the user to close it, then rerun.

## Validation and Acceptance

Focused automated tests should cover the deterministic resource ownership changes:

1. Placeholder images returned for unrendered elements are disposed after drawing or after the ownership result is consumed.
2. Cached rendered images are not disposed by `Grid.DrawElement`; they remain owned by `Element`.
3. Cache invalidation from selection or render completion uses the same synchronization path as drawing.
4. A bounded draw-versus-cache-invalidation stress test does not throw managed exceptions.

Run the focused command and expect the new tests to pass:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore

Run the full test project and expect all tests to pass, allowing only pre-existing warnings:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Manual acceptance in the Timed Sequence Editor:

Open a sequence with multiple rows and effects. Scroll the grid so effects enter and leave the visible time range. Select and deselect effects so selected borders update. Resize an effect and confirm resize indicators still draw. Trigger an effect render by changing effect settings or using an operation that marks an effect dirty, then observe the placeholder and final rendered image path. Move playback or cursor position and confirm cursor lines still draw. No `Unable to draw element image` or `Exception in TimelineGrid.OnPaint()` logs should appear during this normal exercise.

Because VIX-3705 is rare, do not require reproducing the original customer crash to accept the fix. Acceptance is based on proving the identified deterministic defects are fixed, verifying no regression in normal drawing, and documenting residual uncertainty.

## Idempotence and Recovery

The plan is safe to implement incrementally. Each milestone should compile and run focused tests before moving on. If a synchronization change causes a deadlock or visible drawing regression, revert only that milestone's local edits rather than resetting the worktree. Do not use `git reset --hard` or discard unrelated user changes.

If the ownership-aware drawing helper proves too invasive, fall back to a narrower method used only by `Grid.DrawElement`, leaving the old `Element.Draw(...)` method in place for compatibility. Record the reason in `Decision Log` and update `Interfaces and Dependencies`.

If concurrency tests are flaky, reduce them to deterministic lock-ownership tests plus a local-only stress note. Do not leave flaky tests in the suite.

## Artifacts and Notes

Important source locations from initial investigation:

    src/Vixen.Common/Controls/TimeLineControl/Grid.cs
      DrawElement(Graphics g, Row row, Element currentElement, int top)
      _drawElements(Graphics g)
      _drawCursors(Graphics g)
      OnPaint(PaintEventArgs e)
      renderWorker_DoWork(...)
      RenderElement(Element element)

    src/Vixen.Common/Controls/TimeLineControl/Element.cs
      private Bitmap _cachedImage
      Selected setter
      RenderElement()
      DrawImage(...)
      DrawPlaceholder(...)
      Draw(...)

Reported VIX-3705 signature:

    Unable to draw element image. System.Runtime.InteropServices.SEHException (0x80004005): External component has thrown an exception.
       at System.Drawing.SafeNativeMethods.Gdip.GdipDrawImageRectI(...)
       at Common.Controls.Timeline.Grid.DrawElement(...)

    Exception in TimelineGrid.OnPaint() System.InvalidOperationException: Object is currently in use elsewhere.
       at Common.Controls.Timeline.Grid._drawCursors(Graphics g)
       at Common.Controls.Timeline.Grid.OnPaint(PaintEventArgs e)

Milestone 1 focused validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore

    Passed!  - Failed: 0, Passed: 4, Skipped: 0, Total: 4

The final focused validation run emitted only pre-existing LiteDB/package warnings. An earlier passing run also showed transient MSBuild copy retry warnings for `SharpFont.dll` and `QuickFont.dll` under `C:\Output`, with locks attributed to `.NET Host` and `Microsoft Defender Antivirus Service`. The retries succeeded and did not block the test run.

Milestone 2 focused validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore

    Passed!  - Failed: 0, Passed: 7, Skipped: 0, Total: 7

The focused validation run emitted pre-existing LiteDB/package warnings and existing compiler warnings unrelated to VIX-3705.

Milestone 3 focused validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore

    Passed!  - Failed: 0, Passed: 8, Skipped: 0, Total: 8

The focused validation run emitted pre-existing LiteDB/package warnings and existing compiler warnings unrelated to VIX-3705.

Milestone 4 focused validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineGridDrawing --no-restore

    Passed!  - Failed: 0, Passed: 8, Skipped: 0, Total: 8

The focused validation run emitted pre-existing LiteDB/package warnings and existing compiler warnings unrelated to VIX-3705. No new direct unit test was added for the private `Grid.DrawElement(...)`/`OnPaint(...)` control-flow change because exercising it would require reflection or broader WinForms paint harness setup; the change remains private and was validated through compile coverage plus inspection of the paint path.

## Interfaces and Dependencies

Do not introduce new third-party dependencies for this work. Use `System.Drawing`, existing WinForms types, and the current `Vixen.Tests` framework.

Milestone 2 initially introduced an ownership-aware draw result, and Milestone 3 kept that contract internal because it must be paired with the internal draw lock:

`src/Vixen.Common/Controls/TimeLineControl/Element.cs` exposes:

    [Obsolete("Do not use. Timeline element drawing is internal and this compatibility API will be removed.")]
    public Bitmap Draw(Size imageSize, Graphics g, TimeSpan visibleStartTime, TimeSpan visibleEndTime, int overallWidth, bool redBorder)

    internal (Bitmap Image, bool DisposeAfterDraw) DrawV2(Size imageSize, Graphics g, TimeSpan visibleStartTime, TimeSpan visibleEndTime, int overallWidth, bool redBorder)

The obsolete `Draw(...)` wrapper delegates to `DrawV2(...)` and returns only `Image` for compatibility, but its documentation and obsolete message mark it as a hard do-not-use API. `Grid.DrawElement(...)` calls internal `DrawV2(...)` so it can dispose images whose `DisposeAfterDraw` value is `true`. Rendered cached images remain owned by `Element`; placeholder images are temporary and disposed by the consumer. Milestone 3 prevents cached image disposal and drawing from racing by requiring `DrawLock` around the `DrawV2(...)` call and returned-image consumption.

Milestone 3 adds:

    internal object DrawLock { get; }

`DrawLock` returns the same lazily initialized object used internally by `Element` to protect `_cachedImage`. It is intentionally separate from the internal `DrawV2(...)` return value because callers must take the lock before calling `DrawV2(...)`; taking a lock returned after `DrawV2(...)` would leave a race window. `Grid.DrawElement(...)` takes this lock before calling `DrawV2(...)` and holds it until the draw call and placeholder cleanup finish.

The lock added to `Element` must remain stable for the lifetime of the element after normal construction and after deserialization. Because the field is `[NonSerialized]`, `CachedImageLock` lazily initializes it before use so deserialized elements do not have a null lock.

No sequence file format, effect data model, or module descriptor should change for VIX-3705.

## Revision Notes

- 2026-07-11 / Codex: Initial ExecPlan created for VIX-3705 after inspecting the reported stack trace, current `Grid.cs` paint flow, `Element.cs` bitmap cache ownership, and background rendering paths. The plan intentionally separates placeholder disposal, cache synchronization, and paint failure isolation so each can be validated narrowly.
- 2026-07-11 / Codex: Completed Milestone 1 by adding `TimelineGridDrawingTests` for current rendered-cache and placeholder-image behavior, then recorded the focused validation result. No production drawing code changed in this milestone.
- 2026-07-11 / Codex: Completed Milestone 2 by adding ownership-aware `Element.DrawV2(...)`, marking `Element.Draw(...)` obsolete, disposing temporary placeholder images from `Grid.DrawElement`, extending `TimelineGridDrawingTests` to cover ownership flags, and recording focused validation.
- 2026-07-11 / Codex: Completed Milestone 3 by synchronizing `_cachedImage` creation/replacement/disposal and grid drawing with the same per-element lock, making `DrawV2(...)` internal with strong lock/ownership documentation, adding a focused draw-lock test that uses `InternalsVisibleTo` instead of reflection, and recording focused validation.
- 2026-07-11 / Codex: Completed Milestone 4 by aborting the current paint pass after logged element image draw failures, adding contextual draw failure logging, throttling repeated generic paint dialogs until a full paint succeeds, and recording focused validation.

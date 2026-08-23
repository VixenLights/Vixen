# Recover Preview Windows Saved Outside Active Monitors

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document according to `.agents/PLANS.md` from the repository root. This plan implements VIX-3990.

## Purpose / Big Picture

After this work, a Preview viewer or Preview Setup window that was saved wholly outside the current monitors will reopen where the user can activate and move it. This fixes the taskbar-thumbnail-with-no-window failure that can occur after monitor arrangements change, including on a single-monitor machine. The GDI and OpenGL Preview viewers and Preview Setup will make the same corner-based recovery decision, while the GDI viewer will retain its existing special handling for a saved maximized window.

The observable proof is that an off-screen saved Preview window falls back to the Windows default location, whereas a window with either its upper-left pixel or its actual upper-right pixel in an active monitor working area restores at the saved location. A working area is the usable part of a monitor reported by Windows; it normally excludes taskbars and docked bars.

## Progress

- [x] (2026-08-23 15:40Z) Updated the VIX-3990 Jira description by appending the confirmed scope, acceptance criteria, and test plan while preserving the reporter's original text and four attachments.
- [x] (2026-08-23 15:43Z) Added `PreviewWindowBounds.IsRecoverable` and 12 monitor-independent xUnit boundary tests; `msbuild Vixen.sln -m -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` and the focused test run passed.
- [x] (2026-08-23 15:49Z) Routed GDI Preview, OpenGL Preview, and Preview Setup through `PreviewWindowBounds`; preserved GDI's maximized intersection branch; restored OpenGL client size before placement validation; and stopped OpenGL from saving geometry while minimized. The `Vixen_Tests` build and focused policy tests passed.
- [ ] (2026-08-23 15:19Z) Build and run the focused and full test commands, perform the renderer persistence checks, then update VIX-3990 with final requirements and validation results.

## Surprises & Discoveries

- Observation: The affected methods build the intended top-right point with `new Point(rect.Top, rect.Right)`, which transposes the horizontal and vertical coordinates.
  Evidence: `GDIPreview/GDIPreviewForm.cs`, `OpenGL/OpenGLPreviewForm.cs`, and `VixenPreviewSetup3.cs` each contain that expression.

- Observation: `Rectangle.Right` is the coordinate immediately after the rectangle, not the last pixel inside it.
  Evidence: For a `Rectangle(-800, 0, 800, 600)`, `Right` is `0`, while the actual upper-right pixel is `(-1, 0)`; treating `(0, 0)` as a window pixel incorrectly accepts an entirely off-left window on a primary `(0, 0, 1920, 1080)` monitor.

- Observation: OpenGL validates a synthetic `100 x 100` saved rectangle before it reads and applies persisted client dimensions.
  Evidence: `OpenGL/OpenGLPreviewForm.cs` constructs `desktopBounds` with `new Size(100, 100)`, tests it, and only later reads `ClientHeight` and `ClientWidth`.

- Observation: The malformed corner check originated in VIX-2519 in August 2018, so version 3.13 exposed the saved-location defect rather than introducing the underlying defect.
  Evidence: commit `3c4da0cab` is titled “VIX-2519 Amend the logic to allow either of the top left or top right to be visible on any screen to restore.”

## Decision Log

- Decision: Introduce one pure, public `PreviewWindowBounds` policy in the `VixenPreview` module instead of keeping three renderer/form-specific corner implementations.
  Rationale: A method supplied with a `Rectangle` and monitor working-area rectangles can be unit-tested without real monitors, and every in-scope caller must use identical boundary semantics. Make it public so the existing `Vixen.Tests` project can test it through its current `VixenPreview` project reference; add complete XML documentation because it is a public C# API.
  Date/Author: 2026-08-23 / Codex

- Decision: Treat only the upper-left pixel and the actual upper-right pixel of a positive-size rectangle as sufficient to restore a normal window at its saved location.
  Rationale: This is VIX-2519's intentional recovery rule: a visible top edge gives the user a reachable title bar. It is deliberately narrower than arbitrary rectangle intersection, which can leave a window unactivatable or unmovable.
  Date/Author: 2026-08-23 / Codex

- Decision: Preserve GDI's existing whole-rectangle intersection test only for a saved maximized state.
  Rationale: A maximized window need not have a visible saved top corner to be recoverable. The handoff explicitly preserves this separate GDI rule; the normal-window route changes only to use the shared corner policy.
  Date/Author: 2026-08-23 / Codex

- Decision: Do not alter `XMLProfileSettings`, setting names, `VixenPreviewData`, WPF/Catel code, renderer output, OpenGL context setup, or the Timed Sequence Editor.
  Rationale: VIX-3990 is window-position recovery only. The Timed Sequence Editor has the same historic typo but is explicitly deferred pending a scope increase. Existing Preview settings and data-model serialization remain compatible.
  Date/Author: 2026-08-23 / Codex

## Outcomes & Retrospective

Milestones 1 through 3 are complete. VIX-3990 preserves the original reporter text and attachments and adds the agreed user-facing scope, acceptance criteria, and test plan. `PreviewWindowBounds.IsRecoverable` now makes the intended pure recovery decision, its 12 focused xUnit tests pass, and all three in-scope forms use it. GDI retains its maximized intersection branch. OpenGL now restores a positive persisted/fallback client size before checking saved placement and does not overwrite valid saved geometry while minimized. Manual renderer verification and the final Jira validation comment remain for Milestone 4.

## Context and Orientation

`src/Vixen.Modules/Preview/VixenPreview` is the Preview module. Its viewers are Windows Forms windows, even though the module also references WPF. `GDIPreview/GDIPreviewForm.cs` is the conventional GDI viewer, and `OpenGL/OpenGLPreviewForm.cs` is the OpenTK-based OpenGL viewer. Both read per-viewer placement settings from `XMLProfileSettings` using instance-specific prefixes: `Preview_{InstanceId}` for GDI and `OpenGLPreview_{InstanceId}` for OpenGL. `VixenPreviewSetup3.cs` is the Preview Setup form; its placement is kept in the existing `VixenPreviewData.SetupLeft`, `SetupTop`, `SetupWidth`, and `SetupHeight` properties.

Today, GDI's `AreCornersVisibleOnAnyScreen`, OpenGL's `IsVisibleOnAnyScreen`, and Preview Setup's `IsVisibleOnAnyScreen` query `Screen.AllScreens` directly. They accept the saved location when the upper-left corner, or a malformed supposed upper-right corner, is inside a screen `WorkingArea`. All three must be replaced by the same policy. `Screen.AllScreens` should remain at the form boundary only, where each form converts each `Screen.WorkingArea` to the policy input.

GDI also has `IsVisibleOnAnyScreen`, which returns whether a screen working area intersects the entire saved rectangle. In `RestoreWindowState`, it uses that method only before restoring a saved `Maximized` state. Do not replace that maximized-state condition with the corner policy.

OpenGL currently writes client height, client width, location X, and location Y during every save, including when the form is minimized. A minimized Windows Form can expose unsuitable geometry such as a minimized location; preserve the last known normal geometry by skipping all four geometry writes when `WindowState == FormWindowState.Minimized`. Continue saving all non-geometry settings on every save exactly as today. During restore, determine the final client size first, assign it, then create the location-validation rectangle with that final saved/fallback size before deciding whether to set `DesktopLocation`.

The test project is `src/Vixen.Tests/Vixen.Tests.csproj`, which already references the Preview module and uses xUnit v3. Existing Preview unit tests live under `src/Vixen.Tests/Preview`. The new pure policy belongs in the Preview module root and should have a matching test file under `src/Vixen.Tests/Preview/VixenPreview`.

## Plan of Work

### Milestone 1: Record the final issue contract

Before code changes, use the repository's `jira` skill to update VIX-3990. Put the following contract into the issue: the faulty point is `new Point(rect.Top, rect.Right)`; the correct top-right pixel is `(rect.Right - 1, rect.Top)`; GDI, OpenGL, and Preview Setup are in scope; Timed Sequence Editor is deferred; GDI's maximized intersection rule stays; OpenGL must validate the restored persisted client-size rectangle and not save minimized geometry; and neither settings keys nor `XMLProfileSettings` change. Add the unit and manual cases from Validation and Acceptance. This makes the implementation requirements and later evidence visible to the ticket before work begins.

Acceptance for this milestone is an updated VIX-3990 description that a developer can implement without relying on this handoff.

### Milestone 2: Add a monitor-independent bounds policy and prove its boundaries

Create `src/Vixen.Modules/Preview/VixenPreview/PreviewWindowBounds.cs` in namespace `VixenModules.Preview.VixenPreview`. Define a public static class named `PreviewWindowBounds` with a public static method:

    public static bool IsRecoverable(Rectangle windowBounds, IEnumerable<Rectangle> workingAreas)

Use `System.Drawing.Rectangle` for both the saved window bounds and each monitor working area. Add XML documentation for the class, method, parameters, and return value. The method must return `false` when `windowBounds.Width <= 0` or `windowBounds.Height <= 0`. Otherwise, calculate these two pixels exactly:

    var upperLeft = windowBounds.Location;
    var upperRight = new Point(windowBounds.Right - 1, windowBounds.Top);

Return `true` only if at least one supplied working area contains either pixel. Do not use `Rectangle.IntersectsWith` here, do not substitute `windowBounds.Right`, and do not query `Screen.AllScreens` inside the policy. Treat a null or empty sequence as having no active working areas and return `false`; this keeps the utility safe in tests and during unusual display enumeration failures. Enumerate the supplied collection once if necessary rather than materializing unrelated display state.

Create `src/Vixen.Tests/Preview/VixenPreview/PreviewWindowBoundsTests.cs`. Use `[Theory]` plus a compact member-data provider or individually named facts so failure output identifies each geometric contract. Pass arrays of `Rectangle` directly to `IsRecoverable`; no test may depend on the executing machine's monitors. Cover, at minimum, these exact scenarios:

- A primary working area `(0, 0, 1920, 1080)` with positive-size windows wholly off the left, right, above, and below returns `false`. Include the reported regression rectangle `(-800, 0, 800, 600)`.
- A one-pixel window at `(1919, 0, 1, 1)` returns `true`, proving the last in-area pixel is included, while `(1920, 0, 1, 1)` returns `false`, proving the exclusive right boundary is not included.
- A window whose upper-left lies off-left but whose actual upper-right pixel is `(0, 0)`, such as `(-20, 0, 21, 50)`, returns `true`.
- A working area with negative coordinates, such as `(-1280, 0, 1280, 1024)`, accepts a visible saved window on that active left-hand monitor.
- A rectangle located on a formerly present monitor but absent from the supplied working-area list returns `false`.
- Zero width and zero height windows each return `false`, even if their location lies in a working area.
- Two working areas separated by a gap, such as `(0, 0, 1000, 800)` and `(1200, 0, 1000, 800)`, reject a window whose two accepted corners fall in the gap. This proves the policy does not infer visibility from an implied virtual-desktop rectangle.

Run the focused test after adding it. Because `PreviewWindowBounds` is public, follow the project `csharp-docs` skill when implementing this API and do not expose form-specific APIs merely to test screen logic.

At the end of this code-changing milestone, invoke the project `commit-msg` skill and include its formatted, VIX-3990-referencing proposed commit message in the milestone completion response. Do not create a commit unless explicitly requested.

### Milestone 3: Make every in-scope form consume the policy

In `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs`, remove `AreCornersVisibleOnAnyScreen` and replace its normal-window restore call with `PreviewWindowBounds.IsRecoverable(desktopBounds, Screen.AllScreens.Select(screen => screen.WorkingArea))`. Preserve `IsVisibleOnAnyScreen` and its `IntersectsWith` implementation for the `windowState.Equals("Maximized")` branch exactly. Do not change the ordering or keys of the existing GDI load/save settings, including its existing guard that avoids saving placement while minimized.

In `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs`, remove the local `IsVisibleOnAnyScreen` helper. In `Setup`, pass `desktopBounds` and `Screen.AllScreens.Select(screen => screen.WorkingArea)` to `PreviewWindowBounds.IsRecoverable`; retain the existing manual bounds assignment for a recoverable rectangle and Windows default location fallback for an unrecoverable rectangle. Do not change `VixenPreviewData` or its existing setup placement fields.

In `src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs`, remove the local `IsVisibleOnAnyScreen` helper and restructure `RestoreWindowState` in this order:

1. Continue restoring the existing non-geometry settings and configuring the form as it does now.
2. Read `ClientHeight` and `ClientWidth` using the existing keys and defaults. Apply the current size fallback (`FindMaxPreviewSize`, then `_height`/`_width`) before validating position. Normalize the final size so the validation rectangle is positive; use the established fallback for missing/zero dimensions and do not introduce a new stored default.
3. Assign the final `ClientSize`, `_width`, and `_height` before placement validation.
4. Read `WindowLocationX` and `WindowLocationY`, create a saved rectangle from that location and the final client size (not `new Size(100, 100)`), and call `PreviewWindowBounds.IsRecoverable` with the current screen working areas. If recoverable, retain `Manual` start position and set `DesktopLocation`; otherwise retain the current Windows-default-location fallback.

In OpenGL's `SaveWindowState`, wrap writes for `ClientHeight`, `ClientWidth`, `WindowLocationX`, and `WindowLocationY` in the same `WindowState != FormWindowState.Minimized` guard used by GDI. Leave the camera and all existing boolean/non-geometry setting writes outside the guard. Keep every existing setting key and `XMLProfileSettings` behavior unchanged; do not add a `WindowState` key or revive the commented-out state persistence.

Use the existing form imports or add only the minimal `System.Linq` import required for projecting working areas. Keep unrelated rendering, data model, WPF, Catel, and OpenGL context code untouched.

Run the focused unit tests after the integration edits, then invoke the project `commit-msg` skill and include its proposed VIX-3990 commit message in the milestone completion response. Do not create a commit unless explicitly requested.

### Milestone 4: Verify recovery and close the issue loop

Build the configured test target with full MSBuild, then run the already-built tests as described below. Manually exercise both Preview renderers using a profile with a deliberately invalid saved location, preferably first on a single-monitor layout matching the report. For GDI, separately verify that a saved maximized preview still restores maximized when its saved rectangle intersects a working area. For OpenGL, place and size the window validly, minimize it, exit/restart Vixen, and verify the valid normal placement and size were not overwritten by minimized geometry. After either renderer recovers an off-screen location, close/restart it and confirm its recovered location and retained size persist using the existing keys.

After validation, use the `jira` skill to make any necessary final corrections to VIX-3990's description and add a comment containing the exact automated-test outcome, manual renderer outcomes, monitor arrangement used, and any deferred Timed Sequence Editor follow-up. Update Progress, Outcomes & Retrospective, and this plan's revision note with actual dates and results. Invoke the project `commit-msg` skill for any code-changing final adjustment and report its proposed message; do not commit without explicit user authorization.

## Concrete Steps

All commands below run from `C:\Dev\Vixen` in PowerShell.

First, inspect the narrow change set before editing:

    rg -n -C 4 "AreCornersVisibleOnAnyScreen|IsVisibleOnAnyScreen|ClientHeight|ClientWidth|WindowLocation" src/Vixen.Modules/Preview/VixenPreview
    rg -n -C 3 "PreviewCanvasVisibility|VixenPreview" src/Vixen.Tests/Preview src/Vixen.Tests/Vixen.Tests.csproj

Run the focused policy tests while iterating. If the test assembly has already been built for the requested configuration, use:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\" --filter "FullyQualifiedName~PreviewWindowBoundsTests"

Expect xUnit to report all `PreviewWindowBoundsTests` as passed. If no built test assembly is available, build the test target first:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Then run the focused command above, followed by the full suite:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\"

Expect the build to exit with code 0 and the test run to report no failures. If the full build fails in a C++/CLI dependency, do not replace the MSBuild command with a bare `dotnet test`; use the displayed MSBuild error to repair the existing build environment or report it as an environment blocker with the focused test result.

Before handing off, inspect only the intended files:

    git diff --check
    git diff -- src/Vixen.Modules/Preview/VixenPreview/PreviewWindowBounds.cs src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs src/Vixen.Tests/Preview/VixenPreview/PreviewWindowBoundsTests.cs docs/plans/preview/vix-3990-preview-window-bounds-recovery.md
    git status --short

## Validation and Acceptance

Automated acceptance is the boundary-test matrix in Milestone 2. It must prove that the upper-right pixel is `Right - 1, Top`, that no positive-size off-screen window is accepted merely because its exclusive `Right` coordinate happens to be on screen, and that negative monitor coordinates and monitor gaps are handled from the explicitly supplied active working areas.

Manual acceptance requires all of the following:

- With only `(0, 0, 1920, 1080)` active, a GDI Preview and an OpenGL Preview saved at `(-800, 0)` with a width of `800` do not reopen invisibly. Each uses Windows default placement and is visible/activatable.
- A Preview Setup window with equivalent off-screen saved `SetupLeft`, `SetupTop`, `SetupWidth`, and `SetupHeight` values also uses Windows default placement.
- A normal viewer whose upper-right pixel is visible while its upper-left is not restores at its saved location, so its title bar remains reachable.
- GDI restores a saved maximized window under its existing intersection rule; normal GDI restores use the shared two-corner rule.
- OpenGL restores the actual persisted/fallback client size before deciding whether its location is recoverable, never uses a synthetic `100 x 100` validation size, and a minimize/restart cycle does not replace valid saved client dimensions or location with minimized values.
- After fallback recovery, closing and reopening either viewer retains usable geometry through the existing `XMLProfileSettings` keys. No profile migration, settings schema, or data-model change is observed.

## Idempotence and Recovery

The policy and tests are additive and can be rerun without modifying profile data. The code changes alter only future recovery decisions; they do not delete or rename persisted values. To reproduce the problem safely, back up the profile if desired, edit only the existing preview location keys or arrange monitors, start the viewer, and let its ordinary save path recover the geometry. If a manual trial leaves a window unreachable, remove only that Preview instance's existing location/size values through the normal profile-management workflow or temporarily restore the backed-up profile; do not delete the entire profile directory.

If a test fails, first compare the supplied `Rectangle` values with Windows' exclusive right/bottom convention. Do not fix failures by broadening the policy to rectangle intersection, accepting `Right`, or reading the real `Screen.AllScreens` in tests. If the change must be backed out, revert only the new policy, its test file, and the three in-scope callers; leave existing profile keys and `XMLProfileSettings` untouched.

## Artifacts and Notes

The regression's essential geometry is:

    working area:  Rectangle(0, 0, 1920, 1080)
    saved window:  Rectangle(-800, 0, 800, 600)
    old malformed point: Point(window.Top, window.Right) = Point(0, 0)     // incorrectly accepted
    required point:      Point(window.Right - 1, window.Top) = Point(-1, 0) // correctly rejected

The shared policy must have this semantic shape; the implementation may use LINQ or an explicit loop, but must not change the two point coordinates:

    if (windowBounds.Width <= 0 || windowBounds.Height <= 0)
        return false;

    Point upperLeft = windowBounds.Location;
    Point upperRight = new Point(windowBounds.Right - 1, windowBounds.Top);
    return workingAreas != null && workingAreas.Any(area =>
        area.Contains(upperLeft) || area.Contains(upperRight));

Expected focused-test output is concise and should resemble:

    Passed!  - Failed: 0, Passed: <boundary-case count>, Skipped: 0, Total: <boundary-case count>

## Interfaces and Dependencies

`PreviewWindowBounds` is the only new public interface. It belongs to the existing `VixenModules.Preview.VixenPreview` namespace and depends only on .NET's `System.Drawing` geometry types and `System.Collections.Generic.IEnumerable<Rectangle>`:

    public static class PreviewWindowBounds
    {
        public static bool IsRecoverable(Rectangle windowBounds, IEnumerable<Rectangle> workingAreas);
    }

The `workingAreas` argument represents only currently active `Screen.WorkingArea` values. The policy must not mutate it, access Windows display APIs, persist settings, or know which renderer called it. GDI, OpenGL, and Preview Setup remain responsible for taking a fresh snapshot with `Screen.AllScreens.Select(screen => screen.WorkingArea)` and for applying their existing form-specific fallback behavior.

No package, project reference, app setting, serialized member, renderer API, WPF/Catel component, or OpenGL context dependency is added. `src/Vixen.Tests/Vixen.Tests.csproj` already references `VixenPreview.csproj`, so no project-file update is required.

---

Revision note (2026-08-23 15:19Z): Created from the VIX-3990 handoff after source inspection. It records the confirmed point transposition, exclusive-boundary correction, shared testable policy, GDI maximized exception, OpenGL restore/save ordering, required tests, and deferred Timed Sequence Editor scope.

Revision note (2026-08-23 15:40Z): Marked Milestone 1 complete after appending the confirmed scope, acceptance criteria, and test plan to VIX-3990. The original reporter text and all four existing attachments were preserved.

Revision note (2026-08-23 15:43Z): Marked Milestone 2 complete after adding the shared policy and 12 focused boundary tests. The `Vixen_Tests` MSBuild target and the focused `PreviewWindowBoundsTests` run both passed.

Revision note (2026-08-23 15:49Z): Marked Milestone 3 complete after integrating the shared policy with both Preview viewers and Preview Setup, retaining the GDI maximized path, and correcting OpenGL geometry restore/save ordering. The `Vixen_Tests` MSBuild target and the focused `PreviewWindowBoundsTests` run both passed.

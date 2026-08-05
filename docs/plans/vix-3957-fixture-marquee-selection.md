# VIX-3957: Make moving-head marquee selection honor preview zoom

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

In Preview Setup, users can drag an empty area to draw a marquee (also called a rubber-band selection rectangle) around fixtures. A moving-head fixture currently selects correctly only at 100% zoom: at 50%, 200%, or 400%, the marquee is tested at a different location from where the fixture is drawn. After this change, a marquee drawn around a moving-head fixture will select it at every supported zoom level. A left-to-right drag will select only fixtures entirely inside the marquee, while a right-to-left drag will select fixtures that the marquee touches or crosses.

The behavior is demonstrable through focused unit tests and manually in Preview Setup by placing a moving-head fixture at model coordinates 100 through 140, changing zoom, and dragging around its visible bounds.

## Progress

- [x] (2026-08-05) Investigated `PreviewMovingHead.ShapeInRect`, its caller, bounds accessors, repository history, and current test-project conventions.
- [x] (2026-08-05 21:20Z) Updated VIX-3957 with the final requirements, acceptance criteria, and test plan before editing code.
- [ ] Add focused moving-head marquee-selection tests that fail against the current implementation.
- [ ] Correct `PreviewMovingHead.ShapeInRect` and its XML documentation.
- [ ] Run targeted and full test validation, then perform the manual Preview Setup check.
- [ ] Reconcile VIX-3957 with the delivered implementation and comment the validation results.

## Surprises & Discoveries

- Observation: `VixenPreviewControl` already implements the drag-direction policy and passes it to every shape.
  Evidence: `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` calls `item.Shape.ShapeInRect(_bandRect, changeX > 0)`, so positive horizontal motion requests full containment and negative motion requests intersection.

- Observation: the moving-head override ignores `allIn` and checks only whether a fixture corner lies in the marquee.
  Evidence: `PreviewMovingHead.ShapeInRect` has an unused `bool allIn = false` parameter and tests `_topLeft`, `_topRight`, `_bottomLeft`, and `_bottomRight` individually.

- Observation: direct clicking already uses display-space coordinates.
  Evidence: `PreviewMovingHead.PointInShape` compares the mouse point with `Left`, `Right`, `Top`, and `Bottom` multiplied by `ZoomLevel`.

- Observation: the current test project already references `VixenPreview` and uses xUnit v3.
  Evidence: `src/Vixen.Tests/Vixen.Tests.csproj` includes the `VixenPreview.csproj` project reference and the `xunit.v3` package.

## Decision Log

- Decision: Limit production edits to `PreviewMovingHead.ShapeInRect`; do not modify mouse handling, scrolling, persisted preview data, or the moving-head partial class.
  Rationale: the control constructs `_bandRect` in displayed (zoomed canvas) coordinates, and `PreviewMovingHeadPartial.cs` deliberately exposes unzoomed model bounds. The defect is solely the override's coordinate conversion and selection predicate.
  Date/Author: 2026-08-05 / Codex

- Decision: Use `Left`, `Right`, `Top`, and `Bottom` multiplied by `ZoomLevel` as `double` values, rather than scale individual stored corner fields or round values.
  Rationale: these bounds are already normalized for the fixture regardless of corner ordering. Keeping values floating point avoids avoidable boundary shifts and matches the display-space comparison used by `PointInShape`.
  Date/Author: 2026-08-05 / Codex

- Decision: Interpret `allIn == true` as inclusive full containment and `allIn == false` as inclusive axis-aligned rectangle intersection.
  Rationale: this is the base method's documented contract and the caller's drag-direction policy. Inclusive comparisons retain the current behavior that edge contact counts as selection.
  Date/Author: 2026-08-05 / Codex

- Decision: Add direct unit tests for the shape predicate instead of UI-event tests.
  Rationale: the controller's coordinate construction and `allIn` forwarding are already present; direct predicate tests cover the defect deterministically without Windows Forms input or scroll-bar setup.
  Date/Author: 2026-08-05 / Codex

- Decision: Preserve the existing VIX-3957 summary, issue type, status, assignment, and all non-description fields.
  Rationale: Milestone 1 requires only requirements refinement; the existing issue is already an accepted bug assigned to the appropriate owner. The updated description now supplies scope, acceptance criteria, and a test plan without altering workflow state.
  Date/Author: 2026-08-05 / Codex

## Outcomes & Retrospective

Not started. At completion, record the production files changed, the exact targeted and full test results, manual verification result, any remaining limitations, and the final VIX-3957 update/comment.

## Context and Orientation

The Preview module represents each fixture with a `PreviewBaseShape`. Model coordinates are the unscaled coordinates stored by a preview shape. Display-space coordinates are the pixel coordinates on the zoomed canvas. The shape's `ZoomLevel` converts model coordinates to display-space coordinates by multiplication. A `Rectangle` supplied to `ShapeInRect` is a display-space marquee rectangle; its width or height can be negative in general, so its two extrema must be normalized before comparing bounds.

`src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` owns Preview Setup mouse interaction. When a user begins a band selection, it stores display-space mouse points (including scroll translation) in `_bandRect`. In the mouse-move handler, it normalizes that rectangle and calls `ShapeInRect(_bandRect, changeX > 0)` for each visible `DisplayItem`. This behavior is correct and must remain unchanged. Positive horizontal drag is the application's full-containment gesture; negative horizontal drag is its intersection gesture.

`src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHeadPartial.cs` stores the moving-head's four corners. Its `Top`, `Bottom`, `Left`, and `Right` overrides return the unscaled extrema of those corners. `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs` contains behavior specific to moving heads. `PointInShape` already scales the extrema by `ZoomLevel`, but `ShapeInRect` currently compares raw corner coordinates against display-space marquee coordinates. The mismatch makes selection correct only at zoom 1.0; for a stored coordinate `p` at zoom `z`, the visible location is `p * z` while the old selection test checks `p`.

The base declaration in `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs` documents `ShapeInRect(Rectangle rect, bool allIn = false)` as testing whether a shape is wholly or partly contained by a rectangle. This override must conform to that contract. No interface, serialized data, or persisted setting changes are needed.

Create `src/Vixen.Tests/Preview/VixenPreview/PreviewMovingHeadSelectionTests.cs`. The test project is an xUnit v3 project and already references the target module. Give test methods descriptive names in the existing `ReturnsExpected...` style. The tests should instantiate `PreviewMovingHead`, set all four `TopLeftPoint`, `TopRightPoint`, `BottomLeftPoint`, and `BottomRightPoint` to the rectangular model bounds 100..140, then set `ZoomLevel` before calling `ShapeInRect` with `System.Drawing.Rectangle` values in display-space. Set all four points explicitly so tests remain independent of the default constructor's initial geometry.

## Plan of Work

### Milestone 1: Align VIX-3957 before implementation

Update the existing VIX-3957 description in the project issue tracker. State that marquee rectangles originate in zoomed canvas coordinates and moving-head bounds originate in unscaled model coordinates; specify that only `PreviewMovingHead.ShapeInRect` changes. Add acceptance criteria: marquee selection succeeds at zoom 0.5, 1.0, 2.0, and 4.0; left-to-right requires full containment; right-to-left accepts intersection including shared edges; and a rectangle at the stale unscaled location does not select a fixture displayed elsewhere. Add the unit and manual test plan from this document. Do not change issue status unless the team workflow requires it.

This milestone produces an issue description that lets a reviewer understand the scope and expected behavior before code changes begin. Record its completion timestamp and any wording decision in `Progress` and `Decision Log`.

### Milestone 2: Establish failing regression coverage

Add `PreviewMovingHeadSelectionTests` under `src/Vixen.Tests/Preview/VixenPreview`. Include a private factory that builds the 100..140 model-space square and assigns its `ZoomLevel`, plus a small helper that creates a display-space rectangle from explicit bounds. Keep test rectangles non-negative except where a distinct normalization test is intentionally useful; the production caller currently supplies normalized `_bandRect` dimensions.

Use theory data for zoom levels 0.5, 1.0, 2.0, and 4.0. For each zoom, assert that an `allIn: true` marquee just outside the displayed fixture bounds selects it. This is the core regression: at 0.5, a 45..75 marquee selects the displayed 50..70 fixture; at 2.0, a 195..285 marquee selects displayed 200..280; 1.0 and 4.0 follow the same multiplication. Assert that an `allIn: false` marquee partially crossing an edge selects, while the same partial marquee with `allIn: true` does not. Assert that an outside marquee does not select in either mode, and that a rectangle touching exactly one displayed fixture edge selects in intersection mode.

Add explicit tests for the two corner-only failures: an intersection marquee completely inside the fixture must return true even though it contains no fixture corner, and a marquee crossing one fixture edge without containing a fixture corner must return true. Add a test at zoom 2.0 or 4.0 placing the marquee around unscaled 100..140 instead of the displayed 200..280 or 400..560 location; it must return false. These tests must fail before the production change for at least the non-100% cases and the ignored-`allIn` cases.

Run the focused tests after adding them and record the expected pre-fix failures in the plan's `Surprises & Discoveries`. Do not weaken assertions to accommodate existing behavior.

### Milestone 3: Implement the coordinate and contract fix

In `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs`, replace the current corner-by-corner body of `ShapeInRect` with constant-time bounds arithmetic. First normalize the marquee endpoints from `rect.X`, `rect.Y`, `rect.Width`, and `rect.Height` into selection left, right, top, and bottom values. Then calculate fixture left, right, top, and bottom as `Left * ZoomLevel`, `Right * ZoomLevel`, `Top * ZoomLevel`, and `Bottom * ZoomLevel`. Keep the scaled fixture values as `double` values until each comparison.

For `allIn == true`, return true only when the normalized selection contains every fixture edge: selection left is less than or equal to fixture left, selection right is greater than or equal to fixture right, selection top is less than or equal to fixture top, and selection bottom is greater than or equal to fixture bottom. For `allIn == false`, return true when the two closed axis-aligned rectangles intersect: selection left is less than or equal to fixture right, selection right is greater than or equal to fixture left, selection top is less than or equal to fixture bottom, and selection bottom is greater than or equal to fixture top. Return false otherwise. Do not allocate points, arrays, or new rectangles in this per-shape method.

Update the XML documentation on this public override under the project `csharp-docs` rules. Its summary must describe testing the moving-head fixture against a marquee rectangle in display-space, its `rect` parameter must say it is the rectangle to evaluate, its `allIn` parameter must say `<see langword="true" />` requires complete containment and `<see langword="false" />` permits any intersection, and its Boolean return must distinguish the corresponding match from false. Remove the stale statement that says the method tests corners of the specified rectangle. No other public or protected API documentation is in scope.

At the end of this milestone, run focused tests. The new tests should pass without changes to `VixenPreviewControl.cs`, `PreviewMovingHeadPartial.cs`, model serialization, or interfaces. Report the required formatted commit message in the milestone completion response but do not create a commit unless explicitly asked:

    fix(preview): select moving heads correctly at zoom

### Milestone 4: Validate the module and close the tracker loop

Run the focused test class, then the complete `Vixen.Tests` project. Build the Debug configuration if the test command's build is insufficient for the team's build policy. Treat existing warnings as baseline unless the change introduces a new warning or error. Manually open Preview Setup, create or select a moving-head fixture located away from the origin, and verify at 50%, 100%, 200%, and 400% zoom that the fixture selects when the marquee encloses its visible bounds. At 200%, verify left-to-right partial overlap does not select, right-to-left partial overlap does select, exact-edge contact does select in right-to-left mode, and a marquee around the old unscaled position does not select.

After final validation, update VIX-3957 so its description exactly reflects the delivered method and tests. Add a tracker comment containing the commands, pass counts, manual results, and any baseline warnings. Update all living-document sections, including the outcomes and the dated revision note at the end of this file.

## Concrete Steps

Run all commands from `C:\Dev\Vixen` in PowerShell.

1. Use the authenticated project issue tracker to update VIX-3957 before code changes with the requirements and acceptance criteria in Milestone 1. If tracker access is unavailable, do not invent an issue update; note the access blocker in the plan and request the appropriate project access while continuing local implementation only if authorized.

2. Add the tests described in Milestone 2, then establish the regression baseline:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~PreviewMovingHeadSelectionTests"

   Before the fix, expect failures for at least the 0.5, 2.0, and 4.0 containment cases and the `allIn`/corner-only contract cases. After the fix, expect the command to exit with code 0 and report all tests in `PreviewMovingHeadSelectionTests` passed.

3. Implement the small method and documentation change in Milestone 3. Re-run the same focused command. Inspect the diff with:

    git diff --check
    git diff -- src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs src/Vixen.Tests/Preview/VixenPreview/PreviewMovingHeadSelectionTests.cs

   Expect `git diff --check` to have no output and the diff to contain only the selection predicate/XML documentation and the new tests.

4. Run the full unit-test project:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

   Expect exit code 0. Record the actual pass count rather than predicting it, because the suite evolves independently.

5. If a Debug build is required separately, run:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

   Expect a successful build. Investigate any new warning or error attributable to these files; retain unrelated known warnings in the validation record.

6. Perform the manual check in Milestone 4, update VIX-3957 with concrete results, then revise `Progress`, `Surprises & Discoveries`, `Decision Log`, `Outcomes & Retrospective`, and the revision note in this plan.

## Validation and Acceptance

The change is accepted only when all focused regression tests pass and the full `Vixen.Tests` project exits successfully. The focused tests must prove all of the following directly against `PreviewMovingHead.ShapeInRect`:

- At zoom 0.5, 1.0, 2.0, and 4.0, a marquee enclosing the displayed 100..140 model-space fixture selects it with `allIn: true`.
- A left-to-right/full-containment request rejects partial overlap, while a right-to-left/intersection request accepts the same overlap.
- A marquee wholly inside the fixture, an edge-crossing marquee, and an exact edge contact are intersections.
- A marquee with no overlap returns false.
- At non-100% zoom, a marquee around the fixture's obsolete unscaled location returns false when that location does not overlap the displayed fixture.

Manual acceptance mirrors those cases in Preview Setup, including scrolling if practical. Direct click selection must remain functional at non-100% zoom, and no selection behavior for non-moving-head shapes should be modified because their code is not touched.

## Idempotence and Recovery

The test and build commands are read-only with respect to source and can be repeated safely. The code change is isolated to one override and one new test file. If a test setup proves difficult because a default `PreviewMovingHead` state is incomplete, correct the test factory by explicitly setting all four public corner properties; do not add production initialization merely to support a test. If tests expose a different coordinate contract, stop before expanding scope, record the observed rectangle and fixture bounds in `Surprises & Discoveries`, and reassess the caller and base-class behavior.

To revert only this work before committing, delete the new test file and restore the pre-change method/documentation using the version-control diff for these two named files. Do not use broad reset or checkout commands that could discard unrelated work in this shared repository.

## Artifacts and Notes

The relevant geometry is intentionally small and should remain visible in review:

    model fixture:       left=100, right=140, top=100, bottom=140
    displayed at zoom z: left=100*z, right=140*z, top=100*z, bottom=140*z
    50% displayed range: 50..70; enclosing marquee 45..75 must select
    200% displayed range: 200..280; enclosing marquee 195..285 must select

For closed rectangles, use these exact predicates:

    fully contained = selectionLeft <= fixtureLeft && selectionRight >= fixtureRight
                      && selectionTop <= fixtureTop && selectionBottom >= fixtureBottom

    intersects      = selectionLeft <= fixtureRight && selectionRight >= fixtureLeft
                      && selectionTop <= fixtureBottom && selectionBottom >= fixtureTop

Repository history identifies the regression source as commit `b988984ba`, which replaced zoom-aware point checks with raw moving-head corner comparisons. Commit `663309dbb` added the `allIn` parameter to this override but did not implement its contract. These commits are diagnostic evidence only; do not revert either commit because the narrow method correction described here preserves later changes.

## Interfaces and Dependencies

No public signature, new type, configuration value, serialization contract, or dependency is introduced. The existing public override remains:

    public override bool ShapeInRect(Rectangle rect, bool allIn = false)

`Rectangle` is `System.Drawing.Rectangle`. `rect` is in zoomed canvas coordinates. `PreviewMovingHead.Left`, `Right`, `Top`, and `Bottom` are unzoomed integer model bounds; `PreviewMovingHead.ZoomLevel` is the existing `double` scale factor. The final override must convert those four bounds into display-space before selection comparison and must honor `allIn` exactly as described above.

The only testing dependency is the existing xUnit v3 package in `src/Vixen.Tests/Vixen.Tests.csproj`; use its `Fact`, `Theory`, `InlineData`, and `Assert` APIs already used throughout the project. Follow the project `csharp-docs` skill for the changed public override's XML documentation.

---

Revision note (2026-08-05, Codex): Created the initial ExecPlan from the VIX-3957 coordinate-space diagnosis after verifying the controller caller, moving-head bounds implementation, base contract, history, and test-project setup. No production code or tests were changed.

Revision note (2026-08-05, Codex): Completed Milestone 1 by replacing the VIX-3957 description with the validated scope, acceptance criteria, and regression/manual test plan. Preserved the issue's summary, type, status, assignee, and other fields.

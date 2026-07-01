# Add Location Rendering to the Spiral Effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This repository contains `.agents/PLANS.md`; maintain this document according to that file. The improvement specification that led to this plan is `docs/effects/vix-3386-spiral-location-support.md`.

## Purpose / Big Picture

After this change, a Vixen user can apply the Spiral effect to a group made from several props and choose `TargetPositioning = Locations`. Instead of restarting the spiral on each prop's string order, Vixen will use each element's preview X/Y location and render one continuous spiral across the virtual rectangle that encloses the selected props. This enables whole-display Spiral effects in the same style as other location-aware effects such as PinWheel, Bars, and Curtain.

The behavior is visible in two ways. Automated tests will show that Spiral can render in location mode and that a regular location grid matches the existing string-mode output. Manual validation in the Vixen UI will show a Spiral effect crossing prop boundaries as one continuous pattern when scrubbing the sequencer timeline.

## Progress

- [x] (2026-07-01) Created this ExecPlan from `docs/effects/vix-3386-spiral-location-support.md` after reviewing the current Spiral implementation, `PixelEffectBase`, location frame-buffer classes, and existing location-aware effects.
- [x] (2026-07-01 17:43Z) Updated Jira VIX-3386 with the requirements, direct per-location design notes, test plan, acceptance criteria, and ExecPlan reference before code changes began. Used the project `jira` skill for this update.
- [x] (2026-07-01 17:48Z) Added focused characterization tests in `src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs` and a Spiral project reference in `src/Vixen.Tests/Vixen.Tests.csproj`. The focused test run fails because `TargetPositioning` is hidden and `RenderEffectByLocation` throws `NotImplementedException`.
- [x] (2026-07-01 17:52Z) Enabled Spiral target positioning and implemented a direct per-location render path in `src/Vixen.Modules/Effect/Spiral/Spiral.cs`. The focused Spiral location tests now pass.
- [ ] Add parity, sparse-coordinate, movement, level, and edge-case tests.
- [ ] Run focused tests and a build or broader test command.
- [ ] Manually validate Spiral location mode in the Vixen UI.
- [ ] Update Jira VIX-3386 with final implementation notes and validation evidence after the feature is complete. Use the project `jira` skill for this update.

## Surprises & Discoveries

- Observation: The Spiral effect data already inherits `EffectTypeModuleData`, so it already has serialized `TargetPositioning` storage through the base data type.
  Evidence: `src/Vixen.Modules/Effect/Spiral/SpiralData.cs` inherits `EffectTypeModuleData`; `src/Vixen.Modules/Effect/Effect/EffectTypeModuleData.cs` defines `TargetPositioning`.
- Observation: Spiral does not currently call `EnableTargetPositioning(true, true)` and does not override `RenderEffectByLocation`.
  Evidence: `src/Vixen.Modules/Effect/Spiral/Spiral.cs` constructor only initializes data and attributes; `PixelEffectBase.RenderEffectByLocation` throws `NotImplementedException`.
- Observation: The focused test run for Milestone 3 still reports pre-existing compile warnings outside Spiral.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~SpiralLocation --no-restore` passed with warnings in `IElementTemplate.cs`, `HardwareUpdateThread.cs`, `ProgramExecutor.cs`, and `MovingHeadSettings.cs`.

## Decision Log

- Decision: Implement Spiral location mode with direct per-location calculation rather than by filling a dense virtual pixel buffer and sampling it.
  Rationale: Whole-display layouts can be sparse across a large preview area. Direct calculation scales with real element count and avoids allocating or filling pixels that have no target element. This matches the preferred design in `docs/effects/vix-3386-spiral-location-support.md` and the approach used by `PinWheel`.
  Date/Author: 2026-07-01 / Codex
- Decision: Preserve the existing string-mode render loop unless tests or benchmarks justify a broader refactor.
  Rationale: The user-visible risk is regressions in an existing effect. A helper can share frame-state and pixel-evaluation logic with location mode while leaving the proven dense string loop mostly intact.
  Date/Author: 2026-07-01 / Codex
- Decision: Use focused unit tests in `src/Vixen.Tests/` and add a project reference to Spiral only if the implementation needs it.
  Rationale: The feature is internal rendering behavior, and a small test suite is the clearest way to prove location support without requiring UI automation.
  Date/Author: 2026-07-01 / Codex
- Decision: Update Jira VIX-3386 as the first implementation step, before writing tests or code, and use the project `jira` skill for the update.
  Rationale: The source specification says the first milestone should update the existing Jira issue with full requirements, acceptance criteria, and design considerations. Doing this first keeps tracking aligned before implementation starts.
  Date/Author: 2026-07-01 / Codex

## Outcomes & Retrospective

Milestones 1, 2, and 3 are complete. Jira VIX-3386 now contains the planned scope, direct per-location design, test plan, acceptance criteria, and a reference to this ExecPlan. The initial production implementation enables target positioning and adds a direct per-location render path. Focused characterization tests now pass. Broader parity, sparse-coordinate, movement, level, and edge-case tests remain for Milestone 4.

## Context and Orientation

Vixen is a Windows desktop application for sequencing animated light shows. Effects are modules under `src/Vixen.Modules/Effect/`. A pixel effect is an effect that renders colors into a two-dimensional pixel buffer before Vixen converts those colors into intents for individual lighting elements.

The Spiral effect lives in `src/Vixen.Modules/Effect/Spiral/`. The key files are:

- `src/Vixen.Modules/Effect/Spiral/Spiral.cs`, which contains the runtime effect class and current rendering logic.
- `src/Vixen.Modules/Effect/Spiral/SpiralData.cs`, which contains the serialized settings for the effect.
- `src/Vixen.Modules/Effect/Spiral/SpiralDirection.cs` and `src/Vixen.Modules/Effect/Spiral/MovementType.cs`, which define user-facing enum settings used by the render code.
- `src/Vixen.Modules/Effect/Spiral/Spiral.csproj`, the module project file.

The shared pixel-effect base class is `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs`. It supports two target-positioning modes:

- `Strings` means the effect renders according to the target element hierarchy. This is the current Spiral behavior.
- `Locations` means the effect renders according to each target element's preview X/Y coordinates. This is the behavior to add.

In location mode, `PixelEffectBase.ConfigureVirtualBuffer()` collects leaf target elements into `ElementLocations` and computes a virtual rectangle around their preview coordinates. The names `BufferWi` and `BufferHt` mean the width and height of the logical render area. The names `BufferWiOffset` and `BufferHtOffset` mean the preview-coordinate offsets needed to convert absolute element coordinates into zero-based coordinates inside the virtual rectangle.

The sparse location frame buffer is `src/Vixen.Modules/Effect/Effect/Location/PixelLocationFrameBuffer.cs`. Sparse means the frame buffer stores only real element locations, not every pixel in the virtual rectangle. A location-aware effect receives this buffer in `RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)`, sets `frameBuffer.CurrentFrame`, iterates `frameBuffer.ElementLocations`, and writes colors by calling `frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, colorOrHsv)`.

An element's preview location comes from the Location property module in `src/Vixen.Modules/Property/Location/`. `LocationModule.GetPositionForElement(IElementNode element)` reads a `LocationData` object with `X`, `Y`, and `Z` integer properties. If an element lacks this property, it reports `(0, 0)`.

Existing effects to study before editing Spiral are:

- `src/Vixen.Modules/Effect/PinWheel/PinWheel.cs`, especially its constructor and `RenderEffectByLocation`. It uses direct per-location calculation.
- `src/Vixen.Modules/Effect/Plasma/Plasma.cs`, `src/Vixen.Modules/Effect/Shapes/Shapes.cs`, and `src/Vixen.Modules/Effect/Text/Text.cs`, which show the standard conversion from absolute preview coordinates to local render coordinates.
- `src/Vixen.Modules/Effect/Bars/Bars.cs`, which shows a larger effect with location-specific helper methods and XML documentation for protected methods.

The current Spiral render algorithm in `Spiral.RenderEffect(int frame, IPixelFrameBuffer frameBuffer)` works like this. For each frame it computes animation state from the effect time, speed, direction, repeat count, thickness curve, rotation curve, grow/shrink flags, and brightness level. It then iterates spiral strands, strand thickness, and Y coordinates. For each candidate pixel it computes X with this formula:

    var x = (strand + (spiralState / 10) + (y * (int)adjustRotation / BufferHt)) % BufferWi;
    if (x < 0) x += BufferWi;

It picks a color from `Colors[colorIdx]`, applies either vertical blend or thickness-based gradient position, optionally applies `Show3D`, multiplies brightness by `LevelCurve`, and writes the pixel.

## Plan of Work

Start by updating the existing Jira issue VIX-3386. Use the project `jira` skill at `.agents/skills/jira/SKILL.md` when performing this update. Read this ExecPlan and copy the user-facing purpose, design notes, test plan, and acceptance criteria into the issue so the tracker is useful before implementation starts. If Jira fields or workflow state require choices, validate them through the Jira tooling rather than assuming project configuration. This first Jira update should not claim implementation is complete; it should describe planned scope and acceptance criteria.

After the Jira issue is updated, add tests or a characterization milestone. In `src/Vixen.Tests/Vixen.Tests.csproj`, add a project reference to `..\Vixen.Modules\Effect\Spiral\Spiral.csproj` if tests need to instantiate `VixenModules.Effect.Spiral.Spiral` directly. If the tests construct location properties directly, also add a project reference to `..\Vixen.Modules\Property\Location\Location.csproj`. Keep these as project references, not DLL references. Use the existing XML project style and avoid solution-file edits unless a new project is created; no new project should be needed.

Create a test file such as `src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs`. Use xUnit v3 like the rest of `src/Vixen.Tests/`. The first test should prove the setup property can become visible after the implementation. A new `Spiral` instance exposes properties through `ICustomTypeDescriptor`, so use `effect.GetProperties()` or `TypeDescriptor.GetProperties(effect, attributes, true)` and check the `TargetPositioning` descriptor's browsable state. The current code should fail this test because Spiral never calls `EnableTargetPositioning`.

For render tests, prefer an end-to-end path through the effect lifecycle: create element nodes, assign target nodes, set `TimeSpan`, call `PreRender()`, and read `Render()` results. Use `ElementNodeService.Instance.CreateSingle` to create leaf nodes because `ElementNode` and `Element` constructors are internal. Put nodes under a group node when needed so string-mode ordering has a parent structure. Add `LocationModule` instances with `LocationData` to leaf node `Properties` using `AddWithoutDefaults` so the explicit X/Y values are not overwritten by `SetDefaultValues()`. Remove any nodes created by a test from `VixenSystem.Nodes.RootNode` in cleanup to avoid leaking static node-manager state between tests. If the effect lifecycle is too costly or difficult to inspect, make the Spiral pixel evaluator `internal` and expose internals to `Vixen.Tests`, but record that decision in this plan before changing course.

In `src/Vixen.Modules/Effect/Spiral/Spiral.cs`, add `using VixenModules.Effect.Effect.Location;`. In the constructor, call `EnableTargetPositioning(true, true)` after data initialization and before or after `InitAllAttributes()`. Keep `InitAllAttributes()` so existing property visibility rules still apply.

Still in `Spiral.cs`, extract the per-frame calculations from `RenderEffect` into a private helper. A simple private readonly struct or private class named `SpiralFrameState` is acceptable. It should hold the frame's interval position, color count, spiral count, delta strands, final spiral thickness, adjusted rotation, spiral state, and brightness level. Preserve the current movement semantics exactly: `MovementType.Iterations` uses `(intervalPos * Speed) % 1`; `MovementType.Speed` updates the `_position` field by `CalculateSpeed(intervalPosFactor) / 1000`; negative speed uses `_negPosition` as it does now; `Direction.Backwards` and `Direction.None` affect non-speed modes exactly as before. Update `SetupRender()` to reset both `_position = 0` and `_negPosition = false`.

Add a private pixel evaluator, for example `TryGetSpiralPixelColor(int x, int y, SpiralFrameState state, out HSV hsv)`. It should return `false` when the coordinate is transparent and `true` when a strand covers that coordinate. Its loop should reproduce the existing Spiral math by scanning `ns` from `0` to `spiralCount - 1`, `thick` from `0` to `spiralThickness - 1`, calculating the candidate X coordinate for the supplied Y, and returning the first matching color. It must apply `Blend`, `Show3D`, and `LevelCurve` the same way the current dense loop does.

Add `protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)` to `Spiral.cs`. This method loops frames, sets `frameBuffer.CurrentFrame`, computes one `SpiralFrameState` for the frame, iterates `frameBuffer.ElementLocations`, converts absolute preview coordinates into local Spiral coordinates, calls `TryGetSpiralPixelColor`, and writes only covered pixels back to the original absolute location. Use the coordinate conversion pattern documented in `PixelEffectBase.RenderEffectByLocation`:

    int localY = Math.Abs((BufferHtOffset - elementLocation.Y) + (BufferHt - 1 + BufferHtOffset));
    localY = localY - BufferHtOffset;
    int localX = elementLocation.X - BufferWiOffset;

Evaluate with `localX` and `localY`, but write with `frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsv)`. Do not allocate a dense `PixelFrameBuffer` for this normal location path.

Keep the existing `RenderEffect(int frame, IPixelFrameBuffer frameBuffer)` dense string render behavior intact unless tests prove a shared evaluator produces identical output. If you refactor the dense path, do it in small steps and run parity tests after each step. Do not change public settings, serialized data members, UI display names, resource strings, or unrelated formatting.

Handle edge cases defensively. If `Colors.Count == 0`, render no pixels for that frame and avoid modulo or divide-by-zero. If `Repeat < 1`, clamp the effective repeat or effective spiral count to at least one while preserving normal configured values. If `BufferWi` or `BufferHt` is zero, return without rendering. Preserve the existing `spiralCount > BufferWi` behavior unless a test reveals an existing crash; this plan is about location support, not redesigning Spiral geometry.

After the direct implementation works, decide whether a benchmark or prototype is needed. If tests and manual checks show acceptable behavior, do not add a dense-buffer alternative. If performance appears questionable on dense or large sparse layouts, add a temporary measurement harness or focused benchmark and compare direct per-location calculation against a dense virtual buffer plus sampling. Keep any benchmark additive and remove throwaway code before completion unless it is useful as a permanent regression test.

Finally, update this ExecPlan as work proceeds, update Jira VIX-3386 again with final implementation notes and validation evidence using the project `jira` skill, and manually validate in the UI.

## Milestones

Milestone 1 updates Jira VIX-3386 before implementation starts. Use the project `jira` skill. Read this ExecPlan, retrieve the existing VIX-3386 issue, and update it with a description that includes the purpose, planned direct per-location design, test plan, and acceptance criteria. The result of this milestone is a Jira issue that a reviewer can use to understand the scope before any code changes are made. Record the Jira update in `Progress`, including the date and a brief note about what changed.

Milestone 2 characterizes the current behavior and sets up the test seam. Add `src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs` and any needed project references. Write at least one test that fails before implementation because `TargetPositioning` is not exposed for Spiral, plus a render test or helper test that demonstrates location mode is not implemented. Run `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~SpiralLocation --no-restore` from `C:\Dev\Vixen`. The expected result before implementation is a focused failure that proves the missing feature, not a compile failure.

Milestone 3 enables target positioning and adds the direct location render path. Modify only `src/Vixen.Modules/Effect/Spiral/Spiral.cs` unless test project references are still needed. At the end of this milestone, setting `TargetPositioning = TargetPositioningType.Locations` on Spiral should not throw, and a small location grid should receive non-transparent frame data where the spiral covers a coordinate.

Milestone 4 completes behavioral coverage. Add or refine tests for rectangular-grid parity with string rendering, sparse coordinate sampling, multi-frame movement, brightness level application, and empty-color safety. The rectangular-grid parity test is important because it proves location mode did not invent a different Spiral effect; it should compare per-element RGB frame data under deterministic settings.

Milestone 5 validates performance enough for the feature's risk. If the direct implementation is simple and focused tests run quickly, record that no dense-buffer prototype was needed. If performance is visibly slow or the test suite suggests high allocation, create a temporary benchmark or measurement harness for a 50 x 50 dense grid, a 5,000-element sparse layout over a 1,000 x 500 virtual rectangle, and a 20,000-element sparse layout over a 2,000 x 1,000 virtual rectangle. Record the timing and allocation evidence in `Surprises & Discoveries`.

Milestone 6 performs final validation and tracker updates. Run the focused Spiral tests, then run either the full `src\Vixen.Tests\Vixen.Tests.csproj` suite or a build if the full suite is not practical. Manually validate in Vixen by applying Spiral to a group of at least two located props and confirming the pattern crosses prop boundaries in location mode. Update Jira VIX-3386 with final implementation notes, acceptance criteria status, and test evidence using the project `jira` skill.

## Concrete Steps

Work from the repository root:

    cd C:\Dev\Vixen

Before editing, check the worktree and make sure unrelated changes are not mixed into this task:

    git status --short

Read the files that define the behavior:

    Get-Content -LiteralPath docs\effects\vix-3386-spiral-location-support.md
    Get-Content -LiteralPath docs\plans\effects\vix-3386-spiral-location-support.md
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Spiral\Spiral.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Effect\PixelEffectBase.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Effect\Location\PixelLocationFrameBuffer.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\PinWheel\PinWheel.cs

Before adding tests or changing code, update Jira VIX-3386. Use the project `jira` skill. The update should add or replace the issue description with the feature purpose, planned implementation design, test plan, and acceptance criteria from this ExecPlan. If the Jira tool asks for fields or issue type information, validate the available project configuration through Jira rather than assuming it. Record the update in this plan's `Progress` section after it succeeds.

The initial Jira issue update should include this content in substance:

    Purpose: Add location-based rendering support to the Spiral effect so grouped props can render one continuous spiral across preview-space coordinates.

    Design: Enable TargetPositioning for Spiral and implement RenderEffectByLocation using direct per-location calculation over PixelLocationFrameBuffer.ElementLocations. Preserve existing string-mode rendering and serialized settings.

    Test Plan: Add focused Spiral location tests for target-positioning enablement, rectangular-grid parity with string mode, sparse coordinate sampling, movement and level controls, and empty-color safety.

    Acceptance Criteria: Spiral exposes Strings and Locations target positioning; Locations mode renders without throwing; location output forms one continuous virtual-rectangle pattern; existing string-mode output remains representative-compatible; all existing Spiral controls are honored; sparse layouts do not use a dense virtual buffer in the normal location path; automated tests and manual UI validation are recorded.

Add or update tests in `src\Vixen.Tests\Effects\SpiralLocationRenderTests.cs`. If needed, add these project references to `src\Vixen.Tests\Vixen.Tests.csproj`:

    <ProjectReference Include="..\Vixen.Modules\Effect\Spiral\Spiral.csproj" />
    <ProjectReference Include="..\Vixen.Modules\Property\Location\Location.csproj" />

If project references are added, keep the edit scoped to the existing test project file. Do not add a new project to `Vixen.sln`.

Run the focused tests to observe the initial failure:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~SpiralLocation --no-restore

Expected initial evidence should resemble this in meaning:

    Failed Spiral_DefaultConstructor_EnablesTargetPositioning
    Assert.True() Failure
    Expected TargetPositioning to be browsable for Spiral.

Implement the feature in `src\Vixen.Modules\Effect\Spiral\Spiral.cs`. Add the location namespace import, enable target positioning in the constructor, add frame-state extraction, add a pixel evaluator, add `RenderEffectByLocation`, and reset `_negPosition` in `SetupRender()`.

Run the focused tests again:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~SpiralLocation --no-restore

Expected final focused evidence should resemble:

    Passed!  - Failed: 0, Passed: 5, Skipped: 0

Run a broader validation command:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If `--no-restore` fails because dependencies are missing, rerun without `--no-restore` after confirming package restore is acceptable:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

If a full test run is too slow or fails for unrelated existing reasons, run a build and record the reason in this plan:

    dotnet msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Before finishing, normalize edited text files to CRLF if needed. This repository uses CRLF line endings and tabs for C# indentation.

## Validation and Acceptance

Automated acceptance requires focused tests that pass after the implementation and fail or are impossible before it. The minimum expected focused tests are:

- `Spiral_DefaultConstructor_EnablesTargetPositioning`, which verifies a new Spiral exposes `TargetPositioning` and accepts `TargetPositioningType.Locations`.
- `Spiral_LocationRender_RectangularGridMatchesStringRender`, which renders deterministic settings in string mode and location mode on equivalent grids and compares RGB frame data per element.
- `Spiral_LocationRender_SparseCoordinatesSampleVirtualRectangle`, which renders non-contiguous preview coordinates and proves only actual element coordinates are written while the pattern is based on the surrounding virtual rectangle.
- `Spiral_LocationRender_SupportsMovementAndLevelControls`, which renders multiple frames and verifies animation changes over time and brightness follows `LevelCurve`.
- `Spiral_LocationRender_EmptyColorsDoesNotThrow`, which sets `Colors` to an empty list and verifies render completion with transparent output.

Manual acceptance requires opening Vixen, creating or loading a profile with at least two props that have preview locations, grouping those props, adding Spiral to the group in the sequencer, and setting `TargetPositioning` to `Locations`. With visible solid colors, `Repeat = 1`, `MovementType = Iterations`, `Speed = 1`, and `Show3D = false`, scrubbing the timeline should show one Spiral pattern crossing from one prop into the next. Switching `TargetPositioning` back to `Strings` should preserve the existing string-based Spiral behavior.

VIX-3386 is accepted when Spiral exposes both target-positioning modes, location mode no longer throws, location mode honors all existing Spiral controls, representative string-mode output is unchanged, sparse layouts do not allocate or fill a dense virtual buffer in the normal render path, and tests plus manual evidence are recorded in this plan.

Tracker acceptance requires Jira VIX-3386 to be updated twice using the project `jira` skill: once before implementation with planned scope and acceptance criteria, and once after validation with final implementation notes and test evidence.

## Idempotence and Recovery

All code changes in this plan are ordinary source edits and can be repeated safely. Running tests is safe. Adding the same project reference twice is not safe, so check `src\Vixen.Tests\Vixen.Tests.csproj` before inserting references.

If tests leave static Vixen nodes behind, update the tests to remove created nodes from the root node during cleanup or create unique names and clean them in `finally` blocks. Avoid adding broad global reset behavior unless the test suite already has a supported helper for it.

If the first coordinate-conversion attempt appears transposed, do not rewrite the base class. Compare against `Plasma`, `Shapes`, `Text`, `Liquid`, or `Whirlpool` and adjust only Spiral's local conversion so it follows the established location-effect convention.

If a refactor of string rendering causes parity failures, revert only the refactor portion and keep the direct location implementation. The existing string render loop is known behavior and should remain the source of compatibility.

## Artifacts and Notes

The core implementation should add code shaped like this, with exact names allowed to vary:

    using VixenModules.Effect.Effect.Location;

    public Spiral()
    {
        _data = new SpiralData();
        EnableTargetPositioning(true, true);
        InitAllAttributes();
    }

    protected override void SetupRender()
    {
        _position = 0;
        _negPosition = false;
    }

    protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
    {
        for (int frame = 0; frame < numFrames; frame++)
        {
            frameBuffer.CurrentFrame = frame;
            var state = CreateSpiralFrameState(frame);

            foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
            {
                int localY = Math.Abs((BufferHtOffset - elementLocation.Y) + (BufferHt - 1 + BufferHtOffset));
                localY = localY - BufferHtOffset;
                int localX = elementLocation.X - BufferWiOffset;

                if (TryGetSpiralPixelColor(localX, localY, state, out HSV hsv))
                {
                    frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsv);
                }
            }
        }
    }

The Jira update should include this concise summary:

    Add location-based rendering support to the Spiral effect. Spiral currently renders only by target string structure, which prevents whole-display groups composed of multiple props from using one continuous spiral pattern across preview-space coordinates. Implement location mode using direct per-location calculation over PixelLocationFrameBuffer.ElementLocations, preserve existing string-mode behavior, and cover enablement, rectangular-grid parity, sparse layout sampling, movement, brightness, and empty-color safety with automated tests.

The initial Jira update is part of Milestone 1 and must happen before test or code changes. The final Jira update is part of Milestone 6 and should include actual test command output and manual validation notes.

Record actual test output here as the plan is executed. Replace this placeholder with focused and broad validation transcripts.

Milestone 1 Jira evidence:

    Updated issue: https://vixenlights.atlassian.net/browse/VIX-3386
    Updated at: 2026-07-01 12:43 -0500
    Fields changed: description
    Description now includes: Purpose, Planned Design, Test Plan, Acceptance Criteria, and Implementation Reference.

Milestone 2 focused characterization test evidence:

    Command: dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~SpiralLocation --no-restore
    Result: Failed, as expected before implementation.
    Summary: Failed: 2, Passed: 0, Skipped: 0, Total: 2.
    Failure 1: Spiral_DefaultConstructor_EnablesTargetPositioning expected TargetPositioning to be browsable, but it was false.
    Failure 2: Spiral_RenderEffectByLocation_DoesNotThrow expected no exception, but reflection invocation wrapped a NotImplementedException from PixelEffectBase.RenderEffectByLocation.

Milestone 3 focused implementation test evidence:

    Command: dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~SpiralLocation --no-restore
    Result: Passed.
    Summary: Failed: 0, Passed: 2, Skipped: 0, Total: 2.
    Notes: The run emitted existing warnings from Vixen.Core and FixtureGraphics, but no Spiral test failures.

## Interfaces and Dependencies

At the end of implementation, `src/Vixen.Modules/Effect/Spiral/Spiral.cs` must contain:

    protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)

This override must use `VixenModules.Effect.Effect.Location.PixelLocationFrameBuffer` and `ElementLocation` from the existing effect infrastructure. It must not add new public or protected API unless there is no practical alternative. If any public or protected C# API is added or changed, use the repository `csharp-docs` skill and update XML documentation in the same change.

The `Spiral` constructor must call:

    EnableTargetPositioning(true, true);

The private helper API should remain private unless tests force `internal` visibility. Preferred private helpers are:

    private SpiralFrameState CreateSpiralFrameState(int frame)
    private bool TryGetSpiralPixelColor(int x, int y, SpiralFrameState state, out HSV hsv)

`SpiralData` should not require a new data member because `EffectTypeModuleData.TargetPositioning` already stores the target-positioning mode.

The tests may depend on these projects:

- `src/Vixen.Core/Vixen.Core.csproj`, already referenced by `src/Vixen.Tests/Vixen.Tests.csproj`.
- `src/Vixen.Modules/Effect/Spiral/Spiral.csproj`, needed to instantiate Spiral in tests.
- `src/Vixen.Modules/Property/Location/Location.csproj`, needed if tests assign real `LocationModule` instances to element nodes.

Do not introduce new NuGet packages for this work.

## Revision Notes

- 2026-07-01 / Codex: Created the initial ExecPlan from the reviewed VIX-3386 Spiral location-rendering specification so implementation can proceed from a self-contained plan under `docs/plans/`.
- 2026-07-01 / Codex: Revised the plan after it was moved to `docs/plans/effects/` so updating Jira VIX-3386 is the first milestone and both Jira updates explicitly require the project `jira` skill.
- 2026-07-01 / Codex: Completed Milestone 1 by updating Jira VIX-3386 with the planned scope, design notes, test plan, acceptance criteria, and ExecPlan reference before code implementation began.
- 2026-07-01 / Codex: Completed Milestone 2 by adding focused characterization tests and confirming they fail against current Spiral behavior for hidden target positioning and missing location rendering.
- 2026-07-01 / Codex: Completed Milestone 3 by enabling Spiral target positioning, adding the direct location render path, and confirming the focused Spiral location tests pass.

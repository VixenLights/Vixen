# VIX-3386 Spiral Location Rendering Specification

## Purpose

The Spiral effect currently renders only against the target element string structure. This makes the effect useful on a single
matrix, tree, or similarly ordered prop, but it does not support whole-display or multi-prop layouts where the desired pattern
should be based on each element's preview location. VIX-3386 adds location rendering so a user can apply Spiral to a group of
props and see one continuous spiral pattern across their preview-space coordinates.

This document is a detailed implementation specification. It is intended to be converted into an ExecPlan under `docs/plans/`
before coding begins. The ExecPlan must follow `.agents/PLANS.md`.

## Current Behavior

`src/Vixen.Modules/Effect/Spiral/Spiral.cs` inherits from `PixelEffectBase` and implements only the string rendering path:

- The constructor creates `SpiralData` and calls `InitAllAttributes()`.
- It does not call `EnableTargetPositioning(true, true)`, so the `TargetPositioning` setup property remains hidden.
- `RenderEffect(int frame, IPixelFrameBuffer frameBuffer)` renders the effect into a dense string-mode pixel buffer by iterating
  every spiral strand, every thickness pixel, and every Y coordinate.
- There is no `RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)` override. If location mode were made
  selectable without implementing the override, `PixelEffectBase.RenderNodeByLocation` would call the base implementation and
  throw `NotImplementedException`.

`src/Vixen.Modules/Effect/Spiral/SpiralData.cs` already inherits from `EffectTypeModuleData`, which contains the serialized
`TargetPositioning` property. No new data member is needed only to enable location mode.

## Repository Context

The shared location-rendering behavior lives in `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs`.

`PixelEffectBase.TargetPositioning` has two modes:

- `Strings`: render by the target element hierarchy. This is the current Spiral behavior.
- `Locations`: collect all leaf target elements, read each element's Location property, create a virtual coordinate rectangle
  that encloses those elements, and render against those preview coordinates.

When `TargetPositioning` is `Locations`, `PixelEffectBase.ConfigureVirtualBuffer()` computes:

- `ElementLocations`: one `ElementLocation` per target leaf element.
- `BufferWi`, `BufferHt`: the width and height of the virtual rectangle after orientation normalization.
- `BufferWiOffset`, `BufferHtOffset`: the X and Y offsets needed to map absolute preview coordinates into the zero-based
  virtual rectangle used by effect math.

`PixelLocationFrameBuffer` in `src/Vixen.Modules/Effect/Effect/Location/PixelLocationFrameBuffer.cs` stores sparse frame data
only for actual element locations. Effects that render by calculation should set only those locations instead of filling a dense
virtual buffer.

Good examples:

- `src/Vixen.Modules/Effect/PinWheel/PinWheel.cs` enables target positioning and implements a direct per-location calculation.
- `src/Vixen.Modules/Effect/Curtain/Curtain.cs` filters `frameBuffer.ElementLocations` by preview-space coordinates and writes
  only matching locations.
- `src/Vixen.Modules/Effect/Bars/Bars.cs` contains a more complex location implementation with helper methods for coordinate
  conversion, rotation, and sparse location rendering.

## Required User Behavior

After implementation, Spiral must expose the same `TargetPositioning` setup option as other location-aware pixel effects. The
user must be able to choose `Locations`, place Spiral on a group that contains elements from multiple props, and see a single
spiral pattern rendered across the combined preview-space bounding rectangle.

Location mode must preserve the existing Spiral controls:

- `Direction`
- `MovementType`
- `SpeedCurve`
- `Speed`
- `Repeat`
- `ThicknessCurve`
- `RotationCurve`
- `Show3D`
- `Grow`
- `Shrink`
- `Colors`
- `Blend`
- `LevelCurve`

The location rendering result must match string rendering when the target locations form a regular rectangular grid with the
same coordinate order as a string-mode matrix. For sparse layouts, the effect should appear as if it were rendered onto the
virtual rectangle and sampled only at the existing element coordinates.

## Proposed Design

The preferred design is direct per-location calculation, not dense virtual-buffer rendering.

Spiral's current string implementation fills many pixels that may not correspond to real elements in whole-display layouts.
Location mode should instead iterate `frameBuffer.ElementLocations` once per frame, compute the color that the current Spiral
settings would produce at that coordinate, and call `frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsvOrColor)`.
This follows the performance-oriented pattern used by PinWheel and avoids allocating or filling a potentially large virtual
buffer for sparse whole-house groups.

The implementation should refactor the existing Spiral math just enough to share behavior between string and location modes.
Avoid changing public effect settings, serialized data shape, or UI labels unless a verified bug requires it.

### Target Positioning Enablement

In `Spiral.Spiral()`, call:

    EnableTargetPositioning(true, true);

Keep `InitAllAttributes()` so `StringOrientation` continues to be hidden when `TargetPositioning` is `Locations`. `PixelEffectBase`
already forces `StringOrientation` to vertical in location mode.

### Shared Frame State

Extract the per-frame values currently calculated at the beginning of `RenderEffect` into a private helper, for example
`CreateSpiralFrameState(int frame)`. This helper should compute all values needed to evaluate a single pixel:

- `intervalPos`
- `intervalPosFactor`
- `colorCount`
- `spiralCount`
- `deltaStrands`
- `spiralThickness`
- `adjustRotation`
- `spiralGap`
- `position`
- `spiralState`
- `level`

The helper must preserve existing movement behavior exactly. In particular:

- `MovementType.Iterations` uses `(intervalPos * Speed) % 1`.
- `MovementType.Speed` updates the instance field `_position` by `CalculateSpeed(intervalPosFactor) / 1000`.
- Negative speed sets `_negPosition` and inverts `spiralState` only in the same cases as the current code.
- `Direction.Backwards` and `Direction.None` continue to affect non-speed movement exactly as they do today.
- `Grow` and `Shrink` continue to adjust `spiralThickness` using the current `position`.

Because `_position` is stateful for `MovementType.Speed`, `SetupRender()` must continue to reset `_position = 0`. The
implementation should also reset `_negPosition = false` there to make repeated renders deterministic.

### Pixel Evaluation

Create a private helper that evaluates one logical Spiral pixel, for example:

    private bool TryGetSpiralPixelColor(int x, int y, SpiralFrameState state, out HSV hsv)

The helper should return `false` when no spiral strand covers the coordinate. It should return `true` and the computed HSV when
the coordinate is covered.

The helper must reproduce the existing string-mode rendering rules:

1. For each spiral strand `ns` in `0..spiralCount - 1`, calculate `strandBase = ns * deltaStrands` and `colorIdx = ns % colorCount`.
2. For each `thick` in `0..spiralThickness - 1`, calculate `strand = (strandBase + thick) % BufferWi`.
3. Calculate the strand X coordinate at the requested Y using the current formula:

       candidateX = (strand + (spiralState / 10) + (y * (int)adjustRotation / BufferHt)) % BufferWi

   If `candidateX` is negative, add `BufferWi`.
4. If `candidateX != x`, continue searching.
5. If `Blend` is true, use `Colors[colorIdx].GetColorAt((double)(BufferHt - y - 1) / BufferHt)`.
6. If `Blend` is false, use `Colors[colorIdx].GetColorAt((double)thick / spiralThickness)`.
7. Convert to HSV, multiply `V` by `LevelCurve`, and apply `Show3D` using the same formula as the current code.
8. Return the first matching color.

This direct implementation has complexity proportional to `numFrames * elementCount * spiralCount * spiralThickness`. That is
acceptable for the first implementation because `spiralCount` and `spiralThickness` are bounded by existing UI ranges and this
still avoids `numFrames * BufferWi * BufferHt` work on sparse layouts. If benchmarking shows this direct scan is too slow on
dense matrices, the ExecPlan should include a second milestone to index strands by X per frame or otherwise reduce lookup cost.

### String Mode Preservation

After adding the pixel evaluator, `RenderEffect(int frame, IPixelFrameBuffer frameBuffer)` may either keep its current nested
rendering loops or be refactored to call the shared evaluator for each dense pixel. Prefer the lower-risk option:

- Keep the existing dense string-mode rendering loop unless sharing the evaluator can be proven to produce identical output and
  comparable performance.
- If code is duplicated between string and location mode, limit duplication to the small final color application block. Do not
  make broad unrelated formatting changes.

### Location Coordinate Conversion

Location mode receives absolute preview coordinates in `ElementLocation.X` and `ElementLocation.Y`. Spiral math needs zero-based
virtual coordinates matching `BufferWi` and `BufferHt`.

Use the same conversion pattern documented in `PixelEffectBase.RenderEffectByLocation` and used by effects such as Plasma,
Shapes, Text, Liquid, and Whirlpool:

    int localY = Math.Abs((BufferHtOffset - elementLocation.Y) + (BufferHt - 1 + BufferHtOffset));
    localY = localY - BufferHtOffset;
    int localX = elementLocation.X - BufferWiOffset;

Evaluate the Spiral pixel using `localX` and `localY`, but write the result back to the frame buffer using the original absolute
location:

    frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsv);

This preserves the sparse frame buffer keys while applying the same Y inversion used by the dense string buffer.

Before implementation, verify this coordinate mapping against `PixelEffectBase.ConfigureVirtualBuffer()`, because that method's
internal width and height calculations are orientation-normalized. If the first test grid appears transposed, follow the existing
location-aware effect convention rather than inventing a new mapping.

### Location Render Loop

Add:

    protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)

The method should:

1. Loop frames from `0` to `numFrames - 1`.
2. Set `frameBuffer.CurrentFrame = frame`.
3. Optionally call `frameBuffer.ClearFrame()` if transparent defaults are not guaranteed by the sparse buffer initialization.
4. Compute the frame state once.
5. Iterate `frameBuffer.ElementLocations`.
6. Convert each absolute location to local Spiral coordinates.
7. Evaluate the Spiral pixel.
8. Set only covered pixels. Leave uncovered pixels transparent.

The implementation must include `using VixenModules.Effect.Effect.Location;` in `Spiral.cs`.

## Edge Cases

The implementation must handle these cases without throwing:

- `Colors.Count == 0`: Existing UI normally provides colors, but defensive code should avoid divide-by-zero. If no colors exist,
  render transparent for that frame.
- `Repeat < 1`: Existing UI range prevents this, but rendering code should clamp the effective repeat or spiral count to at
  least 1.
- `spiralCount > BufferWi`: Existing string-mode math can produce `deltaStrands = 0`, which then makes `spiralThickness` start
  at 1 and many strands overlap. Do not broaden this fix unless tests prove current behavior is already broken; preserve
  existing visual behavior for compatibility.
- `BufferWi == 0` or `BufferHt == 0`: `PixelEffectBase.RenderNodeByLocation` already returns early, but private helpers should
  still avoid modulo or division by zero.
- Sparse layouts with duplicate preview coordinates: `PixelLocationFrameBuffer` stores sparse data by coordinate, so duplicate
  element locations share the same sampled color. Do not attempt to make duplicates render differently.

## Performance Requirements

Location rendering must not allocate a dense `PixelFrameBuffer` sized to the virtual preview rectangle for the normal Spiral
location path. It should allocate only small per-frame state and reuse existing `PixelLocationFrameBuffer` storage.

The ExecPlan should include a benchmark or measurement milestone before finalizing the implementation if there are competing
approaches. The measurement should compare:

- Direct per-location calculation.
- Dense virtual buffer render plus sparse sampling, if a prototype is created.

Recommended test sizes:

- Dense matrix: 50 x 50, 2,500 elements.
- Medium sparse whole-display layout: 5,000 elements spread across a 1,000 x 500 virtual rectangle.
- Large sparse layout: 20,000 elements spread across a 2,000 x 1,000 virtual rectangle.

The direct calculation approach is acceptable if it is not materially slower than dense rendering on dense matrices and is
clearly faster or lower allocation on sparse layouts. Record actual timing and allocation evidence in the future ExecPlan's
`Surprises & Discoveries` or `Artifacts and Notes` section.

## Test Specification

Add focused automated tests in `src/Vixen.Tests/`. The current test project does not reference the Spiral effect project, so
the implementation milestone must add a project reference to `src/Vixen.Modules/Effect/Spiral/Spiral.csproj` if needed. Follow
the repository rule for project references: project reference, Copy Local = No, Include Assets = None where the project file
style supports it.

Recommended tests:

1. `Spiral_DefaultConstructor_EnablesTargetPositioning`

   Verify a new `Spiral` exposes `TargetPositioning` as a browsable property and can be set to `TargetPositioningType.Locations`
   without throwing.

2. `Spiral_LocationRender_RectangularGridMatchesStringRender`

   Build a small regular grid of elements with matching location properties, render Spiral once in `Strings` mode and once in
   `Locations` mode, and assert each element receives the same RGB frame data. Use deterministic settings:

   - `MovementType = MovementType.Iterations`
   - `Direction = SpiralDirection.None`
   - fixed `ThicknessCurve`
   - fixed `RotationCurve`
   - fixed `LevelCurve`
   - `Show3D = false`
   - `Blend = false`
   - two or three solid `ColorGradient` values

3. `Spiral_LocationRender_SparseCoordinatesSampleVirtualRectangle`

   Build elements at non-contiguous preview coordinates, such as `(10, 20)`, `(15, 20)`, `(10, 25)`, and `(15, 25)`. Render in
   location mode and assert at least one expected location is colored and at least one expected gap remains irrelevant because no
   element exists there. This proves the implementation samples absolute locations without trying to render missing pixels.

4. `Spiral_LocationRender_SupportsMovementAndLevelControls`

   Render multiple frames with `MovementType.Iterations`, `Direction.Forward`, and a non-100 `LevelCurve`. Assert the frame data
   changes over time and brightness is reduced by the level curve.

5. `Spiral_LocationRender_EmptyColorsDoesNotThrow`

   Set `Colors` to an empty list and render in location mode. Assert the render completes and produces transparent frame data.

If setting up full `PreRender()` effect tests is too expensive because of element-node construction or Location property
registration, extract the pixel evaluator with `internal` visibility and add `InternalsVisibleTo` for `Vixen.Tests`. The
preferred test remains end-to-end rendering through the effect lifecycle because it validates `TargetPositioning`, virtual
buffer dimensions, and sparse frame-buffer writes together.

## Manual Validation

After implementation, manually validate in the Vixen UI:

1. Create or open a profile with at least two props that have preview locations.
2. Group those props under one target group.
3. Add Spiral to the group in the sequencer.
4. Set `TargetPositioning` to `Locations`.
5. Use visible solid colors, `Repeat = 1`, `MovementType = Iterations`, `Speed = 1`, and `Show3D = false`.
6. Scrub the timeline and confirm the spiral crosses prop boundaries as one continuous pattern instead of restarting per prop.
7. Switch back to `Strings` and confirm the previous per-string behavior still works.

## Acceptance Criteria

VIX-3386 is complete when all of the following are true:

- Spiral exposes `TargetPositioning` and allows both `Strings` and `Locations`.
- Choosing `Locations` no longer throws during render.
- Spiral in location mode renders one continuous virtual-rectangle pattern across all target element preview coordinates.
- Existing string-mode output is unchanged for representative settings.
- Location mode honors Direction, MovementType, Speed, Repeat, Thickness, Rotation, Show3D, Grow, Shrink, Colors, Blend, and
  Level controls.
- Sparse whole-display layouts do not require filling a dense virtual buffer in the normal render path.
- Automated tests cover enablement, rectangular-grid parity, sparse location sampling, multi-frame movement, and empty-color
  safety.
- Performance evidence is captured if a dense-buffer prototype or alternative algorithm is evaluated.
- The Jira issue VIX-3386 is updated with the final requirements, design notes, test plan, and acceptance criteria from this
  specification.

## Jira Update Draft

Use this content to update VIX-3386 before implementation begins.

Summary:

Add location-based rendering support to the Spiral effect.

Problem:

Spiral currently renders only by target string structure. Whole-display groups composed of multiple props cannot use Spiral as
one continuous pattern across preview-space coordinates.

Requirements:

Spiral must expose TargetPositioning, support Locations, render a continuous spiral across the virtual rectangle enclosing all
target element locations, preserve existing string-mode behavior, and honor all existing Spiral controls in both modes.

Design Notes:

Implement location mode using direct per-location calculation over `PixelLocationFrameBuffer.ElementLocations`. Avoid allocating
and filling a dense virtual buffer for sparse whole-display layouts. Share or carefully duplicate the existing string-mode Spiral
math so movement, direction, thickness, blend, Show3D, and level behavior remain compatible.

Acceptance Criteria:

TargetPositioning can be set to Locations; rendering in Locations mode does not throw; rectangular location grids match
string-mode output; sparse layouts sample the virtual preview rectangle; existing string rendering remains unchanged; automated
tests cover the expected behavior and edge cases.

## ExecPlan Notes

The future ExecPlan should be saved under `docs/plans/` and should break implementation into these milestones:

1. Document current behavior and add tests that prove Spiral currently lacks location support.
2. Enable target positioning and add the location render path using direct per-location calculation.
3. Add or refine parity, sparse-layout, movement, and edge-case tests.
4. Measure performance against a dense virtual-buffer prototype only if there is evidence the direct calculation is too slow.
5. Manually validate in the UI and update VIX-3386 with the implemented design and acceptance evidence.

The ExecPlan must keep `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` current as required
by `.agents/PLANS.md`.

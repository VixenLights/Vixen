# VIX-1119 Fire Location Rendering Specification

## Purpose

The Fire effect currently renders only against the target element string structure. This makes the effect useful on a matrix,
tree, or similarly ordered prop, but it does not support whole-display or multi-prop layouts where the fire should use each
element's preview location. VIX-1119 adds location rendering so a user can apply Fire to a group of props and see one continuous
fire simulation across their combined preview-space coordinates.

This specification follows the same user, compatibility, coordinate, performance, test, and delivery considerations as
`docs/effects/vix-3386-spiral-location-support.md`. Fire requires a different rendering strategy from Spiral because a Fire cell
depends on neighboring cells in the preceding simulation row. Missing preview elements therefore cannot simply be omitted from
the simulation without changing how heat propagates.

The Jira connector and authenticated issue page were unavailable during this analysis. The issue contract below is based on the
user-supplied VIX-1119 scope, the current repository, and the completed VIX-3386 design and implementation. No unavailable old
patch is assumed.

This document is a detailed implementation specification. It is intended to be converted into an ExecPlan under `docs/plans/`
before coding begins. The ExecPlan must follow `.agents/PLANS.md`.

## Current Behavior

`src/Vixen.Modules/Effect/Fire/Fire.cs` inherits from `PixelEffectBase` and implements only the string rendering path:

- The constructor creates `FireData` but does not call `EnableTargetPositioning(true, true)`, so the inherited
  `TargetPositioning` setup property remains hidden.
- `SetupRender()` allocates one dense integer heat buffer with `BufferWi * BufferHt` cells.
- `RenderEffect(int frame, IPixelFrameBuffer frameBuffer)` generates a random source row, propagates heat through every cell of
  the dense rectangle, maps the simulation to the selected `FireDirection`, converts heat indexes through `FirePalette`, and
  writes the colors to the string frame buffer.
- `FireDirection.Left` and `FireDirection.Right` swap the simulation width and height before propagation.
- There is no `RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)` override. If location mode were only
  exposed, `PixelEffectBase.RenderNodeByLocation` would call the base implementation and throw `NotImplementedException`.

The current propagation intentionally samples the previous row at `x - 1`, `x + 1`, `x`, and `x` again. The duplicated center
sample weights the center cell twice. This is existing Fire behavior and must not be "cleaned up" as part of location support.

`src/Vixen.Modules/Effect/Fire/FireData.cs` already inherits from `EffectTypeModuleData`, which contains the serialized
`TargetPositioning` property and preserves it during cloning. No new serialized data member is required.

The existing public setting named `Location` is a `FireDirection` and means the edge from which flames originate: `Bottom`,
`Top`, `Left`, or `Right`. It is separate from the inherited `TargetPositioning` setting, whose values are `Strings` and
`Locations`. Both settings must remain available and retain their current names for compatibility.

## Repository Context

The shared target-positioning behavior lives in `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs`.

`PixelEffectBase.TargetPositioning` has two modes:

- `Strings`: render by the target element hierarchy. This is Fire's current behavior.
- `Locations`: collect all leaf target elements, read their Location property, create a virtual coordinate rectangle enclosing
  those elements, and render against preview coordinates.

When `TargetPositioning` is `Locations`, `PixelEffectBase.ConfigureVirtualBuffer()` computes:

- `ElementLocations`: one `ElementLocation` per target leaf element.
- `BufferWi`, `BufferHt`: the logical width and height of the orientation-normalized virtual rectangle.
- `BufferWiOffset`, `BufferHtOffset`: the absolute preview-coordinate offsets used to map into that rectangle.

`PixelLocationFrameBuffer` in `src/Vixen.Modules/Effect/Effect/Location/PixelLocationFrameBuffer.cs` stores output only for actual
element locations. Duplicate preview coordinates share the same sparse output cell.

Relevant precedents are:

- `src/Vixen.Modules/Effect/Spiral/Spiral.cs`, which enables target positioning, performs the standard absolute-to-local
  coordinate conversion, and writes only actual locations.
- `src/Vixen.Modules/Effect/Spiral/Spiral.cs` and `src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs`, which establish
  rectangular-grid parity, sparse sampling, property enablement, control coverage, and performance measurement as expectations.
- `src/Vixen.Modules/Effect/Wave/Wave/Wave.cs`, `Whirlpool/Whirlpool/Whirlpool.cs`, and `Morph/Morph/Morph.cs`, which show the
  repository's dense virtual-render-then-sample option. That option is correct but would duplicate Fire's already-dense heat
  storage with a second dense color buffer.

The design was reviewed using the project `dotnet-best-practices`, `dotnet-design-pattern-review`, and `catel-mvvm` guidance.
The change stays inside the effect module and its tests; it adds no service, dependency injection, view model, view, command, or
code-behind responsibility.

## Required User Behavior

After implementation, Fire must expose the same inherited `TargetPositioning` setup option as other location-aware pixel
effects. A user must be able to select `Locations`, place Fire on a group containing elements from multiple props, and see one
continuous fire simulation over the combined preview-space bounding rectangle.

Location mode must preserve the existing Fire controls:

- `Location` (`FireDirection.Bottom`, `Top`, `Left`, or `Right`)
- `Height`
- `HueShiftCurve`
- `LevelCurve`

On a regular rectangular location grid, location output must match string output for the same simulated heat frame and
orientation. On a sparse layout, the result must appear as though Fire were simulated across the entire virtual rectangle and
then sampled only at actual element coordinates. A gap between props must still participate in heat propagation even though it
has no output element.

Switching an existing effect between `Strings` and `Locations` must not reset or reinterpret the existing controls. Existing
serialized effects must continue to default to `Strings` unless they already contain a different inherited
`TargetPositioning` value.

## Design Alternatives

### Direct Per-Location Simulation

This is not suitable for Fire. Spiral can calculate a color independently for any coordinate, but Fire cannot: a cell depends
on adjacent cells from the preceding simulation row. Simulating only actual element locations would treat gaps as missing heat,
break propagation across props, and produce a different effect from a dense virtual rectangle.

### Dense Color Buffer Then Sparse Sampling

Location mode could allocate a `PixelFrameBuffer`, call the existing `RenderEffect`, and copy colors at each
`ElementLocation`. This is a valid low-code fallback and follows Wave and Whirlpool, but it retains Fire's dense integer heat
buffer and adds a dense `Color` buffer of the same virtual dimensions. Whole-display layouts are the case most likely to have a
large, sparse bounding rectangle, so the additional allocation and dense HSV-to-RGB conversion are avoidable.

### Dense Heat Simulation With Sparse Projection

This is the preferred design. Continue to simulate every heat cell in `_fireBuffer`, preserving neighborhood behavior, random
call order, and gaps between props. After the heat field is complete, location mode converts and writes colors only for
`frameBuffer.ElementLocations`. String mode projects the same heat field to every logical output coordinate.

This strategy uses the minimum state compatible with Fire's existing algorithm:

- Dense simulation cost: `O(BufferWi * BufferHt)` per frame.
- Location projection cost: `O(elementCount)` per frame.
- Persistent simulation allocation: one `int[BufferWi * BufferHt]` per effect render.
- No additional dense `PixelFrameBuffer` in the normal location path.

## Proposed Design

### Target Positioning Enablement

In `Fire.Fire()`, after `_data` is initialized, call:

    EnableTargetPositioning(true, true);

Initialize the orientation attribute state so `StringOrientation` is visible for `Strings` and hidden for `Locations`. Apply the
same attribute refresh when `ModuleData` is replaced during deserialization. A small private helper such as
`InitAllAttributes()` may call `UpdateStringOrientationAttributes(true)` and be used by both the constructor and the
`ModuleData` setter.

Do not add a second target-positioning property and do not rename the existing `Location` direction setting.

### Shared Simulation and Projection Responsibilities

Refactor `RenderEffect` into three focused private responsibilities while preserving its formulas and iteration order:

1. `CreateFireFrameState(int frame)` calculates frame values that do not vary by cell:
   - effect interval position factor;
   - brightness level;
   - hue shift;
   - simulation width and height after applying `FireDirection`;
   - clamped effective height;
   - heat adjustment step.
2. `GenerateFireHeat(FireFrameState state)` fills `_fireBuffer` for exactly one frame.
3. A projection helper reads a heat index at a logical output coordinate, converts it through `FirePalette`, applies hue and
   level, and returns whether the coordinate is lit.

Exact private names may vary. A private readonly record struct is acceptable for immutable per-frame state if it fits the local
code style; no public data transfer type is needed.

`GenerateFireHeat` must preserve the existing random sequence and calculations:

    for simulationX in 0 .. simulationWidth - 1:
        heat[simulationX] = simulationX is even
            ? 190 + Rand() % 10
            : 100 + Rand() % 50

    effectiveHeight = max((int)Height.GetValue(intervalPositionFactor), 1)
    step = 255 * 100 / simulationHeight / effectiveHeight

    for simulationY in 1 .. simulationHeight - 1:
        for simulationX in 0 .. simulationWidth - 1:
            neighbors = valid values from:
                (simulationX - 1, simulationY - 1)
                (simulationX + 1, simulationY - 1)
                (simulationX,     simulationY - 1)
                (simulationX,     simulationY - 1)  // intentional duplicate
            newIndex = integer average of neighbors, or zero when none exist
            if newIndex > 0:
                newIndex += Rand() % 100 < 20 ? step : -step
                clamp newIndex to [0, FirePalette.Count() - 1]
            heat[simulationY * simulationWidth + simulationX] = newIndex

Do not parallelize this loop. It has row dependencies, and changing evaluation order would also change the sequence of random
values and therefore the effect's visual character.

### Output Coordinate to Simulation Coordinate Mapping

Projection should work from a logical output coordinate `(outputX, outputY)` in the standard `PixelFrameBuffer` coordinate
system, where `(0, 0)` is the lower-left corner. Convert that coordinate to the heat simulation using this inverse mapping:

| `FireDirection` | Simulation width | Simulation height | `simulationX` | `simulationY` |
|---|---:|---:|---:|---:|
| `Bottom` | `BufferWi` | `BufferHt` | `outputX` | `outputY` |
| `Top` | `BufferWi` | `BufferHt` | `outputX` | `BufferHt - outputY - 1` |
| `Left` | `BufferHt` | `BufferWi` | `outputY` | `outputX` |
| `Right` | `BufferHt` | `BufferWi` | `outputY` | `BufferWi - outputX - 1` |

This is the exact inverse of the current forward transformation in `RenderEffect`. It ensures the source row appears on the
bottom, top, left, or right edge respectively without duplicating four render loops.

The pixel evaluator should then:

    colorIndex = GetFireBuffer(simulationX, simulationY, simulationWidth, simulationHeight)
    if colorIndex == 0:
        return false

    hsv = FirePalette.GetColor(colorIndex)
    if hueShift > 0:
        hsv.H += hueShift / 100.0f
    hsv.V *= level
    return true

Keep the current `hueShift > 0` behavior and the current palette semantics. Corrections to negative hue shifts, palette bounds,
or palette construction are separate issues unless characterization tests show location support cannot be implemented safely
without them.

### String Mode Preservation

`RenderEffect(int frame, IPixelFrameBuffer frameBuffer)` should create the frame state, generate the heat field once, iterate all
logical output coordinates, evaluate each coordinate with the shared inverse mapping, and set lit pixels on the supplied dense
frame buffer.

Refactoring the current forward projection to the inverse evaluator is acceptable only after characterization tests cover all
four directions. Setting pixels in a different order does not change the completed heat field, but heat generation loop order
and random call count must remain unchanged.

If the inverse projection refactor cannot be proven equivalent, retain the existing string projection loop and use a separate
inverse mapping only for location sampling. Do not duplicate the heat generation algorithm.

### Location Coordinate Conversion

Add:

    protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)

and import `VixenModules.Effect.Effect.Location` in `Fire.cs`.

For each absolute `ElementLocation`, first convert preview coordinates into the logical output coordinates used by string-mode
effect math:

    int outputY = Math.Abs((BufferHtOffset - elementLocation.Y) +
                           (BufferHt - 1 + BufferHtOffset));
    outputY -= BufferHtOffset;
    int outputX = elementLocation.X - BufferWiOffset;

For locations inside the configured virtual rectangle this is equivalent to:

    outputX = elementLocation.X - previewXMin;
    outputY = previewYMax - elementLocation.Y;

Use the table above to map `(outputX, outputY)` to the heat buffer. Write any resulting color back with the original absolute
preview coordinate:

    frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsv);

This retains the sparse buffer's keys and matches the Y inversion used by Spiral and other location-aware effects.

### Location Render Loop

The location override should:

1. Return without rendering if `numFrames <= 0`, `BufferWi <= 0`, or `BufferHt <= 0`.
2. Loop frames from `0` through `numFrames - 1` in order.
3. Set `frameBuffer.CurrentFrame` for each frame.
4. Create one frame state and generate the complete dense heat field once.
5. Iterate `frameBuffer.ElementLocations` once.
6. Convert each absolute preview location to logical output coordinates.
7. Evaluate the corresponding heat cell using the selected `FireDirection`.
8. Set only lit locations. Unset cells retain the sparse frame buffer's black/transparent default.

`SetupRender()` continues to allocate `_fireBuffer` once for the full virtual rectangle, and `CleanUpRender()` continues to
release it after rendering. The allocation product is unchanged by left/right direction because those directions only swap
width and height.

### Serialized Data and Compatibility

No changes are required in `FireData`, `FireDirection`, `FirePalette`, or `FireDescriptor` for the feature contract.
`EffectTypeModuleData.TargetPositioning` already supplies serialization and clone behavior.

Existing effects remain in `Strings` because `EffectTypeModuleData` defaults `TargetPositioning` to `Strings`. The existing
`FireData` defaults and `OnDeserialized` hue-shift migration remain unchanged.

No new public or protected C# API is expected beyond the required protected override already defined by `PixelEffectBase`. If
implementation adds or changes another public or protected member, use the project `csharp-docs` skill and update XML comments
in the same change.

## Mathematical and Boundary Rules

The implementation must preserve or safely handle these cases:

- `BufferWi == 0` or `BufferHt == 0`: return before allocation, division, or rendering. `PixelEffectBase` normally prevents this,
  but helpers should not rely exclusively on the caller.
- `Height <= 0`: clamp the effective height to `1`, matching current behavior.
- Large `Height`: preserve integer division in the current `step` calculation, including a possible `step` of zero.
- `FireDirection.Left` and `Right`: swap simulation dimensions but not the logical output dimensions.
- Heat index zero: do not set an output pixel, matching the current rendering optimization and black result.
- Sparse gaps: calculate their heat values even though no output is written for them.
- Duplicate preview coordinates: allow `PixelLocationFrameBuffer` to share one sampled color; do not attempt per-element
  differentiation at the same coordinate.
- Missing Location properties: follow the shared `ElementLocation` behavior; do not special-case Fire.
- Hue and level curves: evaluate once per frame, then apply to every sampled lit location exactly as string mode does.
- Randomness: preserve the source-row and propagation random call count and order for a given buffer size and frame sequence.
- Repeated renders: allocate a fresh zeroed heat buffer in `SetupRender()` and do not retain heat between effect render passes.
- Integer allocation size: calculate or validate the cell count before allocating if implementation introduces new dimension
  arithmetic. Do not add an arbitrary preview-size cap without measured evidence and a separately agreed user behavior.

## Concurrency, Performance, and Thread Safety

Each Fire effect instance owns `_fireBuffer`, so no synchronization is required around the heat field. `FirePalette` is
initialized once and read thereafter. `Rand()` uses the existing thread-local random source.

Location rendering must remain sequential per effect instance. Parallel row generation is invalid because each row depends on
the previous row. Parallel cell generation would change random assignment even within a row. The design adds no static mutable
state, tasks, locks, or UI-thread work.

The normal location path must not allocate a dense `PixelFrameBuffer` or convert every virtual heat cell to RGB. It is expected
to retain one dense integer heat field because exact sparse virtual-rectangle semantics require the cells between actual
elements.

The future ExecPlan should measure at least these scenarios in Release configuration:

- Dense matrix: 50 x 50 virtual rectangle, 2,500 elements, 100 frames.
- Medium sparse display: 5,000 elements over a 1,000 x 500 virtual rectangle, 20 frames.
- Large sparse display: 20,000 elements over a 2,000 x 1,000 virtual rectangle, 3 frames.

Measure elapsed time and allocated bytes for:

1. Dense heat simulation with sparse projection, the proposed implementation.
2. Dense heat plus dense `PixelFrameBuffer` rendering and sparse sampling, as a comparison prototype if practical.
3. A dependency-pruned heat simulation only if the proposed implementation is materially too slow. Such an optimization must
   reproduce dense results and random behavior before adoption; it is not part of the initial design.

Record evidence in the ExecPlan. Unlike Spiral, Fire cannot be expected to scale only with element count because its simulation
is spatially dependent. Acceptance should focus on avoiding unnecessary second dense buffers and conversions while preserving
the effect.

## Test Specification

Add focused xUnit tests in `src/Vixen.Tests/Effects/FireLocationRenderTests.cs`. Add a project reference from
`src/Vixen.Tests/Vixen.Tests.csproj` to `..\Vixen.Modules\Effect\Fire\Fire.csproj`; do not add a new test project or solution
folder.

Fire uses randomness, so exact string/location parity must compare two projections of the same deterministic heat field or use a
small deterministic random seam. Do not write flaky assertions against unrelated `ThreadSafeRandom` sequences. Prefer private
helpers exercised through the existing reflection style in `SpiralLocationRenderTests`; use an `internal` seam only if it makes
the production design clearer and does not affect the hot loop.

Recommended tests:

1. `Fire_DefaultConstructor_EnablesTargetPositioning`

   Verify `TargetPositioning` is browsable and can be set to `TargetPositioningType.Locations`. Also verify
   `StringOrientation` hides in location mode and is restored in string mode.

2. `Fire_RenderEffectByLocation_DoesNotThrow`

   Configure a non-empty virtual rectangle and location buffer, invoke the location override, and verify it completes.

3. `Fire_LocationProjection_RectangularGridMatchesStringProjection`

   Generate one deterministic heat frame, project it to a dense string buffer and a complete location grid, and compare RGB at
   every corresponding coordinate after preview Y inversion. Run this theory for Bottom, Top, Left, and Right.

4. `Fire_LocationRender_OriginatesAtSelectedEdge`

   With deterministic source values, verify the source row maps to the bottom, top, left, and right logical output edge for the
   corresponding direction.

5. `Fire_LocationRender_SparseCoordinatesSampleDenseHeatField`

   Use non-contiguous located elements with offsets. Verify only actual coordinates are stored while their values come from the
   same dense heat field used for a complete grid. Include a location whose heat depends on an intermediate coordinate that has
   no element, proving gaps still participate in propagation.

6. `Fire_LocationRender_AppliesHeightHueAndLevel`

   Use deterministic heat generation to verify Height changes propagation, `HueShiftCurve` changes the sampled hue, and
   `LevelCurve` scales value. A zero level is a stable assertion for fully dark output.

7. `Fire_LocationRender_MultipleFramesAdvanceAndStayInBounds`

   Render multiple frames and verify every source and propagation access remains in range for narrow shapes such as 1 x N and
   N x 1 in all directions.

8. `Fire_ModuleData_PreservesLocationAttributeState`

   Replace `ModuleData` with data whose inherited target positioning is `Locations` and verify the setup property state is
   refreshed, especially that `StringOrientation` remains hidden.

If tests expose an existing Fire defect unrelated to location support, document it and scope it separately unless it blocks
safe implementation. Characterize the duplicated center sample, current hue condition, and integer step behavior rather than
silently changing them.

## Manual Validation

After implementation, manually validate in Vixen:

1. Create or open a profile with at least two props that have preview locations and a visible gap between them.
2. Group those props under one target group.
3. Add Fire to the group in the sequencer.
4. Set `TargetPositioning` to `Locations` and confirm `StringOrientation` is hidden.
5. Set `Location` to `Bottom`, use visible Height, Hue Shift, and Brightness curves, and scrub the timeline.
6. Confirm flames form one simulation across both props rather than restarting per prop.
7. Repeat with `Top`, `Left`, and `Right`, confirming flames originate at the selected preview edge.
8. Change Height, Hue Shift, and Brightness and confirm each control affects location rendering.
9. Switch to `Strings` and confirm the previous string-based behavior and orientation setting still work.
10. Save, reopen, and confirm both target positioning and fire direction persist independently.

## Acceptance Criteria

VIX-1119 is complete when all of the following are true:

- Fire exposes `TargetPositioning` and supports both `Strings` and `Locations`.
- Choosing `Locations` renders without `NotImplementedException`.
- Location mode renders one continuous fire simulation over the virtual rectangle containing all target preview coordinates.
- Sparse gaps participate in propagation but allocate no output pixels beyond actual element locations.
- Bottom, Top, Left, and Right originate from the matching preview edge.
- Height, Hue Shift, and Brightness behave in location mode as they do in string mode.
- Regular-grid location projection matches string projection for the same deterministic heat frame.
- Existing string-mode output is unchanged for representative settings and all four directions.
- Location mode does not allocate a second dense color frame buffer in its normal path.
- Automated tests cover property enablement, direction mapping, parity, sparse sampling, controls, frame progression, and module
  data attribute refresh.
- Focused and full-suite automated validation plus manual UI results are recorded in the future ExecPlan.
- VIX-1119 is updated with the final user-facing requirements, acceptance criteria, and validation outcome when Jira access is
  available.

## Jira Update Draft

Use this user-facing content to update VIX-1119 before implementation begins, following the project `jira` skill.

### Summary

Add preview-location rendering to the Fire effect.

### Scope

- Allow Fire to render as one continuous effect across a group of located props.
- Preserve the existing choice of bottom, top, left, or right flame origin.
- Preserve Height, Hue Shift, Brightness, and existing string-based behavior.
- Ensure gaps between props behave as part of the same virtual display area.

### Acceptance Criteria

- Given a group containing multiple located props, when Fire uses location positioning, then one continuous fire pattern spans
  the group instead of restarting for each prop.
- Given any flame origin, when Fire is rendered by location, then flames originate from the matching preview edge.
- Given existing Fire controls, when their values change, then location output responds consistently with string output.
- Given an existing sequence using Fire, when it is opened after the change, then it retains string positioning and its existing
  appearance unless the user selects location positioning.

## ExecPlan Notes

The future ExecPlan should be saved under `docs/plans/effects/vix-1119-fire-location-support.md` and include these milestones:

1. Restore authenticated Jira access if available and update VIX-1119 with the user-facing scope and acceptance criteria.
2. Add baseline tests showing target positioning is hidden and the location render path is absent.
3. Enable target positioning and refresh orientation property attributes during construction and module-data replacement.
4. Extract shared heat generation and color projection without changing string output.
5. Implement dense-heat/sparse-output location rendering and all four direction mappings.
6. Complete deterministic parity, sparse-gap, control, boundary, and persistence tests.
7. Measure CPU and allocation behavior for dense and sparse virtual rectangles; optimize only with evidence.
8. Run focused and full automated validation, perform manual UI validation, and update Jira with the outcome.

Keep `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` current as required by
`.agents/PLANS.md`.

## Architecture Design: VIX-1119 Fire Location Support

- **Core Strategy:** Enable the inherited target-positioning choice and separate Fire into dense heat simulation plus output
  projection. Keep the heat field dense because neighbor propagation crosses sparse gaps; project only actual locations to the
  sparse frame buffer. This applies the Strategy-like separation and compatibility focus from the active .NET design-review
  skills without adding public abstractions.
- **Data Model & Property Contracts:** Reuse inherited serialized `TargetPositioning`; retain `Fire.Location` as the independent
  flame-origin direction; add no serialized fields. Refresh `StringOrientation` visibility after construction, target-mode
  changes, and module-data replacement.
- **Mathematical / Boundary Logic:** Convert absolute preview coordinates to lower-left logical coordinates, then invert the
  existing direction mapping using the table in this specification. Preserve dense row propagation, the duplicated center
  sample, integer averaging and step calculation, palette clamping, hue condition, brightness scaling, and random call order.
- **Subsystem Component Matrix:** Modify `Fire.cs` for enablement, shared simulation/projection helpers, and the location
  override; add `FireLocationRenderTests.cs`; add one Fire project reference to `Vixen.Tests.csproj`. No changes are expected in
  Fire data, direction, palette, descriptor, shared pixel infrastructure, views, or view models.
- **Concurrency, Performance & Thread Safety:** Keep sequential per-instance rendering. Allocate one dense integer heat field
  per render, no dense color field in location mode, and write only actual element coordinates. Benchmark virtual-area cost and
  allocations before considering dependency-pruned simulation.

## TERRA HAND-OFF CONTEXT

VIX-1119 adds `TargetPositioning=Locations` to `VixenModules.Effect.Fire.Fire`. Jira details could not be authenticated, so use
the user-supplied scope and `docs/effects/vix-1119-fire-location-support.md` as the contract. Fire differs from Spiral: heat at
`(x,y)` depends on `(x-1,y-1)`, `(x+1,y-1)`, and `(x,y-1)` twice, so sparse per-element calculation is invalid. Required design:
call `EnableTargetPositioning(true,true)`; refresh `StringOrientation` attributes in constructor and `ModuleData` setter; retain
inherited serialized `EffectTypeModuleData.TargetPositioning` and existing `Fire.Location` direction; split current render into
per-frame state, dense heat generation, and color projection; preserve random call order, duplicate-center weighting, integer
step, palette clamp, hue condition, and level scaling. Location rendering keeps `_fireBuffer` dense but does not allocate a
dense `PixelFrameBuffer`; per frame generate heat once, iterate `PixelLocationFrameBuffer.ElementLocations`, convert absolute
preview `(X,Y)` to logical `outputX=X-BufferWiOffset`, `outputY=previewYMax-Y`, then map to simulation coordinates: Bottom
`(outputX,outputY)`, Top `(outputX,BufferHt-outputY-1)`, Left `(outputY,outputX)`, Right
`(outputY,BufferWi-outputX-1)`. Write colors at original absolute coordinates. Add Fire project reference and deterministic tests
for property visibility, no-throw, regular-grid projection parity for all directions, origin edges, sparse gaps participating in
dense propagation, Height/Hue/Level, multi-frame narrow buffers, and module-data attribute refresh. Benchmark dense 50x50,
sparse 5k over 1000x500, and sparse 20k over 2000x1000. Do not change FireData, FireDirection, FirePalette, FireDescriptor,
PixelEffectBase, UI/MVVM, or public APIs unless implementation evidence forces a separately documented decision.

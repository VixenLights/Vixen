# Add Preview-Location Rendering to the Fire Effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This repository contains `.agents/PLANS.md`; maintain this document according to that file. The implementation contract for this work is `docs/effects/vix-1119-fire-location-support.md`. That contract takes precedence if this plan is revised.

## Purpose / Big Picture

After this change, a Vixen user can apply Fire to a group of props, select `TargetPositioning = Locations`, and see one continuous fire simulation across the bounding rectangle of those props' preview coordinates. Today Fire only uses the string structure of its target, so it restarts or cannot render as a display-wide preview-space effect.

The existing setting named `Location` will continue to mean the flame-origin edge (`Bottom`, `Top`, `Left`, or `Right`). It is independent from the inherited `TargetPositioning` setting (`Strings` or `Locations`). A user will be able to change either setting without changing the meaning or saved value of the other. The behavior is demonstrable through deterministic projection tests, performance measurements, and manual sequencer validation with separated props.

## Progress

- [x] (2026-08-24) Created this ExecPlan from the implementation contract after reading the current Fire renderer, `PixelEffectBase`, `PixelLocationFrameBuffer`, the existing Spiral location tests, and the completed VIX-3386 ExecPlan. No production code or tests have been changed.
- [x] (2026-08-24) Updated Jira VIX-1119 with the user-facing preview-location scope, compatibility commitments, validation plan, and acceptance criteria. The issue is in progress; no implementation completion was claimed.
- [ ] Add failing characterization tests and the Fire project reference.
- [ ] Expose inherited target positioning and correctly refresh Fire setup-property visibility.
- [ ] Separate Fire's dense heat generation from its string and sparse-location output projections without changing existing string output.
- [ ] Add location projection, deterministic behavioral coverage, and boundary coverage.
- [ ] Record Release performance and allocation evidence for all required layouts.
- [ ] Run focused and full automated validation, complete manual UI validation, update Jira with outcomes, and complete this plan's retrospective.

## Surprises & Discoveries

- Observation: Fire has the required inherited serialized storage already, but does not currently expose it or implement location rendering.
  Evidence: `FireData` inherits `EffectTypeModuleData`; `Fire.Fire()` only creates `FireData`; `PixelEffectBase.RenderEffectByLocation` throws `NotImplementedException` unless a derived effect overrides it.
- Observation: Fire cannot compute only the actual element locations when the preview layout has gaps.
  Evidence: each heat cell reads the preceding-row left neighbor, right neighbor, and the center neighbor twice. Omitting a virtual coordinate changes later heat values and therefore changes the visible simulation.
- Observation: `PixelLocationFrameBuffer` is sparse but accepts a virtual location list and deduplicates equal preview coordinates.
  Evidence: its constructor stores `ElementLocations = elementLocations.Distinct()` and creates data only at supplied X/Y keys. `SetPixel` silently writes only a stored coordinate.
- Observation: the virtual-buffer width/height names are orientation-normalized by `PixelEffectBase`; Fire's standard default orientation is vertical.
  Evidence: `ConfigureVirtualBuffer()` assigns raw preview Y extent to `_bufferWi` and X extent to `_bufferHt`; the public protected accessors swap them only when `StringOrientation` is horizontal. Tests must set or construct dimensions through the same orientation assumptions as production code.
- Observation: VIX-1119 was temporarily unavailable to the connected Jira account but was retrievable and editable on retry.
  Evidence: the initial `getJiraIssue(VIX-1119)` call on 2026-08-24 returned “Issue does not exist or you do not have permission to see it.” A retry returned issue 15583 and `editJiraIssue` saved the planned description at 2026-08-24T15:08:11-05:00.

## Decision Log

- Decision: Treat `docs/effects/vix-1119-fire-location-support.md` as the implementation contract and do not infer requirements from an unavailable Jira page.
  Rationale: the contract explicitly records that Jira access was unavailable during analysis and gives complete behavior, test, compatibility, and performance requirements.
  Date/Author: 2026-08-24 / Codex
- Decision: Preserve one dense integer heat field and make output projection sparse in location mode.
  Rationale: Fire's row-to-row neighbor dependency requires every virtual coordinate to participate, but converting every virtual coordinate to RGB or allocating a dense color buffer is unnecessary when only actual elements receive output.
  Date/Author: 2026-08-24 / Codex
- Decision: Preserve Fire's existing random source-row and propagation loop ordering exactly; share or refactor projection only after all four direction parity tests characterize string behavior.
  Rationale: random-call order is observable output for this effect. A mathematically similar heat loop with a changed traversal order can produce a different animation.
  Date/Author: 2026-08-24 / Codex
- Decision: Make no changes to `FireData`, `FireDirection`, `FirePalette`, `FireDescriptor`, `PixelEffectBase`, UI/MVVM code, serialized data, or public APIs unless implementation evidence establishes a separate documented need.
  Rationale: inherited target-positioning storage and the required protected virtual override already exist, so expanding the scope would add compatibility risk without serving VIX-1119.
  Date/Author: 2026-08-24 / Codex

## Outcomes & Retrospective

Planning is complete; implementation has not started. When implementation completes, replace this paragraph with the observed location-rendering behavior, focused/full test results, benchmark results, manual-validation result, Jira status, remaining gaps, and lessons learned compared with the Purpose section.

## Context and Orientation

Vixen is a WPF application for sequencing animated lights. Effects are modules under `src/Vixen.Modules/Effect`. A pixel effect first renders a two-dimensional field of colors, then Vixen creates lighting intents for the selected elements.

`src/Vixen.Modules/Effect/Fire/Fire.cs` contains `VixenModules.Effect.Fire.Fire`, the runtime Fire effect. Its current `SetupRender()` allocates `_fireBuffer`, an integer heat field. For each string-rendered frame, `RenderEffect(int, IPixelFrameBuffer)` first fills row zero with randomized source heat, then builds every later row from the previous row. The neighbor calculation intentionally samples the center cell twice. It selects the heat field dimensions from `BufferWi`/`BufferHt`, swapping simulation width and height for `Left` and `Right`, then performs a forward projection to the output buffer. It evaluates `Height`, `HueShiftCurve`, and `LevelCurve` once per frame. `CleanUpRender()` releases the heat buffer.

`src/Vixen.Modules/Effect/Fire/FireData.cs` holds Fire settings. It inherits `EffectTypeModuleData`, which already serializes and clones `TargetPositioning`. `FireData.Location` is Fire's flame-edge direction; it must not be renamed or repurposed. Its legacy hue-shift migration and all data members remain unchanged.

`src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs` is the shared base class. `TargetPositioning = Strings` selects the existing dense string rendering flow. `TargetPositioning = Locations` calls `ConfigureVirtualBuffer()`, which collects leaf target locations, computes their enclosing preview rectangle, and later calls the derived `RenderEffectByLocation(int, PixelLocationFrameBuffer)` method. The base implementation throws, which is why Fire needs its override. `EnableTargetPositioning(true, true)` makes the inherited setup property visible. `UpdateStringOrientationAttributes(true)` makes `StringOrientation` visible only in string mode and refreshes type-descriptor metadata for property-grid consumers.

The logical output coordinate system used by pixel-effect math has `(0, 0)` at the lower-left. Preview Y coordinates increase in the opposite direction. In a virtual rectangle, an absolute location `(absoluteX, absoluteY)` converts to logical output coordinates as `outputX = absoluteX - BufferWiOffset` and `outputY = previewYMax - absoluteY`. The equivalent base-class formula is:

    outputY = Math.Abs((BufferHtOffset - absoluteY) + (BufferHt - 1 + BufferHtOffset));
    outputY -= BufferHtOffset;

The implementation must use the formula or the explicitly equivalent min/max expression, with the correct existing `PixelEffectBase` offsets. The test fixture must verify nonzero X and Y offsets so a swapped offset cannot pass unnoticed.

`src/Vixen.Modules/Effect/Effect/Location/PixelLocationFrameBuffer.cs` is the location-mode output buffer. It stores only actual element preview coordinates; it is not a dense virtual image. For every actual location, Fire must write using the original absolute coordinate. Duplicate locations intentionally share one sparse cell.

`src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs` demonstrates the existing reflection-based test style for protected rendering methods and the location frame buffer. `src/Vixen.Tests/Vixen.Tests.csproj` already references the Location property module but does not reference Fire. Add only the Fire module project reference required for direct Fire tests, following existing project-reference style.

## Plan of Work

Begin with the issue-tracker record. Read `.agents/skills/jira/SKILL.md` before using Jira. If authenticated Jira access is available, update VIX-1119 before code changes using the user-facing scope in the contract: one continuous preview-location simulation, independent flame-origin direction, compatibility with existing string effects, test coverage, and the required performance evidence. If access remains unavailable, do not fabricate an update; record the failed access and keep the plan's implementation scope authoritative.

Create `src/Vixen.Tests/Effects/FireLocationRenderTests.cs` and add `..\\Vixen.Modules\\Effect\\Fire\\Fire.csproj` to `src/Vixen.Tests/Vixen.Tests.csproj`. Use xUnit v3 and model protected-method invocation and virtual buffer setup on `SpiralLocationRenderTests`. Keep tests deterministic: never compare two independently random Fire runs. Prefer a private test-only reflection seam around a completed heat field, or a narrow internal seam only if it materially clarifies the production design and does not enter the hot loop. If internals are exposed, document why in this Decision Log and preserve the no-public-API constraint.

First characterize representative existing string output for every `FireDirection`. The characterization must use a deterministic source/heat seam and prove the center sample is duplicated, the `Height` clamp and integer `step` behavior are retained, palette index handling is retained, hue adjustment occurs only for positive shifts, and level scaling is retained. The tests must establish parity before refactoring the string projection; a changed random sequence cannot be accepted merely because output looks like fire.

In `Fire.cs`, initialize `_data`, call `EnableTargetPositioning(true, true)`, and refresh orientation attributes through a small private helper such as `InitAllAttributes()`. Use that helper in both the constructor and `ModuleData` setter after assigning new `FireData`. Verify that `TargetPositioning` becomes browsable, `StringOrientation` is visible in `Strings`, hidden in `Locations`, and remains hidden when a deserialized/replaced `FireData` already selects locations. Do not add a Fire-specific target-positioning property.

Refactor only private Fire implementation details into three responsibilities: create a frame state (interval position, effective height, hue shift, and level), generate the dense integer heat field, and project a logical output coordinate to an HSV color. The dense heat generator must retain this exact semantic traversal: source-row X from zero through simulation width minus one; later rows Y from one upward and X left-to-right; every source and propagation `Rand()` call in the same conditions and order as the current code. It must retain the duplicated `GetFireBuffer(x, y - 1, ...)` sample, integer averaging, existing clamp behavior, `Height <= 0` clamped to one, and potentially zero integer `step` for large values. It must generate the complete field exactly once for each rendered frame.

Use one inverse direction mapper for output sampling. Given logical lower-left output `(outputX, outputY)`, select simulation dimensions and coordinates as follows: Bottom uses width `BufferWi`, height `BufferHt`, and `(outputX, outputY)`; Top uses the same dimensions and `(outputX, BufferHt - outputY - 1)`; Left uses width `BufferHt`, height `BufferWi`, and `(outputY, outputX)`; Right uses the swapped dimensions and `(outputY, BufferWi - outputX - 1)`. This is the inverse of the current forward string projection. It prevents four separate location-render loops and places sources at the named edge. The pixel evaluator returns no output for heat index zero; otherwise it reads `FirePalette`, applies hue only when `hueShift > 0`, multiplies HSV value by `level`, and returns an HSV color.

Either change string rendering to use the shared inverse mapper or retain the current forward output loop. A shared mapper is permitted only if deterministic parity tests prove unchanged output for all directions and representative values. Do not duplicate the dense heat-generation code. If parity cannot be proven, retain the current string projection and call the inverse mapper only from location rendering; record that decision and the evidence.

Add `using VixenModules.Effect.Effect.Location;` and implement the inherited protected override `RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)` in `Fire.cs`. Return without rendering for nonpositive frame count or nonpositive virtual dimensions. For frames zero through `numFrames - 1`, set `frameBuffer.CurrentFrame`, create the frame state, generate the dense heat field once, iterate `frameBuffer.ElementLocations` once, translate its absolute preview coordinate to logical output coordinates, map those coordinates inversely for the selected direction, evaluate heat, and call `frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsv)` only when it is lit. Do not allocate `PixelFrameBuffer`, an RGB array for the virtual rectangle, or a second dense color field. Missing output positions remain the sparse buffer's default black/transparent state.

Keep `SetupRender()` ownership of a fresh zeroed dense heat array and `CleanUpRender()` ownership of releasing it. If validation requires a new multiplication or dimension calculation, check it safely before allocating, but do not introduce a preview-size cap without performance evidence and a separately accepted user behavior. Rendering remains sequential per effect instance: rows depend on preceding rows and parallel execution would change random order. Do not add locks, tasks, static mutable data, or UI-thread work.

Add deterministic tests that exercise the real dense generator and both projections of the same heat field. `Fire_DefaultConstructor_EnablesTargetPositioning` verifies setup-property visibility and toggling. `Fire_RenderEffectByLocation_DoesNotThrow` invokes the protected location path with a nonempty rectangle. `Fire_LocationProjection_RectangularGridMatchesStringProjection` runs Bottom, Top, Left, and Right and compares every grid location to string output after preview-Y inversion. `Fire_LocationRender_OriginatesAtSelectedEdge` verifies each direction maps the source row to its named edge. `Fire_LocationRender_SparseCoordinatesSampleDenseHeatField` uses offset, noncontiguous locations and proves only their sparse cells are stored while an output depends on a missing intermediate virtual coordinate. `Fire_LocationRender_AppliesHeightHueAndLevel` characterizes Height propagation plus hue and value changes, including zero level producing black. `Fire_LocationRender_MultipleFramesAdvanceAndStayInBounds` covers more than one frame and 1-by-N/N-by-1 virtual rectangles in all directions. `Fire_ModuleData_PreservesLocationAttributeState` replaces `ModuleData` with location positioning and confirms the refreshed `StringOrientation` state. Add a string-regression test covering the same representative settings and all four directions.

Measure performance in Release after behavior passes. Use an additive test/benchmark harness that reports elapsed wall-clock time and `GC.GetAllocatedBytesForCurrentThread()` (or the repository's established equivalent) with warm-up and repeat counts recorded. Measure: a 50 by 50 dense rectangle with 2,500 locations over 100 frames; 5,000 locations over a 1,000 by 500 rectangle for 20 frames; and 20,000 locations over a 2,000 by 1,000 rectangle for 3 frames. Report the proposed dense-heat/sparse-projection path and, if practical, a comparison prototype that additionally renders into a dense `PixelFrameBuffer` before sparse sampling. A dependency-pruned heat prototype is allowed only if the proposed path is materially too slow; it cannot be adopted unless it exactly reproduces dense results and random behavior. Remove throwaway prototype code unless it is retained as a meaningful regression benchmark. Record command, machine/runtime context, timings, allocations, and conclusion in this plan's living sections.

Finish by running the focused tests and the full Vixen test flow using full MSBuild before `dotnet test --no-build`, as required for the C++/CLI transitive test dependencies. Manually exercise the effect in the application with separated preview props. Read `.agents/skills/commit-msg/SKILL.md` before every milestone-completion response that changes repository files and include its formatted proposed commit message in that response; do not create a commit unless explicitly requested. Then use the Jira skill to update VIX-1119 with final scope adjustments, validation results, and a comment when access is available.

## Milestones

### Milestone 1: Record the implementation contract in Jira

Before changing repository code, read the project Jira skill and retrieve VIX-1119. Update its description with the user-visible purpose, independent meanings of `TargetPositioning` and Fire `Location`, dense-field/sparse-projection design, compatibility restrictions, test plan, performance scenarios, and acceptance criteria from this plan. The result is an issue that a reviewer can use without reading source. If access is unavailable, record the exact limitation in `Surprises & Discoveries`, leave this milestone pending, and proceed only because the local contract is complete.

### Milestone 2: Establish deterministic characterization and test access

Add the Fire project reference and focused Fire location test file. Implement the deterministic seam and initial tests that fail before production changes: hidden target positioning, missing location override, and baseline string characterization in all directions. Establish correct nonzero-offset virtual-buffer setup so tests use actual coordinate semantics. Run the focused filter and record the expected pre-change failures. The result is a reliable regression harness that does not depend on unrelated thread-local random sequences.

### Milestone 3: Enable the inherited configuration without changing data contracts

Change only Fire's construction and module-data replacement behavior to expose inherited target positioning and refresh orientation attributes. Test `Strings` versus `Locations`, the independent Fire direction property, and module-data replacement. No data contract, descriptor, shared infrastructure, UI, or public API may change. Run the focused tests and confirm existing default Fire data still has `Strings` positioning.

### Milestone 4: Separate heat generation from output projection while preserving strings

Extract private frame-state, dense-generation, coordinate-mapping, and color-evaluation helpers in `Fire.cs`. Preserve all source/propagation iteration and random semantics. Keep or carefully refactor the string output loop only after deterministic parity passes for Bottom, Top, Left, and Right. The result is one generation algorithm whose exact heat field can be projected by the existing string path and later by locations. Run string-regression and direction tests after each small refactor; record any decision to retain the original forward projection.

### Milestone 5: Implement sparse preview-location projection

Implement `RenderEffectByLocation` with one dense heat generation per frame and one pass over actual element locations. Translate absolute preview coordinates to lower-left logical coordinates, apply the inverse direction map, and write output only at absolute element keys. Add and run no-throw, rectangular parity, directional edge, sparse-gap, controls, multi-frame, narrow-buffer, and duplicate-location tests. The result is continuous Fire across gaps without a dense color buffer.

### Milestone 6: Collect performance evidence and make only evidence-backed changes

Run the three required Release scenarios with the production dense-heat/sparse-projection implementation. When practical, compare it with a dense-color-buffer projection prototype. Preserve the production design unless measurements show a material issue; if investigating dependency pruning, first prove bit-for-bit/deterministic equivalence and unchanged random order. Record measurements and conclusion in `Surprises & Discoveries` and `Decision Log`. The result is evidence that the normal location path avoids unnecessary dense color conversion while accepting Fire's unavoidable virtual-area simulation cost.

### Milestone 7: Validate end to end and close the tracking loop

Run the focused Fire tests and then the complete test workflow. In the UI, create or open two or more visibly separated props with preview locations, group them, add Fire, select `Locations`, verify `StringOrientation` hides, scrub with visible Height/Hue/Brightness curves, and verify one continuous simulation. Repeat Bottom, Top, Left, and Right; switch back to `Strings`; save and reopen to verify direction and positioning persist independently. Update Jira with final requirements changes if any and a validation comment. Complete Progress, Outcomes, and the revision note below.

## Concrete Steps

All commands below run from `C:\Dev\Vixen` in PowerShell. Use `rg` before editing to confirm method locations and use `apply_patch` for source and plan edits. Do not alter unrelated dirty files; the supplied contract is currently untracked and must be preserved.

For focused development validation after the Fire module and tests build:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-restore --filter FullyQualifiedName~FireLocation

Expected result after Milestone 5 is a successful xUnit run containing the Fire location tests, with no `NotImplementedException`, no out-of-range heat access, and every theory direction passing. Before implementation, record the specific expected characterization failures rather than treating a build failure as evidence.

For the required full test flow, first build the C++/CLI-dependent test target with Visual Studio MSBuild, then run the already-built test assembly:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\"

Expected success is MSBuild exit code zero followed by a passing Vixen test suite. Record test totals and pre-existing warnings separately; do not hide or attribute unrelated warnings to VIX-1119.

For Release performance measurements, use the focused benchmark filter created in the Fire test file or a dedicated additive test helper. Run the exact command recorded by the benchmark harness, for example:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-restore --filter FullyQualifiedName~FireLocationBenchmark --logger "console;verbosity=detailed"

Record a compact transcript in this plan in the form below, replacing placeholders with observed values:

    Runtime: .NET <version>; configuration: Release; machine: <CPU/RAM summary>
    Dense 50x50 / 2,500 locations / 100 frames: <ms>; <bytes> allocated
    Sparse 1,000x500 / 5,000 locations / 20 frames: <ms>; <bytes> allocated
    Sparse 2,000x1,000 / 20,000 locations / 3 frames: <ms>; <bytes> allocated
    Dense-color comparison (if run): <ms>; <bytes> allocated
    Conclusion: <whether the proposed path remains selected and why>

## Validation and Acceptance

VIX-1119 is accepted only when Fire exposes the inherited `TargetPositioning` property and a user can select `Locations` without an exception. A group of elements across multiple preview props must render as one fire simulation over their shared preview rectangle, not one simulation per prop. Empty areas between props must affect later heat propagation even though no output is stored there.

For a full rectangular grid representing the same virtual rectangle, location output must match string output at every coordinate after the preview-Y inversion, using the same deterministic heat field, for Bottom, Top, Left, and Right. The source heat row must appear at the matching visual edge in every direction. Existing representative string-mode output must remain unchanged.

Only actual location coordinates may receive sparse output. Normal location rendering may hold its one dense integer heat field but must not allocate a dense `PixelFrameBuffer` or convert the whole virtual field into a dense RGB output. The documented benchmark evidence must cover all three required layouts and state this allocation conclusion.

Height, hue shift, and brightness/level curves must alter location output consistently with string output. Multiple frames and narrow rectangles must complete without out-of-range accesses. Existing effects must retain `Strings` default behavior; existing Fire `Location` direction must persist independently from target positioning. Focused tests, full-suite validation, and manual UI checks must be recorded in this plan and Jira when available.

## Idempotence and Recovery

The source edits are additive or private refactors and may be reapplied after checking current method bodies with `rg`. Re-run targeted tests after every refactor; if string parity changes, revert only the incomplete private refactor with a targeted patch and retain the known-good forward string projection while investigating. Do not reset the worktree or delete the untracked implementation contract.

If test project build fails because C++/CLI dependencies are not built, rerun the full-MSBuild command above, then repeat `dotnet test --no-build`. If Jira authentication is unavailable, defer the remote update and record it; do not block local implementation or invent issue contents. If a benchmark prototype harms behavior or cannot retain deterministic output, remove only the prototype before final validation and leave the production dense-heat/sparse-projection path intact.

## Artifacts and Notes

The required inverse mapping for a lower-left logical output coordinate is:

    Bottom: simulation size BufferWi by BufferHt; simulation coordinate (outputX, outputY)
    Top:    simulation size BufferWi by BufferHt; simulation coordinate (outputX, BufferHt - outputY - 1)
    Left:   simulation size BufferHt by BufferWi; simulation coordinate (outputY, outputX)
    Right:  simulation size BufferHt by BufferWi; simulation coordinate (outputY, BufferWi - outputX - 1)

The generator's compatibility-critical behavior is:

    source-row order: x increases from 0 to maxWi - 1
    propagation order: y increases from 1 to maxHt - 1, with x increasing left to right
    previous-row samples: x - 1, x + 1, x, x (center deliberately duplicated)
    Height <= 0: use 1; step remains integer 255 * 100 / maxHt / effectiveHeight
    hue: apply only when hueShift > 0; value: multiply by level
    heat index 0: do not write an output pixel

At final completion, add the exact test transcript, benchmark results, a concise manual-validation result, and the final Jira update/comment outcome here or in the living sections. Never place credentials, machine usernames, or unrelated full logs in this plan.

## Interfaces and Dependencies

The only new production member expected is the existing protected extension point already declared by `PixelEffectBase`:

    protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)

It belongs in `src/Vixen.Modules/Effect/Fire/Fire.cs` and must remain protected; no new public or protected API is needed. Its contract is to set `frameBuffer.CurrentFrame` for every requested frame and call `frameBuffer.SetPixel` at original absolute preview coordinates only for lit target locations.

Keep all helper types and methods used to create frame state, generate heat, transform coordinates, and evaluate palette color private to `Fire`. Reuse `VixenModules.Effect.Effect.Location.PixelLocationFrameBuffer`, `VixenModules.Effect.Effect.ElementLocation`, `FirePalette`, the existing `Curve` APIs, and the inherited buffer dimensions. Add only the Fire project reference to `Vixen.Tests.csproj`; use project references rather than DLLs and do not add projects or modify `Vixen.sln`.

No `FireData` members, no new serialization, no `FireDirection` change, no descriptor change, no `PixelEffectBase` change, no UI/MVVM change, and no shared infrastructure change is authorized by this plan. If an implementation block proves one is necessary, stop, document the concrete evidence and compatibility impact in `Decision Log`, and obtain separately documented scope before making that change. If a public or protected API beyond the override becomes necessary, read and apply `.agents/skills/csharp-docs/SKILL.md` and update XML documentation in the same change.

---

Revision note (2026-08-24): Initial ExecPlan created from the user-supplied VIX-1119 implementation contract. It records the current Fire and shared-location code structure, required deterministic preservation rules, tests, performance evidence, Jira workflow, and explicit scope boundaries before implementation begins.

Revision note (2026-08-24): Attempted Milestone 1. The Jira connection and VIX project edit permission are available, but VIX-1119 itself is not visible to the connected account, so no remote update was made and the milestone remains pending.

Revision note (2026-08-24): Retried Milestone 1 after VIX-1119 became visible. Updated the Jira description with the user-facing scope and acceptance criteria, marked the milestone complete, and retained the earlier temporary-access observation for traceability.

# VIX-3989: Add a compatibility-preserving PinWheel size scale basis

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain it in accordance with `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

PinWheel currently turns its percentage-based Size curve into a radius using only the virtual buffer height. On a wide preview rendered by element locations, this can leave the far-left and far-right props outside the effect even at a large Size value. After this change, newly created PinWheel effects will use the larger virtual-buffer dimension by default, while users can deliberately choose Height or Width when appropriate.

Existing sequences and saved effect defaults must look exactly as they did before this change. An old serialized payload has no new size-basis member; it must therefore resolve to Height, the current behavior. A user can observe the feature in the PinWheel property grid: `Size Basis` appears beside `Size` only while percentage offsets are enabled, and a new PinWheel on a wide location-based group can cover locations that a height-scaled PinWheel cannot.

## Progress

- [x] (2026-08-21) Created this ExecPlan from the VIX-3989 handoff after inspecting `docs/reviews/vix-3989-pinwheel-size-scale-basis-design.md`, the PinWheel effect/data code, the effect-editor resources, the current rendering-test seam, and the test project references.
- [x] (2026-08-21 20:05Z) Updated Jira VIX-3989 with the user-facing summary, scope, and acceptance criteria for compatibility-preserving Size Basis behavior. Used the project `jira` skill.
- [x] (2026-08-21) Added the serialized Size Basis contract, documented property-grid setting, localized display/description resources, and generated resource accessors. The shared radius-basis calculation remains Milestone 3 work.
- [x] (2026-08-21) Replaced the duplicated string/location radius selection with one private helper that selects the configured percentage basis and preserves the legacy absolute-offset corner-distance calculation.
- [x] (2026-08-21) Added the PinWheel test-project reference and 17 focused compatibility, rendering, and browsability tests; the Release/x64 focused suite passes.
- [ ] Run the focused and complete x64 test workflows, manually validate in the UI, update Jira, and record actual evidence here.

## Surprises & Discoveries

- Observation: `PinWheel` currently calculates the same radius basis independently in both dense string rendering and location rendering.
  Evidence: `src/Vixen.Modules/Effect/PinWheel/PinWheel.cs` uses `OffsetPercentage ? BufferHt : DistanceFromPoint(...)` in `RenderEffect` and again in `RenderEffectByLocation`.

- Observation: The PinWheel module is already in `Vixen.sln`, but `src/Vixen.Tests/Vixen.Tests.csproj` does not reference `src/Vixen.Modules/Effect/PinWheel/PinWheel.csproj`.
  Evidence: `Vixen.sln` contains the PinWheel project; the test project's reference list includes Spiral and other effects but no PinWheel reference.

- Observation: Pixel frame buffers use `System.Drawing.Color` even though their source files also import the HSV color-model namespace.
  Evidence: `src/Vixen.Modules/Effect/Effect/PixelFrameBuffer.cs` and `PixelLocationFrameBuffer.cs` expose `Color` values compatible with the existing `System.Drawing.Color` assertions in location-render tests.

## Decision Log

- Decision: Use a three-value public enum with `Height = 0`, `Width = 1`, and `LargestDimension = 2`.
  Rationale: `DataContractSerializer` supplies the CLR default for an absent member without running the data class constructor. Reserving zero for Height makes old payloads retain the old height-based radius, while a constructor default can give genuinely new effects the corrected Largest Dimension behavior.
  Date/Author: 2026-08-21 / Codex

- Decision: Do not migrate data or assign `SizeScaleBasis` in `OnDeserialized`.
  Rationale: Buffer dimensions are render-target-specific and unavailable during deserialization. More importantly, assigning the new-effect default on deserialization would destroy the missing-member compatibility discriminator.
  Date/Author: 2026-08-21 / Codex

- Decision: Centralize radius-basis selection in one private PinWheel helper and retain the absolute-offset branch byte-for-byte in meaning.
  Rationale: Both render paths must remain behaviorally identical. The old non-percentage branch uses the distance from the calculated origin to the bottom-right buffer corner and is unrelated to this correction.
  Date/Author: 2026-08-21 / Codex

- Decision: Use a property-grid enum value rather than a hidden version flag or unconditional Largest Dimension behavior.
  Rationale: It preserves old output, fixes the new-effect experience, and lets a user intentionally select an axis for a particular display without recreating an effect.
  Date/Author: 2026-08-21 / Codex

## Outcomes & Retrospective

Implementation has not begun. The intended completed outcome is a localized, documented Size Basis setting with safe old-data behavior, parity between both PinWheel render paths, and automated evidence for serialization, cloning, buffer shapes, visibility, and legacy absolute offsets. Record actual results, remaining gaps, test counts, and manual validation here when the work finishes.

## Context and Orientation

`src/Vixen.Modules/Effect/PinWheel/PinWheel.cs` is the public effect implementation. Its Config properties are annotated with `Value`, `ProviderCategory`, `ProviderDisplayName`, `ProviderDescription`, and `PropertyOrder` so the Effect Editor property grid can display and edit them. `UpdateOffsetAttribute` controls dynamic browsability with `SetBrowsable`, and the `OffsetPercentage` setter refreshes that state. `RenderEffect` paints every cell in a normal pixel frame buffer; `RenderEffectByLocation` paints only actual element locations in a `PixelLocationFrameBuffer`. Both calculate a frame origin, a Size factor, and a maximum radius before entering their pixel/location loops.

`src/Vixen.Modules/Effect/PinWheel/PinWheelData.cs` is the data contract serialized with each PinWheel effect. Its parameterless constructor establishes defaults only for genuinely newly constructed data. `DataContractSerializer` does not use that constructor when reading a serialized object; a `[DataMember]` absent from an older XML payload retains its CLR default. `OnDeserialized` is a legacy upgrade hook and must not touch the new member. `CreateInstanceForClone` explicitly makes independent copies of the existing data and is the place to preserve the selected enum value when an effect is cloned.

The new enum belongs in `src/Vixen.Modules/Effect/PinWheel/PinWheelSizeScaleBasis.cs`, in the `VixenModules.Effect.PinWheel` namespace. Because it is public, and because the new public `PinWheelData` and `PinWheel` properties are public APIs, their XML documentation must be added or updated in the same change.

`src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDisplayNameDescriptors.resx` and `EffectDescriptionDescriptors.resx` are the localized resource sources resolved by the provider attributes. Their matching `.Designer.cs` files are generated accessors and must be regenerated by the repository's established resource-generation process after adding the `SizeBasis` display and description resources. Do not hand-edit generated output unless the established project tooling itself does so.

`src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs` is the closest existing location-render test pattern. It sets private `PixelEffectBase` virtual-buffer fields through reflection, invokes protected rendering methods by reflection, and compares dense and location results. Add `PinWheel.csproj` to `src/Vixen.Tests/Vixen.Tests.csproj` using the repository's standard project-reference metadata, then create a focused `PinWheelSizeScaleBasisTests.cs` under `src/Vixen.Tests/Effects/` following that seam. Use a deterministic standard color, one arm, full thickness, fixed curves, and one frame so assertions do not depend on random colors or animation.

`OffsetPercentage` has two meanings. When true, X/Y offsets are normalized to the virtual buffer and Size must use the selected scale basis. When false, offsets are legacy absolute values and radius must remain the current distance from the calculated origin to `(BufferWiOffset + BufferWi, BufferHtOffset + BufferHt)`. `BufferWi` is the buffer width and `BufferHt` is the buffer height; Width chooses the former, Height chooses the latter, and Largest Dimension chooses `Math.Max(BufferWi, BufferHt)`.

## Plan of Work

### Milestone 1: Record the approved contract in Jira

Before modifying source, use the repository `jira` skill to update VIX-3989. State the user-visible correction, the compatibility guarantee for payloads missing the new member, the exact enum values, the new-effect default, the `OffsetPercentage` visibility rule, the unchanged absolute-offset branch, the localization/XML documentation requirements, and the focused plus full validation plan from this ExecPlan. If Jira access is unavailable, capture the exact failure in `Surprises & Discoveries`, continue with local implementation, and leave the final tracker update pending.

### Milestone 2: Add the data contract and property-grid contract

Read the complete current `PinWheel.cs`, `PinWheelData.cs`, PinWheel project file, resource files, and generated-resource workflow before editing. Read `.agents/skills/csharp-docs/SKILL.md` because this milestone creates or modifies public APIs, and read `.agents/skills/dotnet-best-practices/SKILL.md` before writing the C# changes.

Create `PinWheelSizeScaleBasis.cs` with a public documented enum. Assign values explicitly and in this exact order:

    Height = 0
    Width = 1
    LargestDimension = 2

Document that Height is the compatibility value for effects serialized before VIX-3989, Width scales by the virtual buffer width, and Largest Dimension scales by the larger buffer dimension. Do not reorder values or let an implicit future edit change Height's zero value.

In `PinWheelData.cs`, add a documented `[DataMember] public PinWheelSizeScaleBasis SizeScaleBasis { get; set; }`. In the parameterless constructor explicitly assign `SizeScaleBasis = PinWheelSizeScaleBasis.LargestDimension;`. Do not add `EmitDefaultValue = false`; after an older effect is loaded and saved, serializing its resolved Height value makes its legacy intent durable. Do not set this property in `OnDeserialized`. Include `SizeScaleBasis = SizeScaleBasis` in the `CreateInstanceForClone` initializer so a clone retains the selected behavior rather than receiving the constructor default.

In `PinWheel.cs`, add a documented `[Value]` Config property named `SizeScaleBasis` adjacent to `SizeCurve`, ordered immediately after it and shifting only later Config ordering as needed. It must get and set `_data.SizeScaleBasis`; the setter must set `IsDirty = true` and call `OnPropertyChanged()` like neighboring properties. Use `ProviderDisplayName("SizeBasis")` and `ProviderDescription("SizeBasis")`. The localized display value must be `Size Basis`; the description must explain that it chooses the preview dimension used to scale Size and that Largest Dimension is normally the best choice. Add those keys to the two resource `.resx` files and regenerate/update their strongly typed designer accessors through the established process.

Modify `UpdateOffsetAttribute` so `SizeScaleBasis` is browsable only when `OffsetPercentage` is true. Keep the existing X/Y offset behavior: X/Y offsets are browsable only when `OffsetPercentage` is false. Use `nameof(OffsetPercentage)`, `nameof(SizeScaleBasis)`, `nameof(XOffsetCurve)`, and `nameof(YOffsetCurve)` for keys touched in this method, rather than new string literals. Preserve the refresh behavior: toggling `OffsetPercentage` must update visible properties immediately.

### Milestone 3: Centralize and apply the radius-basis calculation

In `PinWheel.cs`, replace only the duplicated local `xc` selection in `RenderEffect` and `RenderEffectByLocation` with a single private helper, for example `CalculateRadiusBasis(Point origin)`. Call it after calculating the origin and before calculating `maxRadius`; retain the existing Size factor and all pixel math. The helper must return a `double` or another type that preserves the existing multiplication behavior.

Its first branch is non-negotiable:

    if (!OffsetPercentage)
        return DistanceFromPoint(origin, new Point(BufferWiOffset + BufferWi, BufferHtOffset + BufferHt));

For percentage offsets, use a `switch` over `SizeScaleBasis`: Height returns `BufferHt`; Width returns `BufferWi`; Largest Dimension returns `Math.Max(BufferWi, BufferHt)`; and the default/unknown enum value returns `BufferHt`. This defensive fallback preserves the legacy-safe dimension if corrupt or future data holds an unknown numeric enum value. Do not alter `CalculateSize`, curve ranges, origin calculations, offsets, center-hub handling, twist math, color behavior, descriptor version, sequence migration, Catel view models, commands, or services. Calculate once per rendered frame outside every pixel/location loop.

### Milestone 4: Establish focused compatibility and rendering evidence

Add the PinWheel project reference to `src/Vixen.Tests/Vixen.Tests.csproj`, following existing project-reference conventions: a project reference rather than a DLL, no new package, no duplicate reference, and the same copy/local asset behavior used by neighboring effect project references. Do not alter solution platform mappings because the project already exists in `Vixen.sln`.

Create `src/Vixen.Tests/Effects/PinWheelSizeScaleBasisTests.cs` and use xUnit. Keep the test class narrowly focused on VIX-3989. Test data-contract behavior using `DataContractSerializer` with in-memory XML streams, not the application persistence service, so the missing-member rule is proved directly. Include the following independently meaningful tests:

1. `new PinWheelData()` assigns `LargestDimension`.
2. Deserialize a valid PinWheel XML payload produced without a `SizeScaleBasis` element and assert Height, proving old XML retains current output. Build the fixture from a baseline serialization or a concise valid contract fixture, removing only the new member; do not deserialize an incomplete object that bypasses ordinary contract requirements.
3. For Height, Width, and Largest Dimension, serialize and deserialize then assert the exact selected enum survives. Also assert the serialized XML includes the member when it resolves to Height, so it is not accidentally suppressed.
4. For each enum value, clone a configured `PinWheelData` through its normal clone API and assert the clone has the same enum value; also ensure the test does not mistake the constructor's Largest Dimension default for an actual copy.
5. Verify the property grid with `TypeDescriptor.GetProperties(new PinWheel())`: `SizeScaleBasis` is browsable when `OffsetPercentage` is true and not browsable after setting it false. In the same test, assert the existing X/Y offset visibility remains inverse to prevent accidental regression.
6. On a wide virtual buffer, prove Height reproduces the old height-based reach and that Width and Largest Dimension can light a location whose distance is beyond the height-scaled maximum but within the width-scaled maximum. Use an asymmetric rectangle and deterministic values that avoid a radius-boundary equality.
7. On a tall buffer, prove Largest Dimension matches Height and differs from Width where expected. On a square buffer, prove Height, Width, and Largest Dimension produce identical results.
8. For each basis on a dense rectangular grid, compare `RenderEffect` output with `RenderEffectByLocation` output at matching coordinates, applying PinWheel's existing Y inversion when reading the dense buffer. This verifies both paths use the same helper rather than separately drifting.
9. With `OffsetPercentage = false`, use a noncentral origin and multiple distinct Size Basis values, then assert identical dense and location outputs. This characterizes the unchanged bottom-right-distance legacy branch.

Use reflection only for the existing protected/private render seam: set the same `PixelEffectBase` buffer fields that the Spiral tests set, invoke `SetupRender` if required, invoke `RenderEffect(int, IPixelFrameBuffer)`, and invoke `RenderEffectByLocation(int, PixelLocationFrameBuffer)`. Correctly map the backing fields by their names, not by inferred width/height ordering. Build full grids of `ElementLocation` test nodes with a minimal `IElementNode`/`PropertyManager` setup as Spiral does. For every comparison, use a fixed standard color and compare RGB values or explicit lit/unlit state; do not use PinWheel random colors.

### Milestone 5: Validate, inspect, and close the tracker loop

First run the focused tests after building the Visual Studio/MSBuild x64 test target. The test graph has C++/CLI transitive dependencies, so `dotnet test` alone is insufficient to build it. From `C:\Dev\Vixen`, run:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\" --filter FullyQualifiedName~PinWheelSizeScaleBasisTests
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\"
    git diff --check

Expect the focused and full test commands to report zero failed tests. If an environmental C++ toolset/MSBuild failure prevents the first command, record the complete failure and retry on a Visual Studio-equipped machine; do not replace the required validation with `dotnet test` building from scratch.

Manually create a location-positioned PinWheel on a deliberately wide group in Vixen. Confirm a newly added effect has `Size Basis = Largest Dimension`, its Size Basis property is visible while `OffsetPercentage` is enabled, and it can reach preview corners that height-only scaling misses. Change Size Basis to Height and confirm the prior limited reach returns; Width should match Largest Dimension for a wide buffer. Disable `OffsetPercentage` and confirm Size Basis is hidden, X/Y offset curves reappear, and changing the stored basis does not change the rendered result. Load an older sequence or compatible old serialized PinWheel fixture, confirm it resolves to Height with no visual change, then save and reload it to confirm Height persists.

After validation, use the `jira` skill to update VIX-3989's description if discoveries changed requirements and add a comment containing the actual build command, focused/full test results, manual observations, and any known gap. Update all living-plan sections, including Progress, Outcomes & Retrospective, Artifacts, and the revision note, with evidence rather than this plan's examples.

## Concrete Steps

All commands run from `C:\Dev\Vixen`.

1. Protect current user work and read the relevant contracts before changing files:

       git status --short
       git diff -- src/Vixen.Modules/Effect/PinWheel/PinWheel.cs
       Get-Content -Raw docs/reviews/vix-3989-pinwheel-size-scale-basis-design.md
       Get-Content -Raw src/Vixen.Modules/Effect/PinWheel/PinWheel.cs
       Get-Content -Raw src/Vixen.Modules/Effect/PinWheel/PinWheelData.cs
       Get-Content -Raw .agents/skills/csharp-docs/SKILL.md
       Get-Content -Raw .agents/skills/dotnet-best-practices/SKILL.md

2. Locate resource and test conventions before creating the enum, resource entries, tests, and project reference:

       rg -n -C 3 "OffsetPercentage|PinWheelRotation|<data name=\"Size\"" src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes -g "*.resx" -g "*.cs"
       Get-Content -Raw src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs
       Get-Content -Raw src/Vixen.Tests/Vixen.Tests.csproj
       Get-Content -Raw src/Vixen.Modules/Effect/PinWheel/PinWheel.csproj

3. Complete Milestones 2 through 4 with tab indentation and LF line endings. Review only the expected change set:

       git diff --check
       git diff -- src/Vixen.Modules/Effect/PinWheel/PinWheel.cs src/Vixen.Modules/Effect/PinWheel/PinWheelData.cs src/Vixen.Modules/Effect/PinWheel/PinWheelSizeScaleBasis.cs src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDisplayNameDescriptors.resx src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDescriptionDescriptors.resx src/Vixen.Tests/Effects/PinWheelSizeScaleBasisTests.cs src/Vixen.Tests/Vixen.Tests.csproj

4. Run the three Milestone 5 validation commands. A successful test summary contains a zero failure count, for example:

       Passed!  - Failed:     0, Passed:     <count>, Skipped:     0

5. Record the actual command output summary, test counts, Jira result, and manual scenario result in this document before handing work off.

## Validation and Acceptance

The change is accepted when a new PinWheel data object and a new PinWheel UI instance use Largest Dimension, while a valid pre-VIX-3989 XML payload that lacks `SizeScaleBasis` resolves to Height. Every enum choice must survive both data-contract serialization and normal cloning. The public enum and public properties must have accurate XML documentation, and the effect editor must display localized `Size Basis` text and an explanatory description.

On a wide rectangle, Height must retain the old height-limited radius; Width and Largest Dimension must use the width and be able to illuminate a location beyond that old range. On a tall rectangle, Largest Dimension must equal Height; on a square rectangle all choices must agree. For equivalent grids, dense string rendering and location rendering must output the same pixels after accounting for the existing dense-buffer Y inversion. The private helper must retain the bottom-right-distance calculation when `OffsetPercentage` is false, and changing the enum in that mode must have no rendered effect.

`SizeScaleBasis` must be browsable only when `OffsetPercentage` is true. Existing X/Y offset browsability must remain the inverse. The focused `PinWheelSizeScaleBasisTests` and the complete `Vixen.Tests` suite must pass with zero failures after the required Release/x64 MSBuild test-target build. The manual wide-preview and old-sequence scenarios in Milestone 5 must pass.

## Idempotence and Recovery

The planned source edits are additive and can be retried after a failed build. Data-contract tests operate only in memory and render tests use isolated buffers, so they do not alter sequences or effect defaults. Re-running an MSBuild restore/test is safe. Once an old payload is saved after implementation it will explicitly contain Height, which is intentional and preserves its historical rendering thereafter.

If a source edit must be backed out, remove only the new enum, SizeScaleBasis data/UI/helper code, localized resource entries and generated accessors, PinWheel test file, and PinWheel test-project reference after confirming their exact paths with `git status --short`. Do not reset, checkout, delete, or broadly reformat `PinWheel.cs`. If resource designer generation is unavailable, identify the established project command before editing generated files and record the tooling limitation rather than leaving resources and designers out of sync.

## Artifacts and Notes

The required completed public contract is:

    public enum PinWheelSizeScaleBasis
    {
        Height = 0,
        Width = 1,
        LargestDimension = 2
    }

    [DataMember]
    public PinWheelSizeScaleBasis SizeScaleBasis { get; set; }

    [Value]
    public PinWheelSizeScaleBasis SizeScaleBasis { get; set; }

The intended shared radius-basis logic is:

    if (!OffsetPercentage)
        return DistanceFromPoint(origin, new Point(BufferWiOffset + BufferWi, BufferHtOffset + BufferHt));

    return SizeScaleBasis switch
    {
        PinWheelSizeScaleBasis.Height => BufferHt,
        PinWheelSizeScaleBasis.Width => BufferWi,
        PinWheelSizeScaleBasis.LargestDimension => Math.Max(BufferWi, BufferHt),
        _ => BufferHt
    };

This helper selects only the unscaled basis. Existing `CalculateSize(intervalPosFactor) / 100.0` remains the multiplier, so the maximum radius remains `basis * sizeFactor` and no Size curve is rewritten.

Milestone 2 validation evidence:

    msbuild src\Vixen.Modules\Effect\PinWheel\PinWheel.csproj -m -restore -t:Build -p:Configuration=Release -p:Platform=x64 -v:m
    Build succeeded with zero errors. The build reported existing warnings in Vixen.Core and FixtureGraphics.

Milestone 3 validation evidence:

    msbuild src\Vixen.Modules\Effect\PinWheel\PinWheel.csproj -m -t:Build -p:Configuration=Release -p:Platform=x64 -v:m
    Build succeeded with zero errors. The build reported existing warnings in Vixen.Core and FixtureGraphics.

Milestone 4 validation evidence:

    msbuild src\Vixen.Tests\Vixen.Tests.csproj -m -t:Build -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:q
    Build succeeded with zero errors.

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\" --filter FullyQualifiedName~PinWheelSizeScaleBasisTests
    Passed!  - Failed:     0, Passed:    17, Skipped:     0, Total:    17.

## Interfaces and Dependencies

No package, descriptor-version, sequence migration, Catel/ViewModel, service, or solution-platform change is required. The only new production type is the public `VixenModules.Effect.PinWheel.PinWheelSizeScaleBasis` enum. The existing public `PinWheelData.SizeScaleBasis` and `PinWheel.SizeScaleBasis` properties expose it. Use the existing `System.Runtime.Serialization.DataContractSerializer`, `System.ComponentModel.TypeDescriptor`, provider-resource attributes, `PixelEffectBase`, `IPixelFrameBuffer`, and `PixelLocationFrameBuffer` dependencies; do not introduce a new library.

The private helper remains an implementation detail in `PinWheel.cs`; tests may reach the existing render methods and virtual-buffer state through reflection, as `SpiralLocationRenderTests` does, but should not make the helper public solely for testing. The final test project must reference the existing PinWheel module project so it can instantiate the effect and data contract directly.

## Revision Note

2026-08-21: Initial ExecPlan created from the VIX-3989 handoff and repository design note. It resolves serialization defaults, clone behavior, public documentation, resource keys, render helper behavior, location/string parity, legacy absolute offsets, required Jira lifecycle, and full x64 validation so implementation can proceed without relying on prior conversation context.

2026-08-21: Completed Milestone 1 by updating VIX-3989 with a concise user-facing Summary, Scope, and Acceptance Criteria. Detailed design, compatibility constraints, and test steps remain in this repository-local ExecPlan.

2026-08-21: Completed Milestone 2 by adding the documented public `PinWheelSizeScaleBasis` enum, serialized `SizeScaleBasis` storage with the Largest Dimension new-effect default and clone preservation, and a localized Config property shown only for percentage offsets. The Release x64 PinWheel build completed with zero errors.

2026-08-21: Completed Milestone 3 by replacing the separate dense and location rendering radius-basis calculations with one private helper. It retains the absolute-offset bottom-right distance and selects Height, Width, or Largest Dimension for percentage offsets, with Height as the defensive fallback. The Release x64 PinWheel build completed with zero errors.

2026-08-21: Completed Milestone 4 by adding the PinWheel module test reference and 17 focused tests. The tests cover constructor and old-XML defaults, data-contract round trips, cloning, property visibility, wide/tall/square buffer selection, string/location parity, unknown values, and unchanged absolute offsets. The Release x64 focused test run passed with zero failures.

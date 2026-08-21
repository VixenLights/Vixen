# Architecture Design: VIX-3989 — Increase Pinwheel size when using location based rendering

**Issue:** [VIX-3989](https://vixenlights.atlassian.net/browse/VIX-3989) — Bug, Normal priority, status *New Ticket*
**Status:** Design complete, ready for specification and implementation
**Target spec path:** `docs/plans/effects/vix-3989-pinwheel-size-scale-basis.md`

## Purpose

Pinwheel currently derives its percentage-based maximum radius from `BufferHt`. On a wide
location-based preview, even the largest Size setting can therefore leave distant elements outside
the effect. The corrected behavior must allow the radius to use the larger buffer dimension without
changing the appearance of existing sequences.

## Evidence and constraints

- Both render paths calculate `xc` independently in `PinWheel.cs`: string rendering at lines
  430-434 and location rendering at lines 476-482. When `OffsetPercentage` is true, both use
  `BufferHt`.
- `CalculateSize` maps the Size curve to 1-400, and the render path divides that value by 100. The
  effective radius is therefore `basis * sizeFactor`, where `sizeFactor` ranges from 0.01 through
  4.00.
- `PixelEffectBase.ConfigureVirtualBuffer` determines `BufferWi`, `BufferHt`, and their offsets only
  during pre-render. Sequence data migration and `PinWheelData.OnDeserialized` cannot safely know
  which dimension will be larger.
- Effect module data is read with `DataContractSerializer`. A member absent from an older payload
  receives the CLR default; constructors are not used for deserialized instances. This provides a
  compatibility discriminator without a sequence migration.
- When `OffsetPercentage` is false, current behavior uses the distance from the origin to the
  bottom-right buffer corner. VIX-3989 should not alter that legacy branch.
- The working tree already contains a user-owned `Debug.WriteLine` change in `PinWheel.cs`; it is
  unrelated to this design and must not be overwritten during implementation.

## Ranked alternatives

| Rank | Alternative | Existing effects | New-effect experience | Assessment |
|---:|---|---|---|---|
| 1 | Three-state Size Basis: Height, Width, Largest Dimension | Missing data resolves to Height and renders exactly as today | Defaults to Largest Dimension; Width and Height remain available for intentional axis-based sizing | Best balance of compatibility, discoverability, and correct out-of-box behavior |
| 2 | Two-state X/Y selector, always defaulting to current Y/Height | Exact compatibility | Users must discover and change the setting on most wide previews; portrait previews require the opposite choice | Safe and simple, but the reported bug remains the default experience |
| 3 | Hidden legacy flag: old effects use Height, new effects use Largest Dimension | Exact compatibility | Correct for new effects, but an existing effect cannot opt into the fix without recreation or another command | Low UI complexity, poor upgrade experience |
| 4 | Automatically use Largest Dimension and compensate Size curves at first render | Attempts compatibility | Correct after conversion | Rejected: dimensions are target-dependent; lazy mutation during render creates dirty/undo problems, and animated curves plus clamping make compensation fragile |
| 5 | Unconditionally replace Height with Largest Dimension | Existing wide effects visibly change | Correct and simple | Rejected as a breaking sequence change |

## Core Strategy

Use an explicit Strategy value in `PinWheelData` to select the dimension used by percentage-based
Size scaling. The serialized value is an enum because the choices are closed, mutually exclusive,
and meaningful to users. Centralize the basis calculation in one helper so string and location
rendering cannot diverge.

The compatibility behavior relies on two intentionally different defaults:

1. `PinWheelSizeScaleBasis.Height` has numeric value zero. Older serialized effects do not contain
   the new member, so `DataContractSerializer` gives them Height and preserves their output.
2. `PinWheelData()` explicitly assigns `LargestDimension`. Effects newly created after VIX-3989
   therefore get the corrected behavior without user intervention.

This is not a data migration. It requires no buffer dimensions during deserialization and does not
rewrite the Size curve. When an old sequence is next saved, its resolved Height value is serialized
explicitly. Previously saved custom effect defaults also resolve to Height, preserving the user's
chosen appearance; new built-in Pinwheel instances use Largest Dimension.

## Data Model & Property Contracts

### `PinWheelSizeScaleBasis`

Add a public enum in its own `PinWheelSizeScaleBasis.cs` file:

```text
Height = 0              // compatibility value for data that predates VIX-3989
Width = 1               // explicit horizontal-axis scaling
LargestDimension = 2    // corrected automatic behavior
```

Suggested user descriptions:

- `Height (Compatibility)`
- `Width`
- `Largest Dimension (Recommended)`

The type and values require XML documentation because they are public API.

### `PinWheelData.SizeScaleBasis`

- Add a `[DataMember]` public property of type `PinWheelSizeScaleBasis`.
- Set it to `LargestDimension` in `PinWheelData()`.
- Do not assign it in `OnDeserialized`; doing so would erase the missing-member compatibility
  behavior.
- Copy it in `CreateInstanceForClone()`.
- Do not use `EmitDefaultValue = false`; after an older effect is loaded and saved, explicitly
  persisting Height makes its intent durable.
- Add XML documentation to the public property.

### `PinWheel.SizeScaleBasis`

Expose the data value through a public effect property with the usual `[Value]`, Config category,
display-name, description, and property-order attributes. Place it adjacent to Size in the property
grid. Its setter must set `IsDirty`, call `OnPropertyChanged`, and otherwise follow neighboring
Pinwheel properties.

Use the display name `Size Basis`. The description should say that it chooses the preview dimension
used to scale the Size curve and that Largest Dimension is normally the best choice.

The setting has an effect only when `OffsetPercentage` is true. Extend `UpdateOffsetAttribute` so
Size Basis is browsable in that mode and hidden when the older absolute-offset behavior is active.
Use `nameof` for both property keys while touching this method. Add localized entries to the effect
display-name and description resources and regenerate their designer files.

The public effect property requires XML documentation.

## Mathematical / Boundary Logic

Extract the duplicated basis calculation to a private helper used by both render paths:

```text
CalculateRadiusBasis(origin):
    if OffsetPercentage is false:
        return DistanceFromPoint(
            origin,
            (BufferWiOffset + BufferWi, BufferHtOffset + BufferHt))

    return SizeScaleBasis:
        Height          => BufferHt
        Width           => BufferWi
        LargestDimension => max(BufferWi, BufferHt)
        unknown         => BufferHt

sizeFactor = CalculateSize(intervalPosition) / 100.0
maxRadius = CalculateRadiusBasis(origin) * sizeFactor
```

The fallback for an unknown value is Height, favoring compatibility and safe rendering. Preserve the
existing Size curve mapping, offset math, center-hub ratio, twist calculation, and radius comparison.
The scope is the basis only; do not change the `OffsetPercentage == false` corner calculation as part
of this issue.

Apply the selected basis consistently to string and location rendering. Largest Dimension is
orientation-invariant because swapping width and height does not change `max(BufferWi, BufferHt)`.
For square buffers, all three modes are equivalent.

## Subsystem Component Matrix

| Component | Change | Behavioral responsibility |
|---|---|---|
| `PinWheelSizeScaleBasis.cs` | New public enum | Closed strategy choices with Height at zero for compatibility |
| `PinWheelData.cs` | Add serialized property, new-instance default, clone copy | Persist user choice without a sequence migration |
| `PinWheel.cs` | Add UI property and shared radius-basis helper; use helper in both render paths | Keep string/location results aligned and remove duplicated selection logic |
| Effect descriptor `.resx` files | Add Size Basis name and description | Localizable, understandable property-grid text |
| `PinWheelSizeScaleBasisTests.cs` or equivalent under `src/Vixen.Tests/Effects` | Add serialization, clone, UI, and render tests | Lock compatibility and corrected behavior |

No descriptor version change, sequence content migrator, ViewModel, command, service, or dependency
injection change is required. The property-grid attributes already provide the relevant editor UI;
Catel-specific changes are not involved.

## Validation and Acceptance

Automated coverage must establish:

1. `new PinWheelData()` defaults Size Basis to Largest Dimension.
2. Deserializing pre-VIX-3989 Pinwheel XML with no Size Basis member yields Height.
3. Height, Width, and Largest Dimension each survive DataContract round-trip and `Clone()`.
4. On a wide virtual buffer, Height reproduces the old radius while Width and Largest Dimension can
   illuminate a location beyond the old height-based maximum.
5. On a tall virtual buffer, Largest Dimension selects height; on a square buffer all modes match.
6. String and location render paths apply the same basis.
7. With `OffsetPercentage == false`, changing Size Basis has no effect on rendered output.
8. The Size Basis property is browsable only where it affects the percentage-based calculation.

Follow the reflection-based render-test seam in `SpiralLocationRenderTests` unless a smaller
internal pure-function seam is introduced without widening production API. Configure deterministic
Pinwheel colors, one arm, full thickness, fixed curves, and a one-frame buffer so coverage assertions
do not depend on animation or random colors.

After the focused tests, build `Vixen_Tests` with full MSBuild and run the already-built x64 test
project using the commands in `AGENTS.md`. Manual validation should load an older sequence containing
a Pinwheel on a wide location group, confirm no visual change, switch Size Basis to Largest Dimension,
and confirm that Size can cover the preview corners. A newly added Pinwheel should already use Largest
Dimension.

## Concurrency, Performance & Thread Safety

The change adds one enum read and a constant-time `switch` per rendered frame. It allocates no new
objects in the pixel loop. The helper reads instance-local effect data and the already configured
buffer dimensions, so it introduces no shared state or synchronization requirement. Calculate the
basis once per frame, outside the per-pixel loop, as the existing `xc` calculation does.

## TERRA HAND-OFF CONTEXT

VIX-3989 fixes Pinwheel radius scaling on wide location buffers. Current `PinWheel.cs` uses
`OffsetPercentage ? BufferHt : DistanceFromPoint(origin, bottomRight)` in both `RenderEffect` and
`RenderEffectByLocation`; Size maps to a factor of 0.01-4.00. Implement a new public enum in its own
file: `PinWheelSizeScaleBasis { Height = 0, Width = 1, LargestDimension = 2 }`. Zero MUST remain Height:
module data uses DataContractSerializer, whose missing members receive CLR defaults without running
constructors, so old sequence/default payloads automatically remain height-based. Add
`[DataMember] public PinWheelSizeScaleBasis SizeScaleBasis { get; set; }` to `PinWheelData`; explicitly set
LargestDimension in the constructor for truly new effects; do not assign it in `OnDeserialized`; copy
it in `CreateInstanceForClone`; do not suppress default-value serialization. Expose a documented
`[Value]` Config property on `PinWheel`, next to Size, display name `Size Basis`, with setter dirty and
property-changed behavior. Suggested enum labels: Height (Compatibility), Width, Largest Dimension
(Recommended). Add localized display/description resources. Extend `UpdateOffsetAttribute` so Size
Basis is shown only when `OffsetPercentage` is true, using `nameof`. Extract one private helper shared
by both render paths: if `OffsetPercentage` is false, return the existing bottom-right distance
unchanged; otherwise return BufferHt/BufferWi/Math.Max(BufferWi, BufferHt) per enum, falling back to
BufferHt for unknown values. Keep all other math unchanged and calculate once per frame outside pixel
loops. Add xUnit coverage under `src/Vixen.Tests/Effects` for constructor default Largest, old XML
missing member => Height, DataContract round-trip, clone, wide/tall/square behavior, parity of string
and location paths, unchanged false-OffsetPercentage branch, and property browsability. Public enum
and properties need XML docs. Do not overwrite the pre-existing uncommitted Debug.WriteLine edit in
PinWheel.cs. No sequence migration, descriptor version change, Catel/ViewModel change, or new service
is needed.

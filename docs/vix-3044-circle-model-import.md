# VIX-3044: Import xLights Circle xModel Props

## Status

Specification draft. This document is intended to be refined into an ExecPlan before implementation.

## Jira Issue

VIX-3044 will track the final specification, design, acceptance criteria, and test plan for importing xLights circle models into the Vixen Custom Prop Editor.

## Problem Statement

Vixen currently imports xLights custom models into the Custom Prop Editor. xLights also exports spinner and concentric-ring props as circle models. Those files are currently reported as unsupported, even though their node layout can be derived from the circle model attributes and their submodels use the same node-range format already handled for custom models.

Users need to import xLights circle models from both local `.xmodel` files and vendor inventory links, preserving the model's node order, visual coordinates, generated circle groups, and supported child metadata such as `subModel`, `faceInfo`, and `stateInfo`.

## Goals

- Import standalone `<circlemodel>` roots.
- Import wrapped `<model DisplayAs="Circle">` elements under a root `<models>` wrapper.
- Generate Vixen light nodes in the same node order as the xLights circle model.
- Place imported nodes on concentric circular coordinates in the Custom Prop Editor canvas.
- Create a primary model group containing every pixel node.
- Create generated circle groups so users can target individual rings.
- Reuse existing xModel range parsing and assembly behavior for supported `subModel`, `faceInfo`, and `stateInfo` children.
- Preserve the existing custom model import behavior.
- Introduce a reusable model-type parsing pattern so later xLights model types can add only their model-specific node generation.

## Non-Goals

- Do not import more than one model from a wrapper into the same Vixen prop.
- Do not add support for xLights `modelGroup` elements.
- Do not add support for xLights `subModel type="subbuffer"` beyond the current behavior.
- Do not create controller/channel patching from `StartChannel`, `Controller`, or vendor metadata.
- Do not attempt to render 3D rotation or xLights world position attributes.
- Do not require pixel-perfect parity with the xLights Layout view if Vixen's canvas coordinate system needs a practical normalization step.

## Existing Import Context

The Custom Prop Editor imports xModels through `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`.

The current importer:

- Reads XML through `XDocument`.
- Supports a standalone `<custommodel>` root.
- Supports a root `<models>` wrapper and routes selected `<model DisplayAs="Custom">` children through the custom model parser.
- Resolves wrapped non-custom models by `DisplayAs`, for example `DisplayAs="Circle"` resolves to `circlemodel`.
- Builds a `CustomModel` intermediate object with `ModelNode` entries.
- Calls the existing assembly flow to create:
  - `<Prop> - Model`
  - imported submodel groups
  - imported face groups
  - imported State definition metadata on the model group
  - legacy State groups when enabled

Circle import should preserve this flow by adding a circle-specific parser that produces the same kind of node map and child metadata consumed by the existing assembly path.

## Supported Input Shapes

### Standalone Circle Model

```xml
<circlemodel
	name="EFL 2ft Spinner-1"
	DisplayAs="Circle"
	LayerSizes="20,20,40,40,40,40"
	InsideOut="0"
	StartSide="B"
	Dir="L"
	centerPercent="25"
	PixelSize="4">
	<subModel ... />
</circlemodel>
```

### Wrapped Circle Model

```xml
<models type="exported">
	<model
		name="Circle"
		DisplayAs="Circle"
		LayerSizes="30,40,50"
		InsideOut="0"
		StartSide="B"
		Dir="R"
		centerPercent="40"
		PixelSize="2" />
</models>
```

Wrapper selection rules should match the existing wrapper behavior:

- If the wrapper contains exactly one direct child model, import it directly when it is supported.
- If the wrapper contains multiple direct child models, show the existing selection dialog with model name and model type.
- If the selected model is `DisplayAs="Circle"`, route it through the circle parser.
- If the selected model is unsupported, show the normal unsupported-model error for the resolved model type.
- If the wrapper contains no direct child `model` elements, show the existing no-models-found error.

## Circle Attribute Semantics

| Attribute | Required | Meaning |
|---|---:|---|
| `name` | yes | Vixen prop base name. Use existing unnamed-model fallback behavior if absent. |
| `LayerSizes` | yes | Comma-delimited positive integer node counts for each circle. Interpret and traverse these values the same way xLights does so Vixen lands on the same physical circle definitions and wiring order. |
| `Dir` | yes | Wiring direction around each circle. `L` means clockwise. `R` means counter-clockwise. |
| `InsideOut` | yes | Start ring selector. `1` starts on the inside ring. `0` starts on the outside ring. |
| `StartSide` | yes | Start side on the start ring. `B` starts at bottom. `T` starts at top. |
| `centerPercent` | no | Diameter or radius percentage for the innermost ring relative to the outer ring. Missing or non-numeric values import as `0`, matching xLights' field default. Numeric values are clamped to `0..100` for deterministic import behavior. |
| `parm3` | no | Legacy equivalent of `centerPercent` for standalone `<circlemodel>` exports. Use only when `centerPercent` is absent or invalid. |
| `PixelSize` | no | Vixen light size. Use existing custom model default of `1` when missing or invalid. |
| `ScaleX` | no | xLights screen-location X scale. Apply to generated X coordinates before rounding. Missing, invalid, or non-positive values use `1.0`. |
| `ScaleY` | no | xLights screen-location Y scale. Apply to generated Y coordinates before rounding. Missing, invalid, or non-positive values use `1.0`. |
| `PixelCount` | no | Modern wrapped-model total node count. Validate against `LayerSizes` when present. |
| `NumStrings` | no | Modern equivalent of legacy `parm1`. Not needed for node layout except as optional validation. |
| `NodesPerString` | no | Modern equivalent of legacy `parm2`. Not needed for node layout except as optional validation. |
| `parm1` | no | Legacy equivalent of `NumStrings`. Not needed for node layout except as optional validation. |
| `parm2` | no | Legacy equivalent of `NodesPerString`. Not needed for node layout except as optional validation. |
| `StringType` | no | Preserve on the intermediate model where the current custom import preserves it. |

Unsupported or ignored xLights layout attributes include `RotateX`, `RotateY`, `RotateZ`, `ScaleZ`, `WorldPosX`, `WorldPosY`, `WorldPosZ`, `LayoutGroup`, `Antialias`, and `Transparency`.

## Validation Rules

Circle parsing should fail with a clear model import error when:

- `LayerSizes` is missing.
- `LayerSizes` contains no positive integer values.
- `Dir` is present but not `L` or `R`.
- `InsideOut` is present but not `0` or `1`.
- `StartSide` is present but not `B` or `T`.
- `PixelCount` is present, positive, and differs from the sum of `LayerSizes`.

Circle parsing should tolerate and log a warning when:

- `centerPercent` or `parm3` is missing, non-numeric, or outside the supported range. Missing and non-numeric values use `0`; numeric values outside the supported range are clamped to `0..100`.
- `PixelSize` is missing or invalid. Use `1`.
- `ScaleX` or `ScaleY` is missing, invalid, or non-positive. Use `1.0`.
- Optional count fields (`NumStrings`, `NodesPerString`, `parm1`, `parm2`) are inconsistent with `LayerSizes`, unless the implementation team chooses to make this a hard failure.

## Node Order

The imported Vixen node order must follow the circle model wiring order. All generated structures that group circle nodes should also follow wiring order.

Given:

```text
LayerSizes="30,40,50"
InsideOut="0"
StartSide="B"
Dir="L"
```

The first imported Vixen light node is the first node on the outside circle at the bottom. Nodes continue clockwise around that outside circle, then continue to the next circle, and finally end on the inside circle.

When `InsideOut="1"`, the first imported Vixen light node is the first node on the inside circle. Nodes continue around that inside circle, then continue outward.

For each ring:

- Node numbers are assigned sequentially across rings.
- Ring traversal starts from the configured `StartSide`.
- Ring traversal direction follows `Dir`.
- The first node in each subsequent ring starts at the same `StartSide` as the first ring.

## Coordinate Generation

Circle import should generate Vixen `ModelNode` coordinates directly instead of requiring `CustomModel` or `CustomModelCompressed` attributes.

Recommended coordinate model:

- Determine `outerNodeCount = max(LayerSizes)`.
- Determine `outerRadius = outerNodeCount / 2.0`.
- Determine `innerRadius = centerPercent / 100.0 * outerRadius`.
- If there is one ring, use `outerRadius`.
- If there are multiple rings, distribute radii evenly between `innerRadius` and `outerRadius`.
- Apply optional xLights `ScaleX` and `ScaleY` values to the generated centered coordinates before rounding. This bakes the xLights screen-location scale into Vixen's fixed light coordinates and avoids jagged small-ring imports.
- Convert centered circle coordinates into positive Vixen canvas coordinates by shifting all generated points so the minimum `X` and `Y` are `0`, then let the existing assembly offset and minimum prop-size logic apply.

For a WPF-style coordinate system where Y increases downward:

```text
startAngle = StartSide == "T" ? 180 degrees : 0 degrees
direction = Dir == "L" ? -1 : +1
angle = startAngle + direction * 360 degrees * nodeIndexWithinRing / nodeCountInRing
x = centerX + sin(angle) * radius
y = centerY + cos(angle) * radius
```

Coordinates should be rounded consistently to integer `ModelNode.X` and `ModelNode.Y` values. Use one documented rounding strategy, preferably `MidpointRounding.AwayFromZero` or `Math.Round` with the default behavior if it matches existing parser expectations.

### xLights Reference Note

xLights `CircleModel.cpp` uses:

- Maximum layer size as the render width and height.
- `maxRadius = maxLights / 2.0`.
- `minRadius = centerPercent / 100.0 * maxRadius`.
- `StartSide` through an inherited top/bottom flag.
- `Dir` through an inherited left-to-right flag.
- `sin(angle)` for X and `cos(angle)` for Y.

The implementation should follow xLights' layer traversal, including its `GetStrandLength(layerCount - strand - 1)` behavior, rather than independently assuming a different physical ordering from the raw `LayerSizes` text. The result should be verified against the local reference files under `docs/references/circlemodel`.

When mapping xLights `Dir` into Vixen's WPF-style canvas, `Dir="L"` must decrement the angle so a bottom-start ring advances visually clockwise. Incrementing the angle from the bottom moves the second node to the right side of the canvas, which is counter-clockwise.

## Generated Vixen Hierarchy

Importing a circle model named `Circle` with three rings should create:

```text
Circle {1}
|-- Circle {1} - Model
|   |-- Circle {1} Px 1
|   |-- Circle {1} Px 2
|   |-- Circle {1} Px 3
|-- Circle {1} - Circles
|   |-- Circle {1} - Circle 1
|   |   |-- Circle {1} Px 1
|   |   |-- Circle {1} Px 2
|   |-- Circle {1} - Circle 2
|   |   |-- Circle {1} Px 51
|   |   |-- Circle {1} Px 52
|   |-- Circle {1} - Circle 3
|   |   |-- Circle {1} Px 91
|-- Circle {1} - <Imported SubModel Name>
|   |-- Circle {1} Px 23
|   |-- Circle {1} Px 24
```

Hierarchy rules:

- `<Prop> - Model` contains all light nodes in imported wiring order.
- `<Prop> - Model` has `ElementModelType.Model`.
- `<Prop> - Circles` is a generated grouping parent.
- Each generated circle group contains references to the already-created light nodes, not duplicate lights.
- Each generated circle group should use `ElementModelType.SubModel` unless a separate `ElementModelType.Circle` is explicitly approved.
- Imported `subModel` groups remain root-level groups, matching current custom model behavior.
- If an imported ranges `subModel` named `Circles` contains the same per-ring node-order groups as the generated circle groups, keep the imported subModel and skip the generated `Circles` group to avoid duplicate targeting groups.
- Imported `subModel`, `faceInfo`, and `stateInfo` range references must resolve against the generated node order.

Circle group numbering follows wiring order. `Circle 1` is the first wired ring, `Circle 2` is the next wired ring, and so on. This means `InsideOut="0"` makes `Circle 1` the outside ring, while `InsideOut="1"` makes `Circle 1` the inside ring.

## Import Pipeline Design Requirements

The implementation should avoid making `XModelImport` a larger single-class parser.

Recommended shape:

- Keep `XModelImport.ImportAsync(string filePath)` as the public entry point.
- Add an internal model-type parser abstraction, for example `IXModelElementParser`, with:
  - model type support detection
  - XML attribute parsing
  - model-specific node generation
  - common child import delegation
- Extract the current custom model parsing into a custom parser or an internal method with the same contract.
- Add a circle parser that produces the same intermediate representation used by `Assemble`.
- Extract common child parsing for `subModel`, `faceInfo`, `stateInfo`, and ignored `modelGroup` handling so custom and circle imports do not duplicate that logic.
- Keep wrapper discovery and model selection model-type aware.

The common intermediate representation should support:

- Model name.
- Pixel size.
- String type and other currently preserved metadata.
- `Dictionary<int, ModelNode>` keyed by node order.
- Optional generated groups, such as circle groups, expressed as range groups or explicit node-order lists.
- Imported submodels, faces, and states.

## Vendor Inventory Behavior

Vendor imports that download or read `.xmodel` files should use the same importer entry point as file imports. A vendor model linked to a circle xModel should produce the same prop hierarchy as importing that `.xmodel` manually.

Inventory metadata mapping should continue to populate vendor/product metadata as it does today. Circle-specific parsing should not depend on vendor metadata.

## Acceptance Criteria

- A standalone `<circlemodel>` file imports without an unsupported model type error.
- A wrapped `<models><model DisplayAs="Circle">` file imports without an unsupported model type error.
- A wrapper with multiple models lets the user select a circle model and imports only that model.
- Imported circle props contain a root prop, a primary model group, generated circle groups, and any supported imported submodels.
- The model group contains exactly `sum(LayerSizes)` light nodes.
- Light node orders are contiguous from `1` through `sum(LayerSizes)`.
- Generated circle groups contain the expected node ranges for the selected `InsideOut` behavior.
- `StartSide="T"` and `StartSide="B"` place the first node at top and bottom respectively.
- `Dir="L"` and `Dir="R"` produce opposite rotational ordering.
- `centerPercent` changes the inner-ring radius while keeping the outer ring fixed.
- Missing, non-numeric, and out-of-range `centerPercent` values import deterministically using the documented default/clamp behavior.
- Existing custom model import tests continue to pass.
- Unsupported non-circle/non-custom model types still report the normal unsupported model type.
- Imported circle submodels resolve ranges against the generated node order.

## Automated Test Plan

Add focused tests under `src/Vixen.Tests/App/CustomPropEditor/Import/XLights`.

Tests should use embedded minimal XML strings. Do not read the reference `.xmodel` files directly.

Recommended coverage:

- Standalone `<circlemodel>` imports a one-ring model.
- Wrapped `<model DisplayAs="Circle">` imports a one-ring model.
- Three-ring outside-in model creates the expected model node count and circle group ranges.
- Three-ring inside-out model creates the expected model node count and circle group ranges.
- `StartSide="T"` and `StartSide="B"` produce different first-node coordinates.
- `Dir="L"` and `Dir="R"` produce mirrored second-node coordinates.
- `centerPercent` changes the inner radius for a multi-ring model.
- Missing or non-numeric `centerPercent` imports as `0`.
- Numeric `centerPercent` below `0` clamps to `0`; numeric values above `100` clamp to `100`.
- Imported `subModel type="ranges"` maps to generated nodes.
- Invalid `LayerSizes` returns `null` and reports a model import error.
- Mismatched `PixelCount` returns `null` and reports a model import error.
- Existing wrapper custom-model selection behavior remains unchanged.

## Manual Validation Plan

Manually import each file in `docs/references/circlemodel` through the Custom Prop Editor:

- `circle-1-ring-outer-cw.xmodel`
- `Circle-3-ring-bottom-inner-ccw.xmodel`
- `Circle-3-ring-bottom-outer-ccw.xmodel`
- `Circle-3-ring-outer-ccw.xmodel`
- `Circle-3-ring-top-inner-ccw.xmodel`
- `Circle-3-ring-top-outer-ccw.xmodel`
- `circle-vendor.xmodel`

For each imported model:

- Confirm no unsupported model type error appears.
- Confirm the model group contains the expected node count.
- Confirm generated circle groups exist and contain the expected node counts.
- Confirm the visual placement matches the filename's start side, inside/outside start ring, and direction.
- For `circle-vendor.xmodel`, confirm imported xLights submodels appear and contain expected node references.

## Risks and Confirmed Decisions

1. Confirmed: circle group numbering follows wiring order. `Circle 1` is the first wired ring.
2. Confirmed: `LayerSizes` should follow xLights behavior. The ExecPlan should include a small parity investigation before implementation so the parser mirrors xLights' layer traversal rather than relying on a separate interpretation.
3. Confirmed: missing or non-numeric `centerPercent` imports as `0`; numeric values clamp to `0..100`. xLights' `CircleModel` has a field default of `0` and its setter stores the supplied value directly, so Vixen's clamp is an import-safety rule.
4. Risk: coordinate parity should be validated visually against xLights. The first implementation can target deterministic Vixen geometry, but acceptance should define how close it must be to xLights.
5. Risk: adding a new `ElementModelType.Circle` would require public enum documentation and downstream persistence review. Reusing `SubModel` for generated circle groups is lower risk.
6. Risk: the existing range parser uses `Convert.ToInt32` and can throw on malformed ranges. A later hardening task may be useful, but this issue should only change it if circle examples expose a failure.

## Reference Material

- Existing importer: `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`
- Existing wrapper-import plan: `docs/plans/state/vix-custom-prop-editor-xmodel-models-wrapper-import.md`
- Local circle examples: `docs/references/circlemodel`
- xLights circle source:
  - `https://github.com/xLightsSequencer/xLights/blob/master/src-core/models/CircleModel.cpp`
  - `https://github.com/xLightsSequencer/xLights/blob/master/src-core/models/CircleModel.h`

## ExecPlan Notes

The ExecPlan should:

- Read `.agents/PLANS.md`.
- Include a step to create or update the VIX-3044 Jira issue with the finalized design.
- Use project skills as dictated by the implementation:
  - `dotnet-best-practices` for C# implementation.
  - `csharp-async` if async importer code changes beyond existing patterns.
  - `csharp-docs` if public or protected APIs are added or changed.
  - `dotnet-design-pattern-review` for the parser abstraction and intermediate representation.
- Call out the confirmed decisions and remaining risks above before implementation begins.

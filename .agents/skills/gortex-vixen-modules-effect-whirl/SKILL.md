---
name: gortex-vixen-modules-effect-whirl
description: "Work in the Vixen.Modules/Effect · Whirl area — 628 symbols across 14 files (89% cohesion)"
---

# Vixen.Modules/Effect · Whirl

628 symbols | 14 files | 89% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Modules/Effect/Effect/IPixelFrameBuffer.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/IWhirl.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolColorMode.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolDirection.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolMode.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolRotation.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolSideType.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolStartLocation.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/WhirlVortexMetadata.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_ConcentricDrawMethods.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_DrawInMethods.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_DrawOutMethods.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirlpool.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Modules/Effect/Effect/IPixelFrameBuffer.cs` | IPixelFrameBuffer |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/IWhirl.cs` | Height, RightColor, Width, StartLocation, BandLength, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolColorMode.cs` | GradientOverTime, Bands, WhirlpoolColorMode, RectangularRings, LegColors |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolDirection.cs` | In, InAndOut, Out, WhirlpoolDirection |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolMode.cs` | WhirlpoolMode, Meteor, RecurrentWhirls, SymmetricalWhirls |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolRotation.cs` | WhirlpoolRotation, CounterClockwise, Clockwise |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolSideType.cs` | BottomSide, RightSide, LeftSide, TopSide, WhirlpoolSideType |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolStartLocation.cs` | WhirlpoolStartLocation, BottomRight, TopRight, BottomLeft, TopLeft |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl.cs` | SetupRender, width, maxNumberOfPixels, UpdateThicknessAttributes, height, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/WhirlVortexMetadata.cs` | DrawLeft, DrawRight, LastX, LastHeight, LastY, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_ConcentricDrawMethods.cs` | width, DrawWhirlSymmetricalIn, intervalPos, x, thickness, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_DrawInMethods.cs` | height, x, spacing, DrawBottomRightClockwiseIn, x, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_DrawOutMethods.cs` | height, width, width, intervalPos, intervalPos, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirlpool.cs` | UpdateWhirlMode, frame, whirlMode, RenderEffect, frameBuffer, ... |

## Connected Communities

- **Whirlpool/Whirlpool · DoubleThickness** (8 cross-edges)
- **Effect/Meteors +30 dirs** (4 cross-edges)
- **Vixen.Modules/Effect · ScaleCurveToValue** (2 cross-edges)
- **Effect/Fireworks +11 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1232")
explore(operation:"context", task:"understand Vixen.Modules/Effect · Whirl", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

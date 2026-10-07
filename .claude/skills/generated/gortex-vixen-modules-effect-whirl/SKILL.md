---
name: gortex-vixen-modules-effect-whirl
description: "Work in the Vixen.Modules/Effect · Whirl area — 582 symbols across 11 files (89% cohesion)"
---

# Vixen.Modules/Effect · Whirl

582 symbols | 11 files | 89% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Modules/Effect/Effect/IPixelFrameBuffer.cs`
- `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolDirection.cs`
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
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolDirection.cs` | In, Out, InAndOut, WhirlpoolDirection |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolRotation.cs` | WhirlpoolRotation, CounterClockwise, Clockwise |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolSideType.cs` | TopSide, WhirlpoolSideType, RightSide, LeftSide, BottomSide |
| `src/Vixen.Modules/Effect/Whirlpool/Whirl/WhirlpoolStartLocation.cs` | BottomLeft, WhirlpoolStartLocation, TopRight, BottomRight, TopLeft |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl.cs` | height, _colorMode, firstPass, width, x, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/WhirlVortexMetadata.cs` | LastY, LastWidth, DrawRight, LastHeight, DrawLeft, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_ConcentricDrawMethods.cs` | width, spacing, y, intervalPos, frameBuffer, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_DrawInMethods.cs` | intervalPos, DrawTopRightCounterClockwiseIn, spacing, y, height, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirl_DrawOutMethods.cs` | intervalPos, drawBottom, height, height, drawTop, ... |
| `src/Vixen.Modules/Effect/Whirlpool/Whirlpool/Whirlpool.cs` | bufferHt, RenderEffect, intervalPos, numFrames, RenderEffectByLocation, ... |

## Connected Communities

- **Whirlpool/Whirlpool** (8 cross-edges)
- **OpenGL/Volumes +24 dirs** (2 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1289")
explore(operation:"context", task:"understand Vixen.Modules/Effect · Whirl", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

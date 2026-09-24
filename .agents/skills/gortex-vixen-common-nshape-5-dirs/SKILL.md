---
name: gortex-vixen-common-nshape-5-dirs
description: "Work in the Vixen.Common/NShape +5 dirs area — 2522 symbols across 52 files (84% cohesion)"
---

# Vixen.Common/NShape +5 dirs

2522 symbols | 52 files | 84% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/GraphicalPatching/ConnectionTool.cs`
- `src/Vixen.Application/GraphicalPatching/DataFlowConnectionLine.cs`
- `src/Vixen.Application/GraphicalPatching/ElementNodeShape.cs`
- `src/Vixen.Application/GraphicalPatching/FilterSetupShapeBase.cs`
- `src/Vixen.Application/GraphicalPatching/FilterShape.cs`
- `src/Vixen.Application/GraphicalPatching/NestingSetupShape.cs`
- `src/Vixen.Application/GraphicalPatching/OutputShape.cs`
- `src/Vixen.Application/Setup/SetupPatchingGraphical.cs`
- `src/Vixen.Common/NShape/CachedRepository.cs`
- `src/Vixen.Common/NShape/CircularArcBase.cs`
- `src/Vixen.Common/NShape/Collections.cs`
- `src/Vixen.Common/NShape/Command.cs`
- `src/Vixen.Common/NShape/Core.cs`
- `src/Vixen.Common/NShape/CursorProvider.cs`
- `src/Vixen.Common/NShape/DiagramPresenter.cs`
- `src/Vixen.Common/NShape/DiagramSetController.cs`
- `src/Vixen.Common/NShape/DiameterShape.cs`
- `src/Vixen.Common/NShape/Exceptions.cs`
- `src/Vixen.Common/NShape/FreeHandTool.cs`
- `src/Vixen.Common/NShape/GdiHelpers.cs`
- `src/Vixen.Common/NShape/Geometry.cs`
- `src/Vixen.Common/NShape/ImageBasedShape.cs`
- `src/Vixen.Common/NShape/LayerPresenter.cs`
- `src/Vixen.Common/NShape/Layouter.cs`
- `src/Vixen.Common/NShape/LinearShape.cs`
- `src/Vixen.Common/NShape/Model.cs`
- `src/Vixen.Common/NShape/PathBasedShape.cs`
- `src/Vixen.Common/NShape/PolyLineBase.cs`
- `src/Vixen.Common/NShape/Polygone.cs`
- `src/Vixen.Common/NShape/RectangleShape.cs`
- `src/Vixen.Common/NShape/RectangularLineBase.cs`
- `src/Vixen.Common/NShape/Repository.cs`
- `src/Vixen.Common/NShape/Shape.cs`
- `src/Vixen.Common/NShape/ShapeBase.cs`
- `src/Vixen.Common/NShape/ShapeDuplicator.cs`
- `src/Vixen.Common/NShape/ShapeGroup.cs`
- `src/Vixen.Common/NShape/ShapeType.cs`
- `src/Vixen.Common/NShape/Styles.cs`
- `src/Vixen.Common/NShape/Template.cs`
- `src/Vixen.Common/NShape/TextShape.cs`
- `src/Vixen.Common/NShape/Tool.cs`
- `src/Vixen.Common/NShape/ToolCache.cs`
- `src/Vixen.Common/NShape/ToolSetController.cs`
- `src/Vixen.Common/NShape/TriangleBase.cs`
- `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs`
- `src/Vixen.Common/NShapeWinFormsUI/Display.cs`
- `src/Vixen.Common/NShapeWinFormsUI/InplaceTextBox.cs`
- `src/Vixen.Common/NShapeWinFormsUI/LayerListView.cs`
- `src/Vixen.Common/NShapeWinFormsUI/ShapeInfoDialog.cs`
- `src/Vixen.Common/NShapeWinFormsUI/StyleListBox.cs`
- `src/Vixen.Common/NShapeWinFormsUI/WinFormHelpers.cs`
- `src/Vixen.Modules/App/FPPClient/Client/IFppClientFactory.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/GraphicalPatching/ConnectionTool.cs` | ResetPreviewShapes, connectionInfo, mouseState, EnterDisplay, mouseState, ... |
| `src/Vixen.Application/GraphicalPatching/DataFlowConnectionLine.cs` | CopyFrom, CreateInstance, DestinationDataComponent, SourceDataFlowComponentReference, DataFlowConnectionLine.<init>, ... |
| `src/Vixen.Application/GraphicalPatching/ElementNodeShape.cs` | controlPointCapability, ReferenceControlPointHasCapability |
| `src/Vixen.Application/GraphicalPatching/FilterSetupShapeBase.cs` | point, FilterSetupShapeBase.<init>, _textBrush, Output, CopyFrom, ... |
| `src/Vixen.Application/GraphicalPatching/FilterShape.cs` | controlPointCapability, ReferenceControlPointHasCapability |
| `src/Vixen.Application/GraphicalPatching/NestingSetupShape.cs` | ChildFilterShapes |
| `src/Vixen.Application/GraphicalPatching/OutputShape.cs` | controlPointCapability, ReferenceControlPointHasCapability |
| `src/Vixen.Application/Setup/SetupPatchingGraphical.cs` | UpdateConnectionsForControllers, ConnectShapes, controlPoint, shape, source, ... |
| `src/Vixen.Common/NShape/CachedRepository.cs` | GetHashCode, id, ShapeConnection.<init>, Contains |
| `src/Vixen.Common/NShape/CircularArcBase.cs` | pointId, GetMenuItemDefs, DrawThumbnail, x, controlPointId, ... |
| `src/Vixen.Common/NShape/Collections.cs` | layerList, Enumerator, layerCount, Dispose, Enumerator.<init>, ... |
| `src/Vixen.Common/NShape/Command.cs` | to, dx, other, shape, AggregatedCommand.<init>, ... |
| `src/Vixen.Common/NShape/Core.cs` | Dispose, Create, MoveNext, GetEnumerator, Current, ... |
| `src/Vixen.Common/NShape/CursorProvider.cs` | CursorIDs, GetResource, DefaultCursorID, registeredCursors, cursorID, ... |
| `src/Vixen.Common/NShape/DiagramPresenter.cs` | Diagram, shapes, ZoomLevel, HiddenLayers, x, ... |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | alt, KeyDown, D3, Apps, Zoom, ... |
| `src/Vixen.Common/NShape/DiameterShape.cs` | controlPointId, controlPointId, HasControlPointCapability, controlPointCapability, controlPointCapability, ... |
| `src/Vixen.Common/NShape/Exceptions.cs` | NShapeUnsupportedValueException.<init>, value, context, NShapeUnsupportedValueException, value, ... |
| `src/Vixen.Common/NShape/FreeHandTool.cs` | e, ProcessMouseEvent, ProcessKeyEvent, diagramPresenter, diagramPresenter, ... |
| `src/Vixen.Common/NShape/GdiHelpers.cs` | Clone |
| `src/Vixen.Common/NShape/Geometry.cs` | sinAngle, centerOffsetX, divFactorX, height, deltaX, ... |
| `src/Vixen.Common/NShape/ImageBasedShape.cs` | pointId, index, HasControlPointCapability, x, Y, ... |
| `src/Vixen.Common/NShape/LayerPresenter.cs` | eventType, SetMouseEvent, buttons, position, wheelDelta, ... |
| `src/Vixen.Common/NShape/Layouter.cs` | y, ExecuteStepCore, RestoreState, displacement, SaveState, ... |
| `src/Vixen.Common/NShape/LinearShape.cs` | InvalidateDrawCache, controlPointId, TransformCapToOrigin, x, y, ... |
| `src/Vixen.Common/NShape/Model.cs` | Clone, Clone |
| `src/Vixen.Common/NShape/PathBasedShape.cs` | Angle, controlPointId, Y, GetControlPointPosition, HasControlPointCapability, ... |
| `src/Vixen.Common/NShape/PolyLineBase.cs` | VertexA, InsertVertex, y, CalcShapePoints, Equals, ... |
| `src/Vixen.Common/NShape/Polygone.cs` | x, HitTest, y, controlPointCapability, controlPointCapability, ... |
| `src/Vixen.Common/NShape/RectangleShape.cs` | controlPointCapability, y, HasControlPointCapability, controlPointCapability, HasControlPointCapability, ... |
| `src/Vixen.Common/NShape/RectangularLineBase.cs` | Right, controlPointIndex, source, FindSegment, range, ... |
| `src/Vixen.Common/NShape/Repository.cs` | targetPointId, connectorShape, gluePointId, targetShape, RepositoryShapeConnectionEventArgs.<init> |
| `src/Vixen.Common/NShape/Shape.cs` | otherShape, deltaY, other, FirstVertex, ToUInt16, ... |
| `src/Vixen.Common/NShape/ShapeBase.cs` | ownPointId, PropertyName, GetControlPointIds, shape, pointId, ... |
| `src/Vixen.Common/NShape/ShapeDuplicator.cs` | shape, CloneModelObjectOnly |
| `src/Vixen.Common/NShape/ShapeGroup.cs` | otherShape, targetShape, otherShape, targetPointId, LoadFieldsCore, ... |
| `src/Vixen.Common/NShape/ShapeType.cs` | CreatePreviewInstance, shape |
| `src/Vixen.Common/NShape/Styles.cs` | capShape, Round, Diamond, None, ClosedArrow, ... |
| `src/Vixen.Common/NShape/Template.cs` | CreateShape |
| `src/Vixen.Common/NShape/TextShape.cs` | MovePointByCore, HasControlPointCapability, y, Connect, shape, ... |
| `src/Vixen.Common/NShape/Tool.cs` | diagramPresenter, modifier, GetPreviousMouseState, wantsAutoScroll, diagramPresenter, ... |
| `src/Vixen.Common/NShape/ToolCache.cs` | lineStyle, lineStyle, lineStyle, GetCapPoints, lineStyle, ... |
| `src/Vixen.Common/NShape/ToolSetController.cs` | tools, Tools |
| `src/Vixen.Common/NShape/TriangleBase.cs` | width, HitTest, height, x, Fit, ... |
| `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs` | controlPointCapability, HasControlPointCapability, controlPointId |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | shapes, drawMode, SetCursor, capabilities, shape, ... |
| `src/Vixen.Common/NShapeWinFormsUI/InplaceTextBox.cs` | horizontalAlignment, ConvertToContentAlignment |
| `src/Vixen.Common/NShapeWinFormsUI/LayerListView.cs` | eventType, item, buttons, modifiers, clickCount, ... |
| `src/Vixen.Common/NShapeWinFormsUI/ShapeInfoDialog.cs` | GetControlPointCapabilities, id |
| `src/Vixen.Common/NShapeWinFormsUI/StyleListBox.cs` | ToString |
| `src/Vixen.Common/NShapeWinFormsUI/WinFormHelpers.cs` | mouseButtons, GetModifiers, SetButtons |
| `src/Vixen.Modules/App/FPPClient/Client/IFppClientFactory.cs` | IFppClientFactory |

## Connected Communities

- **Vixen.Common/NShape +64 dirs** (67 cross-edges)
- **Vixen.Common/NShape +13 dirs** (61 cross-edges)
- **Vixen.Common/NShape +2 dirs · Draw** (11 cross-edges)
- **Vixen.Common/NShape +3 dirs · CachedRepository** (8 cross-edges)
- **Vixen.Common/NShape +3 dirs · PerformSelection** (8 cross-edges)
- **Vixen.Common · DeleteShapes** (6 cross-edges)
- **Vixen.Common · DoDrawCaptionBounds** (5 cross-edges)
- **Vixen.Common · InvalidateAnglePreview** (5 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (4 cross-edges)
- **Vixen.Common · OnMouseWheel** (3 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · DoOpenCaptionEditor** (3 cross-edges)
- **Vixen.Common · TerminalId** (3 cross-edges)
- **Vixen.Common/NShape · Draw** (3 cross-edges)
- **Vixen.Common/NShape +8 dirs** (2 cross-edges)
- **Vixen.Common/NShape +3 dirs · CreatePreviewStyle** (1 cross-edges)
- **Vixen.Application/Setup +6 dirs** (1 cross-edges)
- **Vixen.Common/NShape · Revert** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · Dispose** (1 cross-edges)
- **Vixen.Application · ElementNodeShape** (1 cross-edges)
- **Vixen.Common/NShape · IntersectsWith** (1 cross-edges)
- **Vixen.Common · SelectShapes** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI +4 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-270")
explore(operation:"context", task:"understand Vixen.Common/NShape +5 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

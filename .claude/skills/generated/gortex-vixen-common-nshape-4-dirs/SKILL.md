---
name: gortex-vixen-common-nshape-4-dirs
description: "Work in the Vixen.Common/NShape +4 dirs area — 2488 symbols across 50 files (84% cohesion)"
---

# Vixen.Common/NShape +4 dirs

2488 symbols | 50 files | 84% cohesion

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

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/GraphicalPatching/ConnectionTool.cs` | IsMoveShapeFeasible, SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE, diagramPresenter, CreateConnectedTargetPreviewShape, StartToolAction, ... |
| `src/Vixen.Application/GraphicalPatching/DataFlowConnectionLine.cs` | CopyFrom, Clone, shapeType, source, DataFlowConnectionLine.<init>, ... |
| `src/Vixen.Application/GraphicalPatching/ElementNodeShape.cs` | ReferenceControlPointHasCapability, controlPointCapability |
| `src/Vixen.Application/GraphicalPatching/FilterSetupShapeBase.cs` | Other, GetProportionalDistanceForPoint, _defaultTextBrush, DataFlowComponent, controlPoint, ... |
| `src/Vixen.Application/GraphicalPatching/FilterShape.cs` | controlPointCapability, ReferenceControlPointHasCapability |
| `src/Vixen.Application/GraphicalPatching/NestingSetupShape.cs` | ChildFilterShapes |
| `src/Vixen.Application/GraphicalPatching/OutputShape.cs` | ReferenceControlPointHasCapability, controlPointCapability |
| `src/Vixen.Application/Setup/SetupPatchingGraphical.cs` | UpdateConnectionsForElements, FakeShapeConnection, _RemoveShapeFromDiagram, controlPoint, _LookupAndConnectShapeToSource, ... |
| `src/Vixen.Common/NShape/CachedRepository.cs` | id, ShapeConnection.<init>, Contains, GetHashCode |
| `src/Vixen.Common/NShape/CircularArcBase.cs` | X, capSize, InsertVertex, startPtId, x, ... |
| `src/Vixen.Common/NShape/Collections.cs` | layerList, Current, Dispose, Create, Empty, ... |
| `src/Vixen.Common/NShape/Command.cs` | mod, ShapeCommand, shape, existingConnection, MoveGluePointCommand.<init>, ... |
| `src/Vixen.Common/NShape/CursorProvider.cs` | registeredCursors, cursorID, CursorProvider, GetResource, DefaultCursorID, ... |
| `src/Vixen.Common/NShape/DiagramPresenter.cs` | InvalidateSnapIndicators, shape, OpenCaptionEditor, newText, SnapToGrid, ... |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | F12, T, Divide, BrowserFavorites, LineFeed, ... |
| `src/Vixen.Common/NShape/DiameterShape.cs` | modifiers, HasControlPointCapability, controlPointId, controlPointId, controlPointCapability, ... |
| `src/Vixen.Common/NShape/Exceptions.cs` | type, NShapeUnsupportedValueException.<init>, NShapeUnsupportedValueException.<init>, context, value, ... |
| `src/Vixen.Common/NShape/FreeHandTool.cs` | ProcessMouseEvent, diagramPresenter, diagramPresenter, e, ProcessKeyEvent, ... |
| `src/Vixen.Common/NShape/GdiHelpers.cs` | Clone |
| `src/Vixen.Common/NShape/Geometry.cs` | centerOffsetX, cosAngle, width, centerOffsetY, MoveRectangleLeft, ... |
| `src/Vixen.Common/NShape/ImageBasedShape.cs` | controlPointCapability, controlPointId, controlPointId, Y, GetControlPointId, ... |
| `src/Vixen.Common/NShape/LayerPresenter.cs` | eventType, clickCount, SetMouseEvent, modifiers, position, ... |
| `src/Vixen.Common/NShape/Layouter.cs` | ExecuteStepCore, shape, ExcludeFromFitting, w, displacement, ... |
| `src/Vixen.Common/NShape/LinearShape.cs` | ControlPoints, pointX, copyVertexPoint, RemoveVertex, source, ... |
| `src/Vixen.Common/NShape/Model.cs` | Clone, Clone |
| `src/Vixen.Common/NShape/PathBasedShape.cs` | Y, controlPointCapability, HasControlPointCapability, controlPointId, Center, ... |
| `src/Vixen.Common/NShape/PolyLineBase.cs` | idxA, beforePointId, Invalidate, VertexB, controlPointCapability, ... |
| `src/Vixen.Common/NShape/Polygone.cs` | controlPointCapability, range, x, y, controlPointCapability, ... |
| `src/Vixen.Common/NShape/RectangleShape.cs` | height, controlPointCapability, controlPointId, HasControlPointCapability, width, ... |
| `src/Vixen.Common/NShape/RectangularLineBase.cs` | RecalcDrawCache, origin, InvalidateLineSegment, v3, x, ... |
| `src/Vixen.Common/NShape/Repository.cs` | connectorShape, targetShape, targetPointId, gluePointId, RepositoryShapeConnectionEventArgs.<init> |
| `src/Vixen.Common/NShape/Shape.cs` | toY, y, Any, OwnPointId, MakePreview, ... |
| `src/Vixen.Common/NShape/ShapeBase.cs` | controlPointCapability, controlPointCapability, GetConnectionInfos, Reset, distance, ... |
| `src/Vixen.Common/NShape/ShapeDuplicator.cs` | shape, CloneModelObjectOnly |
| `src/Vixen.Common/NShape/ShapeGroup.cs` | otherShape, x, otherShape, Y, controlPointId, ... |
| `src/Vixen.Common/NShape/ShapeType.cs` | CreatePreviewInstance, shape |
| `src/Vixen.Common/NShape/Styles.cs` | Square, ClosedArrow, Diamond, CenteredCircle, capShape, ... |
| `src/Vixen.Common/NShape/Template.cs` | CreateShape |
| `src/Vixen.Common/NShape/TextShape.cs` | HasControlPointCapability, modifiers, controlPointId, controlPointId, deltaY, ... |
| `src/Vixen.Common/NShape/Tool.cs` | y, diagramPresenter, DrawConnectionTargets, SmallIcon, ToolTipText, ... |
| `src/Vixen.Common/NShape/ToolCache.cs` | GetCapPath, capStyle, lineStyle, SetLineCap, pen, ... |
| `src/Vixen.Common/NShape/ToolSetController.cs` | Tools, tools |
| `src/Vixen.Common/NShape/TriangleBase.cs` | x, width, range, controlPointCapability, GetControlPointIds, ... |
| `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs` | HasControlPointCapability, controlPointCapability, controlPointId |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | offsetY, InsertShape, CreateNewClones, IsLayerVisible, CreateNewClones, ... |
| `src/Vixen.Common/NShapeWinFormsUI/InplaceTextBox.cs` | horizontalAlignment, ConvertToContentAlignment |
| `src/Vixen.Common/NShapeWinFormsUI/LayerListView.cs` | position, clickCount, LayerListViewMouseEventArgs.<init>, eventType, item, ... |
| `src/Vixen.Common/NShapeWinFormsUI/ShapeInfoDialog.cs` | id, GetControlPointCapabilities |
| `src/Vixen.Common/NShapeWinFormsUI/StyleListBox.cs` | ToString |
| `src/Vixen.Common/NShapeWinFormsUI/WinFormHelpers.cs` | SetButtons, mouseButtons, GetModifiers |

## Connected Communities

- **Vixen.Common/NShape +69 dirs** (67 cross-edges)
- **Vixen.Common/NShape +12 dirs** (57 cross-edges)
- **Vixen.Common/NShape +2 dirs · Draw** (11 cross-edges)
- **Vixen.Common/NShape +3 dirs · PerformSelection** (8 cross-edges)
- **Vixen.Common/NShape +3 dirs · CachedRepository** (8 cross-edges)
- **Vixen.Common · DeleteShapes** (6 cross-edges)
- **Vixen.Common · DoDrawCaptionBounds** (5 cross-edges)
- **Vixen.Common · InvalidateAnglePreview** (5 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (4 cross-edges)
- **Vixen.Common/NShape +8 dirs** (3 cross-edges)
- **Vixen.Common/NShape · Draw** (3 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · DoOpenCaptionEditor** (3 cross-edges)
- **Vixen.Common · TerminalId** (3 cross-edges)
- **Vixen.Common · OnMouseWheel** (2 cross-edges)
- **Vixen.Application/Setup +6 dirs** (1 cross-edges)
- **Vixen.Common/NShape · Revert** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · Dispose** (1 cross-edges)
- **Vixen.Common/NShape · IntersectsWith** (1 cross-edges)
- **Vixen.Application · ElementNodeShape** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI +5 dirs** (1 cross-edges)
- **Vixen.Common · SelectShapes** (1 cross-edges)
- **Vixen.Common/NShape +3 dirs · CreatePreviewStyle** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-285")
explore(operation:"context", task:"understand Vixen.Common/NShape +4 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

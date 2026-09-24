---
name: gortex-vixen-common-nshape-13-dirs
description: "Work in the Vixen.Common/NShape +13 dirs area — 2955 symbols across 64 files (79% cohesion)"
---

# Vixen.Common/NShape +13 dirs

2955 symbols | 64 files | 79% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/GraphicalPatching/NShapeLibraryInitializer.cs`
- `src/Vixen.Application/GraphicalPatching/NestingSetupShape.cs`
- `src/Vixen.Common/Controls/TextProgressBar.cs`
- `src/Vixen.Common/Controls/Theme/ThemeToolStripRenderer.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Element.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`
- `src/Vixen.Common/Controls/TimeLineControl/RowLabel.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`
- `src/Vixen.Common/NShape/CachedRepository.cs`
- `src/Vixen.Common/NShape/Caption.cs`
- `src/Vixen.Common/NShape/CircularArcBase.cs`
- `src/Vixen.Common/NShape/Design.cs`
- `src/Vixen.Common/NShape/Diagram.cs`
- `src/Vixen.Common/NShape/DiagramPresenter.cs`
- `src/Vixen.Common/NShape/DiagramSetController.cs`
- `src/Vixen.Common/NShape/DiameterShape.cs`
- `src/Vixen.Common/NShape/GdiHelpers.cs`
- `src/Vixen.Common/NShape/Geometry.cs`
- `src/Vixen.Common/NShape/ImageBasedShape.cs`
- `src/Vixen.Common/NShape/LinearShape.cs`
- `src/Vixen.Common/NShape/PathBasedShape.cs`
- `src/Vixen.Common/NShape/PolyLineBase.cs`
- `src/Vixen.Common/NShape/Polygone.cs`
- `src/Vixen.Common/NShape/Project.cs`
- `src/Vixen.Common/NShape/RectangleShape.cs`
- `src/Vixen.Common/NShape/RectangularLineBase.cs`
- `src/Vixen.Common/NShape/Shape.cs`
- `src/Vixen.Common/NShape/ShapeAggregation.cs`
- `src/Vixen.Common/NShape/ShapeBase.cs`
- `src/Vixen.Common/NShape/ShapeCollection.cs`
- `src/Vixen.Common/NShape/ShapeGroup.cs`
- `src/Vixen.Common/NShape/ShapeType.cs`
- `src/Vixen.Common/NShape/ShapeUtils.cs`
- `src/Vixen.Common/NShape/Shaper.cs`
- `src/Vixen.Common/NShape/Styles.cs`
- `src/Vixen.Common/NShape/TextShape.cs`
- `src/Vixen.Common/NShape/Tool.cs`
- `src/Vixen.Common/NShape/ToolCache.cs`
- `src/Vixen.Common/NShape/TriangleBase.cs`
- `src/Vixen.Common/NShapeGeneralShapes/Initializer.cs`
- `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs`
- `src/Vixen.Common/NShapeGeneralShapes/QuadrangleShapes.cs`
- `src/Vixen.Common/NShapeGeneralShapes/RoundShapes.cs`
- `src/Vixen.Common/NShapeGeneralShapes/TriangleShapes.cs`
- `src/Vixen.Common/NShapeWinFormsUI/Display.cs`
- `src/Vixen.Common/NShapeWinFormsUI/ExportDiagramDialog.cs`
- `src/Vixen.Common/NShapeWinFormsUI/FontFamilyListBox.cs`
- `src/Vixen.Common/NShapeWinFormsUI/InplaceTextBox.cs`
- `src/Vixen.Common/NShapeWinFormsUI/StyleListBox.cs`
- `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs`
- `src/Vixen.Common/NShapeWinFormsUI/UITypeEditors.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Adorners/ResizeAdorner.cs`
- `src/Vixen.Modules/App/Polygon/Ellipse.cs`
- `src/Vixen.Modules/App/Polygon/PointBasedShape.cs`
- `src/Vixen.Modules/Editor/FixtureGraphics/OpenGL/Volumes/Rectangle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/PreviewWindowBounds.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStar.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs`
- `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs`
- `src/Vixen.Tests/Preview/VixenPreview/PreviewWindowBoundsTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/GraphicalPatching/NShapeLibraryInitializer.cs` | NShapeLibraryInitializer, preferredRepositoryVersion, namespaceName, Initialize, registrar |
| `src/Vixen.Application/GraphicalPatching/NestingSetupShape.cs` | styleSet, shapeType, NestingSetupShape.<init> |
| `src/Vixen.Common/Controls/TextProgressBar.cs` | DrawProgressBar, g |
| `src/Vixen.Common/Controls/Theme/ThemeToolStripRenderer.cs` | image, e, CreateIndeterminateImage, OnRenderItemCheck |
| `src/Vixen.Common/Controls/TimeLineControl/Element.cs` | DrawInfo, rect, g, layer |
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | g, _drawInfo |
| `src/Vixen.Common/Controls/TimeLineControl/RowLabel.cs` | e, GetLabelFont, OnPaint |
| `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` | alignmentTime, GetAlignmentInvalidationRectangle, previousTimes, currentTimes, GetAlignmentInvalidationRectangle |
| `src/Vixen.Common/NShape/CachedRepository.cs` | shapeTypes, IsShapeTypeInUse |
| `src/Vixen.Common/NShape/Caption.cs` | y, PathText, ParagraphStyle, matrix, GetCaptionBounds, ... |
| `src/Vixen.Common/NShape/CircularArcBase.cs` | x, GetVertexCellArea, IsLine, arcSweepAngle, Radius, ... |
| `src/Vixen.Common/NShape/Design.cs` | CharacterStyles, CapStyles, LineStyles, IStyleSet, ParagraphStyles, ... |
| `src/Vixen.Common/NShape/Diagram.cs` | clipRectangle, graphics, DrawBackground, colorBrushBounds |
| `src/Vixen.Common/NShape/DiagramPresenter.cs` | sRect, ScreenToDiagram, dRect |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | withModelObjects, source, shapes, Copy, copyCutBounds |
| `src/Vixen.Common/NShape/DiameterShape.cs` | TopLeftControlPoint, cos, BottomLeftControlPoint, startY, template, ... |
| `src/Vixen.Common/NShape/GdiHelpers.cs` | GetWrapMode, TransformPathGradientBrush, imageLayout, colorMatrix, brush, ... |
| `src/Vixen.Common/NShape/Geometry.cs` | c, pointX, radius, dY, deltaY, ... |
| `src/Vixen.Common/NShape/ImageBasedShape.cs` | CustomizableMetaFile, styleSet, styleSet, MakePreview, PropertyIdImage, ... |
| `src/Vixen.Common/NShape/LinearShape.cs` | styleSet, EndCapIntersectsWith, deltaY, LoadFieldsCore, rotationCenterY, ... |
| `src/Vixen.Common/NShape/PathBasedShape.cs` | styleSet, path, deltaAngle, DivFactorY, matrix, ... |
| `src/Vixen.Common/NShape/PolyLineBase.cs` | IntersectsWithCore, currentCell, point, GetAreaCells, cellsRight, ... |
| `src/Vixen.Common/NShape/Polygone.cs` | RegularPolygoneBase.<init>, VertexCount, width, height, RegularPolygoneBase.<init>, ... |
| `src/Vixen.Common/NShape/Project.cs` | GetRegisteredShapeTypes, libraries |
| `src/Vixen.Common/NShape/RectangleShape.cs` | y, index, RectangleBase.<init>, y, IsoscelesTriangleBase, ... |
| `src/Vixen.Common/NShape/RectangularLineBase.cs` | y, IntersectsWithCore, cellSize, y, x, ... |
| `src/Vixen.Common/NShape/Shape.cs` | GetHashCode, tight, Equals, obj, Angle, ... |
| `src/Vixen.Common/NShape/ShapeAggregation.cs` | SetPreviewStyles, PointPositions.<init>, owner, shape, NotifyChildMoved, ... |
| `src/Vixen.Common/NShape/ShapeBase.cs` | height, deltaX, privateLineStyle, distance, IntersectOutlineWithLineSegment, ... |
| `src/Vixen.Common/NShape/ShapeCollection.cs` | tight, GetBoundingRectangle, tight, tight, boundingRectangle, ... |
| `src/Vixen.Common/NShape/ShapeGroup.cs` | width, image, transparentColor, y, shapeType, ... |
| `src/Vixen.Common/NShape/ShapeType.cs` | ShapeType, styleSetProvider, description, GetShapeType, CreateInstanceForLoading, ... |
| `src/Vixen.Common/NShape/ShapeUtils.cs` | pointId, CalcCell, boundingRectangle, cellSize, points, ... |
| `src/Vixen.Common/NShape/Shaper.cs` | points, EllipseFigureShape.<init> |
| `src/Vixen.Common/NShape/Styles.cs` | Tile, Size, Original, ImageLayoutMode, Fit, ... |
| `src/Vixen.Common/NShape/TextShape.cs` | LabelBase.<init>, followingConnectedShape, ControlPointCount, deltaX, DrawHint, ... |
| `src/Vixen.Common/NShape/Tool.cs` | CancelCore |
| `src/Vixen.Common/NShape/ToolCache.cs` | colorStyle, image, colorStyle, lineStyle, angle, ... |
| `src/Vixen.Common/NShape/TriangleBase.cs` | startY, modifiers, ContainsPointCore, x, DivFactorX, ... |
| `src/Vixen.Common/NShapeGeneralShapes/Initializer.cs` | namespaceName, registrar, Initialize, NShapeLibraryInitializer, preferredRepositoryVersion |
| `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs` | shapeType, ContainsPointCore, BodyEndControlPoint, ArrowTopControlPoint, pointId, ... |
| `src/Vixen.Common/NShapeGeneralShapes/QuadrangleShapes.cs` | shapeType, CalculatePath, CreateInstance, shapeType, shapeType, ... |
| `src/Vixen.Common/NShapeGeneralShapes/RoundShapes.cs` | template, template, CreateInstance, shapeType, template, ... |
| `src/Vixen.Common/NShapeGeneralShapes/TriangleShapes.cs` | styleSet, IsoscelesTriangle.<init>, shapeType, FreeTriangle.<init>, styleSet, ... |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | invalidatedEventArgs, r, ScrollAreaBounds, invalidatedAreaBuffer, controlBrushGradientSin, ... |
| `src/Vixen.Common/NShapeWinFormsUI/ExportDiagramDialog.cs` | imageBounds |
| `src/Vixen.Common/NShapeWinFormsUI/FontFamilyListBox.cs` | itemBounds |
| `src/Vixen.Common/NShapeWinFormsUI/InplaceTextBox.cs` | invalidatedArea, DoUpdateBounds, NotifyInvalidate |
| `src/Vixen.Common/NShapeWinFormsUI/StyleListBox.cs` | previewRect, itemBounds, labelLayoutRect |
| `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs` | drawArea, rectBuffer |
| `src/Vixen.Common/NShapeWinFormsUI/UITypeEditors.cs` | designBuffer, previewBounds, previewText, design, charStyle, ... |
| `src/Vixen.Modules/App/CustomPropEditor/Adorners/ResizeAdorner.cs` | rect, Center |
| `src/Vixen.Modules/App/Polygon/Ellipse.cs` | RoundPoints |
| `src/Vixen.Modules/App/Polygon/PointBasedShape.cs` | RoundPoints |
| `src/Vixen.Modules/Editor/FixtureGraphics/OpenGL/Volumes/Rectangle.cs` | Rectangle, UpdateModelMatrix |
| `src/Vixen.Modules/Preview/VixenPreview/PreviewWindowBounds.cs` | PreviewWindowBounds, IsRecoverable, windowBounds, workingAreas |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | g, DrawInfo |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs` | point, PointInShape |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs` | PointInShape, point |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicle.cs` | PointInShape, point |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStar.cs` | point, PointInShape |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` | _bandRect |
| `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs` | Obsolete |
| `src/Vixen.Tests/Preview/VixenPreview/PreviewWindowBoundsTests.cs` | PreviewWindowBoundsTests, height, IsRecoverable_WhenWindowIsOnNegativeCoordinateMonitor_ReturnsTrue, width, IsRecoverable_WhenWindowStartsAtExclusiveWorkingAreaRightEdge_ReturnsFalse, ... |

## Connected Communities

- **Vixen.Common/NShape +5 dirs** (62 cross-edges)
- **Vixen.Common/NShape +64 dirs** (22 cross-edges)
- **Vixen.Common/NShape +8 dirs** (15 cross-edges)
- **Vixen.Common/NShape · IntersectLineWithRectangle** (14 cross-edges)
- **Vixen.Common/NShape · DistancePointPoint · Geometry (141)** (10 cross-edges)
- **Vixen.Common/NShape · CopyFrom** (6 cross-edges)
- **Vixen.Common/NShape · CalcLine · Geometry (163)** (6 cross-edges)
- **Vixen.Common/NShape · LineIntersectsWithLineSegment** (5 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (4 cross-edges)
- **Vixen.Common/NShape · RotatePoint · Geometry (73)** (4 cross-edges)
- **Vixen.Common/NShape · VectorCrossProduct** (3 cross-edges)
- **Vixen.Common/NShape · MeasureText** (3 cross-edges)
- **Vixen.Common/NShape +3 dirs · CreatePreviewStyle** (3 cross-edges)
- **Vixen.Common/NShape · EllipseIntersectsWithRectangle** (3 cross-edges)
- **Vixen.Common · DisposeObject** (3 cross-edges)
- **Vixen.Common/NShape · DrawPath** (3 cross-edges)
- **Vixen.Common/NShape · IsValidCoordinate** (3 cross-edges)
- **Vixen.Common · DoDrawCaptionBounds** (2 cross-edges)
- **Vixen.Common/NShape · IsValidSize** (2 cross-edges)
- **Vixen.Common/NShape · DistancePointPoint · Geometry (32)** (2 cross-edges)
- **Vixen.Common/NShape · Draw** (2 cross-edges)
- **Vixen.Common/NShape · RectangleContainsPoint** (2 cross-edges)
- **Controls/TimeLineControl · timeToPixels** (1 cross-edges)
- **Controls/TimeLineControl +4 dirs · Grid** (1 cross-edges)
- **Vixen.Common/NShape +3 dirs · CachedRepository** (1 cross-edges)
- **Vixen.Common/NShape · CalcNormalVectorOfRectangle** (1 cross-edges)
- **Vixen.Common/NShape · IsStyleAffected** (1 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (1 cross-edges)
- **VixenPreview/Shapes +22 dirs** (1 cross-edges)
- **Editor/TimedSequenceEditor +46 dirs** (1 cross-edges)
- **Vixen.Common/NShape · LineContainsPoint** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-206")
explore(operation:"context", task:"understand Vixen.Common/NShape +13 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

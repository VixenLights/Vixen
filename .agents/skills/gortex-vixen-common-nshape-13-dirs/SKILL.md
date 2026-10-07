---
name: gortex-vixen-common-nshape-13-dirs
description: "Work in the Vixen.Common/NShape +13 dirs area — 2435 symbols across 64 files (77% cohesion)"
---

# Vixen.Common/NShape +13 dirs

2435 symbols | 64 files | 77% cohesion

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
- `src/Vixen.Common/WpfPropertyGrid/KnownTypes.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Adorners/ResizeAdorner.cs`
- `src/Vixen.Modules/App/Polygon/Ellipse.cs`
- `src/Vixen.Modules/App/Polygon/PointBasedShape.cs`
- `src/Vixen.Modules/Editor/FixtureGraphics/OpenGL/Volumes/Rectangle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStar.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs`
- `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/GraphicalPatching/NShapeLibraryInitializer.cs` | namespaceName, registrar, Initialize, preferredRepositoryVersion, NShapeLibraryInitializer |
| `src/Vixen.Application/GraphicalPatching/NestingSetupShape.cs` | shapeType, styleSet, NestingSetupShape.<init> |
| `src/Vixen.Common/Controls/TextProgressBar.cs` | DrawProgressBar, g |
| `src/Vixen.Common/Controls/Theme/ThemeToolStripRenderer.cs` | OnRenderItemBackground, e |
| `src/Vixen.Common/Controls/TimeLineControl/Element.cs` | layer, g, rect, DrawInfo |
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | g, _drawInfo |
| `src/Vixen.Common/Controls/TimeLineControl/RowLabel.cs` | e, OnPaint, GetLabelFont |
| `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` | GetAlignmentInvalidationRectangle, previousTimes, alignmentTime, GetAlignmentInvalidationRectangle, currentTimes |
| `src/Vixen.Common/NShape/CachedRepository.cs` | IsShapeTypeInUse, shapeTypes |
| `src/Vixen.Common/NShape/Caption.cs` | topRight, y, rotationCenterX, InitializeToDefault, FindCaptionFromPoint, ... |
| `src/Vixen.Common/NShape/CircularArcBase.cs` | CalculateConnectionFoot, SweepAngle, arcStartAngle, cellSize, arcBounds, ... |
| `src/Vixen.Common/NShape/Design.cs` | LineStyles, IStyleSet, CapStyles, ParagraphStyles, CharacterStyles, ... |
| `src/Vixen.Common/NShape/Diagram.cs` | colorBrushBounds |
| `src/Vixen.Common/NShape/DiagramPresenter.cs` | sRect, ScreenToDiagram, dRect |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | shapes, source, Copy, withModelObjects, copyCutBounds |
| `src/Vixen.Common/NShape/DiameterShape.cs` | styleSet, sin, index, ContainsPointCore, y, ... |
| `src/Vixen.Common/NShape/GdiHelpers.cs` | center, angleDeg, gfx, radius, center, ... |
| `src/Vixen.Common/NShape/Geometry.cs` | radius, arcRadiusPtY, divFactorX, IsValidSize, arcRadiusPtX, ... |
| `src/Vixen.Common/NShape/ImageBasedShape.cs` | topLeft, index, y, bottomLeft, fromY, ... |
| `src/Vixen.Common/NShape/LinearShape.cs` | rectangle, rotationCenterY, EndCapIntersectsWith, endCapBounds, rectangle, ... |
| `src/Vixen.Common/NShape/PathBasedShape.cs` | DivFactorY, shapeType, source, cos, controlPoints, ... |
| `src/Vixen.Common/NShape/PolyLineBase.cs` | cellSize, CalculateConnectionFoot, stepY, lineRadius, point, ... |
| `src/Vixen.Common/NShape/Polygone.cs` | width, tight, DivFactorX, y, template, ... |
| `src/Vixen.Common/NShape/Project.cs` | libraries, GetRegisteredShapeTypes |
| `src/Vixen.Common/NShape/RectangleShape.cs` | IsoscelesTriangleBase.<init>, MiddleLeftControlPoint, y, TopRightControlPoint, propertyMapping, ... |
| `src/Vixen.Common/NShape/RectangularLineBase.cs` | y, height, y, IntersectsWithCore, CalculateRelativePosition, ... |
| `src/Vixen.Common/NShape/Shape.cs` | RelativePosition, FillStyle, B, Equals, obj, ... |
| `src/Vixen.Common/NShape/ShapeAggregation.cs` | PointPositions.<init>, shape, NotifyChildMoved, SetPreviewStyles, styleSet, ... |
| `src/Vixen.Common/NShape/ShapeBase.cs` | deltaX, cellSize, deltaX, transparentColor, y1, ... |
| `src/Vixen.Common/NShape/ShapeCollection.cs` | GetBoundingRectangle, GetBoundingRectangle, tight, tight, ResetBoundingRectangles, ... |
| `src/Vixen.Common/NShape/ShapeGroup.cs` | x, x, y, shapeType, CalculateNormalVector, ... |
| `src/Vixen.Common/NShape/ShapeType.cs` | GetShapeType, CreateInstanceForLoading, shapeTypes, typeName, Description, ... |
| `src/Vixen.Common/NShape/ShapeUtils.cs` | cellSize, InflateBoundingRectangle, y, pointId, boundingRectangle, ... |
| `src/Vixen.Common/NShape/Shaper.cs` | EllipseFigureShape.<init>, points |
| `src/Vixen.Common/NShape/Styles.cs` | SizeInPoints, Size |
| `src/Vixen.Common/NShape/TextShape.cs` | y, calcInfo, deltaAngle, height, TextBase.<init>, ... |
| `src/Vixen.Common/NShape/Tool.cs` | CancelCore |
| `src/Vixen.Common/NShape/ToolCache.cs` | rectBuffer, GetFont, characterStyle |
| `src/Vixen.Common/NShape/TriangleBase.cs` | ControlPoint2, TriangleBase.<init>, InitializeToDefault, TriangleBase.<init>, relativePosition, ... |
| `src/Vixen.Common/NShapeGeneralShapes/Initializer.cs` | Initialize, registrar, preferredRepositoryVersion, NShapeLibraryInitializer, namespaceName |
| `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs` | ProcessExecModelPropertyChange, BodyTopControlPoint, modifiers, ThickArrow.<init>, Fit, ... |
| `src/Vixen.Common/NShapeGeneralShapes/QuadrangleShapes.cs` | shapeType, CreateInstance, shapeType, template, CalcCornerRadius, ... |
| `src/Vixen.Common/NShapeGeneralShapes/RoundShapes.cs` | styleSet, shapeType, Ellipse.<init>, CreateInstance, CalculatePath, ... |
| `src/Vixen.Common/NShapeGeneralShapes/TriangleShapes.cs` | CreateInstance, styleSet, shapeType, styleSet, template, ... |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | currentPos, DisplayInvalidatedEventArgs, BackgroundGradientAngle, invalidationBuffer, GetUniversalScrollCursor, ... |
| `src/Vixen.Common/NShapeWinFormsUI/ExportDiagramDialog.cs` | imageBounds |
| `src/Vixen.Common/NShapeWinFormsUI/FontFamilyListBox.cs` | itemBounds |
| `src/Vixen.Common/NShapeWinFormsUI/InplaceTextBox.cs` | invalidatedArea, NotifyInvalidate, DoUpdateBounds |
| `src/Vixen.Common/NShapeWinFormsUI/StyleListBox.cs` | labelLayoutRect, itemBounds, previewRect |
| `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs` | drawArea, rectBuffer |
| `src/Vixen.Common/NShapeWinFormsUI/UITypeEditors.cs` | paragraphStyle, gfx, StyleUITypeEditor.<init>, previewBounds, IsDropDownResizable, ... |
| `src/Vixen.Common/WpfPropertyGrid/KnownTypes.cs` | Point3DCollection, Geometry, Size, VectorCollection, Quaternion, ... |
| `src/Vixen.Modules/App/CustomPropEditor/Adorners/ResizeAdorner.cs` | rect, Center |
| `src/Vixen.Modules/App/Polygon/Ellipse.cs` | RoundPoints |
| `src/Vixen.Modules/App/Polygon/PointBasedShape.cs` | RoundPoints |
| `src/Vixen.Modules/Editor/FixtureGraphics/OpenGL/Volumes/Rectangle.cs` | Rectangle, UpdateModelMatrix |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | g, DrawInfo |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs` | PointInShape, point |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs` | PointInShape, point |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicle.cs` | point, PointInShape |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs` | fp, states, zoomLevel, Draw |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStar.cs` | point, PointInShape |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` | _bandRect |
| `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs` | Obsolete |

## Connected Communities

- **Vixen.Common/NShape +4 dirs** (59 cross-edges)
- **Vixen.Common/NShape +8 dirs** (23 cross-edges)
- **Vixen.Common/NShape +70 dirs** (20 cross-edges)
- **Vixen.Common/NShape · IntersectLineWithRectangle** (14 cross-edges)
- **Vixen.Common/NShape · DistancePointPoint · Geometry (141)** (10 cross-edges)
- **Vixen.Common/NShape · Invalidate** (9 cross-edges)
- **Vixen.Common/NShape · ImageBasedShape** (7 cross-edges)
- **Vixen.Common/NShape · CalcLine · Geometry (163)** (6 cross-edges)
- **Vixen.Common/NShape · LineIntersectsWithLineSegment** (5 cross-edges)
- **Vixen.Common/NShape · RotatePoint · Geometry (73)** (4 cross-edges)
- **Vixen.Common/NShape · MeasureText** (3 cross-edges)
- **Vixen.Common · DisposeObject** (3 cross-edges)
- **Vixen.Common/NShape · EllipseIntersectsWithRectangle** (3 cross-edges)
- **Vixen.Common/NShape · IsValidCoordinate** (3 cross-edges)
- **Vixen.Common/NShape · VectorCrossProduct** (3 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (2 cross-edges)
- **Vixen.Common/NShape · Draw** (2 cross-edges)
- **Vixen.Common/NShape · DistancePointPoint · Geometry (32)** (2 cross-edges)
- **Vixen.Common/NShape · RectangleContainsPoint** (2 cross-edges)
- **Vixen.Common/NShape · IsValidSize** (2 cross-edges)
- **Vixen.Common · DoDrawCaptionBounds** (2 cross-edges)
- **Vixen.Common · GdiHelpers** (2 cross-edges)
- **Vixen.Common/NShape · ImageLayoutMode** (2 cross-edges)
- **Vixen.Common/NShape · DrawPath** (2 cross-edges)
- **Vixen.Common/NShape · CalcNormalVectorOfRectangle** (1 cross-edges)
- **Controls/TimeLineControl +7 dirs** (1 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (1 cross-edges)
- **Vixen.Common/NShape · LineContainsPoint** (1 cross-edges)
- **VixenPreview/Undo +11 dirs** (1 cross-edges)
- **Editor/TimedSequenceEditor +44 dirs** (1 cross-edges)
- **VixenPreview/Shapes +22 dirs** (1 cross-edges)
- **Vixen.Common/NShape · GetImageAttributes** (1 cross-edges)
- **Vixen.Common/NShape · PictureBase** (1 cross-edges)
- **Vixen.Common/NShape +3 dirs · CachedRepository** (1 cross-edges)
- **Controls/TimeLineControl · timeToPixels** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-201")
explore(operation:"context", task:"understand Vixen.Common/NShape +13 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

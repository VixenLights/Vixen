---
name: gortex-polygoneditor-viewmodels-8-dirs
description: "Work in the PolygonEditor/ViewModels +8 dirs area — 784 symbols across 33 files (90% cohesion)"
---

# PolygonEditor/ViewModels +8 dirs

784 symbols | 33 files | 90% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/NShape/Command.cs`
- `src/Vixen.Common/NShape/History.cs`
- `src/Vixen.Common/NShape/LayerController.cs`
- `src/Vixen.Common/NShape/Project.cs`
- `src/Vixen.Common/NShape/Styles.cs`
- `src/Vixen.Common/NShapeWinFormsUI/Display.cs`
- `src/Vixen.Common/WPFCommon/Behaviors/DataGridSortedItemsCommandBehavior.cs`
- `src/Vixen.Modules/App/Polygon/Ellipse.cs`
- `src/Vixen.Modules/App/Polygon/Line.cs`
- `src/Vixen.Modules/App/Polygon/PointBasedShape.cs`
- `src/Vixen.Modules/App/Polygon/Polygon.cs`
- `src/Vixen.Modules/App/Polygon/PolygonContainer.cs`
- `src/Vixen.Modules/App/Polygon/PolygonPoint.cs`
- `src/Vixen.Modules/App/Polygon/Shape.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/Converters/PointCollectionConverter.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/Converters/PolygonPointXConverter.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/Converters/PolygonPointYConverter.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ValidationRules/XValidationRule.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ValidationRules/YValidationRule.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/EllipseViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/LineSegmentViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/LineViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PointBasedViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonEditorViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonLineSegment.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonPointViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonSnapShotViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/ShapeViewModel.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/Views/PolygonControl.xaml.cs`
- `src/Vixen.Modules/Editor/PolygonEditor/Views/PolygonEditorView.xaml.cs`
- `src/Vixen.Modules/Effect/Morph/Morph/Morph.cs`
- `src/Vixen.Modules/Effect/Morph/Morph/MorphPolygon.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/NShape/Command.cs` | Description, Repository, commands, commands, ICommand, ... |
| `src/Vixen.Common/NShape/History.cs` | AddCommand, BeginAggregatingCommands, GetCommandEventArgs, commandEventArgsBuffer, commandCount, ... |
| `src/Vixen.Common/NShape/LayerController.cs` | upperZoomBounds, SetLayerZoomBounds, layer, lowerZoomBounds, diagram |
| `src/Vixen.Common/NShape/Project.cs` | ExecuteCommand, command |
| `src/Vixen.Common/NShape/Styles.cs` | index, RemoveAt |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | CreateUndoMenuItemDef, CreateRedoMenuItemDef |
| `src/Vixen.Common/WPFCommon/Behaviors/DataGridSortedItemsCommandBehavior.cs` | SortedItemsCommand |
| `src/Vixen.Modules/App/Polygon/Ellipse.cs` | StartSideRotation, Ellipse, Angle, Height, Ellipse.<init>, ... |
| `src/Vixen.Modules/App/Polygon/Line.cs` | Clone, Line.<init>, Line |
| `src/Vixen.Modules/App/Polygon/PointBasedShape.cs` | xScaleFactor, sourceShape, FillType, ScalePoints, height, ... |
| `src/Vixen.Modules/App/Polygon/Polygon.cs` | Clone, Polygon, Polygon.<init> |
| `src/Vixen.Modules/App/Polygon/PolygonContainer.cs` | DisplayElementHeight, Width, LineTimes, ShowDisplayElement, PolygonContainer.<init>, ... |
| `src/Vixen.Modules/App/Polygon/PolygonPoint.cs` | Y, Clone, X, PolygonPoint |
| `src/Vixen.Modules/App/Polygon/Shape.cs` | xScaleFactor, height, sourceShape, width, Copy, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Converters/PointCollectionConverter.cs` | PointCollectionConverter, value, culture, targetType, targetType, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Converters/PolygonPointXConverter.cs` | targetType, targetType, parameter, parameter, PolygonPointXConverter, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Converters/PolygonPointYConverter.cs` | BufferHt, Convert, YScaleFactor, targetType, parameter, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ValidationRules/XValidationRule.cs` | XValidationRule, Width, cultureInfo, Validate, value |
| `src/Vixen.Modules/Editor/PolygonEditor/ValidationRules/YValidationRule.cs` | Validate, Height, value, YValidationRule, cultureInfo |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/EllipseViewModel.cs` | Top, ToggleStartSide, EllipseViewModel, HeightProperty, AddPoint, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/LineSegmentViewModel.cs` | CenterPointColorProperty, point2, Color, Point1, Point2, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/LineViewModel.cs` | AddPoint, ToggleStartPoint, Line, EndPoint, line, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PointBasedViewModel.cs` | SegmentsProperty, PointBasedViewModel.<init>, PointBasedViewModel, labelVisible, NotifyPointCollectionChanged, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonEditorViewModel.cs` | CanvasMouseLeftButtonDown, TimeBarVisibleProperty, snapshot, InitializePolygonSnapshots, MouseLeftButtonUpTimeBarCommand, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonLineSegment.cs` | EndPoint, Line, PolygonLineSegment, PolygonLineSegment.<init>, StartPoint |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonPointViewModel.cs` | PolygonPointProperty, PolygonPointViewModel, LabelProperty, GetPoint, ColorProperty, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonSnapShotViewModel.cs` | Initialize, Time, LineViewModel, position, IsMouseOverTimeBar, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonViewModel.cs` | IsMouseOverFirstPolygonPoint, pt, AddPoint, position, Dirty, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/ShapeViewModel.cs` | position, MoveSelectedPoint, labelVisible, position, UpdatePointLabels, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Views/PolygonControl.xaml.cs` | e, _rubberbandAdorner, e, DisplayResizeAdornerForSelectedPoints, sender, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Views/PolygonEditorView.xaml.cs` | PolygonEditorView, Lines, _vm, _polygonContainer, LineTimes, ... |
| `src/Vixen.Modules/Effect/Morph/Morph/Morph.cs` | x0_, CalculateDirection, time, GetLengthOfWipePolygon, StoreLine, ... |
| `src/Vixen.Modules/Effect/Morph/Morph/MorphPolygon.cs` | width, Label, MorphPolygon.<init>, height, LimitPoints |

## Connected Communities

- **Vixen.Modules · Morph** (5 cross-edges)
- **Effect/Effect +104 dirs** (4 cross-edges)
- **Vixen.Common/NShape +70 dirs** (3 cross-edges)
- **Vixen.Common · Paste** (2 cross-edges)
- **Vixen.Common · Project** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1117")
explore(operation:"context", task:"understand PolygonEditor/ViewModels +8 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

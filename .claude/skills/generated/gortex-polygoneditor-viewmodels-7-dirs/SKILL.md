---
name: gortex-polygoneditor-viewmodels-7-dirs
description: "Work in the PolygonEditor/ViewModels +7 dirs area — 708 symbols across 29 files (90% cohesion)"
---

# PolygonEditor/ViewModels +7 dirs

708 symbols | 29 files | 90% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/NShape/Command.cs`
- `src/Vixen.Common/NShape/History.cs`
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
| `src/Vixen.Common/NShape/Command.cs` | AggregatedCommand.<init>, Description, AggregatedCommand.<init>, commands, commands, ... |
| `src/Vixen.Common/NShape/History.cs` | CommandEventArgs.<init>, Reverted, CommandEventArgs, History.<init>, reverted, ... |
| `src/Vixen.Common/WPFCommon/Behaviors/DataGridSortedItemsCommandBehavior.cs` | SortedItemsCommand |
| `src/Vixen.Modules/App/Polygon/Ellipse.cs` | Width, Clone, Angle, Center, RoundPoints, ... |
| `src/Vixen.Modules/App/Polygon/Line.cs` | Line.<init>, Line, Clone |
| `src/Vixen.Modules/App/Polygon/PointBasedShape.cs` | LimitPoints, width, RoundPoints, FillType, PointBasedShape, ... |
| `src/Vixen.Modules/App/Polygon/Polygon.cs` | Polygon.<init>, Clone, Polygon |
| `src/Vixen.Modules/App/Polygon/PolygonContainer.cs` | PolygonTimes, EllipseTimes, Ellipses, LineTimes, ShowDisplayElement, ... |
| `src/Vixen.Modules/App/Polygon/PolygonPoint.cs` | PolygonPoint, Clone, X, Y |
| `src/Vixen.Modules/App/Polygon/Shape.cs` | height, Copy, Scale, Shape.<init>, ID, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Converters/PointCollectionConverter.cs` | targetType, culture, parameter, Convert, culture, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Converters/PolygonPointXConverter.cs` | XScaleFactor, ConvertBack, value, targetType, targetType, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Converters/PolygonPointYConverter.cs` | culture, targetType, YScaleFactor, value, BufferHt, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ValidationRules/XValidationRule.cs` | value, Width, XValidationRule, cultureInfo, Validate |
| `src/Vixen.Modules/Editor/PolygonEditor/ValidationRules/YValidationRule.cs` | value, Height, cultureInfo, Validate, YValidationRule |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/EllipseViewModel.cs` | Top, UpdateCenterPoint, ellipseModel, Ellipse, WidthProperty, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/LineSegmentViewModel.cs` | Point2, Color, point1, point2, CenterPointColorProperty, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/LineViewModel.cs` | position, EndPointProperty, line, UpdateCenterPoint, LineViewModel.<init>, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PointBasedViewModel.cs` | PointBasedViewModel, labelVisible, SegmentsVisible, NotifyPointCollectionChanged, PointBasedViewModel.<init>, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonEditorViewModel.cs` | CanvasWidthProperty, SelectAllPointsOnLine, IsSelecting, AddEllipse, SelectedEllipse, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonLineSegment.cs` | PolygonLineSegment.<init>, StartPoint, EndPoint, PolygonLineSegment, Line |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonPointViewModel.cs` | XProperty, SuppressChangeEvents, PolygonPointProperty, PolygonPointViewModel.<init>, DeselectedColor, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonSnapShotViewModel.cs` | PolygonSnapshotViewModel.<init>, mousePosition, SelectedProperty, Initialize, position, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/PolygonViewModel.cs` | e, pt, InsertPoint, PolygonViewModel, insertPosition, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/ViewModels/ShapeViewModel.cs` | LabelVisibleProperty, limitPointToCanvas, point, IsOverCenterCrossHash, ShapeViewModel, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Views/PolygonControl.xaml.cs` | PolygonControl_Loaded, sender, UpdatePolygonLines, e, DisplayResizeAdorner, ... |
| `src/Vixen.Modules/Editor/PolygonEditor/Views/PolygonEditorView.xaml.cs` | polygonContainer, _vm, PolygonEditorView, EllipseTimes, PolygonTimes, ... |
| `src/Vixen.Modules/Effect/Morph/Morph/Morph.cs` | TargetNodesChanged |
| `src/Vixen.Modules/Effect/Morph/Morph/MorphPolygon.cs` | width, height, LimitPoints, Label |

## Connected Communities

- **Vixen.Common/NShape +69 dirs** (8 cross-edges)
- **Vixen.Modules · Morph** (2 cross-edges)
- **Vixen.Common/NShape +4 dirs** (1 cross-edges)
- **App/Curves +53 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1150")
explore(operation:"context", task:"understand PolygonEditor/ViewModels +7 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

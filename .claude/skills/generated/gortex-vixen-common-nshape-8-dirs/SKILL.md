---
name: gortex-vixen-common-nshape-8-dirs
description: "Work in the Vixen.Common/NShape +8 dirs area — 2229 symbols across 49 files (79% cohesion)"
---

# Vixen.Common/NShape +8 dirs

2229 symbols | 49 files | 79% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/GraphicalPatching/ConnectionTool.cs`
- `src/Vixen.Common/NShape/CachedRepository.cs`
- `src/Vixen.Common/NShape/Collections.cs`
- `src/Vixen.Common/NShape/Command.cs`
- `src/Vixen.Common/NShape/Diagram.cs`
- `src/Vixen.Common/NShape/DiagramController.cs`
- `src/Vixen.Common/NShape/DiagramPresenter.cs`
- `src/Vixen.Common/NShape/DiagramSetController.cs`
- `src/Vixen.Common/NShape/Exceptions.cs`
- `src/Vixen.Common/NShape/FlowLayouter.cs`
- `src/Vixen.Common/NShape/History.cs`
- `src/Vixen.Common/NShape/ImageBasedShape.cs`
- `src/Vixen.Common/NShape/Layer.cs`
- `src/Vixen.Common/NShape/LayerController.cs`
- `src/Vixen.Common/NShape/LayerPresenter.cs`
- `src/Vixen.Common/NShape/Layouter.cs`
- `src/Vixen.Common/NShape/MenuItemDef.cs`
- `src/Vixen.Common/NShape/Model.cs`
- `src/Vixen.Common/NShape/PropertyController.cs`
- `src/Vixen.Common/NShape/Repository.cs`
- `src/Vixen.Common/NShape/Shape.cs`
- `src/Vixen.Common/NShape/ShapeAggregation.cs`
- `src/Vixen.Common/NShape/ShapeBase.cs`
- `src/Vixen.Common/NShape/ShapeCollection.cs`
- `src/Vixen.Common/NShape/ShapeDuplicator.cs`
- `src/Vixen.Common/NShape/ShapeGroup.cs`
- `src/Vixen.Common/NShape/ShapeType.cs`
- `src/Vixen.Common/NShape/Store.cs`
- `src/Vixen.Common/NShape/TemplateController.cs`
- `src/Vixen.Common/NShape/TextShape.cs`
- `src/Vixen.Common/NShape/Tool.cs`
- `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs`
- `src/Vixen.Common/NShapeGeneralShapes/QuadrangleShapes.cs`
- `src/Vixen.Common/NShapeGeneralShapes/TextShapes.cs`
- `src/Vixen.Common/NShapeGeneralShapes/TriangleShapes.cs`
- `src/Vixen.Common/NShapeWinFormsUI/Display.cs`
- `src/Vixen.Common/NShapeWinFormsUI/LayoutDialog.cs`
- `src/Vixen.Common/NShapeWinFormsUI/ModelTreeViewPresenter.cs`
- `src/Vixen.Common/NShapeWinFormsUI/ShapeInfoDialog.cs`
- `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs`
- `src/Vixen.Common/NShapeWinFormsUI/UITypeEditors.cs`
- `src/Vixen.Common/NShapeWinFormsUI/WinFormHelpers.cs`
- `src/Vixen.Core/IO/EmptyMigrator.cs`
- `src/Vixen.Core/IO/IContentMigrator.cs`
- `src/Vixen.Core/IO/IMigrationSegment.cs`
- `src/Vixen.Core/IO/Xml/ModuleStore/ModuleStoreXElementMigrator.cs`
- `src/Vixen.Core/IO/Xml/SystemConfig/SystemConfigXElementMigrator.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs`
- `src/Vixen.Modules/Sequence/Timed/TimedSequenceMigrator.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/GraphicalPatching/ConnectionTool.cs` | originalShape, RemovePreviewOf |
| `src/Vixen.Common/NShape/CachedRepository.cs` | gluePointId, HasOtherChildren, CanDelete, allShapes, shapes, ... |
| `src/Vixen.Common/NShape/Collections.cs` | ReadOnlyList, capacity, ReadOnlyList.<init>, GetLayerBits, collection, ... |
| `src/Vixen.Common/NShape/Command.cs` | shape, modifiedShapes, toY, baseShape, exception, ... |
| `src/Vixen.Common/NShape/Diagram.cs` | shape, RemoveShapesFromLayers, margin, GetShapeLayers, name, ... |
| `src/Vixen.Common/NShape/DiagramController.cs` | diagram, CreateDiagram, name, owner, Tool, ... |
| `src/Vixen.Common/NShape/DiagramPresenter.cs` | shapes, DiagramPresenterShapesEventArgs.<init>, UserMessageEventArgs.<init>, ScreenToDiagram, messageText, ... |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | shapes, SplitCompositeShape, diagram, diagram, diagram, ... |
| `src/Vixen.Common/NShape/Exceptions.cs` | NShapeInternalException.<init>, NShapeInternalException.<init>, format, NShapeInternalException.<init>, NShapeInternalException.<init>, ... |
| `src/Vixen.Common/NShape/FlowLayouter.cs` | layerShapes, layers |
| `src/Vixen.Common/NShape/History.cs` | commands |
| `src/Vixen.Common/NShape/ImageBasedShape.cs` | dataSize, flags, flags, data, dataSize, ... |
| `src/Vixen.Common/NShape/Layer.cs` | Layer21, Layer15, LayerIds, All, Layer02, ... |
| `src/Vixen.Common/NShape/LayerController.cs` | diagram, SetLayers, layers, GetNewLayerName |
| `src/Vixen.Common/NShape/LayerPresenter.cs` | GetSelectedLayerIds, selectedLayers |
| `src/Vixen.Common/NShape/Layouter.cs` | AllShapes, allShapes, Shapes, selectedShapes, Shapes, ... |
| `src/Vixen.Common/NShape/MenuItemDef.cs` | Name, Image, Execute, Checked, name, ... |
| `src/Vixen.Common/NShape/Model.cs` | shapes, Shapes, Shapes |
| `src/Vixen.Common/NShape/PropertyController.cs` | objects, objects, SetObjects |
| `src/Vixen.Common/NShape/Repository.cs` | shape, Insert, id, diagram, DeleteAll, ... |
| `src/Vixen.Common/NShape/Shape.cs` | InitializeToDefault, source, Dispose, graphics, zOrder, ... |
| `src/Vixen.Common/NShape/ShapeAggregation.cs` | AddShapePosition, ReplaceCore, deltaX, relativePositions, Construct, ... |
| `src/Vixen.Common/NShape/ShapeBase.cs` | TopDown, item, GetEnumerator, item, CreateInstanceForTemplate, ... |
| `src/Vixen.Common/NShape/ShapeCollection.cs` | shape, Clone, controlPointCapabilities, AddShapeToIndex, ShapeCollection.<init>, ... |
| `src/Vixen.Common/NShape/ShapeDuplicator.cs` | shape, CloneShapeOnly |
| `src/Vixen.Common/NShape/ShapeGroup.cs` | NotifyChildLayoutChanging, Diagram, Parent, NotifyChildLayoutChanged, IShapeGroup, ... |
| `src/Vixen.Common/NShape/ShapeType.cs` | template, CreateInstance, CreateInstance |
| `src/Vixen.Common/NShape/Store.cs` | bucket, Add |
| `src/Vixen.Common/NShape/TemplateController.cs` | oldTemplateShape, TemplateControllerTemplateShapeReplacedEventArgs.<init>, template, OldTemplateShape, SetTemplateShape, ... |
| `src/Vixen.Common/NShape/TextShape.cs` | gluePointId, Disconnect |
| `src/Vixen.Common/NShape/Tool.cs` | previewShape, originalShape, shapeBuffer, shapesInRange, RemovePreview, ... |
| `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs` | shapeType, template, template, shapeType, RegularPolygone, ... |
| `src/Vixen.Common/NShapeGeneralShapes/QuadrangleShapes.cs` | template, shapeType, template, Clone, CreateInstance, ... |
| `src/Vixen.Common/NShapeGeneralShapes/TextShapes.cs` | shapeType, shapeType, Text.<init>, template, Clone, ... |
| `src/Vixen.Common/NShapeGeneralShapes/TriangleShapes.cs` | CreateInstance, template, shapeType, shapeType, shapeType, ... |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | diagram, scrollBar_KeyDown, shapes, withModelObjects, shapes, ... |
| `src/Vixen.Common/NShapeWinFormsUI/LayoutDialog.cs` | selectedShapes, SelectedShapes |
| `src/Vixen.Common/NShapeWinFormsUI/ModelTreeViewPresenter.cs` | selectedModelObjects |
| `src/Vixen.Common/NShapeWinFormsUI/ShapeInfoDialog.cs` | diagram |
| `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs` | Shape, ShapeItem.<init>, itemIndex, shapesComboBox_SelectedIndexChanged, e, ... |
| `src/Vixen.Common/NShapeWinFormsUI/UITypeEditors.cs` | context, provider, value, EditValue |
| `src/Vixen.Common/NShapeWinFormsUI/WinFormHelpers.cs` | GetKeyEventArgs, eventType, e |
| `src/Vixen.Core/IO/EmptyMigrator.cs` | ValidMigrations |
| `src/Vixen.Core/IO/IContentMigrator.cs` | ValidMigrations |
| `src/Vixen.Core/IO/IMigrationSegment.cs` | IMigrationSegment, ToVersion, T, FromVersion |
| `src/Vixen.Core/IO/Xml/ModuleStore/ModuleStoreXElementMigrator.cs` | ValidMigrations |
| `src/Vixen.Core/IO/Xml/SystemConfig/SystemConfigXElementMigrator.cs` | ValidMigrations |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | Equals, obj |
| `src/Vixen.Modules/Sequence/Timed/TimedSequenceMigrator.cs` | ValidMigrations |

## Connected Communities

- **Vixen.Common/NShape +13 dirs** (17 cross-edges)
- **Vixen.Common/NShape +64 dirs** (14 cross-edges)
- **Vixen.Common/NShape +3 dirs · PerformSelection** (9 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (8 cross-edges)
- **Vixen.Common · Paste** (6 cross-edges)
- **Vixen.Common · Contains** (6 cross-edges)
- **Vixen.Common/NShape +5 dirs** (5 cross-edges)
- **Vixen.Common · SelectShapes** (4 cross-edges)
- **Vixen.Common/NShape +3 dirs · CachedRepository** (4 cross-edges)
- **Vixen.Common/NShape · Revert** (3 cross-edges)
- **Vixen.Common · OnMouseWheel** (2 cross-edges)
- **Vixen.Common · History** (2 cross-edges)
- **Vixen.Common/NShape · DoCloneShape** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · GetKeyEventArgs** (1 cross-edges)
- **Vixen.Common · ExportDiagramDialog** (1 cross-edges)
- **Vixen.Common/NShape · MultiHashList** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · DoOpenCaptionEditor** (1 cross-edges)
- **Vixen.Common · Display** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-271")
explore(operation:"context", task:"understand Vixen.Common/NShape +8 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

---
name: gortex-vixen-common-nshape-8-dirs
description: "Work in the Vixen.Common/NShape +8 dirs area — 2342 symbols across 49 files (79% cohesion)"
---

# Vixen.Common/NShape +8 dirs

2342 symbols | 49 files | 79% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/GraphicalPatching/ConnectionTool.cs`
- `src/Vixen.Common/NShape/CachedRepository.cs`
- `src/Vixen.Common/NShape/Collections.cs`
- `src/Vixen.Common/NShape/Command.cs`
- `src/Vixen.Common/NShape/Core.cs`
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
| `src/Vixen.Application/GraphicalPatching/ConnectionTool.cs` | RemovePreviewOf, originalShape |
| `src/Vixen.Common/NShape/CachedRepository.cs` | withChildren, shapesToCheck, shapes, CanDelete, shapeEventArgs, ... |
| `src/Vixen.Common/NShape/Collections.cs` | diagram, T, ReadOnlyList.<init>, collection, GetLayerBits, ... |
| `src/Vixen.Common/NShape/Command.cs` | shapes, parentShape, unrotatedLinePoints, newValues, attachedObjects, ... |
| `src/Vixen.Common/NShape/Core.cs` | NotifyBoundsChanged, InfoGraphics, HintBackgroundStyle, HintForegroundStyle, IDisplayService |
| `src/Vixen.Common/NShape/Diagram.cs` | shapes, imageFormat, capacity, CheckOwnerboundsUpdateNeeded, layerIds, ... |
| `src/Vixen.Common/NShape/DiagramController.cs` | Diagram, Project, OpenDiagram, Owner, name, ... |
| `src/Vixen.Common/NShape/DiagramPresenter.cs` | messageText, shapes, UserMessageEventArgs.<init>, DiagramPresenterShapesEventArgs.<init>, shapes, ... |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | project_ProjectOpen, diagram, e, liftMode, diagram, ... |
| `src/Vixen.Common/NShape/Exceptions.cs` | NShapeInternalException.<init>, NShapeSecurityException, info, innerException, args, ... |
| `src/Vixen.Common/NShape/FlowLayouter.cs` | layers, layerShapes |
| `src/Vixen.Common/NShape/History.cs` | commands |
| `src/Vixen.Common/NShape/ImageBasedShape.cs` | flags, MetafileCallback, Clone, recordType, data, ... |
| `src/Vixen.Common/NShape/Layer.cs` | Layer09, Layer27, Layer30, Layer12, Layer11, ... |
| `src/Vixen.Common/NShape/LayerController.cs` | diagram, GetNewLayerName |
| `src/Vixen.Common/NShape/LayerPresenter.cs` | GetSelectedLayerIds, selectedLayers |
| `src/Vixen.Common/NShape/Layouter.cs` | allShapes, Shapes, AllShapes, Shapes, selectedShapes, ... |
| `src/Vixen.Common/NShape/MenuItemDef.cs` | Execute, Name, IsGranted, SeparatorMenuItemDef.<init>, ImageTransparentColor, ... |
| `src/Vixen.Common/NShape/Model.cs` | shape, shape, Shapes, Shapes, AttachShape, ... |
| `src/Vixen.Common/NShape/PropertyController.cs` | objects, objects, SetObjects |
| `src/Vixen.Common/NShape/Repository.cs` | shapes, parentShape, diagram, diagram, AddEntityType, ... |
| `src/Vixen.Common/NShape/Shape.cs` | propertyId, CalculateNormalVector, x, ownPointId, width, ... |
| `src/Vixen.Common/NShape/ShapeAggregation.cs` | ResizableShapeAggregation.<init>, PointPositions.<init>, collection, collection, CopyFrom, ... |
| `src/Vixen.Common/NShape/ShapeBase.cs` | item, Contains, lastFound, oldShape, item, ... |
| `src/Vixen.Common/NShape/ShapeCollection.cs` | newShapes, IsSynchronized, shape, occupiedBrush, array, ... |
| `src/Vixen.Common/NShape/ShapeDuplicator.cs` | shape, CloneShapeOnly |
| `src/Vixen.Common/NShape/ShapeGroup.cs` | Parent, IShapeGroup, DisplayService, Clone, NotifyChildLayoutChanged, ... |
| `src/Vixen.Common/NShape/ShapeType.cs` | CreateInstance, CreateInstance, template |
| `src/Vixen.Common/NShape/Store.cs` | Add, bucket |
| `src/Vixen.Common/NShape/TemplateController.cs` | TemplateControllerTemplateShapeReplacedEventArgs.<init>, SetTemplateShape, newTemplateShape, template, oldTemplateShape, ... |
| `src/Vixen.Common/NShape/TextShape.cs` | Disconnect, gluePointId |
| `src/Vixen.Common/NShape/Tool.cs` | shapesInRange, originalShape, previewShape, RemovePreview, RemovePreviewOf, ... |
| `src/Vixen.Common/NShapeGeneralShapes/MiscShapes.cs` | template, template, Picture.<init>, shapeType, RegularPolygone.<init>, ... |
| `src/Vixen.Common/NShapeGeneralShapes/QuadrangleShapes.cs` | Clone, shapeType, CreateInstance, Diamond.<init>, shapeType, ... |
| `src/Vixen.Common/NShapeGeneralShapes/TextShapes.cs` | CreateInstance, template, Clone, Text.<init>, Text, ... |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | Delete, layers, sender, shapes, withModelObjects, ... |
| `src/Vixen.Common/NShapeWinFormsUI/LayoutDialog.cs` | SelectedShapes, selectedShapes |
| `src/Vixen.Common/NShapeWinFormsUI/ModelTreeViewPresenter.cs` | selectedModelObjects |
| `src/Vixen.Common/NShapeWinFormsUI/ShapeInfoDialog.cs` | diagram |
| `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs` | shape, shape, ShapeItem, shapesComboBox_SelectedIndexChanged, Index, ... |
| `src/Vixen.Common/NShapeWinFormsUI/UITypeEditors.cs` | value, provider, EditValue, context |
| `src/Vixen.Common/NShapeWinFormsUI/WinFormHelpers.cs` | eventType, e, GetKeyEventArgs |
| `src/Vixen.Core/IO/EmptyMigrator.cs` | ValidMigrations |
| `src/Vixen.Core/IO/IContentMigrator.cs` | ValidMigrations |
| `src/Vixen.Core/IO/IMigrationSegment.cs` | IMigrationSegment, ToVersion, T, FromVersion |
| `src/Vixen.Core/IO/Xml/ModuleStore/ModuleStoreXElementMigrator.cs` | ValidMigrations |
| `src/Vixen.Core/IO/Xml/SystemConfig/SystemConfigXElementMigrator.cs` | ValidMigrations |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | obj, Equals |
| `src/Vixen.Modules/Sequence/Timed/TimedSequenceMigrator.cs` | ValidMigrations |

## Connected Communities

- **Vixen.Common/NShape +69 dirs** (16 cross-edges)
- **Vixen.Common/NShape +12 dirs** (13 cross-edges)
- **Vixen.Common/NShape +3 dirs · PerformSelection** (9 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (8 cross-edges)
- **Vixen.Common · Contains** (6 cross-edges)
- **Vixen.Common · Paste** (6 cross-edges)
- **Vixen.Common/NShape +4 dirs** (6 cross-edges)
- **Vixen.Common/NShape +3 dirs · CachedRepository** (4 cross-edges)
- **Vixen.Common · SelectShapes** (4 cross-edges)
- **Vixen.Common/NShape · Revert** (3 cross-edges)
- **Vixen.Common · History** (2 cross-edges)
- **Vixen.Common · OnMouseWheel** (2 cross-edges)
- **Vixen.Common · Display** (1 cross-edges)
- **Vixen.Common/NShape · MultiHashList** (1 cross-edges)
- **Vixen.Common/NShape · DoCloneShape** (1 cross-edges)
- **Vixen.Common/NShape · ImageBasedShape** (1 cross-edges)
- **Vixen.Common · GdiHelpers** (1 cross-edges)
- **Vixen.Common · ExportDiagramDialog** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · GetKeyEventArgs** (1 cross-edges)
- **Vixen.Common/NShapeWinFormsUI · DoOpenCaptionEditor** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-286")
explore(operation:"context", task:"understand Vixen.Common/NShape +8 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

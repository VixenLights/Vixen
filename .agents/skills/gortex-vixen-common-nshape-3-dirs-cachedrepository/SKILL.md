---
name: gortex-vixen-common-nshape-3-dirs-cachedrepository
description: "Work in the Vixen.Common/NShape +3 dirs · CachedRepository area — 2308 symbols across 34 files (85% cohesion)"
---

# Vixen.Common/NShape +3 dirs · CachedRepository

2308 symbols | 34 files | 85% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/NShape/AdoNetStore.cs`
- `src/Vixen.Common/NShape/Buffers.cs`
- `src/Vixen.Common/NShape/CachedRepository.cs`
- `src/Vixen.Common/NShape/Collections.cs`
- `src/Vixen.Common/NShape/Command.cs`
- `src/Vixen.Common/NShape/Core.cs`
- `src/Vixen.Common/NShape/Design.cs`
- `src/Vixen.Common/NShape/DesignController.cs`
- `src/Vixen.Common/NShape/DiagramSetController.cs`
- `src/Vixen.Common/NShape/Entity.cs`
- `src/Vixen.Common/NShape/Exceptions.cs`
- `src/Vixen.Common/NShape/FlowLayouter.cs`
- `src/Vixen.Common/NShape/FreeHandTool.cs`
- `src/Vixen.Common/NShape/Geometry.cs`
- `src/Vixen.Common/NShape/Model.cs`
- `src/Vixen.Common/NShape/ModelController.cs`
- `src/Vixen.Common/NShape/Project.cs`
- `src/Vixen.Common/NShape/PropertyController.cs`
- `src/Vixen.Common/NShape/PropertyMappings.cs`
- `src/Vixen.Common/NShape/Repository.cs`
- `src/Vixen.Common/NShape/Shape.cs`
- `src/Vixen.Common/NShape/ShapeBase.cs`
- `src/Vixen.Common/NShape/Store.cs`
- `src/Vixen.Common/NShape/Styles.cs`
- `src/Vixen.Common/NShape/Template.cs`
- `src/Vixen.Common/NShape/TemplateController.cs`
- `src/Vixen.Common/NShape/Tool.cs`
- `src/Vixen.Common/NShape/TypeDescriptionProviders.cs`
- `src/Vixen.Common/NShape/XmlStore.cs`
- `src/Vixen.Common/NShapeGeneralShapes/LinearShapes.cs`
- `src/Vixen.Common/NShapeWinFormsUI/Display.cs`
- `src/Vixen.Common/NShapeWinFormsUI/ModelTreeViewPresenter.cs`
- `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs`
- `src/Vixen.Modules/App/Curves/ZedGraph/StockPt.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/NShape/AdoNetStore.cs` | PrepareInnerObjectsReading, diagram, cache, cache, CreateCommand, ... |
| `src/Vixen.Common/NShape/Buffers.cs` | False, ToString, Unknown, logicalValue, LogicalValue, ... |
| `src/Vixen.Common/NShape/CachedRepository.cs` | GetProject, shapes, TEntity, NewModels, shape, ... |
| `src/Vixen.Common/NShape/Collections.cs` | MoveNext |
| `src/Vixen.Common/NShape/Command.cs` | propertyInfo, ExchangeTemplateCommand, design, modelObjectBuffer, CopyTemplateFromTemplateCommand, ... |
| `src/Vixen.Common/NShape/Core.cs` | version, shapeType, LibraryData, GetPropertyDefinitions, LastSaved, ... |
| `src/Vixen.Common/NShape/Design.cs` | requiredStyle, AssertValidStyle, previewAsGrayScale, styleToAdd, style, ... |
| `src/Vixen.Common/NShape/DesignController.cs` | CreateDesign, Designs, newValue, propertyName, style, ... |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | SelectModelObjects, modelObjects, modelObjects, ModelObjectsEventArgs, modelObjects, ... |
| `src/Vixen.Common/NShape/Entity.cs` | Category, EntityCategory, WriteTemplate, ModelMapping, Diagram, ... |
| `src/Vixen.Common/NShape/Exceptions.cs` | NShapeException.<init>, NShapeException.<init>, format, NShapeException, innerException, ... |
| `src/Vixen.Common/NShape/FlowLayouter.cs` | Prepare |
| `src/Vixen.Common/NShape/FreeHandTool.cs` | matchingTemplates |
| `src/Vixen.Common/NShape/Geometry.cs` | Conditional |
| `src/Vixen.Common/NShape/Model.cs` | id, ownTerminalId, Type, Model.<init>, ownTerminalId, ... |
| `src/Vixen.Common/NShape/ModelController.cs` | EnsureVisibility, parent, ensureVisibility, ModelObjectSelectedEventArgs, ModelObjectSelectedEventArgs.<init>, ... |
| `src/Vixen.Common/NShape/Project.cs` | RegisterBaseLibraryTypes, modelObjectType, libraryName, ApplyDesign, create, ... |
| `src/Vixen.Common/NShape/PropertyController.cs` | objects, SetObjects |
| `src/Vixen.Common/NShape/PropertyMappings.cs` | IModelMapping, GetPropertyDefinitions, GetInteger, ShapePropertyId, Type, ... |
| `src/Vixen.Common/NShape/Repository.cs` | Count, templates, modelObject, Undelete, style, ... |
| `src/Vixen.Common/NShape/Shape.cs` | ToDateTime, provider |
| `src/Vixen.Common/NShape/ShapeBase.cs` | SecurityDomainName |
| `src/Vixen.Common/NShape/Store.cs` | NewStyles, id, LoadedDiagrams, NewModelMappings, id, ... |
| `src/Vixen.Common/NShape/Styles.cs` | ToString, Name, Title, IStyle |
| `src/Vixen.Common/NShape/Template.cs` | GetPropertyMapping, propertyMapping, UnmapAllTerminals, ToString, MapProperties, ... |
| `src/Vixen.Common/NShape/TemplateController.cs` | TemplateControllerTemplateEventArgs, newModelObject, TemplateControllerModelObjectReplacedEventArgs.<init>, template, oldModelObject, ... |
| `src/Vixen.Common/NShape/Tool.cs` | PlanarShapeCreationTool.<init>, category, template, template, RefreshIcons, ... |
| `src/Vixen.Common/NShape/TypeDescriptionProviders.cs` | component, GetValue |
| `src/Vixen.Common/NShape/XmlStore.cs` | modelmappingsTag, cache, projectId, WriteDiagrams, writer, ... |
| `src/Vixen.Common/NShapeGeneralShapes/LinearShapes.cs` | template, RectangularLine.<init>, CircularArc.<init>, StartCapStyle, persistentTypeName, ... |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | modelBuffer |
| `src/Vixen.Common/NShapeWinFormsUI/ModelTreeViewPresenter.cs` | ModelObject, Current, nodesCollection, ModelObjectDragInfo, modelObjectBuffer, ... |
| `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs` | modelObjectComboBox_SelectedIndexChanged, e, formatMapping, shapePropertyInfo, IsIntegerType, ... |
| `src/Vixen.Modules/App/Curves/ZedGraph/StockPt.cs` | ToString, isShowAll, ToString, isShowAll, format |

## Entry Points

- `src/Vixen.Common/NShape/AdoNetStore.cs::AdoNetStore.SaveChanges`

## Connected Communities

- **Vixen.Common/NShape +69 dirs** (28 cross-edges)
- **Vixen.Common/NShape · ReadStyles** (17 cross-edges)
- **Vixen.Common/NShape +8 dirs** (15 cross-edges)
- **Vixen.Common/NShape +3 dirs · CreatePreviewStyle** (10 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (9 cross-edges)
- **Vixen.Common/NShape +4 dirs** (5 cross-edges)
- **Vixen.Common/NShape · Revert** (5 cross-edges)
- **Vixen.Common/NShape · Append** (4 cross-edges)
- **Vixen.Common/NShapeWinFormsUI +5 dirs** (3 cross-edges)
- **Vixen.Common/NShape · ShapeConnection** (2 cross-edges)
- **Vixen.Common/NShape · DoDeleteShapeConnection** (2 cross-edges)
- **Vixen.Common/NShape · Buffers** (1 cross-edges)
- **Vixen.Common/NShape · RegisterCursorResource** (1 cross-edges)
- **Vixen.Common/NShape +2 dirs · Permission** (1 cross-edges)
- **Vixen.Common/NShape · DoWriteValue** (1 cross-edges)
- **Vixen.Common/NShape · XmlStoreWriter** (1 cross-edges)
- **Vixen.Common/NShape · RepositoryWriter** (1 cross-edges)
- **Vixen.Common/NShape · FlowDirection** (1 cross-edges)
- **Vixen.Common · FlowLayouter** (1 cross-edges)
- **Vixen.Common/NShape · EntityPropertyDefinition** (1 cross-edges)
- **Vixen.Common/NShape · GetStyleEventArgs** (1 cross-edges)
- **Vixen.Common · Contains** (1 cross-edges)
- **Vixen.Common/NShape · Reset** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-175")
explore(operation:"context", task:"understand Vixen.Common/NShape +3 dirs · CachedRepository", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Common/NShape/AdoNetStore.cs::AdoNetStore.SaveChanges"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

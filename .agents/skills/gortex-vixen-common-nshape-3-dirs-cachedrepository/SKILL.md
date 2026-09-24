---
name: gortex-vixen-common-nshape-3-dirs-cachedrepository
description: "Work in the Vixen.Common/NShape +3 dirs · CachedRepository area — 2319 symbols across 34 files (85% cohesion)"
---

# Vixen.Common/NShape +3 dirs · CachedRepository

2319 symbols | 34 files | 85% cohesion

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
| `src/Vixen.Common/NShape/AdoNetStore.cs` | DoReadInt32, cache, cache, modelObjectId, dbCommand, ... |
| `src/Vixen.Common/NShape/Buffers.cs` | False, logicalValue, ToString, LogicalValue, True, ... |
| `src/Vixen.Common/NShape/CachedRepository.cs` | template, Update, loadedEntities, DoUpdateModelObjectOwner, diagram, ... |
| `src/Vixen.Common/NShape/Collections.cs` | MoveNext |
| `src/Vixen.Common/NShape/Command.cs` | DeleteModelObjectsCommand.<init>, ModelObjects, repository, modelObject, CreateModelObjectsCommand.<init>, ... |
| `src/Vixen.Common/NShape/Core.cs` | IRegistrar, shapeType, modelObjectType, instances, GetRepositoryVersion, ... |
| `src/Vixen.Common/NShape/Design.cs` | EntityTypeName, Design, Title, CapStyles, requiredStyle, ... |
| `src/Vixen.Common/NShape/DesignController.cs` | newValue, ReplaceStyle, Designs, newName, propertyName, ... |
| `src/Vixen.Common/NShape/DiagramSetController.cs` | modelObjects, SelectModelObjects, modelObjectEventArgs, ModelObjectsEventArgs.<init>, ModelObjects, ... |
| `src/Vixen.Common/NShape/Entity.cs` | WriteTemplate, ModelObject, IEntityType, version, Delete, ... |
| `src/Vixen.Common/NShape/Exceptions.cs` | message, NShapeException.<init>, NShapeException.<init>, message, NShapeException.<init>, ... |
| `src/Vixen.Common/NShape/FlowLayouter.cs` | Prepare |
| `src/Vixen.Common/NShape/FreeHandTool.cs` | matchingTemplates |
| `src/Vixen.Common/NShape/Geometry.cs` | Conditional |
| `src/Vixen.Common/NShape/Model.cs` | Type, version, id, Parent, AttachShape, ... |
| `src/Vixen.Common/NShape/ModelController.cs` | selectedModelObjects, parent, CreateFindShapesAction, modelObjects, modelObjects, ... |
| `src/Vixen.Common/NShape/Project.cs` | designName, libraryName, ApplyDesign, FindLibraryVersion, shapeType, ... |
| `src/Vixen.Common/NShape/PropertyController.cs` | objects, SetObjects |
| `src/Vixen.Common/NShape/PropertyMappings.cs` | slope, value, this[], GetFloat, CanGetStyle, ... |
| `src/Vixen.Common/NShape/Repository.cs` | GluePointId, design, RepositoryStyleEventArgs, modelMappings, Undelete, ... |
| `src/Vixen.Common/NShape/Shape.cs` | provider, ToDateTime |
| `src/Vixen.Common/NShape/ShapeBase.cs` | ModelObject, SecurityDomainName |
| `src/Vixen.Common/NShape/Store.cs` | NewShapes, SetProjectOwnerId, id, LoadedDiagrams, LoadedProjects, ... |
| `src/Vixen.Common/NShape/Styles.cs` | IStyle, Name, ToString, Title |
| `src/Vixen.Common/NShape/Template.cs` | propertyMapping, UnmapAllProperties, Id, Shape, Description, ... |
| `src/Vixen.Common/NShape/TemplateController.cs` | TemplateControllerTemplateEventArgs.<init>, newModelObject, TemplateControllerModelObjectReplacedEventArgs.<init>, TemplateControllerPropertyMappingChangedEventArgs, template, ... |
| `src/Vixen.Common/NShape/Tool.cs` | template, category, Construct, PlanarShapeCreationTool.<init>, template, ... |
| `src/Vixen.Common/NShape/TypeDescriptionProviders.cs` | GetValue, component |
| `src/Vixen.Common/NShape/XmlStore.cs` | DoReadDate, CloseFile, overwrite, storeCache, reader, ... |
| `src/Vixen.Common/NShapeGeneralShapes/LinearShapes.cs` | shapeType, persistentTypeName, Polyline.<init>, Polyline.<init>, RectangularLine.<init>, ... |
| `src/Vixen.Common/NShapeWinFormsUI/Display.cs` | modelBuffer |
| `src/Vixen.Common/NShapeWinFormsUI/ModelTreeViewPresenter.cs` | Current, ModelObjectDragInfo.<init>, e, ModelObjectDragInfo, modelObjectBuffer, ... |
| `src/Vixen.Common/NShapeWinFormsUI/TemplatePresenter.cs` | IsIntegerType, IsStringType, e, numericMapping, styleMapping, ... |
| `src/Vixen.Modules/App/Curves/ZedGraph/StockPt.cs` | format, isShowAll, isShowAll, ToString, ToString |

## Entry Points

- `src/Vixen.Common/NShape/AdoNetStore.cs::AdoNetStore.SaveChanges`

## Connected Communities

- **Vixen.Common/NShape +64 dirs** (29 cross-edges)
- **Vixen.Common/NShape · ReadStyles** (17 cross-edges)
- **Vixen.Common/NShape +8 dirs** (15 cross-edges)
- **Vixen.Common/NShape +3 dirs · CreatePreviewStyle** (10 cross-edges)
- **Vixen.Common/NShape · NotifyColorStyleChanged** (9 cross-edges)
- **Vixen.Common/NShape +5 dirs** (6 cross-edges)
- **Vixen.Common/NShape · Revert** (5 cross-edges)
- **Vixen.Common/NShape · Append** (4 cross-edges)
- **Vixen.Common/NShapeWinFormsUI +4 dirs** (3 cross-edges)
- **Vixen.Common/NShape +2 dirs · Permission** (1 cross-edges)
- **Vixen.Common/NShape · DoWriteValue** (1 cross-edges)
- **Vixen.Common/NShape · RepositoryWriter** (1 cross-edges)
- **Vixen.Common/NShape · FlowDirection** (1 cross-edges)
- **Vixen.Common · Contains** (1 cross-edges)
- **Vixen.Common/NShape · Reset** (1 cross-edges)
- **Vixen.Common · FlowLayouter** (1 cross-edges)
- **Vixen.Common/NShape · RegisterCursorResource** (1 cross-edges)
- **Vixen.Common/NShape · Buffers** (1 cross-edges)
- **Vixen.Common/NShape · XmlStoreWriter** (1 cross-edges)
- **Vixen.Common/NShape · EntityPropertyDefinition** (1 cross-edges)
- **Vixen.Common/NShape · GetStyleEventArgs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-163")
explore(operation:"context", task:"understand Vixen.Common/NShape +3 dirs · CachedRepository", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Common/NShape/AdoNetStore.cs::AdoNetStore.SaveChanges"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

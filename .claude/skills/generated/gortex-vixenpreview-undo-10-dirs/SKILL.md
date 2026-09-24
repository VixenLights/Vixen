---
name: gortex-vixenpreview-undo-10-dirs
description: "Work in the VixenPreview/Undo +10 dirs area — 861 symbols across 49 files (85% cohesion)"
---

# VixenPreview/Undo +10 dirs

861 symbols | 49 files | 85% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`
- `src/Vixen.Common/Controls/Undo/AddToListUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/ModifyItemUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/RemoveFromListUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/UndoAction.cs`
- `src/Vixen.Common/Controls/Undo/UndoManager.cs`
- `src/Vixen.Core/Execution/Context/ContextBase.cs`
- `src/Vixen.Core/Execution/Context/LiveContext.cs`
- `src/Vixen.Core/Execution/DataSource/LiveDataSource.cs`
- `src/Vixen.Core/Services/ApplicationServices.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsLayerChangedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsModifiedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/IDisplayForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeChangeUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeInfo.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPositionInfo.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemResizeMoveInfo.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedRemovedUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsCutUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedSeparateUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupSeparateAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsLockUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsMoveUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsPasteUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsRemovedUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsResizeUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewData.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewDescriptor.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | ClearMarkSnapPoints |
| `src/Vixen.Common/Controls/Undo/AddToListUndoAction.cs` | AddToListUndoAction, List, Redo, Object, AddToListUndoAction.<init>, ... |
| `src/Vixen.Common/Controls/Undo/ModifyItemUndoAction.cs` | obj, Property, Undo, ModifyItemUndoAction, Redo, ... |
| `src/Vixen.Common/Controls/Undo/RemoveFromListUndoAction.cs` | RemoveFromListUndoAction, obj, Redo, List, Object, ... |
| `src/Vixen.Common/Controls/Undo/UndoAction.cs` | State, ReadyForUndo, UndoAction, m_state, Redo, ... |
| `src/Vixen.Common/Controls/Undo/UndoManager.cs` | NumUndoable, m_undoable, MaxItems, UndoManager.<init>, m_redoable, ... |
| `src/Vixen.Core/Execution/Context/ContextBase.cs` | Dispose |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | Clear, waitForReset |
| `src/Vixen.Core/Execution/DataSource/LiveDataSource.cs` | ClearData |
| `src/Vixen.Core/Services/ApplicationServices.cs` | moduleTypeId, id, GetElementTemplate, GetModuleDescriptor, name, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs` | GetEffectData |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | AddEffectsModifiedToUndo, modifiedElements |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsLayerChangedUndoAction.cs` | Count, Undo, EffectsLayerChangedUndoAction, Redo, Description, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsModifiedUndoAction.cs` | changedElements, _changedElements, EffectsModifiedUndoAction, Logging, Redo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs` | Redo, Undo |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs` | Redo, Undo |
| `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIControl.cs` | DisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs` | DisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/IDisplayForm.cs` | InstanceId, Close, IDisplayForm, Setup, Data, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | Draw, obj, DisplayItem.<init>, Equals, highlightedElements, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs` | y, Nudge, y, MoveTo, x, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs` | HasInValidElementMapping |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs` | SerializeToString, ResizeBitmap, st, st, type, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeChangeUndoAction.cs` | changedPreviewItems, Redo, form, _previewForm, PreviewItemPixelSizeChangeUndoAction.<init>, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeInfo.cs` | PreviewItemPixelSizeInfo, PreviewItemPixelSizeInfo.<init>, changeAmount, ChangeAmount |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPositionInfo.cs` | PreviewItemPositionInfo.<init>, TopPosition, LeftPosition, previewItem, PreviewItemPositionInfo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemResizeMoveInfo.cs` | PreviewItemResizeMoveInfo, modifyingElements, OriginalPreviewItem, PreviewItemResizeMoveInfo.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedRemovedUndoAction.cs` | addEffects, items, m_count, PreviewItemsAddedRemovedUndoAction, m_elements, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedUndoAction.cs` | form, Undo, PreviewItemsAddedUndoAction.<init>, PreviewItemsAddedUndoAction, Description, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsCutUndoAction.cs` | Description, form, items, Undo, PreviewItemsCutUndoAction, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedSeparateUndoAction.cs` | PreviewItemsGroupAddedSeparateUndoAction, m_form, newDisplayItem, m_newDisplay, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedUndoAction.cs` | Redo, Undo, PreviewItemsGroupAddedUndoAction.<init>, PreviewItemsGroupAddedUndoAction, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupSeparateAction.cs` | PreviewItemsGroupSeparateAction.<init>, Description, form, newDisplayItem, Redo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsLockUndoAction.cs` | m_form, Description, PreviewItemsLockUndoAction, Undo, Redo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsMoveUndoAction.cs` | form, m_form, Description, Undo, m_changedPreviewItems, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsPasteUndoAction.cs` | PreviewItemsPasteUndoAction, Undo, Redo, Description, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsRemovedUndoAction.cs` | form, Redo, Undo, items, Description, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsResizeUndoAction.cs` | PreviewItemsResizeUndoAction.<init>, m_changedPreviewItems, m_form, form, ChangedPreviewItems, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.Designer.cs` | InitializeComponent, VixenPreviewControl, disposing, contextMenuStrip1, components, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` | DecreaseBulbSize, point, EraseScreen, sender, CreateTemplate, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewData.cs` | DisplayItems, UseOpenGL, SaveLocations, LocationOffset, Left, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewDescriptor.cs` | _typeId, Version, ModulePath, ModuleDataClass, Author, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.Designer.cs` | VixenPreviewModuleInstance |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.cs` | _openGLSupportChecked, Initialize, Dispose, _systemSupportsOpenGl, Update, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.Designer.cs` | panel2, lblBulbSize, buttonAlignBottom, buttonDistributeHorizontal, pnlBulbSize, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` | sender, e, e, e, e, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs` | ClearSelectedNodes, UpdateSelectedDisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.Designer.cs` | Dispose, disposing, VixenPreviewSetupPropertiesDocument, components, InitializeComponent |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs` | SetupPreviewShape, _previewControl, setupControl, SetupControlPropertyEdited, e, ... |

## Entry Points

- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs::VixenPreviewControl.VixenPreviewControl_MouseDown`

## Connected Communities

- **VixenPreview/Shapes +22 dirs** (23 cross-edges)
- **Effect/Effect +72 dirs** (5 cross-edges)
- **Editor/TimedSequenceEditor +46 dirs** (3 cross-edges)
- **Vixen.Core/Sys +79 dirs** (2 cross-edges)
- **Vixen.Core/Sys +15 dirs** (2 cross-edges)
- **Controls/TimeLineControl +4 dirs · ToArray** (2 cross-edges)
- **CustomPropEditor/BackgroundImageScaling +14 dirs** (2 cross-edges)
- **Editor/TimedSequenceEditor +4 dirs** (2 cross-edges)
- **Sys/Output +28 dirs** (1 cross-edges)
- **App/LipSyncApp +3 dirs · LipSyncMapMatrixEditor** (1 cross-edges)
- **Preview/VixenPreview · TemplateComboBoxItem** (1 cross-edges)
- **Xml/Serializer +14 dirs** (1 cross-edges)
- **Controls/TimeLineControl +4 dirs · Grid** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1299")
explore(operation:"context", task:"understand VixenPreview/Undo +10 dirs", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs::VixenPreviewControl.VixenPreviewControl_MouseDown"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

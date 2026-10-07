---
name: gortex-vixenpreview-undo-11-dirs
description: "Work in the VixenPreview/Undo +11 dirs area — 861 symbols across 49 files (87% cohesion)"
---

# VixenPreview/Undo +11 dirs

861 symbols | 49 files | 87% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/Controls/TimeLineControl/BlockingCollectionExtensions.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarkTimeInfo.cs`
- `src/Vixen.Common/Controls/Undo/AddToListUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/ModifyItemUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/RemoveFromListUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/UndoAction.cs`
- `src/Vixen.Common/Controls/Undo/UndoManager.cs`
- `src/Vixen.Core/Execution/Context/LiveContext.cs`
- `src/Vixen.Core/Execution/DataSource/LiveDataSource.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsLayerChangedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsModifiedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksBulkChangeUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/IDisplayForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs`
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
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/Controls/TimeLineControl/BlockingCollectionExtensions.cs` | item, TryAdd |
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | ClearMarkSnapPoints |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarkTimeInfo.cs` | SwapPlaces, rhs, lhs |
| `src/Vixen.Common/Controls/Undo/AddToListUndoAction.cs` | Undo, list, obj, AddToListUndoAction, AddToListUndoAction.<init>, ... |
| `src/Vixen.Common/Controls/Undo/ModifyItemUndoAction.cs` | Undo, ModifyItemUndoAction.<init>, ModifyItemUndoAction, Property, obj, ... |
| `src/Vixen.Common/Controls/Undo/RemoveFromListUndoAction.cs` | list, Redo, RemoveFromListUndoAction.<init>, List, Undo, ... |
| `src/Vixen.Common/Controls/Undo/UndoAction.cs` | m_state, Redo, UndoState, ReadyForUndo, ReadyForRedo, ... |
| `src/Vixen.Common/Controls/Undo/UndoManager.cs` | DefaultMaxItems, _pushOntoUndoStack, OnRedoItemsChanged, action, UndoManager.<init>, ... |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | Clear, waitForReset |
| `src/Vixen.Core/Execution/DataSource/LiveDataSource.cs` | ClearData |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | AddEffectsModifiedToUndo, modifiedElements |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsLayerChangedUndoAction.cs` | Count, _form, form, EffectsLayerChangedUndoAction.<init>, nodes, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsModifiedUndoAction.cs` | labelName, Description, Redo, changedElements, Logging, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs` | Undo, Redo |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksBulkChangeUndoAction.cs` | MarksBulkChangeUndoAction, _marksBulkChangeInfo, Description, form, MarksBulkChangeUndoAction.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs` | Undo, Redo |
| `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIControl.cs` | DisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs` | DisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/IDisplayForm.cs` | UpdatePreview, Close, Setup, InstanceId, IsOnTopWhenPlaying, ... |
| `src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs` | Setup |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | Handle, GetEnumerator, selected, Draw, obj, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs` | y, Nudge, x, y, MoveTo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs` | UpdatePixelCache, HasInValidElementMapping |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs` | DeSerializeToDisplayItemList, obj, type, ResizeBitmap, st, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeChangeUndoAction.cs` | _itemPixelSizeInfo, form, _previewForm, PreviewItemPixelSizeChangeUndoAction.<init>, _changedPreviewItems, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeInfo.cs` | changeAmount, ChangeAmount, PreviewItemPixelSizeInfo.<init>, PreviewItemPixelSizeInfo |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPositionInfo.cs` | PreviewItemPositionInfo.<init>, OriginalPreviewItem, previewItem, LeftPosition, TopPosition, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemResizeMoveInfo.cs` | OriginalPreviewItem, PreviewItemResizeMoveInfo, modifyingElements, PreviewItemResizeMoveInfo.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedRemovedUndoAction.cs` | Count, m_elements, PreviewItemsAddedRemovedUndoAction.<init>, addEffects, m_form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedUndoAction.cs` | PreviewItemsAddedUndoAction, Redo, Undo, PreviewItemsAddedUndoAction.<init>, Description, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsCutUndoAction.cs` | PreviewItemsCutUndoAction, PreviewItemsCutUndoAction.<init>, Undo, items, Description, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedSeparateUndoAction.cs` | newDisplayItem, removeEffects, m_newDisplay, PreviewItemsGroupAddedSeparateUndoAction.<init>, addGroupEffects, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedUndoAction.cs` | form, Description, PreviewItemsGroupAddedUndoAction, newDisplayItem, Undo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupSeparateAction.cs` | newDisplayItem, Undo, Redo, PreviewItemsGroupSeparateAction, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsLockUndoAction.cs` | Description, m_changedPreviewItems, PreviewItemsLockUndoAction, Redo, Undo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsMoveUndoAction.cs` | Redo, PreviewItemsMoveUndoAction.<init>, m_changedPreviewItems, PreviewItemsMoveUndoAction, ChangedPreviewItems, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsPasteUndoAction.cs` | PreviewItemsPasteUndoAction.<init>, Description, items, Redo, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsRemovedUndoAction.cs` | PreviewItemsRemovedUndoAction.<init>, Undo, Description, Redo, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsResizeUndoAction.cs` | PreviewItemsResizeUndoAction, m_form, Redo, Undo, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.Designer.cs` | InitializeComponent, Dispose, components, contextMenuStrip1, VixenPreviewControl, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` | hScroll, width, sender, PixelResize, p, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewData.cs` | SetupHeight, SetupTop, Width, BackgroundAlpha, SetupWidth, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewDescriptor.cs` | TypeId, Description, _typeId, ModulePath, VixenPreviewDescriptor, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.Designer.cs` | VixenPreviewModuleInstance |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.cs` | Name, GetDataModel, _formLock, _setupForm, _openGLSupportChecked, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.Designer.cs` | toolStripSeparator2, label14, panel11, label11, buttonSetBackground, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` | OnSelectDisplayItem, sender, e, e, backgroundPropertiesToolStripMenuItem_Click, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.Designer.cs` | InitializeComponent, components, Dispose, disposing, VixenPreviewSetupPropertiesDocument |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs` | _setupControl, e, ShowSetupControl, ClearSetupControl, _previewControl, ... |

## Entry Points

- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs::VixenPreviewControl.VixenPreviewControl_MouseDown`

## Connected Communities

- **VixenPreview/Shapes +22 dirs** (18 cross-edges)
- **Editor/TimedSequenceEditor +44 dirs** (7 cross-edges)
- **Vixen.Common/NShape +70 dirs** (3 cross-edges)
- **Effect/Effect +104 dirs** (3 cross-edges)
- **Controls/TimeLineControl +7 dirs** (3 cross-edges)
- **Vixen.Core/Sys +14 dirs** (3 cross-edges)
- **Vixen.Core/Sys +77 dirs** (3 cross-edges)
- **Vixen.Application/Setup +25 dirs** (2 cross-edges)
- **App/LipSyncApp +1 dirs** (1 cross-edges)
- **Preview/VixenPreview · TemplateComboBoxItem** (1 cross-edges)
- **Vixen.Common/Controls +2 dirs** (1 cross-edges)
- **Vixen.Core/Sys +6 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1296")
explore(operation:"context", task:"understand VixenPreview/Undo +11 dirs", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs::VixenPreviewControl.VixenPreviewControl_MouseDown"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

---
name: gortex-vixenpreview-undo-10-dirs
description: "Work in the VixenPreview/Undo +10 dirs area — 594 symbols across 44 files (84% cohesion)"
---

# VixenPreview/Undo +10 dirs

594 symbols | 44 files | 84% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`
- `src/Vixen.Common/Controls/Undo/AddToListUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/ModifyItemUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/RemoveFromListUndoAction.cs`
- `src/Vixen.Common/Controls/Undo/UndoAction.cs`
- `src/Vixen.Common/Controls/Undo/UndoManager.cs`
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
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | ClearMarkSnapPoints |
| `src/Vixen.Common/Controls/Undo/AddToListUndoAction.cs` | AddToListUndoAction.<init>, obj, List, AddToListUndoAction, Undo, ... |
| `src/Vixen.Common/Controls/Undo/ModifyItemUndoAction.cs` | Object, OldValue, Property, Undo, property, ... |
| `src/Vixen.Common/Controls/Undo/RemoveFromListUndoAction.cs` | list, obj, List, Object, RemoveFromListUndoAction, ... |
| `src/Vixen.Common/Controls/Undo/UndoAction.cs` | UndoAction, m_state, Undo, ReadyForUndo, Description, ... |
| `src/Vixen.Common/Controls/Undo/UndoManager.cs` | OnUndoItemsChanged, _popFromUndoStack, DefaultMaxItems, n, _pushOntoUndoStack, ... |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | waitForReset, Clear |
| `src/Vixen.Core/Execution/DataSource/LiveDataSource.cs` | ClearData |
| `src/Vixen.Core/Services/ApplicationServices.cs` | filePath, GetActiveEditor, moduleTypeId, GetModuleDescriptor, ApplicationServices, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs` | GetEffectData |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | effectNodes, modifiedElements, AddEffectsModifiedToUndo, SwapLayers |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsLayerChangedUndoAction.cs` | Undo, Redo |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsModifiedUndoAction.cs` | changedElements, Redo, _count, labelName, Count, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs` | Undo, Redo |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs` | _form, MarksTimeChangedUndoAction, Undo, Redo, _moveType, ... |
| `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIControl.cs` | DisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs` | DisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/IDisplayForm.cs` | Close |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | obj, DisplayItem.<init>, _zoomLevel, GetEnumerator, GetEnumerator, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs` | Nudge, x, y, MoveTo, y, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs` | size, DeSerializeToDisplayItemList, DeSerializeToDisplayItem, type, ResizeBitmap, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeChangeUndoAction.cs` | _itemPixelSizeInfo, changedPreviewItems, info, Redo, Description, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPixelSizeInfo.cs` | PreviewItemPixelSizeInfo.<init>, changeAmount, PreviewItemPixelSizeInfo, ChangeAmount |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemPositionInfo.cs` | TopPosition, LeftPosition, PreviewItemPositionInfo, previewItem, PreviewItemPositionInfo.<init>, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemResizeMoveInfo.cs` | PreviewItemResizeMoveInfo.<init>, OriginalPreviewItem, modifyingElements, PreviewItemResizeMoveInfo |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedRemovedUndoAction.cs` | items, m_elements, m_form, form, PreviewItemsAddedRemovedUndoAction, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsAddedUndoAction.cs` | PreviewItemsAddedUndoAction.<init>, items, Description, Undo, PreviewItemsAddedUndoAction, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsCutUndoAction.cs` | PreviewItemsCutUndoAction.<init>, items, form, Description, PreviewItemsCutUndoAction, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedSeparateUndoAction.cs` | form, PreviewItemsGroupAddedSeparateUndoAction, addGroupEffects, PreviewItemsGroupAddedSeparateUndoAction.<init>, newDisplayItem, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupAddedUndoAction.cs` | PreviewItemsGroupAddedUndoAction, Undo, Redo, PreviewItemsGroupAddedUndoAction.<init>, Description, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsGroupSeparateAction.cs` | form, Description, PreviewItemsGroupSeparateAction.<init>, PreviewItemsGroupSeparateAction, Undo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsLockUndoAction.cs` | Description, Redo, ChangedPreviewItems, ChangedPreviewItems, m_changedPreviewItems, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsMoveUndoAction.cs` | Description, form, m_form, ChangedPreviewItems, Redo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsPasteUndoAction.cs` | Undo, PreviewItemsPasteUndoAction, PreviewItemsPasteUndoAction.<init>, Description, form, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsRemovedUndoAction.cs` | Redo, Undo, PreviewItemsRemovedUndoAction.<init>, Description, PreviewItemsRemovedUndoAction, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Undo/PreviewItemsResizeUndoAction.cs` | Redo, Description, Undo, PreviewItemsResizeUndoAction, PreviewItemsResizeUndoAction.<init>, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.Designer.cs` | VixenPreviewControl, disposing, InitializeComponent, Dispose, contextMenuStrip1, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` | Resize_MoveSwapPlaces, ZoomLevel, bufferedGraphics, point, ItemBulbSize, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewData.cs` | _displayItems, DisplayItems |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewDescriptor.cs` | VixenPreviewDescriptor.<init>, VixenPreviewDescriptor, TypeName, Description, Author, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.cs` | Dispose, disposing |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` | backgroundPropertiesToolStripMenuItem_Click, e, sender, e, e, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.Designer.cs` | disposing, VixenPreviewSetupPropertiesDocument, Dispose, InitializeComponent, components |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs` | ClearSetupControl, _previewControl, e, SetupControlPropertyEdited, ShowSetupControl, ... |

## Entry Points

- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs::VixenPreviewControl.VixenPreviewControl_MouseDown`

## Connected Communities

- **VixenPreview/Shapes +20 dirs** (23 cross-edges)
- **Vixen.Core/Sys +14 dirs** (3 cross-edges)
- **Vixen.Core/Sys +64 dirs** (3 cross-edges)
- **App/ColorGradients +92 dirs** (2 cross-edges)
- **TimedSequenceEditor/Forms +37 dirs** (2 cross-edges)
- **Preview/VixenPreview · VixenPreviewSetup3** (2 cross-edges)
- **Vixen.Core/Sys +5 dirs · ToArray** (2 cross-edges)
- **Controls/TimeLineControl +4 dirs** (1 cross-edges)
- **Controls/TimeLineControl +2 dirs · Row** (1 cross-edges)
- **Vixen.Common/Controls +2 dirs** (1 cross-edges)
- **Vixen.Common/NShape +69 dirs** (1 cross-edges)
- **Curves/ZedGraph +10 dirs** (1 cross-edges)
- **Preview/VixenPreview · TemplateComboBoxItem** (1 cross-edges)
- **App/LipSyncApp +1 dirs** (1 cross-edges)
- **Editor/TimedSequenceEditor +11 dirs** (1 cross-edges)
- **Xml/Serializer +14 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1352")
explore(operation:"context", task:"understand VixenPreview/Undo +10 dirs", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs::VixenPreviewControl.VixenPreviewControl_MouseDown"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

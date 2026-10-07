---
name: gortex-controls-timelinecontrol-7-dirs
description: "Work in the Controls/TimeLineControl +7 dirs area — 871 symbols across 28 files (85% cohesion)"
---

# Controls/TimeLineControl +7 dirs

871 symbols | 28 files | 85% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/Controls/TimeLineControl/AlignmentEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/BlockingCollectionExtensions.cs`
- `src/Vixen.Common/Controls/TimeLineControl/DragState.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Element.cs`
- `src/Vixen.Common/Controls/TimeLineControl/EventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/ExtensionMethods.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs`
- `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`
- `src/Vixen.Common/Controls/TimeLineControl/ResizeZone.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Row.cs`
- `src/Vixen.Common/Controls/TimeLineControl/RowLabel.cs`
- `src/Vixen.Common/Controls/TimeLineControl/RowList.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControlBase.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`
- `src/Vixen.Core/Export/ESEQWriter.cs`
- `src/Vixen.Core/Module/SingletonRepository.cs`
- `src/Vixen.Core/Sys/Attribute/ValueAttribute.cs`
- `src/Vixen.Core/Sys/DefaultValueArrayMember.cs`
- `src/Vixen.Core/Sys/IntentChangeCollection.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs`
- `src/Vixen.Tests/Sequencer/GridMarkSnapPointTests.cs`
- `src/Vixen.Tests/Sequencer/RowEventScopeTests.cs`
- `src/Vixen.Tests/Sequencer/TimelineCursorSelectionTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/Controls/TimeLineControl/AlignmentEventArgs.cs` | times, AlignmentEventArgs.<init>, AlignmentEventArgs, active, Times, ... |
| `src/Vixen.Common/Controls/TimeLineControl/BlockingCollectionExtensions.cs` | item, collection, ToArray, GetConsumingPartitioner, T, ... |
| `src/Vixen.Common/Controls/TimeLineControl/DragState.cs` | Normal, Drawing, Waiting, HResizing, Moving, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Element.cs` | CompareTo, Duration, EffectNode, MouseCaptured, _effectNode, ... |
| `src/Vixen.Common/Controls/TimeLineControl/EventArgs.cs` | MouseUpTime, time, DrawElementEventArgs, info, ElementRowChangeEventArgs.<init>, ... |
| `src/Vixen.Common/Controls/TimeLineControl/ExtensionMethods.cs` | T, topLeft, RectangleFromPoints, t1, t2, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | includeChildren, SelectElementsBetween, skipDuplicatesUnlessInRow, _timelineGlobalEventManager, _paintExceptionDialogShown, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` | delta, waitForDragMove, MouseMove_DragMoving, effectDrawMouseUpTime, gridLocation, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs` | e, OnAlignmentActivity |
| `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` | Min, val2, BeginHResize, gridLocation, _markResizeZone, ... |
| `src/Vixen.Common/Controls/TimeLineControl/ResizeZone.cs` | None, ResizeZone, Back, Front |
| `src/Vixen.Common/Controls/TimeLineControl/Row.cs` | GetEnumerator, ElementSelectedHandler, m_visibilityFilter, element, AddBulkElements, ... |
| `src/Vixen.Common/Controls/TimeLineControl/RowLabel.cs` | ShowActiveIndicators, e, OnMouseDown, _RowContextMenuSelect, Resizing, ... |
| `src/Vixen.Common/Controls/TimeLineControl/RowList.cs` | RowLabels |
| `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs` | e, OnTimeRangeDragged, MouseState, Button, DraggingMark, ... |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | height, SelectedRow, SelectAllElements, element, element, ... |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControlBase.cs` | OnTimePerPixelChanged, e, sender, PixelsToTime, px |
| `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` | sender, WaveFormSelectedTimeLineGlobalMove, e, e, e, ... |
| `src/Vixen.Core/Export/ESEQWriter.cs` | periodData, WriteNextPeriodData |
| `src/Vixen.Core/Module/SingletonRepository.cs` | GetAll |
| `src/Vixen.Core/Sys/Attribute/ValueAttribute.cs` | ValueAttribute |
| `src/Vixen.Core/Sys/DefaultValueArrayMember.cs` | DefaultValueArrayMember.<init>, owner, Values |
| `src/Vixen.Core/Sys/IntentChangeCollection.cs` | IntentChangeCollection.<init>, addedIntents, removedIntents |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | changedElements, SwapPlaces |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs` | OnTargetNodesChanged |
| `src/Vixen.Tests/Sequencer/GridMarkSnapPointTests.cs` | CreateGrid, instanceId, markCollections |
| `src/Vixen.Tests/Sequencer/RowEventScopeTests.cs` | RowChanged_WhenDifferentRowChanges_DoesNotNotifyOtherRow, RowToggled_WhenDifferentRowToggled_DoesNotNotifyOtherRow, RowVisibilityChanged_WhenDifferentRowVisibilityChanges_DoesNotNotifyOtherRow, RowEventScopeTests |
| `src/Vixen.Tests/Sequencer/TimelineCursorSelectionTests.cs` | grid, selectedElements, lassoOriginRow, FinalizeMoveCursorLassoSelection |

## Connected Communities

- **Editor/TimedSequenceEditor +44 dirs** (23 cross-edges)
- **Controls/TimeLineControl · timeToPixels** (18 cross-edges)
- **LayerEditor/ImportExport +16 dirs** (3 cross-edges)
- **TimeLineControl/LabeledMarks +3 dirs** (3 cross-edges)
- **VixenPreview/Undo +11 dirs** (2 cross-edges)
- **Controls/TimeLineControl · MarksBar** (1 cross-edges)
- **Controls/TimeLineControl · RowList** (1 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (1 cross-edges)
- **Controls/TimeLineControl · _markCollections_CollectionChan… · MarksBar** (1 cross-edges)
- **Vixen.Common/NShape +13 dirs** (1 cross-edges)
- **Vixen.Core/Sys +4 dirs · SetValue** (1 cross-edges)
- **VixenPreview/Shapes +22 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-105")
explore(operation:"context", task:"understand Controls/TimeLineControl +7 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

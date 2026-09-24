---
name: gortex-controls-timelinecontrol-4-dirs-grid
description: "Work in the Controls/TimeLineControl +4 dirs · Grid area — 861 symbols across 24 files (85% cohesion)"
---

# Controls/TimeLineControl +4 dirs · Grid

861 symbols | 24 files | 85% cohesion

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
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksSelectionManager.cs`
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
- `src/Vixen.Modules/Editor/EffectEditor/Controls/ToggleSwitch.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs`
- `src/Vixen.Tests/Sequencer/RowEventScopeTests.cs`
- `src/Vixen.Tests/Sequencer/TimelineCursorSelectionTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/Controls/TimeLineControl/AlignmentEventArgs.cs` | Times, AlignmentEventArgs, AlignmentEventArgs.<init>, active, times, ... |
| `src/Vixen.Common/Controls/TimeLineControl/BlockingCollectionExtensions.cs` | TryTake, item, GetConsumingPartitioner, T, collection |
| `src/Vixen.Common/Controls/TimeLineControl/DragState.cs` | Moving, Waiting, Selecting, HResizing, Normal, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Element.cs` | StartTime, OnTargetNodesChanged, _origDuration, other, StackCount, ... |
| `src/Vixen.Common/Controls/TimeLineControl/EventArgs.cs` | Type, heightChange, MouseDownTime, ElementsSelectedEventArgs.<init>, ModifierKeysEventArgs.<init>, ... |
| `src/Vixen.Common/Controls/TimeLineControl/ExtensionMethods.cs` | bottomRight, t1, RectangleFromPoints, Min, t1, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | RowContainingElement, CalculateVisibleRowDisplayTops, AppendElementDeleteInfo, percent, offset, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` | location, gridLocation, ElementResizeThreshold, gridLocation, AutoScrollPxScaleFactor, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksSelectionManager.cs` | ClearSelected |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs` | OnAlignmentActivity, e |
| `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` | val2, val1, location, _dragState, e, ... |
| `src/Vixen.Common/Controls/TimeLineControl/ResizeZone.cs` | Back, Front, None, ResizeZone |
| `src/Vixen.Common/Controls/TimeLineControl/Row.cs` | searchBy, m_requestedVisible, SelectAllElements, m_elements, sender, ... |
| `src/Vixen.Common/Controls/TimeLineControl/RowLabel.cs` | e, parentRow, OnMouseDown, RowLabel, e, ... |
| `src/Vixen.Common/Controls/TimeLineControl/RowList.cs` | RowLabels |
| `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs` | ResizeRuler, recalculate, OnResize, OnTimePerPixelChanged, time, ... |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | parent, height, parent, height, AddRow, ... |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControlBase.cs` | e, PixelsToTime, sender, OnTimePerPixelChanged, px |
| `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` | OnMouseMove, e, OnMouseDoubleClick, e |
| `src/Vixen.Modules/Editor/EffectEditor/Controls/ToggleSwitch.cs` | UpdateToggleSwitchContents, oldIsCheckedLeft, isCheckedLeft, newIsCheckedLeft, OnIsCheckedLeftChanged |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | changedElements, e, SwapPlaces, ElementRemovedFromRowHandler, e, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs` | OnTargetNodesChanged |
| `src/Vixen.Tests/Sequencer/RowEventScopeTests.cs` | RowToggled_WhenDifferentRowToggled_DoesNotNotifyOtherRow, RowChanged_WhenDifferentRowChanges_DoesNotNotifyOtherRow, RowVisibilityChanged_WhenDifferentRowVisibilityChanges_DoesNotNotifyOtherRow, RowEventScopeTests |
| `src/Vixen.Tests/Sequencer/TimelineCursorSelectionTests.cs` | FinalizeMoveCursorLassoSelection, grid, lassoOriginRow, selectedElements |

## Connected Communities

- **Editor/TimedSequenceEditor +46 dirs** (25 cross-edges)
- **Controls/TimeLineControl · timeToPixels** (18 cross-edges)
- **TimeLineControl/LabeledMarks +3 dirs** (3 cross-edges)
- **VixenPreview/Undo +10 dirs** (2 cross-edges)
- **TimeLineControl/LabeledMarks · MarksSelectionManager** (2 cross-edges)
- **Controls/TimeLineControl +4 dirs · ToArray** (2 cross-edges)
- **Controls/TimeLineControl · MarksBar** (1 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (1 cross-edges)
- **VixenPreview/Shapes +22 dirs** (1 cross-edges)
- **Controls/TimeLineControl · _markCollections_CollectionChan…** (1 cross-edges)
- **Controls/TimeLineControl · BeginHResize** (1 cross-edges)
- **Controls/TimeLineControl · RowList** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-104")
explore(operation:"context", task:"understand Controls/TimeLineControl +4 dirs · Grid", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._

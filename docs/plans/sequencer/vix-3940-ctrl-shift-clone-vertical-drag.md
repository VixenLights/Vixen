# Ctrl+Shift Clone Vertical Drag Specification (VIX-3940)

## Overview

This specification covers a Timed Sequence Editor timeline improvement for combining two existing effect drag behaviors:

- `Ctrl + Drag` clones the selected effect or effects, then drags the clones to a new timeline location.
- `Shift + Drag` drags the selected effect or effects vertically while preserving their horizontal time position.

The requested improvement is `Ctrl + Shift + Drag`: clone the selected effect or effects, then drag the cloned effects vertically while preserving their original horizontal time position.

JIRA issue: `VIX-3940`

## Repository Context

The timeline grid drag behavior is implemented in `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` and supporting grid behavior is in `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`.

The Timed Sequence Editor host wires timeline cloning through `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`. During editor setup, `TimelineControl.grid.SelectedElementsCloneDelegate` is assigned to `TimedSequenceEditorForm.CloneElements`. That clone method creates copied effect module data, inserts new effect nodes into the sequence, assigns the same layer as the source effect, selects the newly added timeline elements, calls `SequenceModified()`, and adds an `EffectsAddedUndoAction`.

Timeline move undo is handled separately through `TimelineControl.ElementsMovedNew`, which the editor converts into an `ElementsTimeChangedUndoAction`.

## Existing Behavior

### Ctrl + Drag Clone

When the mouse is pressed on an effect and the pointer moves outside the drag threshold, `Grid_Mouse.HandleMouseMove` transitions from `DragState.Waiting` to `DragState.Moving`.

In the `Waiting` branch, the code checks `CtrlPressed`. Because `CtrlPressed` uses `ModifierKeys.HasFlag(Keys.Control)`, both `Ctrl + Drag` and `Ctrl + Shift + Drag` satisfy the clone trigger today.

Before cloning, the grid ensures the effect under the original pointer location is selected. It then calls `CloneSelectedElementsForMove()`, which invokes the editor's `SelectedElementsCloneDelegate`, clears the original selection, marks the returned cloned elements as selected, and raises selection changed. The drag then begins against the cloned selection.

### Shift + Drag Vertical Move

At the beginning of `HandleMouseMove`, before drag-state-specific movement is processed, the current grid location may be constrained by modifier keys.

The existing vertical-only drag is implemented by this exact-key check:

```csharp
if (ModifierKeys == Keys.Shift && m_mouseDownElements.Any())
	gridLocation.X = m_lastGridLocation.X;
```

This preserves the prior X coordinate used by the drag calculation, which makes `MouseMove_DragMoving` compute no horizontal time delta while still allowing vertical row movement.

## Conflict Analysis

The requested operation is viable, but it does not work as a pure emergent combination of the current code.

There is no conflict in the clone path. The clone trigger uses `CtrlPressed`, and `CtrlPressed` is implemented with `ModifierKeys.HasFlag(Keys.Control)`. Therefore `Ctrl + Shift + Drag` already reaches the same clone path as `Ctrl + Drag`.

There is a conflict in the horizontal-lock path. The Shift lock is currently tested with exact equality against `Keys.Shift`, so `ModifierKeys == (Keys.Control | Keys.Shift)` does not pass the existing `Shift + Drag` condition. Without a change, `Ctrl + Shift + Drag` clones the effects but allows horizontal movement, behaving like ordinary `Ctrl + Drag` after clone creation.

The existing Alt combinations must be preserved:

- `Alt + Control` currently locks horizontal movement for selected effect dragging.
- `Shift + Alt` currently locks vertical movement by preserving the prior Y coordinate.
- Resize logic also uses `AltPressed` and `ShiftPressed` for adjoining-effect resize behavior, but the requested feature is limited to whole-effect drag movement and should not change resize behavior.

No repository evidence shows an existing `Ctrl + Shift + Drag` whole-effect movement operation with different semantics in the timeline grid. The combined modifier is used elsewhere for unrelated actions, such as mark creation options, but those paths are not the selected-effect drag path.

## Goals

The implementation must:

- Make `Ctrl + Shift + Drag` on selected timeline effects clone the selected effects exactly as `Ctrl + Drag` does.
- Drag the cloned effects vertically while preserving each clone's original `StartTime` and `Duration`, exactly as `Shift + Drag` preserves time for existing effects.
- Preserve existing `Ctrl + Drag`, `Shift + Drag`, `Alt + Control + Drag`, and `Shift + Alt + Drag` behavior.
- Preserve existing selection behavior used to decide which effects are cloned.
- Preserve existing undo behavior: adding clones remains undoable as an add operation, and the final drag move remains undoable as a move operation when movement is recorded.
- Preserve snapping, row-boundary checks, deprecated-row blocking, and multi-row duplicate-instance handling already used by normal drag movement.

## Non-Goals

The implementation must not:

- Add a new menu item or user preference.
- Change effect resizing semantics.
- Change draw-mode behavior.
- Change lasso selection behavior.
- Change drag/drop behavior for effects dragged from the effects toolbar or external files.
- Change how cloned effect data, layers, or sequence insertion are created.
- Merge clone-add undo and move undo into a new combined undo action unless a separate product decision is made.

## Target User Experience

When one or more effects are selected in the timeline grid:

1. The user holds `Ctrl + Shift`.
2. The user presses the left mouse button on a selected effect and drags beyond the drag threshold.
3. The selected effects are cloned using the same behavior as `Ctrl + Drag`.
4. The cloned effects become the selected drag targets.
5. As the pointer moves, the clones can move up or down to other rows, subject to the same row constraints as ordinary drag movement.
6. The clones do not move earlier or later in time. Their start and end times remain equal to the newly created clone times, which initially match the source effects.
7. On mouse release, the clones remain in the destination row positions at the original time positions.

If the user releases the mouse before moving outside the drag threshold, no drag clone operation should be introduced beyond the selection behavior already present today.

## Detailed Requirements

The movement-lock decision should be made in the existing grid mouse-move modifier handling before `MouseMove_DragMoving` calculates `dt` and `dy`.

`Ctrl + Shift + Drag` must use the same X-locking mechanism as `Shift + Drag`. Practically, this means the grid location passed into drag-move processing must keep the previous X coordinate while still allowing Y to change.

The implementation should avoid replacing the current exact-key checks with a broad `ShiftPressed` check unless it also preserves the existing `Shift + Alt` behavior. A broad `ShiftPressed` condition would also match `Shift + Alt`; if applied before the existing `Shift + Alt` Y-lock, it could accidentally lock both axes. The safer implementation is a small helper or explicit condition that identifies vertical-only drag modifiers, for example:

```csharp
private bool VerticalOnlyDragModifierPressed =>
	ModifierKeys == Keys.Shift || ModifierKeys == (Keys.Control | Keys.Shift);
```

The exact name is an implementation detail. The required behavior is that `Keys.Shift` and `Keys.Control | Keys.Shift` both preserve X, while `Keys.Shift | Keys.Alt` continues to preserve Y as it does today.

The clone trigger should continue to use `CtrlPressed`. No separate clone path should be added for `Ctrl + Shift`.

The final clone placement should be governed by the existing drag-move path:

- Time movement is calculated in `MouseMove_DragMoving` from the constrained `gridLocation.X`.
- Vertical row changes are applied by `MoveElementsVerticallyToLocation`.
- Deprecated destination rows remain invalid.
- Movement remains clipped to visible row boundaries.
- Existing snap calculations should have no practical time movement to apply when the X coordinate is locked.

## Undo and Modified State

The existing clone operation records an `EffectsAddedUndoAction` and marks the sequence modified. The existing drag-move operation records an `ElementsTimeChangedUndoAction` through `ElementsMovedNew`.

For this feature, retain that existing model unless implementation proves that vertical-only movement of newly cloned effects fails to produce an accurate undo entry. If a clone is created and moved vertically, undo should allow the user to back out the placement and clone addition using the same undo stack behavior they experience with current `Ctrl + Drag`.

The implementation must not introduce a new sequence-modified path beyond the clone and move paths already used by `Ctrl + Drag`.

## Acceptance Criteria

- `Ctrl + Drag` still clones selected effects and allows horizontal and vertical movement.
- `Shift + Drag` still moves selected existing effects vertically without changing their start or end times.
- `Ctrl + Shift + Drag` clones the selected effects, selects the clones, and moves only the clones vertically.
- During `Ctrl + Shift + Drag`, cloned effects keep their source start times and durations.
- During `Ctrl + Shift + Drag`, cloned effects can move to valid rows using the same row constraints as normal drag movement.
- During `Ctrl + Shift + Drag`, dragging toward a deprecated row is blocked the same way normal effect drag is blocked.
- `Shift + Alt + Drag` still preserves the existing vertical-coordinate lock behavior and is not converted into a no-movement drag.
- `Alt + Control + Drag` retains its current behavior.
- Clicking with `Ctrl + Shift` but not dragging beyond the drag threshold does not create cloned effects.
- Undo behavior remains consistent with `Ctrl + Drag`: clone creation and final placement can be undone through the existing undo stack behavior.

## Testing Guidance

Automated testing may be difficult because the relevant behavior is WinForms mouse interaction in `Grid`. If practical, isolate the modifier decision behind a small helper and unit test that helper.

Recommended automated coverage:

- `Keys.Shift` is recognized as vertical-only drag.
- `Keys.Control | Keys.Shift` is recognized as vertical-only drag.
- `Keys.Control` is not recognized as vertical-only drag.
- `Keys.Shift | Keys.Alt` preserves the existing Alt+Shift path and is not treated as the same operation as `Ctrl + Shift`.
- `Keys.Control | Keys.Alt` preserves the existing Control+Alt path.

Manual validation should be performed in the Timed Sequence Editor with at least three visible rows and multiple selected effects:

1. Select one effect, `Ctrl + Drag` it diagonally, and verify the clone can move in time and rows.
2. Select one effect, `Shift + Drag` it diagonally, and verify the original moves rows but keeps its time.
3. Select one effect, `Ctrl + Shift + Drag` it diagonally, and verify a clone is created, the original remains in place, and the clone moves rows while keeping the original time.
4. Select multiple effects on one row, `Ctrl + Shift + Drag` them to another row, and verify all clones keep their relative row/time placement as much as the existing vertical movement rules allow.
5. Select effects across multiple visible rows, `Ctrl + Shift + Drag` them up and down near grid boundaries, and verify clipping matches normal vertical drag behavior.
6. Attempt `Ctrl + Shift + Drag` onto a deprecated row and verify the move is blocked.
7. Press `Ctrl + Shift` and click an effect without crossing the drag threshold; verify no clone is created.
8. Validate undo after a completed `Ctrl + Shift + Drag` until the sequence returns to the pre-drag state.
9. Regression-test `Shift + Alt + Drag` and `Alt + Control + Drag` against current expected behavior.

## Implementation Notes

Start by reading:

- `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs`

The smallest expected code change is in `Grid_Mouse.HandleMouseMove`, where the exact Shift-only horizontal lock should be expanded to include `Control | Shift` while preserving the exact Alt combinations.

Avoid changing `CloneSelectedElementsForMove()` unless a defect is found during implementation. The current clone-selection handoff is already the desired behavior for this feature.

## Viability Decision

The improvement is viable.

The only identified conflict is the exact-key Shift check in `HandleMouseMove`, which prevents the Shift movement constraint from applying when Ctrl is also held. Since the clone path already accepts Ctrl as a flag, the feature can be implemented as a narrow movement-modifier change with focused regression testing around existing Alt modifier combinations.

## Open Questions

No product clarification is required before creating the JIRA issue.

One implementation detail should be confirmed during development: whether the existing two-step undo behavior for `Ctrl + Drag` is considered acceptable for `Ctrl + Shift + Drag`. This specification preserves current behavior, but the JIRA issue may explicitly call out undo validation so any existing inconsistency is discovered during implementation.


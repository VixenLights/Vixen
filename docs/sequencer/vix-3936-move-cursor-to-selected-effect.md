# VIX-3936 Move Timeline Cursor To Selected Effect Specification

## Overview

VIX-3936 changes the Timed Sequence Editor default behavior so selecting an effect moves the timeline cursor to the start of that effect. It is tracked as a related change to VIX-2221. Today, selecting an effect updates the active row, but the cursor only moves when the user clicks empty grid space or performs other cursor-specific actions. This creates extra work for users who select an effect and then expect paste, playback-start, or edit workflows that depend on the cursor to begin at the selected effect. Users who prefer the old behavior can enable `Legacy Cursor / Active Row` from the Edit menu.

This document is the requirements and design specification for implementation. The implementation ExecPlan must follow `.agents/PLANS.md`, must be saved under `docs/plans/`, and must include steps to update the existing JIRA issue with the refined specification, acceptance criteria, and testing guidance from this document.

## Repository Context

The timeline grid control is implemented in `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` and `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs`. The grid stores selected effects as `Element.Selected`, exposes the current selection through `Grid.SelectedElements`, and tracks the active row through `Row.Active` and `Grid.ActiveRow`.

The timeline cursor is shared through `Common.Controls.TimelineControl.TimeLineGlobalStateManager` in `src/Vixen.Common/Controls/TimeLineControl/TimeLineGlobalStateManager.cs`. Setting `TimeLineGlobalStateManager.CursorPosition` raises the timeline-wide cursor moved event, causing the grid, ruler, waveform, and Timed Sequence Editor status displays to update.

`Grid` already has a private `CursorPosition` property in `Grid.cs` that delegates to the shared `TimeLineGlobalStateManager`. Blank-grid clicks and right-click context selection currently update this property in `Grid_Mouse.cs`. Effect selection currently raises `ElementsSelected` and updates selected elements and the active row without moving the cursor.

The Timed Sequence Editor host is `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`. It wires `TimelineControl.ElementsSelected` to `timelineControl_ElementsSelected`, where overlapping effects under the cursor are handled by a popup menu. When the user chooses an item from that menu, `contextMenuStripElementSelectionItem_Click` calls `TimelineControl.SelectElement(tse)`.

The preference pattern to mirror is the existing CAD Style Selection Box toggle. It is represented by `Grid.aCadStyleSelectionBox`, the `cADStyleSelectionBoxToolStripMenuItem` Edit menu item in `TimedSequenceEditorForm.Designer.cs`, startup load from `XMLProfileSettings` in `TimedSequenceEditorForm.cs`, close-time save in `TimedSequenceEditorForm.cs`, and click synchronization in `TimedSequenceEditorForm_Menu.cs`.

## Goals

The implementation must:

- Make selected effects move the timeline cursor by default when selected effects cause the active row to update.
- Add an opt-in legacy preference that restores the old cursor and active-row behavior.
- Add an Edit menu toggle similar to `CAD Style Selection Box`.
- Persist the toggle through `XMLProfileSettings` app settings.
- Move the cursor to a deterministic effect start time using the selection rules in this specification.
- When `Legacy Cursor / Active Row` is disabled, finalize lasso cursor placement and active row together only after mouse release, when the selected effects are known.
- Treat cursor movement caused by selection as UI-only state, with no undo entry and no sequence modified flag.
- Preserve existing selection, active row, drag, resize, context menu, and playback behavior except where explicitly changed by the default non-legacy behavior.

## Non-Goals

The implementation must not remove or change the current blank-grid click behavior that moves the cursor to the clicked grid time.

The implementation must not change existing right-click behavior when `Legacy Cursor / Active Row` is checked. When `Legacy Cursor / Active Row` is unchecked, right-clicking an effect must not move the cursor; right-clicking an effect is a context-menu action, not a cursor-placement action for this feature.

The implementation must not scroll the timeline horizontally when moving the cursor to a selected effect. The cursor position should change even if the target time is outside the current visible time span, but `VisibleTimeStart` must not be changed for this feature.

The implementation must not make the new behavior mandatory. The old behavior remains available when the `Legacy Cursor / Active Row` preference is checked.

The implementation must not add an undo unit, mark the sequence modified, or save cursor movement as sequence content.

## Target User Experience

The Edit menu contains a new checked menu item, near `CAD Style Selection Box`, named `Legacy Cursor / Active Row`. The menu item uses `CheckOnClick = true`.

By default, the item is unchecked. With the item unchecked, selecting effects uses the new VIX-3936 behavior: selecting an effect updates the active row and moves the cursor according to the selection rules in this document.

When the item is checked, selecting effects uses legacy behavior. Selecting an effect updates active row as it did before VIX-3936, but it does not move the cursor merely because an effect was selected.

When the user clicks one effect with the left mouse button, the cursor moves to that effect's `StartTime`.

When multiple effects become selected in a single selection operation, the default target is the earliest selected effect start time. The Ctrl and Shift cases have a more specific rule: when Ctrl or Shift selection adds or changes the selection, the cursor moves to the last selected effect associated with that user action. In mouse-driven selection this means the effect or effects under the latest click, not the earliest item already present in the previous selection.

When `Legacy Cursor / Active Row` is unchecked and the user uses a lasso selection box to select multiple effects, the cursor target is the earliest `StartTime` among the effects in the final lasso selection. The cursor must not move while the user is drawing the lasso. It moves only after the user releases the mouse button and the final selected effect set is known.

When `Legacy Cursor / Active Row` is unchecked during a lasso multi-select, the active row target is the row where the lasso operation started, not necessarily the row containing the earliest selected effect. The active row must not change while the user is drawing the lasso. The active row is set at the same finalization point as cursor movement: after mouse release, when the final lasso selection is known.

When `Legacy Cursor / Active Row` is checked, all lasso behavior remains as it was before VIX-3936. The added deferred cursor placement and lasso-origin active-row behavior do not apply.

When `Legacy Cursor / Active Row` is unchecked, the lasso requirements apply to both selection-box styles: the normal selection box and the CAD Style Selection Box controlled by the existing `CAD Style Selection Box` preference.

When the user clicks an effect that is already selected, the cursor still moves to that effect's start time if `Legacy Cursor / Active Row` is unchecked.

The codebase still contains a legacy overlapping-effects selection popup path, but normal grid drawing stacks overlapping effects so this path is not expected to be reachable through ordinary user interaction. If the legacy popup path is triggered, the cursor must not move when the popup opens. It moves only after the user chooses a specific effect from the popup, using that chosen effect's start time.

When `Legacy Cursor / Active Row` is checked, right-click behavior remains exactly as it was before VIX-3936, including any existing cursor movement caused by right-click context selection. When `Legacy Cursor / Active Row` is unchecked and the user right-clicks on any effect, the cursor must not move because of this feature. The effect context menu behavior and active row behavior remain intact. When the user right-clicks empty grid space, the existing behavior remains: the cursor moves to the clicked grid time.

Selection-driven cursor movement must mirror the conditions that currently set the active row. If a code path updates active row for an effect selection, then the default non-legacy behavior should move the cursor for the same interaction. If active row is intentionally not updated for an interaction, this feature should not create a new cursor movement for that interaction.

Playback interaction must also mirror active row behavior. If the current code allows active row to change during playback for a selection path, the default non-legacy behavior may move the edit cursor for the same selection path. If a path suppresses active row updates during playback, cursor movement must be suppressed there as well.

## Detailed Selection Rules

The implementation should centralize the cursor movement decision so that future selection paths do not drift. A helper on `Grid` is recommended, for example a private method that accepts the selected elements for the current user action and an enum or boolean that describes whether the operation should use the earliest selected effect or the last selected effect. The exact names are implementation details, but the resulting behavior must match this section.

For plain single-effect selection, set `CursorPosition` to the selected effect's `StartTime`.

For selecting an already-selected effect without Ctrl or Shift, set `CursorPosition` to the clicked effect's `StartTime`, even if the selection collection does not change.

For a selection operation that selects multiple effects without Ctrl or Shift, set `CursorPosition` to the earliest `StartTime` among the effects selected by that operation.

For a lasso selection operation while `LegacyCursorActiveRow` is disabled, treat mouse-up as the selection action boundary. While the lasso is being drawn, do not change `CursorPosition` and do not change `Row.Active`. At mouse-up, after the lasso has produced the final selected element set, set the active row to the row where the lasso began and, when the final selected set is not empty, set `CursorPosition` to the smallest `StartTime` among the selected effects. This rule applies identically to normal lasso selection and CAD-style lasso selection. When `LegacyCursorActiveRow` is enabled, preserve the existing lasso cursor and active-row behavior exactly.

For Ctrl-click and Shift-click selection operations, set `CursorPosition` to the start time of the last selected effect for that operation. If the operation selects a range, the last selected effect should be the effect that corresponds to the user's latest clicked location. If the implementation cannot identify a single last selected effect for a non-mouse range operation, use the latest action's selected effect set and choose the effect with the greatest row/time ordering closest to the latest user action, then document that decision in the ExecPlan.

For the legacy overlapping-effects popup path handled by `timelineControl_ElementsSelected`, no cursor movement occurs when `AutomaticallyHandleSelection` is set to false and the popup is shown. When `contextMenuStripElementSelectionItem_Click` later selects the chosen `TimedSequenceElement`, the cursor moves to that chosen effect's `StartTime`. This is fallback behavior only, not a required manual validation scenario.

For right-click context selection, preserve existing behavior when `LegacyCursorActiveRow` is enabled. When `LegacyCursorActiveRow` is disabled and `m_mouseDownElements` or equivalent hit-test state contains one or more elements under the cursor, do not change `CursorPosition` for this feature. If no effect is under the cursor, keep the existing empty-grid behavior that sets `CursorPosition` to `PixelsToTime(gridLocation.X)`.

## Preference and Persistence Requirements

Add a public boolean property to `Grid`, named consistently with the existing grid properties, such as `LegacyCursorActiveRow`. The default value must be `false`.

Add a `ToolStripMenuItem` to `TimedSequenceEditorForm.Designer.cs` in the Edit menu near `cADStyleSelectionBoxToolStripMenuItem`. The item must have `CheckOnClick = true`, text `Legacy Cursor / Active Row`, and a click handler in `TimedSequenceEditorForm_Menu.cs` that assigns its checked value to `TimelineControl.grid.LegacyCursorActiveRow`.

Load the setting in `TimedSequenceEditorForm.cs` near the existing CAD setting:

    legacyCursorActiveRowToolStripMenuItem.Checked = TimelineControl.grid.LegacyCursorActiveRow =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LegacyCursorActiveRow", Name), false);

Save the setting in `TimedSequenceEditorForm.cs` near the existing CAD setting:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LegacyCursorActiveRow", Name), legacyCursorActiveRowToolStripMenuItem.Checked);

The exact menu field name may differ, but the setting key must be stable and must default to `false`.

## Implementation Guidance

Start by reading `Grid.cs`, `Grid_Mouse.cs`, `TimelineControl.cs`, `EventArgs.cs`, `TimeLineGlobalStateManager.cs`, `TimedSequenceEditorForm.cs`, `TimedSequenceEditorForm_Menu.cs`, and `TimedSequenceEditorForm.Designer.cs`.

In `Grid_Mouse.cs`, the most important current selection paths are:

- Left-click effect selection where `_ElementsSelected(m_mouseDownElements)` returns true, selected elements are marked selected, and the row under the click is made active.
- Mouse-up handling for clicking one of multiple selected elements, where selection is reduced to the clicked element and active row is set.
- Ctrl and Shift paths that keep or extend the selection and set active row.
- Normal and CAD-style lasso selection paths that update selected elements while the user drags a selection box. When `LegacyCursorActiveRow` is disabled, these paths must defer both cursor movement and active-row assignment until mouse-up, then set active row to the lasso-origin row and move the cursor to the earliest selected effect start in the final selection. When `LegacyCursorActiveRow` is enabled, these paths must keep existing lasso behavior.
- Right-click context handling that sets active row, may set `CursorPosition`, and raises `_ContextSelected`. Existing right-click behavior must remain unchanged while `LegacyCursorActiveRow` is enabled. When the preference is disabled, it must suppress cursor movement only for right-clicks on effects; empty-grid right-clicks keep the current cursor placement behavior.

In `TimedSequenceEditorForm.cs`, the legacy overlapping-effects popup path must be preserved if it is triggered. The popup is shown when `timelineControl_ElementsSelected` sees more than one element under the cursor and sets `AutomaticallyHandleSelection = false`. Cursor movement for that path belongs after a concrete element is chosen in `contextMenuStripElementSelectionItem_Click`, not at popup-open time. Because current row drawing stacks overlapping effects, do not require a manual validation scenario that tries to trigger this popup through ordinary grid clicking.

Prefer changing shared grid behavior where the active row is already updated, rather than adding Timed Sequence Editor-only cursor behavior for ordinary grid clicks. The grid already owns `CursorPosition` and active-row update logic, so keeping ordinary selection-driven movement in `Grid` reduces duplicate event handling. The Timed Sequence Editor host should only need explicit handling for the overlapping-effects popup selection because it suppresses automatic grid selection.

Do not call `SequenceModified()` for this feature. Do not add undo commands. Do not write to sequence model fields.

## Validation and Acceptance

Run the focused test project from the repository root after implementation:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore

If full tests are too slow during development, add and run focused tests for the new selection helper or grid behavior first, then run the full test command before final completion.

Manual acceptance should be performed in the Timed Sequence Editor with a sequence containing at least two rows and several effects:

1. With `Legacy Cursor / Active Row` unchecked, click an effect. The active row changes to the effect row, and the timeline cursor moves to the effect's start time.
2. Check `Edit > Legacy Cursor / Active Row`. Click a single effect. The active row changes to that effect row, and the timeline cursor does not move merely because of the selection.
3. Click an already-selected effect. The cursor moves to that effect's start time again.
4. Select multiple effects in one operation. The cursor moves according to the rules in this document: earliest selected effect for general multi-select, last selected effect for Ctrl or Shift selection.
5. With the menu item unchecked, draw a normal lasso around effects on multiple rows. While drawing, neither the cursor nor the active row changes. On mouse release, the cursor moves to the earliest selected effect start time and the active row becomes the row where the lasso began.
6. Repeat the prior lasso validation with `CAD Style Selection Box` enabled. The same mouse-up timing, earliest-start cursor placement, and lasso-origin active row behavior apply.
7. With `Legacy Cursor / Active Row` checked, perform normal and CAD-style lasso selections and confirm existing lasso cursor and active-row behavior is unchanged.
8. With `Legacy Cursor / Active Row` checked, right-click an effect and confirm existing right-click behavior is unchanged. With the preference unchecked, right-click an effect and confirm the active row and context menu behavior remain intact but the cursor does not move because of the new preference. Right-click empty grid space still moves the cursor to the clicked time.
9. Select an effect whose start time is outside the currently visible timeline range through an existing non-scrolling selection path, if available. The cursor position changes, but the timeline does not scroll horizontally because of this feature.
10. Close and reopen the Timed Sequence Editor. The menu item retains its prior checked state. A fresh profile or missing setting defaults to unchecked.

The core acceptance criterion is: when `Legacy Cursor / Active Row` is unchecked and a user selects an effect in the timeline grid, the timeline cursor moves to the selected effect's start time, while the active row continues to update to the selected effect's row.

## Testing Guidance

Add unit tests where feasible for any new helper that chooses the target cursor time. The tests should cover single selection, multi-selection earliest-start behavior, Ctrl/Shift last-selected behavior, unchecked-legacy right-click-on-effect no-op behavior, checked-legacy right-click preservation, and empty selection no-op behavior.

Add automated coverage for lasso finalization if the lasso selection code can be exercised without brittle mouse simulation. The preferred coverage is a focused test around any helper introduced for non-legacy lasso finalization: given a lasso-origin row and a final selected effect set with multiple start times, it sets active row to the origin row and chooses the smallest selected start time for cursor movement only when `LegacyCursorActiveRow` is disabled. Add enabled-legacy coverage that confirms the new lasso finalization helper does not change cursor or active row and existing lasso behavior is preserved. If the existing lasso implementation can only be validated through WinForms mouse interaction, keep automated tests focused on the target-selection helper and document manual validation for normal and CAD-style lasso.

If direct `Grid` interaction tests are impractical because the control is WinForms-heavy, keep the target-time selection logic isolated enough to test without launching the full UI. Avoid relying solely on manual testing for the ordering rules because regressions in Ctrl/Shift behavior are easy to miss.

For persistence, add a focused test only if the project already has practical coverage around `XMLProfileSettings` or Timed Sequence Editor settings. Otherwise document manual persistence validation in the ExecPlan and perform it during implementation.

## Open Questions

No open product questions remain from the specification discussion. If implementation discovers a selection path that sets active row but has no clear selected effect target, update this document or the implementation ExecPlan with the chosen behavior before coding past it.

## Decision Log

- Decision: The new behavior was initially specified as opt-in with the old behavior as the default. This decision is superseded by the 2026-07-10 decision to make the new behavior the default and expose legacy behavior through `Legacy Cursor / Active Row`.
  Rationale: The user originally requested a preference setting to enable the new behavior versus the old behavior, then later requested the feature default to enabled.
  Date/Author: 2026-07-08 / Codex from user direction; superseded 2026-07-10 / user direction recorded by Codex.

- Decision: The new cursor and active-row behavior is now the default, and the Edit menu exposes `Legacy Cursor / Active Row` to restore the old behavior.
  Rationale: The user requested the feature default to enabled and the menu item become the opt-in path for legacy behavior.
  Date/Author: 2026-07-10 / user direction recorded by Codex.

- Decision: Cursor movement should mirror the conditions that set active row.
  Rationale: This keeps the feature aligned with the existing selection model and avoids moving the cursor for interactions that are not considered active effect selection today.
  Date/Author: 2026-07-08 / Codex from user direction.

- Decision: Single-effect selection and already-selected effect clicks move the cursor to that effect's start time.
  Rationale: This is the primary user workflow requested by VIX-3936, related to VIX-2221.
  Date/Author: 2026-07-08 / Codex from user direction.

- Decision: General multi-select uses the earliest selected effect, while Ctrl/Shift selection uses the last selected effect.
  Rationale: Earliest start gives a stable start point for multi-effect operations; Ctrl/Shift should follow the latest user action.
  Date/Author: 2026-07-08 / Codex from user direction.

- Decision: Non-legacy lasso multi-select moves the cursor to the earliest selected effect only after mouse release, and active row becomes the lasso-origin row at that same finalization point.
  Rationale: The user requested that cursor and active row remain stable while drawing the lasso, then update after the final selection is known. On 2026-07-10 the user clarified that all of these new lasso behaviors are contained in the non-legacy behavior and must not apply when `Legacy Cursor / Active Row` is checked.
  Date/Author: 2026-07-10 / user direction recorded by Codex; updated 2026-07-10 / user clarification recorded by Codex.

- Decision: Normal lasso and CAD-style lasso share the same cursor and active-row behavior.
  Rationale: The user explicitly requested identical behavior for either style lasso.
  Date/Author: 2026-07-10 / user direction recorded by Codex.

- Decision: Overlapping-effects popup selection moves the cursor only after the user chooses an effect.
  Rationale: Opening the popup has not yet identified the user's intended effect. On 2026-07-09 the user clarified that this popup is likely obsolete because overlapping effects are drawn in a stacked manner, so this remains fallback behavior and is not a required manual validation activity.
  Date/Author: 2026-07-08 / Codex from user direction; updated 2026-07-09 / user clarification recorded by Codex.

- Decision: Existing right-click behavior remains unchanged when `Legacy Cursor / Active Row` is checked. When it is unchecked, right-clicking an effect does not move the cursor; right-clicking empty grid keeps raw grid-time behavior.
  Rationale: The user clarified that the no-move rule belongs to the new cursor behavior. With legacy mode enabled, Vixen must preserve old right-click behavior exactly.
  Date/Author: 2026-07-09 / Codex from user direction; updated 2026-07-10 / user direction recorded by Codex.

- Decision: Selection-driven cursor movement does not scroll the viewport and is UI-only.
  Rationale: The user requested only cursor movement, not automatic horizontal scrolling, and specified that the action should not affect undo or dirty state.
  Date/Author: 2026-07-08 / Codex from user direction.

## Notes for JIRA

Use this concise JIRA summary:

    Make selected effects move the timeline cursor by default, with an optional Timed Sequence Editor legacy preference that restores the previous cursor and active-row behavior.

Use this acceptance text:

    By default, selecting an effect in the timeline grid moves the timeline cursor to the selected effect's start time while preserving active row behavior. With Legacy Cursor / Active Row unchecked, normal and CAD-style lasso multi-select do not change the cursor or active row while drawing; after mouse release, the cursor moves to the earliest selected effect start and the active row becomes the row where the lasso began. With Legacy Cursor / Active Row checked, existing cursor and active-row behavior is restored. The preference persists between editor sessions, does not scroll the viewport, and does not mark the sequence modified or create undo history.

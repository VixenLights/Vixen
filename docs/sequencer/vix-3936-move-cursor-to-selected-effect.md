# VIX-3936 Move Timeline Cursor To Selected Effect Specification

## Overview

VIX-3936 adds an optional Timed Sequence Editor behavior that moves the timeline cursor to the start of an effect when that effect is selected in the grid. It is tracked as a related change to VIX-2221. Today, selecting an effect updates the active row, but the cursor only moves when the user clicks empty grid space or performs other cursor-specific actions. This creates extra work for users who select an effect and then expect paste, playback-start, or edit workflows that depend on the cursor to begin at the selected effect.

This document is the requirements and design specification for implementation. The implementation ExecPlan must follow `.agents/PLANS.md`, must be saved under `docs/plans/`, and must include steps to update the existing JIRA issue with the refined specification, acceptance criteria, and testing guidance from this document.

## Repository Context

The timeline grid control is implemented in `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` and `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs`. The grid stores selected effects as `Element.Selected`, exposes the current selection through `Grid.SelectedElements`, and tracks the active row through `Row.Active` and `Grid.ActiveRow`.

The timeline cursor is shared through `Common.Controls.TimelineControl.TimeLineGlobalStateManager` in `src/Vixen.Common/Controls/TimeLineControl/TimeLineGlobalStateManager.cs`. Setting `TimeLineGlobalStateManager.CursorPosition` raises the timeline-wide cursor moved event, causing the grid, ruler, waveform, and Timed Sequence Editor status displays to update.

`Grid` already has a private `CursorPosition` property in `Grid.cs` that delegates to the shared `TimeLineGlobalStateManager`. Blank-grid clicks and right-click context selection currently update this property in `Grid_Mouse.cs`. Effect selection currently raises `ElementsSelected` and updates selected elements and the active row without moving the cursor.

The Timed Sequence Editor host is `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`. It wires `TimelineControl.ElementsSelected` to `timelineControl_ElementsSelected`, where overlapping effects under the cursor are handled by a popup menu. When the user chooses an item from that menu, `contextMenuStripElementSelectionItem_Click` calls `TimelineControl.SelectElement(tse)`.

The preference pattern to mirror is the existing CAD Style Selection Box toggle. It is represented by `Grid.aCadStyleSelectionBox`, the `cADStyleSelectionBoxToolStripMenuItem` Edit menu item in `TimedSequenceEditorForm.Designer.cs`, startup load from `XMLProfileSettings` in `TimedSequenceEditorForm.cs`, close-time save in `TimedSequenceEditorForm.cs`, and click synchronization in `TimedSequenceEditorForm_Menu.cs`.

## Goals

The implementation must:

- Add an opt-in preference that moves the timeline cursor when selected effects cause the active row to update.
- Keep the old behavior as the default for existing and new users.
- Add an Edit menu toggle similar to `CAD Style Selection Box`.
- Persist the toggle through `XMLProfileSettings` app settings.
- Move the cursor to a deterministic effect start time using the selection rules in this specification.
- Treat cursor movement caused by selection as UI-only state, with no undo entry and no sequence modified flag.
- Preserve existing selection, active row, drag, resize, context menu, and playback behavior except where explicitly changed by the enabled preference.

## Non-Goals

The implementation must not remove or change the current blank-grid click behavior that moves the cursor to the clicked grid time.

The implementation must not change existing right-click behavior when the new preference is disabled. When the new preference is enabled, right-clicking an effect must not move the cursor; right-clicking an effect is a context-menu action, not a cursor-placement action for this feature.

The implementation must not scroll the timeline horizontally when moving the cursor to a selected effect. The cursor position should change even if the target time is outside the current visible time span, but `VisibleTimeStart` must not be changed for this feature.

The implementation must not make this behavior mandatory. The old behavior remains the default when the new preference is unchecked.

The implementation must not add an undo unit, mark the sequence modified, or save cursor movement as sequence content.

## Target User Experience

The Edit menu contains a new checked menu item, near `CAD Style Selection Box`, named `Move Cursor To Selected Effect` or equivalent clear wording. The menu item uses `CheckOnClick = true`.

By default, the item is unchecked. With the item unchecked, selecting effects behaves exactly as it does today: selecting an effect updates the active row, but the cursor does not move simply because an effect was selected.

When the item is checked, selecting an effect in the timeline grid also moves the timeline cursor to the selected effect start time. The active row continues to update to the selected effect row.

When the user clicks one effect with the left mouse button, the cursor moves to that effect's `StartTime`.

When multiple effects become selected in a single selection operation, the default target is the earliest selected effect start time. The Ctrl and Shift cases have a more specific rule: when Ctrl or Shift selection adds or changes the selection, the cursor moves to the last selected effect associated with that user action. In mouse-driven selection this means the effect or effects under the latest click, not the earliest item already present in the previous selection.

When the user clicks an effect that is already selected, the cursor still moves to that effect's start time if the enabled preference is on.

The codebase still contains a legacy overlapping-effects selection popup path, but normal grid drawing stacks overlapping effects so this path is not expected to be reachable through ordinary user interaction. If the legacy popup path is triggered, the cursor must not move when the popup opens. It moves only after the user chooses a specific effect from the popup, using that chosen effect's start time.

When the new preference is disabled, right-click behavior remains exactly as it is today, including any existing cursor movement caused by right-click context selection. When the new preference is enabled and the user right-clicks on any effect, the cursor must not move because of this feature. The effect context menu behavior and active row behavior remain intact. When the user right-clicks empty grid space, the existing behavior remains: the cursor moves to the clicked grid time.

Selection-driven cursor movement must mirror the conditions that currently set the active row. If a code path updates active row for an effect selection, then the enabled preference should move the cursor for the same interaction. If active row is intentionally not updated for an interaction, this feature should not create a new cursor movement for that interaction.

Playback interaction must also mirror active row behavior. If the current code allows active row to change during playback for a selection path, the enabled preference may move the edit cursor for the same selection path. If a path suppresses active row updates during playback, cursor movement must be suppressed there as well.

## Detailed Selection Rules

The implementation should centralize the cursor movement decision so that future selection paths do not drift. A helper on `Grid` is recommended, for example a private method that accepts the selected elements for the current user action and an enum or boolean that describes whether the operation should use the earliest selected effect or the last selected effect. The exact names are implementation details, but the resulting behavior must match this section.

For plain single-effect selection, set `CursorPosition` to the selected effect's `StartTime`.

For selecting an already-selected effect without Ctrl or Shift, set `CursorPosition` to the clicked effect's `StartTime`, even if the selection collection does not change.

For a selection operation that selects multiple effects without Ctrl or Shift, set `CursorPosition` to the earliest `StartTime` among the effects selected by that operation.

For Ctrl-click and Shift-click selection operations, set `CursorPosition` to the start time of the last selected effect for that operation. If the operation selects a range, the last selected effect should be the effect that corresponds to the user's latest clicked location. If the implementation cannot identify a single last selected effect for a non-mouse range operation, use the latest action's selected effect set and choose the effect with the greatest row/time ordering closest to the latest user action, then document that decision in the ExecPlan.

For the legacy overlapping-effects popup path handled by `timelineControl_ElementsSelected`, no cursor movement occurs when `AutomaticallyHandleSelection` is set to false and the popup is shown. When `contextMenuStripElementSelectionItem_Click` later selects the chosen `TimedSequenceElement`, the cursor moves to that chosen effect's `StartTime`. This is fallback behavior only, not a required manual validation scenario.

For right-click context selection, preserve existing behavior when `MoveCursorToSelectedEffect` is disabled. When `MoveCursorToSelectedEffect` is enabled and `m_mouseDownElements` or equivalent hit-test state contains one or more elements under the cursor, do not change `CursorPosition` for this feature. If no effect is under the cursor, keep the existing empty-grid behavior that sets `CursorPosition` to `PixelsToTime(gridLocation.X)`.

## Preference and Persistence Requirements

Add a public boolean property to `Grid`, named consistently with the existing grid properties, such as `MoveCursorToSelectedEffect`. The default value must be `false`.

Add a `ToolStripMenuItem` to `TimedSequenceEditorForm.Designer.cs` in the Edit menu near `cADStyleSelectionBoxToolStripMenuItem`. The item must have `CheckOnClick = true` and a click handler in `TimedSequenceEditorForm_Menu.cs` that assigns its checked value to `TimelineControl.grid.MoveCursorToSelectedEffect`.

Load the setting in `TimedSequenceEditorForm.cs` near the existing CAD setting:

    moveCursorToSelectedEffectToolStripMenuItem.Checked = TimelineControl.grid.MoveCursorToSelectedEffect =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/MoveCursorToSelectedEffect", Name), false);

Save the setting in `TimedSequenceEditorForm.cs` near the existing CAD setting:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/MoveCursorToSelectedEffect", Name), moveCursorToSelectedEffectToolStripMenuItem.Checked);

The exact menu field name may differ, but the setting key must be stable and must default to `false`.

## Implementation Guidance

Start by reading `Grid.cs`, `Grid_Mouse.cs`, `TimelineControl.cs`, `EventArgs.cs`, `TimeLineGlobalStateManager.cs`, `TimedSequenceEditorForm.cs`, `TimedSequenceEditorForm_Menu.cs`, and `TimedSequenceEditorForm.Designer.cs`.

In `Grid_Mouse.cs`, the most important current selection paths are:

- Left-click effect selection where `_ElementsSelected(m_mouseDownElements)` returns true, selected elements are marked selected, and the row under the click is made active.
- Mouse-up handling for clicking one of multiple selected elements, where selection is reduced to the clicked element and active row is set.
- Ctrl and Shift paths that keep or extend the selection and set active row.
- Right-click context handling that sets active row, may set `CursorPosition`, and raises `_ContextSelected`. Existing right-click behavior must remain unchanged while `MoveCursorToSelectedEffect` is disabled. When the preference is enabled, it must suppress cursor movement only for right-clicks on effects; empty-grid right-clicks keep the current cursor placement behavior.

In `TimedSequenceEditorForm.cs`, the legacy overlapping-effects popup path must be preserved if it is triggered. The popup is shown when `timelineControl_ElementsSelected` sees more than one element under the cursor and sets `AutomaticallyHandleSelection = false`. Cursor movement for that path belongs after a concrete element is chosen in `contextMenuStripElementSelectionItem_Click`, not at popup-open time. Because current row drawing stacks overlapping effects, do not require a manual validation scenario that tries to trigger this popup through ordinary grid clicking.

Prefer changing shared grid behavior where the active row is already updated, rather than adding Timed Sequence Editor-only cursor behavior for ordinary grid clicks. The grid already owns `CursorPosition` and active-row update logic, so keeping ordinary selection-driven movement in `Grid` reduces duplicate event handling. The Timed Sequence Editor host should only need explicit handling for the overlapping-effects popup selection because it suppresses automatic grid selection.

Do not call `SequenceModified()` for this feature. Do not add undo commands. Do not write to sequence model fields.

## Validation and Acceptance

Run the focused test project from the repository root after implementation:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore

If full tests are too slow during development, add and run focused tests for the new selection helper or grid behavior first, then run the full test command before final completion.

Manual acceptance should be performed in the Timed Sequence Editor with a sequence containing at least two rows and several effects:

1. With `Move Cursor To Selected Effect` unchecked, click an effect. The active row changes to the effect row, and the timeline cursor does not move to the effect start merely because of the selection.
2. Check `Edit > Move Cursor To Selected Effect`. Click a single effect. The active row changes to that effect row, and the timeline cursor moves to the effect's start time.
3. Click an already-selected effect. The cursor moves to that effect's start time again.
4. Select multiple effects in one operation. The cursor moves according to the rules in this document: earliest selected effect for general multi-select, last selected effect for Ctrl or Shift selection.
5. With `Move Cursor To Selected Effect` unchecked, right-click an effect and confirm existing right-click behavior is unchanged. With the preference checked, right-click an effect and confirm the active row and context menu behavior remain intact but the cursor does not move because of the new preference. Right-click empty grid space still moves the cursor to the clicked time.
6. Select an effect whose start time is outside the currently visible timeline range through an existing non-scrolling selection path, if available. The cursor position changes, but the timeline does not scroll horizontally because of this feature.
7. Close and reopen the Timed Sequence Editor. The menu item retains its prior checked state. A fresh profile or missing setting defaults to unchecked.

The core acceptance criterion is: when the preference is enabled and a user selects an effect in the timeline grid, the timeline cursor moves to the selected effect's start time, while the active row continues to update to the selected effect's row.

## Testing Guidance

Add unit tests where feasible for any new helper that chooses the target cursor time. The tests should cover single selection, multi-selection earliest-start behavior, Ctrl/Shift last-selected behavior, preference-enabled right-click-on-effect no-op behavior, preference-disabled right-click preservation, and empty selection no-op behavior.

If direct `Grid` interaction tests are impractical because the control is WinForms-heavy, keep the target-time selection logic isolated enough to test without launching the full UI. Avoid relying solely on manual testing for the ordering rules because regressions in Ctrl/Shift behavior are easy to miss.

For persistence, add a focused test only if the project already has practical coverage around `XMLProfileSettings` or Timed Sequence Editor settings. Otherwise document manual persistence validation in the ExecPlan and perform it during implementation.

## Open Questions

No open product questions remain from the specification discussion. If implementation discovers a selection path that sets active row but has no clear selected effect target, update this document or the implementation ExecPlan with the chosen behavior before coding past it.

## Decision Log

- Decision: The new behavior is opt-in and the old behavior remains the default.
  Rationale: The user explicitly requested a preference setting to enable the new behavior versus the old behavior, with old behavior as default.
  Date/Author: 2026-07-08 / Codex from user direction.

- Decision: Cursor movement should mirror the conditions that set active row.
  Rationale: This keeps the feature aligned with the existing selection model and avoids moving the cursor for interactions that are not considered active effect selection today.
  Date/Author: 2026-07-08 / Codex from user direction.

- Decision: Single-effect selection and already-selected effect clicks move the cursor to that effect's start time.
  Rationale: This is the primary user workflow requested by VIX-3936, related to VIX-2221.
  Date/Author: 2026-07-08 / Codex from user direction.

- Decision: General multi-select uses the earliest selected effect, while Ctrl/Shift selection uses the last selected effect.
  Rationale: Earliest start gives a stable start point for multi-effect operations; Ctrl/Shift should follow the latest user action.
  Date/Author: 2026-07-08 / Codex from user direction.

- Decision: Overlapping-effects popup selection moves the cursor only after the user chooses an effect.
  Rationale: Opening the popup has not yet identified the user's intended effect. On 2026-07-09 the user clarified that this popup is likely obsolete because overlapping effects are drawn in a stacked manner, so this remains fallback behavior and is not a required manual validation activity.
  Date/Author: 2026-07-08 / Codex from user direction; updated 2026-07-09 / user clarification recorded by Codex.

- Decision: Existing right-click behavior remains unchanged when the new preference is disabled. When the new preference is enabled, right-clicking an effect does not move the cursor; right-clicking empty grid keeps raw grid-time behavior.
  Rationale: The user clarified that the no-move rule is only for the enabled preference state. With the preference disabled, Vixen must preserve today's right-click behavior exactly.
  Date/Author: 2026-07-09 / Codex from user direction.

- Decision: Selection-driven cursor movement does not scroll the viewport and is UI-only.
  Rationale: The user requested only cursor movement, not automatic horizontal scrolling, and specified that the action should not affect undo or dirty state.
  Date/Author: 2026-07-08 / Codex from user direction.

## Notes for JIRA

Use this concise JIRA summary:

    Add an optional Timed Sequence Editor preference that moves the timeline cursor to the start of a selected effect when effect selection updates the active row.

Use this acceptance text:

    When the new Edit menu preference is enabled, selecting an effect in the timeline grid moves the timeline cursor to the selected effect's start time while preserving active row behavior. The preference is off by default, persists between editor sessions, does not scroll the viewport, and does not mark the sequence modified or create undo history.

# VIX-3945 Lock Ruler Height Specification

## Overview

VIX-3945 adds a Timed Sequence Editor preference that prevents accidental ruler height changes. Today the user can resize the timeline ruler by hovering near the bottom edge of the ruler, getting the horizontal split cursor, and dragging vertically. The requested improvement adds a `View` menu option named `Lock Ruler Height`. The option defaults to off, so existing ruler resize behavior remains unchanged unless the user enables the lock.

This document is the requirements and design specification for implementation. The implementation ExecPlan must follow `.agents/PLANS.md`, must be saved under `docs/plans/`, and should include steps to update the existing JIRA issue with the refined specification, acceptance criteria, and testing guidance from this document.

JIRA issue: `VIX-3945`

## Repository Context

The timeline ruler control is implemented in `src/Vixen.Common/Controls/TimelineControl/Ruler.cs`. The current resize behavior is entirely local to this control. `Ruler.OnMouseMove` changes the cursor to `Cursors.HSplit` when the pointer is near the bottom edge of the ruler. `Ruler.OnMouseDown` enters `MouseState.ResizeRuler` when the current cursor is `Cursors.HSplit`. While the left mouse button is held, `Ruler.OnMouseMove` changes `Height` for `MouseState.ResizeRuler` when the pointer is lower than 40 pixels. `Ruler.OnMouseDoubleClick` resets the ruler height to 50 when the cursor is `Cursors.HSplit`.

The parent timeline control is implemented in `src/Vixen.Common/Controls/TimelineControl/TimelineControl.cs`. It creates the public `ruler` instance and docks it above the grid. The Timed Sequence Editor host is implemented in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`, `TimedSequenceEditorForm.Designer.cs`, and `TimedSequenceEditorForm_Menu.cs`.

Timed Sequence Editor profile settings are persisted through `XMLProfileSettings` app settings. `TimedSequenceEditorForm.cs` already loads and saves the existing `RulerHeight` setting next to related UI settings such as `WaveFormHeight`, `ResizeIndicatorEnabled`, `CadStyleSelectionBox`, and `LegacyCursorActiveRow`. The new lock setting should be saved in this same profile settings area.

## Goals

The implementation must:

- Add a checked `View` menu item named `Lock Ruler Height`.
- Default the menu item and setting to unchecked/off when the profile setting does not exist.
- Persist the setting in the XML profile with the other Timed Sequence Editor app settings.
- When the setting is off, preserve current ruler resize behavior, including the horizontal split cursor, drag-to-resize behavior, minimum practical resize threshold, double-click reset behavior, and saved `RulerHeight`.
- When the setting is on, prevent the user from resizing the ruler by dragging the ruler bottom edge.
- When the setting is on, prevent the ruler resize cursor indicator from appearing.
- Keep existing ruler behaviors unrelated to height resizing, including ruler clicks, time-range drag selection, mark selection, and mark movement.

## Non-Goals

The implementation must not:

- Change grid row height resizing or row height reset behavior.
- Change waveform height behavior.
- Change the existing effect resize indicator preference in the View/Edit menus; that preference controls timeline effect resize guides, not ruler height.
- Remove or ignore the existing saved `RulerHeight`. Locking the height should prevent interactive user resizing, not force the height back to a default.
- Mark the sequence modified, create undo history, or change sequence file contents.
- Add a toolbar button or keyboard shortcut unless a separate product decision is made.

## Target User Experience

The `View` menu contains a new checked menu item named `Lock Ruler Height`, placed directly below `Full Waveform`. A fresh profile or a profile missing the setting opens with the item unchecked.

With `Lock Ruler Height` unchecked, the ruler behaves exactly as it does today. Moving the mouse near the bottom of the ruler shows the horizontal split cursor, dragging changes the ruler height, and double-clicking the resize edge resets the height to 50 pixels. Closing and reopening the editor preserves the resulting `RulerHeight`.

With `Lock Ruler Height` checked, moving the mouse near the bottom of the ruler does not show the horizontal split cursor. The cursor remains the appropriate non-resize cursor for the current ruler area, such as the hand cursor for ordinary ruler interaction or the mark cursor when hovering a mark. Pressing and dragging near the bottom edge does not enter ruler-height resize mode and does not change the ruler height. Double-clicking near the bottom edge does not reset the ruler height merely because the lock is enabled.

Toggling the menu item applies immediately to the open editor. If the ruler is already at a custom height, enabling the lock keeps that current height and prevents further interactive resize. Disabling the lock restores the current resize affordance without changing the height.

## Detailed Requirements

Add a boolean property to the timeline/ruler surface, with a name such as `LockRulerHeight` or `RulerHeightLocked`. The property must ultimately be readable by `Ruler`. Because `Ruler` is a public field on `TimelineControl`, the smallest implementation is a public boolean property on `Ruler` and an optional forwarding property on `TimelineControl` for cleaner Timed Sequence Editor code. If a public or protected C# API is added or changed, update XML documentation in the same change and use the project `csharp-docs` skill before implementation.

The lock property default must be `false`. A missing XML profile setting must behave the same as `false`.

In `Ruler.OnMouseMove`, the unlocked path must continue to show `Cursors.HSplit` when `e.Location.Y <= Height - 1 && e.Location.Y >= Height - 6`. When the lock is enabled, that specific resize-zone branch must not set `Cursors.HSplit`. The code should fall through to the normal non-resize cursor behavior and must continue to clear alignment activity as the existing hand-cursor path does.

In `Ruler.OnMouseDown`, the resize state must not be entered while the lock is enabled. Guard the `Cursor == Cursors.HSplit` path with the lock state, or better, ensure the cursor can never be `Cursors.HSplit` because of ruler height when locked and still guard `OnMouseDown` defensively.

In `Ruler.OnMouseMove` while the left button is held, `MouseState.ResizeRuler` must not change `Height` while the lock is enabled. This is a defensive requirement for cases where the lock is toggled while a resize drag is in progress. This edge case is not expected during normal use, so do not add excessive state-management logic for it. A simple guard that stops height changes immediately is sufficient.

In `Ruler.OnMouseDoubleClick`, double-click reset to 50 must be disabled while the lock is enabled. This prevents a locked ruler from changing height through a second resize affordance.

The Timed Sequence Editor must load the setting near the existing menu and timeline settings in `TimedSequenceEditorForm.cs`, for example:

    lockRulerHeightToolStripMenuItem.Checked = TimelineControl.RulerHeightLocked =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockRulerHeight", Name), false);

The exact property name may differ, but the XML key should be stable. Use `{Name}/LockRulerHeight` unless implementation discovers an existing naming convention that is a better fit.

The Timed Sequence Editor must save the setting near the existing app setting writes in `TimedSequenceEditorForm.cs`, for example:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockRulerHeight", Name), lockRulerHeightToolStripMenuItem.Checked);

The menu click or checked-changed handler in `TimedSequenceEditorForm_Menu.cs` must synchronize the menu item checked state to the timeline/ruler lock property immediately.

## Implementation Guidance

Start by reading:

- `src/Vixen.Common/Controls/TimelineControl/Ruler.cs`
- `src/Vixen.Common/Controls/TimelineControl/TimelineControl.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`

The likely implementation path is:

1. Add the lock property to `Ruler`, defaulting to `false`.
2. Optionally add a forwarding property to `TimelineControl` so the editor does not reach through the public `ruler` field for this preference.
3. Update `Ruler.OnMouseMove`, `Ruler.OnMouseDown`, and `Ruler.OnMouseDoubleClick` so ruler-height resize affordances and height changes are disabled when the lock is enabled.
4. Add `lockRulerHeightToolStripMenuItem` to the View menu in `TimedSequenceEditorForm.Designer.cs`, directly below `Full Waveform`.
5. Add the menu handler in `TimedSequenceEditorForm_Menu.cs`.
6. Load and save `{Name}/LockRulerHeight` in `TimedSequenceEditorForm.cs` near `RulerHeight`.

Avoid changing the mark movement path in `Ruler`. The cursor can still become `Cursors.VSplit` over marks, and dragging marks must remain independent of the ruler height lock.

Avoid using `Enabled = false` on the whole ruler or intercepting all ruler mouse input. The lock is specifically about height resizing, not the ruler as an interactive timeline surface.

## ExecPlan Path

Create the implementation ExecPlan under `docs/plans/sequencer/vix-3945-lock-ruler-height.md`. The ExecPlan must be self-contained per `.agents/PLANS.md` and should include:

- Purpose and user-visible behavior.
- A context section explaining the `Ruler`, `TimelineControl`, and Timed Sequence Editor settings files.
- Milestones for control-level locking, menu and persistence wiring, and validation.
- A decision log entry for the chosen public property name and XML setting key.
- Manual validation steps for locked and unlocked behavior.
- Automated test guidance or a clear explanation if the WinForms interaction is only manually validated.

## Validation and Acceptance

Run the focused test project from the repository root after implementation:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore

If full tests are too slow during development, run a focused compile or targeted test first, then run the full command before final completion.

Manual acceptance should be performed in the Timed Sequence Editor:

1. Open a sequence with a fresh or missing `LockRulerHeight` profile setting. Confirm `View > Lock Ruler Height` is unchecked.
2. With the item unchecked, hover near the bottom edge of the ruler. Confirm the horizontal split cursor appears.
3. With the item unchecked, drag the ruler bottom edge. Confirm the ruler height changes.
4. With the item unchecked, double-click the ruler resize edge. Confirm the ruler height resets to 50 pixels.
5. Enable `View > Lock Ruler Height`. Confirm the ruler height does not change when the menu item is toggled.
6. With the item checked, hover near the bottom edge of the ruler. Confirm the horizontal split cursor does not appear.
7. With the item checked, drag near the bottom edge of the ruler. Confirm the ruler height does not change.
8. With the item checked, double-click near the bottom edge of the ruler. Confirm the ruler height does not reset.
9. With the item checked, click and drag normal time ranges on the ruler. Confirm ruler click and time-range drag behavior still works.
10. With the item checked, hover and drag existing marks on the ruler. Confirm mark cursor and mark movement behavior still works.
11. Close and reopen the Timed Sequence Editor. Confirm `View > Lock Ruler Height` retains the prior checked state.
12. Disable `View > Lock Ruler Height`, close and reopen the editor, and confirm the item remains unchecked and ruler resizing works again.

The core acceptance criterion is: enabling `Lock Ruler Height` prevents the resize cursor, drag resize, and double-click reset for the ruler height while preserving all non-resize ruler interactions and profile persistence.

## Testing Guidance

Automated testing may be limited because the behavior is WinForms mouse interaction inside a user control. If practical, isolate the lock decision behind a small property or helper on `Ruler` and add unit coverage for the state transitions that can be tested without brittle UI automation.

Recommended automated coverage, if the existing test framework can instantiate the control safely:

- A new `Ruler` has the lock disabled by default.
- Setting the lock to enabled prevents the resize-zone cursor decision from returning or applying `Cursors.HSplit`.
- Setting the lock to enabled prevents height mutation through the resize path.
- Setting the lock back to disabled restores resize-zone behavior.

Manual validation remains required because the user-visible cursor and drag behavior are the important outcomes.

## Open Questions

No product questions remain open for the initial ExecPlan.

## Decision Log

- Decision: The setting defaults to off and uses a stable XML app setting key, proposed as `{Name}/LockRulerHeight`.
  Rationale: The user explicitly requested the option default to off and be saved with the other XML profile settings.
  Date/Author: 2026-07-20 / Codex from user request.

- Decision: The lock applies only to ruler height resize affordances and not to other ruler interactions.
  Rationale: The user asked to prevent accidental ruler resizing; preserving ruler clicks, time selection, and mark behavior keeps the change narrowly scoped.
  Date/Author: 2026-07-20 / Codex from source review.

- Decision: The specification blocks double-click height reset while locked unless product direction says otherwise.
  Rationale: Double-click reset changes the ruler height through the same resize affordance area, so allowing it would weaken the meaning of a height lock.
  Date/Author: 2026-07-20 / Codex from source review; confirmed 2026-07-20 / user direction recorded by Codex.

- Decision: Place `Lock Ruler Height` directly below `Full Waveform` in the View menu.
  Rationale: The user requested this placement, and it groups the ruler lock with another timeline display option rather than with row height controls.
  Date/Author: 2026-07-20 / user direction recorded by Codex.

- Decision: If the lock is enabled while a ruler resize drag is active, stop changing the height immediately using simple guard logic only.
  Rationale: The user approved immediate stop behavior but noted the scenario is unlikely and should not drive excessive implementation complexity.
  Date/Author: 2026-07-20 / user direction recorded by Codex.

## Notes for JIRA

Use this concise JIRA summary:

    Add a persisted Timed Sequence Editor View menu option to lock ruler height and prevent accidental ruler resize.

Use this acceptance text:

    The View menu contains `Lock Ruler Height`, defaulting to unchecked for missing profile settings. When unchecked, the ruler keeps current resize behavior, including the split cursor, drag resize, double-click reset, and saved ruler height. When checked, hovering over the ruler bottom edge does not show the resize cursor, dragging there does not change ruler height, and double-click reset is blocked, while ruler clicks, time-range drags, and mark interactions continue to work. The setting persists in the XML profile between editor sessions.

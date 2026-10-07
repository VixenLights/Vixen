# VIX-3948 Lock Waveform Height Specification

## Overview

VIX-3948 adds a Timed Sequence Editor preference that prevents accidental waveform height changes. Today the user can resize the audio waveform bar by hovering near the bottom edge of the waveform, getting the horizontal split cursor, and dragging vertically. The requested improvement adds a `View` menu option named `Lock Waveform Height`. The option defaults to off, so existing waveform resize behavior remains unchanged unless the user enables the lock.

This document is the requirements and design specification for implementation. The implementation ExecPlan must follow `.agents/PLANS.md`, must be saved under `docs/plans/`, and should include steps to update the existing JIRA issue with the refined specification, acceptance criteria, and testing guidance from this document.

JIRA issue: `VIX-3948`

## Repository Context

The waveform bar is implemented in `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`. The current resize behavior is entirely local to this control. `Waveform.OnMouseMove` changes the cursor to `Cursors.HSplit` when the pointer is near the bottom edge of the waveform. While the left mouse button is held and the current cursor is `Cursors.HSplit`, `Waveform.OnMouseMove` changes `Height` when the pointer is lower than the waveform minimum height. `Waveform.OnMouseDoubleClick` resets the waveform height to 50 when the cursor is `Cursors.HSplit`.

The parent timeline control is implemented in `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`. It creates the public `waveform` instance, docks it at the top of the timeline panel, and exposes related timeline surface properties. The Timed Sequence Editor host is implemented in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`, `TimedSequenceEditorForm.Designer.cs`, and `TimedSequenceEditorForm_Menu.cs`.

Timed Sequence Editor profile settings are persisted through `XMLProfileSettings` app settings. `TimedSequenceEditorForm.cs` already loads and saves the existing `WaveFormHeight` setting next to related UI settings such as `FullWaveform`, `RulerHeight`, `LockRulerHeight`, `ResizeIndicatorEnabled`, `CadStyleSelectionBox`, and `LegacyCursorActiveRow`. The new waveform lock setting should be saved in this same profile settings area.

VIX-3945 added the matching `Lock Ruler Height` behavior. VIX-3948 should follow the same user experience, persistence pattern, menu behavior, and implementation style, while applying only to the waveform bar.

## Goals

The implementation must:

- Add a checked `View` menu item named `Lock Waveform Height`.
- Place `Lock Waveform Height` directly below `Lock Ruler Height`.
- Default the menu item and setting to unchecked/off when the profile setting does not exist.
- Persist the setting in the XML profile with the other Timed Sequence Editor app settings.
- When the setting is off, preserve current waveform resize behavior, including the horizontal split cursor, drag-to-resize behavior, minimum practical resize threshold, double-click reset behavior, and saved `WaveFormHeight`.
- When the setting is on, prevent the user from resizing the waveform by dragging the waveform bottom edge.
- When the setting is on, prevent the waveform resize cursor indicator from appearing.
- Keep existing waveform behaviors unrelated to height resizing, including waveform painting, cursor drawing, mark alignment drawing, full/half waveform display, audio assignment, sample generation, and visibility behavior.

## Non-Goals

The implementation must not:

- Change ruler height behavior or the existing `Lock Ruler Height` setting.
- Change grid row height resizing or row height reset behavior.
- Change the `Full Waveform` display mode or the `WaveformStyle` rendering behavior.
- Change the existing effect resize indicator preference in the View/Edit menus; that preference controls timeline effect resize guides, not waveform height.
- Remove or ignore the existing saved `WaveFormHeight`. Locking the height should prevent interactive user resizing, not force the height back to a default.
- Mark the sequence modified, create undo history, or change sequence file contents.
- Add a toolbar button or keyboard shortcut unless a separate product decision is made.

## Target User Experience

The `View` menu contains a new checked menu item named `Lock Waveform Height`, placed directly below `Lock Ruler Height`. With the current VIX-3945 menu placement, the timeline display items should read `Full Waveform`, `Lock Ruler Height`, then `Lock Waveform Height`. A fresh profile or a profile missing the setting opens with the item unchecked.

With `Lock Waveform Height` unchecked, the waveform behaves exactly as it does today. Moving the mouse near the bottom of the waveform shows the horizontal split cursor, dragging changes the waveform height, and double-clicking the resize edge resets the height to 50 pixels. Closing and reopening the editor preserves the resulting `WaveFormHeight`.

With `Lock Waveform Height` checked, moving the mouse near the bottom of the waveform does not show the horizontal split cursor. The cursor remains the appropriate non-resize cursor for the waveform area, currently the hand cursor. Pressing and dragging near the bottom edge does not change the waveform height. Double-clicking near the bottom edge does not reset the waveform height merely because the lock is enabled.

Toggling the menu item applies immediately to the open editor. If the waveform is already at a custom height, enabling the lock keeps that current height and prevents further interactive resize. Disabling the lock restores the current resize affordance without changing the height.

The waveform lock and ruler lock are independent. A user can lock either one, both, or neither. Locking the waveform must not affect ruler resize behavior, and locking the ruler must not affect waveform resize behavior.

## Detailed Requirements

Add a boolean property to the timeline/waveform surface, with a name such as `LockWaveformHeight` or `WaveformHeightLocked`. The property must ultimately be readable by `Waveform`. Because `Waveform` is a public field on `TimelineControl`, the smallest implementation is a public boolean property on `Waveform` and an optional forwarding property on `TimelineControl` for cleaner Timed Sequence Editor code. If a public or protected C# API is added or changed, update XML documentation in the same change and use the project `csharp-docs` skill before implementation.

The lock property default must be `false`. A missing XML profile setting must behave the same as `false`.

In `Waveform.OnMouseMove`, the unlocked path must continue to show `Cursors.HSplit` when `e.Location.Y <= Height - 1 && e.Location.Y >= Height - 6`. When the lock is enabled, that specific resize-zone branch must not set `Cursors.HSplit`. The code should fall through to the normal non-resize cursor behavior, currently `Cursors.Hand`.

In `Waveform.OnMouseMove` while the left button is held, the resize path must not change `Height` while the lock is enabled. This is a defensive requirement for cases where the lock is toggled while a resize drag is in progress. This edge case is not expected during normal use, so do not add excessive state-management logic for it. A simple guard that stops height changes immediately is sufficient.

In `Waveform.OnMouseDoubleClick`, double-click reset to 50 must be disabled while the lock is enabled. This prevents a locked waveform from changing height through a second resize affordance.

The Timed Sequence Editor must load the setting near the existing menu and timeline settings in `TimedSequenceEditorForm.cs`, for example:

    lockWaveformHeightToolStripMenuItem.Checked = TimelineControl.LockWaveformHeight =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockWaveformHeight", Name), false);

The exact property name may differ, but the XML key should be stable. Use `{Name}/LockWaveformHeight` unless implementation discovers an existing naming convention that is a better fit.

The Timed Sequence Editor must save the setting near the existing app setting writes in `TimedSequenceEditorForm.cs`, for example:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockWaveformHeight", Name), lockWaveformHeightToolStripMenuItem.Checked);

The menu click or checked-changed handler in `TimedSequenceEditorForm_Menu.cs` must synchronize the menu item checked state to the timeline/waveform lock property immediately.

## Implementation Guidance

Start by reading:

- `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`
- `docs/sequencer/vix-3945-lock-ruler-height.md`

The likely implementation path is:

1. Add the lock property to `Waveform`, defaulting to `false`.
2. Optionally add a forwarding property to `TimelineControl` so the editor does not reach through the public `waveform` field for this preference.
3. Update `Waveform.OnMouseMove` and `Waveform.OnMouseDoubleClick` so waveform-height resize affordances and height changes are disabled when the lock is enabled.
4. Add `lockWaveformHeightToolStripMenuItem` to the View menu in `TimedSequenceEditorForm.Designer.cs`, directly below `lockRulerHeightToolStripMenuItem`.
5. Add the menu handler in `TimedSequenceEditorForm_Menu.cs`.
6. Load and save `{Name}/LockWaveformHeight` in `TimedSequenceEditorForm.cs` near `WaveFormHeight` and `LockRulerHeight`.

Avoid changing waveform painting or sample generation. The lock is specifically about height resizing, not whether the waveform is visible, how it renders audio, or whether full waveform mode is enabled.

Avoid using `Enabled = false` on the whole waveform or intercepting all waveform mouse input. The lock is specifically about height resizing, not the waveform as a visual timeline surface.

## ExecPlan Path

Create the implementation ExecPlan under `docs/plans/sequencer/vix-3948-lock-waveform-height.md`. The ExecPlan must be self-contained per `.agents/PLANS.md` and should include:

- Purpose and user-visible behavior.
- A context section explaining the `Waveform`, `TimelineControl`, and Timed Sequence Editor settings files.
- Milestones for control-level locking, menu and persistence wiring, and validation.
- A decision log entry for the chosen public property name and XML setting key.
- Manual validation steps for locked and unlocked behavior.
- Automated test guidance or a clear explanation if the WinForms interaction is only manually validated.

## Validation and Acceptance

Run the focused test project from the repository root after implementation:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore

If full tests are too slow during development, run a focused compile or targeted test first, then run the full command before final completion.

Manual acceptance should be performed in the Timed Sequence Editor:

1. Open a sequence with a fresh or missing `LockWaveformHeight` profile setting. Confirm `View > Lock Waveform Height` is unchecked and appears directly below `Lock Ruler Height`.
2. With the item unchecked, hover near the bottom edge of the waveform. Confirm the horizontal split cursor appears.
3. With the item unchecked, drag the waveform bottom edge. Confirm the waveform height changes.
4. With the item unchecked, double-click the waveform resize edge. Confirm the waveform height resets to 50 pixels.
5. Enable `View > Lock Waveform Height`. Confirm the waveform height does not change when the menu item is toggled.
6. With the item checked, hover near the bottom edge of the waveform. Confirm the horizontal split cursor does not appear.
7. With the item checked, drag near the bottom edge of the waveform. Confirm the waveform height does not change.
8. With the item checked, double-click near the bottom edge of the waveform. Confirm the waveform height does not reset.
9. With `Lock Waveform Height` checked and `Lock Ruler Height` unchecked, confirm ruler height resizing still works.
10. With `Lock Waveform Height` unchecked and `Lock Ruler Height` checked, confirm waveform height resizing still works.
11. With the item checked, confirm the waveform still renders audio, cursor position, and mark alignment indicators as before.
12. Toggle `View > Full Waveform` while `Lock Waveform Height` is checked. Confirm full/half waveform rendering changes without changing the waveform height lock state.
13. Close and reopen the Timed Sequence Editor. Confirm `View > Lock Waveform Height` retains the prior checked state.
14. Disable `View > Lock Waveform Height`, close and reopen the editor, and confirm the item remains unchecked and waveform resizing works again.

The core acceptance criterion is: enabling `Lock Waveform Height` prevents the resize cursor, drag resize, and double-click reset for the waveform height while preserving all non-resize waveform behavior and profile persistence.

## Testing Guidance

Automated testing may be limited because the behavior is WinForms mouse interaction inside a user control. If practical, isolate the lock decision behind a small property or helper on `Waveform` and add unit coverage for the state transitions that can be tested without brittle UI automation.

Recommended automated coverage, if the existing test framework can instantiate the control safely:

- A new `Waveform` has the lock disabled by default.
- Setting the lock to enabled prevents the resize-zone cursor decision from returning or applying `Cursors.HSplit`.
- Setting the lock to enabled prevents height mutation through the resize path.
- Setting the lock back to disabled restores resize-zone behavior.
- The timeline-level forwarding property, if added, reads and writes the underlying waveform lock state.

Manual validation remains required because the user-visible cursor and drag behavior are the important outcomes.

## Open Questions

No product questions remain open for the initial ExecPlan.

## Decision Log

- Decision: The setting defaults to off and uses a stable XML app setting key, proposed as `{Name}/LockWaveformHeight`.
  Rationale: The user requested behavior matching VIX-3945, whose lock setting defaults off and is saved with the other XML profile settings.
  Date/Author: 2026-07-21 / Codex from user request.

- Decision: The lock applies only to waveform height resize affordances and not to rendering or visibility behavior.
  Rationale: The user asked to prevent accidental waveform resizing; preserving waveform display behavior keeps the change narrowly scoped.
  Date/Author: 2026-07-21 / Codex from source review.

- Decision: The waveform lock is independent from the ruler lock.
  Rationale: The requested menu item lands below `Lock Ruler Height` and should act like that feature, but the controls and saved heights are separate.
  Date/Author: 2026-07-21 / Codex from user request and source review.

- Decision: The specification blocks double-click height reset while locked unless product direction says otherwise.
  Rationale: Double-click reset changes the waveform height through the same resize edge affordance, so allowing it would weaken the meaning of a height lock.
  Date/Author: 2026-07-21 / Codex from source review.

- Decision: Place `Lock Waveform Height` directly below `Lock Ruler Height` in the View menu.
  Rationale: The user explicitly requested this placement, and it groups the waveform lock with the matching ruler lock.
  Date/Author: 2026-07-21 / user direction recorded by Codex.

- Decision: If the lock is enabled while a waveform resize drag is active, stop changing the height immediately using simple guard logic only.
  Rationale: This matches the VIX-3945 edge-case behavior and avoids extra state management for an unlikely interaction.
  Date/Author: 2026-07-21 / Codex from VIX-3945 alignment.

## Notes for JIRA

Use this concise JIRA summary:

    Add a persisted Timed Sequence Editor View menu option to lock waveform height and prevent accidental waveform resize.

Use this acceptance text:

    The View menu contains `Lock Waveform Height` directly below `Lock Ruler Height`, defaulting to unchecked for missing profile settings. When unchecked, the waveform keeps current resize behavior, including the split cursor, drag resize, double-click reset, and saved waveform height. When checked, hovering over the waveform bottom edge does not show the resize cursor, dragging there does not change waveform height, and double-click reset is blocked, while waveform rendering, cursor drawing, mark alignment drawing, and full/half waveform display continue to work. The setting persists in the XML profile between editor sessions.

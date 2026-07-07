# Implement VIX-2221 Active Row Arrow-Key Navigation

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. This document is self-contained so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users work in the Timed Sequence Editor by choosing an active timeline row, then drawing, selecting, or pasting effects against that row. Today the active row is primarily changed by mouse interaction. After this change, a user can press the Up and Down arrow keys while the Timed Sequence Editor has focus and move the active row to the previous or next visible timeline row without reaching for the mouse.

The visible result is in `src/Vixen.Modules/Editor/TimedSequenceEditor`. Click a timeline row to make it active, press Down, and the active-row highlight moves to the next visible row. Press Up, and it moves back to the previous visible row. The feature should also work when focus is inside the hosted WPF Layer Editor because `LayerEditor` already forwards WPF key events to the Timed Sequence Editor hotkey bridge.

## Progress

- [x] (2026-07-07 13:35Z) Read `.agents/PLANS.md`, `TimedSequenceEditorForm_Hotkeys.cs`, `LayerEditor.cs`, and the timeline row/active-row implementation in `src/Vixen.Common/Controls/TimeLineControl`.
- [x] (2026-07-07 13:45Z) Created this initial ExecPlan with implementation milestones, validation steps, and open clarification questions.
- [x] (2026-07-07 13:55Z) Confirmed product choices: use only plain Up/Down, do nothing when no row is active, auto-scroll the newly active row into view, do not wrap at row edges, and allow during playback unless implementation reveals a conflict.
- [x] (2026-07-07 13:51Z) Implemented `MoveActiveRow(int direction)` in `Grid` and exposed it through `TimelineControl`. The method requires an existing visible active row, moves by one visible row in the requested direction, clamps at row edges, scrolls the target row into view, and leaves row/effect selection unchanged.
- [x] (2026-07-07 13:51Z) Wired unmodified Up and Down keys in `TimedSequenceEditorForm_Hotkeys.cs` to `TimelineControl.MoveActiveRow(-1)` and `TimelineControl.MoveActiveRow(1)`.
- [x] (2026-07-07 13:51Z) Added focused automated tests in `src/Vixen.Tests/Sequencer/TimelineActiveRowNavigationTests.cs`. `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineActiveRowNavigation --no-restore` passed with 7 tests. `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore` passed with 170 tests.
- [x] (2026-07-07 14:05Z) Fixed WinForms command-key preprocessing for plain Up/Down by routing unmodified arrow keys through `ProcessCmdKey`. User validation showed plain arrows did not reach `OnKeyDown` without this, while modified arrows did.
- [ ] Manually verify the behavior in the Timed Sequence Editor.
- [x] (2026-07-07 13:52Z) Updated JIRA issue VIX-2221 with finalized requirements, acceptance criteria, implementation notes, and testing procedures/results in comment `40128`.

## Surprises & Discoveries

- Observation: The active row is not the same thing as a selected row. A selected row has `Row.Selected = true` and usually means all effects in the row are selected. The active row has `Row.Active = true` and is used as a target row for actions such as moving or pasting effects.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Row.cs` documents `Active` as "not the same as a selected row(s)" and `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` uses `TimelineControl.ActiveRow ?? TimelineControl.SelectedRow` for effect move targets.

- Observation: Active-row state is currently changed by mouse code in `Grid_Mouse.cs`, not by keyboard code.
  Evidence: `Grid_Mouse.cs` calls `ClearActiveRows()` and then sets `row.Active = true` during element clicks, background clicks, and context-click handling. `TimedSequenceEditorForm_Hotkeys.cs` has no Up or Down key cases.

- Observation: The WPF Layer Editor already forwards keyboard events to the Timed Sequence Editor hotkey path.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs` attaches `host.Child.KeyDown += Form_LayerKeyDown`, and `Form_LayerKeyDown` publishes `System.Windows.Input.KeyEventArgs` to the `"KeydownSWI"` broadcast. `TimedSequenceEditorForm_Hotkeys.cs` converts WPF keys to WinForms `Keys` in `HandleQuickKeySWI`.

- Observation: Visible row order is already available and should be used instead of all row order.
  Evidence: `Grid.VisibleRows` returns `Rows.Where(x => x.Visible).ToList()` and is used by existing paste and movement logic to respect collapsed or hidden rows.

- Observation: Direct automated tests can instantiate `TimelineControl`, add visible rows, and exercise active-row navigation through the public wrapper without launching the full Timed Sequence Editor.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineActiveRowNavigation --no-restore` passed on 2026-07-07 with `Passed: 7, Failed: 0`.

- Observation: The TimedSequenceEditor project rebuild is still blocked by local/environment issues unrelated to this change.
  Evidence: `dotnet msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj -t:Rebuild -p:Configuration=Debug -p:UseSharedCompilation=false` failed before editor compilation on missing C++ imports for `Microsoft.Cpp.Default.props` in `QMLibrary.vcxproj` and `LiquidLiquidFunWrapper.vcxproj`. Retrying with `-p:BuildProjectReferences=false` reached pre-existing MarkDocker XAML converter errors for `ColorToSolidBrushConverter` in WPFCommon.

- Observation: VIX-2221 was updated in JIRA with the final behavior and validation notes.
  Evidence: Added JIRA comment `40128` on 2026-07-07 with finalized requirements, acceptance criteria, implementation notes, focused test results, full test results, and local TimedSequenceEditor build blocker notes.

- Observation: Plain Up/Down arrow keys did not reach `TimedSequenceEditorForm.OnKeyDown`, while modified arrow keys did.
  Evidence: User placed breakpoints in `TimedSequenceEditorForm_Hotkeys.cs` and observed that plain arrow keys were ignored unless a modifier was present. WinForms treats plain arrows as command/navigation keys, so `TimedSequenceEditorForm.ProcessCmdKey` now intercepts unmodified Up/Down and routes them through the existing hotkey handler.

## Decision Log

- Decision: Implement active-row navigation in the shared timeline control layer, then call that API from the Timed Sequence Editor hotkey file.
  Rationale: The active-row flag and visible row collection belong to `Common.Controls.Timeline.Grid` and `TimelineControl`; keeping navigation there avoids duplicating row-selection rules inside the editor form and makes the behavior available to other timeline hosts if needed.
  Date/Author: 2026-07-07 / Codex

- Decision: Navigate through `VisibleRows`, not `Rows`.
  Rationale: The user can collapse tree rows or hide rows with tags. Arrow navigation should match what the user can see on screen and should not move the active row to a collapsed or hidden row.
  Date/Author: 2026-07-07 / Codex

- Decision: Treat this as active-row movement only, not row selection.
  Rationale: Existing code intentionally separates `Row.Active` from `Row.Selected`. Up and Down should move the target row highlight without selecting every effect on the row or changing the selected effects.
  Date/Author: 2026-07-07 / Codex

- Decision: Do nothing when no row is currently active.
  Rationale: The user explicitly chose this behavior. Although existing paste/menu code sometimes falls back to selected or top-visible rows, arrow-key navigation should only move an already-established active row and should not infer one.
  Date/Author: 2026-07-07 / user, recorded by Codex

- Decision: Handle only unmodified Up and Down arrow keys.
  Rationale: The user explicitly requested no modifiers. Ctrl, Shift, and Alt combinations should remain untouched so they do not conflict with existing or future shortcuts.
  Date/Author: 2026-07-07 / user, recorded by Codex

- Decision: Keep arrow navigation enabled during playback unless implementation reveals a concrete conflict.
  Rationale: The user approved playback-time navigation if it does not conflict. Active-row navigation changes an editing target highlight and does not by itself alter playback state.
  Date/Author: 2026-07-07 / user, recorded by Codex

## Outcomes & Retrospective

Implementation, automated validation, and JIRA update are complete. Active-row movement is implemented in the shared timeline control and wired into the Timed Sequence Editor hotkey path. Focused tests cover normal up/down movement, hidden-row skipping, first/last-row clamping, no-row behavior, and no-active-row behavior. Full `Vixen.Tests` also passes. Manual validation in the Timed Sequence Editor remains.

After initial user validation, plain Up/Down were found to bypass `OnKeyDown` because they are WinForms command/navigation keys. The hotkey file now also overrides `ProcessCmdKey` for unmodified Up/Down and delegates to the same `OnKeyDown` path, preserving the original modifier behavior.

## Open Questions

These questions were answered by the user on 2026-07-07 and are retained here as requirements.

1. Should Up and Down move the active row only when no modifiers are held? Answer: yes. Plain Up and plain Down move the active row. Ctrl, Shift, and Alt combinations remain available for existing or future shortcuts.

2. If there is no active row, what should the first Up or Down press do? Answer: do nothing. Do not use selected row, top visible row, or first visible row as a fallback for this feature.

3. Should arrow navigation scroll the timeline vertically to keep the newly active row visible? Answer: yes. When the target row is above the viewport, set `VerticalOffset` to the row's `DisplayTop`. When the target row is below the viewport, set `VerticalOffset` so the bottom of the target row is inside `VisibleHeight`.

4. Should navigation wrap from the last visible row to the first visible row, or stop at the edge? Answer: no wrapping. Stop at the first and last visible rows.

5. Should active-row navigation be disabled while playback is running? Answer: allow it during playback if that does not cause a conflict. If implementation reveals a concrete playback conflict, document it in `Surprises & Discoveries` and revisit the decision before adding a playback guard.

## Context and Orientation

The Timed Sequence Editor is a Windows desktop editor under `src/Vixen.Modules/Editor/TimedSequenceEditor`. It hosts a timeline control from `src/Vixen.Common/Controls/TimeLineControl`. A timeline row is represented by `Common.Controls.Timeline.Row`. A visible row is a row whose `Visible` property is `true`, meaning the row is not hidden by tree collapse or row filtering. The active row is the single row where `Row.Active` is `true`; it is drawn with the selection color in `Grid._drawRows`.

The main hotkey file is `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Hotkeys.cs`. `TimedSequenceEditorForm.OnKeyDown(KeyEventArgs e)` handles shortcut keys such as Home, End, PageUp, PageDown, Space, Left, Right, and row zoom. It currently has no `Keys.Up` or `Keys.Down` cases.

The hosted WPF Layer Editor is in `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs`. It creates an `ElementHost`, sets `host.Child = _layerEditorView`, and forwards WPF key down events by publishing `"KeydownSWI"`. `TimedSequenceEditorForm_Hotkeys.HandleQuickKeySWI` converts the WPF key to a WinForms `KeyEventArgs` and then calls `HandleQuickKeySWF`, which calls `OnKeyDown`. This means adding Up and Down handling to `OnKeyDown` should also cover focus inside the Layer Editor, unless a WPF child control marks the key handled before the event reaches `Form_LayerKeyDown`.

The shared timeline control exposes these relevant members:

- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`: `TimelineControl.ActiveRow`, `TimelineControl.SelectedRow`, `TimelineControl.VisibleRows`, `TimelineControl.TopVisibleRow`, `TimelineControl.VerticalOffset`, and `TimelineControl.VisibleHeight`.
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`: `Grid.ActiveRow`, `Grid.VisibleRows`, `Grid.TopVisibleRow`, `Grid.ClearActiveRows(Row leaveRowActive = null)`, and `Grid.VerticalOffset`.
- `src/Vixen.Common/Controls/TimeLineControl/Row.cs`: `Row.Active`, `Row.Selected`, `Row.Visible`, `Row.DisplayTop`, and `Row.Height`.

## Plan of Work

First, add active-row navigation to `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`. Add a method that accepts an integer direction, where negative means up and positive means down. The method should build a local list from `VisibleRows`; return `false` if there are no visible rows; find the current active row in that list; return `false` if there is no active row or if the active row is not currently visible; clamp the new index between zero and `visibleRows.Count - 1`; return `false` if the target row is already the active row; clear active rows while leaving the target active; set `targetRow.Active = true`; ensure the target row is visible by adjusting `VerticalOffset`; invalidate the grid; and return `true`.

Use a name that states behavior rather than implementation, such as `MoveActiveRow(int direction)`. If this method is public, add XML documentation in the same change. A suitable contract is:

    /// <summary>
    /// Moves the active row by the specified number of visible rows.
    /// </summary>
    /// <param name="direction">A negative value to move upward; a positive value to move downward.</param>
    /// <returns><see langword="true" /> if the active row changed; otherwise, <see langword="false" />.</returns>

Second, expose the behavior through `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`. Add a small wrapper method, also with XML documentation if public:

    public bool MoveActiveRow(int direction)
    {
        return grid.MoveActiveRow(direction);
    }

This keeps editor code from reaching into `TimelineControl.grid` for row-navigation behavior.

Third, update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Hotkeys.cs`. In `OnKeyDown`, add cases for `Keys.Up` and `Keys.Down`. Only handle them when `e.Control`, `e.Shift`, and `e.Alt` are all false. For plain Up, call `TimelineControl.MoveActiveRow(-1)`. For plain Down, call `TimelineControl.MoveActiveRow(1)`. If the call returns `true`, set `e.Handled = true` and `e.SuppressKeyPress = true` so child controls do not also process the arrow key. If modifiers are held, leave the event unhandled so existing control behavior is preserved.

Fourth, add tests if the existing test project can instantiate the timeline classes without a full WinForms application. Prefer a focused test file under `src/Vixen.Tests/Sequencer`, for example `TimelineActiveRowNavigationTests.cs`. The tests should create a `TimelineControl` or `Grid`, add rows, and assert that `MoveActiveRow(1)` and `MoveActiveRow(-1)` set only the expected row active. Include coverage for hidden rows by setting a middle row's `Visible` to `false` and verifying navigation skips it. Include coverage for clamping at the first and last visible row.

If direct unit tests are blocked because the controls require a UI handle or message loop, document the blocker in this ExecPlan under `Surprises & Discoveries`, then add the smallest practical test around any extracted pure helper. Do not add brittle UI automation as the first choice.

Finally, manually verify the behavior in a debug build. Open a timed sequence, click a row in the timeline grid to make it active, press Down repeatedly, and observe the active highlight move one visible row at a time and stop on the last visible row. Press Up repeatedly and observe the reverse. Collapse a parent row or hide rows with the existing row filtering/tag UI and verify navigation skips rows that are not visible. Focus the Layer Editor and press Up or Down to verify forwarded key handling still reaches the Timed Sequence Editor.

## Concrete Steps

Work from the repository root:

    cd C:\Dev\Vixen

Inspect the files before editing:

    rg -n "case Keys.Up|case Keys.Down|ActiveRow|ClearActiveRows|VisibleRows" src\Vixen.Modules\Editor\TimedSequenceEditor src\Vixen.Common\Controls\TimeLineControl -g "*.cs"

Implement the shared timeline API in:

    src\Vixen.Common\Controls\TimeLineControl\Grid.cs
    src\Vixen.Common\Controls\TimeLineControl\TimelineControl.cs

Implement the hotkey wiring in:

    src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditorForm_Hotkeys.cs

If tests are feasible, add them in:

    src\Vixen.Tests\Sequencer\TimelineActiveRowNavigationTests.cs

Run the focused tests first:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineActiveRowNavigation --no-restore

Then run the full test project if the environment has already restored dependencies:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

For compile validation when tests are blocked, run:

    dotnet msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj -t:Rebuild -p:Configuration=Debug -p:UseSharedCompilation=false

Expected successful test output should include zero failures. The exact count may vary as tests are added, but the summary should resemble:

    Passed!  - Failed: 0, Passed: <N>, Skipped: 0, Total: <N>

## Validation and Acceptance

Acceptance is user-visible behavior, not only compilation.

After implementation, a user can open the Timed Sequence Editor, click a visible timeline row, and press Down. The active-row highlight moves to the next visible row. Pressing Up moves it to the previous visible row. Repeated Up at the first visible row and repeated Down at the last visible row do not wrap and do not throw errors. The selected effects and selected rows do not change merely because the active row moved.

When some rows are collapsed or hidden, Up and Down move through only the rows currently visible on screen. Navigation should not move to a collapsed child or a hidden row.

When the active row moves beyond the current viewport, the timeline scrolls vertically just enough to keep the new active row visible.

When focus is inside the Layer Editor, plain Up and plain Down still trigger the same behavior through the existing WPF-to-WinForms quick-key bridge, unless a specific child control consumes the key for its own editing behavior. If a child control consumes the key, record that limitation and decide whether `PreviewKeyDown` handling is necessary.

Automated tests, if feasible, should prove:

- Moving down from row 1 activates row 2 and clears row 1's active flag.
- Moving up from row 2 activates row 1 and clears row 2's active flag.
- Hidden or collapsed rows are skipped because navigation uses `VisibleRows`.
- Movement clamps at the first and last visible row.
- Calling the method when there are no rows returns `false` and does not throw.
- Calling the method when rows exist but no row is active returns `false` and does not activate a fallback row.

## JIRA Update

After implementation and validation, update JIRA issue VIX-2221 so the ticket reflects the final behavior delivered in code. The update should be made after any implementation discoveries are recorded in this ExecPlan, because the JIRA text should match the tested behavior rather than the initial request.

Use this finalized requirements summary:

- In the Timed Sequence Editor, plain Up and Down arrow keys move the current active timeline row to the previous or next visible row.
- Modifier combinations such as Ctrl+Up, Shift+Down, Alt+Up, and any other modified Up/Down key press are not handled by this feature.
- If no row is currently active, Up and Down do nothing and do not infer an active row from selection or viewport position.
- Navigation uses visible row order and skips collapsed or hidden rows.
- Navigation stops at the first and last visible row and does not wrap.
- When the active row moves outside the current vertical viewport, the timeline scrolls to keep the newly active row visible.
- The behavior remains available during playback unless implementation uncovers a concrete conflict; if a conflict is found, document the final playback behavior in both this plan and the JIRA issue.
- Focus inside the hosted WPF Layer Editor should continue to route plain Up/Down through the existing quick-key bridge when the focused child control does not consume the key first.

Use these acceptance criteria:

- Given a timed sequence with an active visible row that is not the last visible row, when the user presses plain Down, then the active-row highlight moves to the next visible row and no row selection or effect selection is changed.
- Given a timed sequence with an active visible row that is not the first visible row, when the user presses plain Up, then the active-row highlight moves to the previous visible row and no row selection or effect selection is changed.
- Given collapsed or hidden rows between two visible rows, when the user presses Up or Down, then navigation skips rows that are not visible.
- Given the first visible row is active, when the user presses Up, then the active row remains unchanged and navigation does not wrap.
- Given the last visible row is active, when the user presses Down, then the active row remains unchanged and navigation does not wrap.
- Given no row is active, when the user presses Up or Down, then no row becomes active.
- Given the target row is outside the vertical viewport, when the user presses Up or Down, then the timeline scrolls vertically so the newly active row is visible.
- Given playback is running, when the user presses plain Up or Down and no implementation conflict has been found, then active-row navigation still works without changing playback state.
- Given focus is in the hosted Layer Editor and the focused WPF control does not consume arrow keys for its own editing behavior, when the user presses plain Up or Down, then the Timed Sequence Editor receives the key through the existing quick-key bridge and moves the active row.

Use these testing procedures:

- Run the focused automated tests added for active-row navigation, if available:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineActiveRowNavigation --no-restore

- Run the broader test project when the local environment has restored dependencies:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

- If automated tests are blocked by WinForms control instantiation or local dependency issues, record the blocker in the JIRA update and include the compile command that was run instead:

    dotnet msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj -t:Rebuild -p:Configuration=Debug -p:UseSharedCompilation=false

- Manually verify in the Timed Sequence Editor by activating a row, pressing Up and Down across normal rows, collapsed or hidden rows, first and last visible rows, an off-screen target row, playback, and focus inside the Layer Editor.

## Idempotence and Recovery

The implementation is additive and can be repeated safely. If a method already exists because another contributor implemented it first, inspect that method and adapt the hotkey wiring to the existing API instead of adding a duplicate. If tests cannot instantiate the timeline control, do not force a fragile test by creating hidden application windows; document the blocker and rely on compile plus manual validation.

Do not change unrelated hotkeys, row selection behavior, layer editing behavior, or effect movement behavior. Do not reformat `TimedSequenceEditorForm_Hotkeys.cs`, `Grid.cs`, or `TimelineControl.cs` outside the touched methods.

## Artifacts and Notes

Relevant existing code excerpts:

    // TimedSequenceEditorForm_Hotkeys.cs
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IgnoreKeyDownEvents) return;
        switch (e.KeyCode)
        {
            case Keys.Home:
                ...
            case Keys.PageDown:
                ...
            // No Keys.Up or Keys.Down handling exists before this plan.
        }
    }

    // Grid.cs
    public Row ActiveRow
    {
        get { return Rows.FirstOrDefault(x => x.Active); }
    }

    public List<Row> VisibleRows
    {
        get
        {
            if (_visibleRowsDirty)
            {
                _visibleRows = Rows.Where(x => x.Visible).ToList();
                _visibleRowsDirty = false;
            }
            return _visibleRows;
        }
    }

    public void ClearActiveRows(Row leaveRowActive = null)
    {
        foreach (Row row in Rows)
        {
            if (row != leaveRowActive)
                row.Active = false;
        }
        Invalidate();
    }

## Interfaces and Dependencies

Use only existing project dependencies. This feature should not add NuGet packages, services, or new projects.

At the end of implementation, `Common.Controls.Timeline.Grid` should expose a method with this behavior:

    public bool MoveActiveRow(int direction)

The method returns `true` only when it changes which row is active. It uses visible row order, clears any previous active row, sets exactly one target row active, scrolls the grid if necessary, invalidates the control, and does not select rows or effects.

If there is no active row, the method returns `false` and leaves row state unchanged.

At the end of implementation, `Common.Controls.Timeline.TimelineControl` should expose:

    public bool MoveActiveRow(int direction)

The wrapper delegates to `grid.MoveActiveRow(direction)`.

At the end of implementation, `TimedSequenceEditorForm.OnKeyDown` in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Hotkeys.cs` should handle plain `Keys.Up` and plain `Keys.Down` by calling `TimelineControl.MoveActiveRow(-1)` and `TimelineControl.MoveActiveRow(1)` respectively.

## Revision Notes

- 2026-07-07 / Codex: Initial plan created after repository research. The plan records proposed defaults for unresolved product choices and identifies the implementation points in the timeline control and Timed Sequence Editor hotkey bridge.
- 2026-07-07 / Codex: Updated the plan with user-confirmed requirements: no modifier keys, no fallback when there is no active row, auto-scroll enabled, no wrapping, and playback allowed unless a conflict is found.
- 2026-07-07 / Codex: Added a JIRA Update section with final requirements, acceptance criteria, and testing procedures to copy into VIX-2221 after implementation and validation.
- 2026-07-07 / Codex: Recorded implementation completion, automated test evidence, and the TimedSequenceEditor build blockers encountered during validation.
- 2026-07-07 / Codex: Recorded the VIX-2221 JIRA update comment after posting finalized requirements, acceptance criteria, implementation notes, and validation results.
- 2026-07-07 / Codex: Recorded the follow-up `ProcessCmdKey` fix after user validation showed plain arrow keys did not reach `OnKeyDown`.

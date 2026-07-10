# Implement VIX-3936 Move Cursor To Selected Effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. The source requirements are in `docs/sequencer/vix-3936-move-cursor-to-selected-effect.md`; this ExecPlan repeats the required context so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users often select an effect in the Timed Sequence Editor and then perform cursor-dependent work such as pasting, previewing, or editing around that effect. Today, selecting an effect changes the active row but does not move the timeline cursor to the selected effect; users must click empty grid space separately to place the cursor. After this change, selecting an effect moves the timeline cursor to the selected effect start while preserving active-row behavior by default. Users can opt into `Edit > Legacy Cursor / Active Row` to restore the old cursor and active-row behavior.

The visible result is in the Timed Sequence Editor. With `Legacy Cursor / Active Row` unchecked, left-click selection moves the cursor to the selected effect start; right-clicking an effect does not move the cursor under the new behavior; and right-clicking empty grid space keeps the current clicked-time cursor placement. With `Legacy Cursor / Active Row` unchecked, normal and CAD-style lasso multi-select keep the cursor and active row unchanged while the user draws the lasso, then update them together after mouse release when the final selected effect set is known. With the menu item checked, selecting effects, lasso-selecting effects, and right-clicking effects use the legacy behavior. The code still preserves the legacy overlapping-effects popup path if it is ever triggered, but that path is not expected to be reachable through ordinary grid interaction because overlapping effects are drawn stacked.

## Progress

- [x] (2026-07-09) Read `.agents/PLANS.md`, the VIX-3936 specification, the timeline grid selection code, the Timed Sequence Editor menu persistence code, and existing sequencer tests.
- [x] (2026-07-09) Created this initial ExecPlan with implementation milestones, validation steps, and the clarified right-click behavior.
- [x] (2026-07-09) Completed Milestone 1: initially added `Grid.MoveCursorToSelectedEffect`, the Timed Sequence Editor Edit menu toggle, click synchronization, and `XMLProfileSettings` load/save using `{Name}/MoveCursorToSelectedEffect`. This was later superseded by `Grid.LegacyCursorActiveRow` and `{Name}/LegacyCursorActiveRow`.
- [x] (2026-07-09) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineActiveRowNavigation --no-restore`; build succeeded and 7 tests passed. Warnings were pre-existing package/compiler warnings unrelated to Milestone 1.
- [x] (2026-07-09) Completed Milestone 2: added private cursor target helpers, moved the cursor from effect selection paths when new behavior is active, routed popup selection through `SelectElement`, and preserved/suppressed right-click cursor assignment according to the preference.
- [x] (2026-07-09) Re-ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineActiveRowNavigation --no-restore` after Milestone 2; build succeeded and 7 tests passed. Warnings were pre-existing package/compiler warnings unrelated to VIX-3936.
- [x] (2026-07-09) Completed Milestone 3: added focused `TimelineCursorSelection` tests for public selection APIs and ran focused/full validation successfully.
- [x] (2026-07-09) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineCursorSelection --no-restore`; build succeeded and 5 tests passed. Warnings were pre-existing package/compiler warnings unrelated to VIX-3936.
- [x] (2026-07-09) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore`; build succeeded and 198 tests passed. Warnings were pre-existing package/compiler warnings unrelated to VIX-3936.
- [x] (2026-07-09) Manually validated the Timed Sequence Editor behavior; the feature behaves correctly.
- [x] (2026-07-09) Updated Jira issue `VIX-3936` with implementation notes and validation evidence in comment `40164`.
- [x] (2026-07-10) Updated the source specification and this ExecPlan with additional normal and CAD-style lasso requirements: in non-legacy mode, do not move cursor or active row while drawing, move cursor after mouse-up to the earliest selected effect start, and set active row after mouse-up to the lasso-origin row.
- [x] (2026-07-10) Updated the source specification and this ExecPlan after user clarification that all new lasso cursor and active-row behavior is contained in the non-legacy behavior and must not apply when the legacy preference is enabled.
- [x] (2026-07-10) Implemented the new non-legacy lasso finalization behavior in `Grid_Mouse.cs` and a shared private grid helper. With `LegacyCursorActiveRow` enabled, the selection-box active-row path remains unchanged; with it disabled, selection-box mouse-down no longer clears active row and mouse-up sets the lasso-origin row active and moves the cursor to the earliest selected effect start.
- [x] (2026-07-10) Added focused `TimelineCursorSelection` tests for enabled lasso finalization and disabled no-op behavior.
- [x] (2026-07-10) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineCursorSelection --no-restore`; build succeeded and 7 tests passed. Warnings were pre-existing package/compiler warnings unrelated to VIX-3936.
- [x] (2026-07-10) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore`; build succeeded and 430 tests passed. Warnings were pre-existing package/compiler warnings unrelated to VIX-3936.
- [x] (2026-07-10) User confirmed the implemented lasso changes work as expected in the Timed Sequence Editor.
- [x] (2026-07-10) Updated implementation and documentation so the new behavior is the default and `Legacy Cursor / Active Row` is the checked menu option that restores legacy behavior.
- [x] (2026-07-10) Re-ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineCursorSelection --no-restore` after inverting the preference; build succeeded and 7 tests passed. Warnings were pre-existing package/compiler warnings unrelated to VIX-3936.
- [x] (2026-07-10) Re-ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore` after adding a non-parallel timeline control test collection; build succeeded and 430 tests passed. Warnings were pre-existing package/compiler warnings unrelated to VIX-3936.
- [ ] Update Jira with the added lasso behavior and validation evidence.

## Surprises & Discoveries

- Observation: Right-click currently sets the active row and, when `ClickingGridSetsCursor` is true, unconditionally assigns `CursorPosition = PixelsToTime(gridLocation.X)` in `Grid_Mouse.cs`.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs`, `OnMouseUp`, right-button branch.

- Observation: The overlapping-effects popup suppresses automatic grid selection, then later selects the chosen effect through the public `TimelineControl.SelectElement(tse)` path.
  Evidence: `TimedSequenceEditorForm.timelineControl_ElementsSelected` sets `e.AutomaticallyHandleSelection = false` and `contextMenuStripElementSelectionItem_Click` calls `TimelineControl.SelectElement(tse)`.

- Observation: The overlapping-effects popup path is likely obsolete for normal user interaction because overlapping effects are drawn in a stacked manner.
  Evidence: User clarification on 2026-07-09. Treat this path as legacy fallback behavior, not a required manual validation activity.

- Observation: Tests can instantiate `TimelineControl` directly and add rows without launching the full Timed Sequence Editor.
  Evidence: `src/Vixen.Tests/Sequencer/TimelineActiveRowNavigationTests.cs` creates `new TimelineControl(Guid.NewGuid())`, adds rows, and exercises public timeline APIs.

- Observation: Shift range selection calls `SelectElementsBetween(...)` during mouse-down, but the active row for Ctrl/Shift interactions is set later in the mouse-up branch.
  Evidence: `Grid_Mouse.cs` calls `SelectElementsBetween(m_lastSingleSelectedElementLocation, gridLocation)` in `OnMouseDown`, while the later `OnMouseUp` branch checks `ShiftPressed || CtrlPressed`, clears active rows, and sets `row.Active = true`.

- Observation: Automated right-click coverage would require direct WinForms mouse simulation or exposing additional grid internals.
  Evidence: Milestone 3 tests cover deterministic public selection APIs. Right-click behavior remains in the manual validation checklist to avoid screen-position-dependent tests.

- Observation: Disposing `TimelineControl` from the new tests can hit an existing parallel-test cleanup race in `cEventHelper`.
  Evidence: An initial full-suite run passed the cursor assertion but failed during `TimelineControl.Dispose(Boolean)` with `System.ArgumentException: An item with the same key has already been added. Key: Common.Controls.Timeline.TimeInfo`. The final tests avoid that dispose path and close only the `TimeLineGlobalStateManager` instance created for each test.

- Observation: Additional VIX-3936 requirements were requested after the initial implementation and validation were completed.
  Evidence: User direction on 2026-07-10 requires normal and CAD-style lasso multi-select, in non-legacy mode, to defer cursor and active-row changes until mouse-up, then move the cursor to the earliest selected effect start and set active row to the row where the lasso began. The user then clarified that these new features must not apply when the legacy preference is enabled.

- Observation: Normal and CAD-style lasso share one drag-selection path in `Grid_Mouse.cs`.
  Evidence: `MouseMove_DragSelect` always calls `selectElementsWithin(SelectionArea, true)`, and `selectElementsWithin` applies the CAD behavior internally from `aCadStyleSelectionBox`. A single finalization point in `MouseUp_DragSelect` covers both styles.

- Observation: The existing cursor selection tests were sensitive to removing timeline global managers during the full parallel test run.
  Evidence: The first full-suite run after adding lasso tests failed `SelectElement_WhenMoveCursorEnabled_MovesCursorToElementStart` with expected `00:00:12` and actual `00:00:00`, even though the focused cursor test run passed. `TimeLineGlobalStateManager` stores managers in a static `Dictionary<Guid, TimeLineGlobalStateManager>`. The tests now keep a local manager reference and do not call `CloseManager` during parallel test execution. The subsequent focused run passed 7 tests and the subsequent full run passed 430 tests.

- Observation: Timeline control tests still need to avoid parallel execution because `TimelineControl` creates static timeline managers.
  Evidence: After inverting the preference, one full-suite run failed `SelectElements_WhenLegacyCursorDisabled_MovesCursorToEarliestSelectedStart` with expected `00:00:07` and actual `00:00:00`, while the focused run passed. Adding `TimelineControlTestCollection` with `DisableParallelization = true` and assigning timeline control tests to it produced a focused pass with 7 tests and a full-suite pass with 430 tests.

## Decision Log

- Decision: Add this behavior as an opt-in grid property that defaults to `false`. This decision is superseded by the later `Legacy Cursor / Active Row` preference.
  Rationale: The original requirement was to preserve old behavior by default and persist a user preference from the Timed Sequence Editor menu. The user later requested the new behavior by default with a legacy opt-in.
  Date/Author: 2026-07-09 / Codex from VIX-3936 requirements; superseded 2026-07-10 / user direction recorded by Codex.

- Decision: Replace the opt-in new-behavior preference with an opt-in legacy preference named `Legacy Cursor / Active Row`.
  Rationale: The user requested the feature default to enabled and the menu item allow users to revert to legacy behavior.
  Date/Author: 2026-07-10 / user direction recorded by Codex.

- Decision: Implement ordinary selection cursor movement in the shared timeline grid rather than only in `TimedSequenceEditorForm`.
  Rationale: The grid already owns effect hit testing, selected element state, active-row updates, and the private `CursorPosition` wrapper. Keeping ordinary behavior there avoids editor-only duplication. The popup selection path remains a legacy fallback through `SelectElement`, not a primary implementation driver.
  Date/Author: 2026-07-09 / Codex.

- Decision: Keep the overlapping-effects popup cursor behavior as fallback behavior, but remove it from required manual acceptance.
  Rationale: The user clarified that normal row drawing stacks overlapping effects, so the popup likely cannot occur through ordinary UI use. If the old code path fires, it should still avoid moving the cursor until a concrete effect is chosen.
  Date/Author: 2026-07-09 / user clarification recorded by Codex.

- Decision: Preserve existing right-click behavior when legacy mode is enabled; when legacy mode is disabled, suppress cursor movement only for right-clicks on effects.
  Rationale: The user clarified that the no-move right-click rule belongs to the new cursor behavior. Empty-grid right-click remains a cursor-placement action.
  Date/Author: 2026-07-09 / user clarification recorded by Codex; updated 2026-07-10 / user direction recorded by Codex.

- Decision: Do not scroll horizontally when moving the cursor to an effect start.
  Rationale: VIX-3936 requests cursor state movement only. Changing `VisibleTimeStart` would introduce a second visual behavior that users did not request.
  Date/Author: 2026-07-09 / VIX-3936 requirements.

- Decision: Treat this as UI-only state.
  Rationale: Cursor movement does not change sequence content, so it must not call `SequenceModified()`, create undo history, or write to sequence model fields.
  Date/Author: 2026-07-09 / VIX-3936 requirements.

- Decision: For Ctrl and Shift mouse interactions, use `m_mouseDownElements` as the cursor action target and move the cursor in the mouse-up branch that already sets the active row.
  Rationale: This mirrors the active-row update point and avoids moving the cursor during range-selection calculation before the grid has finalized the interaction's active row.
  Date/Author: 2026-07-09 / Codex.

- Decision: For normal and CAD-style lasso multi-select, defer both cursor movement and active-row assignment until mouse-up only when legacy mode is disabled.
  Rationale: The user explicitly requested that neither the cursor nor active row change while the lasso is being drawn and that the cursor not move until the final effect selection is known. The user then clarified that these new behaviors are part of the new default behavior, while checked `Legacy Cursor / Active Row` restores old behavior.
  Date/Author: 2026-07-10 / user direction recorded by Codex; updated 2026-07-10 / user clarification recorded by Codex.

- Decision: Non-legacy normal and CAD-style lasso multi-select use the earliest selected effect start for cursor placement and the lasso-origin row for active row.
  Rationale: The earliest start gives a deterministic cursor target across all selected effects, while the lasso-origin row preserves the row context where the user began the lasso operation. When legacy mode is enabled, existing lasso behavior must be preserved.
  Date/Author: 2026-07-10 / user direction recorded by Codex; updated 2026-07-10 / user clarification recorded by Codex.

## Outcomes & Retrospective

Milestones 1, 2, and 3 are complete. The grid now exposes `LegacyCursorActiveRow`, the Timed Sequence Editor Edit menu has a persisted `Legacy Cursor / Active Row` toggle, and the toggle synchronizes to the grid property. Selection paths now move the cursor when legacy mode is disabled, the legacy popup selection path moves the cursor only after a concrete effect is selected if that path is ever triggered and legacy mode is disabled, and right-click cursor assignment is suppressed only for effect right-clicks while legacy mode is disabled.

Focused validation after Milestones 1 and 2 passed with 7 `TimelineActiveRowNavigation` tests. Milestone 3 added 5 `TimelineCursorSelection` tests and both focused and full `Vixen.Tests` validation pass. Manual Timed Sequence Editor validation confirms the behavior is correct. Jira issue `VIX-3936` has been updated with implementation and validation evidence.

On 2026-07-10, new lasso requirements were added after that completed implementation. The non-legacy lasso behavior is now implemented and covered by focused automated tests around lasso finalization. The user confirmed the implemented lasso changes work as expected in the Timed Sequence Editor. The remaining gap is a Jira update with the added validation evidence.

## Context and Orientation

The Timed Sequence Editor is the WinForms sequence-editing module under `src/Vixen.Modules/Editor/TimedSequenceEditor`. It hosts the reusable timeline control from `src/Vixen.Common/Controls/TimeLineControl`.

A timeline effect displayed in the grid is represented by `Common.Controls.Timeline.Element` in `src/Vixen.Common/Controls/TimeLineControl/Element.cs`. Its important properties for this work are `StartTime`, `Duration`, `EndTime`, `Selected`, and `Row`. `StartTime` is the left edge of the effect in timeline time.

A row is represented by `Common.Controls.Timeline.Row` in `src/Vixen.Common/Controls/TimeLineControl/Row.cs`. `Row.Active` means the row is the current editing target. This is not the same as `Row.Selected`; selecting a row usually selects the row's effects, while activating a row marks the target row.

The grid is `Common.Controls.Timeline.Grid`, split across `Grid.cs` and `Grid_Mouse.cs`. `Grid.cs` owns properties, events, selection helpers, and drawing. `Grid_Mouse.cs` owns mouse interaction. The grid has a private `CursorPosition` property that delegates to `TimeLineGlobalStateManager.Manager(instanceId).CursorPosition` and invalidates the grid. Setting that property is enough to move the visible timeline cursor in the grid, ruler, waveform, and editor status display because the global state manager raises cursor moved events.

The current effect click selection path is in `Grid_Mouse.cs`, `OnMouseDown`. When the user left-clicks one or more elements, the grid stores those elements in `m_mouseDownElements`. If the editor does not suppress automatic selection through `_ElementsSelected`, the grid sets `element.Selected = true`, sets `m_lastSingleSelectedElementLocation`, sets the row under the click active, and calls `_SelectionChanged()`.

The current mouse-up path in `Grid_Mouse.cs`, `OnMouseUp`, has another single-click case for a click on one of multiple selected elements. It clears selected elements, selects the first element under the click, calls `_SelectionChanged()`, and sets the row active. It also sets the row active for Ctrl/Shift clicks.

The lasso selection box is the drag rectangle the user draws in the grid to select multiple effects. Vixen has two lasso styles: the normal selection box and the CAD Style Selection Box controlled by the `CAD Style Selection Box` menu preference. Both styles must now share the same VIX-3936 finalization behavior, but only when `LegacyCursorActiveRow` is false. While the user is dragging either lasso with `LegacyCursorActiveRow` disabled, selected effects may visually update according to existing selection behavior, but `CursorPosition` and `Row.Active` must remain unchanged. On mouse-up, after the selected effects are final, the active row must become the row where the lasso drag began and, if at least one effect is selected by the lasso, the cursor must move to the smallest `StartTime` among the final selected effects. If `LegacyCursorActiveRow` is true, the new lasso finalization behavior must not run; preserve existing lasso cursor and active-row behavior exactly.

Right-click handling is also in `Grid_Mouse.cs`, `OnMouseUp`. It clears active rows, sets the row under the click active, assigns `CursorPosition = PixelsToTime(gridLocation.X)` when `ClickingGridSetsCursor` is true, and raises `_ContextSelected`. VIX-3936 must not change this behavior while the new preference is off. With the preference on, the cursor assignment must be skipped only when the right-click is on one or more effects.

The Timed Sequence Editor host still contains a legacy overlapping-effects popup path. In `TimedSequenceEditorForm.cs`, `timelineControl_ElementsSelected` shows `contextMenuStripElementSelection` when more than one element is under the cursor and sets `AutomaticallyHandleSelection = false`. When the user chooses one menu item, `contextMenuStripElementSelectionItem_Click` calls `TimelineControl.SelectElement(tse)`. Current row drawing stacks overlapping effects, so this path is not expected to be reachable through ordinary grid clicking. If the old path is triggered, it must move the cursor only after a concrete effect is chosen, not when the popup opens.

The menu and persistence pattern to copy is `CAD Style Selection Box`. The existing menu item is `cADStyleSelectionBoxToolStripMenuItem` in `TimedSequenceEditorForm.Designer.cs`. Startup loading is in `TimedSequenceEditorForm.cs` near `cADStyleSelectionBoxToolStripMenuItem.Checked = TimelineControl.grid.aCadStyleSelectionBox = xml.GetSetting(...)`. Close-time saving is in the same file near `xml.PutSetting(..., "CadStyleSelectionBox", cADStyleSelectionBoxToolStripMenuItem.Checked)`. The click handler is `cADStyleSelectionBoxToolStripMenuItem_Click` in `TimedSequenceEditorForm_Menu.cs`.

Because implementation adds or modifies public C# APIs such as `Grid.LegacyCursorActiveRow`, the implementer must use the project `csharp-docs` skill and add XML comments to those public or protected members in the same change. Use CRLF line endings and tabs for C# indentation.

## Plan of Work

Milestone 1 adds the persisted preference surface. Add a public Boolean property to `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`, near the other grid behavior properties, named `LegacyCursorActiveRow`. Document it with XML comments. The property defaults to `false`; an auto-property is sufficient because `false` is the default Boolean value. In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`, add a `ToolStripMenuItem` field named `legacyCursorActiveRowToolStripMenuItem` or an equivalent project-consistent name. Add it to the Edit menu near `cADStyleSelectionBoxToolStripMenuItem`, set `CheckOnClick = true`, set the text to `Legacy Cursor / Active Row`, and wire its click event. In `TimedSequenceEditorForm_Menu.cs`, add a click handler that assigns `TimelineControl.grid.LegacyCursorActiveRow = legacyCursorActiveRowToolStripMenuItem.Checked`. In `TimedSequenceEditorForm.cs`, load and save the setting beside the CAD setting using the stable app setting key `{Name}/LegacyCursorActiveRow`, defaulting to `false`.

Milestone 1 acceptance is observable without changing selection behavior: launching the editor with no setting shows `Legacy Cursor / Active Row` unchecked, toggling it changes `TimelineControl.grid.LegacyCursorActiveRow`, and after closing and reopening the editor the checked state persists.

Milestone 2 implements cursor movement for selection paths. In `Grid.cs`, add private helpers that centralize the cursor target rule. A clear shape is:

    private enum CursorSelectionTarget
    {
        EarliestSelected,
        LastActionElement
    }

    private void MoveCursorForSelectedEffects(IEnumerable<Element> actionElements, CursorSelectionTarget target)

This enum can stay private to `Grid`; do not make it public unless a public API truly needs it. The helper must return immediately when `LegacyCursorActiveRow` is true or when the action element list is null or empty. For `EarliestSelected`, use the selected elements after the selection operation and choose the minimum `StartTime`. For `LastActionElement`, use the last relevant element from the current action and choose that element's `StartTime`. The helper must set only `CursorPosition`; it must not change `VisibleTimeStart`, row active state, selected elements, undo state, or sequence modified state.

Call this helper in the existing selection paths after selected elements and active row have been updated. For a plain left-click selection where `_ElementsSelected(m_mouseDownElements)` returns true and elements are selected in `OnMouseDown`, move to the selected action element start. If exactly one element is selected, this is its `StartTime`; if multiple elements are selected by that operation without Ctrl/Shift, use the earliest selected start. For the mouse-up path that reduces multiple selected elements to `m_mouseDownElements.First()`, move to that first element start after selecting it and setting the row active. For Ctrl and Shift paths that set active row, move to the last selected effect associated with the latest click, using `m_mouseDownElements.LastOrDefault()` if that is the best available action element. If implementation discovers that `SelectElementsBetween` has a more accurate latest-click target, use that and record the detail in `Surprises & Discoveries`.

Update `Grid.SelectElement(Element element)` so public selection through `TimelineControl.SelectElement(tse)` moves the cursor to the chosen element start when legacy mode is disabled. This also covers the legacy popup selection path if it is ever triggered. This public helper currently only marks the element selected and raises `_SelectionChanged()`. Keep that behavior and add the cursor move after the selection change. Do not show or move the cursor when the legacy overlapping-effects popup opens, because `timelineControl_ElementsSelected` has not yet identified the chosen effect.

Update `Grid.SelectElements(IEnumerable<Element> elements)` only if there is a clear product path that selects multiple effects as one selection action and should honor the preference. If updated, materialize `elements` once to avoid repeated enumeration, select them, call `_SelectionChanged()`, then move to the earliest selected start. Preserve existing behavior if `elements` is empty.

Milestone 2 also updates right-click behavior in `Grid_Mouse.cs`. In the right-button branch of `OnMouseUp`, preserve the current code when `LegacyCursorActiveRow` is true. When the preference is false and `m_mouseDownElements` contains any effects, do not assign `CursorPosition`. When the preference is false and no effect is under the right-click, keep assigning `CursorPosition = PixelsToTime(gridLocation.X)` if `ClickingGridSetsCursor` is true. In all cases, keep clearing and setting active row, and keep raising `_ContextSelected`.

Milestone 2 acceptance is observable in the editor: with `Legacy Cursor / Active Row` unchecked, left-clicking an effect moves the cursor to the effect start and right-clicking an effect does not move the cursor. With the menu item checked, existing selection and right-click behavior remains unchanged. The legacy overlapping-effects popup does not need manual validation; if the path is triggered by tests or future code, choosing an effect through `SelectElement` moves the cursor after the choice when legacy mode is disabled.

Milestone 3 adds tests and validation. Add focused tests under `src/Vixen.Tests/Sequencer`, either in a new `TimelineCursorSelectionTests.cs` file or another clearly named file. Use the pattern from `TimelineActiveRowNavigationTests`: create `TimelineControl` with a known `Guid`, add rows, add `Element` instances with known `StartTime` and `Duration`, and inspect `TimeLineGlobalStateManager.Manager(id).CursorPosition`. Set `timelineControl.grid.LegacyCursorActiveRow = false` for new-behavior cases and `true` for legacy cases. Cover at least: `SelectElement` moves to the chosen element start when legacy mode is disabled; `SelectElement` does not move when legacy mode is enabled; `SelectElements` moves to the earliest selected start if that public method is updated; right-click-on-effect no-op should be covered by automated test only if it can be done cleanly without brittle WinForms mouse simulation. If right-click cannot be tested cleanly, record that in `Surprises & Discoveries` and rely on manual validation for that branch.

Milestone 3 validation requires running:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineCursorSelection --no-restore

Then run the full tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If either command fails for an environment reason unrelated to this change, capture the exact error and run the narrowest possible test command that demonstrates the new behavior. Do not leave failures unexplained.

Milestone 4 performs manual validation and Jira update. Open or create a Timed Sequence Editor sequence with at least two rows and several effects. Validate every acceptance criterion below. Do not spend time trying to trigger the legacy overlapping-effects popup through ordinary grid clicking; current drawing stacks overlapping effects, so that path is fallback behavior only. Then update Jira issue `VIX-3936` with a comment summarizing implementation decisions, automated test results, manual validation results, and any limitations.

Milestone 5 implements the added lasso behavior. In `Grid_Mouse.cs`, inspect the normal selection-box and CAD-style selection-box paths. Identify where the lasso drag starts, where the lasso selection mutates during drag, and where the lasso final selection is committed on mouse-up. Store the lasso-origin row at the start of the lasso operation if the current code does not already preserve it. When `LegacyCursorActiveRow` is true, do not change existing lasso cursor or active-row behavior. When `LegacyCursorActiveRow` is false, do not set active row or move cursor from the intermediate drag-update path. At mouse-up, after the final selected element set is known, set exactly the lasso-origin row active and leave other rows inactive. Then, if the final lasso-selected effect set is not empty, move `CursorPosition` to the minimum selected `StartTime`. Apply this through shared helper logic so normal lasso and CAD-style lasso cannot drift.

Milestone 5 acceptance is observable in the editor. With `Legacy Cursor / Active Row` unchecked, start a lasso on one row and drag across effects on multiple rows, including at least one effect on an earlier time than another selected effect. While the mouse button is down, the cursor and active-row highlight do not change. After mouse release, the active row becomes the row where the lasso began and the cursor moves to the earliest selected effect start. Repeat the same scenario with `CAD Style Selection Box` toggled on and observe identical behavior. With `Legacy Cursor / Active Row` checked, normal and CAD-style lasso retain existing cursor and active-row behavior.

## Concrete Steps

Work from repository root `C:\Dev\Vixen`.

First inspect the current files:

    rg -n "LegacyCursorActiveRow|cADStyleSelectionBox|CadStyleSelectionBox|CursorPosition|SelectElement|ContextSelected|ElementsSelected" src\Vixen.Common\Controls\TimeLineControl src\Vixen.Modules\Editor\TimedSequenceEditor src\Vixen.Tests\Sequencer

Add the grid preference in `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`. Place it near `ClickingGridSetsCursor` or the other grid behavior properties. Use XML documentation like:

    /// <summary>
    /// Gets or sets a value that indicates whether effect selection uses legacy cursor and active-row behavior.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if effect selection uses legacy cursor and active-row behavior; otherwise, <see langword="false" />.
    /// The default is <see langword="false" />.
    /// </value>
    public bool LegacyCursorActiveRow { get; set; }

Add the private helper and use it from the relevant `Grid_Mouse.cs` and `Grid.cs` selection paths. Keep helper names and enum names private unless they need to be called from `TimelineControl`. Prefer materializing enumerables with `ToList()` once before selecting or computing target times.

For the added lasso requirement, extend or add helper logic so non-legacy lasso finalization can be expressed as one operation: given a lasso-origin row and the final selected elements, clear other active rows, set the origin row active, and set `CursorPosition` to the minimum selected `StartTime`. Call this only when `LegacyCursorActiveRow` is false and only from the mouse-up/finalization path for normal and CAD-style lasso selection. Do not call it from the mouse-move or drag-update path. When `LegacyCursorActiveRow` is true, bypass this new helper and preserve existing lasso behavior.

Add the Edit menu item in `TimedSequenceEditorForm.Designer.cs`. Because this is designer-generated code, follow the style already present for `cADStyleSelectionBoxToolStripMenuItem`: declare the field near the existing menu item field, instantiate it near the top initializer block, add it to `editToolStripMenuItem.DropDownItems.AddRange`, set `CheckOnClick`, `Name`, `Size`, `Text`, and `Click`.

Load the setting in `TimedSequenceEditorForm.cs` near the CAD load line:

    legacyCursorActiveRowToolStripMenuItem.Checked = TimelineControl.grid.LegacyCursorActiveRow =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LegacyCursorActiveRow", Name), false);

Save the setting in `TimedSequenceEditorForm.cs` near the CAD save line:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LegacyCursorActiveRow", Name), legacyCursorActiveRowToolStripMenuItem.Checked);

Add the click handler in `TimedSequenceEditorForm_Menu.cs` near `cADStyleSelectionBoxToolStripMenuItem_Click`:

    private void legacyCursorActiveRowToolStripMenuItem_Click(object sender, EventArgs e)
    {
        TimelineControl.grid.LegacyCursorActiveRow = legacyCursorActiveRowToolStripMenuItem.Checked;
    }

Add tests in `src/Vixen.Tests/Sequencer/TimelineCursorSelectionTests.cs`. A minimal setup should look like the active-row navigation tests: create a `Guid id`, create `TimelineControl timelineControl = new TimelineControl(id)`, add rows, add `Element` objects with `StartTime` and `Duration` to rows with `row.AddElement(element)`, set `timelineControl.grid.LegacyCursorActiveRow`, call the public selection method, and assert `TimeLineGlobalStateManager.Manager(id).CursorPosition`.

Add lasso finalization tests if the implementation can expose the behavior without brittle WinForms mouse simulation. A good focused test creates at least two rows, marks final lasso-selected elements with different start times, calls the helper or public path used by lasso mouse-up, and asserts that the active row is the lasso-origin row while `CursorPosition` is the earliest selected start when `LegacyCursorActiveRow` is false. Add an enabled-legacy case that verifies the new lasso finalization behavior does not run and the existing active-row and cursor behavior is preserved. If direct tests would require screen-coordinate-dependent mouse simulation, record that limitation in `Surprises & Discoveries` and rely on manual validation for the UI path.

Run focused and full validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineCursorSelection --no-restore
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Before finishing, verify CRLF line endings for edited Markdown files and do not reformat unrelated code.

## Validation and Acceptance

Automated validation passes when the focused `TimelineCursorSelection` tests pass and the full `Vixen.Tests` suite passes, or when any unrelated environment failure is clearly documented with narrower passing evidence for this change.

Manual validation in the Timed Sequence Editor must show these behaviors:

1. With `Edit > Legacy Cursor / Active Row` unchecked, clicking an effect updates the active row and moves the timeline cursor to the effect start.
2. With the menu item checked, clicking a single effect updates the active row and does not move the cursor merely because of selection.
3. With the menu item unchecked, clicking an already-selected effect moves the cursor to that effect's start time again.
4. With the menu item unchecked, selecting multiple effects in one operation moves to the earliest selected effect start for general multi-select, and Ctrl/Shift selection moves to the latest clicked or last selected effect for that action.
5. With the menu item unchecked and `CAD Style Selection Box` unchecked, start a lasso on one row and drag across multiple rows. While drawing, neither cursor nor active row changes. On mouse release, the cursor moves to the earliest selected effect start and active row becomes the row where the lasso began.
6. With the menu item unchecked and `CAD Style Selection Box` checked, repeat the same lasso scenario. The timing, cursor target, and active-row target are identical to normal lasso.
7. With the menu item checked, repeat normal and CAD-style lasso scenarios. Existing lasso cursor and active-row behavior is unchanged; the new lasso-origin active-row rule and earliest-start cursor rule do not apply.
8. With the menu item checked, right-clicking an effect preserves today's behavior exactly. With the menu item unchecked, right-clicking an effect preserves active row and context menu behavior but does not move the cursor because of this feature. Right-clicking empty grid space still moves the cursor to the clicked time.
9. Moving the cursor to an effect start does not change `VisibleTimeStart` and does not horizontally scroll the grid.
10. Closing and reopening the Timed Sequence Editor preserves the menu item checked state. A missing setting defaults to unchecked.
11. Cursor movement caused by this feature does not call `SequenceModified()`, does not add undo history, and does not write sequence content.

Legacy fallback acceptance: if a test or unusual path triggers `contextMenuStripElementSelection`, opening that popup does not move the cursor; choosing one effect from the popup moves the cursor to the chosen effect's start time when legacy mode is disabled. This is not required for manual validation.

## Idempotence and Recovery

The code changes are additive and can be repeated safely if applied carefully. If the designer file becomes inconsistent, compare the new menu item against the existing `cADStyleSelectionBoxToolStripMenuItem` block and restore the same declaration, instantiation, `DropDownItems.AddRange` entry, property assignments, and click handler wiring.

If tests using direct WinForms mouse simulation become brittle, remove that brittle test and replace it with a unit test for the helper or a documented manual validation step. Do not add sleeps, real mouse movement, or screen-position-dependent assertions to automated tests.

If a public helper is added and later found unnecessary, prefer removing it before completion and keeping cursor target helpers private. Public API should stay minimal; any public or protected API that remains must have XML documentation updated in the same change.

If settings persistence behaves unexpectedly, inspect the neighboring CAD setting first because this feature should follow the same `XMLProfileSettings.SettingType.AppSettings` pattern and save timing.

## Artifacts and Notes

Important source requirements are in:

    docs\sequencer\vix-3936-move-cursor-to-selected-effect.md

The current right-click cursor assignment to preserve or suppress conditionally is:

    if (ClickingGridSetsCursor)
        CursorPosition = PixelsToTime(gridLocation.X);

The setting key to use is:

    {Name}/LegacyCursorActiveRow

Actual focused test output after implementation:

    Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7

Actual full test output after implementation:

    Passed!  - Failed:     0, Passed:   430, Skipped:     0, Total:   430

Actual focused test output after inverting the preference:

    Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7

Actual full test output after inverting the preference and isolating timeline control tests:

    Passed!  - Failed:     0, Passed:   430, Skipped:     0, Total:   430

Additional lasso requirements added on 2026-07-10:

    When LegacyCursorActiveRow is disabled during normal or CAD-style lasso multi-select, do not move the cursor or active row while the lasso is being drawn. On mouse-up, after the final effect selection is known, set active row to the row where the lasso began and move the cursor to the smallest StartTime among the final selected effects. When LegacyCursorActiveRow is enabled, preserve existing lasso cursor and active-row behavior.

## Interfaces and Dependencies

Use existing WinForms, timeline, and profile settings infrastructure only. Do not add new packages.

At the end of implementation, `Common.Controls.Timeline.Grid` must expose:

    public bool LegacyCursorActiveRow { get; set; }

This property controls whether selection-driven cursor and active-row updates use legacy behavior. It does not replace `ClickingGridSetsCursor`, which continues to control ordinary grid-click cursor placement.

`TimedSequenceEditorForm` must have a new `ToolStripMenuItem` field for the Edit menu toggle. The field name should be stable and descriptive, preferably:

    private System.Windows.Forms.ToolStripMenuItem legacyCursorActiveRowToolStripMenuItem;

`TimedSequenceEditorForm_Menu.cs` must contain the matching click handler that synchronizes the menu item to `TimelineControl.grid.LegacyCursorActiveRow`.

No undo service, sequence model, playback service, or rendering service dependency is needed for this feature.

## Revision Notes

- 2026-07-09: Initial ExecPlan created from `docs/sequencer/vix-3936-move-cursor-to-selected-effect.md`, incorporating the clarified requirement that effect right-click behavior remains unchanged in legacy mode and does not move the cursor in new-behavior mode.
- 2026-07-09: Milestone 3 completed with focused `TimelineCursorSelection` tests and full `Vixen.Tests` validation.
- 2026-07-09: Manual Timed Sequence Editor validation completed successfully.
- 2026-07-09: Jira issue `VIX-3936` updated with implementation and validation evidence in comment `40164`.
- 2026-07-10: Added new normal and CAD-style lasso requirements from user direction. The plan now includes Milestone 5 for lasso finalization behavior and records that the previous completed outcome is not final until this new behavior is implemented and validated.
- 2026-07-10: Updated the lasso requirements after user clarification that all new lasso cursor and active-row behavior is contained in non-legacy mode. When legacy mode is enabled, existing lasso behavior must remain unchanged.
- 2026-07-10: Implemented non-legacy lasso finalization behavior, added automated lasso finalization tests, and recorded focused/full test evidence. Manual Timed Sequence Editor validation and Jira update remain.
- 2026-07-10: Recorded user confirmation that the implemented lasso changes work as expected in the Timed Sequence Editor. Jira update remains.
- 2026-07-10: Inverted the preference model so the new cursor and active-row behavior is the default. The Edit menu item is now `Legacy Cursor / Active Row`; checked means legacy behavior and unchecked means the new VIX-3936 behavior.

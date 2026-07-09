# Implement VIX-3936 Move Cursor To Selected Effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. The source requirements are in `docs/sequencer/vix-3936-move-cursor-to-selected-effect.md`; this ExecPlan repeats the required context so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users often select an effect in the Timed Sequence Editor and then perform cursor-dependent work such as pasting, previewing, or editing around that effect. Today, selecting an effect changes the active row but does not move the timeline cursor to the selected effect; users must click empty grid space separately to place the cursor. After this change, users can opt into `Edit > Move Cursor To Selected Effect`. When enabled, selecting an effect moves the timeline cursor to the selected effect start while preserving the existing active-row behavior.

The visible result is in the Timed Sequence Editor. With the new menu item unchecked, selecting effects and right-clicking effects behave as they do today. With the menu item checked, left-click selection moves the cursor to the selected effect start; overlapping-effect popup selection moves the cursor only after a specific effect is chosen; right-clicking an effect does not move the cursor under the new preference; and right-clicking empty grid space keeps the current clicked-time cursor placement.

## Progress

- [x] (2026-07-09) Read `.agents/PLANS.md`, the VIX-3936 specification, the timeline grid selection code, the Timed Sequence Editor menu persistence code, and existing sequencer tests.
- [x] (2026-07-09) Created this initial ExecPlan with implementation milestones, validation steps, and the clarified right-click behavior.
- [x] (2026-07-09) Completed Milestone 1: added `Grid.MoveCursorToSelectedEffect`, the Timed Sequence Editor Edit menu toggle, click synchronization, and `XMLProfileSettings` load/save using `{Name}/MoveCursorToSelectedEffect`.
- [x] (2026-07-09) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineActiveRowNavigation --no-restore`; build succeeded and 7 tests passed. Warnings were pre-existing package/compiler warnings unrelated to Milestone 1.
- [ ] Implement the grid preference, cursor target helpers, and selection-path cursor updates.
- [ ] Add focused automated tests where practical and run the focused/full test commands.
- [ ] Manually validate the Timed Sequence Editor behavior.
- [ ] Update Jira issue `VIX-3936` with implementation notes and validation evidence.

## Surprises & Discoveries

- Observation: Right-click currently sets the active row and, when `ClickingGridSetsCursor` is true, unconditionally assigns `CursorPosition = PixelsToTime(gridLocation.X)` in `Grid_Mouse.cs`.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs`, `OnMouseUp`, right-button branch.

- Observation: The overlapping-effects popup suppresses automatic grid selection, then later selects the chosen effect through the public `TimelineControl.SelectElement(tse)` path.
  Evidence: `TimedSequenceEditorForm.timelineControl_ElementsSelected` sets `e.AutomaticallyHandleSelection = false` and `contextMenuStripElementSelectionItem_Click` calls `TimelineControl.SelectElement(tse)`.

- Observation: Tests can instantiate `TimelineControl` directly and add rows without launching the full Timed Sequence Editor.
  Evidence: `src/Vixen.Tests/Sequencer/TimelineActiveRowNavigationTests.cs` creates `new TimelineControl(Guid.NewGuid())`, adds rows, and exercises public timeline APIs.

## Decision Log

- Decision: Add this behavior as an opt-in grid property that defaults to `false`.
  Rationale: The requirement is to preserve old behavior by default and persist a user preference from the Timed Sequence Editor menu.
  Date/Author: 2026-07-09 / Codex from VIX-3936 requirements.

- Decision: Implement ordinary selection cursor movement in the shared timeline grid rather than only in `TimedSequenceEditorForm`.
  Rationale: The grid already owns effect hit testing, selected element state, active-row updates, and the private `CursorPosition` wrapper. Keeping the behavior there avoids editor-only duplication and keeps popup selection as the only host-specific edge path.
  Date/Author: 2026-07-09 / Codex.

- Decision: Preserve existing right-click behavior when the preference is disabled; when the preference is enabled, suppress cursor movement only for right-clicks on effects.
  Rationale: The user clarified that the no-move right-click rule is only for the enabled preference state. Empty-grid right-click remains a cursor-placement action.
  Date/Author: 2026-07-09 / user clarification recorded by Codex.

- Decision: Do not scroll horizontally when moving the cursor to an effect start.
  Rationale: VIX-3936 requests cursor state movement only. Changing `VisibleTimeStart` would introduce a second visual behavior that users did not request.
  Date/Author: 2026-07-09 / VIX-3936 requirements.

- Decision: Treat this as UI-only state.
  Rationale: Cursor movement does not change sequence content, so it must not call `SequenceModified()`, create undo history, or write to sequence model fields.
  Date/Author: 2026-07-09 / VIX-3936 requirements.

## Outcomes & Retrospective

Milestone 1 is complete. The grid now exposes `MoveCursorToSelectedEffect`, the Timed Sequence Editor Edit menu has a persisted `Move Cursor To Selected Effect` toggle, and the toggle synchronizes to the grid property. No selection behavior has changed yet; Milestone 2 remains responsible for cursor movement and right-click behavior.

Focused validation after Milestone 1 passed with 7 `TimelineActiveRowNavigation` tests. Full VIX-3936 behavior, manual validation, and Jira implementation evidence remain incomplete.

## Context and Orientation

The Timed Sequence Editor is the WinForms sequence-editing module under `src/Vixen.Modules/Editor/TimedSequenceEditor`. It hosts the reusable timeline control from `src/Vixen.Common/Controls/TimeLineControl`.

A timeline effect displayed in the grid is represented by `Common.Controls.Timeline.Element` in `src/Vixen.Common/Controls/TimeLineControl/Element.cs`. Its important properties for this work are `StartTime`, `Duration`, `EndTime`, `Selected`, and `Row`. `StartTime` is the left edge of the effect in timeline time.

A row is represented by `Common.Controls.Timeline.Row` in `src/Vixen.Common/Controls/TimeLineControl/Row.cs`. `Row.Active` means the row is the current editing target. This is not the same as `Row.Selected`; selecting a row usually selects the row's effects, while activating a row marks the target row.

The grid is `Common.Controls.Timeline.Grid`, split across `Grid.cs` and `Grid_Mouse.cs`. `Grid.cs` owns properties, events, selection helpers, and drawing. `Grid_Mouse.cs` owns mouse interaction. The grid has a private `CursorPosition` property that delegates to `TimeLineGlobalStateManager.Manager(instanceId).CursorPosition` and invalidates the grid. Setting that property is enough to move the visible timeline cursor in the grid, ruler, waveform, and editor status display because the global state manager raises cursor moved events.

The current effect click selection path is in `Grid_Mouse.cs`, `OnMouseDown`. When the user left-clicks one or more elements, the grid stores those elements in `m_mouseDownElements`. If the editor does not suppress automatic selection through `_ElementsSelected`, the grid sets `element.Selected = true`, sets `m_lastSingleSelectedElementLocation`, sets the row under the click active, and calls `_SelectionChanged()`.

The current mouse-up path in `Grid_Mouse.cs`, `OnMouseUp`, has another single-click case for a click on one of multiple selected elements. It clears selected elements, selects the first element under the click, calls `_SelectionChanged()`, and sets the row active. It also sets the row active for Ctrl/Shift clicks.

Right-click handling is also in `Grid_Mouse.cs`, `OnMouseUp`. It clears active rows, sets the row under the click active, assigns `CursorPosition = PixelsToTime(gridLocation.X)` when `ClickingGridSetsCursor` is true, and raises `_ContextSelected`. VIX-3936 must not change this behavior while the new preference is off. With the preference on, the cursor assignment must be skipped only when the right-click is on one or more effects.

Overlapping effects are handled by the Timed Sequence Editor host. In `TimedSequenceEditorForm.cs`, `timelineControl_ElementsSelected` shows `contextMenuStripElementSelection` when more than one element is under the cursor and sets `AutomaticallyHandleSelection = false`. When the user chooses one menu item, `contextMenuStripElementSelectionItem_Click` calls `TimelineControl.SelectElement(tse)`. This path must move the cursor only after a concrete effect is chosen, not when the popup opens.

The menu and persistence pattern to copy is `CAD Style Selection Box`. The existing menu item is `cADStyleSelectionBoxToolStripMenuItem` in `TimedSequenceEditorForm.Designer.cs`. Startup loading is in `TimedSequenceEditorForm.cs` near `cADStyleSelectionBoxToolStripMenuItem.Checked = TimelineControl.grid.aCadStyleSelectionBox = xml.GetSetting(...)`. Close-time saving is in the same file near `xml.PutSetting(..., "CadStyleSelectionBox", cADStyleSelectionBoxToolStripMenuItem.Checked)`. The click handler is `cADStyleSelectionBoxToolStripMenuItem_Click` in `TimedSequenceEditorForm_Menu.cs`.

Because implementation adds or modifies public C# APIs such as `Grid.MoveCursorToSelectedEffect`, the implementer must use the project `csharp-docs` skill and add XML comments to those public or protected members in the same change. Use CRLF line endings and tabs for C# indentation.

## Plan of Work

Milestone 1 adds the persisted preference surface. Add a public Boolean property to `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`, near the other grid behavior properties, named `MoveCursorToSelectedEffect`. Document it with XML comments. The property defaults to `false`; an auto-property is sufficient because `false` is the default Boolean value. In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`, add a `ToolStripMenuItem` field named `moveCursorToSelectedEffectToolStripMenuItem` or an equivalent project-consistent name. Add it to the Edit menu near `cADStyleSelectionBoxToolStripMenuItem`, set `CheckOnClick = true`, set the text to `Move Cursor To Selected Effect`, and wire its click event. In `TimedSequenceEditorForm_Menu.cs`, add a click handler that assigns `TimelineControl.grid.MoveCursorToSelectedEffect = moveCursorToSelectedEffectToolStripMenuItem.Checked`. In `TimedSequenceEditorForm.cs`, load and save the setting beside the CAD setting using the stable app setting key `{Name}/MoveCursorToSelectedEffect`, defaulting to `false`.

Milestone 1 acceptance is observable without changing selection behavior: launching the editor with no setting shows the new Edit menu item unchecked, toggling it changes `TimelineControl.grid.MoveCursorToSelectedEffect`, and after closing and reopening the editor the checked state persists.

Milestone 2 implements cursor movement for selection paths. In `Grid.cs`, add private helpers that centralize the cursor target rule. A clear shape is:

    private enum CursorSelectionTarget
    {
        EarliestSelected,
        LastActionElement
    }

    private void MoveCursorForSelectedEffects(IEnumerable<Element> actionElements, CursorSelectionTarget target)

This enum can stay private to `Grid`; do not make it public unless a public API truly needs it. The helper must return immediately when `MoveCursorToSelectedEffect` is false or when the action element list is null or empty. For `EarliestSelected`, use the selected elements after the selection operation and choose the minimum `StartTime`. For `LastActionElement`, use the last relevant element from the current action and choose that element's `StartTime`. The helper must set only `CursorPosition`; it must not change `VisibleTimeStart`, row active state, selected elements, undo state, or sequence modified state.

Call this helper in the existing selection paths after selected elements and active row have been updated. For a plain left-click selection where `_ElementsSelected(m_mouseDownElements)` returns true and elements are selected in `OnMouseDown`, move to the selected action element start. If exactly one element is selected, this is its `StartTime`; if multiple elements are selected by that operation without Ctrl/Shift, use the earliest selected start. For the mouse-up path that reduces multiple selected elements to `m_mouseDownElements.First()`, move to that first element start after selecting it and setting the row active. For Ctrl and Shift paths that set active row, move to the last selected effect associated with the latest click, using `m_mouseDownElements.LastOrDefault()` if that is the best available action element. If implementation discovers that `SelectElementsBetween` has a more accurate latest-click target, use that and record the detail in `Surprises & Discoveries`.

Update `Grid.SelectElement(Element element)` so popup selection through `TimelineControl.SelectElement(tse)` moves the cursor to the chosen element start when the preference is enabled. This public helper currently only marks the element selected and raises `_SelectionChanged()`. Keep that behavior and add the cursor move after the selection change. Do not show or move the cursor when the overlapping-effects popup opens, because `timelineControl_ElementsSelected` has not yet identified the chosen effect.

Update `Grid.SelectElements(IEnumerable<Element> elements)` only if there is a clear product path that selects multiple effects as one selection action and should honor the preference. If updated, materialize `elements` once to avoid repeated enumeration, select them, call `_SelectionChanged()`, then move to the earliest selected start. Preserve existing behavior if `elements` is empty.

Milestone 2 also updates right-click behavior in `Grid_Mouse.cs`. In the right-button branch of `OnMouseUp`, preserve the current code when `MoveCursorToSelectedEffect` is false. When the preference is true and `m_mouseDownElements` contains any effects, do not assign `CursorPosition`. When the preference is true and no effect is under the right-click, keep assigning `CursorPosition = PixelsToTime(gridLocation.X)` if `ClickingGridSetsCursor` is true. In all cases, keep clearing and setting active row, and keep raising `_ContextSelected`.

Milestone 2 acceptance is observable in the editor: with the preference enabled, left-clicking an effect moves the cursor to the effect start, choosing an effect from the overlap popup moves the cursor after the choice, and right-clicking an effect does not move the cursor. With the preference disabled, all existing selection and right-click behavior remains unchanged.

Milestone 3 adds tests and validation. Add focused tests under `src/Vixen.Tests/Sequencer`, either in a new `TimelineCursorSelectionTests.cs` file or another clearly named file. Use the pattern from `TimelineActiveRowNavigationTests`: create `TimelineControl` with a known `Guid`, add rows, add `Element` instances with known `StartTime` and `Duration`, and inspect `TimeLineGlobalStateManager.Manager(id).CursorPosition`. Set `timelineControl.grid.MoveCursorToSelectedEffect = true` for enabled cases. Cover at least: `SelectElement` moves to the chosen element start when enabled; `SelectElement` does not move when disabled; `SelectElements` moves to the earliest selected start if that public method is updated; right-click-on-effect no-op should be covered by automated test only if it can be done cleanly without brittle WinForms mouse simulation. If right-click cannot be tested cleanly, record that in `Surprises & Discoveries` and rely on manual validation for that branch.

Milestone 3 validation requires running:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineCursorSelection --no-restore

Then run the full tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If either command fails for an environment reason unrelated to this change, capture the exact error and run the narrowest possible test command that demonstrates the new behavior. Do not leave failures unexplained.

Milestone 4 performs manual validation and Jira update. Open or create a Timed Sequence Editor sequence with at least two rows and several effects, including overlapping effects. Validate every acceptance criterion below. Then update Jira issue `VIX-3936` with a comment summarizing implementation decisions, automated test results, manual validation results, and any limitations.

## Concrete Steps

Work from repository root `C:\Dev\Vixen`.

First inspect the current files:

    rg -n "MoveCursorToSelectedEffect|cADStyleSelectionBox|CadStyleSelectionBox|CursorPosition|SelectElement|ContextSelected|ElementsSelected" src\Vixen.Common\Controls\TimeLineControl src\Vixen.Modules\Editor\TimedSequenceEditor src\Vixen.Tests\Sequencer

Add the grid preference in `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`. Place it near `ClickingGridSetsCursor` or the other grid behavior properties. Use XML documentation like:

    /// <summary>
    /// Gets or sets a value that indicates whether selecting effects moves the timeline cursor to an effect start.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if effect selection moves the timeline cursor to an effect start; otherwise, <see langword="false" />.
    /// The default is <see langword="false" />.
    /// </value>
    public bool MoveCursorToSelectedEffect { get; set; }

Add the private helper and use it from the relevant `Grid_Mouse.cs` and `Grid.cs` selection paths. Keep helper names and enum names private unless they need to be called from `TimelineControl`. Prefer materializing enumerables with `ToList()` once before selecting or computing target times.

Add the Edit menu item in `TimedSequenceEditorForm.Designer.cs`. Because this is designer-generated code, follow the style already present for `cADStyleSelectionBoxToolStripMenuItem`: declare the field near the existing menu item field, instantiate it near the top initializer block, add it to `editToolStripMenuItem.DropDownItems.AddRange`, set `CheckOnClick`, `Name`, `Size`, `Text`, and `Click`.

Load the setting in `TimedSequenceEditorForm.cs` near the CAD load line:

    moveCursorToSelectedEffectToolStripMenuItem.Checked = TimelineControl.grid.MoveCursorToSelectedEffect =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/MoveCursorToSelectedEffect", Name), false);

Save the setting in `TimedSequenceEditorForm.cs` near the CAD save line:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/MoveCursorToSelectedEffect", Name), moveCursorToSelectedEffectToolStripMenuItem.Checked);

Add the click handler in `TimedSequenceEditorForm_Menu.cs` near `cADStyleSelectionBoxToolStripMenuItem_Click`:

    private void moveCursorToSelectedEffectToolStripMenuItem_Click(object sender, EventArgs e)
    {
        TimelineControl.grid.MoveCursorToSelectedEffect = moveCursorToSelectedEffectToolStripMenuItem.Checked;
    }

Add tests in `src/Vixen.Tests/Sequencer/TimelineCursorSelectionTests.cs`. A minimal setup should look like the active-row navigation tests: create a `Guid id`, create `TimelineControl timelineControl = new TimelineControl(id)`, add rows, add `Element` objects with `StartTime` and `Duration` to rows with `row.AddElement(element)`, set `timelineControl.grid.MoveCursorToSelectedEffect`, call the public selection method, and assert `TimeLineGlobalStateManager.Manager(id).CursorPosition`.

Run focused and full validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TimelineCursorSelection --no-restore
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Before finishing, verify CRLF line endings for edited Markdown files and do not reformat unrelated code.

## Validation and Acceptance

Automated validation passes when the focused `TimelineCursorSelection` tests pass and the full `Vixen.Tests` suite passes, or when any unrelated environment failure is clearly documented with narrower passing evidence for this change.

Manual validation in the Timed Sequence Editor must show these behaviors:

1. With `Edit > Move Cursor To Selected Effect` unchecked, clicking an effect updates the active row and does not move the timeline cursor to the effect start merely because of selection.
2. With the menu item checked, clicking a single effect updates the active row and moves the timeline cursor to that effect's start time.
3. With the menu item checked, clicking an already-selected effect moves the cursor to that effect's start time again.
4. With the menu item checked, selecting multiple effects in one operation moves to the earliest selected effect start for general multi-select, and Ctrl/Shift selection moves to the latest clicked or last selected effect for that action.
5. With the menu item checked, clicking overlapping effects that show the selection popup does not move the cursor when the popup opens. Choosing one effect from the popup moves the cursor to the chosen effect's start time.
6. With the menu item unchecked, right-clicking an effect preserves today's behavior exactly. With the menu item checked, right-clicking an effect preserves active row and context menu behavior but does not move the cursor because of this feature. Right-clicking empty grid space still moves the cursor to the clicked time.
7. Moving the cursor to an effect start does not change `VisibleTimeStart` and does not horizontally scroll the grid.
8. Closing and reopening the Timed Sequence Editor preserves the menu item checked state. A missing setting defaults to unchecked.
9. Cursor movement caused by this feature does not call `SequenceModified()`, does not add undo history, and does not write sequence content.

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

    {Name}/MoveCursorToSelectedEffect

Expected focused test output after implementation should resemble:

    Passed!  - Failed: 0, Passed: <N>, Skipped: 0, Total: <N>

Update this section with actual command output excerpts during implementation.

## Interfaces and Dependencies

Use existing WinForms, timeline, and profile settings infrastructure only. Do not add new packages.

At the end of implementation, `Common.Controls.Timeline.Grid` must expose:

    public bool MoveCursorToSelectedEffect { get; set; }

This property controls only selection-driven cursor movement. It does not replace `ClickingGridSetsCursor`, which continues to control ordinary grid-click cursor placement.

`TimedSequenceEditorForm` must have a new `ToolStripMenuItem` field for the Edit menu toggle. The field name should be stable and descriptive, preferably:

    private System.Windows.Forms.ToolStripMenuItem moveCursorToSelectedEffectToolStripMenuItem;

`TimedSequenceEditorForm_Menu.cs` must contain the matching click handler that synchronizes the menu item to `TimelineControl.grid.MoveCursorToSelectedEffect`.

No undo service, sequence model, playback service, or rendering service dependency is needed for this feature.

## Revision Notes

- 2026-07-09: Initial ExecPlan created from `docs/sequencer/vix-3936-move-cursor-to-selected-effect.md`, incorporating the clarified requirement that effect right-click behavior remains unchanged when the preference is disabled and does not move the cursor when the preference is enabled.

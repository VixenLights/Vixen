# Export Wizard Controller Toggle Improvements (VIX-2823)

## Overview

The Export Wizard provides a list of controllers on step 3 that the user can enable or disable for export. Some users have a long
list of controllers and they may not need all of them selected. VIX-2823 is an improvement to allow the user to multi select
rows that they can then toggle on or off. They would also like to select / unselect all with a simple set of toggle buttons.

## Current Implementation (as of this writing)

- Step 3 is `BulkExportControllersStage` (`src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.cs` and
  `.Designer.cs`), a `WizardStage` (`src/Vixen.Common/Controls/Wizard/WizardStage.cs`).
- The list is `networkListView`, a `Common.Controls.ListViewEx` (`src/Vixen.Common/Controls/DragAndDropListView.cs`) —
  a `ListView` subclass with `OwnerDraw = true`, `CheckBoxes = true`, `View = Details`, `HeaderStyle = Nonclickable`,
  and built-in drag/drop row reordering (`AllowRowReorder`).
- `MultiSelect = false` today — only one row can be highlighted at a time. There is no select-all, no header checkbox,
  and no keyboard shortcut handling anywhere in this stage or elsewhere in the wizard.
- Each row's checkbox reflects `Controller.IsActive`. Checking/unchecking a row fires `ItemChecked` →
  `NetworkListView_ItemChecked` → `ReIndexControllerChannels()`, which recalculates `Controller.Index`, `IsActive`,
  and the Start/End channel numbers for every row in the list.
- Row reordering via drag-and-drop already iterates `SelectedItems` (plural) in `ListViewEx.OnDragDrop`/`OnDragOver`/
  `OnItemDrag`, so it is already written to support moving more than one highlighted row at once — it simply can't be
  exercised today because `MultiSelect` is off.
- The checkbox glyph is fully owner-drawn (`List_DrawSubItem`, `DragAndDropListView.cs:222-259`); clicking it relies on
  `ListView`'s native checkbox hit-testing (unaffected by `OwnerDraw`, which only changes painting).

## Terminology (to avoid the ambiguity in the original ask)

The original requirements overload the word "select" for two different concepts. This spec uses distinct terms:

- **Highlight / highlighted rows** — the standard `ListView` selection state (`ListViewItem.Selected`), changed via
  click, Shift+click, Ctrl+click, Ctrl+A, arrow keys, or Esc. This is a UI-navigation concept only.
- **Checked / enabled** — the existing per-row checkbox state (`ListViewItem.Checked`, mapped to `Controller.IsActive`).
  This is the thing that actually controls whether a controller is included in the export.

The feature lets the user highlight many rows at once, then flip the **checked** state of the whole highlighted group in
one action.

## Requirements

### 1. Multi-highlight selection

- Set `networkListView.MultiSelect = true`.
- Shift+Click and Ctrl+Click behave as standard Windows `ListView` multi-select (native framework behavior — no custom
  code needed beyond enabling `MultiSelect`).
- **Ctrl+A** highlights every row in the list.
- **Esc** clears the current highlight (`SelectedIndices.Clear()`) when the list has focus. Clicking on empty space
  within the list already clears the highlight natively — no new code is required for that case, and clicks on
  sibling controls (label, panel background) outside the list are explicitly out of scope for this iteration.
- Both shortcuts are scoped to when `networkListView` has focus (handled in a `KeyDown` handler on the control, not a
  form-level `ProcessCmdKey` override) so they don't interfere with the wizard's own Next/Back/Cancel keyboard handling.

### 2. Bulk checked-state toggle via Space bar

- With one or more rows highlighted, pressing **Space bar** while `networkListView` has focus toggles the checked
  state of every highlighted row to the opposite of the focused row's (`ListView.FocusedItem`) *current* checked
  state. Concretely: `bool target = !FocusedItem.Checked;` then set `Checked = target` on every item in
  `SelectedItems`.
  - This gives a predictable result even when the highlighted rows have mixed checked states — behavior is driven
    off one well-defined item, not a majority vote or per-item toggle.
  - If nothing is highlighted, Space bar does nothing.
- Clicking directly on a single row's checkbox glyph continues to affect only that row (existing behavior, unchanged)
  and does not apply to the rest of the current highlight — this is a deliberate rejection of "click any checkbox in
  the selection toggles them all," which was considered too implicit/surprising for a plain checkbox click.
- **Batching:** bulk toggles (Space bar here, and the two toolbar buttons below) must not fire
  `ReIndexControllerChannels()` once per row. Temporarily detach the `ItemChecked` handler (or otherwise suppress it),
  apply all `Checked` changes inside `networkListView.BeginUpdate()`/`EndUpdate()`, reattach the handler, and call
  `ReIndexControllerChannels()` exactly once at the end. This matters specifically for the "long list of controllers"
  case called out in the Overview.

### 3. Enable All / Disable All toolbar

- Add two buttons above `networkListView` (following the existing button convention used in
  `BulkExportSourcesStage.Designer.cs`, e.g. `btnAddAll`): **"Enable All"** and **"Disable All"**.
  - Names are deliberately not "Select All / Select None" — that phrasing collides with the highlight-selection
    terminology above and would be ambiguous about which state it acts on.
- These act on **every row's checked state** regardless of current highlight — they are the bulk on/off shortcut
  called out in the Overview ("select/unselect all with a simple set of toggle buttons"), independent of the
  highlight-and-toggle workflow in item 2.
- Same batching rule as above: single `ReIndexControllerChannels()` call per button click, not per row.
- A clickable header checkbox (Gmail/Drive-style) was considered and rejected for this iteration — `ListViewEx`
  currently has `HeaderStyle = Nonclickable` and there is no existing header-owner-draw/hit-testing code anywhere in
  the codebase to build on; adding one would be a materially larger, riskier change to a shared control
  (`Common.Controls.ListViewEx`) used elsewhere, for the same net effect as two buttons.

### 4. Theming

- New buttons must go through the existing `ThemeUpdateControls.UpdateControls(this)` call already present in
  `BulkExportControllersStage`'s constructor — no separate theming work should be needed if styled like the existing
  buttons in `BulkExportSourcesStage`.

## Out of Scope

- Clicking outside the `ListView` control (on sibling controls) to clear the highlight.
- A clickable master checkbox embedded in the column header.
- Changing `Common.Controls.ListViewEx` defaults globally (changes are confined to the `BulkExportControllersStage`
  instance's Designer configuration).
- Any change to drag-and-drop row reordering logic itself, beyond the regression testing called out below.

## Risks / Concerns

1. **Multi-row drag reorder regression.** `ListViewEx.OnDragDrop` already loops over `SelectedItems` and computes a
   single `dropIndex` anchored on `SelectedItems[0].Index`. Turning on `MultiSelect` means this path is exercised for
   the first time in production with potentially non-contiguous highlighted rows (e.g., rows 1, 3, 5 highlighted via
   Ctrl+Click, then dragged). The relative order of inserted items is preserved by the existing loop, but this exact
   scenario has apparently never been tested. Needs explicit manual regression testing before/after this change,
   not just for the new checkbox behavior but for drag reorder with a scattered multi-highlight.
2. **Esc/Ctrl+A/Space scope.** These must only trigger while `networkListView` has focus. Verify Esc does not bubble
   up and get interpreted by the parent wizard dialog as "Cancel," and that Ctrl+A/Space don't leak focus-scoping
   issues if the user tabs between the list and the toolbar buttons.
3. **Owner-drawn checkbox hit box vs. row highlight click.** Pre-existing, not introduced by this change: the
   checkbox glyph position is computed in `CalculateCheckBoxSize`/`List_DrawSubItem` rather than using the OS's
   default checkbox column width, so the native checkbox hit-test box and the drawn glyph could be slightly
   misaligned. This is out of scope to fix but should be spot-checked since multi-select changes how often users
   click within a row versus its checkbox area.
4. **Shared control blast radius.** `ListViewEx` is used elsewhere in the codebase. This spec confines all
   behavioral changes (`MultiSelect`, Space/Ctrl+A/Esc handlers) to `BulkExportControllersStage`'s own event wiring
   and Designer settings — no changes to `ListViewEx`/`DragAndDropListView.cs` itself are required by this spec. If
   implementation finds otherwise, treat that as a design deviation requiring re-review, not a quick add.

## Acceptance Criteria

- [ ] `networkListView` supports Shift+Click range highlight and Ctrl+Click individual add/remove highlight.
- [ ] Ctrl+A (with the list focused) highlights all rows; Esc (with the list focused) clears the highlight.
- [ ] Space bar (with the list focused and ≥1 row highlighted) sets every highlighted row's checked state to the
      opposite of the focused row's checked state, in a single batched update (one `ReIndexControllerChannels()`
      call, verified by channel numbers being correct afterward).
- [ ] "Enable All" checks every row; "Disable All" unchecks every row; each is a single batched update.
- [ ] Clicking a single row's checkbox still only changes that row, regardless of current highlight.
- [ ] Existing drag-and-drop row reordering still works for a single highlighted row, and works correctly for a
      multi-row (including non-contiguous) highlight.
- [ ] `CanMoveNext` / channel start-end numbering remain correct after any bulk toggle operation.
- [ ] No regression to existing single-select-and-check workflow for users who don't use the new shortcuts.

## Test Plan

1. Highlight via mouse: single click, Shift+Click range, Ctrl+Click add/remove/toggle-off.
2. Keyboard: Ctrl+A from various starting highlight states; Esc clears; Space bar toggle with a single row
   highlighted, with a contiguous multi-row highlight, and with a non-contiguous (Ctrl+Click) highlight; Space bar
   with a mixed-checked-state highlight (assert result follows the focused-item rule, not majority).
3. Toolbar: Enable All / Disable All from a fresh load, from a partially-checked state, and with an active highlight
   present (assert toolbar buttons ignore highlight and act on all rows).
4. Channel/index integrity: after each bulk operation, verify Start/End channel columns and `Controller.Index`/
   `IsActive` match the checked rows in list order.
5. Drag reorder regression: drag a single highlighted row; drag a contiguous multi-row highlight; drag a
   non-contiguous (Ctrl+Click) multi-row highlight; verify resulting order and that reindex/channel numbers update
   correctly via the existing `networkListView_DragDrop` → `ReIndexControllerChannels()` path.
6. Focus scoping: verify Ctrl+A/Esc/Space have no effect when focus is on the toolbar buttons or elsewhere in the
   wizard, and that Esc doesn't close/cancel the wizard when the list has focus.
7. Empty-list and single-row-list edge cases for all of the above.

## Guidelines

The client code should follow best practices for Winforms wizards.
Use the project skills dotnet-best-practices, csharp-async, csharp-docs, and dotnet-design-pattern-review. Use these while
making any code changes. Do not write code and then go back and review against the skills in later steps.
Use plans.md for creating the plan to design and build the client. Include steps to create a JIRA issue covering the work in the plan.
Update the JIRA Issue with the refined requirements, acceptance criteria, and test plans using the jira skill before doing any coding.
Call out any risks or concerns in the plan when creating the design.

# VIX-2823: Multi-Select and Bulk Toggle for the Export Wizard Controller List

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`,
`Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.
Maintain this document in accordance with `.agents/plans.md` (the repository's copy of PLANS.md).

## Purpose / Big Picture

Vixen's Export Wizard (a sequence of WinForms pages used to export show data to controllers) has a
step called "Step 3" where the user picks which controllers are included in the export by checking
or unchecking a checkbox next to each controller's name in a list. Some users have dozens of
controllers in that list, and today the only way to turn several of them off is to click each
checkbox one at a time. JIRA issue VIX-2823 (https://vixenlights.atlassian.net/browse/VIX-2823) was
opened by a user who has to un-check "a lot" of controllers before every export and asked for a way
to do this in bulk, ideally by selecting a group of rows the way Windows Explorer lets you select a
group of files.

After this change, on Step 3 of the Export Wizard the user will be able to:

- Click a row, then Shift+Click another row, to highlight every row in between (the same range-select
  gesture used in Windows Explorer's file list).
- Ctrl+Click individual rows to add or remove them from the current highlight one at a time.
- Press Ctrl+A (while the list has focus) to highlight every row at once.
- Press Esc (while the list has focus) to clear the current highlight.
- With one or more rows highlighted, press the Space bar to flip all of their checkboxes together
  (turning a highlighted group of controllers on or off in one keystroke instead of one click per
  row).
- Click a new "Enable All" button to check every controller's checkbox regardless of what is
  currently highlighted, or "Disable All" to uncheck every controller's checkbox, with one click
  each.

You can see this working by launching Vixen, opening the Export Wizard (the exact menu path is
described in the Context and Orientation section below), advancing to Step 3, and trying each of the
gestures above against the list of controllers. Every milestone below states exactly what to click,
press, or observe to confirm that milestone is done.

## Progress

- [x] (2026-07-03) M0: Refine and update JIRA issue VIX-2823 with the detailed specification
  (description now includes Overview, Current Implementation, Terminology, Requirements, Out of
  Scope, Risks, Acceptance Criteria, and Test Plan). Also captured as
  `docs/export/vix-2823-selection-toggle-controller-list.md` in this repository.
- [ ] M1: Enable multi-row highlighting and Ctrl+A / Esc keyboard shortcuts on the Step 3 list.
- [ ] M2: Implement the Space-bar bulk checkbox toggle, batched to a single reindex pass.
- [ ] M3: Add the "Enable All" / "Disable All" toolbar buttons, also batched to a single reindex pass.
- [ ] M4: Manual regression pass (drag-and-drop reorder with multi-row highlights) and full skill
  compliance review (dotnet-best-practices, csharp-async, csharp-docs, dotnet-design-pattern-review),
  then final acceptance walkthrough against every criterion in VIX-2823.

## Surprises & Discoveries

(No implementation work has started yet. Entries will be added here as each milestone proceeds.)

## Decision Log

- Decision: Use distinct vocabulary for "highlighted" (the blue/selected rows a user picks with the
  mouse or keyboard) versus "checked" (the existing on/off checkbox per row that actually controls
  export inclusion), and use "Enable All" / "Disable All" rather than "Select All" / "Select None"
  for the bulk-checkbox buttons.
  Rationale: The original bug report and the plain English word "select" are used for two different
  things in this feature (highlighting rows vs. checking their boxes). Reusing the word "select" for
  both in code, UI text, or this plan would make every subsequent sentence ambiguous. This decision
  was made while refining the specification in `docs/export/vix-2823-selection-toggle-controller-list.md`
  and carried into this plan.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: The Space bar toggles every highlighted row to the opposite of the *focused* row's
  current checked state (not a per-row toggle, not a majority vote), where "focused row" means
  whichever row currently has the dotted focus rectangle (`ListView.FocusedItem` in WinForms terms —
  the one row within a highlighted group that last received keyboard/mouse focus).
  Rationale: A highlighted group can contain a mix of already-checked and already-unchecked rows.
  Picking one well-defined row (the focused one) as the source of truth gives a single, predictable
  outcome every time, instead of ambiguous behavior when the group is mixed.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Reject making the ListView's column header itself a clickable "check all" checkbox
  (the Gmail/Google Drive convention named in the original request) in favor of two ordinary buttons
  ("Enable All" / "Disable All") placed above the list.
  Rationale: The list control used here, `Common.Controls.ListViewEx`
  (`src/Vixen.Common/Controls/DragAndDropListView.cs`), currently sets `HeaderStyle =
  ColumnHeaderStyle.Nonclickable` and has no existing code anywhere in the repository for detecting
  clicks on a column header or drawing a checkbox into one. Building that from scratch would be a
  materially larger and riskier change to a control shared by other parts of the codebase, for the
  same end-user result achievable with two buttons.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Do not add any handling for clicks on controls outside the list (for example, the "Step
  3" label) to clear the highlight. Only Esc-while-focused and the list's own native "click empty
  space clears highlight" behavior will clear it.
  Rationale: WinForms `ListView` already clears its highlight when the user clicks on empty space
  inside the list, which covers the common case for free. Wiring mouse-down handlers on unrelated
  sibling controls to reach into the list and clear its highlight adds coupling for a rarely-needed
  edge case.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Position the new "Enable All"/"Disable All" buttons and `networkListView` using WinForms
  layout containers (a `TableLayoutPanel` with a `FlowLayoutPanel` nested in its button row) instead
  of hardcoded pixel `Location`/`Size` values with `Anchor`-based stretching.
  Rationale: The stage today positions `networkListView` with fixed pixel coordinates
  (`Location = (13, 41)`) and relies on `Anchor` only to stretch it when the wizard window is
  resized — it does not reflow content, and fixed pixel offsets do not adapt cleanly to Windows
  display scaling (for example a user running at 150% or 200% DPI scaling, or a different system
  font size) the way a layout container's measured, proportional positioning does. Introducing a
  button row alongside the list is exactly the situation where hardcoded coordinates become
  brittle — two more controls now have to avoid overlapping the list at every possible size and
  scale factor. Using a `TableLayoutPanel` (one row that auto-sizes to fit the button row, one row
  that fills all remaining space for the list) and a `FlowLayoutPanel` (which lays the two buttons
  out left-to-right and re-measures itself if button text width changes, for example due to a
  larger system font) means neither the buttons nor the list need hand-computed coordinates, and the
  layout adapts automatically to resolution, window size, and DPI/font scaling. This changes the
  original Milestone 3 approach described when this plan was first written (which used fixed
  `Location`/`Size` values); the Milestone 3 section below has been rewritten to reflect this
  decision.
  Date/Author: 2026-07-03 / Jeff Uchitjil

## Outcomes & Retrospective

(To be completed once all milestones are implemented and validated.)

## Context and Orientation

Vixen is a Windows desktop application for sequencing and running animated light shows. Part of it
is built with WPF (the main application shell), but some older tools — including the Export Wizard
this plan changes — are built with WinForms, an older Microsoft UI toolkit for Windows desktop
apps. WinForms and WPF can coexist in the same solution; this plan only touches WinForms code.

**What a "ListView" is, for readers unfamiliar with WinForms:** `System.Windows.Forms.ListView` is
the built-in Windows list/table control (the same kind of control File Explorer's "Details" view
uses). It supports an optional checkbox per row (`CheckBoxes = true`), an optional multi-row
highlight mode (`MultiSelect = true`), and a `Checked`/`Selected` boolean on each row
(`ListViewItem`). "Highlighted"/"Selected" in WinForms terms refers to the blue-background rows the
user has picked (`ListViewItem.Selected`, exposed as the collection `ListView.SelectedItems`); this
is a completely separate concept from a row's checkbox state (`ListViewItem.Checked`). This plan
uses "highlighted" throughout to avoid confusing the two, per the Decision Log entry above.

**The exact place this plan changes:** The Export Wizard lives in the module project
`src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj` (C# namespace
`VixenModules.App.ExportWizard`). It presents a sequence of pages ("stages"), each a subclass of
`WizardStage` (defined in `src/Vixen.Common/Controls/Wizard/WizardStage.cs`, itself a
`System.Windows.Forms.UserControl`). Step 3, "Configure the required outputs and order," is
implemented by two files that together make one C# class:

- `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.cs` — the code-behind: event
  handlers and logic.
- `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.Designer.cs` — the
  Windows-Forms-Designer-generated layout: control declarations, positions, sizes, and the
  properties set on them. This file is normally edited through Visual Studio's visual designer, but
  it is plain C# and can be edited by hand as long as the structure the designer expects is
  preserved (each control assigned to a field, added to `Controls`, positioned with `Location`,
  `Size`, and `Anchor`).

Inside `BulkExportControllersStage`, the list itself is a field named `networkListView`, declared as
`Common.Controls.ListViewEx` — a subclass of the standard `ListView` defined in
`src/Vixen.Common/Controls/DragAndDropListView.cs`. `ListViewEx` adds two things on top of the
standard control: (1) owner-drawing (`OwnerDraw = true`), meaning the control paints its own rows,
checkboxes, and column headers via the `DrawItem`, `DrawSubItem`, and `DrawColumnHeader` events
(handled by `List_DrawItem`, `List_DrawSubItem`, `List_DrawColumnHeader` in that file) instead of
using the operating system's default look, so that Vixen's dark theme colors apply; and (2)
drag-and-drop row reordering (`AllowRowReorder`), so the user can drag a row to a new position in
the list to reorder it, implemented in the overridden `OnDragDrop`, `OnDragOver`, and `OnItemDrag`
methods.

The stage's overall size is `573x355` pixels (`this.Size` at the bottom of
`InitializeComponent()`), and `this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;` is set,
which is WinForms' mechanism for scaling a form's fixed pixel coordinates when the system font size
differs from the one the designer was authored at — but it only rescales existing fixed coordinates
proportionally, it does not reflow controls to make room for new ones or adapt gracefully to every
DPI setting a user might have. Today `networkListView` is positioned with a fixed pixel `Location`
(`(13, 41)`) and a fixed `Size`, and relies on `Anchor = Top | Bottom | Left | Right` only to stretch
(not reflow) when the containing window is resized. Milestone 3 replaces this fixed-coordinate
approach with layout containers for the list and the new buttons, per the Decision Log entry above,
specifically so the new controls do not have to be manually kept from overlapping at every possible
window size, font size, or Windows display-scaling percentage.

As configured today (`BulkExportControllersStage.Designer.cs` lines 49-74), `networkListView` has
`CheckBoxes = true` (so each row shows a checkbox), `MultiSelect = false` (so clicking a row always
replaces any previous highlight with just that one row — this is the setting this plan changes first),
`HeaderStyle = ColumnHeaderStyle.Nonclickable` (clicking a column header does nothing), and four
columns: Controller, Channels, Start, End.

Each `ListViewItem` in the list has its `Tag` property set to the `Controller` object it represents
(`Vixen.Export.Controller` — a data class holding a controller's name, channel count, active/inactive
flag, and position index; see `UpdateNetworkList()` at `BulkExportControllersStage.cs` lines 30-65,
which builds the list from `_data.ActiveProfile.Controllers`). When the user checks or unchecks a
row, the standard `ListView.ItemChecked` event fires; `BulkExportControllersStage` currently wires
this in its `OnLoad` override (lines 18-22) to a handler, `NetworkListView_ItemChecked` (lines
72-75), which calls `ReIndexControllerChannels()` (lines 82-115). That method walks every row in
list order, sets each `Controller.Index` to its position, sets `Controller.IsActive` to match the
row's `Checked` state, and recomputes the "Start"/"End" channel-number columns so active controllers
are numbered contiguously. This method is the single source of truth for keeping the on-screen
channel numbers and the underlying `Controller` model in sync, and every change this plan makes must
continue to route through it (see the batching requirement in Milestone 2).

Drag-and-drop reordering (`OnDragDrop` in `DragAndDropListView.cs`, lines 68-109) already loops over
`SelectedItems` (plural) to move every highlighted row together, anchored on the position of
`SelectedItems[0]` (the first highlighted row). This code has never been exercised with more than one
highlighted row in production, because `MultiSelect` has always been `false` on this particular list
until this plan changes it. Milestone 4 includes an explicit manual regression pass for this reason.

A native WinForms quirk to be aware of before writing Milestone 2's code: when `CheckBoxes = true`,
pressing the Space bar while a `ListView` has keyboard focus already toggles the checkbox of
whichever single row currently has focus — this is a built-in default behavior, not something Vixen's
code currently implements. Milestone 2 must suppress this default behavior (by setting
`KeyEventArgs.SuppressKeyPress = true` inside a `KeyDown` handler for the Space key) and replace it
with the bulk-toggle behavior described in the Decision Log, or the two behaviors will conflict (the
focused row's box would flip once from the native behavior and then again from the new bulk logic).

There is no existing precedent anywhere in this repository for Ctrl+A / Esc keyboard shortcuts on a
`ListView`, nor for a "check all" toolbar, so this plan introduces both patterns for the first time;
there is nothing to reuse from elsewhere in the codebase for these two pieces.

No automated tests exist today for `BulkExportControllersStage` or `ListViewEx` (confirmed by
searching `src/Vixen.Tests/`, which has an `ExportWizard/` folder but it only covers
`FppHostValidator`, `FppDirectUploadService`, and `ExportProfile` cloning — none of it touches the
WinForms UI layer). This is expected for WinForms UI code, which is not practical to unit test
without a UI automation harness that this project does not have. Validation for every milestone in
this plan is therefore manual: launch the application, open the Export Wizard, and observe the
described behavior directly, as described in each milestone's acceptance section.

## Plan of Work

### Milestone 1 — Multi-row highlighting and Ctrl+A / Esc

**Scope:** Turn on multi-row highlighting on `networkListView` and add the two navigation shortcuts.
Nothing about checkbox behavior changes in this milestone — only which rows can be highlighted at
once and how.

**What to edit:**

In `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.Designer.cs`, find the line (around
line 68):

    this.networkListView.MultiSelect = false;

and change it to:

    this.networkListView.MultiSelect = true;

In `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.cs`, add a `KeyDown` event
subscription for `networkListView` inside the existing `OnLoad` override (lines 18-22), alongside the
existing `DragDrop` and `ItemChecked` subscriptions:

    networkListView.KeyDown += NetworkListView_KeyDown;

Add a new private method `NetworkListView_KeyDown(object sender, KeyEventArgs e)` that handles two
cases:

- If `e.KeyCode == Keys.A && e.Control` (Ctrl+A): call `networkListView.BeginUpdate()`, loop over
  every `ListViewItem` in `networkListView.Items` and set `.Selected = true` on each, call
  `networkListView.EndUpdate()`, and set `e.SuppressKeyPress = true` so the "ding" sound WinForms
  otherwise makes for unhandled Ctrl+A does not play.
- If `e.KeyCode == Keys.Escape` (Esc): call `networkListView.SelectedItems.Clear()` — WinForms
  exposes a `Clear()` method on the `SelectedListViewItemCollection` that unhighlights every row
  without changing any checkbox — and set `e.SuppressKeyPress = true`.

Space-bar handling is deliberately deferred to Milestone 2, and is described there so the batching
logic can be explained in one place.

**Acceptance for this milestone:** Build the solution (see Concrete Steps below), launch Vixen, open
the Export Wizard against any profile with at least four or five controllers configured, and advance
to Step 3. Click the first row, then hold Shift and click the fourth row: rows one through four
should highlight (blue background) together. Click a row, then Ctrl+Click two other non-adjacent
rows: all three should stay highlighted, with earlier ones remaining highlighted (this is standard
`MultiSelect = true` behavior — no code changes beyond the Designer.cs flag are needed to produce it,
which is itself a fact worth confirming by observation since it demonstrates the framework is doing
the work for free). With several rows highlighted, press Ctrl+A: every row in the list should become
highlighted. Press Esc: the highlight should clear completely, and every row's checkbox should be
unchanged from before you pressed Esc.

### Milestone 2 — Space-bar bulk checkbox toggle

**Scope:** Implement the actual "toggle a group of controllers on or off at once" behavior that
VIX-2823 asked for, triggered by the Space bar while one or more rows are highlighted.

**What to edit:**

In `BulkExportControllersStage.cs`, extend the `NetworkListView_KeyDown` method added in Milestone 1
with a third case:

- If `e.KeyCode == Keys.Space`: set `e.SuppressKeyPress = true` immediately (this suppresses both the
  "ding" sound and the native single-row checkbox toggle described in the Context section above,
  which would otherwise fire in addition to this method's own logic). If
  `networkListView.SelectedItems.Count == 0`, do nothing further. Otherwise, read
  `networkListView.FocusedItem` (the WinForms property giving the one row that currently has the
  keyboard focus rectangle, which is guaranteed non-null here because at least one item is
  highlighted) and compute `bool target = !networkListView.FocusedItem.Checked;`. Then call a new
  helper method, `SetCheckedForItems(networkListView.SelectedItems.Cast<ListViewItem>(), target)`,
  described next.

Add a new private method `SetCheckedForItems(IEnumerable<ListViewItem> items, bool isChecked)` that
implements the batching requirement from the Context section: temporarily unsubscribe
`NetworkListView_ItemChecked` from `networkListView.ItemChecked`, wrap the loop in
`networkListView.BeginUpdate()`/`EndUpdate()`, set `.Checked = isChecked` on every item in `items`,
re-subscribe `NetworkListView_ItemChecked`, and then call `ReIndexControllerChannels()` exactly once.
This guarantees that toggling ten highlighted rows recalculates the Start/End channel columns one
time, not ten times — important for the "long list of controllers" performance concern named in the
original bug report and the refined specification. This same helper will be reused by Milestone 3's
"Enable All"/"Disable All" buttons, so write it to accept any collection of `ListViewItem`, not just
the current highlight.

**Acceptance for this milestone:** Rebuild and relaunch as in Milestone 1. On Step 3, highlight three
rows using Shift+Click where the first is currently checked. Press Space: all three should become
unchecked (because the logic reads the *focused* row's current state — make sure, when testing, that
the focused row, the one with the dotted rectangle, is the checked one you expect; clicking a row
both highlights and focuses it, so the last row you clicked while building the Shift+Click range is
the focused one). Press Space again: all three should become checked again. Highlight a mixed group
(some checked, some not) and press Space: every row in the group should end up matching the inverse
of whichever row is focused — confirm this matches the Decision Log's rule, not a per-row toggle.
After any Space-bar toggle, confirm the Start/End channel columns for the still-checked rows renumber
correctly with no gaps, exactly as they do today when checking a single row by mouse.

### Milestone 3 — "Enable All" / "Disable All" toolbar buttons

**Scope:** Add the second bulk-toggle mechanism named in the original request — two buttons that act
on every row regardless of the current highlight.

**What to edit:**

Per the Decision Log, this milestone replaces `networkListView`'s fixed-coordinate positioning with
two nested layout containers instead of adding buttons at hand-picked pixel coordinates. Layout
containers are ordinary WinForms controls that position their *children* automatically —
`TableLayoutPanel` arranges children into a grid of rows/columns whose sizes can be "auto" (just big
enough for their content) or "percentage" (share of whatever space is left), and `FlowLayoutPanel`
lays its children out left-to-right (or top-to-bottom), wrapping and re-measuring itself as content
changes, similar to how text wraps in a word processor. Using them here means neither the button row
nor the list need a hand-computed `Location`, and both adapt automatically if the stage is resized or
the system font/DPI scale changes.

In `BulkExportControllersStage.Designer.cs`, add three new fields in addition to the existing ones,
constructed and wired inside `InitializeComponent()` in this order:

- `tableLayoutMain` (`System.Windows.Forms.TableLayoutPanel`) — the outer container. Configure it
  with `Dock = System.Windows.Forms.DockStyle.Fill`, `ColumnCount = 1`, `RowCount = 2`. Add one
  `RowStyle` with `SizeType = System.Windows.Forms.SizeType.AutoSize` (row 0, sized to exactly fit
  the button row) and one `RowStyle` with `SizeType = System.Windows.Forms.SizeType.Percent, Height =
  100F` (row 1, which then consumes all remaining vertical space for the list). Because this panel is
  `Dock = Fill`, it must be added to `this.Controls` *before* `lblConfigureOutput` in the
  `Controls.Add` call order (WinForms docks controls in reverse of their add order — the label,
  which is not part of this layout panel and keeps its own fixed `Location` at the top of the stage,
  needs to be added after `tableLayoutMain` so it still renders above the filled area; alternatively,
  give `lblConfigureOutput` a small fixed-height `Panel` of its own above `tableLayoutMain` with
  `Dock = Top` if that proves simpler once you are editing the Designer file directly — either
  approach is acceptable as long as the label remains visibly above the button row and the list).
- `flowLayoutButtons` (`System.Windows.Forms.FlowLayoutPanel`) — placed in `tableLayoutMain`'s row 0
  via `tableLayoutMain.Controls.Add(this.flowLayoutButtons, 0, 0)`. Configure it with
  `FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight`, `WrapContents = false`,
  `AutoSize = true`, `AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink`, and
  `Dock = System.Windows.Forms.DockStyle.Top` (so it hugs the top of row 0 rather than stretching to
  fill it, since row 0 itself is already sized to `AutoSize`).
- `btnEnableAll` and `btnDisableAll` (`System.Windows.Forms.Button`) — added to
  `flowLayoutButtons.Controls` (not directly to `tableLayoutMain` or the stage) in that order, so the
  `FlowLayoutPanel` places "Enable All" to the left of "Disable All" with its own default item margin
  handling the spacing between them (no manual pixel gap needed). Give each a `Margin` of
  `new Padding(3)` (WinForms' default control margin — explicit here since `FlowLayoutPanel` uses
  each child's `Margin` to decide spacing, unlike the fixed-coordinate approach used elsewhere in this
  file) and `AutoSize = true` so each button sizes itself to fit its own text ("Enable All" /
  "Disable All") rather than using a hardcoded `Size`, which is what makes the buttons safe for
  larger system fonts without clipped text.

Then move `networkListView` out of the stage's direct `Controls` collection and into
`tableLayoutMain`'s row 1 via `tableLayoutMain.Controls.Add(this.networkListView, 0, 1)`, and change
its positioning properties from fixed `Location`/`Size` to `Dock = System.Windows.Forms.DockStyle.Fill`
(removing the now-unnecessary `Anchor` setting, since a docked child of a percentage-sized
`TableLayoutPanel` row already fills all available space without needing anchor-based stretching).

In `BulkExportControllersStage.cs`, wire `btnEnableAll.Click` and `btnDisableAll.Click` (in `OnLoad`,
alongside the other event subscriptions) to two small handlers,
`BtnEnableAll_Click(object sender, EventArgs e)` and `BtnDisableAll_Click(object sender, EventArgs e)`,
each of which calls the `SetCheckedForItems` helper from Milestone 2, passing
`networkListView.Items.Cast<ListViewItem>()` (every row, not just the highlighted ones) and `true` or
`false` respectively.

Per the CLAUDE.md instruction to update XML documentation for any modified public or protected API:
`BulkExportControllersStage` itself has no public members changing shape in this plan (the new
buttons, handlers, and helper method are all private implementation details of this stage), so no XML
doc updates are expected to be needed; confirm this remains true once the milestone's code is written
before marking it complete, using the `csharp-docs` skill.

**Acceptance for this milestone:** Rebuild and relaunch as before. On Step 3, with a partially-checked
list and no rows highlighted, click "Enable All": every row's checkbox should become checked, and the
Start/End columns should renumber to cover every controller contiguously starting at 1. Click "Disable
All": every row's checkbox should become unchecked, and the Start/End columns should go blank exactly
as they do today when unchecking a single row (see `UpdateNetworkList()`'s existing "else" branch,
`BulkExportControllersStage.cs` lines 50-54, for the existing convention of blank Start/End for
inactive controllers). Now highlight a subset of rows (Shift+Click or Ctrl+Click) and click "Enable
All" again: confirm every row becomes checked, including ones outside the highlighted subset — this
confirms the buttons genuinely ignore the current highlight, as required. Finally, resize the wizard
dialog (drag its edge larger and smaller) and confirm the button row stays put at the top with the
list filling the rest of the space with no overlap or clipped text in either button — this confirms
the `TableLayoutPanel`/`FlowLayoutPanel` approach is actually reflowing rather than relying on fixed
coordinates. If a Windows machine with a non-100% display scaling setting (for example 125% or 150%,
found under Windows Settings > Display > Scale) is available, repeat this same check there and
confirm neither button's text is clipped and the list still fills the remaining space correctly.

### Milestone 4 — Regression pass and skill compliance review

**Scope:** This milestone adds no new user-facing behavior. It exists to catch regressions introduced
by turning on `MultiSelect` (specifically to the drag-and-drop reordering path, which the Context
section notes has never run with more than one highlighted row before) and to apply the project's
required code-quality skills before considering the feature done, per CLAUDE.md's instruction to use
those skills "while making any code changes," not "go back and review... in later steps" — this
milestone is the final confirmation pass, not a substitute for applying them during Milestones 1-3.

**What to do:**

Re-read the changes made in Milestones 1 through 3 against each of: `dotnet-best-practices`,
`csharp-async` (even though this feature is entirely synchronous UI event handling, confirm nothing
introduced accidental blocking calls or async-void patterns), `csharp-docs`, and
`dotnet-design-pattern-review` (the project's skills, found under `.agents/skills/`). Fix anything
those skills flag directly in the same milestone rather than deferring it.

Manually test the following regression scenarios that were not exercised before this plan, because
`MultiSelect` was always `false` in production until Milestone 1:

- Highlight two adjacent rows (Shift+Click) and drag them to a new position in the list using the
  mouse (the existing drag-and-drop reordering gesture). Confirm both rows move together and land in
  the expected order at the drop location, and that the Start/End channel columns are correct
  afterward.
- Highlight two non-adjacent rows (Ctrl+Click, e.g. rows 1 and 4 in a five-row list) and drag them.
  Confirm the resulting order is sensible (both selected rows move to sit together at the drop point,
  in their original relative order) and note the actual observed behavior in the Surprises &
  Discoveries section, since this exact scenario was previously unreachable code and its precise
  behavior was not verified before this plan.
- Confirm Ctrl+A, Esc, and Space have no effect when focus is on `btnEnableAll`, `btnDisableAll`, or
  elsewhere in the wizard dialog (for example, the wizard's own Next/Back/Cancel buttons) — only when
  `networkListView` itself has focus.
- Confirm pressing Esc while `networkListView` has focus does not close or cancel the wizard dialog
  (only clears the highlight), since Esc is a common "close dialog" shortcut and the wizard dialog may
  have its own Esc handling that this new code must not conflict with.
- Repeat the Milestone 1-3 acceptance checks against an export profile with only one controller and
  against one with zero controllers configured, to confirm no exception occurs and the new controls
  behave sensibly (for example, "Enable All" with an empty list should simply do nothing observable).

Finally, walk through every checkbox in the "Acceptance Criteria" section of JIRA issue VIX-2823 one
by one and confirm each is genuinely satisfied by manual observation, then update the JIRA issue
(using the `jira` skill) to reflect completion — for example by checking off each acceptance-criteria
box in the issue description and adding a comment summarizing what was implemented and how it was
verified, and transitioning the issue out of "New Ticket" status if a suitable transition exists.

**Acceptance for this milestone:** All bullet points above have been manually exercised with no
unexpected behavior (or any unexpected behavior found has been fixed, or explicitly documented as an
accepted limitation in Decision Log with rationale). VIX-2823's acceptance criteria are all checked
off in JIRA with a comment describing verification.

## Concrete Steps

All commands below are run from the repository root, `C:\Dev\Vixen`, using PowerShell.

Build the solution in Debug configuration after each milestone's code changes:

    msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug

Expect this to finish with `0 Error(s)` in the summary. If new compiler errors appear, they will name
the file and line; re-open that file with the Read tool, fix the issue, and rebuild.

Run the existing automated test suite to confirm no unrelated regression (this suite does not cover
the WinForms UI changed by this plan, per the Context section, but must still pass):

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj

Expect all existing tests to pass; this plan does not add new automated tests because the code it
changes is WinForms UI event-handling with no existing UI test harness in this repository (see
Context and Orientation).

Launch the built application to manually validate each milestone. The build output for a Debug build
lands in `/Debug/Output/`; run the main executable found there (for example
`Debug\Output\Vixen.exe` — confirm the exact executable name by listing that directory, since it is
generated by the build and this plan does not hard-code a filename that could drift). From the running
application, open the Export Wizard (VixEditor's Sequencer/File menu contains an "Export" entry that
launches this wizard — if the exact menu path has moved since this plan was written, search the
running application's menus for "Export" and confirm it opens a multi-step wizard whose third page is
titled "Step 3: Configure the required outputs and order," which is the page this plan modifies).
Step through to Step 3 against any export profile that already has controllers configured (creating
one requires configuring at least one controller in the main application first, which is existing,
unrelated functionality this plan does not change).

## Validation and Acceptance

Each milestone section above states its own acceptance steps; this section restates the full,
end-to-end acceptance scenario a reviewer can follow after all four milestones are complete, without
needing to re-read the whole plan.

1. Build with `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` and confirm `0
   Error(s)`.
2. Run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj` and confirm all tests still pass.
3. Launch the built application, open the Export Wizard, and reach Step 3 against a profile with at
   least five controllers, some checked and some unchecked.
4. Shift+Click across a range of rows: confirm all rows in the range highlight.
5. Ctrl+Click two additional non-adjacent rows: confirm they join the highlight without disturbing
   the rest.
6. Press Ctrl+A: confirm every row highlights.
7. Press Esc: confirm the highlight clears and no checkbox changed.
8. Highlight a group of rows and press Space: confirm every row in the group's checkbox flips to the
   opposite of the focused row's prior state, and the Start/End channel columns are correct
   afterward.
9. Click "Enable All": confirm every row becomes checked and channel numbers renumber contiguously.
10. Click "Disable All": confirm every row becomes unchecked and Start/End columns go blank.
11. Drag a multi-row highlight (both adjacent and non-adjacent cases) to a new position and confirm
    reordering still works and channel numbers remain correct.
12. Confirm Ctrl+A/Esc/Space do nothing when focus is outside `networkListView`, and that Esc does not
    close the wizard.
13. Re-open VIX-2823 in JIRA and confirm every acceptance-criteria checkbox reflects what was just
    observed.

## Idempotence and Recovery

Every step in this plan is an ordinary source-controlled code edit; there is no data migration, no
destructive operation, and no external state to corrupt. If a milestone's build fails partway through
an edit, the fix is simply to correct the C# and rebuild — there is no partial-state cleanup required.
If manual testing reveals a milestone's behavior is wrong, the safe recovery path is to revert just
that milestone's edits (`git diff`/`git checkout` scoped to the two `BulkExportControllersStage*`
files) and retry, since no other files are touched by this plan. Because all changes are confined to
`BulkExportControllersStage.cs`, `BulkExportControllersStage.Designer.cs`, and (only if needed for a
documentation update) this plan file itself, re-running any milestone's edits from a clean checkout is
always possible without affecting unrelated code.

## Artifacts and Notes

The full requirements this plan implements are also recorded in
`docs/export/vix-2823-selection-toggle-controller-list.md` (the refined specification) and in the
JIRA issue description for VIX-2823, both written before this plan and kept consistent with it. If
this plan and either of those documents ever disagree after this plan is revised, this plan is the
one to trust for implementation order and technical detail, but the JIRA issue should be updated to
match per Milestone 4's closing step.

## Interfaces and Dependencies

This plan does not introduce any new public types, interfaces, or cross-project dependencies. All new
members are private to `VixenModules.App.ExportWizard.BulkExportControllersStage`:

In `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.cs`, by the end of this plan, the
following private members exist in addition to what is there today:

    private void NetworkListView_KeyDown(object sender, KeyEventArgs e)
    private void SetCheckedForItems(IEnumerable<ListViewItem> items, bool isChecked)
    private void BtnEnableAll_Click(object sender, EventArgs e)
    private void BtnDisableAll_Click(object sender, EventArgs e)

In `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.Designer.cs`, four new private
fields exist in addition to what is there today:

    private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutButtons;
    private System.Windows.Forms.Button btnEnableAll;
    private System.Windows.Forms.Button btnDisableAll;

`networkListView`'s existing field declaration and type (`Common.Controls.ListViewEx`) do not change
— only its parent container and positioning properties (`Dock = Fill` inside `tableLayoutMain` instead
of a fixed `Location`/`Size`/`Anchor` directly on the stage) change, per Milestone 3.

No project references, NuGet packages, or `Vixen.sln` changes are required — this plan only edits two
existing files in an existing project. `TableLayoutPanel` and `FlowLayoutPanel` are both part of the
`System.Windows.Forms` namespace already referenced by this project; no new `using` directives beyond
what `BulkExportControllersStage.Designer.cs` already needs for `Button`/`ListView` are required.

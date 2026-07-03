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
- [x] (2026-07-03) M1: Enable multi-row highlighting and Ctrl+A / Esc keyboard shortcuts on the Step
  3 list. `networkListView.MultiSelect` set to `true` in
  `BulkExportControllersStage.Designer.cs`; `NetworkListView_KeyDown` added and wired in `OnLoad` in
  `BulkExportControllersStage.cs`, handling Ctrl+A (highlight all rows) and Esc (clear highlight).
  Manually verified in the running application: multi-select and Ctrl+A work as designed. Esc
  initially did not work (it closed the whole wizard instead of clearing the highlight, because
  Escape is a WinForms "dialog key" routed to the wizard's `CancelButton` before `KeyDown` ever
  fires). A first fix attempt — a private `ListViewEx` subclass overriding `IsInputKey` — compiled
  but did not actually resolve the problem in manual re-testing (see Surprises & Discoveries). The
  working fix overrides `ProcessCmdKey` directly on `BulkExportControllersStage`, which intercepts
  Escape one step earlier in WinForms' key-processing pipeline, before `IsInputKey`/`ProcessDialogKey`
  are even consulted. Rebuilt and confirmed 0 errors; manually re-verified in the running
  application that Esc now clears the highlight without closing the wizard. See Surprises &
  Discoveries and the Decision Log for the full explanation.
- [x] (2026-07-03) M2: Originally scoped as "implement the Space-bar bulk checkbox toggle." Manual
  testing during M1 showed this already works natively once `MultiSelect`/`CheckBoxes` are both
  `true` — no bespoke toggle code was needed or written. Rescoped to the one real remaining gap:
  coalescing `ReIndexControllerChannels()` so a native bulk toggle (Space, or a checkbox click across
  a highlighted group) reindexes once instead of once per row. Implemented via a `_reindexPending`
  guard plus `BeginInvoke` in `NetworkListView_ItemChecked`. Verified by building `ExportWizard.csproj`
  with 0 errors; manual in-app confirmation of the coalescing (e.g., no stutter toggling a long list)
  is still outstanding.
- [x] (2026-07-03) M3: Added `tableLayoutMain` (`TableLayoutPanel`, 2 rows: auto-size button row,
  percent-100 list row) and `flowLayoutButtons` (`FlowLayoutPanel`) to
  `BulkExportControllersStage.Designer.cs`, replacing `networkListView`'s fixed `Location`/`Size`/
  `Anchor` with `Dock = Fill` inside the table's second row. `lblConfigureOutput` switched from a
  fixed `Location` to `Dock = Top`, added to `Controls` *after* `tableLayoutMain` so it claims the
  top strip first (WinForms docks the last-added control first). Added `btnEnableAll`/`btnDisableAll`
  (`AutoSize = true`, no hardcoded `Size`) inside `flowLayoutButtons`. In the code-behind, added a
  small private `SetAllChecked(bool isChecked)` helper (loops `networkListView.Items`, wrapped in
  `BeginUpdate`/`EndUpdate`) called by `BtnEnableAll_Click`/`BtnDisableAll_Click` — a minor deviation
  from the plan's original wording (which described each handler looping directly) to avoid two
  near-identical loops; behavior is identical either way, and it still relies on Milestone 2's
  `_reindexPending`/`BeginInvoke` coalescing with no changes to `NetworkListView_ItemChecked`.
  Verified by building `ExportWizard.csproj` with 0 errors. Manual in-app confirmation (resizing the
  wizard, checking both buttons ignore the current highlight, and DPI/scaling if a scaled machine is
  available) is still outstanding.
- [ ] M4: Manual regression pass (drag-and-drop reorder with multi-row highlights) and full skill
  compliance review (dotnet-best-practices, csharp-async, csharp-docs, dotnet-design-pattern-review),
  then final acceptance walkthrough against every criterion in VIX-2823.

## Surprises & Discoveries

- Observation: The Milestone 1 Esc handler (`NetworkListView_KeyDown`, checking
  `e.KeyCode == Keys.Escape`) never ran in manual testing — pressing Esc closed the whole Export
  Wizard instead of clearing the highlight, because the wizard dialog has a `CancelButton` wired to
  its Cancel action. WinForms treats Escape (along with Tab, Enter, and the arrow keys) as a "dialog
  key": `Control.PreProcessMessage` checks `IsInputKey` first, and for any key that a control's
  `IsInputKey` does not claim, the key is routed to `ContainerControl.ProcessDialogKey` — which walks
  up the parent chain and invokes the host form's `CancelButton.PerformClick()` for Escape — before
  the control's own `KeyDown` event ever fires. `Common.Controls.ListViewEx` (and the base
  `System.Windows.Forms.ListView`) does not override `IsInputKey` for Escape, so this stage's
  `networkListView` never saw the keystroke at all; Ctrl+A worked because Ctrl+A is not one of the
  keys WinForms treats specially this way.
  Evidence: Reported directly by manual testing — Ctrl+A and Space-bar toggle both worked as
  designed, but Esc closed the wizard instead of clearing the highlight.

- Observation: The first fix attempt for the above — a private `SelectionAwareListView` subclass of
  `Common.Controls.ListViewEx` overriding `IsInputKey` to claim Escape only while something was
  highlighted — compiled with 0 errors but did not fix the problem; Esc still closed the wizard in
  manual re-testing. `IsInputKey` only controls whether `ProcessDialogKey` gets consulted for a key,
  but WinForms' `Control.PreProcessMessage` checks `ProcessCmdKey` *before* it ever looks at
  `IsInputKey`. `ProcessCmdKey` bubbles starting from whatever control currently has focus, up through
  every ancestor `Control`/`ContainerControl`, independently of `IsInputKey`'s answer for any one
  control along that chain — so nothing in the pipeline was actually consulting `IsInputKey` before
  `ProcessCmdKey` (or something upstream of it) already decided Escape's fate. The `IsInputKey`
  override was removed and replaced with a `ProcessCmdKey` override directly on
  `BulkExportControllersStage` (see Decision Log), which intercepts at an earlier, more reliable
  point in the same bubble chain.
  Evidence: Reported directly by manual testing — "The Esc key still closes the wizard instead of
  clearing the selections" after the `IsInputKey`-based fix had already been built and deployed.

- Observation: Once `MultiSelect = true` (Milestone 1) and `CheckBoxes = true` (already set), the
  Space bar bulk-toggling behavior originally planned for Milestone 2 turned out to already exist as
  built-in `System.Windows.Forms.ListView` behavior — with no Milestone 2 code written yet, manual
  testing showed that pressing Space with several rows highlighted flips every highlighted row's
  checkbox together to one new state. The same native behavior also applies to the mouse: clicking
  directly on any one highlighted row's checkbox toggles every highlighted row's checkbox together,
  not just the row that was clicked.
  Evidence: Reported directly by manual testing before any Milestone 2 code existed — "Multiselect,
  Ctrl+A, Space Toggle of selected all work," and a follow-up confirmation that a mouse click on one
  checkbox within a highlighted group flips the whole group.

## Decision Log

- Decision (superseded — see the `ProcessCmdKey` decision immediately below for what actually
  shipped): Fix the Esc-closes-the-wizard problem by giving `networkListView` a private,
  stage-local subclass of `Common.Controls.ListViewEx` (`BulkExportControllersStage.SelectionAwareListView`)
  that overrides `IsInputKey` to claim Escape as a normal input key only when
  `SelectedItems.Count > 0`, rather than modifying the shared `ListViewEx`/`DragAndDropListView.cs`
  control or overriding `ProcessCmdKey` on the stage/dialog itself.
  Rationale: This keeps the earlier "shared control blast radius" decision intact — no behavior
  change lands in `DragAndDropListView.cs`, which other parts of the codebase also use — while still
  fixing the conflict. Claiming Escape only when something is highlighted means Esc keeps its
  original "cancel the wizard" meaning in the common case (nothing highlighted), and only takes on
  the new "clear the highlight" meaning when there is a highlight to clear, which is the least
  surprising resolution of the two competing meanings of the same key. This turned out not to work
  in manual re-testing — see Surprises & Discoveries — because `IsInputKey` only gates
  `ProcessDialogKey`, and WinForms consults `ProcessCmdKey` first, independently of `IsInputKey`.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Fix the Esc-closes-the-wizard problem by overriding `ProcessCmdKey` directly on
  `BulkExportControllersStage` (the `WizardStage`/`UserControl` itself, not `networkListView` or any
  subclass of it), consuming Escape (returning `true`) only when `networkListView.Focused &&
  networkListView.SelectedItems.Count > 0`, and clearing `SelectedItems` in that same branch before
  returning `true`. `NetworkListView_KeyDown`'s previous Escape branch was removed as dead code —
  once `ProcessCmdKey` consumes the key, `networkListView` never receives a `KeyDown` for it at all.
  Rationale: `ProcessCmdKey` bubbles from whichever control currently holds focus up through every
  ancestor `Control`/`ContainerControl` (including `BulkExportControllersStage`, since `UserControl`
  is itself a `ContainerControl`), and this bubbling happens *before* WinForms' `PreProcessMessage`
  ever calls `IsInputKey` or `ProcessDialogKey` on any control in the chain. Overriding it here
  intercepts Escape at the earliest point that is still specific to this stage (rather than, for
  example, overriding `ProcessCmdKey` on the shared `WizardForm`, which would affect every wizard in
  the codebase, not just this one), and does not require a custom `ListViewEx` subclass at all — the
  `Designer.cs` construction of `networkListView` reverts to plain `new Common.Controls.ListViewEx()`.
  The `networkListView.Focused` check preserves the original design intent that this behavior only
  applies while the list itself has keyboard focus (matching Milestone 4's regression check that
  focus elsewhere in the wizard is unaffected), rather than whenever a highlight merely happens to
  still exist while some other control has focus.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Reverse the earlier "clicking a single row's checkbox continues to affect only that
  row" decision. Accept native `ListView` behavior — clicking any checkbox within a highlighted
  group, or pressing Space, toggles the whole highlighted group together — rather than overriding
  mouse hit-testing to force single-row-only clicks.
  Rationale: This behavior is not something Vixen's code implements; it is `System.Windows.Forms
  .ListView`'s own default once `MultiSelect` and `CheckBoxes` are both `true`, discovered via manual
  testing before any Milestone 2 code was written. Suppressing it to force "a checkbox click only
  ever affects its own row" would mean intercepting and overriding low-level mouse processing
  (checkbox hit-testing) on `Common.Controls.ListViewEx`, a shared control, purely to produce a
  *less* convenient behavior than what the framework already gives for free, and one further away
  from the Explorer-style convention VIX-2823 asked for in the first place. The earlier concern
  that this would be "too implicit" is outweighed by it being the platform's own established
  convention (the same thing happens in Windows Explorer's checkbox-select mode) rather than a
  custom behavior Vixen would be inventing.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Narrow Milestone 2 to only implementing coalesced reindexing in
  `NetworkListView_ItemChecked`, instead of writing bespoke Space-key detection and a
  `SetCheckedForItems` toggle helper.
  Rationale: Since native `ListView` behavior already performs the actual bulk checkbox toggling
  (previous decision), the only remaining gap from the original specification is the "long list of
  controllers" performance concern: `ReIndexControllerChannels()` was, and still is, invoked once
  per `ItemChecked` event, and a native bulk toggle of many rows raises that event once per row.
  The fix implemented is to coalesce those events: `NetworkListView_ItemChecked` now sets a
  `_reindexPending` guard and defers the actual `ReIndexControllerChannels()` call via
  `BeginInvoke`, so every `ItemChecked` event raised synchronously within one user gesture (Space,
  a checkbox click across a highlighted group, or — per Milestone 3 — the "Enable All"/"Disable
  All" buttons) collapses into exactly one reindex pass after the current message finishes
  processing. This is simpler and lower-risk than intercepting and replacing the native toggle
  logic, and it generically covers every current and future source of bulk `Checked` changes on
  this list, not just Space.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Give `lblConfigureOutput` `Dock = System.Windows.Forms.DockStyle.Top` (removing its fixed
  `Location`), and add it to `Controls` *after* `tableLayoutMain`, rather than wrapping it in a
  separate `Panel`.
  Rationale: Milestone 3's plan text left this as an open choice. `tableLayoutMain` is
  `Dock = Fill`, which would otherwise cover the entire stage including the space the label used to
  occupy at its old fixed `Location`. WinForms docks controls in the reverse of their `Controls.Add`
  order — the last control added claims its `Dock` space first — so adding `tableLayoutMain` first and
  `lblConfigureOutput` last means the label's `Dock = Top` claims a strip at the top before
  `tableLayoutMain`'s `Dock = Fill` claims everything else beneath it, with no overlap and no extra
  wrapper `Panel` control needed.
  Date/Author: 2026-07-03 / Jeff Uchitjil

- Decision: Add a small private `SetAllChecked(bool isChecked)` helper in `BulkExportControllersStage.cs`,
  called by both `BtnEnableAll_Click` and `BtnDisableAll_Click`, instead of writing the same
  `BeginUpdate`/loop-over-`Items`/`EndUpdate` logic twice as Milestone 3's plan text literally
  described.
  Rationale: The two button handlers are otherwise identical except for the boolean they set every
  row's `Checked` to; a two-line difference does not justify duplicating a four-line loop. Behavior
  is unchanged from what the plan specified, and it still relies entirely on Milestone 2's
  `_reindexPending`/`BeginInvoke` coalescing in `NetworkListView_ItemChecked` with no further changes
  needed there.
  Date/Author: 2026-07-03 / Jeff Uchitjil

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

- Decision (superseded — see the "Reverse the earlier..." and "Narrow Milestone 2..." decisions
  above, dated 2026-07-03 later the same day, for what actually shipped): The Space bar toggles
  every highlighted row to the opposite of the *focused* row's current checked state (not a per-row
  toggle, not a majority vote), where "focused row" means whichever row currently has the dotted
  focus rectangle (`ListView.FocusedItem` in WinForms terms — the one row within a highlighted group
  that last received keyboard/mouse focus).
  Rationale: A highlighted group can contain a mix of already-checked and already-unchecked rows.
  Picking one well-defined row (the focused one) as the source of truth gives a single, predictable
  outcome every time, instead of ambiguous behavior when the group is mixed. This reasoning turned
  out to be moot: manual testing during Milestone 1 showed native `ListView` behavior already
  bulk-toggles the highlighted group (see Surprises & Discoveries), so no bespoke toggle logic of
  any kind — focused-row-based or otherwise — was implemented.
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

A native WinForms behavior worth knowing about before touching this list: once `CheckBoxes = true`
and `MultiSelect = true` are both set (the latter is what Milestone 1 changes), pressing the Space
bar, or clicking directly on any highlighted row's checkbox, toggles the checkbox of every currently
highlighted row together — not just the one row that was clicked or focused. This is entirely
built-in `System.Windows.Forms.ListView` behavior; Vixen's code does not implement it and does not
need to. This was discovered by manual testing during Milestone 1 (see Surprises & Discoveries) and
means Milestone 2 does not need to write any bespoke Space-key or checkbox-click handling — see
Milestone 2 for what its scope actually became once this was known.

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

Add a new private method `NetworkListView_KeyDown(object sender, KeyEventArgs e)` that handles one
case:

- If `e.KeyCode == Keys.A && e.Control` (Ctrl+A): call `networkListView.BeginUpdate()`, loop over
  every `ListViewItem` in `networkListView.Items` and set `.Selected = true` on each, call
  `networkListView.EndUpdate()`, and set `e.SuppressKeyPress = true` so the "ding" sound WinForms
  otherwise makes for unhandled Ctrl+A does not play.

Escape is deliberately *not* handled inside `NetworkListView_KeyDown` — see below for why, and see
Surprises & Discoveries for a first attempt (a custom `ListViewEx` subclass overriding `IsInputKey`)
that compiled but did not actually work.

Instead, override `ProcessCmdKey` directly on `BulkExportControllersStage`:

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape && networkListView.Focused && networkListView.SelectedItems.Count > 0)
        {
            networkListView.SelectedItems.Clear();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

WinForms treats Escape as a "dialog key" — like Tab, Enter, and the arrow keys — and, before a
keystroke is delivered as an ordinary `KeyDown` event, `Control.PreProcessMessage` first calls
`ProcessCmdKey` starting at whichever control currently has keyboard focus, then walks up through
every ancestor `Control`/`ContainerControl` in turn, each getting a chance to consume the key by
returning `true` from its own `ProcessCmdKey` override. Only if none of them do does WinForms move on
to check `IsInputKey`/`ProcessDialogKey` — which is the stage where the host `WizardForm`'s
`CancelButton` (wired in `WizardForm.Designer.cs` via `this.CancelButton = this.buttonCancel;`) would
otherwise get invoked for Escape. Because `BulkExportControllersStage` is a `WizardStage`, and
`WizardStage`/`UserControl` is itself a `ContainerControl`, it sits directly in that bubble chain
between `networkListView` and the `WizardForm` — overriding `ProcessCmdKey` here intercepts Escape
before `CancelButton` ever gets a chance to see it, whenever the list has focus and something is
highlighted. When nothing is highlighted, this override returns `false` (falls through to
`base.ProcessCmdKey`), so Escape continues bubbling up and cancels the wizard exactly as it always
has — the two meanings of the key stay mutually exclusive by construction, without needing the list
control itself to know anything special about Escape. No changes to `Common.Controls.ListViewEx` or
`DragAndDropListView.cs` are needed for this.

Space-bar handling is intentionally *not* implemented in this milestone or in Milestone 2 — see
Milestone 2 below for why.

**Acceptance for this milestone:** Build the solution (see Concrete Steps below), launch Vixen, open
the Export Wizard against any profile with at least four or five controllers configured, and advance
to Step 3. Click the first row, then hold Shift and click the fourth row: rows one through four
should highlight (blue background) together. Click a row, then Ctrl+Click two other non-adjacent
rows: all three should stay highlighted, with earlier ones remaining highlighted (this is standard
`MultiSelect = true` behavior — no code changes beyond the Designer.cs flag are needed to produce it,
which is itself a fact worth confirming by observation since it demonstrates the framework is doing
the work for free). With several rows highlighted, press Ctrl+A: every row in the list should become
highlighted. Press Esc with rows highlighted: the highlight should clear completely, the wizard
should stay open, and every row's checkbox should be unchanged. Press Esc again with nothing
highlighted: the wizard should close/cancel as it always has.

**Status: implemented and manually verified** (2026-07-03) — Ctrl+A, multi-select, and Esc (clearing
the highlight without closing the wizard) all confirmed working in the running application. Escape
went through two implementation attempts: the first (`IsInputKey` override) built cleanly but did not
fix the problem; the second (`ProcessCmdKey` override, described above) is the one that shipped and
was confirmed working by manual re-test. Committed as `6a8261150` ("VIX-2823 Enable multi-select and
bulk toggle on export list"), which also includes Milestone 2's coalescing.

### Milestone 2 — Bulk checkbox toggle batching

**Scope:** This milestone's scope changed from what was originally planned. It no longer implements
Space-bar detection or a bulk-toggle helper method, because manual testing during Milestone 1 (before
any Milestone 2 code existed) revealed that `System.Windows.Forms.ListView` already implements bulk
checkbox toggling natively once `MultiSelect` and `CheckBoxes` are both `true`: pressing Space with
several rows highlighted flips every highlighted row's checkbox together, and clicking directly on
any one highlighted row's checkbox does the same. See Surprises & Discoveries and the "Reverse the
earlier..." Decision Log entry for the full reasoning. What remains from the original specification's
"long list of controllers" performance concern is that `ReIndexControllerChannels()` still runs once
per `ItemChecked` event, and a native bulk toggle of many rows raises that event once per row — so
this milestone's actual scope is coalescing those events into a single reindex pass.

**What to edit:**

In `BulkExportControllersStage.cs`, add a private field:

    private bool _reindexPending;

and change the existing `NetworkListView_ItemChecked` method from calling `ReIndexControllerChannels()`
directly to deferring it:

    private void NetworkListView_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
        if (_reindexPending)
        {
            return;
        }

        _reindexPending = true;
        BeginInvoke(new Action(() =>
        {
            _reindexPending = false;
            ReIndexControllerChannels();
        }));
    }

`Control.BeginInvoke` (available here because `BulkExportControllersStage` is a `UserControl` via
`WizardStage`) queues the given delegate to run on the UI thread after the current message finishes
being processed. Because a native bulk-toggle gesture (Space, or a checkbox click across a
highlighted group) raises all of its `ItemChecked` events synchronously, one after another, within
the same message before returning control to the message loop, the `_reindexPending` guard ensures
only the *first* of those events schedules a `ReIndexControllerChannels()` call; every subsequent
event in the same burst sees `_reindexPending == true` and returns immediately. The queued call then
runs once, after every row in the burst has already been updated, producing exactly one reindex pass
no matter how many rows were toggled together. This also transparently covers Milestone 3's "Enable
All"/"Disable All" buttons, and any other future code that sets many rows' `Checked` property in a
loop — nothing about this coalescing is specific to Space or to mouse clicks.

**Acceptance for this milestone:** Rebuild and relaunch. On Step 3, highlight three or more rows and
press Space: confirm every highlighted row's checkbox flips together, and the Start/End channel
columns are correct afterward (this is the existing native behavior from Milestone 1's `MultiSelect`
change, being re-confirmed here). With a long list of controllers (a profile with 15-20+ controllers
is a good stress case), toggle all of them at once via Shift+Click-all-then-Space (or Ctrl+A then
Space) and confirm the UI does not noticeably stutter or lag — this is the direct test of the
coalescing behavior added in this milestone, versus the pre-existing per-row reindex. Click a single
row's checkbox with nothing else highlighted: confirm it still behaves exactly as it did before this
plan (only that row's state changes, and channel numbers are still correct).

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
each of which loops over `networkListView.Items.Cast<ListViewItem>()` (every row, not just the
highlighted ones) setting `.Checked = true` or `.Checked = false` respectively, wrapped in
`networkListView.BeginUpdate()`/`EndUpdate()`. No separate toggle helper is needed here: setting
`Checked` on each item still raises `ItemChecked` per row exactly as a native bulk toggle does, so
Milestone 2's `_reindexPending`/`BeginInvoke` coalescing in `NetworkListView_ItemChecked` already
collapses this into a single `ReIndexControllerChannels()` call with no further changes required to
that method.

Per the CLAUDE.md instruction to update XML documentation for any modified public or protected API:
`BulkExportControllersStage` itself has no public members changing shape in this plan (the new
buttons and handlers are all private implementation details of this stage), so no XML doc updates are
expected to be needed; confirm this remains true once the milestone's code is written before marking
it complete, using the `csharp-docs` skill.

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
8. Highlight a group of rows and press Space (or click directly on one highlighted row's checkbox):
   confirm every row in the group's checkbox flips together to one new state (native `ListView`
   behavior, re-confirmed after Milestone 1's `MultiSelect` change), and the Start/End channel
   columns are correct afterward with only a single reindex pass (Milestone 2's coalescing).
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

    private bool _reindexPending;
    private void NetworkListView_KeyDown(object sender, KeyEventArgs e)
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    private void BtnEnableAll_Click(object sender, EventArgs e)
    private void BtnDisableAll_Click(object sender, EventArgs e)

`NetworkListView_ItemChecked` (pre-existing) changes body to defer `ReIndexControllerChannels()` via
`_reindexPending` and `BeginInvoke` rather than calling it directly — see Milestone 2. No
`SetCheckedForItems` helper was introduced; the plan originally anticipated one, but Milestone 2's
rescoping (see Decision Log) made it unnecessary. No custom `ListViewEx` subclass was introduced
either — a first attempt at one (`SelectionAwareListView`, overriding `IsInputKey`) was tried and
removed after it turned out not to fix the Escape problem; see Surprises & Discoveries and the
Decision Log.

In `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.Designer.cs`, four new private
fields exist in addition to what is there today:

    private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutButtons;
    private System.Windows.Forms.Button btnEnableAll;
    private System.Windows.Forms.Button btnDisableAll;

`networkListView`'s existing field declaration, type (`Common.Controls.ListViewEx`), and construction
(`new Common.Controls.ListViewEx()`) do not change — only its parent container and positioning
properties (`Dock = Fill` inside `tableLayoutMain` instead of a fixed `Location`/`Size`/`Anchor` on
the stage) change, per Milestone 3.

No project references, NuGet packages, or `Vixen.sln` changes are required — this plan only edits two
existing files in an existing project. `TableLayoutPanel` and `FlowLayoutPanel` are both part of the
`System.Windows.Forms` namespace already referenced by this project; no new `using` directives beyond
what `BulkExportControllersStage.Designer.cs` already needs for `Button`/`ListView` are required.

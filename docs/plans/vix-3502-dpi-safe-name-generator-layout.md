# Make the Name Generator dialog DPI-safe (VIX-3502)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

At Windows display scaling above 100%, the Name Generator dialog can allow the generated-name preview to overlap the item-count input and can make the selected naming-rule editor appear to disappear after a count refresh. After this change, the configuration, preview, and command areas will occupy explicit container cells that resize relative to one another. The selected editor will fill a dedicated, scrollable host rather than rely on a coordinate within the dialog.

A user can demonstrate the result by opening Create/Modify Multiple Items at 100%, 125%, 150%, and 200% scaling, selecting each naming-rule type, changing the item count repeatedly, and resizing the dialog. The item-count input, rule editor, preview, and OK/Cancel buttons must all remain visible, reachable, and non-overlapping while preview columns continue to resize.

## Progress

- [x] (2026-08-04) Research complete: confirmed that `numericUpDownItemCount_ValueChanged` calls only `PopulateNames`, `DisplayNamingGenerator` creates the editor in `panelRuleConfig`, and the current form and all four hosted editors use absolute coordinates.
- [x] (2026-08-04) Milestone 1: Updated VIX-3502 with the verified layout-only cause, nested table/flow-layout implementation boundary, eight Given/When/Then acceptance criteria, and DPI/mode/theme validation plan. Confirmed status remained `Accepted`; no transition was performed.
- [x] (2026-08-04) Milestone 2: Replaced the form-level and naming-rule absolute layout with named table/flow containers; the 45%/55% content split, auto-sized footer, fill-docked preview/rule regions, hidden-template footer cell, and scrollable rule host are now in `NameGenerator.Designer.cs`. `msbuild src/Vixen.Common/Controls/Controls.csproj -m -t:restore -t:Rebuild -p:Configuration=Debug` succeeded with 0 errors and 4 existing Vixen.Core warnings.
- [x] (2026-08-04) Milestone 3: Set dynamically selected `NameGeneratorEditor` controls to `DockStyle.Fill` before adding them to `panelRuleConfig`, and converted Numeric Counter, Letter Counter, Letter Iterator, and Word Iterator editor designers to dock-filled table layouts. `msbuild src/Vixen.Common/Controls/Controls.csproj -m -t:Rebuild -p:Configuration=Debug` succeeded with 0 errors and the same 4 existing Vixen.Core warnings.
- [ ] Milestone 4: Build, run existing automated tests, manually validate all required DPI and dialog modes, then record results in JIRA and this document.

## Surprises & Discoveries

- Observation: The reported disappearing editor is not a naming-state transition.
  Evidence: `numericUpDownItemCount_ValueChanged` in `src/Vixen.Common/Controls/NameGeneration/NameGenerator.cs` contains only `PopulateNames();`; `PopulateNames` updates `Names` and `listViewNames` but does not modify `panelRuleConfig` or the selected rule.

- Observation: The current editor host has no automatic containment of its child control.
  Evidence: `DisplayNamingGenerator` clears `panelRuleConfig` and adds a `NameGeneratorEditor` without setting its `Dock`, while `panelRuleConfig` has a fixed designer `Location` and `Size`.

- Observation: `SubstitutionRenamer` is a useful local layout precedent, but it does not itself meet every requirement of this issue.
  Evidence: `SubstitutionRenamer.Designer.cs` uses a root `TableLayoutPanel` with percentage content columns and an automatic footer row. VIX-3502 additionally requires a scrollable rule-editor host and flow layouts for icon-button groups.

- Observation: The original report screenshot is not recoverable from VIX-3502.
  Evidence: the issue history names `image-20231214-223634.png`, but the current Jira attachment collection is empty and the historical media URL returns “File not found.” The broken inline reference was removed on 2026-08-04 while retaining the written problem report.

- Observation: The focused Controls Debug build succeeds after the designer rewrite, but its dependency Vixen.Core has four unrelated legacy warnings.
  Evidence: `msbuild src/Vixen.Common/Controls/Controls.csproj -m -t:restore -t:Rebuild -p:Configuration=Debug` completed with 0 errors and warnings CS8632 (twice), CS0618, and CS0067 in Vixen.Core files; no warning names `NameGenerator` or its designer.

- Observation: The rule-type selector did not update an existing selected generator.
  Evidence: `comboBoxRuleTypes_SelectedIndexChanged` only enabled Add, while `listViewGenerators_SelectedIndexChanged` displayed the selected generator without synchronizing the selector. A user could add a correctly typed generator but changing the selector for an existing entry left its editor unchanged.

- Observation: Fill-docked editor tables need an explicit remaining-height row.
  Evidence: without a percentage filler row, the available rule-host height was distributed between editor input rows, separating Sequential Letters fields and placing Sequential Numbers' Step row below its aligned controls.

## Decision Log

- Decision: Treat VIX-3502 as a container-only WinForms maintenance change.
  Rationale: The defect follows absolute positioning and mixed anchors. Existing naming algorithms, constructor contracts, theming, and preview update behavior are not implicated. A WPF/Catel migration or application-wide DPI-policy change would expand scope without addressing the local structural defect.
  Date/Author: 2026-08-04 / Codex

- Decision: Use an approximately 45% configuration and 55% preview split in a percentage-based content table.
  Rationale: The preview needs more horizontal space for generated names, while percentage columns prevent either side from depending on a scaled pixel coordinate.
  Date/Author: 2026-08-04 / Codex

- Decision: Preserve `AutoScaleMode.Font` and the existing public/property contracts.
  Rationale: Font scaling is the dialog's established DPI behavior. `Names`, `FixedCount`, `OldNames`, `SelectedGroupName`, templates, generators, constructor behavior, and dialog results are not part of the layout defect.
  Date/Author: 2026-08-04 / Codex

- Decision: Make `panelRuleConfig` scrollable and dock the runtime editor into it.
  Rationale: The host is the final containment boundary for all four editor controls. Docking prevents an editor from retaining a stale scaled size; scrolling preserves access when unusually large system fonts exceed the available height.
  Date/Author: 2026-08-04 / Codex

- Decision: Treat a user-selected rule type as a replacement for the selected generator.
  Rationale: The type determines the editor UI and name-generation implementation. Synchronizing the selector when list selection changes prevents accidental replacement; a later user selection creates a default instance of the requested type, updates the list entry and editor, and preserves the existing placeholder position in the name format.
  Date/Author: 2026-08-04 / Codex

- Decision: Preserve the rule-type selector as an add-only control and revert the replacement behavior.
  Rationale: The user confirmed that changing a rule requires removing the existing generator and adding a new one of the selected type. The selector must therefore enable Add only; it must not synchronize with list selection, mutate a selected generator, or replace its editor.
  Date/Author: 2026-08-04 / Codex

- Decision: Give each dock-filled editor table a final 100% filler row.
  Rationale: Input rows remain AutoSize and their labels/inputs stay aligned, while the unused host height is confined to the blank row below the settings. This keeps the root table docked to its host without coordinate positioning.
  Date/Author: 2026-08-04 / Codex

## Outcomes & Retrospective

Milestones 1 through 3 are complete. The issue records the verified local layout scope; the main dialog uses table/flow containment; and every hosted editor fills the scrollable rule host through an internal table layout. Milestone 4 remains for full test-suite, application, DPI, mode, theme, and mixed-monitor validation.

## Context and Orientation

`src/Vixen.Common/Controls/NameGeneration/NameGenerator.cs` implements the Create/Modify Multiple Items WinForms dialog. It maintains a private list of `INamingGenerator` objects and produces generated names in `PopulateNames`. The overload taking existing names supports rename mode; the overload taking a count supports fixed-count mode; the default dialog permits editing the count. These behavior modes must remain unchanged.

`src/Vixen.Common/Controls/NameGeneration/NameGenerator.Designer.cs` currently puts the item-count controls, name-format controls, naming-rule group, preview headers/list, hidden template controls, and buttons directly on the form. It also positions the naming-rule selector, list, move buttons, and editor host independently inside `groupBoxSelectedNamingRule`. These coordinate relationships are the defect boundary.

`panelRuleConfig` is the panel in the naming-rule group that hosts a `NameGeneratorEditor` selected by `DisplayNamingGenerator`. The concrete editors are `NumericCounterEditor`, `LetterCounterEditor`, `LetterIteratorEditor`, and `WordIteratorEditor`, each under `src/Vixen.Common/Controls/NameGeneration`. Their designers also use fixed locations, so they must be converted as part of the visible layout hierarchy.

`TableLayoutPanel` divides available space into named rows and columns. Auto-sized rows take the preferred height of their children; percentage rows share the remaining height. `FlowLayoutPanel` positions a short run of controls in one direction while preserving their individual sizes. Use these controls to express every relationship that changes with font or DPI scaling. Fixed dimensions remain appropriate for the add/delete and move icon buttons themselves, but not for their parent positions.

Callers in `src/Vixen.Common/Controls/ElementTree.cs`, `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementTreeViewModel.cs`, and `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementOrderViewModel.cs` construct this dialog through its existing constructors. They require no changes.

## Plan of Work

### Milestone 1 — Record the implementation contract in VIX-3502

Use the repository's JIRA workflow to update VIX-3502 with the verified cause and final contract from this plan. State that at 125% and 150% scaling the preview can overlap the count input and the editor can appear to disappear after a count change, but the count-change handler only repopulates the preview. State that the fix is a nested WinForms table/flow layout, not a naming-logic or DPI-policy change. Include the acceptance and manual validation scenarios from this plan. Keep the issue status as-is unless the project workflow explicitly requires a transition.

Acceptance is that a contributor reading VIX-3502 receives the same scope boundaries, layout design, and validation expectations as this plan.

### Milestone 2 — Give the main dialog explicit resizing boundaries

In `src/Vixen.Common/Controls/NameGeneration/NameGenerator.Designer.cs`, replace form-level placement with descriptively named layout fields. Use a root `mainLayoutPanel` docked to `Fill`, padded uniformly, with two rows: a 100% content row and an AutoSize footer row. Preserve the form's existing `AutoScaleMode.Font`, `AcceptButton`, `CancelButton`, title, load event, double buffering, and a minimum size that remains large enough for the scaled input rows, two primary columns, and footer. Keep the logical baseline minimum size near the existing `715 x 684`; verify the designer's font scaling increases it at larger DPI rather than substituting unscaled coordinates.

Place a `contentLayoutPanel` in the content row with two percentage columns: configuration at 45% and preview at 55%. Both columns must be `DockStyle.Fill` within their cells. Do not set anchors or form-relative locations for these expanding regions.

Make `configurationLayoutPanel` a one-column table in the left content cell. Its item-count row, name-format row, and example row must be AutoSize. Put each label and its input in a small two-column nested table or a horizontal flow, so labels remain adjacent to their own inputs. The count's numeric control must retain its current minimum, maximum, value, enabled-state behavior, and `ValueChanged` event. The name-format box must fill its input cell and retain `TextChanged`. Place `groupBoxSelectedNamingRule` in the final 100% row, docked to fill its cell.

Make `previewLayoutPanel` a one-column table in the right content cell. Its header row contains a two-column header table holding `labelColumnHeader1` and `labelColumnHeader2`; use equal percentage columns and preserve the constructor code that changes their text for rename mode. Put `listViewNames` in the remaining 100% row and dock it to fill. Preserve its columns, `Resize` event, view settings, and `ResizeListviewColumns` calculation. This guarantees the preview list can only occupy the preview cell.

Place a `footerLayoutPanel` in the root footer row. Its left auto-sized region contains the existing hidden `label2` and `comboBoxTemplates` in a small table or flow. Preserve their `Visible = false`, data binding, and selection event so their layout collapses when hidden. Its right region contains an AutoSize, right-to-left `FlowLayoutPanel` with `buttonCancel` and `buttonOk`; preserve dialog results and the existing button dimensions. The footer must not depend on a bottom-right coordinate.

Inside `groupBoxSelectedNamingRule`, add `rulesLayoutPanel`: a two-column table with a 100% working column and an AutoSize move-button column. Its first row is AutoSize and contains the rule-type selector plus a left-to-right `FlowLayoutPanel` containing add and delete buttons. Its second row is a percentage row containing `listViewGenerators` in the working column and a top-down `FlowLayoutPanel` containing the move-up and move-down buttons in the second column. Its final percentage row spans both columns and contains `panelRuleConfig`, docked to fill. Split the two percentage rows evenly unless a layout inspection shows that an editor's preferred height requires a different documented ratio; in either case both rows must receive remaining height rather than fixed pixels. Preserve all existing list, combo box, paint, owner-draw, and button click events. Replace obsolete `panel1` and `panel2` fields with the named flow panels; do not retain empty coordinate panels.

Set `panelRuleConfig.AutoScroll = true`, apply modest padding/margins on containers and child controls rather than blank coordinates, and avoid assigning a scaled `Location` to any expanding child. Keep icon buttons fixed at their current logical sizes so the existing scaled icon images remain visually appropriate.

Acceptance is that every primary region is contained by a table cell: configuration cannot be painted by the preview list, the rule editor cannot leave its host, and the footer remains reachable when the form is resized.

### Milestone 3 — Make hosted editors fill a responsive host

In `DisplayNamingGenerator` in `src/Vixen.Common/Controls/NameGeneration/NameGenerator.cs`, preserve the current type selection, `DataChanged` subscription, button-enable rules, and clear/add sequence. Immediately before adding a non-null `NameGeneratorEditor`, set `newControl.Dock = DockStyle.Fill`. Add `using System.Windows.Forms;` only if the file does not already have access to `DockStyle`; do not change any public or protected API, so XML documentation work is not required.

In each designer below, replace absolute child coordinates with a single root `TableLayoutPanel` docked to `Fill`, padded consistently, using AutoSize rows and columns for labels and compact inputs. Retain `AutoScaleMode.Font`, control names, text, defaults, limits, tab order, load events, and all change handlers. The editors may keep a sensible designer baseline size, but their runtime layout must be governed by the root table and the parent's docked host.

- `NumericCounterEditor.Designer.cs`: use label/input columns for Start Number, End Number, and Step. Put the Endless checkbox in the End Number row after its numeric input, or span it across remaining columns. Preserve its enabled-state event behavior.
- `LetterCounterEditor.Designer.cs`: use two AutoSize label/input rows for Start Letter and Steps, with inputs sized for their short values rather than a position-dependent width.
- `LetterIteratorEditor.Designer.cs`: use AutoSize rows for the instruction and example labels and a percentage-width row for `textBoxLetters`, which docks/fills horizontally.
- `WordIteratorEditor.Designer.cs`: use the same instruction/text/example structure, with `textBoxWords` filling the table's text row.

Do not alter the four `.cs` files' generator mutation behavior or change editor text, including existing spelling, as part of this DPI-only issue. Do not alter resource files.

Acceptance is that selecting each supported generator creates an editor that occupies `panelRuleConfig.ClientRectangle`, respects the host scrollbar at large fonts, and remains visible after repeated preview refreshes.

### Milestone 4 — Validate behavior and record the result

First build the changed controls project and run the repository test suite. The issue is a visual WinForms layout defect and the dialog has no existing lightweight visual-layout test seam, so do not add brittle pixel-coordinate unit tests merely to create coverage. Preserve and run existing automated tests; manual validation at multiple display scales is the required regression proof.

Run the Debug application and exercise all four generators in default editable-count mode, fixed-count mode, and rename mode. At each required scaling level, repeatedly change the count after selecting a rule, resize to the minimum and then substantially enlarge the dialog, and move the dialog between monitors with different scaling when available. Check light and dark themes. Use the behavior in Validation and Acceptance as the test record. If a second monitor or theme is unavailable, record that environmental limitation explicitly rather than claiming the scenario passed.

Update the plan's Progress, Discoveries, and Outcomes sections with actual build/test/manual results. Make a final VIX-3502 description adjustment only if implementation changed requirements, then add a JIRA comment with the build and test commands/results, tested scaling levels, modes, themes, and any environmental limitations. Do not transition the issue unless required.

When a milestone that changes repository files completes, provide the proposed commit message in the completion response using the repository's `commit-msg` skill. Do not create a commit unless explicitly requested.

## Concrete Steps

Run all commands from the repository root, `C:\Dev\Vixen`.

1. Before editing, confirm the relevant source and working tree:

       git status --short
       rg -n -C 5 "DisplayNamingGenerator|PopulateNames|numericUpDownItemCount_ValueChanged|panelRuleConfig" src/Vixen.Common/Controls/NameGeneration/NameGenerator.cs src/Vixen.Common/Controls/NameGeneration/NameGenerator.Designer.cs
       rg -n "Location =|Anchor =" src/Vixen.Common/Controls/NameGeneration/*Editor.Designer.cs

2. Implement Milestone 2 in `NameGenerator.Designer.cs`. Maintain tabs and LF line endings according to `src/.editorconfig`. Keep `InitializeComponent`'s `SuspendLayout`/`ResumeLayout` calls aligned with every added layout container. Remove old fields only after every existing control has been placed in its new parent.

3. Implement Milestone 3. Confirm by inspection that `newControl.Dock = DockStyle.Fill` occurs before `panelRuleConfig.Controls.Add(newControl)` and every listed editor designer has a dock-filled root `TableLayoutPanel`.

4. Build the focused project:

       msbuild src/Vixen.Common/Controls/Controls.csproj -m -t:restore -t:Rebuild -p:Configuration=Debug

   Expected result: the Controls project compiles with no errors introduced by the designer or `DockStyle` change. Investigate all new warnings or errors before proceeding.

5. Run the repository test suite:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --nologo

   Expected result: zero failed tests. Record actual passed/skipped counts rather than predicting them.

6. Build the application for manual verification:

       msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

   Expected result: the build completes with no errors attributable to VIX-3502. If unrelated local SDK or native dependencies prevent a solution build, retain the focused Controls build evidence and document the exact blocker.

7. Launch the Debug application output and follow Validation and Acceptance. Keep a record of each scale, mode, rule editor, theme, and resize result in this plan and the final JIRA comment.

## Validation and Acceptance

The change is accepted only when all of the following are true:

1. Given the Name Generator at 100%, 125%, 150%, and 200% Windows display scaling, when the dialog opens and the item count changes, then the item-count controls, configuration area, and preview list do not overlap.

2. Given a selected Numeric Counter, Letter Counter, Letter Iterator, or Word Iterator rule, when the item count changes repeatedly, then the selected editor remains visible, remains inside the rule-editor host, and remains usable.

3. Given the dialog at its minimum size and at a substantially enlarged size, when the user resizes it, then the configuration and preview columns resize relationally, the preview list remains in the preview cell, and the footer buttons remain reachable.

4. Given unusually large system fonts or an editor taller than the remaining rule-host space, when the editor is selected, then the rule-editor host exposes scrolling rather than clipping or hiding the editor.

5. Given editable-count, fixed-count, and rename construction modes, when the dialog is opened, then the count's existing enabled/value behavior, old/new preview headers, generated names, constructor contracts, and OK/Cancel dialog results are unchanged.

6. Given a preview with one or two visible columns, when the preview width changes, then `ResizeListviewColumns` continues assigning equal column widths based on the list client width, scrollbar width, and existing border allowance.

7. Given the dialog is moved between monitors with different scaling, when Windows relayouts it, then no primary control becomes inaccessible, overlaps another control, or leaves its container. If mixed-DPI monitors are unavailable, record the scenario as blocked by environment rather than passed.

8. Given both light and dark themes, when the dialog and group box paint, then existing theme rendering and owner-drawn generator selection remain intact.

9. The focused Controls Debug build and `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --nologo` complete without failures attributable to this change.

## Idempotence and Recovery

This work changes only WinForms layout and the dock setting of runtime editor controls. It does not change naming templates, serialization, user profiles, generated-name algorithms, or caller contracts. Rebuilding and rerunning tests are safe. If the layout must be backed out, revert only the VIX-3502 commit(s); no data migration or restoration is required. Use a disposable element set for manual rename validation so a canceled dialog cannot affect production sequence data.

## Artifacts and Notes

Populate this section during implementation with concise evidence, for example:

    Focused Controls Debug build: Passed / Failed / Blocked (include the actual result).
    Full test suite: Passed / Failed / Blocked (include actual passed, failed, and skipped counts).
    Full Debug solution build: Passed / Failed / Blocked (include the exact environmental blocker if any).
    DPI manual verification: 100% / 125% / 150% / 200% outcomes.
    Mode and editor verification: editable/fixed/rename plus Numeric/Letter Counter/Letter Iterator/Word Iterator outcomes.
    Theme and mixed-DPI verification: light/dark and monitor-move outcomes or explicit environment limitations.
    JIRA: VIX-3502 description updated during Milestone 1; status remained `Accepted` and no transition was performed. The unrecoverable historical screenshot reference was removed on 2026-08-04 to prevent a failed-media error.

    Focused Controls Debug build: Passed with 0 errors. Four unrelated warnings originate in Vixen.Core: CS8632 (two instances), CS0618, and CS0067.

    Focused Controls Debug build after Milestone 3: Passed with 0 errors and the same four unrelated Vixen.Core warnings.

    Rule-type replacement experiment: reverted on 2026-08-04 after user clarification. The rule-type selector is add-only and does not synchronize with or modify the selected generator.

    Editor compact-row correction: added final percentage filler rows to all four hosted editor tables and left Numeric Counter inputs left-aligned with their labels. Focused Controls Debug build passed with 0 errors and the same four unrelated Vixen.Core warnings.

The research checkpoint found a clean working tree. No production source files were changed while creating this plan.

## Interfaces and Dependencies

No new projects, packages, serialized data, configuration, public/protected APIs, or XML documentation are required.

The only runtime code change is local to the existing private `DisplayNamingGenerator(INamingGenerator generator)` method in `src/Vixen.Common/Controls/NameGeneration/NameGenerator.cs`: a non-null existing `NameGeneratorEditor` must have `DockStyle.Fill` before being added to the existing `panelRuleConfig`. `NameGeneratorEditor`, `INamingGenerator`, and the four existing editor controls retain their names, constructors, events, and data-change semantics.

The affected project is `src/Vixen.Common/Controls/Controls.csproj`. Existing callers in `ElementTree` and CustomPropEditor retain their current `NameGenerator` constructor calls and `DialogResult` handling.

---

Plan created 2026-08-04 from the VIX-3502 Sol handoff and a current-code review. The plan confirms the defect is structural layout behavior, not rule-editor visibility or name-generation logic, and records the local `SubstitutionRenamer` table-layout precedent.

Revised 2026-08-04 after Milestone 1: recorded the VIX-3502 description update and its unchanged `Accepted` status.

Revised 2026-08-04: recorded removal of the unrecoverable historical image reference from VIX-3502's description.

Revised 2026-08-04 after Milestone 2: recorded the designer container-layout implementation and focused Controls Debug-build result.

Revised 2026-08-04 after Milestone 3: recorded docked runtime editors, the four editor-layout conversions, and focused Controls Debug-build result.

Revised 2026-08-04: recorded and corrected the existing-generator rule-type selection contract discovered during validation.

Revised 2026-08-04: reverted the rule-type replacement behavior after user clarification that the selector is add-only.

Revised 2026-08-04: recorded the compact editor-row correction discovered during visual validation.

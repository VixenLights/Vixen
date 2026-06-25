1# Add Range Selection to State Assignment Tree

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

The State Property Setup dialog currently lets a user assign elements to one State item by checking individual nodes in an assignment tree. That is workable for a few elements, but it is slow when a prop has many neighboring elements that should be assigned or removed together. After this change, a user can use conventional range-selection gestures in the Assigned Elements tree, then toggle the checked state for every selected assignment node in one action.

The user-visible proof is in Display Setup. Open a State property, select exactly one State item row, click an assignment-tree node, Shift-click another visible assignment-tree node, and observe the whole visible range selected. Toggle the range and observe each selected node invert its assignment: unchecked nodes become checked, checked nodes become unchecked, and existing group-selection rules still clear and disable descendants when a group is checked.

The correct identifier for this enhancement is VIX-3591. The source requirement document is `docs\state\vix-3591-state-assignment-enhancement.md`.

## Progress

- [x] (2026-06-25 00:00 America/Chicago) Read `.agents\PLANS.md`.
- [x] (2026-06-25 00:00 America/Chicago) Read `docs\state\vix-3591-state-assignment-enhancement.md`, the available assignment enhancement source document.
- [x] (2026-06-25 00:00 America/Chicago) Read the VIX-3591 state requirements and plans under `docs\state` and `docs\plans\state` for stable identity, State definitions, preview, State item grid multi-select, and manual reorder context.
- [x] (2026-06-25 00:00 America/Chicago) Read project skills `dotnet-best-practices`, `catel-mvvm`, `csharp-docs`, and `dotnet-design-pattern-review`.
- [x] (2026-06-25 00:00 America/Chicago) Inspected current State assignment tree implementation, State mapper XAML, row multi-select behavior, and existing State tests.
- [x] (2026-06-25 00:00 America/Chicago) Created this initial implementation plan.
- [x] (2026-06-25 00:00 America/Chicago) Confirmed the correct identifier is VIX-3591 and renamed this plan accordingly.
- [x] (2026-06-25 00:00 America/Chicago) Confirmed range toggling should use an explicit Space / Toggle Selected action unless implementation research finds strong evidence of a broader accepted application convention.
- [x] (2026-06-25 00:00 America/Chicago) Confirmed disabled descendants under a checked group should be excluded from range selection because group rules already govern them.
- [ ] Implement the range-selection model, WPF behavior, XAML wiring, tests, and manual validation described below.

## Surprises & Discoveries

- Observation: The correct identifier for this enhancement is VIX-3591.
  Evidence: The user clarified that VIX-3591 is the intended identifier, and the source requirement document is `docs\state\vix-3591-state-assignment-enhancement.md`.

- Observation: The State item grid already supports extended multi-selection, but the assignment tree is still single-row editing state and does not expose assignment-tree selected nodes.
  Evidence: `src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml` sets the State item `DataGrid` to `SelectionMode="Extended"` and binds `SelectedItems`; the assignment `TreeView` only binds `ItemsSource` and each node checkbox.

- Observation: State assignment group rules already live in the UI-free model and should remain the authority for checking and unchecking nodes.
  Evidence: `src\Vixen.Modules\Property\State\Setup\Models\StateAssignmentTreeNode.cs` clears descendant selections when `IsChecked` becomes `true`, updates descendant enabled state, and computes explicit and effective assigned node IDs.

- Observation: The repository has a shared `TreeViewMultipleSelectionBehavior`, but it selects generated `TreeViewItem` containers and does not toggle checkbox assignment state.
  Evidence: `src\Vixen.Common\WPFCommon\Behaviors\TreeViewMultipleSelectionBehavior.cs` maintains `SelectedItems` and handles Ctrl/Shift selection, while assignment changes in State are driven by `StateAssignmentTreeNode.IsChecked`.

- Observation: The assignment tree is intentionally empty when more than one State item row is selected, so range assignment applies only while editing exactly one State item row.
  Evidence: `StateMapperViewModel.AssignedElementRoots` returns assignment roots only when `SelectedItems.Count == 1`.

## Decision Log

- Decision: Add State-specific assignment tree selection state instead of reusing `TreeViewMultipleSelectionBehavior` directly.
  Rationale: The shared behavior is useful as a behavioral reference, but it owns visual `TreeViewItem` selection and only sees generated, visible containers. The State feature needs model-level selected assignment nodes, a command to toggle their checked state, and group-rule integration through `StateAssignmentTreeNode.IsChecked`.
  Date/Author: 2026-06-25 / Codex

- Decision: Treat the selectable range as the flattened list of currently visible assignment-tree nodes in display order.
  Rationale: This matches common tree behavior: collapsed descendants are not visible and are not part of a Shift-click range. It also avoids toggling hidden descendants unexpectedly. A checked group still affects descendants through the existing group-assignment rules.
  Date/Author: 2026-06-25 / Codex

- Decision: Use a dedicated toggle command, invoked by the Space key and optionally by an icon or text button, rather than toggling all selected nodes immediately on Ctrl-click or Shift-click.
  Rationale: Ctrl-click and Shift-click should first manage selection. A separate toggle action gives the user a chance to review the selected range before changing assignments. The user confirmed this should remain the design unless implementation research finds strong evidence that a broader toggle-on-checkbox-click behavior is a well-accepted convention in other well-known applications. Single checkbox clicks continue to toggle one node as they do today.
  Date/Author: 2026-06-25 / Codex

- Decision: Skip disabled assignment nodes when applying a range toggle.
  Rationale: Disabled descendants belong to a checked group and cannot be selected directly under the current rules. The user confirmed they should be excluded because they are already governed by the checked group rules.
  Date/Author: 2026-06-25 / Codex

## Outcomes & Retrospective

This initial plan captures the implementation path. Update this section after each milestone with what changed, which tests passed, what manual behavior was observed, and whether any decisions changed.

## Context and Orientation

Vixen is a Windows desktop WPF application. The State property module lives in `src\Vixen.Modules\Property\State`. A State property is attached to an `IElementNode`, which is a node in Display Setup. A State definition is one named state inside the property. A State item is one editable row in a State definition; it stores a stable ID, name, color, and assigned element-node IDs.

The setup dialog is `src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml`. The main ViewModel is `src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs`. One selected State item row exposes its assignment tree through `AssignedElementRoots`. The row ViewModel is `src\Vixen.Modules\Property\State\Setup\ViewModels\StateItemViewModel.cs`, and each checkbox tree node is `src\Vixen.Modules\Property\State\Setup\Models\StateAssignmentTreeNode.cs`.

The assignment tree stores explicit checked nodes by ID. If a group node is checked, its checked descendants are cleared and disabled, and the group effectively includes all descendant leaves. The Count column shows effective assigned leaf count. This behavior must not be rewritten; range toggling must call the same `IsChecked` setter so these rules remain centralized.

The State item grid already supports multi-selection for preview. That is separate from this plan. The right-side Assigned Elements tree remains an editor for one State item row only. When multiple State item rows are selected, the tree is empty and range selection is unavailable.

Relevant tests live under `src\Vixen.Tests\Property\State`. Existing assignment-tree tests are in `StateAssignmentTreeTests.cs`. Mapper selection, preview, and validation tests are in `StateMapperDefinitionTests.cs`, `StateMapperPreviewTests.cs`, and `StateMapperValidationTests.cs`.

## Plan of Work

Milestone 1 adds model-level selection state to the State assignment tree. Update `StateAssignmentTreeNode` so each node can be visually selected independently from being checked. Add a public documented `bool IsSelected` property and an internal parent relationship or traversal helper that lets the owning row produce a flattened visible list. The node should expose a method that returns visible nodes in display order, including a node's children only when the node is expanded. Do not make `IsSelected` affect `GetSelectedNodeIds()`; selected means highlighted for a pending batch operation, while checked means assigned.

Add UI-free methods to select and toggle assignment-tree ranges. A good shape is a focused internal helper under `src\Vixen.Modules\Property\State\Setup\Models`, for example `StateAssignmentTreeSelectionController`, owned by `StateItemViewModel`. It should maintain an anchor node for Shift selection and provide operations equivalent to:

    SelectSingle(StateAssignmentTreeNode node)
    ToggleSelection(StateAssignmentTreeNode node)
    SelectRange(StateAssignmentTreeNode node)
    ToggleCheckedStateForSelectedNodes()
    ClearSelection()

`SelectSingle` clears previous selection, selects the clicked node, and updates the anchor. `ToggleSelection` adds or removes one node and sets the anchor when needed. `SelectRange` clears previous selection and selects all visible, enabled nodes between the anchor and clicked node, inclusive. If there is no anchor, it behaves like `SelectSingle`. `ToggleCheckedStateForSelectedNodes` iterates selected enabled nodes in visible order and sets each node's `IsChecked` to the opposite of its current value. Disabled nodes are skipped. After toggling a group to checked, its descendants become disabled through existing logic and should not be toggled later in the same command.

Milestone 2 wires the WPF interaction with a State-specific behavior. Add a behavior under `src\Vixen.Modules\Property\State\Setup\Behaviors`, for example `StateAssignmentTreeSelectionBehavior`. This behavior attaches to the assignment `TreeView`, detects clicks on `TreeViewItem` or its child `CheckBox`, reads `Keyboard.Modifiers`, and calls commands or methods on the bound selection controller. It should handle Ctrl-click for additive selection, Shift-click for range selection, and unmodified click for single selection. It should not place selection logic in `StateMapperView.xaml.cs`.

The behavior must avoid breaking normal checkbox toggling. If the user clicks a checkbox with no Ctrl or Shift modifier, the existing checkbox binding can continue toggling only that one node. If the user Ctrl-clicks or Shift-clicks on the checkbox or row content, the behavior manages selection and suppresses the checkbox's immediate single-node toggle. The batch assignment change happens when the user invokes the toggle command, such as pressing Space or clicking a `Toggle Selected` button.

Milestone 3 updates `StateMapperView.xaml`. Add the behavior namespace and attach the new behavior to the assignment `TreeView`. Bind `TreeViewItem` selection visuals to `StateAssignmentTreeNode.IsSelected`, using existing theme colors where possible. Keep the checkbox bound to `IsChecked` and `IsEnabled`. Add a compact `Toggle Selected` command surface near the `Assigned Elements` header if keyboard-only Space is not discoverable enough. The command should be disabled when no assignment nodes are selected. Do not add explanatory in-app help text; tooltips are acceptable.

Milestone 4 updates `StateItemViewModel` and `StateMapperViewModel` command surfaces. `StateItemViewModel` should own the assignment-tree selection controller because selection belongs to the row being edited. It should expose documented public or internal members required by XAML binding, such as `ToggleSelectedAssignmentsCommand`, `CanToggleSelectedAssignments`, and `SelectedAssignmentCount`, or a controller property if binding to controller commands is cleaner. Any public or protected API must include XML documentation.

When the selected State item row changes, clear assignment selection on the previously selected row and continue expanding checked assignments on the new row. When the State definition changes or multiple State item rows are selected and the assignment tree disappears, clear assignment selection. Assignment-tree selection is temporary dialog state and must not be persisted to `StateItemData`.

Milestone 5 adds tests. Extend `StateAssignmentTreeTests.cs` or add a new `StateAssignmentTreeSelectionTests.cs` to cover visible flattening, Ctrl-style add/remove selection, Shift-style range selection, missing-anchor behavior, collapsed descendants being excluded from range selection, selected-versus-checked independence, and toggle behavior for mixed checked and unchecked nodes. Add tests proving group rules are preserved: toggling a selected group on clears checked descendants and disables them; toggling a selected group off re-enables descendants without restoring previous checks. Add ViewModel tests proving assignment selection clears when row selection changes or when multiple State item rows are selected.

Milestone 6 performs validation. Run the focused State tests:

    cd C:\Dev\Vixen
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"

Then run whitespace validation:

    cd C:\Dev\Vixen
    git diff --check

Before final acceptance, run the full test project and a Debug build:

    cd C:\Dev\Vixen
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

If restore or build is blocked by environment restrictions, rerun with approval as required by the agent instructions and record the limitation in this plan.

## Concrete Steps

Start from the repository root:

    cd C:\Dev\Vixen
    git status --short

Read these files before editing:

    Get-Content src\Vixen.Modules\Property\State\Setup\Models\StateAssignmentTreeNode.cs
    Get-Content src\Vixen.Modules\Property\State\Setup\ViewModels\StateItemViewModel.cs
    Get-Content src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs
    Get-Content src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml
    Get-Content src\Vixen.Tests\Property\State\StateAssignmentTreeTests.cs

Inspect the shared tree selection behavior as a reference, but do not copy it blindly:

    Get-Content src\Vixen.Common\WPFCommon\Behaviors\TreeViewMultipleSelectionBehavior.cs

Implement the UI-free selection model first and run its tests before adding WPF behavior. Then add the behavior, XAML wiring, and ViewModel command bindings. Keep changes scoped to the State module and State tests unless a small reusable behavior improvement in `WPFCommon` becomes clearly preferable; if that happens, update this plan and the Decision Log before expanding the scope.

## Validation and Acceptance

Automated acceptance requires focused State tests proving:

- A clicked assignment node can be selected without changing its checked assignment state.
- Ctrl-style selection adds and removes individual assignment nodes.
- Shift-style selection selects all visible enabled assignment nodes between the anchor and clicked node.
- Collapsed descendants are not included in a range selection.
- Disabled descendants under a checked group are not selected or toggled by range operations.
- Toggling a mixed selected range inverts each selected enabled node.
- Existing group rules still apply when a selected group is toggled on or off.
- `StateItemData.ElementNodeIds` and `AssignmentCount` update through the existing assignment-change path after a range toggle.
- Assignment-tree selection clears when the edited State item row changes, when the State definition changes, and when the assignment tree is hidden because multiple State item rows are selected.

Manual acceptance requires these Display Setup scenarios:

1. Open a State property with a State item whose assignment tree has a visible group and several children.
2. Click one child assignment node and confirm only that tree row is selected and the checkbox does not change unless the checkbox itself is clicked normally.
3. Ctrl-click another visible child and confirm both nodes are selected.
4. Shift-click a later visible node and confirm the continuous visible range is selected.
5. Invoke Toggle Selected and confirm unchecked selected nodes become checked while checked selected nodes become unchecked.
6. Select a range that includes a group node, toggle it on, and confirm checked descendants clear, descendants are disabled or grayed, and the count reflects effective leaves.
7. Toggle the selected checked group off and confirm descendants re-enable without restoring prior checks.
8. Collapse a branch, select a range across it, and confirm hidden descendants are not toggled directly.
9. Save, reopen, and confirm only checked assignment state persisted; visual range selection did not persist.
10. Turn Preview on, toggle a selected range, and confirm active preview refreshes for the edited selected State item using the existing preview behavior.

## Idempotence and Recovery

The selection state is temporary and safe to discard. Reopening the setup dialog must reconstruct checked assignments from persisted `StateItemData.ElementNodeIds` but must not restore assignment-tree selection.

The implementation should be additive. If the behavior wiring causes WPF interaction problems, keep the UI-free controller and tests, remove only the behavior/XAML changes from the failed attempt, and revise this plan with the discovered event-routing issue. Do not revert unrelated user changes. The untracked source document `docs\state\vix-3591-state-assignment-enhancement.md` must not be removed or renamed unless the user explicitly confirms that correction.

## Artifacts and Notes

Files expected to change:

    src\Vixen.Modules\Property\State\Setup\Models\StateAssignmentTreeNode.cs
    src\Vixen.Modules\Property\State\Setup\ViewModels\StateItemViewModel.cs
    src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs
    src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml
    src\Vixen.Tests\Property\State\StateAssignmentTreeTests.cs

Possible new files:

    src\Vixen.Modules\Property\State\Setup\Models\StateAssignmentTreeSelectionController.cs
    src\Vixen.Modules\Property\State\Setup\Behaviors\StateAssignmentTreeSelectionBehavior.cs
    src\Vixen.Tests\Property\State\StateAssignmentTreeSelectionTests.cs

Existing behavior to preserve:

    StateAssignmentTreeNode.IsChecked
      checked group clears descendants
      checked group disables descendants
      unchecked group re-enables descendants without restoring checks

    StateItemViewModel.UpdateAssignments
      writes explicit checked node IDs to StateItemData.ElementNodeIds
      raises AssignmentCount after assignment changes

    StateMapperViewModel.AssignedElementRoots
      returns assignment roots only when exactly one State item row is selected

## Interfaces and Dependencies

Use Catel `TaskCommand` or `Command` for any ViewModel command exposed to XAML. Keep ViewModels free of direct WPF control references. WPF input handling belongs in a behavior, and durable assignment semantics belong in `StateAssignmentTreeNode` and the new selection controller.

All public or protected C# APIs added or modified by this work require XML documentation. Follow `.agents\skills\csharp-docs\SKILL.md`: summaries should be present-tense and properties should start with "Gets..." or "Gets or sets...".

No new NuGet package should be added. Existing dependencies already include `Microsoft.Xaml.Behaviors.Wpf` and `WPFCommon`. Keep the State property module independent of the Live Preview app module.

The expected model/controller surface can vary, but it must support these behaviors:

    StateAssignmentTreeNode.IsSelected
    StateAssignmentTreeNode.GetVisibleNodesInDisplayOrder()
    StateAssignmentTreeSelectionController.SelectSingle(node)
    StateAssignmentTreeSelectionController.ToggleSelection(node)
    StateAssignmentTreeSelectionController.SelectRange(node)
    StateAssignmentTreeSelectionController.ToggleCheckedStateForSelectedNodes()
    StateAssignmentTreeSelectionController.ClearSelection()

If implementation finds better names that match local style, update this section and the Decision Log.

## Risks and Open Questions

The main UI risk is WPF event routing between `TreeViewItem` selection gestures and the nested `CheckBox`. The behavior must prevent Ctrl-click and Shift-click from accidentally changing one checkbox before the user invokes the batch toggle.

The main behavior risk is ordering when a selected range contains both a group and its descendants. The plan skips disabled nodes and applies toggles in visible order. If a group is toggled on before a descendant is reached, the descendant becomes disabled and should be skipped.

## Revision Notes

- 2026-06-25 / Codex: Created the initial ExecPlan from the available assignment enhancement document, VIX-3591 State property requirements and plans, current State assignment tree code, current State mapper XAML/ViewModels, existing row multi-select behavior, and the shared WPF tree multi-selection behavior.
- 2026-06-25 / Codex: Renamed the plan and removed the identifier mismatch question after the user clarified that VIX-3591 is the correct identifier.
- 2026-06-25 / Codex: Recorded user decisions that range toggling should be explicit through Space / Toggle Selected unless broader application convention research proves otherwise, and that disabled descendants under checked groups should be excluded from range selection.

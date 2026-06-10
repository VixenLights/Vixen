# Add Manual State Item Reordering

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

VIX-3591 already lets users sort State Items by grid columns and persists the displayed sort order. Sorting is not enough when a user needs an intentional sequence that is not alphabetical, color-based, or count-based. After this change, the State Property Setup dialog lets the user select a State Item row and move it up or down one position at a time with standard list reordering buttons. The user can save, reopen the setup dialog, and see the custom order preserved without losing State item IDs, colors, assignments, validation state, or preview behavior.

## Progress

- [x] (2026-06-10 21:27Z) Researched current State setup dialog, ViewModel, existing sort-order persistence, and State mapper tests.
- [x] (2026-06-10 21:27Z) Updated final and phase requirements to include explicit manual State Item reordering.
- [x] (2026-06-10 22:18Z) Implemented Move Up and Move Down commands in the State mapper ViewModel.
- [x] (2026-06-10 22:18Z) Added Move Up and Move Down icon buttons with tooltips to the State mapper view.
- [x] (2026-06-10 22:18Z) Added focused tests for command availability, item identity preservation, selection retention, group ordering, and save persistence.
- [x] (2026-06-10 22:19Z) Ran focused State property tests and `git diff --check`; both passed.
- [x] (2026-06-10 22:27Z) JIRA issue VIX-3591 was updated externally by the ticket owner.

## Surprises & Discoveries

- Observation: The setup dialog already persists visible column sort order through a view-facing synchronization hook.
  Evidence: `src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml.cs` handles `DataGrid.Sorting` and calls `StateMapperViewModel.SynchronizeStateItemOrder(...)` with the displayed `DataGrid.Items` order.

- Observation: The ViewModel already has the correct low-level operation for preserving State item identity during order changes.
  Evidence: `StateMapperViewModel.SynchronizeStateItemOrder(...)` moves existing `StateItemViewModel` instances inside `ObservableCollection<StateItemViewModel>` rather than recreating rows.

## Decision Log

- Decision: Implement manual reorder with icon-only Move Up and Move Down buttons using existing Vixen.Common arrow resources and tooltips, instead of drag-and-drop.
  Rationale: Up/Down buttons are a common listbox/list reordering mechanism, are obvious in a setup dialog, can be keyboard-reachable, and avoid ambiguity with the existing sortable `DataGrid` column headers. Icon-only buttons match the requested UI direction and reuse existing assets. Tooltips keep the action discoverable. Drag-and-drop can be added later, but it is harder to test and easier to conflict with row selection, inline editing, and grid sorting.
  Date/Author: 2026-06-10 / Codex

- Decision: Reuse the existing selected State definition `Items` collection and move existing `StateItemViewModel` instances.
  Rationale: Moving existing instances preserves stable IDs, color, assignment tree state, validation state, and selected object identity. Recreating row view models would risk stale assignment tree bindings and broken preview behavior.
  Date/Author: 2026-06-10 / Codex

## Outcomes & Retrospective

Implemented manual State Item reordering in the State setup form. The State mapper now exposes `MoveItemUpCommand` and `MoveItemDownCommand`, moves the selected `StateItemViewModel` instance in the selected definition's `Items` collection, keeps the moved item selected, rebuilds State Item Group preview choices, refreshes preview once after the move, and persists the resulting order on save.

The State Item button row now includes icon-only Move Up and Move Down buttons using `/Resources;component/arrow_up.png` and `/Resources;component/arrow_down.png`, with tooltips describing each action.

Validation completed on 2026-06-10:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"

Result: Passed, 77 tests, 0 failures.

    git diff --check

Result: Passed.

Known warnings during test execution are pre-existing package/build warnings, including NU1904 for LiteDB 4.1.4 and existing compiler warnings outside this change.

## Context and Orientation

The State property lives under `src\Vixen.Modules\Property\State`. A State property contains one or more State definitions. A State definition contains State Items. A State Item is one row in the setup grid and stores a stable ID, display name, color, and assigned element node IDs. Stable IDs are important because future effects should be able to reference definitions or items across rename and reorder operations.

The setup window is `src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml`. It uses a WPF `DataGrid` named `StateItemsGrid` bound to `StateMapperViewModel.Items`. The code-behind file `src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml.cs` already listens for column sorting and synchronizes the ViewModel order after WPF applies a grid sort. This plan keeps that behavior and adds explicit manual reordering.

The main ViewModel is `src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs`. It exposes `Items`, `SelectedItem`, `AddItemCommand`, `RemoveItemCommand`, `EditColorCommand`, and the existing internal method `SynchronizeStateItemOrder(...)`. It also rebuilds State Item Group preview choices using the first visible item appearance order and refreshes preview through `RefreshPreview()`.

Focused tests live under `src\Vixen.Tests\Property\State`. Existing State definition and ordering tests are in `src\Vixen.Tests\Property\State\StateMapperDefinitionTests.cs`.

## Plan of Work

First, add two commands to `StateMapperViewModel`: `MoveItemUpCommand` and `MoveItemDownCommand`. The command handlers should use the current `SelectedItem`, find its index in `Items`, and call `Items.Move(...)` to move the same `StateItemViewModel` instance one position. `MoveItemUpCommand` can execute only when `SelectedItem` is in `Items` and its index is greater than zero. `MoveItemDownCommand` can execute only when `SelectedItem` is in `Items` and its index is less than `Items.Count - 1`.

The move handler should suppress collection-change preview churn during the move, then set `SelectedItem` back to the moved item, rebuild State Item Group choices, refresh preview once if Preview is enabled, and raise `CanExecuteChanged` for both move commands plus the remove command. If the view can scroll the selected row into view with a small, view-only handler, keep that code in `StateMapperView.xaml.cs` and do not put WPF control references into the ViewModel.

Second, update `StateMapperView.xaml`. Add icon-only Move Up and Move Down buttons beside the existing Add and Remove controls for the State Item list. Use the existing resources `src\Vixen.Common\Resources\arrow_up.png` for Move Up and `src\Vixen.Common\Resources\arrow_down.png` for Move Down. The user mentioned `arrow-up.png`; the repository asset is named `arrow_up.png`, so use the actual underscore filename. Each icon button must include a tooltip such as `Move selected State Item up` or `Move selected State Item down`. The exact placement can be a vertical button stack to the right of the `DataGrid`, or a compact button row under the grid. Preserve the existing theme resources and button styles where practical.

Third, add tests in `StateMapperDefinitionTests.cs` or a new `StateMapperItemReorderTests.cs`. Tests should construct a `StateData` with at least three State items using distinct IDs, names, colors, and one assignment. Test that Move Up is disabled for the first row and enabled for a middle row. Test that Move Down is disabled for the last row and enabled for a middle row. Test that moving a middle row up then saving persists the new order and preserves IDs, colors, and assignments. Test that moving a middle row down keeps the same row selected. Test that moving duplicate-name rows changes `AvailableStateItemGroups` according to first-grid-appearance order.

Fourth, run focused validation. Start with:

    cd C:\Dev\Vixen
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"

If focused tests pass, run:

    cd C:\Dev\Vixen
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

Before final handoff, run:

    cd C:\Dev\Vixen
    git diff --check

If dependency restore or test execution fails because the sandbox blocks network or filesystem access, rerun the same command with the required approval and record the limitation in `Outcomes & Retrospective`.

## Concrete Steps

Start from the repository root:

    cd C:\Dev\Vixen
    git status --short

Read the current files before editing:

    Get-Content src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs
    Get-Content src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml
    Get-Content src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml.cs
    Get-Content src\Vixen.Tests\Property\State\StateMapperDefinitionTests.cs

Modify `StateMapperViewModel.cs` by adding private command fields for `MoveItemUpCommand` and `MoveItemDownCommand`, public command properties with XML documentation, `CanMoveItemUp`, `CanMoveItemDown`, `MoveItemUpAsync`, `MoveItemDownAsync`, and one shared helper such as `MoveSelectedItem(int offset)`. Keep the helper private unless tests need public behavior through command execution.

Update command invalidation. Whenever selected row or item collection changes, raise `CanExecuteChanged` for Move Up and Move Down. Existing touch points include `SelectItem(...)`, `ItemsCollectionChanged(...)`, `AddItemAsync(...)`, `RemoveItemAsync(...)`, `SynchronizeStateItemOrder(...)`, and `RaiseOkCanExecuteChanged()`.

Modify `StateMapperView.xaml` so the State Item controls expose Move Up and Move Down. The current `WrapPanel` under the grid contains Add and Remove. A minimal acceptable layout is to add icon buttons whose content is an `Image` sourced from the existing resources and whose `ToolTip` describes the action. The exact resource URI depends on the existing project resource conventions; inspect other XAML files that reference `Vixen.Common\Resources` images before choosing the final syntax. The target behavior is equivalent to:

    <Button Command="{Binding MoveItemUpCommand}" ToolTip="Move selected State Item up" Style="{StaticResource RowButtonStyle}">
        <Image Source=".../arrow_up.png" Width="16" Height="16"/>
    </Button>
    <Button Command="{Binding MoveItemDownCommand}" ToolTip="Move selected State Item down" Style="{StaticResource RowButtonStyle}">
        <Image Source=".../arrow_down.png" Width="16" Height="16"/>
    </Button>

If the buttons are moved to a side stack, keep `StateItemsGrid` named because the sort synchronization code-behind uses it.

Add or update tests. Because `TaskCommand` can be executed directly through `CanExecute` and `Execute`, tests should prefer public command behavior over reflection. It is acceptable to invoke `SaveAsync` by reflection, matching the existing tests, to assert persisted order in the source `StateData`.

After edits, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"
    git diff --check

## Validation and Acceptance

Automated acceptance is demonstrated when the focused Property.State tests pass and include tests proving:

- Move Up is disabled with no selection and for the first selected row.
- Move Down is disabled with no selection and for the last selected row.
- Moving up or down reorders the selected State definition's `Items` collection without recreating `StateItemViewModel` objects.
- The moved State item remains selected after the move.
- Save persists the manual order into `StateData.StateDefinitions[...].Items`.
- State item IDs, colors, names, and assignments stay attached to the same logical State item after reorder.
- State Item Group preview choices reflect first-grid-appearance order after reorder.

Manual acceptance is demonstrated by starting Vixen, opening Display Setup, adding or opening a State property, creating at least three State Items with names that do not sort into the desired final order, selecting a middle row, clicking Move Up or Move Down, clicking OK, reopening setup, and observing the custom order still displayed. The row's color, Count value, and assignment tree must remain attached to the same item.

## Idempotence and Recovery

The implementation is additive. Re-running tests is safe. If a layout change is unsatisfactory, revert only the XAML changes made for the buttons and keep the ViewModel tests as the source of behavior. Do not use `git reset --hard` or `git checkout --` unless explicitly requested. If command tests reveal stale command availability, fix command invalidation rather than broadening `CanExecute` rules.

## Artifacts and Notes

Current relevant behavior:

    StateMapperView.xaml:
      DataGrid x:Name="StateItemsGrid"
      ItemsSource="{Binding Items}"
      SelectedItem="{Binding SelectedItem, Mode=TwoWay}"
      Sorting="StateItemsGrid_Sorting"

    StateMapperView.xaml.cs:
      StateItemsGrid_Sorting waits for WPF sorting to apply, then calls
      StateMapperViewModel.SynchronizeStateItemOrder(StateItemsGrid.Items.OfType<StateItemViewModel>().ToList()).

    StateMapperViewModel.SynchronizeStateItemOrder:
      Validates that visible order contains exactly the selected definition's items.
      Moves existing instances with Items.Move(...).
      Rebuilds State Item Group choices.
      Refreshes preview once.

The manual reorder implementation should follow the same preservation model as `SynchronizeStateItemOrder`, but it should not need to read `DataGrid.Items`.

## Interfaces and Dependencies

Use Catel `TaskCommand` because this ViewModel already exposes item actions with `TaskCommand`. Do not add a new service for simple in-memory row movement. Do not add a dependency from the ViewModel to WPF controls.

The public or protected C# command properties added to `StateMapperViewModel` must include XML documentation because repository guidance treats stale or missing XML docs on public/protected APIs as defects.

Expected command surface at completion:

    public TaskCommand MoveItemUpCommand { get; }
    public TaskCommand MoveItemDownCommand { get; }

Private helper shape can vary, but the behavior must be equivalent to:

    private Task MoveSelectedItemAsync(int offset)

where `offset` is `-1` for Move Up and `1` for Move Down.

## JIRA Update

The ticket owner updated VIX-3591 externally, so no repository-side Jira action remains. The reviewed requirement text is retained below as the implementation record.

Suggested JIRA text:

    New requirement for VIX-3591: State Property Setup must allow manual State Item reordering.

    Users can currently persist a State Item order by sorting grid columns, but some desired orders are intentional and cannot be expressed by Name, Color, or Count sorting. Add common list reordering controls to the State Item list: icon-only Move Up and Move Down buttons that operate on the selected State Item row and move it one position within the selected State definition. Use the existing `src\Vixen.Common\Resources\arrow_up.png` and `src\Vixen.Common\Resources\arrow_down.png` assets and provide tooltips for both buttons.

    Acceptance criteria:
    - Move Up is disabled when no State Item is selected or the selected item is already first.
    - Move Down is disabled when no State Item is selected or the selected item is already last.
    - Moving an item keeps the same logical State Item selected and preserves its stable ID, name, color, assignment IDs, validation state, and preview behavior.
    - Saving and reopening State Property Setup shows the manually reordered State Items in the saved order.
    - Manual reorder is scoped to the selected State definition and does not change State definition creation order.
    - State Item Group preview choices update in first-grid-appearance order after a manual reorder.
    - If Preview is enabled, moving an item refreshes active preview once after the move.

    Test coverage:
    - Add focused Property.State tests for Move Up and Move Down command availability.
    - Add tests proving persisted order and ID/color/assignment preservation after save.
    - Add tests proving selected item retention and State Item Group ordering after reorder.
    - Run `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"`.

## Revision Notes

- 2026-06-10 / Codex: Created this focused ExecPlan after adding manual reorder requirements to `docs\vix-3591-state-property-final-requirements.md` and `docs\vix-3591-state-property-phase-4.md`.
- 2026-06-10 / Codex: Updated the plan after review feedback to require icon-only arrow buttons with tooltips and to keep the JIRA update text in review-only state until explicitly approved.
- 2026-06-10 / Codex: Implemented the ViewModel commands, XAML icon buttons, focused tests, and recorded validation results.
- 2026-06-10 / Codex: Marked the JIRA update task complete after the ticket owner updated VIX-3591 externally.

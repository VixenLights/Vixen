# VIX-3591 State Property Multi-Select Preview Backport

## Purpose

Backport the State item grid multi-select preview behavior from the VIX-3929 Custom Prop Editor work into the VIX-3591
State Property setup dialog. This replaces the older Preview Mode radio buttons and State Item Group combo box with direct
row selection in the State item grid.

The intent is to make preview selection feel like the Custom Prop Editor State Definition mode:

- Select one State item row to preview that item.
- Select multiple State item rows to preview all selected items in their configured colors.
- Clear row selection to preview no active State item colors.
- Use normal grid multi-select gestures instead of a separate preview mode selector.

The user can copy these shared behaviors from the VIX-3929 branch into the VIX-3591 branch before implementation:

- `src/Vixen.Common/WPFCommon/Behaviors/MultiSelectorSelectedItemsBehavior.cs`
- `src/Vixen.Common/WPFCommon/Behaviors/DataGridSortedItemsCommandBehavior.cs`

## Baseline Reference

Use the VIX-3929 branch as the behavior reference:

- `src/Vixen.Modules/App/CustomPropEditor/Views/ElementTree.xaml`
- `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/StateDefinitionEditorViewModel.cs`
- `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/CustomPropStateDefinitionViewModel.cs`
- `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/CustomPropStateItemViewModel.cs`

The VIX-3591 target files are:

- `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml`
- `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs`
- `src/Vixen.Modules/Property/State/Setup/ViewModels/StateDefinitionViewModel.cs`
- `src/Vixen.Modules/Property/State/Setup/Preview/StatePreviewCoordinator.cs`
- State property tests under `src/Vixen.Tests/Property/State`

## Current VIX-3591 Behavior

The State property setup dialog currently has:

- A Preview on/off toggle.
- A Preview Mode radio group:
  - `State Item Group`
  - `Selected State Item`
- A State Item Group combo box containing `<ALL>` plus distinct State item names in first-grid-appearance order.
- A State item `DataGrid` with `SelectionMode="Single"`.

Preview logic is driven by:

- `IsPreviewEnabled`
- `IsStateItemGroupPreviewMode`
- `AvailableStateItemGroups`
- `SelectedStateItemGroup`
- `SelectedItem`
- `GetPreviewedItems()`

This made sense for the first implementation, but the VIX-3929 grid multi-select behavior is more direct and easier to
understand.

## Target Behavior

Keep the Preview on/off toggle. Remove the preview mode selector and State Item Group combo.

When Preview is on:

- The previewed State items are the selected rows in the State item grid.
- A single selected row previews that row.
- Multiple selected rows preview all selected rows.
- No selected rows means no active State item preview.
- Selection changes refresh preview immediately.
- Assignment changes, color changes, row add/remove, row reorder, and State definition switches refresh preview only for
  selected rows.
- Duplicate `(element ID, color)` preview pairs are still activated once.
- The same element can still preview multiple colors when selected rows assign the same element with different colors.

When Preview is off:

- Row selection, assignment edits, color edits, row edits, and definition switches must not publish Live Preview messages.
- Turning Preview off clears the `"State Preview"` context, same as the current implementation.

The assignment tree remains a single-row editing surface.

- `SelectedItem` is still the current row whose assignment tree is displayed and edited when exactly one row is selected.
- `SelectedItems` is the preview row set.
- If multiple rows are selected, the Assigned Elements tree must be empty.
- Assignment edits are disabled when multiple rows are selected because assignments cannot be changed across multiple State
  item rows at once.
- Delete removes all selected State item rows.
- If more than one row is selected, the delete confirmation message must clearly state that multiple rows will be deleted.

## View Model Changes

Update `StateMapperViewModel`:

1. Add a selected-items collection:

   ```csharp
   public ObservableCollection<StateItemViewModel> SelectedItems { get; }
   ```

2. Subscribe to `SelectedItems.CollectionChanged` in the constructor.

3. Remove or obsolete these preview-mode members:

   - `private const string AllStateItemGroups`
   - `IsStateItemGroupPreviewMode`
   - `AvailableStateItemGroups`
   - `SelectedStateItemGroup`
   - `RebuildAvailableStateItemGroups`
   - `SynchronizeAvailableStateItemGroups`
   - `IndexOfAvailableStateItemGroup`

4. Update `SelectItem(StateItemViewModel? item, bool expandCheckedAssignments)`:

   - Keep setting `SelectedItem`.
   - Keep expanding checked assignments when requested.
   - Keep raising item command state.
   - Do not use `IsStateItemGroupPreviewMode`.
   - Refresh preview whenever Preview is on and row selection changes.
   - If more than one row is selected, clear `SelectedItem` or expose an empty assignment-tree source so Assigned Elements
     shows no editable checkboxes.

5. Add a selected-items collection changed handler:

   ```csharp
   private void SelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
   {
       if (SelectedItems.Count > 1)
       {
           SelectedItem = null;
       }
       else if (SelectedItems.Count == 1)
       {
           SelectedItem = SelectedItems[0];
       }

       RaisePropertyChanged(nameof(SelectedItems));
       RaisePropertyChanged(nameof(AssignedElementRoots));
       RefreshPreview();
   }
   ```

   `AssignedElementRoots` is an example property for the assignment tree source. The exact implementation can use another
   name, but the bound tree source must be empty when multiple State item rows are selected.

6. Simplify `GetPreviewedItems()`:

   ```csharp
   private IEnumerable<StateItemViewModel> GetPreviewedItems()
   {
       return SelectedItems.Count > 0 ? SelectedItems : [];
   }
   ```

   Do not fall back to `SelectedItem` unless the selected-items binding cannot be installed. With
   `MultiSelectorSelectedItemsBehavior`, a single grid row selection is represented in `SelectedItems`.

7. Simplify `IsPreviewedItem(StateItemViewModel item)` to check `SelectedItems.Contains(item)`.

8. Remove group-list refresh calls from:

   - Add item
   - Remove item
   - Move item
   - Sorted order synchronization
   - Item name changes

   Keep validation and preview refresh calls.

9. When switching State definitions, clear selected rows and preview:

   - Set `SelectedItem` to `null`.
   - Clear `SelectedItems`.
   - Raise `Items`.
   - If Preview is on, clear the preview context and refresh with no selected rows.

10. Keep `SynchronizeStateItemOrder` if it already exists in the VIX-3591 branch, but remove group-list maintenance from it.
    If it does not exist in the target branch, port the sorted-order command pattern from VIX-3929 using
    `DataGridSortedItemsCommandBehavior`.

11. Continue saving with:

    ```csharp
    _source.StateDefinitions = StateDefinitions.Select(definition => definition.ToData()).ToList();
    ```

    Because the State item collection order is still the persisted order.

12. Update remove-item behavior:

    - Remove every row in `SelectedItems` when one or more rows are selected.
    - Fall back to `SelectedItem` only if the selected-items behavior is not available.
    - If exactly one row is selected, use the existing single-row confirmation text.
    - If more than one row is selected, use confirmation text that identifies the operation as a multi-row delete, for
      example: `Delete 3 State items?`
    - After delete, clear `SelectedItems`, select a sensible next row only when there is a single current row, refresh
      preview, raise item command state, and validate.

## XAML Changes

Update `StateMapperView.xaml`:

1. Add the behavior namespace if it is not already present:

   ```xml
   xmlns:behaviors="clr-namespace:Common.WPFCommon.Behaviors;assembly=WPFCommon"
   xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
   ```

2. Remove the Preview Mode radio buttons.

3. Remove the State Item Group combo box.

4. Keep the Preview on/off toggle.

5. Change the State item grid:

   ```xml
   SelectionMode="Extended"
   SelectionUnit="FullRow"
   ```

6. Bind the grid selected items:

   ```xml
   <i:Interaction.Behaviors>
       <behaviors:MultiSelectorSelectedItemsBehavior SelectedItems="{Binding SelectedItems}"/>
       <behaviors:DataGridSortedItemsCommandBehavior SortedItemsCommand="{Binding PersistStateItemSortCommand}"/>
   </i:Interaction.Behaviors>
   ```

   Use `PersistStateItemSortCommand` only if the VIX-3591 branch uses the command version of sorted-order persistence.
   If that branch still uses the existing `StateItemsGrid_Sorting` code-behind method, either keep the code-behind or replace
   it with the behavior during this backport.

7. The user-facing preview area should only show the Preview toggle. No additional explanatory text is required.

8. Bind Assigned Elements to a source that is empty when multiple rows are selected. Do not bind directly to
   `SelectedItem.AssignmentRoots` unless `SelectedItem` is explicitly cleared for multi-row selection.

## Preview Logic

`StatePreviewCoordinator` can remain unchanged.

The coordinator already works from desired `(element ID, color)` pairs. The only change is which items feed those pairs:

- Before: selected item, exact-name group, or `<ALL>`.
- After: selected grid rows.

The existing incremental behavior remains valid:

- Turn off removed pairs before turning on new pairs.
- Preserve multi-color activations where possible.
- Clear/release the `"State Preview"` context on Preview off or window close.

## Sorting and Persistence

VIX-3591 already requires sorted grid order to persist. Preserve that behavior.

If porting `DataGridSortedItemsCommandBehavior`:

- Add a command such as `PersistStateItemSortCommand` to `StateMapperViewModel`.
- The command receives the visible grid items after sorting.
- Reorder the selected definition's `Items` collection to match the visible order.
- Keep State item IDs, names, colors, assignments, and row selection intact.
- Refresh preview if Preview is on and selected rows are affected.

This mirrors the VIX-3929 fix where DataGrid sorting updates the underlying model order instead of only changing the
collection view.

## Test Plan

Add or update tests under `src/Vixen.Tests/Property/State`.

Required coverage:

- Preview on with no selected State item rows publishes no active State item pairs.
- Preview on with one selected row publishes only that row's assignments and color.
- Preview on with multiple selected rows publishes all selected row assignments and colors.
- Assigned Elements shows the selected row's assignment tree when exactly one row is selected.
- Assigned Elements is empty when multiple rows are selected.
- Assignment changes cannot be made when multiple rows are selected.
- Selecting or deselecting rows while Preview is on refreshes the State preview context.
- Selecting or deselecting rows while Preview is off publishes no preview messages.
- Assignment or color changes refresh preview only when the changed row is selected.
- Switching State definitions clears selected rows and clears active preview before rendering the new selection state.
- Multi-color overlap still works when multiple selected rows assign the same element with different colors.
- Delete with one selected row removes that row using the existing single-row confirmation.
- Delete with multiple selected rows removes all selected rows and uses a confirmation message that says multiple rows will
  be deleted.
- Sorting the State item grid persists row order and keeps selected row preview semantics correct.
- Manual Move Up/Move Down still works after removing State Item Group preview controls.

Recommended focused commands:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State" --no-restore
git -c core.whitespace=trailing-space,space-before-tab,cr-at-eol diff --check
```

Run broader validation when the branch is ready:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
```

## Manual Validation

1. Attach or edit a State property in Display Setup.
2. Turn Preview on.
3. Select one State item row and confirm only that row previews.
4. Ctrl-select or Shift-select multiple State item rows and confirm all selected rows preview.
5. Confirm Assigned Elements is empty while multiple rows are selected.
6. Confirm assignment checkboxes cannot be changed while multiple rows are selected.
7. Delete multiple selected rows and confirm the message says multiple rows will be deleted.
8. Clear row selection and confirm the preview clears active State item colors.
9. Edit the color of a selected row and confirm preview refreshes.
10. Edit the color of an unselected row and confirm preview does not change.
11. Change assignments for a single selected row and confirm preview refreshes only when that row is selected.
12. Sort by Name, Color, and Count, then OK/save/reopen and confirm the displayed order persisted.
13. Switch State definitions and confirm previous preview output clears.
14. Close with OK, Cancel, and window close paths and confirm no stale `"State Preview"` output remains.

## Risks and Notes

- The assignment tree is single-row UI. It must be empty when multiple State item rows are selected, and assignment edits must
  not apply to all selected rows.
- Removing State Item Group preview removes the one-click `<ALL>` preview. Users can still preview all rows by selecting all
  rows in the grid. If desired, make sure standard DataGrid `Ctrl+A` behavior works.
- Remove deletes all selected rows. Use single-row confirmation for one selected row and multi-row confirmation when more
  than one row is selected.
- Tests that currently assert `AvailableStateItemGroups`, `SelectedStateItemGroup`, or `IsStateItemGroupPreviewMode` should
  be rewritten around `SelectedItems`.
- If the target branch does not yet have `DataGridSortedItemsCommandBehavior`, copy it from this branch before wiring XAML.

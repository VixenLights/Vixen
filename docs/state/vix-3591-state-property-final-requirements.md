# VIX-3591 State Property Final Requirements

## Goal

Implement the State Property Vixen Display Setup and xModel import.

The State Property lets a user define one or more named State definitions for a Prop. Each State definition contains State items that
map colors to element assignments. The feature is the persisted authoring foundation for a future State effect, but the
State effect itself is out of scope for VIX-3591.

## Definitions

- Prop: A Prop is a collection of ElementNodes that make up all the parts (usually lights) of a Prop.
- State Definition: A State Definition defines a known State for part or all of a Prop. 
- State Item: A State Item defines a portion of a State Definition. 1 to many State Items construct the entire State Definition.
  A State Item defines the the Name, a set of elements, and a color that are associated.
- State Item Group: Multiple State Items can be grouped together by having the same name. They can be targeted as a group. For
  example, Santas arm may have a glove that is dark red, a cuff that is white, and the arm itself is bright red. Those can 
  be defined as multiple State Items, but associated together by name. 

## Summary

- Add a `State` property module that can be attached to a Display Setup element.
- Store State data as one property container with one or more State definitions.
- Let users add, delete, rename, copy, edit, validate, sort, manually reorder, and preview State definitions in Display Setup.
- Persist stable IDs so future effects can reference State definitions across rename and edit operations.
- Import xLights `StateInfo` into the same State Property model.
- Import xLights `CustomModel` and `CustomModelCompressed` as alternate encodings of the same model data.
- Preserve existing FaceInfo, Submodel, and non-State import behavior.

## Terminology

- **State property**: The `State` property attached to an `IElementNode`. It is a container and does not have its own
  user-visible name or description.
- **State definition**: One named definition inside the State property. It has a stable ID, name, description, State items,
  and preview/editing behavior.
- **State item**: One row inside a State definition. It has a stable ID, name, color, and assigned element node IDs.
- **State Item Group**: The set of State items in the selected State definition whose names exactly match.
- **Effective leaf element**: A leaf element included by a State item assignment. Checking a group includes descendant
  leaves.
- **Logical clone**: A draft, serialization, or edit clone representing the same logical object. Stable IDs are preserved.
- **Copy-as-new**: A user copy operation that creates a distinct logical object. New stable IDs are generated.

## Functional Requirements

### State Data Model

- `StateData` is the State property container.
- `StateData` contains a `StateDefinitions` collection.
- A State property is invalid without at least one State definition.
- The State property container does not expose or persist its own user-visible name or description.
- The attached State property keeps a non-empty stable ID for property identity.
- Each State definition has a non-empty persisted stable ID.
- Each State item has a non-empty persisted stable ID.
- Logical clones preserve State property, State definition, and State item IDs.
- Copying a State property to another element creates a new State property ID.
- Copying a State definition creates a new State definition ID and new State item IDs.
- Renaming or editing a State definition preserves its State definition ID.
- Editing State item names, colors, or assignments preserves State item IDs.
- No legacy migration is required because this data is development-only and unreleased.

### State Definition Management

- The setup dialog includes a State Definition combo box.
- Definitions appear in fixed creation order.
- The first State definition is selected when the setup dialog opens.
- Users can Add, Delete, Rename, and Copy State definitions.
- Add opens a modal name dialog and suggests `State - N`, incrementing until unique.
- Add creates a new definition at the end of the collection with one default State item row.
- Delete uses a standard Catel `IMessageService` question dialog.
- The last State definition cannot be deleted.
- After Delete, the next definition is selected when possible; otherwise the previous definition is selected.
- Rename preserves the selected definition ID.
- Copy prompts for a new unique name, appends the copy, selects it, preserves user-visible values, and generates new nested IDs.
- Add, Rename, and Copy name dialogs must use themed WPF/Catel views and must center correctly over the setup dialog.

### State Definition Validation

- State definition names are required.
- Leading and trailing whitespace is trimmed before a name is accepted.
- Whitespace-only names block OK in the setup dialog and block OK in Add/Rename/Copy dialogs.
- Exact duplicate State definition names are blocked.
- State definition name uniqueness is case-sensitive.
- Case-only differences, such as `Open` and `open`, are allowed but produce a non-blocking warning.
- Case-only warnings apply to State definition names only.
- State definition names shorter than three characters produce a non-blocking warning.
- Switching to another State definition is blocked while the current definition has blocking validation errors.
- OK remains disabled while any State definition or State item has a blocking validation error.
- Cancel and window close remain available and discard draft edits.

### State Item Editing

- The selected State definition shows its description, State item grid, assignment tree, and preview controls.
- Users can add and remove State item rows.
- New State item rows use the current valid default item name and default color behavior.
- When the selected element tree has discrete-color leaf elements, newly added State item rows default to the first color
  that is available on every discrete-color leaf in that tree. If no shared discrete color exists, the default color remains
  white.
- The default State item row created with a newly added State definition follows the same discrete-color default rule.
- State item names are required and trimmed when editing completes.
- Whitespace-only State item names block OK.
- Exact duplicate State item names are allowed.
- State item names are case-sensitive.
- Case-only State item name differences do not warn at the State definition level.
- Users can edit State item names inline in the grid.
- Users can edit State item colors through the standard Vixen color chooser/discrete color chooser behavior.
- Color cells display the selected color as the background and the hex value as text.
- The Count column shows the effective assigned leaf count.
- Sorting the State item grid by a column header and saving persists the displayed row order for that State definition.
- Persisting sorted order must not change State item IDs, names, colors, or assignments.
- Users can manually reorder State item rows when column sorting cannot express the desired order.
- Manual reorder uses common list reordering controls: icon-only Move Up and Move Down buttons next to the State item list,
  bound to commands that move the selected row one position at a time.
- The Move Up button uses `src\Vixen.Common\Resources\arrow_up.png`, the Move Down button uses
  `src\Vixen.Common\Resources\arrow_down.png`, and both buttons must provide clear tooltips.
- Move Up is disabled when no row is selected, the selected row is the first row, or the selected row is not part of the
  selected State definition.
- Move Down is disabled when no row is selected, the selected row is the last row, or the selected row is not part of the
  selected State definition.
- Manual reorder must keep the moved row selected and visible, must preserve State item stable IDs, names, colors, and
  assignments, and must persist after OK/save and reopen.
- Manual reorder updates State Item Group preview choices in the new first-grid-appearance order. If Preview is on, the
  active preview refreshes once after the move.

### Assignment Tree

- The assignment tree displays the selected element and its children for the selected State item.
- Each tree node has a checkbox.
- Checking a group node clears checked descendants, disables/grays descendants, and treats descendant leaves as included.
- Unchecking a group node re-enables descendants without restoring prior checks.
- Selecting a State item row displays that row's assignment tree.
- Selecting a row expands only branches needed to reveal checked assignments.
- Switching State definitions clears selected State item row selection so the user explicitly chooses the row to inspect.

### Live Preview

- Preview state is temporary dialog state and is never persisted.
- Preview is off by default.
- Preview Mode defaults to State Item Group.
- State Item Group selection defaults to `<ALL>`.
- Preview Mode controls are disabled while Preview is off.
- When Preview is off, row selection, assignment edits, color edits, row edits, and definition switches do not publish Live Preview messages.
- Turning Preview on evaluates the current selected State definition and current Preview Mode.
- Selected State Item mode previews only the selected row and does not render when no row is selected.
- State Item Group mode previews all exact-name matches for a named group, or all rows when `<ALL>` is selected.
- Case-different State item names remain separate group choices.
- The State Item Group combo contains `<ALL>` plus distinct State item names in first-grid-appearance order.
- Group choices update after State item add, remove, rename, sorted-order changes, and manual reorder changes.
- Changing group selection refreshes preview when Preview is on.
- Changing row selection does not affect preview while in State Item Group mode.
- Assignment and color edits refresh preview for active State items only.
- Desired preview state is tracked as `(element ID, color)` pairs.
- Duplicate `(element ID, color)` pairs are activated once.
- The same element ID may preview multiple colors.
- Incremental preview refreshes use turn-off before turn-on when both are required.
- Because Live Preview turns off by element ID, removing one color from a multi-color element must turn off the element and reactivate remaining desired colors.
- Preview uses the isolated `"State Preview"` context.
- Turning Preview off clears the `"State Preview"` context.
- Switching State definitions while Preview is on clears the context and renders the new selected definition.
- Closing through OK, Cancel, or window close releases or clears the `"State Preview"` context so no stale preview remains.

### xModel Import Hierarchy

- Import keeps the top-level group naming and wildcard behavior that exists today.
- Example: importing `Santa Waving` creates a top-level group such as `Santa Waving {1}`.
- The decoded model leaves are created under a child model group named with the model name plus ` - Model` and existing wildcard behavior.
- Example: `Santa Waving - Model {1}` is a child of `Santa Waving {1}`.
- The State property container is attached to the child model group, not the top-level group.
- FaceInfo groups remain children of the top-level group as they are today.
- Submodel groups remain children of the top-level group as they are today.
- Optional legacy `States` groups remain children of the top-level group while the temporary importer constant is enabled.
- StateInfo and FaceInfo are optional.
- Models without StateInfo import without creating a State property solely for this feature.
- Models without FaceInfo import without creating Face groups.
- Models with neither StateInfo nor FaceInfo continue through the existing CustomModel import behavior.
- Existing Submodel behavior remains unchanged.

### xModel StateInfo Import

- xModel import creates one State property container on the child model group.
- Each imported `StateInfo` becomes one State definition in that State property.
- State definition names come from the `StateInfo` `Name` attribute.
- Missing, empty, or whitespace-only StateInfo names use `State - N`, incrementing until unique.
- Exact duplicate imported StateInfo names keep the first name unchanged and suffix later names as `Name - 2`, `Name - 3`, and so on.
- Existing suffix collisions increment until unique.
- Case-only imported names remain distinct.
- Imported State definitions receive new non-empty IDs.
- Imported State items retain existing imported item behavior and stable IDs.
- StateInfo node mappings target leaf elements under the child model group.
- New StateInfo import does not require creating separate States groups.

### Temporary Legacy States Group Constant

- Add an internal importer constant controlling whether old `States` element groups are created.
- The constant defaults to `true` during validation.
- When enabled, old `States` elements are created as they are today and receive no new State property logic.
- The constant is not exposed in UI or configuration.
- The code must clearly comment that the constant is temporary and should be removed after validation.

### CustomModel And CustomModelCompressed

- `CustomModel` and `CustomModelCompressed` are alternate source encodings for the same xLights custom model data.
- Import resolves the model source first, then decodes directly into Vixen's shared internal model representation.
- Compressed input must not be converted to `CustomModel` text as an intermediate step.
- `CustomModelCompressed` is preferred when both attributes exist and the compressed data is valid.
- If compressed data is invalid and `CustomModel` is valid, import falls back to `CustomModel` and logs a warning.
- If neither source is valid, import aborts the entire xModel import, logs an error, and shows a user-facing error.
- If only `CustomModel` exists, existing CustomModel behavior is preserved.
- Missing or invalid `parm1` or `parm2` follows existing CustomModel behavior.
- Tests must prove compressed and uncompressed santa and snowman examples resolve to equivalent Vixen model state.
- Tests must validate direct parser behavior and resulting Vixen state, not compressed-to-uncompressed string conversion.
- Reference documentation must describe both formats, precedence, fallback, coordinate rules, and equivalence expectations.

### Future State Effect Contract

- The State effect is out of scope for VIX-3591.
- The future State effect should persist the selected State definition ID as durable identity.
- The State effect may show the State definition name for display, but must not depend on name as durable identity.
- If the referenced State definition is missing or deleted, the effect must not render until the user selects a valid State definition.
- Within the selected State definition, State item activation keeps the current duplicate-name behavior.
- State effect documentation can reference this document when that feature branch is updated.

## Acceptance Criteria

### Data And Identity

- A newly created State property has at least one valid State definition through setup/default creation.
- State property, State definition, and State item IDs are non-empty.
- Logical clones preserve IDs.
- Cross-element property copy creates a distinct State property ID.
- State definition copy creates distinct State definition and State item IDs.
- Renames and ordinary edits preserve the existing stable IDs.
- Zero State definitions is invalid.

### Setup Dialog

- The setup dialog opens with the first State definition selected.
- Add, Delete, Rename, and Copy work from the State Definition controls.
- Add and Copy append at the end of the combo box list.
- Delete confirms and cannot delete the last definition.
- Rename changes the display name without changing State items or IDs.
- Copy preserves user-visible values but produces independent IDs and later edits do not affect the source.
- Invalid names show Catel validation feedback and prevent OK.
- Non-blocking warnings display without preventing OK.
- Cancel and window close discard draft edits.
- State item sorting persists after save/reopen.
- Sorted persistence preserves IDs, names, colors, and assignments.
- State item manual reorder with Move Up and Move Down persists after save/reopen.
- Manual reorder preservation keeps IDs, names, colors, and assignments.

### Assignment And Preview

- Selecting a State item row displays its assignment tree.
- Group checkbox behavior clears/disables descendants and counts effective leaves.
- Row selection expands only branches with checked descendants.
- Preview is off by default and sends no messages until enabled.
- State Item Group mode defaults to `<ALL>`.
- Selected State Item mode does not render until a valid row is selected.
- Preview updates correctly for selected-row mode, group mode, assignment edits, color edits, group selection changes, row add/remove/rename, and State definition switches.
- Preview cleanup occurs on Preview off and dialog close.

### Import

- Existing models without StateInfo continue to import as they do today.
- Existing FaceInfo and Submodel behavior is unchanged.
- Import creates the top-level group using current wildcard behavior.
- Import creates the child model group using ` - Model` plus current wildcard behavior.
- The State property is attached to the child model group.
- Each StateInfo becomes one State definition.
- Imported duplicate names are suffixed using dash notation.
- Imported StateInfo assignments map to child model group leaves.
- Legacy States groups are still created when the temporary constant is enabled.
- CustomModelCompressed and CustomModel create equivalent Vixen model state for equivalent inputs.
- Compressed is preferred when both formats are valid.
- Invalid compressed data falls back to valid CustomModel.
- No valid model data aborts import with a logged error and user-facing error.

## Automated Test Plan

Run focused tests throughout development:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights"
```

Run the full test project before final acceptance:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj
```

Run solution builds:

```powershell
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
```

Also run:

```powershell
git diff --check
```

Automated coverage must include:

- StateData default creation, normalization, clone, copy-for-new-property, and zero-definition invalid cases.
- StateDefinitionData default creation, normalization, clone, clone-as-new, rename ID preservation, and copy ID regeneration.
- StateItemData clone, clone-as-new, ID preservation, and assignment preservation.
- StateModule clone/copy behavior for IDs, names, descriptions, items, colors, and assignments.
- Setup draft OK and Cancel behavior.
- Add, Delete, Rename, and Copy State definition behavior.
- Default State item color selection for newly added rows and newly added State definitions on element trees with shared
  discrete colors.
- Default State item color fallback when discrete-color elements have no shared color.
- Add/Rename/Copy dialog view model validation for empty names, whitespace names, exact duplicates, case-only warnings, and unique names.
- Blocking selection changes while the current definition is invalid.
- OK disabled while any definition or item has blocking errors.
- Non-blocking warnings do not disable OK.
- State item grid sorted order persists after save.
- Sorted order preservation keeps item IDs, colors, and assignments.
- State item Move Up and Move Down command availability, selection retention, persisted order, and ID/assignment
  preservation.
- Assignment tree group selection, descendant clearing, disabling, effective leaf counts, and selected-row expansion.
- Preview coordinator pair diffing, duplicate pair deduplication, multi-color same-element repair, clear, and release.
- Mapper preview defaults, selected-row mode, group mode, group list ordering, group fallback, inactive-row suppression, inactive-definition suppression, and State definition switch behavior.
- Broadcast payload shape for turn-on, turn-off, clear, and release messages.
- xLights name normalization for blank names, duplicate suffixes, suffix collisions, and case-only names.
- CustomModel parser output.
- CustomModelCompressed parser output.
- Equivalent compressed/uncompressed santa-derived model data.
- Equivalent compressed/uncompressed snowman-derived model data.
- Compressed-first precedence.
- Invalid compressed fallback to valid CustomModel.
- Invalid compressed plus invalid/missing CustomModel import abort behavior.
- Import hierarchy: top-level group, child model group, FaceInfo groups, Submodel groups, optional legacy States groups, and State property attachment.
- Import without StateInfo creates no State property.
- Import without FaceInfo remains valid.
- Existing CustomModel behavior remains covered.

Tests must use embedded minimal mocked data based on sample files. They must not read reference `.xmodel` files directly.

## Manual Test Plan

### Display Setup Creation And Editing

1. Start Vixen from the Debug output.
2. Open Display Setup in a test profile.
3. Select a grouped prop with multiple child elements.
4. Add the State property.
5. Open State Property Setup.
6. Confirm one State definition exists and is selected.
7. Confirm the State definition has a valid generated name.
8. Add State item rows, edit names inline, assign colors, and select assignments.
9. Select a middle State item row, use Move Up and Move Down to create a non-sorted custom order, save, reopen, and confirm
   descriptions, rows, colors, assignments, counts, and manual order persisted.
10. Make additional edits, press Cancel, reopen, and confirm canceled edits were discarded.

### Validation

1. Enter a whitespace-only State definition name and confirm OK is blocked.
2. Enter a one- or two-character State definition name and confirm a warning appears but OK remains enabled.
3. Add State definitions named `Open` and `open` and confirm a warning appears without blocking OK.
4. Attempt Add/Rename/Copy with an exact duplicate name and confirm the modal dialog blocks OK.
5. Enter a whitespace-only State item name and confirm OK is blocked.
6. Correct the invalid State item name and confirm OK is enabled when no other errors exist.
7. Add duplicate State item names and confirm they are allowed.
8. On a prop whose leaf elements share at least one discrete color, add a State item and confirm its color defaults to the
   first shared discrete color.
9. On a prop whose discrete-color leaf elements have no shared color, add a State item and confirm its color defaults to white.
10. Confirm Cancel and window close remain available while errors exist and discard edits.

### State Definition Management

1. Add two State definitions and confirm they appear in creation order.
2. Rename one and confirm its State items remain unchanged.
3. Copy one and confirm the copy preserves visible values.
4. Edit the copy and confirm the source is unchanged.
5. Delete the selected definition and confirm the next or previous definition is selected.
6. Attempt to delete the final remaining definition and confirm the UI prevents it.
7. Sort the State item grid by Name, save, reopen, and confirm the displayed sorted order persisted.
8. Select a State item row and use Move Up and Move Down to create an order that is not alphabetical or count-sorted. Save,
   reopen, and confirm the manual row order persisted without changing colors or assignments.

### Assignment Tree

1. Select a State item row with no assignments and confirm the tree is collapsed or minimally expanded.
2. Check a child leaf and confirm the count updates.
3. Check a parent group and confirm checked descendants clear, descendants are disabled/grayed, and effective leaf count updates.
4. Uncheck the parent group and confirm descendants re-enable without restoring prior checks.
5. Save and reopen and confirm assignments are restored.
6. Select a row with checked descendants and confirm only branches needed to reveal checked nodes expand.

### Live Preview

1. Open State Property Setup with assigned State items.
2. Confirm Preview is off and no preview lights activate.
3. Confirm State Item Group mode defaults to `<ALL>`.
4. Turn Preview on and confirm all rows in the selected definition preview.
5. Select a named State Item Group and confirm exact-name matches preview.
6. Switch to Selected State Item mode without selecting a row and confirm no preview renders.
7. Select a row and confirm only that row previews.
8. Change assignments and colors for the active row/group and confirm preview updates.
9. Change assignments and colors for inactive rows and confirm preview does not change.
10. Configure overlapping assignments with same and different colors and confirm duplicate pairs are deduplicated while multiple colors remain active.
11. Switch State definitions while Preview is on and confirm the previous preview clears and the new definition previews.
12. Turn Preview off and confirm the `"State Preview"` context clears.
13. Close through OK, Cancel, and window close in separate runs and confirm no stale preview remains.

### xModel Import

1. Import a model with only `CustomModel` and no StateInfo. Confirm existing behavior remains.
2. Import an equivalent model with `CustomModelCompressed`. Confirm element structure and node mappings match.
3. Import a model containing both formats and confirm compressed is preferred.
4. Import a model with invalid compressed data and valid CustomModel data and confirm import succeeds using CustomModel.
5. Import a model with no valid model source and confirm import aborts with a user-facing error and logged error.
6. Import a model with multiple StateInfo tags.
7. Confirm the top-level group uses existing wildcard naming, for example `Santa Waving {1}`.
8. Confirm the child model group uses ` - Model`, for example `Santa Waving - Model {1}`.
9. Confirm the State property is attached to the child model group.
10. Confirm each StateInfo appears as one State definition in import order.
11. Confirm duplicate StateInfo names are suffixed with ` - 2`, ` - 3`, etc.
12. Confirm StateInfo assignments target leaves under the child model group.
13. Confirm FaceInfo groups remain top-level children.
14. Confirm Submodel groups remain top-level children.
15. With the temporary constant enabled, confirm old States groups remain top-level children.
16. Import models missing FaceInfo and confirm import remains valid.
17. Import models missing StateInfo and confirm no State property is created solely for this feature.

## Out Of Scope

- State effect implementation.
- State effect rendering, discovery, Mark Collection integration, or Effect Editor UI.
- Adding `State` to `MarkCollectionType`.
- State effect playback modes such as countdown, time countdown, or number.
- User-selectable State effect overlap strategies.
- Persisting preview toggle, preview mode, selected preview group, or active preview state.
- UI/configuration exposure for the temporary legacy States group constant.
- Legacy migration for unreleased development State data.

## Notes For Jira

This document is intended to replace separate milestone-by-milestone Jira updates with one rolled-up final state for
VIX-3591. It should be pasted into the Jira issue description or a final requirements comment after review.

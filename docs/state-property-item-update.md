# State Property State Item Preview Updates

## Overview

The State Property Setup dialog currently previews the selected State item by broadcasting Live Preview messages when the
selected State item row changes. The existing work is an initial spike and may be refactored as needed. It does not yet update
the preview when the user checks or unchecks assigned elements, change colors, or preview multiple related State items
together.

This phase adds an explicit Preview toggle, two preview modes, State Item Group selection, and deterministic preview refresh
behavior. The user will be able to turn previewing on when needed, preview one State item or a named group of State items, and
see Live Preview update immediately as assignments, colors, rows, or preview controls change.

This is phase 3 on top of `docs\vix-3591-state-feature.md` and
`docs\vix-3591-state-property-prerequisites.md`. The implementation plan must follow `.agents\PLANS.md`.

## Terminology

- A **State property** is the configured property attached to an element node. It contains an overall name, a description,
  and an ordered list of State items.
- A **State item** is one editable row in State Property Setup. It has a case-sensitive name, a color, and assigned element
  nodes.
- An **effective leaf element** is a leaf element included by a State item's assignment tree. Checking a group element
  includes its descendant leaves.
- The **selected State item** is the row currently selected in the State item grid.
- A **State Item Group** is the set of State items whose names exactly match a selected group name, including letter casing.
- The **preview selected set** is the set of State items considered active by the current Preview Mode.
- An **active preview state** is one `(element ID, color)` pair currently activated in the `"State Preview"` Live Preview
  context.

## User Interface Requirements

Add a Preview controls section above the State item grid.

### Preview Toggle

- Add a WPF `ToggleButton` with a separate label containing the text `Preview`.
- The toggle button displays `Off` when previewing is disabled and `On` when previewing is enabled.
- Preview defaults to `Off` whenever the State Property Setup dialog opens.
- The toggle remains enabled regardless of name validation errors.
- When Preview is `Off`, Preview Mode controls and the State Item Group combo box are disabled.
- While Preview is `Off`, changing grid selections, rows, assignments, or colors must not publish Live Preview messages.
- Turning Preview `On` immediately evaluates the retained Preview Mode and previews its selected set.
- Turning Preview `Off` immediately clears all effects in the `"State Preview"` context.

### Preview Mode

Add a radio button group labeled `Preview Mode` with these options:

- `Selected State Item`
- `State Item Group`

`Selected State Item` is the default mode whenever the dialog opens. The selected mode is temporary dialog state and is not
persisted to the State property.

When Preview is `On`, changing Preview Mode must clear the `"State Preview"` context, recompute the preview selected set, and
activate the newly desired preview states. When Preview is `Off`, the controls are disabled and the retained selection does
not produce messages.

### State Item Group Selection

- Show a combo box when Preview Mode is `State Item Group`.
- Keep the combo box visible but disabled when Preview is `Off`.
- Hide the combo box when Preview Mode is `Selected State Item`.
- Populate the combo box with `<ALL>` followed by distinct State item names.
- `<ALL>` must use exactly that capitalization and angle-bracket syntax.
- `<ALL>` is the default combo box selection whenever the dialog opens and whenever the user first selects
  `State Item Group`.
- Keep `<ALL>` as the only option when there are no State items.
- Populate names from the editable draft after State item name trimming and validation.
- Treat names as case-sensitive. For example, `Open` and `open` are distinct choices.
- Deduplicate exact name matches.
- Order named choices by their first appearance in the State item grid.
- Rebuild the choices immediately after a State item is added, removed, or renamed.
- The selected group is temporary dialog state and is not persisted to the State property.

When the selected named group still exists after a row rename or removal, retain that selected group and recalculate the
preview. When the selected named group no longer exists, fall back to `<ALL>`. If Preview is `On`, refresh immediately using
the resulting `<ALL>` selection.

## Preview Selected Set Rules

When Preview Mode is `Selected State Item`, the preview selected set contains only the selected State item row. If there is
no selected row, the set is empty.

When Preview Mode is `State Item Group` and a named group is selected, the preview selected set contains every State item
whose trimmed name matches the selected group name exactly, including letter casing.

When Preview Mode is `State Item Group` and `<ALL>` is selected, the preview selected set contains all State items.

Selecting a different grid row while Preview Mode is `State Item Group` must not affect the preview. Grid selection continues
to control which State item's assignment tree is displayed for editing.

## Active Preview State Rules

For each State item in the preview selected set, evaluate its effective leaf elements and its assigned color. The desired
preview state is the set of `(element ID, color)` pairs produced by that evaluation.

- Deduplicate pairs that have the same element ID and the same color.
- Preserve pairs that have the same element ID but different colors. Live Preview supports activating the same element with
  multiple colors.
- Do not publish a message when the applicable state collection is empty.
- Use the State item's color translated to hex with the `ToHex()` color extension.
- Use an intensity of `100`.
- Use `int.MaxValue` as the duration for each activated preview state.

The implementation must retain enough dialog-local active preview state to compare the currently activated pairs with newly
desired pairs. This state is temporary and must not be persisted to the State property.

## Incremental Refresh Behavior

Ordinary edits must refresh the preview incrementally rather than clear the entire context. Compare the desired
`(element ID, color)` pairs with the active preview pairs and publish only the required updates.

### Additions

For newly desired pairs that are not active, publish a `TurnOnElementsMessage`. Include one `ElementState` for each new pair.

### Removals

`TurnOffElementsMessage` terminates effects by element ID rather than by `(element ID, color)` pair. When any active pair for
an element ID must be removed:

1. Publish a `TurnOffElementsMessage` for that element ID.
2. Remove all active pairs for that element ID from the dialog-local active preview state.
3. Publish a `TurnOnElementsMessage` to reactivate every desired pair that remains for that element ID.

This rule preserves remaining colors when a multi-color element loses only one active color.

If an incremental refresh contains removals and additions, publish the required turn-off message before publishing turn-on
messages. Deduplicate element IDs in the turn-off payload.

### Color Changes

Changing the color of an active State item removes its prior `(element ID, color)` pairs and activates its new pairs
immediately. Apply the same removal rule so overlapping colors from other active State items remain active.

Changing the color of an inactive State item must not publish a message.

## Full Refresh And Cleanup Behavior

Use `ClearActiveEffectsMessage` with the context name `"State Preview"` for lifecycle boundaries where a complete reset is
clearer and more reliable than an incremental diff.

Clear the context and reset the dialog-local active preview state:

- when Preview changes from `On` to `Off`;
- before rebuilding the preview after Preview Mode changes while Preview is `On`;
- when the dialog closes through OK;
- when the dialog closes through Cancel;
- when the user closes the window through the window close button.

After a Preview Mode change while Preview is `On`, recompute the preview selected set and publish the applicable
`TurnOnElementsMessage` after clearing the context.

Always publish cleanup when the dialog closes, even when Preview is already `Off`. Cleanup is intentionally idempotent and
protects against stale State Preview effects.

Add a concise implementation comment at the dialog-close cleanup hook noting that a future Live Preview broadcast message
should release the named context entirely. Releasing a named Live Preview context is out of scope for this phase because the
broadcast API does not currently provide that message.

## Refresh Triggers

When Preview is `On`, refresh the active preview state as follows:

| Trigger | Required behavior |
| --- | --- |
| Toggle Preview from `Off` to `On` | Evaluate the current mode and activate its desired preview pairs. |
| Toggle Preview from `On` to `Off` | Clear the `"State Preview"` context. |
| Change Preview Mode | Clear the context, recompute the selected set, and activate its desired pairs. |
| Change the State Item Group combo selection | Refresh to the newly selected group's desired pairs. |
| Select a different grid row in `Selected State Item` mode | Refresh to the newly selected row's desired pairs. |
| Select a different grid row in `State Item Group` mode | Do not change the preview. |
| Check or uncheck an assignment-tree node for an active State item | Incrementally refresh affected effective leaves. |
| Check or uncheck an assignment-tree node for an inactive State item | Do not publish messages. |
| Change the color of an active State item | Incrementally replace prior pairs with the newly colored pairs. |
| Change the color of an inactive State item | Do not publish messages. |
| Add a row in `Selected State Item` mode | Preserve the existing behavior of selecting the new row and refresh to its desired pairs. A new row has no assignments, so publish no turn-on message until elements are assigned. |
| Remove the active row in `Selected State Item` mode | Refresh to the fallback selected row, or to an empty selected set if no row remains. |
| Add, remove, or rename a row in named `State Item Group` mode | Rebuild group choices. Refresh when the edit affects the selected group or causes fallback to `<ALL>`. |
| Add, remove, or rename a row while `<ALL>` is selected | Rebuild group choices and refresh because every row is active. |
| Close the dialog through OK, Cancel, or window close | Clear the `"State Preview"` context. |

When Preview is `Off`, none of these editing triggers may publish Live Preview messages. The Preview Mode radio buttons and
State Item Group combo box are disabled while Preview is `Off`.

## Broadcast Live Preview Integration

- Publish messages through `Common.Broadcast.Broadcast.Publish`.
- Use Live Preview message types from the `Common.Messages.LivePreview` namespace.
- Use channel names from `LivePreviewChannels`.
- Add a State-module constant for the context name `"State Preview"` and use it for all State Property Setup preview
  messages.
- The State Property module references the lightweight `Broadcast` and `Messages` projects. It must not reference the
  `Vixen.Modules.App.LivePreview` project.

To activate preview pairs, publish `TurnOnElementsMessage` on `LivePreviewChannels.TurnOnElements`. The payload contains one
`ElementState` per `(element ID, color)` pair:

- `Id`: the effective leaf element ID;
- `Color`: the assigned State item color translated with `ToHex()`;
- `Intensity`: `100`;
- `Duration`: `int.MaxValue`;
- `ContextName`: `"State Preview"`.

To deactivate element IDs during incremental refresh, publish `TurnOffElementsMessage` on
`LivePreviewChannels.TurnOffElements`. The payload contains one `ElementState` per distinct element ID:

- `Id`: the effective leaf element ID;
- `Color`: `string.Empty`;
- leave `Intensity` and `Duration` at their default values;
- `ContextName`: `"State Preview"`.

To clear the State Property Setup preview, publish `ClearActiveEffectsMessage` on
`LivePreviewChannels.ClearActiveEffects` with `ContextName` set to `"State Preview"`.

## Implementation Guidance

The current uncommitted edits in
`src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs` and
`src\Vixen.Modules\Property\State\State.csproj` are an initial spike. Inspect and preserve useful behavior, but refactor as
needed rather than assuming that implementation is the desired final design.

The existing `StateMapperViewModel` owns dialog-level state and the existing `StateItemViewModel` receives assignment changes
from `StateAssignmentTreeNode`. The implementation may keep preview coordination in `StateMapperViewModel` or extract a
focused internal helper if doing so keeps responsibilities clear and makes preview diff behavior easier to test. Do not add
persisted preview settings to `StateData` or `StateItemData`.

Do not infer active preview state from what was last sent to Live Preview. Maintain explicit dialog-local state so overlap
handling, removal, and reactivation behavior are deterministic.

Use the project skills `dotnet-best-practices`, `csharp-async`, `csharp-docs`, `dotnet-design-pattern-review`, and
`catel-mvvm` while creating the implementation plan and making code changes. Update XML documentation when adding or
modifying public or protected C# APIs.

## Automated Test Expectations

Add focused automated tests for the preview coordinator or State mapper behavior. Tests must cover:

- Preview defaults to `Off`.
- Preview Mode defaults to `Selected State Item`.
- State Item Group defaults to `<ALL>`.
- Turning Preview `On` immediately previews the selected row.
- Turning Preview `On` with no State items publishes no turn-on message.
- Turning Preview `Off` publishes a clear-context message.
- Mode changes while Preview is `On` clear the context and activate the desired preview pairs.
- Grid-row selection refreshes `Selected State Item` mode.
- Grid-row selection does not refresh `State Item Group` mode.
- Exact duplicate group names appear once in the group list.
- Case-different group names remain separate group choices.
- Named groups use first-grid-appearance ordering.
- `<ALL>` remains available when there are no State items.
- Renaming or removing a row retains the selected group when another exact-name match remains.
- Renaming or removing the final row in a selected group falls back to `<ALL>`.
- `<ALL>` refreshes after row additions, removals, and renames.
- Checking or unchecking assignments refreshes active State items.
- Assignment edits on inactive State items publish no messages.
- Color changes refresh active State items.
- Color changes on inactive State items publish no messages.
- Duplicate `(element ID, color)` pairs publish only one turn-on state.
- The same element ID with different colors publishes multiple turn-on states.
- Removing one color from a multi-color element turns off the element ID and reactivates the remaining colors.
- Incremental refresh publishes turn-off messages before turn-on messages when both are required.
- Turn-off payloads deduplicate element IDs.
- Turn-on payloads use the assigned hex color, intensity `100`, duration `int.MaxValue`, and context name
  `"State Preview"`.
- Turn-off payloads use the element ID, `string.Empty` color, default intensity and duration, and context name
  `"State Preview"`.
- Empty updates do not publish messages.
- OK, Cancel, and window-close cleanup publish a clear-context message.

Retain and run the existing State Property tests to ensure preview changes do not regress draft editing, validation,
assignment-tree behavior, stable IDs, cloning, copying, or xLights import normalization.

## Manual Acceptance Scenarios

### Preview Toggle And Selected State Item

1. Open State Property Setup for an element with two configured State items and assigned leaves.
2. Confirm the Preview toggle reads `Off`, Preview Mode defaults to `Selected State Item`, and Preview Mode controls are
   disabled.
3. Confirm no State Preview lights are active.
4. Turn Preview `On`.
5. Confirm the toggle reads `On`, Preview Mode controls become enabled, and the selected row's effective leaf elements turn
   on using that row's color.
6. Select another row.
7. Confirm the previous preview is removed and the new row's effective leaf elements turn on.
8. Turn Preview `Off`.
9. Confirm all State Preview lights turn off immediately.

### Assignment And Color Editing

1. Turn Preview `On` in `Selected State Item` mode.
2. Check an unassigned leaf or group node in the selected row's assignment tree.
3. Confirm the applicable effective leaves turn on immediately.
4. Uncheck the node.
5. Confirm the applicable leaves turn off immediately.
6. Change the selected row's color.
7. Confirm its preview updates immediately after the color picker commits the new color.
8. Edit assignments and colors in a row that is not active.
9. Confirm the visible preview does not change.

### State Item Group Preview

1. Configure rows named `Open`, `Open`, and `open`.
2. Turn Preview `On`, select `State Item Group`, and confirm the combo box defaults to `<ALL>`.
3. Confirm all three rows preview.
4. Select `Open`.
5. Confirm both `Open` rows preview and the `open` row does not.
6. Select a different grid row and confirm the preview does not change.
7. Rename one `Open` row and confirm the other `Open` row remains previewed.
8. Rename the last `Open` row and confirm the combo box falls back to `<ALL>` and all remaining rows preview.

### Overlapping Colors

1. Configure two active rows that assign the same leaf with the same color.
2. Confirm the leaf is activated only once for that color.
3. Change one row to a different color.
4. Confirm the leaf previews both colors.
5. Remove one assignment.
6. Confirm the leaf is turned off and then reactivated with its remaining color.

### Dialog Cleanup

1. Turn Preview `On` and activate one or more leaves.
2. Close the dialog with Cancel and confirm the State Preview lights turn off.
3. Repeat with OK and confirm the State Preview lights turn off.
4. Repeat with the window close button and confirm the State Preview lights turn off.

## Out Of Scope

- Do not persist Preview toggle, Preview Mode, selected group, or active preview states.
- Do not add a Live Preview broadcast message that releases a named context. Add only the comment hook described above for
  that future enhancement.
- Do not implement the State effect.
- Do not change State effect discovery, rendering, Mark Collection handling, or Effect Editor controls.
- Do not add `State` to `MarkCollectionType`.
- Do not add user-selectable overlap strategies.

## Jira VIX-3591 Phase 3 Append Block

Paste the following markdown into Jira issue `VIX-3591` before implementation:

---

### Phase 3: State Property Live Preview Updates

#### Goal

Enhance State Property Setup so users can explicitly enable Live Preview, preview either one selected State item or a named
State Item Group, and immediately see assignment and color edits reflected in the Live Preview.

#### Requirements

- Add a `Preview` WPF toggle button that displays `Off` or `On`. Preview defaults to `Off`.
- Disable Preview Mode controls while Preview is off.
- Add Preview Mode options `Selected State Item` and `State Item Group`. Default to `Selected State Item`.
- Add a State Item Group combo box containing `<ALL>` plus distinct, case-sensitive State item names in first-grid-appearance
  order. Default to `<ALL>`.
- Preview all exact-name matches for a selected named group. Preview all rows for `<ALL>`.
- Immediately refresh preview state after applicable row selections, group selections, assignment changes, color changes,
  additions, removals, and renames.
- Deduplicate active preview states by `(element ID, color)`. Preserve multiple colors assigned to the same element.
- Use incremental turn-off and turn-on Live Preview messages for ordinary edits.
- Clear the isolated `"State Preview"` context when Preview turns off, Preview Mode changes, or the dialog closes through
  OK, Cancel, or window close.
- Keep preview state temporary. Do not persist preview controls or active preview states.

#### High-Level Design

Maintain dialog-local active preview state as `(element ID, color)` pairs. Compute desired pairs from the effective leaf
elements assigned to the State items selected by the current Preview Mode. Diff desired pairs against active pairs for
ordinary refreshes.

Live Preview turn-off operates by element ID rather than color. If one color is removed from a multi-color element, first
turn off that element ID and then reactivate any desired colors that remain for it.

Publish `TurnOnElementsMessage`, `TurnOffElementsMessage`, and `ClearActiveEffectsMessage` through `Common.Broadcast` using
the `LivePreviewChannels` constants and the `"State Preview"` context name. Keep the State Property module independent of the
Live Preview app module.

#### Acceptance Criteria

- Preview is off by default and no State Preview lights activate until the user turns it on.
- Selected State Item mode previews only the selected grid row.
- State Item Group mode previews exact-name matches or every row when `<ALL>` is selected.
- Case-different names remain separate groups.
- Checking and unchecking assigned elements updates active preview leaves immediately.
- Changing an active State item's color updates its preview immediately.
- The same element can preview multiple colors while exact duplicate element-and-color pairs are activated only once.
- Turning Preview off and closing the dialog clear all `"State Preview"` effects.

#### Testing

- Add automated tests for toggle defaults, mode behavior, group-list behavior, incremental additions and removals,
  assignment updates, color updates, overlap handling, empty updates, message payloads, and cleanup paths.
- Run the existing State Property test suite to protect draft editing, validation, assignment trees, stable IDs, cloning,
  copying, and xLights import behavior.
- Perform manual acceptance checks for selected-row preview, group preview, overlapping colors, toggle cleanup, and dialog
  close cleanup.

#### Follow-Up Note

The current Live Preview broadcast API can clear a named context but cannot release it. Phase 3 should leave a concise code
comment at the dialog-close cleanup hook for a future broadcast enhancement that releases the `"State Preview"` context
entirely.

---

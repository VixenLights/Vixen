# VIX-3591 State Property Phase 4 Specification

## Goal

Complete the State Property feature by changing a State property from a single named state into a container for one or more
State definitions. Each State definition behaves like the current single State property behavior: it has a stable identity,
a required name, an optional description, and a collection of State items that map colors to element assignments.

The setup dialog must allow users to add, remove, rename, copy, select, edit, validate, and preview State definitions. xModel
import must create one State property container on the imported prop and create one State definition for each imported
`StateInfo`.

This document replaces the earlier phase outline and is intended to be detailed enough to create the implementation ExecPlan.

## Prior Documents

- `docs\plans\vix-3591-state-feature.md`
- `docs\plans\vix-3591-state-property-prerequisites-phase-2.md`
- `docs\plans\vix-3591-state-property-preview-phase-3.md`
- `docs\vix-3591-state-property-prerequisites.md`
- `docs\state-property-item-update.md`

## Terminology

- A **State property** is the property attached to a prop element. In Phase 4 it is only a container and no longer has its
  own user-visible name or description. For xModel import, it is attached to the CustomModel child group.
- A **State definition** is one named state inside the State property. It owns a stable ID, name, description, ordered State
  item collection, and all behavior that previously belonged to the single State property.
- A **State item** is one row inside a State definition. It has its existing stable ID, name, color, and assigned element
  nodes.
- A **prop** is the top-level imported or user-created element group that owns the State property.
- A **copy-as-new operation** creates a distinct logical State definition and must generate new stable IDs.
- A **logical clone** is an editing, serialization, or draft copy of the same logical object and must preserve stable IDs.

## Data Model Requirements

### State Property Container

- `StateData` must become the State property container.
- `StateData` must expose a `StateDefinitions` collection with one or more State definitions.
- The State property container must not expose or persist its own user-visible name or description.
- The State property is invalid when it contains zero State definitions.
- State definitions must remain in fixed creation order unless the user explicitly deletes definitions. Reordering is out of
  scope for Phase 4.
- No legacy migration is required. This feature is still in development and existing test data may be recreated.

### State Definition

- Add a maintainable State definition data type rather than overloading unrelated State item concepts. The implementation
  should favor clear ownership of identity, name, description, and item collection over minimizing file count.
- Each State definition must have a persisted stable `Guid`.
- New State definitions must receive a non-empty stable ID on construction.
- Renaming a State definition must preserve its stable ID.
- Editing description, State items, colors, or assignments must preserve the State definition stable ID.
- Logical clones used for editing, persistence, or serialization must preserve the State definition stable ID.
- Copy-as-new operations must generate a new State definition stable ID.
- Copy-as-new operations must also generate new stable IDs for copied State items.
- Existing State item stable ID behavior remains unchanged within each State definition.

### Default Creation

- Adding a State definition manually must prompt for a valid unique name.
- Generated default State definition names must use dash notation: `State - 1`, `State - 2`, and so on.
- Generated names must increment until they are unique within the State property.
- A newly added State definition must start with one default State item row using the current default State item name and
  default color behavior.

## State Definition Naming And Validation

- State definition names are required.
- Leading and trailing whitespace must be trimmed before validation is accepted.
- Whitespace-only State definition names must block saving or accepting a modal dialog.
- Exact duplicate State definition names are not allowed in the setup UI.
- State definition name uniqueness is case-sensitive. `Open` and `open` are different names.
- State definition names that differ only by casing must show a non-blocking warning because they may be a typo.
- The warning for case-only differences applies only across State definition names.
- State item duplicate-name behavior remains scoped within a single State definition and keeps the behavior defined in prior
  phases.
- Add, Rename, and Copy dialogs must validate names modally and must not close with OK until the proposed State definition
  name is non-empty and unique according to these rules.

## Setup Dialog Requirements

### State Definition Selector

- Add a combo box for selecting the active State definition.
- The combo box must list State definitions in fixed creation order.
- The first State definition must be selected when the State Property Setup dialog opens.
- Switching the combo box selection must update the visible description, State items, assignment tree, and Preview controls
  to the selected State definition.
- If the current State definition has blocking validation errors, switching selection must be blocked and focus must remain
  on the invalid edit.
- If Preview is on and the user switches to another valid State definition, the `"State Preview"` context must be cleared
  and re-rendered for the newly selected State definition.

### Add State Definition

- Provide an Add command that opens a modal name-entry dialog.
- The dialog must suggest the next generated name using dash notation.
- The dialog must block OK until the name is valid and unique.
- Accepting the dialog creates a new State definition at the end of the collection, with a new stable ID and one default
  State item row.
- The newly added State definition becomes the selected State definition.

### Delete State Definition

- Provide a Delete button for the selected State definition.
- Deleting must show a confirmation dialog before changing the draft.
- The State property must always contain at least one State definition. The last remaining State definition cannot be
  deleted.
- After deleting the selected State definition, select the next definition when one exists; otherwise select the previous
  definition.
- Deletion is applied to the setup dialog draft. Closing with Cancel must discard the deletion. Closing with OK must persist
  the deletion.

### Rename And Copy Flyout

- Provide an `...` button beside the State definition selector.
- The `...` button opens a flyout containing Rename and Copy commands.
- Rename opens a modal name-entry dialog initialized with the selected State definition name.
- Rename must preserve the selected State definition stable ID.
- Copy opens a modal name-entry dialog for the new copied State definition name.
- Copy must block OK until the name is valid and unique.
- Copy creates a new State definition at the end of the collection using the selected State definition as the source.
- Copy must generate a new State definition stable ID and new State item stable IDs.
- Copy must preserve user-visible State item names, descriptions, colors, and assignments.
- The copied State definition becomes the selected State definition.

### Existing State Item Editing

- The selected State definition's description and State items must behave like the current single State property behavior.
- All validation, trimming, assignment, color, and preview behavior from prior phases applies to the selected State
  definition only.
- OK must remain disabled while the selected State definition or any State definition in the container has a blocking
  validation error.
- Cancel and window close must remain available and must discard draft edits.

## Preview Requirements

- Phase 3 Preview controls apply only to the currently selected State definition.
- Preview defaults and mode behavior remain unchanged from Phase 3.
- Switching selected State definitions while Preview is off must not publish Live Preview messages.
- Switching selected State definitions while Preview is on must clear the `"State Preview"` context and activate the desired
  preview state for the newly selected definition.
- Closing the dialog through OK, Cancel, or window close must clear the `"State Preview"` context.

## xLights Import Requirements

### Imported Element Hierarchy

- xModel import must continue creating the top-level group with the imported model name and the current wildcard naming
  behavior exactly as it does today.
- The top-level group naming convention must remain unchanged. For example, importing `Santa Waving` currently produces a
  top-level group such as `Santa Waving {1}`.
- The CustomModel element structure, whether parsed from `CustomModel` or `CustomModelCompressed`, must be created as a
  child group under the top-level imported model group.
- The CustomModel child group must follow the same wildcard naming convention as the top-level group, but must include the
  word `Model` in the name. For example, under `Santa Waving {1}`, the CustomModel group should be named
  `Santa Waving - Model {1}`.
- The State property container must be attached to the CustomModel child group, not the top-level imported model group. For
  example, the State property is attached to `Santa Waving - Model {1}`.
- `StateInfo` node mappings must target the child elements created under the CustomModel child group.
- `FaceInfo` groups must remain children of the top-level imported model group, as they are today.
- Submodel groups must remain children of the top-level imported model group, as they are today.
- The optional old `States` groups controlled by the temporary import constant must also remain children of the top-level
  imported model group.
- `StateInfo` and `FaceInfo` are optional in xModels. Missing either tag type must not fail import.
- xModels that do not contain `StateInfo` must continue to import as they do today, without creating a State property
  container solely for this feature.
- xModels that do not contain `FaceInfo` must continue to import as they do today, without creating Face groups.
- xModels that contain neither `StateInfo` nor `FaceInfo` must continue through the existing CustomModel import behavior.
- Existing Submodel import behavior must remain unchanged.

### StateInfo Import

- xModel import must create a single State property container on the CustomModel child group.
- Each imported `StateInfo` must create one State definition inside that State property.
- The State definition name must come from the `StateInfo` `Name` attribute using the same base naming behavior as the
  current importer.
- If the `StateInfo` name is missing, empty, or whitespace-only, use `State - N`, incrementing until unique.
- If imported State definition names collide exactly, including casing, keep the first name unchanged and suffix subsequent
  collisions using dash notation, starting with `Name - 2`.
- If a suffixed name already exists, increment until a unique name is found.
- Case-only differences such as `Open` and `open` are allowed during import.
- Imported State definitions must receive new non-empty stable IDs.
- Imported State items must keep their existing stable ID behavior and imported mapping semantics.
- `StateInfo` node mappings must map into the leaf elements created under the CustomModel child group.
- `StateInfo` import must no longer require creating a separate `States` element group for the new behavior.

### Temporary Old Group Creation Constant

- Add an internal importer constant near the xModel importer that controls whether the old `States` element groups are
  created.
- The constant must default to `true` during Phase 4 implementation and validation.
- When the constant is `true`, create the old elements as they are created today, without adding extra State property logic
  to those elements.
- Add a clear removal comment explaining the constant exists only to validate Phase 4 against the previous import behavior
  and should be turned off and removed after the new State property import is accepted.
- Do not expose this constant in UI or user configuration.

### FaceInfo Import

- `FaceInfo` import remains unchanged for all xModels.
- Any existing FaceInfo element structure behavior must continue to function independently of the State property changes.
- Missing `FaceInfo` data must remain a valid import case.

## CustomModel And CustomModelCompressed Requirements

### Existing CustomModel Behavior

- Preserve the current `CustomModel` import behavior and element ordering.
- `parm1` and `parm2` define the rectangular grid dimensions as they do today.
- Numeric cells map to the same Vixen leaf ElementNode numbering used by current `StateInfo` mapping.
- Import must first resolve the model source from `CustomModelCompressed` or `CustomModel`, then decode that source directly
  into the internal Vixen model representation used for element creation and mapping.
- `CustomModelCompressed` and `CustomModel` are alternate encodings of the same model data. Do not convert compressed text
  into `CustomModel` text as an intermediate step. Each parser should take the direct route from its source format to the
  shared Vixen class data.
- After source decoding, no separate business logic path should exist for StateInfo mapping, element hierarchy creation,
  naming, or child element ordering.

### Compressed Format Support

- Add exact-compatible support for xLights `CustomModelCompressed`.
- xLights source code is the behavior standard. Translate the relevant `CustomModel::ToCompressed` and
  `CustomModel::ToCustomModel` behavior into C# parsing/conversion logic.
- Support both current and older xLights compressed variants represented by that source behavior.
- Fully document both `CustomModel` and `CustomModelCompressed`, including equivalence rules, in a reference document for
  future xModel XML expansion.

### Attribute Precedence And Errors

- xModels may contain both `CustomModel` and `CustomModelCompressed` for backward compatibility.
- When both attributes exist, prefer `CustomModelCompressed`.
- When compressed data is preferred, decode it directly into the same internal Vixen representation consumed by the
  downstream import logic before creating elements.
- When both attributes exist, tests must prove the compressed and uncompressed forms in the santa and snowman examples
  resolve to the same model result.
- If `CustomModelCompressed` is invalid fall back to `CustomModel` if it exists.
- If `CustomModelCompressed` and `CustomModel` are invalid abort the entire xModel import, show a user-facing error, and log the error.
- Missing or invalid `parm1` or `parm2` must behave the same as existing `CustomModel` import behavior.
- If only `CustomModel` exists, use the existing `CustomModel` path.
- All other xModel import flows must remain functional.

## Reference Examples

Reference xModels used during design:

- `docs\references\santa-waving.xmodel`
- `docs\references\snowman-6ft.xmodel`
- `docs\references\teddy.xmodel`
- `docs\references\pumpkin.xmodel`

Automated tests must not read or depend on these files directly. Tests should embed minimal mocked XML/model data extracted
from the sample files or reduced from them.

## Future State Effect Contract

The State effect is out of scope for Phase 4, but Phase 4 must leave a stable contract that the future State effect can
reference.

- The State effect should persist a stable reference to the selected State definition ID.
- The State effect may use the State definition name for display, but not as the durable identity.
- If the referenced State definition is deleted or cannot be found, the State effect must not render until the user selects
  a valid State definition.
- Within the selected State definition, State item activation must retain the current behavior. Duplicate State item names
  remain meaningful within that State definition and activate together according to the State effect rules.
- State effect documentation should be updated in its own feature branch after Phase 4 lands.

## Automated Test Expectations

- Newly constructed State property data contains at least one valid State definition when created through the setup/default
  creation path.
- Newly constructed State definitions receive non-empty stable IDs.
- Renaming a State definition preserves its stable ID.
- Editing description, State items, colors, or assignments preserves the State definition stable ID.
- Logical cloning preserves State definition and State item stable IDs.
- Copy-as-new creates a new State definition ID and new State item IDs while preserving user-visible copied values.
- The setup dialog blocks deleting the last State definition.
- Deleting a selected State definition selects the next definition or the previous definition when no next definition exists.
- Add, Rename, and Copy dialogs block empty, whitespace-only, and exact duplicate names.
- Case-only State definition name differences produce a non-blocking warning and do not disable OK.
- State definitions remain in creation order after add, copy, rename, and delete operations.
- Switching State definitions is blocked while the current definition has blocking validation errors.
- Switching State definitions while Preview is on clears and re-renders the `"State Preview"` context.
- Preview behavior remains scoped to the selected State definition.
- xModel import creates one State property container on the CustomModel child group.
- xModel import creates one State definition for each `StateInfo`.
- Imported empty StateInfo names receive `State - N` fallback names.
- Imported exact duplicate StateInfo names receive `Name - 2`, `Name - 3`, and so on until unique.
- Imported case-only StateInfo names remain distinct.
- Imported StateInfo node mappings target the leaf elements under the CustomModel child group.
- The old `States` group creation constant defaults to `true` and preserves old element creation without extra State
  property logic.
- `FaceInfo` import behavior remains unchanged.
- `CustomModelCompressed` produces the same element result as equivalent `CustomModel` data.
- Minimal test data based on the santa and snowman examples proves both formats decode to equivalent internal Vixen model
  state.
- Tests must mirror the importer behavior by parsing each source format directly into the shared Vixen representation and
  validating that the same Vixen state is achieved.
- Tests must not validate compressed behavior by converting compressed text into `CustomModel` text and comparing strings.
- xModels containing both `CustomModel` and `CustomModelCompressed` prefer the compressed attribute.
- Invalid compressed data falls back to a valid `CustomModel` source when one exists.
- Imports with no valid compressed or uncompressed model source abort the entire xModel import, log an error, and surface a
  user-facing error.
- Existing `CustomModel` tests continue to pass.

## Manual Acceptance Scenarios

### Manual State Definition Editing

1. Add a State property to a prop.
2. Confirm the setup dialog contains one selected State definition with a valid generated name.
3. Add a State definition and confirm it appears at the end of the combo box list.
4. Rename the selected State definition and confirm the name is updated without changing its configured State items.
5. Copy the selected State definition and confirm copied user-visible values are preserved while subsequent edits do not
   affect the source definition.
6. Attempt to rename or copy to an exact duplicate name and confirm the modal dialog blocks OK.
7. Create names that differ only by casing and confirm a warning appears without blocking save.
8. Delete a State definition and confirm the next or previous definition is selected.
9. Attempt to delete the last remaining State definition and confirm the UI prevents it.
10. Save and reopen the setup dialog. Confirm State definitions, descriptions, State items, colors, assignments, and order
    persisted correctly.

### Preview Across State Definitions

1. Configure two State definitions with different State item assignments and colors.
2. Turn Preview on for the first State definition.
3. Switch to the second State definition and confirm the previous State Preview lights clear and the second definition
   previews.
4. Turn Preview off and switch definitions. Confirm no Live Preview messages activate lights.
5. Close the dialog through OK, Cancel, and window close in separate runs and confirm the `"State Preview"` context clears.

### xLights Import

1. Import an xModel with multiple `StateInfo` tags.
2. Confirm one State property exists on the CustomModel child group.
3. Confirm each `StateInfo` appears as one State definition in import order.
4. Confirm State item mappings target the leaf elements under the CustomModel child group rather than a separate State group.
5. Confirm duplicate StateInfo names are suffixed using dash notation.
6. Confirm FaceInfo output remains unchanged.
7. With the temporary constant enabled, confirm the old `States` group elements are still created as before.

### CustomModelCompressed

1. Import a model containing only `CustomModel`.
2. Import an equivalent model containing `CustomModelCompressed`.
3. Confirm both imports create the same prop element structure and node mappings.
4. Import a model containing both attributes and confirm compressed data is preferred.
5. Import a model with invalid compressed data and valid `CustomModel` data and confirm import falls back to `CustomModel`.
6. Import a model with no valid compressed or uncompressed model data and confirm import aborts with a user-facing error and
   logged error.

## Out Of Scope

- Do not implement the State effect.
- Do not add State effect discovery, rendering, Mark Collection handling, or Effect Editor controls.
- Do not add `State` to `MarkCollectionType`.
- Do not implement State effect playback modes such as `Countdown`, `Time Countdown`, or `Number`.
- Do not add user-selectable State effect overlap strategies.
- Do not persist preview toggle, preview mode, selected preview group, or active preview states.
- Do not expose the old xLights `States` group creation constant in UI or configuration.
- Do not implement legacy migration for previously saved development-only State property data.

## Implementation Planning Guidelines

- Use `.agents\PLANS.md` when creating the implementation plan.
- Use the project skills `dotnet-best-practices`, `csharp-docs`, `dotnet-design-pattern-review`, and `catel-mvvm` during
  implementation planning and code changes.
- Use `csharp-async` if implementation touches async import or UI flows.
- Update XML documentation when adding or modifying public or protected C# APIs.
- Inspect copy-versus-clone call paths before changing stable ID behavior.
- Prefer Catel validation and dialog services for setup UI validation and modal prompts.
- Keep ViewModels free of direct WPF dialog APIs.
- Update Jira issue VIX-3591 after converting this specification into the ExecPlan, so Jira reflects the validated plan
  rather than an incomplete outline.

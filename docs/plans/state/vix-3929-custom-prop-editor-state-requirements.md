# VIX-3929 Custom Prop Editor State Requirements

## Goal

Implement State definition authoring in the Custom Prop Editor so users can create, edit, preview, save, reopen, and import
State definitions for a custom prop before that prop is imported into Vixen Preview.

The feature must align Custom Prop Editor state authoring with the State property model completed in
`docs/state/vix-3591-state-property-final-requirements.md` and consumed by the State effect in
`docs/state/vix-3924-state-effect.md`. The Custom Prop Editor must author State data once in the model element's
`ElementModel.StateDefinitionModels` collection and the Preview import must attach that data as a real Vixen State property on
the intended model element.

## Background

The Custom Prop Editor can import xLights `stateInfo` from xModels and can create an element hierarchy that represents those
states. That import path currently produces enough structure for later Preview import, but users cannot manage State
definitions or State items in the Custom Prop Editor in the same way they can manage a State property in Display Setup.

The Custom Prop Editor also exposes three element-level State fields in the Element Info tab:

- `State Name`
- `State Item`
- `State Item Color`

Those fields write to `ElementModel.StateDefinition`. They do not model multiple State definitions per prop/model element
and cannot represent the finalized State property shape where one property contains many State definitions and each
definition contains many State items. They should be replaced by a State Definition workflow that edits
`ElementModel.StateDefinitionModels` on the model element.

## References

- Jira: [VIX-3929](https://vixenlights.atlassian.net/browse/VIX-3929)
- State Property requirements: `docs/state/vix-3591-state-property-final-requirements.md`
- State Effect requirements: `docs/state/vix-3924-state-effect.md`
- State plans: `docs/plans/state/`
- Custom Prop Editor module: `src/Vixen.Modules/App/CustomPropEditor`
- Preview custom prop import: `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs`
- State property module: `src/Vixen.Modules/Property/State`

## Terminology

- **Custom prop**: A `Prop` saved by the Custom Prop Editor and later imported into Vixen Preview.
- **Element model**: An `ElementModel` inside the Custom Prop Editor tree. It represents a future Vixen `ElementNode`.
- **Root element**: The top-level `Prop.RootNode`.
- **Model element**: The `ElementModel` that represents the primary xLights model group. On xModel import this is the child
  group named with the model name plus ` - Model`. For hand-created props, the root element is treated as the model element
  unless the user designates another eligible element as `model`.
- **Submodel element**: An `ElementModel` that represents an xLights submodel or user-defined submodel grouping.
- **FaceInfo element**: An `ElementModel` that represents imported or user-designated face information.
- **StateInfo element**: An `ElementModel` that represents imported or user-designated legacy state grouping.
- **Model Type**: A Custom Prop Editor classification assigned to an `ElementModel`. Valid values are `None`, `Model`,
  `SubModel`, `FaceInfo`, and `StateInfo`.
- **Prop State data**: The Custom Prop Editor persisted State collection stored on the model element's
  `ElementModel.StateDefinitionModels` collection and mapped directly to the State property model.
- **State property**: The Vixen `State` property attached to a Preview-imported `IElementNode`.
- **State definition**: One named definition inside State data. It has a stable ID, name, description, ordered State items,
  and preview/editing behavior.
- **State item**: One row inside a State definition. It has a stable ID, name, color, and assigned Custom Prop Editor element
  model IDs.
- **Effective leaf element**: A leaf `ElementModel` included by a State item assignment. Assigning a group includes its
  descendant leaves.
- **Legacy element-level State data**: Existing `ElementModel.StateDefinition` data used by imported state grouping elements.

## User Workflows

### Import xModel With StateInfo

1. The user imports an xModel that contains one or more `stateInfo` elements.
2. The Custom Prop Editor creates the existing model hierarchy, including the top-level prop group, child model group,
   submodel groups, face groups, and legacy State groups while the legacy group option remains enabled.
3. The imported `stateInfo` values are mapped into `ElementModel.StateDefinitionModels` on the imported model element.
4. The imported State definitions are visible in the new State Definition tab and can be edited without using the legacy
   Element Info State fields.
5. Saving and reopening the prop preserves State definition IDs, State item IDs, colors, assignments, and order.
6. Importing the saved prop into Vixen Preview attaches one State property to the intended model `ElementNode`.

### Create Custom Prop Manually

1. The user creates a new prop without importing an xModel.
2. The user draws or creates the element tree.
3. The user optionally designates one eligible element as `Model`; otherwise the root element is treated as the model
   element for State authoring and Preview import.
4. The user opens the State Definition tab, adds one or more State definitions, adds State items, assigns element nodes, and
   previews them in the Custom Prop Editor canvas.
5. Saving and reopening the prop preserves the authored State data.
6. Importing the saved prop into Vixen Preview attaches one State property to the designated model element, or to the root
   element when no explicit model element exists.

### Edit Older Prop With Legacy StateInfo

1. The user opens an existing `.prp` file containing legacy element-level State data.
2. The Custom Prop Editor migrates any legacy State data into `ElementModel.StateDefinitionModels` in memory.
3. The migrated State definitions are shown in the State Definition tab.
4. Saving the prop persists the new `ElementModel.StateDefinitionModels` data.
5. The Preview import remains able to import older prop files directly even when they have not been opened and saved in the
   Custom Prop Editor.

## Scope

### In Scope

- Add State Definition authoring to the Custom Prop Editor left pane.
- Persist State data in the model element's `ElementModel.StateDefinitionModels` collection in Custom Prop Editor prop files.
- Map Custom Prop Editor State data to the existing Vixen State property on Preview import.
- Migrate older Custom Prop Editor legacy State data to `ElementModel.StateDefinitionModels`.
- Preserve Preview import support for older prop files that still contain only legacy State data.
- Add Model Type display and editing for Custom Prop Editor elements.
- Designate xModel-imported model, submodel, faceInfo, and stateInfo groups with Model Type values.
- Preview State item colors inside the Custom Prop Editor canvas without sending output to Vixen Live Preview or external
  preview modules.
- Remove or hide user-facing element-level State fields that no longer match the State property workflow.
- Add automated and manual test coverage expectations.

### Out Of Scope

- Changes to State effect rendering behavior.
- Changes to Display Setup State property authoring, except for reuse of UI/view-model components where practical.
- Changes to FaceInfo behavior beyond preserving existing import and identifying FaceInfo-related elements with Model Type.
- Removal of the temporary xModel legacy State group creation option.
- Replacing LiteDB persistence with JSON.
- Backward compatibility where older Vixen versions open newly saved prop files.

## Functional Requirements

### State Data Model

- The Custom Prop Editor must persist State definitions in `ElementModel.StateDefinitionModels` on the designated model element.
- `ElementModel.StateDefinitionModels` is the authoritative Custom Prop Editor State storage going forward.
- `ElementModel.StateDefinition` is legacy storage and must be deprecated for authoring.
- New authoring workflows must not write authoritative State data to `ElementModel.StateDefinition`.
- The Custom Prop Editor must persist State definitions as model-element-level data that maps directly to
  `VixenModules.Property.State.StateData`.
- The new Custom Prop Editor State data must support:
  - a stable State property/container ID;
  - one or more State definitions;
  - stable State definition IDs;
  - State definition name and description;
  - ordered State items;
  - stable State item IDs;
  - State item name;
  - State item color;
  - assigned `ElementModel.Id` values.
- A State item assignment may target a group element or a leaf element.
- Preview import must expand or preserve assignments using the same behavior as the State property setup: group assignments
  represent their effective descendant leaves when rendered or previewed.
- A prop with no State definitions must remain valid.
- If a prop has State data, each State definition must contain at least one State item before it can be saved through the
  State Definition editor.
- Stable IDs must be preserved across normal edits, save/reopen, import into Preview, and logical clones.
- Copying a State definition must create a new State definition ID and new State item IDs.
- Renaming or editing a State definition must preserve its State definition ID.
- Editing State item name, color, assignment, or order must preserve the State item ID.
- The Custom Prop Editor data model should reuse State property data and logic helpers where project references and layering
  allow it. If a Custom Prop Editor-specific DTO is required, it must be losslessly mappable to `StateData`,
  `StateDefinitionData`, and `StateItemData`.
- New public or protected C# APIs introduced for State data must include XML documentation following
  `.agents/skills/csharp-docs/SKILL.md`.

### Model Type Data

- Each `ElementModel` must be able to store a `Model Type`.
- Code should use proper .NET naming for Model Type values. Display/import mapping may preserve xLights casing where needed,
  but persisted and code-facing names should not mimic incoming file casing.
- Valid code-facing Model Type values are:
  - `None`
  - `Model`
  - `SubModel`
  - `FaceInfo`
  - `StateInfo`
- Existing props with no Model Type data must load with all elements treated as `None`.
- Imported xModels must assign Model Type values as follows:
  - the imported child model group is `Model`;
  - imported submodel groups are `SubModel`;
  - imported faceInfo child groups that represent specific face definitions are `FaceInfo`;
  - imported stateInfo child groups that represent specific legacy state definitions are `StateInfo`.
- A hand-authored prop does not require any explicit Model Type value.
- If no element has Model Type `Model`, State authoring and Preview import must treat the root element as the model element.

### Model Type Rules

- At most one element in a prop may be Model Type `Model`.
- An element with Model Type `Model` must not be a leaf unless it is the only element in the prop.
- Zero or more elements may be Model Type `SubModel`.
- Zero or more elements may be Model Type `FaceInfo`.
- Zero or more elements may be Model Type `StateInfo`.
- State definitions and Face definitions may exist without corresponding `StateInfo` or `FaceInfo` Model Type elements.
- Setting an element to Model Type `Model` must clear any previous explicit `Model` designation.
- The user must be able to clear an element's Model Type back to `None`.
- The UI must prevent or warn before applying an invalid Model Type assignment.
- Deleting an element with an explicit Model Type must remove that designation with the element.
- If the explicit `Model` element is deleted, the root element becomes the implicit model element until the user designates a
  new model element.

### Valid Imported Tree Shape

An xModel import may create the following tree shape and Model Type assignments:

```text
Santa Waving {1}                   -> Root element, implicit top-level prop group
|-- Santa Waving {1} - Model       -> Model Type: Model, imported from main model definition
|-- Santa Waving {1} - PomPom      -> Model Type: SubModel, imported from subModel
|-- Santa Waving {1} - Belt        -> Model Type: SubModel, imported from subModel
|-- Santa Waving {1} - Faces       -> None, container for faceInfo definitions
|   `-- Santa Waving {1} - Santa   -> Model Type: FaceInfo, imported from faceInfo
`-- Santa Waving {1} - States      -> None, container for legacy stateInfo groups
    |-- Santa Waving {1} - Static  -> Model Type: StateInfo, imported from stateInfo
    `-- Santa Waving {1} - Wave    -> Model Type: StateInfo, imported from stateInfo
```

The exact generated names must follow the current Custom Prop Editor xModel import naming behavior unless an implementation
plan explicitly changes naming and updates existing tests.

### State Definition Tab

- Add a `State Definition` tab in the same primary editor tab control as `Layout` and `Order`.
- `Layout`, `Order`, and `State Definition` are mutually exclusive editing modes.
- The lower-left tab control remains focused on metadata tabs such as `Prop Info` and `Element Info`.
- The tab header must be `State Definition`.
- The State Definition tab must edit `ElementModel.StateDefinitionModels` for the current model element.
- The tab should follow the State property setup workflow in Display Setup wherever practical:
  - State definition selector;
  - Add State definition;
  - Delete State definition;
  - Rename State definition;
  - Copy State definition;
  - State definition description;
  - State item grid;
  - State item Add and Remove;
  - State item color editing;
  - State item assignment through the Custom Prop Editor drawing surface;
  - State item move up and move down;
  - Preview controls.
- The State Definition tab must work whether the current prop was imported from xModel or created manually.
- The implementation should reuse State property data and logic helpers to keep validation, ID behavior, item ordering,
  assignment semantics, and preview selection semantics consistent.
- The UI and view models may be Custom Prop Editor-specific if direct reuse of the State property setup UI is too complex,
  but the visual layout and interaction patterns should remain as consistent with State property setup as practical.
- The State Definition tab must not require a selected element in the element tree to show the current model element's State
  definitions.
- State item assignment checkbox trees are not shown in the Custom Prop Editor because assignment is performed through the
  drawing surface.
- If an explicit Model Type `Model` element exists, assignment and preview are scoped to that element.
- If no explicit model element exists, assignment and preview are scoped to the prop root element.
- The tab must show a clear empty state when no State definitions exist and must provide an Add action.
- The tab must preserve unsaved edits in the editor until the user saves, closes, cancels through an existing dirty prompt,
  or reloads another prop.

### State Definition Management

- Users can add, delete, rename, and copy State definitions.
- Add creates a State definition at the end of the collection and selects it.
- Add suggests `State - N`, incrementing until the name is unique.
- Delete confirms before removal.
- Delete cannot remove the final State definition if the State data container exists. The user may instead remove all State
  data through an explicit remove/clear action.
- Rename preserves the selected State definition ID.
- Copy prompts for a unique name, appends the copied definition, selects it, preserves visible values, and generates new
  nested IDs.
- State definitions display in fixed creation order.
- Switching to another State definition is blocked while the current definition has blocking validation errors.

### State Definition Validation

- State definition names are required.
- Leading and trailing whitespace is trimmed before a name is accepted.
- Whitespace-only names are invalid.
- Exact duplicate State definition names are blocked.
- State definition name uniqueness is case-sensitive.
- Case-only differences, such as `Open` and `open`, are allowed but should produce a non-blocking warning.
- State definition names shorter than three characters should produce a non-blocking warning.
- A prop with no State data is valid, but once State data exists, every State definition and State item must satisfy
  blocking validation before the prop can be saved.
- If State data has blocking validation errors, the prop save must be blocked.
- Save blocking must show a clear warning that identifies why the save cannot continue and guides the user to the invalid
  State Definition fields.

### State Item Editing

- The selected State definition shows its ordered State items in a grid.
- Users can add and remove State item rows.
- New State item rows use the State property default item behavior.
- State item names are required.
- State item names are trimmed when editing completes.
- Whitespace-only State item names are invalid.
- Exact duplicate State item names are allowed.
- State item names are case-sensitive.
- Users can edit State item names inline in the grid.
- Users can edit State item colors through the standard Vixen color picker behavior.
- Double-clicking a color cell opens the standard Vixen color picker behavior.
- Color cells display the selected color as the cell background and a readable hex value.
- Color cell foregrounds must adapt to the selected background color, using the shared
  `BackgroundColorToTextBrushConverter` behavior.
- The Assigned column shows the effective assigned leaf count.
- The State item grid supports multi-row selection for preview.
- Sorting the State item grid by a column header and saving persists the displayed row order for that State definition.
- Manual row reorder through Move Up and Move Down must be available and must preserve IDs, names, colors, and assignments.
- Removing a State item must remove only that item and must not affect other same-named items.

### State Item Assignment

- The Custom Prop Editor does not show State item assignment checkbox trees.
- Selecting exactly one State item row enables canvas assignment editing for that row.
- Clicking an unassigned node in State Definition mode assigns that node to the selected State item.
- Clicking an already assigned node in State Definition mode removes that assignment from the selected State item.
- Drag-box selection in State Definition mode assigns all contained nodes to the selected State item.
- Holding Control while drag-box selecting in State Definition mode removes the contained nodes from the selected State item.
- Canvas assignment editing is disabled when no State item row is selected.
- Canvas assignment editing is disabled when more than one State item row is selected.
- Selection handles and normal layout-selection affordances are disabled while State Definition mode is active.
- Switching State definitions clears selected State item row selection so the user explicitly chooses the row to inspect.
- Assignments must persist as stable `ElementModel.Id` values.
- Deleting an element from the Layout tab must remove that element's `ElementModel.Id` from all State item assignments.
- Refreshing or rebinding State assignments must prune element IDs that no longer exist, preventing dangling assignment
  references before save or preview import.

### Custom Prop Editor Preview

- The State Definition tab must provide a local preview using the existing Custom Prop Editor drawing surface.
- The preview must not publish to Vixen Live Preview and must not activate external preview output.
- Preview state is temporary editor state and is not persisted.
- The State Definition preview is active whenever the State Definition tab is the active primary editor tab.
- The State Definition preview must not expose a Preview on/off toggle.
- When the State Definition tab is not active, the drawing surface must return to the existing Custom Prop Editor behavior,
  including normal colors and selection indicators.
- While the State Definition tab is active, the canvas must show the full prop as a low-emphasis outline using RGB
  25,25,25 for non-active nodes.
- Active State item nodes must overlay their configured State item colors.
- Selecting one State item row previews only that row's assigned nodes.
- Selecting multiple State item rows previews all selected rows in their configured colors.
- When no State item row is selected, the preview renders no active State item color.
- The same element assigned by multiple active State items with different colors must preview a mixed color.
- Mixed preview color calculation must be deterministic and documented in the implementation plan.
- Canvas assignment editing is available only when exactly one State item row is selected.
- When multiple State item rows are selected, the State Definition preview is view-only and drawing-surface selection must not
  change assignments.
- Leaving the State Definition tab, closing the editor, opening another prop, changing model element designation, or removing
  the model element must clear temporary preview state and restore normal viewer behavior.

### Element Info Changes

- Remove or hide the existing user-facing Element Info fields that write legacy State data:
  - `State Name`
  - `State Item`
  - `State Item Color`
- Add a user-facing `Model Type` field to the Element Info tab.
- The `Model Type` field must show the selected element's current Model Type.
- The `Model Type` field must allow setting and clearing Model Type values according to the Model Type rules.
- The `Model Type` field must be disabled or show a mixed-selection state if the Element Info tab is later updated to support
  multi-selection.
- Existing Face fields in Element Info must continue to work as they do today.

### xModel Import

- New xModel imports must map `stateInfo` into `ElementModel.StateDefinitionModels` on the imported model element.
- Each imported `stateInfo` becomes one State definition.
- Each imported state item becomes one State item in its State definition.
- Imported State definition names must use the same normalization rules from VIX-3591:
  - blank names become `State - N`;
  - exact duplicates are suffixed as `Name - 2`, `Name - 3`, and so on;
  - case-only names remain distinct.
- Imported State item names must use existing xModel State item normalization.
- Imported State item colors must preserve xModel colors where present and use the existing default color behavior where
  absent.
- Imported State item assignments must target leaf elements under the imported model group.
- New xModel import should continue to create legacy State element groups while `CreateLegacyStateGroups` remains enabled.
- Legacy State group creation is a compatibility option expected to remain enabled for a production validation period, but it
  must be isolated so it can be disabled later without changing the authoritative State authoring model.
- Legacy State element groups must not receive element-level State data used as the authoritative source for new imports.
- Existing FaceInfo, SubModel, `CustomModel`, and `CustomModelCompressed` behavior must remain unchanged.

### Preview Import

- Importing a Custom Prop into Vixen Preview must attach a State property to the intended model `ElementNode` when the prop
  has State data.
- If an explicit Model Type `Model` element exists, attach the State property to that imported `ElementNode`.
- If no explicit model element exists, attach the State property to the imported root `ElementNode`.
- If the xModel-imported model element exists, attach the State property to that element as current imports do today.
- State definitions and State items imported into Preview must preserve stable IDs where possible.
- State item assignments must map from `ElementModel.Id` values to the corresponding created `ElementNode.Id` values.
- Assignments whose `ElementModel.Id` cannot be mapped must be skipped.
- Empty State definitions after assignment mapping must be skipped or treated as invalid according to the implementation
  plan, but must not create broken State properties.
- The Preview import must continue to support older prop files that contain only legacy element-level State data.
- When both new `ElementModel.StateDefinitionModels` data and legacy element-level State data exist, the new StateDefinitionModels data
  is authoritative and legacy data is ignored for State property creation.
- The Preview import must not modify the prop file on disk while migrating or interpreting older State data.

### Migration

- Opening an older Custom Prop Editor file must migrate legacy element-level State data into `ElementModel.StateDefinitionModels`
  in memory.
- Migration must write migrated data to `ElementModel.StateDefinitionModels`.
- Migration must not preserve or refresh redundant compatibility data in `ElementModel.StateDefinition`.
- Migration must group legacy State items by `StateDefinitionName`.
- Each group becomes one State definition.
- Each legacy element-level State item becomes one State item.
- Legacy item assignment must map to the legacy element itself unless the old structure clearly identifies a more precise
  assigned element set.
- Migration must preserve color and item names.
- Migrated data must receive non-empty stable IDs.
- Migration must not create duplicate State definitions if equivalent `ElementModel.StateDefinitionModels` data already exists.
- Migration must be idempotent across repeated load/save cycles.
- Older props with no legacy State data must load unchanged.
- No existing user-created prop is expected to have meaningful legacy State data, but migration must be defensive for files
  created from xModel imports.

### Persistence

- Saving a prop must persist Model Type data and `ElementModel.StateDefinitionModels` data.
- Saving a prop must not persist temporary State preview state.
- Save As must preserve the same logical State IDs unless the whole prop copy workflow explicitly defines new IDs.
- The design should avoid redundant persisted State data.
- Newly saved props should prefer only `ElementModel.StateDefinitionModels` for State authoring data.
- Older Vixen versions are not required to open newly saved props with State authoring data.
- The structure should be friendly to a future migration from LiteDB serialization to JSON.

### Dirty State And Undo Expectations

- Editing State definitions, State items, assignments, colors, descriptions, Model Type values, or migrated State data must
  mark the prop dirty.
- Closing the editor with unsaved State changes must use the existing save prompt.
- Canceling a close operation must keep unsaved State edits in memory.
- Existing undo/redo behavior is not defined for the Custom Prop Editor; this feature must not claim undo/redo support unless
  an implementation plan adds it explicitly.

## Non-Functional Requirements

- Follow existing Custom Prop Editor MVVM and Catel patterns.
- View models must not directly use WPF visual types for business logic; UI interactions should go through services where
  practical.
- Keep behavior testable through model/view-model/importer helpers rather than UI-only logic.
- Use async naming and `Task`-returning methods for asynchronous work.
- Do not introduce `async void` except for event handlers or existing command patterns that require it.
- Use structured NLog logging for import and migration warnings/errors.
- Keep changes localized to Custom Prop Editor, Preview import, State property reuse helpers, and tests unless a broader
  design change is documented in the implementation plan.
- Update XML documentation when adding or modifying public or protected C# APIs.

## Acceptance Criteria

### Data And Persistence

- A manually created prop can save and reopen with one or more State definitions.
- State definition IDs and State item IDs survive save/reopen.
- State definition copy creates distinct IDs.
- State definition rename preserves the ID.
- State item edit preserves the ID.
- A prop with no State data remains valid and saves normally.
- A prop with invalid State data is not saved and the user sees a warning explaining the validation issue.
- Temporary preview state is not persisted.

### Model Type

- xModel import marks the imported model group as `Model`.
- xModel import marks imported submodel groups as `SubModel`.
- xModel import marks imported faceInfo groups as `FaceInfo`.
- xModel import marks imported legacy stateInfo groups as `StateInfo`.
- The Element Info tab shows and edits Model Type.
- Only one explicit `Model` designation can exist.
- Invalid Model Type assignments are blocked or clearly rejected.
- Deleting the explicit model element falls back to the root element as the implicit model element.

### State Definition Editor

- The primary editor tab control contains `Layout`, `Order`, and `State Definition` as mutually exclusive modes.
- The lower-left tab control continues to contain metadata tabs such as `Prop Info` and `Element Info`.
- The State Definition tab can add, delete, rename, and copy State definitions.
- The tab can add, remove, edit, sort, and manually reorder State items.
- State item names, colors, assignments, and counts display correctly.
- Color cells use the State item color as their background, use a readable foreground, and open the color picker on
  double-click.
- The Assigned column shows the assigned node count.
- State item assignment is performed through the drawing surface, not through assignment checkbox trees.
- Deleting elements from the Layout tab removes their IDs from State item assignments.
- Blocking validation prevents saving invalid State data.

### Preview

- The State Definition preview is active whenever the State Definition tab is active.
- Leaving the State Definition tab restores the normal Custom Prop Editor drawing surface behavior.
- While the State Definition tab is active, the drawing surface shows the full prop in RGB 25,25,25 and active State items in
  their configured colors.
- Overlapping active State items on the same element preview as a mixed color.
- Selecting one State item row previews only the selected item and enables canvas assignment editing.
- Selecting multiple State item rows previews all selected rows in their configured colors and disables canvas assignment
  editing.
- Assignment and color edits refresh the active local preview.
- Clicking assigned nodes toggles them off for the selected State item.
- Drag-box selection assigns contained nodes for the selected State item.
- Holding Control while drag-box selecting removes contained nodes from the selected State item.
- Drawing-surface selection is view-only when no State item row or multiple State item rows are selected.
- Selection handles are disabled in State Definition mode.
- Preview never publishes Vixen Live Preview messages.

### xModel Import

- Importing an xModel with `stateInfo` creates `ElementModel.StateDefinitionModels` visible in the State Definition tab.
- Imported State definitions use normalized names and preserve case-sensitive distinct names.
- Imported duplicate State definition names are suffixed.
- Imported State item assignments target imported model leaves.
- Existing legacy State groups are still created while the legacy option is enabled.
- Existing FaceInfo, SubModel, and CustomModel import behavior is unchanged.

### Preview Import

- Importing a prop with new State data into Preview attaches one State property to the explicit model element, imported model
  element, or root fallback.
- The attached State property contains the authored State definitions and State items.
- Element assignments map to created `ElementNode.Id` values.
- Importing an older prop with only legacy State data still creates the expected State property.
- If both new and legacy State data exist, the new data is used.

### Migration

- Opening an older prop with legacy element-level State data migrates that data into the State Definition tab.
- Migration is idempotent.
- Saving the migrated prop persists the new State data.
- Preview import can interpret older prop files without saving them first.
- Newly saved migrated props use `ElementModel.StateDefinitionModels` as their only authoritative State data.

## Automated Test Plan

Run focused Custom Prop Editor, State property, and Preview import tests during implementation:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor"
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~VixenPreview"
```

Run final validation:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
git diff --check
```

Automated coverage must include:

- Custom Prop Editor State data default, normalize, clone/copy, and save/reopen behavior through
  `ElementModel.StateDefinitionModels`.
- Model Type persistence and rule enforcement.
- xModel import Model Type assignment.
- xModel import StateInfo to `ElementModel.StateDefinitionModels`.
- xModel duplicate, blank, and case-only State definition names.
- State item color, name, order, and assignment import.
- Migration from legacy `ElementModel.StateDefinition` data.
- Migration idempotence.
- Preview import from new `ElementModel.StateDefinitionModels` data.
- Preview import from older legacy State data.
- New-data-over-legacy precedence.
- Assignment mapping from `ElementModel.Id` to `ElementNode.Id`.
- State Definition tab view-model commands and validation.
- Canvas assignment toggle, drag-box assignment, Control drag-box removal, and disabled editing for zero or multiple selected
  State item rows.
- Local preview tab activation, color overlay state, multi-row preview, assignment selection, and reset behavior.
- Mixed-color local preview for overlapping active State items.
- No Live Preview broadcast messages from Custom Prop Editor local preview.

Tests should use embedded minimal prop/xModel data. They should not depend on external `.xmodel` or `.prp` files.

## Manual Test Plan

### Manual Prop Authoring

1. Open Custom Prop Editor.
2. Create a new prop.
3. Add several light nodes and at least one group.
4. Open the State Definition tab.
5. Add a State definition and two State items.
6. Assign State items to visible nodes through the drawing surface.
7. Save, close, reopen, and confirm names, colors, assignments, counts, and row order persisted.
8. Import the prop into Preview and confirm the State property is attached to the root fallback or explicit model element.

### Model Type

1. Select a non-leaf element and set Model Type to `Model`.
2. Select another non-leaf element and set Model Type to `Model`.
3. Confirm the first element is no longer `Model`.
4. Attempt to set a leaf element to `Model` when it is not the only element.
5. Confirm the UI blocks or rejects the assignment.
6. Set several groups to `SubModel`, `FaceInfo`, and `StateInfo`, save, reopen, and confirm values persisted.

### xModel Import

1. Import an xModel containing model nodes, submodels, faceInfo, and stateInfo.
2. Confirm the imported model group has Model Type `Model`.
3. Confirm submodel, faceInfo, and legacy stateInfo groups have the expected Model Type.
4. Open the State Definition tab and confirm each imported `stateInfo` appears as a State definition.
5. Confirm imported State item colors and assignments match the xModel.
6. Edit one imported State definition, save, reopen, and confirm the edit persisted.

### Local Preview

1. Open a prop with at least two State items assigned to visible nodes.
2. Confirm the drawing surface appears normal while the Layout or Order tab is active.
3. Activate the State Definition tab and confirm the drawing surface switches to State Definition preview.
4. Select each State item row and confirm only that row's assigned nodes show its configured color.
5. Select an unassigned visible element in the drawing surface while one State item row is selected and confirm that element
   is assigned to that State item.
6. Select an assigned visible element again and confirm that element is removed from the State item.
7. Drag-box select visible elements and confirm the contained elements are assigned to the selected State item.
8. Hold Control while drag-box selecting assigned visible elements and confirm the contained elements are removed from the
   selected State item.
9. Select multiple State item rows and confirm all selected rows preview in their configured colors.
10. With multiple State item rows selected, select visible elements in the drawing surface and confirm assignments do not
   change.
11. Clear the selected State item row, select visible elements in the drawing surface, and
   confirm assignments do not change.
12. Change an active State item color and assignment and confirm the preview refreshes.
13. Activate the Layout or Order tab and confirm the normal drawing surface behavior returns.

### Legacy Migration

1. Open an older prop containing legacy element-level State data.
2. Confirm migrated State definitions appear in the State Definition tab.
3. Save and reopen the prop.
4. Confirm duplicate migrated State definitions were not created.
5. Import the original older prop directly into Preview and confirm a State property is still created.

## Jira Update Block

Paste the following markdown into Jira issue `VIX-3929` before implementation:

---

### Goal

Add State definition authoring to the Custom Prop Editor so users can create, edit, preview, save, reopen, and import custom
props with State data. The authored data will map to the existing Vixen State property used by Display Setup and the State
effect.

### Requirements Summary

- Add a `State Definition` tab next to `Layout` and `Order`.
- Persist State data in `ElementModel.StateDefinitionModels`, compatible with `StateData`, `StateDefinitionData`, and
  `StateItemData`.
- Remove or hide the legacy Element Info fields `State Name`, `State Item`, and `State Item Color`.
- Add a `Model Type` field for `None`, `Model`, `SubModel`, `FaceInfo`, and `StateInfo`.
- Enforce one explicit `Model` element at most; use the root element as the implicit model when no explicit model exists.
- Map xModel `stateInfo` into `ElementModel.StateDefinitionModels`.
- Continue creating legacy State groups during xModel import while the current compatibility option remains enabled.
- Migrate older prop files with legacy element-level State data into `ElementModel.StateDefinitionModels`.
- Import props into Preview by attaching one State property to the explicit model element, imported model element, or root
  fallback.
- Provide a local Custom Prop Editor preview that shows the full prop in RGB 25,25,25, overlays selected State item colors,
  mixes overlapping colors deterministically, supports canvas assignment editing for one selected row, and does not publish
  Live Preview messages.

### High-Level Design

Use the State property model as the contract for Custom Prop Editor State authoring. Store State definitions on the model
element's `ElementModel.StateDefinitionModels` collection, assign State items to `ElementModel.Id` values, and map those IDs to
`ElementNode.Id` values during Preview import. Keep legacy element-level State data readable for migration and direct Preview
import, but make `ElementModel.StateDefinitionModels` authoritative when both exist.

### Acceptance Criteria

- Users can author State definitions in the Custom Prop Editor for imported and manually created props.
- Authored State definitions save, reopen, and import into Preview as a real State property.
- xModel imports show imported `stateInfo` in the State Definition tab.
- Older props with legacy State data migrate in the editor and still import directly into Preview.
- Model Type values display, persist, and follow the defined rules.
- Local preview updates inside the Custom Prop Editor canvas, mixes overlapping colors, and does not activate external
  preview output.

### Testing

- Add automated tests for State data persistence through `ElementModel.StateDefinitionModels`, Model Type rules, xModel import
  mapping, legacy migration, Preview import mapping, State Definition editor view-model behavior, and local preview behavior.
- Run focused Custom Prop Editor, State property, and Preview import tests.
- Run the full test suite, Debug and Release solution builds, and `git diff --check` before final acceptance.

### Risks

- The Custom Prop Editor currently has both legacy element-level State fields and newer imported State collections. The
  implementation must deprecate `ElementModel.StateDefinition` and avoid writing duplicate authoritative State data.
- LiteDB persistence may require defensive migration code for missing properties and old serialized shapes.
- Preview import must preserve current FaceInfo, SubModel, color setup, and element naming behavior while adding State
  property attachment.
- The drawing surface may need a small preview coordinator to avoid interfering with normal selection colors.

---

## Implementation Planning Notes

- Create an ExecPlan in `docs/plans/state/` before implementation, following `.agents/PLANS.md`.
- The first implementation milestone must update Jira VIX-3929 with the Jira block above, expanded with the final plan,
  file targets, risks, and testing steps.
- Use the project skills `dotnet-best-practices`, `csharp-async`, `csharp-docs`, `dotnet-design-pattern-review`, and
  `catel-mvvm` while creating the implementation plan and code changes.
- Ask for clarification before implementation if the local preview cannot represent overlapping colors or if preserving old
  prop compatibility would require duplicating State data.

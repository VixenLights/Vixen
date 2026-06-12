# Implement VIX-3929 Custom Prop Editor State Authoring

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Any contributor implementing this plan must keep this document self-contained, update it at every stopping point, and record new decisions or discoveries as they occur.

## Purpose / Big Picture

After this work, a Vixen user can open the Custom Prop Editor, import an xLights custom model or create a prop manually, author named State definitions directly in that editor, preview those states on the prop drawing surface, save the prop, reopen it, and import it into Vixen Preview as a real State property. This removes the current gap where xModel `stateInfo` can be imported into a prop-like structure but cannot be edited in the Custom Prop Editor in the same workflow as the Display Setup State property.

The visible result is a third lower-left tab named `State Definition` next to `Prop Info` and `Element Info`. When that tab is active, the right-side prop viewer switches into State Definition preview: the full prop is shown in light gray, active State item assignments are overlaid in their colors, overlapping active colors are mixed deterministically, and selecting nodes in the viewer while a State item row is selected checks the corresponding assignment boxes for that item. Leaving the tab returns the viewer to its normal Custom Prop Editor behavior. Saving a prop persists the new state data in `ElementModel.StateDefinitionModels`, and importing the prop into Preview attaches the Vixen State property to the designated model element or to the root fallback.

## Progress

- [x] (2026-06-11 00:00 -05:00) Read `.agents/PLANS.md`, the VIX-3929 requirements document, project-specific `dotnet-best-practices`, `csharp-async`, `csharp-docs`, `dotnet-design-pattern-review`, and `catel-mvvm` skills.
- [x] (2026-06-11 00:00 -05:00) Inspected current Custom Prop Editor model, xModel import, Preview import builder, State property data model, Custom Prop Editor project references, Preview project references, and existing xModel StateInfo tests.
- [x] (2026-06-11 00:00 -05:00) Created this implementation plan with milestones, design decisions, file targets, Jira paste text, validation commands, and acceptance criteria.
- [x] (2026-06-11 00:00 -05:00) Updated Jira issue VIX-3929 with the plan summary, high-level design, risks, acceptance criteria, and testing steps.
- [x] (2026-06-11 00:00 -05:00) Added the Custom Prop Editor state data model foundation: `ElementModelType`, `StatePropertyId`, authoritative `StateDefinitionModels`, State definition/item models, mapper, model resolver, migration service, direct State property project reference, and focused tests.
- [ ] Update xModel import so new StateInfo imports fill `ElementModel.StateDefinitionModels`, assign Model Type values, and keep legacy State groups as a compatibility option.
- [ ] Update Preview import so `ElementModel.StateDefinitionModels` is authoritative and legacy `ElementModel.StateDefinition` remains a direct-import fallback.
- [ ] Add the Custom Prop Editor State Definition view models, commands, validation, and save blocking.
- [ ] Add the State Definition tab UI and viewer preview integration.
- [ ] Add focused automated tests for model data, import, migration, Preview import, editor view models, and local preview behavior.
- [ ] Run focused tests, full test suite, Debug and Release solution builds, and `git diff --check`; record validation evidence.

## Surprises & Discoveries

- Observation: `ElementModel.StateDefinitions` already exists, but it currently stores `ObservableCollection<StateDefinition>` where `StateDefinition` represents one imported State item row with `StateDefinitionName`, `Name`, `DefaultColor`, `Index`, and `ElementModelIds`.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Model/ElementModel.cs` initializes `StateDefinitions = new ObservableCollection<StateDefinition>()`, and `src/Vixen.Modules/App/CustomPropEditor/Model/StateDefinition.cs` has item-like fields rather than a definition containing ordered items.

- Observation: `ElementModel.StateDefinition` is the legacy element-level State field exposed by the Element Info tab and used by the older Preview import path.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementModelViewModel.cs` exposes `StateDefinitionName`, `StateItemName`, and `StateItemColor` fields backed by `ElementModel.StateDefinition`.

- Observation: Preview import already has both a newer `AddImportedStateDefinitions` path and an older child-element `StateDefinition` fallback path.
  Evidence: `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs` first calls `AddImportedStateDefinitions(model)` from `AddStateProperties`, then scans child models with `child.StateDefinition != null`.

- Observation: The Custom Prop Editor project does not currently reference the State property project, but Preview does.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/CustomPropEditor.csproj` lacks a reference to `src/Vixen.Modules/Property/State/State.csproj`; `src/Vixen.Modules/Preview/VixenPreview/VixenPreview.csproj` already references both CustomPropEditor and State.

- Observation: Existing xModel import tests assert the old item-like shape of `ElementModel.StateDefinitions`.
  Evidence: `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelImportHierarchyTests.cs` expects a single `modelGroup.StateDefinitions` item with `StateDefinitionName == "Wave"` and `Name == "Hand"`.

- Observation: The Custom Prop Editor project can reference the State property project without creating a circular reference.
  Evidence: `dotnet build src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore` succeeded after adding the project reference.

## Decision Log

- Decision: Use `ElementModel.StateDefinitionModels` on the designated model element as the only authoritative Custom Prop Editor State authoring storage, preserving old item-like `ElementModel.StateDefinitions` only as migration input.
  Rationale: The user explicitly chose this storage location and wants `ElementModel.StateDefinition` deprecated so the Custom Prop Editor mirrors the State property behavior instead of maintaining duplicate legacy data.
  Date/Author: 2026-06-11 / Codex

- Decision: Keep `ElementModel.StateDefinition` readable as legacy data for migration and direct Preview import, but do not write it from new authoring workflows.
  Rationale: Existing prop files and current xModel compatibility structures may contain legacy values. Reading them protects existing users while avoiding duplicate authoritative storage in newly saved props.
  Date/Author: 2026-06-11 / Codex

- Decision: Represent the user-facing `Model Type` with a properly named .NET enum, likely `ElementModelType`, with values `None`, `Model`, `SubModel`, `FaceInfo`, and `StateInfo`.
  Rationale: The incoming xLights files use lower/camel casing such as `stateInfo`; code should use normal .NET naming and map display/import text separately.
  Date/Author: 2026-06-11 / Codex

- Decision: Build Custom Prop Editor-specific State Definition UI and view models, but reuse State property data and logic helpers where practical.
  Rationale: The Custom Prop Editor assignment root, local drawing-surface preview, and tab-activation behavior differ from Display Setup State property setup. Reusing logic keeps validation and identity behavior consistent without forcing a mismatched UI.
  Date/Author: 2026-06-11 / Codex

- Decision: The State Definition preview is controlled by active tab state, not by a preview toggle.
  Rationale: The user clarified that the preview should be active whenever the State Definition tab is active, and that leaving the tab should restore normal viewer behavior.
  Date/Author: 2026-06-11 / Codex

- Decision: Mixed preview colors must be deterministic, with the exact formula selected and documented during implementation.
  Rationale: The user requires color mixing for overlapping active State items. The plan leaves room to choose an implementation-compatible formula, but it must be stable and tested.
  Date/Author: 2026-06-11 / Codex

- Decision: Preserve the old item-like `ElementModel.StateDefinitions` property as legacy migration input and introduce `ElementModel.StateDefinitionModels` as the new authoritative State authoring collection.
  Rationale: Older `.prp` files may already contain a serialized field named `StateDefinitions` with the old `ObservableCollection<StateDefinition>` shape. Keeping that property avoids risking LiteDB deserialization while still preventing new authoritative writes to the legacy shape.
  Date/Author: 2026-06-11 / Codex

## Outcomes & Retrospective

This plan is ready for implementation. It resolves the main design questions from the requirements phase: authoritative storage is `ElementModel.StateDefinitionModels`; legacy `ElementModel.StateDefinition` and the old item-like `ElementModel.StateDefinitions` are read-only migration/fallback input; Model Type uses proper .NET naming; invalid State data blocks save with warnings; and preview is tab-activated with viewer selection updating assignments only when a specific State item row is being edited.

No implementation has been performed yet. Update this section after each milestone with what changed, what passed, and what remains.

Milestone 1 is complete. Jira issue VIX-3929 has been updated with the implementation plan summary, design notes, acceptance criteria, risks, and testing steps.

Milestone 2 is complete. The Custom Prop Editor now has foundational State model types, a Model Type enum, a stable State property ID on `ElementModel`, a compatibility-safe authoritative collection named `StateDefinitionModels`, mapping helpers to the Vixen State property model, a model resolver, and a migration service for old imported-row and element-level State data. The Custom Prop Editor project now references the State property project directly. Focused Custom Prop Editor and State property tests pass.

## Context and Orientation

Vixen is a Windows .NET 10 WPF application. The Custom Prop Editor lives under `src/Vixen.Modules/App/CustomPropEditor`. It lets users create or import a prop definition, save it as a `.prp` file, and later import that prop into Vixen Preview. In this plan, a "prop" means `VixenModules.App.CustomPropEditor.Model.Prop`, and an "element model" means `VixenModules.App.CustomPropEditor.Model.ElementModel`, the Custom Prop Editor's serializable stand-in for a real Vixen `ElementNode`.

The Vixen State property lives under `src/Vixen.Modules/Property/State`. Its persistent model is `StateData`, which contains one or more `StateDefinitionData` objects, each containing ordered `StateItemData` objects. A State definition is a named set such as `Wave`; a State item is one row inside that definition, such as `Hand`, with a color and element assignments. The State effect implemented by VIX-3924 consumes this State property in sequences.

The current Custom Prop Editor has two state-related shapes. The legacy shape is `ElementModel.StateDefinition`, a single `StateDefinition` object exposed through Element Info fields named `State Name`, `State Item`, and `State Item Color`. The newer import shape is `ElementModel.StateDefinitions`, currently an `ObservableCollection<StateDefinition>` where each entry is really one imported State item row grouped later by `StateDefinitionName`. VIX-3929 now uses `ElementModel.StateDefinitionModels` as the authoritative State authoring collection that mirrors the State property model, and must deprecate new writes to `ElementModel.StateDefinition` and the legacy item-like `ElementModel.StateDefinitions`.

The xModel import code is `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`. It reads xLights `custommodel`, `subModel`, `faceInfo`, and `stateInfo` XML. It currently creates a top-level prop root such as `Santa Waving {1}`, a model group such as `Santa Waving {1} - Model`, optional submodel groups, optional face groups, and optional legacy State groups when `CreateLegacyStateGroups` is true. It already parses `stateInfo` into `CustomModel.StateInfos`, and then `AttachStateDefinitions` stores imported State items on the model group.

The Preview import code is `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs`. It creates real Vixen `ElementNode` objects from `ElementModel` objects, maps `ElementModel.Id` to `ElementNode`, and attaches properties such as Order, Face, Color, and State. This file must be updated so `ElementModel.StateDefinitionModels` is authoritative, and legacy `ElementModel.StateDefinition` is only a fallback for older files.

The main Custom Prop Editor view is `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`. Its lower-left tab control currently contains `Prop Info` and `Element Info`. Its right-side viewer is `controls:PropDesigner`, backed by `DrawingPanelViewModel`, `LightViewModel`, and current selection behavior. VIX-3929 adds a third lower-left tab named `State Definition` and changes viewer rendering/selection behavior only while that tab is active.

Follow the repository rules in `AGENTS.md`: read docs first, use tabs and CRLF for C# and XAML edits according to `src/.editorconfig`, do not reformat unrelated code, update XML documentation for public or protected C# APIs, and use project references rather than DLL references.

## Plan of Work

Start by updating Jira VIX-3929 with the plan summary in the Milestone 1 text below. If using the Jira skill, read `.agents/skills/jira/SKILL.md` first and follow it. Record the update outcome in `Progress` and `Artifacts and Notes`.

Milestone 2 establishes the data model and mapping foundation. Add a properly named model type enum, update `ElementModel` to store Model Type and a stable State property/container ID, and introduce an authoritative definition/item shape that can map directly to the State property model. The completed implementation uses `ElementModel.StateDefinitionModels` for that new shape and preserves the old item-like `ElementModel.StateDefinitions` collection as legacy migration input. Prefer adding two new model files, `src/Vixen.Modules/App/CustomPropEditor/Model/StateDefinitionModel.cs` and `src/Vixen.Modules/App/CustomPropEditor/Model/StateItemModel.cs`, rather than overloading the legacy `StateDefinition` class. Keep the existing `StateDefinition` class for `ElementModel.StateDefinition` legacy migration input and mark it obsolete if practical. Add helper methods that convert between Custom Prop Editor state models and `VixenModules.Property.State.StateData`, `StateDefinitionData`, and `StateItemData`. If direct reuse requires a project reference from `CustomPropEditor.csproj` to `src/Vixen.Modules/Property/State/State.csproj`, add that project reference and verify `Vixen.sln` platform mappings remain x64 for `Any CPU` entries.

There is one important serialization challenge in Milestone 2. The user-facing requirement is to use `ElementModel.StateDefinitions` going forward, but older prop files may already contain a serialized field with that name using the old item-like `ObservableCollection<StateDefinition>` shape. The first implementation attempt should preserve the desired public/domain name by renaming the old item-like collection to a legacy placeholder such as `LegacyStateDefinitions` or a private LiteDB-compatible backing property, then use `StateDefinitions` for the new definition/item structure. If LiteDB cannot reliably deserialize older files through that rename, keep the old serialized field as the legacy placeholder and introduce a new authoritative property with a clear name such as `StateDefinitionModels` or `StateData`. In that fallback, update this ExecPlan's Decision Log and all affected requirements references before continuing. The priority order is: load old props safely, keep new authoring data non-duplicated and authoritative, and use the `StateDefinitions` name for the new structure when serialization permits it.

The model foundation must include a way to find the current model element. Add a helper such as `PropStateModelResolver` under `src/Vixen.Modules/App/CustomPropEditor/Services` or a focused model helper namespace. It should return the single explicit `ElementModelType.Model` element when present, otherwise return `Prop.RootNode`. It should reject more than one explicit model if bad data is loaded by keeping the first in tree order and reporting validation/warnings, then the editor can repair by clearing all but one. It should also enforce that a `Model` element is not a leaf unless it is the only element in the prop.

Milestone 3 updates xModel import. In `XModelImport.AssembleModel`, mark the created model group with Model Type `Model`. In `AssembleSubModels`, mark imported submodel groups with Model Type `SubModel`. In `AssembleFaces`, mark imported faceInfo definition groups with Model Type `FaceInfo`, not the parent `Faces` container. In `AssembleStates`, mark imported legacy stateInfo definition groups with Model Type `StateInfo`, not the parent `States` container. In `AttachStateDefinitions`, populate the new `ElementModel.StateDefinitionModels` model shape on the model group. Keep `CreateLegacyStateGroups = true` and keep legacy State groups visible for the production validation period, but do not write authoritative `ElementModel.StateDefinition` values to those groups.

Milestone 4 updates migration and Preview import. Add a migration helper that can read old files with legacy `ElementModel.StateDefinition` values and old item-like `ElementModel.StateDefinitions` values if LiteDB can still deserialize them. The migration should group by legacy `StateDefinitionName`, create real State definitions and items with new stable IDs where missing, preserve item names, colors, item order, and assignments, and be idempotent. Use the helper when loading props in `PropEditorViewModel.LoadPropFromPath` or immediately after `PropModelServices.Instance().LoadProp(path)` returns. Update `PreviewCustomPropBuilder.AddImportedStateDefinitions` to read the new `StateDefinitionModels` shape and map each `ElementModel.Id` assignment to the created `ElementNode.Id`. Keep the existing child `StateDefinition` scan as a fallback only when the new `StateDefinitionModels` collection is empty.

Milestone 5 adds State Definition editor view models and validation. Create Custom Prop Editor-specific view models under `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State` or directly under `ViewModels` with clear names such as `StateDefinitionEditorViewModel`, `CustomPropStateDefinitionViewModel`, `CustomPropStateItemViewModel`, and `CustomPropStateAssignmentTreeNodeViewModel`. These should inherit from Catel `ViewModelBase` or the existing project base patterns, expose Catel properties and commands, and keep business logic out of code-behind. Implement Add, Delete, Rename, Copy, Add Item, Remove Item, Move Up, Move Down, color edit, assignment check/uncheck, and selected row behavior. Reuse or mirror the State property validation rules: definition names are required, trimmed, exact duplicates blocked, case-only duplicates warn, short names warn, item names required and trimmed, duplicate item names allowed, and group assignment clears/greys descendants. Invalid State data must block Save and show a warning that points the user to the State Definition tab.

Milestone 6 adds the UI. Update `CustomPropEditorWindow.xaml` to add a `State Definition` tab next to `Prop Info` and `Element Info`. Add a user control or inline layout for State Definition editing. Keep visual behavior as close as practical to `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml`, but do not force reuse if it makes the Custom Prop Editor viewer integration brittle. Update the Element Info view model to remove or hide `StateDefinitionName`, `StateItemName`, and `StateItemColor` from the property grid, and add a `Model Type` property that displays and edits `ElementModel.ModelType`. Setting `Model Type = Model` must clear any previous explicit model designation and must reject invalid leaf selections.

Milestone 7 integrates the local preview. Add a preview coordinator for the Custom Prop Editor drawing surface, likely under `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State` or `Controls`, that computes desired preview color per `LightViewModel` while the State Definition tab is active. It must not publish `Common.Broadcast` Live Preview messages. When the State Definition tab becomes active, the right-side viewer should show the full prop in light gray and overlay active State item colors according to the current preview mode. When the tab is not active, closing, opening another prop, changing model designation, or removing the model, restore normal viewer behavior and selection colors. For overlapping active State item colors on the same element, use a deterministic channel average unless implementation evidence shows a better existing color-mixing helper; document the final formula in the Decision Log. When a specific State item row is selected, selecting elements in the viewer must check those elements in that State item's assignment tree. If no item row is selected, viewer selection is view-only for State editing.

Milestone 8 adds tests. Existing tests in `src/Vixen.Tests/App/CustomPropEditor/Import/XLights` must be updated from the old item-like `StateDefinitions` shape to the new definition/item shape. Add tests for `ElementModelType` persistence and rules, model resolver behavior, xModel Model Type assignment, xModel StateInfo mapping, migration from legacy `ElementModel.StateDefinition`, migration idempotence, Preview import from new data, Preview import fallback from legacy data, new-over-legacy precedence, State editor command behavior and validation, save blocking, assignment tree group behavior, local preview tab activation/reset, mixed color behavior, and viewer selection updating assignments. Tests should use embedded minimal xModel/prop data and should not read external `.xmodel` or `.prp` files.

Milestone 9 performs full validation. Run focused tests during implementation and the full validation suite at the end. Record concise transcripts in `Artifacts and Notes`. The final acceptance is not just compilation: manually verify the Custom Prop Editor tab, local preview behavior, save/reopen, xModel import, and Preview import.

## Concrete Steps

Use `C:\Dev\Vixen` as the working directory for all commands.

1. Update Jira VIX-3929 using the Milestone 1 text in `Artifacts and Notes`.

2. Add or update these expected model files:

    `src/Vixen.Modules/App/CustomPropEditor/Model/ElementModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/Model/ElementModelType.cs`
    `src/Vixen.Modules/App/CustomPropEditor/Model/StateDefinitionModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/Model/StateItemModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/Model/StateDefinition.cs`

   `ElementModel` should expose these persisted members at completion when LiteDB compatibility permits using the desired public name:

    public Guid StatePropertyId { get; set; }
    public ElementModelType ModelType { get; set; }
    public ObservableCollection<StateDefinitionModel> StateDefinitions { get; set; }
    public StateDefinition StateDefinition { get; set; } // legacy only

   If older prop deserialization fails because the old serialized `StateDefinitions` field has the incompatible legacy item-like shape, preserve that serialized field as a legacy placeholder and introduce one of these authoritative alternatives:

    public ObservableCollection<StateDefinition> LegacyStateDefinitions { get; set; } // old item-like shape, migration input only
    public ObservableCollection<StateDefinitionModel> StateDefinitionModels { get; set; } // new authoritative shape

   or:

    public ObservableCollection<StateDefinition> StateDefinitions { get; set; } // old serialized field, migration input only
    public ObservableCollection<StateDefinitionModel> StateDefinitionModels { get; set; } // new authoritative shape

   Choose the smallest compatibility shim that can read old `.prp` files and stop writing meaningful legacy data. Record the final naming decision in the Decision Log.

   `StateDefinitionModel` should expose:

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ObservableCollection<StateItemModel> Items { get; set; }

   `StateItemModel` should expose:

    public Guid Id { get; set; }
    public string Name { get; set; }
    public System.Drawing.Color Color { get; set; }
    public ObservableCollection<Guid> ElementModelIds { get; set; }

   Add normalization methods that ensure non-empty IDs, non-null collections, trimmed names, and default names matching the State property model where appropriate. Public or protected APIs require XML documentation.

3. Add mapping and resolver helpers:

    `src/Vixen.Modules/App/CustomPropEditor/Services/PropStateModelResolver.cs`
    `src/Vixen.Modules/App/CustomPropEditor/Services/CustomPropStateDataMapper.cs`
    `src/Vixen.Modules/App/CustomPropEditor/Services/CustomPropStateMigrationService.cs`

   The mapper should convert new Custom Prop Editor models to State property `StateData`. The migration service should read old legacy state data and populate the new model shape only when equivalent new data is missing.

4. If using State property types directly, update:

    `src/Vixen.Modules/App/CustomPropEditor/CustomPropEditor.csproj`
    `Vixen.sln`

   Add a project reference to `..\..\Property\State\State.csproj`. Verify solution mappings if the project reference or solution entries change. This repo expects `Any CPU` solution mappings to point to `x64`.

5. Update xModel import:

    `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`

   Update `AssembleModel`, `AssembleSubModels`, `AssembleFaces`, `AssembleStates`, and `AttachStateDefinitions`. Preserve existing naming unless tests are intentionally updated for an already-known naming bug. Keep `CreateLegacyStateGroups = true`.

6. Update Preview import:

    `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs`

   Make `AddImportedStateDefinitions` read the new model shape. Preserve the old child `StateDefinition` fallback only when the new collection is empty. Ensure `StateData.Id`, State definition IDs, and State item IDs are carried into the attached State property.

7. Add editor view models and services:

    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/StateDefinitionEditorViewModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/CustomPropStateDefinitionViewModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/CustomPropStateItemViewModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/CustomPropStateAssignmentTreeNodeViewModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/State/CustomPropStatePreviewCoordinator.cs`

   If local patterns strongly prefer a different folder, record the decision in this plan before deviating.

8. Update existing editor view models:

    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/DrawingPanelViewModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementModelViewModel.cs`
    `src/Vixen.Modules/App/CustomPropEditor/ViewModels/LightViewModel.cs`

   `PropEditorViewModel` should expose the State editor view model and react to lower-left tab selection so preview activates only on the State Definition tab. It should call migration after loading props. `ElementModelViewModel` should expose `ModelType` and hide legacy State fields. `DrawingPanelViewModel` and `LightViewModel` should support temporary preview color overlays without losing normal selection behavior.

9. Update views:

    `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`
    `src/Vixen.Modules/App/CustomPropEditor/Views/StateDefinitionEditorView.xaml` if a separate user control is created
    `src/Vixen.Modules/App/CustomPropEditor/Views/StateDefinitionEditorView.xaml.cs` only for view-only initialization that cannot be expressed in XAML

   Prefer commands and bindings over code-behind. Use existing project images/resources for move buttons if practical.

10. Add or update tests:

    `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelImportHierarchyTests.cs`
    `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStateDataTests.cs`
    `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStateMigrationTests.cs`
    `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStateEditorViewModelTests.cs`
    `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStatePreviewTests.cs`
    `src/Vixen.Tests/Preview/VixenPreview/PreviewCustomPropStateImportTests.cs`

   Use exact test names that explain behavior, for example `Import_WithStateInfo_CreatesModelDefinitionWithStateItems`, `Migration_LegacyElementStateDefinition_IsIdempotent`, and `Preview_TabInactive_RestoresNormalLightColor`.

Run these focused commands as milestones progress:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor"
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~VixenPreview"

Run final validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
    git diff --check

Expected successful test output includes an xUnit summary with zero failed tests. Expected successful MSBuild output ends with zero errors. `git diff --check` should produce no whitespace errors.

## Validation and Acceptance

Automated acceptance is met when the focused and full validation commands pass and the tests prove these behaviors:

A manually created prop can store State definitions in the model element's `ElementModel.StateDefinitionModels`, save them, reopen them, and preserve all State property IDs, State definition IDs, State item IDs, colors, item order, descriptions, and assignments.

An xModel import with `stateInfo` creates one State definition per imported `stateInfo`, creates State items inside each definition, maps assignments to model leaf `ElementModel.Id` values, marks the imported model group as `Model`, marks imported submodel groups as `SubModel`, marks imported faceInfo groups as `FaceInfo`, and marks imported legacy stateInfo groups as `StateInfo`.

Preview import attaches one Vixen State property to the explicit model element when present, otherwise to the root fallback. The State property contains the same definitions and items as the Custom Prop Editor model data. Assignments map from `ElementModel.Id` to created `ElementNode.Id` values. Older legacy prop files still import through the fallback path, and new StateDefinitions data wins when both new and legacy data exist.

The State Definition tab can add, delete, rename, copy, validate, and reorder State definitions and items. Invalid State data blocks prop save and shows a clear warning. The assignment tree supports group selection semantics matching State property setup.

The drawing surface switches to State Definition preview only when the State Definition tab is active, shows non-active nodes in light gray, overlays active item colors, mixes overlapping colors deterministically, updates when assignments or colors change, updates assignment checkboxes when the user selects viewer elements while a specific State item row is selected, and returns to normal behavior when another lower-left tab is active.

Manual acceptance is met with this scenario:

1. Start Vixen from Debug output and open the Custom Prop Editor.
2. Import a small xModel with `stateInfo`, `faceInfo`, and `subModel`.
3. Confirm the lower-left area has `Prop Info`, `Element Info`, and `State Definition` tabs.
4. Confirm the imported State definitions appear on the State Definition tab.
5. Select the State Definition tab and confirm the right-side prop viewer changes to light-gray State preview mode.
6. Select a State item row and confirm only that row's assigned nodes show the item color in Selected State Item mode.
7. Select an unassigned node in the drawing surface and confirm that node is checked in the selected State item's assignment tree.
8. Switch to `<ALL>` or group preview and confirm active State item colors render, with overlaps shown as a mixed color.
9. Switch to Prop Info or Element Info and confirm the right-side viewer returns to normal behavior.
10. Save, close, reopen, and confirm State definitions, items, colors, assignments, Model Type values, and row order persisted.
11. Import the prop into Vixen Preview and confirm the attached element has the Vixen State property with the authored definitions.

## Idempotence and Recovery

This plan is safe to implement incrementally. The data model and migration helpers should be additive first: keep legacy `ElementModel.StateDefinition` and old item-like `ElementModel.StateDefinitions` readable until all new paths pass tests. If a test fails after introducing `ElementModel.StateDefinitionModels`, update the mapper or migration helper rather than restoring writes to legacy state fields.

If adding a project reference from CustomPropEditor to the State property project creates a circular reference, stop and record the dependency issue in `Surprises & Discoveries`. In that case, use Custom Prop Editor DTOs with explicit mapper methods in Preview, where the State property reference already exists. Do not force a circular reference.

If LiteDB cannot deserialize older props after changing the `StateDefinitions` collection type, add a tolerant migration shim rather than abandoning the new model. First try to preserve the desired new public name by moving the old item-like collection to `LegacyStateDefinitions` or a private LiteDB-friendly backing property. If LiteDB requires the old serialized field name to remain, keep old `StateDefinitions` as migration input only and use a new authoritative property such as `StateDefinitionModels` or `StateData`. Record the exact serialized compatibility approach in the Decision Log and update the mapper, migration tests, and Preview import tests to use the final authoritative property.

If local preview color overlays interfere with normal selection colors, keep the preview state separate from persisted `LightViewModel` properties and restore the previous visual state when leaving the State Definition tab. Add regression tests for leaving the tab, closing the editor, and opening another prop.

If the full test suite has unrelated failures, rerun the failing tests alone and record whether they are pre-existing or introduced by this work. Do not hide new failures behind a broad statement.

## Artifacts and Notes

Milestone 1 Jira update text:

    ### VIX-3929 Custom Prop Editor State Authoring

    #### Goal

    Add State definition authoring to the Custom Prop Editor so users can create, edit, preview, save, reopen, and import custom props with State data. The authored data maps to the existing Vixen State property used by Display Setup and the State effect.

    #### Requirements

    - Add a `State Definition` tab next to `Prop Info` and `Element Info`.
    - Persist authoritative State data in the model element's `ElementModel.StateDefinitionModels` collection.
    - Deprecate new writes to legacy `ElementModel.StateDefinition` and hide the old Element Info fields `State Name`, `State Item`, and `State Item Color`.
    - Add a `Model Type` field with `None`, `Model`, `SubModel`, `FaceInfo`, and `StateInfo`.
    - Enforce one explicit `Model` element at most; use the root element as the implicit model when no explicit model exists.
    - Map xModel `stateInfo` into `ElementModel.StateDefinitionModels`.
    - Continue creating legacy State groups during xModel import while the compatibility option remains enabled for production validation.
    - Migrate older prop files with legacy element-level State data into `ElementModel.StateDefinitionModels`.
    - Import props into Preview by attaching one State property to the explicit model element, imported model element, or root fallback.
    - Provide a local Custom Prop Editor preview that is active while the State Definition tab is active, shows the full prop in light gray, overlays active State item colors, mixes overlapping colors deterministically, lets viewer selection update assignments for the selected State item row, and does not publish Live Preview messages.

    #### High-Level Design

    Use the State property model as the contract for Custom Prop Editor State authoring. Store State definitions on the model element's `ElementModel.StateDefinitionModels` collection, assign State items to `ElementModel.Id` values, and map those IDs to `ElementNode.Id` values during Preview import. Keep legacy element-level State data readable for migration and direct Preview import, but make `ElementModel.StateDefinitionModels` authoritative when both exist.

    #### Acceptance Criteria

    - Users can author State definitions in the Custom Prop Editor for imported and manually created props.
    - Authored State definitions save, reopen, and import into Preview as a real State property.
    - xModel imports show imported `stateInfo` in the State Definition tab.
    - Older props with legacy State data migrate in the editor and still import directly into Preview.
    - Model Type values display, persist, and follow the defined rules.
    - Invalid State data blocks save with a clear warning.
    - Local preview updates inside the Custom Prop Editor canvas, mixes overlapping colors, updates assignments from viewer selection when a State item row is selected, and restores normal viewer behavior when the tab is not active.

    #### Testing

    - Add automated tests for State data persistence through `ElementModel.StateDefinitionModels`, Model Type rules, xModel import mapping, legacy migration, Preview import mapping, State Definition editor view-model behavior, save blocking, local preview behavior, and no Live Preview broadcast use.
    - Run focused Custom Prop Editor, State property, and Preview import tests.
    - Run the full test suite, Debug and Release solution builds, and `git diff --check` before final acceptance.

    #### Risks

    - The existing `ElementModel.StateDefinitions` collection is currently item-like, so migration and test updates must be careful.
    - LiteDB persistence may require defensive migration code for missing properties and old serialized shapes.
    - Preview import must preserve current FaceInfo, SubModel, color setup, and element naming behavior while adding State property attachment.
    - The drawing surface needs preview overlays that do not corrupt normal selection colors.

Validation evidence should be recorded here as work proceeds. Use concise transcripts such as:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor"
    Passed!  - Failed: 0, Passed: <N>, Skipped: 0, Total: <N>

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    Build succeeded.
    0 Error(s)

Milestone 1 Jira update:

    VIX-3929 was updated with the plan summary, high-level design, risks, acceptance criteria, and testing steps.

Milestone 2 validation:

    dotnet build src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Build succeeded with existing warnings, including the known LiteDB NU1904 advisory.

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor" --no-restore
    Passed!  - Failed: 0, Passed: 26, Skipped: 0, Total: 26

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State" --no-restore
    Passed!  - Failed: 0, Passed: 77, Skipped: 0, Total: 77

    git diff --check
    Exited successfully. Git printed an expected line-ending normalization warning for the new plan file; no whitespace errors were reported.

## Interfaces and Dependencies

Use existing Custom Prop Editor types:

    VixenModules.App.CustomPropEditor.Model.Prop
    VixenModules.App.CustomPropEditor.Model.ElementModel
    VixenModules.App.CustomPropEditor.Model.Light
    VixenModules.App.CustomPropEditor.ViewModels.PropEditorViewModel
    VixenModules.App.CustomPropEditor.ViewModels.ElementModelViewModel
    VixenModules.App.CustomPropEditor.ViewModels.DrawingPanelViewModel
    VixenModules.App.CustomPropEditor.ViewModels.LightViewModel
    VixenModules.App.CustomPropEditor.Services.PropModelServices
    VixenModules.App.CustomPropEditor.Services.PropModelPersistenceService

Use existing State property types directly if dependency layering permits:

    VixenModules.Property.State.StateData
    VixenModules.Property.State.StateDefinitionData
    VixenModules.Property.State.StateItemData
    VixenModules.Property.State.StateDescriptor
    VixenModules.Property.State.StateModule

Use these Preview import types:

    VixenModules.Preview.VixenPreview.PreviewCustomPropBuilder
    Vixen.Sys.ElementNode
    Vixen.Sys.IElementNode

At completion, `ElementModel` must have a stable, serializable way to represent:

    ElementModelType ModelType
    Guid StatePropertyId
    ObservableCollection<StateDefinitionModel> StateDefinitions

At completion, new Custom Prop Editor State model types must map losslessly to the State property data types. The mapper should preserve stable IDs and should skip unmappable assignments only during Preview import when no matching `ElementNode` exists.

At completion, the State Definition editor view model must expose commands for:

    AddStateDefinitionCommand
    DeleteStateDefinitionCommand
    RenameStateDefinitionCommand
    CopyStateDefinitionCommand
    AddStateItemCommand
    RemoveStateItemCommand
    MoveStateItemUpCommand
    MoveStateItemDownCommand

At completion, preview coordination must be local to Custom Prop Editor. It must not publish `TurnOnElementsMessage`, `TurnOffElementsMessage`, `ClearActiveEffectsMessage`, or any other Live Preview broadcast message.

## Revision Notes

- 2026-06-11 / Codex: Initial ExecPlan created from `docs/state/vix-3929-custom-prop-editor-state.md`, the current Custom Prop Editor model/import code, Preview custom prop builder, State property model, and project-specific skill guidance. The plan records the user's clarified decisions about storage, legacy deprecation, tab-activated preview, mixed colors, save blocking, and helper reuse.
- 2026-06-11 / Codex: Marked Milestone 1 complete after user confirmation that Jira issue VIX-3929 was updated with the planning details.
- 2026-06-11 / Codex: Clarified the `ElementModel.StateDefinitions` serialization migration strategy. The plan initially directed implementers to prefer the new structure under `StateDefinitions`, but allowed preserving the old serialized field as a legacy placeholder and adding a new authoritative property if LiteDB compatibility required it.
- 2026-06-11 / Codex: Completed Milestone 2 using the compatibility-safe fallback. The old item-like `ElementModel.StateDefinitions` remains as migration input, and the new authoritative collection is `ElementModel.StateDefinitionModels`.

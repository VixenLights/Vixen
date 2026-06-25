# Implement State Property Phase 4

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents\PLANS.md`. A future implementer must keep this document self-contained and update it whenever implementation details, discoveries, or decisions change.

## Purpose / Big Picture

Vixen users need one State property on an imported or manually configured prop to contain multiple named State definitions. Today the State property behaves like one state: it has one name, one description, and one list of State items. After this work, a single State property contains a creation-ordered collection of State definitions, and the user can select, add, delete, rename, copy, edit, validate, and preview each definition in the State Property Setup dialog.

The xLights xModel importer also needs to understand `CustomModelCompressed`, because modern xLights files can encode the same custom model nodes in compressed form. After this work, import resolves either `CustomModelCompressed` or `CustomModel` directly into shared Vixen model data, creates the same prop structure from either source, and maps every `StateInfo` into one State definition attached to the imported model child group. This can be seen by importing an xModel with multiple `StateInfo` tags and observing one top-level group, one child model group, optional Faces/Submodels/States groups as before, and one State property on the model child group containing all imported State definitions.

## Progress

- [x] (2026-06-03 10:55 America/Chicago) Read `.agents\PLANS.md` and the Phase 4 spec in `docs\vix-3591-state-property-phase-4.md`.
- [x] (2026-06-03 11:05 America/Chicago) Researched the current State property data model, setup ViewModel/XAML, preview coordinator tests, xLights parser, custom prop builder, and existing State import bridge.
- [x] (2026-06-03 11:15 America/Chicago) Created this initial ExecPlan.
- [x] (2026-06-03 11:42 America/Chicago) Implemented the State definition data model and updated stable identity/copy/clone behavior. Added focused tests for default definitions, logical clone ID preservation, and copy-as-new ID regeneration.
- [x] (2026-06-03 12:28 America/Chicago) Updated State Property Setup UI and ViewModels for multi-definition editing. Added selected-definition binding, Add/Delete/Rename/Copy commands, modal dialog service boundary, duplicate/case validation, selection blocking while invalid, and tests.
- [x] (2026-06-03 12:45 America/Chicago) Updated Preview behavior so it is scoped to the selected State definition. Suppressed intermediate preview refreshes during definition switches, then clear-and-rerendered once when Preview is on. Added tests for switching while on/off and inactive-definition edits.
- [x] (2026-06-03 13:18 America/Chicago) Refactored xModel import to decode `CustomModelCompressed` or `CustomModel` into shared Vixen model data. Added direct compressed and uncompressed parsers, compressed-first source resolution, fallback from invalid compressed to valid uncompressed with warning logging, import abort with user-facing error when no valid source exists, and parser tests proving equivalent Vixen `ModelNode` state.
- [x] (2026-06-03 14:02 America/Chicago) Updated imported element hierarchy and State property attachment. xModel import now creates a top-level prop as before, creates a child CustomModel group named with ` - Model {1}`, keeps Submodels/Faces/optional legacy States as top-level children, maps `StateInfo` to imported State definitions on the model group, and has `PreviewCustomPropBuilder` create one multi-definition `StateModule` from those mappings.
- [ ] Add focused automated tests for data model, setup behavior, preview scoping, import hierarchy, compressed decoding, fallback behavior, and compatibility.
- [ ] Run automated tests and manual acceptance scenarios.
- [ ] Update Jira VIX-3591 after this plan is reviewed and before implementation begins, using a rolled-up final summary of
  requirements, design, acceptance criteria, and testing rather than posting each milestone separately.

## Surprises & Discoveries

- Observation: The current State property implementation already contains Phase 2 and Phase 3 work, including stable IDs on `StateData` and `StateItemData`, Catel validation in `StateMapperViewModel`, and Live Preview coordination in `StatePreviewCoordinator`.
  Evidence: `src\Vixen.Modules\Property\State\StateData.cs`, `src\Vixen.Modules\Property\State\StateItemData.cs`, `src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs`, and `src\Vixen.Modules\Property\State\Setup\Preview\StatePreviewCoordinator.cs`.

- Observation: The current xLights importer parses `CustomModel` into a comma/semicolon string and later calls `CustomModel.CreateModelNodesAsync()` to convert that string into `ModelNode` entries. This is not the desired Phase 4 shape for compressed data, because compressed data should not be converted into `CustomModel` text.
  Evidence: `src\Vixen.Modules\App\CustomPropEditor\Import\XLights\XModelImport.cs` assigns `cm.ModelDefinition = reader.GetAttribute("CustomModel")`, and `src\Vixen.Modules\App\CustomPropEditor\Import\XLights\CustomModel.cs` parses `ModelDefinition` by splitting rows and cells.

- Observation: Imported State properties are currently attached later by `PreviewCustomPropBuilder`, not during raw xModel parsing. The importer creates symbolic `ElementModel.StateDefinition` data, and the preview builder converts that symbolic data into real `StateModule` instances on `ElementNode` objects.
  Evidence: `src\Vixen.Modules\Preview\VixenPreview\PreviewCustomPropBuilder.cs` method `AddStateProperties` groups child `ElementModel` values with `StateDefinition != null` and then adds or updates `StateModule`.

- Observation: The Phase 4 spec was corrected during planning so invalid compressed data falls back to a valid `CustomModel` source when one exists, and aborts only when neither source can produce valid Vixen model data.
  Evidence: `docs\vix-3591-state-property-phase-4.md` section `Attribute Precedence And Errors`.

- Observation: `CustomModelCompressed` coordinate entries are `node,row,column` relative to the uncompressed `CustomModel` grid, not `node,x,y`. The parser must map compressed column to Vixen X and compressed row to Vixen Y.
  Evidence: In `docs\references\santa-waving.xmodel`, compressed entry `314,0,176` maps to uncompressed grid coordinate `x=176, y=0`.

## Decision Log

- Decision: Introduce a new `StateDefinitionData` type inside the State property module instead of keeping name, description, and items directly on `StateData`.
  Rationale: `StateData` becomes the container, and each State definition needs its own stable ID, name, description, item collection, clone behavior, and copy-as-new behavior. A dedicated type keeps ownership clear and avoids overloading `StateItemData`.
  Date/Author: 2026-06-03 / Codex

- Decision: Keep the current `StateItemData` stable ID behavior for logical clones, and add an explicit copy-as-new path that regenerates State item IDs when copying a State definition.
  Rationale: A copied State definition is a new logical definition. Effects and future references must not mistake copied items for the same persisted item identity.
  Date/Author: 2026-06-03 / Codex

- Decision: Decode both `CustomModelCompressed` and `CustomModel` directly into shared Vixen model-node data, then use one downstream assembly path.
  Rationale: Compressed and uncompressed formats are alternate source encodings. Converting compressed text to uncompressed text would add an unnecessary intermediate format and would make tests validate string conversion instead of the Vixen state the importer actually needs.
  Date/Author: 2026-06-03 / Codex

- Decision: Attach the imported State property to the CustomModel child group, such as `Santa Waving - Model {1}`, rather than the top-level imported group, such as `Santa Waving {1}`.
  Rationale: Faces, Submodels, and optional old States remain top-level children, while the State property describes the model node structure that StateInfo ranges map into.
  Date/Author: 2026-06-03 / Codex

- Decision: Keep a temporary internal importer constant that defaults to preserving old `States` group element creation.
  Rationale: This allows validating the new State property import against the old visible hierarchy before turning old States group creation off and later removing it.
  Date/Author: 2026-06-03 / Codex

## Outcomes & Retrospective

This plan captures the intended architecture, milestones, validation commands, and acceptance criteria needed to implement Phase 4. Update this section after each major milestone with what changed, what was verified, and what remains.

Milestone 1 is complete. `StateData` now exposes `StateDefinitions`, `StateDefinitionData` owns per-definition identity/name/description/items, logical clones preserve IDs, and copy-as-new regenerates State property, State definition, and State item IDs. Temporary first-definition compatibility properties remain on `StateData` and `StateModule` so the existing setup UI and tests continue to compile until Milestone 2 replaces the single-definition ViewModel surface.

Milestone 2 is complete. `StateMapperViewModel` now owns an editable collection of `StateDefinitionViewModel` objects, proxies existing `Name`, `Description`, and `Items` bindings through the selected definition, and supports Add, Delete, Rename, and Copy through an `IStateDefinitionDialogService` boundary. The setup XAML now includes the State definition combo box and management controls. Focused State property tests pass after adding coverage for definition selection, add/delete/rename/copy, duplicate blocking, case-only warnings, selection blocking, and save across all definitions.

Milestone 3 is complete. Preview is scoped to the selected State definition, switching definitions while Preview is on clears the State Preview context and renders the new definition once, switching while Preview is off publishes no messages, and edits to inactive definitions do not publish preview messages. Focused State property tests now include these multi-definition preview scenarios.

Milestone 4 is complete. xModel import now resolves `CustomModelCompressed` and `CustomModel` as alternate source encodings and decodes the selected source directly into `ModelNode` values. `CustomModelCompressed` is preferred when valid; invalid compressed data falls back to valid `CustomModel` data and logs a warning; no valid source logs an error, shows the user a model import error, and aborts the import. Parser tests use embedded minimal data based on santa and snowman examples and assert equivalent Vixen node state rather than compressed-to-uncompressed text conversion.

Milestone 5 is complete. Imported xModels now create a canonical model child group, such as `Santa Waving - Model {1}`, containing all decoded model lights. Submodels, Faces, and optional legacy States remain top-level children and reference the same model leaves without consuming them out of the model group. Imported `StateInfo` values are stored on the model child group and converted by `PreviewCustomPropBuilder` into one State property with multiple State definitions. Legacy States groups are still created behind the temporary constant and no longer carry old per-group State property metadata.

## Context and Orientation

Vixen is a Windows desktop WPF application. The State property lives in `src\Vixen.Modules\Property\State`. A property module is a reusable capability attached to an `IElementNode`, which is Vixen's in-memory representation of a display element or group of elements. A State property lets users describe named visual states of a prop. A State item is one row inside a state, with a name, a color, and assigned element node IDs.

The current State module is single-definition. `src\Vixen.Modules\Property\State\StateData.cs` has an `Id`, `Name`, `Description`, and `List<StateItemData> Items`. `src\Vixen.Modules\Property\State\StateModule.cs` exposes these fields and launches the setup dialog. `src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs` clones `StateData` into a draft, exposes `Name`, `Description`, and `Items`, validates names with Catel validation, saves draft edits back to the source, and coordinates Live Preview. `src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml` displays the setup dialog. `src\Vixen.Modules\Property\State\Setup\ViewModels\StateItemViewModel.cs` wraps each `StateItemData` row and owns the assignment tree.

The current xLights importer is in `src\Vixen.Modules\App\CustomPropEditor\Import\XLights`. The class `XModelImport` reads `.xmodel` XML and builds a `CustomModel` object. `CustomModel` currently stores a `ModelDefinition` string from the `CustomModel` XML attribute, then creates `ModelNode` values from the comma/semicolon grid string. A `ModelNode` is the intermediate data with node number, X coordinate, and Y coordinate used to create Vixen custom prop elements.

The imported xModel is not immediately a real Vixen element tree. `XModelImport.Assemble` creates a custom prop model using `PropModelServices`. Later, `src\Vixen.Modules\Preview\VixenPreview\PreviewCustomPropBuilder.cs` converts the custom prop model into actual Vixen `ElementNode` objects. This builder is where `StateModule` is currently attached from symbolic imported `StateDefinition` values on child `ElementModel` objects.

Important terms used in this plan:

State property means the property module instance attached to a Vixen element node. In Phase 4 it is a container only.

State definition means one named state inside the State property container. It has a stable ID, name, description, and State items.

Stable ID means a persisted `Guid` that survives rename and normal editing so future effects can keep referring to the same State definition.

Logical clone means a draft or serialization clone of the same logical object. It preserves IDs.

Copy-as-new means a user action that creates a new distinct State definition from an existing one. It generates new State definition and State item IDs.

CustomModel means the xLights XML attribute that defines model nodes in a comma/semicolon rectangular grid.

CustomModelCompressed means the xLights XML attribute that defines the same model nodes in a compressed text format.

Shared Vixen model representation means the internal C# data used by Vixen after either source format is parsed. For this implementation it should be a direct collection of `ModelNode` entries, or a small wrapper around those entries, not a converted `CustomModel` string.

## Plan of Work

Milestone 1 changes the State property data model. Add `src\Vixen.Modules\Property\State\StateDefinitionData.cs` as a public sealed data contract type with `Guid Id`, `string Name`, `string Description`, and `List<StateItemData> Items`. Its constructor generates a new non-empty `Id`, default name `State - 1`, empty description, and one default `StateItemData` only when used through a factory or setup creation path. To avoid surprising deserialization behavior, keep constructor defaults conservative and put reusable creation rules in a helper such as `StateDefinitionData.CreateDefault(string name)`.

Update `StateData` so it exposes `List<StateDefinitionData> StateDefinitions` and no longer treats `Name`, `Description`, and `Items` as the container-level contract. Because this feature is unreleased, no legacy migration is required, but tests and any in-repo construction helpers must be updated. `StateData.Normalize()` must ensure `StateDefinitions` is non-null and every definition is normalized. The default setup creation path must ensure at least one valid definition exists. `StateData.Clone()` must preserve definition IDs and item IDs. Add explicit methods such as `CloneForNewProperty()` and `StateDefinitionData.CloneAsNew(string name)` so copy paths intentionally regenerate IDs. Update XML comments because these are public or protected APIs.

Update `StateModule` so its public surface reflects the container model. Keep `Id` only if it still represents the attached property ID needed by existing callers, but add `StateDefinitions` access. Remove or obsolete direct `Name`, `Description`, and `Items` usage after updating all callers. If temporary compatibility properties are needed to get the branch compiling during the milestone, keep them internal to the milestone and remove them before acceptance. `CloneValues(IProperty sourceProperty)` must continue to create a distinct State property copy and must regenerate appropriate top-level and copied nested IDs.

Milestone 1 is verified by updating and running State data tests:

    cd C:\Dev\Vixen
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"

Before the changes, tests for `StateDefinitions` will not compile. After the milestone, expect the State data and clone tests to pass, including tests proving construction, normalization, clone, rename, and copy-as-new ID behavior.

Milestone 2 updates the setup dialog ViewModel and XAML for multi-definition editing. The main ViewModel should own an observable collection of State definition ViewModels, likely a new `StateDefinitionViewModel` in `src\Vixen.Modules\Property\State\Setup\ViewModels\StateDefinitionViewModel.cs`. That ViewModel wraps one `StateDefinitionData`, exposes `Name`, `Description`, and `ObservableCollection<StateItemViewModel> Items`, validates the definition name, and delegates State item validation to existing `StateItemViewModel`.

Refactor `StateMapperViewModel` so `Draft` is still a cloned `StateData`, but the selected definition controls the visible description, State item grid, assignment tree, and Preview source. Add `AvailableStateDefinitions`, `SelectedStateDefinition`, `AddStateDefinitionCommand`, `DeleteStateDefinitionCommand`, `RenameStateDefinitionCommand`, and `CopyStateDefinitionCommand`. Selection changes must be blocked if the current definition has blocking validation errors. A practical approach is to keep a private `_lastValidSelectedStateDefinition`, validate before accepting selection, and revert the bound selection when invalid. The implementation should keep focus on the invalid edit through WPF validation behavior; do not add code-behind event handlers unless there is no Catel-friendly alternative.

Add a modal name-entry dialog for Add, Rename, and Copy. Prefer Catel services and ViewModels over direct `MessageBox` or direct WPF dialog calls inside ViewModels. If the existing module has no suitable generic prompt service, add a small State-specific service interface under `src\Vixen.Modules\Property\State\Setup\Services`, for example `IStateDefinitionDialogService`, with methods to request a name and confirm deletion. Implement it in WPF-facing code so `StateMapperViewModel` remains UI-agnostic. The dialogs must trim names, block empty names, block exact duplicates, allow case-only differences with a non-blocking warning, and close with OK only when valid.

Update `StateMapperView.xaml`. Add a combo box for State definitions, an Add button, a Delete button, and an `...` button with Rename and Copy. Keep the existing description, Preview controls, State item grid, and assignment tree, but bind them to the selected State definition through the ViewModel. The last State definition cannot be deleted. After delete, select the next definition when one exists, otherwise the previous. Add and Copy append at the end and select the new definition.

The State Item `DataGrid` allows column-header sorting. That visible sorted row order must become the persisted order when the user clicks OK. Do not rely on the underlying `ObservableCollection<StateItemViewModel>` being reordered by WPF sorting; WPF collection views can present a sorted view while leaving the source collection unchanged. A concrete fix is to add a `StateItemSortRequestedCommand` or view-facing sort synchronization hook that receives the sorted `DataGrid.Items` order after WPF applies the column sort, then asks `StateMapperViewModel` to reorder the selected definition's `Items` collection to match the visible `StateItemViewModel` sequence. The reorder must move existing `StateItemViewModel` instances, not recreate them, so State item IDs, validation state, colors, and assignments are preserved. Suppress preview and group-list refresh during the batch reorder, then rebuild State Item Group choices and refresh preview once if needed. If a no-code-behind approach is practical with Catel behaviors, prefer it; otherwise keep any required view code-behind limited to translating the sorted `DataGrid.Items` sequence into a ViewModel command.

Milestone 2 is verified by adding tests in `src\Vixen.Tests\Property\State`, extending existing `StateMapperValidationTests`, `StateMapperDraftTests`, and `StateMapperPreviewTests` or adding new `StateDefinitionMapperTests`. Tests must cover add, delete-last blocked, delete selection fallback, rename, copy-as-new IDs, exact duplicate blocking, case-only warning, creation order, selection blocked while invalid, save persists all definitions, cancel discards draft edits, and saving after a State Item grid sort persists the displayed item order without changing State item IDs or assignments.

Milestone 3 scopes Preview to the selected State definition. Existing classes in `src\Vixen.Modules\Property\State\Setup\Preview` should remain mostly intact. The main change is that `StateMapperViewModel.GetPreviewedItems()` must read from the selected State definition's `Items`, not a single global `Items` collection. When Preview is on and the user switches definitions, clear the `"State Preview"` context and refresh from the new selected definition. When Preview is off, switching definitions must not publish Live Preview messages. Dialog close still releases or clears the context exactly as Phase 3 specifies.

Milestone 3 is verified by updating `StateMapperPreviewTests` and `StatePreviewCoordinatorTests`. Add tests proving Preview is scoped to selected definition, switching definitions while Preview is on clears and re-renders, switching while Preview is off sends no messages, and existing selected-row and group preview behavior still works within one definition.

Milestone 4 refactors xModel model-source parsing. Introduce a small internal representation for decoded xLights custom model nodes. A concrete option is `internal sealed class DecodedCustomModel` in `src\Vixen.Modules\App\CustomPropEditor\Import\XLights\DecodedCustomModel.cs` with `IReadOnlyDictionary<int, ModelNode> NodesByNumber`, `int X`, `int Y`, `int Scale`, and other values needed by assembly. Another option is to let `CustomModel` hold decoded node data directly and remove string-based `CreateModelNodesAsync()`. Choose the smaller change that keeps the parser testable and avoids converting compressed text into uncompressed `CustomModel` text.

Add parser classes or methods such as `CustomModelGridParser` and `CustomModelCompressedParser`. The `CustomModel` parser reads comma/semicolon grid text and produces `ModelNode` entries directly. The compressed parser translates the xLights compressed behavior directly into the same `ModelNode` entries. Use xLights `CustomModel::ToCompressed` and `CustomModel::ToCustomModel` behavior as the compatibility standard, but do not convert compressed text into a `CustomModel` text string as an intermediate result. The tests must validate Vixen state, meaning node number, X coordinate, Y coordinate, scale, and downstream generated elements.

Update `XModelImport.LoadModelFileAsync()` to read both attributes. If `CustomModelCompressed` exists, try to decode it first. If it decodes, use it. If it fails and `CustomModel` exists and decodes successfully, log a warning and use `CustomModel`. If neither source decodes, show a user-facing error through `IMessageService`, log an error with structured NLog parameters, and abort the whole import by returning no prop or throwing an import exception consistent with existing importer behavior. If only `CustomModel` exists, use the uncompressed parser. Missing or invalid `parm1` or `parm2` must keep existing behavior.

Milestone 4 is verified with new tests under `src\Vixen.Tests\App\CustomPropEditor\Import\XLights`. Tests should use minimal embedded XML or parser inputs derived from `docs\references\santa-waving.xmodel` and `docs\references\snowman-6ft.xmodel`, but must not read those files. Add equivalent compressed and uncompressed inputs from those examples and assert that both decode to the same Vixen node state. Add tests that both attributes prefer compressed, invalid compressed falls back to valid uncompressed, and no valid source reports an error.

Milestone 5 updates imported element hierarchy and State property attachment. `XModelImport.Assemble` currently creates Submodels, Faces, States, and leftover "Other" groups directly under the imported prop root. Phase 4 requires the top-level group to remain named as today, such as `Santa Waving {1}`. Under it, create a CustomModel child group named with the same wildcard convention and the word `Model`, such as `Santa Waving - Model {1}`. The decoded model lights belong under that model child group. Faces remain children of the top-level group. Submodels remain children of the top-level group. The temporary old States groups remain children of the top-level group when enabled.

Add an internal constant near `XModelImport`, for example `private const bool CreateLegacyStateGroups = true;`, with a removal comment. When true, old States group elements are created exactly as today and no extra State property logic is attached to those old State groups. When false in future work, only the new State property path remains. Do not expose this constant in UI or configuration.

Update the symbolic model so `StateInfo` can be attached to the CustomModel child group. The current `PreviewCustomPropBuilder.AddStateProperties` scans children with `ElementModel.StateDefinition` and creates one State property on their parent. For Phase 4, it must create one `StateModule` on the CustomModel child group and fill `StateData.StateDefinitions`, one per imported `StateInfo`. Each definition contains State items mapped to the actual leaf `ElementNode` IDs produced under the CustomModel child group. Preserve duplicate State item name behavior within each definition.

Be careful with `lightNodes`. Existing assembly removes nodes from `modelNodes` when they are consumed by Submodels, Faces, or States and reuses existing light nodes when ranges overlap. In Phase 4, StateInfo should map to model leaves, not create a separate structure for the new State property. The implementation should not remove model nodes solely because StateInfo references them for the new State property. Legacy States group creation can continue using the old consume/reuse behavior while the constant is true, but new State property mappings should derive assigned leaf IDs from the final model child leaves.

Milestone 5 is verified with tests that import a small xModel and then run the custom prop builder to real ElementNodes. Assert the top-level node name uses today's wildcard behavior, the child model group exists with ` - Model {1}`, the State property is on the model group, Faces and Submodels remain direct children of the top-level group, optional legacy States are direct children when the constant is true, and models without StateInfo import without a State property.

Milestone 6 updates naming and import normalization. Change `XLightsStateNameNormalizer.NormalizeStateName` or add a new method for State definition names so blank imported StateInfo names become `State - N`, not `State Name N`. Add a uniqueness helper that preserves the first exact imported name and appends ` - 2`, ` - 3`, and so on to later exact duplicates until unique. Case-only differences remain distinct. Keep State item name normalization unchanged unless tests prove it needs adjustment.

Milestone 6 is verified by updating `XLightsStateNameNormalizerTests` and adding import tests for blank names, exact duplicate suffixing, existing suffix collisions, and case-only names.

Milestone 7 writes reference documentation. Create a new file such as `docs\references\xmodel-custom-model-formats.md`. Explain `CustomModel`, `CustomModelCompressed`, `parm1`, `parm2`, grid coordinates, node numbering, parser precedence, fallback behavior, and the shared Vixen representation. Include small illustrative examples using indented snippets, not large copied xLights source. State that tests validate equivalent Vixen state rather than string conversion.

Milestone 8 performs full validation and cleanup. Run focused tests after each milestone, then run:

    cd C:\Dev\Vixen
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

If the full suite is too slow or blocked by environment issues, run the focused State and xLights tests and record the limitation in this plan. Build the solution when ready:

    cd C:\Dev\Vixen
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Manual validation requires opening Display Setup, adding or importing a prop, opening State Property Setup, and verifying the scenarios in `docs\vix-3591-state-property-phase-4.md`. If UI automation is not available, record manual observations and screenshots separately if the team normally captures them.

Milestone 9 updates Jira VIX-3591. Do this after the ExecPlan is reviewed so Jira contains the finalized scope, high-level design, acceptance criteria, test commands, and any known risks. The Jira update must be a rolled-up final-state summary of Phase 4 requirements and testing, not a sequence of milestone-by-milestone append blocks. The Jira update should mention that the State effect remains out of scope but can later reference State definition stable IDs.

## Concrete Steps

Start from the repository root:

    cd C:\Dev\Vixen

Inspect current changes before implementation, because this workspace may contain uncommitted development work:

    git status --short

Read the core State files before editing:

    Get-Content src\Vixen.Modules\Property\State\StateData.cs
    Get-Content src\Vixen.Modules\Property\State\StateItemData.cs
    Get-Content src\Vixen.Modules\Property\State\StateModule.cs
    Get-Content src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs
    Get-Content src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml

Read the importer and builder files before editing:

    Get-Content src\Vixen.Modules\App\CustomPropEditor\Import\XLights\XModelImport.cs
    Get-Content src\Vixen.Modules\App\CustomPropEditor\Import\XLights\CustomModel.cs
    Get-Content src\Vixen.Modules\Preview\VixenPreview\PreviewCustomPropBuilder.cs
    Get-Content src\Vixen.Modules\App\CustomPropEditor\Model\ElementModel.cs
    Get-Content src\Vixen.Modules\App\CustomPropEditor\Model\StateDefinition.cs

After each milestone, run a focused test command. Useful focused commands are:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights"

At the end, run the whole test project:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

Then build:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Expected success is that `dotnet test` reports all selected tests passed and `msbuild` reports `Build succeeded.` If network or package restore is blocked, rerun the same command with the required approval or record the environment limitation in `Outcomes & Retrospective`.

## Validation and Acceptance

Data model acceptance is demonstrated by tests proving `StateData` contains one or more `StateDefinitionData` objects, newly created definitions have non-empty IDs, logical clones preserve IDs, rename preserves IDs, copy-as-new regenerates definition and item IDs, and zero definitions is invalid.

Setup UI acceptance is demonstrated by opening State Property Setup and observing a State definition combo box. Add prompts for a unique name and creates a new definition at the end. Delete confirms first and cannot remove the last definition. Rename preserves the same definition. Copy prompts for a new name, appends a copied definition, and later edits to the copy do not alter the source. Exact duplicate names are blocked. `Open` and `open` are allowed but produce a warning. Switching definitions with invalid edits is blocked. Sorting the State Item grid by a column header and clicking OK persists the displayed row order for that State definition. OK persists all definitions; Cancel discards draft edits.

Preview acceptance is demonstrated by configuring two definitions with different assigned leaves and colors. With Preview on, switching definitions clears the old preview and activates the new selected definition. With Preview off, switching definitions sends no Live Preview messages. OK, Cancel, and window close clear the `"State Preview"` context.

xModel hierarchy acceptance is demonstrated by importing a model named `Santa Waving`. The created top-level group remains named with the current wildcard behavior, such as `Santa Waving {1}`. The model leaves are under `Santa Waving - Model {1}`. The State property is attached to `Santa Waving - Model {1}`. Faces, Submodels, and optional legacy States are children of `Santa Waving {1}`. Models without StateInfo do not get a State property. Missing FaceInfo does not fail import. Existing Submodel behavior remains unchanged.

CustomModelCompressed acceptance is demonstrated by tests using minimal embedded data based on santa and snowman examples. The compressed and uncompressed inputs must parse directly into equivalent Vixen model node state. Tests must not compare compressed-to-uncompressed text conversion. When both attributes exist, compressed is preferred. If compressed is invalid and uncompressed is valid, import falls back to uncompressed and logs a warning. If no valid source exists, import aborts with a user-facing error and logged error.

Future State effect contract acceptance is documentation-level for this phase. The data model must expose stable State definition IDs so the future State effect can persist that ID, use the name only for display, refuse to render when the referenced definition is missing, and activate State items by current within-definition behavior.

## Idempotence and Recovery

The implementation is additive until old single-definition code is removed. If a milestone fails, keep the last passing state by reverting only the changes made in that milestone, not unrelated user changes. Do not use `git reset --hard` or `git checkout --` unless explicitly requested. Prefer small file-scoped edits and focused tests after each milestone.

The xModel importer should be safe to run repeatedly. The temporary legacy States constant defaults to true and creates the old hierarchy as before; turning it off later should be a separate small change with tests. Parser tests must use embedded minimal fixtures, so they do not depend on external files moving or being deleted.

If the compressed parser is uncertain, add parser-only tests before wiring it into `XModelImport`. This keeps failures local and avoids breaking the UI import path while the algorithm is being translated.

If WPF dialog service wiring becomes too large, first implement the State definition data model and ViewModel behavior with mocked dialog services in tests. Then add the concrete dialog views as a later milestone.

## Artifacts and Notes

Current relevant source shape:

    StateData currently has:
      Guid Id
      string Name
      string Description
      List<StateItemData> Items

    Phase 4 target:
      StateData
        Guid Id, if still required for attached property identity
        List<StateDefinitionData> StateDefinitions

      StateDefinitionData
        Guid Id
        string Name
        string Description
        List<StateItemData> Items

Current xModel assembly shape:

    XModelImport.LoadModelFileAsync reads CustomModel XML into CustomModel.ModelDefinition.
    CustomModel.CreateModelNodesAsync parses ModelDefinition text into ModelNode values.
    XModelImport.Assemble creates Submodels, Faces, States, then leftover Other nodes.
    PreviewCustomPropBuilder.AddStateProperties converts symbolic StateDefinition children into StateModule data.

Phase 4 target import shape:

    Resolve CustomModelCompressed or CustomModel.
    Decode source directly into shared Vixen model node state.
    Create top-level prop group with existing name wildcard behavior.
    Create CustomModel child group named "{Model Name} - Model {1}".
    Put model leaves under the CustomModel child group.
    Keep Faces and Submodels as top-level children.
    Keep optional legacy States as top-level children while the constant is true.
    Attach one State property to the CustomModel child group.
    Create one State definition per StateInfo inside that property.

Important bug to avoid:

    Do not remove model nodes from the CustomModel child group merely because StateInfo references them.
    StateInfo should map assignments to model leaves. It should not consume those leaves out of the model group.

## Interfaces and Dependencies

Use Catel MVVM patterns already present in `StateMapperViewModel`. ViewModels inherit from `Catel.MVVM.ViewModelBase`, commands use `TaskCommand`, validation belongs in `ValidateFields` and `ValidateBusinessRules`, and ViewModels must not directly call WPF `MessageBox` or instantiate WPF dialog windows except through a service boundary.

Use existing preview integration in `src\Vixen.Modules\Property\State\Setup\Preview`. Keep Live Preview messages isolated to the `"State Preview"` context. Do not add a dependency from the State property module to the Live Preview app module.

Use NLog for importer warnings and errors. Log structured values rather than string interpolation when adding new logging.

Public or protected C# APIs added for `StateDefinitionData`, parser helpers, or services must have XML documentation following `.agents\skills\csharp-docs\SKILL.md`. Internal classes should also be documented when they encode non-obvious parser behavior.

Recommended new or changed types:

    src\Vixen.Modules\Property\State\StateDefinitionData.cs
      public sealed class StateDefinitionData
      public Guid Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public List<StateItemData> Items { get; set; }
      public StateDefinitionData Clone()
      internal StateDefinitionData CloneAsNew(string name)
      internal void Normalize()

    src\Vixen.Modules\Property\State\StateData.cs
      public List<StateDefinitionData> StateDefinitions { get; set; }
      public override IModuleDataModel Clone()
      internal StateData CloneForNewProperty()
      internal void Normalize()

    src\Vixen.Modules\Property\State\Setup\ViewModels\StateDefinitionViewModel.cs
      public sealed class StateDefinitionViewModel : ViewModelBase
      public string Name { get; set; }
      public string Description { get; set; }
      public ObservableCollection<StateItemViewModel> Items { get; }
      internal StateDefinitionData Definition { get; }

    src\Vixen.Modules\Property\State\Setup\Services\IStateDefinitionDialogService.cs
      Task<string?> RequestNameAsync(string title, string initialName, IReadOnlyCollection<string> existingNames, string? currentName)
      Task<bool> ConfirmDeleteAsync(string name)

    src\Vixen.Modules\App\CustomPropEditor\Import\XLights\CustomModelGridParser.cs
      internal parser from CustomModel grid text to shared Vixen model node state

    src\Vixen.Modules\App\CustomPropEditor\Import\XLights\CustomModelCompressedParser.cs
      internal parser from compressed text to shared Vixen model node state

    src\Vixen.Modules\App\CustomPropEditor\Import\XLights\DecodedCustomModel.cs
      internal shared representation consumed by XModelImport assembly

If implementation discovers better names that fit the codebase more closely, update this section and the Decision Log before proceeding.

## Revision Notes

- 2026-06-03 / Codex: Initial ExecPlan created from `docs\vix-3591-state-property-phase-4.md`, after source research of the State module, xLights import path, and custom prop builder.
- 2026-06-03 / Codex: Updated Jira guidance after plan review. Jira VIX-3591 should receive a rolled-up final-state
  requirements, design, acceptance, and testing summary instead of separate milestone append blocks.
- 2026-06-03 / Codex: Completed Milestone 1. Added `StateDefinitionData`, refactored `StateData` into a definition
  container, preserved temporary first-definition compatibility properties for the existing setup UI, and verified the
  focused State property tests.
- 2026-06-03 / Codex: Completed Milestone 2. Added setup ViewModel and XAML support for selecting and managing State
  definitions, plus a dialog service boundary and focused tests.
- 2026-06-03 / Codex: Completed Milestone 3. Tightened selected-definition Preview switching and added tests proving
  clear-and-rerender behavior, off-state suppression, and inactive-definition edit suppression.
- 2026-06-03 / Codex: Completed Milestone 4. Added direct xModel `CustomModelCompressed` and `CustomModel` parsing into
  shared `ModelNode` data, compressed-first fallback behavior, import abort/error handling for invalid sources, and focused
  xLights parser tests.
- 2026-06-03 / Codex: Completed Milestone 5. Updated xModel assembly to create the canonical ` - Model {1}` child group,
  preserve top-level Faces/Submodels/legacy States, attach imported StateInfo mappings to the model group, and convert
  those mappings into multi-definition State property data in `PreviewCustomPropBuilder`.
- 2026-06-03 / Codex: Corrected `CustomModelCompressed` coordinate mapping after visual import validation. Compressed
  entries are decoded as `node,row,column`, so column maps to Vixen X and row maps to Vixen Y. Updated parser tests with
  paired santa and snowman values that match the uncompressed grid.

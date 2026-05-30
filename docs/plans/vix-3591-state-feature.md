# Implement VIX-3591 State Property Mapping

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md`. Any contributor implementing this plan must keep it self-contained and revise it whenever progress is made, surprises are found, or design decisions change.

## Purpose / Big Picture

Vixen users need a general way to describe named prop states, such as a Santa arm being up or down, a reindeer face expression, or a seven-segment sign digit. After this work, a user can select an element in Display Setup, add or configure the State property, define an overall state definition name and description, add one or more state rows, choose a display color for each row, and assign child `IElementNode` entries to each row from a checkbox tree. The saved data becomes the baseline consumed later by a State effect and can also be populated from imported xLights `stateInfo` data.

The visible proof of the feature is in Display Setup: select a parent prop element, add or configure the State property, create rows such as `Arm Up` and `Arm Down`, check children in the element tree for each row, save, reopen the property, and observe the same rows, colors, descriptions, and element assignments.

## Progress

- [x] (2026-05-29) Read `docs\vix-3591-state-feature.md` and `.agents\PLANS.md`.
- [x] (2026-05-29) Read project skill guidance for `dotnet-best-practices`, `csharp-async`, `csharp-docs`, and `dotnet-design-pattern-review`.
- [x] (2026-05-29) Researched existing Face property setup, Display Setup property configuration, partial State property implementation, color picker controls, and xLights state import code.
- [x] (2026-05-29) Created this initial implementation plan.
- [x] (2026-05-29) Revised the plan to include required unit-test coverage in `src\Vixen.Tests`.
- [x] (2026-05-30) Completed Milestone 1: updated Jira issue `VIX-3591` with the prepared requirements, design, acceptance criteria, and testing steps.
- [x] (2026-05-30) Completed Milestone 2: implemented the State property data contract, stable element assignment IDs, deep module cloning, and project-wide nullable analysis.
- [x] (2026-05-30) Completed Milestone 3: rebuilt the State setup UI around a cloned draft, added UI-free assignment tree models, added the color chooser service, and removed placeholder drag/drop behavior.
- [x] (2026-05-30) Connected the setup dialog save/cancel path so Catel save applies the draft and cancel leaves persisted State data unchanged.
- [ ] Add unit tests for State data cloning, module cloning, setup draft save/cancel behavior, and assignment count/group selection logic.
- [x] (2026-05-30) Completed Milestone 5: materialized imported xLights `stateInfo` metadata as State properties on generated Vixen state-group elements.
- [ ] Build and manually validate the end-to-end workflow.

## Surprises & Discoveries

- Observation: A partial State property module already exists under `src\Vixen.Modules\Property\State`.
  Evidence: `StateDescriptor.cs`, `StateModule.cs`, `StateData.cs`, `Setup\ViewModels\StateMapperViewModel.cs`, and `Setup\Views\StateMapperView.xaml` are present.

- Observation: The current State property data model only stores one `StateName`, one `ItemName`, and one `ItemColor`, so it cannot represent the required overall definition, description, multiple states, or per-row element assignments.
  Evidence: `src\Vixen.Modules\Property\State\StateData.cs` has only three data members: `StateName`, `ItemName`, and `ItemColor`.

- Observation: The current State setup dialog is not yet a working editor.
  Evidence: `StateMapperViewModel.EditColorAsync` changes the color to black and assigns a GUID state name as test behavior, `DragOver` and `Drop` throw `NotImplementedException`, and `StateModule.SetupElements` always returns `true` regardless of dialog result.

- Observation: xLights `stateInfo` is already parsed and converted into custom prop editor model groups, but that model is separate from the Display Setup State property persistence.
  Evidence: `src\Vixen.Modules\App\CustomPropEditor\Import\XLights\XModelImport.cs` parses `stateInfo` and `AssembleStates` creates `StateDefinition` values on custom prop editor `ElementModel` objects.

- Observation: `Vixen.sln` contains a suspicious self dependency for the State project.
  Evidence: near the top of `Vixen.sln`, the `Vixen.Application` project dependency list includes `{4E9D03F3-E85D-4796-AADD-A75497E15F07} = {4E9D03F3-E85D-4796-AADD-A75497E15F07}`. The State project itself uses the same GUID. Verify before changing it.

- Observation: The repository already has an xUnit test project suitable for focused State property unit tests.
  Evidence: `src\Vixen.Tests\Vixen.Tests.csproj` references `xunit.v3`, `Microsoft.NET.Test.Sdk`, and `Moq`, and existing tests live under folders such as `Common`, `Data`, `ExportWizard`, and `FPPClient`.

- Observation: `IElementNode` exposes a stable `Guid Id`, and `ElementNode` receives or creates that identifier when nodes are constructed.
  Evidence: `src\Vixen.Core\Sys\IElementNode.cs` declares `Guid Id { get; }`, and `src\Vixen.Core\Sys\ElementNode.cs` assigns `Id = id` in its constructor.

- Observation: The shared Property-module props disable nullable analysis, while the repository root enables it.
  Evidence: `src\Directory.Build.props` contains `<Nullable>enable</Nullable>`, and `src\Vixen.Modules\Property\Directory.Build.props` overrides it with `<Nullable>disable</Nullable>`. The State project now overrides the shared Property setting with `<Nullable>enable</Nullable>`.

- Observation: The State project builds without warnings after enabling nullable analysis project-wide and correcting the exposed annotations.
  Evidence: `dotnet build src\Vixen.Modules\Property\State\State.csproj --configuration Debug --no-restore` completed with zero warnings and zero errors.

- Observation: The placeholder State setup UI depended on GongSolutions drag/drop only for unimplemented methods.
  Evidence: The old `StateMapperViewModel.DragOver` and `Drop` methods threw `NotImplementedException`. Milestone 3 removed those methods, removed the drag/drop XAML attributes, and removed the unused `gong-wpf-dragdrop` package reference from `State.csproj`.

- Observation: Property module discovery can assign `null` instance data before a profile data set creates or attaches property data.
  Evidence: `src\Vixen.Core\Sys\Modules.cs` initializes `_GetModuleData` with `null` and returns it when a descriptor does not declare `ModuleDataClass`. `StateDescriptor` follows the existing Property pattern and declares `ModuleStaticDataClass`, so `StateModule.ModuleData` must tolerate a missing assignment.

## Decision Log

- Decision: Implement VIX-3591 as a `VixenModules.Property.State` property module, completing the existing partial module rather than creating a new module type.
  Rationale: Display Setup already knows how to add and configure property modules through `IPropertyModuleInstance.HasElementSetupHelper` and `SetupElements`. The Face property uses the same route for element mapping. A property also keeps the state metadata attached to the prop element where users expect to find it.
  Date/Author: 2026-05-29 / Codex

- Decision: Store one State property on the selected parent element, with that property containing the overall state definition and all state rows.
  Rationale: The requirements describe selecting an `IElementNode`, defining an overall name and description for it, and choosing children of that selected node per row. Storing the definition on the selected parent avoids scattering row metadata across child nodes and keeps future State effect lookup simple.
  Date/Author: 2026-05-29 / Codex

- Decision: Persist child element assignments by stable element identity, not by display name.
  Rationale: Element names can be edited and are not guaranteed unique. The implementation should store element identifiers where available and resolve them against the selected element tree when the setup dialog opens.
  Date/Author: 2026-05-29 / Codex

- Decision: Keep xLights import integration as a later milestone after the Display Setup property workflow is working.
  Rationale: The core user workflow must define the data contract first. Import can then map parsed xLights state groups into the same contract without guessing the final shape.
  Date/Author: 2026-05-29 / Codex

- Decision: Materialize imported xLights state definitions as State properties on generated state-group elements.
  Rationale: Each imported `StateInfo` already creates a symbolic state group with one child group per state item. Attaching one State property to the generated state-group element preserves that hierarchy, maps item names and colors into the persisted contract, and stores the generated state-item group IDs as compact assignments.
  Date/Author: 2026-05-30 / Codex

- Decision: Add unit tests by referencing the State property project from `src\Vixen.Tests` and testing UI-free model/helper code, not WPF controls.
  Rationale: The feature has meaningful non-visual behavior: cloning persisted data, preserving or discarding setup drafts, computing effective assignment counts, and applying group-selection semantics. These can be tested deterministically without launching Display Setup or automating WPF.
  Date/Author: 2026-05-29 / Codex

- Decision: Persist State item element assignments as `List<Guid>` values sourced from `IElementNode.Id`.
  Rationale: `IElementNode.Id` is the stable node identifier exposed by the core element tree API. Names can change and are not guaranteed unique, so they remain display-only data.
  Date/Author: 2026-05-30 / Codex

- Decision: Do not include migration logic for the early State property prototype fields.
  Rationale: The State property has never shipped publicly, so no application profiles need compatibility for the prototype `StateName`, `ItemName`, and `ItemColor` fields. Keeping migration code would add complexity without protecting user data.
  Date/Author: 2026-05-30 / Codex

- Decision: Enable nullable analysis for the complete State property project in `State.csproj`.
  Rationale: The repository root enables nullable analysis, but the shared Property-module props disable it. A project-level override applies consistent nullable checking to every State source file and avoids file-by-file directives.
  Date/Author: 2026-05-30 / Codex

- Decision: Count effective assigned leaf nodes while persisting explicitly checked node IDs.
  Rationale: A checked group should remain a compact persisted assignment, while the Count column should describe the leaf elements that a future State effect will operate on. Checking a group clears explicit descendant selections before disabling those descendants. Unchecking the group re-enables descendants without restoring their previous checks.
  Date/Author: 2026-05-30 / Codex

- Decision: Enforce group-selection exclusivity in the assignment tree.
  Rationale: A branch must persist either a selected group or selected descendants, never both. Clearing checked descendants when their parent group is checked keeps assignments unambiguous, while disabling and graying descendants confirms that they cannot be checked until the group is unchecked.
  Date/Author: 2026-05-30 / Codex

- Decision: Require exactly one selected Display Setup node when configuring the State property.
  Rationale: The editor displays one selected element hierarchy and persists one property definition. Returning `false` for zero or multiple nodes avoids silently editing an arbitrary tree.
  Date/Author: 2026-05-30 / Codex

## Outcomes & Retrospective

Milestones 1 through 3 and Milestone 5 are complete. The State property now stores a persistent definition and opens a draft-based setup window with overall name and description fields, editable state rows, color selection, effective assignment counts, and a checkbox element hierarchy. Checking a group clears and disables its descendants, then persists the group assignment. Catel save copies the cloned draft back into the property; cancel discards it. Imported xLights `stateInfo` metadata now materializes as State properties on generated state-group elements. The remaining work is focused xUnit coverage for the persisted contract, draft workflow, assignment tree semantics, and end-to-end validation.

## Context and Orientation

Vixen modules are plugin-like components with descriptors, runtime module instances, and serializable data models. A property module is a module attached to an element in Display Setup. The base class is `src\Vixen.Core\Module\Property\PropertyModuleInstanceBase.cs`. A property can participate in Display Setup by returning `true` from `HasElementSetupHelper` and implementing `SetupElements(IEnumerable<IElementNode> nodes)`.

Display Setup calls property setup from `src\Vixen.Application\Setup\SetupElementsTree.cs`. When the user selects a property and clicks the configure button, `ConfigureSelectedProperties` calls either `property.Setup()` or `property.SetupElements(SelectedElements)`. If the call returns `true`, Display Setup raises an edit event so the profile can be saved.

The closest working precedent is the Face property under `src\Vixen.Modules\Property\Face`. `FaceModule` stores `FaceData`, exposes `HasElementSetupHelper`, and launches `FaceSetupHelper`. `FaceSetupHelper` builds a grid, lets users choose colors through Vixen color picker controls, and writes property data back on OK.

The partial State property already lives under `src\Vixen.Modules\Property\State`. `StateDescriptor` registers the module, `StateModule` exposes a minimal data model, `StateData` serializes one name, one item name, and one color, and `StateMapperView.xaml` provides a WPF setup window. This implementation must be expanded or replaced in place.

Unit tests live in `src\Vixen.Tests`. That project currently references `Vixen.Core`, the FPP client module, and the Export Wizard module. To test the State property, add a project reference to `src\Vixen.Modules\Property\State\State.csproj` with the same conventions already used by the test project. Keep State tests under `src\Vixen.Tests\Property\State`.

Terms used in this plan:

- `IElementNode` means a node in the Display Setup element tree. It may be a group node with children or a leaf node representing an actual light element.
- State definition means the overall named state map attached to a selected parent element, such as `Santa Arm`.
- State item means one row inside the state definition, such as `Arm Up`, with a color and a set of associated element nodes.
- Assignment means the set of child element nodes selected for a state item.

## Plan of Work

Milestone 1 updates Jira before code work starts. Use the markdown below as the Jira ticket body or a comment on `VIX-3591`.

    ## Requirements
    
    Implement a general State property for Display Setup so users can attach a named state definition to a selected prop element. The state definition must include a name, a description, and one or more state rows. Each row must include a state item name, color, and selected child IElementNode assignments. The setup UI must show rows in a grid, allow inline name editing, launch the standard Vixen color chooser from the color cell, show the color cell with the selected color and hex text, show an assignment count, and show a checkbox tree for the selected row. Selecting a group node in the tree must uncheck any selected descendants, gray out those descendants, prevent them from being checked while the group remains selected, and treat them as included by the group selection.
    
    ## High-Level Design
    
    Complete the existing `VixenModules.Property.State` property module. Store one State property on the selected parent element. Replace the current single-name/single-color `StateData` shape with a serializable state definition that includes the overall name, description, and a collection of state item rows. Persist state item assignments by stable element identity rather than display name, and resolve those identifiers against the selected element's descendants when editing. Reuse the existing Display Setup property configuration path through `PropertyModuleInstanceBase.HasElementSetupHelper` and `SetupElements`.
    
    ## Acceptance Criteria
    
    - A user can select an element in Display Setup, add the State property, and configure it.
    - The setup dialog lets the user edit the overall state name and description.
    - The setup dialog lets the user add, edit, and remove multiple state rows.
    - Each row supports inline state item name editing.
    - Clicking the color cell opens the existing Vixen color chooser or discrete color chooser where element colors are constrained.
    - The color cell displays the selected color as the background and the hex value as text.
    - Selecting a row shows a checkbox tree containing the selected element and its children.
    - Checking a group node unchecks any selected descendants, includes all descendants, and disables/grays those descendants.
    - Unchecking a group node re-enables descendants without restoring their previous checks.
    - The count column reflects the number of explicitly selected nodes plus descendants implied by checked group nodes.
    - OK persists the data; Cancel leaves the original property data unchanged.
    - Reopening the property shows the saved name, description, rows, colors, counts, and tree selections.
    - Unit tests cover deep cloning of state data, module clone behavior, save/cancel draft behavior, assignment count calculation, group-selection descendant clearing, and descendant disabling.
    - The solution builds in Debug and Release.
    
    ## Testing Steps
    
    1. Run `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State`.
    2. Build with `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`.
    3. Start Vixen from `Debug\Output`.
    4. In Display Setup, create or select a grouped prop with several child nodes.
    5. Add the State property and configure it.
    6. Enter a state name and description.
    7. Add two rows, set row names, choose different colors, and assign different child nodes.
    8. Check child nodes, then check their parent group. Verify the child checks clear, the descendants become gray, and the descendants count as included.
    9. Uncheck the group and verify its descendants become enabled but remain unchecked.
    10. Save, reopen the State property, and verify all values are restored.
    11. Press Cancel after making changes and verify previously saved values are unchanged.
    12. Build with `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release`.

Milestone 2 defines and implements the persisted data model. This milestone is complete. `src\Vixen.Modules\Property\State\StateData.cs` now serializes the complete state definition, and `src\Vixen.Modules\Property\State\StateItemData.cs` stores each row in a separate one-class-per-file model type. The types live under the State property module rather than the setup UI folder because future effects and import code will need them without referencing setup view models. Public APIs include XML docs as required by `csharp-docs`.

The recommended data shape is:

    public sealed class StateData : ModuleDataModelBase
        public string Name { get; set; }
        public string Description { get; set; }
        public List<StateItemData> Items { get; set; }
        public override IModuleDataModel Clone()
        [OnDeserialized] private void OnDeserialized(StreamingContext context)
    
    public sealed class StateItemData
        public Guid Id { get; set; }
        public string Name { get; set; }
        public System.Drawing.Color Color { get; set; }
        public List<Guid> ElementNodeIds { get; set; }

The stable identifier was verified before implementation: `IElementNode.Id` is a `Guid`, and `ElementNode` stores that identifier. `StateItemData.ElementNodeIds` therefore uses `List<Guid>`.

`src\Vixen.Modules\Property\State\StateModule.cs` now exposes `Name`, `Description`, and `Items` over `_data`, clones the complete collection deeply, initializes defaults, and returns `true` from `SetupElements` only when the dialog is accepted. `StateDescriptor.ModuleId` remains unchanged. No migration logic is required because the prototype State property has never shipped publicly.

Milestone 3 rebuilds the setup view model and UI. This milestone is complete. The WPF setup window remains under `src\Vixen.Modules\Property\State\Setup`. `StateMapperViewModel` receives the selected parent node, persisted `StateData`, and an `IStateColorPickerService`. It clones the persisted data into a draft, exposes editable rows, and copies the draft back only from Catel's `SaveAsync` path. Cancel closes without applying the draft.

The implemented UI models have clear responsibilities:

- `StateMapperViewModel` owns the overall name, description, collection of row view models, selected row, and OK/Cancel/Add/Remove commands.
- `StateItemViewModel` owns one row's name, color, assignment IDs, and computed assignment count.
- `StateAssignmentTreeNode` owns one snapshot node, checked state, enabled state, child tree items, and group selection logic.
- `StateElementNodeSnapshot` isolates tree editing from live `IElementNode` instances and allows unit tests to create hierarchies directly.
- `IStateColorPickerService` isolates the Vixen discrete and general color chooser dialogs from the view model.

The group-selection and count behavior is implemented in UI-free classes. `StateElementNodeSnapshot` contains an assignment ID, name, and child snapshots. `StateAssignmentTreeNode` contains checked/enabled state and computes explicit persisted IDs plus effective assigned leaf IDs. Production code builds snapshots from `IElementNode`; unit tests can build snapshots directly without creating live Vixen element nodes or WPF controls.

The XAML uses a `DataGridTextColumn` for inline item name editing, a colored hex-text template cell that invokes color editing on double-click, and a read-only count column. The tree is row-detail context shown beside the grid for the selected row, not a separate row per element.

For colors, `StateColorPickerService` reuses Vixen's existing controls and behavior. When assigned nodes have valid discrete colors from `VixenModules.Property.Color.ColorModule.getValidColorsForElementNode`, it opens `Common.DiscreteColorPicker.Views.SingleDiscreteColorPickerView`; otherwise it opens `Common.Controls.ColorManagement.ColorPicker.ColorPicker`. The WPF view model depends only on the documented `IStateColorPickerService` interface.

Milestone 4 wires setup behavior into Display Setup correctly. In `StateModule.SetupElements`, validate that exactly one parent element is selected for the first implementation. If multiple elements are selected, either show a short message and return `false`, or support editing only the first selected element after confirming that is an existing Display Setup convention. Prefer one selected element because the requirements say the tree should contain the selected element and its children. Record the final choice in the Decision Log.

When setup opens, ensure the selected element has a State property. If setup was launched from an existing property, edit that property. If invoked through a helper path in the future, add the property to the selected node using `StateDescriptor.ModuleId`. The current Display Setup property configure flow already provides the property instance, so this implementation should not create duplicate properties.

Make `StateMapperView` use Catel/Orchestra conventions already present in the codebase: bindings and commands should carry behavior; code-behind should only initialize the view, set the icon, and handle view-specific setup. Do not use WinForms for new State UI. Remove placeholder drag/drop code or implement it fully; no command or event should throw `NotImplementedException` during normal use.

Milestone 5 integrates xLights and Vixen Prop import after the persisted property workflow is complete. This milestone is complete. The parser already builds `StateInfo` and `StateItem` values in `src\Vixen.Modules\App\CustomPropEditor\Import\XLights`. `PreviewCustomPropBuilder` now materializes those symbolic state definitions as State properties when imported custom props generate Vixen element nodes. Each `StateInfo.Name` maps to a State property `Name`, each `StateItem.Name` and `Color` maps to a `StateItemData`, and each row stores the generated state-item group node ID as its compact assignment.

Milestone 6 validates the feature. Build the solution in Debug and Release from the repository root:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release

The expected outcome is successful builds and updated output in `Debug\Output` and `Release\Output`. If restore fails because of network access in a sandboxed environment, rerun with the required approval outside the sandbox. Automated unit tests are required for the new State work and must pass before the manual Display Setup workflow is accepted.

Milestone 7 adds the unit tests. Add a project reference from `src\Vixen.Tests\Vixen.Tests.csproj` to `..\Vixen.Modules\Property\State\State.csproj`. Add tests under `src\Vixen.Tests\Property\State`. Keep the tests focused on non-UI behavior so they run reliably in the normal test runner.

Create tests with these names or equivalent names that express the same behavior:

- `StateDataCloneTests.Clone_CopiesDefinitionAndItemsDeeply` creates a `StateData` with two items and assignment IDs, clones it, mutates the clone, and asserts the original is unchanged.
- `StateDataCloneTests.OnDeserialized_InitializesMissingCollections` verifies a newly deserialized or manually constructed `StateData` with missing collections is normalized to an empty item list rather than throwing.
- `StateModuleCloneTests.CloneValues_CopiesAllStateItemsAndAssignments` creates a source `StateModule`, clones values into another module, mutates the target, and asserts the source is unchanged.
- `StateAssignmentTreeTests.CheckedLeaf_ReturnsOneEffectiveAssignment` builds a tiny snapshot tree, checks one leaf, and asserts one effective assignment ID and a count of one.
- `StateAssignmentTreeTests.CheckingGroup_ClearsSelectedDescendantsDisablesThemAndCountsEffectiveChildren` selects descendants before checking their parent group, asserts the descendant checks are cleared and disabled or grayed by the model state, and asserts the effective assignment count matches the chosen count rule recorded in the Decision Log.
- `StateAssignmentTreeTests.UncheckingGroup_ReenablesDescendantsWithoutRestoringChecks` checks and then unchecks a group node, asserting descendants become enabled again, remain unchecked, and implied assignment IDs are removed.
- `StateMapperDraftTests.Cancel_DoesNotMutateOriginalStateData` edits the setup draft and cancels or discards it, then asserts the original `StateData` remains unchanged.
- `StateMapperDraftTests.Ok_AppliesDraftToOriginalStateData` edits the setup draft, accepts it, and asserts the module or original data receives the changes.

Run the tests from the repository root:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State

The expected result is that only State-related tests run and all pass. If the test runner does not support the `State` filter because test names or traits differ, run the whole test project:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug

The expected result is all tests pass. If adding the State project reference causes WPF or Windows-only build requirements for the test project, keep the test project Windows-targeted consistently with the solution and document the exact csproj change in this plan.

## Concrete Steps

1. Update Jira `VIX-3591` using the Milestone 1 markdown above. Add a note that the first implementation will complete `VixenModules.Property.State` and defer or separate xLights import property materialization until after the Display Setup workflow is stable.

2. Inspect the element identity API before editing data contracts:

    Working directory: `C:\Dev\Vixen`
    
    Command:
        Get-ChildItem -Path src\Vixen.Core -Recurse -Include *.cs | Select-String -Pattern 'class ElementNode|interface IElementNode|Guid|Id' | Select-Object Path,LineNumber,Line

   Record the selected stable identifier in the Decision Log.

3. Update `src\Vixen.Modules\Property\State\StateData.cs` and add any new persisted data files required under `src\Vixen.Modules\Property\State`. Keep one public class per file. Add XML docs for all public types and members.

4. Update `src\Vixen.Modules\Property\State\StateModule.cs` to initialize, clone, expose, and save the new state definition data.

5. Replace or substantially update `src\Vixen.Modules\Property\State\Setup\Models\StateDefinition.cs`, `Setup\ViewModels\StateMapperViewModel.cs`, `Setup\ViewModels\StateDefinitionViewModel.cs`, and `Setup\Views\StateMapperView.xaml` so they edit a cloned `StateData` draft and satisfy the grid/tree requirements.

6. Add UI-free assignment helper classes under `src\Vixen.Modules\Property\State\Setup\Models` or another State module folder that does not depend on WPF controls. These helpers must compute effective assignment IDs, assignment count, and descendant enabled state when a group node is checked or unchecked.

7. Add a `ProjectReference` to `src\Vixen.Tests\Vixen.Tests.csproj` for `..\Vixen.Modules\Property\State\State.csproj`, then add State tests under `src\Vixen.Tests\Property\State`.

8. Add any required value converters or helper services under the State module only if existing converters in `src\Vixen.Common\WPFCommon` cannot be reused. Prefer existing `ColorToSolidBrushConverter`, `ColorToHexStringConverter`, and `BackgroundColorToTextBrushConverter` already referenced by `StateMapperView.xaml`.

9. Run `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State`. Fix failing State tests before manual validation.

10. Build Debug. Fix compile errors in the State module and any project reference issues. If `Vixen.sln` self dependency for State causes build or IDE issues, remove only the verified erroneous dependency and document the change.

11. Run the manual Display Setup acceptance workflow. Capture notes in `Outcomes & Retrospective`.

12. Build Release. If accepted for this issue, implement xLights import property creation and repeat State unit tests plus Debug and Release builds.

## Validation and Acceptance

Acceptance must be demonstrated through both build output and a manual scenario.

Build validation:

    Working directory: `C:\Dev\Vixen`
    Command:
        dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State
    Expected result:
        All State property unit tests pass.

    Working directory: `C:\Dev\Vixen`
    Command:
        msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    Expected result:
        Build succeeds with no State module compile errors.

    Working directory: `C:\Dev\Vixen`
    Command:
        msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
    Expected result:
        Build succeeds and `Release\Output` is updated.

Manual validation:

Start Vixen from the Debug output. In Display Setup, create or select a parent prop with child nodes. Add the State property to the parent if it is not already present. Configure the State property. Enter an overall name such as `Santa Arm` and a description such as `Controls the waving arm positions`. Add rows named `Arm Up` and `Arm Down`. Assign different colors. Select each row and check different child elements in the tree. Check child nodes, then check their parent group and confirm the child checks clear, descendants are grayed out, and descendants are counted. Uncheck the group and confirm descendants become enabled but remain unchecked. Press OK. Reopen the property and confirm all values are still present. Make a visible change, press Cancel, reopen again, and confirm the canceled change was not persisted.

Import an xModel containing `stateInfo`. The imported prop should preserve its custom prop model state definitions and create State property data on each generated state-group element. Open the State property in Display Setup and verify the imported definition name, item names, colors, and state-item group assignments. The importer should not lose existing face info behavior.

Unit-test acceptance:

The State unit tests must prove that cloning is deep, missing serialized collections are normalized, module clone behavior preserves all state items and assignments, group selection disables descendants, assignment counts update when nodes are checked or unchecked, OK applies a setup draft, and Cancel discards a setup draft. These tests should not instantiate `StateMapperView` or any WPF controls. If a test must use a view model, it should exercise the view model as a plain object through commands or explicit methods and avoid `ShowDialog`.

## Idempotence and Recovery

The implementation is additive within the existing State module. Running tests and builds multiple times is safe. Manual Display Setup validation should be performed in a disposable or test profile so failed attempts do not corrupt a user profile. Keep `StateDescriptor.ModuleId` unchanged so the module identity remains stable as implementation continues.

If a setup dialog change fails at runtime, revert only the State module changes made for this feature. Do not revert unrelated working tree changes. If project or solution changes are required, inspect `Vixen.sln` around the State project and only remove verified spurious entries.

## Artifacts and Notes

Relevant files already inspected:

    src\Vixen.Modules\Property\Face\FaceModule.cs
    src\Vixen.Modules\Property\Face\FaceData.cs
    src\Vixen.Modules\Property\Face\FaceSetupHelper.cs
    src\Vixen.Modules\Property\State\StateDescriptor.cs
    src\Vixen.Modules\Property\State\StateModule.cs
    src\Vixen.Modules\Property\State\StateData.cs
    src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs
    src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml
    src\Vixen.Application\Setup\SetupElementsTree.cs
    src\Vixen.Modules\App\CustomPropEditor\Import\XLights\XModelImport.cs
    src\Vixen.Modules\App\CustomPropEditor\Import\XLights\StateInfo.cs
    src\Vixen.Modules\App\CustomPropEditor\Model\StateDefinition.cs
    src\Vixen.Tests\Vixen.Tests.csproj

Important existing behavior:

    SetupElementsTree.ConfigureSelectedProperties calls property.SetupElements(SelectedElements) when HasElementSetupHelper is true.
    FaceSetupHelper demonstrates validating a chosen color against Color property constraints.
    StateMapperView.xaml already references WPF color converters from WPFCommon.
    Vixen.Tests uses xUnit v3 and can host State property tests once it references the State project.

## Interfaces and Dependencies

Keep the module namespace `VixenModules.Property.State`. Keep `StateDescriptor.ModuleId` as `{35B893C3-D10C-4359-82BC-929C0762E809}`. Continue using `PropertyModuleDescriptorBase`, `PropertyModuleInstanceBase`, and `ModuleDataModelBase`.

Required persisted API at completion:

    StateData.Name
    StateData.Description
    StateData.Items
    StateItemData.Id
    StateItemData.Name
    StateItemData.Color
    StateItemData.ElementNodeIds or equivalent stable assignment IDs

Required runtime behavior in `StateModule`:

    SetDefaultValues initializes a valid empty state definition.
    CloneValues performs a deep copy of all state items and assignment IDs.
    ModuleData accepts and returns the new StateData contract.
    SetupElements opens the setup dialog, persists only on OK, and returns false on Cancel.

Required UI behavior:

    Overall name and description fields bind two-way to the draft state definition.
    DataGrid rows bind two-way to state item names and colors.
    Color cells open Vixen color selection and display hex text over the selected color.
    Count cells are read-only and update when tree selections change.
    Tree nodes have checkboxes; checking a group clears selected descendants, disables them, and counts them as included. Unchecking the group re-enables descendants without restoring their previous checks.

Required unit-test behavior:

    State persisted data clone tests prove deep-copy behavior.
    State module clone tests prove Display Setup clone behavior copies all rows and assignments.
    Assignment tree tests prove group selection, descendant clearing and disabling, unchecking without restoring checks, and count calculations.
    Draft apply/cancel tests prove the setup workflow does not mutate module data until OK.

No new NuGet package should be added for the first implementation. Existing dependencies already include Catel, GongSolutions.Wpf.DragDrop, WPFCommon, Controls, DiscreteColorPicker, Resources, Vixen.Core, and the Color property module. Remove `gong-wpf-dragdrop` only if drag/drop is not used anywhere in the final State UI and no other State code references it.

## Risks and Open Questions

The first risk is element identity. The implementation uses the verified stable `IElementNode.Id` value for serialization. Storing names is not acceptable except as display-only data.

The second risk is group selection semantics. The requirement says that if a group node is selected, explicitly checked descendants are cleared, descendants are grayed out, and descendants are assumed associated. The count must be carefully defined as either explicit selected nodes or effective leaf nodes. This plan recommends effective associated nodes so the count matches what the State effect will eventually operate on.

The third risk is WPF/WinForms interop. The State setup view is WPF, while some existing color pickers are WinForms. Reuse the existing controls through a small service or helper to keep the view model testable and avoid embedding dialog-specific logic in row models.

The fourth risk is scope creep into the future State effect. This plan creates the property data and editor only. Do not implement sequencing or rendering behavior here unless `VIX-3591` is explicitly expanded.

The fifth risk is accidentally making tests depend on WPF UI plumbing. Keep the tested logic in plain model/helper classes and test those directly. Manual testing remains responsible for visual layout and actual dialog behavior.

Open questions to resolve before or during Milestone 2:

- Should the tree allow checking the selected parent element itself, or only descendants?
- Should duplicate state item names be blocked within one state definition?

## Revision Notes

2026-05-29: Initial plan created from `docs\vix-3591-state-feature.md`, `.agents\PLANS.md`, the project skill instructions, and repository research into Face property, Display Setup property configuration, existing State property code, color picker controls, and xLights state import.

2026-05-29: Updated plan to require minimal xUnit coverage for the new State property behavior. Added a test milestone, test project reference guidance, concrete test case names, and validation commands.

2026-05-30: Marked Milestone 1 complete and implemented Milestone 2. Recorded the verified `IElementNode.Id` assignment identity, the new `StateItemData` contract, and the successful State project Debug build.

2026-05-30: Removed prototype State data migration logic because the feature has never shipped publicly. Enabled nullable analysis for the complete State project in `State.csproj` instead of using a file-level directive. Corrected the nullable annotations exposed by that change and verified a zero-warning State project build.

2026-05-30: Completed Milestone 3. Replaced the placeholder State mapper with a draft-based Catel view model, editable grid, checkbox assignment tree, effective leaf count, and isolated color chooser service. Removed unused drag/drop scaffolding and verified a zero-warning State project Debug build.

2026-05-30: Added group-selection exclusivity. Checking a group now clears explicit descendant selections before disabling descendants, and unchecking the group re-enables descendants without restoring their previous checks.

2026-05-30: Fixed State module discovery initialization. `StateModule.ModuleData` now creates default `StateData` when the loader assigns a missing instance data value, preventing a null dereference during module loading.

2026-05-30: Centered the WPF State mapper over the WinForms Display Setup window by assigning the active WinForms handle as the WPF owner and setting `WindowStartupLocation` to `CenterOwner`.

2026-05-30: Completed Milestone 5. Imported xLights `stateInfo` metadata now materializes as State properties on generated state-group elements, with state-item child groups stored as compact assignment IDs.

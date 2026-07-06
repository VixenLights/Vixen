# Implement VIX-2973 Layer Import, Export, And Quick Rename

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. The source requirements are in `docs/sequencer/vix-2973-import-export-layers.md`; this ExecPlan repeats the necessary context so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users build sequences with layers that control how effects combine when multiple effects overlap. Today, a user who creates a useful layer setup in one timed sequence must recreate that setup by hand in the next sequence. After this work, a user can export all non-default layers to a readable `.v3l` JSON file, import that file into another sequence, and see the same layer names, ordering, mixing filter types, and mixing filter configuration. The Layer Editor will also gain a compact quick rename image button that renames a layer to the display name of its selected mixing filter type, which saves typing after the user changes a layer's filter.

The visible result is in the Timed Sequence Editor's Layer Editor. With only the default layer present, Export Layers is disabled. After the user adds standard layers, Export Layers becomes enabled. Export creates a `.v3l` file. Import adds valid layers above the existing layers without asking for confirmation when every layer is valid. If some layers cannot be imported because a mixing filter is missing or configuration cannot be restored, Vixen warns the user and lets them either cancel the whole import or continue with only the valid layers.

## Progress

- [x] (2026-07-06 17:00Z) Created this initial ExecPlan from `docs/sequencer/vix-2973-import-export-layers.md` and repository research.
- [x] (2026-07-06 18:15Z) Researched module-data JSON round-tripping and locked the serializer approach in the Decision Log. Added focused tests in `src/Vixen.Tests/Sequencer/LayerMixingFilterDataJsonSerializationTests.cs`; the targeted test command passed with 2 tests.
- [ ] Refactor the Layer Editor to Catel MVVM while preserving current behavior.
- [ ] Add testable layer operation services and focused unit tests for existing layer behavior.
- [ ] Add the quick rename PNG resource, image-only button, command, and tests.
- [ ] Add `.v3l` DTOs and import/export serialization services with tests.
- [ ] Add Catel file dialog and message-service command flows with tests.
- [ ] Manually verify the Layer Editor in the Timed Sequence Editor host.
- [ ] Update VIX-2973 in JIRA with the final refined specification, acceptance criteria, and test guidance.
- [ ] Record final outcomes and any remaining follow-up work.

## Surprises & Discoveries

- Observation: The current Layer Editor is a WPF `UserControl`, but it is not Catel MVVM. It sets `DataContext = this`, registers routed commands in the view constructor, owns drag/drop advisors, mutates `SequenceLayers` directly, and resolves layer mixing filters through global services.
  Evidence: `src/Vixen.Modules/Editor/LayerEditor/LayerEditorView.cs` contains the constructor, command handlers, drag/drop logic, and `InitializeLayers`.

- Observation: The current sequence persistence path already treats layer mixing filters specially. It stores layer/filter relationships through `LayerMixingFilterSurrogate` and stores layer mixing filter module data in `_layerMixingFilterDataModels`.
  Evidence: `src/Vixen.Core/Module/SequenceType/SequenceTypeDataModelBase.cs` builds `_layerMixingFilterSurrogates` and `_layerMixingFilterDataModels` during `SurrogateWrite`, then recreates filters and assigns module data during `SurrogateRead`.

- Observation: Shared image resources live in `src/Vixen.Common/Resources`, and WPF views reference them with pack URIs such as `/Resources;component/folder_go.png`.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Views/MarkDockerView.xaml` uses `/Resources;component/folder_go.png` and `/Resources;component/folder_open.png`; `src/Vixen.Common/Resources/Resources.csproj` marks many PNGs as `Resource`.

- Observation: `DataContractJsonSerializer` can round-trip all three current layer mixing filter module data models as JSON, including `ChromaKeyData` with `System.Drawing.Color`, `LumaKeyData`, and `MaskAndFillData`.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerMixingFilterDataJsonSerializationTests --no-restore` passed on 2026-07-06 with `Passed: 2, Failed: 0`.

- Observation: `DataContractJsonSerializer` emits compact JSON by default, but the compact JSON can be parsed with `System.Text.Json.JsonDocument` and written back with `WriteIndented = true`; the indented JSON still deserializes through `DataContractJsonSerializer`.
  Evidence: `LayerMixingFilterDataJsonSerializationTests.DataContractJsonSerializer_RestoresIndentedJson` asserts the JSON contains line breaks and restores the expected `LumaKeyData` values.

## Decision Log

- Decision: Refactor the Layer Editor into a Catel view and view model before adding import/export and quick rename behavior.
  Rationale: The source specification requires the new behavior after a Catel MVVM refactor. It also makes commands and dialogs testable without driving WPF controls.
  Date/Author: 2026-07-06 / Codex

- Decision: Keep the existing WinForms `DockContent` host and WPF `ElementHost`.
  Rationale: The surrounding Timed Sequence Editor is still WinForms-hosted. This feature should change the hosted WPF control, not the docking shell.
  Date/Author: 2026-07-06 / Codex

- Decision: Export all non-default layers and disable export when only the default layer exists.
  Rationale: The first version does not need an export selection dialog, and the user explicitly does not want empty `.v3l` files created through the UI.
  Date/Author: 2026-07-06 / Codex

- Decision: Import valid files without a summary confirmation when every layer is importable.
  Rationale: The successful path should be fast and not interrupt the user. Only errors, warnings, or partial-import flows need user messaging.
  Date/Author: 2026-07-06 / Codex

- Decision: Add the quick rename button as an image-only command beside each standard layer name text box, after the Catel refactor.
  Rationale: The Layer Editor is compact, and the user wants a small control to rename a layer to the selected filter type display name.
  Date/Author: 2026-07-06 / Codex

- Decision: Preserve mixing filter configuration data in the `.v3l` file.
  Rationale: Exporting only the filter type would recreate the layer shell but lose setup data for filters such as keyed or configured filters, which does not satisfy the user-visible import/export promise.
  Date/Author: 2026-07-06 / Codex

- Decision: Use `DataContractJsonSerializer` for per-layer `IModuleDataModel` JSON, then prettify its output with `System.Text.Json` before embedding it in the `.v3l` document.
  Rationale: Vixen sequence persistence already uses data-contract serialization for module data, and the milestone-1 tests proved this serializer round-trips every current layer mixing filter data model. Prettifying through `System.Text.Json` satisfies the human-readable internal exchange format requirement without changing the data-contract semantics used by existing module data.
  Date/Author: 2026-07-06 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. The implementation now has focused proof tests showing that the current layer mixing filter module data can be serialized to readable JSON and restored without losing configured values. The next milestone can build the actual import/export service on `DataContractJsonSerializer` for filter `ModuleData`, while still using a versioned top-level `.v3l` JSON document.

## Context and Orientation

Vixen is a Windows desktop application built on .NET 10 and WPF, with many UI areas still hosted by WinForms shells. The current Layer Editor module is under `src/Vixen.Modules/Editor/LayerEditor`. The Timed Sequence Editor hosts this module in `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs`.

A sequence layer is a row-like concept that tells Vixen how to combine effects that overlap in time. The default layer always exists and is the fallback layer. Standard layers are user-created layers above the default layer. A layer mixing filter is a module instance attached to a standard layer that combines colors or intensities between layers. Examples are under `src/Vixen.Modules/LayerMixingFilter`.

The current WPF control is `VixenModules.Editor.LayerEditor.LayerEditorView` in `src/Vixen.Modules/Editor/LayerEditor/LayerEditorView.cs`. It is a `System.Windows.Controls.UserControl`, not a Catel control. It creates routed command bindings for Add Layer, Remove Layer, and Configure Layer. It sets `DataContext = this`, exposes `Layers` and `FilterTypes` directly, and handles drag/drop through `Common.WPFCommon.Input.IDragSourceAdvisor` and `IDropTargetAdvisor`.

The current control template is in `src/Vixen.Modules/Editor/LayerEditor/Themes/Generic.xaml`. It renders Add Layer and Remove Layer text buttons, a `ListBox` named `_lbLayers`, a `TextBox` bound to `LayerName`, a `ComboBox` bound to `FilterTypeId`, and a Configuration button for filters that support setup.

Layer storage is in `Vixen.Sys.LayerMixing.SequenceLayers`, implemented in `src/Vixen.Core/Sys/LayerMixing/SequenceLayers.cs`. The important methods are `AddLayer(ILayer layer)`, `AddLayer(ILayerMixingFilterInstance mixer)`, `InsertLayer(int index, ILayer layer)`, `RemoveLayerAt(int index)`, `MoveLayer(int indexFrom, int indexTo)`, `IndexOfLayer(ILayer item)`, `GetLayer(string name, Guid typeId)`, and `ContainsLayer(Guid layerId)`. `AddLayer(ILayer layer)` inserts at index `0`, which means repeated imports must process the source layers in reverse order if the final imported group should keep the exported top-to-bottom order above the existing layers.

Layer model types are in `src/Vixen.Core/Sys/LayerMixing/Layer.cs`, `DefaultLayer.cs`, `StandardLayer.cs`, and `ILayer.cs`. `Layer.FilterTypeId` gets and sets the attached `ILayerMixingFilterInstance` through `LayerMixingFilterService.Instance.GetInstance(value)`. `Layer.FilterName` returns `LayerMixingFilter.Descriptor.TypeName` or `None`.

Module instances implement `Vixen.Module.IModuleInstance`, which exposes `InstanceId`, `ModuleData`, `StaticModuleData`, and `Descriptor`. `LayerMixingFilterModuleInstanceBase` is the base class for layer mixing filter modules. Sequence save/load already uses `LayerMixingFilterSurrogate` in `src/Vixen.Core/Module/SequenceType/Surrogate/LayerMixingFilterSurrogate.cs` to record a filter type ID and instance ID, and `SequenceTypeDataModelBase` stores filter `IModuleDataModel` values in `_layerMixingFilterDataModels`. This is the repository precedent for preserving filter configuration.

The Mark Collection toolbar is the visual reference for import/export buttons. It is in `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Views/MarkDockerView.xaml`, where Import uses `/Resources;component/folder_go.png` and Export uses `/Resources;component/folder_open.png`.

Shared PNG resources are in `src/Vixen.Common/Resources`; the project file is `src/Vixen.Common/Resources/Resources.csproj`. New WPF-consumable PNG assets should be added there as `<Resource Include="...png" />` unless an existing convention in that file shows a better match.

Catel file-dialog examples exist in `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` and `src/Vixen.Modules/App/TimedSequenceMapper/SequenceElementMapper/ViewModels/ElementMapperViewModel.cs`. They resolve `IOpenFileService` or `ISaveFileService`, create a `DetermineOpenFileContext` or `DetermineSaveFileContext`, and await `DetermineFileAsync`.

The project-specific skills that apply are `catel-mvvm`, `dotnet-best-practices`, `csharp-async`, `csharp-docs`, and `dotnet-design-pattern-review`. The important constraints are: Catel view models inherit from `Catel.MVVM.ViewModelBase`; commands use Catel `Command` or `TaskCommand`; WPF views should use bindings and behaviors rather than code-behind event handlers; view models must use services for dialogs rather than `MessageBox` or direct UI controls; async methods return `Task` or `Task<T>` and use the `Async` suffix; and public or protected C# APIs need XML documentation.

## Plan of Work

Start by researching module-data JSON round-tripping. Create a small spike in tests or a temporary local branch change, then remove any throwaway code before completing the milestone. The goal is to prove how to serialize and restore `ILayerMixingFilterInstance.ModuleData` for at least one filter with real data. The expected approach is to create a new layer mixing filter instance from the exported `filterTypeId`, serialize the source instance `ModuleData` using a serializer compatible with Vixen's data model attributes, store it as human-readable JSON under `filterData`, and restore it into a fresh imported instance without reusing the exported instance ID. If DataContract JSON serialization works for real filter data, record that decision and implement it. If it cannot handle existing module data cleanly, record the failure and choose the closest repository-native serializer that still produces readable JSON.

Refactor the Layer Editor next. Replace the legacy control-template-only `LayerEditorView` with a Catel view and a Catel view model. Keep the public construction path usable by `TimedSequenceEditor.LayerEditor`, but move layer mutations and command state out of the view. The view model should expose the layers, available filter types, selected layer, and Catel commands. The view should bind to those properties and commands. Keep drag/drop hit testing in view-specific code or an attached behavior because it is WPF-specific, but route the actual move operation through a view model command or service method.

Add a layer operations service in the LayerEditor module. This service should be testable without WPF and should own operations such as adding a layer, removing a layer, moving a layer, configuring a filter, generating unique names, quick-renaming a layer, and importing valid layers in the right order. The service can wrap `SequenceLayers`, but do not hide `SequenceLayers` semantics from tests. If existing `SequenceLayers.EnsureUniqueName` remains private, either expose a narrowly named public/internal helper with XML docs or implement an equivalent helper in the new service and cover it with tests.

Add the quick rename icon and command after the Catel refactor. Create or add a representative PNG under `src/Vixen.Common/Resources`, update `Resources.csproj` so the image is available as a WPF resource, add a project reference from `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj` to `src/Vixen.Common/Resources/Resources.csproj` if needed, and reference it from XAML with a pack URI like `/Resources;component/<icon-name>.png`. The quick rename button must be image-only, next to the layer name text box, hidden or disabled for the default layer, and command-bound.

Add `.v3l` DTOs and serialization services. Keep one class or record per file. Use a versioned top-level DTO with `format`, `version`, `exportedUtc`, and `layers`. Each layer DTO must include `name`, `order`, `filterTypeId`, `filterName`, `filterDataType`, and `filterData`. The format literal is `Vixen3Layers`, and version `1` is the first supported version. The file is internal but human-readable; write indented JSON.

Add import validation and result types. The service should parse and validate the file before mutating `SequenceLayers`. It must ignore default layer records if a hand-edited future file contains them. It must classify each standard layer as importable or skipped. Missing filter types and unrestorable module data are skipped-layer conditions. Invalid JSON, unsupported format, unsupported version, and missing required fields are file-level failures and must leave the sequence unchanged.

Add Catel command flows in the view model. Export uses `ISaveFileService` with filter `Vixen 3 Layers (*.v3l)|*.v3l|All Files (*.*)|*.*`, default extension `.v3l`, and a title such as `Export Layers`. Export is disabled when there are no non-default layers and must not show the save-file dialog in that state. Import uses `IOpenFileService` with the same filter and a title such as `Import Layers`. If all layers are valid and importable, import immediately with no summary dialog. If some layers are skipped, use `IMessageService` or the project-local Catel message pattern to ask whether to continue. If the user cancels, import nothing. If all layers are invalid or the file is invalid, show a warning or error and import nothing.

Add tests in `src/Vixen.Tests`. Prefer tests against pure services and DTO serializers so the test suite does not need to launch WPF. If referencing the LayerEditor project from `Vixen.Tests` is necessary, add a project reference to `src/Vixen.Tests/Vixen.Tests.csproj`. If that creates WPF target-framework issues, move the pure operation and serialization services to a project already testable from `Vixen.Tests`, such as `Vixen.Core`, and keep only the Catel view/view model in the LayerEditor module. Record that decision in the Decision Log.

Update documentation and JIRA. After implementation behavior is locked, update VIX-2973 in Jira with the final refined requirements, acceptance criteria, and testing guidance. Also update this ExecPlan's living sections as implementation proceeds.

## Concrete Steps

Work from the repository root:

    cd C:\Dev\Vixen

Before editing, inspect the relevant files:

    Get-Content -LiteralPath docs\sequencer\vix-2973-import-export-layers.md
    Get-Content -LiteralPath .agents\PLANS.md
    Get-Content -LiteralPath src\Vixen.Modules\Editor\LayerEditor\LayerEditorView.cs
    Get-Content -LiteralPath src\Vixen.Modules\Editor\LayerEditor\Themes\Generic.xaml
    Get-Content -LiteralPath src\Vixen.Modules\Editor\TimedSequenceEditor\LayerEditor.cs
    Get-Content -LiteralPath src\Vixen.Core\Sys\LayerMixing\SequenceLayers.cs
    Get-Content -LiteralPath src\Vixen.Core\Module\SequenceType\SequenceTypeDataModelBase.cs
    Get-Content -LiteralPath src\Vixen.Core\Module\SequenceType\Surrogate\LayerMixingFilterSurrogate.cs
    Get-Content -LiteralPath src\Vixen.Common\Resources\Resources.csproj

Research available layer mixing filters and choose at least one configured filter for the module-data round-trip proof:

    rg -n "class .*Data|ModuleData|HasSetup|Setup" src\Vixen.Modules\LayerMixingFilter -g "*.cs"

Milestone 1, module-data serialization proof: create focused test coverage that proves the selected module-data serialization approach can round-trip a real `IModuleDataModel` into human-readable JSON and restore it into a new filter instance. Keep any temporary spike files out of the final change. At the end of this milestone, update `Decision Log` with the serializer decision and update `Surprises & Discoveries` with evidence from test output or failed attempts.

Milestone 1 result: this milestone added `src/Vixen.Tests/Sequencer/LayerMixingFilterDataJsonSerializationTests.cs` and project references from `src/Vixen.Tests/Vixen.Tests.csproj` to the three existing layer mixing filter projects. The tests prove `DataContractJsonSerializer` round-trips `ChromaKeyData`, `LumaKeyData`, and `MaskAndFillData`, and prove that indented JSON produced by `System.Text.Json` remains readable by `DataContractJsonSerializer`. Do not remove these tests when implementing the import/export service; extend them or replace their helper methods with calls into the production serializer once that serializer exists.

Milestone 2, Catel refactor: add a Catel view model under `src/Vixen.Modules/Editor/LayerEditor/ViewModels`, for example `LayerEditorViewModel.cs`. Add any row view models only if they remove real binding complexity; avoid creating view models that merely mirror `ILayer` property names. Convert the view to Catel's `UserControl` pattern, either by replacing `LayerEditorView.cs` plus `Generic.xaml` with a XAML-backed Catel view or by creating a new Catel view and adapting the host. Keep `TimedSequenceEditor.LayerEditor` able to construct and host the control with a `SequenceLayers` instance. The host must still forward collection and layer change notifications into its `LayersChanged` event.

Milestone 3, layer operations: add a testable service under `src/Vixen.Modules/Editor/LayerEditor/Services`, for example `LayerEditorLayerService.cs` and `ILayerEditorLayerService.cs`. It should expose operations equivalent to:

    AddLayer(SequenceLayers layers, Guid filterTypeId)
    RemoveLayer(SequenceLayers layers, ILayer layer)
    MoveLayer(SequenceLayers layers, ILayer layer, int newIndex)
    ConfigureLayer(ILayer layer)
    QuickRenameLayer(SequenceLayers layers, ILayer layer)
    CreateUniqueLayerName(IEnumerable<ILayer> layers, string desiredName)
    HasExportableLayers(SequenceLayers layers)

Use exact names that fit the final design, but preserve these behaviors. If the service or methods are public or protected, add XML documentation in the same change. If they are internal but complex, add concise XML or comments where they help.

Milestone 4, quick rename UI: create the icon under `src/Vixen.Common/Resources`, for example `layer_rename.png`, and include it as a WPF `Resource` in `Resources.csproj`. Add an image-only button beside the name text box in the standard layer editor row. Give it a tooltip such as `Rename to filter type`. Bind it to the quick rename command. The command's target name is `layer.LayerMixingFilter.Descriptor.TypeName`; if that name already exists, use `Name - 2`, `Name - 3`, and so on. The command must be unavailable for the default layer and for a standard layer without a mixing filter.

Milestone 5, `.v3l` DTOs and service: add one DTO per file, likely under `src/Vixen.Modules/Editor/LayerEditor/ImportExport` or `Services`. Suggested DTO names are `LayerExportDocument`, `LayerExportRecord`, `LayerImportResult`, and `LayerImportSkippedRecord`. Add a service such as `LayerImportExportService` and an interface such as `ILayerImportExportService`. Keep file I/O asynchronous with `Task`-returning methods when reading or writing files. Suggested operations are:

    Task ExportAsync(SequenceLayers layers, string filePath, CancellationToken cancellationToken = default)
    Task<LayerImportPlan> ReadImportPlanAsync(string filePath, CancellationToken cancellationToken = default)
    LayerImportResult Import(SequenceLayers layers, LayerImportPlan plan)

The exact method names can change if the final design is clearer, but the service must separate validation from mutation so the view model can ask for confirmation before partial imports.

Milestone 6, Catel import/export command flow: add `TaskCommand` properties to the Layer Editor view model, for example `ImportLayersCommand` and `ExportLayersCommand`. Use `CanExportLayers` to disable Export when only the default layer exists. Use `IOpenFileService`, `ISaveFileService`, and `IMessageService` through constructor injection where practical. If the existing host cannot easily construct the view model with services, isolate Catel dependency resolution at the view or host boundary and record the reason in the Decision Log. Do not use WinForms dialogs or WPF `MessageBox` in the view model.

Milestone 7, tests: create focused tests in `src/Vixen.Tests`, preferably under `src/Vixen.Tests\Sequencer` or `src/Vixen.Tests\LayerEditor`. Cover:

- Add layer creates a standard layer above existing layers and keeps the default layer at the bottom.
- Remove layer refuses or cannot remove the default layer.
- Reorder layer cannot move the default layer.
- Configure layer reports a layer change when setup succeeds.
- Quick rename changes a standard layer's name to the selected filter type display name.
- Quick rename generates `Name - 2`, `Name - 3`, and so on when needed.
- Quick rename is unavailable for the default layer.
- Export excludes the default layer.
- Export preserves top-to-bottom layer order.
- Export includes name, filter type ID, filter name, and mixing filter module data.
- Export command is disabled when only the default layer exists.
- Import adds layers above existing layers in exported relative order.
- Import gives imported layers new IDs.
- Import renames duplicates.
- Import restores filter type and module data.
- Import does not ask for confirmation when every layer is valid.
- Import ignores default layer records.
- Import skips missing filter types only after confirmation.
- Import cancels without mutation when the user declines a partial import.
- Invalid JSON, unsupported format, unsupported version, and missing required fields leave the sequence unchanged.

Run the targeted tests first:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditor --no-restore

Then run the full test project if targeted tests pass:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If `--no-restore` fails because dependencies have not been restored in the local environment, rerun without `--no-restore`:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

Milestone 8, build and manual verification: build the affected solution or project set. At minimum, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditor --no-restore
    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug

If the project build is not enough because resources or host integration fail only at solution scope, run:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Start Vixen through the usual local workflow for this repository, open a timed sequence, open the Layer Editor, and verify the manual acceptance scenario in the next section. Record the result in `Outcomes & Retrospective`.

Milestone 9, JIRA and final plan updates: update VIX-2973 in Jira with the implemented behavior, acceptance criteria, and test commands. Then update `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` in this ExecPlan. If any requirements changed during implementation, also update `docs/sequencer/vix-2973-import-export-layers.md`.

## Validation and Acceptance

Automated validation must include focused unit tests and a build. The preferred targeted test command is:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditor --no-restore

Expected successful output should include a test run summary with zero failed tests. The exact count will depend on the final test names, but the new tests should fail before the implementation and pass after it.

Run the full test project when the targeted tests pass:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Build the affected WPF module:

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug

Manual acceptance must demonstrate the behavior in the actual Timed Sequence Editor host:

1. Open Vixen and open or create a timed sequence.
2. Open the Layer Editor.
3. Confirm Export Layers is disabled when only the default layer exists.
4. Add at least two standard layers and confirm Export Layers becomes enabled.
5. Change one layer's mixing filter type, click the image-only quick rename button beside the name text box, and confirm the name changes to the selected filter type display name.
6. Repeat quick rename when another layer already has that name and confirm Vixen applies the next ` - 2` suffix.
7. Configure a layer mixing filter that supports setup.
8. Click Export Layers, choose a `.v3l` path, and save.
9. Open the `.v3l` file in a text editor and confirm it is indented JSON with `format`, `version`, `exportedUtc`, and `layers`, and that it does not contain the default layer.
10. Open or create another timed sequence.
11. Open the Layer Editor and import the `.v3l` file.
12. Confirm no summary confirmation appears when all layers are importable.
13. Confirm imported layers appear above existing layers, in the same relative order they were exported.
14. Confirm the default layer appears exactly once at the bottom.
15. Confirm imported layer names, filter types, and filter configuration match the exported source except where duplicate names required suffixes.

Manual partial-import acceptance must demonstrate warning behavior:

1. Copy a valid `.v3l` file and hand-edit one layer record to use an unknown `filterTypeId`.
2. Import the edited file.
3. Confirm Vixen shows a warning or confirmation that describes skipped layers.
4. Choose cancel and confirm no layers were imported.
5. Repeat import, choose continue, and confirm valid layers were imported while missing-filter layers were skipped.

## Idempotence and Recovery

Most implementation steps are additive and safe to repeat. Running tests and builds can be repeated at any time. Import/export service tests should write temporary files under the test framework's temporary directory and clean them up automatically.

Do not manually edit sequence files during automated tests. For manual testing, use disposable sequences or backup the sequence first. Import should validate before mutation, so invalid files should not change the current sequence. If manual import produces unexpected layers, remove those layers through the Layer Editor or close the sequence without saving.

When adding a project reference or resource entry, inspect the diff carefully. The solution has explicit conventions for new projects, but this plan does not add a new project. If implementation unexpectedly requires a new project, follow the repository `AGENTS.md` solution-folder and platform-entry rules.

If adding a reference from `Vixen.Tests` to `LayerEditor` causes target-framework or WPF build problems, do not force WPF into the test project. Move pure services and DTOs to a project already suitable for unit tests, record the decision, and keep the UI module as a thin Catel layer over those services.

If module-data JSON round-tripping cannot be completed for all existing layer mixing filter data models, stop before implementing a partial file format. Record the failing data type and error in `Surprises & Discoveries`, then choose either a supported repository-native serializer or ask for a requirement adjustment. The feature must not silently drop filter setup data.

## Artifacts and Notes

The `.v3l` file must be shaped like this, with indented JSON:

    {
    	"format": "Vixen3Layers",
    	"version": 1,
    	"exportedUtc": "2026-07-06T16:30:00Z",
    	"layers": [
    		{
    			"name": "Mask and Fill",
    			"order": 0,
    			"filterTypeId": "00000000-0000-0000-0000-000000000000",
    			"filterName": "Mask and Fill",
    			"filterDataType": "VixenModules.LayerMixingFilter.MaskFill.MaskAndFillData, MaskFill",
    			"filterData": {
    				"...": "serializer-specific module data"
    			}
    		}
    	]
    }

Layer order example:

    Before import:
    Existing A
    Existing B
    Default

    Export file order:
    Imported 1
    Imported 2

    After import:
    Imported 1
    Imported 2
    Existing A
    Existing B
    Default

Because `SequenceLayers.AddLayer` inserts at index `0`, adding `Imported 1` and then `Imported 2` would produce the wrong order. Either import from bottom to top or use a `SequenceLayers` insertion operation that preserves the final order and still recalculates levels.

Catel file service example pattern from existing code:

    var saveFileService = dependencyResolver.Resolve<ISaveFileService>();
    var determineFileContext = new DetermineSaveFileContext
    {
    	Filter = "Vixen 3 Layers (*.v3l)|*.v3l|All Files (*.*)|*.*",
    	Title = "Export Layers",
    	CheckPathExists = true
    };
    var result = await saveFileService.DetermineFileAsync(determineFileContext);

Prefer constructor injection over resolving services inside methods. Use the existing resolver pattern only at a composition boundary if the host cannot supply services directly.

Milestone 1 test evidence:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerMixingFilterDataJsonSerializationTests --no-restore
    Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 102 ms - Vixen.Tests.dll (net10.0)

## Interfaces and Dependencies

Use `Catel.MVVM.ViewModelBase`, `Catel.MVVM.Command`, and `Catel.MVVM.TaskCommand` for the refactored view model. Use `IOpenFileService`, `ISaveFileService`, `DetermineOpenFileContext`, `DetermineSaveFileContext`, and `IMessageService` for dialogs and user messages.

Use `Vixen.Sys.LayerMixing.SequenceLayers`, `ILayer`, `StandardLayer`, and `LayerType` for layer manipulation. Preserve the invariant that the default layer is last.

Use `Vixen.Services.LayerMixingFilterService` or a thin injectable adapter around it to create `ILayerMixingFilterInstance` values from filter type IDs. A thin adapter improves testability and makes missing-filter tests straightforward.

Use `Vixen.Module.IModuleDataModel` and existing sequence persistence code in `SequenceTypeDataModelBase` as the behavioral reference for preserving module data. The final serializer choice must be recorded in the Decision Log after the research milestone.

Add or update these likely files:

- `src/Vixen.Modules/Editor/LayerEditor/LayerEditorView.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Themes/Generic.xaml` or replacement Catel XAML view files
- `src/Vixen.Modules/Editor/LayerEditor/ViewModels/LayerEditorViewModel.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerEditorLayerService.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/LayerEditorLayerService.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerImportExportService.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/LayerImportExportService.cs`
- `src/Vixen.Modules/Editor/LayerEditor/ImportExport/LayerExportDocument.cs`
- `src/Vixen.Modules/Editor/LayerEditor/ImportExport/LayerExportRecord.cs`
- `src/Vixen.Modules/Editor/LayerEditor/ImportExport/LayerImportPlan.cs`
- `src/Vixen.Modules/Editor/LayerEditor/ImportExport/LayerImportResult.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs`
- `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj`
- `src/Vixen.Common/Resources/Resources.csproj`
- `src/Vixen.Common/Resources/<quick-rename-icon>.png`
- `src/Vixen.Tests/Sequencer/LayerEditorLayerServiceTests.cs`
- `src/Vixen.Tests/Sequencer/LayerImportExportServiceTests.cs`
- `src/Vixen.Tests/Vixen.Tests.csproj`, only if new test project references are required

Milestone 1 added project references from `src/Vixen.Tests/Vixen.Tests.csproj` to `src/Vixen.Modules/LayerMixingFilter/ChromaKey/ChromaKey.csproj`, `src/Vixen.Modules/LayerMixingFilter/LumaKey/LumaKey.csproj`, and `src/Vixen.Modules/LayerMixingFilter/MaskFill/MaskFill.csproj`. These references let the test suite prove serialization against the real current filter data models. Because `Vixen.Tests` targets `net10.0-windows`, these references are compatible with the filter projects' WinForms setup UI dependencies.

If any new public or protected API is added or changed, update XML documentation in the same change. If only internal APIs are added, still document complex service contracts enough that the next implementer can understand import/export behavior without reading every test.

## Revision Notes

2026-07-06 / Codex: Initial ExecPlan created from `docs/sequencer/vix-2973-import-export-layers.md` so implementation can proceed from a self-contained plan.

2026-07-06 / Codex: Completed milestone 1 by adding serialization proof tests for the current layer mixing filter module data. Recorded the decision to use `DataContractJsonSerializer` for per-filter module data and prettify its JSON output with `System.Text.Json` for the human-readable `.v3l` format.

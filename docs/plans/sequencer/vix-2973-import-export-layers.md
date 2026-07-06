# Implement VIX-2973 Layer Import, Export, And Quick Rename

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. The source requirements are in `docs/sequencer/vix-2973-import-export-layers.md`; this ExecPlan repeats the necessary context so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users build sequences with layers that control how effects combine when multiple effects overlap. Today, a user who creates a useful layer setup in one timed sequence must recreate that setup by hand in the next sequence. After this work, a user can export all non-default layers to a readable `.v3l` JSON file, import that file into another sequence, and see the same layer names, ordering, mixing filter types, and mixing filter configuration. The Layer Editor will also gain a compact quick rename image button that renames a layer to the display name of its selected mixing filter type, which saves typing after the user changes a layer's filter.

The visible result is in the Timed Sequence Editor's Layer Editor. With only the default layer present, Export Layers is disabled. After the user adds standard layers, Export Layers becomes enabled. Export creates a `.v3l` file. Import adds valid layers above the existing layers without asking for confirmation when every layer is valid. If some layers cannot be imported because a mixing filter is missing or configuration cannot be restored, Vixen warns the user and lets them either cancel the whole import or continue with only the valid layers.

## Progress

- [x] (2026-07-06 17:00Z) Created this initial ExecPlan from `docs/sequencer/vix-2973-import-export-layers.md` and repository research.
- [x] (2026-07-06 18:15Z) Researched module-data JSON round-tripping and locked the serializer approach in the Decision Log. Added focused tests in `src/Vixen.Tests/Sequencer/LayerMixingFilterDataJsonSerializationTests.cs`; the targeted test command passed with 2 tests.
- [x] (2026-07-06 18:28Z) Refactored the Layer Editor to Catel MVVM while preserving the existing hosted `LayerEditorView(SequenceLayers)` construction path. Added `LayerEditorViewModel`, moved add/remove/configure commands and layer/filter state into the view model, changed XAML bindings to Catel commands, and kept WPF drag/drop in the view.
- [x] (2026-07-06 18:45Z) Extended milestone 2 by moving the Layer Editor UI out of `Themes/Generic.xaml` and into a proper Catel view at `src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml` with code-behind at `Views/LayerEditorView.xaml.cs`. Updated the Timed Sequence Editor host to reference `VixenModules.Editor.LayerEditor.Views.LayerEditorView`.
- [x] (2026-07-06 18:55Z) Removed obsolete routed-command and default-template files after the XAML view migration. `src/Vixen.Modules/Editor/LayerEditor/Input/LayerEditorCommands.cs` and `src/Vixen.Modules/Editor/LayerEditor/Themes/Generic.xaml` are no longer needed.
- [x] (2026-07-06 19:25Z) Added testable layer operation services and focused unit tests for existing layer behavior. `LayerEditorViewModel` now delegates add/remove/move/configure operations to `ILayerEditorLayerService`; `LayerEditorLayerServiceTests` passed with 15 targeted tests, and the full `Vixen.Tests` project passed with 142 tests.
- [x] (2026-07-06 19:35Z) Added the quick rename PNG resource, image-only button, and Catel command. The command delegates to the tested layer service quick rename behavior; targeted LayerEditor tests, the LayerEditor project rebuild, and the full test project passed.
- [x] (2026-07-06 20:10Z) Added `.v3l` DTOs and import/export serialization services with tests. `src/Vixen.Modules/Editor/LayerEditor/ImportExport/` gained `LayerExportDocument`, `LayerExportRecord`, `LayerImportEntry`, `LayerImportSkippedRecord`, `LayerImportPlan`, and `LayerImportResult`. `src/Vixen.Modules/Editor/LayerEditor/Services/` gained `ILayerImportExportService`/`LayerImportExportService` and `ILayerMixingFilterResolver`/`LayerMixingFilterResolver`. `src/Vixen.Tests/Sequencer/LayerImportExportServiceTests.cs` passed with 8 targeted tests, and the full `Vixen.Tests` project passed with 150 tests.
- [x] (2026-07-06 21:05Z) Added Catel file dialog and message-service command flows with tests. `LayerEditorViewModel` gained `ExportLayersCommand`/`ImportLayersCommand` (`TaskCommand`), a `V3LFileFilter` constant, and a new `ILayerImportExportService`-accepting constructor overload. `LayerEditorView.xaml` gained an import/export `ToolBarTray` styled after the Mark Collection toolbar. `src/Vixen.Tests/Sequencer/LayerEditorViewModelTests.cs` passed with 5 targeted tests, and the full `Vixen.Tests` project passed with 155 tests.
- [x] (2026-07-06 22:00Z) Added the remaining focused tests from the milestone 7 checklist. `LayerEditorViewModelTests` gained `ConfigureLayerCommand` `LayerChanged`/`CanExecute` coverage and `QuickRenameLayerCommand.CanExecute` coverage for the default layer; `LayerImportExportServiceTests` gained a missing-required-name file-level failure test, an imported-layer-gets-a-new-Id test, and stronger export assertions (filter name, restored module data values). Found and fixed a real bug in the user's own overwrite-confirmation change: `IMessageBoxService.GetUserConfirmation` never returns `MessageResult.Cancel` for its Yes/No dialog, so checking `== MessageResult.Cancel` silently let exports overwrite an existing file even after the user clicked "No". `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore` passed with 163 tests after the fix.
- [ ] Manually verify the Layer Editor in the Timed Sequence Editor host.
- [ ] Update VIX-2973 in JIRA with the final refined specification, acceptance criteria, and test guidance.
- [ ] Record final outcomes and any remaining follow-up work.

## Surprises & Discoveries

- Observation: The current Layer Editor is a WPF `UserControl`, but it is not Catel MVVM. It sets `DataContext = this`, registers routed commands in the view constructor, owns drag/drop advisors, mutates `SequenceLayers` directly, and resolves layer mixing filters through global services.
  Evidence: Before milestone 2, `src/Vixen.Modules/Editor/LayerEditor/LayerEditorView.cs` contained the constructor, command handlers, drag/drop logic, and `InitializeLayers`. That file was replaced by `Views/LayerEditorView.xaml`, `Views/LayerEditorView.xaml.cs`, and `ViewModels/LayerEditorViewModel.cs`.

- Observation: The current sequence persistence path already treats layer mixing filters specially. It stores layer/filter relationships through `LayerMixingFilterSurrogate` and stores layer mixing filter module data in `_layerMixingFilterDataModels`.
  Evidence: `src/Vixen.Core/Module/SequenceType/SequenceTypeDataModelBase.cs` builds `_layerMixingFilterSurrogates` and `_layerMixingFilterDataModels` during `SurrogateWrite`, then recreates filters and assigns module data during `SurrogateRead`.

- Observation: Shared image resources live in `src/Vixen.Common/Resources`, and WPF views reference them with pack URIs such as `/Resources;component/folder_go.png`.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Views/MarkDockerView.xaml` uses `/Resources;component/folder_go.png` and `/Resources;component/folder_open.png`; `src/Vixen.Common/Resources/Resources.csproj` marks many PNGs as `Resource`.

- Observation: `DataContractJsonSerializer` can round-trip all three current layer mixing filter module data models as JSON, including `ChromaKeyData` with `System.Drawing.Color`, `LumaKeyData`, and `MaskAndFillData`.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerMixingFilterDataJsonSerializationTests --no-restore` passed on 2026-07-06 with `Passed: 2, Failed: 0`.

- Observation: `DataContractJsonSerializer` emits compact JSON by default, but the compact JSON can be parsed with `System.Text.Json.JsonDocument` and written back with `WriteIndented = true`; the indented JSON still deserializes through `DataContractJsonSerializer`.
  Evidence: `LayerMixingFilterDataJsonSerializationTests.DataContractJsonSerializer_RestoresIndentedJson` asserts the JSON contains line breaks and restores the expected `LumaKeyData` values.

- Observation: `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj` did not reference Catel before milestone 2, even though the surrounding Timed Sequence Editor already uses Catel MVVM.
  Evidence: `dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -getItem:PackageReference -getProperty:UseWPF` showed `UseWPF` as `true` and no package references before adding `Catel.MVVM`.

- Observation: The isolated LayerEditor project rebuilds successfully after the Catel refactor, but broader TimedSequenceEditor validation is blocked by local/environment issues before it can serve as clean evidence.
  Evidence: `dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug` succeeded. `dotnet msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj -t:Rebuild -p:Configuration=Debug` failed in native C++ project imports for `Microsoft.Cpp.Default.props`. Full `msbuild` then failed on missing `Microsoft.NETCore.App.Host.win-x86` apphost pack for native-wrapper dependencies. A narrow `msbuild ... -p:BuildProjectReferences=false` reached unrelated MarkDocker XAML converter errors.

- Observation: Moving the Layer Editor UI from a default control template to a XAML view compiles cleanly in the LayerEditor project.
  Evidence: After adding `src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml` and `Views/LayerEditorView.xaml.cs`, `dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug` succeeded.

- Observation: The old routed command class is unused after the Catel command binding refactor.
  Evidence: `rg -n "LayerEditorCommands|VixenModules\.Editor\.LayerEditor\.Input|clr-namespace:VixenModules.Editor.LayerEditor.Input" src\Vixen.Modules\Editor\LayerEditor src\Vixen.Modules\Editor\TimedSequenceEditor docs\plans\sequencer\vix-2973-import-export-layers.md` returned no matches after removing `LayerEditorCommands.cs`.

- Observation: `Vixen.Tests` can reference the WPF LayerEditor project and run pure service tests without launching WPF.
  Evidence: Adding a project reference from `src/Vixen.Tests/Vixen.Tests.csproj` to `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj` allowed `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditor --no-restore` to run 15 service tests successfully.

- Observation: LayerEditor must reference the shared Resources project directly before it can use `/Resources;component/...` image pack URIs in its own XAML.
  Evidence: Milestone 4 added `..\..\..\Vixen.Common\Resources\Resources.csproj` to `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj`, added `layer_rename.png` as a WPF `Resource`, and `dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug -p:UseSharedCompilation=false` succeeded.

- Observation: The LayerEditor module tree sets `<Nullable>disable</Nullable>` in `src/Vixen.Modules/Editor/Directory.Build.props`, overriding the repository-wide `<Nullable>enable</Nullable>` default. `?`-annotated reference types in that module compile but emit `CS8632` warnings.
  Evidence: `dotnet build src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -c Debug -p:UseSharedCompilation=false` printed `CS8632` for every nullable-annotated signature added in milestone 5 until the annotations were removed; a subsequent build with plain (unannotated) reference types produced zero warnings.

- Observation: `Vixen.Sys.LayerMixing.Layer.FilterTypeId` returns `Guid.Empty` whenever no `LayerMixingFilter` is attached, and `DefaultLayer` never attaches one. This makes `Guid.Empty` a safe, existing-codebase-consistent sentinel for recognizing a hand-edited default-layer record in a `.v3l` file without needing a separate "is default" field.
  Evidence: `src/Vixen.Core/Sys/LayerMixing/Layer.cs` (`FilterTypeId` getter) and `src/Vixen.Core/Sys/LayerMixing/DefaultLayer.cs` (constructor never sets `LayerMixingFilter`).

- Observation: `System.Text.Json`'s `JsonElement` can be embedded directly as a DTO property and round-trips through `JsonSerializer.Serialize`/`Deserialize` without a custom converter, letting the per-filter `DataContractJsonSerializer` output sit inside the larger `.v3l` document verbatim.
  Evidence: `LayerImportExportServiceTests.ExportAsync_WritesNonDefaultLayersInTopToBottomOrderAsIndentedJson` and `ReadImportPlanAsync_AllLayersValid_ProducesImportableEntriesWithRestoredModuleData` passed using `LayerExportRecord.FilterData` typed as `JsonElement`.

- Observation: The repository's established Catel file/message dialog convention resolves services at command-execution time via `this.GetDependencyResolver()` inside the view model method (see `PropEditorViewModel.SaveModelAs`, `ElementMapperViewModel.OpenMapAsync`), not through constructor injection, because these view models are manually constructed with `new` by WinForms hosts rather than through a Catel `IViewModelFactory`. The repository also prefers its own `Common.WPFCommon.Services.IMessageBoxService` (`GetUserConfirmation`, `ShowError`) over Catel's built-in `IMessageService` for warnings and confirmations.
  Evidence: `rg -n "GetDependencyResolver|IMessageBoxService|IMessageService" src/Vixen.Modules` showed `IMessageBoxService` used pervasively (`PropEditorViewModel.cs`, `ElementMapperViewModel.cs`, `MarkCollectionViewModel.cs`, `VendorInventoryWindowViewModel.cs`) versus a single legacy `IMessageService` use in `XModelImport.cs`. `IMessageBoxService` is registered globally in `VixenApplication.cs`, so it resolves without any LayerEditor-specific service registration.

- Observation: `LayerEditorViewModel`'s constructor calls `ApplicationServices.GetModuleDescriptors<ILayerMixingFilterInstance>()` (via `LoadFilterTypes`), which returns `null` when the module discovery registry (`Vixen.Sys.Modules`) has not been initialized, as is the case in the `Vixen.Tests` host. The pre-existing milestone 2 code passed that `null` straight into a LINQ `.Select`, so constructing `LayerEditorViewModel` directly in a unit test threw a `NullReferenceException` before milestone 6 changes.
  Evidence: Reproduced by writing a temporary test that called `new LayerEditorViewModel(new SequenceLayers())` before the null-guard fix; it failed with `NullReferenceException` inside `LoadFilterTypes`. Adding a `descriptors == null` guard that returns an empty list fixed the crash and let `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditorViewModelTests --no-restore` pass with 5 tests.

- Observation: `Vixen.Module.ModuleDataModelBase` declares `[DataMember] public Guid ModuleTypeId` and `[DataMember] public Guid ModuleInstanceId`, so every `DataContractJsonSerializer` output for a filter's `ModuleData` includes those two fields. Neither is populated for layer mixing filter data models in the current codebase (they surfaced as `Guid.Empty` in exported `.v3l` files), and the import path never reads them: `TryRestoreFilterData` resolves the concrete type from the freshly created filter instance's own `ModuleData.GetType()`, and the layer's real filter type/instance identity already comes from `LayerExportRecord.FilterTypeId` and a fresh `ILayerMixingFilterInstance` created during import. The two fields were pure noise in the exported file.
  Evidence: User inspection of an exported `.v3l` file found `"ModuleTypeId": "00000000-0000-0000-0000-000000000000"` and `"ModuleInstanceId": "00000000-0000-0000-0000-000000000000"` inside `filterData`. Confirmed `ModuleDataModelBase` (`src/Vixen.Core/Module/ModuleDataModelBase.cs`) is the source via its `[DataMember]` attributes.

- Observation: The user added an overwrite-confirmation check to `ExportLayersAsync` (`if (File.Exists(result.FileName)) { ... if (confirmResult.Result == MessageResult.Cancel) return; }`), but `Common.WPFCommon.Services.IMessageBoxService.GetUserConfirmation` shows a Yes/No dialog, not a dialog with a Cancel button. Its "YES" button's `DialogResult` is `DialogResult.OK` and its "No" button's is `DialogResult.No`; `MessageBoxService.GetUserConfirmation` converts that `DialogResult` straight into `MessageResult` by name, so the possible results are only `MessageResult.OK` or `MessageResult.No`, never `MessageResult.Cancel`. The `== MessageResult.Cancel` check could never be true, so clicking "No" fell through and exported (overwrote) the file anyway.
  Evidence: `src/Vixen.Common/Controls/MessageBoxForm.Designer.cs` sets `buttonOk.DialogResult = DialogResult.OK` and `buttonNo.DialogResult = DialogResult.No`; `MessageBoxForm(string, string, MessageBoxButtons, Icon)` never makes `buttonCancel` visible for `MessageBoxButtons.YesNo`. Fixed by checking `confirmResult.Result != MessageResult.OK` instead, matching the `if (response.Result == MessageResult.OK)` pattern already used elsewhere in the repository (e.g. `PropEditorViewModel.cs`).

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

- Decision: Superseded the temporary template-based Catel refactor and moved `LayerEditorView` to a proper XAML view under `src/Vixen.Modules/Editor/LayerEditor/Views`.
  Rationale: Keeping all UI in `Themes/Generic.xaml` made the feature harder to navigate and did not match the requested Model/ViewModel/View organization. A normal Catel view file makes future import/export and quick rename UI edits easier to review, while still preserving the host's ability to construct `LayerEditorView` with a `SequenceLayers` instance.
  Date/Author: 2026-07-06 / Codex

- Decision: Delete `LayerEditorCommands.cs` and `Themes/Generic.xaml` instead of leaving empty compatibility files.
  Rationale: The Catel view binds directly to `LayerEditorViewModel` commands, and the Timed Sequence Editor host no longer merges `Generic.xaml`. Keeping unused routed-command and default-template files would make future contributors search dead paths.
  Date/Author: 2026-07-06 / Codex

- Decision: Leave WPF drag/drop hit testing in `LayerEditorView`, but route the actual move operation through `LayerEditorViewModel.MoveLayer`.
  Rationale: Hit testing `ListBoxItem` containers is view-specific WPF work. The layer mutation should live outside routed command handlers so the next milestone can move it into a testable layer operation service.
  Date/Author: 2026-07-06 / Codex

- Decision: Add `ILayerEditorLayerService.AddLayer(SequenceLayers, ILayerMixingFilterInstance)` alongside the GUID-based add method.
  Rationale: The view model already has an available filter instance from module discovery, and tests can provide a mocked filter instance without initializing the global module registry. The GUID overload remains for later import/export code that will read filter type IDs from `.v3l` files.
  Date/Author: 2026-07-06 / Codex

- Decision: Create `layer_rename.png` in the shared Resources project for the quick rename button instead of reusing an existing generic pencil icon.
  Rationale: The button represents a layer-specific rename-to-filter action. A small rows-plus-pencil icon keeps the compact image-only UI understandable without adding text to the row.
  Date/Author: 2026-07-06 / Codex

- Decision: Introduce `ILayerMixingFilterResolver`/`LayerMixingFilterResolver` as a thin adapter around `LayerMixingFilterService.Instance.GetInstance` and inject it into `LayerImportExportService` instead of calling the static service directly.
  Rationale: Import must be testable for the missing-filter-type scenario (a `.v3l` record referencing a module type that is not installed). A static service call cannot be mocked; the adapter interface lets tests return `null` for an arbitrary type ID without touching the real module registry.
  Date/Author: 2026-07-06 / Codex

- Decision: Store `filterDataType` in `LayerExportRecord` as `"{Type.FullName}, {Type.Assembly.GetName().Name}"` for human readability only; import resolves the concrete `IModuleDataModel` type from the freshly created filter instance's own `ModuleData.GetType()`, not by parsing `filterDataType`.
  Rationale: Trusting a string type name from a hand-editable file to load a type would be fragile and is unnecessary: the filter type ID already identifies the correct module, and that module's own current data-model type is authoritative for `DataContractJsonSerializer`.
  Date/Author: 2026-07-06 / Codex

- Decision: Treat a `.v3l` layer record whose `filterTypeId` is `Guid.Empty` as an ignorable default-layer record, silently skipped rather than added to `LayerImportPlan.SkippedLayers`.
  Rationale: `Guid.Empty` is exactly the value `Layer.FilterTypeId` returns for the default layer today, since the default layer never has an attached mixing filter. Treating it as an unimportable/skipped-with-warning layer would surface a confusing warning for something that isn't a real error.
  Date/Author: 2026-07-06 / Codex

- Decision: Remove nullable-reference (`?`) annotations from the milestone 5 files after discovering `src/Vixen.Modules/Editor/Directory.Build.props` sets `<Nullable>disable</Nullable>` for the whole Editor module tree.
  Rationale: The annotations compiled but produced `CS8632` warnings inconsistent with the rest of the LayerEditor module's existing (unannotated) code; matching the module's established nullable-disabled convention keeps the build warning-free.
  Date/Author: 2026-07-06 / Codex

- Decision: Resolve `ISaveFileService`, `IOpenFileService`, and `Common.WPFCommon.Services.IMessageBoxService` inside `ExportLayersAsync`/`ImportLayersAsync` via `this.GetDependencyResolver()` rather than constructor-injecting them into `LayerEditorViewModel`.
  Rationale: `TimedSequenceEditor.LayerEditor` constructs `LayerEditorView`/`LayerEditorViewModel` directly with `new`, not through a Catel `IViewModelFactory`, so there is no composition root to supply these services as constructor arguments. This matches the established pattern used throughout the codebase (`PropEditorViewModel`, `ElementMapperViewModel`) for manually constructed view models. `ILayerEditorLayerService` and `ILayerImportExportService` remain constructor-injected because they are plain, easily-testable abstractions with no Catel/WPF dependency.
  Date/Author: 2026-07-06 / Codex

- Decision: Use `Common.WPFCommon.Services.IMessageBoxService` (`ShowError`, `GetUserConfirmation`) for import/export warnings and the skipped-layer confirmation prompt, instead of Catel's built-in `IMessageService`.
  Rationale: `IMessageBoxService` is the repository's dominant convention for this kind of dialog (used across `PropEditorViewModel`, `ElementMapperViewModel`, `MarkCollectionViewModel`, `VendorInventoryWindowViewModel`) and is already registered globally in `VixenApplication.cs`, so no additional service registration is needed for the LayerEditor module.
  Date/Author: 2026-07-06 / Codex

- Decision: Add a defensive `null` check in `LayerEditorViewModel.LoadFilterTypes` so it returns an empty list instead of throwing when `ApplicationServices.GetModuleDescriptors<ILayerMixingFilterInstance>()` returns `null`.
  Rationale: This was a latent bug from milestone 2 that only surfaces when the module discovery registry is not yet populated (the `Vixen.Tests` host, and potentially very early application startup). Fixing it was necessary to construct `LayerEditorViewModel` directly in milestone 6 unit tests, and it is a strict robustness improvement with no behavior change in the normal, fully-initialized application.
  Date/Author: 2026-07-06 / Codex

- Decision: Strip `ModuleTypeId` and `ModuleInstanceId` from each layer's `filterData` JSON during export, by parsing the `DataContractJsonSerializer` output into a mutable `System.Text.Json.Nodes.JsonObject` and removing those two properties before embedding it in the `.v3l` document.
  Rationale: The user reported these fields showing up as all-zero GUIDs in exported files. They come from `ModuleDataModelBase`'s `[DataMember]` attributes, are never populated for layer mixing filter data, and are never consulted on import (the filter type comes from `LayerExportRecord.FilterTypeId`, and the instance is freshly created). Removing them keeps the exchange format focused on data the import path actually uses, matching the "internal but human-readable" goal from the requirements doc. `TryRestoreFilterData` did not need to change: `DataContractJsonSerializer` treats missing optional `[DataMember]`s as absent/default on read, so omitting them from the JSON does not break restoring `LowerLimit`/`UpperLimit`/etc.
  Date/Author: 2026-07-06 / Codex

- Decision: Omit `filterDataType` and `filterData` entirely from an exported layer record when the layer's mixing filter has no `ModuleData` (i.e. `filterDataType` would be `""` and `filterData` would be JSON `null`), by post-processing the serialized `LayerExportDocument` as a `JsonNode`/`JsonObject` tree rather than relying on `System.Text.Json`'s attribute-based default-value ignoring.
  Rationale: The user asked that default/placeholder values not be written when avoidable. `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]` was considered but rejected: it compares against `default(T)`, and `default(string)` is `null`, not `""`, so it would not have caught the empty-string case without also changing `LayerExportRecord.FilterDataType`'s default value semantics; `default(JsonElement)` (`Undefined`) also does not equal the JSON-`null` `JsonElement` produced by `SerializeFilterData`, so it would not reliably catch that case either. Post-processing the `JsonNode` tree after serialization is explicit, easy to test, and does not touch the DTOs' declared shape. Import is unaffected: a `LayerExportRecord` missing these keys deserializes with `FilterDataType == string.Empty` (its property initializer) and `FilterData` at `default(JsonElement)` (`ValueKind.Undefined`), which `TryRestoreFilterData` already treats identically to an explicit JSON `null` `FilterData`.
  Date/Author: 2026-07-06 / Codex

- Decision: Fix the user's overwrite-confirmation check in `ExportLayersAsync` from `confirmResult.Result == MessageResult.Cancel` to `confirmResult.Result != MessageResult.OK`.
  Rationale: `IMessageBoxService.GetUserConfirmation` only ever returns `MessageResult.OK` (the "YES" button) or `MessageResult.No` for its Yes/No dialog; it never returns `MessageResult.Cancel`. The original check could never trigger, so declining to overwrite ("No") silently exported anyway. Checking for the affirmative result and treating everything else as "do not overwrite" matches the existing `if (response.Result == MessageResult.OK)` pattern used elsewhere in the repository.
  Date/Author: 2026-07-06 / Codex

- Decision: Leave the overwrite-confirmation flow, the "no confirmation on an all-valid import" flow, and the partial-import skip/cancel confirmation flow without dedicated automated tests; verify them manually in milestone 8 instead.
  Rationale: All three behaviors hinge on `IMessageBoxService`/`IOpenFileService`/`ISaveFileService`, which `LayerEditorViewModel` resolves via `this.GetDependencyResolver()` at call time (see the milestone 6 decision on that pattern) rather than through constructor injection. Unit testing them would require either introducing a seam that no other view model in the repository uses, or standing up a fake Catel service locator/dependency resolver, which is disproportionate for three narrow UI confirmation paths. The underlying decision logic each path depends on (`HasExportableLayers`, `LayerImportPlan.IsValid`/`HasSkippedLayers`/`ImportableLayers`) is already fully unit tested.
  Date/Author: 2026-07-06 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. The implementation now has focused proof tests showing that the current layer mixing filter module data can be serialized to readable JSON and restored without losing configured values. The next milestone can build the actual import/export service on `DataContractJsonSerializer` for filter `ModuleData`, while still using a versioned top-level `.v3l` JSON document.

Milestone 2 is complete. The Layer Editor now has `src/Vixen.Modules/Editor/LayerEditor/ViewModels/LayerEditorViewModel.cs`, which owns the layer collection, filter type list, Add Layer command, Remove Layer command, Configure Layer command, and event forwarding for collection/filter setup changes. `LayerEditorView` now inherits from Catel's `UserControl`, constructs the view model from `SequenceLayers`, and keeps only view-specific drag/drop adapter code.

Milestone 2 refinement is complete. The Layer Editor UI now lives in `src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml`, with its WPF-specific drag/drop adapter in `Views/LayerEditorView.xaml.cs`. `Themes/Generic.xaml` and `Input/LayerEditorCommands.cs` have been deleted because the view is no longer a default-template custom control and no longer uses routed commands. The Timed Sequence Editor host now uses `VixenModules.Editor.LayerEditor.Views.LayerEditorView` and no longer merges `Themes/Generic.xaml` at runtime.

Milestone 3 is complete. `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerEditorLayerService.cs` defines the layer operation contract, and `LayerEditorLayerService.cs` implements add, remove, move, configure, quick rename, unique name generation, and exportability checks. `LayerEditorViewModel` now delegates mutations to that service. `src/Vixen.Tests/Sequencer/LayerEditorLayerServiceTests.cs` covers the existing add/remove/move/configure behavior and the core quick rename/exportability helpers that later UI and import/export milestones will consume.

Milestone 4 is complete. `src/Vixen.Common/Resources/layer_rename.png` is included as a WPF resource, and `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj` references the Resources project so `LayerEditorView.xaml` can use the pack URI. `LayerEditorViewModel` now exposes `QuickRenameLayerCommand`, and the layer row has a 24px image-only button beside the name text box that invokes the command for standard layers.

Milestone 5 is complete. `src/Vixen.Modules/Editor/LayerEditor/ImportExport/` has `LayerExportDocument`, `LayerExportRecord`, `LayerImportEntry`, `LayerImportSkippedRecord`, `LayerImportPlan`, and `LayerImportResult`. `src/Vixen.Modules/Editor/LayerEditor/Services/` adds `ILayerImportExportService`/`LayerImportExportService`, plus `ILayerMixingFilterResolver`/`LayerMixingFilterResolver` so import can be tested against a missing filter type without the real module registry. `ExportAsync` writes indented JSON containing every non-default layer in top-to-bottom order, embedding each filter's `DataContractJsonSerializer` output as a `JsonElement`. `ReadImportPlanAsync` validates format/version/required fields before returning a `LayerImportPlan`, classifies missing-filter and unrestorable-data layers as skipped, and silently ignores hand-edited default-layer records (`filterTypeId == Guid.Empty`). `Import` adds `LayerImportPlan.ImportableLayers` above the existing layers in their exported relative order (added bottom-to-top to compensate for `SequenceLayers.AddLayer` inserting at index 0) and renames collisions through the existing `ILayerEditorLayerService.CreateUniqueLayerName`. `src/Vixen.Tests/Sequencer/LayerImportExportServiceTests.cs` covers export ordering/shape, successful import with module-data restoration, missing-filter skipping, ignoring default-layer records, invalid JSON, unsupported format/version, and duplicate-name renaming on import.

Post-milestone-6 fix: `LayerImportExportService.SerializeFilterData` now removes `ModuleTypeId`/`ModuleInstanceId` from each layer's `filterData` before it is embedded in the `.v3l` document, since those fields came from `ModuleDataModelBase` unpopulated (always `Guid.Empty`) and are never read back on import. `LayerImportExportServiceTests.ExportAsync_WritesNonDefaultLayersInTopToBottomOrderAsIndentedJson` now asserts both properties are absent from exported `filterData`. Full suite re-validated at 155 tests passing.

Post-milestone-6 fix: `ExportAsync` now serializes the `LayerExportDocument` to a `JsonNode`, then removes each layer's `filterDataType` key when it would be an empty string and its `filterData` key when it would be JSON `null`, before writing the final indented JSON. This avoids writing placeholder defaults for layers whose mixing filter has no `ModuleData` (e.g. Chroma Key/Mask and Fill filters without configurable state). Added `LayerImportExportServiceTests.ExportAsync_OmitsFilterDataTypeAndFilterDataWhenFilterHasNoModuleData`, which exports such a layer, confirms both keys are absent from the raw JSON, and confirms re-parsing through `LayerExportDocument` still yields the same defaults (`FilterDataType == string.Empty`, `FilterData.ValueKind == JsonValueKind.Undefined`) that `TryRestoreFilterData` already handles. Full suite re-validated at 156 tests passing.

Milestone 6 is complete. `LayerEditorViewModel` gained a third constructor overload accepting `ILayerImportExportService` (the two- and one-argument overloads now build a default `LayerImportExportService`/`LayerMixingFilterResolver` chain), an `ExportLayersCommand`/`ImportLayersCommand` pair of `TaskCommand`s, and a shared `V3LFileFilter` constant. `ExportLayersAsync` resolves `ISaveFileService` via `this.GetDependencyResolver()`, prompts for a `.v3l` path, and reports IO failures through `IMessageBoxService.ShowError`; `CanExportLayers` reuses `ILayerEditorLayerService.HasExportableLayers` and is re-evaluated on every layer collection change. `ImportLayersAsync` resolves `IOpenFileService`, reads a `LayerImportPlan`, shows an error and imports nothing for file-level failures or an all-skipped file, prompts for confirmation via `IMessageBoxService.GetUserConfirmation` only when some (but not all) layers are skipped, and otherwise imports immediately with no confirmation. `LayerEditorView.xaml` gained an import/export toolbar styled after `MarkDockerView.xaml`'s `folder_go.png`/`folder_open.png` buttons. Fixed a latent `NullReferenceException` in `LoadFilterTypes` (uncovered while writing tests) so the view model can be constructed when module discovery has not run. `src/Vixen.Tests/Sequencer/LayerEditorViewModelTests.cs` covers `CanExecute`/`CanExecuteChanged` wiring for both commands using the real `LayerEditorLayerService` and a mocked `ILayerImportExportService`; the actual dialog/message-box flows are left for milestone 8's manual verification because Catel's `IOpenFileService`/`ISaveFileService`/`IMessageBoxService` require a running WPF host to exercise meaningfully.

Post-milestone-6 fix: the user added an overwrite-confirmation prompt to `ExportLayersAsync` (show a Yes/No dialog via `IMessageBoxService.GetUserConfirmation` when `result.FileName` already exists) but compared the result against `MessageResult.Cancel`, which that dialog can never return. Fixed the comparison to `!= MessageResult.OK` so declining the prompt actually cancels the export. See the Decision Log and Surprises & Discoveries entries for the DialogResult mapping evidence.

Milestone 7 is complete. Filled the remaining gaps against the milestone 7 checklist: `LayerEditorViewModelTests` gained tests for `ConfigureLayerCommand` raising/not-raising `LayerChanged` based on `Setup()`'s result, `ConfigureLayerCommand.CanExecute` being `false` for a filter with no setup, and `QuickRenameLayerCommand.CanExecute` being `false`/`true` for the default layer versus a standard layer with a filter. `LayerImportExportServiceTests` gained a missing-required-`name` file-level-failure test (leaves the sequence unchanged), an assertion that an imported layer gets a new `Id` distinct from the exported source layer's `Id`, and strengthened the main export test to assert `FilterName` and the restored `LowerLimit`/`UpperLimit` values inside `filterData`. The three dialog-dependent behaviors from the checklist (no confirmation on an all-valid import, confirm-before-partial-import, cancel-without-mutation) remain manual-verification items for milestone 8, per the Decision Log. Full suite: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore` passed with 163 tests; `dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug` succeeded with no new warnings.

## Context and Orientation

Vixen is a Windows desktop application built on .NET 10 and WPF, with many UI areas still hosted by WinForms shells. The current Layer Editor module is under `src/Vixen.Modules/Editor/LayerEditor`. The Timed Sequence Editor hosts this module in `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs`.

A sequence layer is a row-like concept that tells Vixen how to combine effects that overlap in time. The default layer always exists and is the fallback layer. Standard layers are user-created layers above the default layer. A layer mixing filter is a module instance attached to a standard layer that combines colors or intensities between layers. Examples are under `src/Vixen.Modules/LayerMixingFilter`.

The current WPF view is `VixenModules.Editor.LayerEditor.Views.LayerEditorView` in `src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml` and `LayerEditorView.xaml.cs`. It is a Catel `UserControl`. The XAML renders Add Layer and Remove Layer text buttons, a `ListBox` named `_lbLayers`, a `TextBox` bound to `LayerName`, a `ComboBox` bound to `FilterTypeId`, and a Configuration button for filters that support setup. The code-behind is limited to WPF-specific drag/drop adapter work and event forwarding for the legacy WinForms host.

The Layer Editor view model is `VixenModules.Editor.LayerEditor.ViewModels.LayerEditorViewModel` in `src/Vixen.Modules/Editor/LayerEditor/ViewModels/LayerEditorViewModel.cs`. It owns the editable `SequenceLayers` collection, the available layer mixing filter types, and the Add Layer, Remove Layer, and Configure Layer Catel commands. The module currently has no separate model classes beyond the existing domain model types in `Vixen.Core`; add a `Models` folder only when new LayerEditor-specific model types are introduced.

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
    Get-Content -LiteralPath src\Vixen.Modules\Editor\LayerEditor\Views\LayerEditorView.xaml
    Get-Content -LiteralPath src\Vixen.Modules\Editor\LayerEditor\Views\LayerEditorView.xaml.cs
    Get-Content -LiteralPath src\Vixen.Modules\Editor\LayerEditor\ViewModels\LayerEditorViewModel.cs
    Get-Content -LiteralPath src\Vixen.Modules\Editor\TimedSequenceEditor\LayerEditor.cs
    Get-Content -LiteralPath src\Vixen.Core\Sys\LayerMixing\SequenceLayers.cs
    Get-Content -LiteralPath src\Vixen.Core\Module\SequenceType\SequenceTypeDataModelBase.cs
    Get-Content -LiteralPath src\Vixen.Core\Module\SequenceType\Surrogate\LayerMixingFilterSurrogate.cs
    Get-Content -LiteralPath src\Vixen.Common\Resources\Resources.csproj

Research available layer mixing filters and choose at least one configured filter for the module-data round-trip proof:

    rg -n "class .*Data|ModuleData|HasSetup|Setup" src\Vixen.Modules\LayerMixingFilter -g "*.cs"

Milestone 1, module-data serialization proof: create focused test coverage that proves the selected module-data serialization approach can round-trip a real `IModuleDataModel` into human-readable JSON and restore it into a new filter instance. Keep any temporary spike files out of the final change. At the end of this milestone, update `Decision Log` with the serializer decision and update `Surprises & Discoveries` with evidence from test output or failed attempts.

Milestone 1 result: this milestone added `src/Vixen.Tests/Sequencer/LayerMixingFilterDataJsonSerializationTests.cs` and project references from `src/Vixen.Tests/Vixen.Tests.csproj` to the three existing layer mixing filter projects. The tests prove `DataContractJsonSerializer` round-trips `ChromaKeyData`, `LumaKeyData`, and `MaskAndFillData`, and prove that indented JSON produced by `System.Text.Json` remains readable by `DataContractJsonSerializer`. Do not remove these tests when implementing the import/export service; extend them or replace their helper methods with calls into the production serializer once that serializer exists.

Milestone 2, Catel refactor: add a Catel view model under `src/Vixen.Modules/Editor/LayerEditor/ViewModels`, for example `LayerEditorViewModel.cs`. Add any row view models only if they remove real binding complexity; avoid creating view models that merely mirror `ILayer` property names. Convert the view to Catel's `UserControl` pattern by replacing the old root `LayerEditorView.cs` plus `Themes/Generic.xaml` template with a XAML-backed Catel view under `src/Vixen.Modules/Editor/LayerEditor/Views`. Keep `TimedSequenceEditor.LayerEditor` able to construct and host the control with a `SequenceLayers` instance. The host must still forward collection and layer change notifications into its `LayersChanged` event.

Milestone 2 result: `LayerEditorViewModel` was added under `src/Vixen.Modules/Editor/LayerEditor/ViewModels`, and `LayerEditorView` now lives under `src/Vixen.Modules/Editor/LayerEditor/Views`. The existing `LayerEditorView(SequenceLayers)` constructor remains, but delegates to a second constructor that accepts `LayerEditorViewModel` and passes it to Catel's `UserControl` base constructor. The view model exposes `Layers`, `FilterTypes`, `AddLayerCommand`, `RemoveLayerCommand`, and `ConfigureLayerCommand`. It forwards collection changes through `INotifyCollectionChanged` and filter setup changes through `LayerChanged`, so the existing Timed Sequence Editor host can continue subscribing to the view's `CollectionChanged` and `LayerChanged` events. The view uses its named `_lbLayers` list box for the existing `DragDropManager` integration; it calls `LayerEditorViewModel.MoveLayer` when a drop completes.

Milestone 3, layer operations: add a testable service under `src/Vixen.Modules/Editor/LayerEditor/Services`, for example `LayerEditorLayerService.cs` and `ILayerEditorLayerService.cs`. It should expose operations equivalent to:

    AddLayer(SequenceLayers layers, Guid filterTypeId)
    RemoveLayer(SequenceLayers layers, ILayer layer)
    MoveLayer(SequenceLayers layers, ILayer layer, int newIndex)
    ConfigureLayer(ILayer layer)
    QuickRenameLayer(SequenceLayers layers, ILayer layer)
    CreateUniqueLayerName(IEnumerable<ILayer> layers, string desiredName)
    HasExportableLayers(SequenceLayers layers)

Use exact names that fit the final design, but preserve these behaviors. If the service or methods are public or protected, add XML documentation in the same change. If they are internal but complex, add concise XML or comments where they help.

Milestone 3 result: `ILayerEditorLayerService` and `LayerEditorLayerService` were added under `src/Vixen.Modules/Editor/LayerEditor/Services`. The service exposes `AddLayer(SequenceLayers, Guid)`, `AddLayer(SequenceLayers, ILayerMixingFilterInstance)`, `RemoveLayer`, `MoveLayer`, `ConfigureLayer`, `QuickRenameLayer`, `CreateUniqueLayerName`, and `HasExportableLayers`. `LayerEditorViewModel` keeps the user-facing Catel commands, but calls the service for mutations. `Vixen.Tests` now references the LayerEditor project and has focused tests in `src/Vixen.Tests/Sequencer/LayerEditorLayerServiceTests.cs`.

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

Milestone 2 validation evidence:

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll

Milestone 2 refinement validation evidence:

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll

Milestone 3 validation evidence:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditor --no-restore
    Passed!  - Failed:     0, Passed:    15, Skipped:     0, Total:    15, Duration: 115 ms - Vixen.Tests.dll (net10.0)

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore
    Passed!  - Failed:     0, Passed:   142, Skipped:     0, Total:   142, Duration: 161 ms - Vixen.Tests.dll (net10.0)

Milestone 4 validation evidence:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditor --no-restore
    Passed!  - Failed:     0, Passed:    15, Skipped:     0, Total:    15, Duration: 111 ms - Vixen.Tests.dll (net10.0)

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug -p:UseSharedCompilation=false
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore
    Passed!  - Failed:     0, Passed:   142, Skipped:     0, Total:   142, Duration: 162 ms - Vixen.Tests.dll (net10.0)

Milestone 5 validation evidence:

    dotnet build src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -c Debug -p:UseSharedCompilation=false
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll (Build succeeded, 0 warnings from new code)

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerImportExportServiceTests --no-restore
    Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8, Duration: 260 ms - Vixen.Tests.dll (net10.0)

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore
    Passed!  - Failed:     0, Passed:   150, Skipped:     0, Total:   150, Duration: 220 ms - Vixen.Tests.dll (net10.0)

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll

Milestone 6 validation evidence:

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug -p:UseSharedCompilation=false
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll (Build succeeded, XAML toolbar compiled cleanly)

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditorViewModelTests --no-restore
    Passed!  - Failed:     0, Passed:     5, Skipped:     0, Total:     5, Duration: 537 ms - Vixen.Tests.dll (net10.0)

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore
    Passed!  - Failed:     0, Passed:   155, Skipped:     0, Total:   155, Duration: 470 ms - Vixen.Tests.dll (net10.0)

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll

Milestone 7 validation evidence:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerEditorViewModelTests --no-restore
    Passed!  - Failed:     0, Passed:    10, Skipped:     0, Total:    10, Duration: 540 ms - Vixen.Tests.dll (net10.0)

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LayerImportExportServiceTests --no-restore
    Passed!  - Failed:     0, Passed:    11, Skipped:     0, Total:    11, Duration: 323 ms - Vixen.Tests.dll (net10.0)

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore
    Passed!  - Failed:     0, Passed:   163, Skipped:     0, Total:   163, Duration: 430 ms - Vixen.Tests.dll (net10.0)

    dotnet msbuild src\Vixen.Modules\Editor\LayerEditor\LayerEditor.csproj -t:Rebuild -p:Configuration=Debug
    LayerEditor -> C:\Dev\Vixen\src\Vixen.Modules\Editor\LayerEditor\Debug\Output\LayerEditor.dll

Broader host validation attempts and blockers:

    dotnet msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj -t:Rebuild -p:Configuration=Debug
    error MSB4278: The imported file "$(VCTargetsPath)\Microsoft.Cpp.Default.props" does not exist

    msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj -t:Rebuild -p:Configuration=Debug
    error NETSDK1145: The Apphost pack is not installed ... Microsoft.NETCore.App.Host.win-x86

    msbuild src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditor.csproj -t:Build -p:Configuration=Debug -p:BuildProjectReferences=false
    error MC3074: The tag 'ColorToSolidBrushConverter' does not exist in XML namespace 'clr-namespace:Common.WPFCommon.Converters;assembly=WPFCommon'

## Interfaces and Dependencies

Use `Catel.MVVM.ViewModelBase`, `Catel.MVVM.Command`, and `Catel.MVVM.TaskCommand` for the refactored view model. Use `IOpenFileService`, `ISaveFileService`, `DetermineOpenFileContext`, `DetermineSaveFileContext`, and `IMessageService` for dialogs and user messages.

Milestone 2 added a `Catel.MVVM` package reference to `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj` because the LayerEditor project previously had no direct Catel reference. The package version remains centrally managed by `Directory.Packages.props`.

Use `Vixen.Sys.LayerMixing.SequenceLayers`, `ILayer`, `StandardLayer`, and `LayerType` for layer manipulation. Preserve the invariant that the default layer is last.

Use `Vixen.Services.LayerMixingFilterService` or a thin injectable adapter around it to create `ILayerMixingFilterInstance` values from filter type IDs. A thin adapter improves testability and makes missing-filter tests straightforward.

Milestone 3 kept direct `LayerMixingFilterService.Instance.GetInstance(filterTypeId)` use inside `LayerEditorLayerService.AddLayer(SequenceLayers, Guid)`, but added an instance-based overload for view model use and unit tests. If missing-filter import tests need finer control later, introduce the thin adapter at the import/export service boundary.

Use `Vixen.Module.IModuleDataModel` and existing sequence persistence code in `SequenceTypeDataModelBase` as the behavioral reference for preserving module data. The final serializer choice must be recorded in the Decision Log after the research milestone.

Add or update these likely files:

- `src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml`
- `src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml.cs`
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
- `src/Vixen.Tests/Sequencer/LayerEditorViewModelTests.cs`
- `src/Vixen.Tests/Vixen.Tests.csproj`, only if new test project references are required

Milestone 1 added project references from `src/Vixen.Tests/Vixen.Tests.csproj` to `src/Vixen.Modules/LayerMixingFilter/ChromaKey/ChromaKey.csproj`, `src/Vixen.Modules/LayerMixingFilter/LumaKey/LumaKey.csproj`, and `src/Vixen.Modules/LayerMixingFilter/MaskFill/MaskFill.csproj`. These references let the test suite prove serialization against the real current filter data models. Because `Vixen.Tests` targets `net10.0-windows`, these references are compatible with the filter projects' WinForms setup UI dependencies.

Milestone 3 added a project reference from `src/Vixen.Tests/Vixen.Tests.csproj` to `src/Vixen.Modules/Editor/LayerEditor/LayerEditor.csproj` so pure LayerEditor services can be tested. The tests do not instantiate WPF controls.

If any new public or protected API is added or changed, update XML documentation in the same change. If only internal APIs are added, still document complex service contracts enough that the next implementer can understand import/export behavior without reading every test.

## Revision Notes

2026-07-06 / Codex: Initial ExecPlan created from `docs/sequencer/vix-2973-import-export-layers.md` so implementation can proceed from a self-contained plan.

2026-07-06 / Codex: Completed milestone 1 by adding serialization proof tests for the current layer mixing filter module data. Recorded the decision to use `DataContractJsonSerializer` for per-filter module data and prettify its JSON output with `System.Text.Json` for the human-readable `.v3l` format.

2026-07-06 / Codex: Completed milestone 2 by introducing a Catel `LayerEditorViewModel`, moving the layer commands and state out of `LayerEditorView`, preserving the existing `LayerEditorView(SequenceLayers)` host contract, and documenting the local host-build blockers encountered during validation.

2026-07-06 / Codex: Refined milestone 2 after user feedback by moving the Layer Editor UI out of `Themes/Generic.xaml` and into `Views/LayerEditorView.xaml`, keeping drag/drop setup in `Views/LayerEditorView.xaml.cs`, and updating the Timed Sequence Editor host to reference the view namespace directly.

2026-07-06 / Codex: Cleaned up the remaining obsolete LayerEditor routed-command/template artifacts by deleting `Input/LayerEditorCommands.cs` and `Themes/Generic.xaml`. The LayerEditor project still rebuilds successfully.

2026-07-06 / Codex: Completed milestone 3 by adding `ILayerEditorLayerService`, `LayerEditorLayerService`, and `LayerEditorLayerServiceTests`; updated `LayerEditorViewModel` to delegate layer mutations to the service; recorded targeted, project, and full-test validation evidence.

2026-07-06 / Codex: Completed milestone 4 by adding the `layer_rename.png` shared resource, wiring a compact image-only quick rename button into `LayerEditorView.xaml`, exposing `QuickRenameLayerCommand` from `LayerEditorViewModel`, and validating with targeted tests, the LayerEditor rebuild, and the full test project.

2026-07-06 / Codex: Completed milestone 5 by adding the `.v3l` DTOs (`LayerExportDocument`, `LayerExportRecord`, `LayerImportEntry`, `LayerImportSkippedRecord`, `LayerImportPlan`, `LayerImportResult`) and the `ILayerImportExportService`/`LayerImportExportService` pair, plus an `ILayerMixingFilterResolver` adapter for testable filter-type resolution. Recorded the `Guid.Empty`-as-default-layer-sentinel decision, the `Nullable disable` Editor-module convention, and validated with 8 targeted tests, the full 150-test suite, and a clean LayerEditor rebuild.

2026-07-06 / Codex: Completed milestone 6 by adding `ExportLayersCommand`/`ImportLayersCommand` to `LayerEditorViewModel`, an import/export toolbar to `LayerEditorView.xaml`, and a new `ILayerImportExportService`-accepting constructor overload. Followed the repository's established `GetDependencyResolver()`/`IMessageBoxService` convention for manually constructed view models instead of constructor-injecting Catel's file/message services. Fixed a latent `NullReferenceException` in `LoadFilterTypes` uncovered while adding `LayerEditorViewModelTests.cs`, and validated with 5 targeted tests, the full 155-test suite, and a clean LayerEditor rebuild.

2026-07-06 / Codex: Completed milestone 7 by adding the remaining checklist tests to `LayerEditorViewModelTests.cs` and `LayerImportExportServiceTests.cs` (`ConfigureLayerCommand` `LayerChanged`/`CanExecute` behavior, `QuickRenameLayerCommand.CanExecute` for the default layer, a missing-required-name file-level failure, a new-layer-Id assertion on import, and stronger export field assertions). While reviewing the user's own overwrite-confirmation addition to `ExportLayersAsync`, found and fixed a real bug: the check compared against `MessageResult.Cancel`, which `IMessageBoxService.GetUserConfirmation`'s Yes/No dialog can never return, so declining the prompt silently exported anyway; changed the check to `!= MessageResult.OK`. Validated with 163 tests passing and a clean LayerEditor rebuild.

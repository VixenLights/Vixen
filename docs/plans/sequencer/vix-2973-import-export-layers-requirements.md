# VIX-2973 Import / Export Layers Specification

## Overview

VIX-2973 adds import and export support for sequence layers in the Timed Sequence Editor. Users should be able to create a useful set of non-default layers once, export that layer setup to a human-readable `.v3l` file, and import it into another sequence without manually recreating layer names, order, layer mixing filter choices, or layer mixing filter configuration.

This document is the requirements and design specification that should be used to create an ExecPlan for implementation. The implementation ExecPlan must follow `.agents/PLANS.md`, must be saved under `docs/plans/`, and must include steps to update the existing JIRA issue with the refined specification, acceptance criteria, and testing guidance from this document.

## Repository Context

The current Layer Editor lives in `src/Vixen.Modules/Editor/LayerEditor`. The main control is `LayerEditorView` in `src/Vixen.Modules/Editor/LayerEditor/LayerEditorView.cs`, with its template in `src/Vixen.Modules/Editor/LayerEditor/Themes/Generic.xaml`.

The Layer Editor is hosted by the Timed Sequence Editor in `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs`. That host is a WinForms `DockContent` that creates a WPF `ElementHost`, places `LayerEditorView` inside it, and listens for layer change events. This hosting arrangement must continue to work after the refactor.

Layer data is owned by `Vixen.Sys.LayerMixing.SequenceLayers` in `src/Vixen.Core/Sys/LayerMixing/SequenceLayers.cs`. `SequenceLayers` maintains the observable layer list, prevents the default layer from being removed or moved above standard layers, updates layer levels after changes, and maps effects to layers. Layer model types are in `src/Vixen.Core/Sys/LayerMixing/Layer.cs`, `DefaultLayer.cs`, `StandardLayer.cs`, and `ILayer.cs`.

The Mark Collection toolbar is the visual comparison point for import and export buttons. Its Catel view is `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Views/MarkDockerView.xaml`. Reuse the same import and export image resources used there: `/Resources;component/folder_go.png` for import and `/Resources;component/folder_open.png` for export.

## Goals

The implementation must:

- Refactor the Layer Editor into a proper Catel MVVM WPF control that can still be hosted in the existing WinForms `ElementHost`.
- Preserve the current Layer Editor behavior for adding, removing, configuring, renaming, selecting, and reordering layers.
- Add import and export commands to the Layer Editor.
- Add a quick rename command to standard layers so users can rename a layer to match its selected mixing filter type without typing the name manually.
- Export all non-default layers to an internal, human-readable JSON `.v3l` exchange file.
- Import all valid layers from a `.v3l` file into the current sequence.
- Preserve layer names, relative order, mixing filter type, and mixing filter configuration data across export and import.
- Add focused unit tests for the refactored layer behavior and the new import/export behavior.

## Non-Goals

The first implementation must not add a layer selection dialog for export. Export always includes every non-default layer known to the Layer Editor.

The first implementation must not add replace-mode import. Import always adds layers to the current sequence and does not remove or overwrite existing layers.

The `.v3l` file is an internal Vixen exchange format. It should be readable by a person for diagnosis and future migrations, but it is not a public compatibility contract for third-party tools.

## Current Behavior To Preserve

The default layer always exists and occupies the known default position at the bottom of the list. It cannot be removed. It cannot be dragged above standard layers. It is not configured like a standard layer.

Adding a layer inserts a new standard layer above existing layers using `SequenceLayers.AddLayer(ILayerMixingFilterInstance)`. The new layer receives a unique name and `SequenceLayers` recalculates layer levels.

Removing a standard layer removes effect-to-layer mappings for that layer so affected effects fall back to the default layer.

Moving a standard layer preserves the default layer at the bottom and recalculates layer levels.

Configuring a layer invokes setup on the layer's mixing filter when the filter supports setup. Successful setup must still notify the Timed Sequence Editor that layers changed.

## Target User Experience

The Layer Editor must show a new top row above the existing Add Layer and Remove Layer buttons. The new row contains two image buttons:

- Import Layers, using the same import image as the Mark Collection toolbar.
- Export Layers, using the same export image as the Mark Collection toolbar.

The existing Add Layer and Remove Layer buttons remain together in the row below the new import/export row.

Each standard layer's expanded editor must include a new quick rename image button next to the layer name text box. This must be an image-only button to preserve horizontal space in the compact layer editor layout. The button renames that layer to the display name of its currently selected layer mixing filter type. For example, if the layer uses a filter whose display name is `Mask and Fill`, the quick rename command changes the layer name to `Mask and Fill`.

The quick rename button must be added after the Catel MVVM refactor, not as part of the legacy control. The button must be command-bound through the Layer Editor view model or layer operation boundary, not handled with code-behind. The implementation will likely need a new representative PNG icon stored in `src/Vixen.Common/Resources` so it can be referenced consistently with other shared image resources.

If another layer already has the target name, the command must use the existing unique-name convention: `Name - 2`, `Name - 3`, and so on. This behavior is intended for the common workflow where a user changes a layer's filter type and then wants the layer name to match the new filter type without manually typing it.

When the user clicks Export Layers, Vixen shows the standard Catel save-file dialog. The dialog defaults to the `.v3l` extension and filters to `Vixen 3 Layers (*.v3l)|*.v3l|All Files (*.*)|*.*`. After the user chooses a file, Vixen writes all non-default layers to the selected path as indented JSON.

The Export Layers button must be disabled when the sequence has no layers beyond the default layer. Users must not be able to export an empty layer set.

When the user clicks Import Layers, Vixen shows the standard Catel open-file dialog. The dialog filters to `Vixen 3 Layers (*.v3l)|*.v3l|All Files (*.*)|*.*`. After the user chooses a file, Vixen validates the file, warns about any layers that cannot be imported, and adds valid imported layers to the current sequence.

Import must append layers in the same way Add Layer does today: each imported layer is added above the existing layers. The final imported group must appear in the same relative order it had when exported. Because `SequenceLayers.AddLayer` inserts at index `0`, the implementation should either import exported layers in reverse order or use an equivalent `SequenceLayers` operation that produces the same final order and still updates levels and collection notifications correctly.

## Catel Requirements

Use the project `catel-mvvm` skill when creating the ExecPlan and during implementation.

The refactored Layer Editor view must inherit from Catel's WPF user control base, follow Catel view-to-view-model conventions, and avoid setting the data context manually from code-behind.

The Layer Editor view model must inherit from `Catel.MVVM.ViewModelBase`. Commands must be Catel `Command` or `TaskCommand` instances with matching `CanExecute` methods where command availability can change.

The view model must not use WinForms dialogs, WPF `MessageBox`, or direct UI controls. File dialogs must go through Catel services. Existing examples in the repository resolve `IOpenFileService` and `ISaveFileService` from Catel and call `DetermineFileAsync` with `DetermineOpenFileContext` or `DetermineSaveFileContext`; use that pattern unless a better project-local Catel wrapper is discovered during implementation.

Warnings and confirmations must use Catel services, such as `IMessageService`, rather than direct message boxes.

## Architecture Requirements

Separate view behavior, layer operations, and import/export serialization enough that the rules can be unit tested without launching WPF.

The expected design is:

- A Catel Layer Editor view for XAML layout and bindings.
- A Layer Editor view model for command orchestration and bindable state.
- A testable layer import/export service for JSON serialization and deserialization.
- A testable layer operation boundary for adding, removing, moving, configuring, quick-renaming, importing, and exporting layers against `SequenceLayers`.

The implementation should prefer constructor injection for new services. If the existing host or module registration makes full dependency injection impractical, the ExecPlan must explicitly document the compromise and isolate any service-locator usage at the composition boundary rather than inside business logic.

The existing WinForms host in `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs` must continue to raise `LayersChanged` when the layer collection changes or a layer configuration changes.

## File Format

The export file extension is `.v3l`, meaning Vixen 3 Layers.

The file format is JSON. JSON must be indented and human-readable.

The file must be versioned from the first implementation so future migrations can be added safely. The top-level JSON object must contain at least:

- `format`: The literal value `Vixen3Layers`.
- `version`: The integer format version. The first version is `1`.
- `exportedUtc`: An ISO-8601 UTC timestamp for diagnostics.
- `layers`: An array of exported standard layers.

Each exported layer record must contain at least:

- `name`: The layer display name.
- `order`: The layer's zero-based exported order, where `0` is the topmost exported layer.
- `filterTypeId`: The `Guid` type ID of the layer mixing filter.
- `filterName`: The layer mixing filter display name, for human readability and diagnostics.
- `filterDataType`: The serializable type identity of the mixing filter's module data, if module data exists.
- `filterData`: The mixing filter module data serialized as JSON, if module data exists.

The file must not export the default layer. If a future or hand-edited file contains a default layer, import must ignore it.

The implementation must preserve layer mixing filter configuration. Layer mixing filters are modules, and their non-static configuration is exposed through `ILayerMixingFilterInstance.ModuleData`. The ExecPlan must research the safest existing Vixen serialization pattern for module data before implementation. The selected implementation must recreate a new filter instance of the same `filterTypeId` and apply equivalent module data so the imported layer behaves like the exported layer.

The imported layer and imported filter instance must receive fresh runtime instance IDs. Do not reuse exported layer IDs or filter instance IDs in the destination sequence.

Example shape:

```json
{
	"format": "Vixen3Layers",
	"version": 1,
	"exportedUtc": "2026-07-06T16:30:00Z",
	"layers": [
		{
			"name": "Top sparkle",
			"order": 0,
			"filterTypeId": "00000000-0000-0000-0000-000000000000",
			"filterName": "Highest Layer Wins",
			"filterDataType": null,
			"filterData": null
		}
	]
}
```

## Export Rules

Export all standard layers known to the Layer Editor.

Do not export the default layer.

Preserve the visible layer order from top to bottom using the `order` field and array ordering.

Preserve each layer's name, mixing filter type ID, mixing filter display name, and mixing filter module data.

If there are no standard layers, disable the export command and do not show the save-file dialog. Do not create a `.v3l` file with an empty `layers` array.

If the selected file cannot be written, show a Catel error message and leave the sequence unchanged.

## Import Rules

Import only `.v3l` files with `format` equal to `Vixen3Layers` and a supported `version`. Version `1` is the initial supported version.

Ignore any default layer records in the file.

For each standard layer record, resolve the `filterTypeId` to an installed layer mixing filter. If the filter type is installed, create a new filter instance and apply the exported module data. If the filter type is missing or module data cannot be applied, classify that layer as not importable.

If any layers are not importable because their filter type is missing or their configuration cannot be restored, show a Catel confirmation dialog before mutating the sequence. The dialog must summarize how many layers will be imported and how many will be skipped, and it should list the skipped layer names and filter names when reasonable. If the user confirms, import the valid layers and skip the invalid layers. If the user cancels, import nothing.

If all layers in the file are valid and importable, import them immediately after file validation without showing a summary confirmation. The successful path should not interrupt the user with an extra dialog.

If all layers are invalid, show a Catel warning or error dialog and import nothing.

Imported layer names must be unique within the destination sequence. If an imported name already exists, rename it using the same convention as existing layer creation: `Name - 2`, `Name - 3`, and so on.

Imported layers must be added above the existing layers and must appear in the same relative order they had when exported. For example, if the destination sequence currently has `Existing A`, `Existing B`, and `Default`, and the exported file contains `Imported 1`, `Imported 2`, then the final order must be `Imported 1`, `Imported 2`, `Existing A`, `Existing B`, `Default`.

Import must raise the same collection and layer changed notifications that normal layer edits raise so the Timed Sequence Editor updates and marks the sequence dirty.

Import must be transactional at the file-validation level. Do not partially import while parsing or validating the file. Once validation completes and the user confirms any warning, it is acceptable to add valid layers and skip invalid ones as specified above.

Invalid JSON, unsupported format, unsupported version, missing required fields, and file I/O failures must produce a Catel error message and leave the sequence unchanged.

## Unit Testing Requirements

Create focused tests in `src/Vixen.Tests/`. Add project references only if needed, and follow solution conventions for project references.

Tests must cover the refactored layer operation behavior:

- Add layer creates a standard layer above existing layers and keeps the default layer at the bottom.
- Remove layer refuses or cannot remove the default layer.
- Reorder layer cannot move the default layer.
- Configure layer reports a layer change when setup succeeds.
- Quick rename changes a standard layer's name to the selected filter type display name.
- Quick rename makes the generated name unique with `Name - 2`, `Name - 3`, and so on when another layer already uses the filter type display name.
- Quick rename is not available for the default layer.
- Duplicate layer names are made unique with `Name - 2`, `Name - 3`, and so on.

Tests must cover export behavior:

- Export excludes the default layer.
- Export preserves top-to-bottom layer order.
- Export includes name, filter type ID, filter name, and mixing filter module data.
- Export command is disabled when only the default layer exists, and no empty `.v3l` file can be produced through the UI.

Tests must cover import behavior:

- Import adds layers above existing layers.
- Import preserves exported relative order.
- Import gives imported layers new IDs.
- Import renames duplicate names using the established naming convention.
- Import restores the mixing filter type and module data.
- Import does not request confirmation when every layer in the file is valid and importable.
- Import ignores default layer records in a hand-edited file.
- Import skips layers with missing filter types only after confirmation.
- Import cancels without mutation when the user declines the missing-filter warning.
- Invalid JSON, unsupported format, unsupported version, and missing required fields leave the sequence unchanged.

Where possible, keep serialization and import/export service tests independent of WPF. Mock or fake Catel file and message services at the view-model boundary rather than launching dialogs.

## Manual Acceptance Criteria

After implementation, a user can complete this scenario:

1. Open a timed sequence.
2. Open the Layer Editor.
3. Confirm Export Layers is disabled when only the default layer exists.
4. Create at least two non-default layers with different names and mixing filters.
5. Confirm Export Layers becomes enabled.
6. Configure a layer mixing filter that supports setup, if one is available.
7. Change one layer's mixing filter type, click the quick rename button next to the name text box, and confirm the layer name changes to the selected filter type display name.
8. Confirm the quick rename control is an image-only button and uses a shared PNG resource from `src/Vixen.Common/Resources`.
9. Repeat quick rename when another layer already has that name and confirm Vixen applies the next ` - 2` style suffix.
10. Click Export Layers, choose a `.v3l` path, and save.
11. Inspect the file and confirm it is readable JSON, contains `format`, `version`, and `layers`, and does not contain the default layer.
12. Open or create another timed sequence.
13. Open the Layer Editor.
14. Click Import Layers and choose the `.v3l` file.
15. Confirm the imported layers appear above existing layers, in the same relative order they were exported, with the same names unless duplicate names required suffixes.
16. Confirm the default layer still appears exactly once at the bottom.
17. Confirm imported layers use the expected mixing filters and restored configuration.

Missing-filter acceptance scenario:

1. Use a hand-edited `.v3l` file containing at least one valid layer and one layer with an unknown `filterTypeId`.
2. Click Import Layers and choose the file.
3. Confirm Vixen shows a warning/confirmation describing skipped layers.
4. Choose cancel and confirm no layers were imported.
5. Repeat import, choose continue, and confirm valid layers were imported while missing-filter layers were skipped.

## Implementation Risks And Concerns

The largest implementation risk is preserving layer mixing filter module data in JSON. The ExecPlan must research how Vixen currently serializes module data for sequence persistence and choose a compatible approach that can round-trip existing layer mixing filter configurations.

The current `LayerEditorView` mixes WPF UI code, routed commands, drag/drop behavior, collection notification forwarding, direct module lookup, and layer mutation. The refactor should avoid changing user-visible behavior while extracting testable seams.

Drag/drop may require a small view-specific behavior because it depends on WPF hit testing. Keep the hit-testing code in the view or a behavior, but route the actual move operation through the view model or layer operation service.

The current Mark Collection import/export code is useful for visual and workflow comparison, but it is not the architecture target for this work because it still uses WinForms dialogs in places. The Layer Editor implementation must use Catel services.

## Required ExecPlan Content

The implementation ExecPlan must:

- Reference this specification and `.agents/PLANS.md`.
- Include a milestone to refactor the existing Layer Editor to Catel MVVM while preserving behavior.
- Include a milestone to create or add the quick rename PNG icon under `src/Vixen.Common/Resources`, then add the image-only quick rename button and command after the Catel MVVM refactor.
- Include a milestone to add the import/export serialization service and JSON DTOs.
- Include a milestone to add Catel file dialogs and warning/confirmation behavior.
- Include a milestone to add and run unit tests.
- Include a milestone to manually verify the Layer Editor in the Timed Sequence Editor host.
- Include a step to update VIX-2973 in JIRA with the refined specification, acceptance criteria, and testing guidance.
- Include risks and rollback/recovery guidance.

## Decisions

- Export includes all non-default layers. A selection dialog is deferred to a future enhancement.
- Export is disabled when there are no non-default layers. Empty `.v3l` files must not be created through the UI.
- The Layer Editor must add a post-refactor image-only quick rename button beside each standard layer name text box; it renames the layer to the selected filter type display name and applies the `Name - 2` uniqueness convention when needed. The implementation will likely create a new representative PNG in `src/Vixen.Common/Resources`.
- Export must include mixing filter configuration data, not just filter type.
- Imported layers are appended above existing layers and must retain exported relative order.
- Duplicate imported layer names are renamed using `Name - 2`, `Name - 3`, and so on.
- Layers referencing missing or unrestorable filters are skipped only after a Catel confirmation dialog.
- Catel file and message services must be used for dialogs.
- Import does not show a summary confirmation when all layers are importable; only error and partial-import flows show warning or confirmation messages.
- `.v3l` is an internal Vixen exchange format, but it must be human-readable JSON.
- This document replaces the original high-level VIX-2973 note and is the source for the implementation ExecPlan.

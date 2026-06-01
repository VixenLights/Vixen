# Harden VIX-3591 State Property Identity and Validation

This ExecPlan is a living document. Maintain the `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections while implementing the work.

This document follows `.agents/PLANS.md`. It is the Phase 2 implementation plan for `docs/vix-3591-state-property-prerequisites.md`. It supplements, and does not replace or modify, the original Phase 1 plan in `docs/plans/vix-3591-state-feature.md`.

## Purpose / Big Picture

The initial VIX-3591 implementation added a configurable State property to Display Setup. Before the State effect work begins under VIX-3924, the property must become a durable reference target and must reject invalid names at the point where users finish editing them.

After this change:

- Every attached State property has a persisted, non-empty `Guid` that remains stable while that property is edited or logically cloned.
- Copying a State property to another element creates a new property identifier because the destination is a distinct attached property.
- State property names and State item names are trimmed and validated when editing completes.
- Blocking validation errors disable `OK`, while `Cancel` and window close remain available and discard edits.
- Informational warnings appear without blocking save.
- xLights imports produce valid, trimmed names and best-effort fallback names without interactive prompts.
- Focused unit tests protect identity, cloning, validation, and import normalization behavior.

The user-visible proof is straightforward: open a State property in Display Setup, make a name invalid, and observe a Catel validation error with a disabled `OK` button. Correct the value, save, copy the property to another element, and verify through tests that the copied property has a distinct stable ID.

## Progress

- [x] (2026-05-31) Read the Phase 2 requirements, original Phase 1 plan, `.agents/PLANS.md`, and applicable project skills.
- [x] (2026-05-31) Research the State module, setup dialog, copy paths, xLights parser, importer materialization path, and existing Catel validation examples.
- [x] (2026-05-31) Write the separate Phase 2 ExecPlan and Jira-ready issue update.
- [x] (2026-06-01 09:01 -05:00) Confirmed the Phase 2 update was added to Jira issue VIX-3591 before implementation. The user reported that the included text was appended; no Jira comment URL was provided.
- [x] (2026-06-01 09:01 -05:00) Established the baseline: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State` passed all 8 filtered tests, and `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` completed successfully.
- [ ] Add stable State property identity and explicit logical-clone versus copied-property semantics.
- [ ] Normalize xLights imported State names and add best-effort fallbacks.
- [ ] Add Catel validation to the State mapper dialog and focused ViewModel tests.
- [ ] Run targeted tests, Debug and Release builds, and the manual Display Setup scenarios.

## Surprises & Discoveries

- Observation: `StateItemData` already has a stable `Guid Id`, but `StateData` does not have an identifier for the overall State property.
  Evidence: `src/Vixen.Modules/Property/State/StateItemData.cs` initializes and normalizes `Id`; `src/Vixen.Modules/Property/State/StateData.cs` currently stores only `Name`, `Description`, and `Items`.

- Observation: The existing `StateData.Clone()` operation is used as a logical clone for the setup dialog draft and must preserve the new overall property ID.
  Evidence: `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs` creates `_draft = (StateData)source.Clone()` and only copies edited values back during save.

- Observation: `StateModule.CloneValues(IProperty)` is the cross-property copy hook and should create a new overall State property ID for the destination.
  Evidence: `src/Vixen.Common/Controls/ElementTree.cs` creates or locates a destination property and calls `destinationProperty.CloneValues(sourceProperty)` during copy/paste. The base API documentation describes cloning values so they are appropriate for the local node.

- Observation: xLights State definitions are parsed before they are materialized into State modules.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs` creates `StateInfo` and `StateItem` models; `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs` later maps those models to `StateModule`.

- Observation: The repository already uses the desired Catel validation pattern.
  Evidence: `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyEditorViewModel.cs` overrides Catel validation methods, `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyWindowViewModelBase.cs` enables validation before save, and `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeWindowView.xaml` uses `catel:WarningAndErrorValidator`.

- Observation: Existing State tests already cover cloning, draft save/cancel behavior, and assignment-tree behavior, so Phase 2 can extend that fixture set instead of introducing a new test structure.
  Evidence: `src/Vixen.Tests/Property/State/` contains `StateDataCloneTests`, `StateModuleCloneTests`, `StateMapperDraftTests`, and `StateAssignmentTreeTests`.

- Observation: The Phase 2 branch starts with a passing State-focused test baseline and a successful full Debug rebuild.
  Evidence: On 2026-06-01, `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State` passed 8 of 8 filtered tests, and `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` exited successfully.

## Decision Log

- Decision: Add `Guid Id` to `StateData` and expose it through `StateModule`.
  Rationale: The future State effect must persist a reference that survives renaming. The ID belongs to the overall attached property, not to its display name.
  Date: 2026-05-31

- Decision: Keep `StateData.Clone()` as a logical clone that preserves the overall State property ID and item IDs. Add an explicit copied-property operation that assigns a new overall property ID while retaining copied item values.
  Rationale: The setup dialog draft represents the same attached property. `CloneValues(IProperty)` represents a distinct destination property. Encoding both operations explicitly prevents accidental identity changes.
  Date: 2026-05-31

- Decision: Normalize missing or empty property IDs in `StateData.Normalize()` rather than adding migration code.
  Rationale: State has not shipped publicly. Normalization protects malformed in-memory or serialized data without creating a legacy migration path.
  Date: 2026-05-31

- Decision: Implement name validation using Catel validation primitives and show aggregate results through `catel:WarningAndErrorValidator`.
  Rationale: This matches repository conventions and gives field-level feedback plus a summary without popup dialogs.
  Date: 2026-05-31

- Decision: Validate and trim names when editing completes: on text field loss of focus, DataGrid cell commit, and before `OK` executes if a field is still active.
  Rationale: The user requested immediate feedback at field completion rather than validation deferred until save. The final command guard prevents invalid persistence if WPF focus ordering differs across interactions.
  Date: 2026-05-31

- Decision: Exact duplicate State item names are valid. Names that differ only by casing produce a non-blocking warning.
  Rationale: Future rendering intentionally activates all exact same-name items together, while case-only differences are valid but likely to be accidental.
  Date: 2026-05-31

- Decision: Generate xLights fallback names at the parser boundary, where XML context such as `s1` is still available.
  Rationale: Downstream models should remain valid, and the parser has the best information for deterministic fallback names.
  Date: 2026-05-31

- Decision: Preserve the original Phase 1 Jira content and append a Phase 2 prerequisite update.
  Rationale: The original issue history remains useful. This work tightens the property contract before VIX-3924 rather than replacing the initial implementation.
  Date: 2026-05-31

## Outcomes & Retrospective

Milestone 1 is complete. The Phase 2 Jira scope was recorded before implementation, the existing State-focused tests passed 8 of 8, and the full Debug rebuild succeeded. No code changes have started. Update this section after each subsequent milestone with test results, deviations from the design, and any follow-up work deferred to VIX-3924.

## Context and Orientation

Vixen properties are modules attached to element nodes. The State property lives under `src/Vixen.Modules/Property/State/`.

The persisted data model is `StateData` in `src/Vixen.Modules/Property/State/StateData.cs`. It stores the overall State property name, description, and a list of `StateItemData`. Each item has its own identifier, name, color, and referenced element-node identifiers. Phase 2 adds a stable identifier for the overall property.

The module wrapper is `StateModule` in `src/Vixen.Modules/Property/State/StateModule.cs`. It exposes data to the property module system and implements `CloneValues(IProperty)`. In this plan, a **logical clone** is a copy of the data used while editing or serializing the same attached property. A **copied property** is a new State property attached to a different element. Logical clones keep the property ID; copied properties receive a new property ID.

The setup dialog is under `src/Vixen.Modules/Property/State/Setup/`. `StateMapperViewModel` owns the editable draft and save/cancel commands. `StateItemViewModel` wraps each grid row. `StateMapperView.xaml` defines the form, State item DataGrid, and assignment tree. The dialog currently has no Catel validation summary and its name fields update on every keystroke.

xLights import parsing occurs in `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`. Parsed State definitions are later applied to element properties in `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs`. Phase 2 should keep that separation: normalize names while parsing and rely on normal State construction to create a new stable property ID when materializing an imported property.

Tests live in `src/Vixen.Tests/Property/State/`. Add importer normalization tests in the closest matching `Vixen.Tests` folder if the parser helper is directly testable. If this requires a new project reference, add only the narrow `CustomPropEditor.csproj` reference and follow the solution's existing project-reference conventions.

## Jira Update Markdown

Before implementation, append the following comment to Jira issue `VIX-3591`. Keep the original Phase 1 issue description and prior implementation notes intact.

```markdown
## Phase 2 Prerequisite Update for VIX-3924

### Requirements

Harden the VIX-3591 State property before implementing the VIX-3924 State effect.

- Add a persisted, non-empty stable identifier to each attached State property.
- Keep the State property identifier unchanged when the same property is renamed, edited, serialized, or logically cloned for an edit draft.
- Assign a new State property identifier when a property is newly created, imported from xLights, or copied from one element to another.
- Normalize a missing or empty State property identifier to a newly generated identifier. State has not shipped publicly, so no legacy migration path is required.
- Require the overall State property name to contain at least one non-whitespace character. Trim whitespace when the field edit completes.
- Show a non-blocking informational warning when the trimmed overall State property name is shorter than three characters.
- Require every State item name to contain at least one non-whitespace character. Trim whitespace when the DataGrid cell edit completes.
- Allow exact duplicate State item names because same-name items intentionally activate together.
- Treat State item names as case-sensitive, but show a non-blocking informational warning when names differ only by casing.
- Ensure every newly added State item starts with a valid non-whitespace default name.
- Show Catel validation feedback in a summary section at the top of the State mapper dialog and retain field-level visual validation.
- Disable `OK` while blocking validation errors exist. Keep `Cancel` and window close enabled so edits can be discarded.
- Avoid popup validation dialogs. Informational warnings do not require acknowledgement and do not disable `OK`.
- During xLights import, trim State names and generate best-effort fallback names for empty or whitespace-only values. Use the XML State item tag, such as `s1`, as the item fallback. Use a deterministic numbered fallback for an overall State property name when no useful source context exists.

### High-Level Design

Add `Guid Id` to the persisted `StateData` model and expose it through `StateModule`. Keep `StateData.Clone()` as the logical-clone operation for the same attached property, preserving the property ID and item IDs. Make the `StateModule.CloneValues(IProperty)` cross-property copy path explicit so it copies values but assigns a new destination property ID.

Use Catel model validation in the State mapper ViewModels. Validate and trim names when editing completes, aggregate row validation into the parent dialog, place `catel:WarningAndErrorValidator` at the top of the form, and make the `OK` command executable only when no blocking validation errors exist. Revalidate immediately before save so an active editor cannot bypass validation.

Normalize xLights State names at the XML parsing boundary, before parsed State models are materialized into Vixen properties. Imported State properties receive stable IDs through normal State property construction.

### Acceptance Criteria

- A newly created State property has a non-empty stable ID.
- Editing, renaming, saving, reopening, and logical cloning a State property preserve its ID.
- Copying a State property from one element to another creates a distinct non-empty destination ID.
- Assigning State data with an empty ID normalizes it to a new non-empty ID.
- Invalid overall State property and State item names show field-level Catel validation and a summary message at the top of the dialog.
- Completing a name edit trims leading and trailing whitespace immediately.
- `OK` is disabled while blocking validation errors exist and becomes enabled after correction.
- `Cancel` and window close remain available while validation errors exist and discard edits.
- Overall names shorter than three characters show a non-blocking warning.
- Exact duplicate State item names remain valid.
- State item names that differ only by casing show a non-blocking warning.
- New State item rows always begin with a valid default name.
- xLights State imports produce valid trimmed names and deterministic fallback names without interactive prompts.
- Focused unit tests cover identity, clone semantics, validation, warnings, save/cancel behavior, and import name normalization.
- The solution builds in Debug and Release.

### Testing Steps

1. Run `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State`.
2. Build with `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`.
3. Start Vixen from `Debug\Output`.
4. In Display Setup, add or edit a State property and verify the generated default overall name and item names are valid.
5. Clear the overall State property name, leave the field, and verify a field error and top-of-form summary appear and `OK` is disabled.
6. Enter a valid value with surrounding whitespace, leave the field, and verify it is visibly trimmed and `OK` is enabled when no other errors exist.
7. Enter a one- or two-character overall name and verify the informational warning appears without disabling `OK`.
8. Clear a State item name, commit the DataGrid cell by moving to another row, and verify field and summary errors appear and `OK` is disabled.
9. Add exact duplicate State item names and verify they remain valid.
10. Add names that differ only by casing and verify a non-blocking informational warning appears.
11. While errors exist, press `Cancel` and reopen the State property to verify the original property remains unchanged.
12. Import an xLights model containing blank State property and item names and verify deterministic fallback names are created without prompts.
13. Build with `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release`.
```

## Plan of Work

### Milestone 1: Record Jira Scope and Establish the Baseline

Add the markdown above to Jira issue `VIX-3591` before code changes. Record the Jira comment URL or timestamp in `Progress`.

Run the existing targeted State tests and a Debug build before editing. This establishes whether the branch starts clean and prevents pre-existing failures from being attributed to Phase 2.

Commands:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
```

If the build environment cannot restore packages because network access is unavailable, record that fact and run the most complete local verification possible.

### Milestone 2: Add Stable State Property Identity

Update `src/Vixen.Modules/Property/State/StateData.cs`:

- Add a persisted public `Guid Id` property with XML documentation.
- Initialize `Id` with `Guid.NewGuid()` for newly constructed data.
- Preserve `Id` in `Clone()`.
- Normalize `Guid.Empty` to a newly generated identifier in `Normalize()`.
- Add an explicit internal copied-property clone helper, such as `CloneForNewProperty()`, that deep clones the data and replaces only the overall property `Id`.

Update `src/Vixen.Modules/Property/State/StateModule.cs`:

- Expose the overall stable property identifier through a documented public `Guid Id` property.
- Keep module data normalization in the `ModuleData` setter.
- Change `CloneValues(IProperty)` to use the copied-property clone helper so the destination receives a new overall property ID.
- Preserve item IDs and copied values unless implementation research identifies a concrete reason item IDs must change. The Phase 2 requirement changes the attached State property identity, not the identities of copied item definitions.

Do not add migration code. Normalization is sufficient because the feature has not shipped publicly.

Extend tests under `src/Vixen.Tests/Property/State/`:

- A new `StateData` has a non-empty ID.
- `StateData.Clone()` preserves the property ID and deep clones nested items.
- Assigning data with `Guid.Empty` through the module data path normalizes the ID.
- `StateModule.CloneValues()` creates a non-empty destination ID distinct from the source while copying names, descriptions, items, colors, and assignments.
- Editing through the setup draft and saving keeps the source property ID unchanged.
- Cancelling draft edits keeps the original property and ID unchanged.

Expected result: the future State effect can safely persist `StateModule.Id`, and every copy workflow using `CloneValues` produces a distinct attached property identity.

### Milestone 3: Normalize xLights Imported State Names

Update the xLights parsing path under `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/`.

Introduce a small normalization helper at the parser boundary. Keep it close to `XModelImport` unless a reusable parser model is clearly cleaner after implementation inspection. The helper must:

- Trim valid overall State property names.
- Generate a deterministic numbered overall fallback, such as `State Name 1`, when the imported name is empty or whitespace-only and no useful source context exists.
- Trim valid State item names.
- Generate a State item fallback from the XML attribute tag, such as `s1`, when the imported item name is empty or whitespace-only.

Update `XModelImport.cs` so normalization occurs before `StateInfo` and `StateItem` values flow into assembled State definitions. Capture the XML tag while parsing because downstream models do not retain that context.

Review `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs` and confirm that imported properties are materialized through normal State module creation. The `StateData` constructor should assign the imported property a new non-empty ID. Only change the builder if that invariant is not true in practice.

Add focused tests for the normalization helper or parser path:

- Valid names are trimmed.
- Blank overall names receive deterministic numbered fallbacks.
- Blank item names receive XML-tag fallbacks.
- Valid XML-tag fallback names remain stable across repeated parsing.

Prefer a parser-level test. If direct helper tests are substantially simpler, keep the helper internal where possible and add the narrowest test visibility mechanism used by the solution. If a new test project reference is required, add only:

```xml
<ProjectReference Include="..\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj" />
```

Match the conventions already present in `src/Vixen.Tests/Vixen.Tests.csproj`.

Expected result: imported State definitions are valid before they reach the Vixen property model, and imports remain non-interactive.

### Milestone 4: Add Catel Validation to the State Mapper

Update `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs`:

- Enable validation before the first save attempt with `DeferValidationUntilFirstSaveCall = false`.
- Trim the overall State property name when its edit completes.
- Override Catel validation methods to report:
  - A blocking field error when the overall State property name is empty after trimming.
  - A non-blocking field warning when the trimmed overall State property name has fewer than three characters.
  - Aggregated blocking State item name errors.
  - An aggregated non-blocking warning when distinct State item names differ only by casing.
- Keep exact duplicate item names valid.
- Ensure `OkCommand` can execute only when no blocking validation errors exist.
- Revalidate before saving to prevent an active editor from bypassing validation due to WPF focus timing.
- Raise command executability when field values, item values, item validation state, or the item collection change.
- Keep `CancelCommand` executable regardless of validation state.

Update `src/Vixen.Modules/Property/State/Setup/ViewModels/StateItemViewModel.cs`:

- Use Catel model validation for row-level name validation so the DataGrid can show a field-level visual error.
- Trim State item names when cell editing completes.
- Report a blocking field error for empty trimmed names.
- Preserve the existing item ID, color, and assignment behavior.

Keep `StateAssignmentTreeNode` on its existing lightweight bindable base unless implementation shows a concrete need to change it.

Before coding warnings, verify the exact warning factory exposed by the restored Catel package. Repository code demonstrates `FieldValidationResult.CreateError(...)`; use the corresponding Catel warning factory or supported severity constructor for informational warnings. Record the exact API in `Surprises & Discoveries` if it differs from the expected `CreateWarning(...)` shape.

Update `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml`:

- Add a top-of-form `<catel:WarningAndErrorValidator Source="{Binding}" />` section.
- Change the overall State property name binding to update on loss of focus.
- Change the State item DataGrid name binding to update when cell editing completes.
- Preserve field-level visual validation through Catel/WPF validation bindings.
- Keep the existing layout, color chooser, assignment count, assignment tree, and disabled-descendant styling intact.

Add focused ViewModel tests:

- Existing invalid data is validated when the dialog opens.
- Overall name whitespace is trimmed on edit completion.
- An empty overall name blocks `OK`.
- Correcting the overall name re-enables `OK` when no other errors exist.
- A one- or two-character overall name creates a warning but does not block `OK`.
- A new row starts with a valid default name.
- An empty item name blocks `OK`.
- Correcting the item name re-enables `OK`.
- Exact duplicate item names are accepted.
- Case-only item-name differences create a warning but do not block `OK`.
- `Cancel` remains available while errors exist and leaves the source unchanged.
- Saving preserves the overall State property ID.

Avoid UI automation for validation rules that can be proven in ViewModel tests. Use the manual scenarios below to verify WPF focus, DataGrid commit, summary display, and button state wiring.

Expected result: invalid State definitions cannot be saved, validation occurs at field completion, and users can cancel without popup dialogs.

### Milestone 5: Verify the Integrated Behavior

Run the focused tests:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State
```

If importer tests use a name that does not include `State`, run them explicitly as well:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter XModelImport
```

Build both configurations:

```powershell
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
```

Perform the manual Display Setup and import scenarios from the Jira update. In addition, verify:

- Clicking `OK` while a currently focused field contains whitespace-only text commits, trims, validates, and does not save.
- Moving between DataGrid rows commits and validates the previous row name.
- Closing the window with validation errors discards draft changes.
- Copying a State property still copies visible configuration values.
- Reopening copied properties shows the expected values and does not surface validation errors for generated defaults.

Record exact commands and outcomes in `Progress`. If any behavior deviates from this plan, update `Decision Log` before changing the implementation direction.

## Concrete Steps

Run commands from the repository root:

```powershell
Set-Location C:\Dev\Vixen
git status --short
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
```

Implement milestones in order. After each milestone, inspect the scoped diff:

```powershell
git diff -- src/Vixen.Modules/Property/State src/Vixen.Modules/App/CustomPropEditor/Import/XLights src/Vixen.Modules/Preview/VixenPreview src/Vixen.Tests
```

Run targeted tests after each behavior change:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State
```

At the end, run:

```powershell
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
git status --short
```

## Validation and Acceptance

Implementation is complete when:

- The Jira Phase 2 update has been added to VIX-3591.
- The original `docs/plans/vix-3591-state-feature.md` remains unchanged.
- Every State property has a normalized, non-empty stable ID.
- Logical clones preserve property identity and cross-element copies create new property identity.
- xLights imports normalize names and generate deterministic fallbacks.
- The mapper validates on field completion, summarizes Catel errors and warnings at the top of the form, and disables only `OK` for blocking errors.
- Unit tests cover the new model, module, ViewModel, and import normalization behavior.
- Debug and Release builds pass.
- Manual Display Setup validation, cancellation, copy, and xLights import scenarios pass.

## Idempotence and Recovery

The implementation is additive and can be applied incrementally.

- `StateData.Normalize()` may run repeatedly. It must generate a new ID only when `Id == Guid.Empty`.
- Name normalization may run repeatedly. Trimming and valid-name fallback generation must be deterministic after the first normalization.
- `CloneValues(IProperty)` intentionally creates a new destination property ID on each invocation because it represents a fresh copy operation.
- `StateData.Clone()` must never generate a new property ID because it represents the same logical property.
- The setup dialog edits a draft. If implementation is interrupted or validation fails, cancelling or closing the dialog must leave the source model untouched.
- Do not modify the original Phase 1 plan while updating this living Phase 2 document.

## Artifacts and Notes

Applicable project skills reviewed while preparing this plan:

- `.agents/skills/dotnet-best-practices/SKILL.md`
- `.agents/skills/csharp-docs/SKILL.md`
- `.agents/skills/dotnet-design-pattern-review/SKILL.md`
- `.agents/skills/catel-mvvm/SKILL.md`
- `.agents/skills/csharp-async/SKILL.md`

The implementation changes public State APIs, so update XML documentation in the same change. The setup commands are already asynchronous Catel commands; preserve that pattern and do not introduce synchronous blocking or `async void`.

## Interfaces and Dependencies

At the end of Milestone 2, `StateData` should expose this documented public property:

```csharp
public Guid Id { get; set; }
```

At the end of Milestone 2, `StateModule` should expose this documented public property:

```csharp
public Guid Id { get; }
```

The exact setter visibility on `StateModule.Id` should remain as narrow as the module API permits. External consumers such as the future VIX-3924 effect need to read the ID, while identity mutation belongs inside construction, normalization, and copy semantics.

The copied-property operation should remain internal to the State model or module. Prefer an explicit helper such as:

```csharp
internal StateData CloneForNewProperty()
```

The helper must deep clone nested data, preserve State item IDs and values, and assign a new overall property ID.

For Catel validation, use the restored package's supported `FieldValidationResult` and `BusinessRuleValidationResult` factories or constructors. Keep validation logic in ViewModels and model wrappers, not in XAML code-behind.

No new runtime package is expected. A narrow `Vixen.Tests` project reference to `CustomPropEditor.csproj` is acceptable only if needed to test importer normalization directly.

## Revision Notes

- 2026-05-31: Created the Phase 2 prerequisite ExecPlan after reviewing the refined requirements, original VIX-3591 plan, State module code, copy pathways, xLights parsing/materialization path, Catel validation examples, and existing State tests. The original Phase 1 plan was intentionally left unchanged.
- 2026-06-01: Completed Milestone 1 after the user confirmed the Jira update was appended. Recorded the passing 8-test State baseline and successful full Debug rebuild so later implementation failures can be distinguished from pre-existing behavior.

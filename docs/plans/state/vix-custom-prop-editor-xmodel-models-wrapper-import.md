# Add xModel models-wrapper import selection to Custom Prop Editor

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Keep this document self-contained and revise it whenever implementation discoveries change the approach.

## Purpose / Big Picture

Users can currently import a single xLights custom model into the Custom Prop Editor when the `.xmodel` file starts with a `custommodel` element. Some xLights exports instead start with a `models` wrapper element and contain one or more child `model` elements. Today those files fail immediately with `Unsupported model type: models. Import only supports custom model types at this time.`

After this change, importing an `.xmodel` file with a root `models` wrapper will find its direct child `model` elements. If there is one model, the importer will import it directly. If there is more than one model, the user will see a selection dialog showing each model's `name` attribute, choose exactly one, and the selected model will continue through the same Custom Prop Editor xModel import flow used by single-model files. If the user cancels the selection dialog, import is canceled without replacing the current prop.

## Progress

- [x] (2026-07-16 21:49Z) Read `.agents/PLANS.md`, the current xModel importer, Custom Prop Editor state requirements, the reference `docs/references/GE Stardust 1500.xmodel`, existing Custom Prop Editor window patterns, and existing xModel importer tests.
- [x] (2026-07-16 21:49Z) Read project `catel-mvvm`, `dotnet-best-practices`, and `csharp-docs` skills for planning guidance because implementation will touch WPF view models, C# importer code, and likely public or protected APIs.
- [x] (2026-07-16 21:56Z) Incorporated the requirement that wrapped `<model DisplayAs="Custom">` elements are treated as custom models and parsed through the custommodel path.
- [x] (2026-07-16 22:00Z) Incorporated the requirement that `modelGroup` tags inside imported models are ignored with info-level log entries and no user-facing warning.
- [x] (2026-07-16 22:03Z) Confirmed Jira issue key `VIX-3943` and updated this plan with the exact key.
- [x] (2026-07-16 22:05Z) Updated Jira issue `VIX-3943` description with background, requirements, acceptance criteria, test plan, and implementation-plan reference.
- [x] (2026-07-16 22:09Z) Incorporated the clarification that non-custom `DisplayAs` values resolve to their corresponding model type and should be handled as unsupported unless that type is implemented.
- [x] (2026-07-16 22:28Z) Completed Milestone 2 parser refactor: the importer now loads the XML into an `XDocument`, routes root `custommodel` elements through a reusable `XElement` custom-model parser, treats `model DisplayAs="Custom"` as custommodel when passed to that parser, resolves non-custom `DisplayAs` values to unsupported model types, and logs ignored `modelGroup` elements at info level.
- [x] (2026-07-16 22:35Z) Completed Milestone 3 wrapper discovery and selection routing: root `models` imports enumerate direct child `model` elements, build stable selection items with name fallback and duplicate disambiguation, auto-import a single supported custom model, report unsupported all-non-custom wrappers through the normal unsupported-model path, route selected supported models through the reusable import flow, and cancel when the selection service returns no selection.
- [ ] Add focused automated tests for wrapper import, single-child wrapper import, multi-child wrapper selection, cancel behavior, and non-wrapper regression.
- [ ] Run focused tests, full Custom Prop Editor tests as practical, and final build/check commands.
- [ ] Record implementation results in `Outcomes & Retrospective`.

## Surprises & Discoveries

- Observation: The reference file `docs/references/GE Stardust 1500.xmodel` starts with `<models type="exported">` and contains one direct child `<model name="GE Stardust 1500" DisplayAs="Custom" ...>`. It also contains nested `modelGroup` and `dimensions` elements inside the child model, but those are not importable model choices.
  Evidence: The first lines of the reference file show the wrapper root and a single direct child model; the closing lines show `modelGroup` and `dimensions` children before `</model></models>`.
- Observation: The current importer only accepts a root element named `custommodel`; every other root element goes to the unsupported model type error.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs` checks `if ("custommodel".Equals(reader.Name) && reader.HasAttributes)` and otherwise shows `Unsupported model type: {reader.Name}`.
- Observation: Existing focused tests already exercise xModel import under `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelImportHierarchyTests.cs` using embedded temporary `.xmodel` strings rather than external reference files.
  Evidence: `XModelImportHierarchyTests.ImportAsync` writes the supplied XML string to a temporary `.xmodel` file and calls `new XModelImport().ImportAsync(filePath)`.
- Observation: The parser can be refactored to `XDocument` without changing existing direct custommodel behavior.
  Evidence: After the refactor, `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore` passed all 20 focused xLights importer tests.
- Observation: The Custom Prop Editor project exposes internals to `Vixen.Tests`, so the xModel selection boundary can remain internal and still be faked by automated tests.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/CustomPropEditor.csproj` contains `InternalsVisibleToAttribute` for `Vixen.Tests`.

## Decision Log

- Decision: Preserve the existing single-model import behavior and add wrapper support as a narrow pre-processing step.
  Rationale: The user asked for wrapper support only when a root `models` tag exists. Non-wrapper xModel files must keep using the current path so existing `custommodel`, submodel, faceInfo, stateInfo, `CustomModel`, and `CustomModelCompressed` behavior is not disturbed.
  Date/Author: 2026-07-16 / Codex
- Decision: Treat direct child `model` elements of a root `models` element as candidate choices, and do not treat nested `modelGroup` elements as model choices.
  Rationale: The requested wrapper shape is root `models` containing embedded model tags. The reference file has direct child `model` and nested `modelGroup`; only the direct child `model` has the attributes used by the current custom model import flow.
  Date/Author: 2026-07-16 / Codex
- Decision: Display the `name` attribute as the selection identifier, with a deterministic fallback such as `Unnamed model 1` only if the attribute is missing or blank.
  Rationale: The request says to use the model name for the identifier. A fallback prevents a blank dialog row and makes malformed files testable.
  Date/Author: 2026-07-16 / Codex
- Decision: If the wrapper contains exactly one direct child `model`, import it without prompting. If it contains more than one, prompt. If the prompt is canceled, return `null` from `ImportAsync`.
  Rationale: The request only requires asking when there is more than one model, and the existing `PropEditorViewModel.ImportProp` already leaves the current prop unchanged when `ImportAsync` returns `null`.
  Date/Author: 2026-07-16 / Codex
- Decision: Introduce a testable selection abstraction instead of hard-coding dialog construction inside parser logic.
  Rationale: The importer is covered by unit tests and currently can be constructed directly. A small service interface lets tests choose or cancel without showing UI while the production path can use Catel dialog services.
  Date/Author: 2026-07-16 / Codex
- Decision: Treat a wrapped `<model>` element with `DisplayAs="Custom"` as a custom model and attempt to parse it using the same logic as a standalone `<custommodel>` root.
  Rationale: xLights wrapper exports identify the model type through the `DisplayAs` attribute rather than by using the `custommodel` element name. The user clarified that `DisplayAs="Custom"` is the signal to assume custommodel behavior.
  Date/Author: 2026-07-16 / Codex
- Decision: Ignore `modelGroup` elements encountered inside the selected model, log one info-level message for each ignored `modelGroup`, and do not show any user-facing warning.
  Rationale: The user clarified that xLights model groups are not supported by this import improvement. Logging preserves diagnostic evidence without interrupting a successful single-model import.
  Date/Author: 2026-07-16 / Codex
- Decision: Preserve the original Jira description's `circlemodel` report as explicit out-of-scope context in `VIX-3943`.
  Rationale: The issue description already mentioned a separate unsupported `circlemodel` failure. Milestone 1 should not silently erase that report, but the current requirements only cover root `models` wrappers with `DisplayAs="Custom"` embedded models.
  Date/Author: 2026-07-16 / Codex
- Decision: Map non-custom wrapped `model` elements by `DisplayAs` to their corresponding xLights model type and handle them through the normal unsupported-model path when that type is not implemented.
  Rationale: The user clarified that `DisplayAs="Circle"` resolves to `circlemodel`, and `circlemodel` is not supported today. This keeps wrapped non-custom models consistent with standalone unsupported model roots instead of silently filtering or pretending they are custom models.
  Date/Author: 2026-07-16 / Codex
- Decision: Use `XDocument`/`XElement` for the importer refactor.
  Rationale: Wrapper selection needs to pass a chosen embedded model through the same parse path as a root custommodel. Element-based parsing keeps that path reusable while preserving the existing attribute and child-element semantics.
  Date/Author: 2026-07-16 / Codex
- Decision: Keep the selection contract internal and injectable through an internal settable importer property until the WPF dialog is added.
  Rationale: Milestone 3 needs testable selection routing without changing the public importer API. The production selection service intentionally returns no selection until Milestone 4 provides the dialog implementation.
  Date/Author: 2026-07-16 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. Jira issue `VIX-3943` now contains the scoped requirements, acceptance criteria, and test plan for importing wrapped custom xModels. The description preserves the original `circlemodel` report as out-of-scope context so future work can address it separately if desired.

Milestone 2 is complete. `XModelImport` now has a reusable custom-model import method that accepts an `XElement`, allowing a future selected wrapper child model to follow the same code path as a standalone `custommodel`. Existing direct custommodel tests pass after the refactor, and `modelGroup` children are ignored with info-level logging.

Milestone 3 is complete. `XModelImport` now detects a root `models` element, gathers direct child `model` elements, creates internal `XModelSelectionItem` values for selection, imports a single supported custom model without prompting, reports unsupported all-non-custom wrappers through the normal unsupported model path, and routes multi-model wrappers through `IXModelSelectionService`. The default production service returns no selection until Milestone 4 adds the user-facing dialog.

## Clarifying Questions

The implementation can proceed with the defaults recorded in the Decision Log, but these questions should be answered before or during Milestone 1 if possible.

1. The Jira issue key is `VIX-3943`.
2. Resolved: only `DisplayAs="Custom"` child models are eligible for custommodel import. Child models with other `DisplayAs` values resolve to their corresponding model type, such as `DisplayAs="Circle"` resolving to `circlemodel`, and should be handled as unsupported until that model type is implemented.
3. If a wrapper contains duplicate model names, should the dialog show duplicates as-is, or append disambiguating details such as `Name (2)` or the child index? This plan defaults to preserving the name and appending an index only for display if duplicates would otherwise be indistinguishable.
4. Should a wrapper containing zero child `model` elements show a new wrapper-specific error, or reuse the existing unsupported/invalid import messaging? This plan defaults to showing a clear `No models were found in the xModel file.` error.

## Context and Orientation

The Custom Prop Editor is a WPF app module under `src/Vixen.Modules/App/CustomPropEditor`. Users open it from Vixen and import `.xmodel` files through the File menu item `Import xModel` in `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`. That menu item calls `PropEditorViewModel.ImportCommand` in `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`. The command asks for a file, shows a busy indicator, and calls `ImportProp(path)`. `ImportProp` constructs `IModelImport import = new XModelImport();`, awaits `import.ImportAsync(path)`, and replaces the editor's current `Prop` only when the returned value is not null.

The xLights importer lives in `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`. The public entry point is `Task<Prop> ImportAsync(string filePath)` from `src/Vixen.Modules/App/CustomPropEditor/Import/IModelImport.cs`. The current importer reads the file with `XmlReader`, calls `MoveToContentAsync`, and imports only when the root element name is `custommodel`. It creates a `CustomModel`, reads attributes such as `name`, `parm1`, `parm2`, `PixelSize`, `StringType`, `StrandNames`, `NodeNames`, `CustomModelCompressed`, and `CustomModel`, then reads child `subModel`, `faceInfo`, and `stateInfo` elements. It then calls `Assemble(cm)` to create the Custom Prop Editor `Prop` tree. That `Assemble` method is the normal import flow this plan must preserve.

An xLights `.xmodel` file may also have a root `models` element. In this plan, a `models wrapper` means a root XML element literally named `models`, as in `<models type="exported">`. An `embedded model` means a direct child element named `model` under that root. A `custom model` means a standalone `custommodel` root or a wrapped `model` element with `DisplayAs="Custom"` whose attributes can be parsed by the existing importer into a grid of pixel nodes using `CustomModelCompressed` or `CustomModel`. A non-custom wrapped model means a wrapped `model` element whose `DisplayAs` value is something other than `Custom`; that value resolves to the corresponding xLights model type, for example `DisplayAs="Circle"` resolves to `circlemodel`. Non-custom wrapped models are not made importable by this issue unless the resolved model type is already supported. A `modelGroup` is an xLights grouping element that can appear inside a model; this issue does not add import support for model groups.

The reference file for this issue is `docs/references/GE Stardust 1500.xmodel`. It has one embedded model with `name="GE Stardust 1500"` and `DisplayAs="Custom"`. Even though this reference wrapper contains only one model and therefore should not prompt, it proves that root `models` should not be treated as an unsupported model type.

Existing automated tests for this importer are in `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelImportHierarchyTests.cs`. They use embedded XML strings written to temporary files, call `new XModelImport().ImportAsync(filePath)`, and assert the resulting `Prop` tree. New tests should follow that pattern and should not depend on the large reference file.

## Plan of Work

Milestone 1 updates Jira issue `VIX-3943`. Update the Jira issue description with a concise version of this plan: the problem statement, the root `models` wrapper behavior, single-child behavior, multi-child selection behavior, cancel behavior, modelGroup skip logging, acceptance criteria, and automated/manual test plan. If using the project Jira skill, read `.agents/skills/jira/SKILL.md` first and use the Atlassian tooling it prescribes. Record any wording changes in this ExecPlan's Progress and Decision Log.

Milestone 2 extracts the current single-model parser into a reusable internal path. In `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`, keep `ImportAsync(string filePath)` as the public entry point. Refactor the body so the code that imports a single custom model element can be called for either a root `custommodel` element or a selected child `model` element from a wrapper. A wrapped child `model` is eligible for this custom path when it has `DisplayAs="Custom"`; in that case, assume custommodel semantics and parse the model attributes and children exactly as the standalone custommodel path does. If a wrapped child `model` has a non-custom `DisplayAs` value, resolve that value to the corresponding xLights model type for error messaging, such as `Circle` to `circlemodel`, and handle it like any other unsupported model type. Do not attempt to parse non-custom wrapped models through the custommodel path. One practical approach is to load the XML with `XDocument` or `XmlDocument` for wrapper discovery and then pass a selected `XElement` into a new private method such as `ImportModelElementAsync(XElement modelElement)`. That method should normalize accepted custom model elements so both `custommodel` and wrapped `model DisplayAs="Custom"` use the same attribute and child-element parsing code. During child-element parsing, explicitly ignore any `modelGroup` elements. For each ignored `modelGroup`, write an info-level NLog entry that includes the imported model name and the modelGroup name when present, for example `Skipping xModel modelGroup {ModelGroupName} in model {ModelName}; modelGroup import is not supported.` Do not show a warning, error, or confirmation dialog for skipped model groups. Use structured NLog logging for wrapper parse errors. Keep method names async when they await work. If any new public or protected APIs are added, document them with XML comments following `.agents/skills/csharp-docs/SKILL.md`.

Milestone 3 adds model-wrapper discovery and selection. In the importer, when the root element is `models`, enumerate direct child elements whose local name is `model`. Classify each candidate by its `DisplayAs` attribute. Treat `DisplayAs="Custom"` as ordinal-ignore-case unless existing xLights import code clearly uses a different convention. For custom candidates, build a small immutable DTO such as `XModelSelectionItem` in its own file under `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/` with an index and display name, or keep it internal if it is only used by the importer and selection service. If the wrapper has exactly one direct child model and that child is non-custom, resolve its `DisplayAs` to the unsupported model type and show the normal unsupported model error for that type. If the wrapper has multiple direct child models, show the selection dialog with enough type information that unsupported non-custom entries are identifiable; selecting a non-custom model should show the normal unsupported model error and cancel import. If only custom candidates are offered in the first implementation for simplicity, the importer must still produce the normal unsupported model error when the wrapper contains no custom candidates but does contain non-custom models. If there are no model children at all, show the no-models-found error and return null. If exactly one custom candidate exists, select it automatically only when there are no other direct child model candidates that would require a user choice; otherwise prompt so the user can choose among embedded models. If the service returns no selection because the user canceled, return null. After a custom candidate is selected, pass that selected model element into the same single-model import method from Milestone 2. Do not import multiple models into one prop, because the Custom Prop Editor can only edit one model at a time.

Milestone 4 adds the selection UI through a testable service. Create an interface such as `IXModelSelectionService` and a production implementation under Custom Prop Editor services or an `Import/XLights` UI-adjacent folder, following existing namespace conventions. The interface should expose a Task-returning method that receives the model choices and returns the selected choice or null for cancel. The production implementation should use Catel MVVM patterns: a ViewModel derived from `ViewModelBase`, constructor-injected services where practical, `Command` or `TaskCommand` for OK and Cancel, Catel registered properties for the selected item and items collection, and no business logic in code-behind. The view should be a small WPF dialog listing names in a ListBox or ComboBox with OK and Cancel buttons. The dialog text should make clear that only one embedded model can be imported. Register or resolve this service consistently with the existing Custom Prop Editor dependency patterns. If full Catel `IUIVisualizerService` registration is too invasive for this module, follow the existing Custom Prop Editor precedent but record the reason in the Decision Log.

Milestone 5 extends automated tests. Add tests to `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelImportHierarchyTests.cs` or a new one-class-per-file test class in the same namespace. Use embedded minimal XML strings, not `docs/references/GE Stardust 1500.xmodel`. Add a fake selection service that returns a chosen item or null. Cover these behaviors: a wrapper with one child model imports without prompting and creates the same tree as a direct `custommodel`; a wrapper with two child models calls the selection service and imports only the selected model; canceling selection returns null; a direct `custommodel` still imports as before; a wrapper with zero child model elements returns null and reports a clear error; duplicate display names are disambiguated or at least still selectable by stable index. Add a focused `modelGroup` test using a minimal wrapped custom model containing one or more `modelGroup` children. Assert that import still succeeds and no extra Custom Prop Editor nodes are created from the `modelGroup` tags. If the project's test infrastructure can capture NLog output without brittle setup, assert one info-level log entry per ignored modelGroup. If log capture is impractical, record the reason in this plan and cover the logging behavior with a code-review or manual acceptance check. Existing tests should continue to pass unchanged.

Milestone 6 performs manual validation. Start Vixen, open Custom Prop Editor, choose `File > Import xModel`, and select `docs/references/GE Stardust 1500.xmodel`. Because the reference wrapper contains one embedded model, no selection dialog should appear. The import should create a prop named `GE Stardust 1500 {1}` with a model child named `GE Stardust 1500 {1} - Model`, submodels such as `All_Pixels` and `Half_Left`, and no unsupported model type error. For a hand-created test file containing two direct child `model` elements, the selection dialog should appear. Choosing one should import only that model. Canceling should leave the currently open prop unchanged.

## Concrete Steps

Before implementation, confirm the Jira key and update Jira. From `C:\Dev\Vixen`, inspect the working tree:

    git status --short

Read the project Jira skill if the Jira update is performed by an agent:

    Get-Content -Path .agents\skills\jira\SKILL.md

Update Jira issue `VIX-3943` with the following description content:

    Summary:
    Add support for importing xLights .xmodel files whose root element is a models wrapper. The Custom Prop Editor currently only accepts a root custommodel element and shows "Unsupported model type: models" for wrapped exports.

    Requirements:
    - Detect a root <models> element during xModel import.
    - Enumerate direct child <model DisplayAs="Custom"> elements as embedded import candidates.
    - Use each model element's name attribute as the selection identifier, with a deterministic fallback for blank names.
    - If the wrapper contains one model, import it without prompting.
    - If the wrapper contains more than one model, show a selection dialog and allow the user to choose exactly one.
    - If the user cancels the selection dialog, cancel import and leave the currently open prop unchanged.
    - Treat each chosen <model DisplayAs="Custom"> element as a custommodel and route it through the existing xModel custom model import flow so CustomModelCompressed, CustomModel, subModel, faceInfo, stateInfo, Model Type, and StateDefinitionModels behavior remains unchanged.
    - Treat a non-custom DisplayAs value as the corresponding xLights model type, for example DisplayAs="Circle" resolves to circlemodel.
    - Handle non-custom wrapped models through the normal unsupported-model path unless that resolved model type is implemented.
    - Ignore modelGroup tags inside the selected model because modelGroup import is not supported.
    - Log one info-level message for each ignored modelGroup, including the model name and modelGroup name when available.
    - Do not show a user-facing warning or error for ignored modelGroup tags.
    - Continue supporting existing non-wrapper .xmodel files.
    - Do not add support for importing multiple embedded models into a single prop in this issue.

    Acceptance Criteria:
    - Importing docs/references/GE Stardust 1500.xmodel no longer shows "Unsupported model type: models".
    - A wrapper containing a single child model with DisplayAs="Custom" imports that model directly.
    - A wrapper containing multiple child models with DisplayAs="Custom" prompts the user to choose one by model name.
    - Choosing a model imports only the selected model.
    - Canceling the prompt cancels import without changing the current prop.
    - Existing direct custommodel imports behave the same as before.
    - Existing submodel, faceInfo, stateInfo, CustomModel, and CustomModelCompressed behavior is unchanged for the selected model.
    - A wrapped model with DisplayAs="Circle" is reported as unsupported circlemodel and is not parsed as a custom model.
    - modelGroup tags do not create imported Custom Prop Editor nodes, and each skipped modelGroup is recorded in the info log only.

    Test Plan:
    - Add unit tests with embedded minimal xModel XML for single-child wrappers, multi-child wrappers, cancel behavior, zero-child wrappers, duplicate names, ignored modelGroup tags, non-custom DisplayAs unsupported behavior, and direct custommodel regression.
    - Run dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights"
    - Run dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor"
    - Run git diff --check.
    - Manually import docs/references/GE Stardust 1500.xmodel and a small two-model wrapper file in the Custom Prop Editor.

Then implement the code changes. Keep edits scoped to these likely files unless implementation discoveries require otherwise:

    src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs
    src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelSelectionItem.cs
    src/Vixen.Modules/App/CustomPropEditor/Import/XLights/IXModelSelectionService.cs
    src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelSelectionService.cs
    src/Vixen.Modules/App/CustomPropEditor/ViewModels/XModelSelectionViewModel.cs
    src/Vixen.Modules/App/CustomPropEditor/Views/XModelSelectionView.xaml
    src/Vixen.Modules/App/CustomPropEditor/Views/XModelSelectionView.xaml.cs
    src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelImportHierarchyTests.cs

Run focused tests from `C:\Dev\Vixen`:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore

If the focused test filter does not discover all tests due to namespace naming, run the broader Custom Prop Editor filter:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor" --no-restore

Before final acceptance, run:

    git diff --check
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor" --no-restore

If time and local dependencies allow, run the full suite:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

## Validation and Acceptance

Automated validation succeeds when the focused xLights importer tests pass and demonstrate the following behavior. A root `custommodel` file still imports. A root `models` file with one direct child `model DisplayAs="Custom"` imports without selection when there are no other model candidates. A root `models` file with multiple direct child model elements invokes the fake selection service and imports only the selected custom model's name, pixel nodes, submodels, faces, and states. A canceled selection returns null from `ImportAsync`. A root `models` file with no direct child model elements returns null and shows or records a clear error. A root `models` file with only non-custom child models, such as `DisplayAs="Circle"`, resolves the selected or only model to the corresponding unsupported model type, such as `circlemodel`, and handles it like any other unsupported model. A selected custom model with `modelGroup` children imports successfully, creates no modelGroup-derived nodes, logs one info message per ignored modelGroup, and does not show a user warning.

Manual validation succeeds when importing `docs/references/GE Stardust 1500.xmodel` through the Custom Prop Editor no longer shows the unsupported model type error and produces a usable prop. The resulting prop should have the imported model's name, a primary model child, and the same normal imported structures as if the selected child model had been a standalone custommodel file. A hand-authored wrapper file with two embedded `model DisplayAs="Custom"` elements should show a dialog with both names. Choosing the second model should produce a prop named from the second model, not the first. Canceling that dialog should leave the currently loaded prop unchanged.

Jira validation succeeds when `VIX-3943` contains the requirements, acceptance criteria, and test plan in Milestone 1, and this ExecPlan records the update date.

## Idempotence and Recovery

The code changes are additive and refactoring-oriented. Re-running tests should not leave persistent files because importer tests should create temporary `.xmodel` files and delete them in `finally` blocks, matching existing tests. If a temporary file remains after a failed test, delete only that generated file from the system temp directory after confirming its generated name.

Do not modify `docs/references/GE Stardust 1500.xmodel`; it is a reference artifact. Do not use it as a unit test fixture because it is large and reference files can change for documentation reasons. Use small embedded XML strings in tests.

If the dialog registration through Catel `IUIVisualizerService` is blocked by existing project conventions, implement the smallest local dialog service that can be injected into the importer and record the deviation in `Decision Log`. Keep the parser independent from WPF types so unit tests remain UI-free.

If wrapper import refactoring breaks existing `custommodel` tests, first restore direct-root behavior and only then reapply wrapper handling. The direct-root import path is the regression baseline.

## Artifacts and Notes

The current user-facing failure comes from this unsupported root branch in `XModelImport.cs`:

    await ms.ShowErrorAsync($"Unsupported model type: {reader.Name}. \nImport only supports custom model types at this time.", "Model import error");

The reference file begins with:

    <?xml version="1.0"?>
    <models type="exported">
        <model name="GE Stardust 1500" DisplayAs="Custom" ...>

A minimal unit-test wrapper with one model can look like:

    <models type="exported">
        <model name="Wrapped Snowman" DisplayAs="Custom" parm1="300" parm2="300" PixelSize="1" CustomModelCompressed="1,0,0;2,1,0" />
    </models>

A minimal unit-test wrapper with two models can look like:

    <models type="exported">
        <model name="First" DisplayAs="Custom" parm1="300" parm2="300" PixelSize="1" CustomModelCompressed="1,0,0" />
        <model name="Second" DisplayAs="Custom" parm1="300" parm2="300" PixelSize="1" CustomModelCompressed="1,1,0;2,2,0" />
    </models>

A minimal unit-test wrapper with ignored model groups can look like:

    <models type="exported">
        <model name="Grouped" DisplayAs="Custom" parm1="300" parm2="300" PixelSize="1" CustomModelCompressed="1,0,0">
            <modelGroup name="Ignored Group" models="Grouped/SubModel" />
        </model>
    </models>

A minimal unit-test wrapper for unsupported non-custom DisplayAs behavior can look like:

    <models type="exported">
        <model name="Circle Model" DisplayAs="Circle" parm1="300" parm2="300" />
    </models>

The expected unsupported type for that example is `circlemodel`, not `model` and not `custommodel`.

## Interfaces and Dependencies

Do not add new NuGet packages. Use .NET XML APIs already available to the project, such as `System.Xml.Linq`, or continue with `System.Xml` if the final refactor can remain readable. Use NLog for logging because the importer already uses `NLog.Logger`.

At the end of implementation, these code-facing contracts should exist or an equivalent should be documented in the Decision Log:

    internal sealed record XModelSelectionItem(int Index, string DisplayName);

    internal interface IXModelSelectionService
    {
        Task<XModelSelectionItem?> SelectModelAsync(IReadOnlyList<XModelSelectionItem> models);
    }

If these types must be public for Catel view-model binding, add XML comments for every public type and public member. If they remain internal and simple, XML comments are optional but still useful for the selection service because it defines a behavioral boundary.

The importer should keep its public API:

    public Task<Prop> ImportAsync(string filePath);

It may add an internal or overload constructor for tests, for example:

    internal XModelImport(IXModelSelectionService selectionService)

The default public constructor should continue to work for `PropEditorViewModel`, resolving or creating the production selection service as needed.

## Revision Notes

2026-07-16 / Codex: Created the initial ExecPlan from the user request, current importer behavior, reference wrapper file, existing tests, and project planning and skill guidance.
2026-07-16 / Codex: Updated the plan after user clarification that wrapped model elements with `DisplayAs="Custom"` should be treated as custommodel imports.
2026-07-16 / Codex: Updated the plan after user clarification that `modelGroup` tags inside a model are unsupported, should be ignored, and should produce info-level log entries only.
2026-07-16 / Codex: Updated the plan with confirmed Jira issue key `VIX-3943`.
2026-07-16 / Codex: Completed Milestone 1 by updating Jira issue `VIX-3943` with the requirements, acceptance criteria, test plan, and implementation-plan reference.
2026-07-16 / Codex: Updated the plan after user clarification that non-custom wrapped `DisplayAs` values resolve to their corresponding unsupported model type, such as `Circle` to `circlemodel`.
2026-07-16 / Codex: Completed Milestone 2 by refactoring the importer to reusable `XElement` custom-model parsing and adding unsupported non-custom resolution plus info-only `modelGroup` logging in that path.
2026-07-16 / Codex: Completed Milestone 3 by adding wrapper discovery, internal selection items and service boundary, single supported model auto-import, unsupported non-custom wrapper handling, and cancel behavior for no selection.

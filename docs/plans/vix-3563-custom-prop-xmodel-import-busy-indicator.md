# Display the Custom Prop Editor busy indicator during local xModel imports

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document in accordance with `.agents/PLANS.md` from the repository root. It is self-contained so that an implementer can complete VIX-3563 without relying on prior chat context.

## Purpose / Big Picture

When a user imports a large local xLights `.xmodel` file through the Custom Prop Editor, the application can appear unresponsive because its busy indicator does not become visible before import work occupies the WPF user-interface thread. After this change, that indicator is visibly rendered before the local import begins. Vendor imports and multiple-model imports must retain their existing behavior, including the model-selection dialog.

The user can verify the result by opening the Custom Prop Editor, selecting a large local `.xmodel` containing a single importable model, and observing the existing busy indicator before the imported prop replaces the current prop.

## Progress

- [x] (2026-08-04 21:21Z) Analyzed VIX-3563, retrieved its Jira description, inspected the local, vendor, and multiple-model import paths, and created this ExecPlan.
- [x] (2026-08-05 13:56Z) Updated Jira issue VIX-3563 with refined scope, acceptance criteria, and automated/manual validation steps; no workflow transition was made.
- [x] (2026-08-05 14:06Z) Added a 200-millisecond asynchronous message-loop handoff and `try`/`finally` busy-indicator cleanup to the local xModel file-import command; the Custom Prop Editor project builds successfully.
- [ ] Build and run focused regression tests.
- [ ] Perform manual validation for local single-model, multiple-model, and vendor imports.
- [ ] Update VIX-3563 with final validation results and revise this document's living sections.

## Surprises & Discoveries

- Observation: The missing indicator is a UI scheduling issue, not an xModel parser-selection issue.
  Evidence: `PropEditorViewModel.Import` calls `IBusyIndicatorService.Show()` and immediately enters `ImportProp(path)`. In contrast, vendor imports await a download before importing, and multiple-model imports enter a modal selection dialog.

- Observation: The xModel importer deliberately contains no busy-indicator dependency.
  Evidence: `XModelImport` in `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs` only loads, parses, selects, and assembles model data. The busy indicator is owned by the view-model command layer.

- Observation: The multiple-model selection service intentionally hides the busy indicator while its selection dialog is shown and restores it afterward.
  Evidence: `XModelSelectionService.SelectModelAsync` calls `Hide()` before `IUIVisualizerService.ShowDialogAsync` and calls `Show()` in its `finally` block.

## Decision Log

- Decision: Fix the local file-import command rather than adding UI behavior to `XModelImport`.
  Rationale: The importer is reused by both vendor and local paths and is covered by non-UI tests. Keeping it UI-free preserves the existing parser/assembler separation and avoids coupling import data logic to WPF services.
  Date/Author: 2026-08-04 / Codex

- Decision: Await a 200-millisecond delay after showing the busy indicator and before invoking `ImportProp`.
  Rationale: Runtime validation disproved the initial context-idle approach. Catel 6.2's WPF `IBusyIndicatorService` changes `Mouse.OverrideCursor` to the wait cursor; it does not render a spinner window. A short asynchronous delay returns control to the Windows message loop long enough for the cursor transition to reach the desktop before CPU-bound import work starts. The delay is deliberately limited to local xModel imports and does not move mutable prop construction off the UI thread.
  Date/Author: 2026-08-05 / Codex

- Decision: Use `try/finally` around every `Show()` in this command scope.
  Rationale: The current linear `Show(); await ImportProp(); Hide();` sequence leaves the indicator vulnerable to an unexpected exception. `finally` guarantees cleanup while retaining `ImportProp`'s existing user-facing error behavior.
  Date/Author: 2026-08-04 / Codex

- Decision: Do not introduce a new automated UI-test seam unless an existing testable busy-indicator abstraction is discovered during implementation.
  Rationale: The observable defect is WPF render scheduling, while the existing command constructs its dependency resolver internally and no local busy-indicator test double exists. A new abstraction solely to observe `Show`/`Hide` would not prove the indicator was rendered. Preserve importer unit tests and require manual runtime validation.
  Date/Author: 2026-08-04 / Codex

## Outcomes & Retrospective

Planning outcome: the defect is localized to the Custom Prop Editor file-import command. Jira now records the final Milestone 1 delivery contract, including acceptance criteria and test plan. Milestone 2 awaits a 200-millisecond UI-message-loop handoff before local import work and guarantees `IBusyIndicatorService.Hide()` in `finally`. `dotnet build src\\Vixen.Modules\\App\\CustomPropEditor\\CustomPropEditor.csproj --no-restore` succeeded with two package-vulnerability warnings and zero errors. Regression tests, manual validation, and final Jira updates remain pending.

At completion, replace this paragraph with the commands run, the observed manual results, any variance from the planned dispatcher yield, and any remaining limitations.

## Context and Orientation

Vixen is a Windows desktop application using WPF, a Windows user-interface framework that processes UI work through a dispatcher queue. In the Catel 6.2 package used by this repository, `IBusyIndicatorService` is implemented for WPF by setting `Mouse.OverrideCursor` to the wait cursor. It does not create or render a separate busy-indicator window. Returning control to the message loop briefly after setting that cursor lets Windows apply the visual cursor change before CPU-bound work continues.

The Custom Prop Editor module is at `src/Vixen.Modules/App/CustomPropEditor`. Its primary view model is `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`. The `ImportCommand` property creates a Catel command that invokes the private `Import(string type)` method. That method opens a file picker, resolves `IBusyIndicatorService`, shows the busy indicator, and calls `ImportProp(path)`.

`ImportProp` creates `XModelImport` and awaits `ImportAsync(path)`. The importer at `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs` loads XML, chooses an embedded model when the file has a root `models` wrapper, parses supported model types, and assembles a `Prop`. It must not gain UI scheduling or busy-indicator logic.

For a file containing multiple embedded models, `XModelImport` uses `XModelSelectionService` at `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelSelectionService.cs`. It hides the indicator while showing the selection dialog, then shows it again after the dialog closes. Vendor imports use `OpenVendorBrowserAsync` and `LoadVendorModel` in `PropEditorViewModel`; `LoadVendorModel` already shows a busy indicator around the downloaded model import. These two paths explain why the problem is most visible for a single local model import.

## Plan of Work

### Milestone 1: Record the delivery contract in Jira

Update VIX-3563 before code changes. Replace or expand the issue description so it states that importing a local, single-model `.xmodel` through the Custom Prop Editor must visibly show the existing busy indicator before parsing and prop assembly begin. State that vendor imports and multiple-model selection behavior remain unchanged, that the xModel importer remains UI-free, and that the busy indicator must always be hidden after import success or unexpected failure.

Add these acceptance criteria to Jira: a large local single-model import visibly shows the indicator; selecting one model from a multiple-model wrapper still shows the selection dialog and completes normally; vendor import still completes normally; and the indicator is not left visible after success, cancellation, or an import error. Add the exact automated and manual validation commands from this ExecPlan as the issue test plan. Do not transition the issue unless separately requested.

### Milestone 2: Permit the busy indicator to render before local import work

Edit `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`.

In the non-empty-path branch of `Import(string type)`, retain the existing `IBusyIndicatorService` resolution and `Show()` call. Put the remaining operation in `try/finally`: after `Show()`, await `Task.Delay(200)`; then await `ImportProp(path)`; in `finally`, call `Hide()`.

The intended shape is:

    var pleaseWaitService = dependencyResolver.Resolve<IBusyIndicatorService>();
    pleaseWaitService.Show();
    try
    {
        await Task.Delay(200);
        await ImportProp(path);
    }
    finally
    {
        pleaseWaitService.Hide();
    }

The delay is intentional and must remain asynchronous so the UI thread can process messages. Do not use `Thread.Sleep`, `Task.Run`, or a blocking wait: `Thread.Sleep` prevents the cursor update, while `Task.Run` would risk unsafe access to the mutable prop model and Catel services.

Do not modify `ImportProp`, `XModelImport`, `XModelSelectionService`, model selection views, vendor metadata assignment, or the xModel parser/assembler. The only behavioral change is an opportunity to paint the already-requested busy indicator before the local import blocks the UI thread.

### Milestone 3: Validate the change and close the handoff loop

First build the changed module or solution to catch compiler issues with WPF dispatcher APIs. Then run the existing focused Custom Prop Editor xModel-import test suite to prove that direct, wrapped, and selected-model parsing behavior remains unchanged. Run the broader Custom Prop Editor filter if practical.

Manually start the Debug application output and perform three imports: a large local direct/single-model `.xmodel`; a two-model wrapper `.xmodel`; and a vendor-linked model. The first must visibly show the indicator before the prop appears. The second must still show the model-selection dialog rather than an obstructing indicator. The vendor path must retain its existing indicator and import behavior. Also induce or use a known-invalid xModel and verify that the existing error is shown and the indicator disappears.

Update the Jira description if implementation evidence changes any requirement, then add a Jira comment listing the build/test commands and manual results. Update `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` in this ExecPlan with actual evidence.

## Concrete Steps

All commands below run from `C:\Dev\Vixen` in PowerShell.

Inspect the starting state before editing:

    git status --short
    Get-Content -Path .agents\PLANS.md -Raw
    Get-Content -Path src\Vixen.Modules\App\CustomPropEditor\ViewModels\PropEditorViewModel.cs -TotalCount 1050

Edit only `src\Vixen.Modules\App\CustomPropEditor\ViewModels\PropEditorViewModel.cs` for the code change described in Milestone 2. Preserve tab indentation and LF line endings. Do not reformat unrelated code.

Build the Custom Prop Editor project first:

    dotnet build src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj --no-restore

Expected result:

    Build succeeded.
    0 Error(s)

Run importer regression tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore

Then run broader Custom Prop Editor tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor" --no-restore

Expected result for each command is a successful test run with zero failed tests. Existing package advisory warnings may be reported, but any compilation failure or failed test must be investigated before proceeding.

Check the final patch scope and whitespace:

    git diff --check
    git diff -- src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs
    git status --short

If the direct `dotnet build` has solution-specific resolution failures, use the repository build command instead:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

## Validation and Acceptance

The change is accepted only when all of the following are true.

Given the Custom Prop Editor is open and a large local `.xmodel` with one supported model is selected, when import starts, then the existing busy indicator becomes visible before the imported prop replaces the current prop.

Given an `.xmodel` with more than one embedded model, when import starts, then the selection dialog remains usable, choosing a model imports only that model, and canceling leaves the current prop unchanged. The change must not cause the busy indicator to cover or prevent the selection dialog.

Given a vendor model is selected through the vendor browser, when its model file is downloaded and imported, then its current busy-indicator behavior and vendor metadata mapping still work.

Given an invalid or unsupported local `.xmodel`, when its existing import error path executes, then the error is displayed and the busy indicator is no longer visible afterward.

Given the focused xModel importer regression suite and broader Custom Prop Editor suite are run, when they finish, then no test fails. Given `git diff --check` is run, then it produces no output and exits successfully.

## Idempotence and Recovery

The source edit is small and can safely be reapplied if a previous attempt was interrupted. Do not delete generated output folders or reset unrelated working-tree changes. If build artifacts interfere, use the normal project build or test commands again; they overwrite their own outputs.

If the application dispatcher is unexpectedly unavailable in this command path, stop and capture the exception and call stack in this plan before choosing an alternative. The expected command is UI-thread initiated, so `Application.Current.Dispatcher` should be available. Do not move parsing or prop assembly to `Task.Run` as a workaround, because those operations interact with existing application services and mutable UI-owned state.

## Artifacts and Notes

The pre-change local import branch has this effective sequence:

    pleaseWaitService.Show();
    await ImportProp(path);
    pleaseWaitService.Hide();

The problem is that `Show()` changes cursor state but CPU-bound import work can begin before Windows has processed the cursor transition. Catel 6.2's WPF busy-indicator implementation uses `Mouse.OverrideCursor = Cursors.Wait`; it does not display a separate spinner window. The 200-millisecond asynchronous boundary is therefore a deliberate message-loop handoff, not background import work.

The implementation should produce a narrow diff resembling:

      pleaseWaitService.Show();
    - await ImportProp(path);
    - pleaseWaitService.Hide();
    + try
    + {
    +     await Task.Delay(200);
    +     await ImportProp(path);
    + }
    + finally
    + {
    +     pleaseWaitService.Hide();
    + }

## Interfaces and Dependencies

Use the existing Catel `IBusyIndicatorService` from `Catel.Services`; no new service, interface, package, or project reference is needed. Continue resolving it with `this.GetDependencyResolver()` as `PropEditorViewModel` already does.

Use the existing `Task.Delay` API with a 200-millisecond duration. It asynchronously yields to the current WPF synchronization context, allowing Windows to apply Catel's wait-cursor override without moving the xModel parser or prop assembler off the UI thread.

No public or protected API changes are planned. Do not change the existing public `IModelImport.ImportAsync(string filePath)` contract. Consequently, no XML documentation update is required for this focused implementation.

## Revision Notes

2026-08-04 / Codex: Created this ExecPlan from VIX-3563 and direct inspection of the Custom Prop Editor local import, vendor import, multiple-model selection, and xModel importer paths. The plan records the dispatcher-yield approach because the indicator state is already requested but cannot render before UI-thread import work begins.

2026-08-05 / Codex: Completed Milestone 1 by updating VIX-3563 with the focused busy-indicator scope, binary acceptance criteria, and automated/manual validation plan. The issue remained in its existing Accepted status.

2026-08-05 / Codex: Completed Milestone 2. `PropEditorViewModel.Import` now yields the WPF dispatcher at `ContextIdle` after showing the existing busy indicator, then imports the selected local file and dismisses the indicator in `finally`. The Custom Prop Editor project compiled successfully; automated and manual validation remain Milestone 3 work.

2026-08-05 / Codex: Revised the Milestone 2 implementation after runtime feedback showed that the dispatcher boundary did not make the cursor visible. Inspection of Catel 6.2 confirmed that `IBusyIndicatorService` only overrides the mouse cursor. The command now awaits `Task.Delay(200)` after `Show()` so Windows can apply that override before CPU-bound import work; the project compiled successfully with zero errors.

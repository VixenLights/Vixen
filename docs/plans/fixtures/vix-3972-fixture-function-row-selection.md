# VIX-3972: Make Fixture Wizard function-row selection independent of startup event order

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

Fixture Wizard users must be able to click any function row, such as Tilt, and immediately see that function's details. The shared grid theme configures a `VixenDataGrid` for cell selection (`CellOrRowHeader`), so a function-grid cell click does not select the row that owns the detail pane. The existing one-click edit handler then focuses the cell editor, making the cell state even more prominent. Separately, an initialization-only guard can remain active if WPF raises the grid's first selection event before Catel attaches `FunctionTypeViewModel` to the view. It then rejects later row changes and leaves the old Pan detail panel visible. After this change, the function grid explicitly selects full rows, and the initial row is activated explicitly after the view and its nested detail views are ready.

The result is observable by opening the supplied `New Heads.xml` in Fixture Wizard, clicking Pan and Tilt repeatedly, and seeing both the selected-row highlight and the matching detail panel change every time. An incomplete detail record must still prevent leaving its row, because that is existing validation behavior rather than this regression.

## Progress

- [x] (2026-08-09 14:01 -05:00) Read the affected view, view model, Catel lifecycle metadata, repository history, and fixture-editor conventions.
- [x] (2026-08-09 14:01 -05:00) Determined the production change: explicit post-load initial activation and removal of the stale initialization guard.
- [x] (2026-08-09 14:01 -05:00) Wrote this implementation plan; no production source or test files were changed.
- [x] (2026-08-09 14:01 -05:00) Refined the diagnosis from user-observed behavior: the shared theme's `CellOrRowHeader` selection unit prevents a function-cell click from selecting its row.
- [ ] Update VIX-3972 only after the fix is confirmed by automated and manual validation, per user direction on 2026-08-09.
- [ ] Establish the DevBuild and fixture reproduction baseline.
- [x] (2026-08-09 14:10 -05:00) Implemented the local `FullRow` function-grid selection and lifecycle-safe initial detail activation; removed the obsolete initial-selection sentinel.
- [x] (2026-08-09 14:10 -05:00) Confirmed that `Vixen.Tests` has no existing STA/dispatcher WPF test pattern, so no fragile UI test infrastructure was introduced.
- [x] (2026-08-09 14:12 -05:00) Built the full `Vixen_Tests` target and ran the complete test suite: 676 passed, 0 failed, 0 skipped.
- [ ] Perform the supplied-fixture manual validation, inspect the effective grid styles, and record the final tracker results only if the fix is confirmed.

## Surprises & Discoveries

- Observation: VIX-3880 is not a behavior change to the one-click edit handler. It replaces the four identical handlers with `DataGridView.DataGrid_CellGotFocus`; the handler body is equivalent to the prior `FunctionTypeView` body.
  Evidence: `git diff 40e03a4d9^ 40e03a4d9 -- src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeView.xaml.cs` changes inheritance and deletes the local duplicate; `DataGridView.cs` contains the same `BeginEdit` and child-control-focus logic.

- Observation: `InitialSelectedFunction` is only a startup sentinel, yet it is public and is cleared only by `SelectFunctionItem`.
  Evidence: its only production references are the constructor and `SelectFunctionItem` in `FunctionTypeViewModel.cs`, plus the rejection condition in `FunctionTypeView.xaml.cs`.

- Observation: the selection event safely returns when `ViewModel` is null, but leaves the sentinel set. A later user selection of a different row therefore returns before `SelectFunctionItem` is reached.
  Evidence: `FunctionTypeView.SelectionChanged` first tests `vm == null`, then tests `InitialSelectedFunction != obj.SelectedItem`; only `SelectFunctionItem` clears the sentinel.

- Observation: Catel exposes both `ViewModelChanged` and `OnViewModelChanged`, and `UserControl` exposes `OnLoaded`. Either can occur in a different order from the WPF grid's first `SelectionChanged` event.
  Evidence: reflection against `Catel.MVVM` 6.2.0 reports the `ViewModelChanged` event and protected `OnViewModelChanged()` / `OnLoaded(EventArgs)` members.

- Observation: initial function activation can require nested child view models. The existing Pan/Tilt path already has a null check because Catel's child collection has previously been empty during initialization.
  Evidence: `FunctionTypeViewModel.DisplayTiltPan` contains the VIX-3248 defensive null check and comment.

- Observation: the shared dark DataGrid style explicitly sets `SelectionUnit` to `CellOrRowHeader`; `FunctionTypeView.functionGrid` does not override it.
  Evidence: `src/Vixen.Common/WPFCommon/Theme/ExpressionDark.xaml` sets `controls:DataGrid.SelectionUnit` to `CellOrRowHeader`, while `FunctionTypeView.xaml` has no `SelectionUnit` attribute. Under this WPF selection mode, clicking a cell selects that cell; clicking a row header selects a row. The function grid deliberately hides its row header with `RowHeaderWidth="0"`, leaving no normal row-selection target.

- Observation: the row selection visual exists but is driven by `DataGridRow.IsSelected`, whereas the cell visual is driven by `DataGridCell.IsSelected`.
  Evidence: `ExpressionDark.xaml` contains `RowStyle` triggers for `DataGridRow.IsSelected` and `CellStyle` triggers for `DataGridCell.IsSelected`. This matches the report that a cell enters edit without a stable selected-row border.

- Observation: VIX-3591 added implicit `DataGridCell` and `DataGridRow` styles to `Theme.xaml` after 3.12u6, but it postdates VIX-3880 and therefore cannot explain a failure first present in DevBuild-1405.
  Evidence: 3.12u6 is dated 2025-12-30; VIX-3880 (`40e03a4d9`) is dated 2026-04-12; VIX-3591's DataGrid-theme commit (`be30c9559`) is dated 2026-06-16. The VIX-3591 patch adds the implicit styles at `Theme.xaml` near the DataGrid styles.

- Observation: `FunctionTypeView` merges `Theme.xaml` and then `ExpressionDark.xaml`. The latter's implicit `DataGrid` style explicitly assigns its own keyed `RowStyle` and `CellStyle`.
  Evidence: `FunctionTypeView.xaml` orders the merged dictionaries Theme then ExpressionDark. `ExpressionDark.xaml` defines `RowStyle` and `CellStyle`, then assigns them through `DataGrid.RowStyle` and `DataGrid.CellStyle`. Those explicit values should override Theme's new implicit row/cell styles for a normally styled function grid; this must be confirmed once by inspecting the live grid's effective styles during manual validation.

- Observation: `Vixen.Tests` contains no established STA, dispatcher frame, or WPF-fact test pattern.
  Evidence: a search for `STAThread`, `ApartmentState`, `DispatcherFrame`, `WpfFact`, and `WpfTheory` under `src/Vixen.Tests` returned no matches. A new WPF lifecycle test would need bespoke test infrastructure and is not justified for this focused regression.

- Observation: the affected module and its dependencies rebuild successfully after the production change.
  Evidence: `msbuild src/Vixen.Modules/Editor/FixturePropertyEditor/FixturePropertyEditor.csproj -m -restore -t:Rebuild -p:Configuration=Debug -p:Platform=x64 -v:m` exited 0. Existing warnings originated in `Vixen.Core` and `FixtureGraphics`; no warning or error named an edited file.

- Observation: the full test build and complete existing test suite pass after the production change.
  Evidence: `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` exited 0. `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"` then reported 676 passed, 0 failed, and 0 skipped in four seconds. Existing warnings originated outside the edited files.

## Decision Log

- Decision: Do not revert VIX-3880 or alter `DataGridView.cs` as the VIX-3972 fix.
  Rationale: history shows the extraction preserved the relevant event-handler behavior. A build boundary can still identify a timing change, but reverting a shared deduplication refactor would not repair the underlying reliance on the first grid event.
  Date/Author: 2026-08-09 / Codex

- Decision: Make initial activation an explicit view-lifecycle action and remove `InitialSelectedFunction` rather than trying to repair the sentinel from `SelectionChanged`.
  Rationale: the initial selection is a setup concern, not a user selection. Any state which survives a missed early event can suppress a real click indefinitely. Removing it makes each later selection subject only to the existing validation rule.
  Date/Author: 2026-08-09 / Codex

- Decision: Override the shared selection policy only on `FunctionTypeView.functionGrid` by setting `SelectionUnit="FullRow"` in XAML.
  Rationale: this grid navigates a single detail pane per function, so a row—not an individual cell—is its unit of navigation. A local override restores the expected user interaction without changing the behavior of the other fixture-editor grids, some of which may intentionally support cell-level editing.
  Date/Author: 2026-08-09 / Codex

- Decision: Retain `DataGridCell.GotFocus="DataGrid_CellGotFocus"` for the initial implementation unless a manual verification proves it prevents `FullRow` selection.
  Rationale: the handler existed in 3.12u6 and VIX-3880 extracted equivalent code. Removing it preemptively would alter established one-click editing across the function fields. The local selection-unit override is the minimal correction for the observed row-selection failure.
  Date/Author: 2026-08-09 / Codex

- Decision: Do not edit the shared WPFCommon DataGrid styles for VIX-3972; add an effective-style inspection to validation instead.
  Rationale: VIX-3591 is too late to be the reported first regression and its implicit styles are expected to be superseded by the Function Type view's ExpressionDark grid style. A shared-theme modification would risk unrelated grids without first proving that the Function Type grid is receiving the wrong effective `RowStyle` or `CellStyle`.
  Date/Author: 2026-08-09 / Codex

- Decision: Defer all VIX-3972 tracker edits until implementation is confirmed.
  Rationale: the user explicitly requested one final JIRA update rather than incremental updates while validation may still reveal another cause. The implementation and validation evidence are recorded in this plan until then.
  Date/Author: 2026-08-09 / User direction recorded by Codex

- Decision: Schedule activation only after both `FunctionTypeView` is loaded and Catel has supplied a `FunctionTypeViewModel`, using the WPF dispatcher at a post-load priority.
  Rationale: the selected function may be Pan, Tilt, indexed, color-wheel, or zoom. Activating it calls into nested detail view models, so the operation must wait until the visual tree has created them. Observing both lifecycle edges makes the solution safe regardless of which order they occur.
  Date/Author: 2026-08-09 / Codex

- Decision: Retain `AllowSelectionToChange` and `RestorePreviouslySelectedFunction` unchanged in the primary fix.
  Rationale: they intentionally prevent data loss when an active detail editor is invalid. The plan requires manual verification of that behavior and calls for a separate follow-up only if the fixture proves it silently rejects valid data.
  Date/Author: 2026-08-09 / Codex

## Outcomes & Retrospective

Implementation and automated validation are complete pending manual confirmation. `FunctionTypeView.functionGrid` now locally requests full-row selection; `FunctionTypeView` explicitly initializes its selected function only after both Catel view-model attachment and WPF loading; and the obsolete `InitialSelectedFunction` sentinel was removed. The module build and full 676-test suite pass. The supplied fixture was not present in the workspace, so the DevBuild boundary, live effective-style inspection, and exact attached-data validation remain required before closing VIX-3972. JIRA remains intentionally unchanged until those checks confirm the fix.

## Context and Orientation

The Fixture Property Editor module lives at `src/Vixen.Modules/Editor/FixturePropertyEditor`. Its `FunctionTypeView` is the WPF user control shown by Fixture Wizard for editing a fixture's functions. The left `functionGrid` lists functions such as Pan and Tilt. The right side contains nested detail controls (`PanTiltView`, `IndexedView`, `ColorWheelView`, and `ZoomView`). Catel is the MVVM library that attaches a view model to each view and tracks child view models for nested controls.

`src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeView.xaml` binds `functionGrid.SelectedItem` two-way to `FunctionTypeViewModel.SelectedItem` and routes the WPF `SelectionChanged` event to code-behind. It currently inherits `SelectionUnit="CellOrRowHeader"` from `src/Vixen.Common/WPFCommon/Theme/ExpressionDark.xaml` and hides row headers with `RowHeaderWidth="0"`. In that WPF mode, a mouse click selects a cell rather than the function row. `FunctionTypeView.xaml.cs` asks the view model whether it is safe to leave the previous detail panel, calls `SelectFunctionItem` to display the newly selected function, or asynchronously restores the old row when validation denies the change.

`src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeViewModel.cs` constructs the function rows and sets `SelectedItem` to the requested starting function. Its current public `InitialSelectedFunction` property exists only to ignore WPF's presumed default-first-row selection. That rule is unsafe: if the event arrives before `ViewModel` exists, the event returns and the sentinel is never cleared.

`src/Vixen.Modules/Editor/FixturePropertyEditor/Views/DataGridView.cs` is the shared Catel `UserControl` base introduced in VIX-3880. It starts editing a grid cell when it gains focus. It is not the owner of function-selection state and must not change for this fix.

The test project at `src/Vixen.Tests/Vixen.Tests.csproj` currently does not reference the Fixture Property Editor module. WPF visual-lifecycle tests require a Windows STA (single-threaded apartment) dispatcher; do not add a fragile test that runs the view on xUnit's normal worker thread. The manual fixture scenario below is mandatory even if a focused automated test is added.

## Plan of Work

### Milestone 1: Confirm and document the regression boundary

Update VIX-3972 before modifying code. State the user-visible failure: clicking a function cell enters that cell's edit state without selecting the owning row; clicking a different function, including Tilt, can therefore leave the initially selected Pan detail panel active and no stable row highlight. Record the scope as `FunctionTypeView` row selection and initial activation lifecycle; explicitly exclude fixture serialization, shared `DataGridView` one-click editing, and the valid-data selection policy.

Using the supplied `New Heads.xml`, execute the existing manual Fixture Wizard scenario on DevBuild-1404 and DevBuild-1405. If the behavior differs, record VIX-3880 as the timing boundary but retain the lifecycle repair described here. If both builds fail, repeat on 1398 and 1399 and record the Catel upgrade result. Independently test the first build containing VIX-3591's DataGrid theme update, because it can affect selection presentation only in later builds. If no historical binaries are available, record that limitation and reproduce on a source build with debugger breakpoints in `SelectionChanged`, `FunctionTypeView.OnViewModelChanged` or its event equivalent, and `SelectFunctionItem`. The required evidence is whether an early `SelectionChanged` observes a null view model and whether a later Tilt event returns because the initial sentinel is still Pan.

Add acceptance criteria to the issue: a click in any function cell selects the complete row, not just that cell; the requested initial function displays once the editor opens; clicking any complete function row changes both the selected-row highlight and detail panel; repeated Pan/Tilt switches work; an invalid active detail continues to prevent navigation and restores the preceding row; and opening/closing the editor does not throw. Add the exact build and manual checks from this plan. Do not change issue status unless project workflow requires it.

### Milestone 2: Remove event-order-dependent initialization

Edit `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeView.xaml` first. On the `local:VixenDataGrid` named `functionGrid`, set `SelectionUnit="FullRow"`. This local property overrides the shared dark-theme default of `CellOrRowHeader` for this navigation grid alone. Retain `SelectionMode="Single"` through the theme and retain `DataGridCell.GotFocus="DataGrid_CellGotFocus"` in the first implementation. A click in Name, Type, Preview Legend, or Tag must now select the entire function row before any cell's normal edit control receives focus.

Do not change `src/Vixen.Common/WPFCommon/Theme/Theme.xaml` or `ExpressionDark.xaml`: their defaults are shared by other grids. Before changing either shared style, use the debugger's Live Visual Tree or inspect `functionGrid.RowStyle`, `functionGrid.CellStyle`, and a realized row/cell's `Style` while the Function Type view is open. Record whether the effective grid styles are the keyed `ExpressionDark.xaml` `RowStyle` and `CellStyle`; if they are, VIX-3591's implicit Theme styles are not in the active rendering path. Do not change `DataGridView.cs` in this milestone, because history proves the handler logic itself was already present in the release and its one-click-edit behavior remains useful. If `FullRow` unexpectedly does not produce the expected selection in manual testing, record the actual `functionGrid.SelectedItem`, `SelectedCells`, and `DataGridRow.IsSelected` values before considering a scoped replacement of the focus handler.

Edit `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeViewModel.cs`. In the constructor, assign the item returned by `InitializeChildViewModels` directly to `SelectedItem`. Delete `InitialSelectedFunction` and remove the line in `SelectFunctionItem` that clears it. There must be no remaining reference to that property. This removes a public API member, so read and follow `.agents/skills/csharp-docs/SKILL.md` before the edit; update XML documentation for every affected public member and do not leave documentation describing the deleted startup sentinel.

Edit `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeView.xaml.cs`. Remove the `InitialSelectedFunction` branch from `SelectionChanged`. Preserve the null checks, `AllowSelectionToChange`, `SelectFunctionItem`, add-row editing behavior, scrolling, and asynchronous restore behavior. A normal user click must now always reach either accepted selection or the existing validation rollback.

Add a private, view-specific initialization helper to `FunctionTypeView`. It must run exactly when both conditions are true: the WPF control is loaded and Catel's `ViewModel` is a non-null `FunctionTypeViewModel`. Subscribe to `Loaded` in the constructor and observe Catel's `ViewModelChanged` lifecycle (by overriding `OnViewModelChanged` and calling the base implementation, or by subscribing to the event in the constructor). Both paths call the same helper; do not duplicate selection logic.

The helper must schedule a single WPF dispatcher callback at a post-load priority such as `DispatcherPriority.Loaded` or later. In the callback, re-check that the view remains loaded and still has the same valid `FunctionTypeViewModel`. If `SelectedItem` is non-null, set `functionGrid.SelectedItem` to it only if necessary, then call `SelectFunctionItem` for that item. This explicit call establishes `PreviouslySelectedItem`, subscribes to its change event, and initializes the appropriate nested detail panel without depending on `SelectionChanged`. Use a private Boolean or a view-model identity field so the same view-model is initialized once, but reset that state if Catel supplies a different view model. Do not call `AllowSelectionToChange` for initial activation: there is no prior user edit to save.

The callback must tolerate disposal and lifecycle changes: it must return without touching controls if the view is unloaded or its view model changed. Do not use `Thread.Sleep`, a synchronous dispatcher call, a global service locator, or a new public API. This is view-specific visual setup, which is the justified exception to the project's normal code-behind restriction.

### Milestone 3: Add feasible focused coverage

First determine whether the existing xUnit setup can run a WPF dispatcher test in STA without adding a test-framework dependency. Search `src/Vixen.Tests` for established `[STAThread]`, dispatcher, or WPF-view test patterns. If a supported pattern exists, add the Fixture Property Editor project reference to `src/Vixen.Tests/Vixen.Tests.csproj` with the project's normal project-reference metadata and create one focused test class under a matching `src/Vixen.Tests/Editor/FixturePropertyEditor` folder.

The focused test must instantiate `FunctionTypeViewModel` with at least Pan and Tilt fixture functions, attach it to `FunctionTypeView` after the control has loaded or simulate the corresponding lifecycle ordering, pump the dispatcher through the post-load callback, then select Tilt through `functionGrid`. Assert that `functionGrid.SelectionUnit` is `FullRow`, the function-grid selection, `SelectedItem`, and `PreviouslySelectedItem` are Tilt, and that the Pan/Tilt details remain available. Add the inverse Tilt-to-Pan assertion. The test must fail before the production fix by exercising both the inherited cell-selection configuration and the missed-initial-event ordering, and pass after it.

If no supported STA WPF test pattern exists, do not introduce ad hoc threading infrastructure or a new package merely for this regression. Instead, record that limitation in VIX-3972 and the plan, add no unreliable test, and use the reproducible manual fixture scenario as the acceptance test. A plain view-model test is insufficient because the defect is the order between WPF `SelectionChanged`, Catel view-model attachment, and nested-control initialization.

### Milestone 4: Build, validate, and close the tracker loop

Run the Fixture Property Editor project build first. Then run the project-standard test sequence from `C:\Dev\Vixen`:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

If the full test target is unavailable in the implementation environment, run the module project build and any focused fixture test, record the reason and output, and do not call the work fully validated until a normal Windows build agent runs the prescribed sequence. Treat existing warnings as baseline only; investigate every warning introduced by the change.

Manually open the supplied `New Heads.xml` in Fixture Wizard. Verify the initially requested function's row and details. Click Tilt, Pan, Tilt, and every other complete function in turn. Each click must produce a persistent selected row and matching detail panel. Intentionally leave a required field incomplete in the active detail panel, click another row, and confirm the existing validation prevents navigation and restores the original row without a crash. Correct the field and confirm navigation succeeds. Close and reopen the editor and repeat one Pan/Tilt switch.

After validation, update VIX-3972 with the delivered behavior, exact commands and outcomes, fixture/manual observations, any test limitation, and baseline warnings. Add a tracker comment with those results and reconcile this document's `Progress`, `Surprises & Discoveries`, `Decision Log`, `Outcomes & Retrospective`, and dated revision note. Do not create a commit unless explicitly asked. When a milestone that changes repository files completes, invoke the project `commit-msg` skill and report this formatted candidate message:

    fix(fixture): initialize function selection after view load

## Concrete Steps

Run commands from `C:\Dev\Vixen` in PowerShell. The following read-only commands are safe to repeat while investigating:

    git diff --ignore-space-at-eol 40e03a4d9^ 40e03a4d9 -- src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeView.xaml.cs src/Vixen.Modules/Editor/FixturePropertyEditor/Views/DataGridView.cs
    rg -n -C 4 "InitialSelectedFunction|SelectFunctionItem|AllowSelectionToChange|ViewModelChanged|SelectionUnit" src/Vixen.Modules/Editor/FixturePropertyEditor src/Vixen.Common/WPFCommon/Theme
    rg -n "STAThread|Dispatcher|ApartmentState" src/Vixen.Tests --glob '*.cs'

After editing, prove the obsolete sentinel is fully removed:

    rg -n "InitialSelectedFunction" src/Vixen.Modules/Editor/FixturePropertyEditor

Expected result: no matches. Then build the affected project:

    msbuild src/Vixen.Modules/Editor/FixturePropertyEditor/FixturePropertyEditor.csproj -m -restore -t:Rebuild -p:Configuration=Debug -p:Platform=x64 -v:m

Expected result: the build exits with code 0 and reports no new warnings or errors attributable to the edited files. Follow it with the full test commands in Milestone 4.

## Validation and Acceptance

Acceptance requires both automated build/test evidence and the manual fixture scenario. The following behavior must be demonstrated:

- Fixture Wizard opens `New Heads.xml` with the requested initial function selected and its matching details visible.
- Clicking Name, Type, Preview Legend, or Tag on a different function selects the whole row, including the row border/highlight; it does not leave only the clicked cell selected.
- The debugger inspection shows whether `ExpressionDark.xaml`'s keyed `RowStyle` and `CellStyle`, rather than the implicit VIX-3591 Theme styles, are effective for `functionGrid`; do not alter shared theme files unless this check proves otherwise.
- A click from Pan to Tilt makes Tilt the stable selected row and displays Tilt details; a click back to Pan does the reverse. Repeating these switches never leaves stale details behind.
- Every complete function row can be selected, including rows with indexed, color-wheel, and zoom details when present.
- A deliberately invalid active detail still uses the existing validation policy: navigation is denied and the preceding row is restored. Once valid, the same target row can be selected.
- Closing or reopening the editor during initialization does not throw an exception, and the full `Vixen_Tests` target and `dotnet test --no-build` command complete successfully.

## Idempotence and Recovery

The source edit is idempotent: initialization state belongs to the view instance and is reset when Catel attaches a different view model. Rebuilding and rerunning the manual scenario do not modify fixture source data unless the tester explicitly saves the edited fixture. Use a copy of `New Heads.xml` for the invalid-detail scenario or close without saving after that check. If the explicit activation exposes a null nested child view model, do not add timing delays blindly; capture the call order, retain the existing VIX-3248 null guard, and schedule the helper only after the child lifecycle is demonstrably ready. If the DevBuild comparison contradicts the hypothesis, preserve the plan and update its Decision Log before changing production scope.

## Artifacts and Notes

The decisive state transition in the current code is:

    FunctionTypeViewModel constructor:
        InitialSelectedFunction = selected Pan
        SelectedItem = selected Pan

    early WPF SelectionChanged before Catel ViewModel is assigned:
        return;                       // sentinel remains Pan

    later user click on Tilt:
        if sentinel is Pan and grid item is Tilt, return;
        // SelectFunctionItem is never called; Pan details remain

The intended transition after this work is:

    user clicks any cell in a function row:
        FunctionTypeView.functionGrid uses FullRow selection
        the function row, rather than only the cell, becomes selected
        SelectionChanged receives the new function item

    Catel has attached FunctionTypeViewModel and FunctionTypeView is loaded:
        dispatcher callback selects the requested item once
        SelectFunctionItem initializes its detail panel

    later user click on any complete row:
        AllowSelectionToChange succeeds
        SelectFunctionItem updates previous item and details

`DataGridView.DataGrid_CellGotFocus` remains unchanged. It begins cell edit after focus and is unrelated to selection ownership.

## Interfaces and Dependencies

No new package, interface, serialized model, or persisted fixture format is required. Production code continues to use `Catel.Windows.Controls.UserControl` through `DataGridView`, the existing `FunctionTypeViewModel`, WPF `DataGrid`, and WPF `Dispatcher` supplied by the view. The only removed member is `FunctionTypeViewModel.InitialSelectedFunction`, which has no production caller outside this module and must be confirmed by repository search before deletion.

The final `FunctionTypeView` must retain its existing private `SelectionChanged(object, SelectionChangedEventArgs)` event handler and `RestorePreviouslySelectedFunction()` behavior. Its `functionGrid` must explicitly set `SelectionUnit="FullRow"`, and it gains only private lifecycle coordination. `FunctionTypeViewModel.SelectFunctionItem(FunctionItemViewModel item)` remains the single routine that records `PreviouslySelectedItem`, subscribes to function-type changes, and chooses the detail control.

Revision note (2026-08-09): Revised after user-observed cell-only interaction and WPFCommon-theme review. The plan now makes a local `FullRow` selection-unit override the first production change, retains explicit initialization for the stale detail-pane path, and requires an effective-style inspection before any shared Theme or ExpressionDark change. VIX-3591's June DataGrid-theme change postdates VIX-3880's April boundary, so it is recorded as a later visual interaction rather than the original DevBuild-1405 trigger.

Revision note (2026-08-09): Implemented the local row-selection and lifecycle changes. JIRA updates are deliberately deferred until automated and manual validation confirms the final scope.

Revision note (2026-08-09): The affected module build, full `Vixen_Tests` target, and 676-test suite pass. Manual validation remains the required confirmation because the attached fixture is unavailable in this workspace.

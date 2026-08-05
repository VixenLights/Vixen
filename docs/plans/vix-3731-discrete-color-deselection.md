# Make discrete color choices deselectable

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. This document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

VIX-3731 fixes the multiple discrete color picker so a user can check a color and then clear that same checkbox by clicking anywhere in its color row, including the colored rectangle. Today, the picker is configured for one selected row only, so the visual ListBox selection cannot be removed by clicking it again. After this change, WPF uses the row’s internal selection state only to process input; the checkbox and the colors returned when the user presses **OK** share one state, and the row never renders selected. A user can independently check several colors, clear all of them, and use Space to toggle the focused row.

The visible proof is a gradient color edit using the multiple discrete color picker: clicking a color rectangle checks its checkbox without adding a row highlight; clicking it again clears that checkbox; pressing **OK** returns exactly the checked colors. Clearing the final color continues to use existing behavior that deletes the gradient point.

## Progress

- [x] (2026-08-05 09:58 -05:00) Updated Jira issue VIX-3731 with the final scope, acceptance criteria, validation plan, and ExecPlan path before implementation.
- [ ] Inspect the working tree and the four picker files named in this plan before editing; confirm no unrelated changes overlap the target XAML file.
- [x] (2026-08-05 10:06 -05:00) Changed the multi-picker ListBox to `SelectionMode="Multiple"`, bound its internal row selection and checkbox two-way to `CheckBoxSelected`, and added a picker-local row template that suppresses all selection visuals.
- [x] (2026-08-05 10:11 -05:00) Updated shared picker sizing so the dialog is wide enough for at least three color items before the wrap panel starts a new row.
- [ ] (partially completed 2026-08-05 10:13 -05:00) Built `DiscreteColorPicker` in Release successfully with 0 errors; remaining: run the repository test project.
- [ ] Manually verify multi-select toggling, initial selections, confirmation/cancellation, and the unchanged single-color picker.
- [ ] Update VIX-3731 with any final requirement or test-plan adjustments and add a comment containing the validation results.
- [ ] Update this ExecPlan’s living sections and outcomes after implementation.

## Surprises & Discoveries

- Observation: `MultipleDiscreteColorPickerView.xaml` leaves `ListBox.SelectionMode` unset, which means WPF uses its default `Single` mode.
  Evidence: The current `<ListBox>` at `src/Vixen.Common/DiscreteColorPicker/Views/MultipleDiscreteColorPickerView.xaml` has no `SelectionMode` attribute, and its `ListBoxItem.IsSelected` setter binds to `ColorItem.IsSelected` rather than the multiple-picker checkbox state.

- Observation: an empty result is already meaningful to both known callers.
  Evidence: `src/Vixen.Modules/App/ColorGradients/GradientEditPanel.cs` calls `DeleteColor()` when `GetSelectedColors().Count() == 0`; `src/Vixen.Modules/Editor/EffectEditor/Controls/BaseInlineGradientEditor.cs` similarly calls `DeletePoint()`.

- Observation: VIX-3731 already exists as an in-progress Bug in the VIX project and is assigned to Jeff Uchitjil.
  Evidence: Jira issue `VIX-3731` was retrieved before editing; its status was `In Progress`, issue type was `Bug`, and its description accepted the expanded scope, acceptance criteria, and validation plan without a required-field or permission error.

- Observation: the shared `ListBoxStyle` leaves a ListBoxItem’s platform selected-state template active, so binding `IsSelected` to `CheckBoxSelected` alone displays a row highlight.
  Evidence: `src/Vixen.Common/WPFCommon/Theme/Theme.xaml` defines `ListBoxStyle` with an item-container style but no replacement `ControlTemplate`; the platform ListBoxItem template responds to `IsSelected`. A local template without `IsSelected` triggers is required to keep selection internal.

- Observation: the multiple picker already supplies `ConfigureWindowSize(92, 3)`, but the base class's fixed 250-pixel minimum is narrower than three 92-pixel items plus its 10-pixel window allowance.
  Evidence: `3 * 92 + 10 = 286`, whereas `src/Vixen.Common/DiscreteColorPicker/Views/DiscreteColorPickerViewBase.cs` previously enforced only 250 pixels. The dialog could therefore wrap after two multiple-picker items despite the three-item threshold.

- Observation: the focused Release build succeeds, but it reports four warnings from existing `Vixen.Core` sources.
  Evidence: `msbuild src\\Vixen.Common\\DiscreteColorPicker\\DiscreteColorPicker.csproj -m -t:Restore,Rebuild -p:Configuration=Release -p:Platform=x64` completed with `0 Error(s)` and warnings `CS8632` in `IElementTemplate.cs`, `CS0618` in `HardwareUpdateThread.cs`, and `CS0067` in `ProgramExecutor.cs`; none are in the changed picker files.

## Decision Log

- Decision: Use WPF `SelectionMode="Multiple"`, not `Extended`, and rely on WPF’s built-in selection interaction.
  Rationale: In `Multiple` mode, a normal click and Space toggle the focused item without modifier keys. `Extended` preserves conventional single-selection behavior for ordinary clicks and would not meet the deselection requirement. WPF therefore owns the toggle operation, avoiding custom event logic and double-toggle/reentrancy problems.
  Date/Author: 2026-08-05 / Codex planning from Sol handoff

- Decision: Bind both `ListBoxItem.IsSelected` and `CheckBox.IsChecked` two-way to `MultiSelectColorItem.CheckBoxSelected`, but render the ListBoxItem with a local template that ignores `IsSelected`.
  Rationale: `MultipleDiscreteColorPickerViewModel.GetSelectedColors()` already returns colors whose `CheckBoxSelected` is true, and `ProcessSelectedItem()` sets that same property for initial selections. The shared state keeps WPF input and accepted results synchronized, while the local template ensures the checkbox—not the row—is the only selection indicator.
  Date/Author: 2026-08-05 / Codex planning from Sol handoff

- Decision: Do not change the view model, `MultiSelectColorItem`, `ColorItem`, gradient callers, code-behind, commands, or public APIs.
  Rationale: Existing initialization and result handling already support zero or many selections. Altering the inherited legacy `ColorItem.IsSelected` property would expand this narrowly scoped UI fix and require public API documentation work without improving VIX-3731.
  Date/Author: 2026-08-05 / Codex planning from Sol handoff

- Decision: Use manual WPF interaction testing rather than add an automated UI test in this issue.
  Rationale: The defect is WPF ListBox mouse/keyboard selection behavior in a dialog, and the repository has no discovered UI test harness for this picker. The production change is declarative XAML only; project build plus a focused manual dialog exercise directly proves the required behavior. A test harness is outside this issue’s scope.
  Date/Author: 2026-08-05 / Codex planning

- Decision: Replace the existing brief VIX-3731 description with the implementation contract, acceptance criteria, and validation plan captured by this ExecPlan.
  Rationale: The original issue described the symptom but did not define keyboard behavior, the state-synchronization contract, excluded scope, zero-selection behavior, or demonstrable completion criteria. The expanded description makes the issue independently reviewable and executable.
  Date/Author: 2026-08-05 / Codex, Milestone 1

- Decision: Suppress selected-row rendering in the multiple picker while retaining its internal `IsSelected` binding.
  Rationale: The requirement changed after the initial Milestone 2 implementation: clicking a row must toggle its checkbox, but the ListBoxItem must never visually indicate selection. Replacing only this picker’s container template preserves WPF `Multiple` mode mouse/keyboard behavior, leaves the checkbox as the sole indicator, and does not alter the shared or single-picker styles.
  Date/Author: 2026-08-05 / Codex, revised Milestone 2

- Decision: Calculate the minimum dialog width as the larger of 250 pixels and three item widths plus the existing 10-pixel allowance.
  Rationale: This guarantees that the common three-color multiple picker has 286 pixels and does not wrap early, while the 72-pixel single picker retains its established 250-pixel minimum because three of its items require only 226 pixels. The rule belongs in the shared sizing method because both dialogs use it.
  Date/Author: 2026-08-05 / Codex, sizing refinement

## Outcomes & Retrospective

Not started. At completion, replace this paragraph with the build/test results, the manual test evidence, the final VIX-3731 comment reference, and any deviations from this plan.

## Context and Orientation

Vixen is a .NET 10 Windows Presentation Foundation (WPF) desktop application. WPF displays a `ListBox` as a collection of visual rows. A `ListBoxItem` is one row; its `IsSelected` Boolean property determines whether WPF presents it as selected. A WPF binding keeps a visual property and a view-model property synchronized. `Mode=TwoWay` means user interaction writes back to the view model and view-model updates redraw the visual.

This issue affects only the multi-color dialog in `src/Vixen.Common/DiscreteColorPicker`. `Common.DiscreteColorPicker.Views.MultipleDiscreteColorPickerView` is declared in `Views/MultipleDiscreteColorPickerView.xaml`. It renders `Colors` from `MultipleDiscreteColorPickerViewModel` in a wrapping `ListBox`; each row contains a checkbox and a 50-by-50 color rectangle.

The internal input-state flow that must exist after the change is:

    ListBoxItem.IsSelected  <->  MultiSelectColorItem.CheckBoxSelected  <->  CheckBox.IsChecked

`MultiSelectColorItem` in `ViewModels/ColorItems/MultiSelectColorItem.cs` inherits `ColorItem` and supplies the `CheckBoxSelected` Catel property. Catel properties notify WPF bindings when their value changes. `MultipleDiscreteColorPickerViewModel` in `ViewModels/MultipleDiscreteColorPickerViewModel.cs` obtains the accepted result through `GetSelectedColors()`, which filters `Colors` where `CheckBoxSelected` is true. Its inherited `InitializeViewModel` path sets `ColorItem.IsSelected` for supplied initial colors, assigns each one to `SelectedItem`, and the multi-select override `ProcessSelectedItem` sets `CheckBoxSelected = true`. That initialization behavior must remain intact.

`ColorItem.IsSelected` is a separate inherited property retained for shared initialization compatibility. The implementation must not merge or remove it in this issue. The ListBox container binding in the multi-select XAML intentionally stops using it and instead binds to `CheckBoxSelected`. The local `ListBoxItem` template must contain a transparent `Border` and `ContentPresenter`, with no trigger that reads `IsSelected`; that keeps the item hit-testable without showing a row highlight, border, or other selection visual.

The same dialog is opened by `GradientEditPanel` and `BaseInlineGradientEditor`. Both deliberately treat no returned colors as a request to delete the current gradient color or point. The single-color dialog at `src/Vixen.Common/DiscreteColorPicker/Views/SingleDiscreteColorPickerView.xaml` is a separate view and must not change; it retains normal one-item ListBox selection.

No services, threads, asynchronous work, allocations beyond ordinary WPF binding propagation, or new dependencies are needed. Each user toggle updates one Boolean on WPF’s UI thread.

## Plan of Work

### Milestone 1: Record the final issue contract in Jira

Before editing code, update VIX-3731 through the repository’s `jira` skill at `.agents/skills/jira/SKILL.md`. Replace or augment the issue description so it states that the multiple discrete color picker must let an ordinary click or Space toggle each row; that its checkbox and returned colors are synchronized through `CheckBoxSelected`; that the checkbox is the only visual selection indicator; and that zero selected colors is valid. Include the acceptance scenarios from `Validation and Acceptance`, state that the single-color picker is excluded, and link this plan as `docs/plans/vix-3731-discrete-color-deselection.md`. Record the Jira update in `Progress` and any constraint learned from Jira in `Surprises & Discoveries`.

This milestone changes external issue-tracking data only. It creates a shared definition of done before source code changes begin.

### Milestone 2: Unify the multi-picker’s WPF selection state

Edit only `src/Vixen.Common/DiscreteColorPicker/Views/MultipleDiscreteColorPickerView.xaml`. On its existing top-level `ListBox`, add `SelectionMode="Multiple"` alongside the existing binding and visual attributes. This enables WPF’s normal toggle semantics: clicking an unselected row selects it; clicking an already-selected row deselects it; Space toggles the focused row. Do not set `SelectionMode="Extended"`.

Within the existing `ListBox.ItemContainerStyle`, replace the `IsSelected` setter value with a two-way binding to `CheckBoxSelected`:

    <Setter Property="IsSelected" Value="{Binding CheckBoxSelected, Mode=TwoWay}" />

Within the existing item template, make the checkbox’s existing binding explicitly two-way:

    <CheckBox IsChecked="{Binding CheckBoxSelected, Mode=TwoWay}"></CheckBox>

In the same local `ListBoxItem` style, set `Template` to a minimal `ControlTemplate` that has a `Border Background="Transparent"` containing a `ContentPresenter` whose horizontal and vertical alignments use `TemplateBinding`. Do not add an `IsSelected` trigger to that template. The transparent border preserves the row’s complete hit area; because the template has no selected-state trigger, WPF may select the container to process the click or Space but cannot render a selection highlight.

Keep all existing namespaces, layout, ListBox style, `ItemsSource`, `SelectedItem`, rectangle appearance, buttons, and XAML structure. Do not add code-behind handlers, `EventSetter` elements, commands, converters, view-model code, or tests solely to invert the selection. Do not modify `SingleDiscreteColorPickerView.xaml`.

At the end of this milestone, WPF changes a row’s internal selected state, the container binding writes the same Boolean to `CheckBoxSelected`, and the checkbox binding reflects that Boolean. Direct checkbox interaction uses the same Boolean and therefore updates the row state. The local container template suppresses all selected-state rendering. The expected selection rule for every interaction is `new selection = NOT current selection`.

After the source edit, record a completion item in `Progress`. Before committing source changes, provide this milestone’s commit message in the completion response, but do not create a commit unless explicitly requested:

    fix(discrete-color-picker): allow deselecting color rows

### Milestone 3: Guarantee three color items fit before wrapping

Edit `src/Vixen.Common/DiscreteColorPicker/Views/DiscreteColorPickerViewBase.cs`, the shared view-specific window-sizing base class used by the single and multiple dialogs. Retain its existing square-grid calculation and 250-pixel button-layout minimum. Add a `MinimumColorItemsPerRow` constant with value `3`, calculate the width needed for three items using the existing `itemWidth` and 10-pixel allowance, and use the larger of that value and 250 pixels when applying the minimum width.

Document the protected `ConfigureWindowSize` method in the same edit. Its parameters must state what each value represents, and its remarks must state that the method guarantees room for at least three color items before the wrap panel creates a new row. Do not change either view's `ConfigureWindowSize` call or add a per-view sizing special case.

The resulting minimums must be 286 pixels for the multiple picker (`3 * 92 + 10`) and remain 250 pixels for the single picker (`max(250, 3 * 72 + 10)`). At the end of this milestone, record the completion in `Progress`, update the Jira issue description and acceptance criteria to include this behavior, and provide this commit message before committing source changes:

    VIX-3731 Keep three colors on one row

### Milestone 4: Build and prove the dialog behavior

First build the modified project from the repository root. Then run the existing repository test project to catch regressions in the broader supported test suite. A successful build/test does not replace the manual dialog verification because WPF mouse and keyboard selection behavior must be observed in a running window.

Open a Vixen workflow that displays the multiple picker, such as editing a gradient point in the Color Gradients application module or in the Effect Editor’s inline gradient editor. Use a gradient point whose targets have at least two valid discrete colors. Execute the interaction cases in `Validation and Acceptance`, recording the Vixen version/configuration and exact result in both the Jira comment and this plan’s `Outcomes & Retrospective` section. Do not alter caller code if clearing the final selection deletes the gradient point; that is the intentional existing contract.

At the end of the milestone, update VIX-3731 one final time. If implementation revealed a requirement or test-plan correction, make that adjustment first. Add a Jira comment with the actual build, automated-test, and manual-test results, including failures or skipped validation and why. Mark the final `Progress` items accurately and update this document’s living sections. Before committing plan-only status changes, provide this milestone’s commit message in the completion response, but do not create a commit unless explicitly requested:

    docs(plans): record VIX-3731 validation results

## Concrete Steps

All commands run in `C:\Dev\Vixen` unless stated otherwise. They are read-only or build/test operations and can be rerun safely.

1. Inspect the status and relevant source before editing. If `MultipleDiscreteColorPickerView.xaml` already has unrelated edits, preserve them and reconcile only the three intended attribute/binding changes; do not overwrite the file.

        git status --short
        Get-Content -Raw src\Vixen.Common\DiscreteColorPicker\Views\MultipleDiscreteColorPickerView.xaml
        Get-Content -Raw src\Vixen.Common\DiscreteColorPicker\ViewModels\MultipleDiscreteColorPickerViewModel.cs
        Get-Content -Raw src\Vixen.Common\DiscreteColorPicker\ViewModels\ColorItems\MultiSelectColorItem.cs

2. Update VIX-3731 as described in Milestone 1, following `.agents/skills/jira/SKILL.md`. Include this concise issue content:

        Make the multiple discrete color picker toggle a color when a user clicks its ListBox row or presses Space.
        In MultipleDiscreteColorPickerView.xaml, use ListBox SelectionMode="Multiple" and bind both
        ListBoxItem.IsSelected and CheckBox.IsChecked two-way to MultiSelectColorItem.CheckBoxSelected.
        Use a picker-local ListBoxItem template with no IsSelected trigger so the checkbox is the only indicator.
        CheckBoxSelected remains the sole accepted-result state because GetSelectedColors filters it and
        ProcessSelectedItem initializes it. Do not add code-behind, event handlers, commands, or use Extended mode.
        Zero selected colors is valid and existing gradient callers delete their point/color in that case.

3. Make the exact scoped XAML changes described in Milestone 2. Review the resulting focused diff:

        git diff --check
        git diff -- src/Vixen.Common/DiscreteColorPicker/Views/MultipleDiscreteColorPickerView.xaml

    The relevant portion should be equivalent to this; retain the repository’s tab indentation and existing surrounding attributes:

        <ListBox Grid.Row="0" ItemsSource="{Binding Colors}"
                 SelectedItem="{Binding Path=SelectedItem, Mode=TwoWay}"
                 SelectionMode="Multiple"
                 ...>
            ...
            <ListBox.ItemContainerStyle>
                <Style TargetType="{x:Type ListBoxItem}">
                    <Setter Property="IsSelected" Value="{Binding CheckBoxSelected, Mode=TwoWay}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="{x:Type ListBoxItem}">
                                <Border Background="Transparent">
                                    <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                                      VerticalAlignment="{TemplateBinding VerticalContentAlignment}" />
                                </Border>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </Style>
            </ListBox.ItemContainerStyle>
            ...
            <CheckBox IsChecked="{Binding CheckBoxSelected, Mode=TwoWay}"></CheckBox>

4. Apply the shared sizing refinement in `src/Vixen.Common/DiscreteColorPicker/Views/DiscreteColorPickerViewBase.cs` as described in Milestone 3. The minimum-width branch must compare `Width` against the larger of 250 and `3 * itemWidth + 10`; it must not hard-code the 286-pixel multiple-picker value.

5. Build the affected project. A clean build prints `Build succeeded.` and exits with code 0. Warnings already present outside the edit may be recorded but must not be silently treated as errors caused by this work.

        msbuild src\Vixen.Common\DiscreteColorPicker\DiscreteColorPicker.csproj -m -t:Restore,Rebuild -p:Configuration=Release -p:Platform=x64

6. Run the existing unit test suite. It should exit successfully with no failed tests. The suite may take longer than the focused build because it references numerous Vixen modules.

        dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release -p:SolutionDir="C:\Dev\Vixen\\"

7. Perform the manual dialog test in a Debug or Release Vixen build. If no current executable is available, build the solution first:

        msbuild Vixen.sln -m -t:Restore,Rebuild -p:Configuration=Debug -p:Platform=x64

    Start Vixen from the generated Debug output using the project’s normal local launch procedure. Navigate to a color-gradient or inline-gradient point that opens `MultipleDiscreteColorPickerView`; avoid the single-color picker for the toggle checks. Follow every scenario in the next section.

8. Add the results to VIX-3731, then revise this plan’s `Progress`, `Surprises & Discoveries`, `Decision Log`, `Outcomes & Retrospective`, and the change note below. Include commands, pass/fail counts, manual observations, and any deviation. Review all changes before requesting a commit:

        git diff --check
        git status --short
        git diff -- docs/plans/vix-3731-discrete-color-deselection.md src/Vixen.Common/DiscreteColorPicker/Views/MultipleDiscreteColorPickerView.xaml

## Validation and Acceptance

The change is accepted only when the project build and existing test suite succeed and a person has observed the following in the running multiple discrete color picker:

1. Start with at least two available discrete colors and no initially selected colors. Click an unchecked color rectangle. Its checkbox becomes checked, while its ListBox row shows no selected, highlighted, bordered, or other selection visual.
2. Click the same checked rectangle again. Its checkbox clears and its row remains visually unchanged. This proves an ordinary row click can deselect without exposing ListBox selection.
3. Click a checkbox directly once. It changes state exactly once, and the row remains visually unchanged except for the checkbox. It must not flip back because of a second event.
4. Click another location inside a row’s active hit area, such as the margin or colored rectangle. It toggles the checkbox without displaying a ListBox selection indicator.
5. Check two or more colors by clicking their rows. Each checkbox remains independently checked; no row renders selected.
6. Use keyboard navigation to focus a row. Press Space once and observe its checkbox becomes checked; press Space again and observe it becomes unchecked. Neither action may render selection styling.
7. Deselect the final selected color and press **OK**. `GetSelectedColors()` returns no colors, and the invoking gradient workflow performs its existing deletion behavior without an error.
8. Open the multiple picker with one or more initially selected colors. Those colors start checked without row selection styling, and newly checked or cleared rows produce the expected accepted result after **OK**.
9. Change selections, press **Cancel**, reopen the picker, and verify the underlying gradient was not committed by the canceled dialog.
10. Open the separate single discrete color picker and confirm its normal one-color selection behavior is unchanged.
11. Open the multiple picker with three available colors. Its initial window width is at least 286 pixels, all three items appear on the first row, and no horizontal wrap occurs. Open the single picker to confirm its 250-pixel minimum and existing layout remain unchanged.

Expected build evidence is a zero exit code and text comparable to:

    Build succeeded.
        0 Error(s)

Expected test evidence is a zero exit code with a summary comparable to:

    Passed!  - Failed:     0, Passed: <existing total>, Skipped:     <existing total>, Total: <existing total>

The exact current test count is not prescribed because this plan does not add tests; the required invariant is zero failures.

## Idempotence and Recovery

The XAML edit is idempotent when it produces exactly one `SelectionMode="Multiple"` attribute, exactly one two-way `CheckBoxSelected` binding for each of the ListBox item container and checkbox, and one local `ListBoxItem` template that has a transparent border but no `IsSelected` trigger. Before retrying an interrupted edit, inspect `git diff` and the full XAML file; do not add duplicate attributes, setters, or templates.

No data migration or destructive operation is involved. To revert only an uncommitted implementation change if validation proves it incorrect, manually restore the modified XAML expressions to their original values after confirming the target file and preserving unrelated work: remove `SelectionMode="Multiple"`, remove the local ListBoxItem template, bind `ListBoxItem.IsSelected` back to `IsSelected`, and remove the explicit checkbox `Mode=TwoWay`. Do not use a broad Git reset or checkout that could discard other contributors’ changes.

If the focused project build cannot resolve the solution directory or shared build paths, run the documented full solution build in Concrete Step 6 and record the exact failure in Jira. If WPF selection styling does not track checkbox clicks as required, do not introduce event handlers as an unplanned workaround; stop, capture the observed binding values, and revise this ExecPlan’s decision log and issue description before choosing a new design.

## Artifacts and Notes

The expected source diff is deliberately limited to `src/Vixen.Common/DiscreteColorPicker/Views/MultipleDiscreteColorPickerView.xaml` and `src/Vixen.Common/DiscreteColorPicker/Views/DiscreteColorPickerViewBase.cs`:

    - <ListBox Grid.Row="0" ItemsSource="{Binding Colors}"
    + <ListBox Grid.Row="0" ItemsSource="{Binding Colors}"
    +          SelectionMode="Multiple"
    ...
    - <Setter Property="IsSelected" Value="{Binding IsSelected}"/>
    + <Setter Property="IsSelected" Value="{Binding CheckBoxSelected, Mode=TwoWay}" />
    + <Setter Property="Template">
    +     <Setter.Value>
    +         <ControlTemplate TargetType="{x:Type ListBoxItem}">
    +             <Border Background="Transparent">
    +                 <ContentPresenter ... />
    +             </Border>
    +         </ControlTemplate>
    +     </Setter.Value>
    + </Setter>
    ...
    - <CheckBox IsChecked="{Binding Path=CheckBoxSelected}"></CheckBox>
    + <CheckBox IsChecked="{Binding CheckBoxSelected, Mode=TwoWay}"></CheckBox>

The sizing diff in `DiscreteColorPickerViewBase.cs` is equivalent to:

    const int MinimumWidth = 250;
    const int MinimumColorItemsPerRow = 3;
    int minimumWidthForColorItems = MinimumColorItemsPerRow * itemWidth + 10;
    int minimumWidth = Math.Max(MinimumWidth, minimumWidthForColorItems);
    if (Width < minimumWidth)
    {
        Width = minimumWidth;
    }

No view-model, caller, code-behind, solution, package, or automated-test file is expected to change for VIX-3731. The shared view-specific sizing base class changes only to guarantee three items fit before wrapping. The ExecPlan itself is an implementation artifact and will be updated as work is performed.

## Interfaces and Dependencies

There are no new interfaces, public APIs, NuGet packages, services, commands, or files. The implementation relies on existing WPF controls from the `PresentationFramework` stack and existing Catel property-change support.

The post-change binding contracts are:

    In src/Vixen.Common/DiscreteColorPicker/Views/MultipleDiscreteColorPickerView.xaml:
        ListBox.SelectionMode = Multiple
        ListBoxItem.IsSelected <-> MultiSelectColorItem.CheckBoxSelected (internal input state only)
        CheckBox.IsChecked     <-> MultiSelectColorItem.CheckBoxSelected
        ListBoxItem.Template   = transparent hit surface with no IsSelected trigger

    In src/Vixen.Common/DiscreteColorPicker/ViewModels/MultipleDiscreteColorPickerViewModel.cs (unchanged):
        public IEnumerable<Color> GetSelectedColors()

`GetSelectedColors()` must continue returning `Colors.Where(item => item.CheckBoxSelected)`, including an empty collection. `ProcessSelectedItem(MultiSelectColorItem selectedItem)` must continue setting `selectedItem.CheckBoxSelected = true` during the shared initial-selection path. `ColorItem.IsSelected` remains available for that shared initializer but is not the multi-picker’s active selection binding after this change. The multi-picker’s `ListBoxItem.IsSelected` is intentionally internal and must never provide a visual indicator.

### Critical files

- `docs/plans/vix-3731-discrete-color-deselection.md` — this living ExecPlan; update it throughout implementation.
- `src/Vixen.Common/DiscreteColorPicker/Views/MultipleDiscreteColorPickerView.xaml` — the only production source file to edit.
- `src/Vixen.Common/DiscreteColorPicker/Views/DiscreteColorPickerViewBase.cs` — shared dialog sizing; update its minimum width calculation and protected-method XML documentation.
- `src/Vixen.Common/DiscreteColorPicker/ViewModels/MultipleDiscreteColorPickerViewModel.cs` — read-only confirmation of initialization and accepted-result behavior.
- `src/Vixen.Common/DiscreteColorPicker/ViewModels/ColorItems/MultiSelectColorItem.cs` — read-only confirmation of the authoritative Boolean property.
- `src/Vixen.Common/DiscreteColorPicker/Views/SingleDiscreteColorPickerView.xaml` — read-only regression check; do not edit.
- `src/Vixen.Modules/App/ColorGradients/GradientEditPanel.cs` and `src/Vixen.Modules/Editor/EffectEditor/Controls/BaseInlineGradientEditor.cs` — read-only confirmation that zero selected colors retains defined deletion behavior.

## Change Note

2026-08-05 / Codex: Created the initial ExecPlan from the Sol architecture handoff. The plan records a XAML-only `SelectionMode="Multiple"` and two-way binding solution, excludes public API and caller changes, and includes the required Jira pre-implementation and final-validation milestones.

2026-08-05 / Codex: Completed Milestone 1 by replacing VIX-3731’s symptom-only description with the final scoped requirements, acceptance criteria, validation plan, and this plan’s repository path. Jira confirmed the issue was already an in-progress Bug; no tracker constraint blocked the next milestone.

2026-08-05 / Codex: Completed Milestone 2 with the planned three-expression XAML change in `MultipleDiscreteColorPickerView.xaml`. The container selection and checkbox now share `CheckBoxSelected`, and WPF `Multiple` mode owns ordinary click and Space toggling. No code-behind, commands, model, view-model, caller, or single-picker changes were made.

2026-08-05 / Codex: Revised Milestone 2 after the requirement clarified that the checkbox is the sole selection indicator. Added a picker-local transparent `ListBoxItem` template with no selected-state trigger, updated VIX-3731’s requirements and acceptance criteria, and revised this plan so WPF selection remains internal while row highlighting is prohibited.

2026-08-05 / Codex: Added and completed the shared sizing refinement after the requirement clarified that three colors are common. `ConfigureWindowSize` now enforces the larger of its legacy button-layout minimum and three item widths, so the multiple picker does not wrap three 92-pixel items while the single picker keeps its existing 250-pixel minimum. The original validation milestone is renumbered to Milestone 4.

2026-08-05 / Codex: Built `DiscreteColorPicker` in Release after the sizing refinement. The project compiled with 0 errors; four recorded warnings originated in existing `Vixen.Core` files outside this change.

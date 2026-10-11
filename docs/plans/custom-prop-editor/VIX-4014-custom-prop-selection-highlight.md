# Restore immediate pixel selection highlighting in the Custom Prop Editor (VIX-4014)


This ExecPlan is a living document maintained in accordance with `.agents/PLANS.md`. Keep Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective current. This document contains the architecture design and active execution plan; milestone 2 is complete, and milestone 3 remains pending.

## Purpose / Big Picture


A pixel selected in the Custom Prop Editor viewer must immediately show the configured selection color, normally pink. Clicking blank space must immediately remove the selection color. Users must be able to trust that the viewer colors, element tree, and movement handles describe the same selection after each interaction, including Control-clicking several pixels.

The issue is [VIX-4014: Highlighting pixel in custom prop editor issue](https://vixenlights.atlassian.net/browse/VIX-4014). Its reproduction comment, ID 40579, reports that viewer colors lag one selection behind, clearing selection leaves stale pink pixels, tree selection works, movement includes the actual selected pixels, and State item selection works. The fix will restore the viewer selection notification order used by the working tree path.

## Progress


- [x] (2026-10-11 14:08Z) Read VIX-4014 and reproduction comment 40579; inspected attachment metadata. The attachment could not be downloaded without authenticated attachment access.
- [x] (2026-10-11 14:08Z) Read the project analyze-and-plan-issue skill and `.agents/PLANS.md`; consulted Custom Prop Editor State requirements and implementation notes.
- [x] (2026-10-11 14:08Z) Established the source-level cause and bounded production changes to three private PropDesigner methods. Rider test discovery reports no existing PropDesigner tests.
- [x] (2026-10-11 14:08Z) Prepared this design and execution plan. The initial working tree was clean.
- [x] (2026-10-11 14:15Z) Milestone 1: Updated Jira VIX-4014 with user-facing Summary, Scope, Acceptance Criteria, and Testing sections. Read the issue back and confirmed the original report, reproduction comment 40579, attachment `vixen4.png` (18208), Accepted status, and unassigned state were preserved. No status, assignee, or comment changes were made.
- [x] (2026-10-11 14:42Z) Milestone 2: Corrected viewer notification ordering, added actual WPF regression tests, and completed automated and manual acceptance. Focused tests passed 3/3; Custom Prop Editor tests passed 148/148; the Release `Vixen_Tests` target build and Release solution rebuild succeeded; Rider file analysis found no problems in either changed C# file; and `git diff --check` passed. The user manually confirmed single selection, replacement, Ctrl-click multi-selection, rectangle selection/removal and additive behavior, click-anywhere clearing (including repeated clear), tree synchronization, movement and handle behavior, Layout/Order regression expectations, and State Definition assignment/preview behavior except for the separate Layout/Order tab-switch issue recorded under Surprises & Discoveries. That switch issue predates this change and is deferred to another issue by user direction. Gortex detected the three changed production methods, found no configured guards, reported no indexed test edges for the changed methods because the new test file is untracked, and issued a behavioral-risk warning for review.
- [ ] Milestone 3: Record validation and reconcile Jira with the completed change.

## Surprises & Discoveries


Observation: The apparent off-by-one symptom is caused by synchronous notification ordering, rather than pixel indexing. In `src/Vixen.Modules/App/CustomPropEditor/Controls/PropDesigner.cs`, `_drawingCanvas_MouseLeftMouseDown` adds a newly selected light to SelectedModels before setting IsSelected. Both the ordinary-click and Control-click addition branches have this order. `DrawingPanelViewModel.SelectedItems_CollectionChanged` immediately calls ApplyNormalLightColors when State preview is inactive, and that method reads IsSelected to choose DisplayColor. IsSelected does not independently refresh DisplayColor.

Evidence, current click order:

    SelectedModels.Add(l);
    l.IsSelected = true;

Evidence, color computation in `src/Vixen.Modules/App/CustomPropEditor/ViewModels/DrawingPanelViewModel.cs`, ApplyNormalLightColors:

    lightViewModel.DisplayColor = lightViewModel.IsSelected ? GetSelectedLightColor() : GetLightColor();

Observation: ClearSelections clears SelectedModels before setting item flags false. Consequently, its Reset notification paints the previously selected pixels pink before their flags are cleared. UpdateSelection has the same reversed order in its non-Control rectangle deselection branch. Control-click removal already sets the flag false first, explaining the original reporter's observation that removing a highlight works.

Observation: The tree path provides the working reference. DrawingPanelViewModel.Select sets each light's IsSelected flag before SelectedItems.AddItems; DeselectAll sets all flags false before SelectedItems.Clear. PropEditorViewModel suppresses recursive tree/viewer synchronization with `_selectionChanging`, so tree synchronization during a viewer-originated event does not repair the viewer's premature color computation.

Observation: Movement handles appear only when SelectedModels.Count is greater than one, by design in PropDesigner.UpdateResizeAdorner. Their absence for a single selected pixel is not an additional defect to fix here.

Observation: State Definition mode uses explicit assignment operations and temporary preview colors. The primary repository reference is `docs/plans/state/vix-3929-custom-prop-editor-state-requirements.md`, particularly State Item Assignment and Custom Prop Editor Preview. The related implementation plan, `docs/plans/state/vix-3929-custom-prop-editor-state.md`, documents temporary LightViewModel.DisplayColor values and restoration of configured normal colors on leaving that mode. The current code agrees with those requirements. Older test commands in the State requirements omit the full-MSBuild C++/CLI prerequisite; use the current AGENTS.md workflow below.

Observation: Rider findTests for VixenModules.App.CustomPropEditor.Controls.PropDesigner returned: "There are no existing tests for VixenModules.App.CustomPropEditor.Controls.PropDesigner". Existing drawing color and State tests are in `src/Vixen.Tests/App/CustomPropEditor/State/CustomPropStateEditorViewModelTests.cs`; they use xUnit and the CustomPropEditor test collection. They do not exercise viewer-originated collection notifications. Milestone 2 added `src/Vixen.Tests/App/CustomPropEditor/PropDesignerSelectionTests.cs` to exercise actual PropDesigner click and clear paths with the DrawingPanelViewModel color observer. Its fixture temporarily installs a `ConfigurationService.Config` with explicit Blue/HotPink colors and restores the prior service value in cleanup.

Observation: The issue contains `vixen4.png`, attachment 18208. The public download endpoint returned "You do not have permission to view attachment with id: 18208". The image itself was not inspected. The detailed reproduction and decisive source ordering are sufficient for this design. No interactive runtime reproduction or application tests have been performed during analysis.

## Decision Log


Decision: Correct selection mutation ordering inside PropDesigner rather than changing the normal color algorithm. Rationale: The tree already demonstrates the intended order, and preserving it makes synchronous observers see the final flags. This repairs additions, resets, and rectangle removals at their source. Date/Author: 2026-10-11, Codex.

Decision: Keep the production edit limited to `_drawingCanvas_MouseLeftMouseDown`, ClearSelections, and UpdateSelection. Rationale: No data migration, new fields, public API, event subscription, asynchronous dispatch, or rendering lifecycle change is needed. Date/Author: 2026-10-11, Codex.

Decision: Test the actual private control paths using a small STA fixture and reflection, consistent with existing tests' use of reflection for private behavior. STA means a Windows thread initialized as a single-threaded apartment, required to construct WPF controls. Rationale: Merely adding items to a view-model collection in a test would miss the ordering bug in the viewer. Preserve production accessibility and avoid introducing a helper solely for tests. Date/Author: 2026-10-11, Codex.

Decision: Use manual Control-click and rectangle interaction checks alongside automated click/clear tests. Rationale: The current handler reads the physical keyboard state, and rectangle selection needs materialized visual containers. Do not add operating-system input injection or redesign keyboard handling for this small fix. Date/Author: 2026-10-11, Codex.

Decision: Follow analyze-and-plan-issue's explicit milestone review stops, and do not create commits automatically. Rationale: Its stopping instructions are more specific than PLANS.md's general continuation advice, and AGENTS.md requires explicit authorization to commit. Jira updates below are future execution work; the present design request does not execute them. Date/Author: 2026-10-11, Codex.

Decision: Update only the VIX-4014 description in Milestone 1, retaining the reporter's original text under an Original report heading and leaving workflow fields and history untouched. Rationale: This adds concise, reviewable user-facing requirements without discarding the reproduction context. The updated issue was read back; its original report, comment 40579, attachment 18208, Accepted status, and unassigned state remain present. Date/Author: 2026-10-11, Codex.

## Outcomes & Retrospective


Analysis produced a source-backed cause and Milestone 1 updated Jira VIX-4014's description while preserving the original report, reproduction comment, and attachment. Milestone 2 contains the synchronous selection-order fix and three real WPF regression tests. The focused tests passed 3/3 and the Custom Prop Editor suite passed 148/148; both required Release builds, Rider file analysis, whitespace validation, and the user's manual acceptance sequence passed. The only observed Layout/Order tab-switch issue was confirmed by the user to predate this fix and is deferred to another issue; it is unrelated to immediate viewer selection highlighting. The key lesson is that selection flags must be finalized before publishing collection changes.

## Context and Orientation


The repository root is `C:\Dev\Vixen`. The detected IDE is JetBrains Rider with callable test-discovery and file-analysis tools. Production code is in the CustomPropEditor module, whose namespace is VixenModules.App.CustomPropEditor. The viewer control PropDesigner inherits WPF MultiSelector. Its SelectedModels list is the external selection collection, while ItemsSource contains selectable LightViewModel instances. Each light has an IsSelected flag and a DisplayColor used for its visible color. An observable collection immediately notifies subscribers when an item is added, removed, or the list is cleared.

`src/Vixen.Modules/App/CustomPropEditor/ViewModels/DrawingPanelViewModel.cs` owns SelectedItems and updates normal light colors on collection notifications. `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs` synchronizes that selection with the element tree and element order views. The proposed changes are entirely within the visual interaction control; retain those view-model responsibilities.

The project skills used for this design are analyze-and-plan-issue, jira, dotnet-best-practices, catel-mvvm, and csharp-docs from `.agents/skills/`. Their effect is a bounded synchronous fix that preserves existing Catel properties and State color ownership, with documented public test APIs. No interface, object lifetime, or asynchronous code change is proposed, so design-pattern and async skills are unnecessary.

## Architecture Design: VIX-4014


### Core Strategy


For each viewer-originated transition, first finish the relevant IsSelected assignments, then mutate SelectedModels to notify observers. The collection remains the existing route for color refresh, tree synchronization, command availability, and movement handles. This is the same strategy as the working tree selection path.

### Data Model & Property Contracts


There are no new fields, configuration settings, interfaces, or persistence changes. Retain ISelectable.IsSelected, LightViewModel.DisplayColor, PropDesigner.SelectedModels, and DrawingPanelViewModel.SelectedItems. At each normal selection collection notification, every affected item's IsSelected flag must match the intended selection. Clear all flags before the Reset notification, rather than clearing each flag after Reset.

### Mathematical / Boundary Logic


For normal mode, the color is C(light) = selected color if light.IsSelected is true, otherwise normal light color. GetSelectedLightColor and GetLightColor retain their existing explicit-property, configuration, and default fallback behavior. Tests should set distinct explicit colors to avoid depending on the user's settings.

An unselected click becomes true then Add. A Control-click removal remains false then Remove. A blank-space reset becomes all flags false then Clear. A rectangle removal becomes false then Remove. Rectangle additions already become true then Add. Repeated clicks on an already selected pixel keep their current behavior; empty selection and null ItemsSource keep the current ClearSelections guards. Do not change rectangle containment or Control retention semantics.

In State Definition mode, retain the existing assignment early returns and disabled-editing checks. Assigned nodes keep State item colors and unassigned nodes keep the configured State preview base color; normal selection refresh remains suppressed while that preview is active.

### Subsystem Component Matrix


`src/Vixen.Modules/App/CustomPropEditor/Controls/PropDesigner.cs` is the only production file to edit. Its three private methods will publish collection changes after flags are finalized.

`src/Vixen.Modules/App/CustomPropEditor/ViewModels/DrawingPanelViewModel.cs`, `src/Vixen.Modules/App/CustomPropEditor/ViewModels/LightViewModel.cs`, and `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs` are reference and regression context. Their existing property and event behavior remains the contract.

Create `src/Vixen.Tests/App/CustomPropEditor/PropDesignerSelectionTests.cs` for the new viewer-originated regression tests. Reuse the CustomPropEditor collection and the model construction patterns from the existing State tests. Run those State tests as regression coverage.

## Plan of Work


### Milestone 1: Align the issue with the observable fix


Context: This milestone changes Jira VIX-4014's requirements when implementation is authorized. Preserve the original report, reproduction comment, and attachment. Read the current issue again before updating so newer information is not overwritten.

Plan of Work: Using the project jira skill, retain the existing report and add concise user-facing Summary, Scope, Acceptance Criteria, and Testing sections. Describe immediate selection colors for ordinary click, Control-click additions and removals, blank-space clearing, replacement selection, and rectangle selection. State that tree selection, movement, and State Definition assignment/preview must continue working. Keep formulas, internal method names, and test fixtures in this repository plan.

Validation and Acceptance: Read the updated issue back and confirm it preserves the original report while expressing all behaviors in the Validation and Acceptance section below. Update Progress and the Decision Log with the result. Do not change status, assignee, or post a completion comment in this milestone.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run git status --short and git diff for this plan, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4014 as the subject prefix, and output its paste-ready Commit message block. Do not create a commit. Wait for explicit user confirmation before advancing.

### Milestone 2: Correct viewer ordering and prove the result


Context: Work in `src/Vixen.Modules/App/CustomPropEditor/Controls/PropDesigner.cs` and the new `src/Vixen.Tests/App/CustomPropEditor/PropDesignerSelectionTests.cs`. Inspect git status first and preserve unrelated edits. Use Gortex to read exact current methods, assess impact, and apply edits. No declaration or signature changes are required in production.

Plan of Work: Add failing regression tests before changing the control. In Rider, apply changes at method-block level using Apply snippet from chat when available; automation must use Gortex edit/refactor. Then reorder both newly selected light branches in `_drawingCanvas_MouseLeftMouseDown` to:

    l.IsSelected = true;
    SelectedModels.Add(l);

In ClearSelections, keep its SelectedModels.Count guard and ItemsSource null guard. Move SelectedModels.Clear after the entire existing loop that sets all ISelectable flags false:

    if (SelectedModels.Count > 0)
    {
        if (ItemsSource != null)
        {
            foreach (ISelectable lvm in ItemsSource)
            {
                lvm.IsSelected = false;
            }
        }
        SelectedModels.Clear();
    }

In UpdateSelection's normal-mode rectangle removal branch, reorder only these statements:

    item.IsSelected = false;
    SelectedModels.Remove(item);

Keep Control-click removal, rectangle addition, State Definition branches, resize logic, and tree coordination intact. Add a short comment at the clearing or click transition explaining that synchronous selection observers compute colors from IsSelected. Do not add duplicate refresh calls, Dispatcher callbacks, or new selection subscriptions.

Test Design: Create a public sealed PropDesignerSelectionTests class in the test project's existing namespace convention, marked Collection("CustomPropEditor"). Document the public class and public test methods with the project csharp-docs skill. Execute WPF control setup and assertions on a dedicated STA thread; capture any exception, join the thread with a bounded timeout, and rethrow the captured exception on the xUnit thread. Tear down any created controls and detach test event listeners. Do not manipulate physical keyboard state.

Construct an in-memory prop through PropModelServices.Instance().CreateProp, with a model containing two leaf elements and one Light per leaf. The State tests' CreateServicePropWithLights shows this setup. Build ElementTreeViewModel and DrawingPanelViewModel from it. Set explicit LightColor = Color.Blue and SelectedLightColor = Color.HotPink. Bind a PropDesigner instance's ItemsSource to drawing.LightNodes and SelectedModels to drawing.SelectedItems. Keep IsDrawing false and IsStateDefinitionMode false. Inject an unattached Canvas into the private `_drawingCanvas` field through reflection; this supports the click handler's coordinate and focus calls without opening a window. Set MouseButtonEventArgs.Source to a WPF Path whose DataContext is the target LightViewModel, then invoke `_drawingCanvas_MouseLeftMouseDown` through reflection with that Canvas as sender. Use Mouse.PrimaryDevice and MouseButton.Left for the event arguments. Confirm Control is not physically held during these ordinary-click tests.

Add three focused tests: ViewerClick_SelectsAndHighlightsFirstLightImmediately; ViewerClick_ReplacesSelectionAndRestoresPreviousLightColor; ViewerClearSelections_ClearsFlagsBeforeResetAndRestoresNormalColors. The first two exercise the actual handler and assert SelectedItems membership, IsSelected, and DisplayColor immediately after the call, without triggering any later refresh. The clear test preselects both light flags before adding them to the drawing selection, binds the control after preparing that selection, then invokes private ClearSelections. Capture Reset observer state and assert all flags are false when Reset is delivered, the selection is empty, and both colors are normal immediately. Binding after preselection avoids invoking resize geometry against unmaterialized containers while building the fixture. Capture observer snapshots and assert afterward rather than throwing inside production event handlers.

All three tests should fail before the production reorder: the newly clicked light keeps the normal color, replacement leaves stale colors, and Clear observes old flags and leaves pink. If fixture setup fails before assertions, fix the test setup; do not count a WPF initialization error as reproducing the bug. The existing State tests remain the automated acceptance for temporary preview colors and assignment behavior. Rectangle deselection and Control-click addition/removal are covered by the manual checks below.

Concrete Steps: From `C:\Dev\Vixen`, build tests with full Visual Studio MSBuild before using dotnet test. Use a Visual Studio Developer PowerShell where msbuild and the C++ toolset are available. Run the focused new tests before and after the change:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~PropDesignerSelectionTests"

Before the fix, expect the three behavioral assertions described above to fail. After the fix, expect all three new tests to pass. Then run the surrounding Custom Prop Editor tests:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~App.CustomPropEditor"

Build the affected solution configuration for the application check:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64
    git diff --check

Expect no build errors, no failed tests, and no whitespace errors. Record actual test counts and command output; do not invent a total. Start `Release/Output/VixenApplication.exe` from the build output to perform the manual acceptance checks below. If the entry executable name differs in the current build, use Rider's configured Vixen application run configuration.

After editing, call Gortex change.detect, then change.tests, change.guards, and change.contract with the returned changed symbol IDs. Run Rider get_file_problems on PropDesigner.cs and PropDesignerSelectionTests.cs. Resolve only findings introduced on changed lines. Use Rider Test Runner to inspect the new regression results; a coverage snapshot is optional and must not replace exercising the actual viewer paths.

Validation and Acceptance: Complete the full manual sequence below and record observed results. The tests must prove immediate color changes; a passing build alone is insufficient. Update this plan with final code locations, results, and any fixture changes.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run git status --short and git diff for the two C# files and this plan, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4014 as the subject prefix, and output its paste-ready Commit message block. Do not create a commit. Wait for explicit user confirmation before advancing.

### Milestone 3: Record final user-visible results


Context: Reconcile Jira VIX-4014 and this plan after the implementation and validation are complete. This milestone does not require more application edits unless a failed acceptance check requires returning to Milestone 2.

Plan of Work: Using the jira skill, read the current description, align it with any approved requirement changes, and add a concise completion comment describing immediate highlighting and clearing plus the actual automated and manual validation results. Report anything not tested accurately. Preserve the reproduction history. Update Outcomes & Retrospective and Progress. Do not transition the issue or commit unless separately authorized.

Validation and Acceptance: Read the updated issue and comment back and confirm they agree with the completed change and recorded results.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run git status --short and git diff for any repository files changed in this milestone, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4014 as the subject prefix, and output its paste-ready Commit message block. Do not create a commit. Wait for explicit user confirmation before any further work.

## Concrete Steps


The working directory for every command is `C:\Dev\Vixen`. Milestone 2 contains the exact build and test commands and expected before/after behavior. Start long-running commands with native exec_command and yield_time_ms=10000; poll completion with write_stdin and yield_time_ms of at least 30000. Keep the user informed during long builds.

For this analysis, the initial git status --short returned no entries; the final status listed only this new plan. Rider found no existing PropDesigner tests. The command git diff --no-index --check -- /dev/null docs/plans/custom-prop-editor/VIX-4014-custom-prop-selection-highlight.md emitted no whitespace diagnostics (exit 1 reflects the new-file difference). Rider get_file_problems could not analyze the Markdown file because it is not included in a solution project. Gortex change.detect does not include untracked files; consequently it returned no changed symbols for this plan. No application build, unit test, or interactive application run was performed because the deliverable is a design document.

## Validation and Acceptance


Milestone 2 manual results (2026-10-11): The user confirmed that single and replacement selection, Ctrl-click multi-selection, rectangle selection/removal and additive behavior, blank-space and repeated clearing, tree synchronization, multi-item movement and handle behavior, and State Definition assignment/preview behavior all pass. The user observed that Layout/Order tab switching retains selection only on the first switch and then fails in either direction; the user confirmed this behavior predates VIX-4014 and will be handled under another issue. This unrelated finding is excluded from the VIX-4014 acceptance result.

Open or create a custom prop with at least five separated pixels. Use Layout selection mode rather than draw mode. Start with no selected pixels. Click the first pixel; it must immediately turn the configured selected color. Control-click four more; after each click, every selected pixel, including the newest one, must be selected-colored, and the tree must show the same selection. Move the selection and confirm all selected pixels move together.

Control-click a selected pixel to remove it; its normal color must return immediately and the remaining pixels must remain highlighted. Click blank space without Control; all pixels must immediately regain normal colors, the tree selection must clear, and movement handles must disappear. Click another pixel after a multi-selection; the new pixel must highlight immediately and all former selections must lose the highlight. Repeat clearing when the selection is already empty.

Drag a rectangle to select pixels, then adjust the rectangle to exclude a previously selected pixel without Control; the excluded pixel must immediately regain its normal color. Repeat rectangle selection while holding Control and verify the existing additive behavior. Select a leaf and a group from the tree; the corresponding viewer pixels must highlight immediately. Switch between Layout and Order and verify their existing selection behavior.

Activate State Definition mode with a prop containing State items. Select one item and verify assigned colors, click assignment toggling, drag-box assignment, and Control drag-box removal. With zero or multiple State rows selected, assignments must not change. Preview colors must remain active and normal layout handles must remain disabled in that mode. Return to Layout; normal configured colors and selection highlighting must work immediately. None of these operations should change prop coordinates except explicit movement or change State assignments outside the State Definition workflow.

## Idempotence and Recovery


No schema migration, dependency addition, or external output is involved. The statement reorders can be applied once and recognized by their final order on subsequent runs. Check the current diff before applying any edit. If a build fails due to missing C++ tooling, rerun from Visual Studio Developer PowerShell with the installed x64 C++ toolset; do not try to rebuild C++/CLI dependencies using dotnet's bundled MSBuild. Preserve unrelated working-tree changes. Roll back only the task's reviewed hunks if necessary, without destructive whole-tree Git commands.

## Artifacts and Notes


The intended notification sequence is:

    Viewer interaction
        -> finalize affected IsSelected flags
        -> mutate SelectedModels
        -> DrawingPanelViewModel collection notification
        -> compute DisplayColor from final flags
        -> synchronize tree and update selection affordances

The previous viewer sequence published the collection change before finalizing flags. It explains both the missing newest highlight and stale highlighting after a reset. The reproduction comment is more detailed than the inaccessible screenshot and is the primary issue evidence for this plan.

## Interfaces and Dependencies


Retain PropDesigner : MultiSelector and LightViewModel : ViewModelBase, ISelectable, IDisposable. Retain the current signatures of `_drawingCanvas_MouseLeftMouseDown(object sender, MouseButtonEventArgs e)`, ClearSelections(), and UpdateSelection(Point startPoint, Point endPoint). These methods remain private. Keep SelectedModels as IList and preserve the existing observable selection collection.

Use the existing xUnit test project, WPF, System.Reflection, and System.Threading for the regression fixture. Do not add a new testing package or expose a production method for testing. The new public test class and test methods require XML summaries under the project csharp-docs rules. Production public/protected APIs are not modified.

Plan creation note (2026-10-11): Created from VIX-4014 and comment 40579, current Gortex source evidence, Rider test discovery, and the documented State Definition contracts. Production implementation and further Jira reporting are pending; Milestone 1's requirements update is complete.

Analysis complete and plan integrated with plans.md.

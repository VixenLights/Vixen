# Fix Custom Prop Editor layout-tree drag/drop ordering

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document in accordance with `.agents/PLANS.md` from the repository root. It is self-contained so that an implementer can complete the work without relying on prior chat context. This work is tracked as [VIX-3998](https://vixenlights.atlassian.net/browse/VIX-3998).

## Purpose / Big Picture

The Custom Prop Editor lets a user organize a prop's elements by dragging one or more rows in its Layout tree. Moving several sibling rows downward currently can crash the editor, and other before/after moves can place rows in the wrong order. The crash can also lose an element from its collection before the exception reaches the user. After this change, single-item and multi-item moves work in either direction, keep the dragged order (or the editor's existing Control-key reversed order), and preserve every element, light, parent relationship, and live view-model object that should survive the move.

The exact reported regression starts with siblings `[A, B, C, D, E, F, G]`. Dragging `[B, C, D]` before `G` must finish as `[A, E, F, B, C, D, G]` without an exception. A user can see the complete fix by repeating upward, downward, before, after, and cross-parent drags in the Custom Prop Editor, saving the prop, reopening it, and observing the same hierarchy and order.

## Progress

- [x] (2026-09-12 13:48Z) Read `.agents/PLANS.md`, the repository guidance, the project-specific .NET and Catel MVVM skills, the current drag/drop and model services, the model/view-model collection bridge, the test project configuration, the local GongSolutions.WPF.DragDrop 4.0.0 documentation, and existing Custom Prop Editor tests and plans.
- [x] (2026-09-12 13:48Z) Authored this repository-grounded ExecPlan; no production or test implementation was changed.
- [x] (2026-09-12 14:05Z) Milestone 1: Updated [VIX-3998](https://vixenlights.atlassian.net/browse/VIX-3998) with a user-facing Summary, Scope, Acceptance Criteria, and Validation section covering the exact multi-sibling regression, end-of-list moves, ordering, preservation, and save/reopen behavior. Preserved the issue's Bug type, Custom Prop Editor component, In Progress status, and other workflow metadata.
- [x] (2026-09-12 14:02Z) Milestone 2: Added internal `TryMoveWithinParent` to `PropModelServices`, with complete pre-mutation validation, structured rejection warnings, and the cursor-based `ObservableCollection.Move` algorithm. Added six focused service-level test cases in `ElementTreeDragDropTests`: downward block reorder, end-slot move, selected-block no-op, negative and too-large slots, and empty/duplicate/foreign model requests. `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` completed successfully (pre-existing solution warnings only); the focused `dotnet test` command passed 6/6 tests, 0 failed. The view-model `Drop` integration remains untouched.
- [x] (2026-09-12 14:11Z) Milestone 3: Refactored `ElementTreeViewModel.Drop` to snapshot drag data, prioritize `TargetItemCenter`, call `TryMoveWithinParent` once for same-parent sibling moves using `UnfilteredInsertIndex`, and advance a cross-parent insertion cursor only after each insert before removing the source parent. Added public `Drop` tests for the exact downward regression, after-final-item slots, single upward/downward moves, noncontiguous and Control-reversed selections, within-selection no-ops, cross-parent identity/parent/light preservation, and center-flag append precedence. The full `Vixen_Tests` MSBuild target completed successfully with pre-existing solution warnings; focused `ElementTreeDragDropTests` passed 15/15, 0 failed.
- [ ] Milestone 4: Run automated and manual validation, reconcile the Jira description if discoveries changed the contract, and post the final evidence to VIX-3998.

## Surprises & Discoveries

- Observation: `GongSolutions.Wpf.DragDrop.IDropInfo.UnfilteredInsertIndex` is documented by the locally restored 4.0.0 package as the insertion position in the unfiltered source collection and as a value intended for use during `Drop`. It is a pre-mutation slot, so an end-of-list drop can legitimately provide `Children.Count`.
  Evidence: `Directory.Packages.props` pins `gong-wpf-dragdrop` to 4.0.0, and `%USERPROFILE%\.nuget\packages\gong-wpf-dragdrop\4.0.0\lib\net8.0-windows7.0\GongSolutions.WPF.DragDrop.xml` documents `IDropInfo.UnfilteredInsertIndex` as the unfiltered collection insertion position.

- Observation: `ObservableCollection<T>.Move` accepts the final index of an existing item, not a collection insertion slot. The current `ElementTreeViewModel.Drop` passes `dropInfo.InsertIndex + elementIndex` for downward before-drops and `dropInfo.InsertIndex - 1` for every after-drop. With `[A, B, C, D, E, F, G]`, moving `[B, C, D]` to slot 6 first attempts `Move(B, 6)` and then attempts `Move(C, 7)`, which is outside the valid final-index range `0..6`.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementTreeViewModel.cs`, in `Drop`, and `src/Vixen.Modules/App/CustomPropEditor/Services/PropModelServices.cs`, in `MoveWithinParent`.

- Observation: a failed `ObservableCollection<T>.Move` is not safe to catch and continue. Its implementation can remove the old item before the insert at the invalid destination throws, leaving the collection changed even though the move failed.
  Evidence: the reported failure at `PropModelServices.cs` line 213 removes a dragged item before `ArgumentOutOfRangeException` escapes. The fix must validate the complete batch before the first mutation and must not use exception handling or index clamping as recovery.

- Observation: same-parent and cross-parent moves intentionally need different collection notifications. `ElementViewModelCollection` derives from `Common.WPFCommon.Utils.TransformedCollection`; its `Move` handler reorders the existing `ElementModelViewModel`, while its remove/insert handlers dispose the source-parent view model and create the target-parent view model.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementViewModelCollection.cs` and the `NotifyCollectionChangedAction.Move` branch in `src/Vixen.Common/WPFCommon/Utils/TransformedCollection.cs`.

- Observation: `ElementModel.RemoveParent` treats an element with no remaining parents as deleted: it clears a leaf's lights or recursively detaches a group's descendants. A cross-parent move must therefore attach the destination parent before detaching the source parent.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Model/ElementModel.cs`, in `RemoveParent`, and the current insert-then-remove sequence in `ElementTreeViewModel.Drop`.

- Observation: the test assembly already has internal access to the Custom Prop Editor assembly, includes Moq and xUnit v3, and existing stateful Custom Prop Editor tests use `[Collection("CustomPropEditor")]`. No project or solution edit is needed for the new test file.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/CustomPropEditor.csproj` declares `InternalsVisibleTo` for `Vixen.Tests`, and `src/Vixen.Tests/Vixen.Tests.csproj` already references the module, Moq, and xUnit.

## Decision Log

- Decision: Interpret both before and after sibling drops through `dropInfo.UnfilteredInsertIndex` as one pre-mutation insertion-slot contract; do not add or subtract one based on `RelativeInsertPosition`.
  Rationale: Gong has already resolved whether the pointer is before or after the target when it produces the slot. The unfiltered value addresses the actual model collection, and it remains valid at `Children.Count` for an after-final-item drop.
  Date/Author: 2026-09-12 / Codex

- Decision: Put same-parent index translation in one internal batch method, `TryMoveWithinParent`, rather than reproducing cursor arithmetic in the view model.
  Rationale: the model service owns collection mutation, can validate the complete request before mutation, and can preserve `ObservableCollection.Move` notifications. The view model should translate UI intent into one service call rather than implement collection-index mechanics per dragged row.
  Date/Author: 2026-09-12 / Codex

- Decision: Reject invalid batch metadata before any move, return `false`, and emit one structured NLog warning containing a reason plus parent, insertion-index, child-count, and model-count context.
  Rationale: drag metadata is external UI input, and returning `false` lets `Drop` stop without crashing. Full prevalidation prevents the partial-removal failure mode. Structured fields make future reports diagnosable without interpolated log text. Clamping or catching a failed move would conceal the contract error after data may already have changed.
  Date/Author: 2026-09-12 / Codex

- Decision: Preserve input order with a moving cursor: initialize `cursor` to the insertion slot; for each model, find its current index, decrement `cursor` when that index is before the cursor, move to `cursor` when needed, then set `cursor` to the destination plus one.
  Rationale: recomputing each current index accounts for earlier moves in the same batch. The adjustment converts a pre-mutation insertion slot into a valid final item index and works for upward, downward, contiguous, noncontiguous, end-of-list, and within-selection drops.
  Date/Author: 2026-09-12 / Codex

- Decision: Snapshot the dragged view-model sequence before mutating any collection, then apply the existing Control-key reversal to that snapshot.
  Rationale: cross-parent removal disposes source-parent view models, and enumerating a live selection or transformed collection while changing it is unsafe. A snapshot also freezes the order that the batch service must preserve.
  Date/Author: 2026-09-12 / Codex

- Decision: Test through the public `ElementTreeViewModel.Drop` boundary for all user-facing reorder scenarios, while adding small direct tests for the internal batch method's all-or-nothing validation contract.
  Rationale: public-boundary tests prove the Gong metadata, branch selection, model mutation, view-model identity, and reselection work together. Direct service tests efficiently prove invalid requests return `false` without mutation, a guarantee that normal accepted drop scenarios do not exercise.
  Date/Author: 2026-09-12 / Codex

- Decision: Keep this change limited to `PropModelServices.cs`, `ElementTreeViewModel.cs`, and the new test file.
  Rationale: Gong 4.0.0 already provides the needed slot, the SDK-style projects already include new C# files, and the requested fix does not require XAML, persistence schema, package, project, solution, or public API changes.
  Date/Author: 2026-09-12 / Codex

## Outcomes & Retrospective

Milestones 2 and 3 are complete. `PropModelServices.TryMoveWithinParent` now converts a validated pre-mutation insertion slot to a valid final `Move` index while preserving input order, and `ElementTreeViewModel.Drop` now routes same-parent sibling moves through that operation exactly once. The public drop tests prove the reported crash case, all planned ordering variants, collection/view-model identity, cross-parent parent and light preservation, and center precedence; all 15 focused cases pass. Manual UI and save/reopen validation, final Jira evidence, and any resulting final adjustments remain for Milestone 4. Update this section after every major milestone with what was actually delivered, any gaps, and the build, test, and manual-validation results. At completion, compare the observed tree order, identity, parent IDs, lights, save/reopen state, and invalid-request behavior against the acceptance criteria below.

## Context and Orientation

Vixen is a .NET 10 WPF desktop application. The relevant module is `src/Vixen.Modules/App/CustomPropEditor`. A `Prop` owns an `ElementModel` root node, and each `ElementModel.Children` property is an `ObservableCollection<ElementModel>` whose order is the Layout-tree order. A parent relationship is represented in both directions: the parent contains the child in `Children`, and the child contains the parent's ID in `Parents`. A leaf element can own `Light` objects whose `ParentModelId` points to that leaf; moving a leaf must not recreate or clear those lights.

`src/Vixen.Modules/App/CustomPropEditor/Services/PropModelServices.cs` centralizes tree mutation. Its existing singular `MoveWithinParent(ElementModel parent, ElementModel model, int newIndex)` finds the model's current index and calls `parent.Children.Move(oldIndex, newIndex)`. Keep this singular method and call it from the new batch method so all same-parent movement continues to use `ObservableCollection.Move`.

`src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementTreeViewModel.cs` implements Gong's `IDropTarget`. Its public `Drop(IDropInfo dropInfo)` receives `Data` as dragged `ElementModelViewModel` objects, `TargetItem` as the row under the pointer, `InsertPosition` as a flags enumeration describing center/before/after, `UnfilteredInsertIndex` as a slot in the target model collection, `Effects` as copy or move, and `KeyStates` for the existing Control-key reversal. A slot is a gap between items: a collection of seven items has eight slots numbered 0 through 7. By contrast, `ObservableCollection.Move` must receive an existing final item index numbered 0 through 6.

`ElementTreeViewModel.CanAcceptData` already rejects a mixed sibling selection in which only some dragged items belong to the target parent. Therefore a before/after drop reaching `Drop` is either entirely same-parent or entirely cross-parent. The implementation should still avoid partial mutation if the classification is neither all nor none, because tests and future callers can invoke public `Drop` without first invoking `CanAcceptData`.

Each `ElementModelViewModel` creates an `ElementViewModelCollection` over its model's `Children`. That transformed collection maps model collection notifications to view-model collection notifications. A model `Move` reuses the exact existing child view model. A model remove/insert disposes the old source-parent view model and creates a new target-parent view model. `ElementModelLookUpService` maps a model ID to those live view models and is how `SelectModelWithParent` finds and selects the newly created cross-parent instance.

The test project is `src/Vixen.Tests/Vixen.Tests.csproj`. Add `src/Vixen.Tests/App/CustomPropEditor/ElementTreeDragDropTests.cs` in namespace `Vixen.Tests.App.CustomPropEditor`, decorate it with `[Collection("CustomPropEditor")]`, and use Moq to create `IDropInfo`. The Custom Prop Editor service and lookup objects are singletons, so reset `ElementModelLookUpService.Instance` before and after each test fixture or test and build each prop afresh. Keep the tests synchronous unless actual dispatcher behavior proves that an STA test is required; no window should be shown.

## Plan of Work

### Milestone 1: Align VIX-3998 with the executable specification

Before editing code, read `.agents/skills/jira/SKILL.md` and use the Jira integration to update VIX-3998. The description must state the insertion-slot/index mismatch, the partial-mutation risk, the exact `[A, B, C, D, E, F, G]` regression and expected `[A, E, F, B, C, D, G]` result, the validated batch algorithm, the same-parent versus cross-parent behavior, Control reversal, center-drop precedence, identity/light/parent invariants, the complete automated matrix, and manual save/reopen validation. Keep the issue's existing type, status, ownership, sprint, labels, and other workflow metadata unless separately directed.

This milestone is complete when the Jira description is sufficient to reconstruct the behavior and acceptance criteria in this plan and its URL or update result is recorded in `Progress`. It changes no repository files, so no commit message is required.

### Milestone 2: Add an atomic same-parent batch operation

Edit `src/Vixen.Modules/App/CustomPropEditor/Services/PropModelServices.cs` beside the existing singular `MoveWithinParent` method. Add exactly this internal API:

    internal bool TryMoveWithinParent(
        ElementModel parent,
        IReadOnlyList<ElementModel> models,
        int insertionIndex)

Before the first collection mutation, validate the entire call. A valid `parent` and `models` reference must be supplied; `insertionIndex` must be between zero and `parent.Children.Count`, inclusive; `models` must contain at least one non-null, unique model; and every model must currently appear in `parent.Children`. Treat equality consistently with the `ElementModel` identity contract, whose equality is based on `Id`. If any validation fails, log one structured warning through the existing NLog logger and return `false`. Include stable fields such as `Reason`, `ParentId`, `InsertionIndex`, `ChildCount`, and `ModelCount`; do not use string interpolation for this warning. Do not mutate the collection, catch a move exception, retry, or clamp the slot.

After successful validation, execute this algorithm exactly in the input order:

    cursor = insertionIndex
    for each model in models:
        oldIndex = parent.Children.IndexOf(model)
        if oldIndex < cursor:
            cursor = cursor - 1
        destination = cursor
        if oldIndex != destination:
            MoveWithinParent(parent, model, destination)
        cursor = destination + 1
    return true

The destination derived this way is always a valid final index after complete prevalidation, including when the initial slot equals `Children.Count`. Retain the existing public singular method unchanged in signature and continue to call it so `ObservableCollection.Move` sends a `Move` notification and `TransformedCollection` keeps the existing view models.

Begin `src/Vixen.Tests/App/CustomPropEditor/ElementTreeDragDropTests.cs` with reusable prop/tree builders and direct service tests which prove negative and too-large slots, an empty list, duplicate models, and a model from another parent each return `false` and leave the original child sequence byte-for-byte equivalent by model identity. Also prove that a valid no-op placement returns `true`. Capturing the log event is optional; the production log call itself must be visibly structured in code review.

Run the focused test filter after this milestone. The direct service tests must pass, and no previously built test under the filter may fail. Before handing off the milestone, update all living-document sections and use `.agents/skills/commit-msg/SKILL.md` to output a formatted commit message in the completion response; do not create a commit unless the user explicitly asks.

### Milestone 3: Route sibling drag/drop through the slot-aware service

Refactor only `Drop` and any small private helpers needed in `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementTreeViewModel.cs`. Leave `CanAcceptData`, `DragOver`, XAML, and persistence code unchanged.

At the start of a valid drop, materialize the dragged `IList<ElementModelViewModel>` into a new list. Apply the current Control-key behavior to that snapshot so a Control drop processes the reversed snapshot and an ordinary drop processes the original snapshot. Never enumerate the live `Data` or `SelectedItems` collection while model mutations are occurring. Continue to ignore non-list data and non-move effects as the current method does.

Because `RelativeInsertPosition` is a flags enumeration, test `TargetItemCenter` first with `HasFlag`. The center branch must retain the current append-to-group behavior: for each snapshot item whose source parent differs from the target group, call `AddToParent` before `RemoveFromParent`, then select the new target-parent view model through `SelectModelWithParent`. This branch must not use `UnfilteredInsertIndex`, and it must continue expanding the target group. A combined flags value containing center plus before or after must take the center branch.

For a before or after sibling drop, require a non-null target parent and use `dropInfo.UnfilteredInsertIndex` directly as the one pre-mutation slot. Do not use `InsertIndex`, `elementIndex`, `+1`, or `-1`. Determine the source parent for every snapshot item before mutating.

If all dragged view models already have `targetModelParent`, call `TryMoveWithinParent(targetModelParent.ElementModel, snapshot.Select(item => item.ElementModel).ToList(), insertionIndex)` exactly once. When it returns `true`, reselect the same existing `ElementModelViewModel` objects; this proves the `Move` notification path retained their identity. When it returns `false`, stop without attempting individual moves. If neither all nor none of the items belong to the target parent, return without mutation because such mixed data violates `CanAcceptData`'s established contract.

If none of the dragged items belongs to the target parent, initialize a cross-parent insertion cursor to `UnfilteredInsertIndex`. For each snapshot entry, first call `InsertToParent` with that cursor, increment the cursor once after every successful insert, and only then call `RemoveFromParent` on the previously captured source parent. Select the newly created target-parent view model through `SelectModelWithParent`. Inserting first ensures `ElementModel.RemoveParent` never observes an orphan and therefore cannot clear lights or descendants. Do not change `InsertToParent`, `RemoveFromParent`, `ElementModel.RemoveParent`, or `SelectModelWithParent` for this fix.

Complete `ElementTreeDragDropTests.cs` with Moq-based calls to public `ElementTreeViewModel.Drop`. A common helper should configure at least `Data`, `TargetItem`, `InsertPosition`, `UnfilteredInsertIndex`, `Effects = DragDropEffects.Move`, and `KeyStates`. Set `InsertIndex` to a deliberately different value such as zero in sibling-drop tests so the tests fail if production code accidentally consults the filtered index. Construct real `Prop`, `ElementModel`, `Light`, `ElementTreeViewModel`, and `ElementModelViewModel` objects; mock only `IDropInfo`.

Cover these scenarios and exact outcomes:

- Downward multi-item before regression: from `[A, B, C, D, E, F, G]`, drag `[B, C, D]` to the before-`G` slot 6 and assert `[A, E, F, B, C, D, G]`.
- After the final item: from `[A, B, C, D, E, F, G]`, drag `[B, C, D]` to the after-`G` slot 7 (equal to `Count`) and assert `[A, E, F, G, B, C, D]`.
- Single downward before: drag `B` before `E` at slot 4 and assert `[A, C, D, B, E, F, G]`.
- Single upward after: drag `E` after `B` at slot 2 and assert `[A, B, E, C, D, F, G]`.
- Noncontiguous selection: drag `[B, D]` before `G` at slot 6 and assert `[A, C, E, F, B, D, G]`.
- Control-reversed block: drag `[B, C, D]` before `G` at slot 6 with the Control state and assert `[A, E, F, D, C, B, G]`.
- Drop within the selected block: drag `[B, C, D]` before `D` at slot 3 and assert the unchanged `[A, B, C, D, E, F, G]` order.
- Cross-parent insertion: move `[B, C]` from source `[A, B, C, D]` before `G` in target `[E, F, G]` at target slot 2 and assert source `[A, D]` and target `[E, F, B, C, G]`.
- Center precedence and append: supply a flags value containing `TargetItemCenter` and a sibling-position flag while targeting a different group; assert the dragged items append in snapshot order and do not take the sibling branch.

In every same-parent case, retain references to all original models and the dragged view models. Assert each original model remains in the prop tree exactly once, the child count is unchanged, the reordered transformed collection contains the same view-model instances, and the dragged existing view models are selected after the drop. In the cross-parent case, assert the exact original `ElementModel` instances now occur once under the target, each `Parents` collection contains the target parent ID and not the source parent ID, the exact original `Light` instances and collections survive, each light's `ParentModelId` remains its element ID, the source and target contain no duplicates, the old source-parent view models are replaced by selected target-parent view models, and the target parent is expanded. Apply the same exactly-once invariant to the center test.

Run the full-MSBuild test build and the focused test command from `Concrete Steps`. At the end of this milestone, all `ElementTreeDragDropTests` cases must pass with zero failures. Update the living sections and use `.agents/skills/commit-msg/SKILL.md` to output a formatted commit message; do not commit unless explicitly requested.

### Milestone 4: Prove the complete user workflow and close the Jira loop

Run the full automated validation sequence exactly as listed below. Then launch `Release\Output\Vixen.Application.exe`, open or create a prop with at least two groups and seven sibling elements, and exercise ordinary and Control-modified multi-selection drags. Repeat upward and downward moves, before and after drops (including after the final sibling), a noncontiguous selection, a drop whose pointer lies within the selected block, and moves between groups. After every operation, verify the visible order is exact, no item disappears or duplicates, selection remains on moved rows, group expansion remains usable, and no exception dialog or crash occurs. Include at least one leaf with a visible light and one group with descendants in cross-parent moves so orphan cleanup damage would be observable.

Save the prop, close the Custom Prop Editor, reopen the saved prop, and verify hierarchy, sibling order, light count/placement, and group descendants match the pre-save state. No persistence format or migration should be triggered by this fix.

Read `.agents/skills/jira/SKILL.md` before the final Jira action. If implementation discoveries changed requirements, acceptance criteria, or the test plan, first reconcile the VIX-3998 description. Then add a comment containing the exact build and test commands, their pass/fail counts, `git diff --check`, manual scenarios, save/reopen result, relevant warnings, and any remaining gaps. Do not transition the issue unless separately directed.

This milestone is complete when automated tests pass, manual behavior and save/reopen pass, Jira contains the final evidence, and `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` accurately describe the completed state. Use `.agents/skills/commit-msg/SKILL.md` to output the final formatted commit message; do not create a commit without explicit authorization.

## Concrete Steps

Run every command from the repository root, `C:\Dev\Vixen`, in PowerShell. Begin each implementation session with a hygiene check:

    git status --short

Preserve unrelated user changes. The planned file scope is:

    src/Vixen.Modules/App/CustomPropEditor/Services/PropModelServices.cs
    src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementTreeViewModel.cs
    src/Vixen.Tests/App/CustomPropEditor/ElementTreeDragDropTests.cs
    docs/plans/custom-prop-editor/VIX-3998-fix-layout-tree-drag-drop-order.md

After adding the service tests or completing the Drop integration, build the test target with full Visual Studio MSBuild because transitive C++/CLI projects cannot be built correctly by `dotnet test` alone:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Expected result: MSBuild exits with code 0 and reports no build errors. Record warnings, distinguishing new warnings from pre-existing ones.

Run the already-built focused tests:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)/" --filter "FullyQualifiedName~ElementTreeDragDropTests"

Expected result: every test case in `ElementTreeDragDropTests` passes, with 0 failed. The exact regression test should fail before the production fix with `ArgumentOutOfRangeException` or an incorrect order and pass after it with `[A, E, F, B, C, D, G]`.

Check formatting and whitespace:

    git diff --check

Expected result: no output and exit code 0.

Inspect the final scoped diff and working tree:

    git diff -- src/Vixen.Modules/App/CustomPropEditor/Services/PropModelServices.cs src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementTreeViewModel.cs src/Vixen.Tests/App/CustomPropEditor/ElementTreeDragDropTests.cs docs/plans/custom-prop-editor/VIX-3998-fix-layout-tree-drag-drop-order.md
    git status --short

Expected result: the diff contains only the planned service, view-model, tests, and living-plan updates unless the user has separately authorized other changes.

For manual validation after the Release build, launch:

    .\Release\Output\Vixen.Application.exe

Use the Custom Prop Editor UI and perform the Milestone 4 matrix. Record the prop shape and observed order before and after each operation so the Jira result is reproducible.

## Validation and Acceptance

Acceptance requires observable correctness, not merely a successful build.

The exact VIX-3998 regression passes when `[B, C, D]` moved before `G` transforms `[A, B, C, D, E, F, G]` into `[A, E, F, B, C, D, G]` without throwing. An after-final-item drop accepts slot 7 for seven initial siblings and moves the selected block to the end. Upward, downward, before, after, noncontiguous, Control-reversed, and within-selection cases produce the exact sequences listed in Milestone 3.

Every valid same-parent drop calls the batch service once, preserves the supplied order, retains each original `ElementModel` and `ElementModelViewModel` instance, reselects the existing moved view models, keeps the total count unchanged, and leaves every original element exactly once in the tree. A no-op within-selection drop remains a successful no-op, not a reorder cycle that recreates view models.

Every invalid direct batch request returns `false`, logs a structured warning, and leaves order, membership, and count unchanged. No code catches `ObservableCollection.Move` failures, clamps invalid slots, or mutates before completing validation.

Every cross-parent sibling or center move inserts into the target before removing from the source. The exact model and light objects survive; child `Parents` IDs reflect the new parent only; `Light.ParentModelId` values remain tied to the leaf; descendants and light collections are not cleared; target order follows the insertion cursor; moved rows are selected through their new target-parent view models; and all original elements exist exactly once.

Center takes precedence whenever `InsertPosition` contains `TargetItemCenter`, even if another position flag is also present. Center-on-group still appends. Before and after sibling branches use `UnfilteredInsertIndex` and never use a filtered index or unconditional arithmetic offset.

The full MSBuild test target succeeds, the `FullyQualifiedName~ElementTreeDragDropTests` run reports 0 failed, and `git diff --check` is clean. Manual upward/downward/before/after/within-selection/cross-parent exercises do not crash or lose data, and save/reopen preserves the final hierarchy, order, descendants, and lights. There are no XAML, persistence schema, dependency version, project, solution, or public API changes.

## Idempotence and Recovery

The source edits and tests are safe to reapply and rerun. `TryMoveWithinParent` is deterministic for a given current order, input sequence, and insertion slot; a drop within the selected block may perform no `Move` calls and still return `true`. Invalid requests are deliberately non-mutating.

If a test fails during development, do not catch the exception in production code, clamp the index, or remove items first. Inspect the mocked `UnfilteredInsertIndex`, current child sequence, dragged snapshot order, and each recomputed `oldIndex`/`cursor` pair. Because validation occurs before mutation, a `false` service result should leave a stable state that can be retried after correcting metadata or code.

If cross-parent assertions show cleared lights or descendants, confirm `InsertToParent` executes before `RemoveFromParent` for every individual model and that the source parent was captured before removal disposed its view model. If view-model identity changes on a same-parent move, confirm the implementation called `ObservableCollection.Move` through `MoveWithinParent` rather than remove/insert. Reset `ElementModelLookUpService.Instance` between tests if stale singleton entries cause ambiguous lookup results.

Do not use `git reset --hard`, `git checkout --`, or broad file deletion to recover. Revert only the scoped hunk with `apply_patch` or correct it forward, preserving unrelated working-tree changes. Build artifacts under `Release\Output` are generated and need not be edited or committed.

## Artifacts and Notes

The core slot-to-final-index trace for the reported case should remain understandable during implementation:

    Initial children: [A, B, C, D, E, F, G]
    Dragged input:    [B, C, D]
    Insertion slot:   6 (before G)

    B: oldIndex 1 < cursor 6, so destination 5
       [A, C, D, E, F, B, G]
    C: oldIndex 1 < cursor 6, so destination 5
       [A, D, E, F, B, C, G]
    D: oldIndex 1 < cursor 6, so destination 5
       [A, E, F, B, C, D, G]

For an after-final-item drop, the initial cursor can equal `Count`. The first dragged model necessarily has an old index below that slot, so the decrement converts the slot to the last valid destination before `Move` is called. Repeating the calculation after every move keeps all later destinations valid and preserves order.

A suitable structured warning shape is:

    Logging.Warn(
        "Rejected same-parent batch move: {Reason}; ParentId={ParentId}; InsertionIndex={InsertionIndex}; ChildCount={ChildCount}; ModelCount={ModelCount}",
        reason,
        parent?.Id,
        insertionIndex,
        parent?.Children.Count,
        models?.Count);

The exact wording may follow nearby conventions, but named fields and non-interpolated arguments are required. Do not log element names as the only identifiers because names are editable and need not be unique.

## Interfaces and Dependencies

At completion, `src/Vixen.Modules/App/CustomPropEditor/Services/PropModelServices.cs` must contain:

    internal bool TryMoveWithinParent(
        ElementModel parent,
        IReadOnlyList<ElementModel> models,
        int insertionIndex)

It returns `true` after a valid move or valid no-op and `false` after any invalid request without mutation. It uses the existing singular `MoveWithinParent` for each necessary move and the existing NLog logger for a structured rejection warning.

`ElementTreeViewModel.Drop(IDropInfo)` remains the existing public `IDropTarget` implementation; its signature and inherited XML documentation remain unchanged. Its sibling branch passes a snapshot of `ElementModel` objects and `dropInfo.UnfilteredInsertIndex` to the internal batch service exactly once for a same-parent drop. Its cross-parent branch uses the same slot as an insertion cursor, incrementing it after every target insert. Its center branch is checked first and retains append semantics.

Use only existing dependencies: `ObservableCollection<T>.Move`, GongSolutions.WPF.DragDrop 4.0.0, Catel view models and relational parent links, NLog, Moq, and xUnit v3. `CustomPropEditor.csproj` already grants `Vixen.Tests` access to internal members, and SDK-style compilation automatically includes the new `.cs` test file. Do not add packages, interfaces, service registrations, XAML, persistence fields, or public APIs.

Revision note (2026-09-12): Created the initial VIX-3998 ExecPlan from the supplied failure analysis and verified it against the current service, view-model, collection-notification, model-lifecycle, dependency, and test code. The plan is intentionally unimplemented.

Revision note (2026-09-12): Completed the first implementation slice at the user's direction: added and directly tested the internal batch-move service contract. Recorded build and focused-test results above. `ElementTreeViewModel.Drop` was deliberately not changed and remains the next implementation milestone.

Revision note (2026-09-12): Completed Milestone 3. `Drop` now uses `UnfilteredInsertIndex` and the validated batch service for same-parent movement, maintains insert-then-remove cross-parent movement, and handles center flags before sibling flags. Added public-boundary regression coverage; the full test build and 15 focused test cases pass. Manual UI/save-reopen validation and the final Jira comment remain pending in Milestone 4.

# Automatically apply a curve or gradient to the sole timeline target (VIX-4010)


This ExecPlan is a living document. Maintain its Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective sections in accordance with `.agents/PLANS.md`. The design was prepared using `.agents/skills/analyze-and-plan-issue/SKILL.md` and `.agents/skills/dotnet-best-practices/SKILL.md`.

## Purpose / Big Picture


A person dropping a library gradient or curve onto an effect in the Timed Sequence Editor should not have to select an item when there is only one eligible destination. After implementation, dropping a gradient onto a one-entry gradient list, a curve onto a one-entry gradient/level-pair list, or a gradient onto a one-entry gradient/level-pair list will immediately update the destination. A selector will still appear when the effect offers more than one eligible destination. Undo and Redo must reverse and restore the drop.

This document delivers the design for Jira improvement VIX-4010, “Dropping a Gradient or a Curve on a list that only has one entry still prompts the selector,” at https://vixenlights.atlassian.net/browse/VIX-4010. Product implementation, Jira updates, commits, and runtime validation have not been performed during plan preparation.

## Progress


- [x] (2026-10-07 18:11Z) Read the issue, the project planning rules, relevant drag documentation, and the current drop and picker implementation. The issue is Accepted and has no attachments, comments, or linked issues.
- [x] (2026-10-07 18:11Z) Established the cause and reviewed the two outer handlers' blast radius. The working tree was clean before creating this plan.
- [x] (2026-10-07 18:11Z) Designed a selection-only change that reuses the existing property/index application loops.
- [x] (2026-10-07 18:20Z) Milestone 1: Updated VIX-4010's description with scope, acceptance criteria, manual regression scenarios, and the planned build/test commands. Re-read the issue and confirmed the description was saved; status remains Accepted and no validation results are claimed.
- [ ] Milestone 2: Bypass the selector for a sole candidate in the two outer drop handlers.
- [ ] Milestone 3: Complete build, IDE analysis, and manual regression acceptance.
- [ ] Milestone 4: Align the final issue description and publish validation evidence.

## Surprises & Discoveries


The one-entry shortcut already exists in `HandleCurveDropOnGradientLevelPairList` and `HandleGradientDropOnGradientLevelPairList`. It is bypassed by the outer handlers: their initial branch requires both one discovered property and a scalar property type. A list property therefore enters the combined selection branch even when it contains only one item.

The relevant current conditions in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` are:

    HandleCurveDrop:
        properties.Count == 1 && properties[0].PropertyType == typeof(Curve)
    HandleGradientDropOnElements:
        properties.Count == 1 && properties[0].PropertyType == typeof(ColorGradient)

The combined selection branches already flatten eligible scalar properties and list entries into `EffectParameterPickerControl` objects. Each object carries the destination `PropertyInfo` and zero-based `Index`. Their unconditional `ShowDialog(this)` calls explain the reported single-item selector.

`HandleGradientDropOnColorGradientList` has no one-entry shortcut. Nevertheless, fixing that helper alone would not fix the timeline path because its outer scalar-type gate also prevents normal list properties from reaching it.

Gortex reported seven direct dependent entries and 22 transitively affected entries for the two outer handlers, with no covering test files. This was a lower-bound graph result, not a proof of complete coverage. Dependencies include gradient handling invoked by color drops. Rider's `findTests` lookup for `TimedSequenceEditorForm.HandleGradientDropOnElements` returned “No symbols found”; it did not establish that runtime behavior is covered.

The completed plan `docs/plans/vix-3965-inline-library-drag-regression.md` documents the library payload contract: linked drags carry a library reference and unlinked drags carry an independent value. Current source confirms the timeline consumes that payload using `DragDropUtils.TryGetDragDropData`. This work must preserve that contract. That older plan's standalone test commands are superseded for this task by the current AGENTS.md requirement to build native dependencies with full MSBuild before running tests.

## Decision Log


- Decision: Count actual eligible destinations in each outer combined-selection branch and bypass the dialog when the existing candidate list contains one item.
  Rationale: One property can contain many entries, and many properties can expose several choices. The existing flattened candidates already describe the choices that the selector would present. Selecting the sole candidate fixes all three issue cases without treating property count as list-entry count.
  Date/Author: 2026-10-07 / Codex.

- Decision: Reuse the existing successful-selection application body with local selected-property and selected-index variables.
  Rationale: The body already transfers metadata to each selected effect, checks list bounds, copies lists, preserves the opposite member of a pair, records old values, and completes undo. Simply relaxing the scalar-type gates would route lists through different helper paths and can change selection behavior across effects whose lists have different lengths.
  Date/Author: 2026-10-07 / Codex.

- Decision: Treat no eligible candidates as a no-op and retain the picker for two or more candidates. A one-entry list accompanied by another eligible property still requires a choice.
  Rationale: Automatic application is justified only when the destination is unambiguous. No candidates must not produce an empty selector or index access. Count candidates on the same representative effect used by the current handler, then apply its property/index choice to the existing target effect set.
  Date/Author: 2026-10-07 / Codex.

- Decision: Make a local change in two private methods, with no new public API, shared dialog policy, dependency, service, or test framework.
  Rationale: The problem is a small Windows Forms selection branch. A focused manual regression matrix verifies the observable behavior; a new abstraction or tests duplicating a count check would expand the change without proving the UI path. Preserve repository naming and architecture under the .NET skill's scoped-maintenance rules.
  Date/Author: 2026-10-07 / Codex.

- Decision: Keep the skill's explicit manual review boundaries and do not create commits automatically.
  Rationale: The invoked skill requires a stop at every execution milestone, and AGENTS.md requires explicit authorization for commits. The user requested design only. Jira-writing milestones describe future work and are not authorization to edit Jira during analysis.
  Date/Author: 2026-10-07 / Codex.

## Outcomes & Retrospective


Milestone 1 is complete: VIX-4010 now records the agreed user behavior, acceptance criteria, regression scenarios, and validation commands. The issue remains Accepted. No product code was changed, and no build, test, or runtime result is claimed. The remaining work is milestones 2–4. The important design finding is that the existing leaf shortcuts are insufficient: the combined picker branches in the outer handlers must participate in automatic selection. Recorded commands and expected results below remain instructions for future execution, not completed test evidence.

## Context and Orientation


Vixen is a Windows light-show sequencer using .NET 10 with Windows Forms and WPF. The timeline drop code involved here is legacy Windows Forms, not a Catel view model. Open the solution in Rider; Rider MCP is available for file-problem analysis and test discovery. Use Rider's “Apply snippet from chat” for small, method-scoped updates during human execution; coding agents must perform source mutations with Gortex edit/refactor as required by AGENTS.md.

The product file is `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`. `TimelineControlGrid_DragDrop` identifies the dropped library value and calls `HandleCurveDrop` or `HandleGradientDrop`. The gradient wrapper obtains the affected elements and calls `HandleGradientDropOnElements`. Here an element means a timeline representation of an effect, not a hardware output. `GetElementsForDrop` chooses selected effects of the same concrete effect type when the drop target is selected; otherwise it chooses the drop target alone. `ValidateMultipleEffects` may separately ask about a mixed effect-type selection. That confirmation serves a different purpose and remains intact.

A `ColorGradient` defines color changes over time. A `Curve` defines a level over time. A `GradientLevelPair` contains both. A curve drop must replace only the curve, and a gradient drop must replace only the gradient. `PropertyMetaData` describes a discovered editable property and the object owning it. Its `ToNewOwner` operation maps a chosen property to another selected effect, including nested ownership.

`CreateGradientListPickerControls` makes one control per gradient entry. `CreateGradientLevelPairPickerControls` makes one per pair, showing either its gradient or curve. These controls live in `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectParameterPickerControl.cs`. `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormParameterPicker.cs` builds the selection dialog, starts an eight-second cancellation timer in its constructor, and exposes the clicked control's metadata. Avoid constructing this form on the automatic path, so that path starts neither a window nor the timer.

`UpdateEffectProperty` calls the property descriptor's `SetValue`, notifies the timeline through `UpdateNotifyContentChanged`, and marks the sequence modified. `CompleteDrop` creates an `EffectsPropertyModifiedUndoAction` from the captured old values when at least one effect changed. Retaining both operations preserves the current content-change and undo path.

## Architecture Design: VIX-4010


Detected IDE environment: Rider with MCP file analysis and test discovery. No Catel or public/protected API change is required. The core strategy is a local branch change guided by the project .NET best-practices skill: decide whether selection is necessary, then use the established mutation path.

Data model and property contracts: No persisted fields, property types, effect APIs, or drag payload types change. In each combined-selection branch introduce local variables holding the selected `PropertyMetaData` and integer index. The choice returned by the picker and the automatically selected candidate must populate the same variables.

Mathematical and boundary logic: Let N be `parameterPickerControls.Count` after all eligible scalar properties and list entries have been collected for the existing representative effect. For N = 0, perform no mutation and do not open a dialog. For N = 1, take `parameterPickerControls[0].PropertyInfo` and `.Index`. For N > 1, retain the current dialog, its cancellation behavior, and the selected control's metadata/index. A list entry's index is zero based; a singleton list therefore supplies index 0. Preserve each destination list's existing check that its count is at least `selectedIndex + 1`; an effect without that index is skipped. There is no index wrap-around, insertion, or resizing.

Subsystem component matrix, expressed as file responsibilities: `TimedSequenceEditorForm.cs` is the only planned product edit and changes the selection stage in `HandleCurveDrop` and `HandleGradientDropOnElements`. `EffectParameterPickerControl.cs` supplies existing candidate metadata and remains a reference. `Forms/FormParameterPicker.cs` remains the dialog for multiple choices. `docs/plans/vix-4010-single-entry-drop-selection.md` records the living plan and validation evidence. No library drag-source or effect model edit is needed.

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Milestone 1: Record the agreed behavior in Jira


Context: VIX-4010 currently describes the three single-entry cases but does not spell out multiple-property, undo, or selected-effect acceptance. This milestone makes the implementation contract reviewable before source changes. Execute this Jira-writing milestone only as part of an authorized implementation workflow.

Plan of Work: Read the current issue through the Atlassian connector. Preserve its original description and add requirements and acceptance derived from this plan: all three single-entry drops are immediate when there is one eligible candidate; multiple candidates retain the selector; no candidates leave the effect unchanged; a pair preserves its other member; canceling selection changes nothing; selected effects retain the existing property/index propagation; Undo/Redo and linked/unlinked values remain correct. Include the manual scenarios and build commands below. Do not change status, assignee, or unrelated fields.

Concrete Steps: Working directory is `C:\Dev\Vixen`. Run `git status --short` and compare any changes against the implementation scope. Use the project Jira skill if executing Jira edits. Read its complete `.agents/skills/jira/SKILL.md` before using its workflow. Re-read the issue after the update and compare its requirements and acceptance with this document.

Validation and Acceptance: The issue includes all three cases and the boundaries above, with no claimed test result. Update this plan's Progress to record that fact.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run `git status --short` and scoped `git diff`, invoke `.agents/skills/commit-msg/SKILL.md` if repository files changed, and output its paste-ready Commit message with VIX-4010 as the subject prefix. Do not create a commit without an explicit request. Pause for explicit confirmation before the next milestone.

### Milestone 2: Select the sole candidate in the actual outer drop paths


Context: Edit only `HandleCurveDrop` and `HandleGradientDropOnElements` in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`. Their combined-selection branches already build all picker candidates and perform successful application. Preserve the scalar fast paths and the list helper implementations.

Plan of Work: After each method has built `parameterPickerControls`, replace unconditional dialog selection with local selected-property/index resolution. For zero controls, end that branch without creating a dialog or changing an effect; preserve ordinary status cleanup and do not bypass any necessary completion for already recorded changes. For one control, copy its `PropertyInfo` and `Index` into the locals immediately. For more than one, create the existing parameter picker, call `ShowMultiDropMessage` and `ShowDialog(this)`, and populate those same locals only on OK. On cancellation, retain `UpdateToolStrip4(String.Empty)` and make no update.

Keep the existing successful application loop in each method and change its metadata/index accesses from `parameterPicker.PropertyInfo` and `parameterPicker.SelectedControl.Index` to the resolved locals. Do not duplicate the effect mutation loop. Do not add a second dialog when applying a selection to another effect. Ensure every automatically selected candidate reaches the same loop as a manually selected candidate.

In the curve loop retain direct Curve assignment and pair reconstruction as `new GradientLevelPair(existingPair.ColorGradient, curve)`. In the gradient loop retain direct gradient assignment, copied gradient-list replacement, and pair reconstruction as `new GradientLevelPair(gradient, existingPair.Curve)`. Continue copying lists with `ToList()` before replacement and capturing the old property value before `UpdateEffectProperty`. Keep `ToNewOwner` success checks, index-bound checks, and the final `CompleteDrop` calls.

The automatic path creates an existing candidate control but no parent dialog. Dispose that unused control after its metadata/index have been copied, using a local using scope or equivalent guaranteed cleanup; do not add a shared UI lifetime refactor or alter the control's public image/property APIs. Keep picker selection and application synchronous on the UI thread. Do not introduce async work or a background thread.

Concrete Steps: Inspect `git status --short` before editing. Use Gortex source reads to verify both methods, then `change(operation:"impact")` with their symbol IDs. No signature change is proposed, so signature verification is unnecessary. Apply two small method-local updates through Gortex. In Rider, review the blocks using “Apply snippet from chat” for manual work, not a whole-file replacement. Use tabs and LF and preserve untouched formatting.

After edits, call Gortex `change(operation:"detect")` and use its changed symbol IDs with `tests`, `guards`, and `contract`. Run Rider `get_file_problems` for the edited file. Resolve only new diagnostics in changed lines. No public/protected API changes are planned; if implementation requires one, stop and revise the design and apply the project csharp-docs skill before changing it.

Validation and Acceptance: Review shows exactly two outer handlers changed. For one candidate neither `CreateParameterPicker` nor `ShowDialog` nor `ShowMultiDropMessage` is called. Both automatic and manual choices use the same property/index application loop. Preserve the property's original list object for Undo. Record actual diagnostics and graph results in this plan.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run `git status --short` and `git diff -- src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs docs/plans/vix-4010-single-entry-drop-selection.md`, invoke `.agents/skills/commit-msg/SKILL.md`, and output its paste-ready Commit message with VIX-4010 as the subject prefix. Do not create a commit without an explicit request. Pause for explicit confirmation before the next milestone.

### Milestone 3: Prove the timeline behavior and regression boundaries


Context: Validate the actual timeline drop path in a Windows development build. A passing build alone cannot prove that a selector was suppressed. Use a scratch profile and sequence with distinct gradients and curves so each change is visible in the Effect Editor.

Concrete Steps: Run from `C:\Dev\Vixen` in a shell with full Visual Studio MSBuild and the native C++ toolset available:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Expected output includes `Build succeeded` and zero errors. Launch the application from Rider's application run configuration using that Debug build. Build output is in `Debug/Output/`. Record the actual command, outcome, and any unrelated pre-existing warning without repairing unrelated code.

Validation and Acceptance: Prepare an effect with one eligible `List<ColorGradient>` entry and drop a visibly different gradient onto it. Observe immediate replacement and no item selector. Prepare an effect with one eligible `List<GradientLevelPair>` entry. Drop a visibly different curve; observe immediate replacement and unchanged gradient. Drop a visibly different gradient; observe immediate replacement and unchanged curve. Confirm the displayed property and timeline representation update and the sequence becomes modified. For each case, Undo restores the old value and Redo restores the dropped value. Save and reopen the scratch sequence to confirm the new value persists.

Repeat each applicable case with at least two list entries. The selector must appear and choosing the second entry must change only that entry. Escape or cancellation timeout must make no change or undo entry. Use an effect with two eligible properties, each containing one choice, and confirm selection remains necessary. Where the editor permits an empty list, confirm no selectable destination produces no dialog or mutation; retain existing null-property behavior. If a configuration cannot be created through the UI, record that case as not exercised rather than claiming it passed.

For selected same-type effects, drop on a selected effect with one candidate. Confirm all mapped targets receive its property/index choice without per-effect item prompts. Include another selected effect with a longer list: only the same chosen index changes there. Include an empty destination list: it is skipped safely. Include mixed effect types and confirm the existing separate mixed-selection confirmation still works. Repeat with two candidates on the representative effect and confirm the one existing picker chooses the index for all mapped effects.

Use both an unlinked library value and a linked one. Verify the intended independent or linked state is retained, and the untouched half of each pair keeps its original state. Repeat a scalar Curve and scalar ColorGradient drop. Since color drop handling can delegate to gradient handling, also smoke-test dropping a color onto a gradient-backed property for correct assignment and Undo.

Automated testing: No new UI testing framework or test-only public API is required for these two local branches. If existing relevant tests become available, use Rider's Test Runner to run them. Any command-line test run must follow the current repository workflow, from `C:\Dev\Vixen`:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/

Expect zero failed tests and record the actual count; do not invent a count or coverage claim. `dotnet test` alone cannot build the transitive C++/CLI projects. If no relevant automated test is run, state that explicitly and use the required manual outcomes as regression evidence.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run `git status --short` and scoped `git diff`, invoke `.agents/skills/commit-msg/SKILL.md` for changed repository files, and output its paste-ready Commit message with VIX-4010 as the subject prefix. Do not create a commit without an explicit request. Pause for explicit confirmation before the next milestone.

### Milestone 4: Record final acceptance and validation in Jira


Context: Finish only after the three issue scenarios and required regression checks have been exercised. Execute this external-writing milestone only within the authorized implementation workflow.

Plan of Work: Re-read VIX-4010 and reconcile any approved requirements or test-plan changes with this document. Add a comment listing the commands actually run, their results, manually passed scenarios, and any skipped or blocked cases with reasons. Keep the issue description aligned with the implemented behavior. Do not claim resolution from compilation alone. Do not transition the issue, push, create a pull request, or commit without a separate request.

Validation and Acceptance: The issue and this plan agree. Progress and Outcomes & Retrospective distinguish actual completed work from remaining limitations. Validation evidence is sufficiently specific for another person to repeat it.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run `git status --short` and scoped `git diff`, invoke `.agents/skills/commit-msg/SKILL.md` for changed repository files, and output its paste-ready Commit message with VIX-4010 as the subject prefix. Do not create a commit without an explicit request. Pause for explicit confirmation before any further work.

## Idempotence and Recovery


The change is two local selection edits with no data migration. Re-running validation against a scratch sequence is safe. Before repeating a Jira edit or comment, read current content to avoid duplicating it. If interrupted, use the current diff and this document to complete only the intended blocks; preserve all user changes and do not reset the worktree. If acceptance fails, retain the failure evidence and repair only the changed selection logic. A failure in the existing source payload or unrelated effect behavior requires a recorded scope decision before expanding the work.

## Artifacts and Notes


Source evidence at design time is in `TimedSequenceEditorForm.cs`: `HandleCurveDrop` begins near line 4510, `HandleGradientDropOnElements` near 4687, the pair helpers near 4631 and 4831, and the gradient-list helper near 4810. Prefer these names over fixed line positions as the file changes.

The desired selection stage is:

    Build the existing eligible candidate list.
    No candidates: no selection, no mutation.
    One candidate: selectedProperty = candidate.PropertyInfo; selectedIndex = candidate.Index.
    Multiple candidates: show existing picker; proceed only on OK.
    Apply selectedProperty/selectedIndex with the original update loop.
    CompleteDrop records changed effects once.

Plan-preparation checks: `git status --short` returned no changes before this document was created. Source was inspected through Gortex; its impact analysis found the described dependents and no covering test file. Rider test discovery returned a missing-symbol result. No build, automated test, or runtime manual scenario was run during design.

## Interfaces and Dependencies


Retain these private signatures in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`:

    private void HandleCurveDrop(Element element, Curve curve)
    private void HandleGradientDropOnElements(IEnumerable<Element> elements, ColorGradient gradient)

The local selection uses existing `PropertyMetaData` and `int`; it needs no new type or interface. Reuse `EffectParameterPickerControl.PropertyInfo`, `.Index`, `FormParameterPicker.SelectedControl`, `PropertyMetaData.ToNewOwner`, `UpdateEffectProperty`, and `CompleteDrop`. Retain the existing Curve, ColorGradient, and GradientLevelPair dependencies and drag-payload serialization. No NuGet package, project-reference change, database field, or new public/protected API is required.

## Plan Revision Note


2026-10-07: Created the plan from Jira and current source after the user authorized additional Gortex inspection and identified TimedSequenceEditorForm. The design targets the actual outer combined-picker paths and preserves their existing multi-effect property/index application behavior.

2026-10-07: Updated VIX-4010's description for milestone 1. It now includes singleton immediate application, multi/no-candidate and cancellation behavior, pair preservation, selected-effect propagation, linked/independent values, Undo/Redo, manual scenarios, and the repository validation commands. The issue was re-read after the update; its status was unchanged, and no validation result is represented as completed.

Analysis complete and plan integrated with plans.md.

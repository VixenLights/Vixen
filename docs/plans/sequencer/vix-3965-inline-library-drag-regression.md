# Fix linked library drags into inline Effect Editor properties

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document according to `.agents/PLANS.md`. This work is tracked by Jira issue VIX-3965, “Fix linked curve and gradient drags to inline Effect Editor properties,” which is related to the closed VIX-2226 toolbar feature.

## Purpose / Big Picture

After this change, a person can drag a curve or color gradient from a Timed Sequence Editor library toolbar to an inline Effect Editor field and decide whether the destination should stay connected to the library. Holding Ctrl makes a linked destination: the inline editor displays its chain icon and linked tooltip and prevents direct editing. Dragging without Ctrl makes an independent editable copy. The same distinction will work from the full Curve Library and Gradient Library windows, while moving items within any library toolbar, library list, or the Layer Editor will continue to reorder rather than copy.

The regression occurs because a library-toolbar drag begins as a Move containing the original library object, then its DragLeave handler starts a second, nested Copy containing the linked clone. The WinForms timeline receives the second payload, but the WPF Effect Editor accepts the first Move payload and therefore assigns the original unlinked object. The correction is deliberately at the source and shared target-contract layers; it must not add a special case to inline editors.

## Progress

- [x] (2026-08-06 00:00Z) Researched the toolbar, full-library, WPF drop-manager, model-copy, and test-project code paths; confirmed the nested drag regression.
- [x] (2026-08-06 00:00Z) Created Jira Bug VIX-3965 with scope, design notes, automated and manual acceptance criteria, and a relationship to VIX-2226.
- [x] (2026-08-06 00:00Z) Compared VIX-3965 to this ExecPlan at implementation start; no scope, acceptance, or validation change required.
- [x] (2026-08-06 00:00Z) Added the pure payload factory, test-only access, runtime test dependency, and focused tests.
- [x] (2026-08-06 00:00Z) Replaced the nested WinForms toolbar and full-library drag sources with one multi-effect payload and operation.
- [x] (2026-08-06 00:00Z) Added target-side WPF effect negotiation, documented `AcceptedEffects`, the non-None completion guard, and focused resolver tests.
- [x] (2026-08-06 00:00Z) Completed automated and manual validation and recorded the results in VIX-3965.

## Surprises & Discoveries

- Observation: The fault is not in `EffectPropertyEditorGrid.cs` or the property setters. `PropertyItemValue.OnDropCompleted` simply deserializes a value of the property's declared type and assigns it.
  Evidence: `src/Vixen.Modules/Editor/EffectEditor/PropertyItemValue.cs` uses `DragDropUtils.TryGetDragDropData` followed by `Value = data`.

- Observation: all three library source forms use the same two-stage pattern. The toolbar starts Move in `toolStripLibraryButton_MouseMove` and starts Copy from `toolStripLibrary_DragLeave`; Curve and Gradient library lists start Move from ItemDrag and Copy from DragLeave.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs`, `Forms/Form_CurveLibrary.cs`, and `Forms/Form_GradientLibrary.cs` each contain two `DoDragDrop` calls.

- Observation: `DragDropManager.UpdateEffects` only considers the source's `DragEventArgs.AllowedEffects`; it never asks the WPF target which operation it accepts.
  Evidence: `src/Vixen.Common/WPFCommon/Input/IDropTargetAdvisor.cs` has no effect property, and `DragDropManager.cs` chooses Copy or Move from `e.AllowedEffects` alone.

- Observation: unit tests can access `WPFCommon` internals already, but the test project has no reference to `TimedSequenceEditor`, and that project does not yet declare `InternalsVisibleTo` for `Vixen.Tests`.
  Evidence: `src/Vixen.Tests/Vixen.Tests.csproj` references `WPFCommon` but not `TimedSequenceEditor`; `WPFCommon.csproj` exposes internals to `Vixen.Tests`, whereas `TimedSequenceEditor.csproj` does not.

- Observation: Milestone 1's focused test run passes, although restore/build continues to report existing package and analyzer warnings.
  Evidence: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~DragDropManagerTests` passed 11 tests; output included NU1904 warnings for LiteDB 4.1.4 and pre-existing compiler warnings in unrelated projects.

- Observation: The full solution MSBuild command uses the repository's shared solution output layout and successfully builds Timed Sequence Editor, including the new factory.
  Evidence: `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` completed successfully in 52 seconds. The standalone `Vixen.Tests` build remains unsuitable for focused execution because it is not built by that solution configuration and its Rebuild target cleans and evaluates transitive native projects.

- Observation: A full Debug solution rebuild succeeds after the source-drag refactor.
  Evidence: `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` completed successfully after replacing the toolbar and library-list nested drag operations.

- Observation: Manual validation confirms the corrected linked and unlinked library drag behavior.
  Evidence: User manual testing on 2026-08-06 reported correct behavior after launching the Debug solution output.

- Observation: The nine factory-test failures were caused by a missing runtime copy of the Timed Sequence Editor module, not a factory behavior failure.
  Evidence: All nine `LibraryDragPayloadFactoryTests` initially threw `FileNotFoundException` for `Module.Editor.TimedSequenceEditor.dll`. Restoring the default runtime-copy behavior for the test project reference and adjusting the linked-gradient data assertion produced a focused 9/9 pass.

- Observation: The full test suite passes after the factory-test runtime setup was corrected.
  Evidence: User ran the complete suite on 2026-08-06 and reported 669 passing tests with zero failures.

## Decision Log

- Decision: Fix the source payload and shared effect negotiation rather than modify `EffectPropertyEditorGrid`, inline assignment, model serialization, or property setters.
  Rationale: the receiving inline editor already accepts and assigns the data it is given. The data race is created by nested source drags, and WPF currently negotiates an effect without a target-side contract.
  Date/Author: 2026-08-06 / Codex, from the Sol handoff and repository inspection.

- Decision: Add `AcceptedEffects` to `IDropTargetAdvisor` instead of reusing `IDragSourceAdvisor.SupportedEffects`.
  Rationale: a component may expose different behavior as a drag source and as a drop target. The interface name makes target acceptance explicit and permits inline targets to be Copy-only while Layer Editor remains Move-only.
  Date/Author: 2026-08-06 / Codex.

- Decision: Keep `LibraryDragPayloadFactory` internal and UI-independent in the Timed Sequence Editor assembly, and grant `Vixen.Tests` internal access rather than make the factory public.
  Rationale: drag-payload construction is an implementation detail, but it must be directly unit tested without Windows Forms drag events. This repository already uses an MSBuild `InternalsVisibleTo` assembly attribute for this exact test pattern.
  Date/Author: 2026-08-06 / Codex.

- Decision: Preserve a single `Move | Copy` OLE drag operation for each toolbar or full-library drag. Ctrl changes whether the domain payload contains a library reference; it does not create a second external Copy operation.
  Rationale: the target selects the effect. Internal reorder destinations select Move, and timeline or inline value destinations select Copy, eliminating competing event streams and redundant payload serialization.
  Date/Author: 2026-08-06 / Codex.

## Outcomes & Retrospective

Milestone 1 is complete. WPF drop targets now declare their accepted effect independently of source support: Effect Editor property and collection targets accept Copy, while Layer Editor accepts Move. `DragDropManager` resolves the intersection of source and target effects, gives Ctrl a Copy preference only when Copy is effective, and does not invoke target completion for a None result. The focused resolver suite passed 11 tests. The payload-source and end-to-end drag milestones remain outstanding; at full completion, replace this entry with all validation and manual evidence.

Milestone 3 is complete. Toolbar Curve, ColorGradient, and Color sources now construct one factory payload and start one `Move | Copy` drag operation, with deterministic cleanup of reorder and drag-box state. Curve and Gradient library lists use the same payload while carrying their raw `ListViewItem` as an additional internal-reorder format. The obsolete nested `DragLeave` handlers and subscriptions, along with `_dragValid`, have been removed. The full Debug solution build passed; manual drag coverage remains for Milestone 4.

Manual validation confirmed the corrected behavior. The focused factory suite passes 9/9, the full suite passes 669/669, and the Debug solution build succeeds. No known limitation remains for VIX-3965.

## Context and Orientation

Vixen is a Windows desktop light-show editor built with .NET, Windows Forms, and WPF. The Timed Sequence Editor is predominantly Windows Forms. The Effect Editor is WPF and uses `Common.WPFCommon.Input.DragDropManager` with two advisor interfaces: a drag-source advisor supplies data and allowed effects, while a drop-target advisor validates the data and completes the drop.

The actual library-domain types are `VixenModules.App.Curves.Curve` in `src/Vixen.Modules/App/Curves/Curve.cs` and `VixenModules.App.ColorGradients.ColorGradient` in `src/Vixen.Modules/App/ColorGradients/ColorGradient.cs`. Their copy constructors make independent data copies. `LibraryReferenceName` is empty for an independent value and contains the library key for a linked value. `IsCurrentLibraryCurve` and `IsCurrentLibraryGradient` identify the object stored in the library itself; a dragged copy must always clear the corresponding flag. `System.Drawing.Color` is a value type and therefore needs no clone.

`Utilities.DragDropUtils` in `src/Vixen.Common/Utilities/DragDropUtils.cs` creates a Windows Forms `DataObject`, storing `Color` directly and serializing other values as JSON under their runtime type. This work must use it unchanged. The WPF Effect Editor has a separate but analogous `Common.WPFCommon.Input.DragDropUtils`; do not change either serializer.

The current toolbar source is `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs`. It stores its pending drag state in `_itemMove`, `_dragValid`, and `_dragBoxFromToolStripMouseDown`. The three toolbar controls wire the obsolete DragLeave handler in `TimedSequenceEditorForm.Designer.cs`. The full-library sources are `Forms/Form_CurveLibrary.cs` and `Forms/Form_GradientLibrary.cs`; their designers wire equivalent DragLeave handlers. Their existing drag-enter and drop methods already distinguish a raw `ListViewItem` reorder from a serialized Curve or ColorGradient copy, so a multi-format data object preserves that distinction.

The WPF targets that implement `IDropTargetAdvisor` are `src/Vixen.Modules/Editor/EffectEditor/PropertyItemValue.cs`, `src/Vixen.Modules/Editor/EffectEditor/Internal/CollectionItemValue.cs`, and `src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml.cs`. The first two are inline value targets and must accept Copy. Layer Editor represents layer ordering and must accept Move. `src/Vixen.Common/WpfPropertyGrid/PropertyItemValue.cs` is unrelated and must not be changed.

The existing WPF manager test file is `src/Vixen.Tests/Common/WPFCommon/DragDropManagerTests.cs`. New factory tests belong under `src/Vixen.Tests/Sequencer/`, alongside the other sequence-editor tests. The test project must reference `TimedSequenceEditor.csproj`; that module project must expose its new internal factory to `Vixen.Tests` using the repository's MSBuild `InternalsVisibleTo` convention. Use normal project references, not DLL references, and keep the project-reference asset behavior consistent with the surrounding project.

The current working tree contains unrelated user changes in `src/Vixen.Modules/Editor/EffectEditor/EffectEditor.csproj` and deletions or replacements involving `Themes/Images/librarylink.png`, `clear.png`, and `search.png`. Do not stage, restore, alter, or include those files in this work.

## Plan of Work

### Milestone 1: Align Jira and establish testable contracts

Before editing product code, open VIX-3965 and compare its description with this ExecPlan. Update its description if implementation discovery has changed required files, the acceptance criteria, or the test plan. Keep the issue as a Bug in the VIX project, with the Editor/Sequencer component and its relationship to VIX-2226. This makes the reviewable work item agree with the executable specification before behavior changes begin.

In `src/Vixen.Common/WPFCommon/Input/IDropTargetAdvisor.cs`, add `DragDropEffects AcceptedEffects { get; }`. Add XML documentation following `.agents/skills/csharp-docs/SKILL.md`: the summary must begin “Gets,” explain that the value is the set of operations this target permits, and provide a `<value>` description that identifies it as a bitwise combination of `DragDropEffects` values. Add any required `System.Windows` import for the enum. This is a public API change; do not leave it undocumented.

Implement the property without changing assignment behavior in each advisor:

    src/Vixen.Modules/Editor/EffectEditor/PropertyItemValue.cs
        public DragDropEffects AcceptedEffects => DragDropEffects.Copy;

    src/Vixen.Modules/Editor/EffectEditor/Internal/CollectionItemValue.cs
        public DragDropEffects AcceptedEffects => DragDropEffects.Copy;

    src/Vixen.Modules/Editor/LayerEditor/Views/LayerEditorView.xaml.cs
        public DragDropEffects AcceptedEffects => DragDropEffects.Move;

Retain their existing `SupportedEffects` values, source methods, `IsValidDataObject`, and `OnDropCompleted` logic. In particular, do not add Ctrl behavior to `PropertyItemValue.OnDropCompleted` and do not infer a library name at the inline target.

In `src/Vixen.Common/WPFCommon/Input/DragDropManager.cs`, replace the source-only branch logic in `UpdateEffects` with a call to an internal pure resolver. Name it `ResolveDropEffect` and keep it static and free of `DragEventArgs`, UI elements, and side effects. It must receive: whether `CurrentDropTargetAdvisor.IsValidDataObject` returned true, source allowed effects, target accepted effects, and key state. It returns one `DragDropEffects` value with this precise behavior:

    invalid data -> None
    effectiveEffects = sourceAllowedEffects & targetAcceptedEffects
    neither Copy nor Move in effectiveEffects -> None
    Ctrl is present and Copy is effective -> Copy
    Move is effective -> Move
    Copy is effective -> Copy
    otherwise -> None

Use bitwise checks for Ctrl so Shift, Alt, or other flags do not hide Ctrl. `UpdateEffects` assigns the resolver result to `e.Effects`. In `DropTarget_PreviewDrop`, call `OnDropCompleted` only when the resolver result is not `None` and the data remains valid. Keep the cleanup of preview adorners and `e.Handled` behavior. This must ensure an invalid or unaccepted drop cannot mutate a target even when a `Drop` event arrives.

Extend `src/Vixen.Tests/Common/WPFCommon/DragDropManagerTests.cs` with direct `ResolveDropEffect` cases, so tests do not synthesize WPF drag events. Cover invalid data; Move-only source into Copy-only target; Copy|Move into Copy-only with and without Ctrl; Copy|Move into Move-only; both accepted with Ctrl selecting Copy and without Ctrl selecting Move; and a result of None. State in the test naming and assertion that None is the completion guard, and inspect `DropTarget_PreviewDrop` during review to confirm it does not invoke the advisor for that result.

Run from `C:\Dev\Vixen`:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~DragDropManagerTests

Expect the resolver tests to pass. This milestone is complete when the test suite proves target acceptance is part of effect selection, with no property-assignment changes.

### Milestone 2: Add immutable library-drag payload construction and tests

Create `src/Vixen.Modules/Editor/TimedSequenceEditor/LibraryDragPayloadFactory.cs` in namespace `VixenModules.Editor.TimedSequenceEditor`. Define an `internal static` factory method, for example:

    internal static object Create(object sourceValue, string libraryItemName, bool linkToLibrary)

Validate inputs before returning. Throw `ArgumentNullException` for a null source value. Reject unsupported source types with `ArgumentException` naming `sourceValue`. Because a linked payload cannot be meaningful without a library key, reject a null or empty `libraryItemName` when `linkToLibrary` is true with an argument exception; an unlinked call may accept the supplied key without using it. Do not mutate `sourceValue` in any branch.

For `Curve`, construct `new Curve(source)`, set `LibraryReferenceName` to `libraryItemName` only when `linkToLibrary` is true and to `string.Empty` otherwise, then set `IsCurrentLibraryCurve` to false. For `ColorGradient`, use its copy constructor and apply the corresponding reference and `IsCurrentLibraryGradient = false` rules. For `Color`, return the value unchanged. Do not change either model class, its copy constructor, serialization settings, or `Utilities.DragDropUtils`.

Add a project reference from `src/Vixen.Tests/Vixen.Tests.csproj` to `..\\Vixen.Modules\\Editor\\TimedSequenceEditor\\TimedSequenceEditor.csproj`. In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditor.csproj`, add the standard MSBuild `AssemblyAttribute` for `System.Runtime.CompilerServices.InternalsVisibleToAttribute` with `_Parameter1` equal to `Vixen.Tests`. This exposes only the factory to tests through the existing assembly relationship, while the factory remains non-public to production consumers.

Create `src/Vixen.Tests/Sequencer/LibraryDragPayloadFactoryTests.cs`. Use Curve points and a ColorGradient with multiple color and alpha points so deep-copy evidence is meaningful. Add tests that verify:

- a linked Curve is a different instance, retains point data, has the supplied reference key, and is not current-library;
- an unlinked Curve is a different instance, retains points, has an empty reference key, and is not current-library;
- linked and unlinked ColorGradient payloads have equivalent reference and current-library behavior while retaining their color and alpha data;
- changing the clone's reference or mutable data does not change the source, and the factory never changes a source that began with its own reference/current-library flags;
- a Color is returned with the same value;
- null, unsupported values, and a linked request without a usable name throw the documented argument exception type.

Run from `C:\Dev\Vixen`:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~LibraryDragPayloadFactoryTests

Expect all factory tests to pass and to require neither a running application nor a synthesized drag event. This milestone proves the eventual UI paths have one safe domain payload to serialize.

### Milestone 3: Use one payload and one drag operation at every library source

In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs`, replace the split logic in `toolStripLibraryButton_MouseMove` and remove `toolStripLibrary_DragLeave` completely. When the pointer exits `_dragBoxFromToolStripMouseDown` with the left button pressed, validate the selected button, source tag, and owning strip as the existing code does. Set `_itemMove` true before starting the drag so a drop back on the toolbar identifies an internal reorder. Determine the domain link state once using:

    (ModifierKeys & Keys.Control) == Keys.Control

Call `LibraryDragPayloadFactory.Create(_selectedButton.Tag, _selectedButton.Name, linkToLibrary)`, serialize the returned value once with `Utilities.DragDropUtils.CreateDataObject`, and call `_contextToolStrip.DoDragDrop(dataObject, DragDropEffects.Move | DragDropEffects.Copy)` exactly once. Do not use DragLeave to alter a payload or initiate another drag.

Wrap the synchronous `DoDragDrop` call in `try/finally`. In the `finally`, set `_itemMove` false and `_dragBoxFromToolStripMouseDown` to `Rectangle.Empty`; also clear any per-drag local state needed to prevent a second drag when MouseMove resumes. Remove the `_dragValid` field and every assignment or condition based on it. Update each toolbar `DragEnter` method so it selects Move solely for valid same-toolbar reorders identified by `_itemMove`, and Copy for valid external values; no drag-valid flag is necessary. Keep the existing drop/reorder and persistence code unchanged other than any compile-required removal of `_dragValid`.

In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`, remove the three `DragLeave` subscriptions for Color, Curve, and Gradient toolbars. Do not alter other generated control setup.

In `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_CurveLibrary.cs`, replace `listViewCurves_ItemDrag` and remove `listViewCurves_DragLeave`. On item drag, require a selected item, construct its payload using the factory with the selected item's `Tag`, `Name`, and existing `LinkCurves` checkbox state, then use `Utilities.DragDropUtils.CreateDataObject`. Add the selected raw `ListViewItem` to the same `DataObject` under `typeof(ListViewItem)`, then start exactly one `listViewCurves.DoDragDrop(dataObject, DragDropEffects.Move | DragDropEffects.Copy)`. The current DragEnter prioritizes `ListViewItem` for Move and recognizes Curve payloads for Copy, so preserve that order and all reorder/drop logic.

Apply the same transformation in `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_GradientLibrary.cs`, using `LinkGradients` and the selected gradient item. In the Curve and Gradient designer files, `Form_CurveLibrary.Designer.cs` and `Form_GradientLibrary.Designer.cs`, remove only the obsolete list-view DragLeave subscriptions. Preserve ItemDrag, DragEnter, DragDrop, selection, scaling, and all other event subscriptions.

No UI code should directly clone Curve or ColorGradient for library dragging after this milestone: all toolbar and full-library source construction goes through `LibraryDragPayloadFactory`. The timeline and inline targets distinguish their behavior by negotiated effect: timeline and inline targets receive Copy; library toolbar/list reorders receive Move.

Build this module from `C:\Dev\Vixen` after the source edits:

    msbuild src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditor.csproj -m -t:Restore,Rebuild -p:Configuration=Debug

Expect a successful build and no compiler references to `toolStripLibrary_DragLeave`, `listViewCurves_DragLeave`, `listViewGradients_DragLeave`, or `_dragValid`.

### Milestone 4: Validate the regression end to end and close the work-item loop

Run the complete automated commands from `C:\Dev\Vixen`:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Treat any existing unrelated failure as a stop-and-record condition: identify whether it reproduces without this change and do not repair unrelated work. Review `git status --short` before and after validation. The only intentional source-control changes must be the files described by this plan and the new plan itself; preserve the user's existing EffectEditor project and image changes.

Perform manual acceptance in a development build. Drag a toolbar Curve to a direct inline Curve field and a toolbar ColorGradient to a direct inline gradient field while Ctrl is held. Confirm each displays the linked chain indicator and tooltip and cannot be edited as an independent value. Repeat with a collection-backed inline target such as `GradientLevelPair`. Repeat all four cases without Ctrl and confirm the results are unlinked and editable. Repeat Ctrl toolbar drops to the timeline to confirm existing timeline linking remains intact.

Reorder color, curve, and gradient toolbar entries with and without Ctrl; then reorder entries within the Curve and Gradient library lists. Open each full library window and exercise its Link checkbox for a timeline destination and an inline destination. Finally reorder Layer Editor layers and cancel a toolbar drag, then begin another drag; verify reordering remains Move-only and there is no stale drag box or `_itemMove` behavior.

Update VIX-3965's description if any approved implementation detail, test name, or acceptance wording changed from this ExecPlan. Add a Jira comment recording the exact automated commands, their pass/fail result, the manual scenarios completed, and any known limitations. Do not transition, commit, push, or open a pull request unless separately instructed. If a milestone that edits repository files is reported complete, use the repository `commit-msg` skill to provide a formatted proposed commit message in the completion response, but do not create the commit.

## Concrete Steps

All commands are run from `C:\Dev\Vixen`.

1. Confirm the protected working tree entries before implementation:

       git status --short

   Expected relevant pre-existing entries include `src/Vixen.Modules/Editor/EffectEditor/EffectEditor.csproj` and the EffectEditor image files. Leave them untouched.

2. Implement Milestone 1 and run:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~DragDropManagerTests

   Expected transcript ends with the targeted tests passing and zero failed tests.

3. Implement Milestone 2 and run:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~LibraryDragPayloadFactoryTests

   Expected transcript ends with all factory tests passing.

4. Implement Milestone 3 and run:

       msbuild src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditor.csproj -m -t:Restore,Rebuild -p:Configuration=Debug

   Expected transcript contains `Build succeeded` with zero errors.

5. Complete Milestone 4 with:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj
       msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

   Expected transcript contains zero failed tests and `Build succeeded` for the Debug solution build. Record the actual test count and build result in VIX-3965 rather than guessing it in advance.

## Validation and Acceptance

The change is accepted only when the automated tests establish the following effect-resolution behavior: invalid data resolves to None; a Move-only source into a Copy-only target resolves to None; a Copy|Move source into a Copy-only target resolves to Copy with or without Ctrl; a Copy|Move source into a Move-only target resolves to Move; and a target accepting both resolves to Copy with Ctrl and Move otherwise. The implementation must guard `OnDropCompleted` with the non-None result so a None resolution cannot invoke target mutation.

The payload tests must establish that Curve and ColorGradient outputs are independent clones, retain their full point or gradient content, use the supplied library name only when linked, clear their current-library flag, and leave their source untouched. They must also establish Color's value semantics and rejection of invalid factory inputs.

Manual acceptance requires linked Ctrl drags to direct and collection-backed inline editors, unlinked non-Ctrl drags, toolbar-to-timeline linking, full-library Link-checkbox behavior at timeline and inline destinations, toolbar/list/Layer Editor reordering, and cancellation cleanup to all behave as described in Milestone 4. A regression in any reordering mode blocks completion because the single drag must retain both Copy and Move use cases.

## Idempotence and Recovery

The code and test steps are additive or local replacements and can be rerun safely. If a source edit is interrupted, restore only the incomplete files being changed by this plan from their known repository version or reapply the small edit; do not use a broad reset and do not alter the unrelated EffectEditor worktree changes. If adding the test project reference creates unexpected solution configuration entries, do not use `dotnet sln add` because the Timed Sequence Editor project is already in `Vixen.sln`.

If full validation fails, first rerun the focused tests to separate factory/negotiation failures from an unrelated environment or solution failure. For a failure caused by the protected user changes, record it in VIX-3965 and leave those changes intact. For an issue-description mismatch, update the Jira description and this living plan together before continuing.

## Artifacts and Notes

The desired drag negotiation is:

    effectiveEffects = sourceAllowedEffects & targetAcceptedEffects
    if data is invalid or effectiveEffects has neither Copy nor Move: None
    if Ctrl is pressed and Copy is effective: Copy
    else if Move is effective: Move
    else if Copy is effective: Copy
    else: None

The intended source flow is:

    library value + library item name + requested link state
        -> LibraryDragPayloadFactory creates a safe domain value
        -> Utilities.DragDropUtils serializes one DataObject
        -> one DoDragDrop with Move | Copy
        -> reorder target chooses Move; timeline/inline target chooses Copy

This eliminates the nested OLE drag loop and redundant serialization that presently produce competing drag event streams. No asynchronous work or locking is required: all construction and drag execution remain on the UI thread, and `try/finally` makes the toolbar state cleanup deterministic after normal completion, rejection, cancellation, or an exception.

## Interfaces and Dependencies

At the end of Milestone 1, `src/Vixen.Common/WPFCommon/Input/IDropTargetAdvisor.cs` must include the documented public contract:

    DragDropEffects AcceptedEffects { get; }

At the end of Milestone 1, `DragDropManager` must expose this test-visible internal pure method, with the final parameter type chosen to match the existing WPF key state:

    internal static DragDropEffects ResolveDropEffect(
        bool isValidDataObject,
        DragDropEffects sourceAllowedEffects,
        DragDropEffects targetAcceptedEffects,
        DragDropKeyStates keyStates)

At the end of Milestone 2, `src/Vixen.Modules/Editor/TimedSequenceEditor/LibraryDragPayloadFactory.cs` must contain an internal, UI-independent static factory equivalent to:

    internal static object Create(object sourceValue, string libraryItemName, bool linkToLibrary)

It returns only Curve, ColorGradient, or Color payloads; Curve and ColorGradient are independent copies; it does not serialize data, display UI, query the library, or mutate its input. The event sources remain responsible for calling `Utilities.DragDropUtils.CreateDataObject` exactly once. No new NuGet package, service, background thread, data-model field, serializer option, or inline-editor API is needed.

## Plan Revision Note

2026-08-06: Created this initial ExecPlan from the Sol handoff and direct repository research. The revision records the verified nested-drag paths, formalizes VIX-3965, and chooses an internal factory plus internal test access so the regression can be tested without UI event synthesis.

2026-08-06: Recorded completion of Milestone 1 after the documented target contract, pure resolver, completion guard, and focused tests were implemented. No Jira description update was needed because the approved scope and acceptance criteria remained unchanged.

2026-08-06: Added Milestone 2's factory, test-only access, and unit tests. The full Debug solution build verifies the production assembly in the repository's required shared-output layout. Keep the focused-test validation pending because `Vixen.Tests` is outside that solution build configuration and its standalone Rebuild target is not compatible with the native project graph.

2026-08-06: Corrected the validation note after confirming the full solution MSBuild command succeeds. The previous note incorrectly generalized a direct-project build limitation to the repository's supported solution build.

2026-08-06: Completed Milestone 3. Replaced all Curve, ColorGradient, and Color library source nested drag sequences with a single factory-built `Move | Copy` operation, preserving raw list-item formats for list reordering and clearing toolbar drag state in `finally`. The full Debug solution build succeeded.

2026-08-06: Diagnosed and corrected the nine `LibraryDragPayloadFactoryTests` failures. The test project was not copying the Timed Sequence Editor runtime dependency, and its linked-gradient assertion triggered normal library resolution for a nonexistent isolated-test item. The focused suite now passes 9/9, and the user confirmed the full suite passes 669/669 along with successful manual validation.

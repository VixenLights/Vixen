# Refresh mark-collection selectors after collection changes

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain it in accordance with `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

VIX-2775 fixes a stale dropdown in an already-selected effect. After the change, adding a Mark Collection or renaming an existing Mark Collection refreshes the selected effect's `MarkCollectionId` editor dropdown immediately, so its choices use the current collection names. The selected collection's persisted `Guid` must not change merely because its display name changes, and a rename must not mark the effect dirty.

The behavior is demonstrable in the Timed Sequence Editor by selecting an effect with a converter-backed Mark Collection selector, then adding or renaming a Mark Collection while keeping the effect selected. The selector list updates without reselecting the effect. Focused unit tests prove the notification bridge, stable selection ID, no-dirty rename behavior, and event cleanup.

## Progress

- [x] (2026-09-04 00:00Z) Researched the existing BaseEffect collection lifecycle, mark collection contract/model, name converter, property-grid refresh path, and the existing BaseEffect selection tests.
- [x] (2026-09-08 15:53Z) Extended Wave and Liquid first-selection behavior: child mark-mode setters select the first current collection when their persisted ID is empty, while parent notifications continue to activate shared normalization for lifecycle repair. Validation: Release/x64 `Vixen_Tests` MSBuild target succeeded; focused Wave/Liquid tests passed 3/3; full suite passed 912/912.
- [x] (2026-09-08 16:19Z) Isolated Wave Mark Collection rename refreshes so display-only child notifications do not dirty or invalidate the selected or unrelated Wave effects. Validation: Release/x64 `Vixen_Tests` MSBuild target succeeded; focused Wave/Liquid tests passed 4/4; full suite passed 913/913.
- [x] (2026-09-08 16:27Z) Applied the same rename isolation to Liquid and restored Wave's non-dirty `Waves` notification after a collection add so its selector re-queries available values. Validation: Release/x64 `Vixen_Tests` MSBuild target succeeded; focused Wave/Liquid tests passed 6/6; full suite passed 915/915.
- [x] (2026-09-08 16:35Z) Restored presentation-only outer refresh notifications during Wave and Liquid name/list rebuilding so selected editors show renamed values and added/removed collections without marking effects dirty. Validation: Release/x64 `Vixen_Tests` MSBuild target succeeded; focused Wave/Liquid tests passed 8/8; full suite passed 917/917.
- [x] (2026-09-04 20:02Z) Completed Milestone 1: updated VIX-2775's description with user-facing summary, scope, acceptance criteria, and Release/x64 validation intent; status remains In Progress. Evidence: https://vixenlights.atlassian.net/browse/VIX-2775 (updated 2026-09-04 15:02:47.844-05:00).
- [x] (2026-09-04 20:05Z) Completed Milestone 2: added BaseEffect's private reference-identity subscription tracking, lifecycle synchronization, add/remove cleanup, selector refresh notifications, and disposal cleanup. Validation: `msbuild src\\Vixen.Modules\\Effect\\Effect\\Effect.csproj -t:Build -p:Configuration=Release -p:Platform=x64 -v:m` succeeded with four warnings in dependent projects.
- [x] (2026-09-04 20:11Z) Completed Milestone 3: added BaseEffect notification and cleanup regressions using the existing TestEffect seam. Validation: Release/x64 `Vixen_Tests` MSBuild target succeeded; focused `BaseEffectMarkCollectionSelectionTests` passed 8 of 8 tests (0 failed, 0 skipped).
- [x] (2026-09-04 15:59Z) Expanded the selector contract to LipSync and added a special-converter regression for a renamed selected legacy collection. Evidence: required Release/x64 MSBuild target succeeded; focused BaseEffect/LipSync tests passed 18/18; full suite passed 909/909.
- [ ] Align VIX-2775 with final behavior and add a validation-results comment.

## Surprises & Discoveries

- Observation: `IMarkCollection` already implements `INotifyPropertyChanged`, and the concrete `MarkCollection.Name` setter raises `PropertyChanged` with `Name`.
  Evidence: `src/Vixen.Core/Marks/IMarkCollection.cs` inherits `INotifyPropertyChanged`; `src/Vixen.Modules/App/Marks/MarkCollection.cs` calls `OnPropertyChanged(nameof(Name))` in its `Name` setter.

- Observation: the dropdown data source is already current whenever it is queried, and the effect property grid already has an asynchronous standard-values refresh path.
  Evidence: `src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs` enumerates `effect.MarkCollections` in `GetStandardValues`; `src/Vixen.Modules/Editor/EffectEditor/EffectPropertyEditorGrid.cs` queues standard-value refreshes from component/property notifications. The missing event is therefore on the effect instance, not in either UI component.

- Observation: `BaseEffect` already centralizes mark-selection normalization through sealed lifecycle overrides, and tests supply a minimal derived `TestEffect` seam.
  Evidence: `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` seals `MarkCollectionsChanged`, `MarkCollectionsAdded`, and `MarkCollectionsRemoved`; `src/Vixen.Tests/Effects/BaseEffectMarkCollectionSelectionTests.cs` uses `TestEffect` and `TestSelection` to exercise that behavior without a concrete effect module.

- Observation: assignment and add/remove lifecycle callbacks themselves now raise `MarkCollectionId`, so cleanup tests must discard events emitted by the action that removes or replaces a collection before asserting a later rename is silent.
  Evidence: the focused tests clear their recorded effect-property events after removal, replacement, or disposal and then raise a collection name change; all eight focused tests passed.

- Observation: the initial BaseEffect bridge used a property name that is not present on every derived effect.
  Evidence: the updated regression suite includes a BaseEffect-derived non-selector, whose renamed collection produces no `MarkCollectionId` event; focused tests passed 9/9.

- Observation: LipSync's special converter already preserves a selected legacy collection while excluding other non-Phoneme collections.
  Evidence: `LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames` includes a collection when it is Phoneme-typed or its ID is the selected ID; the new regression passes after renaming the selected Generic collection.

- Observation: Waveform exposes the active-selection inputs but did not notify the owning Wave effect when `WaveType` or `UseMarks` changed; Liquid's `FlowControl` already raises that notification.
  Evidence: `Waveform` previously called only its attribute-update helpers from those setters, while `Emitter.FlowControl` calls `OnPropertyChanged()`; both child collections already forward child notifications to their parent effects.

- Observation: forwarding activation through the parent does not guarantee that the expando-model property edited by the UI receives its new display value in every editor path.
  Evidence: user testing still showed blank Wave and Liquid selectors after the parent-trigger implementation. A Rider test-debugger breakpoint could not bind to the dynamically loaded Wave module, so the fix was moved to the child setters that own `MarkCollectionId` and verified without a parent event bridge.

- Observation: Wave subscribes to every sequence Mark Collection, and its rename refresh previously emitted child `MarkCollections` and `MarkCollectionName` notifications for every waveform. The generic child handler treated each as an effect-data edit.
  Evidence: `Wave.MarkCollectionPropertyChanged` called `UpdateMarkCollectionNames()` without checking whether a waveform selected the renamed ID; `OnWavesChildPropertyChanged` then called `MarkDirty()` and raised `Waves` for each forwarded child notification.

- Observation: Liquid has the equivalent all-collection subscription and restored each emitter's ID while rebuilding names; Wave requires an outer `Waves` notification after an add because its expando converter reads the current child collection for standard values.
  Evidence: `Liquid.UpdateMarkCollectionNames()` calls `EmitterList.UpdateSelectedMarkCollectionNames()` and reassigns every emitter ID; `ExpandoMarkCollectionNameConverter.GetStandardValues` reads `IMarkCollectionExpandoObject.MarkCollections`, and the focused add regression observes the required `Waves` notification.

## Decision Log

- Decision: Implement the refresh bridge only in `BaseEffect`; do not modify `IMarkCollectionNameConverter`, `EffectPropertyEditorGrid`, or individual effects.
  Rationale: the converter reads the live collection-name list and the grid already responds to effect property notifications. `BaseEffect` is the shared owner of the collection lifecycle for the affected basic effects, so it is the narrow missing bridge.
  Date/Author: 2026-09-04 / Codex, from the VIX-2775 handoff and source research.

- Decision: Notify `MarkCollectionId` for collection additions, removals, assignment/reset, and a subscribed collection's `Name`, null, or empty property name notification; do not write a selection ID or call `MarkDirty` on a rename.
  Rationale: the property name causes the grid to requery the selector's standard values. IDs, rather than names, are persisted selections, so a rename changes only presentation and must not create an effect-data change.
  Date/Author: 2026-09-04 / Codex, from the approved handoff.

- Decision: Keep lifecycle ordering as normalization, effect-specific callback work, then the UI notification. Removal cleanup remains before normalization so no listener survives a collection that has left the sequence.
  Rationale: existing derived hooks depend on normalized selections and listener cleanup. The new notification is intentionally the final observable event so the UI never refreshes against partially updated effect state.
  Date/Author: 2026-09-04 / Codex, from the current BaseEffect implementation and handoff ordering requirement.

- Decision: Configure the private TestEffect's descriptor and module data so its inherited disposal path can run as it does for a real effect while preserving the existing lightweight test seam.
  Rationale: BaseEffect disposal consults descriptor and module-data infrastructure that the prior normalization-only seam did not need. The test configuration makes the disposed-collection regression exercise actual BaseEffect cleanup instead of a test-only substitute.
  Date/Author: 2026-09-04 / Codex, during Milestone 3 implementation.

- Decision: Introduce `Vixen.Marks.IMarkCollectionSelector` as a separate editor-facing string-property contract; do not merge it with `IMarkCollectionSelection`.
  Rationale: the latter represents persisted `Guid` selections used by both effects and child models such as Waveforms and Liquid Emitters. Only the former guarantees the public property BaseEffect must refresh. The standard converter-backed effects already satisfy it, while Wave and Liquid remain intentionally outside this scope.
  Date/Author: 2026-09-04 / Codex, during selector-contract hardening.

- Decision: Include LipSync in `IMarkCollectionSelector` while retaining its specialized converter and collection filter.
  Rationale: LipSync has the same editor-facing string selector property and needs the same BaseEffect refresh event. Its special allowed-value policy is already isolated in its converter and is preserved by a dedicated regression rather than a one-off notification path.
  Date/Author: 2026-09-04 / Codex, at user direction.

- Decision: Trigger existing BaseEffect selection activation from Wave and Liquid child-mode notifications instead of adding a second first-collection policy to their child models.
  Rationale: Waveforms and Emitters already implement `IMarkCollectionSelection` with `AllowsFirstCollectionFallback` enabled. Calling `ActivateMarkCollectionSelections()` at the parent reuses the central policy, preserves valid selections, and performs the existing listener and display refresh work.
  Date/Author: 2026-09-08 / Codex, at user direction.

- Decision: Make Waveform and Emitter select the first collection directly when their own mark-driven settings become active and their ID is empty; retain the parent activation notifications as a complementary lifecycle path.
  Rationale: the child owns both the editor-facing name and persisted ID, so setting the ID at that boundary makes the editor update independent of collection-to-parent forwarding. The fallback applies only to an empty ID, preserving an existing valid choice; BaseEffect remains responsible for normalizing stale IDs and collection lifecycle changes.
  Date/Author: 2026-09-08 / Codex, after user validation of the parent-only implementation.

- Decision: Treat Wave name-refresh child notifications as presentation-only and refresh only an effect that has a waveform selecting the renamed collection.
  Rationale: a rename does not change a persisted Wave setting, so it must not dirty or invalidate Wave effects. The waveform itself still raises its display-name notification for the nested editor; the parent ignores it while the scoped refresh is in progress.
  Date/Author: 2026-09-08 / Codex, after user-reported gray Wave effects on rename.

- Decision: Apply the presentation-only rename guard to Liquid and explicitly notify Wave's outer `Waves` property after a collection add without marking the effect dirty.
  Rationale: Liquid's emitter refresh has the same event shape as Wave's. Wave has no effect-level selector contract, so its nested expando editor needs a non-dirty outer notification to requery the live collection list after an add.
  Date/Author: 2026-09-08 / Codex, at user direction.

## Outcomes & Retrospective

Milestone 2 centralized the event bridge in BaseEffect without changing selection policy, persisted data, converter behavior, or property-grid behavior. Milestone 3 added eight focused regressions covering add, rename, arbitrary refresh names, ignored property changes, and removed/replaced/disposed cleanup. The selector-contract amendment added a documented Core contract, opted in every effect-level editor selector—including LipSync—and added regressions for the non-selector boundary and LipSync's renamed selected legacy collection. The Release/x64 target, 18-test focused suite, and full 909-test suite pass. User manual testing confirms the editor behavior. Jira closeout remains.

The Wave and Liquid extension closes the remaining first-collection activation gap without extending the selector-refresh contract to their child editor properties. A Decaying Sine waveform that enables marks, a waveform changed to Decaying Sine after marks are enabled, and an emitter that changes to mark-controlled flow now set their own empty ID to the first current collection. Wave name renames now refresh only the selected waveform's displayed name without dirtying or invalidating any Wave effect. The Release/x64 test target, four focused regressions, and the full 913-test suite pass. Jira closeout and a desktop manual check remain.

## Context and Orientation

Mark Collections are named tracks of marks owned by a timed sequence. Each `IMarkCollection` has a stable `Guid` `Id`, a display `Name`, and a `PropertyChanged` event. Effects persist a Mark Collection selection by ID in their own data model. Several effects expose that selection through a property named `MarkCollectionId`; the `IMarkCollectionNameConverter` supplies the dropdown names by enumerating the effect's current `MarkCollections` list.

`src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` owns the `MarkCollections` property and invokes virtual callbacks when a list is assigned, changed, added to, or removed from. `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` is the common implementation for basic effects. Its sealed overrides normalize the active ID-based selections and then call protected `MarkCollectionsChangedCore`, `MarkCollectionsAddedCore`, and `MarkCollectionsRemovedCore` hooks for effect-specific listener or display work. It also owns existing mark-content listeners and disposal cleanup.

The editor's property grid observes property changes from the selected effect and can refresh a property's standard values. It does not receive a notification when a Mark Collection's own `Name` changes, which is why an open effect selector becomes stale. This work adds that forwarding notification without changing the converter or grid.

The initial bridge changes `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` and `src/Vixen.Tests/Effects/BaseEffectMarkCollectionSelectionTests.cs`. The selector-contract amendment also adds `src/Vixen.Core/Marks/IMarkCollectionSelector.cs` and updates only the class declarations of Alternating, Dissolve, Fireworks, Shapes, State, Strobe, Text, and LipSync. The new public interface requires XML documentation; no converter, property-grid, persisted-data, or selector setter logic changes are needed.

Wave and Liquid differ from the effect-level selectors because their persisted choices live in child models: `Waveform` and `Emitter`. Both child types already satisfy `IMarkCollectionSelection`, but their active state changes after BaseEffect's collection lifecycle has completed. Their parent effects already receive child property notifications, making those handlers the narrow place to call `ActivateMarkCollectionSelections()` when a child enters mark mode.

## Plan of Work

### Milestone 1 — Record the executable VIX-2775 scope

Before changing local source, update VIX-2775 through the repository's Jira workflow. Read `.agents/skills/jira/SKILL.md` completely and use its supported tracker tooling. Put the following requirements, acceptance criteria, and test plan in the issue description or an implementation comment: selected converter-backed effect dropdowns refresh when Mark Collections are added or renamed; the implementation listens in `BaseEffect`; a `Name`, null, or empty collection property-name event raises `MarkCollectionId`; add/remove/reset/assignment retain their existing normalized lifecycle and notify last; renames preserve the selected ID and clean state; and removed, replaced, and disposed collections cannot continue producing notifications. State that Alternating, Dissolve, Fireworks, Shapes, State, Strobe, and Text expose this selector as `MarkCollectionId`, while the fix itself remains shared rather than per-effect.

The milestone is complete when the Jira record has enough detail for a tester to perform the editor scenario and run the focused/full tests below. Do not transition the issue unless separately instructed.

### Milestone 2 — Add the BaseEffect collection-name notification bridge

In `src/Vixen.Modules/Effect/Effect/BaseEffect.cs`, add private subscription tracking for the `PropertyChanged` event on `IMarkCollection` instances. Use a collection that represents exactly the instances to which this effect is currently subscribed; it must prevent duplicate handler registration and permit deterministic removal. Keep the tracking private to `BaseEffect` and do not alter `EffectModuleInstanceBase`.

Add private helpers to subscribe one non-null collection, unsubscribe one tracked collection, and synchronize the tracked subscriptions to the current `MarkCollections` sequence. Synchronization must detach every tracked collection no longer present and attach every current collection not yet tracked. It must tolerate `MarkCollections == null`. In the sealed `MarkCollectionsChanged` override, synchronize all collection property subscriptions, then preserve the existing `NormalizeMarkCollectionSelections()` and `MarkCollectionsChangedCore()` order, and finally call `OnPropertyChanged("MarkCollectionId")`.

In the sealed `MarkCollectionsAdded` override, retain the current normalize/core/conditional changed-core behavior. Subscribe precisely the `addedCollections` instances as part of that lifecycle, before the final UI notification. In the sealed `MarkCollectionsRemoved` override, unsubscribe precisely the `removedCollections` instances before the existing mark-listener removal and normalization work. Preserve its current normalize/core/conditional changed-core behavior. In both overrides call `OnPropertyChanged("MarkCollectionId")` only after all existing normalization and effect-specific callback work completes. This is required even if normalization did not alter an ID, because the dropdown's available names changed.

Add a private collection `PropertyChanged` handler. When `PropertyChangedEventArgs.PropertyName` is `"Name"`, null, or empty, call only `OnPropertyChanged("MarkCollectionId")`. Ignore all other collection property names. In particular, do not set any selector value, call `NormalizeMarkCollectionSelections`, call `MarkDirty`, or invoke effect-specific lifecycle hooks from this handler.

Update `Dispose(bool disposing)` so the disposing path removes the collection-`PropertyChanged` handler from every tracked collection and clears the tracking collection before calling `base.Dispose(disposing)`. Retain the existing mark-content listener cleanup. Cleanup must not depend on `SupportsMarks` or on the current `MarkCollections` reference, because a prior list may already have been replaced. This ensures old and disposed objects cannot retain the effect or trigger stale UI events.

Do not change `src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs`, `src/Vixen.Modules/Editor/EffectEditor/EffectPropertyEditorGrid.cs`, selection normalization rules, persisted data types, or dirty-state behavior other than preserving the existing lifecycle effects. Use tabs and LF line endings, and do not reformat unrelated legacy code.

### Milestone 3 — Prove notifications, identity stability, and cleanup

Extend `src/Vixen.Tests/Effects/BaseEffectMarkCollectionSelectionTests.cs` rather than creating a separate fixture. Reuse its existing `TestEffect` seam and augment it only with narrowly scoped test helpers needed to expose the selected `Guid`, clear/check dirty state, subscribe to effect `PropertyChanged`, and dispose the test effect. Do not introduce production test-only APIs.

Add focused xUnit tests with concrete `MarkCollection` instances and event-name collection/assertion helpers. The tests must demonstrate all of the following observable contracts:

- Adding a collection to an assigned `ObservableCollection<IMarkCollection>` raises an effect `PropertyChanged` event named `MarkCollectionId` after the add lifecycle. Continue asserting the existing normalization-before-core-hook test so the new notification does not change its ordering guarantee.
- Renaming a currently subscribed collection raises `MarkCollectionId`. Set a valid selected ID first, explicitly make the test effect clean, rename the collection, then assert that the selected ID remains exactly the same and `IsDirty` remains false. This proves a name refresh is not selection repair or persisted-data mutation.
- The private handler treats both null and empty property names as a general refresh notification, and it ignores a non-name property such as `Locked`. Use a minimal test collection or a testable `MarkCollection` subclass/seam that can raise arbitrary `PropertyChangedEventArgs`; do not rely on reflection into BaseEffect.
- A collection removed from the assigned sequence cannot raise a later `MarkCollectionId` notification. Use the original collection reference, remove it, clear the recorded effect events after the removal lifecycle, then rename it and assert no new event.
- A collection from a replaced `MarkCollections` list cannot raise a later notification. Assign an initial list, replace it with a distinct list, clear recorded events after assignment, rename an item from the old list, and assert no new event. Also rename an item in the replacement list and assert the bridge still raises `MarkCollectionId`.
- A collection cannot notify a disposed effect. Subscribe to the effect event, dispose the test effect, clear any events already recorded, rename the formerly tracked collection, and assert no later effect notification. The test should dispose in a `finally` block when needed so test failures do not leave subscriptions alive.

Keep tests deterministic and free of WPF dispatcher dependencies. Assertions should count or inspect only newly recorded events after each action so the expected add/remove/assignment notifications do not obscure the cleanup assertions.

### Milestone 3a — Harden the editor selector contract

Add `src/Vixen.Core/Marks/IMarkCollectionSelector.cs` with a documented public `string MarkCollectionId { get; set; }` property. This contract represents only the editor-facing display-name proxy; it is not the persisted `Guid` selection contract. Declare Alternating, Dissolve, Fireworks, Shapes, State, Strobe, Text, and LipSync as implementations, relying on their existing public string properties. Do not include Waveform or Liquid Emitter types.

In `BaseEffect`, replace each literal selector refresh with a private helper that calls `OnPropertyChanged(nameof(IMarkCollectionSelector.MarkCollectionId))` only when the effect implements `IMarkCollectionSelector`. Retain all existing lifecycle positions and no-dirty rename behavior. Update the test seam to use a distinct persisted `Guid` accessor and a test-owned string selector, then add a non-selector derived-effect test that verifies name changes do not raise the selector property event.

Extend `src/Vixen.Tests/Effects/LipSyncMarkCollectionNameConverterTests.cs` with a regression that assigns collections to a real `LipSync` effect, selects a Generic legacy collection, and renames it. Verify the effect raises `IMarkCollectionSelector.MarkCollectionId`, its selected display value changes to the new name, and the special converter includes that renamed value with Phoneme collections while excluding an unselected non-Phoneme collection. This proves the shared refresh target is active without weakening LipSync's specialized selection policy.

### Milestone 3b — Normalize child selections when Wave or Liquid enters mark mode

In `src/Vixen.Modules/Effect/Wave/Wave/Waveform.cs`, raise the existing child `PropertyChanged` event after `WaveType` and `UseMarks` change, and set an empty `MarkCollectionId` to the first current collection whenever both mark-driven conditions are active. In `src/Vixen.Modules/Effect/Liquid/Liquid/Emitter.cs`, do the same when `FlowControl` becomes `UseMarks`. The child fallback must leave a nonempty ID unchanged. Keep the parent calls to `ActivateMarkCollectionSelections()` in `Wave.OnWavesChildPropertyChanged` and `Liquid.EmitterListChildPropertyChanged` so stale selections still receive the shared lifecycle normalization.

Add `src/Vixen.Tests/Effects/WaveAndLiquidMarkCollectionSelectionTests.cs`, and add direct Wave and Liquid project references to `src/Vixen.Tests/Vixen.Tests.csproj`. The tests must assign an ordered collection list directly to an unselected child, then assert that enabling marks on Decaying Sine Wave, selecting Decaying Sine after marks were enabled, and choosing `FlowControl.UseMarks` on Liquid each retain the first collection ID.

### Milestone 3c — Keep Wave rename refreshes presentation-only

In `src/Vixen.Modules/Effect/Wave/Wave/Wave.cs`, have the collection name-change handler refresh Waveforms only when one selects the renamed collection. While `UpdateMarkCollectionNames()` raises child display notifications, prevent `OnWavesChildPropertyChanged` from marking the effect dirty or raising the outer `Waves` property. This preserves the nested editor's child-name update without causing a render invalidation. Add a regression that renames a selected collection shared by a selected and unrelated Wave effect, verifies the selected waveform name changes, and verifies both effects remain clean.

### Milestone 4 — Validate and close the tracker loop

First run the focused BaseEffect test class while iterating, but build the test target using the repository's full MSBuild command before any `--no-build` execution. `Vixen.Tests` has C++/CLI transitive dependencies and cannot be reliably built by `dotnet test` alone. From `C:\Dev\Vixen`, run:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Then run the already-built focused test class, followed by the full suite using the same Release/x64 output:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~BaseEffectMarkCollectionSelectionTests -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

Expect the MSBuild target to succeed and both test commands to report zero failed tests. Record actual passed/skipped totals rather than predicting them. If the focused test assembly is stale or missing, rerun the MSBuild command; do not drop `--no-build` or replace it with an unsupported plain `dotnet test` build.

Perform a manual Timed Sequence Editor check if a desktop session is available. Create or open a timed sequence with a converter-backed effect (for example, Alternating, Dissolve, Fireworks, Shapes, State, Strobe, or Text) selected in the Effect Property Editor. Add a Mark Collection and confirm the selected effect's `MarkCollectionId` dropdown gains it without changing effect selection. Rename a listed Mark Collection and confirm the dropdown displays the new name while the effect remains selected and is not rendered/marked modified solely because of the rename. Record if this cannot be performed in the environment.

Finally, reread VIX-2775. If implementation discoveries changed the requirement, acceptance criteria, or test plan, correct its description. Add a concise comment with the user-visible result, exact command results, manual-check result or limitation, and residual risk. Update all living-plan sections and append a dated revision note. If a milestone changed repository files, read and use `.agents/skills/commit-msg/SKILL.md` to include a proposed commit message in the milestone handoff, but do not create a commit unless explicitly requested.

## Concrete Steps

1. From `C:\Dev\Vixen`, run `git status --short`; preserve unrelated user changes. Re-read `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` and `src/Vixen.Tests/Effects/BaseEffectMarkCollectionSelectionTests.cs` before editing.
2. Complete Milestone 1 using the project Jira skill. If the tracker is unavailable due to credentials or connectivity, record that condition in this plan and continue with local implementation and validation; do not alter unrelated tracker state.
3. Implement Milestone 2 with `apply_patch`. Re-read the final diff to verify only BaseEffect subscription/notification/disposal behavior changed.
4. Implement Milestone 3 with `apply_patch`. Run the full-MSBuild target and focused test command after the test changes, fixing only failures in scope.
5. Run the full prescribed test command, carry out the manual check when possible, complete Milestone 4's Jira closeout, and update `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` with concrete evidence.

Expected successful output has this shape; replace placeholders with real totals in the completed plan:

    Build succeeded.
    Passed!  - Failed: 0, Passed: <actual>, Skipped: <actual>, Total: <actual>

## Validation and Acceptance

Automated acceptance requires the focused tests to fail before the bridge is implemented and pass afterward. They must establish that add and rename events raise `MarkCollectionId` for a selector participant, that rename preserves the exact selected `Guid` and a clean `IsDirty` state, and that removed, replaced, and disposed collections cannot generate stale notifications. The arbitrary null/empty property-name tests confirm the handler follows standard `INotifyPropertyChanged` semantics rather than only the current `MarkCollection.Name` setter implementation. The contract test also establishes that a BaseEffect-derived non-selector receives no editor-selector event.

Wave and Liquid acceptance requires that a child with no selected ID receives the first collection ID when it enters its mark-driven mode. This must happen at the child setter so the effect-property editor observes its name update. A pre-existing valid ID remains unchanged. Renaming a selected collection updates only the selected child display name and leaves selected and unrelated effects clean. Adding a collection emits Wave's non-dirty outer selector refresh.

Wave rename acceptance requires that the selected waveform displays the new collection name while neither it nor unrelated Wave effects become dirty or are invalidated.

Full acceptance requires a successful Release/x64 `Vixen_Tests` MSBuild target followed by the prescribed `dotnet test --no-build --no-restore` suite with zero failures.

Human acceptance is met when an effect remains selected while a Mark Collection is added or renamed and its `MarkCollectionId` dropdown immediately shows the current list/name. The collection's ID-backed selection remains the same after rename, and the rename does not make the effect dirty. Effects not using the converter are not required to add their own notification logic.

## Idempotence and Recovery

The subscription helpers must be idempotent: synchronizing the same list repeatedly must leave one handler per collection, and removing an already-unsubscribed collection must be harmless. Replacing a list must detach all stale collection-property handlers even though `EffectModuleInstanceBase` owns the list reference and collection-change subscription.

No persisted data migration, sequence rewrite, or destructive operation is involved. If a regression is found, revert only the new private collection-property tracking and notification bridge in `BaseEffect` plus its tests. Do not compensate by changing converter behavior, forcing a property-grid reload, or adding individual-effect subscriptions; those would duplicate the shared lifecycle responsibility and could cause stale subscriptions to recur.

## Artifacts and Notes

The required final lifecycle ordering is represented by this pseudocode. It is descriptive rather than a mandate for exact helper names:

    MarkCollectionsChanged():
        synchronize collection PropertyChanged subscriptions to current MarkCollections
        normalize active ID selections
        run MarkCollectionsChangedCore()
        raise effect PropertyChanged("MarkCollectionId")

    MarkCollectionsAdded(added):
        normalize active ID selections
        run MarkCollectionsAddedCore(added)
        if normalization changed a selection: run MarkCollectionsChangedCore()
        subscribe exactly added collections to PropertyChanged
        raise effect PropertyChanged("MarkCollectionId")

    MarkCollectionsRemoved(removed):
        unsubscribe exactly removed collections from PropertyChanged
        remove their existing mark-content listeners
        normalize active ID selections
        run MarkCollectionsRemovedCore(removed)
        if normalization changed a selection: run MarkCollectionsChangedCore()
        raise effect PropertyChanged("MarkCollectionId")

    collection PropertyChanged(sender, args):
        if args.PropertyName is "Name", null, or empty:
            raise effect PropertyChanged("MarkCollectionId")
        do not mutate selection state or dirty state

    Dispose(disposing):
        detach collection PropertyChanged subscriptions from every tracked instance
        clear tracking
        retain existing mark-content listener cleanup
        call base.Dispose(disposing)

The converter deliberately returns display strings, while effect data uses IDs. The bridge exists solely to prompt the already-present property-grid standard-value refresh mechanism to call the converter again.

## Interfaces and Dependencies

No new external dependency, project reference, project file change, serialization member, or protected API is required. `Vixen.Marks.IMarkCollectionSelector` is a new documented public Core interface with this member:

    string MarkCollectionId { get; set; }

It describes the editor's display-name proxy, not the persisted identifier exposed by `IMarkCollectionSelection`.

The implementation uses existing BCL `System.ComponentModel.INotifyPropertyChanged` / `PropertyChangedEventArgs`, existing `Vixen.Marks.IMarkCollection`, and the inherited protected `EffectModuleInstanceBase.OnPropertyChanged(string)` and `MarkDirty()` behavior. The final BaseEffect private handler conceptually has this signature:

    private void MarkCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)

It must issue `OnPropertyChanged("MarkCollectionId")` only for `Name`, null, or empty `PropertyName`. Its subscription helpers remain private implementation details, so consumers and concrete effects continue using the existing `BaseEffect` lifecycle hooks unchanged.

## Revision Note

2026-09-04 / Codex: Initial planning-only ExecPlan created from the VIX-2775 handoff after inspecting the effect lifecycle, mark collection model, current converter/grid refresh behavior, existing selection tests, and the preceding VIX-3995 lifecycle plan. No product or test implementation was performed.

2026-09-04 / Codex: Completed Milestone 1 by replacing VIX-2775's stale two-item description with the approved user-facing summary, scope, acceptance criteria, and validation intent. The issue remains In Progress; no source or test implementation was performed.

2026-09-04 / Codex: Completed Milestone 2 by adding BaseEffect-only reference-identity tracking for Mark Collection property notifications. Assignment/reset synchronizes the complete subscription set; add/remove manage the exact event instances; name, null, and empty collection notifications refresh `MarkCollectionId`; and disposal removes every tracked handler. The existing normalization and effect-specific callbacks remain before the new selector notification. A Release/x64 module build succeeded; behavioral regression tests remain planned for Milestone 3.

2026-09-04 / Codex: Completed Milestone 3 by extending the existing BaseEffect test seam with real disposal configuration and eight focused lifecycle/notification regressions. The Release/x64 `Vixen_Tests` target rebuilt the test assembly, and the focused filter passed 8/8. Full-suite validation and the manual editor check remain deliberately deferred to Milestone 4.

2026-09-04 / Codex: Amended the plan and completed selector-contract hardening after the bridge proved correct in manual testing. Added documented Core `IMarkCollectionSelector`; opted in Alternating, Dissolve, Fireworks, Shapes, State, Strobe, and Text; guarded BaseEffect notification by that contract; and added a non-selector boundary regression. The required Release/x64 MSBuild target succeeded, focused tests passed 9/9, and the full suite passed 908/908. The contract is intentionally separate from `IMarkCollectionSelection`; Waveform and Liquid Emitter remain excluded.

2026-09-04 / Codex: Expanded the selector contract to LipSync at user direction so all effect-level editor selectors participate consistently. LipSync retains its special phoneme-filtering converter; its regression proves a renamed selected legacy collection remains visible with Phoneme collections and nonselected non-Phoneme collections remain absent. The required Release/x64 build succeeded, focused tests passed 18/18, and the full suite passed 909/909.

2026-09-08 / Codex: Implemented the requested Wave and Liquid first-collection activation extension. Waveform now notifies its parent after `WaveType` and `UseMarks` changes; Wave and Liquid parent child-change handlers activate the existing BaseEffect normalization for their mark-mode properties. Added direct test-project references and two focused regressions. The required Release/x64 MSBuild target succeeded, focused tests passed 2/2, and the full suite passed 911/911. No desktop manual check or Jira closeout was performed in this change.

2026-09-08 / Codex: Corrected the Wave and Liquid implementation after user validation showed the parent-only trigger left editor selectors blank. The first-collection fallback now runs in the child setters that own the persisted ID and display name; it is limited to empty IDs and retains parent lifecycle normalization. Added the Wave mode-order regression. The required Release/x64 MSBuild target succeeded, focused tests passed 3/3, and the full suite passed 912/912. A Rider debugger probe was cleaned up after it could not bind to the dynamically loaded Wave module; no desktop manual check or Jira closeout was performed.

2026-09-08 / Codex: Fixed user-reported gray Wave effects after a Mark Collection rename. Wave now scopes the rename refresh to effects with a matching selected ID and ignores the resulting display-only child notifications at the outer effect level. Added a regression proving the selected name updates while both selected and unrelated Wave effects remain clean. The required Release/x64 MSBuild target succeeded, focused tests passed 4/4, and the full suite passed 913/913. No desktop manual check or Jira closeout was performed.

2026-09-08 / Codex: Extended rename isolation to Liquid and fixed Wave's missing collection-add selector refresh. Liquid now ignores internal display-only emitter notifications while rebuilding its name list; Wave emits `Waves` after an add without becoming dirty. Added focused regressions for both cases. The required Release/x64 MSBuild target succeeded, focused tests passed 6/6, and the full suite passed 915/915. No desktop manual check or Jira closeout was performed.

2026-09-08 / Codex: Corrected the presentation-refresh guard after user testing showed that it also prevented nested editor lists from updating. Wave now raises `Waves` while suppressing only dirty-state mutation, and Liquid raises `EmitterList` once after rebuilding its display-name list. Added Wave removal and Liquid add/remove regressions plus outer-notification assertions. The required Release/x64 MSBuild target succeeded, focused tests passed 8/8, and the full suite passed 917/917. No desktop manual check or Jira closeout was performed.

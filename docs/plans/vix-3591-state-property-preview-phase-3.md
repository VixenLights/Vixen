# Add Interactive Live Preview to State Property Setup

This ExecPlan is a living document. Maintain the `Progress`, `Surprises & Discoveries`, `Decision Log`, and
`Outcomes & Retrospective` sections while implementing the work.

This document follows `.agents/PLANS.md`. It is the Phase 3 implementation plan for
`docs/state-property-item-update.md`. It builds on the completed State Property work described by
`docs/vix-3591-state-feature.md` and `docs/vix-3591-state-property-prerequisites.md`, but it is self-contained: an
implementer can execute this plan using the current working tree without reading the earlier plans or this plan's authoring
conversation.

## Purpose / Big Picture

The State Property Setup dialog lets a user assign element nodes to named State items such as `Open`, `Closed`, or
`Waving`. A State item preview is useful while editing because the user can see which physical or simulated lights belong to
the row. The current branch contains an initial selected-row preview spike, but it activates preview immediately, does not
offer a Preview toggle, and does not respond correctly to assignment edits, color changes, grouped rows, overlapping colors,
or dialog cleanup.

After this change, the user can turn Preview on explicitly, choose whether to preview the selected row or a State Item Group,
and see Live Preview update immediately while editing. Preview remains off by default. Same-name rows can preview together.
The same leaf element can remain active in more than one color. Turning Preview off clears active effects in the isolated
`"State Preview"` Live Preview context. Closing the dialog releases that named context entirely so setup work does not leave
stale lights or an unused live context behind.

The user-visible proof is to open a State property in Display Setup, turn Preview on, check and uncheck assigned leaves,
switch between selected-row and State Item Group modes, change colors, and close the dialog. The visible preview must follow
each action and must be clear after the dialog closes.

## Progress

- [x] (2026-06-01 21:31 -05:00) Read `.agents/PLANS.md`, the Phase 3 specification, the current State Property code, the
  uncommitted preview spike, and the project skills `dotnet-best-practices`, `csharp-async`, `csharp-docs`,
  `dotnet-design-pattern-review`, and `catel-mvvm`.
- [x] (2026-06-01 21:31 -05:00) Inspect the Live Preview message contract, static broadcast implementation, existing State
  tests, WPF theme resources, and Catel 6.2 lifecycle documentation.
- [x] (2026-06-01 21:31 -05:00) Write the Phase 3 ExecPlan and Jira-ready append block.
- [x] (2026-06-02 09:59 -05:00) Revise the plan for the typed `BroadcastChannel<TMessage>` contract and the new
  `ReleaseContextMessage`. Dialog close now releases the named context instead of leaving a future-release TODO.
- [x] (2026-06-02) Harden `BroadcastChannel<TMessage>` as a sealed reference type with an internal named constructor,
  validate typed Broadcast overloads, repair the State spike to publish through declared `LivePreviewChannels`, and allow
  Broadcast publication when no WPF application dispatcher exists.
- [x] (2026-06-02 12:41 -05:00) Confirm the Phase 3 Jira markdown in this plan was added to issue `VIX-3591`.
- [x] (2026-06-02 12:41 -05:00) Establish the Milestone 1 baseline: `31` focused State tests pass and the full Debug
  rebuild completes successfully.
- [ ] Replace the selected-row preview spike with an internal, testable preview publisher and coordinator.
- [ ] Add State mapper preview controls, State Item Group selection, refresh triggers, and Catel close cleanup.
- [ ] Add focused coordinator and mapper automated tests, then run the complete State test suite.
- [ ] Run Debug and Release builds, whitespace and line-ending checks, and the manual Display Setup acceptance scenarios.
- [ ] Update this living plan with implementation discoveries, evidence, final outcomes, and a revision note.

## Surprises & Discoveries

- Observation: The current branch already contains an uncommitted selected-row preview spike.
  Evidence: `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs` publishes
  `TurnOffElementsMessage` and `TurnOnElementsMessage` from the `SelectedItem` setter, and
  `src/Vixen.Modules/Property/State/State.csproj` adds `Broadcast` and `Messages` project references.

- Observation: The spike cannot be promoted unchanged because it sends messages while the dialog opens, does not retain a
  dialog-local active preview set, uses `Int16.MaxValue` instead of `int.MaxValue`, and handles only one selected row.
  Evidence: `StateMapperViewModel.SelectedItem` invokes `TurnOffPreview` and `TurnOnPreview` unconditionally, and
  `TurnOnPreview` constructs `ElementState.Duration = Int16.MaxValue`.

- Observation: Live Preview activation and deactivation are intentionally asymmetric. Activation carries one
  `ElementState` per `(element ID, color)` pair, but deactivation terminates by element ID.
  Evidence: `src/Vixen.Modules/App/LivePreview/LivePreviewService.cs` builds one `EffectNode` per turn-on state, while
  `TurnOffElements` calls `context.TerminateNodes(states.Select(s => s.Id))`.

- Observation: `ElementState.Duration` is an `int`, so the highest supported finite duration requested by Phase 3 is
  `int.MaxValue`.
  Evidence: `src/Vixen.Common/Messages/LivePreview/ElementState.cs` declares `public int Duration { get; init; }`.

- Observation: The mapper already listens to each row ViewModel's `PropertyChanged` event, and the row raises
  `AssignmentCount` after assignment-tree changes. Phase 3 can extend that event path rather than adding WPF event handlers.
  Evidence: `StateMapperViewModel.AttachItem` subscribes to `item.PropertyChanged`; `StateItemViewModel.UpdateAssignments`
  raises `PropertyChanged` for `AssignmentCount`.

- Observation: Catel 6.2 provides a ViewModel lifecycle hook that runs after save, cancel, or direct close processing.
  Evidence: the installed Catel XML documentation for `ViewModelBase.OnClosedAsync(bool?)` states that it is called when the
  ViewModel has just been closed. `CloseViewModelAsync(bool?)` is documented as running after `CancelAsync()` or
  `SaveAsync()`.

- Observation: The WPF theme already provides resources needed for the UI without adding converters or code-behind.
  Evidence: `src/Vixen.Common/WPFCommon/Theme/Theme.xaml` contains `BooleanToVisibilityConverter` and toggle-button styles.

- Observation: State mapper tests are intentionally serialized because Catel ViewModel validation is not safe to construct
  concurrently in the existing fixtures.
  Evidence: `src/Vixen.Tests/Property/State/StateMapperTestCollection.cs` defines a non-parallel xUnit collection used by
  `StateMapperDraftTests` and `StateMapperValidationTests`.

- Observation: Static `Broadcast.Publish` is an application boundary and is inconvenient for deterministic unit tests.
  Evidence: `src/Vixen.Common/Broadcast/Broadcast.cs` dispatches through `WeakReferenceMessenger.Default` and may invoke the
  WPF application dispatcher when the current thread is not `VixenSystem.UIThread`.

- Observation: Correcting the State spike to use a declared channel exposed a Broadcast null dereference when tests publish
  without a running WPF application.
  Evidence: State mapper construction published through `LivePreviewChannels.TurnOnElements` during focused tests, and
  `Application.Current` was `null`. `Broadcast.Publish` now dispatches directly when no WPF application exists and retains
  dispatcher marshalling when an application is available.

- Observation: Live Preview channels are now typed declarations rather than raw channel-name constants.
  Evidence: `src/Vixen.Common/Messages/LivePreview/LivePreviewChannels.cs` exposes
  `BroadcastChannel<TurnOnElementsMessage>`, `BroadcastChannel<TurnOffElementsMessage>`,
  `BroadcastChannel<ClearActiveEffectsMessage>`, and `BroadcastChannel<ReleaseContextMessage>` values. The typed
  `Broadcast.Publish`, `Subscribe`, and `Unsubscribe` overloads use the channel declaration's message type.

- Observation: The State spike initially attempted to adapt to typed channels incorrectly by constructing default channel
  values. Contract hardening now prevents that misuse.
  Evidence: Before the prerequisite fix, `StateMapperViewModel.TurnOnPreview` and `TurnOffPreview` called
  `Broadcast.Publish(new BroadcastChannel<TMessage>(), message)`. `BroadcastChannel<TMessage>` is now a sealed reference
  type with an internal constructor that requires a non-whitespace name, and the spike publishes through
  `LivePreviewChannels.TurnOnElements` and `LivePreviewChannels.TurnOffElements`.

- Observation: Live Preview now supports releasing a named context through broadcast, and turn-off payloads carry IDs
  directly.
  Evidence: `src/Vixen.Common/Messages/LivePreview/ReleaseContextMessage.cs` requires a `ContextName`;
  `LivePreviewChannels.ReleaseContext` carries that message; `VixenModules.App.LivePreview.Module` subscribes and delegates
  to `ILivePreviewService.ReleaseContext`. `TurnOffElementsMessage` now exposes `ElementIds`.

- Observation: The Milestone 1 baseline is green before replacing the selected-row spike.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State --no-restore` passes
  `31` of `31` tests, and `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` completes successfully.

- Observation: The baseline reports a known critical-severity `LiteDB` advisory and unrelated existing compiler warnings.
  Evidence: Restore reports `NU1904` for `LiteDB` `4.1.4` with advisory `GHSA-3x49-g6rc-c284`. The focused State test run
  also reports existing compiler warnings in `Vixen.Core`, `FixtureGraphics`, and `WpfPropertyGrid`.

## Decision Log

- Decision: Treat the existing uncommitted preview code as a spike and refactor it rather than layering group behavior onto
  its direct `SelectedItem` publications.
  Rationale: The final behavior requires toggle gating, pair-based state tracking, overlap repair, clear-context lifecycle
  boundaries, and deterministic unit tests. Direct row-selection publications do not represent that state machine.
  Date/Author: 2026-06-01 / Codex

- Decision: Represent active preview state as a set of `(element ID, color)` pairs.
  Rationale: Exact duplicate assignments should activate once, while the same element assigned different colors must keep
  multiple active effects.
  Date/Author: 2026-06-01 / User and Codex

- Decision: Add a focused internal `StatePreviewCoordinator` and a narrow `IStatePreviewPublisher` boundary.
  Rationale: The coordinator owns dialog-local active pairs and the asymmetric turn-off/repair algorithm. The publisher
  adapter is the only type that knows about static `Broadcast.Publish`, which keeps the coordinator and mapper tests
  deterministic and prevents WPF dispatcher requirements from leaking into unit tests.
  Date/Author: 2026-06-01 / Codex

- Decision: Keep WPF control state in `StateMapperViewModel` and do not persist it in `StateData` or `StateItemData`.
  Rationale: Preview toggle state, Preview Mode, selected group, and active pairs describe one setup-dialog session rather
  than the configured State property.
  Date/Author: 2026-06-01 / User and Codex

- Decision: Use `bool IsPreviewEnabled`, `bool IsStateItemGroupPreviewMode`, `ObservableCollection<string>
  AvailableStateItemGroups`, and `string SelectedStateItemGroup` as mapper properties instead of adding a persisted enum.
  Rationale: These properties bind directly to the toggle, radio buttons, visibility, enabled state, and combo box. A
  boolean mode is sufficient because Phase 3 has exactly two modes and avoids adding a public enum solely for WPF binding.
  Date/Author: 2026-06-01 / Codex

- Decision: Use Catel `OnClosedAsync(bool?)` for final cleanup.
  Rationale: It covers OK, Cancel, and the window close button in one ViewModel lifecycle hook. It avoids view code-behind
  event handlers and keeps cleanup idempotent.
  Date/Author: 2026-06-01 / Codex

- Decision: Clear active effects in the `"State Preview"` context when Preview turns off and when mode changes while Preview
  is on. Release the named context when the mapper closes. Use incremental pair diffs for ordinary edits.
  Rationale: Mode changes and Preview-off transitions keep the dialog open and may reuse the named context. Dialog close is
  the ownership boundary and should release the context entirely now that `ReleaseContextMessage` exists.
  Date/Author: 2026-06-02 / User and Codex

- Decision: Publish Live Preview operations only through the typed declarations in `LivePreviewChannels`.
  Rationale: `BroadcastChannel<TMessage>` binds the channel name to its message type at the declaration site and prevents
  mismatched publish calls. `Common.Messages` owns channel declarations long term, so `BroadcastChannel<TMessage>` is a
  sealed reference type with an internal constructor that requires a non-whitespace name. Consumer modules cannot construct
  ad hoc or unnamed typed channels.
  Date/Author: 2026-06-02 / Codex

- Decision: When removing any active pair for one element ID, turn off that ID once and then reactivate every desired pair
  that remains for the ID.
  Rationale: `TurnOffElementsMessage` terminates by element ID and cannot remove only one color. Repair activation preserves
  remaining colors.
  Date/Author: 2026-06-01 / User and Codex

- Decision: Keep the State module independent of `Vixen.Modules.App.LivePreview`.
  Rationale: The existing universal Live Preview architecture intentionally exposes logic-free message DTOs and broadcast
  channels so feature modules publish requests without depending on the Live Preview app module.
  Date/Author: 2026-06-01 / Codex

- Decision: Do not change asynchronous behavior beyond Catel lifecycle integration.
  Rationale: Preview diff computation and publication are synchronous, in-memory UI operations. Existing Catel commands
  correctly return `Task` or `Task<T>`. Do not introduce `async void`, `.Wait()`, `.Result`, or background scheduling.
  Date/Author: 2026-06-01 / Codex

## Outcomes & Retrospective

Planning and the Milestone 1 baseline gate are complete. Implementation code has not started. The plan intentionally
separates the Live Preview diff algorithm from the Catel ViewModel so overlap behavior can be proved with focused xUnit
tests before the WPF surface is changed.

## Context and Orientation

Vixen is a .NET 10 WPF desktop application for animated light shows. A display **element node** is a configured prop, group,
or leaf light in the display hierarchy. A **leaf element** is a terminal node that can be lit. A selected group assignment
implicitly includes all descendant leaf elements.

The State property is a Vixen property module under `src/Vixen.Modules/Property/State/`. Its setup dialog edits a cloned
draft and copies the draft back only when the user saves. Preview controls added by this plan are temporary and must never
be copied into persisted State data.

The key State files are:

- `src/Vixen.Modules/Property/State/State.csproj` contains project dependencies. The current working tree already adds
  project references to `src/Vixen.Common/Broadcast/Broadcast.csproj` and
  `src/Vixen.Common/Messages/Messages.csproj`. Preserve those references, remove the accidental UTF-8 byte-order mark if it
  remains in the first line, and add `InternalsVisibleTo` only if internal coordinator tests require it.
- `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs` owns the dialog draft, grid rows, selected row,
  validation, commands, and color-picker service. It currently contains the preview spike to replace.
- `src/Vixen.Modules/Property/State/Setup/ViewModels/StateItemViewModel.cs` wraps one grid row. It exposes `Name`, `Color`,
  `AssignmentRoots`, and computed `AssignmentCount`. It updates `ElementNodeIds` and raises `AssignmentCount` when its
  assignment tree changes.
- `src/Vixen.Modules/Property/State/Setup/Models/StateAssignmentTreeNode.cs` represents one checkbox node in the assignment
  tree. Its `GetEffectiveLeafNodeIds()` method expands checked groups into descendant leaf IDs.
- `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml` defines the mapper window, State item grid, assignment
  tree, validation summary, and bottom OK/Cancel buttons.
- `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml.cs` initializes the view and sets its icon and
  `DataContext`. Do not add close-event logic here unless Catel lifecycle validation proves that `OnClosedAsync(bool?)`
  cannot cover window-close cleanup.

Live Preview integration is intentionally message-based:

- `src/Vixen.Common/Messages/LivePreview/ElementState.cs` is a logic-light message payload containing `Id`, `Duration`,
  `Color`, and `Intensity`.
- `src/Vixen.Common/Messages/LivePreview/TurnOnElementsMessage.cs`,
  `src/Vixen.Common/Messages/LivePreview/TurnOffElementsMessage.cs`, and
  `src/Vixen.Common/Messages/LivePreview/ClearActiveEffectsMessage.cs` wrap preview operations and an optional context name.
  `TurnOffElementsMessage` carries element IDs directly because turn-off operates by ID rather than by color.
- `src/Vixen.Common/Messages/LivePreview/ReleaseContextMessage.cs` requests release of a named Live Preview context.
- `src/Vixen.Common/Messages/BroadcastChannel.cs` associates a channel name with its message type. Consumers use a declared
  `BroadcastChannel<TMessage>` value so the compiler rejects mismatched messages.
- `src/Vixen.Common/Messages/LivePreview/LivePreviewChannels.cs` provides the typed Live Preview channel declarations.
- `src/Vixen.Common/Broadcast/Broadcast.cs` publishes messages through a static weak-reference messenger.
- `src/Vixen.Modules/App/LivePreview/LivePreviewService.cs` receives operations. Turn-on creates one effect per
  `ElementState`. Turn-off terminates by element ID. Clear removes all effects from one named context without releasing it.
  Release removes the named context from the registry and asks the context factory to release it.

In this plan, a **preview pair** is a value containing one effective leaf element ID and one hex color. The same ID and color
appearing more than once is one pair. The same ID with two different colors is two pairs. A **desired pair set** is computed
from the rows selected by Preview Mode. The coordinator compares desired pairs with its current active pair set and publishes
the smallest correct update.

Existing tests live in `src/Vixen.Tests/Property/State/`. Mapper tests use
`[Collection(StateMapperTestCollection.Name)]` so Catel ViewModels are not constructed concurrently. Follow that convention
for new mapper tests.

## Skills Design Review

The requested project skills shape the implementation as follows.

The `dotnet-design-pattern-review` skill favors clear boundaries, SOLID responsibilities, and testability. The State mapper
should remain an orchestration ViewModel. The preview diff algorithm belongs in an internal coordinator, while static
broadcast publication belongs in a narrow adapter. Repository, provider, and factory patterns are not applicable because
this phase has no persistence, external service, or complex object-creation boundary. Do not add them.

The `dotnet-best-practices` skill requires focused types, constructor null checks, one type per file, WPF bindings rather than
new UI code-behind, collection expressions where appropriate, and xUnit AAA tests. Use an internal sealed class for each
implementation type that is not intended for inheritance. Use a `readonly record struct` for the immutable preview pair.

The `csharp-async` skill requires task-based lifecycle methods and prohibits blocking waits. The coordinator and publisher
remain synchronous because they perform in-memory diffing and same-thread message publication. Override Catel
`OnClosedAsync(bool?)`, clear preview before awaiting the base hook, then await the base task normally because this is a WPF
ViewModel path that returns to the UI context. Do not use `ConfigureAwait(false)` in the ViewModel lifecycle hook.

The `csharp-docs` skill requires XML comments when adding or modifying public or protected APIs. The mapper's new public
bindable properties and the protected `OnClosedAsync(bool?)` override must be documented. Internal coordinator members should
also receive succinct XML comments where their intent is not obvious. Do not leave stale docs on modified APIs.

The `catel-mvvm` skill also applies because this is WPF ViewModel work. Keep the mapper derived from `ViewModelBase`, define
bound values with Catel `RegisterProperty<T>`, use existing command binding, and keep view code-behind limited to visual
initialization.

## Interfaces and Dependencies

Add the following internal files under `src/Vixen.Modules/Property/State/Setup/Preview/`. Keep one type per file.

In `StatePreviewPair.cs`, define an immutable pair:

    internal readonly record struct StatePreviewPair(Guid ElementId, string Color);

The pair's default record-struct equality is the deduplication rule: equal IDs and equal ordinal hex-color strings collapse;
the same ID with a different color remains distinct. Colors originate from `ToHex()` and therefore use a consistent format.

In `IStatePreviewPublisher.cs`, define a narrow publication contract:

    internal interface IStatePreviewPublisher
    {
        void TurnOn(IReadOnlyCollection<StatePreviewPair> pairs);
        void TurnOff(IReadOnlyCollection<Guid> elementIds);
        void Clear();
        void Release();
    }

In `BroadcastStatePreviewPublisher.cs`, define an internal sealed adapter. Add a constant:

    internal const string ContextName = "State Preview";

`TurnOn` publishes `TurnOnElementsMessage` only when at least one pair exists. Convert each pair into `ElementState` with
`Id = pair.ElementId`, `Color = pair.Color`, `Intensity = 100`, and `Duration = int.MaxValue`. `TurnOff` publishes
`TurnOffElementsMessage` only when at least one ID exists. Deduplicate IDs and assign the resulting IDs to
`TurnOffElementsMessage.ElementIds`. `Clear` publishes `ClearActiveEffectsMessage` with the same context name. `Release`
publishes `ReleaseContextMessage` with the same context name. Publish every message through the corresponding typed
declaration: `LivePreviewChannels.TurnOnElements`, `LivePreviewChannels.TurnOffElements`,
`LivePreviewChannels.ClearActiveEffects`, or `LivePreviewChannels.ReleaseContext`. Consumer modules cannot construct
`BroadcastChannel<TMessage>` because its named constructor is internal to `Common.Messages`; do not pass raw string channel
names. This adapter is the only new State type that calls `Broadcast.Publish`.

In `StatePreviewCoordinator.cs`, define an internal sealed coordinator with constructor injection and a null check:

    internal sealed class StatePreviewCoordinator(IStatePreviewPublisher publisher)
    {
        public void Refresh(IEnumerable<StatePreviewPair> desiredPairs);
        public void Clear();
        public void Release();
    }

The actual visibility of coordinator methods may remain internal if convenient for testing through `InternalsVisibleTo`.
Keep an internal `HashSet<StatePreviewPair>` of active pairs. `Refresh` materializes and deduplicates the desired set. Compute
IDs that need termination from active pairs missing in the desired set. If any ID needs termination, publish one turn-off
operation for the distinct IDs, remove every active pair for those IDs, and reactivate every desired pair for those IDs.
Then publish desired additions that are not active. Publish turn-off before turn-on. Update the active set only in the same
order as successful publisher calls. If there is nothing to publish, return without a message. `Clear` always publishes a
clear message and empties the active set. `Release` always publishes a release message and empties the active set. Both
operations are intentionally safe to repeat, but use `Clear` while the dialog remains open and `Release` when the dialog
closes.

The mapper remains responsible for computing desired pairs because it owns selection semantics and rows. Add temporary Catel
properties to `StateMapperViewModel`:

    public bool IsPreviewEnabled { get; set; }
    public bool IsStateItemGroupPreviewMode { get; set; }
    public ObservableCollection<string> AvailableStateItemGroups { get; }
    public string SelectedStateItemGroup { get; set; }

Use `"<ALL>"` as a mapper constant. `IsPreviewEnabled` defaults to `false`.
`IsStateItemGroupPreviewMode` defaults to `false`, which means Selected State Item mode.
`SelectedStateItemGroup` defaults to `"<ALL>"`.
`AvailableStateItemGroups` always starts with `"<ALL>"`.

Add an internal constructor overload to `StateMapperViewModel` that accepts `IStatePreviewPublisher` or
`StatePreviewCoordinator`, while keeping the existing public constructor intact for production callers. The public
constructor delegates to the internal overload with `new BroadcastStatePreviewPublisher()`. This keeps
`StateModule.SetupElements()` unchanged and lets tests inject a recording publisher. If internal types are referenced
directly from `Vixen.Tests`, add:

    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleToAttribute">
        <_Parameter1>Vixen.Tests</_Parameter1>
    </AssemblyAttribute>

to `src/Vixen.Modules/Property/State/State.csproj`.

Use the existing `Broadcast` and `Messages` project references already present in the working tree. Do not reference
`src/Vixen.Modules/App/LivePreview/LivePreview.csproj` from the State project.

## Plan of Work

### Milestone 1: Record Phase 3 in Jira and establish the baseline

Before editing implementation files, paste the Jira append block from this plan into issue `VIX-3591`. The user requested a
paste-ready block rather than an automated Jira update. Record confirmation in `Progress`; do not claim the Jira update is
complete until the user confirms it was pasted.

Run the focused State tests and a full Debug rebuild from `C:\Dev\Vixen`. Record the exact pass count and any pre-existing
warnings in `Surprises & Discoveries`. The working tree already contains uncommitted Phase 3 spike files and this plan. Do
not revert them. Review them before every edit and refactor them in place.

Acceptance for this milestone is a Jira confirmation, a recorded State test baseline, and a recorded Debug build baseline.

### Milestone 2: Extract and prove pair-based preview coordination

Create the internal types under `src/Vixen.Modules/Property/State/Setup/Preview/`. Refactor the spike so static broadcast
publication moves into `BroadcastStatePreviewPublisher`. Keep the message DTO project logic-free.

Add `src/Vixen.Tests/Property/State/StatePreviewCoordinatorTests.cs`. Build a small recording implementation of
`IStatePreviewPublisher` in its own test file if it grows beyond a trivial nested fixture. Tests must use Arrange, Act,
Assert structure and verify operation order as well as payloads.

Cover these coordinator cases:

- An empty desired set produces no publication.
- New pairs produce one turn-on publication.
- Exact duplicate pairs collapse.
- One element ID with two different colors preserves both pairs.
- Removing one pair turns off the ID and reactivates desired colors still assigned to that ID.
- A refresh containing removals and additions publishes turn-off before turn-on.
- Turn-off IDs are deduplicated.
- Repeating an already active desired set produces no publication.
- `Clear()` publishes clear and resets active state.
- Calling `Clear()` twice publishes twice because Preview-off and mode-change cleanup are intentionally repeatable.
- `Release()` publishes context release and resets active state.
- Calling `Release()` twice publishes twice because dialog-close release is intentionally repeatable and the Live Preview
  service treats release of a missing context as a no-op.

Add `src/Vixen.Tests/Property/State/BroadcastStatePreviewPublisherTests.cs` if the adapter can be tested cleanly by
subscribing temporary recipients through `Broadcast.Subscribe`. If the static WPF dispatcher boundary makes those tests
fragile outside an application dispatcher, test mapper/coordinator output through the injected recording publisher and
record that limitation in `Surprises & Discoveries`. At minimum, review the adapter directly and prove message DTO payload
construction through a narrow testable conversion method or recording boundary.

Acceptance for this milestone is a passing coordinator fixture proving deduplication, multi-color activation, asymmetric
turn-off repair, clear behavior, and publication order.

### Milestone 3: Add mapper state, groups, and incremental refresh triggers

Refactor `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs`.

Remove the spike's `TurnOnPreview(StateItemViewModel?)` and `TurnOffPreview(StateItemViewModel?)` methods. Add the injected
coordinator and temporary preview properties described above. Keep all preview publication gated by `IsPreviewEnabled`,
except explicit cleanup calls.

Add a focused method that rebuilds `AvailableStateItemGroups`. It always starts with `"<ALL>"`, then adds non-empty row names
in first-grid-appearance order using `StringComparer.Ordinal`. It retains the selected named group when an exact match still
exists. If the selected named group disappears, set `SelectedStateItemGroup` to `"<ALL>"`. Avoid publishing twice when a
property setter invoked during rebuilding would otherwise trigger refresh; use a small private suppression flag or a
single orchestration method.

Add a method that selects active rows:

- In Selected State Item mode, return the one `SelectedItem`, or no rows when none is selected.
- In State Item Group mode with `"<ALL>"`, return every row.
- In State Item Group mode with a named selection, return every row whose trimmed name equals the group name with ordinal,
  case-sensitive comparison.

Add a method that computes desired pairs by projecting each active row's `AssignmentRoots` through
`GetEffectiveLeafNodeIds()`, combining each leaf ID with `row.Color.ToHex()`, and materializing distinct
`StatePreviewPair` values. Hand that set to `StatePreviewCoordinator.Refresh`.

Update mapper event handling:

- `IsPreviewEnabled = true` refreshes the current mode immediately.
- `IsPreviewEnabled = false` clears the coordinator immediately.
- Changing `IsStateItemGroupPreviewMode` while Preview is on clears the coordinator, then recomputes and activates the
  desired set. When switching into group mode for the first time or when no valid group remains, select `"<ALL>"`.
- Changing `SelectedStateItemGroup` while Preview is on refreshes incrementally.
- Changing `SelectedItem` keeps assignment-tree expansion and remove-command refresh behavior. It refreshes preview only in
  Selected State Item mode and only when Preview is on. Grid selection does not affect group-mode preview.
- `ItemsCollectionChanged` continues attach/detach handling and validation, rebuilds group choices, and refreshes only when
  the active selected set can change. `<ALL>` refreshes for every row add or removal. A named group refreshes when an affected
  row belonged or belongs to that exact group, or when fallback to `"<ALL>"` occurs. Selected State Item mode refreshes when
  selection behavior changes the active row.
- `ItemPropertyChanged` continues validation refresh for `Name`. For `Name`, rebuild group choices and refresh under the
  named-group rules above. For `Color` and `AssignmentCount`, refresh only when the changed row belongs to the current
  preview selected set. Do not publish for inactive-row edits.

Take care with collection order. Add/remove operations currently select a new or fallback row. Consolidate refresh calls so
each user action publishes one logically minimal update rather than intermediate turn-off and turn-on sequences caused by
property setters firing during orchestration.

Override Catel `OnClosedAsync(bool? result)` in `StateMapperViewModel`. Always call coordinator `Release()` exactly once from
that hook, then await `base.OnClosedAsync(result)`. This hook must remain the one dialog-close release location so OK,
Cancel, and window close cannot diverge. Preview-off and mode-change cleanup continue to call `Clear()` because the dialog
remains open and may reactivate the same named context.

Add mapper tests under `src/Vixen.Tests/Property/State/StateMapperPreviewTests.cs` and place the fixture in
`StateMapperTestCollection`. Use a recording publisher injected through the internal constructor. Cover:

- Preview defaults off and produces no constructor-time messages.
- Selected State Item mode is the default.
- Group choices default to only `"<ALL>"` when there are no rows.
- Group choices deduplicate exact names, preserve case-different names, and use first-grid-appearance order.
- Turning Preview on previews the selected row immediately.
- Turning Preview on with no rows emits no turn-on operation.
- Turning Preview off clears.
- Selecting another row refreshes Selected State Item mode.
- Selecting another row does not publish in State Item Group mode.
- Mode changes while Preview is on clear before activating the new desired pairs.
- Assignment and color edits refresh active rows but do not publish for inactive rows.
- Named-group selection previews exact-name matches.
- `"<ALL>"` refreshes after add, remove, and rename.
- A selected named group remains selected when another exact-name row remains after rename or removal.
- Losing the final matching row falls back to `"<ALL>"` and refreshes all remaining rows.
- Multi-color overlap behavior passes through the mapper into coordinator repair correctly.
- Preview off suppresses edits, row selections, mode effects, and group effects.
- Invoking the close lifecycle hook releases the named context even when Preview is already off.

Acceptance for this milestone is a passing mapper preview fixture plus the pre-existing State fixtures.

### Milestone 4: Add the WPF Preview controls

Update `src/Vixen.Modules/Property/State/Setup/Views/StateMapperView.xaml`. Keep XAML binding-only; do not add event handlers
to `StateMapperView.xaml.cs`.

Add a preview-controls section above the State item grid. Increase the mapper's left-panel row definitions as needed. The
controls must present:

- a `TextBlock` label with text `Preview`;
- a WPF `ToggleButton` bound two-way to `IsPreviewEnabled`, with content `Off` while unchecked and `On` while checked;
- a `Preview Mode` label;
- a `Selected State Item` radio button;
- a `State Item Group` radio button;
- a combo box bound to `AvailableStateItemGroups` and `SelectedStateItemGroup`.

Bind both mode radio buttons and the combo box `IsEnabled` value to `IsPreviewEnabled`. Bind the State Item Group radio
button to `IsStateItemGroupPreviewMode`. Bind the Selected State Item radio button using an inverse binding converter or
XAML triggers so exactly one mode is selected without duplicating mode state in the ViewModel. Use the existing merged
`WPFCommon` theme and its boolean visibility converter. Show the combo box only in State Item Group mode and keep it disabled
while Preview is off.

For the toggle's `On` / `Off` content, use a local style with a default `Content="Off"` setter and an
`IsChecked=True` trigger that sets `Content="On"`. Do not add a converter for this two-value presentation.

Build the State project after the XAML edit. Then launch Vixen and inspect layout at the default window size and minimum
window size. Preview controls must remain readable without obscuring the item grid or assignment tree.

Acceptance for this milestone is a compiling XAML surface with toggle defaults, disabled mode controls while off, group-combo
visibility only in group mode, and no new view code-behind behavior.

### Milestone 5: Verify the feature end to end

Run the focused State suite, full Debug rebuild, full Release rebuild, `git diff --check`, and explicit CRLF checks for
touched text files. Start Vixen from the Debug output and complete the manual scenarios under `Validation and Acceptance`.

Update this plan's living sections with exact command results, unexpected behavior, implementation decisions, and final
outcomes. Add a revision note at the bottom.

Acceptance for this milestone is a passing automated suite, successful Debug and Release rebuilds, clean whitespace checks,
and manual evidence that previews update and clear correctly.

## Jira Update Markdown

Before implementation, append the following markdown to Jira issue `VIX-3591`. Keep the prior Phase 1 and Phase 2 history
intact. The user will paste this block manually.

    ### Phase 3: State Property Live Preview Updates

    #### Goal

    Enhance State Property Setup so users can explicitly enable Live Preview, preview either one selected State item or a
    named State Item Group, and immediately see assignment and color edits reflected in the Live Preview.

    #### Requirements

    - Add a `Preview` WPF toggle button that displays `Off` or `On`. Preview defaults to `Off`.
    - Disable Preview Mode controls while Preview is off.
    - Add Preview Mode options `Selected State Item` and `State Item Group`. Default to `Selected State Item`.
    - Add a State Item Group combo box containing `<ALL>` plus distinct, case-sensitive State item names in
      first-grid-appearance order. Default to `<ALL>`.
    - Preview all exact-name matches for a selected named group. Preview all rows for `<ALL>`.
    - Immediately refresh preview state after applicable row selections, group selections, assignment changes, color
      changes, additions, removals, and renames.
    - Deduplicate active preview states by `(element ID, color)`. Preserve multiple colors assigned to the same element.
    - Use incremental turn-off and turn-on Live Preview messages for ordinary edits.
    - Clear active effects in the isolated `"State Preview"` context when Preview turns off or Preview Mode changes.
    - Release the named `"State Preview"` context when the dialog closes through OK, Cancel, or window close.
    - Keep preview state temporary. Do not persist preview controls or active preview states.

    #### High-Level Design

    Maintain dialog-local active preview state as `(element ID, color)` pairs. Compute desired pairs from the effective leaf
    elements assigned to the State items selected by the current Preview Mode. Diff desired pairs against active pairs for
    ordinary refreshes.

    Live Preview turn-off operates by element ID rather than color. If one color is removed from a multi-color element,
    first turn off that element ID and then reactivate any desired colors that remain for it.

    Publish `TurnOnElementsMessage`, `TurnOffElementsMessage`, `ClearActiveEffectsMessage`, and `ReleaseContextMessage`
    through `Common.Broadcast` using the typed `LivePreviewChannels` declarations and the `"State Preview"` context name.
    Keep the State Property module independent of the Live Preview app module.

    #### Acceptance Criteria

    - Preview is off by default and no State Preview lights activate until the user turns it on.
    - Selected State Item mode previews only the selected grid row.
    - State Item Group mode previews exact-name matches or every row when `<ALL>` is selected.
    - Case-different names remain separate groups.
    - Checking and unchecking assigned elements updates active preview leaves immediately.
    - Changing an active State item's color updates its preview immediately.
    - The same element can preview multiple colors while exact duplicate element-and-color pairs are activated only once.
    - Turning Preview off clears all `"State Preview"` effects.
    - Closing the dialog releases the named `"State Preview"` context.

    #### Testing

    - Add automated tests for toggle defaults, mode behavior, group-list behavior, incremental additions and removals,
      assignment updates, color updates, overlap handling, empty updates, message payloads, and cleanup paths.
    - Run the existing State Property test suite to protect draft editing, validation, assignment trees, stable IDs,
      cloning, copying, and xLights import behavior.
    - Perform manual acceptance checks for selected-row preview, group preview, overlapping colors, toggle cleanup, and
      dialog close cleanup.

## Concrete Steps

Run commands from `C:\Dev\Vixen` in PowerShell unless a step states otherwise.

Before implementation, inspect the dirty working tree:

    git status --short
    git diff -- src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs src/Vixen.Modules/Property/State/State.csproj

Expected initial relevant output includes modified `StateMapperViewModel.cs`, modified `State.csproj`, the Phase 3
specification, and this plan. Do not discard uncommitted work.

Establish the baseline:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

After Milestone 2, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter StatePreviewCoordinator

After Milestone 3, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State

After Milestone 4, compile the State project and then the solution:

    dotnet build src\Vixen.Modules\Property\State\State.csproj --configuration Debug
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

For final verification, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --configuration Debug --filter State --no-restore
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
    git diff --check

Run an explicit CRLF check for each touched text file. PowerShell should report no files with bare line-feed endings:

    $files = @(
        'docs\plans\vix-3591-state-property-preview-phase-3.md',
        'src\Vixen.Modules\Property\State\State.csproj',
        'src\Vixen.Modules\Property\State\Setup\Preview\StatePreviewPair.cs',
        'src\Vixen.Modules\Property\State\Setup\Preview\IStatePreviewPublisher.cs',
        'src\Vixen.Modules\Property\State\Setup\Preview\BroadcastStatePreviewPublisher.cs',
        'src\Vixen.Modules\Property\State\Setup\Preview\StatePreviewCoordinator.cs',
        'src\Vixen.Modules\Property\State\Setup\ViewModels\StateMapperViewModel.cs',
        'src\Vixen.Modules\Property\State\Setup\Views\StateMapperView.xaml',
        'src\Vixen.Tests\Property\State\StatePreviewCoordinatorTests.cs',
        'src\Vixen.Tests\Property\State\StateMapperPreviewTests.cs'
    )
    foreach ($file in $files) {
        if (Test-Path $file) {
            $text = [System.IO.File]::ReadAllText((Resolve-Path $file))
            if ($text -match "(?<!`r)`n") { Write-Output "Bare LF: $file" }
        }
    }

Start the Debug application using the normal repository build output and exercise Display Setup manually. If the Debug
rebuild places the executable at a different exact path than expected, inspect `Debug\Output` and record the executable path
in this plan before launching it.

## Validation and Acceptance

Automated acceptance requires all focused State tests to pass, including coordinator, mapper preview, existing draft,
validation, assignment-tree, identity, clone, copy, and xLights-import tests. Debug and Release rebuilds must complete
successfully. `git diff --check` and the CRLF check must be clean.

Perform these manual scenarios in Display Setup.

For Preview toggle and selected-row behavior:

1. Open State Property Setup for an element with two configured State items and assigned leaves.
2. Confirm the Preview toggle displays `Off`, Preview Mode defaults to `Selected State Item`, and mode controls are disabled.
3. Confirm no State Preview lights are active.
4. Turn Preview on and confirm the toggle displays `On`, mode controls become enabled, and selected-row leaves turn on using
   that row's color.
5. Select another row and confirm the prior preview is replaced by the new row's effective leaves.
6. Turn Preview off and confirm all State Preview lights clear immediately.

For assignment and color editing:

1. Turn Preview on in Selected State Item mode.
2. Check an unassigned leaf or group node and confirm effective leaves turn on.
3. Uncheck it and confirm effective leaves turn off.
4. Change the selected row color and confirm preview updates only after the picker commits the new color.
5. Edit an inactive row and confirm visible preview does not change.

For State Item Group behavior:

1. Configure rows named `Open`, `Open`, and `open`.
2. Turn Preview on, select `State Item Group`, and confirm the combo box defaults to `<ALL>`.
3. Select `Open` and confirm both `Open` rows preview while `open` does not.
4. Change grid selection and confirm group preview remains unchanged.
5. Rename one `Open` row and confirm the remaining `Open` row keeps the selected group active.
6. Rename the final `Open` row and confirm fallback to `<ALL>` previews all remaining rows.

For overlapping colors:

1. Configure two active rows that assign the same leaf with the same color and confirm only one activation for that pair.
2. Change one row to a different color and confirm both colors remain active for the leaf.
3. Remove one assignment and confirm the remaining color is restored after the leaf ID is terminated.

For dialog cleanup:

1. Turn Preview on and activate leaves.
2. Close with Cancel and confirm all State Preview lights turn off because the named context is released.
3. Repeat with OK.
4. Repeat with the window close button.

## Idempotence and Recovery

The coordinator's `Refresh` operation is idempotent for an unchanged desired set: it publishes nothing. `Clear` is
intentionally idempotent in effect but not silent: every explicit in-dialog cleanup request publishes
`ClearActiveEffectsMessage`. `Release` is also intentionally repeatable: every dialog-close request publishes
`ReleaseContextMessage`, and Live Preview treats release of a missing context as a no-op. These rules protect against stale
lights when prior local state is incomplete.

All edits are source changes and can be applied incrementally. The repository may already be dirty. Never revert user edits
or the existing spike wholesale. Before changing `StateMapperViewModel.cs` or `State.csproj`, read the current file and work
with its contents. If a half-completed refactor fails to compile, finish or repair the additive types and rerun the focused
tests. Do not use destructive Git commands.

If Catel `OnClosedAsync(bool?)` does not execute for the window close button during manual testing, record the evidence in
`Surprises & Discoveries`. Use the smallest ViewModel-centered fallback available. Only add a XAML behavior or view
code-behind close hook if Catel cannot expose the lifecycle event, and document why the exception to the no-code-behind rule
is necessary.

If static broadcast adapter tests require a WPF application dispatcher, do not weaken production code to satisfy the test.
Keep algorithm tests behind `IStatePreviewPublisher`, test deterministic conversion where possible, and document the
dispatcher limitation.

## Artifacts and Notes

The correct incremental removal pattern is:

    Active:  (LeafA, #FF0000), (LeafA, #00FF00)
    Desired: (LeafA, #00FF00)

    Publish TurnOffElements: LeafA
    Publish TurnOnElements:  (LeafA, #00FF00)

Do not attempt to publish a color-specific turn-off message. The Live Preview service terminates by ID.

The correct mode-change pattern while Preview is on is:

    Publish ClearActiveEffectsMessage for "State Preview"
    Reset dialog-local active pairs
    Compute desired pairs for the new mode
    Publish TurnOnElementsMessage only when desired pairs are non-empty

The correct dialog-close pattern is:

    Publish ReleaseContextMessage for "State Preview"
    Reset dialog-local active pairs

Publish messages through the declared typed channels:

    Broadcast.Publish(LivePreviewChannels.TurnOnElements, message)
    Broadcast.Publish(LivePreviewChannels.TurnOffElements, message)
    Broadcast.Publish(LivePreviewChannels.ClearActiveEffects, message)
    Broadcast.Publish(LivePreviewChannels.ReleaseContext, message)

Do not attempt to construct `BroadcastChannel<TMessage>` at the call site. Its constructor is intentionally internal to
`Common.Messages`, which owns broadcast declarations. Always use the declared Live Preview channel.

The production State project must depend on:

    Vixen.Common\Broadcast\Broadcast.csproj
    Vixen.Common\Messages\Messages.csproj

It must not depend on:

    Vixen.Modules\App\LivePreview\LivePreview.csproj

## Risks and Concerns

The largest correctness risk is multi-color overlap. A naive diff that turns off only removed pairs is impossible because
the Live Preview message terminates by ID. The coordinator must repair all remaining colors for an affected ID after
termination.

The largest lifecycle risk is stale preview output or an unused context after the setup dialog closes. Catel
`OnClosedAsync(bool?)` is the chosen single release hook. Manual validation must prove the window close button reaches it.

The largest orchestration risk is duplicate publications caused by nested Catel setters and collection notifications.
Suppress intermediate refreshes during group-list rebuilding and multi-step add/remove operations, then refresh once after
the state is coherent.

The largest testing risk is the static broadcast dispatcher boundary. Keep it isolated in the publisher adapter and test the
coordinator with a recording publisher.

The typed-channel migration originally exposed a call-site risk because a struct channel could be default-constructed
without a name. The hardened sealed reference type with an internal named constructor removes that misuse path. Continue to
use the corresponding `LivePreviewChannels` declaration so the message type and channel remain coupled.

The project file currently contains an accidental UTF-8 byte-order mark from the spike. Remove it while preserving the two
required project references and the repository's CRLF convention.

## Revision Notes

- 2026-06-01: Created the Phase 3 ExecPlan from `docs/state-property-item-update.md`, current State mapper code, the Live
  Preview message architecture, Catel lifecycle documentation, and the requested project skills. The plan treats the
  existing uncommitted preview code as a refactorable spike and requires Jira confirmation before implementation.
- 2026-06-02: Revised the plan for typed `BroadcastChannel<TMessage>` declarations, the ID-only
  `TurnOffElementsMessage.ElementIds` contract, and `ReleaseContextMessage`. Dialog close now releases the named
  `"State Preview"` context; Preview-off and mode-change transitions continue clearing active effects while the dialog is
  open.
- 2026-06-02: Recorded the prerequisite broadcast-contract hardening. `BroadcastChannel<TMessage>` is now a sealed reference
  type with an internal constructor requiring a non-whitespace name. Typed Broadcast overloads reject null channels, and
  State spike publications use declared `LivePreviewChannels`. `Broadcast.Publish` also dispatches directly when no WPF
  application exists, which keeps non-UI execution and focused tests from dereferencing a missing dispatcher.
- 2026-06-02: Completed Milestone 1 after the user confirmed the Phase 3 block was added to `VIX-3591`. Recorded the green
  baseline of `31` focused State tests and a successful full Debug rebuild, along with the existing `LiteDB` advisory and
  unrelated compiler warnings.

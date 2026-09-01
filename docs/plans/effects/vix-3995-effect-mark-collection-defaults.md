# Add State-aware effect mark collection defaults

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain it in accordance with `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

VIX-3995 makes a State mark collection a first-class type and gives effects a dependable, sequence-local mark collection selection when their saved selection is empty or has been deleted. A user can tag a marks-docker track as `State`, select State effect mark rendering, and have the State effect choose the first State-tagged track; if none exists, it chooses the first track. Existing user choices always remain unchanged, including a valid generic fallback chosen before a State track is added later.

The result is observable in the Timed Sequence Editor: State appears in the collection-type menu but offers no parent-link menu, State exports include label text by default, and State, ordinary mark-driven effects, LipSync, Wave, and Liquid repair only invalid selections. Automated tests prove the same behavior for effect defaults and a different sequence, so persisted defaults never carry a stale collection identifier.

## Progress

- [x] (2026-09-01 17:49Z) Updated Jira issue VIX-3995 with the user-facing requirements, acceptance criteria, and regression intent; status remains New Ticket. Evidence: https://vixenlights.atlassian.net/browse/VIX-3995 (updated 2026-09-01 12:49:27.792-05:00).
- [ ] Add the `State` enum value, its XML documentation, and serialization/menu/export coverage.
- [ ] Add the documented, stateless Core selection contract and selection service.
- [ ] Move mark-collection lifecycle normalization into `BaseEffect` and migrate effect-specific lifecycle work to its template hooks.
- [ ] Apply the State, ordinary, LipSync, Wave, and Liquid policies without changing persisted State effect data.
- [ ] Add unit and integration-path regression tests; run the required Release/x64 test workflow.
- [ ] Update VIX-3995 with final acceptance and validation results; record the final outcome in this plan.

## Surprises & Discoveries

- Observation: `StateData.MarkCollectionId` already is a `Guid`, is a `[DataMember]`, and is marked `[ExcludeFromEffectDefault]`.
  Evidence: `src/Vixen.Modules/Effect/State/StateData.cs` stores `Guid.Empty` by default. The established effect-default scrubber deliberately clears sequence-specific mark identifiers, so this issue needs lifecycle repair, not a migration or a default-data exception.

- Observation: State currently tries to discover the not-yet-defined enum member with `Enum.TryParse<MarkCollectionType>("State")`; its fallback activation only assigns when a State collection is found.
  Evidence: `src/Vixen.Modules/Effect/State/State.cs`, `GetFirstStateMarkCollection` and the `RenderSource` setter.

- Observation: Wave and Liquid retain child selection IDs around a name-list refresh, but their current update routines do not choose defaults for empty child IDs.
  Evidence: `Wave.UpdateMarkCollectionNames` and `Liquid.UpdateMarkCollectionNames` update child display names; Waveform and Emitter data preserve `MarkCollectionId` separately.

## Decision Log

- Decision: Append `State = 4` to `MarkCollectionType`; explicitly assign all existing numeric values `Generic = 0`, `Phrase = 1`, `Word = 2`, and `Phoneme = 3`.
  Rationale: Serialized enum values must retain their established meaning. Appending preserves every existing stored value.
  Date/Author: 2026-09-01 / Codex, from the approved VIX-3995 handoff.

- Decision: Treat an existing selection as valid solely when its non-empty `Guid` occurs in the current sequence's collection list. Do not require its collection type to match a policy.
  Rationale: A user-selected generic, word, or any other valid collection is intentional and must not be replaced because a preferred State or Phoneme collection exists.
  Date/Author: 2026-09-01 / Codex, from the authoritative clarification.

- Decision: Put the reusable selection policy in `Vixen.Core/Marks` as a stateless service, and make `BaseEffect` own when normalization runs.
  Rationale: Collection type and IDs are Core concepts; policy selection must be unit-testable without WPF or a dispatcher. `BaseEffect` is the common base already responsible for mark listeners, while individual effects retain only their active-slot declaration and listener/display-name details.
  Date/Author: 2026-09-01 / Codex, informed by the repository's .NET design and documentation guidance.

- Decision: Normalize only on mark-collection assignment and collection change notifications, plus the relevant effect-data/default application lifecycle. Never normalize during `_Render`, `RenderEffect`, or per-frame mark queries.
  Rationale: This keeps work at O(active selection slots × collections), avoids render-loop healing, and permits reading the shared `ObservableCollection` without mutating it.
  Date/Author: 2026-09-01 / Codex, from the approved performance/threading requirements.

- Decision: Retain the existing LipSync rule: choose the first Phoneme collection when an active LipSync selector is empty or stale, otherwise choose `null`/`Guid.Empty` if there is no Phoneme collection. Do not fall back to Generic.
  Rationale: This is intentional existing behavior and differs from ordinary and State policies.
  Date/Author: 2026-09-01 / Codex, from the approved regression requirements.

- Decision: Do not alter `StateData.MarkCollectionId`, remove `[ExcludeFromEffectDefault]`, add a State-data migration, or rewrite shared collection metadata such as names, types, order, links, tags, marks, or visibility.
  Rationale: A collection reference is sequence-local. The collection's own persisted `CollectionType` already carries the new enum value; repair is local to each effect instance.
  Date/Author: 2026-09-01 / Codex, from the approved data and isolation requirements.

## Outcomes & Retrospective

Not started. On completion, replace this paragraph with the implemented behavior, the exact test/build results, any deviations cross-referenced to the Decision Log, and remaining risks. Add a dated note at the end of this plan for every revision and why it was made.

## Context and Orientation

A mark collection is one named, ordered timeline track in a sequence. It has an immutable-in-practice identity (`IMarkCollection.Id`, a `Guid`) and a `CollectionType`. Effect data stores only the `Guid`; display-name properties and type converters exist for the editor UI but must never be used to commit a default selection because names can be duplicated or renamed.

`src/Vixen.Core/Marks/IMarkType.cs` currently declares `MarkCollectionType` without explicit numeric values. `src/Vixen.Core/Marks/IMarkCollection.cs` exposes a collection's `Id`, `CollectionType`, marks, ordering, and links. `src/Vixen.Modules/App/Marks/MarkCollection.cs` is the concrete sequence-owned implementation whose persisted collection type will naturally serialize the new enum member.

`src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` receives the sequence's shared `ObservableCollection<IMarkCollection>` and raises `MarkCollectionsChanged`, `MarkCollectionsAdded`, and `MarkCollectionsRemoved`. `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` supplies the common mark-listener helpers and is the intended Template Method owner for this ticket. A Template Method is a base-class-controlled lifecycle sequence that calls small derived-class hooks; it prevents each effect from reimplementing selection validity and fallback rules inconsistently.

The State effect is `src/Vixen.Modules/Effect/State/State.cs`; it is active for collection normalization only when `RenderSource == StateRenderSource.MarkCollection`. Its data remains in `StateData.cs`. LipSync is active only in `LipSyncMode.MarkCollection` and has an intentional Phoneme-only fallback. Wave owns multiple `IWaveform` children and Liquid owns multiple `IEmitter` children, each with its own `Guid MarkCollectionId`; normalize each active child separately. CountDown has no mark collection selector and is explicitly out of scope.

The marks docker uses Catel view models. `MarkCollectionViewModel.SetupCollectionTypeCheckboxes` enumerates `Enum.GetValues`, so no menu-specific State branch is needed. Its link switch defaults unrecognized types to Generic, which makes State non-linkable and must remain so. `MarkCollectionExportRowViewModel` initializes export options and must include State labels as text by default. Native import/export must preserve State through existing `CollectionType` persistence; do not change classifications assigned by external timing importers.

`src/Vixen.Core/Services/EffectDefaults/EffectDefaultScrubber.cs` removes fields decorated with `[ExcludeFromEffectDefault]`. The creation seam documented in `docs/plans/effects/vix-3964-effect-default-settings.md` applies scrubbed module data to a newly-created effect. Once the effect receives the destination sequence's mark collection list, the new lifecycle normalization must repair the empty selection locally.

## Plan of Work

### Milestone 1 — Record the approved user-facing contract in Jira

Use the Jira connector to read VIX-3995 before editing it. Update its description using the repository Jira format: a concise Summary, Scope, and Given/When/Then acceptance criteria. Describe State-tagged collection selection, valid-selection preservation, State text export, and the fact that State tracks are non-linkable. Mention regression coverage succinctly, but do not expose source file names or internal service design. Do not transition the issue. Record the issue's current status and a link or update timestamp in this plan's Progress section.

This milestone is independently verifiable by re-reading VIX-3995: a reviewer can understand what a user gains and how it is accepted without reading this plan.

### Milestone 2 — Add the State collection type and docker/persistence coverage

Edit `src/Vixen.Core/Marks/IMarkType.cs`. Add XML documentation for `MarkCollectionType` and every member, explicitly assigning the stable values: `Generic = 0`, `Phrase = 1`, `Word = 2`, `Phoneme = 3`, then append `State = 4`. Do not reorder members or use a migration.

Edit `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionExportRowViewModel.cs` so `IsTextIncluded` defaults to `true` for State as well as Phrase, Word, and Phoneme. Leave `MarkCollectionViewModel.SetupCollectionTypeCheckboxes` enumeration unchanged, and leave `SetupLinkedToCheckboxes`' default-to-Generic behavior unchanged; State must therefore show in the type menu and have `IsLinkableType == false`.

Locate existing native collection serialization and import/export tests under `src/Vixen.Tests/Sequencer`. Add focused tests to prove all old enum integer values, `State == 4`, and a native persisted `MarkCollection` round-trip preserves `CollectionType == State`. Add marks-docker view-model tests that create a State collection and assert it appears among checkbox states, has no link candidates/is non-linkable, and creates an export row with `IsTextIncluded == true`. Keep external importer fixtures and classifications unchanged; add only a regression proving an externally classified collection remains its existing type if a relevant test seam already exists.

Acceptance for this milestone is a State type visible in the docker and serializable without changing any historic type number.

### Milestone 3 — Create the Core selection contract and BaseEffect lifecycle template

Create `src/Vixen.Core/Marks/IMarkCollectionSelection.cs` and `src/Vixen.Core/Marks/MarkCollectionSelectionService.cs`, adding them to `Vixen.Core.csproj` only if the project does not use SDK default compile globs. Both public types and their public members require complete XML documentation.

Make `IMarkCollectionSelection` the documented policy contract consumed by the base lifecycle. It must describe a selection as: the effect-local `Guid` getter/setter to normalize, whether that slot is currently active, its preferred collection type (or no preference), and whether it may use first-collection fallback. The contract must also make clear that it owns no shared collection state. Model the service as stateless—no static mutable state, caches, UI services, dispatcher calls, `TypeDescriptor.Refresh`, collection subscriptions, or writes to `IMarkCollection`.

Give `MarkCollectionSelectionService` one documented operation that receives the current ordered collection sequence and an `IMarkCollectionSelection`, then returns/commits only an `IMarkCollection.Id` or `Guid.Empty`. The precise public signature should be selected to match repository style, but it must express these deterministic rules:

    1. If the slot is inactive, do nothing.
    2. If its non-empty ID matches any current collection ID, leave it unchanged regardless of collection type.
    3. Otherwise, choose the first collection with the requested preferred type, when one is requested.
    4. Otherwise, if the policy allows fallback, choose the first collection in current collection order.
    5. Otherwise set/retain `Guid.Empty`.

Use IDs and `IMarkCollection` instances throughout this operation. Do not call display-name setters or search by name. Enumerate only enough to satisfy the rule and treat the existing `ObservableCollection` as read-only. A straightforward scan per active slot is acceptable and is the required O(slots × collections) bound.

Refactor `BaseEffect.cs` to override the three collection lifecycle callbacks inherited from `EffectModuleInstanceBase` and make those overrides the stable Template Method. It must normalize active slots before or at the same lifecycle point as effect-specific listener/display updates, mark the effect dirty only when an effect-local selection actually changes, and then invoke protected derived hooks for the legacy effect-specific work. Replace affected derived overrides of `MarkCollectionsChanged`, `MarkCollectionsAdded`, and `MarkCollectionsRemoved` with those new hooks so they cannot bypass normalization. Preserve existing listener attach/detach and name-refresh behavior. Do not put this shared behavior in `EffectModuleInstanceBase`, which is broader than the basic effects covered by the ticket.

Add service-level xUnit tests in a new focused file under `src/Vixen.Tests/Effects` (or the existing Marks test grouping if it is the established Core test location). Test ordinary first-collection policy, State preference then first fallback, Phoneme preference with no fallback, valid ID preservation across every policy, missing non-empty ID repair, no-collection result, and that collection type/name/order/link/tag fields are not changed. Use concrete `MarkCollection` instances with deliberately distinct IDs and names to prove identity—not text—controls selection.

Acceptance is a direct test run proving deterministic selection policy without WPF or rendering, and a `BaseEffect` lifecycle that has one normalization route.

### Milestone 4 — Bind policies to effects without data migration

Implement the ordinary policy for each active single-selector basic effect already deriving from `BaseEffect`: an empty or missing `Guid` selects the first sequence collection, and a valid ID remains untouched. Declare only slots that the effect is actively using; an inactive mode must neither receive a default nor be cleared. Audit the current mark-using effects listed in the effect-default scrubber and their lifecycle overrides, rather than assuming every `MarkCollectionId` is active in every mode.

For `State.cs`, replace `Enum.TryParse<MarkCollectionType>("State")` with direct `MarkCollectionType.State`. Remove the ad hoc name-based fallback in the `RenderSource` setter and feed the active State slot to the shared lifecycle with preferred type `State` and first-collection fallback enabled. Keep listener management, but commit the chosen `Guid` directly to `_data.MarkCollectionId`; the display-name property remains UI conversion only. A valid Generic ID persists when State collections are later inserted. On removal, the BaseEffect lifecycle sees the missing ID and selects the next State collection in sequence order, otherwise the first remaining collection; it reaches `Guid.Empty` only when the sequence has no collections. Preserve `[ExcludeFromEffectDefault]` and `StateData` exactly.

For `LipSync.cs`, replace pre-render/default and removal repair with the shared active-slot policy: preferred type `Phoneme`, no first-collection fallback. Preserve the existing converter behavior that lists a valid legacy non-Phoneme choice beside Phoneme choices; normalization must not invalidate a valid legacy selection. In particular, do not make LipSync fall back to Generic when a phoneme track is absent.

For `Wave.cs` and `Liquid.cs`, declare one ordinary-policy selection per active child selector, respectively each `IWaveform` and each `IEmitter` that is in its mark-controlled mode. Normalize children independently, including after defaults/cross-sequence module data creates empty IDs and after deletion. Reuse their existing display-name/listener refresh routines only after IDs are repaired; do not let a name refresh silently clear or replace a valid ID. CountDown is unchanged because it declares no selection.

Add effect integration tests. For State, cover: valid collection preservation irrespective of type; first State selection; Generic first fallback when no State exists; selected collection removal choosing next State then first remaining; and adding State after a valid Generic fallback causing no reselection. Exercise the effect-default/cross-sequence path by applying scrubbed State default data to a fresh State effect, supplying another sequence's ordered collections, and asserting its active State slot repairs as described. Add equivalent empty/missing ID and valid-preservation regressions for LipSync, Wave, and Liquid. If Wave/Liquid are not referenced by `Vixen.Tests.csproj`, add project references that follow repository convention (`Copy Local = No`, `Include Assets = None`) before writing their direct tests; do not use reflection to avoid a missing test reference.

Every test must snapshot shared collection IDs, names, `CollectionType`, `LinkedMarkCollectionId`, order, and mark content before normalization, then assert they are identical afterwards. Only the effect-local selected IDs may change.

Acceptance is a stateful effect-level test suite that proves selection repairs occur on lifecycle events, not in rendering, and that one child slot cannot overwrite another.

### Milestone 5 — Validate, document results, and close the Jira loop

Run the narrow test filters while iterating, then use the repository's full-MSBuild workflow because the test project has C++/CLI transitive dependencies. From `C:\Dev\Vixen`, run:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

The first command must build `Vixen_Tests` successfully; the second must report all tests passed. Also build the Release/x64 solution if the changed Core/module project graph is not fully covered by the test target:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -v:m

Perform a manual Timed Sequence Editor walkthrough: create ordered Generic and State tracks; tag the State track; verify State is non-linkable; add a State effect in Mark Collection mode; remove its selected State track; add a State track after a valid Generic fallback; and export the State collection with default text inclusion. Observe the selection rules stated in Purpose / Big Picture and no unexpected collection mutations.

Update VIX-3995 only if the final user-facing scope differs from Milestone 1, then add a concise closeout comment containing user-visible result, exact automated command outcome/counts, manual walkthrough result, and residual risk. Do not transition the issue. Update every living-plan section and append a dated revision note before handoff. If a milestone changes repository files, use the repository `commit-msg` skill to include its formatted proposed commit message in the completion response, but do not commit unless explicitly requested.

## Concrete Steps

1. Before edits, run `git status --short` in `C:\Dev\Vixen`; preserve all unrelated changes.
2. Complete Milestone 1 through Milestone 5 in order. After each milestone, update Progress with a UTC timestamp, record discoveries/decisions, run that milestone's focused tests, and write a proposed conventional commit message in the milestone response when code changed.
3. Use `rg` to find every `MarkCollectionId`, every `MarkCollectionsChanged`/`Added`/`Removed` override, and every collection importer before changing lifecycle code. Re-read each file that will be edited; do not mechanically modify unrelated effects.
4. Use `apply_patch` for source and plan edits. Keep tabs and LF line endings per `src/.editorconfig`; do not reformat unrelated legacy code.

Expected final test transcript shape:

    Build succeeded.
    Passed!  - Failed: 0, Passed: <updated total>, Skipped: 0, Total: <updated total>

The exact passed total must be recorded rather than guessed in the final plan/Jira comment.

## Validation and Acceptance

Automated acceptance requires tests for enum number stability, State serialization, marks-docker menu/link/export defaults, shared selection rules, and the State/LipSync/Wave/Liquid lifecycle cases named above. A test that simply calls render is insufficient; include assignment, add, remove, and default/cross-sequence repair paths. The new State tests must fail before the change because `MarkCollectionType.State` does not exist and State cannot select it.

Human acceptance requires these visible results:

- State is offered as a collection type; it has no linking choices.
- A State-mark-rendering effect with no selection uses the first State track, or the first track when none is State.
- A valid selected track remains selected after a preferred State track is added.
- Removing the selected track selects the next State track, otherwise the first remaining track.
- Exporting a State track starts with mark text included.
- A State default created in one sequence does not retain that sequence's ID in another; it selects from the destination sequence using the stated policy.

## Idempotence and Recovery

The source changes are additive or local refactors and can be re-run safely. No data migration, profile rewrite, or shared collection mutation is permitted. If a lifecycle refactor causes listener regressions, restore the pre-existing listener/name-refresh work inside the new BaseEffect hooks while retaining the centralized ID-only normalization; do not reintroduce per-effect policy copies. If a test project reference is required for Wave or Liquid, add it once with repository reference metadata and rerun restore; do not substitute a DLL reference.

If build output is stale, rerun the full `Vixen_Tests` MSBuild command before `dotnet test --no-build`. If a Jira update fails due to credentials, leave code untouched, record the failure in this plan, and report it for user action; the local plan and implementation work can still be validated.

## Artifacts and Notes

The essential selection algorithm to preserve in code and tests is:

    Normalize(slot, collections):
        if slot is inactive:
            return unchanged
        if slot.Id is non-empty and collections contains collection.Id == slot.Id:
            return unchanged
        preferred = first collection where CollectionType == slot.PreferredType
        if preferred exists:
            slot.Id = preferred.Id
        else if slot.AllowsFirstCollectionFallback:
            slot.Id = collections.FirstOrDefault()?.Id ?? Guid.Empty
        else:
            slot.Id = Guid.Empty

For State, `PreferredType` is `MarkCollectionType.State` and fallback is enabled. For ordinary active slots, no preferred type and fallback is enabled. For LipSync, `PreferredType` is `MarkCollectionType.Phoneme` and fallback is disabled. This order is intentional: validity always wins before preference.

## Interfaces and Dependencies

In `src/Vixen.Core/Marks/IMarkCollectionSelection.cs`, create a public, XML-documented interface representing one effect-local selectable mark-collection slot. Its final contract must make these members available to `BaseEffect`/the service: whether the slot is active, the current selected `Guid`, the optional preferred `MarkCollectionType`, and whether first-collection fallback is allowed. The setter writes only to the owning effect or child model; it must never write to an `IMarkCollection`.

In `src/Vixen.Core/Marks/MarkCollectionSelectionService.cs`, create a public, XML-documented stateless service implementing a documented normalization method conceptually equivalent to:

    Guid Normalize(IReadOnlyList<IMarkCollection> collections, IMarkCollectionSelection selection)

The concrete signature may return a selected `IMarkCollection?` and let `BaseEffect` commit `.Id`, but it must preserve the exact rules in Artifacts and Notes and must not identify collections by name. Keep dependencies limited to Core `Vixen.Marks` contracts and BCL collection types.

In `src/Vixen.Modules/Effect/Effect/BaseEffect.cs`, add the protected lifecycle extension points that expose active `IMarkCollectionSelection` slots and derived post-normalization collection-change handling. `BaseEffect` must be the only class calling the service in normal effect lifecycle. Derived effects declare policy/active slots and retain listeners/UI notifications only through the hooks.

No new NuGet packages, dispatcher usage, UI service registration, serialization schema version, or State-data migration is allowed.

Revision note (2026-09-01 / Codex): Created from the approved VIX-3995 handoff after source research. This is a planning-only change; no product implementation was performed.

Revision note (2026-09-01 / Codex): Completed Milestone 1 by replacing VIX-3995's description with the approved user-facing Summary, Scope, and acceptance criteria. The issue remains in New Ticket; no transition or implementation work was performed.

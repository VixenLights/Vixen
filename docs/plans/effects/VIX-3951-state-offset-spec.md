# Implement VIX-3951 State Effect Cycle Offset

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept current as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. It is intentionally self-contained: a contributor must be able to implement VIX-3951 from this file and the current working tree without relying on an earlier conversation or another design document. The approved technical specification is retained at `docs/effects/VIX-3951-state-offset-spec.md`; if implementation requires a change to the behavior described here, revise this plan first, record the reason in `Decision Log`, and then reconcile the technical specification.

## Purpose / Big Picture

The State effect can already render a static selection or repeatedly cycle State items, Mark Collection segments, or custom State rows. VIX-3951 gives a sequence author a Cycle Offset control that changes which existing cycle slot appears first. For example, a State effect that normally cycles `Red`, `Green`, `Blue` can start at `Green` with an offset of one and then continue `Green`, `Blue`, `Red`.

This is a phase shift, not a different order algorithm. It does not change slot duration, grouping, colors, marks, assignments, or how many times a sequence repeats. The feature is observable in the State Effect Editor: Cycle Offset is available only in Iterate playback mode, accepts values from zero through 100, marks the effect dirty when changed, and causes the first rendered state to move forward by that many slots with wrap-around. The feature must work consistently for State Item, Mark Collection, and Custom render sources while preserving all current behavior at offset zero.

## Progress

- [x] (2026-07-27 00:00Z) Created the approved technical specification at `docs/effects/VIX-3951-state-offset-spec.md`.
- [x] (2026-07-27 00:00Z) Created this implementation ExecPlan from that specification and read `.agents/PLANS.md`.
- [x] (2026-07-27) Read the project implementation skills and inspect the current State effect, resources, and focused tests before editing source.
- [x] (2026-07-27) Add the persisted raw `CycleOffset` data contract and editor property, including localized metadata and Iterate-only visibility.
- [ ] Add allocation-free indexed offset selection to State Item, Mark Collection, and Custom Iterate planning paths.
- [ ] Add focused data/editor/planner regression tests for zero-offset compatibility, wrapping, blank slots, grouping, iterations, and remainder ticks.
- [ ] Run focused automated validation, State project build, and the broadest practical regression test suite; record actual output below.
- [ ] Perform manual State Effect Editor/playback verification and update this plan with observed results.
- [ ] Update Jira issue VIX-3951's description with the refined requirements, acceptance criteria, and test plan, then close out this plan with outcomes, residual risk, and a revision note.

## Surprises & Discoveries

- Observation: The current State planner already separates the three render-source paths and calculates durations from an output slot index.
  Evidence: `StateRenderPlanner.CreateIteratedIntervals`, `AddIteratedMarkIntervals`, `CreateIteratedCustomIntervals`, and `CreateGroupedCustomIntervals` all select a source with `index % baseSlotCount` and call `GetIntervalDuration` using the unmodified output `index`.

- Observation: Mark Collection Iterate mode preserves empty and unknown comma-delimited segments in its parsed name list.
  Evidence: `AddIteratedMarkIntervals` counts `names.Count * NormalizeIterations(iterations)`, then skips intent creation only after the segment has received its duration. This is why a Cycle Offset must select from the unfiltered parsed list.

- Observation: Custom grouped mode groups only consecutive rows and intentionally gives missing IDs distinct grouping keys.
  Evidence: `CreateCustomStateItemGroups` uses `GetCustomStateItemGroupKey`; valid rows use `Name:<exact name>`, `<None>` uses `None:`, and unresolved rows use `Missing:<guid>`.

- Observation: The completed base slot list and current modulo expression are distinct in every Iterate path.
  Evidence: State Item `<All>` uses `orderedNames` from `GetUniqueStateItemNames` in `CreateIteratedIntervals` (`orderedNames[index % orderedNames.Count]`); Mark Collection uses each clipped mark's complete parsed `names` list in `AddIteratedMarkIntervals` (`names[index % names.Count]`); Custom individual uses `customStateItems` in `CreateIteratedCustomIntervals`; and grouped Custom uses the completed `groups` list in `CreateGroupedCustomIntervals`. Each path derives duration from the chronological loop `index`, so a future offset must change only source selection.

- Observation: The State editor's dynamic visibility mechanism has a single central seam.
  Evidence: `State.SetRenderSourceBrowsables` passes property names and Boolean visibility conditions to `SetBrowsable`, then calls `TypeDescriptor.Refresh(this)`. Existing iterate-only `Iterations` and Custom-Iterate-only `CycleIndividually` establish the required metadata and invalidation pattern for Cycle Offset.

## Decision Log

- Decision: Treat Cycle Offset as an initial index offset applied to a completed base slot list before iteration repetition.
  Rationale: This produces `[B, C, A, B, C, A]` for base `[A, B, C]`, offset one, and two iterations. Rotating the fully repeated output instead would produce an incorrect sequence boundary.
  Date/Author: 2026-07-27 / Codex

- Decision: Persist the raw integer and normalize only against the current base slot count during rendering.
  Rationale: Slot count depends on the active State definition, mark text, custom rows, and Custom grouping. Persisting a modulo result would make the stored setting change merely because those inputs change and would lose the author's intended raw value.
  Date/Author: 2026-07-27 / Codex

- Decision: Apply no offset in Default playback and hide Cycle Offset there.
  Rationale: Default playback renders selected items simultaneously, so it has no chronological slots to rotate. Showing an irrelevant editor control would imply behavior that does not exist.
  Date/Author: 2026-07-27 / Codex

- Decision: Use normalized indexed lookup rather than materializing a rotated collection.
  Rationale: Indexing preserves existing slot objects and avoids extra `Skip`, `Concat`, or `ToList` allocations. More importantly, it keeps parsing and grouping logically before the offset step.
  Date/Author: 2026-07-27 / Codex

- Decision: Treat malformed non-positive persisted offsets as zero only in the planner and do not rewrite stored data during rendering.
  Rationale: The editor range prevents negative input, but defensive rendering avoids invalid negative indices if manually edited or legacy data contains one. Mutating data during render would violate the raw-persistence contract and create unexpected saves.
  Date/Author: 2026-07-27 / Codex

## Outcomes & Retrospective

Implementation has not started. The expected completed outcome is a State Effect Editor Cycle Offset property that changes Iterate scheduling only, preserves zero-offset output exactly, and is proven by focused automated tests plus manual playback across all three supported render sources.

At completion, replace this paragraph with the implemented files, actual test summaries, manual verification observations, any deviations from this plan, and remaining risks.

## Milestones

### Milestone 1: Establish the implementation baseline

Read the project-specific C# implementation and public-API documentation guidance before changing code: `.agents/skills/dotnet-best-practices/SKILL.md` and `.agents/skills/csharp-docs/SKILL.md`. Inspect the State feature documents under `docs/state/`, especially `docs/state/vix-3924-state-effect.md`, `docs/state/vix-3924-state-effect-phase-2.md`, and `docs/state/vix-3924-state-effect-phase-3.md`. These documents define the existing State Item, Mark Collection, Custom, and grouped-Custom semantics that VIX-3951 must preserve.

Then inspect `StateData.cs`, `State.cs`, `StateRenderPlanner.cs`, both Effect Editor resource files, and the existing State tests. Confirm the current editor ordering and resource-key convention before adding any metadata. Confirm that no unrelated working-tree change is mixed into the task. The milestone is complete when the executor can explain which current list is the base slot list for each source and where the planner currently uses `index % count`.

### Milestone 2: Add the persisted and editor-facing contract

Add the `CycleOffset` data member and public editor property without changing any render scheduling yet. A newly created effect and a deserialized effect that lacks the member must behave as zero offset. The editor must expose an integer SliderEditor range of zero through 100 only when `PlaybackMode` is Iterate. Its setter must follow the existing State property invalidation pattern: assign only when changed, set `IsDirty`, and notify the property grid.

At the end of this milestone, a developer can instantiate `StateData` and observe `CycleOffset == 0`; clone data and preserve a nonzero raw value; and inspect the State effect through `TypeDescriptor` to see localized Cycle Offset metadata and correct visibility. The property may be visible but must still be a no-op until Milestone 3 supplies scheduling behavior.

### Milestone 3: Apply the offset to completed Iterate slot sequences

Implement a small private planner helper that converts an output slot index to a source index using the raw offset modulo the completed base slot count. The helper may normalize the offset once before looping, but it must guard empty lists before modulo and must select by index rather than allocate a rotated list.

Integrate it after each source has established its existing slots: unique exact-name groups for State Item `<All>`, parsed segments per clipped Mark Collection mark, raw Custom rows when cycling individually, and already-built consecutive groups when cycling as groups. Preserve the current output-index duration computation and final-slot remainder ticks. Do not change Default-mode paths. At the end of this milestone, an offset one visibly starts each supported Iterate source on its second calculated slot while blank/missing slots still consume time.

### Milestone 4: Prove compatibility and source-specific behavior

Extend the focused test suite. Data/editor tests must prove persistence, cloning, bounds, visibility, ordering, and dirty notification. Planner tests must prove zero-offset parity, forward rotation, equal/count-plus-one/large wrap-around, zero and singleton lists, iteration ordering, State Item atomic name groups, Mark Collection blank segments, Custom individual rows, grouped Custom atomicity, per-row colors, missing values, and tick remainder behavior.

The milestone is complete when the new tests would fail if offset is applied before grouping/parsing, after repeating iterations, or by filtering blank timing slots. Tests should use deterministic `StateRenderInterval` identity, start, duration, and color assertions rather than relying only on interval count.

### Milestone 5: Validate the integrated module and visual behavior

Run the focused State tests, compile the State module, and run the broadest practical State/test-project regression suite. Record command output and any unrelated warnings in `Artifacts and Notes` and `Surprises & Discoveries`. Then manually exercise each supported source in the Timed Sequence Editor. Verify the first visible state changes with Cycle Offset, wraps correctly, and that switching to Default hides the control and produces existing simultaneous behavior. Finish by updating Progress, Outcomes, and Revision Notes with actual evidence.

### Milestone 6: Update the Jira issue and close out the plan

After implementation and validation are complete, update the description of Jira issue `VIX-3951` at `https://vixenlights.atlassian.net/browse/VIX-3951`. Use the project Jira skill at `.agents/skills/jira/SKILL.md` and the configured Atlassian connector; do not change the issue status unless the user separately requests a transition. Replace or revise the issue description so it has distinct **Refined Requirements**, **Acceptance Criteria**, and **Test Plan** sections based on this ExecPlan's final, implemented behavior.

The refined-requirements section must state that Cycle Offset is a raw persisted integer, editor-limited to 0 through 100, visible only for Iterate playback, and applied as `(outputSlotIndex + CycleOffset) % baseSlotCount` after source-specific slots are calculated and before `Iterations` repeats them. It must name all supported sources—State Item, Mark Collection, and Custom—and explicitly preserve Mark unknown/empty timing slots, Custom grouped atomicity, Default behavior, timeline text, and zero-offset compatibility. The acceptance-criteria section must state the binary wrapping, iteration, empty/singleton, grouping, timing-remainder, dirty-state, and resource/editor requirements. The test-plan section must include the focused automated test command, State project build, broad regression command when feasible, and the manual playback scenarios recorded in this plan.

Before updating Jira, read the current issue description and verify the issue key/project through the connector. After the update, re-read the issue and record the update time, fields changed, and a concise summary of the description's sections in `Artifacts and Notes`. Then mark this milestone and the final Progress item complete, update `Outcomes & Retrospective`, and append a Revision Note. Acceptance is that VIX-3951's description—not merely a comment—contains the final refined requirements, acceptance criteria, and test plan and that this plan records evidence of the update.

## Context and Orientation

Vixen is a Windows desktop application built with .NET and WPF for sequencing animated lighting. An effect is a plugin that produces render intents over an effect time span. The State effect is the plugin in `src/Vixen.Modules/Effect/State/`; it applies named, colored State items from a State property definition to assigned element nodes.

A State definition contains ordered State items. Multiple State items may share an exact name; such same-name rows form an atomic State Item slot when cycling `<All>`, because activating that name activates every matching row together. A render interval is the planner's internal representation of one State item rendered at a start time for a duration. `StateRenderPlanner` creates those intervals before `State` expands assignments into leaf-node segments, resolves discrete colors, coalesces compatible adjacent segments, and renders intents. None of those later steps are part of Cycle Offset.

`PlaybackMode.Default` means current items render concurrently for the full relevant duration. `PlaybackMode.Iterate` means the source produces chronological timing slots, repeats them according to `Iterations`, and divides the duration among them. The editor may describe Iterate as Cycle; they are the same State effect mode for this task.

The State effect has exactly three relevant render sources:

- `StateRenderSource.StateItem` obtains a selected State item or the `<All>` selection from the active definition. A specific selected item is a full-duration single group. `<All>` with Iterate builds one slot per unique exact State-item name in definition order.
- `StateRenderSource.MarkCollection` reads timeline marks. A mark label is parsed as comma-delimited exact State-item names. In Iterate mode every parsed segment is one slot for that mark, including empty strings and unknown names; those non-rendering slots consume time. Marks are clipped to the State effect time span, gaps stay blank, and overlaps are independently rendered.
- `StateRenderSource.Custom` uses the persisted ordered `CustomStateItemData` row collection. In Iterate mode, `CycleIndividually = true` treats every row as one slot. With `CycleIndividually = false`, only consecutive rows with the same grouping key form one slot; the group contents remain ordered and keep their individual color overrides. A `<None>` row has an empty State item id and consumes a blank slot. A missing id also consumes time but produces no interval.

The persisted configuration class is `src/Vixen.Modules/Effect/State/StateData.cs`. It is a `[DataContract]` deriving from `EffectTypeModuleData`; `[DataMember]` properties are serialized in effect data. `CreateInstanceForClone()` is the clone path and must explicitly copy the new value. Existing data that has no new data member receives C#'s default integer value, zero.

The runtime/editor class is `src/Vixen.Modules/Effect/State/State.cs`. It owns `_data`, provides public editor properties decorated with `Value`, Effect Editor provider attributes, and `NumberRange`, and sets `IsDirty` to request rerendering. `CreateRenderIntervals` is the one dispatch point that passes State configuration to the planner. `SetRenderSourceBrowsables` uses a property-name-to-Boolean dictionary and `SetBrowsable` to hide or show properties based on source and playback mode.

`src/Vixen.Modules/Effect/State/StateRenderPlanner.cs` is an internal static class. Its public-to-the-assembly entry points return `IReadOnlyList<StateRenderInterval>`. `CreateStateItemIntervals`, `CreateMarkCollectionIntervals`, and `CreateCustomIntervals` choose a render-source path. `GetIntervalDuration` assigns equal whole-tick durations except that the final output slot receives `effectDuration - accumulatedPriorDurations`; preserve that method and its callers' output index behavior.

The Effect Editor obtains localized labels and descriptions from `src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDisplayNameDescriptors.resx` and `src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDescriptionDescriptors.resx`. The State effect currently uses resource keys such as `StateIterations` and `StateCycleIndividually`; add matching Cycle Offset keys to both files rather than hard-coding user-facing text.

Focused tests are under `src/Vixen.Tests/Effect/State/`. `StateDataTests.cs` already tests data defaults, cloning, Effect Editor browsability, ordering, and resource-backed display metadata. `StateRenderPlannerTests.cs` already constructs State definitions, marks, and Custom row data to test scheduling without starting the WPF editor. Add cases in those files unless a narrowly named companion test file materially improves clarity.

## Plan of Work

Start from a clean understanding of the current code rather than editing only from a proposed signature. Read every current planner function that builds Iterate intervals. The key implementation rule is: build the source's existing base slots first, then rotate source selection by an index while retaining the existing output-slot count, output index, start time, and duration. Applying the offset to State definition rows, filtering a Mark Collection list, or rotating a repeated list would be behaviorally wrong.

In `StateData.cs`, add internal constants for the editor range if that matches the existing `MinIterations` and `MaxIterations` pattern: `MinCycleOffset = 0` and `MaxCycleOffset = 100`. Add the documented `[DataMember] public int CycleOffset { get; set; }`, defaulting implicitly or explicitly to zero, near `Iterations` and `CycleIndividually`. Do not add a setter that clamps, normalizes, or otherwise modifies the input. Copy the raw member in `CreateInstanceForClone`. No migration is needed: missing serialized values default to zero.

In `State.cs`, add a documented public `CycleOffset` property after `Iterations` and before `CycleIndividually`. Decorate it with `[Value]`, `[ProviderCategory("Config", 2)]`, resource-backed display and description attributes, `[PropertyEditor("SliderEditor")]`, `[NumberRange(StateData.MinCycleOffset, StateData.MaxCycleOffset, 1)]`, and a property order that leaves Iterations before it and CycleIndividually/CustomStateItems after it. Match the State class's existing tab indentation and style. The getter returns `_data.CycleOffset`. The setter must compare stored/raw values, assign a changed raw value, set `IsDirty = true`, and call `OnPropertyChanged()`. It must not normalize the value, notify visual representation, or modify other configuration.

Update `SetRenderSourceBrowsables` to include `nameof(CycleOffset)` with `PlaybackMode == PlaybackMode.Iterate`. This is deliberately independent of `RenderSource`: State Item, Mark Collection, and Custom all use it in Iterate mode. Add the new display-name and description resource entries alongside existing State keys. Use concise text such as `Cycle Offset` and `Sets the number of Cycle timing slots to skip before the sequence begins.` Preserve each `.resx` file's existing encoding and XML formatting.

Extend the `CreateRenderIntervals` calls in `State.cs` so each relevant planner entry point receives `CycleOffset`. Thread the argument only into Iterate logic; Default output must not be changed by the value. If compatibility overloads are retained for current tests/internal callers, have them explicitly forward `cycleOffset: 0` and ensure the main production overload is unambiguous.

In `StateRenderPlanner.cs`, implement a private helper such as `GetOffsetSlotIndex`. It receives the chronological output index, a positive base slot count, and the raw offset. The operation is:

    normalizedOffset = cycleOffset > 0 ? cycleOffset % slotCount : 0
    sourceIndex = (outputIndex % slotCount + normalizedOffset) % slotCount

Calculate `normalizedOffset` once per planner operation when practical. Never call it for zero slots; the caller must return an empty interval list before division or modulo. A one-slot list naturally selects index zero. Do not use `Skip`, `Take`, `Concat`, or `ToList` to rotate input collections. Do not mutate `definition.Items`, parsed names, custom rows, or custom groups.

For State Item `<All>` Iterate, keep `GetUniqueStateItemNames(items)` as the base slot list. Keep `intervalCount = orderedNames.Count * normalizedIterations`, `GetIntervalDuration`, and `intervalStart` exactly as they are. Replace the name lookup with the indexed offset selection, then retain the current exact-name expansion over `items`. Leave `CreateSelectedItemIntervals` and `CreateDefaultIntervals` untouched so a specifically selected State Item remains one full-duration group and Default is unchanged.

For Mark Collection Iterate, retain the per-mark loop, effect clipping, parser, mark order, and `names.Count * normalizedIterations` calculation. `names` is the completed base list even if it contains empty strings or unknown labels. During the current segment loop, calculate duration using the unmodified chronological `index`, then obtain the name by offset source index. The existing `if (!string.IsNullOrEmpty(name) && itemGroups.TryGetValue(...))` remains after selection so invalid slots still advance `segmentStart` and consume timing. Do not offset marks against one another; each clipped mark independently rotates its own segment list. Leave `AddDefaultMarkIntervals` unchanged.

For Custom individual Iterate, use the original ordered `customStateItems` list as the base slots. Preserve `intervalCount = customStateItems.Count * normalizedIterations`, then choose `customStateItems[sourceIndex]` at each output index. The existing checks for `Guid.Empty` and missing item IDs must remain after selection. This preserves duplicates, `<None>`, missing ids, and per-row color overrides as timing slots.

For Custom grouped Iterate, call the existing `CreateCustomStateItemGroups` first. Retain current grouping keys and the existing `groups.Count * normalizedIterations` count. Choose a group by source index, then enumerate its rows in their existing sequence, producing the same interval start/duration for every renderable row in that group. Do not flatten groups or re-run grouping after rotating: this would allow an offset to split atomic groups and violate Phase 3 behavior. Consider and guard the unlikely empty-groups path before modulo, even though non-empty custom input should produce at least one group.

Add tests before or alongside each behavior. Follow the project C# test conventions: xUnit, Arrange/Act/Assert comments, tabs, and deterministic data. Use existing test helpers such as `CreateDefinition`, `CreateItem`, custom-row creators, and mark fakes where present. Where zero-offset parity is required, either compare against a dedicated zero-offset expected sequence or retain a compatibility planner call and compare every interval's item id, start, duration, and optional override color. Do not assert only aggregate counts because several important regressions preserve counts while moving output into the wrong timing slot.

## Concrete Steps

Run all commands from the repository root, `C:\Dev\Vixen`.

First inspect the worktree so unrelated user edits are preserved:

    git status --short

Read the required implementation guidance and State documentation:

    Get-Content -Raw .agents\skills\dotnet-best-practices\SKILL.md
    Get-Content -Raw .agents\skills\csharp-docs\SKILL.md
    Get-Content -Raw docs\state\vix-3924-state-effect.md
    Get-Content -Raw docs\state\vix-3924-state-effect-phase-2.md
    Get-Content -Raw docs\state\vix-3924-state-effect-phase-3.md
    Get-Content -Raw docs\effects\VIX-3951-state-offset-spec.md

Inspect the exact implementation and test seams before changing them:

    Get-Content -Raw src\Vixen.Modules\Effect\State\StateData.cs
    Get-Content -Raw src\Vixen.Modules\Effect\State\State.cs
    Get-Content -Raw src\Vixen.Modules\Effect\State\StateRenderPlanner.cs
    Get-Content -Raw src\Vixen.Tests\Effect\State\StateDataTests.cs
    Get-Content -Raw src\Vixen.Tests\Effect\State\StateRenderPlannerTests.cs
    rg -n -C 3 "StateIterations|StateCycleIndividually" src\Vixen.Modules\EffectEditor\EffectDescriptorAttributes

Implement the data and editor work in these files:

    src\Vixen.Modules\Effect\State\StateData.cs
    src\Vixen.Modules\Effect\State\State.cs
    src\Vixen.Modules\EffectEditor\EffectDescriptorAttributes\EffectDisplayNameDescriptors.resx
    src\Vixen.Modules\EffectEditor\EffectDescriptorAttributes\EffectDescriptionDescriptors.resx

Then modify `src\Vixen.Modules\Effect\State\StateRenderPlanner.cs` and thread the raw `CycleOffset` from `State.CreateRenderIntervals`. Restrict behavioral changes to Iterate branches. Keep the visible State timeline text unchanged; no code that builds visual-representation text should use Cycle Offset.

Add focused tests in:

    src\Vixen.Tests\Effect\State\StateDataTests.cs
    src\Vixen.Tests\Effect\State\StateRenderPlannerTests.cs

Use test names that make the scheduling guarantee clear. At minimum, add tests equivalent to:

    CycleOffset_DefaultsToZero
    Clone_CopiesCycleOffsetRawValue
    CycleOffset_UsesResourceBackedDisplayMetadata
    CycleOffset_IsBrowsableOnlyForIteratePlayback
    CycleOffset_ChangedValueMarksEffectDirty
    CreateStateItemIntervals_CycleOffsetRotatesUniqueNameSlots
    CreateStateItemIntervals_CycleOffsetZeroPreservesIntervals
    CreateMarkCollectionIntervals_CycleOffsetRotatesUnknownAndEmptyTimingSlots
    CreateCustomIntervals_CycleOffsetRotatesIndividualRows
    CreateCustomIntervals_CycleOffsetRotatesCompletedGroups
    Create..._CycleOffsetRepeatsRotatedBaseSequenceBeforeIterations
    Create..._CycleOffsetPreservesFinalRemainderTicks

The precise names may follow existing test naming, but the tests must prove the stated behavior. In particular, create a test whose expected output distinguishes base-sequence rotation before repetition from rotating the final repeated sequence.

Check only task files for whitespace problems before testing:

    git diff --check -- src\Vixen.Modules\Effect\State\StateData.cs src\Vixen.Modules\Effect\State\State.cs src\Vixen.Modules\Effect\State\StateRenderPlanner.cs src\Vixen.Modules\EffectEditor\EffectDescriptorAttributes\EffectDisplayNameDescriptors.resx src\Vixen.Modules\EffectEditor\EffectDescriptorAttributes\EffectDescriptionDescriptors.resx src\Vixen.Tests\Effect\State\StateDataTests.cs src\Vixen.Tests\Effect\State\StateRenderPlannerTests.cs

Run focused State tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore

Expected success resembles:

    Passed!  - Failed: 0, Passed: <positive count>, Skipped: 0, Total: <positive count>

Run the State effect project build:

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore

Expected success includes `Build succeeded.` and no State effect compilation errors. If dependencies are not available for `--no-restore`, first confirm the failure is restore-related, then run the same command without `--no-restore` only if package restore is authorized in the current environment.

Run the broad test project when feasible:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If the broad command fails for an established unrelated failure, record the complete failing test name and error in `Surprises & Discoveries`, then keep the focused State results as the VIX-3951 validation evidence. Do not alter unrelated production code merely to obtain a green broad run.

Finally, manually validate from the Timed Sequence Editor using a State definition with clearly distinct colors or props. For State Item `<All>` / Iterate, verify offsets zero, one, count, and count plus one. Add a Mark Collection with `Open,Unknown,,Closed`, plus a clipped or gapped mark, and verify blank timing shifts with offset. Add Custom rows with repeated values, `<None>`, a missing/deleted selection if practical, and different override colors; verify individual and grouped mode separately. Switch each scenario to Default and verify Cycle Offset disappears and current behavior remains simultaneous. Record the observed results in this plan.

As the final execution step, update Jira issue VIX-3951's **description** using the project Jira skill. Before writing, retrieve the issue through the configured Atlassian connector to confirm the key and current description. Replace or revise the description to include these headings and final evidence:

    Refined Requirements
    Acceptance Criteria
    Test Plan

The content must be derived from this plan's final implemented behavior and recorded validation, not copied as unverified future-tense work. Keep the issue status unchanged. Re-read VIX-3951 after the update and record the result in the Jira evidence placeholder below.

## Validation and Acceptance

Automated acceptance requires all of the following.

- A new `StateData` has zero Cycle Offset; cloning retains the raw integer, including values larger than a current slot count; deserializing old data with no member remains zero.
- The public State editor property has XML documentation, resource-backed display text `Cycle Offset`, a concise resource-backed description, SliderEditor metadata, and a `0..100` editor range.
- The editor shows Cycle Offset exactly for Iterate playback across State Item, Mark Collection, and Custom; Default hides it. A changed value marks the effect dirty and raises its property notification; same-value assignment is harmless.
- At offset zero, existing planner expectations are interval-for-interval identical: item identity, start, duration, ordering, and color override all match existing scheduling.
- A base list `[A, B, C, D]` with offset one schedules `[B, C, D, A]`; offset equal to count schedules the original list; count plus one schedules the offset-one list; and 100 uses modulo against the current base list.
- A base list `[A, B, C]`, offset one, and two iterations produces `[B, C, A, B, C, A]`. This proves offset precedes iteration repetition.
- Empty input returns no intervals without modulo/divide-by-zero. Singleton input remains unchanged for every valid editor offset.
- State Item `<All>` keeps all exact-name rows together as one offset slot. A specifically selected State Item remains its existing full-duration exact-name group.
- Mark Collection Iterate keeps unknown and empty parsed segments as blank but timing-consuming slots after rotation. Mark clipping, gaps, overlaps, and Default behavior remain unchanged.
- Custom individual mode rotates full rows, including duplicate, `<None>`, and missing selections, without losing their time slots or colors. Grouped Custom mode rotates only completed consecutive groups and preserves group membership, row order, and row color overrides.
- A duration with a nonzero tick remainder still assigns the remainder to the final chronological output slot. Rotation changes slot contents only, not boundaries or total duration.

Manual acceptance requires the State Effect Editor and rendered playback to match the automated assertions. The tester should be able to see the first state advance by one for each offset increment, wrap back to the original after a full base count, see blank Mark Collection/Custom slots move in time, see Custom group members activate together after rotation, and observe no Cycle Offset control in Default mode. The State effect's existing visual representation text must not mention the numeric offset or otherwise change when only Cycle Offset changes.

Tracker acceptance requires the final Jira VIX-3951 description to contain clearly labeled Refined Requirements, Acceptance Criteria, and Test Plan sections. Those sections must accurately reflect the implemented Cycle Offset behavior and the actual automated/manual validation evidence from this plan. Updating the description is the final step; it must not transition the issue without separate user authorization.

## Idempotence and Recovery

All source and test edits in this plan are additive and can be applied repeatedly if the executor first checks whether a member, resource key, or test already exists. Do not add duplicate `.resx` keys or duplicate test names. Do not use a destructive reset to discard user work; inspect `git status --short` and scope diffs to the listed task files.

If a planner signature change makes existing tests fail to compile, prefer updating all internal State planner callers to pass the new argument. A forwarding overload that supplies `0` is acceptable only as a compatibility seam for tests and must not hide the production raw-value path. If overload ambiguity appears, remove the ambiguous form rather than introducing optional parameters that make a missed Cycle Offset argument silently compile.

If an offset test changes duration boundaries, restore the existing `GetIntervalDuration` call inputs first. The output loop index, not the rotated source index, must determine duration and final remainder. If a Mark Collection test loses an empty/unknown timing share, ensure rotation is applied to the parser's entire `names` list before the existing renderability check. If grouped Custom tests split groups, ensure `CreateCustomStateItemGroups` runs before index offset selection and that rotation indexes `groups`, not `customStateItems`.

If project resources fail to load in the Effect Editor, compare the new XML entries with the existing `StateIterations` and `StateCycleIndividually` keys; do not substitute a literal display attribute because the State module already uses localized descriptors. If build/test restore is blocked, do not delete caches or lock files; record the exact blocker and run any already-restored focused command that is still available.

## Artifacts and Notes

The intended indexed selection is shown here as pseudocode. It is not a requirement to use these exact method or variable names, but it defines the required ordering and defensive behavior:

    if (slotCount == 0)
    {
        return no intervals;
    }

    normalizedOffset = cycleOffset > 0 ? cycleOffset % slotCount : 0;

    for each chronological output index i
    {
        duration = GetIntervalDuration(effectDuration, outputSlotCount, i, elapsed);
        sourceIndex = ((i % slotCount) + normalizedOffset) % slotCount;
        sourceSlot = baseSlots[sourceIndex];
        render sourceSlot using elapsed and duration;
        elapsed += duration;
    }

For a three-slot base list and two iterations, the source-index sequence should be:

    Base list:             [A, B, C]
    Offset:                1
    Output indices:        0  1  2  3  4  5
    Source indices:        1  2  0  1  2  0
    Rendered slot values:  B  C  A  B  C  A

The State source-specific base slots are:

    State Item `<All>`: unique exact item names in State-definition order.
    Mark Collection Iterate: parsed comma-delimited segments for one clipped mark, including empty and unknown text.
    Custom Iterate, individual: persisted custom rows in order.
    Custom Iterate, grouped: completed consecutive groups of persisted rows.

Do not rotate any of these prematurely. In particular, it is wrong to rotate `definition.Items` before unique-name grouping, to remove Mark Collection blank segments before counting, to rotate Custom rows before building groups, or to rotate a repeated `slotCount * iterations` list.

Record actual validation output here as the work proceeds.

Focused State test evidence:

    Command: dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore
    Result: Passed — Failed: 0, Passed: 79, Skipped: 0, Total: 79. Existing NU1904 LiteDB vulnerability advisories were emitted.

State module build evidence:

    Command: dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Result: Build succeeded — 0 errors. Existing nullable, obsolete-event, unused-event, and FixtureGraphics equality warnings were emitted.

Broad regression evidence:

    Command: pending
    Result: pending

Manual validation evidence:

    Result: pending
    Scenarios: State Item `<All>`, Mark Collection recognized/unknown/empty segments, Custom individual, Custom grouped, wrap-around, Default-mode visibility, and visual text unchanged.

Jira description-update evidence:

    Issue: VIX-3951
    Command/action: pending final Jira description update through the project Jira skill.
    Result: pending
    Required record: update time, description field changed, Refined Requirements/Acceptance Criteria/Test Plan sections present, and issue status unchanged.

## Interfaces and Dependencies

The final persisted public API in `src/Vixen.Modules/Effect/State/StateData.cs` must include a documented member equivalent to:

    [DataMember]
    public int CycleOffset { get; set; }

The value is an author-configured raw slot offset. Its default is zero. It must not clamp to 100, modulo by any current slot count, or mutate while rendering. The effect editor, rather than the persistence type, limits normal user entry to zero through 100.

The final public API in `src/Vixen.Modules/Effect/State/State.cs` must include a documented member equivalent to:

    [Value]
    [ProviderCategory("Config", 2)]
    [ProviderDisplayName("StateCycleOffset")]
    [ProviderDescription("StateCycleOffset")]
    [PropertyEditor("SliderEditor")]
    [NumberRange(StateData.MinCycleOffset, StateData.MaxCycleOffset, 1)]
    public int CycleOffset { get; set; }

The exact resource key may differ if the existing resource naming convention demands it, but display and description must be resource-backed. `SetRenderSourceBrowsables` must include `nameof(CycleOffset)` and set it true exactly when `PlaybackMode == PlaybackMode.Iterate`.

`State.CreateRenderIntervals` must pass the raw property to the State Item, Mark Collection, and Custom planner paths. `StateRenderPlanner` remains internal; do not make new planner helpers public merely for tests. The planner's private helper takes or otherwise uses an output index, positive base slot count, and raw offset to select an index. It must preserve the output index for `GetIntervalDuration`.

No NuGet packages, new projects, changes to `StateRenderSource`, changes to `PlaybackMode`, changes to State assignment data, or synchronization primitives are needed. Continue using the existing .NET, Catel/Effect Editor, State property, Marks, and xUnit dependencies already referenced by the State module and test project.

## Revision Notes

- 2026-07-27 / Codex: Created the initial ExecPlan from `docs/effects/VIX-3951-state-offset-spec.md`. The plan records the approved raw-persistence rule, Iterate-only visibility, completed-slot offset ordering, all three source-specific scheduling rules, and the required automated/manual validation so implementation can proceed without external context.
- 2026-07-27 / Codex: Revised the plan to make updating Jira issue VIX-3951's description the final execution step. The update must contain the final refined requirements, acceptance criteria, and test plan, with recorded evidence and no issue transition unless separately authorized.
- 2026-07-27 / Codex: Completed Milestone 1. Inspected the required State documentation, project C# and XML documentation skills, State data/editor/planner code, Effect Editor resources, and focused State tests. Confirmed the existing completed-slot collections and modulo sites without modifying production behavior.
- 2026-07-27 / Codex: Completed Milestone 2. Added raw persisted `CycleOffset`, editor range/metadata, Iterate-only visibility, clone support, and focused contract tests without changing planner scheduling.

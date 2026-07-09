# Implement State Effect Custom Group Cycling

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Any contributor implementing this plan must keep this document self-contained, update it at every stopping point, and record new decisions or discoveries as they occur.

## Purpose / Big Picture

After this change, a Vixen sequence author using the State effect can decide how `Render Source = Custom` behaves in `Playback Mode = Iterate`. The current behavior cycles every custom row individually. The new `Cycle Individually` checkbox keeps that behavior when checked, and when unchecked it groups consecutive custom rows with the same State item name so they render together in one time slot. This lets a user create multiple custom rows for the same named State item, with different row colors, and have them activate together before moving to the next consecutive name group.

The behavior is visible in the standard Effect Editor. Add a State effect to a target with a configured State property, set `Render Source` to `Custom`, set `Playback Mode` to `Iterate`, and edit the `Custom State Items` collection. With `Cycle Individually` checked, rows such as `Item 2`, `Item 2`, `<None>`, `Item 4` occupy four slots. With it unchecked, the first two `Item 2` rows occupy one slot, `<None>` occupies one silent slot, and `Item 4` occupies the next slot. The timeline visual text should show `Custom Group` when grouped mode is active.

## Progress

- [x] (2026-07-09 00:00 -05:00) Created this ExecPlan from `docs/state/vix-3924-state-effect-phase-3.md`, current State effect source, and `.agents/PLANS.md`.
- [x] (2026-07-09 00:35 -05:00) Implemented persisted `CycleIndividually` data, editor property, conditional visibility, editor order, and `Custom Group` visual text.
- [ ] Implement grouped Custom Iterate scheduling while preserving current individual and Default behavior.
- [x] (2026-07-09 00:35 -05:00) Added focused Milestone 1 tests for data defaults/cloning, editor visibility/order, metadata text, and visual text.
- [ ] Add focused tests for grouped interval scheduling and unchanged Custom Default behavior.
- [x] (2026-07-09 00:40 -05:00) Ran Milestone 1 focused tests, State effect build, and whitespace checks; recorded evidence below.

## Surprises & Discoveries

- Observation: `StateRenderPlanner.CreateCustomIntervals` currently receives only the State definition, custom rows, playback mode, iterations, and effect duration, so grouped behavior requires either a new parameter or a separate planner method.
  Evidence: `src/Vixen.Modules/Effect/State/State.cs` calls `StateRenderPlanner.CreateCustomIntervals(selectedDefinition, _data.CustomStateItems ?? [], PlaybackMode, Iterations, TimeSpan)`.

- Observation: The current Custom Iterate path already preserves timing for `<None>` and missing rows because it uses `customStateItems.Count * Iterations` as the slot count and advances the start time for every row, even when no interval is emitted.
  Evidence: `StateRenderPlanner.CreateIteratedCustomIntervals` increments `intervalStart` for every indexed custom row and emits an interval only when the row ID resolves to a State item.

- Observation: The Effect Editor visibility for State effect properties is centralized in `State.SetRenderSourceBrowsables`.
  Evidence: The method currently controls `StateItem`, `MarkCollectionId`, `CustomStateItems`, and `Iterations`.

- Observation: `CustomStateItemCollection` inherited a base minimum item count of `1`, while the existing State tests expected a detached collection to allow zero items.
  Evidence: The first Milestone 1 focused test run failed `CustomStateItemCollection_AllowsEmptyCollection`, and `ExpandoObjectObservableCollection.GetMinimumItemCount()` returns `1` by default. The collection now overrides the method to return `1` only when the parent State effect is using `Render Source = Custom`; otherwise it returns `0`.

- Observation: The Effect Editor display/description provider falls back to the supplied key when a resource entry is missing.
  Evidence: `EffectResourceManager.GetDisplayNameString` and `GetDescriptionString` return `ResourceManager.GetString(key) ?? key`, so `ProviderDisplayName(@"Cycle Individually")` and a sentence-valued `ProviderDescription` avoid broad shared `.resx` edits while still displaying user-facing text.

## Decision Log

- Decision: Persist the new option as `StateData.CycleIndividually` with a default value of `true`.
  Rationale: Existing saved data will deserialize without the new field and must keep current Custom Iterate behavior. A `true` initializer on the data property preserves that behavior.
  Date/Author: 2026-07-09 / Codex

- Decision: Add a `cycleIndividually` Boolean parameter to `StateRenderPlanner.CreateCustomIntervals` instead of adding a new public planner method.
  Rationale: The existing planner method is already the single entry point for Custom interval scheduling. A parameter keeps the call path explicit and limits edits to the State effect.
  Date/Author: 2026-07-09 / Codex

- Decision: In grouped mode, valid rows group by resolved `StateItemData.Name`, `<None>` rows group by a dedicated none key, and missing rows group by their missing State item ID.
  Rationale: The feature requirement says valid rows group by exact name, consecutive `<None>` rows collapse together, and unrelated missing IDs should not merge just because they no longer resolve.
  Date/Author: 2026-07-09 / Codex

- Decision: Grouped Custom mode renders only the custom rows the user added and does not expand a row to every State definition item with the same name.
  Rationale: Custom rows are explicit user-selected rows with explicit color overrides. Expanding by name would add output the user did not add to the collection.
  Date/Author: 2026-07-09 / Codex

- Decision: Use literal provider keys for the `Cycle Individually` display name and description rather than editing the shared Effect Editor resource files.
  Rationale: The provider already falls back to the key string. Literal keys keep the UI text correct and avoid noisy `.resx` whitespace churn unrelated to the State effect implementation.
  Date/Author: 2026-07-09 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. The State effect now persists and clones `CycleIndividually`, exposes `Cycle Individually` in the Effect Editor only for Custom Iterate mode, orders it before `Custom State Items`, and changes Custom Iterate grouped visual text to `Custom Group` when unchecked. Focused tests cover the data, editor descriptor, and visual text behavior. The main remaining risks are preserving exact existing behavior when `Cycle Individually = true` and correctly consuming timing for silent groups during Milestone 2.

## Context and Orientation

Vixen is a Windows desktop .NET/WPF application for sequencing animated light shows. Effects live under `src/Vixen.Modules/Effect`. The State effect module is in `src/Vixen.Modules/Effect/State` and renders named State property assignments from `src/Vixen.Modules/Property/State`.

The State effect has three render sources. `StateRenderSource.StateItem` renders one selected State item name or all names. `StateRenderSource.MarkCollection` reads mark text from a Mark Collection. `StateRenderSource.Custom` renders the ordered `Custom State Items` collection configured directly on the effect. This plan changes only the Custom render source when `PlaybackMode.Iterate` is active.

`PlaybackMode.Default` means selected items render for the full effect duration. `PlaybackMode.Iterate` means items are divided into sequential timing slots within the effect duration. `Iterations` repeats that sequence inside the same duration; it does not extend the effect.

The relevant files are:

- `src/Vixen.Modules/Effect/State/StateData.cs`: persisted data for the effect. It currently stores selected State definition IDs, render source, playback mode, iterations, visual setting, and serialized custom rows.
- `src/Vixen.Modules/Effect/State/State.cs`: editor-facing properties, render interval dispatch, custom item collection handling, and visual representation text.
- `src/Vixen.Modules/Effect/State/StateRenderPlanner.cs`: internal scheduling helper that creates `StateRenderInterval` entries for State Item, Mark Collection, and Custom sources.
- `src/Vixen.Modules/Effect/State/StateRenderInterval.cs`: record describing one State item, start time, duration, and optional color override.
- `src/Vixen.Modules/Effect/State/CustomStateItemData.cs`: serialized custom row with `StateItemId` and `Color`.
- `src/Vixen.Tests/Effect/State/StateRenderPlannerTests.cs`: focused tests for State render planning.

The existing Custom Iterate implementation in `StateRenderPlanner.CreateIteratedCustomIntervals` treats every custom row as one timing slot. Rows with `Guid.Empty`, which represent `<None>` in Iterate mode, and rows with missing/deleted item IDs consume timing but emit no interval. Custom Default behavior is different: it skips `<None>`, missing rows, and duplicate IDs, then renders valid rows for the full effect duration. This plan must not change Custom Default behavior.

The phrase "grouped mode" in this plan means all of these are true: `Render Source = Custom`, `Playback Mode = Iterate`, and `Cycle Individually = false`.

## Plan of Work

First, update the persisted data. In `src/Vixen.Modules/Effect/State/StateData.cs`, add a public `bool CycleIndividually { get; set; } = true` property with `[DataMember]` and XML documentation. Copy it in `CreateInstanceForClone`. The property is public because the existing editor-facing effect data uses public serialized properties. Keep the default initializer so existing profiles missing the field behave as checked.

Next, expose the editor option. In `src/Vixen.Modules/Effect/State/State.cs`, add a public `CycleIndividually` property that reads and writes `_data.CycleIndividually`. Mark it with the same editor metadata pattern as other State effect configuration properties: `[Value]`, `[ProviderCategory("Config", 2)]`, `[ProviderDisplayName(@"CycleIndividually")]`, `[ProviderDescription(@"CycleIndividually")]`, and `[RefreshProperties(RefreshProperties.All)]` only if needed for editor refresh. The setter should return when the value is unchanged; otherwise set the data field, set `IsDirty = true`, call `OnPropertyChanged()`, and call `OnPropertyChanged(nameof(ForceGenerateVisualRepresentation))` or `TypeDescriptor.Refresh(this)` if needed to update the timeline visual/editor. Give it a `PropertyOrder` immediately before `CustomStateItems`. Because `CustomStateItems` currently uses `PropertyOrder(6)`, a practical implementation is to assign `CycleIndividually` order `5` or move `CustomStateItems` to a later order if needed after checking the editor's sort behavior. Do not disrupt the visible order of unrelated properties.

Update `SetRenderSourceBrowsables` in `State.cs` so `CycleIndividually` is visible only when `RenderSource == StateRenderSource.Custom && PlaybackMode == PlaybackMode.Iterate`. Keep `CustomStateItems` visible whenever `RenderSource == StateRenderSource.Custom`, and keep `Iterations` visible whenever `PlaybackMode == PlaybackMode.Iterate`, matching current behavior.

Update visual text in `State.GetVisualRepresentationText`. Today `StateRenderSource.Custom` returns `State - {StateDefinition} - Custom`. Change it so grouped mode returns `State - {StateDefinition} - Custom Group`, and all other Custom states still return `State - {StateDefinition} - Custom`. Grouped mode is active only when `RenderSource == StateRenderSource.Custom`, `PlaybackMode == PlaybackMode.Iterate`, and `CycleIndividually == false`.

Then update render planning. Change `State.CreateRenderIntervals` so the Custom call passes `_data.CycleIndividually` into `StateRenderPlanner.CreateCustomIntervals`. Change the planner signature accordingly:

    internal static IReadOnlyList<StateRenderInterval> CreateCustomIntervals(
        StateDefinitionData? definition,
        IReadOnlyList<CustomStateItemData> customStateItems,
        PlaybackMode playbackMode,
        int iterations,
        bool cycleIndividually,
        TimeSpan effectDuration)

or put `cycleIndividually` before `iterations` if the surrounding code reads better. Update every test call site.

Inside `StateRenderPlanner.CreateCustomIntervals`, keep all existing early returns. Keep Custom Default routing exactly as it is today. For Iterate mode, call the existing individual scheduler when `cycleIndividually` is true. When it is false, call a new private helper such as `CreateGroupedCustomIntervals`.

The grouped helper should build consecutive groups from the original custom row list. A valid row's group key is the resolved `StateItemData.Name`, using exact ordinal string equality. A `<None>` row has `StateItemId == Guid.Empty` and should use one shared none key so consecutive `<None>` rows collapse together. A missing row has a non-empty `StateItemId` that does not exist in `itemsById` and should use a key that includes that missing GUID so consecutive rows with the same missing ID collapse, while different missing IDs remain separate. The implementation can avoid adding new types by using small private helper methods and `List<List<CustomStateItemData>>`; avoid adding extra helper classes unless they are placed in their own file according to repository style.

After groups are built, calculate `intervalCount = groups.Count * StateData.NormalizeIterations(iterations)`. Iterate through that count using the existing `GetIntervalDuration` helper so tick rounding remains consistent with the current implementation. For each group slot, walk only the custom rows in that group. Emit a `StateRenderInterval` only for rows whose `StateItemId` is non-empty and resolves in `itemsById`. Use that row's `Color` as the interval color override. Always advance `intervalStart` once per group slot, even when the group contains only `<None>` or missing rows and emits no interval.

Do not expand a valid custom row to other `StateItemData` entries with the same name. The group key controls timing only. The rendered intervals come from the row's exact `StateItemId`.

Finally, add tests. Extend `src/Vixen.Tests/Effect/State/StateRenderPlannerTests.cs` with focused tests that call the planner directly. Add a State data test file under `src/Vixen.Tests/Effect/State` if no suitable file exists for `StateData` clone/default tests. Add State effect tests for editor visibility/order and visual text only if there is an existing lightweight pattern for inspecting property descriptors; otherwise cover visual text directly through the internal `GetVisualRepresentationText` helper and record any editor descriptor test gap in this plan. Since `State.GetVisualRepresentationText` is internal and the State effect already exposes internals to `Vixen.Tests`, it can be tested directly.

## Milestones

Milestone 1 adds the data and editor surface. At the end of this milestone, the effect has a persisted `CycleIndividually` option that defaults to checked, clones correctly, is visible only for Custom Iterate, appears immediately before the custom row collection, and changes the visual text to `Custom Group` in grouped mode. Run focused State effect tests after adding data and visual tests. Acceptance is that the new property exists, default and clone tests pass, and manual property descriptor inspection or automated descriptor tests confirm the conditional visibility and order.

Milestone 2 implements grouped scheduling in the render planner. At the end of this milestone, `CycleIndividually = true` still produces the same intervals as the current behavior, while `CycleIndividually = false` collapses consecutive name groups, consecutive `<None>` groups, and matching missing-ID groups into timing slots. Run the `Effect.State` filtered tests. Acceptance is that new planner tests demonstrate the examples from `docs/state/vix-3924-state-effect-phase-3.md`, including iterations repeating groups inside the same duration.

Milestone 3 validates integration and records evidence. At the end of this milestone, the State effect project builds, focused tests pass, whitespace checks pass, and this ExecPlan contains the command output summaries. Manual acceptance can be performed in Vixen by placing the State effect on a prop with a State property and observing the Custom Iterate timing and timeline text.

## Concrete Steps

Work from the repository root:

    C:\Dev\Vixen

Read the source files before editing:

    Get-Content -Raw -LiteralPath docs\state\vix-3924-state-effect-phase-3.md
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\State\StateData.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\State\State.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\State\StateRenderPlanner.cs
    Get-Content -Raw -LiteralPath src\Vixen.Tests\Effect\State\StateRenderPlannerTests.cs

Implement Milestone 1 in `StateData.cs` and `State.cs`. Add XML documentation for public properties. Preserve CRLF line endings and tabs for indentation.

Implement Milestone 2 in `StateRenderPlanner.cs` and update the Custom call in `State.cs`. Keep the individual iterate helper behavior intact for `cycleIndividually = true`; this is the compatibility path.

Add or update focused tests under:

    src\Vixen.Tests\Effect\State

Run focused tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore

Expected successful output should include a summary like:

    Passed!  - Failed: 0, Passed: <N>, Skipped: 0, Total: <N>

Build the State effect project:

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore

Expected output should end with:

    Build succeeded.
    0 Error(s)

Run whitespace validation:

    git diff --check

Expected output is empty and the exit code is 0. If Git prints CRLF normalization warnings but exits successfully, record that in `Artifacts and Notes`.

If broader validation is desired after focused tests pass, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State" --no-restore

This should pass because the State effect must remain compatible with State property behavior.

## Validation and Acceptance

Automated acceptance is met when tests prove these behaviors:

New `StateData` instances have `CycleIndividually == true`.

Cloning `StateData` preserves `CycleIndividually`.

Custom Iterate with `cycleIndividually = true` produces the same row-by-row timing behavior as before. This includes `<None>` and missing rows consuming timing and rendering no output.

Custom Iterate with `cycleIndividually = false` collapses consecutive valid rows with the same exact resolved `StateItemData.Name` into one slot. Matching is ordinal and case-sensitive.

Grouped valid rows render only the exact custom rows the user added, using each row's selected `StateItemId` and row color override. A duplicate-name State definition item not present in `Custom State Items` must not render.

Non-consecutive rows with the same name do not merge.

Consecutive `<None>` rows collapse into one timing slot and render no output.

Rows with missing/deleted State item IDs consume timing in grouped mode. Consecutive rows with the same missing ID collapse, and different missing IDs do not collapse merely because both are missing.

`Iterations` repeats grouped timing entries inside the same total duration. For three groups and `Iterations = 2`, there are six timing slots.

Custom Default behavior is unchanged. It still renders valid unique rows for the full duration and skips `<None>`, missing, and duplicate IDs as before.

The Effect Editor descriptor shows `Cycle Individually` only when `Render Source = Custom` and `Playback Mode = Iterate`, and it appears immediately before `Custom State Items`.

`State.GetVisualRepresentationText()` returns text ending in `Custom Group` only when grouped mode is active. Other Custom states still end in `Custom`.

Manual acceptance is met with this scenario:

1. Start Vixen from a Debug build.
2. Open a profile with a prop that has a State property containing at least `Item 2`, `Item 4`, and enough assigned elements to see output.
3. Add a State effect to that prop.
4. Set `Render Source = Custom` and `Playback Mode = Iterate`.
5. Add custom rows `Item 2`, `Item 2`, `Item 2`, `<None>`, `Item 4`, `Item 4`, `Item 4`.
6. With `Cycle Individually` checked, confirm the rows cycle through seven equal timing slots.
7. Uncheck `Cycle Individually`. Confirm the timeline visual text includes `Custom Group`.
8. Render or play the effect. Confirm the first three `Item 2` rows activate together with their custom colors, the `<None>` slot is silent, and the three `Item 4` rows activate together.
9. Set `Iterations = 2` and confirm the grouped sequence repeats twice inside the same effect duration.

## Idempotence and Recovery

These changes are additive and can be implemented incrementally. Running tests repeatedly is safe. The new persisted field defaults to `true`, so existing data without the field should keep current behavior.

If tests show that `CycleIndividually = true` changed current Custom Iterate timing, restore the original `CreateIteratedCustomIntervals` path and route only `cycleIndividually = false` through the new grouped helper.

If grouped scheduling emits extra duplicate-name State items, inspect the grouped helper and ensure it emits intervals only for the `CustomStateItemData` rows inside the group. Do not reuse the State Item `<All>` grouping logic, because that expands by name and is intentionally different from Custom grouped mode.

If editor ordering is difficult to test directly, verify with `TypeDescriptor.GetProperties(new State())` in a focused test or temporary diagnostic, then remove any diagnostic code before completion. Record the final evidence in this plan.

If a broad test run fails outside `Effect.State` or `Property.State`, record the failure summary here and rerun the focused test that covers this change before changing unrelated code.

## Artifacts and Notes

Source facts gathered before implementation:

    src/Vixen.Modules/Effect/State/StateData.cs currently stores Iterations, ShowEffectVisual, and CustomStateItems, but no CycleIndividually field.

    src/Vixen.Modules/Effect/State/State.cs currently routes Custom rendering through StateRenderPlanner.CreateCustomIntervals without a cycle option.

    src/Vixen.Modules/Effect/State/State.cs currently returns "State - {StateDefinition} - Custom" for every Custom visual representation.

    src/Vixen.Modules/Effect/State/StateRenderPlanner.cs currently uses customStateItems.Count * iterations for every Custom Iterate schedule.

Record validation evidence here as implementation proceeds. Keep transcripts concise:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore
    Milestone 1: Passed! - Failed: 0, Passed: 67, Skipped: 0, Total: 67.

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Milestone 1: Build succeeded with 0 warnings and 0 errors.

    git diff --check
    Milestone 1: Exited successfully. Git printed the expected local warning that `src/Vixen.Modules/Effect/State/State.cs` will be normalized from LF to CRLF the next time Git touches it; no whitespace errors were reported.

## Interfaces and Dependencies

Use existing State effect and State property types. Do not add new project references.

At completion, `src/Vixen.Modules/Effect/State/StateData.cs` must expose and clone:

    public bool CycleIndividually { get; set; } = true;

The property must have `[DataMember]` and XML documentation.

At completion, `src/Vixen.Modules/Effect/State/State.cs` must expose:

    public bool CycleIndividually { get; set; }

The property must have Effect Editor metadata consistent with other State effect config properties and must be included in `SetRenderSourceBrowsables` with this condition:

    RenderSource == StateRenderSource.Custom && PlaybackMode == PlaybackMode.Iterate

At completion, `StateRenderPlanner.CreateCustomIntervals` must accept the cycle option and preserve existing behavior when it is `true`:

    internal static IReadOnlyList<StateRenderInterval> CreateCustomIntervals(
        StateDefinitionData? definition,
        IReadOnlyList<CustomStateItemData> customStateItems,
        PlaybackMode playbackMode,
        int iterations,
        bool cycleIndividually,
        TimeSpan effectDuration)

The exact parameter order can be adjusted during implementation, but all call sites and tests must read clearly and consistently.

The grouped helper must remain internal or private. If any new public or protected C# API is introduced, add XML documentation immediately according to `.agents/skills/csharp-docs/SKILL.md`.

## Revision Notes

- 2026-07-09 / Codex: Initial ExecPlan created from the reviewed Phase 3 design specification. The plan records the key design choices for default compatibility, grouped Custom timing, missing row grouping, editor visibility, visual text, and focused validation.
- 2026-07-09 / Codex: Completed Milestone 1 by adding the persisted/editor-facing `CycleIndividually` option, conditional browsability, visual `Custom Group` text, focused data/editor/visual tests, and validation evidence.

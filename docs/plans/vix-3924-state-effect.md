# Implement VIX-3924 State Effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Any contributor implementing this plan must keep this document self-contained, update it at every stopping point, and record new decisions or discoveries as they occur.

## Purpose / Big Picture

After this work, a sequence author can place a new `State` effect on a prop that has the State property, choose one configured State definition such as `Arm Position`, and either render one State item group for the full effect duration or drive State item names from a Mark Collection over time. This lets non-face props reuse named State definitions created in Display Setup, so a waving Santa, decorated model, or any custom prop can activate authored element/color assignments without manually layering several Set Level effects.

The behavior is visible in the standard Effect Editor. Add the State effect to a target tree with a configured State property, select a State definition in the `State` combo box, choose `State Item` or `Mark Collection` as the render source, and render the sequence. The effect should light only the assigned leaf nodes from the selected State definition, using each State item's configured color and respecting discrete-color element restrictions.

## Progress

- [x] (2026-06-08 10:35 -05:00) Read `.agents/PLANS.md`, `docs/vix-3924-state-effect.md`, `docs/vix-3591-state-property-final-requirements.md`, the partial State effect under `src/Vixen.Modules/Effect/State`, State property models under `src/Vixen.Modules/Property/State`, and comparable effects/converters including LipSync, Alternating, CustomValue, `BaseEffect`, and `EffectListTypeConverterBase`.
- [x] (2026-06-08 10:35 -05:00) Created this implementation plan with milestones, Jira paste text, risks, concrete file targets, validation commands, and acceptance criteria.
- [x] (2026-06-08 10:45 -05:00) Updated `docs/vix-3924-state-effect.md` and this ExecPlan so removed Mark Collection selections follow LipSync and Alternating: remove listeners, clear the selected Mark Collection ID, and render nothing until a new collection is selected.
- [x] (2026-06-08 10:58 -05:00) Completed Milestone 2. `StateData` now persists selected State definition, State property, owner element, render source, selected State item, Mark Collection, and playback mode. Added `StateRenderSource`, removed `TimingMode`, added the State property project reference and `InternalsVisibleTo`, and corrected the State effect `Release|Any CPU` solution mapping to `Release|x64`.
- [x] (2026-06-08 11:05 -05:00) Completed Milestone 1. Jira issue VIX-3924 was updated with the plan summary, design, acceptance criteria, risks, and testing notes.
- [x] (2026-06-08 11:04 -05:00) Completed Milestone 3. Added State definition discovery across target trees, duplicate display-label disambiguation, State and State Item combo converters/options, missing/no-option placeholders, first-definition auto-selection, selected State item preservation by matching name, and render-source-specific browsability for State Item versus Mark Collection.
- [x] Replace the partial State effect data model and editor metadata with durable State definition/item identity, render source, playback mode, and dynamic combo box options.
- [x] Implement State definition discovery across target trees and missing-selection/no-option handling.
- [x] (2026-06-08 11:23 -05:00) Completed Milestone 4. Added State Item render planning for `Default`, `Iterate`, `<All>`, selected item anchors by ID, duplicate item names, missing anchors, and exact case-sensitive names. Wired State Item intervals into rendering with assignment lookup scoped to the effect target trees, render-time leaf expansion, deleted/out-of-scope assignment skipping, discrete-color fallback to the first supported color, and per-pass `_elementData` reset.
- [x] Implement State Item rendering for `Default`, `Iterate`, `<All>`, duplicate item names, group expansion, discrete-color fallback, and render-pass reset.
- [ ] Implement Mark Collection rendering, mark parsing, clipping, invalid-selection behavior, listener refresh, and dirty-state invalidation.
- [ ] Implement contiguous intent coalescing and the CustomValue-style visual representation.
- [ ] Add focused tests for discovery, editor option generation, rendering, marks, discrete colors, coalescing, and invalid selections.
- [ ] Run filtered and full validation commands, record evidence, and update this plan's outcome sections.

## Surprises & Discoveries

- Observation: The partial effect exists and builds, but it currently contains only Mark Collection scaffolding plus an empty `RenderNodes` placeholder.
  Evidence: `src/Vixen.Modules/Effect/State/State.cs` creates `_elementData` in `_PreRender`, calls `RenderNodes()`, and `RenderNodes()` contains only comments.

- Observation: The partial effect uses `TimingMode.State`, `TimingMode.MarkCollection`, and `AllowMarkGaps`, but the final VIX-3924 requirements use the terms `Render Source`, `State Item`, `Mark Collection`, and `Playback Mode`. `AllowMarkGaps` is not part of the final effect contract because mark gaps always render nothing.
  Evidence: `docs/vix-3924-state-effect.md` requires `Render Source` with `State Item` and `Mark Collection`, `Playback Mode` with `Default` and `Iterate`, and says gaps between marks render nothing.

- Observation: The State property data needed by the effect already exists and exposes stable IDs. `StateData.Id` is the attached property ID, `StateDefinitionData.Id` is the durable State definition ID, and `StateItemData.Id` is the durable State item ID. Assignments are stored as `StateItemData.ElementNodeIds`.
  Evidence: `src/Vixen.Modules/Property/State/StateData.cs`, `StateDefinitionData.cs`, and `StateItemData.cs` all initialize and serialize stable IDs.

- Observation: `StateModule.GetStateModuleForElement(IElementNode element)` is the right property lookup helper for discovery.
  Evidence: `src/Vixen.Modules/Property/State/StateModule.cs` returns `element.Properties.Get(StateDescriptor.ModuleId) as StateModule`.

- Observation: The existing `BaseEffect.CreateIntentsForElement` skips discrete leaves whose valid colors do not contain the requested color, but VIX-3924 requires falling back to the first supported discrete color. The State effect needs its own color resolution helper rather than using `CreateIntentsForElement` directly.
  Evidence: `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` continues when a discrete element does not support the requested color; `docs/vix-3924-state-effect.md` requires using the first supported color instead.

- Observation: The new State effect project is already present in `Vixen.sln`, but its solution configuration mappings should be verified during implementation because partial additions can leave invalid platform mappings.
  Evidence: `Vixen.sln` contains project `{011C7353-839B-4D5C-A673-482D47D5BE78}` for `src\Vixen.Modules\Effect\State\State.csproj`.

- Observation: `TypeDescriptor.Refresh(effect)` in the Effect Editor refreshed browsability but did not invalidate `PropertyItem.StandardValues`, so dependent combo boxes such as State Item did not call their type converters after another property changed.
  Evidence: `EffectPropertyEditorGrid.OnTypeDescriptorRefreshedInvoke` called `UpdateBrowsable()` only; `PropertyItem.OnComponentChanged()` is the code path that raises `StandardValues` changes.

## Decision Log

- Decision: Treat `StateDefinitionId` as the primary persisted selection and store `StatePropertyId` and `StateOwnerElementId` only as optional lookup accelerators.
  Rationale: VIX-3924 explicitly says State definition names are display values only and that the selected definition ID is the durable identity. Property/owner IDs can speed rediscovery but must not decide correctness if the definition ID is found elsewhere within the targeted trees.
  Date/Author: 2026-06-08 / Codex

- Decision: Replace `TimingMode` with a requirement-aligned `RenderSource` enum and keep `PlaybackMode` as the `Default`/`Iterate` enum.
  Rationale: The partial `TimingMode.State` name obscures the final editor semantics. Using the requirement terms makes editor labels, persisted data, and tests easier to reason about.
  Date/Author: 2026-06-08 / Codex

- Decision: Persist the selected State item by anchor ID and expose the current anchor name through the editor. `<All>` is represented by an empty `Guid` in the data model.
  Rationale: VIX-3924 requires renamed State item anchors to remain selected by ID and render every item in the renamed exact-name group. `Guid.Empty` is already used elsewhere in Vixen effect data as an unset sentinel and cleanly distinguishes `<All>` from a real item ID.
  Date/Author: 2026-06-08 / Codex

- Decision: Keep discovery, option-label generation, mark parsing, interval planning, leaf/color resolution, and coalescing in small internal helper types under the State effect project, instead of one large `State.cs` method.
  Rationale: This feature has many edge cases and needs focused unit tests. Internal helpers can be tested through `InternalsVisibleTo` without exposing new public API.
  Date/Author: 2026-06-08 / Codex

- Decision: Do not add a new `MarkCollectionType.State` in this milestone.
  Rationale: The requirement explicitly says adding a State Mark Collection type is not required. The effect should prefer such collections only if a future change adds that enum value.
  Date/Author: 2026-06-08 / Codex

- Decision: Follow LipSync and Alternating behavior when a selected Mark Collection is removed by clearing the selected Mark Collection ID after removing listeners.
  Rationale: Mark Collection removal should be universal across effects and should not deviate from established Vixen patterns. Clearing the selected ID prevents a stale reference while still rendering nothing until the user chooses another collection.
  Date/Author: 2026-06-08 / Codex

## Outcomes & Retrospective

This plan has started implementation. The main remaining implementation risks are editor combo box behavior for missing selections, reliable unit-test construction of Vixen element trees and property attachments, and ensuring coalescing reduces intent count without changing ordering semantics. Record completed behavior, validation results, and any deferred gaps here after implementation milestones finish.

Milestone 2 is complete. The State effect now has the persisted identity and mode fields required for later discovery and rendering work, references the State property module, exposes internals to `Vixen.Tests`, and builds successfully in Debug x64. Rendering remains a placeholder and is intentionally deferred to later milestones.

Milestone 3 is complete. The effect now discovers State definitions from the current target nodes and descendants, exposes dynamic Effect Editor options for State definitions and State item names, keeps missing persisted selections visible, defaults to the first discovered definition when no persisted definition exists, and switches editor browsability between `State Item` and `Mark Collection` based on `Render Source`. Rendering remains a placeholder and is intentionally deferred to later milestones.

Milestone 4 is complete. State Item render mode now produces real intents from the selected State definition, supports full-duration and iterated State item intervals, preserves duplicate-name semantics, skips missing selected anchors plus deleted or out-of-scope assignments, expands assigned groups to leaves at render time, and applies the required discrete-color fallback. Mark Collection rendering, contiguous coalescing, visual representation, and focused tests remain deferred to later milestones.

## Context and Orientation

Vixen is a modular .NET 10 WPF application. Effects live under `src/Vixen.Modules/Effect` and are loaded through descriptors. The new effect project is `src/Vixen.Modules/Effect/State/State.csproj`; the module class is `VixenModules.Effect.State.State`; the descriptor is `StateDescriptor`; and the persisted effect data is `StateData` in the same folder. This effect extends `VixenModules.Effect.Effect.BaseEffect`, which provides common intent creation and color-property helpers for basic effects.

The State property is a separate property module under `src/Vixen.Modules/Property/State`. A property module is data attached to an `IElementNode`, which is a Vixen display tree node. A State property container is represented by `VixenModules.Property.State.StateModule` and stores `StateData`. That property data contains `StateDefinitionData` rows. Each State definition has stable `Id`, display `Name`, `Description`, and ordered `Items`. Each `StateItemData` has stable `Id`, display `Name`, `Color`, and `ElementNodeIds`, where each element-node ID points to an assigned display tree node. Assignments may point to groups or leaves. A leaf node is an `IElementNode` with no children and is the only node type that receives output intents.

The effect editor is driven by public properties on the effect class decorated with attributes from `Vixen.Attributes`, `Vixen.Sys.Attribute`, and `VixenModules.EffectEditor.EffectDescriptorAttributes`. Selection combo boxes normally use a `TypeConverter` that returns strings. Existing examples include `IMarkCollectionNameConverter` in `src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs` and effect-specific converters inheriting `EffectListTypeConverterBase<T>` in `src/Vixen.Modules/Effect/Effect/EffectListTypeConverterBase.cs`.

Rendering produces `EffectIntents`, which maps leaf element IDs to timed intent nodes. The effect base calls `_PreRender`, then `_Render`. A render pass must rebuild `_elementData` from scratch. Add intents with `EffectIntents.AddIntentForElement(leaf.Element.Id, intent, relativeStartTime)`, where `relativeStartTime` is relative to the effect start.

Mark Collections are label/timing collections exposed through `IEffect.MarkCollections`. Existing Mark Collection effects use `MarkCollectionId` as a public string property that displays names but persists the selected collection GUID. `BaseEffect` provides listener helpers: `AddMarkCollectionListeners`, `RemoveMarkCollectionListeners`, and `InitializeMarkCollectionListeners`. It also marks an effect dirty when watched marks inside the effect timespan change.

The partial implementation has these important current files:

`src/Vixen.Modules/Effect/State/State.cs` currently exposes `RenderSource` and `MarkCollectionId`, creates `_elementData`, and has an empty `RenderNodes` method. Later milestones add State definition and State item editor properties plus rendering behavior.

`src/Vixen.Modules/Effect/State/StateData.cs` now stores the selected State definition ID, containing State property ID, owner element ID, render source, selected State item ID, Mark Collection ID, and playback mode.

`src/Vixen.Modules/Effect/State/PlaybackMode.cs` already has `Default` and `Iterate`, matching the required playback modes.

`src/Vixen.Modules/Effect/State/StateRenderSource.cs` defines `StateItem` and `MarkCollection`. The old `TimingMode.cs` file was removed because the final requirement calls this concept `Render Source` and does not include `AllowMarkGaps`.

`src/Vixen.Modules/Effect/State/StateDescriptor.cs` already gives the effect name, type ID, module class, data class, `EffectGroups.Basic`, and `SupportsMarks = true`.

`src/Vixen.Modules/Effect/State/State.csproj` references Vixen.Core, the common Effect project, and `src/Vixen.Modules/Property/State/State.csproj`. It also declares `InternalsVisibleTo("Vixen.Tests")` so later internal helpers can be unit tested.

## Plan of Work

Milestone 1 updates Jira and records stakeholder-facing scope. Before changing code, update VIX-3924 in the VIX Jira project with the following markdown. If Jira tool access is available, use the project `jira` skill and paste this text into the issue description or a planning comment. If Jira is unavailable, paste it manually and record that fact in `Surprises & Discoveries`.

    ## VIX-3924 State Effect Implementation Plan

    ### Summary
    Implement a new standard `State` effect under `src/Vixen.Modules/Effect/State` that consumes the State property data completed by VIX-3591. The effect lets users select one State definition from the targeted display tree, then render State items either statically for the effect duration or from Mark Collection labels over time.

    ### High-Level Design
    The effect persists the selected State definition by stable `StateDefinitionData.Id`. It may also persist the containing State property ID and owner element ID as lookup accelerators, but rendering and selection validity are based on the State definition ID discovered inside the effect target trees.

    The Effect Editor exposes `State`, `Render Source`, `State Item` or `Mark Collection`, and `Playback Mode`, in that order. The State and State Item combo boxes use dynamic type converters based on the current target nodes and selected State definition. Missing selections remain visible as invalid placeholders and render nothing until corrected.

    Rendering resolves assignments from the selected State definition only. Each active State item expands assigned groups to current descendant leaves during each render pass, resolves discrete-color fallback per leaf, and adds static color intents. `Default` mode renders recognized names simultaneously. `Iterate` mode divides the selected duration equally across ordered names or mark text segments. Adjacent intervals for the exact same State item and leaf are coalesced when there is no gap.

    Mark Collection mode parses comma-delimited mark text. Matching is case-sensitive. Leading/trailing whitespace is trimmed. Unknown or empty segments render nothing. In `Default` mode recognized names are de-duplicated per mark. In `Iterate` mode every segment consumes its share, including unknown or empty segments. Removed Mark Collection selections are cleared using the established LipSync and Alternating pattern and then render nothing until the user selects another collection.

    ### Acceptance Criteria
    A user can add State to a prop with a State property, choose a State definition, choose `<All>` or a State item name, and render the assigned leaves with configured colors for the full effect duration.

    Duplicate State definition labels are disambiguated by owner element name and, if still needed, a short State definition ID suffix.

    A renamed selected State definition continues to work because selection is by ID. A deleted selected State definition renders nothing and shows an invalid selection until the user chooses another definition.

    A renamed selected State item anchor continues to work by ID and activates every item with the anchor's new exact name. A deleted selected State item anchor renders nothing until the user chooses a valid item or `<All>`.

    State Item mode supports `<All>` with `Default` and `Iterate`, duplicate item names, ordered overlaps, empty definitions, and no-State placeholders.

    Mark Collection mode supports whitespace trimming, case-sensitive names, unknown and empty segments, Default de-duplication, Iterate repeated names, gaps, overlaps, clipping, selected collection removal clearing, and dirty-state invalidation.

    Rendering affects only selected State definition assignments and always expands assignments to current leaf nodes at render time.

    Discrete-color leaves use the configured color if supported, otherwise the first supported color; discrete leaves with no supported colors are skipped.

    Each render pass starts with empty intents. Adjacent intervals for the exact same State item and leaf are merged only when contiguous and not separated by a gap.

    The sequence editor visual representation is a dark gray bar with white `State` text.

    ### Testing
    Add focused unit tests under `src/Vixen.Tests/Effect/State` for discovery, duplicate labels, selection retention, missing selections, editor option generation, State Item rendering, Mark Collection parsing/rendering, leaf expansion, discrete-color fallback, intent ordering, coalescing, and dirty invalidation.

    Run:
    `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"`
    `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State"`
    `dotnet test src/Vixen.Tests/Vixen.Tests.csproj`
    `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`
    `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release`
    `git diff --check`

    ### Risks and Concerns
    Mark Collection removal behavior must remain consistent with LipSync and Alternating. The State effect should remove listeners, clear the selected Mark Collection ID, and render nothing until the user selects another collection.

    Effect Editor combo boxes expose strings while the effect must persist IDs. The implementation needs stable label-to-ID mapping and tests for duplicate labels and missing selections.

    Coalescing must not merge across gaps, across different State items, or merely because two different State items happen to render the same color.

Milestone 2 cleans up project and data foundations. Update `src/Vixen.Modules/Effect/State/State.csproj` to reference `..\..\..\Vixen.Modules\Property\State\State.csproj` using a project reference with `PrivateAssets` or equivalent no-copy settings if existing module project references use them. Add an `InternalsVisibleTo` assembly attribute for `Vixen.Tests` if the test project cannot access internal helpers. Verify `Vixen.sln` contains the State effect project with `Debug|Any CPU` and `Deploy|Any CPU` mapped to `x64` according to the repository instructions, and correct any invalid mappings. Do not run `dotnet sln add` unless the project is missing, because the project already exists and running the command can create spurious solution folders.

Update `src/Vixen.Modules/Effect/State/StateData.cs`. Keep the class name `StateData`, but replace the current fields with the final persisted model:

    [DataMember] public Guid SelectedStateDefinitionId { get; set; }
    [DataMember] public Guid StatePropertyId { get; set; }
    [DataMember] public Guid StateOwnerElementId { get; set; }
    [DataMember] public StateRenderSource RenderSource { get; set; } = StateRenderSource.StateItem;
    [DataMember] public Guid SelectedStateItemId { get; set; } = Guid.Empty;
    [DataMember] public Guid MarkCollectionId { get; set; } = Guid.Empty;
    [DataMember] public PlaybackMode PlaybackMode { get; set; } = PlaybackMode.Default;

Create `src/Vixen.Modules/Effect/State/StateRenderSource.cs` with values `StateItem` and `MarkCollection`. Retire `TimingMode.cs` by deleting it or leaving it unused only if deletion causes serialization compatibility problems. Because VIX-3924 says no migration is required for development-only State effect data, prefer deleting `TimingMode.cs` and removing `AllowMarkGaps` entirely. Update `CreateInstanceForClone()` to copy all new fields. Add XML docs for any public enum or public property introduced or changed.

Milestone 3 implements discovery and editor option generation. Add internal helper files under `src/Vixen.Modules/Effect/State`:

`DiscoveredStateDefinition.cs` as an internal sealed record that contains owner node ID/name, State property ID, State definition ID, State definition name, the `StateDefinitionData` reference, and the computed display label.

`StateDefinitionDiscovery.cs` as an internal static helper with a method equivalent to:

    internal static IReadOnlyList<DiscoveredStateDefinition> Discover(IEnumerable<IElementNode> targetNodes)

This method must traverse each target node and every descendant in element-tree order. Include the target node itself. For each visited node, call `StateModule.GetStateModuleForElement(node)`. If a module exists, enumerate `StateDefinitions` in list order and capture each definition. Exclude State properties outside the targeted trees. Do not use global `VixenSystem.Nodes` for discovery except for resolving assignment IDs later.

Label generation must normally use the State definition name. If more than one discovered definition has the same name, append ` (owner node name)`. If duplicate labels remain, append a short stable ID suffix such as ` [01234567]`, using the first eight hex digits of the State definition ID with no braces. A missing selected definition should be exposed as a display value such as `<Missing State: 01234567>` so the combo box can show the invalid durable selection until the user corrects it. If no definitions exist and there is no missing selection, expose only `<No States Available>`.

Add `StateDefinitionNameConverter` inheriting `EffectListTypeConverterBase<State>` and returning `State.GetStateDefinitionOptions()`. Add `StateItemNameConverter` returning `State.GetStateItemOptions()`. These converters must return strings because the existing selection editor stores display values. Implement string properties on `State.cs` that map display labels to IDs:

`StateDefinition` displays the selected discovered label, the missing placeholder, or `<No States Available>`. Setting it to a valid label updates `SelectedStateDefinitionId`, `StatePropertyId`, `StateOwnerElementId`, refreshes State Item options, preserves a matching State Item name when possible, falls back to `<All>` when no matching item name exists, marks the effect dirty, and calls `OnPropertyChanged()` for dependent properties. Setting it to a placeholder must not change durable IDs.

`StateItem` displays `<All>`, a valid anchor item's current exact name, a missing placeholder such as `<Missing State Item: 01234567>`, or `<No State Items Available>`. Setting it to `<All>` stores `Guid.Empty`. Setting it to a valid item name stores the first item ID in that exact-name group. Setting it to placeholders must not change durable IDs.

Add `RenderSource` and `PlaybackMode` public properties with editor attributes. Use `SetBrowsable` and `TypeDescriptor.Refresh(this)` so `StateItem` is visible only for `RenderSource == StateRenderSource.StateItem`, and `MarkCollectionId` is visible only for `RenderSource == StateRenderSource.MarkCollection`. `PlaybackMode` remains visible for both. Property order must be `State` order 0, `RenderSource` order 1, `StateItem` or `MarkCollectionId` order 2, and `PlaybackMode` order 3.

When `TargetNodesChanged()` runs, rediscover definitions, retain a valid `SelectedStateDefinitionId`, auto-select the first discovered definition for a newly added effect with no selected ID, and leave missing selections intact when the selected ID no longer exists. Call `GetValidColors()` only as a general flag refresh; rendering must resolve discrete colors independently.

Milestone 4 implements State Item render planning. Add an internal rendering helper that converts selected data into interval work items before adding intents. A useful shape is:

    internal sealed record StateRenderInterval(StateItemData Item, TimeSpan Start, TimeSpan Duration);

For `RenderSource == StateItem`, resolve the selected State definition by ID. If missing, return no intervals. If `SelectedStateItemId == Guid.Empty`, treat it as `<All>`. In `PlaybackMode.Default`, return every State item in the definition for the full effect duration, in State item order. In `PlaybackMode.Iterate`, group State items by exact `Name` in first-row order, divide the full effect duration equally by unique name count, and return all rows in each group for that group's interval. If a single item anchor is selected, find the anchor by ID, take its current `Name`, and render all State items with that exact name for the full effect duration in both playback modes. If the anchor ID is missing, return no intervals.

Do not normalize or trim State item names during effect rendering. The State property setup is responsible for valid persisted names. Matching must be exact and case-sensitive.

Milestone 5 implements Mark Collection render planning. Keep the public `MarkCollectionId` property as a string for the editor, but continue persisting `_data.MarkCollectionId` as a GUID. The getter should display the selected collection name if present, or an empty string when no collection is selected. The setter should update only when the supplied display value maps to a real collection or is empty. In `MarkCollectionsRemoved`, follow LipSync and Alternating: if the removed collection is selected, remove listeners and set `MarkCollectionId = string.Empty`, which clears `_data.MarkCollectionId`, marks the effect dirty, and causes rendering to produce no Mark Collection-driven intents until the user selects another collection.

When `RenderSource` changes to `MarkCollection` and `_data.MarkCollectionId == Guid.Empty`, auto-select the first available collection with `CollectionType` equal to `State` only if such a type exists in the current enum. Because this milestone does not add `MarkCollectionType.State`, implement the lookup defensively by checking enum names or leave a clearly documented helper that returns null today. If there is no State collection, use the standard Mark Collection selector's normal behavior by selecting the first available collection only if existing Vixen selector behavior requires a non-empty selection. If no collections exist, keep the combo empty and render nothing.

For Mark Collection rendering, resolve the selected Mark Collection by `_data.MarkCollectionId`. If it is missing, render nothing. Get marks with `MarksInclusiveOfTime(StartTime, StartTime + TimeSpan)`. For each mark, clip to the effect boundaries:

    relativeStart = max(mark.StartTime - StartTime, TimeSpan.Zero)
    relativeEnd = min(mark.EndTime - StartTime, TimeSpan)
    duration = relativeEnd - relativeStart

Skip marks whose clipped duration is zero or negative. Parse `mark.Text` by splitting on commas. Trim leading/trailing whitespace for each segment. Preserve empty and unknown segments in Iterate mode for timing shares. In Default mode, build a distinct recognized name list in first segment order and return every State item with those names for the full clipped mark duration. In Iterate mode, divide the clipped mark duration by segment count, including unknown and empty segments. For each recognized segment, return every matching State item in State definition order for that segment's interval. Repeated names remain repeated in Iterate mode.

Mark gaps naturally render nothing because no intervals are generated. Overlapping marks should both generate intervals; do not suppress one mark because another overlaps it.

Milestone 6 converts intervals into intents, resolves leaf assignments, and coalesces adjacent intervals. For each interval, enumerate the interval's `StateItemData.ElementNodeIds` in stored order. Resolve each ID from the current effect target nodes and their descendants only. If the node is missing or outside the effect target scope, skip it. Expand the node to leaves with `GetLeafEnumerator()` at render time. This ensures group membership changes are reflected on the next render while preventing assignments outside the effect's target tree from rendering. For each leaf, resolve the color:

If `ColorModule.isElementNodeDiscreteColored(leaf)` is false, use the State item color. If the leaf is discrete, call `ColorModule.getValidColorsForElementNode(leaf, false)` and materialize the list. If the list is empty, skip the leaf. If the list contains the State item color, use that color. Otherwise use the first color in the list.

Create a static lighting or discrete lighting intent at 100 percent intensity with `CreateIntent(leaf, resolvedColor, 1.0, duration)` and add it to `_elementData` at the interval start. Because `BaseEffect.CreateIntent` already selects discrete intents when `HasDiscreteColors` and `IsElementDiscrete(leaf)` are true, set `HasDiscreteColors` accurately before rendering. If this proves unreliable for per-leaf discrete handling, call `CreateDiscreteIntent` directly for discrete leaves and `CreateIntent` for full-color leaves.

Implement coalescing before adding intents or while building a list of leaf render segments. Coalesce only when the same `StateItemData.Id` renders the same leaf element ID with the same resolved color and the next segment starts exactly at the previous segment's end. Do not coalesce if there is a gap, if State item IDs differ, or if leaf IDs differ. Do not coalesce merely because two different State items have the same color. Preserve ordering by adding final coalesced segments in the same order they would have been emitted without coalescing.

Each `_PreRender` must begin with `_elementData = new EffectIntents();`, then perform discovery and rendering into that new collection. This satisfies render-pass reset.

Milestone 7 adds the visual representation. In `State.cs`, override `ForceGenerateVisualRepresentation` to return true. Override `GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle)` following `src/Vixen.Modules/Effect/CustomValue/CustomValue.cs`: fill the rectangle with `Color.DarkGray`, choose an adjusted font with `Vixen.Common.Graphics.GetAdjustedFont`, and draw the static text `State` in white at a small left/top offset. Add any missing project reference needed for the graphics helper only if the State effect project does not already receive it transitively.

Milestone 8 adds tests. Create `src/Vixen.Tests/Effect/State` if it does not exist. Add a non-parallel xUnit collection if tests use global `VixenSystem.Nodes`, because the node manager is shared process state. Prefer focused internal helper tests that do not require WPF. Tests should follow Arrange, Act, Assert and use project conventions. If helper visibility requires it, add `InternalsVisibleTo` to the State effect project.

Add discovery and editor tests covering target tree traversal, exclusion of non-target trees, fixed creation order, duplicate label disambiguation, auto-select first definition, selected definition ID retention after rename, missing definition placeholder, no-State placeholder, target-node change invalidation, State Item option refresh, `<All>` first, duplicate State item names listed once, anchor rename by ID, and anchor removal placeholder.

Add State Item render tests covering `<All>` Default simultaneous rendering, `<All>` Iterate equal intervals by unique name, one selected name in both playback modes, duplicate names rendering every matching row, overlapping same leaf assignments preserving State item order, empty State definitions rendering nothing, deleted assignments skipped, assignment group leaf expansion at render time, and group membership changes reflected on a second render.

Add Mark Collection tests covering no collections, selected collection removal clearing, no automatic replacement selection, render-nothing behavior after selected collection removal, listener refresh, dirty invalidation on mark text/time changes, comma parsing, whitespace trimming, case sensitivity, unknown names, empty segments, Default distinct recognized names, Iterate repeated names, gaps, overlapping marks, and clipping to effect boundaries.

Add color and coalescing tests covering full-color leaf rendering configured color, discrete leaf supported configured color, discrete fallback to first supported color, discrete leaf with no supported colors skipped, render-pass reset, coalescing adjacent same item/leaf/color intervals, no coalescing across gaps, no coalescing across different State items with same color, and no coalescing across different leaves.

If constructing real element nodes in unit tests is difficult, create narrowly scoped internal helpers that accept target `IElementNode` collections or a scoped node lookup delegate for testing assignment expansion independently from the Vixen global node manager, then keep a smaller integration test proving the production resolver excludes assignments outside the effect target tree.

Milestone 9 validates and documents evidence. Run the commands in the `Concrete Steps` section. Record concise pass/fail evidence in `Artifacts and Notes`. If any command cannot run because of environment limitations, record the exact error and the reason in `Surprises & Discoveries`, then run the most focused command that can still prove behavior.

## Concrete Steps

From repository root `C:\Dev\Vixen`, first inspect current status so unrelated user changes are not overwritten:

    git status --short

If there are uncommitted user changes outside the State effect and docs plan files, leave them alone. If files needed by this plan already contain unexpected changes not made for this work, stop and ask the user how to proceed.

Update Jira VIX-3924 using the Milestone 1 paste text. If using the project Jira skill, read `.agents/skills/jira/SKILL.md` first and follow it. Record the Jira update outcome in `Progress` and `Artifacts and Notes`.

Implement Milestones 2 through 7. Keep changes localized to these expected files unless discoveries justify more:

    src/Vixen.Modules/Effect/State/State.csproj
    src/Vixen.Modules/Effect/State/State.cs
    src/Vixen.Modules/Effect/State/StateData.cs
    src/Vixen.Modules/Effect/State/PlaybackMode.cs
    src/Vixen.Modules/Effect/State/StateDescriptor.cs
    src/Vixen.Modules/Effect/State/StateRenderSource.cs
    src/Vixen.Modules/Effect/State/DiscoveredStateDefinition.cs
    src/Vixen.Modules/Effect/State/StateDefinitionDiscovery.cs
    src/Vixen.Modules/Effect/State/StateDefinitionNameConverter.cs
    src/Vixen.Modules/Effect/State/StateItemNameConverter.cs
    src/Vixen.Modules/Effect/State/StateRenderPlanner.cs
    src/Vixen.Modules/Effect/State/StateMarkParser.cs
    src/Vixen.Modules/Effect/State/StateIntentBuilder.cs
    Vixen.sln, only if solution mappings are wrong

Delete `src/Vixen.Modules/Effect/State/TimingMode.cs` only after replacing all references. Do not delete generated `obj` files unless the user explicitly asks for cleanup; they are build artifacts but removing them is not necessary for implementing the feature.

Add tests under:

    src/Vixen.Tests/Effect/State

Run focused tests after each rendering/editor milestone:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State"
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State"

Run final validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
    git diff --check

Expected successful test output should include `Passed!` or equivalent xUnit summary with zero failed tests. Expected successful MSBuild output should end with zero errors. `git diff --check` should produce no output and exit with code 0.

## Validation and Acceptance

Automated acceptance is met when these behaviors are covered by passing tests:

A State effect targeting a tree with one State property auto-selects the first discovered State definition, lists it in the `State` option list, and renders the selected definition's assigned leaf nodes.

Discovery includes State properties on target nodes and descendants only, keeps State definitions in creation order, works with multiple target nodes, and excludes State properties outside the target trees.

Duplicate State definition names are displayed with owner node names, and remaining duplicate labels include a short stable State definition ID suffix.

Renaming a selected State definition does not break the effect because the selected ID remains valid. Deleting the selected definition causes no render output and shows a missing placeholder until the user selects a valid State definition.

When no State definitions are available, the `State` combo options contain `<No States Available>` and rendering produces no intents.

The editor exposes properties in this order: `State`, `Render Source`, `State Item` or `Mark Collection`, and `Playback Mode`. `Playback Mode` remains visible for both render sources. Hidden render-source selections are retained.

Changing State definition refreshes State Item options, preserves a matching selected name when possible, otherwise selects `<All>`. `<All>` is first and unique State item names follow first-row order.

A renamed selected State item anchor remains selected by ID, displays the new name, and renders every item with that new exact name. A removed selected anchor is retained as missing and renders nothing until corrected or changed to `<All>`.

State Item Default and Iterate modes satisfy all VIX-3924 timing and duplicate-name requirements.

Mark Collection Default and Iterate modes satisfy all VIX-3924 parsing, clipping, gap, overlap, case-sensitive matching, repeated-name, unknown-name, and empty-segment requirements.

Rendering affects only State item assignments, expands assigned groups to current leaf nodes during each render pass, skips deleted assignments, preserves State item order for overlaps, uses discrete-color fallback per leaf, and adds no intents when nothing is active.

Each render pass starts with an empty intent collection, and contiguous-intent coalescing merges only exact same State item plus same leaf plus same color intervals that touch without a gap.

The generated visual representation is a dark gray bar with white `State` text.

Manual acceptance is met with this scenario:

1. Start Vixen from Debug output.
2. Open a test profile with a prop that has a State property containing at least one State definition and multiple State items.
3. Add the State effect to the prop's model group.
4. Confirm the Effect Editor shows the expected controls and defaults to the first State definition, `State Item`, and `Default` playback.
5. Select `<All>` and render/play the sequence. Confirm all assigned leaves for the selected State definition activate with configured or fallback colors for the full effect duration.
6. Change `Playback Mode` to `Iterate` and confirm unique State item names activate sequentially.
7. Switch to `Mark Collection`, choose a collection with marks such as `Open, Closed, Open`, and confirm Default renders recognized names together per mark while Iterate renders each segment share in order.
8. Rename the selected State definition in Display Setup, reopen the sequence, and confirm the effect remains selected and still renders.
9. Delete the selected State definition, reopen the sequence, and confirm the effect renders nothing and shows the missing selection until corrected.

## Idempotence and Recovery

The plan is safe to run incrementally. Discovery and rendering helpers are additive and tests can be run repeatedly. The data model replacement is safe for this development-only State effect because no migration is required by VIX-3924; do not add migration code unless new evidence shows released profiles already contain this partial effect.

If a test run fails, fix the smallest failing behavior and rerun the focused filter before running the full suite. If a build fails because `Vixen.sln` platform mappings are wrong for the State effect project, correct only the mappings for project `{011C7353-839B-4D5C-A673-482D47D5BE78}` and rerun the same build.

If Mark Collection listener behavior causes stale dirty-state updates, remove listeners from old selected collections before adding listeners to new selected collections. For removed selected collections, remove listeners and clear `_data.MarkCollectionId` through the public `MarkCollectionId` setter so the implementation stays aligned with LipSync and Alternating.

If effect editor placeholders can be selected accidentally, make setters ignore placeholder values and leave persisted IDs unchanged. This prevents a missing durable selection from being silently cleared.

If coalescing introduces ordering regressions, disable only the problematic coalescing path and keep tests for uncoalesced correctness passing, then reintroduce coalescing in a narrower helper. Correct rendering is more important than intent-count optimization.

## Artifacts and Notes

Key source facts gathered before implementation:

    src/Vixen.Modules/Effect/State/State.cs: `_PreRender` creates a new `EffectIntents` and calls an empty `RenderNodes()` placeholder.
    src/Vixen.Modules/Effect/State/StateData.cs: data now stores `SelectedStateDefinitionId`, `StatePropertyId`, `StateOwnerElementId`, `RenderSource`, `SelectedStateItemId`, `MarkCollectionId`, and `PlaybackMode`.
    src/Vixen.Modules/Effect/State/StateRenderSource.cs: new render-source enum replaces the removed `TimingMode` enum.
    src/Vixen.Modules/Property/State/StateData.cs: State property data stores stable `Id` and `StateDefinitions`.
    src/Vixen.Modules/Property/State/StateDefinitionData.cs: each definition stores stable `Id`, `Name`, `Description`, and ordered `Items`.
    src/Vixen.Modules/Property/State/StateItemData.cs: each item stores stable `Id`, `Name`, `Color`, and `ElementNodeIds`.
    src/Vixen.Modules/Property/State/StateModule.cs: `GetStateModuleForElement` returns the attached State property for an element node.
    src/Vixen.Modules/Effect/Effect/BaseEffect.cs: `CreateIntentsForElement` skips unsupported discrete colors, so State needs explicit fallback logic.
    src/Vixen.Modules/Effect/CustomValue/CustomValue.cs: `GenerateVisualRepresentation` shows the dark-gray/white-text drawing pattern requested by VIX-3924.

Record Jira update evidence and validation transcripts here as implementation proceeds. Keep transcripts concise, for example:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State"
    Passed!  - Failed: 0, Passed: <N>, Skipped: 0, Total: <N>

Milestone 2 validation:

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Build succeeded.
    0 Warning(s)
    0 Error(s)

Milestone 3 validation:

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Build succeeded.
    0 Warning(s)
    0 Error(s)

    git diff --check
    Exited successfully. Git printed the expected local checkout warning that `src/Vixen.Modules/Effect/State/State.cs` will be normalized from LF to CRLF the next time Git touches it; no whitespace errors were reported.

Milestone 4 validation:

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Build succeeded.
    0 Warning(s)
    0 Error(s)

    git diff --check
    Exited successfully. Git printed the expected local checkout warning that `src/Vixen.Modules/Effect/State/State.cs` will be normalized from LF to CRLF the next time Git touches it; no whitespace errors were reported.

State Item editor refresh validation:

    dotnet build src\Vixen.Modules\Editor\EffectEditor\EffectEditor.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Build succeeded.
    13 existing warnings.
    0 Error(s)

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Build succeeded.
    0 Warning(s)
    0 Error(s)

Milestone 1 Jira update:

    VIX-3924 was updated with the plan summary, design, acceptance criteria, risks, and testing notes from Milestone 1.

## Interfaces and Dependencies

The State effect depends on the State property project:

    src/Vixen.Modules/Property/State/State.csproj

Use these existing property types directly:

    VixenModules.Property.State.StateModule
    VixenModules.Property.State.StateData
    VixenModules.Property.State.StateDefinitionData
    VixenModules.Property.State.StateItemData

Use these existing effect/editor/rendering types:

    VixenModules.Effect.Effect.BaseEffect
    VixenModules.Effect.Effect.EffectTypeModuleData
    VixenModules.Effect.Effect.EffectListTypeConverterBase<T>
    Vixen.Sys.EffectIntents
    Vixen.Sys.IElementNode
    Vixen.Sys.IElementNode.GetLeafEnumerator()
    Vixen.Intent.IntentBuilder through BaseEffect intent helpers
    Vixen.Marks.IMarkCollection
    Vixen.Marks.IMark
    Vixen.TypeConverters.IMarkCollectionNameConverter

At completion, `src/Vixen.Modules/Effect/State/StateData.cs` must expose and clone these persisted properties:

    Guid SelectedStateDefinitionId
    Guid StatePropertyId
    Guid StateOwnerElementId
    StateRenderSource RenderSource
    Guid SelectedStateItemId
    Guid MarkCollectionId
    PlaybackMode PlaybackMode

At completion, `src/Vixen.Modules/Effect/State/State.cs` must expose these editor-facing properties with XML docs if public:

    string StateDefinition { get; set; }
    StateRenderSource RenderSource { get; set; }
    string StateItem { get; set; }
    string MarkCollectionId { get; set; }
    PlaybackMode PlaybackMode { get; set; }

The helper types should remain internal unless the Effect Editor requires public visibility. If any helper becomes public, add XML docs immediately per `.agents/skills/csharp-docs/SKILL.md`.

## Revision Notes

- 2026-06-08 / Codex: Initial ExecPlan created from VIX-3924 requirements, State property final requirements, the partial State effect implementation, and comparable Vixen effect patterns. The plan resolves naming and identity decisions up front because the partial effect uses pre-requirement terminology and lacks durable State definition selection.
- 2026-06-08 / Codex: Updated Mark Collection removal behavior to clear the selected collection ID, matching LipSync and Alternating, because the project direction is to keep Mark Collection removal behavior universal across effects.
- 2026-06-08 / Codex: Completed Milestone 2 and updated the current-state context, artifacts, and validation evidence so the plan reflects the repository after the data/project foundation changes.
- 2026-06-08 / Codex: Marked Milestone 1 complete after confirmation that Jira issue VIX-3924 was updated with the planning details.
- 2026-06-08 / Codex: Completed Milestone 3 by adding State definition discovery, dynamic Effect Editor option converters, missing/no-option placeholders, selected State item preservation, and render-source-specific property visibility.
- 2026-06-08 / Codex: Completed Milestone 4 by adding State Item render intervals and intent emission with target-scoped assignment lookup, render-time leaf expansion, discrete-color fallback, and render-pass reset.
- 2026-06-08 / Codex: Corrected Effect Editor refresh handling so `TypeDescriptor.Refresh(effect)` also invalidates property standard values, allowing State Item options to refresh when State Definition changes.
- 2026-06-08 / Codex: Changed State Definition selection fallback so State Item returns to `<All>` when the new definition does not contain a matching previous item name.

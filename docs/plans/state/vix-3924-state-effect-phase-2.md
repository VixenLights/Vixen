# Implement VIX-3924 State Effect Phase 2 Custom Render Source

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Any contributor implementing this plan must keep this document self-contained, update it at every stopping point, and record new decisions or discoveries as they occur.

## Purpose / Big Picture

After this work, a sequence author can use the State effect without creating a Mark Collection just to render a hand-picked subset of State items. The user selects a State definition, sets `Render Source` to `Custom`, adds custom State rows, and optionally overrides each row's color. In `Default` playback mode, the custom rows act like a curated `<All>` set: all rows render together for the full effect duration. In `Iterate` playback mode, the row list becomes a timed sequence, can include repeated State items, and can include `<None>` rows that intentionally render blank time.

The behavior is visible in the standard Effect Editor. Add a State effect to a prop with a State property, choose a State definition, choose `Custom`, add custom rows, and play the sequence. The effect should light only the assigned nodes from the selected State items, use each row's color override, use a full-color or discrete-color picker based on the selected row's assigned elements, and show a `Custom` hint in the timeline visual.

## Progress

- [x] (2026-06-29 00:00 -05:00) Read `.agents/PLANS.md`, `docs/state/vix-3924-state-effect-phase-2.md`, and the project C# skills called out by the specification.
- [x] (2026-06-29 00:00 -05:00) Reviewed current State effect source locations under `src/Vixen.Modules/Effect/State` and existing expandable collection examples in Wave and Liquid.
- [x] (2026-06-29 00:00 -05:00) Created this ExecPlan from the final phase 2 specification.
- [x] (2026-06-29 08:20 -05:00) Completed Milestone 1. Read VIX-3924 and added the phase 2 Custom render source summary, requirements, high-level design, acceptance criteria, testing, and risks as Jira comment `40085`.
- [ ] Implement persisted custom row data and collection runtime models.
- [ ] Add `Custom` render source editor behavior, custom row option generation, and mode-change normalization.
- [ ] Add custom render planning and rendering with row color overrides.
- [ ] Make row color editing honor the selected State item's assigned element color capabilities.
- [ ] Update visual representation to show a Custom hint.
- [ ] Add focused tests and run validation commands.
- [ ] Complete manual validation and update this plan's evidence and retrospective.

## Surprises & Discoveries

- Observation: No implementation discoveries have been made yet for phase 2.
  Evidence: This plan was created from the final specification and initial source orientation only.

- Observation: The Atlassian MCP became available after Rider was restarted.
  Evidence: Tool discovery exposed the `mcp__atlassian` namespace. `getAccessibleAtlassianResources` returned the `vixenlights` Jira resource with `read:jira-work` and `write:jira-work` scopes, and `addCommentToJiraIssue` created comment `40085` on VIX-3924.

## Decision Log

- Decision: Implement `Custom` as a third `StateRenderSource` value rather than as a variation of `State Item` or `Mark Collection`.
  Rationale: The user-visible workflow is distinct: it stores an effect-owned row collection with per-row color overrides and does not depend on Mark Collection text.
  Date/Author: 2026-06-29 / Codex

- Decision: Preserve the current `Playback Mode` when switching `Render Source` to `Custom`.
  Rationale: The user explicitly decided Custom should not force the effect to Iterate, and existing render source switching preserves hidden state.
  Date/Author: 2026-06-29 / Codex

- Decision: Treat Default custom playback as a curated `<All>` set.
  Rationale: In Default mode, all custom rows are enabled simultaneously just like `<All>` in the existing State Item render source, except the custom list defines the enabled set.
  Date/Author: 2026-06-29 / Codex

- Decision: Allow each State item only once in Default mode, but allow repeated State items in Iterate mode.
  Rationale: Duplicate rows do not make sense when every row is active simultaneously, but they are meaningful in Iterate mode for repeated timing and color overrides.
  Date/Author: 2026-06-29 / Codex

- Decision: Normalize the custom row collection when changing from Iterate to Default.
  Rationale: Default mode must not contain `<None>` or repeated State items. Keeping the first row for each State item preserves the earliest user-defined color and ordering while removing invalid rows.
  Date/Author: 2026-06-29 / Codex

- Decision: Clear the custom row collection when the top-level State definition changes.
  Rationale: Existing row IDs belong to the prior State definition and should not silently point at unrelated items.
  Date/Author: 2026-06-29 / Codex

- Decision: Use simple `System.Drawing.Color` row colors, not gradients.
  Rationale: State items currently support simple colors. Gradients are a possible future enhancement and are outside this phase.
  Date/Author: 2026-06-29 / Codex

- Decision: Make the row color picker use the selected State item's assigned element set as its color context.
  Rationale: A user editing a row should see the color choices that the row's actual assigned elements support, not the color capabilities of unrelated effect targets or State items.
  Date/Author: 2026-06-29 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. VIX-3924 now has a Jira comment documenting the phase 2 Custom render source enhancement discovered during validation and user testing. Code implementation has not started yet. At each milestone, add a short entry here that states what now works, what was validated, and what remains.

## Context and Orientation

Vixen is a Windows .NET WPF application for sequencing light shows. Effects are modules under `src/Vixen.Modules/Effect`. The State effect already exists in `src/Vixen.Modules/Effect/State` and depends on the State property models under `src/Vixen.Modules/Property/State`.

The State property is attached to element nodes in Display Setup. A State property contains State definitions. A State definition is a named group of State items. A State item has a stable `Guid` ID, a name, a simple color, and assigned element node IDs. The State effect renders those assigned element nodes by looking up the selected State definition and selected State item behavior.

The current State effect already supports two render sources:

- `State Item`, where the user chooses `<All>` or one State item name from the selected State definition.
- `Mark Collection`, where mark text drives State item names over time.

The current State effect files are:

- `src/Vixen.Modules/Effect/State/State.cs`, the effect module, editor-facing properties, rendering entry point, State definition discovery usage, Mark Collection handling, visual representation, and browsability logic.
- `src/Vixen.Modules/Effect/State/StateData.cs`, persisted effect data.
- `src/Vixen.Modules/Effect/State/StateRenderSource.cs`, the enum that currently contains `StateItem` and `MarkCollection`.
- `src/Vixen.Modules/Effect/State/StateRenderPlanner.cs`, interval planning for State Item and Mark Collection render sources.
- `src/Vixen.Modules/Effect/State/StateRenderInterval.cs`, an internal render interval record currently carrying a `StateItemData`, start time, and duration.
- `src/Vixen.Modules/Effect/State/StateRenderSegment.cs` and `StateRenderSegmentCoalescer.cs`, render segment creation and adjacent interval coalescing.
- `src/Vixen.Modules/Effect/State/StateDefinitionDiscovery.cs`, target-tree State definition discovery and display-label helpers.
- `src/Vixen.Modules/Effect/State/StateDefinitionNameConverter.cs` and `StateItemNameConverter.cs`, Effect Editor combo-box option providers.
- `src/Vixen.Modules/Effect/State/PlaybackMode.cs`, the `Default` and `Iterate` playback mode enum.

The State property model files used by the effect include:

- `src/Vixen.Modules/Property/State/StateDefinitionData.cs`, which stores `Id`, `Name`, `Description`, and ordered `Items`.
- `src/Vixen.Modules/Property/State/StateItemData.cs`, which stores each State item row's stable `Id`, `Name`, `Color`, and assigned `ElementNodeIds`.
- `src/Vixen.Modules/Property/State/StateData.cs`, which stores State definitions on an element property.

The new `Custom` render source must use the existing expandable object collection pattern used by other effects. Relevant examples are:

- `src/Vixen.Modules/Effect/Wave/Wave/WaveFormCollection.cs`, a collection class inheriting from `MarkCollectionExpandoObjectCollection<IWaveform, Waveform>`.
- `src/Vixen.Modules/Effect/Wave/Wave/Waveform.cs`, an expandable runtime row object with property-grid attributes.
- `src/Vixen.Modules/Effect/Wave/Wave/WaveData.cs` and `WaveformData.cs`, serialized row list and row data.
- `src/Vixen.Modules/Effect/Liquid/Liquid/EmitterCollection.cs`, a collection class that pushes parent and shared context to row objects when the collection changes.
- `src/Vixen.Modules/Effect/Liquid/Liquid/Emitter.cs` and `EmitterData.cs`, runtime and serialized row examples.
- `src/Vixen.Modules/Effect/Effect/ExpandoObjectObservableCollection.cs`, `IExpandoObjectCollection.cs`, and `MarkCollectionExpandoObjectCollection.cs`, the shared expandable collection infrastructure.

A custom State row means one effect-owned collection entry. It stores a selected State item ID and a row color override. In Iterate mode, a row may also store `Guid.Empty` to represent `<None>`, an intentional blank timing slot. In Default mode, `<None>` is invalid and the same State item can appear only once.

## Plan of Work

Milestone 1 updates Jira before coding. Read the current VIX-3924 Jira issue, then add the phase 2 Custom render source summary as an additional enhancement discovered during validation and user testing. The Jira paste text should include the requirements, high-level design, acceptance criteria, test plan, risks, and manual validation scenario from this plan. Record the Jira update result in `Progress` and `Artifacts and Notes`.

Milestone 2 adds the persisted and runtime custom row model. Create `src/Vixen.Modules/Effect/State/CustomStateItemData.cs` as a `[DataContract]` class with `[DataMember] Guid StateItemId` and `[DataMember] Color Color`. Give it a clone helper. Add `[KnownType(typeof(CustomStateItemData))]` to `StateData` if the serializer pattern requires it, and add `List<CustomStateItemData> CustomStateItems` initialized to an empty list. Update `StateData.CreateInstanceForClone()` to deep-clone the rows and include the existing `ShowEffectVisual` value while preserving all existing fields. Add a defensive normalizer, for example `EnsureCustomStateItems()`, so old effect data with a missing list does not throw.

Milestone 3 creates the editor-facing custom row collection. Add `CustomStateItem.cs`, `CustomStateItemCollection.cs`, and a row State item option converter such as `CustomStateItemNameConverter.cs`. The runtime row object should be an expandable object with `State Item` and `Color` properties. It should keep a reference to a small provider owned by `State`, or directly to the parent `State` effect if that matches local patterns, so it can ask for available State item options and resolve selected labels to State item IDs. The collection should notify the parent effect when rows are added, removed, or edited so `IsDirty` becomes true and the effect rerenders. Do not introduce WPF-specific controls into the effect or row model.

Milestone 4 adds `Custom` editor behavior to `State.cs`. Add `Custom` to `StateRenderSource` with XML documentation and a `[Description("Custom")]` attribute. Add a `CustomStateItems` editor property to `State` with display name `Custom State Items`, the correct category and property order, and browsability only when `RenderSource == StateRenderSource.Custom`. Update `SetRenderSourceBrowsables()` so `StateItem`, `MarkCollectionId`, and `CustomStateItems` are mutually exclusive based on render source. Switching to Custom must preserve `PlaybackMode`.

Milestone 5 implements row option rules and mode-change cleanup. In Default playback mode, row State item options list only State item rows not already used by another custom row. `<None>` is not offered. In Iterate playback mode, row options list `<None>` first and then all State item rows, including already-selected rows. Duplicate State item names must be disambiguated by assignment summary first, ordinal second, and short ID suffix only if needed. When `PlaybackMode` changes from Iterate to Default, remove `<None>` rows and duplicate State item rows, keeping the first row found for each `StateItemId`. Changing the top-level `StateDefinition` clears the custom row collection. Changing a row's State item resets the row color to that State item's current default color.

Milestone 6 makes the row color editor use the selected State item's assigned element color context. Inspect the existing color editor and color-property patterns before coding. The row `Color` property should expose enough color information for the property grid to choose full-color versus discrete-color behavior based on the selected State item's assigned element nodes. If the existing editor infrastructure cannot express a mixed assignment set perfectly, record the discovery in `Surprises & Discoveries`, choose the closest established behavior, and update this plan before implementation. Rendering already performs discrete-color fallback; the editor should avoid presenting misleading choices where possible.

Milestone 7 extends rendering for Custom intervals. Add custom interval planning to `StateRenderPlanner`, either by adding `CreateCustomIntervals` or by introducing a small interval input record that carries `StateItemData`, row color, start time, and duration. If `StateRenderInterval` currently assumes State item default color, extend it or create a new record so custom rows can override color without mutating `StateItemData`. In Default mode, return one full-duration interval per valid custom row after Default cleanup. In Iterate mode, split the effect duration by row count times `Iterations`; `<None>` and missing rows consume time but produce no intervals. Render valid rows using the row color and the selected State item's assignments.

Milestone 8 updates visual representation. In `State.GenerateVisualRepresentation`, include a Custom hint when `RenderSource == StateRenderSource.Custom`, for example `State - Custom - <StateDefinition>` or another compact form that fits the existing visual style. Keep the current dark gray background and white text pattern.

Milestone 9 adds tests. Add focused tests under `src/Vixen.Tests/Effect/State` for `CustomStateItemData` cloning, `StateData` default/null-list behavior, row option generation, duplicate label disambiguation, Default uniqueness, Iterate duplicates, Iterate `<None>`, Iterate-to-Default cleanup, clearing rows on State definition change, custom interval planning, row color override rendering, color-capability editor behavior where practical, and regression coverage for existing State Item and Mark Collection tests. Tests should follow the existing xUnit style in `src/Vixen.Tests/Effect/State/StateRenderPlannerTests.cs` and related State tests.

Milestone 10 validates the feature. Run the focused State tests, build the State effect project, run `git diff --check`, and perform the manual acceptance scenario. If time permits, run the full `Vixen.Tests` project and record any unrelated failures separately.

## Concrete Steps

Start from the repository root:

    cd C:\Dev\Vixen

Before editing implementation files, inspect the current code:

    Get-Content -Raw -LiteralPath docs\state\vix-3924-state-effect-phase-2.md
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\State\State.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\State\StateData.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\State\StateRenderPlanner.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\Wave\Wave\WaveFormCollection.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\Wave\Wave\Waveform.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\Liquid\Liquid\EmitterCollection.cs
    Get-Content -Raw -LiteralPath src\Vixen.Modules\Effect\Liquid\Liquid\Emitter.cs

Read Jira VIX-3924, then update it with this markdown text adapted to the current Jira issue state:

    ## Additional enhancement discovered during State effect validation: Custom render source

    Phase 2 adds a `Custom` render source to the State effect. This lets a sequence author curate an effect-owned list of State items and override each row's color without creating a Mark Collection solely for timing control.

    ### Requirements

    - Add `Custom` to the State effect `Render Source` options.
    - Show a `Custom State Items` collection only when `Render Source = Custom`.
    - Each custom row contains `State Item` and `Color`.
    - Row colors are simple colors matching current State item support; gradients remain future scope.
    - The row color picker uses the selected State item's assigned element set to choose full-color versus discrete-color behavior.
    - `Custom` preserves the current `Playback Mode`.
    - In `Default`, custom rows behave like a curated `<All>` set: all valid rows render for the full effect duration, `<None>` is not offered, and each State item can appear once.
    - In `Iterate`, custom rows play sequentially, `<None>` is available as a blank timing slot, and State items may be repeated.
    - Switching from `Iterate` to `Default` removes `<None>` rows and duplicate State item rows, keeping the first row for each State item.
    - Changing the selected top-level State definition clears the custom row collection.
    - Duplicate State item names are selectable individually and disambiguated using assignment context, row ordinal, or short ID suffix.
    - The timeline visual includes a Custom hint when Custom is selected.

    ### High-level design

    Persist `CustomStateItemData` rows in `StateData`. Expose an expandable object collection in `State.cs` using the existing Wave/Liquid collection pattern. Extend State render planning with Custom intervals that carry row color overrides. Keep existing State Item and Mark Collection behavior unchanged.

    ### Acceptance criteria

    - Users can add, remove, and edit custom rows.
    - Default mode hides `<None>`, prevents duplicate State item additions, and renders all valid rows together.
    - Iterate mode shows `<None>`, allows duplicate State item rows, and repeats the row sequence according to `Iterations`.
    - Row color overrides render instead of State item default colors.
    - Discrete-color State item assignments show constrained color choices.
    - Existing State Item and Mark Collection render sources continue to pass their tests.

    ### Testing

    Run focused Effect.State and Property.State tests, build the State effect project, run `git diff --check`, and manually validate the Effect Editor workflow with Default and Iterate Custom rows.

    ### Risks

    Duplicate State item label disambiguation and row color editor context may require careful integration with existing property-grid infrastructure.

Add or update implementation files in this order:

    src\Vixen.Modules\Effect\State\CustomStateItemData.cs
    src\Vixen.Modules\Effect\State\CustomStateItem.cs
    src\Vixen.Modules\Effect\State\CustomStateItemCollection.cs
    src\Vixen.Modules\Effect\State\CustomStateItemNameConverter.cs
    src\Vixen.Modules\Effect\State\StateRenderSource.cs
    src\Vixen.Modules\Effect\State\StateData.cs
    src\Vixen.Modules\Effect\State\State.cs
    src\Vixen.Modules\Effect\State\StateRenderInterval.cs or a new custom interval record
    src\Vixen.Modules\Effect\State\StateRenderPlanner.cs
    src\Vixen.Modules\Effect\State\StateRenderSegment.cs if row identity or row color needs to flow through coalescing
    src\Vixen.Tests\Effect\State\*.cs

After each milestone that compiles, run focused validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore

Before finalizing, run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State" --no-restore
    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    git diff --check

Expected successful test output should include `Passed!` with zero failed tests. Expected build output should end with `Build succeeded.` and zero errors. `git diff --check` should produce no whitespace errors.

## Validation and Acceptance

Automated acceptance is met when tests prove these behaviors:

`StateRenderSource` includes `Custom` and existing `StateItem` and `MarkCollection` values retain their behavior.

`StateData` initializes and clones `CustomStateItems` safely. Existing data with no custom row list does not throw when loaded or cloned.

The Effect Editor browsability switches correctly: `State Item` is visible only for State Item render source, `MarkCollectionId` is visible only for Mark Collection render source, and `Custom State Items` is visible only for Custom render source.

Switching to Custom preserves the current `PlaybackMode`.

In Default playback mode, `<None>` is not offered, each State item can be selected once, already-selected State items are not offered for additional rows, and all valid rows render for the full effect duration with their row colors.

In Iterate playback mode, `<None>` is offered first, State items may be selected multiple times, the row sequence repeats according to `Iterations`, and `<None>` consumes time while rendering nothing.

Switching from Iterate to Default removes `<None>` rows and duplicate State item rows, keeping the first row for each State item ID.

Changing a custom row's State item stores that State item's ID and resets the row color to the State item's configured color.

Changing the selected top-level State definition clears the custom row collection.

Duplicate State item names are displayed with enough context to select one specific State item ID.

The row color picker uses the selected State item's assigned elements to choose full-color versus discrete-color editor behavior.

Custom rendering uses the row color override, not the State item default color, while using the selected State item's assignments for element lookup.

Discrete-color fallback during rendering remains correct: unsupported row colors fall back to the first supported leaf color, and leaves with no supported colors are skipped.

The timeline visual includes a Custom hint when `RenderSource == StateRenderSource.Custom`.

Existing State Item and Mark Collection tests still pass.

Manual acceptance is met with this scenario:

1. Start Vixen from Debug output.
2. Open or create a profile with a prop that has a State property containing at least one State definition with three State item rows, assigned elements, and distinct colors.
3. Add the State effect to the prop.
4. Select the State definition.
5. Set `Render Source` to `Custom`.
6. Confirm `Custom State Items` is visible and the top-level `State Item` and `Mark Collection` selectors are hidden.
7. In `Default` playback mode, add the first State item and second State item. Confirm `<None>` is not listed and the already-added State items are no longer offered for new rows.
8. Change the first row's color and confirm the picker matches that State item's assigned elements.
9. Render or play the effect and confirm the first and second State item assignments render together for the full effect duration using row colors.
10. Change `Playback Mode` to `Iterate` and set `Iterations` to `2`.
11. Confirm `<None>` appears first in row options and already-selected State items can be selected again.
12. Add `<None>` between the first and second State rows and add the first State item again.
13. Render or play the effect and confirm the sequence is first item, blank, second item, first item, then repeats once more.
14. Change `Playback Mode` back to `Default` and confirm `<None>` and the duplicate first item row are removed, keeping the first first-item row and its color.
15. Switch `Render Source` away from Custom and back to Custom and confirm remaining rows are retained.
16. Change the selected State definition and confirm the custom row collection is cleared.
17. Confirm the timeline visual includes a Custom hint while Custom is selected.

## Idempotence and Recovery

The implementation is additive and safe to run incrementally. Adding data members to `StateData` must be backward-compatible by initializing missing lists. New helper files can be added and tests can be run repeatedly.

If the expandable collection editor cannot disable adding rows in Default mode when all State items are already selected, keep the collection stable and make the new row become a missing/no-item placeholder only after updating this plan with the discovery and rationale. Prefer preventing invalid rows through options and mode cleanup whenever the infrastructure allows it.

If duplicate labels cannot be supported by the existing selection editor, do not collapse duplicate State items silently. Update this plan with the observed limitation and implement disambiguated labels that stay unique.

If the row color picker cannot perfectly model mixed full-color and discrete-color assigned element sets, document the existing editor limitation in `Surprises & Discoveries` and choose the closest established behavior that avoids presenting colors that obviously cannot render.

If custom interval changes break existing State Item or Mark Collection tests, restore the existing path first and move Custom planning into a separate helper. Existing render sources are regression-sensitive and must remain unchanged.

If coalescing merges across a `<None>` gap or across different row colors, narrow or bypass coalescing for Custom intervals until correctness is restored. Correct output is more important than reducing intent count.

Do not delete or rewrite unrelated build artifacts. Do not reformat unrelated files. Preserve CRLF line endings.

## Artifacts and Notes

Source facts used to create this plan:

    docs/state/vix-3924-state-effect-phase-2.md defines the final product requirements and resolved design decisions for Custom render source.
    src/Vixen.Modules/Effect/State/State.cs currently exposes StateDefinition, StateItem, MarkCollectionId, RenderSource, PlaybackMode, Iterations, and ShowEffectVisual editor properties.
    src/Vixen.Modules/Effect/State/StateRenderSource.cs currently has StateItem and MarkCollection.
    src/Vixen.Modules/Effect/State/StateRenderPlanner.cs currently plans State Item and Mark Collection intervals.
    src/Vixen.Modules/Effect/Wave/Wave and src/Vixen.Modules/Effect/Liquid/Liquid contain expandable object collection examples.

Record validation transcripts here as implementation proceeds, for example:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore
    Passed!  - Failed: 0, Passed: <N>, Skipped: 0, Total: <N>

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    Build succeeded.
    0 Error(s)

Milestone 1 Jira update:

    2026-06-29 08:20 -05:00 / Codex: Read VIX-3924 using cloud ID cc8261dc-f522-4dfa-96f0-3effdc1f0a1f. The issue summary is "Create the State Effect", status is In Progress, issue type is New Feature, component is Effects, and the existing description contains the phase 1 State effect requirements. Added the phase 2 Custom render source update as Jira comment 40085.

Record additional Jira update evidence here if the issue description is later edited or linked to implementation work.

## Interfaces and Dependencies

Use the existing State property types:

    VixenModules.Property.State.StateDefinitionData
    VixenModules.Property.State.StateItemData

Use the existing State effect types:

    VixenModules.Effect.State.State
    VixenModules.Effect.State.StateData
    VixenModules.Effect.State.StateRenderSource
    VixenModules.Effect.State.PlaybackMode
    VixenModules.Effect.State.StateRenderPlanner

Use the existing effect/editor infrastructure:

    VixenModules.Effect.Effect.BaseEffect
    VixenModules.Effect.Effect.EffectTypeModuleData
    VixenModules.Effect.Effect.EffectListTypeConverterBase<T>
    VixenModules.Effect.Effect.ExpandoObjectBase
    VixenModules.Effect.Effect.ExpandoObjectObservableCollection<TInterface, TImpl>
    Vixen.Attributes.ValueAttribute
    Vixen.Sys.Attribute.ProviderCategoryAttribute
    Vixen.Sys.Attribute.ProviderDisplayNameAttribute
    Vixen.Sys.Attribute.ProviderDescriptionAttribute
    VixenModules.EffectEditor.EffectDescriptorAttributes.PropertyEditorAttribute
    VixenModules.EffectEditor.EffectDescriptorAttributes.PropertyOrderAttribute
    VixenModules.Property.Color.ColorModule

At completion, `src/Vixen.Modules/Effect/State/StateRenderSource.cs` must contain:

    StateItem
    MarkCollection
    Custom

At completion, `src/Vixen.Modules/Effect/State/StateData.cs` must persist and clone:

    Guid SelectedStateDefinitionId
    Guid StatePropertyId
    Guid StateOwnerElementId
    StateRenderSource RenderSource
    Guid SelectedStateItemId
    Guid MarkCollectionId
    PlaybackMode PlaybackMode
    int Iterations
    bool ShowEffectVisual
    List<CustomStateItemData> CustomStateItems

At completion, add `src/Vixen.Modules/Effect/State/CustomStateItemData.cs` with:

    [DataContract]
    public sealed class CustomStateItemData
    {
        [DataMember]
        public Guid StateItemId { get; set; }

        [DataMember]
        public Color Color { get; set; }

        public CustomStateItemData CreateInstanceForClone();
    }

If this class can be internal without breaking serialization, prefer internal. If it must be public for serializer or editor access, include XML documentation on the type, properties, and clone method.

At completion, add an editor-facing row object under `src/Vixen.Modules/Effect/State`, tentatively:

    [ExpandableObject]
    public sealed class CustomStateItem : ExpandoObjectBase
    {
        public string StateItem { get; set; }
        public Color Color { get; set; }
        public CustomStateItemData CreateData();
    }

The final implementation may adjust names to match local conventions, but it must keep one class per file and update XML docs for all public/protected APIs.

At completion, `State.cs` must expose:

    public CustomStateItemCollection CustomStateItems { get; }

or an equivalent settable property if the existing expandable collection editor requires a setter.

The implementation should avoid new async code. If any asynchronous work is introduced unexpectedly, follow `.agents/skills/csharp-async/SKILL.md`: use `Task` or `Task<T>`, add `Async` suffixes, avoid `.Wait()` and `.Result`, and propagate exceptions.

## Revision Notes

- 2026-06-29 / Codex: Initial ExecPlan created from `docs/state/vix-3924-state-effect-phase-2.md`, `.agents/PLANS.md`, current State effect source orientation, and the project's C# skill guidance. The plan captures resolved product decisions so implementation can proceed without reopening the requirements discussion.
- 2026-06-29 / Codex: Recorded the Milestone 1 Jira update attempt as blocked because Atlassian/Jira MCP tools were not available in the initial session. The plan still contains paste-ready Jira text for VIX-3924.
- 2026-06-29 / Codex: Completed Milestone 1 after Rider restart exposed the Atlassian MCP. Added Jira comment 40085 to VIX-3924 and updated Progress, Surprises & Discoveries, Outcomes & Retrospective, and Artifacts and Notes.

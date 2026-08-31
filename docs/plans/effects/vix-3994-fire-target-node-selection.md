# Add each element/group behavior to the Fire effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This repository contains `.agents/PLANS.md`; maintain this document according to that file. VIX-3994 is the tracker issue for this work. The completed Wipe precedent is `docs/plans/effects/vix-3938-wipe-target-node-selection.md`; this plan incorporates the relevant behavior rather than requiring the implementer to read that plan.

## Purpose / Big Picture

After this change, a user can choose whether Fire runs as one continuous fire field across all selected elements/groups or starts an independent fire field for each selected element/group. This is useful for strings and preview-location targets: a user can make one flame pattern span an entire display group, or make each prop/subgroup have its own locally sized fire pattern.

Existing Fire effects must remain visually and serially compatible. They will load with `Across Elements/Groups` selected and use the current combined rendering behavior. The new controls are visible only when a target hierarchy or multiple selected targets makes the choice meaningful. In individual mode, a single deep target may select an intermediate child depth, matching Wipe's useful-depth rules.

## Progress

- [x] (2026-08-31 16:00Z) Read VIX-3994, its clarification comment, `.agents/PLANS.md`, the Wipe ExecPlan/implementation, Spin and Chase precedents, Fire, `PixelEffectBase`, and Fire's existing test coverage.
- [x] (2026-08-31 21:42Z) Updated VIX-3994 with the final user-facing summary, scope, acceptance criteria, and validation plan. No repository code or tests changed.
- [x] (2026-08-31 22:01Z) Added `FireTargetNodeSelectionTests` for default data/effect settings, serialized fields, legacy-data compatibility, editor visibility, and default group-mode location rendering. Full `Vixen_Tests` build passed; the focused filter passed 1 group-render test and failed 6 tests only for the deliberately absent Fire target-selection contract.
- [x] (2026-08-31 22:10Z) Added persisted Fire target settings, property-grid visibility/normalization, `FireTargetElementDepthConverter`, and Fire localization resources. The focused suite now passes all 10 tests.
- [x] (2026-08-31 22:35Z) Added the documented `PixelEffectBase` render-group seam and Fire's group resolver. Each group now receives its own target-scoped buffer configuration, setup/render/cleanup lifecycle, and merged intents. Focused location lifecycle tests verify two intermediate-depth groups and two separately selected targets each use independent local `3 x 1` buffers. The full `Vixen_Tests` build and focused Fire filter passed (41 tests).
- [x] (2026-08-31 22:50Z) Removed the re-entrant descriptor refresh from Fire's `DepthOfEffect` setter after manual target-drag testing exposed a property-grid refresh loop. The setter now normalizes before raising its change notification. A focused regression test confirms depth changes do not issue a `TypeDescriptor` refresh; the full build and focused Fire filter passed (42 tests).
- [x] (2026-08-31 23:15Z) The target-drag loop persisted because the property grid reapplied a stale depth after targeting normalized it back to its already-stored value. Fire and Wipe now notify bindings only if the final normalized depth changed. Focused Fire/Wipe tests passed 26/26, including new stale-selection notification regressions; the full `Vixen_Tests` build passed.
- [ ] Run focused and broader validation, update the Jira issue, and record final evidence in this plan.

## Surprises & Discoveries

- Observation: Fire already supports both string and preview-location positioning, but `PixelEffectBase._PreRender()` configures one buffer from all `TargetNodes` and has no per-target-group rendering seam.
  Evidence: `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs` calls `ConfigureDisplayElementSize()`, `SetupRender()`, and `RenderNode()` directly from its `_PreRender()` implementation; Fire enables both positioning modes in its constructor.
- Observation: Wipe provides the exact requested user-facing terminology and persistence defaults, including `TargetNodeSelection.Group`, but it is not a pixel effect and cannot be reused directly by Fire.
  Evidence: `src/Vixen.Modules/Effect/Wipe/WipeModule.cs` and `WipeData.cs` implement `TargetNodeHandling`, `DepthOfEffect`, and group-local rendering; Fire inherits `PixelEffectBase` instead of `BaseEffect` directly.
- Observation: Fire's dense heat field is mutable instance state and must be initialized and cleaned up separately for every independent group.
  Evidence: `src/Vixen.Modules/Effect/Fire/Fire.cs` allocates `_fireBuffer` in `SetupRender()`, mutates it once per frame in `GenerateFireBuffer()`, and clears it in `CleanUpRender()`.
- Observation: the VIX-3994 comment explicitly removes the initial location-only limitation.
  Evidence: Jira comment dated 2026-08-31: “This could be useful on strings or locations, so removing the original restriction that it should target only locations.”
- Observation: The characterization tests can protect legacy group-mode output without adding a deterministic random source to Fire.
  Evidence: `FireRender_DefaultGroupModeRendersLocatedLeavesTogether` passes through the existing Fire lifecycle and verifies both located leaves receive intents. The remaining focused failures all stop at missing `TargetNodeHandling`, `DepthOfEffect`, `TargetNodeSelection`, or their data members.
- Observation: Provider display and description attributes resolve resource keys directly through `EffectResourceManager`; they do not consume the generated strongly typed resource properties.
  Evidence: `ProviderDisplayNameAttribute` and `ProviderDescriptionAttribute` call `EffectResourceManager`, while the existing generated designer files do not contain Wipe's already-shipped `WipeTargetNodeSelection` key. Adding the Fire keys to the `.resx` files is therefore the complete runtime localization change.
- Observation: the previous pixel lifecycle configured one shared location buffer, then invoked the location renderer for every selected root; explicit render groups need one renderer invocation for the complete group instead.
  Evidence: `PixelEffectBase.RenderNodeByLocation()` renders every entry in `ElementLocations`, which already contains all leaves of the configured group. The scoped renderer now calls `RenderNodes()` once per group and merges the resulting intents.
- Observation: refreshing an effect's `TypeDescriptor` from the depth setter can re-enter the property-grid selector while a target change is already rebuilding it.
  Evidence: the target-drag stack trace repeatedly alternates `Fire.set_DepthOfEffect`, `TypeDescriptor.Refresh`, and WPF `ItemCollection.SetCollectionView`. `DepthOfEffect` visibility does not depend on the chosen value, so it can normalize and notify without refreshing the descriptor.
- Observation: removing the descriptor refresh alone is insufficient when a selector's stale value is normalized back to the value that was stored before its setter ran.
  Evidence: the second target-drag stack trace repeats `Fire.set_DepthOfEffect` through `PropertyItem.ComponentValueChanged` without `TypeDescriptor.Refresh` in the repeated cycle. Raising `PropertyChanged` for an unchanged final value causes the selector to reapply its stale value indefinitely.

## Decision Log

- Decision: Model VIX-3994 on Wipe's `Across Elements/Groups` and `Each Element/Group` behavior, including intermediate depth selection, rather than Spin/Chase's broader depth picker.
  Rationale: The issue explicitly names Wipe. Wipe hides depth unless individual mode has one sufficiently deep target and excludes leaf-equivalent depth values, avoiding controls that cannot alter Fire's observable grouping.
  Date/Author: 2026-08-31 / Codex
- Decision: Preserve group mode as Fire's legacy and default behavior.
  Rationale: Existing saved sequences must render unchanged. `TargetNodeSelection.Group` is enum value zero and `DepthOfEffect` is zero, but explicit constructor defaults and deserialization tests will prove compatibility rather than depend on implicit defaults.
  Date/Author: 2026-08-31 / Codex
- Decision: Render each individual group with its own buffer dimensions, locations/string layout, Fire setup, frame sequence, and cleanup.
  Rationale: Reusing the parent buffer would restart random heat but would still make flame scale and source edges depend on sibling groups. Independent local buffers are the user-visible meaning of “Each Element/Group.”
  Date/Author: 2026-08-31 / Codex
- Decision: Add a narrow protected scoped-rendering seam to `PixelEffectBase`; do not duplicate its buffer projection implementation inside Fire or mutate `TargetNodes` while rendering.
  Rationale: Fire needs both string and location rendering to honor the same groups. Temporarily replacing target state is unsafe during rendering, while copying the renderer would drift from existing pixel effects. The default seam must retain the current one-group behavior for every other pixel effect.
  Date/Author: 2026-08-31 / Codex
- Decision: Keep the useful-depth converter local to Fire, following Wipe's current isolation, unless implementation proves an existing shared converter can express the same filtered values without changing other effects.
  Rationale: `TargetElementDepthConverter` exposes depth zero and the terminal leaf-equivalent depth; changing it would alter Chase and Spin. A Fire-local converter avoids a cross-module dependency on Wipe and limits behavioral scope.
  Date/Author: 2026-08-31 / Codex
- Decision: Do not manually edit the generated resource designer files.
  Rationale: Runtime provider attributes use the embedded `.resx` resources via `EffectResourceManager`, and the designer files are already stale for Wipe's identical feature key. Editing generated code would not affect runtime behavior and would create avoidable generated-file drift.
  Date/Author: 2026-08-31 / Codex

## Outcomes & Retrospective

Planning is complete. No production code, tests, JIRA fields, or issue comments were changed while creating this ExecPlan.

Milestone 1 is complete. VIX-3994 now describes the Fire target-handling choice for both strings and preview locations, compatibility expectations for existing effects, user-facing acceptance criteria, and automated/manual validation outcomes. The issue remains in its existing Accepted status.

Milestone 2 is complete. `src/Vixen.Tests/Effects/FireTargetNodeSelectionTests.cs` adds seven focused tests. `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` completed successfully. The focused `FireTargetNodeSelection` test run passed the existing group-mode location rendering characterization and failed the other six tests only because the Milestone 3 data and property APIs are intentionally not implemented yet.

Milestone 3 is complete. Fire now serializes `TargetNodeSelection` and `DepthOfEffect`, defaults new and legacy data to group handling and depth zero, normalizes invalid persisted values, and preserves these values when cloned. `Fire.TargetNodeHandling` and `Fire.DepthOfEffect` expose the Wipe-compatible editor behavior; depth is visible only for a single deep individual target, and its local converter offers intermediate values only. Fire rendering itself remains unchanged for Milestone 4. The full `Vixen_Tests` build and focused filter passed with 10 tests.

Milestone 4 is complete. `PixelEffectBase` now exposes a documented protected render-group selector whose default is one group containing all selected targets. It configures, sets up, renders, cleans up, and clears location state independently for each supplied group, while merging every group's intents. Fire overrides that selector to preserve group mode, split multiple selected targets, or resolve one deep target at its selected depth. The shared string path builds a single scoped frame buffer and maps the scoped roots' elements once; the location path renders the scoped location buffer once. `FireTargetNodeSelectionTests` now verifies depth and multiple-target location groups use local buffers. The full `Vixen_Tests` build passed, and the focused Fire filter passed 41 tests.

Manual drag testing found a Fire property-grid refresh loop after a target change. The depth setter applies targeting normalization without refreshing `TypeDescriptor`, because a depth value does not affect which controls are visible. A follow-up trace showed that an unchanged final depth could still be broadcast to the selector, which then reapplied its stale value. Fire and Wipe now raise `PropertyChanged` and set dirty state only when normalization leaves a depth that differs from the prior stored value. Regression tests cover both the absence of descriptor refresh and the absence of a notification for a normalized stale value. The full build passed and the focused Fire/Wipe filter passed 26 tests.

At implementation completion, replace this entry with the final user-visible outcome, the exact validation results, any remaining limitations, and lessons that affected the final design.

## Context and Orientation

Vixen effects are modules. A target node is an element or a group selected in the sequence editor. A leaf is an element node that has no children. A target depth is a number of child levels below a selected target at which rendering should split into independent groups. A virtual buffer is the rectangular grid used for a location-positioned pixel effect; it includes blank coordinates between actual preview elements so the Fire heat simulation can spread across gaps.

`src/Vixen.Modules/Effect/Fire/Fire.cs` is the runtime Fire effect. It inherits `PixelEffectBase`, exposes Fire direction, height, hue shift, and brightness settings, and keeps the mutable `int[] _fireBuffer` heat field. `FireData.cs` is its serialized settings model. `FireDescriptor.cs` declares no parameter signature and requires no signature change for these effect-editor settings.

`src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs` is the shared base for grid-style effects. Today it renders all `TargetNodes` through one setup/render/cleanup sequence. Its string path builds a dense `PixelFrameBuffer`; its location path builds a `PixelLocationFrameBuffer` only for actual preview locations while Fire still maintains its dense heat field. The change must preserve that lifecycle and call ordering within each group.

`src/Vixen.Modules/Effect/Effect/BaseEffect.cs` supplies `DetermineDepth()` and protected `GetNodesAtEffectDepth(IElementNode node, int depthOfEffect)`. Depth zero means the leaf elements. A nonzero depth walks child nodes and falls back to leaves if it finds no nodes. Wipe, Chase, and Spin use `TargetNodeSelection` from `src/Vixen.Modules/Effect/Effect/TargetNodeSelection.cs`: `Group` displays as `Across Elements/Groups`, and `Individual` displays as `Each Element/Group`.

The required Fire behavior is:

- In group mode, all selected targets are one render group. This is the current Fire behavior and ignores `DepthOfEffect`.
- In individual mode with several selected targets, each selected target is one render group and depth is reset to zero/hidden.
- In individual mode with exactly one target, resolve the selected intermediate depth under that target. Each resolved node is a render group.
- For every render group, configure dimensions from only that group's leaves, allocate/seed/render/clean up one Fire heat field, and write intents only for that group's elements. This applies equally to `Strings` and `Locations`.
- Fire must not render empty groups, must respect the existing cancellation behavior exposed by the lifecycle, and must leave the base renderer's one-group behavior unchanged for all effects that do not opt in.

Use Wipe's useful depth rules. Show target handling only when there are multiple selected targets or one target has child depth greater than two. Show depth only in individual mode with exactly one target and at least one useful intermediate depth. Permit values `1` through `availableDepth - 2`; do not offer `0` or `availableDepth - 1`, since each resolves to leaf-level behavior after the group is rendered. If the target changes so the selected value is invalid, reset it to `1` when useful values exist, otherwise reset to `0` and force group mode where individual mode is no longer meaningful.

## Plan of Work

### Milestone 1: align Jira with the implementation contract

Before repository code changes, update VIX-3994 using `.agents/skills/jira/SKILL.md`. Keep the issue user-facing. State that Fire will offer `Across Elements/Groups` and `Each Element/Group` for both strings and preview locations, retain the first as the compatibility default, and independently restart/scale the fire for each individual group. Include the acceptance criteria from this plan and the focused/broader test commands below. Do not include internal class names in the Jira description.

At the end of this milestone, the tracker is sufficient for a reviewer to understand the outcome and test approach without this document.

### Milestone 2: characterize compatibility and the new grouping contract

Add `src/Vixen.Tests/Effects/FireTargetNodeSelectionTests.cs`. Keep the existing `FireLocationRenderTests.cs` focused on heat/projection math; add only narrowly related assertions there if a shared helper must be inspected. The test project already references the Fire module and Location property module, so do not add duplicate project references.

Test a new `Fire` and a new `FireData` for `TargetNodeSelection.Group` and depth zero. Test a payload or data instance that omits the new data members to prove legacy defaults. Test clone preservation once the data fields exist. Use `TypeDescriptor.GetProperties` to test target-handling and depth visibility for a shallow target, a single deep target in both modes, and multiple targets. Test the Fire-specific depth converter returns only `1..depth-2` and returns no choices for depth two or less.

Add end-to-end lifecycle tests using real or mock element hierarchies consistent with `WipeTargetNodeSelectionTests.cs` and `FireLocationRenderTests.cs`. Establish a deterministic Fire seam for tests without changing production random behavior: either provide a protected/internal testable random source only if existing patterns permit it, or assert group-local dimensions, output element membership, and independent buffer setup rather than exact random colors. Cover both target-positioning modes:

- Group mode uses one scoped render and produces output for the union of two separated child groups.
- Individual mode at an intermediate depth creates one scoped render per child group; both groups begin at their own source edge and neither group's buffer dimensions include its sibling.
- Multi-target individual mode creates one local render per selected target, hides/resets depth, and does not combine the two target geometries.
- Existing one-target string and location rendering tests still pass unchanged, including all four Fire directions and sparse location behavior.

Run the focused test filter before implementing the production behavior. It may initially fail only for the missing public property/data contract; record the exact result in `Progress` and `Surprises & Discoveries`.

### Milestone 3: add Fire persistence and property-grid behavior

Modify `src/Vixen.Modules/Effect/Fire/FireData.cs`. Add `[DataMember]` properties `int DepthOfEffect` and `TargetNodeSelection TargetNodeSelection`; initialize them to zero and `Group` in the constructor, copy both in `CreateInstanceForClone()`, and extend `OnDeserialized` to normalize an invalid enum and negative depth. Retain the existing hue-shift migration logic. These are public serialized APIs, so update XML documentation for the changed public members according to `.agents/skills/csharp-docs/SKILL.md`.

Add `src/Vixen.Modules/Effect/Fire/FireTargetElementDepthConverter.cs`, modeled on Wipe's converter but in the Fire namespace. It must derive from `TypeConverter`, resolve the minimum usable selected-target depth from the property-grid context, and return string choices from `1` inclusive to `depth - 1` exclusive. Document its public type and overridden behavior with XML comments. Do not reference the Wipe assembly.

Modify `Fire.cs` to add `TargetNodeHandling` under the `Behavior` category and `DepthOfEffect` under the `Depth` category. Use `[Value]`, the standard provider display/description attributes, a `SelectionEditor` for depth, and the Fire-specific converter. Both setters must update the data model, mark the effect dirty, notify the property grid, recalculate targeting visibility/normalization, and refresh `TypeDescriptor` exactly as Wipe does. Override `TargetNodesChanged()` to call `base.TargetNodesChanged()` first so existing string counts/orientation behavior remains intact, then update Fire targeting attributes and refresh the descriptor. Call the same attribute initialization from the constructor and `ModuleData` setter.

Add `FireTargetNodeSelection` entries to `src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDisplayNameDescriptors.resx` with value `Fire` and to `EffectDescriptionDescriptors.resx` with value `Determines how the Fire effect is applied across the target elements that are presented to the effect.` Regenerate or update the corresponding `.Designer.cs` files by the repository's established resource-generation process; do not hand-edit generated code if that process is available.

### Milestone 4: scope the pixel renderer and render Fire independently

Refactor `PixelEffectBase` only enough to render an explicit list of target roots as one scoped render. Preserve the current public behavior and default one-group path. The resulting protected seam must let a derived effect provide a sequence of render groups without assigning to `TargetNodes`, and it must perform, for each group, the following in order: calculate string counts or location bounds from that group's leaves; set up renderer state; render the group's string/location output once; merge its intents; clean up renderer state; clear per-group location state. Add XML documentation for every new or changed protected API.

Keep the existing default implementation equivalent to one group containing all `TargetNodes`. Do not make unrelated pixel effects opt in and do not change their serialized models. Extract parameterized private helpers only as necessary so group-scoped configuration cannot accidentally retain locations, string pixel counts, buffer offsets, or cached elements from the previous group. In location mode, render a group once against its own `PixelLocationFrameBuffer` and emit intents for that group's leaf elements once. In string mode, create/map the frame buffer once for the scoped set and preserve the existing orientation and element ordering semantics. Add a focused base-level regression test only if the new protected seam needs one; otherwise Fire's group-mode characterization test is the compatibility guard.

Override the new group-selection seam in `Fire.cs`. Return `[TargetNodes]` for group mode, each selected root for multiple-target individual mode, and one root per `GetNodesAtEffectDepth(TargetNodes.Single(), DepthOfEffect)` result for one-target individual mode. Ensure a target node is not duplicated within a group and skip empty results. Do not alter Fire's `GenerateFireBuffer`, palette clamping, direction coordinate mapping, random call order inside a single group, height/hue/level calculations, or sparse-location projection algorithm.

Run the focused tests after each refactoring step. If legacy group mode differs from the characterization, stop, restore the default path to a single scoped group containing all targets, and add evidence before continuing. If an existing pixel-effect test reveals that the shared default path changed, revert that broader behavior and narrow the seam further before proceeding.

### Milestone 5: validate, document results, and close the tracker loop

Run the focused Fire target-selection tests, the existing Fire location tests, and the repository test workflow specified below. Manually verify the effect editor and sequencer with a parent group containing two separated subgroups in both `Strings` and `Locations`: verify a single large fire in Across mode, then verify two locally scaled fires in Each mode. Save and reopen a sequence containing both settings and verify the choice persists. Also open an older Fire effect and verify it remains in Across mode with its prior output.

Update VIX-3994 if implementation evidence changes requirements or tests, then add a concise user-facing Jira comment with validation results. Update every living section of this plan with commands, pass/fail evidence, decisions, and final outcomes. When this milestone changes repository files, generate the required formatted commit message with `.agents/skills/commit-msg/SKILL.md`; do not create a commit unless explicitly asked.

## Concrete Steps

Run all commands from `C:\Dev\Vixen` in PowerShell.

First build the test target with full MSBuild because Vixen's test graph includes C++/CLI dependencies:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Then run the focused tests without rebuilding:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)\" --filter "FullyQualifiedName~FireTargetNodeSelection|FullyQualifiedName~FireLocationRender"

Expect all selected Fire tests to pass. The output should end with a successful test summary, for example:

    Passed!  - Failed:     0, Passed:    <count>, Skipped:     0, Total:    <count>

Run the broader already-built suite before finalizing:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)\"

If the build or broad test command fails for an unrelated pre-existing reason, preserve the output in `Artifacts and Notes`, run the narrow Fire filter to isolate this change, and report the unrelated blocker in Jira rather than weakening the new assertions.

## Validation and Acceptance

VIX-3994 is accepted when all of the following are demonstrably true:

- A newly created Fire effect and a legacy Fire data payload use `Across Elements/Groups` and depth zero by default; an existing Fire sequence looks the same after loading.
- The editor shows the Fire behavior choice only for multiple targets or a sufficiently deep single target. In individual mode, it shows a depth picker only for one deep target and offers intermediate values only.
- With a deep parent that contains two child groups, Across mode creates one Fire simulation across their combined string layout or preview rectangle. Each mode creates two independent simulations whose dimensions and source edges are local to their respective groups.
- With two separately selected targets, Each mode runs independently for each target in both string and location positioning and does not expose a stale depth selection.
- Fire's Bottom, Top, Left, and Right origins, sparse preview-location behavior, Height, Hue Shift, Brightness, and string-orientation behavior continue to pass the existing tests.
- The focused and broad test commands complete successfully, and the manual save/reopen test proves persisted settings.

## Idempotence and Recovery

The code and test steps are additive and safe to rerun. Do not delete or rewrite existing sequences. If a scoped-rendering refactor causes a regression, first restore `PixelEffectBase` so its default group is all `TargetNodes`, then retain the focused failing test and reintroduce only the protected Fire opt-in seam. If migration tests reveal a malformed saved value, normalize only invalid enum values and negative depths; do not overwrite valid individual-mode settings. If resource generation is unavailable, identify the established generation command before modifying designer output.

## Artifacts and Notes

The Jira description to apply in Milestone 1 is:

    ## Summary

    Add a choice to the Fire effect that controls whether one fire pattern spans all selected elements and groups or each selected element/group receives its own fire pattern.

    ## Scope

    - Support the choice for both string-based and preview-location Fire effects.
    - Keep the current across-target behavior as the default for existing sequences.
    - Allow a deeply nested target to split Fire at a useful child-group depth.
    - Preserve Fire direction, brightness, height, hue shift, and existing string/location behavior.

    ## Acceptance Criteria

    - Given an existing Fire effect, when it is opened after the change, then it remains an across-target Fire effect and retains its appearance.
    - Given a group containing multiple child props, when Each Element/Group is selected, then each prop has an independent locally scaled Fire effect.
    - Given two separately selected targets, when Each Element/Group is selected, then Fire renders independently for both string and preview-location targets.
    - Given a target without useful child depth, when Fire is edited, then unavailable target-depth controls are not shown.

No `FireDescriptor.Parameters` change is planned: it is currently empty, and these are persisted effect settings rather than descriptor parameters used by an existing signature. Revisit only if implementation discovers a caller that requires these fields in a parameter signature; record the evidence and compatibility decision here before changing it.

## Interfaces and Dependencies

The implementation must end with these persisted Fire data members in `VixenModules.Effect.Fire.FireData`:

    [DataMember]
    public int DepthOfEffect { get; set; }

    [DataMember]
    public TargetNodeSelection TargetNodeSelection { get; set; }

`VixenModules.Effect.Fire.Fire` must expose a `[Value]` `TargetNodeHandling` property backed by `FireData.TargetNodeSelection` and a `[Value]` `DepthOfEffect` property backed by `FireData.DepthOfEffect`. Use the existing `TargetNodeSelection` enum; do not create a Fire-specific enum.

`PixelEffectBase` must expose a protected, documented mechanism for a derived pixel effect to supply render groups. Its default must represent exactly one group containing all current `TargetNodes`; Fire is the only effect that opts in for this ticket. A render group is a non-empty collection of `IElementNode` roots that shares one complete pixel buffer and Fire heat-field lifecycle. This mechanism must not change `TargetNodes`, module data, or UI state while rendering.

---

Plan created 2026-08-31 / Codex. Reason: VIX-3994 requires a self-contained implementation plan following the established Wipe, Chase, and Spin target-node pattern while extending it safely to Fire's shared pixel rendering pipeline.

Plan revised 2026-08-31 / Codex. Reason: Milestone 1 updated VIX-3994 with the final user-facing requirements, acceptance criteria, and validation plan before repository implementation begins.

Plan revised 2026-08-31 / Codex. Reason: Milestone 2 added focused Fire target-selection characterization tests and recorded their expected pre-implementation results.

Plan revised 2026-08-31 / Codex. Reason: Milestone 3 implemented Fire data compatibility, property-grid target handling, filtered depth selection, and runtime localization resources.

Plan revised 2026-08-31 / Codex. Reason: Milestone 4 added target-scoped pixel rendering and Fire's independent group selection with focused lifecycle coverage.

Plan revised 2026-08-31 / Codex. Reason: Manual target-drag testing exposed a re-entrant Fire depth-selector refresh, which is now covered and avoided.

Plan revised 2026-08-31 / Codex. Reason: The follow-up target-drag trace exposed a stale selector value being re-notified after depth normalization; Fire and Wipe now suppress no-op depth notifications.

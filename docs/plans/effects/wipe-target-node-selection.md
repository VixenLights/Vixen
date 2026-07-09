# Add Target Node Selection to the Wipe Effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This repository contains `.agents/PLANS.md`; maintain this document according to that file. The feature specification that led to this plan is `docs/effects/wipe-target-node-selection.md`.

## Purpose / Big Picture

After this change, a Vixen user can place the Wipe effect on a parent group and choose whether the wipe runs once across the entire selected group or restarts independently for each child group at a selected hierarchy depth. Today Wipe always flattens all selected target leaves into one preview-location set, so a wipe across a parent group spans every child prop together. The new behavior keeps that default but adds an individual mode for users who want each prop or subgroup to receive its own local wipe.

The behavior is visible in the Vixen effect editor and sequencer. Existing Wipe effects should load and render exactly as they did before because their default target handling remains `Group` with depth `0`. New or edited Wipe effects on deep element hierarchies should expose target-node handling. In `Individual` mode, a single selected deep parent should expose only useful intermediate depth choices, excluding depth `0` and the maximum offered depth because both collapse to leaf-level rendering for Wipe.

## Progress

- [x] (2026-07-09 21:06Z) Created this ExecPlan from `docs/effects/wipe-target-node-selection.md` after reviewing Wipe, Chase, Spin, `BaseEffect`, `TargetElementDepthConverter`, and `.agents/PLANS.md`.
- [x] (2026-07-09 21:06Z) Created Jira issue VIX-3938 as a New Feature with purpose, compatibility requirements, design notes, automated test plan, manual testing steps, acceptance criteria, and ExecPlan reference.
- [x] (2026-07-10 03:28Z) Added focused Wipe target-node selection tests for module defaults, data defaults, serialized data members, legacy payload defaults, deep-target property visibility, and default group-mode rendering. The focused test command builds, passes the group-mode rendering characterization, and fails because Wipe does not yet expose the target/depth properties or data fields.
- [x] (2026-07-10 03:52Z) Implemented Wipe data compatibility and UI properties for target-node handling and depth selection. Added Wipe-specific filtered depth converter and localization resource entries. Focused tests now pass for Milestone 3 property/data behavior.
- [ ] Add or adopt the narrow shared depth helper if it reduces duplication without changing Chase or Spin behavior.
- [ ] Run focused and broader validation, then update Jira and this plan with final evidence.

## Surprises & Discoveries

- Observation: Wipe is already location-based, but it always combines all selected target leaves into one coordinate set.
  Evidence: `src/Vixen.Modules/Effect/Wipe/WipeModule.cs` builds `renderedNodes` in `_PreRender()` with `TargetNodes.SelectMany(x => x.GetLeafEnumerator())`.
- Observation: Chase and Spin implement the target-node selection behavior that Wipe should follow, but their depth UI differs from the desired Wipe behavior.
  Evidence: `src/Vixen.Modules/Effect/Chase/Chase.cs` and `src/Vixen.Modules/Effect/Spin/Spin.cs` both expose `TargetNodeHandling` and `DepthOfEffect`; their `UpdateTargetingAttributes()` methods show depth whenever `DetermineDepth() > 2`, while this plan requires Wipe depth to be hidden in group mode and filtered to useful intermediate depths only.
- Observation: `TargetElementDepthConverter` supports an `OffsetAttribute` that can remove depth 0 from the displayed values, but it does not support excluding the maximum offered depth.
  Evidence: `src/Vixen.Core/TypeConverters/TargetElementDepthConverter.cs` builds values from `0` to `depth - 1` plus an optional offset; no attribute exists to drop the final value.
- Observation: Wipe target-node selection tests now compile against the current code, preserve one passing default group-mode render characterization, and fail at runtime for the intended missing feature surface.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WipeTargetNodeSelection --no-restore` built `Vixen.Tests.dll`, passed 1 group-mode render test, and failed 6 tests because `WipeModule.TargetNodeHandling`, `WipeModule.DepthOfEffect`, `WipeData.TargetNodeSelection`, and `WipeData.DepthOfEffect` are absent.
- Observation: Milestone 3 property/data implementation satisfies the focused tests without changing Wipe's render path.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WipeTargetNodeSelection --no-restore` passed 8 tests after adding `WipeData` fields, `WipeModule` properties, target visibility updates, and `WipeTargetElementDepthConverter`.

## Decision Log

- Decision: Keep existing Wipe behavior as the default and migration target by setting legacy and new Wipe data to `TargetNodeSelection.Group` and `DepthOfEffect = 0`.
  Rationale: No existing Wipe effect should regress. Group mode matches the current `_PreRender()` behavior because it collects all leaves under all targets and renders one combined wipe.
  Date/Author: 2026-07-09 / Codex
- Decision: In Wipe individual mode, each independent child group uses its own local preview-location bounds.
  Rationale: The user wants each child group to receive a full local Wipe render. Sharing parent bounds would restart timing but still make each child group depend on the full parent geometry, which is not the requested behavior.
  Date/Author: 2026-07-09 / Codex
- Decision: Do not offer depth `0` or the maximum offered depth in Wipe individual-mode depth selection.
  Rationale: Depth `0` renders individual leaves one at a time, which behaves like a pulse per element. The maximum offered depth also resolves to leaf-level rendering after the render pass expands each selected group to leaves. Neither is useful as a Wipe group-depth choice.
  Date/Author: 2026-07-09 / Codex
- Decision: Keep this Jira and implementation scope strictly Wipe-only.
  Rationale: Chase and Spin are precedent, but changing their UI behavior is outside this feature and could surprise users who rely on the current controls.
  Date/Author: 2026-07-09 / Codex
- Decision: Consider only a small `BaseEffect` depth traversal helper, and add XML documentation if the helper is protected.
  Rationale: Chase and Spin duplicate the same `GetNodesToRenderOn` traversal. Wipe needs the same primitive. A small shared helper can reduce future duplication, but moving Wipe-specific property visibility or location rendering into the base class would be premature.
  Date/Author: 2026-07-09 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. Jira VIX-3938 now tracks the planned feature scope, compatibility requirements, automated tests, manual validation steps, and acceptance criteria.

Milestone 2 is complete. `src/Vixen.Tests/Effects/WipeTargetNodeSelectionTests.cs` now contains focused characterization tests, and `src/Vixen.Tests/Vixen.Tests.csproj` references the Wipe and Location modules. The focused test command builds successfully, passes 1 default group-mode rendering test, and currently fails 6 tests for the expected missing Wipe target/depth feature surface.

Milestone 3 is complete. Wipe now persists `TargetNodeSelection` and `DepthOfEffect`, defaults legacy and new data to `Group` and depth `0`, exposes `TargetNodeHandling` and `DepthOfEffect` in the property grid, hides depth outside useful single-target individual mode, resets depth to `0` for group and multiple-target modes, and filters Wipe depth choices to exclude `0` and the maximum offered target depth. The render path still uses the existing group behavior; individual rendering remains for Milestone 4.

## Context and Orientation

Vixen is a Windows desktop sequencing application for animated light shows. Effects are modules under `src/Vixen.Modules/Effect/`. A Wipe effect renders pulses across elements based on each element's preview X/Y location. In this plan, a "target node" is an element or group selected by the user as the target of an effect. A "leaf" is an element node with no child nodes. A "child group depth" is how many child levels below a selected parent should be treated as independent groups.

The Wipe effect lives in `src/Vixen.Modules/Effect/Wipe/`. The important files are:

- `src/Vixen.Modules/Effect/Wipe/WipeModule.cs`, the runtime effect class. It owns the effect properties, target-node change handling, and rendering.
- `src/Vixen.Modules/Effect/Wipe/WipeData.cs`, the serialized data model for Wipe settings.
- `src/Vixen.Modules/Effect/Wipe/WipeDescriptor.cs`, the module descriptor and parameter signature.
- `src/Vixen.Modules/Effect/Wipe/Wipe.csproj`, the Wipe module project.

The shared basic-effect base class is `src/Vixen.Modules/Effect/Effect/BaseEffect.cs`. It already provides `DetermineDepth()`, which returns the minimum maximum-child-depth across the current `TargetNodes`. This matters because target-node handling should only be shown when the selected targets have enough hierarchy depth to make the choice meaningful.

The target-selection enum is `src/Vixen.Modules/Effect/Effect/TargetNodeSelection.cs`. It defines:

    public enum TargetNodeSelection
    {
        [Description("Across Elements/Groups")]
        Group,
        [Description("Each Element/Group")]
        Individual
    }

Because `Group` is the first enum value, its numeric default is `0`. That helps legacy deserialization, but the implementation must still prove legacy behavior with a focused test rather than relying only on that assumption.

The existing depth dropdown converter is `src/Vixen.Core/TypeConverters/TargetElementDepthConverter.cs`. It uses the selected effect's first target node to calculate standard values. It can apply `Vixen.Attributes.OffsetAttribute` to shift the displayed values, but it cannot exclude the maximum offered depth. Wipe therefore needs a Wipe-specific converter or a narrow extension that filters both depth `0` and the maximum offered depth.

Chase and Spin are the precedent effects:

- `src/Vixen.Modules/Effect/Chase/Chase.cs` and `src/Vixen.Modules/Effect/Chase/ChaseData.cs`
- `src/Vixen.Modules/Effect/Spin/Spin.cs` and `src/Vixen.Modules/Effect/Spin/SpinData.cs`

Both persist `DepthOfEffect` and `TargetNodeSelection`. Both expose `TargetNodeHandling` in the `Behavior` category and `DepthOfEffect` in the `Depth` category. Both contain a private `GetNodesToRenderOn(IElementNode node)` helper that returns leaves for depth `0`, walks child nodes for greater depths, and falls back to leaves when the selected depth resolves to no nodes. Wipe should follow that traversal behavior internally, but it should not copy Chase and Spin's exact depth visibility rules because Wipe must hide depth in group mode and hide depth when no useful intermediate depth exists.

Wipe's current render pipeline in `WipeModule._PreRender()` works like this. It creates `_elementData`, collects all leaves under all `TargetNodes`, reads each leaf's `LocationData`, filters to nodes with positive X values, calculates the combined min/max X/Y bounds, groups nodes by wipe direction, and calls one of `RenderCount`, `RenderPulseLength`, or `RenderMovement`. The implementation should preserve this exact combined pipeline for group mode. Individual mode should run this same local pipeline once per independent child group and accumulate the resulting intents.

`LocationData` comes from `src/Vixen.Modules/Property/Location/`. Wipe currently reads it through each element's property bag with `s.Properties.Get(LocationDescriptor._typeId)` and casts the module data to `LocationData`.

The test project is `src/Vixen.Tests/Vixen.Tests.csproj`. Tests should be added under `src/Vixen.Tests/Effects/`, following the existing xUnit style. Add project references only if they are needed to instantiate Wipe and location data directly. Use project references rather than DLL references, consistent with repository guidance.

## Plan of Work

Begin by creating or updating a Jira issue for this feature. Use the project `jira` skill from `.agents/skills/jira/SKILL.md` when doing Jira work. The Jira issue should include the purpose, compatibility requirement, implementation notes, automated test plan, manual testing steps, and acceptance criteria from this plan. Do this before code changes so the tracker represents the intended scope.

Next, add characterization tests. These tests should prove the current and required behavior before broad implementation. At minimum, add tests that a new `WipeModule` defaults to `TargetNodeSelection.Group` and `DepthOfEffect == 0`, and that legacy Wipe data missing the new fields loads or migrates to the same values. Add property-visibility tests using `TypeDescriptor.GetProperties(effect, attributes, true)` or `effect.GetProperties()` if that is the established effect pattern. The tests should distinguish shallow targets, deep single targets, group mode, individual mode, and multiple selected target nodes.

Add rendering characterization for group mode before changing the render path. The safest test is to create a small target hierarchy with located leaves, run Wipe in its default group mode, and record that the render helper sees one combined location set or that the emitted intents match the current behavior for a deterministic setup. If direct intent comparison is too brittle, create a small internal helper for grouping only and test that helper. Keep any helper narrowly scoped and document the decision in this plan.

Modify `WipeData` in `src/Vixen.Modules/Effect/Wipe/WipeData.cs`. Add `[DataMember] public int DepthOfEffect { get; set; }` and `[DataMember] public TargetNodeSelection TargetNodeSelection { get; set; }`. Initialize `DepthOfEffect = 0` and `TargetNodeSelection = TargetNodeSelection.Group` in the constructor. Verify how the serializer handles missing new fields. If the test cannot prove missing enum and int fields become `Group` and `0`, or if invalid enum values can be loaded, add an `[OnDeserialized]` method that normalizes legacy or invalid values without changing newly saved effects that intentionally select `Individual`.

Modify `WipeModule` in `src/Vixen.Modules/Effect/Wipe/WipeModule.cs`. Add a `TargetNodeHandling` property with `[Value]`, `ProviderCategory(@"Behavior", 0)`, and a Wipe-specific display/description key such as `WipeTargetNodeSelection`. Add a `DepthOfEffect` property in the `Depth` category with `SelectionEditor` and a filtered depth converter. The setter for `TargetNodeHandling` should mark the effect dirty, raise property changed, update target-related attribute visibility, and refresh the type descriptor.

Implement filtered depth choices for Wipe. Prefer a Wipe-specific converter if that avoids changing shared behavior. A suitable type name is `WipeTargetElementDepthConverter` in the Wipe namespace. It should inspect the current `IEffect` or `IEffect[]` from the type descriptor context, compute the available depth from the first target node or the minimum across selected effects, and return standard values that exclude `0` and the maximum offered target depth. For example, if the normal converter would return `0, 1, 2, 3`, Wipe should return `1, 2`. If there are no useful intermediate depths, return an empty standard value collection and hide the property through attribute state.

Add target-attribute update logic in `WipeModule`. It should call `DetermineDepth()`, force `TargetNodeHandling = Group` when depth is less than `3`, reset `DepthOfEffect = 0` when multiple target nodes are selected, reset out-of-range or filtered-out depths to the first useful depth when a single deep target is selected in individual mode, and call `SetBrowsable()` for `TargetNodeHandling` and `DepthOfEffect`. `TargetNodeHandling` is visible when more than one target node is selected or depth is greater than `2`. `DepthOfEffect` is visible only when `TargetNodeHandling == Individual`, exactly one target node is selected, depth is greater than `2`, and at least one useful intermediate depth exists after filtering.

Refactor Wipe rendering narrowly. Extract the current location collection, bounds calculation, direction grouping, and movement dispatch into a helper that accepts a set of root nodes to render. Group mode calls this helper once with all current `TargetNodes`, preserving today's behavior. Individual mode resolves independent render groups and calls this helper once per group. The helper should collect leaves under the supplied group roots, calculate local min/max X/Y values only for those leaves, call the existing `GetRenderedDiagonal`, `GetRenderedLRUD`, `GetRenderedCircle`, `GetRenderedDiamond`, or `GetRenderedRectangle`, and then call the existing render method for the selected `WipeMovement`.

Add a depth traversal helper. If implementation can keep it small and tests remain focused, add this protected helper to `BaseEffect`:

    /// <summary>
    /// Gets the element nodes selected by a depth setting for the specified target node.
    /// </summary>
    /// <param name="node">Target node to traverse.</param>
    /// <param name="depthOfEffect">Depth value where 0 means leaf elements.</param>
    /// <returns>Distinct nodes selected by the requested depth, or distinct leaf elements when the depth has no nodes.</returns>
    protected List<IElementNode> GetNodesAtEffectDepth(IElementNode node, int depthOfEffect)

Use the project `csharp-docs` skill before adding this protected API because repository instructions require XML documentation for public or protected C# API changes. Do not refactor Chase or Spin to use the helper in this ticket unless the implementation naturally touches shared code and tests prove no behavior changed. If the base helper feels too invasive during implementation, keep the helper private in Wipe and record that decision in `Decision Log`.

Review `WipeDescriptor`. If Wipe parameter signatures are used for presets, mapping, or automation in this codebase, append new parameters for target node selection and depth rather than inserting them in the middle. If the existing Wipe signature is compatibility-sensitive or unused for these properties, leave it unchanged and record the reason in `Decision Log`.

Add localization resource entries if provider display or description keys require them. Chase and Spin use `ChaseTargetNodeSelection` and `SpinTargetNodeSelection`. Wipe should use a Wipe-specific key such as `WipeTargetNodeSelection` so the effect editor shows a meaningful label and description. Do not modify Chase or Spin resources.

Complete the tests. Add coverage for default data values, legacy migration, property visibility, filtered depth options, group-mode preservation, individual mode local bounds, multiple target node behavior, cancellation preservation if practical, and missing location data. Keep tests focused and deterministic. If effect-render tests are difficult, at least test the grouping and target resolution helpers directly, then add one end-to-end render test that proves individual mode restarts the wipe per child group.

Finally, run focused tests and broader validation, update Jira with the implementation summary and test evidence, and update this plan's living sections.

## Milestones

Milestone 1 creates or updates the Jira issue before implementation starts. Use the project `jira` skill from `.agents/skills/jira/SKILL.md`. The Jira issue should include the purpose, compatibility requirement that legacy Wipe effects load as group mode with depth `0`, the filtered-depth UI rule, the local-bounds individual rendering design, automated test plan, manual testing steps, and acceptance criteria. At the end of this milestone, a reviewer can inspect Jira and understand the planned feature without reading the code. Record the Jira key in `Progress` and in this plan's notes.

Milestone 2 adds characterization tests and proves the current gaps. Add tests under `src/Vixen.Tests/Effects/`, with project references only if required. The focused test command is:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WipeTargetNodeSelection --no-restore

Before implementation, expect tests for target handling visibility, depth properties, or individual rendering to fail because Wipe has no such properties or behavior. Default group-mode tests should pass or be adjusted to capture current behavior. At the end of this milestone, the failure should demonstrate the missing feature rather than a compile error.

Milestone 3 implements data compatibility and UI properties. Modify `WipeData` and `WipeModule` so new Wipe effects and legacy Wipe data default to `Group` and depth `0`, `TargetNodeHandling` appears only when supported, and `DepthOfEffect` appears only for useful single-target individual mode. Implement filtered depth choices that exclude `0` and the maximum offered depth. At the end of this milestone, property and migration tests should pass.

Milestone 4 implements the render path. Refactor Wipe `_PreRender()` so group mode uses the current combined location behavior and individual mode runs the existing Wipe rendering pipeline independently for each resolved child group. At the end of this milestone, group-mode render characterization should still pass, and individual-mode tests should show local bounds and a wipe restart per child group.

Milestone 5 evaluates the small shared helper. Decide whether to add `BaseEffect.GetNodesAtEffectDepth(IElementNode node, int depthOfEffect)` or keep the helper private to Wipe. If adding the protected helper, use the project `csharp-docs` skill and include XML documentation. Do not change Chase or Spin behavior in this milestone. Record the decision and rationale in `Decision Log`.

Milestone 6 performs final validation and tracker updates. Run the focused Wipe tests, then run either the full `src\Vixen.Tests\Vixen.Tests.csproj` suite or a narrower build/test command if the full suite is not practical. Manually validate in Vixen by applying Wipe to a parent group containing at least two located child groups. Confirm group mode renders one combined wipe and individual mode restarts locally for each child group. Update Jira with the final implementation notes and validation evidence.

## Concrete Steps

Work from the repository root:

    cd C:\Dev\Vixen

Before editing, check the worktree and avoid mixing unrelated changes into this task:

    git status --short

Read the files that define the behavior:

    Get-Content -LiteralPath docs\effects\wipe-target-node-selection.md
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Wipe\WipeModule.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Wipe\WipeData.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Wipe\WipeDescriptor.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Chase\Chase.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Spin\Spin.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Effect\BaseEffect.cs
    Get-Content -LiteralPath src\Vixen.Core\TypeConverters\TargetElementDepthConverter.cs

Create or update the Jira issue using the project `jira` skill. The Jira description should include this content in substance:

    Purpose: Add target-node selection to Wipe so users can choose the existing combined group behavior or an individual mode that restarts Wipe per selected child group.

    Compatibility: Existing and legacy serialized Wipe effects must load as Group mode with DepthOfEffect = 0 and render exactly as they do today.

    Design: Add WipeData fields for TargetNodeSelection and DepthOfEffect. Add Wipe properties and filtered depth choices. Preserve group-mode rendering by running the existing combined location pipeline once. Add individual mode by resolving depth-selected child groups and running the same location pipeline once per group with local bounds.

    Acceptance: Wipe defaults to Group; legacy data migrates to Group/depth 0; depth is hidden in Group mode; depth choices exclude 0 and max offered depth; individual mode renders each selected child group independently; group mode preserves current output; focused tests cover migration, visibility, depth filtering, and rendering.

    Manual testing: Use a Vixen profile with a parent group containing at least two child groups that have preview locations. Add Wipe to the parent group. In Group mode, scrub the sequencer and verify one combined wipe crosses all child groups. Switch to Individual mode, choose an intermediate depth, scrub again, and verify the wipe restarts locally for each selected child group. Confirm the depth dropdown excludes 0 and the maximum offered target depth. Load or create an older Wipe effect and verify it still opens in Group mode and renders like it did before this feature.

Add tests under a file such as:

    src\Vixen.Tests\Effects\WipeTargetNodeSelectionTests.cs

If direct test instantiation requires references, add project references to `src\Vixen.Tests\Vixen.Tests.csproj` using existing project-reference style. Likely candidates are:

    <ProjectReference Include="..\Vixen.Modules\Effect\Wipe\Wipe.csproj" />
    <ProjectReference Include="..\Vixen.Modules\Property\Location\Location.csproj" />

Do not create a new project or edit `Vixen.sln`.

Run focused tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WipeTargetNodeSelection --no-restore

After implementation, run broader validation. Prefer the full test project when practical:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If the full suite is too slow or blocked by unrelated environment requirements, run a build plus the focused tests and record the limitation in `Outcomes & Retrospective`:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WipeTargetNodeSelection --no-restore

Manual validation should use a Vixen profile with a parent group containing at least two child groups that have preview locations. Add Wipe to the parent group. In group mode, scrub the sequencer and observe one wipe across all child groups combined. Switch to individual mode and choose an intermediate depth. Scrub again and observe the wipe restart locally on each child group. Confirm the depth dropdown does not offer `0` or the maximum offered target depth.

## Validation and Acceptance

This feature is accepted when all of these are true:

Existing and legacy Wipe effects are safe. A newly constructed `WipeModule` has `TargetNodeSelection.Group` and `DepthOfEffect == 0`. Serialized Wipe data that predates these fields loads or migrates to `TargetNodeSelection.Group` and `DepthOfEffect == 0`. Group mode ignores depth and renders the same combined target behavior as the current implementation.

The effect editor exposes the new controls only when useful. Target-node handling is visible when multiple target nodes are selected or a single target has depth greater than `2`. Depth is visible only when target handling is `Individual`, exactly one target node is selected, and useful intermediate depths exist. The depth dropdown excludes `0` and the maximum offered target depth.

Individual mode renders independently. For one selected deep parent, Wipe resolves the selected intermediate depth into child groups, computes local bounds for each group, and renders the full Wipe duration per group. Missing location data in one group does not prevent sibling groups from rendering. Multiple selected target nodes reset depth to `0`, hide depth selection, and render each selected node independently in individual mode.

Automated validation passes. Run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WipeTargetNodeSelection --no-restore

Expect all Wipe target-node selection tests to pass. If the full suite is run:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Expect all tests to pass, or document any unrelated pre-existing failures with enough evidence that they are not caused by this feature.

Manual validation is recorded in this plan. The person validating should describe the target hierarchy, the selected Wipe settings, and the observed difference between group and individual mode.

## Idempotence and Recovery

The implementation is additive and should be safe to retry. Re-running tests is safe. Adding data members to `WipeData` is backward-compatible only if legacy load behavior is proven or normalized, so do not skip the migration test. If a first attempt at a shared `BaseEffect` helper causes broad compile failures or forces Chase/Spin changes, revert that helper work and keep the traversal private to Wipe. This plan explicitly allows that narrower path.

Do not use destructive Git commands to recover from failed edits. Use `git status --short` to identify touched files, inspect diffs, and apply targeted patches. If unrelated user changes appear, leave them intact and work around them.

If a render refactor changes group-mode behavior, stop and add a focused characterization test that captures the old behavior. Restore the group path to call the extracted helper once with all current `TargetNodes`; do not continue with individual-mode work until group mode is stable.

## Artifacts and Notes

Important evidence to collect during implementation:

    Focused test command:
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WipeTargetNodeSelection --no-restore

    Expected final focused result:
    Passed!  - Failed: 0, Passed: <number>, Skipped: 0

    Manual validation note format:
    Profile: <profile name or test setup>
    Target hierarchy: <parent and child group names>
    Group mode observation: <one combined wipe across all child groups>
    Individual mode observation: <wipe restarts locally per selected depth child group>
    Depth dropdown observation: <0 and maximum offered depth are absent>

Revision note, 2026-07-09 / Codex: Initial ExecPlan created from `docs/effects/wipe-target-node-selection.md`. The plan includes Jira creation, legacy migration, Wipe-only scope, filtered depth choices, group-mode preservation, individual local-bounds rendering, and an optional narrow `BaseEffect` helper.

Revision note, 2026-07-09 / Codex: Updated the Jira creation guidance to require manual testing steps in the Jira issue, not only automated unit-test coverage. This ensures the tracker captures UI validation for group mode, individual mode, filtered depth choices, and legacy Wipe behavior.

Jira issue created, 2026-07-09 / Codex: VIX-3938, "Add target node selection and depth support to Wipe effect", created as a New Feature in the VIX project. The issue includes the ExecPlan reference, compatibility requirements, automated test plan, manual testing steps, and acceptance criteria.

## Interfaces and Dependencies

At completion, `src/Vixen.Modules/Effect/Wipe/WipeData.cs` must contain serialized settings equivalent to:

    [DataMember]
    public int DepthOfEffect { get; set; }

    [DataMember]
    public TargetNodeSelection TargetNodeSelection { get; set; }

The constructor must initialize:

    DepthOfEffect = 0;
    TargetNodeSelection = TargetNodeSelection.Group;

If required by legacy deserialization behavior, `WipeData` must contain an `[OnDeserialized]` method that normalizes missing or invalid values while preserving intentionally saved individual-mode values.

At completion, `src/Vixen.Modules/Effect/Wipe/WipeModule.cs` must expose:

    public TargetNodeSelection TargetNodeHandling { get; set; }

    public int DepthOfEffect { get; set; }

`TargetNodeHandling` must be a `[Value]` property in the `Behavior` category. `DepthOfEffect` must be a `[Value]` property in the `Depth` category and use a selection editor with filtered standard values.

If a shared helper is added to `src/Vixen.Modules/Effect/Effect/BaseEffect.cs`, it must be protected, documented with XML comments, and behave as follows:

    protected List<IElementNode> GetNodesAtEffectDepth(IElementNode node, int depthOfEffect)

Depth `0` returns distinct leaf elements. Depth greater than `0` walks children by the requested depth and returns distinct nodes. Empty results fall back to distinct leaf elements.

No Chase or Spin behavior may change as part of this plan. They are reference implementations only.

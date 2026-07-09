# Wipe Target Node Selection Specification

## Purpose

The Chase and Spin effects let users choose whether an effect runs across all selected targets as one group or independently
for each selected element/group. Wipe currently always flattens all target leaves into one location set, so a wipe across a
parent group spans every child prop together. This feature extends Wipe with the same target-node handling concept so users can
apply Wipe independently to child groups at a selected hierarchy depth.

This document is a feature specification and viability review. The feature is viable with the decisions recorded below. Create
a Jira issue from the Jira-ready summary near the end of this document, then convert this specification into an ExecPlan under
`docs/plans/effects/` before implementation.

## Current Behavior

`src/Vixen.Modules/Effect/Wipe/WipeModule.cs` inherits from `BaseEffect`.

On target changes, Wipe only calls `CheckForInvalidColorData()`. It does not currently:

- inspect the target hierarchy depth,
- expose `TargetNodeSelection`,
- expose a depth selector,
- reset an out-of-range depth setting, or
- refresh browsable target-related attributes.

During prerender, Wipe always builds one location set:

```csharp
TargetNodes.SelectMany(x => x.GetLeafEnumerator())
```

Each leaf is mapped to its `LocationData` X/Y/Z values, filtered to positive X values, grouped into wipe positions according to
`Direction`, and rendered by the selected `WipeMovement` mode. Because all selected target leaves are combined before the wipe
geometry is calculated, the effect uses one bounding rectangle and one wipe timing sequence across all selected targets.

## Related Precedent

Chase and Spin already implement the target-selection model that Wipe should follow.

Both effects persist:

- `DepthOfEffect`
- `TargetNodeSelection`

Both expose:

- `TargetNodeHandling` in the `Behavior` category,
- `DepthOfEffect` in the `Depth` category with `TargetElementDepthConverter`, and
- property visibility driven by `DetermineDepth()`.

Their current visibility behavior is:

- `TargetNodeHandling` is visible when more than one target node is selected or the selected hierarchy depth is greater than 2.
- `DepthOfEffect` is visible when selected hierarchy depth is greater than 2.
- If depth drops below 3 while `TargetNodeHandling` is `Individual`, the effect is forced back to `Group`.

Their render behavior is:

- `Group` mode renders across one selected group when a single target is selected.
- `Individual` mode first finds child nodes at `DepthOfEffect`, then renders each of those nodes separately.
- `DepthOfEffect == 0` means leaf elements.
- A depth that resolves to no nodes falls back to leaf elements.
- When multiple target nodes are selected, `DepthOfEffect` is reset to 0 and each selected target can be treated as its own
  independent unit in individual mode.

## Proposed User Behavior

Wipe gains a target handling setting with two values:

- `Across Elements/Groups` (`TargetNodeSelection.Group`)
- `Each Element/Group` (`TargetNodeSelection.Individual`)

The default remains `Group`, preserving current Wipe behavior for existing and new effects.

When Wipe is in `Group` mode:

- Existing behavior is preserved.
- Wipe collects all leaf elements under all `TargetNodes`.
- Wipe calculates one coordinate bounding rectangle.
- Wipe renders one wipe sequence across the full combined target set.
- Depth is not used for rendering.

When Wipe is in `Individual` mode:

- Wipe determines the independent render groups from the selected target hierarchy.
- For a single selected parent node, `DepthOfEffect` selects the child depth that should be treated as independent groups.
- For a single selected parent node, the depth selector should offer only useful child-group depths. It should not offer 0 or
  the maximum depth because both resolve to leaf-level rendering for Wipe.
- For multiple selected target nodes, each selected target node is treated as an independent group and `DepthOfEffect` is reset
  to 0, matching Chase and Spin.
- Each independent group calculates its own location bounding rectangle and wipe position groups.
- Each independent group renders the full Wipe effect duration using the current Wipe settings.
- The resulting intents from all groups are accumulated into the effect output.

## Depth Behavior

Add `DepthOfEffect` to Wipe data and UI.

The internal depth meaning matches Chase and Spin:

- `DepthOfEffect == 0`: use leaf elements for the selected node.
- `DepthOfEffect > 0`: walk down the selected node's children `DepthOfEffect` times and use those nodes as the render groups.
- If the selected depth produces no nodes, fall back to leaf elements.

For Wipe individual mode, 0 and the maximum offered target depth are not useful user selections:

- Depth 0 renders each leaf independently, which is effectively just a pulse on each element.
- The maximum offered depth also resolves to leaf-level rendering once the render pass collects leaves for each selected render
  group.

The UI should exclude both values from the `DepthOfEffect` dropdown. Keep `DepthOfEffect == 0` as the serialized default and as
an internal fallback/reset value for backward compatibility and for multiple selected target nodes.

Depth is only meaningful in `Individual` mode. The UI should hide `DepthOfEffect` unless all of these are true:

- target hierarchy depth is greater than 2, and
- `TargetNodeHandling == Individual`, and
- exactly one target node is selected, and
- at least one useful non-leaf child-group depth exists after excluding 0 and the maximum depth.

This intentionally differs from Chase and Spin, which currently show `DepthOfEffect` whenever depth is greater than 2. The
requested Wipe behavior says depth is not needed in `Group` mode, so Wipe should avoid showing a setting that does not affect
rendering. Do not change Chase or Spin as part of this feature.

## Resolved Decisions

- Individual Wipe render groups use their own local preview-location bounds.
- `DepthOfEffect` is hidden in `Group` mode.
- `DepthOfEffect == 0` and maximum depth are not offered as individual-mode depth choices because both resolve to leaf-level
  rendering for Wipe.
- Multiple selected target nodes reset depth to 0 and each selected node renders independently.
- The Jira and implementation scope is strictly Wipe. Do not change Chase or Spin behavior.

## Implementation Design

### Data Model

Modify `src/Vixen.Modules/Effect/Wipe/WipeData.cs`:

- Add `[DataMember] public int DepthOfEffect { get; set; }`.
- Add `[DataMember] public TargetNodeSelection TargetNodeSelection { get; set; }`.
- Initialize `DepthOfEffect = 0`.
- Initialize `TargetNodeSelection = TargetNodeSelection.Group`.
- Add an `[OnDeserialized]` migration hook if needed to force legacy data that does not contain the new fields to
  `TargetNodeSelection.Group` and `DepthOfEffect = 0`.
- Update clone behavior if `MemberwiseClone()` is replaced with explicit cloning during implementation.

Because WipeData inherits `EffectTypeModuleData`, adding new `[DataMember]` properties should preserve backward compatibility
for existing sequences, but do not rely only on constructor defaults. Existing serialized Wipe effects may be deserialized
without running the constructor in a way that initializes new members. The implementation must explicitly verify legacy
deserialization behavior and add a migration step when required so older Wipe effects behave exactly as they did before this
feature.

### Legacy Data Migration

Existing Wipe effects must deserialize into the old rendering behavior:

- `TargetNodeSelection` must become `Group`.
- `DepthOfEffect` must become `0`.
- Group-mode rendering must ignore depth and flatten all target leaves under `TargetNodes`, matching the current Wipe pipeline.

If the serializer leaves missing enum data at the enum default and `Group` remains the zero value, that is acceptable only if a
focused legacy-deserialization test proves it. If that behavior is not guaranteed or cannot be proven, add an explicit
`[OnDeserialized]` method in `WipeData` to normalize missing/invalid values. The migration hook should be narrow and should not
change newly saved Wipe effects that intentionally select `Individual`.

### Properties and Attributes

Modify `src/Vixen.Modules/Effect/Wipe/WipeModule.cs`:

- Add `using Vixen.TypeConverters;`.
- Add a `TargetNodeHandling` property in the `Behavior` category using `TargetNodeSelection`.
- Add a `DepthOfEffect` property in the `Depth` category using `SelectionEditor`.
- Do not expose the unfiltered `TargetElementDepthConverter` values directly for Wipe individual mode. It can hide 0 through
  `OffsetAttribute`, but it still offers the maximum depth. Use a Wipe-specific converter or a narrowly extended converter
  option that excludes both 0 and the maximum depth for this property.
- Add a target-attribute update method similar to Chase and Spin.
- Call the target-attribute update method from the constructor, `ModuleData` setter, `TargetNodesChanged()`, and the
  `TargetNodeHandling` setter.
- Refresh the type descriptor when target-node handling or depth visibility changes.

Preferred visibility rules:

- `TargetNodeHandling` visible when `TargetNodes.Length > 1 || DetermineDepth() > 2`.
- `DepthOfEffect` visible when `TargetNodeHandling == TargetNodeSelection.Individual`, `TargetNodes.Length == 1`, and
  `DetermineDepth() > 2`.
- If `DetermineDepth() < 3` and `TargetNodeHandling == Individual`, force `TargetNodeHandling = Group`.

### Render Group Selection

Extract Wipe's current prerender location collection into a helper that can render one node set at a time.

Proposed structure:

- `_PreRender()` initializes `_elementData`.
- If `TargetNodeHandling == Group`, call a helper once with the current `TargetNodes`.
- If `TargetNodeHandling == Individual`, enumerate independent groups and call the same helper once per group.
- The helper collects leaf elements for its supplied group, calculates the local bounds, creates wipe position groups, and
  invokes `RenderCount`, `RenderPulseLength`, or `RenderMovement`.

This keeps Wipe's direction and movement math local to each render group in individual mode.

### Node Selection Helper

Add a helper equivalent to Chase and Spin's `GetNodesToRenderOn(IElementNode node)`:

- For depth 0, return `node.GetLeafEnumerator().Distinct().ToList()`.
- For depth greater than 0, walk children down by `DepthOfEffect` and return distinct nodes.
- Fall back to leaf elements when the depth walk produces no nodes.

For Wipe individual rendering, the helper should be used in two phases for a single selected parent:

1. Resolve independent group nodes at `DepthOfEffect`.
2. Render each independent group by collecting its leaves and locations.

When `DepthOfEffect == 0`, this renders individual leaf pixels one at a time and acts like a pulse on each element. The maximum
depth also resolves to leaf-level rendering after each selected render group is expanded to leaves. Because neither outcome is
useful for Wipe individual mode, the UI should not offer either value even though the render helper should tolerate them.

### BaseEffect Refactor Option

Chase and Spin currently duplicate target-depth helper logic. Wipe needs similar behavior, and future effects may benefit from
the same primitive. Consider a small `BaseEffect` helper if implementation can keep the change narrow.

Recommended candidate:

```csharp
protected List<IElementNode> GetNodesAtEffectDepth(IElementNode node, int depthOfEffect)
```

Expected behavior:

- `depthOfEffect == 0` returns distinct leaf elements.
- `depthOfEffect > 0` walks children by the requested depth and returns distinct nodes.
- Empty results fall back to distinct leaf elements.

This would let Wipe use shared depth traversal without changing Chase or Spin in the same ticket. Updating Chase and Spin to use
the helper can be left as a future cleanup unless the Wipe implementation naturally touches the shared code and the tests remain
low risk.

Do not move Wipe-specific target handling, property visibility, location grouping, or rendering into `BaseEffect`. Those
behaviors are effect-specific enough that a base-class abstraction would be premature for this feature.

### Descriptor Parameters

Review `src/Vixen.Modules/Effect/Wipe/WipeDescriptor.cs`.

Chase and Spin include `"Depth of Effect"` in their `ParameterSignature`; Wipe currently exposes only color gradient, direction,
curve, and pulse time. The implementation should determine whether Wipe parameter signatures are actively used for preset,
mapping, or automation workflows. If yes, add:

- `"Target Node Selection"` or equivalent, type `TargetNodeSelection`
- `"Depth of Effect"`, type `int`

If existing Wipe parameter ordering must remain stable for compatibility, do not insert new parameters into the middle of the
signature.

### Localization

Add display and description resource entries for Wipe target handling if existing provider descriptor resources are required:

- `WipeTargetNodeSelection`

The user-facing strings should match Chase and Spin's concept unless product wording is deliberately changed.

## Edge Cases

- Existing Wipe effects must continue to render exactly as before because the default target handling is `Group`.
- Existing serialized Wipe effects that predate this feature must migrate or deserialize to `TargetNodeSelection.Group` and
  `DepthOfEffect = 0`.
- A Wipe on a flat element group should hide target handling and depth.
- A Wipe on a hierarchy with depth 2 or less should force `Group`.
- A Wipe on a deep hierarchy should allow `Individual`.
- In `Individual` mode, each child group should render with its own local X/Y bounds, not the parent group's combined bounds.
- If a child group has no usable location data, it should render nothing and must not prevent sibling groups from rendering.
- Cancellation should still be honored inside each independent render pass.
- Discrete color handling should continue to use the existing `HasDiscreteColors` behavior.

## Test Strategy

Add focused tests if the test harness can instantiate Wipe with synthetic element nodes and location properties.

Recommended tests:

1. New Wipe defaults to `TargetNodeSelection.Group` and `DepthOfEffect == 0`.
2. Legacy Wipe data that lacks `TargetNodeSelection` and `DepthOfEffect` deserializes or migrates to
   `TargetNodeSelection.Group` and `DepthOfEffect == 0`.
3. Existing group-mode rendering remains equivalent to the current leaf-flattening behavior.
4. Target handling is hidden for shallow target hierarchies.
5. Target handling is visible for a single deep target hierarchy.
6. Depth is visible only when individual mode is selected for exactly one target node and useful intermediate depths exist.
7. Depth options exclude 0 and the maximum offered target depth.
8. Individual mode renders each depth-selected child group with local bounds, producing a restart of the wipe per child group.
9. Multiple selected target nodes reset depth to 0, hide depth selection, and render each selected target independently in
   individual mode.

If effect-render tests are expensive to construct, first add lower-level characterization tests around target group resolution and
property visibility, then add at least one integration-style render test for local-bounds behavior.

## Viability

This feature appears viable.

Wipe already has the needed rendering pipeline structure: collect locations, calculate bounds, convert locations into wipe
position groups, and render intents. The main change is to run that existing pipeline once per selected child group instead of
only once for the full combined target set.

The primary design risk is user expectation around depth semantics. Chase and Spin use the depth setting both to choose nodes to
render on and to choose independent child groups. For Wipe, the useful behavior is "which child groups get their own wipe
coordinate space." Depth 0 and maximum depth both collapse to leaf-level rendering, which is not useful as a Wipe depth choice
and may be expensive on large selections, so they should be filtered from the UI.

The primary implementation risk is preserving exact group-mode behavior while factoring the current `_PreRender()` logic into
helpers. Keep the refactor narrow and verify with a group-mode characterization test if possible.

## Jira-Ready Draft

Title:

Add target node selection and depth support to Wipe effect

Description:

Extend the Wipe effect with the same target-node handling concept used by Chase and Spin. Wipe should default to applying across
the selected group as it does today, but users with deeper target hierarchies should be able to select individual mode and choose
the hierarchy depth whose child groups each receive an independent Wipe render. In individual mode, each selected child group
should render the full Wipe duration using its own location bounds so the wipe restarts per child group.

Acceptance criteria:

- Existing Wipe effects continue to default to group mode and render as they do today.
- Legacy serialized Wipe effects that lack the new fields load as group mode with depth 0.
- Wipe exposes target-node handling only when the selected target hierarchy supports it.
- Wipe exposes depth selection for individual mode only when useful intermediate depths exist.
- Wipe depth selection excludes 0 and the maximum offered target depth.
- Individual mode renders each selected child group independently.
- Group mode ignores depth and preserves the current combined-target wipe behavior.
- Invalid or out-of-range depth values are reset consistently with Chase and Spin.
- Focused tests cover default data values, target-property visibility, and at least one individual render grouping scenario.

Manual testing:

- Use a Vixen profile with a parent group containing at least two child groups that have preview locations.
- Add Wipe to the parent group, leave target handling in group mode, scrub the sequencer, and verify one combined wipe crosses
  all child groups.
- Switch Wipe to individual mode, choose an intermediate depth, scrub again, and verify the wipe restarts locally for each
  selected child group.
- Confirm the depth dropdown excludes 0 and the maximum offered target depth.
- Load or create an older Wipe effect and verify it still opens in group mode and renders like it did before this feature.

Implementation notes:

- Use Chase and Spin as behavioral precedent.
- Add serialized `DepthOfEffect` and `TargetNodeSelection` fields to `WipeData`.
- Refactor Wipe prerendering so the existing location grouping/rendering pipeline can be invoked once per independent target
  group.
- Consider adding a small shared `BaseEffect` helper for depth-based node traversal, but keep behavior changes scoped to Wipe.

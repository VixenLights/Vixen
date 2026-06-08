# State Effect

## Overview

This feature creates a State effect that uses the State property completed by VIX-3591 and specified in
`docs/vix-3591-state-property-final-requirements.md`. It is similar in purpose to the LipSync effect, but it applies named
states to props that are not limited to faces or phonemes.

For example, a waving Santa can define State definitions such as `Arm Up` and `Arm Down`. A decorated Santa can define a
`Full Outfit` State definition with State items for a red coat, white cuffs, and a gold belt buckle. A face can define
`Eyes Open` and `Eyes Closed`. The State effect lets a user activate configured State items inside one selected State
definition statically or drive them from a Mark Collection over time.

## Terminology

- A **State property** is the `State` property container attached to an `IElementNode`. It has stable property identity and
  contains one or more State definitions. It does not expose its own user-visible name or description.
- A **State definition** is one named definition inside a State property. It has a stable ID, name, description, ordered State
  items, and element assignments.
- A **State item** is one row within a State definition. It has a stable ID, name, color, and one or more assigned element
  nodes.
- A **State item group** is the set of State items in the selected State definition whose names exactly match. Activating a
  name activates every row in that group.
- A **render source** determines whether the effect uses one State Item selection or Mark Collection text over time.
- A **playback mode** determines whether selected State item names render together or sequentially.

## Requirements

### State Property Dependency

- The State effect depends on the finalized State property model in `docs/vix-3591-state-property-final-requirements.md`.
- The effect must not redefine State property setup, xModel import, preview, validation, or ID generation behavior except where
  this document explicitly describes effect-specific fallback or rendering behavior.
- State property containers, State definitions, and State items all have non-empty persisted stable IDs.
- The effect must persist the selected State definition ID as durable identity.
- The effect may also persist the containing State property ID and owning element ID as lookup accelerators, but those values
  must not replace the selected State definition ID as the primary durable selection.
- State definition names are display values only. Renaming a selected State definition must not break an existing effect.
- If the referenced State definition is missing or deleted, the effect must not render until the user selects a valid State
  definition.
- No migration behavior is required for State property data because the State property data was development-only before this
  effect.

### Effect Target and State Definition Discovery

- A State effect is placed on one or more target `IElementNode` entries through the existing `TargetNodes` collection.
- The initial user workflow is expected to use one target node, but discovery and rendering must remain correct if multiple
  target nodes are supplied later.
- The effect discovers every State property container attached to a target node or any descendant in its element tree.
- For each discovered State property container, the effect discovers every State definition in fixed creation order.
- State property containers and State definitions outside the targeted trees are excluded.
- The Effect Editor presents the discovered State definitions in a combo box labeled `State`.
- The user selects exactly one State definition for each State effect instance.
- Combo-box entries normally display the State definition name.
- If multiple discovered State definitions have the same name, each duplicate includes its owning element node name in
  parentheses, such as `Eyes Open (Santa - Model)`. `IElementNode` names are unique by contract.
- If duplicate display labels are still possible after adding the owning element name, the label must add the selected State
  definition ID suffix in a short, stable form so users can distinguish entries.
- If the selected State definition no longer exists, the effect renders nothing and the combo box shows the missing selection
  as invalid until the user selects a valid State definition.
- If no State definitions are available, the effect renders nothing and the combo box displays `<No States Available>`.
- When target nodes change, the effect rediscovers available State definitions, validates the previous selected State
  definition ID, retains valid selections, and otherwise applies the missing-selection behavior.
- A newly added State effect auto-selects the first discovered State definition in element-tree traversal order.

### Effect Editor

- The State effect must be editable through the standard Effect Editor.
- Controls appear in this order: `State`, `Render Source`, `State Item` or `Mark Collection`, and `Playback Mode`.
- `State` is a combo box that lists discovered State definitions.
- `Render Source` offers `State Item` and `Mark Collection`.
- `Playback Mode` offers `Default` and `Iterate`.
- `Playback Mode` remains visible for both render sources.
- When `Render Source` is `State Item`, the State Item combo box is visible and the Mark Collection combo box is hidden.
- When `Render Source` is `Mark Collection`, the Mark Collection combo box is visible and the State Item combo box is
  hidden.
- Hidden selections are retained so switching render sources back and forth restores the previous selection.
- When the selected State definition changes, the State Item combo box refreshes. It retains the selected name if that name
  exists in the new State definition; otherwise, it selects the first unique State item name.
- When a named State Item option is selected, the effect persists the stable ID of the first State item in that exact-name
  group as the selection anchor.
- If the selected State item anchor is renamed, the effect retains the selection by ID, displays the new name, and activates
  every State item in the selected State definition with that new exact name.
- If the selected State item anchor is removed, the effect retains the missing selection and renders nothing until the user
  selects a valid State item or `<All>`.
- If the selected State definition has no items, the State Item combo box displays `<No State Items Available>` and the effect
  renders nothing.
- The State Item combo box lists `<All>` first, followed by each unique State item name once in first-State-definition-row
  order.
- A newly added State effect defaults to the `State Item` render source and `Default` playback mode.

### Mark Collection Integration

- The Mark Collection combo box uses the same standard Mark Collection selector pattern used by other effects.
- The Mark Collection combo box lists existing Mark Collections that can provide mark labels containing State item names.
- The State effect does not require adding a new `State` Mark Collection type for this milestone.
- If a `State` Mark Collection type is added, State Mark Collections become the preferred default choices in the Mark
  Collection combo box.
- When the Mark Collection render source is selected and no Mark Collection has previously been selected, the effect
  auto-selects the first available State Mark Collection if one exists.
- If no State Mark Collection exists, the effect uses the standard Mark Collection selector's normal default behavior.
- The user explicitly chooses the `Mark Collection` render source when mark labels should drive the State item names.
- If no Mark Collections exist, the Mark Collection combo box remains empty.
- If a selected Mark Collection is removed, the effect follows the established LipSync and Alternating behavior: it removes
  listeners for the removed collection, clears the selected Mark Collection ID, and renders nothing until the user selects a
  Mark Collection again. It must not silently select a replacement.
- Changes to the selected Mark Collection or its marks must invalidate the effect so the render is refreshed.

### State Item Render Source

- State Item selection operates on unique State item names within the selected State definition.
- Selecting one name activates every State item with that exact name.
- Selecting `<All>` in `Default` mode activates every State item simultaneously for the full effect duration.
- Selecting `<All>` in `Iterate` mode activates each unique State item name sequentially in State definition order. Each unique
  name receives an equal share of the full effect duration, and all rows with that name activate together.
- Selecting one name in either playback mode activates every matching State item for the full effect duration.

### Mark Collection Render Source

- Mark text solely determines the active State items. `<All>` and the State Item selection have no role when a Mark
  Collection is used.
- Mark text may contain one or more comma-delimited State item names from the selected State definition.
- Name matching is case-sensitive.
- Leading and trailing whitespace around each comma-delimited segment is ignored.
- Empty segments and unknown names are preserved as segments but render nothing.
- In `Default` mode, recognized names are compacted to a distinct set and are active simultaneously for the mark duration.
  Empty segments and unknown names are ignored. Each same-named State item renders once.
- In `Iterate` mode, every comma-delimited segment receives an equal share of the mark duration. Recognized names render in
  the listed order. Empty segments and unknown names consume their share of time but render nothing.
- Repeated names remain significant in `Iterate` mode. For example, `Open, Closed, Open` produces an `Open` interval, a
  `Closed` interval, and another `Open` interval.
- Gaps between marks render nothing. The user can close mark gaps in the existing Marks user interface when desired.
- Overlapping marks all render. Their generated intents are combined later by the rendering pipeline.
- Marks that partially overlap the effect timespan are clipped to the effect boundaries.
- Mark text is name-based. Text that refers to a renamed or removed State item is treated as unknown and ignored.

### Rendering

- Rendering affects only the element assignments defined by the selected State definition. Other descendants of the effect
  targets remain untouched.
- An active State item renders its configured color on every assigned element node.
- Rendering always produces intents for leaf element nodes.
- If an assigned node is a group, the effect discovers its current descendant leaf nodes during each render pass.
- Deleted assignments are naturally skipped because only existing nodes can be resolved.
- Changes to group membership affect the next render without requiring the effect data to be edited.
- Multiple State items with the same active name all render their respective nodes and configured colors.
- Multiple active State items may overlap the same leaf nodes. Every applicable intent is added in State item order from first
  to last so the rendering pipeline can mix them later.
- State Item `Default` mode with `<All>` follows the same ordered overlap behavior.
- No intensity level or level curve is exposed by this effect. Users can apply intensity changes through layers outside the
  effect.
- If no State item is active, the effect adds no intents.

### Discrete Colors

- The effect can render on any target tree and does not use element locations.
- For a full-color leaf node, the effect renders the State item's configured color.
- For a discrete-color leaf node, the effect uses that leaf node's supported colors.
- If the configured color is supported, the effect uses it.
- If the configured color is not supported, the effect uses the first supported color.
- If a discrete-color leaf node has no supported colors, the effect skips that leaf node and adds no intent.
- Discrete-color fallback is resolved independently for each leaf node during rendering.

### Render Lifecycle and Intent Coalescing

- Each render pass starts over with an empty intent collection. Previously generated intents are discarded.
- The existing `IsDirty` behavior controls when Vixen requests a fresh render.
- Intent coalescing occurs only within the current render pass.
- To reduce intent count without changing output, adjacent intervals should be merged when the exact same State item renders
  the same leaf node consecutively.
- This optimization applies across adjacent segments and adjacent marks where possible.
- Intervals must not be merged across a gap or across different State items, even when their colors are the same.

### Visual Representation

- The State effect provides a simple visual representation for the sequence editor.
- Follow the existing CustomValue effect style: draw a dark gray bar with white text.
- Display the static text `State`.
- The visual representation does not attempt to preview State definition item sequencing, Mark Collection timing, or intent
  mixing.

### Automated Test Expectations

- Reuse VIX-3591 State property tests as the dependency baseline for property, definition, and item IDs, setup validation,
  xModel import, and assignment behavior.
- Add focused State effect tests for State definition discovery across target trees, duplicate display labels, selected State
  definition ID retention, missing-definition rendering suppression, and the no-State placeholder.
- Add tests for Effect Editor option generation and fallback behavior without automating the visual editor itself.
- Add tests for State Item `Default` and `Iterate` behavior, including `<All>`, duplicate names, overlaps, renamed selected
  anchors, removed selected anchors, and empty State definitions.
- Add tests for mark parsing, whitespace trimming, case sensitivity, unknown names, empty segments, `Default` mode
  de-duplication, `Iterate` mode repeated names, gaps, overlapping marks, and clipping to effect boundaries.
- Add tests for leaf expansion at render time, deleted assignments, changed group membership, overlapping State items, and
  intent ordering.
- Add tests for discrete-color fallback and leaves with no supported colors.
- Add tests for render-pass reset and contiguous-intent coalescing.
- Add tests for Mark Collection selection, removal, invalid selections, background refresh behavior, and dirty-state
  invalidation.

## Deferred Scope

- Additional playback modes such as `Countdown`, `Time Countdown`, and `Number` are deferred until their behavior is defined
  in a future requirement.
- User-selectable overlap strategies such as `First Wins` or `Last Wins` are deferred. The first release adds all applicable
  intents and allows the rendering pipeline to mix them.

## Design Constraints

- Create a new effect under `src\Vixen.Modules\Effect` that extends `BaseEffect`.
- Model Mark Collection discovery, defaulting, and invalidation after the established LipSync and Alternating effect patterns.
- Use the standard Effect Editor annotations required for editable fields.
- The effect is a standard string-like effect and does not use element locations.
- Use the existing rendering pipeline for intent mixing.

## Guidelines

- Use the project skills dotnet-best-practices, csharp-async, csharp-docs, and dotnet-design-pattern-review as part of the design process.
- Use plans.md for creating the plan to design and build the functionality.
- The first implementation milestone must include Updating JIRA issue VIX-3924 in the VIX project covering the work in
  the plan including requirements, high level design, acceptance criteria and testing steps. Provide information to paste into
  into the JIRA ticket in markdown in the plan.
- Call out any risks or concerns in the plan when creating the design.
- Ask questions if you need further clarification.

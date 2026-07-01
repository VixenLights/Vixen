# VIX-2690 Element Tags

## Overview

Over time an element or group of elements can become unused, reserved for a special purpose, or useful to identify as
part of a workflow. For example, a user may want to mark elements as Deprecated, Whole House, Hidden, or with a custom
label that helps them sort and filter the display tree in setup or sequencing views.

Introduce Element Tags as lightweight labels that can be assigned to zero or more `IElementNode` instances. Tags are not
module properties and should not model physical element behavior. They are user-facing metadata for organization,
filtering, sorting, visibility, and workflow state.

## Problem

Vixen currently has no user-friendly way to label elements with simple organizational or workflow metadata. A user who
wants to stop using an element must remember that state manually, rename the element, move it in the tree, or rely on
external notes. This makes it easy to accidentally sequence against deprecated elements or difficult to filter large
displays by meaningful user-defined groups.

## Terminology

- **Element Tag**: A lightweight label that can be assigned to an `IElementNode`.
- **Tag definition**: The profile-level definition of a tag, including its stable identity, display name, built-in key,
  and optional presentation metadata.
- **Tag assignment**: The association between an `IElementNode` and a tag definition.
- **Built-in tag**: A predefined tag supplied by Vixen with stable identity and behavior.
- **User-defined tag**: A tag created and managed by the user.
- **Direct tags**: Tags assigned directly to a specific node.
- **Effective tags**: Tags considered for a node after applying any future inheritance or parent/group rules.

## Requirements

Add an Element Tags feature that allows each `IElementNode` to have zero or more tags assigned.

Element tags should support both built-in tags and user-defined tags.

The initial built-in tags should include:

- `Deprecated`
- `Hidden`
- `Prop`

Built-in tags must have stable identities and stable semantic keys so application code can safely check for behavior such
as "is this node deprecated?" without depending on editable display text.

User-defined tags must have stable identities so they can be renamed without rewriting every node assignment.

Tag assignments should be stored by stable tag identity, not by display name.

Tag definitions should be stored at the system/profile level. Individual element nodes should store only their assigned
tag references.

Existing profiles without tag data should load successfully. On load, Vixen should seed any missing built-in tag
definitions and leave all existing nodes with no assigned tags.

Tags should reusable on the future Prop Centric path so Props can be decorated and used in the same manner. Eventually they
may no longer be needed at the IElementNode level.

## Tag Definition Model

A tag definition should include enough data to support built-in behavior, user editing, display, and stable persistence.

Suggested fields:

- `Id`: Stable `Guid` used for persistence and node assignments.
- `Key`: Stable semantic identifier for built-in tags, such as `deprecated` or `whole-house`.
- `Name`: User-facing display name.
- `IsBuiltIn`: Indicates whether the tag is supplied by Vixen.
- `Description`: Optional explanatory text.
- `DisplayColor`: Optional UI color used when showing the tag.
- `SortOrder`: Optional ordering value for tag pickers and filter lists.

The exact implementation can vary, but it must preserve the distinction between stable identity, stable built-in
semantics, and editable display text.

## IElementNode API Direction

Add a new public API to `IElementNode` for accessing assigned tags. The API should represent a node's tag assignments,
not the full profile-level tag catalog.

Potential shape:

```csharp
ElementTagCollection Tags { get; }
```

The collection should allow callers to inspect, add, and remove tag assignments for a node while preventing duplicate
assignments.

Tag catalog management, validation, built-in lookup, and cross-node cleanup should live in a dedicated service rather
than directly on `IElementNode`.

Potential service responsibilities:

- List all tag definitions.
- Create user-defined tags.
- Rename user-defined tags.
- Delete user-defined tags.
- Resolve built-in tags by key.
- Determine whether a node has a specific tag.
- Remove deleted tag assignments from all nodes.

## Built-In Tag Behavior

`Deprecated` marks an element or group as no longer intended for new sequencing work.

Deprecated nodes should be visually distinguishable in relevant UIs and should be optionally hidden or filtered out in
the sequencer.

`Prop` marks an element or group as representing a Prop target. This could be used for Prop migration in the future.

`Hidden` marks an element or group as hidden so it could be filterd out in the sequencer, but is not deprecated.

Built-in tags should not be deletable or editable. Behavior must continue to rely on stable built-in keys, not display names.

## User-Defined Tag Behavior

Users should be able to create custom tags from the Display Setup and/or Sequencer workflows.

User-defined tag names must be trimmed.

Blank tag names are not valid.

Tag names must be unique case-insensitively within a profile.

Users should be able to rename user-defined tags. Renaming a tag must preserve all existing node assignments.

Users should be able to delete user-defined tags. Deleting a tag should require confirmation and must remove that tag
assignment from all nodes.

## Assignment Behavior

Users should be able to add or remove tags associated with an `IElementNode` in Display Setup.

Users should be able to add or remove tags associated with an `IElementNode` in the Sequencer if the relevant element
management UI is available there.

The same tag should not be assignable more than once to the same node.

Tag assignment should initially be direct-only. A tag assigned to a group should not automatically assign the tag to all
children unless a specific workflow explicitly does so.

Filtering and UI behavior should define whether it uses direct tags only or effective tags. If inheritance or effective
tag behavior is added, it should be explicit and visible to the user.

## Filtering, Sorting, And Display

Display Setup and Sequencer views should be able to filter element nodes by tags.

Filtering should support at least:

- Include nodes with a selected tag.
- Exclude nodes with a selected tag.
- Show nodes without any tags.

Sequencer views should provide a way to hide or show deprecated nodes.

Element lists or trees should be able to display assigned tags in a compact way, such as a small label, icon, color chip,
or tooltip.

Sorting and grouping by tag should be supported where element lists already provide sorting or grouping behavior.

## Persistence

Tag definitions should be persisted once per profile/system configuration.

Node tag assignments should be persisted as part of element node data.

Persistence must be backward-compatible with existing profile data.

Loading a node assignment that references a missing tag definition should not prevent profile loading. Missing references
should either be ignored or surfaced for cleanup.

## Risks And Open Questions (Resolved Decisions)

### Tag definition catalog location

**Decision:** Add tag definitions as a new collection on `SystemConfig` (e.g. `IEnumerable<ElementTagDefinition> Tags`),
following the same pattern already used for `Filters`, `OutputControllers`, `Previews`, and `DataFlow`. Add a matching
`XmlElementTagDefinitionCollectionSerializer` wired into `XmlSystemConfigFilePolicy` (`ReadTags`/`WriteTags`), so the
catalog persists once per profile in `SystemConfig.xml` alongside the other system-level collections.

Catalog CRUD, built-in lookup, and cross-node cleanup should live in a new singleton service, `ElementTagService`
(`Vixen.Services`), mirroring the existing `ElementNodeService` / `OutputFilterService` pattern (private constructor,
static `Instance`). This keeps catalog logic off `IElementNode`, consistent with the "Tag catalog management ... should
live in a dedicated service" requirement above.

### `ProxyElementNode` tag collection

**Decision:** `ProxyElementNode` returns an empty, non-persistent tag collection, consistent with how it already returns
empty `Children`/`Parents` and uses a non-persistent `PropertyManager`. `ProxyElementNode` exists only to preserve
effect-to-node references for sequences whose target node is missing or not yet loaded (created in
`EffectNodeSurrogate.CreateEffectNode()`); it does not participate in the node tree and is never serialized as part of
`SystemConfig`, so there is nothing for a tag assignment to attach to. Carrying tag references across the "node is
missing" gap for later reconciliation is out of scope for v1 — if a real need for it emerges (e.g. Prop migration
tooling), it should be added as an explicit, visible follow-on rather than implied by this change.

### Deprecated nodes and rendering/export behavior

**Decision (confirmed):** `Deprecated` (and tags generally) must **not** affect rendering or output behavior. `Element` already has
a `Masked` flag that is the one existing mechanism controlling whether an element contributes to rendering/output, and
it is a distinct, lower-level concept from tags. Tags stay UI-only: visual distinction, optional hide/filter in Setup
and Sequencer views. This is required by the doc's own framing — "Tags are not module properties and should not model
physical element behavior." Any future workflow that wants `Deprecated` to imply `Masked` must do so explicitly (e.g.
a user-triggered "apply deprecated state to output" action), never as an implicit side effect of assigning the tag.

Implication to call out explicitly: "hide/filter deprecated nodes in the sequencer" only hides them from the view — it
does **not** stop already-sequenced effects on a deprecated node from rendering during playback or export. If that
surprises users in practice, that's a separate, later decision about coupling `Deprecated` to `Masked`, not something
this change should do implicitly.

### Group-level tags and effective tags

**Decision (confirmed):** V1 ships direct-only tag *assignment* — assigning a tag to a group does not implicitly assign
that tag to its children, and no effective-tag computation (e.g. "does this node inherit Deprecated from an ancestor")
is implemented now. `GetTags`/`HasTag` operate on direct assignments only; a distinct name (e.g. `GetEffectiveTags`) is
reserved for if/when true tag inheritance is added later, per the "explicit and visible to the user" requirement.

This is separate from *filtering/hiding cascading structurally through the tree*, which still applies. If a user hides
a `Deprecated` group node in a tree/list view, its children are hidden along with it — not because they inherited the
`Deprecated` tag, but because the tree view cannot show orphaned children under a hidden parent without breaking the
tree. That is ordinary tree-rendering behavior (the same as collapsing/hiding any branch node) and applies regardless
of tags. Filtering implementations should be built against direct tag assignments plus normal tree containment, not
against a computed "effective tags" set.

### Tags vs. existing element Properties

**Decision:** Keep Tags and Properties structurally separate; do not route tags through the Properties system.
Properties are pluggable module instances (`IPropertyModuleInstance`) managed by `PropertyManager`, backed by module
descriptors, and persisted as instance data in `VixenSystem.ModuleStore` (keyed by `(TypeId, InstanceId)`) — they exist
to model physical/module-level element configuration (e.g. `ColorProperty`). Tags are simple catalog-referenced
metadata: a node's `Tags` collection stores only stable `Guid` references into the `ElementTagDefinition` catalog, with
no module machinery, no `ModuleStore` involvement, and no per-instance data model. `ElementTagCollection` and
`IElementNode.Tags` should carry XML doc remarks calling out this distinction explicitly so future contributors don't
reach for the Property module pattern when extending tags.

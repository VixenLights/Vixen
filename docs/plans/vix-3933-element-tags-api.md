# Element Tags — Core API, Catalog Service, and Persistence (VIX-3933)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

This plan implements the *API and persistence layer* for Element Tags as specified in `docs/vix-3933-element-tags.md`. It does **not** implement Display Setup or Sequencer UI, filtering, sorting, or visual display of tags — those are explicit non-goals here and belong in a follow-on ticket/plan once this API exists. Anyone picking this plan up needs no other document to succeed; every fact from `docs/vix-3933-element-tags.md` that is relevant to the API is repeated here.

## Purpose / Big Picture

Today `IElementNode` (the interface every element and group in a Vixen display tree implements — see `src/Vixen.Core/Sys/IElementNode.cs`) has no way to carry lightweight organizational metadata. There is no way for code (and, eventually, UI) to ask "does this node have the `Deprecated` tag?" or "list every user-defined tag in this profile." This plan adds that capability at the API and file-persistence level: a tag catalog stored once per profile, a `Tags` collection on every node that stores only stable tag-ID references, and a service that manages the catalog (create/rename/delete user tags, resolve built-ins, clean up orphaned assignments).

After this plan is complete, a developer (not yet an end user — there is no UI change) can observe the feature working in two ways:

1. Automated tests: running `dotnet test src/Vixen.Tests/Vixen.Tests.csproj` shows new passing tests that create tag definitions, assign/remove them on nodes, prevent duplicate assignment, rename/delete user tags while preserving or cleaning up node assignments, and round-trip tag data through XML exactly like the existing `Filters` collection does.
2. Manual verification against the real application: building and running Vixen (`msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` then launching `Vixen.Application`), opening any existing profile (including one saved before this change), and observing in the NLog log output (see "Validation and Acceptance" below) that the three built-in tags (`Deprecated`, `Hidden`, `Prop`) were seeded and that the profile loaded without error.

A later plan will wire `ElementTagService` and `IElementNode.Tags` into Display Setup/Sequencer UI, filtering, and visual display — this plan's job is to make that wiring possible by having a correct, tested, persisted API underneath it.

## Progress

- [x] (2026-07-01) Milestone 1: Create JIRA issue for this work and link it to VIX-2690. Done — [VIX-3933](https://vixenlights.atlassian.net/browse/VIX-3933), issue type "New Feature", linked to [VIX-2690](https://vixenlights.atlassian.net/browse/VIX-2690) with a "Relates" link, assigned to Jeff Uchitjil, priority Normal.
- [x] (2026-07-01) Milestone 2: Create branch `VIX-3933` off `master`. Done — branch created locally. Also discovered the design doc had been renamed/staged as `docs/vix-3933-element-tags.md` (was `docs/vix-2690-element-tags.md`) on `master` before branching; updated this plan's references to the new path accordingly.
- [x] (2026-07-01) Milestone 3: Core data model — `ElementTagDefinition`, `ElementTagCollection`, built-in tag constants, and the `Tags` member on `IElementNode` / `ElementNode` / `ProxyElementNode`. Done — 18 new unit tests pass (`ElementTagCollectionTests`, `BuiltInElementTagsTests`, `ElementTagDefinitionTests`), full solution builds clean with no new warnings.
- [ ] Milestone 4: Catalog service — `ElementTagService` with CRUD, built-in resolution, validation, and cross-node cleanup.
- [ ] Milestone 5: Persistence — `SystemConfig.Tags`, XML serializers, `SystemConfigFilePolicy` wiring, per-node tag persistence in `XmlElementNodeSerializer`, and `VixenSystem` load/save/seed/cleanup wiring.
- [ ] Milestone 6: Apply project skills (`csharp-docs`, `dotnet-best-practices`, `csharp-async`, `dotnet-design-pattern-review`) and address findings.
- [ ] Milestone 7: Manual end-to-end verification against the running application and JIRA issue closeout.

## Surprises & Discoveries

*(Fill in as work progresses.)*

- The repository now has a real test project, `src/Vixen.Tests` (xunit v3 + Moq), even though `CLAUDE.md` still says "There are no automated tests in this repository." That statement is stale; use `Vixen.Tests` for all new unit tests in this plan. Test files there follow a documented AAA convention with a naming scheme `MethodName_Condition_ExpectedBehavior` — see the header comment in `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs` for the canonical example.
- No existing test in `src/Vixen.Tests` touches the static `Vixen.Sys.VixenSystem` class directly (confirmed by grep — zero matches). `VixenSystem` is a static class that bootstraps file I/O, module loading, and global managers on `LoadSystemConfig()`; it is not designed to be unit-tested in isolation, and neither is `ElementNodeService` (the closest existing precedent for the new `ElementTagService`), which also has no tests. This plan follows that existing boundary: model classes and XML serializers get unit tests; `VixenSystem` wiring (seeding, save/load integration) is verified manually against the running app in Milestone 7, not with automated tests. Do not attempt to instantiate or mock `VixenSystem` in tests — follow the codebase's established pattern instead.

## Decision Log

- Decision: Tag definitions are stored as `SystemConfig.Tags` (`IEnumerable<ElementTagDefinition>`), following the exact same pattern as `SystemConfig.Filters` (see `src/Vixen.Core/Sys/SystemConfig.cs` lines 102-112). Runtime access goes through a new `internal static List<ElementTagDefinition> VixenSystem.TagDefinitions`, mirroring how `SystemConfig.Filters` is loaded into the runtime `VixenSystem.Filters` manager on `LoadSystemConfig()`.
  Rationale: This is a direct, low-risk application of an existing, working pattern in the same file, for the same kind of "catalog stored once per profile" data. No new architectural concept is introduced.
  Date/Author: 2026-07-01, planning session.

- Decision: `ElementTagService` (public, `Vixen.Services` namespace, `src/Vixen.Core/Services/ElementTagService.cs`) is a private-constructor singleton exposed via `public static ElementTagService Instance`, exactly like `ElementNodeService` (`src/Vixen.Core/Services/ElementNodeService.cs`).
  Rationale: Matches the existing service pattern used throughout `Vixen.Core/Services`. Keeps catalog CRUD off `IElementNode`, per the design doc's explicit requirement that "Tag catalog management ... should live in a dedicated service rather than directly on `IElementNode`."
  Date/Author: 2026-07-01, planning session.

- Decision: `ProxyElementNode.Tags` returns a single shared, static, "inert" `ElementTagCollection` instance whose `Add`/`Remove` methods are no-ops (they do not throw, and `Add` always returns `false`).
  Rationale: `docs/vix-3933-element-tags.md`'s resolved decision states `ProxyElementNode` should expose an empty, non-persistent tag collection because it does not participate in the node tree and is never serialized. Making mutation a silent no-op (rather than throwing) avoids surprising crashes in any future code that treats all `IElementNode` instances uniformly (e.g. a generic "add this tag to every selected node" UI command running over a mixed selection that happens to include a proxy).
  Date/Author: 2026-07-01, planning session.

- Decision: Per-node tag assignment persistence is added inline inside `XmlElementNodeSerializer` (a `<Tags>` element containing `<Tag id="..."/>` children written/read alongside the existing `<Properties>` element), not as a separate top-level serializer class.
  Rationale: The existing serializer already handles a similarly simple case inline — the leaf `channelId` attribute — without a dedicated class. A full collection-serializer class (`XmlElementTagAssignmentCollectionSerializer`) would be pure boilerplate for a list of GUIDs with no per-item complexity. This keeps the change minimal and consistent with the file's existing style.
  Date/Author: 2026-07-01, planning session.

- Decision: Orphaned-tag-reference cleanup (a node references a tag ID that no longer exists in the catalog) happens once, in `VixenSystem.LoadSystemConfig()`, immediately after both `Nodes` and `TagDefinitions` have been populated — not inside the XML serializer.
  Rationale: The XML serializer for a single node has no visibility into the full tag catalog (it is deserializing one node's `<Node>` element in isolation) and no visibility into whether the catalog has finished loading yet. `VixenSystem.LoadSystemConfig()` already contains a directly analogous pattern, `CleanUpOrphanedPreviewModuleData()` (`src/Vixen.Core/Sys/VixenSystem.cs`, called at the end of `LoadSystemConfig()`), which removes stale preview module data after all relevant collections are loaded. This plan adds a new `CleanUpOrphanedElementTagAssignments()` following the same shape and calls it from the same place.
  Date/Author: 2026-07-01, planning session.

- Decision: Built-in tag `Id` values are fixed, hard-coded GUIDs (not generated at seed time), so that a `Deprecated` tag created on one user's machine has the same `Id` as on every other machine and across every reseed.
  Rationale: The design doc requires "stable identities" for built-in tags so application code can check behavior (e.g. "is this deprecated?") without depending on editable text. If the seed logic generated a new `Guid` each time it ran, re-seeding (e.g. after a partially-corrupted `SystemConfig.xml`) would silently orphan every existing node's `Deprecated` assignment. The fixed IDs chosen for this plan are: `Deprecated` = `753115da-e27a-4261-a136-8222ccc3f22e`, `Hidden` = `2f21bfcb-07a3-4cac-bdc5-4b6e37773c04`, `Prop` = `6db1852d-71d8-449d-afa5-193a6def26a0`. These exact values must be used verbatim in `src/Vixen.Core/Sys/BuiltInElementTags.cs` (see Milestone 3) and never regenerated.
  Date/Author: 2026-07-01, planning session.

- Decision: `ElementTagCollection.Empty`'s inertness is implemented with a private `_isInert` flag checked at the top of `Add`/`Remove`, not the subclass sketch in the Plan of Work's Milestone 3 pseudocode.
  Rationale: The plan explicitly left the inheritance mechanics as an implementer's choice. A private bool flag on a single sealed class is simpler than introducing a nested subclass, keeps `ElementTagCollection` sealed (no `protected virtual` surface to reason about), and produces the identical required behavior (real collections behave normally; `Empty` never accumulates state no matter how many times `Add` is called).
  Date/Author: 2026-07-01, Milestone 3 implementation.

## Outcomes & Retrospective

*(Fill in at completion.)*

## Context and Orientation

Vixen is a .NET 10 WPF desktop app for sequencing animated light shows (see root `CLAUDE.md`). This plan touches only `src/Vixen.Core` (the framework/module/data-model assembly) and `src/Vixen.Tests` (the xunit test project). No WPF/UI project is touched.

**The node tree.** Every element or group of elements in a Vixen display is represented by an `ElementNode` (`src/Vixen.Core/Sys/ElementNode.cs`), which implements the public interface `IElementNode` (`src/Vixen.Core/Sys/IElementNode.cs`). `IElementNode` currently exposes: `Element`, `Id` (a `Guid`), `Name`, `Children`, `Parents`, `IsLeaf`, several enumerator methods, `Properties` (a `PropertyManager`), and `IsProxy` (a `bool`). `ElementNode` instances are never constructed directly by outside code — their constructors are `internal` — they are created through `Vixen.Services.ElementNodeService` or `VixenSystem.Nodes.AddNode(...)` (`Vixen.Sys.NodeManager`, `src/Vixen.Core/Sys/Managers/NodeManager.cs`). `NodeManager.GetAllNodes()` returns every node in the tree (not just leaves) — this plan uses it for cross-node tag cleanup.

**`ProxyElementNode`** (`src/Vixen.Core/Sys/ProxyElementNode.cs`) is a second, much simpler `IElementNode` implementation. It exists only to stand in for a node that a sequence's effect data references but that no longer exists in (or has not yet been loaded into) the current project — created in `EffectNodeSurrogate.CreateEffectNode()`. It has empty `Children`/`Parents`, `IsLeaf` is always `true`, `IsProxy` is always `true`, and it is never part of `SystemConfig` and never serialized. Do not confuse this with `ElementNodeProxy` (`src/Vixen.Core/Sys/ElementNodeProxy.cs`), a completely different, unrelated DTO class used only for exporting/importing node *templates* (`NodeManager.ExportElementNodeProxy` / `ImportElementNodeProxy`). `ElementNodeProxy` does not implement `IElementNode` and is out of scope for this plan — it is not touched.

**Per-node metadata today: `PropertyManager`.** Every `ElementNode` has a `Properties` member of type `PropertyManager` (`src/Vixen.Core/Sys/PropertyManager.cs`). This is the *existing* mechanism for attaching metadata to a node, and it is intentionally heavyweight: properties are pluggable modules (`IPropertyModuleInstance`, e.g. `ColorProperty` under `src/Vixen.Modules/Property/Color/`) with their own per-instance data stored in `VixenSystem.ModuleStore.InstanceData` (a `Dictionary`-backed store keyed by `(TypeId, InstanceId)` tuples). Tags must **not** be built this way — the design doc is explicit that "Tags are not module properties and should not model physical element behavior." `ElementTagCollection` (introduced by this plan) will store nothing but a set of stable `Guid` tag-ID references directly on the node, with no module machinery and no `ModuleStore` involvement whatsoever.

**System-level catalog storage: `SystemConfig`.** `src/Vixen.Core/Sys/SystemConfig.cs` defines an `internal class SystemConfig` that holds every collection that is persisted once per profile: `Elements`, `Nodes`, `OutputControllers`, `Previews`, `Filters` (`IEnumerable<IOutputFilterModuleInstance>`), `DataFlow`. Each of these follows the same shape: a private backing field, a public property whose getter lazily defaults to an empty array, and it is persisted to `SystemData/SystemConfig.xml` (constants `SystemConfig.Directory` / `SystemConfig.FileName`). Because tag definitions belong once per profile, not once per node, they belong here as `SystemConfig.Tags`, following the exact shape of `SystemConfig.Filters`.

**How `SystemConfig` is read and written.** `src/Vixen.Core/IO/Policy/SystemConfigFilePolicy.cs` is an internal abstract class with a `Write()` method that calls, in order: `WriteContextFlag`, `WriteIdentity`, `WriteFilterEvaluationAllowance`, `WriteDefaultUpdateInterval`, `WriteClearEffectCacheOnExit`, `WriteVideoEffectOptions`, `WriteElements`, `WriteNodes`, `WriteControllers`, `WritePreviews`, `WriteFilters`, `WriteDataFlowPatching`, `WriteDisabledDevices` — each an `abstract` method the subclass must implement. `Read()` mirrors this with a matching sequence of `Read*` methods. The single concrete subclass is `src/Vixen.Core/IO/Xml/SystemConfig/XmlSystemConfigFilePolicy.cs`, which implements each of these by constructing a small dedicated XML serializer and calling `WriteObject`/`ReadObject` on it — for example `WriteFilters()` builds an `XmlOutputFilterCollectionSerializer` (`src/Vixen.Core/IO/Xml/Serializer/XmlOutputFilterCollectionSerializer.cs`) and calls `serializer.WriteObject(_systemConfig.Filters)`. `XmlOutputFilterCollectionSerializer` itself is a thin wrapper: it writes/reads a `<Filters>` element containing per-item `<Filter>` elements produced by a per-item serializer, `XmlOutputFilterSerializer`. This exact two-class pattern (collection serializer wrapping an item serializer) is what this plan reproduces for tag definitions.

**How individual nodes are serialized.** `src/Vixen.Core/IO/Xml/Serializer/XmlElementNodeSerializer.cs` writes/reads a single `<Node>` XML element with `name`/`id` attributes, either a `channelId` attribute (for a leaf node referencing an `Element`) or nested child `<Node>` elements (for a branch), and a `<Properties>` element built by `XmlPropertyCollectionSerializer`. `WriteObject`/`ReadObject` on this class are called recursively by `XmlElementNodeCollectionSerializer` for the whole tree.

**How the runtime loads and saves all of this.** `VixenSystem` (`src/Vixen.Core/Sys/VixenSystem.cs`) is a big static class. Its `LoadSystemConfig()` method (starting at line 229) creates fresh runtime managers (`Elements = new ElementManager()`, `Nodes = new NodeManager()`, `Filters = new FilterManager(DataFlow)`, etc.), then loads `ModuleStore` from disk, then loads `SystemConfig` from disk (`SystemConfig = _LoadSystemConfig(systemDataPath) ?? new SystemConfig();`), then copies each `SystemConfig` collection into the matching runtime manager: `Elements.AddElements(SystemConfig.Elements); Nodes.AddNodes(SystemConfig.Nodes); ... Filters.AddRange(SystemConfig.Filters);`. Immediately after, it calls `CleanUpOrphanedPreviewModuleData()`, a private method that removes stale `ModuleStore` entries for previews that no longer exist — this is the direct precedent this plan follows for cleaning up node tag assignments that reference a deleted tag definition. Saving happens in `SaveSystemConfigAsync()` (line 161), which copies the current runtime state back onto `SystemConfig` (`SystemConfig.OutputControllers = OutputControllers; ... SystemConfig.Filters = Filters; ...`) before calling `SystemConfig.Save()`.

**Testing.** `src/Vixen.Tests/Vixen.Tests.csproj` is an xunit v3 + Moq test project referencing `Vixen.Core`. Run tests from the repository root with `dotnet test src/Vixen.Tests/Vixen.Tests.csproj`. Follow the AAA convention and `MethodName_Condition_ExpectedBehavior` naming documented at the top of `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs`.

## Plan of Work

### Milestone 2 — Branch

Before any code changes, create branch `VIX-3933` off the current `master` tip (`git checkout master && git pull && git checkout -b VIX-3933`). All commits for this plan go on this branch. Prefix commit messages with `VIX-3933` per the project's existing convention (see recent commits like `VIX-3911 Refactor DataSource flow to reduce allocations`).

### Milestone 3 — Core data model

Create `src/Vixen.Core/Sys/ElementTagDefinition.cs`, a plain public class (not a module, not `[Serializable]` needed since XML serialization is hand-written via the serializer classes in Milestone 5, matching how `IOutputFilterModuleInstance` itself needs no `[Serializable]` attribute):

    public sealed class ElementTagDefinition
    {
        public Guid Id { get; }
        public string Key { get; }        // stable semantic key, e.g. "deprecated"; empty string for user-defined tags
        public string Name { get; set; }  // user-facing display name; trimmed, non-blank, case-insensitively unique within a profile
        public bool IsBuiltIn { get; }
        public string? Description { get; set; }
        public string? DisplayColor { get; set; }   // nullable; a hex string such as "#FFA500", validated/interpreted by UI later
        public int SortOrder { get; set; }

        public ElementTagDefinition(Guid id, string key, string name, bool isBuiltIn)
        { /* assign Id, Key (default ""), Name, IsBuiltIn; SortOrder defaults to 0 */ }
    }

`Key` and `IsBuiltIn` are get-only (set only through the constructor) because they define stable identity/semantics that must never change after creation, per the design doc: "Built-in tags must have stable identities and stable semantic keys." `Name`, `Description`, `DisplayColor`, `SortOrder` are mutable because users edit them (rename, re-order, re-describe, re-color).

Create `src/Vixen.Core/Sys/BuiltInElementTags.cs`:

    public static class BuiltInElementTags
    {
        public const string DeprecatedKey = "deprecated";
        public const string HiddenKey = "hidden";
        public const string PropKey = "prop";

        public static readonly Guid DeprecatedId = new Guid("753115da-e27a-4261-a136-8222ccc3f22e");
        public static readonly Guid HiddenId = new Guid("2f21bfcb-07a3-4cac-bdc5-4b6e37773c04");
        public static readonly Guid PropId = new Guid("6db1852d-71d8-449d-afa5-193a6def26a0");

        public static IReadOnlyList<ElementTagDefinition> CreateDefaults()
        {
            return new[]
            {
                new ElementTagDefinition(DeprecatedId, DeprecatedKey, "Deprecated", isBuiltIn: true) { SortOrder = 0 },
                new ElementTagDefinition(HiddenId, HiddenKey, "Hidden", isBuiltIn: true) { SortOrder = 1 },
                new ElementTagDefinition(PropId, PropKey, "Prop", isBuiltIn: true) { SortOrder = 2 },
            };
        }
    }

Use these exact GUID literals — they are recorded in the Decision Log above and must not be regenerated.

Create `src/Vixen.Core/Sys/ElementTagCollection.cs`. This is the per-node collection exposed as `IElementNode.Tags`:

    public sealed class ElementTagCollection : IEnumerable<Guid>
    {
        private readonly HashSet<Guid> _tagIds = new();

        public bool Add(Guid tagId)     // returns false (no-op) if already present — prevents duplicate assignment
        public bool Remove(Guid tagId)  // returns false if not present
        public bool Contains(Guid tagId)
        public int Count { get; }
        public IEnumerator<Guid> GetEnumerator()
        IEnumerator IEnumerable.GetEnumerator()

        internal static readonly ElementTagCollection Empty = new InertElementTagCollection();

        private sealed class InertElementTagCollection : ElementTagCollection
        {
            // Overrides Add/Remove to always return false and never mutate _tagIds.
            // (If C# access rules make overriding awkward for a sealed base, make ElementTagCollection
            // non-sealed with a `protected virtual` Add/Remove pair, or give InertElementTagCollection
            // its own private no-op fields entirely instead of inheriting — either is acceptable; pick
            // whichever keeps the public API identical for both real and proxy nodes.)
        }
    }

The exact inheritance mechanics are an implementation detail left to the implementer; the required *behavior* is not: a real node's `ElementTagCollection` must support add/remove/contains against a real backing set, duplicate `Add` must be a no-op returning `false`, and the single shared instance used by every `ProxyElementNode` must never actually store anything no matter how many times `Add` is called on it.

Modify `src/Vixen.Core/Sys/IElementNode.cs`: add `ElementTagCollection Tags { get; }` to the interface, next to the existing `PropertyManager Properties { get; }` member. Update its XML doc summary comment (the interface currently has none — add one per the `csharp-docs` skill, see Milestone 6) explaining that `Tags` holds this node's direct tag assignments (stable tag-ID references only), not the profile-wide tag catalog.

Modify `src/Vixen.Core/Sys/ElementNode.cs`: add `public ElementTagCollection Tags { get; } = new ElementTagCollection();` initialized the same way `Properties` is initialized in the constructor (line 35: `Properties = new PropertyManager(this);`) — add `Tags = new ElementTagCollection();` on the following line inside the constructor at line ~20-36, rather than as a field initializer, to keep the initialization visibly grouped with `Properties` for a future reader.

Modify `src/Vixen.Core/Sys/ProxyElementNode.cs`: add `public ElementTagCollection Tags => ElementTagCollection.Empty;` inside the `#region Implementation of IElementNode` block, next to `IsProxy`.

Add unit tests in a new file `src/Vixen.Tests/Sys/ElementTagCollectionTests.cs` covering: adding a new tag ID returns `true` and `Contains` becomes `true`; adding the same ID twice returns `false` on the second call and `Count` stays `1`; removing a present ID returns `true` and `Contains` becomes `false`; removing an absent ID returns `false`; `ElementTagCollection.Empty.Add(...)` returns `false` and `Empty.Contains(...)` stays `false` no matter what ID is passed, called from multiple "logical proxies" (i.e. call `Add` on the same `Empty` instance twice in one test to prove it never accumulates state — this is the behavior a `ProxyElementNode` depends on).

This milestone's acceptance: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter ElementTagCollectionTests` shows all new tests passing, and `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` (a Build, not full Rebuild, is enough here) succeeds with no new warnings introduced by these files.

### Milestone 4 — Catalog service

Create `src/Vixen.Core/Services/ElementTagService.cs`, `namespace Vixen.Services`, matching the shape of `ElementNodeService.cs` (private constructor, `public static ElementTagService Instance`). Responsibilities, each a public method:

- `IReadOnlyList<ElementTagDefinition> GetAll()` — returns `VixenSystem.TagDefinitions` ordered by `SortOrder` then `Name`.
- `ElementTagDefinition? GetByKey(string key)` — built-in lookup; returns the definition whose `Key` matches (ordinal, case-sensitive — keys are internal constants, not user text).
- `ElementTagDefinition? GetById(Guid id)`.
- `ElementTagDefinition CreateUserTag(string name)` — trims `name`; throws `ArgumentException` if the trimmed result is empty, or if any existing tag's `Name` matches case-insensitively (`string.Equals(existing.Name, trimmed, StringComparison.OrdinalIgnoreCase)`); otherwise creates a new `ElementTagDefinition(Guid.NewGuid(), key: "", trimmedName, isBuiltIn: false)`, appends it to `VixenSystem.TagDefinitions`, and returns it.
- `void RenameUserTag(Guid id, string newName)` — looks up the tag by `id`; throws `InvalidOperationException` if not found or if `IsBuiltIn` is `true` ("Built-in tags cannot be renamed."); otherwise applies the same trim/blank/duplicate-name validation as `CreateUserTag` (excluding the tag being renamed from the duplicate check) and sets `Name`. Because node assignments are stored by `Id`, not by name, no node data needs to change — this is what "renaming a tag must preserve all existing node assignments" means concretely: nothing on any `ElementNode.Tags` collection is touched by this method.
- `void DeleteUserTag(Guid id)` — looks up the tag; throws `InvalidOperationException` if not found or `IsBuiltIn` is `true` ("Built-in tags cannot be deleted."); otherwise removes it from `VixenSystem.TagDefinitions` and then iterates `VixenSystem.Nodes.GetAllNodes()` calling `node.Tags.Remove(id)` on every node (harmless no-op on nodes that don't have it). This method does not show or require a confirmation dialog itself — the design doc's "deleting a tag should require confirmation" is a UI-layer concern for the future Display Setup/Sequencer milestone, not this service; document this boundary in the method's XML doc `<remarks>`.
- `bool HasTag(IElementNode node, Guid tagId)` — `node.Tags.Contains(tagId)`. Provided as a convenience/readability wrapper; also works uniformly for `ProxyElementNode` since its `Tags` is always empty.
- `bool HasTag(IElementNode node, string key)` — resolves `key` via `GetByKey` first (returns `false` if the key doesn't exist), then delegates to the `Guid` overload. This is the shape application code should use for the "is this deprecated?" check called out in the design doc, e.g. `ElementTagService.Instance.HasTag(node, BuiltInElementTags.DeprecatedKey)`.

Add unit tests in `src/Vixen.Tests/Services/ElementTagServiceTests.cs`. Because `ElementTagService` reads/writes the static `VixenSystem.TagDefinitions` and `VixenSystem.Nodes` (see Surprises & Discoveries above — this is the same untestable-in-isolation boundary `ElementNodeService` already has), do **not** attempt to unit test `ElementTagService` end-to-end against live `VixenSystem` state. Instead, extract the pure validation/lookup logic that does not depend on `VixenSystem` into small internal static helper methods on `ElementTagService` (e.g. `internal static void ValidateNewTagName(string candidateName, IEnumerable<ElementTagDefinition> existing)` that throws or returns normally, called by both `CreateUserTag` and `RenameUserTag`), and unit test *those* helpers directly by passing in a plain in-memory `List<ElementTagDefinition>` — no `VixenSystem` involved. Cover: blank/whitespace-only name rejected; leading/trailing whitespace trimmed before comparison and before storage; case-insensitive duplicate rejected (`"Deprecated"` vs `"deprecated"`); distinct names accepted; renaming a tag to its own current name (case-identical) does not throw. Mark the parts of this milestone that require live `VixenSystem` state (full `CreateUserTag`/`RenameUserTag`/`DeleteUserTag` behavior, cross-node cleanup) as verified manually in Milestone 7 instead.

### Milestone 5 — Persistence

**5a. `SystemConfig.Tags`.** In `src/Vixen.Core/Sys/SystemConfig.cs`, add a private field `private IEnumerable<ElementTagDefinition> _tags;` and a public property `Tags` with the identical lazy-default-to-empty-array shape as `Filters` (lines 102-112):

    public IEnumerable<ElementTagDefinition> Tags
    {
        get
        {
            if (_tags == null) {
                _tags = new ElementTagDefinition[0];
            }
            return _tags;
        }
        set { _tags = value; }
    }

**5b. XML serializers.** Create `src/Vixen.Core/IO/Xml/Serializer/XmlElementTagDefinitionSerializer.cs`, modeled directly on `XmlOutputFilterSerializer.cs` (read that file first to copy its exact style/error-handling before writing this one). It writes/reads a single `<Tag>` element with attributes `id`, `key`, `name`, `isBuiltIn`, `sortOrder`, and an optional `<Description>` child element (present only if non-null/non-empty) and an optional `description` — wait, keep it simple: use attributes for all scalar fields (`id`, `key`, `name`, `isBuiltIn`, `sortOrder`, and optional `description`, `displayColor` attributes only written when non-null). `ReadObject` must tolerate a missing `key` attribute (treat as empty string) and a missing `sortOrder` attribute (treat as `0`), since forward-compatibility means a future field must never break loading of a file written by this version.

Create `src/Vixen.Core/IO/Xml/Serializer/XmlElementTagDefinitionCollectionSerializer.cs`, modeled directly on `XmlOutputFilterCollectionSerializer.cs`: writes/reads a `<Tags>` element wrapping per-item `<Tag>` elements via `XmlElementTagDefinitionSerializer`.

**5c. `SystemConfigFilePolicy` wiring.** In `src/Vixen.Core/IO/Policy/SystemConfigFilePolicy.cs`, add `protected abstract void WriteTagDefinitions();` and `protected abstract void ReadTagDefinitions();`, and call them in `Write()`/`Read()` immediately after `WriteFilters()`/`ReadFilters()` and before `WriteDataFlowPatching()`/`ReadDataFlowPatching()` (i.e., insert `WriteTagDefinitions();` between the existing `WriteFilters();` and `WriteDataFlowPatching();` lines, and the mirror in `Read()`). The exact position relative to `Nodes`/`Filters` does not matter functionally (tag definitions have no load-order dependency on nodes or filters), but keeping new code adjacent to `Filters` — the pattern it was copied from — makes the diff easy to review.

In `src/Vixen.Core/IO/Xml/SystemConfig/XmlSystemConfigFilePolicy.cs`, implement both new abstract methods immediately after the existing `WriteFilters()`/`ReadFilters()` implementations, following their exact shape:

    protected override void WriteTagDefinitions()
    {
        XmlElementTagDefinitionCollectionSerializer serializer = new XmlElementTagDefinitionCollectionSerializer();
        XElement element = serializer.WriteObject(_systemConfig.Tags);
        _content.Add(element);
    }

    protected override void ReadTagDefinitions()
    {
        XmlElementTagDefinitionCollectionSerializer serializer = new XmlElementTagDefinitionCollectionSerializer();
        _systemConfig.Tags = serializer.ReadObject(_content);
    }

**5d. Per-node tag persistence.** In `src/Vixen.Core/IO/Xml/Serializer/XmlElementNodeSerializer.cs`:

In `WriteObject`, after building `propertyCollectionElement` (line 42-43), add:

    XElement tagsElement = null;
    if (value.Tags.Any())
    {
        tagsElement = new XElement("Tags", value.Tags.Select(tagId => new XElement("Tag", new XAttribute("id", tagId))));
    }

and add `if (tagsElement != null) result.Add(tagsElement);` alongside the existing `if (propertyCollectionElement != null) result.Add(propertyCollectionElement);` line. Omitting the `<Tags>` element entirely when a node has no tags keeps existing profiles' XML diff-free for untagged nodes and matches the file's existing convention of omitting empty optional elements.

In `ReadObject`, after the existing block that reads `<Properties>` into `node.Properties` (lines 103-108), add a matching block:

    XElement nodeTagsElement = element.Element("Tags");
    if (nodeTagsElement != null)
    {
        foreach (XElement tagElement in nodeTagsElement.Elements("Tag"))
        {
            Guid? tagId = XmlHelper.GetGuidAttribute(tagElement, "id");
            if (tagId != null)
            {
                node.Tags.Add(tagId.Value);
            }
        }
    }

Note this deliberately does **not** validate that `tagId` exists in the tag catalog — at this point in loading, `SystemConfig.Tags` may not be populated yet (nodes and tag definitions are independent top-level collections read in sequence by `SystemConfigFilePolicy.Read()`). Catalog validation happens once, centrally, in Milestone 5e.

**5e. `VixenSystem` load/save/seed/cleanup wiring.** In `src/Vixen.Core/Sys/VixenSystem.cs`:

Add `public static List<ElementTagDefinition> TagDefinitions { get; private set; }` near the other manager properties (e.g. next to `public static NodeManager Nodes { get; private set; }` at line 363). It is a plain `List<ElementTagDefinition>`, not a manager class, because — unlike `Filters` or `DataFlow` — the tag catalog has no execution-time/runtime behavior; it is just a list `ElementTagService` reads and mutates directly. Make it `public` (not `internal` like `SystemConfig`) because `ElementTagService`, which is also in `Vixen.Core` but conceptually the intended access point, needs it, and other `Vixen.Core`-internal code may reasonably need direct list access the way `VixenSystem.Nodes` is used directly all over the codebase.

In `LoadSystemConfig()` (starting line 229), after the existing block that copies collections from `SystemConfig` into runtime managers (after line 277's `Filters.AddRange(SystemConfig.Filters);`, before line 279's `DataFlow.Initialize(...)`), add:

    TagDefinitions = SystemConfig.Tags.ToList();
    EnsureBuiltInElementTags();

and after the existing `CleanUpOrphanedPreviewModuleData();` call at line 281, add:

    CleanUpOrphanedElementTagAssignments();

Add two new private static methods near `CleanUpOrphanedPreviewModuleData()` (around line 284):

    private static void EnsureBuiltInElementTags()
    {
        foreach (var builtIn in BuiltInElementTags.CreateDefaults())
        {
            if (!TagDefinitions.Any(t => t.Id == builtIn.Id))
            {
                TagDefinitions.Add(builtIn);
                Logging.Info($"Seeded built-in element tag '{builtIn.Name}' ({builtIn.Key}).");
            }
        }
    }

    private static void CleanUpOrphanedElementTagAssignments()
    {
        var validTagIds = new HashSet<Guid>(TagDefinitions.Select(t => t.Id));
        foreach (var node in Nodes.GetAllNodes())
        {
            var orphaned = node.Tags.Where(id => !validTagIds.Contains(id)).ToList();
            foreach (var orphanedId in orphaned)
            {
                node.Tags.Remove(orphanedId);
                Logging.Warn($"Removed orphaned element tag assignment {orphanedId} from node '{node.Name}' ({node.Id}).");
            }
        }
    }

`EnsureBuiltInElementTags()` is what makes "existing profiles without tag data should load successfully" and "seed any missing built-in tag definitions" true: an old `SystemConfig.xml` with no `<Tags>` element at all deserializes to an empty `Tags` collection (per the lazy-default in `SystemConfig.cs`), and the three built-ins are added fresh every time they're missing — this is also what makes it idempotent (loading a file that already has all three built-ins adds nothing).

In `SaveSystemConfigAsync()` (line 161), add `SystemConfig.Tags = TagDefinitions;` alongside the existing lines `SystemConfig.OutputControllers = OutputControllers; ... SystemConfig.Filters = Filters; SystemConfig.DataFlow = DataFlow;` (around line 182-188).

Add unit tests in `src/Vixen.Tests/IO/XmlElementTagDefinitionSerializerTests.cs` and `.../XmlElementTagDefinitionCollectionSerializerTests.cs` (these do not touch `VixenSystem` — they operate directly on `XElement` and `ElementTagDefinition` instances) covering: a definition with all fields populated round-trips exactly (id, key, name, isBuiltIn, description, displayColor, sortOrder all preserved); a definition with null `Description`/`DisplayColor` round-trips without writing those attributes and reads back as `null`; reading an element with no `key` attribute yields `Key == ""` rather than throwing; reading an empty `<Tags/>` element yields an empty collection; a collection of three definitions round-trips in the same order.

If `XmlElementNodeSerializer`'s existing constructor dependency (`_underlyingElementMap`, required for resolving leaf `channelId` references) makes constructing a bare `XmlElementNodeSerializer` awkward in a unit test, it is acceptable to test the `<Tags>` read/write logic for nodes via a smaller, focused test that constructs a real `ElementNode` (via its `internal` constructor — `Vixen.Tests` can access `internal` members of `Vixen.Core` only if `InternalsVisibleTo` is configured; check `src/Vixen.Core/Vixen.Core.csproj` and `AssemblyInfo`/`Directory.Build.props` for an existing `InternalsVisibleTo` entry granting `Vixen.Tests` access before assuming this works, and if none exists, do not add one purely for this feature — instead test node-level tag persistence manually in Milestone 7 and note this limitation in Surprises & Discoveries). Do not spend more than a short investigation on this; the `ElementTagDefinition`/collection serializer tests above are the primary automated coverage for Milestone 5.

### Milestone 6 — Project skills

Every new/modified public or protected member added in Milestones 3-5 (`ElementTagDefinition`, `ElementTagCollection`, `IElementNode.Tags`, `ElementNode.Tags`, `ProxyElementNode.Tags`, every public method on `ElementTagService`, `SystemConfig.Tags`) needs XML doc comments per `CLAUDE.md`'s "XML Docs" section and the `csharp-docs` skill (`.agents/skills/csharp-docs/SKILL.md` — read it before writing docs, it has project-specific conventions beyond the generic skill). Then run the `dotnet-best-practices`, `csharp-async` (there is no async code added by this plan, but confirm nothing was accidentally introduced that should be async, e.g. any future file I/O), and `dotnet-design-pattern-review` skills against the diff and address their findings before moving to Milestone 7.

### Milestone 7 — Manual verification and closeout

Build the full solution (`msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`) and launch `Vixen.Application`. Open an existing profile that predates this change (any profile in `Debug/Output/` from before this branch, or a fresh default profile if none exists). Confirm the application opens without error. Check the NLog log output (Vixen's log file location — typically under the application's data directory; find the exact path via the existing NLog configuration file, e.g. `NLog.config`, if unsure) for the three `Logging.Info("Seeded built-in element tag ...")` lines from `EnsureBuiltInElementTags()`, proving the three built-ins were seeded on load of a profile that had no tag data. Save the profile and reopen it; confirm the seeding log lines do *not* reappear (proving `EnsureBuiltInElementTags()` is idempotent once the tags are persisted).

Update the JIRA issue VIX-3933 with a summary of what was implemented, the final test results (`dotnet test` output), and transition it per the team's normal workflow (use the `jira` skill — check available transitions with `mcp__atlassian__getTransitionsForJiraIssue` rather than assuming a transition ID). Update this ExecPlan's `Outcomes & Retrospective` section summarizing what shipped, what (if anything) was deferred, and any lessons learned — in particular, record whatever was actually decided about the `XmlElementNodeSerializer` node-level test coverage gap noted in Milestone 5.

## Concrete Steps

Run all commands from the repository root, `C:\Dev\Vixen`.

1. `git checkout master && git pull && git checkout -b VIX-3933` — Milestone 2.
2. Create/edit the files listed in Milestones 3-5 using an editor; there is no code generation step.
3. After each milestone's files are in place: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~ElementTag"` to run just the new tests, expecting all to pass with `0` failed.
4. `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` after each milestone to confirm the whole solution still compiles (a full `Rebuild` is only necessary before Milestone 7's manual run).
5. Before Milestone 7: `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`, then launch `Debug\Output\Vixen.Application\Vixen.exe` (or the equivalent path reported by the build) and follow the manual verification steps in Milestone 7.
6. Commit after each milestone with messages prefixed `VIX-3933`, e.g. `VIX-3933 Add ElementTagDefinition, ElementTagCollection, and IElementNode.Tags`.

## Validation and Acceptance

- `dotnet test src/Vixen.Tests/Vixen.Tests.csproj` reports all existing tests still passing plus the new tests from Milestones 3-5 (`ElementTagCollectionTests`, the `ElementTagService` validation-helper tests, `XmlElementTagDefinitionSerializerTests`, `XmlElementTagDefinitionCollectionSerializerTests`) passing, `0` failed.
- `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` succeeds with no new errors or warnings attributable to this change.
- Opening a pre-existing profile (saved before this branch existed) in the running application does not throw, and the log shows the three built-in tags being seeded exactly once (on first load with no existing tag data), and not on subsequent loads once the profile has been re-saved.
- Adding the same `Guid` to an `ElementNode.Tags` twice via code (e.g. in a quick scratch test or the debugger) results in `Tags.Count == 1`, proving duplicate-assignment prevention.
- `ProxyElementNode.Tags` is always empty and calling `Add` on it never throws and never changes `Contains` results for any other `ProxyElementNode` instance (since they all share the one `Empty` instance) or for any real node.

## Idempotence and Recovery

Every step in this plan is safe to re-run. Re-running `EnsureBuiltInElementTags()` (i.e., reloading the profile) never duplicates built-in tags because it checks for an existing `Id` match first. `ElementTagCollection.Add` is idempotent by construction (backed by a `HashSet<Guid>`). If a milestone's code change causes a build failure, fix it and recompile — no step depends on a prior partial/failed state existing on disk; `SystemConfig.xml` is only ever fully rewritten by `SaveSystemConfigAsync()`, never partially patched. If manual verification in Milestone 7 reveals a corrupted or unexpected `SystemConfig.xml`, do not hand-edit the shipped profile under test — instead point the application at a scratch/copy profile for verification, leaving the developer's real working profiles untouched.

## Artifacts and Notes

The three fixed built-in tag GUIDs (repeated here for convenience, must match `BuiltInElementTags.cs` exactly):

    Deprecated: 753115da-e27a-4261-a136-8222ccc3f22e
    Hidden:     2f21bfcb-07a3-4cac-bdc5-4b6e37773c04
    Prop:       6db1852d-71d8-449d-afa5-193a6def26a0

JIRA issue for this plan: [VIX-3933](https://vixenlights.atlassian.net/browse/VIX-3933) ("Element Tags: core API and persistence (catalog, node assignment, ElementTagService)"), scoped explicitly to the API/persistence layer with Display Setup/Sequencer UI, filtering, and visual display called out as out-of-scope/follow-on work. Linked "Relates to" [VIX-2690](https://vixenlights.atlassian.net/browse/VIX-2690) ("Eliminated element setting"), the original feature request this work grew out of.

Design source of truth for all product/behavior decisions referenced throughout this plan: `docs/vix-3933-element-tags.md` in this repository — read it in full before starting if any requirement referenced above seems ambiguous; do not guess at intent that document already resolves.

## Interfaces and Dependencies

In `Vixen.Sys` (`src/Vixen.Core/Sys/`), define:

    public interface IElementNode : IGroupNode  // extended, not new
    {
        // ...existing members unchanged...
        ElementTagCollection Tags { get; }
    }

    public sealed class ElementTagDefinition
    {
        public Guid Id { get; }
        public string Key { get; }
        public string Name { get; set; }
        public bool IsBuiltIn { get; }
        public string? Description { get; set; }
        public string? DisplayColor { get; set; }
        public int SortOrder { get; set; }
    }

    public static class BuiltInElementTags
    {
        public const string DeprecatedKey = "deprecated";
        public const string HiddenKey = "hidden";
        public const string PropKey = "prop";
        public static readonly Guid DeprecatedId;
        public static readonly Guid HiddenId;
        public static readonly Guid PropId;
        public static IReadOnlyList<ElementTagDefinition> CreateDefaults();
    }

    public sealed class ElementTagCollection : IEnumerable<Guid>
    {
        public bool Add(Guid tagId);
        public bool Remove(Guid tagId);
        public bool Contains(Guid tagId);
        public int Count { get; }
    }

In `Vixen.Services` (`src/Vixen.Core/Services/`), define:

    public class ElementTagService
    {
        public static ElementTagService Instance { get; }
        public IReadOnlyList<ElementTagDefinition> GetAll();
        public ElementTagDefinition? GetByKey(string key);
        public ElementTagDefinition? GetById(Guid id);
        public ElementTagDefinition CreateUserTag(string name);
        public void RenameUserTag(Guid id, string newName);
        public void DeleteUserTag(Guid id);
        public bool HasTag(IElementNode node, Guid tagId);
        public bool HasTag(IElementNode node, string key);
    }

`VixenSystem` (`src/Vixen.Core/Sys/VixenSystem.cs`) gains `public static List<ElementTagDefinition> TagDefinitions { get; private set; }`, populated and seeded in `LoadSystemConfig()`, saved back to `SystemConfig.Tags` in `SaveSystemConfigAsync()`.

Depends on nothing new externally — no new NuGet packages. All new code lives in `Vixen.Core`, which every other project in the solution already references.

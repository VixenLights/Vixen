# VIX-3931: Widen Public APIs from `ElementNode` to `IElementNode`

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`,
`Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.
Maintain this document in accordance with `.agents/PLANS.md`.

---

## Purpose / Big Picture

Vixen already defines an interface, `Vixen.Sys.IElementNode` (`src/Vixen.Core/Sys/IElementNode.cs`),
and there are two concrete implementations: `ElementNode` (the real node in the live tree) and
`ProxyElementNode` (a lightweight stand-in used in the TimedSequenceMapper, identified by `IsProxy == true`).
However, many public APIs — including the `IElementTemplate` interface, the application setup control
interfaces, and various preview and effect helpers — accept or return the **concrete** `ElementNode` class
directly rather than the interface. This means a `ProxyElementNode` cannot legally be passed to those
APIs even when the code only needs to traverse the node tree (read name, enumerate children, inspect
properties). It also makes those APIs untestable with stub implementations.

After this change, every public API that performs only read/traverse operations on element nodes will
accept `IElementNode` instead of `ElementNode`. Code that creates, deletes, or mutates the node tree
structure will continue to use the concrete `ElementNode` because those operations (adding or removing
children, moving nodes in the tree) call methods on the underlying `GroupNode<Element>` base class
that are not and cannot be part of the interface.

The practical win: `ProxyElementNode` instances can flow through element template calls, setup dialogs,
and preview helpers without being forcibly cast or causing `InvalidCastException` at runtime.

---

## Progress

- [x] (2026-06-22) M1: Created JIRA issue VIX-3931; plan file renamed; Decision Log updated
- [x] (2026-06-22) M2: Widened `IElementTemplate.SetupTemplate` and `GenerateElements` parameters to `IEnumerable<IElementNode>?`; all 8 implementations updated; build clean, 73/73 tests pass
- [x] (2026-06-22) M3: All three candidate sites in the setup control interfaces are ineligible under the decision rule; zero code changes; Decision Log updated with per-site rationale
- [x] (2026-06-22) M4: Triage remaining eligible files in `src/Vixen.Modules/`; one eligible site found and widened (`CountPixelsAndStrings`); all other candidates ineligible under the decision rule

---

## Surprises & Discoveries

- **M3 yielded zero code changes.** The plan's "Interfaces and Dependencies" section listed aspirational final signatures for `ISetupElementsControl` and `ISetupPatchingControl`. After applying the decision rule to all three candidate sites, every site was found ineligible (see Decision Log). The selection and patching paths carry live-tree `ElementNode` instances exclusively; `ProxyElementNode` never reaches these APIs. The aspirational signatures in that section cannot be achieved without new casts; the section should be read as a goal template, not a guarantee.

---

## Decision Log

- Decision: JIRA issue VIX-3931 created for this refactoring work.
  Rationale: VIX-2690 was already in use for a separate ticket; a new issue was required.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: Target only read/traverse sites; leave mutation sites (`NodeManager`, `ElementNodeService`) as `ElementNode`.
  Rationale: The mutations call `GroupNode<Element>` methods (`AddChild`, `InsertChild`, `RemoveFromParent`,
  `IndexOfChild`) that are not on the interface. Widening those sites would require either casting back to
  `ElementNode` (defeating the purpose) or pulling mutation onto the interface (which would require
  `ProxyElementNode` to implement no-op stubs for tree modification — wrong).
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M3 — `ISetupPatchingControl.UpdateElementSelection` and `UpdateElementDetails` left as `IEnumerable<ElementNode>`.
  Rationale: `SetupPatchingGraphical._updateElementDisplay` stores the incoming nodes in `List<ElementNode> rootNodes` and passes them into `_UpdateElementShapesFromElements`, which rebuilds `_elementNodeToElementShapes: Dictionary<ElementNode, List<ElementNodeShape>>`. Looking up or inserting into that dictionary requires `ElementNode` as the key. Widening the method signature to `IEnumerable<IElementNode>` would require either a `(ElementNode)` cast at the dictionary lookup (prohibited) or changing the dictionary key type to `IElementNode` — which is architecturally out of scope for this milestone and would cascade into `ElementNodeShape.Node` (typed `ElementNode`). Since both implementations of the interface must share the same signature, and one implementation (`SetupPatchingGraphical`) is ineligible, the interface method stays typed as `ElementNode`.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M3 — `ISetupElementsControl.SelectedElements` left as `IEnumerable<ElementNode>`.
  Rationale: The setter in `SetupElementsTree` calls `elementTree.PopulateNodeTree(value)`, where `PopulateNodeTree` takes `IEnumerable<ElementNode>`. Passing `IEnumerable<IElementNode>` to it would fail to compile without widening `PopulateNodeTree` in `ElementTree.cs` (Vixen.Common), which cascades into `GenerateEquivalentTreeNodeFullPathFromElement`. Those methods use only interface members and would widen cleanly, but expanding into `Vixen.Common` is out of scope for M3. C# does not permit a property getter and setter to declare different types, so the getter cannot be narrowed independently. The practical impact is minimal: selection in the UI tree always contains live `ElementNode` instances, never `ProxyElementNode`.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M3 — `ElementNodesEventArgs.ElementNodes` left as `List<ElementNode>`.
  Rationale: The field is consumed in `DisplaySetup.cs` at the `ElementSelectionChanged` handler, which passes `e.ElementNodes` directly to `_currentPatchingControl.UpdateElementSelection(...)`. Since `UpdateElementSelection` is ineligible (see above), widening `ElementNodes` to `List<IElementNode>` would make passing it to `UpdateElementSelection(IEnumerable<ElementNode>)` a compile error — `IEnumerable<IElementNode>` is not assignable to `IEnumerable<ElementNode>` in the contravariant direction. The widening is blocked by the ineligibility of its only consumer in this file.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M4 — `PreviewTools.CountPixelsAndStrings` parameter widened from `ElementNode` to `IElementNode`; foreach loop variable widened from `ElementNode` to `IElementNode`.
  Rationale: Body uses only interface members (`ParentNode.Children`, `child.IsLeaf`). The single caller in `PreviewMultiString.cs:332` passes `inputElements: ElementNode` — covariant-safe into `IElementNode`. No new casts introduced anywhere in the call chain. XML docs updated per csharp-docs skill (fixed "Retruns" typo, added `<param>` tags).
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M4 — `PreviewTools.GetLeafNodes` and `GetParentNodes` left as `ElementNode`.
  Rationale: Both methods accumulate iterated children into `List<ElementNode>` and return it. Widening the parameter to `IElementNode` forces the `foreach` variable to `IElementNode`, which forces the list element type to `IElementNode`, which forces the return type to `List<IElementNode>`. Every caller then receives `IElementNode` items and immediately assigns them to `PreviewPixel.Node` (typed `ElementNode`) or passes them to `PreviewLine(ElementNode selectedNode)` — both require `ElementNode`. The cascade from parameter → return type → caller forces new casts at 8+ call sites. Entire method family stays typed as `ElementNode`.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M4 — `PreviewBaseShape.Reconfigure(ElementNode node)` virtual method family left as `ElementNode`.
  Rationale: 13 override bodies contain at least one of these blockers: `pixel.Node = node` (`PreviewPixel.Node` is `ElementNode`); `AddPixels(node, lightCount)` (`PreviewLightBaseShape.AddPixels` takes `ElementNode`); `new PreviewLine(..., node, ZoomLevel)` (`PreviewLine` constructor takes `ElementNode`); private helpers `IsPixelTreeSelected(ElementNode)`, `IsStandardTreeSelected(ElementNode)`, `IsPixelGridSelected(ElementNode)`, `IsStandardGridSelected(ElementNode)`, `IsPixelStar(ElementNode)`, `AddAllChildren(ElementNode)` (all take `ElementNode`); `initiallyAssignedNode = node` (field typed `ElementNode`). A virtual method family must be entirely castless at every override — one blocker in any override disqualifies the whole family. The family has 13 overrides, all blocked. Stays as `ElementNode`.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M4 — `PreviewCustomProp.AddLightNodes(ElementModel model, ElementNode node)` left as `ElementNode`.
  Rationale: Body immediately assigns `p.Node = node` where `p.Node` (`PreviewPixel.Node`) is typed `ElementNode`. Widening the parameter to `IElementNode` makes `p.Node = node` a compile error without a cast. Ineligible.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M4 — `ElementModeling.ElementsToSvg(ElementNode elementNode)` left as `ElementNode`.
  Rationale: Body calls `elementNode.GetLeafEnumerator()` into `leafNodes`, then `OrderNodes(leafNodes, elementNode)`. `OrderNodes` takes `(IEnumerable<ElementNode>, ElementNode)`. If the parameter is widened to `IElementNode`, `GetLeafEnumerator()` dispatches to the interface definition returning `IEnumerable<IElementNode>`, making `leafNodes: IEnumerable<IElementNode>`. Passing that to `OrderNodes(IEnumerable<ElementNode>, ElementNode)` requires a cast. Ineligible.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M4 — `VixenPreviewControl.HighlightNode(ElementNode node)` and `OrderModule.AddPatchingOrder(IEnumerable<ElementNode>)` not widened (no proxy value).
  Rationale: Both are mechanically eligible (bodies use only interface members; callers pass `ElementNode` which is covariant-safe). However, `ProxyElementNode` never reaches either path: `HighlightNode` is called from UI selection events where nodes come from `ElementTree.SelectedElementNodes` (live tree only); `AddPatchingOrder` is called from element template generation which constructs real `ElementNode` objects. Widening these APIs would not enable any new proxy flow — it would be churn with no benefit, per the value filter discussed in the M4 Decision Log notes. Left as `ElementNode`.
  Date/Author: 2026-06-22 / Jeff Uchitjil

- Decision: M2 widened only `SetupTemplate` and `GenerateElements` *parameters*; return types of all four `IElementTemplate` methods remain `IEnumerable<ElementNode>`.
  Rationale: `ElementTemplateHelper.ProcessElementTemplate` stores `GenerateElements` output in `IEnumerable<ElementNode> createdElements` and passes its first element to an `Action<ElementNode> addToTree` callback. Widening the return type would make `createdElements.First()` return `IElementNode`, which is incompatible with `Action<ElementNode>` — requiring a cast. Similarly, `GetElementsToDelete` is iterated as `foreach (ElementNode node in ...)` and each node is passed to `treeElements.SelectElementNode(node)` which expects `ElementNode`. `GetLeafNodes` is consumed as `IEnumerable<ElementNode>` in `VixenPreviewControl`. None of these return types can be widened without introducing a new cast at the call site. The parameter widening is safe because `IEnumerable<T>` is covariant — callers passing `IEnumerable<ElementNode>` to `IEnumerable<IElementNode>?` need no cast.
  Date/Author: 2026-06-22 / Jeff Uchitjil

---

## Outcomes & Retrospective

*(Populate at completion.)*

---

## Context and Orientation

### Key types

**`IElementNode`** (`src/Vixen.Core/Sys/IElementNode.cs`) — the interface that both concrete
implementations must satisfy. Its read-only surface covers:

    Element Element { get; set; }
    Guid Id { get; }
    string Name { get; set; }
    IEnumerable<IElementNode> Children { get; }
    IEnumerable<IElementNode> Parents { get; }
    bool IsLeaf { get; }
    int GetMaxChildDepth();
    IEnumerable<IElementNode> GetLeafEnumerator();
    IEnumerable<Element> GetElementEnumerator();
    IEnumerable<IElementNode> GetNodeEnumerator();
    IEnumerable<IElementNode> GetNonLeafEnumerator();
    PropertyManager Properties { get; }
    bool IsProxy { get; }

**`ElementNode`** (`src/Vixen.Core/Sys/ElementNode.cs`) — the real node. Inherits from
`GroupNode<Element>`, which adds `AddChild`, `InsertChild`, `RemoveFromParent`, `RemoveChild`,
`IndexOfChild`, and `Find`. Also exposes `Masked`, `InvalidChildren()`, and `GetAllParentNodes()`,
none of which are on the interface. Note that `ElementNode.Children` returns `IEnumerable<ElementNode>`
(covariant concrete), while `IElementNode.Children` returns `IEnumerable<IElementNode>` — both are
present on the class via explicit interface implementation.

**`ProxyElementNode`** (`src/Vixen.Core/Sys/ProxyElementNode.cs`) — a lightweight `IElementNode`
stand-in holding only `Id`, `Name`, and an empty `Properties`. It reports `IsProxy == true`. Used
by the TimedSequenceMapper when sequences reference elements that may not yet be in the live tree.
It cannot be upcast to `ElementNode`.

**`NodeManager`** (`src/Vixen.Core/Sys/Managers/NodeManager.cs`) — the registry and factory for
real nodes. Its `AddChildToParent`, `MoveNode`, `RemoveNode`, `RenameNode`, etc., all require
concrete `ElementNode` because they call mutation methods from `GroupNode<Element>`. This class
stays as-is.

**`ElementNodeService`** (`src/Vixen.Core/Services/ElementNodeService.cs`) — a thin service layer
over `NodeManager`. It creates nodes and calls `NodeManager` mutation methods. Because it delegates
to `NodeManager`, and `NodeManager` demands concrete types, the service's parameters also stay
`ElementNode` for the mutation-path parameters (`parentNode` in `CreateSingle`, `CreateMultiple`,
`ImportTemplateOnce`, `ImportTemplateMany`; the node in `Rename`).

### The decision rule (repeat at every ambiguous site)

A site **may** use `IElementNode` only when **no new cast back to `ElementNode`** is
introduced anywhere as a result — not in the method body, not in callers receiving a widened
return type, and not inside loops iterating a widened collection. If widening a parameter or
return type would force any code anywhere to write `as ElementNode`, `(ElementNode)`, or
`Cast<ElementNode>()` that was not already there, the site is ineligible and must stay typed
as `ElementNode`.

More specifically, a site **may** use `IElementNode` only if all of the following are true:

- The method body touches only interface members on the node:
  `Element`, `Id`, `Name`, `Children`, `Parents`, `IsLeaf`, `GetMaxChildDepth()`,
  `GetLeafEnumerator()`, `GetElementEnumerator()`, `GetNodeEnumerator()`,
  `GetNonLeafEnumerator()`, `Properties`, `IsProxy`.
- Every caller that receives the widened return value also needs only interface members and
  does not pass the result to any API that requires `ElementNode`.
- Every loop that iterates a widened collection (`foreach (IElementNode x in ...)`) uses only
  interface members on the iteration variable — no concrete-only members anywhere in the loop
  body.

A site **must** keep `ElementNode` if:
- It calls any `GroupNode<Element>` mutation method: `AddChild`, `InsertChild`,
  `RemoveFromParent`, `RemoveChild`, `IndexOfChild`.
- It calls any concrete-only member: `Find`, `Masked`, `InvalidChildren()`,
  `GetAllParentNodes()`, the `IEqualityComparer<ElementNode>` implementation.
- It passes the node (or anything derived from it) to a method that requires `ElementNode`,
  such as `NodeManager.AddChildToParent`, `NodeManager.RemoveNode`, or
  `ElementNodeService.CreateSingle`.
- Widening it would cause any caller or downstream consumer to introduce a new cast.

### Serialisation note

`ElementNode` is `[Serializable]`. The `EffectNodeSurrogate`
(`src/Vixen.Core/Module/SequenceType/Surrogate/EffectNodeSurrogate.cs`) serialises effect target
nodes. Precedent: `IEffect.TargetNodes` is already typed as `IElementNode[]` and round-trips
correctly through this surrogate. Use this as evidence that the interface is serialisation-safe,
but verify at each new serialisation site rather than assuming globally.

### Traversal chain cascades

When a method signature changes from `ElementNode` to `IElementNode`, every `foreach` loop
that previously declared its iteration variable as `ElementNode` now iterates `IElementNode`
instead. If the loop body calls any concrete-only member on that variable, the code will not
compile. Walk the full traversal chain — including nested helpers called from within the loop —
before marking a site as eligible.

The correct response to a cascade failure is **not** to add a cast. It is to conclude that the
site is ineligible and leave it as `ElementNode`. Example:

    // Before — ineligible for widening because body uses GetAllParentNodes():
    foreach (ElementNode child in node.Children) { child.GetAllParentNodes(); }

    // Wrong — do not do this:
    foreach (IElementNode child in node.Children) { ((ElementNode)child).GetAllParentNodes(); }

    // Correct — leave the parameter as ElementNode; this site is out of scope.

If the loop body uses only interface members and the traversal compiles cleanly without any cast,
the site is eligible. If any cast is required anywhere in the chain, the whole method stays typed
as `ElementNode`.

---

## Plan of Work

### Milestone 1 — Create JIRA issue for this refactoring work

Before any code changes, create a new JIRA issue in the Vixen project at
`http://vixenlights.atlassian.net`. Use the `jira` skill
(`.agents/skills/jira/SKILL.md`) to create the ticket with the following content:

- **Summary:** Widen public APIs from `ElementNode` to `IElementNode`
- **Issue Type:** Improvement
- **Description:** Many public APIs in Vixen.Core and Vixen.Application accept or return the
  concrete `Vixen.Sys.ElementNode` class where the `Vixen.Sys.IElementNode` interface would
  suffice. This prevents `ProxyElementNode` — a legitimate second implementation used in the
  TimedSequenceMapper — from flowing through those APIs without an unsafe cast. The refactoring
  widens every read/traverse site to use the interface, leaving only mutation sites (which call
  `GroupNode<Element>` methods not on the interface) as `ElementNode`. Target areas:
  (1) `IElementTemplate` interface and all template subclasses;
  (2) `ISetupElementsControl` and `ISetupPatchingControl` and their implementations;
  (3) preview shape utilities and any other eligible sites found during triage.
  See `docs/plans/vix-3931-ielementnode-refactor.md` for the full plan.
- **Labels / Component:** Core, Refactoring
- **Testing notes:** The implementer must confirm the following after each milestone:
  (1) the full solution builds clean in Debug with zero errors and zero new warnings;
  (2) all existing unit tests pass: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj`;
  (3) a grep for new cast patterns (`as ElementNode`, `(ElementNode)`, `Cast<ElementNode>()`)
  introduced by this change returns zero results in files that were changed;
  (4) the Display Setup form opens, element templates create nodes correctly, and the patching
  panel reflects the correct channels after node creation;
  (5) an existing sequence loads and plays back in the sequence editor and Preview window
  without exceptions. No new tests are required for this refactor.
- **Acceptance criteria:**
  - All changed method signatures use `IElementNode` where the decision rule permits.
  - No new casts to `ElementNode` appear anywhere in changed files.
  - Element template creation (Single Item, Pixel Grid, Megatree) produces the expected nodes.
  - Selecting elements in the setup tree updates the patching panel correctly.
  - Loading and playing a sequence renders effects on preview elements without errors.

Record the new ticket number (e.g., `VIX-3931`) in the Decision Log below and rename this plan
file to match: `docs/plans/vix-3931-ielementnode-refactor.md`. Update the branch name to
`VIX-3931` (or create a new branch from `master` with that name) before committing any code.

All subsequent commit messages must reference the new ticket number.

There is nothing to build or run to validate this milestone; done means the JIRA ticket exists,
its description matches this plan, and the plan file and branch carry the correct ticket number.

### Milestone 2 — Widen `IElementTemplate` and all implementations

`IElementTemplate` (`src/Vixen.Core/Rule/IElementTemplate.cs`) is a public interface in the core
library. It currently exposes four methods using the concrete type:

    bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null);
    Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode> selectedNodes = null);
    IEnumerable<ElementNode> GetElementsToDelete();
    IEnumerable<ElementNode> GetLeafNodes();

Before changing any method in `IElementTemplate`, read the full body of every implementation
in the eight subclasses listed below. Apply the decision rule to each method individually:

- `SetupTemplate` and `GenerateElements` receive `selectedNodes` as input. If any implementation
  passes those nodes to `ElementNodeService.CreateSingle`, `ElementNodeService.CreateMultiple`,
  or any other method that requires `ElementNode`, that method is **ineligible** — widening it
  would force a cast back to `ElementNode` inside the body. Leave it as-is.

- `GetElementsToDelete` and `GetLeafNodes` return nodes. Read every caller in `SetupElementsTree.cs`
  and `DisplaySetup.cs`. If any caller passes the returned nodes to `NodeManager.RemoveNode`,
  `NodeManager.AddChildToParent`, or any other `ElementNode`-typed API, those return types are
  **ineligible** — widening them would force a cast at the call site. Leave them as-is.

Only widen the methods whose bodies and whose callers are fully clear of concrete member access
and concrete-typed downstream APIs. It is acceptable for this milestone to narrow in scope
(or even have zero changes) once the subclass bodies have been read. The purpose is not to force
the interface to use `IElementNode`; it is to widen only where it is safe to do so without any cast.

Files to read and evaluate before making any change:

1. **`src/Vixen.Core/Rule/IElementTemplate.cs`** — note the four method signatures.

2. **`src/Vixen.Application/Setup/ElementTemplates/ElementTemplateBase.cs`** — the abstract base.

3. **All eight template subclasses** in `src/Vixen.Application/Setup/ElementTemplates/`:
   - `SingleItem.cs`
   - `PixelGrid.cs`
   - `NumberedGroup.cs`
   - `Megatree.cs`
   - `LipSync.cs`
   - `Icicles.cs`
   - `StarBurst.cs`
   - `IntelligentFixtureTemplate.cs`

4. **`src/Vixen.Application/Setup/SetupElementsTree.cs`** — callers of all four interface methods.

Acceptance for M2:

Build and test verification — run the following from the repository root and confirm all pass:

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj

Zero build errors required. All existing tests must pass; a failing test is a regression that
must be resolved before proceeding.

Cast verification — run the following grep from the repository root and confirm it returns
zero results in any file that was modified during this milestone:

    grep -rn "as ElementNode\|(ElementNode)\|Cast<ElementNode>" src/Vixen.Core/Rule/ src/Vixen.Application/Setup/ElementTemplates/ src/Vixen.Application/Setup/SetupElementsTree.cs

If any method was left as `ElementNode` because the decision rule disqualified it, record it in
the Decision Log with the reason before marking the milestone complete.

Manual regression — launch `Debug\Output\Vixen.exe`, open Display Setup, and exercise each of
the following template types using the "Add Elements from Template" button:

- Single Item: add one element named "Test Light". Verify one node appears in the element tree
  with that name.
- Pixel Grid: configure a 4×4 grid. Verify 16 leaf nodes appear nested under a group node.
- Megatree: configure a small tree (e.g., 8 strings of 10 pixels). Verify the group/leaf
  structure matches the configured string and pixel counts.

After each template run, verify that patching is still functional: select a leaf node and
confirm the patching panel shows its output channel without errors. Delete all test nodes
before proceeding to avoid polluting the element tree for later milestones.

### Milestone 3 — Widen application setup control interfaces

Two internal application-level interfaces in `src/Vixen.Application/Setup/` expose `ElementNode`
directly:

**`ISetupElementsControl`** (`ISetupElementsControl.cs`):

    IEnumerable<ElementNode> SelectedElements { get; set; }

    // And the companion event arg class in the same file:
    public class ElementNodesEventArgs : EventArgs {
        public List<ElementNode> ElementNodes;
    }

These represent a read-only selection of nodes from the UI tree. Neither interface member needs to
call any concrete-only method; change both to use `IElementNode`.

**`ISetupPatchingControl`** (`ISetupPatchingControl.cs`):

    void UpdateElementSelection(IEnumerable<ElementNode> nodes);
    void UpdateElementDetails(IEnumerable<ElementNode> nodes);

These pass selected nodes from the element tree into the patching panel for display. They are
read/display operations; change both to `IEnumerable<IElementNode>`.

Files to change:

1. **`src/Vixen.Application/Setup/ISetupElementsControl.cs`** — widen `SelectedElements` and
   `ElementNodesEventArgs.ElementNodes`.

2. **`src/Vixen.Application/Setup/ISetupPatchingControl.cs`** — widen both method parameters.

3. **`src/Vixen.Application/Setup/SetupElementsTree.cs`** — implements `ISetupElementsControl`.
   Before changing `SelectedElements`, read every usage of it in this file. If any loop body
   or consumer calls a concrete-only member on the iterated node, or passes the node to a
   method requiring `ElementNode`, the `SelectedElements` property is ineligible and must stay
   typed as `IEnumerable<ElementNode>`. Do not add any cast to resolve a cascade failure.

4. **`src/Vixen.Application/Setup/SetupPatchingSimple.cs`** — implements `ISetupPatchingControl`.
   Update both method signatures; the implementations likely iterate nodes for display only and can
   accept `IElementNode`.

5. **`src/Vixen.Application/Setup/SetupPatchingGraphical.cs`** — same as above.

6. **`src/Vixen.Application/Setup/DisplaySetup.cs`** — the orchestrating form that calls both
   interfaces. Update any local variable types changed by the above.

Acceptance for M3:

Build and test verification:

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj

Zero build errors required. All existing tests must pass.

Cast verification — grep the changed files and confirm no new casts appear:

    grep -rn "as ElementNode\|(ElementNode)\|Cast<ElementNode>" src/Vixen.Application/Setup/ISetupElementsControl.cs src/Vixen.Application/Setup/ISetupPatchingControl.cs src/Vixen.Application/Setup/SetupElementsTree.cs src/Vixen.Application/Setup/SetupPatchingSimple.cs src/Vixen.Application/Setup/SetupPatchingGraphical.cs src/Vixen.Application/Setup/DisplaySetup.cs

If `SelectedElements` or either patching method was left as `ElementNode` because the decision
rule disqualified it, record it in the Decision Log.

Manual regression — launch `Debug\Output\Vixen.exe` and open Display Setup. With at least four
elements in the tree (create them manually if needed), perform each of the following and verify
the patching panel responds correctly each time:

- Select a single leaf node. Patching panel shows its channel and any assigned controller output.
- Select a group node. Patching panel shows all leaf channels beneath it.
- Multi-select two non-adjacent leaf nodes (Ctrl+click). Patching panel shows both channels.
- Deselect all (click empty area). Patching panel clears or shows no selection.
- Select an element, then delete it. No exception is thrown and the patching panel updates.

### Milestone 4 — Triage and upgrade remaining eligible files

The `ElementNode` type appears in approximately 162 source files. The first three milestones
address the highest-value sites. The remaining files must be triaged individually using the
decision rule stated in the Context section.

The following specific sites are known candidates based on initial analysis and should be
addressed in this milestone:

**Preview utility methods** (`src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs`):

    public static List<ElementNode> GetLeafNodes(ElementNode node)
    public static List<ElementNode> GetParentNodes(ElementNode node)
    public static void CountPixelsAndStrings(ElementNode ParentNode, out int Pixels, out int Strings)

Each body iterates `node.Children` and recurses, using only interface-accessible members. Change
all three signatures to accept `IElementNode` and return `List<IElementNode>`. Check every call
site for cascading type changes.

**`PreviewBaseShape.Reconfigure`** (`src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs`):

    internal virtual void Reconfigure(ElementNode node)

This is a virtual method overridden in the preview shape subclasses. Read the base declaration
and every override before touching the signature. All overrides that exist must use only interface
members for the widening to be valid — if even one override calls a concrete-only member, the
entire virtual method family must stay typed as `ElementNode` (a base signature of `IElementNode`
with one override still using `ElementNode` is a compile error). Note the outcome in the Decision
Log regardless of which way it goes.

**All other files in `src/Vixen.Modules/`** — apply the decision rule file by file. Use the
`Grep` tool with pattern `ElementNode[^I]` in each subdirectory to find usages. For each hit:
read the surrounding code, apply the rule, change or leave accordingly. Commit each logical group
of changes (e.g., "all preview shapes", "all property modules") with a message referencing VIX-2690.

**Files to explicitly skip** (mutation sites; document here so future contributors don't re-triage):

- `src/Vixen.Core/Sys/Managers/NodeManager.cs` — all methods call `GroupNode<Element>` mutations.
- `src/Vixen.Core/Services/ElementNodeService.cs` — delegates to `NodeManager`; mutation paths.
- `src/Vixen.Core/Sys/ElementNode.cs` — the concrete class itself.
- `src/Vixen.Core/Sys/ElementNodeProxy.cs` and `ElementNodeTemplate.cs` — serialisation proxies.
- `src/Vixen.Core/IO/**` — serialisers and readers write to/from concrete `ElementNode` via
  reflection or direct construction; changing them risks breaking the save/load round-trip.

Acceptance for M4:

Build and test verification:

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug
    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Release
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj

Zero build errors required for both configurations. All existing tests must pass.

Cast verification — for each file changed during triage, grep for new casts:

    grep -rn "as ElementNode\|(ElementNode)\|Cast<ElementNode>" <changed-file>

Zero results required. If a file was evaluated and left unchanged because it was ineligible,
record it with the reason in the Decision Log — do not leave the decision implicit.

Manual regression — the following sequence exercises the preview shape paths most affected by
this milestone:

1. Launch `Debug\Output\Vixen.exe` and open an existing sequence that has a configured Preview
   window (or configure one: Add Preview → select Display Preview, assign elements to shapes).
2. Verify all preview shapes render their assigned elements at the correct positions. No shapes
   should be blank, mispositioned, or missing element assignments.
3. Press Play on the sequence. Verify effects animate on the preview elements without exceptions
   appearing in the log (`Debug\Output\Vixen.log`).
4. Open the Preview setup dialog and re-assign an element to a shape using the element picker.
   Verify the assignment persists and the shape renders after clicking OK.
5. Close and reopen Vixen. Load the same sequence. Verify all preview assignments are still
   present — confirming that any changed return types did not silently break serialisation.

---

## Concrete Steps

For each milestone, the implementer should:

1. Run the following from the repository root and record the baseline error and test counts before
   making any changes:

       msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
       dotnet test src/Vixen.Tests/Vixen.Tests.csproj

2. Make the changes described in the milestone.

3. Run the build and tests again:

       msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug
       dotnet test src/Vixen.Tests/Vixen.Tests.csproj

   Zero build errors required. All tests that passed at the start of the milestone must still
   pass. If a test now fails, investigate and fix the root cause — do not skip or suppress it.

4. Launch `Debug\Output\Vixen.exe` and perform the manual acceptance check for the milestone.

5. Commit with a message of the form:
       VIX-3931 <short description of what changed>
   where `VIX-3931` is the ticket number created in M1. This ensures the commit appears in the
   ticket's activity log in JIRA.

---

## Validation and Acceptance

All validation is performed after each milestone. The per-milestone acceptance criteria are
stated at the end of each milestone description above. The criteria below summarise the complete
acceptance gate for the full plan. No new tests are required for this refactor; the existing
suite serves as the regression safety net.

### Compile-time and test gate (run after every milestone)

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug
    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Release
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj

All three must pass. The build must produce zero errors. New warnings introduced by the change
must be investigated; they are not automatically acceptable. All existing tests must pass; any
test that fails after a change is a regression that must be fixed before the milestone is marked
complete.

### Cast hygiene gate (run after every milestone)

Run the following from the repository root, substituting the files changed in the milestone:

    grep -rn "as ElementNode\|(ElementNode)\|Cast<ElementNode>" <changed-files>

Zero new results are required. Any pre-existing cast in a file is acceptable only if it was
already there before this branch and the file was otherwise not in scope. Record any
pre-existing cast noticed during triage in the Decision Log to distinguish it from a newly
introduced one.

### Manual functional acceptance (run after all milestones are complete)

Scenario 1 — Element template creation:
Launch `Debug\Output\Vixen.exe`, open Display Setup → Elements tab.

- Add a Single Item element named "Bulb 1". Verify one node named "Bulb 1" appears.
- Add a Pixel Grid (4 columns × 4 rows, name "Grid"). Verify a "Grid" group node with 16 leaf
  children appears.
- Add a Megatree (8 strings of 10 pixels, name "Tree"). Verify a "Tree" group with 8 string
  subgroups each containing 10 leaf nodes.
- Select each of the three top-level nodes in turn and verify the patching panel shows the
  correct channels with no exceptions.

Scenario 2 — Patching panel selection:

- Select a leaf node. Confirm the patching panel shows its output channel.
- Select a group node. Confirm the patching panel shows all channels under the group.
- Multi-select two leaf nodes. Confirm the patching panel reflects both.
- Deselect all. Confirm the patching panel clears without error.

Scenario 3 — Sequence playback:

- Open any existing sequence in the Sequence Editor.
- Press Play. Confirm effects render in the sequence editor timeline and in the Preview window.
- Monitor `Debug\Output\Vixen.log`. No `InvalidCastException`, `NullReferenceException`, or
  unhandled exceptions may appear during or after playback.

Scenario 4 — Preview element assignment persistence:

- Open the Preview setup dialog for a display preview.
- Reassign one shape's element using the element picker.
- Click OK to close.
- Close and reopen Vixen; reload the sequence.
- Confirm the assignment is still present, proving that widened return types did not silently
  break the save/load round-trip.

Scenario 5 — No regression in ineligible sites:

- For every site recorded in the Decision Log as ineligible (left as `ElementNode`), confirm that
  its behaviour is unchanged compared to the `master` branch by exercising the feature it belongs
  to (element templates, patching, preview, etc.) and observing no regressions.

---

## Idempotence and Recovery

All changes are additive interface widenings and local type annotation changes. There is no schema
migration, no file format change, and no new runtime dependency. If a build step fails midway,
simply fix the error and rebuild — there is no partial state to clean up. The branch can be reset
to any prior commit without leaving the tree in an inconsistent state.

---

## Artifacts and Notes

The files involved per milestone, at a glance:

    Milestone 2 (IElementTemplate):
      src/Vixen.Core/Rule/IElementTemplate.cs
      src/Vixen.Application/Setup/ElementTemplates/ElementTemplateBase.cs
      src/Vixen.Application/Setup/ElementTemplates/SingleItem.cs
      src/Vixen.Application/Setup/ElementTemplates/PixelGrid.cs
      src/Vixen.Application/Setup/ElementTemplates/NumberedGroup.cs
      src/Vixen.Application/Setup/ElementTemplates/Megatree.cs
      src/Vixen.Application/Setup/ElementTemplates/LipSync.cs
      src/Vixen.Application/Setup/ElementTemplates/Icicles.cs
      src/Vixen.Application/Setup/ElementTemplates/StarBurst.cs
      src/Vixen.Application/Setup/ElementTemplates/IntelligentFixtureTemplate.cs
      src/Vixen.Application/Setup/SetupElementsTree.cs  (callers)

    Milestone 3 (Setup control interfaces):
      src/Vixen.Application/Setup/ISetupElementsControl.cs
      src/Vixen.Application/Setup/ISetupPatchingControl.cs
      src/Vixen.Application/Setup/SetupElementsTree.cs
      src/Vixen.Application/Setup/SetupPatchingSimple.cs
      src/Vixen.Application/Setup/SetupPatchingGraphical.cs
      src/Vixen.Application/Setup/DisplaySetup.cs

    Milestone 4 (Triage — known candidates):
      src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs
      src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs
      (+ any preview shape subclasses whose Reconfigure overrides touch only interface members)
      (+ all other Vixen.Modules files after per-file triage)

    Explicitly excluded (mutation sites):
      src/Vixen.Core/Sys/Managers/NodeManager.cs
      src/Vixen.Core/Services/ElementNodeService.cs
      src/Vixen.Core/IO/**

---

## Interfaces and Dependencies

At the conclusion of this work, the following signatures must exist (among others):

In `src/Vixen.Core/Rule/IElementTemplate.cs`:

    bool SetupTemplate(IEnumerable<IElementNode> selectedNodes = null);
    Task<IEnumerable<IElementNode>> GenerateElements(IEnumerable<IElementNode> selectedNodes = null);
    IEnumerable<IElementNode> GetElementsToDelete();
    IEnumerable<IElementNode> GetLeafNodes();

In `src/Vixen.Application/Setup/ISetupElementsControl.cs`:

    IEnumerable<IElementNode> SelectedElements { get; set; }

    public class ElementNodesEventArgs : EventArgs {
        public List<IElementNode> ElementNodes;
        public ElementNodesEventArgs(IEnumerable<IElementNode> nodes) { ... }
    }

In `src/Vixen.Application/Setup/ISetupPatchingControl.cs`:

    void UpdateElementSelection(IEnumerable<IElementNode> nodes);
    void UpdateElementDetails(IEnumerable<IElementNode> nodes);

No new projects, NuGet packages, or solution-level changes are required. All changes are within
existing files.

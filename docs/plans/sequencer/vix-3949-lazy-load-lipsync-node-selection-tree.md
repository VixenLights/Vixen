# Lazy-load the LipSync node selection tree

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document in accordance with `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

The LipSync Mapping node-selection dialog currently creates a Windows Forms `TreeNode` for every element in the profile before the dialog is usable. On a large profile this makes the dialog slow to open and leaves unnecessary tree objects in memory even when the user never explores most branches. After this change, the dialog opens with root element nodes only. A branch receives its direct children only when the user expands that branch, so opening the dialog and keyboard range selection remain responsive while the existing target-selection behavior is retained.

The visible proof is simple: open the LipSync Mapping target dialog for a large profile. Root nodes appear immediately with expand glyphs where applicable. Expanding one branch reveals only its direct children; its collapsed descendants do not yet exist as Windows Forms tree nodes. Add targets, recursive addition, group inclusion, matrix options, and multi-select keyboard navigation continue to work as before.

## Progress

- [x] (2026-08-07 16:00Z) Read VIX-3949, `.agents/PLANS.md`, the project Jira workflow, current LipSync dialog source, the shared `MultiSelectTreeview`, `IElementNode`, and the VIX-938 keyboard-selection history.
- [x] (2026-08-07 16:05Z) Updated VIX-3949's description with the finalized specification, acceptance criteria, and test plan; retained its In Progress status.
- [x] (2026-08-07 16:10Z) Implemented root-only tree population, placeholder expand nodes, and one-time direct-child loading in `LipSyncNodeSelect`; verified the LipSyncApp Debug x64 build succeeds.
- [ ] Validate automated tests, manually exercise a large profile, and record results in VIX-3949 and this plan.

## Surprises & Discoveries

- Observation: `MultiSelectTreeview` already performs Shift+Up, Shift+Down, Shift+Home, and Shift+End over visible nodes only. Its `NextVisibleNode` and `LastVisibleNode` do not expand collapsed branches.
  Evidence: `src/Vixen.Common/Controls/MultiSelectTreeview.cs` implements visible-node traversal, and `src/Vixen.Tests/Common/MultiSelectTreeviewKeyboardSelectionTests.cs` covers Shift navigation and collapsed-descendant skipping.

- Observation: the current dialog resolves a selected tree node by `TreeNode.Text`, scanning `VixenSystem.Nodes` for every element node with that name. Duplicate element names therefore currently add every name match rather than the one visible tree item.
  Evidence: `LipSyncNodeSelect.buttonAdd_Click` calls `findAndAddElements(treeNode.Text, recurseCB.Checked)`; `findAndAddElements` loops over `VixenSystem.Nodes` and compares `node.Name`.

## Decision Log

- Decision: preserve the existing name-based selected-target lookup; use `TreeNode.Tag` only as the backing `IElementNode` needed to lazy-load the node's direct children.
  Rationale: this removes eager hierarchy construction without changing the established duplicate-name behavior of Add. Replacing the lookup with `Tag` would cause a selected duplicate name to add only one node instead of all matching nodes and would be a behavior change unrelated to the performance remediation.
  Date/Author: 2026-08-07 / Codex

- Decision: track expanded/populated UI nodes with a private `HashSet<TreeNode>` rather than inferring state solely from child count or placeholder text.
  Rationale: a leaf has zero children after population, while an unpopulated expandable node has one placeholder. An explicit set makes the one-time-population guarantee unambiguous, does not depend on display text, and avoids accidental duplicate child insertion.
  Date/Author: 2026-08-07 / Codex

- Decision: add a single blank placeholder child to every source element node that has direct children.
  Rationale: a Windows Forms `TreeView` only displays an expand glyph for a node with child `TreeNode` objects. The placeholder provides that glyph without constructing real descendant UI nodes.
  Date/Author: 2026-08-07 / Codex

## Outcomes & Retrospective

Planning outcome: a source-local, behavior-preserving lazy-loading design has been selected. VIX-3949 now records the finalized implementation requirements, acceptance criteria, and validation plan. No application code has been changed.

Implementation outcome: the dialog's initial load now creates display nodes for roots only. Each expandable display node carries its source `IElementNode` in `Tag`, receives one blank placeholder, and replaces that placeholder with direct children on its first expansion. The Add path remains name-based. Automated and manual behavioral validation remains pending in Milestone 3.

## Context and Orientation

`src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` is a legacy Windows Forms dialog used by the LipSync application module to choose mapping targets. It owns `nodeTreeView`, a `Common.Controls.MultiSelectTreeview` declared by `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.Designer.cs`. `MultiSelectTreeview` is the repository's `TreeView` subclass that supports multiple selected rows and Shift-based visible-range selection.

The dialog currently calls `VixenSystem.Nodes.GetRootNodes()` in `LipSyncNodeSelect_Load`. For each root, it constructs a display `TreeNode` and calls `BuildNode`. `BuildNode(TreeNode parentNode, IElementNode node)` recursively constructs all descendants before attaching each result to the UI. This is the eager work to remove.

An `IElementNode`, declared in `src/Vixen.Core/Sys/IElementNode.cs`, is a profile element-tree node. Its `Name` supplies display text, its `Children` property enumerates only direct child element nodes, and `IsLeaf` states whether it has no children. `VixenSystem.Nodes.GetRootNodes()` returns only the top-level element nodes. Store the matching `IElementNode` in the display node's `Tag` property; `Tag` is an object slot provided by Windows Forms for app-specific metadata.

The dialog's right-hand `chosenTargets` list stores actual `IElementNode` objects. `AllowGroups` controls whether a selected non-leaf node may be added itself. `AllowRecursiveAdd` controls whether the existing `addElementNodes` method walks all children when targets are added. Neither setting controls how the left tree is rendered, so lazy rendering must not alter either behavior.

The VIX-938 changes in `MultiSelectTreeview` are relevant but require no modification. They navigate and select visible rows, which means lazy collapsed branches naturally remain outside keyboard range traversal until the user expands them.

## Plan of Work

Milestone 1 updates VIX-3949 before code work. Replace or amend the issue description so it matches this plan's final requirements and removes the unresolved choice about changing name lookup. State that `TreeNode.Tag` identifies the element for lazy child population only, while Add keeps its current name-based lookup. Add the acceptance criteria and test plan from `Artifacts and Notes`. Do not transition the issue solely for this planning update.

Milestone 2 replaces recursive UI construction in `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs`. Add a private `HashSet<TreeNode>` field, for example `_populatedTreeNodes`, initialized once per dialog. This collection records UI nodes whose real direct children have been populated. It is private implementation state, not a new public API.

Replace `BuildNode` with a helper whose responsibility is to create one display node for exactly one `IElementNode`; name it to make that scope clear, such as `CreateTreeNode`. The helper must set the returned node's `Text` from `elementNode.Name` and set `Tag` to the same `IElementNode`. It must not recursively call itself. When `elementNode.Children.Any()` is true, add exactly one blank placeholder `TreeNode` beneath the returned node. Do not put an `IElementNode` in that placeholder's `Tag`; it is only the expand-glyph sentinel.

In `LipSyncNodeSelect_Load`, enclose the initial root-node operation in `nodeTreeView.BeginUpdate()` and `nodeTreeView.EndUpdate()` with `EndUpdate` in `finally`. Clear `nodeTreeView.Nodes` and `_populatedTreeNodes` first so a repeated load cannot retain stale nodes. Enumerate `VixenSystem.Nodes.GetRootNodes()`, call `CreateTreeNode` for each root, and add only those roots to `nodeTreeView.Nodes`. The method must not enumerate descendants to create their display nodes.

Wire `nodeTreeView.BeforeExpand` after `InitializeComponent`, preferably in the dialog constructor, to a private `nodeTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)` handler. The handler must return when `e.Node.Tag` is not an `IElementNode`; this safely ignores placeholders if one is ever presented. It must also return when `_populatedTreeNodes.Add(e.Node)` returns false, because the node has already been populated. Otherwise use `nodeTreeView.BeginUpdate()` / `EndUpdate()` in `try` / `finally`, remove the placeholder children with `e.Node.Nodes.Clear()`, and enumerate only `elementNode.Children`. For each direct child, append `CreateTreeNode(child)` to `e.Node.Nodes`. Do not recursively populate grandchildren. If a source node unexpectedly has no children at expansion time, clearing the placeholder is still correct and leaves it with no expand glyph.

Keep `buttonAdd_Click`, `findAndAddElements`, `addElementNodes`, `AllowGroups`, `AllowRecursiveAdd`, `SelectedElementNodes`, and `SelectedNodeNames` unchanged except for incidental compile-safe renames. In particular, `buttonAdd_Click` must continue passing `treeNode.Text` to `findAndAddElements`; do not substitute `(IElementNode)treeNode.Tag` there. This preserves the existing behavior for same-name elements, which must be explicitly manually validated.

Milestone 3 validates performance and behavior. Do not add brittle UI automation merely to assert WinForms expansion events. First run the existing shared keyboard-selection test class because it is the common behavior most affected by tree visibility. Then run the full test project. Manually test a deliberately large profile, nested branches, a repeated expand/collapse cycle, group and recursion switches, matrix options, keyboard selection, and duplicate element names. Record the exact commands, results, profile used, and any limitations in the living sections of this plan. Update VIX-3949 with the final implementation note, test evidence, and any change to the criteria.

## Concrete Steps

Work from `C:\Dev\Vixen`.

1. Before changing code, verify the target remains as researched and check for user-owned work:

       git status --short
       rg -n "BuildNode|LipSyncNodeSelect_Load|buttonAdd_Click|findAndAddElements|nodeTreeView" src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.Designer.cs

2. Update Jira issue VIX-3949 with the Markdown specification in `Artifacts and Notes` using the repository Jira workflow. The update is a planning requirement; it must happen before implementation.

3. Edit only `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` for the production change. Add the private population set, register the `BeforeExpand` handler, replace the recursive `BuildNode` method with one-node construction, update the load handler to add roots only, and add the guarded one-time expansion handler described in `Plan of Work`. Do not edit the generated designer file unless the existing code style requires the event subscription there; subscription in the constructor avoids an unnecessary designer diff.

4. Confirm the hot path no longer recursively creates UI descendants:

       rg -n -C 3 "CreateTreeNode|BeforeExpand|_populatedTreeNodes|BuildNode|GetRootNodes|elementNode.Children" src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs

   Expected result: the root-load path calls `CreateTreeNode` for roots only, `CreateTreeNode` does not call itself, and `elementNode.Children` is enumerated by the `BeforeExpand` handler only.

5. Run focused shared-tree coverage:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~MultiSelectTreeviewKeyboardSelectionTests --no-restore

   Expected result: the command exits with code 0 and reports all tests in `MultiSelectTreeviewKeyboardSelectionTests` passed, including the collapsed-descendant scenario.

6. Run the full test project:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore

   Expected result: exit code 0 with no failed tests. If dependencies have not been restored, record the failure and retry using the repository Debug build command only when the environment supports it:

       msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

7. Run the manual scenarios in `Validation and Acceptance`. Update the VIX-3949 description if implementation discoveries alter requirements, then add a Jira comment with the actual automated and manual results. Update every living-plan section and append a dated revision note.

## Validation and Acceptance

Automated acceptance requires the focused shared control test and the full test project to pass. The focused test validates that keyboard range selection follows visible tree rows and skips collapsed descendants—the essential navigation contract on which lazy loading relies.

Manual acceptance requires a profile with enough nested elements to make eager construction noticeable:

1. Open the LipSync Mapping node-selection dialog. Before expanding anything, root nodes appear, branches with direct children have an expand glyph, and a debugger or tree inspection shows that only root display nodes plus one placeholder per expandable root exist.
2. Expand one root. Its direct children appear once, child branches show expand glyphs where appropriate, and grandchildren have not been materialized.
3. Collapse and re-expand that root several times. No child is duplicated, ordering and names remain stable, and no exception occurs.
4. Expand a second-level branch. Only that branch's direct children are added; expanding it again does not add duplicates.
5. Select visible rows and press Shift+Up, Shift+Down, Shift+Home, and Shift+End. Selection remains responsive. Collapsed descendants are not selected or traversed; expanded visible rows participate normally.
6. With `AllowRecursiveAdd` enabled and disabled in turn, add selected leaf and group nodes. The selected targets match the pre-change behavior. With `AllowGroups` disabled, groups themselves are excluded; with it enabled, groups can be included.
7. Exercise the matrix orientation controls with `MatrixOptionsOnly` both enabled and disabled. The controls retain their existing visibility and orientation-change warning behavior.
8. In a profile with two or more elements sharing the same name, select one visible matching node and click Add. Confirm the resulting chosen-target list has the same name-based duplicate resolution as the pre-change dialog: every matching node that `findAndAddElements` finds is processed under the existing group and recursion settings.

The feature is accepted when the initial dialog load avoids all-descendant `TreeNode` construction, each branch populates direct children exactly once upon first expansion, current target-add semantics remain intact, and visible-range keyboard navigation remains responsive.

## Idempotence and Recovery

The UI population must be idempotent within a dialog instance. A `BeforeExpand` event for a node already in `_populatedTreeNodes` must leave its child collection unchanged. Re-running the load handler must clear both the display roots and population set before recreating roots, making it safe if the dialog framework invokes the handler more than once.

This change does not alter profile data, mappings, or selected target storage. If it must be backed out, restore the eager `BuildNode` call path in `LipSyncNodeSelect.cs` and remove only the lazy-population field, helper, and event subscription. Do not revert unrelated working-tree changes. If a partial edit causes a branch to appear empty, verify that the source node has children, the parent display node has `Tag` set to that source node, and the `BeforeExpand` subscription has not been omitted.

## Artifacts and Notes

Use the following body to update VIX-3949 before implementation:

    ## Specification

    The LipSync Mapping node-selection dialog currently constructs a Windows Forms TreeNode for every element node in the active profile during dialog load. Large profiles therefore pay the full hierarchy-construction cost before the user expands or selects anything. Change `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` so `nodeTreeView` initially contains only root element nodes.

    Every display TreeNode must retain its backing `IElementNode` in `TreeNode.Tag`. For a source node with direct children, add one blank placeholder child so the tree displays an expand glyph without constructing real descendants. On the first `BeforeExpand` event for a display node, remove its placeholder and add TreeNodes for the source node's direct children only. Each display node must be populated at most once. Use BeginUpdate/EndUpdate around bulk root and child insertion to avoid redraw churn.

    Preserve `AllowGroups`, `AllowRecursiveAdd`, matrix options, selected-target behavior, and `MultiSelectTreeview` selection behavior. Preserve the current name-based add lookup: the Add button must continue passing TreeNode.Text to `findAndAddElements`, so same-name elements retain their existing behavior of all matching names being processed. TreeNode.Tag is for hierarchy population only; it must not change target-resolution semantics.

    ## Acceptance Criteria

    * Opening the dialog constructs root display nodes only; it does not recursively construct all descendant TreeNodes.
    * Nodes with direct source children display an expand glyph before their real children are loaded.
    * First expansion removes the placeholder and materializes immediate children only.
    * Collapsed descendants are not materialized until the user expands their parent.
    * Repeated expansion does not duplicate children.
    * Add targets, Allow Groups, Allow Recursive Add, matrix options, and selected-node behavior remain unchanged.
    * Duplicate element-name behavior remains name-based: selecting one visible matching name still processes every source node matched by the existing lookup.
    * Shift+Up, Shift+Down, Shift+Home, and Shift+End remain responsive and operate over visible rows.

    ## Test Plan

    * Automated: run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~MultiSelectTreeviewKeyboardSelectionTests --no-restore` and then `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore`.
    * Manual: open the dialog with a large nested profile; verify initial root-only display, expand glyphs, immediate-child-only population, and repeated-expansion stability.
    * Manual: verify Shift navigation, group and recursive additions, matrix options, and name-based duplicate-element selection behavior.

Use this Jira completion comment after actual validation, replacing placeholders with evidence:

    Implemented lazy loading for the LipSync Mapping node-selection tree.

    Summary:
    - Initial tree load creates root display nodes only.
    - Expandable nodes receive a placeholder and load direct children once on first expansion.
    - Existing name-based target lookup and selection options were retained.

    Validation:
    - Automated: <focused command and result>
    - Automated: <full test command and result>
    - Manual: <large-profile, expansion, keyboard, option, and duplicate-name results>

## Interfaces and Dependencies

No new public or protected API is required. The production change stays inside `VixenModules.App.LipSyncApp.LipSyncNodeSelect` in `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` and uses existing .NET Windows Forms APIs:

    private readonly HashSet<TreeNode> _populatedTreeNodes = new();

    private TreeNode CreateTreeNode(IElementNode elementNode);

    private void nodeTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e);

`CreateTreeNode` returns exactly one display node, sets `Text` and `Tag`, and adds a blank placeholder only when `elementNode.Children` is non-empty. `nodeTreeView_BeforeExpand` consumes `e.Node.Tag` as `IElementNode`, uses `_populatedTreeNodes` as the one-time guard, and appends `CreateTreeNode` results for direct children. It must not modify `MultiSelectTreeview`, `IElementNode`, profile serialization, or the dialog's selected-target interfaces.

No NuGet packages, project references, designer layout changes, or solution-file changes are expected. Because no public or protected C# API is introduced or altered, this implementation does not require XML documentation changes; retain and do not disturb existing documentation.

## Revision Notes

- 2026-08-07 / Codex: Initial ExecPlan created after reading VIX-3949, `.agents/PLANS.md`, the project Jira workflow, current LipSync dialog source, the shared multi-select tree implementation and tests, `IElementNode`, and VIX-938 history. The plan resolves the ticket's lookup ambiguity by preserving the existing name-based add behavior and limiting `TreeNode.Tag` to lazy hierarchy population.
- 2026-08-07 / Codex: Completed Milestone 1 by replacing the VIX-3949 description with the finalized Specification, Acceptance Criteria, and Test Plan from this ExecPlan. The issue remains In Progress; no source code was changed.
- 2026-08-07 / Codex: Completed Milestone 2 by replacing recursive `BuildNode` construction with root-only `CreateTreeNode` construction and a guarded `BeforeExpand` population path. The target build command `msbuild src\\Vixen.Modules\\App\\LipSyncApp\\LipSyncApp.csproj -t:Build -p:Configuration=Debug -p:Platform=x64 -p:RestoreIgnoreFailedSources=true` succeeded. Milestone 3 owns automated and manual behavioral validation.

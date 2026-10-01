# Restore normal controller output selection after locating patches (VIX-4004)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with `.agents/PLANS.md`. Issue: https://vixenlights.atlassian.net/browse/VIX-4004.

## Purpose / Big Picture


After using Find Patched Outputs in Display Setup, users must be able to select another output normally. Clicking an output without modifier keys must leave only that output selected and update the selected-output count to one. Clicking empty space must remove the selection and its highlights. Repeated finds must replace previous results with exactly the newly matched outputs, including when controller branches are already expanded.

The user confirms that the first find on a collapsed controller works, subsequent finds on that controller break selection until Display Setup is closed and reopened, and clicking empty space fails to remove the resulting highlights. This is the manual reproduction baseline; selection counts during the broken state have not been measured.

## Progress


- [x] (2026-10-01) Read VIX-4004, its comments and attachment metadata, the planning and Jira skills, `.agents/PLANS.md`, relevant source and existing controller virtualization tests.
- [x] (2026-10-01) Confirmed expected unmodified-click behavior and the collapsed-first-find/repeated-find reproduction with the user.
- [x] (2026-10-01) Identified source defects in visual selection projection and top-visible-node lookup; prepared this Rider execution plan.
- [x] (2026-10-01) Milestone 1: Updated VIX-4004 with user-facing scope, acceptance criteria, and a concise test scenario; preserved the existing status and affected versions.

- [x] (2026-10-01) Milestone 2: Fixed complete selection projection and selection-neutral scroll lookup; added regression coverage for 5,000/5,001 outputs, empty result clearing, saved top output, and mouse gestures. Full-MSBuild test target built successfully; focused suite passed 24/24.
- [ ] Milestone 3: Complete manual reproduction, solution validation, and final Jira reporting when authorized.

## Surprises & Discoveries


Observation: selection has several representations. ControllerTree stores controller IDs and output identities independently of the visible tree. MultiSelectTreeview additionally stores selected TreeNode objects, a focus node, a Shift anchor, and highlight colors. Clearing its exposed List directly resets only the list.

Evidence: `ProjectLogicalSelection()` calls `treeview.SelectedNodes.Clear()`. `MultiSelectTreeview.ClearSelectedNodes()` instead resets colors, the list, focus, and anchor. `AddOutputLeaves()` highlights logical matches only when leaves are created. Projecting into an already materialized branch does not recreate leaves or select existing matching leaves.

Observation: the full production refresh creates a failure even though tests using the projection helper do not follow the same sequence. `_PopulateControllerTree()` restores expanded branches before calling `ProjectLogicalSelection()`. Those branches create and select matching leaves; projection subsequently clears their list entries without clearing their colors. A first find on a collapsed branch creates leaves during projection and therefore populates the list successfully.

Observation: restoring scroll position can change selection. `FindNode()` calls `SelectOutput()` for an output identity and retrieves the node from SelectedNodes. This can add the formerly top visible output to the new find result.

Observation: existing coverage missed the reported behavior. `LogicalSelection_ReplacesPreviousOutputs` asserted exported logical indexes without checking highlight colors or interactions. The original supplied-controller population helper also bypassed production expansion and top-node restoration. New pre-fix regressions failed for both 5,000 and 5,001 outputs: the direct-output case lost selected-node membership, and the paged case retained the old output highlight.

Observation: the supplied-controller regression path now shares `_PopulateControllerTree(IEnumerable<IControllerDevice>)` with production, including expansion restoration, logical projection, and saved top-node restoration. STA tests create only hidden control handles through a test hook that skips startup population; they do not create or show a form. They verify saved top output and invoke the control's mouse handlers after repeated finds.

Observation: the prior reference plan, `docs/plans/display-setup/display-setup-ok-performance.md` for VIX-3955, establishes logical selection, collapse eviction, and a final 5,000-output page size. Some older narrative sections retain superseded page sizes or reuse-on-collapse behavior. Current source and the recorded final decisions establish the applicable behavior here: 5,000-output pages and eviction on collapse. Preserve those contracts.

## Decision Log


Decision: repair selection synchronization within the existing ControllerTree rather than replace the shared tree control.
Rationale: source explains the collapsed-first-find/repeated-find difference and stale highlights. Restricting the repair avoids changing unrelated element-tree interactions.
Date/Author: 2026-10-01 / Codex.

Decision: use complete selection clearing followed by explicit projection onto all materialized matching nodes.
Rationale: clearing only the list loses the ability to remove highlights. Depending on creation-time highlighting fails when leaves already exist.
Date/Author: 2026-10-01 / Codex.

Decision: make top-node lookup independent of selection.
Rationale: preserving the scroll position must never add outputs to a replacement selection.
Date/Author: 2026-10-01 / Codex.

Decision: finish with regression and manual evidence before claiming runtime resolution.
Rationale: static source identifies defects, but mouse interactions, counts, and painting still require validation.
Date/Author: 2026-10-01 / Codex.

Decision: route supplied-controller regressions through the same private rebuild sequence as production.
Rationale: tests must observe restored expansions, selection projection, and saved scroll state in production order.
Date/Author: 2026-10-01 / Codex.

## Outcomes & Retrospective


Milestone 2 implementation and automated validation are complete. The selection projection now clears focus, anchor, and highlights before explicitly restoring all materialized logical matches. Saved scroll-node lookup now resolves output leaves by controller identity and output index without selecting them. The focused ControllerTreeVirtualizationTests suite passes 24/24 after a successful full-MSBuild Vixen_Tests build; the two new replacement-selection cases failed before the fix as expected. `git diff --check` passes. Rider file-problem diagnostics were unavailable because the configured Gortex connection has no C# LSP provider. Manual profile-based acceptance is still pending for milestone 3.

## Architecture Design: VIX-4004


Detected IDE Environment: Rider with automation hooks, including `get_file_problems`. Use Rider file navigation and small Apply snippet from chat updates when executing manually; an agent may use the repository's guarded edit tools.

Core Strategy: maintain the existing WinForms UI and synchronize its logical selection and visual selection under `_projectingLogicalSelection`. Apply `.agents/skills/dotnet-best-practices/SKILL.md` for resource handling and focused C# changes. No interface or lifecycle redesign is required. If a public or protected API's behavior or documentation changes, read `.agents/skills/csharp-docs/SKILL.md` and update its XML documentation in the same change.

Data Model & Property Contracts: retain the private OutputIdentity of controller GUID and zero-based output index, controller-root selection semantics, and `GetSelectedControllerOutputs()`. No persisted fields, packages, public signatures, or projects are added. The logical set remains authoritative when collapsed outputs have no TreeNodes. User selection events replace that logical set with the user's current tree selection.

Mathematical / Boundary Logic: retain output bounds `0 <= outputIndex < controller.OutputCount`. Controllers with at most 5,000 outputs have direct leaves; larger controllers use page start `(outputIndex / 5000) * 5000`. Materialize only expanded branches and branches needed for selection or saved scroll position. No timing assertions or eager expansion of every page are required.

Subsystem Component Matrix:

| Component | Responsibility and proposed change |
| --- | --- |
| src/Vixen.Common/Controls/ControllerTree.cs | Clear complete visual selection, explicitly restore materialized matches, and locate scroll nodes without selecting them. Share production rebuild sequencing with a supplied-controller internal test path. |
| src/Vixen.Common/Controls/MultiSelectTreeview.cs | Existing ClearSelectedNodes and AddSelectedNode supply clearing and highlighting. Read and validate their focus/anchor contracts; production edits are not planned unless a regression proves an additional defect necessary to this issue. |
| src/Vixen.Tests/Common/ControllerTreeVirtualizationTests.cs | Assert selected nodes, exported indexes, colors, clearing, repeated find projection, and saved-top-node restoration using the production rebuild sequence. |
| src/Vixen.Application/Setup/SetupControllersSimple.cs | Existing selection property delegates to PopulateControllerTree and count UI consumes exported outputs. Verify counts manually; no production edit is currently required. |

## Context and Orientation


This part of Display Setup is WinForms. `SetupElementsTree.buttonSelectDestinationOutputs_Click()` in `src/Vixen.Application/Setup/SetupElementsTree.cs` discovers patched outputs and calls `DisplaySetup.SelectControllersAndOutputs()` in `src/Vixen.Application/Setup/DisplaySetup.cs`. That assigns `SetupControllersSimple.SelectedControllersAndOutputs`, whose setter calls `ControllerTree.PopulateControllerTree(dictionary)`, then scrolls to the results.

A materialized node means a real TreeNode exists for an output. Collapsed branches discard output TreeNodes and retain logical selection, allowing later expansion to restore the highlights. `_projectingLogicalSelection` suppresses selection capture while rebuilding visual state so clearing/recreating nodes cannot erase the intended logical result.

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Milestone 1: Record the agreed user behavior in Jira


Context: VIX-4004 has a description but no comments or attachments. The current request authorizes evaluation and a local plan. Execute Jira writes only when the user authorizes implementation or issue updates.

Plan of Work: using the project Jira skill, update the issue description with Summary, Scope, and Acceptance Criteria describing normal clicks, empty-space clearing, repeated finds, exact output counts, and direct/paged controller behavior. Include a concise user-facing test scenario. Retain the existing affected versions and status. Read the issue back to confirm the description.

Concrete Steps: inspect `git status --short` from C:\Dev\Vixen and use the Jira connector's structured issue-edit argument. No application source changes occur in this milestone.

Validation and Acceptance: the issue explicitly states that finding A then B selects only B's patched outputs; an unmodified click selects one output with count one; empty-space click clears all selection; both grouped and ungrouped controllers obey these rules.

STOP HERE for manual review and commit execution before proceeding. Halt execution, inspect `git status --short` and scoped `git diff`, and wait for explicit confirmation before advancing. If repository files changed, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4004 and output its paste-ready Commit message. Do not create a commit without explicit authorization.

### Milestone 2: Repair projection and scroll lookup with regression coverage


Context: edit only `src/Vixen.Common/Controls/ControllerTree.cs` and `src/Vixen.Tests/Common/ControllerTreeVirtualizationTests.cs` unless evidence justifies an additional directly related file. Read their current contents and inspect git status first.

Plan of Work: add regression cases before the repair. Exercise a first find on collapsed branches, followed by a replacement find on already expanded branches, for 5,000 and 5,001 outputs. Assert exact SelectedTreeNodes membership, logical exported indexes, matching highlight colors, and normal colors on outputs from the previous result. Also test repeated projection onto an already materialized page, empty result clearing, a saved top-visible output outside the new result, and preserved logical selection through collapse and re-expansion.

Refactor `_PopulateControllerTree()` privately so supplied-controller tests can exercise the same expansion, projection, and scroll restoration sequence used in production. Preserve every public signature; avoid a test-only duplicate implementation of the rebuild. Use an internal overload/helper if necessary. Native TopNode and mouse tests need a realized WinForms control on an STA thread (a thread initialized for Windows UI); dispose handles and join the thread. Do not rely on TreeView handles or expansion behavior silently absent from an unrealized control.

In `ProjectLogicalSelection()`, enable the projection guard before calling complete `ClearSelectedNodes()`. Materialize and expand only required matching controller/page branches. Then restore exactly the selected roots and matching existing output leaves, using `RestoreMaterializedLogicalSelection()` or equivalent existing-node traversal under the same guard. Retain the guard through every clear and restore operation and release it in finally. Ensure restoration works whether nodes were newly created or already existed. Notifications must observe final consistent selection; preserve the outer explicit selection notification.

For `FindNode(NodeIdentity)`, replace output lookup via SelectOutput with a private selection-neutral lookup that resolves the controller, validates the index, materializes the containing branch when needed, and returns its existing output leaf. A private helper shared by SelectOutput is appropriate, but finding a scroll target must not add that target to selection. Direct/paged outputs and duplicate output names must resolve by controller identity and index.

Use the realized control to exercise real unmodified mouse down/up on an unrelated output and clicking empty space after repeated finds. Prefer an existing UI test harness; if unavailable, add only a narrow internal gesture seam tied to production code or retain the physical mouse scenarios as mandatory manual acceptance. Keyboard selection alone does not prove mouse behavior. Keep any new test types scoped to this defect and document public test methods as required by the project csharp-docs skill.

Concrete Steps: from C:\Dev\Vixen, build with full MSBuild, then run the built focused tests before and after the repair. Record the failing regression names before and their passing results afterwards.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter FullyQualifiedName~ControllerTreeVirtualizationTests
    git diff --check

Expected results after repair: MSBuild reports zero errors, focused tests report zero failures, and whitespace validation reports no errors. Do not invent expected pass counts; record the actual results.

Validation and Acceptance: run Rider `get_file_problems` on each changed C# file, resolving only problems on changed lines. Use Gortex impact before edits, detect after edits, and tests/guards/contract assessment afterwards. Any changed signature requires signature verification. Repeated selection, highlights, focus, and exported counts must agree. Expansion and collapse must retain the 5,000-output page boundary and logical hidden-output selection.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run git status and scoped git diff, invoke the project commit-msg skill with VIX-4004, output a complete Commit message, and wait for explicit confirmation. Do not create a commit without explicit authorization.

### Milestone 3: Validate user interactions and report completion


Context: the change affects the shared controls used by the application and tests. Complete focused checks before broader solution validation. Use a copy of a profile containing patched elements, a small controller, and a controller with more than 5,000 outputs.

Plan of Work: manually reproduce the user's scenario with a controller initially collapsed. Find patched outputs for element A, then element B on the same controller, then click an unrelated output without modifiers. Only that output may remain highlighted and selected, and the count must be one. Click empty space: all highlights and selection must clear and the count must be zero. Repeat with initially expanded branches, two pages, two controllers, and the old top-visible output outside the new find result. Check Ctrl toggle, Shift range selection, keyboard navigation after find, and collapse/re-expansion of logically selected outputs. Finding elements with no patches must retain the established Not Found dialog behavior.

Concrete Steps: from C:\Dev\Vixen, reuse Milestone 2's built tests if no source has changed, then build the full affected configuration.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64
    git diff --check

Validation and Acceptance: the full tests pass, the Release x64 solution rebuild reports zero errors, and all manual interactions above behave normally without reopening Display Setup. Record actual warnings, results, and any unavailable manual scenario. If mouse acceptance is unavailable, leave it incomplete rather than declaring the issue resolved.

Update this plan's living sections with final evidence. When Jira reporting is authorized, reconcile the issue description with the final behavior, add a concise validation comment using the Jira skill, and read it back. No workflow transition is included.

STOP HERE for manual review and commit execution before proceeding. Halt execution, inspect status and scoped diffs, invoke the project commit-msg skill if repository files changed, and output its complete VIX-4004 Commit message. Do not create a commit without explicit authorization.

## Validation and Acceptance


The observable contract is exact agreement between highlighted outputs, SelectedTreeNodes, exported logical output indexes, and the UI count after a completed user selection event. Unmodified click replaces selection with one output. Empty-space click clears it. Repeated find replaces previous results and does not select the saved top-visible output. Ctrl and Shift retain normal existing multi-selection behavior. Collapsed branches retain their logical results and restore correct highlights on re-expansion.

Automated coverage must expose the production rebuild order, not just assert the logical HashSet. Manual mouse acceptance is required even when keyboard and selection-helper tests pass.

## Idempotence and Recovery


Repeated find and population calls must produce the same result without accumulating highlights or selection entries. Use exception-safe projection guards and update scopes. Re-read files after user edits and preserve unrelated changes. To recover from an unsuccessful local implementation, inspect and reverse only this issue's scoped edits; never reset the entire working tree. Do not save profile experiments to the user's original configuration.

## Artifacts and Notes


Analysis command: `git status --short` returned no changes before plan creation. The Jira issue has no attachments or comments. Source inspection and user reproduction details are recorded above. No runtime debugger session, build, or test run was performed during planning.

## Interfaces and Dependencies


Preserve existing public ControllerTree population and selection signatures. Use existing ClearSelectedNodes, AddSelectedNode, logical identity records, and materialized-leaf enumeration. No persistence change, project addition, NuGet package, Catel binding, or async behavior change is required. Internal test access may be expanded only enough to invoke production sequencing. Read the applicable project skills before implementation.

## Revision Notes


2026-10-01 / Codex: Created the plan after the user confirmed standard single-click replacement behavior, first-find success on a collapsed controller, later-find failure until reopening Display Setup, and failed empty-space clearing. Scoped implementation to selection projection and selection-neutral scroll lookup, with production-path regression and manual acceptance.

Analysis complete and plan integrated with plans.md.

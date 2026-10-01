# Restore normal controller output selection after locating patches (VIX-4004)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with `.agents/PLANS.md`. Issue: https://vixenlights.atlassian.net/browse/VIX-4004.

## Purpose / Big Picture


After using Find Patched Outputs in Display Setup, users must be able to select another output normally. Clicking an output without modifier keys must leave only that output selected and update the selected-output count to one. Clicking empty space must remove the selection and its highlights. Repeated finds must replace previous results with exactly the newly matched outputs, including when controller branches are already expanded.

The user confirms that the first find on a collapsed controller works, subsequent finds on that controller break selection until Display Setup is closed and reopened, and clicking empty space fails to remove the resulting highlights. This is the manual reproduction baseline; selection counts during the broken state have not been measured.

Follow-up scope (2026-10-01): the original selection fix is implemented and manually validated. Removing outputs before surviving patched outputs now exposes a lookup offset equal to the removed count. This extension must make patch lookup immediately reflect each surviving output's new position during the same Display Setup session. Insertions must retain correct lookup too; the user has not confirmed an insertion failure. The completed milestones below remain historical records, and new work begins at Milestone 4.

## Progress


- [x] (2026-10-01) Read VIX-4004, its comments and attachment metadata, the planning and Jira skills, `.agents/PLANS.md`, relevant source and existing controller virtualization tests.
- [x] (2026-10-01) Confirmed expected unmodified-click behavior and the collapsed-first-find/repeated-find reproduction with the user.
- [x] (2026-10-01) Identified source defects in visual selection projection and top-visible-node lookup; prepared this Rider execution plan.
- [x] (2026-10-01) Milestone 1: Updated VIX-4004 with user-facing scope, acceptance criteria, and a concise test scenario; preserved the existing status and affected versions.

- [x] (2026-10-01) Milestone 2: Fixed complete selection projection and selection-neutral scroll lookup; added regression coverage for 5,000/5,001 outputs, empty result clearing, saved top output, and mouse gestures. Full-MSBuild test target built successfully; focused suite passed 24/24.
- [x] (2026-10-01) Milestone 3: User reported a successful full build, all 1,019 unit tests passing, and manual confirmation that repeated finds replace and scroll to the right outputs, empty-space clicks clear selection, modifier selection adds/removes predictably, and finds across expanded groups work. Manual selection across a group boundary can select the group node; the user identified this as a pre-existing issue outside VIX-4004.

- [x] (2026-10-01 22:12Z) Investigated the output-removal offset: ReIndexOutputs updates output objects but not the manager's cached output indexes. Confirmed insertion already updates shifted cached entries. Preserved the user's pre-existing plan edits.
- [x] (2026-10-01) Milestone 4: Synchronized registered adapter indexes in `ReIndexOutputs()`; added real mutation regressions for removal, insertion, mixed mutations, manager lookup, adapter/output identity, surviving patch sources, event timing, repeated reindex/removal, edge cases, and the 5,001-output boundary. The full-MSBuild test target succeeded and 34 focused tests passed. The fail-before run produced four expected stale-index failures. Rider file diagnostics were unavailable because no C# LSP provider is registered.
- [x] (2026-10-01) Milestone 5: User reports that the full build and all 1,029 unit tests pass. Manual testing confirms correct Find Patched Outputs results after removal, correct insertion lookup, and correct reverse lookup after add and remove. Paged boundary, collapsed/expanded branches, unaffected controller, selection behavior, and persistence/discard scenarios all pass.

## Surprises & Discoveries


Observation: selection has several representations. ControllerTree stores controller IDs and output identities independently of the visible tree. MultiSelectTreeview additionally stores selected TreeNode objects, a focus node, a Shift anchor, and highlight colors. Clearing its exposed List directly resets only the list.

Evidence: `ProjectLogicalSelection()` calls `treeview.SelectedNodes.Clear()`. `MultiSelectTreeview.ClearSelectedNodes()` instead resets colors, the list, focus, and anchor. `AddOutputLeaves()` highlights logical matches only when leaves are created. Projecting into an already materialized branch does not recreate leaves or select existing matching leaves.

Observation: the full production refresh creates a failure even though tests using the projection helper do not follow the same sequence. `_PopulateControllerTree()` restores expanded branches before calling `ProjectLogicalSelection()`. Those branches create and select matching leaves; projection subsequently clears their list entries without clearing their colors. A first find on a collapsed branch creates leaves during projection and therefore populates the list successfully.

Observation: restoring scroll position can change selection. `FindNode()` calls `SelectOutput()` for an output identity and retrieves the node from SelectedNodes. This can add the formerly top visible output to the new find result.

Observation: existing coverage missed the reported behavior. `LogicalSelection_ReplacesPreviousOutputs` asserted exported logical indexes without checking highlight colors or interactions. The original supplied-controller population helper also bypassed production expansion and top-node restoration. New pre-fix regressions failed for both 5,000 and 5,001 outputs: the direct-output case lost selected-node membership, and the paged case retained the old output highlight.

Observation: the supplied-controller regression path now shares `_PopulateControllerTree(IEnumerable<IControllerDevice>)` with production, including expansion restoration, logical projection, and saved top-node restoration. STA tests create only hidden control handles through a test hook that skips startup population; they do not create or show a form. They verify saved top output and invoke the control's mouse handlers after repeated finds.

Observation: manual selection across a group boundary can select the group node itself, including when selecting only the group node. The user identified this as a pre-existing issue and outside the VIX-4004 change.

Observation: the prior reference plan, `docs/plans/display-setup/display-setup-ok-performance.md` for VIX-3955, establishes logical selection, collapse eviction, and a final 5,000-output page size. Some older narrative sections retain superseded page sizes or reuse-on-collapse behavior. Current source and the recorded final decisions establish the applicable behavior here: 5,000-output pages and eviction on collapse. Preserve those contracts.

Observation (follow-up, 2026-10-01): output removal leaves a second representation of the output index stale. In src/Vixen.Core/Sys/Output/OutputController.cs, RemoveOutputs removes requested objects and calls ReIndexOutputs. ReIndexOutputs assigns sequential CommandOutput.Index values, but does not update OutputControllerManager's dictionary from data-flow adapter to controller/index tuple. getOutputDetailsForDataFlowComponent returns the old tuple.

Evidence: removing original zero-based indexes 2 and 3 moves original output 8 to index 6, while its cached lookup remains 8. Near the end of a controller, the stale lookup can exceed the new OutputCount and be discarded by selection bounds checks. For noncontiguous removal, the offset is the number of removed outputs before each survivor, rather than necessarily the total removed count.

Observation (follow-up): InsertOutputsAt already calls UpdateControllerOutputIndex when reattaching each shifted survivor. Newly inserted outputs use AddOutput and normal registration. The source therefore does not establish the same missing-update defect for insertion. Mixed removal/insertion sequences still need regression coverage; there is no user-confirmed insertion failure.

Observation (follow-up): adapter identity is stable across output renumbering. CommandOutputDataFlowAdapterFactory caches adapters by CommandOutput.Id. Updating the existing manager entry can preserve the output object, adapter, and surviving patch. Re-registering or rebuilding the graph is unnecessary for this defect.

Observation (implementation, 2026-10-01): the production loop assigns each survivor's `CommandOutput.Index` and calls `UpdateControllerOutputIndex` for its cached adapter before advancing to the next output. The first test fixture attempt could not configure Moq for internal `IModuleConsumer<T>` members, so the fixture uses a narrow in-test consumer implementation. A full-MSBuild run then succeeded. With the manager update temporarily removed, four removal regressions failed on stale indexes: original output 8 remained at cached index 8 instead of 6; boundary survivors remained at old positions, including output 5000 instead of 4998. Restoring the update made all 34 focused output-index and ControllerTree tests pass. The suite also characterizes insertion and remove-then-insert with real controller mutation paths.

Observation (follow-up): lookup directions use different source data. Elements to Find Patched Outputs uses getOutputDetailsForDataFlowComponent, which reads the stale cache. Outputs to Find Patched Elements in SetupControllersSimple.buttonSelectSourceElements_Click uses the selected current index into oc.Outputs and follows that output's source. Validate both directions with a freshly selected surviving output. An offset only in reverse lookup would require investigating retained logical selection separately; the cache defect alone does not prove that reverse lookup is broken.

Observation (manual validation, user reported 2026-10-01): the full build succeeds and all 1,029 unit tests pass. In Display Setup, Find Patched Outputs identifies the proper outputs after removal, insertion lookup works, and reverse lookup works after both adding and removing outputs. The user also confirms all remaining scenarios pass: paged boundary behavior, collapsed and expanded branches, the unaffected controller, selection behavior, and OK/reopen persistence plus Cancel/reopen discard.

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

Decision: leave standalone group-node selection behavior unchanged in VIX-4004.
Rationale: the user identified it as a pre-existing issue outside this selection-after-find fix.
Date/Author: 2026-10-01 / Codex.

Decision: append output-index synchronization work as Milestones 4 and 5, preserving completed Milestones 1 through 3 and the user's existing completion records.
Rationale: output removal is a newly discovered follow-up; the original selection and scroll repair is validated and must remain intact.
Date/Author: 2026-10-01 22:12Z / Codex.

Decision: update existing registered adapter indexes within OutputController.ReIndexOutputs using OutputControllerManager.UpdateControllerOutputIndex, the existing insertion update path.
Rationale: the output object's Index and the cached tuple must describe the same current position. Repairing the mutation source fixes subsequent find consumers without rebuilding the data-flow graph or compensating in the tree.
Date/Author: 2026-10-01 22:12Z / Codex.

Decision: characterize insertion and both lookup directions without assuming an unconfirmed insertion or reverse-lookup defect.
Rationale: insertion already updates shifted entries; reverse lookup reads current output objects. Covering these paths and mixed operations verifies the contract while keeping production changes tied to demonstrated failures.
Date/Author: 2026-10-01 22:12Z / Codex.

Decision: pair every sequential `CommandOutput.Index` assignment in `ReIndexOutputs()` with `OutputControllerManager.UpdateControllerOutputIndex` for the adapter returned by the controller's existing factory.
Rationale: this updates already registered lookup state in place, retaining output and adapter identity and leaving patch connections intact. The edit does not create registrations for unregistered adapters.
Date/Author: 2026-10-01 / Codex.

## Outcomes & Retrospective


Milestones 2 and 3 are complete. The selection projection clears focus, anchor, and highlights before restoring materialized logical matches. Saved scroll-node lookup resolves output leaves by controller identity and output index without selecting them. The focused ControllerTreeVirtualizationTests suite passed 24/24 after a successful full-MSBuild Vixen_Tests build; the two replacement-selection cases failed before the fix as expected. The user reports that the full build and all 1,019 unit tests pass. Manual validation confirmed repeated finds replace and scroll to the correct outputs, empty-space clicks clear selection, modifier selection adds/removes predictably, and finds across expanded group boundaries work. The remaining manually observed group-node selection behavior is pre-existing and outside this change. `git diff --check` passed. Rider file-problem diagnostics were unavailable because the configured Gortex connection has no C# LSP provider.

Milestone 4 outcome (2026-10-01): `OutputController.ReIndexOutputs()` now synchronizes existing adapter lookup entries along with output positions and documents the identity/source preservation contract. New tests exercise actual `OutputController` mutation paths using fresh managers and restore process-wide manager references in fixture disposal. They assert lookup and object/adapter identity, real surviving patch source references, removal cleanup, lookup consistency before `OutputCountChanged`, repeated reindexing and removal, boundary/noncontiguous/all-output removal, insertions at the beginning/before a survivor/at the end, remove-then-insert, and the 5,001-to-4,999 transition. Against the original loop, four removal regressions failed with stale cached positions; after the repair, the full-MSBuild `Vixen_Tests` target succeeded and the combined focused suite passed 34/34. `git diff --check` passed. Rider file-problem diagnostics remain unavailable because no C# LSP provider is registered. The earlier 24 focused tests and 1,019 full tests remain historical validation of the original selection repair only.

Milestone 5 outcome (2026-10-01): the user reports the full build succeeds and all 1,029 unit tests pass. Manual validation confirms Find Patched Outputs works after output removal, insertion lookup works, and reverse Find Patched Elements works after both addition and removal. The user also reports passing results for the paged boundary, collapsed and expanded branches, an unaffected controller, normal selection behavior, persistence after OK/reopen, and discard after Cancel/reopen. Combined with the 34/34 focused regression suite and fail-before evidence recorded for Milestone 4, the mutation fix and UI acceptance scenarios are complete.

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

Follow-up component scope: src/Vixen.Core/Sys/Output/OutputController.cs is the planned production edit. src/Vixen.Core/Sys/Managers/OutputControllerManager.cs supplies the existing update API and is a read/validation target. Add src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs and src/Vixen.Tests/Core/OutputControllerOutputIndexTestCollection.cs for real mutation coverage. Existing ControllerTreeVirtualizationTests remain regression protection. No tree selection or patch-discovery rewrite is planned for the established missing-cache-update defect.

Follow-up index contract: for every surviving output at array position j, its CommandOutput.Index and the manager lookup of its existing adapter must both equal j and refer to the owning controller. If an original output at i survives removal of the original-index set R, its new position is i minus the count of removed indexes less than i. Inserting c outputs at position p leaves i less than p unchanged and shifts original i greater than or equal to p to i + c. Preserve output and adapter identity, surviving sources, and removal of deleted adapters from the lookup and graph.

## Context and Orientation


This part of Display Setup is WinForms. `SetupElementsTree.buttonSelectDestinationOutputs_Click()` in `src/Vixen.Application/Setup/SetupElementsTree.cs` discovers patched outputs and calls `DisplaySetup.SelectControllersAndOutputs()` in `src/Vixen.Application/Setup/DisplaySetup.cs`. That assigns `SetupControllersSimple.SelectedControllersAndOutputs`, whose setter calls `ControllerTree.PopulateControllerTree(dictionary)`, then scrolls to the results.

A materialized node means a real TreeNode exists for an output. Collapsed branches discard output TreeNodes and retain logical selection, allowing later expansion to restore the highlights. `_projectingLogicalSelection` suppresses selection capture while rebuilding visual state so clearing/recreating nodes cannot erase the intended logical result.

Follow-up orientation: OutputController owns the ordered output array through its mediator. Its private adapter factory returns the same adapter for a surviving output's GUID. VixenSystem.OutputControllers is a process-wide OutputControllerManager whose dictionary caches controller/index pairs; VixenSystem.DataFlow holds registered adapters and patch relationships. RemoveOutputs calls RemoveOutput on deleted objects, ReIndexOutputs on survivors, then updates names and raises OutputCountChanged. The repair must finish index synchronization before that event is raised. InsertOutputsAt already updates the cached indexes of retained shifted adapters as it reattaches them.

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

### Milestone 4: Synchronize output lookup after controller mutations


Context: Milestones 1 through 3 are complete and must not be replayed or edited. The new defect is in src/Vixen.Core/Sys/Output/OutputController.cs, where removal renumbers surviving outputs without refreshing cached manager indexes. Read that file, src/Vixen.Core/Sys/Managers/OutputControllerManager.cs, src/Vixen.Core/Sys/CommandOutputDataFlowAdapterFactory.cs, and the relevant tests before editing. Apply the project dotnet-best-practices and csharp-docs skills, since ReIndexOutputs is public and its documented behavior will change. Keep all production signatures unchanged.

Plan of Work: when Jira editing is authorized for this extension, use the project Jira skill to append user-facing scope, acceptance criteria, and test scenarios for lookup immediately after removing outputs. Describe insertion as required regression behavior, not a confirmed defect. Preserve the existing issue history, affected versions, and completed requirements; read the updated issue back. Current authorization is to investigate and revise the local plan, so no Jira write belongs to this revision turn.

Add src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs. Exercise the real OutputController.RemoveOutputs, ReIndexOutputs, and InsertOutputsAt paths with an in-memory mediator and mocked hardware/module consumer; do not substitute a mocked ReIndexOutputs method or merely test the manager setter. Use a fresh OutputControllerManager and DataFlowManager, registering real output adapters through AddOutput. The constructor requires a module consumer whose Module.DataPolicyFactory can create an IDataPolicy; configure those mocks without starting hardware or loading a persisted profile. If assembly-internal constructor access is unavailable, follow the existing tests' narrow reflection-construction precedent without expanding production public APIs.

Because these paths reference VixenSystem statics, save their prior OutputControllers and DataFlow property values, install test managers through their private setters in a disposable fixture, and restore them even when setup or assertions fail. Put these tests in a new collection with DisableParallelization = true, declared separately in src/Vixen.Tests/Core/OutputControllerOutputIndexTestCollection.cs; use src/Vixen.Tests/Sequencer/SequenceExecutorTestCollection.cs as the existing convention. Do not initialize or shut down the user's application, rewrite configuration, or introduce a production dependency-injection redesign for this regression.

Build output lists with known stable IDs and saved adapter/source references before each mutation. For each survivor, assert its current array position, CommandOutput.Index, manager lookup controller and index, original object identity, adapter identity, and surviving patch source. For each removed output, assert its adapter no longer resolves in the manager and is absent from DataFlowManager. Use real graph source relationships for patched examples, so deletion cleanup and survival are both exercised.

Include a deterministic example with ten outputs: remove original indexes 2 and 3; original index 8 must resolve to 6 and original index 9 to 7 immediately. Test noncontiguous removal, removal at the beginning/end, all outputs removed, and repeated removal. Include a survivor shifted across the 5,000-output page boundary and a count transition from 5,001 to below 5,001. For unaffected outputs before a removed region, indexes remain unchanged. Insertion coverage must include insertion before a patched survivor, at zero, at the end, and removal followed by insertion; verify new outputs are registered exactly once and survivor adapters retain their source connections. Prove ReIndexOutputs can run twice without moving indexes or duplicating registration. Existing insertion characterization may already pass before this fix; record that accurately.

Capture failing removal regressions against the current source before implementing the repair. Change only the ReIndexOutputs loop so every survivor's sequential Index assignment is paired with UpdateControllerOutputIndex(_adapterFactory.GetAdapter(commandOutput), this, index). Use the existing update method rather than AddControllerOutputForDataFlowComponent or RemoveOutput/AddOutput, which would alter registrations or patches. Update the public method's XML summary/remarks to state that it synchronizes output positions and registered lookup indexes without changing output identities or surviving sources. Preserve RemoveOutputs event and naming behavior, insertion's existing cache updates, and all validated ControllerTree selection logic. No general output lifecycle or map-storage redesign is necessary.

Concrete Steps: from C:\Dev\Vixen inspect git status and scoped diffs, run Gortex impact before source edits, and build with full MSBuild before running built tests. Repeat the focused tests after the repair, recording the failing-before/passing-after evidence. Start long-running commands with exec_command and yield_time_ms=10000; when waiting only, use write_stdin with yield_time_ms at least 30000.

    git status --short
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~OutputControllerOutputIndexTests|FullyQualifiedName~ControllerTreeVirtualizationTests"
    git diff --check

Validation and Acceptance: expect no build errors and no focused failures after the repair; removal cases must fail before it because the cached index differs from the survivor's current position. Record actual pass counts rather than reuse the prior 24-test baseline. Run get_file_problems on every changed C# file and address only diagnostics in changed lines. Run Gortex detect and relevant tests/guards/contract checks after source edits. Manager lookup must be correct before OutputCountChanged observers run; include an observer assertion in the removal regression. The existing selection repair tests must remain green.

STOP BOUNDARY: STOP HERE for manual review and commit execution before proceeding. Stop execution, run git status --short and git diff for this milestone's changed files, invoke .agents/skills/commit-msg/SKILL.md using VIX-4004, output its complete paste-ready Commit message, and wait for explicit human review before starting Milestone 5. Do not create a commit unless explicitly authorized.

### Milestone 5: Validate patch lookup after removal and insertion


Context: verify the new cache synchronization through Display Setup using a copy of a profile, then record evidence. Prior selection-fix acceptance remains required. This milestone does not alter completed Milestone 3 or reinterpret its reported results.

Plan of Work: in one Display Setup session, patch distinguishable elements to outputs before and after a removable region on a direct-output controller. Remove two outputs in the region. Find Patched Outputs for the surviving elements and verify their new positions, patch icons, and counts agree, including the final surviving output. Then freshly select each expected output and use Find Patched Elements to verify the original associated element is found. Record both lookup directions explicitly; reverse lookup starts from the selected current output, so a reverse-only failure must be traced separately rather than presumed fixed by the manager update.

Repeat for noncontiguous removals, a paged controller crossing the 5,000 boundary, initially collapsed and already expanded branches, and a second unaffected controller. Insert outputs before a surviving patched element, then repeat both lookup directions. Perform remove-then-insert and repeated remove/find sequences without closing Display Setup. The inserted outputs should be unpatched, removed outputs should no longer have patch points, and surviving patch relationships should retain their elements. Verify normal unmodified clicks, empty-space clearing, Ctrl selection, and counts still satisfy the completed selection fix. Verify persistence once with OK/reopen and discard once with Cancel/reopen on the copied profile.

Concrete Steps: from C:\Dev\Vixen reuse the already-built focused tests if source is unchanged, then run the full built suite and a Release x64 solution rebuild because the production change affects Vixen.Core and its application consumers.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64
    git diff --check

Validation and Acceptance: expect no failures or build errors; record actual counts, warnings, and the manual outcomes for each lookup direction. Source-confirmed insertion updates are not a substitute for checking insertion at runtime. Do not claim the new bug fixed if manual removal lookup remains unavailable or if a reverse-only discrepancy remains unexplained.

Update Progress, Surprises & Discoveries, Decision Log, Outcomes & Retrospective, and Artifacts and Notes with new results while preserving historical milestones. When Jira reporting is authorized, reconcile the extended acceptance criteria with the final implementation and add a concise user-facing validation comment using the project Jira skill. Read the issue back; workflow transitions are not included.

STOP BOUNDARY: STOP HERE for manual review and commit execution before proceeding. Stop execution, run git status --short and scoped git diff, invoke .agents/skills/commit-msg/SKILL.md with VIX-4004 for any repository changes, output the complete Commit message, and wait for explicit human review. Do not create a commit unless explicitly authorized.

## Validation and Acceptance


The observable contract is exact agreement between highlighted outputs, SelectedTreeNodes, exported logical output indexes, and the UI count after a completed user selection event. Unmodified click replaces selection with one output. Empty-space click clears it. Repeated find replaces previous results and does not select the saved top-visible output. Ctrl and Shift retain normal existing multi-selection behavior. Collapsed branches retain their logical results and restore correct highlights on re-expansion.

Automated coverage must expose the production rebuild order, not just assert the logical HashSet. Manual mouse acceptance is required even when keyboard and selection-helper tests pass.

Follow-up acceptance: immediately after deletion or insertion, finding a surviving patched element's outputs must select their current positions with the right counts. Freshly selecting those outputs and finding their elements must resolve their surviving sources. Removed adapters must not resolve, new output adapters must register normally, and survivors must keep their output and adapter identities. Each removed index before a survivor subtracts one from its old position; insertion adds the inserted count for survivors at or after its insertion point. Exercise direct and paged outputs and mixed mutations within one session.

## Idempotence and Recovery


Repeated find and population calls must produce the same result without accumulating highlights or selection entries. Use exception-safe projection guards and update scopes. Re-read files after user edits and preserve unrelated changes. To recover from an unsuccessful local implementation, inspect and reverse only this issue's scoped edits; never reset the entire working tree. Do not save profile experiments to the user's original configuration.

For the follow-up, repeat ReIndexOutputs and verify no additional index shift or duplicate registration occurs. Tests replacing VixenSystem managers must always restore previous static values and must not overlap other test collections. Production repair must update existing map entries rather than reconstruct surviving adapters. Preserve the user's pre-existing plan edits, which already recorded Milestone 3 completion when this revision began.

## Artifacts and Notes


Analysis command: `git status --short` returned no changes before plan creation. The Jira issue has no attachments or comments. Source inspection and user reproduction details are recorded above. No runtime debugger session, build, or test run was performed during planning.

Follow-up analysis evidence (2026-10-01): before implementation, `git status --short` showed only this plan modified; source inspection confirmed `RemoveOutputs -> ReIndexOutputs` changed only `CommandOutput.Index`, while manager lookup returned its cached tuple. `InsertOutputsAt` already updated shifted survivors. Milestone 4 implementation evidence (2026-10-01): `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` completed successfully. `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~OutputControllerOutputIndexTests|FullyQualifiedName~ControllerTreeVirtualizationTests"` passed 34/34. Running the new suite with the manager update removed produced four expected stale-index failures; restoring the production update returned the suite to green. `git diff --check` passed. Rider file diagnostics could not run because the configured Gortex connection has no C# LSP provider. No Jira write was performed. Final Milestone 5 evidence reported by the user (2026-10-01): full build succeeds, all 1,029 unit tests pass, and manual scenarios pass for removal lookup, insertion lookup, reverse lookup after add/remove, the paged boundary, collapsed/expanded branches, an unaffected controller, normal selection behavior, persistence after OK/reopen, and discard after Cancel/reopen.

## Interfaces and Dependencies


Preserve existing public ControllerTree population and selection signatures. Use existing ClearSelectedNodes, AddSelectedNode, logical identity records, and materialized-leaf enumeration. No persistence change, project addition, NuGet package, Catel binding, or async behavior change is required. Internal test access may be expanded only enough to invoke production sequencing. Read the applicable project skills before implementation.

Follow-up public API contract: keep void OutputController.ReIndexOutputs() and all removal/insertion signatures unchanged. Its implementation will additionally synchronize existing output-adapter lookup entries through bool OutputControllerManager.UpdateControllerOutputIndex(IDataFlowComponent component, IControllerDevice controller, int outputIndex). Preserve that method's existing update-only behavior and return contract; do not create missing registrations inside ReIndexOutputs. Add XML documentation for ReIndexOutputs in the same source change. Use existing xUnit/Moq dependencies and the repository's nonparallel test collection convention; no new package or production abstraction is required.

## Revision Notes


2026-10-01 / Codex: Created the plan after the user confirmed standard single-click replacement behavior, first-find success on a collapsed controller, later-find failure until reopening Display Setup, and failed empty-space clearing. Scoped implementation to selection projection and selection-neutral scroll lookup, with production-path regression and manual acceptance.

Analysis complete and plan integrated with plans.md.

2026-10-01 22:12Z / Codex: Appended Milestones 4 and 5 after the user reported incorrect patch lookup positions following output removal. Identified a stale manager index tuple because ReIndexOutputs renumbers only output objects, and prescribed update of existing cache entries with stable adapter identity. Added removal, insertion, mixed-operation, and both-direction validation while preserving completed Milestones 1 through 3 and the user's uncommitted completion records. Insertion remains an unconfirmed runtime concern; its current source already updates shifted entries.

Plan revision complete and recorded in the Decision Log. Ready for coding model to implement the next milestone.

2026-10-01 / Codex: Completed Milestone 4 after the user pointed out its prescribed tests were still missing. Added real OutputController mutation tests and a nonparallel fixture, captured four fail-before stale-index failures against the original loop, restored and verified the repair with 34/34 focused tests, and recorded the unavailable Rider C# diagnostics. Milestone 5 remains the next step for manual Display Setup lookup validation.

2026-10-01 / Codex: Completed Milestone 5 after the user reported all remaining manual scenarios pass, including direct and paged lookups, collapsed/expanded branches, insertion and reverse lookup after add/remove, unaffected controller behavior, selection, and persistence/discard. Recorded the reported full build and 1,029 passing unit tests.

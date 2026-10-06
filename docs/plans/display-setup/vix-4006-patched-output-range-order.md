# Show selected output bounds and patch in controller order (VIX-4006)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with `.agents/PLANS.md`. Issue: https://vixenlights.atlassian.net/browse/VIX-4006. This plan was produced using `.agents/skills/analyze-and-plan-issue/SKILL.md` and records design only; application implementation has not begun.

## Purpose / Big Picture


In Display Setup, selecting controller outputs manually or using Find Patched Outputs must display the same selected range. A selection containing outputs 1639 through 1704 must show First Output as controller #1639 and Last Output as controller #1704, including when the discovery visits the outputs backwards or in a custom element order. Reverse Output Order must change only the order used to connect patch points, while these labels continue to describe the normal selected range.

When patching, destinations must follow the controller pane's controller order and ascending output number within each controller. Reverse Output Order reverses that complete destination sequence. Preferred element patching order and Reverse Element Order continue to control the source sequence independently. This prevents discovery order from accidentally changing either displayed bounds or subsequent patch connections.

## Progress


- [x] (2026-10-06) Read VIX-4006, both comments, attachment metadata, the project analysis and Jira skills, and `.agents/PLANS.md`.
- [x] (2026-10-06) Read current selection, discovery, summary, patching, controller virtualization tests, and output-index tests; checked the VIX-3955 and VIX-4004 reference plans.
- [x] (2026-10-06) User confirmed that reverse must not change the displayed bounds and destinations must follow the selected controller range order.
- [x] (2026-10-06) Compared source before 51ca0cca67aadffb795c8d34dcf63a64ebe05dcd with the logical-selection implementation and VIX-4004 fixes; identified the loss of implicit tree sorting and the older reverse-label behavior.
- [x] (2026-10-06 15:41Z) Prepared this implementation plan and recorded its test, review, and Jira reporting boundaries.
- [x] (2026-10-06) Milestone 1: Updated VIX-4006 with user-facing Summary, Scope, and Acceptance Criteria; read it back and confirmed the issue context was preserved.
- [x] (2026-10-06) Milestone 2: Added failing consumer regressions, normalized destination order, captured normal endpoints before reversal, and passed focused plus existing selection/index tests.
- [x] (2026-10-06) Milestone 3: Passed full-suite and Release x64 solution validation; user confirmed manual label/find behavior and actual patch connections for straight-through, Reverse Element Order, reversed custom-order, and zig-zag custom-order setups. Updated VIX-4006 with final validation and patching evidence, preserving issue status and description.

## Surprises & Discoveries


Observation: the supplied starting commit is a planning commit for VIX-3955, not VIX-3995. It cannot directly introduce runtime behavior because it changes only a Markdown document.

Evidence:

    51ca0cca6 VIX-3955 Initial plan and spec for improvement
    docs/plans/display-setup/display-setup-ok-performance.md | 454 insertions

Observation: before that work, SetupControllersSimple.BuildSelectedControllersAndOutputs iterated ControllerTree.SelectedTreeNodes. MultiSelectTreeview sorted those nodes with TreeNodeSorter, which compares sibling indexes and traverses the tree in display order across branches. The consumer still stored indexes in HashSet<int> and did not explicitly sort them. Tree sorting supplied normal insertion order in practice, but a set's enumeration order was never a reliable ordering contract.

Observation: 986a788812d92fe6f138646e27b259995b0158fc, titled "VIX-3955 Virtualize Display Setup outputs", replaced that export path with ControllerTree.GetSelectedControllerOutputs. The controller sequence still comes from treeview.Nodes, but each output collection comes from the logical _selectedOutputs set. Find Patched Outputs traverses a Stack<IDataFlowComponent>, so discovery commonly visits destinations in reverse order and custom element arrangements can yield an arbitrary permutation. The patching consumer inherits that order through further ToHashSet calls.

Evidence:

    Before: foreach (TreeNode node in controllerTree.SelectedTreeNodes)
    After:  foreach (var (controller, outputIndexes) in controllerTree.GetSelectedControllerOutputs())
            result[controller] = outputIndexes.ToHashSet();

Observation: f8072ef3610c8a7b46acaa52b76f5860c8b0d102, the VIX-4004 selection repair, clears and restores materialized highlights and makes saved scroll-node lookup selection-neutral. It does not change GetSelectedControllerOutputs or SetupPatchingSimple. Its later eab40b924 output-removal repair synchronizes adapter indexes in OutputController and also does not change output ordering. Visual selection restoration can affect observed enumeration as selections are recaptured, but the consumer still lacks an explicit sort. The history therefore establishes the ordering defect in the VIX-3955 export change, rather than a new sort regression in VIX-4004.

Observation: Reverse Output Order has historically changed the labels. The entire SetupPatchingSimple.cs file is identical between the parent of 51ca0cca67aadffb795c8d34dcf63a64ebe05dcd and analysis HEAD a02b5ff2d745099dade4c80ba6287b98d846a666. Both versions reverse _controllerInputs before reading its First and Last entries. This is direct evidence contrary to reverse-independent labels, and the user has explicitly requested the corrected behavior.

Evidence:

    if (checkBoxReverseOutputOrder.Checked)
        _controllerInputs.Reverse();
    if (skipStats) return;
    ...getOutputDetailsForDataFlowComponent(_controllerInputs.First().Item, ...);
    ...getOutputDetailsForDataFlowComponent(_controllerInputs.Last().Item, ...);

Observation: the same list feeds _selectedPatchDestinations. buttonDoPatching_Click rebuilds it with _updateControllerDetails(_cachedControllersAndOutputs, true) immediately before making connections. Fixing only the labels or sorting only a UI export would leave that preparation path vulnerable. The fix must sort in the consuming method and preserve reversal even when statistics are skipped.

Observation: the Reverse Output Order checkbox handler refreshes controller inputs but does not refresh the summary's filtered destination list. The actual patch click rebuilds both inputs and eligible destinations. Tests therefore assert the private input list immediately after checkbox changes and exercise the filtered summary after calling the public details refresh, while the skipStats case asserts the actual patch-preparation input list.

Observation: Rider findTests reported no existing tests for SetupPatchingSimple._updateControllerDetails. The test project already references Vixen.Application and uses xUnit, Moq, and Xunit.StaFact. Existing OutputControllerOutputIndexTests demonstrate real in-memory output registration and exception-safe restoration of VixenSystem.OutputControllers and VixenSystem.DataFlow.

Observation: three screenshots are attached (vixen1.jpg, vixen2.png, vixen3.jpg). Their metadata was reviewed, but anonymous downloads of attachments 18203, 18204, and 18205 returned HTTP 403. No screenshot content is claimed as inspected; the issue text, user clarification, source, and history establish the design without them.

Observation: manual verification of the custom zig-zag patch-order setup exposed a separate zig-zag setup defect. The user filed it as VIX-4007. It is unrelated to selected-output normalization and remains outside VIX-4006 scope.

## Decision Log


Decision: normalize output order in SetupPatchingSimple._updateControllerDetails, rather than change the selection data model or graph traversal.
Rationale: ControllersAndOutputsSet is intentionally a dictionary of sets. Ordering is required when the set becomes a patching sequence. A consumer sort covers manual selection, find results, details refresh, and skipStats preparation without depending on HashSet behavior or materialized TreeNodes.
Date/Author: 2026-10-06 / Codex.

Decision: preserve controller enumeration from the selected-controller dictionary and sort only indexes within each controller.
Rationale: production constructs that dictionary from GetSelectedControllerOutputs, which iterates controller roots in their current pane order, including unsaved reorder operations. Sorting by name, GUID, or persisted manager order would alter that contract. Numeric output indexes from different controllers do not form one comparable global range.
Date/Author: 2026-10-06 / Codex.

Decision: capture normal ordered endpoints before applying reverse and use those captured endpoints for labels.
Rationale: the user expressly requires labels to describe the selected range independent of reverse. For multiple controllers, First is the lowest selected output of the first nonempty controller in pane order; Last is the highest selected output of the last nonempty controller. A single-controller selection is its numerical minimum and maximum.
Date/Author: 2026-10-06 / User and Codex.

Decision: continue reversing the entire flattened destination list, then apply the existing All Outputs/Unpatched Outputs Only filter.
Rationale: this retains the existing cross-controller reversal and eligibility behavior while fixing its normal base order. It does not change preferred element ordering, source reversal, existing connections merely through selection, or graph discovery.
Date/Author: 2026-10-06 / Codex.

Decision: use the existing public update methods and narrowly scoped reflection in tests rather than add production public or internal test APIs.
Rationale: hidden WinForms controls, their named child labels/check boxes, and the actual private destination lists can be exercised directly. This verifies the production consumer and its skipStats route without a second ordering implementation, a new abstraction, or automation of modal confirmation dialogs.
Date/Author: 2026-10-06 / Codex.

Decision: retain the validated VIX-4004 projection, collapse, scrolling, and output-index synchronization behavior.
Rationale: those repairs concern selection membership and identity. Explicit consumer ordering can repair VIX-4006 without undoing the performance or selection architecture.
Date/Author: 2026-10-06 / Codex.

## Outcomes & Retrospective


Milestone 2 implementation and focused validation are complete. The regression suite failed before the production repair in four ordering/bounds cases (4 failed, 1 passed), then passed after the repair (6 passed). Full-MSBuild test-target builds succeeded, and the existing controller virtualization/output-index suites passed (34 tests). During Milestone 3, the user reported a successful full manual build and all 1,045 unit tests passing; the agent independently reran the full test suite (1,045 passed, 0 failed, 0 skipped) and a Release x64 full-solution rebuild (0 errors, 73 warnings). The user manually verified First/Last labels during manual selection and Find Elements for normally patched and custom-order props. They also verified actual patching with straight-through order, Reverse Element Order, and custom order set through the order property in reversed and zig-zagged arrangements. Automated coverage includes multi-controller ordering, the 5,001-output paging boundary, filtering/reverse behavior, and VIX-4004 controller virtualization/output-index regressions. A separate zig-zag setup defect found during manual testing was filed as VIX-4007 and was kept out of scope. Milestone 3 acceptance is complete.

## Architecture Design: VIX-4006 — Patched outputs showing incorrectly


Detected IDE Environment: Rider with automation hooks. Rider findTests was used and found no existing coverage of the affected method. During implementation, navigate to the named file and use small Apply snippet from chat replacements for the loop, endpoint capture, label reads, and XML comments. An agent may use the permitted guarded repository editing workflow.

Core Strategy: make ordering explicit at the existing WinForms consumer. Apply `.agents/skills/dotnet-best-practices/SKILL.md` for focused C# changes and disposable test fixtures, and `.agents/skills/csharp-docs/SKILL.md` for the changed public update contracts and new test APIs. This is legacy WinForms maintenance; no Catel/WPF binding, interface boundary, lifecycle, or async redesign is involved.

Data Model & Property Contracts: retain ControllersAndOutputsSet : Dictionary<IControllerDevice, HashSet<int>>, PatchStatusItem<IDataFlowComponent>, and all existing method signatures. _controllerInputs remains the destination sequence used for patching, including reversal. Display endpoints are method-local references captured before reversal. No persisted field, package, project, new production class, or new production API is required.

Mathematical / Boundary Logic: for each controller c in pane order, let S(c) be its valid selected zero-based output indexes. The normal sequence is the concatenation of ascending S(c). Labels use the first and last entries of this normal sequence and display outputIndex + 1. The effective patching sequence is normal when reverse is unchecked and the reversal of normal when checked. Filtering unpatched destinations preserves relative order. Gaps are not filled. An empty sequence clears both labels; a single output appears in both labels. Existing upper-bound rejection remains in place before adapter creation. The tree already excludes negative and out-of-range indexes; no unrelated input-validation redesign is needed.

For example, custom discovery [1700, 1638, 1703, 1650] must produce normal zero-based destinations [1638, 1650, 1700, 1703], labels #1639 and #1704, and reversed destinations [1703, 1700, 1650, 1638] with the same labels. With controllers A then B in the pane, A:[5,1] and B:[3,0] become A1,A5,B0,B3 normally and B3,B0,A5,A1 in reverse. Labels are A #2 and B #4 in both modes.

Subsystem Component Matrix:

| Component | Responsibility and planned change |
| --- | --- |
| src/Vixen.Application/Setup/SetupPatchingSimple.cs | Sort selected indexes per controller, capture normal endpoints, keep reverse in destination preparation, and document the public update contract. |
| src/Vixen.Tests/Setup/SetupPatchingSimpleOutputOrderTests.cs (new) | Exercise real registered adapters and the hidden control's public updates, label text, destination order, reverse toggle, filtering, and skipStats preparation. |
| src/Vixen.Tests/Setup/SetupPatchingTestOutputModuleConsumer.cs (new) | Supply an isolated internal module-consumer stub for real in-memory OutputController construction, following the existing output-index test precedent. |
| src/Vixen.Common/Controls/ControllerTree.cs | Read/validation target for logical selection and controller pane order; no production change planned. |
| src/Vixen.Application/Setup/SetupControllersSimple.cs and SetupElementsTree.cs | Read/validation targets for export and find discovery; no production change planned. |
| src/Vixen.Tests/Common/ControllerTreeVirtualizationTests.cs and src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs | Existing VIX-3955/VIX-4004 regressions to rerun; no edits planned. |

## Context and Orientation


Display Setup is a WinForms dialog even though the application also uses WPF. SetupElementsTree.buttonSelectDestinationOutputs_Click follows element-to-filter-to-controller connections in VixenSystem.DataFlow. A CommandOutputDataFlowAdapter is the data-flow component representing one controller output. The output manager maps that adapter back to its controller and zero-based output index. The find operation collects those indexes in sets and calls DisplaySetup.SelectControllersAndOutputs.

SetupControllersSimple assigns that selection to ControllerTree, whose logical selection retains output identities even when collapsed output pages have no TreeNodes. GetSelectedControllerOutputs exports selected controllers in current root order and their index sets. BuildSelectedControllersAndOutputs wraps the export in the application-owned ControllersAndOutputsSet. DisplaySetup's selection event forwards the result to SetupPatchingSimple.UpdateControllerSelection.

SetupPatchingSimple._updateControllerDetails builds _controllerInputs containing adapter references and their patched/unpatched flags. It computes selection counts and First/Last labels. _updatePatchingSummary chooses all destinations or only unpatched ones. buttonDoPatching_Click separately builds the ordered source list, rebuilds destination inputs with skipStats=true, applies eligibility filtering, and connects matching source/destination positions. Sorting at the shared destination-building point is therefore necessary for both the displayed summary and actual patching.

Read these primary references before implementation: docs/plans/display-setup/display-setup-ok-performance.md and docs/plans/display-setup/vix-4004-controller-output-selection.md. Their final contracts are 5,000-output pages, eviction on collapse, logical hidden-output selection, replacement finds, selection-neutral scroll restoration, and synchronized adapter indexes after removal. Some older sections retain superseded page sizes or validation commands. Use current source, final recorded decisions, and the full-MSBuild test workflow below.

## Plan of Work


Perform the three milestones below in order. The analysis request authorizes this local plan; Jira writes and application implementation are future execution actions. The project analysis skill requires explicit review stops between milestones, and AGENTS.md forbids creating commits without an explicit request. Generate milestone commit messages with the project commit-msg skill, but do not create commits automatically.

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Milestone 1: Record the agreed behavior and acceptance criteria in Jira


Context: VIX-4006 currently describes reversed/inaccurate First/Last labels after finding patch points, says the show works, and notes custom patching order. Preserve that original report and both comments. This design also contains the user's explicit requirements for reverse-independent labels and correctly ordered destinations.

Plan of Work: when implementation or issue editing is authorized, use `.agents/skills/jira/SKILL.md` to revise the issue description with Summary, Scope, and Acceptance Criteria. Describe matching labels for manual/find selections, lowest/highest bounds on one controller, stable labels when reverse toggles, ascending destinations in pane order, intentional reverse patching, and unchanged custom element order. Include a short test scenario using 1639–1704, an irregular selection, and reverse. Keep formulas, file names, internal types, and test-fixture details in this local plan. Preserve status, versions, attachments, and comments. Read the issue back after the write.

Concrete Steps: from C:\Dev\Vixen run git status --short, then use the Jira connector's structured issue update for VIX-4006 on vixenlights.atlassian.net. No source edits are made in this milestone.

Validation and Acceptance: the issue describes the agreed user outcomes and a reproducible test scenario, without claiming that implementation or validation is complete.

STOP HERE for manual review and commit execution before proceeding.

1. Halt execution and do not start the next milestone.
2. Run git status --short and git diff for this milestone's repository changes.
3. If repository files changed, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4006 as the subject prefix.
4. Output the skill's paste-ready Commit message block; do not create a commit without an explicit request.
5. Wait for explicit user confirmation before advancing.

### Milestone 2: Prove and repair destination ordering and label independence


Context: production edits belong only in C:\Dev\Vixen\src\Vixen.Application\Setup\SetupPatchingSimple.cs. New regression files belong in C:\Dev\Vixen\src\Vixen.Tests\Setup\. Read current files, inspect git status, and apply the project dotnet-best-practices and csharp-docs skills before coding.

Plan of Work: first add SetupPatchingSimpleOutputOrderTests.cs using the existing OutputControllerOutputIndexTestCollection.Name collection, which disables parallel execution with other global-state tests. Use [StaFact]/[StaTheory] for hidden WinForms controls; STA means a Windows UI thread initialized to support these controls. Dispose controls on their creating thread. Save VixenSystem.OutputControllers and DataFlow before installing fresh managers, restore both on disposal, and restore them in a constructor catch if setup fails. Never initialize the user's running application, hardware, or persisted profile.

Build real OutputController instances with the OutputMediator, mocked hardware/module/data-policy factory, and AddOutput registration pattern in src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs. Put the small module-consumer implementation in its own new internal sealed SetupPatchingTestOutputModuleConsumer class/file rather than editing or extracting the existing fixture. Read its interface before implementing its members. Construct controllers with distinct names and real CommandOutput objects so manager lookups can verify each adapter's owning controller and output index.

Instantiate SetupPatchingSimple without showing a form. Call UpdateControllerSelection and UpdateControllerDetails; read labelFirstOutput, labelLastOutput, labelOutputCount, checkBoxReverseOutputOrder, and the All Outputs/Unpatched Outputs Only radio buttons with Controls.Find(name, true). Assert exactly one matching control with the expected type. Read _controllerInputs and _selectedPatchDestinations through narrow private-field reflection and resolve every adapter through the real manager. For patch-click preparation, invoke the actual _updateControllerDetails(selection, true) method through narrow method reflection, then assert its destination order. Do not copy the algorithm into tests or invoke buttonDoPatching_Click unattended because it opens modal dialogs. Actual connection acceptance belongs in Milestone 3.

Add cases for descending selection of zero-based 1638–1703; an irregular insertion sequence [1700,1638,1703,1650] whose endpoints are not its extrema; ascending manual-like input; one output; empty selection after a previous nonempty selection; and two controllers in a known pane-derived order. A paging-boundary integration case should construct a real ControllerTree with its existing supplied-controller seam, project [5000,4999,0] on a 5,001-output controller, export its logical selection through ToHashSet as production does, and pass it to the patching control. No new tree seam or new production export API is necessary.

For each nonempty selection, assert exact normal adapter order, First/Last text, and counts. Toggle reverse on and off through the actual check box and assert that destination order reverses/restores while labels and counts stay fixed. Invoke the skipStats path in both reverse states and confirm the same effective destination order with labels unchanged. Give one interior output a real patch source, change the destination radio selection to Unpatched Outputs Only, and verify filtering retains relative order and labels still describe the full selection. Assert that updates and reverse toggles alone preserve every existing Source reference. Empty and singleton cases must remain safe in both reverse states.

Build and run these tests before changing production. Capture failures for unordered normal destinations and reverse-changing labels; ascending and empty characterization cases may already pass. Then change the inner loop in _updateControllerDetails to enumerate controllersAndOutput.Value.OrderBy(outputIndex => outputIndex). Retain outer controller order, existing upper-bound rejection, counts, patch-state classification, and adapter identity.

Immediately after building the normal _controllerInputs list, capture its first and last PatchStatusItem references with FirstOrDefault and LastOrDefault into method-local variables. Then retain the existing _controllerInputs.Reverse operation and its placement before the skipStats early return. In the statistics block, clear both labels as today, and when captured endpoints exist, perform the existing output-manager lookup/formatting using those captured items rather than _controllerInputs.First()/Last(). Because only the list order is reversed, the captured item references remain the normal selected bounds. Add a short explanatory comment at endpoint capture describing why display bounds must be independent of reverse patch order.

Document the existing public UpdateControllerSelection and UpdateControllerDetails methods with summary, parameter, and remarks explaining that output indexes are normalized within the supplied controller order, selected bounds describe normal order, and reverse affects patch destinations only. Keep signatures unchanged. Document new public test types/methods and stub interface implementations as required by csharp-docs. Do not clean up unrelated public members, relocate PatchStatusItem, reformat the designer, replace HashSet types, or alter discovery/projection behavior.

Concrete Steps: run the following from C:\Dev\Vixen before and after the production repair. Build with full MSBuild because QMLibrary and LiquidFunWrapper are C++/CLI dependencies. Start long-running commands with exec_command and yield_time_ms=10000; when only waiting for completion use write_stdin with yield_time_ms at least 30000.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter FullyQualifiedName~SetupPatchingSimpleOutputOrderTests
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~ControllerTreeVirtualizationTests|FullyQualifiedName~OutputControllerOutputIndexTests"
    git diff --check

Expected after the repair: MSBuild succeeds with zero errors; new focused tests and existing selection/index regressions report zero failures; whitespace validation emits no errors. Record actual pass counts and warnings rather than inventing them.

Validation and Acceptance: use Gortex impact before edits and detect, tests, guards, and contract assessment after edits. Verify any changed signature if implementation departs from this signature-preserving design. Run Rider get_file_problems on each changed C# file and address diagnostics only on added/changed lines. Report an unavailable diagnostic service accurately. The destination tests must exercise the actual consumer and skipStats branch; a sorted-label-only test is insufficient.

STOP HERE for manual review and commit execution before proceeding.

1. Halt execution and do not start the next milestone.
2. Run git status --short and scoped git diff for production, test, and plan changes.
3. Invoke `.agents/skills/commit-msg/SKILL.md` using VIX-4006 as the subject prefix.
4. Output its complete paste-ready Commit message block; do not create a commit without an explicit request.
5. Wait for explicit user confirmation before advancing.

### Milestone 3: Verify actual patch connections and record final results


Context: use a copy of a profile with distinguishable elements, preferred custom element order, a direct-output controller, and a controller above the 5,000-output page boundary. Selection and patching use global application state; manual experiments must not save to the user's original profile.

Plan of Work: manually select outputs 1639–1704, then find the same outputs from normally patched, reversed, and custom-order props. In every case labels must show the same minimum/maximum and count. Toggle Reverse Output Order; labels and selection remain fixed. Repeat with a noncontiguous set whose discovery starts and ends inside its numerical bounds, a singleton, and an empty selection. Check repeated finds on already expanded branches, collapse/re-expansion, and outputs across the 5,000-output boundary. A normal click after a find must still replace selection and an empty-space click must clear it, preserving VIX-4004.

Create a small copied-profile patching scenario with four distinguishable element sources and four chosen controller outputs discovered in an irregular order. With source order fixed and reverse unchecked, patch and inspect graphical connections or use Find Patched Elements to prove that successive sources reach ascending selected destinations. Unpatch only that test selection, toggle reverse, patch again, and prove that successive sources reach descending selected destinations while labels remain lowest/highest. Repeat with Unpatched Outputs Only and an interior destination already connected; verify that existing connection survives and eligible destinations keep their effective relative order. Test preferred element patching order and Reverse Element Order independently so source ordering remains effective.

With two controllers, confirm destinations follow their current pane order normally and the complete sequence reverses when requested. Reorder controllers within the open dialog before patching and verify that the new pane order is used even before saving. Verify selected bounds are the first controller's lowest selected output and the last controller's highest selected output in both reverse states. Recheck finding outputs after a removal/insertion using the VIX-4004 scenarios; labels must reflect current indexes without changing surviving connections.

Concrete Steps: from C:\Dev\Vixen run the full built test suite and Release x64 solution rebuild after focused validation succeeds. If no source changed, do not repeat focused tests merely to produce another transcript.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64
    git diff --check

Validation and Acceptance: all tests pass and the rebuild has zero errors. Record actual counts and warnings. Record manual evidence for normal/reverse connections, stable labels, irregular selection, custom source order, two-controller order, page-boundary selection, and VIX-4004 regressions. Do not claim runtime completion if actual patching acceptance cannot be performed.

Update the plan's living sections with implementation and validation evidence. When Jira reporting is authorized, reconcile the final user-facing description/acceptance criteria and add a concise validation comment with the Jira skill, preserving original report context and issue state. Read it back. No workflow transition is included.

STOP HERE for manual review and commit execution before proceeding.

1. Halt execution; all required implementation and reporting should now be complete.
2. Run git status --short and scoped git diff for this milestone's changes.
3. If repository files changed, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4006 as the subject prefix.
4. Output its complete paste-ready Commit message block; do not create a commit without an explicit request.
5. Wait for explicit user review before any additional work.

## Concrete Steps


The executable commands and their working directory are given in each milestone. To reproduce the historical evidence without changing branches, run these read-only commands from C:\Dev\Vixen:

    git show --stat --oneline 51ca0cca67aadffb795c8d34dcf63a64ebe05dcd
    git show '51ca0cca67aadffb795c8d34dcf63a64ebe05dcd^:src/Vixen.Application/Setup/SetupControllersSimple.cs'
    git show '51ca0cca67aadffb795c8d34dcf63a64ebe05dcd^:src/Vixen.Common/Controls/MultiSelectTreeview.cs'
    git show 986a788812d92fe6f138646e27b259995b0158fc -- src/Vixen.Application/Setup/SetupControllersSimple.cs src/Vixen.Common/Controls/ControllerTree.cs
    git show f8072ef3610c8a7b46acaa52b76f5860c8b0d102 -- src/Vixen.Common/Controls/ControllerTree.cs
    git diff '51ca0cca67aadffb795c8d34dcf63a64ebe05dcd^' a02b5ff2d745099dade4c80ba6287b98d846a666 -- src/Vixen.Application/Setup/SetupPatchingSimple.cs

The final comparison emits no diff, proving the old reverse-label behavior was retained rather than newly introduced by these commits. No checkout, reset, or historical worktree mutation is required.

## Validation and Acceptance


First/Last always describe the normal selection, including discovery in any permutation and both reverse states. Normal patching uses ascending output indexes within current controller pane order; reverse flips that entire sequence. Eligibility filtering removes outputs without shuffling survivors. Selection changes do not alter existing connections. Preferred element order and reverse source order remain independent. Direct and paged selection, repeated find, clearing, collapse, and output-index lookup continue to satisfy existing tests and manual acceptance.

The regression suite must fail before the fix for unsorted destinations and reverse-dependent labels and pass afterwards. Actual graphical/lookup inspection must prove connection order in both modes. Build success alone is insufficient. Commands, counts, manual evidence, and unavailable checks belong in this plan and the implementation handoff.

## Idempotence and Recovery


Repeated updates and toggles must rebuild from the same selection, avoiding cumulative reversals, set mutation, connection changes, or duplicate registrations. Sort through enumeration rather than modifying the supplied sets. Test fixtures must restore saved VixenSystem managers and dispose all controls even after failure; use the existing nonparallel collection. Retrying a test must not depend on saved profiles or loaded hardware.

Re-read status and files before editing. Preserve unrelated changes. If implementation fails, revert only this issue's reviewed edits; never reset the workspace or undo VIX-4004. Keep manual experiments confined to a copied profile and discard or save only that copy deliberately.

## Artifacts and Notes


Analysis started with git status --short reporting a clean worktree. Historical git show/log/diff commands succeeded. The supplied commit is docs-only; 986a78881 introduces logical selection export; f8072ef36 leaves ordering unchanged; eab40b924 repairs removal indexes. The baseline-to-analysis-HEAD comparison of SetupPatchingSimple.cs is empty. Rider findTests returned: "There are no existing tests for VixenApplication.Setup.SetupPatchingSimple._updateControllerDetails". Attachment downloads returned 403 and were not used as visual evidence.

Gortex impact of _updateControllerDetails identified public selection/detail updates, reverse checkbox changes, and buttonDoPatching_Click as direct callers, with DisplaySetup event consumers beyond them. The graph found no existing test file for this method. An impact query for this new Markdown plan returned file_not_indexed because it does not yet exist as an indexed code file.

Planning validation confirmed every required section, three milestones and three explicit review stops, no nested/outer Markdown fences in the standalone file, and the required concluding statement. Milestone 1 updated VIX-4006 with the accepted behavior and verified preserved issue context. For Milestone 2, the initial full-MSBuild test-target build succeeded after importing the existing test collection namespace; the pre-fix focused suite failed in four cases (4 failed, 1 passed). After implementation, `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` succeeded. The focused suite passed 6/6 and the existing `ControllerTreeVirtualizationTests` plus `OutputControllerOutputIndexTests` passed 34/34. Rider file-problem checks reported zero errors in all three changed C# files; the broader lint output included existing diagnostics outside changed lines. `git diff --check` emitted no whitespace errors. In Milestone 3, `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/` passed all 1,045 tests, and `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64` completed with 0 errors and 73 warnings. The user manually verified First/Last labels with manual selection and Find Elements for normally patched and custom-order props, plus actual straight-through, Reverse Element Order, reversed custom-order, and zig-zag custom-order patching. Automated tests cover multi-controller destination order, selection across the 5,000-output paging boundary, filtering/reverse behavior, and VIX-4004 controller virtualization/output-index regressions. The separate zig-zag setup defect was filed as VIX-4007. Final Jira validation and patching evidence was appended without changing VIX-4006's status or description.

## Interfaces and Dependencies


Keep these production signatures unchanged:

    public void SetupPatchingSimple.UpdateControllerSelection(ControllersAndOutputsSet controllersAndOutputs)
    public void SetupPatchingSimple.UpdateControllerDetails(ControllersAndOutputsSet controllersAndOutputs)
    private void SetupPatchingSimple._updateControllerDetails(ControllersAndOutputsSet controllersAndOutputs, bool skipStats = false)

Use existing LINQ OrderBy/FirstOrDefault/LastOrDefault, the current output-manager lookup, existing xUnit/Moq/Xunit.StaFact dependencies, and the existing OutputControllerOutputIndexTestCollection. No persisted data, configuration migration, new NuGet dependency, solution project, async workflow, service, or production helper abstraction is required.

## Revision Notes


2026-10-06 / Codex: Created the design after the user resolved reverse-label and destination-order requirements and requested comparison with pre-51ca0cca history. Recorded VIX-3955 attribution, the logical-export ordering regression, older reverse-label behavior, and the separation between these concerns and VIX-4004's validated selection/index fixes. Scoped implementation to the destination consumer and regression files.

2026-10-06 / Codex: Completed Milestone 3 after the user confirmed actual straight-through, Reverse Element Order, reversed custom-order, and zig-zag custom-order patching, in addition to range-label checks. Recorded the independent zig-zag setup bug under VIX-4007, appended final validation and patching results to VIX-4006 without changing its status, and documented the 1,045-test suite and Release x64 rebuild results.

Analysis complete and plan integrated with plans.md.

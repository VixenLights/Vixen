# Restore element range selection direction when patching (VIX-4009)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with `.agents/PLANS.md`. Issue: https://vixenlights.atlassian.net/browse/VIX-4009. Related issue: https://vixenlights.atlassian.net/browse/VIX-938. This document records an implementation-ready design; application implementation has not begun.

## Purpose / Big Picture


A user must be able to choose the direction of element patching by selecting a range in that direction. With elements A, B, C displayed top-to-bottom, selecting C and Shift-clicking A must connect C, B, A to ascending controller outputs. Selecting A and Shift-clicking C must connect A, B, C. Keyboard Shift+Up, Shift+Down, Shift+Home, and Shift+End must supply the same anchor-to-endpoint order. The anchor is the original selected row; the endpoint is the row currently reached by the range gesture.

Preserve VIX-938's working keyboard navigation, stable anchor, range shrinking, visible-row traversal, host shortcuts, guarded handle creation, and large-tree performance. Preferred Patching Order and Reverse Element Order retain their existing precedence over the base source sequence. Controller output ordering remains the VIX-4006 behavior. Ordinary clicks and Ctrl selection retain their existing semantics; this issue does not introduce a complete chronological Ctrl-click ordering contract.

## Progress


- [x] (2026-10-07) Read the project analyze-and-plan-issue and Jira skills, `.agents/PLANS.md`, the VIX-4006 reference plan, and relevant source and regression fixtures.
- [x] (2026-10-07) Confirmed the historical event-before-sort behavior in 3.12u6 and the upward-range normalization introduced by 23910a6a after 29eea926.
- [x] (2026-10-07) Verified VIX project creation access and its Bug type, checked related issue summaries, and read the final VIX-938 contract.
- [x] (2026-10-07) Milestone 1: Created VIX-4009 with user-facing requirements and acceptance criteria, linked it to VIX-938 using Relates, and verified the description, Bug type, New Ticket status, and link by reading it back.
- [x] (2026-10-07) Prepared this plan and the scoped design review in `docs/reviews/vix-4009-range-selection-order-design.md`.
- [ ] Milestone 2: Add failing direction regressions, implement the separate range-order snapshot and Display Setup handoff, and pass focused checks.
- [ ] Milestone 3: Verify complete patch behavior, VIX-938 and controller regressions, Release x64 build, and final Jira reporting.

## Surprises & Discoveries


The presence of sorting in the historical tree control did not prove which order patching received. In tag 3.12u6, SelectNode begins a deferred update, adds upward range nodes through PrevVisibleNode, and raises OnAfterSelect before its finally block calls EndUpdate. SetupElementsTree forwards that sequence synchronously, and SetupPatchingSimple.UpdateElementSelection copies it into _cachedElementNodes. EndUpdate sorts the live tree list afterwards but cannot reorder the cached copy. This explains why reverse Shift-click could patch bottom-to-top even though the finished tree list was sorted.

Evidence from 3.12u6, commit 2439a3657f10bcea725c8659dd4102c3a3b409a0, dated December 30, 2025:

    SelectNode: BeginUpdate(); ... ToggleNode(previousVisibleNode, true);
    SelectNode: OnAfterSelect(new TreeViewEventArgs(m_SelectedNode));
    SelectNode finally: EndUpdate();
    UpdateElementSelection: _cachedElementNodes = nodes.ToList();
    buttonDoPatching_Click: GetOrderedElementOutputs(_cachedElementNodes);

Commit 29eea926e8bd2cf8aa59678ff14a9c9ace14e166 on July 22, 2026 introduced anchored mouse and keyboard range handling for VIX-938. Commit 23910a6a1ea211bdc1757ad4f48397350a606dc6 on the same date introduced SelectRangeNode to avoid requesting recursive selected-node sorting for every range member. It also added nodes.Reverse() to NodesInPreviousVisibleRange, making upward ranges top-to-bottom before their notification. That normalization is the direct source of the reported change.

Existing MultiSelectTreeviewKeyboardSelectionTests deliberately assert top-to-bottom SelectedNodes for upward ranges. Retaining those assertions protects other consumers. Restoring range direction by globally removing nodes.Reverse() would change that established shared-control behavior and can affect tree operations that expect display order.

DisplaySetup.ActivatePatchingControl reads ISetupElementsControl.SelectedElements when switching patching views. Fixing only the selection-event payload would therefore lose directional order after a view switch. Both the event and the selected-elements getter must expose the same retained range-order snapshot.

The VIX-4006 reference plan establishes that controller destinations follow pane order and ascending output indexes within each controller. Its source-order controls are independent. The new repair belongs on the element selection handoff and must not undo destination normalization or its reverse-independent bounds.

Historical Gortex commit snapshots remained unavailable during the preceding investigation. Historical evidence was obtained through read-only Git comparisons under the session's performance-fallback instruction; current implementation and test source was inspected with Gortex. Historical observation is source evidence, not a live UI reproduction.

## Decision Log


Decision: retain the existing canonical SelectedNodes list and provide a separate read-only range-order snapshot for consumers that require selection direction.
Rationale: this repairs element patching without changing existing tree order, range membership, controller selection, drag/reverse operations, or LipSync consumers. It avoids reverting the VIX-938 traversal and performance changes.
Date/Author: 2026-10-07 / Codex, based on the user's request.

Decision: derive directional order from the current normalized visible range and its original anchor.
Rationale: the existing traversal already identifies exactly the visible selected rows. A linear copy and optional reversal is sufficient; no new tree walk, TreeNodeSorter invocation, per-node ToggleNode loop, or key-handling redesign is needed.
Date/Author: 2026-10-07 / Codex.

Decision: route both SetupElementsTree selection events and its SelectedElements getter through the new directional projection.
Rationale: events populate the current patching cache, while the getter repopulates it when patching views switch. Both must preserve the same sequence after EndUpdate.
Date/Author: 2026-10-07 / Codex.

Decision: clear the range snapshot on clearing selection or ordinary membership changes; fall back to the existing live selected-node sequence for non-range operations.
Rationale: this prevents stale TreeNode references and prevents a previous range's order from leaking into ordinary clicks, Ctrl toggles, programmatic replacement, or tree rebuilding. It preserves existing non-range notification timing without promising full click chronology.
Date/Author: 2026-10-07 / Codex.

Decision: leave GetOrderedElementOutputs and controller destination logic intact.
Rationale: existing ordered input is sufficient. Preferred order properties, first-occurrence deduplication, leaf expansion, Reverse Element Order, and destination normalization already belong to those consumers and must retain their behavior.
Date/Author: 2026-10-07 / Codex.

Decision: this turn completes issue creation and planning only. Future implementation milestones retain the project skill's manual review stops, and no commits are created without an explicit request.
Rationale: the user requested an execution plan and Jira bug, not application implementation in this turn.
Date/Author: 2026-10-07 / Codex.

## Outcomes & Retrospective


Planning and issue creation are complete. VIX-4009 is a verified Bug in New Ticket status and relates to the closed VIX-938 issue. No application source or test code has changed, and no runtime, build, or automated test result is claimed for this proposed repair. Remaining work is Milestone 2 implementation and Milestone 3 validation/reporting.

## Architecture Design: VIX-4009 — Restore element range selection direction when patching


Detected IDE Environment: Rider with automation hooks. Use Rider navigation and small Apply snippet from chat updates to the named members. An executing agent must use the repository's guarded Gortex edit workflow. Rider get_file_problems is available for changed C# files.

Core Strategy: maintain two explicit views of one selection: existing display order for tree operations and anchor-to-endpoint order for element patching. This is a small addition to existing legacy WinForms controls and the existing selection handoff; it adds no new service, interface hierarchy, command framework, or lifecycle. The project dotnet-best-practices skill governs scoped C# edits, csharp-docs governs added/changed public contracts, and dotnet-design-pattern-review informed the separate-consumer contract recorded in the accompanying review. Catel/WPF and async skills are not needed for these synchronous WinForms paths.

Data Model & Property Contracts: add a private nullable IReadOnlyList<TreeNode> _rangeSelectionOrder field and a public read-only IReadOnlyList<TreeNode> SelectedNodesInSelectionOrder property in MultiSelectTreeview. Store an array-backed read-only snapshot after each successful range gesture. The getter returns that snapshot while a range is active and otherwise exposes the existing selected-node sequence through a read-only view. Empty selection yields an empty sequence. Do not expose an editable directional list. Add ElementTree.SelectedElementNodesInSelectionOrder as an IEnumerable<ElementNode> projection of those nodes' ElementNode tags. Keep existing SelectedNodes, SelectedTreeNodes, and SelectedElementNodes contracts and signatures intact.

SetupElementsTree.SelectedElements retains its existing IEnumerable<ElementNode> getter/setter signature. Change its getter to the new directional ElementTree property, leave the setter's PopulateNodeTree path unchanged, and use the same getter for its selection and deselection event payloads. Document the directional-range and non-range fallback behavior. No serialization, profile field, user setting, new package, project, or configuration migration is required.

Mathematical / Boundary Logic: NodesInVisibleRange already produces a top-to-bottom list R containing the anchor and endpoint inclusively. Copy R once after range membership is applied. If the first item of R is the anchor, retain the copy; otherwise the anchor must be R's last item and reverse the copy. Store that copy through an array-backed read-only wrapper. A singleton range needs no reversal. Keep the existing normalized R unchanged, including NodesInPreviousVisibleRange's reverse operation. Capture the snapshot after ClearSelectedNodes has invalidated the previous one and before OnAfterSelect can notify the host.

For A, B, C, D, an anchor at C and endpoint A produce patching order C,B,A while SelectedNodes remains A,B,C. Moving the endpoint back to B produces C,B, then reaching C produces C, then moving past the anchor to D produces C,D. Shrinking never changes the anchor; crossing it changes the range's direction. Invalid/unreachable endpoints retain the existing no-change behavior. A newly cleared or replaced selection cannot retain the preceding snapshot. Each new range creates a new snapshot, so an event-time sequence previously copied by a consumer cannot be mutated by a later gesture.

Use the existing visible-node traversal: collapsed children are excluded, expanded descendants appear in row order, and no wrap occurs past either end. Selecting groups changes only the order of selected groups; GetOrderedElementOutputs continues expanding each group's leaves in its existing child order, then removes duplicate leaves at their first occurrence. This issue does not reverse the contents of each group separately. Preferred Patching Order takes precedence when valid, and the existing Reverse Element Order option reverses the resulting flattened leaf source sequence afterwards.

The additional snapshot copy and optional reversal take linear time and storage in the selected range length. Do not introduce additional whole-tree traversal, recursive sorting, or a ToggleNode-based range implementation. Existing deferred-sort flag behavior is outside this repair; do not add or broaden sort requests while implementing the snapshot.

Subsystem Component Matrix:

| File | Responsibility and planned work |
| --- | --- |
| src/Vixen.Common/Controls/MultiSelectTreeview.cs | Retain canonical range order, capture read-only directional snapshot, invalidate it on clear/ordinary membership changes, and document the new property. |
| src/Vixen.Common/Controls/ElementTree.cs | Add the directional ElementNode projection without changing its existing general selection properties. |
| src/Vixen.Application/Setup/SetupElementsTree.cs | Forward directional order in selection events and SelectedElements so patching view switches retain it. |
| src/Vixen.Tests/Common/MultiSelectTreeviewKeyboardSelectionTests.cs | Preserve all existing VIX-938 assertions and add keyboard direction, stable snapshot, and invalidation regressions. |
| src/Vixen.Tests/Common/MultiSelectTreeviewRangeSelectionOrderTests.cs (new) | Cover mouse-equivalent range gestures and canonical-versus-directional order without new public test seams. |
| src/Vixen.Tests/Setup/SetupPatchingSimpleElementOrderTests.cs (new) | Verify the actual selection handoff, cached source order, consumer preparation, preferred order/reverse precedence, and view-switch getter behavior. |
| src/Vixen.Application/Setup/DisplaySetup.cs and SetupPatchingSimple.cs | Read/validation targets for view switching and patch consumption; no production edit expected. |
| src/Vixen.Common/Controls/ControllerTree.cs and existing output-order tests | Regression targets only; preserve controller membership, paging, output normalization, and bounds. |

## Context and Orientation


MultiSelectTreeview is the shared multi-select WinForms tree used by ElementTree, ControllerTree, and LipSync selection. SelectNode(TreeNode, Keys) is the common selection path reached by mouse handling and ProcessKeyboardSelection. SelectRange chooses rows from the original _selectionAnchorNode to the current endpoint and highlights them through SelectRangeNode. Current SelectedNodes exposes its canonical top-to-bottom list. BeginUpdate/EndUpdate batch rendering and can defer selection sorting. The new separate snapshot makes patching order independent of deferred sorting or event timing.

ElementTree associates each TreeNode with an ElementNode through its Tag. SetupElementsTree hosts ElementTree and implements the existing element-control contract. Its treeviewAfterSelect and treeviewDeselected handlers forward ElementNodesEventArgs. DisplaySetup.control_ElementSelectionChanged calls the active patching control's UpdateElementSelection. SetupPatchingSimple immediately copies the sequence into _cachedElementNodes. Its Patch button builds source patch points through GetOrderedElementOutputs and matches them to destination outputs position by position.

DisplaySetup.ActivatePatchingControl also asks the current element control for SelectedElements after switching between simple and graphical patching. The new snapshot must remain valid after the gesture finishes, rather than being a temporary event-only order. Programmatic selection replacement or tree rebuilding starts a new selection state and invalidates the old directional snapshot.

Primary repository reference: docs/plans/display-setup/vix-4006-patched-output-range-order.md. Preserve its final contract for ascending controller outputs within current pane order, complete destination reversal when requested, and reverse-independent First/Last labels. The scoped design review is docs/reviews/vix-4009-range-selection-order-design.md. Existing regression fixtures are MultiSelectTreeviewKeyboardSelectionTests and SetupPatchingSimpleOutputOrderTests; the latter demonstrates STA controls, exception-safe global-manager restoration, a nonparallel collection, real output adapters, and narrow private-member reflection. STA means the Windows UI threading mode required by these controls.

## Plan of Work


Milestone 1 is already complete in this planning turn. Milestone 2 implements only the named selection snapshot and consumer handoff, with regressions that demonstrate the actual event-time and post-gesture order. Milestone 3 proves resulting patch connections and preserves the VIX-938, VIX-4006, and controller contracts, then records the delivered evidence in Jira. Read current source again before editing during execution, preserve unrelated changes, and keep all living sections current. The manual review boundaries below are required by the project analyze-and-plan-issue skill. They do not authorize automatic commits.

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Milestone 1: Record the agreed bug and acceptance criteria in Jira


Context: the user's historical report and explicit requirement establish the bug: reverse element ranges must patch in their selection direction for both mouse and keyboard, without undoing VIX-938. Jira descriptions remain user-facing; technical design lives in this document.

Plan of Work: completed. Validated the VIX project and Bug type, searched related issue summaries, created VIX-4009 with Summary, Scope, and Acceptance Criteria, linked it to VIX-938 with Relates, and read it back. No assignee, fix version, or priority was explicitly chosen, so no custom values were supplied. Preserve VIX-938's closed status and final requirements. This milestone also authors the local plan and scoped review.

Concrete Steps: the initial git status --short was empty. Jira creation and relation calls succeeded; readback confirms the expected issue type, description, New Ticket status, and related issue. Validate only the new documentation files and present a planning commit message through the project commit-msg skill. Do not create a commit.

Validation and Acceptance: VIX-4009 describes reproducible C-to-A and A-to-C patching, keyboard equivalence, stable anchored shrinking/crossing, view-switch retention, explicit-order precedence, and retained controller/shortcut/performance behavior. It does not claim the implementation is complete.

STOP HERE for manual review and commit execution before proceeding.

Halt before application implementation. Run git status --short and scoped git diff, inspect the untracked plan/review through Gortex, invoke the project commit-msg skill with VIX-4009 as the subject prefix, and output a complete paste-ready Commit message block. Do not create a commit without an explicit request. Wait for explicit user direction to execute Milestone 2.

### Milestone 2: Prove and implement directional element selection without changing canonical tree order


Context: edits belong in the three production files and three test files listed in the component matrix. Apply the project dotnet-best-practices and csharp-docs skills. Do not change existing signatures, key handling, traversal helpers, controller export, patch ordering algorithms, or unrelated formatting.

Plan of Work: first extend the existing nine keyboard regressions with characterization assertions at AfterSelect and after the gesture returns. They must continue asserting canonical SelectedNodes display order. Add tests for the new directional property once its minimal compiling declaration exists, so tests can fail on behavior rather than missing API compilation. For keyboard selection, use the existing internal ProcessKeyboardSelection seam. For mouse-equivalent range selection, use narrow reflection to invoke the existing private SelectNode(TreeNode, Keys) overload with Keys.Shift; this exercises the shared production selection path without global desktop input or a new production test API. Verify actual mouse hit-testing manually in Milestone 3.

Use A,B,C,D trees to test downward and upward ranges, Shift+Home/End, anchor-preserving shrinking and crossing, singleton and empty selections, expanded branches and collapsed descendants, and one range replacing another. At AfterSelect capture the new sequence into a separate array; assert the same order after EndUpdate and assert an earlier captured snapshot is unchanged by the next gesture. Check ordinary clicks, Ctrl addition/removal, AddSelectedNode, SelectedNodes replacement, SelectedNode replacement, ClearSelectedNodes, and tree repopulation invalidate range state rather than reusing old nodes. A Ctrl operation must still follow its old live-list behavior; do not change existing event timing to manufacture full chronological click order.

Add _rangeSelectionOrder and SelectedNodesInSelectionOrder as defined above. In SelectRange, leave canonical range computation and SelectRangeNode highlighting intact. After successful membership replacement, copy the normalized range, reverse only the copy when the anchor is its last item, and assign the read-only snapshot before returning to SelectNode's notification. Clear the snapshot in ClearSelectedNodes and at the beginning of ToggleNode, which covers ordinary selection mutations and AddSelectedNode. Do not clear it merely because EndUpdate sorts the canonical list. Keep the existing normal traversal and the nodes.Reverse() operation in NodesInPreviousVisibleRange unchanged. Document the new getter's active-range snapshot and non-range fallback, and its nonserialized nature with the existing Browsable/DesignerSerializationVisibility conventions.

In ElementTree, add SelectedElementNodesInSelectionOrder next to the existing selected-element projection and document that it provides anchor-to-endpoint range order without changing the other properties. In SetupElementsTree, use that projection in the SelectedElements getter. Forward SelectedElements from both selection and deselection handlers. Leave SelectedElements' setter unchanged so explicit repopulation clears range state normally. Add XML summary/value/remarks to the changed public getter and new public properties in this same change. Existing interfaces and event signatures remain unchanged.

Add SetupPatchingSimpleElementOrderTests using the established nonparallel global-state collection and STA test attributes. Inspect the current ElementNode/Element construction and registration APIs before creating real leaf fixtures; use the existing node service and isolated managers rather than copying the ordering algorithm. Save and restore every VixenSystem manager the fixture replaces, including on setup failure, and dispose all controls on their creating thread. Reuse the existing output-consumer test stub without broad fixture extraction or unrelated refactoring.

Construct tagged tree rows with distinct leaf elements and exercise the shared selection path, ElementTree's projection, and SetupElementsTree's actual event/getter handoff. Forward the event to SetupPatchingSimple.UpdateElementSelection, then inspect _cachedElementNodes and invoke GetOrderedElementOutputs through narrow reflection to verify the real source preparation path. Register the leaf data-flow components in the isolated manager so their identities can be checked in the resulting source references. Do not test a second hand-written ordering helper. Do not invoke Patch unattended if it opens modal dialogs. Read SelectedElements after the gesture and after a patching-view refresh to confirm that activation receives the same sequence; manual switching remains mandatory in Milestone 3.

Cover plain upward/downward leaf ranges, group selection with existing leaf expansion and first-occurrence deduplication, valid preferred Patching Order overriding directional input, Reverse Element Order reversing the final source sequence once, and filtering retaining eligible source order. For explicit-order tests avoid duplicate/missing preferred orders that open interactive conflict dialogs. An event-only test or an assertion only against canonical SelectedNodes does not establish this fix.

Concrete Steps: from C:\Dev\Vixen use full MSBuild to build the test target before running already-built tests, because QMLibrary and LiquidFunWrapper are C++/CLI dependencies. Run focused tests first, before and after production repair, and record actual failures and pass counts.

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~MultiSelectTreeviewKeyboardSelectionTests|FullyQualifiedName~MultiSelectTreeviewRangeSelectionOrderTests|FullyQualifiedName~SetupPatchingSimpleElementOrderTests"
    git diff --check

Start long-running commands with exec_command and yield_time_ms=10000. When only waiting for completion use write_stdin with yield_time_ms at least 30000. Expected after implementation: the build has zero errors, the new directional tests and existing nine VIX-938 tests have zero failures, and whitespace validation has no errors. Record actual results rather than assuming a total count.

Validation and Acceptance: demonstrate the directional regressions fail against the pre-repair behavior and pass afterwards. Gortex impact is required before each production edit; verify any signature change if execution departs from this additive design. After edits use detect and its changed symbol IDs with tests, guards, and contract. Run Rider get_file_problems on every changed C# file and fix only diagnostics within changed lines. Confirm snapshot processing adds only a linear selected-range copy/reversal and does not add recursive sorting or force handle creation. Public-property XML docs must match the implemented fallback and invalidation behavior.

STOP HERE for manual review and commit execution before proceeding.

Halt execution. Run git status --short and scoped git diff for these production files, test files, and the plan. Invoke the project commit-msg skill with VIX-4009 as the subject prefix and include Related to VIX-938. Output a complete paste-ready Commit message block. Do not create a commit without an explicit request. Wait for explicit user confirmation before Milestone 3.

### Milestone 3: Verify patch connections and retain the VIX-938 fixes


Context: use an isolated copied profile with four distinct simple leaf elements A,B,C,D, no preferred Patching Order initially, and a direct-output controller. Use a large materialized tree for LipSync responsiveness checks and the existing paged controller test scenarios. Do not save experimental patching into the original profile or send output to hardware as part of validation.

Plan of Work: verify actual mouse Shift-click selection in both directions, then keyboard Shift+Up/Down and Shift+Home/End. With ascending destination outputs and both reverse options off, inspect graphical connections or Find Patched Elements to prove that C-to-A selection connects C,B,A and A-to-C selection connects A,B,C. Shrink a reverse range back toward its anchor, cross the anchor, and patch the resulting range to prove its new order. Switch to graphical patching and back before patching and confirm that order is retained. Repeat patch preparation twice to check reversals do not accumulate.

Set an explicit valid preferred Patching Order and prove it overrides range direction. Enable Reverse Element Order and prove it reverses the effective flattened source order once. Check controller destinations and First/Last labels still satisfy VIX-4006, including Reverse Output Order and a selection spanning paged outputs. Group ranges retain their existing within-group leaf order. Selection changes alone do not modify existing connections.

Retest VIX-938's stable-anchor growing and shrinking in Element Tree and Controller Tree, Shift+Home/End, the TopNode fallback with no selection, no wrapping, Ctrl+Up/Down no-op behavior, collapsed descendant exclusion, unmodified navigation, and host Delete/copy/paste shortcuts. Use a large LipSync tree to confirm repeated range extension remains responsive and its canonical selected-node ordering remains unchanged. Compare the same tree and gestures before and after the repair; record observed timings or qualitative results honestly, without inventing a performance threshold or repeating whole-tree sorting.

Concrete Steps: from C:\Dev\Vixen run the existing output-order and controller/index suites after focused direction tests pass. Then run the complete built suite and a Release x64 solution rebuild because the change crosses Vixen.Common and Vixen.Application.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~SetupPatchingSimpleOutputOrderTests|FullyQualifiedName~ControllerTreeVirtualizationTests|FullyQualifiedName~OutputControllerOutputIndexTests"
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64
    git diff --check

Validation and Acceptance: all relevant tests and the full suite pass, the Release x64 build has zero errors, actual patch connections follow range direction, and VIX-938 navigation/performance plus VIX-4006 destination behavior remain intact. Report warnings, unavailable IDE checks, and unperformed manual checks accurately. Do not claim runtime completion from source assertions alone.

Update this plan's living sections with the delivered design, exact commands, pass counts, and manual evidence. Use the Jira skill to reconcile VIX-4009's user-facing description if requirements changed and add a concise validation comment. Preserve VIX-938's existing state and do not transition either issue without separate authorization. Read VIX-4009 back after reporting. No implementation-complete comment is authorized during the planning-only turn.

STOP HERE for manual review and commit execution before proceeding.

Halt execution after final evidence and reporting. Run git status --short and scoped git diff for this milestone. If repository files changed, invoke the project commit-msg skill with VIX-4009 as the subject prefix and output a complete paste-ready Commit message block. Do not create a commit without an explicit request. Wait for user review before further work.

## Concrete Steps


The exact execution commands are recorded in Milestones 2 and 3 with C:\Dev\Vixen as the working directory. The planning turn performs only source/document analysis, Jira creation/readback/linking, and local documentation validation. No build or runtime test is needed to validate newly authored Markdown, and no such result is claimed.

Before implementation, re-read git status, relevant source and fixture APIs through Gortex, and applicable project skills. Do not switch branches, reset the workspace, or revert the VIX-938 commits. Read historical source through Gortex commit views when available; never treat an inexact fallback as the requested historical source.

## Validation and Acceptance


With preferred order absent and Reverse Element Order off, the effective source sequence follows the original anchor toward the current endpoint for mouse and keyboard ranges. Canonical tree selection remains in display order. Directional reads agree during selection notification and after the gesture, including patching view switches. Shrinking and crossing the anchor recompute the sequence correctly. Empty/ordinary/programmatic selection cannot retain stale range order.

Existing nine VIX-938 keyboard tests remain green without changing their ascending SelectedNodes assertions. Add explicit direction/event/snapshot tests, actual consumer preparation tests, and manual connection evidence. Retain explicit source-order precedence, group expansion, eligibility filtering, controller output ordering, labels, paging, shortcuts, and large-tree responsiveness. This is an element source-sequence repair, not a controller destination-order change.

## Idempotence and Recovery


Every successful range replaces the snapshot; never reverse a previously cached snapshot in place. A consumer copy remains stable across later gestures. Clearing, replacing, rebuilding, or ordinary selection mutation invalidates the old snapshot. Repeated directional reads and view switches do not change membership or patch connections.

Fixtures restore all replaced global managers and dispose controls even on failure. Restrict manual experiments to copied profiles. Preserve unrelated worktree changes, and recover only this issue's reviewed edits if needed; do not reset the repository or revert unrelated VIX-938/VIX-4006 work. Before retrying Jira creation, read VIX-4009 rather than creating another issue. Before replaying reporting, inspect existing comments to avoid duplicates.

## Artifacts and Notes


Planning began with a clean git status. Jira project/type validation succeeded. VIX-4009 was created as Bug, linked to VIX-938 with Relates, and read back with New Ticket status and the expected acceptance criteria. Gortex impact for SelectRange reported medium risk, 23 transitively affected symbols, and MultiSelectTreeviewKeyboardSelectionTests as an existing test target. Impact checks for the new Markdown paths returned file_not_indexed because the files did not yet exist; this does not represent a source edit.

The plan and review are the only intended repository changes in this turn. Capture documentation validation and final status in the handoff. No C# was generated, no application tests/build were run, and no implementation commit was created.

## Interfaces and Dependencies


Add these public read-only properties and document them in the same implementation change:

    IReadOnlyList<TreeNode> MultiSelectTreeview.SelectedNodesInSelectionOrder
    IEnumerable<ElementNode> ElementTree.SelectedElementNodesInSelectionOrder

Keep these existing signatures unchanged:

    IEnumerable<ElementNode> SetupElementsTree.SelectedElements { get; set; }
    void SetupPatchingSimple.UpdateElementSelection(IEnumerable<ElementNode> nodes)
    bool MultiSelectTreeview.ProcessKeyboardSelection(Keys keyCode, Keys modifiers)
    private void MultiSelectTreeview.SelectNode(TreeNode node, Keys modifiers)
    private void MultiSelectTreeview.SelectRange(TreeNode anchorNode, TreeNode targetNode)

Use existing collection/array/LINQ facilities, TreeNode references, ElementNodesEventArgs, xUnit, Moq, Xunit.StaFact, the existing global-state nonparallel test collection, and the full-MSBuild workflow. No new interface, test-only production API, async workflow, package, or project is required. Runtime snapshots are not persisted.

## Revision Notes


2026-10-07 / Codex: Created the plan after the user requested restoring mouse and keyboard range direction without undoing VIX-938. Recorded the historical event-before-sort explanation, chose an explicit range-order snapshot while retaining canonical selection order, included the patching-view getter route, and created/verified VIX-4009 and its relation to VIX-938. Application implementation remains pending.

Analysis complete and plan integrated with plans.md.

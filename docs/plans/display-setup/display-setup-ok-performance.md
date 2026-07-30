# Improve Display Setup Close Responsiveness with Paged Controller Outputs (VIX-3955)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

JIRA issue: [VIX-3955](https://vixenlights.atlassian.net/browse/VIX-3955) — Improve Display Setup close responsiveness with paged controller outputs

## Purpose / Big Picture

Display Setup currently creates one native WinForms `TreeNode` for every controller output as soon as the dialog opens. On the measured profile, 27 controller roots eagerly created 163,655 output nodes. Closing either with OK or Cancel then made the UI thread spend roughly 33 seconds deleting those native tree items. The dialog remained visible but partially dismantled during that interval, which looked like an application hang.

After this change, Display Setup will keep the existing controller `TreeView`, but it will use bounded, adaptive output pages rather than copying `ElementTree`'s whole-branch lazy loading. Initially the tree will create only controller roots plus one virtual child for each non-empty controller. Expanding a controller with at most 256 outputs will materialize those outputs directly. Expanding a larger controller will create lightweight range nodes such as “Outputs 1–256” and “Outputs 257–512”; only expanding a range will materialize its maximum of 256 output leaves. A controller with 5,000 outputs, which is normal in Vixen profiles, will therefore create about 20 range nodes on controller expansion instead of 5,000 native output items.

The same change will correct two secondary feedback problems. Once the modal dialog can close promptly, the OK path will visibly paint a “Preparing Display Setup Changes” progress stage before synchronous orphan cleanup and controller reordering, then advance through the two existing save stages. The Cancel path will visibly paint “Reloading System Configuration” before its synchronous reload. Both paths will restore the cursor, controls, and progress visibility through `finally`, including when work fails.

A user can see the completed behavior by opening Display Setup on the representative 27-controller/163,655-output profile, expanding a 5,000-or-more-output controller, and observing range nodes appear immediately without thousands of output leaves. Expanding one range must display that page's outputs with their existing names, patch-state icons, selection behavior, and context-menu operations. Clicking either OK or Cancel after ordinary browsing must close the dialog promptly. On OK the main window must show preparation and save progress; on Cancel it must show reload progress.

## Progress

- [x] (2026-07-30 09:31 -05:00) Read `.agents/PLANS.md`, the repository instructions, the Display Setup transaction boundary, and the original OK-cleanup implementation.
- [x] (2026-07-30 10:15 -05:00) Analyzed the original OK and Cancel Timeline snapshots and established that both stalls occur during native window destruction after `WM_DESTROY`, not in the OK-only cleanup.
- [x] (2026-07-30 11:45 -05:00) Added and ran temporary subtree-disposal diagnostics. `CurrentControllers` reproduced the delay while `CurrentElements` and `CurrentPatching` did not.
- [x] (2026-07-30 12:15 -05:00) Added and ran the definitive controller-tree experiments. `ControllerTreeOnly` reproduced the delay, and `ControllerTreeClearNodesThenDispose` measured 33.006 seconds in `Nodes.Clear()` for 163,655 output nodes while disposal of the emptied tree took only a few milliseconds.
- [x] (2026-07-30 12:24 -05:00) Read `ElementTree` and `ControllerTree` population, expansion, selection, refresh, reorder, and output-operation paths. Revised this plan to retain the TreeView and lazily materialize controller outputs using the `ElementTree` precedent.
- [x] (2026-07-30 12:38 -05:00) Re-evaluated the `ElementTree` precedent after confirming that 5,000-or-more-output controllers are normal. Replaced whole-controller materialization with adaptive 256-output range pages and stable controller-ID/output-index state.
- [x] (2026-07-30 13:10 -05:00) Created [VIX-3955](https://vixenlights.atlassian.net/browse/VIX-3955) as a VIX Improvement under Display Setup, synchronized its proven-cause description, acceptance criteria, and test plan, and read it back without transitioning it.
- [ ] Add focused controller-tree virtualization tests that fail against eager or whole-controller population and protect range creation, page materialization, selection, refresh, and output operations.
- [ ] Implement adaptive, 256-output page materialization in `ControllerTree`.
- [ ] Move OK-only confirmed cleanup into the post-dialog progress workflow and force the preparation progress state to paint before cleanup starts.
- [ ] Force the Cancel reload progress state to paint before synchronous reload and use common `try/finally` UI restoration for both outcomes.
- [ ] Validate the production behavior with the temporary close diagnostics still present and capture a comparable post-change Timeline snapshot.
- [ ] Remove all temporary close-diagnostic code and experiment hooks, then run focused tests, the full test suite, a Debug build, whitespace validation, and final manual OK/Cancel checks.
- [ ] Record final evidence here and on the JIRA issue.

## Surprises & Discoveries

- Observation: OK-only cleanup is not the cause of the multi-second close stall.
  Evidence: the measured OK click entry-to-exit interval was about 229.5 ms, while the delay after `WM_DESTROY` was about 34.4 seconds. Cancel performed no cleanup, its click handler took about 0.003 ms, and it still delayed about 38.3 seconds before `WM_NCDESTROY`.

- Observation: the dominant cost is native TreeView item deletion, not managed form lifecycle callbacks.
  Evidence: the Timeline snapshots attribute roughly 30–35 seconds of UI-thread own CPU to `NtUserDestroyWindow`; `OnFormClosing`, `OnFormClosed`, and `OnHandleDestroyed` markers themselves were short.

- Observation: `SetupControllersSimple` is the only active major subtree that reproduces the stall.
  Evidence: isolated disposal measured approximately 4.45 ms for patching, 11.28 ms for elements, and 32.34 seconds for controllers. Disposing all active subtrees attributed about 34.73 seconds to controllers, about 49 ms to elements, and about 3.19 ms to patching.

- Observation: the `ControllerTree` native item collection, not the empty TreeView window, is conclusive.
  Evidence: `ControllerTreeOnly` took about 34.51 seconds and its Timeline profile attributed about 32.45 seconds of own CPU to `NtUserDestroyWindow`. The clear-nodes experiment counted 27 root nodes, 163,682 total nodes, and 163,655 output nodes; `Nodes.Clear()` took 33.0064184 seconds, `EndUpdate()` took 1.5375 ms, disposal of the empty tree took about 4.53 ms, and the remaining parent close completed in roughly 95 ms.

- Observation: the disappearing controls were a diagnostic clue, not a separate rendering defect.
  Evidence: early disposal experiments deliberately detached and disposed subtrees. The dialog stayed open because destruction of the remaining controller TreeView items continued synchronously on the UI thread.

- Observation: `ElementTree` supplies a useful sentinel/expansion precedent.
  Evidence: `ElementTree` initially adds root nodes with a child named `VIRT`, handles `BeforeExpand` by replacing that child with real children, recursively adds virtual children for unopened descendants, and does not discard materialized children on collapse.

- Observation: the `ElementTree` pattern is only a starting point, because it materializes an entire child collection at once.
  Evidence: 5,000-or-more-output controllers are normal. The measured aggregate averaged about 6,061 outputs per controller. Applying the measured 33.006-second deletion cost linearly to an average controller predicts roughly 1.22 seconds of teardown after expanding only one whole controller; this is an estimate, but it is too large to use whole-controller expansion as the production boundary.

- Observation: WinForms `TreeView` does not provide an owner-data or `VirtualMode` equivalent.
  Evidence: every real `TreeNode` becomes a native tree item. The practical way to retain this control is to bound which real items are inserted, rather than expecting owner-drawn text or callback labels to eliminate native item lifetime.

- Observation: the progress bar is set visible before Cancel reload today, but does not get a chance to paint.
  Evidence: `VixenApplication.SetupDisplay()` calls `UpdateProgress(...)` and then immediately calls synchronous `VixenSystem.ReloadSystemConfig()` on the UI thread. The observed reload is a separate multi-second interval after the modal form returns.

- Observation: the original plan's proposed property-model and filter-graph optimizations are not justified by the conclusive close evidence.
  Evidence: measured OK cleanup is hundreds of milliseconds, whereas native deletion of eager controller output nodes is tens of seconds and affects both OK and Cancel.

## Decision Log

- Decision: keep the controller TreeView and use adaptive, range-paged output materialization.
  Rationale: the user wants to retain the hierarchical interaction, 5,000-output controllers are normal, and whole-controller lazy loading would still create enough native items for a noticeable stall.
  Date/Author: 2026-07-30 / User and Codex

- Decision: create all controller roots initially, but no real output nodes. Add exactly one sentinel child to each controller whose `OutputCount` is greater than zero.
  Rationale: roots preserve controller visibility, selection, ordering, status, and expand affordances while keeping the native item count proportional to controller count.
  Date/Author: 2026-07-30 / Codex

- Decision: use a fixed production page size of 256 outputs.
  Rationale: the measured aggregate cost is about 0.202 ms per deleted output item, so 256 leaves project to about 52 ms of native deletion. This leaves substantially more headroom under the 200 ms interaction target than a 512-item page. A 5,000-output controller needs only about 20 range nodes.
  Date/Author: 2026-07-30 / Codex

- Decision: controllers with 256 or fewer outputs materialize leaves directly on controller expansion; larger controllers materialize navigation-only range nodes and then leaves one page at a time.
  Rationale: small controllers retain the current two-level interaction, while large controllers receive a hard per-expansion leaf bound without changing the TreeView control.
  Date/Author: 2026-07-30 / User and Codex

- Decision: retain a materialized page for the rest of the dialog session instead of unloading it on collapse.
  Rationale: current multi-selection is stored as live `TreeNode` references. Removing collapsed leaves would require a broader logical-selection rewrite and could invalidate selections used by patching and channel actions. Page-level materialization already prevents a single large-controller expansion from creating thousands of leaves. If users commonly expand every page, selection-independent eviction can be designed separately.
  Date/Author: 2026-07-30 / Codex

- Decision: programmatic selection of an output must materialize only its controller and containing 256-output page.
  Rationale: patching and tree-state restoration can target outputs directly without expanding unrelated ranges or controllers.
  Date/Author: 2026-07-30 / Codex

- Decision: selecting a controller continues to mean all of its outputs logically, even when none of its output nodes exist.
  Rationale: `SetupControllersSimple.BuildSelectedControllersAndOutputs()` already derives all outputs from the selected controller's `OutputCount`; it must not depend on materialized child nodes.
  Date/Author: 2026-07-30 / Codex

- Decision: stop using output display names and native `TreeNode.FullPath` strings as logical selection/restoration identities.
  Rationale: the added range level changes native paths, output names can be renamed or duplicated, and the existing API already supplies controller instances plus zero-based output indexes. Controller GUID plus output index is the stable identity; range start is derived as `(outputIndex / 256) * 256`.
  Date/Author: 2026-07-30 / Codex

- Decision: move the three existing OK-only preparation operations out of `buttonOk_Click` and run them after `ShowDialogAsync()` returns OK but before either configuration save.
  Rationale: the modal form must close before the main form can show progress, while cleanup and controller ordering must still precede persistence.
  Date/Author: 2026-07-30 / Codex

- Decision: force immediate painting with the existing control's `Refresh()` after setting each blocking progress stage.
  Rationale: visibility and text assignment alone do not pump a paint message before synchronous UI-thread work. `Refresh()` performs the bounded paint directly without `Application.DoEvents`, an arbitrary delay, or unsafe background execution.
  Date/Author: 2026-07-30 / Codex

- Decision: keep cleanup and reload on the UI thread.
  Rationale: both mutate global Vixen configuration and graph state that has not been established as thread-safe. This issue supplies prompt visual feedback rather than introducing a speculative concurrency change.
  Date/Author: 2026-07-30 / Codex

- Decision: retire the original plan's speculative orphan-property and orphan-filter algorithm changes.
  Rationale: they do not address the proven common OK/Cancel cause and would broaden the change. Their current semantics remain protected by existing behavior; any future optimization requires its own measurement and scope.
  Date/Author: 2026-07-30 / Codex

- Decision: retain the profiling logs and snapshots under `docs/references/display-setup`, but remove the temporary runtime diagnostics before completion.
  Rationale: the artifacts are useful evidence; the invasive WndProc, handle-destruction, census, and disposal-mode code is not production functionality.
  Date/Author: 2026-07-30 / Codex

- Decision: track the work as VIX-3955 with the Improvement issue type and the Display Setup component.
  Rationale: the VIX project exposes Improvement as a configured non-bug work type, matching this measured performance enhancement; no workflow transition is appropriate during issue creation.
  Date/Author: 2026-07-30 / Codex

## Outcomes & Retrospective

The investigation phase is complete and the root cause is conclusive: eager native output items in `ControllerTree` dominate Display Setup close time. The design now goes beyond `ElementTree`-style whole-branch loading: it retains the TreeView but bounds each large-controller output expansion to a 256-item page, uses stable model identities for state, and retains the reliable progress-painting work. Milestone 1 is complete: [VIX-3955](https://vixenlights.atlassian.net/browse/VIX-3955) captures the as-designed scope, acceptance criteria, and validation plan; implementation remains in subsequent milestones.

No production fix has been implemented by this plan revision. Temporary diagnostic changes remain in the working tree so the implementer can make one post-fix comparison before removing them. Update this section with the implemented files, measured before/after node counts and close latency, test results, and any follow-up limitations when the work is complete.

## Context and Orientation

`DisplaySetup` is the modal WinForms dialog in `src/Vixen.Application/Setup/DisplaySetup.cs`. `VixenApplication.SetupDisplay()` in `src/Vixen.Application/VixenApplication.cs` creates it in a `using` block and awaits `ShowDialogAsync()`. The established transaction boundary is:

- OK prepares confirmed in-memory state, saves system configuration, then saves module configuration.
- Cancel saves nothing and calls `VixenSystem.ReloadSystemConfig()` to discard the session's in-memory changes.

Do not change that boundary. `docs/plans/vix-2690-element-tags-workflow.md` documents why Display Setup changes are intentionally captured or discarded by this host-level OK/Cancel flow.

The controller UI is `Common.Controls.ControllerTree` in `src/Vixen.Common/Controls/ControllerTree.cs`, hosted by `src/Vixen.Application/Setup/SetupControllersSimple.cs`. Its current `_PopulateControllerTree()` clears the TreeView and calls `AddControllerToTree(...)` for every output controller. `AddControllerToTree(...)` immediately loops from zero to `controller.OutputCount - 1` and creates a native child `TreeNode` for each output. This loop is the source of the 163,655 native items.

Each controller root uses its controller GUID as `TreeNode.Name` and stores the `IControllerDevice` in `TreeNode.Tag`. Each current output child uses its output name for `Name` and `Text`, and its zero-based output index for `Tag`. Preserve controller tags and output-index tags for existing operations, but do not preserve output names as logical state keys. The paged implementation introduces a navigation-only range descriptor in `Tag` for nodes labeled `Outputs {first}–{last}`. Any code deciding whether a node represents an output must require `Tag is int`; range and sentinel nodes are never selected channels.

`ControllerTree.PopulateControllerTree(Dictionary<IControllerDevice, HashSet<int>>)` already receives the stable information needed for targeted selection: controller plus zero-based output indexes. It currently converts those values to paths containing output names. Replace that representation with typed internal state: selected controller GUIDs, selected `(controller GUID, output index)` pairs, expanded controller GUIDs, expanded `(controller GUID, page start)` pairs, and a typed preferred-top-node identity. For output index `i`, compute `pageStart = (i / OutputPageSize) * OutputPageSize`. Restoration expands only the owning controller and, for large controllers, the containing range before locating the `int`-tagged output leaf.

`RefreshControllerOutputNames()` and `RefreshControllerOutputStatus()` currently assume output leaves are immediate controller children. Under paging they must enumerate materialized leaves through a helper that understands both direct small-controller leaves and leaves under range nodes. Deferred pages read current output names and patch sources when materialized. Controller status and controller reordering operate only on roots and must not materialize ranges or outputs.

The precedent is `src/Vixen.Common/Controls/ElementTree.cs`. It demonstrates the sentinel/`BeforeExpand` mechanism, but not the required granularity: it replaces a sentinel with every child. Reuse only that event-driven materialization concept. `ControllerTree` needs an adaptive provider:

- zero outputs: controller has no sentinel or expansion affordance;
- 1–256 outputs: controller has a sentinel, and first expansion creates direct output leaves;
- more than 256 outputs: controller has a sentinel, and first expansion creates `ceil(OutputCount / 256)` range nodes, each with its own sentinel;
- range expansion: replace only that range's sentinel with its at-most-256 output leaves;
- collapse: retain materialized nodes for selection compatibility during this issue.

The range label is presentation only. Internal indexes are zero-based; display labels are one-based and inclusive, for example a descriptor with `StartIndex = 256` and `Count = 256` displays `Outputs 257–512`. The final range ends at the actual `OutputCount`.

Several output actions currently assume `node.Parent.Tag is OutputController`, including insert, remove, unpatch, and other selected-channel operations. That is false for paged leaves because their immediate parent is a range node. Add one focused owning-controller resolver that walks ancestors until it finds an `IControllerDevice`, and use it everywhere an output leaf must resolve its controller. Audit `SetupControllersSimple.BuildSelectedControllersAndOutputs()` separately because it lives in another assembly; it must likewise resolve the controller ancestor rather than only `node.Parent`.

The OK button has `DialogResult.OK`. Its current click handler calls, in order:

    VixenSystem.Filters.RemoveOrphanedFilters();
    PropertyManager.RemoveOrphanedProperties();
    _setupControllersSimple?.ReorderControllers();

Those operations currently complete before the dialog can return. They must move together into an assembly-internal instance method on the still-live `DisplaySetup` object, called after `ShowDialogAsync()` returns OK and before saving. The form object has not yet been disposed because it remains inside the `using` block.

`VixenApplication.SetupDisplay()` owns the existing main-window progress bar. Its OK path already displays system-save and module-save labels; its Cancel path assigns a reload label. The missing behavior is an immediate paint before synchronous cleanup or reload and reliable restoration if an exception occurs.

The temporary investigation is implemented in `src/Vixen.Application/Setup/DisplaySetup.CloseDiagnostics.cs`, with hooks in `DisplaySetup.cs`, `SetupControllersSimple.cs`, and `VixenApplication.cs`. It records close markers, native handle census data, subtree-disposal experiments, controller-tree node counts, and the `ControllerTreeClearNodesThenDispose` experiment. None of those hooks or experiment modes belongs in the final product.

## Plan of Work

### Milestone 1: Create and synchronize the JIRA issue

Use the project Jira skill at `.agents/skills/jira/SKILL.md`. Create an Improvement if that type exists in the VIX project; otherwise use the closest configured non-bug work type and record the choice in `Decision Log`. Suggested summary:

    Improve Display Setup close responsiveness with paged controller outputs

The issue description must explain the proven cause, not the retired cleanup hypothesis: Display Setup eagerly creates controller output nodes and on large display, this can easily exceed 100,000 with several controllers; native deletion costs about 33 seconds and affects both OK and Cancel; 5,000-or-more-output controllers are normal; the retained TreeView will use adaptive 256-output range pages rather than whole-controller materialization; and visible progress will precede OK cleanup/saves and Cancel reload. Link this stable plan path and include the acceptance and test criteria below.

Replace the JIRA placeholder in this file immediately after creation, add the issue URL, and read the issue back to verify its description. Do not transition it.

### Milestone 2: Add bounded-virtualization characterization tests

Add `src/Vixen.Tests/Common/ControllerTreeVirtualizationTests.cs`, following the existing WinForms tests in `src/Vixen.Tests/Common/MultiSelectTreeviewKeyboardSelectionTests.cs`. `Controls.csproj` already exposes internals to `Vixen.Tests`, so prefer a narrow internal test seam over a new public API if direct testing is otherwise blocked by `VixenSystem`.

The tests must establish these behaviors:

- initial population creates one controller root per controller and exactly one virtual child per non-empty controller, with no child tagged as an output index;
- a zero-output controller has no virtual child and no expand affordance;
- expanding controllers with 1, 255, and 256 outputs creates direct leaves and no range nodes;
- expanding controllers with 257, 5,000, and an exact multiple of 256 outputs creates respectively 2, 20, and the exact expected number of range nodes, with no real output leaves;
- range labels are one-based and inclusive while descriptors remain zero-based; the final range count and label stop at the actual output count;
- expanding one range creates no more than 256 output leaves, with the existing output name, zero-based index tag, and white/grey/green patch-state image rules;
- expanding another range does not materialize the intervening or sibling ranges, and expanding another controller does not materialize any output in the first controller;
- a second expansion and collapse/re-expand cycle do not duplicate controller ranges or output leaves;
- selecting output indexes 0, 255, 256, and the final output of a 5,000-output controller materializes only the correct direct branch or containing page and selects the correct leaf;
- selection and restoration still find an output after its display name changes and can distinguish duplicate output names because logical identity is controller GUID plus output index;
- restoring expanded state and the preferred top node materializes only the controller/pages required for that saved state;
- selecting a controller does not materialize ranges or outputs and still exposes that controller through `SelectedControllers`; verify the application-level `BuildSelectedControllersAndOutputs()` all-logical-outputs behavior in the manual integration scenario because `Vixen.Tests` does not reference `Vixen.Application`;
- page and sentinel navigation nodes are ignored by selected-channel building and cause output context menus/actions to remain unavailable;
- owning-controller resolution succeeds for direct leaves and paged leaves, and insert/remove/unpatch/find-patched paths receive the same controller/output index as before;
- output name and patch-status refresh affect all materialized leaves, ignore navigation/sentinel nodes, and are reflected correctly when a deferred page is expanded later;
- repopulation after output-count changes returns the controller to one sentinel, rebuilds correct page boundaries, and materializes only state-required pages;
- controller reorder reads only root nodes and never forces page or output materialization.

Use a fake controller with at least 5,000 outputs to prove bounded node creation by count, not wall-clock timing. Assert that expanding the controller creates 20 range nodes and zero leaves, and that expanding any one range creates at most 256 leaves. Do not add a flaky elapsed-time unit assertion.

### Milestone 3: Implement adaptive controller output pages

Modify `src/Vixen.Common/Controls/ControllerTree.cs`.

Add constants for the sentinel and hard page boundary:

    private const string VirtualNodeName = @"VIRT";
    private const int OutputPageSize = 256;

Change controller-root creation so it always creates and adds the controller root immediately, preserves the existing controller icon, and adds one sentinel child only when `controller.OutputCount > 0`. Do not enumerate `controller.Outputs` during initial population.

Introduce one internal/private range descriptor containing the owning controller, zero-based `StartIndex`, and `Count`. Keep it out of persisted configuration. On first expansion of a controller:

- if `OutputCount <= OutputPageSize`, replace the sentinel with direct output leaves;
- otherwise, replace the sentinel with range nodes generated by stepping `StartIndex` from zero by `OutputPageSize`; give each range node a sentinel child, a one-based inclusive label, and no output/channel semantics.

On first expansion of a range, replace only that range's sentinel with leaves for `[StartIndex, StartIndex + Count)`. Move the existing leaf creation and patch-state image rules into one helper shared by direct and paged materialization. Preserve output `Name`, `Text`, and zero-based `int` tag. Retain materialized leaves on collapse during this issue.

Extend `treeView_BeforeExpand` while preserving existing double-click suppression first: when the event is cancelled, return without materializing. Dispatch expansion by node role—controller, range, output, or sentinel—and make repeated events idempotent. Do not populate from paint, selection-changed, status-refresh, controller-status, or context-menu-opening events.

Replace string-path state with typed internal keys. Save and restore selected controllers by controller GUID, selected outputs by `(controller GUID, output index)`, expanded controllers by GUID, expanded ranges by `(controller GUID, page start)`, and the preferred top node by an equivalent typed discriminator. `PopulateControllerTree(Dictionary<IControllerDevice, HashSet<int>>)` must stop converting indexes to output-name paths. To restore an output, find its controller root, expand it, compute and expand its page when paging applies, then find the leaf by `Tag == outputIndex`. Selecting or restoring a controller alone must leave its children deferred.

Add a focused helper that walks ancestors from an `int`-tagged leaf to its owning `IControllerDevice`. Use it in every `ControllerTree` operation that currently assumes `SelectedNode.Parent.Tag` or `node.Parent.Tag`, including insert, remove, unpatch, find-patched, and any drag/context behavior. Update `src/Vixen.Application/Setup/SetupControllersSimple.cs` so `BuildSelectedControllersAndOutputs()` also resolves a controller ancestor for paged leaves. Page and sentinel nodes must not enable controller or channel operations.

Refactor output-name and output-status refresh to enumerate only materialized `int`-tagged leaves at either supported depth. Deferred pages read current state when expanded. Output-count changes and controller reconfiguration already repopulate the tree; ensure repopulation recalculates page boundaries rather than retaining stale descriptors.

Harden selection-dependent UI explicitly. In `ControllerTree.contextMenuStripTreeView_Opening`, show channel actions only when the selection is non-empty and every selected node is an `int`-tagged output leaf; any selection containing a range or sentinel must cancel the channel menu. In `SetupControllersSimple`, replace the raw `SelectedTreeNodes.Count > 0` button-enablement check with a check for at least one selected controller root or output leaf. Audit keyboard selection and drag initiation so a selected range cannot reorder controllers or invoke channel commands.

Do not change `ControllerTree` to a grid, use owner-drawn fake rows, add a new UI dependency, or build a generic tree-provider framework. Do not materialize an entire controller merely to restore selection, calculate controller-wide logical selection, refresh status, or perform controller reorder.

### Milestone 4: Make OK preparation and Cancel reload progress visible

Modify `src/Vixen.Application/Setup/DisplaySetup.cs`. Replace the work in `buttonOk_Click` with:

    internal void ApplyConfirmedChanges()

The method must synchronously execute the same three calls in the same order: remove orphaned filters, remove orphaned properties, and apply controller ordering. Remove the OK click subscription and handler if it has no remaining work. The internal method does not require XML documentation; if implementation changes a public or protected API, use `.agents/skills/csharp-docs/SKILL.md` and update its XML documentation.

Modify `src/Vixen.Application/VixenApplication.cs`. In `SetupDisplay()`, after `ShowDialogAsync()` returns, enter one common busy-UI scope: set the wait cursor, disable buttons, and restore cursor/buttons/progress visibility in `finally`.

For OK, perform these stages in order:

    Show and paint: 0, "Preparing Display Setup Changes"
    form.ApplyConfirmedChanges()
    Show and paint: a later value, "Saving System Configuration"
    await VixenSystem.SaveSystemConfigAsync()
    Show and paint: a later value, "Saving Module Configuration"
    await VixenSystem.SaveModuleConfigAsync()

For Cancel or another non-OK result, perform:

    Show and paint: 0, "Reloading System Configuration"
    VixenSystem.ReloadSystemConfig()

Use a small private helper if useful to set `progressBar.Visible`, call the existing `UpdateProgress(...)`, and immediately call `progressBar.Refresh()`. Refresh every stage so the custom text and value are painted before the next blocking operation. If manual testing shows the parent layout must also update when the bar becomes visible, update the containing control directly; do not use a general message pump.

Do not use `Task.Run`, `.Wait()`, `.Result`, `Application.DoEvents()`, `Task.Delay()`, a timer, or a minimum display duration. A fast cleanup may make the preparation stage brief, but it must be painted before cleanup begins. Preserve the existing `_ = MakeTopMost()` behavior after Cancel reload.

### Milestone 5: Validate the fix with diagnostics, then remove diagnostics

With the temporary instrumentation still present, first record each controller's `OutputCount` and the minimum, median, maximum, and total across the representative profile. This confirms the real distribution behind the aggregate and identifies at least one 5,000-or-more-output controller for page validation. This diagnostic is temporary and must be removed with the other instrumentation.

Run the representative profile with no controller branches expanded. Capture one OK and one Cancel log and Timeline snapshot. The initial controller-tree census should show 27 controller roots, zero real output nodes, and at most one sentinel per non-empty controller instead of 163,655 real output nodes. Both results should return from the dialog promptly, without a 30-second `NtUserDestroyWindow` block.

Repeat with a controller having at least 5,000 outputs. Expanding the controller must create about `ceil(OutputCount / 256)` range nodes and zero output leaves. Expand one range and confirm that the census increases by no more than 256 real output nodes. Collapse and re-expand the same range and verify it was materialized once. Then programmatically select an output in a different range from the patching flow and verify only that containing page is additionally materialized. Capture close timing with one and with two materialized pages.

After those measurements are recorded, delete `src/Vixen.Application/Setup/DisplaySetup.CloseDiagnostics.cs` and remove:

- diagnostic marker calls and the temporary Cancel click subscription/handler from `DisplaySetup.cs`;
- `RecordShowDialogAsyncReturnedAndWriteDiagnostics()` from `VixenApplication.cs`;
- `ControllerTreeViewForCloseDiagnostics` and its diagnostic XML comment from `SetupControllersSimple.cs`;
- any disposal-mode environment-variable parsing, WndProc overrides, handle census, or diagnostic-only imports left by the instrumentation.

Do not remove the evidence under `docs/references/display-setup`. Run a final application session without instrumentation to ensure removal did not alter the production result.

### Milestone 6: Complete automated, manual, and performance validation

Read and apply `.agents/skills/dotnet-best-practices/SKILL.md` to the C# diff and `.agents/skills/csharp-async/SKILL.md` to the revised `SetupDisplay()` flow before final validation. Use `csharp-docs` only if a public or protected API changes.

Run the focused controller-tree virtualization tests, the full `Vixen.Tests` project, a Debug rebuild, and `git diff --check`. Then perform the manual acceptance scenarios in `Validation and Acceptance`.

Analyze the final Timeline snapshots with the `dottrace-analyze` skill. If snapshot analysis fails, stop making performance claims: retain the functional/test results, record the analysis failure in this plan, and obtain a readable replacement snapshot before declaring the performance acceptance criteria complete.

### Milestone 7: Finalize JIRA and this living plan

Update all living sections with as-built names and evidence. Add a final JIRA comment containing the files changed, focused/full test results, manual results, initial and materialized node counts, OK/Cancel click-to-progress latency, dialog-return latency, and before/after native destruction CPU. Keep workflow transitions outside this plan.

## Concrete Steps

Run commands from `C:\Dev\Vixen` in PowerShell.

Inspect the relevant implementation before editing:

    Get-Content -Raw .agents/PLANS.md
    Get-Content -Raw .agents/skills/dotnet-best-practices/SKILL.md
    Get-Content -Raw .agents/skills/csharp-async/SKILL.md
    Get-Content -Raw src/Vixen.Common/Controls/ElementTree.cs
    Get-Content -Raw src/Vixen.Common/Controls/ControllerTree.cs
    Get-Content -Raw src/Vixen.Application/Setup/SetupControllersSimple.cs
    Get-Content -Raw src/Vixen.Application/Setup/DisplaySetup.cs
    Get-Content -Raw src/Vixen.Application/VixenApplication.cs

Run focused tests while implementing:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore --filter FullyQualifiedName~ControllerTreeVirtualizationTests --nologo

Run the complete test suite:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --nologo

Build the application and all solution dependencies:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Check whitespace and ensure temporary diagnostics are gone:

    git diff --check
    rg -n "DISPLAY_SETUP_CLOSE_DIAGNOSTICS|RecordCloseDiagnostic|PerformCancelDisposalExperiment|ControllerTreeViewForCloseDiagnostics" src

The final `rg` command must return no matches. Record exact commands, pass counts, build result, warnings, and profiling artifact names in `Artifacts and Notes`.

## Validation and Acceptance

Acceptance is binary unless a criterion explicitly records a measured value.

1. Given the representative profile with 27 controllers and 163,655 outputs, when Display Setup first appears, then the controller TreeView contains 27 controller roots, zero range nodes, zero real output nodes, and one sentinel child for each non-empty controller.
2. Given no controller has been expanded, when the user clicks OK or Cancel, then `ShowDialogAsync()` returns and the applicable main-window progress stage is visibly painted within 200 ms on the profiling machine; there is no tens-of-seconds native destruction interval.
3. Given a controller with 1–256 outputs, when it is expanded, then its outputs appear directly and exactly once with correct names, zero-based index tags, and white/grey/green patch-state icons.
4. Given a controller with 5,000 or more outputs, when it is expanded, then only one range node per at-most-256 outputs appears, no output leaves are created, and controller expansion remains responsive.
5. Given a range is expanded, when its leaves appear, then it creates at most 256 outputs for exactly that range; no sibling range or controller output is materialized.
6. Given an already-materialized controller or range is collapsed and expanded again, when it reopens, then its existing children are reused without duplicates.
7. Given output indexes 0, 255, 256, or the last output are requested through `SelectedControllersAndOutputs`, when the tree restores selection, then only the owning controller and containing range are materialized and the correct output leaf is selected.
8. Given duplicate output names or an output rename, when selection and tree state are restored, then controller GUID plus output index selects the correct output without relying on the display name.
9. Given a controller root is selected while its outputs are deferred, when selected controller/output data is built, then all logical outputs of that controller are included exactly as before.
10. Given a page or sentinel node is clicked or included in multi-selection, when selection-dependent UI updates, then it is treated only as navigation and cannot enable or execute channel operations.
11. Given a direct or paged output leaf is selected, when insert, remove, unpatch, find-patched, or patching selection code resolves it, then it receives the correct owning controller and zero-based output index.
12. Given controller output names, count, or patch sources change, when loaded leaves refresh or deferred pages first expand, then displayed output state and range boundaries match the current controller model.
13. Given controllers are reordered, when the user clicks OK and reopens Display Setup, then root order persists without materializing unrelated ranges or outputs.
14. Given one page containing 256 outputs has been materialized, when OK or Cancel closes the dialog, then native destruction and dialog return remain below 200 ms on the profiling machine. Record the exact count and timing; if 256 leaves do not leave sufficient margin, reduce `OutputPageSize` and repeat rather than relaxing the target.
15. Given OK is clicked, when the modal dialog returns, then “Preparing Display Setup Changes” is painted before orphan cleanup/controller reorder, followed by “Saving System Configuration” and “Saving Module Configuration”; cleanup occurs before both saves.
16. Given Cancel is clicked after an in-memory setup change, when the modal dialog returns, then “Reloading System Configuration” is painted before reload, no OK cleanup or save executes, and reopening Display Setup shows the change was discarded.
17. Given cleanup, reload, or save throws, when the operation exits, then the progress bar is hidden, buttons are enabled, and the cursor is restored by `finally`; existing error propagation/logging behavior is not silently swallowed.
18. Given the production fix is complete, then no close-diagnostic source file, WndProc instrumentation, disposal experiment, environment switch, controller-count logger, or diagnostic test hook remains in `src`.
19. The focused tests pass, the full `Vixen.Tests` suite passes, the Debug solution rebuild succeeds, and `git diff --check` reports no errors.

This issue intentionally does not guarantee constant close time after a user expands every range in every controller. The accepted boundary is that no single controller expansion can create thousands of leaves and no single range expansion can create more than 256. Materialized pages remain cached for selection compatibility, so record the number of materialized pages and output leaves in every performance comparison.

## Idempotence and Recovery

Population must be idempotent. A controller sentinel may be replaced exactly once with either direct leaves or page nodes. A page sentinel may be replaced exactly once with at-most-256 leaves. Repeated `BeforeExpand` events must observe the node role and existing children rather than append duplicates. Full repopulation may clear the TreeView, recreate controller roots and sentinels, then selectively materialize only controllers/pages required by typed saved state or requested selection.

If a test fails midway, rerun the focused filter; it must not depend on a persisted profile. Use mock or test controllers and restore any global Vixen state installed by a fixture in `finally` or fixture disposal.

Profiling runs must use a copy of the representative user profile. OK saves and Cancel reloads mutate or reload global configuration, so do not experiment against an unbacked-up profile.

Keep temporary diagnostics until the first post-fix comparison is captured. Their removal is then a normal, explicit source edit. Do not use `git reset --hard`, `git checkout --`, or broad deletion commands because the working tree contains user-owned diagnostic and documentation changes.

If selection restoration cannot locate an output, do not fall back to eager population or output-name matching. Resolve the controller by GUID, validate the zero-based index against current `OutputCount`, compute the containing page, materialize only that page, and correct the typed state traversal. If the index is no longer valid after an output-count change, skip that stale selection safely.

## Artifacts and Notes

The conclusive evidence is stored under `docs/references/display-setup/`:

- `display-setup-cancel-none.{log,dtp}`: natural close baseline.
- `display-setup-cancel-current-patching.{log,dtp}`: patching-only isolation.
- `display-setup-cancel-current-elements.{log,dtp}`: elements-only isolation.
- `display-setup-cancel-current-controllers.{log,dtp}`: controller-subtree isolation.
- `display-setup-cancel-all-active-subtrees.{log,dtp}`: sequential active-subtree isolation.
- `display-setup-cancel-controller-tree-only.{log,dtp}`: controller TreeView isolation.
- `display-setup-cancel-controller-tree-clear-nodes.{log,dtp}`: definitive node census and `Nodes.Clear()` timing.

Important measured values:

    Original OK cleanup:                         ~229.5 ms
    Original OK WM_DESTROY -> WM_NCDESTROY:      ~34.416 s
    Original Cancel click handler:               ~0.003 ms
    Original Cancel WM_DESTROY -> WM_NCDESTROY:  ~38.275 s
    CurrentPatching isolated disposal:           ~4.45 ms
    CurrentElements isolated disposal:           ~11.28 ms
    CurrentControllers isolated disposal:        ~32.34 s
    ControllerTreeOnly disposal:                 ~34.51 s
    Controller roots:                            27
    Controller output nodes:                     163,655
    Total recursive controller-tree nodes:        163,682
    Controller TreeView Nodes.Clear():            33.0064184 s
    Approximate deletion cost per output:          ~0.202 ms
    Average outputs per controller:                ~6,061
    Estimated average whole-controller deletion:  ~1.22 s
    Planned maximum output page size:              256
    Estimated deletion cost per full page:         ~52 ms
    Empty ControllerTree disposal:                ~4.53 ms
    Remaining parent close after emptying tree:   ~95 ms
    NtUserDestroyWindow own CPU, tree-only run:   ~32.45 s

Post-change evidence to fill in:

    JIRA issue and URL: VIX-3955 — https://vixenlights.atlassian.net/browse/VIX-3955 (created and read back 2026-07-30; status: New Ticket; no transition performed)
    Controller output count distribution: TBD
    Focused virtualization tests: TBD
    Full tests: TBD
    Debug build: TBD
    Initial root/sentinel/range/output counts: TBD
    5,000+ controller range count: TBD
    One-materialized-page node count: TBD
    Two-materialized-page node count: TBD
    OK click to visible preparation progress: TBD
    Cancel click to visible reload progress: TBD
    OK dialog-return/native destruction time: TBD
    Cancel dialog-return/native destruction time: TBD
    Final Timeline snapshots: TBD
    Diagnostic removal verification: TBD
    Manual selection/refresh/reorder validation: TBD

## Interfaces and Dependencies

No new NuGet packages, projects, solution entries, services, configuration formats, or persisted fields are required.

`ControllerTree` should add only private implementation details unless tests require a narrow assembly-internal seam:

    private const string VirtualNodeName = @"VIRT";
    private const int OutputPageSize = 256;
    private void AddControllerChildren(TreeNode controllerNode, IControllerDevice controller);
    private void AddOutputLeaves(TreeNodeCollection target, IControllerDevice controller, int startIndex, int count);
    private IControllerDevice? FindOwningController(TreeNode outputNode);
    private void SelectOutput(IControllerDevice controller, int outputIndex);

Equivalent descriptive names are acceptable. Add one private or assembly-internal page descriptor containing controller, `StartIndex`, and `Count`, plus typed internal state keys for selected/expanded/top-node restoration. Do not introduce persisted DTOs or use page labels as keys.

Preserve existing public `PopulateControllerTree(...)`, selection, refresh, reorder, and controller-operation signatures. `src/Vixen.Application/Setup/SetupControllersSimple.cs` may add a private controller-ancestor resolver for `BuildSelectedControllersAndOutputs()`; no new cross-assembly public API is required.

`DisplaySetup` should add:

    internal void ApplyConfirmedChanges()

It synchronously performs the existing confirmed cleanup and reorder operations in their existing order and is called only after `ShowDialogAsync()` returns `DialogResult.OK`.

`VixenApplication.SetupDisplay()` remains the existing private `async void` event entry. It may use a private progress-paint helper, but it must not add another `async void` member or move global configuration mutation to a worker thread.

If implementation adds or changes any public or protected member despite this design, update XML documentation in the same change as required by `AGENTS.md`.

## Revision Notes

- 2026-07-30 / Codex: Created the original cleanup-first ExecPlan from the initial OK pause and classic snapshot. It preserved the OK-save/Cancel-reload boundary and proposed visible preparation progress plus speculative property/filter cleanup optimizations.
- 2026-07-30 / Codex: Replaced the disproven cleanup-first root-cause narrative after OK/Cancel Timeline profiling, subtree isolation, controller-tree-only isolation, and the definitive 163,655-node `Nodes.Clear()` experiment. The production design now keeps the TreeView, applies `ElementTree`-style first-expand lazy output loading, paints progress before OK cleanup and Cancel reload, removes speculative property/filter optimization work, and requires removal of all temporary diagnostics after post-fix validation.
- 2026-07-30 / Codex: Replaced whole-controller `ElementTree`-style loading after the user clarified that 5,000-or-more-output controllers are normal. The plan now uses adaptive 256-output range pages, stable controller-GUID/output-index state, ancestor-based controller resolution for paged leaves, tests for page boundaries and navigation-node safety, and profiling acceptance with one and two materialized pages. The OK/Cancel progress fixes are unchanged.
- 2026-07-30 / Codex: Completed Milestone 1 by creating and reading back VIX-3955 as a VIX Improvement under the Display Setup component. The issue records the measured native TreeView-deletion cause, 256-output paging approach, progress-paint requirements, acceptance criteria, and validation commands; it remains in New Ticket.

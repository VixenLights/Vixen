# VIX-4009 range selection order design review

Issue: https://vixenlights.atlassian.net/browse/VIX-4009. Related issue: https://vixenlights.atlassian.net/browse/VIX-938. Execution plan: docs/plans/display-setup/vix-4009-range-selection-patch-order.md. Reviewed on October 7, 2026 using the project dotnet-design-pattern-review skill. This is a scoped design review; no application implementation or runtime validation is claimed.

## Finding and proposed repair


In 3.12u6, upward Shift-click added selected nodes bottom-to-top while sorting was deferred. The selection event reached Display Setup before EndUpdate sorted the live list. SetupPatchingSimple copied the event-time sequence, allowing reverse selection to control patching. VIX-938's July 22, 2026 performance commit 23910a6a explicitly normalizes upward range traversal before that notification, which changed patching behavior.

Keep the current canonical selected-node order and add a separate read-only snapshot representing the active range from its original anchor to its current endpoint. ElementTree projects that snapshot to ElementNode values. SetupElementsTree sends it in selection events and returns it from SelectedElements. This is required because DisplaySetup.ActivatePatchingControl reads the getter when switching views, independently of selection notifications.

## Contract and responsibility assessment


MultiSelectTreeview owns visible range membership, the original anchor, and the endpoint. It is therefore the appropriate owner of range direction. ElementTree already owns TreeNode-to-ElementNode projection. SetupElementsTree owns Display Setup's selection handoff. SetupPatchingSimple already owns source expansion, preferred order, reversal, and source-to-output connections. These responsibilities remain separate; patching must not reconstruct direction from row names or infer it from controller output order.

Add only MultiSelectTreeview.SelectedNodesInSelectionOrder and ElementTree.SelectedElementNodesInSelectionOrder. Keep interface and event signatures unchanged. Retain the existing SelectedNodes and SelectedElementNodes behavior for other consumers. The new properties require XML summary, value, and remarks documenting range direction, read-only access, invalidation, and the non-range fallback. SetupElementsTree.SelectedElements also requires updated XML documentation because its range-order behavior changes.

Do not add a service, dependency-injection layer, command hierarchy, strategy interface, or factory for this small synchronous legacy WinForms repair. Repository/provider/resource patterns and asynchronous I/O patterns do not participate in the selection defect. No WPF view model or binding changes are involved.

## Performance and state assessment


The existing normalized range contains both endpoints. A copied array can retain its order when the anchor is first or reverse the copy when the anchor is last. This adds linear work in the selected range length and leaves the canonical list unchanged. Do not restore per-node ToggleNode sorting or add another whole-tree traversal. VIX-938's guarded EnsureVisible path and keyboard handling must remain intact.

Invalidate the directional snapshot on selection clear, ordinary ToggleNode changes, programmatic replacement, and rebuilding. Store it after range membership has been replaced and before the host can receive the event. Do not mutate an earlier snapshot or discard it merely because canonical sorting finishes. Empty and singleton selection remain safe. A non-range read falls back to the existing selected-node sequence; this does not establish a new chronological Ctrl-click contract.

Current collection APIs allow consumers to access the original selected-node list. Implementation must inspect changed-symbol guards and affected usages before editing and verify that supported selection mutations invalidate range state. Do not expand the task into replacing those existing collection APIs.

## Testability and acceptance assessment


Existing MultiSelectTreeviewKeyboardSelectionTests assert canonical display order and should remain unchanged in that respect. New tests must separately assert directional order at notification and after EndUpdate. The shared private SelectNode(TreeNode, Keys) overload can be invoked through narrow reflection for mouse-equivalent gestures; actual hit-testing requires manual verification. Keyboard tests should use the existing internal ProcessKeyboardSelection method.

Cover upward/downward ranges, Home/End, shrinking, crossing the anchor, collapsed descendants, stale snapshot invalidation, and immutable prior range reads. Consumer tests must exercise SetupElementsTree's real event/getter route and SetupPatchingSimple's cache and source preparation, rather than reproduce the selection-order algorithm. Existing STA fixtures, isolated global managers, nonparallel test collection, and output test stub can be reused without broad fixture extraction.

Verify preferred Patching Order and Reverse Element Order retain their current precedence, controller destinations retain VIX-4006 normalization, and selecting alone changes no connections. Full-MSBuild test-target compilation is required before dotnet test --no-build because the test project has C++/CLI dependencies. Run Rider file-problem checks only against changed C# files and resolve only changed-line diagnostics. Manual verification must prove actual patch connections in both range directions and retain VIX-938 navigation and large-tree responsiveness.

## Recommendation


Proceed with the additive range-order snapshot and existing consumer handoff described in the ExecPlan. A global removal of NodesInPreviousVisibleRange's reversal is not recommended because it changes the established order seen by unrelated tree consumers. An event-only change is insufficient because switching patching views obtains selection through the getter. No additional architecture refactor is needed.

The user has authorized planning and issue creation in this turn. Application implementation, test results, and final completion reporting remain future ExecPlan milestones.

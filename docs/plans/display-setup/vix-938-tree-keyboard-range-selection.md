# Fix VIX-938 Display Setup Tree Keyboard Range Selection

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md` from the repository root. It is self-contained so a future implementer can work from this file alone.

## Purpose / Big Picture

Display Setup users should be able to select a contiguous range of items in the Element Tree and Controller Tree using the keyboard, the same way they expect in common Windows tree controls. After this change, a user can click or arrow to one visible tree node, hold Shift, press Down or Up, and watch the selected range grow or shrink through the visible tree rows without using the mouse.

The visible result is in Display Setup, which hosts `Common.Controls.ElementTree` and `Common.Controls.ControllerTree`. Both controls use the same shared `Common.Controls.MultiSelectTreeview` implementation. Fixing that shared tree control restores keyboard range selection for both trees and any other host that uses `MultiSelectTreeview`.

## Progress

- [x] (2026-07-22 00:00 America/Chicago) Read `.agents/PLANS.md` and the project instructions for plans, docs, and C# changes.
- [x] (2026-07-22 00:00 America/Chicago) Read the project `dotnet-best-practices`, `catel-mvvm`, and `jira` skills because this task touches C#/WinForms controls hosted in Display Setup and must include a JIRA update step.
- [x] (2026-07-22 00:00 America/Chicago) Inspected `src/Vixen.Common/Controls/ElementTree.cs`, `ElementTree.Designer.cs`, `ControllerTree.cs`, `ControllerTree.Designer.cs`, and `MultiSelectTreeview.cs`.
- [x] (2026-07-22 00:00 America/Chicago) Identified the likely root cause: `MultiSelectTreeview.OnKeyDown` returns immediately when Shift or Ctrl is held, so `Shift+Up` and `Shift+Down` never reach the code that could extend selection to `PrevVisibleNode` or `NextVisibleNode`.
- [x] (2026-07-22 00:00 America/Chicago) Created this initial ExecPlan under `docs/plans/display-setup/`.
- [x] (2026-07-22 00:00 America/Chicago) Clarified behavior questions with the user: reversed Shift+arrow shrinks the range like Windows Explorer; Ctrl+Up/Down do nothing; Shift+Home/End select to top/bottom; Shift+Down with no selection selects `TopNode`.
- [ ] Implement the shared keyboard range-selection fix.
- [ ] Add focused automated coverage or document why UI-only coverage is the practical limit.
- [ ] Run focused validation and manual Display Setup acceptance.
- [ ] Update JIRA issue VIX-938 with the final cause, remediation, and test plan/results.

## Surprises & Discoveries

- Observation: The requested folder `docs/plans/display-setup` did not exist in this checkout.
  Evidence: `Get-ChildItem -LiteralPath docs\plans\display-setup` failed with "Cannot find path"; this plan creates that folder by adding this file under it.

- Observation: `ElementTree` and `ControllerTree` are WinForms user controls in `src/Vixen.Common/Controls`, not WPF controls.
  Evidence: both designer files instantiate `Common.Controls.MultiSelectTreeview`, and `src/Vixen.Common/Controls/Controls.csproj` sets `UseWindowsForms` to `true`.

- Observation: The two reported trees share the same underlying keyboard-selection implementation.
  Evidence: `src/Vixen.Common/Controls/ElementTree.Designer.cs` constructs `treeview = new MultiSelectTreeview()` and wires `treeview.KeyDown += treeview_KeyDown`; `src/Vixen.Common/Controls/ControllerTree.Designer.cs` constructs `this.treeview = new Common.Controls.MultiSelectTreeview()` and wires the same underlying control's events.

- Observation: The current `OnKeyDown` logic contains unreachable Shift range-selection branches for Home and End and blocks Shift arrow range-selection entirely.
  Evidence: in `src/Vixen.Common/Controls/MultiSelectTreeview.cs`, `OnKeyDown` calls `base.OnKeyDown(e)` and then immediately returns when `e.Control || e.Shift` is true. The next line computes `bool bShift = (ModifierKeys == Keys.Shift)`, and later Home/End test `bShift`, but those branches cannot run while Shift is held. Up and Down call `SelectNode(m_SelectedNode.PrevVisibleNode)` or `SelectNode(m_SelectedNode.NextVisibleNode)`, and `SelectNode` already contains range-selection behavior when `ModifierKeys == Keys.Shift`, but the early return prevents that code from running for `Shift+Up` and `Shift+Down`.

## Decision Log

- Decision: Fix `MultiSelectTreeview` instead of adding separate key handlers in `ElementTree` and `ControllerTree`.
  Rationale: Both reported controls host the same shared treeview, and the defect is in the shared key handler. A shared fix keeps behavior consistent and avoids duplicated selection rules that could diverge.
  Date/Author: 2026-07-22 / Codex

- Decision: Treat selection range order as visible tree order, using `TreeNode.PrevVisibleNode` and `TreeNode.NextVisibleNode`.
  Rationale: Common Windows tree controls navigate through what the user can see. Collapsed descendants should not be selected by Shift+arrow because they are not visible rows.
  Date/Author: 2026-07-22 / Codex

- Decision: Use a stable selection anchor for Shift range selection.
  Rationale: The user confirmed reversed Shift+arrow should shrink the selected range like Windows Explorer. The current `SelectNode(TreeNode node)` implementation starts from `m_SelectedNode`, and `ToggleNode` updates `m_SelectedNode` as nodes are added. That risks moving the anchor while extending a range. A stable anchor records the node where range selection began, while a separate focus/current node moves as the user presses Shift+Up/Down or Shift+Home/End.
  Date/Author: 2026-07-22 / Codex

- Decision: Leave Ctrl+Up and Ctrl+Down unhandled.
  Rationale: The user explicitly chose no action for Ctrl+arrow in these trees. This avoids introducing a focus-only concept that the current fake multi-selection tree does not expose.
  Date/Author: 2026-07-22 / user, recorded by Codex

- Decision: Restore Shift+Home and Shift+End with Windows Explorer-style range behavior.
  Rationale: The user confirmed these should select the visible range to the top or bottom. The current code already has intended Shift+Home/End branches, but they are unreachable because `OnKeyDown` returns when Shift is held.
  Date/Author: 2026-07-22 / user, recorded by Codex

- Decision: If no node is selected, Shift+Down selects `TopNode`.
  Rationale: The user explicitly chose `TopNode` for the no-selection Shift+Down case. This matches the current plain-arrow fallback more closely than inferring the first root node.
  Date/Author: 2026-07-22 / user, recorded by Codex

## Outcomes & Retrospective

This initial plan records the cause and a concrete remediation path. Implementation has not started. Update this section after the fix is coded, tests run, manual Display Setup behavior is observed, and VIX-938 is updated.

## Answered Questions

These questions were answered by the user on 2026-07-22 and are retained as requirements:

1. When Shift+Down selects A through C and the user then presses Shift+Up, the range should shrink back toward the original anchor, like Windows Explorer list/tree range selection.

2. Ctrl+Up and Ctrl+Down should do nothing.

3. Shift+Home and Shift+End should be restored at the same time as Shift+Up and Shift+Down. They should select all visible nodes to the top or bottom, like Windows Explorer.

4. If no node is selected and the user presses Shift+Down, the tree should select `TopNode`. Shift+Up with no selection should not wrap; use the same initial `TopNode` fallback and then remain there if there is no previous visible node.

Implementation should not re-ask these questions unless new source evidence shows these choices conflict with existing required behavior.

## Context and Orientation

Vixen is a Windows desktop application. Display Setup lives under `src/Vixen.Application/Setup`, and it hosts reusable controls from `src/Vixen.Common/Controls`. The two controls named in VIX-938 are `ElementTree` and `ControllerTree`.

`ElementTree` is the left-side tree of element nodes, such as props, groups, and pixels. Its files are `src/Vixen.Common/Controls/ElementTree.cs` and `src/Vixen.Common/Controls/ElementTree.Designer.cs`. The designer creates a private field named `treeview` of type `MultiSelectTreeview`. `ElementTree` adds element-specific behavior such as add, delete, rename, tags, copy/paste, drag/drop, and exporting.

`ControllerTree` is the tree of output controllers and controller outputs. Its files are `src/Vixen.Common/Controls/ControllerTree.cs` and `src/Vixen.Common/Controls/ControllerTree.Designer.cs`. Its designer also creates a private field named `treeview` of type `MultiSelectTreeview`. `ControllerTree` adds controller-specific behavior such as configure, rename, delete, start, stop, insert outputs, remove outputs, unpatch, and reorder.

`MultiSelectTreeview` is the custom multi-selection TreeView implementation in `src/Vixen.Common/Controls/MultiSelectTreeview.cs`. WinForms `TreeView` normally has only one selected node. This class fakes multiple selection by keeping a `List<TreeNode>` named `m_SelectedNodes`, hiding the native `SelectedNode` property behind its own `SelectedNode`, canceling native selection in `OnBeforeSelect`, and painting selected nodes by setting each node's `BackColor` and `ForeColor`.

The important current methods are:

- `OnKeyDown(KeyEventArgs e)`, which handles arrow, Home, End, PageUp, PageDown, and type-to-search keyboard behavior.
- `SelectNode(TreeNode node)`, which already knows how to extend selection when `ModifierKeys == Keys.Shift`.
- `SelectSingleNode(TreeNode node, bool notify = true)`, which clears all selected nodes and selects one.
- `ToggleNode(TreeNode node, bool bSelectNode)`, which adds or removes a tree node from the fake multi-selection list and updates colors.
- `TreeNodeSorter`, which keeps selected nodes sorted in tree display order.

The root cause is localized to `OnKeyDown`. It returns for any Shift or Ctrl key state before it handles navigation:

    base.OnKeyDown(e);
    if (e.Control || e.Shift) return;
    bool bShift = (ModifierKeys == Keys.Shift);

Because of this, `Shift+Up`, `Shift+Down`, `Shift+Home`, and `Shift+End` never execute the selection code below. The later `bShift` branches prove a previous implementation expected Shift-selection to work, but the early return now makes that impossible.

## Plan of Work

First, update `src/Vixen.Common/Controls/MultiSelectTreeview.cs` so `OnKeyDown` no longer returns for all Shift-modified keys. Keep Ctrl+Up and Ctrl+Down unhandled because the user explicitly chose no action for those keys. Replace `if (e.Control || e.Shift) return;` with logic that returns for Ctrl, Alt, and unsupported Shift combinations, but allows Shift with Up, Down, Home, and End to flow into the tree's range-selection code.

The implementation should make `OnKeyDown` easy to reason about. Compute local values such as `bool isShiftOnly = e.Shift && !e.Control && !e.Alt;` and `bool isUnmodified = !e.Shift && !e.Control && !e.Alt;`. Handle normal navigation for unmodified keys. Handle range navigation for Shift+Up, Shift+Down, Shift+Home, and Shift+End by selecting the visible range between a stable anchor node and the target visible node. Mark the event handled and suppress the key press only when the tree actually processes the key.

Second, add a separate private anchor field such as `_selectionAnchorNode` if the existing fields cannot already represent both anchor and focus. Set the anchor on unmodified single selection and Ctrl selection when appropriate. Keep it stable during Shift range selection. Track the moving focus/current node separately so repeated Shift+Down extends the range and Shift+Up shrinks it back toward the anchor. Select exactly the visible nodes between the anchor and the target visible node, inclusive, and clear nodes outside that range during Shift-keyboard range updates.

Third, preserve and align mouse behavior. Shift-click selection in `SelectNode` currently walks visible nodes between the selected node and the clicked node, including across different parents. Do not remove that behavior. If an anchor field is introduced, use it consistently for both Shift-click and Shift+keyboard so users see one Windows-style range-selection model.

Fourth, add focused automated coverage if practical. Prefer a new test file `src/Vixen.Tests/Common/MultiSelectTreeviewKeyboardSelectionTests.cs`. Because `OnKeyDown` is protected and WinForms modifier state normally comes from actual keyboard state, the best testable design may be to extract the core selection operation into an internal method that accepts the key and modifier state explicitly, for example `ProcessSelectionKey(Keys keyCode, Keys modifiers)`. If that method is internal, add `InternalsVisibleTo` only if the repository already has a convention for it; otherwise keep it private and expose behavior through a small test subclass that calls protected methods if feasible. Tests should instantiate `MultiSelectTreeview`, add simple `TreeNode`s, select one, invoke the keyboard-selection path, and assert `SelectedNodes` contents.

Fifth, manually verify in Display Setup. Because this is a WinForms control behavior where the exact modifier state and focus routing matter, manual verification is required even if unit tests pass. Verify both Element Tree and Controller Tree, including collapsed branches and selection across parent/child boundaries.

Sixth, update JIRA issue VIX-938 after implementation and validation. The JIRA update must include the cause, the remediation steps actually taken, the automated test names and results, and the manual test plan/results.

## Concrete Steps

Work from the repository root:

    cd C:\Dev\Vixen
    git status --short

Read the relevant files before editing:

    Get-Content src\Vixen.Common\Controls\MultiSelectTreeview.cs
    Get-Content src\Vixen.Common\Controls\ElementTree.cs
    Get-Content src\Vixen.Common\Controls\ElementTree.Designer.cs
    Get-Content src\Vixen.Common\Controls\ControllerTree.cs
    Get-Content src\Vixen.Common\Controls\ControllerTree.Designer.cs
    Get-Content src\Vixen.Tests\Vixen.Tests.csproj

Find the exact key-selection code:

    rg -n "OnKeyDown|SelectNode|SelectSingleNode|ToggleNode|SelectedNodes|ModifierKeys|Keys.Up|Keys.Down|Keys.Home|Keys.End" src\Vixen.Common\Controls\MultiSelectTreeview.cs

Implement the shared fix in:

    src\Vixen.Common\Controls\MultiSelectTreeview.cs

If tests are feasible, add them in:

    src\Vixen.Tests\Common\MultiSelectTreeviewKeyboardSelectionTests.cs

Run focused tests first:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~MultiSelectTreeviewKeyboardSelection --no-restore

Run a broader common-controls compile/test check:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~Vixen.Tests.Common --no-restore

Then run the full test project if dependencies are restored:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Run whitespace validation:

    git diff --check

If a project build is needed because tests cannot cover the WinForms control path, run:

    dotnet msbuild src\Vixen.Common\Controls\Controls.csproj -t:Rebuild -p:Configuration=Debug -p:UseSharedCompilation=false

Expected successful test output should include zero failures. The exact count may vary, but the summary should resemble:

    Passed!  - Failed: 0, Passed: <N>, Skipped: 0, Total: <N>

## Validation and Acceptance

Automated acceptance, if the WinForms control path can be tested, should prove:

- Given nodes A, B, and C are visible and B is selected, when the tree processes Shift+Down, then B and C are selected.
- Given nodes A, B, and C are visible and B is selected, when the tree processes Shift+Up, then A and B are selected.
- Given A is a collapsed parent with hidden children and B is the next visible node, when the tree processes Shift+Down from A, then B is selected and hidden descendants are not selected directly.
- Given the first visible node is selected, Shift+Up does not throw and does not wrap to the bottom.
- Given the last visible node is selected, Shift+Down does not throw and does not wrap to the top.
- Given Shift+Down selects B through D from an anchor at B, when Shift+Up is pressed once, then the selected range shrinks to B through C.
- Given Shift+Home is processed from a middle visible node, the target becomes the first visible node and the selected range is exactly the visible nodes between the stable anchor and that first visible node, inclusive.
- Given Shift+End is processed from a middle visible node, the target becomes the last visible node and the selected range is exactly the visible nodes between the stable anchor and that last visible node, inclusive.
- Given no node is selected and Shift+Down is processed, `TopNode` is selected if it exists.
- Given Ctrl+Up or Ctrl+Down is pressed, selection and focus remain unchanged.
- Given plain Up and plain Down are pressed, existing single-selection navigation still works.
- Given Delete, Ctrl+C, Ctrl+V, Ctrl+Shift+V in `ElementTree`, and Delete in `ControllerTree`, the host-specific key handlers still receive the shortcuts they own.

Manual acceptance requires these Display Setup scenarios:

1. Open Display Setup and focus the Element Tree. Select a middle visible element node. Hold Shift and press Down twice. The selection grows through the next two visible nodes.
2. Continue holding Shift and press Up once. Confirm the selection shrinks back toward the original anchor, matching Windows Explorer-style range selection.
3. In the Element Tree, collapse a group, select a visible node above it, then use Shift+Down across the collapsed group. Hidden descendants are not selected directly.
4. In the Element Tree, select a parent node and use Shift+Down into its visible children. The selected range follows the visible tree order.
5. Repeat equivalent Shift+Up and Shift+Down checks in the Controller Tree with controller output rows visible.
6. In the Controller Tree, select a controller row and use Shift+Down into output rows if that is allowed by existing selection rules. Confirm existing context menu behavior still correctly distinguishes selected controllers from selected output rows.
7. Press Delete in both trees with selected items and confirm the existing delete prompts still appear as before.
8. Use mouse Shift-click in both trees and confirm it still behaves consistently with Shift+keyboard range selection.

## JIRA Update

After implementation and validation, update JIRA issue VIX-938 with a comment that includes cause, remediation, and test plan/results. Use the Atlassian/JIRA tool if available in the execution environment.

The comment should include this cause summary, adjusted if implementation reveals more details:

    Cause:
    ElementTree and ControllerTree both use Common.Controls.MultiSelectTreeview. MultiSelectTreeview.OnKeyDown returns immediately when Shift or Ctrl is held. That prevents Shift+Up and Shift+Down from reaching the existing visible-node navigation and range-selection code. The same early return also makes the existing Shift+Home and Shift+End branches unreachable.

The remediation section should name the actual files changed and describe the final behavior:

    Remediation:
    Updated src/Vixen.Common/Controls/MultiSelectTreeview.cs so Shift-modified navigation keys are processed by the shared tree control. Shift+Up and Shift+Down use a stable anchor so the range grows and shrinks like Windows Explorer. Shift+Home and Shift+End select to the top or bottom visible node. Ctrl+Up and Ctrl+Down remain unhandled. Existing plain arrow navigation and host-owned shortcuts remain unchanged.

The test plan section should include the automated command output and manual scenarios:

    Test plan:
    - Run focused MultiSelectTreeview keyboard-selection tests.
    - Run Vixen.Tests common tests or the full test project as feasible.
    - Run git diff --check.
    - Manually verify Shift+Up and Shift+Down in Display Setup Element Tree.
    - Manually verify Shift+Up and Shift+Down in Display Setup Controller Tree.
    - Verify collapsed descendants are skipped because selection follows visible rows.
    - Verify existing Delete/copy/paste shortcuts and context menu enablement still work.

## Idempotence and Recovery

The implementation should be limited to the shared tree control and focused tests. Re-running tests and builds is safe. If the initial implementation only changes `OnKeyDown` and manual testing shows reversed Shift selection does not shrink correctly, keep the buildable intermediate state, record the discovery in this plan, and then add an explicit range-selection anchor as a second step.

Do not revert unrelated work in the repository. If the WinForms designer rewrites unrelated layout or resource data, inspect the diff carefully and remove unrelated designer churn before finalizing. Prefer manual code edits through `apply_patch` for `MultiSelectTreeview.cs`; do not open the designer unless absolutely necessary.

If automated tests are blocked by WinForms keyboard modifier state, document the blocker in `Surprises & Discoveries` and rely on a combination of extracted pure helper tests plus manual Display Setup validation. Do not add brittle UI automation unless the extracted helper approach cannot validate the core selection behavior.

## Artifacts and Notes

Key evidence from the current source:

    src/Vixen.Common/Controls/MultiSelectTreeview.cs
    OnKeyDown:
        base.OnKeyDown(e);
        if (e.Control || e.Shift) return;
        bool bShift = (ModifierKeys == Keys.Shift);
        ...
        else if (e.KeyCode == Keys.Up) {
            if (m_SelectedNode.PrevVisibleNode != null) {
                SelectNode(m_SelectedNode.PrevVisibleNode);
            }
        }
        else if (e.KeyCode == Keys.Down) {
            if (m_SelectedNode.NextVisibleNode != null) {
                SelectNode(m_SelectedNode.NextVisibleNode);
            }
        }

    src/Vixen.Common/Controls/MultiSelectTreeview.cs
    SelectNode:
        else if (ModifierKeys == Keys.Shift) {
            // Shift+Click selects nodes between the selected node and here.
            TreeNode ndStart = m_SelectedNode;
            TreeNode ndEnd = node;
            ...
            ToggleNode(ndStart, true);
        }

This evidence means the bug is not in Display Setup itself. Display Setup exposes the problem because both reported trees are hosted there, but the regression-causing behavior is in the shared `MultiSelectTreeview` keyboard handler.

## Interfaces and Dependencies

The primary type at the end of this work remains:

    namespace Common.Controls
    {
        public class MultiSelectTreeview : TreeView
        {
            public List<TreeNode> SelectedNodes { get; set; }
            public new TreeNode SelectedNode { get; set; }
            protected override void OnKeyDown(KeyEventArgs e);
        }
    }

If a testable helper is introduced, keep it inside `MultiSelectTreeview` unless reuse becomes necessary. A possible shape is:

    private bool ProcessKeyboardSelection(Keys keyCode, Keys modifiers)

or, if tests require direct access and the repository accepts internal testable APIs:

    internal bool ProcessKeyboardSelection(Keys keyCode, Keys modifiers)

Do not introduce WPF dependencies for this fix. `ElementTree` and `ControllerTree` are WinForms controls, and the defect lives in a WinForms `TreeView` subclass.

## Revision Notes

- 2026-07-22 / Codex: Created the initial plan after source inspection. The plan records the root cause, open behavior questions, shared-control remediation, validation expectations, and the required VIX-938 JIRA update step.
- 2026-07-22 / Codex: Updated the plan with the user's behavior answers. The implementation now requires Windows Explorer-style anchored Shift range selection, restored Shift+Home/End, no Ctrl+arrow action, and `TopNode` selection for no-selection Shift+Down.

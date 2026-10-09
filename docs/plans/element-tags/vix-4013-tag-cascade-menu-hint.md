# Show the Element Tag cascade instruction once per menu (VIX-4013)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective as work proceeds. Follow `.agents/PLANS.md` and the project versions of skills under `.agents/skills/`. This document records a design; application implementation and Jira updates have not started.

## Purpose / Big Picture


Users should discover that holding Ctrl while choosing an Element Tag cascades the operation to child nodes. Display Setup, Preview Setup, and the Sequencer will show one instruction at the top of their existing Tags submenu, followed by a separator and the normal tag choices. The instruction will neither highlight on hover nor participate in keyboard selection. Individual tags retain their existing names without repeated modifier hints.

The exact instruction is `💡 Hold Ctrl to cascade tag to child nodes`. Use a standard non-link `ToolStripLabel`, rather than the disabled `ToolStripMenuItem` originally requested in Jira. This follows the user's clarification on 2026-10-09: avoid hover highlighting and complex overrides while keeping the modifier text out of individual tag entries.

## Progress


- [x] (2026-10-09 19:16Z) Read VIX-4013, its empty attachment/comment lists, the planning skill, `.agents/PLANS.md`, the relevant Element Tags workflow specification, and both menu-population methods.
- [x] (2026-10-09 19:16Z) Resolve the disabled-menu-item hover conflict by selecting a standard non-link label in response to the user's preference.
- [x] (2026-10-09 19:16Z) Design the two small menu insertions and the manual validation scenarios.
- [ ] Milestone 1: Align Jira with the clarified label behavior when external updates are authorized.
- [ ] Milestone 2: Add and validate the instruction in both existing menu-population methods.
- [ ] Milestone 3: Record final acceptance and validation results in Jira when external updates are authorized.

## Surprises & Discoveries


The prescribed disabled `ToolStripMenuItem` does not fully meet the original non-highlight requirement. .NET 10 WinForms sets `SupportsDisabledHotTracking = true` in `ToolStripMenuItem.Initialize`; `ToolStripItem.HandleMouseEnter` can call `Select()` for a disabled item when that flag is true. Disabling activation alone therefore does not guarantee a nonselectable mouse-hover header.

A standard `ToolStripLabel` supplies the needed behavior. At runtime its `CanSelect` getter returns false when `IsLink` is false; its expression is `IsLink || DesignMode`. Set `Enabled = false` as well to suppress activation. No custom subclass, renderer, event workaround, or new control is needed. Verified against the official .NET 10 source:

[ToolStripMenuItem initialization](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Controls/ToolStrips/ToolStripMenuItem.cs#L181-L190), [ToolStripItem mouse handling](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Controls/ToolStrips/ToolStripItem.cs#L2275-L2303), and [ToolStripLabel selection](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Controls/ToolStrips/ToolStripLabel.cs#L60-L63).

The older `docs/plans/element-tags/vix-2690-element-tags-workflow-requirements.md` describes the Sequencer row-label surface. Current menu construction is in `TimelineControl.PopulateTagsMenu`, not `TimedSequenceRowLabel`. The shared `ElementTree` still covers Display Setup and Preview Setup. Use the current source locations below while retaining the specification's user-facing scope.

The inspected click closures already read Ctrl at the moment of activation and call the existing toggle methods. This issue adds discoverability rather than a new cascade algorithm.

The initial working tree was clean. The ElementTree impact query reported one direct dependent, its context-menu opening method, and no test files. The TimelineControl impact traversal reached its budget, so its result is incomplete and cannot establish that no further callers or tests exist. Rider test discovery did not return usable coverage evidence before cancellation; do not claim existing coverage or successful tests from this analysis.

## Decision Log


- Decision: Use a standard `ToolStripLabel` with `IsLink = false` and `Enabled = false` for the header.
  Rationale: The user prefers no hover highlighting without complex overrides. This standard item meets that behavior and changes only the originally prescribed header type.
  Date/Author: 2026-10-09 / Codex, based on the user's clarification.

- Decision: Keep the exact instruction at index 0 and a `ToolStripSeparator` at index 1 in each Tags submenu.
  Rationale: A single instruction per menu is visible before choosing a tag and avoids repeating modifier text on each entry.
  Date/Author: 2026-10-09 / Codex.

- Decision: Make the insertions independently in the existing population methods, without a shared helper or public API change.
  Rationale: Two small standard-component insertions do not justify an abstraction. Keeping the surrounding source intact satisfies the issue's handler and enumeration guardrails.
  Date/Author: 2026-10-09 / Codex.

- Decision: Validate presentation and interaction manually and build the Release x64 solution. Do not introduce tests that merely restate the item initializer.
  Rationale: Hover, keyboard navigation, glyph display, theme legibility, and repeated menu opening are the meaningful acceptance checks. No cascade implementation is changing.
  Date/Author: 2026-10-09 / Codex.

- Decision: Produce the local design now; defer application edits and external Jira writes to execution.
  Rationale: The user requested a solution design. The initial and final Jira milestones required by `.agents/PLANS.md` remain explicit future work, without treating the planning request as authorization to modify Jira.
  Date/Author: 2026-10-09 / Codex.

## Outcomes & Retrospective


Analysis resolves the original hover conflict using an existing framework component. The implementation is limited to two menu-population methods in Vixen.Common. This plan is the only requested repository artifact; no application behavior has changed and no runtime validation has been performed. The lesson is that a disabled command item and a nonselectable informational label have different WinForms behavior.

## Context and Orientation


Vixen is a Windows .NET 10 application with WPF and existing WinForms editor controls. This change maintains existing WinForms context menus; it does not create a new UI stack. Rider automation is available. The project `dotnet-best-practices` skill permits maintenance of legacy WinForms surfaces. Catel, async, design-pattern, and XML-documentation skills are unnecessary for these private-method insertions because no view model, asynchronous flow, interface, lifecycle, or public/protected API changes are planned.

`src/Vixen.Common/Controls/ElementTree.cs` contains `ElementTree.PopulateTagsMenu()` around line 1008. It clears `tagsToolStripMenuItem.DropDownItems`, obtains selected nodes, enumerates `ElementTagService.Instance.GetAll()`, and adds each tag as a menu item. Each click calls `ToggleTagOnSelectedNodes` with the tag identifier and the current Ctrl modifier. A final separator and `Manage Tags...` item follow the tag choices. Display Setup and Preview Setup embed this shared tree control.

`src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` contains `TimelineControl.PopulateTagsMenu(ToolStripMenuItem tagsMenuItem)` around line 942. It clears `tagsMenuItem.DropDownItems`, gets selected row nodes, preserves parent-menu enablement based on selection count, and enumerates the same catalog. Each tag click calls `ToggleTagOnSelectedRows` with the current Ctrl modifier. It likewise appends a separator and `Manage Tags...`.

A tag is metadata assigned to an element node. A child node is a node below the selected group in the tree. Cascading here means passing the existing Ctrl request to the existing tag operation for descendants; it does not introduce tag inheritance. A submenu is the list opened from the existing Tags entry. The header belongs inside that submenu, rather than at the top of the entire tree context menu.

## Architecture Design: VIX-4013


Detected IDE Environment: Rider with automation hooks. Use Rider's Apply snippet from chat workflow for the two small blocks if implementing manually, and its file-problem analysis for the modified files. Agents must still perform repository navigation and mutations through the mandatory Gortex workflow.

Core Strategy: Prepend one standard informational label and one standard separator immediately after each existing `DropDownItems.Clear()`. Because the collection is empty at that point, adding them gives indices 0 and 1 without altering the tag enumeration loop or any functional array binding. Both menus rebuild on opening, so the header is recreated once per rebuild rather than accumulating.

Data Model & Property Contracts: No new persisted fields, settings, services, interfaces, dependencies, or API signatures. The header is a `ToolStripLabel` with the exact text, `Enabled = false`, `IsLink = false`, and `TextAlign = ContentAlignment.MiddleLeft`. Attach no click or paint handler, shortcut, check state, or tag identifier to it. Leave the functional items untouched.

Mathematical / Boundary Logic: There is no new mathematical algorithm or wrap-around rule. Position is determined by insertion into the just-cleared collection. Parent submenu enablement remains governed by existing selection logic. If the catalog is empty, the header and its separator still precede the existing trailing separator and Manage Tags entry; do not add special-case enumeration or separator cleanup as part of this issue. Reopening must never duplicate headers. Verify mixed selections and leaf nodes through existing behavior rather than changing it.

Subsystem Component Matrix: `ElementTree.PopulateTagsMenu()` changes the submenu presentation in Display Setup and Preview Setup. `TimelineControl.PopulateTagsMenu(ToolStripMenuItem)` changes the submenu presentation in the Sequencer. The catalog service, node tag collections, toggle handlers, selection logic, refresh/filter events, designer files, and rendering/output loops receive no planned modifications.

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Milestone 1: Align Jira with the clarified instruction behavior


Context: VIX-4013 currently names a disabled menu item while also requiring no hover highlight. The user's clarification favors a simple standard label. This milestone aligns the external issue with the behavior described here before application implementation.

Plan of Work: When Jira writes are authorized, use the project `jira` skill to update the issue description with Summary, Scope, and Acceptance Criteria. Describe one static instruction above a separator in Display Setup/Preview Setup and the Sequencer, no hover highlighting or keyboard selection, and unchanged tag choices and Ctrl behavior. Keep technical type names, source paths, and implementation details in this plan. The user-facing test plan is to open each Tags submenu, inspect its layout, hover and navigate by keyboard, reopen it, and compare ordinary versus Ctrl tag operations. Read the current issue first and preserve any intervening user requirements.

Concrete Steps: Fetch VIX-4013 through Atlassian MCP, prepare the description from the observable acceptance below, apply only the authorized description update, and read it back. There is no shell command that should substitute for the Jira connector. Do not change status, assignee, or unrelated fields.

Validation and Acceptance: The issue should describe the static instruction and the same interaction checks as this plan, without prescribing a disabled command item whose hover behavior contradicts acceptance.

STOP HERE for manual review and commit execution before proceeding. Halt execution and record the stopping point in Progress. Run `git status --short` in `C:\Dev\Vixen` and review the scoped plan diff if this milestone changed the plan. For repository changes, invoke the project `commit-msg` skill and supply a paste-ready VIX-4013-prefixed message under Commit message. Do not create a commit unless explicitly requested. Wait for explicit confirmation before advancing, as required by the invoked planning skill.

### Milestone 2: Add the static instruction to both menus and verify the result


Context: Modify only `src/Vixen.Common/Controls/ElementTree.cs` and `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`, specifically their private `PopulateTagsMenu` methods. At completion, both submenus present the same nonselectable header and separator above the untouched tag list.

Plan of Work: Inspect `git status --short` first and preserve unrelated edits. Recall prior decisions, use Gortex `explore(operation: "task")` for the scoped implementation, and inspect the actual methods before editing. Call `change(operation: "impact")` on each method. No signature verification is needed because declarations stay unchanged. Mutate only through Gortex edit/refactor. Retain tabs and LF; do not reformat surrounding blocks.

In `ElementTree.PopulateTagsMenu()`, insert the following immediately after `tagsToolStripMenuItem.DropDownItems.Clear();`. Apply only this block through Rider's Apply snippet from chat workflow when working manually:

    tagsToolStripMenuItem.DropDownItems.Add(new ToolStripLabel("💡 Hold Ctrl to cascade tag to child nodes")
    {
        Enabled = false,
        IsLink = false,
        TextAlign = ContentAlignment.MiddleLeft
    });
    tagsToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

In `TimelineControl.PopulateTagsMenu(ToolStripMenuItem tagsMenuItem)`, insert the following immediately after `tagsMenuItem.DropDownItems.Clear();`:

    tagsMenuItem.DropDownItems.Add(new ToolStripLabel("💡 Hold Ctrl to cascade tag to child nodes")
    {
        Enabled = false,
        IsLink = false,
        TextAlign = ContentAlignment.MiddleLeft
    });
    tagsMenuItem.DropDownItems.Add(new ToolStripSeparator());

These snippets show four-space Markdown indentation; the C# inserted into the existing methods must use the repository's tab indentation. Neither method declaration changes. Preserve every pre-existing statement: selection retrieval, parent enablement, catalog enumeration, check-state calculation, click closures, color-dot painting, tag-item append calls, trailing separator, and Manage Tags entry. Do not set shortcut text or append modifier text to tag names. Do not introduce a custom label subclass or a new shared builder.

Concrete Steps: After editing, call Gortex `change(operation: "detect")` and use its changed-symbol IDs for `tests`, `guards`, and `contract`. Run Rider `get_file_problems` separately for each changed file with `rootFolder = C:\Dev\Vixen`; resolve only problems on changed lines. If useful, use Rider `findTests` for the two production classes and their PopulateTagsMenu methods, supplying the actual namespace from current source rather than guessing it. Inspect and run any relevant existing tests it identifies, but do not treat an empty or timed-out response as passing coverage.

Run the following from `C:\Dev\Vixen` in a shell with full Visual Studio MSBuild available:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64

Expected outcome is exit code 0 and a Build succeeded result; warning counts may depend on the existing repository state. Launch the Release application through the existing Rider run configuration, configured to use the new output under `Release/Output/`, and execute Validation and Acceptance below using a disposable profile. Do not claim an expected transcript as an actual result; record the command, exit code, and manual observations in this living plan.

If relevant existing unit tests need rebuilding, the repository requires full MSBuild for its C++/CLI dependencies, followed by execution of the built tests. Use these commands from `C:\Dev\Vixen`, adding an appropriate test filter to the second command only if relevant tests have been identified:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/

This test workflow is conditional on meaningful existing tests, not a requirement to add UI-construction tests. Do not use dotnet test alone to build the solution's C++/CLI dependencies.

Validation and Acceptance: All manual scenarios below must pass. Record any inability to launch or inspect the UI as a validation gap rather than asserting hover behavior from compilation. A targeted diff must show only the two new header/separator blocks plus plan updates.

STOP HERE for manual review and commit execution before proceeding. Halt execution and update Progress. Run `git status --short` and scoped `git diff` for the two source files and this plan. Invoke the project `commit-msg` skill with VIX-4013 as the subject prefix and output a complete paste-ready message under Commit message. Do not create a commit unless explicitly requested. Wait for explicit confirmation before advancing, as required by the invoked planning skill.

### Milestone 3: Record final acceptance and validation in Jira


Context: Once implementation and verification are complete, VIX-4013 should describe the delivered user behavior and actual validation results. This milestone changes the external issue only when authorized and keeps this plan's outcome current.

Plan of Work: Read the issue again through Atlassian MCP, make any necessary user-facing description adjustment, and add a short comment reporting the static header in the supported menus, preserved tag behavior, actual build/manual validation results, and any remaining gap. Use the project `jira` skill. Do not post internal class names or detailed algorithms in the issue description, and do not claim tests or UI checks that were not run. Do not transition the issue unless authorized.

Concrete Steps: Apply the authorized description/comment operations and read back their results. Update this plan's Progress and Outcomes & Retrospective with actual completion evidence.

Validation and Acceptance: The issue and local plan agree about delivered behavior and accurately distinguish completed checks from skipped checks.

STOP HERE for manual review and commit execution before proceeding. Halt execution, run `git status --short` and the scoped plan diff, and invoke the project `commit-msg` skill for any repository change. Output a paste-ready VIX-4013-prefixed message under Commit message; do not commit without an explicit request. Wait for explicit confirmation before further work.

## Validation and Acceptance


Use a disposable profile with a group containing a child group and at least two leaf nodes, plus an unselected sibling group. Use the existing Prop tag for assignment checks so Hidden filtering and Deprecated restrictions do not obscure results. Confirm initial tag states before every comparison; repeat the fixture setup rather than relying on toggles to undo mixed states.

In Display Setup, Preview Setup, and the Sequencer, select a node or row, open its context menu, and open Tags. The first item displays exactly `💡 Hold Ctrl to cascade tag to child nodes`; the second item is a standard separator. The functional tag list follows, with its existing order, names, check states, dots, and Manage Tags entry. No tag name contains modifier instructions or new shortcut text. The label is readable and fully visible at the tested display scale and current theme; the bulb glyph need not render as a colored emoji, but must not be missing or clipped.

Hover the instruction: it must not display a selected-menu background or act as a command. Clicking it must not assign/remove a tag or open Manage Tags. Open the submenu by keyboard and navigate with arrow keys: focus must skip the header and separator and reach functional entries normally. Enter/Space must not activate the header. Verify the actual themed application rather than only a default-framework sample.

Close and reopen each submenu several times. Exactly one header and one separator before the tag list appear after every rebuild. Tag state changes still appear on the next opening. With no selection, preserve the existing parent-menu availability behavior.

From a known untagged fixture, select only the parent group and choose Prop without Ctrl. Observe the existing direct assignment on the selected group without newly tagging its unselected descendants. Reset the fixture, select only that parent, and choose Prop while Ctrl is held at activation. Observe the existing cascade to descendants; the unselected sibling branch stays unchanged. Repeat removal from a uniformly tagged group and repeat a multiple-selection operation to establish that the added header does not change those existing paths. A leaf selection continues to work with or without Ctrl.

If any unrelated baseline tag behavior fails, record it separately and avoid expanding this issue into a cascade refactor. The acceptance change here is the instruction's presentation and nonselection while preserving existing tag behavior.

## Idempotence and Recovery


Only add the two blocks once in source. At runtime the existing Clear calls make rebuilding idempotent: no duplicate instruction accumulates. Inspect current code before reapplying snippets and do not rely on absolute line numbers after edits. Preserve any overlapping user work and resolve uncertainty before writing.

Use a disposable profile for tag mutation checks. Do not reset or delete a user's working tree or real profile to recover. If validation fails, inspect the two inserted blocks and remove or adjust only those task-specific edits using the guarded Gortex workflow, keeping unrelated changes intact. Do not introduce custom rendering or selection overrides to compensate silently; record any framework/application mismatch and revise the design deliberately.

## Artifacts and Notes


The planned repository artifact is `docs/plans/element-tags/vix-4013-tag-cascade-menu-hint.md`. The final source diff should contain two small insertions, one per existing menu-population method. No designer file, tag catalog model, sequence serialization, or effect implementation is part of the design.

No application build, unit-test run, or interactive UI validation has been performed during planning. Source inspection and official framework-source verification support the design; execution must supply runtime acceptance evidence.

## Interfaces and Dependencies


Use the already referenced System.Windows.Forms types `ToolStripLabel` and `ToolStripSeparator`, and `System.Drawing.ContentAlignment.MiddleLeft`. Add no packages or project references. The existing declarations remain:

    private void PopulateTagsMenu()
    private void PopulateTagsMenu(ToolStripMenuItem tagsMenuItem)

The first declaration is owned by ElementTree; the second by TimelineControl. No public or protected C# API is added or modified, so no new XML-documentation work is required. The menu item owns its header/separator through its existing DropDownItems collection and existing menu lifecycle.

## Plan Revision Note


2026-10-09: Created this plan after the user clarified that a simple header without hover highlighting is preferred and modifier text must not be repeated on tag entries. The chosen ToolStripLabel replaces Jira's originally prescribed disabled ToolStripMenuItem because official .NET 10 source demonstrates their different selection behavior. This revision records the rationale, current source ownership, small implementation blocks, and observable validation before application execution.

Analysis complete and plan integrated with plans.md.

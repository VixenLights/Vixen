# Open the Patching Order Zig Zag dialog safely (VIX-4007)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with `.agents/PLANS.md`. Issue: https://vixenlights.atlassian.net/browse/VIX-4007. The design was prepared using `.agents/skills/analyze-and-plan-issue/SKILL.md`. This document records the analysis and implementation evidence. Milestone 1 updated VIX-4007, and milestone 2 implemented the caller-side dialog fix.

## Purpose / Big Picture


In Display Setup, a user can configure Patching Order, select multiple elements in its list, and choose Zig Zag from the context menu. The numeric dialog must open even when fewer than 50 elements are selected. After the repair, the initial zig-zag length is the smaller of 50 and the selection count, and the user can choose any length from 2 through that count. Confirming a valid length keeps the existing alternating-group reversal behavior. Canceling the numeric dialog leaves the order untouched.

Demonstrate the repair with ten elements: select all ten in Patching Order and choose Zig Zag. Before the repair, creating the dialog throws an exception because its value is 50 and its maximum is 10. Afterward, it opens with value 10, minimum 2, and maximum 10. Choose length 2 and confirm to see every second pair reverse.

The adjacent keyboard-selection repair adds a second observable outcome: pressing Ctrl alone must leave the current selection intact, Ctrl-click must add or remove only the clicked row, and Ctrl+A must still select all rows. Plain A must not invoke Select All. Demonstrate this in the same Patching Order dialog with ten rows and one selected row. This is a separately scoped follow-up; the completed numeric-dialog repair and its recorded acceptance remain intact.

## Progress


- [x] (2026-10-06) Read the project analysis, Jira, .NET best-practices, and commit-message skills, and the complete `.agents/PLANS.md`.
- [x] (2026-10-06) Read VIX-4007, including its stack trace, status, comments, links, and attachment metadata. There are no comments, linked issues, or attachments.
- [x] (2026-10-06) Confirmed a clean starting worktree and inspected HEAD history, OrderSetupHelper, NumberDialog, the Order project, shared-dialog callers, and the relevant VIX-4006 plan.
- [x] (2026-10-06) Established the source-level cause and prepared a caller-scoped repair with concrete validation and review boundaries.
- [x] (2026-10-06) Milestone 1: Updated VIX-4007 with concise user outcomes, scope, and acceptance criteria; preserved the original report and exception trace. Read the saved description back and confirmed status remains In Progress.
- [x] (2026-10-06) Milestone 2: Implemented the bounded dialog construction, built the Release x64 Order module, and completed Rider diagnostics. The user reports a full manual build, all 1045 unit tests passing, successful Zig Zag ordering for all six boundary counts (2, 10, 49, 50, 51, 100), correct defaults (selected count through 50, then 50), persistence after confirmation, and cancellation restoring the prior state.
- [x] (2026-10-06 19:07 UTC) Revised the active plan using the project revise-active-plan skill. Confirmed the Ctrl-only selection cause directly in the subscribed keyboard handler, identified the plain-A defect from the same condition, and prepared the focused repair and acceptance cases. The starting worktree is clean; milestones 1 and 2 are committed and their text is preserved.
- [x] (2026-10-06) Milestone 2A: Added adjacent keyboard-selection acceptance to VIX-4007, read the saved description back, and confirmed the original crash evidence and In Progress status remain. The implementation and runtime keyboard validation are still pending.
- [x] (2026-10-06) Milestone 2B: Implemented Ctrl+A-only Select All with protected-handler XML documentation; Release x64 Order module build, Rider diagnostics, and whitespace checks passed. The user reports Ctrl alone and plain A no longer select all, Ctrl+A does, and individual and Shift selection work as expected. The user's earlier six-boundary report confirms Zig Zag ordering, persistence, and Cancel behavior.
- [ ] Milestone 3: Record final numeric-dialog and keyboard-selection validation in this plan and VIX-4007 when Jira reporting is authorized.

## Surprises & Discoveries


Observation: the failure occurs before ShowDialog, so it is fully explained by the supplied constructor arguments. It does not require debugger-driven runtime investigation.

Evidence from `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs`, ZigZagItems_Click:

    new NumberDialog("ZigZag Length", "How many pixels to ZigZag?", 50, 2, elementList.SelectedIndices.Count)

Evidence from `src/Vixen.Common/Controls/NumberDialog.cs`, constructor:

    numericUpDownChooser.Minimum = minimum;
    numericUpDownChooser.Maximum = maximum;
    numericUpDownChooser.Value = value;

For selection counts 2 through 49, the value 50 is outside the configured bounds. The context menu normally offers Zig Zag only when SelectedItems.Count is greater than one. The click handler itself currently has no corresponding guard.

Observation: this is legacy WinForms UI despite Vixen also having a WPF application shell. It needs no Catel view-model, binding, asynchronous code, new interface, or lifecycle redesign.

Observation: the shared NumberDialog also serves controller output counts, filter-copy counts, universe counts, and a numeric-input service. Changing its constructor to silently clamp all values would change a shared API contract beyond this caller's defect. Keep those callers and the shared constructor unchanged.

Observation: PerformZigZag copies the selected row indexes but its swaps use start/end positions directly in elementList.Items, rather than selectedIndexes[start/end]. This suggests a separate defect for selections that do not start at row zero or contain gaps. This plan's alternating-order acceptance uses all rows, and its partial-selection acceptance checks opening/canceling only. Do not silently include an indexing repair in VIX-4007's dialog-crash change.

Observation: `docs/plans/display-setup/vix-4006-patched-output-range-order.md` explicitly records this crash as a separate issue discovered during its completed custom-order validation. Do not revisit its destination sorting or range-label design.

Observation: before milestone 1, Jira reported VIX-4007 as In Progress (the user noted the issue had been moved back from an incorrect status). The selected checkout still contains the crashing constructor call. Starting HEAD is 8605b014c, the VIX-4006 merge. Status is unchanged by the milestone 1 description update.

Observation: source searches in `src/Vixen.Tests/` found no NumberDialog or OrderSetupHelper tests. Gortex impact found zero test files for the handler, and the post-edit tests assessment reports 0/1 changed symbols covered. Rider findTests was invoked during planning but did not return before its orchestration call was canceled; do not interpret this as a successful no-tests response. No test runner was run for milestone 2.

Observation: the Release x64 Order module build succeeded and emitted four existing warnings from Vixen.Core (two CS8632 nullable-context warnings, one CS0618 obsolete API warning, and one CS0067 unused event warning). The user later reported a full manual build and all 1045 unit tests passing; the command transcript was not supplied or independently run by Codex. Rider file analysis returned no errors, but many WEAK WARNING suggestions across the legacy file. The changed NumberDialog construction is flagged only for object allocation, which is inherent to creating this dialog. Gortex has no C# LSP provider, and the post-edit contract assessment remains warn (risk score 100, blast size 6) after reviewing the six-symbol impact around the existing menu event path.

Observation: the available Rider `Vixen.Application` configuration is a .NET project configuration and does not confirm a Release/Output launch or an isolated copied profile. Runtime acceptance was not run by Codex to avoid launching the application with an unverified user profile or hardware-output configuration. The user subsequently reported successful dialog defaults and Zig Zag ordering for all six boundary counts in the acceptance table, plus persistence after confirmation and cancellation restoring the prior state.

Observation: the adjacent selection defect is caused by the existing OrderSetupHelper.OnKeyDown condition, not the numeric-dialog changes. The constructor subscribes this handler to the list's KeyDown event; the Designer declares the list as the existing DragDropListView control. The handler sets Selected on every ListViewItem and suppresses the key whenever either A is pressed or Control is held. On a Ctrl-only event the A comparison is false and Control is true, so the OR expression is true. Plain A also passes the condition. Ctrl-click then toggles already-selected rows, explaining the user's observed deselection. The source fully explains the report without needing runtime debugger state.

    src/Vixen.Modules/Property/Order/OrderSetupHelper.cs
    elementList.KeyDown += OnKeyDown;
    if (e.KeyCode == Keys.A | e.Control)
    item.Selected = true;
    e.SuppressKeyPress = true;

Observation: OnKeyDown is protected and currently has no XML documentation. Its behavior change requires a summary and parameter documentation under the repository API documentation rule. Keep its signature and event subscription intact. The Gortex impact assessment identifies three affected entries (the constructor and two parameters), no test files, and MEDIUM risk. Its inferred relationships are not evidence that this ListView uses MultiSelectTreeview; the Designer establishes the actual control type.

Observation: the revision starts at HEAD 3c3fed877 after the acceptance-results commit. Prior commits 6c3852848 and c32d188ae contain the caller fix and Jira acceptance update. The working tree is clean. New keyboard acceptance has not been run, and the previous 1045 passing tests predate this proposed change.

## Decision Log


Decision: compute the initial value in ZigZagItems_Click as Math.Min(50, selectedCount), with selectedCount read once and an early return for counts below 2.
Rationale: the existing maximum is the selection count and the intended lower bound is 2. For every supported selection this establishes 2 <= initialValue <= selectedCount, preserves the customary default of 50 for larger selections, and leaves existing user-entered divisibility validation intact. A guard also prevents construction with an impossible interval if selection changes before the handler runs.
Date/Author: 2026-10-06 / Codex.

Decision: use a using declaration for the dialog at the existing construction point.
Rationale: this is a disposable Windows form owned by this handler. Dispose it after its value has been consumed, whether the user confirms or cancels. The project .NET skill requires proper resource disposal. This is local to the changed statement.
Date/Author: 2026-10-06 / Codex.

Decision: use a focused module build, file diagnostics, and a manual dialog acceptance matrix without introducing a test seam or new automated tests for this small handler repair.
Rationale: the defect is a direct boundary violation in a modal UI path. A test of Math.Min alone would repeat the implementation, while automating the private modal handler would add machinery beyond the change. Manual checks exercise the actual constructor, displayed bounds, confirmation, cancellation, and existing error flow. Build success alone does not satisfy acceptance.
Date/Author: 2026-10-06 / Codex.

Decision: retain the current reorder algorithm, messages, property persistence, and shared-dialog contract.
Rationale: VIX-4007 requests that the numeric dialog opens safely. The separate selected-row indexing concern does not explain this exception and needs its own scope and evidence.
Date/Author: 2026-10-06 / Codex.

Decision: preserve user-reported validation separately from checks executed by Codex, and mark milestone 2 complete based on the user's report of the full acceptance matrix.
Rationale: the user reports all six boundary counts, successful ordering in each case, persistence after confirmation, cancellation restoring the previous state, a full manual build, and 1045 passing unit tests. Record provenance accurately; Codex independently ran the scoped Order module build and Rider diagnostics. Preserve the review boundary before milestone 3; AGENTS.md forbids commits unless explicitly requested.
Date/Author: 2026-10-06 / Codex.

Decision: append milestones 2A and 2B before the uncompleted final reporting milestone, preserving completed milestones 1 and 2 verbatim.
Rationale: the user explicitly requested a revise-active-plan investigation for an adjacent Order dialog bug. Record its source cause and concrete fix separately from the accepted numeric-dialog work. Milestone 2A aligns issue acceptance before new implementation; milestone 2B owns the keyboard repair. This revision writes only the plan and does not perform Jira writes or application edits. Each future milestone retains the skill's explicit review stop.
Date/Author: 2026-10-06 19:07 UTC / Codex.

Decision: replace the Boolean OR with conditional AND in OrderSetupHelper.OnKeyDown, retaining the existing select-all loop and SuppressKeyPress only inside the Ctrl+A branch.
Rationale: requiring both A and Control eliminates Ctrl-only, plain-A, and unrelated Ctrl-key selection while preserving the intended shortcut and ordinary control input. Merely replacing the single OR with a double OR would retain the bug. Keep the existing treatment of extra modifiers when Control and A are both present; this repair does not introduce a new exact-modifier shortcut policy. No shared control, Designer, persistence, or reorder algorithm change is needed. Add XML documentation to this protected handler in the same edit.
Date/Author: 2026-10-06 19:07 UTC / Codex.

Decision: verify the actual selection behavior manually after a focused module build and Rider diagnostics; do not add a test seam or a test that repeats the Boolean expression.
Rationale: the source establishes the cause, while the actual dialog confirms Ctrl-only, Ctrl-click, Ctrl+A, plain A, and ordinary keyboard selection. The change is local and reversible; the existing full test result remains historical evidence until the new code is validated. Broader validation is warranted only if new findings expand scope or reveal failures.
Date/Author: 2026-10-06 19:07 UTC / Codex.

## Outcomes & Retrospective


Milestone 1 is complete. VIX-4007 now records the user-facing summary, scope, acceptance criteria, ten-element scenario, and 49/50/51 boundary check. The original reproduction and exception evidence remain in the description, and the issue remains In Progress. Milestone 2 changed only ZigZagItems_Click: it returns for fewer than two selected rows, starts at Math.Min(50, selectedCount), keeps the 2..selectedCount range, and disposes the dialog after its value is consumed. The Release x64 Order module build and Rider file diagnostics completed. The user reports a full manual build and all 1045 unit tests passed. The user manually tested all six boundary counts in the matrix (2, 10, 49, 50, 51, 100): each dialog opened with the expected default and each Zig Zag operation produced the expected ordering. The user also reports that Patching Order persists after confirmation and Cancel restores the prior state. These manual results were reported by the user, not independently run by Codex. The design avoids changing a shared dialog API for one invalid caller and records the separate indexing concern so runtime acceptance does not accidentally claim coverage of that defect.

The adjacent Ctrl-selection repair is implemented: OnKeyDown now requires both A and Control before running the existing Select All loop. Its protected handler has XML documentation for the summary, parameters, and shortcut behavior. The Release x64 Order module build passed; Rider reported no errors. The user manually reports that Ctrl alone and plain A no longer select all, Ctrl+A selects all, and individual and Shift selection work as expected. The user's earlier report confirms successful Zig Zag ordering for all six boundary counts, persistence after confirmation, and Cancel restoring the prior state. These UI results are user-reported; Codex did not run a separate UI session or automated tests for the handler. Gortex reports no tests covering the handler, no configured guard rules, and a contract warning with a reviewed blast size of three. Milestone 2A added the separate acceptance subsection to VIX-4007 and confirmed the original exception evidence remains and status is In Progress.

## Architecture Design: VIX-4007 — ZigZag setup of Patching Order property crashes Vixen


Detected IDE Environment: Rider with automation hooks. During implementation, navigate to the handler in the named file and use a small Apply snippet from chat replacement for the construction block, or the permitted repository editing tool. Rider get_file_problems is available for scoped diagnostics. The attempted Rider test lookup was inconclusive, as recorded above.

Core Strategy: validate the caller's selection count and bound its initial input before constructing the existing dialog. Apply `.agents/skills/dotnet-best-practices/SKILL.md` for input boundaries and disposal. Apply `.agents/skills/jira/SKILL.md` for future issue descriptions and validation reporting. This is maintenance of existing WinForms UI; no architectural pattern or WPF redesign is needed.

Data Model & Property Contracts: no new fields, settings, serialized data, interfaces, or public/protected members. Keep OrderSetupHelper's public Perform contract and the NumberDialog constructor/Value contract unchanged. All proposed locals belong to the private ZigZagItems_Click method. If execution expands to a public/protected API, read the project csharp-docs skill and update its XML documentation in the same change; this plan does not require such an expansion.

Mathematical / Boundary Logic: let n be elementList.SelectedIndices.Count. If n < 2, return without constructing a dialog or changing order. Otherwise, minimum = 2, maximum = n, and initialValue = Math.Min(50, n). For 2 <= n < 50, initialValue = n. For n >= 50, initialValue = 50. The selected length every remains subject to the existing n % every == 0 check after confirmation. For example, n = 51 opens safely at 50; confirming 50 displays the existing divisibility warning without reordering. Do not choose an automatic divisor or broaden the input range.

Subsystem Component Matrix:

| Component | Role in this design |
| --- | --- |
| src/Vixen.Modules/Property/Order/OrderSetupHelper.cs | Only production edit: guard selection count, bound the initial value, and dispose the NumberDialog in ZigZagItems_Click. |
| src/Vixen.Common/Controls/NumberDialog.cs | Read/validation reference for bounds, returned Value, Enter, and Escape; no edit. |
| src/Vixen.Modules/Property/Order/OrderSetupHelper.Designer.cs | Existing list and OK/Cancel controls; no edit. |
| src/Vixen.Modules/Property/Order/Order.csproj | Narrow module build entry point; no edit. |
| docs/plans/display-setup/vix-4007-zigzag-dialog-range.md | Living design and execution evidence. |
| VIX-4007 | Initial acceptance clarification and final user-facing validation report during authorized execution. |

Adjacent keyboard repair scope: the only production file remains OrderSetupHelper, but the new change belongs to its protected OnKeyDown event handler. Require both the A key and Control modifier, add accurate XML documentation to that handler, and leave its signature, event hookup, select-all loop, update batching, and suppression behavior intact. No Catel, shared-control, new WinForms UI, asynchronous, serialization, or public API redesign is required. Consult the project .NET and C# documentation skills before implementation.

    src/Vixen.Modules/Property/Order/OrderSetupHelper.cs
    .agents/skills/dotnet-best-practices/SKILL.md
    .agents/skills/csharp-docs/SKILL.md

## Context and Orientation


Patching Order is an element property that controls the order of element sources during patching. OrderSetupHelper is the existing setup form for this property. Its Perform method populates a list of distinct leaf elements, shows the form, and writes the displayed order back to element properties only when the outer form returns OK. Its context menu offers Reverse and Zig Zag when more than one row is selected.

ZigZagItems_Click creates a NumberDialog, shows it, and calls PerformZigZag with the chosen integer only when the numeric dialog returns OK. PerformZigZag requires the selected count to be evenly divisible by that integer, then reverses every second group and reindexes displayed rows. ReIndexElementNodes updates the working lookup; the outer Perform confirmation is the existing persistence boundary. A leaf element is an individual element without child elements. A group is a parent containing those elements.

NumberDialog is a shared WinForms form backed by NumericUpDown, a control that enforces a minimum and maximum when its Value is assigned. Its constructor sets those bounds before assigning the initial value. The caller must supply a value within that interval. Enter returns OK and Escape returns Cancel. Existing callers and bounds remain unchanged.

Primary repository documentation for this area is the completed VIX-4006 plan at `docs/plans/display-setup/vix-4006-patched-output-range-order.md`. It records why custom element order must remain independent of destination order and explicitly excludes this crash. Documentation and current source agree that this is a separate Patching Order setup concern.

## Plan of Work


Milestones 1 and 2 are complete and committed. Execute the newly added milestone 2A, then 2B, then the revised milestone 3. This revision records the adjacent bug's cause and fix for review under the explicitly invoked revise-active-plan skill. Do not perform Jira writes while revising the plan. Every new or modified milestone ends at an explicit human-review boundary. Generate commit messages for repository changes with the project commit-msg skill, but create commits only when explicitly requested.

The completed numeric-dialog implementation below is historical context, not an instruction to reapply it. The new production edit is the OnKeyDown condition plus its required protected-method XML documentation. Preserve the previously accepted ZigZagItems_Click block, selection/reorder algorithms, event subscription, and shared controls. After implementation, run the scoped module build and file diagnostics and exercise the keyboard-selection cases described in milestone 2B.

The completed milestone 2 source edit replaced NumberDialog construction with a guarded local block. Its ShowDialog, OK check, button disabling/enabling, PerformZigZag, and persistence remain unchanged. The new milestone 2B changes only shortcut recognition and the protected handler's documentation. Keep tabs and LF line endings; do not reformat untouched code or clean up existing warnings.

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Milestone 1: Record the dialog behavior and acceptance criteria in Jira


Context: VIX-4007 describes a crash selecting Zig Zag and expects a numeric dialog. Preserve its original reproduction and exception evidence. At execution start the issue was In Progress; retain its status and unrelated fields. It has no attachments, comments, or linked issues.

Plan of Work: when execution includes Jira editing, read the project Jira skill and current issue again, then add concise Summary, Scope, and Acceptance Criteria covering safe opening with 2–49 selected elements, the bounded initial value, the selectable range, cancellation, and existing divisibility feedback. Include the ten-element reproduction and a short 49/50/51 boundary check as the user-facing test scenario. Keep file names, formulas, internal types, and detailed test instructions in this local plan. Do not replace the original stack trace or claim validation has passed. Use a structured update for the description and read the result back.

Concrete Steps: working directory C:\Dev\Vixen. Run the commands below, inspect current VIX-4007 with the Jira connector, and make only the agreed description update. Update this plan's living sections with the result.

    git status --short
    git diff -- docs/plans/display-setup/vix-4007-zigzag-dialog-range.md

Validation and Acceptance: the issue now states that the numeric dialog opens for any selection of at least two elements, starts at the smaller of 50 and the selection count, allows 2 through the count, and retains cancellation/divisibility behavior. Verify preserved exception evidence and unchanged status. No application code changes in this milestone.

STOP HERE for manual review and commit execution before proceeding.

1. Halt execution; do not start the next milestone.
2. Run git status --short and git diff for the files modified in this milestone.
3. If repository files changed, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4007 as the subject prefix.
4. Output its complete paste-ready Commit message block; do not create a commit without an explicit request.
5. Pause and wait for explicit user confirmation before advancing.

### Milestone 2: Repair the caller and verify the actual numeric dialog


Context: the production edit belongs only in C:\Dev\Vixen\src\Vixen.Modules\Property\Order\OrderSetupHelper.cs, inside the private ZigZagItems_Click method. Re-read current source and git status, and apply the project .NET skill before editing. If another contributor has already repaired this call, compare its behavior with the contract and validate rather than overwriting it.

Plan of Work: use Gortex impact for ZigZagItems_Click before editing. Replace its first NumberDialog statement with the following block, keeping the existing remainder of the handler. In Rider, apply this as one block replacement at the existing construction point. Match the file's tab indentation; the four-space indentation below is Markdown formatting, not repository C# indentation.

    var selectedCount = elementList.SelectedIndices.Count;
    if (selectedCount < 2)
    {
        return;
    }

    using var numberDialog = new NumberDialog("ZigZag Length", "How many pixels to ZigZag?",
        Math.Min(50, selectedCount), 2, selectedCount);

The using declaration's scope must include ShowDialog and the existing numberDialog.Value read, so disposal occurs after the chosen value is consumed. Do not modify NumberDialog, PerformZigZag, menu creation, designer files, captions, or public signatures. Do not add an automatic divisor search, new test API, service, dependency, or test that merely asserts Math.Min.

Concrete Steps: working directory C:\Dev\Vixen. Use full MSBuild, available in the configured developer environment, for the affected project and its dependencies. SolutionDir is required because repository build output is centralized relative to the solution.

    msbuild src/Vixen.Modules/Property/Order/Order.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -p:SolutionDir=C:/Dev/Vixen/ -v:m
    git diff --check
    git diff -- src/Vixen.Modules/Property/Order/OrderSetupHelper.cs

Expected results: the project build exits successfully with zero errors, diff --check emits no whitespace errors, and the source diff contains only the count guard and changed dialog construction. Record actual warnings and results. Start long-running commands with native exec_command and yield_time_ms=10000; while only waiting, use write_stdin with yield_time_ms at least 30000.

Run Gortex detect plus applicable tests/guards/contract assessments after edits. No signature changes are planned; if one becomes necessary, verify it before proceeding. Run Rider get_file_problems on src/Vixen.Modules/Property/Order/OrderSetupHelper.cs with rootFolder C:/Dev/Vixen and errorsOnly false. Address only diagnostics triggered on changed/added lines. Record unavailable or timed-out tools rather than claiming success.

Validation and Acceptance: launch the newly built Release application in Rider using its existing application run configuration, or start the existing executable in Release/Output. Confirm the run configuration points to Release/Output and that Module.Property.Order uses the just-built assembly, rather than validating an older Debug output. Use a copied test profile. Open Display Setup, add/configure Patching Order for a numbered group, select rows inside its helper list, and right-click a selected row to choose Zig Zag. Execute the matrix below without hardware output.

| Selected count | Expected initial value | Expected allowed range |
| --- | --- | --- |
| 2 | 2 | 2–2 |
| 10 | 10 | 2–10 |
| 49 | 49 | 2–49 |
| 50 | 50 | 2–50 |
| 51 | 50 | 2–51 |
| 100 | 50 | 2–100 |

Each numeric dialog must open without the reported exception. Check lower/upper bounds with the control's arrows and by committing typed values. With zero or one selected row, the normal context-menu route must not offer Zig Zag. If a stale menu invokes the handler with fewer than two selected rows, the early return must create no dialog and change no order; source inspection establishes this guard without adding a production seam.

Select all eight rows initially ordered 1,2,3,4,5,6,7,8. Choose length 2 and confirm: element identities must appear as 1,2,4,3,5,6,8,7, with displayed order numbers reindexed consecutively. On all 100 rows, confirm default length 50 and verify that rows 1–50 keep their identities and rows 51–100 reverse. On all 51 rows, confirm default 50: the existing divisibility warning appears, order remains unchanged, and the outer buttons remain usable. Dismiss that warning, reopen, choose 3, and confirm successful alternating groups. With all ten rows, confirming default 10 is valid and leaves the only group unchanged.

Cancel the numeric dialog with its Cancel button and with Escape after changing its value; element identities/order must not change. Confirm with Enter in a valid all-rows case. Open and cancel with a smaller selection starting after the first row or containing gaps to prove dialog construction is safe there, but do not use that selection to claim the separate reorder-indexing concern is repaired. Finally, cancel the outer Patching Order helper to verify no property changes are committed. Repeat a valid reorder, confirm the outer helper, then reopen to verify the accepted property order persists.

STOP HERE for manual review and commit execution before proceeding.

1. Halt execution; do not start the next milestone.
2. Run git status --short and scoped git diff for the handler and this plan.
3. Invoke `.agents/skills/commit-msg/SKILL.md` using VIX-4007 as the subject prefix.
4. Output its complete paste-ready Commit message block; do not create a commit without an explicit request.
5. Pause and wait for explicit user confirmation before advancing.

### Milestone 2A: Record adjacent keyboard-selection acceptance in Jira


Context: the user observed Ctrl alone selecting all rows while testing the accepted numeric-dialog repair. This is a distinct pre-existing defect in the same Patching Order helper. The local plan continues to use VIX-4007 as its active issue prefix; no new issue or transition is implied by this revision. Preserve the original crash report, trace, numeric-dialog acceptance, and current issue status.

Plan of Work: during authorized execution, read the project Jira skill and current VIX-4007. Append a clearly labeled adjacent keyboard-selection scope and acceptance subsection. State that Ctrl alone preserves the existing selection, Ctrl-click toggles only the clicked row, Ctrl+A selects all rows, and plain A does not invoke Select All. Include the ten-row reproduction and expected behavior. Keep internal code details in this plan. Read the saved issue back, and record the result in the living sections. Do not claim the keyboard fix has been implemented or validated at this stage.

    .agents/skills/jira/SKILL.md

Concrete Steps: use the repository root as the working directory. Inspect status and the scoped plan diff before editing. Use structured Jira update arguments, preserving other fields and the existing numeric-dialog content.

    C:\Dev\Vixen
    git status --short
    git diff -- docs/plans/display-setup/vix-4007-zigzag-dialog-range.md

Validation and Acceptance: the saved issue clearly distinguishes the already accepted numeric-dialog behavior from the adjacent selection requirement; original evidence and status remain present. This milestone changes no application code.

STOP BOUNDARY: Stop execution, run git status and git diff on the modified files, invoke the project commit-msg skill with VIX-4007 as the subject prefix for repository changes, output its complete paste-ready Commit message block, and wait for explicit human review before advancing. Do not create a commit unless explicitly requested.

    .agents/skills/commit-msg/SKILL.md

### Milestone 2B: Repair Ctrl+A recognition and verify selection behavior


Context: OrderSetupHelper.OnKeyDown is subscribed directly to the element list's KeyDown event. Its current Boolean OR selects every row for either A or any key event with Control held. Replace that condition with AND so both inputs are required. This is separate from the completed Zig Zag dialog repair and the still-excluded partial-selection Zig Zag indexing concern.

    src/Vixen.Modules/Property/Order/OrderSetupHelper.cs
    src/Vixen.Modules/Property/Order/OrderSetupHelper.Designer.cs

Plan of Work: inspect current git status, read the current handler and its constructor subscription, and read the project .NET and C# documentation skills. Run Gortex impact on the handler before editing. In the existing method, replace the condition below; change no other executable statements. Add a summary, both parameter descriptions, and remarks that only Ctrl+A invokes the custom Select All operation and suppresses the key. This method is a protected event handler with two parameters, not the one-parameter WinForms override. Preserve its exact signature and do not rename it, change access, add an overload, or modify Designer wiring.

    .agents/skills/dotnet-best-practices/SKILL.md
    .agents/skills/csharp-docs/SKILL.md
    Before: if (e.KeyCode == Keys.A | e.Control)
    After:  if (e.KeyCode == Keys.A && e.Control)
    protected void OnKeyDown(object sender, KeyEventArgs e)

Use a summary such as Handles the element list keyboard shortcut for selecting all rows. Document sender as The control that raised the keyboard event, and e as The keyboard event data. Remarks should state that A with Control selects all rows and suppresses the key press, while other input continues through the control's normal handling. No returns documentation is needed for this void method. Keep tabs and LF in the source; the four-space indentation in this plan is Markdown formatting. Leave BeginUpdate, EndUpdate, the row-selection loop, and SuppressKeyPress in the recognized-shortcut branch. Keep the bounded NumberDialog construction from milestone 2 untouched. Do not modify shared controls, introduce new UI or test infrastructure, or repair the separate Zig Zag indexing concern.

Concrete Steps: use full MSBuild from the repository root to rebuild the Release x64 Order module and dependencies. Run Gortex detect and applicable tests, guards, and contract checks afterward. Run Rider get_file_problems for the changed source with rootFolder C:/Dev/Vixen and errorsOnly false; fix only diagnostics on changed or added lines. No signature changes are planned; if the scope requires any, use Gortex verify first and revise this plan before expanding it.

    C:\Dev\Vixen
    msbuild src/Vixen.Modules/Property/Order/Order.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -p:SolutionDir=C:/Dev/Vixen/ -v:m
    git diff --check
    git diff -- src/Vixen.Modules/Property/Order/OrderSetupHelper.cs docs/plans/display-setup/vix-4007-zigzag-dialog-range.md

Start the build with native exec_command and yield_time_ms=10000; while only waiting, use write_stdin with yield_time_ms at least 30000. Expect exit 0 and zero compiler errors; record actual warnings without unrelated cleanup. The scoped source diff must contain only the condition replacement and handler documentation. No new automated test is required for this condition-only repair; do not turn a truth-table assertion into a production test seam. If validation is broadened for a concrete failure or shared-behavior change, use the full-MSBuild test workflow already specified in this plan and report actual results. Earlier user-reported test passes do not validate this new edit.

Validation and Acceptance: launch the rebuilt Release application with the just-built Order module, using a copied test profile and no hardware output as in milestone 2. Open Patching Order with ten identifiable rows. Click row 3 so only row 3 is selected. Press and release left Ctrl alone, then right Ctrl alone, and hold each long enough for repeated input: exactly row 3 must remain selected, with no global highlight change or row reorder. Repeat from a noncontiguous selection and confirm its membership stays unchanged.

While holding Ctrl with only row 3 selected, click row 7: rows 3 and 7 must be selected. Click row 3 while still holding Ctrl: only row 7 must remain selected. Release Ctrl: row 7 must remain selected. Verify Ctrl+A selects all ten rows, including when started with no selected rows or a noncontiguous selection, and repeating Ctrl+A leaves all rows selected. Reset to one selected row, press A alone, and confirm it does not select all; allow the ListView's normal name-search behavior, so the exact single selected row may change. With one selected row, press Ctrl with another letter such as B and confirm no Select All. A held with Shift but without Control must not invoke Select All either.

Check ordinary click, Shift-click, and Shift+arrow range selection still work. Selection gestures alone must never reorder row identities or displayed order numbers; canceling the outer helper must preserve prior persisted order. Perform a focused Zig Zag regression using all ten rows (Ctrl+A, dialog default 10, cancel unchanged), then all eight rows with length 2 and expected identity order 1,2,4,3,5,6,8,7. Confirm and reopen to check persistence, then repeat and cancel to check rollback. Do not run Zig Zag on the Ctrl-click subset to claim the separate indexing issue is fixed. Repeating every prior numeric boundary or all 1045 tests is unnecessary unless a new finding warrants it.

Record manual observations with provenance. If Codex cannot launch the copied profile or automate real key input, leave the keyboard matrix pending until the user reports it; a successful build and expression inspection are not proof of UI acceptance. Mark implementation and runtime acceptance separately in Progress as needed.

STOP BOUNDARY: Stop execution, run git status and git diff on the modified source and plan, invoke the project commit-msg skill with VIX-4007 as the subject prefix, output its complete paste-ready Commit message block, and wait for explicit human review before advancing. Do not create a commit unless explicitly requested.

    .agents/skills/commit-msg/SKILL.md

### Milestone 3: Record the evidence and align the issue with the delivered fix


Context: no further source changes are planned after milestone 2B. Review the accepted numeric-dialog results from milestone 2 and the new module build, diagnostics, keyboard-selection cases, and focused Zig Zag regression from milestone 2B. The related VIX-4006 fix must remain intact. Complete any deferred keyboard acceptance before describing it as verified. Preserve the distinction between the previously reported full build and 1045 passing tests and checks run after the adjacent repair.

Plan of Work: update this plan's living sections with exact commands, exit results, manual outcomes, and any unavailable checks. If manual application access was unavailable, clearly retain that acceptance as pending and state precisely what build/source checks established. Do not mark implementation fully accepted from compilation alone. When Jira reporting is authorized, re-read VIX-4007, reconcile its user-facing acceptance criteria with the final delivered behavior, and add a concise validation comment. Preserve the original report/stack trace and status. Use the project Jira skill and read the result back.

Concrete Steps: working directory C:\Dev\Vixen. Inspect final scope and whitespace. If no source changed after Milestone 2 validation, do not repeat the build merely for another transcript.

    git status --short
    git diff --check
    git diff -- src/Vixen.Modules/Property/Order/OrderSetupHelper.cs docs/plans/display-setup/vix-4007-zigzag-dialog-range.md

Validation and Acceptance: the final handoff states the bounded default, minimum/maximum, cancellation behavior, alternating reversal and persistence evidence, and the additional Ctrl-only, Ctrl-click, Ctrl+A, and plain-A results. Give the module build and Rider diagnostics for the adjacent edit and any pending manual cases. VIX-4007's final comment and any necessary description adjustments reflect both scoped repairs and report only validation actually performed, identifying user-reported results. No issue transition or automatic commit is part of this plan.

STOP BOUNDARY: Stop execution, run git diff on all files modified in this milestone, invoke the project commit-msg skill with VIX-4007 as the subject prefix for repository changes, and wait for explicit human review. The existing detailed review instructions below continue to apply.

    .agents/skills/commit-msg/SKILL.md

STOP HERE for manual review and commit execution before proceeding.

1. Halt execution; all authorized implementation and reporting should now be complete.
2. Run git status --short and scoped git diff for this milestone's repository changes.
3. If repository files changed, invoke `.agents/skills/commit-msg/SKILL.md` using VIX-4007 as the subject prefix.
4. Output its complete paste-ready Commit message block; do not create a commit without an explicit request.
5. Pause and wait for explicit user review before additional work.

## Concrete Steps


The executable module-build and review commands are given in each milestone with C:\Dev\Vixen as their working directory. During planning, git status --short reported a clean worktree; git log -5 --oneline showed HEAD 8605b014c; source searches located the invalid call and related documentation. Jira was read, its description was updated and read back, and status was verified as In Progress. The original stack trace remains present in the description. The module build and runtime scenarios above are future work.

If later changes actually affect shared behavior or multiple projects, broaden validation deliberately and document why. For a full Release x64 solution build use:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64

If meaningful automated tests are added during an explicitly revised scope, read the relevant test code and project skills first. Build tests with full MSBuild because their QMLibrary and LiquidFunWrapper dependencies are C++/CLI projects; dotnet test alone cannot build them. Then run the already-built tests:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/

These broader commands are conditional, not required repetitions for the planned single-handler repair. Report actual counts; no test pass count is predicted by this plan.

## Validation and Acceptance


The primary proof is the actual numeric dialog opening for 2, 10, 49, 50, 51, and 100 selected elements with the stated defaults and bounds. Confirmed valid lengths retain existing alternating-group order, invalid divisibility gives the existing warning, numeric cancellation changes nothing, and outer confirmation retains the accepted property order. The guard, bounded value, and scoped disposal must appear in the actual handler. Module compilation, Rider diagnostics, and whitespace checks complement these observable behaviors.

During planning, no build, automated tests, or runtime acceptance was run. Milestone 2 validation: `msbuild src/Vixen.Modules/Property/Order/Order.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -p:SolutionDir=C:/Dev/Vixen/ -v:m` exited 0 and produced `Release/Output/Module.Property.Order.dll`; four Vixen.Core compiler warnings were reported as detailed above. Rider `get_file_problems` for this file (`rootFolder=C:/Dev/Vixen`, `errorsOnly=false`) returned no errors and legacy weak warnings; the warning on the changed dialog allocation is inherent to construction. `git diff --check` exited 0. Gortex tests found no test files; guards found no rules; contract stayed at warn after impact review (6 affected symbols); C# diagnostics had no LSP provider. Codex did not run the manual UI matrix because the available app run configuration does not confirm Release/Output or an isolated test profile. The user reports testing every listed boundary count (2, 10, 49, 50, 51, 100), observing the expected initial values and proper Zig Zag ordering in all six cases, confirming persistence, and confirming Cancel restores the prior state. The user also reports a full manual build and 1045 unit tests passing. These manual and test results are user-reported; the scoped module build, Rider diagnostics, and diff check were run by Codex. Existing unrelated diagnostics are not authorization for cleanup.

Milestone 2B validation: the scoped Release x64 Order module rebuild exited 0 and produced `Release/Output/Module.Property.Order.dll`; it reported four Vixen.Core warnings (two CS8632, one CS0618, and one CS0067). Rider file diagnostics reported no errors and legacy weak warnings, including an “Invert if” suggestion on the changed condition line, which remains unchanged to preserve the planned branch structure. `git diff --check` exited 0. Gortex impact reports three affected entries, MEDIUM risk, and zero test files; post-edit tests report 0/1 coverage, guards have no configured rules, and contract remains `warn` (risk score 100, blast size 3) after impact review. Gortex `detect` rejected the symbol selector and C# diagnostics have no LSP provider. The user manually reports Ctrl-alone and plain-A preservation, Ctrl+A selecting all, and expected individual and Shift selection. The user's earlier report confirms ordering for all six Zig Zag boundary counts, persistence after confirmation, and Cancel restoring the prior state. UI results are user-reported; no automated tests or separate UI session were run by Codex.

## Idempotence and Recovery


Re-read status/source before every edit and preserve unrelated user changes. The replacement is applied only once at the identified construction point. Repeated dialog cancellation must not accumulate changes. Repeated intentional confirmation may alternate the affected groups again; that is the existing behavior, not an idempotent command.

Keep experiments confined to a copied profile and avoid hardware output. Cancel the outer helper when discarding a trial; save only the copy when deliberately checking persistence. Retry builds after resolving actual environmental errors without substituting dotnet's MSBuild for native dependency builds. If rollback is needed, remove only this issue's reviewed edits; do not reset the workspace, delete profiles, or undo VIX-4006.

For the adjacent fix, repeated Ctrl-only presses and releases must preserve selection; repeated Ctrl+A must leave all rows selected. Ctrl-click intentionally toggles the clicked row. Preserve the completed caller fix if rolling back the adjacent condition change: remove only that change and its documentation, retaining unrelated user work and previously committed numeric-dialog behavior.

## Artifacts and Notes


Planning evidence: VIX-4007 reports ArgumentOutOfRangeException with Actual value 50 at NumberDialog.cs line 14, called by OrderSetupHelper.cs line 82. Both files match that trace. Eight textual NumberDialog construction matches were found, including one commented-out call; this establishes shared usage, not an exhaustive audit of every caller's correctness. Gortex impact identified ElementListOnMouseClick as the direct event-registration path and zero test files. An impact query for the new plan returned file_not_indexed because it did not yet exist in the graph.

No external resources or attachments are required to follow this plan. Its source and mathematical evidence are sufficient for the reported crash. Rider findTests was attempted but canceled without a result. This limitation does not change the direct source diagnosis.

Planning validation passed a PowerShell structural check for all required sections, three milestones, three explicit review stops, no triple-backtick fences, and the required concluding statement. Final git status --short showed only this untracked plan; git diff --check returned no errors. Because tracked diff checks omit new files, git diff --no-index --check -- NUL docs/plans/display-setup/vix-4007-zigzag-dialog-range.md was also run: it emitted no whitespace diagnostics and returned 1 for a differing file. Gortex detect likewise reported no tracked changes and explicitly excludes untracked files. Symbol-level tests/guards/contract checks do not apply to this Markdown-only addition. Rider's post-edit hook excluded the Markdown file because it is not part of a solution project; no C# lint or build result is claimed.

Revision evidence: the constructor subscribes OnKeyDown to the element list; the handler currently tests A OR Control and explicitly selects every ListViewItem. The Boolean outcomes are Ctrl only: false OR true; plain A: true OR false; Ctrl+A: true OR true. All currently pass. The proposed AND accepts only the last case. Designer confirms the list is the legacy DragDropListView; no change to that control is proposed. Gortex impact reports three affected entries, zero test files, and MEDIUM risk. There is no runtime-selection transcript for the new repair at planning time.

Milestone 2A evidence: VIX-4007 was In Progress before editing and after read-back. Its description now has an “Adjacent keyboard-selection acceptance” subsection covering Ctrl-alone selection preservation, Ctrl-click toggling only the clicked row, Ctrl+A selecting all, and plain A not invoking the custom Select All behavior. The saved description still contains the original Zig Zag summary, acceptance criteria, and exception evidence. No implementation or keyboard validation was claimed for that milestone.

Milestone 2B evidence: the source now requires both Ctrl and A before selecting all rows. This protects Ctrl-alone, plain A, and other Ctrl-key input from the custom Select All branch while preserving Ctrl+A. The handler's signature and existing select-all behavior remain unchanged. Build and Rider checks passed as detailed above. The user reports that Ctrl alone and plain A no longer select all, Ctrl+A does select all, and individual and Shift selection work as expected. The user's earlier six-boundary report confirms Zig Zag ordering, persistence, and Cancel behavior. These manual observations are user-reported; Codex did not run separate UI or automated tests for this edit.

Revision validation: git status shows only the plan modified; git diff --check exits 0 with no whitespace diagnostics. A PowerShell structural comparison against HEAD verifies that completed milestone 1 and 2 bodies are unchanged, new milestones 2A and 2B and modified milestone 3 contain explicit review boundaries, no nested code fences exist, and the diff contains only this plan. The scoped git diff was reviewed for the commit-msg skill. Gortex detect reports only the plan changed with LOW risk and no affected dependents. Symbol-scoped plan assessments find no tests or guard rules and a contract verdict of allow with risk score 0. Build, unit tests, runtime acceptance, and C# file lint are not run for this Markdown-only revision.

## Interfaces and Dependencies


Keep these signatures and existing dependencies unchanged:

    private void OrderSetupHelper.ZigZagItems_Click(object sender, EventArgs e)
    private void OrderSetupHelper.PerformZigZag(int every)
    public bool OrderSetupHelper.Perform(IEnumerable<IElementNode> selectedNodes)
    public NumberDialog(string title, string prompt, int value, int minimum = 0, int maximum = int.MaxValue)
    public int NumberDialog.Value { get; }

Use the existing System.Math.Min method, ListView selection collection, NumberDialog, WinForms DialogResult, and disposal support. No new package, project, configuration, data migration, interface, production helper, or public API is required.

The adjacent milestone also preserves the protected keyboard-handler signature and uses existing WinForms key event data. Add its required XML documentation without changing parameters or access. No new dependency or shared-control API is required.

    protected void OrderSetupHelper.OnKeyDown(object sender, KeyEventArgs e)
    System.Windows.Forms.KeyEventArgs.KeyCode
    System.Windows.Forms.KeyEventArgs.Control
    System.Windows.Forms.KeyEventArgs.SuppressKeyPress
    System.Windows.Forms.Keys.A

## Revision Notes


2026-10-06 / Codex: Created the VIX-4007 design from the issue's exception evidence and the current caller/constructor source. Chose a bounded caller default with a small selection guard and local dialog disposal. Recorded the separate selected-row indexing concern, inconclusive Rider test lookup, existing Jira status, and future Jira/review boundaries. Application implementation and validation remain pending.

2026-10-06 / Codex: Recorded planning structure, whitespace, and worktree validation. Clarified that partial-selection dialog acceptance begins after the first row, and recorded the distinction between untracked-plan checks and source diagnostics.

2026-10-06 / Codex: Completed milestone 1 by updating VIX-4007 with the planned user-facing summary, scope, acceptance criteria, and 49/50/51 scenario. Read the saved description back and verified the original exception evidence remains and status is In Progress. Paused for milestone review as required by the plan.

2026-10-06 / Codex: Implemented milestone 2's guarded, bounded, disposable NumberDialog construction in ZigZagItems_Click. The Order module Release x64 build and whitespace check passed; Rider diagnostics had no errors, with the changed dialog allocation's expected weak warning and unrelated legacy warnings. Gortex found no test/guard rules and retained a contract risk warning after impact review. The user reports a full manual build, all 1045 unit tests passing, expected dialog defaults and Zig Zag ordering for all six boundary counts, persistence after confirmation, and Cancel restoring the prior state. These results are recorded as user-reported. No Jira update or commit was made. Milestone 2 is complete; paused at its review boundary before milestone 3.

2026-10-06 19:07 UTC / Codex: Used the project revise-active-plan skill for the user's adjacent Ctrl-only selection report. Read the active plan, the plan rules, the event handler, and Designer declaration. Found Boolean OR in Select All detection; this also explains plain A selecting all. Added milestones 2A and 2B for issue acceptance and the focused AND-condition repair with protected-handler XML docs, manual keyboard/mouse cases, and focused Zig Zag regression. Expanded only the uncompleted final reporting milestone. Preserved the completed milestone bodies and prior acceptance; no production code, Jira fields, or commits changed during this revision. Whitespace and structural checks passed, the scoped diff was reviewed, and Gortex reports a plan-only change with LOW risk and contract allow. Paused for plan review under the revise-active-plan skill; milestone 2A is next.

2026-10-06 / Codex: Completed milestone 2A by appending a clearly labeled adjacent keyboard-selection acceptance subsection to VIX-4007. The saved issue was read back; its original Zig Zag criteria and exception evidence remain, and status remains In Progress. The new acceptance covers Ctrl-alone preservation, Ctrl-click row toggling, Ctrl+A Select All, and plain A not invoking Select All, with implementation and validation explicitly pending. No application code, test, build, or manual keyboard validation was changed or claimed. Milestone 2A is complete; paused at its review boundary before milestone 2B.

2026-10-06 / Codex: Implemented the Milestone 2B source change in `OrderSetupHelper.OnKeyDown`: Ctrl+A is now the only shortcut that runs the existing select-all loop and suppresses the key. Added protected-method XML documentation without changing its signature or any other executable statement. The Release x64 Order module rebuild and `git diff --check` passed; Rider diagnostics found no errors and legacy warnings, including an “Invert if” suggestion on the changed line, were left untouched to preserve the planned branch structure. Gortex reports 0/1 test coverage, no configured guards, and contract warn after reviewing MEDIUM impact (three affected entries). The keyboard/manual behavior was previously pending.

2026-10-06 / Codex: Completed Milestone 2B after the user reported manual confirmation that Ctrl alone and plain A no longer select all, Ctrl+A selects all, and individual and Shift selection work as expected. The earlier user report also confirmed correct ordering for all six Zig Zag boundaries, persistence after confirmation, and Cancel restoring the prior state. Recorded the manual results with user-reported provenance. No automated tests or separate UI session were run by Codex; no commit was created. Paused at the Milestone 2B review boundary.

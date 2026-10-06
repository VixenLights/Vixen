# Open the Patching Order Zig Zag dialog safely (VIX-4007)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with `.agents/PLANS.md`. Issue: https://vixenlights.atlassian.net/browse/VIX-4007. The design was prepared using `.agents/skills/analyze-and-plan-issue/SKILL.md`. This document records the analysis and implementation evidence. Milestone 1 updated VIX-4007, and milestone 2 implemented the caller-side dialog fix.

## Purpose / Big Picture


In Display Setup, a user can configure Patching Order, select multiple elements in its list, and choose Zig Zag from the context menu. The numeric dialog must open even when fewer than 50 elements are selected. After the repair, the initial zig-zag length is the smaller of 50 and the selection count, and the user can choose any length from 2 through that count. Confirming a valid length keeps the existing alternating-group reversal behavior. Canceling the numeric dialog leaves the order untouched.

Demonstrate the repair with ten elements: select all ten in Patching Order and choose Zig Zag. Before the repair, creating the dialog throws an exception because its value is 50 and its maximum is 10. Afterward, it opens with value 10, minimum 2, and maximum 10. Choose length 2 and confirm to see every second pair reverse.

## Progress


- [x] (2026-10-06) Read the project analysis, Jira, .NET best-practices, and commit-message skills, and the complete `.agents/PLANS.md`.
- [x] (2026-10-06) Read VIX-4007, including its stack trace, status, comments, links, and attachment metadata. There are no comments, linked issues, or attachments.
- [x] (2026-10-06) Confirmed a clean starting worktree and inspected HEAD history, OrderSetupHelper, NumberDialog, the Order project, shared-dialog callers, and the relevant VIX-4006 plan.
- [x] (2026-10-06) Established the source-level cause and prepared a caller-scoped repair with concrete validation and review boundaries.
- [x] (2026-10-06) Milestone 1: Updated VIX-4007 with concise user outcomes, scope, and acceptance criteria; preserved the original report and exception trace. Read the saved description back and confirmed status remains In Progress.
- [ ] Milestone 2: Implementation, Release x64 Order module build, and Rider file diagnostics completed; manual numeric-dialog and reorder acceptance remains pending because no confirmed Release/Output launch with an isolated test profile is configured.
- [ ] Milestone 3: Record final validation in this plan and VIX-4007 when Jira reporting is authorized.

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

Observation: the Release x64 Order module build succeeded and emitted four existing warnings from Vixen.Core (two CS8632 nullable-context warnings, one CS0618 obsolete API warning, and one CS0067 unused event warning). Rider file analysis returned no errors, but many WEAK WARNING suggestions across the legacy file. The changed NumberDialog construction is flagged only for object allocation, which is inherent to creating this dialog. Gortex has no C# LSP provider, and the post-edit contract assessment remains warn (risk score 100, blast size 6) after reviewing the six-symbol impact around the existing menu event path.

Observation: the available Rider `Vixen.Application` configuration is a .NET project configuration and does not confirm a Release/Output launch or an isolated copied profile. Runtime acceptance was deferred to avoid launching the application with an unverified user profile or hardware-output configuration.

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

Decision: implement milestone 2 only in ZigZagItems_Click, leaving runtime acceptance pending until a Release/Output launch can be isolated from the user's profile and hardware output.
Rationale: the user explicitly authorized milestone 2. The available application run configuration does not confirm a Release/Output launch or copied test profile, so compilation and static diagnostics can be reported while UI acceptance remains unverified. Preserve the review boundary before milestone 3; AGENTS.md forbids commits unless explicitly requested.
Date/Author: 2026-10-06 / Codex.

## Outcomes & Retrospective


Milestone 1 is complete. VIX-4007 now records the user-facing summary, scope, acceptance criteria, ten-element scenario, and 49/50/51 boundary check. The original reproduction and exception evidence remain in the description, and the issue remains In Progress. Milestone 2 changed only ZigZagItems_Click: it returns for fewer than two selected rows, starts at Math.Min(50, selectedCount), keeps the 2..selectedCount range, and disposes the dialog after its value is consumed. The Release x64 Order module build and Rider file diagnostics completed; runtime dialog/reorder acceptance remains pending because no safe isolated Release launch was available. The design avoids changing a shared dialog API for one invalid caller and records the separate indexing concern so runtime acceptance does not accidentally claim coverage of that defect.

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

## Context and Orientation


Patching Order is an element property that controls the order of element sources during patching. OrderSetupHelper is the existing setup form for this property. Its Perform method populates a list of distinct leaf elements, shows the form, and writes the displayed order back to element properties only when the outer form returns OK. Its context menu offers Reverse and Zig Zag when more than one row is selected.

ZigZagItems_Click creates a NumberDialog, shows it, and calls PerformZigZag with the chosen integer only when the numeric dialog returns OK. PerformZigZag requires the selected count to be evenly divisible by that integer, then reverses every second group and reindexes displayed rows. ReIndexElementNodes updates the working lookup; the outer Perform confirmation is the existing persistence boundary. A leaf element is an individual element without child elements. A group is a parent containing those elements.

NumberDialog is a shared WinForms form backed by NumericUpDown, a control that enforces a minimum and maximum when its Value is assigned. Its constructor sets those bounds before assigning the initial value. The caller must supply a value within that interval. Enter returns OK and Escape returns Cancel. Existing callers and bounds remain unchanged.

Primary repository documentation for this area is the completed VIX-4006 plan at `docs/plans/display-setup/vix-4006-patched-output-range-order.md`. It records why custom element order must remain independent of destination order and explicitly excludes this crash. Documentation and current source agree that this is a separate Patching Order setup concern.

## Plan of Work


Execute the three milestones in order. The user authorized milestone 2; complete its review boundary before milestone 3. Do not perform future Jira writes merely while authoring or reviewing the plan. Every execution milestone ends at the explicit review boundary required by the analysis skill. Generate commit messages for repository changes with the project commit-msg skill, but create commits only when explicitly requested.

The single source edit replaces the current NumberDialog construction statement with a guarded local block. Keep ShowDialog, the OK check, button disabling/enabling, PerformZigZag, and persistence unchanged. Keep tabs and LF line endings; do not reformat untouched code or clean up existing warnings.

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

### Milestone 3: Record the evidence and align the issue with the delivered fix


Context: no further source changes are planned. Review the module build, diagnostics, matrix, order, cancellation, and persistence results from Milestone 2. The related VIX-4006 fix must remain intact. Complete any acceptance that was deferred before describing the runtime behavior as verified.

Plan of Work: update this plan's living sections with exact commands, exit results, manual outcomes, and any unavailable checks. If manual application access was unavailable, clearly retain that acceptance as pending and state precisely what build/source checks established. Do not mark implementation fully accepted from compilation alone. When Jira reporting is authorized, re-read VIX-4007, reconcile its user-facing acceptance criteria with the final delivered behavior, and add a concise validation comment. Preserve the original report/stack trace and status. Use the project Jira skill and read the result back.

Concrete Steps: working directory C:\Dev\Vixen. Inspect final scope and whitespace. If no source changed after Milestone 2 validation, do not repeat the build merely for another transcript.

    git status --short
    git diff --check
    git diff -- src/Vixen.Modules/Property/Order/OrderSetupHelper.cs docs/plans/display-setup/vix-4007-zigzag-dialog-range.md

Validation and Acceptance: the final handoff states the bounded default, minimum/maximum, cancellation behavior, actual alternating reversal and persistence evidence, module build result, Rider diagnostics, and any remaining limitation. VIX-4007's final comment reports only validation actually performed. No issue transition or automatic commit is part of this plan.

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

During planning, no build, automated tests, or runtime acceptance was run. Milestone 2 validation: `msbuild src/Vixen.Modules/Property/Order/Order.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -p:SolutionDir=C:/Dev/Vixen/ -v:m` exited 0 and produced `Release/Output/Module.Property.Order.dll`; four Vixen.Core compiler warnings were reported as detailed above. Rider `get_file_problems` for this file (`rootFolder=C:/Dev/Vixen`, `errorsOnly=false`) returned no errors and legacy weak warnings; the warning on the changed dialog allocation is inherent to construction. `git diff --check` exited 0. Gortex tests found no test files; guards found no rules; contract stayed at warn after impact review (6 affected symbols); C# diagnostics had no LSP provider. Manual UI matrix, alternating-order checks, cancellation, and persistence were not run because the available app run configuration does not confirm Release/Output or an isolated test profile. Record these as pending, not passed. Existing unrelated diagnostics are not authorization for cleanup.

## Idempotence and Recovery


Re-read status/source before every edit and preserve unrelated user changes. The replacement is applied only once at the identified construction point. Repeated dialog cancellation must not accumulate changes. Repeated intentional confirmation may alternate the affected groups again; that is the existing behavior, not an idempotent command.

Keep experiments confined to a copied profile and avoid hardware output. Cancel the outer helper when discarding a trial; save only the copy when deliberately checking persistence. Retry builds after resolving actual environmental errors without substituting dotnet's MSBuild for native dependency builds. If rollback is needed, remove only this issue's reviewed edits; do not reset the workspace, delete profiles, or undo VIX-4006.

## Artifacts and Notes


Planning evidence: VIX-4007 reports ArgumentOutOfRangeException with Actual value 50 at NumberDialog.cs line 14, called by OrderSetupHelper.cs line 82. Both files match that trace. Eight textual NumberDialog construction matches were found, including one commented-out call; this establishes shared usage, not an exhaustive audit of every caller's correctness. Gortex impact identified ElementListOnMouseClick as the direct event-registration path and zero test files. An impact query for the new plan returned file_not_indexed because it did not yet exist in the graph.

No external resources or attachments are required to follow this plan. Its source and mathematical evidence are sufficient for the reported crash. Rider findTests was attempted but canceled without a result. This limitation does not change the direct source diagnosis.

Planning validation passed a PowerShell structural check for all required sections, three milestones, three explicit review stops, no triple-backtick fences, and the required concluding statement. Final git status --short showed only this untracked plan; git diff --check returned no errors. Because tracked diff checks omit new files, git diff --no-index --check -- NUL docs/plans/display-setup/vix-4007-zigzag-dialog-range.md was also run: it emitted no whitespace diagnostics and returned 1 for a differing file. Gortex detect likewise reported no tracked changes and explicitly excludes untracked files. Symbol-level tests/guards/contract checks do not apply to this Markdown-only addition. Rider's post-edit hook excluded the Markdown file because it is not part of a solution project; no C# lint or build result is claimed.

## Interfaces and Dependencies


Keep these signatures and existing dependencies unchanged:

    private void OrderSetupHelper.ZigZagItems_Click(object sender, EventArgs e)
    private void OrderSetupHelper.PerformZigZag(int every)
    public bool OrderSetupHelper.Perform(IEnumerable<IElementNode> selectedNodes)
    public NumberDialog(string title, string prompt, int value, int minimum = 0, int maximum = int.MaxValue)
    public int NumberDialog.Value { get; }

Use the existing System.Math.Min method, ListView selection collection, NumberDialog, WinForms DialogResult, and disposal support. No new package, project, configuration, data migration, interface, production helper, or public API is required.

## Revision Notes


2026-10-06 / Codex: Created the VIX-4007 design from the issue's exception evidence and the current caller/constructor source. Chose a bounded caller default with a small selection guard and local dialog disposal. Recorded the separate selected-row indexing concern, inconclusive Rider test lookup, existing Jira status, and future Jira/review boundaries. Application implementation and validation remain pending.

2026-10-06 / Codex: Recorded planning structure, whitespace, and worktree validation. Clarified that partial-selection dialog acceptance begins after the first row, and recorded the distinction between untracked-plan checks and source diagnostics.

2026-10-06 / Codex: Completed milestone 1 by updating VIX-4007 with the planned user-facing summary, scope, acceptance criteria, and 49/50/51 scenario. Read the saved description back and verified the original exception evidence remains and status is In Progress. Paused for milestone review as required by the plan.

2026-10-06 / Codex: Implemented milestone 2's guarded, bounded, disposable NumberDialog construction in ZigZagItems_Click. The Order module Release x64 build and whitespace check passed; Rider diagnostics had no errors, with the changed dialog allocation's expected weak warning and unrelated legacy warnings. Gortex found no tests/guard rules and retained a contract risk warning after impact review. Runtime acceptance remains pending because the available application run configuration does not verify Release/Output or an isolated test profile. No Jira update or commit was made. Paused at milestone 2's review boundary before milestone 3.

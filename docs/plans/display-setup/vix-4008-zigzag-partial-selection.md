# Apply Patching Order Zig Zag only to selected rows (VIX-4008)

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with `.agents/PLANS.md`. Issue: https://vixenlights.atlassian.net/browse/VIX-4008. This plan was prepared using the project analyze-and-plan-issue skill. At plan creation, implementation and Jira updates had not started; Milestone 1 later aligned the issue description with the confirmed reproduction.

## Purpose / Big Picture


A user selecting only part of the Patching Order list must be able to apply Zig Zag without moving an unselected element. Zig Zag groups the selected elements in their displayed order, leaves the first group unchanged, reverses the second, and continues alternating. Gaps between selected rows do not participate in a group. Every unselected element stays in its original list position.

With ten elements initially named Element 1 through Element 10, select Elements 3, 4, 5, and 6 and apply length 2. The resulting identities must be 1,2,3,4,6,5,7,8,9,10. From the same initial order, select Elements 2, 4, 6, and 8 and apply length 2. The result must be 1,2,3,4,5,8,7,6,9,10. Compare element names and object identities, because the displayed order numbers are refreshed after reordering.

The existing numeric-dialog defaults, length bounds, divisibility warning, keyboard selection, numeric cancellation, and outer OK/Cancel behavior must continue to work. Confirming the outer dialog saves the displayed order; canceling it leaves the previously saved order intact.

## Progress


- [x] (2026-10-06) Read the project analyze-and-plan-issue, Jira, .NET best-practices, and C# documentation skills and the complete `.agents/PLANS.md`.
- [x] (2026-10-06) Read VIX-4008, its linked VIX-4007 summary and latest reproduction comment; there are no attachments and no unresolved requirement ambiguities.
- [x] (2026-10-06) Confirmed a clean starting worktree at HEAD 79d69d922, the VIX-4007 merge. Inspected OrderSetupHelper, its Designer, NumberDialog, project references, the VIX-4007 plan, relevant VIX-4006 documentation, and existing Display Setup test conventions.
- [x] (2026-10-06) Diagnosed the selected-position versus full-list-index error and designed a private-method repair with actual-handler regression coverage and manual dialog acceptance.
- [x] (2026-10-06) Validated the saved plan: 14 required sections, three milestones and review stops, no Markdown fences, correct full-MSBuild test workflow, clean whitespace, and only this new untracked plan in git status.
- [x] (2026-10-06) Milestone 1: Updated VIX-4008 to state that the reporter manually reproduced the defect against the listed scenarios; read back the saved issue and confirmed both expected sequences, acceptance criteria, and In Progress status were retained.
- [x] (2026-10-06) Added six StaFact regressions that invoke the real private handler. After fixing fixture-only compile/assertion issues, the pre-fix run failed four cases and passed two; both ticket cases failed on the incorrect identity at displayed position 3.
- [x] (2026-10-06) Mapped swap positions through the frozen selected-index array. Full-MSBuild Release test-target builds and focused runs passed; final focused result: 6 passed, 0 failed. Rider checks found no diagnostics on changed production lines; test-file allocation suggestions remain.
- [x] (2026-10-06) User-reported manual checks: manual build and all 1051 unit tests pass; the continuous and gap selection scenarios from Jira work correctly; outer OK persists changes and Cancel reverts them.
- [x] (2026-10-06) User additionally confirmed Ctrl+A, Ctrl/Shift-click behavior, and all VIX-4007 boundary checks still pass.
- [x] (2026-10-06) User confirmed the VIX-4008 invalid-length check: with four selected rows and length 3, the warning appears and confirming it leaves the order unchanged.
- [x] (2026-10-06) Milestone 3: Re-read VIX-4008; its user-facing description already matched the confirmed behavior and acceptance criteria, so no description edit was needed. Added validation comment 40572 with automated and user-reported manual results, then read the issue back and confirmed the comment, description, and In Progress status.

## Surprises & Discoveries


Observation: before Milestone 1, the description said the bug had not been manually reproduced, but Jeff Uchitjil's comment dated 2026-10-06 15:05 America/Chicago confirmed reproduction against the ticket's test criteria. Milestone 1 replaced that stale statement; this report is evidence of the existing defect, not validation of the proposed repair.

Observation: PerformZigZag already takes a stable copy of the selected full-list indexes, but only uses its length when swapping. For a selection at zero-based indexes 2,3,4,5 and length 2, the second group's selection positions are 2 and 3. The current code incorrectly swaps full-list indexes 2 and 3 instead of mapping them to indexes 4 and 5.

    int[] selectedIndexes = new int[elementList.SelectedIndices.Count];
    elementList.SelectedIndices.CopyTo(selectedIndexes,0);
    var i1 = elementList.Items[start];
    var i2 = elementList.Items[end];

Observation: all-row selection hides the defect because selectedIndexes[k] equals k. The completed VIX-4007 plan explicitly separated this indexing concern from its dialog and keyboard repairs. Its earlier successful boundary checks do not validate VIX-4008.

Observation: this form is legacy WinForms. The Designer declares elementList as Common.Controls.DragDropListView.DragDropListView, a ListView subclass. It is not a Catel view model or MultiSelectTreeview. Some graph relationships identify similarly named update methods on other controls; current source and Designer establish the control actually involved.

Observation: ReIndexElementNodes updates the list's order text and the form's private working dictionary. Only Perform, after an outer DialogResult.OK, assigns OrderModule.Order to element properties. Testing the private reorder method can therefore check actual row movement without showing a modal form or modifying persisted element properties.

Observation: Gortex impact for PerformZigZag reports MEDIUM risk, eight affected graph entries including parameters and the existing click/menu path, and zero test files. A text search found no OrderSetupHelper or Zig Zag tests in src/Vixen.Tests. Rider findTests was attempted but did not return before its orchestration was canceled; do not describe that as a completed no-tests result. A scoped follow-up task exploration rejected its path filter; direct source reads and impact supplied the required evidence.

Observation: src/Vixen.Tests/Setup/SetupPatchingSimpleOutputOrderTests.cs already uses StaFact, named child-control lookup, reflection to invoke private production behavior, and a non-parallel collection. The tests project already references Vixen.Application, which references the Order module. Existing xUnit v3, Xunit.StaFact, and Moq dependencies support focused tests without a new production API or package.

Observation: the first regression build exposed a conditional-expression type mismatch in the new fixture. After correcting it, an initial test run exposed a reference-equality assertion issue with Moq interface proxies; the helper was changed to assert each node by reference. The subsequent pre-fix run had no setup failures and demonstrated the defect in the four offset/gapped cases while the all-row and single-group compatibility cases passed.

## Decision Log


Decision: map selection positions through selectedIndexes immediately before each swap and use the resulting full-list indexes for both reads, both replacements, and both Selected assignments.
Rationale: this fixes contiguous offset and gapped selections using the existing stable snapshot and alternating loop. Changing only the reads, or only the writes, would still move the wrong rows. Mapping by the first selected index would handle offsets but fail for gaps.
Date/Author: 2026-10-06 / Codex.

Decision: keep the method private and preserve the existing ListViewItem clone-and-replacement sequence, group loop, divisibility check, reindexing, and UI batching.
Rationale: the error is the index mapping, and the existing replacement sequence already supports all-row Zig Zag. No interface, lifecycle, data-model, public API, or shared-control redesign is needed. The .NET skill permits maintaining legacy WinForms UI; its guidance does not require converting this form to WPF.
Date/Author: 2026-10-06 / Codex.

Decision: add focused tests that invoke the actual private handler on a real ListView using the established Display Setup test pattern.
Rationale: this bug can move an unselected element and persist an unintended patching order. The regressions exercise real replacement, selection, Tag identity, and reindexing; they are not a separate implementation of the formula. Reflection stays in test helpers, with asserted member lookup, and avoids widening production visibility. Manual acceptance remains necessary for dialogs and persistence.
Date/Author: 2026-10-06 / Codex.

Decision: preserve the VIX-4007 and VIX-4006 implementations and plans; author a separate VIX-4008 plan.
Rationale: the current checkout includes the accepted dialog and keyboard repairs. Destination-output ordering in VIX-4006 is a separate consumer concern. This issue changes the selected source-element ordering only.
Date/Author: 2026-10-06 / Codex.

Decision: perform only analysis and create this plan now. Its Jira and implementation milestones are future execution steps.
Rationale: the user requested a design. No issue fields, comments, application files, tests, or commits are changed during planning. Each future milestone retains the explicitly invoked analysis skill's review stop; commits require an explicit user request under AGENTS.md.
Date/Author: 2026-10-06 / Codex.

## Outcomes & Retrospective


Milestone 1 aligned VIX-4008 with the reporter-confirmed reproduction. Milestone 2 added six actual-handler STA regressions and mapped selected-subset positions back to full-list row indexes for every read, replacement, and selection update in the swap. Before the production edit, four tests failed on offset/gapped row identities and two compatibility cases passed. After the edit, the Release Vixen_Tests target built successfully and all six focused tests passed. The user subsequently reported that a manual build and all 1051 unit tests pass, both Jira scenarios (continuous and gap selection) now work correctly, outer OK persists changes, and Cancel reverts them. The user additionally confirmed Ctrl+A, Ctrl/Shift-click behavior, and all VIX-4007 boundary checks pass. The user also tested four selected rows with length 3: the invalid-divisibility warning appeared, and confirming it left the order unchanged. Milestone 3 re-read VIX-4008 and found its description already reflected the confirmed reproduction and acceptance criteria, so it was left unchanged. A concise final validation comment (Jira comment 40572) was added and read back; the issue remains In Progress as required.

Validation commands and results: `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` first exited 1 on the fixture's CS0173 conditional-expression type mismatch; after correcting the fixture it exited 0, and after the production edit it exited 0 again. Successful builds reported existing nullable warnings in other test files. `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Vixen.Tests.Setup.OrderSetupHelperZigZagTests"` exited 1 before the fix with 4 failed and 2 passed, then exited 0 after the fix with 6 passed and 0 failed. The user additionally reports a successful manual build and 1051 passing unit tests; command transcripts were not provided. `git diff --check` exited 0; `git diff --no-index --check -- NUL src/Vixen.Tests/Setup/OrderSetupHelperZigZagTests.cs` exited 1 with no whitespace diagnostics because the test file is untracked. Rider diagnostics reported no issue on changed production lines; the new test file has weak allocation suggestions only. Gortex change detection reports the production method's eight-entry blast radius; coverage analysis does not link the untracked test file, guards have no configured rules, and contract continues to warn about high change risk. Manual acceptance of both ticket scenarios, outer OK/Cancel persistence, Ctrl+A, Ctrl/Shift-click behavior, all VIX-4007 boundary checks, and the VIX-4008 invalid-length warning is complete per the user's reports. With four selected rows and length 3, the warning appeared and confirming it left the order unchanged.

## Architecture Design: VIX-4008 — Patching Order Zig Zag reorders wrong elements for partial selections


Detected IDE Environment: Rider with automation hooks. The plan supplies a small block that can be applied through Rider's Apply snippet from chat. Rider get_file_problems is available for each changed C# file, and its test runner can execute the focused StaFact tests. During agent execution, use the required repository edit tooling and Gortex impact/post-edit assessments.

Core Strategy: correct the mapping from a position in the selected subset to a position in the displayed list. Apply `.agents/skills/dotnet-best-practices/SKILL.md` for focused C# maintenance, meaningful names, disposal of test forms, and actual-behavior testing. Apply `.agents/skills/csharp-docs/SKILL.md` for the new public test type and methods. The production edit changes a private method body only. Catel, async, and design-pattern skills are not needed because no WPF binding, asynchronous operation, interface, boundary, or object lifecycle changes.

Data Model & Property Contracts: no new fields, settings, serialized members, interfaces, or production declarations. Each swapped row continues to carry its original IElementNode in ListViewItem.Tag. The private working order is recomputed once after all swaps; saved OrderModule properties are assigned only on outer OK. Selected positions remain the same set throughout the operation, and the moved elements remain selected. Preserve the existing numeric default Math.Min(50, selectedCount), minimum 2, maximum selectedCount, and early return for fewer than two selected rows.

Mathematical / Boundary Logic: let S be the frozen ascending list of selected full-list indexes, n its length, and L the chosen length. The existing dialog enforces 2 <= L <= n; the existing warning rejects n % L != 0 before any reordering. Selection positions 0 through L-1 form the first group and stay unchanged. For group starts i = L, 3L, 5L, and so on below n, swap selection positions start = i and end = i+L-1 while start < end, then move start forward and end backward. The actual row indexes are S[start] and S[end]. An odd-length reversed group leaves its middle selected element in place. L = n leaves the one group unchanged. All-row selection yields S[k] = k and preserves the old correct result.

Every mapped index belongs to the selected set. Item replacement keeps the total list count fixed, and the saved S array is unaffected by live selection updates. Consequently no unselected list position is written. The existing clone retains the row's Tag and subitems; identity assertions verify that there are no lost or duplicated elements. Time remains proportional to the selected count plus the existing full-list reindexing; the repair allocates only two integer locals beyond existing allocations.

Subsystem Component Matrix:

| File or component | Role and planned modification |
| --- | --- |
| src/Vixen.Modules/Property/Order/OrderSetupHelper.cs | Only production change: map all six Items accesses within the private PerformZigZag swap block. |
| src/Vixen.Tests/Setup/OrderSetupHelperZigZagTests.cs | New focused real-control regression tests and private test helpers. |
| src/Vixen.Tests/Vixen.Tests.csproj and src/Vixen.Application/Vixen.Application.csproj | Existing test dependencies and transitive Order reference; no planned edit. |
| src/Vixen.Modules/Property/Order/OrderSetupHelper.Designer.cs and src/Vixen.Common/Controls/NumberDialog.cs | Reference for actual controls and modal contracts; no planned edit. |
| docs/plans/display-setup/vix-4008-zigzag-partial-selection.md | Living design, decisions, and validation record. |
| VIX-4008 | Initial user-facing acceptance alignment and final validation reporting during execution. |

## Context and Orientation


Patching Order is an element property used to determine the source-element sequence during patching. A leaf element is an element without children. OrderSetupHelper.PopulateElementList gathers distinct leaves, orders them by their existing Order property, and creates rows whose Tag identifies each leaf. Each row has a displayed order number and element name. ReIndexElementNodes updates those numbers and the form's private dictionary to match the current list.

The helper's context menu offers Reverse and Zig Zag when more than one row is selected. ZigZagItems_Click shows NumberDialog and invokes PerformZigZag only after numeric OK. The private method copies SelectedIndices before editing rows, checks divisibility, reverses every second group, and reindexes. Perform shows the outer form and writes properties only if it returns OK. Numeric Cancel does not call the reorder method; outer Cancel does not persist the working order.

Read `docs/plans/display-setup/vix-4007-zigzag-dialog-range.md` for historical acceptance of the current dialog and keyboard behavior and `docs/plans/display-setup/vix-4006-patched-output-range-order.md` for the separation of source order and destination order. Required behavior is embedded here: Ctrl alone preserves selection, Ctrl-click toggles just the clicked row, Ctrl+A selects all, and plain A does not invoke the custom Select All handler. Destination sorting and range labels are outside this repair.

The test project is src/Vixen.Tests/Vixen.Tests.csproj. It uses xUnit v3 and Xunit.StaFact for Windows control tests. StaFact runs a test in the single-threaded apartment required by these controls. Existing Display Setup tests use Vixen.Tests.Core.OutputControllerOutputIndexTestCollection, which prevents parallel execution around process-wide Vixen managers. Reuse that collection for consistency with nearby control tests; this new fixture must not replace any managers or persist real element properties.

Full MSBuild is required to build Vixen.Tests because transitive QMLibrary and LiquidFunWrapper projects need the Visual C++ toolset. dotnet test is used only after the full-MSBuild build, with --no-build --no-restore. Commands below assume Windows, the installed .NET 10 SDK, and full MSBuild on PATH. If unavailable, use a Visual Studio Developer PowerShell with the C++/CLI toolset installed; do not substitute dotnet build for the native-project build.

## Plan of Work


Execute the initial Jira alignment milestone, then the code-and-validation milestone, then the closing evidence milestone. Do not perform these steps as part of drafting the plan. Before each production edit, inspect git status, re-read source, and assess PerformZigZag impact. Preserve unrelated user changes. Use tabs and LF in C# and avoid formatting untouched blocks.

Create actual-handler regressions before the source repair and record the observed failures. Then add startIndex and endIndex inside the existing while loop and use them in all six row accesses. No declaration or signature changes are required. Rebuild tests, run the focused suite, perform file-problem checks, and exercise the dialogs and persistence. Record implementation and pending manual acceptance separately if UI checks are unavailable. Do not mark the repair fully accepted from compilation alone.

### Milestone 1: Align the confirmed bug and acceptance in Jira


Context: VIX-4008 already contains the required two ten-element examples and useful acceptance criteria. Before this milestone, its description stated reproduction was unconfirmed, conflicting with the latest comment. The only repository file edited in this milestone is this living plan's progress/evidence record.

Plan of Work: use the project Jira skill and re-read the current issue before writing. Replace the outdated reproduction statement with a concise acknowledgement that the reporter confirmed both scenarios. Preserve the original examples, acceptance criteria, linked VIX-4007 scope, and comments. Retain or tighten user-facing wording for alternating selected groups, unchanged unselected positions, no missing or duplicated elements, full-selection correctness, invalid-length warning, and both cancellation boundaries. Keep the issue description free of formulas, class names, file paths, and test-seam details; those belong here. Preserve the issue status and do not add a completion claim.

Concrete Steps: working directory C:\Dev\Vixen. Inspect the starting state, then perform the scoped description update through Atlassian MCP and read the saved issue back. Expected read-back: the reporter-confirmed reproduction replaces the stale sentence, both identity sequences remain, and status is unchanged.

    git status --short
    git diff -- docs/plans/display-setup/vix-4008-zigzag-partial-selection.md

Validation and Acceptance: a reader can understand the confirmed bug and the required results without reading source code. The saved issue retains the two exact scenarios and existing VIX-4007 protections. Update Progress with actual read-back evidence.

STOP HERE for manual review and commit execution before proceeding.

Halt execution at this milestone. Run git status --short and scoped git diff for changed repository files, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4008 as the subject prefix, and output its complete paste-ready Commit message block. Do not create a commit unless explicitly requested. Wait for explicit user confirmation before advancing.

### Milestone 2: Prove and repair selected-row swaps


Context: the production edit is inside PerformZigZag in C:\Dev\Vixen\src\Vixen.Modules\Property\Order\OrderSetupHelper.cs. New tests belong in C:\Dev\Vixen\src\Vixen.Tests\Setup\OrderSetupHelperZigZagTests.cs. Use the nearby SetupPatchingSimpleOutputOrderTests as the actual-control, StaFact, reflection, and named-control reference. Re-read the project .NET and C# documentation skills before implementing.

Plan of Work: create one public sealed OrderSetupHelperZigZagTests class in namespace Vixen.Tests.Setup with six public StaFact methods and private fixture/assertion helpers. Document the class and public methods with XML summaries describing the behavior being verified. Apply the existing non-parallel collection attribute described above. Instantiate and dispose an OrderSetupHelper within each test, find the named child elementList through Controls.Find(name, true), and assert it is the actual DragDropListView. Create its native handle before setting row selections so SelectedIndices reads real WinForms selection state on a hidden control.

Populate the list directly with uniquely named ListViewItems whose Tag values are distinct Moq IElementNode instances with matching Name properties. Include the second, element-name subitem. Do not call the modal Perform or ZigZagItems_Click methods, register real modules, or change VixenSystem managers. Select the scenario's rows, deliberately setting selections in descending click order for a gapped case, then assert SelectedIndices matches the ascending displayed positions before invoking the method. Assert reflection lookup succeeds for the private instance method PerformZigZag, and invoke it with the valid length. Tests must call the production method, never a copied swap implementation.

For each case, assert the complete expected element-name sequence and expected original Tag references, list count, and identity uniqueness. Assert unselected positions still contain the very same original ListViewItem objects and are not selected. Assert the selected-index set remains unchanged and all moved elements stay selected. Assert row Text values are 1 through the row count and the private _elementOrderLookup dictionary maps each Tag to its displayed one-based index. Then invoke Zig Zag a second time with the same selection and length and assert the initial identity sequence is restored; this verifies that live selection changes did not corrupt the saved-index behavior.

Use exactly these six independent cases. Numbers denote element identities, not refreshed order-column text.

| Test method | Initial rows; selected identities; length | Expected identities after one application |
| --- | --- | --- |
| ContiguousOffsetSelection_ReversesOnlySecondSelectedGroup | 1..10; 3,4,5,6; 2 | 1,2,3,4,6,5,7,8,9,10 |
| GappedSelection_ReversesOnlySelectedPositions | 1..10; 2,4,6,8; 2 | 1,2,3,4,5,8,7,6,9,10 |
| AllRowsSelection_PreservesAlternatingPairBehavior | 1..10; all; 2 | 1,2,4,3,5,6,8,7,9,10 |
| GappedOddLength_ReversesGroupAndKeepsMiddleIdentity | 1..10; 2,4,6,7,9,10; 3 | 1,2,3,4,5,6,10,8,9,7 |
| MultipleGappedGroups_ReverseEverySecondGroup | 1..16; 2,4,6,8,10,12,14,16; 2 | 1,2,3,4,5,8,7,6,9,10,11,12,13,16,15,14 |
| LengthEqualsSelectedCount_LeavesOneGroupUnchanged | 1..10; 2,4,6,8; 4 | 1,2,3,4,5,6,7,8,9,10 |

Build and run these tests before the repair. At least the two ticket scenarios must fail on incorrect identities; record actual totals and failure details. If the first failure concerns setup, dependency loading, or zero selected indexes, fix only the fixture, verify its selected-index assertion, and rerun before attributing failure to the production bug.

In the existing while loop, replace the swap block with the following statements, using Rider Apply snippet from chat or the required guarded edit tool. Keep start/end as positions within the frozen selection. Leave their increments and all enclosing control flow intact.

    // Map positions within the selection to rows in the full list.
    var startIndex = selectedIndexes[start];
    var endIndex = selectedIndexes[end];
    var i1 = elementList.Items[startIndex];
    var i2 = elementList.Items[endIndex];
    elementList.Items[startIndex] = (ListViewItem)i2.Clone();
    elementList.Items[endIndex] = i1;
    elementList.Items[startIndex].Selected = true;
    elementList.Items[endIndex].Selected = true;

Preserve private void PerformZigZag(int every), the selection-copy statement, divisibility branch and warning text, group starts, clone semantics, reindexing call, BeginUpdate/EndUpdate, cursor behavior, and the click handler. No public/protected production API changes are planned. Do not add a new ordering service, change Reverse, modify the Designer, or reformat surrounding legacy code. The existing transitive project reference should compile the new tests. If an actual compiler failure establishes that Order needs a direct reference, add only a ProjectReference to ..\Vixen.Modules\Property\Order\Order.csproj with Private=false and IncludeAssets=none, document that evidence here, and repeat the build. No DLL reference, package, solution entry, or friend-assembly attribute is needed.

Concrete Steps: all commands run from C:\Dev\Vixen. Use full MSBuild for the test target. Start long commands with native exec_command and yield_time_ms=10000; when only waiting, poll write_stdin with yield_time_ms at least 30000. Run the build and focused test command before and after the production repair.

    git status --short
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Vixen.Tests.Setup.OrderSetupHelperZigZagTests"
    git diff --check
    git diff -- src/Vixen.Modules/Property/Order/OrderSetupHelper.cs src/Vixen.Tests/Setup/OrderSetupHelperZigZagTests.cs src/Vixen.Tests/Vixen.Tests.csproj docs/plans/display-setup/vix-4008-zigzag-partial-selection.md

Expected after the repair: build exits 0, all six focused cases pass, and no whitespace diagnostics are emitted. Record actual counts, exit codes, and warnings. Do not assume prior VIX-4007 test counts remain current. Use git diff --no-index --check -- NUL followed by the new test path to check untracked-file whitespace; exit 1 with no whitespace diagnostics is the expected differing-file result.

Run Rider get_file_problems for both changed C# files with rootFolder=C:/Dev/Vixen, errorsOnly=false, and a bounded timeout. Inspect every result, and fix diagnostics only on added or changed lines. Existing unrelated suggestions are not cleanup scope. Run Gortex change detect after edits, then tests, guards, and contract for the changed method. Record unavailable checks honestly; graph warnings or missing test links do not substitute for the actual regression results. No signature verification is needed because the production signature is unchanged. An untracked test file may be absent from tracked-diff assessments; review it directly.

Validation and Acceptance: run the built Release application from Release/Output using a copied test profile with ten uniquely named leaf elements and no active hardware output. Open Display Setup and the Patching Order helper through the existing property-configuration action. Reset the identity order to 1..10 before each independent scenario. For each ticket case, select exactly the listed identities, right-click a selected row, choose Zig Zag, set length 2, and confirm. Expect the two sequences in Purpose, every unselected element in its original slot, no missing or duplicated identity, selected rows still selected, and sequential displayed order numbers. Repeat the gapped case selecting rows in a different click order; displayed-order grouping must produce the same result.

Select all ten with Ctrl+A and apply length 2; expect 1,2,4,3,5,6,8,7,9,10. For four selected rows, enter length 3: expect the existing divisibility warning and no order, selection, or saved-property change. For four selected rows, accept length 4: expect unchanged order. Cancel the numeric dialog after changing its input and confirm the working order stays unchanged. For a valid reordered subset, confirm the outer dialog, reopen it, and observe the saved sequence. From a saved baseline, make a valid reorder and cancel the outer dialog; reopening must show the saved baseline. Exercise both ticket selections through outer OK and Cancel.

Recheck the VIX-4007 protections with selection counts 2, 10, 49, 50, and 51: the dialog opens, default is the count up through 50 and 50 thereafter, minimum is 2, maximum is selected count. At count 51, confirming the default 50 must warn without changing order. Ctrl alone preserves a single or gapped selection, Ctrl-click toggles only its clicked row, Ctrl+A selects all, and plain A does not invoke Select All. These gestures must not reorder identities.

If UI access is unavailable, leave these manual checks explicitly pending until actual results are available; record automated implementation verification separately. Do not launch an unverified profile or claim the modal acceptance passed from the private-method tests. Do not repeat broader builds or full-suite tests absent a concrete new concern; the relevant six-case suite and the actual UI checks establish this local repair.

STOP HERE for manual review and commit execution before proceeding.

Halt execution at this milestone. Update the living sections with the observed failing-before/passing-after evidence and manual outcomes. Run git status --short and scoped git diff for every changed repository file, review any new untracked file directly, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4008 as the subject prefix, and output its complete paste-ready Commit message block. Do not create a commit unless explicitly requested. Wait for explicit user confirmation before advancing.

### Milestone 3: Reconcile acceptance and record the delivered evidence


Context: the production repair and test code should be complete and accepted. This milestone updates this plan and VIX-4008; it does not introduce further application changes. Complete any pending acceptance before calling the issue fully verified.

Plan of Work: record exact commands, before/after test results, Rider diagnostics, actual manual observations, and limitations in Progress and Outcomes & Retrospective. Distinguish agent-run evidence from reporter/user observations. Re-read VIX-4008 and adjust only user-facing requirements that changed during implementation, preserving original reproduction examples and comments. Add one concise final comment reporting that partial selections now reverse selected groups while leaving unselected elements fixed, followed by actual automated/build/manual validation results. Do not claim unavailable UI checks passed, change status, or close the issue automatically. Use the project Jira skill and read back all writes.

Concrete Steps: working directory C:\Dev\Vixen. Review final scope and whitespace. If source is unchanged since successful Milestone 2 validation, do not rerun builds for a duplicate transcript.

    git status --short
    git diff --check
    git diff -- docs/plans/display-setup/vix-4008-zigzag-partial-selection.md

Validation and Acceptance: the plan and saved Jira text agree on confirmed behavior, two reproduction sequences, all-row compatibility, unchanged unselected positions, warning behavior, and OK/Cancel persistence. The final comment reports actual results and their provenance. Pending validation, if any, remains explicit. Read-back confirms the issue status was preserved.

STOP HERE for manual review and commit execution before proceeding.

Halt execution at this milestone. Run git status --short and scoped git diff, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4008 as the subject prefix for repository changes, and output its complete paste-ready Commit message block. Do not create a commit unless explicitly requested. Wait for explicit user confirmation before additional work.

## Concrete Steps


Planning commands run from C:\Dev\Vixen: git status --short returned no changes; git log -1 reported 79d69d922, the VIX-4007 merge; scoped rg searches located the existing documentation and confirmed no existing OrderSetupHelper/ZigZag test match. Jira was read through Atlassian MCP. No build, unit test, application session, or Jira write was run during analysis.

Milestone 2 contains the exact required full-MSBuild build and filtered no-build test commands. If a concrete failure expands the scope to shared behavior, record why and broaden validation accordingly:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/

Build the Vixen_Tests target first if the tests are not current; a solution build is not a substitute for confirming the specific test target succeeded. Record actual full-suite totals if this conditional check is run.

## Validation and Acceptance


Primary automated proof is the actual-handler suite's failing-before and passing-after identity results, including selected/unselected row membership, preserved Tag references, and order reindexing. All six named tests must pass after the repair. The expected test assertions are in Milestone 2 and must not be generated from a second copy of the production algorithm.

Primary runtime proof is the ticket's two partial-selection sequences in the actual UI, combined with full-selection compatibility, invalid divisibility, numeric cancellation, outer confirmation persistence, and outer cancellation restoring the saved baseline. The VIX-4007 dialog and keyboard protections must also pass. A successful build alone is not proof of those modal behaviors.

Plan-only validation must confirm all mandatory living sections, architecture design, three narrative milestones, three explicit review stops, exact build/test commands, no triple-backtick fences in the standalone Markdown, and that git status contains only this new plan. Use git diff --check for tracked changes and git diff --no-index --check -- NUL docs/plans/display-setup/vix-4008-zigzag-partial-selection.md for the new file. No C# file was generated in analysis, so C# lint and runtime/build validation are deferred to execution.

## Idempotence and Recovery


Re-read source and status before editing; apply the index replacement once, and validate already-equivalent code rather than duplicating the locals. Tests use fresh forms and mock nodes and dispose controls on the STA thread. They do not save profiles or element properties. Build retries are safe after resolving actual toolchain errors.

Applying the same valid Zig Zag twice to the same selected positions restores the initial identity sequence because each reversed group is reversed twice. This is an intentional toggle, not an operation that leaves the first result unchanged on repeated confirmation. Numeric Cancel changes nothing; outer Cancel discards working dictionary changes.

Keep manual experiments in a copied profile. Reset the working baseline between cases; do not reset the repository or alter real controller connections. If rollback is needed, revert only the reviewed VIX-4008 source/test/reference edits while preserving the VIX-4007 fixes and unrelated user changes. Read Jira before any retry to avoid duplicate comments or overwriting intervening edits.

## Artifacts and Notes


Source evidence anchors: OrderSetupHelper.cs lines 84 onward contain the guarded numeric caller; lines 106 onward contain PerformZigZag and all six incorrect Items[start/end] accesses; lines 153 onward contain Perform's outer-confirmation persistence; lines 225 onward contain ReIndexElementNodes. Line numbers may shift after edits; use the method names as stable anchors.

The new plan's pre-write impact assessment returned file_not_indexed because the file did not exist. The production method's impact was assessed independently. Gortex task localization initially ranked unrelated effect Zig Zag implementations above the actual property helper; the read source and explicit symbol impact narrowed the repair to the correct method. No unrelated effect implementation is in scope.

The new regression suite needs no serialized fixture, attached file, new package, or hardware. Both reported outputs and all extra expected outputs are specified explicitly in this plan. The pending manual matrix supplies the remaining modal and persistence proof.

Planning validation: a PowerShell structural check passed for all 14 required sections, three milestones, three explicit stops, no code fences, and the full-MSBuild/no-build-test workflow. git diff --check exited 0. git diff --no-index --check -- NUL docs/plans/display-setup/vix-4008-zigzag-partial-selection.md emitted no whitespace diagnostics and exited 1 because the new file differs from NUL. git status --short listed only this untracked plan. Gortex detect reported no tracked changes and explicitly excludes untracked files; subsequent impact for the indexed plan returned LOW risk and no dependents. The unchanged production symbol's tests assessment found no coverage, guards found no configured rules, and contract returned a risk warning for eight graph entries. These are planning assessments, not executed tests or a failed validation of edited C#; there is no C# change. C# lint, compilation, unit tests, and runtime checks were skipped because this request produces only the design document.

## Interfaces and Dependencies


Preserve all existing production signatures:

    private void OrderSetupHelper.PerformZigZag(int every)
    private void OrderSetupHelper.ZigZagItems_Click(object sender, EventArgs e)
    private void OrderSetupHelper.ReIndexElementNodes()
    public bool OrderSetupHelper.Perform(IEnumerable<IElementNode> selectedNodes)
    protected void OrderSetupHelper.OnKeyDown(object sender, KeyEventArgs e)

Use the existing System.Windows.Forms ListView item/index collections, ListViewItem.Clone, IElementNode Tag identities, and private working dictionary. The two new integer locals do not escape the swap loop. No constructor, field, service registration, descriptor, OrderData contract, or NumberDialog API changes are required.

The new test type is Vixen.Tests.Setup.OrderSetupHelperZigZagTests in its matching file. Its six public test methods are named in Milestone 2 and have XML summaries. Helpers remain private. Use existing xUnit v3, Xunit.StaFact, Moq, System.Reflection, and Common.Controls.DragDropListView dependencies. Prefer existing transitive Order module access; add the conditional direct project reference only if compilation demonstrates a need, following the prescribed no-copy project-reference settings.

## Revision Notes


2026-10-06 / Codex: Created the separate VIX-4008 design after reading the issue, confirmed reproduction comment, current source, prior documentation, and test patterns. Chose full-list index mapping within the existing private swap block and real-control regressions that preserve the production API. Recorded future Jira alignment, actual UI acceptance, and per-milestone manual review stops. Only this plan is created during analysis.

2026-10-06 / Codex: Recorded successful plan structure, whitespace, and worktree-scope checks. Documented the untracked-file limitation of diff-based detection and distinguished unchanged-symbol risk/coverage assessments from actual implementation validation. Builds, tests, and UI checks remain future execution work.

2026-10-06 / Codex: Completed Milestone 1 by updating VIX-4008's stale reproduction statement based on the reporter's confirmation, then reading the issue back. The expected sequences, acceptance criteria, and In Progress status were preserved. No repository implementation files were changed.

2026-10-06 / Codex: Implemented the selected-index mapping and six STA actual-handler regression cases for Milestone 2. The pre-fix focused run reported 4 failed and 2 passed; both ticket cases failed at the expected unselected identity. The post-fix Release test target built successfully and the focused run reported 6 passed. User-reported manual checks confirm both Jira selection scenarios, outer OK/Cancel persistence, Ctrl+A, Ctrl/Shift-click behavior, all VIX-4007 boundary checks, and the VIX-4008 invalid-length warning. With four selected rows and length 3, the warning appeared and confirming it left the order unchanged. The user also reports a manual build and 1051 passing unit tests.

2026-10-06 / Codex: Completed Milestone 3. Re-read VIX-4008; the description already reflected the confirmed behavior and criteria, so it remained unchanged. Added and read back final validation comment 40572 covering the implementation and reported build, unit-test, and manual results. Confirmed the issue remains In Progress.

Analysis complete and plan integrated with plans.md.

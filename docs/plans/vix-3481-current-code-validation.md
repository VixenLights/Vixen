# Prevent reference-alignment commands from using empty timeline space as a reference (VIX-3481)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

When a user preserves a multiple-effect selection with Ctrl and then right-clicks empty timeline space, Vixen currently offers reference-based alignment commands even though no effect was clicked. Choosing one of those commands can terminate the Sequencer with a null-reference exception. After this work, commands that require a clicked effect are disabled whenever the context-menu click was on empty space, while the two distribution commands remain available for a valid multiple-effect selection. The alignment engine itself will also safely ignore a null reference effect, preventing a future UI-state defect from crashing Vixen.

A user can see the fix by selecting two effects, holding Ctrl while right-clicking an empty timeline row, releasing Ctrl after the menu opens, and opening Alignment: Align Start Times and the other reference-based commands are disabled; Distribute Equally and Distribute Effects remain enabled. Right-clicking an effect continues to enable reference-based alignment where the existing count rules allow it.

## Progress

- [x] (2026-08-03) Research complete: confirmed the Ctrl-preserved-selection reproduction in `Grid_Mouse.cs`, the unsafe context-menu enablement in `TimedSequenceEditorForm_ContextMenu.cs`, and the seven public `Grid` methods that dereference the reference effect.
- [x] (2026-08-03) Milestone 1: Updated JIRA VIX-3481 with the corrected Ctrl-right-click reproduction, scope boundary, command-availability rules, acceptance criteria, and test plan. Confirmed its status remained `Accepted`; no transition was performed.
- [ ] Add the context-menu command-availability split and retain the current ordinary right-click selection behavior.
- [ ] Add null-safe guards and XML documentation to every public reference-alignment method.
- [ ] Add focused timeline-control regression tests, build, manually exercise the Sequencer scenarios, and update JIRA with final results.

## Surprises & Discoveries

- Observation: An ordinary empty-space right-click does not reproduce the crash because it clears selection before the context menu is built.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` clears selected elements, rows, and active rows only when `CtrlPressed` is false.

- Observation: The context menu has nine non-mark alignment actions, but only seven need an effect clicked under the cursor.
  Evidence: `TimedSequenceEditorForm_ContextMenu.cs` passes `element` to seven `Grid.AlignElement*` calls; `Distribute Equally` and `Distribute Effects` call editor methods without `element`.

- Observation: The public alignment boundary contains seven methods that dereference `referenceElement` after checking only selection eligibility.
  Evidence: `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` defines `AlignElementStartTimes`, `AlignElementEndTimes`, `AlignElementDurations`, `AlignElementStartEndTimes`, `AlignElementStartToEndTimes`, `AlignElementEndToStartTime`, and `AlignElementCenters`; each accesses a member of `referenceElement`.

## Decision Log

- Decision: Do not change mouse-selection behavior.
  Rationale: Ctrl-right-click intentionally preserves selection, while ordinary empty-space right-click intentionally replaces it. The defect is that menu availability treats selection count as proof that a reference effect exists.
  Date/Author: 2026-08-03 / Codex

- Decision: Use two local command-availability facts, `canReferenceAlign` and `canDistribute`, rather than one shared condition.
  Rationale: Reference-based operations require both a valid selected-effect count and the clicked reference effect; distribution needs only a valid multiple-effect selection. Keeping the facts separate makes this distinction apparent and prevents reintroducing the unsafe shortcut.
  Date/Author: 2026-08-03 / Codex

- Decision: Guard every public `Grid` reference-alignment method even after fixing the UI.
  Rationale: These public methods can be called by future UI, hotkey, or integration paths. A no-op for a null reference is safer than a process-terminating exception and does not change valid alignment behavior.
  Date/Author: 2026-08-03 / Codex

- Decision: Keep mark-alignment availability independent of the new two command facts.
  Rationale: The three “Align … to nearest mark” commands use marks rather than the clicked effect as their reference and are already enabled separately when a labeled mark collection contains marks.
  Date/Author: 2026-08-03 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. VIX-3481 now distinguishes the verified Ctrl-right-click current-code path from the unverified historical gesture and records the implementation and validation contract. At completion, record the affected files, automated-test results, Debug build result, manual regression result, and the final JIRA update/comment here.

## Context and Orientation

Vixen’s Timed Sequence Editor is a WinForms Sequencer built on the reusable timeline controls in `src/Vixen.Common/Controls/TimeLineControl`. An `Element` is the timeline-control object representing an effect’s start time, end time, duration, selected state, and row. A reference-alignment command moves or resizes selected effects relative to one clicked `Element`. A distribution command arranges selected effects without such a clicked reference.

`src/Vixen.Common/Controls/TimeLineControl/Grid_Mouse.cs` handles mouse-down selection. When the mouse-down hit test produces no elements and Ctrl is not held, `OnMouseDown` clears current selections. Holding Ctrl bypasses that clearing, so an empty-space right-click can open a context menu with existing selected effects but no `ElementsUnderCursor` entry. This behavior is intentional and outside the change boundary.

`src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs` builds the Sequencer effect/timeline context menu in `timelineControl_ContextSelected`. It derives `element` from `e.ElementsUnderCursor.FirstOrDefault()`, which is null for the empty-space case. The current single condition enables all reference and distribution commands when more than one effect remains selected, and each reference command closure passes `element` into `TimelineControl.grid`.

`src/Vixen.Common/Controls/TimeLineControl/Grid.cs` implements alignment. `OkToUseAlignmentHelper(IEnumerable<Element>)` rejects invalid alignment selection states, including more than 32 selected effects in a row. It does not validate a reference effect. The following public methods all require a non-null reference effect: `AlignElementStartTimes`, `AlignElementEndTimes`, `AlignElementDurations`, `AlignElementStartEndTimes`, `AlignElementStartToEndTimes`, `AlignElementEndToStartTime`, and `AlignElementCenters`.

`src/Vixen.Tests/Sequencer/TimelineCursorSelectionTests.cs` demonstrates how to create a `TimelineControl`, add `Element` instances to rows, and isolate timeline tests using `[Collection(TimelineControlTestCollection.Name)]`. This is the appropriate home and pattern for direct `Grid` null-reference regression tests. The UI form is not currently a lightweight unit-test seam, so command enabled-state verification belongs in manual Sequencer validation unless implementation extracts a pure helper without widening production API surface.

## Plan of Work

### Milestone 1 — Record the corrected issue contract

Update VIX-3481 through the project’s JIRA workflow. Replace any inaccurate ordinary-right-click reproduction with the corrected sequence: select at least two effects, hold Ctrl, right-click empty timeline space, release Ctrl after the menu opens, then inspect Alignment. Record that the historical log does not prove modifier-key state, but this current path produces the same null dereference and stack-frame path. Include the command rules and acceptance criteria in this plan. Do not transition the issue unless the project workflow specifically requires it.

Acceptance is that VIX-3481 accurately distinguishes the verified current defect from the unverified historical gesture and gives implementers the same boundaries as this plan.

### Milestone 2 — Make context-menu availability express each command’s actual prerequisites

In `timelineControl_ContextSelected` in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs`, retain the existing initial `OkToUseAlignmentHelper(TimelineControl.SelectedElements)` calculation as the selected-count/row-limit eligibility input. After `element` is derived, calculate local Boolean values with clear names:

- `canReferenceAlign` is true only when `element` is not null, the existing alignment count/row-limit eligibility is true, and either more than one effect is selected or exactly one effect is selected and that clicked effect is not already selected.
- `canDistribute` is true only when the existing alignment count/row-limit eligibility is true and more than one effect is selected.

Assign `canReferenceAlign` to the enabled state of these seven submenu items: Align Start Times, Align End Times, Align Both Times, Align Centerpoints, Match Duration, Align Start to End, and Align End to Start. Assign `canDistribute` to Distribute Equally and Distribute Effects. Preserve the existing mark-command enablement loop: when labeled marks exist, it may enable the parent Alignment menu and the three mark commands even if the other command groups are disabled.

Set the parent Alignment menu’s enabled state and tooltip based on whether at least one of the reference, distribution, or mark command groups has a valid action. Preserve the existing warning tooltip wording for selection states that permit neither alignment nor distribution and have no marks; refine wording only if needed to avoid claiming reference alignment is available when it is not. Do not enable a disabled child merely because the parent is enabled for marks.

Do not alter `Grid_Mouse.cs`, keyboard shortcut conditions in `TimedSequenceEditorForm_Hotkeys.cs`, action delegates, or the behavior of valid right-clicks on effects. The existing hotkey path already requires a non-null hovered effect and should remain unchanged.

Acceptance is that Ctrl-right-click on empty timeline space with multiple selected effects keeps the selection but disables exactly the seven commands that pass a clicked effect into `Grid`; both distribution commands remain available. A right-click directly on an effect continues to make all applicable reference commands available under the existing selection rules.

### Milestone 3 — Make the public alignment boundary null-safe

In `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`, add an early null guard to each of the seven public methods listed in Context and Orientation. The guard must execute before `OkToUseAlignmentHelper` so an invalid reference does not show the unrelated selection-limit warning dialog. Return without changing element timing, selection, or undo state when `referenceElement` is null.

Declare the reference parameter nullable (`Element? referenceElement`) for each changed method so nullable-reference analysis and callers see the supported no-op contract. Update each method’s XML documentation in the same edit, following `.agents/skills/csharp-docs/SKILL.md`: use accurate summaries and parameter descriptions, and explicitly state in remarks or parameter documentation that a null reference produces no alignment. Replace the currently empty or inaccurate parameter documentation on the three methods where it is touched. Do not change return types, method names, element enumeration behavior, or valid-reference alignment mathematics.

Acceptance is that every public reference-alignment method accepts null without throwing or mutating selected element timing, while every valid-reference call preserves today’s result.

### Milestone 4 — Add focused automated coverage

Add a sequencer test class such as `src/Vixen.Tests/Sequencer/GridAlignmentNullReferenceTests.cs`, tagged with `[Collection(TimelineControlTestCollection.Name)]`. Follow the existing test helper approach from `TimelineCursorSelectionTests`: create a `TimelineControl`, add a visible row, add at least two elements with distinct start times and durations, and select them. Store each element’s original start time, end time, and duration.

Exercise all seven public reference-alignment methods with the selected elements and `referenceElement: null`, including both Boolean options for methods that accept one where practical. Assert that no call throws and that each element retains its original timing. Test methods should use the project convention `MethodName_Condition_ExpectedBehavior`; a single theory or a small group of focused facts is acceptable if it clearly reports which operation failed. Do not require a modal dialog interaction; the early guard must prevent one.

If timeline-control construction proves unsuitable for one method, extract only a test-local helper that creates the same `TimelineControl`/row/element state; do not expose a production testing API or use reflection for public methods.

Acceptance is that the new tests fail before the null guards with a `NullReferenceException`, pass afterward, and cover each public reference-alignment entry point.

### Milestone 5 — Validate end to end and close out the issue

Build and run the Debug application. Use a disposable sequence or copy of test data. Verify ordinary right-click separately from Ctrl-right-click to ensure the selection model did not change. Record build and test results in this document’s Progress and Outcomes sections. Make a final VIX-3481 update if implementation changed any requirement, then add a JIRA comment containing the corrected reproduction exercised, automated test command/result, Debug build result, and manual results. Do not transition the issue unless the repository’s workflow requires it.

When a milestone that changes repository files completes, include the proposed commit message in the completion response using the project’s `commit-msg` skill; do not create a commit unless explicitly requested.

## Concrete Steps

Run all commands from the repository root, `C:\Dev\Vixen`.

1. Before editing, inspect the current focused paths and working tree:

       git status --short
       rg -n -C 5 "ElementsUnderCursor|OkToUseAlignmentHelper|AlignElement(Start|End|Duration|Centers)" src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs src/Vixen.Common/Controls/TimeLineControl/Grid.cs

2. Implement Milestones 2 through 4. Maintain tabs and LF line endings as required by `src/.editorconfig`; do not reformat unrelated code.

3. Run the focused tests while iterating:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --nologo --filter "FullyQualifiedName~GridAlignmentNullReferenceTests"

   Expected result after implementation: the test run completes with zero failures and reports the new test or tests as passed.

4. Run the complete unit suite:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --nologo

   Expected result: zero failed tests. Record the actual passed/skipped counts rather than predicting a fixed total.

5. Build the application:

       msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

   Expected result: build succeeds with no errors. Investigate any new warnings or failures attributable to this change before manual testing.

6. Launch the Debug output using the executable produced by the build and perform the scenarios in Validation and Acceptance. If the application cannot start because of local profile, audio, or hardware configuration, still complete the focused/full automated tests and document the environmental blocker with the exact error.

## Validation and Acceptance

The change is accepted only when all of the following are true:

1. Given two or more selected effects, when the user holds Ctrl and right-clicks empty timeline space, then the selection remains selected and Alignment opens without a crash.

2. Given that Ctrl-right-click state with an alignment-eligible selection, when the Alignment submenu opens, then Align Start Times, Align End Times, Align Both Times, Align Centerpoints, Match Duration, Align Start to End, and Align End to Start are disabled; Distribute Equally and Distribute Effects are enabled.

3. Given two or more selected effects, when the user ordinarily right-clicks empty timeline space without Ctrl, then the existing behavior remains: selection is cleared and selection-dependent alignment/distribution commands are unavailable (except mark alignment when marks make those commands valid).

4. Given a valid multiple-effect selection and a right-click directly on an effect, when Alignment opens, then reference-based commands remain enabled under their existing eligibility rules and produce their existing alignment behavior.

5. Given any valid selected-effects collection, when each public `Grid.AlignElement*` method that accepts a reference receives `null`, then it does not throw, show the selection-limit warning, or change any effect timing.

6. Given labeled marks exist but a clicked effect does not, when Alignment opens, then only the mark-dependent commands may be enabled by marks; reference and distribution commands still follow their own prerequisites.

7. `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --nologo` and the Debug `msbuild` command complete with zero errors introduced by this change.

## Idempotence and Recovery

The changes are limited to context-menu state, defensive input validation, documentation, and tests. They do not alter sequence serialization, profiles, or effect data. Re-running the build and tests is safe. If implementation needs to be backed out, revert only the commits for VIX-3481; no user data migration or restoration is needed. Use a copied sequence for manual verification because the pre-fix crash can terminate Vixen before unsaved work is written.

## Artifacts and Notes

Populate this section during implementation with concise evidence, for example:

    Focused null-reference tests: Passed (actual count recorded at implementation time)
    Full unit tests: Passed / Failed / Blocked (actual result recorded at implementation time)
    Debug build: Succeeded / Failed (actual result recorded at implementation time)
    Manual Ctrl-right-click regression: Passed / Failed / Blocked (actual result recorded at implementation time)

No source files were changed while creating this plan. The working tree was clean at the research checkpoint.

## Interfaces and Dependencies

No new projects, packages, configuration, serialized data, or external services are required.

The modified public interfaces are the existing `Common.Controls.TimelineControl.Grid` methods below. Their names, return type, and successful behavior remain unchanged; their `referenceElement` parameters become nullable and have a documented no-op contract for null:

    public void AlignElementStartTimes(IEnumerable<Element> elements, Element? referenceElement, bool holdDuration)
    public void AlignElementEndTimes(IEnumerable<Element> elements, Element? referenceElement, bool holdDuration)
    public void AlignElementDurations(IEnumerable<Element> elements, Element? referenceElement, bool holdEndTime)
    public void AlignElementStartEndTimes(IEnumerable<Element> elements, Element? referenceElement)
    public void AlignElementStartToEndTimes(IEnumerable<Element> elements, Element? referenceElement, bool holdEndTime)
    public void AlignElementEndToStartTime(IEnumerable<Element> elements, Element? referenceElement, bool holdStartTime)
    public void AlignElementCenters(IEnumerable<Element> elements, Element? referenceElement)

`TimedSequenceEditorForm_ContextMenu.cs` remains the only UI behavior change. `Grid_Mouse.cs` is deliberately not changed. `TimedSequenceEditorForm_Hotkeys.cs` is deliberately not changed because it already rejects a null cursor element before calling the alignment API.

---

Plan created 2026-08-03 from the VIX-3481 current-code-validation handoff. It records the corrected Ctrl-right-click reproduction and makes command availability and public API defense separate, testable responsibilities.

Revised 2026-08-03 after Milestone 1: recorded the completed JIRA description update and its unchanged `Accepted` status.

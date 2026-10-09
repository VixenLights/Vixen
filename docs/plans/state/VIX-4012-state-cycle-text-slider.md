# VIX-4012: Enter larger State Cycle counts and reveal the numeric drag strip


This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective as work proceeds. This document follows `.agents/PLANS.md` and the project `.agents/skills/analyze-and-plan-issue/SKILL.md`. Milestone 1's Jira description update is complete. Application implementation and any later Jira updates remain future execution work.

## Purpose / Big Picture


Sequence authors need more than 20 cycles when a State effect spans a long sequence. Replace the bounded Cycle slider with the existing numeric textbox that also supports dragging. The author can type 100, press Enter, save, and reopen the sequence with 100 still selected. Add a graduated shaded strip below numeric textboxes so authors can discover the drag area using a visual familiar from regular sliders. State and Spin should both show this cue. The strip is seven device-independent pixels high, with a subtle defined edge and a brighter hover state. Its shading is static because dragging adjusts the current value relatively rather than selecting a position on a bounded range.

These are two user-visible improvements, implemented in two code milestones. Keep the State count a whole number with a minimum of one. Its upper bound is the existing integer representation, 2,147,483,647, rather than a new small product limit. Representability does not promise that billions of cycles can render interactively: rendering still allocates intervals in proportion to requested cycles and source contents.

## Progress


- [x] (2026-10-09) Read VIX-4012, its comments and attachments, the planning skill, `.agents/PLANS.md`, and applicable project C# guidance. Jira has no comments or attachments.
- [x] (2026-10-09) Inspect current State data, rendering, Spin metadata, numeric editor selection, and integer/double templates; inspect the related VIX-3951 specification.
- [x] (2026-10-09) Prepare this design and execution plan. Initial `git status --short` was empty; no application implementation had occurred at design time.
- [x] (2026-10-09) Milestone 1: Published and reread the user-facing requirements on VIX-4012; title and Accepted status preserved.
- [ ] Milestone 2: Enable larger whole-number Cycle counts with persistence and rendering regressions.
- [x] (2026-10-09 15:02 UTC) Revise the approved visual design from arrows to a static graduated shaded strip, height seven, with a defined edge and brighter hover feedback; implementation remains pending.
- [ ] Milestone 3: Add the shared shaded drag strip and verify State, Spin, and another numeric consumer.
- [ ] Milestone 4: Record actual validation and align the Jira description with the delivered behavior when publishing is authorized.

## Surprises & Discoveries


Observation: Changing the editor attribute alone will silently preserve the old ceiling. Evidence: `StateData.MaxIterations` is 20, and `NormalizeIterations` uses `Math.Clamp(iterations, MinIterations, MaxIterations)`. The data property's getter and setter, effect setter, and four Iterate render helpers call this normalization.

Observation: Spin and State should use counterpart controls rather than the same numeric type. Evidence: `Spin.RevolutionCount` is a double with no explicit editor attribute; `EditorCollection.Cache` maps double to `DoubleEditorKey` and integer to `IntegerEditorKey`. State's existing persisted `Iterations` is an integer. Both controls already provide textbox entry and dragging.

Observation: There is already space for the cue. Evidence: both `Themes/IntegerEditor.xaml` and `Themes/DoubleEditor.xaml` place a transparent rectangle named `PART_dragger`, height five, directly beneath `textboxEditor`. Their hover triggers use that name and their code handles mouse capture, drag tolerance, and editing events. The revised design increases this existing strip to height seven, adding only two device-independent pixels to the editor row.

Observation: Numeric dragging and conventional slider positioning have different meanings. Evidence: the numeric controls adjust their current value from mouse movement, while `Themes/SliderEditor.xaml` sets `IsMoveToPointEnabled="True"` on a bounded Slider. Use static graduated shading to suggest a drag surface; a moving thumb, numbered ticks, or value-dependent fill would imply a position-to-value mapping that these numeric controls do not provide.

Observation: Larger counts expose arithmetic that was safe under the ceiling of 20. Evidence: State Item, Custom individual, Custom grouped, and Mark Iterate helpers multiply an integer base-slot count by normalized iterations; their total counts and indexes currently remain integers.

Observation: Earlier offset documentation requires the existing slot arithmetic to remain unchanged for that earlier feature. VIX-4012 deliberately supersedes only the old count width and limit. The VIX-3951 specification at `docs/plans/effects/VIX-3951-state-offset-spec-requirements.md` preserves atomic groups, blank slots, offset-before-repetition, and the final-slot remainder. Its older direct `dotnet test` instructions also need the full-MSBuild preparation now specified by AGENTS.md; use the workflow below.

Observation: The graph's zero-dependent result for the XAML template is not proof of isolation. It reports extraction gaps for named template elements. Treat the templates as shared UI and verify multiple consumers manually.

## Decision Log


Decision: Retain `int Iterations`, its data-member identity, default of one, property ordering, localized names, and Iterate-only visibility. Reuse the default IntegerEditor by removing the explicit SliderEditor attribute. Rationale: this provides Spin's established interaction without changing State's whole-cycle semantics or serialized shape. Date/Author: 2026-10-09 / Codex.

Decision: Set `StateData.MaxIterations` to `int.MaxValue`, retaining `MinIterations = 1` and the existing normalization path. Rationale: remove the arbitrary ceiling while preserving a single consistent data/editor contract and defensive handling of zero and negative values. Date/Author: 2026-10-09 / Codex.

Decision: Widen total-slot arithmetic and output indexes to long in the four Iterate paths. Rationale: an integer count above 20 must not overflow when multiplied by the number of slots. Keep source collection indexes as integers after modulo. Date/Author: 2026-10-09 / Codex.

Decision (initial visual choice; superseded by the 15:02 UTC decision below): Add the same decoration in both numeric templates, inside the existing five-pixel strip. Rationale: State uses IntegerEditor, while the issue explicitly cites Spin, which uses DoubleEditor. This also consistently benefits other existing consumers. No new control, service, view model, or dependency is required. Date/Author: 2026-10-09 / Codex.

Decision: Replace the arrow-ended line with a static graduated shaded strip in both numeric templates; increase height from five to seven, define its edge, and brighten it on hover and during dragging. Rationale: the user approved a visual closer to familiar regular sliders and permitted one to two extra pixels. Preserve relative adjustment behavior, existing cursor, and textbox editing. The decoration must not indicate a bounded range: add no moving thumb, numbered ticks, or value-proportional fill. Date/Author: 2026-10-09 15:02 UTC / user direction recorded by Codex.

Decision: The skill's explicit milestone review stops govern execution. Never create a commit without the user's explicit request. The user later authorized Milestone 1's Jira description update; subsequent Jira work remains subject to its milestone scope. Rationale: honor the plan's manual-review boundaries and repository commit policy while carrying out explicitly authorized work. Date/Author: 2026-10-09 / Codex.

## Outcomes & Retrospective


Analysis and design are complete, including the user-approved shaded-strip revision. Milestone 1 is complete: VIX-4012 now has concise user-facing Summary, Scope, and Acceptance Criteria covering entry of counts above 20, retention after save/reopen, lower-bound normalization, and the seven-pixel graduated drag cue. A reread confirmed the original title and Accepted status were preserved. Application implementation, build/test/lint checks, and runtime verification remain pending; no application source has changed.

## Architecture Design: VIX-4012


Detected IDE environment: JetBrains Rider with available analysis automation. Apply changes as small, file-scoped blocks using Rider's Apply snippet from chat workflow when implementing manually. A coding agent must instead use the guarded Gortex edit/refactor workflow required by AGENTS.md. Use Rider's test runner for focused inspection, with the command-line build/test workflow below as reproducible validation.

Core strategy: reuse the existing type-selected numeric editors and decorate their templates with the same static seven-pixel graduated shaded strip, a defined edge, and brighter hover/drag feedback. The project `dotnet-best-practices` and `csharp-docs` skills guide scoped C# changes and documentation. Existing custom WPF controls and template bindings suffice; the change does not introduce Catel view models, commands, bindings, object lifecycle changes, or new architectural boundaries.

Data model and property contracts: `State.Iterations` and `[DataMember] StateData.Iterations` remain public integers. Accepted values are 1 through `int.MaxValue`, default one. The existing normalized setter still marks the effect dirty and raises its property notification only for a changed normalized value. Cloning and deserialization retain larger values through the same data member. Preserve Cycle Offset, Cycle Individually, source selections, effect-default behavior, and display resources.

Mathematical and boundary logic: a timing slot is one chronological share of the relevant duration. It may contain several simultaneously active State items or contain blank time. If S is the number of base slots and R is normalized repetitions, the total is `(long)S * R`. An integer slot count and integer repetition count multiply safely in long. Iterate with a long output index i. Preserve selection as `(int)(((i % S) + normalizedOffset) % S)` after the existing nonempty-source checks. Computing the addition in long also prevents intermediate overflow. Durations remain `TimeSpan.FromTicks(duration.Ticks / totalSlots)` except the final slot, which receives the remaining duration. This keeps rounding and final boundaries identical for existing counts. Do not introduce fractional cycles, a multiplier, a duration-based clamp, or new resampling behavior.

## Context and Orientation


`src/Vixen.Modules/Effect/State/State.cs` contains the public `Iterations` effect property, its editor attributes, dirty/notification behavior, conditional visibility, and the calls that pass its count to the render planner. The display name uses resource key `StateIterations`; the issue calls this the Cycle count. Do not confuse it with `CycleOffset`, which selects a starting slot.

`src/Vixen.Modules/Effect/State/StateData.cs` stores the count, normalizes it, and copies it during cloning. `src/Vixen.Modules/Effect/State/StateRenderPlanner.cs` builds timed intervals. Its four relevant helpers are `CreateIteratedIntervals`, `CreateIteratedCustomIntervals`, `CreateGroupedCustomIntervals`, and `AddIteratedMarkIntervals`. State Item cycles unique exact names; Custom cycles rows or consecutive groups; Mark Collection repeats each mark's parsed segments independently. Unknown names and blank custom rows still occupy timing slots.

`src/Vixen.Modules/Effect/Spin/Spin.cs` is the interaction precedent and needs no edit. `src/Vixen.Modules/Editor/EffectEditor/Editors/EditorCollection.cs` selects default integer/double editors. `src/Vixen.Modules/Editor/EffectEditor/Themes/EditorResources.xaml` binds those controls to the property value and NumberRange metadata. These two files also need no edit.

`src/Vixen.Modules/Editor/EffectEditor/Themes/IntegerEditor.xaml` and `src/Vixen.Modules/Editor/EffectEditor/Themes/DoubleEditor.xaml` define the textbox and drag strip. Their respective code files under `Controls/` already handle dragging and limits and need no functional change. An editor template is the XAML describing the control's visible parts; preserving their names keeps existing code and hover triggers connected.

`src/Vixen.Tests/Effect/State/StateDataTests.cs` has `Iterations_DefaultsToOne`, `Iterations_NormalizesRange`, and `Clone_CopiesIterations`. `src/Vixen.Tests/Effect/State/StateRenderPlannerTests.cs` already tests repetitions for the four sources, Cycle Offset, and grouped behavior. Extend those existing xUnit fixtures rather than adding another test project or UI snapshot framework.

## Plan of Work


### Milestone 1: Agree and publish the user-facing requirements


Context: VIX-4012 is at https://vixenlights.atlassian.net/browse/VIX-4012. The user authorized this milestone during implementation.

Plan of Work: Complete. The description now has concise Summary, Scope, and Acceptance Criteria sections. It explains entry of whole-number Cycle counts above 20 by typing or dragging, retention after save/reopen, a minimum of one, and the graduated shaded strip shared with Spin. Acceptance criteria cover entering 100 cycles on a long effect, save/reopen, lower-bound handling, a seven-pixel strip with defined edge and brighter hover, and existing relative drag behavior. The description states the strip is static and does not represent a bounded range, replacing the originally proposed arrow-ended line. It contains no implementation internals.

Concrete Steps: Read the existing issue description, updated only its description, then reread it. The issue retained its original title, “Convert the Cycle slider in the State effect to the text entry slider that Spin uses,” and its Accepted status.

Validation and Acceptance: Reread confirmed the published description includes both user-visible outcomes and save/reopen acceptance without implementation internals. Milestone 1 is complete.

STOP HERE for manual review and commit execution before proceeding. Halt execution; run `git status --short` and review `git diff` for files changed in the milestone. If repository files changed, invoke the project commit-msg skill with VIX-4012 as the subject prefix and output its paste-ready Commit message block. Never create a commit without an explicit request. Wait for explicit user confirmation before advancing.

### Milestone 2: Enable larger State Cycle counts end to end


Context: edit State.cs, StateData.cs, StateRenderPlanner.cs, and the two existing State test files listed above. The outcome is that a typed count of 100 remains 100 through editing, cloning, persistence, and all applicable Cycle rendering paths.

Plan of Work: in `StateData`, change only `MaxIterations = 20` to `MaxIterations = int.MaxValue`. Keep normalization and the existing serialized property name/type. Update the public property's XML value documentation to state the positive whole-number range and default. In `State.Iterations`, remove `[PropertyEditor("SliderEditor")]` and retain `[NumberRange(StateData.MinIterations, StateData.MaxIterations, 1)]`. The default type selection then supplies IntegerEditor. Update its XML value documentation to match; preserve the setter, metadata ordering, and visibility.

In the planner, centralize the widened count calculation in a small internal helper with the final signature `internal static long GetTotalSlotCount(int slotCount, int iterations)`, returning `(long)slotCount * StateData.NormalizeIterations(iterations)`. This gives the four paths one calculation and permits a fast overflow regression without rendering billions of slots. Use it for their intervalCount or segmentCount, and use `long index = 0` for their loops. Change `GetIntervalDuration`'s intervalCount and intervalIndex parameters from int to long; keep its TimeSpan parameters and formula. Change `GetOffsetSlotIndex`'s outputIndex from int to long and explicitly cast the final modulo result to int. Do not change planner entry-point signatures, group creation, parsing, offset normalization, or interval construction.

Extend `Iterations_NormalizesRange` with negative/zero => one and 21, 100, and int.MaxValue => unchanged. Update any test expecting values above 20 to clamp to 20. Extend clone coverage with 100 and int.MaxValue. Add a serialization roundtrip and public effect setter check with 100, including dirty notification and same-value behavior, using the existing test conventions. Test the total-slot helper with two slots and int.MaxValue, expecting 4,294,967,294, and with zero slots, expecting zero; these tests must not allocate interval collections. Add manageable planner cases with two slots and 100 repetitions for State Item, Custom individual, Custom grouped, and a single Mark Collection mark. Verify 200 timing slots, correct repeating selections, and the exact final boundary. A grouped slot may emit more than one interval, so assert timing-slot count separately from interval-row count. Keep offset and blank-slot regressions passing.

Concrete Steps: follow the Gortex workflow described below before each edit. Apply small snippets per file in Rider. Build the tests with full MSBuild and run the focused State suite using the commands below.

Validation and Acceptance: the new data tests fail before the cap is removed and pass after. The manageable planner tests show the actual greater-than-20 behavior; the arithmetic test proves overflow protection without an enormous render. In the editor, choose State Cycle playback, type 100 and press Enter, save/reopen, and confirm 100 remains. Verify one, 20, 21, and 100 work; zero/negative entries normalize to one; fractional text is rejected by the existing integer editor. Check dragging and ordinary undo/redo through the existing edit route. Switch to Default playback and confirm Cycle settings retain their existing visibility behavior.

STOP HERE for manual review and commit execution before proceeding. Halt execution; run `git status --short` and `git diff` for the milestone's files. Invoke the project commit-msg skill with VIX-4012 as subject prefix and output its paste-ready Commit message block. Do not commit without an explicit request. Update this plan with validation evidence and wait for explicit user confirmation before advancing.

### Milestone 3: Show the graduated shaded strip beneath both numeric textboxes


Context: edit only `src/Vixen.Modules/Editor/EffectEditor/Themes/IntegerEditor.xaml` and `src/Vixen.Modules/Editor/EffectEditor/Themes/DoubleEditor.xaml`. The cue appears below State's integer textbox and Spin's decimal textbox, as well as other users of these shared templates.

Plan of Work: replace each transparent rectangle with a Grid named `PART_dragger`, `Background="Transparent"`, and `Height="7"`. This preserves a full-width hit target and increases editor height by exactly two device-independent pixels. Inside it, add a stretchable Border with a subtle one-pixel edge and a Rectangle providing graduated shading. Keep all decoration `IsHitTestVisible="False"` and nonfocusable; the parent remains hit-testable even over decorative edges and padding. Use a static LinearGradientBrush with smooth light-to-dark shading across the width, and a subtle edge or bevel to read as a draggable surface. During implementation, inspect the actual regular Slider theme resources and reuse its compatible existing theme colors/brushes where available; otherwise use existing theme foreground/background resources for the gradient endpoints and border. Do not invent resource keys or hard-code a light-theme palette. Verify the result in the running application before accepting the visual.

Extend the existing `PART_dragger.IsMouseOver` trigger to brighten the decorative surface while retaining its current cursor setter. Use the same brighter state during `IsDragging`, so feedback persists while the pointer is captured outside the strip. Preserve the disabled presentation through existing theme resources or a template trigger and keep it visually noninteractive. The shading must not change with numeric Value: there is no moving thumb, value-proportional fill, numbered scale, or arrow decoration. Preserve the textbox bindings, extender commit/rollback settings, StackPanel, template part names, and existing IsDragging/cursor behavior.

The textbox and drag-strip contract at the end is:

    textboxEditor: existing TextBox, existing Text or Value binding
    PART_dragger: transparent, hit-testable Grid, Height=7
    decorative graduated surface and defined edge: no hit testing or focus
    normal appearance: static, theme-aware gradient
    hover/drag appearance: brighter static surface, existing cursor
    Value: no effect on the decorative shading or geometry

This is a template-only change. Do not change dragging code, timer behavior, event handlers, or editor registrations. The small visual block and brushes can be repeated across the two dictionaries while reusing available theme resources; do not introduce a new shared resource system or public API for this cue.

Concrete Steps: apply the visual block in both templates through guarded edits or Rider snippets. Run Rider `get_file_problems` on both changed files and resolve only issues introduced on changed lines. Build the solution configuration below because this is shared editor UI. Run the focused State regression suite once against the combined implementation.

Validation and Acceptance: launch the built application, open a State Cycle effect and Spin Revolution Count, and confirm a seven-pixel graduated shaded strip with a defined edge appears under each textbox. Hover over the strip and verify it brightens and the existing drag cursor appears; move away and verify it returns to normal. Drag on the shaded area, border, and padding; the value must adjust through the existing interaction. While dragging outside the strip, the brighter feedback must remain until the drag ends. Change the numeric value by typing and dragging and confirm the shading/geometry stay static rather than depicting a range position. Clicking and typing in the textbox, Enter commit, Escape rollback, modifier-assisted dragging, and undo/redo must behave as before. Check an additional integer numeric property and a decimal property. Check available light/dark themes, disabled/read-only presentation, a narrow property panel, and Windows scaling at 100% and 150%. Disabled controls must not suggest an active drag operation. The only intended row-height increase is two device-independent pixels; there must be no clipping, additional height growth, theme-invisible shading, or new focus target. Document actual observations; visual acceptance is manual, with no new snapshot tests.

STOP HERE for manual review and commit execution before proceeding. Halt execution; run `git status --short` and the scoped `git diff`. Invoke the project commit-msg skill with VIX-4012 as subject prefix and output its paste-ready Commit message block. Do not commit without an explicit request. Update validation evidence in this plan and wait for explicit user confirmation before advancing.

### Milestone 4: Record results and close the documentation loop


Context: update this plan and, when publishing is authorized, the existing VIX-4012 description and one closing comment. This milestone reports delivered behavior and actual test results.

Plan of Work: reconcile the issue description if implementation changed any user-facing requirement. Add a concise Jira comment stating whether counts above 20 persist and render, whether State and Spin show the drag cue, and the actual build/test/manual results. Include any unverified items plainly. Do not transition the issue or create a commit unless asked. Mark Progress complete only for work that has actually passed acceptance.

Concrete Steps: capture the commands and results below in Artifacts and Notes, update Outcomes & Retrospective, and read back any published Jira changes.

Validation and Acceptance: the plan and Jira accurately describe the delivered behavior; no expected output has been substituted for observed evidence.

STOP HERE for manual review and commit execution before proceeding. Halt execution; run `git status --short` and the scoped `git diff`. If repository files changed, invoke the project commit-msg skill with VIX-4012 as subject prefix and output its paste-ready Commit message block. Never commit without an explicit request. Wait for explicit user confirmation before any further work.

## Concrete Steps


Run all commands from `C:\Dev\Vixen` in PowerShell. Verify the worktree before implementing and preserve unrelated changes. MSBuild must be the full Visual Studio installation with the C++ toolset; the tests depend transitively on C++/CLI projects.

    git status --short
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~Effect.State"

Expected outcomes are a successful build and all selected State tests passing; record the actual selected/passed count during execution. Do not claim a fixed count before tests have been added and discovered.

For the shared UI changes, build the affected solution configuration:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64

Expected outcome is Build succeeded with no new errors. Existing unrelated warnings are recorded rather than cleaned up. Launch the newly built Vixen application from `Release/Output/` using Rider or the existing application entry point; verify the loaded build is the one just produced. Use a test sequence/profile for the manual scenarios, then record results in this plan.

If the filter selects no tests, correct it using the actual fixture namespaces before claiming a pass. If full MSBuild is not on PATH, resolve the installed Visual Studio MSBuild executable and run the same arguments; do not substitute `dotnet test` with a build of the C++/CLI dependencies.

During coding, use native Gortex MCP `explore(operation:"task")` for each implementation scope, the bounded follow-up permitted by AGENTS.md, and `change(operation:"impact")` before mutation. Verify proposed helper signature changes with `change(operation:"verify")`. Mutate only with Gortex edit/refactor. After mutation call `change(operation:"detect")` and use its returned symbol IDs with tests, guards, and contract operations. Run Rider `get_file_problems` for all generated/changed C# and XAML files; resolve only findings on changed lines. Do not reopen indexed source with shell reads/searches. Use tabs and preserve unrelated formatting.

## Validation and Acceptance


The count improvement is accepted when an author can enter 100 cycles, retain it after save/reopen and copy, and observe the expected repeated sequence for State Item, Mark Collection, Custom individual, and Custom grouped playback. A 200-second test duration with two base slots and 100 cycles gives 200 one-second slots; applying offset one starts at the second slot and repeats the same rotated pair. Marks repeat within their own clipped duration. A specifically selected State Item keeps its existing full-duration behavior. Existing counts 1 through 20 produce the same order, colors, grouping, blank slots, and timing boundaries as before.

The cue improvement is accepted when State and Spin both show a static graduated shaded strip below the numeric textbox, seven device-independent pixels high, with a defined edge and brighter hover/drag feedback. Dragging anywhere in the strip must work with the established relative adjustment and cursor behavior. Shading must stay independent of the numeric value, without a thumb, numbered scale, or value-dependent fill. Rendering must remain legible in available themes and scaling configurations, with only the intended two-pixel row-height increase. The application build, focused tests, Rider file analysis, and manual checks are complementary evidence; passing compilation alone is insufficient for this visual feature.

Boundary tests inspect storage, normalization, cloning, and count arithmetic at int.MaxValue. Do not render int.MaxValue cycles in an automated or manual test. This plan removes the small user-interface ceiling and prevents arithmetic overflow; it does not redesign interval allocation or guarantee responsiveness for extreme counts.

## Idempotence and Recovery


The change preserves serialized names and types and adds no migration or dependency. Reapply only missing snippets; do not duplicate template names, shaded surfaces, or visual-state triggers. Repeat builds and focused tests safely. If a build or test fails, inspect the failure and fix only task-related code. Existing sequences with values 1 through 20 remain compatible. Opening a newly saved count above 20 in an older Vixen build can still apply that older build's cap; backward application behavior cannot be changed by this implementation.

Before reverting work, inspect the current diff and remove only the edits made for this task; preserve user changes. Do not reset the worktree or restart/re-track the Gortex daemon. Record a failed or skipped check with its reason and resume the incomplete milestone after the cause is resolved.

## Artifacts and Notes


Design and milestone evidence observed on 2026-10-09:

    Jira VIX-4012: Accepted; no comments, attachments, or linked issues at design time. Milestone 1 description update completed and reread; original title and Accepted status retained. Published description: “State effects currently limit Cycle counts to 20, which can be too few for long sequences. Authors need to enter larger whole-number counts directly and keep them when a sequence is saved and reopened. State should use the familiar numeric text-and-drag control used by Spin. A graduated shaded strip beneath the number should make its drag area easier to discover.” Scope and acceptance criteria specify counts above 20 by typing or dragging, retention after save/reopen, minimum one, a static seven-pixel strip with defined edge and brighter hover, 100 cycles on a long effect, and existing relative drag behavior.
    Initial git status --short at design time: empty.
    StateData.MaxIterations: 20.
    State.Iterations: int; SliderEditor; NumberRange(1,20,1).
    Spin.RevolutionCount: double; default numeric editor.
    IntegerEditor.xaml and DoubleEditor.xaml: transparent PART_dragger, Height=5.

Application builds, unit tests, and runtime/manual checks were not run during design because no application code changed. Record actual execution commands and their outputs here later. Gortex impact on the new plan path reports file_not_indexed before creation, as expected for a new document. The write receipt verified the new document on disk, and subsequent git status listed only this new plan. `git diff --check` exited zero, but that command does not inspect the untracked plan. Rider `get_file_problems` could not analyze this Markdown file because it is not included in a solution project. Gortex detect likewise excludes untracked files and returned no changed source symbols; the proposed State symbol checks reported no configured guard rules and identified the two State test files already included in this plan.

## Interfaces and Dependencies


Keep public `int State.Iterations` and `[DataMember] int StateData.Iterations`, together with existing NumberRange metadata, XML documentation, and notification routes. Set internal `MaxIterations` to `int.MaxValue`; keep `NormalizeIterations(int)`'s signature.

The planner's affected internal/private signatures are:

    internal static long GetTotalSlotCount(int slotCount, int iterations)
    private static TimeSpan GetIntervalDuration(TimeSpan effectDuration, long intervalCount, long intervalIndex, TimeSpan intervalStart)
    private static int GetOffsetSlotIndex(long outputIndex, int slotCount, int normalizedCycleOffset)

Use existing WPF Grid, Border, Rectangle, LinearGradientBrush, and template triggers with compatible existing theme resources. The final `PART_dragger` height is seven device-independent pixels, shading is static, and hover/drag feedback is brighter. Preserve `textboxEditor` and `PART_dragger` names in both existing control templates. No new package, project, public control property, Catel service, or serializer schema is required.

## Revision Notes


2026-10-09: Initial design derived from VIX-4012 and current source. Added data normalization and overflow work because changing only the editor would leave the cap in place or expose integer multiplication overflow. Kept the visual cue within the existing strip in both numeric templates to cover State and Spin consistently. Analysis complete and plan integrated with plans.md.

2026-10-09 15:02 UTC: Incorporated the user's approved replacement of the arrow-ended line with a familiar graduated shaded surface. Updated purpose, progress, architectural strategy, future Jira criteria, Milestone 3, acceptance, recovery, and dependency guidance together. The strip increases from five to seven device-independent pixels, uses a defined edge and brighter hover/drag state, and remains independent of numeric Value to preserve the meaning of relative dragging. The initial visual decision is retained as superseded history; completed research and the State count design are unchanged. No application changes were made. Plan revision complete and recorded in the Decision Log.

2026-10-09: Published and reread Milestone 1's user-facing VIX-4012 description with Summary, Scope, and Acceptance Criteria. Verified the issue title and Accepted status were preserved. Updated Progress, Milestone 1, Decision Log, Outcomes, and Artifacts accordingly. Application implementation remains pending.

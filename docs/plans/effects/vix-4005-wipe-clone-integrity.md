# VIX-4005 Preserve Wipe settings and independent data when copying

This ExecPlan is a living document maintained in accordance with `.agents/PLANS.md`. Keep Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective current. Milestones 1–3 are complete; implementation has not yet had full-suite/manual verification. Do not create commits without an explicit user request.

## Purpose / Big Picture


Copying, pasting, or cloning a Wipe should retain its settings, including Each Element/Group and the selected depth when the destination supports them. Editing the copy's intensity curve, movement curve, or color gradient should not change the original. Verify this with regression tests and a sequence containing a parent group with several child props and preview locations.

## Progress


- [x] (2026-10-02) Read VIX-4005, repository plan conventions, Wipe specification, clone contracts, and editor assignment order; inspected a clean working tree.
- [x] (2026-10-02) Designed preservation of all data fields, independent mutable values, and deferred target validation.
- [x] (2026-10-02) Milestone 1: Recorded the original Jira description here and aligned the Jira description with user outcomes and acceptance criteria.
- [x] (2026-10-02) Milestone 2: Added clone-integrity, data-before-target, and empty-target reassignment regressions; pre-production focused run: 17 passed, 7 failed as expected.
- [x] (2026-10-02) Milestone 3: Deep-cloned Wipe curves/gradient and deferred targeting normalization for empty targets; focused suite: 24 passed.
- [ ] Milestone 4: Complete automated and manual validation and reconcile Wipe documentation.
- [ ] Milestone 5: Align the final Jira description and report actual validation results.

## Surprises & Discoveries


VIX-4005, fetched on 2026-10-02, is titled “Wipe clone does not respect the Across/Each Element/Group setting.” It reports that copy/paste and clone always return to Across Elements/Groups. There are no comments or attachments.

The current `WipeData.CreateInstanceForClone()` already returns `(WipeData)MemberwiseClone()`. MemberwiseClone copies all instance fields, including enum values and inherited fields, but keeps references to the same mutable objects. Consequently TargetNodeSelection is not omitted by this method, while Curve, MovementCurve, and ColorGradient are shared. The base `ModuleDataModelBase.Clone()` documentation calls for deep cloning.

The reset follows data assignment. `TimedSequenceEditorForm.CloneElements()` at line 3574 assigns cloned ModuleData to a fresh effect before CreateEffectNode attaches targets. Paste does the same at line 5343 with clipboard data. New effects have an empty TargetNodes array. The Wipe ModuleData setter calls UpdateAttributes, which calls UpdateTargetingAttributes. Its current condition resets Individual whenever targetNodeHandlingVisible is false, including when there are no targets; the next condition resets depth because the mode is now Group. This is direct source evidence; runtime reproduction has not been run during planning.

The specification `docs/effects/wipe-target-node-selection.md` describes the original feature and includes historical “current behavior” statements that predate the implementation. Follow the source and existing tests for implemented behavior, while preserving the specification's useful-target and legacy-default contracts. Its instruction to update cloning if replacing MemberwiseClone does not establish that enum copying is currently broken.

Milestone 2 test evidence: the Release test target builds successfully. The focused suite has 24 cases; 17 pass and 7 fail before production changes. Scalar and enum clone comparisons, including both target modes and inherited serialized members, pass. The 7 expected failures identify shared curve/gradient references and target normalization while targets are empty or removed: 2 ownership/isolation tests, 4 clone/clipboard assignment cases (mode or depth is reset), and 1 removal/reassignment case. The tests use complete WipeData JSON round trips for clipboard-style assignment. No runtime or production changes were made.

## Decision Log


Decision: Keep MemberwiseClone as the initial copy and replace the three mutable Wipe references with independent copies through their existing copy constructors.
Rationale: Automatic copying retains all current and future scalar fields, TargetNodeSelection, DepthOfEffect, and inherited metadata. An explicit initializer risks omissions when new fields are added. Reflection is used only in tests to detect newly introduced reference members that require an explicit cloning rule.
Date/Author: 2026-10-02 / Codex.

Decision: Defer all targeting-value normalization while TargetNodes is empty, while hiding both targeting properties during that state.
Rationale: Empty targets are a normal construction state in both editor copy paths. Values can only be validated against a populated destination. Existing shallow-target, group-mode, multiple-target, and useful-depth rules still apply when targets arrive.
Date/Author: 2026-10-02 / Codex.

Decision: Preserve null reference members as null during raw data cloning; do not substitute constructor defaults or run deserialization callbacks.
Rationale: Raw cloning copies state faithfully, including incomplete legacy data. Rendering or data-load repair is a separate concern. Tests of incomplete data should call WipeData.Clone directly rather than assign unusable color data to a live module.
Date/Author: 2026-10-02 / Codex.

Decision: Preserve ModuleDataSet as a shared owning-service reference, and preserve ModuleTypeId and ModuleInstanceId in raw data cloning.
Rationale: MemberwiseClone already does this. ModuleDataSet is an owner, not Wipe-editable data. Retain existing instance ownership conventions; do not deep-clone the service or change module identity allocation.
Date/Author: 2026-10-02 / Codex.

Decision: Limit production changes to WipeData and WipeModule, retain existing public signatures and serialization members, and use the repository's xUnit v3 tests.
Rationale: The Prototype pattern (copying an existing instance) and Template Method pattern (EffectTypeModuleData.Clone delegates effect-specific copying) already fit the problem. The project dotnet-best-practices, dotnet-design-pattern-review, and csharp-docs skills inform ownership, testing, and documentation. Additional interfaces, factories, or changes to other effects are unnecessary.
Date/Author: 2026-10-02 / Codex.

## Outcomes & Retrospective


Milestone 1 Jira baseline (fetched 2026-10-02, before update):

> When copy and pasting or cloning a Wipe effect, the setting for the behavior of Across Element/Group, or Each Element Group is not maintained. It always lands on the default Across Elements/Group.
>
> The Target Node Handling setting should maintain whatever setting is on the originating effect.

Analysis and design are complete. Milestone 1 updated the Jira description. Milestone 2 added the regression tests and recorded expected pre-production failures. This plan addresses both the source-obvious target reset and mutable data sharing. Implementation must demonstrate that the complete copy workflow works; a passing raw enum-copy test alone cannot close VIX-4005.

Milestone 2 outcome: `WipeDataCloneTests` inventories and checks all 21 serialized members across WipeData and its base classes, verifies owner identity, mutable-member ownership, null copying, library metadata, and bidirectional point edits. `WipeTargetNodeSelectionTests` covers raw clone and complete JSON clipboard-style assignment before targets, plus empty-target removal and reassignment. Rider reported no file problems in either changed C# file. The full-MSBuild test target exited successfully. The focused test run exited 1 as expected: 17 passed, 7 failed (shared references and target/depth resets).

Milestone 3 outcome: `WipeData.CreateInstanceForClone` now preserves the memberwise state while copying Curve, MovementCurve, and ColorGradient via their copy constructors, retaining nulls and the shared ModuleDataSet owner. `WipeModule.UpdateTargetingAttributes` hides both controls and returns without normalizing mode or depth while targets are empty; populated-target rules are unchanged. Added XML documentation to the clone hook, ModuleData override, and TargetNodesChanged. Updated the former empty-target stale-depth test to assert retention and notification, consistent with the new contract. Rider reported no file problems in all three changed C# files. `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` succeeded (pre-existing warnings in unrelated files); the focused command from Concrete Steps passed all 24 tests. `git diff --check` passed. Pause here for manual review before milestone 4.

## Architecture Design: VIX-4005


Detected IDE Environment: JetBrains Rider with callable get_file_problems automation. Use Rider's “Apply snippet from chat” for focused method replacements during implementation and Rider Test Runner for the named test classes. Terminal verification remains required.

Core Strategy: Retain the existing effect-specific clone hook, copy all fields automatically, and detach mutable Wipe values through existing deep copy constructors. Delay target-dependent normalization until a destination exists. The design-pattern review is recorded in `docs/reviews/vix-4005-wipe-clone-design.md`.

Data Model & Property Contracts: No new persisted fields, interfaces, enum values, or signatures. WipeData.Clone returns a different WipeData instance with equivalent settings. Curve, MovementCurve, and ColorGradient, including their editable point collections, are independent. Library reference names and current-library flags are preserved. ModuleDataSet retains its ownership reference. Both target modes are copied exactly before destination validation.

Mathematical / Boundary Logic: No rendering formula changes. Empty targets hide targeting controls and preserve mode and depth. With targets present, keep current normalization: unsupported shallow single targets force Group; Group resets depth to zero; multiple targets retain Individual and reset depth to zero; supported deep single targets retain a valid intermediate depth or select the first useful depth. A useful depth is greater than zero and less than DetermineDepth() minus one. Do not force a source depth onto an incompatible destination.

Subsystem Component Matrix: `src/Vixen.Modules/Effect/Wipe/WipeData.cs` supplies independent effect settings; `src/Vixen.Modules/Effect/Wipe/WipeModule.cs` performs destination validation after attachment; `src/Vixen.Tests/Effects/WipeDataCloneTests.cs` will cover all data fields and mutable ownership; `src/Vixen.Tests/Effects/WipeTargetNodeSelectionTests.cs` covers data-before-target assignment and supported destination behavior. `docs/effects/wipe-target-node-selection.md` records the clarified empty-target and clone contracts. Editor code and shared base classes provide evidence and require no edits.

## Context and Orientation


The repository root is `C:\Dev\Vixen`. Wipe is a plugin project at `src/Vixen.Modules/Effect/Wipe/Wipe.csproj`. `WipeData` holds its settings and inherits `EffectTypeModuleData` from `src/Vixen.Modules/Effect/Effect/EffectTypeModuleData.cs`. The latter's public Clone method calls the protected CreateInstanceForClone hook and copies TargetPositioning. `src/Vixen.Core/Module/ModuleDataModelBase.cs` adds ModuleTypeId, ModuleInstanceId, and ModuleDataSet. The owning ModuleDataSet is not editable effect content.

WipeData currently declares 18 DataMember properties: ColorHandling, ColorGradient, Direction, Curve, PulseTime, PassCount, PulsePercent, WipeOn, WipeOff, MovementCurve, WipeMovement, ReverseDirection, ColorAcrossItemPerCount, ReverseColorDirection, XOffset, YOffset, DepthOfEffect, and TargetNodeSelection. Inherited serialized members are TargetPositioning, ModuleTypeId, and ModuleInstanceId. All must survive raw cloning. Values hidden in the property grid must also survive.

`src/Vixen.Modules/App/Curves/Curve.cs` provides Curve(Curve), which copies the PointPairList and curve library metadata. `src/Vixen.Modules/App/ColorGradients/ColorGradient.cs` provides ColorGradient(ColorGradient), which copies color and alpha points and gradient metadata through CloneFrom. Use these established APIs; do not add generic serialization or reflection cloning in production.

`WipeModule.ModuleData` assigns its WipeData and refreshes attributes. UpdateTargetingAttributes currently mixes property visibility with data normalization. `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` initializes TargetNodes to an empty array and calls TargetNodesChanged when targets are assigned. `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` sets data before CreateEffectNode in both clone and paste. The fix must tolerate that order.

The existing xUnit v3 test file `src/Vixen.Tests/Effects/WipeTargetNodeSelectionTests.cs` supplies mocked target hierarchies and a SetTargetNodesWithoutPropertyValidation helper that invokes the real TargetNodesChanged callback while bypassing unrelated module-store setup. Reuse that helper for the target-assignment tests. Existing tests cover defaults, legacy loading, visibility, depth normalization, notifications, and independent rendering. The test project already references Wipe; adding a new test file requires no new project reference.

## Plan of Work


### Milestone 1: Align VIX-4005 requirements


Use the project jira skill and fetch the issue again before editing to preserve intervening user changes. This is a future execution step; planning does not publish to Jira. Update the description with concise Summary, Scope, and Acceptance Criteria sections. Explain that copies retain all Wipe settings supported by the destination, including Across/Each Element/Group and useful depth, and that edits to a copy's curves or gradient do not affect its source. State that existing defaults and destination validity rules continue to apply. Keep file names and implementation details in this plan. Include the user-visible validation scenarios below, with the detailed commands retained locally.

Acceptance is a Jira description that accurately expresses these outcomes without claiming the fix is implemented. Record the original description locally in the plan before updating so it can be recovered if necessary. STOP HERE for manual review and commit execution before proceeding. Do not create a commit unless requested.

### Milestone 2: Prove data sharing and the initialization reset


Create `src/Vixen.Tests/Effects/WipeDataCloneTests.cs` as a single sealed test class, with XML comments on added public tests and the class as required by the csharp-docs skill. Use xUnit, Arrange/Act/Assert, and tabs with LF. Parameterize both Group and Individual. Configure all 18 Wipe properties and inherited serialized fields with deliberately nondefault values, distinct multi-point curves, and a gradient containing both color and alpha points. Call the public Clone method; assert a different WipeData object, equal scalar and enum state, and equal nested content with distinct references. Assert ModuleDataSet retains the same owner reference using an IModuleDataSet mock.

Create a test-only inventory of every DataMember across the WipeData inheritance chain. Discover properties and fields, including nonpublic members, at each declaring type. Require every discovered member to have a fixture value and an equality assertion; fail with the missing member name instead of silently skipping a new member. Compare values explicitly, and use content comparisons for the three mutable references. A separate reference-member inventory must classify Curve, MovementCurve, and ColorGradient as deep copies, ModuleDataSet as a shared owner, strings as immutable if introduced, and fail for any other unclassified reference member. Cover future private fields as well as auto-property backing fields without double-counting them. Keep this mechanism in tests; avoid asserting generated compiler field names. A new mutable field should require a deliberate ownership decision, not silently share source state.

Add tests that mutate points inside each cloned curve and gradient, then prove source content is unchanged, and repeat in the opposite direction. Check library names and current-library flags. Add raw-clone cases with null Curve, MovementCurve, and ColorGradient to prove faithful state copying without default substitution. If Curve and MovementCurve reference the same source Curve, both resulting curves must be independent of the source; they need not preserve that internal alias.

Extend `src/Vixen.Tests/Effects/WipeTargetNodeSelectionTests.cs` with a test assigning `sourceData.Clone()` to a new WipeModule while its targets are empty, then attaching a supported deep hierarchy through the existing test helper. Assert Individual and a valid nondefault depth survive both stages. Add the same test using deserialized clipboard-style data rather than raw Clone to cover paste's assignment order. The data fixture must include usable curves and a gradient; use a serialize/deserialize round trip of complete WipeData rather than the incomplete legacy JSON fixture. Assert the original data stays unchanged. Add empty-target removal/reassignment coverage and verify both controls are hidden while values are retained. Parameterize source mode where practical. Existing shallow and multiple-target rules must continue passing.

Build and run the focused tests using Concrete Steps. Before production changes, expect failures for mutable reference independence and Individual/depth preservation during ModuleData assignment. Expect raw scalar copying, including TargetNodeSelection, to pass. Record actual failures in this plan; do not claim the enum was omitted by MemberwiseClone. STOP HERE for manual review and commit execution before proceeding.

### Milestone 3: Implement independent copies and destination-aware validation


In `src/Vixen.Modules/Effect/Wipe/WipeData.cs`, change only CreateInstanceForClone and its XML documentation. Keep its protected override signature. Start with a memberwise copy, then replace its three mutable members through existing copy constructors, preserving null values. The intended method body is:

    var clone = (WipeData)MemberwiseClone();
    clone.Curve = Curve is null ? null : new Curve(Curve);
    clone.MovementCurve = MovementCurve is null ? null : new Curve(MovementCurve);
    clone.ColorGradient = ColorGradient is null ? null : new ColorGradient(ColorGradient);
    return clone;

Use the existing project nullability conventions; inspect Rider diagnostics for these assignments rather than changing unrelated property types. Add a summary, returns, and remarks explaining scalar preservation, independent mutable members, retained library metadata, and shared ModuleDataSet ownership. No new public API is required.

In `src/Vixen.Modules/Effect/Wipe/WipeModule.cs`, add an initial empty-target branch to UpdateTargetingAttributes. When TargetNodes is empty, call SetBrowsable with both TargetNodeHandling and DepthOfEffect false, then return before any assignment to _data.TargetNodeSelection or _data.DepthOfEffect. Preserve the populated-target branch, notification behavior, and target-change callback. Update the XML documentation of the existing public ModuleData override to explain that it loads settings and defers target normalization until target attachment; document its value. Update TargetNodesChanged remarks to state that normalization occurs for populated targets. Do not reorder shared editor construction or add a global lifecycle API.

Run Rider get_file_problems for every changed C# file, fix only findings within changed lines, then run the focused tests. Expected result: all new clone and initialization tests and existing Wipe targeting tests pass. STOP HERE for manual review and commit execution before proceeding.

### Milestone 4: Verify real copy behavior and reconcile documentation


Run the full Vixen.Tests suite after the focused suite passes. Build the Release solution for integration coverage because the production module and test project are both in scope. Start Release/Output/Vixen.exe with a disposable test sequence in an existing test profile. Create a parent hierarchy with multiple child props carrying valid preview locations. Set Wipe to Each Element/Group and choose a valid intermediate depth other than the first choice where the hierarchy allows it. Configure nondefault direction, offsets, movement settings, intensity curve, movement curve, and gradient. Clone through the editor and copy/paste onto the same supported parent. Confirm mode, depth, other settings, and rendered local restart behavior match the source.

Edit each copy's curve and gradient and confirm the original is unchanged. Repeat with Across Elements/Groups. Paste onto multiple targets and onto a shallow single target: mode/depth must follow the existing destination validity rules, rather than retaining an unusable depth. Save and reopen the disposable sequence and verify settings survive. If using the test harness's reflection bypass, describe that limitation explicitly; manual editor checks establish the real attachment path.

Update `docs/effects/wipe-target-node-selection.md` with the implemented copy contract and an explicit distinction between temporary empty targets and a populated unsupported destination. Reconcile historical wording deliberately and keep changes scoped to VIX-4005. Record commands, pass counts, manual observations, and any skipped checks here. STOP HERE for manual review and commit execution before proceeding.

### Milestone 5: Close the Jira validation record


Fetch VIX-4005 again and preserve intervening content. Make only final requirement adjustments justified by the implemented behavior and add a concise user-facing comment with actual automated and manual validation results. Do not claim manual verification if it was not performed; report that gap. Do not transition the issue or change its assignee unless requested. Update Progress and Outcomes & Retrospective with final results and remaining limitations. STOP HERE for manual review and commit execution before proceeding.

### Stop boundary for every milestone


Halt execution at the milestone boundary. Run git status --short and scoped git diff for the files changed in that milestone, and read any untracked files because git diff omits them. For a milestone changing repository files, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4005 as the subject prefix and output a complete paste-ready Commit message with subject and wrapped body. Do not create a commit without an explicit request. Pause for explicit user confirmation before advancing to the next milestone, as required by the analyze-and-plan-issue skill.

## Concrete Steps


Run from `C:\Dev\Vixen` in PowerShell, with Visual Studio full MSBuild and the C++ toolset available. Inspect git status before each edit and preserve unrelated work. Planning has run only read-only source and Git checks; the following are implementation commands, not completed results.

    git status --short
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~WipeDataCloneTests|FullyQualifiedName~WipeTargetNodeSelectionTests"

Use full MSBuild to build tests first because transitive C++/CLI projects cannot be built by dotnet's bundled MSBuild. Start long commands with native exec_command and yield_time_ms=10000; poll with write_stdin and yield_time_ms at least 30000. Expected pre-fix transcript is a nonzero test exit with clone-isolation and empty-target preservation failures. Expected post-fix transcript is Failed: 0 with a positive pass count. Record exact observed counts when implemented.

After focused tests pass:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64

Expect successful exits and no new errors. In Rider call get_file_problems with rootFolder `C:/Dev/Vixen` and filePath for each changed C# file. Do not resolve pre-existing findings outside edited lines. If full MSBuild is absent from PATH, locate the installed Visual Studio MSBuild.exe and invoke that absolute path; do not substitute dotnet build for the C++/CLI build.

## Validation and Acceptance


Both raw Wipe data modes preserve all 21 current serialized members, including the three inherited members. Mutable curves and gradient are equivalent at copy time and independent during later edits, including nested points. Null mutable members remain null in raw copies. Owner-service identity is preserved.

A fresh WipeModule assigned copied or round-tripped Individual data before targets retains Individual and its selected depth while controls are hidden. Attaching a supported deep target retains a useful source depth. Empty-target detachment/reassignment does not erase settings. Populated shallow and multiple-target destinations still apply existing normalization. Existing defaults, legacy loading, property notifications, and render tests continue passing.

The test-only inventories fail if a newly introduced serialized member is absent from fixture/assertion coverage or a mutable reference is unclassified. Raw enum preservation already passes before the fix; shared mutable data and module initialization tests must fail before and pass after. Manual editor clone and clipboard paste show the expected mode, depth, and independent editing behavior. Record precise results rather than inventing a test count.

## Idempotence and Recovery


Changes are local and repeatable. No migration, persisted schema change, dependency addition, or solution registration is required. Use a disposable sequence for manual validation and avoid overwriting user sequences. Re-run the build before --no-build tests after code changes. If a checkpoint fails, stop there, keep diagnostic output, and correct only scoped changes. Never reset the user's working tree. Re-fetch Jira immediately before updates and retain the previous description locally for recovery.

## Artifacts and Notes


Source evidence for the data-before-target route:

    TimedSequenceEditorForm.CloneElements:
        newEffect.ModuleData = element.EffectNode.Effect.ModuleData.Clone();
        // CreateEffectNode follows and attaches row targets.

    TimedSequenceEditorForm paste path:
        newEffect.ModuleData = moduleData;
        var node = CreateEffectNode(newEffect, pasteTargetRow, targetTime, duration);

    WipeModule.UpdateTargetingAttributes:
        if (!targetNodeHandlingVisible && TargetNodeHandling == TargetNodeSelection.Individual)
            _data.TargetNodeSelection = TargetNodeSelection.Group;

The source shows how settings are overwritten after a successful raw data clone. Runtime/manual evidence remains to be collected during implementation. No build or tests were run to author this plan because no executable code was changed.

## Interfaces and Dependencies


Keep the existing signatures:

    protected override EffectTypeModuleData CreateInstanceForClone()
    public override IModuleDataModel ModuleData { get; set; }
    protected override void TargetNodesChanged()
    private void UpdateTargetingAttributes()

Use Curve(Curve) and ColorGradient(ColorGradient); preserve the existing EffectTypeModuleData.Clone template. Tests use xUnit v3, Moq, and the current Wipe project reference. Introduce no new runtime dependencies or global copy helper. Apply project csharp-docs and dotnet-best-practices guidance to changed APIs and tests. The associated design-pattern review deliberately recommends retaining the existing plugin boundaries.

Milestone 1 outcome: Jira VIX-4005 now has concise Summary, Scope, and Acceptance Criteria sections describing preserved Wipe settings, destination compatibility, and independent curve/gradient edits. The description does not claim implementation is complete.

Milestone 2 outcome: See the test evidence and focused run results in Outcomes & Retrospective. Only the two scoped test files and this plan changed. Pause here for manual review before milestone 3.

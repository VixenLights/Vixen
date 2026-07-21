# VIX-3946 Filter LipSync Mark Collections to Phoneme Tracks

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Keep this document self-contained when revising it so a future contributor can implement the work with only this file and the current working tree.

## Purpose / Big Picture

VIX-3946 improves the LipSync effect setup workflow. A Mark Collection is a named track of timeline marks. LipSync can use a Mark Collection whose marks are labeled with phoneme names such as `AI`, `E`, or `REST` to animate singing faces. Today the LipSync setup dropdown can show every Mark Collection in the sequence, including generic, phrase, word, and beat tracks that are not intended for LipSync. After this change, the LipSync Mark Collection dropdown shows only collections explicitly tagged as `Phoneme`, while preserving an existing non-phoneme selection so older sequences that happen to work do not break.

The behavior is visible in the Timed Sequence Editor by creating Mark Collections with different `CollectionType` values, opening a LipSync effect, and observing that only `Phoneme` collections appear in the dropdown. If an existing LipSync effect already points at an untagged collection, that current selection remains available and continues rendering until the user selects a correctly tagged phoneme collection.

## Progress

- [x] (2026-07-21 00:00Z) Created and approved the detailed requirements specification at `docs/effects/vix-3946-lipsync-phoneme-mark-collections.md`.
- [x] (2026-07-21 00:00Z) Created this implementation ExecPlan from the approved specification.
- [x] (2026-07-21 18:30Z) Read the project skills required for implementation: `.agents/skills/dotnet-best-practices/SKILL.md` and `.agents/skills/csharp-docs/SKILL.md`.
- [x] (2026-07-21 18:31Z) Updated Jira issue VIX-3946 description with requirements, acceptance criteria, test plan, and implementation notes before code implementation begins.
- [x] (2026-07-21 18:43Z) Inspected the LipSync property editor and selection editor behavior; warning display aliases are not feasible without broad editor changes, so the approved plain-name fallback will be used.
- [x] (2026-07-21 19:12Z) Implemented the LipSync-specific Mark Collection filtering behavior with `LipSyncMarkCollectionNameConverter` and updated `LipSync.MarkCollectionId` to use it.
- [x] (2026-07-21 19:13Z) Added focused automated tests for the filtering rules and compatibility behavior.
- [ ] Run automated validation and record the exact command results in this plan.
- [ ] Perform manual validation in the Timed Sequence Editor and record observations in this plan.
- [ ] Update Jira VIX-3946 with a closeout comment describing any specification changes and the final validation results.

## Surprises & Discoveries

- Observation: The approved specification expects the warning suffix `(Collection is not Phoneme)` only if the editor supports a display label that differs from the committed value.
  Evidence: `LipSync.MarkCollectionId` currently accepts a collection name string and resolves it through `MarkCollections.FirstOrDefault(x => x.Name.Equals(value))`, so returning a suffixed string as the actual standard value would not select the collection without additional support.

- Observation: The Effect Editor selection template does not separate display text from committed value.
  Evidence: `src/Vixen.Modules/Editor/EffectEditor/Editors/SelectionEditor.cs` uses `EditorKeys.ComboBoxEditorKey`. That template in `src/Vixen.Modules/Editor/EffectEditor/Themes/EditorResources.xaml` binds the ComboBox `ItemsSource` directly to `ParentProperty.StandardValues` and `SelectedValue` directly to `StringValue`, with no `DisplayMemberPath`, `SelectedValuePath`, or item template that could map warning display text to an underlying collection name.

- Observation: Standard values are direct converter values, and selection writes the selected string back through the property value path.
  Evidence: `src/Vixen.Modules/Editor/EffectEditor/PropertyItem.cs` returns `Converter.GetStandardValues(this)` from `StandardValues`. `src/Vixen.Modules/Editor/EffectEditor/PropertyItemValue.cs` converts `StringValue` by calling `ConvertStringToValue(value)` and then assigns `Value`. For the LipSync `MarkCollectionId` property, the property type is `string`, so `ConvertStringToValue` returns the selected string directly.

- Observation: The test project did not already reference the LipSync effect module.
  Evidence: `src/Vixen.Tests/Vixen.Tests.csproj` had effect references for Chase, State, Spiral, Spin, Text, and Wipe, but not `src/Vixen.Modules/Effect/LipSync/LipSync.csproj`. A project reference was added so focused converter tests can compile against the real LipSync module.

## Decision Log

- Decision: Scope filtering to the LipSync effect by using a LipSync-specific converter or helper rather than changing `IMarkCollectionNameConverter`.
  Rationale: The shared converter is used by other effects that intentionally allow generic or non-phoneme Mark Collections. A global filter would be a behavioral regression outside VIX-3946.
  Date/Author: 2026-07-21 / Codex

- Decision: Include only collections whose `CollectionType` is exactly `MarkCollectionType.Phoneme`, plus the currently selected non-phoneme collection when preserving legacy behavior.
  Rationale: The approved specification rejects inference from collection names, linked collection metadata, parent relationships, or mark labels. This makes the behavior predictable and aligned with the user's tagging workflow.
  Date/Author: 2026-07-21 / Codex

- Decision: Preserve an existing non-phoneme selection until the user changes it.
  Rationale: Some existing sequences use untagged collections containing valid phoneme marks. Clearing or replacing those selections would break rare but valid user data.
  Date/Author: 2026-07-21 / Codex

- Decision: Keep today's fallback behavior when no phoneme collection exists and no collection is selected.
  Rationale: The user explicitly approved keeping today's behavior. The current code falls back from `LipSyncMode.MarkCollection` to `LipSyncMode.Phoneme` in `SetupMarks()` when all available collections are generic or no usable collection exists.
  Date/Author: 2026-07-21 / Codex

- Decision: Use the approved plain-name fallback for retained non-phoneme selections instead of displaying `Collection Name (Collection is not Phoneme)`.
  Rationale: The existing Effect Editor selection template commits the same object/string that it displays. Implementing a true warning alias would require a broader selection-editor redesign, such as adding display/value binding support, or encoding warning text into the LipSync property value and teaching the getter/setter to handle pseudo-values. That would be more invasive than VIX-3946 requires. The approved fallback still preserves compatibility and removes unrelated non-phoneme collections from the dropdown.
  Date/Author: 2026-07-21 / Codex

- Decision: Put the testable filter helper on `LipSyncMarkCollectionNameConverter` instead of constructing `LipSync` or adding test-only state access.
  Rationale: `LipSync` has application-service and rendering dependencies that are unnecessary for testing dropdown filtering. A small public static helper lets tests cover the exact collection-type and selected-id rules without launching the effect editor or mutating serialized data. The converter still owns the production behavior and calls the same helper.
  Date/Author: 2026-07-21 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. The required project skills were read, the project Jira skill was read, and Jira issue VIX-3946 now contains the implementation requirements, acceptance criteria, test plan, and notes that scope the change to LipSync while preserving legacy non-phoneme selections.

Milestone 2 is complete. The Effect Editor selection editor does not support separate warning display text and committed collection values without broad changes. The implementation will use the approved fallback: retained non-phoneme selections appear by plain collection name while selected, and unrelated non-phoneme collections remain hidden. No production code has been changed yet. The remaining work starts with Milestone 3: implementing the LipSync-specific filtering converter or helper.

Milestone 3 is complete. `src/Vixen.Modules/Effect/LipSync/LipSyncMarkCollectionNameConverter.cs` now provides the LipSync-specific standard-values filter, and `src/Vixen.Modules/Effect/LipSync/LipSync.cs` uses that converter for `MarkCollectionId`. The shared `IMarkCollectionNameConverter` is unchanged. The filter includes only `MarkCollectionType.Phoneme` collections plus the currently selected collection when it exists, preserving sequence order. Focused tests in `src/Vixen.Tests/Effects/LipSyncMarkCollectionNameConverterTests.cs` cover phoneme-only filtering, retained selected `Generic`, `Word`, and `Phrase` collections, selected phoneme de-duplication, no-phoneme empty results, missing selected collections, and cleanup after selecting a phoneme collection. The remaining work starts with Milestone 4: broader automated validation and recording final command results.

## Milestones

Milestone 1 establishes the tracker and implementation constraints before code changes begin. Read the required project skills, then update Jira issue VIX-3946 so the description contains the requirements, acceptance criteria, and test plan from this ExecPlan. After this milestone, a developer, tester, or product owner can validate the intended behavior from Jira without opening this repository. Acceptance for this milestone is that VIX-3946 describes phoneme-only LipSync filtering, preserved legacy non-phoneme selections, the optional warning suffix, unchanged behavior for other effects, and the planned automated and manual validation.

Milestone 2 resolves the only approved-spec implementation unknown: whether the existing selection editor can display `Collection Name (Collection is not Phoneme)` while committing the real collection name or id. Inspect the selection editor and property-grid standard-values path, record the finding in `Surprises & Discoveries`, and add a decision to `Decision Log`. If aliases are supported without broad editor work, the implementation will use the warning suffix. If aliases are not supported, the implementation will use the approved plain-name fallback and preserve the selected non-phoneme collection without the suffix. Acceptance for this milestone is a documented decision with file evidence from the editor implementation.

Milestone 3 implements the scoped LipSync filtering behavior. Add the LipSync-specific converter or helper, update `LipSync.MarkCollectionId` to use it, keep `IMarkCollectionNameConverter` unchanged, and preserve render-time selection by `LipSyncData.MarkCollectionId`. At the end of this milestone, LipSync standard values include only `MarkCollectionType.Phoneme` collections plus the current selected non-phoneme collection, if one exists. Acceptance is code inspection plus focused tests proving the filter includes phoneme collections, excludes unrelated non-phoneme collections, and retains the current legacy selection.

Milestone 4 completes automated validation. Add or update tests for the filtering rules, deleted-selection behavior, no-phoneme list behavior, and the post-selection cleanup rule where the old non-phoneme collection disappears after a phoneme selection is current. Run `git diff --check` and focused LipSync tests, then run the full test project or the broadest feasible subset. Acceptance is recorded command output showing the new focused tests pass and any broader validation result or environment blocker is documented.

Milestone 5 completes manual validation in the Timed Sequence Editor. Use a sequence with `Generic`, `Phrase`, `Word`, and `Phoneme` Mark Collections, then verify the LipSync dropdown, retained legacy selection, selection-change cleanup, another effect's unfiltered dropdown, and missing selected collection behavior. Acceptance is a short manual validation record in this ExecPlan describing each scenario and observed result.

Milestone 6 closes out the implementation. Update this ExecPlan's living sections with final outcomes, then add a Jira VIX-3946 closeout comment containing implementation notes, any differences from the approved specification, automated validation results, manual validation results, and residual risk. Acceptance is that Jira contains both the initial description update and the final closeout comment, and this ExecPlan can explain what changed and how it was proven.

## Context and Orientation

Vixen is a Windows desktop .NET/WPF application for building animated light shows. Effects are plugin modules under `src/Vixen.Modules/Effect/`. The LipSync effect lives in `src/Vixen.Modules/Effect/LipSync/` and animates singing faces from either a static phoneme setting or a Mark Collection.

A Mark Collection is a sequence track containing timeline marks. The interface is `src/Vixen.Core/Marks/IMarkCollection.cs`. Its important members for this work are `Id`, `Name`, `CollectionType`, and `MarksInclusiveOfTime(...)`. The collection type enum is `src/Vixen.Core/Marks/IMarkType.cs` and currently includes `Generic`, `Phrase`, `Word`, and `Phoneme`.

The LipSync effect runtime class is `src/Vixen.Modules/Effect/LipSync/LipSync.cs`. Its serialized data class is `src/Vixen.Modules/Effect/LipSync/LipSyncData.cs`. `LipSyncData.MarkCollectionId` stores the selected Mark Collection as a `Guid`. The public property `LipSync.MarkCollectionId` displays the selected collection name in the effect editor but stores the selected collection id in `_data.MarkCollectionId`.

The current `LipSync.MarkCollectionId` property is decorated with `[TypeConverter(typeof(IMarkCollectionNameConverter))]`. The shared converter lives in `src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs` and returns all Mark Collection names for any effect. That shared converter is used by multiple effects, including Alternating, Dissolve, Fireworks, Shapes, Strobe, Text, and State. Do not change the shared converter for this issue.

`LipSync.SetupMarks()` is called before rendering when `LipSyncMode == LipSyncMode.MarkCollection`. It currently falls back to `LipSyncMode.Phoneme` when every available collection is generic, auto-selects the first `MarkCollectionType.Phoneme` collection when no collection is selected, and then resolves marks by `_data.MarkCollectionId`. Because rendering already uses the stored id, preserving a legacy non-phoneme selection is primarily a UI filtering concern, not a render-path redesign.

The approved specification for this work is `docs/effects/vix-3946-lipsync-phoneme-mark-collections.md`. If this ExecPlan and that specification diverge during implementation, update this ExecPlan first and record the reason in `Decision Log`. At closeout, add a Jira comment describing any specification changes and validation results.

## Plan of Work

Begin by reading the project skills for C# implementation and public/protected API documentation. Use `.agents/skills/dotnet-best-practices/SKILL.md` because this change writes C# code. Use `.agents/skills/csharp-docs/SKILL.md` if the implementation adds a public or protected converter, helper, type, method, property, or event. Project-specific skills under `.agents/skills/` take precedence over shared skills.

Before changing code, update Jira issue VIX-3946 so the tracker contains the requirements, acceptance criteria, and test plan from this ExecPlan. Use the project Jira skill at `.agents/skills/jira/SKILL.md` before performing the Jira update. The Jira description should explain that LipSync should list only `MarkCollectionType.Phoneme` collections, preserve an existing selected non-phoneme collection, optionally display `(Collection is not Phoneme)` when feasible, keep today's fallback behavior when no phoneme collection is available, and leave other effects unchanged.

Next, inspect the property editor path enough to decide whether warning display aliases are feasible without major changes. Search for `PropertyEditor("SelectionEditor")`, `StandardValuesCollection`, and selection editor implementations. The key question is whether the UI can display `Collection Name (Collection is not Phoneme)` while committing the real collection name or id back to `LipSync.MarkCollectionId`. If this is not supported by the existing editor and would require broad property-grid changes, do not implement the suffix in code. Record that finding in `Surprises & Discoveries` and `Decision Log`, then proceed with the plain collection name fallback approved by the specification.

Implement filtering in the LipSync module. The recommended low-risk approach is to add a LipSync-specific converter in `src/Vixen.Modules/Effect/LipSync/`, for example `LipSyncMarkCollectionNameConverter.cs`, and change the `LipSync.MarkCollectionId` attribute from `IMarkCollectionNameConverter` to the new converter. The converter should build its standard values from the current LipSync effect instance. It should return all collection names where `CollectionType == MarkCollectionType.Phoneme`. It should also include the currently selected collection if `_data.MarkCollectionId` resolves to an existing collection that is not already in the phoneme list. It should not include unrelated `Generic`, `Phrase`, or `Word` collections.

Keep the shared `IMarkCollectionNameConverter` unchanged. Keep `LipSyncData.MarkCollectionId` unchanged. Keep render-time mark resolution by `_data.MarkCollectionId` unchanged. Do not change Mark Collection `CollectionType` values and do not attempt to detect phoneme-compatible marks by mark text.

Review `LipSync.MarkCollectionId` setter after adding the converter. If the warning suffix is implemented as a display value, adjust the setter or converter so selecting that item still stores the selected collection id. If the warning suffix is not implemented because the editor cannot separate display and value, the setter should continue selecting by real collection name.

Review refresh behavior after selection changes. The desired least-invasive behavior is that a retained non-phoneme collection appears only while it is the current selection. Once the user selects a phoneme collection and the property grid refreshes, the old non-phoneme selection should no longer appear. If `OnPropertyChanged()` is not enough, use the existing `TypeDescriptor.Refresh(this)` pattern already present in `LipSync.cs`, but keep refresh changes narrowly scoped to the selection property.

Add focused automated tests. Prefer testing a small helper or converter method that can be invoked without launching the full WPF property grid. If a converter is difficult to test directly because it relies on `ITypeDescriptorContext`, add an internal helper method or small class in the LipSync module that accepts `IEnumerable<IMarkCollection>` and an optional selected `Guid`, then returns the allowed collection names. Test that helper. Avoid adding public API only for tests unless there is already a repository pattern for doing so.

Run focused tests first, then a broader test slice. At minimum, run a targeted test command for the new LipSync tests and `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LipSync --no-restore`. If the focused filter does not discover the new tests because of namespace choice, adjust the command and record the command that actually validates the tests. If feasible, run the full test project. If build or test execution is blocked by environment dependencies, record the exact error and run the broadest available validation that does not require missing dependencies.

Perform manual validation in the Timed Sequence Editor. Create or use a sequence with one `Generic`, one `Word`, one `Phrase`, and one `Phoneme` Mark Collection. Open a LipSync effect and confirm only the `Phoneme` collection appears. Load or construct an existing LipSync effect whose stored `MarkCollectionId` points to a `Generic` collection with valid phoneme mark labels and confirm it remains selected and renders. Select a real `Phoneme` collection, refresh or reopen the effect editor, and confirm the old generic collection disappears from the dropdown. Open another mark-driven effect, such as Text, and confirm its dropdown remains unfiltered.

At closeout, update Jira VIX-3946 with a comment. Use the project Jira skill before making this update. The comment must include any changes from the approved specification, the implementation summary, automated validation commands and results, manual validation results, and any residual risk or skipped validation. If the warning suffix was not implemented because the editor cannot support it without major changes, state that explicitly and explain that the approved fallback was used.

## Concrete Steps

Work from `C:\Dev\Vixen`.

1. Read required skills before implementation:

        Get-Content .agents\skills\dotnet-best-practices\SKILL.md
        Get-Content .agents\skills\csharp-docs\SKILL.md

2. Read the current source files:

        Get-Content docs\effects\vix-3946-lipsync-phoneme-mark-collections.md
        Get-Content src\Vixen.Modules\Effect\LipSync\LipSync.cs
        Get-Content src\Vixen.Modules\Effect\LipSync\LipSyncData.cs
        Get-Content src\Vixen.Core\TypeConverters\IMarkCollectionNameConverter.cs
        Get-Content src\Vixen.Core\Marks\IMarkType.cs
        Get-Content src\Vixen.Core\Marks\IMarkCollection.cs

3. Use the project Jira skill and update the VIX-3946 Jira description with this content, adjusted only for Jira formatting:

        Requirements:
        - The LipSync Mark Collection dropdown lists only Mark Collections whose CollectionType is Phoneme.
        - A currently selected non-phoneme collection remains selected and available so existing sequences continue to render.
        - If feasible without major property editor changes, the retained non-phoneme selection displays with "(Collection is not Phoneme)" appended.
        - If warning display aliases are not feasible, the retained non-phoneme selection may display as its plain collection name.
        - No unrelated Generic, Phrase, or Word collections appear in the LipSync dropdown.
        - When no collection is selected, LipSync keeps today's first-phoneme auto-selection and fallback behavior.
        - Other effects using Mark Collections are unchanged.

        Acceptance criteria:
        - A LipSync effect opened against mixed Generic, Phrase, Word, and Phoneme collections shows only Phoneme collections.
        - An existing LipSync effect selected to a non-phoneme collection keeps that selection and continues rendering.
        - After selecting a Phoneme collection, the old non-phoneme collection no longer appears after refresh.
        - Other mark-driven effects, such as Text, still show their normal unfiltered Mark Collection list.
        - Missing selected collections are handled without crashing.

        Test plan:
        - Automated tests cover phoneme-only filtering, retained legacy selections, deleted selections, and no-phoneme fallback list behavior.
        - Run focused LipSync tests and the broader available Vixen test command.
        - Manually validate the LipSync dropdown with mixed collection types, legacy non-phoneme selection retention, selection change cleanup, and another effect's unfiltered dropdown.

4. Research warning display feasibility:

        rg -n "SelectionEditor|StandardValuesCollection|GetStandardValues|PropertyEditor\\(\"SelectionEditor\"\\)" src

    Record the finding in `Surprises & Discoveries`. If a display alias cannot be committed as the real collection name or id without broad editor changes, use the plain-name fallback.

5. Implement the LipSync-specific filtering converter or helper:

    Create `src/Vixen.Modules\Effect\LipSync\LipSyncMarkCollectionNameConverter.cs` or an equivalently named file. The converter should follow the structure of `IMarkCollectionNameConverter`, but it should inspect the current `LipSync` instance and return only allowed names. If adding a public converter class, include XML documentation according to the project `csharp-docs` skill.

    The core filter should behave like this in plain terms:

        Start with every Mark Collection whose CollectionType is MarkCollectionType.Phoneme.
        Find the collection whose Id equals the current LipSync data MarkCollectionId.
        If that selected collection exists and is not already in the list, add it.
        Return the resulting collection names to the selection editor.

6. Update `src\Vixen.Modules\Effect\LipSync\LipSync.cs`:

    Replace the `TypeConverter` attribute on `MarkCollectionId` so it uses the LipSync-specific converter. Keep the property order and `PropertyEditor("SelectionEditor")` behavior.

    Review the `MarkCollectionId` setter. If values are still plain names, leave selection by name intact. If warning display aliases are implemented, ensure selection stores the correct collection id.

    Add a narrow refresh call after selection changes if needed so the legacy non-phoneme option disappears after the user selects a phoneme collection.

7. Add tests. Search existing test layout first:

        rg -n "LipSync|MarkCollectionName|IMarkCollectionNameConverter|MarkCollectionType" src\Vixen.Tests

    Add tests in the most appropriate existing test folder or create a focused file such as `src\Vixen.Tests\Effect\LipSyncMarkCollectionNameConverterTests.cs` if that matches local organization. Use real Mark Collection objects if the test project already references the Marks module; otherwise use a minimal test double implementing `IMarkCollection`.

8. Run formatting and focused validation:

        git diff --check
        dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LipSync --no-restore

    If the new tests use a different namespace, run the narrowest filter that includes them and record the exact command.

9. Run broader validation when feasible:

        dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

    If full tests are too slow or blocked by environment dependencies, run the broadest relevant subset and document the limitation.

10. Perform manual validation in Vixen:

    Open the Timed Sequence Editor. Create or load a sequence with `Generic`, `Phrase`, `Word`, and `Phoneme` Mark Collections. Add or edit a LipSync effect and inspect the Mark Collection dropdown. Then validate the retained legacy selection behavior, selection-change cleanup, another effect's unfiltered dropdown, and missing selected collection handling.

11. Update this ExecPlan:

    Mark completed progress items, add validation transcripts, record any implementation deviations in `Decision Log`, and summarize results in `Outcomes & Retrospective`.

12. Use the project Jira skill and add a closeout comment to VIX-3946. The comment must include:

        Implementation summary:
        - What files changed.
        - Whether the warning suffix was implemented or the approved plain-name fallback was used.
        - Any differences from docs/effects/vix-3946-lipsync-phoneme-mark-collections.md.

        Validation:
        - Exact automated test commands and pass/fail results.
        - Manual validation scenarios and results.
        - Any tests or manual checks not run, with reason.

## Validation and Acceptance

Automated acceptance starts with focused tests for the filtering behavior. The new tests should fail before the implementation because the shared converter returns every collection name, then pass after the LipSync-specific filtering exists.

Run from `C:\Dev\Vixen`:

        git diff --check
        dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LipSync --no-restore

Expected result: `git diff --check` prints no whitespace errors. The LipSync-focused test command reports that all discovered tests passed, including the new filter tests.

Run broader validation when practical:

        dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Expected result: the test project completes successfully. If unrelated environment issues prevent the full test run, record the exact failure and run the broadest relevant subset available.

Manual acceptance requires these observable behaviors:

Open a sequence with `Beat Marks` as `Generic`, `Words` as `Word`, `Phrases` as `Phrase`, and `Phonemes` as `Phoneme`. Add or edit a LipSync effect. The Mark Collection dropdown shows `Phonemes` and does not show `Beat Marks`, `Words`, or `Phrases`.

Open an existing LipSync effect whose stored `MarkCollectionId` points to `Old Manual Track`, a `Generic` collection with valid phoneme labels. The dropdown shows `Old Manual Track` plus any `Phoneme` collections, or shows `Old Manual Track (Collection is not Phoneme)` if warning aliases were implemented. The effect continues rendering from `Old Manual Track`.

Change the selection from `Old Manual Track` to `Phonemes`. Refresh or reopen the effect editor. The dropdown no longer shows `Old Manual Track`.

Open another mark-driven effect, such as Text. Its Mark Collection dropdown still shows the normal unfiltered list.

Delete the selected collection or load a sequence where the selected collection id no longer exists. Open LipSync and confirm the effect handles the missing collection without crashing and only available `Phoneme` collections appear in the dropdown.

Jira acceptance requires two updates. Before implementation, VIX-3946's description must contain the requirements, acceptance criteria, and test plan. At closeout, VIX-3946 must have a comment containing implementation notes, any specification changes, automated validation results, and manual validation results.

## Idempotence and Recovery

The implementation should be additive and safe to retry. Adding a LipSync-specific converter does not modify user sequence data. Running tests repeatedly should not change repository state.

Do not mutate existing Mark Collections or automatically retag them. If a manual validation sequence is created, it can be discarded after validation. If the converter approach needs to be backed out, restore the `TypeConverter` attribute on `LipSync.MarkCollectionId` to `IMarkCollectionNameConverter` and remove the LipSync-specific converter and tests.

If warning display aliases require broad property editor redesign, do not attempt that redesign in VIX-3946. Record the finding, use the approved plain-name fallback, and keep the filtering and compatibility behavior scoped to LipSync.

If tests fail for reasons unrelated to this change, record the failing command and error text in `Surprises & Discoveries`, then run the narrowest tests that prove the new behavior. Do not hide or delete failing tests.

## Artifacts and Notes

Approved specification:

        docs/effects/vix-3946-lipsync-phoneme-mark-collections.md

Key source files:

        src/Vixen.Modules/Effect/LipSync/LipSync.cs
        src/Vixen.Modules/Effect/LipSync/LipSyncMarkCollectionNameConverter.cs
        src/Vixen.Modules/Effect/LipSync/LipSyncData.cs
        src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs
        src/Vixen.Core/Marks/IMarkCollection.cs
        src/Vixen.Core/Marks/IMarkType.cs
        src/Vixen.Tests/Effects/LipSyncMarkCollectionNameConverterTests.cs
        src/Vixen.Tests/Vixen.Tests.csproj

Jira update completed for Milestone 1:

        Issue: VIX-3946
        URL: https://vixenlights.atlassian.net/browse/VIX-3946
        Updated: 2026-07-21 13:30:57 -0500
        Description now includes: Requirements, Acceptance Criteria, Test Plan, and Implementation Notes.

Selection editor research completed for Milestone 2:

        src/Vixen.Modules/Editor/EffectEditor/Editors/SelectionEditor.cs
            SelectionEditor uses EditorKeys.ComboBoxEditorKey.

        src/Vixen.Modules/Editor/EffectEditor/Themes/EditorResources.xaml
            ComboBoxEditorKey binds ItemsSource="{Binding ParentProperty.StandardValues}".
            ComboBoxEditorKey binds SelectedValue="{Binding StringValue}".
            There is no DisplayMemberPath, SelectedValuePath, or item template for separate display and value.

        src/Vixen.Modules/Editor/EffectEditor/PropertyItem.cs
            StandardValues returns Converter.GetStandardValues(this).

        src/Vixen.Modules/Editor/EffectEditor/PropertyItemValue.cs
            StringValue setter assigns Value = ConvertStringToValue(value).
            ConvertStringToValue returns the selected string directly when the edited property type is string.

        Decision:
            Use the approved plain-name fallback for retained non-phoneme selections.

Implementation completed for Milestone 3:

        src/Vixen.Modules/Effect/LipSync/LipSyncMarkCollectionNameConverter.cs
            Added a LipSync-specific TypeConverter.
            GetStandardValues reads the current LipSync effect and selected MarkCollectionId.
            GetAllowedMarkCollectionNames returns only Phoneme collections plus the selected existing collection.

        src/Vixen.Modules/Effect/LipSync/LipSync.cs
            MarkCollectionId now uses LipSyncMarkCollectionNameConverter.
            The shared IMarkCollectionNameConverter was not changed.

        src/Vixen.Tests/Effects/LipSyncMarkCollectionNameConverterTests.cs
            Added eight focused tests for the approved filtering rules.

        src/Vixen.Tests/Vixen.Tests.csproj
            Added a project reference to src/Vixen.Modules/Effect/LipSync/LipSync.csproj.

Focused validation run during Milestone 3:

        git diff --check
        Result: passed with no whitespace errors.

        dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~LipSyncMarkCollectionNameConverter --no-restore
        Result: passed. Failed: 0, Passed: 8, Skipped: 0, Total: 8.
        Notes: Existing warnings were emitted, including the known LiteDB NU1904 advisory and pre-existing compiler warnings in unrelated projects.

Current relevant behavior in `LipSync.cs`:

        SetupMarks() auto-selects the first MarkCollectionType.Phoneme collection when no MarkCollectionId is set.
        The LipSyncMode setter does the same when switching to LipSyncMode.MarkCollection.
        MarkCollectionId getter returns the selected collection name from _data.MarkCollectionId.
        MarkCollectionId setter stores the id for the collection whose Name equals the selected value.

Approved fallback for warning display:

        Prefer: Old Manual Track (Collection is not Phoneme)
        Fallback: Old Manual Track

The fallback is acceptable only when the property editor cannot display a warning label while committing the real collection name or id without major changes.

## Interfaces and Dependencies

No new NuGet package or project should be required.

The new converter should live in the LipSync effect module namespace:

        namespace VixenModules.Effect.LipSync

If implemented as a TypeConverter, the class should derive from `System.ComponentModel.TypeConverter` and provide the same basic conversion support as `IMarkCollectionNameConverter`: standard values are supported and exclusive, conversion from and to strings works for selection values, and `GetStandardValues(...)` returns the filtered list.

The converter or helper must depend on:

        System.ComponentModel
        System.Globalization
        Vixen.Marks

It may depend on:

        Vixen.Module.Effect

Only if needed to handle property-grid contexts containing effect arrays, matching the shared converter behavior. When the property grid passes an array of effects, preserve the existing broad behavior of finding a LipSync instance that supports marks and using that instance's Mark Collections. If multi-effect editing cannot identify a single LipSync selected id safely, return the intersection or first LipSync instance's allowed values only if that matches the shared converter pattern; otherwise document the limitation in `Surprises & Discoveries` and keep behavior no worse than the existing converter.

A helper signature for testability may look like this:

        internal static IReadOnlyList<string> GetAllowedMarkCollectionNames(
            IEnumerable<IMarkCollection> markCollections,
            Guid selectedMarkCollectionId)

The helper should return names in the same order as the sequence's Mark Collection list. It should avoid duplicates when the selected collection is already a phoneme collection.

Do not change `LipSyncData.MarkCollectionId`:

        [DataMember]
        public Guid MarkCollectionId { get; set; }

Do not change `MarkCollectionType` values:

        Generic
        Phrase
        Word
        Phoneme

## Revision Note

2026-07-21: Initial ExecPlan created from the approved `docs/effects/vix-3946-lipsync-phoneme-mark-collections.md` specification. The plan includes the requested Jira-description update before implementation and Jira closeout comment after validation.

2026-07-21: Added an explicit `Milestones` section after review found that the initial ExecPlan had `Plan of Work` and `Concrete Steps` but did not satisfy `.agents/PLANS.md`'s requirement for narrative, independently verifiable milestones.

2026-07-21: Completed Milestone 1 by reading the required implementation and Jira skills, then updating the VIX-3946 Jira description with the requirements, acceptance criteria, test plan, and implementation notes.

2026-07-21: Completed Milestone 2 by inspecting the Effect Editor selection editor and standard-values path. The selection editor does not support warning display aliases without broad changes, so the approved plain-name fallback will be used during implementation.

2026-07-21: Completed Milestone 3 by adding the LipSync-specific Mark Collection converter, switching LipSync.MarkCollectionId to it, adding focused converter tests, and recording passing focused validation.

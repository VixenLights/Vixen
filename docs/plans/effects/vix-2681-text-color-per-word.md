# Add Character and Word Cycle Modes to the Text Effect

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This repository contains `.agents/PLANS.md`; maintain this document according to that file. The improvement specification that led to this plan is `docs/effects/vix-2681-text-color-per-word.md`.

## Purpose / Big Picture

After this change, a Vixen user editing a Text effect can turn on one `Cycle Color` setting and choose `Cycle Mode = Character` or `Cycle Mode = Word`. Character mode preserves the current color-per-character behavior. Word mode uses the configured Text gradients one word at a time, so text such as `Tune To Music` can render `Tune`, `To`, and `Music` with different gradients while preserving the original spacing.

The same control pair also applies when Text is driven by a mark collection. Marks still determine when each word or mark label appears. `Cycle Mode` only controls gradient assignment: Character colors the active marked word or label by character, and Word colors each marked word or label by its word or mark position. The behavior is visible in the effect editor by enabling `Cycle Color`, changing `Cycle Mode`, scrubbing the timeline, and observing colors advance by character or word without changing text timing.

## Progress

- [x] (2026-07-07) Created this ExecPlan from `docs/effects/vix-2681-text-color-per-word.md` after reviewing `.agents/PLANS.md`, the project `dotnet-best-practices`, `csharp-docs`, `csharp-async`, and `dotnet-design-pattern-review` skills, `src/Vixen.Modules/Effect/Text/Text.cs`, `src/Vixen.Modules/Effect/Text/TextData.cs`, `src/Vixen.Modules/Effect/Text/Text.csproj`, and `src/Vixen.Tests/Vixen.Tests.csproj`.
- [x] (2026-07-07 19:50Z) Updated Jira VIX-2681 with the original request, planning references, unified `Cycle Color`/`Cycle Mode` requirements, Mark mode timing rule, migration rules, acceptance criteria, automated test plan, manual validation plan, and implementation-not-started status.
- [x] (2026-07-07 20:08Z) Added `src/Vixen.Tests/Effects/TextCycleColorModeTests.cs` and a Text project reference in `src/Vixen.Tests/Vixen.Tests.csproj`. The focused test command compiles and runs, then fails with five expected characterization failures: missing `CycleColorMode`, legacy per-character data not migrating `CycleColor` to true, and `CycleCharacterColor` still browsable.
- [x] (2026-07-07 20:23Z) Added `TextCycleColorMode`, serialized `TextData.CycleColorMode`, constructor defaulting, clone copying, and compatibility migration for legacy `CycleCharacterColor` and Mark mode `CycleColor`. Focused test run now reports `Passed: 4, Failed: 1`; the remaining failure is the Milestone 4 UI/property visibility work.
- [x] (2026-07-07 20:41Z) Replaced the exposed `CycleCharacterColor` setup entry with a hidden legacy property, exposed unified `Cycle Color` and `Cycle Mode` properties, and refreshed `Cycle Mode` browsability when `Cycle Color` changes. Focused test run now reports `Passed: 5, Failed: 0`.
- [x] (2026-07-07 21:06Z) Implemented Character and Word rendering behavior across normal Text, generated thumbnails, and mark-driven Text. Rendering now uses mode helpers instead of the visible legacy flag, normal Word mode preserves literal spaces without consuming gradients for extra spaces, mark-driven Word mode keeps each active word or label on its mark timing, and empty color lists return without modulo-by-zero exceptions. Focused test run now reports `Passed: 8, Failed: 0`.
- [ ] Complete focused automated tests and run validation commands.
- [ ] Manually validate the behavior in Vixen and update Jira VIX-2681 with final evidence.

## Surprises & Discoveries

- Observation: The current Text effect already has two separate color-cycling booleans. `CycleCharacterColor` drives per-character drawing, while `CycleColor` is used in mark-driven and rotate-related paths.
  Evidence: `src/Vixen.Modules/Effect/Text/Text.cs` exposes both properties around the color property block, uses `CycleCharacterColor` in `DrawTextWithBrush(...)` and `DrawText(...)`, and uses `CycleColor` with `_wordIteration` in mark-driven rendering.
- Observation: Mark-driven Text already has word-indexed color behavior.
  Evidence: `Text.cs` sets `_wordIteration` while walking marks in the visual representation path and chooses `Colors[_wordIteration % Colors.Count]` when `TextSource != TextSource.None && CycleColor`.
- Observation: The implementation is expected to be synchronous.
  Evidence: Text effect rendering, property editing, data migration, and tests use synchronous effect lifecycle and data-contract APIs. The project `csharp-async` skill is still noted because the source specification required it, but this feature should not introduce async methods unless Jira tooling or future test helpers require them.
- Observation: Milestone 2 focused tests compile and fail as intended before implementation.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TextCycleColorMode --no-restore` built the Text module and Vixen.Tests, then reported `Failed: 5, Passed: 0, Skipped: 0`. Failures were assertion failures for missing `CycleColorMode`, legacy character migration leaving `CycleColor` false, and `CycleCharacterColor` being browsable.
- Observation: Milestone 3 data-model implementation satisfies the focused data and migration tests while leaving the planned UI failure.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TextCycleColorMode --no-restore` reported `Failed: 1, Passed: 4, Skipped: 0`. The remaining failure is `TextProperties_ShowCycleModeOnlyWhenCycleColorIsEnabled`, which still sees `CycleCharacterColor` as browsable.
- Observation: Milestone 4 property visibility is now represented by descriptors instead of manual UI inspection.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TextCycleColorMode --no-restore` reported `Passed: 5, Failed: 0`; `CycleCharacterColor` is not browsable, `CycleColorMode` starts hidden, and enabling `CycleColor` makes `CycleColorMode` browsable.
- Observation: Milestone 5 rendering now routes cycle behavior through `CycleColorMode`.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TextCycleColorMode --no-restore` reported `Passed: 8, Failed: 0`. The added tests cover literal-space-preserving word runs, mode helper behavior, mark Word mode gating, and empty-color visual representation safety.
- Observation: A modern save with `CycleColor = false` could reload as enabled when stale legacy `CycleCharacterColor = true` was still serialized.
  Evidence: A focused regression test initially failed with `Assert.False() Failure` because `TextData.OnDeserialized(...)` treated `CycleCharacterColor = true` as authoritative even though `CycleColorMode` was present in the saved data.
- Observation: Character-cycle rendering had an existing leading offset compared with non-cycle and word-cycle rendering.
  Evidence: The character-cycle paths use `MeasureCharacterRanges(...)` and then draw each single character back into the measured bounds. `MeasureCharacterRanges(...)` already includes the GDI+ leading inset, so drawing a single character at that measured X position applies the inset again. A focused visual-representation bounds test covers character-cycle alignment against non-cycle rendering.

## Decision Log

- Decision: Treat `CycleColor` as the new serialized and user-facing `Cycle Color` flag, and keep `CycleCharacterColor` only as a legacy compatibility member unless implementation proves it can be removed safely.
  Rationale: Existing sequence data already serializes both booleans. Reusing `CycleColor` avoids adding a third boolean and matches the user's instruction to combine Mark mode's current `CycleColor` behavior into the new model.
  Date/Author: 2026-07-07 / Codex
- Decision: Add a `TextCycleColorMode` enum with `Character` and `Word` values.
  Rationale: The UI needs a two-value selector and rendering code needs an explicit mode. An enum is clearer and more serialization-friendly than string values or two mutually exclusive booleans.
  Date/Author: 2026-07-07 / Codex
- Decision: Existing old data with `CycleCharacterColor = true` migrates to `CycleColor = true` and `CycleColorMode = Character`; old Mark mode data with `CycleColor = true` and `CycleCharacterColor = false` migrates to `CycleColorMode = Word`.
  Rationale: Per-character compatibility must preserve existing visual output. Mark mode's existing `CycleColor` behavior acts like word/mark-position cycling, so Word is the compatibility-preserving mode for those effects.
- Decision: Empty Text color lists render no cycling text for that draw path instead of falling back to an arbitrary color.
  Rationale: The Text effect cannot choose a configured gradient when none exist. Returning early avoids modulo-by-zero exceptions and keeps the failure mode visually quiet until the user restores a color.
- Decision: Apply legacy `CycleCharacterColor` and Mark-mode migration only when serialized data does not contain `CycleColorMode`.
  Rationale: Absence of `CycleColorMode` identifies old sequence data. If modern saved data contains the mode, `CycleColor` must remain the authoritative enabled/disabled flag so disabled Cycle Color does not turn itself back on during reload.
- Decision: Compensate character-cycle drawing by subtracting the measured first-character leading inset from each per-character draw rectangle.
  Rationale: This keeps the existing character-range layout, color selection, and explode/fall behavior intact while aligning the rendered text origin with the non-cycle and word-cycle draw paths.
  Date/Author: 2026-07-07 / Codex
- Decision: Do not introduce new asynchronous APIs for this feature.
  Rationale: Effect rendering and setup property changes are synchronous in the current Text module. Adding async would increase complexity without any I/O or long-running operation to await.
  Date/Author: 2026-07-07 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. Jira VIX-2681 now contains the plan's requirements, design notes, migration rules, acceptance criteria, and test plan before code changes began.

Milestone 2 is complete. Focused tests now describe the missing data model, migration, and property visibility behavior without requiring production code changes. The tests compile and fail by assertion, so Milestone 3 can make them pass by adding `TextCycleColorMode` and migration logic.

Milestone 3 is complete. The Text data model now has a documented `TextCycleColorMode` enum, serialized `CycleColorMode` state, constructor defaulting to `Character`, clone preservation, and compatibility migration. The focused data tests pass; the remaining focused failure is the expected Milestone 4 property visibility change.

Milestone 4 is complete. The Text setup surface now exposes the unified `Cycle Color` checkbox and the `Cycle Mode` selector, hides the legacy `CycleCharacterColor` property, and refreshes mode visibility when cycling is toggled. Rendering behavior is intentionally unchanged until Milestone 5.

Milestone 5 is complete. Rendering now uses `CycleColorMode` helpers for Character and Word behavior, normal Word mode advances gradients per non-empty word while preserving literal space runs, mark-driven Word mode uses the active mark/word index without changing timing, and empty color collections do not throw in the covered visual representation path.

## Context and Orientation

Vixen is a Windows desktop application for sequencing animated light shows. A Text effect is a module that renders text into pixel output. The Text effect module lives in `src/Vixen.Modules/Effect/Text/`.

The main files for this change are:

- `src/Vixen.Modules/Effect/Text/Text.cs`, the runtime Text effect class. It exposes editable effect properties through attributes such as `ProviderDisplayName`, controls property visibility through `UpdateTextModeAttributes(...)` and `UpdatePositionXAttribute(...)`, prepares render state in `SetupRender()`, draws normal text through `DrawText(IEnumerable<string>, Graphics, Point)`, draws individual text runs through `DrawTextWithBrush(...)`, and draws mark-driven visual representations through the overload `DrawText(Graphics, Rectangle, string, LinearGradientMode, int)`.
- `src/Vixen.Modules/Effect/Text/TextData.cs`, the data-contract class serialized into sequence files. It currently stores `CycleColor` and `CycleCharacterColor` as `[DataMember]` booleans, initializes both to `false`, repairs older data in `OnDeserialized(StreamingContext)`, and clones settings in `CreateInstanceForClone()`.
- `src/Vixen.Modules/Effect/Text/TextSource.cs`, the enum that identifies whether Text comes from normal text entry, a mark collection, or mark collection labels.
- `src/Vixen.Modules/Effect/Text/Text.csproj`, the Text effect project. New source files under this folder are automatically included by the SDK project.
- `src/Vixen.Tests/Vixen.Tests.csproj`, the shared xUnit v3 test project. Add a project reference to `..\Vixen.Modules\Effect\Text\Text.csproj` if tests directly instantiate Text effect types.

Important terms:

- A mark is a timed label or timing point in a sequence. In this effect, mark-driven Text can display words or mark labels at times determined by marks.
- `Cycle Color` is the new user-facing boolean flag. When it is off, the cycle mode selector is hidden and Text color behavior remains the non-cycling behavior.
- `Cycle Mode` is the new user-facing enum selector. `Character` means gradients advance character by character. `Word` means gradients advance word by word.
- A word for this feature is a non-space run of text separated by the literal space character `' '`. Multiple spaces are preserved visually and do not consume gradients for empty words.
- A data-contract member is a property marked with `[DataMember]` that participates in Vixen's serialized effect data.

The current `Text.cs` has two color cycle booleans. `CycleCharacterColor` is exposed as a color property and causes per-character drawing in `DrawTextWithBrush(...)` and `DrawText(...)`. `CycleColor` is exposed separately and is currently visible for mark-driven or rotate paths; mark-driven code uses `_wordIteration` and `Colors[_wordIteration % Colors.Count]` to select gradients by mark/word position. This plan intentionally merges those concepts into one UI model.

The source specification requires using project skills. Apply `dotnet-best-practices` when writing C# and tests, `csharp-docs` when adding or changing public or protected APIs such as a public enum or public property, `csharp-async` as a guardrail that no blocking async patterns are introduced, and `dotnet-design-pattern-review` to review the final shape for unnecessary abstractions and cohesion.

## Plan of Work

Start by updating Jira VIX-2681 before implementation begins. Use the project `jira` skill at `.agents/skills/jira/SKILL.md` for that update. The Jira update should summarize the unified `Cycle Color`/`Cycle Mode` behavior, Mark mode timing preservation, migration rules, acceptance criteria, and test plan. Do not claim implementation is complete in this first update.

Next add focused tests. If the test project does not yet reference the Text effect project, add this project reference to `src/Vixen.Tests/Vixen.Tests.csproj`:

    <ProjectReference Include="..\Vixen.Modules\Effect\Text\Text.csproj" />

Keep it as a project reference. Do not edit `Vixen.sln` because no new project is being added. Create `src/Vixen.Tests/Effects/TextCycleColorModeTests.cs`. Use xUnit v3 and the arrange/act/assert style already used in `src/Vixen.Tests`. Add tests for `TextData` defaults, cloning, property visibility, and data-contract deserialization. Use `DataContractSerializer` or `DataContractJsonSerializer` consistently with the serialization path being tested; `src/Vixen.Tests/Sequencer/LayerMixingFilterDataJsonSerializationTests.cs` shows an existing JSON serializer pattern, and `src/Vixen.Modules/Sequence/Timed/TimedSequenceMigrator.cs` shows `DataContractSerializer` use for module data.

Add `src/Vixen.Modules/Effect/Text/TextCycleColorMode.cs`. Define a public enum in namespace `VixenModules.Effect.Text` with `Character` and `Word`. Add XML documentation because the enum is public. Use present-tense summary text and document each value succinctly.

Update `TextData.cs`. Add `[DataMember] public TextCycleColorMode CycleColorMode { get; set; }` and set `CycleColorMode = TextCycleColorMode.Character` in the constructor. Keep `[DataMember] public bool CycleCharacterColor { get; set; }` as a legacy compatibility member. In `OnDeserialized(StreamingContext c)`, after the existing default repairs, normalize the new data. If `CycleCharacterColor` is true, set `CycleColor = true` and `CycleColorMode = TextCycleColorMode.Character`. If `CycleCharacterColor` is false but `CycleColor` is true and `TextSource != TextSource.None`, set `CycleColorMode = TextCycleColorMode.Word`. If neither legacy flag requests cycling, leave `CycleColor` false and `CycleColorMode` as Character. If `CycleColor` is true and the data already contains a non-default `CycleColorMode`, preserve that explicit mode unless doing so would break the legacy precedence rule for `CycleCharacterColor = true`. Add `CycleColorMode = CycleColorMode` to `CreateInstanceForClone()`.

If implementation proves that `OnDeserialized` cannot tell whether `CycleColorMode` was omitted from old data or explicitly serialized as `Character`, keep the rule simple and deterministic: `CycleCharacterColor = true` wins as Character, and old Mark mode `CycleColor = true` with `CycleCharacterColor = false` becomes Word. Record the exact serializer behavior in `Surprises & Discoveries`.

Update `Text.cs` color properties. Replace the visible `CycleCharacterColor` setup property with a compatibility wrapper or hide it through property browsability. The user-visible `CycleColor` property must have display name `Cycle Color`, description `Cycle Color`, and property order just before the new mode. Add a `CycleColorMode` property with display name `Cycle Mode`, description `Selects whether Cycle Color advances per character or per word, including text displayed from marks.`, and property order immediately after `Cycle Color`. The `CycleColor` setter must set `_data.CycleColor`, mark dirty, call the attribute refresh method, and raise `OnPropertyChanged()`. The `CycleColorMode` setter must set `_data.CycleColorMode`, mark dirty, and raise `OnPropertyChanged()`.

Create private helpers in `Text.cs` to keep visibility and render checks readable. For example:

    private bool IsCycleColorSupported()
    {
        return Direction != TextDirection.Rotate || TextSource != TextSource.None || Direction == TextDirection.Rotate;
    }

    private bool IsCharacterCycleEnabled()
    {
        return CycleColor && CycleColorMode == TextCycleColorMode.Character;
    }

    private bool IsWordCycleEnabled()
    {
        return CycleColor && CycleColorMode == TextCycleColorMode.Word;
    }

The exact helper names can differ, but the intent must be clear: rendering should no longer branch directly on visible `CycleCharacterColor`, and property visibility should be derived from one `CycleColor` state. Avoid over-generalizing this into a separate service; this is localized effect behavior and a small private helper set is enough.

Update `UpdateTextModeAttributes(...)` and `UpdatePositionXAttribute(...)`. The required visible state is: `CycleColor` is visible wherever cycling is supported, including normal non-rotate text, rotate behavior currently supported by `CycleColor`, and mark-driven text; `CycleColorMode` is visible only when `CycleColor` is visible and true; `CycleCharacterColor` is not visible in the normal editor. Both methods must set `CycleColorMode` visibility consistently. The `CycleColor` setter must call the refresh method so toggling the checkbox immediately shows or hides `Cycle Mode`.

Update rendering in small, testable steps. First change all current `CycleCharacterColor` render checks that mean "per-character cycling" to use the new helper `IsCharacterCycleEnabled()`. Preserve special direction behavior such as `Direction > (TextDirection)5` only if it is unrelated to color cycling. Then implement word-level drawing for non-mark normal text. A practical approach is to add a helper that tokenizes a string into word and separator runs, draws word runs with `Colors[wordIndex % Colors.Count]`, preserves separator runs by advancing the drawing X position, and skips gradient advancement for empty words. Do not use culture-specific word splitting; split only on literal spaces.

Apply word mode in the normal drawing helpers and in generated visual representation drawing. For mark-driven Text, preserve current timing and text selection. The mark collection code that chooses which word or label is active must remain responsible for timing. Only color selection changes: Character mode colors characters inside the active word or label; Word mode uses `_wordIteration` or an equivalent mark/word index to choose the gradient for the active word or label. Be careful not to make Word mode split a single mark label into separately timed words unless the existing TextSource behavior already does that.

Handle empty colors defensively. Any path that indexes `Colors[index % Colors.Count]` must avoid modulo by zero. If `Colors.Count == 0`, render no cycling text for that frame or use the existing fallback if code inspection finds one. Record the decision in the Decision Log and cover it with a test.

After implementation, update Jira VIX-2681 with final implementation notes and validation evidence using the project `jira` skill. Include focused test results, broader test or build results, and manual validation status.

## Milestones

Milestone 1 updates tracking and confirms the design before code changes. Read this ExecPlan and update Jira VIX-2681 with the purpose, unified control model, Mark mode timing rule, migration rules, acceptance criteria, and test plan. Use the project `jira` skill. Acceptance for this milestone is a Jira issue that clearly states `Cycle Color` plus `Cycle Mode` covers both normal and mark-driven Text, and that implementation has not yet been claimed complete.

Milestone 2 adds characterization tests and any needed test project reference. Add `src/Vixen.Tests/Effects/TextCycleColorModeTests.cs` and a project reference to `src/Vixen.Modules/Effect/Text/Text.csproj` if needed. Tests should initially fail or be skipped only where production behavior is missing; they should not fail because of missing references or compile errors. Include tests for default data, clone behavior, property visibility around `CycleColor`, and migration from old serialized data.

Milestone 3 implements the data model and migration. Add `TextCycleColorMode`, update `TextData`, and run the focused tests. At the end of this milestone, new `TextData` defaults to cycling off and Character mode, cloned data preserves the mode, old per-character data migrates to Character, and old Mark mode `CycleColor` data migrates to Word.

Milestone 4 updates the effect setup properties and visibility. Modify `Text.cs` so the editor exposes `Cycle Color` and, only when enabled, `Cycle Mode`. The legacy `CycleCharacterColor` property must not appear as a normal user-facing setup property. Focused property visibility tests should pass. Manual inspection in Vixen can wait until the final milestone, but the property descriptors should prove the intended UI model.

Milestone 5 implements rendering behavior. Replace per-character branches with Character mode checks and add Word mode drawing while preserving spacing. Keep Mark mode timing unchanged: marks choose when words or labels appear, and the cycle mode only chooses gradients. Add or update tests that prove Character mode preserves representative per-character behavior, Word mode maps words to gradients, multiple spaces do not consume extra gradients, Mark mode timing still selects the same word or label, and empty colors do not throw.

Milestone 6 validates the feature. Run the focused Text tests, then run the broader test project or a build command if the full suite is too slow. Manually validate in Vixen: normal Text with multiple words, Mark Collection text with multiple words and marks, `Cycle Mode = Character`, `Cycle Mode = Word`, save and reopen persistence, and disabled `Cycle Color` hiding `Cycle Mode`. Update this ExecPlan's `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` with evidence.

Milestone 7 updates Jira VIX-2681 with final evidence. Use the project `jira` skill. Include the implementation summary, focused test command and result, broader validation command and result, and manual validation notes. Acceptance is a Jira issue that lets a reviewer see what changed and how it was verified.

## Concrete Steps

Work from the repository root:

    cd C:\Dev\Vixen

Before editing, inspect the worktree:

    git status --short

Read the source specification and relevant files:

    Get-Content -LiteralPath docs\effects\vix-2681-text-color-per-word.md
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Text\Text.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Text\TextData.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Text\TextSource.cs
    Get-Content -LiteralPath src\Vixen.Tests\Vixen.Tests.csproj

Update Jira VIX-2681 before code changes. Use `.agents/skills/jira/SKILL.md`. The Jira content should include:

    Purpose: Unify Text effect color cycling into one Cycle Color setting with Character and Word modes.
    Mark mode rule: Marks still control timing; Cycle Mode controls only gradient assignment.
    Migration: CycleCharacterColor=true -> Character; Mark mode CycleColor=true with CycleCharacterColor=false -> Word.
    Acceptance: UI visibility, character compatibility, word cycling, mark timing preservation, persistence, tests, and manual validation.

Add tests and the Text project reference if needed:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TextCycleColorMode --no-restore

Before implementation, the focused command may fail because the tests describe missing behavior. It should fail with assertion failures, not project-load or compile errors. After implementation, expect the focused Text tests to pass.

Implement the data model:

    src\Vixen.Modules\Effect\Text\TextCycleColorMode.cs
    src\Vixen.Modules\Effect\Text\TextData.cs

Implement the UI property and render changes:

    src\Vixen.Modules\Effect\Text\Text.cs

Run focused tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TextCycleColorMode --no-restore

Run broader validation. Prefer the full test project:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If the full suite is not practical in the current environment, run a build and record why the full suite was skipped:

    dotnet msbuild src\Vixen.Modules\Effect\Text\Text.csproj -t:Build -p:Configuration=Debug -p:Platform=x64
    dotnet msbuild src\Vixen.Tests\Vixen.Tests.csproj -t:Build -p:Configuration=Debug

Manual validation in Vixen should use a Debug or Release build. Add a Text effect to a pixel group, set text to `Tune To Music`, configure at least three distinct gradients, confirm `Cycle Mode` is hidden while `Cycle Color` is off, then enable `Cycle Color`. With `Cycle Mode = Character`, scrub the timeline and confirm colors advance by character. With `Cycle Mode = Word`, confirm `Tune`, `To`, and `Music` advance by word. Configure Mark Collection text with multiple words and marks, confirm Word mode preserves word-to-mark timing while advancing colors per marked word, then confirm Character mode colors the active marked word by character without changing timing. Save and reopen the sequence and confirm `Cycle Color` and `Cycle Mode` persist.

## Validation and Acceptance

The feature is accepted when all of these behaviors are demonstrated:

1. A new Text effect shows `Cycle Color` off by default and does not show `Cycle Mode`.
2. Enabling `Cycle Color` immediately shows `Cycle Mode` with `Character` and `Word`.
3. `Cycle Mode = Character` produces representative output matching the old `CycleCharacterColor = true` behavior.
4. `Cycle Mode = Word` maps gradients to space-delimited words and preserves multiple spaces.
5. Mark mode still uses marks to time which word or label appears.
6. Mark mode with Character colors the active marked word or label by character.
7. Mark mode with Word colors active marked words or labels by word/mark position.
8. Old serialized data with `CycleCharacterColor = true` loads as `CycleColor = true` and `CycleColorMode = Character`.
9. Old serialized Mark mode data with `CycleColor = true` and `CycleCharacterColor = false` loads as `CycleColor = true` and `CycleColorMode = Word`.
10. Cloning or copying Text data preserves `CycleColor` and `CycleColorMode`.
11. Empty and single-gradient color lists do not crash rendering.
12. Focused tests pass.
13. A broader test or build command is run and recorded.
14. Jira VIX-2681 is updated before implementation and after validation.

Expected focused test result after completion:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~TextCycleColorMode --no-restore
    ...
    Passed!  Failed: 0, Passed: <N>, Skipped: 0

Record the actual number of passed tests in `Progress` and `Outcomes & Retrospective`.

## Idempotence and Recovery

The plan is safe to rerun. Adding `TextCycleColorMode.cs` is additive. Re-running tests is safe. If the test project reference already exists, do not add a duplicate. If a code change fails halfway, use `git diff` to inspect only files touched by this plan and continue from the last completed milestone.

Do not remove `CycleCharacterColor` from serialized data until compatibility has been proven with tests. If migration behavior is wrong, revert only the local migration edits in `TextData.cs` and keep the enum and tests in place while correcting the logic. Do not use `git reset --hard` or revert unrelated user changes.

If rendering changes cause broad visual regressions, preserve the data model and UI work, then narrow the render change to the smallest helper that maps a text run to a gradient index. Update `Decision Log` before changing course.

## Artifacts and Notes

The source specification is `docs/effects/vix-2681-text-color-per-word.md`.

The implementation should avoid large unrelated refactors in `Text.cs`. The file is already large and uses regions for property groups; keep new logic close to the current color properties and drawing helpers.

Potential test names:

    TextData_DefaultsToCycleColorOffAndCharacterMode
    TextData_CloneCopiesCycleColorMode
    TextData_MigratesLegacyCharacterCycleToCharacterMode
    TextData_MigratesLegacyMarkCycleToWordMode
    TextProperties_ShowCycleModeOnlyWhenCycleColorIsEnabled
    TextRender_CycleModeCharacterMatchesLegacyCharacterSequence
    TextRender_CycleModeWordAdvancesPerSpaceDelimitedWord
    TextRender_CycleModeWordPreservesMultipleSpaces
    TextRender_MarkModeCharacterPreservesMarkTiming
    TextRender_MarkModeWordPreservesMarkTimingAndUsesWordIndex
    TextRender_EmptyColorsDoesNotThrow

If direct render tests are too expensive, extract an internal helper for assigning gradient indexes to character and word runs, expose internals to `Vixen.Tests` only if necessary, and record that decision. Prefer end-to-end effect lifecycle tests where practical.

## Interfaces and Dependencies

Add `src/Vixen.Modules/Effect/Text/TextCycleColorMode.cs`:

    namespace VixenModules.Effect.Text
    {
        /// <summary>
        /// Specifies how the Text effect advances gradients when cycle color is enabled.
        /// </summary>
        public enum TextCycleColorMode
        {
            /// <summary>
            /// Advances gradients for each rendered character.
            /// </summary>
            Character,

            /// <summary>
            /// Advances gradients for each rendered word or marked word.
            /// </summary>
            Word
        }
    }

Update `TextData` to contain:

    [DataMember]
    public TextCycleColorMode CycleColorMode { get; set; }

Keep:

    [DataMember]
    public bool CycleColor { get; set; }

Keep `CycleCharacterColor` as a `[DataMember]` compatibility property unless tests prove removal is safe. If it remains public, document in code comments or XML docs that it is legacy compatibility state and not the preferred UI setting.

Update `Text.cs` to expose:

    public bool CycleColor { get; set; }
    public TextCycleColorMode CycleColorMode { get; set; }

The visible display names must be `Cycle Color` and `Cycle Mode`.

No new NuGet packages are required. No new project is required. No async APIs are expected. Use existing .NET and System.Drawing APIs already used by Text.

2026-07-07 / Codex: Created this ExecPlan from the detailed specification and current Text implementation research so a future agent can implement VIX-2681 from this file alone.

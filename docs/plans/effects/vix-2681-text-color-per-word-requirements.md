# VIX-2681 Text Effect Color Per Word Specification

## Purpose

VIX-2681 adds a second color-cycling mode to the Text effect and unifies the existing color-cycling controls. Today the Text effect can cycle gradients per character through the `CycleCharacterColor` setting, and mark-driven Text can use `CycleColor` in a way that effectively cycles by word as marks advance through words. After this change, users should be able to enable one `Cycle Color` setting and then choose whether gradients advance per character or per word in normal text and mark-driven text. This lets a phrase such as `Tune to 88.1 FM` render each word with the next configured gradient while preserving the current per-character behavior for existing sequences.

This document is a detailed specification. It is not an ExecPlan, but it is intended to contain enough detail for a self-contained ExecPlan under `docs/plans/effects/` to be created from it.

## Current Implementation Context

The Text effect module lives under `src/Vixen.Modules/Effect/Text/`.

The main runtime class is `src/Vixen.Modules/Effect/Text/Text.cs`. It exposes effect properties through attributes consumed by the Vixen effect setup UI. The color-related properties currently include:

- `Colors`, a `List<ColorGradient>` used by all Text color and gradient rendering modes.
- `CycleCharacterColor`, a Boolean displayed as `CycleCharacterColor`.
- `CycleColor`, a Boolean displayed as `CycleColor`, currently used for mark-driven text and rotate-related paths.
- `GradientMode`, which selects how each gradient is applied to text.

The serialized data class is `src/Vixen.Modules/Effect/Text/TextData.cs`. It currently stores both `CycleColor` and `CycleCharacterColor` as `[DataMember]` Boolean properties, initializes both to `false`, migrates older fields in `OnDeserialized`, and copies both values in `CreateInstanceForClone()`.

Rendering behavior is split across several paths in `Text.cs`. The primary per-character drawing behavior is in `DrawTextWithBrush(...)` and `DrawText(...)`, where `CycleCharacterColor` causes the renderer to measure characters and select `Colors[_characterNumber % Colors.Count]` or similar per-character gradients. Existing mark-driven text already has some word-oriented behavior through `_wordIteration` when `TextSource != TextSource.None && CycleColor`.

Property visibility is controlled by `UpdateTextModeAttributes(...)` and `UpdatePositionXAttribute(...)`. These methods currently hide or show `CycleCharacterColor` and `CycleColor` based on direction, text source, rotate mode, and the current `CycleCharacterColor` value.

## User-Facing Requirements

The setup UI must present one primary Boolean setting named `Cycle Color` instead of exposing the old `Cycle Color per Character` concept.

When `Cycle Color` is off, the Text effect must behave as it does today with color cycling disabled. The cycle mode selector must not be visible.

When `Cycle Color` is on, a mode selector with display name `Cycle Mode` must be visible. The selector must offer exactly two user-facing values:

- `Character`
- `Word`

`Character` mode must preserve the existing `CycleCharacterColor` behavior. Each character advances to the next gradient from the `Colors` list, wrapping to the first gradient when the end of the list is reached.

`Word` mode must split text on the space character (`' '`) and advance to the next gradient for each word, wrapping to the first gradient when the end of the `Colors` list is reached. A word is the non-space text between spaces. Spaces are separators, not separately colored words, and the rendered spacing must remain visually equivalent to the input text.

Mark-driven Text must use the same `Cycle Color` and `Cycle Mode` controls. Marks still determine when each word or mark label is displayed; the selected cycle mode determines only how gradients are assigned to the displayed text. In Mark mode, `Character` means the currently displayed marked word or label cycles by character, and `Word` means each marked word or label advances through the gradient list by word/mark position. This combines the existing Mark mode `CycleColor` behavior with the new general `Cycle Mode` behavior instead of keeping a separate Mark-only color-cycling concept.

The default for new Text effects must remain color cycling off. A new effect must open with `Cycle Color = false` and the `Cycle Mode` selector hidden.

The gradient source is unchanged. Character and word cycling both use the existing `Colors` gradient list in the same order users configure today.

## Proposed Data Model

Introduce a Text-specific enum in `src/Vixen.Modules/Effect/Text/`, for example `TextCycleColorMode.cs`, with the values:

- `Character`
- `Word`

The enum should use the existing namespace, `VixenModules.Effect.Text`. If the enum is public or protected, add XML documentation in the same change by applying the project `csharp-docs` skill.

`TextData` should continue to serialize enough state to preserve existing sequence files and newly selected word-mode settings. The recommended target model is:

- Keep `CycleColor` as the serialized Boolean that represents the new user-facing `Cycle Color` setting.
- Add a serialized `TextCycleColorMode CycleColorMode` property.
- Treat `CycleColorMode = Character` as the default whenever existing serialized data does not contain the new mode.
- Keep the old serialized `CycleCharacterColor` member only as a compatibility field if removing or renaming it would break deserialization of existing sequences. If retained, it should no longer be exposed as a primary user-facing setting after migration is complete.

Migration behavior must be explicit:

- Existing data with `CycleCharacterColor = false` must load with `CycleColor = false`. `CycleColorMode` may default to `Character`, but it is not user-visible until cycling is enabled.
- Existing data with `CycleCharacterColor = true` must load with `CycleColor = true` and `CycleColorMode = Character`.
- Existing data that already has `CycleColor = true` for mark-driven behavior and did not have `CycleCharacterColor = true` must continue to load as cycling enabled with `CycleColorMode = Word`, because the current Mark mode `CycleColor` behavior acts like per-word color cycling.
- If old data has both `CycleCharacterColor = true` and `CycleColor = true`, `CycleCharacterColor` compatibility takes precedence and the data must load with `CycleColor = true` and `CycleColorMode = Character`.
- Cloning a Text effect must copy `CycleColor` and `CycleColorMode`.

Because `bool` and enum default values are both valid values, the implementation must not rely solely on default CLR values to determine whether old data omitted the new field. The ExecPlan should include a short research step to inspect the repository's data-contract migration patterns and choose a reliable compatibility approach. If the serializer cannot distinguish omitted `CycleColorMode` from an explicit `Character` value, that is acceptable because `Character` is the required compatibility default.

## Proposed Public Effect Properties

In `Text.cs`, expose `CycleColor` as the user-facing Boolean setting:

- Category: `Color`
- Display name: `Cycle Color`
- Description: update to match the display name or use the existing resource pattern if one exists for Text effect strings.
- Property order: keep it near the existing color list, before `Cycle Mode`.

Expose a new `CycleColorMode` property:

- Category: `Color`
- Display name: `Cycle Mode`
- Description: a concise description such as `Selects whether Cycle Color advances per character or per word, including text displayed from marks.`
- Property order: immediately after `Cycle Color`.
- Browsable only when `CycleColor` is `true` and the active Text mode/direction supports color cycling.

`CycleCharacterColor` should no longer appear as the normal setup UI property. Existing callers or serialized data may still need the underlying data member during migration, but users should not see both `CycleCharacterColor` and the new `Cycle Color`/`Cycle Mode` pair.

When the `CycleColor` setter changes, it must mark the effect dirty, refresh property visibility, and raise property change notification. The visibility refresh is required so `Cycle Mode` appears immediately when the user enables `Cycle Color` and disappears immediately when the user disables it.

When the `CycleColorMode` setter changes, it must mark the effect dirty and raise property change notification. It should also refresh render-affecting attributes if the selected mode changes which settings are visible.

## Rendering Requirements

When `CycleColor` is `false`, the renderer must continue to draw text using the same gradient behavior it uses today when per-character cycling is disabled.

When `CycleColor` is `true` and `CycleColorMode` is `Character`, the renderer must behave like the current `CycleCharacterColor = true` implementation. This includes existing handling for standard text, rotated text, mark collection text, explode, fall, fades, brightness level, and gradient mode unless a specific path is already unsupported today. In mark-driven Text, Character mode must color the characters inside the word or label being displayed for the active mark; it must not change which word is timed to which mark.

When `CycleColor` is `true` and `CycleColorMode` is `Word`, the renderer must advance gradients by word rather than by character. In mark-driven Text, this is the continuation of the existing Mark mode `CycleColor` behavior: marks still time each word to its mark, and the color assigned to that displayed word advances by word/mark index. Word mode must apply consistently across the main Text render paths that currently honor color cycling:

- normal text rendering through `DrawTextWithBrush(...)`;
- location or target-positioned Text rendering if it uses the same drawing helpers;
- mark-driven rendering through `DrawText(...)` and `_wordIteration`, with timing and text selection still controlled by the mark collection;
- thumbnail generation, because Text forces visual representation generation through `ForceGenerateVisualRepresentation`.

Word splitting is defined as splitting on the literal space character (`' '`). Tabs, newlines, and punctuation are not new word separators for this feature unless they are already handled as separate text lines by existing Text effect logic. Multiple consecutive spaces must preserve the user's spacing in the rendered output and must not consume extra gradients for empty words.

For a single text line `RED GREEN BLUE` with three gradients `[A, B, C]`, word mode must render:

- `RED` with gradient `A`;
- `GREEN` with gradient `B`;
- `BLUE` with gradient `C`;
- spaces between those words without causing an additional gradient advance.

For `ONE  TWO` with two spaces and gradients `[A, B]`, word mode must render `ONE` with `A`, preserve two visible spaces, and render `TWO` with `B`.

If there are more words than gradients, selection wraps by modulo count. For four words and two gradients `[A, B]`, the word gradients are `A, B, A, B`.

If `Colors` is empty, the renderer must not throw due to modulo by zero. The ExecPlan should include a decision to either preserve the current fallback behavior if one exists or render transparent/no text for that frame. The chosen behavior must be tested.

## UI and Visibility Rules

The new property visibility must be derived from the current visibility of the legacy cycling controls, not invented independently. The implementation must review and update both `UpdateTextModeAttributes(...)` and `UpdatePositionXAttribute(...)`.

Required visibility behavior:

- `Cycle Color` is visible wherever the old `CycleCharacterColor` setting was visible for non-rotate text.
- `Cycle Color` remains visible for any existing path where the current `CycleColor` property is required, including rotate or mark-driven paths.
- `Cycle Mode` is visible only when `Cycle Color` is visible and enabled.
- `Cycle Mode` is hidden for new effects because `Cycle Color` defaults to off.
- Disabling `Cycle Color` hides `Cycle Mode` immediately and must not discard the last selected mode. Re-enabling `Cycle Color` in the same session should show the prior selected mode.

The ExecPlan should include a short UI behavior audit because the current `CycleColor` property is visible under different conditions from `CycleCharacterColor`. The expected outcome is a single `Cycle Color` setting with one `Cycle Mode` selector wherever cycling is supported, including Mark mode.

## Backward Compatibility

Existing sequences must open without user action.

Existing Text effects that had `CycleCharacterColor = false` must not unexpectedly start cycling colors after upgrade.

Existing Text effects that had `CycleCharacterColor = true` must look the same after upgrade by loading as `Cycle Color = true` and `Cycle Mode = Character`.

Existing Text effects that used current `CycleColor` behavior for mark-based text must continue to render with cycling enabled and word-style color advancement. These effects must migrate to `Cycle Color = true` and `Cycle Mode = Word` unless they also had the legacy `CycleCharacterColor = true`, in which case character compatibility takes precedence.

The implementation should not change unrelated Text effect settings, property order outside the color cycling area, font behavior, target positioning behavior, or gradient mode semantics.

## Acceptance Criteria

1. A new Text effect shows `Cycle Color` off by default and does not show `Cycle Mode`.
2. Enabling `Cycle Color` immediately shows `Cycle Mode` with `Character` and `Word` choices.
3. Choosing `Character` produces the same output as the old `Cycle Color per Character` behavior for representative Text effect scenarios.
4. Choosing `Word` cycles the configured gradients once per space-delimited word while preserving spaces in the rendered text.
5. In Mark mode, marks continue to determine which word or label appears at each mark, while `Cycle Mode = Character` colors the displayed marked text by character.
6. In Mark mode, marks continue to determine which word or label appears at each mark, while `Cycle Mode = Word` colors each marked word or label by word/mark position.
7. Existing serialized Text effect data with `CycleCharacterColor = false` and no Mark mode `CycleColor` behavior loads with cycling disabled.
8. Existing serialized Text effect data with `CycleCharacterColor = true` loads with cycling enabled and `Cycle Mode = Character`.
9. Existing serialized Mark mode Text effect data with `CycleColor = true` and `CycleCharacterColor = false` loads with cycling enabled and `Cycle Mode = Word`.
10. Cloning or copying a Text effect preserves `Cycle Color` and `Cycle Mode`.
11. Empty or single-item `Colors` lists do not crash rendering.
12. Focused automated tests pass, and the broader `src/Vixen.Tests/Vixen.Tests.csproj` suite or an agreed build command is run and recorded.
13. Jira issue VIX-2681 is updated with the final specification, acceptance criteria, and test plan before implementation begins, then updated again with validation evidence after implementation.

## Test Plan

Add focused tests under `src/Vixen.Tests/Effects/` or the nearest existing effect-test location. If tests need to instantiate the Text effect directly, add a project reference from `src/Vixen.Tests/Vixen.Tests.csproj` to `src/Vixen.Modules/Effect/Text/Text.csproj` using the repository's project-reference rules.

Recommended automated coverage:

- Data defaults: new `TextData` has `CycleColor = false` and `CycleColorMode = Character`.
- Clone behavior: `CreateInstanceForClone()` copies both `CycleColor` and `CycleColorMode`.
- Migration behavior: deserializing old data without `CycleColorMode` and with `CycleCharacterColor = true` results in `CycleColor = true` and `CycleColorMode = Character`.
- Migration behavior: deserializing old non-Mark data with `CycleCharacterColor = false` and no active mark-driven `CycleColor` behavior results in `CycleColor = false`.
- Migration behavior: deserializing old Mark mode data with `CycleColor = true` and `CycleCharacterColor = false` results in `CycleColor = true` and `CycleColorMode = Word`.
- Property visibility: `Cycle Mode` is hidden when `Cycle Color` is false and visible when `Cycle Color` is true.
- Character compatibility: a deterministic render with `CycleColor = true` and `CycleColorMode = Character` matches the old per-character color sequence.
- Word behavior: a deterministic render or extracted helper test proves `ONE TWO THREE` maps gradients per word as `0, 1, 2`.
- Mark mode Character behavior: a deterministic render or helper test proves the mark still chooses the active word/label and the selected word/label cycles by character.
- Mark mode Word behavior: a deterministic render or helper test proves the mark still chooses the active word/label and the selected word/label receives the gradient for its word/mark index.
- Multiple spaces: `ONE  TWO` preserves spacing and advances gradients only for `ONE` and `TWO`.
- Wrap behavior: more words than gradients wrap through the gradient list without throwing.
- Empty colors: render or helper logic handles `Colors.Count == 0` without throwing.

Manual validation in Vixen:

1. Start Vixen from a Debug or Release build.
2. Create or open a sequence with a pixel element group.
3. Add a Text effect with text containing at least three words, such as `Tune To Music`.
4. Configure at least three visibly different gradients in `Colors`.
5. Confirm `Cycle Mode` is hidden while `Cycle Color` is off.
6. Enable `Cycle Color`, select `Character`, scrub the timeline, and confirm colors advance by character.
7. Select `Word`, scrub the timeline, and confirm colors advance by word.
8. Save and reopen the sequence and confirm `Cycle Color` and `Cycle Mode` persist.
9. Configure Text to use a Mark Collection source with multiple words and marks, enable `Cycle Color`, and confirm `Cycle Mode = Word` preserves word-to-mark timing while advancing colors per marked word.
10. Switch the same Mark Collection setup to `Cycle Mode = Character` and confirm the active marked word still follows mark timing while its characters cycle through the gradients.

## ExecPlan Handoff Requirements

Create the implementation ExecPlan under `docs/plans/effects/`, for example `docs/plans/effects/vix-2681-text-color-per-word.md`. The ExecPlan must follow `.agents/PLANS.md`.

The ExecPlan must start with research milestones before implementation:

- Read this specification and the current Text effect files.
- Use the project `dotnet-best-practices`, `csharp-docs`, `csharp-async`, and `dotnet-design-pattern-review` skills as required by the original task note. `csharp-async` may only produce a short note that no async work is expected unless Jira or test infrastructure introduces async code.
- Inspect current serialization and migration patterns before changing `TextData`.
- Inspect property visibility rules before replacing `CycleCharacterColor`.
- Update Jira VIX-2681 with this detailed specification, acceptance criteria, and test plan before code changes.

The ExecPlan should include implementation milestones for:

- data model and migration;
- property exposure and UI visibility;
- rendering behavior for character and word modes, including Mark mode timing preservation;
- automated tests;
- manual validation;
- final Jira update with validation evidence.

## Risks and Open Questions

The existing Text effect already has a `CycleColor` property used in mark-driven and rotate-related paths. The implementation must intentionally merge the mark-driven behavior into the new primary user-facing `Cycle Color` and `Cycle Mode` model instead of treating Mark mode as a separate exception.

Data-contract migration from two Booleans to one Boolean plus one enum needs careful handling. The safest implementation may keep `CycleCharacterColor` as a serialized compatibility field while hiding it from the UI.

Word-level rendering depends on text measurement. Splitting and drawing words separately can affect spacing if implemented naively. Tests and manual validation must check multiple spaces and compare rendered positioning against the non-cycling path.

Some Text rendering paths already split text by spaces for mark collections. The implementation should reuse or consolidate that behavior where practical, and tests must prove that mark collection timing remains unchanged while the color mode changes only gradient assignment.

The original requirements specify splitting by the space character. This deliberately does not include punctuation, tabs, or culture-specific word boundaries.

## Original Source Note

The original request was to improve the Text effect so a user can utilize a gradient per word. The effect currently supports color per character, so the requested enhancement is to change color at word boundaries using spaces.

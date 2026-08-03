# Correct Text Fall and Explode direction-state allocation

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Follow `.agents/PLANS.md` from the repository root when maintaining this document. This plan is for Jira issue VIX-3956.

## Purpose / Big Picture

People using the Text effect in Stacked layout can safely render text containing literal spaces with the Fall or Explode direction. Before this change, a first render frame can throw `ArgumentOutOfRangeException` because the renderer has fewer per-character direction states than it consumes. After this change, sequences such as `A B`, leading or trailing spaces, consecutive spaces, and a space-only row render without an exception and retain their existing visual behavior.

The observable proof is a focused automated test that renders frame zero for Fall and Explode in Stacked layout using each spacing case. Those tests fail on the current allocation rule and pass after the rule is corrected.

## Progress

- [x] (2026-08-02) Investigated the Text effect allocation and drawing paths and confirmed the under-allocation for literal-space entries in Stacked layout.
- [x] (2026-08-02) Authored this implementation-ready ExecPlan.
- [x] (2026-08-02) Updated VIX-3956 with the final scope, acceptance criteria, and test plan before changing code.
- [x] (2026-08-02) Added frame-zero render-level regression tests for Fall and Explode, then captured the expected pre-fix failures for all literal-space cases.
- [x] (2026-08-02) Corrected the private direction-state allocation rule in `Text.cs` and verified the focused frame-zero regression suite passes.
- [x] (2026-08-03) Confirmed focused and full unit-test validation passes and recorded successful manual validation of the affected Text rendering scenario.
- [x] (2026-08-03) Added the final validation comment to VIX-3956.
- [x] (2026-08-03) Completed this document's outcomes, evidence, and revision record after implementation.

## Surprises & Discoveries

- Observation: Direction-state allocation does not occur in `SetupRender()` or a public `PreRender()` entry point; it occurs only when `RenderEffect` handles `frame == 0`.
  Evidence: `src/Vixen.Modules/Effect/Text/Text.cs` creates `_directionClass` in the `if (frame == 0)` block around line 893. A regression test must invoke `RenderEffect(0, frameBuffer)` after `SetupRender()`.

- Observation: The renderer's state consumption is exactly one greater than the source string length, including empty split entries.
  Evidence: `DrawTextWithBrush` uses `text.Split(' ')`, retaining empty tokens, appends one space to every token, then advances `_characterNumber` once per resulting character. For a string with length `L` and `S` literal spaces, it processes `L - S` non-space characters plus `S + 1` appended spaces, or `L + 1` states.

- Observation: The full frame-zero test reaches the intended failing accesses for both directions and every specified literal-space form.
  Evidence: On 2026-08-02, `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~TextDirectionRenderTests` built successfully and ran 10 tests. All 10 failed with `ArgumentOutOfRangeException`: Fall at `Text.cs:1334` and Explode at `Text.cs:1326`.

- Observation: Allocating `text.Length + 1` states resolves every focused literal-space failure without changing the draw algorithm.
  Evidence: On 2026-08-02, the focused `TextDirectionRenderTests` suite passed all 10 frame-zero Fall and Explode cases after the loop bound changed from the non-empty-token formula to `text.Length + 1`.

## Decision Log

- Decision: Use the minimal allocation correction, not a tokenizer refactor or index clamping.
  Rationale: The renderer's current literal-space behavior is established and the defect is only that the allocation code implements a different count. Allocating `text.Length + 1` matches the existing draw loop exactly, is already equivalent to the mark-source allocation branch, and does not alter glyph placement or animation state ownership.
  Date/Author: 2026-08-02 / Codex

- Decision: Test full rendering rather than only private allocation or text-preparation methods.
  Rationale: The failure occurs when `DrawTextWithBrush` indexes `_directionClass`; invoking frame zero proves allocation and consumption agree through the actual rendering path.
  Date/Author: 2026-08-02 / Codex

- Decision: Do not change `PreviewSetElements.cs`, `PixelEffectBase.cs`, serialized data, public APIs, or UI code.
  Rationale: The exception is entirely within Text effect direction-state management. The preview mapper assigns pixels to nodes, and `PixelEffectBase` provides dimensions, neither of which controls `_directionClass` capacity.
  Date/Author: 2026-08-02 / Codex

## Outcomes & Retrospective

VIX-3956 is complete. Stacked Text now assigns each non-mark render entry the same `text.Length + 1` direction-state count consumed by the existing renderer. Fall and Explode no longer run out of private animation state for ordinary, leading, trailing, consecutive, or all-space literal-space text.

The focused `TextDirectionRenderTests` suite passed all 10 cases. The full `Vixen.Tests` unit-test suite and manual validation also passed, as confirmed on 2026-08-03. Jira comment 40272 records these validation results and the two implementation commits. No serialized data, public/protected API, preview mapping, or `PixelEffectBase` behavior changed.

## Context and Orientation

Vixen is a .NET WPF light-show application. Its Text effect is implemented by `src/Vixen.Modules/Effect/Text/Text.cs`, in the `VixenModules.Effect.Text` namespace. `Text` inherits from `PixelEffectBase` and writes colors into an `IPixelFrameBuffer` during rendering. The direction-state list, `_directionClass`, is private state that holds the per-character values used by animated directions. `Fall` stores a falling distance (`Delta`); `Explode` stores the random X and Y motion offsets. These are values for individual drawn characters, so every character consumed by the draw loop must have one list entry.

`TextMode.Rotated` is the enum value displayed in the user interface as Stacked. With `TextSource == TextSource.None`, `PrepareTextLinesForRendering()` calls `SplitTextIntoCharacters(TextLines)`. That turns a visible string such as `A B` into the render entries `A`, ` `, and `B`; it also retains `Environment.NewLine` entries between original text rows. The affected configurations are `Direction == TextDirection.Fall` and `Direction == TextDirection.Explode`; they take the character-by-character draw path even when color cycling is disabled.

During `RenderEffect(0, frameBuffer)`, the Text effect creates `_directionClass`. The `TextSource.None` branch currently adds `text.Length + text.Split(' ', RemoveEmptyEntries).Length` entries for each prepared string. Later, `DrawTextWithBrush()` calls `text.Split(' ')` without removing empty entries and draws every split token after appending one synthetic trailing space. The space-only prepared entry illustrates the mismatch: allocation sees one source character and zero non-empty tokens, so it adds one state; drawing splits it into two empty tokens, converts each to a one-space word, and consumes two states. In a Stacked `A B` example, the prepared entries allocate 2 states for `A`, 1 for the literal space, and 2 for `B`, but draw consumes 2, 2, and 2 respectively. The sixth access is outside the five-item list.

`src/Vixen.Tests/Effects/TextEmptyRowSpacingTests.cs` characterizes text-row preparation, including Stacked splitting, but does not render Fall or Explode. `src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs` demonstrates the repository's test approach for non-public effect lifecycle methods: use reflection to invoke `SetupRender` and `RenderEffect`, and configure `PixelEffectBase`'s private virtual-buffer fields before rendering. `PixelFrameBuffer` in `src/Vixen.Modules/Effect/Effect/PixelFrameBuffer.cs` is the concrete string-oriented frame buffer to use for this regression.

## Plan of Work

### Milestone 1: Bring VIX-3956 to implementation-ready state

Update VIX-3956's description before code changes. Preserve the reported failure context and add a concise explanation that only Stacked Text with literal spaces is affected by the state-count mismatch. Add explicit acceptance criteria: both Fall and Explode must render the first frame without an exception for `A B`, ` A`, `A `, `A  B`, and ` ` when `TextSource` is None and `TextMode` is Rotated. State that no serialized properties, public/protected APIs, preview mapping, or `PixelEffectBase` behavior will change.

Use the configured Atlassian Jira connection if it is available and the executing contributor has permission. If it is unavailable, save the intended description in the implementation notes or report the integration limitation; do not block the code fix.

Acceptance for this milestone is that the ticket gives a reviewer enough information to distinguish the narrow Text rendering defect from preview or layout mapping issues.

### Milestone 2: Establish rendering regressions

Add a new focused test class, `src/Vixen.Tests/Effects/TextDirectionRenderTests.cs`. Keep this class dedicated to rendering behavior; retain `TextEmptyRowSpacingTests.cs` as preparation-only characterization.

Create one data-driven xUnit test for `TextDirection.Fall` and one for `TextDirection.Explode`, or one theory taking each enum value. It must execute the cases `A B`, ` A`, `A `, `A  B`, and ` `. For every case, construct a `Text` instance with `TextSource = TextSource.None`, `TextMode = TextMode.Rotated`, the supplied one-item `TextLines` list, a usable `Font`, non-empty `Colors`, and a nonzero `TimeSpan`. Set the private `_bufferHt` and `_bufferWi` fields declared by `PixelEffectBase` through reflection to a practical rectangular render buffer such as 200 by 100. Follow the existing reflection approach in `SpiralLocationRenderTests.cs`: invoke the non-public `SetupRender()` method, create `new PixelFrameBuffer(width, height)`, then invoke the `RenderEffect(int, IPixelFrameBuffer)` overload with frame `0`.

Wrap that render call in `Record.Exception`. Assert the exception is null, and include the direction and input case in the assertion message or test display name. Do not assert exact pixels: Fall incorporates random motion and the purpose of this regression is preserving the state-count invariant that prevents the thrown exception. Do not use `GenerateVisualRepresentation()` as the sole regression path because it does not prove the `RenderEffect(frame == 0)` allocation path.

Run the new class before making the production change. The expected pre-fix result is an `ArgumentOutOfRangeException` for a case with a literal-space prepared entry, such as `A B`; record this evidence in `Surprises & Discoveries` when executing the plan.

### Milestone 3: Make allocation match the draw algorithm

Edit `src/Vixen.Modules/Effect/Text/Text.cs` in the `frame == 0` direction-state initialization inside `RenderEffect`. In the `TextSource == TextSource.None` branch, replace the loop bound:

    text.Length + text.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries).Length

with the exact number of direction states consumed by a `DrawTextWithBrush(text, ...)` invocation:

    text.Length + 1

Keep the loop's zero-based `i < requiredCount` structure and its existing `CreateDirectionClass()` body. A private `GetDirectionStateCount(string text)` helper returning `text.Length + 1` may be introduced if it is used to make the allocation contract explicit, but do not refactor rendering tokenization as part of this narrowly scoped bug fix. If a helper is introduced, keep it private and update no XML documentation because no public or protected API changes.

Do not add a clamp for `_characterNumber`. A clamp would make distinct characters share state and visibly change Fall or Explode animation. Do not add a per-character capacity-growth guard as the primary fix; exact preallocation is deterministic and avoids a capacity check during every draw. The existing `TextSource != TextSource.None` branch, which uses `i <= txt.Length`, must remain unchanged because it already creates `txt.Length + 1` states.

At the end of this milestone, every prepared string in the non-mark source path has exactly as many states allocated as the current renderer consumes, including strings with no literal spaces, one or many spaces, leading/trailing spaces, a space-only string, and an `Environment.NewLine` separator.

### Milestone 4: Validate, perform a manual smoke test, and close the Jira loop

Run the focused test class and the entire Text test subset. Then run the full test project. A full solution build is useful if the local environment supports the repository's Windows dependencies; use the documented Release rebuild command after tests if it is feasible.

For a manual smoke test, start Vixen from the built output, add a Text effect to any configured element, select Stacked layout, enter `A B`, select Fall and preview or render it, then repeat with Explode. Repeat with a leading space, trailing space, consecutive spaces, and a single space. The effect should animate rather than display an unhandled exception. Normal layout and Text effects using mark collections should still render as before.

Finally, update VIX-3956 to match any final naming or test changes and add a comment containing the test commands and their passed results. If the Jira connection was unavailable, state that plainly in the handoff rather than fabricating an update.

## Concrete Steps

All commands below run from `C:\Dev\Vixen` in PowerShell.

1. Inspect the exact current code and nearby tests before editing:

       rg -n -C 10 "_directionClass|DrawTextWithBrush|PrepareTextLinesForRendering" src/Vixen.Modules/Effect/Text/Text.cs
       Get-Content src/Vixen.Tests/Effects/SpiralLocationRenderTests.cs

2. Update VIX-3956 through the configured Jira client with the acceptance criteria described in Milestone 1. Confirm the ticket still identifies the issue key and scope before moving on.

3. Create `src/Vixen.Tests/Effects/TextDirectionRenderTests.cs` with the frame-zero tests described in Milestone 2. Run the targeted suite before the production edit:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~TextDirectionRenderTests

   Before the fix, expect the relevant test to fail with an inner `ArgumentOutOfRangeException` originating at the Fall or Explode `_directionClass[_characterNumber]` access. Reflection may wrap the exception in `TargetInvocationException`; assert or report the inner exception when diagnosing the pre-fix failure.

4. Edit only `src/Vixen.Modules/Effect/Text/Text.cs` as specified in Milestone 3. Review the diff to ensure the allocation loop is the only production change:

       git diff -- src/Vixen.Modules/Effect/Text/Text.cs src/Vixen.Tests/Effects/TextDirectionRenderTests.cs

5. Run focused verification:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~TextDirectionRenderTests
       dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~Text

   Expect every selected xUnit test to pass and no `ArgumentOutOfRangeException`.

6. Run broad verification:

       dotnet test src/Vixen.Tests/Vixen.Tests.csproj
       msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release

   Expect a zero exit code. If the build fails because of an existing local dependency or unrelated failure, preserve the error output, verify the focused tests still pass, and document the limitation in the Jira comment and this plan.

7. Perform the manual smoke test in Milestone 4 if the application can be launched locally. Then add the Jira validation comment, update this plan's living sections and revision note, and report the final results without committing unless explicitly requested.

## Validation and Acceptance

The change is accepted when all of the following are true:

- Rendering frame zero for a Stacked (`TextMode.Rotated`) Text effect with `TextSource.None` and `Direction.Fall` completes without an exception for `A B`, ` A`, `A `, `A  B`, and ` `.
- The same five inputs complete without an exception when `Direction.Explode` is used.
- The focused regression tests fail before the allocation correction for a literal-space scenario and pass after it.
- Existing Text-focused tests and the complete `Vixen.Tests` project pass, subject only to documented pre-existing environment failures.
- A manual Vixen preview or render, when available, shows Fall and Explode animating the test inputs without crashing.
- The production diff contains no changes to `TextData`, public/protected contracts, `PreviewSetElements.cs`, `PixelEffectBase.cs`, or serialization.

## Idempotence and Recovery

The test additions, allocation correction, and test commands are safe to repeat. The production fix is a deterministic one-line loop-bound change; rerunning it must not create duplicate state or persistent data. Do not edit existing sequences or user profiles during manual verification.

If a render test fails due to reflection setup rather than the target exception, compare its helper methods with `SpiralLocationRenderTests.cs`, verify `SetupRender()` is called before `RenderEffect(0, ...)`, and verify the buffer fields are set on `PixelEffectBase`. If the full suite or Release build is blocked by unrelated local setup, do not weaken or remove the focused regression; document the blocked command and preserve its output.

To revert the implementation before a commit, restore only the two planned files through the repository's normal source-control UI or a targeted version-control operation. Do not use a broad reset that could discard unrelated work in the shared working tree.

## Artifacts and Notes

The essential production change should be equivalent to this narrow diff in `Text.cs`:

    foreach (var text in _text)
    {
        for (var i = 0; i < text.Length + 1; i++)
        {
            CreateDirectionClass();
        }
    }

The invariant protected by the tests is:

    DirectionStatesAllocated(text) == DirectionStatesConsumedByDrawTextWithBrush(text) == text.Length + 1

For the Stacked input `A B`, text preparation produces `A`, ` `, and `B`. Allocation must create 2 + 2 + 2 = 6 states, matching draw consumption. The old formula creates 2 + 1 + 2 = 5 states and fails on index 5.

The initial Jira description update should include this concise acceptance statement:

    In Stacked Text with TextSource None, Fall and Explode render the first frame without an exception for ordinary, leading, trailing, consecutive, and all-space literal-space inputs. No serialized data, public API, preview mapping, or PixelEffectBase behavior changes.

The final Jira comment should identify the exact commands run, their outcomes, whether manual verification ran, and any environment-limited command. It should not claim a Jira update, test result, or manual verification that did not occur.

Milestone 1 Jira update:

    VIX-3956 description updated on 2026-08-02. It contains the reported failure, root cause, intended one-line allocation correction, acceptance criteria for Fall and Explode, scope exclusions, and the focused/full test commands.

Milestone 2 baseline:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~TextDirectionRenderTests
    Failed: 10, Passed: 0, Skipped: 0, Total: 10

    Fall cases fail at Text.cs:1334 and Explode cases fail at Text.cs:1326 with ArgumentOutOfRangeException while indexing _directionClass. The test project compiled successfully; existing restore and build warnings were unrelated to this change.

Milestone 2 suggested commit message:

    VIX-3956 Add Text direction render regressions

    Stacked Text entries containing literal spaces exhaust private direction
    state for Fall and Explode. Add frame-zero coverage so the failure is
    reproducible for all affected spacing forms.

    Related to VIX-3956

Milestone 2 commit:

    db7e8b2ae VIX-3956 Add Text direction render regressions

Milestone 3 validation:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~TextDirectionRenderTests
    Passed: 10, Failed: 0, Skipped: 0, Total: 10

Milestone 3 suggested commit message:

    VIX-3956 Fix Text direction state allocation

    Stacked literal spaces can make Fall and Explode consume more animation
    state than they allocate. Match allocation to the renderer's
    length-plus-one state count.

    Related to VIX-3956

Milestone 3 commit:

    9dfb6a728 VIX-3956 Fix Text direction state allocation

Milestone 4 validation and Jira evidence:

    Focused TextDirectionRenderTests: 10 passed, 0 failed.
    Full src/Vixen.Tests/Vixen.Tests.csproj unit-test suite: passed.
    Manual validation: passed.
    VIX-3956 final validation comment: 40272, posted 2026-08-03.

Milestone 4 suggested commit message:

    VIX-3956 Record validation results

    Capture focused, full-suite, and manual validation evidence in the
    ExecPlan and Jira ticket for the completed Text rendering fix.

    Related to VIX-3956

## Interfaces and Dependencies

No new interfaces, NuGet packages, serialized fields, or public/protected members are required. The implementation uses the existing private members of `VixenModules.Effect.Text.Text`:

- `_text`, the prepared render strings.
- `_directionClass`, the private `List<DirectionClass>` used by Fall and Explode.
- `CreateDirectionClass()`, which appends one initialized state.
- `DrawTextWithBrush(...)`, which advances `_characterNumber` once per consumed state.

The test project already references the Text effect module, xUnit, `VixenModules.Effect.Effect.PixelFrameBuffer`, and reflection support. Reuse those dependencies rather than adding a framework or changing test-project references.

Revision note (2026-08-02): Created the initial ExecPlan from direct inspection of the Text allocation and draw paths. It records the frame-zero rendering requirement so the regression exercises the actual failure site.

Revision note (2026-08-02): Completed Milestone 1 by updating VIX-3956 through the configured Atlassian connection, then recorded the outcome in Progress, Outcomes & Retrospective, and Artifacts and Notes. No source or test implementation was performed.

Revision note (2026-08-02): Completed Milestone 2 by adding `src/Vixen.Tests/Effects/TextDirectionRenderTests.cs`. The data-driven frame-zero test covers Fall and Explode with ordinary, leading, trailing, consecutive, and all-space literal-space inputs. The focused suite builds and records the expected ten pre-fix failures.

Revision note (2026-08-02): Added the required Milestone 2 formatted commit-message handoff after it was omitted from the original completion response. The test-only milestone was subsequently committed as `db7e8b2ae`.

Revision note (2026-08-02): Completed Milestone 3 by changing the non-mark Text direction-state allocation bound to `text.Length + 1`. The focused render suite passes all 10 Fall and Explode literal-space cases. No public/protected API, serialized data, preview mapping, or `PixelEffectBase` code changed.

Revision note (2026-08-03): Completed Milestone 4 after confirmation that the full unit-test suite and manual validation passed. Added Jira comment 40272 with the final validation evidence and implementation commit identifiers, then updated Progress, Outcomes & Retrospective, and Artifacts and Notes. No implementation code changed in this milestone.

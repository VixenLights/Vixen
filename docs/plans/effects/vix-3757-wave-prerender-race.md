# Serialize effect pre-rendering to prevent Wave render-state races

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain it in accordance with `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

VIX-3757 prevents intermittent failures when Vixen renders a Wave effect. A user with Wave effects on props such as Spinner, Spinner - 2, Spinner - 3, and Spinner - 4 can render or preview a sequence without seeing `Collection was modified; enumeration operation may not execute` or the accompanying `NullReferenceException` in the log. The visible Wave output must remain unchanged when rendering occurs normally.

The fix makes an effect instance's pre-render operation exclusive: when two callers request rendering of the same dirty effect simultaneously, exactly one caller performs the render and the other waits for it to finish. This is observable through automated concurrent-call coverage and by opening a sequence with the reported Wave layout, rendering it repeatedly, and observing no Wave render errors.

## Progress

- [x] (2026-09-04 00:00Z) Investigated the VIX-3757 stack trace and created this implementation-only ExecPlan. No production or test code has been changed.
- [x] (2026-09-04 18:53Z) Completed Milestone 1: updated Jira issue VIX-3757 with the user-facing summary, scope, and acceptance criteria; it remains In Progress. Evidence: https://vixenlights.atlassian.net/browse/VIX-3757 (updated 2026-09-04 13:53:32.514-05:00).
- [x] (2026-09-04 19:01Z) Completed Milestone 2: added `EffectModuleInstanceBasePreRenderTests` with coordinated concurrent callers, bounded waits, exclusive-execution assertions, and cancellation-token forwarding coverage. `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` succeeded. The pre-fix focused `dotnet test` run failed deterministically in 57 ms because `PreRender` calls `_PreRender()` without the supplied token (expected `CancellationTokenSource` vs. actual `null`). In that run the 16 coordinated callers produced one `_PreRender` entry, so the test does not claim to have forced the narrower existing check-and-set interleaving without adding a production test seam.
- [x] (2026-09-04 19:06Z) Completed Milestone 3: replaced the non-atomic `IsRendering` guard and busy-spin loop with a private per-instance monitor gate in `EffectModuleInstanceBase.PreRender`, forwarded the supplied cancellation token to `_PreRender`, and documented the concurrent-call contract in both `EffectModuleInstanceBase` and `IEffect`. The Release/x64 `Vixen_Tests` build succeeded, and the focused regression test passed: 1 passed, 0 failed in 28 ms.
- [x] (2026-09-04 19:18Z) Completed Milestone 4: the full build succeeded and all 901 tests passed. Manual testing with rapid effect changes across several effects produced no errors and all tested effects rendered as expected. Jira VIX-3757 received the user-facing completion comment: https://vixenlights.atlassian.net/browse/VIX-3757?focusedCommentId=40433. The issue remains In Progress (updated 2026-09-04 14:18:39.864-05:00).

## Surprises & Discoveries

- Observation: the current `PreRender` guard is a non-atomic check followed by an assignment.
  Evidence: `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` tests `IsDirty && !IsRendering` and only then assigns `IsRendering = true`. Two threads can both pass the test before either assignment occurs.

- Observation: Wave rendering keeps mutable, instance-owned state that is not safe to mutate concurrently.
  Evidence: `src/Vixen.Modules/Effect/Wave/Wave/Wave.cs` clears, enqueues, dequeues, rotates, and snapshots `IWaveform.Pixels`; `RenderColumns` snapshots it through `wave.Pixels.ToList()`. The reported exception originates from the `Queue<T>` enumerator used by that snapshot while another path modifies the queue.

- Observation: the trace contains both `SetupRender` and `RenderEffect` paths at the same timestamp.
  Evidence: the supplied log records `GrowAndShrink` reached from `SetupRender` as well as from `RenderEffect` for related Spinner targets. In one normal `PixelEffectBase._PreRender` pass, setup completes before frame rendering, so overlapping passes on the same effect state are the relevant failure mode.

- Observation: the guard was intended to prevent this exact category of problem, but it spin-waits rather than synchronizing entry.
  Evidence: the `else` branch in `EffectModuleInstanceBase.PreRender` comments that it prevents multiple threads in the editor pre-render process, then loops over `IsRendering` with `Thread.Sleep(1)`.

- Observation: the new concurrent regression scenario did not reproduce two `_PreRender` entries in its first pre-fix execution.
  Evidence: the coordinated 16-caller run reached the exclusive-execution assertions with `PreRenderCount == 1` and `MaximumConcurrentPreRenders == 1`, then failed at the deterministic cancellation-token assertion because the derived method received `null`. The source-level non-atomic guard remains the root cause identified by the Wave trace; forcing the instruction-level check-and-set race would require a production test seam, which is intentionally out of scope for this test-only milestone.

## Decision Log

- Decision: fix synchronization in `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs`, rather than adding a lock around `Waveform.Pixels` in `Wave.cs`.
  Rationale: `Pixels` is only one part of Wave's render state. Wave also mutates frame counters, waveform counters, window positions, shrink state, dimensions, and Fractal Ivey state during setup and frame rendering. A queue-only lock would leave invalid interleavings and protects no other stateful effect from the same base-class race.
  Date/Author: 2026-09-04 / Codex

- Decision: use a private, per-instance monitor lock (`lock` on a dedicated readonly object) around the complete `PreRender` decision and execution lifecycle.
  Rationale: the method is synchronous. A monitor lock atomically decides whether a dirty effect needs rendering, serializes all access to that effect's mutable render state, and waits without a polling loop. A static lock would unnecessarily serialize unrelated effect instances and reduce parallel sequence rendering.
  Date/Author: 2026-09-04 / Codex

- Decision: do not change Wave's serialized settings, Wave output algorithm, or queue type for VIX-3757.
  Rationale: the reported failure is caused by overlapping lifecycle executions, not a malformed Wave configuration. Preserving Wave's existing output limits compatibility risk.
  Date/Author: 2026-09-04 / Codex

- Decision: preserve the existing caller contract that a caller which waits for another render does not itself re-render the now-clean effect.
  Rationale: Vixen invokes `PreRender` from editor and parallel sequence paths. Once the lock holder successfully sets `IsDirty` to false, a waiting caller should return successfully without duplicate work. If the holder fails, `IsDirty` remains true and a waiting caller may retry, which matches the current practical behavior.
  Date/Author: 2026-09-04 / Codex

## Outcomes & Retrospective

All milestones are complete. Jira VIX-3757 describes the user-facing rendering reliability outcome, preserves the original-report context, states that Wave visuals and saved settings remain unchanged, and contains reviewable acceptance criteria. Its completion comment is [comment 40433](https://vixenlights.atlassian.net/browse/VIX-3757?focusedCommentId=40433); the issue remains In Progress for the project workflow to transition. `EffectModuleInstanceBase.PreRender` now serializes the dirty-state decision and derived pre-render lifecycle per effect instance without a polling loop, and forwards the supplied cancellation token. The focused regression test passes with exactly one derived pre-render call, both callers completing successfully, the effect clean afterward, and the same token source received by the derived method.

No Wave algorithm or serialized setting changed. The correction is intentionally shared because `EffectModuleInstanceBase.PreRender` owns the faulty concurrency boundary. The Release/x64 full build succeeded, all 901 tests passed, and manual rapid changes across several effects produced no errors while effects rendered normally. This satisfies the automated and manual acceptance evidence available in the workspace.

Residual limitation: the original Spinner sequence/profile was not identified in the workspace, so manual validation used rapid changes across several available effects rather than that exact saved layout. No Wave or general effect-rendering errors were observed.

## Context and Orientation

Vixen is a Windows WPF desktop application that renders lighting effects into time-based color intents. An effect instance is one configured effect placed on one or more element nodes in a sequence. Before Vixen can provide its output, it calls `PreRender`, which calculates all frames and stores the resulting intents. More than one editor or sequence worker can request that work at nearly the same time.

`src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` is the shared base class for effect instances. Its public `PreRender(CancellationTokenSource cancellationToken = null)` method decides whether work is dirty, calls the derived `_PreRender` implementation, catches and logs exceptions, and resets the rendering state. It currently uses a private Boolean named `IsRendering` as a guard. Reading the Boolean and later setting it are two independent operations, which leaves a race window.

`src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs` supplies the normal pixel-effect lifecycle. Its `_PreRender` configures display size, calls the derived `SetupRender`, then renders frames through `RenderEffect`. Wave derives through this path.

`src/Vixen.Modules/Effect/Wave/Wave/Wave.cs` is the Wave effect implementation named in the VIX-3757 stack trace. `SetupRender` resets each waveform and can prime it. `RenderEffect` advances each waveform over frames. `GrowAndShrink` and `RenderColumns` mutate and enumerate `wave.Pixels`, a `Queue<List<Tuple<Color, int>>>` owned by `Waveform`. A queue is a first-in, first-out collection and is not safe for one thread to enumerate while another thread changes it. `RenderColumns` calls `wave.Pixels.ToList()`, which is the operation reported by the `InvalidOperationException`.

The relevant tests belong in `src/Vixen.Tests/Effects/`. The test project is `src/Vixen.Tests/Vixen.Tests.csproj`; it already references `src/Vixen.Core/Vixen.Core.csproj`. Create a focused test class named `EffectModuleInstanceBasePreRenderTests.cs`. It may define a private test-only derived effect in the same file because that helper is not production code. The helper must implement the abstract members of `EffectModuleInstanceBase` and deliberately hold `_PreRender` until two callers have attempted `PreRender`.

The solution includes C++/CLI dependencies, so ordinary `dotnet test` must not build the test project from source. Build the `Vixen_Tests` target using full MSBuild first, then run the built tests with `--no-build`, as described in the repository root `AGENTS.md`.

## Plan of Work

First update VIX-3757 in Jira before changing code. Use the project `jira` skill in `.agents/skills/jira/SKILL.md`. Keep the Jira description user-facing: Wave effects intermittently fail during concurrent rendering; the change prevents duplicate simultaneous rendering of an effect; Wave appearance and saved settings are unchanged; automated tests prove one render runs for simultaneous requests; manual validation renders the affected sequence repeatedly without errors. Do not place source paths, implementation alternatives, or lock names in the Jira description.

Next, characterize the base-class race with a deterministic test. Do not write a timing-only test that merely starts two tasks and hopes for an overlap. The test effect's `_PreRender` must signal when it has started, remain blocked on a test-controlled synchronization primitive, and count executions. Start a first `PreRender` task, wait until its `_PreRender` has entered, then start a second `PreRender` task while the first is still blocked. Release the first and await both callers. The final regression assertions are that `_PreRender` executed once, the maximum number of simultaneous `_PreRender` invocations was one, no exception escaped, both calls report success, and the effect is no longer dirty. The test must have bounded waits and clean up all synchronization primitives so a failure cannot hang the test run.

With the test in place, replace the `IsDirty && !IsRendering` check, `IsRendering` assignments, and `Thread.Sleep(1)` loop in `EffectModuleInstanceBase.PreRender` with a private readonly lock object. Acquire it before reading `IsDirty`; if the instance is no longer dirty after waiting, return success immediately. Otherwise retain the existing try/catch logging behavior while calling `_PreRender(cancellationToken)`, set `IsDirty = false` only after successful completion, and return the result from inside the protected section. Do not invoke arbitrary callbacks while holding a static/shared lock; the lock must be a field on each effect instance.

Pass the existing `cancellationToken` parameter through to `_PreRender`. The current implementation calls `_PreRender()` without the argument even though the method receives one. This plan includes passing it through because it is a behavior-preserving correction at the same lifecycle boundary; add a focused assertion in the test helper that the supplied token source reaches `_PreRender` if the test can do so without complicating the concurrency assertion.

Modify the XML documentation for the public `PreRender` API in both `EffectModuleInstanceBase` and `IEffect` to state that simultaneous calls for the same effect instance are serialized and that a call returns without rendering when another successful caller has already made it clean. Use `.agents/skills/csharp-docs/SKILL.md` before modifying these public APIs, as required by the repository instructions. Do not expose the lock, `IsRendering`, or another synchronization type as part of an interface.

Do not change `Wave.cs` unless the focused test or a debugger trace proves an independent Wave-only source of concurrent mutation remains after base pre-render serialization. If that happens, stop and record the evidence in `Surprises & Discoveries`; a Wave-specific fix requires updating this plan before expanding scope.

## Milestones

### Milestone 1: Align VIX-3757 with the final user outcome

Update Jira issue VIX-3757 before repository changes. The issue description must say that Wave effects can intermittently fail while a sequence is rendered concurrently, that users should be able to render affected sequences reliably, and that existing Wave visuals and saved effect data remain unchanged. Include concise Given/When/Then acceptance criteria for simultaneous render requests and repeat rendering of the reported Spinner scenario. Add the planned automated and manual validation in plain language. At the end of this milestone, the tracker is sufficient for a user or reviewer to understand the intended outcome without source-level details.

### Milestone 2: Establish a deterministic regression test

Add `src/Vixen.Tests/Effects/EffectModuleInstanceBasePreRenderTests.cs`. Define a minimal private derived effect that counts `_PreRender` entry and uses test-controlled events to hold a render in progress. The test must prove the intended exclusive lifecycle with two concurrent `PreRender` callers. Run the focused test after adding it. Before the production correction, expect it to fail reliably by observing two `_PreRender` entries, or use a controlled test seam only if necessary to make the existing check-and-set interleaving reproducible. Record the precise pre-fix result in this plan.

At the end of this milestone, the test demonstrates the base lifecycle defect rather than a Wave rendering artifact. It is independently verifiable because it does not require a display, profile, or timing-sensitive queue enumeration.

### Milestone 3: Serialize effect-instance pre-rendering

Modify `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` to use one private readonly per-instance monitor object for the complete `PreRender` decision and render lifecycle. Remove the `IsRendering` Boolean and busy-spin loop. Preserve exception logging, leave an effect dirty after a failed `_PreRender`, and only clear it after a successful completion. Forward the supplied cancellation token to `_PreRender`.

Update XML documentation in `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` and `src/Vixen.Core/Module/Effect/IEffect.cs` according to the `csharp-docs` skill. Run the focused regression test and demonstrate that it now passes: exactly one `_PreRender` invocation occurs and both callers complete.

### Milestone 4: Validate Wave behavior and close out the tracker

Run the full MSBuild test workflow, then the focused pre-render test and broader test suite. Manually load the sequence/profile that produced the VIX-3757 Spinner Wave errors, or construct an equivalent Wave effect on at least four Spinner-like targets. Trigger repeated preview, render, or export activity that causes concurrent pre-render requests. Confirm the log contains no `Error rendering Wave`, `Collection was modified`, or related `NullReferenceException` entries, and visually confirm Wave still animates.

Update VIX-3757 with a concise user-facing completion comment that reports the validation result. Revise `Progress`, `Surprises & Discoveries`, `Outcomes & Retrospective`, and the revision note at the bottom of this plan with exact commands and results. When a milestone changes repository files, generate the proposed commit message with the project `commit-msg` skill; do not create a commit unless explicitly requested.

## Concrete Steps

Run all commands from `C:\Dev\Vixen` in PowerShell.

Inspect the worktree before edits and preserve unrelated changes:

    git status --short

Read the complete implementation context before editing:

    Get-Content -LiteralPath .agents\skills\csharp-docs\SKILL.md
    Get-Content -LiteralPath src\Vixen.Core\Module\Effect\IEffect.cs
    Get-Content -LiteralPath src\Vixen.Core\Module\Effect\EffectModuleInstanceBase.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Effect\PixelEffectBase.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Wave\Wave\Wave.cs
    Get-Content -LiteralPath src\Vixen.Modules\Effect\Wave\Wave\Waveform.cs

Use the `jira` skill to update VIX-3757 with this user-facing content:

    ## Summary

    Prevent intermittent Wave-effect rendering errors when the same effect is requested simultaneously during sequence rendering.

    ## Scope

    - Ensure only one render of an individual effect runs at a time.
    - Keep existing Wave visuals and saved settings unchanged.
    - Preserve normal sequence preview, rendering, and export behavior.

    ## Acceptance Criteria

    - Given two simultaneous requests to render the same changed effect, when rendering runs, then it completes once without an error.
    - Given a sequence with Wave effects on multiple Spinner targets, when it is rendered repeatedly, then no Wave rendering exceptions are logged.
    - Given an existing Wave effect, when it is rendered after the change, then its animation and saved settings remain unchanged.

Create the focused test file, then build the test graph with full MSBuild:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Run only the new test without rebuilding:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)\" --filter "FullyQualifiedName~EffectModuleInstanceBasePreRenderTests"

Before Milestone 3, record the deterministic expected failure. After Milestone 3, expect output of this form:

    Passed!  - Failed:     0, Passed:     <count>, Skipped:     0, Total:     <count>

After focused success, rerun the full build because the changed base class is shared across effects:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Then run the already-built suite:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)\"

Do not use `dotnet test` without `--no-build` for this repository's test graph: its C++/CLI dependencies require full MSBuild and the Visual C++ toolset.

## Validation and Acceptance

VIX-3757 is accepted when the focused test proves that two overlapping `PreRender` calls for one dirty effect cause exactly one execution of its derived `_PreRender` method, complete without exceptions, and leave the effect clean after success. The same test must prove that the cancellation token source passed to `PreRender` reaches the derived method if that behavior is covered.

The Release/x64 full-MSBuild test target must succeed, followed by the already-built Vixen test suite with zero failures. If any unrelated failure prevents the full suite from completing, capture its complete command, failing test name, and output in `Artifacts and Notes`; rerun the focused test successfully and document the limitation in Jira rather than suppressing the new regression test.

Manual acceptance requires rendering the actual affected sequence if it is available. If it is unavailable, use a comparable sequence with a Wave effect on four Spinner-like targets and repeatedly trigger editor preview plus render/export activity. The log must not contain `Error rendering Wave`, `Collection was modified; enumeration operation may not execute`, or the accompanying Wave `NullReferenceException`. The wave must visibly animate, demonstrating the lock has not skipped normal output.

## Idempotence and Recovery

The Jira update, test creation, and source changes are safe to rerun. The monitor lock is local to an effect instance and does not alter sequence data, profile data, or serialized Wave data.

If the focused test hangs, terminate the test run, inspect whether the test helper always releases its blocking event in a `finally` block, and fix the test harness before changing production code. Do not increase arbitrary timeouts to hide a deadlock. If the base-class lock reveals a re-entrant call to `PreRender` on the same instance, record the call stack and decide deliberately whether same-thread re-entry is valid; a C# monitor is re-entrant, so this plan does not introduce a self-deadlock by itself.

If Wave errors persist after the base fix, retain the passing base regression test and collect a debugger trace showing the distinct writer. Update this plan before adding Wave-specific synchronization; do not apply a broad lock around individual `Queue` operations because that does not protect Wave's surrounding state transition.

## Artifacts and Notes

The relevant current code pattern, to be removed from `EffectModuleInstanceBase.PreRender`, is conceptually:

    if (IsDirty && !IsRendering)
    {
        IsRendering = true;
        _PreRender();
        IsDirty = false;
        IsRendering = false;
    }

This is unsafe because checking `IsRendering` and setting it are separate operations. The implementation must instead guard the check, `_PreRender`, and dirty-state update as one exclusive operation for one effect instance.

The Wave stack trace's strongest evidence is not the reported source line number, which can vary by build and debug symbols. It is the framework portion:

    System.Collections.Generic.Queue`1.Enumerator.MoveNext()
    System.Collections.Generic.List`1..ctor(IEnumerable`1 collection)
    VixenModules.Effect.Wave.Wave.GrowAndShrink(...)

That sequence corresponds to `wave.Pixels.ToList()` in the current `RenderColumns` implementation. It proves a queue mutation overlapped its enumeration.

## Interfaces and Dependencies

No new external package, project, module, descriptor parameter, serialized Wave setting, or public synchronization type is needed.

At the end of implementation, `IEffect.PreRender` and `EffectModuleInstanceBase.PreRender` retain this signature:

    bool PreRender(CancellationTokenSource cancellationToken = null)

Their documented contract must additionally state that concurrent calls for the same effect instance are serialized. The derived protected contract remains:

    protected abstract void _PreRender(CancellationTokenSource cancellationToken = null)

`EffectModuleInstanceBase` must own a private readonly synchronization object. It must not expose that object through `IEffect`, store it statically, or use it to synchronize different effect instances. Derived effects, including `VixenModules.Effect.Wave.Wave`, require no API or serialized-data changes for this ticket.

---

Plan created 2026-09-04 / Codex. Reason: VIX-3757's Wave stack trace proves concurrent mutation of Wave render state, and source inspection identifies the non-atomic pre-render guard in the shared effect base class as the lifecycle race to correct.

Plan revised 2026-09-04 / Codex. Reason: Milestone 1 updated VIX-3757 with the final user-facing summary, scope, and acceptance criteria before repository implementation begins.

Plan revised 2026-09-04 / Codex. Reason: Milestone 2 added bounded concurrent pre-render regression coverage and recorded the exact pre-fix failure and the observed limitation of forcing the legacy check-and-set race without a production test seam.

Plan revised 2026-09-04 / Codex. Reason: The Milestone 2 test now passes its synchronization resources as explicit static-delegate state, removing Rider's captured-variable-disposed-in-outer-scope warnings without changing its assertions or expected pre-fix result.

Plan revised 2026-09-04 / Codex. Reason: Milestone 3 implemented per-instance pre-render serialization, cancellation-token forwarding, and the documented concurrent-call contract; the focused regression test now passes.

Plan revised 2026-09-04 / Codex. Reason: Milestone 4 recorded the successful full build, all 901 passing tests, successful manual rapid-effect-change testing, and Jira completion comment 40433.

# VIX-3991: Prevent a disposed looping sequence from restarting

This ExecPlan is a living document. Maintain its `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections as work proceeds. Follow `.agents/PLANS.md` from the repository root when revising this document.

## Purpose / Big Picture

When a looping sequence reaches its end at the same time that its playback context is stopped, released, or disposed, Vixen can terminate with a fatal `ArgumentNullException`. After this work, stopping or closing a looping sequence will reliably prevent any already-queued restart from touching its disposed playback resources. Users can verify the change by stopping or closing a looping sequence at its end without a crash, an unexpected restart, or continued media playback.

VIX-3991 concerns only the shared sequence-executor lifecycle. It must preserve ordinary playback, looping, pause/resume behavior, `SequenceStarted`, `SequenceReStarted`, and `SequenceEnded` notifications for a live executor. It must not change sequence file data, timing-module contracts, or introduce a new UI implementation.

## Progress

- [x] (2026-08-23 20:00Z) Investigated the reported stack trace, `SequenceExecutor`, `SequenceContext`, `ProgramExecutor`, and `HighResolutionTimer`; identified a queued loop-restart callback racing executor disposal.
- [x] (2026-08-23 20:00Z) Read VIX-3991 and created this implementation plan without modifying production or test code.
- [x] (2026-08-23 14:06Z) Updated VIX-3991 with the user-facing requirements, scope, acceptance criteria, and validation approach; added a progress comment.
- [ ] Add deterministic executor lifecycle tests that reproduce a queued restart becoming stale.
- [ ] Implement restart invalidation and disposal-safe timer synchronization in `SequenceExecutor`.
- [ ] Verify focused and full x64 test runs, then manually exercise the close/stop-at-loop-boundary scenario.
- [ ] Align VIX-3991 with delivered behavior and add the final validation comment.

## Surprises & Discoveries

- Observation: The exact failing statement is `lock (_endCheckTimer)` in `BaseSequence.SequenceExecutor._loopPlay`.
  Evidence: The report identifies `src/Vixen.Common/BaseSequence/SequenceExecutor.cs:line 216`; `lock` throws `ArgumentNullException` when its lock expression is null.

- Observation: `SequenceExecutor.Dispose(bool)` detaches the timer event and assigns `_endCheckTimer = null`, while `_CheckForNaturalEnd` posts `_loopPlay` asynchronously to the synchronization context.
  Evidence: `SequenceExecutor.cs` lines 386 and 411-423. The report's `System.Windows.Forms.Control.InvokeMarshaledCallbacks` frames prove the callback was queued to the WinForms UI message queue before it ran.

- Observation: Stopping the timer cannot retract a callback already posted to the UI queue.
  Evidence: `_CheckForNaturalEnd` calls `_syncContext.Post`, which has no cancellation handle. The current `_loopPlay` has no `IsRunning`, `_loop`, or disposal check before restarting timing and media state.

- Observation: The timer field is also read and locked by `_EndCheckTimerElapsed`, so fixing only `_loopPlay` would leave another disposal-time null race.
  Evidence: `SequenceExecutor.cs` lines 362-376 read `_endCheckTimer.IsRunning` and then lock `_endCheckTimer`.

- Observation: `SequenceContext.Dispose(bool)` disposes its sequence executor before `ContextBase.Dispose(bool)` has an opportunity to stop a running context.
  Evidence: `src/Vixen.Core/Execution/Context/SequenceContext.cs` lines 226-237 and `ContextBase.cs` lines 186-197. The normal `ContextManager` release route stops the context first, but direct disposal still needs a safe executor.

## Decision Log

- Decision: Solve the stale queued-work problem with an execution-generation token (a monotonically increasing value identifying one active play/loop run), not with only a null guard.
  Rationale: A null guard avoids the immediate exception but leaves a stale callback capable of restarting a sequence after the user stopped it. A captured generation value lets the UI callback prove that it still belongs to the current live loop before it changes timing, media, or events.
  Date/Author: 2026-08-23 / Codex

- Decision: Use a dedicated private, readonly synchronization object for executor timer state rather than the mutable `_endCheckTimer` reference.
  Rationale: A lock target must remain valid for the lifetime of the executor. The timer reference is deliberately cleared during disposal, so it is not a valid synchronization object.
  Date/Author: 2026-08-23 / Codex

- Decision: Make `Dispose` stop and invalidate a running executor before releasing timer-related references; each timer callback and UI-posted callback must harmlessly no-op after invalidation.
  Rationale: Disposal is a terminal lifecycle operation and may race with the background high-resolution timer and the UI queue. The executor itself must be safe even when its caller did not explicitly call `Stop` first.
  Date/Author: 2026-08-23 / Codex

- Decision: Keep the change local to the executor unless tests prove a context-disposal ordering correction is needed for consistency.
  Rationale: `SequenceExecutor` is used both by sequence contexts and by `ProgramExecutor`; it is the common fault boundary. Correcting it there protects all callers and avoids relying on every owner to get cleanup order right.
  Date/Author: 2026-08-23 / Codex

## Outcomes & Retrospective

Milestone 1 is complete. VIX-3991 now defines the user-visible loop-boundary behavior and the required automated and manual validation before code changes begin. The intended implementation result remains a sequence executor whose posted loop restart is valid only for its originating active loop and whose disposal is safe while timer or UI work is in flight. Update this section with actual test counts, manual observations, remaining gaps, and the final VIX-3991 status when work is complete.

## Context and Orientation

`src/Vixen.Common/BaseSequence/SequenceExecutor.cs` is the shared playback implementation used by the Timed and Vixen 2.x sequence modules. It owns an `ITiming` source, media startup and shutdown, playback range state, and a `HighResolutionTimer` named `_endCheckTimer`. The timer runs its `Elapsed` event on a background thread every ten milliseconds to determine whether the timing position has reached the requested end time.

For normal non-loop playback, `_CheckForNaturalEnd` posts `_Stop` to the synchronization context captured in the executor constructor. For loop playback, it posts `_loopPlay`. A synchronization context is the mechanism that transfers work from one thread to another; in the application it is the WinForms UI message queue. Posting work is asynchronous: the timer thread returns immediately, and the UI may execute the callback later. Consequently, a queued restart can exist after `Stop` or `Dispose` has completed.

`_loopPlay` currently stops the timer, sets `TimingSource.Position` to `StartTime`, starts timing, raises `SequenceReStarted`, waits for movement, and starts the timer again. It locks on `_endCheckTimer`. `Dispose(bool)` removes the elapsed handler and clears both `_endCheckTimer` and `_syncContext`. This causes the reported failure if the UI dispatches a restart that was posted before disposal.

`src/Vixen.Core/Execution/Context/SequenceContext.cs` creates the association between an execution context and an `ISequenceExecutor`, forwards start/pause/resume/stop calls, and disposes an executor when a context is replaced or disposed. `src/Vixen.Core/Execution/ProgramExecutor.cs` independently owns and disposes sequence executors for queued program playback. Neither owner can cancel a callback already placed on the synchronization context, so `SequenceExecutor` must reject stale callbacks itself.

`src/Vixen.Core/Utility/HighResolutionTimer.cs` is a reusable, non-overlapping background timer. `Stop(false)` stops its loop but intentionally does not join its thread. Its event can therefore be at or near the dispatch boundary when executor state is being cleaned up. Do not alter this general-purpose utility for this bug unless investigation finds a defect independent of executor ownership.

Tests live in `src/Vixen.Tests`. The test project must be built with full Visual Studio MSBuild before `dotnet test --no-build`, because two transitive C++/CLI projects require the Visual C++ toolset. `BaseSequence` currently has no dedicated executor lifecycle test. Add one in `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs` and add the minimal project reference or test-access configuration required to use `BaseSequence.SequenceExecutor`; do not make test-only methods public.

## Plan of Work

### Milestone 1: Publish the VIX-3991 user contract

Before editing code, use `.agents/skills/jira/SKILL.md` and the configured Jira connection to update VIX-3991. Keep the description user-facing. Explain that Vixen must not crash, restart playback, or leave a sequence playing when a looping sequence ends while a user stops playback, closes the sequence/editor, changes playback context, or exits the application.

Add acceptance criteria stating that normal looping continues to restart and report its restart event while the executor remains active; stopping or disposing at the loop boundary does not crash or restart; media and timing remain stopped after the stop; and existing non-loop end behavior remains unchanged. State that automated race-focused tests, the full x64 test suite, and a manual stop/close boundary scenario will validate the work. Add a concise progress comment after each implementation or validation milestone. If Jira is unavailable, record the failure in this plan and leave tracker actions pending; do not fabricate updates.

### Milestone 2: Establish deterministic stale-callback tests

Read all of `SequenceExecutor.cs`, `HighResolutionTimer.cs`, `SequenceContext.cs`, `ProgramExecutor.cs`, `ISequenceExecutor.cs`, `IExecutor.cs`, and `ITiming.cs` immediately before editing. Then create `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs` using xUnit v3 and Moq, consistent with the existing test project. Add the smallest appropriate reference from `Vixen.Tests` to `src/Vixen.Common/BaseSequence/BaseSequence.csproj`; follow repository project-reference conventions and do not add NuGet packages.

The tests must use a deterministic synchronization context installed before constructing the executor. The context must capture `Post` callbacks without automatically running them, and expose a test-only method that runs the next captured callback on the test thread. Use a minimal fake or mock `ISequence` and `ITiming` that let playback begin, reach its requested end, and avoid real media. If the existing timing and sequence interfaces make that setup unwieldy, add a narrowly scoped internal test seam with friend-assembly access rather than depending on elapsed-time sleeps or reflection into private state. Any new internal seam must preserve the normal production constructor and have XML documentation only if it is public or protected.

Cover these observable cases:

- A live `PlayLoop` run reaches its end, dispatches the captured callback, restarts timing at `StartTime`, and raises exactly one `SequenceReStarted` event.
- A loop restart is captured, then `Stop` is called before dispatch. Dispatching the stale callback does not throw, does not restart timing or media, and does not raise `SequenceReStarted`.
- A loop restart is captured, then `Dispose` is called before dispatch. Dispatching the stale callback does not throw and does not access cleared executor state.
- A completed non-loop run still queues and performs its normal stop behavior, including a single `SequenceEnded` notification.
- Disposal while the end-check path is active does not throw from timer-state synchronization. Make this deterministic through the same test seam or controlled callback boundary; do not use a timing-sensitive stress test as the sole regression proof.

Keep all test helpers private to the test file unless reuse genuinely requires a separate helper file. The tests must prove behavior before and after the posted-work boundary rather than merely asserting that fields are non-null.

### Milestone 3: Invalidate stale runs and make executor cleanup safe

In `src/Vixen.Common/BaseSequence/SequenceExecutor.cs`, introduce private executor lifecycle state that distinguishes a current active play run from stale work. Prefer a private integer or long generation value protected by one private readonly lock object. Increment or otherwise invalidate this value before any operation that makes queued work obsolete: beginning a new play request, stopping playback, and disposal. Capture the value at the point `_CheckForNaturalEnd` decides to post `_loopPlay` or `_Stop`; pass it as the `Post` state. In the UI callback, take the lifecycle lock and return before touching timing, media, events, or the timer unless all of these remain true: the executor is not disposed, it is still running, the run identifier matches, and the requested action is still applicable (`_loop` for restart).

Retain valid behavior: a live loop callback resets to `StartTime`, starts the same timing source, raises `SequenceReStarted`, and resumes end checking. A valid non-loop natural-end callback still stops the sequence. Make the run-validation and timer transition atomic relative to `Stop` and `Dispose`, so a stop cannot interleave between a successful check and restart initialization. Do not hold the lifecycle lock while executing event subscribers or while the existing wait loop sleeps; snapshot only the required live references and release the lock before those operations if necessary. This prevents UI event handlers from deadlocking when they request playback changes.

Replace every `lock (_endCheckTimer)` with the dedicated immutable lock object. Under that lock, copy `_endCheckTimer` to a local variable, verify it is non-null and the executor is live, and only then query, start, or stop it. Apply this consistently in `_loopPlay`, `_Stop`, `_EndCheckTimerElapsed`, and any other timer-access path discovered during review. Do not dereference `_syncContext` after disposal; before posting work, capture it in a local and verify it is non-null and the run is still valid.

Update `Dispose(bool)` to be idempotent and terminal. While holding the lifecycle lock, mark the executor disposed, invalidate the current run, and capture the timer reference. Outside the lock, stop the captured timer and detach its event handler without relying on a mutable field. Preserve safe repeated disposal. Clear references only after no future executor path requires them; retaining a harmless reference is preferable to nulling a field that an asynchronous callback could observe. Do not change the public `ISequenceExecutor` API unless the implementation cannot be made testable otherwise. If any public or protected API changes, read and follow `.agents/skills/csharp-docs/SKILL.md` and update its XML documentation in the same change.

Review `SequenceContext.Dispose(bool)` after the executor fix. If tests show direct context disposal can leave timing/media running before executor cleanup, make the smallest ordering correction so it calls `Stop` before `_DisposeSequenceExecutor`; preserve the normal `ContextManager` release behavior and avoid duplicate end events. Record the decision and test evidence in this plan. Do not alter `HighResolutionTimer` unless a focused test proves executor-side synchronization cannot address the race.

### Milestone 4: Validate the repaired lifecycle and update the ticket

From `C:\Dev\Vixen`, inspect the intentional diff and run the project-prescribed x64 test build. Run the new focused tests first, then the complete suite:

    git diff --check
    git diff -- src/Vixen.Common/BaseSequence/SequenceExecutor.cs src/Vixen.Core/Execution/Context/SequenceContext.cs src/Vixen.Tests/Vixen.Tests.csproj src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs docs/plans/sequencer/vix-3991-loop-play-disposal-race.md
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/" --filter "FullyQualifiedName~SequenceExecutorLifecycleTests"
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/"

Expect the MSBuild command to finish with zero errors and both test commands to report zero failures. Record actual test counts. If the full toolchain is unavailable or an unrelated existing test fails, preserve the complete failure output in this plan, run every remaining viable check, and do not modify dependencies or unrelated tests merely to obtain a green run.

Manually open a short sequence with media in the Timed Sequence Editor. Configure or use the existing loop playback command over a short, non-zero range. Repeatedly let the range reach its end and immediately stop playback; repeat while closing the editor or application at the boundary. Also let the loop run through several normal restarts and run the same range without loop enabled. Observe that normal looping restarts cleanly, while stop/close prevents a restart, leaves audio/output stopped, and produces no application error. Include the exact manual scenario and observed result in this plan.

Finally, revise VIX-3991 if implementation discoveries changed the user-facing requirements or acceptance criteria. Add a concise Jira comment with the focused test result, full-suite result, and manual validation result. Update every living-document section of this plan and append a dated revision note. When a completed milestone changes repository files, generate a proposed commit message using `.agents/skills/commit-msg/SKILL.md`; do not create a commit unless the user explicitly requests it.

## Concrete Steps

All commands run from `C:\Dev\Vixen`.

1. Reconfirm the relevant asynchronous and disposal paths before changing them:

       rg -n -C 8 "_loopPlay|_CheckForNaturalEnd|_EndCheckTimerElapsed|_Stop\(|Dispose\(|_endCheckTimer|_syncContext" src/Vixen.Common/BaseSequence/SequenceExecutor.cs src/Vixen.Core/Execution/Context/SequenceContext.cs src/Vixen.Core/Execution/ProgramExecutor.cs

2. Complete Milestone 1's Jira description update, then add the focused lifecycle tests and run the focused command from Milestone 4. The stale-callback tests must fail against the original implementation by reaching the queued restart after disposal, then pass after the repair without relying on a real UI pump or a sleep-based race.

3. Implement Milestone 3 in small edits. After each edit, rerun the focused test command. Review the final diff with `git diff --check` and confirm no unrelated formatting changed.

4. Run the full build and test commands in Milestone 4, perform the manual loop-boundary exercise, and record concrete evidence in this plan and VIX-3991.

## Validation and Acceptance

The repair is accepted when a normal live looping sequence still restarts at the configured start time and raises one restart event per natural end, and a non-loop sequence still ends normally. A stop, context release, context disposal, editor close, or application exit that occurs after a loop restart has been queued but before it executes must cause the queued callback to do nothing. It must not throw, restart timing, restart media, raise `SequenceReStarted`, or start the end-check timer again.

Automated acceptance requires deterministic tests for live restart, stale callback after stop, stale callback after disposal, non-loop completion, and timer/disposal synchronization, plus the complete `Vixen.Tests` suite after the x64 MSBuild test build. Manual acceptance requires repeatedly stopping or closing a looping short sequence at its end with no fatal error and no unexpected continued playback.

## Idempotence and Recovery

The implementation must make `Stop` and `Dispose` safe to call repeatedly. A stale callback is expected during normal queue timing and must return without logging an error or changing playback state. If a test intermittently depends on timer scheduling, replace elapsed-time waiting with the deterministic synchronization-context or timer-dispatch seam described above; do not increase sleeps or retry loops as a substitute for correctness.

If a change prevents valid looping, inspect generation creation and invalidation points first: a generation must remain valid from `PlayLoop` until `Stop`, `Dispose`, or a new play request invalidates it. If shutdown deadlocks, identify whether a lock is held while event handlers, media operations, timing operations, or a thread join runs; reduce the protected region to state transition and local-reference capture. Revert only the intentional executor/context/test changes if necessary, retain the regression tests, and rework the synchronization approach.

## Artifacts and Notes

The reported failure is represented by this causal sequence:

    timer thread detects end while _loop is true
    -> _syncContext.Post(_loopPlay)
    -> Stop or Dispose invalidates/releases executor state
    -> WinForms UI queue invokes _loopPlay
    -> current code executes lock(_endCheckTimer)
    -> _endCheckTimer is null and ArgumentNullException terminates the application

The post-dispatch guard must be semantic, not merely null-safe:

    if (disposed || !IsRunning || !_loop || postedGeneration != currentGeneration)
        return;

The exact names and field types may vary with repository style, but the final implementation must preserve this behavior and use one immutable lock target for all timer/lifecycle transitions.

## Interfaces and Dependencies

Keep `Vixen.Execution.ISequenceExecutor`, `Vixen.Execution.IExecutor`, `Vixen.Module.Timing.ITiming`, and `Vixen.Utility.HighResolutionTimer` public contracts unchanged. `BaseSequence.SequenceExecutor` remains the owner of timer, timing, and media lifecycle. `Vixen.Execution.Context.SequenceContext` and `Vixen.Execution.ProgramExecutor` remain owners that request lifecycle transitions; they must not need knowledge of the synchronization-context implementation.

Use existing .NET synchronization primitives (`lock`, `SynchronizationContext.Post`, and an integer generation value protected by the lifecycle lock) and existing xUnit/Moq packages. Do not add package dependencies, new threads, a polling cancellation worker, or a public cancellation API. Any test visibility required for deterministic tests must be internal and limited to the `Vixen.Tests` friend assembly, with production behavior exercised through the normal executor methods.

Plan revision note (2026-08-23): Initial VIX-3991 ExecPlan created from the Jira report, the crash investigation, and `.agents/PLANS.md`. It requires the mandated initial and final Jira milestones, chooses generation-based stale-callback invalidation with immutable timer synchronization, and explicitly prohibits a null-guard-only fix.

Plan revision note (2026-08-23): Completed Milestone 1. Updated VIX-3991 with a concise user-facing Summary, Scope, Acceptance Criteria, and validation approach, then added a progress comment. No production or test code was changed.

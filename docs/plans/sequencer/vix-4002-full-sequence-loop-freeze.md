# Prevent full-sequence audio loops from freezing the sequencer

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain it according to `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

VIX-4002 fixes a sequencer freeze that occurs when an audio-backed loop reaches the end of the entire sequence and restarts at time zero. After this change, full-sequence loops and selected subrange loops restart without blocking the UI, a loop whose audio timing is temporarily still at zero remains controllable, and an obsolete audio-device completion callback cannot dispose a newer output device. A user can observe the result by repeatedly looping a short sequence with audio over both the whole sequence and a nonzero range, then stopping or closing it at a loop boundary without a hang or unexpected restart.

## Progress

- [x] (2026-09-17) Read `.agents/PLANS.md`, the VIX-4002 design handoff, current executor and audio-player implementations, existing VIX-3991 lifecycle tests, project references, and the prescribed test workflow.
- [x] (2026-09-17 09:53-05:00) Updated VIX-4002 with the finalized user-facing requirements, acceptance criteria, and validation plan.
- [x] (2026-09-17 10:02-05:00) Added deterministic executor and audio-output ownership regression tests; the focused pre-fix baseline reports 7 passed and 5 expected failures.
- [ ] Remove executor position polling; implement guarded stop-seek-start loop transitions and timing-advance gating.
- [ ] Make CoreAudioPlayer natural-stop cleanup conditional on the originating output instance.
- [ ] Run focused and full x64 validation, perform manual loop testing, update VIX-4002, and record final evidence here.

## Surprises & Discoveries

- Observation: Both initial playback and the UI-posted loop restart synchronously wait for `TimingSource.Position` to differ from `StartTime`.
  Evidence: `src/Vixen.Common/BaseSequence/SequenceExecutor.cs` lines 219-222 and 255-258 contain unbounded `Thread.Sleep(1)` loops. `_loopPlay` is invoked through the synchronization context at line 442.

- Observation: A stopped CoreAudioPlayer deliberately reports position zero.
  Evidence: `src/Vixen.Common/AudioPlayer/CoreAudioPlayer.cs` lines 177-184 return `TimeSpan.Zero` when `PlaybackState` is stopped. At a full-range restart, `StartTime` is also zero, so either wait loop can never exit.

- Observation: VIX-3991 already protects queued executor work with `_executionGeneration` and `_lifecycleLock`, but does not distinguish a just-started timing source from an ended timing source that reports zero.
  Evidence: `_IsEndOfSequence` at `SequenceExecutor.cs` lines 480-484 treats either `Position >= EndTime` or `Position == TimeSpan.Zero` as a natural end, and `_CheckForNaturalEnd` posts work with the current generation.

- Observation: The natural `PlaybackStopped` handler cleans whichever instance is currently in `_soundOut`, rather than the instance that raised the event.
  Evidence: `PlaybackDeviceOnPlaybackStopped` at `CoreAudioPlayer.cs` lines 372-375 calls parameterless `CleanupSoundOut`, which reads mutable `_soundOut` under its lock. A delayed event from an old output can therefore dispose a replacement.

- Observation: `Vixen.Tests` currently references `BaseSequence` but not `AudioPlayer`; no CoreAudioPlayer ownership tests exist.
  Evidence: `src/Vixen.Tests/Vixen.Tests.csproj` includes `BaseSequence.csproj`, while searching `src/Vixen.Tests` finds no CoreAudioPlayer or `IWavePlayer` tests.

- Observation: The new tests reproduce all intended pre-fix failures without a real UI, audio device, media file, or timer race.
  Evidence: After the x64 `Vixen_Tests` build, the focused test filter ran 12 tests in one second: 7 passed and 5 failed. The failures are the initial timing stall, full-range restart stall, full-range timing-advance gate, partial-range restart stall, and stale output cleanup; the existing VIX-3991 Stop/Dispose callback tests remain passing.

- Observation: Reflection can exercise CoreAudioPlayer's private ownership callback using an uninitialized instance and fake `IWavePlayer` objects, so an internal production test seam is unnecessary.
  Evidence: `CoreAudioPlayerOwnershipTests` initializes only `_soundOutLock` and `_soundOut`, invokes `PlaybackDeviceOnPlaybackStopped`, and the stale-sender assertion fails against the current implementation because it clears/disposes the replacement.

## Decision Log

- Decision: Treat every loop boundary as an explicit stop, seek, then start lifecycle transition.
  Rationale: Restarting a timing source without first stopping it leaves the previous timing run and its asynchronous audio cleanup able to overlap the new run. The ordered transition makes ownership and test assertions unambiguous.
  Date/Author: 2026-09-17 / Codex

- Decision: Replace synchronous position polling with a private `_timingHasAdvanced` gate, guarded by the existing `_lifecycleLock`.
  Rationale: A timing source may legitimately continue to report its configured start immediately after `Start`; waiting for it blocks the UI. The existing ten-millisecond end-check timer can instead defer end detection until it has observed a position strictly greater than `StartTime`.
  Date/Author: 2026-09-17 / Codex

- Decision: Preserve the existing `Position == TimeSpan.Zero` natural-end rule after the gate opens.
  Rationale: CoreAudioPlayer represents natural audio completion as stopped/zero, and the timer can sample that state before it samples a position at or past `EndTime`. Removing the rule would regress normal non-loop end detection.
  Date/Author: 2026-09-17 / Codex

- Decision: Keep VIX-3991's generation checks and use no new public playback API, worker thread, retry loop, or polling wait.
  Rationale: The generation token already makes queued UI callbacks harmless after Stop or Dispose. The defect is startup/end interpretation, not missing background work.
  Date/Author: 2026-09-17 / Codex

- Decision: Make audio-output cleanup ownership-aware through a small internal, hardware-free test seam if direct construction of `CoreAudioPlayer` would require a real audio file/device.
  Rationale: The regression is object ownership, so fake `IWavePlayer` instances must deterministically raise late callbacks. Keep this seam internal and add `InternalsVisibleTo` only if necessary; do not expose it through `IPlayer`.
  Date/Author: 2026-09-17 / Codex

- Decision: Use the repository's existing reflection-based private-boundary test convention instead of adding an internal AudioPlayer seam.
  Rationale: A constructor-free CoreAudioPlayer test can set only its lock and current output then invoke the real private callback with fake `IWavePlayer` instances. This keeps Milestone 2 test-only and avoids expanding AudioPlayer's production surface before the ownership repair is implemented.
  Date/Author: 2026-09-17 / Codex

## Outcomes & Retrospective

Milestones 1 and 2 are complete: VIX-4002 records the agreed user-facing behavior, and the deterministic regression suite exposes the current stalls and stale-output cleanup defect. Production implementation has not begun. This plan defines the required lifecycle behavior, ownership rule, and validation evidence for the remaining work.

## Context and Orientation

`src/Vixen.Common/BaseSequence/SequenceExecutor.cs` is the shared playback owner for sequence execution. It determines the selected range through `StartTime` and `EndTime`, starts an `ITiming` source and sequence media, and uses a `HighResolutionTimer` every ten milliseconds to detect a natural end. `ITiming.Position` is the current playback time. When a natural end occurs, the executor posts a callback to the synchronization context captured at construction; in the desktop application this is the UI message queue. A posted callback runs later, so it can race with Stop or Dispose.

VIX-3991 introduced `_executionGeneration` and `_lifecycleLock` to invalidate callbacks that were posted before Stop or Dispose. Preserve that behavior exactly: a callback must return without touching timing, media, events, or timer state unless it belongs to the current running generation; loop callbacks must additionally require loop mode. `SequenceReStarted` is the existing event consumed by sequence execution contexts to reset loop-dependent state. It must still fire exactly once for each valid loop restart.

`src/Vixen.Common/AudioPlayer/CoreAudioPlayer.cs` adapts NAudio's `IWavePlayer` output to Vixen's `IPlayer`. `_soundOut` owns the current output, and NAudio raises `PlaybackStopped` asynchronously, including after an output has been replaced or disposed. Ownership-aware cleanup means a callback may release its sender only if the sender is reference-equal to the currently owned `_soundOut`; a callback from any older output is a no-op. Explicit owner operations (`Stop`, pre-`Play` cleanup, and `Dispose`) still clean the current output intentionally.

`src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs` is the existing VIX-3991 regression suite. It installs a queued synchronization context, manually drives the private natural-end boundary, and uses a test `ITiming`, avoiding real UI dispatch and elapsed-time races. `src/Vixen.Tests/Sequencer/SequenceExecutorTestCollection.cs` prevents parallel mutation of the process-wide synchronization context. Extend these tests rather than creating a second executor lifecycle harness.

The test project requires full Visual Studio MSBuild for its C++/CLI transitive dependencies. Build the `Vixen_Tests` target first, then use `dotnet test --no-build`; do not rely on `dotnet test` to build the project.

## Plan of Work

### Milestone 1: Publish the VIX-4002 contract

Before editing production code, read the project version of `.agents/skills/jira/SKILL.md` and use the configured Jira connection to update VIX-4002's description. State, in user-facing language, that looping either an entire audio sequence or a selected range must not freeze the editor at a boundary, a failed/delayed restart must remain stoppable, and an obsolete audio completion callback must not interrupt a newer playback output.

Include acceptance criteria for repeated full-range and partial-range loops, one `SequenceReStarted` per valid boundary, unchanged non-loop completion, safe Stop and Dispose around queued callbacks, and immunity to stale audio-output callbacks. State the deterministic automated tests, the prescribed x64 build/test workflow, and repeated manual audio-loop checks. Do not add interim Jira comments. If the connection is unavailable, record the exact failure in this plan and leave this milestone pending rather than inventing an update.

### Milestone 2: Add deterministic regression coverage

Read all of `SequenceExecutor.cs`, `CoreAudioPlayer.cs`, `ISequenceExecutor.cs`, `ITiming.cs`, `IPlayer.cs`, `HighResolutionTimer.cs`, and the audio project/test project files immediately before editing. Preserve unrelated work in the existing dirty worktree, including `docs/reviews/vix-4002-full-sequence-loop-design.md`.

Extend `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs` and its existing non-parallel collection. Refactor its private `TestTiming` into a deterministic configurable fake: it must record each `Stop`, `Position` setter, and `Start` operation; it must support normal automatic advancement, a restart that stays at zero, and explicit later advancement. Keep helpers private to the test file unless multiple tests require a small shared helper.

Add these executable tests, all driven by the queued synchronization context and direct private boundary invocation already used by the suite:

- Initial `Play` with a source that remains at `StartTime` returns promptly. This is the regression for the initial `_Play` wait loop; it must not use a sleep timeout as proof of correctness.
- A full-range `PlayLoop(TimeSpan.Zero, EndTime)` reaches end, then restarts with a timing fake that remains zero. Dispatching the queued callback returns synchronously, records exactly `Stop`, `Position = TimeSpan.Zero`, `Start` for the restart, raises one `SequenceReStarted`, and leaves the executor running.
- While that full-range restart remains at zero, repeated direct end checks return not-ended and enqueue no additional callbacks. After explicitly advancing beyond start, the next end check arms normal end detection; reaching `EndTime` queues exactly one next restart.
- A nonzero partial-range loop uses the same operation order and gate. It must seek to the configured nonzero start, not zero, and only regard a position strictly greater than that start as timing advancement.
- A stopped or disposed executor with a queued stalled-restart callback retains VIX-3991 behavior: dispatch neither throws nor starts timing/media, raises no restart event, and leaves running state false where applicable.
- A non-loop run that advances past the gate still naturally ends once at `EndTime`; a post-gate zero position is also still recognized as a natural end. This explicitly preserves the audio natural-end rule.

Do not make the production code waitable, add production-only delays, or depend on the real high-resolution timer. The pre-fix initial and restart cases should demonstrate the regression by a bounded test-dispatch mechanism that detects non-return, then be replaced by direct immediate assertions after the code fix; never leave an indefinitely blocking pre-fix test runnable in the normal suite.

Add a focused audio ownership suite, preferably `src/Vixen.Tests/AudioPlayer/CoreAudioPlayerOwnershipTests.cs`. Add the minimal `AudioPlayer.csproj` project reference to `Vixen.Tests.csproj`. Because the production CoreAudioPlayer constructor loads real media and observes system audio devices, extract only the private ownership transition needed for testability into an `internal` helper in the AudioPlayer assembly, or add a narrowly scoped internal factory/owner seam. Give the test project friend access in `AudioPlayer.csproj` only if the selected seam requires it. The seam must own an `IWavePlayer`, subscribe/unsubscribe its `PlaybackStopped` handler, and offer both unconditional cleanup for the owner and expected-instance cleanup for a natural callback; it must not create an alternative player lifecycle.

Use fake `IWavePlayer` objects that count `Dispose`, retain their event handlers, and can invoke an old `PlaybackStopped` event after a replacement is installed. Test that a stale natural-stop callback neither clears nor disposes the current replacement, that a current callback clears/disposes only its own output, and that stale callbacks triggered by explicit `Stop` and by `Dispose` cannot clean a subsequently installed output. Assert subscription cleanup and disposal counts as well as current ownership so the tests catch both stale cleanup and event leaks. No real file, sound card, or timer belongs in these tests.

### Milestone 3: Make executor lifecycle transitions non-blocking and gated

In `src/Vixen.Common/BaseSequence/SequenceExecutor.cs`, add private `_timingHasAdvanced` state protected exclusively by `_lifecycleLock`. Reset it to `false` immediately before every timing `Start`: initial `_Play` and a valid `_loopPlay` restart. Reset or invalidate it on `_Stop` and `Dispose(bool)` with the current generation so it cannot leak into a later execution.

Remove both `while (TimingSource.Position == StartTime) { Thread.Sleep(1); }` blocks completely. In `_Play`, retain the existing initialization order for media and start timing at the coerced range start, set `IsRunning`, and start the end-check timer immediately. Do not synchronously read or wait for timing movement.

Change `_loopPlay(long executionGeneration)` so that, while holding `_lifecycleLock`, it first validates `_IsCurrentLoopExecution(executionGeneration)`, captures the required live references and range, stops the end-check timer, calls `timingSource.Stop()`, sets `_timingHasAdvanced = false`, assigns `timingSource.Position = startTime`, then calls `timingSource.Start()`. This is the required stop-seek-start order. Still while the state is known current, prepare the existing restart event data and arrange timer restart for that same generation. Release `_lifecycleLock` before calling `OnSequenceReStarted`, because event subscribers may issue playback lifecycle commands. Reacquire through `_StartEndCheckTimer(executionGeneration)` after the event, retaining its generation guard, so a Stop/Dispose issued by a subscriber prevents the timer from restarting. Do not hold the lock while invoking subscribers, sleeping, waiting, or joining a thread.

Modify the natural-end decision behind `_CheckForNaturalEnd` so it samples `TimingSource.Position` under the existing lifecycle protection and applies this exact state machine only for timed sequences:

    On initial start or valid loop restart: _timingHasAdvanced = false.
    At an end check while _timingHasAdvanced is false:
        if Position <= StartTime, return not-ended without posting work.
        otherwise set _timingHasAdvanced = true.
    Once it is true, natural end means Position >= EndTime or Position == TimeSpan.Zero.

The strict `Position > StartTime` condition is intentional. It prevents both a normal immediate-start report and a stopped source reporting zero from being mistaken for end, including a partial loop whose start is nonzero. Do not remove the zero rule after the gate is true. Keep all generation checks, `_IsCurrentLoopExecution`, and timer access under `_lifecycleLock`; a stale queued callback must return before altering `_timingHasAdvanced`, timing, media, events, or timer state.

Do not change `ISequenceExecutor`, `ITiming`, `IPlayer`, sequence serialization, or `SequenceContext` unless the new deterministic tests prove an executor-only repair cannot meet the stated behavior. These are private lifecycle changes, so no public/protected C# API change or XML documentation change is expected. If implementation changes a public/protected member, read and apply `.agents/skills/csharp-docs/SKILL.md` before that edit and update all affected XML documentation.

### Milestone 4: Make CoreAudioPlayer cleanup instance-aware

In `src/Vixen.Common/AudioPlayer/CoreAudioPlayer.cs`, alter natural-stop cleanup so `PlaybackDeviceOnPlaybackStopped(object? sender, StoppedEventArgs e)` treats `sender as IWavePlayer` as the expected output. The cleanup path called from this handler must acquire `_soundOutLock`, compare the expected output by reference with the current `_soundOut`, and return without changing anything when they differ or sender is not an `IWavePlayer`. Only the currently owned sender may be detached, cleared, and disposed.

Retain an unconditional cleanup path for deliberate owner actions: `Stop`, `Play`'s stopped-state pre-cleanup, exception recovery, and `Dispose`. Keep all ownership state changes atomic: under `_soundOutLock`, capture the matching current instance, detach `PlaybackStopped`, clear the field, then dispose the captured instance in an order that cannot let a synchronous/reentrant or delayed callback erase a newer replacement. Whether this remains a private overload of `CleanupSoundOut` or delegates to the internal ownership helper introduced for tests, its behavior must be equivalent. Invoke the existing `PlaybackEnded` notification consistently with current semantics, but stale callbacks must never cause cleanup of a newer output.

Do not alter media decoding, volume, audio-device selection, `IPlayer`, or public CoreAudioPlayer behavior outside output ownership. Avoid waiting, retries, and background tasks. If an internal helper was introduced in Milestone 2, keep it narrowly scoped to wave-player ownership and use it as the single source of truth rather than duplicating lock/identity logic in production and tests.

### Milestone 5: Validate, exercise loops, and close the ticket

Run focused tests first from `C:\Dev\Vixen` after the full MSBuild test build has produced the C++/CLI dependencies:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)/" --filter "FullyQualifiedName~SequenceExecutorLifecycleTests|FullyQualifiedName~CoreAudioPlayerOwnershipTests"

Expect the focused command to match nonzero tests and report `Failed: 0`. Then inspect only the intentional diff and run the full prescribed suite:

    git diff --check
    git diff -- src/Vixen.Common/BaseSequence/SequenceExecutor.cs src/Vixen.Common/AudioPlayer/CoreAudioPlayer.cs src/Vixen.Common/AudioPlayer/AudioPlayer.csproj src/Vixen.Tests/Vixen.Tests.csproj src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs src/Vixen.Tests/AudioPlayer/CoreAudioPlayerOwnershipTests.cs docs/plans/sequencer/vix-4002-full-sequence-loop-freeze.md
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)/"

Expect zero build errors, no whitespace errors, and zero test failures. Record actual test counts and any environmental warnings in this plan. If full MSBuild is blocked by a missing Visual C++ toolset or locked output, record the exact failure, resolve only the local environmental condition, rerun all viable checks, and do not claim success without evidence.

Manually run a Debug build of Vixen and open a short sequence with audible media. First enable loop playback for the entire sequence, let it cross at least ten boundaries, and interact with the UI at every boundary; then stop and close the editor at a boundary. Repeat using a selected nonzero subrange. Finally disable looping and let playback finish normally. Observe that both loop forms restart without a UI freeze, restart events/reset behavior occur once per boundary, Stop/close works even immediately after a boundary, and non-loop playback ends once. Include the range values, repetitions, and observed outcomes in `Artifacts and Notes`.

Use the Jira skill to revise VIX-4002 only if implementation discoveries change its user-facing contract, then add one final Jira comment containing the focused test result, full-suite result, and manual-loop evidence. Update every living-document section, append a dated revision note, and—when completing a file-changing milestone—generate a proposed commit message with `.agents/skills/commit-msg/SKILL.md`. Do not create a commit unless explicitly requested.

## Concrete Steps

All commands run from `C:\Dev\Vixen` in PowerShell.

1. Reconfirm scope and preserve unrelated changes before editing:

       git status --short
       rg -n -C 8 "_Play|_loopPlay|_CheckForNaturalEnd|_IsEndOfSequence|_StartEndCheckTimer|_Stop\(|Dispose\(|_executionGeneration|_endCheckTimer" src/Vixen.Common/BaseSequence/SequenceExecutor.cs
       rg -n -C 8 "CleanupSoundOut|PlaybackDeviceOnPlaybackStopped|EnsureDeviceCreated|InitializeDevice|_soundOut" src/Vixen.Common/AudioPlayer/CoreAudioPlayer.cs

2. Complete Milestone 1's Jira description update. Read the Jira skill before using the tracker. If unavailable, write the failure under `Surprises & Discoveries` and continue code/test work without fabricating tracker evidence.

3. Complete Milestone 2. Run the focused test command from Milestone 5 after the full MSBuild test build. Confirm the filter runs at least one test. Keep all regression timing controlled by fakes and the queued synchronization context.

4. Complete Milestone 3, running the executor-focused filter after each coherent edit. Confirm the fake's initial and restart operations are exactly stop, seek, start where applicable, and no production `Thread.Sleep` remains in SequenceExecutor.

5. Complete Milestone 4, running the ownership-focused filter after each coherent edit. Confirm fake stale callbacks do not dispose the replacement.

6. Complete Milestone 5's diff check, full suite, Debug manual tests, Jira close-out, and living-plan update. Keep concise command output and manual observations in `Artifacts and Notes`.

## Validation and Acceptance

The work is accepted only when all of the following are demonstrably true:

- An audio-backed full-sequence loop whose restart reports zero remains responsive and does not queue repeated restarts until timing moves beyond zero.
- A nonzero partial-range loop uses its selected start on every restart and waits for movement beyond that selected start before arming natural-end detection.
- Valid loop restarts issue timing operations in Stop, Position, Start order and raise exactly one `SequenceReStarted` event per boundary.
- Stopped/disposed queued callbacks retain VIX-3991 behavior and cannot restart timing, media, or the end-check timer.
- Normal non-loop playback still ends once at `EndTime`, and a zero position after timing has advanced remains a natural end.
- A stale `PlaybackStopped` event from old output A cannot clear or dispose current output B; explicit Stop/Dispose callbacks from A likewise cannot affect B.
- Focused deterministic executor/ownership tests, the prescribed x64 MSBuild/no-build full test workflow, `git diff --check`, and repeated manual audio loops all succeed with recorded evidence.

## Idempotence and Recovery

These changes are lifecycle-only and require no data migration. It is safe to repeat builds, focused tests, and manual loop scenarios. Stop and Dispose must remain idempotent, and stale callback no-ops are an expected normal outcome rather than an error to log or retry.

If a test depends on elapsed time, replace its timing with the fake's explicit position advancement or captured synchronization-context dispatch; do not lengthen sleeps or add retries. If a loop no longer restarts, inspect the gate reset and the `Position > StartTime` transition first. If a stale callback still affects a replacement output, inspect whether field clearing and event detachment occurred atomically under `_soundOutLock` before disposal. Do not reset, discard, or overwrite the user's existing untracked design review or unrelated worktree changes.

## Artifacts and Notes

The causal sequence this plan must eliminate is:

    timing reaches natural end
        -> executor posts _loopPlay to the UI synchronization context
        -> loop restart seeks to zero and starts audio/timing
        -> stopped or raced CoreAudioPlayer reports Position == zero
        -> current _loopPlay waits forever for Position != StartTime on the UI thread

The repaired sequence is:

    timer observes natural end for current generation
        -> UI callback validates generation and loop mode
        -> stop timer and timing, reset advance gate, seek start, start timing
        -> raise one restart event without holding lifecycle lock
        -> timer samples until Position > StartTime
        -> only then evaluate Position >= EndTime or Position == zero

The required audio ownership boundary is:

    old output A raises PlaybackStopped after new output B is installed
        -> callback supplies A as expected output
        -> A is not reference-equal to current B
        -> cleanup is a no-op; B remains subscribed, owned, and undisposed

Add real validation transcripts here during implementation, for example:

    Focused lifecycle/ownership tests: Passed: <actual>, Failed: 0.
    x64 Vixen_Tests build: succeeded with 0 errors.
    Full no-build Vixen.Tests: Passed: <actual>, Failed: 0.
    Manual loop exercise: full range <range> repeated <count> times; partial range <range> repeated <count> times; no freeze, normal non-loop end, Stop/close responsive.

## Interfaces and Dependencies

Keep the public interfaces `Vixen.Execution.ISequenceExecutor`, `Vixen.Module.Timing.ITiming`, and `Common.AudioPlayer.IPlayer` unchanged. Continue using `SynchronizationContext.Post`, `HighResolutionTimer`, `IWavePlayer`, xUnit v3, and Moq; add no packages.

The final private executor state includes a lifecycle-lock-protected boolean equivalent to `_timingHasAdvanced`. Its effective rule is:

    bool IsNaturalEnd(TimeSpan position)
    {
        if (!_IsTimedSequence)
            return false;

        if (!_timingHasAdvanced)
        {
            if (position <= StartTime)
                return false;

            _timingHasAdvanced = true;
        }

        return position >= EndTime || position == TimeSpan.Zero;
    }

The exact private method name may differ, but it must run under `_lifecycleLock`, retain generation validation at posting/restart boundaries, and have the behavior above. CoreAudioPlayer's cleanup must effectively support both unconditional current-owner cleanup and expected-instance cleanup used by `PlaybackDeviceOnPlaybackStopped`; the latter must require reference equality with the currently owned `IWavePlayer`.

Out of scope: redesigning audio-device selection, changing NAudio packages, modifying sequence data or timing interfaces, changing HighResolutionTimer, adding polling threads, or changing VIX-3991's generation model.

Plan revision note (2026-09-17): Created this VIX-4002 ExecPlan from the handoff and current source review. No production code, tests, Jira issue, or existing untracked design review was modified.

Plan revision note (2026-09-17): Completed Milestone 1. Replaced VIX-4002's brief report with the finalized user-facing Summary, Scope, Acceptance Criteria, and validation approach. No production or test code was changed, and no interim Jira comment was added.

Plan revision note (2026-09-17): Completed Milestone 2. Added the AudioPlayer test reference, seven new deterministic executor/audio ownership regressions, and configurable test timing/queued-dispatch helpers. The x64 `Vixen_Tests` build succeeded; the focused pre-fix baseline ran 12 tests with 7 passing and 5 expected failures. No production behavior was changed.

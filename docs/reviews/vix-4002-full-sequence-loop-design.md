# Architecture Design: VIX-4002 — Sequencer hangs at the end when in loop mode

- **Core Strategy:** Treat a loop boundary as a lifecycle transition with three ordered operations: stop the current timing run, seek to the configured loop start, and start a new timing run. Remove the synchronous position polling from `SequenceExecutor`; the end-check timer will instead arm end detection only after it observes timing advance beyond `StartTime`. In `CoreAudioPlayer`, make playback-stop cleanup conditional on the callback belonging to the currently owned output instance. This applies the lifecycle ownership and resource disposal guidance from the project `dotnet-best-practices` and `dotnet-design-pattern-review` skills without changing a public contract.

  VIX-4002 reports that looping an entire sequence freezes the application at the sequence end. The source contains the direct freeze mechanism: both `_Play` and `_loopPlay` execute an unbounded `Thread.Sleep(1)` loop while the timing position equals the requested start, and `_loopPlay` runs on the captured UI synchronization context. If audio restart leaves `Position` at zero, the UI thread cannot process input, stop, close, or later callbacks.

  The full-sequence boundary also exposes an audio ownership race. `CoreAudioPlayer.Position` reports zero whenever its output is stopped. At end-of-file, `PlaybackDeviceOnPlaybackStopped` asynchronously calls `CleanupSoundOut()`, which cleans whichever object is currently in `_soundOut`. Meanwhile, `SequenceExecutor._loopPlay` can seek to zero and call `Start`, and `CoreAudioPlayer.Play` can replace `_soundOut`. A delayed callback from the old output can therefore clean the replacement. VIX-3465 documents the same entire-sequence-only freeze and identifies cleanup at the audio restart crossover; commit `9b7aa168b` added pre-play cleanup but did not associate asynchronous cleanup with the output instance that raised it.

- **Data Model & Property Contracts:** Keep `ISequenceExecutor`, `ITiming`, `IMedia`, and `IPlayer` unchanged. Add one private, lifecycle-lock-protected state value to `SequenceExecutor`, conceptually `bool _timingHasAdvanced`. Set it to `false` immediately before each initial start or loop restart. Set it to `true` only after the end-check path observes `TimingSource.Position > StartTime`. Reset or discard it when stopping, disposing, or starting a new execution generation.

  Refine `CoreAudioPlayer` cleanup internally so the method may receive an expected `IWavePlayer` instance. A natural-stop callback passes its `sender` as the expected instance. Under `_soundOutLock`, cleanup proceeds only when the expected instance is still the same object as `_soundOut`. Explicit owner operations such as `Stop`, `Play` cleanup, and `Dispose` may request unconditional cleanup of the currently owned instance. No serialized sequence data, settings, module data, or public API changes are required.

- **Mathematical / Boundary Logic:** Define `S = StartTime`, `E = EndTime`, `P = TimingSource.Position`, and `A = _timingHasAdvanced` for the active execution generation.

      Start or restart:
          stop end-check timer
          if this is a restart:
              timingSource.Stop()
          A = false
          timingSource.Position = S
          timingSource.Start()
          publish the existing started/restarted event
          start end-check timer immediately

      Each end-check tick, while the generation is current:
          P = timingSource.Position

          if A is false:
              if P <= S:
                  return NOT_ENDED
              A = true

          return (P >= E) OR (P == 00:00:00)

  The `P == 0` rule must remain because `CoreAudioPlayer` exposes natural audio completion as zero after its output enters the stopped state, potentially before the timer samples a position at or beyond `E`. The startup gate prevents that same zero from being misclassified during initial startup or immediately after a restart. Requiring `P > S`, rather than merely `P != S`, also prevents a failed nonzero seek whose stopped timing source reports zero from creating a rapid restart loop.

  The loop restart callback retains the VIX-3991 generation checks. Its required order is:

      validate current generation and loop mode
      stop timer
      capture current sequence, timing source, and range
      timingSource.Stop()
      mark timing as not advanced
      timingSource.Position = startTime
      timingSource.Start()
      raise SequenceReStarted once
      restart timer for the same generation

  There is no synchronous wait for position movement. If a timing source cannot start, the UI remains responsive and end detection remains unarmed instead of repeatedly posting restarts. Existing Stop and Dispose operations can still invalidate the generation and terminate the run.

- **Subsystem Component Matrix:** 

  | Component | Current role and defect | Designed change | Verification |
  |---|---|---|---|
  | `src/Vixen.Common/BaseSequence/SequenceExecutor.cs` | Owns timing and loop lifecycle; waits indefinitely on the UI context for `Position` to move. Restart does not explicitly stop the prior timing run. | Add the timing-advance gate, remove both sleep loops, and use stop–seek–start for loop restart while preserving execution-generation guards and event order. | Extend `SequenceExecutorLifecycleTests` with a timing source that remains at zero on restart; dispatch must return promptly, Stop must still work, and no second natural-end callback may be queued until position advances. Assert restart call order and one restart event. |
  | `src/Vixen.Common/AudioPlayer/CoreAudioPlayer.cs` | A delayed `PlaybackStopped` handler cleans the mutable current `_soundOut`, even when a newer instance has replaced the sender. | Make callback cleanup instance-aware and clear ownership atomically under `_soundOutLock`; a stale callback becomes a no-op. | Add focused tests through the smallest internal audio-output factory seam, or extract an internal owner helper if direct `IWavePlayer` construction is impractical. Simulate old-stop callback after replacement and assert the replacement remains owned and playable. |
  | `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs` | Covers VIX-3991 stale callbacks but its fake timing advances synchronously, so it cannot reproduce VIX-4002. | Add deterministic zero-position restart, delayed advance, full-range, subrange, stop-after-stalled-restart, and existing-generation regression cases. Avoid sleeps and real audio hardware. | Focused xUnit run passes and fails against the pre-fix code by blocking or by a bounded dispatch harness detecting that dispatch did not return. |
  | `src/Vixen.Core/Execution/Context/SequenceContext.cs` | Resets current effects when `SequenceReStarted` is raised. | No change. Preserve exactly one restart event after timing restart is requested. | Existing lifecycle tests plus an assertion that the event count remains one per boundary. |

- **Concurrency, Performance & Thread Safety:** `_timingHasAdvanced`, `_executionGeneration`, `_loop`, timer state, and the decision to post or restart must remain protected by `_lifecycleLock`. Do not hold `_lifecycleLock` while sleeping, waiting for hardware, joining a thread, or invoking subscriber code. Capture event arguments under the lock, release it, and then raise `SequenceReStarted`. Preserve the existing rule that stale synchronization-context callbacks return before touching timing, media, events, or timer state.

  `CoreAudioPlayer` must use `_soundOutLock` only for ownership changes. Capture the output being removed, detach its event, and clear `_soundOut` under the lock; dispose the captured output in the safest order supported by NAudio so a reentrant or delayed callback cannot clear a replacement. The expected-instance check is mandatory for natural-stop callbacks. Do not add polling, retries, new threads, or a blocking wait.

  The timer already samples every 10 ms, so moving startup detection into its existing tick adds no thread or meaningful CPU cost. It removes the UI-thread spin and its repeated position reads. The public timing abstractions and sequence serialization stay stable.

  Acceptance requires all of the following: an audio-backed full sequence loops repeatedly from zero without freezing; a selected subrange still loops from its configured start; non-loop playback still ends once; restart raises one `SequenceReStarted`; Stop and Dispose remain effective before and after a queued boundary callback; a timing source that never advances cannot freeze the UI; and an old audio stop callback cannot release a newer output instance. Run the focused executor/audio tests first, then the repository-prescribed x64 MSBuild and `dotnet test --no-build` workflow, followed by manual full-sequence and subrange loop checks with audio.

## TERRA HAND-OFF CONTEXT

VIX-4002 is a recurrence of the full-sequence loop freeze described by VIX-3465. The visible freeze is deterministic in `BaseSequence.SequenceExecutor`: `_loopPlay` is posted to the UI synchronization context and busy-waits with `Thread.Sleep(1)` until `TimingSource.Position != StartTime`; at a full-range restart `StartTime == 0`, and stopped audio reports position zero, so any failed/raced restart blocks the UI forever. The trigger is an EOF ownership race in `Common.AudioPlayer.CoreAudioPlayer`: `PlaybackDeviceOnPlaybackStopped` calls parameterless `CleanupSoundOut`, which operates on mutable `_soundOut`; after `Play` replaces the output, a delayed callback from the old output can dispose the replacement. Implement two complementary private changes with no public API changes. (1) In `SequenceExecutor`, model loop restart as timer-stop, `timingSource.Stop()`, seek, start, event, timer-start. Remove both initial and restart position-wait loops. Add private `_timingHasAdvanced`, guarded by `_lifecycleLock`, reset before every start/restart. In end checking, while false return not-ended until `Position > StartTime`, then set true and apply the existing `Position >= EndTime || Position == TimeSpan.Zero` end rule. Preserve VIX-3991 execution-generation validation and never hold `_lifecycleLock` during waits or event callbacks. (2) In `CoreAudioPlayer`, make natural-stop cleanup accept the callback sender/expected `IWavePlayer` and no-op unless it is reference-equal to current `_soundOut`; change ownership atomically under `_soundOutLock` so stale callbacks cannot clear replacements. Tests: extend `SequenceExecutorLifecycleTests` with deterministic timing that remains at zero on second Start, prove synchronization-context dispatch returns, prove no end is detected before advance, then advance and prove normal looping; assert restart operation order Stop→Position→Start, one restart event, full-range and subrange behavior, stale Stop/Dispose callback protections. Add an internal audio ownership test/seam to simulate old stop callback after replacement. Validate focused tests, full x64 MSBuild/test workflow, and manual repeated full-sequence and subrange audio looping.

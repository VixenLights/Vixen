# Fix Playback End Button Preserving Time Range Start

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Keep this document self-contained when revising it so a future contributor can implement the fix without relying on chat history.

## Purpose / Big Picture

Users can drag on the sequencer ruler to define a playback time range, then use the playback toolbar's End button to extend that range to the end of the sequence. Today the End button corrupts the range by moving the playback start marker to the sequence length. The next Play command therefore starts at the end of the sequence instead of the start of the selected time range. After this change, pressing End while a range exists will leave the range start unchanged, set the range end to the sequence end, and the next Play command will begin at the original range start.

The behavior is visible in the Timed Sequence Editor by creating a range in the ruler, pressing End, then pressing Play. Playback must start from the range's left marker and continue to the sequence end.

## Progress

- [x] (2026-07-23 00:00Z) Read `.agents/PLANS.md` and confirmed the required ExecPlan structure.
- [x] (2026-07-23 00:00Z) Read `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs` and confirmed the ruler writes `PlaybackStartTime` and `PlaybackEndTime` when a user drags a time range.
- [x] (2026-07-23 00:00Z) Read the Timed Sequence Editor playback code and identified the faulty End button assignment in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs`.
- [x] (2026-07-23 00:00Z) Fetched Jira issue `VIX-1931` and confirmed the existing bug report describes this same playback-range behavior.
- [ ] Implement the End button fix in `TimedSequenceEditorForm_Toolstrip.cs`.
- [ ] Add or update focused regression coverage for the playback range state, if practical in the existing test harness.
- [ ] Build or test the affected project.
- [ ] Manually verify the sequencer workflow in the running application.
- [x] (2026-07-23 00:00Z) Updated Jira issue `VIX-1931` with cause, fix, acceptance criteria, and test plan.

## Surprises & Discoveries

- Observation: The End toolbar handler assigns the sequence length to `TimelineControl.PlaybackStartTime`, not `TimelineControl.PlaybackEndTime`.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs` currently contains `TimelineControl.PlaybackStartTime = _mPrevPlaybackEnd = _sequence.Length;` in `toolStripButton_End_Click`.
- Observation: Normal Play reads the timeline range directly, so the bad toolbar assignment immediately feeds the execution engine.
  Evidence: `PlaySequence()` in `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` uses `start = TimelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);` and `end = TimelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);`, then calls `_PlaySequence(start, end)`.
- Observation: Stopping playback restores `TimelineControl.PlaybackStartTime` and `TimelineControl.PlaybackEndTime` from `_mPrevPlaybackStart` and `_mPrevPlaybackEnd`.
  Evidence: `context_ContextEnded()` restores both fields. This explains why pressing Stop and Play again can appear to repair the range after the first bad Play path.
- Observation: The ruler itself is not the source of the End button bug.
  Evidence: `Ruler.OnMouseMove` sets `PlaybackStartTime` from the lower drag position and `PlaybackEndTime` from the higher drag position, then `timelineControl_TimeRangeDragged` stores those values into `_mPrevPlaybackStart` and `_mPrevPlaybackEnd` when the range drag completes.

## Decision Log

- Decision: Fix the toolbar End handler by assigning `_sequence.Length` to `TimelineControl.PlaybackEndTime` and `_mPrevPlaybackEnd`, leaving `TimelineControl.PlaybackStartTime` and `_mPrevPlaybackStart` unchanged.
  Rationale: The user's intended operation is to change the range end, not the range start. Normal playback already uses `PlaybackStartTime` as the start and `PlaybackEndTime` as the end, so preserving the start marker corrects the execution inputs with the smallest behavior change.
  Date/Author: 2026-07-23 / Codex
- Decision: Do not change `Ruler.cs` for this bug.
  Rationale: The ruler range drag path already creates a valid start and end pair. The corruption occurs only after the sequencer toolbar's End button mutates the shared `TimeInfo` playback fields.
  Date/Author: 2026-07-23 / Codex
- Decision: Keep the Start button out of scope unless testing proves it participates in the same regression.
  Rationale: `toolStripButton_Start_Click` currently sets the playback start to zero, which is consistent with a command named Start. `VIX-1931` specifically reports the End button changing the range start and causing Play to start at the sequence end.
  Date/Author: 2026-07-23 / Codex

## Outcomes & Retrospective

This plan has identified the fault, specifies a minimal correction, and the Jira issue has been updated with the cause, fix, acceptance criteria, and test plan. Implementation remains to be completed. After implementation, the expected outcome is that pressing End after selecting a playback range changes only the range end and the first subsequent Play starts from the original range start without requiring a Stop/Play recovery cycle.

## Context and Orientation

Vixen's timeline UI stores shared timing state in `src/Vixen.Common/Controls/TimeLineControl/TimeInfo.cs`. The important fields are `PlaybackStartTime`, `PlaybackEndTime`, and `PlaybackCurrentTime`. A playback time range is the optional pair of `PlaybackStartTime` and `PlaybackEndTime` shown by the ruler as gray start and end indicators. The current playback position is separate and is shown with the green playback indicator.

`src/Vixen.Common/Controls/TimeLineControl/Ruler.cs` lets the user drag across the ruler to create a time range. During a drag it writes the lower time to `PlaybackStartTime` and the higher time to `PlaybackEndTime`. When the drag finishes it raises `TimeRangeDragged`.

`src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` owns the shared `TimeInfo` object and wires it into child controls including the ruler, waveform, and grid. It exposes `RulerBeginDragTimeRange` and `RulerTimeRangeDragged` so the editor can react to ruler gestures.

`src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` handles sequencer playback. When a ruler range drag completes, `timelineControl_TimeRangeDragged` saves the current range into `_mPrevPlaybackStart` and `_mPrevPlaybackEnd`. When the user presses Play, `PlaySequence()` reads `TimelineControl.PlaybackStartTime` and `TimelineControl.PlaybackEndTime` and passes them to `_PlaySequence(rangeStart, rangeEnd)`, which calls either `_context.Play(rangeStart, rangeEnd)` or `_context.PlayLoop(rangeStart, rangeEnd)`.

`src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs` contains toolbar click handlers. The faulty code is in `toolStripButton_End_Click`. It currently stores `_sequence.Length` in `TimelineControl.PlaybackStartTime`, even though the End command is supposed to update the playback range end.

Jira issue `VIX-1931` is a Bug in project `VIX`, component `Editor/Sequencer`, titled `Playback Start Point`. Its existing description says that after sectioning off a part of the sequence, pressing End resets the play window to the beginning and should instead adjust the playback window from the selected start point to the end.

## Plan of Work

First, update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs` in `toolStripButton_End_Click`. Replace the bad assignment to `TimelineControl.PlaybackStartTime` with an assignment to `TimelineControl.PlaybackEndTime`. The final behavior should be equivalent to:

    TimelineControl.PlaybackEndTime = _mPrevPlaybackEnd = _sequence.Length;
    TimelineControl.VisibleTimeStart = TimelineControl.TotalTime - TimelineControl.VisibleTimeSpan;

Do not set `_mPrevPlaybackStart` or `TimelineControl.PlaybackStartTime` in this handler. Leaving those values alone preserves a ruler-selected range start such as 00:30.000 when the user extends the end to the sequence length.

Second, review whether `TimelineControl.TotalTime - TimelineControl.VisibleTimeSpan` can become negative for short sequences or wide zoom levels. If existing behavior already tolerates it and no related test fails, leave it unchanged to keep the fix focused. If a test or manual verification shows a negative visible start after pressing End, clamp it to `TimeSpan.Zero` in the same handler and document that extra finding in this plan.

Third, add focused regression coverage if the test project can reasonably instantiate the editor or exercise the toolbar behavior without launching the full WinForms UI. Prefer testing a small extracted internal helper over testing a private event handler through reflection. If introducing a helper, keep it internal to the Timed Sequence Editor assembly and avoid changing public or protected APIs. If a public or protected API must change, read and apply `.agents/skills/csharp-docs/SKILL.md` and update XML documentation in the same change.

Fourth, update Jira issue `VIX-1931` after the code change and validation. The Jira description should include the original user-visible symptom, the root cause in `toolStripButton_End_Click`, the planned or completed fix, acceptance criteria, and a test plan. Use the Jira MCP with `contentFormat` set to `markdown`.

## Concrete Steps

Work from the repository root:

    cd C:\Dev\Vixen

Inspect the current faulty handler:

    Select-String -Path src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceEditorForm_Toolstrip.cs -Pattern "toolStripButton_End_Click" -Context 0,8

Expected evidence before the fix:

    TimelineControl.PlaybackStartTime = _mPrevPlaybackEnd = _sequence.Length;

Edit `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs` so `toolStripButton_End_Click` writes the sequence length to `PlaybackEndTime` instead of `PlaybackStartTime`.

After editing, inspect the handler again. Expected evidence after the fix:

    TimelineControl.PlaybackEndTime = _mPrevPlaybackEnd = _sequence.Length;

Run a focused search to confirm no accidental duplicate End handler change was introduced:

    rg -n "toolStripButton_End_Click|PlaybackStartTime = _mPrevPlaybackEnd|PlaybackEndTime = _mPrevPlaybackEnd" src\Vixen.Modules\Editor\TimedSequenceEditor

If regression tests are added, run the narrowest relevant test command. If no focused test can be added because the editor is tightly coupled to WinForms and the execution context, record that limitation in `Outcomes & Retrospective` and rely on the manual verification below plus build coverage.

Run the Timed Sequence Editor project build, or the broader solution build if local time allows:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

If the environment lacks `msbuild`, use the repo's .NET test project for compile coverage:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

Update Jira issue `VIX-1931` using the Atlassian MCP. The new description should include text equivalent to the following:

    Original symptom:
    When a playback time range is selected in the sequencer ruler and the user clicks the playback toolbar End button, the range end should move to the end of the sequence while the original range start stays in place. Instead, the next Play starts at the sequence end. Pressing Stop and then Play again can restore the expected range behavior.

    Cause:
    The End button handler in src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs writes _sequence.Length to TimelineControl.PlaybackStartTime while also storing the value in _mPrevPlaybackEnd. PlaySequence() reads PlaybackStartTime as the start argument for _context.Play(), so the first Play after pressing End starts at the sequence length instead of the selected range start.

    Fix:
    Change toolStripButton_End_Click so it writes _sequence.Length to TimelineControl.PlaybackEndTime and _mPrevPlaybackEnd, and does not alter TimelineControl.PlaybackStartTime or _mPrevPlaybackStart. This preserves the selected range start and extends only the range end.

    Acceptance criteria:
    1. Given a sequence with a selected playback range from a non-zero start to an earlier end, clicking End moves the range end marker to the sequence end and leaves the range start marker unchanged.
    2. Pressing Play immediately after clicking End starts playback at the preserved range start, not at the sequence end.
    3. Pressing Stop is not required before the first correct playback.
    4. Existing ruler range drag behavior and normal Play/Stop behavior remain unchanged.

    Test plan:
    1. Create or open a timed sequence longer than one minute.
    2. Drag a playback range on the ruler, for example 00:15.000 to 00:30.000.
    3. Click the playback toolbar End button.
    4. Verify the left range marker remains at 00:15.000 and the right range marker moves to the sequence end.
    5. Click Play and verify playback begins at 00:15.000 and continues toward the sequence end.
    6. Repeat with Loop enabled and verify the loop uses 00:15.000 to the sequence end.
    7. Run the automated test or build command listed in the implementation notes and attach the result to the issue if project practice requires it.

## Validation and Acceptance

The primary acceptance test is manual because this bug is a user interaction between the ruler, toolbar, and playback execution context.

Open the Vixen application in Debug output, open a timed sequence, and create a playback range in the ruler from a non-zero start to a midpoint. Click End in the playback toolbar. The gray playback start indicator must remain at the selected non-zero start. The gray playback end indicator must move to the sequence end. Press Play immediately. The green current playback indicator and audio/playback must begin at the selected range start, not at the sequence end. Pressing Stop before Play must not be necessary.

Also verify the no-range case. Clear or reset the range, click End, and confirm the editor still scrolls to the end without throwing and without creating an invalid start/end pair. If the product expects End to create a full-sequence range, document and implement that explicitly; otherwise preserve the existing no-range behavior except for correcting the bad start assignment.

Run one of these validation commands from `C:\Dev\Vixen` and record the result:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

or:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

The change is accepted when the build or tests pass and the manual workflow proves that the first Play after pressing End starts from the selected range start.

## Idempotence and Recovery

The code change is a one-line assignment correction and can be safely reapplied by inspecting the handler. If the file already contains `TimelineControl.PlaybackEndTime = _mPrevPlaybackEnd = _sequence.Length;`, do not edit it again.

If a broader refactor is attempted and causes compile errors, revert only the refactor you made and keep the one-line End handler correction. Do not use `git reset --hard` or discard unrelated user changes. If validation reveals that `VisibleTimeStart` can become negative, add a small clamp in the End handler and document that discovery before continuing.

The Jira update is not idempotent if repeated blindly because it can overwrite issue text. Before editing Jira again, fetch `VIX-1931`, preserve any newer human edits, and merge this plan's cause/fix/acceptance/test-plan content into the current description.

## Artifacts and Notes

Faulty code before the fix:

    private void toolStripButton_End_Click(object sender, EventArgs e)
    {
        //TODO: JEMA - Check to see if this is functioning properly.
        TimelineControl.PlaybackStartTime = _mPrevPlaybackEnd = _sequence.Length;
        TimelineControl.VisibleTimeStart = TimelineControl.TotalTime - TimelineControl.VisibleTimeSpan;
    }

Expected code after the fix:

    private void toolStripButton_End_Click(object sender, EventArgs e)
    {
        //TODO: JEMA - Check to see if this is functioning properly.
        TimelineControl.PlaybackEndTime = _mPrevPlaybackEnd = _sequence.Length;
        TimelineControl.VisibleTimeStart = TimelineControl.TotalTime - TimelineControl.VisibleTimeSpan;
    }

Relevant playback flow:

    PlaySequence()
      start = TimelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero)
      end = TimelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue)
      _PlaySequence(start, end)
      _context.Play(start, end) or _context.PlayLoop(start, end)

## Interfaces and Dependencies

The implementation should use the existing `TimelineControl` and `TimeInfo` properties. No new external library or service is required.

The important existing members are:

    src/Vixen.Common/Controls/TimeLineControl/TimeInfo.cs
    public TimeSpan? PlaybackStartTime { get; set; }
    public TimeSpan? PlaybackEndTime { get; set; }
    public TimeSpan? PlaybackCurrentTime { get; set; }

    src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs
    private TimeSpan? _mPrevPlaybackStart;
    private TimeSpan? _mPrevPlaybackEnd;
    private void PlaySequence()
    private void _PlaySequence(TimeSpan rangeStart, TimeSpan rangeEnd)

    src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs
    private void toolStripButton_End_Click(object sender, EventArgs e)

Do not introduce a public or protected API for this fix unless testing proves it necessary. If a public or protected API is added or modified, update XML documentation using the project `csharp-docs` skill before completing the change.

## Revision Notes

- 2026-07-23 / Codex: Created the plan after tracing the ruler drag path, toolbar End handler, normal Play path, and Jira issue `VIX-1931`.
- 2026-07-23 / Codex: Updated the Progress and Outcomes sections after successfully updating Jira issue `VIX-1931`.

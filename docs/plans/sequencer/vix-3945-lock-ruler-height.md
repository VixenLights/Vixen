# VIX-3945 Lock Ruler Height

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. It is self-contained so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users can accidentally resize the Timed Sequence Editor ruler by brushing the bottom edge of the ruler and dragging when the resize cursor appears. After this change, the View menu has a persisted `Lock Ruler Height` option. The option defaults to off, so current ruler resize behavior is preserved for existing users. When the user enables it, the ruler keeps its current height, the resize cursor no longer appears at the ruler bottom edge, drag resizing is blocked, and double-click reset is blocked.

The visible result is in the Timed Sequence Editor. Open a sequence, choose `View > Lock Ruler Height`, and move the pointer near the bottom edge of the ruler. With the option checked, the horizontal split cursor does not appear and dragging does not change the ruler height. Uncheck the option and the current resize behavior returns.

## Progress

- [x] (2026-07-20 00:00 -05:00) Created the approved feature specification at `docs/sequencer/vix-3945-lock-ruler-height.md`.
- [x] (2026-07-20 00:00 -05:00) Created this initial ExecPlan from the approved specification.
- [x] (2026-07-20 15:28 -05:00) Milestone 1: Updated Jira VIX-3945 with refined requirements, implementation outline, acceptance criteria, and test plan in planning comment `40212`.
- [x] (2026-07-20 15:36 -05:00) Milestone 2: Added `LockRulerHeight` to the reusable ruler surface, added the `TimelineControl` forwarding property, and guarded ruler resize cursor, drag resize, and double-click reset paths.
- [ ] Add the Timed Sequence Editor View menu item below `Full Waveform` and persist `{Name}/LockRulerHeight` through `XMLProfileSettings`.
- [ ] Add focused automated coverage where practical.
- [ ] Run automated validation and perform manual Timed Sequence Editor validation.
- [ ] Update Jira VIX-3945 and this ExecPlan with final implementation and validation evidence.

## Surprises & Discoveries

- Observation: The original Jira description requested turning the lock on by default, but the approved specification says the option defaults off.
  Evidence: VIX-3945 description says "turn that on by default"; planning comment `40212` explicitly records the resolved default as unchecked/off.

- Observation: The tracked source folder is `src/Vixen.Common/Controls/TimeLineControl`, with a capital `L` in `Line`, even though some planning text and user shorthand use `TimelineControl`.
  Evidence: `git ls-files` returns `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs` and `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`; the Milestone 2 source changes are in those tracked files.

## Decision Log

- Decision: The setting defaults to off and uses the stable XML app setting key `{Name}/LockRulerHeight`.
  Rationale: The user explicitly requested the option default to off and be saved with the other XML profile settings.
  Date/Author: 2026-07-20 / Codex from user request.

- Decision: Place `Lock Ruler Height` directly below `Full Waveform` in the View menu.
  Rationale: The user approved this exact placement after reviewing the specification.
  Date/Author: 2026-07-20 / user direction recorded by Codex.

- Decision: The lock applies only to ruler height resizing and not to other ruler interactions.
  Rationale: The requested improvement is to prevent accidental height resize. Ruler clicks, time-range drags, mark selection, and mark movement are separate existing workflows and must keep working.
  Date/Author: 2026-07-20 / Codex from source review.

- Decision: Double-click ruler height reset is blocked while the lock is enabled.
  Rationale: Double-click reset changes ruler height through the same resize edge affordance. A locked height must not change from that interaction.
  Date/Author: 2026-07-20 / user direction recorded by Codex.

- Decision: If the lock is enabled while a ruler resize drag is active, stop changing the height immediately with simple guard logic only.
  Rationale: The user approved immediate stop behavior but noted the combined operation is unlikely and should not drive excessive state-management complexity.
  Date/Author: 2026-07-20 / user direction recorded by Codex.

## Outcomes & Retrospective

Code implementation has started and Milestone 2 is complete. The expected outcome remains a narrow Timed Sequence Editor improvement: a persisted View menu toggle controls whether the ruler can be interactively resized. The implementation should stay local to the reusable timeline ruler control and the editor settings/menu wiring.

Milestone 1 is complete. Jira VIX-3945 now has planning comment `40212` with refined requirements, implementation outline, acceptance criteria, and test plan. The comment also records that the approved implementation default is unchecked/off, resolving the older Jira description text that mentioned turning the lock on by default.

Milestone 2 is complete. `Ruler` now exposes a documented `LockRulerHeight` property, and `TimelineControl` exposes a documented forwarding property for later editor wiring. The only guarded interactions are the existing ruler-height resize affordances: the bottom-edge `Cursors.HSplit` branch, `MouseState.ResizeRuler` entry, height mutation during `MouseState.ResizeRuler`, and double-click reset. A focused build of `src\Vixen.Common\Controls\Controls.csproj` passed with four pre-existing warnings from `Vixen.Core` and zero errors.

## Context and Orientation

Vixen is a Windows desktop application for sequencing animated light shows. The Timed Sequence Editor is the main timeline editor. The timeline surface is built from reusable WinForms controls under `src/Vixen.Common/Controls/TimeLineControl`, and the editor host lives under `src/Vixen.Modules/Editor/TimedSequenceEditor`.

The ruler is the horizontal time display above the timeline grid. In this repository it is the `Common.Controls.Timeline.Ruler` class in `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs`. It draws time ticks, the playback cursor, selection arrows, and mark-related alignment indicators. It also handles mouse input for ruler clicks, time-range dragging, mark movement, and ruler height resizing.

The current ruler height resize behavior is implemented in `Ruler.cs`:

- `OnMouseMove` sets `Cursor = Cursors.HSplit` when no mouse button is held and the pointer is within about 6 pixels of the ruler bottom edge.
- `OnMouseDown` enters `MouseState.ResizeRuler` when the cursor is `Cursors.HSplit`.
- `OnMouseMove`, while the left button is held and `m_mouseState` is `MouseState.ResizeRuler`, changes `Height` when `e.Location.Y > 40`.
- `OnMouseDoubleClick` resets `Height` to 50 when the cursor is `Cursors.HSplit`.

`Cursors.HSplit` is the horizontal split cursor that tells the user a horizontal boundary can be dragged vertically. In this feature, it is the resize indicator that must not appear when the ruler height is locked.

`TimelineControl` in `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` creates the public `ruler` instance. During construction it docks the ruler above the grid with a default height of 50 pixels.

The Timed Sequence Editor form is split across generated and hand-written files:

- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs` declares menu items and the View menu layout.
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` contains menu event handlers.
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` loads and saves XML profile settings and wires editor events.

Profile settings are stored through `XMLProfileSettings` app settings. In `TimedSequenceEditorForm.cs`, existing timeline settings are loaded near startup. The relevant neighboring settings include `ResizeIndicatorEnabled`, `CadStyleSelectionBox`, `LegacyCursorActiveRow`, `WaveFormHeight`, and `RulerHeight`. The existing ruler height is loaded with a default of 50 and saved when the editor closes. The new lock setting belongs beside these settings and must not replace `RulerHeight`.

The approved feature specification is `docs/sequencer/vix-3945-lock-ruler-height.md`. If this ExecPlan and the specification ever disagree, update both documents before continuing so future agents do not inherit contradictory requirements.

## Plan of Work

Milestone 1 updates Jira with the approved plan before code changes. Use the project `jira` skill by reading `.agents/skills/jira/SKILL.md` first. Read VIX-3945, then add or update planning content with the summary, requirements, acceptance criteria, and test plan from this ExecPlan. If Jira tools are unavailable in the session, record that in `Surprises & Discoveries` and leave the paste-ready Jira text in `Artifacts and Notes`.

Milestone 2 adds the lock state to the reusable timeline control. Because the behavior belongs to the ruler itself, add a boolean property to `Ruler` named `LockRulerHeight`. The default value is false. Since this is a public C# API, use the project `csharp-docs` skill before implementation and add XML documentation to the new property. The property should be simple and should not persist anything by itself.

Optionally add a forwarding property to `TimelineControl`, also documented if public, so editor code can assign `TimelineControl.LockRulerHeight` instead of reaching through `TimelineControl.ruler.LockRulerHeight`. This forwarding property is recommended because the editor already reaches into many timeline sub-controls, but a named timeline-level property makes the setting easier to understand. If added, it should simply get and set `ruler.LockRulerHeight`.

Update `Ruler.OnMouseMove`. In the no-button branch, the mark hover path must remain first so marks still use `Cursors.VSplit` and continue raising alignment activity. The bottom-edge resize branch must only set `Cursors.HSplit` when `LockRulerHeight` is false. When the lock is true and the pointer is at the bottom edge, the code should fall through to the same hand-cursor path used for ordinary ruler interaction and clear alignment activity in the same way the current hand-cursor path does.

Update `Ruler.OnMouseDown`. Guard the `Cursor == Cursors.HSplit` branch so it cannot enter `MouseState.ResizeRuler` when `LockRulerHeight` is true. This is defensive even though the cursor should no longer be set to `Cursors.HSplit` by the ruler-height edge when locked. Do not block mark selection, mark dragging, or ordinary time-range drag setup.

Update `Ruler.OnMouseMove` in the `MouseState.ResizeRuler` case. If `LockRulerHeight` is true, do not assign `Height`. A simple guard is enough. It is acceptable to do nothing until mouse up resets the state. Do not add complex cancellation logic for this unlikely edge case.

Update `Ruler.OnMouseDoubleClick`. Only reset height to 50 when `LockRulerHeight` is false and the current cursor is `Cursors.HSplit`.

Milestone 3 wires the Timed Sequence Editor menu and persistence. In `TimedSequenceEditorForm.Designer.cs`, add a `ToolStripMenuItem` named `lockRulerHeightToolStripMenuItem` with `CheckOnClick = true`, text `Lock Ruler Height`, and a click or checked-changed handler. Place it directly below `fullWaveformToolStripMenuItem` in the `viewToolStripMenuItem.DropDownItems` order. This means it should appear after `Full Waveform` and before the separator or following window/tool entries that currently come after `Full Waveform`.

In `TimedSequenceEditorForm_Menu.cs`, add the handler. It should assign the checked state to the ruler lock property immediately:

    private void lockRulerHeightToolStripMenuItem_Click(object sender, EventArgs e)
    {
        TimelineControl.LockRulerHeight = lockRulerHeightToolStripMenuItem.Checked;
    }

If no forwarding property is added to `TimelineControl`, assign `TimelineControl.ruler.LockRulerHeight` instead.

In `TimedSequenceEditorForm.cs`, load `{Name}/LockRulerHeight` near the existing `RulerHeight` and other timeline view settings:

    lockRulerHeightToolStripMenuItem.Checked = TimelineControl.LockRulerHeight =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockRulerHeight", Name), false);

Save it near the existing `RulerHeight` write:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockRulerHeight", Name), lockRulerHeightToolStripMenuItem.Checked);

Keep the existing `RulerHeight` load and save unchanged. Locking controls user interaction only; it does not force a default height and does not discard an existing custom height.

Milestone 4 adds focused automated coverage where practical. Start by inspecting existing tests under `src/Vixen.Tests/Sequencer` for patterns that instantiate timeline controls or test private helper behavior. If direct WinForms mouse simulation is brittle, keep automated coverage focused on deterministic logic. A small internal or public helper solely for testing is not required unless it makes the code cleaner. Do not over-abstract just to test cursor state.

Practical tests may include:

- a new `Ruler` defaults `LockRulerHeight` to false;
- assigning `LockRulerHeight` to true stores the property state;
- if a forwarding `TimelineControl.LockRulerHeight` property is added, setting it changes `ruler.LockRulerHeight`.

If deeper tests can cleanly exercise the mouse logic without timers, display handles, or screen-coordinate brittleness, add them. Otherwise record the limitation in `Surprises & Discoveries` and rely on manual validation for cursor and drag behavior.

Milestone 5 validates and closes out. Run a focused test or compile first, then run the full test project. Manually validate in the Timed Sequence Editor using the scenarios in `Validation and Acceptance`. Update Jira VIX-3945 with implementation notes, automated test results, and manual validation results. Update this ExecPlan's `Progress`, `Surprises & Discoveries`, `Decision Log`, `Outcomes & Retrospective`, and `Artifacts and Notes` with the final evidence.

## Concrete Steps

Work from repository root `C:\Dev\Vixen`.

First inspect the current files:

    rg -n "LockRulerHeight|RulerHeight|Full Waveform|fullWaveformToolStripMenuItem|ResizeRuler|HSplit|OnMouseDoubleClick" src\Vixen.Common\Controls\TimeLineControl src\Vixen.Modules\Editor\TimedSequenceEditor

Read required skills before editing public C# APIs:

    Get-Content -LiteralPath .agents\skills\csharp-docs\SKILL.md

If updating Jira in this same implementation session, also read:

    Get-Content -LiteralPath .agents\skills\jira\SKILL.md

Update Jira VIX-3945 using the Milestone 1 text in `Artifacts and Notes`. If Jira tools are not available, leave the text in this plan and record the blockage.

Implement `LockRulerHeight` in `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs`. Add XML documentation for the public property. Keep the default false value by relying on the default value for a bool field or auto-property.

If using the recommended forwarding property, implement it in `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` with XML documentation. Use the project style already present in that file for properties.

Modify `Ruler.OnMouseMove`. The no-button branch should behave like this in intent:

    if (marks.Any())
    {
        Cursor = Cursors.VSplit;
        _timeLineGlobalEventManager.OnAlignmentActivity(...);
    }
    else if (!LockRulerHeight && e.Location.Y <= Height - 1 && e.Location.Y >= Height - 6)
    {
        Cursor = Cursors.HSplit;
    }
    else
    {
        Cursor = Cursors.Hand;
        _timeLineGlobalEventManager.OnAlignmentActivity(new AlignmentEventArgs(false, null));
    }

Modify `Ruler.OnMouseDown` so the resize state branch requires the lock to be off:

    else if (!LockRulerHeight && Cursor == Cursors.HSplit)
    {
        m_mouseState = MouseState.ResizeRuler;
    }

Modify the `MouseState.ResizeRuler` case so it does not change height when locked:

    if (!LockRulerHeight && e.Location.Y > 40)
    {
        Height = e.Location.Y + 1;
    }

Modify `Ruler.OnMouseDoubleClick`:

    if (!LockRulerHeight && Cursor == Cursors.HSplit)
    {
        Height = 50;
    }

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`. Add `lockRulerHeightToolStripMenuItem` to the field list, instantiate it, configure it, wire the handler, and insert it into the View menu immediately after `fullWaveformToolStripMenuItem`.

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` with the menu handler.

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` to load and save `LockRulerHeight` beside the existing ruler and view settings. The setting key must be exactly `{Name}/LockRulerHeight` unless this plan is revised with a documented decision.

Add tests if practical. If adding a new test class, place it under an existing appropriate folder such as `src/Vixen.Tests/Sequencer`. Name it clearly, for example `RulerLockHeightTests`.

Run focused tests if added:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~RulerLockHeight --no-restore

Run the full test project:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If the test command fails because restore is needed or dependencies are missing, run the appropriate restore/build command already used by the repository, then rerun the tests:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj

Manual validation in the running application requires a Debug or Release build. From repository root, build with one of:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

or:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release

Launch the built Vixen application from the corresponding output folder, open a sequence in the Timed Sequence Editor, and perform the manual scenarios in `Validation and Acceptance`.

## Validation and Acceptance

Automated validation should include the full test command:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If focused tests were added, also run the focused command and update this plan with the actual test count and result. A successful full run should report all `Vixen.Tests` tests passed. If unrelated pre-existing failures appear, capture their names and evidence, then also capture a passing focused run or compile for this feature.

Manual acceptance is required because the most important behavior is a WinForms cursor and mouse-drag workflow. Verify these scenarios in the Timed Sequence Editor:

1. Open a sequence with a fresh or missing `LockRulerHeight` profile setting. Confirm `View > Lock Ruler Height` is unchecked and appears directly below `Full Waveform`.
2. With the item unchecked, hover near the bottom edge of the ruler. The horizontal split cursor appears.
3. With the item unchecked, drag the ruler bottom edge. The ruler height changes.
4. With the item unchecked, double-click the ruler resize edge. The ruler height resets to 50 pixels.
5. Enable `View > Lock Ruler Height`. The ruler height does not change when the menu item is toggled.
6. With the item checked, hover near the bottom edge of the ruler. The horizontal split cursor does not appear.
7. With the item checked, drag near the bottom edge of the ruler. The ruler height does not change.
8. With the item checked, double-click near the bottom edge of the ruler. The ruler height does not reset.
9. With the item checked, click and drag normal time ranges on the ruler. Ruler click and time-range drag behavior still works.
10. With the item checked, hover and drag existing marks on the ruler. Mark cursor and mark movement behavior still works.
11. Close and reopen the Timed Sequence Editor. `View > Lock Ruler Height` retains the prior checked state.
12. Disable `View > Lock Ruler Height`, close and reopen the editor, and confirm the item remains unchecked and ruler resizing works again.

Acceptance criteria for VIX-3945 are:

- `View > Lock Ruler Height` exists directly below `Full Waveform`.
- The menu item defaults to unchecked when the XML profile setting is missing.
- The setting persists through `XMLProfileSettings` using `{Name}/LockRulerHeight`.
- When unchecked, current ruler resize behavior remains unchanged.
- When checked, the ruler bottom edge does not show the horizontal split cursor.
- When checked, dragging the ruler bottom edge does not change ruler height.
- When checked, double-clicking the ruler resize edge does not reset ruler height.
- Ruler clicks, time-range dragging, mark hover indicators, mark selection, and mark movement still work.
- Existing saved `RulerHeight` is still loaded and saved.
- No sequence modified state, undo command, or sequence file content is introduced by this preference.

## Idempotence and Recovery

The implementation is additive and can be done incrementally. Re-running tests is safe. Updating Jira should be done once before implementation and once after validation; if a Jira update fails, read the issue or comments before retrying to avoid duplicate comments.

If the menu designer edit causes unrelated churn, revert only the unintended generated-file changes and reapply the minimal menu insertion. Do not revert user changes elsewhere in the worktree.

If the lock accidentally blocks ruler clicks, time-range drag, or mark movement, narrow the guards to only the `Cursors.HSplit`, `MouseState.ResizeRuler`, and double-click height reset paths. Do not disable the whole `Ruler` control and do not intercept all mouse input.

If tests cannot instantiate WinForms controls in the current environment, record the failure and keep manual validation explicit. The feature can still be accepted with compile/test validation plus manual UI validation because cursor and drag behavior are inherently user-interface behavior.

If a missing restore or sandboxed dependency issue blocks `dotnet test --no-restore`, rerun the necessary restore/test command with the appropriate approval flow for this environment, then document the command and result.

## Artifacts and Notes

Milestone 1 Jira update text:

    Summary:
    Add a persisted Timed Sequence Editor View menu option to lock ruler height and prevent accidental ruler resize.

    Requirements:
    - Add `View > Lock Ruler Height` directly below `Full Waveform`.
    - Default the option to unchecked when the XML profile setting is missing.
    - Save and load the option through Timed Sequence Editor XML profile app settings using `{Name}/LockRulerHeight`.
    - Preserve current ruler resize behavior when the option is unchecked.
    - When checked, prevent the ruler bottom edge from showing the horizontal split resize cursor.
    - When checked, prevent drag resizing of the ruler height.
    - When checked, block double-click reset of the ruler height.
    - Preserve ruler clicks, time-range dragging, mark hover indicators, mark selection, and mark movement.
    - Keep existing `RulerHeight` persistence unchanged.

    Implementation outline:
    Add a documented `LockRulerHeight` boolean property to the reusable timeline ruler surface, optionally forward it through `TimelineControl`, guard only the ruler height resize affordance paths in `Ruler.OnMouseMove`, `Ruler.OnMouseDown`, resize handling, and `Ruler.OnMouseDoubleClick`, then wire the Timed Sequence Editor View menu and XML profile setting.

    Acceptance criteria:
    With the option unchecked, the ruler can still show the split cursor, resize by drag, and reset height by double-click. With the option checked, the split cursor does not appear at the ruler bottom edge, drag resize does not change height, and double-click reset does not change height. The option persists between editor sessions and does not affect non-resize ruler interactions.

    Test plan:
    Run `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore` and any focused tests added for the lock property. Manually validate locked and unlocked ruler behavior in the Timed Sequence Editor, including persistence after close/reopen and mark/time-range interactions while locked.

Milestone 1 Jira update result:

    Added planning comment `40212` to VIX-3945 on 2026-07-20 15:28 -05:00. The comment includes the refined requirements, implementation outline, acceptance criteria, and test plan. It also calls out that the approved default is unchecked/off even though the original issue description mentioned turning the lock on by default.

Milestone 2 implementation evidence:

    Changed `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs`:
    - Added documented public `LockRulerHeight` property.
    - Prevented `MouseState.ResizeRuler` entry while locked.
    - Prevented height mutation in the resize state while locked.
    - Prevented the bottom-edge `Cursors.HSplit` resize indicator while locked.
    - Prevented double-click height reset while locked.

    Changed `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`:
    - Added documented public forwarding property `LockRulerHeight`.

Milestone 2 build evidence:

    dotnet build src\Vixen.Common\Controls\Controls.csproj --no-restore

    Build succeeded.
        4 Warning(s)
        0 Error(s)

    Warnings were from existing `Vixen.Core` code: two nullable annotation context warnings in `Rule\IElementTemplate.cs`, one obsolete `Extensions.Raise` warning in `Sys\Managers\HardwareUpdateThread.cs`, and one unused event warning in `Execution\ProgramExecutor.cs`.

Record final validation evidence here after implementation. Include focused test output, full test output, manual validation notes, and Jira update identifiers if available.

## Interfaces and Dependencies

This feature uses existing WinForms and repository infrastructure only. No new external dependencies are required.

The expected new or changed interfaces are:

In `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs`, add:

    /// <summary>
    /// Gets or sets a value indicating whether interactive ruler height resizing is disabled.
    /// </summary>
    public bool LockRulerHeight { get; set; }

If using the recommended forwarding property, in `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`, add:

    /// <summary>
    /// Gets or sets a value indicating whether interactive ruler height resizing is disabled.
    /// </summary>
    public bool LockRulerHeight
    {
        get { return ruler.LockRulerHeight; }
        set { ruler.LockRulerHeight = value; }
    }

In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`, add a `ToolStripMenuItem` field named:

    private System.Windows.Forms.ToolStripMenuItem lockRulerHeightToolStripMenuItem;

Configure it with:

    CheckOnClick = true
    Text = "Lock Ruler Height"
    Click or CheckedChanged handler = lockRulerHeightToolStripMenuItem_Click

In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs`, add:

    private void lockRulerHeightToolStripMenuItem_Click(object sender, EventArgs e)
    {
        TimelineControl.LockRulerHeight = lockRulerHeightToolStripMenuItem.Checked;
    }

In `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`, add XML profile load and save using:

    string.Format("{0}/LockRulerHeight", Name)

The setting type is `XMLProfileSettings.SettingType.AppSettings`, matching the existing `RulerHeight` and other editor preferences.

## Change Log

- 2026-07-20 / Codex: Created initial ExecPlan from approved `docs/sequencer/vix-3945-lock-ruler-height.md` specification.
- 2026-07-20 / Codex: Completed Milestone 1 by adding Jira planning comment `40212` and recording the default-setting clarification.
- 2026-07-20 / Codex: Completed Milestone 2 by adding the ruler/timeline lock properties, guarding ruler height resize paths, and recording focused build evidence.

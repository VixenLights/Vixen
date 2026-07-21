# VIX-3948 Lock Waveform Height

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. It is self-contained so a future implementer can work from this file alone.

## Purpose / Big Picture

Vixen users can accidentally resize the Timed Sequence Editor waveform bar by brushing the bottom edge of the waveform and dragging when the resize cursor appears. After this change, the View menu has a persisted `Lock Waveform Height` option directly below `Lock Ruler Height`. The option defaults to off, so current waveform resize behavior is preserved for existing users. When the user enables it, the waveform keeps its current height, the resize cursor no longer appears at the waveform bottom edge, drag resizing is blocked, and double-click reset is blocked.

The visible result is in the Timed Sequence Editor. Open a sequence with audio, choose `View > Lock Waveform Height`, and move the pointer near the bottom edge of the waveform bar. With the option checked, the horizontal split cursor does not appear and dragging does not change the waveform height. Uncheck the option and the current resize behavior returns. The waveform lock is independent from `View > Lock Ruler Height`.

## Progress

- [x] (2026-07-21 00:00 -05:00) Created the feature specification at `docs/sequencer/vix-3948-lock-waveform-height.md`.
- [x] (2026-07-21 00:00 -05:00) Created this initial ExecPlan from the specification and the VIX-3945 ruler-height plan pattern.
- [x] (2026-07-21 11:37 -05:00) Milestone 1: Updated Jira VIX-3948 description with the refined specification, acceptance criteria, and test plan.
- [ ] Milestone 2: Add `LockWaveformHeight` to the reusable waveform surface and guard waveform resize cursor, drag resize, and double-click reset paths.
- [ ] Milestone 3: Add `View > Lock Waveform Height` directly below `Lock Ruler Height`, wire the menu handler, and persist `{Name}/LockWaveformHeight` through `XMLProfileSettings`.
- [ ] Milestone 4: Add focused automated coverage where practical.
- [ ] Milestone 5: Run automated validation, complete manual Timed Sequence Editor validation, update Jira with final evidence, and close out this ExecPlan.

## Surprises & Discoveries

- Observation: The tracked source folder is `src/Vixen.Common/Controls/TimeLineControl`, with a capital `L` in `Line`, while some docs and shorthand refer to `TimelineControl`.
  Evidence: Current source paths include `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` and `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`.

- Observation: VIX-3945 is already present in the working tree and provides the closest implementation pattern.
  Evidence: `docs/plans/sequencer/vix-3945-lock-ruler-height.md` records a completed `LockRulerHeight` property, `TimelineControl` forwarding property, menu item, XML setting, automated tests, and Jira update workflow.

## Decision Log

- Decision: The setting defaults to off and uses the stable XML app setting key `{Name}/LockWaveformHeight`.
  Rationale: The VIX-3948 specification requires VIX-3945 matching behavior, and VIX-3945 defaults the lock off while preserving the existing saved height.
  Date/Author: 2026-07-21 / Codex from user request and source review.

- Decision: Place `Lock Waveform Height` directly below `Lock Ruler Height` in the View menu.
  Rationale: The user explicitly requested this placement, and it groups the waveform lock with the matching ruler lock.
  Date/Author: 2026-07-21 / user direction recorded by Codex.

- Decision: The waveform lock applies only to interactive height resizing and not to waveform rendering, audio loading, visibility, cursor drawing, mark alignment drawing, or full/half waveform display.
  Rationale: The requested improvement is to prevent accidental resizing. Other waveform behavior is already separate and should remain unchanged.
  Date/Author: 2026-07-21 / Codex from source review.

- Decision: Block double-click waveform height reset while the lock is enabled.
  Rationale: Double-click reset changes waveform height through the same bottom-edge resize affordance. A locked height must not change from that interaction.
  Date/Author: 2026-07-21 / Codex from source review and VIX-3945 alignment.

- Decision: If the lock is enabled while a waveform resize drag is active, stop changing the height immediately with simple guard logic only.
  Rationale: This matches VIX-3945 and avoids extra state management for an unlikely interaction.
  Date/Author: 2026-07-21 / Codex from VIX-3945 alignment.

## Outcomes & Retrospective

Milestone 1 is complete. Jira VIX-3948 now has the refined specification, acceptance criteria, and test plan in the issue description. The Jira issue remains `In Progress`, with summary `Add waveform height lock option`, issue type `New Feature`, component `Editor/Sequencer`, and updated timestamp `2026-07-21T11:37:29.796-0500`.

Code implementation has not started. The intended outcome remains a narrow Timed Sequence Editor improvement: a persisted View menu toggle controls whether the waveform bar can be interactively resized. The implementation should stay local to the reusable waveform control and the editor settings/menu wiring.

## Context and Orientation

Vixen is a Windows desktop application for sequencing animated light shows. The Timed Sequence Editor is the main timeline editor. The timeline surface is built from reusable WinForms controls under `src/Vixen.Common/Controls/TimeLineControl`, and the editor host lives under `src/Vixen.Modules/Editor/TimedSequenceEditor`.

The waveform bar is the audio waveform display above the timeline grid. In this repository it is the `Common.Controls.Timeline.Waveform` class in `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`. It paints audio samples, draws the playback cursor, and draws mark-alignment indicators when marks are being moved elsewhere on the timeline. It also handles mouse input for waveform height resizing.

The current waveform height resize behavior is implemented in `Waveform.cs`:

- `OnMouseMove` sets `Cursor = Cursors.HSplit` when no mouse button is held and the pointer is within about 6 pixels of the waveform bottom edge.
- `OnMouseMove`, while the left button is held and the current cursor is `Cursors.HSplit`, changes `Height` when `e.Location.Y > MinimumHeight`.
- `OnMouseDoubleClick` resets `Height` to 50 when the cursor is `Cursors.HSplit`.

`Cursors.HSplit` is the horizontal split cursor that tells the user a horizontal boundary can be dragged vertically. In this feature, it is the resize indicator that must not appear when the waveform height is locked.

`TimelineControl` in `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` creates the public `waveform` instance. During construction it docks the waveform at the top of the timeline panel with a default height of 50 pixels. It also creates the `ruler` and already exposes `LockRulerHeight` from the VIX-3945 implementation.

The Timed Sequence Editor form is split across generated and hand-written files:

- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs` declares menu items and the View menu layout.
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` contains menu event handlers.
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` loads and saves XML profile settings and wires editor events.

Profile settings are stored through `XMLProfileSettings` app settings. In `TimedSequenceEditorForm.cs`, existing timeline settings are loaded near startup. The relevant neighboring settings include `FullWaveform`, `ResizeIndicatorEnabled`, `CadStyleSelectionBox`, `LegacyCursorActiveRow`, `WaveFormHeight`, `RulerHeight`, and `LockRulerHeight`. The existing waveform height is loaded with a default of 50 and saved when the editor closes. The new lock setting belongs beside these settings and must not replace `WaveFormHeight`.

The source specification for this work is `docs/sequencer/vix-3948-lock-waveform-height.md`. The matching prior feature specification is `docs/sequencer/vix-3945-lock-ruler-height.md`, and the matching prior implementation plan is `docs/plans/sequencer/vix-3945-lock-ruler-height.md`. If this ExecPlan and the VIX-3948 specification ever disagree, update both documents before continuing so future agents do not inherit contradictory requirements.

## Plan of Work

Milestone 1 updates Jira before code changes. Use the project `jira` skill by reading `.agents/skills/jira/SKILL.md` first. Read VIX-3948 and replace or update the issue description with the refined content in `Artifacts and Notes`: summary, specification, acceptance criteria, and test plan. If Jira tools are unavailable in the session, record that in `Surprises & Discoveries` and leave the paste-ready Jira text in `Artifacts and Notes`. The Jira update is part of the implementation workflow, not merely a final comment, because reviewers should be able to see the agreed requirements before code lands.

Milestone 2 adds the lock state to the reusable timeline control. Because the behavior belongs to the waveform itself, add a boolean property to `Waveform` named `LockWaveformHeight`. The default value is false. Since this is a public C# API, use the project `csharp-docs` skill before implementation and add XML documentation to the new property. The property should be simple and should not persist anything by itself.

Add a forwarding property to `TimelineControl`, also documented because it is public, so editor code can assign `TimelineControl.LockWaveformHeight` instead of reaching through `TimelineControl.waveform.LockWaveformHeight`. This mirrors the VIX-3945 `LockRulerHeight` pattern and keeps the Timed Sequence Editor settings code readable. The forwarding property should simply get and set `waveform.LockWaveformHeight` when `waveform` is not null.

Update `Waveform.OnMouseMove`. The existing drag resize path must continue to work when the lock is off. When the lock is on, the left-button resize path must not assign `Height`, and the no-button bottom-edge branch must not set `Cursor = Cursors.HSplit`. The locked bottom-edge cursor should fall back to `Cursors.Hand`, which is the current non-resize waveform cursor.

Update `Waveform.OnMouseDoubleClick`. Only reset height to 50 when `LockWaveformHeight` is false and the current cursor is `Cursors.HSplit`.

Milestone 3 wires the Timed Sequence Editor menu and persistence. In `TimedSequenceEditorForm.Designer.cs`, add a `ToolStripMenuItem` named `lockWaveformHeightToolStripMenuItem` with `CheckOnClick = true`, text `Lock Waveform Height`, and a click or checked-changed handler. Place it directly below `lockRulerHeightToolStripMenuItem` in the `viewToolStripMenuItem.DropDownItems` order. This means the View menu timeline display items should appear as `Full Waveform`, `Lock Ruler Height`, then `Lock Waveform Height`.

In `TimedSequenceEditorForm_Menu.cs`, add the handler. It should assign the checked state to the waveform lock property immediately:

    private void lockWaveformHeightToolStripMenuItem_Click(object sender, EventArgs e)
    {
        TimelineControl.LockWaveformHeight = lockWaveformHeightToolStripMenuItem.Checked;
    }

If no forwarding property is added to `TimelineControl`, assign `TimelineControl.waveform.LockWaveformHeight` instead and record that decision in `Decision Log`. The recommended path is the forwarding property.

In `TimedSequenceEditorForm.cs`, load `{Name}/LockWaveformHeight` near the existing waveform and ruler settings:

    lockWaveformHeightToolStripMenuItem.Checked = TimelineControl.LockWaveformHeight =
        xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockWaveformHeight", Name), false);

Save it near the existing `WaveFormHeight` and `LockRulerHeight` writes:

    xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockWaveformHeight", Name), lockWaveformHeightToolStripMenuItem.Checked);

Keep the existing `WaveFormHeight` load and save unchanged. Locking controls user interaction only; it does not force a default height and does not discard an existing custom height.

Milestone 4 adds focused automated coverage where practical. Start by inspecting the VIX-3945 tests, likely named around `RulerLockHeightTests`, and mirror their test style for waveform behavior if the current test framework can instantiate `Waveform` safely. If direct WinForms mouse simulation is brittle, keep automated coverage focused on deterministic property forwarding and any protected mouse behavior that can be invoked through a test subclass. Do not over-abstract just to test cursor state.

Practical tests may include a new `Waveform` defaults `LockWaveformHeight` to false, assigning `LockWaveformHeight` to true stores the property state, `TimelineControl.LockWaveformHeight` forwards to `waveform.LockWaveformHeight`, locked bottom-edge hover suppresses `Cursors.HSplit`, locked in-progress resize does not mutate `Height`, and locked double-click does not reset `Height`. If some of these require brittle UI automation, record the limitation in `Surprises & Discoveries` and cover them manually.

Milestone 5 validates and closes out. Run a focused test or compile first, then run the full test project. Manually validate in the Timed Sequence Editor using the scenarios in `Validation and Acceptance`. Update Jira VIX-3948 with implementation notes, automated test results, and manual validation results. Update this ExecPlan's `Progress`, `Surprises & Discoveries`, `Decision Log`, `Outcomes & Retrospective`, and `Artifacts and Notes` with the final evidence.

## Concrete Steps

Work from repository root `C:\Dev\Vixen`.

First inspect the current files:

    rg -n "LockWaveformHeight|LockRulerHeight|WaveFormHeight|Full Waveform|fullWaveformToolStripMenuItem|lockRulerHeightToolStripMenuItem|HSplit|OnMouseDoubleClick" src\Vixen.Common\Controls\TimeLineControl src\Vixen.Modules\Editor\TimedSequenceEditor docs\sequencer docs\plans\sequencer

Read required skills before editing public C# APIs or Jira:

    Get-Content -LiteralPath .agents\skills\csharp-docs\SKILL.md
    Get-Content -LiteralPath .agents\skills\jira\SKILL.md

Update Jira VIX-3948 using the Milestone 1 text in `Artifacts and Notes`. The target is the issue description, not only a comment. If Jira permissions or tools prevent editing the description, add a planning comment with the same content, record the blockage, and ask the user whether the description should be updated manually.

Implement `LockWaveformHeight` in `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`. Add XML documentation for the public property. Keep the default false value by relying on the default value for a bool auto-property.

Implement the recommended forwarding property in `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` near `LockRulerHeight`:

    /// <summary>
    /// Gets or sets a value indicating whether interactive waveform height resizing is disabled.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if users cannot resize the waveform height with the mouse; otherwise,
    /// <see langword="false" />. The default is <see langword="false" />.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool LockWaveformHeight
    {
        get { return waveform != null && waveform.LockWaveformHeight; }
        set
        {
            if (waveform != null)
            {
                waveform.LockWaveformHeight = value;
            }
        }
    }

Modify `Waveform.OnMouseMove`. The resulting intent should be:

    if (e.Button == MouseButtons.Left & !LockWaveformHeight & Cursor == Cursors.HSplit & e.Location.Y > MinimumHeight)
    {
        Height = e.Location.Y + 1;
    }
    else
    {
        Cursor = !LockWaveformHeight && e.Location.Y <= Height - 1 && e.Location.Y >= Height - 6 ? Cursors.HSplit : Cursors.Hand;
    }

Prefer `&&` over `&` only if doing so is consistent with the surrounding code and does not cause noisy unrelated formatting. The important behavior is that locked mode never assigns `Height` and never sets `Cursors.HSplit` from the waveform bottom-edge zone.

Modify `Waveform.OnMouseDoubleClick`:

    if (!LockWaveformHeight && Cursor == Cursors.HSplit)
    {
        Height = 50;
    }

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`. Add `lockWaveformHeightToolStripMenuItem` to the field list, instantiate it, configure it, wire the handler, and insert it into the View menu immediately after `lockRulerHeightToolStripMenuItem`.

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` with the menu handler.

Update `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` to load and save `LockWaveformHeight` beside the existing waveform and ruler settings. The setting key must be exactly `{Name}/LockWaveformHeight` unless this plan is revised with a documented decision.

Add tests if practical. If adding a new test class, place it near the VIX-3945 tests in `src/Vixen.Tests`. Name it clearly, for example `WaveformLockHeightTests`.

Run focused tests if added:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~WaveformLockHeight --no-restore

Run the full test project before final completion:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If the full test command fails because dependencies are not restored, rerun the restore/build command only if the environment permits it:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

If validation is blocked by missing native build dependencies or unavailable local tooling, record the exact command and error text in `Surprises & Discoveries` and `Artifacts and Notes`.

## Validation and Acceptance

Automated validation must include the full test project unless this environment cannot run it:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

Passing automated validation means the command exits with code 0 and reports no failed tests. If focused `WaveformLockHeightTests` are added, they should fail before the lock implementation and pass after it.

Manual acceptance must be performed in the Timed Sequence Editor:

1. Open a sequence with audio and with a fresh or missing `LockWaveformHeight` profile setting. Confirm `View > Lock Waveform Height` is unchecked and appears directly below `Lock Ruler Height`.
2. With the item unchecked, hover near the bottom edge of the waveform. The horizontal split cursor appears.
3. With the item unchecked, drag the waveform bottom edge. The waveform height changes.
4. With the item unchecked, double-click the waveform resize edge. The waveform height resets to 50 pixels.
5. Enable `View > Lock Waveform Height`. The waveform height does not change when the menu item is toggled.
6. With the item checked, hover near the bottom edge of the waveform. The horizontal split cursor does not appear.
7. With the item checked, drag near the bottom edge of the waveform. The waveform height does not change.
8. With the item checked, double-click near the bottom edge of the waveform. The waveform height does not reset.
9. With `Lock Waveform Height` checked and `Lock Ruler Height` unchecked, ruler height resizing still works.
10. With `Lock Waveform Height` unchecked and `Lock Ruler Height` checked, waveform height resizing still works.
11. With the item checked, the waveform still renders audio, cursor position, and mark alignment indicators as before.
12. Toggle `View > Full Waveform` while `Lock Waveform Height` is checked. Full/half waveform rendering changes without changing the waveform height lock state.
13. Close and reopen the Timed Sequence Editor. `View > Lock Waveform Height` retains the prior checked state.
14. Disable `View > Lock Waveform Height`, close and reopen the editor, and confirm the item remains unchecked and waveform resizing works again.

The core acceptance criterion is that enabling `Lock Waveform Height` prevents the resize cursor, drag resize, and double-click reset for waveform height while preserving non-resize waveform behavior, full/half waveform rendering, ruler lock independence, and XML profile persistence.

## Idempotence and Recovery

The source edits are additive and local. Running the implementation steps more than once should not create duplicate menu items or duplicate settings; verify with `rg` before adding anything. If a partially applied designer edit creates duplicate field declarations or duplicate `DropDownItems` entries, remove only the duplicate lines that correspond to this feature and keep unrelated designer content intact.

Do not revert unrelated working-tree changes. Before editing, check `git status --short` and treat existing changes as user work unless this plan or the current task clearly owns them. If a file already contains edits from VIX-3945, work with those edits because VIX-3948 intentionally builds on them.

The Jira update is safe to repeat if the description content is identical. If Atlassian tooling cannot update the description, add the paste-ready text from `Artifacts and Notes` as a comment and record that the description itself still needs manual update.

## Artifacts and Notes

Use this Jira summary:

    Add a persisted Timed Sequence Editor View menu option to lock waveform height and prevent accidental waveform resize.

Use this Jira description body for Milestone 1. If Jira supports Markdown, preserve the headings. If Jira requires Atlassian Document Format, keep the same sections and wording.

    h2. Specification

    VIX-3948 adds a Timed Sequence Editor preference that prevents accidental waveform height changes. Today the user can resize the audio waveform bar by hovering near the bottom edge of the waveform, getting the horizontal split cursor, and dragging vertically. Add a checked View menu option named Lock Waveform Height directly below Lock Ruler Height. The option defaults to unchecked/off for missing profile settings, so existing waveform resize behavior remains unchanged unless the user enables the lock.

    The waveform bar is implemented in src/Vixen.Common/Controls/TimeLineControl/Waveform.cs. Current height resizing is local to Waveform.OnMouseMove and Waveform.OnMouseDoubleClick. The parent timeline control is src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs. The Timed Sequence Editor menu and XML profile settings are in src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs, TimedSequenceEditorForm_Menu.cs, and TimedSequenceEditorForm.cs.

    The implementation should follow VIX-3945 Lock Ruler Height. Add a LockWaveformHeight or equivalent boolean property to the waveform surface, optionally forward it through TimelineControl, and persist it with XMLProfileSettings using the stable key {Name}/LockWaveformHeight. Keep the existing WaveFormHeight load and save unchanged.

    When Lock Waveform Height is unchecked, preserve current waveform resize behavior, including the horizontal split cursor, drag-to-resize behavior, the existing minimum-height threshold, double-click reset to 50 pixels, and saved WaveFormHeight. When checked, hovering over the waveform bottom edge must not show Cursors.HSplit, dragging near the bottom edge must not change waveform height, and double-clicking the resize edge must not reset waveform height. The lock must not change waveform painting, audio sample generation, visibility, cursor drawing, mark alignment drawing, Full Waveform display, ruler height behavior, grid row height behavior, sequence modified state, undo history, or sequence file contents.

    h2. Acceptance Criteria

    * View > Lock Waveform Height exists directly below View > Lock Ruler Height.
    * The item defaults to unchecked/off when the XML profile setting is missing.
    * The checked state persists between Timed Sequence Editor sessions using {Name}/LockWaveformHeight.
    * When unchecked, the waveform keeps current resize behavior: split cursor appears at the bottom edge, drag resize changes height, double-click reset changes height to 50 pixels, and WaveFormHeight remains persisted.
    * When checked, hovering over the waveform bottom edge does not show the split cursor.
    * When checked, dragging near the waveform bottom edge does not change waveform height.
    * When checked, double-clicking near the waveform resize edge does not reset waveform height.
    * The waveform lock is independent from Lock Ruler Height; either lock can be enabled without changing the other control's resize behavior.
    * Full Waveform still toggles half/full rendering while the waveform height lock is enabled.
    * Waveform rendering, cursor drawing, mark alignment drawing, audio assignment, sample generation, and visibility behavior continue to work.
    * The sequence is not marked modified, no undo history is created, and sequence file contents are not changed merely by toggling the lock.

    h2. Test Plan

    Automated validation: run dotnet test src/Vixen.Tests/Vixen.Tests.csproj --no-restore from the repository root. Add focused WaveformLockHeight tests where practical. Recommended coverage includes default unlocked state, property assignment, TimelineControl forwarding if added, locked bottom-edge cursor suppression, locked in-progress resize blocking, and locked double-click reset suppression. If WinForms mouse interaction cannot be tested reliably without brittle UI automation, record that limitation and rely on manual validation for cursor and drag behavior.

    Manual validation in the Timed Sequence Editor: open a sequence with audio; confirm View > Lock Waveform Height appears below Lock Ruler Height and defaults unchecked; verify unlocked hover, drag resize, and double-click reset still work; enable the lock and verify hover no longer shows the split cursor, drag does not resize, and double-click does not reset; verify Lock Ruler Height remains independent; verify Full Waveform still toggles rendering; close and reopen the editor to confirm persistence; disable the lock and reopen again to confirm resizing works.

Use this final Jira implementation comment after Milestone 5, filling in actual evidence:

    Implemented Lock Waveform Height.

    Summary:
    - Added waveform/timeline lock state for interactive waveform height resizing.
    - Added View > Lock Waveform Height directly below Lock Ruler Height.
    - Persisted {Name}/LockWaveformHeight with XMLProfileSettings.
    - Preserved WaveFormHeight and non-resize waveform behavior.

    Validation:
    - Automated: <command and result>
    - Manual: <manual validation summary>

Milestone 1 Jira update evidence:

    Issue: VIX-3948
    Summary: Add waveform height lock option
    Status after update: In Progress
    Updated: 2026-07-21T11:37:29.796-0500
    Description sections updated: Specification, Acceptance Criteria, Test Plan

## Interfaces and Dependencies

Use only existing WinForms and repository infrastructure. The lock state lives in `Common.Controls.Timeline.Waveform` and is consumed by existing mouse event overrides. The editor setting is persisted through the existing `XMLProfileSettings` API; do not add a new settings subsystem.

At the end of implementation, `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` should expose:

    /// <summary>
    /// Gets or sets a value indicating whether interactive waveform height resizing is disabled.
    /// </summary>
    public bool LockWaveformHeight { get; set; }

At the end of implementation, `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` should expose a documented forwarding property:

    public bool LockWaveformHeight { get; set; }

At the end of implementation, `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs` should declare and configure:

    private System.Windows.Forms.ToolStripMenuItem lockWaveformHeightToolStripMenuItem;

The `View` menu order must place `lockWaveformHeightToolStripMenuItem` immediately after `lockRulerHeightToolStripMenuItem`.

At the end of implementation, `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` should contain a handler that assigns:

    TimelineControl.LockWaveformHeight = lockWaveformHeightToolStripMenuItem.Checked;

At the end of implementation, `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` should load and save:

    {Name}/LockWaveformHeight

This key is an app setting and must default to false when missing.

## Revision Notes

- 2026-07-21 / Codex: Initial ExecPlan created from `docs/sequencer/vix-3948-lock-waveform-height.md`, the VIX-3945 ruler-height specification and implementation plan, and current source review. The plan explicitly includes Jira description update content because VIX-3948 should carry the refined spec, acceptance criteria, and test plan before implementation begins.

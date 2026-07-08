# Hide Locked Preview Shapes (VIX-3768)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

Preview Setup can become difficult to edit when a dense layout has many overlapping shapes. Today a user can lock a shape so it cannot be selected or moved accidentally, but locked shapes still visually cover other shapes. This change adds a transient Preview Setup view toggle that hides locked shapes from the visual canvas until the user turns the toggle off or closes Preview Setup.

After this work, a user can lock completed preview shapes, click `Hide Locked`, and edit only the remaining visible shapes as if the locked shapes do not exist on the canvas. The hidden shapes remain in the Preview Setup element tree and remain part of the saved preview data. This behavior applies only to Preview Setup's GDI setup canvas and does not affect the actual preview viewer, OpenGL preview viewer, sequence playback, or persisted preview data.

## Progress

- [x] (2026-07-08 19:47Z) Created this initial ExecPlan from `docs/preview/vix-3768-hide-locked-preview-shapes.md`, the user's specification answers, and a targeted review of Preview Setup lock, selection, and drawing code.
- [x] (2026-07-08 19:47Z) Applied the project `dotnet-design-pattern-review` skill for planning. The design keeps the new behavior scoped to Preview Setup view state and avoids changing the preview shape lock model or persisted data model.
- [x] (2026-07-08 19:50Z) Updated Jira issue `VIX-3768` with the refined specification, acceptance criteria, test plan, and ExecPlan path. The issue remains in `New Ticket`; no workflow transition was performed.
- [x] (2026-07-08 19:55Z) Updated `docs/preview/vix-3768-hide-locked-preview-shapes.md` so it is the canonical product specification; this ExecPlan now serves as the implementation plan for that specification.
- [x] (2026-07-08 20:18Z) Implemented the transient hide-locked state, toolbar toggle, Edit menu toggle, rendering filter, selection filter, and button/menu state updates.
- [x] (2026-07-08 20:18Z) Added generated `HideLocked.png` and `ShowLocked.png` resources under `src/Vixen.Modules/Preview/VixenPreview/Resources` and wired them through the preview module resources.
- [x] (2026-07-08 20:18Z) Added focused unit tests for the visibility, command-state, and selection-retention rules. `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~HideLockedPreview --no-restore` passed 12/12 tests.
- [x] (2026-07-08 20:18Z) Ran broader preview-filtered validation. `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~Preview --no-restore` passed 31/31 tests.
- [x] (2026-07-08 20:20Z) Ran preview module build validation. `dotnet build src\Vixen.Modules\Preview\VixenPreview\VixenPreview.csproj --no-restore` succeeded with 5 pre-existing warnings and 0 errors.
- [x] (2026-07-08 20:28Z) Manual Preview Setup verification confirmed by the user and recorded in Jira `VIX-3768`.

## Surprises & Discoveries

- Observation: The source issue asks for two image buttons, but the refined specification chooses one toggle because it better communicates the current state.
  Evidence: User answer on 2026-07-08: "The toggle button may be more obvious as to the state."
- Observation: Hidden locked shapes must be filtered from more than drawing.
  Evidence: User answer on 2026-07-08: hidden locked shapes should be excluded from mouse hit testing, marquee selection, drag operations, context menus, and keyboard actions so "it should appear like they do not exist."
- Observation: The current lock behavior is implemented as a shape property and already participates in selection rules.
  Evidence: `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs` exposes `Locked`; `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` has `DisplayItemAtPoint(...)` skip locked shapes unless selected or override selection is active.
- Observation: Adding the preview module reference to `Vixen.Tests` compiled successfully, so focused helper tests can live in the existing test project.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~HideLockedPreview --no-restore` built `VixenPreview` and passed 12/12 tests.

## Decision Log

- Decision: Implement this as a single toggle exposed on the lock toolbar section and as a corresponding toggle menu item under Edit.
  Rationale: A toggle button clearly communicates whether the canvas is currently hiding locked shapes. The menu item gives keyboard/menu users the same command surface as the toolbar.
  Date/Author: 2026-07-08 / Codex, from user specification answers.

- Decision: Use `Hide Locked` text/tooltip when locked shapes are visible and `Show Locked` when the toggle is active.
  Rationale: These labels are concise and were explicitly selected by the user.
  Date/Author: 2026-07-08 / Codex, from user specification answers.

- Decision: Keep hide-locked state transient and view-only.
  Rationale: The state must not be serialized, must reset when Preview Setup closes, and must not alter the existing lock/unlock undo history or preview data.
  Date/Author: 2026-07-08 / Codex, from user specification answers.

- Decision: Treat hidden locked shapes as absent from the visual canvas.
  Rationale: Hiding only the pixels would leave confusing invisible hit targets. The user explicitly wants hidden locked shapes excluded from drawing, information overlays, hit testing, marquee selection, dragging, context menus, and keyboard actions.
  Date/Author: 2026-07-08 / Codex, from user specification answers.

- Decision: Keep hidden locked shapes visible in the Preview Setup element tree and properties data model.
  Rationale: This feature is a canvas decluttering tool, not a data filter or preview membership change.
  Date/Author: 2026-07-08 / Codex, from user specification answers.

- Decision: Do not add undo/redo for hide/show.
  Rationale: The setting is transient view state, not a model mutation.
  Date/Author: 2026-07-08 / Codex, from user specification answers.

## Outcomes & Retrospective

The implementation milestone is complete. The code now keeps hide/show state transient in `VixenPreviewControl`, exposes one toolbar/menu toggle from `VixenPreviewSetup3`, filters hidden locked items from drawing, info overlays, hit testing, marquee selection, dragging, and edit commands, and adds focused tests for the pure visibility rules. Automated focused tests, broader preview-filtered tests, preview module build validation, and user-confirmed manual Preview Setup validation are complete.

## Context and Orientation

The relevant module is `src/Vixen.Modules/Preview/VixenPreview`, a WinForms/WPF preview module. Preview Setup is the editor used to place and configure preview shapes. The visual canvas is hosted by `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs`; it owns the current list of `DisplayItem` objects through `Data.DisplayItems`, tracks selected items, handles mouse interactions, and draws the edit-mode foreground through `RenderInForeground()`.

A `DisplayItem` is a wrapper around a preview shape. Its implementation is in `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs`. The underlying shape derives from `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs`. The shape has a public `Locked` property that already prevents normal selection and movement unless the existing override behavior is used. This plan does not change what `Locked` means. It only adds a view filter that hides shapes whose `Shape.Locked` property is true while the hide-locked toggle is active.

The Preview Setup frame is `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` with layout generated in `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.Designer.cs`. Existing lock toolbar buttons are `btnLock`, `btnUnlock`, and `btnUnlockAll`; their icons are loaded in the constructor from resources `Resources.locked`, `Resources.unlocked`, and `Resources.unlockedAll`. Existing Edit menu items include `lockToolStripMenuItem`, `unlockToolStripMenuItem`, and `unlockAllToolStripMenuItem`.

The GDI drawing path to update is `VixenPreviewControl.RenderInForeground()`. In edit mode it loops over all `DisplayItems` and calls `displayItem.Draw(...)`, then optionally loops over all `DisplayItems` again to draw `displayItem.DrawInfo(...)`. When hide-locked is active, both loops must skip locked display items. This is what hides both the shapes and their `Show Info` overlays.

The interaction paths to update are inside `VixenPreviewControl`. Important existing locations include `DisplayItemAtPoint(...)`, selection-band logic in the mouse-move handler, selected item movement, context-menu setup in mouse-down, delete/copy/cut operations, and lock/unlock operations. The implementation should prefer one shared predicate such as `IsDisplayItemVisibleOnCanvas(DisplayItem displayItem)` so every path answers the same question: if hide-locked is off, all display items are visible; if hide-locked is on, only unlocked display items are visible.

The actual live preview/viewer is outside this scope. Do not change `VixenPreviewModuleInstance` display behavior, `GDIPreviewForm`, `OpenGLPreviewForm`, playback rendering, serialized `VixenPreviewData`, or preview module data migration. The user-visible feature is only in Preview Setup.

## Plan of Work

Start by updating Jira `VIX-3768` with the refined specification from this plan, including acceptance criteria and the test plan. Use the project `jira` skill and the Atlassian connector. The Jira update should not claim implementation is complete.

Add a transient hide state to `VixenPreviewControl`, for example a public `bool HideLockedDisplayItems` property with a private backing field. When the property changes, it should deselect any currently selected locked shapes, clear the active single selection if it is locked, raise `OnSelectionChanged`, and call `EndUpdate()` or the local redraw path that is already appropriate for Preview Setup state changes. It must not write to `VixenPreviewData`.

Add one shared visibility predicate in `VixenPreviewControl`, for example `public bool IsDisplayItemVisibleOnCanvas(DisplayItem displayItem)` if tests or setup code need to call it, or an internal/private equivalent plus a small public/internal helper if testing requires isolation. The predicate should return false only when `HideLockedDisplayItems` is true and `displayItem.Shape.Locked` is true. Use this predicate anywhere Preview Setup's canvas enumerates display items for user-visible editing.

Update drawing first. In `RenderInForeground()`, skip hidden locked display items in the edit-mode shape drawing loop and in the `ShowInfo` overlay loop. This proves the core visual behavior before touching selection. Non-edit-mode rendering inside this method should not be expanded beyond Preview Setup requirements; if it is used by setup infrastructure, it may also use the predicate only when it is definitely setup-canvas rendering. Do not change the actual preview viewer forms.

Update mouse and selection behavior so hidden locked shapes act as absent. `DisplayItemAtPoint(...)` should ignore hidden items before calling `PointInShape`. Any loop that adds or removes items from `SelectedDisplayItems` based on a band rectangle should skip hidden items. `MouseOverSelectedDisplayItems(...)` should naturally stop finding hidden items after locked selected items are cleared when the toggle is enabled; still guard it with the predicate if it loops over selections that could become stale. Context-menu logic should not show a shape context menu for a hidden locked item. Keyboard and edit commands that operate on selected items should not be able to target hidden locked items because enabling the toggle clears them from selection; add filtering as a defensive measure where command paths enumerate `SelectedDisplayItems`.

Update lock/unlock interactions. When hide-locked is already active and the user locks another visible selected shape, it should disappear immediately after the lock operation, and selection should be cleared as it is today. When hide-locked is active and the user runs `Unlock All`, the newly unlocked shapes should immediately reappear while the hide toggle remains active. The toggle should not auto-turn off when no locked shapes remain.

Add the UI toggle. In `VixenPreviewSetup3.Designer.cs`, add a new button in `pnlLock` near the existing lock/unlock/unlock-all buttons. Use the same button style and sizing as the existing lock buttons. Add a matching Edit menu item near the lock/unlock menu items. The toolbar button and menu item represent the same state. When hide is off, both surfaces show `Hide Locked`; when hide is on, both show `Show Locked`. Keep their enabled state synchronized by extending `SetButtonEnabledState()`: when hide is off, enable the toggle only if any `previewForm.Preview.DisplayItems` has `Shape.Locked`; when hide is on, keep the toggle enabled even if no locked shapes currently exist. Continue to update existing lock/unlock/unlock-all button states as before.

Add icons as PNG resources under `src/Vixen.Modules/Preview/VixenPreview/Resources` and include them in the preview module resources in the same way as the existing `locked.png`, `unlocked.png`, and `unlockedAll.png` resources. The icons should visually match the existing toolbar style and communicate hidden/visible locked shapes. The implementation may use generated bitmap images if no suitable existing icon exists. Load the icon in `VixenPreviewSetup3` with `Tools.GetIcon(...)`, using the same `iconSize` path as the other lock buttons. If the toggle uses two visual states, load a hide icon when the action is `Hide Locked` and a show icon when the action is `Show Locked`.

Add focused tests. Prefer extracting a small pure helper if direct testing of `VixenPreviewControl` is too UI-heavy. A suitable helper can live in the preview module and answer questions such as: should this display item be visible when hide-locked is active, should the hide toggle be enabled, and which selected items should be retained when hide-locked turns on. If the helper is public or protected, use the project `csharp-docs` skill and write XML documentation. If the test project does not currently reference `VixenPreview.csproj`, add a project reference only if the preview module and test target build cleanly; otherwise move pure helper logic to an already referenced project only if that does not create an architectural dependency leak. The tests should cover locked and unlocked items with hide off, locked and unlocked items with hide on, toggle enabled behavior when no locked shapes exist, and selection cleanup when the toggle is enabled.

Validate manually in the application after automated tests. Create or open a preview with overlapping shapes. Lock at least one shape. Confirm `Hide Locked` becomes enabled, clicking it changes the label/icon to `Show Locked`, the locked shapes disappear from the canvas, cannot be clicked, cannot be marquee-selected, do not show info overlays, and do not participate in drag or context-menu operations. Confirm the same shapes remain in the Preview Setup element tree. Lock another visible shape while hide mode is active and confirm it disappears immediately. Run `Unlock All` while hide mode is active and confirm all shapes reappear but the toggle still says `Show Locked` until clicked. Close and reopen Preview Setup and confirm everything starts visible.

## Concrete Steps

From repository root `C:\Dev\Vixen`, update Jira first after this plan is written:

    Use the Atlassian connector to add a planning comment or update the issue body for VIX-3768.
    Include the refined specification, acceptance criteria, test plan, and this ExecPlan path:
    docs/plans/preview/vix-3768-hide-locked-preview-shapes.md

Research and edit these files:

    src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs
    src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs
    src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.Designer.cs
    src/Vixen.Modules/Preview/VixenPreview/Resources/<new hide/show icon files>
    src/Vixen.Modules/Preview/VixenPreview/Properties/Resources.resx
    src/Vixen.Modules/Preview/VixenPreview/Properties/Resources.Designer.cs
    src/Vixen.Tests/Vixen.Tests.csproj, only if a preview project reference is needed
    src/Vixen.Tests/Preview/<new focused test file>.cs

Run the focused tests from repository root:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~Preview

If the preview module reference makes the test build too broad or exposes unrelated failures, narrow the filter to the new test class name:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~HideLockedPreview

Run at least a compile validation if focused tests cannot cover UI designer/resource changes:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore --filter FullyQualifiedName~HideLockedPreview

For a broader build after implementation, use the repository's normal build command if time allows:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

Record exact commands and results in this plan's `Progress` and `Outcomes & Retrospective` sections.

## Validation and Acceptance

Acceptance criterion 1: Preview Setup opens with all shapes visible every time, regardless of the previous session's hide-locked state.

Acceptance criterion 2: The lock toolbar section contains one hide/show toggle button. The Edit menu contains a corresponding toggle item. Both surfaces stay synchronized.

Acceptance criterion 3: When at least one shape is locked and hide mode is off, the command is enabled and presented as `Hide Locked`. When the user activates it, locked shapes are hidden from the Preview Setup visual canvas, the command changes to `Show Locked`, and hidden locked shapes are deselected.

Acceptance criterion 4: When hide mode is on, hidden locked shapes are not drawn, do not show `Show Info` overlays, cannot be clicked, cannot be marquee-selected, cannot be dragged, do not receive context-menu actions, and do not receive keyboard/edit commands through stale selection.

Acceptance criterion 5: Hidden locked shapes remain visible in the Preview Setup element tree and remain in the preview data model.

Acceptance criterion 6: When hide mode is on and the user locks another visible shape, that shape disappears immediately.

Acceptance criterion 7: When hide mode is on and the user unlocks hidden shapes, including through `Unlock All`, those shapes reappear immediately. Hide mode remains on until the user toggles `Show Locked`.

Acceptance criterion 8: Hide/show state is not persisted and is not part of undo/redo history.

Acceptance criterion 9: Actual preview viewing, OpenGL preview viewing, sequence playback, saved preview data, and preview serialization are unchanged.

Automated tests should prove the pure visibility rules and command-state rules. Manual testing must prove the WinForms canvas behavior, toolbar/menu synchronization, generated icon appearance, and non-persistence across Preview Setup close/reopen.

## Idempotence and Recovery

The planned changes are additive and transient. Re-running tests is safe. If resource generation causes designer churn, keep only the resource entries required for the new icons and avoid unrelated formatting changes in `.resx` or `.Designer.cs` files. If the new test project reference to `VixenPreview.csproj` causes broad build failures, stop and document the blocker in `Surprises & Discoveries`; then extract the smallest pure helper into a more appropriate already referenced project only after confirming that does not invert dependencies.

Do not use destructive git commands. The repository may contain user changes; inspect `git status --short` before editing and preserve unrelated changes.

## Artifacts and Notes

Initial source issue:

    docs/preview/vix-3768-hide-locked-preview-shapes.md

This source issue document is the canonical product specification. Keep the user-facing requirements, acceptance criteria, and test plan there. Keep implementation sequencing, code orientation, risks, and progress tracking in this ExecPlan.

Key user decisions captured on 2026-07-08:

    Use one toggle button instead of two buttons.
    Add a corresponding Edit menu toggle.
    Use labels Hide Locked and Show Locked.
    Locking a shape while hide is active hides it immediately.
    Unlocking hidden shapes while hide is active shows them immediately.
    Enabling hide clears locked selections.
    Hidden locked shapes should behave as if absent from the visual canvas.
    Hidden locked shapes remain in the element tree.
    Show Info overlays are hidden for hidden locked shapes.
    Scope is Preview Setup only.
    Hide state resets only by user toggle or Preview Setup close.
    Hide/show is not undoable.
    Add focused unit tests plus manual verification.
    Use the Jira connector to update VIX-3768.

## Interfaces and Dependencies

The core interface at the end of implementation should be a small Preview Setup canvas state API on `VixenPreviewControl`. Use names that match existing code style. One acceptable shape is:

    public bool HideLockedDisplayItems { get; set; }
    public bool IsDisplayItemVisibleOnCanvas(DisplayItem displayItem)

If these members are public or protected, add XML documentation in the same change. The setter for `HideLockedDisplayItems` should be responsible for selection cleanup and redraw triggering, so toolbar/menu click handlers do not duplicate that logic.

`VixenPreviewSetup3` should own only UI synchronization: toggling `previewForm.Preview.HideLockedDisplayItems`, updating button/menu text, updating button/menu image, and calling `SetButtonEnabledState()`.

No new persistence field should be added to `VixenPreviewData`. No undo action should be added for hide/show. Existing `PreviewItemsLockUndoAction` remains responsible only for model lock state changes.

## Revision Notes

2026-07-08 / Codex: Created the initial ExecPlan from the source improvement note, targeted Preview Setup code review, project planning guidance, and user answers to refinement questions.

2026-07-08 / Codex: Updated Jira `VIX-3768` with the refined specification, acceptance criteria, test plan, and ExecPlan path before implementation so the tracker matches the planned scope.

2026-07-08 / Codex: Updated `docs/preview/vix-3768-hide-locked-preview-shapes.md` to contain the refined product specification directly. The ExecPlan remains focused on implementation details and points back to that specification as the source of truth.

2026-07-08 / Codex: Recorded user-confirmed manual Preview Setup validation after implementation and automated test validation completed.

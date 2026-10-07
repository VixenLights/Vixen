# VIX-3768 Hide Locked Preview Shapes

## Overview

Preview Setup can become difficult to edit when a dense layout has many overlapping preview shapes. Users can already lock shapes to prevent accidental movement or normal selection, but locked shapes still cover other shapes on the setup canvas.

Add a transient Preview Setup view toggle that hides locked preview shapes from the visual canvas. This is a canvas decluttering feature only. Hidden locked shapes remain in the Preview Setup element tree, remain in the preview data model, and continue to operate normally in the actual preview viewer and sequencing workflows.

This feature applies only to Preview Setup. It does not affect the actual preview viewer, OpenGL preview viewer, sequence playback, saved preview data, or preview serialization.

## Requirements

- Add one toggle button in the existing Lock / Unlock toolbar section.
- Add a corresponding toggle menu item near the existing Lock, Unlock, and Unlock All Edit menu items.
- When locked shapes are visible, the command text and tooltip must be `Hide Locked`.
- When hide mode is active, the command text and tooltip must be `Show Locked`.
- The toolbar button and Edit menu item must stay synchronized.
- The toggle must be enabled when hide mode is inactive and at least one shape is locked.
- The toggle must remain enabled while hide mode is active, even if no locked shapes currently remain.
- Add generated PNG icon resources that visually match the existing lock, unlock, and unlock-all toolbar icons.
- Preview Setup must always open with all shapes visible.
- Hide state must not be persisted and must not be part of undo / redo history.
- Hide state changes only when the user toggles the command or closes Preview Setup.
- Enabling hide mode must deselect any selected locked shapes.
- Hidden locked shapes must not be drawn on the Preview Setup visual canvas.
- Hidden locked shapes must not show `Show Info` overlays.
- Hidden locked shapes must not be clickable, marquee-selectable, draggable, available to context-menu actions, or available to keyboard/edit commands through stale selection.
- Hidden locked shapes must remain visible in the Preview Setup element tree.
- Hidden locked shapes must remain in the preview data model.
- If hide mode is active and the user locks another visible shape, that shape must disappear immediately.
- If hide mode is active and the user unlocks hidden shapes, including through `Unlock All`, those shapes must reappear immediately.
- `Unlock All` must not turn hide mode off. The user must toggle `Show Locked` to leave hide mode.
- Existing lock behavior must not be changed. Lock state is only used to determine which shapes are hidden.

## Acceptance Criteria

1. Preview Setup opens with all shapes visible every time, regardless of the previous session's hide state.
2. The lock toolbar section contains one hide/show toggle button, and the Edit menu contains a corresponding toggle item.
3. When at least one shape is locked and hide mode is off, the command is enabled and presented as `Hide Locked`.
4. Activating `Hide Locked` hides locked shapes from the Preview Setup visual canvas, changes the command to `Show Locked`, and deselects hidden locked shapes.
5. Hidden locked shapes are not drawn, do not show `Show Info` overlays, cannot be clicked, cannot be marquee-selected, cannot be dragged, do not receive context-menu actions, and do not receive keyboard/edit commands through stale selection.
6. Hidden locked shapes remain visible in the Preview Setup element tree and remain in the preview data model.
7. Locking another visible shape while hide mode is active hides that shape immediately.
8. Unlocking hidden shapes while hide mode is active makes them reappear immediately.
9. `Unlock All` leaves hide mode active until the user toggles `Show Locked`.
10. Hide/show state is not persisted and is not part of undo / redo history.
11. Actual preview viewing, OpenGL preview viewing, sequence playback, saved preview data, and preview serialization are unchanged.

## Test Plan

Automated focused tests should cover:

- Locked and unlocked display items with hide mode off.
- Locked and unlocked display items with hide mode on.
- Toggle enabled-state rules when there are no locked shapes, one locked shape, and hide mode already active.
- Selection cleanup when hide mode is enabled and selected items include locked shapes.

Prefer a small pure helper or predicate test if direct WinForms control testing is too fragile.

Manual Preview Setup testing should cover:

- Create or open a preview with overlapping shapes.
- Lock at least one shape and confirm `Hide Locked` becomes enabled.
- Click `Hide Locked` and confirm locked shapes disappear, the command text/icon changes to `Show Locked`, and hidden locked shapes cannot be selected, dragged, hit-tested, context-menued, or shown in info overlays.
- Confirm hidden shapes remain in the element tree.
- Lock another visible shape while hide mode is active and confirm it disappears immediately.
- Run `Unlock All` while hide mode is active and confirm all shapes reappear while the toggle remains in `Show Locked` state.
- Close and reopen Preview Setup and confirm all shapes start visible.

Validation commands:

```bash
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~HideLockedPreview
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter FullyQualifiedName~Preview
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
```

## Technical Notes

The preview module is located under `src\Vixen.Modules\Preview\VixenPreview`.

The implementation plan is maintained separately at `docs\plans\preview\vix-3768-hide-locked-preview-shapes.md`. This document is the product specification; the ExecPlan covers implementation sequence, code orientation, risks, and validation workflow.

The planned design keeps hide/show state as transient Preview Setup canvas state. It must not add fields to `VixenPreviewData` and must not add undo actions.

## Guidelines

Use the project skills `dotnet-best-practices`, `csharp-async`, and `csharp-docs` when doing coding.
Use the project skill `dotnet-design-pattern-review` when doing planning or designing.
Use `.agents\PLANS.md` for implementation planning.
Call out risks or concerns in the implementation plan.
Update Jira issue `VIX-3768` when the specification, acceptance criteria, test plan, or implementation outcome changes.

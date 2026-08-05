# Debounce Preview Setup zoom rendering

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document is maintained in accordance with `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

Dragging the Preview Setup zoom slider currently assigns `Preview.ZoomLevel` at every slider position. That assignment performs an expensive redraw, so dragging can feel slow. After this change, the slider thumb and displayed percentage remain immediate, while the preview redraw occurs only after 250 milliseconds without input or immediately when the mouse button is released. The final preview scale always matches the displayed percentage.

## Progress

- [x] (2026-08-05) Inspected the target form, its zoom event flow, repository planning requirements, and Jira project metadata.
- [x] (2026-08-05) Created VIX-3962 with the scope, design, acceptance criteria, and validation plan.
- [x] (2026-08-05) Added the component-owned trailing-edge debounce to `VixenPreviewSetup3.cs` only.
- [x] (2026-08-05) Rebuilt the Preview module and the full solution in Debug; both succeeded.
- [ ] Manually exercise Preview Setup interactions in the running application; this requires an interactive desktop session.
- [x] (2026-08-05) Added the build results and remaining interactive validation scenarios to VIX-3962.

## Surprises & Discoveries

- Observation: The form already adjusts the zoom tracker maximum based on process bitness, so the debounce must preserve its existing 25% minimum and 200%/400% maximum values rather than introduce new limits.
  Evidence: `VixenPreviewSetup3` sets `trackerZoom.Maximum` to 400 in a 64-bit process and 200 otherwise.

- Observation: This form imports WPF input types as well as WinForms types, so the new MouseUp handler must explicitly use `System.Windows.Forms.MouseEventArgs`.
  Evidence: The initial focused build reported CS0104 for `MouseEventArgs`; fully qualifying the WinForms type made the Preview module rebuild successfully.

## Decision Log

- Decision: Use `System.Windows.Forms.Timer` owned by the designer component container rather than a background timer.
  Rationale: Its Tick event runs on the UI thread, which owns the preview and controls, and disposal is handled with the form.
  Date/Author: 2026-08-05 / Codex

- Decision: Limit source edits to `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs`.
  Rationale: The expensive assignment originates at the form input boundary; changing preview rendering or the custom tracker would broaden risk without improving the coalescing behavior.
  Date/Author: 2026-08-05 / Codex

## Outcomes & Retrospective

The production change is complete: slider changes update the pending percentage and label immediately, one component-owned UI timer coalesces redraw requests after 250 milliseconds, and mouse release flushes the pending zoom without delay. External zoom changes and form closing stop pending work, while the equal-value guard avoids an unnecessary redraw. Both the focused Preview module and the full Debug solution rebuild completed successfully. Interactive UI verification remains for a desktop session because it cannot be reliably exercised in this non-interactive build environment.

## Context and Orientation

`src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` is the WinForms form used to configure a preview. Its `trackerZoom` control reports percentage values: 25 represents a zoom level of 0.25, 100 represents 1.0, and the existing maximum is 200 or 400 depending on process architecture. The form responds to external zoom changes through `VixenPreviewSetup3_ChangeZoomLevel`, such as mouse-wheel and keyboard commands emitted by the preview.

A trailing-edge debounce is a short delay which is restarted by every new input. It coalesces a continuous drag into one expensive operation after input settles. Here, the inexpensive UI feedback (the slider and percentage label) happens immediately; only the preview zoom assignment is delayed. A mouse release flushes the pending assignment immediately.

## Plan of Work

First create a VIX Jira Improvement entitled `Debounce Preview Setup zoom slider rendering`. Its description will explain the redraw problem, state that the only production file in scope is `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs`, and include the acceptance criteria in this plan. The issue must be updated at completion if validation changes the requirements, and receive a final validation comment.

In `VixenPreviewSetup3.cs`, alias `System.Windows.Forms.Timer` to avoid ambiguity, then add a 250-millisecond constant, a readonly component-owned timer, and an integer pending percentage initialized to 100. Immediately after `InitializeComponent()` construct `new Timer(components)`, set its interval, and subscribe its Tick event. Subscribe the zoom tracker’s `MouseUp` event there as well.

Replace the zoom tracker's direct `Preview.ZoomLevel` assignment. Its ValueChanged handler will copy the current percentage to the pending field, update `labelZoomLevel`, then stop and restart the timer. Add `ApplyPendingZoom`, which stops the timer, returns if `previewForm?.Preview` is unavailable, converts the pending percentage with `/ 100d`, and avoids assigning an equal zoom level before assigning the new value. The timer Tick handler and MouseUp handler both call it.

Update `VixenPreviewSetup3_ChangeZoomLevel` to stop pending slider work and synchronize the pending percentage from the external zoom before it updates the label and tracker. In `VixenPreviewSetup3_FormClosing`, stop the timer after the invalid-link validation has allowed closing and before the preview is reset to 1.0. This prevents a delayed Tick from applying zoom while shutdown is in progress.

## Concrete Steps

From `C:\Dev\Vixen`:

1. Create and document the Jira Improvement using the content of this plan.
2. Edit `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` as described in Plan of Work. Preserve tabs and existing formatting; do not edit the designer file or preview-control implementation.
3. Inspect the focused diff with:

       git diff --check
       git diff -- src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs docs/plans/preview/vix-preview-setup-zoom-debounce.md

4. Build the configured solution with:

       msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug

   Expect a successful Debug rebuild. If an unrelated pre-existing failure occurs, capture the first failing project and error and do not hide it.
5. Update this plan’s Progress and Outcomes sections, align the Jira description with final behavior, and add the build and manual-validation result to Jira.

## Validation and Acceptance

With Preview Setup open, continuously drag the zoom slider. The thumb and `labelZoomLevel` percentage must update at every position, while preview rendering is deferred. Release the mouse and verify that the preview immediately updates to the displayed percentage. Hold the pointer still while dragging for more than 250 milliseconds and verify one settled update; resume dragging and verify that the later value supersedes the earlier one.

Clicking the track must commit the clicked zoom on mouse release. Mouse-wheel and keyboard zoom must remain immediate, must update the slider and label, and must not later be overwritten by a pending slider value. Closing after changing the slider but before 250 milliseconds must not produce a delayed zoom callback. Existing limits remain 25% minimum, 400% maximum for 64-bit processes, and 200% maximum for 32-bit processes. The Debug build command in Concrete Steps must succeed.

## Idempotence and Recovery

Stopping an already-stopped WinForms timer is safe, so repeating the interaction and shutdown paths does not accumulate callbacks. The form component container disposes the timer with the form. If the edit needs to be reverted before committing, restore only the target file and this plan from version control; do not reset unrelated working-tree changes.

## Artifacts and Notes

The production change uses these private members and methods:

    private const int ZoomDebounceIntervalMilliseconds = 250;
    private readonly Timer _zoomDebounceTimer;
    private int _pendingZoomPercent = 100;

    private void ZoomDebounceTimerOnTick(object sender, EventArgs e)
    private void TrackerZoomOnMouseUp(object sender, MouseEventArgs e)
    private void ApplyPendingZoom()

The equality guard in `ApplyPendingZoom` is required because assigning an unchanged `Preview.ZoomLevel` still invokes an expensive redraw.

## Interfaces and Dependencies

No public API, serialized data, designer-generated field, module descriptor, or XML documentation changes are required. The implementation depends only on `System.Windows.Forms.Timer`, which is constructed with the existing `components` container and therefore uses the existing UI thread and form lifetime.

Plan revision: 2026-08-05. Created the initial implementation plan after source and Jira-project discovery so the issue can contain the finalized scope and acceptance criteria.

Plan revision: 2026-08-05. Implemented the debounce, qualified the WinForms MouseEventArgs type after the focused build exposed the WPF naming collision, and recorded successful Debug builds. Interactive acceptance checks remain pending a desktop session.

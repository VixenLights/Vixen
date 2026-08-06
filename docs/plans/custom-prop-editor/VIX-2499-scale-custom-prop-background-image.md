# Scale Custom Prop Editor Background Images

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document according to `.agents/PLANS.md` from the repository root. The associated Jira issue is VIX-2499, “Scale Custom Prop Background Image.”

## Purpose / Big Picture

After this change, a Custom Prop Editor user can choose View → Scale Background Image… and make a large background image fit a practical logical editor canvas without changing the bitmap stored in the `.prp` file. For example, a 4032×3024 photo scaled to 25% will display on a 1008×756 canvas, retain its original source pixels, and optionally move every light marker proportionally with the canvas.

The feature is intentionally a canvas-scaling tool, not an image editor. It does not crop, resample, rotate, zoom, or replace the stored bitmap. Its observable proof is the new modal dialog, the resized scrollable canvas, unchanged marker radii, and a save/reopen cycle that preserves the chosen canvas dimensions.

## Progress

- [x] (2026-08-05 00:00Z) Read `.agents/PLANS.md`, the Custom Prop Editor model, persistence service, editor view, view models, project files, existing dialog pattern, and test layout; no production files were changed.
- [x] (2026-08-06 13:27Z) Updated VIX-2499’s Jira description with the finalized requirements, acceptance criteria, implementation constraints, and validation commands; retained Normal priority and New Ticket status.
- [x] (2026-08-06 13:40Z) Implemented and unit-tested the pure scaling contracts and Catel modal dialog state. The module build succeeded and the focused `BackgroundImageScaling` suite passed 23 tests.
- [x] (2026-08-06 14:08Z) Implemented non-destructive model, persistence, canvas, and coordinate-scaling behavior. The module build succeeded; 35 focused scaling tests and 121 Custom Prop Editor tests passed.
- [x] (2026-08-06 15:03Z) Completed manual WPF acceptance testing. The menu and dialog worked for default and assigned backgrounds; aspect-lock/unlock, validation, cancel behavior, optional light scaling, and save/reopen persistence all behaved as specified. Recorded the result in Jira comment 40297.
- [x] (2026-08-06 14:24Z) Re-ran the exact targeted build and focused test commands: the build succeeded with 0 errors and the Custom Prop Editor filter passed 121 tests. VIX-2499 already matched the implementation; added Jira comment 40295 with the commands, results, existing warnings, and manual-verification follow-up.
- [x] (2026-08-06 14:24Z) Recorded automated validation evidence and the remaining live WPF acceptance exercise in this document.
- [x] (2026-08-06 14:51Z) Revised the aspect-lock contract in this plan and VIX-2499: it preserves the current logical canvas ratio, not the original bitmap ratio.
- [x] (2026-08-06 14:57Z) Corrected aspect lock to capture the current logical canvas ratio, added two regression tests, and re-ran validation: the module build succeeded with 0 errors and the focused Custom Prop Editor filter passed 123 tests. Added Jira comment 40296. The live WPF exercise remains pending.

## Surprises & Discoveries

- Observation: `Prop.Image` currently assigns `Height = _image.Height` and `Width = _image.Width` whenever a bitmap is attached. Since persistence deserializes dimensions first and attaches the image afterward, this destroys saved logical dimensions on reopening.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Model/Prop.cs` and `src/Vixen.Modules/App/CustomPropEditor/Services/PropModelPersistenceService.cs`.

- Observation: the existing canvas obtains its dimensions from `DrawingPanelViewModel.Width` and `Height`, which map directly to `Prop.Width` and `Height`; its `ImageBrush` has no explicit `Stretch` value.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`, lines 315–321.

- Observation: `Prop.GetLeafNodes()` may return the same leaf through multiple groups, whereas `DrawingPanelViewModel.LightNodes` is built from a dictionary keyed by leaf ID and is the bound edit surface.
  Evidence: `Prop.cs` documents possible duplicates in `GetLeafNodes`; `DrawingPanelViewModel.RefreshLightViewModels()` builds `_elementModelMap` by `ElementModel.Id`.

- Observation: Catel view models expose `RaisePropertyChanged`, rather than accepting a property name through `OnPropertyChanged`.
  Evidence: the initial module build reported `CS1503` for three computed result properties; replacing those calls with `RaisePropertyChanged(nameof(...))` produced a successful build.

- Observation: a non-uniform canvas resize uses independent X and Y factors; resizing 100×100 to 200×150 moves (10, 10) to (20, 15), not (20, 30).
  Evidence: the first coordinate-scaling test run caught the incorrect expected Y value; the corrected test passed with the intended `target/current` formulas.

- Observation: the available environment can compile WPF XAML and run the Custom Prop Editor tests, but does not provide an interactive Vixen application session for exercising the modal dialog directly.
  Evidence: the module build and 121-test Custom Prop Editor filter passed; live menu/dialog acceptance remains an explicit manual step.

- Observation: the final exact validation commands continue to pass after the workflow wiring changes.
  Evidence: on 2026-08-06, the module build exited 0 with no errors and the focused Custom Prop Editor test run reported 121 passed, 0 failed, and 0 skipped. Existing LiteDB NU1904 and unrelated compiler warnings remain.

- Observation: a source bitmap's ratio can differ from the current logical canvas ratio after an unlocked scale.
  Evidence: an 800×600 bitmap can be deliberately stretched to a 600×600 canvas. Reopening the dialog must retain the 1:1 canvas ratio when aspect lock is applied, so source dimensions cannot define the lock ratio.

- Observation: enabling aspect lock after an unlocked 640×400 edit must retain 640×400 rather than immediately changing it to the source image's 4:3 ratio.
  Evidence: the prior test expectation of 533×400 failed after the correction; updating it to the current 640×400 ratio and adding the 800×600-source/600×600-canvas regression produced 14 passing view-model tests.

- Observation: the completed interactive Custom Prop Editor exercise confirmed that the modal workflow applies only confirmed scaling and preserves the stored bitmap through save/reopen.
  Evidence: product-tester acceptance recorded in Jira comment 40297 on 2026-08-06.

## Decision Log

- Decision: Treat `Prop.Width` and `Prop.Height` as persisted logical canvas dimensions and keep `Prop.Image` as the original bitmap.
  Rationale: LiteDB already persists the two dimensions, so this gives non-destructive scaling and save/reopen compatibility without a new field, database migration, or format revision.
  Date/Author: 2026-08-05 / Codex

- Decision: Calculate percentage dimensions from `BitmapSource.PixelWidth` and `PixelHeight`; calculate aspect-lock dimensions from the logical canvas ratio current when the dialog opens or the user enables the lock; calculate light movement from the current canvas dimensions.
  Rationale: source pixels provide stable image-relative percentage sizing, while aspect lock must preserve the shape the user is currently editing, including a deliberately stretched canvas.
  Date/Author: 2026-08-06 / Codex

- Decision: Apply coordinate changes through distinct `LightViewModel` instances by setting `Center` once per light, rather than directly writing model X/Y values or iterating the potentially duplicated model traversal.
  Rationale: Catel’s `ViewModelToModel` mapping supplies model change notifications and dirty tracking. It also avoids applying the scale more than once to a grouped light.
  Date/Author: 2026-08-05 / Codex

- Decision: Keep scaling UI-thread confined and perform one O(n) pass only after the dialog returns affirmative.
  Rationale: the operation mutates UI-bound state and has no bitmap processing; parallel work adds synchronization risk without meaningful benefit.
  Date/Author: 2026-08-05 / Codex

- Decision: Replace the issue’s short free-form request with a structured implementation contract before coding.
  Rationale: this makes the intended behavior, compatibility rules, acceptance criteria, and test commands reviewable directly from VIX-2499 while preserving its existing priority and workflow state.
  Date/Author: 2026-08-06 / Codex

- Decision: Keep the pure scale unit, immutable options record, and calculator under `BackgroundImageScaling`, while locating the Catel view model under the existing `ViewModels` namespace.
  Rationale: the calculation contracts remain UI-independent, while the view model follows the module’s established view-model locator convention for the new Catel window.
  Date/Author: 2026-08-06 / Codex

- Decision: Treat finite dimensions from 1 through 100,000 as valid persisted logical canvas dimensions when attaching an image.
  Rationale: this matches the dialog’s validation range, preserves valid saved scale choices, and supplies native bitmap pixels only for incomplete or invalid legacy data.
  Date/Author: 2026-08-06 / Codex

- Decision: Apply scale options only after `IUIVisualizerService.ShowDialogAsync` reports an affirmative dialog result and the dialog exposes options.
  Rationale: this preserves cancel and close semantics without introducing a live preview or dirty-state mutation before the user confirms.
  Date/Author: 2026-08-06 / Codex

- Decision: Leave VIX-2499 in its existing New Ticket state after automated validation, and explicitly record the pending interactive acceptance exercise rather than treating compilation and unit tests as a substitute for it.
  Rationale: the feature's modal-dialog and save/reopen behavior still requires an interactive Vixen host to verify end to end.
  Date/Author: 2026-08-06 / Codex

- Decision: Reopen implementation work for the aspect-lock correction before treating the prior automated validation as final.
  Rationale: the previously implemented source-ratio calculation conflicts with the clarified product behavior for a canvas that was intentionally stretched while unlocked.
  Date/Author: 2026-08-06 / Codex

- Decision: Capture a lock-base width and height when aspect lock becomes active, and retain them while the lock remains active.
  Rationale: this provides stable current-canvas ratio behavior across width/height edits and unit switches while allowing a newly enabled lock to adopt a deliberately stretched canvas.
  Date/Author: 2026-08-06 / Codex

- Decision: Treat the completed product-tester exercise as final user-visible acceptance for this plan.
  Rationale: it verifies the WPF behavior that the automated calculator, view-model, persistence, and coordinate tests cannot observe directly.
  Date/Author: 2026-08-06 / Codex

## Outcomes & Retrospective

Milestones 1–3 are complete, and Milestone 4’s code is complete. The View menu now opens the scaling dialog and applies options only after confirmation; the canvas image uses explicit fill stretching. The confirmed scale updates the logical canvas and can proportionally move unique light centers without changing the stored bitmap or marker radius. Valid saved logical dimensions survive bitmap attachment and persistence tests cover save/reopen behavior.

Milestone 5 automated validation is complete for the implementation state that existed on 2026-08-06: the exact module build succeeded with 0 errors, and the focused Custom Prop Editor test command passed 121 tests with 0 failures or skips. Jira comment 40295 records those results. Existing LiteDB NU1904 and unrelated compiler warnings remain.

Milestone 6 corrects the clarified aspect-lock requirement. The dialog now captures the current logical canvas ratio when lock becomes active and uses it across subsequent edits; source dimensions remain limited to percentage conversion and display. The corrected module build succeeded with 0 errors and the focused Custom Prop Editor test command passed 123 tests with 0 failures or skips; Jira comment 40296 records the result.

The final manual Custom Prop Editor acceptance exercise has passed and is recorded in Jira comment 40297. The menu and dialog work for both default and assigned backgrounds; locking, unlocked stretching, validation, cancel behavior, and optional light scaling behave as expected; and save/reopen preserves the logical canvas without changing the stored bitmap. All planned validation is complete.

## Context and Orientation

The Custom Prop Editor module lives at `src/Vixen.Modules/App/CustomPropEditor`. A prop is a `Prop` object in `Model/Prop.cs`. The bitmap is deliberately excluded from LiteDB document serialization with `[BsonIgnore]`; `Services/PropModelPersistenceService.cs` stores it separately as `$/image/background.jpg` and attaches it to the deserialized prop when loading. Width and height are ordinary persisted `Prop` properties.

`ViewModels/PropEditorViewModel.cs` owns the editor workflow. It creates `DrawingPanelViewModel`, which maps its `Width`, `Height`, `Image`, and `Opacity` properties to `Prop`. `Views/CustomPropEditorWindow.xaml` hosts a scroll viewer containing the canvas and paints the image with an `ImageBrush`. `LightViewModel.Center` maps a WPF `Point` to the model light’s X/Y coordinate and is therefore the correct editor-facing route for moving a light.

“Logical canvas dimensions” means the width and height of the WPF editing surface, in editor pixels. They may differ from the native bitmap dimensions. “Source dimensions” means the immutable `BitmapSource.PixelWidth` and `PixelHeight`. The editor has a default blank bitmap, so a prop normally has an image even before a user chooses one; the Scale menu is available whenever `Prop.Image` is non-null.

All new scaling implementation types are internal and one type per file under the Custom Prop Editor module, with `InternalsVisibleTo("Vixen.Tests")` already declared in `CustomPropEditor.csproj`. No project or solution change is needed because this is an SDK-style WPF project that includes new C# and XAML files automatically.

## Plan of Work

### Milestone 1: Record the implementation contract in Jira

Before coding, use the repository’s `.agents/skills/jira/SKILL.md` and the configured Jira integration to update VIX-2499. Replace the ambiguous earlier proposal with this plan’s concrete user experience, 1–100,000 pixel validation, rounding rules, non-destructive persistence behavior, coordinate-scaling rules, and the two validation commands below. Preserve the issue’s normal priority and open state unless the product owner directs otherwise.

The issue description must say that assigning a new image starts at the new image’s native pixel dimensions, while reopening an existing prop retains valid saved logical dimensions. It must also state that no general image-editing behavior, bitmap resampling, added persisted fields, migration, or background concurrency is part of VIX-2499. Confirm the revised Jira description accurately names the acceptance scenarios in `Validation and Acceptance` before beginning Milestone 2.

Expected milestone completion response commit message, if repository files are changed in this milestone: `docs(custom-prop-editor): document VIX-2499 scaling requirements`. Do not create a commit unless explicitly requested.

### Milestone 2: Build the deterministic scale contract and dialog state

Create these internal, one-type-per-file C# contracts in a new `BackgroundImageScaling` folder beneath `src/Vixen.Modules/App/CustomPropEditor`:

- `BackgroundImageScaleUnit.cs`: `Pixels` and `Percent`.
- `BackgroundImageScaleOptions.cs`: an immutable result carrying integer `TargetWidth`, integer `TargetHeight`, and `bool ScaleExistingLightPositions`.
- `BackgroundImageScaleCalculator.cs`: a pure static calculator with named 1 and 100000 dimension limits. It converts a numeric input to pixels, applies aspect ratio, rounds with `MidpointRounding.AwayFromZero`, checks `double.IsFinite`, and validates final dimensions. It must not reference WPF controls, `BitmapSource`, a view model, or a `Prop`.
- `BackgroundImageScaleViewModel.cs`: Catel `ViewModelBase` state for the dialog. Its constructor accepts only source width/height, current canvas width/height, and whether the prop contains at least one unique light. It must not accept a `BitmapSource`, `Window`, or service locator.

Use canonical integer target dimensions in the view model. When its selected unit changes, calculate the displayed width and height from those canonical values: pixels display the dimensions directly; percent displays `target/source × 100`. Never calculate a new target from a prior rounded display value merely because the unit changed. This prevents cumulative rounding drift.

Bind editable numeric values as floating-point values so the calculator can reject NaN, infinity, zero, negative values, and dimensions outside the permitted range. Track the last edited width or height in private view-model state. Capture `lockedWidth` and `lockedHeight` from the canonical logical target dimensions when the dialog opens with its default lock enabled and whenever the user enables the lock. With aspect lock enabled, a width edit calculates `round(targetWidth × lockedHeight / lockedWidth, AwayFromZero)` and a height edit calculates `round(targetHeight × lockedWidth / lockedHeight, AwayFromZero)`. When the user enables the lock, first capture the then-current canonical dimensions and then recompute the opposite dimension from the last-edited dimension. With the lock disabled, preserve independently edited values so a stretch such as 640×400 is valid. Source dimensions must not be used for aspect-lock calculations.

Implement Catel `ValidateFields` and/or `ValidateBusinessRules` so invalid final dimensions produce field errors and disable the OK command through its `CanOk` method. Default `IsAspectRatioLocked` and `ScaleExistingLightPositions` to true. If no light exists, force the latter false, disable its control, and ensure the result carries false. `OkCommand` calls `SaveAndCloseViewModelAsync()` only when validation succeeds; `CancelCommand` calls `CancelAndCloseViewModelAsync()` and neither command changes a prop. Expose a non-null `Options` only after an accepted valid save.

Add `Views/BackgroundImageScaleWindow.xaml` and its `.xaml.cs` file. Follow `XModelSelectionView.xaml` for a Catel `<catel:Window>`, shared theme resources, centered modal window, and code-behind that only calls `InitializeComponent`. Bind text or numeric editors for width and height, a shared Pixels/Percent selector, aspect-lock and scale-lights check boxes, source/current/result dimension text, and OK/Cancel buttons. The UI must make the scale-lights check box disabled when the source has no lights.

Add `src/Vixen.Tests/App/CustomPropEditor/BackgroundImageScaling/BackgroundImageScaleCalculatorTests.cs` and `BackgroundImageScaleViewModelTests.cs`. Cover pixel conversion, 25% of 4032×3024 becoming 1008×756, a current 4:3 canvas changing locked width 640 to height 480, unlocked 640×400, and the regression where an 800×600 bitmap has a current 600×600 canvas and locked width 800 produces height 800. Also cover AwayFromZero midpoint behavior, unit switching without drift, lock-on using the last edit, all invalid cases, defaults, disabled light option, accepted options, and cancellation leaving the dialog result absent. The tests must instantiate the view model and calculator directly; they must not show a WPF window.

Expected milestone completion response commit message: `feat(custom-prop-editor): add background scaling dialog state`.

### Milestone 3: Preserve logical dimensions and apply a confirmed scale

Update `Model/Prop.cs`. Add XML documentation conforming to `.agents/skills/csharp-docs/SKILL.md` for the changed public `Image`, `Width`, and `Height` properties, including their relationship: width/height are logical canvas dimensions, and image attachment uses native pixel dimensions only as a fallback. Change `Image` so assigning a non-null, changed bitmap initializes width and height from `PixelWidth` and `PixelHeight` only when the existing dimensions are invalid. Treat non-finite, zero, negative, or out-of-range dimensions as invalid using the same 1–100000 range. A valid deserialized width/height must survive image attachment. Preserve normal property-change notifications.

Update `Services/PropModelServices.cs` in `SetImage`. After loading and cloning a user-selected `BitmapImage`, explicitly set `_prop.Width` and `_prop.Height` to the clone’s `PixelWidth` and `PixelHeight`, regardless of their prior values. Perform this after assigning `_prop.Image`, so new images begin at their native source size while the `Image` setter remains safe for loaded props. Do not alter the image loader’s cache behavior.

Add an internal `DrawingPanelViewModel.ApplyBackgroundImageScale(BackgroundImageScaleOptions options)` method. Validate its options and current dimensions defensively before mutation. Capture the current canvas width and height, then set the mapped `Width` and `Height` to the target dimensions. If `ScaleExistingLightPositions` is true, calculate `scaleX = targetWidth / currentWidth` and `scaleY = targetHeight / currentHeight`, select distinct entries from `LightNodes` by `Light.Id`, and set each `LightViewModel.Center` exactly once to `(oldX × scaleX, oldY × scaleY)`. Do not clamp an out-of-bounds point and do not write `Size`. Keep the work synchronous and on the caller’s UI thread.

Add tests in `CoordinateScalingTests.cs` and `PropImageDimensionPersistenceTests.cs`. Verify both coordinate factors, opt-out preservation, unchanged `Light.Size`, an already out-of-bounds coordinate remaining proportionally out of bounds, and a duplicated grouped light scaling only once. Verify a new image reset to its native dimensions, valid saved dimensions surviving image attachment, invalid/missing legacy dimensions falling back to native dimensions, and LiteDB save/reopen preserving a scaled canvas. Use a temporary unique `.prp` path and clean it up in `finally`/disposal so tests do not touch user data.

Expected milestone completion response commit message: `feat(custom-prop-editor): preserve and scale logical canvases`.

### Milestone 4: Wire the editor workflow and verify the user-visible feature

Update `ViewModels/PropEditorViewModel.cs` with a `TaskCommand ScaleBackgroundImageCommand`, a `CanScaleBackgroundImage` method, and an asynchronous handler. Follow the existing `IUIVisualizerService.ShowDialogAsync` usage, resolving the service through the established editor dependency resolver. `CanScaleBackgroundImage` returns true whenever `Prop?.Image` is non-null and must be reevaluated after a prop changes and when an image is assigned. The handler constructs `BackgroundImageScaleViewModel` using `Prop.Image.PixelWidth`, `Prop.Image.PixelHeight`, current `DrawingPanelViewModel.Width`/`Height`, and whether `DrawingPanelViewModel.LightNodes` has a unique light. Await the dialog, and only when its dialog result is `true` and its `Options` is valid call `DrawingPanelViewModel.ApplyBackgroundImageScale(options)`. A cancel, close, or invalid dialog makes no prop mutation and no editor dirty-state change.

Update `Views/CustomPropEditorWindow.xaml`. Insert `<MenuItem Header="Scale Background Image…" Command="{Binding ScaleBackgroundImageCommand}"/>` in the View menu immediately after Assign Background and before Background Opacity. Set the existing canvas `ImageBrush` explicitly to `Stretch="Fill"`; this makes a non-proportional logical width/height intentionally stretch the background rather than tile, crop, or preserve aspect ratio. Do not add a toolbar command unless product direction expands the requested scope.

Manually open the Custom Prop Editor and exercise both a large assigned photograph and its default blank background. Confirm the menu is enabled for both, result labels update, invalid values prevent OK, Cancel changes nothing, aspect-lock/unlock behavior matches the dialog, and selecting default light scaling moves positions while the visual marker radii remain unchanged. Save, close, reopen, and confirm the logical canvas dimensions remain scaled while the original bitmap content remains the background.

Expected milestone completion response commit message: `feat(custom-prop-editor): expose background image scaling`.

### Milestone 5: Validate, reconcile Jira, and close out the plan

From `C:\Dev\Vixen`, run the exact build and focused test commands in `Concrete Steps`. If either command fails because of a pre-existing environment or unrelated test failure, capture the complete relevant error, retry only after the cause is understood, and distinguish that from a VIX-2499 failure. Do not silently weaken the test filter or omit a failing test.

Use the Jira skill to compare VIX-2499’s description with the implemented behavior. Make any final wording corrections needed to align requirements, acceptance criteria, and test plan, then add a Jira comment containing the exact commands and concise pass/fail results. Update Progress, Surprises & Discoveries, Decision Log, Outcomes & Retrospective, and append a dated revision note at the end of this plan.

Expected milestone completion response commit message: `test(custom-prop-editor): cover background image scaling`.

### Milestone 6: Preserve the current logical canvas ratio when aspect lock is active

Correct `ViewModels/BackgroundImageScaleViewModel.cs` and the pure calculation support it uses so the aspect lock no longer derives a ratio from `sourceWidth` and `sourceHeight`. The source bitmap remains the basis for Pixels/Percent display and conversion, but it is not the basis for the lock. On dialog construction, initialize the lock ratio from the valid current logical canvas dimensions supplied to the view model. When the user turns the lock on after editing unlocked dimensions, capture the canonical target width and height at that moment as the new lock ratio before recalculating the opposite dimension from the most recently edited input. While the lock remains on, retain that captured ratio across subsequent width/height edits and unit changes.

Add a regression test that constructs the dialog state with source dimensions 800×600 and current dimensions 600×600, retains the default lock, changes width to 800, and asserts a target height of 800. Add a second test that starts unlocked, creates a stretched canonical target, turns the lock on, and proves the captured stretched ratio is retained. Keep the existing 4:3 case, but make it explicit that it describes a current 4:3 logical canvas rather than a source-image invariant.

Re-run the exact build and focused test commands in `Concrete Steps`. After the correction, use the Jira skill to add a final comment with the new validation result and update this plan’s Progress, Outcomes & Retrospective, and revision notes. The manual WPF exercise remains required before the overall feature is fully verified.

Expected milestone completion response commit message: `fix(custom-prop-editor): retain canvas aspect ratio`.

## Concrete Steps

All commands run from `C:\Dev\Vixen` in PowerShell.

1. Inspect the target module and tests before each edit so implementation does not overwrite unrelated user changes.

       git status --short
       rg -n "ScaleBackgroundImage|BackgroundImageScale|ImageBrush|public (BitmapSource Image|double Height|double Width)" src/Vixen.Modules/App/CustomPropEditor src/Vixen.Tests/App/CustomPropEditor

2. Build the module after Milestones 2–4.

       dotnet build src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore

   Expected result: MSBuild exits with code 0 and reports `Build succeeded` with no new warnings/errors attributable to VIX-2499.

3. Run the focused Custom Prop Editor tests after adding the test suite.

       dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor" --no-restore

   Expected result: the test run exits with code 0; all existing Custom Prop Editor tests and the new `BackgroundImageScaling` tests pass.

4. Perform the manual acceptance exercise described in Milestone 4 using a 4032×3024 image, a 4:3 image, a prop with lights, and a prop without lights. Record the observed target sizes and save/reopen result in the Jira comment and plan revision note.

## Validation and Acceptance

The feature is complete only when all of the following are demonstrably true:

- Choosing View → Scale Background Image… works whenever a prop has an image, including the default blank background, and the menu appears between Assign Background and Background Opacity.
- A 4032×3024 bitmap at 25% produces a 1008×756 logical canvas without changing the stored bitmap dimensions or resampling it.
- For a current 4:3 logical canvas, entering width 640 with aspect lock produces height 480. With aspect lock off, 640×400 is accepted and visibly stretches the background because the image brush uses `Stretch="Fill"`. If an 800×600 bitmap has already been stretched to a 600×600 logical canvas, reopening the dialog with the lock active and entering width 800 produces 800×800.
- A width or height conversion round uses `MidpointRounding.AwayFromZero`; NaN, infinity, zero, negative, and results below 1 or above 100000 disable OK and report Catel validation errors.
- Switching Pixels and Percent derives each display from the canonical target size and does not introduce repeated rounding drift. Re-enabling aspect lock uses the dimension edited most recently.
- With the default scale-lights option selected, every distinct light center is multiplied by target/current X and Y factors exactly once. With it cleared, all coordinates remain unchanged. Light marker size remains unchanged in both cases, and out-of-bounds coordinates are not clamped.
- Cancel, Escape, closing the dialog, and failed validation leave the prop and editor dirty state unchanged.
- Selecting a new background image resets the logical canvas to native `PixelWidth × PixelHeight`; valid dimensions persisted in an existing prop survive reopening; legacy props with missing or invalid saved dimensions fall back to the attached bitmap’s native pixel dimensions.
- The module build and focused test command both pass.

## Idempotence and Recovery

The plan changes only source, XAML, tests, Jira text, and this plan; it makes no data migration and does not rewrite existing `.prp` files during development. Re-running the build and test commands is safe. The dialog must have no live preview mutations, so cancellation is inherently recoverable.

If an implementation edit causes a failed build, correct or revert only the VIX-2499 edit after inspecting `git diff`; do not reset or discard unrelated working-tree changes. If a persistence test leaves a temporary `.prp` file, remove only the exact test-generated path after verifying it is within the test temporary directory. If old props reveal dimensions outside the documented range, preserve the file and rely on the new load-time fallback rather than attempting a bulk repair.

## Artifacts and Notes

The key data flow after implementation is:

    View menu
      → PropEditorViewModel.ScaleBackgroundImageCommand
      → IUIVisualizerService.ShowDialogAsync(BackgroundImageScaleViewModel)
      → validated BackgroundImageScaleOptions
      → DrawingPanelViewModel.ApplyBackgroundImageScale
      → Prop.Width / Prop.Height and distinct LightViewModel.Center values
      → existing LiteDB prop serialization plus separate original bitmap storage

The required formulas are:

    pixels = round(inputPixels, AwayFromZero)
    widthPixels = round(sourceWidth * widthPercent / 100, AwayFromZero)
    heightPixels = round(sourceHeight * heightPercent / 100, AwayFromZero)
    lockedHeight = round(targetWidth * lockBaseHeight / lockBaseWidth, AwayFromZero)
    lockedWidth = round(targetHeight * lockBaseWidth / lockBaseHeight, AwayFromZero)

    # lockBaseWidth and lockBaseHeight are the canonical logical canvas
    # dimensions captured when aspect lock becomes active.
    newX = oldX * (targetWidth / currentWidth)
    newY = oldY * (targetHeight / currentHeight)

## Interfaces and Dependencies

At the end of implementation, the following internal interfaces/types and integration points must exist in the Custom Prop Editor assembly:

    internal enum BackgroundImageScaleUnit
    {
        Pixels,
        Percent
    }

    internal sealed record BackgroundImageScaleOptions(
        int TargetWidth,
        int TargetHeight,
        bool ScaleExistingLightPositions);

    internal static class BackgroundImageScaleCalculator
    {
        // Exposes pure conversion, locked-dimension, rounding, and validation operations.
        // Valid final dimensions are integers from 1 through 100000 inclusive.
    }

    internal sealed class BackgroundImageScaleViewModel : ViewModelBase
    {
        // Receives source/current dimensions and has no WPF image/window dependency.
        // Exposes unit, inputs, aspect lock, scale-lights flag, result display,
        // validation, OkCommand, CancelCommand, and BackgroundImageScaleOptions.
    }

    internal void DrawingPanelViewModel.ApplyBackgroundImageScale(BackgroundImageScaleOptions options)

    public TaskCommand PropEditorViewModel.ScaleBackgroundImageCommand { get; }

The implementation must use Catel `TaskCommand` for awaiting the modal UI, `IUIVisualizerService` for dialog presentation, `ViewModelBase` property registration/validation for dialog state, and `SaveAndCloseViewModelAsync` / `CancelAndCloseViewModelAsync` for dialog completion. The implementation phase must explicitly read and follow `.agents/skills/catel-mvvm/SKILL.md` for the WPF/Catel changes and `.agents/skills/csharp-docs/SKILL.md` before changing the public `Prop` properties.

No new NuGet package, project reference, project item, project file, solution entry, persisted field, schema version, image editor, bitmap allocation/resampling pipeline, `AsParallel`, background task, or new synchronization mechanism is permitted.

Revision note (2026-08-05): Created from the VIX-2499 architecture handoff after repository research. This is a planning-only change; implementation has not begun.

Revision note (2026-08-06): Completed Milestone 1 by updating VIX-2499 with the executable specification while retaining its Normal priority and New Ticket status. No source implementation was performed.

Revision note (2026-08-06): Completed Milestone 2. Added the deterministic scaling contracts, Catel dialog state and view, and 23 focused passing tests. The module build passed with only the pre-existing LiteDB NU1904 dependency warning; the full Custom Prop Editor filter also passed 109 tests.

Revision note (2026-08-06): Completed Milestone 3. Preserved valid logical canvas dimensions during bitmap attachment, reset dimensions for a newly selected image, added optional unique-light coordinate scaling, and added persistence/coordinate tests. The module build passed; 35 focused scaling tests and 121 Custom Prop Editor tests passed. The LiteDB NU1904 dependency warning remains pre-existing.

Revision note (2026-08-06): Implemented Milestone 4’s command and XAML wiring. The module build and 121 Custom Prop Editor tests passed. Manual dialog/menu verification remains pending because no interactive Vixen host is available in this environment.

Revision note (2026-08-06): Completed Milestone 5 automated validation. The exact module build succeeded with 0 errors and the focused Custom Prop Editor filter passed 121 tests. VIX-2499 required no description corrections; Jira comment 40295 records the validation results and the still-pending interactive WPF exercise.

Revision note (2026-08-06): Clarified the product behavior for aspect lock. It preserves the current logical canvas ratio, including a deliberate unlocked stretch, rather than the original bitmap ratio. Updated VIX-2499 and added Milestone 6 for the required implementation correction and regression tests.

Revision note (2026-08-06): Completed Milestone 6. Aspect lock now captures and preserves the logical canvas ratio current when the lock is active. Added regressions for a 600×600 canvas backed by an 800×600 source and for re-enabling lock after an unlocked stretch. The module build passed and 123 focused Custom Prop Editor tests passed; Jira comment 40296 records the result. Live interactive verification remains pending.

Revision note (2026-08-06): Completed final manual WPF acceptance. Product testing confirmed the dialog and menu workflow for default and assigned backgrounds, aspect-lock/unlock behavior, validation, cancel safety, optional light scaling, and save/reopen persistence. Jira comment 40297 records the completion; all plan validation is now complete.

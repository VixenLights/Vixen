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
- [ ] Implement the non-destructive model, persistence, canvas, and light-coordinate changes.
- [ ] Add the View menu workflow and dialog view, then verify it manually.
- [ ] Run the targeted build and test commands, update VIX-2499 with final requirements if needed, and comment the validation results.
- [ ] Record the completed outcome, evidence, and any follow-up work in this document.

## Surprises & Discoveries

- Observation: `Prop.Image` currently assigns `Height = _image.Height` and `Width = _image.Width` whenever a bitmap is attached. Since persistence deserializes dimensions first and attaches the image afterward, this destroys saved logical dimensions on reopening.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Model/Prop.cs` and `src/Vixen.Modules/App/CustomPropEditor/Services/PropModelPersistenceService.cs`.

- Observation: the existing canvas obtains its dimensions from `DrawingPanelViewModel.Width` and `Height`, which map directly to `Prop.Width` and `Height`; its `ImageBrush` has no explicit `Stretch` value.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`, lines 315–321.

- Observation: `Prop.GetLeafNodes()` may return the same leaf through multiple groups, whereas `DrawingPanelViewModel.LightNodes` is built from a dictionary keyed by leaf ID and is the bound edit surface.
  Evidence: `Prop.cs` documents possible duplicates in `GetLeafNodes`; `DrawingPanelViewModel.RefreshLightViewModels()` builds `_elementModelMap` by `ElementModel.Id`.

- Observation: Catel view models expose `RaisePropertyChanged`, rather than accepting a property name through `OnPropertyChanged`.
  Evidence: the initial module build reported `CS1503` for three computed result properties; replacing those calls with `RaisePropertyChanged(nameof(...))` produced a successful build.

## Decision Log

- Decision: Treat `Prop.Width` and `Prop.Height` as persisted logical canvas dimensions and keep `Prop.Image` as the original bitmap.
  Rationale: LiteDB already persists the two dimensions, so this gives non-destructive scaling and save/reopen compatibility without a new field, database migration, or format revision.
  Date/Author: 2026-08-05 / Codex

- Decision: Calculate percent and aspect-lock dimensions from `BitmapSource.PixelWidth` and `PixelHeight`, but calculate light movement from the current canvas dimensions.
  Rationale: source pixels provide stable image-relative sizing while current dimensions preserve the intended scale ratio over repeated resizing.
  Date/Author: 2026-08-05 / Codex

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

## Outcomes & Retrospective

Milestones 1 and 2 are complete. VIX-2499 contains the final implementation contract, and the module now has a validated, UI-independent scaling calculator plus a Catel dialog state/view with focused tests. The feature is not yet reachable from the editor and does not yet modify a prop; Milestones 3–5 remain. The completed result must state whether the user can scale a background, whether `.prp` save/reopen retains the logical canvas size, the exact build/test outcomes, and any remaining limitations.

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

Bind editable numeric values as floating-point values so the calculator can reject NaN, infinity, zero, negative values, and dimensions outside the permitted range. Track the last edited width or height in private view-model state. With aspect lock enabled, a width edit calculates `round(targetWidth × sourceHeight / sourceWidth, AwayFromZero)` and a height edit calculates `round(targetHeight × sourceWidth / sourceHeight, AwayFromZero)`. When the user enables the lock, recompute the opposite dimension from the last-edited dimension. With the lock disabled, preserve independently edited values so a stretch such as 640×400 is valid.

Implement Catel `ValidateFields` and/or `ValidateBusinessRules` so invalid final dimensions produce field errors and disable the OK command through its `CanOk` method. Default `IsAspectRatioLocked` and `ScaleExistingLightPositions` to true. If no light exists, force the latter false, disable its control, and ensure the result carries false. `OkCommand` calls `SaveAndCloseViewModelAsync()` only when validation succeeds; `CancelCommand` calls `CancelAndCloseViewModelAsync()` and neither command changes a prop. Expose a non-null `Options` only after an accepted valid save.

Add `Views/BackgroundImageScaleWindow.xaml` and its `.xaml.cs` file. Follow `XModelSelectionView.xaml` for a Catel `<catel:Window>`, shared theme resources, centered modal window, and code-behind that only calls `InitializeComponent`. Bind text or numeric editors for width and height, a shared Pixels/Percent selector, aspect-lock and scale-lights check boxes, source/current/result dimension text, and OK/Cancel buttons. The UI must make the scale-lights check box disabled when the source has no lights.

Add `src/Vixen.Tests/App/CustomPropEditor/BackgroundImageScaling/BackgroundImageScaleCalculatorTests.cs` and `BackgroundImageScaleViewModelTests.cs`. Cover pixel conversion, 25% of 4032×3024 becoming 1008×756, 4:3 locked 640 becoming 640×480, unlocked 640×400, AwayFromZero midpoint behavior, unit switching without drift, lock-on using the last edit, all invalid cases, defaults, disabled light option, accepted options, and cancellation leaving the dialog result absent. The tests must instantiate the view model and calculator directly; they must not show a WPF window.

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
- For a 4:3 bitmap, entering width 640 with aspect lock produces height 480. With aspect lock off, 640×400 is accepted and visibly stretches the background because the image brush uses `Stretch="Fill"`.
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
    lockedHeight = round(targetWidth * sourceHeight / sourceWidth, AwayFromZero)
    lockedWidth = round(targetHeight * sourceWidth / sourceHeight, AwayFromZero)
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

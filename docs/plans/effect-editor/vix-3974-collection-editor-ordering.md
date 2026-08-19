# Add button-based ordering to the three approved Effect Editor collections

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document according to `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

VIX-3974 lets an Effect Editor user move the selected item one position earlier or later in exactly three ordered collections: color gradients, colors, and gradient-level pairs. The user will see textual **Up** and **Down** buttons next to the existing Add and Remove buttons. The commands are disabled at the list boundaries and preserve the moved logical item as the selection after the editor refreshes.

The change deliberately does not introduce drag reordering and does not alter any collection editor other than the three named templates. A user can prove the feature by selecting an item in each approved editor, moving it, observing the list order change and selection follow it, and confirming the first/last item disables the unavailable direction.

## Progress

- [x] (2026-08-19 14:08Z) Read `.agents/PLANS.md`, the three target templates, existing collection commands, collection mutation code, editor registrations, and the test project layout.
- [x] (2026-08-19 14:24Z) Updated VIX-3974 with final scope, acceptance criteria, exclusions, and validation plan; status remains Accepted.
- [x] (2026-08-19 14:31Z) Added the ordering commands and validated collection mutation path; CollectionView selection behavior remains for Milestone 3.
- [x] (2026-08-19 14:36Z) Registered and implemented shared move command routing, boundary enablement, and post-move selection retention in `CollectionView`.
- [x] (2026-08-19 14:46Z) Added Up/Down button opt-in to exactly the three approved palette/pair templates and verified the excluded templates remain unchanged.
- [x] (2026-08-19 14:52Z) Added pure internal move-seam tests, the narrow test visibility declaration, and an Effect Editor test project reference; focused suite passes 12/12.
- [x] (2026-08-19 15:15Z) Full build, unit tests, and the complete manual-validation scenario passed; updated VIX-3974 description and added a validation comment without changing its In Progress status.
- [ ] Add focused pure xUnit coverage and required test visibility/reference.
- [ ] Build, test, manually validate, reconcile VIX-3974, and record outcomes in this plan.

## Surprises & Discoveries

- Observation: The three approved `List<T>` types are already individually registered with their respective templates in `src/Vixen.Modules/Editor/EffectEditor/Editors/EditorCollection.cs`.
  Evidence: The registrations map `List<ColorGradient>`, `List<Color>`, and `List<GradientLevelPair>` to the three named editor keys on consecutive lines; no registration change is required.

- Observation: `CollectionView` owns the existing remove routed-command binding, while `PropertyItem` owns clone, assignment, change notification, and property-change publication for collection mutation.
  Evidence: `CollectionView` calls `PropertyItemValue.RemoveItemFromCollection`; `PropertyItem.RemoveCollectionValue` clones values, mutates the `IList`, calls `SetValueCore`, then raises `ValueChanged` and `PropertyValue` once.

- Observation: `src/Vixen.Tests/Vixen.Tests.csproj` does not currently reference the Effect Editor module.
  Evidence: Its project-reference list contains other editor modules but not `src/Vixen.Modules/Editor/EffectEditor/EffectEditor.csproj`.

- Observation: The affected module builds with six existing warnings outside the Milestone 2 changes.
  Evidence: Release build completed successfully with obsolete serialization-culture, formatter serialization, and unused-event warnings in existing `PropertyItemValue.cs`, `CollectionItemValue.cs`, and `MergedPropertyDescriptor.cs` code; no warning identifies a newly added member.

- Observation: Rebuilding the module can also rebuild dependencies and repeat their existing warnings.
  Evidence: The Milestone 3 Release build completed successfully while additionally reporting known warnings from `Vixen.Core.IElementTemplate`, `HardwareUpdateThread`, `ProgramExecutor`, and `FixtureGraphics.MovingHeadSettings`; no warning identifies `CollectionView.cs`.

- Observation: The Effect Editor test project reference must copy the module assembly to the test output directory.
  Evidence: A reference configured with `Private=false` compiled but all 12 focused tests failed with `FileNotFoundException` for `EffectEditor.dll`. Reverting to the repository's established bare project-reference form copied the assembly and the same 12 tests passed.

## Decision Log

- Decision: Opt in through Up/Down button presence in only the three approved XAML templates; do not add an `AllowReordering` dependency property.
  Rationale: A shared `CollectionView` is embedded by many templates. Command buttons are the smallest explicit opt-in and meet the approved scope without changing unrelated editor behavior.
  Date/Author: 2026-08-19 / Codex

- Decision: Put list reordering in `PropertyItem` and expose it through `PropertyItemValue`; keep `CollectionView` responsible only for routed-command availability, execution, and visual selection restoration.
  Rationale: `PropertyItem` already provides the repository's collection cloning, multi-value write, notification, and validation boundary. Keeping WPF selection code in the control prevents data and UI responsibilities from being mixed.
  Date/Author: 2026-08-19 / Codex

- Decision: Test an internal static `PropertyItem.TryMoveItem(IList, int, int)` directly, with `Vixen.Tests` granted access only when direct compilation requires it.
  Rationale: The requested tests concern deterministic `IList` ordering and object identity, not WPF. A narrow internal seam avoids reflection, a dispatcher, and public API expansion.
  Date/Author: 2026-08-19 / Codex

## Outcomes & Retrospective

All milestones are complete. VIX-3974 now delivers Up/Down ordering in exactly the approved color-gradient palette, color palette, and gradient-level pair editors. Movement is bounded, preserves selection and item identity, and leaves existing Add/Remove and drag/drop behavior unchanged. The full build, unit tests, and manual validation all passed; VIX-3974 contains the final user-facing scope and validation comment. The issue remains In Progress because no workflow transition was requested.

## Context and Orientation

The Effect Editor module is `src/Vixen.Modules/Editor/EffectEditor`. Its property grid presents effect properties through `PropertyItem`, which wraps a property descriptor and writes values back to one effect or to several selected effects. A `PropertyItemValue` is the presentation wrapper bound by XAML. For a collection property it materializes `CollectionItemValue` wrappers, exposes whether the property is editable, and delegates collection mutations back to `PropertyItem`.

`src/Vixen.Modules/Editor/EffectEditor/Controls/CollectionView.cs` is a WPF `ListView` subclass reused in several XAML templates. Its `PropertyValue` dependency property is bound to the `PropertyItemValue`; when that value changes it reloads `ItemsSource` from `CollectionValues`. It already registers `RemoveCollectionItem` once and clears selection when focus leaves. It is not subclassed by the three target editors.

The only approved visual opt-ins are these templates:

- `src/Vixen.Modules/Editor/EffectEditor/Themes/ColorGradientPaletteEditor.xaml`, for `List<ColorGradient>`.
- `src/Vixen.Modules/Editor/EffectEditor/Themes/ColorPaletteEditor.xaml`, for `List<Color>`. Its items render horizontally, but ordering remains the underlying list's previous/next index order.
- `src/Vixen.Modules/Editor/EffectEditor/Themes/GradientLevelPairEditor.xaml`, for `List<GradientLevelPair>`.

`GradientLevelPair` is the domain object containing its `Curve` and `ColorGradient`. For this feature it is one indivisible list item: movement must retain the same object reference and must never independently move, copy, or reconstruct either component.

`src/Vixen.Modules/Editor/EffectEditor/Input/PropertyEditorCommands.cs` is the static command catalogue. Its public `RoutedUICommand` properties are bound from XAML using the existing `input:PropertyEditorCommands` namespace. `PropertyItem.CloneValues()` creates the prior list state for single or merged multi-effect editing; `SetValueCore`, `OnValueChanged`, and `OnPropertyChanged("PropertyValue")` publish a property mutation. A routed command is WPF's command object that asks its target whether the operation is currently allowed before executing it.

The test project is `src/Vixen.Tests/Vixen.Tests.csproj` and uses xUnit v3. It does not yet reference Effect Editor. Keep the tests pure: construct ordinary mutable `IList` instances and call the internal move seam. Do not create a WPF visual test merely to test list ordering.

## Plan of Work

### Milestone 1: Align the Jira issue before code changes

Update VIX-3974 before implementation. The description must say that Up/Down applies only to `ColorGradientPaletteEditor` (`List<ColorGradient>`), `ColorPaletteEditor` (`List<Color>`), and `GradientLevelPairEditor` (`List<GradientLevelPair>`). State the operation precisely: Up moves the selected index to `index - 1`; Down moves it to `index + 1`; neither wraps. For the horizontal color palette, previous/next means list order, not a different visual axis.

Record acceptance criteria that the first selected item disables Up, the last disables Down, and empty, single-item, unselected, non-editable, invalid-parameter, and out-of-range cases disable or reject the command. Require selection retention after the collection wrappers reload, including multi-effect editing. State that the moved `GradientLevelPair` instance keeps its identity and components together. Include the pure test cases and manual checks described below.

Explicitly record exclusions: `TextEditor.xaml` and its `List<string>` editor, every other `EditorResources` collection template, editor registrations, `CollectionItemValue`, `ColorGradientEditor.xaml`, domain types, serialization, drag/drop infrastructure, `DragDropManager`, drag-advisor accepted/supported effects, and drag reordering. Do not transition the issue unless the normal workflow requires it.

### Milestone 2: Add the shared command and data mutation path

Before editing C#, read `.agents/skills/csharp-docs/SKILL.md` because `PropertyEditorCommands` gains two public properties, and follow its documentation requirements. Also follow `.agents/skills/dotnet-best-practices/SKILL.md` for the C# edits.

In `src/Vixen.Modules/Editor/EffectEditor/Input/PropertyEditorCommands.cs`, add two private `RoutedUICommand` fields with user-visible text `Move Collection Item Up` and `Move Collection Item Down`, then add documented public `RoutedUICommand` properties named exactly `MoveCollectionItemUp` and `MoveCollectionItemDown`. Their command names should be stable counterparts such as `MoveCollectionItemUpCommand` and `MoveCollectionItemDownCommand`. Preserve all existing commands and documentation.

In `src/Vixen.Modules/Editor/EffectEditor/PropertyItemValue.cs`, add internal `CanMoveItemInCollection(int sourceIndex, int targetIndex)` and `MoveItemInCollection(int sourceIndex, int targetIndex)`. The first must return false unless the wrapped property is a writable collection with an underlying list and valid, distinct source and target indexes. The second must delegate to `PropertyItem.MoveCollectionValue` and return its Boolean result. It must not move a `CollectionItemValue` wrapper, clone an item, or issue two per-index replacement writes.

In `src/Vixen.Modules/Editor/EffectEditor/PropertyItem.cs`, add an internal Boolean `MoveCollectionValue(int sourceIndex, int targetIndex)` and a pure internal static Boolean test seam `TryMoveItem(IList collection, int sourceIndex, int targetIndex)`. `TryMoveItem` must reject null lists, negative indexes, equal indexes, source indexes outside `0..Count-1`, and target indexes outside `0..Count-1`, returning false without modifying the list. For valid values, it captures the object at `sourceIndex`, calls `RemoveAt(sourceIndex)`, then calls `Insert(targetIndex, capturedItem)`, and returns true. That exact remove/insert sequence implements destination indexes in the original list: moving index 1 to 2 changes `[A, B, C]` to `[A, C, B]`.

`MoveCollectionValue` must first reject non-collections, read-only properties, null lists, and invalid/equal indexes before it calls `CloneValues()`. Use the same bounds rule as `TryMoveItem` so clone capture cannot occur for a rejected move. For a valid move, clone once, invoke `TryMoveItem` once, call `SetValueCore(collectionValue)` once, call `OnValueChanged(oldValues, GetValue())` once, call `OnPropertyChanged("PropertyValue")` once, and return true. If the helper returns false, do not write or notify and return false. This preserves the existing merged-property undo/change semantics while retaining list-object identity and every contained item's identity. Do not call `SetCollectionValue` twice and do not clone collection item values.

### Milestone 3: Bind and execute the commands in the shared CollectionView

Edit `src/Vixen.Modules/Editor/EffectEditor/Controls/CollectionView.cs`. In the constructor, register `MoveCollectionItemUp` and `MoveCollectionItemDown` once, alongside the existing remove binding. Each `CanExecute` handler must require all of the following: the command parameter is non-null and equals this control's `PropertyValue`; it can be cast to `PropertyItemValue`; the value is editable; `SelectedIndex` is selected; the collection count is available; and the calculated target (`SelectedIndex - 1` for Up, `SelectedIndex + 1` for Down) is within `0..count-1`. Delegate final validation to `CanMoveItemInCollection`. Set `CanExecute` false and mark the routed event handled for every failure, including null or mismatched parameters.

Each execute handler must recalculate the target from the current `SelectedIndex`, call `MoveItemInCollection`, and do nothing further when it returns false. On success, set `SelectedIndex` to the target after the property wrapper reload, call `ScrollIntoView(SelectedItem)`, and request WPF command-state reevaluation with `CommandManager.InvalidateRequerySuggested()`. The final selected `CollectionItemValue` wrapper must represent the same logical list item that was selected before the move; setting selection by target index after the `PropertyValue` reload achieves that. Do not add an `AllowReordering` dependency property, do not alter the focus-loss behavior, and do not change remove/add behavior.

### Milestone 4: Opt in only the three approved templates

In each of these files—`ColorGradientPaletteEditor.xaml`, `ColorPaletteEditor.xaml`, and `GradientLevelPairEditor.xaml`—add textual buttons with `Content="Up"` and `Content="Down"` immediately after the existing Add and Remove buttons in the existing horizontal `WrapPanel`. Use the existing margin, alignment, padding, and editable visibility convention. Each new button must set all of the following:

- Its appropriate command: `input:PropertyEditorCommands.MoveCollectionItemUp` or `input:PropertyEditorCommands.MoveCollectionItemDown`.
- `CommandParameter="{Binding}"`.
- `CommandTarget="{Binding ElementName=PART_editor}"`.
- `FocusManager.IsFocusScope="True"`.
- The existing `IsEditable` to `BooleanToVisibilityConverter` visibility binding.

The `CommandTarget` is required because the buttons sit outside the `CollectionView` in the template visual tree. Do not add buttons, bindings, or changes to `TextEditor.xaml`, `EditorResources.xaml`, `Generic.xaml`, `EditorCollection.cs`, `CollectionItemValue.cs`, `ColorGradientEditor.xaml`, or any drag/drop code.

### Milestone 5: Add pure ordering tests and build evidence

Add `src/Vixen.Modules/Editor/EffectEditor/EffectEditor.csproj` as a project reference of `src/Vixen.Tests/Vixen.Tests.csproj`, following the repository's project-reference conventions. Because `TryMoveItem` is internal and direct tests are required, expose only this assembly relationship to `Vixen.Tests`: add an `InternalsVisibleTo("Vixen.Tests")` declaration using the project’s established SDK-compatible assembly-attribute convention. If build discovery shows the test assembly can access the internal helper without a new declaration, do not add one; do not make the helper public and do not use reflection.

Create `src/Vixen.Tests/EffectEditor/PropertyItemCollectionMoveTests.cs`. Test `PropertyItem.TryMoveItem` with ordinary `List<object>` or similarly simple `IList` values, using named reference objects where identity matters. Include focused xUnit facts for:

- Moving an interior item upward produces the exact expected order and returns true.
- Moving an interior item downward produces the exact expected order and returns true.
- The moved object is the same reference at its destination, proving no item clone/reconstruction; include a `GradientLevelPair` instance if its module dependencies are readily available, otherwise use a unique reference object and retain the manual `GradientLevelPair` check.
- Negative source/target indexes, equal indexes, target equal to count, source equal to count, and source/target past the end each return false and leave the list unchanged.
- Empty and single-item lists return false for invalid/no-op requests and remain unchanged.

Keep these tests independent of a dispatcher and Effect Editor UI. Add only test visibility/reference needed to compile direct access. If a focused test is added for the property wrapper's single notification, make it a narrowly scoped non-WPF test; otherwise use the manual property-notification observation below and do not fabricate a complicated descriptor fixture.

### Milestone 6: Validate behavior and close the tracker loop

From `C:\Dev\Vixen`, first build the affected module and then run the repository-standard unit-test pipeline:

    msbuild src/Vixen.Modules/Editor/EffectEditor/EffectEditor.csproj -m -restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -v:m
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

Expect each command to exit zero. The full test target is necessary because its C++/CLI transitive projects require full MSBuild before `dotnet test --no-build`. Investigate errors and all new warnings; record unrelated baseline warnings separately rather than accepting a newly introduced warning.

Manually test all three approved editors, using a property with at least three distinguishable values. Select a middle value, press Up, then Down, and verify exact list ordering and selected-item retention after each operation. For `ColorPaletteEditor`, verify the meaning is still prior/next list order despite its horizontal presentation. Select the first item and confirm Up disables; select the last and confirm Down disables. Confirm both buttons disable for empty, one-item, unselected, and read-only lists. Repeat the key scenarios while multiple effects are selected, ensuring their values receive the same ordering operation and the selected logical item remains selected.

For a `GradientLevelPair` collection, record the selected pair's object identity and verify that the same pair, with its original `Curve` and `ColorGradient`, moved as one unit. Test the existing Add and Remove buttons in each target template to ensure no regression. Use a debugger breakpoint, event counter, or focused instrumentation to verify one successful move causes exactly one `PropertyItem.ValueChanged` call and one `PropertyChanged` event whose name is `PropertyValue`.

Finally update VIX-3974 with the delivered scope, the commands run and their results, automated-test count, manual observations, and any limitation. Add a Jira comment containing the validation evidence. Reconcile this document's living sections and add a dated revision note. Do not create a commit unless explicitly asked. When an implementation milestone that changes repository files completes, invoke the project `commit-msg` skill and report this candidate message:

    feat(effect-editor): add palette item ordering controls

## Concrete Steps

Run these read-only discovery commands from `C:\Dev\Vixen` before editing; they are safe to repeat:

    rg -n -C 8 "MoveCollection|RemoveCollectionItem|CanRemove" src/Vixen.Modules/Editor/EffectEditor
    rg -n -C 8 "CloneValues|AddCollectionValue|RemoveCollectionValue|SetCollectionValue" src/Vixen.Modules/Editor/EffectEditor/PropertyItem.cs
    rg -n -C 6 "ColorGradientPaletteEditorKey|ColorPaletteEditorKey|GradientLevelPairEditorKey" src/Vixen.Modules/Editor/EffectEditor/Editors
    rg -n "InternalsVisibleTo|STAThread|Dispatcher" src/Vixen.Tests src/Vixen.Modules/Editor/EffectEditor --glob '*.cs' --glob '*.csproj'

After the edits, prove that scope did not expand:

    git diff --check
    git diff --name-only
    rg -n "MoveCollectionItem(Up|Down)" src/Vixen.Modules/Editor/EffectEditor
    rg -n "Content=\"(Up|Down)\"" src/Vixen.Modules/Editor/EffectEditor/Themes/ColorGradientPaletteEditor.xaml src/Vixen.Modules/Editor/EffectEditor/Themes/ColorPaletteEditor.xaml src/Vixen.Modules/Editor/EffectEditor/Themes/GradientLevelPairEditor.xaml
    git diff -- src/Vixen.Modules/Editor/EffectEditor/Themes/TextEditor.xaml src/Vixen.Modules/Editor/EffectEditor/Themes/EditorResources.xaml src/Vixen.Modules/Editor/EffectEditor/Editors/EditorCollection.cs src/Vixen.Modules/Editor/EffectEditor/Internal/CollectionItemValue.cs src/Vixen.Modules/Editor/EffectEditor/Themes/ColorGradientEditor.xaml

Expected result: `git diff --check` reports no whitespace errors; command references occur in the command catalogue, `CollectionView`, and only the three approved templates; and the final scoped `git diff` is empty. Then run the build and test commands in Milestone 6.

## Validation and Acceptance

Acceptance requires all automated checks to pass and the following behavior to be demonstrated manually:

- Exactly the three approved palette/pair editors display Up and Down. `TextEditor` and all other collection templates do not gain ordering controls.
- Up moves the selected item from index `n` to `n - 1`; Down moves it from `n` to `n + 1`; neither wraps around.
- Commands cannot execute for a nonmatching or null parameter, noneditable property, no selection, empty list, single item, first-item Up, last-item Down, or invalid target index.
- After a successful move, the moved logical item is selected at its destination and scrolled into view, including after wrapper reload and while editing multiple effects.
- `GradientLevelPair` moves by reference as a complete pair. Its curve and gradient are not split, cloned, or independently reordered.
- One successful move clones old property values once, writes the collection once, raises `ValueChanged` once, and raises `PropertyChanged("PropertyValue")` once.
- Existing Add/Remove functionality remains usable in all three templates. Existing drag/drop behavior and the accepted/supported drag effects remain unchanged.
- The three build/test commands in Milestone 6 exit successfully, and all new pure xUnit cases pass.

## Idempotence and Recovery

The source changes are additive and may be safely rebuilt and retested repeatedly. `TryMoveItem` rejects no-op and invalid moves without mutation, so rerunning edge tests cannot alter persistent data. Manual tests should use a disposable sequence/effect or close without saving after experimentation; reordering an effect property is intentionally an editable change.

If a command remains enabled at a boundary, inspect the `CanExecute` parameter equality, `SelectedIndex`, underlying collection count, and calculated target before adding new dependency properties. If selection does not follow the moved item, inspect whether `PropertyValue` reloads the `ItemsSource` before setting `SelectedIndex`; retain the target-index selection sequence rather than holding an obsolete `CollectionItemValue` wrapper. If direct internal test access fails, use the minimal `InternalsVisibleTo` declaration described in Milestone 5 and rebuild; do not widen production visibility. If a list implementation does not support `RemoveAt`/`Insert`, treat the operation as non-executable and record that constraint rather than partially writing it through another mutation path.

## Artifacts and Notes

The intended successful list transition is:

    sourceIndex = 1, targetIndex = 2, list = [A, B, C]
    capturedItem = list[1]             // B, same object reference
    list.RemoveAt(1)                   // [A, C]
    list.Insert(2, capturedItem)       // [A, C, B]

The data and UI transition is:

    CollectionView command executes for selected index n
    PropertyItemValue.MoveItemInCollection(n, target)
    PropertyItem clones pre-move values once, moves the existing item, writes once, and notifies once
    PropertyItemValue reloads collection wrappers through its existing PropertyValue notification path
    CollectionView selects target and ScrollIntoView(SelectedItem)
    CommandManager re-queries so the new boundary state is visible

Files expected to change during implementation are limited to:

- `src/Vixen.Modules/Editor/EffectEditor/Input/PropertyEditorCommands.cs`
- `src/Vixen.Modules/Editor/EffectEditor/Controls/CollectionView.cs`
- `src/Vixen.Modules/Editor/EffectEditor/PropertyItemValue.cs`
- `src/Vixen.Modules/Editor/EffectEditor/PropertyItem.cs`
- The three approved XAML templates named in Context and Orientation
- `src/Vixen.Tests/Vixen.Tests.csproj` and the narrowly needed Effect Editor assembly-visibility declaration
- `src/Vixen.Tests/EffectEditor/PropertyItemCollectionMoveTests.cs`

## Interfaces and Dependencies

No NuGet package, domain-model change, serialization change, editor registration change, drag/drop change, or public property-editor API other than the two documented commands is required.

At completion, the command catalogue contains these public documented members:

    public static RoutedUICommand MoveCollectionItemUp { get; }
    public static RoutedUICommand MoveCollectionItemDown { get; }

The internal collection contract is:

    internal bool CanMoveItemInCollection(int sourceIndex, int targetIndex);
    internal bool MoveItemInCollection(int sourceIndex, int targetIndex);
    internal bool MoveCollectionValue(int sourceIndex, int targetIndex);
    internal static bool TryMoveItem(IList collection, int sourceIndex, int targetIndex);

All methods above return false without mutation for invalid state or indexes. `TryMoveItem` moves only the captured object reference. `MoveCollectionValue` owns the one-time clone/write/notification sequence. `CollectionView` owns WPF command routing and selection restoration. The three templates opt in solely by binding the new commands to their named `PART_editor` controls.

Revision note (2026-08-19): Initial ExecPlan created from the approved VIX-3974 design handoff after repository inspection. No production, test, Jira, or implementation files were changed.

Revision note (2026-08-19): Completed Milestone 1 by updating VIX-3974's user-facing description, scope, acceptance criteria, and validation plan. The issue remains in Accepted status; no source code changed.

Revision note (2026-08-19): Completed Milestone 2 by adding public documented ordering commands and internal collection move validation/mutation. The Effect Editor Release build succeeds with only six pre-existing warnings. CollectionView routing, XAML opt-in, tests, and end-to-end validation remain.

Revision note (2026-08-19): Completed Milestone 3 by registering shared move commands in CollectionView, enforcing parameter/editability/selection/boundary checks, and restoring the moved item's selection after reload. The Effect Editor Release build succeeds with only existing dependency and module warnings. XAML opt-in, tests, and end-to-end validation remain.

Revision note (2026-08-19): Completed Milestone 4 by adding targeted Up/Down controls to the color-gradient palette, color palette, and gradient-level pair templates. The required command target, parameter, focus scope, and editable visibility bindings are present in all three; excluded templates have no diff. The Effect Editor Release build succeeds with only existing warnings.

Revision note (2026-08-19): Completed Milestone 5 by exposing `PropertyItem.TryMoveItem` only to Vixen.Tests, adding focused non-WPF ordering/identity/boundary tests, and adding the required module project reference. The first reference configuration suppressed the runtime assembly and failed all focused tests; the repository-standard reference form corrected that. The standard test build succeeds and the focused suite passes 12/12.

Revision note (2026-08-19): Completed Milestone 6 from the user's reported successful full build, unit-test, and manual-validation evidence. VIX-3974's validation language was converted from planned to completed, and a concise validation comment was added. Its In Progress status was intentionally preserved because no transition was requested.

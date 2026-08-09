# VIX-3964 Add Option to Set Effect Default Settings

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Keep this document self-contained when revising it so a future contributor can implement the work with only this file and the current working tree. The approved architecture review this plan is built from lives at `docs/reviews/vix-3964-effect-default-settings-design.md`; if this ExecPlan and that review ever diverge during implementation, update this ExecPlan first and record the reason in `Decision Log`.

## Purpose / Big Picture

Today, every time a user drags a new instance of an effect (for example, Pulse, Wave, or Fireworks) onto the Timed Sequence Editor timeline, that effect starts from Vixen's hard-coded default settings: whatever colors, curves, gradients, and other values the module's data-model constructor happens to assign. A user who prefers, say, a particular color gradient and curve shape on every Pulse effect they place has to reapply those settings by hand each time.

After this change, the effect editor header gains two new buttons next to the existing help link: a "Save as Default" button and a "Reset to Built-in" button. While editing a single selected effect, pressing "Save as Default" captures every setting in that effect's data model (its `ModuleData` — the serializable class holding the effect's configuration, such as `PulseData` for the Pulse effect) and stores it, scoped to the current Vixen profile, as the new default for that effect type. From that moment forward, every new instance of that effect type created in the Timed Sequence Editor — drawn on the timeline, added via "Add Multiple Effects," added at marks, dropped from the toolbox, applied via a hotkey, used to replace another effect type, or dropped from a media file — starts pre-populated with the saved settings instead of the module's built-in constructor defaults. Pressing "Reset to Built-in" deletes the stored default (after a Yes/No confirmation) so future new instances revert to Vixen's original built-in behavior; it does not alter the effect currently open in the editor.

A user can see this working by: opening any effect (for example, Pulse) in the Timed Sequence Editor, changing its color gradient and level curve to a custom look, clicking "Save as Default" in the effect editor header, then adding a brand-new Pulse effect anywhere on the timeline — the new effect immediately shows the custom gradient and curve instead of Vixen's default ramp curve and white gradient. Clicking "Reset to Built-in" on that effect type and then adding another new Pulse effect shows it reverting to the original built-in defaults.

A secondary, less visible capability ships in later milestones: the saved defaults can be exported to a file and imported into another Vixen profile (for sharing a "look" between installations), and a diagnostic menu command can dump the stored defaults as human-readable indented XML for troubleshooting.

## Progress

- [x] (2026-08-08) Read `docs/reviews/vix-3964-effect-default-settings-design.md` (the approved architecture review) end to end.
- [x] (2026-08-08) Verified every file path, line number, and code claim cited in the architecture review against the current working tree (see `Surprises & Discoveries`); all citations matched exactly, no drift found.
- [x] (2026-08-08) Authored this ExecPlan from the approved architecture review.
- [x] (2026-08-09) Milestone 1: Updated Jira issue VIX-3964's description with the full purpose statement, resolved-decisions summary, milestone breakdown, and acceptance criteria/test plan from this ExecPlan. Status left unchanged ("New Ticket"); only the description field was edited.
- [x] (2026-08-09) Milestone 2: Implemented core storage — `ExcludeFromEffectDefaultAttribute` and its ten applications, `EffectDefaultsStore`/`EffectDefaultEntry`, `EffectDefaultsService` (`HasDefault`, `CreateDefaultData`, `SaveDefault`, `ClearDefault`, `Reload`, `GetSummaries`; `Export`/`Import`/`WriteDiagnosticDump` deliberately deferred to Milestone 4, see `Decision Log`), `EffectDefaultScrubber`, `EffectDefaultSummary`, plus automated tests (6 new tests, all passing). No user-visible behavior change yet. Full solution build (`msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m`) succeeded with zero new warnings/errors; full `dotnet test` run: 682 passed, 0 failed.
- [x] (2026-08-09) Milestone 3: Editor integration — added `SaveEffectDefault`/`ClearEffectDefault` commands, wired their bindings and a `HasStoredDefault` notify-property, added the two header buttons to both XAML layouts, added `CreateEffectInstanceWithDefaults` to `TimedSequenceEditorForm` and switched all six specified call sites to it, leaving the other three untouched. First user-visible behavior. Full solution build succeeded with no new warnings/errors; full `dotnet test` run: 682 passed, 0 failed (unchanged from Milestone 2 — no new automated tests were added this milestone, see `Decision Log`).
- [x] (2026-08-09) Milestone 4: Implemented `EffectDefaultsService.Export`/`Import`/`WriteDiagnosticDump` (deferred from Milestone 2), added an "Effect Defaults" submenu under Tools with Export/Import/Dump commands, and a checkbox selection form (`EffectDefaultsExportSelectionForm`) for choosing which saved defaults to export. Full solution build succeeded with no new warnings/errors; full `dotnet test` run: 686 passed, 0 failed (4 new tests added this milestone).
- [x] (2026-08-09) Milestone 5: Added `docs/effects/effect-defaults.md`, a user-facing document describing the Save/Reset buttons, per-profile storage, what is and isn't carried into a saved default (including the live library-tracking and Mark Collection scrubbing behavior), and the export/import/diagnostic-dump menu commands, with a cross-reference to this ExecPlan and the architecture review for implementers.
- [x] (2026-08-09) Post-Milestone-3 tweak from manual testing: the `ClearEffectDefault` confirmation dialog now names the actual effect type (for example "Remove the saved default for Pulse?") instead of the generic "this effect type," so the user can tell which effect is being reset. See `Decision Log`.
- [ ] Milestone 6: Final Jira alignment and closeout comment with validation results.

## Surprises & Discoveries

- Observation: Every file path and line number cited in the architecture review (`docs/reviews/vix-3964-effect-default-settings-design.md`) was independently re-verified against the current working tree before this ExecPlan was written, and all of them matched exactly.
  Evidence: `src/Vixen.Modules/Effect/Pulse/PulseData.cs:26` — `result.LevelCurve = LevelCurve;` (shared reference, confirmed). `src/Vixen.Modules/App/Curves/Curve.cs:73-81` — `Points` getter calls `CheckLibraryReference()` before returning `_points` (confirmed). `src/Vixen.Modules/App/Curves/Curve.cs:100` — `[DataMember] protected string _libraryReferenceName;` (confirmed non-public `[DataMember]`). `src/Vixen.Core/Sys/SystemConfig.cs:23` — `[DataPath] public static readonly string Directory = Path.Combine(Paths.DataRootPath, "SystemData");` is the exact precedent pattern to mirror (confirmed). `src/Vixen.Core/Sys/Paths.cs:70` — `Assembly.GetExecutingAssembly().GetTypes()` inside `_BuildDataDirectories()` confirms the `[DataPath]` auto-creation constant must live in a `Vixen.Core` type. `src/Vixen.Core/Sys/VixenSystem.cs:77` — `Paths.DataRootPath = dataRootDirectory ?? _GetUserDataPath();` confirms the profile-switch timing concern. `src/Vixen.Core/Module/Effect/IEffect.cs:11` — `IEffect: INotifyPropertyChanged` confirms `IEffect` does **not** extend `IModuleInstance`. `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` lines 2272, 2571, 2610, 3779, 4229, 4942 all contain `ApplicationServices.Get<IEffectModuleInstance>(...)` calls exactly as cited, and lines 3579, 5321, 6091 (plus `TimedSequenceEditorForm_Menu.cs:533`) are the three sites that must be left alone, all confirmed. `src/Vixen.Modules/Editor/EffectEditor/Design/AlphabeticalLayout.xaml` line 13 confirms its header is a bare `Border` with no `Grid` and no help hyperlink, unlike `CategorizedLayout.xaml` lines 17-31. `src/Vixen.Common/Resources/disk.png` and `src/Vixen.Common/Resources/arrow_refresh.png` both exist (confirmed via file glob). `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs:39,52` confirms the `AssemblyQualifiedName` + `XmlDictionaryWriter.CreateBinaryWriter` precedent this design deliberately avoids for the persistent store. `src/Vixen.Common/WPFCommon/Services/MessageBoxService.cs:44` — `public MessageBoxResult GetUserConfirmation(string question, string title)` confirmed for the Reset confirmation dialog.

- Observation: The user correctly identified that unlinking a saved default's `Curve`/`ColorGradient` from the library on save is unnecessary and undesirable. `Curve.UpdateLibraryReference()` (`Curve.cs:151-169`) and the equivalent `ColorGradient.UpdateLibraryReference()` (`ColorGradient.cs:1360-1377`) already self-heal when a referenced library entry is deleted or renamed: `Library.Contains(LibraryReferenceName)` returning false clears `LibraryReferenceName` to empty and keeps whatever was last materialized into `_points`/`_colors`/`_alphas`. There is no dangling-reference risk to defend against, so this plan's original `ILibraryLinkable` interface and the scrubber's unlink step were dropped entirely (see `Decision Log`); saved defaults now keep their library links live, matching the whole point of linking a curve or gradient to the library in the first place.
  Evidence: `Curve.cs:151-169`, `ColorGradient.cs:1360-1377`.

- Observation: `Curve.UpdateLibraryReference()` and `ColorGradient.UpdateLibraryReference()` are not symmetrically null-safe. `Curve.UpdateLibraryReference()` guards with `if (Library != null)` before calling `Library.Contains(...)`, so it degrades gracefully when no `CurveLibrary` module instance is available (for example, in a unit test host with no running Vixen application — `Library` resolves to `null` via `ApplicationServices.Get<IAppModuleInstance>(...)` returning `null`). `ColorGradient.UpdateLibraryReference()` has no such guard and calls `Library.Contains(LibraryReferenceName)` directly, which would throw a `NullReferenceException` if `Library` is `null`. This does not affect this plan's capture pipeline (see the next observation) but is a latent asymmetry worth fixing separately if `ColorGradient`'s public `Colors`/`Alphas`/`Title`/`Gammacorrected` getters are ever called outside a fully initialized Vixen application (for example, from a future unit test). Not fixed here — out of scope for this ticket.
  Evidence: `Curve.cs:151-169` (`if (Library != null) { if (Library.Contains(...)) ... }`) vs. `ColorGradient.cs:1360-1377` (`if (Library.Contains(LibraryReferenceName)) ...`, no null check on `Library`). `src/Vixen.Tests/EffectDefaults/EffectDefaultScrubberTests.cs`'s `Scrub_LeavesLibraryReferencedCurveUntouched` test relies on `Curve`'s null-safety to read `.Points` in the unit test host without throwing; the equivalent test was not written against `ColorGradient` for this reason.

- Observation: The Decision Log's original claim that serializing `effect.ModuleData` "reads through... the `ColorGradient` property getters (`ColorGradient.cs:807,817,827,841`), which call `CheckLibraryReference()` as a side effect" is not accurate for `ColorGradient` (it is accurate for `Curve`). `ColorGradient`'s `[DataMember]` attributes are declared directly on the private backing fields `_colors`, `_alphas`, `_gammacorrected`, `_title` (`ColorGradient.cs:257-260`), not on the public `Colors`/`Alphas`/`Gammacorrected`/`Title` properties that wrap them and call `CheckLibraryReference()` in their getters. `DataContractSerializer` serializes `[DataMember]`-decorated fields by reading the field directly via reflection, never through an unrelated property's getter, so serializing a live `ColorGradient` does not itself trigger `CheckLibraryReference()`. `Curve`, by contrast, puts `[DataMember]` on the public `Points` property itself (`Curve.cs:71-73`), so serializing a live `Curve` does call `CheckLibraryReference()` through the property getter. This does not change the design's correctness: the live-tracking behavior described in `Context and Orientation` (a freshly deserialized default's cached library-lookup fields come back `null`, so the *next* read after deserialization re-resolves against the library) is what actually makes live tracking work for both types, not any refresh that happens during capture. The Decision Log entry below has been corrected to remove the inaccurate `ColorGradient` claim.
  Evidence: `ColorGradient.cs:257-260` (`[DataMember] private PointList<ColorPoint> _colors;` etc., no `[DataMember]` on the `Colors`/`Alphas`/`Gammacorrected`/`Title` properties at `ColorGradient.cs:803-849`) vs. `Curve.cs:71-73` (`[DataMember] ... public PointPairList Points { get { CheckLibraryReference(); return _points; } ... }`).

- Observation: Two Rider/ReSharper "field is sometimes used inside synchronized block and sometimes used without synchronization" warnings surfaced during implementation of `EffectDefaultsService`, once for `_entriesByTypeId` (expected: it was a `volatile` field read outside the lock and written inside it, an intentional double-checked-locking pattern) and once for the static `Logging` field (a false positive, since `NLog.Logger` needs no external synchronization at all — but the warning is still raised for any field touched both inside and outside the same class's `lock` blocks). Rather than argue with the linter, `EffectDefaultsService` was simplified to hold `_loadLock` for the full body of every public method (including `CreateDefaultData`'s deserialization work), eliminating the double-checked-locking pattern entirely. This trades a small amount of concurrency (two callers on different threads briefly serialize through `CreateDefaultData`) for simplicity and a clean static analysis pass; given `CreateDefaultData` only runs when an effect is newly created (not a hot path), this is the right tradeoff.
  Evidence: Rider post-edit hook output during implementation, e.g. `src\Vixen.Core\Services\EffectDefaults\EffectDefaultsService.cs:107:6 [WARNING] The field is sometimes used inside synchronized block and sometimes used without synchronization` (on the `Logging` field, before the fix).

- Observation: `VixenModules.Editor.EffectEditor.Design.AlphabeticalLayout` (the class styled by `AlphabeticalLayout.xaml`) is confirmed dead code — nothing in `src/Vixen.Modules` constructs it (`grep "new AlphabeticalLayout"` only matches the unrelated `Vixen.Common.WpfPropertyGrid.Design.AlphabeticalLayout`, a different class used by a different, generic property grid). The header buttons were still added to `AlphabeticalLayout.xaml` for template parity, per the plan's instruction, even though nothing currently renders them.
  Evidence: `grep -rn "new AlphabeticalLayout" src/Vixen.Modules` returned no matches; the only `new AlphabeticalLayout()` construction in the whole `src` tree is `src/Vixen.Common/WpfPropertyGrid/PropertyGrid.cs:341`, which instantiates `Vixen.Common.WpfPropertyGrid.Design.AlphabeticalLayout`, not `VixenModules.Editor.EffectEditor.Design.AlphabeticalLayout`.

- Observation: `Common.WPFCommon.Services.IMessageBoxService.GetUserConfirmation`'s Yes/No dialog reports its "confirm" (Yes) button as `MessageResult.OK`, not `MessageResult.Yes` — confirmed by an existing code comment at `src/Vixen.Modules/Editor/LayerEditor/ViewModels/LayerEditorViewModel.cs:254-255` ("IMessageBoxService.GetUserConfirmation shows a Yes/No dialog whose 'confirm' button reports MessageResult.OK ... it never reports Cancel"). This differs from Catel's own `IMessageService.ShowAsync`, used elsewhere in the codebase (for example `StateDefinitionDialogService.cs`), which does report `MessageResult.Yes` for its Yes/No dialogs. `ClearEffectDefault`'s confirmation check uses `!= MessageResult.OK`, matching the `IMessageBoxService`-specific behavior, not the Catel one.
  Evidence: `src/Vixen.Modules/Editor/LayerEditor/ViewModels/LayerEditorViewModel.cs:254-264`, `src/Vixen.Modules/Property/State/Setup/Services/StateDefinitionDialogService.cs:57-65`.

- Observation: `EffectPropertyEditorGrid`/`PropertyGridCommands`-related XAML in this project routes `Command="input:PropertyGridCommands.SomeCommand"` directly (no `x:Static` markup extension needed, since `RoutedUICommand` static properties are referenced this way in WPF XAML by convention) and relies purely on the `CommandBinding`'s `CanExecute` handler to auto-disable the bound `Button`/`ToggleSwitch` — no explicit `IsEnabled` binding is used anywhere else in this project for command-bound controls (confirmed via the pre-existing `TogglePreview` binding in `Themes/PropertyGrid.xaml:36`). The two new buttons follow this same idiom: no `IsEnabled` binding on either button, relying entirely on `OnCanExecuteSaveEffectDefaultCommand`/`OnCanExecuteClearEffectDefaultCommand`.
  Evidence: `src/Vixen.Modules/Editor/EffectEditor/Themes/PropertyGrid.xaml:36` (`Command="input:PropertyGridCommands.TogglePreview"`, no `IsEnabled` binding present).

- Observation: Adding menu items to `TimedSequenceEditorForm` requires hand-editing the auto-generated `TimedSequenceEditorForm.Designer.cs` (field declarations, `InitializeComponent()` construction lines, a `DropDownItems.AddRange` array entry, and the item's own `Name`/`Size`/`Text`/`Click` wiring block) because no WinForms Designer tool is available in this environment; there is no simpler code-only path to add a menu item in this form. The existing `lipSyncMappingsToolStripMenuItem` submenu (`TimedSequenceEditorForm.Designer.cs:1104-1136`) was used as the structural precedent for the new "Effect Defaults" submenu (a `ToolStripMenuItem` whose own `DropDownItems` contains Export/Import/Dump), added into the "Tools" top-level menu's `DropDownItems.AddRange` list alongside `bulkEffectMoveToolStripMenuItem`.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs:1006-1017` (`toolsToolStripMenuItem.DropDownItems.AddRange`), `:1104-1136` (`lipSyncMappingsToolStripMenuItem` submenu precedent).

- Observation: Every `.cs` file under `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/`, even though physically nested in a `Forms` subfolder, uses the flat namespace `VixenModules.Editor.TimedSequenceEditor` rather than `VixenModules.Editor.TimedSequenceEditor.Forms` (confirmed for `FindEffectForm.cs`, `Form_AddMultipleEffects.cs`, and others). The new `EffectDefaultsExportSelectionForm` follows this existing, if inconsistent-with-folder-structure, convention for consistency with its siblings; a "namespace does not match file location" analyzer warning is expected and was left as-is, matching the pre-existing pattern across the whole `Forms` folder.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FindEffectForm.cs:8` (`namespace VixenModules.Editor.TimedSequenceEditor`).

## Decision Log

- Decision: Capture only the effect's `ModuleData` (not `TargetNodes`, `TimeSpan`, `StartTime`, `Media`, or `MarkCollections`).
  Rationale: Those other fields are either sequence-specific placement data (targets, timing) or per-sequence resources (media files, mark collections) that have no meaningful "default" outside the sequence they were created in. Only the settings a user tweaks in the property grid — the `ModuleData` — generalize across new effect instances.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: Store defaults profile-scoped in a single file at `{Paths.DataRootPath}\Effect Defaults\EffectDefaults.vfd`, deliberately outside `VixenSystem.ModuleStore`.
  Rationale: Keeping it out of the module store lets the file round-trip independently (for export/import) without dragging in unrelated module state, and placing it inside the profile folder means Vixen's existing `DataZipForm` support-archive tooling picks it up automatically with no extra code.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: Use binary `DataContractSerializer` (via `XmlDictionaryWriter.CreateBinaryWriter`) as the wire format, with a separate on-demand indented-XML diagnostic dump rather than storing XML directly.
  Rationale: Binary keeps the persistent file compact and matches an existing precedent in the codebase (`EffectModelCandidate.cs:52`, used for clipboard copy/paste of effects). Because the diagnostic dump is generated from the exact same object graph through `XmlWriter.Create(..., Indent=true)` on demand, it cannot drift from the stored bytes, so no second source of truth is introduced.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: Never persist a payload's `AssemblyQualifiedName`; always resolve the payload `Type` at load time from `Modules.GetDescriptorById(typeId).ModuleDataClass`.
  Rationale: `EffectModelCandidate.cs:39` shows the codebase already does this for clipboard data via `Type.GetType(assemblyQualifiedName)`, which is acceptable because a clipboard payload lives for seconds. Embedding an assembly version in a file meant to persist across Vixen releases would break on the next assembly version bump. `DataModelTypeName` is still stored, but purely for diagnostics and mismatch logging, never for `Type` resolution.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: The capture pipeline must be serialize-live → deserialize-copy → scrub-copy → serialize-copy → store. Do not use `Clone()` anywhere in this pipeline.
  Rationale: `PulseData.CreateInstanceForClone` (`src/Vixen.Modules/Effect/Pulse/PulseData.cs:26`) assigns `result.LevelCurve = LevelCurve` — a shared reference, not a copy. The scrub step blanks out every `[ExcludeFromEffectDefault]` member (the ten `MarkCollectionId` fields) to `default(T)`; if the capture path cloned the live effect with `Clone()` and then scrubbed the clone, a shared reference like `LevelCurve` means the scrub could mutate state still reachable from the live effect the user is actively editing, not just the copy. Serializing to bytes and deserializing a fresh object graph guarantees the copy shares no references with the live effect, so scrubbing it is always safe. Serializing the live object first also reads through `Curve.Points` (`Curve.cs:73`), which calls `CheckLibraryReference()` as a side effect and materializes the current library values into the object's own `[DataMember]` field. (`ColorGradient` does not have this same serialize-time refresh — its `[DataMember]` attributes are on the private backing fields directly, not on the `Colors`/`Alphas`/`Gammacorrected`/`Title` properties that call `CheckLibraryReference()`, so serializing a live `ColorGradient` reads the fields as last materialized rather than forcing a fresh library lookup; see `Surprises & Discoveries` for the corrected analysis.) Either way, what actually makes a saved default's library-linked curve or gradient keep tracking live library edits later is what happens on deserialization: a freshly deserialized default's cached library-lookup fields (`LibraryReferencedCurve`, `_libraryReferencedGradient`) come back `null` since they are not `[DataMember]`, so the *next* read of `.Points`/`.Colors`/etc. on the new effect re-resolves against whatever the library currently contains.
  Date/Author: Recorded in the approved architecture review, 2026-08-08; rationale updated 2026-08-08 to reflect dropping the library-unlink step; corrected 2026-08-09 during Milestone 2 implementation after discovering the `ColorGradient` serialization claim was inaccurate (see `Surprises & Discoveries`).

- Decision: Do not introduce an `ILibraryLinkable` interface or any scrubber step that severs library links. Saved effect defaults keep their `Curve`/`ColorGradient` library references exactly as captured, so a default that points at a shared library curve or gradient continues to track that library entry's current values for every future effect created from the default.
  Rationale: An earlier draft of this plan (see the superseded decision immediately below) unlinked defensively at save time to avoid a "dangling reference." That defense turned out to be unnecessary: `Curve.UpdateLibraryReference()` (`Curve.cs:151-169`) and `ColorGradient.UpdateLibraryReference()` (`ColorGradient.cs:1360-1377`) already self-heal when a referenced library entry is deleted or renamed — they clear the link name and fall back to the last-materialized values automatically, with no help needed from this feature. More importantly, live tracking is the actual point of linking an effect's curve or gradient to the library in the first place: a user builds a shared gradient in the Color Gradient Library specifically so that editing it later updates every place that references it. Freezing the link at save time would silently take that behavior away for exactly the effects that most benefit from it.
  Date/Author: User correction during ExecPlan review, 2026-08-08.

- Decision (superseded, kept for history): Introduce a new marker interface `Vixen.Data.Library.ILibraryLinkable` with a single member `void UnlinkFromLibrary()`, implemented by `ColorGradient` (already has a matching method) and `Curve` (via an explicit interface implementation that delegates to the existing `UnlinkFromLibraryCurve()`), and have the scrubber call it on every reachable library-linkable object.
  Rationale (as originally recorded, now known to be flawed): `Vixen.Core` cannot reference `Curve` or `ColorGradient` directly, since both live in `Vixen.Modules` projects that already depend on `Vixen.Core`. A narrow interface defined in `Vixen.Core` would have let the scrubber sever library links without knowing about the concrete module types. This is no longer needed because the scrubber no longer severs library links at all — see the decision above.
  Date/Author: Recorded in the approved architecture review, 2026-08-08; superseded 2026-08-08.

- Decision: Add a new `[ExcludeFromEffectDefault]` attribute and apply it to exactly ten members (each a `MarkCollectionId` field/property that is meaningful only within one sequence), not to `Emitter.cs:863` or `Waveform.cs:389` (which are runtime wrappers, not serialized data models).
  Rationale: A saved effect default should not silently carry a specific sequence's Mark Collection `Guid` into an unrelated sequence, where that `Guid` refers to nothing (or, worse, to a different, unrelated collection that happens to share the `Guid` by coincidence — extremely unlikely, but the safer behavior is always to reset to `default(T)`, i.e., `Guid.Empty`).
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision (superseded): The scrubber walk only needs to enumerate public `[DataMember]` properties and fields; it does not need to reach into non-public members.
  Rationale: The only reason an earlier draft of this plan needed non-public member access was to reach `ColorGradient._colors`/`_alphas`/`_libraryReferenceName` (`ColorGradient.cs:257-259,1314`) and `Curve._libraryReferenceName` (`Curve.cs:100`) for the now-dropped library-unlink step. All ten `[ExcludeFromEffectDefault]` members are public auto-properties, so a public-only reflection walk (`BindingFlags.Public | BindingFlags.Instance`) is sufficient and simpler. If a future member needing exclusion is ever private, extend the `BindingFlags` at that time rather than defending against it now.
  Date/Author: Recorded in the approved architecture review, 2026-08-08; narrowed 2026-08-08 after dropping the library-unlink step.

- Decision: The seam where saved defaults are applied is narrow and explicit — a single new private helper `TimedSequenceEditorForm.CreateEffectInstanceWithDefaults(Guid effectTypeId)` — and exactly six call sites in `TimedSequenceEditorForm.cs` (lines 2272, 2571, 2610, 3779, 4229, 4942) must be switched to call it instead of the plain `ApplicationServices.Get<IEffectModuleInstance>(...)`. Three other sites (`TimedSequenceEditorForm.cs:3579` `CloneElements`, `TimedSequenceEditorForm.cs:5321` paste, `TimedSequenceEditorForm.cs:6091` and `TimedSequenceEditorForm_Menu.cs:533` LipSync) must be left as plain `ApplicationServices.Get<IEffectModuleInstance>(...)` calls.
  Rationale: There is no single central chokepoint in the codebase where all new `IEffectModuleInstance` creation funnels through one factory method — effect creation is spread across many call sites in the Timed Sequence Editor. Rather than invent a new central factory (a bigger, riskier refactor outside this ticket's scope), this design accepts a narrow, explicitly enumerated seam. The three excluded sites all overwrite `ModuleData` on the very next statement (clone copies `ModuleData.Clone()`, paste assigns the clipboard's captured `ModuleData`, LipSync assigns its own data), so materializing a saved default there would be wasted work that gets immediately discarded — applying it would be silently correct but wasteful, so it is skipped for efficiency, not correctness. Because there is no central fallback, this list of six sites **is** the specification: if a future contributor adds a seventh place that creates new effect instances for permanent placement on the timeline, they must add it to this list and switch it too, or defaults silently will not apply there.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: Do not cache a single deserialized default `ModuleData` prototype and `Clone()` it per new effect instance; deserialize the stored payload fresh for every `CreateDefaultData` call.
  Rationale: The same shallow-`Curve`-sharing hazard that rules out `Clone()` in the capture pipeline also rules it out here — cloning a cached prototype would let many concurrently-created effects share one mutable `Curve` instance, so editing one effect's curve would silently corrupt every other effect created from the same prototype. Deserializing fresh each time is slightly more CPU work (tens to low-hundreds of microseconds per effect) but is correct, and it exactly replaces the constructor call that would otherwise run, so it is not additional cost relative to today's behavior.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: `CreateDefaultData` never throws into an effect-creation path; every failure (missing descriptor, type mismatch, deserialization exception) is caught, logged, and returns `null`, and every caller falls back to the existing built-in constructor path.
  Rationale: A corrupted or stale stored default must never prevent a user from placing a new effect on the timeline. Degrading silently to today's behavior is always safer than surfacing an error in the middle of sequencing work.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: When a stored default's effect type is not currently installed (its module was uninstalled), retain the entry in memory and rewrite it unchanged the next time the store is saved, rather than silently discarding it.
  Rationale: A user who temporarily removes and reinstalls a module (or upgrades in a way that briefly changes module discovery) should not lose their saved default as a side effect of an unrelated operation.
  Date/Author: Recorded in the approved architecture review, 2026-08-08.

- Decision: `EffectDefaultsService.Export`, `Import`, and `WriteDiagnosticDump` were not declared (not even as stub/`NotImplementedException` methods) in Milestone 2, even though the architecture review's full public surface listing includes them. They will be added in Milestone 4 when actually implemented.
  Rationale: Milestone 2's own narrative text only describes implementing `HasDefault`, `CreateDefaultData`, `SaveDefault`, `ClearDefault`, `Reload`, `GetSummaries`, and lazy loading — it never describes implementing export/import/diagnostics, which Milestone 4 explicitly owns. Declaring three public methods that immediately throw `NotImplementedException` would be a half-finished implementation left sitting in the codebase for an unknown number of commits, which the project's coding guidelines call out to avoid. Adding them fresh in Milestone 4 (along with the `ImportMode` enum and `EffectDefaultsImportResult` type their signatures require) keeps every commit's public surface fully functional.
  Date/Author: Implementer decision during Milestone 2, 2026-08-09.

- Decision: `EffectDefaultsService`'s in-memory store is a plain `Dictionary<Guid, EffectDefaultEntry>` guarded entirely by one `lock (_loadLock)` held for the full body of every public method, rather than a `ConcurrentDictionary<Guid, byte[]>` payload cache read outside the lock as the Milestone 2 narrative originally sketched.
  Rationale: A `volatile`-field-plus-`ConcurrentDictionary` design (reads outside the lock, writes inside it) triggered a Rider/ReSharper "field is sometimes used inside synchronized block and sometimes used without synchronization" warning — correctly on the field itself, and also (spuriously) on the unrelated static `Logging` field, which is touched inside `LoadLocked`'s catch block (under lock) and inside `CreateDefaultData`'s original outside-lock warning calls. Rather than fight the analyzer with suppressions, the simpler and equally correct fix was to hold the lock for the whole operation in every public method. `EffectDefaultsService` is not a hot path (it is only consulted when an effect is newly created or a default is explicitly saved/cleared/queried), so the small serialization cost of holding one lock per call is an acceptable, simple tradeoff. See `Surprises & Discoveries` for the warning text.
  Date/Author: Implementer decision during Milestone 2, 2026-08-09.

- Decision: The regression test proving `SaveDefault` does not mutate live `ModuleData` calls a new `internal static EffectDefaultsService.CaptureScrubbedPayload(DataContractSerializer, IModuleDataModel)` method directly (exposed to `Vixen.Tests` via the existing `InternalsVisibleTo` in `Vixen.Core.csproj`) rather than calling the public `SaveDefault(IEffectModuleInstance)` end-to-end.
  Rationale: `SaveDefault` persists to `EffectDefaultsService.Directory`, a `[DataPath] public static readonly string` field evaluated once from `Paths.DataRootPath` — which, absent a running `VixenSystem`, defaults to the real user's `Documents\Vixen 3` folder. Calling `SaveDefault` from a unit test would write real files under the developer's (or CI runner's) actual Documents folder, and because the field is `static readonly`, no per-test override is possible within a single test process. `CaptureScrubbedPayload` factors out exactly the serialize→deserialize→scrub→serialize logic that the regression test needs to verify (the part that matters for the `PulseData.CreateInstanceForClone` shared-reference hazard) with zero disk I/O, so the test exercises the real pipeline code `SaveDefault` calls without any risk of touching the filesystem outside the test process. Verified after the fact: a full `dotnet test` run left `Documents\Vixen 3\Effect Defaults` absent.
  Date/Author: Implementer decision during Milestone 2, 2026-08-09.

- Decision: Added a `ProjectReference` to `src\Vixen.Modules\Effect\Pulse\Pulse.csproj` in `src\Vixen.Tests\Vixen.Tests.csproj` so the regression test can construct a real `PulseData` (the exact type the Decision Log's shared-reference hazard concerns).
  Rationale: `Pulse.csproj` already references `Curves.csproj` and `ColorGradients.csproj` transitively, so this single addition also made `Curve` and `ColorGradient` available to the new `EffectDefaults` test folder without adding further references.
  Date/Author: Implementer decision during Milestone 2, 2026-08-09.

- Decision: `CreateEffectInstanceWithDefaults` was not extracted a second time into a separately unit-tested helper beyond what already exists on `TimedSequenceEditorForm`, and no new automated tests were added in Milestone 3.
  Rationale: The method is six lines of straight-line glue logic (call `ApplicationServices.Get<IEffectModuleInstance>`, and if non-null, call `EffectDefaultsService.Instance.CreateDefaultData` and assign `ModuleData` if non-null) with no branching complexity of its own — the actual decision logic (does a saved default exist, does it type-match, does it deserialize cleanly) lives entirely in `EffectDefaultsService.CreateDefaultData`, which Milestone 2's test suite already covers directly. `TimedSequenceEditorForm` itself remains impractical to unit test in isolation (WinForms-hosted WPF editor form with heavy UI/application-service dependencies), matching the plan's anticipated fallback.
  Date/Author: Implementer decision during Milestone 3, 2026-08-09.

- Decision: `ClearEffectDefault`'s confirmation check compares `confirmResult.Result != MessageResult.OK`, not `!= MessageResult.Yes`, even though the dialog is Yes/No.
  Rationale: `Common.WPFCommon.Services.IMessageBoxService.GetUserConfirmation` (the exact service the ExecPlan specifies) reports its Yes/No dialog's confirm button as `MessageResult.OK`, per an existing, verified precedent and code comment elsewhere in the codebase (see `Surprises & Discoveries`). Using `MessageResult.Yes` here — which is correct for Catel's own `IMessageService`, a different service used elsewhere in the codebase — would have made the confirmation dialog's Yes button silently do nothing.
  Date/Author: Implementer decision during Milestone 3, 2026-08-09.

- Decision: `EffectDefaultsService.Export`, `Import`, and `WriteDiagnosticDump` each delegate their disk-independent core logic to a new `internal static` helper (`BuildExportStore`, `MergeEntries`, `WriteIndentedXml` respectively), mirroring the `CaptureScrubbedPayload` extraction from Milestone 2, and Milestone 4's automated tests call those helpers directly instead of going through `EffectDefaultsService.Instance`.
  Rationale: `EffectDefaultsService.Instance` is a process-wide singleton whose primary store path (`Directory`, a `[DataPath] public static readonly string` field) is fixed to the real user's profile folder unless `VixenSystem.Start` has run. `Import` in particular calls `PersistLocked()` (writing the primary store file) as part of its documented behavior, and `Export`/`GetSummaries` read `_entriesByTypeId`, which is lazily loaded from that same real file. Exercising these methods through the singleton in a unit test would either write to the developer's real `Documents\Vixen 3\Effect Defaults\EffectDefaults.vfd` (for `Import`) or read whatever happens to already be there (for `Export`), exactly the same testability problem recorded for `SaveDefault` in Milestone 2's Decision Log. Extracting the pure subsetting/merging/XML-writing logic into static helpers keeps the automated tests fully isolated from any disk state while still exercising the real code each public method runs.
  Date/Author: Implementer decision during Milestone 4, 2026-08-09.

- Decision: Import always merges every entry in the imported file (via `ImportMode.Overwrite`, the only supported mode); there is no partial-import selection UI, even though the export side has one.
  Rationale: The Milestone 4 plan text explicitly allows this simplification ("optionally, Import... if the format supports partial import — otherwise import brings in everything in the file and merges"), and the architecture review's documented behavior is "merge on import." Adding a second checkbox-selection dialog for import would be additional UI with no design requirement calling for it.
  Date/Author: Implementer decision during Milestone 4, 2026-08-09.

- Decision: The `ClearEffectDefault` confirmation prompt was changed from the generic "Remove the saved default for this effect type?" (Milestone 3's original wording) to `$"Remove the saved default for {effect.Descriptor.TypeName}?"`, interpolating the effect's display name (for example "Pulse" or "Wipe").
  Rationale: User feedback after manually testing Milestone 3 pointed out that the generic wording doesn't tell the user which effect type they're about to reset, which matters because the property grid can be showing any effect at the time the button is clicked. `effect.Descriptor.TypeName` is the same display name already shown elsewhere in the UI (it is `IModuleDescriptor.TypeName`, `src/Vixen.Core/Module/IModuleDescriptor.cs:10`) and was already in scope at the point `OnClearEffectDefaultCommand` builds the prompt, so no new lookup was needed.
  Date/Author: User-reported tweak after manual testing, 2026-08-09.

## Outcomes & Retrospective

Milestone 5 is complete: added `docs/effects/effect-defaults.md`, a user-facing document (no code changes) written for a Vixen user rather than a developer. It covers what the "Save as Default" and "Reset to Built-in" buttons do, a worked example, that saved defaults are per-profile, what is and isn't captured in a saved default (explicitly calling out the Mark Collection scrubbing and the live library-tracking behavior for `Curve`/`ColorGradient`, both established in earlier milestones), and the Tools → Effect Defaults menu's Export/Import/Dump commands from Milestone 4. It closes with a cross-reference to this ExecPlan and the architecture review for implementers, per `CLAUDE.md`'s "Use Docs First" convention. No build or test run was needed for this milestone since no source files changed.

Milestone 4 is complete: `EffectDefaultsService` now has working `Export`, `Import`, and `WriteDiagnosticDump` methods (declared but deliberately left unimplemented at the end of Milestone 2), each backed by a disk-independent, directly-testable helper (`BuildExportStore`, `MergeEntries`, `WriteIndentedXml`). The Timed Sequence Editor's Tools menu gained an "Effect Defaults" submenu with Export/Import/Dump commands; Export shows a new checkbox selection form (`EffectDefaultsExportSelectionForm`) listing every currently saved default so the user can choose a subset, while Import always merges everything in the chosen file (overwriting by `TypeId`) and Dump writes the same in-memory store as indented, human-readable XML. Four new automated tests were added covering export subsetting (including the "no matching entry" case), import merge/overwrite counting, and diagnostic-dump XML validity/round-trip; all pass, and the full suite (686 tests, up from 682 in Milestone 3) passes with no regressions. The full solution also builds cleanly in Release/x64 with no new warnings.

Two implementation-level adjustments are recorded above: the automated tests exercise new `internal` helper methods rather than the public `Export`/`Import`/`WriteDiagnosticDump` directly, for the same real-profile-folder isolation reason established in Milestone 2 for `SaveDefault`; and Import has no partial-selection UI, matching the plan's own stated simplification. Adding the three menu commands required hand-editing the WinForms Designer-generated `TimedSequenceEditorForm.Designer.cs` (no Designer tool available in this environment), modeled directly on the existing `lipSyncMappingsToolStripMenuItem` submenu precedent. Milestone 5 (user-facing documentation under `docs/effects/`) is next.

Milestone 3 is complete: the effect editor header now has "Save as Default" and "Reset to Built-in" buttons (in both `CategorizedLayout.xaml`, the layout actually used at runtime, and `AlphabeticalLayout.xaml`, confirmed dead code but kept in parity per the plan), and all six specified `TimedSequenceEditorForm` call sites now materialize new effect instances through `CreateEffectInstanceWithDefaults`, which applies a saved default on top of the module's built-in constructor result when one exists. This is the plan's first user-visible behavior. The three call sites that immediately overwrite `ModuleData` (clone, paste, LipSync) were left untouched, exactly as specified. Full solution build succeeded with no new warnings or errors, and the full automated test suite (682 tests, unchanged from Milestone 2) still passes — no regressions. No new automated tests were added this milestone; see `Decision Log` for why, and `Validation and Acceptance` for the manual walkthrough a human should still perform in a running Debug build to confirm the button behavior, live library tracking, and Mark Collection scrubbing end to end.

Two implementation-level deviations from the narrative surfaced and are recorded above: the Reset confirmation check uses `MessageResult.OK` rather than `MessageResult.Yes` (a `Common.WPFCommon.Services.IMessageBoxService`-specific quirk, verified against existing code elsewhere in the repo), and no separate testable helper was extracted beyond `CreateEffectInstanceWithDefaults` itself, since its logic is trivial glue over already-tested `EffectDefaultsService` behavior. Neither changes the feature's design as approved. Milestone 4 (export, import, and the diagnostic dump) is next.

Milestone 2 is complete: `docs.plans.effects.vix-3964-effect-default-settings.md`'s core-storage layer now exists and is fully covered by automated tests, with zero user-visible behavior change (nothing in the editor calls any of this new code yet — that begins in Milestone 3). What was built: `Vixen.Sys.Attribute.ExcludeFromEffectDefaultAttribute` (new file) applied to exactly the ten `MarkCollectionId` members specified in `Plan of Work`; `Vixen.Services.EffectDefaults.EffectDefaultsStore`/`EffectDefaultEntry` (the on-disk shape); `Vixen.Services.EffectDefaults.EffectDefaultScrubber` (the reflection-based scrub walk); `Vixen.Services.EffectDefaults.EffectDefaultSummary` (the read-only projection type); and `Vixen.Services.EffectDefaults.EffectDefaultsService` (the public service, minus `Export`/`Import`/`WriteDiagnosticDump`, deliberately deferred to Milestone 4 — see `Decision Log`). Six new automated tests were added under `src/Vixen.Tests/EffectDefaults/` covering every case Milestone 2 required: exclusion at the top level and nested inside a `List<T>`, a library-referenced `Curve` left untouched by scrubbing, cycle termination, a binary store round-trip, and the `PulseData.LevelCurve` shared-reference non-mutation regression test. All 6 pass, and the full suite (682 tests) passes with no regressions; the full solution also builds cleanly in Release/x64 with no new warnings.

Three deviations from the original narrative surfaced during implementation, all recorded above: `Export`/`Import`/`WriteDiagnosticDump` were not stubbed out (deferred whole to Milestone 4); the in-memory store ended up as a lock-guarded `Dictionary` rather than a `ConcurrentDictionary`-backed cache (a static-analysis-driven simplification, not a behavior change); and the non-mutation regression test calls a new `internal` capture-pipeline method instead of the public `SaveDefault`, to avoid writing to the real user profile folder during test runs. None of these change the feature's design as approved — they are implementation-level adjustments, each with its own `Decision Log` entry. Also recorded: two genuine corrections to the architecture review's technical claims (the `ColorGradient` vs. `Curve` serialization-time behavior, and the `Curve`/`ColorGradient` null-safety asymmetry in `UpdateLibraryReference()`), neither of which affects the feature's correctness but both of which are now accurately documented for whoever implements Milestone 3 next.

Nothing remains outstanding from Milestone 2's scope. Milestone 3 (editor integration — the two header buttons and the six `TimedSequenceEditorForm` call sites) is next.

## Context and Orientation

Vixen is a Windows desktop application (.NET 10, WPF) for creating and running animated light shows sequenced to music. It is built around a descriptor-based plugin ("module") architecture: every capability — effects, controllers, editors, output filters, previews — is a module living under `src/Vixen.Modules/{ModuleType}/{ModuleName}/`. This work concerns the `Effect` module type: visual effects such as Pulse, Wave, Fireworks, and Text, rendered onto element timelines in the Timed Sequence Editor (the main sequencing UI, at `src/Vixen.Modules/Editor/TimedSequenceEditor/`).

An effect *instance* implements `IEffectModuleInstance` (`src/Vixen.Core/Module/Effect/IEffectModuleInstance.cs`), which extends the base `IModuleInstance` interface and adds effect-specific members like `TypeId`, `Descriptor`, and `ModuleData`. The narrower `IEffect` interface (`src/Vixen.Core/Module/Effect/IEffect.cs`) does **not** extend `IModuleInstance` — it only extends `INotifyPropertyChanged` and exposes rendering-facing members like `TargetNodes`, `TimeSpan`, `Render()`, and `MarkCollections`. This distinction matters because the effect editor's property grid works against `IEffect` (via `SelectedObject`/`SelectedObjects` on `EffectPropertyEditorGrid`), so reaching an effect's `TypeId`, `Descriptor`, or `ModuleData` from a selected effect in the editor requires an `as IEffectModuleInstance` cast that can legitimately return `null` and must be handled as a no-op, not an exception.

Every effect instance owns a `ModuleData` object — a serializable class holding that specific effect's configuration. For example, `PulseData` (`src/Vixen.Modules/Effect/Pulse/PulseData.cs`) holds a `Curve LevelCurve` and a `ColorGradient ColorGradient`. These data classes are decorated with `[DataContract]`/`[DataMember]` attributes (from `System.Runtime.Serialization`) so they can be serialized. Two of the module-referenced types deserve special explanation because they are central to this feature:

`Curve` (`src/Vixen.Modules/App/Curves/Curve.cs`) represents a value-over-time curve (used for level, e.g., how bright an effect gets over its duration). A `Curve` can either hold its own point data directly, or be a "library reference" — a named pointer into a shared `CurveLibrary` of reusable curves that many effects can share. When a `Curve` is a library reference, reading its `Points` property calls `CheckLibraryReference()` first, which pulls the current library curve's data into the instance's own private `_points` field as a side effect of the read.

`ColorGradient` (`src/Vixen.Modules/App/ColorGradients/ColorGradient.cs`) is the analogous concept for color: a gradient can hold its own color/alpha point lists directly, or reference a shared `ColorGradientLibrary` entry by name. Reading `Colors`, `Alphas`, `Title`, or `Gammacorrected` all call `CheckLibraryReference()` first, with the same materialize-into-own-fields side effect.

This "read materializes library data into private fields" behavior matters for a different reason than an earlier draft of this plan assumed: saved effect defaults **keep** their library link name (see `Decision Log`), so this is not used to make unlinking safe — it is what makes *live tracking* work. When a default whose `Curve` or `ColorGradient` still carries a `LibraryReferenceName` is deserialized into a brand-new effect, the non-`[DataMember]` fields that cache the resolved library object (`LibraryReferencedCurve`, `_libraryReferencedGradient`) come back `null`, so the very next read of `.Points`/`.Colors`/etc. calls `CheckLibraryReference()` → `UpdateLibraryReference()`, which looks the name up in the library again and pulls in whatever that library entry currently contains — not what it contained back when the default was saved. If the user has since edited the library curve or gradient, the new effect picks up the edit automatically. If the library entry was deleted or renamed in the meantime, `UpdateLibraryReference()` (`Curve.cs:151-169`, `ColorGradient.cs:1360-1377`) clears the link name on its own and falls back to whatever point/color data was last captured in the payload — so a saved default is never left broken even though this feature does nothing special to protect against it.

A "Mark Collection" is a named track of timeline marks (points in time) within one specific sequence, identified by a `Guid`. Several effect data models — for example `AlternatingData`, `DissolveData`, `FireworksData`, `LipSyncData`, the nested `EmitterData` inside the Liquid effect, `ShapesData`, `StateData`, `StrobeData`, `TextData`, and the nested `WaveformData` inside the Wave effect — store a `MarkCollectionId` field pointing at a specific Mark Collection in whatever sequence the effect currently lives in. Such a `Guid` is meaningless outside that one sequence, so it must never be captured into a saved default; the scrubbing step (described below) resets exactly these ten members back to their type's `default` value (`Guid.Empty`, since all ten are `Guid`).

`Paths.DataRootPath` (`src/Vixen.Core/Sys/Paths.cs`) is the root folder of the currently-loaded Vixen "profile" (a user's saved configuration set — controllers, sequences, preferences). It is assigned once during `VixenSystem.Start` (`src/Vixen.Core/Sys/VixenSystem.cs:77`). Types decorated with the `[DataPath]` attribute (`src/Vixen.Core/Sys/Attribute/DataPathAttribute.cs`) that expose a `public static readonly string` field are automatically scanned at startup by `Paths._BuildDataDirectories()` (`src/Vixen.Core/Sys/Paths.cs:64`), which creates that directory on disk if it does not exist. Critically, that scan only reflects over `Assembly.GetExecutingAssembly().GetTypes()` — i.e., only types physically compiled into the `Vixen.Core` assembly — so the new `[DataPath]` constant for this feature's storage folder must be declared on a type that lives in `Vixen.Core`, not in a `Vixen.Modules` project, or the folder will never get auto-created.

`Modules.GetDescriptorById(Guid moduleTypeId)` (`src/Vixen.Core/Sys/Modules.cs:361`) is the existing lookup that resolves a module's static descriptor (including its `ModuleDataClass`, the `System.Type` of its data model) from the module's `TypeId` GUID. This is the mechanism this design uses to resolve a stored payload's `System.Type` at load time, instead of persisting an `AssemblyQualifiedName` string and calling `Type.GetType(...)` on it (which is what the existing effect clipboard code at `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs:39-59` does — that is acceptable there because a clipboard payload only needs to survive a few seconds, not across a Vixen version upgrade where the assembly version embedded in an `AssemblyQualifiedName` could go stale).

The effect editor property grid — where the two new buttons for this feature will live — is `EffectPropertyEditorGrid` (`src/Vixen.Modules/Editor/EffectEditor/EffectPropertyEditorGrid.cs`), a WPF control. Its visual header layout is defined in two XAML resource dictionaries under `src/Vixen.Modules/Editor/EffectEditor/Design/`: `CategorizedLayout.xaml` (the layout actually used at runtime — `EffectPropertyEditorGrid.cs:1126` hard-assigns `Layout = new CategorizedLayout()`, and nothing in `FormEffectEditor` overrides it) and `AlphabeticalLayout.xaml` (an apparently-unused alternate layout that should still be kept in parity so the two templates do not silently diverge). `CategorizedLayout.xaml` already has a header `Grid` (lines 17-31) with a `Label` for the effect name and a `TextBlock` containing a `NavigatableHyperLink` with a `/Resources;component/help.png` icon; the two new buttons belong in that same `Grid`, to the left of the existing help hyperlink, using two new `Auto`-width columns. `AlphabeticalLayout.xaml`'s header (lines 13-17) is currently a bare `Border` containing only a `Label`, with no `Grid` at all and no help hyperlink — it needs the same `Grid` structure added from scratch to carry the two new buttons in parity.

Commands for the property grid are centralized as static `RoutedUICommand`s in `PropertyGridCommands` (`src/Vixen.Modules/Editor/EffectEditor/Input/PropertyGridCommands.cs`) — existing examples include `ShowFilter`, `ResetFilter`, `Reload`. Command bindings (which method runs when a command fires, and when it is allowed to fire) are wired up in `EffectPropertyEditorGrid`'s partial class `PropertyGrid.Commands.cs` (`src/Vixen.Modules/Editor/EffectEditor/PropertyGrid.Commands.cs`), inside `InitializeCommandBindings()` (line 31).

The icons needed for the two buttons — a floppy disk icon for Save, and a circular-refresh icon for Reset — already exist in the shared resources project at `src/Vixen.Common/Resources/disk.png` and `src/Vixen.Common/Resources/arrow_refresh.png`, and are referenced with the same `/Resources;component/...` WPF pack-URI syntax already used for `help.png`.

Confirmation dialogs use `MessageBoxService.GetUserConfirmation(string question, string title)` (`src/Vixen.Common/WPFCommon/Services/MessageBoxService.cs:44`), which returns a Yes/No `MessageBoxResult`.

Unit tests live in `src/Vixen.Tests/`, organized into subfolders roughly mirroring the source tree (for example `src/Vixen.Tests/Effects/`, `src/Vixen.Tests/Sys/`). They use xUnit. Building the test project requires full MSBuild rather than plain `dotnet build`, because two of its transitive dependencies (`QMLibrary`, `LiquidFunWrapper`) are C++/CLI projects that `dotnet`'s bundled MSBuild toolchain cannot resolve; see the `Build` section of `CLAUDE.md` at the repository root for the exact two-step build-then-test command sequence, which is also repeated verbatim in this plan's `Concrete Steps` section below.

## Plan of Work

### Milestone 1 — Jira tracker update (no code)

Before any code changes, read the project Jira skill at `.agents/skills/jira/SKILL.md`, then update Jira issue VIX-3964 (currently status "New Ticket", Improvement, Normal priority) so its description contains this ExecPlan's purpose statement, the full resolved-decisions summary from `docs/reviews/vix-3964-effect-default-settings-design.md`, the milestone breakdown below, and the acceptance criteria and test plan from the `Validation and Acceptance` section of this document. This lets a developer, tester, or product owner understand and validate the intended behavior from Jira alone, without needing to open this repository. Do not transition the issue's status as part of this milestone — only update its description. Record the Jira update in `Progress` with a timestamp once done.

### Milestone 2 — Core storage (no user-visible behavior yet)

This milestone builds everything needed to capture, scrub, store, and retrieve a saved default, proven entirely by automated tests — no UI changes yet.

Saved effect defaults intentionally keep their `Curve`/`ColorGradient` library links exactly as captured — no interface or scrubbing step severs them (see `Decision Log` for why an earlier draft's `ILibraryLinkable` unlink step was dropped). `ColorGradient` and `Curve` are not modified by this milestone at all.

In `src/Vixen.Core/Sys/Attribute/ExcludeFromEffectDefaultAttribute.cs` (new file, alongside the existing `DataPathAttribute.cs` and `ModuleDataPathAttribute.cs` in that same folder), define an attribute targeting properties and fields:

    namespace Vixen.Sys.Attribute
    {
        [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
        public sealed class ExcludeFromEffectDefaultAttribute : System.Attribute
        {
        }
    }

Apply `[ExcludeFromEffectDefault]` to exactly these ten members (do not add or omit any):

    src/Vixen.Modules/Effect/Alternating/AlternatingData.cs:35       (public Guid MarkCollectionId { get; set; })
    src/Vixen.Modules/Effect/Dissolve/DissolveData.cs:31              (public Guid MarkCollectionId { get; set; })
    src/Vixen.Modules/Effect/Fireworks/FireworksData.cs:112           (public Guid MarkCollectionId { get; set; })
    src/Vixen.Modules/Effect/LipSync/LipSyncData.cs:59                (public Guid MarkCollectionId { get; set; })
    src/Vixen.Modules/Effect/Liquid/Liquid/EmitterData.cs:209         (public Guid MarkCollectionId { get; set; } — nested inside List<EmitterData>)
    src/Vixen.Modules/Effect/Shapes/ShapesData.cs:193                 (public Guid MarkCollectionId { get; set; })
    src/Vixen.Modules/Effect/State/StateData.cs:60                    (public Guid MarkCollectionId { get; set; } = Guid.Empty;)
    src/Vixen.Modules/Effect/Strobe/StrobeData.cs:37                  (public Guid MarkCollectionId { get; set; })
    src/Vixen.Modules/Effect/Text/TextData.cs:121                     (public Guid MarkCollectionId { get; set; })
    src/Vixen.Modules/Effect/Wave/Wave/WaveformData.cs:114            (public Guid MarkCollectionId { get; set; } — nested inside List<WaveformData>)

Do not annotate `src/Vixen.Modules/Effect/Liquid/Liquid/Emitter.cs:863` or `src/Vixen.Modules/Effect/Wave/Wave/Waveform.cs:389` — both are runtime wrapper classes around the serialized data, not the data models themselves, and are never part of a serialized `ModuleData` graph.

In `src/Vixen.Core/Services/EffectDefaults/EffectDefaultsStore.cs` (new file, new folder), define the on-disk `[DataContract]` shape:

    [DataContract]
    public class EffectDefaultEntry
    {
        [DataMember] public Guid TypeId { get; set; }
        [DataMember] public string EffectName { get; set; }
        [DataMember] public string DataModelTypeName { get; set; }
        [DataMember] public DateTime SavedUtc { get; set; }
        [DataMember] public byte[] Payload { get; set; }
    }

    [DataContract]
    public class EffectDefaultsStore
    {
        [DataMember] public int Version { get; set; } = 1;
        [DataMember] public List<EffectDefaultEntry> Entries { get; set; } = new List<EffectDefaultEntry>();
    }

`EffectName` and `DataModelTypeName` are never used to resolve behavior; they exist purely so a human reading the diagnostic XML dump (Milestone 4) can identify entries, and so a load-time type mismatch (the resolved `ModuleDataClass.FullName` does not match the stored `DataModelTypeName`) can be detected and logged as a warning rather than silently producing corrupt data.

In `src/Vixen.Core/Services/EffectDefaults/EffectDefaultScrubber.cs` (new file), implement the depth-first, cycle-guarded scrub walk. This walk exists solely to find and blank out `[ExcludeFromEffectDefault]` members — it does not touch library links. Use a `HashSet<object>` built with `ReferenceEqualityComparer.Instance` to guard against cycles (reference types only — box a value type and every visit is a distinct boxed reference, so there is no point guarding on them, and skipping that guard for value types is harmless). Cap recursion depth at 32 to guarantee termination even in the presence of an undiscovered cycle. The walk must:

    1. Return immediately if the node is null or the depth exceeds 32.
    2. Return immediately if the node is a reference type already present in the visited set; otherwise add it.
    3. If the node is `IEnumerable` and not a `string`, recurse into every element and return (do not also walk it as an object with members). This is required because two of the ten excluded members, `EmitterData.MarkCollectionId` and `WaveformData.MarkCollectionId`, are nested inside `List<EmitterData>`/`List<WaveformData>`.
    4. Otherwise, reflect over every public `[DataMember]`-decorated property and field on the node's runtime type — `BindingFlags.Public | BindingFlags.Instance`, both `PropertyInfo` and `FieldInfo` — and for each member: if it carries `[ExcludeFromEffectDefault]`, set it to `default(TMember)` and continue; otherwise read its value, skip if null or if the member's type is a "leaf" type (see below), and otherwise recurse into the value at depth+1. Public-only reflection is sufficient because all ten `[ExcludeFromEffectDefault]` members are public auto-properties; see `Decision Log` for why an earlier draft required non-public access.

"Leaf" types that never get recursed into: all primitive numeric types, `string`, `decimal`, `DateTime`, `TimeSpan`, `Guid`, any `enum`, and `System.Drawing.Color`.

In `src/Vixen.Core/Services/EffectDefaults/EffectDefaultSummary.cs` (new file), define a small read-only projection type used by the UI and diagnostics to list saved defaults without exposing the raw payload — fields at minimum: `Guid TypeId`, `string EffectName`, `DateTime SavedUtc`, `bool ModuleInstalled` (true if `Modules.GetDescriptorById(TypeId)` currently resolves).

In `src/Vixen.Core/Services/EffectDefaults/EffectDefaultsService.cs` (new file), implement the public surface listed in the architecture review:

    public sealed class EffectDefaultsService
    {
        public static EffectDefaultsService Instance { get; }
        public bool HasDefault(Guid effectTypeId);
        public IModuleDataModel CreateDefaultData(IModuleInstance effectModule);
        public void SaveDefault(IEffectModuleInstance effect);
        public bool ClearDefault(Guid effectTypeId);
        public void Reload();
        public IReadOnlyCollection<EffectDefaultSummary> GetSummaries();
        public void Export(string path, IEnumerable<Guid> effectTypeIds);
        public EffectDefaultsImportResult Import(string path, ImportMode mode);
        public void WriteDiagnosticDump(string path);
    }

Declare the storage directory exactly following the `SystemConfig.Directory` precedent (`src/Vixen.Core/Sys/SystemConfig.cs:23`):

    [DataPath] public static readonly string Directory = Path.Combine(Paths.DataRootPath, "Effect Defaults");
    public const string FileName = "EffectDefaults.vfd";

Because `Paths._BuildDataDirectories()` only reflects over the `Vixen.Core` assembly, this constant must be declared on a type physically inside `Vixen.Core` (which `EffectDefaultsService` already is) for the folder to be auto-created at startup.

Implement `SaveDefault` following the capture pipeline established in `Decision Log`:

    var dataType = effect.Descriptor.ModuleDataClass;
    var serializer = GetOrAddSerializer(dataType);          // ConcurrentDictionary<Type, DataContractSerializer> cache
    var liveBytes = WriteBinary(serializer, effect.ModuleData);   // reading properties here resolves library links
    var copy = (IModuleDataModel)ReadBinary(serializer, liveBytes); // independent object graph; live effect untouched
    EffectDefaultScrubber.Scrub(copy);
    var payloadBytes = WriteBinary(serializer, copy);
    upsert an EffectDefaultEntry keyed by effect.Descriptor.TypeId into the in-memory store, then persist atomically (write to a temp file in the same directory, then File.Replace/Move over the real file, so a crash mid-write cannot corrupt the existing store).

Implement `CreateDefaultData` to never throw:

    if the payload cache has no entry for typeId, return null immediately (no I/O — this is the common case for effects that have no saved default).
    resolve dataType from Modules.GetDescriptorById(typeId)?.ModuleDataClass; if null, or if dataType.FullName does not match the stored DataModelTypeName, log a warning and return null.
    try to deserialize the payload with the cached serializer for dataType and return the result; on any exception, log a warning and return null.

Every caller of `CreateDefaultData` must treat a `null` result as "fall back to the effect's normal built-in constructor" — never propagate an exception into an effect-creation code path.

Implement lazy, profile-aware loading: on first access to any service member, if the store has not yet been loaded, or if the recorded root path used to build the current in-memory cache no longer matches the current `Paths.DataRootPath`, reload from disk (treating a missing or corrupt file as an empty store, logging the condition, and continuing). `Reload()` exposes this same reload logic for explicit invalidation after an import (Milestone 4).

Back the payload cache with `ConcurrentDictionary<Guid, byte[]>` and the per-type serializer cache with `ConcurrentDictionary<Type, DataContractSerializer>`, because effects are created both on the UI thread (interactive editing) and on background threads (sequence load, the web server). Verify during implementation that concurrent `ReadObject`/`WriteObject` calls on a single shared `DataContractSerializer` instance are stable under this workload (they are documented as thread-safe for this usage, but confirm empirically with a stress test in this milestone's test suite); if any instability appears, fall back to a per-type lock around serializer use and record that finding in `Surprises & Discoveries`.

Add automated tests under `src/Vixen.Tests/` (create a new folder such as `src/Vixen.Tests/EffectDefaults/`) covering:

    - Scrubber: a member carrying `[ExcludeFromEffectDefault]` is reset to `default(T)` both at the top level of an object and when nested inside a `List<T>` element (mirroring the two nested cases, `EmitterData` and `WaveformData`).
    - Scrubber: a `Curve` or `ColorGradient` member that is a library reference is left completely untouched — its `LibraryReferenceName` and materialized points/colors are unchanged after scrubbing, proving the walk does not sever library links.
    - Scrubber: a deliberately constructed reference cycle does not stack-overflow or infinite-loop; the walk terminates.
    - Store round-trip: an `EffectDefaultsStore` written to a temp file and reloaded produces an equal set of entries.
    - Regression test proving `SaveDefault` does not mutate the live effect's `ModuleData` — specifically, construct a `PulseData` with a non-library `LevelCurve`, call `SaveDefault`, then assert the original `PulseData.LevelCurve` reference and its points are unchanged. This is the direct regression test for the `PulseData.CreateInstanceForClone` shared-reference hazard documented in `Decision Log`.

### Milestone 3 — Editor integration (first user-visible behavior)

Add two new `RoutedUICommand`s to `PropertyGridCommands` (`src/Vixen.Modules/Editor/EffectEditor/Input/PropertyGridCommands.cs`), following the exact pattern of the existing `Reload` command (private static field, public static property, XML doc summary):

    SaveEffectDefault  — text "Save as Default"
    ClearEffectDefault — text "Reset to Built-in"

Wire up their `CommandBinding`s in `PropertyGrid.Commands.cs` (`src/Vixen.Modules/Editor/EffectEditor/PropertyGrid.Commands.cs`), inside `InitializeCommandBindings()` (line 31). `SaveEffectDefault`'s `CanExecute` handler returns true only when exactly one effect is selected (`SelectedObjects.Length == 1`) and `SelectedObjects[0] as IEffectModuleInstance` is non-null. `ClearEffectDefault`'s `CanExecute` handler additionally requires `HasStoredDefault` to be true. Executing `SaveEffectDefault` casts the selected object to `IEffectModuleInstance` (no-op if the cast fails) and calls `EffectDefaultsService.Instance.SaveDefault(effect)`; no confirmation dialog is shown, and the Reset button immediately becoming enabled afterward serves as sufficient implicit confirmation that the save succeeded. Executing `ClearEffectDefault` shows a Yes/No confirmation via `MessageBoxService.GetUserConfirmation($"Remove the saved default for {effect.Descriptor.TypeName}?", "Reset Effect Default")` — the effect's display name (for example "Pulse" or "Wipe") is interpolated into the prompt so the user can tell which effect type is being reset, since the property grid can be showing any effect at the time (see `Decision Log`) — and, only on Yes, calls `EffectDefaultsService.Instance.ClearDefault(typeId)`; it must not alter the currently open effect's `ModuleData` in any way, since resetting only deletes the stored entry.

Add a `HasStoredDefault` notify-property to `EffectPropertyEditorGrid` (`src/Vixen.Modules/Editor/EffectEditor/EffectPropertyEditorGrid.cs`), recomputed whenever `SelectedObjects` changes (there is already a `SelectedObjectsChanged` event / `OnSelectedObjectsChanged()` override point at line 550 to hook this into): `HasStoredDefault = SelectedObjects.Length == 1 && SelectedObjects[0] is IEffectModuleInstance effect && EffectDefaultsService.Instance.HasDefault(effect.TypeId)`.

Add the two buttons to `CategorizedLayout.xaml`'s header `Grid` (`src/Vixen.Modules/Editor/EffectEditor/Design/CategorizedLayout.xaml`, lines 17-31), to the left of the existing help hyperlink, adding two new `Auto`-width `ColumnDefinition`s. Use `/Resources;component/disk.png` for Save and `/Resources;component/arrow_refresh.png` for Reset, bound to the new commands, with the Reset button's `IsEnabled` bound to `HasStoredDefault` (or relying on `CanExecute`, whichever is the established idiom elsewhere in this file — check how `ResetFilter`/`Reload` buttons are already wired, if any exist visually, and match that idiom). Mirror the identical button structure into `AlphabeticalLayout.xaml` (`src/Vixen.Modules/Editor/EffectEditor/Design/AlphabeticalLayout.xaml`), which currently has no `Grid` in its header at all (just a bare `Border` containing a `Label`) — add the same `Grid` structure used in `CategorizedLayout.xaml` so the two templates do not diverge, even though `AlphabeticalLayout` may currently be dead code (confirm during implementation whether anything still constructs it before treating it as safe to skip, and record the finding in `Surprises & Discoveries`).

Add the new private helper to `TimedSequenceEditorForm` (`src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`):

    /// <remarks>Do not use where the caller immediately overwrites ModuleData
    /// (clone, paste) — the default would be materialized and thrown away.</remarks>
    private IEffectModuleInstance CreateEffectInstanceWithDefaults(Guid effectTypeId)
    {
        var effect = ApplicationServices.Get<IEffectModuleInstance>(effectTypeId);
        if (effect != null)
        {
            var defaultData = EffectDefaultsService.Instance.CreateDefaultData(effect);
            if (defaultData != null)
            {
                effect.ModuleData = defaultData;
            }
        }
        return effect;
    }

(Confirm the exact `ModuleData` assignment pattern already used at the surrounding call sites during implementation — some sites may need `effect.ModuleData = defaultData;` directly, others may go through a different setter; match whatever the existing code around each of the six call sites already does when it needs to replace an effect's data model, and note any deviation from the sketch above in `Surprises & Discoveries`.)

Replace `ApplicationServices.Get<IEffectModuleInstance>(...)` with `CreateEffectInstanceWithDefaults(...)` at exactly these six locations (this list is the specification — do not skip or add sites without updating this document first):

    TimedSequenceEditorForm.cs:2272  — draw effect on timeline
    TimedSequenceEditorForm.cs:2571  — Add Multiple Effects dialog
    TimedSequenceEditorForm.cs:2610  — add effects at marks
    TimedSequenceEditorForm.cs:3779  — AddNewEffectById (toolbox drop, hotkeys)
    TimedSequenceEditorForm.cs:4229  — replace selected elements with a different effect type
    TimedSequenceEditorForm.cs:4942  — drag media file onto timeline

Line numbers will shift slightly once Milestone 2's new `using` statements and any earlier edits land; treat the listed line numbers as of the current `master`-derived working tree (see `git log` in `Concrete Steps`) and re-locate each site by its call pattern (`ApplicationServices.Get<IEffectModuleInstance>(...)` at that logical location) if line numbers have drifted by the time this milestone starts.

At site 4942 (drag media file onto timeline), the existing code sets specific properties (`PictureSource`, filename) on the newly created effect immediately after construction — applying saved defaults first and then letting those explicit property assignments run afterward is correct and intentional, since the explicit assignments must win.

Leave these three sites untouched, still calling the plain `ApplicationServices.Get<IEffectModuleInstance>(...)`:

    TimedSequenceEditorForm.cs:3579                 — CloneElements (assigns ModuleData.Clone() on the very next statement)
    TimedSequenceEditorForm.cs:5321                 — paste (assigns the clipboard's captured ModuleData on the very next statement)
    TimedSequenceEditorForm.cs:6091                 — LipSync (sets its own data)
    TimedSequenceEditorForm_Menu.cs:533              — LipSync (sets its own data)

Add or update automated tests proving `CreateEffectInstanceWithDefaults` applies a saved default when one exists and falls through to the built-in constructor result when none exists or `CreateDefaultData` returns `null`. If `TimedSequenceEditorForm` is not practically unit-testable in isolation (it is a WinForms-hosted WPF editor form with substantial UI and application-service dependencies), a passing alternative is to unit test `EffectDefaultsService.CreateDefaultData` directly (already covered in Milestone 2) plus a narrowly-scoped test of the helper's decision logic extracted into a small testable method if one is introduced; record whichever approach is actually taken, and why, in `Decision Log`.

### Milestone 4 — Export, import, diagnostics

Extend `TimedSequenceEditorForm_Menu.cs` (`src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs`) with three new menu commands: Export Effect Defaults, Import Effect Defaults, and a diagnostic "Dump Effect Defaults (Readable XML)" command. Add a small selection form (new file under the TimedSequenceEditor editor's UI folder) that lists the current `EffectDefaultSummary` entries (from `EffectDefaultsService.Instance.GetSummaries()`) with checkboxes, used by both Export (choose which saved defaults to write out) and, optionally, Import (choose which entries from the imported file to bring in, if the format supports partial import — otherwise import brings in everything in the file and merges).

Implement `EffectDefaultsService.Export(string path, IEnumerable<Guid> effectTypeIds)`: build a new `EffectDefaultsStore` containing only the requested entries (by their already-known payload bytes — no re-serialization needed since payloads are stored pre-scrubbed) and write it with the same binary `DataContractSerializer` format used for the primary store file.

Implement `EffectDefaultsService.Import(string path, ImportMode mode)`: read the file with the same binary format, and for each entry, upsert it into the current in-memory store, overwriting any existing entry for the same `TypeId` (define `ImportMode` as an enum if different merge strategies are needed — at minimum an "overwrite existing" mode is required; add a "skip existing" mode only if the review's transfer requirements call for it, otherwise keep this to the single documented merge behavior "merge on import" from the architecture review). After import completes, call `Reload()` so any UI already showing `GetSummaries()` picks up the change, and persist the merged store to disk.

Implement `EffectDefaultsService.WriteDiagnosticDump(string path)`: serialize the exact same in-memory `EffectDefaultsStore` object graph used for the binary store, but through `XmlWriter.Create(path, new XmlWriterSettings { Indent = true })` instead of a binary `XmlDictionaryWriter`, so the readable dump can never drift out of sync with what is actually stored — it is produced from the same objects, just written differently.

Add tests covering: export writes only the requested subset; import merges new entries and overwrites existing entries for the same `TypeId` without disturbing untouched entries; the diagnostic dump is valid, indented XML containing the same `TypeId`/`EffectName`/`SavedUtc` values as the in-memory store.

### Milestone 5 — Documentation

Add a new document under `docs/effects/` (a new file, e.g. `docs/effects/effect-defaults.md`) describing, for a Vixen user (not a developer), what the Save/Reset buttons do, where the saved defaults are stored, that they are per-profile, and how export/import and the diagnostic dump work. Cross-reference this ExecPlan and the architecture review for implementers who need the technical detail. Per `CLAUDE.md`'s "Use Docs First" guidance, this becomes the primary reference other contributors should consult before changing this feature's behavior in the future — keep it accurate as the source of truth going forward.

### Milestone 6 — Final Jira alignment and closeout

Update this ExecPlan's `Outcomes & Retrospective` section with a full summary of what was built, any deviations from the plan (cross-referenced to `Decision Log` entries recorded along the way), and the final validation results. Then update Jira issue VIX-3964's description if the final implementation diverged from what Milestone 1 recorded, and add a closeout comment containing the implementation summary, automated validation results (exact test command output counts), manual validation results (the walkthrough described in `Validation and Acceptance` below), and any residual risk or follow-up work. Per project convention (see `.agents/skills/jira/SKILL.md` and prior ExecPlans such as `docs/plans/effects/vix-3946-lipsync-phoneme-mark-collections.md`), do not transition the issue's status — leave that to the PR/merge process.

## Concrete Steps

All commands below run from the repository root, `C:\Dev\Vixen`.

Build the full solution in Release configuration:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release

Build and run the test project (two-step process required because two transitive dependencies, `QMLibrary` and `LiquidFunWrapper`, are C++/CLI projects that plain `dotnet test` cannot build):

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)/"

Run a focused subset of only the new tests once Milestone 2's test folder exists (adjust the filter to match whatever test-class names are actually used):

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)/" --filter "FullyQualifiedName~EffectDefaults"

This section must be updated with the exact commands actually run and a short excerpt of their output as each milestone completes, per `.agents/PLANS.md`'s "Capture evidence" requirement.

## Validation and Acceptance

Automated acceptance: after Milestone 2, the full test command above passes with the new `EffectDefaults` test suite included, and in particular the regression test proving `SaveDefault` does not mutate `PulseData.LevelCurve` on the live effect passes both before manual inspection confirms the bug it targets is real (it should fail if the capture pipeline were changed to use `Clone()`, as a sanity check during development) and after the correct serialize/deserialize pipeline is implemented.

Manual acceptance, performed in the running application after Milestone 3 (build in Debug via `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`, launch the built application from `/Debug/Output/`, open or create a sequence in the Timed Sequence Editor):

    1. Add a Pulse effect to the timeline. Open it in the effect editor; confirm it shows Vixen's built-in default level curve (a ramp) and default gradient (solid white).
    2. Change the level curve shape and the color gradient to something visibly different (for example, a flat curve and a red-to-blue gradient).
    3. Click the new "Save as Default" button in the effect editor header. Confirm the "Reset to Built-in" button becomes enabled.
    4. Add a brand-new Pulse effect anywhere else on the timeline (try at least two of: drawing on the timeline, the toolbox, a hotkey). Open it in the effect editor and confirm it already shows the custom curve and gradient from step 2, not the built-in defaults.
    5. Click "Reset to Built-in" on the effect from step 4 (or any Pulse effect); confirm the Yes/No confirmation appears, confirm Yes, and confirm the button becomes disabled again. Confirm the effect currently open in the editor is unchanged (its `ModuleData` still shows the custom curve/gradient it already had — Reset only affects future new instances).
    6. Add one more brand-new Pulse effect and confirm it now reverts to the original built-in default curve and gradient.
    7. Repeat steps 1-4 for at least one effect with a library-referenced gradient (create a gradient in the Color Gradient Library, apply it to an effect, save it as the default). Then edit that library gradient's colors and confirm a *new* default-derived effect created afterward shows the *edited* gradient, not the colors that were in the library at save time — this proves saved defaults keep their library link live rather than freezing a snapshot. Finally, delete that library gradient entirely and confirm one more new default-derived effect still comes up with the gradient's last-known values instead of an error or a visibly broken/blank gradient — this is the pre-existing `UpdateLibraryReference()` self-healing behavior in `ColorGradient`/`Curve`, not something this feature had to add.
    8. Repeat steps 1-4 for an effect with a `MarkCollectionId` (for example, Alternating in Mark Collection mode) and confirm a new default-derived instance in a *different* sequence does not inherit the specific Mark Collection selection from the sequence where the default was saved (it should show no collection selected, or the mode's fallback behavior, not a `Guid` pointing at nothing).

Manual acceptance for Milestone 4 (export/import/diagnostics): export the saved Pulse default to a file, delete it via Reset, import the file back, and confirm a new Pulse effect again shows the previously-saved settings. Run the diagnostic dump command and confirm the resulting file is readable, indented XML containing the Pulse entry with a human-readable `EffectName` and `SavedUtc`.

## Idempotence and Recovery

All storage writes (the primary `.vfd` file, exports) use a write-to-temporary-file-then-atomic-replace pattern, so an interrupted write (crash, forced shutdown) leaves the previous valid file intact rather than a half-written corrupt one. A corrupt or missing store file is treated as an empty store at load time, logged, and does not throw — the application continues to start normally and simply behaves as if no defaults were ever saved. Re-running "Save as Default" on the same effect at any time is safe and simply overwrites that effect type's single stored entry (`upsert entry by TypeId`), so there is no accumulation of stale entries from repeated saves. Import merges are safe to re-run: importing the same file twice produces the same end state as importing it once, because each entry is upserted by `TypeId`. Deleting the entire `{Paths.DataRootPath}\Effect Defaults\` folder is always a safe, fully reversible way to reset this feature to its pre-implementation state for a given profile — the folder is re-created automatically on next start via the `[DataPath]` mechanism, and every existing effect and sequence is completely unaffected because nothing in this feature ever touches sequence files or module store data.

## Artifacts and Notes

The architecture review at `docs/reviews/vix-3964-effect-default-settings-design.md` is the primary design source for this plan and contains a compressed "Hand-off context" data dump at its end intended for spec-generation tooling; that dump's content has been fully absorbed into this ExecPlan's `Context and Orientation`, `Plan of Work`, and `Decision Log` sections; it is not duplicated verbatim here to avoid a second, driftable source of truth. If a future revision of this plan needs to re-check a specific citation, re-verify it against the current working tree rather than trusting either document.

## Interfaces and Dependencies

New public/protected surface introduced by this plan (all require XML documentation per `CLAUDE.md`'s "XML Docs" section, using the `.agents/skills/csharp-docs/SKILL.md` skill):

In `src/Vixen.Core/Sys/Attribute/ExcludeFromEffectDefaultAttribute.cs`, define:

    namespace Vixen.Sys.Attribute
    {
        public sealed class ExcludeFromEffectDefaultAttribute : System.Attribute { }
    }

In `src/Vixen.Core/Services/EffectDefaults/EffectDefaultsService.cs`, define:

    namespace Vixen.Services.EffectDefaults
    {
        public sealed class EffectDefaultsService
        {
            public static EffectDefaultsService Instance { get; }
            public bool HasDefault(System.Guid effectTypeId);
            public Vixen.Module.IModuleDataModel CreateDefaultData(Vixen.Module.IModuleInstance effectModule);
            public void SaveDefault(Vixen.Module.Effect.IEffectModuleInstance effect);
            public bool ClearDefault(System.Guid effectTypeId);
            public void Reload();
            public System.Collections.Generic.IReadOnlyCollection<EffectDefaultSummary> GetSummaries();
            public void Export(string path, System.Collections.Generic.IEnumerable<System.Guid> effectTypeIds);
            public EffectDefaultsImportResult Import(string path, ImportMode mode);
            public void WriteDiagnosticDump(string path);
        }
    }

`EffectDefaultsService` depends on `Modules.GetDescriptorById` (`Vixen.Sys.Modules`, `src/Vixen.Core/Sys/Modules.cs`) to resolve payload types, `Paths.DataRootPath` (`Vixen.Sys.Paths`, `src/Vixen.Core/Sys/Paths.cs`) for its storage location, and `System.Runtime.Serialization.DataContractSerializer` plus `System.Xml.XmlDictionaryWriter`/`XmlDictionaryReader` for the binary wire format, matching the precedent in `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs`.

`ColorGradient` (`src/Vixen.Modules/App/ColorGradients/ColorGradient.cs`) and `Curve` (`src/Vixen.Modules/App/Curves/Curve.cs`) are not modified by this plan; their existing library-linking behavior is used as-is so saved defaults keep tracking live library edits.

`PropertyGridCommands` (`src/Vixen.Modules/Editor/EffectEditor/Input/PropertyGridCommands.cs`) gains two new `public static RoutedUICommand` properties: `SaveEffectDefault` and `ClearEffectDefault`.

`EffectPropertyEditorGrid` (`src/Vixen.Modules/Editor/EffectEditor/EffectPropertyEditorGrid.cs`) gains a new `public bool HasStoredDefault { get; }` notify-property.

`TimedSequenceEditorForm` (`src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`) gains a new `private IEffectModuleInstance CreateEffectInstanceWithDefaults(Guid effectTypeId)` method (private, so no XML doc is required by `CLAUDE.md`'s rule, but the `<remarks>` shown in `Plan of Work` documenting its "do not use where ModuleData is immediately overwritten" caveat should still be included as it is load-bearing guidance for future maintainers).

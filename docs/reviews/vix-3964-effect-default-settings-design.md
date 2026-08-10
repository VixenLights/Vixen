# Architecture Design: VIX-3964 — Add option to set effect default settings

**Issue:** [VIX-3964](https://vixenlights.atlassian.net/browse/VIX-3964) — Improvement, Normal priority, status *New Ticket*
**Status:** Design complete, ready for hand-off to implementation
**Target spec path:** `docs/plans/effects/vix-3964-effect-default-settings.md`

## Purpose

A user configures an effect the way they like it — colours, gradients, curves, and every other
setting — and presses one button in the effect editor header. From then on, every new instance of
that effect they create in the sequence editor starts with those settings instead of the settings
hard-coded into Vixen. A second button removes the saved default so the effect reverts to
Vixen's built-in behaviour for future instances.

## Resolved decisions

| Decision | Choice |
|---|---|
| Scope of captured settings | The effect's `ModuleData` only |
| Storage scope | Profile-level, with export/import between profiles |
| Storage location | `{Paths.DataRootPath}\Effect Defaults\EffectDefaults.vfd`, single file |
| Wire format | Binary `DataContractSerializer` (`XmlDictionaryWriter.CreateBinaryWriter`) |
| Readable diagnostics | On-demand menu command producing indented XML |
| Sequence-scoped values | Stripped via an opt-out attribute |
| Library-linked items | Unlinked; underlying data copied in |
| "Reset" button | Deletes the stored default only; does not touch the open effect |
| Save button availability | Single-effect selection only |
| Granularity | Whole data model, only for effects the user explicitly saved |
| Injection point | The Timed Sequence Editor's add-effect paths only |

## Core Strategy

A new profile-scoped service in `Vixen.Core` owns a store of per-effect-type default `ModuleData`
payloads, deliberately kept **out of the module store** (`VixenSystem.ModuleStore`) so it
round-trips independently and can be exported. Saving captures the currently edited effect's data
model; adding a new effect in the sequence editor materialises from that store instead of calling
the data model's constructor.

Three findings from the codebase shape the design and must not be discarded.

**`Clone()` cannot be used to capture the default.** `PulseData.CreateInstanceForClone`
(`src/Vixen.Modules/Effect/Pulse/PulseData.cs:23`) assigns `result.LevelCurve = LevelCurve` — a
shared reference, not a copy. Several data models are shallow in this way. If the save path cloned
and then unlinked library references, it would mutate the *live* effect the user is editing. The
capture path therefore deep-copies by serialize→deserialize before touching anything.

**Serialization order makes unlinking lossless.** `Curve.Points`
(`src/Vixen.Modules/App/Curves/Curve.cs:73`) and the `ColorGradient` accessors call
`CheckLibraryReference()` in their getters, which materialises the library's points into the
instance's own `[DataMember]` fields. So the bytes produced when serializing a live effect already
contain fully-resolved point data alongside the link name. Clearing the link name afterwards on the
copy discards nothing — which is exactly the "remove the link and copy the underlying data" outcome
the ticket asks for, and it removes the dangling-library-item risk entirely.

**Only `Curve` and `ColorGradient` are reachable library-linkable types.** `LipSyncData` stores its
phoneme map as a plain `string` name (`src/Vixen.Modules/Effect/LipSync/LipSyncData.cs:16`), not an
embedded `LipSyncMapData`, so the `LipSyncMapData.PictureDirectory` breakage that unlinking would
otherwise cause cannot occur.

The capture pipeline is therefore: **serialize live → deserialize copy → scrub copy → serialize
copy → store.**

## Data Model & Property Contracts

### `Vixen.Data.Library.ILibraryLinkable`

New, at `src/Vixen.Core/Data/Library/ILibraryLinkable.cs`. Single member `void UnlinkFromLibrary()`.
This lets `Vixen.Core` sever library links without referencing the module assemblies that define
`Curve` and `ColorGradient` — both of those projects already reference `Vixen.Core`, so the
dependency direction is correct.

`ColorGradient.UnlinkFromLibrary()` (`src/Vixen.Modules/App/ColorGradients/ColorGradient.cs:1379`)
already matches the signature. `Curve` implements the interface explicitly, delegating to the
existing `UnlinkFromLibraryCurve()` (`src/Vixen.Modules/App/Curves/Curve.cs:204`), so no redundant
public method appears on `Curve`.

### `Vixen.Sys.Attribute.ExcludeFromEffectDefaultAttribute`

New, alongside the existing `DataPathAttribute` and `ModuleDataPathAttribute` in
`src/Vixen.Core/Sys/Attribute/`. Targets `AttributeTargets.Property | AttributeTargets.Field`.

Marks members that are meaningful only within one sequence; the scrubber resets them to
`default(T)`. Apply to these **ten** members. Note that two are nested inside collections, which is
why the scrubber must recurse into collection elements:

| File | Line |
|---|---|
| `src/Vixen.Modules/Effect/Alternating/AlternatingData.cs` | 35 |
| `src/Vixen.Modules/Effect/Dissolve/DissolveData.cs` | 31 |
| `src/Vixen.Modules/Effect/Fireworks/FireworksData.cs` | 112 |
| `src/Vixen.Modules/Effect/LipSync/LipSyncData.cs` | 59 |
| `src/Vixen.Modules/Effect/Liquid/Liquid/EmitterData.cs` | 209 — **nested** in `List<EmitterData>` |
| `src/Vixen.Modules/Effect/Shapes/ShapesData.cs` | 193 |
| `src/Vixen.Modules/Effect/State/StateData.cs` | 60 |
| `src/Vixen.Modules/Effect/Strobe/StrobeData.cs` | 37 |
| `src/Vixen.Modules/Effect/Text/TextData.cs` | 121 |
| `src/Vixen.Modules/Effect/Wave/Wave/WaveformData.cs` | 114 — **nested** in `List<WaveformData>` |

Do **not** annotate `src/Vixen.Modules/Effect/Liquid/Liquid/Emitter.cs:863` or
`src/Vixen.Modules/Effect/Wave/Wave/Waveform.cs:389` — those are runtime wrappers, not serialized
data models.

### `EffectDefaultsStore` / `EffectDefaultEntry`

New `[DataContract]` types describing the on-disk shape.

`EffectDefaultEntry` fields: `TypeId` (`Guid`), `EffectName` (`string`, diagnostics only),
`DataModelTypeName` (`string`, diagnostics and mismatch detection only), `SavedUtc` (`DateTime`),
`Payload` (`byte[]`, the inner binary-DataContract encoding of the data model).

`EffectDefaultsStore` fields: `Version` (`int`, currently `1`) and `List<EffectDefaultEntry>`.

The payload type is **never resolved from `DataModelTypeName`**. Resolve it at load time from
`Modules.GetDescriptorById(typeId).ModuleDataClass`. Persisting an `AssemblyQualifiedName` and
calling `Type.GetType` — as `EffectModelCandidate.cs:39` does for the clipboard — embeds an assembly
version, which is acceptable for a clipboard payload that lives for seconds but would break this
file across Vixen releases. `DataModelTypeName` is stored only so a mismatch can be detected and
logged.

### `EffectDefaultsService`

New, under `src/Vixen.Core/Services/EffectDefaults/`. Public surface:

    static EffectDefaultsService Instance { get; }
    bool HasDefault(Guid effectTypeId)
    IModuleDataModel? CreateDefaultData(IModuleInstance effectModule)
    void SaveDefault(IEffectModuleInstance effect)
    bool ClearDefault(Guid effectTypeId)
    void Reload()
    IReadOnlyCollection<EffectDefaultSummary> GetSummaries()
    void Export(string path, IEnumerable<Guid> effectTypeIds)
    EffectDefaultsImportResult Import(string path, ImportMode mode)
    void WriteDiagnosticDump(string path)

Declare the directory with the existing auto-creation mechanism, mirroring `SystemConfig.Directory`
(`src/Vixen.Core/Sys/SystemConfig.cs:23`):

    [DataPath] public static readonly string Directory = Path.Combine(Paths.DataRootPath, "Effect Defaults");

`Paths._BuildDataDirectories` reflects only over `Assembly.GetExecutingAssembly()`, i.e.
`Vixen.Core`, so the constant must live in a `Vixen.Core` type for the folder to be created
automatically. Because it sits inside the profile folder, `DataZipForm` picks it up in support
archives with no extra work.

### Effect editor surface

Two `RoutedUICommand`s added to `PropertyGridCommands`
(`src/Vixen.Modules/Editor/EffectEditor/Input/PropertyGridCommands.cs`): `SaveEffectDefault` and
`ClearEffectDefault`.

`EffectPropertyEditorGrid` gains a `HasStoredDefault` notify-property driving the reset button's
enabled state, refreshed whenever `SelectedObjects` changes.

Note that `IEffect` does **not** extend `IModuleInstance`
(`src/Vixen.Core/Module/Effect/IEffect.cs:11`); only `IEffectModuleInstance` does. Reaching
`TypeId`, `Descriptor` or `ModuleData` from `SelectedObject` requires an `as IEffectModuleInstance`
cast, and the command must no-op if that cast yields null.

## Mathematical / Boundary Logic

### Capture (`SaveDefault`)

    dataType   := effect.Descriptor.ModuleDataClass
    serializer := DataContractSerializer(dataType)           // cached per type
    liveBytes  := WriteBinary(serializer, effect.ModuleData) // getters resolve library points here
    copy       := ReadBinary(serializer, liveBytes)          // independent graph; live effect untouched
    Scrub(copy)
    entry.Payload := WriteBinary(serializer, copy)
    upsert entry by TypeId; WriteStoreAtomically()

### Scrub

Depth-first, cycle-guarded, recursing into collection elements:

    Scrub(node, visited, depth):
      if node is null or depth > 32: return
      if node is reference type and not visited.Add(node): return   // ReferenceEqualityComparer
      if node is ILibraryLinkable link: link.UnlinkFromLibrary()
      if node is IEnumerable and not string:
          foreach item in node: Scrub(item, visited, depth+1)
          return
      foreach member in DataMembers(node.GetType()):     // props + fields, public and non-public
          if member has [ExcludeFromEffectDefault]:
              SetValue(node, member, default(member.Type)); continue
          value := GetValue(node, member)
          if value is null or IsLeaf(member.Type): continue
          Scrub(value, visited, depth+1)

`DataMembers` must include **non-public** members. `ColorGradient` keeps `_colors`, `_alphas` and
`_libraryReferenceName` as private `[DataMember]` fields (`ColorGradient.cs:257-259` and
`ColorGradient.cs:1314`), `Curve` keeps `_libraryReferenceName` the same way (`Curve.cs:100`), and
`XYZ` keeps `_x`, `_y`, `_z` that way (`src/Vixen.Core/Common/ColorSpaces.cs:18`). A public-only
walk would silently miss every gradient link.

`IsLeaf` returns true for primitives, `string`, `decimal`, `DateTime`, `TimeSpan`, `Guid`, enums,
and `System.Drawing.Color`.

Add value types to `visited` — boxing makes every visit a distinct reference, so guarding on them is
useless and harmless to skip.

### Apply (`CreateDefaultData`)

    if not _payloads.TryGetValue(typeId, out payload): return null    // dictionary miss, no I/O
    dataType := Modules.GetDescriptorById(typeId)?.ModuleDataClass
    if dataType is null or dataType.FullName != entry.DataModelTypeName: log; return null
    try    return ReadBinary(CachedSerializer(dataType), payload)
    catch  log warning; return null                                   // caller falls back to built-in

Every failure path returns `null`, and every caller treats `null` as "use the built-in constructor".
A corrupt or stale entry degrades to today's behaviour; it never throws into an effect-add path.

### Boundary cases

**Effect module not installed.** `GetDescriptorById` returns null. Skip the entry at load, but
**retain it in the in-memory store and rewrite it on save**, so uninstalling and reinstalling a
module does not silently destroy the user's default.

**Data model shape changed incompatibly.** Deserialize throws, caught, warning logged, built-in
defaults used. The stored entry is left intact so the user can decide to re-save.

**Store file missing or corrupt.** Treat as empty, log, and continue. Write with
write-to-temp-then-replace so a crash mid-write cannot destroy the existing file.

**Profile switch.** `Paths.DataRootPath` is assigned during `VixenSystem.Start`
(`src/Vixen.Core/Sys/VixenSystem.cs:77`). Load lazily on first access and record the root path the
cache was built from; if it differs on a later access, reload. `Reload()` is also exposed for
explicit invalidation after an import.

## Subsystem Component Matrix

### New — `Vixen.Core`

- `Data/Library/ILibraryLinkable.cs`
- `Sys/Attribute/ExcludeFromEffectDefaultAttribute.cs`
- `Services/EffectDefaults/EffectDefaultsService.cs`
- `Services/EffectDefaults/EffectDefaultsStore.cs`
- `Services/EffectDefaults/EffectDefaultScrubber.cs`
- `Services/EffectDefaults/EffectDefaultSummary.cs`

### Modified — App modules

- `src/Vixen.Modules/App/Curves/Curve.cs` — implement `ILibraryLinkable`
- `src/Vixen.Modules/App/ColorGradients/ColorGradient.cs` — implement `ILibraryLinkable`

### Modified — effect data models

The ten `MarkCollectionId` members listed above gain `[ExcludeFromEffectDefault]`.

### Modified — `EffectEditor`

- `Input/PropertyGridCommands.cs` — two new commands
- `PropertyGrid.Commands.cs` — bindings and `CanExecute` in `InitializeCommandBindings` (line 31)
- `EffectPropertyEditorGrid.cs` — `HasStoredDefault`, refresh on selection change
- `Design/CategorizedLayout.xaml` — two buttons in the header `Grid` (line 17), left of the existing
  help hyperlink (line 27); add two `Auto` columns
- `Design/AlphabeticalLayout.xaml` — parity; its header is currently a bare `Border` with no `Grid`,
  so it needs the same grid structure

Icons `disk.png` and `arrow_refresh.png` already exist in the `Resources` project
(`src/Vixen.Common/Resources/`) and are referenced with the same `/Resources;component/…` pack
syntax as the current `help.png`.

Confirm before use whether `AlphabeticalLayout` is ever selected at runtime —
`EffectPropertyEditorGrid` hard-assigns `Layout = new CategorizedLayout()` (line 1126) and
`FormEffectEditor` never overrides it, so it may be dead. Mirror it anyway rather than let the two
templates diverge.

### Modified — `TimedSequenceEditor`

One new private helper on `TimedSequenceEditorForm`:

    /// <remarks>Do not use where the caller immediately overwrites ModuleData
    /// (clone, paste) — the default would be materialised and thrown away.</remarks>
    private IEffectModuleInstance CreateEffectInstanceWithDefaults(Guid effectTypeId)

Because the narrow injection seam was chosen, these six sites must each be switched to the helper,
and **the list is the specification** — there is no central fallback if one is missed:

| Line in `TimedSequenceEditorForm.cs` | Path |
|---|---|
| 2272 | Draw effect on timeline |
| 2571 | Add Multiple Effects dialog |
| 2610 | Add effects at marks |
| 3779 | `AddNewEffectById` (toolbox drop, hotkeys) |
| 4229 | Replace selected elements with a different effect type |
| 4942 | Drag media file onto timeline |

Three sites must **keep** the plain `ApplicationServices.Get<IEffectModuleInstance>` call, because
each overwrites `ModuleData` on the very next statement:

| Location | Reason |
|---|---|
| `TimedSequenceEditorForm.cs:3579` — `CloneElements` | assigns `ModuleData.Clone()` immediately |
| `TimedSequenceEditorForm.cs:5321` — paste | assigns clipboard `ModuleData` immediately |
| `TimedSequenceEditorForm.cs:6091` and `TimedSequenceEditorForm_Menu.cs:533` — LipSync | sets its own data |

Site 4942 sets specific properties after creation (`PictureSource`, filename); applying defaults
first is correct there, as the explicit assignments win.

Also modified: `TimedSequenceEditorForm_Menu.cs` for export, import and the diagnostic-dump
commands, plus a small selection form for choosing which effects to export or import.

### New — `Vixen.Tests`

- Scrubber tests: library unlinking; `[ExcludeFromEffectDefault]` blanking at top level *and* nested
  in collections; cycle safety
- Store round-trip tests
- A regression test asserting that capture does not mutate the source effect — the `PulseData`
  shared-`Curve` hazard specifically

## Concurrency, Performance & Thread Safety

Effects are created on the UI thread in the editor but also on background threads during sequence
load and by the web server, so the payload cache is a `ConcurrentDictionary<Guid, byte[]>` and the
per-type `DataContractSerializer` cache is likewise concurrent. Serializer construction is the
expensive part (milliseconds); caching one instance per data-model type reduces steady-state
deserialization to tens of microseconds. `DataContractSerializer` instances are documented as safe
for concurrent `ReadObject`/`WriteObject` calls; verify this during implementation and fall back to
a per-type lock if any instability appears under the bulk-add path.

Retrieval cost is dominated by the common case of an effect with **no** saved default: one
`ConcurrentDictionary` miss, zero I/O, zero allocation, then the existing constructor runs exactly
as today. For an effect that *does* have a default, the cost is one payload deserialization
*replacing* the constructor call that would otherwise have run — the same order of work, not
additional work. Loading the store is a single small file read plus one shallow deserialize into
`Guid → byte[]`; payloads stay opaque until first use. The scrub walk is reflection-heavy but runs
only on an explicit user save, where its cost is irrelevant.

The one contention point worth flagging: bulk operations such as Add Multiple Effects or drawing
across many rows will deserialize the payload once per created effect. At roughly 20–100 µs each, a
thousand effects costs well under a tenth of a second. Caching a single deserialized prototype and
cloning it would be faster but is **unsafe** for exactly the reason established above — `Clone()` is
shallow for `Curve` in several data models, so every effect created from one prototype would share a
mutable `Curve`.

## Suggested milestone breakdown

Per `.agents/PLANS.md`, these milestones need to update the description on JIRA issue VIX-3964, before code
work begins.

1. **Core storage.** `ILibraryLinkable`, the exclusion attribute and its ten applications, store
   types, service, scrubber, plus tests. No user-visible change; proven by tests.
2. **Editor integration.** Effect editor buttons and the six TSE call sites — first user-visible
   behaviour.
3. **Transfer and diagnostics.** Export, import, and the on-demand readable diagnostic dump.
4. **Documentation** under `docs/effects/`.

## Open items resolved during design

`PhonemeMapping` on `LipSyncData` is a library *name* stored as a string, and it is **not** marked
excluded — it is captured in the default like any other setting. Phoneme mappings are a fixed set
and the library is only ever added to, never pruned, so a captured name cannot become dangling.
Import therefore needs no special handling for it, and no warning is required.

Reset shows a Yes/No confirmation via `MessageBoxService.GetUserConfirmation`
(`src/Vixen.Common/WPFCommon/Services/MessageBoxService.cs:44`). Save shows no dialog, with the reset
button becoming enabled as implicit confirmation.

Any public or protected member introduced here must carry XML documentation per `CLAUDE.md`, using
the project's `csharp-docs` skill.

## Hand-off context

Compressed data dump for the `generate-spec` skill.

    ISSUE: VIX-3964 "Add option to set effect default settings" (Improvement, New Ticket, Normal)
    TARGET SPEC PATH: docs/plans/effects/vix-3964-effect-default-settings.md
    CAPTURE SCOPE: IEffectModuleInstance.ModuleData only. Not TargetNodes/TimeSpan/StartTime/Media/MarkCollections.
    STORAGE: profile-scoped, single file {Paths.DataRootPath}\Effect Defaults\EffectDefaults.vfd
             declared as [DataPath] static readonly in a Vixen.Core type (Paths._BuildDataDirectories
             reflects only over Vixen.Core's assembly). Explicitly NOT VixenSystem.ModuleStore.
             Inside profile folder => included in DataZipForm support archives automatically.
    FORMAT: binary DataContractSerializer via XmlDictionaryWriter.CreateBinaryWriter,
            precedent EffectModelCandidate.cs:52. Store = [DataContract] EffectDefaultsStore
            { int Version=1; List<EffectDefaultEntry> }. Entry { Guid TypeId; string EffectName;
            string DataModelTypeName; DateTime SavedUtc; byte[] Payload }.
            Payload type resolved from Modules.GetDescriptorById(typeId).ModuleDataClass,
            NEVER from a persisted AssemblyQualifiedName (version-pinning breaks upgrades).
            DataModelTypeName is diagnostics + mismatch detection only.
    NEW TYPES: Vixen.Data.Library.ILibraryLinkable { void UnlinkFromLibrary(); }
               Vixen.Sys.Attribute.ExcludeFromEffectDefaultAttribute (Property|Field)
               Vixen.Services.EffectDefaults.{EffectDefaultsService, EffectDefaultsStore,
                                              EffectDefaultScrubber, EffectDefaultSummary}
    ILibraryLinkable IMPLEMENTORS: ColorGradient (UnlinkFromLibrary exists, ColorGradient.cs:1379),
                                   Curve (explicit impl delegating to UnlinkFromLibraryCurve, Curve.cs:204).
                                   These are the ONLY library-linkable types reachable from effect data;
                                   LipSyncData holds its map as a string name, not LipSyncMapData.
    [ExcludeFromEffectDefault] ON 10 MEMBERS:
      AlternatingData.cs:35  DissolveData.cs:31  FireworksData.cs:112  LipSyncData.cs:59
      Liquid/EmitterData.cs:209 (NESTED in List<EmitterData>)  ShapesData.cs:193
      StateData.cs:60  StrobeData.cs:37  TextData.cs:121
      Wave/WaveformData.cs:114 (NESTED in List<WaveformData>)
      DO NOT annotate Liquid/Emitter.cs:863 or Wave/Waveform.cs:389 (runtime wrappers, not data models).
    CAPTURE PIPELINE (order is load-bearing):
      serialize live -> deserialize copy -> scrub copy -> serialize copy -> store.
      MUST NOT use Clone(): PulseData.CreateInstanceForClone:23 assigns result.LevelCurve = LevelCurve
      (shared ref). Cloning then unlinking would mutate the live effect being edited.
      Serializing live FIRST is what resolves library points: Curve.Points getter (Curve.cs:73) and
      ColorGradient accessors (ColorGradient.cs:807-841) call CheckLibraryReference() and materialise
      library data into own [DataMember] fields => unlinking afterwards is lossless.
    SCRUB WALK: depth-first, ReferenceEqualityComparer visited set, depth cap 32, recurses into
      IEnumerable elements (required: 2 of 10 excluded members are nested in collections).
      MUST enumerate NON-PUBLIC [DataMember]s: ColorGradient._colors/_alphas/_libraryReferenceName
      (ColorGradient.cs:257-259,1314), Curve._libraryReferenceName (Curve.cs:100), XYZ._x/_y/_z.
      Public-only walk silently misses every gradient link.
      Leaf types (no recursion): primitives, string, decimal, DateTime, TimeSpan, Guid, enum,
      System.Drawing.Color.
    APPLY SEAM (NARROW - no central fallback, list IS the spec):
      New helper TimedSequenceEditorForm.CreateEffectInstanceWithDefaults(Guid).
      SWITCH these 6: TimedSequenceEditorForm.cs 2272, 2571, 2610, 3779, 4229, 4942
      LEAVE these 3 on plain ApplicationServices.Get (they overwrite ModuleData next statement):
        3579 CloneElements, 5321 paste, 6091 + TimedSequenceEditorForm_Menu.cs:533 LipSync
      Modules.GetById is NOT modified.
    FAILURE MODE: CreateDefaultData returns null on every error path (missing descriptor, type
      mismatch, deserialize throw); caller falls back to the built-in constructor. Never throws
      into an effect-add path. Unknown-module entries are retained in memory and rewritten on save
      so module uninstall/reinstall does not destroy defaults. Atomic write: temp then replace.
    UI: RoutedUICommands SaveEffectDefault + ClearEffectDefault on PropertyGridCommands;
        bound in PropertyGrid.Commands.cs InitializeCommandBindings (line 31).
        Buttons in CategorizedLayout.xaml header Grid (line 17), left of help hyperlink (line 27);
        mirror into AlphabeticalLayout.xaml (currently a bare Border, needs the Grid added).
        Icons: /Resources;component/disk.png and arrow_refresh.png (both already exist).
        CanExecute Save: exactly ONE selected effect. CanExecute Clear: one selected AND
        HasStoredDefault. NOTE IEffect does NOT extend IModuleInstance (IEffect.cs:11) - must cast
        `SelectedObject as IEffectModuleInstance` to reach TypeId/Descriptor/ModuleData; no-op if null.
        Reset = delete stored entry only, does NOT alter the open effect => no undo action needed.
        Reset confirms via MessageBoxService.GetUserConfirmation; Save shows no dialog.
    EXPORT/IMPORT: same .vfd format, user-selected subset, merge on import, Reload() after.
    DIAGNOSTICS: on-demand menu command only. Same DataContractSerializer + same object graph,
      written through XmlWriter.Create(..., Indent=true) => readable copy cannot drift from stored bytes.
    PERF: ConcurrentDictionary<Guid,byte[]> payload cache + ConcurrentDictionary<Type,
      DataContractSerializer>. No-default case = one dictionary miss, zero I/O. Has-default case
      REPLACES the constructor call, so it is not additive cost. Do NOT cache a deserialized
      prototype and Clone() it - shallow Curve clone would share a mutable Curve across all effects.
    PROFILE SWITCH: Paths.DataRootPath assigned in VixenSystem.Start (VixenSystem.cs:77). Lazy-load,
      record source root path, reload if it changes.
    TESTS (Vixen.Tests): scrubber (unlink; blanking top-level AND nested-in-collection; cycles);
      store round-trip; regression proving capture does not mutate the source effect (PulseData
      shared-Curve hazard).
    MILESTONES (each needs a JIRA issue created BEFORE code work, per .agents/PLANS.md):
      M1 core storage+service+scrubber+attribute+tests (no UI)
      M2 editor buttons + 6 TSE call sites
      M3 export/import + diagnostic dump
      M4 docs under docs/effects/
    CONSTRAINTS: tabs not spaces; XML docs required on all new public/protected members
      (csharp-docs skill); C# 12+, nullable enabled.
    SETTLED: LipSyncData.PhonemeMapping is a library NAME kept as a string and is NOT excluded -
      it is captured like any other setting. Phoneme mappings are a fixed set and the library is
      only ever added to, so a captured name cannot dangle. No import warning needed.
    NO OPEN QUESTIONS REMAIN.

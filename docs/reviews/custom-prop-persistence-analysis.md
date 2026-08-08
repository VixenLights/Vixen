# Architecture Design: Custom Prop Persistence and LiteDB Migration

## Core Strategy

Replace LiteDB as the write format for Custom Prop Editor `.prp` files with a versioned package format. Keep `.prp` as a single file, but make the file a ZIP archive with two entries:

- `prop.json`, containing a normalized persistence document.
- `background.jpg`, containing the background image as binary data.

The JSON document must store each `ElementModel` once and represent relationships with identifiers. It must not serialize the live `Prop`/`ElementModel` object graph directly. This follows the Provider and Strategy patterns: a persistence facade detects the file format, a JSON package provider reads and writes the new format, and a read-only legacy provider imports old LiteDB data.

This is preferable to a normalized LiteDB 5 database. LiteDB can avoid the large-document limit if nodes and lights are split across collections, but a `.prp` file contains one aggregate that is always loaded and saved as a unit. A database adds format opacity and dependency risk without providing useful query or concurrency benefits. A versioned JSON package is easier to validate, migrate, inspect, test, and recover.

Do not embed the JPEG as base64 inside JSON. Base64 increases the image size by about one third and requires another large string/byte-array allocation. Keeping the image as a ZIP entry still embeds it in the single `.prp` file while allowing streaming and native image decoding.

Do not rely exclusively on LiteDB 5 `Upgrade=true` for backward compatibility. Repository commit `430310a27` documents that the earlier VIX-3530 upgrade was reverted because large props exceeded LiteDB 5's approximately 16 MB single-document limit. LiteDB 5 upgrades a v4 file by rebuilding and reinserting each legacy document, so the same limit applies during upgrade. Stock LiteDB 5 therefore cannot guarantee that every existing large v4 prop can be opened.

## Data Model and Property Contracts

Add persistence-only DTOs under `src/Vixen.Modules/App/CustomPropEditor/Persistence/Documents`. These contracts must not derive from `BindableBase`, use WPF types, or reference LiteDB. Use `System.Text.Json` source generation so the accepted types are explicit and serialization does not emit runtime type metadata.

The root document should have this logical shape:

```json
{
  "format": "vixen.custom-prop",
  "schemaVersion": 1,
  "prop": {
    "id": "guid",
    "name": "Prop name",
    "type": "...",
    "createdBy": "...",
    "creationDate": "ISO-8601",
    "modifiedDate": "ISO-8601",
    "opacity": 1.0,
    "width": 800.0,
    "height": 600.0,
    "vendorMetadata": {},
    "physicalMetadata": {},
    "informationMetadata": {}
  },
  "rootElementId": "guid",
  "elements": [
    {
      "id": "guid",
      "name": "Model",
      "order": -1,
      "lightSize": 3,
      "modelType": "Model",
      "statePropertyId": "guid",
      "faceDefinition": {},
      "stateDefinitionModels": [],
      "childIds": ["guid"],
      "lights": []
    }
  ],
  "image": {
    "entry": "background.jpg",
    "mediaType": "image/jpeg"
  }
}
```

The exact new-format DTOs should include every currently meaningful persisted property from `Prop`, `ElementModel`, `Light`, `FaceDefinition`, `StateDefinitionModel`, `StateItemModel`, `VendorMetadata`, `PhysicalMetadata`, and `InformationMetadata`. The legacy `StateDefinition` type is deliberately excluded from the new-format DTOs.

`ElementModel.StateDefinition` and `ElementModel.StateDefinitions` are legacy input fields, not schema v1 persistence fields. Current xModel import writes authoritative State data to `ElementModel.StateDefinitionModels`; its optional legacy State groups are structural compatibility groups and do not receive element-level legacy State values. The remaining production reads of the legacy properties are in `CustomPropStateMigrationService` and the Preview legacy fallback. The hidden `ElementModelViewModel` accessors still reference `StateDefinition`, but they are compatibility-era accessors and must not make that property part of the new file contract.

Accordingly:

- `prop.json` schema v1 must not define or write `legacyStateDefinition` or `legacyStateDefinitions`, even when the corresponding live-model properties are non-empty.
- Empty legacy values must therefore be absent, not serialized as `null` or `[]`.
- New prop creation, new xModel import, State Definition editing, and package hydration must use only `StateDefinitionModels`.
- The LiteDB legacy reader may read `StateDefinition` and `StateDefinitions` into a migration-only representation. It must run `CustomPropStateMigrationService` before the model becomes eligible for a new-format save.
- After successful migration, the in-memory legacy properties should be cleared so they cannot be mistaken for authoritative data by later code.
- If non-empty legacy data cannot be converted without loss, saving in the new format must fail with a clear migration error rather than silently discarding it or copying it into schema v1.

Persist these relationships only once:

- `PropDocument.RootElementId` identifies the root.
- `ElementDocument.ChildIds` preserves child order and supports the existing directed acyclic graph, where one element can appear beneath multiple parents.
- `ElementModel.Parents` is derived from `ChildIds` during hydration and is not persisted.
- A light is owned by one `ElementDocument`; `Light.ParentModelId` is derived from that owner and is not persisted.
- Computed properties such as `ElementType`, `LightCount`, `IsLeaf`, `IsLightNode`, and `IsGroupNode` are not persisted.
- `ElementModel.StateDefinition` and `ElementModel.StateDefinitions` are accepted only by the legacy importer, migrated into `StateDefinitionModels`, and never written to the new package.

Use explicit wire representations for `System.Drawing.Color` and enums. A fixed ARGB integer or `#AARRGGBB` color string is suitable. Do not let CLR type or enum renames silently change the file contract; schema migrations must handle intentional wire-name changes.

The domain mapper must be a separate component. A suggested contract is:

```csharp
internal interface IPropDocumentMapper
{
	PropDocument ToDocument(Prop prop);
	Prop ToModel(PropDocument document, BitmapSource image);
}
```

Hydration is a two-pass operation. The first pass creates all elements and lights keyed by ID. The second pass links `ChildIds`, rebuilds `Parents`, assigns the root, and normalizes `StateDefinitionModels`. New-format hydration must leave `StateDefinition` null and `StateDefinitions` empty. Add internal hydration factories or methods to the domain model instead of making setters public solely for serialization.

## Mathematical and Boundary Logic

Flatten the graph without duplicating shared elements:

```text
queue := [root]
seen := empty set of element IDs
documents := empty list

while queue is not empty:
    element := dequeue(queue)
    if element.ID is in seen:
        continue
    add element.ID to seen
    append a document containing element scalars, ordered child IDs, and lights
    enqueue each child
```

Validate before constructing the live model:

```text
require format == "vixen.custom-prop"
require schemaVersion is supported
require exactly one prop.json and at most one declared image entry
require all element IDs and light IDs are non-empty and unique
require rootElementId refers to an element
require every child ID and State item assignment ID refers to an element
derive parent IDs from child IDs
reject a directed cycle using iterative depth-first search with visiting/visited sets
require every element is reachable from root unless a future schema explicitly permits orphans
require finite width, height, opacity, coordinates, Z values, and sizes
require declared and actual archive sizes are within documented safety limits
```

The new format has no 16 MB logical document boundary. Safety limits should protect against corrupt or hostile files rather than reproduce LiteDB's normal-model ceiling. Establish limits from real large props and leave headroom. Read archive entries directly; never extract paths to disk. Reject duplicate entry names, unexpected path traversal names, excessive entry counts, excessive uncompressed sizes, and implausible compression ratios.

File format detection must inspect bytes rather than trust `.prp`:

```text
if ZIP signature and required manifest entry exist:
    use package reader
else if LiteDB v4/v5 header is recognized:
    use legacy reader
else:
    report unsupported or corrupt prop file
```

## Legacy Migration

Legacy loading and new-format saving must be separate operations. Opening an old prop must not mutate the source file. In memory, the loader migrates any meaningful `StateDefinition`/`StateDefinitions` content into `StateDefinitionModels` and clears the legacy fields after validating the conversion. The first successful Save writes a new package to a temporary file in the same directory, validates that temporary package, and atomically replaces the original while preserving a one-time legacy backup. If migration, writing, or validation fails, the original LiteDB file remains intact.

The compatibility path must cover v4 documents larger than 16 MB. There are two viable implementation stages:

1. For the transition release, isolate LiteDB 4 behind a read-only legacy adapter. Open `props` as an untyped `BsonDocument`, recursively reject the `_type` field implicated in CVE-2022-23535, and map explicitly to persistence DTOs. Do not call `GetCollection<Prop>`, `ToObject<Prop>`, insert, update, delete, shrink, or otherwise invoke typed or write behavior. This mitigates the known exploit path and preserves large-file support, but the vulnerable NuGet identity will still be reported by package auditing. Treat this only as a time-bounded transition.
2. For the clean end state, remove the LiteDB 4 NuGet package. Build a read-only v4 datafile importer from LiteDB 5.0.21's MIT-licensed `FileReaderV7`/legacy BSON reader, with attribution, and expose only the operations needed to obtain the `props` document and reconstruct `$/image/background.jpg` or `background.png`. The v5 code already reads v4 documents before the failing reinsertion step; the Vixen adapter must stop at the raw-document stage and map fields without upgrading the database. A short proof-of-concept should verify a generated v4 prop above 16 MB before this approach is promoted.

If Vixen releases containing LiteDB 5 `.prp` files were publicly distributed, add a separate read-only v5 provider using patched LiteDB 5.0.21. Do not load LiteDB 4 and 5 into the default .NET load context together because both assemblies have the same identity. Keep any transitional converters behind a DTO/stream boundary or use the single adapted v5 legacy reader approach.

The migration behavior should be:

- New prop: save package schema 1.
- Existing package schema 1: read and save schema 1.
- Supported older package schema: migrate in memory, save the current schema on the next user save.
- Legacy LiteDB prop: import read-only, migrate legacy State fields into `StateDefinitionModels`, clear the in-memory legacy fields, mark its source format as legacy internally, and save only the current package fields on the next user save.
- Unknown newer package schema: refuse to overwrite and provide a clear compatibility error.

## Subsystem Component Matrix

| Component | Recommended responsibility | Change |
|---|---|---|
| `Services/PropModelPersistenceService.cs` | Format-neutral facade | Replace direct/static LiteDB CRUD with injected package/legacy providers and atomic save orchestration. |
| `Persistence/IPropFileReader.cs` | Reader strategy | Detect/support one file format and return a persistence document plus image stream. |
| `Persistence/PropPackageReader.cs` | New format reader | Validate ZIP entries and schema, deserialize JSON, decode image. |
| `Persistence/PropPackageWriter.cs` | New format writer | Stream JSON and JPEG entries to a temporary ZIP and validate before replacement. |
| `Persistence/LegacyLiteDbPropReader.cs` | Compatibility import | Read old v4 data without writing or using unsafe polymorphic mapping. |
| `Persistence/PropDocumentMapper.cs` | Domain boundary | Flatten and hydrate `Prop`/`ElementModel` without serializer-specific annotations. |
| `Persistence/PropDocumentValidator.cs` | Trust boundary | Enforce IDs, graph validity, numeric validity, archive limits, and schema support. |
| `Model/Prop.cs`, `Model/ElementModel.cs`, `Model/Light.cs` | Live editor domain | Add internal hydration support; remove LiteDB attributes after the legacy typed path is gone. |
| `ViewModels/PropEditorViewModel.cs` | UI orchestration | Use an injected async persistence service and `TaskCommand`; display load/save/migration errors through UI services. |
| `Preview/VixenPreview/VixenPreviewControl.cs` | Preview import | Await the same format-neutral loader; do not know whether the source was JSON or LiteDB. |
| `Directory.Packages.props` and `CustomPropEditor.csproj` | Dependency cleanup | Remove LiteDB 4 after the raw legacy reader is available; use BCL `System.Text.Json` and `System.IO.Compression`. |

Recommended service contract:

```csharp
public interface IPropModelPersistenceService
{
	Task<Prop> LoadAsync(string path, CancellationToken cancellationToken = default);
	Task SaveAsync(Prop prop, string path, CancellationToken cancellationToken = default);
}
```

Keep synchronous WPF bitmap encoding/decoding behind an image codec abstraction if dispatcher affinity is observed. File and JSON operations should use async streams. Remove the current `Task.Factory.StartNew` wrapper; it is not true asynchronous I/O and provides no cancellation.

## Concurrency, Performance, and Thread Safety

Persistence providers should be stateless and safe for independent files. Serialize writes to the same canonical path to prevent two temporary files from racing. Use a unique temporary name in the destination directory and close all streams before `File.Replace`/`File.Move`.

JSON serialization must operate on the normalized records, so graph depth does not affect serializer depth and shared elements are not duplicated. ZIP compression should materially reduce repetitive node JSON. The JPEG should normally use `CompressionLevel.NoCompression` because it is already compressed; `prop.json` should use an appropriate compression level.

Start with source-generated `System.Text.Json` serialization to an archive stream and measure peak memory using a prop larger than the old 16 MB limit. If DTO duplication creates unacceptable peak memory, keep the same wire format and replace only the writer/reader implementation with `Utf8JsonWriter`/`Utf8JsonReader` streaming. Do not change the schema merely to optimize allocations.

`BitmapSource` loaded from the archive should use `BitmapCacheOption.OnLoad`, detach from the archive stream, and be frozen when possible so it can safely cross threads.

## Validation and Acceptance

Add focused tests under `src/Vixen.Tests/App/CustomPropEditor/Persistence`:

- Round-trip every current persisted scalar, metadata field, face definition, `StateDefinitionModel`, light coordinate, image, canvas dimension, and ID.
- Save a new prop and prove the JSON contains neither `legacyStateDefinition` nor `legacyStateDefinitions`; empty values must not appear as `null` or `[]`.
- Load a legacy prop with meaningful `StateDefinition` and `StateDefinitions` data, migrate and save it, and prove the data exists only under `stateDefinitionModels` in the new JSON.
- Prove new prop creation, xModel import, package hydration, and State Definition editing leave `StateDefinition` null and `StateDefinitions` empty.
- Prove an unconvertible non-empty legacy State shape blocks new-format save without modifying the original file.
- Round-trip a shared element with multiple parents and prove it is one object instance after hydration.
- Round-trip a tree deeper than 20 levels and a generated prop whose JSON exceeds 16 MB.
- Load committed representative LiteDB v4 fixtures using both `background.jpg` and `background.png`.
- Generate/load a v4 `props` document larger than 16 MB and prove migration succeeds without `Upgrade=true`.
- Insert malicious `_type` metadata into a legacy fixture and prove it is rejected before typed materialization.
- Reject duplicate IDs, missing references, cycles, non-finite numeric values, duplicate ZIP entries, oversized entries, path traversal names, unsupported newer schemas, and corrupt images.
- Interrupt/fail a save after temporary-file creation and prove the original remains readable.
- Load a legacy file, save it, reopen it as the package format, and prove the one-time backup remains a valid legacy file.
- Exercise Preview import through the same persistence facade.

Run:

```text
dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Persistence"
dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor|FullyQualifiedName~Preview.VixenPreview"
dotnet list src/Vixen.Modules/App/CustomPropEditor/CustomPropEditor.csproj package --vulnerable --include-transitive
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
git diff --check
```

Final acceptance requires the package audit to no longer report LiteDB 4.1.4, new `.prp` files to contain `prop.json` and `background.jpg`, and both ordinary and over-16-MB legacy props to load and migrate without altering the source until Save.

## Risks and Decisions

- Backward compatibility is one-way: current Vixen reads old and new props; older Vixen versions need not read newly saved package props. This matches the existing Custom Prop State requirements.
- Directly serializing the domain graph is rejected because it duplicates shared nodes, couples the file to WPF/bindable implementation details, persists obsolete legacy fields, and reintroduces graph depth/reference problems.
- LiteDB 5 normalized collections are technically possible but not recommended because the prop is a single aggregate and gains little from a database.
- LiteDB 5 in-place upgrade is rejected because it mutates the source before Vixen proves it can materialize and resave the prop, and it cannot guarantee migration of documents over its single-document limit.
- A transitional read-only LiteDB 4 adapter reduces exploitability but does not make dependency scanning green. The read-only legacy parser/removal milestone is required for full remediation.
- Keep the `.prp` extension to avoid breaking dialogs, inventory links, and user expectations. Detect format by content.

## Sources

- Repository package declaration: `Directory.Packages.props` currently resolves LiteDB 4.1.4.
- Repository history: VIX-3530 commit `2a946ab87` upgraded to LiteDB 5.0.20; VIX-3561 commit `430310a27` reverted it due to the 16 MB document limit.
- LiteDB advisory GHSA-3x49-g6rc-c284 identifies versions below 5.0.13 as affected by unsafe `_type` deserialization.
- LiteDB 5.0.21 `Constants.MAX_DOCUMENT_SIZE` documents the approximately 16 MB limit, while `RebuildService` uses the v4 `FileReaderV7` and reinserts documents into a new engine.
- LiteDB's connection-string documentation states that `Upgrade=true` checks for and upgrades older datafiles before opening.

## TERRA HAND-OFF CONTEXT

Replace Custom Prop Editor LiteDB writes with a single-file `.prp` ZIP package containing `prop.json` plus binary `background.jpg`. Use versioned persistence-only DTOs and source-generated System.Text.Json. Flatten the ElementModel DAG: store every element once by ID, preserve ordered child IDs, derive Parents and Light.ParentModelId, and omit computed properties. Schema v1 must not define or emit legacyStateDefinition/legacyStateDefinitions, including null or empty values. New creation, xModel import, State editing, and package hydration use only StateDefinitionModels. The legacy reader may load old StateDefinition/StateDefinitions into a migration-only representation; it must migrate them to StateDefinitionModels, validate equivalence, clear the in-memory legacy values, and block save on lossy conversion. Add a mapper, validator, new-format reader/writer, format-neutral async facade, atomic same-directory temp save, and one-time legacy backup. Never direct-serialize Prop/ElementModel and never base64 the image. Stock LiteDB5 Upgrade=true is insufficient: repo commit 430310a27 confirms large props hit v5's ~16MB document limit, and v5 rebuild reinserts each v4 document. For transition, a read-only LiteDB4 raw-Bson adapter may reject `_type` recursively and explicitly map fields, but full remediation requires removing LiteDB4 and adapting LiteDB5.0.21's MIT FileReaderV7/raw BSON path so it exports v4 props/images without reinserting/upgrading. Opening legacy files must not mutate the source; next Save atomically writes the package. Keep `.prp`, sniff content. Validate schema, archive bounds, unique IDs, root/references, reachability, cycles, finite numbers, images, and lossless legacy State migration. Test absence of legacy JSON fields, current State round trip, migrated legacy State, new workflows leaving legacy fields empty, >16MB v4 migration, malicious `_type`, corrupt/hostile archives, atomic failure recovery, legacy backup, and Preview import. Final audit must not report LiteDB4.

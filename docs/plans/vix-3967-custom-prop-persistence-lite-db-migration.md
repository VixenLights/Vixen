# Replace Custom Prop LiteDB Persistence with a Safe Versioned Package

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document in accordance with `.agents/PLANS.md` from the repository root. It is self-contained: an implementer must be able to deliver this work without the original architecture handoff or its analysis document.

## Purpose / Big Picture

Custom Prop Editor users currently save `.prp` files as LiteDB databases. The database write path serializes the live WPF domain object graph, depends on the vulnerable LiteDB 4.1.4 package, and cannot safely be migrated through stock LiteDB 5 when a legacy `props` document exceeds its approximately 16 MiB document limit. After this work, a newly saved `.prp` is a ZIP package containing a versioned `prop.json` manifest and a binary `background.jpg`. Existing LiteDB props, including large v4 props, open without changing their source; their first successful save atomically replaces the `.prp` with the package and preserves one valid legacy backup.

A user can verify the outcome by creating and saving a prop, opening the resulting `.prp` with a ZIP viewer to see `prop.json` and `background.jpg`, reopening it in Custom Prop Editor and Preview, and opening then saving a representative LiteDB v4 prop. The package must preserve the prop, image, shared element relationships, State definitions, and canvas dimensions while removing LiteDB 4 from the package audit.

## Progress

- [x] (2026-08-07 00:00Z) Read `.agents/PLANS.md`, the Custom Prop persistence analysis, existing persistence, state-migration, editor, Preview, project, and test code; created this ExecPlan only.
- [x] (2026-08-07 00:00Z) Created Jira Improvement VIX-3967, set its Custom Prop Editor delivery contract, acceptance criteria, and test plan, and renamed this plan to begin with the issue key.
- [x] (2026-08-07 00:00Z) Implemented the internal schema-1 DTO boundary, source-generated JSON metadata, iterative graph mapper, pre-hydration validator, and mapper/validator tests for Milestone 3. The Custom Prop Editor module builds successfully; the aggregate test-project build remains blocked by missing x86 apphost packs in native dependencies.
- [x] (2026-08-07 00:00Z) Implemented the schema-1 ZIP reader/writer, detached WPF JPEG codec, per-path atomic publisher, temporary-package revalidation, and package I/O tests for Milestone 4. The Custom Prop Editor module builds successfully; aggregate test compilation remains blocked by unavailable native dependency outputs.
- [x] (2026-08-07 00:00Z) Implemented the async, format-neutral persistence facade; registered it for the editor and Preview; promoted the v4 raw reader to a read-only legacy loader; and routed editor/Preview load and save paths through the shared service for Milestone 5.
- [x] (2026-08-07 00:00Z) Removed LiteDB 4 from the Custom Prop Editor production project and central package versions for Milestone 6. The Custom Prop Editor vulnerable-package audit reports no vulnerable packages; the adapted raw v4 reader remains the only production legacy parser.
- [x] (2026-08-08 00:00Z) Removed the test-only LiteDB 4 fixture builder and its four raw-reader proof tests. Retained the package, document-mapper, and image-dimension persistence tests, which exercise the supported persisted format without a LiteDB package dependency.
- [x] (2026-08-07 00:00Z) Completed schema-v1 DTOs, mapping, validation, ZIP reading/writing, and atomic replacement.
- [x] (2026-08-07 00:00Z) Replaced the editor and Preview persistence integration with the asynchronous format-neutral facade.
- [x] (2026-08-07 00:00Z) Added the adapted read-only LiteDB v4 reader, removed LiteDB 4 from production, and completed migration/security coverage.
- [x] (2026-08-07 00:00Z) Completed focused and full validation. User verification confirmed legacy and package files load equivalently in the editor and Preview, controlled rejection of malformed packages, and a successful full build.

## Surprises & Discoveries

- Observation: The current service modifies a LiteDB file for both saves and loads through typed `Prop` deserialization.
  Evidence: `Services/PropModelPersistenceService.cs` opens `new LiteDatabase(path)`, calls `GetCollection<Prop>("props")`, and writes image files into LiteDB FileStorage. `GetModelAsync` is a `Task.Factory.StartNew` wrapper around the same synchronous LiteDB code.

- Observation: Legacy State conversion is already centralized but does not clear legacy fields or report a lossy conversion.
  Evidence: `Services/CustomPropStateMigrationService.cs` maps `StateDefinition` and `StateDefinitions` into `StateDefinitionModels`; the current method returns only whether it added data.

- Observation: Custom Prop persistence is shared by the editor and Preview, so a format-specific Preview reader would create divergent migration behavior.
  Evidence: `ViewModels/PropEditorViewModel.cs` calls `PropModelPersistenceService` for open/save, while `Modules/Preview/VixenPreview/VixenPreviewControl.cs` calls `GetModelAsync` for Preview import.

- Observation: LiteDB 4.1.4 is centrally pinned and referenced only by Custom Prop Editor in the inspected persistence path.
  Evidence: `Directory.Packages.props` declares `LiteDB` version `4.1.4`; `CustomPropEditor.csproj` references it; `Prop.cs` and `PropModelPersistenceService.cs` import LiteDB namespaces.

- Observation: The LiteDB v4 page reader can reconstruct a raw `props` BSON document and FileStorage image data without opening a LiteDB engine.
  Evidence: `LegacyLiteDbRawReaderPrototypeTests` passed four tests against deterministic v4 files, including a 17 MiB raw BSON document, while asserting the source SHA-256 hash and write timestamp remain unchanged.

- Observation: `dotnet test` with project build enabled cannot evaluate the test project in this environment because the dotnet CLI cannot import two C++ projects; after the full Visual Studio MSBuild solution build, direct `dotnet vstest` against the Debug test assembly discovers and runs the focused tests.
  Evidence: the initial CLI command reported missing `Microsoft.Cpp.Default.props`; `msbuild Vixen.sln -m -t:Build -p:Configuration=Debug` succeeded, then `dotnet vstest src\\Vixen.Tests\\bin\\Debug\\Vixen.Tests.dll --TestCaseFilter:"FullyQualifiedName~LegacyLiteDbRawReaderPrototypeTests"` passed 4 tests.

## Decision Log

- Decision: Keep the `.prp` extension but identify its contents from bytes, not from the extension.
  Rationale: Existing file dialogs, inventory links, and user files keep working; content detection safely distinguishes package and legacy data.
  Date/Author: 2026-08-07 / Codex

- Decision: Make schema version 1 a two-entry ZIP package with `prop.json` and `background.jpg`; JSON never contains base64 image data.
  Rationale: The image remains in one portable file without base64's roughly one-third expansion or an unnecessary large string allocation.
  Date/Author: 2026-08-07 / Codex

- Decision: Persist a normalized directed acyclic graph (DAG), meaning a directed hierarchy in which an element may have more than one parent but no route returns to its starting element.
  Rationale: `ElementModel` instances can be shared by multiple groups. Storing each element once by ID prevents duplication and avoids serializing WPF bindings, parents, and calculated properties.
  Date/Author: 2026-08-07 / Codex

- Decision: Use an adapted, read-only LiteDB 5.0.21 legacy datafile/BSON reader for the final compatibility path; do not use `Upgrade=true`, a LiteDB engine write operation, or typed legacy materialization.
  Rationale: LiteDB 5 upgrades v4 by reinserting documents and therefore applies its 16 MiB document limit during upgrade. A raw reader can obtain the old v4 document before reinsertion. This also permits removal of the vulnerable LiteDB 4 package.
  Date/Author: 2026-08-07 / Codex

- Decision: Support legacy LiteDB v4 `.prp` input only; do not implement a LiteDB v5 file reader.
  Rationale: No Vixen release created LiteDB v5 prop files. The attempted v5 migration was reverted as soon as its document-size failure was found, so v5 support would add unneeded parsing and security surface.
  Date/Author: 2026-08-07 / Codex

- Decision: Do not retain a LiteDB 4 production adapter after the proof-of-concept milestone.
  Rationale: A raw untyped adapter could reduce the unsafe `_type` path but still leaves a known-vulnerable package in dependency audits. The final acceptance criterion is an audit with no LiteDB 4.1.4 finding.
  Date/Author: 2026-08-07 / Codex

- Decision: Treat unknown newer schemas, validation failures, and lossy legacy State conversion as save-blocking errors.
  Rationale: Replacing a file after information was rejected or cannot be represented would silently destroy user data.
  Date/Author: 2026-08-07 / Codex

- Decision: Generate the Milestone 2 v4 fixtures at test time instead of committing binary files.
  Rationale: The test-only LiteDB 4 fixture builder deterministically creates JPEG/PNG, legacy State-shaped, malicious `_type`, and over-16-MiB cases while preventing a large binary fixture from entering the repository. The production reader itself has no LiteDB package dependency.
  Date/Author: 2026-08-07 / Codex

- Decision: Limit package input to two uncompressed logical entries, a 4 MiB JSON manifest, a 64 MiB JPEG, and a 100:1 compression ratio.
  Rationale: These bounds admit realistic prop images while rejecting ZIP-bomb-shaped input before JSON or WPF image decoding.
  Date/Author: 2026-08-07 / Codex

- Decision: Track a legacy source path against its loaded live `Prop` with a weak association until its first successful save.
  Rationale: This enables the one-time backup without broadening the editor model or exposing migration state to editor and Preview callers.
  Date/Author: 2026-08-07 / Codex

## Outcomes & Retrospective

Milestones 1 through 7 are complete. The editor and Preview now share a format-neutral persistence facade: it reads schema-1 packages and legacy LiteDB v4 props without modifying the legacy source, then writes only validated schema-1 packages with a one-time backup on the first successful same-path save. User verification confirmed matching State definitions, Face data, element hierarchy and ordering between old and new versions; verified the package entries with 7-Zip; loaded both legacy and new props into Preview; and exercised backup, repeat-save, large-file, deep/shared-graph, failed-save, and malformed-input cases. The full build and automated tests pass. LiteDB is absent from both production and test package dependencies; package, document-mapper, and image-dimension tests retain coverage for the supported schema-1 format. Jira Improvement VIX-3967 remains in `New Ticket`; no workflow transition was made.

## Context and Orientation

The Custom Prop Editor module is `src/Vixen.Modules/App/CustomPropEditor`. Its live editor domain consists of `Model/Prop.cs`, `Model/ElementModel.cs`, `Model/Light.cs`, the metadata classes in `Model`, `FaceDefinition`, and authored State types `StateDefinitionModel` and `StateItemModel`. These types inherit WPF-oriented `BindableBase` classes and include calculated fields and bidirectional relationships, so they are not file contracts.

Today, `Services/PropModelPersistenceService.cs` is a static LiteDB service. `SaveModel` and `UpdateModel` serialize a `Prop` collection and JPEG image; `GetModel` loads it; `GetModelAsync` runs synchronous database work on a worker task. `Services/PropModelServices.cs` calls it during editor load. `ViewModels/PropEditorViewModel.cs` calls it during open, Save, and Save As. Preview's `VixenPreviewControl.cs` calls the asynchronous method. Existing image-dimension persistence coverage is in `src/Vixen.Tests/App/CustomPropEditor/BackgroundImageScaling/PropImageDimensionPersistenceTests.cs`.

The replacement is a facade—one service that selects a reader based on file contents—over a package reader/writer and a legacy reader. A persistence document is a plain DTO (data-transfer object), a class whose only job is to hold explicitly named file values. It must not derive from `BindableBase`, reference WPF image types, or reference LiteDB. New files use `System.Text.Json` source generation so the permitted serialized types are explicit and no CLR runtime type name is written or accepted.

Schema 1 has exactly these logical records. `PropDocument` has `format` equal to `vixen.custom-prop`, `schemaVersion` equal to integer `1`, a `prop` scalar/metadata record, `rootElementId`, a unique `elements` array, and an `image` record declaring entry `background.jpg` and media type `image/jpeg`. Every `ElementDocument` has its ID, name, order, light size, model type, State-property ID, face definition, authored `stateDefinitionModels`, ordered `childIds`, and owned lights. The prop record contains ID, name, type, created-by, creation and modified dates in ISO-8601 form, opacity, width, height, vendor metadata, physical metadata, and information metadata. Each light contains its ID, coordinates, Z, and size. Represent `System.Drawing.Color` as a fixed eight-hex-digit `#AARRGGBB` string and enums as explicitly assigned string wire names. Include every currently meaningful scalar of the listed domain and metadata types after comparing their public persisted members; omit only calculated values and the relationships listed next.

Never write `ElementModel.Parents`, `Light.ParentModelId`, `ElementType`, `LightCount`, `IsLeaf`, `IsLightNode`, `IsGroupNode`, `StateDefinition`, or `StateDefinitions`. The loader derives parent IDs from each `childIds` list and derives each light's parent ID from the element that owns it. Schema 1 must omit the two legacy State fields entirely—not even `null` or an empty array. New prop creation, xModel import, State editing, and package hydration must use `StateDefinitionModels` only. The legacy importer may use a migration-only representation of legacy State values, must convert them before exposing a live prop, must clear the live legacy fields, and must fail a save if conversion cannot preserve meaningful data.

The package reader is a trust boundary: validate before building any live model. Accept no duplicate ZIP names, no directory entries, no absolute/path-traversal names, no unexpected entries, and exactly one `prop.json` and one `background.jpg`. Reject archives with more than two entries, uncompressed `prop.json` over 128 MiB, an image over 64 MiB, total uncompressed content over 192 MiB, or a compressed-to-uncompressed ratio above 100:1 for either entry. Decode the JPEG from its archive stream using `BitmapCacheOption.OnLoad`, require a valid JPEG and at most 100,000,000 pixels, then freeze it when possible. The writer always encodes JPEG and writes JSON compressed with `CompressionLevel.Optimal`; write the already compressed JPEG with `CompressionLevel.NoCompression`.

Validate `format`, supported schema, nonempty/unique element and light IDs, root existence, every child and State item reference, no cycles, full reachability from the root, finite numeric values, and valid image metadata. Use iterative traversal with explicit visiting and visited ID sets, not recursive traversal, for untrusted graphs. The mapper's write traversal similarly uses a queue and a seen-ID set so shared nodes are emitted once and deep graphs cannot overflow the stack.

## Plan of Work

### Milestone 1: Create the Improvement and record the delivery contract

Before changing code, create one Jira issue of type `Improvement` in project `VIX`. Use summary `Replace Custom Prop LiteDB persistence with validated versioned .prp packages`. State that it replaces only Custom Prop Editor `.prp` writes, keeps the extension, opens legacy LiteDB props read-only by content detection, and performs one-way conversion on the next successful save. Include the package contract, graph normalization, State exclusion/migration rules, archive and schema validation, atomic save/legacy-backup behavior, no-16-MiB legacy migration requirement, LiteDB 4 removal requirement, Preview integration, and the test plan below. Add the parent/epic, priority, fix version, labels, and assignee only when the team policy or project defaults supply them; do not invent them.

Add acceptance criteria in Jira stating: new files are ZIPs with exactly the two required entries; ordinary and over-16-MiB v4 files load without source modification; malformed or hostile input is rejected without a partial live model; legacy State is migrated losslessly or save is blocked; shared and deep graphs survive; failed saves leave a readable original; a successful legacy save creates one valid backup; Preview imports both formats through the same facade; and `dotnet list ... --vulnerable` reports no LiteDB 4.1.4 advisory. Add the exact validation commands from this plan as the Jira test plan. Record the created issue key in `Progress`, replace `VIX-<new-key>` placeholders in this plan, and update the Jira description once implementation discoveries refine it. Do not transition the issue unless separately requested.

### Milestone 2: Establish fixtures, contracts, and the raw-reader proof of concept

Create `src/Vixen.Tests/App/CustomPropEditor/Persistence` and place fixtures in a test-data folder under it with `CopyToOutputDirectory=PreserveNewest` in `Vixen.Tests.csproj` if required. Add a compact ordinary v4 `.prp` fixture with `background.jpg`, another with `background.png`, a fixture containing meaningful legacy `StateDefinition` and `StateDefinitions`, and a deliberately malformed fixture carrying nested `_type` metadata. Generate the over-16-MiB v4 fixture deterministically in a test or fixture-generation helper; it must contain one `props` document whose raw BSON document is larger than 16 MiB, rather than merely an archive file larger than that limit. Do not commit a needless huge binary if deterministic test construction is practical; document the generation command and measured document size.

Create an additive proof-of-concept test project area and an internal `LegacyLiteDbRawReaderPrototype` that imports only the necessary MIT-licensed portions of LiteDB 5.0.21's v4 `FileReaderV7` and raw BSON reading path. Store third-party source under a clearly named `Persistence/Legacy/LiteDb5021` folder, preserve copyright/license notices, record the upstream tag/commit in a `THIRD-PARTY-NOTICES.md` beside it, and expose no LiteDB database engine API. Its only observable result is raw access to the `props` document and `$/image/background.jpg` or `background.png` bytes without upgrade, reinsertion, or write access.

First write tests proving this prototype reads ordinary v4 and the generated over-16-MiB v4 fixture without modifying their hashes or timestamps. Test it recursively rejects any `_type` field before mapping. If the adapted reader cannot complete all three tests, stop before package integration, record the failure and source-version evidence in `Surprises & Discoveries`, and revise the Jira description and this plan with a safe alternative. Do not introduce LiteDB 4 as a production dependency to make the tests pass.

### Milestone 3: Define schema-1 documents, mapping, and validation

Add the following internal types under `src/Vixen.Modules/App/CustomPropEditor/Persistence/Documents`: `PropPackageDocument`, `PropDocument`, `ElementDocument`, `LightDocument`, `ImageDocument`, metadata documents, face documents, State definition documents, and `CustomPropJsonSerializerContext`. Use `JsonSerializable` attributes for the root and all nested DTO types, fixed `JsonPropertyName` values matching schema 1, and a source-generated context. Do not reuse live model classes as document classes and do not configure polymorphic serialization.

Add `Persistence/IPropDocumentMapper.cs` and `Persistence/PropDocumentMapper.cs` with the internal contract:

    internal interface IPropDocumentMapper
    {
        PropPackageDocument ToDocument(Prop prop);
        Prop ToModel(PropPackageDocument document, BitmapSource image);
    }

`ToDocument` must iteratively visit from `prop.RootNode`, emit each element once, preserve each parent’s child order, and reject a live graph with duplicate/missing IDs or cycles instead of silently writing it. `ToModel` must first construct all element and light instances by ID, then link child references, rebuild `Parents`, assign each light owner, set the root, normalize authored State models, and leave `StateDefinition` null and `StateDefinitions` empty. Add narrowly scoped internal factories or hydration methods to `Prop`, `ElementModel`, and `Light`; do not make setters public only to support persistence. Because this changes public/protected C# members if any existing visibility must change, the implementer must first read and follow `.agents/skills/csharp-docs/SKILL.md` and update XML documentation in the same change.

Add `Persistence/PropDocumentValidator.cs` and a typed `PropPersistenceException` that carries a safe user-facing message plus a diagnostic reason for logs. Validate the document before mapper hydration and validate a generated document before it is published. Make validation error messages identify the failing field or ID but never echo an entire untrusted JSON payload.

Add tests for round-tripping all meaningful prop, element, light, metadata, face, and State fields; one shared element under two parents must hydrate as the same object; a hierarchy deeper than 20 levels must work; package JSON must contain neither legacy State field; and duplicate IDs, missing references, cycles, orphans, nonfinite values, invalid image declarations, and invalid State item references must fail predictably.

### Milestone 4: Implement safe package I/O and atomic replacement

Add `Persistence/IPropFileReader.cs`, `Persistence/PropPackageReader.cs`, `Persistence/PropPackageWriter.cs`, and a small `Persistence/IPropImageCodec.cs` abstraction if WPF bitmap thread affinity is encountered. A reader returns a validated persistence document, fully loaded image, and a source-format marker; it does not mutate the file. The package reader first checks the ZIP signature and then applies all entry, size, compression, JSON, image, and document validation rules from Context and Orientation. It must stream archive entries and never extract them to disk. The package writer maps the live prop to a document, streams `prop.json` and encoded JPEG into a uniquely named temporary file in the destination directory, closes all handles, reopens and validates that temporary package, then publishes it.

Add `Persistence/AtomicPropFileWriter.cs` to serialize writes per canonical destination path. For an existing legacy source, compute a deterministic one-time backup path by appending `.legacy-litedb.bak` to the `.prp` path; if that path already exists, verify it is a legacy file and preserve it rather than overwrite it. On Windows, after temp-package validation, use `File.Replace(temp, destination, backup)` where available. For a new destination or a platform where replacement is unavailable, use an explicitly documented same-directory move sequence that never deletes the only readable source before the new package is validated. Delete only the temporary file on failure. Preserve the original when mapping, encoding, validation, or replacement fails.

Test a newly saved prop with a ZIP reader and assert exactly `prop.json` and `background.jpg`; verify the JPEG is not JSON/base64. Test duplicate ZIP names, traversal-like names, missing/extra entries, oversized uncompressed values, implausible ratios, corrupt JSON, corrupt/non-JPEG images, and an unsupported newer schema. Add a fault-injection seam after temporary-file creation and before publish; prove it leaves the original readable and removes or safely reports only its temporary file.

### Milestone 5: Add the final legacy provider and format-neutral asynchronous facade

Promote the successful raw-reader prototype to `Persistence/LegacyLiteDbPropReader.cs`. It must recognize LiteDB v4 headers only, read untyped raw BSON, recursively reject `_type`, explicitly map only known primitive/document/array fields into a migration-only legacy DTO, retrieve either legacy image entry, and never call typed `ToObject`, `GetCollection<T>`, insert, update, delete, shrink, upgrade, or any file-mutating API. Treat a LiteDB v5 header as unsupported/corrupt input with a compatibility error; do not add a v5 parser. Read v4 files with permissions that prevent writing when the operating system supports them; regardless of flags, prove byte-for-byte source preservation in tests.

Extend `CustomPropStateMigrationService` or add a result-returning companion that validates legacy State conversion. It must distinguish no legacy data, successful lossless migration, and a conversion that would discard meaningful data. After a successful conversion, clear all live legacy State properties. Add a legacy source-format marker to the persistence load result so the facade knows when the first save must request a backup.

Replace `PropModelPersistenceService` with a non-static, dependency-injected `IPropModelPersistenceService` facade. Its public contract is:

    public interface IPropModelPersistenceService
    {
        Task<Prop> LoadAsync(string path, CancellationToken cancellationToken = default);
        Task SaveAsync(Prop prop, string path, CancellationToken cancellationToken = default);
    }

It sniffs content in this order: valid ZIP package with manifest, recognized supported legacy header, otherwise unsupported/corrupt file. It uses asynchronous file and JSON streams, propagates cancellation before publish, and maps exceptions to log details plus clear UI-safe errors. It must not use `Task.Factory.StartNew` as a substitute for asynchronous I/O. Register the facade, readers, mapper, validator, writer, and image codec in the module’s existing dependency-registration location. Add XML documentation for the public interface and members, following the project `csharp-docs` skill.

Update `Services/PropModelServices.cs` and `ViewModels/PropEditorViewModel.cs` to await the facade for open/save and to show a busy indicator while I/O is pending, restore it in `finally`, and display a user-facing error without clearing the current prop or dirty flag on failure. Convert non-event-handler save/load commands to the existing Catel task-command pattern as necessary; follow `.agents/skills/catel-mvvm/SKILL.md` and `.agents/skills/csharp-async/SKILL.md` before making those WPF/async changes. Update `VixenPreviewControl.cs` to await only the same facade, restoring the cursor in `finally`; Preview must not contain ZIP or LiteDB branching logic.

### Milestone 6: Remove LiteDB 4 and complete migration coverage

Once all legacy-reader tests pass through the facade, remove `using LiteDB` and `[BsonIgnore]` from `Model/Prop.cs`, remove LiteDB references from the persistence service, remove the `LiteDB` package reference from `CustomPropEditor.csproj`, and remove its central version from `Directory.Packages.props` only after confirming no other project references it. Search the repository with `rg -n "LiteDB|Bson"` and retain only the adapted reader’s documented raw-file implementation/attribution and tests where necessary; no package dependency, typed mapper, or serializer attribute may remain.

Add end-to-end tests that load each legacy fixture through the facade, assert its source hash is unchanged, save it, reopen it as a package, and assert the expected `.legacy-litedb.bak` opens through the legacy reader. Test the one-time backup is not overwritten on later package saves. Test ordinary and over-16-MiB migration, meaningful legacy State migration and JSON absence of legacy fields, intentionally lossy legacy State blocking save without source replacement, and Preview import through the shared facade. Run the vulnerable-package audit and fail this milestone if LiteDB 4.1.4 remains reported.

### Milestone 7: Validate, document completion, and close the Jira loop

Run the focused persistence tests, broader Custom Prop Editor and Preview tests, Debug/Release solution builds, the vulnerable-package audit, and whitespace check listed below. Manually create a prop with an image, hierarchy, shared element, and State definition; save it; inspect the package; reopen it in the editor; then import it into Preview. Repeat opening/saving an approved ordinary legacy fixture and prove the backup exists and opens. Perform the over-16-MiB automated migration test rather than trying to create that prop manually.

Update the Jira description if implementation changes a requirement, acceptance criterion, or test plan. Add a Jira comment with the exact test/build results, fixture sizes, package-audit result, manual verification outcome, and any compatibility limitation. Update all living-plan sections with timestamps and actual evidence, write `Outcomes & Retrospective`, and provide a formatted commit message using `.agents/skills/commit-msg/SKILL.md` after each code-changing milestone; do not create a commit unless separately requested.

## Concrete Steps

All commands run from `C:\Dev\Vixen` in PowerShell. Begin each milestone by preserving unrelated work:

    git status --short
    Get-Content -Raw .agents\PLANS.md
    Get-Content -Raw docs\plans\vix-3967-custom-prop-persistence-lite-db-migration.md

Create the Jira Improvement with the repository `jira` skill. Read `.agents/skills/jira/SKILL.md` before using the connected Jira service. Paste the Milestone 1 scope, acceptance criteria, and validation plan into the issue; record the returned key in this plan and use it in branch/commit messages if the team directs that workflow.

Inspect the precise model surface before finalizing DTO fields:

    Get-ChildItem src\Vixen.Modules\App\CustomPropEditor\Model -Filter *.cs | Get-Content
    rg -n "PropModelPersistenceService|LoadProp\(|SaveModel\(|UpdateModel\(|GetModelAsync|StateDefinition|StateDefinitions" src\Vixen.Modules src\Vixen.Tests
    rg -n "LiteDB|Bson" Directory.Packages.props src

Run the initial focused tests before changing behavior:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor" --no-restore
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Preview.VixenPreview" --no-restore

During implementation, run focused persistence tests after each reader/writer/mapper change:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Persistence" --no-restore

Run integration and final checks after Milestones 5 and 6:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor|FullyQualifiedName~Preview.VixenPreview" --no-restore
    dotnet list src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj package --vulnerable --include-transitive
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release
    git diff --check
    git status --short

Expected outcomes are zero failed tests, successful builds with zero errors, no output from `git diff --check`, and no LiteDB 4.1.4 vulnerability in the package audit. Investigate any package advisory before treating the work as complete; do not suppress it merely to pass the command.

## Validation and Acceptance

Acceptance is behavior-based. A new prop save produces a `.prp` that a ZIP inspector reports contains exactly `prop.json` and `background.jpg`; `prop.json` identifies schema 1, contains each element once, preserves child ordering, and contains no legacy State field or base64 image. Reopening it restores the same image pixels, logical width and height, metadata, face data, authored State data, lights, and a shared child as one object with two rebuilt parents.

Given a normal legacy v4 `.prp`, a legacy v4 prop with a raw `props` document over 16 MiB, or a legacy PNG-background fixture, when opened, then the prop loads and the original file hash and timestamp remain unchanged. When the user successfully saves it, then the file becomes a readable package and a valid one-time `.legacy-litedb.bak` remains. Given a save failure after temporary output exists, then the original remains readable and the app reports the failure.

Given a package or legacy file with an invalid schema, duplicate ID, missing reference, cycle, unreachable element, nonfinite number, unsafe ZIP entry, excessive archive ratio/size, corrupt image, unexpected newer schema, or legacy `_type` metadata, when opened, then no live prop is partially accepted and the user receives a clear error. Given a non-empty legacy State value that cannot be represented losslessly, when saved, then save is blocked and no source file is replaced.

Given Custom Prop Editor or Preview imports a `.prp`, when it is new or legacy, then both use `IPropModelPersistenceService` and behave the same apart from editor-only save behavior. The final vulnerability audit must not list LiteDB 4.1.4, and all commands in Concrete Steps must succeed.

## Idempotence and Recovery

All tests and builds are repeatable. Test files must use unique temporary paths and cleanup only paths they created. Never modify a legacy input on open, never use `Upgrade=true`, and never run a database shrink/rebuild in the legacy path. Keep fixture originals immutable and compare hashes before/after loading.

The writer’s only replaceable artifact before publication is a unique same-directory temporary file. If it fails, leave the destination untouched, dispose every archive/image stream, and remove the temporary file when possible. If cleanup fails, log its exact path for manual deletion; do not delete a broad directory. A pre-existing backup is evidence of an earlier migration and must not be overwritten. If the adapted reader proof of concept fails, retain LiteDB 4 unchanged for that branch of work and do not merge a partial migration; record the blocker and pursue a separately reviewed safe reader approach.

## Artifacts and Notes

Schema-1 `prop.json` is structurally equivalent to this abbreviated example; actual DTOs include all meaningful metadata, face, State, and light fields:

    {
      "format": "vixen.custom-prop",
      "schemaVersion": 1,
      "prop": { "id": "...", "name": "Star", "opacity": 1.0, "width": 800.0, "height": 600.0 },
      "rootElementId": "...",
      "elements": [
        { "id": "...", "name": "Star", "childIds": ["..."], "lights": [] }
      ],
      "image": { "entry": "background.jpg", "mediaType": "image/jpeg" }
    }

The mapper’s essential nonrecursive write algorithm is:

    enqueue root element
    while queue is not empty:
        remove next element
        if its ID was already seen: continue
        mark ID seen and emit its scalar fields, ordered child IDs, and owned lights
        enqueue its children

The pre-change dependency state to remove is:

    Directory.Packages.props: LiteDB 4.1.4
    src/Vixen.Modules/App/CustomPropEditor/CustomPropEditor.csproj: <PackageReference Include="LiteDB" />
    src/Vixen.Modules/App/CustomPropEditor/Services/PropModelPersistenceService.cs: typed LiteDB CRUD

The raw legacy reader must carry local MIT attribution for imported LiteDB 5.0.21 code. Its implementation may read raw fields only; it must not be exposed as a general database API and must not deserialize a runtime type based on file data.

## Interfaces and Dependencies

Use BCL `System.IO.Compression` for ZIP archives and `System.Text.Json` source generation for schema JSON. Do not add a database package for the new format. Use `System.Windows.Media.Imaging` only behind the image codec/mapper boundary and detach decoded images from archive streams with `BitmapCacheOption.OnLoad`.

At the end of the work, the following public interface exists in `Services/IPropModelPersistenceService.cs` with XML documentation:

    public interface IPropModelPersistenceService
    {
        Task<Prop> LoadAsync(string path, CancellationToken cancellationToken = default);
        Task SaveAsync(Prop prop, string path, CancellationToken cancellationToken = default);
    }

The implementation keeps `IPropFileReader`, `IPropDocumentMapper`, validation, raw legacy DTOs, package DTOs, and atomic-write machinery internal to Custom Prop Editor. The facade is responsible for source-format detection and migration orchestration. A load result retains internal source-format state so saving a legacy-loaded prop makes one backup; callers receive only a valid `Prop` or a controlled exception.

## Revision Notes

2026-08-07 / Codex: Created from the Custom Prop persistence architecture handoff and direct inspection of the current LiteDB persistence, State migration, editor, Preview, project references, and existing persistence test. Added an initial Jira Improvement creation milestone because no issue key was supplied, then made every later milestone independently describable in that issue.

2026-08-07 / Codex: Clarified from review that no LiteDB v5 `.prp` files were ever released: the v5 attempt was reverted immediately. The compatibility contract, final legacy-provider milestone, and decision log now limit import/migration work to LiteDB v4 and explicitly reject v5 input rather than planning an unnecessary v5 reader.

2026-08-07 / Codex: Completed Milestone 1. Validated that `VIX` permits creation and that `Improvement` is an available issue type, then created VIX-3967 with the Custom Prop Editor component, this plan's scope, acceptance criteria, and validation plan. The issue remains in `New Ticket`; no parent, priority override, fix version, labels, or assignee was invented. Renamed this document to the repository ticket-key convention.

2026-08-07 / Codex: Completed Milestone 2. Added the unencrypted LiteDB v4 raw-reader proof of concept under `Persistence/Legacy/LiteDb5021` with MIT attribution to LiteDB v5.0.21 source commit `84065086a8e8716063b255d0abb332708d0b2ad3`. Deterministic tests create temporary v4 files rather than committing a large fixture and passed JPEG/PNG image reconstruction, legacy State-shaped BSON, nested `_type` rejection, source hash/timestamp preservation, and a 17 MiB raw props document. The full Debug solution build passed; the focused test command used direct `dotnet vstest` after that build because the dotnet CLI build path lacks the installed C++ targets.

2026-08-07 / Codex: Completed Milestone 4. Added strict two-entry ZIP package I/O with ZIP-signature, path, size, compression-ratio, JSON, schema, and JPEG validation. The writer creates a unique same-directory temporary archive, validates it through the reader, supports an injected pre-publish failure seam, and uses a per-canonical-path publisher with `File.Replace` when available. The Custom Prop Editor module build passed; the aggregate test project could not compile because native referenced projects lack x86 apphost/reference outputs in this environment.

2026-08-07 / Codex: Completed Milestone 5. Added the documented public asynchronous persistence interface and internal facade, routed editor and Preview calls through the same registered service, and restored busy/cursor state in `finally` blocks. The facade recognizes schema-1 ZIP content before LiteDB v4 headers, maps v4 raw BSON without a LiteDB engine, and requests the deterministic legacy backup only on the first successful same-path legacy save. Custom Prop Editor and Vixen Preview module builds passed; the aggregate test project remains blocked by missing native x86 apphost/reference outputs.

2026-08-07 / Codex: Completed Milestone 6. Removed the `LiteDB` package reference and the model's LiteDB serialization attribute, leaving raw v4 page/BSON reading as the compatibility path. `dotnet list src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj package --vulnerable --include-transitive` reported no vulnerable packages. The legacy fixture builder still uses LiteDB only in test source and will be replaced by a package-free fixture writer when aggregate test compilation is unblocked.

2026-08-07 / Codex: Corrected the legacy State mapper after read-only inspection of a user-provided v4 prop. It now maps authored `StateDefinitionModels`, State item colors and assignments, and LiteDB `_id` values rather than only obsolete State rows. `Santa Waving.prp` hydrated nine authored State definitions and retained 130 assignments in its first item.

2026-08-07 / Codex: Final user verification passed. Side-by-side comparison confirmed State definitions, Face information, element tree, and order match; 7-Zip confirmed saved packages contain `prop.json` and `background.jpg`; and Preview accepted both legacy v4 and schema-1 files. The remaining migration, malformed-package, backup, repeat-save, deep/shared graph, and large-file checks passed, as did automated tests and the full build. A package with its background image removed produced the intended controlled load error.

2026-08-08 / Codex: Removed the test-only LiteDB 4 fixture builder and raw-reader proof suite to eliminate the last LiteDB package dependency. The supported schema-1 package, DTO mapper, and image persistence tests remain; user-completed manual migration validation continues to cover real legacy v4 files, including large and malformed cases.

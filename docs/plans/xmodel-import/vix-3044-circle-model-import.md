# Import xLights Circle xModels into the Custom Prop Editor

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This plan follows `.agents/PLANS.md` from the repository root. Keep this document self-contained and revise it whenever implementation discoveries change the approach. The source specification for this work is `docs/vix-3044-circle-model-import.md`.

## Purpose / Big Picture

Vixen users can currently import xLights custom models into the Custom Prop Editor, but xLights circle models used for spinners and concentric-ring props still import as unsupported model types. After this change, a user can choose `File > Import xModel` in the Custom Prop Editor, select a standalone `<circlemodel>` file or a wrapped `<model DisplayAs="Circle">` file, and get a usable Vixen prop with correctly ordered light nodes, concentric visual placement, generated circle groups, and imported xLights submodels.

The behavior is visible in automated tests and in the UI. Tests will import small embedded circle xModel XML strings and assert the Vixen prop tree, node order, coordinates, generated circle groups, submodel ranges, and validation errors. Manual validation will import the committed examples in `docs/references/circlemodel` and show that no unsupported-model error appears and that the imported prop visually matches the file name's top/bottom, inside/outside, and clockwise/counter-clockwise wiring intent.

## Progress

- [x] (2026-07-17) Read `.agents/PLANS.md`, the committed specification `docs/vix-3044-circle-model-import.md`, the existing wrapper import plan, the current xLights importer, current importer tests, local circle reference files, and the project `dotnet-design-pattern-review` skill.
- [x] (2026-07-17) Created this ExecPlan with the confirmed decisions that circle groups follow wiring order, `LayerSizes` follows xLights behavior, and invalid `centerPercent` values use deterministic default/clamp behavior.
- [x] (2026-07-18) Updated Jira issue `VIX-3044` with the finalized requirements, design notes, acceptance criteria, test plan, manual validation plan, and this ExecPlan path.
- [x] (2026-07-18) Read `.agents/skills/dotnet-best-practices/SKILL.md` before adding test code. Deferred `.agents/skills/csharp-async/SKILL.md`, `.agents/skills/csharp-docs/SKILL.md`, and `.agents/skills/dotnet-design-pattern-review/SKILL.md` until importer async, public API, or parser abstraction code changes begin.
- [x] (2026-07-18) Added focused circle import characterization tests in `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelCircleImportTests.cs`.
- [x] (2026-07-18) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore`; baseline result is 12 failed, 34 passed, 0 skipped, 46 total. The failing tests are the new circle success-path behavior tests and fail because current circle imports return null. Existing importer tests and the new invalid-circle/custom-regression tests pass.
- [x] (2026-07-20) Refactored xModel import into internal parser and shared assembly pieces without changing custom model behavior. Added `IXModelElementParser`, `CustomXModelElementParser`, `XModelParsedModel`, `XModelGeneratedGroup`, `XModelElementMetadata`, and `XModelChildElementImporter`.
- [x] (2026-07-20) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore`; result remained the intended milestone-2 baseline: 12 failed, 34 passed, 0 skipped, 46 total. Existing custom import behavior still passes, and circle success-path tests still fail because circle parsing has not been implemented.
- [x] (2026-07-20) Implemented circle parser support for standalone `<circlemodel>` and wrapped `<model DisplayAs="Circle">` elements, including required attribute validation, tolerated optional defaults, `centerPercent` default/clamp handling, and shared child metadata parsing.
- [x] (2026-07-20) Added invalid `Dir`, `InsideOut`, and `StartSide` coverage to `XModelCircleImportTests`.
- [x] (2026-07-20) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore`; result is 12 failed, 37 passed, 0 skipped, 49 total. The remaining failures are node traversal, coordinate, and generated circle group expectations for milestones 5 and 6.
- [x] (2026-07-20) Implemented xLights-compatible layer traversal and coordinate generation. Circle nodes now use wired ring order, xLights-derived radii, top/bottom start side, direction, normalized coordinates, and stored per-ring node order metadata for generated circle groups.
- [x] (2026-07-20) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore`; result is 7 failed, 42 passed, 0 skipped, 49 total. The remaining failures are generated circle group expectations for milestone 6.
- [x] (2026-07-20) Implemented generated circle groups under `<model name> {1} - Circles`, with child groups named by wiring-order circle number and populated from existing light nodes by node order.
- [x] (2026-07-20) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore`; result is 0 failed, 49 passed, 0 skipped, 49 total.
- [x] (2026-07-20) Finished wrapped circle model support by covering multi-model wrapper selection for supported circle models and selected unsupported models. Updated unsupported-model error text to list custom and circle support.
- [x] (2026-07-20) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore`; result is 0 failed, 51 passed, 0 skipped, 51 total.
- [x] (2026-07-20) Applied xLights `ScaleX` and `ScaleY` to circle coordinates before rounding so imported circles use the exported layout scale and avoid small, jagged geometry.
- [x] (2026-07-20) Ran `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore`; result is 0 failed, 53 passed, 0 skipped, 53 total.
- [ ] Run broader relevant tests.
- [ ] Manually validate the committed circle examples in the Custom Prop Editor.
- [ ] Record final implementation results in `Outcomes & Retrospective`.

## Surprises & Discoveries

- Observation: The current importer has already been refactored to use `XDocument`, supports a root `models` wrapper, and has a selection service for embedded model choices.
  Evidence: `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs` loads an `XDocument`, routes root `custommodel` elements, routes root `models` elements through `ImportModelsWrapperAsync`, and injects `IXModelSelectionService`.
- Observation: Wrapped non-custom model types are currently classified by `DisplayAs` but unsupported because only custom models mark `XModelSelectionItem.IsSupported` true.
  Evidence: `ResolveModelType` maps `<model DisplayAs="Circle">` to `circlemodel`, while `IsCustomModelElement` only returns true for `<custommodel>` or `<model DisplayAs="Custom">`.
- Observation: Circle reference files cover both supported input shapes required by this plan.
  Evidence: `docs/references/circlemodel/circle-vendor.xmodel` starts with `<circlemodel ... DisplayAs="Circle" ...>`, while the other reference files use `<models type="exported"><model ... DisplayAs="Circle" ... /></models>`.
- Observation: Existing focused importer tests use embedded XML strings written to temporary `.xmodel` files rather than reading reference files.
  Evidence: `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelImportHierarchyTests.cs` writes `modelXml` to a temp file and calls `new XModelImport().ImportAsync(filePath)`.
- Observation: The xLights circle source defaults `centerPercent` to zero and assigns supplied values directly in the circle class.
  Evidence: xLights `CircleModel.h` declares `int _centerPercent = 0;` and `void SetCenterPercent(int val) { _centerPercent = val; }`; this plan still clamps imported numeric values to `0..100` as a Vixen import-safety rule.
- Observation: The focused importer test suite compiles with the new circle tests before implementation, and the red baseline is isolated to circle success-path imports.
  Evidence: `dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore` reported 12 failed, 34 passed, 0 skipped, 46 total on 2026-07-18. The new invalid `LayerSizes`, mismatched `PixelCount`, and wrapped custom regression tests pass against current behavior.
- Observation: The parser refactor can preserve the same focused-test baseline without making Circle supported yet.
  Evidence: After moving custom parsing into `CustomXModelElementParser`, shared child parsing into `XModelChildElementImporter`, and assembly input to `XModelParsedModel`, the focused test command still reports 12 failed, 34 passed, 0 skipped, 46 total on 2026-07-20.
- Observation: Circle parsing support moves valid circle files past the unsupported-model path before node generation is implemented.
  Evidence: After registering `CircleXModelElementParser`, valid circle tests fail on empty model nodes or missing generated circle groups rather than null imports. Invalid required attributes and mismatched `PixelCount` return null and pass their tests.
- Observation: Circle node generation satisfies the model-node, coordinate, center-radius, and range-submodel tests before generated circle groups exist.
  Evidence: The focused xModel importer test run reports 7 failed, 42 passed, 0 skipped, 49 total after node generation. The remaining failures all look for `<name> {1} - Circles`.
- Observation: The existing generated group assembly path can create circle groups without duplicating light nodes.
  Evidence: After adding generated circle group definitions from `CircleXModelElementParser`, the focused xModel importer test run reports 0 failed, 49 passed, 0 skipped, 49 total.
- Observation: Wrapped circle model support was already enabled by parser-based candidate classification; the missing milestone work was focused coverage for mixed wrappers and stale unsupported-model text.
  Evidence: `XModelImport` marks candidates supported when `GetModelParser(element) != null`; the new wrapper tests pass for selecting a circle model from a mixed wrapper and for selecting an unsupported model.
- Observation: xLights circle screen coordinates are generated in a small render space and then transformed by screen-location scale.
  Evidence: xLights `CircleModel::SetCircleCoord()` sets render size from `maxLights`, while `ModelScreenLocation` stores `scalex` and `scaley`; local wrapped circle examples export `ScaleX` and `ScaleY`.

## Decision Log

- Decision: Generated circle groups follow wiring order. `Circle 1` is the first wired ring, not always the physical inner ring.
  Rationale: Effects and submodel ranges refer to node order, and the user explicitly confirmed that all circle grouping should follow wiring order.
  Date/Author: 2026-07-17 / Codex
- Decision: Match xLights layer traversal for `LayerSizes`.
  Rationale: The import should land on the same physical circle definitions as xLights, including its use of reversed layer lookup when iterating strands. The implementation must not invent an independent interpretation of the raw `LayerSizes` string.
  Date/Author: 2026-07-17 / Codex
- Decision: Missing or non-numeric `centerPercent` imports as `0`; numeric values below `0` clamp to `0`; numeric values above `100` clamp to `100`.
  Rationale: xLights' circle field default is zero, and clamping prevents invalid imported geometry from creating inverted or oversized inner-ring radii in Vixen.
  Date/Author: 2026-07-17 / Codex
- Decision: Reuse `ElementModelType.SubModel` for generated circle groups unless implementation discovers an unavoidable need for a new enum value.
  Rationale: Adding `ElementModelType.Circle` would change public persisted model semantics and require broader documentation and migration review. Generated circle groups behave like targeting groups, which already fits the imported submodel role.
  Date/Author: 2026-07-17 / Codex
- Decision: Add reusable internal parser/assembly structure before adding circle parsing.
  Rationale: `XModelImport` already owns wrapper selection, custom parsing, child metadata parsing, and prop assembly. Circle support adds a second model-specific parser and generated groups; extracting shared seams first reduces duplication and keeps later xLights model types practical.
  Date/Author: 2026-07-17 / Codex

## Outcomes & Retrospective

Implementation milestones 1 through 7 are complete. Broader automated validation, manual reference-file validation, and final retrospective notes remain.

## Context and Orientation

Vixen is a Windows desktop application for sequencing light shows. The Custom Prop Editor is a WPF app module under `src/Vixen.Modules/App/CustomPropEditor`. A user imports xLights model files through the Custom Prop Editor menu item `File > Import xModel` in `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`. That command reaches `PropEditorViewModel.ImportCommand` in `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`, asks the user for a `.xmodel` path, constructs `IModelImport import = new XModelImport();`, and replaces the current editor prop only when `ImportAsync(path)` returns a non-null `Prop`.

An xLights `.xmodel` file is XML. A standalone custom model uses a root element named `custommodel`. A wrapped export uses a root element named `models` with direct child elements named `model`. The child model's `DisplayAs` attribute identifies the xLights model type, such as `DisplayAs="Custom"` or `DisplayAs="Circle"`. A standalone circle model uses a root element named `circlemodel`.

The current importer is `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`. Its public entry point is `Task<Prop> ImportAsync(string filePath)`, from `src/Vixen.Modules/App/CustomPropEditor/Import/IModelImport.cs`. It reads the XML into an `XDocument`, imports `<custommodel>` roots directly, imports root `<models>` wrappers by enumerating direct child `<model>` elements, and uses `IXModelSelectionService` when a wrapper contains multiple choices. A wrapped `<model DisplayAs="Custom">` is parsed as a custom model. A wrapped `<model DisplayAs="Circle">` currently resolves to unsupported model type `circlemodel`.

The existing custom import path creates an internal `CustomModel` from `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CustomModel.cs`. `CustomModel` stores the model name, pixel size, string metadata, a dictionary of `ModelNode` objects keyed by node order, imported submodels, imported face information, and imported state information. `ModelNode` in `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/ModelNode.cs` has `Order`, `X`, and `Y` integer properties. `XModelImport.Assemble(CustomModel cm)` creates the Vixen prop tree from this intermediate object.

The assembled tree currently creates a root prop named `<model name> {1}` and a model group named `<model name> {1} - Model`. It adds one light node per imported `ModelNode`, with names like `<model name> {1} Px 1`. It then creates root-level imported submodel groups, face groups, and legacy State groups, and attaches imported State definition metadata to the model group. The current range parser handles xLights range strings such as `1`, `1-10`, `10-1`, and comma-separated combinations by building `RangeGroup` objects under `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/Ranges`.

Circle models are xLights models for concentric rings. The important XML attributes are `LayerSizes`, `InsideOut`, `StartSide`, `Dir`, `centerPercent`, `PixelSize`, `PixelCount`, `NumStrings`, `NodesPerString`, and their older standalone equivalents `parm1`, `parm2`, and `parm3`. `LayerSizes` is a comma-separated list of positive integer node counts for each ring. `InsideOut="0"` means the first wired ring is outside; `InsideOut="1"` means the first wired ring is inside. `StartSide="B"` starts each ring at the bottom, while `StartSide="T"` starts each ring at the top. `Dir="L"` means clockwise and `Dir="R"` means counter-clockwise. `centerPercent` controls the inner-ring radius as a percentage of the outer-ring radius.

The committed local examples live in `docs/references/circlemodel`. Unit tests must not read these files directly; use embedded minimal XML strings so tests are stable and self-contained. Manual validation should import these files through the UI.

The xLights `CircleModel` behavior that matters for this implementation is small enough to embed here. xLights chooses the maximum layer size as the render width and height. It sets `maxRadius = maxLights / 2.0` and `minRadius = centerPercent / 100.0 * maxRadius`. For layer traversal, `GetStrandLength(strand)` returns the layer size at `layerCount - strand - 1`. That reversed lookup is important because Vixen must follow xLights' wiring order rather than only the raw textual order of `LayerSizes`. xLights then applies screen-location scale outside the circle coordinate generator; Vixen bakes `ScaleX` and `ScaleY` into imported coordinates because the Custom Prop Editor stores fixed light positions.

## Plan of Work

Milestone 1 updates Jira and confirms implementation prerequisites. Update Jira issue `VIX-3044` with a concise version of this plan: the problem, supported XML shapes, confirmed wiring-order decisions, parser/assembly design, acceptance criteria, automated test plan, manual validation plan, and this file path. If an agent performs the Jira update, read `.agents/skills/jira/SKILL.md` first and use the Atlassian tooling prescribed there. Before code changes, read `.agents/skills/dotnet-best-practices/SKILL.md`. Read `.agents/skills/csharp-async/SKILL.md` if the importer async flow is changed beyond existing awaits. Read `.agents/skills/csharp-docs/SKILL.md` if any public or protected APIs are added or modified. Re-read `.agents/skills/dotnet-design-pattern-review/SKILL.md` before finalizing the parser abstraction.

Milestone 2 adds focused failing tests for circle support. Add a new test class such as `src/Vixen.Tests/App/CustomPropEditor/Import/XLights/XModelCircleImportTests.cs` in the same namespace as `XModelImportHierarchyTests`. Use the same helper pattern: write embedded XML to a temporary `.xmodel`, call `XModelImport.ImportAsync`, and delete the temporary file in `finally`. The initial tests should prove the target behaviors and will fail before implementation because circle models are unsupported. Cover a standalone `<circlemodel>` with one ring; a wrapped `<models><model DisplayAs="Circle">` with one ring; a three-ring `InsideOut="0"` model whose generated `Circle 1` contains the first wired outside-ring nodes; a three-ring `InsideOut="1"` model whose generated `Circle 1` contains the first wired inside-ring nodes; top versus bottom first-node coordinates; clockwise versus counter-clockwise second-node coordinates; `centerPercent` radius effects and default/clamp behavior; imported `subModel type="ranges"` mapping to generated nodes; invalid `LayerSizes`; mismatched `PixelCount`; and a custom wrapper regression proving custom import still works.

Milestone 3 refactors the importer into shared model parsing and assembly pieces while keeping behavior unchanged. Keep `XModelImport.ImportAsync(string filePath)` as the only public entry point. Introduce internal types under `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/` with names that describe the domain rather than implementation mechanics. A practical shape is an internal parser interface such as `IXModelElementParser`, internal parser classes such as `CustomXModelElementParser` and later `CircleXModelElementParser`, and a shared parsed model object that can carry the current `CustomModel` data plus generated grouping metadata. Avoid changing public APIs. If the cleanest implementation modifies public or protected APIs, update XML comments in the same change.

The refactor should extract common child parsing for `subModel`, `faceInfo`, `stateInfo`, and ignored `modelGroup` elements out of the custom-specific branch. It should also extract shared prop assembly so both custom and circle parsed models can create a root prop, the primary model group, imported child metadata, and generated groups. After this milestone, existing custom model and wrapper tests should still pass, and circle tests may still fail as unsupported.

Milestone 4 implements circle attribute parsing and validation. Add a circle parser that accepts a standalone `<circlemodel>` root and a wrapped `<model DisplayAs="Circle">`. Parse `LayerSizes` into positive integers, parse `Dir`, `InsideOut`, `StartSide`, `centerPercent` or fallback `parm3`, `PixelSize`, `PixelCount`, `NumStrings` or `parm1`, and `NodesPerString` or `parm2`. Fail import with a clear `IMessageService.ShowErrorAsync` message when `LayerSizes` is missing or contains no positive values, `Dir` is not `L` or `R`, `InsideOut` is not `0` or `1`, `StartSide` is not `B` or `T`, or positive `PixelCount` differs from the sum of `LayerSizes`. Treat missing or invalid `PixelSize` as `1`. Treat missing or non-numeric `centerPercent` as `0`; clamp numeric values below `0` to `0` and above `100` to `100`. Log warnings for tolerated bad optional values using structured NLog messages.

Milestone 5 implements xLights-compatible node traversal and coordinate generation. The parser should generate `ModelNode` values directly instead of looking for `CustomModel` or `CustomModelCompressed`. Follow xLights layer traversal. Store enough per-ring order information to create generated circle groups later. The first wired ring depends on `InsideOut`: with `InsideOut="0"`, the first wired ring is the outside ring; with `InsideOut="1"`, the first wired ring is the inside ring. `Circle 1` must always refer to this first wired ring. Within each ring, node order starts at `StartSide` and advances according to `Dir`.

For coordinates, use deterministic Vixen canvas coordinates derived from the xLights circle math. Let `maxLights` be `max(LayerSizes)`, `maxRadius = maxLights / 2.0`, and `minRadius = centerPercent / 100.0 * maxRadius`. If there is one ring, use `maxRadius`. If there are multiple rings, distribute ring radii evenly from inside to outside, but assign those radii to wired rings according to the same xLights traversal used for node order. For each node in a ring, compute an angle where `StartSide="T"` starts at the top and `StartSide="B"` starts at the bottom; `Dir="L"` advances clockwise and `Dir="R"` advances counter-clockwise. Use `sin(angle) * radius * ScaleX` for X and `cos(angle) * radius * ScaleY` for Y, treating missing, invalid, or non-positive scale values as `1.0`. Round after applying scale, then normalize all generated points so minimum X and Y are zero before assembly adds its existing offset. Use one documented integer rounding strategy and keep tests aligned with it.

Milestone 6 creates generated circle groups during assembly. Extend the intermediate parsed model with generated group definitions. A generated group definition should be able to express a parent group named `<model name> {1} - Circles` and child groups named `<model name> {1} - Circle 1`, `<model name> {1} - Circle 2`, and so on. Each child group should contain references to existing light nodes by node order, not duplicate light nodes. Set generated circle group `ModelType` to `ElementModelType.SubModel` unless this plan is explicitly revised to add a new enum value. Imported xLights `subModel` groups remain root-level groups as they are today.

Milestone 7 finishes wrapper support for circle models. Update model classification so a wrapped `<model DisplayAs="Circle">` marks the selection item as supported. Single supported wrapped circle models should import without prompting. Wrappers containing multiple models should use the existing selection service. Selecting a circle model imports through the circle parser; selecting an unsupported model still shows the normal unsupported-model error. Existing wrapped custom model behavior must remain unchanged.

Milestone 8 runs automated validation. Run focused importer tests first from `C:\Dev\Vixen`:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore

If the focused tests pass, run the broader test project when practical:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore

If `--no-restore` fails because dependencies have not been restored in the local environment, run the same commands without `--no-restore` only after confirming that dependency restore is acceptable for the environment. Record exact results in `Progress`, `Surprises & Discoveries`, or `Outcomes & Retrospective` as appropriate.

Milestone 9 performs manual validation in Vixen. Start Vixen from the normal development build, open the Custom Prop Editor, choose `File > Import xModel`, and import every file in `docs/references/circlemodel`. For each file, verify that no unsupported model type error appears, the model group contains the expected number of nodes, generated circle groups exist, circle group counts match the ring sizes in wiring order, imported submodels appear for `circle-vendor.xmodel`, and the visual placement matches the filename's top/bottom, inside/outside, and direction. Record any visual mismatch in this plan before changing code.

## Concrete Steps

From `C:\Dev\Vixen`, begin by checking the worktree:

    git status --short

Read required instructions:

    Get-Content -LiteralPath .agents\PLANS.md
    Get-Content -LiteralPath docs\vix-3044-circle-model-import.md
    Get-Content -LiteralPath .agents\skills\dotnet-best-practices\SKILL.md

If updating Jira, also read:

    Get-Content -LiteralPath .agents\skills\jira\SKILL.md

Create or update focused tests in:

    src\Vixen.Tests\App\CustomPropEditor\Import\XLights\XModelCircleImportTests.cs

Expected pre-implementation behavior for the first circle tests is failure or null import because the current importer reports `circlemodel` as unsupported. After implementation, the tests should pass and should assert user-visible tree behavior rather than only internal parser state.

Implement internal parser and model changes under:

    src\Vixen.Modules\App\CustomPropEditor\Import\XLights\

Do not create a new project. Do not edit `Vixen.sln`. Do not read committed `.xmodel` examples from unit tests. Use project references only if a test genuinely needs a new project reference; importer tests should not need one because `Vixen.Tests` already references Custom Prop Editor.

Run focused validation:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore

A successful focused run should report all xLights importer tests passed, including new circle tests. The exact test count will depend on how many focused tests are added; record it in this plan when known.

## Validation and Acceptance

Automated validation succeeds when the focused xLights importer tests pass and demonstrate these behaviors. A standalone `<circlemodel>` imports into a `Prop` named `<name> {1}`. A wrapped `<model DisplayAs="Circle">` imports through the existing wrapper flow. The primary model group contains exactly `sum(LayerSizes)` light nodes with contiguous orders from `1` through `sum(LayerSizes)`. Generated circle groups exist under `<name> {1} - Circles`; `Circle 1` follows the first wired ring; and group membership follows node order. `StartSide="T"` and `StartSide="B"` produce different first-node coordinates in the expected vertical direction. `Dir="L"` and `Dir="R"` produce opposite second-node positions. `centerPercent` changes the inner-ring radius, missing or non-numeric values use zero, and out-of-range numeric values clamp. Imported `subModel type="ranges"` references generated light nodes by node order. Invalid `LayerSizes` and mismatched `PixelCount` return null and report clear model import errors. Existing custom model wrapper tests still pass.

Manual validation succeeds when each committed example in `docs/references/circlemodel` imports from the Custom Prop Editor without an unsupported model type error. The resulting prop should have a model group, generated circle groups, and a visual arrangement matching the file's intended top or bottom start, inside or outside start ring, and clockwise or counter-clockwise direction. The vendor example should also import its xLights submodels as Vixen grouping nodes.

## Idempotence and Recovery

All implementation steps are additive and can be repeated. Unit tests should create temporary `.xmodel` files and delete them in `finally` blocks, following `XModelImportHierarchyTests`. If a test leaves a temporary file behind, delete only the generated file after confirming its generated path. Do not modify files under `docs/references/circlemodel`; they are reference artifacts. Do not use `git reset --hard` or revert unrelated work. If a parser refactor breaks custom imports, stop after focused tests fail, record the failure in `Surprises & Discoveries`, and restore behavior by narrowing the refactor rather than removing existing wrapper support.

## Artifacts and Notes

The minimal wrapped circle XML used in tests can look like this:

    <models type="exported">
        <model name="Circle" DisplayAs="Circle" LayerSizes="4" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="2" PixelCount="4" />
    </models>

A minimal standalone circle XML used in tests can look like this:

    <circlemodel name="Spinner" DisplayAs="Circle" LayerSizes="3,4,5" InsideOut="0" StartSide="B" Dir="L" centerPercent="40" PixelSize="1" />

A minimal circle submodel test can add:

    <subModel name="FirstTwo" type="ranges" layout="" line0="1-2" />

The expected node order for `LayerSizes="3,4,5"` with `InsideOut="0"` is first wired outside ring with five nodes, then the middle ring with four nodes, then the inside ring with three nodes. Therefore generated `Circle 1` contains node orders `1-5`, `Circle 2` contains `6-9`, and `Circle 3` contains `10-12`. With `InsideOut="1"`, generated `Circle 1` contains the inside ring first.

## Interfaces and Dependencies

Keep public dependencies unchanged. Continue to use `System.Xml.Linq.XDocument` and `XElement` for XML parsing, Catel `IMessageService` for user-facing import errors, NLog for structured logging, and existing Custom Prop Editor model services for creating props and nodes.

Internal names may be adjusted during implementation, but the end state should include these responsibilities:

    internal interface IXModelElementParser
    {
        bool CanImport(XElement modelElement);
        string ResolveModelType(XElement modelElement);
        Task<XModelParsedModel> ParseAsync(XElement modelElement);
    }

    internal sealed class CustomXModelElementParser : IXModelElementParser
    {
        // Parses standalone custommodel and wrapped model DisplayAs="Custom".
    }

    internal sealed class CircleXModelElementParser : IXModelElementParser
    {
        // Parses standalone circlemodel and wrapped model DisplayAs="Circle".
    }

    internal sealed class XModelParsedModel
    {
        // Carries name, pixel size, string metadata, ModelNode dictionary,
        // imported submodels, faces, states, and generated grouping definitions.
    }

    internal sealed class XModelGeneratedGroup
    {
        // Carries generated parent/child group names and node orders for groups such as Circles.
    }

These types should stay internal unless a concrete cross-module consumer requires otherwise. If public or protected APIs are introduced, update XML documentation immediately and record the reason in the Decision Log.

## Revision Notes

2026-07-17 / Codex: Created the initial ExecPlan from `docs/vix-3044-circle-model-import.md` after reviewing `.agents/PLANS.md`, the current importer, current tests, local circle references, the existing wrapper-import plan, and the project design-pattern skill.
2026-07-17 / Codex: Moved the ExecPlan from `docs/plans/state` to `docs/plans/xmodel-import` because xModel import work is adjacent to State features but should be tracked separately.
2026-07-18 / Codex: Completed milestone 1 by updating Jira issue `VIX-3044` with the implementation requirements, design notes, acceptance criteria, automated test plan, manual validation plan, and plan path.
2026-07-18 / Codex: Completed milestone 2 by adding focused circle import tests and recording the expected pre-implementation failing baseline.
2026-07-20 / Codex: Completed milestone 3 by introducing internal parser/shared assembly structure while preserving the existing custom import behavior and the expected circle unsupported baseline.
2026-07-20 / Codex: Completed milestone 4 by adding circle attribute parsing and validation while leaving node generation and generated circle groups for the next milestones.
2026-07-20 / Codex: Completed milestone 5 by generating circle model nodes in xLights-compatible wiring order with deterministic normalized coordinates.
2026-07-20 / Codex: Completed milestone 6 by generating parent and per-ring circle groups from the stored wiring-order ring metadata.
2026-07-20 / Codex: Completed milestone 7 by validating wrapped circle selection in mixed wrappers and updating unsupported-model error text.
2026-07-20 / Codex: Refined circle coordinate generation to apply exported `ScaleX` and `ScaleY` before rounding, matching xLights' screen-location scale behavior more closely.

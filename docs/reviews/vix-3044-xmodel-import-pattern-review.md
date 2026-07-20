# VIX-3044 xModel Import Pattern Review

## Scope

This review compares the current custom xModel and circle xModel import code after the circle implementation. The goal is to identify naming and organization changes that would make the next xLights model type easier to add consistently.

Reviewed files:

- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelImport.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/IXModelElementParser.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/XModelParsedModel.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CustomXModelElementParser.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CustomModel.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CustomModelParser.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CustomModelSourceResolver.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CircleXModelElementParser.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CircleXModelConfiguration.cs`
- `src/Vixen.Modules/App/CustomPropEditor/Import/XLights/CircleXModelRing.cs`

## Findings

### 1. `CustomModel` is now the shared intermediate model, not only the xLights Custom model

`CustomXModelElementParser` and `CircleXModelElementParser` both produce a `CustomModel`. `XModelImport` also treats `XModelParsedModel.CustomModel` as the common assembly input for all supported xModel types. That shape works, but the name is misleading now that circles and future non-custom xLights models also use it.

Recommended direction: rename `CustomModel` to a type that describes its role in the importer, such as `XModelImportModel`, `XModelAssemblyModel`, or `XModelPropModel`.

The strongest candidate is `XModelImportModel` because it reads naturally in both parser and assembly code:

```csharp
var importModel = parsedModel.ImportModel;
var propSize = CalculatePropSize(importModel);
await Assemble(parsedModel);
```

Follow-on rename:

- `XModelParsedModel.CustomModel` -> `ImportModel`
- local variables named `customModel` in circle parser -> `importModel`
- local variables named `cm` in `XModelImport` -> `importModel`

This is the highest-value alignment change because it removes the main conceptual mismatch future contributors will see.

### 2. The custom parser has split responsibilities, while the circle parser owns the full pipeline

The custom path is organized as:

- `CustomXModelElementParser` parses high-level attributes and coordinates child metadata.
- `CustomModelSourceResolver` chooses `CustomModelCompressed` or `CustomModel`.
- `CustomModelParser` parses coordinate definitions.
- `CustomModelParseResult` reports parse outcome.

The circle path is mostly contained in `CircleXModelElementParser`:

- XML shape detection
- required attribute parsing
- optional attribute defaulting
- ring definition generation
- coordinate generation
- generated group creation
- duplicate `Circles` subModel matching

This is acceptable for the first circle implementation, but it does not give future model types a clear pattern. A matrix, star, tree, or spinner parser would likely grow another large all-in-one parser.

Recommended direction: split circle into the same broad shape as custom:

- `CircleXModelConfigurationParser`: parse XML attributes and validate required values.
- `CircleXModelNodeGenerator`: turn `CircleXModelConfiguration` into `Dictionary<int, ModelNode>`.
- `CircleXModelGroupGenerator`: create generated groups and handle matching imported groups.
- `CircleXModelElementParser`: orchestrate those pieces and attach shared child metadata.

This keeps `CircleXModelElementParser` parallel to `CustomXModelElementParser`: it recognizes whether it can import an element, asks model-specific helpers to produce nodes/groups, imports shared children, and returns `XModelParsedModel`.

### 3. Parser names should consistently distinguish xLights model types from Vixen import intermediates

Current names mix xLights input concepts and Vixen assembly concepts:

- `CustomXModelElementParser` clearly means "parser for xLights Custom model elements."
- `CircleXModelElementParser` clearly means "parser for xLights Circle model elements."
- `CustomModelParser` sounds like it parses the shared `CustomModel`, but it actually parses xLights `CustomModel` and `CustomModelCompressed` coordinate attributes.
- `CircleXModelConfiguration` is clearly circle-specific and input-oriented.

Recommended direction:

- Keep `CustomXModelElementParser` and `CircleXModelElementParser`.
- Rename `CustomModelParser` to `CustomXModelNodeParser` or `CustomXModelCoordinateParser`.
- Rename `CustomModelSourceResolver` to `CustomXModelSourceResolver`.
- Rename `CustomModelParseResult` to `CustomXModelParseResult`.
- Rename `CustomModelSource` to `CustomXModelSource`.

This gives future types a repeatable naming pattern:

- `<Type>XModelElementParser`
- `<Type>XModelConfiguration` when the type has meaningful model-specific attributes.
- `<Type>XModelConfigurationParser` when attribute parsing is large enough to extract.
- `<Type>XModelNodeGenerator` or `<Type>XModelNodeParser` depending on whether nodes are generated from attributes or parsed from an xLights node list.
- `<Type>XModelGroupGenerator` when generated groups are type-specific.

### 4. `XModelParsedModel` contains a circle-specific property

`XModelParsedModel.CircleConfiguration` is currently only diagnostic/contextual. The common import assembly path does not need to know about circle configuration after generated groups are created.

Recommended direction: remove `CircleConfiguration` from `XModelParsedModel` unless another consumer needs it. Keep model-specific configuration inside the circle parser and generated group helper. If debugging or test access is needed later, expose it through model-specific internal tests rather than the shared parsed-model contract.

This keeps the shared contract model-type-neutral for future parsers.

### 5. Model type detection is split between shared metadata and individual parsers

`CustomXModelElementParser.CanImport` delegates to `XModelElementMetadata.IsCustomModelElement`. `CircleXModelElementParser.CanImport` performs its own element-name and `DisplayAs` checks inline.

Recommended direction: make this consistent. Either:

- add `XModelElementMetadata.IsCircleModelElement`, or
- add a generic helper such as `IsModelElement(modelElement, standaloneElementName, displayAs)`.

The generic helper scales better:

```csharp
internal static bool IsXModelTypeElement(XElement modelElement, string standaloneElementName, string displayAs)
{
	return ElementNameEquals(modelElement, standaloneElementName) ||
		ElementNameEquals(modelElement, "model") &&
		displayAs.Equals(GetAttributeValue(modelElement, "DisplayAs"), StringComparison.OrdinalIgnoreCase);
}
```

Then future parsers can implement:

```csharp
public bool CanImport(XElement modelElement)
{
	return XModelElementMetadata.IsXModelTypeElement(modelElement, "circlemodel", "Circle");
}
```

### 6. Shared model attribute parsing should be centralized

Both parser types read the same xLights attributes:

- `name`
- `PixelSize`
- `StringType`
- `StrandNames`
- `NodeNames`
- `ScaleX`
- `ScaleY`

Some of these are already handled by `XModelCoordinateScale`, but `PixelSize` and string metadata are duplicated or parser-specific. The circle parser has a robust `GetPixelSize` with warning behavior; the custom parser has inline parsing without warning.

Recommended direction: create a small shared value object/helper for common xModel attributes, for example `XModelCommonConfiguration`:

```csharp
internal sealed class XModelCommonConfiguration
{
	public string Name { get; init; }
	public int PixelSize { get; init; }
	public string StringType { get; init; }
	public string StrandNames { get; init; }
	public string NodeNames { get; init; }
}
```

Then add a parser/helper such as `XModelCommonConfigurationParser.FromModelElement(modelElement, logging)`. Both custom and circle parsers can populate the shared import model from that object. Future model types get consistent defaults and logging for common attributes.

### 7. `XModelImport` assembly code would benefit from a dedicated assembler before adding more model types

`XModelImport` currently owns wrapper selection, parser creation, prop creation, physical metadata fallback, and assembly of model nodes, submodels, faces, states, and generated groups. That is still manageable, but future model types will likely increase assembly options and generated-group behavior.

Recommended direction: after the naming cleanup, extract the assembly portion into an internal `XModelPropAssembler`:

- input: `XModelParsedModel`
- output: `Prop`
- responsibilities: prop sizing, primary model group, child metadata assembly, generated group assembly, physical node count

Then `XModelImport` stays focused on XML loading, wrapper selection, parser selection, and user-facing errors. This is a natural Strategy plus Assembler split: parsers are strategies for model-specific parsing; the assembler is the stable common output path.

## Suggested Refactor Sequence

Do this in small commits so behavior stays provable.

1. Rename the shared `CustomModel` intermediate to `XModelImportModel` and update `XModelParsedModel.CustomModel` to `ImportModel`.
2. Rename custom-specific helper types so they include `XModel` and describe coordinates/source parsing rather than the shared import model.
3. Add shared model type detection in `XModelElementMetadata` and update both custom and circle parsers to use it.
4. Add shared common-attribute parsing for `name`, `PixelSize`, string metadata, and possibly scale defaults.
5. Split circle parsing into configuration parsing, node generation, and group generation helpers.
6. Remove `CircleConfiguration` from `XModelParsedModel` if it remains unused outside circle parsing.
7. Extract `XModelPropAssembler` from `XModelImport` once the parsed model contract is type-neutral.

Run after each step:

```text
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~App.CustomPropEditor.Import.XLights" --no-restore
```

Run the broader project test after the full refactor:

```text
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --no-restore
```

## Recommended End Pattern

Future model types should follow this pattern:

- Add `<Type>XModelElementParser` implementing `IXModelElementParser`.
- Use `XModelElementMetadata` for standalone and wrapped element matching.
- Parse common attributes through a shared common-configuration helper.
- Keep model-specific XML parsing in `<Type>XModelConfigurationParser` when the model has more than a few attributes.
- Generate or parse nodes through `<Type>XModelNodeGenerator` or `<Type>XModelNodeParser`.
- Generate type-specific groups through `<Type>XModelGroupGenerator` when needed.
- Return only type-neutral data through `XModelParsedModel`.
- Let the common assembler create Vixen `Prop`, model nodes, submodels, faces, states, generated groups, and metadata.

This pattern keeps new model support additive: a new model type should mostly add new files and one parser registration entry, while shared import assembly remains stable.

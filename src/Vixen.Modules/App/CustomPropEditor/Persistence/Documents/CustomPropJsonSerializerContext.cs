using System.Text.Json.Serialization;

namespace VixenModules.App.CustomPropEditor.Persistence.Documents;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(PropPackageDocument))]
[JsonSerializable(typeof(PropDocument))]
[JsonSerializable(typeof(ElementDocument))]
[JsonSerializable(typeof(LightDocument))]
[JsonSerializable(typeof(ImageDocument))]
[JsonSerializable(typeof(VendorMetadataDocument))]
[JsonSerializable(typeof(PhysicalMetadataDocument))]
[JsonSerializable(typeof(InformationMetadataDocument))]
[JsonSerializable(typeof(FaceDefinitionDocument))]
[JsonSerializable(typeof(StateDefinitionDocument))]
[JsonSerializable(typeof(StateItemDocument))]
internal sealed partial class CustomPropJsonSerializerContext : JsonSerializerContext;

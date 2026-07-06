using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Vixen.Module;
using Vixen.Module.MixingFilter;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.ImportExport;

namespace VixenModules.Editor.LayerEditor.Services
{
	/// <summary>
	/// Provides the default `.v3l` layer export and import implementation.
	/// </summary>
	public sealed class LayerImportExportService : ILayerImportExportService
	{
		private static readonly JsonSerializerOptions DocumentSerializerOptions = new()
		{
			WriteIndented = true
		};

		private readonly ILayerEditorLayerService _layerService;
		private readonly ILayerMixingFilterResolver _filterResolver;

		/// <summary>
		/// Initializes a new instance of the <see cref="LayerImportExportService"/> class.
		/// </summary>
		/// <param name="layerService">The layer operation service used to add imported layers and generate unique names.</param>
		/// <param name="filterResolver">The resolver used to create layer mixing filter instances during import.</param>
		public LayerImportExportService(ILayerEditorLayerService layerService, ILayerMixingFilterResolver filterResolver)
		{
			ArgumentNullException.ThrowIfNull(layerService);
			ArgumentNullException.ThrowIfNull(filterResolver);

			_layerService = layerService;
			_filterResolver = filterResolver;
		}

		/// <inheritdoc />
		public async Task ExportAsync(SequenceLayers layers, string filePath, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(layers);
			ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

			var standardLayers = layers.Layers.Where(layer => !layers.IsDefaultLayer(layer)).ToList();

			var document = new LayerExportDocument
			{
				ExportedUtc = DateTime.UtcNow,
				Layers = standardLayers.Select((layer, index) => new LayerExportRecord
				{
					Name = layer.LayerName,
					Order = index,
					FilterTypeId = layer.FilterTypeId,
					FilterName = layer.FilterName,
					FilterDataType = DescribeFilterDataType(layer.LayerMixingFilter?.ModuleData),
					FilterData = SerializeFilterData(layer.LayerMixingFilter?.ModuleData)
				}).ToList()
			};

			var documentNode = JsonSerializer.SerializeToNode(document, DocumentSerializerOptions);
			RemoveDefaultOptionalFilterDataProperties(documentNode);

			var json = documentNode!.ToJsonString(DocumentSerializerOptions);
			await File.WriteAllTextAsync(filePath, json, cancellationToken);
		}

		/// <inheritdoc />
		public async Task<LayerImportPlan> ReadImportPlanAsync(string filePath, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

			string json;
			try
			{
				json = await File.ReadAllTextAsync(filePath, cancellationToken);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				return LayerImportPlan.Failed($"The file could not be read: {ex.Message}");
			}

			LayerExportDocument document;
			try
			{
				document = JsonSerializer.Deserialize<LayerExportDocument>(json, DocumentSerializerOptions);
			}
			catch (JsonException ex)
			{
				return LayerImportPlan.Failed($"The file is not valid JSON: {ex.Message}");
			}

			if (document is null)
			{
				return LayerImportPlan.Failed("The file does not contain a layer export document.");
			}

			if (!string.Equals(document.Format, LayerExportDocument.CurrentFormat, StringComparison.Ordinal))
			{
				return LayerImportPlan.Failed($"'{document.Format}' is not a supported layer export format.");
			}

			if (document.Version != LayerExportDocument.CurrentVersion)
			{
				return LayerImportPlan.Failed($"Layer export version {document.Version} is not supported.");
			}

			if (document.Layers is null)
			{
				return LayerImportPlan.Failed("The file does not contain any layer records.");
			}

			var importable = new List<LayerImportEntry>();
			var skipped = new List<LayerImportSkippedRecord>();

			foreach (var record in document.Layers.OrderBy(record => record.Order))
			{
				// A default layer never has an attached mixing filter, so its FilterTypeId is always Guid.Empty.
				// Ignore any such record instead of treating it as a file-level failure or a skipped layer.
				if (record.FilterTypeId == Guid.Empty)
				{
					continue;
				}

				if (string.IsNullOrWhiteSpace(record.Name))
				{
					return LayerImportPlan.Failed("A layer record is missing its required name.");
				}

				var filterInstance = _filterResolver.Resolve(record.FilterTypeId);
				if (filterInstance is null)
				{
					skipped.Add(new LayerImportSkippedRecord(record.Name,
						$"The '{record.FilterName}' mixing filter ({record.FilterTypeId}) is not installed."));
					continue;
				}

				if (!TryRestoreFilterData(filterInstance, record.FilterData, out var restoreError))
				{
					skipped.Add(new LayerImportSkippedRecord(record.Name, restoreError));
					continue;
				}

				importable.Add(new LayerImportEntry(record.Name, record.Order, filterInstance));
			}

			return LayerImportPlan.Succeeded(importable, skipped);
		}

		/// <inheritdoc />
		public LayerImportResult Import(SequenceLayers layers, LayerImportPlan plan)
		{
			ArgumentNullException.ThrowIfNull(layers);
			ArgumentNullException.ThrowIfNull(plan);

			if (!plan.IsValid || plan.ImportableLayers.Count == 0)
			{
				return new LayerImportResult(0, plan.SkippedLayers);
			}

			var orderedEntries = plan.ImportableLayers.OrderBy(entry => entry.Order).ToList();

			// SequenceLayers.AddLayer inserts at index 0, so entries must be added from bottom to top
			// of the exported order to preserve that order once they sit above the existing layers.
			foreach (var entry in Enumerable.Reverse(orderedEntries))
			{
				var addedLayer = _layerService.AddLayer(layers, entry.LayerMixingFilter);
				addedLayer.LayerName = _layerService.CreateUniqueLayerName(
					layers.Layers.Where(existingLayer => !ReferenceEquals(existingLayer, addedLayer)), entry.Name);
			}

			return new LayerImportResult(orderedEntries.Count, plan.SkippedLayers);
		}

		private static void RemoveDefaultOptionalFilterDataProperties(JsonNode documentNode)
		{
			if (documentNode is not JsonObject documentObject || documentObject["layers"] is not JsonArray layersArray)
			{
				return;
			}

			foreach (var layerNode in layersArray)
			{
				if (layerNode is not JsonObject layerObject)
				{
					continue;
				}

				// A layer whose filter has no module data has an empty FilterDataType and a JSON-null
				// FilterData. Omitting both keeps the exported file focused on data the import path
				// actually uses instead of writing out placeholder defaults for every plain layer.
				if (layerObject["filterDataType"] is JsonValue filterDataTypeValue && filterDataTypeValue.GetValue<string>().Length == 0)
				{
					layerObject.Remove("filterDataType");
				}

				if (layerObject.ContainsKey("filterData") && layerObject["filterData"] is null)
				{
					layerObject.Remove("filterData");
				}
			}
		}

		private static string DescribeFilterDataType(IModuleDataModel dataModel)
		{
			if (dataModel is null)
			{
				return string.Empty;
			}

			var type = dataModel.GetType();
			return $"{type.FullName}, {type.Assembly.GetName().Name}";
		}

		private static JsonElement SerializeFilterData(IModuleDataModel dataModel)
		{
			if (dataModel is null)
			{
				return JsonDocument.Parse("null").RootElement.Clone();
			}

			var serializer = new DataContractJsonSerializer(dataModel.GetType());
			using var stream = new MemoryStream();
			serializer.WriteObject(stream, dataModel);
			stream.Position = 0;

			// ModuleTypeId and ModuleInstanceId come from ModuleDataModelBase and are not meaningful
			// outside the live module system; the filter type and a fresh instance ID are already
			// tracked by LayerExportRecord.FilterTypeId and the resolved filter instance on import.
			var node = JsonNode.Parse(stream);
			if (node is JsonObject jsonObject)
			{
				jsonObject.Remove(nameof(IModuleDataModel.ModuleTypeId));
				jsonObject.Remove(nameof(IModuleDataModel.ModuleInstanceId));
			}

			return node.Deserialize<JsonElement>();
		}

		private static bool TryRestoreFilterData(ILayerMixingFilterInstance filterInstance, JsonElement filterData, out string error)
		{
			error = string.Empty;
			var currentData = filterInstance.ModuleData;
			if (currentData is null)
			{
				return true;
			}

			if (filterData.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
			{
				error = "The layer record does not contain filter configuration data.";
				return false;
			}

			try
			{
				var serializer = new DataContractJsonSerializer(currentData.GetType());
				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(filterData.GetRawText()));
				var restored = (IModuleDataModel)serializer.ReadObject(stream)!;
				filterInstance.ModuleData = restored;
				return true;
			}
			catch (Exception ex) when (ex is SerializationException or FormatException or InvalidOperationException)
			{
				error = $"The filter configuration could not be restored: {ex.Message}";
				return false;
			}
		}
	}
}

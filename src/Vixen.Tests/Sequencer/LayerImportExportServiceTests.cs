using System.Text.Json;
using Moq;
using Vixen.Module;
using Vixen.Module.MixingFilter;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.ImportExport;
using VixenModules.Editor.LayerEditor.Services;
using VixenModules.LayerMixingFilter.LumaKey;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class LayerImportExportServiceTests
{
	private readonly LayerEditorLayerService _layerService = new();

	[Fact]
	public async Task ExportAsync_WritesNonDefaultLayersInTopToBottomOrderAsIndentedJson()
	{
		var layers = new SequenceLayers();
		var bottomTypeId = Guid.NewGuid();
		var topTypeId = Guid.NewGuid();

		var bottomLayer = _layerService.AddLayer(layers, CreateFilter(bottomTypeId, "Luma Key", new LumaKeyData { LowerLimit = 0.2, UpperLimit = 0.8 }));
		bottomLayer.LayerName = "Bottom";
		var topLayer = _layerService.AddLayer(layers, CreateFilter(topTypeId, "Luma Key", new LumaKeyData { LowerLimit = 0.3, UpperLimit = 0.7 }));
		topLayer.LayerName = "Top";

		var service = CreateService();
		var filePath = Path.GetTempFileName();
		try
		{
			await service.ExportAsync(layers, filePath, TestContext.Current.CancellationToken);
			var json = await File.ReadAllTextAsync(filePath, TestContext.Current.CancellationToken);

			Assert.Contains(Environment.NewLine, json);

			var document = JsonSerializer.Deserialize<LayerExportDocument>(json)!;
			Assert.Equal(LayerExportDocument.CurrentFormat, document.Format);
			Assert.Equal(LayerExportDocument.CurrentVersion, document.Version);
			Assert.Equal(2, document.Layers.Count);
			Assert.Equal("Top", document.Layers[0].Name);
			Assert.Equal(0, document.Layers[0].Order);
			Assert.Equal(topTypeId, document.Layers[0].FilterTypeId);
			Assert.Equal("Bottom", document.Layers[1].Name);
			Assert.Equal(1, document.Layers[1].Order);
			Assert.DoesNotContain(document.Layers, record => record.FilterTypeId == Guid.Empty);
			Assert.Equal("Luma Key", document.Layers[0].FilterName);
			Assert.Equal(0.3, document.Layers[0].FilterData.GetProperty("LowerLimit").GetDouble());
			Assert.Equal(0.7, document.Layers[0].FilterData.GetProperty("UpperLimit").GetDouble());

			foreach (var record in document.Layers)
			{
				Assert.False(record.FilterData.TryGetProperty("ModuleTypeId", out _));
				Assert.False(record.FilterData.TryGetProperty("ModuleInstanceId", out _));
			}
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task ExportAsync_OmitsFilterDataTypeAndFilterDataWhenFilterHasNoModuleData()
	{
		var layers = new SequenceLayers();
		var layer = _layerService.AddLayer(layers, CreateFilter(Guid.NewGuid(), "Mask and Fill", null));
		layer.LayerName = "Plain";

		var service = CreateService();
		var filePath = Path.GetTempFileName();
		try
		{
			await service.ExportAsync(layers, filePath, TestContext.Current.CancellationToken);
			var json = await File.ReadAllTextAsync(filePath, TestContext.Current.CancellationToken);

			using var document = JsonDocument.Parse(json);
			var layerElement = document.RootElement.GetProperty("layers")[0];

			Assert.False(layerElement.TryGetProperty("filterDataType", out _));
			Assert.False(layerElement.TryGetProperty("filterData", out _));

			var importDocument = JsonSerializer.Deserialize<LayerExportDocument>(json)!;
			Assert.Equal(string.Empty, importDocument.Layers[0].FilterDataType);
			Assert.Equal(JsonValueKind.Undefined, importDocument.Layers[0].FilterData.ValueKind);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task ReadImportPlanAsync_AllLayersValid_ProducesImportableEntriesWithRestoredModuleData()
	{
		var sourceLayers = new SequenceLayers();
		var typeIdA = Guid.NewGuid();
		var typeIdB = Guid.NewGuid();
		var layerA = _layerService.AddLayer(sourceLayers, CreateFilter(typeIdA, "Luma Key", new LumaKeyData { LowerLimit = 0.15, UpperLimit = 0.95 }));
		layerA.LayerName = "Bottom";
		var layerB = _layerService.AddLayer(sourceLayers, CreateFilter(typeIdB, "Luma Key", new LumaKeyData { LowerLimit = 0.25, UpperLimit = 0.55 }));
		layerB.LayerName = "Top";

		var filePath = Path.GetTempFileName();
		try
		{
			await CreateService().ExportAsync(sourceLayers, filePath, TestContext.Current.CancellationToken);

			var resolver = new Mock<ILayerMixingFilterResolver>();
			resolver.Setup(r => r.Resolve(typeIdA)).Returns(() => CreateFilter(typeIdA, "Luma Key", new LumaKeyData()));
			resolver.Setup(r => r.Resolve(typeIdB)).Returns(() => CreateFilter(typeIdB, "Luma Key", new LumaKeyData()));

			var service = new LayerImportExportService(_layerService, resolver.Object);
			var plan = await service.ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			Assert.True(plan.IsValid);
			Assert.False(plan.HasSkippedLayers);
			Assert.Equal(2, plan.ImportableLayers.Count);
			Assert.Equal("Top", plan.ImportableLayers[0].Name);
			Assert.Equal(0.25, ((LumaKeyData)plan.ImportableLayers[0].LayerMixingFilter.ModuleData).LowerLimit);
			Assert.Equal("Bottom", plan.ImportableLayers[1].Name);
			Assert.Equal(0.15, ((LumaKeyData)plan.ImportableLayers[1].LayerMixingFilter.ModuleData).LowerLimit);

			var targetLayers = new SequenceLayers();
			var existingLayer = _layerService.AddLayer(targetLayers, CreateFilter(Guid.NewGuid(), "Mask and Fill", null));
			existingLayer.LayerName = "Existing";

			var result = service.Import(targetLayers, plan);

			Assert.Equal(2, result.ImportedCount);
			Assert.Empty(result.SkippedLayers);
			Assert.Equal("Top", targetLayers.Layers[0].LayerName);
			Assert.Equal("Bottom", targetLayers.Layers[1].LayerName);
			Assert.Equal("Existing", targetLayers.Layers[2].LayerName);
			Assert.True(targetLayers.IsDefaultLayer(targetLayers.Layers[3]));
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task ReadImportPlanAsync_MissingFilterType_SkipsOnlyThatLayer()
	{
		var sourceLayers = new SequenceLayers();
		var missingTypeId = Guid.NewGuid();
		var installedTypeId = Guid.NewGuid();
		var missingLayer = _layerService.AddLayer(sourceLayers, CreateFilter(missingTypeId, "Old Filter", new LumaKeyData()));
		missingLayer.LayerName = "Missing";
		var installedLayer = _layerService.AddLayer(sourceLayers, CreateFilter(installedTypeId, "Luma Key", new LumaKeyData()));
		installedLayer.LayerName = "Installed";

		var filePath = Path.GetTempFileName();
		try
		{
			await CreateService().ExportAsync(sourceLayers, filePath, TestContext.Current.CancellationToken);

			var resolver = new Mock<ILayerMixingFilterResolver>();
			resolver.Setup(r => r.Resolve(missingTypeId)).Returns((ILayerMixingFilterInstance)null!);
			resolver.Setup(r => r.Resolve(installedTypeId)).Returns(() => CreateFilter(installedTypeId, "Luma Key", new LumaKeyData()));

			var service = new LayerImportExportService(_layerService, resolver.Object);
			var plan = await service.ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			Assert.True(plan.IsValid);
			Assert.True(plan.HasSkippedLayers);
			Assert.Single(plan.SkippedLayers);
			Assert.Equal("Missing", plan.SkippedLayers[0].Name);
			Assert.Single(plan.ImportableLayers);
			Assert.Equal("Installed", plan.ImportableLayers[0].Name);

			var targetLayers = new SequenceLayers();
			var result = service.Import(targetLayers, plan);

			Assert.Equal(1, result.ImportedCount);
			Assert.Single(result.SkippedLayers);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task ReadImportPlanAsync_IgnoresHandEditedDefaultLayerRecord()
	{
		var document = new LayerExportDocument
		{
			ExportedUtc = DateTime.UtcNow,
			Layers =
			[
				new LayerExportRecord
				{
					Name = "Default",
					Order = 0,
					FilterTypeId = Guid.Empty,
					FilterName = "None",
					FilterData = JsonDocument.Parse("null").RootElement.Clone()
				},
				new LayerExportRecord
				{
					Name = "Top",
					Order = 1,
					FilterTypeId = Guid.NewGuid(),
					FilterName = "Luma Key",
					FilterData = JsonDocument.Parse("null").RootElement.Clone()
				}
			]
		};
		var realTypeId = document.Layers[1].FilterTypeId;

		var filePath = Path.GetTempFileName();
		try
		{
			await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(document), TestContext.Current.CancellationToken);

			var resolver = new Mock<ILayerMixingFilterResolver>();
			resolver.Setup(r => r.Resolve(realTypeId)).Returns(() => CreateFilter(realTypeId, "Luma Key", null));

			var service = new LayerImportExportService(_layerService, resolver.Object);
			var plan = await service.ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			Assert.True(plan.IsValid);
			Assert.False(plan.HasSkippedLayers);
			Assert.Single(plan.ImportableLayers);
			Assert.Equal("Top", plan.ImportableLayers[0].Name);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task ReadImportPlanAsync_InvalidJson_ReturnsFailedPlanAndLeavesSequenceUnchanged()
	{
		var filePath = Path.GetTempFileName();
		try
		{
			await File.WriteAllTextAsync(filePath, "{ this is not valid json", TestContext.Current.CancellationToken);

			var service = CreateService();
			var plan = await service.ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			Assert.False(plan.IsValid);
			Assert.NotNull(plan.FailureReason);

			var layers = new SequenceLayers();
			var result = service.Import(layers, plan);

			Assert.Equal(0, result.ImportedCount);
			Assert.Single(layers.Layers);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task ReadImportPlanAsync_MissingRequiredName_ReturnsFailedPlanAndLeavesSequenceUnchanged()
	{
		var document = new LayerExportDocument
		{
			ExportedUtc = DateTime.UtcNow,
			Layers =
			[
				new LayerExportRecord
				{
					Name = "",
					Order = 0,
					FilterTypeId = Guid.NewGuid(),
					FilterName = "Luma Key",
					FilterData = JsonDocument.Parse("null").RootElement.Clone()
				}
			]
		};

		var filePath = Path.GetTempFileName();
		try
		{
			await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(document), TestContext.Current.CancellationToken);

			var service = CreateService();
			var plan = await service.ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			Assert.False(plan.IsValid);
			Assert.NotNull(plan.FailureReason);

			var layers = new SequenceLayers();
			var result = service.Import(layers, plan);

			Assert.Equal(0, result.ImportedCount);
			Assert.Single(layers.Layers);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task Import_AssignsNewLayerIdDistinctFromSourceLayer()
	{
		var sourceLayers = new SequenceLayers();
		var typeId = Guid.NewGuid();
		var sourceLayer = _layerService.AddLayer(sourceLayers, CreateFilter(typeId, "Luma Key", new LumaKeyData()));
		sourceLayer.LayerName = "Top";
		var sourceLayerId = sourceLayer.Id;

		var filePath = Path.GetTempFileName();
		try
		{
			await CreateService().ExportAsync(sourceLayers, filePath, TestContext.Current.CancellationToken);

			var resolver = new Mock<ILayerMixingFilterResolver>();
			resolver.Setup(r => r.Resolve(typeId)).Returns(() => CreateFilter(typeId, "Luma Key", new LumaKeyData()));

			var service = new LayerImportExportService(_layerService, resolver.Object);
			var plan = await service.ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			var targetLayers = new SequenceLayers();
			service.Import(targetLayers, plan);

			var importedLayer = targetLayers.Layers[0];
			Assert.NotEqual(Guid.Empty, importedLayer.Id);
			Assert.NotEqual(sourceLayerId, importedLayer.Id);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Theory]
	[InlineData("NotVixen3Layers", 1)]
	[InlineData(LayerExportDocument.CurrentFormat, 2)]
	public async Task ReadImportPlanAsync_UnsupportedFormatOrVersion_ReturnsFailedPlan(string format, int version)
	{
		var document = new LayerExportDocument
		{
			Format = format,
			Version = version,
			ExportedUtc = DateTime.UtcNow,
			Layers = []
		};

		var filePath = Path.GetTempFileName();
		try
		{
			await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(document), TestContext.Current.CancellationToken);

			var plan = await CreateService().ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			Assert.False(plan.IsValid);
			Assert.NotNull(plan.FailureReason);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	[Fact]
	public async Task Import_RenamesImportedLayerThatCollidesWithExistingLayerName()
	{
		var sourceLayers = new SequenceLayers();
		var typeId = Guid.NewGuid();
		var sourceLayer = _layerService.AddLayer(sourceLayers, CreateFilter(typeId, "Luma Key", new LumaKeyData()));
		sourceLayer.LayerName = "Top";

		var filePath = Path.GetTempFileName();
		try
		{
			await CreateService().ExportAsync(sourceLayers, filePath, TestContext.Current.CancellationToken);

			var resolver = new Mock<ILayerMixingFilterResolver>();
			resolver.Setup(r => r.Resolve(typeId)).Returns(() => CreateFilter(typeId, "Luma Key", new LumaKeyData()));

			var service = new LayerImportExportService(_layerService, resolver.Object);
			var plan = await service.ReadImportPlanAsync(filePath, TestContext.Current.CancellationToken);

			var targetLayers = new SequenceLayers();
			var existingLayer = _layerService.AddLayer(targetLayers, CreateFilter(Guid.NewGuid(), "Mask and Fill", null));
			existingLayer.LayerName = "Top";

			var result = service.Import(targetLayers, plan);

			Assert.Equal(1, result.ImportedCount);
			Assert.Equal("Top - 2", targetLayers.Layers[0].LayerName);
			Assert.Equal("Top", targetLayers.Layers[1].LayerName);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	private LayerImportExportService CreateService()
	{
		return new LayerImportExportService(_layerService, Mock.Of<ILayerMixingFilterResolver>());
	}

	private static ILayerMixingFilterInstance CreateFilter(Guid typeId, string typeName, IModuleDataModel? moduleData)
	{
		var descriptor = new Mock<IModuleDescriptor>();
		descriptor.SetupGet(d => d.TypeName).Returns(typeName);
		descriptor.SetupGet(d => d.TypeId).Returns(typeId);

		var filter = new Mock<ILayerMixingFilterInstance>();
		filter.SetupGet(f => f.Descriptor).Returns(descriptor.Object);
		filter.SetupGet(f => f.TypeId).Returns(typeId);
		filter.SetupGet(f => f.InstanceId).Returns(Guid.NewGuid());
		filter.SetupProperty(f => f.ModuleData, moduleData);

		return filter.Object;
	}
}

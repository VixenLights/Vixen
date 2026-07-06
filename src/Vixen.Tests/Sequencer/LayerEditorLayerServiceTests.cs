using Moq;
using Vixen.Module;
using Vixen.Module.MixingFilter;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.Services;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class LayerEditorLayerServiceTests
{
	private readonly LayerEditorLayerService _service = new();

	[Fact]
	public void AddLayer_CreatesStandardLayerAboveDefaultLayer()
	{
		var layers = new SequenceLayers();
		var filter = CreateFilter("Mask and Fill");

		var addedLayer = _service.AddLayer(layers, filter);

		Assert.Equal(2, layers.Count);
		Assert.Same(addedLayer, layers.Layers[0]);
		Assert.Equal(LayerType.Standard, addedLayer.Type);
		Assert.Same(filter, addedLayer.LayerMixingFilter);
		Assert.True(layers.IsDefaultLayer(layers.Layers[1]));
	}

	[Fact]
	public void AddLayer_GeneratesUniqueDefaultLayerName()
	{
		var layers = new SequenceLayers();

		var firstLayer = _service.AddLayer(layers, CreateFilter("First"));
		var secondLayer = _service.AddLayer(layers, CreateFilter("Second"));

		Assert.Equal("Default - 2", firstLayer.LayerName);
		Assert.Equal("Default - 3", secondLayer.LayerName);
	}

	[Fact]
	public void RemoveLayer_RemovesStandardLayer()
	{
		var layers = new SequenceLayers();
		var addedLayer = _service.AddLayer(layers, CreateFilter("Mask and Fill"));

		var removed = _service.RemoveLayer(layers, addedLayer);

		Assert.True(removed);
		Assert.Single(layers.Layers);
		Assert.True(layers.IsDefaultLayer(layers.Layers[0]));
	}

	[Fact]
	public void RemoveLayer_DoesNotRemoveDefaultLayer()
	{
		var layers = new SequenceLayers();
		var defaultLayer = layers.Layers[0];

		var removed = _service.RemoveLayer(layers, defaultLayer);

		Assert.False(removed);
		Assert.Single(layers.Layers);
		Assert.Same(defaultLayer, layers.Layers[0]);
	}

	[Fact]
	public void MoveLayer_MovesStandardLayer()
	{
		var layers = new SequenceLayers();
		var lowerLayer = _service.AddLayer(layers, CreateFilter("Lower"));
		var upperLayer = _service.AddLayer(layers, CreateFilter("Upper"));

		var moved = _service.MoveLayer(layers, lowerLayer, 0);

		Assert.True(moved);
		Assert.Same(lowerLayer, layers.Layers[0]);
		Assert.Same(upperLayer, layers.Layers[1]);
		Assert.True(layers.IsDefaultLayer(layers.Layers[2]));
	}

	[Fact]
	public void MoveLayer_DoesNotMoveDefaultLayer()
	{
		var layers = new SequenceLayers();
		var standardLayer = _service.AddLayer(layers, CreateFilter("Standard"));
		var defaultLayer = layers.Layers[1];

		var moved = _service.MoveLayer(layers, defaultLayer, 0);

		Assert.False(moved);
		Assert.Same(standardLayer, layers.Layers[0]);
		Assert.Same(defaultLayer, layers.Layers[1]);
	}

	[Fact]
	public void MoveLayer_DoesNotMoveStandardLayerToDefaultLayerIndex()
	{
		var layers = new SequenceLayers();
		var standardLayer = _service.AddLayer(layers, CreateFilter("Standard"));
		var defaultLayer = layers.Layers[1];

		var moved = _service.MoveLayer(layers, standardLayer, 1);

		Assert.False(moved);
		Assert.Same(standardLayer, layers.Layers[0]);
		Assert.Same(defaultLayer, layers.Layers[1]);
	}

	[Fact]
	public void ConfigureLayer_ReturnsTrueWhenSetupSucceeds()
	{
		var filter = CreateFilter("Configurable", hasSetup: true, setupResult: true);

		var configured = _service.ConfigureLayer(filter);

		Assert.True(configured);
		Mock.Get(filter).Verify(instance => instance.Setup(), Times.Once);
	}

	[Fact]
	public void ConfigureLayer_ReturnsFalseWhenFilterHasNoSetup()
	{
		var filter = CreateFilter("No Setup", hasSetup: false, setupResult: true);

		var configured = _service.ConfigureLayer(filter);

		Assert.False(configured);
		Mock.Get(filter).Verify(instance => instance.Setup(), Times.Never);
	}

	[Fact]
	public void CreateUniqueLayerName_UsesDashNumberConvention()
	{
		var layers = new[]
		{
			new StandardLayer("Mask and Fill"),
			new StandardLayer("Mask and Fill - 2")
		};

		var name = _service.CreateUniqueLayerName(layers, "Mask and Fill");

		Assert.Equal("Mask and Fill - 3", name);
	}

	[Fact]
	public void QuickRenameLayer_RenamesStandardLayerToFilterName()
	{
		var layers = new SequenceLayers();
		var layer = _service.AddLayer(layers, CreateFilter("Mask and Fill"));

		var renamed = _service.QuickRenameLayer(layers, layer);

		Assert.True(renamed);
		Assert.Equal("Mask and Fill", layer.LayerName);
	}

	[Fact]
	public void QuickRenameLayer_UsesUniqueNameForDuplicateFilterName()
	{
		var layers = new SequenceLayers();
		var existingLayer = _service.AddLayer(layers, CreateFilter("Mask and Fill"));
		existingLayer.LayerName = "Mask and Fill";
		var renamedLayer = _service.AddLayer(layers, CreateFilter("Mask and Fill"));

		var renamed = _service.QuickRenameLayer(layers, renamedLayer);

		Assert.True(renamed);
		Assert.Equal("Mask and Fill - 2", renamedLayer.LayerName);
	}

	[Fact]
	public void QuickRenameLayer_DoesNotRenameDefaultLayer()
	{
		var layers = new SequenceLayers();
		var defaultLayer = layers.Layers[0];

		var renamed = _service.QuickRenameLayer(layers, defaultLayer);

		Assert.False(renamed);
		Assert.Equal("Default", defaultLayer.LayerName);
	}

	[Fact]
	public void HasExportableLayers_ReturnsFalseWhenOnlyDefaultLayerExists()
	{
		var layers = new SequenceLayers();

		var hasExportableLayers = _service.HasExportableLayers(layers);

		Assert.False(hasExportableLayers);
	}

	[Fact]
	public void HasExportableLayers_ReturnsTrueWhenStandardLayerExists()
	{
		var layers = new SequenceLayers();
		_service.AddLayer(layers, CreateFilter("Mask and Fill"));

		var hasExportableLayers = _service.HasExportableLayers(layers);

		Assert.True(hasExportableLayers);
	}

	private static ILayerMixingFilterInstance CreateFilter(string typeName, bool hasSetup = false, bool setupResult = false)
	{
		var typeId = Guid.NewGuid();
		var descriptor = new Mock<IModuleDescriptor>();
		descriptor.SetupGet(moduleDescriptor => moduleDescriptor.TypeName).Returns(typeName);
		descriptor.SetupGet(moduleDescriptor => moduleDescriptor.TypeId).Returns(typeId);

		var filter = new Mock<ILayerMixingFilterInstance>();
		filter.SetupGet(instance => instance.Descriptor).Returns(descriptor.Object);
		filter.SetupGet(instance => instance.TypeId).Returns(typeId);
		filter.SetupGet(instance => instance.InstanceId).Returns(Guid.NewGuid());
		filter.SetupGet(instance => instance.HasSetup).Returns(hasSetup);
		filter.Setup(instance => instance.Setup()).Returns(setupResult);

		return filter.Object;
	}
}

using Moq;
using Vixen.Module;
using Vixen.Module.MixingFilter;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.Services;
using VixenModules.Editor.LayerEditor.ViewModels;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class LayerEditorViewModelTests
{
	[Fact]
	public void ExportLayersCommand_CannotExecute_WhenOnlyDefaultLayerExists()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out _);

		Assert.False(viewModel.ExportLayersCommand.CanExecute(null));
	}

	[Fact]
	public void ExportLayersCommand_CanExecute_AfterAddingStandardLayer()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out var layerService);

		layerService.AddLayer(layers, CreateFilter("Mask and Fill"));

		Assert.True(viewModel.ExportLayersCommand.CanExecute(null));
	}

	[Fact]
	public void ExportLayersCommand_RaisesCanExecuteChanged_WhenLayerAdded()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out var layerService);

		var raised = false;
		viewModel.ExportLayersCommand.CanExecuteChanged += (_, _) => raised = true;

		layerService.AddLayer(layers, CreateFilter("Mask and Fill"));

		Assert.True(raised);
	}

	[Fact]
	public void ExportLayersCommand_CannotExecute_AfterRemovingOnlyStandardLayer()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out var layerService);
		var addedLayer = layerService.AddLayer(layers, CreateFilter("Mask and Fill"));

		layerService.RemoveLayer(layers, addedLayer);

		Assert.False(viewModel.ExportLayersCommand.CanExecute(null));
	}

	[Fact]
	public void ImportLayersCommand_CanAlwaysExecute_EvenWithOnlyDefaultLayer()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out _);

		Assert.True(viewModel.ImportLayersCommand.CanExecute(null));
	}

	[Fact]
	public void QuickRenameLayerCommand_CannotExecute_ForDefaultLayer()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out _);
		var defaultLayer = layers.Layers[0];

		Assert.False(viewModel.QuickRenameLayerCommand.CanExecute(defaultLayer));
	}

	[Fact]
	public void QuickRenameLayerCommand_CanExecute_ForStandardLayerWithFilter()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out var layerService);
		var layer = layerService.AddLayer(layers, CreateFilter("Mask and Fill"));

		Assert.True(viewModel.QuickRenameLayerCommand.CanExecute(layer));
	}

	[Fact]
	public void ConfigureLayerCommand_RaisesLayerChanged_WhenSetupSucceeds()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out _);
		var filter = CreateFilter("Configurable", hasSetup: true, setupResult: true);

		var raised = false;
		viewModel.LayerChanged += (_, _) => raised = true;

		viewModel.ConfigureLayerCommand.Execute(filter);

		Assert.True(raised);
	}

	[Fact]
	public void ConfigureLayerCommand_DoesNotRaiseLayerChanged_WhenSetupFails()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out _);
		var filter = CreateFilter("Configurable", hasSetup: true, setupResult: false);

		var raised = false;
		viewModel.LayerChanged += (_, _) => raised = true;

		viewModel.ConfigureLayerCommand.Execute(filter);

		Assert.False(raised);
	}

	[Fact]
	public void ConfigureLayerCommand_CannotExecute_WhenFilterHasNoSetup()
	{
		var layers = new SequenceLayers();
		var viewModel = CreateViewModel(layers, out _);
		var filter = CreateFilter("No Setup", hasSetup: false, setupResult: true);

		Assert.False(viewModel.ConfigureLayerCommand.CanExecute(filter));
	}

	private static LayerEditorViewModel CreateViewModel(SequenceLayers layers, out LayerEditorLayerService layerService)
	{
		layerService = new LayerEditorLayerService();
		return new LayerEditorViewModel(layers, layerService, Mock.Of<ILayerImportExportService>());
	}

	private static ILayerMixingFilterInstance CreateFilter(string typeName, bool hasSetup = false, bool setupResult = false)
	{
		var typeId = Guid.NewGuid();
		var descriptor = new Mock<IModuleDescriptor>();
		descriptor.SetupGet(d => d.TypeName).Returns(typeName);
		descriptor.SetupGet(d => d.TypeId).Returns(typeId);

		var filter = new Mock<ILayerMixingFilterInstance>();
		filter.SetupGet(f => f.Descriptor).Returns(descriptor.Object);
		filter.SetupGet(f => f.TypeId).Returns(typeId);
		filter.SetupGet(f => f.InstanceId).Returns(Guid.NewGuid());
		filter.SetupGet(f => f.HasSetup).Returns(hasSetup);
		filter.Setup(f => f.Setup()).Returns(setupResult);

		return filter.Object;
	}
}

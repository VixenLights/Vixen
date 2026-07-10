using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.Preview.VixenPreview;
using VixenModules.Property.State;
using Xunit;

namespace Vixen.Tests.Preview.VixenPreview;

public sealed class PreviewCustomPropStateImportTests : IDisposable
{
	private readonly string _dataRootPath;

	public PreviewCustomPropStateImportTests()
	{
		_dataRootPath = Path.Combine(Path.GetTempPath(), $"VixenPreviewStateTests-{Guid.NewGuid():N}");
		Paths.DataRootPath = _dataRootPath;
		SetVixenSystemProperty(
			nameof(VixenSystem.Instrumentation),
			new global::Vixen.Sys.Instrumentation.Instrumentation());
		VixenSystem.LoadSystemConfig();
	}

	[Fact]
	public async Task CreateAsync_WithoutStateDefinitions_DoesNotAttachStateProperty()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);

		var rootNode = await CreatePreviewRootAsync(prop);

		Assert.False(FindNode(rootNode, model.Name).Properties.Contains(StateDescriptor.ModuleId));
		Assert.False(FindNode(rootNode, leaf.Name).Properties.Contains(StateDescriptor.ModuleId));
	}

	[Fact]
	public async Task CreateAsync_WithStateDefinitionModels_AttachesStatePropertyWithMappedAssignments()
	{
		var propertyId = Guid.NewGuid();
		var definitionId = Guid.NewGuid();
		var itemId = Guid.NewGuid();
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.StatePropertyId = propertyId;
		model.StateDefinitionModels =
		[
			new StateDefinitionModel
			{
				Id = definitionId,
				Name = "Wave",
				Description = "Arm positions",
				Items =
				[
					new StateItemModel
					{
						Id = itemId,
						Name = "Hand",
						Color = Color.Red,
						ElementModelIds = new ObservableCollection<Guid> { leaf.Id, leaf.Id, Guid.NewGuid() }
					}
				]
			}
		];

		var rootNode = await CreatePreviewRootAsync(prop);

		var modelNode = FindNode(rootNode, model.Name);
		var leafNode = FindNode(rootNode, leaf.Name);
		var state = AssertStateModule(modelNode);
		Assert.Equal(propertyId, state.Id);
		var definition = Assert.Single(state.StateDefinitions);
		Assert.Equal(definitionId, definition.Id);
		Assert.Equal("Wave", definition.Name);
		Assert.Equal("Arm positions", definition.Description);
		var item = Assert.Single(definition.Items);
		Assert.Equal(itemId, item.Id);
		Assert.Equal("Hand", item.Name);
		Assert.Equal(Color.Red.ToArgb(), item.Color.ToArgb());
		Assert.Equal([leafNode.Id], item.ElementNodeIds);
	}

	[Fact]
	public async Task CreateAsync_WhenNewAndLegacyStateExist_UsesStateDefinitionModels()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.StateDefinitionModels =
		[
			new StateDefinitionModel
			{
				Name = "New",
				Items =
				[
					new StateItemModel
					{
						Name = "Authored",
						Color = Color.Blue,
						ElementModelIds = new ObservableCollection<Guid> { leaf.Id }
					}
				]
			}
		];
		model.StateDefinitions.Add(new StateDefinition
		{
			StateDefinitionName = "Old Imported",
			Name = "Legacy Row",
			DefaultColor = Color.Red,
			ElementModelIds = [leaf.Id]
		});
		leaf.StateDefinition = new StateDefinition
		{
			StateDefinitionName = "Old Element",
			Name = "Legacy Element",
			DefaultColor = Color.Green
		};

		var rootNode = await CreatePreviewRootAsync(prop);

		var state = AssertStateModule(FindNode(rootNode, model.Name));
		var definition = Assert.Single(state.StateDefinitions);
		Assert.Equal("New", definition.Name);
		var item = Assert.Single(definition.Items);
		Assert.Equal("Authored", item.Name);
	}

	[Fact]
	public async Task CreateAsync_WithLegacyImportedRows_AttachesStatePropertyFallback()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.StateDefinitions.Add(new StateDefinition
		{
			StateDefinitionName = "Wave",
			Name = "Hand",
			DefaultColor = Color.Red,
			Index = 2,
			ElementModelIds = [leaf.Id]
		});

		var rootNode = await CreatePreviewRootAsync(prop);

		var state = AssertStateModule(FindNode(rootNode, model.Name));
		var definition = Assert.Single(state.StateDefinitions);
		Assert.Equal("Wave", definition.Name);
		var item = Assert.Single(definition.Items);
		Assert.Equal("Hand", item.Name);
		Assert.Equal([FindNode(rootNode, leaf.Name).Id], item.ElementNodeIds);
	}

	[Fact]
	public async Task CreateAsync_WithLegacyElementStateDefinition_AttachesStatePropertyFallback()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.StatePropertyId = Guid.NewGuid();
		leaf.StateDefinition = new StateDefinition
		{
			StateDefinitionName = "Wave",
			Name = "Hand",
			DefaultColor = Color.Green
		};

		var rootNode = await CreatePreviewRootAsync(prop);

		var state = AssertStateModule(FindNode(rootNode, model.Name));
		Assert.Equal(model.StatePropertyId, state.Id);
		var definition = Assert.Single(state.StateDefinitions);
		Assert.Equal("Wave", definition.Name);
		var item = Assert.Single(definition.Items);
		Assert.Equal("Hand", item.Name);
		Assert.Equal(Color.Green.ToArgb(), item.Color.ToArgb());
		Assert.Equal([FindNode(rootNode, leaf.Name).Id], item.ElementNodeIds);
	}

	public void Dispose()
	{
		if (Directory.Exists(_dataRootPath))
		{
			Directory.Delete(_dataRootPath, true);
		}
	}

	private static async Task<ElementNode> CreatePreviewRootAsync(Prop prop)
	{
		var builder = new PreviewCustomPropBuilder(prop, 1, null);
		return await builder.CreateAsync();
	}

	private static Prop CreatePropWithModel(out ElementModel model, out ElementModel leaf)
	{
		var prop = new Prop("Preview State Prop");
		model = new ElementModel("Preview State Model", prop.RootNode)
		{
			ModelType = ElementModelType.Model
		};
		leaf = new ElementModel("Preview State Leaf", model);
		model.AddChild(leaf);
		prop.RootNode.AddChild(model);
		return prop;
	}

	private static ElementNode FindNode(ElementNode rootNode, string name)
	{
		return rootNode
			.GetNodeEnumerator()
			.Single(node => node.Name == name);
	}

	private static StateModule AssertStateModule(ElementNode node)
	{
		return Assert.IsType<StateModule>(node.Properties.Get(StateDescriptor.ModuleId));
	}

	private static void SetVixenSystemProperty(string propertyName, object value)
	{
		var property = typeof(VixenSystem).GetProperty(
			propertyName,
			BindingFlags.Public | BindingFlags.Static);
		property!.SetValue(null, value);
	}
}

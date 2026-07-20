using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Model;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.Import.XLights;

[Collection("CustomPropEditor")]
public class XModelImportHierarchyTests
{
	[Fact]
	public async Task Import_WithStateInfo_CreatesModelChildAndAttachesStateDefinitions()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Santa Waving" parm1="300" parm2="300" PixelSize="1" CustomModelCompressed="1,0,0;2,1,0;3,2,0">
				<subModel name="Arm" type="ranges" layout="" line0="1-2" />
				<faceInfo Name="Face" Type="NodeRange" Eyes-Open="3" Eyes-Open-Color="#00FF00" />
				<stateInfo Name="Wave" Type="NodeRange" s1="1-2" s1-Name="Hand" s1-Color="#FF0000" />
			</custommodel>
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Equal("Santa Waving {1}", prop.Name);
		Assert.Equal("3", prop.PhysicalMetadata.NodeCount);

		var rootChildren = prop.RootNode.Children.Select(child => child.Name).ToList();
		Assert.Contains("Santa Waving {1} - Model", rootChildren);
		Assert.Contains("Santa Waving {1} - Arm", rootChildren);
		Assert.Contains("Santa Waving {1} - Faces ", rootChildren);
		Assert.Contains("Santa Waving {1} - States ", rootChildren);

		var modelGroup = prop.RootNode.Children.Single(child => child.Name == "Santa Waving {1} - Model");
		Assert.Equal(ElementModelType.Model, modelGroup.ModelType);
		Assert.Equal([1, 2, 3], modelGroup.Children.Select(child => child.Order).Order());

		Assert.Empty(modelGroup.StateDefinitions);
		var importedStateDefinition = Assert.Single(modelGroup.StateDefinitionModels);
		Assert.Equal("Wave", importedStateDefinition.Name);
		var importedStateItem = Assert.Single(importedStateDefinition.Items);
		Assert.Equal("Hand", importedStateItem.Name);
		Assert.Equal(System.Drawing.Color.Red.ToArgb(), importedStateItem.Color.ToArgb());
		Assert.Equal(
			modelGroup.Children.Where(child => child.Order is 1 or 2).Select(child => child.Id).Order(),
			importedStateItem.ElementModelIds.Order());

		var subModelGroup = prop.RootNode.Children.Single(child => child.Name == "Santa Waving {1} - Arm");
		Assert.Equal(ElementModelType.SubModel, subModelGroup.ModelType);

		var faceGroup = prop.RootNode.GetNodeEnumerator().Single(node => node.Name == "Santa Waving {1} - Face ");
		Assert.Equal(ElementModelType.FaceInfo, faceGroup.ModelType);

		var stateGroup = prop.RootNode.GetNodeEnumerator().Single(node => node.Name == "Santa Waving {1} - Wave ");
		Assert.Equal(ElementModelType.StateInfo, stateGroup.ModelType);

		var legacyStateGroups = prop.RootNode
			.GetNodeEnumerator()
			.Where(node => node.Name.Contains("Wave"))
			.ToList();
		Assert.All(legacyStateGroups, legacyStateGroup => Assert.Null(legacyStateGroup.StateDefinition));
	}

	[Fact]
	public async Task Import_WithoutStateInfo_CreatesModelChildWithoutStateDefinitions()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Snowman" parm1="300" parm2="300" PixelSize="1" CustomModelCompressed="1,0,0;2,1,0" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		var modelGroup = Assert.Single(prop.RootNode.Children);
		Assert.Equal("Snowman {1} - Model", modelGroup.Name);
		Assert.Equal(ElementModelType.Model, modelGroup.ModelType);
		Assert.Empty(modelGroup.StateDefinitions);
		Assert.Empty(modelGroup.StateDefinitionModels);
		Assert.DoesNotContain(prop.RootNode.Children, child => child.Name.Contains("States"));
		Assert.Equal([1, 2], modelGroup.Children.Select(child => child.Order).Order());
	}

	[Fact]
	public async Task Import_WithSmallModelBounds_UsesMinimumPropSize()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Tiny Snowman" parm1="10" parm2="10" PixelSize="3" CustomModelCompressed="1,0,0;2,4,5" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Equal(800, prop.Width);
		Assert.Equal(600, prop.Height);
		var modelGroup = Assert.Single(prop.RootNode.Children);
		Assert.All(modelGroup.Children, child => Assert.InRange(child.Lights.Single().X, 0, prop.Width));
		Assert.All(modelGroup.Children, child => Assert.InRange(child.Lights.Single().Y, 0, prop.Height));
	}

	[Fact]
	public async Task Import_WithLargeModelBounds_ExpandsPropToImportedNodes()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Large Snowman" parm1="10" parm2="10" PixelSize="3" CustomModelCompressed="1,0,0;2,700,900" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Equal(3611, prop.Width);
		Assert.Equal(2811, prop.Height);
		var modelGroup = Assert.Single(prop.RootNode.Children);
		Assert.All(modelGroup.Children, child => Assert.InRange(child.Lights.Single().X, 0, prop.Width));
		Assert.All(modelGroup.Children, child => Assert.InRange(child.Lights.Single().Y, 0, prop.Height));
	}

	[Fact]
	public async Task Import_WithCustomDimensions_UsesCustomWidthAndHeightForScale()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Modern Snowman" CustomWidth="300" CustomHeight="300" parm1="10" parm2="10" PixelSize="3" CustomModelCompressed="1,0,0;2,700,900" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Equal(911, prop.Width);
		Assert.Equal(711, prop.Height);
	}

	[Fact]
	public async Task Import_WithScaleXAndScaleY_AppliesScaleToCustomCoordinates()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Scaled Snowman" parm1="10" parm2="10" PixelSize="1" ScaleX="5" ScaleY="3" CustomModelCompressed="1,0,0;2,1,2" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		var modelGroup = Assert.Single(prop.RootNode.Children);
		var secondLight = modelGroup.Children.Single(child => child.Order == 2).Lights.Single();
		Assert.Equal(18, secondLight.X);
		Assert.Equal(11, secondLight.Y);
	}

	[Fact]
	public async Task Import_WithoutCustomDimensions_FallsBackToLegacyDimensionsForScale()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Legacy Snowman" parm1="300" parm2="300" PixelSize="3" CustomModelCompressed="1,0,0;2,700,900" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Equal(911, prop.Width);
		Assert.Equal(711, prop.Height);
	}

	[Fact]
	public async Task Import_WithDuplicateStateInfoNames_CreatesUniqueStateDefinitionModelNames()
	{
		// Arrange
		const string modelXml =
			"""
			<custommodel name="Santa Waving" parm1="300" parm2="300" PixelSize="1" CustomModelCompressed="1,0,0;2,1,0">
				<stateInfo Name="Wave" Type="NodeRange" s1="1" s1-Name="Left" s1-Color="#FF0000" />
				<stateInfo Name="Wave" Type="NodeRange" s1="2" s1-Name="Right" s1-Color="#00FF00" />
				<stateInfo Name="wave" Type="NodeRange" s1="1-2" s1-Name="Both" s1-Color="#0000FF" />
			</custommodel>
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		var modelGroup = prop.RootNode.Children.Single(child => child.Name == "Santa Waving {1} - Model");
		Assert.Equal(
			["Wave", "Wave - 2", "wave"],
			modelGroup.StateDefinitionModels.Select(definition => definition.Name).ToList());
	}

	[Fact]
	public async Task Import_WithSingleWrappedCustomModel_ImportsWithoutSelection()
	{
		// Arrange
		const string modelXml =
			"""
			<models type="exported">
				<model name="Wrapped Snowman" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,0,0;2,1,0" />
			</models>
			""";
		var selectionService = new FakeXModelSelectionService
		{
			ThrowOnSelection = true
		};

		// Act
		var prop = await ImportAsync(modelXml, selectionService);

		// Assert
		Assert.Equal("Wrapped Snowman {1}", prop.Name);
		var modelGroup = Assert.Single(prop.RootNode.Children);
		Assert.Equal("Wrapped Snowman {1} - Model", modelGroup.Name);
		Assert.Equal([1, 2], modelGroup.Children.Select(child => child.Order).Order());
		Assert.Equal(0, selectionService.CallCount);
	}

	[Fact]
	public async Task Import_WithMultipleWrappedCustomModels_ImportsSelectedModel()
	{
		// Arrange
		const string modelXml =
			"""
			<models type="exported">
				<model name="First" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,0,0" />
				<model name="Second" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,1,0;2,2,0" />
			</models>
			""";
		var selectionService = new FakeXModelSelectionService
		{
			SelectedIndex = 1
		};

		// Act
		var prop = await ImportAsync(modelXml, selectionService);

		// Assert
		Assert.Equal("Second {1}", prop.Name);
		Assert.Equal(1, selectionService.CallCount);
		Assert.Equal(["First", "Second"], selectionService.SeenModels.Select(model => model.DisplayName).ToList());
		var modelGroup = Assert.Single(prop.RootNode.Children);
		Assert.Equal([1, 2], modelGroup.Children.Select(child => child.Order).Order());
	}

	[Fact]
	public async Task Import_WithMultipleWrappedCustomModelsAndCanceledSelection_ReturnsNull()
	{
		// Arrange
		const string modelXml =
			"""
			<models type="exported">
				<model name="First" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,0,0" />
				<model name="Second" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,1,0" />
			</models>
			""";
		var selectionService = new FakeXModelSelectionService
		{
			CancelSelection = true
		};

		// Act
		var prop = await ImportAsync(modelXml, selectionService);

		// Assert
		Assert.Null(prop);
		Assert.Equal(1, selectionService.CallCount);
	}

	[Fact]
	public async Task Import_WithDuplicateWrappedModelNames_DisambiguatesSelectionNames()
	{
		// Arrange
		const string modelXml =
			"""
			<models type="exported">
				<model name="Duplicate" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,0,0" />
				<model name="Duplicate" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,1,0" />
			</models>
			""";
		var selectionService = new FakeXModelSelectionService
		{
			SelectedIndex = 1
		};

		// Act
		var prop = await ImportAsync(modelXml, selectionService);

		// Assert
		Assert.Equal("Duplicate {1}", prop.Name);
		Assert.Equal(["Duplicate (1)", "Duplicate (2)"], selectionService.SeenModels.Select(model => model.DisplayName).ToList());
	}

	[Fact]
	public async Task Import_WithWrappedModelGroup_IgnoresModelGroupElements()
	{
		// Arrange
		const string modelXml =
			"""
			<models type="exported">
				<model name="Grouped" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,0,0">
					<modelGroup name="Ignored Group" models="Grouped/SubModel" />
				</model>
			</models>
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Equal("Grouped {1}", prop.Name);
		Assert.DoesNotContain(prop.RootNode.GetNodeEnumerator(), node => node.Name.Contains("Ignored Group"));
		var modelGroup = Assert.Single(prop.RootNode.Children);
		Assert.Equal("Grouped {1} - Model", modelGroup.Name);
		Assert.Single(modelGroup.Children);
	}

	private static Task<Prop> ImportAsync(string modelXml)
	{
		var import = new XModelImport();
		return ImportAsync(modelXml, import);
	}

	private static Task<Prop> ImportAsync(string modelXml, IXModelSelectionService selectionService)
	{
		var import = new XModelImport
		{
			SelectionService = selectionService
		};
		return ImportAsync(modelXml, import);
	}

	private static async Task<Prop> ImportAsync(string modelXml, XModelImport import)
	{
		var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xmodel");
		await File.WriteAllTextAsync(filePath, modelXml);
		try
		{
			return await import.ImportAsync(filePath);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	private sealed class FakeXModelSelectionService : IXModelSelectionService
	{
		public int CallCount { get; private set; }

		public int SelectedIndex { get; set; }

		public bool CancelSelection { get; set; }

		public bool ThrowOnSelection { get; set; }

		public IReadOnlyList<XModelSelectionItem> SeenModels { get; private set; } = [];

		public Task<XModelSelectionItem> SelectModelAsync(IReadOnlyList<XModelSelectionItem> models)
		{
			if (ThrowOnSelection)
			{
				throw new InvalidOperationException("Selection service should not be called.");
			}

			CallCount++;
			SeenModels = models;
			return Task.FromResult(CancelSelection ? null! : models.Single(model => model.Index == SelectedIndex));
		}
	}
}

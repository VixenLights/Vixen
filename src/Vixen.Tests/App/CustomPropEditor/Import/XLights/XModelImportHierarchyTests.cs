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

	private static async Task<Prop> ImportAsync(string modelXml)
	{
		var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xmodel");
		await File.WriteAllTextAsync(filePath, modelXml);
		try
		{
			return await new XModelImport().ImportAsync(filePath);
		}
		finally
		{
			File.Delete(filePath);
		}
	}
}

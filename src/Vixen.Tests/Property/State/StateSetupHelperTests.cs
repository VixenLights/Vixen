using System.Reflection;
using Moq;
using Vixen.Module;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.Property.State;
using VixenModules.Property.State.Setup.Services;
using Xunit;

namespace Vixen.Tests.Property.State;

[Collection(StateMapperTestCollection.Name)]
public sealed class StateSetupHelperTests
{
	[Fact]
	public void Perform_NewPropertyAccepted_AttachesConfiguredModuleWithOriginalDataReference()
	{
		// Arrange
		InitializeModuleStore();
		var node = CreateNode();
		var stateModule = CreateStateModule();
		var expectedData = Assert.IsType<StateData>(stateModule.ModuleData);
		var dialogService = new RecordingStateMapperDialogService(true, data => data.Id = Guid.NewGuid());
		var helper = new StateSetupHelper(() => stateModule, dialogService);

		// Act
		var result = helper.Perform([node]);

		// Assert
		Assert.True(result);
		Assert.Same(expectedData, dialogService.Data);
		var attachedModule = node.Properties.Get(StateDescriptor.ModuleId);
		Assert.Same(stateModule, attachedModule);
		Assert.Same(expectedData, attachedModule!.ModuleData);
	}

	[Fact]
	public void Perform_NewPropertyCancelled_AttachesNothing()
	{
		// Arrange
		var node = CreateNode();
		var stateModule = CreateStateModule();
		var helper = new StateSetupHelper(() => stateModule, new RecordingStateMapperDialogService(false));

		// Act
		var result = helper.Perform([node]);

		// Assert
		Assert.False(result);
		Assert.False(node.Properties.Contains(StateDescriptor.ModuleId));
	}

	[Fact]
	public void Perform_ExistingPropertyAccepted_MutatesExistingDataInstance()
	{
		// Arrange
		var node = CreateNode();
		var stateModule = CreateStateModule();
		var data = Assert.IsType<StateData>(stateModule.ModuleData);
		AddPropertyWithoutModuleStore(node.Properties, stateModule);
		var helper = new StateSetupHelper(
			() => throw new InvalidOperationException("A new module must not be created."),
			new RecordingStateMapperDialogService(true, stateData => stateData.Id = Guid.NewGuid()));

		// Act
		var result = helper.Perform([node]);

		// Assert
		Assert.True(result);
		Assert.Same(data, node.Properties.Get(StateDescriptor.ModuleId)!.ModuleData);
		Assert.NotEqual(Guid.Empty, data.Id);
	}

	[Fact]
	public void Perform_ExistingPropertyCancelled_PreservesExistingData()
	{
		// Arrange
		var node = CreateNode();
		var stateModule = CreateStateModule();
		var data = Assert.IsType<StateData>(stateModule.ModuleData);
		var originalId = data.Id;
		AddPropertyWithoutModuleStore(node.Properties, stateModule);
		var helper = new StateSetupHelper(
			() => throw new InvalidOperationException("A new module must not be created."),
			new RecordingStateMapperDialogService(false, stateData => stateData.Id = Guid.NewGuid()));

		// Act
		var result = helper.Perform([node]);

		// Assert
		Assert.False(result);
		Assert.Same(data, node.Properties.Get(StateDescriptor.ModuleId)!.ModuleData);
		Assert.Equal(originalId, data.Id);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Perform_InvalidNewModuleCreation_ReturnsFalse(bool returnsInvalidData)
	{
		// Arrange
		var node = CreateNode();
		var dialogService = new RecordingStateMapperDialogService(true);
		var helper = new StateSetupHelper(
			returnsInvalidData ? CreateInvalidDataModule : () => null,
			dialogService);

		// Act
		var result = helper.Perform([node]);

		// Assert
		Assert.False(result);
		Assert.Null(dialogService.Data);
		Assert.False(node.Properties.Contains(StateDescriptor.ModuleId));
	}

	private static IElementNode CreateNode()
	{
		var node = new Mock<IElementNode>();
		var properties = new PropertyManager(node.Object);
		node.SetupGet(elementNode => elementNode.Properties).Returns(properties);

		return node.Object;
	}

	private static StateModule CreateStateModule()
	{
		return new StateModule { Descriptor = new StateDescriptor() };
	}

	private static IPropertyModuleInstance CreateInvalidDataModule()
	{
		var module = new Mock<IPropertyModuleInstance>();
		module.SetupGet(instance => instance.ModuleData).Returns(Mock.Of<IModuleDataModel>());

		return module.Object;
	}

	private static void AddPropertyWithoutModuleStore(PropertyManager properties, IPropertyModuleInstance property)
	{
		var itemsField = typeof(PropertyManager).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(itemsField);
		var items = Assert.IsType<Dictionary<Guid, IPropertyModuleInstance>>(itemsField.GetValue(properties));
		items[property.TypeId] = property;
	}

	private static void InitializeModuleStore()
	{
		var moduleStoreProperty = typeof(VixenSystem).GetProperty("ModuleStore", BindingFlags.Static | BindingFlags.NonPublic);
		Assert.NotNull(moduleStoreProperty);
		moduleStoreProperty.SetValue(null, new ModuleStore());
	}

	private sealed class RecordingStateMapperDialogService(bool result, Action<StateData>? configure = null) : IStateMapperDialogService
	{
		public StateData? Data { get; private set; }

		public bool Show(IElementNode node, StateData data)
		{
			Data = data;
			if (result)
			{
				configure?.Invoke(data);
			}

			return result;
		}
	}
}

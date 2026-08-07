using System.Collections.ObjectModel;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Persistence;
using VixenModules.App.CustomPropEditor.Persistence.Documents;

namespace Vixen.Tests.App.CustomPropEditor.Persistence;

public class PropDocumentMapperTests
{
	[Fact]
	public void RoundTrip_PreservesSharedElementIdentityAndStateData()
	{
		var prop = new Prop("Test") { Width = 640, Height = 480, Opacity = .75, Type = "Tree", CreatedBy = "Tester" };
		var first = new ElementModel("First");
		var second = new ElementModel("Second");
		var shared = new ElementModel("Shared", 4) { ModelType = ElementModelType.Model };
		prop.RootNode.AddChild(first);
		prop.RootNode.AddChild(second);
		first.AddChild(shared);
		second.AddChild(shared);
		first.AddParent(prop.RootNode);
		second.AddParent(prop.RootNode);
		shared.AddParent(first);
		shared.AddParent(second);
		shared.StateDefinitionModels.Add(new StateDefinitionModel
		{
			Name = "On",
			Items = new ObservableCollection<StateItemModel> { new() { Name = "Blue", ElementModelIds = new ObservableCollection<Guid> { shared.Id } } }
		});

		var mapper = new PropDocumentMapper();
		var document = mapper.ToDocument(prop);
		var hydrated = mapper.ToModel(document, prop.Image);

		var hydratedFirst = hydrated.RootNode.Children[0];
		var hydratedSecond = hydrated.RootNode.Children[1];
		Assert.Same(hydratedFirst.Children[0], hydratedSecond.Children[0]);
		Assert.Equal(prop.Id, hydrated.Id);
		Assert.Equal("Blue", hydratedFirst.Children[0].StateDefinitionModels[0].Items[0].Name);
		Assert.Empty(hydratedFirst.Children[0].StateDefinitions);
		Assert.Null(hydratedFirst.Children[0].StateDefinition);
	}

	[Fact]
	public void ToDocument_HandlesDeepGraphsWithoutRecursiveTraversal()
	{
		var prop = new Prop("Deep");
		var parent = prop.RootNode;
		for (var index = 0; index < 25; index++)
		{
			var child = new ElementModel($"Node {index}");
			parent.AddChild(child);
			child.AddParent(parent);
			parent = child;
		}

		var document = new PropDocumentMapper().ToDocument(prop);

		Assert.Equal(26, document.Elements.Count);
	}

	[Fact]
	public void Validate_RejectsCyclesAndNonFiniteCoordinates()
	{
		var first = Guid.NewGuid();
		var second = Guid.NewGuid();
		var document = ValidDocument(first) with
		{
			Elements =
			[
				new ElementDocument { Id = first, StatePropertyId = Guid.NewGuid(), LightSize = 1, ChildIds = [second] },
				new ElementDocument { Id = second, StatePropertyId = Guid.NewGuid(), LightSize = 1, ChildIds = [first] }
			]
		};

		Assert.Throws<PropPersistenceException>(() => PropDocumentValidator.Validate(document));
	}

	[Fact]
	public void Validate_RejectsInvalidStateReferences()
	{
		var elementId = Guid.NewGuid();
		var document = ValidDocument(elementId) with
		{
			Elements = [new ElementDocument
			{
				Id = elementId, StatePropertyId = Guid.NewGuid(), LightSize = 1,
				StateDefinitions = [new StateDefinitionDocument { Id = Guid.NewGuid(), Items = [new StateItemDocument { Id = Guid.NewGuid(), ElementIds = [Guid.NewGuid()] }] }]
			}]
		};

		Assert.Throws<PropPersistenceException>(() => PropDocumentValidator.Validate(document));
	}

	private static PropPackageDocument ValidDocument(Guid rootId) => new()
	{
		Prop = new PropDocument { Id = Guid.NewGuid(), Opacity = 1, Width = 10, Height = 10 },
		RootElementId = rootId,
		Elements = [new ElementDocument { Id = rootId, StatePropertyId = Guid.NewGuid(), LightSize = 1 }]
	};
}

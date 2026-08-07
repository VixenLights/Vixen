using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Media.Imaging;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Persistence.Documents;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed class PropDocumentMapper : IPropDocumentMapper
{
	public PropPackageDocument ToDocument(Prop prop)
	{
		if (prop == null) throw new ArgumentNullException(nameof(prop));
		var seen = new Dictionary<Guid, ElementModel>();
		var documents = new List<ElementDocument>();
		var pending = new Queue<ElementModel>();
		pending.Enqueue(prop.RootNode ?? throw new PropPersistenceException("The prop cannot be saved.", "The live prop has no root element."));
		while (pending.Count > 0)
		{
			var element = pending.Dequeue();
			if (element == null || element.Id == Guid.Empty) throw new PropPersistenceException("The prop cannot be saved.", "The live prop contains an empty element identifier.");
			if (seen.TryGetValue(element.Id, out var existing))
			{
				if (!ReferenceEquals(existing, element)) throw new PropPersistenceException("The prop cannot be saved.", "Distinct live elements share an identifier.");
				continue;
			}
			seen.Add(element.Id, element);
			var children = element.Children?.ToList() ?? throw new PropPersistenceException("The prop cannot be saved.", "An element has no children collection.");
			if (children.Any(child => child == null)) throw new PropPersistenceException("The prop cannot be saved.", "An element contains a null child.");
			documents.Add(ToElementDocument(element, children));
			foreach (var child in children) pending.Enqueue(child);
		}

		var document = new PropPackageDocument
		{
			Prop = new PropDocument
			{
				Id = prop.Id, Type = prop.Type ?? string.Empty, CreatedBy = prop.CreatedBy ?? string.Empty,
				CreationDate = prop.CreationDate, ModifiedDate = prop.ModifiedDate, Opacity = prop.Opacity, Width = prop.Width, Height = prop.Height,
				Vendor = ToVendor(prop.VendorMetadata), Physical = ToPhysical(prop.PhysicalMetadata), Information = new InformationMetadataDocument { Notes = prop.InformationMetadata?.Notes ?? string.Empty }
			},
			RootElementId = prop.RootNode.Id,
			Elements = documents
		};
		PropDocumentValidator.Validate(document);
		return document;
	}

	public Prop ToModel(PropPackageDocument document, BitmapSource image)
	{
		if (image == null) throw new ArgumentNullException(nameof(image));
		PropDocumentValidator.Validate(document);
		var elements = document.Elements.ToDictionary(item => item.Id, CreateElement);
		foreach (var source in document.Elements)
		{
			var target = elements[source.Id];
			foreach (var childId in source.ChildIds)
			{
				var child = elements[childId];
				target.Children.Add(child);
				child.Parents.Add(target.Id);
			}
			foreach (var light in source.Lights)
				target.Lights.Add(new Light { Id = light.Id, ParentModelId = target.Id, X = light.X, Y = light.Y, Z = light.Z, Size = light.Size });
			target.NormalizeStateModelData();
		}

		var prop = new Prop();
		var sourceProp = document.Prop;
		prop.Hydrate(sourceProp.Id, elements[document.RootElementId], image, sourceProp.Type, sourceProp.CreatedBy,
			sourceProp.CreationDate, sourceProp.ModifiedDate, sourceProp.Opacity, sourceProp.Width, sourceProp.Height,
			ToVendor(sourceProp.Vendor), ToPhysical(sourceProp.Physical), new InformationMetadata { Notes = sourceProp.Information.Notes });
		return prop;
	}

	private static ElementDocument ToElementDocument(ElementModel element, IReadOnlyCollection<ElementModel> children) => new()
	{
		Id = element.Id, StatePropertyId = element.StatePropertyId, Name = element.Name ?? string.Empty, Order = element.Order, LightSize = element.LightSize,
		ModelType = element.ModelType.ToString(), Face = new FaceDefinitionDocument { Component = element.FaceDefinition.FaceComponent.ToString(), DefaultColor = ColorText(element.FaceDefinition.DefaultColor) },
		StateDefinitions = (element.StateDefinitionModels ?? []).Select(ToStateDefinition).ToList(),
		ChildIds = children.Select(child => child.Id).ToList(),
		Lights = (element.Lights ?? []).Select(light => new LightDocument { Id = light.Id, X = light.X, Y = light.Y, Z = light.Z, Size = light.Size }).ToList()
	};

	private static ElementModel CreateElement(ElementDocument source) => new()
	{
		Id = source.Id, StatePropertyId = source.StatePropertyId, Name = source.Name, Order = source.Order, LightSize = source.LightSize,
		ModelType = ParseEnum<ElementModelType>(source.ModelType, "element model type"),
		FaceDefinition = new FaceDefinition { FaceComponent = ParseEnum<FaceComponent>(source.Face.Component, "face component"), DefaultColor = ParseColor(source.Face.DefaultColor) },
		StateDefinition = null, StateDefinitions = new ObservableCollection<StateDefinition>(),
		StateDefinitionModels = new ObservableCollection<StateDefinitionModel>(source.StateDefinitions.Select(ToStateDefinition))
	};

	private static StateDefinitionDocument ToStateDefinition(StateDefinitionModel source) => new()
	{
		Id = source.Id, Name = source.Name ?? string.Empty, Description = source.Description ?? string.Empty,
		Items = (source.Items ?? []).Select(item => new StateItemDocument { Id = item.Id, Name = item.Name ?? string.Empty, Color = ColorText(item.Color), ElementIds = (item.ElementModelIds ?? []).ToList() }).ToList()
	};

	private static StateDefinitionModel ToStateDefinition(StateDefinitionDocument source) => new()
	{
		Id = source.Id, Name = source.Name, Description = source.Description,
		Items = new ObservableCollection<StateItemModel>(source.Items.Select(item => new StateItemModel { Id = item.Id, Name = item.Name, Color = ParseColor(item.Color), ElementModelIds = new ObservableCollection<Guid>(item.ElementIds) }))
	};

	private static VendorMetadataDocument ToVendor(VendorMetadata source) => new() { Name = source?.Name ?? string.Empty, Website = source?.Website ?? string.Empty, Contact = source?.Contact ?? string.Empty, Email = source?.Email ?? string.Empty, Phone = source?.Phone ?? string.Empty };
	private static VendorMetadata ToVendor(VendorMetadataDocument source) => new() { Name = source.Name, Website = source.Website, Contact = source.Contact, Email = source.Email, Phone = source.Phone };
	private static PhysicalMetadataDocument ToPhysical(PhysicalMetadata source) => new() { Height = source?.Height ?? string.Empty, Width = source?.Width ?? string.Empty, Depth = source?.Depth ?? string.Empty, Material = source?.Material ?? string.Empty, NodeCount = source?.NodeCount ?? string.Empty, BulbType = source?.BulbType ?? string.Empty, ColorMode = source?.ColorMode.ToString() ?? nameof(ColorMode.FullColor) };
	private static PhysicalMetadata ToPhysical(PhysicalMetadataDocument source) => new() { Height = source.Height, Width = source.Width, Depth = source.Depth, Material = source.Material, NodeCount = source.NodeCount, BulbType = source.BulbType, ColorMode = ParseEnum<ColorMode>(source.ColorMode, "color mode") };
	private static string ColorText(Color color) => $"#{color.ToArgb():X8}";
	private static Color ParseColor(string value) => Color.FromArgb(int.Parse(value[1..], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
	private static T ParseEnum<T>(string value, string name) where T : struct, Enum => Enum.TryParse(value, out T result) && Enum.IsDefined(result) ? result : throw new PropPersistenceException("The prop package is invalid.", $"The {name} is invalid.");
}

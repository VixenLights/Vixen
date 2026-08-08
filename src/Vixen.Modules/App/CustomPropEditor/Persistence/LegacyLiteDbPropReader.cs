using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Persistence.Legacy.LiteDb5021;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed class LegacyLiteDbPropReader : IPropFileReader
{
	public Task<PropFileReadResult> ReadAsync(string path, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var raw = new LegacyLiteDbRawReaderPrototype().Read(path);
		var values = LegacyBsonDocumentReader.Read(raw.PropDocument);
		var image = DecodeImage(raw.Image);
		var root = values.TryGetValue("RootNode", out var rootValue) && rootValue is IReadOnlyDictionary<string, object> rootDocument
			? MapElement(rootDocument, new Dictionary<Guid, ElementModel>())
			: new ElementModel(GetString(values, "Name", "Legacy Custom Prop"));
		var prop = new Prop();
		prop.Hydrate(GetGuid(values, "_id", GetGuid(values, "Id", Guid.NewGuid())), root, image,
			GetString(values, "Type", string.Empty), GetString(values, "CreatedBy", string.Empty), DateTime.Now, DateTime.Now,
			GetDouble(values, "Opacity", 1), GetDouble(values, "Width", image.PixelWidth), GetDouble(values, "Height", image.PixelHeight),
			new VendorMetadata(), new PhysicalMetadata(), new InformationMetadata());
		prop.Name = GetString(values, "Name", root.Name);
		return Task.FromResult(new PropFileReadResult(null, image, PropFileSourceFormat.LegacyLiteDbV4, prop));
	}

	private static ElementModel MapElement(IReadOnlyDictionary<string, object> source, IDictionary<Guid, ElementModel> mapped)
	{
		var id = GetGuid(source, "_id", GetGuid(source, "Id", Guid.NewGuid()));
		if (mapped.TryGetValue(id, out var existing)) return existing;
		var element = new ElementModel(GetString(source, "Name", string.Empty))
		{
			Id = id, StatePropertyId = GetGuid(source, "StatePropertyId", Guid.NewGuid()), Order = (int)GetDouble(source, "Order", 0), LightSize = (int)GetDouble(source, "LightSize", ElementModel.DefaultLightSize),
			ModelType = ParseEnum<ElementModelType>(GetString(source, "ModelType", nameof(ElementModelType.None)), ElementModelType.None),
			FaceDefinition = MapFaceDefinition(source.GetValueOrDefault("FaceDefinition") as IReadOnlyDictionary<string, object>),
			Children = new ObservableCollection<ElementModel>(), Parents = new ObservableCollection<Guid>(), Lights = new ObservableCollection<Light>(), StateDefinitions = new ObservableCollection<StateDefinition>(), StateDefinitionModels = new ObservableCollection<StateDefinitionModel>()
		};
		mapped.Add(id, element);
		if (source.TryGetValue("Lights", out var lightsValue) && lightsValue is List<object> lights)
			foreach (var value in lights.OfType<IReadOnlyDictionary<string, object>>())
				element.Lights.Add(new Light { Id = GetGuid(value, "Id", Guid.NewGuid()), ParentModelId = id, X = GetDouble(value, "X", 0), Y = GetDouble(value, "Y", 0), Z = GetDouble(value, "Z", 0), Size = GetDouble(value, "Size", element.LightSize) });
		if (source.TryGetValue("Children", out var childrenValue) && childrenValue is List<object> children)
			foreach (var childValue in children.OfType<IReadOnlyDictionary<string, object>>())
			{
				var child = MapElement(childValue, mapped);
				element.Children.Add(child);
				if (!child.Parents.Contains(id)) child.Parents.Add(id);
			}
		MapAuthoredStates(source, element);
		if (!element.StateDefinitionModels.Any()) MapLegacyStates(source, element);
		return element;
	}

	private static void MapAuthoredStates(IReadOnlyDictionary<string, object> source, ElementModel element)
	{
		if (source.GetValueOrDefault("StateDefinitionModels") is not List<object> definitions) return;
		foreach (var definition in definitions.OfType<IReadOnlyDictionary<string, object>>())
		{
			var items = definition.GetValueOrDefault("Items") as List<object> ?? [];
			element.StateDefinitionModels.Add(new StateDefinitionModel
			{
				Id = GetGuid(definition, "_id", GetGuid(definition, "Id", Guid.NewGuid())),
				Name = GetString(definition, "Name", StateDefinitionModel.DefaultName),
				Description = GetString(definition, "Description", string.Empty),
				Items = new ObservableCollection<StateItemModel>(items.OfType<IReadOnlyDictionary<string, object>>().Select(item => new StateItemModel
				{
					Id = GetGuid(item, "_id", GetGuid(item, "Id", Guid.NewGuid())),
					Name = GetString(item, "Name", StateItemModel.DefaultName),
					Color = ParseLegacyColor(GetString(item, "Color", "255,255,255,255")),
					ElementModelIds = new ObservableCollection<Guid>((item.GetValueOrDefault("ElementModelIds") as List<object> ?? []).Select(value => value is byte[] { Length: 16 } bytes ? new Guid(bytes) : Guid.Empty).Where(value => value != Guid.Empty))
				}))
			});
		}
	}

	private static void MapLegacyStates(IReadOnlyDictionary<string, object> source, ElementModel element)
	{
		var legacyRows = new List<IReadOnlyDictionary<string, object>>();
		if (source.GetValueOrDefault("StateDefinition") is IReadOnlyDictionary<string, object> state) legacyRows.Add(state);
		if (source.GetValueOrDefault("StateDefinitions") is List<object> states) legacyRows.AddRange(states.OfType<IReadOnlyDictionary<string, object>>());
		foreach (var group in legacyRows.GroupBy(row => GetString(row, "StateDefinitionName", StateDefinitionModel.DefaultName)))
		{
			element.StateDefinitionModels.Add(new StateDefinitionModel
			{
				Name = group.Key,
				Items = new ObservableCollection<StateItemModel>(group.Select(row => new StateItemModel { Name = GetString(row, "Name", StateItemModel.DefaultName) }))
			});
		}
		element.StateDefinition = null;
		element.StateDefinitions.Clear();
	}

	private static BitmapSource DecodeImage(byte[] bytes)
	{
		using var stream = new MemoryStream(bytes, writable: false);
		var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
		if (decoder.Frames.Count != 1) throw new PropPersistenceException("The legacy prop image is invalid.", "The legacy image has an unexpected frame count.");
		var image = decoder.Frames[0];
		if (image.CanFreeze) image.Freeze();
		return image;
	}

	private static string GetString(IReadOnlyDictionary<string, object> values, string name, string fallback) => values?.GetValueOrDefault(name) as string ?? fallback;
	private static double GetDouble(IReadOnlyDictionary<string, object> values, string name, double fallback) => values.GetValueOrDefault(name) switch { int value => value, long value => value, double value when double.IsFinite(value) => value, _ => fallback };
	private static Guid GetGuid(IReadOnlyDictionary<string, object> values, string name, Guid fallback) => values.GetValueOrDefault(name) switch { byte[] { Length: 16 } value => new Guid(value), string value when Guid.TryParse(value, out var id) => id, _ => fallback };
	private static Color ParseLegacyColor(string value)
	{
		var components = value.Split(',');
		return components.Length == 4 && components.All(component => byte.TryParse(component, out _))
			? Color.FromArgb(byte.Parse(components[0]), byte.Parse(components[1]), byte.Parse(components[2]), byte.Parse(components[3]))
			: Color.White;
	}
	private static FaceDefinition MapFaceDefinition(IReadOnlyDictionary<string, object> source) => new()
	{
		FaceComponent = ParseEnum<FaceComponent>(GetString(source, "FaceComponent", nameof(FaceComponent.None)), FaceComponent.None),
		DefaultColor = ParseLegacyColor(GetString(source, "DefaultColor", "255,255,255,255"))
	};
	private static T ParseEnum<T>(string value, T fallback) where T : struct, Enum => Enum.TryParse(value, out T parsed) && Enum.IsDefined(parsed) ? parsed : fallback;
}

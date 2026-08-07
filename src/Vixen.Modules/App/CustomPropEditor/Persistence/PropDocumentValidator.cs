using VixenModules.App.CustomPropEditor.Persistence.Documents;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal static class PropDocumentValidator
{
	internal static void Validate(PropPackageDocument document)
	{
		if (document == null) Fail("The prop package is missing its document.");
		if (document.Format != PropPackageDocument.CurrentFormat || document.SchemaVersion != PropPackageDocument.CurrentSchemaVersion)
			Fail("The prop package uses an unsupported schema version.");
		if (document.Prop == null || document.Image == null || document.Elements == null)
			Fail("The prop package document is incomplete.");
		if (document.Prop.Id == Guid.Empty || document.RootElementId == Guid.Empty)
			Fail("The prop package contains an empty identifier.");
		if (document.Image.EntryName != "background.jpg" || document.Image.MediaType != "image/jpeg")
			Fail("The prop package image declaration is invalid.");
		if (!double.IsFinite(document.Prop.Opacity) || !double.IsFinite(document.Prop.Width) || !double.IsFinite(document.Prop.Height) ||
			document.Prop.Width < 1 || document.Prop.Height < 1)
			Fail("The prop package contains invalid canvas dimensions.");

		var elements = new Dictionary<Guid, ElementDocument>();
		var lights = new HashSet<Guid>();
		foreach (var element in document.Elements)
		{
			if (element == null || element.Id == Guid.Empty || !elements.TryAdd(element.Id, element)) Fail("The prop package contains duplicate element identifiers.");
			if (element.StatePropertyId == Guid.Empty || element.LightSize < 1 || element.ChildIds == null || element.Lights == null || element.StateDefinitions == null)
				Fail("The prop package contains an invalid element.");
			foreach (var light in element.Lights)
			{
				if (light == null || light.Id == Guid.Empty || !lights.Add(light.Id) || !double.IsFinite(light.X) || !double.IsFinite(light.Y) || !double.IsFinite(light.Z) || !double.IsFinite(light.Size) || light.Size <= 0)
					Fail("The prop package contains an invalid light.");
			}
		}
		if (!elements.ContainsKey(document.RootElementId)) Fail("The prop package root element is missing.");

		foreach (var element in elements.Values)
		{
			if (element.ChildIds.Distinct().Count() != element.ChildIds.Count || element.ChildIds.Any(id => !elements.ContainsKey(id)))
				Fail("The prop package contains an invalid element reference.");
			foreach (var definition in element.StateDefinitions)
			{
				if (definition == null || definition.Id == Guid.Empty || definition.Items == null) Fail("The prop package contains an invalid State definition.");
				foreach (var item in definition.Items)
					if (item == null || item.Id == Guid.Empty || item.ElementIds == null || item.ElementIds.Any(id => !elements.ContainsKey(id)) || !IsColor(item.Color))
						Fail("The prop package contains an invalid State item.");
			}
		}

		var visited = Visit(document.RootElementId, elements);
		if (visited.Count != elements.Count) Fail("The prop package contains unreachable elements.");
	}

	private static ISet<Guid> Visit(Guid rootId, IReadOnlyDictionary<Guid, ElementDocument> elements)
	{
		var visited = new HashSet<Guid>();
		var active = new HashSet<Guid>();
		var pending = new Stack<(Guid Id, bool Leaving)>();
		pending.Push((rootId, false));
		while (pending.Count > 0)
		{
			var current = pending.Pop();
			if (current.Leaving)
			{
				active.Remove(current.Id);
				continue;
			}
			if (!active.Add(current.Id)) Fail("The prop package contains an element cycle.");
			if (!visited.Add(current.Id))
			{
				active.Remove(current.Id);
				continue;
			}
			pending.Push((current.Id, true));
			foreach (var childId in elements[current.Id].ChildIds) pending.Push((childId, false));
		}
		return visited;
	}

	private static bool IsColor(string color) => color?.Length == 9 && color[0] == '#' && color.Skip(1).All(Uri.IsHexDigit);
	private static void Fail(string message) => throw new PropPersistenceException("The prop package is invalid.", message);
}

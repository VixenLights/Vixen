using System.Xml.Linq;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal static class XModelElementMetadata
	{
		public static bool IsCustomModelElement(XElement modelElement)
		{
			return modelElement.HasAttributes &&
				(ElementNameEquals(modelElement, "custommodel") ||
				ElementNameEquals(modelElement, "model") &&
				"Custom".Equals(GetAttributeValue(modelElement, "DisplayAs"), StringComparison.OrdinalIgnoreCase));
		}

		public static string ResolveModelType(XElement modelElement)
		{
			if (!ElementNameEquals(modelElement, "model"))
			{
				return modelElement.Name.LocalName;
			}

			var displayAs = GetAttributeValue(modelElement, "DisplayAs");
			return string.IsNullOrWhiteSpace(displayAs)
				? modelElement.Name.LocalName
				: $"{displayAs.ToLowerInvariant()}model";
		}

		public static bool ElementNameEquals(XElement element, string name)
		{
			return name.Equals(element.Name.LocalName, StringComparison.Ordinal);
		}

		public static string GetAttributeValue(XElement element, string name)
		{
			return element.Attribute(name)?.Value;
		}
	}
}

using System.Xml.Linq;
using NLog;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal static class XModelCommonConfigurationParser
	{
		internal static XModelCommonConfiguration FromModelElement(XElement modelElement, Logger logging)
		{
			return new XModelCommonConfiguration
			{
				Name = XModelElementMetadata.GetAttributeValue(modelElement, "name"),
				PixelSize = GetPixelSize(modelElement, logging),
				StringType = XModelElementMetadata.GetAttributeValue(modelElement, "StringType"),
				StrandNames = XModelElementMetadata.GetAttributeValue(modelElement, "StrandNames"),
				NodeNames = XModelElementMetadata.GetAttributeValue(modelElement, "NodeNames")
			};
		}

		private static int GetPixelSize(XElement modelElement, Logger logging)
		{
			var pixelSizeValue = XModelElementMetadata.GetAttributeValue(modelElement, "PixelSize");
			if (int.TryParse(pixelSizeValue, out var pixelSize) && pixelSize > 0)
			{
				return pixelSize;
			}

			if (!string.IsNullOrWhiteSpace(pixelSizeValue))
			{
				logging.Warn(
					"xModel {ModelName} has invalid PixelSize value {PixelSize}; using 1.",
					XModelElementMetadata.GetAttributeValue(modelElement, "name"),
					pixelSizeValue);
			}

			return 1;
		}
	}
}

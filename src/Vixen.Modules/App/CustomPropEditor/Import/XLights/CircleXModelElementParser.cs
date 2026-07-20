using System.Xml.Linq;
using NLog;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class CircleXModelElementParser(Func<string, string, Task> showModelErrorAsync) : IXModelElementParser
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly XModelChildElementImporter _childElementImporter = new();

		public bool CanImport(XElement modelElement)
		{
			return XModelElementMetadata.ElementNameEquals(modelElement, "circlemodel") ||
				XModelElementMetadata.ElementNameEquals(modelElement, "model") &&
				"Circle".Equals(XModelElementMetadata.GetAttributeValue(modelElement, "DisplayAs"), StringComparison.OrdinalIgnoreCase);
		}

		public string ResolveModelType(XElement modelElement)
		{
			return XModelElementMetadata.ResolveModelType(modelElement);
		}

		public async Task<XModelParsedModel> ParseAsync(XElement modelElement)
		{
			if (!TryParseConfiguration(modelElement, out var configuration, out var errorMessage))
			{
				await showModelErrorAsync(errorMessage, "Model import error");
				return null;
			}

			var customModel = new CustomModel(configuration.Name)
			{
				X = configuration.LayerSizes.Max(),
				Y = configuration.LayerSizes.Max(),
				PixelSize = configuration.PixelSize,
				StringType = XModelElementMetadata.GetAttributeValue(modelElement, "StringType"),
				StrandNames = XModelElementMetadata.GetAttributeValue(modelElement, "StrandNames"),
				NodeNames = XModelElementMetadata.GetAttributeValue(modelElement, "NodeNames")
			};

			_childElementImporter.ImportChildElements(customModel, modelElement);
			return new XModelParsedModel(customModel)
			{
				CircleConfiguration = configuration
			};
		}

		private static bool TryParseConfiguration(
			XElement modelElement,
			out CircleXModelConfiguration configuration,
			out string errorMessage)
		{
			configuration = null;
			var modelName = XModelElementMetadata.GetAttributeValue(modelElement, "name");

			if (!TryParseLayerSizes(modelElement, out var layerSizes))
			{
				errorMessage = $"Circle model '{modelName}' has an invalid LayerSizes value.";
				return false;
			}

			if (!TryParseInsideOut(modelElement, out var insideOut))
			{
				errorMessage = $"Circle model '{modelName}' has an invalid InsideOut value.";
				return false;
			}

			if (!TryParseStartSide(modelElement, out var startSide))
			{
				errorMessage = $"Circle model '{modelName}' has an invalid StartSide value.";
				return false;
			}

			if (!TryParseDirection(modelElement, out var direction))
			{
				errorMessage = $"Circle model '{modelName}' has an invalid Dir value.";
				return false;
			}

			var pixelCount = GetIntegerAttributeValue(modelElement, "PixelCount");
			var expectedPixelCount = layerSizes.Sum();
			if (pixelCount > 0 && pixelCount != expectedPixelCount)
			{
				errorMessage = $"Circle model '{modelName}' PixelCount does not match the sum of LayerSizes.";
				return false;
			}

			configuration = new CircleXModelConfiguration
			{
				Name = modelName,
				LayerSizes = layerSizes,
				InsideOut = insideOut,
				StartSide = startSide,
				Direction = direction,
				CenterPercent = GetCenterPercent(modelElement),
				PixelSize = GetPixelSize(modelElement),
				PixelCount = pixelCount,
				NumStrings = GetIntegerAttributeValue(modelElement, "NumStrings", "parm1"),
				NodesPerString = GetIntegerAttributeValue(modelElement, "NodesPerString", "parm2")
			};
			errorMessage = null;
			return true;
		}

		private static bool TryParseLayerSizes(XElement modelElement, out List<int> layerSizes)
		{
			layerSizes = [];
			var layerSizesValue = XModelElementMetadata.GetAttributeValue(modelElement, "LayerSizes");
			if (string.IsNullOrWhiteSpace(layerSizesValue))
			{
				return false;
			}

			foreach (var layerSizeValue in layerSizesValue.Split(','))
			{
				if (!int.TryParse(layerSizeValue.Trim(), out var layerSize) || layerSize <= 0)
				{
					return false;
				}

				layerSizes.Add(layerSize);
			}

			return layerSizes.Any();
		}

		private static bool TryParseInsideOut(XElement modelElement, out bool insideOut)
		{
			var insideOutValue = XModelElementMetadata.GetAttributeValue(modelElement, "InsideOut");
			insideOut = insideOutValue == "1";
			return insideOutValue is "0" or "1";
		}

		private static bool TryParseStartSide(XElement modelElement, out char startSide)
		{
			var startSideValue = XModelElementMetadata.GetAttributeValue(modelElement, "StartSide");
			startSide = string.IsNullOrWhiteSpace(startSideValue) ? '\0' : char.ToUpperInvariant(startSideValue.Trim()[0]);
			return startSide is 'B' or 'T';
		}

		private static bool TryParseDirection(XElement modelElement, out char direction)
		{
			var directionValue = XModelElementMetadata.GetAttributeValue(modelElement, "Dir");
			direction = string.IsNullOrWhiteSpace(directionValue) ? '\0' : char.ToUpperInvariant(directionValue.Trim()[0]);
			return direction is 'L' or 'R';
		}

		private static int GetCenterPercent(XElement modelElement)
		{
			var centerPercentValue = XModelElementMetadata.GetAttributeValue(modelElement, "centerPercent");
			if (string.IsNullOrWhiteSpace(centerPercentValue))
			{
				centerPercentValue = XModelElementMetadata.GetAttributeValue(modelElement, "parm3");
			}

			if (string.IsNullOrWhiteSpace(centerPercentValue))
			{
				return 0;
			}

			if (!int.TryParse(centerPercentValue, out var centerPercent))
			{
				Logging.Warn(
					"Circle model {ModelName} has non-numeric centerPercent value {CenterPercent}; using 0.",
					XModelElementMetadata.GetAttributeValue(modelElement, "name"),
					centerPercentValue);
				return 0;
			}

			return Math.Clamp(centerPercent, 0, 100);
		}

		private static int GetPixelSize(XElement modelElement)
		{
			var pixelSizeValue = XModelElementMetadata.GetAttributeValue(modelElement, "PixelSize");
			if (int.TryParse(pixelSizeValue, out var pixelSize) && pixelSize > 0)
			{
				return pixelSize;
			}

			if (!string.IsNullOrWhiteSpace(pixelSizeValue))
			{
				Logging.Warn(
					"Circle model {ModelName} has invalid PixelSize value {PixelSize}; using 1.",
					XModelElementMetadata.GetAttributeValue(modelElement, "name"),
					pixelSizeValue);
			}

			return 1;
		}

		private static int GetIntegerAttributeValue(XElement modelElement, string attributeName)
		{
			return GetIntegerAttributeValue(modelElement, attributeName, null);
		}

		private static int GetIntegerAttributeValue(XElement modelElement, string attributeName, string fallbackAttributeName)
		{
			var attributeValue = XModelElementMetadata.GetAttributeValue(modelElement, attributeName);
			if (int.TryParse(attributeValue, out var value))
			{
				return value;
			}

			if (string.IsNullOrWhiteSpace(fallbackAttributeName))
			{
				return 0;
			}

			var fallbackValue = XModelElementMetadata.GetAttributeValue(modelElement, fallbackAttributeName);
			return int.TryParse(fallbackValue, out value) ? value : 0;
		}
	}
}

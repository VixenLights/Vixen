using System.Xml.Linq;
using NLog;
using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Circle
{
	internal static class CircleXModelConfigurationParser
	{
		internal static bool TryParse(
			XElement modelElement,
			XModelCommonConfiguration commonConfiguration,
			Logger logging,
			out CircleXModelConfiguration configuration,
			out string errorMessage)
		{
			configuration = null;
			var modelName = commonConfiguration.Name;

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

			var coordinateScale = XModelCoordinateScale.FromModelElement(
				modelElement,
				XModelCoordinateScale.GetDefaultScale(layerSizes.Max(), layerSizes.Max()),
				logging);

			configuration = new CircleXModelConfiguration
			{
				Name = modelName,
				LayerSizes = layerSizes,
				InsideOut = insideOut,
				StartSide = startSide,
				Direction = direction,
				CenterPercent = GetCenterPercent(modelElement, logging),
				PixelSize = commonConfiguration.PixelSize,
				ScaleX = coordinateScale.X,
				ScaleY = coordinateScale.Y,
				PixelCount = pixelCount,
				NumStrings = GetIntegerAttributeValue(modelElement, "NumStrings", "parm1"),
				NodesPerString = GetIntegerAttributeValue(modelElement, "NodesPerString", "parm2"),
				Rings = CreateRingDefinitions(layerSizes, insideOut)
			};
			errorMessage = null;
			return true;
		}

		private static List<CircleXModelRing> CreateRingDefinitions(IReadOnlyList<int> layerSizes, bool insideOut)
		{
			var physicalLayerIndexes = insideOut
				? Enumerable.Range(0, layerSizes.Count)
				: Enumerable.Range(0, layerSizes.Count).Reverse();
			var rings = new List<CircleXModelRing>();
			var nextNodeOrder = 1;
			var circleNumber = 1;

			foreach (var physicalLayerIndex in physicalLayerIndexes)
			{
				var nodeOrders = Enumerable
					.Range(nextNodeOrder, layerSizes[physicalLayerIndex])
					.ToList();
				rings.Add(new CircleXModelRing
				{
					CircleNumber = circleNumber,
					PhysicalLayerIndex = physicalLayerIndex,
					NodeOrders = nodeOrders
				});

				nextNodeOrder += layerSizes[physicalLayerIndex];
				circleNumber++;
			}

			return rings;
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

		private static int GetCenterPercent(XElement modelElement, Logger logging)
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
				logging.Warn(
					"Circle model {ModelName} has non-numeric centerPercent value {CenterPercent}; using 0.",
					XModelElementMetadata.GetAttributeValue(modelElement, "name"),
					centerPercentValue);
				return 0;
			}

			return Math.Clamp(centerPercent, 0, 100);
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

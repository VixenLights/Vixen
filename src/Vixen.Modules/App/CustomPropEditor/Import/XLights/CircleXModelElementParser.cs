using System.Globalization;
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
			customModel.ModelNodes = CreateModelNodes(configuration);

			_childElementImporter.ImportChildElements(customModel, modelElement);
			var parsedModel = new XModelParsedModel(customModel)
			{
				CircleConfiguration = configuration
			};
			parsedModel.GeneratedGroups.Add(CreateGeneratedCircleGroups(configuration));
			return parsedModel;
		}

		private static XModelGeneratedGroup CreateGeneratedCircleGroups(CircleXModelConfiguration configuration)
		{
			return new XModelGeneratedGroup
			{
				Name = $"{configuration.Name} {{1}} - Circles",
				Children = configuration.Rings
					.OrderBy(ring => ring.CircleNumber)
					.Select(ring => new XModelGeneratedGroup
					{
						Name = $"{configuration.Name} {{1}} - Circle {ring.CircleNumber}",
						NodeOrders = ring.NodeOrders
					})
					.ToList()
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
				ScaleX = GetScale(modelElement, "ScaleX"),
				ScaleY = GetScale(modelElement, "ScaleY"),
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

		private static Dictionary<int, ModelNode> CreateModelNodes(CircleXModelConfiguration configuration)
		{
			var generatedNodes = new List<ModelNode>();
			var maxLights = configuration.LayerSizes.Max();
			var maxRadius = maxLights / 2.0;
			var minRadius = configuration.CenterPercent / 100.0 * maxRadius;

			foreach (var ring in configuration.Rings)
			{
				var nodeCount = configuration.LayerSizes[ring.PhysicalLayerIndex];
				var radius = GetLayerRadius(
					ring.PhysicalLayerIndex,
					configuration.LayerSizes.Count,
					minRadius,
					maxRadius);
				var startAngle = configuration.StartSide == 'T' ? Math.PI : 0;
				var direction = configuration.Direction == 'L' ? 1 : -1;
				var angleStep = 2 * Math.PI / nodeCount;

				for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
				{
					var angle = startAngle + direction * angleStep * nodeIndex;
					generatedNodes.Add(new ModelNode
					{
						Order = ring.NodeOrders[nodeIndex],
						X = (int)Math.Round(Math.Sin(angle) * radius * configuration.ScaleX, MidpointRounding.AwayFromZero),
						Y = (int)Math.Round(Math.Cos(angle) * radius * configuration.ScaleY, MidpointRounding.AwayFromZero)
					});
				}
			}

			NormalizeCoordinates(generatedNodes);
			return generatedNodes.ToDictionary(node => node.Order);
		}

		private static double GetLayerRadius(int physicalLayerIndex, int layerCount, double minRadius, double maxRadius)
		{
			if (layerCount == 1)
			{
				return maxRadius;
			}

			return minRadius + (maxRadius - minRadius) * physicalLayerIndex / (layerCount - 1);
		}

		private static void NormalizeCoordinates(IReadOnlyCollection<ModelNode> modelNodes)
		{
			if (!modelNodes.Any())
			{
				return;
			}

			var minX = modelNodes.Min(node => node.X);
			var minY = modelNodes.Min(node => node.Y);

			foreach (var modelNode in modelNodes)
			{
				modelNode.X -= minX;
				modelNode.Y -= minY;
			}
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

		private static double GetScale(XElement modelElement, string attributeName)
		{
			var scaleValue = XModelElementMetadata.GetAttributeValue(modelElement, attributeName);
			if (double.TryParse(scaleValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var scale) && scale > 0)
			{
				return scale;
			}

			if (!string.IsNullOrWhiteSpace(scaleValue))
			{
				Logging.Warn(
					"Circle model {ModelName} has invalid {ScaleAttribute} value {Scale}; using 1.",
					XModelElementMetadata.GetAttributeValue(modelElement, "name"),
					attributeName,
					scaleValue);
			}

			return 1.0;
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

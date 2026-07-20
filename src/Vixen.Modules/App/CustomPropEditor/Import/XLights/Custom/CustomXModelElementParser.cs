using System.Xml.Linq;
using NLog;
using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Custom
{
	internal sealed class CustomXModelElementParser(Func<string, string, Task> showModelErrorAsync) : IXModelElementParser
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly XModelChildElementImporter _childElementImporter = new();

		public bool CanImport(XElement modelElement)
		{
			return XModelElementMetadata.IsCustomModelElement(modelElement);
		}

		public string ResolveModelType(XElement modelElement)
		{
			return XModelElementMetadata.ResolveModelType(modelElement);
		}

		public async Task<XModelParsedModel> ParseAsync(XElement modelElement)
		{
			var commonConfiguration = XModelCommonConfigurationParser.FromModelElement(modelElement, Logging);
			var importModel = new XModelImportModel(commonConfiguration.Name)
			{
				X = GetModelDimension(modelElement, "CustomWidth", "parm1"),
				Y = GetModelDimension(modelElement, "CustomHeight", "parm2"),
				PixelSize = commonConfiguration.PixelSize,
				StringType = commonConfiguration.StringType,
				StrandNames = commonConfiguration.StrandNames,
				NodeNames = commonConfiguration.NodeNames
			};

			var compressedModel = XModelElementMetadata.GetAttributeValue(modelElement, "CustomModelCompressed");
			var customModelDefinition = XModelElementMetadata.GetAttributeValue(modelElement, "CustomModel");
			var coordinateScale = XModelCoordinateScale.FromModelElement(modelElement, importModel.Scale, Logging);
			if (!await TryResolveModelNodesAsync(importModel, compressedModel, customModelDefinition, coordinateScale))
			{
				return null;
			}

			_childElementImporter.ImportChildElements(importModel, modelElement);
			return new XModelParsedModel(importModel);
		}

		private static int GetModelDimension(XElement modelElement, string preferredAttributeName, string fallbackAttributeName)
		{
			return TryGetPositiveAttributeValue(modelElement, preferredAttributeName, out var preferredValue)
				? preferredValue
				: TryGetPositiveAttributeValue(modelElement, fallbackAttributeName, out var fallbackValue)
					? fallbackValue
					: 0;
		}

		private static bool TryGetPositiveAttributeValue(XElement modelElement, string attributeName, out int value)
		{
			return int.TryParse(XModelElementMetadata.GetAttributeValue(modelElement, attributeName), out value) && value > 0;
		}

		private async Task<bool> TryResolveModelNodesAsync(
			XModelImportModel importModel,
			string compressedModelDefinition,
			string modelDefinition,
			XModelCoordinateScale scale)
		{
			var result = CustomXModelSourceResolver.Resolve(
				compressedModelDefinition,
				modelDefinition,
				scale);
			if (result.Success)
			{
				if (result.Source == CustomXModelSource.CustomModel &&
					result.CompressedException != null)
				{
					Logging.Warn(
						result.CompressedException,
						"Unable to parse CustomModelCompressed for xModel {ModelName}. Falling back to CustomModel.",
						importModel.Name);
				}

				importModel.ModelNodes = result.ModelNodes;
				return true;
			}

			LogParseFailure(importModel.Name, result);
			await showModelErrorAsync(
				$"Unable to parse a valid CustomModel or CustomModelCompressed for model '{importModel.Name}'.",
				"Model import error");
			return false;
		}

		private static void LogParseFailure(string modelName, CustomXModelParseResult result)
		{
			if (result.CompressedException == null &&
				result.CustomModelException == null)
			{
				Logging.Error(
					"xModel {ModelName} does not contain a valid CustomModelCompressed or CustomModel definition.",
					modelName);
				return;
			}

			if (result.CompressedException != null)
			{
				Logging.Error(
					result.CompressedException,
					"Unable to parse CustomModelCompressed for xModel {ModelName}.",
					modelName);
			}

			if (result.CustomModelException != null)
			{
				Logging.Error(
					result.CustomModelException,
					"Unable to parse CustomModel for xModel {ModelName}.",
					modelName);
			}
		}
	}
}

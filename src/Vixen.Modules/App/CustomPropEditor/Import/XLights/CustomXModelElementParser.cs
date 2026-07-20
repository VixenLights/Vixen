using System.Xml.Linq;
using NLog;

namespace VixenModules.App.CustomPropEditor.Import.XLights
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
			var name = XModelElementMetadata.GetAttributeValue(modelElement, "name");
			var customModel = new CustomModel(name)
			{
				X = GetModelDimension(modelElement, "CustomWidth", "parm1"),
				Y = GetModelDimension(modelElement, "CustomHeight", "parm2")
			};

			if (int.TryParse(XModelElementMetadata.GetAttributeValue(modelElement, "PixelSize"), out var nodeSize) && nodeSize > 0)
			{
				customModel.PixelSize = nodeSize;
			}

			customModel.StringType = XModelElementMetadata.GetAttributeValue(modelElement, "StringType");
			customModel.StrandNames = XModelElementMetadata.GetAttributeValue(modelElement, "StrandNames");
			customModel.NodeNames = XModelElementMetadata.GetAttributeValue(modelElement, "NodeNames");

			var compressedModel = XModelElementMetadata.GetAttributeValue(modelElement, "CustomModelCompressed");
			var customModelDefinition = XModelElementMetadata.GetAttributeValue(modelElement, "CustomModel");
			if (!await TryResolveModelNodesAsync(customModel, compressedModel, customModelDefinition))
			{
				return null;
			}

			_childElementImporter.ImportChildElements(customModel, modelElement);
			return new XModelParsedModel(customModel);
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
			CustomModel customModel,
			string compressedModelDefinition,
			string modelDefinition)
		{
			var result = CustomModelSourceResolver.Resolve(
				compressedModelDefinition,
				modelDefinition,
				customModel.Scale);
			if (result.Success)
			{
				if (result.Source == CustomModelSource.CustomModel &&
					result.CompressedException != null)
				{
					Logging.Warn(
						result.CompressedException,
						"Unable to parse CustomModelCompressed for xModel {ModelName}. Falling back to CustomModel.",
						customModel.Name);
				}

				customModel.ModelNodes = result.ModelNodes;
				return true;
			}

			LogParseFailure(customModel.Name, result);
			await showModelErrorAsync(
				$"Unable to parse a valid CustomModel or CustomModelCompressed for model '{customModel.Name}'.",
				"Model import error");
			return false;
		}

		private static void LogParseFailure(string modelName, CustomModelParseResult result)
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

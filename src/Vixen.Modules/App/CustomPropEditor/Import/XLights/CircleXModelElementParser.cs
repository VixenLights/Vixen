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
			return XModelElementMetadata.IsXModelTypeElement(modelElement, "circlemodel", "Circle");
		}

		public string ResolveModelType(XElement modelElement)
		{
			return XModelElementMetadata.ResolveModelType(modelElement);
		}

		public async Task<XModelParsedModel> ParseAsync(XElement modelElement)
		{
			var commonConfiguration = XModelCommonConfigurationParser.FromModelElement(modelElement, Logging);
			if (!CircleXModelConfigurationParser.TryParse(
				modelElement,
				commonConfiguration,
				Logging,
				out var configuration,
				out var errorMessage))
			{
				await showModelErrorAsync(errorMessage, "Model import error");
				return null;
			}

			var importModel = new XModelImportModel(configuration.Name)
			{
				X = configuration.LayerSizes.Max(),
				Y = configuration.LayerSizes.Max(),
				PixelSize = commonConfiguration.PixelSize,
				StringType = commonConfiguration.StringType,
				StrandNames = commonConfiguration.StrandNames,
				NodeNames = commonConfiguration.NodeNames,
				ModelNodes = CircleXModelNodeGenerator.CreateModelNodes(configuration)
			};

			_childElementImporter.ImportChildElements(importModel, modelElement);
			var parsedModel = new XModelParsedModel(importModel);
			if (!CircleXModelGroupGenerator.HasMatchingImportedCirclesSubModel(importModel, configuration))
			{
				parsedModel.GeneratedGroups.Add(CircleXModelGroupGenerator.CreateGeneratedCircleGroups(configuration));
			}

			return parsedModel;
		}
	}
}

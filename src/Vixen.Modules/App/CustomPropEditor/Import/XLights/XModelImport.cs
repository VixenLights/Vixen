using Catel.IoC;
using Catel.Services;
using System.IO;
using System.Xml.Linq;
using VixenModules.App.CustomPropEditor.Import.XLights.Circle;
using VixenModules.App.CustomPropEditor.Import.XLights.Custom;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelImport : IModelImport
	{
		internal IXModelSelectionService SelectionService { private get; set; } = new XModelSelectionService();
		
		public async Task<Prop> ImportAsync(string filePath)
		{
			return await LoadModelFileAsync(filePath);
		}

		private async Task<Prop> LoadModelFileAsync(string filePath)
		{
			await using var fileStream = File.OpenRead(filePath);
			var document = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
			var root = document.Root;
			if (root == null)
			{
				await ShowModelErrorAsync("The xModel file does not contain a model definition.", "Model import error");
				return null;
			}

			var parser = GetModelParser(root);
			if (parser != null)
			{
				return await ImportModelElementAsync(root, parser);
			}

			if (XModelElementMetadata.ElementNameEquals(root, "models"))
			{
				return await ImportModelsWrapperAsync(root);
			}

			await ShowUnsupportedModelTypeAsync(XModelElementMetadata.ResolveModelType(root));
			return null;
		}

		private async Task<Prop> ImportModelsWrapperAsync(XElement modelsElement)
		{
			var modelCandidates = modelsElement.Elements()
				.Where(element => XModelElementMetadata.ElementNameEquals(element, "model"))
				.Select((element, index) => new XModelCandidate(
					element,
					new XModelSelectionItem(
						index,
						GetModelDisplayName(element, index),
						XModelElementMetadata.ResolveModelType(element),
						GetModelParser(element) != null)))
				.ToList();

			if (!modelCandidates.Any())
			{
				await ShowModelErrorAsync("No models were found in the xModel file.", "Model import error");
				return null;
			}

			if (modelCandidates.All(candidate => !candidate.SelectionItem.IsSupported))
			{
				await ShowUnsupportedModelTypeAsync(modelCandidates[0].SelectionItem.ModelType);
				return null;
			}

			var selectionItems = DisambiguateSelectionItems(modelCandidates.Select(candidate => candidate.SelectionItem).ToList());
			modelCandidates = modelCandidates
				.Zip(selectionItems, (candidate, selectionItem) => candidate with { SelectionItem = selectionItem })
				.ToList();

			var selectedCandidate = modelCandidates.Count == 1
				? modelCandidates[0]
				: await SelectModelCandidateAsync(modelCandidates);

			if (selectedCandidate == null)
			{
				return null;
			}

			if (!selectedCandidate.SelectionItem.IsSupported)
			{
				await ShowUnsupportedModelTypeAsync(selectedCandidate.SelectionItem.ModelType);
				return null;
			}

			var parser = GetModelParser(selectedCandidate.Element);
			if (parser == null)
			{
				await ShowUnsupportedModelTypeAsync(selectedCandidate.SelectionItem.ModelType);
				return null;
			}

			return await ImportModelElementAsync(selectedCandidate.Element, parser);
		}

		private async Task<XModelCandidate> SelectModelCandidateAsync(IReadOnlyList<XModelCandidate> modelCandidates)
		{
			var selection = await SelectionService.SelectModelAsync(modelCandidates.Select(candidate => candidate.SelectionItem).ToList());
			if (selection == null)
			{
				return null;
			}

			return modelCandidates.SingleOrDefault(candidate => candidate.SelectionItem.Index == selection.Index);
		}

		private static string GetModelDisplayName(XElement modelElement, int index)
		{
			var modelName = XModelElementMetadata.GetAttributeValue(modelElement, "name");
			return string.IsNullOrWhiteSpace(modelName)
				? $"Unnamed model {index + 1}"
				: modelName.Trim();
		}

		private static List<XModelSelectionItem> DisambiguateSelectionItems(IReadOnlyList<XModelSelectionItem> selectionItems)
		{
			var nameCounts = selectionItems
				.GroupBy(selectionItem => selectionItem.DisplayName, StringComparer.Ordinal)
				.ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
			var usedNames = new Dictionary<string, int>(StringComparer.Ordinal);
			var disambiguatedItems = new List<XModelSelectionItem>();

			foreach (var selectionItem in selectionItems)
			{
				var displayName = selectionItem.DisplayName;
				if (nameCounts[displayName] > 1)
				{
					usedNames.TryGetValue(displayName, out var usedCount);
					usedCount++;
					usedNames[displayName] = usedCount;
					displayName = $"{displayName} ({usedCount})";
				}

				disambiguatedItems.Add(selectionItem with { DisplayName = displayName });
			}

			return disambiguatedItems;
		}

		private async Task<Prop> ImportModelElementAsync(XElement modelElement, IXModelElementParser parser)
		{
			if (!parser.CanImport(modelElement))
			{
				await ShowUnsupportedModelTypeAsync(parser.ResolveModelType(modelElement));
				return null;
			}

			var parsedModel = await parser.ParseAsync(modelElement);
			if (parsedModel == null)
			{
				return null;
			}

			return await new XModelPropAssembler().AssembleAsync(parsedModel);
		}

		private async Task ShowModelErrorAsync(string message, string title)
		{
			var dependencyResolver = this.GetDependencyResolver();
			var messageService = dependencyResolver.Resolve<IMessageService>();
			await messageService.ShowErrorAsync(message, title);
		}

		private async Task ShowUnsupportedModelTypeAsync(string modelType)
		{
			await ShowModelErrorAsync($"Unsupported model type: {modelType}. \nImport supports custom and circle model types at this time.", "Model import error");
		}

		private IXModelElementParser GetModelParser(XElement modelElement)
		{
			return CreateModelParsers().FirstOrDefault(parser => parser.CanImport(modelElement));
		}

		private IReadOnlyList<IXModelElementParser> CreateModelParsers()
		{
			return
			[
				new CustomXModelElementParser(ShowModelErrorAsync),
				new CircleXModelElementParser(ShowModelErrorAsync)
			];
		}
	}
}

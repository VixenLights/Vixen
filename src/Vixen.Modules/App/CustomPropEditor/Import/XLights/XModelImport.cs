using Catel.IoC;
using Catel.Services;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Point = System.Windows.Point;
using Range = VixenModules.App.CustomPropEditor.Import.XLights.Ranges.Range;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelImport : IModelImport
	{
		private const int Offset = 7;
		private const int MinimumPropWidth = 800;
		private const int MinimumPropHeight = 600;
		private const bool CreateLegacyStateGroups = true;

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

			var customModel = parsedModel.CustomModel;
			var propSize = CalculatePropSize(customModel);
			var prop = PropModelServices.Instance().CreateProp($"{customModel.Name} {{1}}", propSize.Width, propSize.Height);
			prop.CreatedBy = @"xModel Import";

			await Assemble(parsedModel);
			return prop;
		}

		private static (int Width, int Height) CalculatePropSize(CustomModel customModel)
		{
			if (!customModel.ModelNodes.Any())
			{
				return (
					Math.Max(customModel.X + Offset * 2, MinimumPropWidth),
					Math.Max(customModel.Y + Offset * 2, MinimumPropHeight));
			}

			var width = customModel.ModelNodes.Values.Max(modelNode => modelNode.X) + Offset + customModel.PixelSize;
			var height = customModel.ModelNodes.Values.Max(modelNode => modelNode.Y) + Offset + customModel.PixelSize;

			return (Math.Max(width, MinimumPropWidth), Math.Max(height, MinimumPropHeight));
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

		private async Task Assemble(XModelParsedModel parsedModel)
		{
			var cm = parsedModel.CustomModel;

			//Create the list of light Node candidates.
			var modelNodes = await cm.CreateModelNodesAsync();

			Dictionary<int, ElementModel> lightNodes = new Dictionary<int, ElementModel>();
			var modelGroup = AssembleModel(cm, modelNodes, lightNodes);
			AttachStateDefinitions(cm, modelGroup, lightNodes);

			if (cm.SubModels.Any())
			{
				AssembleSubModels(cm, lightNodes);
			}

			if (cm.FaceInfos.Any())
			{
				AssembleFaces(cm, lightNodes);
			}

			if (CreateLegacyStateGroups && cm.StateInfos.Any())
			{
				AssembleStates(cm, lightNodes);
			}

			if (parsedModel.GeneratedGroups.Any())
			{
				AssembleGeneratedGroups(parsedModel.GeneratedGroups, lightNodes);
			}
		}

		private ElementModel AssembleModel(
			CustomModel cm,
			Dictionary<int, ModelNode> modelNodes,
			Dictionary<int, ElementModel> lightNodes)
		{
			var modelGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - Model");
			modelGroup.ModelType = ElementModelType.Model;
			foreach (var modelNode in modelNodes.OrderBy(x => x.Value.Order))
			{
				var lightNode = PropModelServices.Instance().AddLightNode(
					modelGroup,
					new Point(modelNode.Value.X + Offset, modelNode.Value.Y + Offset),
					modelNode.Value.Order,
					cm.PixelSize,
					$"{cm.Name} {{1}} Px {modelNode.Value.Order}");
				lightNodes[modelNode.Value.Order] = lightNode;
			}

			return modelGroup;
		}

		private void AttachStateDefinitions(CustomModel cm, ElementModel modelGroup, Dictionary<int, ElementModel> lightNodes)
		{
			var usedDefinitionNames = new HashSet<string>(StringComparer.Ordinal);

			foreach (var stateInfo in cm.StateInfos)
			{
				var stateDefinition = new StateDefinitionModel
				{
					Name = GetUniqueStateDefinitionName(stateInfo.Name, usedDefinitionNames),
					Description = string.Empty,
					Items = new ObservableCollection<StateItemModel>()
				};

				foreach (var stateItem in stateInfo.StateItems.OrderBy(item => item.Index))
				{
					var elementModelIds = GetElementModelIds(stateItem.RangeGroup, lightNodes);
					if (!elementModelIds.Any())
					{
						continue;
					}

					stateDefinition.Items.Add(new StateItemModel
					{
						Name = stateItem.Name,
						Color = stateItem.Color,
						ElementModelIds = new ObservableCollection<Guid>(elementModelIds)
					});
				}

				if (stateDefinition.Items.Any())
				{
					modelGroup.StateDefinitionModels.Add(stateDefinition);
				}
			}

			modelGroup.NormalizeStateModelData();
		}

		private static string GetUniqueStateDefinitionName(string name, HashSet<string> usedDefinitionNames)
		{
			var candidateName = string.IsNullOrWhiteSpace(name)
				? StateDefinitionModel.DefaultName
				: name.Trim();

			if (usedDefinitionNames.Add(candidateName))
			{
				return candidateName;
			}

			var suffix = 2;
			string uniqueName;
			do
			{
				uniqueName = $"{candidateName} - {suffix}";
				suffix++;
			}
			while (!usedDefinitionNames.Add(uniqueName));

			return uniqueName;
		}

		private static List<Guid> GetElementModelIds(RangeGroup rangeGroup, Dictionary<int, ElementModel> lightNodes)
		{
			var elementModelIds = new List<Guid>();
			foreach (var smRange in rangeGroup.Ranges)
			{
				foreach (var order in EnumerateRange(smRange))
				{
					if (lightNodes.TryGetValue(order, out var lightNode))
					{
						elementModelIds.Add(lightNode.Id);
					}
				}
			}

			return elementModelIds.Distinct().ToList();
		}

		private void AssembleSubModels(CustomModel cm, Dictionary<int, ElementModel> lightNodes)
		{
			foreach (var subModel in cm.SubModels)
			{
				if(subModel.Type == ModelType.Ranges && !subModel.Ranges.Any()) continue; //Skip sub models with empty ranges
				var subModelGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {subModel.Name}");
				subModelGroup.ModelType = ElementModelType.SubModel;

                bool addRangeGroup = subModel.Ranges.Count > 1;
                int rangeGroupIndex = 1;

                var subModelRangeGroup = subModelGroup;
				foreach (var rangeGroup in subModel.Ranges)
                {
	                if (!rangeGroup.Ranges.Any())
	                {
						continue;
	                }
                    if (addRangeGroup)
                    {
	                    subModelRangeGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {subModel.Name} - {subModelGroup.Name} {rangeGroupIndex}", subModelGroup);
                    }

                    AddRangeLights(rangeGroup.Ranges, subModelRangeGroup, lightNodes);

                    rangeGroupIndex++;
				}

                
            }
		}

		private void AssembleFaces(CustomModel cm, Dictionary<int, ElementModel> lightNodes)
		{
			if (!cm.FaceInfos.Any())
			{
				return;
			}

			var parentFaceGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - Faces ");

			foreach (var faceInfo in cm.FaceInfos)
			{
				var faceGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {faceInfo.Name} ", parentFaceGroup);
				faceGroup.ModelType = ElementModelType.FaceInfo;

				foreach (var faceDefinition in faceInfo.FaceDefinitions)
				{
					var faceItemGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {faceInfo.Name} - {faceDefinition.Name}", faceGroup);

					faceItemGroup.FaceDefinition = new FaceDefinition()
					{
						DefaultColor = faceDefinition.Color.Name != "ff000000"?faceDefinition.Color:Color.White,
						FaceComponent = faceDefinition.FaceComponent
					};

					var subModelRangeGroup = faceItemGroup;
					
					AddRangeLights(faceDefinition.RangeGroup.Ranges, subModelRangeGroup, lightNodes);
				}
			}

		}

		private void AssembleStates(CustomModel cm, Dictionary<int, ElementModel> lightNodes)
		{
			if (!cm.StateInfos.Any())
			{
				return;
			}

			var parentStateGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - States ");

			foreach (var stateInfo in cm.StateInfos)
			{
				var stateGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {stateInfo.Name} ", parentStateGroup);
				stateGroup.ModelType = ElementModelType.StateInfo;

				foreach (var stateItem in stateInfo.StateItems)
				{
					var stateItemGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {stateInfo.Name} - S{stateItem.Index} - {stateItem.Name}", stateGroup);

					var subModelRangeGroup = stateItemGroup;
					AddRangeLights(stateItem.RangeGroup.Ranges, subModelRangeGroup, lightNodes);
				}
			}
		}

		private static void AssembleGeneratedGroups(
			IEnumerable<XModelGeneratedGroup> generatedGroups,
			Dictionary<int, ElementModel> lightNodes)
		{
			foreach (var generatedGroup in generatedGroups)
			{
				AssembleGeneratedGroup(generatedGroup, null, lightNodes);
			}
		}

		private static void AssembleGeneratedGroup(
			XModelGeneratedGroup generatedGroup,
			ElementModel parent,
			Dictionary<int, ElementModel> lightNodes)
		{
			var group = parent == null
				? PropModelServices.Instance().CreateNode(generatedGroup.Name)
				: PropModelServices.Instance().CreateNode(generatedGroup.Name, parent);
			group.ModelType = ElementModelType.SubModel;

			AddRangeLights(generatedGroup.NodeOrders, group, lightNodes);

			foreach (var childGroup in generatedGroup.Children)
			{
				AssembleGeneratedGroup(childGroup, group, lightNodes);
			}
		}

		private static void AddRangeLights(
			IEnumerable<Range> ranges,
			ElementModel parent,
			Dictionary<int, ElementModel> lightNodes)
		{
			foreach (var smRange in ranges)
			{
				foreach (var order in EnumerateRange(smRange))
				{
					if (lightNodes.TryGetValue(order, out var lightNode))
					{
						AddToParentIfNeeded(lightNode, parent);
					}
				}
			}
		}

		private static void AddRangeLights(
			IEnumerable<int> nodeOrders,
			ElementModel parent,
			Dictionary<int, ElementModel> lightNodes)
		{
			foreach (var order in nodeOrders)
			{
				if (lightNodes.TryGetValue(order, out var lightNode))
				{
					AddToParentIfNeeded(lightNode, parent);
				}
			}
		}

		private static IEnumerable<int> EnumerateRange(Range range)
		{
			int inc = range.Start > range.End ? -1 : 1;
			for (int i = range.Start; ; i += inc)
			{
				if (inc > 0 && i > range.End)
				{
					break;
				}

				if (inc < 0 && i < range.End)
				{
					break;
				}

				yield return i;
			}
		}

		private static void AddToParentIfNeeded(ElementModel lightNode, ElementModel parent)
		{
			if (lightNode.Parents.Contains(parent.Id))
			{
				return;
			}

			PropModelServices.Instance().AddToParent(lightNode, parent);
		}

	}
}

using Catel.IoC;
using Catel.Services;
using NLog;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using VixenModules.App.CustomPropEditor.Import.XLights.Faces;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Point = System.Windows.Point;
using Range = VixenModules.App.CustomPropEditor.Import.XLights.Ranges.Range;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelImport : IModelImport
	{
		protected static Logger Logging = LogManager.GetCurrentClassLogger();
		private const int Offset = 7;
		private const bool CreateLegacyStateGroups = true;
		
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

			if (IsCustomModelElement(root))
			{
				return await ImportCustomModelElementAsync(root);
			}

			await ShowUnsupportedModelTypeAsync(ResolveModelType(root));
			return null;
		}

		private async Task<Prop> ImportCustomModelElementAsync(XElement modelElement)
		{
			if (!IsCustomModelElement(modelElement))
			{
				await ShowUnsupportedModelTypeAsync(ResolveModelType(modelElement));
				return null;
			}

			var name = GetAttributeValue(modelElement, "name");
			var cm = new CustomModel(name);

			//These are the size of the grid near as I can tell
			//We will use them to gauge a scale.
			int.TryParse(GetAttributeValue(modelElement, "parm1"), out var x);
			int.TryParse(GetAttributeValue(modelElement, "parm2"), out var y);

			cm.X = x;
			cm.Y = y;

			var prop = x < 800 && y < 600
				//Ensure a minimum size by using the default
				? PropModelServices.Instance().CreateProp($"{name} {{1}}")
				: PropModelServices.Instance().CreateProp($"{name} {{1}}", x + 20, y + 20);

			prop.CreatedBy = @"xModel Import";

			int.TryParse(GetAttributeValue(modelElement, "PixelSize"), out var nodeSize);
			cm.PixelSize = nodeSize;

			cm.StringType = GetAttributeValue(modelElement, "StringType");
			cm.StrandNames = GetAttributeValue(modelElement, "StrandNames");
			cm.NodeNames = GetAttributeValue(modelElement, "NodeNames");
			var compressedModel = GetAttributeValue(modelElement, "CustomModelCompressed");
			var customModel = GetAttributeValue(modelElement, "CustomModel");
			if (!await TryResolveModelNodesAsync(cm, compressedModel, customModel))
			{
				return null;
			}

			foreach (var childElement in modelElement.Elements())
			{
				if (ElementNameEquals(childElement, "subModel"))
				{
					ImportSubModel(cm, childElement);
				}
				else if (ElementNameEquals(childElement, "faceInfo"))
				{
					ImportFaceInfo(cm, childElement);
				}
				else if (ElementNameEquals(childElement, "stateInfo"))
				{
					ImportStateInfo(cm, childElement);
				}
				else if (ElementNameEquals(childElement, "modelGroup"))
				{
					Logging.Info(
						"Skipping xModel modelGroup {ModelGroupName} in model {ModelName}; modelGroup import is not supported.",
						GetAttributeValue(childElement, "name"),
						cm.Name);
				}
			}

			await Assemble(cm);
			return prop;
		}

		private void ImportSubModel(CustomModel customModel, XElement subModelElement)
		{
			var subModel = new SubModel(GetAttributeValue(subModelElement, "name"))
			{
				Layout = GetAttributeValue(subModelElement, "layout")
			};
			var type = GetAttributeValue(subModelElement, "type");
			switch (type)
			{
				case "ranges":
					subModel.Type = ModelType.Ranges;
					subModel.Ranges = ProcessRanges(subModelElement);
					customModel.SubModels.Add(subModel);
					break;
				case "subbuffer":
					//There is currently no equivalent for this option
					break;
			}
		}

		private void ImportFaceInfo(CustomModel customModel, XElement faceInfoElement)
		{
			var type = GetAttributeValue(faceInfoElement, "Type");
			if (string.IsNullOrEmpty(type) || !type.Equals("NodeRange"))
			{
				return;
			}

			var faceInfo = new FaceInfo(GetAttributeValue(faceInfoElement, "Name"));
			foreach (var attribute in FaceInfo.Attributes)
			{
				var range = GetAttributeValue(faceInfoElement, attribute.Key);
				if (string.IsNullOrEmpty(range))
				{
					continue;
				}

				var faceItem = new FaceItem
				{
					FaceComponent = attribute.Value,
					RangeGroup = ParseRanges(range)
				};

				var color = GetAttributeValue(faceInfoElement, attribute.Key + "-Color");
				if (!string.IsNullOrEmpty(color))
				{
					faceItem.Color = ColorTranslator.FromHtml(color);
				}

				faceItem.Name = attribute.Key;
				faceInfo.FaceDefinitions.Add(faceItem);
			}

			customModel.FaceInfos.Add(faceInfo);
		}

		private void ImportStateInfo(CustomModel customModel, XElement stateInfoElement)
		{
			if (!stateInfoElement.HasAttributes)
			{
				return;
			}

			var type = GetAttributeValue(stateInfoElement, "Type");
			if (string.IsNullOrEmpty(type) || !type.Equals("NodeRange") && !type.Equals("SingleNode"))
			{
				return;
			}

			var stateInfo = new StateInfo(
				XLightsStateNameNormalizer.NormalizeStateName(
					GetAttributeValue(stateInfoElement, "Name"),
					customModel.StateInfos.Count + 1));

			//Parse the state values
			Dictionary<string, StateItem> states = new();

			foreach (var attribute in stateInfoElement.Attributes())
			{
				switch (attribute.Name.LocalName)
				{
					case "Type":
					case "NodeRange":
					case "Name":
					case "CustomColors":
						continue;
				}

				if (!attribute.Name.LocalName.StartsWith("s"))
				{
					continue;
				}

				var parts = attribute.Name.LocalName.Split('-');
				if (parts.Length <= 0)
				{
					continue;
				}

				if (!states.ContainsKey(parts[0]))
				{
					if (int.TryParse(parts[0].Substring(1), out var index))
					{
						var stateItem = new StateItem(
							index,
							XLightsStateNameNormalizer.NormalizeStateItemName(null, parts[0]));
						states.Add(parts[0], stateItem);
					}
				}

				if (parts.Length == 1)
				{
					var ranges = attribute.Value;
					if (states.ContainsKey(parts[0]))
					{
						if (type == "NodeRange")
						{
							states[parts[0]].RangeGroup = ParseRanges(ranges);
						}
						else if (type == "SingleNode")
						{
							var nodeInfo = ranges.Split(' ');
							if (nodeInfo.Length == 2)
							{
								if (int.TryParse(nodeInfo[1], out var nodeNumber))
								{
									//we have a valid node, so send it to the range parser to create a range of one.
									states[parts[0]].RangeGroup = ParseRanges(nodeInfo[1]);
								}
							}
						}
					}
				}
				else if (parts.Length == 2)
				{
					if ("Color".Equals(parts[1]))
					{
						var color = attribute.Value;
						if (!string.IsNullOrEmpty(color))
						{
							if (states.ContainsKey(parts[0]))
							{
								states[parts[0]].Color = ColorTranslator.FromHtml(color);
							}
						}
					}

					if ("Name".Equals(parts[1]))
					{
						var stateItemName = XLightsStateNameNormalizer.NormalizeStateItemName(
							attribute.Value,
							parts[0]);

						if (states.ContainsKey(parts[0]))
						{
							states[parts[0]].Name = stateItemName;
						}
					}
				}
			}

			foreach (var stateItem in states)
			{
				if (stateItem.Value.RangeGroup == null)
				{
					continue;
				}
				stateInfo.StateItems.Add(stateItem.Value);
			}

			customModel.StateInfos.Add(stateInfo);
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
			await ShowModelErrorAsync(
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

		private async Task ShowModelErrorAsync(string message, string title)
		{
			var dependencyResolver = this.GetDependencyResolver();
			var messageService = dependencyResolver.Resolve<IMessageService>();
			await messageService.ShowErrorAsync(message, title);
		}

		private async Task ShowUnsupportedModelTypeAsync(string modelType)
		{
			await ShowModelErrorAsync($"Unsupported model type: {modelType}. \nImport only supports custom model types at this time.", "Model import error");
		}

		private static bool IsCustomModelElement(XElement modelElement)
		{
			return modelElement.HasAttributes &&
				(ElementNameEquals(modelElement, "custommodel") ||
				ElementNameEquals(modelElement, "model") &&
				"Custom".Equals(GetAttributeValue(modelElement, "DisplayAs"), StringComparison.OrdinalIgnoreCase));
		}

		private static string ResolveModelType(XElement modelElement)
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

		private static bool ElementNameEquals(XElement element, string name)
		{
			return name.Equals(element.Name.LocalName, StringComparison.Ordinal);
		}

		private static string GetAttributeValue(XElement element, string name)
		{
			return element.Attribute(name)?.Value;
		}

		private List<RangeGroup> ProcessRanges(XElement element)
		{
			int line = 0;
			bool found = true;
			List<RangeGroup> rangeGroups = new List<RangeGroup>();
			while (found)
			{
				var range = GetAttributeValue(element, $"line{line}");
				if (!string.IsNullOrEmpty(range))
				{
					rangeGroups.Add(ParseRanges(range));
					line++;
				}
				else
				{
					found = false;
				}
			}

			return rangeGroups;
		}

		private async Task Assemble(CustomModel cm)
		{
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

		private RangeGroup ParseRanges(string rangeLines)
		{
			List<Range> rangeList = new List<Range>();
			var ranges = rangeLines.Split(',');
			foreach (var range in ranges)
			{
				var trimmedRange = range.Trim();
				if (!string.IsNullOrEmpty(trimmedRange))
				{
					Range r = new Range();
					var startEnd = trimmedRange.Split('-');
					if (startEnd.Length == 2)
					{
						r.Start = Convert.ToInt32(startEnd[0]);
						r.End = Convert.ToInt32(startEnd[1]);
						rangeList.Add(r);
					}
					else if (startEnd.Length == 1)
					{
						r.Start = r.End = Convert.ToInt32(startEnd[0]);
						rangeList.Add(r);
					}
				}
			}

			return new RangeGroup(rangeList);

		}

		
	}
}

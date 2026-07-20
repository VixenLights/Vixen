using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Point = System.Windows.Point;
using Range = VixenModules.App.CustomPropEditor.Import.XLights.Ranges.Range;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class XModelPropAssembler
	{
		private const int Offset = 7;
		private const int MinimumPropWidth = 800;
		private const int MinimumPropHeight = 600;
		private const bool CreateLegacyStateGroups = true;

		internal async Task<Prop> AssembleAsync(XModelParsedModel parsedModel)
		{
			var importModel = parsedModel.ImportModel;
			var propSize = CalculatePropSize(importModel);
			var prop = PropModelServices.Instance().CreateProp($"{importModel.Name} {{1}}", propSize.Width, propSize.Height);
			prop.CreatedBy = @"xModel Import";

			await Assemble(parsedModel);
			prop.PhysicalMetadata.NodeCount = CountDistinctLightNodes(prop).ToString(CultureInfo.InvariantCulture);
			return prop;
		}

		private static int CountDistinctLightNodes(Prop prop)
		{
			return prop.GetLeafNodes()
				.Where(node => node.IsLightNode)
				.Select(node => node.Id)
				.Distinct()
				.Count();
		}

		private static (int Width, int Height) CalculatePropSize(XModelImportModel importModel)
		{
			if (!importModel.ModelNodes.Any())
			{
				return (
					Math.Max(importModel.X + Offset * 2, MinimumPropWidth),
					Math.Max(importModel.Y + Offset * 2, MinimumPropHeight));
			}

			var width = importModel.ModelNodes.Values.Max(modelNode => modelNode.X) + Offset + importModel.PixelSize;
			var height = importModel.ModelNodes.Values.Max(modelNode => modelNode.Y) + Offset + importModel.PixelSize;

			return (Math.Max(width, MinimumPropWidth), Math.Max(height, MinimumPropHeight));
		}

		private async Task Assemble(XModelParsedModel parsedModel)
		{
			var importModel = parsedModel.ImportModel;

			//Create the list of light Node candidates.
			var modelNodes = await importModel.CreateModelNodesAsync();

			Dictionary<int, ElementModel> lightNodes = new Dictionary<int, ElementModel>();
			var modelGroup = AssembleModel(importModel, modelNodes, lightNodes);
			AttachStateDefinitions(importModel, modelGroup, lightNodes);

			if (importModel.SubModels.Any())
			{
				AssembleSubModels(importModel, lightNodes);
			}

			if (importModel.FaceInfos.Any())
			{
				AssembleFaces(importModel, lightNodes);
			}

			if (CreateLegacyStateGroups && importModel.StateInfos.Any())
			{
				AssembleStates(importModel, lightNodes);
			}

			if (parsedModel.GeneratedGroups.Any())
			{
				AssembleGeneratedGroups(parsedModel.GeneratedGroups, lightNodes);
			}
		}

		private ElementModel AssembleModel(
			XModelImportModel importModel,
			Dictionary<int, ModelNode> modelNodes,
			Dictionary<int, ElementModel> lightNodes)
		{
			var modelGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - Model");
			modelGroup.ModelType = ElementModelType.Model;
			foreach (var modelNode in modelNodes.OrderBy(x => x.Value.Order))
			{
				var lightNode = PropModelServices.Instance().AddLightNode(
					modelGroup,
					new Point(modelNode.Value.X + Offset, modelNode.Value.Y + Offset),
					modelNode.Value.Order,
					importModel.PixelSize,
					$"{importModel.Name} {{1}} Px {modelNode.Value.Order}");
				lightNodes[modelNode.Value.Order] = lightNode;
			}

			return modelGroup;
		}

		private void AttachStateDefinitions(XModelImportModel importModel, ElementModel modelGroup, Dictionary<int, ElementModel> lightNodes)
		{
			var usedDefinitionNames = new HashSet<string>(StringComparer.Ordinal);

			foreach (var stateInfo in importModel.StateInfos)
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

		private void AssembleSubModels(XModelImportModel importModel, Dictionary<int, ElementModel> lightNodes)
		{
			foreach (var subModel in importModel.SubModels)
			{
				if(subModel.Type == ModelType.Ranges && !subModel.Ranges.Any()) continue; //Skip sub models with empty ranges
				var subModelGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - {subModel.Name}");
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
	                    subModelRangeGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - {subModel.Name} - {subModelGroup.Name} {rangeGroupIndex}", subModelGroup);
                    }

                    AddRangeLights(rangeGroup.Ranges, subModelRangeGroup, lightNodes);

                    rangeGroupIndex++;
				}

                
            }
		}

		private void AssembleFaces(XModelImportModel importModel, Dictionary<int, ElementModel> lightNodes)
		{
			if (!importModel.FaceInfos.Any())
			{
				return;
			}

			var parentFaceGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - Faces ");

			foreach (var faceInfo in importModel.FaceInfos)
			{
				var faceGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - {faceInfo.Name} ", parentFaceGroup);
				faceGroup.ModelType = ElementModelType.FaceInfo;

				foreach (var faceDefinition in faceInfo.FaceDefinitions)
				{
					var faceItemGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - {faceInfo.Name} - {faceDefinition.Name}", faceGroup);

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

		private void AssembleStates(XModelImportModel importModel, Dictionary<int, ElementModel> lightNodes)
		{
			if (!importModel.StateInfos.Any())
			{
				return;
			}

			var parentStateGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - States ");

			foreach (var stateInfo in importModel.StateInfos)
			{
				var stateGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - {stateInfo.Name} ", parentStateGroup);
				stateGroup.ModelType = ElementModelType.StateInfo;

				foreach (var stateItem in stateInfo.StateItems)
				{
					var stateItemGroup = PropModelServices.Instance().CreateNode($"{importModel.Name} {{1}} - {stateInfo.Name} - S{stateItem.Index} - {stateItem.Name}", stateGroup);

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

using System.Text.RegularExpressions;
using Common.Controls;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.OutputFilter.DimmingCurve;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Property.Color;
using VixenModules.Property.Face;
using VixenModules.Property.Order;
using VixenModules.Property.State;
using FaceComponent = VixenModules.App.CustomPropEditor.Model.FaceComponent;

namespace VixenModules.Preview.VixenPreview
{
	public class PreviewCustomPropBuilder
	{
		private readonly Prop _prop;

		private Dictionary<Guid, ElementNode> _elementModelMap;
		private HashSet<string> _elementNames;
		private readonly List<ElementNode> _leafNodes = new List<ElementNode>();
		private readonly Dictionary<string, string> _tokenLookup = new Dictionary<string, string>();
		private static readonly Regex Regex = new Regex(@"{\d+}");
		private readonly VixenPreviewControl _parent;

		public PreviewCustomPropBuilder(Prop prop, double zoomLevel, VixenPreviewControl parent)
		{
			if (prop == null)
			{
				throw new ArgumentNullException(nameof(prop));
			}
			_prop = prop;
			_parent = parent;
			PreviewCustomProp = new PreviewCustomProp(zoomLevel);
		}

		public PreviewCustomProp PreviewCustomProp { get; private set; }

		public async Task<ElementNode> CreateAsync()
		{
			return await Task.Factory.StartNew(() =>
			{
				_elementModelMap = new Dictionary<Guid, ElementNode>();
				//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
				_elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

				var rootNode = _prop.RootNode;

				ElementNode rootElementNode = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(_elementNames, TokenizeName(rootNode.Name)), true, false);
				PreviewCustomProp.Name = rootElementNode.Name;
				_elementNames.Add(rootElementNode.Name);

				_elementModelMap.Add(rootNode.Id, rootElementNode);

				CreateElementsForChildren(rootElementNode, rootNode);
				AddStateProperties(rootNode);

				var parent = Application.OpenForms["VixenPreviewSetup3"];
				if (parent != null)
				{
					//Get on the UI thread
					parent.Invoke((MethodInvoker)delegate
					{
						var question = new MessageBoxForm("Would you like to configure a dimming curve for this element?", "Dimming Curve Setup", MessageBoxButtons.YesNo, SystemIcons.Question);
						var ans = question.ShowDialog(parent);

						if (ans == DialogResult.OK)
						{
							DimmingCurveHelper dimmingHelper = new DimmingCurveHelper(true);
							dimmingHelper.Owner = parent;
							dimmingHelper.Perform(_leafNodes);
						}

						if (_prop.PhysicalMetadata.ColorMode != ColorMode.Other)
						{
							//Now lets setup the color handling.
							ColorSetupHelper helper = new ColorSetupHelper();
							helper.Owner = parent;
							switch (_prop.PhysicalMetadata.ColorMode)
							{
								case ColorMode.FullColor:
									helper.SetColorType(ElementColorType.FullColor);
									helper.SilentMode = true;
									break;
								case ColorMode.Multiple:
									helper.SetColorType(ElementColorType.MultipleDiscreteColors);
									break;
								default:
									helper.SetColorType(ElementColorType.SingleColor);
									break;
							}

							helper.Perform(_leafNodes);
							if (helper.GetColorType() != ElementColorType.FullColor)
							{
								EnsureFaceMapColors(rootElementNode);
							}
						}

					});
				}

				PreviewCustomProp.UpdateColorType();
				
				PreviewCustomProp.Layout();

				return rootElementNode;
			});
		}

		private void EnsureFaceMapColors(IElementNode node)
		{
			foreach (var elementNode in node.GetNodeEnumerator())
			{
				var fm = FaceModule.GetFaceModuleForElement(elementNode);
				if (fm != null)
				{
					var color = ColorModule.getValidColorsForElementNode(elementNode, true).FirstOrDefault();
					fm.DefaultColor = color;
				}
			}
		}

		private DialogResult ShowDimmingCurveMessage()
		{
			var question = new MessageBoxForm("Would you like to configure a dimming curve for this element?", "Dimming Curve Setup", MessageBoxButtons.YesNo, SystemIcons.Question);

			var parent = Application.OpenForms["VixenPreviewSetup3"];

			return question.ShowDialog(parent);
		}

		private string TokenizeName(string name)
		{
			if (name == null) return string.Empty;
			var returnValue = name;
			var match = Regex.Match(name);
			while (match.Success)
			{
				string value;
				if (!_tokenLookup.TryGetValue(match.Value, out value))
				{
					value = _parent.GetSubstitutionString(match.Value);
					_tokenLookup.Add(match.Value, value);
				}
				
				returnValue = returnValue.Replace(match.Value, value);
				match = match.NextMatch();
			}

			return returnValue;
		}

		private void CreateElementsForChildren(ElementNode parentNode, ElementModel model)
		{
			foreach (var elementModel in model.Children)
			{
				var newnode = FindOrCreateElementNode(elementModel, parentNode);
				CreateElementsForChildren(newnode, elementModel);
			}
		}

		private void AddStateProperties(ElementModel model)
		{
			foreach (var child in model.Children)
			{
				AddStateProperties(child);
			}

			AddImportedStateDefinitions(model);

			var stateItemModels = model.Children
				.Where(child => child.StateDefinition != null)
				.ToList();
			if (stateItemModels.Count == 0 || !_elementModelMap.TryGetValue(model.Id, out var node))
			{
				return;
			}

			var state = node.Properties.Contains(StateDescriptor.ModuleId)
				? node.Properties.Get(StateDescriptor.ModuleId) as StateModule
				: node.Properties.Add(StateDescriptor.ModuleId) as StateModule;
			if (state == null)
			{
				return;
			}

			state.Name = stateItemModels[0].StateDefinition.StateDefinitionName;
			state.Items = stateItemModels
				.Where(item => _elementModelMap.ContainsKey(item.Id))
				.Select(item => new StateItemData
				{
					Name = item.StateDefinition.Name,
					Color = item.StateDefinition.DefaultColor,
					ElementNodeIds = [_elementModelMap[item.Id].Id]
				})
				.ToList();
		}

		private void AddImportedStateDefinitions(ElementModel model)
		{
			if (model.StateDefinitions == null ||
				!model.StateDefinitions.Any() ||
				!_elementModelMap.TryGetValue(model.Id, out var node))
			{
				return;
			}

			var state = node.Properties.Contains(StateDescriptor.ModuleId)
				? node.Properties.Get(StateDescriptor.ModuleId) as StateModule
				: node.Properties.Add(StateDescriptor.ModuleId) as StateModule;
			if (state == null)
			{
				return;
			}

			state.StateDefinitions = model.StateDefinitions
				.GroupBy(definition => definition.StateDefinitionName)
				.Select(group => new StateDefinitionData
				{
					Name = group.Key,
					Description = string.Empty,
					Items = group
						.Select(item => new StateItemData
						{
							Name = item.Name,
							Color = item.DefaultColor,
							ElementNodeIds = (item.ElementModelIds ?? [])
								.Where(elementModelId => _elementModelMap.ContainsKey(elementModelId))
								.Select(elementModelId => _elementModelMap[elementModelId].Id)
								.Distinct()
								.ToList()
						})
						.Where(item => item.ElementNodeIds.Any())
						.ToList()
				})
				.Where(definition => definition.Items.Any())
				.ToList();
		}

		private ElementNode FindOrCreateElementNode(ElementModel elementModel, ElementNode parentNode)
		{
			ElementNode node;
			if (!_elementModelMap.TryGetValue(elementModel.Id, out node))
			{
				//Validate we have a name
				if (string.IsNullOrEmpty(elementModel.Name))
				{
					elementModel.Name = @"Unnamed";
				}
				//We have not created our element yet
				node = ElementNodeService.Instance.CreateSingle(parentNode,
					NamingUtilities.Uniquify(_elementNames, TokenizeName(elementModel.Name)));
				_elementModelMap.Add(elementModel.Id, node);
				_elementNames.Add(node.Name);
				if (elementModel.FaceDefinition.FaceComponent != FaceComponent.None)
				{
					FaceModule fm;
					if (node.Properties.Contains(FaceDescriptor.ModuleId))
					{
						fm = node.Properties.Get(FaceDescriptor.ModuleId) as FaceModule;
					}
					else
					{
						fm = node.Properties.Add(FaceDescriptor.ModuleId) as FaceModule;
					}

					if (ElementModel.IsPhoneme(elementModel.FaceDefinition.FaceComponent))
					{
						fm.PhonemeList.Add(elementModel.FaceDefinition.FaceComponent.ToString(), true);
					}
					else
					{
						switch (elementModel.FaceDefinition.FaceComponent)
						{
							case FaceComponent.EyesOpen:
								fm.FaceComponents.Add(Property.Face.FaceComponent.EyesOpen, true);
								break;
							case FaceComponent.EyesClosed:
								fm.FaceComponents.Add(Property.Face.FaceComponent.EyesClosed, true);
								break;
							case FaceComponent.Outlines:
								fm.FaceComponents.Add(Property.Face.FaceComponent.Outlines, true);
								break;
						}
					}
					
					//Handle colors
					fm.DefaultColor = elementModel.FaceDefinition.DefaultColor;
				}
				if (elementModel.IsLightNode)
				{
					if (node.Properties.Add(OrderDescriptor.ModuleId) is OrderModule order)
					{
						order.Order = elementModel.Order;
					}

					_leafNodes.Add(node);

					PreviewCustomProp.AddLightNodes(elementModel, node);
				}
			}
			else
			{
				//Our element exists, so add this one as a child.
				VixenSystem.Nodes.AddChildToParent(node,parentNode);
			}

			return node;
		}
	}
}

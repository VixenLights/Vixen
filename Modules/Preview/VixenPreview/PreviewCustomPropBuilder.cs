using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catel.Services;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Property.Color;
using VixenModules.Property.Order;

namespace VixenModules.Preview.VixenPreview
{
	public class PreviewCustomPropBuilder
	{
		private readonly Prop _prop;

		private Dictionary<Guid, ElementNode> _elementModelMap;
		private HashSet<string> _elementNames;
		private List<ElementNode> _leafNodes = new List<ElementNode>();
		private string _nameTokenValue = String.Empty;

		public PreviewCustomPropBuilder(Prop prop, double zoomLevel)
		{
			if (prop == null)
			{
				throw new ArgumentNullException(nameof(prop));
			}
			_prop = prop;
			PreviewCustomProp = new PreviewCustomProp(zoomLevel);
		}

		public PreviewCustomProp PreviewCustomProp { get; private set; }

		public async Task CreateAsync()
		{

			if (_prop.SupportsToken)
			{
				MessageBoxService mbs = new MessageBoxService();
				var response = mbs.GetUserInput("Enter name token replacement value.", "Prop naming", "1");
				if (response.Result == MessageResult.OK)
				{
					_nameTokenValue = response.Response;
				}
			}

			Task t = Task.Factory.StartNew(() =>
			{
				_elementModelMap = new Dictionary<Guid, ElementNode>();
				//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
				_elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

				var rootNode = _prop.RootNode;

				ElementNode rootElementNode = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(_elementNames, TokenizeName(rootNode.Name)), true, false);
				
				_elementNames.Add(rootElementNode.Name);

				_elementModelMap.Add(rootNode.Id, rootElementNode);

				CreateElementsForChildren(rootElementNode, rootNode);

				if (_prop.PhysicalMetadata.ColorMode != ColorMode.Other)
				{
					//Now lets setup the color handling.
					ColorSetupHelper helper = new ColorSetupHelper();
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
				}
				
				PreviewCustomProp.Layout();
				
			});

			await t;

		}

		private string TokenizeName(string name)
		{
			if (_prop.SupportsToken)
			{
				return name.Replace(_prop.ReplacementToken, _nameTokenValue);
			}

			return name;
		}

		private void CreateElementsForChildren(ElementNode parentNode, ElementModel model)
		{
			foreach (var elementModel in model.Children)
			{
				var newnode = FindOrCreateElementNode(elementModel, parentNode);
				CreateElementsForChildren(newnode, elementModel);
			}
		}

		private ElementNode FindOrCreateElementNode(ElementModel elementModel, ElementNode parentNode)
		{
			ElementNode node;
			if (!_elementModelMap.TryGetValue(elementModel.Id, out node))
			{
				//We have not created our element yet
				node = ElementNodeService.Instance.CreateSingle(parentNode,
					NamingUtilities.Uniquify(_elementNames, TokenizeName(elementModel.Name)));
				_elementModelMap.Add(elementModel.Id, node);
				_elementNames.Add(node.Name);
				if (elementModel.IsLightNode)
				{
					var order = node.Properties.Add(OrderDescriptor.ModuleId) as OrderModule;
					if (order != null)
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
				parentNode.AddChild(node);
			}

			return node;
		}
	}
}

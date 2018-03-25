using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;
using VixenModules.App.CustomPropEditor.Model;
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
			Task t = Task.Factory.StartNew(() =>
			{
				_elementModelMap = new Dictionary<Guid, ElementNode>();
				//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
				_elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

				var rootNode = _prop.RootNode;

				ElementNode rootElementNode = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(_elementNames, rootNode.Name), true, false);
				var order = rootElementNode.Properties.Add(OrderDescriptor.ModuleId) as OrderModule;
				if (order != null)
				{
					order.Order = rootNode.Order;
				}

				_elementNames.Add(rootElementNode.Name);

				_elementModelMap.Add(rootNode.Id, rootElementNode);

				CreateElementsForChildren(rootElementNode, rootNode);

				//Now lets setup the color handling.
				ColorSetupHelper helper = new ColorSetupHelper();
				switch (_prop.PhysicalMetadata.ColorMode)
				{
					case ColorMode.FullColor:
						helper.SetColorType(ElementColorType.FullColor);
						helper.SilentMode = false;
						break;
					case ColorMode.Multiple:
						helper.SetColorType(ElementColorType.MultipleDiscreteColors);
						break;
					default:
						helper.SetColorType(ElementColorType.SingleColor);
						break;
				}
				
				helper.Perform(_leafNodes);

				PreviewCustomProp.Layout();
				
			});

			await t;

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
					NamingUtilities.Uniquify(_elementNames, elementModel.Name));
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

					////Check to see if we are a full color prop and if so add the color property for it
					//if (_prop.PhysicalMetadata.ColorMode == ColorMode.FullColor)
					//{
					//	var colorProperty = node.Properties.Add(ColorDescriptor.ModuleId) as ColorModule;
					//	if (colorProperty != null)
					//	{
					//		colorProperty.ColorType = ElementColorType.FullColor;
					//		colorProperty.SingleColor = Color.Empty;
					//		colorProperty.ColorSetName = null;
					//	}
					//}
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

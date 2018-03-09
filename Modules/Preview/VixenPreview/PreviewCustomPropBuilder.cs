using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Property.Order;

namespace VixenModules.Preview.VixenPreview
{
	public class PreviewCustomPropBuilder
	{
		private readonly Prop _prop;

		private Dictionary<Guid, ElementNode> _elementModelMap;
		private HashSet<string> _elementNames;

		public PreviewCustomPropBuilder(Prop prop)
		{
			if (prop == null)
			{
				throw new ArgumentNullException(nameof(prop));
			}
			_prop = prop;
			PreviewCustomProp = new PreviewCustomProp();
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

				PreviewCustomProp.UpdateBounds();
				PreviewCustomProp.MoveTo(20,20);
				
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

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
				List<ElementNode> result = new List<ElementNode>();

				//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
				HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

				var rootNode = _prop.RootNode;

				ElementNode grouphead = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames, rootNode.Name), true, false);
				var order = grouphead.Properties.Add(OrderDescriptor.ModuleId) as OrderModule;
				if (order != null)
				{
					order.Order = rootNode.Order;
				}

				result.Add(grouphead);
				elementNames.Add(grouphead.Name);
				CreateElementsForChildren(grouphead, rootNode, result, elementNames);

				PreviewCustomProp.UpdateBounds();
				PreviewCustomProp.MoveTo(20,20);
				
			});

			await t;

		}

		private void CreateElementsForChildren(ElementNode parentNode, ElementModel model, List<ElementNode> results, HashSet<string> elementNames)
		{
			foreach (var elementModel in model.Children)
			{
				ElementNode newnode = ElementNodeService.Instance.CreateSingle(parentNode, NamingUtilities.Uniquify(elementNames, elementModel.Name), true, false);
				results.Add(newnode);
				elementNames.Add(newnode.Name);
				var order = newnode.Properties.Add(OrderDescriptor.ModuleId) as OrderModule;
				if (order != null)
				{
					order.Order = elementModel.Order;
				}

				if (elementModel.IsLightNode)
				{
					PreviewCustomProp.AddLightNodes(elementModel, newnode);
				}

				CreateElementsForChildren(newnode, elementModel, results, elementNames);
			}
		}
	}
}

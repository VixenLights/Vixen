using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

		public async Task CreateAsync()
		{
			Task t = Task.Factory.StartNew(() =>
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

				PreviewCustomProp.UpdateColorType();
				
				PreviewCustomProp.Layout();
				
			});

			await t;

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
				VixenSystem.Nodes.AddChildToParent(node,parentNode);
			}

			return node;
		}
	}
}

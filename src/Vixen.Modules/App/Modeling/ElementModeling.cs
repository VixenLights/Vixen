using System.Diagnostics;
using System.IO;
using Svg;
using Vixen.Sys;
using VixenModules.Property.Location;
using VixenModules.Property.Order;
using VixenModules.App.Modeling.Views;

namespace VixenModules.App.Modeling
{
	public class ElementModeling
	{
		/// <summary>
		/// Exports an SVG wire diagram for an already ordered sequence of points.
		/// </summary>
		/// <param name="nodes">The points and existing displayed node numbers in wiring order.</param>
		/// <param name="flip"><see langword="true" /> to mirror point positions; otherwise, <see langword="false" />.</param>
		/// <remarks>Numbering and sequence order are preserved. Canceling or closing the options dialog produces no export.</remarks>
		public static void OrderedPointsToSvg(IEnumerable<Tuple<Point, int>> nodes, bool flip = false)
		{
			ArgumentNullException.ThrowIfNull(nodes);
			var orderedNodes = nodes.ToList();
			if (orderedNodes.Count == 0) return;

			var options = WireDiagramExportOptionsWindow.ShowAsDialog();
			if (options == null) return;

			var document = WireDiagramSvgBuilder.Build(orderedNodes, flip, options);
			if (document != null)
			{
				OpenDoc(document);
			}
		}

		/// <summary>
		/// Exports an SVG wire diagram for the located elements beneath the specified node.
		/// </summary>
		/// <param name="elementNode">The root of the element hierarchy to export.</param>
		/// <param name="flip"><see langword="true" /> to mirror point positions; otherwise, <see langword="false" />.</param>
		/// <remarks>Existing wiring order and displayed numbers are preserved. Canceling or closing the options dialog produces no export.</remarks>
		public static void ElementsToSvg(ElementNode elementNode, bool flip = false)
		{
			if (elementNode == null) return;

			var leafNodes = elementNode.GetLeafEnumerator().Distinct().ToList();
			if (leafNodes.Count == 0 || !leafNodes.All(node => node.Properties.Contains(LocationDescriptor._typeId))) return;

			IEnumerable<ElementNode> orderedNodes = leafNodes;
			if (leafNodes.All(node => node.Properties.Contains(OrderDescriptor.ModuleId)))
			{
				orderedNodes = OrderNodes(leafNodes, elementNode);
			}

			var nodes = new List<Tuple<Point, int>>();
			var counter = 1;
			foreach (var node in orderedNodes)
			{
				var order = node.Properties.Get(OrderDescriptor.ModuleId) is OrderModule orderModule
					? orderModule.Order
					: counter;
				nodes.Add(Tuple.Create(LocationModule.GetPositionForElement(node), order));
				counter++;
			}

			var options = WireDiagramExportOptionsWindow.ShowAsDialog();
			if (options == null) return;

			var document = WireDiagramSvgBuilder.Build(nodes, flip, options);
			if (document != null)
			{
				OpenDoc(document);
			}
		}

		private static void OpenDoc(SvgDocument doc)
		{
			var file = Path.GetTempFileName();
			var filePath = Path.ChangeExtension(file, "svg");
			doc.Write(filePath);
			var psi = new ProcessStartInfo()
			{
				FileName = filePath,
				UseShellExecute = true
			};
			Process.Start(psi);
		}

		private static List<ElementNode> OrderNodes(IEnumerable<ElementNode> leafNodes, ElementNode TopLevelNode)
		{
			//Try simple ordering first
			var count = leafNodes.DistinctBy(x => ((OrderModule) x.Properties.Get(OrderDescriptor.ModuleId)).Order).Count();
			if (count == leafNodes.Count())
			{
				//we have a distinct set, so simple ordering should work
				return leafNodes.OrderBy(x => ((OrderModule)x.Properties.Get(OrderDescriptor.ModuleId)).Order).ToList();
			}

			//Try to order them by groups. This may not work right all the time either.
			Dictionary<ElementNode, List<ElementNode>> parentChild = new Dictionary<ElementNode, List<ElementNode>>();
			var nonLeafs = TopLevelNode.GetNonLeafEnumerator();
			foreach (var leafNode in leafNodes)
			{
				var parent = leafNode.Parents.FirstOrDefault(x => nonLeafs.Contains(x));
				if (parent != null)
				{
					if (parentChild.TryGetValue(parent, out var children))
					{
						children.Add(leafNode);
					}
					else
					{
						var childList = new List<ElementNode>();
						childList.Add(leafNode);
						parentChild.Add(parent, childList);
					}
				}
			}

			List<ElementNode> orderedNodes = new List<ElementNode>();
			foreach (var childList in parentChild.Values)
			{
				orderedNodes.AddRange(childList.OrderBy(x => ((OrderModule) x.Properties.Get(OrderDescriptor.ModuleId)).Order).ToList());
			}

			return orderedNodes;
		}
	}
}

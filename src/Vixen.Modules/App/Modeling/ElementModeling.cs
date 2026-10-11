using System.Diagnostics;
using Svg;
using Vixen.Sys;
using VixenModules.Property.Location;
using VixenModules.Property.Order;

namespace VixenModules.App.Modeling
{
	public class ElementModeling
	{
		public static void OrderedPointsToSvg(IEnumerable<Tuple<Point, int>> nodes, bool flip = false)
		{
			ArgumentNullException.ThrowIfNull(nodes);
			var orderedNodes = nodes.ToList();
			if (orderedNodes.Count == 0) return;

			var document = WireDiagramSvgBuilder.Build(orderedNodes, flip, new WireDiagramExportOptions());
			if (document != null)
			{
				OpenDoc(document);
			}
		}

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

			var document = WireDiagramSvgBuilder.Build(nodes, flip, new WireDiagramExportOptions());
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

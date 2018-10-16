using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using Svg;
using Vixen.Sys;
using VixenModules.Property.Location;
using VixenModules.Property.Order;

namespace VixenModules.App.Modeling
{
	public class ElementModeling
	{
		public static void ElementsToSvg(ElementNode elementNode)
		{
			if(elementNode == null) return;
			var leafNodes = elementNode.GetLeafEnumerator().Distinct();
			if (leafNodes.All(x => x.Properties.Contains(LocationDescriptor._typeId)))
			{
				var locations = leafNodes.Select(LocationModule.GetPositionForElement);
				var rect = CalculateModelRectangle(locations);
				var scale = 1f;
				if (rect.Width < 800)
				{
					scale = 800f / rect.Width;
				}
				SvgDocument doc = new SvgDocument { Width = rect.Width*scale, Height = rect.Height*scale };
				doc.ViewBox = new SvgViewBox(-20, -20, doc.Width+40, doc.Height+40);

				var group = new SvgGroup();
				doc.Children.Add(group);

				if (leafNodes.All(x => x.Properties.Contains(OrderDescriptor.ModuleId)))
				{
					leafNodes = OrderNodes(leafNodes, elementNode);
				}

				var lastPoint = new PointF();
				var first = true;
				var radius = 6;
				int counter = 1;
				foreach (var node in leafNodes)
				{
					var point = ScalePoint(LocationModule.GetPositionForElement(node),scale, rect.Location);

					var lightNode = CreateLightNode(point, radius);
					group.Children.Add(lightNode);
					int order;
					if (node.Properties.Get(OrderDescriptor.ModuleId) is OrderModule orderModule)
					{
						order = orderModule.Order;
					}
					else
					{
						order = counter;
					}

					var label = CreateLabel(order, point, radius);
					group.Children.Add(label);

					if (!first)
					{
						var line = CreateLine(lastPoint, point, radius);
						group.Children.Add(line);
					}
					first = false;
					lastPoint = point;
					counter++;
					

				}
				
				//var stream = new FileStream(@"C:\Test\circle.svg", FileMode.Create);
				var file = System.IO.Path.GetTempFileName();
				var filePath = System.IO.Path.ChangeExtension(file, "svg");
				//var filePath = $"{file}{System.IO.Path.PathSeparator}{nodes.First()}.svg";
				doc.Write(filePath);
				System.Diagnostics.Process.Start(filePath);
			}


		}

		private static PointF ScalePoint(Point p, float scale, Point offset)
		{
			return new PointF((p.X-offset.X)*scale, (p.Y - offset.Y)*scale);
		}

		private static Rectangle CalculateModelRectangle(IEnumerable<Point> points)
		{
			var xMax = points.Max(p => p.X);
			var xMin = points.Min(p => p.X);
			var yMax = points.Max(p => p.Y);
			var yMin = points.Min(p => p.Y);
			var width = xMax - xMin;
			var height = yMax - yMin;
			return new Rectangle(xMin, yMin, width>0?width:50, height>0?height:50);
		}

		private static SvgLine CreateLine(PointF start, PointF end, int radius)
		{
			//TODO figure out which way the line is moving
			return new SvgLine
			{
				StartX = start.X,
				StartY = start.Y,
				EndX = end.X,
				EndY = end.Y,
				Fill = new SvgColourServer(Color.Black),
				StrokeWidth = 1,
				Stroke = new SvgColourServer(Color.Black)

			};
		}

		private static SvgText CreateLabel(int order, PointF p, int radius)
		{
			return new SvgText
			{
				Text = order.ToString(),
				Fill = new SvgColourServer(Color.Black),
				FontSize = 16,
				X = { p.X + radius },
				Y = { p.Y - radius }
			};
		}

		private static SvgCircle CreateLightNode(PointF p, int radius)
		{
			return new SvgCircle
			{
				Radius = radius,
				Fill = new SvgColourServer(Color.Red),
				//Stroke = new SvgColourServer(Color.Black),
				//StrokeWidth = 1,
				CenterX = p.X,
				CenterY = p.Y
			};
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

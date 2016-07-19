using System.Drawing;
using Vixen.Sys;
using VixenModules.Property.Location;

namespace VixenModules.Effect.Effect.Location
{
	public class ElementLocation
	{
		public ElementLocation(ElementNode node)
		{
			ElementNode = node;
			InitializeLocation(node);
		}
		public int X { get; set; }

		public int Y { get; set; }

		public ElementNode ElementNode { get; set; }

		private void InitializeLocation(ElementNode node)
		{
			var point = GetLocation(node);
			X = point.X;
			Y = point.Y;
		}

		private static Point GetLocation(ElementNode node)
		{
			return LocationModule.GetPositionForElement(node);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys;
using VixenModules.Property.Location;

namespace VixenModules.Effect.Effect.Location
{
	public class ElementLocation:IEquatable<ElementLocation>, IEqualityComparer<ElementLocation>
	{
		public ElementLocation(IElementNode node)
		{
			ElementNode = node;
			InitializeLocation(node);
		}
		public int X { get; set; }

		public int Y { get; set; }

		public IElementNode ElementNode { get; set; }

		private void InitializeLocation(IElementNode node)
		{
			var point = GetLocation(node);
			X = point.X;
			Y = point.Y;
		}

		private static Point GetLocation(IElementNode node)
		{
			return LocationModule.GetPositionForElement(node);
		}

		public override bool Equals(object obj)
		{
			var el = obj as ElementLocation;
			if (el != null)
			{
				return Equals(el);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ElementNode.GetHashCode();
		}

		public bool Equals(ElementLocation other)
		{
			return other.ElementNode.Equals(ElementNode);
		}

		public bool Equals(ElementLocation x, ElementLocation y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(ElementLocation obj)
		{
			return ElementNode.GetHashCode();
		}
	}
}

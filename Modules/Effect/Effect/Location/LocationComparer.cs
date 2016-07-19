using System.Collections.Generic;

namespace VixenModules.Effect.Effect.Location
{
	internal class LocationComparer: IComparer<ElementLocation>
	{
		public int Compare(ElementLocation first, ElementLocation second)
		{
			if (first.X == second.X)
			{
				return first.Y - second.Y;
			}
			return first.X - second.X;
		}
	}
}

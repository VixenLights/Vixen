using System.Collections.Generic;

namespace VixenModules.Effect.Effect.Location
{
	internal class LocationComparer: IComparer<ElementLocation>
	{
		private readonly bool _sortByY;
		public LocationComparer(bool sortByY = false)
		{
			_sortByY = sortByY;
		}

		public int Compare(ElementLocation first, ElementLocation second)
		{
			if (_sortByY)
			{
				if (first.Y == second.Y)
				{
					return first.X - second.X;
				}
				return first.Y - second.Y;
			}

			if (first.X == second.X)
			{
				return first.Y - second.Y;
			}
			return first.X - second.X;
		}
	}
}

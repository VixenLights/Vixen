#nullable enable
namespace VixenModules.Preview.VixenPreview
{
	/// <summary>
	/// Provides recovery rules for Preview windows restored from saved bounds.
	/// </summary>
	public static class PreviewWindowBounds
	{
		/// <summary>
		/// Determines whether a saved Preview window can be restored at its saved location.
		/// </summary>
		/// <param name="windowBounds">The saved bounds of the Preview window.</param>
		/// <param name="workingAreas">The working areas of the active monitors.</param>
		/// <returns><see langword="true" /> if a positive-size window has an upper corner in an active working area; otherwise, <see langword="false" />.</returns>
		public static bool IsRecoverable(Rectangle windowBounds, IEnumerable<Rectangle>? workingAreas)
		{
			if (windowBounds.Width <= 0 || windowBounds.Height <= 0 || workingAreas is null)
			{
				return false;
			}

			var upperLeft = windowBounds.Location;
			var upperRight = new Point(windowBounds.Right - 1, windowBounds.Top);

			foreach (var workingArea in workingAreas)
			{
				if (workingArea.Contains(upperLeft) || workingArea.Contains(upperRight))
				{
					return true;
				}
			}

			return false;
		}
	}
}

using System.Drawing;

namespace Common.DiscreteColorPicker.Views
{
	/// <summary>
	/// Base class for a discrete color picker view window.
	/// </summary>
	public abstract class DiscreteColorPickerViewBase : Catel.Windows.Window
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="availableColors">Available discrete colors to pick from</param>
		/// <param name="initiallySelectedColors">Initially selected colors</param>
		public DiscreteColorPickerViewBase(HashSet<Color> availableColors, HashSet<Color> initiallySelectedColors)
		{
			// Store off the available colors
			AvailableColors = availableColors;

			// Store off the initially selected colors
			InitiallySelectedColors = initiallySelectedColors;
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Collection of available colors to pick from.
		/// </summary>
		protected HashSet<Color> AvailableColors { get; private set; }

		/// <summary>
		/// Collection of initially selected colors.
		/// </summary>
		protected HashSet<Color> InitiallySelectedColors { get; private set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Configures the window size based on the specified item width and the number of available colors.
		/// </summary>
		/// <param name="itemWidth">Width of each color item</param>
		/// <param name="thresholdForFirstRow">
		/// Due to the minimum dialog width being driven by OK and Cance button this argument controls
		/// how many color items to display on the first row before starting on the second row.
		/// </param>
		protected void ConfigureWindowSize(int itemWidth, int thresholdForFirstRow)
		{
			// Count the number of available of colors
			int count = AvailableColors.Count;

			// Determine the square root of the count and round up
			int root = (int)Math.Round((Math.Sqrt(count) + 0.5), MidpointRounding.AwayFromZero);

			// If the current color item is the bottom left corner
			if ((root - 1) * (root - 1) == count)
			{
				// The rounding used to calculate the root is off by one for the bottom right color item.
				// Need to reduce it by one so that the grid does not expand.
				root = root - 1;
			}

			// If we are still under the threshold for filling the first row then...
			if (count < thresholdForFirstRow)
			{
				// Ensure we stay on the first row
				root = 1;
			}
			// Otherwise check to see if we are still filling the 2nd row
			else if (count < 5)
			{
				root = 2;
			}


			// Max out the grid at 8x8 (64 colors)
			const int MaxColumns = 8;

			int scrollBar = 0;

			// Limit the width to 8 colors
			if (root > MaxColumns)
			{
				root = MaxColumns;
				scrollBar = 10;
			}

			// Set the width of the window
			Width = root * itemWidth + 10 + scrollBar;

			// Don't let the minimum width go below 250 pixels otherwise the Ok, Cancel buttons look goofy
			const int MinimumWidth = 250;

			// If the width is below the minimum then...
			if (Width < MinimumWidth)
			{
				// Set the width to the minimum
				Width = MinimumWidth;
			}

			// Determine if there are unused boxes
			int delta = root * root - count;

			// Determine if there are unused rows
			int heightAdjustment = delta / root;

			// If there are more colors than grid spots
			if (heightAdjustment < 0)
			{
				// Zero out the height adjustment
				heightAdjustment = 0;
			}

			// Determine the height of the window
			const int ItemHeight = 72;
			Height = (root - heightAdjustment) * ItemHeight + 75;
		}

		#endregion
	}
}

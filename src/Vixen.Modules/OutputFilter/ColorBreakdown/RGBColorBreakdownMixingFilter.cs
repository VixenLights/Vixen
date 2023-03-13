namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// This filter gets the intensity percent for a given state in simple RGB mixing.
	/// </summary>
	internal class RGBColorBreakdownMixingFilter : ColorBreakdownMixingFilterBase 
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="breakdownItem">Breakdown item to create filter for</param>
		public RGBColorBreakdownMixingFilter(ColorBreakdownItem breakdownItem)
		{
			// Configure the RGB filter
			ConfigureRGBFilter(breakdownItem);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Configures the RGB filter for the specified breakdown item.
		/// </summary>
		/// <param name="breakdownItem">Breakdown item to process</param>
		private void ConfigureRGBFilter(ColorBreakdownItem breakdownItem)
		{
			if (breakdownItem.Color.Equals(Color.Red))
			{
				GetMaxProportionFunc = color => color.R / 255f;
			}
			else if (breakdownItem.Color.Equals(Color.Lime))
			{
				GetMaxProportionFunc = color => color.G / 255f;
			}
			else
			{
				GetMaxProportionFunc = color => color.B / 255f;
			}
		}

		#endregion
	}
}

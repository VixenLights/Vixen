using VixenModules.OutputFilter.DimmingFilter.Outputs;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.DimmingFilter
{
    /// <summary>
    /// Maintains a dimming filter module.
    /// </summary>
    public class DimmingFilterModule : TaggedFilterModuleBase<DimmingFilterData, DimmingFilterOutput, DimmingFilterDescriptor>
	{
		#region IHasSetup

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override bool HasSetup => true;

		/// <summary>
		/// Configures the filter.
		/// </summary>
		/// <returns>True if the filter was configured</returns>
		public override bool Setup()
		{
			// Default to the OK button not being selected
			bool okSelected = false;

			// Display dimming filter setup dialog
			using (DimmingFilterSetup setup = new DimmingFilterSetup(Data))
			{
				// If the user selected OK then...
				if (setup.ShowDialog() == DialogResult.OK)
				{
					// Re-create the filter's output
					CreateOutput();

					// Indicate that setup completed successfully
					okSelected = true;
				}
			}

			return okSelected;
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Determines if colors are converted into dimming intents.
		/// </summary>
		public bool ConvertColorIntoDimmingIntents
		{
			get { return Data.ConvertRGBIntoDimmingIntents; }
			set { Data.ConvertRGBIntoDimmingIntents = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override DimmingFilterOutput CreateOutputInternal()
		{
			// Create the dimming filter output
			DimmingFilterOutput dimmingfFilterOutput = new DimmingFilterOutput(Data.Tag, Data.ConvertRGBIntoDimmingIntents);

			// Configure the output
			dimmingfFilterOutput.ConfigureFilter();

			// Return the filter output
			return dimmingfFilterOutput;
		}

		#endregion
	}
}
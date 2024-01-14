using VixenModules.OutputFilter.PrismFilter.Output;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.PrismFilter
{
	/// <summary>
	/// Maintains a prism filter module.
	/// </summary>
	public class PrismFilterModule : TaggedFilterModuleBase<PrismFilterData, PrismFilterOutput, PrismFilterDescriptor>
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

			// Display the Tagger filter setup dialog
			using (PrismFilterSetup setup = new PrismFilterSetup(Data))
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
		/// Flag which determines if prism intents are converted into open prism intents.
		/// </summary>
		public bool ConvertPrismIntentsIntoOpenPrismIntents
		{
			get { return Data.ConvertPrismIntentsIntoOpenPrismIntents; }
			set { Data.ConvertPrismIntentsIntoOpenPrismIntents = value; }
		}

		/// <summary>
		/// Open prism index command value.
		/// </summary>
		public byte OpenPrismIndexValue
		{
			get { return Data.OpenPrismIndexValue; }
			set { Data.OpenPrismIndexValue = value; }
		}

		/// <summary>
		/// Close prism index command value.
		/// </summary>
		public byte ClosePrismIndexValue
		{
			get { return Data.ClosePrismIndexValue; }
			set { Data.ClosePrismIndexValue = value; }
		}

		/// <summary>
		/// Prism Function Name associated with the filter.
		/// </summary>
		public string AssociatedFunctionName
		{
			get { return Data.AssociatedFunctionName; }
			set { Data.AssociatedFunctionName = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override PrismFilterOutput CreateOutputInternal()
		{
			// Create the prism filter output
			PrismFilterOutput output = new PrismFilterOutput(
				Data.Tag, 
				Data.ConvertPrismIntentsIntoOpenPrismIntents, 
				Data.OpenPrismIndexValue, 
				Data.ClosePrismIndexValue,
				Data.AssociatedFunctionName);

			// Configure the filter
			output.ConfigureFilter();

			// Return the filter's output
			return output;
		}

		#endregion
	}
}
using System.Windows.Forms;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
    /// <summary>
    /// Maintains a tagged filter module.
    /// </summary>
    public class TaggedFilterModule : TaggedFilterModuleBase<TaggedFilterData, TaggedFilterOutput, TaggedFilterDescriptor>
	{
		#region IHasSetup

		/// <summary>
		/// Refer interface documentation.
		/// </summary>
		public override bool HasSetup => true;

		/// <summary>
		/// Refer interface documentation.
		/// </summary>
		public override bool Setup()
		{
			// Default to the OK button not being selected
			bool okSelected = false;

			// Display the Tagger filter setup dialog
			using (TaggedFilterSetup setup = new TaggedFilterSetup(Data))
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

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override TaggedFilterOutput CreateOutputInternal()
		{
			// Create the tagged filter output
			return new TaggedFilterOutput(Data.Tag);
		}

		#endregion
	}
}
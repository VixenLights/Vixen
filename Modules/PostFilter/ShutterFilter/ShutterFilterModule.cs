using System.Windows.Forms;
using VixenModules.OutputFilter.ShutterFilter.Output;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ShutterFilter
{
	/// <summary>
	/// Maintains a shutter filter module.
	/// </summary>
	public class ShutterFilterModule : TaggedFilterModuleBase<ShutterFilterData, ShutterFilterOutput, ShutterFilterDescriptor>
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
			using (ShutterFilterSetup setup = new ShutterFilterSetup(Data))
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
		/// Flag which determines if color intents are converted into shutter intents.
		/// </summary>
		public bool ConvertColorIntoShutterIntents
		{
			get { return Data.ConvertColorIntoShutterIntents; }
			set { Data.ConvertColorIntoShutterIntents = value; }
		}

		/// <summary>
		/// Open Shutter index command value.
		/// </summary>
		public byte OpenShutterIndexValue
		{
			get { return Data.OpenShutterIndexValue; }
			set { Data.OpenShutterIndexValue = value; }
		}

		/// <summary>
		/// Close Shutter index command value.
		/// </summary>
		public byte CloseShutterIndexValue
		{
			get { return Data.CloseShutterIndexValue; }
			set { Data.CloseShutterIndexValue = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override ShutterFilterOutput CreateOutputInternal()
		{
			// Create the shutter filter output
			ShutterFilterOutput output = new ShutterFilterOutput(Data.Tag, Data.ConvertColorIntoShutterIntents, Data.OpenShutterIndexValue, Data.CloseShutterIndexValue);

			// Configure the filter
			output.ConfigureFilter();

			// Return the filter's output
			return output;
		}

		#endregion
	}
}
using System.Collections.Generic;
using System.Windows.Forms;
using VixenModules.App.Fixture;
using VixenModules.OutputFilter.ColorWheelFilter.Outputs;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ColorWheelFilter
{
	/// <summary>
	/// Maintains a color wheel filter module.
	/// </summary>
	public class ColorWheelFilterModule : TaggedFilterModuleBase<ColorWheelFilterData, ColorWheelFilterOutput, ColorWheelFilterDescriptor>
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
			using (ColorWheelFilterSetup setup = new ColorWheelFilterSetup(Data))
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
		/// Color wheel data associated with the function.
		/// </summary>
		public List<FixtureColorWheel> ColorWheelData
		{
			get { return Data.ColorWheelData; }
			set { Data.ColorWheelData = value; }
		}

		/// <summary>
		/// Flag which determines if color intents are converted into color wheel index commands.
		/// </summary>
		public bool ConvertRGBIntoIndexCommands
		{
			get { return Data.ConvertColorIntentsIntoIndexCommands; }
			set { Data.ConvertColorIntentsIntoIndexCommands = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override ColorWheelFilterOutput CreateOutputInternal()
		{
			// Create the color wheel filter output
			ColorWheelFilterOutput colorWheelFilter = new ColorWheelFilterOutput(Data.Tag, Data.ColorWheelData, Data.ConvertColorIntentsIntoIndexCommands);

			// Configure the filter output
			colorWheelFilter.ConfigureFilter();

			return colorWheelFilter;
		}

		#endregion
	}
}
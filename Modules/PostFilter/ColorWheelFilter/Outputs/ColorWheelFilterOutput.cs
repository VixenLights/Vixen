using System.Collections.Generic;
using VixenModules.App.Fixture;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.ColorWheelFilter.Outputs
{
    /// <summary>
    /// Color wheel filter output.  This output contains special conversion logic for lamp fixtures.
    /// </summary>
    public class ColorWheelFilterOutput : TaggedFilterOutputBase<Filters.ColorWheelFilter>
	{
		#region Fields

		/// <summary>
		/// Color wheel data.  This property allows the output to know what colors are supported by the color wheel.
		/// </summary>
		private List<FixtureColorWheel> _colorWheelData;

		/// <summary>
		/// Flag indicating if the output should convert color intents into color wheel commands.
		/// </summary>
		private bool _convertColorIntentsIntoColorWheel;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag applicable to the color wheel</param>
		/// <param name="colorWheelData">Color wheel data</param>
		/// <param name="convertRBGIntoColorWheel">True when the output should convert RGB into color wheel index command</param>
		public ColorWheelFilterOutput(
			string tag, 
			List<FixtureColorWheel> colorWheelData,
			bool convertRBGIntoColorWheel) : base(tag)
		{
			// Save off the color wheel data
			_colorWheelData = colorWheelData;

			// Save off whether to convert color intents into color wheel index commands
			_convertColorIntentsIntoColorWheel = convertRBGIntoColorWheel;
		}

        #endregion

        #region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
        public override void ConfigureFilter()
        {
			// Give the color wheel filter the color wheel data
			((Filters.ColorWheelFilter)Filter).ColorWheelData = _colorWheelData;

			// Configure whether the filter should convert color intents into color wheel index commands
			((Filters.ColorWheelFilter)Filter).ConvertColorIntentsIntoColorWheel = _convertColorIntentsIntoColorWheel;
		}

        #endregion
    }
}

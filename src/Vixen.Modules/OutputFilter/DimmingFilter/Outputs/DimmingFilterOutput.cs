using VixenModules.OutputFilter.TaggedFilter.Outputs;


namespace VixenModules.OutputFilter.DimmingFilter.Outputs
{
    /// <summary>
    /// Dimming filter output.  This output contains special conversion logic for lamp fixtures.
    /// This output extract the intensity from color intents and creates dimming intents.
    /// </summary>
    public class DimmingFilterOutput : TaggedFilterOutputBase<Filters.DimmingFilter>
	{
		#region Fields

		/// <summary>
		/// Flag determines if the output converts color intensity into dim intents
		/// otherwise it is going to set the dim intent to full intensity.
		/// </summary>
		private bool _convertColorIntensityIntoDimIntents;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag (function name) of the dimmer function</param>
		/// <param name="convertColorIntensityIntoDimIntents">True when color intents should be converted to dimming intents</param>
		public DimmingFilterOutput(string tag, bool convertColorIntensityIntoDimIntents) : base(tag)			
		{
			// Store off whether to convert color intensity into dimmer intents
			_convertColorIntensityIntoDimIntents = convertColorIntensityIntoDimIntents;
		}

        #endregion

        #region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
        public override void ConfigureFilter()
        {
			// Configure whether the filter should convert color intents into dimmer intents
			((Filters.DimmingFilter)Filter).ConvertColorIntensityIntoDimIntents = _convertColorIntensityIntoDimIntents;
        }

        #endregion
    }
}

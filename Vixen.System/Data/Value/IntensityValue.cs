namespace Vixen.Data.Value
{
	/// <summary>
	/// Value that represents the intesity of something
	/// It is a 0 - 1 value representing the intensity as a percent
	/// </summary>
	public struct IntensityValue : IIntentDataType
	{
		private double _intensity;

		public IntensityValue(double intensity)
		{
			_intensity = intensity;
		}

		/// <summary>
		/// Get / Set the percent intensity as a 0 - 1 value
		/// </summary>
		public double Intensity
		{
			get { return _intensity; }
			set { _intensity = value; }
		}
	}
}

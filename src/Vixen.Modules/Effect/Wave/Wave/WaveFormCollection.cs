using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Maintains a collection of IWaveform objects.
	/// </summary>
	public class WaveFormCollection : MarkCollectionExpandoObjectCollection<IWaveform, Waveform>
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public WaveFormCollection() : 
			base("Waves")
		{
		}

		#endregion
	}
}

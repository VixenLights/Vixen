using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Maintains and serializes the settings of the wave effect.
	/// </summary>
	[DataContract]
	[KnownType(typeof(WaveformData))]
	class WaveData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public WaveData()
		{
			Orientation = StringOrientation.Horizontal;			
			WaveformData = new List<WaveformData>();

			// Add two waves 
			// 1st wave is entirely default
			WaveformData.Add(new WaveformData());

			// 2nd wave is purple and white and offset by 90 degrees
			WaveformData.Add(new WaveformData());

			WaveformData[1].Color.Colors.Clear();
			WaveformData[1].Color.Colors.Add(new ColorPoint(Color.Purple, 0.0));			
			WaveformData[1].Color.Colors.Add(new ColorPoint(Color.White, 1.0));
			WaveformData[1].ColorHandling = WaveColorHandling.Across;
			WaveformData[1].PhaseShift = 90;
			WaveformData[1].Direction = DirectionType.RightToLeft;
			
			Colors = new List<ColorGradient>
			{
				new ColorGradient(System.Drawing.Color.Blue),
				new ColorGradient(System.Drawing.Color.White),
				new ColorGradient(System.Drawing.Color.Purple)
			};
		}

		#endregion

		#region Public Properties

		[DataMember]
		public StringOrientation Orientation { get; set; }
						
		[DataMember]
		public List<WaveformData> WaveformData { get; set; }
		
		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		#endregion

		#region Protected Methods

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			WaveData result = new WaveData
			{
				Orientation = Orientation,					
			};

			// Clone the waves
			for (int index = 0; index < WaveformData.Count; index++)
			{
				result.WaveformData[index] = (WaveformData)(WaveformData[index]).CreateInstanceForClone();
			}
		
			return result;
		}

		#endregion
	}	
}

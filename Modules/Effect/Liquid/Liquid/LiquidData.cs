using Liquid;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Liquid
{
	/// <summary>
	/// Maintains and serializes the settings of the liquid effect.
	/// </summary>
	[DataContract]
	[KnownType(typeof(EmitterData))]
	public class LiquidData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public LiquidData()
		{
			Orientation = StringOrientation.Horizontal;

			MixColors = true;			
			ParticleSize = 500;

			// Default to two emitters
			EmitterData = new List<EmitterData>();
			EmitterData.Add(new EmitterData());
			EmitterData.Add(new EmitterData());

			// First emitter is animated
			EmitterData[0].Animate = true;
			EmitterData[0].X = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			EmitterData[0].Y = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));

			// Second Emitter is static
			EmitterData[1].Color = new ColorGradient(System.Drawing.Color.Purple);

			// Initialize the scale factor to 1/4
			RenderScaleFactor = 4;

			Colors = new List<ColorGradient>
			{
				new ColorGradient(System.Drawing.Color.Blue),
				new ColorGradient(System.Drawing.Color.White),
				new ColorGradient(System.Drawing.Color.Purple)
			};

			// Audio Defaults
			DecayTime = 1500;
			AttackTime = 50;
			Normalize = true;
			Gain = 0;
			Range = 10;
			LowPass = false;
			LowPassFreq = 100;
			HighPass = false;
			HighPassFreq = 500;

			WarmUpFrames = 0;
			DespeckleThreshold = 0;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public bool TopBarrier { get; set; }

		[DataMember]
		public bool BottomBarrier { get; set; }

		[DataMember]
		public bool LeftBarrier { get; set; }

		[DataMember]
		public bool RightBarrier { get; set; }

		[DataMember]
		public bool MixColors { get; set; }
		
		[DataMember]
		public int ParticleSize { get; set; }

		[DataMember]
		public List<EmitterData> EmitterData { get; set; }

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			LiquidData result = new LiquidData
			{
				Orientation = Orientation,
				TopBarrier = TopBarrier,
				BottomBarrier = BottomBarrier,
				LeftBarrier = LeftBarrier,
				RightBarrier = RightBarrier,
				MixColors = MixColors,			
				ParticleSize = ParticleSize,

				Gain = Gain,
				HighPass = HighPass,
				HighPassFreq = HighPassFreq,
				LowPass = LowPass,
				LowPassFreq = LowPassFreq,
				Normalize = Normalize,
				Range = Range,
				DecayTime = DecayTime,
				AttackTime = AttackTime,
				RenderScaleFactor = RenderScaleFactor,
				WarmUpFrames = WarmUpFrames,
				DespeckleThreshold = DespeckleThreshold,
			};

			// Clone the emitters
			for (int index = 0; index < EmitterData.Count; index++)
			{
				result.EmitterData[index] = (EmitterData)(EmitterData[index]).CreateInstanceForClone();
			}

			return result;
		}

		[DataMember]
		public int Gain { get; set; }

		[DataMember]
		public bool HighPass { get; set; }

		[DataMember]
		public int HighPassFreq { get; set; }

		[DataMember]
		public bool LowPass { get; set; }

		[DataMember]
		public int LowPassFreq { get; set; }

		[DataMember]
		public bool Normalize { get; set; }

		[DataMember]
		public int Range { get; set; }

		[DataMember]
		public int DecayTime { get; set; }

		[DataMember]
		public int AttackTime { get; set; }

		[DataMember]
		public int RenderScaleFactor { get; set; }

		[DataMember]
		public int WarmUpFrames { get; set; }

		[DataMember]
		public int DespeckleThreshold { get; set; }
	}

	#endregion
}

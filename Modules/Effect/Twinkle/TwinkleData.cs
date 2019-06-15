using System.Runtime.Serialization;
using System.Drawing;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Twinkle
{
	[DataContract]
	public class TwinkleData : EffectTypeModuleData
	{
		[DataMember]
		public bool IndividualChannels { get; set; }

		[DataMember]
		public double MinimumLevel { get; set; }

		[DataMember]
		public double MaximumLevel { get; set; }

		[DataMember]
		public int LevelVariation { get; set; }

		[DataMember]
		public int AveragePulseTime { get; set; }

		[DataMember]
		public int PulseTimeVariation { get; set; }

		[DataMember]
		public int AverageCoverage { get; set; }

		[DataMember]
		public TwinkleColorHandling ColorHandling { get; set; }

		private Color _staticColor;

		[DataMember]
		public Color StaticColor
		{
			get { return _staticColor; }
			set
			{
				_staticColor = value;
				StaticColorGradient = new ColorGradient(_staticColor);
			}
		}

		public ColorGradient StaticColorGradient { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (StaticColor.IsEmpty)
			{
				StaticColor = Color.White;
			}

			if (MaximumLevel > 1)
			{
				MaximumLevel = 1;
			}
		}

		[DataMember]
		public int DepthOfEffect { get; set; }

		public TwinkleData()
		{
			IndividualChannels = true;
			MinimumLevel = 0;
			MaximumLevel = 1;
			LevelVariation = 50;
			AveragePulseTime = 400;
			PulseTimeVariation = 30;
			AverageCoverage = 50;
			ColorHandling = TwinkleColorHandling.GradientForEachPulse;
			StaticColor = Color.White;
			ColorGradient = new ColorGradient(Color.White);
			DepthOfEffect = 0;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			TwinkleData result = new TwinkleData();
			result.IndividualChannels = IndividualChannels;
			result.MinimumLevel = MinimumLevel;
			result.MaximumLevel = MaximumLevel;
			result.LevelVariation = LevelVariation;
			result.AveragePulseTime = AveragePulseTime;
			result.PulseTimeVariation = PulseTimeVariation;
			result.AverageCoverage = AverageCoverage;
			result.ColorHandling = ColorHandling;
			result.StaticColor = StaticColor;
			result.ColorGradient = new ColorGradient(ColorGradient);
			result.DepthOfEffect = DepthOfEffect;
			return result;
		}
	}
}
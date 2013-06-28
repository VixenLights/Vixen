using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Drawing;
using Vixen.Module;
using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.Twinkle
{
	[DataContract]
	public class TwinkleData : ModuleDataModelBase
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

		[DataMember]
		public Color StaticColor { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[DataMember]
		public int DepthOfEffect { get; set; }

		public TwinkleData()
		{
			IndividualChannels = true;
			MinimumLevel = 0;
			MaximumLevel = 100;
			LevelVariation = 50;
			AveragePulseTime = 400;
			PulseTimeVariation = 30;
			AverageCoverage = 50;
			ColorHandling = TwinkleColorHandling.GradientForEachPulse;
			StaticColor = Color.Empty;
			ColorGradient = new ColorGradient();
			DepthOfEffect = 0;
		}

		public override IModuleDataModel Clone()
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

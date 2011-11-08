using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using CommonElements.ColorManagement.ColorModels;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Property.RGB;
using VixenModules.Effect.Pulse;
using System.Drawing;
using ZedGraph;

namespace VixenModules.Effect.Twinkle
{
	public class Twinkle : EffectModuleInstanceBase
	{
		private static Random _random = new Random();

		private TwinkleData _data;
		private ChannelData _channelData = null;

		public Twinkle()
		{
			_data = new TwinkleData();
		}

		protected override void _PreRender()
		{
			_channelData = new ChannelData();

			IEnumerable<ChannelNode> targetNodes = RGBModule.FindAllRenderableChildren(TargetNodes);

			List<IndividualTwinkleDetails> twinkles = null;
			if (!IndividualChannels)
				twinkles = GenerateTwinkleData();

			foreach (ChannelNode node in targetNodes) {
				_channelData.AddChannelData(RenderChannel(node, twinkles));
			}
		}

		protected override ChannelData _Render()
		{
			return _channelData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as TwinkleData; }
		}

		public override object[] ParameterValues
		{
			get
			{
				return new object[] {
					IndividualChannels,
					MinimumLevel,
					MaximumLevel,
					LevelVariation,
					AveragePulseTime,
					PulseTimeVariation,
					AverageCoverage,
					ColorHandling,
					StaticColor,
					ColorGradient
				};
			}
			set
			{
				if (value.Length != 10) {
					VixenSystem.Logging.Warning("Twinkle effect parameters set with " + value.Length + " parameters");
					return;
				}

				IndividualChannels = (bool)value[0];
				MinimumLevel = (Level)value[1];
				MaximumLevel = (Level)value[2];
				LevelVariation = (int)value[3];
				AveragePulseTime = (int)value[4];
				PulseTimeVariation = (int)value[5];
				AverageCoverage = (int)value[6];
				ColorHandling = (TwinkleColorHandling)value[7];
				StaticColor = (Color)value[8];
				ColorGradient = (ColorGradient)value[9];
			}
		}

		public override bool IsDirty
		{
			get
			{
				if (!ColorGradient.CheckLibraryReference())
					return true;

				return base.IsDirty;
			}
			protected set
			{
				base.IsDirty = value;
			}
		}

		public bool IndividualChannels
		{
			get { return _data.IndividualChannels; }
			set { _data.IndividualChannels = value; IsDirty = true; }
		}

		public Level MinimumLevel
		{
			get { return _data.MinimumLevel; }
			set { _data.MinimumLevel = value; IsDirty = true; }
		}

		public Level MaximumLevel
		{
			get { return _data.MaximumLevel; }
			set { _data.MaximumLevel = value; IsDirty = true; }
		}

		public int LevelVariation
		{
			get { return _data.LevelVariation; }
			set { _data.LevelVariation = value; IsDirty = true; }
		}

		public int AveragePulseTime
		{
			get { return _data.AveragePulseTime; }
			set { _data.AveragePulseTime = value; IsDirty = true; }
		}

		public int PulseTimeVariation
		{
			get { return _data.PulseTimeVariation; }
			set { _data.PulseTimeVariation = value; IsDirty = true; }
		}

		public int AverageCoverage
		{
			get { return _data.AverageCoverage; }
			set { _data.AverageCoverage = value; IsDirty = true; }
		}

		public TwinkleColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set { _data.ColorHandling = value; IsDirty = true; }
		}

		public Color StaticColor
		{
			get { return _data.StaticColor; }
			set { _data.StaticColor = value; IsDirty = true; }
		}

		public ColorGradient ColorGradient
		{
			get { return _data.ColorGradient; }
			set { _data.ColorGradient = value; IsDirty = true; }
		}

		private ChannelData RenderChannel(ChannelNode node, List<IndividualTwinkleDetails> twinkles = null)
		{
			if (node == null)
				return null;

			if (twinkles == null)
				twinkles = GenerateTwinkleData();

			ChannelData result = new ChannelData();

			foreach (IndividualTwinkleDetails twinkle in twinkles) {
				{
					// make a pulse for it
					Pulse.Pulse pulse = new Pulse.Pulse();
					pulse.TargetNodes = new ChannelNode[] { node };
					pulse.TimeSpan = twinkle.Duration;
					pulse.LevelCurve = twinkle.TwinkleCurve;

					// figure out what color gradient to use for the pulse
					switch (ColorHandling) {
						case TwinkleColorHandling.GradientForEachPulse:
							pulse.ColorGradient = ColorGradient;
							break;

						case TwinkleColorHandling.GradientThroughWholeEffect:
							double startPos = ((double)twinkle.StartTime.Ticks / (double)TimeSpan.Ticks);
							double endPos = ((double)(twinkle.StartTime + twinkle.Duration).Ticks / (double)TimeSpan.Ticks);
							pulse.ColorGradient = ColorGradient.GetSubGradient(startPos, endPos);
							break;

						case TwinkleColorHandling.StaticColor:
							pulse.ColorGradient = new ColorGradient(StaticColor);
							break;
					}

					ChannelData pulseData = pulse.Render();
					pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
					result.AddChannelData(pulseData);
				}
			}

			return result;
		}


		private List<IndividualTwinkleDetails> GenerateTwinkleData()
		{
			List<IndividualTwinkleDetails> result = new List<IndividualTwinkleDetails>();

			// step through the effect in increments of a 20th of the average pulse time.
			TimeSpan step = TimeSpan.FromMilliseconds(AveragePulseTime / 100.0);
			double chance = AverageCoverage / 100.0 / 100.0;		// was a percentage.
			for (TimeSpan current = TimeSpan.Zero; current < TimeSpan; current += step) {
				// if the next random twinkle instance happens, make a pulse for it
				if (_random.NextDouble() <= chance) {

					// generate a time length for it (all in ms)
					int maxDurationVariation = (int)((PulseTimeVariation / 100.0) * AveragePulseTime);
					int twinkleDurationMs = _random.Next(AveragePulseTime - maxDurationVariation, AveragePulseTime + maxDurationVariation + 1);
					TimeSpan twinkleDuration = TimeSpan.FromMilliseconds(twinkleDurationMs);

					// it might have to be capped to fit within the duration of the whole effect, so figure that out
					if (current + twinkleDuration > TimeSpan) {
						// it's past the end of the effect. If it can be reduced to fit in the acceptable range, do that, otherwise skip it
						if ((TimeSpan - current).TotalMilliseconds >= AveragePulseTime - maxDurationVariation) {
							twinkleDuration = (TimeSpan - current);
						} else {
							// if we can't fit anything else into this time gap, not much point continuing the iteration
							break;
						}
					}

					// generate the levels/curve for it
					Level minLevel = MinimumLevel;
					int maxLevelVariation = (int)((LevelVariation / 100.0) * (MaximumLevel - MinimumLevel));
					int reduction = _random.Next(maxLevelVariation);
					Level maxLevel = MaximumLevel - reduction;
					Curve curve = new Curve(new PointPairList(new double[] { 0, 50, 100 }, new double[] { minLevel, maxLevel, minLevel }));

					IndividualTwinkleDetails occurance = new IndividualTwinkleDetails();
					occurance.StartTime = current;
					occurance.Duration = twinkleDuration;
					occurance.TwinkleCurve = curve;

					result.Add(occurance);
				}
			}

			return result;
		}

		private class IndividualTwinkleDetails
		{
			public TimeSpan StartTime;
			public TimeSpan Duration;
			public Curve TwinkleCurve;
		}
	}
}

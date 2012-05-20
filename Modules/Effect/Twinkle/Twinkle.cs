using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using ZedGraph;

namespace VixenModules.Effect.Twinkle
{
	public class Twinkle : EffectModuleInstanceBase
	{
		private static Random _random = new Random();

		private TwinkleData _data;
		private EffectIntents _channelData = null;

		public Twinkle()
		{
			_data = new TwinkleData();
		}

		protected override void _PreRender()
		{
			_channelData = new EffectIntents();

			IEnumerable<ChannelNode> targetNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());

			List<IndividualTwinkleDetails> twinkles = null;
			if (!IndividualChannels)
				twinkles = GenerateTwinkleData();

			foreach (ChannelNode node in targetNodes) {
				_channelData.Add(node.Channel.Id, RenderChannel(node, twinkles));
			}
		}

		protected override EffectIntents _Render()
		{
			return _channelData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as TwinkleData; }
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

		[Value]
		public bool IndividualChannels
		{
			get { return _data.IndividualChannels; }
			set { _data.IndividualChannels = value; IsDirty = true; }
		}

		[Value]
		public double MinimumLevel
		{
			get { return _data.MinimumLevel; }
			set { _data.MinimumLevel = value; IsDirty = true; }
		}

		[Value]
		public double MaximumLevel
		{
			get { return _data.MaximumLevel; }
			set { _data.MaximumLevel = value; IsDirty = true; }
		}

		[Value]
		public int LevelVariation
		{
			get { return _data.LevelVariation; }
			set { _data.LevelVariation = value; IsDirty = true; }
		}

		[Value]
		public int AveragePulseTime
		{
			get { return _data.AveragePulseTime; }
			set { _data.AveragePulseTime = value; IsDirty = true; }
		}

		[Value]
		public int PulseTimeVariation
		{
			get { return _data.PulseTimeVariation; }
			set { _data.PulseTimeVariation = value; IsDirty = true; }
		}

		[Value]
		public int AverageCoverage
		{
			get { return _data.AverageCoverage; }
			set { _data.AverageCoverage = value; IsDirty = true; }
		}

		[Value]
		public TwinkleColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set { _data.ColorHandling = value; IsDirty = true; }
		}

		[Value]
		public Color StaticColor
		{
			get { return _data.StaticColor; }
			set { _data.StaticColor = value; IsDirty = true; }
		}

		[Value]
		public ColorGradient ColorGradient
		{
			get { return _data.ColorGradient; }
			set { _data.ColorGradient = value; IsDirty = true; }
		}

		private IntentNodeCollection RenderChannel(ChannelNode node, List<IndividualTwinkleDetails> twinkles = null)
		{
			if (node == null)
				return null;

			if (twinkles == null)
				twinkles = GenerateTwinkleData();

			EffectIntents result = new EffectIntents();

			// render the flat 'minimum value' across the entire effect
			Pulse.Pulse pulse = new Pulse.Pulse();
			pulse.TargetNodes = new ChannelNode[] { node };
			pulse.TimeSpan = TimeSpan;
			pulse.LevelCurve = new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { MinimumLevel, MinimumLevel }));

			// figure out what color gradient to use for the pulse
			switch (ColorHandling) {
				case TwinkleColorHandling.GradientForEachPulse:
					pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(0));
					break;

				case TwinkleColorHandling.GradientThroughWholeEffect:
					pulse.ColorGradient = ColorGradient;
					break;

				case TwinkleColorHandling.StaticColor:
					pulse.ColorGradient = new ColorGradient(StaticColor);
					break;
			}

			EffectIntents pulseData = pulse.Render();
			result.Add(pulseData);

			// render all the individual twinkles
			foreach (IndividualTwinkleDetails twinkle in twinkles) {
				{
					// make a pulse for it
					pulse = new Pulse.Pulse();
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

					pulseData = pulse.Render();
					pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
					result.Add(pulseData);
				}
			}

			return result.GetIntentNodesForChannel(node.Channel.Id);
		}


		private List<IndividualTwinkleDetails> GenerateTwinkleData()
		{
			List<IndividualTwinkleDetails> result = new List<IndividualTwinkleDetails>();

			// the mean interval between individual flickers (used for random generation later)
			double meanMillisecondsBetweenTwinkles = AveragePulseTime / (AverageCoverage / 100.0) / 2.0;
			double maxMillisecondsBetweenTwinkles = AveragePulseTime / (AverageCoverage / 100.0);

			// the maximum amount of time an individual flicker/twinkle can vary off the average by
			int maxDurationVariation = (int)((PulseTimeVariation / 100.0) * AveragePulseTime);

			for (TimeSpan current = TimeSpan.Zero; current < TimeSpan; ) {

				// calculate how long until the next flicker, and clamp it (since there's a small chance it's huge)
				double nextTime = Math.Log(1.0 - _random.NextDouble()) * -meanMillisecondsBetweenTwinkles;
				if (nextTime > maxMillisecondsBetweenTwinkles)
				    nextTime = maxMillisecondsBetweenTwinkles;

				// check if the timespan will be off the end, if so, bail
				current += TimeSpan.FromMilliseconds(nextTime);
				if (current >= TimeSpan)
					break;

				// generate a time length for it (all in ms)
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
				double minLevel = MinimumLevel;
				int maxLevelVariation = (int)((LevelVariation / 100.0) * (MaximumLevel - MinimumLevel));
				int reduction = _random.Next(maxLevelVariation);
				double maxLevel = MaximumLevel - reduction;
				Curve curve = new Curve(new PointPairList(new double[] { 0, 50, 100 }, new double[] { minLevel * 100, maxLevel * 100, minLevel * 100 }));

				IndividualTwinkleDetails occurance = new IndividualTwinkleDetails();
				occurance.StartTime = current;
				occurance.Duration = twinkleDuration;
				occurance.TwinkleCurve = curve;

				result.Add(occurance);
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

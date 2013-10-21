using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using ZedGraph;
using VixenModules.Property.Color;

namespace VixenModules.Effect.Twinkle
{
	public class Twinkle : EffectModuleInstanceBase
	{
		private static Random _random = new Random();
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private TwinkleData _data;
		private EffectIntents _elementData = null;

		public Twinkle()
		{
			_data = new TwinkleData();
		}

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			IEnumerable<ElementNode> targetNodes = GetNodesToRenderOn();

			List<IndividualTwinkleDetails> twinkles = null;
			if (!IndividualElements)
				twinkles = GenerateTwinkleData();

			int totalNodes = targetNodes.Count();
			double i = 0;

			foreach (ElementNode node in targetNodes) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;

				if (node != null)
					_elementData.Add(RenderElement(node, i++/(double) totalNodes, twinkles));
			}
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			HashSet<Color> validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));

			if (validColors.Any() &&
				(!validColors.Contains(_data.StaticColor) || !_data.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))) //Discrete colors specified
			{
				_data.ColorGradient = new ColorGradient(validColors.DefaultIfEmpty(Color.White).First());

				//Set a default color 
				_data.StaticColor = validColors.First();
			}

		}

		protected override EffectIntents _Render()
		{
			return _elementData;
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
			protected set { base.IsDirty = value; }
		}

		[Value]
		public bool IndividualElements
		{
			get { return _data.IndividualChannels; }
			set
			{
				_data.IndividualChannels = value;
				IsDirty = true;
			}
		}

		[Value]
		public double MinimumLevel
		{
			get { return _data.MinimumLevel; }
			set
			{
				_data.MinimumLevel = value;
				IsDirty = true;
			}
		}

		[Value]
		public double MaximumLevel
		{
			get { return _data.MaximumLevel; }
			set
			{
				_data.MaximumLevel = value;
				IsDirty = true;
			}
		}

		[Value]
		public int LevelVariation
		{
			get { return _data.LevelVariation; }
			set
			{
				_data.LevelVariation = value;
				IsDirty = true;
			}
		}

		[Value]
		public int AveragePulseTime
		{
			get { return _data.AveragePulseTime; }
			set
			{
				_data.AveragePulseTime = value;
				IsDirty = true;
			}
		}

		[Value]
		public int PulseTimeVariation
		{
			get { return _data.PulseTimeVariation; }
			set
			{
				_data.PulseTimeVariation = value;
				IsDirty = true;
			}
		}

		[Value]
		public int AverageCoverage
		{
			get { return _data.AverageCoverage; }
			set
			{
				_data.AverageCoverage = value;
				IsDirty = true;
			}
		}

		[Value]
		public TwinkleColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set
			{
				_data.ColorHandling = value;
				IsDirty = true;
			}
		}

		[Value]
		public Color StaticColor
		{
			get
			{
				return _data.StaticColor;
			}
			set
			{
				_data.StaticColor = value;
				IsDirty = true;
			}
		}

		[Value]
		public ColorGradient ColorGradient
		{
			get
			{
				return _data.ColorGradient;
			}
			set
			{
				_data.ColorGradient = value;
				IsDirty = true;
			}
		}

		//Created to hold a ColorGradient version of color rather than continually creating them from Color for static colors.
		protected ColorGradient StaticColorGradient
		{
			get { return _data.StaticColorGradient; }
			set { _data.StaticColorGradient = value; }
		}

		[Value]
		public int DepthOfEffect
		{
			get { return _data.DepthOfEffect; }
			set
			{
				_data.DepthOfEffect = value;
				IsDirty = true;
			}
		}

		private EffectIntents RenderElement(ElementNode node, double positionWithinGroup,
		                                    List<IndividualTwinkleDetails> twinkles = null)
		{
			if (node == null || node.Element == null)
				return null;

			if (twinkles == null)
				twinkles = GenerateTwinkleData();

			EffectIntents result = new EffectIntents();

			bool discreteColors = ColorModule.isElementNodeDiscreteColored(node);

			// render the flat 'minimum value' across the entire effect
			Pulse.Pulse pulse = new Pulse.Pulse();
			pulse.TargetNodes = new ElementNode[] {node};
			pulse.TimeSpan = TimeSpan;
			double minPulseValue = MinimumLevel * 100.0;
			EffectIntents pulseData;

			// figure out what color gradient to use for the pulse
			if (MinimumLevel > 0.0) {
				switch (ColorHandling) {
					case TwinkleColorHandling.GradientForEachPulse:
						if (discreteColors) {
							List<Tuple<Color, float>> colorProportions = ColorGradient.GetDiscreteColorsAndProportionsAt(0);
							foreach (Tuple<Color, float> colorProportion in colorProportions) {
								double value = minPulseValue * colorProportion.Item2;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {value, value}));
								pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
								pulseData = pulse.Render();
								result.Add(pulseData);
							}
						} else {
							pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
							pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(0));
							pulseData = pulse.Render();
							result.Add(pulseData);
						}
						break;

					case TwinkleColorHandling.GradientThroughWholeEffect:
						pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
						pulse.ColorGradient = ColorGradient;
						pulseData = pulse.Render();
						result.Add(pulseData);
						break;

					case TwinkleColorHandling.StaticColor:
						pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
						pulse.ColorGradient = StaticColorGradient;
						pulseData = pulse.Render();
						result.Add(pulseData);
						break;

					case TwinkleColorHandling.ColorAcrossItems:
						if (discreteColors) {
							List<Tuple<Color, float>> colorsAtPosition = ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
							foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
								double value = minPulseValue * colorProportion.Item2;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {value, value}));
								pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
								pulseData = pulse.Render();
								result.Add(pulseData);
							}
						} else {
							pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
							pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup));
							pulseData = pulse.Render();
							result.Add(pulseData);
						}
						break;
				}
			}

			// render all the individual twinkles
			foreach (IndividualTwinkleDetails twinkle in twinkles) {
				{
					// make a pulse for it
					pulse = new Pulse.Pulse();
					pulse.TargetNodes = new ElementNode[] {node};
					pulse.TimeSpan = twinkle.Duration;
					pulse.LevelCurve = twinkle.TwinkleCurve;

					// figure out what color gradient to use for the pulse
					switch (ColorHandling) {
						case TwinkleColorHandling.GradientForEachPulse:
							pulse.ColorGradient = ColorGradient;
							pulseData = pulse.Render();
							pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
							result.Add(pulseData);
							break;

						case TwinkleColorHandling.GradientThroughWholeEffect:
							double startPos = ((double) twinkle.StartTime.Ticks/(double) TimeSpan.Ticks);
							double endPos = ((double) (twinkle.StartTime + twinkle.Duration).Ticks/(double) TimeSpan.Ticks);

							if (discreteColors) {
								double range = endPos - startPos;
								if (range <= 0.0) {
									Logging.Error("Twinkle: bad range: " + range + " (SP=" + startPos + ", EP=" + endPos + ")");
									break;
								}

								ColorGradient cg = ColorGradient.GetSubGradientWithDiscreteColors(startPos, endPos);

								foreach (Color color in cg.GetColorsInGradient()) {
									Curve newCurve = new Curve(pulse.LevelCurve.Points);
									foreach (PointPair point in newCurve.Points) {
										double effectRelativePosition = startPos + ((point.X / 100.0) * range);
										double proportion = ColorGradient.GetProportionOfColorAt(effectRelativePosition, color);
										point.Y *= proportion;
									}
									pulse.LevelCurve = newCurve;
									pulse.ColorGradient = new ColorGradient(color);
									pulseData = pulse.Render();
									pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
									result.Add(pulseData);
									
								}

							} else {
								pulse.ColorGradient = ColorGradient.GetSubGradient(startPos, endPos);
								pulseData = pulse.Render();
								pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
								result.Add(pulseData);
							}
							break;

						case TwinkleColorHandling.StaticColor:
							pulse.ColorGradient = StaticColorGradient;
							pulseData = pulse.Render();
							pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
							result.Add(pulseData);
							break;

						case TwinkleColorHandling.ColorAcrossItems:
							if (discreteColors) {
								List<Tuple<Color, float>> colorsAtPosition = ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
								foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
									float proportion = colorProportion.Item2;
									// scale all levels of the twinkle curve by the proportion that is applicable to this color
									Curve newCurve = new Curve(pulse.LevelCurve.Points);
									foreach (PointPair pointPair in newCurve.Points) {
										pointPair.Y *= proportion;
									}
									pulse.LevelCurve = newCurve;
									pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
									pulseData = pulse.Render();
									pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
									result.Add(pulseData);
								}
							} else {
								pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup));
								pulseData = pulse.Render();
								pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
								result.Add(pulseData);
							}
							break;
					}
				}
			}

			return result;
		}


		private List<IndividualTwinkleDetails> GenerateTwinkleData()
		{
			List<IndividualTwinkleDetails> result = new List<IndividualTwinkleDetails>();

			// the mean interval between individual flickers (used for random generation later)
			// avoid any divide-by-zeros -- if it was <= 0, cap it to 1% at least
			double averageCoverage = ((AverageCoverage <= 0) ? 1.0 : AverageCoverage)/100.0;
			double meanMillisecondsBetweenTwinkles = AveragePulseTime/averageCoverage/2.0;
			double maxMillisecondsBetweenTwinkles = AveragePulseTime/averageCoverage;

			// the maximum amount of time an individual flicker/twinkle can vary off the average by
			int maxDurationVariation = (int) ((PulseTimeVariation/100.0)*AveragePulseTime);

			for (TimeSpan current = TimeSpan.Zero; current < TimeSpan;) {
				// calculate how long until the next flicker, and clamp it (since there's a small chance it's huge)
				double nextTime = Math.Log(1.0 - _random.NextDouble())*-meanMillisecondsBetweenTwinkles;
				if (nextTime > maxMillisecondsBetweenTwinkles)
					nextTime = maxMillisecondsBetweenTwinkles;

				// check if the timespan will be off the end, if so, bail
				current += TimeSpan.FromMilliseconds(nextTime);
				if (current >= TimeSpan)
					break;

				// generate a time length for it (all in ms)
				int twinkleDurationMs = _random.Next(AveragePulseTime - maxDurationVariation,
				                                     AveragePulseTime + maxDurationVariation + 1);
				TimeSpan twinkleDuration = TimeSpan.FromMilliseconds(twinkleDurationMs);

				// it might have to be capped to fit within the duration of the whole effect, so figure that out
				if (current + twinkleDuration > TimeSpan) {
					// it's past the end of the effect. If it can be reduced to fit in the acceptable range, do that, otherwise skip it
					if ((TimeSpan - current).TotalMilliseconds >= AveragePulseTime - maxDurationVariation) {
						twinkleDuration = (TimeSpan - current);
					}
					else {
						// if we can't fit anything else into this time gap, not much point continuing the iteration
						break;
					}
				}

				// generate the levels/curve for it
				double minLevel = MinimumLevel;
				int maxLevelVariation = (int) ((LevelVariation/100.0)*(MaximumLevel - MinimumLevel));
				int reduction = _random.Next(maxLevelVariation);
				double maxLevel = MaximumLevel - reduction;
				Curve curve =
					new Curve(new PointPairList(new double[] {0, 50, 100}, new double[] {minLevel*100, maxLevel*100, minLevel*100}));

				IndividualTwinkleDetails occurance = new IndividualTwinkleDetails();
				occurance.StartTime = current;
				occurance.Duration = twinkleDuration;
				occurance.TwinkleCurve = curve;

				result.Add(occurance);
			}

			return result;
		}

		private List<ElementNode> GetNodesToRenderOn()
		{
			IEnumerable<ElementNode> renderNodes = null;

			if (DepthOfEffect == 0 || !IndividualElements) {
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator()).Distinct();
			}
			else {
				renderNodes = TargetNodes;
				for (int i = 0; i < DepthOfEffect; i++) {
					renderNodes = renderNodes.SelectMany(x => x.Children).Distinct();
				}
			}

			// If the given DepthOfEffect results in no nodes (because it goes "too deep" and misses all nodes), 
			// then we'll default to the LeafElements, which will at least return 1 element (the TargetNode)
			if (!renderNodes.Any())
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator()).Distinct();

			return renderNodes.ToList();
		}

		private class IndividualTwinkleDetails
		{
			public TimeSpan StartTime;
			public TimeSpan Duration;
			public Curve TwinkleCurve;
		}
	}
}
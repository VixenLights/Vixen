using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using NLog;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;
using ZedGraph;

namespace VixenModules.Effect.Twinkle
{
	public class Twinkle : BaseEffect
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		private TwinkleData _data;
		private EffectIntents _elementData = null;
		private Dictionary<Color, ColorValue[]> _colorValueSet;
		
		public Twinkle()
		{
			_data = new TwinkleData();
			UpdateAllAttributes();
		}

		protected override void TargetNodesChanged()
		{
			if (TargetNodes.Any())
			{
				CheckForInvalidColorData();
				var firstNode = TargetNodes.FirstOrDefault();
				if (firstNode != null && DepthOfEffect > firstNode.GetMaxChildDepth() - 1)
				{
					DepthOfEffect = 0;
				}
			}
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			IEnumerable<IElementNode> targetNodes = GetNodesToRenderOn();

			List<IndividualTwinkleDetails> twinkles = null;
			if (!IndividualElements)
				twinkles = GenerateTwinkleData();

			int totalNodes = targetNodes.Count();
			double i = 0;

			
			_colorValueSet = new Dictionary<Color, ColorValue[]>(3);
			

			foreach (IElementNode node in targetNodes) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;

				if (node != null)
				{
					bool discreteColors = HasDiscreteColors && ColorModule.isElementNodeDiscreteColored(node);
					var intents = RenderElement(node, i++/totalNodes, discreteColors, twinkles);
					//_elementData.Add(IntentBuilder.ConvertToStaticArrayIntents(intents, TimeSpan, discreteColors));
					_elementData.Add(intents);
				}
			}

			_colorValueSet = null;
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();

			if (validColors.Any())
			{
				if (!validColors.Contains(_data.StaticColor) || !_data.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors)) //Discrete colors specified
				{
					_data.ColorGradient = new ColorGradient(validColors.DefaultIfEmpty(Color.White).First());

					//Set a default color 
					_data.StaticColor = validColors.First();
				}
			}
		}
		
		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as TwinkleData;
				CheckForInvalidColorData();
				IsDirty = true;
				UpdateAllAttributes();
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
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
		[ProviderCategory(@"Depth", 10)]
		[ProviderDisplayName(@"IndividualElements")]
		[ProviderDescription(@"TwinkleDepth")]
		public bool IndividualElements
		{
			get { return _data.IndividualChannels; }
			set
			{
				_data.IndividualChannels = value;
				IsDirty = true;
				UpdateDepthAttributes();
				TypeDescriptor.Refresh(this);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness",2)]
		[ProviderDisplayName(@"MinBrightness")]
		[ProviderDescription(@"MinBrightness")]
		[PropertyEditor("LevelEditor")]
		public double MinimumLevel
		{
			get { return _data.MinimumLevel; }
			set
			{
				_data.MinimumLevel = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness",2)]
		[ProviderDisplayName(@"MaxBrightness")]
		[ProviderDescription(@"MaxBrightness")]
		[PropertyEditor("LevelEditor")]
		public double MaximumLevel
		{
			get { return _data.MaximumLevel; }
			set
			{
				if (value <= 1 && value >= 0)
				{
					_data.MaximumLevel = value;
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Brightness",2)]
		[ProviderDisplayName(@"Variation")]
		[ProviderDescription(@"TwinkleLevelVariation")]
		[PropertyEditor("SliderEditor")]
		public int LevelVariation
		{
			get { return _data.LevelVariation; }
			set
			{
				_data.LevelVariation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config",3)]
		[ProviderDisplayName(@"AveragePulseTime")]
		[ProviderDescription(@"TwinkleAvgPulseTime")]
		public int AveragePulseTime
		{
			get { return _data.AveragePulseTime; }
			set
			{
				_data.AveragePulseTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config",3)]
		[ProviderDisplayName(@"Variation")]
		[ProviderDescription(@"TwinklePulseTimeVariation")]
		[PropertyEditor("SliderEditor")]
		public int PulseTimeVariation
		{
			get { return _data.PulseTimeVariation; }
			set
			{
				_data.PulseTimeVariation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config",3)]
		[ProviderDisplayName(@"Coverage")]
		[ProviderDescription(@"TwinkleCoverage")]
		[PropertyEditor("SliderEditor")]
		public int AverageCoverage
		{
			get { return _data.AverageCoverage; }
			set
			{
				_data.AverageCoverage = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color",1)]
		[ProviderDisplayName(@"ColorHandling")]
		[ProviderDescription(@"ColorHandling")]
		[PropertyOrder(1)]
		public TwinkleColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set
			{
				_data.ColorHandling = value;
				IsDirty = true;
				UpdateColorHandlingAttributes();
				TypeDescriptor.Refresh(this);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color",1)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(2)]
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
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color",1)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(3)]
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
				OnPropertyChanged();
			}
		}

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/twinkle/"; }
		}

		#endregion

		#region Attributes

		private void UpdateAllAttributes()
		{
			UpdateColorHandlingAttributes();
			UpdateDepthAttributes();
			TypeDescriptor.Refresh(this);
		}

		private void UpdateColorHandlingAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"StaticColor", ColorHandling.Equals(TwinkleColorHandling.StaticColor)},
				{"ColorGradient", !ColorHandling.Equals(TwinkleColorHandling.StaticColor)}
			};
			SetBrowsable(propertyStates);
		}

		private void UpdateDepthAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"DepthOfEffect", IndividualElements}
			};
			SetBrowsable(propertyStates);
		}

		#endregion

		//Created to hold a ColorGradient version of color rather than continually creating them from Color for static colors.
		protected ColorGradient StaticColorGradient
		{
			get { return _data.StaticColorGradient; }
			set { _data.StaticColorGradient = value; }
		}

		[Value]
		[ProviderCategory(@"Depth")]
		[ProviderDisplayName(@"Depth")]
		[ProviderDescription(@"Depth")]
		[TypeConverter(typeof(TargetElementDepthConverter))]
		[PropertyEditor("SelectionEditor")]
		public int DepthOfEffect
		{
			get { return _data.DepthOfEffect; }
			set
			{
				_data.DepthOfEffect = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		private EffectIntents RenderElement(IElementNode node, double positionWithinGroup, bool discreteColors,
		                                    List<IndividualTwinkleDetails> twinkles = null)
		{
			if (twinkles == null)
				twinkles = GenerateTwinkleData();

			EffectIntents result = new EffectIntents();

			// render the flat 'minimum value' across the entire effect
			//Pulse.Pulse pulse = new Pulse.Pulse();
			//pulse.TargetNodes = new ElementNode[] {node};
			//pulse.TimeSpan = TimeSpan;
			
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
								//pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new [] {value, value}));
								//pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
								//pulseData = pulse.Render();
								pulseData = PulseRenderer.RenderNode(node, new Curve(new PointPairList(new double[] { 0, 100 }, new[] { value, value })), new ColorGradient(colorProportion.Item1), TimeSpan, HasDiscreteColors);
								result.Add(pulseData);
							}
						} else {
							//pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
							//pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(0));
							//pulseData = pulse.Render();
							pulseData = PulseRenderer.RenderNode(node, new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { minPulseValue, minPulseValue })), new ColorGradient(ColorGradient.GetColorAt(0)), TimeSpan, HasDiscreteColors);
							result.Add(pulseData);
						}
						break;

					case TwinkleColorHandling.GradientThroughWholeEffect:
						//pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
						//pulse.ColorGradient = ColorGradient;
						//pulseData = pulse.Render();
						pulseData = PulseRenderer.RenderNode(node, new Curve(new PointPairList(new double[] { 0, 100 }, new [] { minPulseValue, minPulseValue })), ColorGradient, TimeSpan, HasDiscreteColors);
						result.Add(pulseData);
						break;

					case TwinkleColorHandling.StaticColor:
						//pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
						//pulse.ColorGradient = StaticColorGradient;
						//pulseData = pulse.Render();
						pulseData = PulseRenderer.RenderNode(node, new Curve(new PointPairList(new double[] { 0, 100 }, new [] { minPulseValue, minPulseValue })), StaticColorGradient, TimeSpan, HasDiscreteColors);
						result.Add(pulseData);
						break;

					case TwinkleColorHandling.ColorAcrossItems:
						if (discreteColors) {
							List<Tuple<Color, float>> colorsAtPosition = ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
							foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
								double value = minPulseValue * colorProportion.Item2;
								//pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {value, value}));
								//pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
								//pulseData = pulse.Render();
								pulseData = PulseRenderer.RenderNode(node, new Curve(new PointPairList(new double[] { 0, 100 }, new[] { value, value })), new ColorGradient(colorProportion.Item1), TimeSpan, HasDiscreteColors);
								result.Add(pulseData);
							}
						} else {
							//pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {minPulseValue, minPulseValue}));
							//pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup));
							//pulseData = pulse.Render();
							pulseData = PulseRenderer.RenderNode(node, new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { minPulseValue, minPulseValue })), new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup)), TimeSpan, HasDiscreteColors);
							result.Add(pulseData);
						}
						break;
				}
			}


			_colorValueSet.Clear();

			if (!IsElementDiscrete(node))
			{
				_colorValueSet.Add(Color.Empty, new ColorValue[(int)(TimeSpan.TotalMilliseconds / FrameTime)]);
			}
			
			// render all the individual twinkles
			foreach (IndividualTwinkleDetails twinkle in twinkles) {
				{
					var curve = new Curve(new PointPairList(new double[] { 0, 50, 100 }, new[] { twinkle.CurvePoints[0], twinkle.CurvePoints[1], twinkle.CurvePoints[2] }));
					// figure out what color gradient to use for the pulse
					switch (ColorHandling) {
						case TwinkleColorHandling.GradientForEachPulse:
							RenderPulseSegment(twinkle.StartTime, twinkle.Duration, curve, ColorGradient, node);
							break;

						case TwinkleColorHandling.GradientThroughWholeEffect:
							double startPos = (twinkle.StartTime.Ticks/(double) TimeSpan.Ticks);
							double endPos = ((twinkle.StartTime + twinkle.Duration).Ticks/(double) TimeSpan.Ticks);

							if (discreteColors) {
								double range = endPos - startPos;
								if (range <= 0.0) {
									Logging.Error("Twinkle: bad range: " + range + " (SP=" + startPos + ", EP=" + endPos + ")");
									break;
								}

								ColorGradient cg = ColorGradient.GetSubGradientWithDiscreteColors(startPos, endPos);

								foreach (Color color in cg.GetColorsInGradient()) {
									curve = new Curve(curve.Points);
									foreach (PointPair point in curve.Points) {
										double effectRelativePosition = startPos + ((point.X / 100.0) * range);
										double proportion = ColorGradient.GetProportionOfColorAt(effectRelativePosition, color);
										point.Y *= proportion;
									}
									RenderPulseSegment(twinkle.StartTime, twinkle.Duration, curve, new ColorGradient(color), node);
									//pulseData = PulseRenderer.RenderNode(node, curve, new ColorGradient(color), twinkle.Duration, HasDiscreteColors);
									//pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
									//result.Add(pulseData);
									
								}

							} else {
								RenderPulseSegment(twinkle.StartTime, twinkle.Duration, curve, ColorGradient.GetSubGradient(startPos, endPos), node);
							}
							break;

						case TwinkleColorHandling.StaticColor:
							RenderPulseSegment(twinkle.StartTime, twinkle.Duration, curve, StaticColorGradient, node);
							break;

						case TwinkleColorHandling.ColorAcrossItems:
							if (discreteColors) {
								List<Tuple<Color, float>> colorsAtPosition = ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
								foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
									float proportion = colorProportion.Item2;
									// scale all levels of the twinkle curve by the proportion that is applicable to this color
									curve = new Curve(curve.Points);
									foreach (PointPair pointPair in curve.Points) {
										pointPair.Y *= proportion;
									}
									
									RenderPulseSegment(twinkle.StartTime, twinkle.Duration, curve, new ColorGradient(colorProportion.Item1), node);
									//pulseData = PulseRenderer.RenderNode(node, curve, new ColorGradient(colorProportion.Item1), twinkle.Duration, HasDiscreteColors);
									//pulseData.OffsetAllCommandsByTime(twinkle.StartTime);
									//result.Add(pulseData);
								}
							} else {
								RenderPulseSegment(twinkle.StartTime, twinkle.Duration, curve, new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup)), node);
							}
							break;
					}
				}
			}

			foreach (var set in _colorValueSet)
			{
				if (set.Key == Color.Empty)
				{
					var data = CreateIntentForValues(set.Value);
					if (node.IsLeaf)
					{
						result.AddIntentForElement(node.Element.Id, data, TimeSpan.Zero);
					}
					else
					{
						foreach (var leafNode in node.GetLeafEnumerator())
						{
							result.AddIntentForElement(leafNode.Element.Id, data, TimeSpan.Zero);
						}
					}

				}
				else
				{
					var data = CreateDiscreteIntentForValues(set.Value);
					if (node.IsLeaf)
					{
						result.AddIntentForElement(node.Element.Id, data, TimeSpan.Zero);
					}
					else
					{
						foreach (var leafNode in node.GetLeafEnumerator())
						{
							result.AddIntentForElement(leafNode.Element.Id, data, TimeSpan.Zero);
						}
					}
				}
				
			}
			

			return result;
		}

		private struct ColorValue
		{
			public Color c;
			public double i;
		}

		private StaticArrayIntent<LightingValue> CreateIntentForValues(ColorValue[] values)
		{
			var intentValues = values.Select(x => new LightingValue(x.c, x.i));
			var intent = new StaticArrayIntent<LightingValue>(FrameTimespan, intentValues.ToArray(), TimeSpan);
			return intent;
		}

		private StaticArrayIntent<DiscreteValue> CreateDiscreteIntentForValues(ColorValue[] values)
		{
			var intentValues = values.Select(x => new DiscreteValue(x.c, x.i));
			var intent = new StaticArrayIntent<DiscreteValue>(FrameTimespan, intentValues.ToArray(), TimeSpan);
			return intent;
		}

		private void RenderPulseSegment(TimeSpan startTime, TimeSpan duration, Curve c, ColorGradient cg, IElementNode elementNode)
		{
			ColorValue[] values;
			if (HasDiscreteColors && IsElementDiscrete(elementNode))
			{
				IEnumerable<Color> colors = ColorModule.getValidColorsForElementNode(elementNode, false)
					.Intersect(cg.GetColorsInGradient());
				foreach (Color color in colors)
				{
					
					if (!_colorValueSet.TryGetValue(color, out values))
					{
						values = new ColorValue[(int) (TimeSpan.TotalMilliseconds / FrameTime)];
						_colorValueSet.Add(color, values);
					}
					RenderPulseSegment(values, startTime, duration, c, cg, color);
				}
			}
			else
			{
				if (_colorValueSet.TryGetValue(Color.Empty, out values))
				{
					RenderPulseSegment(values, startTime, duration, c, cg);
				}
			}
		}

		private void RenderPulseSegment(ColorValue[] values, TimeSpan startTime, TimeSpan duration, Curve c,
			ColorGradient cg, Color? filterColor = null)
		{
			var intervals = duration.TotalMilliseconds / FrameTime;
			var startOffset = (int)startTime.TotalMilliseconds / FrameTime;
			var endInterval = startOffset + intervals;
			for (int i = startOffset; i <= endInterval; i++)
			{
				if (i >= values.Length) break;
				var currentValue = values[i];
				var samplePoint = (i - startOffset) / intervals;

				if (filterColor.HasValue) //This is a discrete filter color
				{
					//Sample the curve
					var intensity = (cg.GetProportionOfColorAt(samplePoint, (Color)filterColor) * c.GetValue(samplePoint * 100) / 100);
					//Check the intensity
					if (currentValue.i > intensity) continue;
					values[i] = new ColorValue { c = (Color)filterColor, i = intensity };
				}
				else
				{
					//Sample the curve
					var intensity = c.GetValue(samplePoint * 100) / 100;
					//Check the intensity
					if (currentValue.i > intensity) continue;
					//sample the gradient
					var color = cg.GetColorAt(samplePoint);
					values[i] = new ColorValue { c = color, i = intensity };
				}
			}
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
				//Replaced the number generator to a version that provides a better distribution around the mean 
				//to prevent excessive twinkles from being gnerated. JU 
				//double nextTime = Math.Log(1.0 - _random.NextDouble())*-meanMillisecondsBetweenTwinkles;
				double nextTime = RandomGenerator.GetExponential(meanMillisecondsBetweenTwinkles);
				if (nextTime > maxMillisecondsBetweenTwinkles)
					nextTime = maxMillisecondsBetweenTwinkles;

				// check if the timespan will be off the end, if so, bail
				current += TimeSpan.FromMilliseconds(nextTime);
				if (current >= TimeSpan)
					break;

				// generate a time length for it (all in ms)
				int twinkleDurationMs = Rand(AveragePulseTime - maxDurationVariation,
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
				int reduction = Rand(0, maxLevelVariation);
				double maxLevel = MaximumLevel - reduction;

				IndividualTwinkleDetails occurance = new IndividualTwinkleDetails();
				occurance.StartTime = current;
				occurance.Duration = twinkleDuration;
				occurance.CurvePoints = new [] {minLevel*100, maxLevel*100, minLevel*100};
				result.Add(occurance);
				
			}
			return result;
		}

		private List<IElementNode> GetNodesToRenderOn()
		{
			IEnumerable<IElementNode> renderNodes = null;

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

		private struct IndividualTwinkleDetails
		{
			public TimeSpan StartTime;
			public TimeSpan Duration;
			public double[] CurvePoints;
		}
	}
}
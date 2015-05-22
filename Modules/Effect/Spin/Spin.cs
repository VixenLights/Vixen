using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using NLog;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;
using ZedGraph;

namespace VixenModules.Effect.Spin
{
	public class Spin : EffectModuleInstanceBase
	{
		private SpinData _data;
		private EffectIntents _elementData = null;
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		public Spin()
		{
			_data = new SpinData();
			InitAllAttributes();
		}

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			DoRendering(tokenSource);
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
			set
			{
				_data = value as SpinData;
				IsDirty = true;
				InitAllAttributes();
			}
		}

		public override bool IsDirty
		{
			get
			{
				if (!PulseCurve.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				if (!ColorGradient.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		[Value]
		[ProviderCategory(@"Speed",4)]
		[ProviderDisplayName(@"SpeedFormat")]
		[ProviderDescription(@"SpinSpeedFormat")]
		[PropertyOrder(1)]
		public SpinSpeedFormat SpeedFormat
		{
			get { return _data.SpeedFormat; }
			set
			{
				_data.SpeedFormat = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateSpeedFormatAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[ProviderCategory(@"Pulse",5)]
		[ProviderDisplayName(@"PulseType")]
		[ProviderDescription(@"PulseType")]
		[PropertyOrder(1)]
		public SpinPulseLengthFormat PulseLengthFormat
		{
			get { return _data.PulseLengthFormat; }
			set
			{
				_data.PulseLengthFormat = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdatePulseLengthFormatAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[ProviderCategory(@"Color",1)]
		[ProviderDisplayName(@"ColorHandling")]
		[ProviderDescription(@"ColorHandling")]
		[PropertyOrder(1)]
		public SpinColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set
			{
				_data.ColorHandling = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateColorHandlingAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[ProviderCategory(@"Speed",4)]
		[ProviderDisplayName(@"RevolutionCount")]
		[ProviderDescription(@"RevolutionCount")]
		[PropertyOrder(3)]
		public double RevolutionCount
		{
			get { return _data.RevolutionCount; }
			set
			{
				_data.RevolutionCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Speed",4)]
		[ProviderDisplayName(@"RevolutionFrequency")]
		[ProviderDescription(@"RevolutionFrequency")]
		[PropertyOrder(4)]
		public double RevolutionFrequency
		{
			get { return _data.RevolutionFrequency; }
			set
			{
				_data.RevolutionFrequency = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Speed",4)]
		[ProviderDisplayName(@"RevolutionTime")]
		[ProviderDescription(@"RevolutionTime")]
		[PropertyOrder(5)]
		public int RevolutionTime
		{
			get { return _data.RevolutionTime; }
			set
			{
				_data.RevolutionTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Pulse",5)]
		[ProviderDisplayName(@"PulseDuration")]
		[ProviderDescription(@"PulseDuration")]
		[PropertyOrder(2)]
		public int PulseTime
		{
			get { return _data.PulseTime; }
			set
			{
				_data.PulseTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Pulse",5)]
		[ProviderDisplayName(@"PulsePercent")]
		[ProviderDescription(@"PulseSpinPercent")]
		[PropertyEditor("SliderEditor")]
		[PropertyOrder(3)]
		public int PulsePercentage
		{
			get { return _data.PulsePercentage; }
			set
			{
				_data.PulsePercentage = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness",2)]
		[ProviderDisplayName(@"DefaultBrightness")]
		[ProviderDescription(@"DefaultBrightness")]
		[PropertyEditor("LevelEditor")]
		[PropertyOrder(2)]
		public double DefaultLevel
		{
			get { return _data.DefaultLevel; }
			set
			{
				_data.DefaultLevel = value;
				IsDirty = true;
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
				CheckForInvalidColorData();
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
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(3)]
		public ColorGradient ColorGradient
		{
			get
			{
				//CheckForInvalidColorData();
				return _data.ColorGradient;
			}
			set
			{
				_data.ColorGradient = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		//Created to hold a ColorGradient version of color rather than continually creating them from Color for static colors.
		protected ColorGradient StaticColorGradient
		{
			get { return _data.StaticColorGradient; }
			set { _data.StaticColorGradient = value; }
		}

		[Value]
		[ProviderCategory(@"Brightness",2)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"PulseShape")]
		[PropertyOrder(1)]
		public Curve PulseCurve
		{
			get { return _data.PulseCurve; }
			set
			{
				_data.PulseCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Direction",3)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[TypeConverter(typeof(BooleanStringTypeConverter))]
		[BoolDescription("Forward", "Reverse")]
		[PropertyEditor("SelectionEditor")]
		public bool ReverseSpin
		{
			get { return _data.ReverseSpin; }
			set
			{
				_data.ReverseSpin = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Depth",20)]
		[ProviderDisplayName(@"Depth")]
		[ProviderDescription(@"Depth")]
		[TypeConverter(typeof(TargetElementDepthConverter))]
		[PropertyEditor("SelectionEditor")]
		[MergableProperty(false)]
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

		#region Attributes

		private void InitAllAttributes()
		{
			UpdateColorHandlingAttributes();
			UpdateSpeedFormatAttributes();
			UpdatePulseLengthFormatAttributes();
			TypeDescriptor.Refresh(this);
		}


		private void UpdateColorHandlingAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"StaticColor", ColorHandling.Equals(SpinColorHandling.StaticColor)},
				{"ColorGradient", !ColorHandling.Equals(SpinColorHandling.StaticColor)}
			};
			SetBrowsable(propertyStates);
		}

		private void UpdateSpeedFormatAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{"RevolutionCount", SpeedFormat.Equals(SpinSpeedFormat.RevolutionCount)},
				{"RevolutionTime", SpeedFormat.Equals(SpinSpeedFormat.FixedTime)},
				{"RevolutionFrequency", SpeedFormat.Equals(SpinSpeedFormat.RevolutionFrequency)}
			};
			SetBrowsable(propertyStates);
		}

		private void UpdatePulseLengthFormatAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"PulseTime", PulseLengthFormat.Equals(SpinPulseLengthFormat.FixedTime)},
				{"PulsePercentage", PulseLengthFormat.Equals(SpinPulseLengthFormat.PercentageOfRevolution)}
			};
			SetBrowsable(propertyStates);
		}

		#endregion

		private void DoRendering(CancellationTokenSource tokenSource = null)
		{
			//TODO: get a better increment time. doing it every X ms is..... shitty at best.
			TimeSpan increment = TimeSpan.FromMilliseconds(10);

			List<ElementNode> renderNodes = GetNodesToRenderOn();
			int targetNodeCount = renderNodes.Count;
			
			//If there are no nodes to render on Exit!
			if (targetNodeCount == 0) return;

			ElementNode lastTargetedNode = null;

			Pulse.Pulse pulse;
			EffectIntents pulseData;

			// apply the 'background' values to all targets if nonzero
			if (DefaultLevel > 0) {
				int i = 0;
				foreach (ElementNode target in renderNodes)
				{
					if (tokenSource != null && tokenSource.IsCancellationRequested) return;

					bool discreteColors = ColorModule.isElementNodeDiscreteColored(target);

					if (target == null)
						continue;

					if (target != null) {
						pulse = new Pulse.Pulse();
						pulse.TargetNodes = new ElementNode[] {target};
						pulse.TimeSpan = TimeSpan;
						double level = DefaultLevel*100.0;

						// figure out what color gradient to use for the pulse
						switch (ColorHandling) {
							case SpinColorHandling.GradientForEachPulse:
								pulse.ColorGradient = StaticColorGradient;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {level, level}));
								pulseData = pulse.Render();
								_elementData.Add(pulseData);
								break;

							case SpinColorHandling.GradientThroughWholeEffect:
								pulse.ColorGradient = ColorGradient;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {level, level}));
								pulseData = pulse.Render();
								_elementData.Add(pulseData);
								break;

							case SpinColorHandling.StaticColor:
								pulse.ColorGradient = StaticColorGradient;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {level, level}));
								pulseData = pulse.Render();
								_elementData.Add(pulseData);
								break;

							case SpinColorHandling.ColorAcrossItems:
								double positionWithinGroup = i/(double) targetNodeCount;
								if (discreteColors) {
									List<Tuple<Color, float>> colorsAtPosition =
										ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
									foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
										double value = level*colorProportion.Item2;
										pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {value, value}));
										pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
										pulseData = pulse.Render();
										_elementData.Add(pulseData);
									}
								}
								else {
									pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup));
									pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {level, level}));
									pulseData = pulse.Render();
									_elementData.Add(pulseData);
								}
								break;
						}

						i++;
					}
				}
			}

			// calculate the pulse time and revolution time exactly (based on the parameters from the data)
			double revTimeMs = 0; // single revolution time (ms)

			// figure out the relative length of a individual pulse
			double pulseConstant = 0; // how much of each pulse is a constant time
			double pulseFractional = 0; // how much of each pulse is a fraction of a single spin
			if (PulseLengthFormat == SpinPulseLengthFormat.FixedTime) {
				pulseConstant = PulseTime;
			}
			else if (PulseLengthFormat == SpinPulseLengthFormat.PercentageOfRevolution) {
				pulseFractional = PulsePercentage/100.0;
			}
			else if (PulseLengthFormat == SpinPulseLengthFormat.EvenlyDistributedAcrossSegments) {
				pulseFractional = 1.0/(double) targetNodeCount;
			}

			// magic number. (the inaccuracy of interpolating the curve into a position. eg. if we have 5 'positions', then
			// the curve should really be from 0-80% for the last spin, to make sure the last pulse finishes accurately.)
			double pulseInterpolationOffset = 1.0/(double) targetNodeCount;

			// figure out either the revolution count or time, based on what data we have
			if (SpeedFormat == SpinSpeedFormat.RevolutionCount) {
				revTimeMs = (TimeSpan.TotalMilliseconds - pulseConstant)/
				            (RevolutionCount + pulseFractional - pulseInterpolationOffset);
			}
			else if (SpeedFormat == SpinSpeedFormat.RevolutionFrequency) {
				revTimeMs = (1.0/RevolutionFrequency)*1000.0; // convert Hz to period ms
			}
			else if (SpeedFormat == SpinSpeedFormat.FixedTime) {
				revTimeMs = RevolutionTime;
			}

			double pulTimeMs = pulseConstant + (revTimeMs*pulseFractional);

			TimeSpan revTimeSpan = TimeSpan.FromMilliseconds(revTimeMs);
			TimeSpan pulseTimeSpan = TimeSpan.FromMilliseconds(pulTimeMs);

			// figure out which way we're moving through the elements
			Curve movement;
			if (ReverseSpin)
				movement = new Curve(new PointPairList(new double[] {0, 100}, new double[] {100, 0}));
			else
				movement = new Curve(new PointPairList(new double[] {0, 100}, new double[] {0, 100}));

			// iterate up to and including the last pulse generated
			// a bit iffy, but stops 'carry over' spins past the end (when there's overlapping spins). But we need to go past
			// (total - pulse) as the last pulse can often be a bit inaccurate due to the rounding of the increment
			for (TimeSpan current = TimeSpan.Zero; current <= TimeSpan - pulseTimeSpan + increment; current += increment) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;

				double currentPercentageIntoSpin = ((double) (current.Ticks%revTimeSpan.Ticks)/(double) revTimeSpan.Ticks)*100.0;

				double targetElementPosition = movement.GetValue(currentPercentageIntoSpin);
				int currentNodeIndex = (int) ((targetElementPosition/100.0)*targetNodeCount);

				// on the off chance we hit the 100% mark *exactly*...
				if (currentNodeIndex == targetNodeCount)
					currentNodeIndex--;

				if (currentNodeIndex >= targetNodeCount) {
					Logging.Warn(
						"Spin effect: rendering, but the current node index is higher or equal to the total target nodes.");
					continue;
				}

				ElementNode currentNode = renderNodes[currentNodeIndex];
				if (currentNode == lastTargetedNode)
					continue;

				bool discreteColors = ColorModule.isElementNodeDiscreteColored(currentNode);

				// make a pulse for it
				pulse = new Pulse.Pulse();
				pulse.TargetNodes = new ElementNode[] {currentNode};
				pulse.TimeSpan = pulseTimeSpan;
				pulse.LevelCurve = new Curve(PulseCurve);

				// figure out what color gradient to use for the pulse
				switch (ColorHandling) {
					case SpinColorHandling.GradientForEachPulse:
						pulse.ColorGradient = ColorGradient;
						pulseData = pulse.Render();
						pulseData.OffsetAllCommandsByTime(current);
						_elementData.Add(pulseData);
						break;

					case SpinColorHandling.GradientThroughWholeEffect:
						double startPos = ((double) current.Ticks/(double) TimeSpan.Ticks);
						double endPos = 1.0;
						if (TimeSpan - current >= pulseTimeSpan)
							endPos = ((double)(current + pulseTimeSpan).Ticks / (double)TimeSpan.Ticks);

						if (discreteColors) {
							double range = endPos - startPos;
							if (range <= 0.0) {
								Logging.Error("Spin: bad range: " + range + " (SP=" + startPos + ", EP=" + endPos + ")");
								break;
							}

							ColorGradient cg = ColorGradient.GetSubGradientWithDiscreteColors(startPos, endPos);

							foreach (Color color in cg.GetColorsInGradient()) {
								if (tokenSource != null && tokenSource.IsCancellationRequested)
									return;

								Curve newCurve = new Curve(pulse.LevelCurve.Points);
								foreach (PointPair point in newCurve.Points) {
									double effectRelativePosition = startPos + ((point.X / 100.0) * range);
									double proportion = ColorGradient.GetProportionOfColorAt(effectRelativePosition, color);
									point.Y *= proportion;
								}
								pulse.LevelCurve = newCurve;
								pulse.ColorGradient = new ColorGradient(color);
								pulseData = pulse.Render();
								pulseData.OffsetAllCommandsByTime(current);
								_elementData.Add(pulseData);
							}
						} else {
							pulse.ColorGradient = ColorGradient.GetSubGradient(startPos, endPos);
							pulseData = pulse.Render();
							pulseData.OffsetAllCommandsByTime(current);
							_elementData.Add(pulseData);
						}
						break;

					case SpinColorHandling.StaticColor:
						pulse.ColorGradient = StaticColorGradient;
						pulseData = pulse.Render();
						pulseData.OffsetAllCommandsByTime(current);
						_elementData.Add(pulseData);
						break;

					case SpinColorHandling.ColorAcrossItems:
						if (discreteColors) {
							List<Tuple<Color, float>> colorsAtPosition = ColorGradient.GetDiscreteColorsAndProportionsAt(targetElementPosition / 100.0);
							foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
								if (tokenSource != null && tokenSource.IsCancellationRequested)
									return;

								float proportion = colorProportion.Item2;
								// scale all levels of the pulse curve by the proportion that is applicable to this color
								Curve newCurve = new Curve(pulse.LevelCurve.Points);
								foreach (PointPair pointPair in newCurve.Points) {
									pointPair.Y *= proportion;
								}
								pulse.LevelCurve = newCurve;
								pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
								pulseData = pulse.Render();
								pulseData.OffsetAllCommandsByTime(current);
								_elementData.Add(pulseData);
							}
						} else {
							pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(targetElementPosition/100.0));
							pulseData = pulse.Render();
							pulseData.OffsetAllCommandsByTime(current);
							_elementData.Add(pulseData);
						}
						break;
				}

				lastTargetedNode = currentNode;
			}

			_elementData = EffectIntents.Restrict(_elementData, TimeSpan.Zero, TimeSpan);
		}

		private List<ElementNode> GetNodesToRenderOn()
		{
			IEnumerable<ElementNode> renderNodes = null;

			if (DepthOfEffect == 0) {
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator()).ToList();
			}
			else {
				renderNodes = TargetNodes;
				for (int i = 0; i < DepthOfEffect; i++) {
					renderNodes = renderNodes.SelectMany(x => x.Children);
				}
			}

			// If the given DepthOfEffect results in no nodes (because it goes "too deep" and misses all nodes), 
			// then we'll default to the LeafElements, which will at least return 1 element (the TargetNode)
			if (!renderNodes.Any())
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());

			return renderNodes.ToList();
		}
	}
}
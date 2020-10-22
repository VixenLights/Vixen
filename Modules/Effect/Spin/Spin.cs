using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using NLog;
using Vixen.Attributes;
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

namespace VixenModules.Effect.Spin
{
	public class Spin : BaseEffect
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
			if (TargetNodes.Any())
			{
				if (TargetNodes.Length > 1)
				{
					DepthOfEffect = 0;
				}
				else
				{ 
					CheckForInvalidColorData(); 
					var firstNode = TargetNodes.FirstOrDefault();
					if (firstNode != null && DepthOfEffect > firstNode.GetMaxChildDepth() - 1)
					{
						DepthOfEffect = 0;
					}
				}
			}
			UpdateTargetingAttributes();
			TypeDescriptor.Refresh(this);
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			if (TargetNodeHandling == TargetNodeSelection.Group)
			{
				if (TargetNodes.Length == 1)
				{
					var renderNodes = GetNodesToRenderOn(TargetNodes.First());
					DoRendering(renderNodes, tokenSource);
				}
				else
				{
					DoRendering(TargetNodes.ToList(), tokenSource);
				}
				
			}
			else 
			{
				if (TargetNodes.Length == 1)
				{
					var targetNodes = GetNodesToRenderOn(TargetNodes.First());
					foreach (var elementNode in targetNodes)
					{
						var renderNodes = GetNodesToRenderOn(elementNode);
						DoRendering(renderNodes, tokenSource);
					}
				}
				else
				{
					foreach (var elementNode in TargetNodes)
					{
						var renderNodes = GetNodesToRenderOn(elementNode);
						DoRendering(renderNodes, tokenSource);
					}
				}
			}
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();

			if (validColors.Any())
			{
				if (!validColors.Contains(_data.StaticColor) || !_data.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
					//Discrete colors specified
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
				_data = value as SpinData;
				CheckForInvalidColorData();
				IsDirty = true;
				InitAllAttributes();
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
		[ProviderCategory(@"Behavior", 0)]
		[ProviderDisplayName(@"SpinTargetNodeSelection")]
		[ProviderDescription(@"SpinTargetNodeSelection")]
		public TargetNodeSelection TargetNodeHandling
		{
			get => _data.TargetNodeSelection;
			set
			{
				_data.TargetNodeSelection = value;
				IsDirty = true;
				OnPropertyChanged();
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
		[ProviderCategory(@"Brightness", 2)]
		[ProviderDisplayName(@"EnableMinimumBrightness")]
		[ProviderDescription(@"EnableMinimumBrightness")]
		[PropertyOrder(2)]
		public bool EnableDefaultLevel
		{
			get { return _data.EnableDefaultLevel; }
			set
			{
				_data.EnableDefaultLevel = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateDefaultLevelAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[ProviderCategory(@"Brightness",2)]
		[ProviderDisplayName(@"MinimumBrightness")]
		[ProviderDescription(@"MinimumBrightness")]
		[PropertyEditor("LevelEditor")]
		[PropertyOrder(3)]
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
		[ProviderDisplayName(@"ColorGradient")]
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
		[ProviderDisplayName(@"PulseShape")]
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

		public override string Information
		{
			get { return "See the Vixen Lights website for more information on layering with this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/spin/"; }
		}

		#region Attributes

		private void InitAllAttributes()
		{
			UpdateColorHandlingAttributes();
			UpdateSpeedFormatAttributes();
			UpdatePulseLengthFormatAttributes();
			UpdateDefaultLevelAttributes();
			UpdateTargetingAttributes();
			TypeDescriptor.Refresh(this);
		}
		
		private void UpdateTargetingAttributes()
		{
			var depth = DetermineDepth();
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add(nameof(TargetNodeHandling), TargetNodes.Length > 1 || depth > 2);
			propertyStates.Add(nameof(DepthOfEffect), depth > 2);
			SetBrowsable(propertyStates);
		}

		private void UpdateDefaultLevelAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("DefaultLevel", EnableDefaultLevel);
			SetBrowsable(propertyStates);
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

		private void DoRendering(List<IElementNode> renderNodes, CancellationTokenSource tokenSource = null)
		{
			int targetNodeCount = renderNodes.Count;
			
			//If there are no nodes to render on Exit!
			if (targetNodeCount == 0) return;

			IElementNode lastTargetedNode = null;

			//Pulse.Pulse pulse;
			EffectIntents pulseData;

			// apply the 'background' values to all targets if nonzero
			if (EnableDefaultLevel) {
				int i = 0;
				foreach (IElementNode target in renderNodes)
				{
					if (tokenSource != null && tokenSource.IsCancellationRequested) return;

					bool discreteColors = ColorModule.isElementNodeDiscreteColored(target);

					if (target == null)
						continue;

					if (target != null) {
						double level = DefaultLevel*100.0;

						// figure out what color gradient to use for the pulse
						switch (ColorHandling) {
							case SpinColorHandling.GradientForEachPulse:
								pulseData = PulseRenderer.RenderNode(target,
									new Curve(new PointPairList(new double[] {0, 100}, new [] {level, level})), StaticColorGradient, TimeSpan, HasDiscreteColors, true);
								_elementData.Add(pulseData);
								break;

							case SpinColorHandling.GradientThroughWholeEffect:
								pulseData = PulseRenderer.RenderNode(target,
									new Curve(new PointPairList(new double[] { 0, 100 }, new [] { level, level })), ColorGradient, TimeSpan, HasDiscreteColors, true);
								_elementData.Add(pulseData);
								break;

							case SpinColorHandling.StaticColor:
								pulseData = PulseRenderer.RenderNode(target,
									new Curve(new PointPairList(new double[] {0, 100}, new[] {level, level})), StaticColorGradient, TimeSpan, HasDiscreteColors, true);
								_elementData.Add(pulseData);
								break;

							case SpinColorHandling.ColorAcrossItems:
								double positionWithinGroup = i/(double) targetNodeCount;
								if (discreteColors) {
									List<Tuple<Color, float>> colorsAtPosition =
										ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
									foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
										double value = level*colorProportion.Item2;
										pulseData = PulseRenderer.RenderNode(target,
											new Curve(new PointPairList(new double[] { 0, 100 }, new [] { value, value })), new ColorGradient(colorProportion.Item1), TimeSpan, HasDiscreteColors, true);
										_elementData.Add(pulseData);
									}
								}
								else {
									pulseData = PulseRenderer.RenderNode(target,
											new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { level, level })), new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup)), TimeSpan, HasDiscreteColors, true);
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
			if (SpeedFormat == SpinSpeedFormat.RevolutionCount)
			{
				var t = (RevolutionCount + pulseFractional - pulseInterpolationOffset);
				if (t <= 0)
				{
					t = RevolutionCount > 0?RevolutionCount:1;
				}
				revTimeMs = (TimeSpan.TotalMilliseconds - pulseConstant) / t;
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

			//TODO: get a better increment time. doing it every X ms is..... shitty at best.
			//Less crappy is try to make some adjustment if there are a lot of nodes in a shorter time to sample more often. 
			//A hard and fast 2ms was leaving gaps in larger node counts
			var sampleMs = revTimeSpan.TotalMilliseconds / targetNodeCount / 2.0;
			if (sampleMs < .25)
			{
				sampleMs = .25;
			}
			else if (sampleMs > 2)
			{
				sampleMs = 2;
			}
			TimeSpan increment = TimeSpan.FromTicks((long)(sampleMs * TimeSpan.TicksPerMillisecond));

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

				IElementNode currentNode = renderNodes[currentNodeIndex];
				if (currentNode == lastTargetedNode)
					continue;

				bool discreteColors = ColorModule.isElementNodeDiscreteColored(currentNode);

				// figure out what color gradient to use for the pulse
				switch (ColorHandling) {
					case SpinColorHandling.GradientForEachPulse:
						pulseData = PulseRenderer.RenderNode(currentNode, new Curve(PulseCurve), ColorGradient, pulseTimeSpan, HasDiscreteColors);
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
								Curve newCurve = new Curve(PulseCurve.Points);
								foreach (PointPair point in newCurve.Points)
								{
									double effectRelativePosition = startPos + ((point.X / 100.0) * range);
									double proportion = ColorGradient.GetProportionOfColorAt(effectRelativePosition, color);
									point.Y *= proportion;
								}
								pulseData = PulseRenderer.RenderNode(currentNode, newCurve, new ColorGradient(color), pulseTimeSpan, HasDiscreteColors);
								pulseData.OffsetAllCommandsByTime(current);
								_elementData.Add(pulseData);
							}
						} else {
							pulseData = PulseRenderer.RenderNode(currentNode, new Curve(PulseCurve), ColorGradient.GetSubGradient(startPos, endPos), pulseTimeSpan, HasDiscreteColors);
							pulseData.OffsetAllCommandsByTime(current);
							_elementData.Add(pulseData);
						}
						break;

					case SpinColorHandling.StaticColor:
						pulseData = PulseRenderer.RenderNode(currentNode, new Curve(PulseCurve), StaticColorGradient, pulseTimeSpan, HasDiscreteColors);
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

								Curve newCurve = new Curve(PulseCurve.Points);
								foreach (PointPair pointPair in newCurve.Points)
								{
									pointPair.Y *= proportion;
								}
								pulseData = PulseRenderer.RenderNode(currentNode, newCurve, new ColorGradient(colorProportion.Item1), pulseTimeSpan, HasDiscreteColors);
								pulseData.OffsetAllCommandsByTime(current);
								_elementData.Add(pulseData);
							}
						} else {
							pulseData = PulseRenderer.RenderNode(currentNode, new Curve(PulseCurve), new ColorGradient(ColorGradient.GetColorAt(targetElementPosition / 100.0)), pulseTimeSpan, HasDiscreteColors);
							pulseData.OffsetAllCommandsByTime(current);
							_elementData.Add(pulseData);
						}
						break;
				}

				lastTargetedNode = currentNode;
			}

			_elementData = EffectIntents.Restrict(_elementData, TimeSpan.Zero, TimeSpan);
		}

		private List<IElementNode> GetNodesToRenderOn(IElementNode node)
		{
			IEnumerable<IElementNode> renderNodes = null;

			if (DepthOfEffect == 0) {
				renderNodes = node.GetLeafEnumerator().ToList();
			}
			else 
			{
				renderNodes = new []{node};
				for (int i = 0; i < DepthOfEffect; i++) {
					renderNodes = renderNodes.SelectMany(x => x.Children);
				}
			}

			// If the given DepthOfEffect results in no nodes (because it goes "too deep" and misses all nodes), 
			// then we'll default to the LeafElements, which will at least return 1 element (the TargetNode)
			if (!renderNodes.Any())
				renderNodes = node.GetLeafEnumerator();

			return renderNodes.ToList();
		}
	}
}
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

namespace VixenModules.Effect.Chase
{
	public class Chase : BaseEffect
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private ChaseData _data;
		private EffectIntents _elementData;

		public Chase()
		{
			_data = new ChaseData();
			InitAllAttributes();
		}

		protected override void TargetNodesChanged()
		{
			if (TargetNodes.Any())
			{
				CheckForInvalidColorData();

				if (TargetNodes.Length > 1)
				{
					DepthOfEffect = 0;
				}
				else
				{
					var firstNode = TargetNodes.FirstOrDefault();
					if (firstNode != null && DepthOfEffect > firstNode.GetMaxChildDepth() - 1)
					{
						DepthOfEffect = 0;
					}
				}
				
				UpdateTargetingAttributes();
				TypeDescriptor.Refresh(this);
			}
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
			//_elementData = IntentBuilder.ConvertToStaticArrayIntents(_elementData, TimeSpan, IsDiscrete());
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
				_data = value as ChaseData;
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

				if (!ChaseMovement.CheckLibraryReference())
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
		[ProviderCategory(@"Behavior", 0)]
		[ProviderDisplayName(@"ChaseTargetNodeSelection")]
		[ProviderDescription(@"ChaseTargetNodeSelection")]
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
		public ChaseColorHandling ColorHandling
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
		[ProviderCategory(@"Color", 1)]
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
		[PropertyOrder(3)]
		[PropertyEditor("LevelEditor")]
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
		[ProviderCategory(@"Pulse", 4)]
		[ProviderDisplayName(@"PulseOverlap")]
		[ProviderDescription(@"PulseOverlap")]
		public int PulseOverlap
		{
			get { return _data.PulseOverlap; }
			set
			{
				_data.PulseOverlap = value;
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
		public Curve ChaseMovement
		{
			get { return _data.ChaseMovement; }
			set
			{
				_data.ChaseMovement = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Depth",5)]
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

		[Value]
		[ProviderCategory(@"Pulse",4)]
		[ProviderDisplayName(@"ExtendPulseStart")]
		[ProviderDescription(@"ExtendPulseStart")]
		public bool ExtendPulseToStart
		{
			get { return _data.ExtendPulseToStart; }
			set
			{
				_data.ExtendPulseToStart = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Pulse",4)]
		[ProviderDisplayName(@"ExtendPulseEnd")]
		[ProviderDescription(@"ExtendPulseEnd")]
		public bool ExtendPulseToEnd
		{
			get { return _data.ExtendPulseToEnd; }
			set
			{
				_data.ExtendPulseToEnd = value;
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/chase/"; }
		}

		#region Attributes

		private void InitAllAttributes()
		{
			UpdateColorHandlingAttributes();
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

		private void UpdateColorHandlingAttributes()
		{
			Dictionary<string,bool> propertyStates = new Dictionary<string, bool>(2); 
			propertyStates.Add("StaticColor", ColorHandling.Equals(ChaseColorHandling.StaticColor));
			propertyStates.Add("ColorGradient", !ColorHandling.Equals(ChaseColorHandling.StaticColor));
			SetBrowsable(propertyStates);
		}

		private void UpdateDefaultLevelAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("DefaultLevel", EnableDefaultLevel);
			SetBrowsable(propertyStates);
		}

		#endregion

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();

			if (validColors.Any())
			{
				if(!validColors.Contains(_data.StaticColor) || !_data.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
					//Discrete colors specified
				{
					_data.ColorGradient = new ColorGradient(validColors.DefaultIfEmpty(Color.White).First());

					//Set a default color 
					_data.StaticColor = validColors.First();
				}
			}
		}


		private void DoRendering(List<IElementNode> renderNodes, CancellationTokenSource tokenSource = null)
		{
			int targetNodeCount = renderNodes.Count;

			// apply the 'background' values to all targets if the level is supposed to be enabled
			if (EnableDefaultLevel) {
				int i = 0;
				foreach (IElementNode target in renderNodes) {
					if (tokenSource != null && tokenSource.IsCancellationRequested) return;
					
					if (target != null)
					{
						bool discreteColors = ColorModule.isElementNodeDiscreteColored(target);

						double level = DefaultLevel*100.0;

						// figure out what color gradient to use for the pulse
						EffectIntents pulseData;
						switch (ColorHandling) {
							case ChaseColorHandling.GradientForEachPulse:
								pulseData = PulseRenderer.RenderNode(target,
									new Curve(new PointPairList(new double[] { 0, 100 }, new[] { level, level })), StaticColorGradient, TimeSpan, HasDiscreteColors, true);
								_elementData.Add(pulseData);
								break;

							case ChaseColorHandling.GradientThroughWholeEffect:
								pulseData = PulseRenderer.RenderNode(target,
									new Curve(new PointPairList(new double[] { 0, 100 }, new[] { level, level })), ColorGradient, TimeSpan, HasDiscreteColors, true);
								_elementData.Add(pulseData);
								break;

							case ChaseColorHandling.StaticColor:
								pulseData = PulseRenderer.RenderNode(target,
									new Curve(new PointPairList(new double[] { 0, 100 }, new[] { level, level })), StaticColorGradient, TimeSpan, HasDiscreteColors, true);
								_elementData.Add(pulseData);
								break;

							case ChaseColorHandling.ColorAcrossItems:
								double positionWithinGroup = i/(double) targetNodeCount;
								if (discreteColors) {
									List<Tuple<Color, float>> colorsAtPosition =
										ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
									foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
										if (tokenSource != null && tokenSource.IsCancellationRequested)
											return;
										double value = level*colorProportion.Item2;
										pulseData = PulseRenderer.RenderNode(target,
											new Curve(new PointPairList(new double[] { 0, 100 }, new[] { value, value })), new ColorGradient(colorProportion.Item1), TimeSpan, HasDiscreteColors, true);
										_elementData.Add(pulseData);
									}
								}
								else {
									pulseData = PulseRenderer.RenderNode(target,
											new Curve(new PointPairList(new double[] { 0, 100 }, new [] { level, level })), new ColorGradient(ColorGradient.GetColorAt(positionWithinGroup)), TimeSpan, HasDiscreteColors, true);
									_elementData.Add(pulseData);
								}
								break;
						}

						i++;
					}
				}
			}

			

			// the total chase time
			TimeSpan chaseTime = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds - PulseOverlap);
			if (chaseTime.TotalMilliseconds <= 0)
				chaseTime = TimeSpan.FromMilliseconds(1);

			//TODO: get a better increment time. doing it every X ms is..... shitty at best.
			//Less crappy is try to make some adjustment if there are a lot of nodes in a shorter time to sample more often. 
			//A hard and fast 2ms was leaving gaps in larger node counts
			var sampleMs = chaseTime.TotalMilliseconds/targetNodeCount/2.0;
			if (sampleMs < .25)
			{
				sampleMs = .25;
			}
			else if (sampleMs > 2)
			{
				sampleMs = 2;
			}
			TimeSpan increment = TimeSpan.FromTicks((long)(sampleMs*TimeSpan.TicksPerMillisecond));

			// we need to keep track of the element that is 'under' the curve at a given time, to see if it changes,
			// and when it does, we make the effect for it then (since it's a variable time pulse).
			IElementNode lastTargetedNode = null;
			TimeSpan lastNodeStartTime = TimeSpan.Zero;

			// iterate up to and including the last pulse generated
			for (TimeSpan current = TimeSpan.Zero; current <= TimeSpan; current += increment) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;
				double currentPercentageIntoChase = (current.Ticks/(double) chaseTime.Ticks)*100.0;

				double currentMovementPosition = ChaseMovement.GetValue(currentPercentageIntoChase);
				int currentNodeIndex = (int) ((currentMovementPosition/100.0)*targetNodeCount);

				// on the off chance we hit the 100% mark *exactly*...
				if (currentNodeIndex == targetNodeCount && currentNodeIndex > 0)
					currentNodeIndex--;

				if (currentNodeIndex >= targetNodeCount) {
					Logging.Warn(
						"Chase effect: rendering, but the current node index is higher or equal to the total target nodes.");
					continue;
				}

				if (currentNodeIndex < 0)
					continue;

				IElementNode currentNode = renderNodes[currentNodeIndex];
				if (currentNode == lastTargetedNode)
					continue;

				// if the last node targeted wasn't null, we need to make a pulse for it
				if (lastTargetedNode != null)
				{
					TimeSpan duration = current - lastNodeStartTime + TimeSpan.FromMilliseconds(PulseOverlap);
					GeneratePulse(lastTargetedNode, lastNodeStartTime, duration
					              , currentMovementPosition);
				}

				lastTargetedNode = currentNode;
				lastNodeStartTime = current;

				// if we've hit the 100% mark of the chase curve, bail (the last one gets generated after)
				if (currentPercentageIntoChase >= 100.0)
					break;
			}

			// generate the last pulse
			if (lastTargetedNode != null) {
				GeneratePulse(lastTargetedNode, lastNodeStartTime, TimeSpan - lastNodeStartTime, ChaseMovement.GetValue(100));
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
				renderNodes = new[] {node};
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

		//private void GenerateExtendedStaticPulse(ElementNode target, IIntentNode intentNode, ColorGradient gradient=null)
		//{
		//	if (intentNode == null || intentNode.EndTime >= TimeSpan) return;
		//	var lightingIntent = intentNode.Intent as LightingIntent;
		//	if (lightingIntent != null && lightingIntent.EndValue.Intensity > 0)
		//	{
		//		var newCurve = new Curve(lightingIntent.EndValue.Intensity*100);
		//		GenerateExtendedStaticPulse(target, newCurve, gradient ?? new ColorGradient(lightingIntent.EndValue.FullColor),
		//			TimeSpan - intentNode.EndTime, intentNode.EndTime);
		//	}
		//	else
		//	{
		//		var discreteIntent = intentNode.Intent as DiscreteLightingIntent;
		//		if (discreteIntent != null && discreteIntent.EndValue.Intensity > 0)
		//		{
		//			var newCurve = new Curve(discreteIntent.EndValue.Intensity*100);
		//			GenerateExtendedStaticPulse(target, newCurve, gradient ?? new ColorGradient(discreteIntent.EndValue.FullColor),
		//				TimeSpan - intentNode.EndTime, intentNode.EndTime);
		//		}
		//	}
		//}

		//private void GenerateExtendedStaticPulse(ElementNode target, Curve newCurve, ColorGradient gradient, TimeSpan duration, TimeSpan offset)
		//{
		//	var result = PulseRenderer.RenderNode(target, newCurve, gradient, duration, HasDiscreteColors);
		//	result.OffsetAllCommandsByTime(offset);
		//	_elementData.Add(result);
		//}

		//private void GenerateStartingStaticPulse(ElementNode target, IIntentNode intentNode, ColorGradient gradient=null)
		//{
		//	if (intentNode == null || intentNode.StartTime <= TimeSpan.Zero) return;
		//	var lightingIntent = intentNode.Intent as LightingIntent;
		//	if (lightingIntent != null && lightingIntent.StartValue.Intensity > 0)
		//	{
		//		var newCurve = new Curve(lightingIntent.StartValue.Intensity*100);
		//		GenerateStartingStaticPulse(target,newCurve, gradient ?? new ColorGradient(lightingIntent.StartValue.FullColor), intentNode.StartTime);
		//	}
		//	else
		//	{
		//		var discreteIntent = intentNode.Intent as DiscreteLightingIntent;
		//		if (discreteIntent != null && discreteIntent.StartValue.Intensity > 0)
		//		{
		//			var newCurve = new Curve(discreteIntent.StartValue.Intensity*100);
		//			GenerateStartingStaticPulse(target, newCurve, gradient ?? new ColorGradient(discreteIntent.StartValue.FullColor),
		//				intentNode.StartTime);
		//		}
		//	}
		//}

		//private void GenerateStartingStaticPulse(ElementNode target, Curve newCurve, ColorGradient gradient, TimeSpan time)
		//{
		//	var result = PulseRenderer.RenderNode(target, newCurve, gradient, time, HasDiscreteColors);
		//	_elementData.Add(result);
		//}

		private void GeneratePulse(IElementNode target, TimeSpan startTime, TimeSpan duration, double currentMovementPosition)
		{
			EffectIntents result = null;
			
			// figure out what color gradient to use for the pulse
			switch (ColorHandling) {
				case ChaseColorHandling.GradientForEachPulse:
					result = PulseRenderer.RenderNode(target, PulseCurve, ColorGradient, duration, HasDiscreteColors);
					result.OffsetAllCommandsByTime(startTime);
					if (ExtendPulseToStart && result.Count > 0)
					{
						foreach (var effectIntent in result.FirstOrDefault().Value)
						{ 
							_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(target, effectIntent, HasDiscreteColors));
						}
						
					}	
					_elementData.Add(result);
					if (ExtendPulseToEnd && result.Count > 0)
					{
						foreach (var effectIntent in result.FirstOrDefault().Value)
						{
							_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(target, effectIntent, TimeSpan, HasDiscreteColors));
						}
					}
					break;

				case ChaseColorHandling.GradientThroughWholeEffect:
					double startPos = (startTime.Ticks/(double) TimeSpan.Ticks);
					double endPos = ((startTime + duration).Ticks/(double) TimeSpan.Ticks);
					if (startPos < 0.0) startPos = 0.0;
					if (endPos > 1.0) endPos = 1.0;

					if (HasDiscreteColors) {
						double range = endPos - startPos;
						if (range <= 0.0) {
							Logging.Error("Chase: bad range: " + range + " (SP=" + startPos + ", EP=" + endPos + ")");
							break;
						}

						ColorGradient cg = ColorGradient.GetSubGradientWithDiscreteColors(startPos, endPos);

						foreach (Color color in cg.GetColorsInGradient()) {
							Curve newCurve = new Curve(PulseCurve.Points);
							foreach (PointPair point in newCurve.Points) {
								double effectRelativePosition = startPos + ((point.X / 100.0) * range);
								double proportion = ColorGradient.GetProportionOfColorAt(effectRelativePosition, color);
								point.Y *= proportion;
							}
							result = PulseRenderer.RenderNode(target, newCurve, new ColorGradient(color), duration, HasDiscreteColors);
							result.OffsetAllCommandsByTime(startTime);
							if (ExtendPulseToStart && result.Count > 0)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(target, effectIntent, HasDiscreteColors, new ColorGradient(color)));
								}
							}
							if(result.Count>0) _elementData.Add(result);
							if (ExtendPulseToEnd && result.Count>0)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(target, effectIntent, TimeSpan, HasDiscreteColors, new ColorGradient(color)));
								}
							}
						}
					} else {
						result = PulseRenderer.RenderNode(target, PulseCurve, ColorGradient.GetSubGradient(startPos, endPos), duration, HasDiscreteColors);
						result.OffsetAllCommandsByTime(startTime);
						if (ExtendPulseToStart && result.Count > 0)
						{
							foreach (var effectIntent in result.FirstOrDefault().Value)
							{
								_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(target, effectIntent, HasDiscreteColors, ColorGradient.GetSubGradient(0, startPos)));
							}
						}
						_elementData.Add(result);
						if (ExtendPulseToEnd && result.Count > 0)
						{
							foreach (var effectIntent in result.FirstOrDefault().Value)
							{
								_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(target, effectIntent, TimeSpan, HasDiscreteColors, ColorGradient.GetSubGradient(endPos,1)));
							}
						}
					}

					break;

				case ChaseColorHandling.StaticColor:
					result = PulseRenderer.RenderNode(target, PulseCurve, StaticColorGradient, duration, HasDiscreteColors);
					result.OffsetAllCommandsByTime(startTime);
					if (ExtendPulseToStart && result.Count > 0)
					{
						var intent = result.FirstOrDefault().Value.FirstOrDefault();
						_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(target, intent, HasDiscreteColors));
					}
					_elementData.Add(result);
					if (ExtendPulseToEnd && result.Count > 0)
					{
						var intent = result.FirstOrDefault().Value.LastOrDefault();
						_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(target, intent, TimeSpan, HasDiscreteColors));
					}
					break;

				case ChaseColorHandling.ColorAcrossItems:
					if (HasDiscreteColors) {
						List<Tuple<Color, float>> colorsAtPosition = ColorGradient.GetDiscreteColorsAndProportionsAt(currentMovementPosition / 100.0);
						foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
							float proportion = colorProportion.Item2;
							// scale all levels of the pulse curve by the proportion that is applicable to this color
							Curve newCurve = new Curve(PulseCurve.Points);
							foreach (PointPair pointPair in newCurve.Points) {
								pointPair.Y *= proportion;
							}
							result = PulseRenderer.RenderNode(target, newCurve, new ColorGradient(colorProportion.Item1), duration, HasDiscreteColors);
							result.OffsetAllCommandsByTime(startTime);
							if (ExtendPulseToStart && result.Count > 0)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(target, effectIntent, HasDiscreteColors, new ColorGradient(colorProportion.Item1)));
								}
							}
							if(result.Count>0)_elementData.Add(result);
							if (ExtendPulseToEnd && result.Count > 0)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(target, effectIntent, TimeSpan, HasDiscreteColors, new ColorGradient(colorProportion.Item1)));
								}
							}
						}
					} else {
						result = PulseRenderer.RenderNode(target, PulseCurve, new ColorGradient(ColorGradient.GetColorAt(currentMovementPosition / 100.0)), duration, HasDiscreteColors);
						result.OffsetAllCommandsByTime(startTime);
						if (ExtendPulseToStart && result.Count > 0)
						{
							foreach (var effectIntent in result.FirstOrDefault().Value)
							{
								_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(target, effectIntent, HasDiscreteColors));
							}
						}
						_elementData.Add(result);
						if (ExtendPulseToEnd && result.Count > 0)
						{
							foreach (var effectIntent in result.FirstOrDefault().Value)
							{
								_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(target, effectIntent, TimeSpan, HasDiscreteColors));
							}
						}
					}
					break;
			}
		}
	}
}
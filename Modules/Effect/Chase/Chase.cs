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
using VixenModules.Property.Color;

namespace VixenModules.Effect.Chase
{
	public class Chase : EffectModuleInstanceBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ChaseData _data;
		private EffectIntents _elementData = null;

		public Chase()
		{
			_data = new ChaseData();
		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();
			if (IsDirty) CheckForInvalidColorData();

			DoRendering();
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as ChaseData; }
		}

		public override bool IsDirty
		{
			get
			{
				if (!PulseCurve.CheckLibraryReference())
					return true;

				if (!ChaseMovement.CheckLibraryReference())
					return true;

				if (!ColorGradient.CheckLibraryReference())
					return true;

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		[Value]
		public ChaseColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set
			{
				_data.ColorHandling = value;
				IsDirty = true;
			}
		}


		[Value]
		public int PulseOverlap
		{
			get { return _data.PulseOverlap; }
			set
			{
				_data.PulseOverlap = value;
				IsDirty = true;
			}
		}

		[Value]
		public double DefaultLevel
		{
			get { return _data.DefaultLevel; }
			set
			{
				_data.DefaultLevel = value;
				IsDirty = true;
			}
		}

		[Value]
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
			}
		}

		//Created to hold a ColorGradient version of color rather than continually creating them from Color for static colors.
		protected ColorGradient StaticColorGradient
		{
			get { return _data.StaticColorGradient; }
			set { _data.StaticColorGradient = value; }
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

		[Value]
		public Curve PulseCurve
		{
			get { return _data.PulseCurve; }
			set
			{
				_data.PulseCurve = value;
				IsDirty = true;
			}
		}

		[Value]
		public Curve ChaseMovement
		{
			get { return _data.ChaseMovement; }
			set
			{
				_data.ChaseMovement = value;
				IsDirty = true;
			}
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


		private void DoRendering()
		{
			//TODO: get a better increment time. doing it every X ms is..... shitty at best.
			TimeSpan increment = TimeSpan.FromMilliseconds(2);

			List<ElementNode> renderNodes = GetNodesToRenderOn();

			int targetNodeCount = renderNodes.Count;

			Pulse.Pulse pulse;
			EffectIntents pulseData;

			// apply the 'background' values to all targets if the level is supposed to be nonzero
			if (DefaultLevel > 0) {
				int i = 0;
				foreach (ElementNode target in renderNodes) {
					if (target != null) {
						bool discreteColors = ColorModule.isElementNodeDiscreteColored(target);

						pulse = new Pulse.Pulse();
						pulse.TargetNodes = new ElementNode[] {target};
						pulse.TimeSpan = TimeSpan;
						double level = DefaultLevel*100.0;

						// figure out what color gradient to use for the pulse
						switch (ColorHandling) {
							case ChaseColorHandling.GradientForEachPulse:
								pulse.ColorGradient = StaticColorGradient;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {level, level}));
								pulseData = pulse.Render();
								_elementData.Add(pulseData);
								break;

							case ChaseColorHandling.GradientThroughWholeEffect:
								pulse.ColorGradient = ColorGradient;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {level, level}));
								pulseData = pulse.Render();
								_elementData.Add(pulseData);
								break;

							case ChaseColorHandling.StaticColor:
								pulse.ColorGradient = StaticColorGradient;
								pulse.LevelCurve = new Curve(new PointPairList(new double[] {0, 100}, new double[] {level, level}));
								pulseData = pulse.Render();
								_elementData.Add(pulseData);
								break;

							case ChaseColorHandling.ColorAcrossItems:
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

						pulseData = pulse.Render();
						_elementData.Add(pulseData);
						i++;
					}
				}
			}


			// the total chase time
			TimeSpan chaseTime = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds - PulseOverlap);
			if (chaseTime.TotalMilliseconds <= 0)
				chaseTime = TimeSpan.FromMilliseconds(1);

			// we need to keep track of the element that is 'under' the curve at a given time, to see if it changes,
			// and when it does, we make the effect for it then (since it's a variable time pulse).
			ElementNode lastTargetedNode = null;
			TimeSpan lastNodeStartTime = TimeSpan.Zero;

			// iterate up to and including the last pulse generated
			for (TimeSpan current = TimeSpan.Zero; current <= TimeSpan; current += increment) {
				double currentPercentageIntoChase = ((double) current.Ticks/(double) chaseTime.Ticks)*100.0;

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

				ElementNode currentNode = renderNodes[currentNodeIndex];
				if (currentNode == lastTargetedNode)
					continue;

				// if the last node targeted wasn't null, we need to make a pulse for it
				if (lastTargetedNode != null) {
					GeneratePulse(lastTargetedNode, lastNodeStartTime,
					              current - lastNodeStartTime + TimeSpan.FromMilliseconds(PulseOverlap), currentMovementPosition);
				}

				lastTargetedNode = currentNode;
				lastNodeStartTime = current;

				// if we've hit the 100% mark of the chase curve, bail (the last one gets generated after)
				if (currentPercentageIntoChase >= 100.0)
					break;
			}

			// generate the last pulse
			if (lastTargetedNode != null) {
				GeneratePulse(lastTargetedNode, lastNodeStartTime, TimeSpan - lastNodeStartTime, 1.0);
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


		private void GeneratePulse(ElementNode target, TimeSpan startTime, TimeSpan duration, double currentMovementPosition)
		{
			EffectIntents result;
			Pulse.Pulse pulse = new Pulse.Pulse();
			pulse.TargetNodes = new ElementNode[] {target};
			pulse.TimeSpan = duration;
			pulse.LevelCurve = new Curve(PulseCurve);

			bool discreteColors = ColorModule.isElementNodeDiscreteColored(target);

			// figure out what color gradient to use for the pulse
			switch (ColorHandling) {
				case ChaseColorHandling.GradientForEachPulse:
					pulse.ColorGradient = ColorGradient;
					result = pulse.Render();
					result.OffsetAllCommandsByTime(startTime);
					_elementData.Add(result);
					break;

				case ChaseColorHandling.GradientThroughWholeEffect:
					double startPos = ((double) startTime.Ticks/(double) TimeSpan.Ticks);
					double endPos = ((double) (startTime + duration).Ticks/(double) TimeSpan.Ticks);
					if (startPos < 0.0) startPos = 0.0;
					if (endPos > 1.0) endPos = 1.0;

					if (discreteColors) {
						double range = endPos - startPos;
						if (range <= 0.0) {
							Logging.Error("Chase: bad range: " + range + " (SP=" + startPos + ", EP=" + endPos + ")");
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
							result = pulse.Render();
							result.OffsetAllCommandsByTime(startTime);
							_elementData.Add(result);
						}
					} else {
						pulse.ColorGradient = ColorGradient.GetSubGradient(startPos, endPos);
						result = pulse.Render();
						result.OffsetAllCommandsByTime(startTime);
						_elementData.Add(result);
					}

					break;

				case ChaseColorHandling.StaticColor:
					pulse.ColorGradient = StaticColorGradient;
					result = pulse.Render();
					result.OffsetAllCommandsByTime(startTime);
					_elementData.Add(result);
					break;

				case ChaseColorHandling.ColorAcrossItems:
					if (discreteColors) {
						List<Tuple<Color, float>> colorsAtPosition = ColorGradient.GetDiscreteColorsAndProportionsAt(currentMovementPosition / 100.0);
						foreach (Tuple<Color, float> colorProportion in colorsAtPosition) {
							float proportion = colorProportion.Item2;
							// scale all levels of the pulse curve by the proportion that is applicable to this color
							Curve newCurve = new Curve(pulse.LevelCurve.Points);
							foreach (PointPair pointPair in newCurve.Points) {
								pointPair.Y *= proportion;
							}
							pulse.LevelCurve = newCurve;
							pulse.ColorGradient = new ColorGradient(colorProportion.Item1);
							result = pulse.Render();
							result.OffsetAllCommandsByTime(startTime);
							_elementData.Add(result);
						}
					} else {
						pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(currentMovementPosition/100.0));
						result = pulse.Render();
						result.OffsetAllCommandsByTime(startTime);
						_elementData.Add(result);
					}
					break;
			}

		}
	}
}
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

namespace VixenModules.Effect.Spin
{
	public class Spin : EffectModuleInstanceBase
	{
		private SpinData _data;
		private EffectIntents _elementData = null;

		public Spin()
		{
			_data = new SpinData();
		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();
			DoRendering();
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as SpinData; }
		}

		public override bool IsDirty
		{
			get
			{
				if (!PulseCurve.CheckLibraryReference())
					return true;

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
		public SpinSpeedFormat SpeedFormat
		{
		    get { return _data.SpeedFormat; }
		    set { _data.SpeedFormat = value; IsDirty = true; }
		}

		[Value]
		public SpinPulseLengthFormat PulseLengthFormat
		{
			get { return _data.PulseLengthFormat; }
			set { _data.PulseLengthFormat = value; IsDirty = true; }
		}

		[Value]
		public SpinColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set { _data.ColorHandling = value; IsDirty = true; }
		}

		[Value]
		public double RevolutionCount
		{
			get { return _data.RevolutionCount; }
			set { _data.RevolutionCount = value; IsDirty = true; }
		}

		[Value]
		public double RevolutionFrequency
		{
			get { return _data.RevolutionFrequency; }
			set { _data.RevolutionFrequency = value; IsDirty = true; }
		}

		[Value]
		public int RevolutionTime
		{
			get { return _data.RevolutionTime; }
			set { _data.RevolutionTime = value; IsDirty = true; }
		}

		[Value]
		public int PulseTime
		{
			get { return _data.PulseTime; }
			set { _data.PulseTime = value; IsDirty = true; }
		}

		[Value]
		public int PulsePercentage
		{
			get { return _data.PulsePercentage; }
			set { _data.PulsePercentage = value; IsDirty = true; }
		}

		[Value]
		public double DefaultLevel
		{
			get { return _data.DefaultLevel; }
			set { _data.DefaultLevel = value; IsDirty = true; }
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

		[Value]
		public Curve PulseCurve
		{
			get { return _data.PulseCurve; }
			set { _data.PulseCurve = value; IsDirty = true; }
		}

		[Value]
		public bool ReverseSpin
		{
			get { return _data.ReverseSpin; }
			set { _data.ReverseSpin = value; IsDirty = true; }
		}

		[Value]
		public int DepthOfEffect
		{
			get { return _data.DepthOfEffect; }
			set { _data.DepthOfEffect = value; IsDirty = true; }
		}

		private void DoRendering()
		{
			//TODO: get a better increment time. doing it every X ms is..... shitty at best.
			TimeSpan increment = TimeSpan.FromMilliseconds(10);

			List<ElementNode> renderNodes = GetNodesToRenderOn();
			int targetNodeCount = renderNodes.Count;
			ElementNode lastTargetedNode = null;

			Pulse.Pulse pulse;
			EffectIntents pulseData;

			// apply the 'background' values to all targets
			int i = 0;
			foreach (ElementNode target in renderNodes) {
				pulse = new Pulse.Pulse();
				pulse.TargetNodes = new ElementNode[] { target };
				pulse.TimeSpan = TimeSpan;
				pulse.LevelCurve = new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { DefaultLevel * 100.0, DefaultLevel * 100.0 }));

				// figure out what color gradient to use for the pulse
				switch (ColorHandling) {
					case SpinColorHandling.GradientForEachPulse:
						pulse.ColorGradient = new ColorGradient(StaticColor);
						break;

					case SpinColorHandling.GradientThroughWholeEffect:
						pulse.ColorGradient = ColorGradient;
						break;

					case SpinColorHandling.StaticColor:
						pulse.ColorGradient = new ColorGradient(StaticColor);
						break;

					case SpinColorHandling.ColorAcrossItems:
						pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt((double)i / (double)targetNodeCount));
						break;
				}

				pulseData = pulse.Render();
				_elementData.Add(pulseData);
				i++;
			}


			// calculate the pulse time and revolution time exactly (based on the parameters from the data)
			double revTimeMs = 0;				// single revolution time (ms)

			// figure out the relative length of a individual pulse
			double pulseConstant = 0;			// how much of each pulse is a constant time
			double pulseFractional = 0;			// how much of each pulse is a fraction of a single spin
			if (PulseLengthFormat == SpinPulseLengthFormat.FixedTime) {
				pulseConstant = PulseTime;
			} else if (PulseLengthFormat == SpinPulseLengthFormat.PercentageOfRevolution) {
				pulseFractional = PulsePercentage / 100.0;
			} else if (PulseLengthFormat == SpinPulseLengthFormat.EvenlyDistributedAcrossSegments) {
				pulseFractional = 1.0 / (double)targetNodeCount;
			}

			// magic number. (the inaccuracy of interpolating the curve into a position. eg. if we have 5 'positions', then
			// the curve should really be from 0-80% for the last spin, to make sure the last pulse finishes accurately.)
			double pulseInterpolationOffset = 1.0 / (double)targetNodeCount;

			// figure out either the revolution count or time, based on what data we have
			if (SpeedFormat == SpinSpeedFormat.RevolutionCount) {
				revTimeMs = (TimeSpan.TotalMilliseconds - pulseConstant) / (RevolutionCount + pulseFractional - pulseInterpolationOffset);
			} else if (SpeedFormat == SpinSpeedFormat.RevolutionFrequency) {
				revTimeMs = (1.0 / RevolutionFrequency) * 1000.0;	// convert Hz to period ms
			} else if (SpeedFormat == SpinSpeedFormat.FixedTime) {
				revTimeMs = RevolutionTime;
			}

			double pulTimeMs = pulseConstant + (revTimeMs * pulseFractional);

			TimeSpan revTimeSpan = TimeSpan.FromMilliseconds(revTimeMs);
			TimeSpan pulseTimeSpan = TimeSpan.FromMilliseconds(pulTimeMs);

			// figure out which way we're moving through the elements
			Curve movement;
			if (ReverseSpin)
				movement = new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { 100, 0 }));
			else
				movement = new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { 0, 100 }));
			
			// iterate up to and including the last pulse generated
			// a bit iffy, but stops 'carry over' spins past the end (when there's overlapping spins). But we need to go past
			// (total - pulse) as the last pulse can often be a bit inaccurate due to the rounding of the increment
			for (TimeSpan current = TimeSpan.Zero; current <= TimeSpan - pulseTimeSpan + increment; current += increment) {
				double currentPercentageIntoSpin = ((double)(current.Ticks % revTimeSpan.Ticks) / (double)revTimeSpan.Ticks) * 100.0;

				double targetElementPosition = movement.GetValue(currentPercentageIntoSpin);
				int currentNodeIndex = (int)((targetElementPosition / 100.0) * targetNodeCount);

				// on the off chance we hit the 100% mark *exactly*...
				if (currentNodeIndex == targetNodeCount)
					currentNodeIndex--;

				if (currentNodeIndex >= targetNodeCount) {
					VixenSystem.Logging.Warning("Spin effect: rendering, but the current node index is higher or equal to the total target nodes.");
					continue;
				}

				ElementNode currentNode = renderNodes[currentNodeIndex];
				if (currentNode == lastTargetedNode)
					continue;

				// make a pulse for it
				pulse = new Pulse.Pulse();
				pulse.TargetNodes = new ElementNode[] { currentNode };
				pulse.TimeSpan = pulseTimeSpan;
				pulse.LevelCurve = new Curve(PulseCurve);

				// figure out what color gradient to use for the pulse
				switch (ColorHandling) {
					case SpinColorHandling.GradientForEachPulse:
						pulse.ColorGradient = ColorGradient;
						break;

					case SpinColorHandling.GradientThroughWholeEffect:
						double startPos = ((double)current.Ticks / (double)TimeSpan.Ticks);
						double endPos = 1.0;
						if (TimeSpan - current >= pulseTimeSpan)
							endPos = ((double)(current + pulseTimeSpan).Ticks / (double)TimeSpan.Ticks);
						pulse.ColorGradient = ColorGradient.GetSubGradient(startPos, endPos);
						break;

					case SpinColorHandling.StaticColor:
						pulse.ColorGradient = new ColorGradient(StaticColor);
						break;

					case SpinColorHandling.ColorAcrossItems:
						pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(targetElementPosition / 100.0));
						break;
				}

				pulseData = pulse.Render();
				pulseData.OffsetAllCommandsByTime(current);
				_elementData.Add(pulseData);
				
				lastTargetedNode = currentNode;
			}

			_elementData = EffectIntents.Restrict(_elementData, TimeSpan.Zero, TimeSpan);
		}

		private List<ElementNode> GetNodesToRenderOn()
		{
			IEnumerable<ElementNode> renderNodes = null;

			if (DepthOfEffect == 0)
			{
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator()).ToList();
			}
			else
			{
				renderNodes = TargetNodes;
				for (int i = 0; i < DepthOfEffect; i++)
				{
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

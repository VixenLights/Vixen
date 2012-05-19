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
using System.Drawing;
using ZedGraph;

namespace VixenModules.Effect.Spin
{
	public class Spin : EffectModuleInstanceBase
	{
		private SpinData _data;
		private EffectIntents _channelData = null;

		public Spin()
		{
			_data = new SpinData();
		}

		protected override void _PreRender()
		{
			_channelData = new EffectIntents();
			DoRendering();
		}

		protected override EffectIntents _Render()
		{
			return _channelData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as SpinData; }
		}

		public override object[] ParameterValues
		{
			get
			{
				return new object[] {
					SpeedFormat,
					PulseLengthFormat,
					ColorHandling,
					RevolutionCount,
					RevolutionFrequency,
					RevolutionTime,
					PulseTime,
					PulsePercentage,
					DefaultLevel,
					StaticColor,
					ColorGradient,
					PulseCurve,
					ReverseSpin
				};
			}
			set
			{
				if (value.Length != 13) {
					VixenSystem.Logging.Warning("Spin effect parameters set with " + value.Length + " parameters");
					return;
				}

				SpeedFormat = (SpinSpeedFormat)value[0];
				PulseLengthFormat = (SpinPulseLengthFormat)value[1];
				ColorHandling = (SpinColorHandling)value[2];
				RevolutionCount = (double)value[3];
				RevolutionFrequency = (double)value[4];
				RevolutionTime = (int)value[5];
				PulseTime = (int)value[6];
				PulsePercentage = (int)value[7];
				DefaultLevel = (Level)value[8];
				StaticColor = (Color)value[9];
				ColorGradient = (ColorGradient)value[10];
				PulseCurve = (Curve)value[11];
				ReverseSpin = (bool)value[12];
			}
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


		public SpinSpeedFormat SpeedFormat
		{
		    get { return _data.SpeedFormat; }
		    set { _data.SpeedFormat = value; IsDirty = true; }
		}

		public SpinPulseLengthFormat PulseLengthFormat
		{
			get { return _data.PulseLengthFormat; }
			set { _data.PulseLengthFormat = value; IsDirty = true; }
		}

		public SpinColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set { _data.ColorHandling = value; IsDirty = true; }
		}

		public double RevolutionCount
		{
			get { return _data.RevolutionCount; }
			set { _data.RevolutionCount = value; IsDirty = true; }
		}

		public double RevolutionFrequency
		{
			get { return _data.RevolutionFrequency; }
			set { _data.RevolutionFrequency = value; IsDirty = true; }
		}

		public int RevolutionTime
		{
			get { return _data.RevolutionTime; }
			set { _data.RevolutionTime = value; IsDirty = true; }
		}

		public int PulseTime
		{
			get { return _data.PulseTime; }
			set { _data.PulseTime = value; IsDirty = true; }
		}

		public int PulsePercentage
		{
			get { return _data.PulsePercentage; }
			set { _data.PulsePercentage = value; IsDirty = true; }
		}

		public Level DefaultLevel
		{
			get { return _data.DefaultLevel; }
			set { _data.DefaultLevel = value; IsDirty = true; }
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

		public Curve PulseCurve
		{
			get { return _data.PulseCurve; }
			set { _data.PulseCurve = value; IsDirty = true; }
		}

		public bool ReverseSpin
		{
			get { return _data.ReverseSpin; }
			set { _data.ReverseSpin = value; IsDirty = true; }
		}

		private void DoRendering()
		{
			//TODO: get a better increment time. doing it every X ms is..... shitty at best.
			TimeSpan increment = TimeSpan.FromMilliseconds(10);

			List<ChannelNode> renderNodes = RGBModule.FindAllRenderableChildren(TargetNodes);
			int targetNodeCount = renderNodes.Count;
			ChannelNode lastTargetedNode = null;

			Pulse.Pulse pulse;
			EffectIntents pulseData;

			// apply the 'background' values to all targets
			int i = 0;
			foreach (ChannelNode target in renderNodes) {
				pulse = new Pulse.Pulse();
				pulse.TargetNodes = new ChannelNode[] { target };
				pulse.TimeSpan = TimeSpan;
				pulse.LevelCurve = new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { DefaultLevel, DefaultLevel }));

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
				//TODO
				//_channelData.AddChannelData(pulseData);
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

			// figure out which way we're moving through the channels
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

				double targetChannelPosition = movement.GetValue(currentPercentageIntoSpin);
				int currentNodeIndex = (int)((targetChannelPosition / 100.0) * targetNodeCount);

				// on the off chance we hit the 100% mark *exactly*...
				if (currentNodeIndex == targetNodeCount)
					currentNodeIndex--;

				if (currentNodeIndex >= targetNodeCount) {
					VixenSystem.Logging.Warning("Spin effect: rendering, but the current node index is higher or equal to the total target nodes.");
					continue;
				}

				ChannelNode currentNode = renderNodes[currentNodeIndex];
				if (currentNode == lastTargetedNode)
					continue;

				// make a pulse for it
				pulse = new Pulse.Pulse();
				pulse.TargetNodes = new ChannelNode[] { currentNode };
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
							endPos = ((double)(current + TimeSpan.FromMilliseconds(PulseTime)).Ticks / (double)TimeSpan.Ticks);
						pulse.ColorGradient = ColorGradient.GetSubGradient(startPos, endPos);
						break;

					case SpinColorHandling.StaticColor:
						pulse.ColorGradient = new ColorGradient(StaticColor);
						break;

					case SpinColorHandling.ColorAcrossItems:
						pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(targetChannelPosition / 100.0));
						break;
				}

				pulseData = pulse.Render();
				//TODO
				//pulseData.OffsetAllCommandsByTime(current);
				//_channelData.AddChannelData(pulseData);
				
				lastTargetedNode = currentNode;
			}

			_channelData = EffectIntents.Restrict(_channelData, TimeSpan.Zero, TimeSpan);
		}
	}
}

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

namespace VixenModules.Effect.Chase
{
	public class Chase : EffectModuleInstanceBase
	{
		private ChaseData _data;
		private ChannelData _channelData = null;

		public Chase()
		{
			_data = new ChaseData();
		}

		protected override void _PreRender()
		{
			_channelData = new ChannelData();
			DoRendering();
		}

		protected override ChannelData _Render()
		{
			return _channelData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as ChaseData; }
		}

		public override object[] ParameterValues
		{
			get
			{
				return new object[] {
					ColorHandling,
					PulseOverlap,
					DefaultLevel,
					StaticColor,
					ColorGradient,
					PulseCurve,
					ChaseMovement
				};
			}
			set
			{
				if (value.Length != 7) {
					VixenSystem.Logging.Warning("Chase effect parameters set with " + value.Length + " parameters");
					return;
				}

				ColorHandling = (ChaseColorHandling)value[0];
				PulseOverlap = (int)value[1];
				DefaultLevel = (Level)value[2];
				StaticColor = (Color)value[3];
				ColorGradient = (ColorGradient)value[4];
				PulseCurve = (Curve)value[5];
				ChaseMovement = (Curve)value[6];
			}
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
			protected set
			{
				base.IsDirty = value;
			}
		}

		public ChaseColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set { _data.ColorHandling = value; IsDirty = true; }
		}


		public int PulseOverlap
		{
			get { return _data.PulseOverlap; }
			set { _data.PulseOverlap = value; IsDirty = true; }
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

		public Curve ChaseMovement
		{
			get { return _data.ChaseMovement; }
			set { _data.ChaseMovement = value; IsDirty = true; }
		}


		private void DoRendering()
		{
			//TODO: get a better increment time. doing it every X ms is..... shitty at best.
			TimeSpan increment = TimeSpan.FromMilliseconds(5);

			List<ChannelNode> renderNodes = RGBModule.FindAllRenderableChildren(TargetNodes);
			int targetNodeCount = renderNodes.Count;

			Pulse.Pulse pulse;
			ChannelData pulseData;

			// apply the 'background' values to all targets
			int i = 0;
			foreach (ChannelNode target in renderNodes) {
				pulse = new Pulse.Pulse();
				pulse.TargetNodes = new ChannelNode[] { target };
				pulse.TimeSpan = TimeSpan;
				pulse.LevelCurve = new Curve(new PointPairList(new double[] { 0, 100 }, new double[] { DefaultLevel, DefaultLevel }));

				// figure out what color gradient to use for the pulse
				switch (ColorHandling) {
					case ChaseColorHandling.GradientForEachPulse:
						pulse.ColorGradient = new ColorGradient(StaticColor);
						break;

					case ChaseColorHandling.GradientThroughWholeEffect:
						pulse.ColorGradient = ColorGradient;
						break;

					case ChaseColorHandling.StaticColor:
						pulse.ColorGradient = new ColorGradient(StaticColor);
						break;

					case ChaseColorHandling.ColorAcrossItems:
						pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt((double)i / (double)targetNodeCount));
						break;
				}

				pulseData = pulse.Render();
				_channelData.AddChannelData(pulseData);
				i++;
			}


			// the total chase time
			TimeSpan chaseTime = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds - PulseOverlap);

			// we need to keep track of the channel that is 'under' the curve at a given time, to see if it changes,
			// and when it does, we make the effect for it then (since it's a variable time pulse).
			ChannelNode lastTargetedNode = null;
			TimeSpan lastNodeStartTime = TimeSpan.Zero;
			
			// iterate up to and including the last pulse generated
			for (TimeSpan current = TimeSpan.Zero; current <= TimeSpan; current += increment) {
				double currentPercentageIntoChase = ((double)current.Ticks / (double)chaseTime.Ticks) * 100.0;

				double currentMovementPosition = ChaseMovement.GetValue(currentPercentageIntoChase);
				int currentNodeIndex = (int)((currentMovementPosition / 100.0) * targetNodeCount);

				// on the off chance we hit the 100% mark *exactly*...
				if (currentNodeIndex == targetNodeCount)
					currentNodeIndex--;

				if (currentNodeIndex >= targetNodeCount) {
					VixenSystem.Logging.Warning("Chase effect: rendering, but the current node index is higher or equal to the total target nodes.");
					continue;
				}

				ChannelNode currentNode = renderNodes[currentNodeIndex];
				if (currentNode == lastTargetedNode)
					continue;

				// if the last node targeted wasn't null, we need to make a pulse for it
				if (lastTargetedNode != null) {
					GeneratePulse(lastTargetedNode, lastNodeStartTime, current - lastNodeStartTime + TimeSpan.FromMilliseconds(PulseOverlap), currentMovementPosition);
				}

				lastTargetedNode = currentNode;
				lastNodeStartTime = current;

				// if we've hit the 100% mark of the chase curve, bail (the last one gets generated after)
				if (currentPercentageIntoChase >= 100.0)
					break;
			}

			// generate the last pulse
			if (lastTargetedNode != null) {
				GeneratePulse(lastTargetedNode, lastNodeStartTime, TimeSpan - lastNodeStartTime, ChaseMovement.GetValue(100.0));
			}

			_channelData = ChannelData.Restrict(_channelData, TimeSpan.Zero, TimeSpan);
		}

		private void GeneratePulse(ChannelNode target, TimeSpan startTime, TimeSpan duration, double currentMovementPosition)
		{
			ChannelData result;
			Pulse.Pulse pulse = new Pulse.Pulse();
			pulse.TargetNodes = new ChannelNode[] { target };
			pulse.TimeSpan = duration;
			pulse.LevelCurve = new Curve(PulseCurve);

			// figure out what color gradient to use for the pulse
			switch (ColorHandling) {
				case ChaseColorHandling.GradientForEachPulse:
					pulse.ColorGradient = ColorGradient;
					break;

				case ChaseColorHandling.GradientThroughWholeEffect:
					double startPos = ((double)startTime.Ticks / (double)TimeSpan.Ticks);
					double endPos = ((double)(startTime + duration).Ticks / (double)TimeSpan.Ticks);
					if (startPos < 0.0) startPos = 0.0;
					if (endPos > 1.0) endPos = 1.0;
					pulse.ColorGradient = ColorGradient.GetSubGradient(startPos, endPos);
					break;

				case ChaseColorHandling.StaticColor:
					pulse.ColorGradient = new ColorGradient(StaticColor);
					break;

				case ChaseColorHandling.ColorAcrossItems:
					pulse.ColorGradient = new ColorGradient(ColorGradient.GetColorAt(currentMovementPosition / 100.0));
					break;
			}

			result = pulse.Render();
			result.OffsetAllCommandsByTime(startTime);
			_channelData.AddChannelData(result);
		}
	}
}

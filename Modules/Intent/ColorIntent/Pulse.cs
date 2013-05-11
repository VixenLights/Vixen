using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Intent;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Property.RGB;

namespace VixenModules.Intent.Color
{
	public class Pulse : IntentModuleInstanceBase
	{
		private PulseData _data;
		private EffectIntents _channelData = null;
		private RGBModule _rgbModule;

		public Pulse()
		{
			_data = new PulseData();
			_rgbModule = new RGBModule();
		}

		//protected override void _PreRender()
		//{
		//    _channelData = new EffectIntents();

		//    foreach (ChannelNode node in TargetNodes) {
		//        RenderNode(node);
		//    }
		//}

		//protected override EffectIntents _Render()
		//{
		//    return _channelData;
		//}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as PulseData; }
		}

		//public override object[] ParameterValues
		//{
		//    get
		//    {
		//        return new object[] { LevelCurve, ColorGradient };
		//    }
		//    set
		//    {
		//        if (value.Length != 2) {
		//            VixenSystem.Logging.Error("Pulse parameters set with " + value.Length + " parameters!");
		//        } else {
		//            LevelCurve = (Curve)value[0];
		//            ColorGradient = (ColorGradient)value[1];
		//        }
		//    }
		//}

		//public override bool IsDirty
		//{
		//    get
		//    {
		//        if (!LevelCurve.CheckLibraryReference())
		//            return true;

		//        if (!ColorGradient.CheckLibraryReference())
		//            return true;

		//        return base.IsDirty;
		//    }
		//    protected set
		//    {
		//        base.IsDirty = value;
		//    }
		//}


		[Value]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set { _data.LevelCurve = value; }
		}

		[Value]
		public ColorGradient ColorGradient
		{
			get { return _data.ColorGradient; }
			set { _data.ColorGradient = value; }
		}

		// The minimum amount of time between successive generated commands. Currently, this 
		// is essentially a fixed value, but if we need to later we can add the ability to change this.
		// (note: this value is nothing more than a minimum interval. If the value doesn't change much,
		// the intervals may be much higher.)
		//private TimeSpan MinimumRenderInterval { get { return TimeSpan.FromMilliseconds(10); } }

		// the minimum level change between successive generated commands. As above, currently fixed,
		// may change later.
		// This is only used for monochrome commands, as it will be hard to tell what to do with RGB
		// commands (as the RGB property module may render them off to subchannels, etc.)
		//private double MinimumLevelChangeInterval { get { return 1.0; } }


		// renders the given node to the internal ChannelData dictionary. If the given node is
		// not a channel, will recursively descend until we render its channels.
		//private void RenderNode(ChannelNode node)
		//{
		//    foreach (ChannelNode renderableNode in RGBModule.FindAllRenderableChildren(node)) {
		//        RenderRGB(renderableNode);
		//    }
		//}

		//private void RenderRGB(ChannelNode node)
		//{
		//    // for the given Channel, render a bunch of color commands (using the RGB property helpers)
		//    // for individual time slices of the effect. There's no real easy way (that's obvious after
		//    // 5 minutes of thought) to intelligently generate commands more efficently (ie. slower if we
		//    // can get away with it), so for now, just blat it out at full speed. TODO here for that.
		//    TimeSpan currentTime = TimeSpan.Zero;
		//    RGBModule rgbProperty = node.Properties.Get(PulseDescriptor._RGBPropertyId) as RGBModule;

		//}

		public override Command GetCurrentState(TimeSpan timeOffset) {
			double fractionalProgress = timeOffset.TotalMilliseconds / TimeSpan.TotalMilliseconds;
			double percentProgress = fractionalProgress * 100.0;
			Level currentLevel = LevelCurve.GetValue(percentProgress);
			System.Drawing.Color currentColor = ColorGradient.GetColorAt(fractionalProgress);

			////TimeSpan sliceDuration = (currentTime + MinimumRenderInterval < TimeSpan) ? MinimumRenderInterval : TimeSpan - currentTime;
			//ChannelCommands rgbData = _rgbModule.RenderColorToCommands(currentColor, currentLevel);
			//foreach(KeyValuePair<Guid, Command[]> kvp in rgbData) {
			//    foreach(Command c in kvp.Value) {
			//        CommandNode newCommandNode = new CommandNode(c, currentTime, sliceDuration);
			//        _channelData.AddCommandNodeForChannel(kvp.Key, newCommandNode);
			//    }
			//}

			//currentTime += MinimumRenderInterval;

			return new Lighting.Polychrome.SetColor(currentColor);
		}
	}
}

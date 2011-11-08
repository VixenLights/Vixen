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

namespace VixenModules.Effect.Spin
{
	public class Spin : EffectModuleInstanceBase
	{
		private SpinData _data;
		private ChannelData _channelData = null;

		public Spin()
		{
			_data = new SpinData();
		}


		protected override void _PreRender()
		{
			_channelData = new ChannelData();

			foreach (ChannelNode node in TargetNodes) {
			}
		}

		protected override ChannelData _Render()
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
				//return new object[] { LevelCurve, ColorGradient };
				return null;
			}
			set
			{
				if (value.Length != 2) {
					VixenSystem.Logging.Error("Pulse parameters set with " + value.Length + " parameters!");
				} else {
					//LevelCurve = (Curve)value[0];
					//ColorGradient = (ColorGradient)value[1];
				}
			}
		}

		public override bool IsDirty
		{
			get
			{
				//if (!LevelCurve.CheckLibraryReference())
				//    return true;

				//if (!ColorGradient.CheckLibraryReference())
				//    return true;

				return base.IsDirty;
			}
			protected set
			{
				base.IsDirty = value;
			}
		}


		//public Curve LevelCurve
		//{
		//    get { return _data.LevelCurve; }
		//    set { _data.LevelCurve = value; IsDirty = true; }
		//}

		//public ColorGradient ColorGradient
		//{
		//    get { return _data.ColorGradient; }
		//    set { _data.ColorGradient = value; IsDirty = true; }
		//}
	}
}

using System;
using System.Linq;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace ColorWithIntent {
	public class ColorModule : EffectModuleInstanceBase {
		private ColorData _data;
		private EffectIntents _intents;

		protected override void _PreRender() {
			_intents = new EffectIntents();
			Channel[] channels = TargetNodes.SelectMany(x => x).ToArray();

			foreach(Channel channel in channels) {
				//IIntentModuleInstance intent = ApplicationServices.Get<IIntentModuleInstance>(ColorDescriptor._colorIntentId);
				ColorTransitionIntent intent = new ColorTransitionIntent(ColorGradient.Colors.First().Color.ToRGB(), ColorGradient.Colors.Last().Color.ToRGB(), TimeSpan);
				//intent.TimeSpan = TimeSpan;
				//intent.Values = new object[] { LevelCurve, ColorGradient };
				_intents.AddIntentForChannel(channel.Id, new IntentNode(intent, TimeSpan.Zero));
			}
		}

		protected override EffectIntents _Render() {
			return _intents;
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = (ColorData)value; }
		}

		[Value]
		public Curve LevelCurve {
			get { return _data.LevelCurve; }
			set { _data.LevelCurve = value; }
		}

		[Value]
		public ColorGradient ColorGradient {
			get { return _data.ColorGradient; }
			set { _data.ColorGradient = value; }
		}
	}
}

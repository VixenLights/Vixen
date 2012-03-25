using System;
using System.Drawing;
using System.Linq;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.Sys.CombinationOperation;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace ColorWithIntent {
	public class ColorModule : EffectModuleInstanceBase {
		private ColorData _data;
		private EffectIntents _intents;

		private bool _isSubordinate;
		protected override void _PreRender() {
			//**********************
			//*** going to add a subordinate, just for testing
			if(!_isSubordinate) {
				ColorModule otherEffect = (ColorModule)Vixen.Services.ApplicationServices.Get<IEffectModuleInstance>(Descriptor.TypeId);
				Curve levelCurve = LevelCurve;
				ColorGradient colorGradient = new ColorGradient(Color.FromArgb(64,128,192));
				otherEffect.ParameterValues = new object[] {levelCurve, colorGradient};
				otherEffect.TimeSpan = TimeSpan;
				otherEffect.TargetNodes = TargetNodes;
				otherEffect._isSubordinate = true;
				SubordinateEffects.Add(new SubordinateEffect(otherEffect, new BooleanAnd()));
			}
			//**********************

			_intents = new EffectIntents();
			Channel[] channels = TargetNodes.SelectMany(x => x).ToArray();

			foreach(Channel channel in channels) {
				//IIntentModuleInstance intent = ApplicationServices.Get<IIntentModuleInstance>(ColorDescriptor._colorIntentId);
				ColorTransitionIntent intent = new ColorTransitionIntent(ColorGradient.Colors.First().Color.ToRGB(), ColorGradient.Colors.Last().Color.ToRGB(), TimeSpan);
				//intent.TimeSpan = TimeSpan;
				//intent.Values = new object[] { LevelCurve, ColorGradient };
				_intents.AddIntentForChannel(channel.Id, intent, TimeSpan.Zero);
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

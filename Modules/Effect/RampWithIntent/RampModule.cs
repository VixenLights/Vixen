using System;
using System.Linq;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace RampWithIntent {
	public class RampModule : EffectModuleInstanceBase {
		private RampData _data;
		private EffectIntents _intents;

		protected override void _PreRender() {
			_intents = new EffectIntents();
			Channel[] channels = TargetNodes.SelectMany(x => x).ToArray();

			foreach(Channel channel in channels) {
				//IIntent intent = new FloatTransitionIntent(StartLevel, EndLevel, TimeSpan);
				//IIntent intent = new PercentageTransitionIntent(StartLevel / 255, EndLevel / 255, TimeSpan);
				IIntent intent = new LongTransitionIntent((long)StartLevel, (long)EndLevel, TimeSpan);
				_intents.AddIntentForChannel(channel.Id, new IntentNode(intent, TimeSpan.Zero));
			}
		}

		protected override EffectIntents _Render() {
			return _intents;
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = (RampData)value; }
		}

		//For automatically-set parameter values...
		[Value]
		public float StartLevel {
			get { return _data.StartLevel; }
			set { _data.StartLevel = value; }
		}

		[Value]
		public float EndLevel {
			get { return _data.EndLevel; }
			set { _data.EndLevel = value; }
		}

	}
}

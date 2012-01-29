using System;
using System.Linq;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Module.Intent;
using Vixen.Sys;

namespace RampWithIntent {
	public class RampModule : EffectModuleInstanceBase {
		private RampData _data;
		private EffectIntents _intents;

		protected override void _PreRender() {
			_intents = new EffectIntents();
			//TargetNodes
			//ParameterValues
			Channel[] channels = TargetNodes.SelectMany(x => x).ToArray();
			//*** it should do this for us; values should always goes through ParameterValues
			//    and that should lead to members in the data being set (the latter being
			//    a module responsibility).
			//if(ParameterValues.Length != 2) throw new InvalidOperationException("Invalid number of parameters.");
			//StartLevel = ParameterValues[0].DynamicCast<Level>();
			//EndLevel = ParameterValues[1].DynamicCast<Level>();

			foreach(Channel channel in channels) {
				IIntentModuleInstance intent = ApplicationServices.Get<IIntentModuleInstance>(RampDescriptor._levelIntentId);
				intent.TimeSpan = TimeSpan;
				intent.Values = new object[] { StartLevel, EndLevel };
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
		public Level StartLevel {
			get { return _data.StartLevel; }
			set { _data.StartLevel = value; }
		}

		[Value]
		public Level EndLevel {
			get { return _data.EndLevel; }
			set { _data.EndLevel = value; }
		}

	}
}

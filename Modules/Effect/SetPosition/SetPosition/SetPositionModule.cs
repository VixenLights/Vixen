using System;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.SetPosition {
	public class SetPositionModule : EffectModuleInstanceBase {
		private SetPositionData _data;
		private EffectIntents _effectIntents;

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = (SetPositionData)value; }
		}

		// Using Vixen-defined PositionValue for two reasons:
		// 1. It's defined to be limited to values between 0 and 1.
		// 2. An editor can be defined against that type and work for other
		//    effects that need a 0-1 value editor.

		[Value]
		public PositionValue StartPosition {
			get { return _data.StartPosition; }
			set {
				_data.StartPosition = value;
				IsDirty = true;
			}
		}

		[Value]
		public PositionValue EndPosition {
			get { return _data.EndPosition; }
			set {
				_data.EndPosition = value;
				IsDirty = true;
			}
		}

		protected override void _PreRender() {
			_effectIntents = new EffectIntents();

			foreach(ChannelNode node in TargetNodes) {
				foreach(Channel channel in node.GetChannelEnumerator()) {
					IIntent intent = new PositionIntent(StartPosition, EndPosition, TimeSpan);
					_effectIntents.AddIntentForChannel(channel.Id, intent, TimeSpan.Zero);
				}
			}
		}

		protected override EffectIntents _Render() {
			return _effectIntents;
		}
	}
}

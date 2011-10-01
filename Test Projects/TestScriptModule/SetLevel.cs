using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Commands.KnownDataTypes;

namespace TestScriptModule {
	public class SetLevel : EffectModuleInstanceBase {
		private ChannelData _channelData = null;
		private SetLevelData _data;

		protected override void _PreRender() {
			_channelData = SetLevelBehavior.Render(TargetNodes, TimeSpan, _data.Level);
		}

		protected override ChannelData _Render() {
			return _channelData;
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as SetLevelData; }
		}

		public override object[] ParameterValues {
			get { return new object[] { _data.Level }; }
			set {
				_data.Level = (Level)value[0];
				IsDirty = true;
			}
		}
	}
}

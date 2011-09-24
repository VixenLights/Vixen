using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using CommandStandard.Types;

namespace TestScriptModule {
	public class Nested : EffectModuleInstanceBase {
		private ChannelData _channelData = null;
		private NestedData _data;

		protected override void _PreRender() {
			_channelData = NestedBehavior.Render(TargetNodes, TimeSpan, ParameterValues);
		}

		protected override ChannelData _Render() {
			return _channelData;
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as NestedData; }
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

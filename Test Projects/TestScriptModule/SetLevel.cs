using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using CommandStandard;

namespace TestScriptModule {
	public class SetLevel : EffectModuleInstanceBase {
		private ChannelData _channelData = null;

		protected override void _PreRender() {
			_channelData = SetLevelBehavior.Render(TargetNodes, TimeSpan, ParameterValues);
		}

		protected override ChannelData _Render() {
			return _channelData;
		}
	}
}

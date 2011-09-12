using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using CommandStandard;

namespace TestScriptModule {
	public class Nested : EffectModuleInstanceBase {
		private ChannelData _channelData = null;

		override protected void _PreRender(ChannelNode[] nodes, TimeSpan timeSpan, object[] parameterValues) {
			_channelData = NestedBehavior.Render(nodes, timeSpan, parameterValues);
		}

		override protected ChannelData _Render(ChannelNode[] nodes, TimeSpan timeSpan, object[] parameterValues) {
			PreRender(nodes, timeSpan, parameterValues);
			return _channelData;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;

namespace Vixen.Sys {
	/// <summary>
	/// Qualifies an effect with parameter values.
	/// </summary>
	public class Command {
		private IEffectModuleInstance _effect;
		
		public Command(Guid effectId, params object[] parameterValues) {
			_effect = Modules.ModuleManagement.GetEffect(effectId);
			if(_effect == null) throw new ArgumentException("Effect does not exist.");
			ParameterValues = parameterValues;
		}

		public Guid EffectId {
			get { return _effect.Descriptor.TypeId; }
		}

		public void PreRender(CommandNode commandNode, long commandTimeSpan) {
			_effect.PreRender(commandNode.TargetNodes.ToArray(), commandTimeSpan, ParameterValues);
		}

		public ChannelData Render(CommandNode commandNode, long commandTimeSpan, long renderStartTime, long renderTimeSpan) {
			// We're adding only the parameter values.
			ChannelData data = _effect.Render(commandNode.TargetNodes.ToArray(), commandTimeSpan, ParameterValues);
			
			// And then restricting by time.
			long renderEndTime = renderStartTime + renderTimeSpan;
			data = ChannelData.Restrict(data, renderStartTime, renderEndTime);

			return data;
		}

		public object[] ParameterValues { get; set; }
	}
}

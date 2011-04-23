using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using CommandStandard;

namespace Vixen.Module.Effect {
    public interface IEffect {
		CommandData[][] Generate(int channelCount, int intervalCount, params object[] parameterValues);
		string EffectName { get; }
		CommandParameterSpecification[] Parameters { get; }
    }
}

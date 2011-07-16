using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;

namespace Vixen.Module.Effect {
	public interface IEffectModuleDescriptor : IModuleDescriptor {
		string EffectName { get; }
		CommandParameterSpecification[] Parameters { get; }
	}
}

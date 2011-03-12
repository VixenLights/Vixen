using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;

namespace Vixen.Module.Effect {
	public interface IEffectModuleDescriptor : IModuleDescriptor {
		string CommandName { get; }
		CommandParameterSpecification[] Parameters { get; }
	}
}

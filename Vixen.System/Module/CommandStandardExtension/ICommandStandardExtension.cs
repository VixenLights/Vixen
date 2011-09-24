using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;

namespace Vixen.Module.CommandStandardExtension {
	/// <summary>
	/// Defines a custom command.
	/// </summary>
    public interface ICommandStandardExtension {
		string Name { get; }
		byte CommandPlatform { get; }
		byte CommandCategory { get; }
		byte CommandIndex { get; }
		CommandParameterSpecification[] Parameters { get; }
		CommandParameterCombiner ParameterCombiner { get; }
    }
}

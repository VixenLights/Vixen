using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.CommandStandardExtension {
	// Hate doing this since it adds no value, but it keeps with the pattern.
	abstract public class CommandStandardExtensionModuleInstanceBase : ModuleInstanceBase, ICommandStandardExtensionModuleInstance, IEqualityComparer<ICommandStandardExtensionModuleInstance> {
		abstract public string Name { get; }

		abstract public byte CommandPlatform { get; }

		abstract public byte CommandCategory { get; }

		abstract public byte CommandIndex { get; }

		abstract public CommandParameterSpecification[] Parameters { get; }

		public bool Equals(ICommandStandardExtensionModuleInstance x, ICommandStandardExtensionModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ICommandStandardExtensionModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}

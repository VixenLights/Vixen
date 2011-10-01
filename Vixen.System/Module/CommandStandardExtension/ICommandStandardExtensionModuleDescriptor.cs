using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.CommandStandardExtension {
	public interface ICommandStandardExtensionModuleDescriptor : IModuleDescriptor {
		string CommandName { get; }
		byte CommandPlatform { get; }
		byte CommandIndex { get; }
	}
}

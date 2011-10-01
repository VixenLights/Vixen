using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Module.CommandStandardExtension {
	/// <summary>
	/// Defines a custom command.
	/// </summary>
    public interface ICommandStandardExtension {
		string Name { get; }
		byte CommandPlatform { get; }
		byte CommandIndex { get; }
		Command GetCommand();
    }
}

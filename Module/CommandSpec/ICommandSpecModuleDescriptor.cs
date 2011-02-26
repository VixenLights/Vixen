using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;

namespace Vixen.Module.CommandSpec {
	public interface ICommandSpecModuleDescriptor : IModuleDescriptor {
		string CommandName { get; }
		CommandParameterSpecification[] Parameters { get; }
	}
}

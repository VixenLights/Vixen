using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using CommandStandard;

namespace Vixen.Module.CommandSpec {
	// This has the knowledge of the command behavior, Command does not.  Command is a
	// command spec with parameter values to give it meaning.
    public interface ICommandSpec {
		CommandData[][] Generate(int channelCount, int intervalCount, params object[] parameterValues);
		string CommandName { get; }
		CommandParameterSpecification[] Parameters { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sequence;
using CommandStandard;
using Vixen.Module.CommandSpec;
using Vixen.Sys;
using Vixen.Module.Sequence;

namespace Vixen.Script {
	// This class exists only to provide the templated UserScriptSequence with
	// the necessary data.
	public partial class UserScriptSequence {
		private ScriptSequenceBase _sequence;
		// Command name : ICommandSpecModuleInstance
		private Dictionary<string, ICommandSpecModuleInstance> _commands;

		public UserScriptSequence(ScriptSequenceBase sequence) {
			_sequence = sequence;
			Namespace = ScriptHostGenerator.UserScriptNamespace;
			ClassName = ScriptHostGenerator.Mangle(sequence.Name);
			EntryPointName = "Start";

			ICommandSpecModuleInstance[] commandSpecs = Server.ModuleManagement.GetAllCommandSpec();
			_commands = new Dictionary<string,ICommandSpecModuleInstance>();
			foreach(var commandNames in commandSpecs.GroupBy(x => x.CommandName)) {
				if(commandNames.Count() == 1) {
					// Name is unique;
					_commands[ScriptHostGenerator.Mangle(commandNames.Key)] = commandNames.First();
				} else {
					// Name is not unique.
					// Append an index to each to make it unique.
					int i=1;
					foreach(var command in commandNames) {
						_commands[ScriptHostGenerator.Mangle(commandNames.Key) + "_" + i++] = command;
					}
				}
			}
		}

		public string ClassName { get; private set; }

		public string EntryPointName { get; set; }

		public string Namespace { get; private set; }
	}
}

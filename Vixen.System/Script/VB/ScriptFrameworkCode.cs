using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sequence;
using CommandStandard;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Module.Sequence;

// This is the same for either language, but a copy needs to be in each namespace
// because they are part of the partial class completed by the T4 class.

namespace Vixen.Script.VB {
	// This class exists only to provide the templated UserScriptSequence with
	// the necessary data.
	public partial class ScriptFramework : IScriptFrameworkGenerator {
		private ScriptSequenceBase _sequence;
		// Command name : IEffectModuleInstance
		private Dictionary<string, IEffectModuleInstance> _commands;

		public ScriptFramework() {
			IEffectModuleInstance[] effects = Server.ModuleManagement.GetAllEffect();
			_commands = new Dictionary<string,IEffectModuleInstance>();
			foreach(var commandNames in effects.GroupBy(x => x.CommandName)) {
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

		public ScriptSequenceBase Sequence {
			get { return _sequence; }
			set {
				_sequence = value;
				EntryPointName = "Start";
				Namespace = ScriptHostGenerator.UserScriptNamespace;
				ClassName = ScriptHostGenerator.Mangle(_sequence.Name);
			}
		}
	}
}

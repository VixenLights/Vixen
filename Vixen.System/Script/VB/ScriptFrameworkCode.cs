using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private ScriptSequence _sequence;
		// Command name : IEffectModuleInstance
		private Dictionary<string, IEffectModuleInstance> _effects;

		public ScriptFramework() {
			IEffectModuleInstance[] effects = VixenSystem.ModuleManagement.GetAllEffect();
			_effects = new Dictionary<string,IEffectModuleInstance>();
			foreach(var effectNames in effects.GroupBy(x => x.EffectName)) {
				if(effectNames.Count() == 1) {
					// Name is unique;
					_effects[ScriptHostGenerator.Mangle(effectNames.Key)] = effectNames.First();
				} else {
					// Name is not unique.
					// Append an index to each to make it unique.
					int i=1;
					foreach(var command in effectNames) {
						_effects[ScriptHostGenerator.Mangle(effectNames.Key) + "_" + i++] = command;
					}
				}
			}
		}

		public string ClassName { get; private set; }

		public string EntryPointName { get; set; }

		public string Namespace { get; private set; }

		public ScriptSequence Sequence {
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

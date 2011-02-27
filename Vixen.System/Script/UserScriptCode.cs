using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Sequence;

namespace Vixen.Script {
	public partial class UserScript {
		public UserScript(ScriptSequenceBase sequence) {
			Namespace = ScriptHostGenerator.UserScriptNamespace;
			ClassName = ScriptHostGenerator.Mangle(sequence.Name);
		}

		public string Namespace { get; private set; }

		public string ClassName { get; private set; }
	}
}

using System.Collections.Generic;
using Vixen.Module.Script;

namespace Common.ScriptSequence.Script {
	class ScriptCompilerParameters : ICompilerParameters {
		public ScriptCompilerParameters() {
			ReferencedAssemblies = new List<string>();
		}

		public bool GenerateInMemory { get; set; }
		public bool IncludeDebugInformation { get; set; }
		public List<string> ReferencedAssemblies { get; private set; }
	}
}

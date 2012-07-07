using System.Collections.Generic;

namespace Vixen.Module.Script {
	public interface ICompilerParameters {
		bool GenerateInMemory { get; set; }
		bool IncludeDebugInformation { get; set; }
		List<string> ReferencedAssemblies { get; }
	}
}

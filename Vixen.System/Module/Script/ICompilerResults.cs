using System.Reflection;

namespace Vixen.Module.Script {
	public interface ICompilerResults {
		ICompilerError[] Errors { get; }
		bool HasErrors { get; }
		Assembly CompiledAssembly { get; }
	}
}

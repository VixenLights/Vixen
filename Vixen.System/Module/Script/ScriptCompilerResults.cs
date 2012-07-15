using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vixen.Module.Script {
	public class ScriptCompilerResults : ICompilerResults {
		public ScriptCompilerResults(CompilerResults compilerResults) {
			CompiledAssembly = (compilerResults.Errors.HasErrors) ? null : compilerResults.CompiledAssembly;
			Errors = compilerResults.Errors.Cast<CompilerError>().Select(x => new ScriptCompilerError(x)).Cast<ICompilerError>().ToArray();
		}

		public ScriptCompilerResults(Assembly compiledAssembly, IEnumerable<ICompilerError> errors) {
			Errors = errors.ToArray();
			CompiledAssembly = compiledAssembly;
		}

		public ICompilerError[] Errors { get; private set; }

		public bool HasErrors {
			get { return Errors.Length > 0; }
		}

		public Assembly CompiledAssembly { get; private set; }
	}
}

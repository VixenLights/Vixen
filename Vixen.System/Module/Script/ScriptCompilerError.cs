using System.CodeDom.Compiler;

namespace Vixen.Module.Script {
	public class ScriptCompilerError : ICompilerError {
		public ScriptCompilerError() {
		}

		public ScriptCompilerError(CompilerError compilerError) {
			Column = compilerError.Column;
			ErrorNumber = compilerError.ErrorNumber;
			ErrorText = compilerError.ErrorText;
			FileName = compilerError.FileName;
			IsWarning = compilerError.IsWarning;
			Line = compilerError.Line;
		}

		public int Column { get; set; }

		public string ErrorNumber { get; set; }

		public string ErrorText { get; set; }

		public string FileName { get; set; }

		public bool IsWarning { get; set; }

		public int Line { get; set; }
	}
}

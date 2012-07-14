using System;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
using Vixen.Module.Script;

namespace VixenModules.Script.VB {
	public class VB_CodeProvider : IScriptCodeProvider {
		VBCodeProvider _codeProvider = new VBCodeProvider();

		public ICompilerResults CompileAssemblyFromFile(ICompilerParameters options, string[] fileNames) {
			if(_codeProvider != null) {
				CompilerParameters compilerParameters = _CreateCompilerParameters(options);
				CompilerResults compilerResults = _codeProvider.CompileAssemblyFromFile(compilerParameters, fileNames);
				return new ScriptCompilerResults(compilerResults);
			}

			throw new ObjectDisposedException("CodeProvider");
		}

		private CompilerParameters _CreateCompilerParameters(ICompilerParameters options) {
			CompilerParameters compilerParameters = new CompilerParameters {
				GenerateInMemory = options.GenerateInMemory
			};

			compilerParameters.ReferencedAssemblies.AddRange(options.ReferencedAssemblies.ToArray());

			return compilerParameters;
		}

		public void Dispose() {
			_codeProvider.Dispose();
			_codeProvider = null;
		}
	}
}

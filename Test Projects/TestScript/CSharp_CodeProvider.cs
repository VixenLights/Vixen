using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Vixen.Script;

namespace TestScript {
	public class CSharp_CodeProvider : IScriptCodeProvider {
		private CSharpCodeProvider _codeProvider = new CSharpCodeProvider();

		public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string[] fileNames) {
			if(_codeProvider != null) {
				return _codeProvider.CompileAssemblyFromFile(options, fileNames);
			}
			throw new ObjectDisposedException("CodeProvider");
		}

		public void Dispose() {
			_codeProvider.Dispose();
			_codeProvider = null;
		}
	}
}

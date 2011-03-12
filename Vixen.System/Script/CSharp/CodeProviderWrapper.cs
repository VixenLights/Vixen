using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Vixen.Script.CSharp {
	class CodeProviderWrapper : ICodeProvider {
		private CSharpCodeProvider _codeProvider = new CSharpCodeProvider();

		public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string[] fileNames) {
			return _codeProvider.CompileAssemblyFromFile(options, fileNames);
		}

		public void Dispose() {
			_codeProvider.Dispose();
		}
	}
}

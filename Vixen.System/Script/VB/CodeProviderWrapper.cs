using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;

namespace Vixen.Script.VB {
	class CodeProviderWrapper : ICodeProvider {
		private VBCodeProvider _codeProvider = new VBCodeProvider();

		public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string[] fileNames) {
			return _codeProvider.CompileAssemblyFromFile(options, fileNames);
		}

		public void Dispose() {
			_codeProvider.Dispose();
		}
	}
}

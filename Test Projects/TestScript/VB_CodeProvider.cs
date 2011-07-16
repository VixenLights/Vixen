using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
using Vixen.Script;

namespace TestScript {
	public class VB_CodeProvider : IScriptCodeProvider {
		VBCodeProvider _codeProvider = new VBCodeProvider();

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

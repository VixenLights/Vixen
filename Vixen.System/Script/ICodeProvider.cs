using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace Vixen.Script {
	public interface ICodeProvider : IDisposable {
		CompilerResults CompileAssemblyFromFile(CompilerParameters options, string[] fileNames);
	}
}

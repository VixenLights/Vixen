using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace Vixen.Script {
	public interface IScriptCodeProvider : IDisposable {
		CompilerResults CompileAssemblyFromFile(CompilerParameters options, string[] fileNames);
	}
}

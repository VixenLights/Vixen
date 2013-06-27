using System;

namespace Vixen.Module.Script
{
	public interface IScriptCodeProvider : IDisposable
	{
		ICompilerResults CompileAssemblyFromFile(ICompilerParameters options, string[] fileNames);
	}
}
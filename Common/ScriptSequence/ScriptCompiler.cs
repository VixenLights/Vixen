using System.Linq;
using Common.ScriptSequence.Script;
using Vixen.Module.Script;

namespace Common.ScriptSequence
{
	internal class ScriptCompiler
	{
		public void Compile(ScriptSequence sequence)
		{
			ScriptHostGenerator hostGenerator = new ScriptHostGenerator();
            ScriptHost = hostGenerator.GenerateScript(sequence);
			Errors = hostGenerator.Errors.ToArray();
		}

		public string[] Errors { get; private set; }

		public bool HasErrors
		{
			get { return Errors.Length > 0; }
		}

		public IUserScriptHost ScriptHost { get; private set; }
	}
}
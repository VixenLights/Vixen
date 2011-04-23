using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Script {
	interface IScriptFrameworkGenerator {
		string ClassName { get; }
		string EntryPointName { get; set; }
		string Namespace { get; }
		ScriptSequence Sequence { get; set; }
		string TransformText();
	}
}

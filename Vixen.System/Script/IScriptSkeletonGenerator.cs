using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Sequence;

namespace Vixen.Script {
	interface IScriptSkeletonGenerator {
		ScriptSequenceBase Sequence { get; set; }
		string TransformText();
	}
}

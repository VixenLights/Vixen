using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;

namespace Vixen.Script {
	public interface IScriptFrameworkGenerator {
		string Generate(string nameSpace, string className);
	}
}

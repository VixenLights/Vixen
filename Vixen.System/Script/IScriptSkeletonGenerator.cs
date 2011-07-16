using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Script {
	public interface IScriptSkeletonGenerator {
		string Generate(string nameSpace, string className);
	}
}

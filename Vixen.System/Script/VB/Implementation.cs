using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Script.VB {
	class Implementation {
		[Preload]
		static private void _RegisterScriptType() {
			Registration.Add(new ScriptTypeImplementation("VB", ".vb", typeof(Skeleton), typeof(ScriptFramework), typeof(CodeProviderWrapper)));
		}
	}
}

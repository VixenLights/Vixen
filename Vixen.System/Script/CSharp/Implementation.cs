using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Script.CSharp {
	class Implementation {
		[Preload]
		static private void _RegisterScriptType() {
			Registration.Add(new ScriptTypeImplementation("C#", ".cs", typeof(Skeleton), typeof(ScriptFramework), typeof(CodeProviderWrapper)));
		}
	}
}

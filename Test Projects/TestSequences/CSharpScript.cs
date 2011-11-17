using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace TestSequences {
	public class CSharpScript : ScriptSequenceModuleInstanceBase {
		public CSharpScript()
			: base("C#") {
		}

	    public override IModuleInstance Clone()
	    {
	        throw new NotImplementedException();
	    }
	}
}

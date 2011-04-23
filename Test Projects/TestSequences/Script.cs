using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace TestSequences {
	public class Script : ScriptSequence, ISequenceModuleInstance {
		public Script() {
			// Set a default interval.
			Data.TimingInterval = 50;
			//For testing...
			Language = "C#";
		}

		public Guid TypeId {
			get { return ScriptSequenceModule._typeId; }
		}

		public string FileExtension {
			get { return ScriptSequenceModule._fileExtension; }
		}


		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() { }
	}
}

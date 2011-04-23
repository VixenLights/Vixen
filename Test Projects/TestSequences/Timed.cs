using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;
//using Vixen.Sequence;
using Vixen.Sys;

namespace TestSequences {
	public class Timed : Sequence, ISequenceModuleInstance {
		public Guid TypeId {
			get { return TimedSequenceModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public void Dispose() { }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public string FileExtension {
			get { return TimedSequenceModule._fileExtension; }
		}
	}
}

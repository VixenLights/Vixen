using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;
//using Vixen.Sequence;
using Vixen.Sys;
using Vixen.Module.RuntimeBehavior;

namespace TestSequences {
	public class Recording : Sequence, ISequenceModuleInstance {
		public Recording() {
			// Required to execute (length > 0).
			Length = Forever;
			// Required for data sync.
			Data.TimingInterval = 50;
			// Enable the live behavior in a bad hacky fashion.
			RuntimeBehaviors.First(x => x.TypeName.Contains("Recording")).Enabled = true;
		}

		public Guid TypeId {
			get { return RecordingSequenceModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public void Dispose() { }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public string FileExtension {
			get { return RecordingSequenceModule._fileExtension; }
		}
	}
}

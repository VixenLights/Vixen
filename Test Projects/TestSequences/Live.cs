using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace TestSequences {
	public class Live : Sequence, ISequenceModuleInstance {
		public Live()
		{
			Length = Forever;
			Data.TimingInterval = 50;
			// Enable the live behavior in a bad hacky fashion, since this is only a test sequence type.
			RuntimeBehaviors.First(x => x.TypeName == "Live").Enabled = true;
		}

		public Guid TypeId {
			get { return LiveSequenceModule._typeId; }
		}

		public string FileExtension {
			get { return LiveSequenceModule._fileExtension; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() { }
	}
}

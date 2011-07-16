using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Sequence {
	abstract public class ScriptSequenceModuleInstanceBase : ScriptSequence, ISequenceModuleInstance {
		protected ScriptSequenceModuleInstanceBase(string language)
			: base(language) {
		}

		public string FileExtension {
			get { return (Descriptor as ISequenceModuleDescriptor).FileExtension; }
		}

		public Guid InstanceId { get; set; }

		virtual public IModuleDataModel ModuleData { get; set; }

		virtual public IModuleDescriptor Descriptor { get; set; }

		virtual public void Dispose() { }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace VixenModules.Sequence.Timed {
	public class TimedSequence : SequenceModuleInstanceBase
	{
		public TimedSequence()
		{
		}

		public TimedSequence(TimedSequence original)
			: base(original)
		{
		}

		public List<MarkCollection> MarkCollections
		{
			get { return (ModuleData as TimedSequenceData).MarkCollections; }
			set { (ModuleData as TimedSequenceData).MarkCollections = value; }
		}

		public override IModuleInstance Clone() {
			return new TimedSequence(this);
		}
	}
}

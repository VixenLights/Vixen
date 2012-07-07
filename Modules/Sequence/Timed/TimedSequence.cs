using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.SequenceType;
using Vixen.Sys;

namespace VixenModules.Sequence.Timed {
	public class TimedSequence : BaseSequence.Sequence {
		public List<MarkCollection> MarkCollections {
			get { return (SequenceData as TimedSequenceData).MarkCollections; }
			set { (SequenceData as TimedSequenceData).MarkCollections = value; }
		}
	}

	//public class TimedSequence : SequenceTypeModuleInstanceBase
	//{
	//    public TimedSequence()
	//    {
	//    }

	//    public TimedSequence(TimedSequence original)
	//        : base(original)
	//    {
	//    }

	//    public List<MarkCollection> MarkCollections
	//    {
	//        get { return (ModuleData as TimedSequenceData).MarkCollections; }
	//        set { (ModuleData as TimedSequenceData).MarkCollections = value; }
	//    }

	//    public override IModuleInstance Clone() {
	//        return new TimedSequence(this);
	//    }
	//}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace VixenModules.Sequence.Timed {
	public class TimedSequenceModuleDescriptor : SequenceModuleDescriptorBase {
		private Guid _typeId = new Guid("{4C258A3B-E725-4AE7-B50B-103F6AB8121E}");

		override public string FileExtension {
			get { return ".tim"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(TimedSequence); }
		}

		override public Type ModuleDataClass {
			get { return null; }
		}

		override public string Author {
			get { return "Vixen Team"; }
		}

		override public string TypeName {
			get { return "Timed Sequence"; }
		}

		override public string Description {
			get { return "A basic timed sequence, which is a collection of effects configured to occur at predefined times."; }
		}

		override public string Version {
			get { return "0.1"; }
		}
	}
}

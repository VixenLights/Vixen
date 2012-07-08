using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.SequenceType;

namespace VixenModules.Sequence.Timed {
	public class TimedSequenceModuleDescriptor : SequenceTypeModuleDescriptorBase {
		private Guid _typeId = new Guid("{296bdba2-9bf3-4bff-a9f2-13efac5c8ecb}");

		override public string FileExtension {
			get { return ".tim"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(TimedSequenceTypeModule); }
		}

		override public Type ModuleDataClass {
			get { return typeof(TimedSequenceData); }
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
			get { return "1.0"; }
		}

		public override int ClassVersion {
			get { return 3; }
		}
	}
}

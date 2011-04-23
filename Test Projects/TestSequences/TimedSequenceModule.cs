using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace TestSequences {
	public class TimedSequenceModule : ISequenceModuleDescriptor {
		static internal Guid _typeId = new Guid("{4C258A3B-E725-4ae7-B50B-103F6AB8121E}");
		static internal string _fileExtension = ".tim";

		public string FileExtension {
			get { return _fileExtension; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(Timed); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Timed sequence"; }
		}

		public string Description {
			get { throw new NotImplementedException(); }
		}

		public string Version {
			get { throw new NotImplementedException(); }
		}

		public string FileName { get; set; }

		public string ModuleTypeName { get; set; }
	}
}

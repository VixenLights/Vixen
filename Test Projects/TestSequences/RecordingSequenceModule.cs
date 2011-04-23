using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace TestSequences {
	public class RecordingSequenceModule : ISequenceModuleDescriptor {
		static internal Guid _typeId = new Guid("{8D61518B-22D9-4c44-8271-D08AC64D5B19}");
		static internal string _fileExtension = ".rec";

		public string FileExtension {
			get { return _fileExtension; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(Recording); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Recording sequence"; }
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

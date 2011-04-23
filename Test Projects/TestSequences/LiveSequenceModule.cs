using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace TestSequences {
	public class LiveSequenceModule : ISequenceModuleDescriptor {
		static internal Guid _typeId = new Guid("{CC7FCE3E-8A36-42e1-AF65-0EA9E5331959}");
		static internal string _fileExtension = ".liv";

		public string FileExtension {
			get { return _fileExtension; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(Live); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Live sequence"; }
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

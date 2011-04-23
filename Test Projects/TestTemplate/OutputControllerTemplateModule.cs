using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Module.FileTemplate;

namespace TestTemplate {
	public class OutputControllerTemplateModule : IFileTemplateModuleDescriptor {
		static internal Guid _typeId = new Guid("{D7C0DF55-C3E2-416e-AC23-BF3BCEFDCAEE}");

		public string FileType {
			get { return ".out"; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(OutputControllerTemplate); }
		}

		public Type ModuleDataClass {
			get { return typeof(OutputControllerTemplateData); }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Output controller template"; }
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

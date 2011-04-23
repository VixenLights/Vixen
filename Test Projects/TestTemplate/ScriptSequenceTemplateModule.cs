using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Module.FileTemplate;

namespace TestTemplate {
	public class ScriptSequenceTemplateModule : IFileTemplateModuleDescriptor {
		static internal Guid _typeId = new Guid("{F46FB19C-28D6-458e-8646-96E23A59BD96}");

		public string FileType {
			get { return ".scr"; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(ScriptSequenceTemplate); }
		}

		public Type ModuleDataClass {
			get { return typeof(ScriptSequenceTemplateData); }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Scripted sequence template"; }
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

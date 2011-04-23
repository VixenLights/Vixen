using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Module.Editor;

namespace TestEditor {
	public class Module : IEditorModuleDescriptor {
		static internal Guid _typeId = new Guid("{B833C1AF-2397-419e-8B6B-DFE64511B57B}");
		private string[] _extensions = new string[] { ".tim" };

		public string[] FileExtensions {
			get { return _extensions; }
		}

		//public string FileTypeName {
		//    get { return "Standard Vixen sequence"; }
		//}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(NotARealEditor); }
		}

		public Type ModuleDataClass {
			get { return typeof(NotARealEditorDataModel); }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Standard Vixen sequence"; }
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

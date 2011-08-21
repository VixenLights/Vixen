using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Editor;

namespace TestEditor {
	public class Module : EditorModuleDescriptorBase {
		private Guid _typeId = new Guid("{B833C1AF-2397-419e-8B6B-DFE64511B57B}");
		private string[] _extensions = new string[] { ".tim" };

		override public string[] FileExtensions {
			get { return _extensions; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(NotARealEditor); }
		}

		override public Type ModuleDataClass {
			get { return typeof(NotARealEditorDataModel); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Standard Vixen sequence"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}

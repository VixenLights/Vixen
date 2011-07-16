using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Module.Editor;

namespace TestEditor {
	public class CSharp_ScriptEditorModule : EditorModuleDescriptorBase {
		private Guid _typeId = new Guid("{CEFF9B1C-BB75-4f76-96C2-C0BBADB75035}");
		private string[] _extensions = new string[] { ".csp" };

		override public string[] FileExtensions {
			get { return _extensions; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(CSharp_ScriptEditor); }
		}

		override public Type ModuleDataClass {
			get { return null; }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Vixen script project"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}

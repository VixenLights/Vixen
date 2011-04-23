using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Module.Editor;

namespace TestEditor {
	public class ScriptEditorModule : IEditorModuleDescriptor {
		static internal Guid _typeId = new Guid("{CEFF9B1C-BB75-4f76-96C2-C0BBADB75035}");
		private string[] _extensions = new string[] { ".scr" };

		public string[] FileExtensions {
			get { return _extensions; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(ScriptEditor); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Vixen script project"; }
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

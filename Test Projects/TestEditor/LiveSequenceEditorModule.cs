using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Module.Editor;

namespace TestEditor {
	public class LiveSequenceEditorModule : IEditorModuleDescriptor {
		static internal Guid _typeId = new Guid("{CDA705A5-89D4-4118-8D70-60AB8121E220}");
		private string[] _extensions = new string[] { ".liv" };

		public string[] FileExtensions {
			get { return _extensions; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(LiveEditor); }
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

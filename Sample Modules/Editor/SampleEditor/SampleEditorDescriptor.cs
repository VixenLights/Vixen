using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Editor;

namespace SampleEditor {
	public class SampleEditorDescriptor : EditorModuleDescriptorBase {
		private Guid _typeId = new Guid("{01AE1AED-D882-4bfb-BC5D-6356E061E12D}");

		public override string TypeName {
			get { return "Sample Editor"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(SampleEditorModule); }
		}

		public override string Author {
			get { return "Vixen Development Team"; }
		}

		public override string Description {
			get { return "Sample editor module"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override string[] FileExtensions {
			get { return new[] { ".fil" }; }
		}

		public override Type EditorUserInterfaceClass {
			get { return typeof(TheEditor); }
		}
	}
}

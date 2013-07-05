using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.WipeEditor {
	public class WipeEditorDescriptor : EffectEditorModuleDescriptorBase {

		private static Guid _typeId = new Guid("{F3B6A86C-8125-49E4-86C4-FCBF488F83DF}");
		private static Guid _wipeTypeId = new Guid("{61746B54-A96C-4723-8BD6-39C7EA985F80}");

		public override string Author {
			get { return "Darren McDaniel"; }
		}

		public override string Description {
			get { return "An editor for the Wipe effect."; }
		}

		public override Type ModuleClass {
			get { return typeof(WipeEditorModule); }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override string TypeName {
			get { return "Wipe Effect Editor"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Type[] ParameterSignature {
			get { return null; }
		}

		public override Guid EffectTypeId {
			get { return _wipeTypeId; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Value;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.PositionValueEditor {
	public class PositionValueEditorDescriptor : EffectEditorModuleDescriptorBase {
		private Guid _typeId = new Guid("{8B188AB2-E358-49D6-B276-4273F7D54D93}");

		public override string TypeName {
			get { return "PositionValue editor"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(PositionValueEditorModule); }
		}

		public override string Author {
			get { return "Vixen Team"; }
		}

		public override string Description {
			get { return "Editor for values between 0 and 1."; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Guid EffectTypeId {
			get { return Guid.Empty; }
		}

		public override Type[] ParameterSignature {
			get { return new[] { typeof(PositionValue) }; }
		}
	}
}

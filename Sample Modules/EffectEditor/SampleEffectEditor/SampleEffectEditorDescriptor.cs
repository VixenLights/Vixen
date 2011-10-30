using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace SampleEffectEditor {
	public class SampleEffectEditorDescriptor : EffectEditorModuleDescriptorBase {
		private Guid _typeId = new Guid("{B8C49489-C282-4921-B1A9-E1ED3292AEB1}");
		private Guid _effectId = new Guid("{C4E3BF3D-4B38-407e-8122-0404D604C4E1}");

		public override string TypeName {
			get { return "Level editor"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(SampleEffectEditorModule); }
		}

		public override string Author {
			get { return "Vixen Development Team"; }
		}

		public override string Description {
			get { return "Edits a Level-type parameter"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Guid EffectTypeId {
			get { return _effectId; }
		}

		public override Type[] ParameterSignature {
			get { return new[] { typeof(Level) }; }
		}
	}
}

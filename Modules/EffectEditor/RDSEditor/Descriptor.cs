using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.RDSEditor
{
    public class Descriptor : EffectEditorModuleDescriptorBase {
		private static Guid _typeId = new Guid("{1ECE5C0F-AEA1-4097-86E0-6364705283C2}");
		private static Guid _effectId = new Guid("{AFB67313-A17B-4FC3-9E0D-B564934423D5}");

		public override string TypeName {
			get { return "RDS Editor"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(Module); }
		}

		public override string Author {
			get { return "Vixen Team"; }
		}

		public override string Description {
			get { return "RDS Editor"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Guid EffectTypeId {
			get { return _effectId; }
		}

		public override Type[] ParameterSignature {
			get { return new[] { typeof(String) }; }
		}
	}

}

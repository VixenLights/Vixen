using System;
using Vixen.Module.SequenceType;
using Vixen.Sys.Attribute;

namespace VixenModules.SequenceType.Script {
	public class ScriptDescriptor : SequenceTypeModuleDescriptorBase {
		static ScriptDescriptor() {
			ScriptSourceDirectory = "ScriptSource";
		}

		[ModuleDataPath]
		internal static string ScriptSourceDirectory { get; set; }

		private Guid _typeId = new Guid("{CD5CA8E5-10D8-4342-9A42-AED48209C7CC}");

		public override string TypeName {
			get { return "Scripted sequence"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(ScriptModule); }
		}

		public override Type ModuleDataClass {
			get { return typeof(ScriptData); }
		}

		public override string Author {
			get { return "Vixen Team"; }
		}

		public override string Description {
			get { return "Sequence that is authored as .NET code."; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override string FileExtension {
			get { return ".scr"; }
		}

		public override int ClassVersion {
			get { return 1; }
		}
	}
}

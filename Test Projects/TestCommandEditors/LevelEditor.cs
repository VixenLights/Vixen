using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace TestCommandEditors {
	public class LevelEditor : IEffectEditorModuleInstance {
		private CommandParameterSpecification[] _paramSpec = { new CommandParameterSpecification("Level", typeof(Level)) };
		
		public IEffectEditorControl CreateEditorControl() {
			return new ();
		}

		public Guid EffectTypeId {
			get { return Guid.Empty; }
		}

		public CommandParameterSpecification[] CommandSignature {
			get { return _paramSpec; }
		}

		public Guid TypeId {
			get { return LevelEditorModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() { }
	}
}

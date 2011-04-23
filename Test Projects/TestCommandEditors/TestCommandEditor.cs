using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace TestCommandEditors {
	public class TestCommandEditor : IEffectEditorModuleInstance {
		private static Guid _targetCommandId = new Guid("{88D2A581-CC6D-4e15-85E3-F235F14336BC}");
		private CommandParameterSpecification[] _paramSpec = { new CommandParameterSpecification("Level", typeof(Level)) };

		public IEffectEditorControl CreateEditorControl() {
			return new TestCommandEditorControl();
		}

		public Guid EffectTypeId {
			get { return _targetCommandId; }
		}

		public CommandParameterSpecification[] CommandSignature {
			get { return _paramSpec; }
		}

		public Guid TypeId {
			get { return TestCommandEditorModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() { }
	}
}

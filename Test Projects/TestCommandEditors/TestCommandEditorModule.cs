using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module.EffectEditor;

namespace TestCommandEditors {
	public class TestCommandEditorModule : EffectEditorModuleDescriptorBase {
		private Guid _typeId = new Guid("{BA73EAC1-66D8-488e-9889-4E979557D72D}");
		private Guid _targetCommandId = new Guid("{88D2A581-CC6D-4e15-85E3-F235F14336BC}");
		private CommandParameterSpecification[] _paramSpec = { new CommandParameterSpecification("Level", typeof(Level)) };
		
		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(TestCommandEditor); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Test command editor"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}

		override public Guid EffectTypeId {
			get { return _targetCommandId; }
		}

		override public CommandParameterSpecification[] CommandSignature {
			get { return _paramSpec; }
		}
	}
}

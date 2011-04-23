using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module.Effect;

namespace TestScriptModule {
	public class SetLevelModule : IEffectModuleDescriptor {
		static internal Guid _typeId = new Guid("{603E3297-994C-4705-9F17-02A62ECC14B5}");
		static internal string _commandName = "Set level";
		static internal CommandParameterSpecification[] _parameters = { new CommandParameterSpecification("Level", typeof(Level)) };

		public string CommandName {
			get { return _commandName; }
		}

		public CommandParameterSpecification[] Parameters {
			get { return _parameters; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(SetLevel); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return CommandName; }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module.Effect;

namespace TestScriptModule {
	public class PulseModule : IEffectModuleDescriptor {
		static internal Guid _typeId = new Guid("{88D2A581-CC6D-4e15-85E3-F235F14336BC}");
		static internal string _effectName = "Super hard math...thing";
		static internal CommandParameterSpecification[] _parameters =
			new CommandParameterSpecification[] { 
					new CommandParameterSpecification("level", typeof(Level))
				};

		//public string[] CommandsHandled {
		//    get {
		//        return new string[] {
		//            "000000"
		//        };
		//    }
		//}
		public string CommandName {
			get { return _effectName; }
		}

		public CommandParameterSpecification[] Parameters {
			get { return _parameters; }
		}

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(PulseInstance); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Pulse"; }
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

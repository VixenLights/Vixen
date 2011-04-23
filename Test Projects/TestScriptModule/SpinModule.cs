using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module.Effect;

namespace TestScriptModule {
	public class SpinModule : IEffectModuleDescriptor {
		static internal Guid _typeId = new Guid("{0154D85D-628C-4ddb-978D-26C2745E7803}");
		static internal string _effectName = "Spin";
		static internal CommandParameterSpecification[] _parameters =
		{
		};
		
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
			get { return typeof(Spin); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Spin"; }
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

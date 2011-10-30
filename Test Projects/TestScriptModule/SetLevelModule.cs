using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace TestScriptModule {
	public class SetLevelModule : EffectModuleDescriptorBase {
		private Guid _typeId = new Guid("{603E3297-994C-4705-9F17-02A62ECC14B5}");
		static internal Guid _rgbProperty = new Guid("{55960E71-2151-454c-885E-00B9713A93EF}");
		//private Guid[] _dependencies;
		private string _commandName = "Set level (test)";
		private ParameterSignature _parameters = new ParameterSignature(new ParameterSpecification("Level", typeof(Level)));

		public SetLevelModule() {
			//_dependencies = new[] {
			//    _rgbProperty
			//};
		}

		override public string EffectName {
			get { return _commandName; }
		}

		override public ParameterSignature Parameters {
			get { return _parameters; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(SetLevel); }
		}

		public override Type ModuleDataClass {
			get { return typeof(SetLevelData); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return EffectName; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}

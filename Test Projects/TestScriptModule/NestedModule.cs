using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace TestScriptModule {
	public class NestedModule : EffectModuleDescriptorBase {
		private Guid _typeId = new Guid("{82526F5B-BE3E-4589-B3C7-4D33ABA5BB08}");
		private Guid _rgbProperty = new Guid("{55960E71-2151-454c-885E-00B9713A93EF}");
		static internal Guid _setLevelEffect = new Guid("{603E3297-994C-4705-9F17-02A62ECC14B5}");
		private Guid[] _dependencies;
		private string _commandName = "Nested RGB set level";
		// Only using a start-level parameter because I don't want to create an editor right now.
		private CommandParameterSignature _parameters = new CommandParameterSignature(new CommandParameterSpecification("Target end level", typeof(Level)));

		public NestedModule() {
			_dependencies = new[] {
				_rgbProperty,
				_setLevelEffect
			};
		}

		override public string EffectName {
			get { return _commandName; }
		}

		override public CommandParameterSignature Parameters {
			get { return _parameters; }
		}

		override public string TypeName {
			get { return EffectName; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(Nested); }
		}

		public override Type ModuleDataClass {
			get { return typeof(NestedData); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}

		override public Guid[] Dependencies {
			get { return _dependencies; }
		}
	}
}

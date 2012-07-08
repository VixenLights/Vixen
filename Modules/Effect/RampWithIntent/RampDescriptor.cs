using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace RampWithIntent {
	public class RampDescriptor : EffectModuleDescriptorBase {
		private Guid _typeId = new Guid("{CCB599F3-41B2-4f84-9EDA-9EF84E3F80A3}");
		private ParameterSignature _signature;

		public RampDescriptor() {
			_signature = new ParameterSignature(
				new ParameterSpecification("Start level", typeof(float)),
				new ParameterSpecification("End level", typeof(float)));
		}

		public override string TypeName {
			get { return "Ramp (intent)"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(RampModule); }
		}

		public override Type ModuleDataClass {
			get { return typeof(RampData); }
		}

		public override string Author {
			get { throw new NotImplementedException(); }
		}

		public override string Description {
			get { throw new NotImplementedException(); }
		}

		public override string Version {
			get { throw new NotImplementedException(); }
		}

		public override string EffectName {
			get { return TypeName; }
		}

		public override ParameterSignature Parameters {
			get { return _signature; }
		}

		//public override Guid[] Dependencies {
		//    get { return new[] { _levelIntentId }; }
		//}
	}
}

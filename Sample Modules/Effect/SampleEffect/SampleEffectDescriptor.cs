using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace SampleEffect {
	public class SampleEffectDescriptor : EffectModuleDescriptorBase {
		private Guid _typeId = new Guid("{C4E3BF3D-4B38-407e-8122-0404D604C4E1}");

		public SampleEffectDescriptor() {
			Parameters = new ParameterSignature(new[] {
				new ParameterSpecification("Flash level", typeof(Level))
			});				
		}

		public override string TypeName {
			get { return "Sample effect"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(SampleEffectModule); }
		}

		public override string Author {
			get { return "Vixen Development Team"; }
		}

		public override string Description {
			get { return "Sample effect module"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override string EffectName {
			get { return "Blinky Flashy"; }
		}

		public override Type ModuleDataClass {
			get { return typeof(SampleEffectData); }
		}
	}
}

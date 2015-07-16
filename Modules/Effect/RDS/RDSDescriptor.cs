using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.RDS {
	public class RDSDescriptor : EffectModuleDescriptorBase {
		private static Guid _typeId = new Guid("{AFB67313-A17B-4FC3-9E0D-B564934423D5}");

		public override string EffectName {
			get { return "RDS"; }
		}

		public override EffectGroups EffectGroup {
			get { return EffectGroups.Device; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(RDSModule); }
		}

		public override Type ModuleDataClass {
			get { return typeof(RDSData); }
		}

		public override string Author {
			get { return "Darren McDaniel / Steve Dupuis"; }
		}

		public override string TypeName {
			get { return EffectName; }
		}

		public override string Description {
			get { return "Transmits RDS Data"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Guid[] Dependencies {
			get { return new Guid[] { }; }
		}



		public override Vixen.Sys.ParameterSignature Parameters {
			get
			{
				return new ParameterSignature(
						new ParameterSpecification("Title", typeof(string)) ,
						new ParameterSpecification("Artist", typeof(string)) 
						);
			}
		}
	}
}

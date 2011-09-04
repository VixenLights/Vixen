using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.ModuleTemplate;

namespace TestTemplate {
	public class OutputControllerTemplateModule : ModuleTemplateModuleDescriptorBase {
		private Guid _typeId = new Guid("{D7C0DF55-C3E2-416e-AC23-BF3BCEFDCAEE}");

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(OutputControllerTemplate); }
		}

		override public Type ModuleDataClass {
			get { return typeof(OutputControllerTemplateData); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Output controller template"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}

		//public override Guid[] Dependencies {
		//    get {
		//        return new[] {
		//            new Guid("{989A88B6-9348-466c-A50F-321FAFD2183A}"),
		//            new Guid("{5E867382-36E4-45a3-A4CA-A220081D1167}")
		//        };
		//    }
		//}
	}
}

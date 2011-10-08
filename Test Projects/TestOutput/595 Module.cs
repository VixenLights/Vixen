using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace TestOutput {
    public class _595_Module : OutputModuleDescriptorBase {
        private Guid _typeId = new Guid("{989A88B6-9348-466c-A50F-321FAFD2183A}");

		override public Guid TypeId {
            get { return _typeId; }
        }

		override public Type ModuleClass {
            get { return typeof(_595); }
        }

		override public string Author {
            get { return ""; }
        }

		override public string TypeName {
            get { return "Test Olsen 595"; }
        }

		override public string Description {
            get { return ""; }
        }

		override public string Version {
            get { return ""; }
        }
	}
}

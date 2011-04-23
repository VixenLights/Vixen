using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace TestOutput {
    public class _595_Module : IOutputModuleDescriptor {
        static internal Guid _typeId = new Guid("{989A88B6-9348-466c-A50F-321FAFD2183A}");

        public Guid TypeId {
            get { return _typeId; }
        }

        public Type ModuleClass {
            get { return typeof(_595); }
        }

        public Type ModuleDataClass {
            get { return null; }
        }

        public string Author {
            get { return ""; }
        }

        public string TypeName {
            get { return "Olsen 595"; }
        }

        public string Description {
            get { return ""; }
        }

        public string Version {
            get { return ""; }
        }

		public string FileName { get; set; }
		public string ModuleTypeName { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;

namespace GenericTimer {
    public class Module : ITimingModuleDescriptor {
        // Catch anything that doesn't specify a timing source.
        static internal Guid _typeId = Guid.Empty;

        public Guid TypeId {
            get { return _typeId; }
        }

        public Type ModuleClass {
            get { return typeof(Timer); }
        }

        public Type ModuleDataClass {
            get { return null; }
        }

        public string Author {
            get { throw new NotImplementedException(); }
        }

        public string TypeName {
            get { return "Generic timer"; }
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

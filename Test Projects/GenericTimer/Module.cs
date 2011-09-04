using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;

namespace GenericTimer {
    public class Module : TimingModuleDescriptorBase {
        // Catch anything that doesn't specify a timing source.
        private Guid _typeId = Guid.Empty;

        override public Guid TypeId {
            get { return _typeId; }
        }

		override public Type ModuleClass {
            get { return typeof(Timer); }
        }

		override public string Author {
            get { throw new NotImplementedException(); }
        }

		override public string TypeName {
            get { return "Generic timer"; }
        }

		override public string Description {
            get { throw new NotImplementedException(); }
        }

		override public string Version {
            get { throw new NotImplementedException(); }
        }
	}
}

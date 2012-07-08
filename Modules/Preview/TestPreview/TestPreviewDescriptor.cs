using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Preview;

namespace TestPreview {
	public class TestPreviewDescriptor : PreviewModuleDescriptorBase {
		private Guid _typeId = new Guid("{07A5BCCB-0646-405a-B583-F604D58D91E2}");

		public override string TypeName {
			get { return "Test Preview"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(TestPreviewModule); }
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
	}
}

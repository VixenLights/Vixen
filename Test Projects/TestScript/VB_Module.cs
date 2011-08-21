using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Script;

namespace TestScript {
	public class VB_Module : ScriptModuleDescriptorBase {
		private Guid _typeId = new Guid("{413B76C5-1FD6-4a49-BE2A-CF36BDCA7D59}");

		override public string Language {
			get { return "VB"; }
		}

		override public string FileExtension {
			get { return ".vb"; }
		}

		override public Type SkeletonGenerator {
			get { return typeof(VB_Skeleton); }
		}

		override public Type FrameworkGenerator {
			get { return typeof(VB_Framework); }
		}

		override public Type CodeProvider {
			get { return typeof(VB_CodeProvider); }
		}

		override public string TypeName {
			get { return "VB script"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(VB); }
		}

		override public Type ModuleDataClass {
			get { return null; }
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
	}
}

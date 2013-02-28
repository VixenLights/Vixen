using System;
using Vixen.Module.Script;

namespace VixenModules.Script.VB {
	public class VB_Descriptor : ScriptModuleDescriptorBase {
		private Guid _typeId = new Guid("{413B76C5-1FD6-4a49-BE2A-CF36BDCA7D59}");

		override public string LanguageName {
			get { return "VB"; }
		}

		override public string FileExtension {
			get { return ".vb"; }
		}

		override public Type SkeletonGenerator {
			get { return typeof(VB_Skeleton); }
		}

		override public Type FrameworkGenerator {
			get { return typeof(VB_ScriptFramework); }
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
			get { return typeof(VB_Module); }
		}

		override public string Author {
			get { return "Vixen Team"; }
		}

		override public string Description {
			get { return "Implementation of the Visual Basic language for scripted sequences."; }
		}

		override public string Version {
			get { return "1.0"; }
		}
	}
}

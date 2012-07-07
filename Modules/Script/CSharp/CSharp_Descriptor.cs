using System;
using Vixen.Module.Script;

namespace CSharp {
	public class CSharp_Descriptor : ScriptModuleDescriptorBase {
		private Guid _typeId = new Guid("{2D17E4A5-8B05-4a3d-810E-4F6724777799}");

		override public string LanguageName {
			get { return "C#"; }
		}

		override public string FileExtension {
			get { return ".cs"; }
		}

		override public Type SkeletonGenerator {
			get { return typeof(CSharp_Skeleton); }
		}

		override public Type FrameworkGenerator {
			get { return typeof(CSharp_ScriptFramework); }
		}

		override public Type CodeProvider {
			get { return typeof(CSharp_CodeProvider); }
		}

		override public string TypeName {
			get { return "C# script"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(CSharp_Module); }
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

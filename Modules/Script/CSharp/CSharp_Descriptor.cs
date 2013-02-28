using System;
using Vixen.Module.Script;

namespace VixenModules.Script.CSharp {
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
			get { return "Vixen Team"; }
		}

		override public string Description {
			get { return "Implementation of the C# language for scripted sequences."; }
		}

		override public string Version {
			get { return "1.0"; }
		}
	}
}

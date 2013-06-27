using System;
using Vixen.Module.Script;

namespace VixenModules.Script.CSharp
{
	public class CSharp_Descriptor : ScriptModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{2D17E4A5-8B05-4a3d-810E-4F6724777799}");

		public override string LanguageName
		{
			get { return "C#"; }
		}

		public override string FileExtension
		{
			get { return ".cs"; }
		}

		public override Type SkeletonGenerator
		{
			get { return typeof (CSharp_Skeleton); }
		}

		public override Type FrameworkGenerator
		{
			get { return typeof (CSharp_ScriptFramework); }
		}

		public override Type CodeProvider
		{
			get { return typeof (CSharp_CodeProvider); }
		}

		public override string TypeName
		{
			get { return "C# script"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (CSharp_Module); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Implementation of the C# language for scripted sequences."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
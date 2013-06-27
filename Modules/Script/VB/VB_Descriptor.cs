using System;
using Vixen.Module.Script;

namespace VixenModules.Script.VB
{
	public class VB_Descriptor : ScriptModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{413B76C5-1FD6-4a49-BE2A-CF36BDCA7D59}");

		public override string LanguageName
		{
			get { return "VB"; }
		}

		public override string FileExtension
		{
			get { return ".vb"; }
		}

		public override Type SkeletonGenerator
		{
			get { return typeof (VB_Skeleton); }
		}

		public override Type FrameworkGenerator
		{
			get { return typeof (VB_ScriptFramework); }
		}

		public override Type CodeProvider
		{
			get { return typeof (VB_CodeProvider); }
		}

		public override string TypeName
		{
			get { return "VB script"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (VB_Module); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Implementation of the Visual Basic language for scripted sequences."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
using System;
using Vixen.Module.App;
using VixenModules.App.CustomPropEditor;

namespace VixenModules.App.InputEffectRouter
{
	public class CustomPropEditorDescriptor : AppModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{DC844055-E978-47FE-8359-DBCD5B568E80}");

		public override string TypeName
		{
			get { return "Custom Prop Editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Enables creation of custom props for Previews"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof(CustomPropEditorModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(CustomPropEditorData); }
		}
	}
}
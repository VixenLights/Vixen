using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.EffectEditor;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Sys;
using VixenModules.Effect.CustomValue;
using VixenModules.EffectEditor.ColorTypeEditor;

namespace VixenModules.EffectEditor.CustomValueEditor
{
	public class CustomValueEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("178be00f-10d6-4853-87ab-b588dc38cc00");

		public override string TypeName
		{
			get { return "Custom Value Editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(CustomValueEditorModule); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Editor for the Custom Value effect."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid EffectTypeId
		{
			get { return CustomValueDescriptor.TypeGuid; }
		}

		public override Type[] ParameterSignature
		{
			get { return null; }
		}
	}
	



	public class CustomValueEditorModule : EffectEditorModuleInstanceBase
	{
		public override IEffectEditorControl CreateEditorControl()
		{
			return new CustomValueEditorControl();
		}
	}
}

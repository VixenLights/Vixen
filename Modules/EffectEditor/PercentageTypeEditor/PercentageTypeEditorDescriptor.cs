using System;
using Vixen.Common.ValueTypes;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.PercentageTypeEditor
{
	public class PercentageTypeEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{2547BAD1-F016-4DBA-B968-6677A76964DF}");

		public override string TypeName
		{
			get { return "Percentage Editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (PercentageTypeEditorModule); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Editor for values between 0 and 1."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid EffectTypeId
		{
			get { return Guid.Empty; }
		}

		public override Type[] ParameterSignature
		{
			get { return new[] {typeof (Percentage)}; }
		}
	}
}
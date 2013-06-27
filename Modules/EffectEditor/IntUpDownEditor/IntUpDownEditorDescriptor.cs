using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.IntUpDownEditor
{
	public class IntUpDownEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{DD3532DA-D613-4A94-93FC-2772FBA658C4}");

		public override string TypeName
		{
			get { return "Integer editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (IntUpDownEditorModule); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Editor for integer values.  Max value of 32,768."; }
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
			get { return new[] {typeof (int)}; }
		}
	}
}
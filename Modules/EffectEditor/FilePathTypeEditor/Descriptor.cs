using System;
using Common.ValueTypes;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.FilePathTypeEditor
{
	public class Descriptor : EffectEditorModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{12D611CC-11DF-46D9-8C34-5D5EAABA498B}");

		public override string TypeName
		{
			get { return "File Path Editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Module); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "File path selection"; }
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
			get { return new[] {typeof (FilePath)}; }
		}
	}
}